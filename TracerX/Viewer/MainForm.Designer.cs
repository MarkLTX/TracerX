namespace TracerX.Viewer {
    partial class MainForm {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.TheListView = new System.Windows.Forms.ListView();
            this.headerText = new System.Windows.Forms.ColumnHeader();
            this.headerLine = new System.Windows.Forms.ColumnHeader();
            this.headerLevel = new System.Windows.Forms.ColumnHeader();
            this.headerLogger = new System.Windows.Forms.ColumnHeader();
            this.headerThreadId = new System.Windows.Forms.ColumnHeader();
            this.headerThreadName = new System.Windows.Forms.ColumnHeader();
            this.headerTime = new System.Windows.Forms.ColumnHeader();
            this.headerMethod = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showOnlySelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSelectedThreadNamesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSelectedThreadsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loggersToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ShowTraceLevelsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideSelectedThreadNamesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideSelectedThreadsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loggersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HideTraceLevelsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bookmarkSelectedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bookmarkThreadsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bookmarkLoggersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bookmarkTraceLevelsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.showCallStackMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTextWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.callerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.endOfMethodMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setZeroTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.filenameLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.pctLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonStop = new System.Windows.Forms.ToolStripDropDownButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.bookmarkToggle = new System.Windows.Forms.ToolStripMenuItem();
            this.nextBookmarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previousBookmarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearAllBookmarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.copyTextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyColsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearAllFilteringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.columnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.openFilters = new System.Windows.Forms.ToolStripButton();
            this.NoFilteringButton = new System.Windows.Forms.ToolStripButton();
            this.startAutoRefresh = new System.Windows.Forms.ToolStripButton();
            this.stopAutoRefresh = new System.Windows.Forms.ToolStripButton();
            this.columnContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.colMenuFilterItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colMenuRemoveItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.colMenuOptionsItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colMenuColumnItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.columnContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // TheListView
            // 
            this.TheListView.AllowColumnReorder = true;
            this.TheListView.BackColor = System.Drawing.SystemColors.Window;
            this.TheListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.headerText,
            this.headerLine,
            this.headerLevel,
            this.headerLogger,
            this.headerThreadId,
            this.headerThreadName,
            this.headerTime,
            this.headerMethod});
            this.TheListView.ContextMenuStrip = this.contextMenuStrip1;
            this.TheListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TheListView.FullRowSelect = true;
            this.TheListView.HideSelection = false;
            this.TheListView.Location = new System.Drawing.Point(0, 49);
            this.TheListView.Name = "TheListView";
            this.TheListView.ShowItemToolTips = true;
            this.TheListView.Size = new System.Drawing.Size(886, 593);
            this.TheListView.SmallImageList = this.imageList1;
            this.TheListView.TabIndex = 0;
            this.TheListView.UseCompatibleStateImageBehavior = false;
            this.TheListView.View = System.Windows.Forms.View.Details;
            this.TheListView.VirtualMode = true;
            this.TheListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TheListView_MouseDoubleClick);
            this.TheListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.TheListView_ColumnClick);
            this.TheListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.GetVirtualItem);
            this.TheListView.CacheVirtualItems += new System.Windows.Forms.CacheVirtualItemsEventHandler(this.listView1_CacheVirtualItems);
            // 
            // headerText
            // 
            this.headerText.DisplayIndex = 7;
            this.headerText.Text = "Text";
            this.headerText.Width = 298;
            // 
            // headerLine
            // 
            this.headerLine.DisplayIndex = 0;
            this.headerLine.Text = "Line #";
            // 
            // headerLevel
            // 
            this.headerLevel.DisplayIndex = 1;
            this.headerLevel.Text = "Level";
            this.headerLevel.Width = 48;
            // 
            // headerLogger
            // 
            this.headerLogger.DisplayIndex = 2;
            this.headerLogger.Text = "Logger";
            this.headerLogger.Width = 87;
            // 
            // headerThreadId
            // 
            this.headerThreadId.DisplayIndex = 3;
            this.headerThreadId.Text = "Th#";
            this.headerThreadId.Width = 45;
            // 
            // headerThreadName
            // 
            this.headerThreadName.DisplayIndex = 4;
            this.headerThreadName.Text = "ThName";
            this.headerThreadName.Width = 84;
            // 
            // headerTime
            // 
            this.headerTime.DisplayIndex = 5;
            this.headerTime.Text = "Time";
            this.headerTime.Width = 117;
            // 
            // headerMethod
            // 
            this.headerMethod.DisplayIndex = 6;
            this.headerMethod.Text = "Method";
            this.headerMethod.Width = 78;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showOnlySelectedToolStripMenuItem,
            this.hideSelectedToolStripMenuItem,
            this.bookmarkSelectedMenuItem,
            this.toolStripSeparator4,
            this.showCallStackMenuItem,
            this.viewTextWindowToolStripMenuItem,
            this.goToToolStripMenuItem,
            this.setZeroTimeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(161, 164);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // showOnlySelectedToolStripMenuItem
            // 
            this.showOnlySelectedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showSelectedThreadNamesMenuItem,
            this.showSelectedThreadsMenuItem,
            this.loggersToolStripMenuItem1,
            this.ShowTraceLevelsMenuItem});
            this.showOnlySelectedToolStripMenuItem.Name = "showOnlySelectedToolStripMenuItem";
            this.showOnlySelectedToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.showOnlySelectedToolStripMenuItem.Text = "Show Only Selected";
            // 
            // showSelectedThreadNamesMenuItem
            // 
            this.showSelectedThreadNamesMenuItem.Name = "showSelectedThreadNamesMenuItem";
            this.showSelectedThreadNamesMenuItem.Size = new System.Drawing.Size(154, 22);
            this.showSelectedThreadNamesMenuItem.Text = "Thread Names";
            this.showSelectedThreadNamesMenuItem.Click += new System.EventHandler(this.showSelectedThreadNamesMenuItem_Click);
            // 
            // showSelectedThreadsMenuItem
            // 
            this.showSelectedThreadsMenuItem.Name = "showSelectedThreadsMenuItem";
            this.showSelectedThreadsMenuItem.Size = new System.Drawing.Size(154, 22);
            this.showSelectedThreadsMenuItem.Text = "Thread IDs";
            this.showSelectedThreadsMenuItem.Click += new System.EventHandler(this.showSelectedThreadsMenuItem_Click);
            // 
            // loggersToolStripMenuItem1
            // 
            this.loggersToolStripMenuItem1.Name = "loggersToolStripMenuItem1";
            this.loggersToolStripMenuItem1.Size = new System.Drawing.Size(154, 22);
            this.loggersToolStripMenuItem1.Text = "Loggers";
            this.loggersToolStripMenuItem1.Click += new System.EventHandler(this.ShowSelectedLoggersMenuItem_Click);
            // 
            // ShowTraceLevelsMenuItem
            // 
            this.ShowTraceLevelsMenuItem.Name = "ShowTraceLevelsMenuItem";
            this.ShowTraceLevelsMenuItem.Size = new System.Drawing.Size(154, 22);
            this.ShowTraceLevelsMenuItem.Text = "Trace Levels";
            this.ShowTraceLevelsMenuItem.Click += new System.EventHandler(this.ShowTraceLevelsMenuItem_Click);
            // 
            // hideSelectedToolStripMenuItem
            // 
            this.hideSelectedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideSelectedThreadNamesMenuItem,
            this.hideSelectedThreadsMenuItem,
            this.loggersToolStripMenuItem,
            this.HideTraceLevelsMenuItem});
            this.hideSelectedToolStripMenuItem.Name = "hideSelectedToolStripMenuItem";
            this.hideSelectedToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.hideSelectedToolStripMenuItem.Text = "Hide Selected";
            // 
            // hideSelectedThreadNamesMenuItem
            // 
            this.hideSelectedThreadNamesMenuItem.Name = "hideSelectedThreadNamesMenuItem";
            this.hideSelectedThreadNamesMenuItem.Size = new System.Drawing.Size(154, 22);
            this.hideSelectedThreadNamesMenuItem.Text = "Thread Names";
            this.hideSelectedThreadNamesMenuItem.Click += new System.EventHandler(this.hideSelectedThreadNamesMenuItem_Click);
            // 
            // hideSelectedThreadsMenuItem
            // 
            this.hideSelectedThreadsMenuItem.Name = "hideSelectedThreadsMenuItem";
            this.hideSelectedThreadsMenuItem.Size = new System.Drawing.Size(154, 22);
            this.hideSelectedThreadsMenuItem.Text = "Thread IDs";
            this.hideSelectedThreadsMenuItem.Click += new System.EventHandler(this.hideSelectedThreadsMenuItem_Click);
            // 
            // loggersToolStripMenuItem
            // 
            this.loggersToolStripMenuItem.Name = "loggersToolStripMenuItem";
            this.loggersToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.loggersToolStripMenuItem.Text = "Loggers";
            this.loggersToolStripMenuItem.Click += new System.EventHandler(this.HideSelectedLoggersMenuItem_Click);
            // 
            // HideTraceLevelsMenuItem
            // 
            this.HideTraceLevelsMenuItem.Name = "HideTraceLevelsMenuItem";
            this.HideTraceLevelsMenuItem.Size = new System.Drawing.Size(154, 22);
            this.HideTraceLevelsMenuItem.Text = "Trace Levels";
            this.HideTraceLevelsMenuItem.Click += new System.EventHandler(this.HideTraceLevelsMenuItem_Click);
            // 
            // bookmarkSelectedMenuItem
            // 
            this.bookmarkSelectedMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bookmarkThreadsMenuItem,
            this.bookmarkLoggersMenuItem,
            this.bookmarkTraceLevelsMenuItem});
            this.bookmarkSelectedMenuItem.Enabled = false;
            this.bookmarkSelectedMenuItem.Name = "bookmarkSelectedMenuItem";
            this.bookmarkSelectedMenuItem.Size = new System.Drawing.Size(160, 22);
            this.bookmarkSelectedMenuItem.Text = "Bookmark Selected";
            // 
            // bookmarkThreadsMenuItem
            // 
            this.bookmarkThreadsMenuItem.Name = "bookmarkThreadsMenuItem";
            this.bookmarkThreadsMenuItem.Size = new System.Drawing.Size(145, 22);
            this.bookmarkThreadsMenuItem.Text = "Threads";
            this.bookmarkThreadsMenuItem.Click += new System.EventHandler(this.bookmarkThreadsMenuItem_Click);
            // 
            // bookmarkLoggersMenuItem
            // 
            this.bookmarkLoggersMenuItem.Name = "bookmarkLoggersMenuItem";
            this.bookmarkLoggersMenuItem.Size = new System.Drawing.Size(145, 22);
            this.bookmarkLoggersMenuItem.Text = "Loggers";
            this.bookmarkLoggersMenuItem.Click += new System.EventHandler(this.bookmarkLoggersMenuItem_Click);
            // 
            // bookmarkTraceLevelsMenuItem
            // 
            this.bookmarkTraceLevelsMenuItem.Name = "bookmarkTraceLevelsMenuItem";
            this.bookmarkTraceLevelsMenuItem.Size = new System.Drawing.Size(145, 22);
            this.bookmarkTraceLevelsMenuItem.Text = "Trace Levels";
            this.bookmarkTraceLevelsMenuItem.Click += new System.EventHandler(this.bookmarkTraceLevelsMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(157, 6);
            // 
            // showCallStackMenuItem
            // 
            this.showCallStackMenuItem.Name = "showCallStackMenuItem";
            this.showCallStackMenuItem.Size = new System.Drawing.Size(160, 22);
            this.showCallStackMenuItem.Text = "View Call Stack...";
            this.showCallStackMenuItem.Click += new System.EventHandler(this.showCallStackMenuItem_Click);
            // 
            // viewTextWindowToolStripMenuItem
            // 
            this.viewTextWindowToolStripMenuItem.Name = "viewTextWindowToolStripMenuItem";
            this.viewTextWindowToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.viewTextWindowToolStripMenuItem.Text = "View Text Window...";
            this.viewTextWindowToolStripMenuItem.Click += new System.EventHandler(this.viewTextWindowToolStripMenuItem_Click);
            // 
            // goToToolStripMenuItem
            // 
            this.goToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.callerMenuItem,
            this.endOfMethodMenuItem});
            this.goToToolStripMenuItem.Name = "goToToolStripMenuItem";
            this.goToToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.goToToolStripMenuItem.Text = "Go To";
            // 
            // callerMenuItem
            // 
            this.callerMenuItem.Name = "callerMenuItem";
            this.callerMenuItem.Size = new System.Drawing.Size(155, 22);
            this.callerMenuItem.Text = "Caller";
            this.callerMenuItem.Click += new System.EventHandler(this.callerMenuItem_Click);
            // 
            // endOfMethodMenuItem
            // 
            this.endOfMethodMenuItem.Name = "endOfMethodMenuItem";
            this.endOfMethodMenuItem.Size = new System.Drawing.Size(155, 22);
            this.endOfMethodMenuItem.Text = "End of method";
            this.endOfMethodMenuItem.Click += new System.EventHandler(this.endOfMethodMenuItem_Click);
            // 
            // setZeroTimeToolStripMenuItem
            // 
            this.setZeroTimeToolStripMenuItem.Name = "setZeroTimeToolStripMenuItem";
            this.setZeroTimeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.setZeroTimeToolStripMenuItem.Text = "Set Zero Time";
            this.setZeroTimeToolStripMenuItem.Click += new System.EventHandler(this.setZeroTimeToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "Plus.ico");
            this.imageList1.Images.SetKeyName(2, "BookmarkPlus.ico");
            this.imageList1.Images.SetKeyName(3, "Minus.ico");
            this.imageList1.Images.SetKeyName(4, "BookmarkMinus.ico");
            this.imageList1.Images.SetKeyName(5, "ArrowDown.ico");
            this.imageList1.Images.SetKeyName(6, "BookmarkArrowDown.ico");
            this.imageList1.Images.SetKeyName(7, "ArrowUp.ico");
            this.imageList1.Images.SetKeyName(8, "BookmarkArrowUp.ico");
            this.imageList1.Images.SetKeyName(9, "Filter.ico");
            this.imageList1.Images.SetKeyName(10, "Subline.ico");
            this.imageList1.Images.SetKeyName(11, "BookmarkSubline.ico");
            this.imageList1.Images.SetKeyName(12, "LastSubline.ico");
            this.imageList1.Images.SetKeyName(13, "BookmarkLastSubline.ico");
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filenameLabel,
            this.toolStripProgressBar1,
            this.pctLabel,
            this.buttonStop});
            this.statusStrip1.Location = new System.Drawing.Point(0, 642);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(886, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // filenameLabel
            // 
            this.filenameLabel.Name = "filenameLabel";
            this.filenameLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.AutoToolTip = true;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // pctLabel
            // 
            this.pctLabel.Name = "pctLabel";
            this.pctLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // buttonStop
            // 
            this.buttonStop.BackColor = System.Drawing.Color.Silver;
            this.buttonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonStop.Enabled = false;
            this.buttonStop.Image = ((System.Drawing.Image)(resources.GetObject("buttonStop.Image")));
            this.buttonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.ShowDropDownArrow = false;
            this.buttonStop.Size = new System.Drawing.Size(33, 20);
            this.buttonStop.Text = "Stop";
            this.buttonStop.ToolTipText = "Stop loading file";
            this.buttonStop.Click += new System.EventHandler(this.toolStripDropDownButton1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(886, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.propertiesToolStripMenuItem,
            this.toolStripSeparator1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.fileToolStripMenuItem_DropDownItemClicked);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(131, 6);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem,
            this.findNextToolStripMenuItem,
            this.findPreviousToolStripMenuItem,
            this.toolStripSeparator2,
            this.bookmarkToggle,
            this.nextBookmarkToolStripMenuItem,
            this.previousBookmarkToolStripMenuItem,
            this.clearAllBookmarksToolStripMenuItem,
            this.toolStripSeparator3,
            this.copyTextMenuItem,
            this.copyColsMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.DropDownClosed += new System.EventHandler(this.editToolStripMenuItem_DropDownClosed);
            this.editToolStripMenuItem.DropDownOpening += new System.EventHandler(this.editToolStripMenuItem_DropDownOpening);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Enabled = false;
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.findToolStripMenuItem.Text = "Find...";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // findNextToolStripMenuItem
            // 
            this.findNextToolStripMenuItem.Enabled = false;
            this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
            this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findNextToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.findNextToolStripMenuItem.Text = "Find Next";
            this.findNextToolStripMenuItem.Click += new System.EventHandler(this.findNextToolStripMenuItem_Click);
            // 
            // findPreviousToolStripMenuItem
            // 
            this.findPreviousToolStripMenuItem.Enabled = false;
            this.findPreviousToolStripMenuItem.Name = "findPreviousToolStripMenuItem";
            this.findPreviousToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.findPreviousToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.findPreviousToolStripMenuItem.Text = "Find Previous";
            this.findPreviousToolStripMenuItem.Click += new System.EventHandler(this.findPreviousToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(242, 6);
            // 
            // bookmarkToggle
            // 
            this.bookmarkToggle.Name = "bookmarkToggle";
            this.bookmarkToggle.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
            this.bookmarkToggle.Size = new System.Drawing.Size(245, 22);
            this.bookmarkToggle.Text = "Toggle Bookmark";
            this.bookmarkToggle.Click += new System.EventHandler(this.bookmarkToggle_Click);
            // 
            // nextBookmarkToolStripMenuItem
            // 
            this.nextBookmarkToolStripMenuItem.Name = "nextBookmarkToolStripMenuItem";
            this.nextBookmarkToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.nextBookmarkToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.nextBookmarkToolStripMenuItem.Text = "Next Bookmark";
            this.nextBookmarkToolStripMenuItem.Click += new System.EventHandler(this.nextBookmarkToolStripMenuItem_Click);
            // 
            // previousBookmarkToolStripMenuItem
            // 
            this.previousBookmarkToolStripMenuItem.Name = "previousBookmarkToolStripMenuItem";
            this.previousBookmarkToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
            this.previousBookmarkToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.previousBookmarkToolStripMenuItem.Text = "Previous Bookmark";
            this.previousBookmarkToolStripMenuItem.Click += new System.EventHandler(this.previousBookmarkToolStripMenuItem_Click);
            // 
            // clearAllBookmarksToolStripMenuItem
            // 
            this.clearAllBookmarksToolStripMenuItem.Name = "clearAllBookmarksToolStripMenuItem";
            this.clearAllBookmarksToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.clearAllBookmarksToolStripMenuItem.Text = "Clear All Bookmarks";
            this.clearAllBookmarksToolStripMenuItem.Click += new System.EventHandler(this.clearAllBookmarksToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(242, 6);
            // 
            // copyTextMenuItem
            // 
            this.copyTextMenuItem.Name = "copyTextMenuItem";
            this.copyTextMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyTextMenuItem.Size = new System.Drawing.Size(245, 22);
            this.copyTextMenuItem.Text = "Copy Text";
            this.copyTextMenuItem.Click += new System.EventHandler(this.copyTextMenuItem_Click);
            // 
            // copyColsMenuItem
            // 
            this.copyColsMenuItem.Name = "copyColsMenuItem";
            this.copyColsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
                        | System.Windows.Forms.Keys.C)));
            this.copyColsMenuItem.Size = new System.Drawing.Size(245, 22);
            this.copyColsMenuItem.Text = "Copy Visible Columns";
            this.copyColsMenuItem.Click += new System.EventHandler(this.copyColsMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterToolStripMenuItem,
            this.clearAllFilteringToolStripMenuItem,
            this.columnsToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.refreshMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "View";
            this.viewToolStripMenuItem.DropDownOpening += new System.EventHandler(this.viewToolStripMenuItem_DropDownOpening);
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.Enabled = false;
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.filterToolStripMenuItem.Text = "Filter...";
            this.filterToolStripMenuItem.Click += new System.EventHandler(this.filterToolStripMenuItem_Click);
            // 
            // clearAllFilteringToolStripMenuItem
            // 
            this.clearAllFilteringToolStripMenuItem.Enabled = false;
            this.clearAllFilteringToolStripMenuItem.Image = global::TracerX.Properties.Resources.FilterNot;
            this.clearAllFilteringToolStripMenuItem.Name = "clearAllFilteringToolStripMenuItem";
            this.clearAllFilteringToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.clearAllFilteringToolStripMenuItem.Text = "Clear All Filtering";
            this.clearAllFilteringToolStripMenuItem.Click += new System.EventHandler(this.clearAllFilteringToolStripMenuItem_Click);
            // 
            // columnsToolStripMenuItem
            // 
            this.columnsToolStripMenuItem.Name = "columnsToolStripMenuItem";
            this.columnsToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.columnsToolStripMenuItem.Text = "Columns...";
            this.columnsToolStripMenuItem.Click += new System.EventHandler(this.columnsToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.optionsToolStripMenuItem.Text = "Options...";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // refreshMenuItem
            // 
            this.refreshMenuItem.Enabled = false;
            this.refreshMenuItem.Name = "refreshMenuItem";
            this.refreshMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshMenuItem.Size = new System.Drawing.Size(165, 22);
            this.refreshMenuItem.Text = "Refresh";
            this.refreshMenuItem.Click += new System.EventHandler(this.refreshMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.licenseToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // licenseToolStripMenuItem
            // 
            this.licenseToolStripMenuItem.Name = "licenseToolStripMenuItem";
            this.licenseToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.licenseToolStripMenuItem.Text = "License...";
            this.licenseToolStripMenuItem.Click += new System.EventHandler(this.licenseToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFilters,
            this.NoFilteringButton,
            this.startAutoRefresh,
            this.stopAutoRefresh});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(886, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // openFilters
            // 
            this.openFilters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openFilters.Enabled = false;
            this.openFilters.Image = global::TracerX.Properties.Resources.Filter;
            this.openFilters.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openFilters.Name = "openFilters";
            this.openFilters.Size = new System.Drawing.Size(23, 22);
            this.openFilters.Text = "Open filter dialog";
            this.openFilters.Click += new System.EventHandler(this.openFilters_Click);
            // 
            // NoFilteringButton
            // 
            this.NoFilteringButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.NoFilteringButton.Enabled = false;
            this.NoFilteringButton.Image = global::TracerX.Properties.Resources.FilterNot;
            this.NoFilteringButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NoFilteringButton.Name = "NoFilteringButton";
            this.NoFilteringButton.Size = new System.Drawing.Size(23, 22);
            this.NoFilteringButton.Text = "toolStripButton1";
            this.NoFilteringButton.ToolTipText = "Clear all filtering";
            this.NoFilteringButton.Click += new System.EventHandler(this.clearAllFilteringToolStripMenuItem_Click);
            // 
            // startAutoRefresh
            // 
            this.startAutoRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.startAutoRefresh.Enabled = false;
            this.startAutoRefresh.Image = ((System.Drawing.Image)(resources.GetObject("startAutoRefresh.Image")));
            this.startAutoRefresh.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.startAutoRefresh.Name = "startAutoRefresh";
            this.startAutoRefresh.Size = new System.Drawing.Size(23, 22);
            this.startAutoRefresh.Text = "startAutoRefresh";
            this.startAutoRefresh.ToolTipText = "Start auto-refresh";
            this.startAutoRefresh.Click += new System.EventHandler(this.startAutoRefresh_Click);
            // 
            // stopAutoRefresh
            // 
            this.stopAutoRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopAutoRefresh.Enabled = false;
            this.stopAutoRefresh.Image = ((System.Drawing.Image)(resources.GetObject("stopAutoRefresh.Image")));
            this.stopAutoRefresh.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.stopAutoRefresh.Name = "stopAutoRefresh";
            this.stopAutoRefresh.Size = new System.Drawing.Size(23, 22);
            this.stopAutoRefresh.Text = "Stop Auto-Refresh";
            this.stopAutoRefresh.Click += new System.EventHandler(this.stopAutoRefresh_Click);
            // 
            // columnContextMenu
            // 
            this.columnContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.colMenuFilterItem,
            this.colMenuRemoveItem,
            this.toolStripSeparator5,
            this.colMenuOptionsItem,
            this.colMenuColumnItem});
            this.columnContextMenu.Name = "columnContextMenu";
            this.columnContextMenu.ShowImageMargin = false;
            this.columnContextMenu.Size = new System.Drawing.Size(152, 98);
            // 
            // colMenuFilterItem
            // 
            this.colMenuFilterItem.Name = "colMenuFilterItem";
            this.colMenuFilterItem.Size = new System.Drawing.Size(151, 22);
            this.colMenuFilterItem.Text = "Filter...";
            this.colMenuFilterItem.Click += new System.EventHandler(this.colMenuFilterItem_Click);
            // 
            // colMenuRemoveItem
            // 
            this.colMenuRemoveItem.Name = "colMenuRemoveItem";
            this.colMenuRemoveItem.Size = new System.Drawing.Size(151, 22);
            this.colMenuRemoveItem.Text = "Remove from Filter";
            this.colMenuRemoveItem.Click += new System.EventHandler(this.colMenuRemoveItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(148, 6);
            // 
            // colMenuOptionsItem
            // 
            this.colMenuOptionsItem.Name = "colMenuOptionsItem";
            this.colMenuOptionsItem.Size = new System.Drawing.Size(151, 22);
            this.colMenuOptionsItem.Text = "Options...";
            this.colMenuOptionsItem.Click += new System.EventHandler(this.colMenuOptionsItem_Click);
            // 
            // colMenuColumnItem
            // 
            this.colMenuColumnItem.Name = "colMenuColumnItem";
            this.colMenuColumnItem.Size = new System.Drawing.Size(151, 22);
            this.colMenuColumnItem.Text = "Columns...";
            this.colMenuColumnItem.Click += new System.EventHandler(this.columnsToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 664);
            this.Controls.Add(this.TheListView);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "TracerX Log Viewer";
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.columnContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton buttonStop;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bookmarkToggle;
        private System.Windows.Forms.ToolStripMenuItem nextBookmarkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previousBookmarkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearAllBookmarksToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem hideSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideSelectedThreadsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOnlySelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSelectedThreadsMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton NoFilteringButton;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearAllFilteringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem columnsToolStripMenuItem;
        public System.Windows.Forms.ListView TheListView;
        private System.Windows.Forms.ToolStripMenuItem loggersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loggersToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem HideTraceLevelsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ShowTraceLevelsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showCallStackMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem callerMenuItem;
        private System.Windows.Forms.ToolStripMenuItem endOfMethodMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyTextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyColsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripStatusLabel pctLabel;
        private System.Windows.Forms.ToolStripStatusLabel filenameLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem bookmarkSelectedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bookmarkThreadsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bookmarkLoggersMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bookmarkTraceLevelsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshMenuItem;
        private System.Windows.Forms.ContextMenuStrip columnContextMenu;
        private System.Windows.Forms.ToolStripMenuItem colMenuFilterItem;
        private System.Windows.Forms.ToolStripMenuItem colMenuRemoveItem;
        private System.Windows.Forms.ToolStripMenuItem colMenuColumnItem;
        private System.Windows.Forms.ToolStripMenuItem viewTextWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setZeroTimeToolStripMenuItem;
        public System.Windows.Forms.ColumnHeader headerLogger;
        public System.Windows.Forms.ColumnHeader headerThreadId;
        public System.Windows.Forms.ColumnHeader headerThreadName;
        public System.Windows.Forms.ColumnHeader headerLevel;
        private System.Windows.Forms.ToolStripMenuItem showSelectedThreadNamesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideSelectedThreadNamesMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem colMenuOptionsItem;
        public System.Windows.Forms.ColumnHeader headerLine;
        public System.Windows.Forms.ColumnHeader headerTime;
        public System.Windows.Forms.ColumnHeader headerText;
        public System.Windows.Forms.ColumnHeader headerMethod;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton startAutoRefresh;
        private System.Windows.Forms.ToolStripButton stopAutoRefresh;
        private System.Windows.Forms.ToolStripMenuItem licenseToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton openFilters;
    }
}