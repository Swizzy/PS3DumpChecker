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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.autopatch.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(12, 199);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(182, 23);
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
            this.groupBox1.Controls.Add(this.dorepcheck);
            this.groupBox1.Location = new System.Drawing.Point(12, 128);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 65);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Checks";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.autopatch);
            this.groupBox2.Controls.Add(this.autoexit);
            this.groupBox2.Controls.Add(this.AutoDLcfg);
            this.groupBox2.Controls.Add(this.AutoDLhashlist);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(182, 110);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Auto Settings";
            // 
            // Settings
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(206, 234);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
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
    }
}