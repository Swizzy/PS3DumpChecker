#!/usr/bin/env python3
"""
coreos_decrypt.py - Decrypt CORE_OS_PACKAGE.pkg from a PS3 PUP.

Reimplements the SCE PKG decrypt pipeline from fail0verflow/ps3tools (unpkg,
cosunpkg) in pure Python. Ported from wargio/ps3tools tools.c, GPLv2.

Input: PS3UPDAT.PUP (retail or Evilnat CFW).
Output: `content` file (decrypted CoreOS layout) + individual SELFs of the
CoreOS package alongside it.

The `content` file is the 7,340,000-byte ROS payload the game hashes and
patches. That is what ros_hash.py needs and what ships as patch.bin.

Usage:
    python coreos_decrypt.py PS3UPDAT.PUP out_dir
    python coreos_decrypt.py --extract-selfs PS3UPDAT.PUP out_dir

Exit codes: 0 ok, 1 error.
"""

import argparse
import os
import struct
import sys
import zlib

try:
    from Crypto.Cipher import AES
except ImportError:
    print("error: pycryptodome missing. run: python -m pip install --user pycryptodome",
          file=sys.stderr)
    sys.exit(1)


# PKG keys from public scetool keys.conf. Two revisions cover every retail PUP
# that ships CORE_OS_PACKAGE; sce_decrypt_header brute-forces the list until
# the meta header decrypts to the expected zero pattern.
PKG_KEYS = [
    {
        "revision": 0x00,
        "erk": bytes.fromhex("A97818BD193A67A16FE83A855E1BE9FB5640938D4DBCB2CB52C5A2F8B02B1031"),
        "riv": bytes.fromhex("4ACEF01224FBEEDF8245F8FF10211E6E"),
    },
    {
        "revision": 0x01,
        "erk": bytes.fromhex("F8F99006F1C007D5D0B1909E9566E0E70B569399FC3394A811809FDB5CAE92CD"),
        "riv": bytes.fromhex("59D28DB4ADDFB40B7D768BC9667C67B1"),
    },
]


def be(data, offset, length):
    return int.from_bytes(data[offset:offset + length], "big")


def be16(d, o): return be(d, o, 2)
def be32(d, o): return be(d, o, 4)
def be64(d, o): return be(d, o, 8)


# -------- PUP layer (pupunpack) --------

PUP_MAGIC = b"SCEUF"
PUP_HEADER = 0x30
PUP_ENTRY = 0x20

PUP_ENTRY_NAMES = {
    0x100: "version.txt",
    0x101: "license.xml",
    0x102: "promo_flags.txt",
    0x103: "update_flags.txt",
    0x104: "patch_build.txt",
    0x200: "ps3swu.self",
    0x201: "vsh.tar",
    0x202: "dots.txt",
    0x203: "patch_data.pkg",
    0x300: "update_files.tar",
    0x501: "spkg_hdr.tar",
    0x601: "ps3swu2.self",
}


def parse_pup(path):
    """Return (data, entries[]) where each entry is dict(id, offset, length)."""
    with open(path, "rb") as fh:
        data = fh.read()

    if not data.startswith(PUP_MAGIC):
        raise ValueError("not a PUP (magic mismatch)")
    if len(data) < PUP_HEADER:
        raise ValueError("truncated PUP header")
    n = be64(data, 0x18)
    table_end = PUP_HEADER + PUP_ENTRY * n
    if n > 0x1000 or table_end > len(data):
        raise ValueError(
            "PUP claims %d entries, table would need 0x%X bytes of a %d-byte file"
            % (n, table_end, len(data)))
    entries = []
    for i in range(n):
        base = PUP_HEADER + PUP_ENTRY * i
        entries.append({
            "id": be64(data, base),
            "offset": be64(data, base + 0x08),
            "length": be64(data, base + 0x10),
        })
    return data, entries


def find_entry(entries, entry_id):
    for e in entries:
        if e["id"] == entry_id:
            return e
    return None


# -------- SPKG layer (unpkg) --------

def aes256cbc_decrypt(key, iv, data):
    return AES.new(key, AES.MODE_CBC, iv).decrypt(data)


