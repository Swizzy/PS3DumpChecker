# tools

Helpers for adding support for a new PS3 firmware release.

Adding a firmware has always been a manual process: unpack two PUPs, decrypt
the CoreOS out of each, MD5 the ROS region, hand-edit `hashlist.xml`, and drop
the new payload in as `patch.bin`. These scripts cover the parts that can be
checked by a machine, so a typo in a hex digit fails here instead of on a
user's dump.

Python 3. No third-party packages.

## ros_hash.py

Computes the ROS MD5 that `hashlist.xml` stores, and prints the XML entries
ready to paste.

```
python tools/ros_hash.py ofw_content cfw_content --version 4.93
python tools/ros_hash.py content --name "4.93 CEX"
python tools/ros_hash.py content --name "4.93 CEX Patched (Evilnat based)" --patched
```

Input is a *decrypted* CoreOS payload -- the `content` file from an unpacked
`CORE_OS_PACKAGE.pkg`, not the `.pkg` itself and not the PUP. It is the same
7,340,000-byte blob that ships as `Patches/patch.bin`.

The hash is the MD5 of the first `0x6FFFE0` bytes, matching
`HashCheck.CheckHash`. The byte-swapped MD5 is printed alongside it: NOR dumps
store the region swapped and NAND dumps do not, so seeing both makes a
mismatch obvious rather than silent.

Sanity check -- this reproduces the 4.92 entry already in `hashlist.xml`:

```
$ python tools/ros_hash.py src/PS3DumpChecker/Patches/patch.bin \
      --name "4.92 CEX Patched (Evilnat based)" --patched
  MD5         : 36BD44795F06B59EECBDAAD6982BE426
```

## validate_data.py

Checks `hashlist.xml` and `config.xml` before they ship. Run it after editing
either file.

```
python tools/validate_data.py
```

Catches duplicate MD5s, duplicate entry names, malformed hashes, non-hex size
and offset attributes, bad `patched` values, offsets missing attributes, and
hashes referencing a type that was never declared. Exits non-zero on error.

## build_nofsm_patch.py

Rebuilds the exact `patch.bin` that PS3Xploit flash-writer 4.93 writes to
flash. Uses only public inputs (OFW 4.93 CoreOS + `flash493.P3T` from
`aldostools/flashwriter`). Result matches
MD5 `AFE831050C31EFB381F9BE4098F1834C` byte-for-byte, i.e. the same ROS
`pyPS3patcher` and the PS3 Toolset will ship for 4.93 -- so the three tools
stay in sync and the patched ROS is the safe non-Cobra variant, not the
Cobra CFW CoreOS.

```
python tools/build_nofsm_patch.py ofw_content flash493.P3T patch.bin
```

Get the inputs with:

- `python tools/coreos_decrypt.py PS3UPDAT_493_OFW.PUP ofw_out`
  (yields `ofw_out/content`)
- `curl -LO https://raw.githubusercontent.com/aldostools/flashwriter/main/493/flash493.P3T`

The transformation is a byte splice:

    patched_ros = OFW[0 : 0x1D0] + P3T[:] + OFW[0x1D0 + len(P3T) :]

The P3T is a partial-ROS overlay whose body mirrors the OFW ROS from
offset 0x1D0 with only a few SELFs actually modified
(`sdk_version`, `spu_pkg_rvk_verifier.self`, `default.spp`, `lv1.self`).

## coreos_decrypt.py

Decrypts `CORE_OS_PACKAGE.pkg` out of a PUP and writes the `content` blob --
the 7,340,000-byte ROS payload `ros_hash.py` needs.

```
python tools/coreos_decrypt.py PS3UPDAT.PUP out_dir
python tools/coreos_decrypt.py --extract-selfs PS3UPDAT.PUP out_dir
```

Requires `pycryptodome` (`python -m pip install --user pycryptodome`). No other
external tools -- reimplements `pupunpack`, `unpkg`, and `cosunpkg` from
fail0verflow/ps3tools in Python, using the public retail PKG keys embedded in
the script.

`--extract-selfs` writes each SELF from the decrypted content next to
`content`. Useful for sanity-checking the extraction (each file should start
with the `SCE\0` magic).

## Getting the decrypted CoreOS

1. Get both PUPs and verify them:
   - official `PS3UPDAT.PUP` for the firmware, from Sony's update CDN
   - the matching **CEX** Evilnat CFW -- not `noBD`, `noBT` or `noBD+noBT`,
     which drop modules and therefore hash differently
2. `python tools/coreos_decrypt.py PS3UPDAT.PUP out_dir` on each. The
   `out_dir/content` file is what `ros_hash.py` consumes.
3. `python tools/ros_hash.py ofw_out/content cfw_out/content --version X.XX`.

`pup_info.py` prints a PUP's entry table if you want to confirm the layout
before decrypting.

## Adding a firmware, end to end

1. Verify both PUP downloads against their published checksums. The Evilnat
   `.rar` ships an `md5.txt`; the official PUP has a SHA-256 published
   alongside it.
2. `python tools/coreos_decrypt.py OFW.PUP ofw_out` and again for the CFW.
3. `python tools/ros_hash.py ofw_out/content cfw_out/content --version X.XX`
4. Paste both entries at the top of `<type name="ROS">` in
   `src/PS3DumpChecker/hashlist.xml`, newest first.
5. Copy the CFW `content` to `src/PS3DumpChecker/Patches/patch.bin` and update
   `Patches/patch_info.txt` to `noFSM X.XX (Evilnat based)`.
6. `python tools/validate_data.py`
7. Bump `AssemblyVersion` and `AssemblyFileVersion` in
   `Properties/AssemblyInfo.cs`, and add a changelog entry.
8. Build Release_Embedded. The post-build step refreshes
   `Latest Compiled Version/`, which is what the in-app updater serves --
   `default.cfg`, `default.hashlist` and their `.md5` files must be regenerated
   or existing installs will keep fetching the old data.
