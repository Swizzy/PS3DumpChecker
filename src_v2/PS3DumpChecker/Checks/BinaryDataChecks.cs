namespace PS3DumpChecker.Checks {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;

    internal class BinaryDataChecks {
        private readonly Dictionary <string, Types.MultiBinaryData> _multiEntries = new Dictionary <string, Types.MultiBinaryData>();
        private readonly List <Types.SingleBinaryData> _singleEntries = new List <Types.SingleBinaryData>();

        private BinaryDataChecks(string file) : this(File.OpenRead(file)) { }

        private BinaryDataChecks(Stream stream) {
            using (var xml = XmlReader.Create(stream)) {
                bool single = false, multi = false;
                while (xml.Read()) {
                    if (!xml.IsStartElement())
                        continue;
                    switch (xml.Name.ToLower()) {
                        case "single":
                            single = true;
                            multi = false;
                            break;
                        case "multi":
                            multi = true;
                            single = false;
                            break;
                        case "entry":
                            if (single) {
                                var name = xml["name"];
                                var ascii = !string.IsNullOrEmpty(xml["ascii"]) && xml["ascii"].Equals("true", StringComparison.CurrentCultureIgnoreCase);
                                var disablePatcher = !string.IsNullOrEmpty(xml["disablepatch"]) && xml["disablepatch"].Equals("true", StringComparison.CurrentCultureIgnoreCase);
                                int offset;
                                if (!int.TryParse(xml["offset"], NumberStyles.HexNumber, null, out offset))
                                    break; // no need to continue, it's not valid anyways!
                                xml.Read();
                                if (string.IsNullOrEmpty(xml.Value))
                                    break; // again no need to continue... invalid!
                                _singleEntries.Add(new Types.SingleBinaryData(xml.Value, offset, name, ascii, disablePatcher));
                            }
                            if (multi) {
                                if (string.IsNullOrEmpty(xml["name"]))
                                    break;
                                var name = xml["name"];
                                var disablePatcher = !string.IsNullOrEmpty(xml["disablepatch"]) && xml["disablepatch"].Equals("true", StringComparison.CurrentCultureIgnoreCase);
                                if (!_multiEntries.ContainsKey(name)) {
                                    var ascii = !string.IsNullOrEmpty(xml["ascii"]) && xml["ascii"].Equals("true", StringComparison.CurrentCultureIgnoreCase);
                                    int offset;
                                    if (!int.TryParse(xml["offset"], NumberStyles.HexNumber, null, out offset))
                                        break; // no need to continue, it's not valid anyways!
                                    xml.Read();
                                    if (string.IsNullOrEmpty(xml.Value))
                                        break; // again no need to continue... invalid!
                                    _multiEntries.Add(name, new Types.MultiBinaryData(xml.Value, offset, ascii, disablePatcher));
                                }
                                else {
                                    xml.Read();
                                    if (string.IsNullOrEmpty(xml.Value))
                                        break; // again no need to continue... invalid!
                                    _multiEntries[name].Data.Add(new Types.MultiBinaryData.MultiBinaryDataSub(xml.Value, disablePatcher));
                                }
                            }
                            break;
                    }
                }
            }
        }

        public List <Types.CheckResults> DoBinaryDataChecks(ref byte[] data) {
            var ret = new List <Types.CheckResults>();
            foreach (var entry in _singleEntries) {
                var results = new Types.CheckResults {
                    CheckCount = ret.Count
                };
                DoBinaryCheck(ref data, entry, ref results);
                if (results.CheckCount > ret.Count) // Check if we actually checked something, if not... ignore this one...
                    ret.Add(results);
            }
            foreach (var entry in _multiEntries) {
                var results = new Types.CheckResults {
                    CheckCount = ret.Count
                };
                DoBinaryCheck(ref data, entry, ref results);
                if (results.CheckCount > ret.Count) // Check if we actually checked something, if not... ignore this one...
                    ret.Add(results);
            }
            return ret;
        }

        private static void DoBinaryCheck(ref byte[] data, KeyValuePair <string, Types.MultiBinaryData> checkData, ref Types.CheckResults results) {
            var cData = checkData.Value;
            results.Name = checkData.Key;
            if (cData.Offset + cData.Data[0].Data.Length > data.Length || data.Length == 0 || cData.Data[0].Data.Length == 0)
                return; // Nothing to check
            var sb = new StringBuilder();
            foreach (var chkdata in cData.Data) {
                var tmp = chkdata.Data;
                sb.AppendLine(cData.AsciiOut ? Common.GetAsciiString(ref tmp) : Common.BytesToHex(ref tmp));
                if (!results.Result) {
                    results.Result = Common.CompareArrays(ref data, ref tmp, cData.Offset, tmp.Length);
                    results.DisablePatcher = chkdata.DisablePatcher;
                }
                if (string.IsNullOrEmpty(results.Actual))
                    results.Actual = cData.AsciiOut ? Common.GetAsciiString(ref data, cData.Offset, tmp.Length) : Common.BytesToHex(ref data, cData.Offset, tmp.Length);
            }
            results.Expected = string.Format("Offset: {0:X}\r\n{1}", cData.Offset, sb);
            results.CheckCount++;
        }

        private static void DoBinaryCheck(ref byte[] data, Types.SingleBinaryData checkData, ref Types.CheckResults results) {
            if (checkData.Offset + checkData.Data.Length > data.Length || data.Length == 0 || checkData.Data.Length == 0)
                return; // Nothing to check
            var tmp = checkData.Data;
            results.Name = checkData.Name;
            results.Expected = string.Format("Offset: {0:X}\r\n{1}", checkData.Offset, !checkData.AsciiOut ? Common.BytesToHex(ref tmp, checkData.Offset, tmp.Length) : Common.GetAsciiString(ref tmp));
            results.Result = Common.CompareArrays(ref data, ref tmp, checkData.Offset, 0, tmp.Length);
            results.Actual = !checkData.AsciiOut ? Common.BytesToHex(ref data, checkData.Offset, tmp.Length) : Common.GetAsciiString(ref data, checkData.Offset, tmp.Length);
            results.CheckCount++;
        }
    }
}