namespace TracerX
{
    partial class FormDataGridViewColumnSelector
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("one");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("two");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("three");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("four");
            this.label1 = new System.Windows.Forms.Label();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.okBtn = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.checkAllBtn = new System.Windows.Forms.Button();
            this.uncheckAllBtn = new System.Windows.Forms.Button();
            this.invertBtn = new System.Windows.Forms.Button();
            this.upBtn = new System.Windows.Forms.Button();
            this.downBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Check the columns to show.";
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(137, 229);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 2;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okBtn.Location = new System.Drawing.Point(137, 200);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 3;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.HideSelection = false;
            listViewItem1.StateImageIndex = 0;
            listViewItem2.StateImageIndex = 0;
            listViewItem3.StateImageIndex = 0;
            listViewItem4.StateImageIndex = 0;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4});
            this.listView1.Location = new System.Drawing.Point(8, 21);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(123, 230);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // checkAllBtn
            // 
            this.checkAllBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkAllBtn.Location = new System.Drawing.Point(137, 21);
            this.checkAllBtn.Name = "checkAllBtn";
            this.checkAllBtn.Size = new System.Drawing.Size(75, 23);
            this.checkAllBtn.TabIndex = 5;
            this.checkAllBtn.Text = "Check All";
            this.checkAllBtn.UseVisualStyleBackColor = true;
            this.checkAllBtn.Click += new System.EventHandler(this.checkAllBtn_Click);
            // 
            // uncheckAllBtn
            // 
            this.uncheckAllBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uncheckAllBtn.Location = new System.Drawing.Point(137, 50);
            this.uncheckAllBtn.Name = "uncheckAllBtn";
            this.uncheckAllBtn.Size = new System.Drawing.Size(75, 23);
            this.uncheckAllBtn.TabIndex = 6;
            this.uncheckAllBtn.Text = "Uncheck All";
            this.uncheckAllBtn.UseVisualStyleBackColor = true;
            this.uncheckAllBtn.Click += new System.EventHandler(this.uncheckAllBtn_Click);
            // 
            // invertBtn
            // 
            this.invertBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.invertBtn.Location = new System.Drawing.Point(137, 79);
            this.invertBtn.Name = "invertBtn";
            this.invertBtn.Size = new System.Drawing.Size(75, 23);
            this.invertBtn.TabIndex = 7;
            this.invertBtn.Text = "Invert";
            this.invertBtn.UseVisualStyleBackColor = true;
            this.invertBtn.Click += new System.EventHandler(this.invertBtn_Click);
            // 
            // upBtn
            // 
            this.upBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.upBtn.Enabled = false;
            this.upBtn.Location = new System.Drawing.Point(137, 124);
            this.upBtn.Name = "upBtn";
            this.upBtn.Size = new System.Drawing.Size(75, 23);
            this.upBtn.TabIndex = 8;
            this.upBtn.Text = "Move Up";
            this.upBtn.UseVisualStyleBackColor = true;
            this.upBtn.Click += new System.EventHandler(this.upBtn_Click);
            // 
            // downBtn
            // 
            this.downBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.downBtn.Enabled = false;
            this.downBtn.Location = new System.Drawing.Point(137, 153);
            this.downBtn.Name = "downBtn";
            this.downBtn.Size = new System.Drawing.Size(75, 23);
            this.downBtn.TabIndex = 9;
            this.downBtn.Text = "Move Down";
            this.downBtn.UseVisualStyleBackColor = true;
            this.downBtn.Click += new System.EventHandler(this.downBtn_Click);
            // 
            // FormDataGridViewColumnSelector
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 259);
            this.Controls.Add(this.downBtn);
            this.Controls.Add(this.upBtn);
            this.Controls.Add(this.invertBtn);
            this.Controls.Add(this.uncheckAllBtn);
            this.Controls.Add(this.checkAllBtn);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(179, 293);
            this.Name = "FormDataGridViewColumnSelector";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "Columns";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button checkAllBtn;
        private System.Windows.Forms.Button uncheckAllBtn;
        private System.Windows.Forms.Button invertBtn;
        private System.Windows.Forms.Button upBtn;
        private System.Windows.Forms.Button downBtn;
    }
}