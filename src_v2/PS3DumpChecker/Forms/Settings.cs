namespace PS3DumpChecker.Forms {
    using Microsoft.Win32;
    using PS3DumpChecker.Properties;
    using System;
    using System.Windows.Forms;

    internal sealed partial class Settings: Form {
        private static RegistryKey _regkey;

        internal Settings() {
            InitializeComponent();
            Icon = Program.AppIcon;
            foreach(Control ctrl in Controls) {
                if(ctrl is GroupBox)
                    SetCheckBoxes(ctrl);
            }
            SetCheckBoxes(this);
#if EMBEDDED_PATCHES
            UseInternalPatcher.Text = Resources.UseEmbeddedPatches; // This will use embedded files
#else
            UseInternalPatcher.Text = Resources.UseInternalPatcher; // This will use external files
#endif
            disabledisclaimerbtn.Enabled = !HasDisabledDisclaimer();
        }

        private static void SetCheckBoxes(Control ctrl) {
            foreach(var control in ctrl.Controls) {
                var cbox = control as CheckBox;
                if(cbox == null)
                    continue;
                cbox.Checked = GetRegSetting(cbox.Name);
            }
        }

        private static void GetCheckBoxes(Control ctrl) {
            foreach(var control in ctrl.Controls) {
                var cbox = control as CheckBox;
                if(cbox == null)
                    continue;
                SetRegSetting(cbox.Name, cbox.Checked);
            }
        }

        private void SaveSettings(object sender, EventArgs e) {
            foreach(Control ctrl in Controls) {
                if(ctrl is GroupBox)
                    GetCheckBoxes(ctrl);
            }
            GetCheckBoxes(this);
            //if(dohashcheck.Checked)
            //    Program.MainForm.DoParseHashList();
            Close();
        }

        private void DisabledisclaimerbtnClick(object sender, EventArgs e) { HasAcceptedTerms(true); }

        private static void SetupRegKey() {
            if(_regkey != null)
                return;
            var key = Registry.CurrentUser.CreateSubKey("Software");
            if(key == null)
                throw new InvalidOperationException();
            key = key.CreateSubKey("Swizzy");
            if(key == null)
                throw new InvalidOperationException();
            _regkey = key.CreateSubKey("PS3 Dump Checker");
        }

        internal static bool GetRegSetting(string setting, bool value = false) {
            SetupRegKey();
            return _regkey.GetValue(setting, -1) is int ? (int)_regkey.GetValue(setting, value ? 1 : 0) > 0 : value;
        }

        internal static void SetRegSetting(string setting, bool value = false) {
            SetupRegKey();
            _regkey.SetValue(setting, value ? 1 : 0);
        }

        internal bool HasDisabledDisclaimer() {
            SetupRegKey();
            var termskey = _regkey.GetValue("TermsAccepted", 0) is int ? (int)_regkey.GetValue("TermsAccepted", 0) : 0;
            return termskey == 2;
        }

        internal static bool HasAcceptedTerms(bool disable = false) {
            SetupRegKey();
            if(disable) {
                _regkey.SetValue("TermsAccepted", 2);
                MessageBox.Show(Resources.YouveBeenWarned);
                return true;
            }
            var termskey = _regkey.GetValue("TermsAccepted", 0) is int ? (int)_regkey.GetValue("TermsAccepted", 0) : 0;
            return termskey == 2 || AcceptRandomized(termskey);
        }

        private static bool AcceptRandomized(int current) {
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
            _regkey.SetValue("TermsAccepted", current == 1 ? 0 : 1);
            if(!ret)
                MessageBox.Show(Resources.DisclaimerFailedMessage);
            return ret;
        }
    }
}