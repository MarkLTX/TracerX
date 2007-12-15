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
            this.cancel = new System.Windows.Forms.Button();
            this.apply = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.timePage = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.showAbsolute = new System.Windows.Forms.RadioButton();
            this.showRelative = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dontShowDuplicateTimes = new System.Windows.Forms.RadioButton();
            this.showDuplicateTimes = new System.Windows.Forms.RadioButton();
            this.linePage = new System.Windows.Forms.TabPage();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textPage = new System.Windows.Forms.TabPage();
            this.indentAmount = new System.Windows.Forms.TextBox();
            this.indentChar = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.expandLinesWithNewlines = new System.Windows.Forms.CheckBox();
            this.autoRefreshPage = new System.Windows.Forms.TabPage();
            this.reapplyFilter = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.refreshSeconds = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.versionPage = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtVersionInterval = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.timePage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.linePage.SuspendLayout();
            this.textPage.SuspendLayout();
            this.autoRefreshPage.SuspendLayout();
            this.versionPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(226, 184);
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
            this.apply.Location = new System.Drawing.Point(145, 184);
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
            this.ok.Location = new System.Drawing.Point(64, 184);
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
            this.tabControl1.Controls.Add(this.timePage);
            this.tabControl1.Controls.Add(this.linePage);
            this.tabControl1.Controls.Add(this.textPage);
            this.tabControl1.Controls.Add(this.autoRefreshPage);
            this.tabControl1.Controls.Add(this.versionPage);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(315, 176);
            this.tabControl1.TabIndex = 7;
            this.tabControl1.Deselecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Deselecting);
            // 
            // timePage
            // 
            this.timePage.Controls.Add(this.groupBox2);
            this.timePage.Controls.Add(this.groupBox1);
            this.timePage.Location = new System.Drawing.Point(4, 22);
            this.timePage.Name = "timePage";
            this.timePage.Padding = new System.Windows.Forms.Padding(3);
            this.timePage.Size = new System.Drawing.Size(307, 150);
            this.timePage.TabIndex = 0;
            this.timePage.Text = "Time";
            this.timePage.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.showAbsolute);
            this.groupBox2.Controls.Add(this.showRelative);
            this.groupBox2.Location = new System.Drawing.Point(7, 76);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(294, 63);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Format";
            // 
            // showAbsolute
            // 
            this.showAbsolute.AutoSize = true;
            this.showAbsolute.Location = new System.Drawing.Point(6, 37);
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
            this.showRelative.Location = new System.Drawing.Point(6, 19);
            this.showRelative.Name = "showRelative";
            this.showRelative.Size = new System.Drawing.Size(111, 17);
            this.showRelative.TabIndex = 1;
            this.showRelative.TabStop = true;
            this.showRelative.Text = "Show relative time";
            this.showRelative.UseVisualStyleBackColor = true;
            this.showRelative.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dontShowDuplicateTimes);
            this.groupBox1.Controls.Add(this.showDuplicateTimes);
            this.groupBox1.Location = new System.Drawing.Point(7, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(294, 64);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Duplicates";
            // 
            // dontShowDuplicateTimes
            // 
            this.dontShowDuplicateTimes.AutoSize = true;
            this.dontShowDuplicateTimes.Location = new System.Drawing.Point(6, 19);
            this.dontShowDuplicateTimes.Name = "dontShowDuplicateTimes";
            this.dontShowDuplicateTimes.Size = new System.Drawing.Size(229, 17);
            this.dontShowDuplicateTimes.TabIndex = 4;
            this.dontShowDuplicateTimes.TabStop = true;
            this.dontShowDuplicateTimes.Text = "Show only when different from previous line";
            this.dontShowDuplicateTimes.UseVisualStyleBackColor = true;
            this.dontShowDuplicateTimes.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // showDuplicateTimes
            // 
            this.showDuplicateTimes.AutoSize = true;
            this.showDuplicateTimes.Location = new System.Drawing.Point(6, 38);
            this.showDuplicateTimes.Name = "showDuplicateTimes";
            this.showDuplicateTimes.Size = new System.Drawing.Size(188, 17);
            this.showDuplicateTimes.TabIndex = 3;
            this.showDuplicateTimes.TabStop = true;
            this.showDuplicateTimes.Text = "Show all times including duplicates";
            this.showDuplicateTimes.UseVisualStyleBackColor = true;
            this.showDuplicateTimes.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // linePage
            // 
            this.linePage.Controls.Add(this.checkBox1);
            this.linePage.Location = new System.Drawing.Point(4, 22);
            this.linePage.Name = "linePage";
            this.linePage.Size = new System.Drawing.Size(307, 150);
            this.linePage.TabIndex = 1;
            this.linePage.Text = "Line Number";
            this.linePage.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(9, 4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(241, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Use digit grouping (i.e. thousands separators).";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // textPage
            // 
            this.textPage.Controls.Add(this.indentAmount);
            this.textPage.Controls.Add(this.indentChar);
            this.textPage.Controls.Add(this.label2);
            this.textPage.Controls.Add(this.label1);
            this.textPage.Controls.Add(this.expandLinesWithNewlines);
            this.textPage.Location = new System.Drawing.Point(4, 22);
            this.textPage.Name = "textPage";
            this.textPage.Padding = new System.Windows.Forms.Padding(3);
            this.textPage.Size = new System.Drawing.Size(307, 150);
            this.textPage.TabIndex = 2;
            this.textPage.Text = "Text";
            this.textPage.UseVisualStyleBackColor = true;
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
            // autoRefreshPage
            // 
            this.autoRefreshPage.Controls.Add(this.reapplyFilter);
            this.autoRefreshPage.Controls.Add(this.label5);
            this.autoRefreshPage.Controls.Add(this.label4);
            this.autoRefreshPage.Controls.Add(this.refreshSeconds);
            this.autoRefreshPage.Controls.Add(this.label3);
            this.autoRefreshPage.Location = new System.Drawing.Point(4, 22);
            this.autoRefreshPage.Name = "autoRefreshPage";
            this.autoRefreshPage.Padding = new System.Windows.Forms.Padding(3);
            this.autoRefreshPage.Size = new System.Drawing.Size(307, 150);
            this.autoRefreshPage.TabIndex = 3;
            this.autoRefreshPage.Text = "Refresh";
            this.autoRefreshPage.UseVisualStyleBackColor = true;
            // 
            // reapplyFilter
            // 
            this.reapplyFilter.AutoSize = true;
            this.reapplyFilter.Checked = true;
            this.reapplyFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.reapplyFilter.Location = new System.Drawing.Point(12, 95);
            this.reapplyFilter.Name = "reapplyFilter";
            this.reapplyFilter.Size = new System.Drawing.Size(215, 17);
            this.reapplyFilter.TabIndex = 4;
            this.reapplyFilter.Text = "Reapply current filter after refreshing file.";
            this.reapplyFilter.UseVisualStyleBackColor = true;
            this.reapplyFilter.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(9, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(292, 58);
            this.label5.TabIndex = 3;
            this.label5.Text = "Use the Play and Stop buttons on the toolbar to start and stop the auto-refresh f" +
                "eature.  Select the last row in the log (or press the End key) to stay at the en" +
                "d of the log after every refresh.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(180, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "seconds.";
            // 
            // refreshSeconds
            // 
            this.refreshSeconds.Location = new System.Drawing.Point(138, 5);
            this.refreshSeconds.MaxLength = 4;
            this.refreshSeconds.Name = "refreshSeconds";
            this.refreshSeconds.Size = new System.Drawing.Size(36, 20);
            this.refreshSeconds.TabIndex = 1;
            this.refreshSeconds.TextChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Check for changes every";
            // 
            // versionPage
            // 
            this.versionPage.Controls.Add(this.label9);
            this.versionPage.Controls.Add(this.label8);
            this.versionPage.Controls.Add(this.txtVersionInterval);
            this.versionPage.Controls.Add(this.label7);
            this.versionPage.Controls.Add(this.label6);
            this.versionPage.Location = new System.Drawing.Point(4, 22);
            this.versionPage.Name = "versionPage";
            this.versionPage.Size = new System.Drawing.Size(307, 150);
            this.versionPage.TabIndex = 4;
            this.versionPage.Text = "Version Checking";
            this.versionPage.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(307, 32);
            this.label6.TabIndex = 0;
            this.label6.Text = "How often should the viewer check for a newer version of TracerX on the CodePlex " +
                "website?";
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
            // txtVersionInterval
            // 
            this.txtVersionInterval.Location = new System.Drawing.Point(35, 34);
            this.txtVersionInterval.MaxLength = 5;
            this.txtVersionInterval.Name = "txtVersionInterval";
            this.txtVersionInterval.Size = new System.Drawing.Size(35, 20);
            this.txtVersionInterval.TabIndex = 2;
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
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(0, 63);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(249, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Note: Enter 0 to check every time the viewer starts.";
            // 
            // OptionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 217);
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
            this.Text = "Options";
            this.tabControl1.ResumeLayout(false);
            this.timePage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.linePage.ResumeLayout(false);
            this.linePage.PerformLayout();
            this.textPage.ResumeLayout(false);
            this.textPage.PerformLayout();
            this.autoRefreshPage.ResumeLayout(false);
            this.autoRefreshPage.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton dontShowDuplicateTimes;
        private System.Windows.Forms.RadioButton showDuplicateTimes;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabPage linePage;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TabPage textPage;
        private System.Windows.Forms.CheckBox expandLinesWithNewlines;
        private System.Windows.Forms.TextBox indentAmount;
        private System.Windows.Forms.TextBox indentChar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage autoRefreshPage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox refreshSeconds;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox reapplyFilter;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtVersionInterval;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.TabPage versionPage;
        public System.Windows.Forms.TabControl tabControl1;
    }
}