namespace TracerX.Viewer {
    partial class FindDialog {
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
            this.searchUp = new System.Windows.Forms.CheckBox();
            this.matchCase = new System.Windows.Forms.CheckBox();
            this.findNext = new System.Windows.Forms.Button();
            this.close = new System.Windows.Forms.Button();
            this.bookmarkAll = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.wildHelpLabel = new System.Windows.Forms.Label();
            this.normalRad = new System.Windows.Forms.RadioButton();
            this.regexRad = new System.Windows.Forms.RadioButton();
            this.wildcardRad = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Find text:";
            // 
            // searchUp
            // 
            this.searchUp.AutoSize = true;
            this.searchUp.Location = new System.Drawing.Point(12, 79);
            this.searchUp.Name = "searchUp";
            this.searchUp.Size = new System.Drawing.Size(75, 17);
            this.searchUp.TabIndex = 2;
            this.searchUp.Text = "Search up";
            this.searchUp.UseVisualStyleBackColor = true;
            // 
            // matchCase
            // 
            this.matchCase.AutoSize = true;
            this.matchCase.Location = new System.Drawing.Point(12, 57);
            this.matchCase.Name = "matchCase";
            this.matchCase.Size = new System.Drawing.Size(82, 17);
            this.matchCase.TabIndex = 3;
            this.matchCase.Text = "Match case";
            this.matchCase.UseVisualStyleBackColor = true;
            // 
            // findNext
            // 
            this.findNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.findNext.Location = new System.Drawing.Point(12, 163);
            this.findNext.Name = "findNext";
            this.findNext.Size = new System.Drawing.Size(75, 23);
            this.findNext.TabIndex = 4;
            this.findNext.Text = "Find Next";
            this.findNext.UseVisualStyleBackColor = true;
            this.findNext.Click += new System.EventHandler(this.findNext_Click);
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.close.Location = new System.Drawing.Point(188, 163);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(75, 23);
            this.close.TabIndex = 5;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // bookmarkAll
            // 
            this.bookmarkAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bookmarkAll.Location = new System.Drawing.Point(94, 163);
            this.bookmarkAll.Name = "bookmarkAll";
            this.bookmarkAll.Size = new System.Drawing.Size(87, 23);
            this.bookmarkAll.TabIndex = 6;
            this.bookmarkAll.Text = "Bookmark All";
            this.bookmarkAll.UseVisualStyleBackColor = true;
            this.bookmarkAll.Click += new System.EventHandler(this.bookmarkAll_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 25);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(250, 21);
            this.comboBox1.TabIndex = 7;
            // 
            // wildHelpLabel
            // 
            this.wildHelpLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.wildHelpLabel.Location = new System.Drawing.Point(13, 126);
            this.wildHelpLabel.Name = "wildHelpLabel";
            this.wildHelpLabel.Size = new System.Drawing.Size(251, 32);
            this.wildHelpLabel.TabIndex = 10;
            this.wildHelpLabel.Text = "Backslash (\\) is the wildcard escape character. Use \\* to find *, \\? to find ?, a" +
                "nd \\\\ to find \\.";
            // 
            // normalRad
            // 
            this.normalRad.AutoSize = true;
            this.normalRad.Checked = true;
            this.normalRad.Location = new System.Drawing.Point(145, 57);
            this.normalRad.Name = "normalRad";
            this.normalRad.Size = new System.Drawing.Size(58, 17);
            this.normalRad.TabIndex = 11;
            this.normalRad.TabStop = true;
            this.normalRad.Text = "Normal";
            this.normalRad.UseVisualStyleBackColor = true;
            // 
            // regexRad
            // 
            this.regexRad.AutoSize = true;
            this.regexRad.Location = new System.Drawing.Point(145, 102);
            this.regexRad.Name = "regexRad";
            this.regexRad.Size = new System.Drawing.Size(115, 17);
            this.regexRad.TabIndex = 12;
            this.regexRad.TabStop = true;
            this.regexRad.Text = "Regular expression";
            this.regexRad.UseVisualStyleBackColor = true;
            // 
            // wildcardRad
            // 
            this.wildcardRad.AutoSize = true;
            this.wildcardRad.Location = new System.Drawing.Point(145, 79);
            this.wildcardRad.Name = "wildcardRad";
            this.wildcardRad.Size = new System.Drawing.Size(67, 17);
            this.wildcardRad.TabIndex = 13;
            this.wildcardRad.TabStop = true;
            this.wildcardRad.Text = "Wildcard";
            this.wildcardRad.UseVisualStyleBackColor = true;
            this.wildcardRad.CheckedChanged += new System.EventHandler(this.wildcardRad_CheckedChanged);
            // 
            // FindDialog
            // 
            this.AcceptButton = this.findNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.close;
            this.ClientSize = new System.Drawing.Size(276, 198);
            this.Controls.Add(this.wildcardRad);
            this.Controls.Add(this.regexRad);
            this.Controls.Add(this.normalRad);
            this.Controls.Add(this.wildHelpLabel);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.bookmarkAll);
            this.Controls.Add(this.close);
            this.Controls.Add(this.findNext);
            this.Controls.Add(this.matchCase);
            this.Controls.Add(this.searchUp);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(1000, 2000);
            this.MinimumSize = new System.Drawing.Size(284, 200);
            this.Name = "FindDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find Text";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox searchUp;
        private System.Windows.Forms.CheckBox matchCase;
        private System.Windows.Forms.Button findNext;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.Button bookmarkAll;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label wildHelpLabel;
        private System.Windows.Forms.RadioButton normalRad;
        private System.Windows.Forms.RadioButton regexRad;
        private System.Windows.Forms.RadioButton wildcardRad;
    }
}