namespace TracerX.Viewer {
    partial class FileProperties {
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
            this.close = new System.Windows.Forms.Button();
            this.sessionListView = new System.Windows.Forms.ListView();
            this.sessionNameCol = new System.Windows.Forms.ColumnHeader();
            this.sessionValueCol = new System.Windows.Forms.ColumnHeader();
            this.commonListView = new System.Windows.Forms.ListView();
            this.commonNameCol = new System.Windows.Forms.ColumnHeader();
            this.commonValueCol = new System.Windows.Forms.ColumnHeader();
            this.sessionCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.close.Location = new System.Drawing.Point(320, 399);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(75, 23);
            this.close.TabIndex = 5;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            // 
            // sessionListView
            // 
            this.sessionListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sessionListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sessionNameCol,
            this.sessionValueCol});
            this.sessionListView.GridLines = true;
            this.sessionListView.Location = new System.Drawing.Point(11, 179);
            this.sessionListView.Name = "sessionListView";
            this.sessionListView.Size = new System.Drawing.Size(381, 213);
            this.sessionListView.TabIndex = 6;
            this.sessionListView.UseCompatibleStateImageBehavior = false;
            this.sessionListView.View = System.Windows.Forms.View.Details;
            // 
            // sessionNameCol
            // 
            this.sessionNameCol.Text = "Property";
            // 
            // sessionValueCol
            // 
            this.sessionValueCol.Text = "Value";
            this.sessionValueCol.Width = 225;
            // 
            // commonListView
            // 
            this.commonListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.commonListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.commonNameCol,
            this.commonValueCol});
            this.commonListView.GridLines = true;
            this.commonListView.Location = new System.Drawing.Point(11, 22);
            this.commonListView.Name = "commonListView";
            this.commonListView.Size = new System.Drawing.Size(381, 83);
            this.commonListView.TabIndex = 7;
            this.commonListView.UseCompatibleStateImageBehavior = false;
            this.commonListView.View = System.Windows.Forms.View.Details;
            // 
            // commonNameCol
            // 
            this.commonNameCol.Text = "Property";
            // 
            // commonValueCol
            // 
            this.commonValueCol.Text = "Value";
            this.commonValueCol.Width = 225;
            // 
            // sessionCombo
            // 
            this.sessionCombo.FormattingEnabled = true;
            this.sessionCombo.Location = new System.Drawing.Point(61, 138);
            this.sessionCombo.Name = "sessionCombo";
            this.sessionCombo.Size = new System.Drawing.Size(64, 21);
            this.sessionCombo.TabIndex = 8;
            this.sessionCombo.SelectedIndexChanged += new System.EventHandler(this.sessionCombo_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 141);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Session:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 162);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Session properties:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "File properties:";
            // 
            // FileProperties
            // 
            this.AcceptButton = this.close;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.close;
            this.ClientSize = new System.Drawing.Size(400, 428);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sessionCombo);
            this.Controls.Add(this.commonListView);
            this.Controls.Add(this.sessionListView);
            this.Controls.Add(this.close);
            this.MinimumSize = new System.Drawing.Size(408, 462);
            this.Name = "FileProperties";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "File Properties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button close;
        private System.Windows.Forms.ListView sessionListView;
        private System.Windows.Forms.ColumnHeader sessionNameCol;
        private System.Windows.Forms.ColumnHeader sessionValueCol;
        private System.Windows.Forms.ListView commonListView;
        private System.Windows.Forms.ColumnHeader commonNameCol;
        private System.Windows.Forms.ColumnHeader commonValueCol;
        private System.Windows.Forms.ComboBox sessionCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;

    }
}