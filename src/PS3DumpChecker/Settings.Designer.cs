namespace PS3DumpChecker
{
    internal sealed partial class Settings
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
            this.components = new System.ComponentModel.Container();
            this.autopatch = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.autoexit = new System.Windows.Forms.CheckBox();
            this.dohashcheck = new System.Windows.Forms.CheckBox();
            this.AutoDLcfg = new System.Windows.Forms.CheckBox();
            this.AutoDLhashlist = new System.Windows.Forms.CheckBox();
            this.dorepcheck = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dorosvercheck = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.UseInternalPatcher = new System.Windows.Forms.CheckBox();
            this.disabledisclaimerbtn = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.patchinfoLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.patchbutton = new System.Windows.Forms.Button();
            this.patchBox = new System.Windows.Forms.TextBox();
            this.customrospatch = new System.Windows.Forms.CheckBox();
            this.rosheaders = new System.Windows.Forms.CheckBox();
            this.trvkpatches = new System.Windows.Forms.CheckBox();
            this.forcepatch = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.restoredefaultBoutton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // autopatch
            // 
            this.autopatch.AutoSize = true;
            this.autopatch.Location = new System.Drawing.Point(6, 18);
            this.autopatch.Name = "autopatch";
            this.autopatch.Size = new System.Drawing.Size(113, 17);
            this.autopatch.TabIndex = 0;
            this.autopatch.Text = "Enable auto patch";
            this.toolTip1.SetToolTip(this.autopatch, "Patch the image once verified, if Ok and not already patched.");
            this.autopatch.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(7, 321);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(369, 35);
            this.button1.TabIndex = 1;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1Click);
            // 
            // autoexit
            // 
            this.autoexit.AutoSize = true;
            this.autoexit.Location = new System.Drawing.Point(6, 41);
            this.autoexit.Name = "autoexit";
            this.autoexit.Size = new System.Drawing.Size(156, 17);
            this.autoexit.TabIndex = 0;
            this.autoexit.Text = "Enable auto exit after patch";
            this.autoexit.UseVisualStyleBackColor = true;
            // 
            // dohashcheck
            // 
            this.dohashcheck.AutoSize = true;
            this.dohashcheck.Location = new System.Drawing.Point(6, 19);
            this.dohashcheck.Name = "dohashcheck";
            this.dohashcheck.Size = new System.Drawing.Size(123, 17);
            this.dohashcheck.TabIndex = 0;
            this.dohashcheck.Text = "Enable hash checks";
            this.dohashcheck.UseVisualStyleBackColor = true;
            // 
            // AutoDLcfg
            // 
            this.AutoDLcfg.AutoSize = true;
            this.AutoDLcfg.Location = new System.Drawing.Point(6, 64);
            this.AutoDLcfg.Name = "AutoDLcfg";
            this.AutoDLcfg.Size = new System.Drawing.Size(150, 17);
            this.AutoDLcfg.TabIndex = 0;
            this.AutoDLcfg.Text = "Enable auto cfg download";
            this.AutoDLcfg.UseVisualStyleBackColor = true;
            // 
            // AutoDLhashlist
            // 
            this.AutoDLhashlist.AutoSize = true;
            this.AutoDLhashlist.Location = new System.Drawing.Point(6, 87);
            this.AutoDLhashlist.Name = "AutoDLhashlist";
            this.AutoDLhashlist.Size = new System.Drawing.Size(170, 17);
            this.AutoDLhashlist.TabIndex = 0;
            this.AutoDLhashlist.Text = "Enable auto hashlist download";
            this.AutoDLhashlist.UseVisualStyleBackColor = true;
            // 
            // dorepcheck
            // 
            this.dorepcheck.AutoSize = true;
            this.dorepcheck.Location = new System.Drawing.Point(6, 42);
            this.dorepcheck.Name = "dorepcheck";
            this.dorepcheck.Size = new System.Drawing.Size(148, 17);
            this.dorepcheck.TabIndex = 0;
            this.dorepcheck.Text = "Enable repetitions checks";
            this.dorepcheck.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dohashcheck);
            this.groupBox1.Controls.Add(this.dorosvercheck);
            this.groupBox1.Controls.Add(this.dorepcheck);
            this.groupBox1.Location = new System.Drawing.Point(200, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(171, 110);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Checks";
            // 
            // dorosvercheck
            // 
            this.dorosvercheck.AutoSize = true;
            this.dorosvercheck.Location = new System.Drawing.Point(6, 65);
            this.dorosvercheck.Name = "dorosvercheck";
            this.dorosvercheck.Size = new System.Drawing.Size(161, 17);
            this.dorosvercheck.TabIndex = 0;
            this.dorosvercheck.Text = "Enable ROS Version checks";
            this.dorosvercheck.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.autopatch);
            this.groupBox2.Controls.Add(this.autoexit);
            this.groupBox2.Controls.Add(this.AutoDLcfg);
            this.groupBox2.Controls.Add(this.AutoDLhashlist);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(181, 110);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Auto Settings";
            // 
            // UseInternalPatcher
            // 
            this.UseInternalPatcher.AutoSize = true;
            this.UseInternalPatcher.Location = new System.Drawing.Point(6, 19);
            this.UseInternalPatcher.Name = "UseInternalPatcher";
            this.UseInternalPatcher.Size = new System.Drawing.Size(137, 17);
            this.UseInternalPatcher.TabIndex = 3;
            this.UseInternalPatcher.Text = "Use embedded patcher";
            this.toolTip1.SetToolTip(this.UseInternalPatcher, "Use embedded patcher and patches instead of the external patcher.exe (aka Autopat" +
        "cher).");
            this.UseInternalPatcher.UseVisualStyleBackColor = true;
            this.UseInternalPatcher.CheckedChanged += new System.EventHandler(this.UseInternalPatcher_CheckedChanged);
            // 
            // disabledisclaimerbtn
            // 
            this.disabledisclaimerbtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.disabledisclaimerbtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.disabledisclaimerbtn.Enabled = false;
            this.disabledisclaimerbtn.Location = new System.Drawing.Point(7, 292);
            this.disabledisclaimerbtn.Name = "disabledisclaimerbtn";
            this.disabledisclaimerbtn.Size = new System.Drawing.Size(183, 23);
            this.disabledisclaimerbtn.TabIndex = 1;
            this.disabledisclaimerbtn.Text = "Disable Disclaimer";
            this.disabledisclaimerbtn.UseVisualStyleBackColor = true;
            this.disabledisclaimerbtn.Click += new System.EventHandler(this.DisabledisclaimerbtnClick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.patchinfoLabel);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.patchbutton);
            this.groupBox3.Controls.Add(this.patchBox);
            this.groupBox3.Controls.Add(this.customrospatch);
            this.groupBox3.Controls.Add(this.rosheaders);
            this.groupBox3.Controls.Add(this.trvkpatches);
            this.groupBox3.Controls.Add(this.forcepatch);
            this.groupBox3.Controls.Add(this.UseInternalPatcher);
            this.groupBox3.Location = new System.Drawing.Point(12, 128);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(359, 155);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Patcher Settings";
            // 
            // patchinfoLabel
            // 
            this.patchinfoLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.patchinfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patchinfoLabel.Location = new System.Drawing.Point(172, 29);
            this.patchinfoLabel.Name = "patchinfoLabel";
            this.patchinfoLabel.Size = new System.Drawing.Size(178, 30);
            this.patchinfoLabel.TabIndex = 10;
            this.patchinfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(169, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Embedded ROS patch:";
            // 
            // patchbutton
            // 
            this.patchbutton.Location = new System.Drawing.Point(328, 80);
            this.patchbutton.Name = "patchbutton";
            this.patchbutton.Size = new System.Drawing.Size(24, 23);
            this.patchbutton.TabIndex = 4;
            this.patchbutton.Text = "...";
            this.toolTip1.SetToolTip(this.patchbutton, "Select your patch file");
            this.patchbutton.UseVisualStyleBackColor = true;
            this.patchbutton.Click += new System.EventHandler(this.patchbutton_Click);
            // 
            // patchBox
            // 
            this.patchBox.Location = new System.Drawing.Point(18, 82);
            this.patchBox.Name = "patchBox";
            this.patchBox.ReadOnly = true;
            this.patchBox.Size = new System.Drawing.Size(308, 20);
            this.patchBox.TabIndex = 8;
            this.patchBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // customrospatch
            // 
            this.customrospatch.AutoSize = true;
            this.customrospatch.Location = new System.Drawing.Point(18, 65);
            this.customrospatch.Name = "customrospatch";
            this.customrospatch.Size = new System.Drawing.Size(141, 17);
            this.customrospatch.TabIndex = 7;
            this.customrospatch.Text = "Use custom ROS patch:";
            this.toolTip1.SetToolTip(this.customrospatch, "Use a custom ROS patch instead of the embedded one.");
            this.customrospatch.UseVisualStyleBackColor = true;
            this.customrospatch.CheckedChanged += new System.EventHandler(this.customrospatch_CheckedChanged);
            // 
            // rosheaders
            // 
            this.rosheaders.AutoSize = true;
            this.rosheaders.Location = new System.Drawing.Point(18, 132);
            this.rosheaders.Name = "rosheaders";
            this.rosheaders.Size = new System.Drawing.Size(130, 17);
            this.rosheaders.TabIndex = 6;
            this.rosheaders.Text = "Restore ROS headers";
            this.toolTip1.SetToolTip(this.rosheaders, "For advanced users only! Must be used with embedded patcher.");
            this.rosheaders.UseVisualStyleBackColor = true;
            // 
            // trvkpatches
            // 
            this.trvkpatches.AutoSize = true;
            this.trvkpatches.Location = new System.Drawing.Point(18, 42);
            this.trvkpatches.Name = "trvkpatches";
            this.trvkpatches.Size = new System.Drawing.Size(125, 17);
            this.trvkpatches.TabIndex = 5;
            this.trvkpatches.Text = "Apply TRVK patches";
            this.toolTip1.SetToolTip(this.trvkpatches, "Not mandatory for a regular noFSM jailbreak.\r\nMandatory for an old school 3.55 FS" +
        "M downgrade.");
            this.trvkpatches.UseVisualStyleBackColor = true;
            // 
            // forcepatch
            // 
            this.forcepatch.AutoSize = true;
            this.forcepatch.Location = new System.Drawing.Point(6, 110);
            this.forcepatch.Name = "forcepatch";
            this.forcepatch.Size = new System.Drawing.Size(130, 17);
            this.forcepatch.TabIndex = 4;
            this.forcepatch.Text = "Enable force patching";
            this.toolTip1.SetToolTip(this.forcepatch, "For advanced users only!");
            this.forcepatch.UseVisualStyleBackColor = true;
            this.forcepatch.CheckedChanged += new System.EventHandler(this.forcepatch_CheckedChanged);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 15000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // restoredefaultBoutton
            // 
            this.restoredefaultBoutton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.restoredefaultBoutton.Location = new System.Drawing.Point(196, 292);
            this.restoredefaultBoutton.Name = "restoredefaultBoutton";
            this.restoredefaultBoutton.Size = new System.Drawing.Size(180, 23);
            this.restoredefaultBoutton.TabIndex = 4;
            this.restoredefaultBoutton.Text = "Restore default settings";
            this.restoredefaultBoutton.UseVisualStyleBackColor = true;
            this.restoredefaultBoutton.Click += new System.EventHandler(this.restoredefaultBoutton_Click);
            // 
            // Settings
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 368);
            this.Controls.Add(this.restoredefaultBoutton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.disabledisclaimerbtn);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox autopatch;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox autoexit;
        private System.Windows.Forms.CheckBox dohashcheck;
        private System.Windows.Forms.CheckBox AutoDLcfg;
        private System.Windows.Forms.CheckBox AutoDLhashlist;
        private System.Windows.Forms.CheckBox dorepcheck;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox UseInternalPatcher;
        private System.Windows.Forms.Button disabledisclaimerbtn;
        private System.Windows.Forms.CheckBox dorosvercheck;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox forcepatch;
        private System.Windows.Forms.CheckBox rosheaders;
        private System.Windows.Forms.CheckBox trvkpatches;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox customrospatch;
        private System.Windows.Forms.Button patchbutton;
        private System.Windows.Forms.TextBox patchBox;
        private System.Windows.Forms.Button restoredefaultBoutton;
        private System.Windows.Forms.Label patchinfoLabel;
        private System.Windows.Forms.Label label1;
    }
}