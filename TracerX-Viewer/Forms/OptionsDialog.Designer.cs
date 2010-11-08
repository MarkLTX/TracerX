namespace TracerX.Viewer {
    partial class OptionsDialog {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsDialog));
            this.cancel = new System.Windows.Forms.Button();
            this.apply = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.miscPage = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtFileSize = new System.Windows.Forms.TextBox();
            this.chkDigitGrouping = new System.Windows.Forms.CheckBox();
            this.timePage = new System.Windows.Forms.TabPage();
            this.chkDuplicateTimes = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtCustomTimeFormat = new System.Windows.Forms.TextBox();
            this.chkUseCustomTime = new System.Windows.Forms.CheckBox();
            this.showAbsolute = new System.Windows.Forms.RadioButton();
            this.showRelative = new System.Windows.Forms.RadioButton();
            this.threadsPage = new System.Windows.Forms.TabPage();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.radThreadName = new System.Windows.Forms.RadioButton();
            this.radThreadNumber = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.textPage = new System.Windows.Forms.TabPage();
            this.hex = new System.Windows.Forms.Label();
            this.indentAmount = new System.Windows.Forms.TextBox();
            this.indentChar = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.expandLinesWithNewlines = new System.Windows.Forms.CheckBox();
            this.autoUpdatePage = new System.Windows.Forms.TabPage();
            this.autoUpdate = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.versionPage = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtVersionInterval = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.chkReapplyFilter = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtMaxRecords = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.miscPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.timePage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.threadsPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.textPage.SuspendLayout();
            this.autoUpdatePage.SuspendLayout();
            this.versionPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(302, 184);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 6;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // apply
            // 
            this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.apply.Enabled = false;
            this.apply.Location = new System.Drawing.Point(221, 184);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(75, 23);
            this.apply.TabIndex = 5;
            this.apply.Text = "Apply";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // ok
            // 
            this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ok.Enabled = false;
            this.ok.Location = new System.Drawing.Point(140, 184);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 4;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.miscPage);
            this.tabControl1.Controls.Add(this.timePage);
            this.tabControl1.Controls.Add(this.threadsPage);
            this.tabControl1.Controls.Add(this.textPage);
            this.tabControl1.Controls.Add(this.autoUpdatePage);
            this.tabControl1.Controls.Add(this.versionPage);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(391, 176);
            this.tabControl1.TabIndex = 7;
            this.tabControl1.Deselecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Deselecting);
            // 
            // miscPage
            // 
            this.miscPage.Controls.Add(this.chkReapplyFilter);
            this.miscPage.Controls.Add(this.groupBox1);
            this.miscPage.Controls.Add(this.chkDigitGrouping);
            this.miscPage.Location = new System.Drawing.Point(4, 22);
            this.miscPage.Name = "miscPage";
            this.miscPage.Size = new System.Drawing.Size(383, 150);
            this.miscPage.TabIndex = 1;
            this.miscPage.Text = "Miscellaneous";
            this.miscPage.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.txtFileSize);
            this.groupBox1.Location = new System.Drawing.Point(8, 52);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(365, 92);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Network files";
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(6, 58);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(352, 31);
            this.label11.TabIndex = 6;
            this.label11.Text = "When loading larger files from network locations, TracerX makes a temp local copy" +
                " for faster loading.  Specify 0 to disable copying.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(70, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "KB";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 16);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(300, 13);
            this.label12.TabIndex = 4;
            this.label12.Text = "Specify the maximum file size to load directly over the network.";
            // 
            // txtFileSize
            // 
            this.txtFileSize.Location = new System.Drawing.Point(9, 34);
            this.txtFileSize.MaxLength = 6;
            this.txtFileSize.Name = "txtFileSize";
            this.txtFileSize.Size = new System.Drawing.Size(55, 20);
            this.txtFileSize.TabIndex = 2;
            this.txtFileSize.TextChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // chkDigitGrouping
            // 
            this.chkDigitGrouping.AutoSize = true;
            this.chkDigitGrouping.Checked = true;
            this.chkDigitGrouping.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDigitGrouping.Location = new System.Drawing.Point(9, 4);
            this.chkDigitGrouping.Name = "chkDigitGrouping";
            this.chkDigitGrouping.Size = new System.Drawing.Size(318, 17);
            this.chkDigitGrouping.TabIndex = 0;
            this.chkDigitGrouping.Text = "Use digit grouping (i.e. thousands separators) for line numbers.";
            this.chkDigitGrouping.UseVisualStyleBackColor = true;
            this.chkDigitGrouping.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // timePage
            // 
            this.timePage.Controls.Add(this.chkDuplicateTimes);
            this.timePage.Controls.Add(this.groupBox2);
            this.timePage.Location = new System.Drawing.Point(4, 22);
            this.timePage.Name = "timePage";
            this.timePage.Padding = new System.Windows.Forms.Padding(3);
            this.timePage.Size = new System.Drawing.Size(383, 150);
            this.timePage.TabIndex = 0;
            this.timePage.Text = "Time";
            this.timePage.UseVisualStyleBackColor = true;
            // 
            // chkDuplicateTimes
            // 
            this.chkDuplicateTimes.AutoSize = true;
            this.chkDuplicateTimes.Location = new System.Drawing.Point(7, 8);
            this.chkDuplicateTimes.Name = "chkDuplicateTimes";
            this.chkDuplicateTimes.Size = new System.Drawing.Size(160, 17);
            this.chkDuplicateTimes.TabIndex = 5;
            this.chkDuplicateTimes.Text = "Show duplicate time stamps.";
            this.chkDuplicateTimes.UseVisualStyleBackColor = true;
            this.chkDuplicateTimes.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtCustomTimeFormat);
            this.groupBox2.Controls.Add(this.chkUseCustomTime);
            this.groupBox2.Controls.Add(this.showAbsolute);
            this.groupBox2.Controls.Add(this.showRelative);
            this.groupBox2.Location = new System.Drawing.Point(7, 37);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(370, 85);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Format";
            // 
            // txtCustomTimeFormat
            // 
            this.txtCustomTimeFormat.Location = new System.Drawing.Point(178, 54);
            this.txtCustomTimeFormat.Name = "txtCustomTimeFormat";
            this.txtCustomTimeFormat.Size = new System.Drawing.Size(188, 20);
            this.txtCustomTimeFormat.TabIndex = 7;
            this.txtCustomTimeFormat.TextChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // chkUseCustomTime
            // 
            this.chkUseCustomTime.AutoSize = true;
            this.chkUseCustomTime.Location = new System.Drawing.Point(27, 56);
            this.chkUseCustomTime.Name = "chkUseCustomTime";
            this.chkUseCustomTime.Size = new System.Drawing.Size(145, 17);
            this.chkUseCustomTime.TabIndex = 6;
            this.chkUseCustomTime.Text = "Use custom format string:";
            this.chkUseCustomTime.UseVisualStyleBackColor = true;
            this.chkUseCustomTime.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // showAbsolute
            // 
            this.showAbsolute.AutoSize = true;
            this.showAbsolute.Location = new System.Drawing.Point(6, 33);
            this.showAbsolute.Name = "showAbsolute";
            this.showAbsolute.Size = new System.Drawing.Size(117, 17);
            this.showAbsolute.TabIndex = 2;
            this.showAbsolute.TabStop = true;
            this.showAbsolute.Text = "Show absolute time";
            this.showAbsolute.UseVisualStyleBackColor = true;
            this.showAbsolute.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // showRelative
            // 
            this.showRelative.AutoSize = true;
            this.showRelative.Location = new System.Drawing.Point(6, 15);
            this.showRelative.Name = "showRelative";
            this.showRelative.Size = new System.Drawing.Size(111, 17);
            this.showRelative.TabIndex = 1;
            this.showRelative.TabStop = true;
            this.showRelative.Text = "Show relative time";
            this.showRelative.UseVisualStyleBackColor = true;
            this.showRelative.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // threadsPage
            // 
            this.threadsPage.Controls.Add(this.pictureBox4);
            this.threadsPage.Controls.Add(this.pictureBox3);
            this.threadsPage.Controls.Add(this.pictureBox2);
            this.threadsPage.Controls.Add(this.pictureBox1);
            this.threadsPage.Controls.Add(this.radThreadName);
            this.threadsPage.Controls.Add(this.radThreadNumber);
            this.threadsPage.Controls.Add(this.label3);
            this.threadsPage.Location = new System.Drawing.Point(4, 22);
            this.threadsPage.Name = "threadsPage";
            this.threadsPage.Padding = new System.Windows.Forms.Padding(3);
            this.threadsPage.Size = new System.Drawing.Size(383, 150);
            this.threadsPage.TabIndex = 5;
            this.threadsPage.Text = "Threads";
            this.threadsPage.UseVisualStyleBackColor = true;
            // 
            // pictureBox4
            // 
            this.pictureBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox4.Image = global::TracerX.Properties.Resources.SameThreadNext;
            this.pictureBox4.Location = new System.Drawing.Point(251, 96);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(18, 18);
            this.pictureBox4.TabIndex = 6;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox3.Image = global::TracerX.Properties.Resources.SameThreadPrev;
            this.pictureBox3.Location = new System.Drawing.Point(229, 96);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(18, 18);
            this.pictureBox3.TabIndex = 5;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Image = global::TracerX.Properties.Resources.AnyThreadNext;
            this.pictureBox2.Location = new System.Drawing.Point(207, 96);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(18, 18);
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Image = global::TracerX.Properties.Resources.AnyThreadPrev;
            this.pictureBox1.Location = new System.Drawing.Point(185, 96);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(18, 18);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // radThreadName
            // 
            this.radThreadName.AutoSize = true;
            this.radThreadName.Location = new System.Drawing.Point(11, 109);
            this.radThreadName.Name = "radThreadName";
            this.radThreadName.Size = new System.Drawing.Size(138, 17);
            this.radThreadName.TabIndex = 2;
            this.radThreadName.TabStop = true;
            this.radThreadName.Text = "Search by thread name.";
            this.radThreadName.UseVisualStyleBackColor = true;
            this.radThreadName.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // radThreadNumber
            // 
            this.radThreadNumber.AutoSize = true;
            this.radThreadNumber.Location = new System.Drawing.Point(11, 86);
            this.radThreadNumber.Name = "radThreadNumber";
            this.radThreadNumber.Size = new System.Drawing.Size(147, 17);
            this.radThreadNumber.TabIndex = 1;
            this.radThreadNumber.TabStop = true;
            this.radThreadNumber.Text = "Search by thread number.";
            this.radThreadNumber.UseVisualStyleBackColor = true;
            this.radThreadNumber.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(365, 70);
            this.label3.TabIndex = 0;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // textPage
            // 
            this.textPage.Controls.Add(this.hex);
            this.textPage.Controls.Add(this.indentAmount);
            this.textPage.Controls.Add(this.indentChar);
            this.textPage.Controls.Add(this.label2);
            this.textPage.Controls.Add(this.label1);
            this.textPage.Controls.Add(this.expandLinesWithNewlines);
            this.textPage.Location = new System.Drawing.Point(4, 22);
            this.textPage.Name = "textPage";
            this.textPage.Padding = new System.Windows.Forms.Padding(3);
            this.textPage.Size = new System.Drawing.Size(383, 150);
            this.textPage.TabIndex = 2;
            this.textPage.Text = "Text";
            this.textPage.UseVisualStyleBackColor = true;
            // 
            // hex
            // 
            this.hex.AutoSize = true;
            this.hex.Location = new System.Drawing.Point(147, 42);
            this.hex.Name = "hex";
            this.hex.Size = new System.Drawing.Size(24, 13);
            this.hex.TabIndex = 5;
            this.hex.Text = "hex";
            // 
            // indentAmount
            // 
            this.indentAmount.Location = new System.Drawing.Point(120, 62);
            this.indentAmount.MaxLength = 1;
            this.indentAmount.Name = "indentAmount";
            this.indentAmount.Size = new System.Drawing.Size(20, 20);
            this.indentAmount.TabIndex = 4;
            this.indentAmount.TextChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // indentChar
            // 
            this.indentChar.Location = new System.Drawing.Point(120, 40);
            this.indentChar.MaxLength = 1;
            this.indentChar.Name = "indentChar";
            this.indentChar.Size = new System.Drawing.Size(20, 20);
            this.indentChar.TabIndex = 3;
            this.indentChar.TextChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Indentation amount";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Indentation character";
            // 
            // expandLinesWithNewlines
            // 
            this.expandLinesWithNewlines.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.expandLinesWithNewlines.Location = new System.Drawing.Point(9, 7);
            this.expandLinesWithNewlines.Name = "expandLinesWithNewlines";
            this.expandLinesWithNewlines.Size = new System.Drawing.Size(292, 33);
            this.expandLinesWithNewlines.TabIndex = 0;
            this.expandLinesWithNewlines.Text = "Expand text with embedded newlines (yellow triangles) when file is loaded.";
            this.expandLinesWithNewlines.UseVisualStyleBackColor = true;
            this.expandLinesWithNewlines.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // autoUpdatePage
            // 
            this.autoUpdatePage.Controls.Add(this.txtMaxRecords);
            this.autoUpdatePage.Controls.Add(this.label13);
            this.autoUpdatePage.Controls.Add(this.autoUpdate);
            this.autoUpdatePage.Controls.Add(this.label5);
            this.autoUpdatePage.Location = new System.Drawing.Point(4, 22);
            this.autoUpdatePage.Name = "autoUpdatePage";
            this.autoUpdatePage.Padding = new System.Windows.Forms.Padding(3);
            this.autoUpdatePage.Size = new System.Drawing.Size(383, 150);
            this.autoUpdatePage.TabIndex = 3;
            this.autoUpdatePage.Text = "Auto-Update";
            this.autoUpdatePage.UseVisualStyleBackColor = true;
            // 
            // autoUpdate
            // 
            this.autoUpdate.AutoSize = true;
            this.autoUpdate.Checked = true;
            this.autoUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoUpdate.Location = new System.Drawing.Point(9, 6);
            this.autoUpdate.Name = "autoUpdate";
            this.autoUpdate.Size = new System.Drawing.Size(371, 17);
            this.autoUpdate.TabIndex = 5;
            this.autoUpdate.Text = "Auto-update (monitor the file and display new records as they are logged).";
            this.autoUpdate.UseVisualStyleBackColor = true;
            this.autoUpdate.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(25, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(292, 31);
            this.label5.TabIndex = 3;
            this.label5.Text = "Select the last row in the log (or press the End key) to stay at the end of the l" +
                "og when it changes.";
            // 
            // versionPage
            // 
            this.versionPage.Controls.Add(this.label10);
            this.versionPage.Controls.Add(this.label9);
            this.versionPage.Controls.Add(this.label8);
            this.versionPage.Controls.Add(this.txtVersionInterval);
            this.versionPage.Controls.Add(this.label7);
            this.versionPage.Controls.Add(this.label6);
            this.versionPage.Location = new System.Drawing.Point(4, 22);
            this.versionPage.Name = "versionPage";
            this.versionPage.Size = new System.Drawing.Size(383, 150);
            this.versionPage.TabIndex = 4;
            this.versionPage.Text = "Version Checking";
            this.versionPage.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(0, 80);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(304, 55);
            this.label10.TabIndex = 5;
            this.label10.Text = "Set VersionCheckingAllowed to false in the application config file to disable ver" +
                "sion checking and hide this tab page.";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(0, 63);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(249, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Note: Enter 0 to check every time the viewer starts.";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(73, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "days.";
            // 
            // txtVersionInterval
            // 
            this.txtVersionInterval.Location = new System.Drawing.Point(35, 34);
            this.txtVersionInterval.MaxLength = 5;
            this.txtVersionInterval.Name = "txtVersionInterval";
            this.txtVersionInterval.Size = new System.Drawing.Size(35, 20);
            this.txtVersionInterval.TabIndex = 2;
            this.txtVersionInterval.TextChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Every";
            // 
            // label6
            // 
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(383, 32);
            this.label6.TabIndex = 0;
            this.label6.Text = "How often should the viewer check for a newer version of TracerX on the CodePlex " +
                "website?";
            // 
            // chkReapplyFilter
            // 
            this.chkReapplyFilter.AutoSize = true;
            this.chkReapplyFilter.Checked = true;
            this.chkReapplyFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReapplyFilter.Location = new System.Drawing.Point(9, 27);
            this.chkReapplyFilter.Name = "chkReapplyFilter";
            this.chkReapplyFilter.Size = new System.Drawing.Size(215, 17);
            this.chkReapplyFilter.TabIndex = 6;
            this.chkReapplyFilter.Text = "Reapply current filter after refreshing file.";
            this.chkReapplyFilter.UseVisualStyleBackColor = true;
            this.chkReapplyFilter.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(9, 61);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(278, 13);
            this.label13.TabIndex = 6;
            this.label13.Text = "Cancel auto-updating when this many records are loaded:";
            // 
            // txtMaxRecords
            // 
            this.txtMaxRecords.Location = new System.Drawing.Point(12, 77);
            this.txtMaxRecords.MaxLength = 9;
            this.txtMaxRecords.Name = "txtMaxRecords";
            this.txtMaxRecords.Size = new System.Drawing.Size(80, 20);
            this.txtMaxRecords.TabIndex = 7;
            this.txtMaxRecords.TextChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // OptionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 217);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.apply);
            this.Controls.Add(this.ok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TracerX Options";
            this.tabControl1.ResumeLayout(false);
            this.miscPage.ResumeLayout(false);
            this.miscPage.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.timePage.ResumeLayout(false);
            this.timePage.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.threadsPage.ResumeLayout(false);
            this.threadsPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.textPage.ResumeLayout(false);
            this.textPage.PerformLayout();
            this.autoUpdatePage.ResumeLayout(false);
            this.autoUpdatePage.PerformLayout();
            this.versionPage.ResumeLayout(false);
            this.versionPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button apply;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.TabPage timePage;
        private System.Windows.Forms.RadioButton showAbsolute;
        private System.Windows.Forms.RadioButton showRelative;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabPage miscPage;
        private System.Windows.Forms.CheckBox chkDigitGrouping;
        private System.Windows.Forms.TabPage textPage;
        private System.Windows.Forms.CheckBox expandLinesWithNewlines;
        private System.Windows.Forms.TextBox indentAmount;
        private System.Windows.Forms.TextBox indentChar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage autoUpdatePage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtVersionInterval;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.TabPage versionPage;
        public System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Label hex;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox autoUpdate;
        private System.Windows.Forms.TabPage threadsPage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radThreadName;
        private System.Windows.Forms.RadioButton radThreadNumber;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.CheckBox chkDuplicateTimes;
        private System.Windows.Forms.CheckBox chkUseCustomTime;
        private System.Windows.Forms.TextBox txtCustomTimeFormat;
        private System.Windows.Forms.TextBox txtFileSize;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkReapplyFilter;
        private System.Windows.Forms.TextBox txtMaxRecords;
        private System.Windows.Forms.Label label13;
    }
}