namespace TestApp
{
    partial class TextTest
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
            this.chkUse_00 = new System.Windows.Forms.CheckBox();
            this.chkUseKb = new System.Windows.Forms.CheckBox();
            this.txtCircularSize = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMaxSize = new System.Windows.Forms.TextBox();
            this.txtAppendSize = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmboPolicy = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnOpenAndClose = new System.Windows.Forms.Button();
            this.btnLog1 = new System.Windows.Forms.Button();
            this.btnLogTilClosed = new System.Windows.Forms.Button();
            this.btnLogTilWrapped = new System.Windows.Forms.Button();
            this.btnLog2Files = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkUse_00
            // 
            this.chkUse_00.AutoSize = true;
            this.chkUse_00.Location = new System.Drawing.Point(161, 8);
            this.chkUse_00.Name = "chkUse_00";
            this.chkUse_00.Size = new System.Drawing.Size(63, 17);
            this.chkUse_00.TabIndex = 0;
            this.chkUse_00.Text = "Use_00";
            this.chkUse_00.UseVisualStyleBackColor = true;
            // 
            // chkUseKb
            // 
            this.chkUseKb.AutoSize = true;
            this.chkUseKb.Location = new System.Drawing.Point(12, 25);
            this.chkUseKb.Name = "chkUseKb";
            this.chkUseKb.Size = new System.Drawing.Size(105, 17);
            this.chkUseKb.TabIndex = 1;
            this.chkUseKb.Text = "Use Kb for sizes.";
            this.chkUseKb.UseVisualStyleBackColor = true;
            // 
            // txtCircularSize
            // 
            this.txtCircularSize.Location = new System.Drawing.Point(145, 86);
            this.txtCircularSize.Name = "txtCircularSize";
            this.txtCircularSize.Size = new System.Drawing.Size(38, 20);
            this.txtCircularSize.TabIndex = 37;
            this.txtCircularSize.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 13);
            this.label4.TabIndex = 36;
            this.label4.Text = "CircularStartSizeKb:";
            // 
            // txtMaxSize
            // 
            this.txtMaxSize.Location = new System.Drawing.Point(145, 64);
            this.txtMaxSize.Name = "txtMaxSize";
            this.txtMaxSize.Size = new System.Drawing.Size(38, 20);
            this.txtMaxSize.TabIndex = 35;
            this.txtMaxSize.Text = "2";
            // 
            // txtAppendSize
            // 
            this.txtAppendSize.Location = new System.Drawing.Point(145, 42);
            this.txtAppendSize.Name = "txtAppendSize";
            this.txtAppendSize.Size = new System.Drawing.Size(38, 20);
            this.txtAppendSize.TabIndex = 34;
            this.txtAppendSize.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 33;
            this.label3.Text = "MaxSizeMb:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 13);
            this.label2.TabIndex = 32;
            this.label2.Text = "AppendIfSmallerThanMb:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 13);
            this.label1.TabIndex = 38;
            this.label1.Text = "File name will be TextTest.txt";
            // 
            // cmboPolicy
            // 
            this.cmboPolicy.FormattingEnabled = true;
            this.cmboPolicy.Location = new System.Drawing.Point(145, 112);
            this.cmboPolicy.Name = "cmboPolicy";
            this.cmboPolicy.Size = new System.Drawing.Size(121, 21);
            this.cmboPolicy.TabIndex = 39;
            this.cmboPolicy.SelectedIndexChanged += new System.EventHandler(this.cmboPolicy_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 40;
            this.label5.Text = "FullFilePolicy";
            // 
            // btnOpenAndClose
            // 
            this.btnOpenAndClose.Location = new System.Drawing.Point(12, 148);
            this.btnOpenAndClose.Name = "btnOpenAndClose";
            this.btnOpenAndClose.Size = new System.Drawing.Size(105, 23);
            this.btnOpenAndClose.TabIndex = 41;
            this.btnOpenAndClose.Text = "Open and Close";
            this.btnOpenAndClose.UseVisualStyleBackColor = true;
            this.btnOpenAndClose.Click += new System.EventHandler(this.btnOpenAndClose_Click);
            // 
            // btnLog1
            // 
            this.btnLog1.Location = new System.Drawing.Point(12, 177);
            this.btnLog1.Name = "btnLog1";
            this.btnLog1.Size = new System.Drawing.Size(105, 23);
            this.btnLog1.TabIndex = 42;
            this.btnLog1.Text = "Log 1 Line";
            this.btnLog1.UseVisualStyleBackColor = true;
            this.btnLog1.Click += new System.EventHandler(this.btnLog1_Click);
            // 
            // btnLogTilClosed
            // 
            this.btnLogTilClosed.Location = new System.Drawing.Point(119, 148);
            this.btnLogTilClosed.Name = "btnLogTilClosed";
            this.btnLogTilClosed.Size = new System.Drawing.Size(147, 23);
            this.btnLogTilClosed.TabIndex = 43;
            this.btnLogTilClosed.Text = "Log Until File Closes";
            this.btnLogTilClosed.UseVisualStyleBackColor = true;
            this.btnLogTilClosed.Click += new System.EventHandler(this.btnLogTilClosed_Click);
            // 
            // btnLogTilWrapped
            // 
            this.btnLogTilWrapped.Location = new System.Drawing.Point(119, 177);
            this.btnLogTilWrapped.Name = "btnLogTilWrapped";
            this.btnLogTilWrapped.Size = new System.Drawing.Size(147, 23);
            this.btnLogTilWrapped.TabIndex = 44;
            this.btnLogTilWrapped.Text = "Log Until File Wraps";
            this.btnLogTilWrapped.UseVisualStyleBackColor = true;
            this.btnLogTilWrapped.Click += new System.EventHandler(this.btnLogTilWrapped_Click);
            // 
            // btnLog2Files
            // 
            this.btnLog2Files.Location = new System.Drawing.Point(50, 219);
            this.btnLog2Files.Name = "btnLog2Files";
            this.btnLog2Files.Size = new System.Drawing.Size(147, 23);
            this.btnLog2Files.TabIndex = 45;
            this.btnLog2Files.Text = "Log Calls to 2 Files";
            this.btnLog2Files.UseVisualStyleBackColor = true;
            this.btnLog2Files.Click += new System.EventHandler(this.button1_Click);
            // 
            // TextTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 266);
            this.Controls.Add(this.btnLog2Files);
            this.Controls.Add(this.btnLogTilWrapped);
            this.Controls.Add(this.btnLogTilClosed);
            this.Controls.Add(this.btnLog1);
            this.Controls.Add(this.btnOpenAndClose);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmboPolicy);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCircularSize);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtMaxSize);
            this.Controls.Add(this.txtAppendSize);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkUseKb);
            this.Controls.Add(this.chkUse_00);
            this.Name = "TextTest";
            this.Text = "Text File";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkUse_00;
        private System.Windows.Forms.CheckBox chkUseKb;
        private System.Windows.Forms.TextBox txtCircularSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtMaxSize;
        private System.Windows.Forms.TextBox txtAppendSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmboPolicy;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnOpenAndClose;
        private System.Windows.Forms.Button btnLog1;
        private System.Windows.Forms.Button btnLogTilClosed;
        private System.Windows.Forms.Button btnLogTilWrapped;
        private System.Windows.Forms.Button btnLog2Files;
    }
}