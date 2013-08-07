namespace PS3DumpChecker {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    internal static class Common {
        internal static readonly Dictionary<long, TypeData> Types = new Dictionary<long, TypeData>();
        internal static HashCheck Hashes;
        internal static readonly Dictionary<int, PartsObject> PartList = new Dictionary<int, PartsObject>();

        public static event EventHandler<StatusEventArgs> StatusUpdate;

        public static event EventHandler ListUpdate;

        internal static void SendStatus(string msg) {
            if(StatusUpdate != null)
                StatusUpdate(null, new StatusEventArgs(msg));
        }

        internal static byte[] HexToArray(string input) {
            if(String.IsNullOrEmpty(input))
                return null;
            var ret = new byte[input.Length / 2];
            for(var i = 0; i < input.Length; i += 2)
                ret[i / 2] = Byte.Parse(input.Substring(i, 2), NumberStyles.HexNumber);
            return ret;
        }

        internal static void AddItem(int key, PartsObject data) {
            PartList.Add(key, data);
            ListUpdate(String.Empty, new EventArgs());
        }

        internal static void AddBad(ref ImgInfo ret) {
            ret.IsOk = false;
            ret.BadCount++;
        }

        public static bool SwapBytes(ref byte[] data) {
            if((data.Length % 2) != 0)
                return false;
            for(var i = 0; i < data.Length; i += 2) {
                var b = data[i];
                data[i] = data[i + 1];
                data[i + 1] = b;
            }
            GC.Collect();
            return true;
        }

        public static string GetDataReadable(IEnumerable<byte> data) {
            var ret = "";
            var count = 0;
            foreach(var b in data) {
                ret += String.Format("{0:X2} ", b);
                if((count % 0x10) == 0 && count > 0x10)
                    ret += Environment.NewLine;
                count++;
            }
            return ret;
        }

        public static string GetDataReadable(string input) {
            var count = 0;
            return GetDataReadable(input, ref count);
        }

        public static string GetDataReadable(string input, ref int count) {
            var ret = "";
            foreach(var c in input) {
                if(c.ToString(CultureInfo.InvariantCulture) == " ")
                    continue;
                if((count % 0x2) != 0)
                    ret += String.Format("{0} ", c);
                else
                    ret += String.Format("{0}", c);
                if((count % 0x20) == 0 && count > 0x20)
                    ret += Environment.NewLine;
                count++;
            }
            return ret;
        }

        public static string GetDataForTest(IEnumerable<byte> data) {
            var ret = "";
            foreach(var b in data)
                ret += String.Format("{0:X2}", b);
            return ret;
        }

        public static uint GetLdrSize(ref byte[] data) {
            SwapBytes(ref data);
            var tmpval = BitConverter.ToUInt16(data, 0);
            var ret = (uint) tmpval * 0x10;
            return ret + 0x40;
        }

        #region Nested type: BinCheck

        public struct BinCheck {
            internal readonly bool Asciiout;
            internal readonly string Description;
            internal readonly string Expected;
            internal readonly Holder<List<MultiBin>> ExpectedList;
            internal readonly bool IsMulti;
            internal readonly long Offset;

            internal BinCheck(List<MultiBin> expectedList, bool isMulti, string offset, string description, string asciiout, string expected = "") {
                IsMulti = isMulti;
                if(!Int64.TryParse(offset, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out Offset))
                    Offset = 0;
                ExpectedList = expectedList != null ? new Holder<List<MultiBin>>(expectedList) : null;
                Expected = Regex.Replace(expected, @"\s+", "");
                Description = description;
                Asciiout = (asciiout != null && asciiout.Equals("true", StringComparison.CurrentCultureIgnoreCase));
            }
        }

        #endregion

        #region Nested type: DataCheck

        public struct DataCheck {
            public int DataKey;
            public long LdrSize;
            public string Name;
            public long Offset;
            public long Size;
            public Dictionary<string, double> ThresholdList;
        }

        #endregion

        #region Nested type: ImgInfo

        public struct ImgInfo {
            internal int BadCount;
            internal string FileName;
            internal bool IsOk;
            internal string MinVer;
            internal bool Reversed;
            internal string SKUModel;
            internal string Status;
        }

        #endregion

        #region Nested type: MultiBin

        public struct MultiBin {
            internal readonly string Expected;
            internal readonly string Id;

            internal MultiBin(string expected, string id) {
                Expected = expected;
                Id = id;
            }
        }

        #endregion

        #region Nested type: PartsObject

        internal struct PartsObject {
            internal string ActualString;
            internal string ExpectedString;
            internal string Name;
            internal bool Result;
        }

        #endregion

        #region Nested type: SKUDataEntry

        internal struct SKUDataEntry {
            internal uint Offset;
            internal uint Size;
            internal string Type;
        }

        #endregion

        #region Nested type: SKUEntry

        internal struct SKUEntry {
            internal string Data;
            internal string MinVer;
            internal string Name;
            internal int SKUKey;
            internal string Type;
            internal bool Warn;
            internal string WarnMsg;
        }

        #endregion

        #region Nested type: StatCheck

        public struct StatCheck {
            internal readonly double High;
            internal readonly double Low;

            internal StatCheck(double low, double high) {
                Low = low;
                High = high;
            }
        }

        #endregion

        #region Nested type: TypeData

        internal struct TypeData {
            internal readonly Holder<Dictionary<string, Holder<BinCheck>>> Bincheck;
            internal readonly Holder<List<DataCheck>> DataCheckList;
            internal readonly Holder<string> Name;
            internal readonly Holder<List<SKUDataEntry>> SKUDataList;
            internal readonly Holder<List<SKUEntry>> SKUList;
            internal readonly Holder<string> StatDescription;
            internal readonly Holder<Dictionary<string, Holder<StatCheck>>> Statlist;

            internal TypeData(bool isnew = true) {
                SKUList = new Holder<List<SKUEntry>>(new List<SKUEntry>());
                SKUDataList = new Holder<List<SKUDataEntry>>(new List<SKUDataEntry>());
                Statlist = new Holder<Dictionary<string, Holder<StatCheck>>>(new Dictionary<string, Holder<StatCheck>>());
                Bincheck = new Holder<Dictionary<string, Holder<BinCheck>>>(new Dictionary<string, Holder<BinCheck>>());
                DataCheckList = new Holder<List<DataCheck>>(new List<DataCheck>());
                Name = new Holder<string>("");
                StatDescription = new Holder<string>("");
                if(isnew)
                    GC.Collect();
            }
        }

        #endregion
    }
}