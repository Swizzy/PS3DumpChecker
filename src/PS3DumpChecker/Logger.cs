using System;
using System.IO;
using System.Windows.Forms;

namespace PS3DumpChecker
{
    public static class Logger
    {
        private const string DefaultName = "PS3Check.log";
        public static string LogPath;
        public static bool Enabled;

        public static void WriteLine(string entry) { Write2(string.Format("{0} : {1}{2}", GetTimeOfDay(), entry, Environment.NewLine)); }

        public static void WriteLine2(string entry) { Write2(string.Format("{0}{1}", entry, Environment.NewLine)); }

        public static void Write(string entry) { Write2(string.Format("{0} : {1}", GetTimeOfDay(), entry)); }

        public static void Write2(string entry)
        {
            if (!Enabled)
                return;
            if (string.IsNullOrEmpty(LogPath))
                LogPath = string.Format("{0}\\{1}", Path.GetDirectoryName(Application.ExecutablePath), DefaultName);
            try
            {
                using (var sw = new StreamWriter(File.Open(LogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
                    sw.Write(entry);
                GC.Collect();
            }
            catch { }
        }

        private static string GetTimeOfDay() { return string.Format("{0:T}", DateTime.Now); }

    }
}