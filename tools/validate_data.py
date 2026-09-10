#!/usr/bin/env python3
"""
validate_data.py - Sanity-check PS3DumpChecker's data files.

hashlist.xml and config.xml drive every check the app performs, and they are
edited by hand on each firmware release. A malformed entry is only noticed at
runtime, on a user's dump. This validates them up front.

Checks performed on hashlist.xml:
  - well-formed XML
  - every <hash> has a name and a 32-hex-digit MD5 body
  - no duplicate MD5s (two names claiming the same ROS)
  - no duplicate names
  - size attributes parse as hex and match the expected ROS size
  - every <offset> has fsize/type/name/size and a hex offset body
  - each hash's type has a matching <type> block

Checks performed on config.xml:
  - well-formed XML
  - offset/size attributes parse as hex where present

Usage:
    python validate_data.py
    python validate_data.py --hashlist path --config path

Exit codes: 0 all good, 1 problems found.
"""

import argparse
import os
import re
import sys
import xml.etree.ElementTree as ET

ROS_SIZE = 0x6FFFE0
MD5_RE = re.compile(r"^[0-9A-F]{32}$")

HERE = os.path.dirname(os.path.abspath(__file__))
REPO = os.path.dirname(HERE)
DEFAULT_HASHLIST = os.path.join(REPO, "src", "PS3DumpChecker", "hashlist.xml")
DEFAULT_CONFIG = os.path.join(REPO, "src", "PS3DumpChecker", "config.xml")


class Report(object):
    def __init__(self):
        self.errors = []
        self.warnings = []

    def error(self, msg):
        self.errors.append(msg)

    def warn(self, msg):
        self.warnings.append(msg)


def parse_hex(value):
    try:
        return int(value, 16)
    except (TypeError, ValueError):
        return None


def validate_hashlist(path, rep):
    if not os.path.exists(path):
        rep.error("hashlist not found: %s" % path)
        return
    try:
        root = ET.parse(path).getroot()
    except ET.ParseError as exc:
        rep.error("hashlist.xml is not well-formed: %s" % exc)
        return

    declared_types = set()
    for type_el in root.iter("type"):
        name = type_el.get("name")
        if not name:
            rep.error("<type> without a name attribute")
            continue
        if name in declared_types:
            rep.error("duplicate <type name=\"%s\">" % name)
        declared_types.add(name)

    seen_md5 = {}
    seen_name = {}
    count = 0

    for type_el in root.iter("type"):
        type_name = type_el.get("name") or "?"
        for hash_el in type_el.iter("hash"):
            count += 1
            name = hash_el.get("name")
            body = (hash_el.text or "").strip().upper()

            if not name:
                rep.error("<hash> without a name (type %s, md5 %s)"
                          % (type_name, body or "empty"))
                name = "<unnamed>"

            if not body:
                rep.error("%s: empty MD5 body" % name)
            elif not MD5_RE.match(body):
                rep.error("%s: body is not a 32-hex-digit MD5: %r" % (name, body))
            else:
                if body in seen_md5:
                    rep.error("duplicate MD5 %s shared by %r and %r"
                              % (body, seen_md5[body], name))
                else:
                    seen_md5[body] = name

            if name in seen_name:
                rep.error("duplicate hash name %r" % name)
            else:
                seen_name[name] = body

            size_attr = hash_el.get("size")
            if size_attr is None:
                rep.error("%s: <hash> missing size attribute" % name)
            else:
                size = parse_hex(size_attr)
                if size is None:
                    rep.error("%s: size=%r is not hex" % (name, size_attr))
                elif size != ROS_SIZE:
                    rep.error("%s: size 0x%X differs from the ROS size 0x%X"
                              % (name, size, ROS_SIZE))

            patched = hash_el.get("patched")
            if patched is not None and patched.lower() not in ("true", "false"):
                rep.error("%s: patched=%r must be true or false" % (name, patched))

    for offset_el in root.iter("offset"):
        name = offset_el.get("name") or "<unnamed offset>"
        for attr in ("fsize", "type", "name", "size"):
            if offset_el.get(attr) is None:
                rep.error("%s: <offset> missing %s attribute" % (name, attr))

        fsize = offset_el.get("fsize")
        if fsize is not None:
            try:
                int(fsize)
            except ValueError:
                rep.error("%s: fsize=%r is not a decimal integer" % (name, fsize))

        if offset_el.get("size") is not None and parse_hex(offset_el.get("size")) is None:
            rep.error("%s: size=%r is not hex" % (name, offset_el.get("size")))

        body = (offset_el.text or "").strip()
        if not body:
            rep.error("%s: <offset> has no offset value" % name)
        elif parse_hex(body) is None:
            rep.error("%s: offset body %r is not hex" % (name, body))

        otype = offset_el.get("type")
        if otype and otype not in declared_types:
            rep.error("%s: references type %r with no matching <type> block"
                      % (name, otype))

    print("hashlist.xml : %d hash entries, %d types, %d offsets"
          % (count, len(declared_types), len(list(root.iter("offset")))))


def validate_config(path, rep):
    if not os.path.exists(path):
        rep.error("config not found: %s" % path)
        return
    try:
        root = ET.parse(path).getroot()
    except ET.ParseError as exc:
        rep.error("config.xml is not well-formed: %s" % exc)
        return

    hex_attrs = ("offset", "size", "regionstart", "regionsize", "ldrsize")
    elements = 0
    for el in root.iter():
        elements += 1
        for attr in hex_attrs:
            raw = el.get(attr)
            if raw is None:
                continue
            if parse_hex(raw) is None:
                rep.error("<%s> %s=%r is not hex" % (el.tag, attr, raw))

    print("config.xml   : %d elements" % elements)


def main(argv=None):
    ap = argparse.ArgumentParser(description="Validate PS3DumpChecker data files.")
    ap.add_argument("--hashlist", default=DEFAULT_HASHLIST)
    ap.add_argument("--config", default=DEFAULT_CONFIG)
    args = ap.parse_args(argv)

    rep = Report()
    validate_hashlist(args.hashlist, rep)
    validate_config(args.config, rep)
    print()

    for msg in rep.warnings:
        print("warning: %s" % msg)
    for msg in rep.errors:
        print("error:   %s" % msg, file=sys.stderr)

    if rep.errors:
        print("\nFAILED: %d error(s), %d warning(s)"
              % (len(rep.errors), len(rep.warnings)), file=sys.stderr)
        return 1
    print("OK: no errors, %d warning(s)" % len(rep.warnings))
    return 0


if __name__ == "__main__":
    sys.exit(main())
