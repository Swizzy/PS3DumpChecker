namespace PS3DumpChecker {
    using System.Windows.Forms;

    internal sealed partial class Settings : Form {
        internal Settings() {
            InitializeComponent();
            Icon = Program.AppIcon;
            autopatch.Checked = Program.GetRegSetting("autopatch");
            autoexit.Checked = Program.GetRegSetting("autoexit");
            dohashcheck.Checked = Program.GetRegSetting("dohashcheck", true);
        }

        private void Button1Click(object sender, System.EventArgs e)
        {
            Program.SetRegSetting("autopatch", autopatch.Checked);
            Program.SetRegSetting("autoexit", autoexit.Checked);
            Program.SetRegSetting("dohashcheck", dohashcheck.Checked);
            if (dohashcheck.Checked)
                Program.MainForm.DoParseHashList();
            Close();
        }
    }
}