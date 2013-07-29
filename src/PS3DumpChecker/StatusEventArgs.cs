using System;

namespace PS3DumpChecker
{
    internal sealed class StatusEventArgs : EventArgs
    {
        internal readonly string Status;

        internal StatusEventArgs(string msg)
        {
            Status = msg;
        }
    }
}