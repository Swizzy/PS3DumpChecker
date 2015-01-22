namespace PS3DumpChecker {
    using System;
    using System.IO;
    using System.Windows.Forms;
    using PS3DumpChecker.Properties;

    internal sealed partial class Settings : Form {
        internal Settings()
        {
            InitializeComponent();
            Icon = Program.AppIcon;
            foreach (Control ctrl in Controls)
            {
                if (ctrl is GroupBox)
                    SetCheckBoxes(ctrl);
            }
            SetCheckBoxes(this);
            //#if EMBEDDED_PATCHES
            //            UseInternalPatcher.Text = Resources.UseEmbeddedPatches; // This will use embedded files
            //#else
            //            UseInternalPatcher.Text = Resources.UseInternalPatcher; // This will use external files
            //#endif
            disabledisclaimerbtn.Enabled = !Program.HasAcceptedTerms2();
            trvkpatches.Enabled = customrospatch.Enabled = UseInternalPatcher.Checked;
            patchBox.Enabled = patchbutton.Enabled = UseInternalPatcher.Checked && customrospatch.Checked;
            patchBox.Text = Program.GetRegSettingText(patchBox.Name);
            rosheaders.Enabled = forcepatch.Checked && UseInternalPatcher.Checked;

            StreamReader reader = null;
            Stream input = null;
            try
            {
                input = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("{0}.{1}", typeof(Program).Namespace, "Patches.patch_info.txt"));
                reader = new StreamReader(input);
                patchinfoLabel.Text = reader.ReadToEnd();
            }
            catch
            {
                MessageBox.Show("Error reading embedded ROS patch version!");
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
            if ((patchBox.Text == "") && UseInternalPatcher.Checked && customrospatch.Checked)
            {
                MessageBox.Show(Resources.ErrorNoSeletedPatch);
            }
            else 
            {
            foreach (Control ctrl in Controls)
                if (ctrl is GroupBox)
                    GetCheckBoxes(ctrl);
            GetCheckBoxes(this);
            Program.SetRegSetting(patchBox.Name, false, patchBox.Text);
            if (dohashcheck.Checked)
                Program.MainForm.DoParseHashList();
            Program.MainForm.forcepatchstate();
            Close();
            }
        }

        private void DisabledisclaimerbtnClick(object sender, EventArgs e) {
            Program.HasAcceptedTerms(true);
        }

        private void UseInternalPatcher_CheckedChanged(object sender, EventArgs e)
        {
            trvkpatches.Enabled = customrospatch.Enabled = UseInternalPatcher.Checked;
            patchBox.Enabled = patchbutton.Enabled = UseInternalPatcher.Checked && customrospatch.Checked;
            rosheaders.Enabled = forcepatch.Checked && UseInternalPatcher.Checked;
            if (!UseInternalPatcher.Checked)
                rosheaders.Checked = false;
        }

        private void customrospatch_CheckedChanged(object sender, EventArgs e)
        {
             patchBox.Enabled = patchbutton.Enabled = customrospatch.Checked;
        }

        private void patchbutton_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = Resources.selpatch,
                FileName = "patch.bin",
                Filter = Resources.ofdfilter,
                DefaultExt = "bin",
                AutoUpgradeEnabled = true,
                AddExtension = true
            };
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            patchBox.Text = ofd.FileName;
        }

        private void forcepatch_CheckedChanged(object sender, EventArgs e)
        {
            rosheaders.Enabled = forcepatch.Checked && UseInternalPatcher.Checked;
        }

        private void restoredefaultBoutton_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show(Resources.RestoreDefaultSettings, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == System.Windows.Forms.DialogResult.Yes)
            {
                Program.ClearRegSetting();
                MessageBox.Show(Resources.RestartMessage, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Restart();
            }
        }
    }
}