namespace PS3DumpChecker {
    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Windows.Forms;
    using PS3DumpChecker.Forms;
    using PS3DumpChecker.Properties;
    using Settings = PS3DumpChecker.Forms.Settings;

    internal static class Program {
        internal static readonly Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        internal static MainForm MainForm;

        private static void LogException(string ex) {
            var errorlog = string.Format("{0}\\{1}", Path.GetDirectoryName(Application.ExecutablePath), Resources.CrashLogName);
            try {
                var ver = Assembly.GetAssembly(typeof (Program)).GetName().Version;
                var old = "";
                if (File.Exists(errorlog))
                    old = File.ReadAllText(errorlog);
                old += ex + string.Format("\r\nPS3 Dump Checker v{0}.{1} (Build: {2}) Error Log:", ver.Major, ver.Minor, ver.Build);
                File.WriteAllText(errorlog, old);
                MessageBox.Show(string.Format(Resources.PleaseSendLogTo, Environment.NewLine, errorlog), Resources.YouFoundABug);
            }
            catch {
            }
        }

        private static void ExecHandler(object sender, UnhandledExceptionEventArgs args) { LogException(args.ExceptionObject.ToString()); }

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)] [STAThread] private static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += ExecHandler;
            if (!Settings.HasAcceptedTerms())
                return;
            MainForm = new MainForm(args);
            var wrkdir = Path.GetDirectoryName(Application.ExecutablePath);
            if (!string.IsNullOrEmpty(wrkdir) && Directory.Exists(wrkdir))
                Directory.SetCurrentDirectory(wrkdir);
            Application.Run(MainForm);
        }
    }
}