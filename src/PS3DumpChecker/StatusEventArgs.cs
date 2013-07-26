namespace PS3DumpChecker {
    using System;

    internal sealed class StatusEventArgs : EventArgs {
        internal readonly string Status;

        internal StatusEventArgs(string msg) {
            Status = msg;
        }
    }
}