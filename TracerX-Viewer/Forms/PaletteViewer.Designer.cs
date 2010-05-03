namespace TracerX.Forms {
    partial class PaletteViewer {
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
            this.baseColorBtn = new System.Windows.Forms.Button();
            this.colorPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.sizeFactorBox = new System.Windows.Forms.NumericUpDown();
            this.goBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.minContrastBox = new System.Windows.Forms.TextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.rgbCol = new System.Windows.Forms.ColumnHeader();
            this.exampleCol = new System.Windows.Forms.ColumnHeader();
            this.contrastCol = new System.Windows.Forms.ColumnHeader();
            this.okBtn = new System.Windows.Forms.Button();
            this.sort1Btn = new System.Windows.Forms.Button();
            this.sort2Btn = new System.Windows.Forms.Button();
            this.sort3Btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.sizeFactorBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "BaseColor:";
            // 
            // baseColorBtn
            // 
            this.baseColorBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.baseColorBtn.Location = new System.Drawing.Point(169, 10);
            this.baseColorBtn.Name = "baseColorBtn";
            this.baseColorBtn.Size = new System.Drawing.Size(27, 19);
            this.baseColorBtn.TabIndex = 16;
            this.baseColorBtn.Text = "...";
            this.baseColorBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.baseColorBtn.UseVisualStyleBackColor = true;
            this.baseColorBtn.Click += new System.EventHandler(this.baseColorBtn_Click);
            // 
            // colorPanel
            // 
            this.colorPanel.BackColor = System.Drawing.Color.Black;
            this.colorPanel.Location = new System.Drawing.Point(125, 10);
            this.colorPanel.Name = "colorPanel";
            this.colorPanel.Size = new System.Drawing.Size(38, 19);
            this.colorPanel.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Spectrum size factor:";
            // 
            // sizeFactorBox
            // 
            this.sizeFactorBox.Location = new System.Drawing.Point(125, 36);
            this.sizeFactorBox.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.sizeFactorBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.sizeFactorBox.Name = "sizeFactorBox";
            this.sizeFactorBox.Size = new System.Drawing.Size(38, 20);
            this.sizeFactorBox.TabIndex = 18;
            this.sizeFactorBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // goBtn
            // 
            this.goBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goBtn.Location = new System.Drawing.Point(16, 88);
            this.goBtn.Name = "goBtn";
            this.goBtn.Size = new System.Drawing.Size(164, 24);
            this.goBtn.TabIndex = 19;
            this.goBtn.Text = "Generate Contrasting Colors";
            this.goBtn.UseVisualStyleBackColor = true;
            this.goBtn.Click += new System.EventHandler(this.goBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Minimum contrast:";
            // 
            // minContrastBox
            // 
            this.minContrastBox.Location = new System.Drawing.Point(125, 62);
            this.minContrastBox.Name = "minContrastBox";
            this.minContrastBox.Size = new System.Drawing.Size(38, 20);
            this.minContrastBox.TabIndex = 21;
            this.minContrastBox.Text = "5.0";
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.rgbCol,
            this.exampleCol,
            this.contrastCol});
            this.listView1.Location = new System.Drawing.Point(16, 119);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(264, 272);
            this.listView1.TabIndex = 22;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // rgbCol
            // 
            this.rgbCol.Text = "RGB";
            this.rgbCol.Width = 78;
            // 
            // exampleCol
            // 
            this.exampleCol.Text = "Example";
            this.exampleCol.Width = 82;
            // 
            // contrastCol
            // 
            this.contrastCol.Text = "Contrast";
            this.contrastCol.Width = 72;
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.Location = new System.Drawing.Point(205, 397);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 23;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // sort1Btn
            // 
            this.sort1Btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sort1Btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sort1Btn.Location = new System.Drawing.Point(16, 396);
            this.sort1Btn.Name = "sort1Btn";
            this.sort1Btn.Size = new System.Drawing.Size(55, 24);
            this.sort1Btn.TabIndex = 24;
            this.sort1Btn.Text = "Sort 1";
            this.sort1Btn.UseVisualStyleBackColor = true;
            this.sort1Btn.Click += new System.EventHandler(this.sort1Btn_Click);
            // 
            // sort2Btn
            // 
            this.sort2Btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sort2Btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sort2Btn.Location = new System.Drawing.Point(77, 396);
            this.sort2Btn.Name = "sort2Btn";
            this.sort2Btn.Size = new System.Drawing.Size(55, 24);
            this.sort2Btn.TabIndex = 25;
            this.sort2Btn.Text = "Sort 2";
            this.sort2Btn.UseVisualStyleBackColor = true;
            this.sort2Btn.Click += new System.EventHandler(this.sort2Btn_Click);
            // 
            // sort3Btn
            // 
            this.sort3Btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sort3Btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sort3Btn.Location = new System.Drawing.Point(138, 396);
            this.sort3Btn.Name = "sort3Btn";
            this.sort3Btn.Size = new System.Drawing.Size(55, 24);
            this.sort3Btn.TabIndex = 26;
            this.sort3Btn.Text = "Sort 3";
            this.sort3Btn.UseVisualStyleBackColor = true;
            this.sort3Btn.Click += new System.EventHandler(this.sort3Btn_Click);
            // 
            // PaletteViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 432);
            this.Controls.Add(this.sort3Btn);
            this.Controls.Add(this.sort2Btn);
            this.Controls.Add(this.sort1Btn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.minContrastBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.goBtn);
            this.Controls.Add(this.sizeFactorBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.baseColorBtn);
            this.Controls.Add(this.colorPanel);
            this.Controls.Add(this.label1);
            this.Name = "PaletteViewer";
            this.Text = "PaletteViewer";
            ((System.ComponentModel.ISupportInitialize)(this.sizeFactorBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button baseColorBtn;
        private System.Windows.Forms.Panel colorPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown sizeFactorBox;
        private System.Windows.Forms.Button goBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox minContrastBox;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader rgbCol;
        private System.Windows.Forms.ColumnHeader exampleCol;
        private System.Windows.Forms.ColumnHeader contrastCol;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button sort1Btn;
        private System.Windows.Forms.Button sort2Btn;
        private System.Windows.Forms.Button sort3Btn;
    }
}