namespace PS3DumpChecker {
    using System.Collections.Generic;

    internal abstract class Types {
        #region Nested type: CheckResults

        internal sealed class CheckResults {
            public string Actual;
            public int CheckCount;
            public bool DisablePatcher;
            public string Expected;
            public string Name;
            public bool Result;
        }

        #endregion

        #region Nested type: MultiBinaryData

        public sealed class MultiBinaryData {
            internal readonly bool AsciiOut;
            internal readonly List<MultiBinaryDataSub> Data = new List<MultiBinaryDataSub>();
            internal readonly int Offset;

            public MultiBinaryData(string data, int offset, bool asciiOut, bool disablePatcher = false) {
                AsciiOut = asciiOut;
                Data.Add(new MultiBinaryDataSub(data, disablePatcher));
                Offset = offset;
            }

            #region Nested type: MultiBinaryDataSub

            internal sealed class MultiBinaryDataSub {
                internal readonly byte[] Data;
                internal readonly bool DisablePatcher;

                public MultiBinaryDataSub(string data, bool disablePatcher) {
                    DisablePatcher = disablePatcher;
                    Data = Common.HexToBytes(data);
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: SKUModel

        internal sealed class SKUModel {
            internal List<SKUModelData> DataList = new List<SKUModelData>();
            internal string MinVer;
            internal string Name;
            internal int SKUKey;
            internal bool Warn;
            internal string WarnMsg;
        }

        #endregion

        #region Nested type: SKUModelData

        internal sealed class SKUModelData {
            internal string Data;
            internal string Type;
        }

        #endregion

        #region Nested type: SKUModelOffsets

        internal sealed class SKUModelOffsets {
            internal int Offset;
            internal int Size;
            internal string Type;
        }

        #endregion

        #region Nested type: SingleBinaryData

        public sealed class SingleBinaryData {
            internal readonly bool AsciiOut;
            internal readonly byte[] Data;
            internal readonly bool DisablePatcher;
            internal readonly string Name;
            internal readonly int Offset;

            public SingleBinaryData(string data, int offset, string name, bool asciiOut, bool disablePatcher = false) {
                AsciiOut = asciiOut;
                Data = Common.HexToBytes(data);
                DisablePatcher = disablePatcher;
                Name = name;
                Offset = offset;
            }
        }

        #endregion
    }
}