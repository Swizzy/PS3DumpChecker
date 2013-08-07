namespace PS3DumpChecker {
    using System;
    using System.Drawing;
    using System.Windows.Forms;

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
    }
}