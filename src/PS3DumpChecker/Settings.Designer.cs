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
            this.forcepatch = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // autopatch
            // 
            this.autopatch.AutoSize = true;
            this.autopatch.Location = new System.Drawing.Point(9, 28);
            this.autopatch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.autopatch.Name = "autopatch";
            this.autopatch.Size = new System.Drawing.Size(165, 24);
            this.autopatch.TabIndex = 0;
            this.autopatch.Text = "Enable auto patch";
            this.autopatch.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(18, 495);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(273, 35);
            this.button1.TabIndex = 1;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1Click);
            // 
            // autoexit
            // 
            this.autoexit.AutoSize = true;
            this.autoexit.Location = new System.Drawing.Point(9, 63);
            this.autoexit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.autoexit.Name = "autoexit";
            this.autoexit.Size = new System.Drawing.Size(230, 24);
            this.autoexit.TabIndex = 0;
            this.autoexit.Text = "Enable auto exit after patch";
            this.autoexit.UseVisualStyleBackColor = true;
            // 
            // dohashcheck
            // 
            this.dohashcheck.AutoSize = true;
            this.dohashcheck.Location = new System.Drawing.Point(9, 29);
            this.dohashcheck.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dohashcheck.Name = "dohashcheck";
            this.dohashcheck.Size = new System.Drawing.Size(178, 24);
            this.dohashcheck.TabIndex = 0;
            this.dohashcheck.Text = "Enable hash checks";
            this.dohashcheck.UseVisualStyleBackColor = true;
            // 
            // AutoDLcfg
            // 
            this.AutoDLcfg.AutoSize = true;
            this.AutoDLcfg.Location = new System.Drawing.Point(9, 98);
            this.AutoDLcfg.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AutoDLcfg.Name = "AutoDLcfg";
            this.AutoDLcfg.Size = new System.Drawing.Size(219, 24);
            this.AutoDLcfg.TabIndex = 0;
            this.AutoDLcfg.Text = "Enable auto cfg download";
            this.AutoDLcfg.UseVisualStyleBackColor = true;
            // 
            // AutoDLhashlist
            // 
            this.AutoDLhashlist.AutoSize = true;
            this.AutoDLhashlist.Location = new System.Drawing.Point(9, 134);
            this.AutoDLhashlist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AutoDLhashlist.Name = "AutoDLhashlist";
            this.AutoDLhashlist.Size = new System.Drawing.Size(251, 24);
            this.AutoDLhashlist.TabIndex = 0;
            this.AutoDLhashlist.Text = "Enable auto hashlist download";
            this.AutoDLhashlist.UseVisualStyleBackColor = true;
            // 
            // dorepcheck
            // 
            this.dorepcheck.AutoSize = true;
            this.dorepcheck.Location = new System.Drawing.Point(9, 65);
            this.dorepcheck.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dorepcheck.Name = "dorepcheck";
            this.dorepcheck.Size = new System.Drawing.Size(217, 24);
            this.dorepcheck.TabIndex = 0;
            this.dorepcheck.Text = "Enable repetitions checks";
            this.dorepcheck.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dohashcheck);
            this.groupBox1.Controls.Add(this.dorosvercheck);
            this.groupBox1.Controls.Add(this.dorepcheck);
            this.groupBox1.Location = new System.Drawing.Point(18, 197);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(273, 135);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Checks";
            // 
            // dorosvercheck
            // 
            this.dorosvercheck.AutoSize = true;
            this.dorosvercheck.Location = new System.Drawing.Point(9, 100);
            this.dorosvercheck.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dorosvercheck.Name = "dorosvercheck";
            this.dorosvercheck.Size = new System.Drawing.Size(236, 24);
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
            this.groupBox2.Location = new System.Drawing.Point(18, 18);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(273, 169);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Auto Settings";
            // 
            // UseInternalPatcher
            // 
            this.UseInternalPatcher.AutoSize = true;
            this.UseInternalPatcher.Location = new System.Drawing.Point(9, 29);
            this.UseInternalPatcher.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.UseInternalPatcher.Name = "UseInternalPatcher";
            this.UseInternalPatcher.Size = new System.Drawing.Size(205, 24);
            this.UseInternalPatcher.TabIndex = 3;
            this.UseInternalPatcher.Text = "Use embedded patches";
            this.UseInternalPatcher.UseVisualStyleBackColor = true;
            // 
            // disabledisclaimerbtn
            // 
            this.disabledisclaimerbtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.disabledisclaimerbtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.disabledisclaimerbtn.Enabled = false;
            this.disabledisclaimerbtn.Location = new System.Drawing.Point(18, 451);
            this.disabledisclaimerbtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.disabledisclaimerbtn.Name = "disabledisclaimerbtn";
            this.disabledisclaimerbtn.Size = new System.Drawing.Size(273, 35);
            this.disabledisclaimerbtn.TabIndex = 1;
            this.disabledisclaimerbtn.Text = "Disable Disclaimer";
            this.disabledisclaimerbtn.UseVisualStyleBackColor = true;
            this.disabledisclaimerbtn.Click += new System.EventHandler(this.DisabledisclaimerbtnClick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.forcepatch);
            this.groupBox3.Controls.Add(this.UseInternalPatcher);
            this.groupBox3.Location = new System.Drawing.Point(18, 342);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Size = new System.Drawing.Size(273, 100);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Patcher Settings";
            // 
            // forcepatch
            // 
            this.forcepatch.AutoSize = true;
            this.forcepatch.Location = new System.Drawing.Point(9, 63);
            this.forcepatch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.forcepatch.Name = "forcepatch";
            this.forcepatch.Size = new System.Drawing.Size(190, 24);
            this.forcepatch.TabIndex = 4;
            this.forcepatch.Text = "Enable force patching";
            this.forcepatch.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 549);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.disabledisclaimerbtn);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
    }
}