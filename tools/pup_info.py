#!/usr/bin/env python3
"""
pup_info.py - Print the header and entry table of a PS3UPDAT.PUP.

Useful for confirming a PUP is what you think it is before decrypting it, and
for spotting layout changes. Optionally extracts one entry raw.

Usage:
    python pup_info.py PS3UPDAT.PUP
    python pup_info.py PS3UPDAT.PUP --extract 768 --out update_files.tar

Note: extracting an entry yields the *encrypted* SCE package. For the ROS
`content` blob PS3DumpChecker consumes, use `coreos_decrypt.py` instead, which
walks the PUP and decrypts the CoreOS package end-to-end.

The CoreOS package appears in two places in a modern retail PUP:
  - top-level entry 0x200, and
  - inside `update_files.tar` (entry 0x300).
The two blobs differ in wrapping but decrypt to the same 7,340,000-byte ROS
payload. `coreos_decrypt.py` walks the tar, matching the console's install
path.

Exit codes: 0 ok, 1 error.
"""

import argparse
import struct
import sys

MAGIC = b"SCEUF"
HEADER_SIZE = 0x30
ENTRY_SIZE = 0x20

# Entry ids seen in retail PUPs.
KNOWN_ENTRIES = {
    0x100: "version.txt",
    0x101: "license.xml",
    0x103: "promo_flags.txt",
    0x200: "CORE_OS_PACKAGE.pkg (top-level)",
    0x201: "UPDATE_FILES.pkg",
    0x202: "spkg_hdr.tar",
    0x300: "update_files.tar (contains CORE_OS_PACKAGE.pkg)",
    0x501: "CORE_OS_PACKAGE.pkg digests",
    0x601: "CORE_OS_PACKAGE.pkg signature",
}


def read_header(fh):
    fh.seek(0)
    raw = fh.read(HEADER_SIZE)
    if len(raw) != HEADER_SIZE:
        raise ValueError("file is too small to be a PUP")
    if not raw.startswith(MAGIC):
        raise ValueError("not a PUP (magic is %r, expected %r)" % (raw[:5], MAGIC))
    pkg_ver, img_ver, count, hdr_len, data_len = struct.unpack(">QQQQQ", raw[8:HEADER_SIZE])
    return {
        "package_version": pkg_ver,
        "image_version": img_ver,
        "file_count": count,
        "header_length": hdr_len,
        "data_length": data_len,
    }


def read_entries(fh, count):
    fh.seek(HEADER_SIZE)
    raw = fh.read(ENTRY_SIZE * count)
    if len(raw) != ENTRY_SIZE * count:
        raise ValueError("truncated entry table")
    entries = []
    for i in range(count):
        entry_id, offset, length, _pad = struct.unpack(
            ">QQQQ", raw[i * ENTRY_SIZE:(i + 1) * ENTRY_SIZE])
        entries.append({"id": entry_id, "offset": offset, "length": length})
    return entries


def extract(fh, entry, out_path):
    fh.seek(entry["offset"])
    remaining = entry["length"]
    with open(out_path, "wb") as out:
        while remaining:
            chunk = fh.read(min(1 << 20, remaining))
            if not chunk:
                raise ValueError("unexpected EOF while extracting")
            out.write(chunk)
            remaining -= len(chunk)


def main(argv=None):
    ap = argparse.ArgumentParser(description="Inspect a PS3 PUP.")
    ap.add_argument("pup")
    ap.add_argument("--extract", type=lambda s: int(s, 0), metavar="ID",
                    help="entry id to extract, e.g. 512 or 0x200")
    ap.add_argument("--out", help="output path for --extract")
    args = ap.parse_args(argv)

    if args.extract is not None and not args.out:
        ap.error("--out is required with --extract")

    try:
        with open(args.pup, "rb") as fh:
            hdr = read_header(fh)
            entries = read_entries(fh, hdr["file_count"])

            print("file            : %s" % args.pup)
            print("package version : %d" % hdr["package_version"])
            print("image version   : %d" % hdr["image_version"])
            print("file count      : %d" % hdr["file_count"])
            print("header length   : 0x%X" % hdr["header_length"])
            print("data length     : 0x%X" % hdr["data_length"])
            print()
            print("%-8s %-14s %-14s %s" % ("ID", "OFFSET", "LENGTH", "LIKELY CONTENT"))
            for entry in entries:
                print("%-8d %-14d %-14d %s"
                      % (entry["id"], entry["offset"], entry["length"],
                         KNOWN_ENTRIES.get(entry["id"], "")))

            if args.extract is not None:
                match = next((e for e in entries if e["id"] == args.extract), None)
                if match is None:
                    print("\nerror: entry %d not found" % args.extract, file=sys.stderr)
                    return 1
                extract(fh, match, args.out)
                print("\nextracted entry %d (%d bytes) to %s"
                      % (match["id"], match["length"], args.out))
    except (OSError, ValueError, struct.error) as exc:
        print("error: %s" % exc, file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
