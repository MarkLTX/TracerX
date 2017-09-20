namespace TracerX
{
    partial class PathSelector
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pathGrid1 = new TracerX.PathControl();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(5, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(598, 50);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server: {0}\r\nFolder: {1}";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(519, 413);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pathGrid1
            // 
            this.pathGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathGrid1.ArchiveVisibility = TracerX.ArchiveVisibility.ViewedArchives;
            this.pathGrid1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pathGrid1.BackColor = System.Drawing.Color.Transparent;
            this.pathGrid1.ColumnHeaderBackColor = System.Drawing.Color.LemonChiffon;
            this.pathGrid1.Location = new System.Drawing.Point(7, 58);
            this.pathGrid1.MinimumSize = new System.Drawing.Size(124, 0);
            this.pathGrid1.Name = "pathGrid1";
            this.pathGrid1.PathsAreFolders = false;
            this.pathGrid1.PathsAreLocal = false;
            this.pathGrid1.RemoteServer = null;
            this.pathGrid1.ShowTimesAgo = false;
            this.pathGrid1.Size = new System.Drawing.Size(595, 346);
            this.pathGrid1.TabIndex = 0;
            this.pathGrid1.LastClickedItemChanged += new System.EventHandler(this.pathGrid1_LastClickedItemChanged);
            // 
            // PathSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(608, 450);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pathGrid1);
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "PathSelector";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Remote Files - TracerX";
            this.ResumeLayout(false);

        }

        #endregion

        private PathControl pathGrid1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
    }
}