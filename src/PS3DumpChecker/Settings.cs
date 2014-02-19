namespace PS3DumpChecker {
    using System;
    using System.Windows.Forms;

    internal sealed partial class Settings : Form {
        internal Settings() {
            InitializeComponent();
            Icon = Program.AppIcon;
            foreach(Control ctrl in Controls) {
                if(ctrl is GroupBox)
                    SetCheckBoxes(ctrl);
            }
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
            if(dohashcheck.Checked)
                Program.MainForm.DoParseHashList();
            Close();
        }
    }
}