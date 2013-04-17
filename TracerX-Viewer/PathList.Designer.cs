namespace TracerX
{
    partial class PathList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PathList));
            this.topPanel = new System.Windows.Forms.Panel();
            this.lblHeading = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkFullPaths = new System.Windows.Forms.CheckBox();
            this.pathPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.topPanel.SuspendLayout();
            this.pathPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.topPanel.Controls.Add(this.lblHeading);
            this.topPanel.Controls.Add(this.label2);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(342, 24);
            this.topPanel.TabIndex = 1;
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.Color.Maroon;
            this.lblHeading.Location = new System.Drawing.Point(0, 0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(124, 24);
            this.lblHeading.TabIndex = 1;
            this.lblHeading.Text = "Heading Text";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.Maroon;
            this.label2.Location = new System.Drawing.Point(0, 14);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(342, 1);
            this.label2.TabIndex = 2;
            this.label2.Text = "label2";
            // 
            // chkFullPaths
            // 
            this.chkFullPaths.AutoSize = true;
            this.chkFullPaths.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkFullPaths.Location = new System.Drawing.Point(0, 24);
            this.chkFullPaths.Name = "chkFullPaths";
            this.chkFullPaths.Padding = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.chkFullPaths.Size = new System.Drawing.Size(342, 21);
            this.chkFullPaths.TabIndex = 2;
            this.chkFullPaths.Text = "Full paths";
            this.chkFullPaths.UseVisualStyleBackColor = true;
            this.chkFullPaths.CheckedChanged += new System.EventHandler(this.chkFullPaths_CheckedChanged);
            // 
            // pathPanel
            // 
            this.pathPanel.AutoSize = true;
            this.pathPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pathPanel.BackColor = System.Drawing.Color.Transparent;
            this.pathPanel.Controls.Add(this.linkLabel2);
            this.pathPanel.Controls.Add(this.linkLabel1);
            this.pathPanel.Controls.Add(this.linkLabel3);
            this.pathPanel.Controls.Add(this.linkLabel4);
            this.pathPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pathPanel.Location = new System.Drawing.Point(5, 48);
            this.pathPanel.Name = "pathPanel";
            this.pathPanel.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.pathPanel.Size = new System.Drawing.Size(97, 76);
            this.pathPanel.TabIndex = 6;
            this.pathPanel.WrapContents = false;
            this.pathPanel.SizeChanged += new System.EventHandler(this.pathPanel_SizeChanged);
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.linkLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel2.Image = ((System.Drawing.Image)(resources.GetObject("linkLabel2.Image")));
            this.linkLabel2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel2.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel2.LinkColor = System.Drawing.Color.Blue;
            this.linkLabel2.Location = new System.Drawing.Point(5, 1);
            this.linkLabel2.Margin = new System.Windows.Forms.Padding(3, 1, 3, 0);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Padding = new System.Windows.Forms.Padding(20, 1, 0, 1);
            this.linkLabel2.Size = new System.Drawing.Size(89, 18);
            this.linkLabel2.TabIndex = 7;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "linkLabel2";
            this.linkLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.Image = global::TracerX.Properties.Resources.clockNext;
            this.linkLabel1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel1.LinkColor = System.Drawing.Color.Blue;
            this.linkLabel1.Location = new System.Drawing.Point(5, 20);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(3, 1, 3, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Padding = new System.Windows.Forms.Padding(20, 1, 0, 1);
            this.linkLabel1.Size = new System.Drawing.Size(89, 18);
            this.linkLabel1.TabIndex = 6;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "linkLabel1";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.linkLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel3.Image = global::TracerX.Properties.Resources.clockNext;
            this.linkLabel3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel3.LinkColor = System.Drawing.Color.Blue;
            this.linkLabel3.Location = new System.Drawing.Point(5, 39);
            this.linkLabel3.Margin = new System.Windows.Forms.Padding(3, 1, 3, 0);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Padding = new System.Windows.Forms.Padding(20, 1, 0, 1);
            this.linkLabel3.Size = new System.Drawing.Size(89, 18);
            this.linkLabel3.TabIndex = 8;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "linkLabel3";
            this.linkLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.linkLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel4.Image = global::TracerX.Properties.Resources.clockNext;
            this.linkLabel4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel4.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel4.LinkColor = System.Drawing.Color.Blue;
            this.linkLabel4.Location = new System.Drawing.Point(5, 58);
            this.linkLabel4.Margin = new System.Windows.Forms.Padding(3, 1, 3, 0);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Padding = new System.Windows.Forms.Padding(20, 1, 0, 1);
            this.linkLabel4.Size = new System.Drawing.Size(89, 18);
            this.linkLabel4.TabIndex = 9;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "linkLabel4";
            this.linkLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PathList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.pathPanel);
            this.Controls.Add(this.chkFullPaths);
            this.Controls.Add(this.topPanel);
            this.Name = "PathList";
            this.Size = new System.Drawing.Size(342, 339);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.pathPanel.ResumeLayout(false);
            this.pathPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkFullPaths;
        private System.Windows.Forms.FlowLayoutPanel pathPanel;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.LinkLabel linkLabel4;
    }
}
