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
            this.SuspendLayout();
            // 
            // autopatch
            // 
            this.autopatch.AutoSize = true;
            this.autopatch.Location = new System.Drawing.Point(12, 12);
            this.autopatch.Name = "autopatch";
            this.autopatch.Size = new System.Drawing.Size(79, 17);
            this.autopatch.TabIndex = 0;
            this.autopatch.Text = "Auto Patch";
            this.autopatch.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(12, 81);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(139, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1Click);
            // 
            // autoexit
            // 
            this.autoexit.AutoSize = true;
            this.autoexit.Location = new System.Drawing.Point(12, 35);
            this.autoexit.Name = "autoexit";
            this.autoexit.Size = new System.Drawing.Size(123, 17);
            this.autoexit.TabIndex = 0;
            this.autoexit.Text = "Auto Exit after Patch";
            this.autoexit.UseVisualStyleBackColor = true;
            // 
            // dohashcheck
            // 
            this.dohashcheck.AutoSize = true;
            this.dohashcheck.Location = new System.Drawing.Point(12, 58);
            this.dohashcheck.Name = "dohashcheck";
            this.dohashcheck.Size = new System.Drawing.Size(126, 17);
            this.dohashcheck.TabIndex = 0;
            this.dohashcheck.Text = "Enable Hash Checks";
            this.dohashcheck.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(163, 116);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dohashcheck);
            this.Controls.Add(this.autoexit);
            this.Controls.Add(this.autopatch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox autopatch;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox autoexit;
        private System.Windows.Forms.CheckBox dohashcheck;
    }
}