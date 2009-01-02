namespace TracerX.Forms {
    partial class ExportCSVForm {
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
            this.fileBox = new System.Windows.Forms.TextBox();
            this.browseBtn = new System.Windows.Forms.Button();
            this.indentChk = new System.Windows.Forms.CheckBox();
            this.headerChk = new System.Windows.Forms.CheckBox();
            this.separatorChk = new System.Windows.Forms.CheckBox();
            this.okBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.omitDupTimeChk = new System.Windows.Forms.CheckBox();
            this.timeAsTextRad = new System.Windows.Forms.RadioButton();
            this.timeWithBlankChk = new System.Windows.Forms.CheckBox();
            this.timeAsIntRad = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Output file:";
            // 
            // fileBox
            // 
            this.fileBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileBox.Location = new System.Drawing.Point(18, 84);
            this.fileBox.Name = "fileBox";
            this.fileBox.Size = new System.Drawing.Size(339, 20);
            this.fileBox.TabIndex = 1;
            // 
            // browseBtn
            // 
            this.browseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.browseBtn.Location = new System.Drawing.Point(363, 84);
            this.browseBtn.Name = "browseBtn";
            this.browseBtn.Size = new System.Drawing.Size(27, 20);
            this.browseBtn.TabIndex = 2;
            this.browseBtn.Text = "...";
            this.browseBtn.UseVisualStyleBackColor = true;
            this.browseBtn.Click += new System.EventHandler(this.browseBtn_Click);
            // 
            // indentChk
            // 
            this.indentChk.AutoSize = true;
            this.indentChk.Checked = true;
            this.indentChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.indentChk.Location = new System.Drawing.Point(18, 134);
            this.indentChk.Name = "indentChk";
            this.indentChk.Size = new System.Drawing.Size(123, 17);
            this.indentChk.TabIndex = 4;
            this.indentChk.Text = "Indent the Text field.";
            this.indentChk.UseVisualStyleBackColor = true;
            // 
            // headerChk
            // 
            this.headerChk.AutoSize = true;
            this.headerChk.Checked = true;
            this.headerChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.headerChk.Location = new System.Drawing.Point(18, 110);
            this.headerChk.Name = "headerChk";
            this.headerChk.Size = new System.Drawing.Size(268, 17);
            this.headerChk.TabIndex = 5;
            this.headerChk.Text = "Include a header record containing the field names.";
            this.headerChk.UseVisualStyleBackColor = true;
            // 
            // separatorChk
            // 
            this.separatorChk.AutoSize = true;
            this.separatorChk.Location = new System.Drawing.Point(18, 157);
            this.separatorChk.Name = "separatorChk";
            this.separatorChk.Size = new System.Drawing.Size(355, 17);
            this.separatorChk.TabIndex = 6;
            this.separatorChk.Text = "Include thousands separators in the Line Number field (e.g. \"12,345\").";
            this.separatorChk.UseVisualStyleBackColor = true;
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okBtn.Location = new System.Drawing.Point(232, 279);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 13;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(313, 279);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 14;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(15, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(372, 42);
            this.label2.TabIndex = 15;
            this.label2.Text = "The currently visible records (based on filtering and expanded/collapsed methods)" +
                " will be exported. The currently visible columns will be exported in the order t" +
                "hey are displayed.";
            // 
            // omitDupTimeChk
            // 
            this.omitDupTimeChk.AutoSize = true;
            this.omitDupTimeChk.Location = new System.Drawing.Point(18, 180);
            this.omitDupTimeChk.Name = "omitDupTimeChk";
            this.omitDupTimeChk.Size = new System.Drawing.Size(156, 17);
            this.omitDupTimeChk.TabIndex = 16;
            this.omitDupTimeChk.Text = "Omit duplicate Time values.";
            this.omitDupTimeChk.UseVisualStyleBackColor = true;
            // 
            // timeAsTextRad
            // 
            this.timeAsTextRad.AutoSize = true;
            this.timeAsTextRad.Location = new System.Drawing.Point(18, 204);
            this.timeAsTextRad.Name = "timeAsTextRad";
            this.timeAsTextRad.Size = new System.Drawing.Size(266, 17);
            this.timeAsTextRad.TabIndex = 17;
            this.timeAsTextRad.TabStop = true;
            this.timeAsTextRad.Text = "Export Time as text (as displayed on the main form).";
            this.timeAsTextRad.UseVisualStyleBackColor = true;
            this.timeAsTextRad.CheckedChanged += new System.EventHandler(this.timeAsIntRad_CheckedChanged);
            // 
            // timeWithBlankChk
            // 
            this.timeWithBlankChk.AutoSize = true;
            this.timeWithBlankChk.Location = new System.Drawing.Point(38, 224);
            this.timeWithBlankChk.Name = "timeWithBlankChk";
            this.timeWithBlankChk.Size = new System.Drawing.Size(215, 17);
            this.timeWithBlankChk.TabIndex = 18;
            this.timeWithBlankChk.Text = "Insert leading blank for viewing in Excel.";
            this.timeWithBlankChk.UseVisualStyleBackColor = true;
            // 
            // timeAsIntRad
            // 
            this.timeAsIntRad.AutoSize = true;
            this.timeAsIntRad.Location = new System.Drawing.Point(18, 248);
            this.timeAsIntRad.Name = "timeAsIntRad";
            this.timeAsIntRad.Size = new System.Drawing.Size(256, 17);
            this.timeAsIntRad.TabIndex = 19;
            this.timeAsIntRad.TabStop = true;
            this.timeAsIntRad.Text = "Export Time as integer (DateTime.Ticks, in UTC).";
            this.timeAsIntRad.UseVisualStyleBackColor = true;
            this.timeAsIntRad.CheckedChanged += new System.EventHandler(this.timeAsIntRad_CheckedChanged);
            // 
            // ExportCSVForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 314);
            this.Controls.Add(this.timeAsIntRad);
            this.Controls.Add(this.timeWithBlankChk);
            this.Controls.Add(this.timeAsTextRad);
            this.Controls.Add(this.omitDupTimeChk);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.separatorChk);
            this.Controls.Add(this.headerChk);
            this.Controls.Add(this.indentChk);
            this.Controls.Add(this.browseBtn);
            this.Controls.Add(this.fileBox);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(9999, 348);
            this.MinimumSize = new System.Drawing.Size(408, 348);
            this.Name = "ExportCSVForm";
            this.Text = "Export Comma-Separated Values";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox fileBox;
        private System.Windows.Forms.Button browseBtn;
        private System.Windows.Forms.CheckBox indentChk;
        private System.Windows.Forms.CheckBox headerChk;
        private System.Windows.Forms.CheckBox separatorChk;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox omitDupTimeChk;
        private System.Windows.Forms.RadioButton timeAsTextRad;
        private System.Windows.Forms.CheckBox timeWithBlankChk;
        private System.Windows.Forms.RadioButton timeAsIntRad;
    }
}