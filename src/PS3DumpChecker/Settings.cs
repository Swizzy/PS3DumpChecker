namespace PS3DumpChecker {
    using System;
    using System.Windows.Forms;

    internal sealed partial class Settings : Form {
        internal Settings() {
            InitializeComponent();
            Icon = Program.AppIcon;
            foreach(var ctrl in Controls) {
                var cbox = ctrl as CheckBox;
                if(cbox == null)
                    continue;
                switch(cbox.Name) {
                    case "dohashcheck":
                        dohashcheck.Checked = Program.GetRegSetting("dohashcheck", true); // We want this to be true by default, thus we seperate it from the others
                        break;
                    default:
                        cbox.Checked = Program.GetRegSetting(cbox.Name);
                        break;
                }
            }
        }

        private void Button1Click(object sender, EventArgs e) {
            foreach(var ctrl in Controls) {
                var cbox = ctrl as CheckBox;
                if(cbox == null)
                    continue;
                Program.SetRegSetting(cbox.Name, cbox.Checked);
            }
            if(dohashcheck.Checked)
                Program.MainForm.DoParseHashList();
            Close();
        }
    }
}