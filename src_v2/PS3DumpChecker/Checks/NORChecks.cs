namespace PS3DumpChecker.Checks {
    using System;
    using System.IO;

    internal class NORChecks {
        public static bool IsReversed(byte[] data) {
            var tmp = BitConverter.ToUInt32(data, 0x200);
            switch(tmp) {
                case 0x00494649: // IFI (Human Readable)
                    return false;
                case 0x49004946: // I FI (PS3 Readable)
                    return true;
                default:
                    throw new InvalidDataException();
            }
        }
    }
}