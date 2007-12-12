namespace TracerX.Viewer {
    partial class About {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.minFileVer = new System.Windows.Forms.TextBox();
            this.curFileVer = new System.Windows.Forms.TextBox();
            this.asmVer = new System.Windows.Forms.TextBox();
            this.ok = new System.Windows.Forms.Button();
            this.loadDir = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(414, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "TracerX is a combination logger and viewer for .NET written by Mark A. Lauritsen." +
                "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Assembly version:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 150);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Generates log file version:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(164, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Earliest log file version supported:";
            // 
            // minFileVer
            // 
            this.minFileVer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.minFileVer.Location = new System.Drawing.Point(176, 114);
            this.minFileVer.Name = "minFileVer";
            this.minFileVer.ReadOnly = true;
            this.minFileVer.Size = new System.Drawing.Size(251, 20);
            this.minFileVer.TabIndex = 4;
            // 
            // curFileVer
            // 
            this.curFileVer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.curFileVer.Location = new System.Drawing.Point(176, 147);
            this.curFileVer.Name = "curFileVer";
            this.curFileVer.ReadOnly = true;
            this.curFileVer.Size = new System.Drawing.Size(251, 20);
            this.curFileVer.TabIndex = 5;
            // 
            // asmVer
            // 
            this.asmVer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.asmVer.Location = new System.Drawing.Point(176, 51);
            this.asmVer.Name = "asmVer";
            this.asmVer.ReadOnly = true;
            this.asmVer.Size = new System.Drawing.Size(251, 20);
            this.asmVer.TabIndex = 6;
            // 
            // ok
            // 
            this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ok.Location = new System.Drawing.Point(352, 185);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 7;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            // 
            // loadDir
            // 
            this.loadDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.loadDir.Location = new System.Drawing.Point(176, 81);
            this.loadDir.Name = "loadDir";
            this.loadDir.ReadOnly = true;
            this.loadDir.Size = new System.Drawing.Size(251, 20);
            this.loadDir.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Loaded from directory:";
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 226);
            this.Controls.Add(this.loadDir);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.asmVer);
            this.Controls.Add(this.curFileVer);
            this.Controls.Add(this.minFileVer);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1298, 260);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(298, 260);
            this.Name = "About";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "About TracerX";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox minFileVer;
        private System.Windows.Forms.TextBox curFileVer;
        private System.Windows.Forms.TextBox asmVer;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.TextBox loadDir;
        private System.Windows.Forms.Label label5;
    }
}