namespace TracerX.Forms {
    partial class ColorRulesDialog {
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
            this.listBox = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.newBtn = new System.Windows.Forms.Button();
            this.removeBtn = new System.Windows.Forms.Button();
            this.upBtn = new System.Windows.Forms.Button();
            this.downBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.backPanel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.textPanel = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.backColorBtn = new System.Windows.Forms.Button();
            this.textColorBtn = new System.Windows.Forms.Button();
            this.allChk = new System.Windows.Forms.CheckBox();
            this.levelChk = new System.Windows.Forms.CheckBox();
            this.methodChk = new System.Windows.Forms.CheckBox();
            this.loggerChk = new System.Windows.Forms.CheckBox();
            this.threadNameChk = new System.Windows.Forms.CheckBox();
            this.textChk = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.chkCase = new System.Windows.Forms.CheckBox();
            this.chkWild = new System.Windows.Forms.CheckBox();
            this.txtDoesNotContain = new System.Windows.Forms.TextBox();
            this.chkDoesNotContain = new System.Windows.Forms.CheckBox();
            this.txtContains = new System.Windows.Forms.TextBox();
            this.chkContain = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.exampleBox = new System.Windows.Forms.TextBox();
            this.okBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.chkRegex = new System.Windows.Forms.CheckBox();
            this.applyBtn = new System.Windows.Forms.Button();
            this.exportBtn = new System.Windows.Forms.Button();
            this.importBtn = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(12, 73);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(146, 109);
            this.listBox.TabIndex = 2;
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(403, 31);
            this.label2.TabIndex = 3;
            this.label2.Text = "Each line in the log is highlighted according to the first enabled rule it matche" +
                "s.  Use the checkboxes to enable or disable rules.";
            // 
            // newBtn
            // 
            this.newBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newBtn.Location = new System.Drawing.Point(165, 73);
            this.newBtn.Name = "newBtn";
            this.newBtn.Size = new System.Drawing.Size(75, 23);
            this.newBtn.TabIndex = 4;
            this.newBtn.Text = "New/Copy";
            this.newBtn.UseVisualStyleBackColor = true;
            this.newBtn.Click += new System.EventHandler(this.newBtn_Click);
            // 
            // removeBtn
            // 
            this.removeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeBtn.Location = new System.Drawing.Point(165, 103);
            this.removeBtn.Name = "removeBtn";
            this.removeBtn.Size = new System.Drawing.Size(75, 23);
            this.removeBtn.TabIndex = 5;
            this.removeBtn.Text = "Remove";
            this.removeBtn.UseVisualStyleBackColor = true;
            this.removeBtn.Click += new System.EventHandler(this.removeBtn_Click);
            // 
            // upBtn
            // 
            this.upBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.upBtn.Location = new System.Drawing.Point(165, 133);
            this.upBtn.Name = "upBtn";
            this.upBtn.Size = new System.Drawing.Size(75, 23);
            this.upBtn.TabIndex = 6;
            this.upBtn.Text = "Up";
            this.upBtn.UseVisualStyleBackColor = true;
            this.upBtn.Click += new System.EventHandler(this.upBtn_Click);
            // 
            // downBtn
            // 
            this.downBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.downBtn.Location = new System.Drawing.Point(165, 162);
            this.downBtn.Name = "downBtn";
            this.downBtn.Size = new System.Drawing.Size(75, 23);
            this.downBtn.TabIndex = 7;
            this.downBtn.Text = "Down";
            this.downBtn.UseVisualStyleBackColor = true;
            this.downBtn.Click += new System.EventHandler(this.downBtn_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(270, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Background:";
            // 
            // backPanel
            // 
            this.backPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.backPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.backPanel.Location = new System.Drawing.Point(344, 108);
            this.backPanel.Name = "backPanel";
            this.backPanel.Size = new System.Drawing.Size(38, 19);
            this.backPanel.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(270, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Text color:";
            // 
            // textPanel
            // 
            this.textPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textPanel.BackColor = System.Drawing.Color.Black;
            this.textPanel.Location = new System.Drawing.Point(344, 78);
            this.textPanel.Name = "textPanel";
            this.textPanel.Size = new System.Drawing.Size(38, 19);
            this.textPanel.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(270, 140);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Example:";
            // 
            // backColorBtn
            // 
            this.backColorBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.backColorBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.backColorBtn.Location = new System.Drawing.Point(388, 108);
            this.backColorBtn.Name = "backColorBtn";
            this.backColorBtn.Size = new System.Drawing.Size(27, 19);
            this.backColorBtn.TabIndex = 13;
            this.backColorBtn.Text = "...";
            this.backColorBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.backColorBtn.UseVisualStyleBackColor = true;
            this.backColorBtn.Click += new System.EventHandler(this.backColorBtn_Click);
            // 
            // textColorBtn
            // 
            this.textColorBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textColorBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textColorBtn.Location = new System.Drawing.Point(388, 78);
            this.textColorBtn.Name = "textColorBtn";
            this.textColorBtn.Size = new System.Drawing.Size(27, 19);
            this.textColorBtn.TabIndex = 14;
            this.textColorBtn.Text = "...";
            this.textColorBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.textColorBtn.UseVisualStyleBackColor = true;
            this.textColorBtn.Click += new System.EventHandler(this.textColorBtn_Click);
            // 
            // allChk
            // 
            this.allChk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.allChk.AutoSize = true;
            this.allChk.Location = new System.Drawing.Point(316, 270);
            this.allChk.Name = "allChk";
            this.allChk.Size = new System.Drawing.Size(78, 17);
            this.allChk.TabIndex = 15;
            this.allChk.Text = "All of these";
            this.allChk.UseVisualStyleBackColor = true;
            this.allChk.CheckedChanged += new System.EventHandler(this.allChk_CheckedChanged);
            // 
            // levelChk
            // 
            this.levelChk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.levelChk.AutoSize = true;
            this.levelChk.Location = new System.Drawing.Point(335, 360);
            this.levelChk.Name = "levelChk";
            this.levelChk.Size = new System.Drawing.Size(52, 17);
            this.levelChk.TabIndex = 17;
            this.levelChk.Text = "Level";
            this.levelChk.UseVisualStyleBackColor = true;
            // 
            // methodChk
            // 
            this.methodChk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.methodChk.AutoSize = true;
            this.methodChk.Location = new System.Drawing.Point(335, 342);
            this.methodChk.Name = "methodChk";
            this.methodChk.Size = new System.Drawing.Size(62, 17);
            this.methodChk.TabIndex = 18;
            this.methodChk.Text = "Method";
            this.methodChk.UseVisualStyleBackColor = true;
            // 
            // loggerChk
            // 
            this.loggerChk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loggerChk.AutoSize = true;
            this.loggerChk.Location = new System.Drawing.Point(335, 324);
            this.loggerChk.Name = "loggerChk";
            this.loggerChk.Size = new System.Drawing.Size(59, 17);
            this.loggerChk.TabIndex = 19;
            this.loggerChk.Text = "Logger";
            this.loggerChk.UseVisualStyleBackColor = true;
            // 
            // threadNameChk
            // 
            this.threadNameChk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.threadNameChk.AutoSize = true;
            this.threadNameChk.Location = new System.Drawing.Point(335, 306);
            this.threadNameChk.Name = "threadNameChk";
            this.threadNameChk.Size = new System.Drawing.Size(89, 17);
            this.threadNameChk.TabIndex = 20;
            this.threadNameChk.Text = "Thread name";
            this.threadNameChk.UseVisualStyleBackColor = true;
            // 
            // textChk
            // 
            this.textChk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textChk.AutoSize = true;
            this.textChk.Checked = true;
            this.textChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.textChk.Location = new System.Drawing.Point(335, 288);
            this.textChk.Name = "textChk";
            this.textChk.Size = new System.Drawing.Size(47, 17);
            this.textChk.TabIndex = 21;
            this.textChk.Text = "Text";
            this.textChk.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.Location = new System.Drawing.Point(312, 238);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 27);
            this.label6.TabIndex = 22;
            this.label6.Text = "Select one or more fields to search.";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label7.Location = new System.Drawing.Point(259, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(1, 155);
            this.label7.TabIndex = 23;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 57);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(208, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Select or create a rule to edit its properties.";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(267, 58);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(174, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "Select the colors for matching lines.";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Location = new System.Drawing.Point(10, 228);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(426, 1);
            this.label10.TabIndex = 26;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Location = new System.Drawing.Point(303, 243);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(1, 145);
            this.label11.TabIndex = 27;
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label12.Location = new System.Drawing.Point(11, 235);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(220, 30);
            this.label12.TabIndex = 34;
            this.label12.Text = "If both criteria are enabled, both must match for the rule to apply.";
            // 
            // chkCase
            // 
            this.chkCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkCase.AutoSize = true;
            this.chkCase.Location = new System.Drawing.Point(12, 371);
            this.chkCase.Name = "chkCase";
            this.chkCase.Size = new System.Drawing.Size(82, 17);
            this.chkCase.TabIndex = 33;
            this.chkCase.Text = "Match case";
            this.chkCase.UseVisualStyleBackColor = true;
            // 
            // chkWild
            // 
            this.chkWild.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkWild.AutoSize = true;
            this.chkWild.Location = new System.Drawing.Point(102, 371);
            this.chkWild.Name = "chkWild";
            this.chkWild.Size = new System.Drawing.Size(73, 17);
            this.chkWild.TabIndex = 32;
            this.chkWild.Text = "Wildcards";
            this.chkWild.UseVisualStyleBackColor = true;
            this.chkWild.CheckedChanged += new System.EventHandler(this.chkWild_CheckedChanged);
            // 
            // txtDoesNotContain
            // 
            this.txtDoesNotContain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDoesNotContain.Enabled = false;
            this.txtDoesNotContain.Location = new System.Drawing.Point(31, 339);
            this.txtDoesNotContain.Name = "txtDoesNotContain";
            this.txtDoesNotContain.Size = new System.Drawing.Size(263, 20);
            this.txtDoesNotContain.TabIndex = 31;
            // 
            // chkDoesNotContain
            // 
            this.chkDoesNotContain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkDoesNotContain.AutoSize = true;
            this.chkDoesNotContain.Location = new System.Drawing.Point(12, 321);
            this.chkDoesNotContain.Name = "chkDoesNotContain";
            this.chkDoesNotContain.Size = new System.Drawing.Size(175, 17);
            this.chkDoesNotContain.TabIndex = 30;
            this.chkDoesNotContain.Text = "Match lines that do not contain:";
            this.chkDoesNotContain.UseVisualStyleBackColor = true;
            this.chkDoesNotContain.CheckedChanged += new System.EventHandler(this.chkDoesNotContain_CheckedChanged);
            // 
            // txtContains
            // 
            this.txtContains.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContains.Enabled = false;
            this.txtContains.Location = new System.Drawing.Point(31, 290);
            this.txtContains.Name = "txtContains";
            this.txtContains.Size = new System.Drawing.Size(263, 20);
            this.txtContains.TabIndex = 29;
            this.txtContains.Text = "Enter text here";
            // 
            // chkContain
            // 
            this.chkContain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkContain.AutoSize = true;
            this.chkContain.Checked = true;
            this.chkContain.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkContain.Location = new System.Drawing.Point(12, 272);
            this.chkContain.Name = "chkContain";
            this.chkContain.Size = new System.Drawing.Size(142, 17);
            this.chkContain.TabIndex = 28;
            this.chkContain.Text = "Match lines that contain:";
            this.chkContain.UseVisualStyleBackColor = true;
            this.chkContain.CheckedChanged += new System.EventHandler(this.chkContain_CheckedChanged);
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(9, 196);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(61, 13);
            this.label13.TabIndex = 35;
            this.label13.Text = "Rule name:";
            // 
            // nameBox
            // 
            this.nameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.nameBox.Location = new System.Drawing.Point(76, 193);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(164, 20);
            this.nameBox.TabIndex = 36;
            this.nameBox.Text = "Rule 1";
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label14.Location = new System.Drawing.Point(10, 395);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(426, 1);
            this.label14.TabIndex = 37;
            // 
            // exampleBox
            // 
            this.exampleBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.exampleBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.exampleBox.ForeColor = System.Drawing.Color.Black;
            this.exampleBox.Location = new System.Drawing.Point(344, 137);
            this.exampleBox.Name = "exampleBox";
            this.exampleBox.ReadOnly = true;
            this.exampleBox.Size = new System.Drawing.Size(71, 20);
            this.exampleBox.TabIndex = 38;
            this.exampleBox.Text = "abc 123 (-+";
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okBtn.Location = new System.Drawing.Point(198, 403);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 39;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(360, 403);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 40;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            // 
            // chkRegex
            // 
            this.chkRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkRegex.AutoSize = true;
            this.chkRegex.Location = new System.Drawing.Point(181, 371);
            this.chkRegex.Name = "chkRegex";
            this.chkRegex.Size = new System.Drawing.Size(121, 17);
            this.chkRegex.TabIndex = 41;
            this.chkRegex.Text = "Regular expressions";
            this.chkRegex.UseVisualStyleBackColor = true;
            this.chkRegex.CheckedChanged += new System.EventHandler(this.chkRegex_CheckedChanged);
            // 
            // applyBtn
            // 
            this.applyBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.applyBtn.Location = new System.Drawing.Point(279, 403);
            this.applyBtn.Name = "applyBtn";
            this.applyBtn.Size = new System.Drawing.Size(75, 23);
            this.applyBtn.TabIndex = 42;
            this.applyBtn.Text = "Apply";
            this.applyBtn.UseVisualStyleBackColor = true;
            this.applyBtn.Click += new System.EventHandler(this.applyBtn_Click);
            // 
            // exportBtn
            // 
            this.exportBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.exportBtn.Location = new System.Drawing.Point(10, 403);
            this.exportBtn.Name = "exportBtn";
            this.exportBtn.Size = new System.Drawing.Size(75, 23);
            this.exportBtn.TabIndex = 43;
            this.exportBtn.Text = "Export...";
            this.exportBtn.UseVisualStyleBackColor = true;
            this.exportBtn.Click += new System.EventHandler(this.exportBtn_Click);
            // 
            // importBtn
            // 
            this.importBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.importBtn.Location = new System.Drawing.Point(91, 403);
            this.importBtn.Name = "importBtn";
            this.importBtn.Size = new System.Drawing.Size(75, 23);
            this.importBtn.TabIndex = 44;
            this.importBtn.Text = "Import...";
            this.importBtn.UseVisualStyleBackColor = true;
            this.importBtn.Click += new System.EventHandler(this.importBtn_Click);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(180, 404);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(1, 20);
            this.label5.TabIndex = 45;
            // 
            // ColorRulesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 438);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.importBtn);
            this.Controls.Add(this.exportBtn);
            this.Controls.Add(this.applyBtn);
            this.Controls.Add(this.chkRegex);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.exampleBox);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.chkCase);
            this.Controls.Add(this.chkWild);
            this.Controls.Add(this.txtDoesNotContain);
            this.Controls.Add(this.chkDoesNotContain);
            this.Controls.Add(this.txtContains);
            this.Controls.Add(this.chkContain);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textChk);
            this.Controls.Add(this.threadNameChk);
            this.Controls.Add(this.loggerChk);
            this.Controls.Add(this.methodChk);
            this.Controls.Add(this.levelChk);
            this.Controls.Add(this.allChk);
            this.Controls.Add(this.textColorBtn);
            this.Controls.Add(this.backColorBtn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textPanel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.backPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.downBtn);
            this.Controls.Add(this.upBtn);
            this.Controls.Add(this.removeBtn);
            this.Controls.Add(this.newBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listBox);
            this.MinimumSize = new System.Drawing.Size(455, 472);
            this.Name = "ColorRulesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Coloring Rules";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox listBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button newBtn;
        private System.Windows.Forms.Button removeBtn;
        private System.Windows.Forms.Button upBtn;
        private System.Windows.Forms.Button downBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel backPanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel textPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button backColorBtn;
        private System.Windows.Forms.Button textColorBtn;
        private System.Windows.Forms.CheckBox allChk;
        private System.Windows.Forms.CheckBox levelChk;
        private System.Windows.Forms.CheckBox methodChk;
        private System.Windows.Forms.CheckBox loggerChk;
        private System.Windows.Forms.CheckBox threadNameChk;
        private System.Windows.Forms.CheckBox textChk;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox chkCase;
        private System.Windows.Forms.CheckBox chkWild;
        private System.Windows.Forms.TextBox txtDoesNotContain;
        private System.Windows.Forms.CheckBox chkDoesNotContain;
        private System.Windows.Forms.TextBox txtContains;
        private System.Windows.Forms.CheckBox chkContain;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox exampleBox;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.CheckBox chkRegex;
        private System.Windows.Forms.Button applyBtn;
        private System.Windows.Forms.Button exportBtn;
        private System.Windows.Forms.Button importBtn;
        private System.Windows.Forms.Label label5;
    }
}