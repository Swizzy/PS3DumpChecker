#!/usr/bin/env python3
"""
build_nofsm_patch.py - Reproduce the PS3Xploit flash-writer 4.93 patched
CoreOS ROS from public inputs.

Given an OFW 4.93 CoreOS payload (decrypted with `coreos_decrypt.py`) and
the `flash493.P3T` shipped by `aldostools/flashwriter`, this reconstructs
byte-for-byte the same 4.93 CEX Patched (Evilnat-based) ROS that
PS3Xploit's noFSM flow writes to flash. It is the exact blob PS3DumpChecker
recognises as `4.93 CEX Patched (Evilnat based)`,
MD5 = `AFE831050C31EFB381F9BE4098F1834C`.

Transformation (reverse-engineered from the flash-writer ROP flow):

    patched_ros = OFW[0 : 0x1D0] + P3T[:] + OFW[0x1D0 + len(P3T) :]

The P3T file is a partial ROS overlay whose body is mostly identical to
the OFW ROS beginning at offset 0x1D0. Only four SELFs are actually
modified in it (sdk_version, spu_pkg_rvk_verifier.self, default.spp,
lv1.self). The remaining bytes match the OFW verbatim, so splicing the
whole P3T over the OFW at 0x1D0 yields the fully-patched CoreOS.

Usage:
    python build_nofsm_patch.py OFW_CONTENT flash493.P3T OUT_PATH

Where:
    OFW_CONTENT   the decrypted 4.93 OFW CoreOS `content` file
                  (7,340,000 bytes = 0x6FFFE0)
    flash493.P3T  the PS3Xploit flash-writer 4.93 overlay
                  (from aldostools/flashwriter/493/flash493.P3T)
    OUT_PATH      where to write the reconstructed patch.bin

Requires no third-party packages. Exit codes: 0 ok, 1 error.
"""

from __future__ import annotations

import argparse
import hashlib
import sys
from pathlib import Path

OVERLAY_OFFSET = 0x1D0
EXPECTED_ROS_SIZE = 0x6FFFE0
TARGET_MD5 = "AFE831050C31EFB381F9BE4098F1834C"


def build_patch(ofw: bytes, p3t: bytes) -> bytes:
    if len(ofw) != EXPECTED_ROS_SIZE:
        raise ValueError(
            "OFW ROS size 0x%X != expected 0x%X" % (len(ofw), EXPECTED_ROS_SIZE)
        )
    end = OVERLAY_OFFSET + len(p3t)
    if end > len(ofw):
        raise ValueError(
            "P3T overlay end 0x%X exceeds ROS size 0x%X" % (end, len(ofw))
        )
    out = bytearray(ofw)
    out[OVERLAY_OFFSET:end] = p3t
    return bytes(out)


def main(argv: list[str] | None = None) -> int:
    ap = argparse.ArgumentParser(
        description="Rebuild the 4.93 CEX Patched (Evilnat) ROS from OFW + P3T."
    )
    ap.add_argument("ofw_content", type=Path,
                    help="decrypted OFW 4.93 CoreOS content (0x6FFFE0 bytes)")
    ap.add_argument("p3t", type=Path,
                    help="flash493.P3T from aldostools/flashwriter")
    ap.add_argument("out", type=Path, help="output patch.bin path")
    args = ap.parse_args(argv)

    try:
        ofw = args.ofw_content.read_bytes()
        p3t = args.p3t.read_bytes()
        patched = build_patch(ofw, p3t)
    except (OSError, ValueError) as exc:
        print("error: %s" % exc, file=sys.stderr)
        return 1

    args.out.write_bytes(patched)
    md5 = hashlib.md5(patched).hexdigest().upper()

    print("OFW  : %s  (0x%X bytes)" % (args.ofw_content, len(ofw)))
    print("P3T  : %s  (0x%X bytes)" % (args.p3t, len(p3t)))
    print("OUT  : %s  (0x%X bytes)" % (args.out, len(patched)))
    print("MD5  : %s" % md5)
    print("WANT : %s" % TARGET_MD5)

    if md5 != TARGET_MD5:
        print("MISMATCH", file=sys.stderr)
        return 1
    print("MATCH")
    return 0


if __name__ == "__main__":
    sys.exit(main())
