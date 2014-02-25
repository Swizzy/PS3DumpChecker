namespace PS3DumpChecker {
    using System;
    using System.IO;
    using System.Windows.Forms;

    public partial class Simulation : Form {
        public Simulation() {
            InitializeComponent();
            Icon = Program.AppIcon;
        }

        private void Sim0BtnClick(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();
            if(ofd.ShowDialog() != DialogResult.OK)
                return;
            if(new FileInfo(ofd.FileName).Length != 0x1000000)
                return;
            var srcBuffer = File.ReadAllBytes(ofd.FileName);
            var dstBuffer = new byte[srcBuffer.Length];
            var a0 = (int)addressLine.Value;
            /////////////////////////////////////////////////////
            // Simulate not connected address line (int a0)
            /////////////////////////////////////////////////////
            for (int address = 0; address < (srcBuffer.Length / 2); address++)
            {
                if ((address & (1 << a0)) > 0)
                {
                    dstBuffer[address * 2] = srcBuffer[(address & ~(1 << a0)) * 2];
                    dstBuffer[address * 2 + 1] = srcBuffer[((address & ~(1 << a0)) * 2) + 1];
                }
                else
                {
                    dstBuffer[address * 2] = srcBuffer[address * 2];
                    dstBuffer[address * 2 + 1] = srcBuffer[address * 2 + 1];
                }
            }
            var sfd = new SaveFileDialog();
            if(sfd.ShowDialog() != DialogResult.OK)
                return;
            File.WriteAllBytes(sfd.FileName, dstBuffer);
        }

        private void Sim1BtnClick(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            if (new FileInfo(ofd.FileName).Length != 0x1000000)
                return;
            var srcBuffer = File.ReadAllBytes(ofd.FileName);
            var dstBuffer = new byte[srcBuffer.Length];
            var a0 = (int)address0.Value;
            var a1 = (int)address1.Value;
            /////////////////////////////////////////////////////
            // Simulate swapped address lines (int a0 / int a1)
            /////////////////////////////////////////////////////
            for (int address = 0; address < (srcBuffer.Length / 2); ++address)
            {
                if ((((address & (1 << a0)) > 0) && ((address & (1 << a1)) > 0)) || (((address & (1 << a0)) == 0) && ((address & (1 << a1)) == 0)))
                {
                    dstBuffer[address * 2] = srcBuffer[address * 2];
                    dstBuffer[address * 2 + 1] = srcBuffer[address * 2 + 1];
                }
                else if ((address & (1 << a0)) > 0)
                {
                    dstBuffer[((address | (1 << a1)) & ~(1 << a0)) * 2] = srcBuffer[address * 2];
                    dstBuffer[(((address | (1 << a1)) & ~(1 << a0)) * 2) + 1] = srcBuffer[address * 2 + 1];
                }
                else if ((address & (1 << a1)) > 0)
                {
                    dstBuffer[((address | (1 << a0)) & ~(1 << a1)) * 2] = srcBuffer[address * 2];
                    dstBuffer[(((address | (1 << a0)) & ~(1 << a1)) * 2) + 1] = srcBuffer[address * 2 + 1];
                }
            }
            var sfd = new SaveFileDialog();
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            File.WriteAllBytes(sfd.FileName, dstBuffer);
        }
    }
}