def aes128ctr(key, iv, data):
    """AES-128-CTR matching fail0verflow's aes128ctr (be64 nonce increment)."""
    out = bytearray(len(data))
    counter = bytearray(iv)
    ecb = AES.new(key, AES.MODE_ECB)
    keystream = b""
    for i, b in enumerate(data):
        if (i & 0xF) == 0:
            keystream = ecb.encrypt(bytes(counter))
            lo = int.from_bytes(counter[8:16], "big") + 1
            if lo > 0xFFFFFFFFFFFFFFFF:
                lo = 0
                hi = int.from_bytes(counter[0:8], "big") + 1
                counter[0:8] = hi.to_bytes(8, "big")
            counter[8:16] = lo.to_bytes(8, "big")
        out[i] = b ^ keystream[i & 0xF]
    return bytes(out)


def sce_decrypt_header(pkg):
    """Decrypt SCE meta header in place. Returns index of matching key or -1.

    Ported from tools.c sce_decrypt_header. For each candidate key, AES-256-CBC
    the 0x40 bytes at meta_offset+0x20; a matching key leaves bytes
    [0x10..0x20) and [0x30..0x40) all zero.
    """
    meta_offset = be32(pkg, 0x0c)
    header_len = be64(pkg, 0x10)

    matched = -1
    for idx, k in enumerate(PKG_KEYS):
        tmp = aes256cbc_decrypt(k["erk"], k["riv"],
                                bytes(pkg[meta_offset + 0x20:meta_offset + 0x60]))
        if all(b == 0 for b in tmp[0x10:0x20]) and all(b == 0 for b in tmp[0x30:0x40]):
            pkg[meta_offset + 0x20:meta_offset + 0x60] = tmp
            matched = idx
            break
    if matched < 0:
        return -1

    # tools.c does two aes128ctr calls sharing the same iv buffer, whose
    # counter mutates in place -> effectively one continuous CTR stream over
    # [+0x60, +header_len). Match that with a single call here.
    key128 = bytes(pkg[meta_offset + 0x20:meta_offset + 0x30])
    iv = bytes(pkg[meta_offset + 0x40:meta_offset + 0x50])

    meta_len = header_len - meta_offset
    start = meta_offset + 0x60
    end = meta_offset + meta_len
    pkg[start:end] = aes128ctr(key128, iv, bytes(pkg[start:end]))
    return matched


def sce_decrypt_data(pkg):
    """Decrypt every data segment in place using the section table."""
    meta_offset = be32(pkg, 0x0c)
    n_hdr = be32(pkg, meta_offset + 0x60 + 0x0c)
    keytable_base = meta_offset + 0x80 + 0x30 * n_hdr

    for i in range(n_hdr):
        entry = meta_offset + 0x80 + 0x30 * i
        offset = be64(pkg, entry)
        size = be64(pkg, entry + 8)
        keyid = be32(pkg, entry + 0x24)
        ivid = be32(pkg, entry + 0x28)
        if keyid == 0xFFFFFFFF or ivid == 0xFFFFFFFF:
            continue
        key = bytes(pkg[keytable_base + keyid * 0x10:keytable_base + keyid * 0x10 + 0x10])
        iv = bytes(pkg[keytable_base + ivid * 0x10:keytable_base + ivid * 0x10 + 0x10])
        pkg[offset:offset + size] = aes128ctr(key, iv, bytes(pkg[offset:offset + size]))


def unpkg_content(pkg):
    """After decrypt, return the `content` blob (decompressed if needed)."""
    meta_offset = be32(pkg, 0x0c)
    dec_size = be64(pkg, 0x18)

    tmp = meta_offset + 0x80 + 0x30 * 2
    offset = be64(pkg, tmp)
    size = be64(pkg, tmp + 8)
    flag = be32(pkg, tmp + 0x2c)
    size_real = dec_size - 0x80

    raw = bytes(pkg[offset:offset + size])
    if flag == 0x2:
        return zlib.decompress(raw)
    return raw


# -------- CoreOS layer (cosunpkg) --------

def cosunpkg_files(content):
    """Iterate (name, data) tuples from a decrypted CoreOS content blob."""
    n = be32(content, 4)
    for i in range(n):
        entry = 0x10 + 0x30 * i
        offset = be64(content, entry)
        size = be64(content, entry + 8)
        name = content[entry + 0x10:entry + 0x30].rstrip(b"\x00").decode("ascii", "replace")
        yield name, content[offset:offset + size]


