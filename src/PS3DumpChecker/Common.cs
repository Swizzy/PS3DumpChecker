namespace PS3DumpChecker {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    internal static class Common {
        private static readonly char[] HexCharTable = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        internal static readonly Dictionary<long, TypeData> Types = new Dictionary<long, TypeData>();
        internal static HashCheck Hashes;
        internal static readonly Dictionary<int, PartsObject> PartList = new Dictionary<int, PartsObject>();

        public static string dmpname;
        public static bool swapstate;
        public static bool chkresult;

                
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
            return true;
        }

        public static string GetDataReadable(IEnumerable<byte> data) {
            var ret = "";
            var count = 0;
            var datalength = 0;
            foreach (var b in data) { datalength++; }
            foreach(var b in data) {
                ret += String.Format("{0:X2} ", b);
                if (((count + 1) % 0x10) == 0 && count >= 0xF && (count + 1) != datalength)
                    ret = ret.Trim() + Environment.NewLine;
                count++;
            }
            //ret = ret.Trim();
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
                if (((count + 1) % 0x20) == 0 && count >= 0x1E && (count + 1) != input.Length) 
                    ret = ret.Trim() + Environment.NewLine;
                count++;
            }
            //ret = ret.Trim();
            return ret;
        }

        public static string GetDataForTest(IList<byte> data) {
            var c = new char[data.Count * 2];
            var i = 0;
            for (var p = 0; i < data.Count; )
            {
                var d = data[i++];
                c[p++] = HexCharTable[d / 0x10];
                c[p++] = HexCharTable[d % 0x10];
            }
            return new string(c);
        }

        public static string GetDataForTest(ref byte[] data, int offset, int length)
        {
            var c = new char[length * 2];
            var i = offset;
            for (var p = 0; i < offset + length; )
            {
                var d = data[i++];
                c[p++] = HexCharTable[d / 0x10];
                c[p++] = HexCharTable[d % 0x10];
            }
            return new string(c);
        }

        public static uint GetLdrSize(ref byte[] data) {
            SwapBytes(ref data);
            var tmpval = BitConverter.ToUInt16(data, 0);
            var ret = (uint) tmpval * 0x10;
            return ret + 0x40;
        }

        public static uint GetSizefrom(ref byte[] data)
        {
            SwapBytes(ref data);
            return BitConverter.ToUInt16(data, 0); ;
        }

        public static ulong Swap(ulong x) { return x << 56 | x << 40 & 0xff000000000000 | x << 24 & 0xff0000000000 | x << 8 & 0xff00000000 | x >> 8 & 0xff000000 | x >> 24 & 0xff0000 | x >> 40 & 0xff00 | x >> 56; }

        public static uint Swap(uint x) { return (x & 0x000000FF) << 24 | (x & 0x0000FF00) << 8 | (x & 0x00FF0000) >> 8 | (x & 0xFF000000) >> 24; }

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
                if(!string.IsNullOrEmpty(description)) {
                    description = description.Trim();
                    description += Environment.NewLine;
                    description = description.Replace("\\n", Environment.NewLine);
                }
                else
                    description = "";
                Description = description.Trim();
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
            internal bool DisablePatch;
            internal string ROS0Version;
            internal string ROS1Version;
            internal bool IsPatched;
        }

        #endregion

        #region Nested type: MultiBin

        public struct MultiBin {
            internal readonly string Expected;
            internal readonly string Id;
            internal readonly bool DisablePatch;

            internal MultiBin(string expected, string id, bool disablepatch = false) {
                Expected = expected;
                Id = id;
                DisablePatch = disablepatch;
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

        #region Nested type: RepCheckData

        internal sealed class RepCheckData {
            internal string Name;
            internal int Offset;
            internal readonly List<int> FoundAt = new List<int>();
        }

        #endregion

        #region Nested type: DataMatch

        internal sealed class DataMatch
        {
            internal string Name;
            internal int Offset;
            internal int Length;
            internal int SequenceRepetitions;
            internal bool DisableDisplay;
        }

        #endregion

        #region Nested type: DataMatchID

        internal sealed class DataMatchID
        {
            internal string Name;
            internal List<DataMatch> Data = new List<DataMatch>();
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

        public struct DataFillEntry {
#pragma warning disable 649
            internal int Offset;
            internal int Length;
            internal byte Data;
            internal string Name;
            internal int RegionStart;
            internal int RegionSize;
            internal int Sizefrom;
            internal int LdrSize;
            internal int vtrmentrycount_offset;
#pragma warning restore 649
        }

        #region Nested type: TypeData

        internal class TypeData {
            internal readonly Holder<Dictionary<string, Holder<BinCheck>>> Bincheck;
            internal readonly Holder<List<DataCheck>> DataCheckList;
            internal readonly Holder<string> Name;
            internal readonly Holder<Dictionary<string, Holder<DataMatchID>>> DataMatchList;
            internal readonly Holder<Dictionary<string, Holder<RepCheckData>>> RepCheck;
            internal readonly Holder<List<SKUDataEntry>> SKUDataList;
            internal readonly Holder<List<SKUEntry>> SKUList;
            internal readonly Holder<string> StatDescription;
            internal readonly Holder<Dictionary<string, Holder<StatCheck>>> Statlist;
            internal readonly Holder<List<DataFillEntry>> DataFillEntries;
            internal int ROS0Offset;
            internal int ROS1Offset;

            internal TypeData() {
                DataFillEntries = new Holder<List<DataFillEntry>>(new List<DataFillEntry>());
                DataMatchList = new Holder<Dictionary<string, Holder<DataMatchID>>>(new Dictionary<string, Holder<DataMatchID>>());
                RepCheck = new Holder<Dictionary<string, Holder<RepCheckData>>>(new Dictionary<string, Holder<RepCheckData>>());
                SKUList = new Holder<List<SKUEntry>>(new List<SKUEntry>());
                SKUDataList = new Holder<List<SKUDataEntry>>(new List<SKUDataEntry>());
                Statlist = new Holder<Dictionary<string, Holder<StatCheck>>>(new Dictionary<string, Holder<StatCheck>>());
                Bincheck = new Holder<Dictionary<string, Holder<BinCheck>>>(new Dictionary<string, Holder<BinCheck>>());
                DataCheckList = new Holder<List<DataCheck>>(new List<DataCheck>());
                Name = new Holder<string>("");
                StatDescription = new Holder<string>("");
            }
        }

        #endregion
    }
}