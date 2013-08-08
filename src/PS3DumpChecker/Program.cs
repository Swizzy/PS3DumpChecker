namespace PS3DumpChecker {
    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;
    using Microsoft.Win32;

    internal static class Program {
        internal static readonly Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        internal static Main MainForm;

        /// <summary>
        ///   The main entry point for the application.
        /// </summary>
        [STAThread] private static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm = new Main(args);
            Application.Run(MainForm);
        }

        internal static void ExtractResource(FileInfo fi, string resource) {
            var toexe = fi.OpenWrite();
            var fromexe = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            const int size = 4096;
            var bytes = new byte[size];
            int numBytes;
            while(fromexe != null && (numBytes = fromexe.Read(bytes, 0, size)) > 0)
                toexe.Write(bytes, 0, numBytes);
            toexe.Close();
            if(fromexe != null)
                fromexe.Close();
        }

        internal static bool GetRegSetting(string setting, bool value = false) {
            var key = Registry.CurrentUser.CreateSubKey("Software");
            if(key == null)
                return value;
            key = key.CreateSubKey("Swizzy");
            if(key == null)
                return value;
            key = key.CreateSubKey("PS3 Dump Checker");
            if(key == null)
                return value;
            return key.GetValue(setting, -1) is int ? (int) key.GetValue(setting, value ? 1 : 0) > 0 : value;
        }

        internal static void SetRegSetting(string setting, bool value = true) {
            var key = Registry.CurrentUser.CreateSubKey("Software");
            if(key == null)
                return;
            key = key.CreateSubKey("Swizzy");
            if(key == null)
                return;
            key = key.CreateSubKey("PS3 Dump Checker");
            if(key == null)
                return;
            key.SetValue(setting, value ? 1 : 0);
        }
    }
}