#!/usr/bin/env python3
"""
ros_hash.py - Compute PS3DumpChecker ROS hashes from a decrypted CoreOS.

Given a decrypted CoreOS payload (the "content" file produced by unpacking
CORE_OS_PACKAGE.pkg from a PS3UPDAT.PUP), this prints the MD5 that
PS3DumpChecker stores in hashlist.xml, plus the ready-to-paste XML entries.

The hash is computed exactly the way HashCheck.CheckHash does it:
the first ROS_SIZE (0x6FFFE0) bytes of the region are MD5'd. For NOR the
region is byte-swapped first; NAND is not swapped. Both dump layouts share
the same CoreOS payload, so a single hash covers both -- but this tool
reports the swapped variant too, so a mismatch is visible rather than silent.

Usage:
    python ros_hash.py content --name "4.93 CEX"
    python ros_hash.py content --name "4.93 CEX Patched (Evilnat based)" --patched
    python ros_hash.py ofw_content cfw_content --version 4.93

Exit codes: 0 ok, 1 error.
"""

import argparse
import hashlib
import os
import sys
from xml.sax.saxutils import quoteattr

# Size of the ROS region hashed by PS3DumpChecker (HashCheck.cs / hashlist.xml).
ROS_SIZE = 0x6FFFE0

# Region offsets per dump type, from hashlist.xml <offsets>.
OFFSETS = {
    "NOR":  {"size": 0x1000000,  "ros0": 0xC0010, "ros1": 0x7C0010},
    "NAND": {"size": 0x10000000, "ros0": 0xC0030, "ros1": 0x7C0020},
}


def swap_bytes(data):
    """Byte-swap 16-bit words, mirroring Common.SwapBytes."""
    if len(data) % 2:
        raise ValueError("data length must be even to byte-swap")
    out = bytearray(data)
    out[0::2], out[1::2] = data[1::2], data[0::2]
    return bytes(out)


def read_ros(path):
    """Read the first ROS_SIZE bytes of a decrypted CoreOS payload."""
    size = os.path.getsize(path)
    if size < ROS_SIZE:
        raise ValueError(
            "%s is %d bytes, need at least %d (0x%X). "
            "Is this the decrypted 'content' file?" % (path, size, ROS_SIZE, ROS_SIZE)
        )
    with open(path, "rb") as fh:
        data = fh.read(ROS_SIZE)
    if len(data) != ROS_SIZE:
        raise ValueError("short read on %s" % path)
    return data, size


def md5_upper(data):
    return hashlib.md5(data).hexdigest().upper()


def describe(path):
    data, size = read_ros(path)
    plain = md5_upper(data)
    swapped = md5_upper(swap_bytes(data))
    return {
        "path": path,
        "size": size,
        "trailing": size - ROS_SIZE,
        "md5": plain,
        "md5_swapped": swapped,
    }


def xml_entry(name, md5, patched):
    attrs = 'name=%s size="%X"' % (quoteattr(name), ROS_SIZE)
    if patched:
        attrs += ' patched="true"'
    return '    <hash %s>\n      %s\n    </hash>' % (attrs, md5)


def report(info, label):
    print("== %s ==" % label)
    print("  file        : %s" % info["path"])
    print("  file size   : %d bytes" % info["size"])
    print("  hashed      : %d bytes (0x%X)" % (ROS_SIZE, ROS_SIZE))
    print("  trailing    : %d bytes not hashed" % info["trailing"])
    print("  MD5         : %s" % info["md5"])
    print("  MD5 swapped : %s" % info["md5_swapped"])
    print()


def main(argv=None):
    ap = argparse.ArgumentParser(
        description="Compute PS3DumpChecker ROS hashes from decrypted CoreOS payload(s).",
        epilog="Obtain the 'content' file by unpacking CORE_OS_PACKAGE.pkg "
               "(PUP entry 512) with PUAD GUI, pupunpack, or scetool.",
    )
    ap.add_argument("content", nargs="+",
                    help="decrypted CoreOS payload(s); with --version pass OFW then CFW")
    ap.add_argument("--name", help="hash entry name, e.g. '4.93 CEX'")
    ap.add_argument("--patched", action="store_true",
                    help="mark the entry as a patched (CFW) ROS")
    ap.add_argument("--version",
                    help="firmware version, e.g. 4.93; emits both OFW and patched "
                         "entries from two inputs")
    args = ap.parse_args(argv)

    if len(args.content) > 2:
        ap.error("pass at most two files (OFW and CFW)")
    if len(args.content) == 2 and not args.version:
        ap.error("--version is required when passing two files")

    print("PS3DumpChecker ROS hash tool")
    print("ROS region size: 0x%X (%d bytes)" % (ROS_SIZE, ROS_SIZE))
    for kind, off in OFFSETS.items():
        print("  %-4s dump 0x%X: ROS0 @ 0x%X, ROS1 @ 0x%X"
              % (kind, off["size"], off["ros0"], off["ros1"]))
    print()

    try:
        infos = [describe(p) for p in args.content]
    except (OSError, ValueError) as exc:
        print("error: %s" % exc, file=sys.stderr)
        return 1

    if len(infos) == 2:
        report(infos[0], "OFW")
        report(infos[1], "CFW (patched)")
        if infos[0]["md5"] == infos[1]["md5"]:
            print("error: OFW and CFW hash identically -- inputs are the same file",
                  file=sys.stderr)
            return 1
        entries = [
            xml_entry("%s CEX Patched (Evilnat based)" % args.version,
                      infos[1]["md5"], True),
            xml_entry("%s CEX" % args.version, infos[0]["md5"], False),
        ]
    else:
        report(infos[0], "CoreOS")
        if not args.name:
            print("error: --name is required for a single input", file=sys.stderr)
            return 1
        entries = [xml_entry(args.name, infos[0]["md5"], args.patched)]

    print("Paste into hashlist.xml, at the top of <type name=\"ROS\">:")
    print()
    for entry in entries:
        print(entry)
    print()
    print("The same payload is also the ROS patch: copy it to "
          "src/PS3DumpChecker/Patches/patch.bin and update patch_info.txt.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
