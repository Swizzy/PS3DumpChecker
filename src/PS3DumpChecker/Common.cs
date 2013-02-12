using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PS3DumpChecker
{
    public class Common {

        public static readonly Dictionary<long, TypeData> Types = new Dictionary<long, TypeData>();

        public static event EventHandler<StatusEventArgs> StatusUpdate;

        public static event EventHandler ListUpdate;

        public static void SendStatus(string msg) {
            if (StatusUpdate != null)
                StatusUpdate(null, new StatusEventArgs(msg));
        }

        public static byte[] HexToArray(string input) {
            if (String.IsNullOrEmpty(input))
                return null;
            var ret = new byte[input.Length/2];
            for (var i = 0; i < input.Length; i += 2)
                ret[i/2] = Byte.Parse(input.Substring(i, 2), NumberStyles.HexNumber);
            return ret;
        }

        public static readonly Dictionary<int, PartsObject> PartList =
            new Dictionary<int, PartsObject>();

        public struct PartsObject {
            public string Name;
            public string ExpectedString;
            public string ActualString;
            public bool Result;
        }

        public struct SKUEntry 
        {
            public int SKUKey;
            public string Name;
            public string Data;
            public string Type;
            public bool Warn;
            public string WarnMsg;
        }

        public struct SKUDataEntry
        {
            public string Type;
            public uint Offset;
            public uint Size;
        }

        public struct ImgInfo
        {
            public bool IsOk;
            public int BadCount;
            public bool Reversed;
            public string Status;
            public string SKUModel;
        }

        public static void AddItem(int key, PartsObject data)
        {
            PartList.Add(key, data);
            ListUpdate(String.Empty, new EventArgs());
        }

        public static void AddBad(ref ImgInfo ret)
        {
            ret.IsOk = false;
            ret.BadCount++;
        }

        public struct StatCheck
        {
            public readonly double Low;
            public readonly double High;
            public StatCheck(double low, double high)
            {
                Low = low;
                High = high;
            }
        }

        public struct BinCheck
        {
            public readonly bool IsMulti;
            public readonly long Offset;
            public readonly string Expected;
            public readonly Holder<List<MultiBin>> ExpectedList;
            public readonly string Description;
            public readonly bool Asciiout;
            public BinCheck(List<MultiBin> expectedList, bool isMulti, string offset, string description, string asciiout, string expected = "")
            {
                IsMulti = isMulti;
                if (!Int64.TryParse(offset, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out Offset))
                    Offset = 0;
                ExpectedList = expectedList != null ? new Holder<List<MultiBin>>(expectedList) : null;
                Expected = Regex.Replace(expected, @"\s+", "");
                Description = description;
                Asciiout = (asciiout != null && asciiout.Equals("true", StringComparison.CurrentCultureIgnoreCase));
            }
        }

        public struct TypeData
        {
            public readonly Holder<string> Name;
            public readonly Holder<Dictionary<string, Holder<StatCheck>>> Statlist;
            public readonly Holder<string> StatDescription;
            public readonly Holder<Dictionary<string, Holder<BinCheck>>> Bincheck;
            public readonly Holder<List<SKUEntry>> SKUList;
            public readonly Holder<List<SKUDataEntry>> SKUDataList;
            public readonly Holder<List<DataCheck>> DataCheckList;
            public TypeData(bool isnew = true)
            {
                SKUList = new Holder<List<SKUEntry>>(new List<SKUEntry>());
                SKUDataList = new Holder<List<SKUDataEntry>>(new List<SKUDataEntry>());
                Statlist = new Holder<Dictionary<string, Holder<StatCheck>>>(new Dictionary<string, Holder<StatCheck>>());
                Bincheck = new Holder<Dictionary<string, Holder<BinCheck>>>(new Dictionary<string, Holder<BinCheck>>());
                DataCheckList = new Holder<List<DataCheck>>(new List<DataCheck>());
                Name = new Holder<string>("");
                StatDescription = new Holder<string>("");
                if (isnew)
                    GC.Collect();
            }
        }

        public struct MultiBin
        {
            public readonly string Id;
            public readonly string Expected;

            public MultiBin(string expected, string id)
            {
                Expected = expected;
                Id = id;
            }
        }

        public struct DataCheck {
            public int DataKey;
            public long Offset;
            public long Size;
            public long LdrSize;
            public string Name;
            public Dictionary<string, double> ThresholdList;
        }

        public static bool SwapBytes(ref byte[] data)
        {
            if ((data.Length % 2) != 0)
                return false;
            for (var i = 0; i < data.Length; i += 2)
            {
                var b = data[i];
                data[i] = data[i + 1];
                data[i + 1] = b;
            }
            GC.Collect();
            return true;
        }

        public static string GetDataReadable(IEnumerable<byte> data)
        {
            var ret = "";
            var count = 0;
            foreach (var b in data)
            {
                ret += String.Format("{0:X2} ", b);
                if ((count % 0x10) == 0 && count > 0x10)
                    ret += Environment.NewLine;
                count++;
            }
            return ret;
        }

        public static string GetDataReadable(string input)
        {
            var count = 0;
            return GetDataReadable(input, ref count);
        }

        public static string GetDataReadable(string input, ref int count)
        {
            var ret = "";
            foreach (var c in input)
            {
                if (c.ToString(CultureInfo.InvariantCulture) == " ")
                    continue;
                if ((count % 0x2) != 0)
                    ret += String.Format("{0} ", c);
                else
                    ret += String.Format("{0}", c);
                if ((count % 0x20) == 0 && count > 0x20)
                    ret += Environment.NewLine;
                count++;
            }
            return ret;
        }

        public static string GetDataForTest(IEnumerable<byte> data)
        {
            var ret = "";
            foreach (var b in data)
                ret += String.Format("{0:X2}", b);
            return ret;
        }

        public static uint GetLdrSize(ref byte[] data)
        {
            SwapBytes(ref data);
            var tmpval = BitConverter.ToUInt16(data, 0);
            var ret = (uint)tmpval * 0x10;
            return ret + 0x40;
        }
    }

    public class StatusEventArgs : EventArgs
    {
        public StatusEventArgs(string msg) { Status = msg; }
        public readonly string Status;
    }

    public class Holder<T>
    {
        public T Value;
        public Holder(T value) { Value = value; }
    }
}