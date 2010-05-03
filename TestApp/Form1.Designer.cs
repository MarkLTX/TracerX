namespace TestApp {
    partial class Form1 {
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
            this.emptyBtn = new System.Windows.Forms.Button();
            this.viewerBtn = new System.Windows.Forms.Button();
            this.oneLineBtn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.createAppendBtn = new System.Windows.Forms.Button();
            this.appendToFileBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.noCircularRad = new System.Windows.Forms.RadioButton();
            this.noWrapRad = new System.Windows.Forms.RadioButton();
            this.circularWrapsRad = new System.Windows.Forms.RadioButton();
            this.emptyRad = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.exceedMaxMbRad = new System.Windows.Forms.RadioButton();
            this.textFileChk = new System.Windows.Forms.CheckBox();
            this.circularStartSizeBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.maxSessionSizeBox = new System.Windows.Forms.TextBox();
            this.maxAppendSizeBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textWriterTraceListenerBtn = new System.Windows.Forms.Button();
            this.readConfigBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // emptyBtn
            // 
            this.emptyBtn.Location = new System.Drawing.Point(13, 39);
            this.emptyBtn.Name = "emptyBtn";
            this.emptyBtn.Size = new System.Drawing.Size(75, 23);
            this.emptyBtn.TabIndex = 0;
            this.emptyBtn.Text = "Empty File";
            this.emptyBtn.UseVisualStyleBackColor = true;
            this.emptyBtn.Click += new System.EventHandler(this.emptyBtn_Click);
            // 
            // viewerBtn
            // 
            this.viewerBtn.Location = new System.Drawing.Point(205, 80);
            this.viewerBtn.Name = "viewerBtn";
            this.viewerBtn.Size = new System.Drawing.Size(75, 23);
            this.viewerBtn.TabIndex = 1;
            this.viewerBtn.Text = "Viewer";
            this.viewerBtn.UseVisualStyleBackColor = true;
            this.viewerBtn.Click += new System.EventHandler(this.viewerBtn_Click);
            // 
            // oneLineBtn
            // 
            this.oneLineBtn.Location = new System.Drawing.Point(13, 68);
            this.oneLineBtn.Name = "oneLineBtn";
            this.oneLineBtn.Size = new System.Drawing.Size(75, 23);
            this.oneLineBtn.TabIndex = 2;
            this.oneLineBtn.Text = "One Line";
            this.oneLineBtn.UseVisualStyleBackColor = true;
            this.oneLineBtn.Click += new System.EventHandler(this.oneLineBtn_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 97);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(144, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Five Threads Non-Circular";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 126);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(144, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Stop At Circular ";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(13, 155);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(144, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Circular With 1 Block";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(13, 184);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(144, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "One Thread Wraps";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(13, 213);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(144, 23);
            this.button5.TabIndex = 7;
            this.button5.Text = "Five Threads Wrap";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(13, 242);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(144, 23);
            this.button6.TabIndex = 8;
            this.button6.Text = "Start Circular By Time";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(12, 271);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(144, 23);
            this.button7.TabIndex = 9;
            this.button7.Text = "Missing Method Entry";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(12, 300);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(144, 23);
            this.button8.TabIndex = 10;
            this.button8.Text = "Missing Method Exit";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(12, 329);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(144, 23);
            this.button9.TabIndex = 11;
            this.button9.Text = "Missing Entries and Exits";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(13, 358);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(144, 23);
            this.button10.TabIndex = 12;
            this.button10.Text = "All Unicode Chars";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(13, 387);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(163, 23);
            this.button11.TabIndex = 13;
            this.button11.Text = "More Than Uint.MaxValue lines";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(13, 416);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(163, 23);
            this.button12.TabIndex = 14;
            this.button12.Text = "Controlled Logging...";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(13, 445);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(247, 23);
            this.button13.TabIndex = 15;
            this.button13.Text = "Time to log 1 Million messages with wrapping";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(13, 474);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(247, 23);
            this.button14.TabIndex = 16;
            this.button14.Text = "Time to log 1 Million messages without wrapping";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(12, 503);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(145, 23);
            this.button15.TabIndex = 17;
            this.button15.Text = "Random Method Calls";
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // createAppendBtn
            // 
            this.createAppendBtn.Location = new System.Drawing.Point(6, 19);
            this.createAppendBtn.Name = "createAppendBtn";
            this.createAppendBtn.Size = new System.Drawing.Size(145, 23);
            this.createAppendBtn.TabIndex = 18;
            this.createAppendBtn.Text = "Create file for appending";
            this.createAppendBtn.UseVisualStyleBackColor = true;
            this.createAppendBtn.Click += new System.EventHandler(this.createAppendBtn_Click);
            // 
            // appendToFileBtn
            // 
            this.appendToFileBtn.Location = new System.Drawing.Point(157, 19);
            this.appendToFileBtn.Name = "appendToFileBtn";
            this.appendToFileBtn.Size = new System.Drawing.Size(145, 23);
            this.appendToFileBtn.TabIndex = 19;
            this.appendToFileBtn.Text = "Append to file";
            this.appendToFileBtn.UseVisualStyleBackColor = true;
            this.appendToFileBtn.Click += new System.EventHandler(this.appendToFileBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Each button creates a separate App Domain.";
            // 
            // noCircularRad
            // 
            this.noCircularRad.AutoSize = true;
            this.noCircularRad.Location = new System.Drawing.Point(151, 71);
            this.noCircularRad.Name = "noCircularRad";
            this.noCircularRad.Size = new System.Drawing.Size(86, 17);
            this.noCircularRad.TabIndex = 22;
            this.noCircularRad.Text = "Log one line.";
            this.noCircularRad.UseVisualStyleBackColor = true;
            // 
            // noWrapRad
            // 
            this.noWrapRad.AutoSize = true;
            this.noWrapRad.Location = new System.Drawing.Point(9, 111);
            this.noWrapRad.Name = "noWrapRad";
            this.noWrapRad.Size = new System.Drawing.Size(148, 17);
            this.noWrapRad.TabIndex = 23;
            this.noWrapRad.Text = "Log until circular par starts";
            this.noWrapRad.UseVisualStyleBackColor = true;
            // 
            // circularWrapsRad
            // 
            this.circularWrapsRad.AutoSize = true;
            this.circularWrapsRad.Location = new System.Drawing.Point(163, 111);
            this.circularWrapsRad.Name = "circularWrapsRad";
            this.circularWrapsRad.Size = new System.Drawing.Size(154, 17);
            this.circularWrapsRad.TabIndex = 24;
            this.circularWrapsRad.Text = "Log until circular part wraps";
            this.circularWrapsRad.UseVisualStyleBackColor = true;
            // 
            // emptyRad
            // 
            this.emptyRad.AutoSize = true;
            this.emptyRad.Checked = true;
            this.emptyRad.Location = new System.Drawing.Point(9, 71);
            this.emptyRad.Name = "emptyRad";
            this.emptyRad.Size = new System.Drawing.Size(136, 17);
            this.emptyRad.TabIndex = 25;
            this.emptyRad.TabStop = true;
            this.emptyRad.Text = "Empty (open and close)";
            this.emptyRad.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.exceedMaxMbRad);
            this.groupBox1.Controls.Add(this.textFileChk);
            this.groupBox1.Controls.Add(this.circularStartSizeBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.maxSessionSizeBox);
            this.groupBox1.Controls.Add(this.maxAppendSizeBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.createAppendBtn);
            this.groupBox1.Controls.Add(this.emptyRad);
            this.groupBox1.Controls.Add(this.appendToFileBtn);
            this.groupBox1.Controls.Add(this.circularWrapsRad);
            this.groupBox1.Controls.Add(this.noCircularRad);
            this.groupBox1.Controls.Add(this.noWrapRad);
            this.groupBox1.Location = new System.Drawing.Point(12, 532);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(311, 203);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Appending";
            // 
            // exceedMaxMbRad
            // 
            this.exceedMaxMbRad.AutoSize = true;
            this.exceedMaxMbRad.Location = new System.Drawing.Point(9, 91);
            this.exceedMaxMbRad.Name = "exceedMaxMbRad";
            this.exceedMaxMbRad.Size = new System.Drawing.Size(221, 17);
            this.exceedMaxMbRad.TabIndex = 33;
            this.exceedMaxMbRad.Text = "Disable circular, log until file doesn\'t grow.";
            this.exceedMaxMbRad.UseVisualStyleBackColor = true;
            // 
            // textFileChk
            // 
            this.textFileChk.AutoSize = true;
            this.textFileChk.Location = new System.Drawing.Point(9, 48);
            this.textFileChk.Name = "textFileChk";
            this.textFileChk.Size = new System.Drawing.Size(133, 17);
            this.textFileChk.TabIndex = 32;
            this.textFileChk.Text = "Text file (vs. binary file)";
            this.textFileChk.UseVisualStyleBackColor = true;
            // 
            // circularStartSizeBox
            // 
            this.circularStartSizeBox.Location = new System.Drawing.Point(153, 178);
            this.circularStartSizeBox.Name = "circularStartSizeBox";
            this.circularStartSizeBox.Size = new System.Drawing.Size(38, 20);
            this.circularStartSizeBox.TabIndex = 31;
            this.circularStartSizeBox.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 181);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "Circular start Kb:";
            // 
            // maxSessionSizeBox
            // 
            this.maxSessionSizeBox.Location = new System.Drawing.Point(153, 156);
            this.maxSessionSizeBox.Name = "maxSessionSizeBox";
            this.maxSessionSizeBox.Size = new System.Drawing.Size(38, 20);
            this.maxSessionSizeBox.TabIndex = 29;
            this.maxSessionSizeBox.Text = "1";
            // 
            // maxAppendSizeBox
            // 
            this.maxAppendSizeBox.Location = new System.Drawing.Point(153, 134);
            this.maxAppendSizeBox.Name = "maxAppendSizeBox";
            this.maxAppendSizeBox.Size = new System.Drawing.Size(38, 20);
            this.maxAppendSizeBox.TabIndex = 28;
            this.maxAppendSizeBox.Text = "2";
            this.maxAppendSizeBox.TextChanged += new System.EventHandler(this.maxAppendSizeBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 159);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Max session Mb:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Append if smaller than (Mb):";
            // 
            // textWriterTraceListenerBtn
            // 
            this.textWriterTraceListenerBtn.Location = new System.Drawing.Point(12, 741);
            this.textWriterTraceListenerBtn.Name = "textWriterTraceListenerBtn";
            this.textWriterTraceListenerBtn.Size = new System.Drawing.Size(144, 23);
            this.textWriterTraceListenerBtn.TabIndex = 27;
            this.textWriterTraceListenerBtn.Text = "TextWriterTraceListener";
            this.textWriterTraceListenerBtn.UseVisualStyleBackColor = true;
            this.textWriterTraceListenerBtn.Click += new System.EventHandler(this.textWriterTraceListenerBtn_Click);
            // 
            // readConfigBtn
            // 
            this.readConfigBtn.Location = new System.Drawing.Point(13, 770);
            this.readConfigBtn.Name = "readConfigBtn";
            this.readConfigBtn.Size = new System.Drawing.Size(144, 23);
            this.readConfigBtn.TabIndex = 28;
            this.readConfigBtn.Text = "Read Config File";
            this.readConfigBtn.UseVisualStyleBackColor = true;
            this.readConfigBtn.Click += new System.EventHandler(this.readConfigBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 827);
            this.Controls.Add(this.readConfigBtn);
            this.Controls.Add(this.textWriterTraceListenerBtn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.oneLineBtn);
            this.Controls.Add(this.viewerBtn);
            this.Controls.Add(this.emptyBtn);
            this.Name = "Form1";
            this.Text = "TracerX Test App";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button emptyBtn;
        private System.Windows.Forms.Button viewerBtn;
        private System.Windows.Forms.Button oneLineBtn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button createAppendBtn;
        private System.Windows.Forms.Button appendToFileBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton noCircularRad;
        private System.Windows.Forms.RadioButton noWrapRad;
        private System.Windows.Forms.RadioButton circularWrapsRad;
        private System.Windows.Forms.RadioButton emptyRad;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox maxAppendSizeBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox circularStartSizeBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox maxSessionSizeBox;
        private System.Windows.Forms.Button textWriterTraceListenerBtn;
        private System.Windows.Forms.CheckBox textFileChk;
        private System.Windows.Forms.RadioButton exceedMaxMbRad;
        private System.Windows.Forms.Button readConfigBtn;
    }
}

