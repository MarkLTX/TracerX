namespace TracerX
{
    partial class CleanUpFilesDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CleanUpFilesDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFileSpec = new System.Windows.Forms.TextBox();
            this.chkBinary = new System.Windows.Forms.CheckBox();
            this.chkText = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFolderSpec = new System.Windows.Forms.TextBox();
            this.chkDeleteFolders = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpFromTime = new System.Windows.Forms.DateTimePicker();
            this.dtpToTime = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtFromSize = new System.Windows.Forms.TextBox();
            this.txtToSize = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnList = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(5, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(281, 70);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 155);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(222, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "File spec (can use wild-cards, omit extension):";
            // 
            // txtFileSpec
            // 
            this.txtFileSpec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileSpec.Location = new System.Drawing.Point(5, 172);
            this.txtFileSpec.MaxLength = 256;
            this.txtFileSpec.Name = "txtFileSpec";
            this.txtFileSpec.Size = new System.Drawing.Size(281, 20);
            this.txtFileSpec.TabIndex = 2;
            this.txtFileSpec.Text = "*";
            // 
            // chkBinary
            // 
            this.chkBinary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkBinary.AutoSize = true;
            this.chkBinary.Checked = true;
            this.chkBinary.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBinary.Location = new System.Drawing.Point(5, 198);
            this.chkBinary.Name = "chkBinary";
            this.chkBinary.Size = new System.Drawing.Size(102, 17);
            this.chkBinary.TabIndex = 3;
            this.chkBinary.Text = "Binary (.tx1) files";
            this.chkBinary.UseVisualStyleBackColor = true;
            // 
            // chkText
            // 
            this.chkText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkText.AutoSize = true;
            this.chkText.Checked = true;
            this.chkText.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkText.Location = new System.Drawing.Point(121, 198);
            this.chkText.Name = "chkText";
            this.chkText.Size = new System.Drawing.Size(91, 17);
            this.chkText.TabIndex = 4;
            this.chkText.Text = "Text (.txt) files";
            this.chkText.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(162, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Folder spec (can use wild-cards):";
            // 
            // txtFolderSpec
            // 
            this.txtFolderSpec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolderSpec.Location = new System.Drawing.Point(5, 101);
            this.txtFolderSpec.MaxLength = 256;
            this.txtFolderSpec.Name = "txtFolderSpec";
            this.txtFolderSpec.Size = new System.Drawing.Size(281, 20);
            this.txtFolderSpec.TabIndex = 6;
            this.txtFolderSpec.Text = "*";
            // 
            // chkDeleteFolders
            // 
            this.chkDeleteFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkDeleteFolders.AutoSize = true;
            this.chkDeleteFolders.Checked = true;
            this.chkDeleteFolders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDeleteFolders.Location = new System.Drawing.Point(5, 127);
            this.chkDeleteFolders.Name = "chkDeleteFolders";
            this.chkDeleteFolders.Size = new System.Drawing.Size(122, 17);
            this.chkDeleteFolders.TabIndex = 7;
            this.chkDeleteFolders.Text = "Delete empty folders";
            this.chkDeleteFolders.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 226);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "From time:";
            // 
            // dtpFromTime
            // 
            this.dtpFromTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dtpFromTime.CustomFormat = "yyyy/MM/dd  HH:mm";
            this.dtpFromTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromTime.Location = new System.Drawing.Point(5, 243);
            this.dtpFromTime.Name = "dtpFromTime";
            this.dtpFromTime.Size = new System.Drawing.Size(134, 20);
            this.dtpFromTime.TabIndex = 10;
            this.dtpFromTime.Value = new System.DateTime(1999, 12, 31, 23, 59, 0, 0);
            // 
            // dtpToTime
            // 
            this.dtpToTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dtpToTime.CustomFormat = "yyyy/MM/dd  HH:mm";
            this.dtpToTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToTime.Location = new System.Drawing.Point(152, 243);
            this.dtpToTime.Name = "dtpToTime";
            this.dtpToTime.Size = new System.Drawing.Size(134, 20);
            this.dtpToTime.TabIndex = 11;
            this.dtpToTime.Value = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(152, 226);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "To time:";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 276);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "From size (KB):";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(152, 276);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "To size (KB):";
            // 
            // txtFromSize
            // 
            this.txtFromSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtFromSize.Location = new System.Drawing.Point(5, 292);
            this.txtFromSize.MaxLength = 12;
            this.txtFromSize.Name = "txtFromSize";
            this.txtFromSize.Size = new System.Drawing.Size(134, 20);
            this.txtFromSize.TabIndex = 15;
            this.txtFromSize.Text = "0";
            // 
            // txtToSize
            // 
            this.txtToSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtToSize.Location = new System.Drawing.Point(152, 292);
            this.txtToSize.MaxLength = 12;
            this.txtToSize.Name = "txtToSize";
            this.txtToSize.Size = new System.Drawing.Size(134, 20);
            this.txtToSize.TabIndex = 16;
            this.txtToSize.Text = "999999999";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.Location = new System.Drawing.Point(208, 329);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Location = new System.Drawing.Point(108, 329);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 18;
            this.btnDelete.Text = "Delete Files";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnList
            // 
            this.btnList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnList.Location = new System.Drawing.Point(8, 329);
            this.btnList.Name = "btnList";
            this.btnList.Size = new System.Drawing.Size(75, 23);
            this.btnList.TabIndex = 19;
            this.btnList.Text = "List Files";
            this.btnList.UseVisualStyleBackColor = true;
            this.btnList.Click += new System.EventHandler(this.btnList_Click);
            // 
            // CleanUpFilesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(291, 368);
            this.Controls.Add(this.btnList);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtToSize);
            this.Controls.Add(this.txtFromSize);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dtpToTime);
            this.Controls.Add(this.dtpFromTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkDeleteFolders);
            this.Controls.Add(this.txtFolderSpec);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkText);
            this.Controls.Add(this.chkBinary);
            this.Controls.Add(this.txtFileSpec);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 407);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(307, 407);
            this.Name = "CleanUpFilesDialog";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Clean Up Log Files";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFileSpec;
        private System.Windows.Forms.CheckBox chkBinary;
        private System.Windows.Forms.CheckBox chkText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFolderSpec;
        private System.Windows.Forms.CheckBox chkDeleteFolders;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpFromTime;
        private System.Windows.Forms.DateTimePicker dtpToTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtFromSize;
        private System.Windows.Forms.TextBox txtToSize;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnList;
    }
}