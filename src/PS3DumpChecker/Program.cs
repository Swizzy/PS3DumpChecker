namespace PS3DumpChecker {
    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Windows.Forms;
    using Microsoft.Win32;
    using PS3DumpChecker.Properties;

    internal static class Program {
        internal static readonly Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        internal static MainForm MainForm;

        private static void LogException(string ex) {
            var errorlog = string.Format("{0}\\{1}", MainForm.Wrkdir, Resources.CrashLogName);
            try {
                var old = "";
                if(File.Exists(errorlog))
                    old = File.ReadAllText(errorlog);
                old += ex;
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
            if(!HasAcceptedTerms())
                return;
            MainForm = new MainForm(args);
            Application.Run(MainForm);
        }

        internal static void ExtractResource(FileInfo fi, string resource, bool isFullName = true) {
            if(!isFullName)
                resource = string.Format("{0}.{1}", typeof(Program).Namespace, resource);
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

        internal static bool HasAcceptedTerms(bool disableCheck = false) {
            var key = Registry.CurrentUser.CreateSubKey("Software");
            if(key == null)
                return false;
            key = key.CreateSubKey("Swizzy");
            if(key == null)
                return false;
            key = key.CreateSubKey("PS3 Dump Checker");
            if(key == null)
                return false;
            if(disableCheck)
                key.SetValue("DonorTermsAccepted", 2);
            var termskey = key.GetValue("DonorTermsAccepted", 0) is int ? (int) key.GetValue("DonorTermsAccepted", 0) : 0;
            return termskey == 2 || AcceptRandomized(key, termskey);
        }

        private static bool AcceptRandomized(RegistryKey key, int current) {
            var ret = false;
            DialogResult res;
            if(current == 1) {
                res = MessageBox.Show(Resources.AcceptTermsMessage, Resources.AcceptTermsTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                switch(res) {
                    case DialogResult.Yes:
                        ret = true;
                        break;
                    case DialogResult.No:
                        break;
                    default:
                        return false;
                }
            }
            else {
                res = MessageBox.Show(Resources.AcceptTermsMessageReversed, Resources.AcceptTermsTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                switch(res) {
                    case DialogResult.No:
                        ret = true;
                        break;
                    case DialogResult.Yes:
                        break;
                    default:
                        return false;
                }
            }
            key.SetValue("DonorTermsAccepted", current == 1 ? 0 : 1);
            if(!ret)
                MessageBox.Show(Resources.DisclaimerFailedMessage);
            return ret;
        }
    }
}