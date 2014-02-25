namespace PS3DumpChecker
{
    partial class Simulation
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
            this.sim0btn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.addressLine = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.address1 = new System.Windows.Forms.NumericUpDown();
            this.address0 = new System.Windows.Forms.NumericUpDown();
            this.sim1btn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.addressLine)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.address1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.address0)).BeginInit();
            this.SuspendLayout();
            // 
            // sim0btn
            // 
            this.sim0btn.Location = new System.Drawing.Point(99, 19);
            this.sim0btn.Name = "sim0btn";
            this.sim0btn.Size = new System.Drawing.Size(95, 23);
            this.sim0btn.TabIndex = 0;
            this.sim0btn.Text = "Simulate";
            this.sim0btn.UseVisualStyleBackColor = true;
            this.sim0btn.Click += new System.EventHandler(this.Sim0BtnClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.addressLine);
            this.groupBox1.Controls.Add(this.sim0btn);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 49);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Simluate Bad Address Line";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "A#:";
            // 
            // addressLine
            // 
            this.addressLine.Location = new System.Drawing.Point(42, 19);
            this.addressLine.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.addressLine.Name = "addressLine";
            this.addressLine.Size = new System.Drawing.Size(51, 20);
            this.addressLine.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.address1);
            this.groupBox2.Controls.Add(this.address0);
            this.groupBox2.Controls.Add(this.sim1btn);
            this.groupBox2.Location = new System.Drawing.Point(12, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 73);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Simluate Swapped Address Line";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "A#1:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "A#0:";
            // 
            // address1
            // 
            this.address1.Location = new System.Drawing.Point(42, 45);
            this.address1.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.address1.Name = "address1";
            this.address1.Size = new System.Drawing.Size(51, 20);
            this.address1.TabIndex = 1;
            // 
            // address0
            // 
            this.address0.Location = new System.Drawing.Point(42, 19);
            this.address0.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.address0.Name = "address0";
            this.address0.Size = new System.Drawing.Size(51, 20);
            this.address0.TabIndex = 1;
            // 
            // sim1btn
            // 
            this.sim1btn.Location = new System.Drawing.Point(99, 19);
            this.sim1btn.Name = "sim1btn";
            this.sim1btn.Size = new System.Drawing.Size(95, 48);
            this.sim1btn.TabIndex = 0;
            this.sim1btn.Text = "Simulate";
            this.sim1btn.UseVisualStyleBackColor = true;
            this.sim1btn.Click += new System.EventHandler(this.Sim1BtnClick);
            // 
            // Simulation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 152);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Simulation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Simulation Shit";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.addressLine)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.address1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.address0)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button sim0btn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown addressLine;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown address1;
        private System.Windows.Forms.NumericUpDown address0;
        private System.Windows.Forms.Button sim1btn;
        private System.Windows.Forms.Label label3;
    }
}