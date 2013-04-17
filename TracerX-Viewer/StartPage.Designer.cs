namespace TracerX
{
    partial class StartPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlVersion = new System.Windows.Forms.Panel();
            this.topPanel = new System.Windows.Forms.Panel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pathPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.linkWebsite = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.recentlyCreatedFiles = new TracerX.PathList();
            this.recentlyViewedFiles = new TracerX.PathList();
            this.recentlyViewedFolders = new TracerX.PathList();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlVersion.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.pathPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.pnlVersion, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.recentlyCreatedFiles, -1, 1);
            this.tableLayoutPanel1.Controls.Add(this.recentlyViewedFiles, -1, 2);
            this.tableLayoutPanel1.Controls.Add(this.recentlyViewedFolders, -1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(502, 483);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // pnlVersion
            // 
            this.pnlVersion.AutoSize = true;
            this.pnlVersion.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlVersion.Controls.Add(this.pathPanel);
            this.pnlVersion.Controls.Add(this.topPanel);
            this.pnlVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlVersion.Location = new System.Drawing.Point(3, 3);
            this.pnlVersion.Name = "pnlVersion";
            this.pnlVersion.Size = new System.Drawing.Size(476, 68);
            this.pnlVersion.TabIndex = 12;
            // 
            // topPanel
            // 
            this.topPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.topPanel.Controls.Add(this.lblVersion);
            this.topPanel.Controls.Add(this.label2);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(476, 24);
            this.topPanel.TabIndex = 2;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.Maroon;
            this.lblVersion.Location = new System.Drawing.Point(0, 0);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(330, 24);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "A New Version of TracerX is Available";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.Maroon;
            this.label2.Location = new System.Drawing.Point(0, 14);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(476, 1);
            this.label2.TabIndex = 2;
            this.label2.Text = "label2";
            // 
            // pathPanel
            // 
            this.pathPanel.AutoSize = true;
            this.pathPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pathPanel.BackColor = System.Drawing.Color.Transparent;
            this.pathPanel.Controls.Add(this.linkWebsite);
            this.pathPanel.Controls.Add(this.linkLabel1);
            this.pathPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pathPanel.Location = new System.Drawing.Point(5, 27);
            this.pathPanel.Name = "pathPanel";
            this.pathPanel.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.pathPanel.Size = new System.Drawing.Size(292, 38);
            this.pathPanel.TabIndex = 7;
            this.pathPanel.WrapContents = false;
            // 
            // linkWebsite
            // 
            this.linkWebsite.AutoSize = true;
            this.linkWebsite.BackColor = System.Drawing.Color.Transparent;
            this.linkWebsite.Dock = System.Windows.Forms.DockStyle.Top;
            this.linkWebsite.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkWebsite.Image = ((System.Drawing.Image)(resources.GetObject("linkWebsite.Image")));
            this.linkWebsite.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkWebsite.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkWebsite.LinkColor = System.Drawing.Color.Blue;
            this.linkWebsite.Location = new System.Drawing.Point(5, 1);
            this.linkWebsite.Margin = new System.Windows.Forms.Padding(3, 1, 3, 0);
            this.linkWebsite.Name = "linkWebsite";
            this.linkWebsite.Padding = new System.Windows.Forms.Padding(20, 1, 0, 1);
            this.linkWebsite.Size = new System.Drawing.Size(284, 18);
            this.linkWebsite.TabIndex = 7;
            this.linkWebsite.TabStop = true;
            this.linkWebsite.Text = "Visit http://TracerX.CodePlex.com/releases";
            this.linkWebsite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkWebsite.MouseClick += new System.Windows.Forms.MouseEventHandler(this.linkWebsite_MouseClick);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.Image = global::TracerX.Properties.Resources.Options;
            this.linkLabel1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel1.LinkColor = System.Drawing.Color.Blue;
            this.linkLabel1.Location = new System.Drawing.Point(5, 20);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(3, 1, 3, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Padding = new System.Windows.Forms.Padding(20, 1, 0, 1);
            this.linkLabel1.Size = new System.Drawing.Size(284, 18);
            this.linkLabel1.TabIndex = 6;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Edit version checking settings";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.linkLabel1_MouseClick);
            // 
            // recentlyCreatedFiles
            // 
            this.recentlyCreatedFiles.AutoSize = true;
            this.recentlyCreatedFiles.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.recentlyCreatedFiles.BackColor = System.Drawing.Color.Transparent;
            this.recentlyCreatedFiles.Dock = System.Windows.Forms.DockStyle.Top;
            this.recentlyCreatedFiles.Heading = "Recently Created Files (on Local Machine)";
            this.recentlyCreatedFiles.Location = new System.Drawing.Point(0, 0);
            this.recentlyCreatedFiles.MinimumSize = new System.Drawing.Size(0, 140);
            this.recentlyCreatedFiles.Name = "recentlyCreatedFiles";
            this.recentlyCreatedFiles.Paths = null;
            this.recentlyCreatedFiles.PathsAreFolders = false;
            this.recentlyCreatedFiles.Size = new System.Drawing.Size(10, 140);
            this.recentlyCreatedFiles.TabIndex = 9;
            this.recentlyCreatedFiles.LastClickedPathChanged += new System.EventHandler(this.recentlyCreatedPaths_LastClickedPathChanged);
            // 
            // recentlyViewedFiles
            // 
            this.recentlyViewedFiles.AutoSize = true;
            this.recentlyViewedFiles.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.recentlyViewedFiles.BackColor = System.Drawing.Color.Transparent;
            this.recentlyViewedFiles.Dock = System.Windows.Forms.DockStyle.Top;
            this.recentlyViewedFiles.Heading = "Recently Viewed Files";
            this.recentlyViewedFiles.Location = new System.Drawing.Point(0, 0);
            this.recentlyViewedFiles.MinimumSize = new System.Drawing.Size(0, 140);
            this.recentlyViewedFiles.Name = "recentlyViewedFiles";
            this.recentlyViewedFiles.Paths = null;
            this.recentlyViewedFiles.PathsAreFolders = false;
            this.recentlyViewedFiles.Size = new System.Drawing.Size(10, 140);
            this.recentlyViewedFiles.TabIndex = 10;
            this.recentlyViewedFiles.LastClickedPathChanged += new System.EventHandler(this.recentlyCreatedPaths_LastClickedPathChanged);
            // 
            // recentlyViewedFolders
            // 
            this.recentlyViewedFolders.AutoSize = true;
            this.recentlyViewedFolders.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.recentlyViewedFolders.BackColor = System.Drawing.Color.Transparent;
            this.recentlyViewedFolders.Dock = System.Windows.Forms.DockStyle.Top;
            this.recentlyViewedFolders.Heading = "Recently Viewed Folders";
            this.recentlyViewedFolders.Location = new System.Drawing.Point(0, 0);
            this.recentlyViewedFolders.MinimumSize = new System.Drawing.Size(0, 140);
            this.recentlyViewedFolders.Name = "recentlyViewedFolders";
            this.recentlyViewedFolders.Paths = null;
            this.recentlyViewedFolders.PathsAreFolders = true;
            this.recentlyViewedFolders.Size = new System.Drawing.Size(10, 140);
            this.recentlyViewedFolders.TabIndex = 11;
            this.recentlyViewedFolders.LastClickedPathChanged += new System.EventHandler(this.recentlyCreatedPaths_LastClickedPathChanged);
            // 
            // StartPage
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "StartPage";
            this.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.Size = new System.Drawing.Size(512, 483);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.pnlVersion.ResumeLayout(false);
            this.pnlVersion.PerformLayout();
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.pathPanel.ResumeLayout(false);
            this.pathPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private PathList recentlyCreatedFiles;
        private PathList recentlyViewedFiles;
        private PathList recentlyViewedFolders;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel pnlVersion;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel pathPanel;
        private System.Windows.Forms.LinkLabel linkWebsite;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}
