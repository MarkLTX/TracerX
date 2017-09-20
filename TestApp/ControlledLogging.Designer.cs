namespace TestApp
{
    partial class ControlledLogging
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
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.circularBtn = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.depth1Plus = new System.Windows.Forms.Button();
            this.depth1Minus = new System.Windows.Forms.Button();
            this.depth2Minus = new System.Windows.Forms.Button();
            this.depth2Plus = new System.Windows.Forms.Button();
            this.wrapBtn = new System.Windows.Forms.Button();
            this.wrap2Btn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lastWriteBox = new System.Windows.Forms.TextBox();
            this.sizeBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.positionBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.circularBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.wrappedBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.blockBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.btnLog10 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Log One Line";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(13, 71);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(104, 17);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Log a line every ";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(112, 68);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(44, 20);
            this.textBox1.TabIndex = 2;
            // 
            // circularBtn
            // 
            this.circularBtn.Location = new System.Drawing.Point(149, 12);
            this.circularBtn.Name = "circularBtn";
            this.circularBtn.Size = new System.Drawing.Size(110, 23);
            this.circularBtn.TabIndex = 3;
            this.circularBtn.Text = "Start Circular";
            this.circularBtn.UseVisualStyleBackColor = true;
            this.circularBtn.Click += new System.EventHandler(this.circularBtn_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(15, 207);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(169, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Log to position MaxMB - 1 KB";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // depth1Plus
            // 
            this.depth1Plus.Enabled = false;
            this.depth1Plus.Location = new System.Drawing.Point(13, 137);
            this.depth1Plus.Name = "depth1Plus";
            this.depth1Plus.Size = new System.Drawing.Size(120, 23);
            this.depth1Plus.TabIndex = 5;
            this.depth1Plus.Text = "++Worker1.Depth";
            this.depth1Plus.UseVisualStyleBackColor = true;
            this.depth1Plus.Click += new System.EventHandler(this.depth1Plus_Click);
            // 
            // depth1Minus
            // 
            this.depth1Minus.Enabled = false;
            this.depth1Minus.Location = new System.Drawing.Point(139, 137);
            this.depth1Minus.Name = "depth1Minus";
            this.depth1Minus.Size = new System.Drawing.Size(120, 23);
            this.depth1Minus.TabIndex = 6;
            this.depth1Minus.Text = "--Worker1.Depth";
            this.depth1Minus.UseVisualStyleBackColor = true;
            this.depth1Minus.Click += new System.EventHandler(this.depth1Minus_Click);
            // 
            // depth2Minus
            // 
            this.depth2Minus.Enabled = false;
            this.depth2Minus.Location = new System.Drawing.Point(139, 166);
            this.depth2Minus.Name = "depth2Minus";
            this.depth2Minus.Size = new System.Drawing.Size(120, 23);
            this.depth2Minus.TabIndex = 8;
            this.depth2Minus.Text = "--Worker2.Depth";
            this.depth2Minus.UseVisualStyleBackColor = true;
            this.depth2Minus.Click += new System.EventHandler(this.depth2Minus_Click);
            // 
            // depth2Plus
            // 
            this.depth2Plus.Enabled = false;
            this.depth2Plus.Location = new System.Drawing.Point(13, 166);
            this.depth2Plus.Name = "depth2Plus";
            this.depth2Plus.Size = new System.Drawing.Size(120, 23);
            this.depth2Plus.TabIndex = 7;
            this.depth2Plus.Text = "++Worker2.Depth";
            this.depth2Plus.UseVisualStyleBackColor = true;
            this.depth2Plus.Click += new System.EventHandler(this.depth2Plus_Click);
            // 
            // wrapBtn
            // 
            this.wrapBtn.Location = new System.Drawing.Point(13, 236);
            this.wrapBtn.Name = "wrapBtn";
            this.wrapBtn.Size = new System.Drawing.Size(169, 23);
            this.wrapBtn.TabIndex = 9;
            this.wrapBtn.Text = "Wrap Past Current Position";
            this.wrapBtn.UseVisualStyleBackColor = true;
            this.wrapBtn.Click += new System.EventHandler(this.wrapBtn_Click);
            // 
            // wrap2Btn
            // 
            this.wrap2Btn.Location = new System.Drawing.Point(13, 265);
            this.wrap2Btn.Name = "wrap2Btn";
            this.wrap2Btn.Size = new System.Drawing.Size(169, 23);
            this.wrap2Btn.TabIndex = 10;
            this.wrap2Btn.Text = "Wrap To Circular Start";
            this.wrap2Btn.UseVisualStyleBackColor = true;
            this.wrap2Btn.Click += new System.EventHandler(this.wrap2Btn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(162, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "milliseconds.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 306);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Last write time:";
            // 
            // lastWriteBox
            // 
            this.lastWriteBox.Location = new System.Drawing.Point(96, 303);
            this.lastWriteBox.Name = "lastWriteBox";
            this.lastWriteBox.ReadOnly = true;
            this.lastWriteBox.Size = new System.Drawing.Size(131, 20);
            this.lastWriteBox.TabIndex = 13;
            // 
            // sizeBox
            // 
            this.sizeBox.Location = new System.Drawing.Point(96, 329);
            this.sizeBox.Name = "sizeBox";
            this.sizeBox.ReadOnly = true;
            this.sizeBox.Size = new System.Drawing.Size(131, 20);
            this.sizeBox.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 332);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "File size:";
            // 
            // positionBox
            // 
            this.positionBox.Location = new System.Drawing.Point(96, 355);
            this.positionBox.Name = "positionBox";
            this.positionBox.ReadOnly = true;
            this.positionBox.Size = new System.Drawing.Size(131, 20);
            this.positionBox.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 358);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "File position:";
            // 
            // circularBox
            // 
            this.circularBox.Location = new System.Drawing.Point(96, 381);
            this.circularBox.Name = "circularBox";
            this.circularBox.ReadOnly = true;
            this.circularBox.Size = new System.Drawing.Size(131, 20);
            this.circularBox.TabIndex = 19;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 384);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Circular started:";
            // 
            // wrappedBox
            // 
            this.wrappedBox.Location = new System.Drawing.Point(96, 407);
            this.wrappedBox.Name = "wrappedBox";
            this.wrappedBox.ReadOnly = true;
            this.wrappedBox.Size = new System.Drawing.Size(131, 20);
            this.wrappedBox.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 410);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Ever wrapped";
            // 
            // blockBox
            // 
            this.blockBox.Location = new System.Drawing.Point(96, 433);
            this.blockBox.Name = "blockBox";
            this.blockBox.ReadOnly = true;
            this.blockBox.Size = new System.Drawing.Size(131, 20);
            this.blockBox.TabIndex = 23;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 436);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Current block:";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(15, 108);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(244, 23);
            this.button3.TabIndex = 24;
            this.button3.Text = "Start Worker threads";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnLog10
            // 
            this.btnLog10.Location = new System.Drawing.Point(12, 41);
            this.btnLog10.Name = "btnLog10";
            this.btnLog10.Size = new System.Drawing.Size(110, 23);
            this.btnLog10.TabIndex = 25;
            this.btnLog10.Text = "Log 10 Lines";
            this.btnLog10.UseVisualStyleBackColor = true;
            this.btnLog10.Click += new System.EventHandler(this.btnLog10_Click);
            // 
            // ControlledLogging
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 492);
            this.Controls.Add(this.btnLog10);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.blockBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.wrappedBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.circularBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.positionBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.sizeBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lastWriteBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.wrap2Btn);
            this.Controls.Add(this.wrapBtn);
            this.Controls.Add(this.depth2Minus);
            this.Controls.Add(this.depth2Plus);
            this.Controls.Add(this.depth1Minus);
            this.Controls.Add(this.depth1Plus);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.circularBtn);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button1);
            this.Name = "ControlledLogging";
            this.Text = "ControlledLogging";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button circularBtn;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button depth1Plus;
        private System.Windows.Forms.Button depth1Minus;
        private System.Windows.Forms.Button depth2Minus;
        private System.Windows.Forms.Button depth2Plus;
        private System.Windows.Forms.Button wrapBtn;
        private System.Windows.Forms.Button wrap2Btn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox lastWriteBox;
        private System.Windows.Forms.TextBox sizeBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox positionBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox circularBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox wrappedBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox blockBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btnLog10;
    }
}