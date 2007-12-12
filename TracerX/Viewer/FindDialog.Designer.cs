namespace BBS.TracerX.Viewer {
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
            this.useWildcards = new System.Windows.Forms.CheckBox();
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
            this.searchUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.searchUp.AutoSize = true;
            this.searchUp.Location = new System.Drawing.Point(12, 55);
            this.searchUp.Name = "searchUp";
            this.searchUp.Size = new System.Drawing.Size(75, 17);
            this.searchUp.TabIndex = 2;
            this.searchUp.Text = "Search up";
            this.searchUp.UseVisualStyleBackColor = true;
            // 
            // matchCase
            // 
            this.matchCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.matchCase.AutoSize = true;
            this.matchCase.Location = new System.Drawing.Point(12, 78);
            this.matchCase.Name = "matchCase";
            this.matchCase.Size = new System.Drawing.Size(82, 17);
            this.matchCase.TabIndex = 3;
            this.matchCase.Text = "Match case";
            this.matchCase.UseVisualStyleBackColor = true;
            // 
            // findNext
            // 
            this.findNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.findNext.Location = new System.Drawing.Point(12, 131);
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
            this.close.Location = new System.Drawing.Point(188, 131);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(75, 23);
            this.close.TabIndex = 5;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            // 
            // bookmarkAll
            // 
            this.bookmarkAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bookmarkAll.Location = new System.Drawing.Point(94, 131);
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
            // useWildcards
            // 
            this.useWildcards.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.useWildcards.AutoSize = true;
            this.useWildcards.Location = new System.Drawing.Point(12, 101);
            this.useWildcards.Name = "useWildcards";
            this.useWildcards.Size = new System.Drawing.Size(92, 17);
            this.useWildcards.TabIndex = 8;
            this.useWildcards.Text = "Use wildcards";
            this.useWildcards.UseVisualStyleBackColor = true;
            // 
            // FindDialog
            // 
            this.AcceptButton = this.findNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.close;
            this.ClientSize = new System.Drawing.Size(276, 166);
            this.Controls.Add(this.useWildcards);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.bookmarkAll);
            this.Controls.Add(this.close);
            this.Controls.Add(this.findNext);
            this.Controls.Add(this.matchCase);
            this.Controls.Add(this.searchUp);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(1000, 200);
            this.MinimumSize = new System.Drawing.Size(284, 200);
            this.Name = "FindDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
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
        private System.Windows.Forms.CheckBox useWildcards;
    }
}