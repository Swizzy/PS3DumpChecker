namespace PS3DumpChecker {
    using System;
    using System.Windows.Forms;
    using PS3DumpChecker.Properties;

    internal sealed partial class Settings : Form {
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
            disabledisclaimerbtn.Enabled = !Program.HasAcceptedTerms2();
        }

        private static void SetCheckBoxes(Control ctrl) {
            foreach(var control in ctrl.Controls) {
                var cbox = control as CheckBox;
                if(cbox == null)
                    continue;
                cbox.Checked = Program.GetRegSetting(cbox.Name);
            }
        }

        private static void GetCheckBoxes(Control ctrl) {
            foreach(var control in ctrl.Controls) {
                var cbox = control as CheckBox;
                if(cbox == null)
                    continue;
                Program.SetRegSetting(cbox.Name, cbox.Checked);
            }
        }

        private void Button1Click(object sender, EventArgs e) {
            foreach(Control ctrl in Controls)
                if(ctrl is GroupBox)
                    GetCheckBoxes(ctrl);
            GetCheckBoxes(this);
            if(dohashcheck.Checked)
                Program.MainForm.DoParseHashList();
            Close();
        }

        private void DisabledisclaimerbtnClick(object sender, EventArgs e) {
            Program.HasAcceptedTerms(true);
        }
    }
}