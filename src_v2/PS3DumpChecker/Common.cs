namespace PS3DumpChecker {
    using System;
    using System.Globalization;
    using System.Text;

    internal static class Common {
        private static readonly char[] HexCharTable = {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            'A',
            'B',
            'C',
            'D',
            'E',
            'F'
        };

        public static byte[] HexToBytes(string data) {
            if (string.IsNullOrEmpty(data))
                return null;
            var ret = new byte[data.Length / 2];
            for (var i = 0; i < data.Length; i += 2)
                ret[i / 2] = byte.Parse(data.Substring(i, 2), NumberStyles.HexNumber);
            return ret;
        }

        public static string BytesToHex(byte[] data, int offset = 0, int length = -1) { return BytesToHex(ref data, offset, length); }

        public static string BytesToHex(ref byte[] data, int offset = 0, int length = -1) {
            if (length <= 0)
                length = data.Length - offset;
            var c = new char[length * 2];
            var i = offset;
            for (var p = 0; i < offset + length;) {
                var d = data[i++];
                c[p++] = HexCharTable[d / 0x10];
                c[p++] = HexCharTable[d % 0x10];
            }
            return new string(c);
        }

        public static bool CompareArrays(ref byte[] a1, ref byte[] a2, int a1Offset = -1, int a2Offset = -1, int length = -1) {
            if (a1 == a2)
                return true; // They're the same damn thing!
            if (length <= -1)
                length = a1.Length;
            if (a1.Length < a1Offset + length || a2.Length < a2Offset + length)
                throw new ArgumentOutOfRangeException();
            if (a1Offset <= 0)
                a1Offset = 0;
            if (a2Offset <= 0)
                a2Offset = 0;
            for (var i = 0; i < length; i++) {
                if (a1[i + a1Offset] != a2[i + a2Offset])
                    return false; // They're not equal, no point in continuing...
            }
            return true;
        }

        public static bool CheckByteFill(ref byte[] array, byte data, int offset = -1, int length = -1) {
            if (length <= -1)
                length = array.Length - offset;
            if (array.Length < offset + length)
                throw new ArgumentOutOfRangeException();
            if (offset < 0)
                offset = 0;
            for (var i = offset; i < offset + length; i++) {
                if (array[i] != data)
                    return false; // It's not what we wanted to find :(
            }
            return true;
        }

        public static bool CheckByteBand(ref byte[] array, byte data, int offset = -1, int length = -1, int count = 4) {
            if (length <= -1)
                length = array.Length - offset;
            if (array.Length < offset + length)
                throw new ArgumentOutOfRangeException();
            if (offset < 0)
                offset = 0;
            var tmp = 0;
            for (var i = offset; i < offset + length; i++) {
                if (array[i] == data) {
                    tmp++;
                    if (count == tmp)
                        return false; // It's not what we wanted to find :(
                }
                else
                    tmp = 0;
            }
            return true;
        }

        public static string GetAsciiString(ref byte[] data, int offset, int length) { return Encoding.ASCII.GetString(data, offset, length).Trim(); }

        public static string GetAsciiString(ref byte[] data) { return Encoding.ASCII.GetString(data).Trim(); }

        public static void SwapBytes(ref byte[] data) {
            if ((data.Length % 2) != 0)
                throw new ArgumentException("data.Length should be dividable by 2!");
            for (var i = 0; i < data.Length; i += 2) {
                var b = data[i];
                data[i] = data[i + 1];
                data[i + 1] = b;
            }
        }

        public static ushort Swap(ushort x) { return (ushort)((x & 0x00FF) << 8 | (x & 0xFF00) >> 8); }

        public static uint GetLdrSize(ref byte[] data, int offset) {
            var tmpval = Swap(BitConverter.ToUInt16(data, offset));
            var ret = (uint) tmpval * 0x10;
            return ret + 0x40;
        }
    }
}