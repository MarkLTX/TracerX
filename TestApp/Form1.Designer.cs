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
            this.btnTimeOneMillionWithWrapping = new System.Windows.Forms.Button();
            this.btnTimeOneMillionWithoutWrapping = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.createAppendBtn = new System.Windows.Forms.Button();
            this.appendToFileBtn = new System.Windows.Forms.Button();
            this.noCircularRad = new System.Windows.Forms.RadioButton();
            this.noWrapRad = new System.Windows.Forms.RadioButton();
            this.circularWrapsRad = new System.Windows.Forms.RadioButton();
            this.emptyRad = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkUseKb = new System.Windows.Forms.CheckBox();
            this.use00Chk = new System.Windows.Forms.CheckBox();
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
            this.button16 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button17 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.changingThreadNameBtn = new System.Windows.Forms.Button();
            this.btnCrossAppDomains = new System.Windows.Forms.Button();
            this.btnReopenNoncircular = new System.Windows.Forms.Button();
            this.btnReopenCircular = new System.Windows.Forms.Button();
            this.btnAutoReopen = new System.Windows.Forms.Button();
            this.btnEventHandler = new System.Windows.Forms.Button();
            this.btnMultiFileSingleThread = new System.Windows.Forms.Button();
            this.btnMultiFileMultiThread = new System.Windows.Forms.Button();
            this.btnTextTest = new System.Windows.Forms.Button();
            this.chkUsePassword = new System.Windows.Forms.CheckBox();
            this.viewerWrapper1 = new TestApp.ViewerWrapper();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // emptyBtn
            // 
            this.emptyBtn.Location = new System.Drawing.Point(13, 39);
            this.emptyBtn.Name = "emptyBtn";
            this.emptyBtn.Size = new System.Drawing.Size(145, 23);
            this.emptyBtn.TabIndex = 0;
            this.emptyBtn.Text = "Make Empty File *";
            this.emptyBtn.UseVisualStyleBackColor = true;
            this.emptyBtn.Click += new System.EventHandler(this.emptyBtn_Click);
            // 
            // oneLineBtn
            // 
            this.oneLineBtn.Location = new System.Drawing.Point(161, 39);
            this.oneLineBtn.Name = "oneLineBtn";
            this.oneLineBtn.Size = new System.Drawing.Size(145, 23);
            this.oneLineBtn.TabIndex = 2;
            this.oneLineBtn.Text = "Log One Line *";
            this.oneLineBtn.UseVisualStyleBackColor = true;
            this.oneLineBtn.Click += new System.EventHandler(this.oneLineBtn_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 68);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(145, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Five Threads Non-Circular *";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(161, 68);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(145, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Stop At Circular *";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(161, 97);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(145, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Circular With 1 Block";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(13, 97);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(145, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "One Thread Wraps *";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(13, 126);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(145, 23);
            this.button5.TabIndex = 7;
            this.button5.Text = "Five Threads Wrap *";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(161, 126);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(145, 23);
            this.button6.TabIndex = 8;
            this.button6.Text = "Start Circular By Time";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(13, 155);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(145, 23);
            this.button7.TabIndex = 9;
            this.button7.Text = "Missing Method Entry";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(161, 155);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(145, 23);
            this.button8.TabIndex = 10;
            this.button8.Text = "Missing Method Exit";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(13, 184);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(145, 23);
            this.button9.TabIndex = 11;
            this.button9.Text = "Missing Entries and Exits";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(13, 213);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(145, 23);
            this.button10.TabIndex = 12;
            this.button10.Text = "Log All Unicode Chars";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(13, 300);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(293, 23);
            this.button11.TabIndex = 13;
            this.button11.Text = "Log More Than Uint.MaxValue lines (takes forever)";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(161, 213);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(145, 23);
            this.button12.TabIndex = 14;
            this.button12.Text = "Controlled Logging...";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // btnTimeOneMillionWithWrapping
            // 
            this.btnTimeOneMillionWithWrapping.Location = new System.Drawing.Point(13, 271);
            this.btnTimeOneMillionWithWrapping.Name = "btnTimeOneMillionWithWrapping";
            this.btnTimeOneMillionWithWrapping.Size = new System.Drawing.Size(293, 23);
            this.btnTimeOneMillionWithWrapping.TabIndex = 15;
            this.btnTimeOneMillionWithWrapping.Text = "Time to log 1 Million messages with wrapping *";
            this.btnTimeOneMillionWithWrapping.UseVisualStyleBackColor = true;
            this.btnTimeOneMillionWithWrapping.Click += new System.EventHandler(this.btnTimeOneMillionWithWrapping_Click);
            // 
            // btnTimeOneMillionWithoutWrapping
            // 
            this.btnTimeOneMillionWithoutWrapping.Location = new System.Drawing.Point(13, 242);
            this.btnTimeOneMillionWithoutWrapping.Name = "btnTimeOneMillionWithoutWrapping";
            this.btnTimeOneMillionWithoutWrapping.Size = new System.Drawing.Size(292, 23);
            this.btnTimeOneMillionWithoutWrapping.TabIndex = 16;
            this.btnTimeOneMillionWithoutWrapping.Text = "Time to log 1 Million messages without wrapping *";
            this.btnTimeOneMillionWithoutWrapping.UseVisualStyleBackColor = true;
            this.btnTimeOneMillionWithoutWrapping.Click += new System.EventHandler(this.btnTimeOneMillionWithoutWrapping_Click);
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(161, 184);
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
            this.createAppendBtn.Text = "Create file for appending *";
            this.createAppendBtn.UseVisualStyleBackColor = true;
            this.createAppendBtn.Click += new System.EventHandler(this.createAppendBtn_Click);
            // 
            // appendToFileBtn
            // 
            this.appendToFileBtn.Location = new System.Drawing.Point(157, 19);
            this.appendToFileBtn.Name = "appendToFileBtn";
            this.appendToFileBtn.Size = new System.Drawing.Size(145, 23);
            this.appendToFileBtn.TabIndex = 19;
            this.appendToFileBtn.Text = "Append to file *";
            this.appendToFileBtn.UseVisualStyleBackColor = true;
            this.appendToFileBtn.Click += new System.EventHandler(this.appendToFileBtn_Click);
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
            this.groupBox1.Controls.Add(this.chkUseKb);
            this.groupBox1.Controls.Add(this.use00Chk);
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
            this.groupBox1.Location = new System.Drawing.Point(13, 329);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(321, 203);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Appending";
            // 
            // chkUseKb
            // 
            this.chkUseKb.AutoSize = true;
            this.chkUseKb.Location = new System.Drawing.Point(197, 137);
            this.chkUseKb.Name = "chkUseKb";
            this.chkUseKb.Size = new System.Drawing.Size(102, 17);
            this.chkUseKb.TabIndex = 35;
            this.chkUseKb.Text = "Use Kb for sizes";
            this.chkUseKb.UseVisualStyleBackColor = true;
            // 
            // use00Chk
            // 
            this.use00Chk.AutoSize = true;
            this.use00Chk.Location = new System.Drawing.Point(148, 48);
            this.use00Chk.Name = "use00Chk";
            this.use00Chk.Size = new System.Drawing.Size(111, 17);
            this.use00Chk.TabIndex = 34;
            this.use00Chk.Text = "Use _00 file name";
            this.use00Chk.UseVisualStyleBackColor = true;
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
            this.textWriterTraceListenerBtn.Location = new System.Drawing.Point(161, 538);
            this.textWriterTraceListenerBtn.Name = "textWriterTraceListenerBtn";
            this.textWriterTraceListenerBtn.Size = new System.Drawing.Size(144, 23);
            this.textWriterTraceListenerBtn.TabIndex = 27;
            this.textWriterTraceListenerBtn.Text = "TextWriterTraceListener";
            this.textWriterTraceListenerBtn.UseVisualStyleBackColor = true;
            this.textWriterTraceListenerBtn.Click += new System.EventHandler(this.textWriterTraceListenerBtn_Click);
            // 
            // readConfigBtn
            // 
            this.readConfigBtn.Location = new System.Drawing.Point(13, 538);
            this.readConfigBtn.Name = "readConfigBtn";
            this.readConfigBtn.Size = new System.Drawing.Size(144, 23);
            this.readConfigBtn.TabIndex = 28;
            this.readConfigBtn.Text = "Read Config File";
            this.readConfigBtn.UseVisualStyleBackColor = true;
            this.readConfigBtn.Click += new System.EventHandler(this.readConfigBtn_Click);
            // 
            // button16
            // 
            this.button16.Location = new System.Drawing.Point(13, 567);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(144, 23);
            this.button16.TabIndex = 30;
            this.button16.Text = "Log to All Destinations";
            this.button16.UseVisualStyleBackColor = true;
            this.button16.Click += new System.EventHandler(this.button16_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 55);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(306, 20);
            this.textBox1.TabIndex = 31;
            // 
            // button17
            // 
            this.button17.Location = new System.Drawing.Point(7, 26);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(64, 23);
            this.button17.TabIndex = 32;
            this.button17.Text = "Open File:";
            this.button17.UseVisualStyleBackColor = true;
            this.button17.Click += new System.EventHandler(this.button17_Click);
            // 
            // button18
            // 
            this.button18.Location = new System.Drawing.Point(77, 26);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(64, 23);
            this.button18.TabIndex = 33;
            this.button18.Text = "Close File";
            this.button18.UseVisualStyleBackColor = true;
            this.button18.Click += new System.EventHandler(this.button18_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.button18);
            this.groupBox2.Controls.Add(this.button17);
            this.groupBox2.Location = new System.Drawing.Point(12, 596);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(318, 81);
            this.groupBox2.TabIndex = 34;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Viewer Control";
            // 
            // changingThreadNameBtn
            // 
            this.changingThreadNameBtn.Location = new System.Drawing.Point(161, 567);
            this.changingThreadNameBtn.Name = "changingThreadNameBtn";
            this.changingThreadNameBtn.Size = new System.Drawing.Size(144, 23);
            this.changingThreadNameBtn.TabIndex = 35;
            this.changingThreadNameBtn.Text = "Changing Thread Name";
            this.changingThreadNameBtn.UseVisualStyleBackColor = true;
            this.changingThreadNameBtn.Click += new System.EventHandler(this.changingThreadNameBtn_Click);
            // 
            // btnCrossAppDomains
            // 
            this.btnCrossAppDomains.Location = new System.Drawing.Point(164, 683);
            this.btnCrossAppDomains.Name = "btnCrossAppDomains";
            this.btnCrossAppDomains.Size = new System.Drawing.Size(145, 23);
            this.btnCrossAppDomains.TabIndex = 36;
            this.btnCrossAppDomains.Text = "Cross App Domains";
            this.btnCrossAppDomains.UseVisualStyleBackColor = true;
            this.btnCrossAppDomains.Click += new System.EventHandler(this.btnCrossAppDomains_Click);
            // 
            // btnReopenNoncircular
            // 
            this.btnReopenNoncircular.Location = new System.Drawing.Point(13, 683);
            this.btnReopenNoncircular.Name = "btnReopenNoncircular";
            this.btnReopenNoncircular.Size = new System.Drawing.Size(145, 23);
            this.btnReopenNoncircular.TabIndex = 37;
            this.btnReopenNoncircular.Text = "Reopen Non-Circular";
            this.btnReopenNoncircular.UseVisualStyleBackColor = true;
            this.btnReopenNoncircular.Click += new System.EventHandler(this.btnReopenNoncircular_Click);
            // 
            // btnReopenCircular
            // 
            this.btnReopenCircular.Location = new System.Drawing.Point(12, 712);
            this.btnReopenCircular.Name = "btnReopenCircular";
            this.btnReopenCircular.Size = new System.Drawing.Size(145, 23);
            this.btnReopenCircular.TabIndex = 38;
            this.btnReopenCircular.Text = "Reopen Circular";
            this.btnReopenCircular.UseVisualStyleBackColor = true;
            this.btnReopenCircular.Click += new System.EventHandler(this.btnReopenCircular_Click);
            // 
            // btnAutoReopen
            // 
            this.btnAutoReopen.Location = new System.Drawing.Point(164, 712);
            this.btnAutoReopen.Name = "btnAutoReopen";
            this.btnAutoReopen.Size = new System.Drawing.Size(145, 23);
            this.btnAutoReopen.TabIndex = 39;
            this.btnAutoReopen.Text = "Auto Reopen";
            this.btnAutoReopen.UseVisualStyleBackColor = true;
            this.btnAutoReopen.Click += new System.EventHandler(this.btnAutoReopen_Click);
            // 
            // btnEventHandler
            // 
            this.btnEventHandler.Location = new System.Drawing.Point(12, 741);
            this.btnEventHandler.Name = "btnEventHandler";
            this.btnEventHandler.Size = new System.Drawing.Size(145, 23);
            this.btnEventHandler.TabIndex = 40;
            this.btnEventHandler.Text = "EventHandler Dest";
            this.btnEventHandler.UseVisualStyleBackColor = true;
            this.btnEventHandler.Click += new System.EventHandler(this.btnEventHandler_Click);
            // 
            // btnMultiFileSingleThread
            // 
            this.btnMultiFileSingleThread.Location = new System.Drawing.Point(164, 741);
            this.btnMultiFileSingleThread.Name = "btnMultiFileSingleThread";
            this.btnMultiFileSingleThread.Size = new System.Drawing.Size(145, 23);
            this.btnMultiFileSingleThread.TabIndex = 41;
            this.btnMultiFileSingleThread.Text = "Multi-File Single-Thread";
            this.btnMultiFileSingleThread.UseVisualStyleBackColor = true;
            this.btnMultiFileSingleThread.Click += new System.EventHandler(this.btnMultiFileSingleThread_Click);
            // 
            // btnMultiFileMultiThread
            // 
            this.btnMultiFileMultiThread.Location = new System.Drawing.Point(164, 770);
            this.btnMultiFileMultiThread.Name = "btnMultiFileMultiThread";
            this.btnMultiFileMultiThread.Size = new System.Drawing.Size(145, 23);
            this.btnMultiFileMultiThread.TabIndex = 42;
            this.btnMultiFileMultiThread.Text = "Multi-File Multi-Thread";
            this.btnMultiFileMultiThread.UseVisualStyleBackColor = true;
            this.btnMultiFileMultiThread.Click += new System.EventHandler(this.btnMultiFileMultiThread_Click);
            // 
            // btnTextTest
            // 
            this.btnTextTest.Location = new System.Drawing.Point(13, 770);
            this.btnTextTest.Name = "btnTextTest";
            this.btnTextTest.Size = new System.Drawing.Size(145, 23);
            this.btnTextTest.TabIndex = 43;
            this.btnTextTest.Text = "Test Text File";
            this.btnTextTest.UseVisualStyleBackColor = true;
            this.btnTextTest.Click += new System.EventHandler(this.btnTextTest_Click);
            // 
            // chkUsePassword
            // 
            this.chkUsePassword.AutoSize = true;
            this.chkUsePassword.Location = new System.Drawing.Point(13, 12);
            this.chkUsePassword.Name = "chkUsePassword";
            this.chkUsePassword.Size = new System.Drawing.Size(185, 17);
            this.chkUsePassword.TabIndex = 44;
            this.chkUsePassword.Text = "Use password \"abc123\" for * files";
            this.chkUsePassword.UseVisualStyleBackColor = true;
            this.chkUsePassword.CheckedChanged += new System.EventHandler(this.chkUsePassword_CheckedChanged);
            // 
            // viewerWrapper1
            // 
            this.viewerWrapper1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.viewerWrapper1.AutoScroll = true;
            this.viewerWrapper1.AutoScrollMinSize = new System.Drawing.Size(100, 150);
            this.viewerWrapper1.BackColor = System.Drawing.Color.Bisque;
            this.viewerWrapper1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewerWrapper1.Location = new System.Drawing.Point(339, 0);
            this.viewerWrapper1.Name = "viewerWrapper1";
            this.viewerWrapper1.Size = new System.Drawing.Size(676, 801);
            this.viewerWrapper1.TabIndex = 29;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 801);
            this.Controls.Add(this.chkUsePassword);
            this.Controls.Add(this.btnTextTest);
            this.Controls.Add(this.btnMultiFileMultiThread);
            this.Controls.Add(this.btnMultiFileSingleThread);
            this.Controls.Add(this.btnEventHandler);
            this.Controls.Add(this.btnAutoReopen);
            this.Controls.Add(this.btnReopenCircular);
            this.Controls.Add(this.btnReopenNoncircular);
            this.Controls.Add(this.btnCrossAppDomains);
            this.Controls.Add(this.changingThreadNameBtn);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button16);
            this.Controls.Add(this.viewerWrapper1);
            this.Controls.Add(this.readConfigBtn);
            this.Controls.Add(this.textWriterTraceListenerBtn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.btnTimeOneMillionWithoutWrapping);
            this.Controls.Add(this.btnTimeOneMillionWithWrapping);
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
            this.Controls.Add(this.emptyBtn);
            this.Name = "Form1";
            this.Text = "TracerX Test App";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button emptyBtn;
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
        private System.Windows.Forms.Button btnTimeOneMillionWithWrapping;
        private System.Windows.Forms.Button btnTimeOneMillionWithoutWrapping;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button createAppendBtn;
        private System.Windows.Forms.Button appendToFileBtn;
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
        private ViewerWrapper viewerWrapper1;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button17;
        private System.Windows.Forms.Button button18;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox use00Chk;
        private System.Windows.Forms.Button changingThreadNameBtn;
        private System.Windows.Forms.Button btnCrossAppDomains;
        private System.Windows.Forms.Button btnReopenNoncircular;
        private System.Windows.Forms.Button btnReopenCircular;
        private System.Windows.Forms.Button btnAutoReopen;
        private System.Windows.Forms.CheckBox chkUseKb;
        private System.Windows.Forms.Button btnEventHandler;
        private System.Windows.Forms.Button btnMultiFileSingleThread;
        private System.Windows.Forms.Button btnMultiFileMultiThread;
        private System.Windows.Forms.Button btnTextTest;
        private System.Windows.Forms.CheckBox chkUsePassword;
    }
}

