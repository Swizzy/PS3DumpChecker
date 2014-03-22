namespace PS3DumpChecker.Checks {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;

    internal class SKUModels {
        private readonly List <Types.SKUModel> _models = new List <Types.SKUModel>();
        private readonly List <Types.SKUModelOffsets> _nandData = new List <Types.SKUModelOffsets>();
        private readonly List <Types.SKUModelOffsets> _norData = new List <Types.SKUModelOffsets>();

        private SKUModels(string filePath) : this(File.OpenRead(filePath)) { }

        private SKUModels(Stream stream) {
            var key = 0;
            bool nor = false, nand = false;
            using (var xml = XmlReader.Create(stream)) {
                while (xml.Read()) {
                    if (!xml.IsStartElement())
                        continue;
                    switch (xml.Name.ToLower()) {
                        case "nordata":
                            nor = true;
                            nand = false;
                            break;
                        case "nanddata":
                            nand = true;
                            nor = false;
                            break;
                        case "skudataentry":
                            var tmp = new Types.SKUModelOffsets();
                            if (!int.TryParse(xml["offset"], NumberStyles.HexNumber, null, out tmp.Offset))
                                break;
                            if (!int.TryParse(xml["size"], NumberStyles.HexNumber, null, out tmp.Size))
                                break;
                            xml.Read();
                            if (string.IsNullOrEmpty(xml.Value))
                                break;
                            tmp.Type = xml.Value;
                            if (nor)
                                _norData.Add(tmp);
                            else if (nand)
                                _nandData.Add(tmp);
                            break;
                        case "skulist":
                            _models.Add(new Types.SKUModel {
                                SKUKey = ++key,
                                Warn = xml["warn"] != null && xml["warn"].Equals("true", StringComparison.CurrentCultureIgnoreCase),
                                WarnMsg = xml["warnmsg"],
                                Name = xml["Name"],
                                MinVer = xml["minver"]
                            });
                            break;
                        case "skuentry":
                            var data = new Types.SKUModelData {
                                Type = xml["type"]
                            };
                            xml.Read();
                            if (string.IsNullOrEmpty(xml.Value))
                                break;
                            data.Data = Regex.Replace(xml.Value, "\\s+", "");
                            foreach (var skuModel in _models)
                                if (skuModel.SKUKey == key)
                                    skuModel.DataList.Add(data);
                            break;
                    }
                }
            }
        }

        public Types.SKUModel GetSKUModel(ref byte[] data, ref Types.CheckResults results) {
            var sb = new StringBuilder();
            var list = new List<Types.SKUModel>(_models);
            List <Types.SKUModelOffsets> offsetlist;
            switch (data.Length) {
                case 0x1000000:
                    offsetlist = _norData;
                    break;
                case 0x10000000:
                    offsetlist = _nandData;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            foreach (var entry in offsetlist)
            {
                var tmp = entry.Type.Equals("bootldrsize", StringComparison.CurrentCultureIgnoreCase)  ? Common.GetLdrSize(ref data, entry.Offset).ToString("X") : Common.BytesToHex(ref data, entry.Offset, entry.Size);
                sb.AppendLine(string.Format("{0} : {1}", entry.Type, tmp));
                GetFilterList(entry.Type, tmp, ref list);
            }
            results.Actual = sb.ToString();
            return list.Count != 1 ? null : list[0];
        }

        private static void GetFilterList(string type, string value, ref List<Types.SKUModel> list) {
            var list2 = new List <Types.SKUModel>();
            foreach (var skuModel in list) {
                foreach (var skuModelData in skuModel.DataList)
                {
                    if (skuModelData.Type.Equals(type, StringComparison.CurrentCultureIgnoreCase))
                        if (!skuModelData.Data.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                            continue;
                    list2.Add(skuModel);
                }
            }
            list.Clear();
            list.AddRange(list2);
        }
    }
}