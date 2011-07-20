namespace MultiCoreHighPerformanceTimer
{
    partial class Form1
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
            this.buttonRunTest = new System.Windows.Forms.Button();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.checkBoxForcePriority = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonRunTest
            // 
            this.buttonRunTest.Location = new System.Drawing.Point(13, 13);
            this.buttonRunTest.Name = "buttonRunTest";
            this.buttonRunTest.Size = new System.Drawing.Size(75, 23);
            this.buttonRunTest.TabIndex = 0;
            this.buttonRunTest.Text = "Run test";
            this.buttonRunTest.UseVisualStyleBackColor = true;
            this.buttonRunTest.Click += new System.EventHandler(this.buttonRunTest_Click);
            // 
            // textBoxLog
            // 
            this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLog.Location = new System.Drawing.Point(13, 43);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLog.Size = new System.Drawing.Size(933, 501);
            this.textBoxLog.TabIndex = 1;
            // 
            // checkBoxForcePriority
            // 
            this.checkBoxForcePriority.AutoSize = true;
            this.checkBoxForcePriority.Location = new System.Drawing.Point(95, 18);
            this.checkBoxForcePriority.Name = "checkBoxForcePriority";
            this.checkBoxForcePriority.Size = new System.Drawing.Size(128, 17);
            this.checkBoxForcePriority.TabIndex = 2;
            this.checkBoxForcePriority.Text = "Force real-time priority";
            this.checkBoxForcePriority.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(958, 556);
            this.Controls.Add(this.checkBoxForcePriority);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.buttonRunTest);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "Form1";
            this.Text = "Multi-Core High Performance Timer Tester";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRunTest;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.CheckBox checkBoxForcePriority;
    }
}

