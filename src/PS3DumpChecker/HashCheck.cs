namespace PS3DumpChecker {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Xml;

    internal sealed class HashCheck {
        public static string ROS0Ver;
        public static string ROS1Ver;
        public static bool LastIsPatched;
        private readonly Dictionary<string, Holder<Dictionary<string, HashListObject>>> _hashlist = new Dictionary<string, Holder<Dictionary<string, HashListObject>>>();
        public readonly Dictionary<long, Holder<List<HashListObject>>> Offsets = new Dictionary<long, Holder<List<HashListObject>>>();

        public HashCheck(string filename) { LoadHashList(filename); }

        private void LoadHashList(string filename) {
            Common.SendStatus(string.Format("Parsing {0}", filename));
            _hashlist.Clear();
            Offsets.Clear();
            var type = "";
            using(var xml = XmlReader.Create(filename)) {
                while(xml.Read()) {
                    if(!xml.IsStartElement())
                        continue;
                    switch(xml.Name.ToLower()) {
                        case "offset":
                            var fsize = xml["fsize"];
                            HashListObject tmp;
                            if(fsize != null) {
                                var size = long.Parse(fsize);
                                if(!Offsets.ContainsKey(size))
                                    Offsets.Add(size, new Holder<List<HashListObject>>(new List<HashListObject>()));
                                tmp = new HashListObject {
                                    Name = xml["name"],
                                    Type = xml["type"]
                                };
                                fsize = xml["size"];
                                if(fsize == null)
                                    throw new InvalidOperationException("size doesn't exist");
                                tmp.Size = int.Parse(fsize, NumberStyles.AllowHexSpecifier);
                                xml.Read();
                                fsize = xml.Value;
                                if(fsize == null)
                                    throw new InvalidOperationException("no offset");
                                tmp.Offset = int.Parse(fsize, NumberStyles.AllowHexSpecifier);
                                Offsets[size].Value.Add(tmp);
                            }
                            break;
                        case "type":
                            type = xml["name"];
                            if(type != null && !_hashlist.ContainsKey(type))
                                _hashlist.Add(type, new Holder<Dictionary<string, HashListObject>>(new Dictionary<string, HashListObject>()));
                            break;
                        case "hash":
                            if(string.IsNullOrEmpty(type))
                                throw new InvalidOperationException("No type specified...");
                            tmp = new HashListObject {
                                Name = xml["name"],
                                Type = xml["type"],
                                ROSVersion = xml["rosver"],
                                Patched = !string.IsNullOrEmpty(xml["patched"]) && xml["patched"].Equals("true", StringComparison.CurrentCultureIgnoreCase)
                            };
                            //fsize = xml["size"];
                            //long.TryParse(fsize, NumberStyles.AllowHexSpecifier, null, out tmp.Size);
                            xml.Read();
                            _hashlist[type].Value.Add(xml.Value.ToUpper().Trim(), tmp);
                            break;
                    }
                }
            }
            Common.SendStatus("Parsing done!");
        }

        public string CheckHash(ref byte[] data, long offset, long size, bool reversed, string type, string name, out string hash) {
            LastIsPatched = false;
            if(name.StartsWith("ros0", StringComparison.InvariantCultureIgnoreCase))
                ROS0Ver = "";
            else if (name.StartsWith("ros1", StringComparison.InvariantCultureIgnoreCase))
                ROS1Ver = "";
            var tmp = new byte[size];
            Buffer.BlockCopy(data, (int) offset, tmp, 0, tmp.Length);
            if(reversed)
                SwapBytes(ref tmp);
            tmp = MD5.Create().ComputeHash(tmp);
            hash = "";
            foreach(var b in tmp)
                hash += b.ToString("X2");
            if(_hashlist[type].Value.ContainsKey(hash)) {
                if (name.StartsWith("ros0", StringComparison.InvariantCultureIgnoreCase))
                    ROS0Ver = _hashlist[type].Value[hash].ROSVersion;
                if (name.StartsWith("ros1", StringComparison.InvariantCultureIgnoreCase))
                    ROS1Ver = _hashlist[type].Value[hash].ROSVersion;
                LastIsPatched = _hashlist[type].Value[hash].Patched;

            }
            return _hashlist[type].Value.ContainsKey(hash) ? _hashlist[type].Value[hash].Name : "";
        }

        private static void SwapBytes(ref byte[] data) {
            if((data.Length % 2) != 0)
                return;
            for(var i = 0; i < data.Length; i += 2) {
                var b = data[i];
                data[i] = data[i + 1];
                data[i + 1] = b;
            }
        }

        #region Nested type: HashListObject

        internal struct HashListObject {
            public string Name;
            public long Offset;
            public string ROSVersion;
            public long Size;
            public string Type;
            public bool Patched;
        }

        #endregion
    }
}