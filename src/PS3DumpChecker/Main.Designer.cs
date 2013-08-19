namespace PS3DumpChecker
{
    internal sealed partial class Main
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
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= MainLoad;
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
            this.checkbtn = new System.Windows.Forms.Button();
            this.advbox = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.expdatabox = new System.Windows.Forms.TextBox();
            this.actdatabox = new System.Windows.Forms.TextBox();
            this.imginfo = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.minverbox = new System.Windows.Forms.RichTextBox();
            this.reversed = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.idmatchbox = new System.Windows.Forms.RichTextBox();
            this.imgstatus = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.status = new System.Windows.Forms.ToolStripStatusLabel();
            this.worker = new System.ComponentModel.BackgroundWorker();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.update = new System.Windows.Forms.ToolStripMenuItem();
            this.loadHashlistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logstate = new System.Windows.Forms.CheckBox();
            this.statuslabel = new System.Windows.Forms.Label();
            this.partslist = new PS3DumpChecker.Controls.CustomListBox();
            this.advbox.SuspendLayout();
            this.imginfo.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkbtn
            // 
            this.checkbtn.Enabled = false;
            this.checkbtn.Location = new System.Drawing.Point(298, 33);
            this.checkbtn.Name = "checkbtn";
            this.checkbtn.Size = new System.Drawing.Size(450, 25);
            this.checkbtn.TabIndex = 1;
            this.checkbtn.Text = "Check Image";
            this.checkbtn.UseVisualStyleBackColor = true;
            this.checkbtn.Click += new System.EventHandler(this.CheckbtnClick);
            // 
            // advbox
            // 
            this.advbox.Controls.Add(this.label4);
            this.advbox.Controls.Add(this.label3);
            this.advbox.Controls.Add(this.expdatabox);
            this.advbox.Controls.Add(this.actdatabox);
            this.advbox.Location = new System.Drawing.Point(298, 167);
            this.advbox.Name = "advbox";
            this.advbox.Size = new System.Drawing.Size(450, 281);
            this.advbox.TabIndex = 2;
            this.advbox.TabStop = false;
            this.advbox.Text = "Advanced View";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 147);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Actual data:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Expected Data:";
            // 
            // expdatabox
            // 
            this.expdatabox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.expdatabox.Location = new System.Drawing.Point(6, 32);
            this.expdatabox.Multiline = true;
            this.expdatabox.Name = "expdatabox";
            this.expdatabox.ReadOnly = true;
            this.expdatabox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.expdatabox.Size = new System.Drawing.Size(438, 112);
            this.expdatabox.TabIndex = 2;
            // 
            // actdatabox
            // 
            this.actdatabox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.actdatabox.Location = new System.Drawing.Point(6, 163);
            this.actdatabox.Multiline = true;
            this.actdatabox.Name = "actdatabox";
            this.actdatabox.ReadOnly = true;
            this.actdatabox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.actdatabox.Size = new System.Drawing.Size(438, 112);
            this.actdatabox.TabIndex = 2;
            // 
            // imginfo
            // 
            this.imginfo.Controls.Add(this.label6);
            this.imginfo.Controls.Add(this.minverbox);
            this.imginfo.Controls.Add(this.reversed);
            this.imginfo.Controls.Add(this.label2);
            this.imginfo.Controls.Add(this.idmatchbox);
            this.imginfo.Controls.Add(this.imgstatus);
            this.imginfo.Controls.Add(this.label5);
            this.imginfo.Controls.Add(this.label1);
            this.imginfo.Location = new System.Drawing.Point(298, 64);
            this.imginfo.Name = "imginfo";
            this.imginfo.Size = new System.Drawing.Size(450, 97);
            this.imginfo.TabIndex = 0;
            this.imginfo.TabStop = false;
            this.imginfo.Text = "Image Information";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(178, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(132, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Lowest downgradeable to:";
            // 
            // minverbox
            // 
            this.minverbox.ForeColor = System.Drawing.Color.Black;
            this.minverbox.Location = new System.Drawing.Point(316, 45);
            this.minverbox.Multiline = false;
            this.minverbox.Name = "minverbox";
            this.minverbox.ReadOnly = true;
            this.minverbox.Size = new System.Drawing.Size(128, 20);
            this.minverbox.TabIndex = 1;
            this.minverbox.Text = "N/A";
            // 
            // reversed
            // 
            this.reversed.ForeColor = System.Drawing.Color.Black;
            this.reversed.Location = new System.Drawing.Point(122, 45);
            this.reversed.Multiline = false;
            this.reversed.Name = "reversed";
            this.reversed.ReadOnly = true;
            this.reversed.Size = new System.Drawing.Size(50, 20);
            this.reversed.TabIndex = 1;
            this.reversed.Text = "N/A";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Raw (PS3 Readable):";
            // 
            // idmatchbox
            // 
            this.idmatchbox.ForeColor = System.Drawing.Color.Black;
            this.idmatchbox.Location = new System.Drawing.Point(122, 71);
            this.idmatchbox.Multiline = false;
            this.idmatchbox.Name = "idmatchbox";
            this.idmatchbox.ReadOnly = true;
            this.idmatchbox.Size = new System.Drawing.Size(322, 20);
            this.idmatchbox.TabIndex = 1;
            this.idmatchbox.Text = "N/A";
            // 
            // imgstatus
            // 
            this.imgstatus.ForeColor = System.Drawing.Color.Black;
            this.imgstatus.Location = new System.Drawing.Point(122, 19);
            this.imgstatus.Multiline = false;
            this.imgstatus.Name = "imgstatus";
            this.imgstatus.ReadOnly = true;
            this.imgstatus.Size = new System.Drawing.Size(322, 20);
            this.imgstatus.TabIndex = 1;
            this.imgstatus.Text = "N/A";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(37, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Matching SKU:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Image Status:";
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status});
            this.statusStrip.Location = new System.Drawing.Point(0, 549);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(760, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip";
            // 
            // status
            // 
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(131, 17);
            this.status.Text = "Waiting for user input...";
            // 
            // worker
            // 
            this.worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DoWork);
            this.worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.WorkerRunWorkerCompleted);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadConfigurationToolStripMenuItem,
            this.update,
            this.loadHashlistToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(760, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // loadConfigurationToolStripMenuItem
            // 
            this.loadConfigurationToolStripMenuItem.Name = "loadConfigurationToolStripMenuItem";
            this.loadConfigurationToolStripMenuItem.Size = new System.Drawing.Size(122, 20);
            this.loadConfigurationToolStripMenuItem.Text = "Load Configuration";
            this.loadConfigurationToolStripMenuItem.Click += new System.EventHandler(this.LoadConfigurationToolStripMenuItemClick);
            // 
            // update
            // 
            this.update.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.update.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.update.Image = global::PS3DumpChecker.Properties.Resources.maj;
            this.update.Name = "update";
            this.update.Size = new System.Drawing.Size(28, 20);
            this.update.Text = "update";
            this.update.Click += new System.EventHandler(this.UpdateClick);
            // 
            // loadHashlistToolStripMenuItem
            // 
            this.loadHashlistToolStripMenuItem.Name = "loadHashlistToolStripMenuItem";
            this.loadHashlistToolStripMenuItem.Size = new System.Drawing.Size(90, 20);
            this.loadHashlistToolStripMenuItem.Text = "Load Hashlist";
            this.loadHashlistToolStripMenuItem.Click += new System.EventHandler(this.LoadHashlistToolStripMenuItemClick);
            // 
            // logstate
            // 
            this.logstate.AutoSize = true;
            this.logstate.Checked = true;
            this.logstate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.logstate.Location = new System.Drawing.Point(626, 7);
            this.logstate.Name = "logstate";
            this.logstate.Size = new System.Drawing.Size(100, 17);
            this.logstate.TabIndex = 5;
            this.logstate.Text = "Enable Logging";
            this.logstate.UseVisualStyleBackColor = true;
            // 
            // statuslabel
            // 
            this.statuslabel.AutoSize = true;
            this.statuslabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 60F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statuslabel.ForeColor = System.Drawing.Color.Black;
            this.statuslabel.Location = new System.Drawing.Point(298, 451);
            this.statuslabel.Name = "statuslabel";
            this.statuslabel.Size = new System.Drawing.Size(354, 91);
            this.statuslabel.TabIndex = 6;
            this.statuslabel.Text = "STATUS";
            // 
            // partslist
            // 
            this.partslist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.partslist.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.partslist.FormattingEnabled = true;
            this.partslist.Location = new System.Drawing.Point(12, 33);
            this.partslist.Name = "partslist";
            this.partslist.ScrollAlwaysVisible = true;
            this.partslist.Size = new System.Drawing.Size(280, 500);
            this.partslist.Sorted = true;
            this.partslist.TabIndex = 0;
            this.partslist.SelectedIndexChanged += new System.EventHandler(this.PartslistSelectedIndexChanged);
            // 
            // Main
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.ClientSize = new System.Drawing.Size(760, 571);
            this.Controls.Add(this.statuslabel);
            this.Controls.Add(this.logstate);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.imginfo);
            this.Controls.Add(this.advbox);
            this.Controls.Add(this.checkbtn);
            this.Controls.Add(this.partslist);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "PS3 Dump Checker v";
            this.Load += new System.EventHandler(this.MainLoad);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainDragEnter);
            this.advbox.ResumeLayout(false);
            this.advbox.PerformLayout();
            this.imginfo.ResumeLayout(false);
            this.imginfo.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PS3DumpChecker.Controls.CustomListBox partslist;
        private System.Windows.Forms.Button checkbtn;
        private System.Windows.Forms.GroupBox advbox;
        private System.Windows.Forms.GroupBox imginfo;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel status;
        private System.ComponentModel.BackgroundWorker worker;
        private System.Windows.Forms.RichTextBox reversed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox imgstatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox actdatabox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox expdatabox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadConfigurationToolStripMenuItem;
        private System.Windows.Forms.CheckBox logstate;
        private System.Windows.Forms.RichTextBox idmatchbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label statuslabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RichTextBox minverbox;
        private System.Windows.Forms.ToolStripMenuItem update;
        private System.Windows.Forms.ToolStripMenuItem loadHashlistToolStripMenuItem;
    }
}