# -------- driver --------

def extract_core_os_from_pup(pup_path):
    """Return raw CORE_OS_PACKAGE.pkg bytes from a PUP.

    In retail PUPs the CoreOS package is inside update_files.tar, itself in
    entry 0x300. `unpkg` runs after untarring update_files.tar to get
    CORE_OS_PACKAGE.pkg. For simplicity we do the whole thing inline.
    """
    data, entries = parse_pup(pup_path)
    up = find_entry(entries, 0x300)
    if up is None:
        raise ValueError("PUP has no update_files.tar (entry 0x300)")
    tar = data[up["offset"]:up["offset"] + up["length"]]

    # Minimal tar walker: 512-byte header blocks, name at 0, size octal at 124.
    pos = 0
    while pos + 512 <= len(tar):
        header = tar[pos:pos + 512]
        if header == b"\x00" * 512:
            break
        name = header[0:100].split(b"\x00", 1)[0].decode("ascii", "replace")
        size_field = header[124:136].split(b"\x00", 1)[0].strip()
        size = int(size_field, 8) if size_field else 0
        pos += 512
        if name.endswith("CORE_OS_PACKAGE.pkg"):
            return tar[pos:pos + size]
        pos += (size + 511) & ~511
    raise ValueError("CORE_OS_PACKAGE.pkg not found in update_files.tar")


def decrypt_core_os_pkg(pkg_bytes):
    """Given raw CORE_OS_PACKAGE.pkg, return content blob and matched key rev."""
    pkg = bytearray(pkg_bytes)

    if be16(pkg, 0x0a) != 3:
        raise ValueError("not an SCE PKG (type != 3)")

    rev = sce_decrypt_header(pkg)
    if rev < 0:
        raise ValueError("header decrypt failed: no PKG key matches")
    sce_decrypt_data(pkg)
    return unpkg_content(pkg), PKG_KEYS[rev]["revision"]


def main(argv=None):
    ap = argparse.ArgumentParser(description="Decrypt CORE_OS_PACKAGE.pkg from a PS3 PUP.")
    ap.add_argument("pup")
    ap.add_argument("out")
    ap.add_argument("--extract-selfs", action="store_true",
                    help="also write each SELF from the content blob")
    args = ap.parse_args(argv)

    try:
        print("[1/3] parsing PUP...")
        pkg_bytes = extract_core_os_from_pup(args.pup)
        print("      CORE_OS_PACKAGE.pkg extracted (%d bytes)" % len(pkg_bytes))

        print("[2/3] decrypting SCE PKG...")
        content, rev = decrypt_core_os_pkg(pkg_bytes)
        print("      decrypted with pkg key revision %02x (%d bytes)" % (rev, len(content)))
    except (OSError, ValueError, KeyError, struct.error) as exc:
        print("error: %s" % exc, file=sys.stderr)
        return 1

    print("[3/3] writing outputs...")
    os.makedirs(args.out, exist_ok=True)
    content_path = os.path.join(args.out, "content")
    with open(content_path, "wb") as fh:
        fh.write(content)
    print("      wrote %s (%d bytes)" % (content_path, len(content)))

    if args.extract_selfs:
        out_root = os.path.realpath(args.out)
        for name, blob in cosunpkg_files(content):
            if not name:
                continue
            # Reject path traversal in archive-supplied names: no absolute
            # paths, no drive letters, no separators. CoreOS entries are flat
            # basenames like "lv0" or "lv2_kernel.self".
            if (os.path.isabs(name) or "/" in name or "\\" in name
                    or name in (".", "..") or ":" in name):
                print("      skip %r (unsafe path)" % name, file=sys.stderr)
                continue
            out = os.path.join(args.out, name)
            if not os.path.realpath(out).startswith(out_root + os.sep) \
                    and os.path.realpath(out) != out_root:
                print("      skip %r (escapes out dir)" % name, file=sys.stderr)
                continue
            with open(out, "wb") as fh:
                fh.write(blob)
            print("      wrote %s (%d bytes)" % (out, len(blob)))
    return 0


if __name__ == "__main__":
    sys.exit(main())
