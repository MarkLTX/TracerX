namespace TracerX
{
    partial class NewStartPage
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewStartPage));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("All Servers");
            this.pnlVersion = new System.Windows.Forms.Panel();
            this.pathPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.linkWebsite = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.topPanel = new System.Windows.Forms.Panel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnLeftManageServers = new TracerX.FlatButton();
            this.btnAddServer = new TracerX.FlatButton();
            this.btnRemoveServer = new TracerX.FlatButton();
            this.saveServerFilterBtn = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.serverTree1 = new TracerX.ServerTree();
            this.panel1 = new System.Windows.Forms.Panel();
            this.serverFilterCombo = new System.Windows.Forms.ComboBox();
            this.lblServerFilter = new System.Windows.Forms.Label();
            this.leftButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.filesGrid = new TracerX.PathControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.foldersGrid = new TracerX.PathControl();
            this.pnlVersion.SuspendLayout();
            this.pathPanel.SuspendLayout();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.leftButtonPanel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlVersion
            // 
            this.pnlVersion.AutoSize = true;
            this.pnlVersion.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlVersion.BackColor = System.Drawing.Color.Transparent;
            this.pnlVersion.Controls.Add(this.pathPanel);
            this.pnlVersion.Controls.Add(this.topPanel);
            this.pnlVersion.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlVersion.Location = new System.Drawing.Point(0, 0);
            this.pnlVersion.Name = "pnlVersion";
            this.pnlVersion.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pnlVersion.Size = new System.Drawing.Size(769, 68);
            this.pnlVersion.TabIndex = 12;
            // 
            // pathPanel
            // 
            this.pathPanel.AutoSize = true;
            this.pathPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pathPanel.BackColor = System.Drawing.Color.Transparent;
            this.pathPanel.Controls.Add(this.linkWebsite);
            this.pathPanel.Controls.Add(this.linkLabel1);
            this.pathPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pathPanel.Location = new System.Drawing.Point(15, 27);
            this.pathPanel.Name = "pathPanel";
            this.pathPanel.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.pathPanel.Size = new System.Drawing.Size(335, 38);
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
            this.linkWebsite.Size = new System.Drawing.Size(327, 18);
            this.linkWebsite.TabIndex = 7;
            this.linkWebsite.TabStop = true;
            this.linkWebsite.Text = "Visit https://github.com/MarkLTX/TracerX/releases";
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
            this.linkLabel1.Size = new System.Drawing.Size(327, 18);
            this.linkLabel1.TabIndex = 6;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Edit version checking settings";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.linkLabel1_MouseClick);
            // 
            // topPanel
            // 
            this.topPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.topPanel.Controls.Add(this.lblVersion);
            this.topPanel.Controls.Add(this.label2);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(10, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(759, 24);
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
            this.label2.Size = new System.Drawing.Size(759, 1);
            this.label2.TabIndex = 2;
            this.label2.Text = "label2";
            // 
            // btnLeftManageServers
            // 
            this.btnLeftManageServers.AutoSize = true;
            this.btnLeftManageServers.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLeftManageServers.BackColor = System.Drawing.Color.Transparent;
            this.btnLeftManageServers.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btnLeftManageServers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeftManageServers.Image = ((System.Drawing.Image)(resources.GetObject("btnLeftManageServers.Image")));
            this.btnLeftManageServers.IsChecked = false;
            this.btnLeftManageServers.Location = new System.Drawing.Point(3, 3);
            this.btnLeftManageServers.Name = "btnLeftManageServers";
            this.btnLeftManageServers.Size = new System.Drawing.Size(24, 24);
            this.btnLeftManageServers.TabIndex = 5;
            this.toolTip1.SetToolTip(this.btnLeftManageServers, "Manage servers");
            this.btnLeftManageServers.UseVisualStyleBackColor = false;
            this.btnLeftManageServers.Click += new System.EventHandler(this.btnLeftManageServers_Click);
            // 
            // btnAddServer
            // 
            this.btnAddServer.AutoSize = true;
            this.btnAddServer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddServer.BackColor = System.Drawing.Color.Transparent;
            this.btnAddServer.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btnAddServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddServer.Image = ((System.Drawing.Image)(resources.GetObject("btnAddServer.Image")));
            this.btnAddServer.IsChecked = false;
            this.btnAddServer.Location = new System.Drawing.Point(33, 3);
            this.btnAddServer.Name = "btnAddServer";
            this.btnAddServer.Size = new System.Drawing.Size(24, 24);
            this.btnAddServer.TabIndex = 6;
            this.toolTip1.SetToolTip(this.btnAddServer, "Add server");
            this.btnAddServer.UseVisualStyleBackColor = false;
            this.btnAddServer.Click += new System.EventHandler(this.btnAddServer_Click);
            // 
            // btnRemoveServer
            // 
            this.btnRemoveServer.AutoSize = true;
            this.btnRemoveServer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRemoveServer.BackColor = System.Drawing.Color.Transparent;
            this.btnRemoveServer.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btnRemoveServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveServer.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveServer.Image")));
            this.btnRemoveServer.IsChecked = false;
            this.btnRemoveServer.Location = new System.Drawing.Point(63, 3);
            this.btnRemoveServer.Name = "btnRemoveServer";
            this.btnRemoveServer.Size = new System.Drawing.Size(24, 24);
            this.btnRemoveServer.TabIndex = 7;
            this.toolTip1.SetToolTip(this.btnRemoveServer, "Remove server");
            this.btnRemoveServer.UseVisualStyleBackColor = false;
            this.btnRemoveServer.Click += new System.EventHandler(this.btnRemoveServer_Click);
            // 
            // saveServerFilterBtn
            // 
            this.saveServerFilterBtn.Enabled = false;
            this.saveServerFilterBtn.Image = global::TracerX.Properties.Resources.Green_Plus_16;
            this.saveServerFilterBtn.Location = new System.Drawing.Point(174, 0);
            this.saveServerFilterBtn.Name = "saveServerFilterBtn";
            this.saveServerFilterBtn.Size = new System.Drawing.Size(21, 21);
            this.saveServerFilterBtn.TabIndex = 3;
            this.toolTip1.SetToolTip(this.saveServerFilterBtn, "Save");
            this.saveServerFilterBtn.UseVisualStyleBackColor = true;
            this.saveServerFilterBtn.Click += new System.EventHandler(this.saveServerFilterBtn_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.DarkGray;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 68);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.serverTree1);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.leftButtonPanel);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.splitContainer1.Panel1MinSize = 50;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.BlueViolet;
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel2MinSize = 50;
            this.splitContainer1.Size = new System.Drawing.Size(769, 530);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 17;
            this.splitContainer1.SplitterMoving += new System.Windows.Forms.SplitterCancelEventHandler(this.splitContainer1_SplitterMoving);
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // serverTree1
            // 
            this.serverTree1.BackColor = System.Drawing.Color.Bisque;
            this.serverTree1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.serverTree1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverTree1.FullRowSelect = true;
            this.serverTree1.Location = new System.Drawing.Point(0, 92);
            this.serverTree1.Name = "serverTree1";
            treeNode1.Name = "Node0";
            treeNode1.Text = "All Servers";
            this.serverTree1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.serverTree1.SelectedServer = null;
            this.serverTree1.ShowLines = false;
            this.serverTree1.Size = new System.Drawing.Size(198, 438);
            this.serverTree1.TabIndex = 5;
            this.serverTree1.SelectedServerChanged += new System.EventHandler(this.serverTree1_SelectedServerChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.saveServerFilterBtn);
            this.panel1.Controls.Add(this.serverFilterCombo);
            this.panel1.Controls.Add(this.lblServerFilter);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 62);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(198, 30);
            this.panel1.TabIndex = 8;
            // 
            // serverFilterCombo
            // 
            this.serverFilterCombo.FormattingEnabled = true;
            this.serverFilterCombo.Location = new System.Drawing.Point(36, 0);
            this.serverFilterCombo.Name = "serverFilterCombo";
            this.serverFilterCombo.Size = new System.Drawing.Size(135, 21);
            this.serverFilterCombo.TabIndex = 2;
            this.serverFilterCombo.SelectedIndexChanged += new System.EventHandler(this.serverFilterCombo_SelectedIndexChanged);
            this.serverFilterCombo.TextUpdate += new System.EventHandler(this.serverFilterCombo_TextUpdate);
            // 
            // lblServerFilter
            // 
            this.lblServerFilter.Location = new System.Drawing.Point(3, 3);
            this.lblServerFilter.Name = "lblServerFilter";
            this.lblServerFilter.Size = new System.Drawing.Size(33, 13);
            this.lblServerFilter.TabIndex = 0;
            this.lblServerFilter.Text = "Filter:";
            this.lblServerFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // leftButtonPanel
            // 
            this.leftButtonPanel.AutoSize = true;
            this.leftButtonPanel.BackColor = System.Drawing.Color.Transparent;
            this.leftButtonPanel.Controls.Add(this.btnLeftManageServers);
            this.leftButtonPanel.Controls.Add(this.btnAddServer);
            this.leftButtonPanel.Controls.Add(this.btnRemoveServer);
            this.leftButtonPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.leftButtonPanel.Location = new System.Drawing.Point(0, 29);
            this.leftButtonPanel.Name = "leftButtonPanel";
            this.leftButtonPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.leftButtonPanel.Size = new System.Drawing.Size(198, 33);
            this.leftButtonPanel.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label1.Size = new System.Drawing.Size(74, 29);
            this.label1.TabIndex = 19;
            this.label1.Text = "Servers";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.ItemSize = new System.Drawing.Size(50, 22);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(9, 4);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(565, 530);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.filesGrid);
            this.tabPage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.tabPage1.Size = new System.Drawing.Size(557, 500);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Files";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // filesGrid
            // 
            this.filesGrid.ArchiveVisibility = TracerX.ArchiveVisibility.ViewedArchives;
            this.filesGrid.AutoSize = true;
            this.filesGrid.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.filesGrid.BackColor = System.Drawing.Color.PaleVioletRed;
            this.filesGrid.ColumnHeaderBackColor = System.Drawing.Color.LemonChiffon;
            this.filesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filesGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.filesGrid.Location = new System.Drawing.Point(0, 4);
            this.filesGrid.Margin = new System.Windows.Forms.Padding(0);
            this.filesGrid.MinimumSize = new System.Drawing.Size(20, 0);
            this.filesGrid.Name = "filesGrid";
            this.filesGrid.PathsAreFolders = false;
            this.filesGrid.PathsAreLocal = false;
            this.filesGrid.RemoteServer = null;
            this.filesGrid.ShowTimesAgo = false;
            this.filesGrid.Size = new System.Drawing.Size(557, 496);
            this.filesGrid.TabIndex = 0;
            this.filesGrid.RefreshClicked += new System.EventHandler(this.filesGrid_RefreshClicked);
            this.filesGrid.LastClickedItemChanged += new System.EventHandler(this.PathGrid_LastClickedItemChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.foldersGrid);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.tabPage2.Size = new System.Drawing.Size(557, 500);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Folders";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // foldersGrid
            // 
            this.foldersGrid.ArchiveVisibility = TracerX.ArchiveVisibility.NoArchives;
            this.foldersGrid.AutoSize = true;
            this.foldersGrid.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.foldersGrid.BackColor = System.Drawing.Color.Transparent;
            this.foldersGrid.ColumnHeaderBackColor = System.Drawing.Color.LemonChiffon;
            this.foldersGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.foldersGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.foldersGrid.Location = new System.Drawing.Point(0, 4);
            this.foldersGrid.Margin = new System.Windows.Forms.Padding(0);
            this.foldersGrid.MinimumSize = new System.Drawing.Size(20, 0);
            this.foldersGrid.Name = "foldersGrid";
            this.foldersGrid.PathsAreFolders = true;
            this.foldersGrid.PathsAreLocal = false;
            this.foldersGrid.RemoteServer = null;
            this.foldersGrid.ShowTimesAgo = false;
            this.foldersGrid.Size = new System.Drawing.Size(557, 496);
            this.foldersGrid.TabIndex = 0;
            this.foldersGrid.RefreshClicked += new System.EventHandler(this.filesGrid_RefreshClicked);
            this.foldersGrid.LastClickedItemChanged += new System.EventHandler(this.PathGrid_LastClickedItemChanged);
            // 
            // NewStartPage
            // 
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.pnlVersion);
            this.Name = "NewStartPage";
            this.Size = new System.Drawing.Size(769, 598);
            this.pnlVersion.ResumeLayout(false);
            this.pnlVersion.PerformLayout();
            this.pathPanel.ResumeLayout(false);
            this.pathPanel.PerformLayout();
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.leftButtonPanel.ResumeLayout(false);
            this.leftButtonPanel.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlVersion;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel pathPanel;
        private System.Windows.Forms.LinkLabel linkWebsite;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ServerTree serverTree1;
        private PathControl filesGrid;
        private PathControl foldersGrid;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.FlowLayoutPanel leftButtonPanel;
        private FlatButton btnLeftManageServers;
        private FlatButton btnAddServer;
        private FlatButton btnRemoveServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblServerFilter;
        private System.Windows.Forms.ComboBox serverFilterCombo;
        private System.Windows.Forms.Button saveServerFilterBtn;
    }
}
