namespace PS3DumpChecker
{
    internal sealed partial class UpdateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cfgbtn = new System.Windows.Forms.Button();
            this.hashlistbtn = new System.Windows.Forms.Button();
            this.appbtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.changelog = new System.Windows.Forms.RichTextBox();
            this.statusstrip = new System.Windows.Forms.StatusStrip();
            this.statuslbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.changelogbw = new System.ComponentModel.BackgroundWorker();
            this.dwlbw = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.statusstrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // cfgbtn
            // 
            this.cfgbtn.Enabled = false;
            this.cfgbtn.Location = new System.Drawing.Point(13, 13);
            this.cfgbtn.Name = "cfgbtn";
            this.cfgbtn.Size = new System.Drawing.Size(152, 23);
            this.cfgbtn.TabIndex = 0;
            this.cfgbtn.Text = "Download Latest CFG";
            this.cfgbtn.UseVisualStyleBackColor = true;
            this.cfgbtn.Click += new System.EventHandler(this.CfgbtnClick);
            // 
            // hashlistbtn
            // 
            this.hashlistbtn.Enabled = false;
            this.hashlistbtn.Location = new System.Drawing.Point(171, 13);
            this.hashlistbtn.Name = "hashlistbtn";
            this.hashlistbtn.Size = new System.Drawing.Size(176, 23);
            this.hashlistbtn.TabIndex = 0;
            this.hashlistbtn.Text = "Download Latest Hashlist";
            this.hashlistbtn.UseVisualStyleBackColor = true;
            this.hashlistbtn.Click += new System.EventHandler(this.HashlistbtnClick);
            // 
            // appbtn
            // 
            this.appbtn.Enabled = false;
            this.appbtn.Location = new System.Drawing.Point(13, 42);
            this.appbtn.Name = "appbtn";
            this.appbtn.Size = new System.Drawing.Size(334, 23);
            this.appbtn.TabIndex = 1;
            this.appbtn.Text = "Download Latest PS3 Dump Checker";
            this.appbtn.UseVisualStyleBackColor = true;
            this.appbtn.Click += new System.EventHandler(this.AppbtnClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.changelog);
            this.groupBox1.Location = new System.Drawing.Point(13, 71);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(334, 218);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Changelog";
            // 
            // changelog
            // 
            this.changelog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.changelog.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.changelog.Location = new System.Drawing.Point(3, 16);
            this.changelog.Name = "changelog";
            this.changelog.ReadOnly = true;
            this.changelog.Size = new System.Drawing.Size(328, 199);
            this.changelog.TabIndex = 0;
            this.changelog.Text = "";
            this.changelog.WordWrap = false;
            // 
            // statusstrip
            // 
            this.statusstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statuslbl});
            this.statusstrip.Location = new System.Drawing.Point(0, 292);
            this.statusstrip.Name = "statusstrip";
            this.statusstrip.Size = new System.Drawing.Size(359, 22);
            this.statusstrip.TabIndex = 3;
            this.statusstrip.Text = "statusStrip1";
            // 
            // statuslbl
            // 
            this.statuslbl.Name = "statuslbl";
            this.statuslbl.Size = new System.Drawing.Size(38, 17);
            this.statuslbl.Text = "status";
            // 
            // changelogbw
            // 
            this.changelogbw.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BWDoWork);
            this.changelogbw.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BWRunWorkerCompleted);
            // 
            // UpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 314);
            this.Controls.Add(this.statusstrip);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.appbtn);
            this.Controls.Add(this.hashlistbtn);
            this.Controls.Add(this.cfgbtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "UpdateForm";
            this.Load += new System.EventHandler(this.UpdateFormLoad);
            this.groupBox1.ResumeLayout(false);
            this.statusstrip.ResumeLayout(false);
            this.statusstrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cfgbtn;
        private System.Windows.Forms.Button hashlistbtn;
        private System.Windows.Forms.Button appbtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox changelog;
        private System.Windows.Forms.StatusStrip statusstrip;
        private System.Windows.Forms.ToolStripStatusLabel statuslbl;
        private System.ComponentModel.BackgroundWorker changelogbw;
        private System.ComponentModel.BackgroundWorker dwlbw;

    }
}