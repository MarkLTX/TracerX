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
            this.recentlyViewedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentlyCreatedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.clearFilterMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.columnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.openFileButton = new System.Windows.Forms.ToolStripButton();
            this.propertiesButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.openFilters = new System.Windows.Forms.ToolStripButton();
            this.clearFilterButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.startAutoRefresh = new System.Windows.Forms.ToolStripButton();
            this.stopAutoRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.bookmarkToggleButton = new System.Windows.Forms.ToolStripButton();
            this.bookmarkPrev = new System.Windows.Forms.ToolStripButton();
            this.bookmarkNext = new System.Windows.Forms.ToolStripButton();
            this.bookmarkClear = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.findButton = new System.Windows.Forms.ToolStripButton();
            this.findPrevButton = new System.Windows.Forms.ToolStripButton();
            this.findNextButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.optionsButton = new System.Windows.Forms.ToolStripButton();
            this.columnsButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.expandAllButton = new System.Windows.Forms.ToolStripButton();
            this.relativeTimeButton = new System.Windows.Forms.ToolStripButton();
            this.columnContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.colMenuFilterItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colMenuRemoveItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.colMenuOptionsItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colMenuColumnItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.commandProvider = new Commander.UICommandProvider(this.components);
            this.openFileCmd = new Commander.UICommand(this.components);
            this.propertiesCmd = new Commander.UICommand(this.components);
            this.findCmd = new Commander.UICommand(this.components);
            this.findNextCmd = new Commander.UICommand(this.components);
            this.findPrevCmd = new Commander.UICommand(this.components);
            this.bookmarkToggleCmd = new Commander.UICommand(this.components);
            this.bookmarkNextCmd = new Commander.UICommand(this.components);
            this.bookmarkPrevCmd = new Commander.UICommand(this.components);
            this.bookmarkClearCmd = new Commander.UICommand(this.components);
            this.filterDlgCmd = new Commander.UICommand(this.components);
            this.filterClearCmd = new Commander.UICommand(this.components);
            this.columnsCmd = new Commander.UICommand(this.components);
            this.optionsCmd = new Commander.UICommand(this.components);
            this.refreshCmd = new Commander.UICommand(this.components);
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
            this.TheListView.Location = new System.Drawing.Point(0, 49);
            this.TheListView.Name = "TheListView";
            this.TheListView.ShowItemToolTips = true;
            this.TheListView.Size = new System.Drawing.Size(886, 593);
            this.TheListView.SmallImageList = this.imageList1;
            this.TheListView.TabIndex = 0;
            this.commandProvider.SetUICommand(this.TheListView, null);
            this.TheListView.UseCompatibleStateImageBehavior = false;
            this.TheListView.View = System.Windows.Forms.View.Details;
            this.TheListView.VirtualMode = true;
            this.TheListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TheListView_MouseDoubleClick);
            this.TheListView.VirtualItemsSelectionRangeChanged += new System.Windows.Forms.ListViewVirtualItemsSelectionRangeChangedEventHandler(this.TheListView_VirtualItemsSelectionRangeChanged);
            this.TheListView.SelectedIndexChanged += new System.EventHandler(this.TheListView_SelectedIndexChanged);
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
            this.commandProvider.SetUICommand(this.contextMenuStrip1, null);
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
            this.commandProvider.SetUICommand(this.showOnlySelectedToolStripMenuItem, null);
            // 
            // showSelectedThreadNamesMenuItem
            // 
            this.showSelectedThreadNamesMenuItem.Name = "showSelectedThreadNamesMenuItem";
            this.showSelectedThreadNamesMenuItem.Size = new System.Drawing.Size(154, 22);
            this.showSelectedThreadNamesMenuItem.Text = "Thread Names";
            this.commandProvider.SetUICommand(this.showSelectedThreadNamesMenuItem, null);
            this.showSelectedThreadNamesMenuItem.Click += new System.EventHandler(this.showSelectedThreadNamesMenuItem_Click);
            // 
            // showSelectedThreadsMenuItem
            // 
            this.showSelectedThreadsMenuItem.Name = "showSelectedThreadsMenuItem";
            this.showSelectedThreadsMenuItem.Size = new System.Drawing.Size(154, 22);
            this.showSelectedThreadsMenuItem.Text = "Thread IDs";
            this.commandProvider.SetUICommand(this.showSelectedThreadsMenuItem, null);
            this.showSelectedThreadsMenuItem.Click += new System.EventHandler(this.showSelectedThreadsMenuItem_Click);
            // 
            // loggersToolStripMenuItem1
            // 
            this.loggersToolStripMenuItem1.Name = "loggersToolStripMenuItem1";
            this.loggersToolStripMenuItem1.Size = new System.Drawing.Size(154, 22);
            this.loggersToolStripMenuItem1.Text = "Loggers";
            this.commandProvider.SetUICommand(this.loggersToolStripMenuItem1, null);
            this.loggersToolStripMenuItem1.Click += new System.EventHandler(this.ShowSelectedLoggersMenuItem_Click);
            // 
            // ShowTraceLevelsMenuItem
            // 
            this.ShowTraceLevelsMenuItem.Name = "ShowTraceLevelsMenuItem";
            this.ShowTraceLevelsMenuItem.Size = new System.Drawing.Size(154, 22);
            this.ShowTraceLevelsMenuItem.Text = "Trace Levels";
            this.commandProvider.SetUICommand(this.ShowTraceLevelsMenuItem, null);
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
            this.commandProvider.SetUICommand(this.hideSelectedToolStripMenuItem, null);
            // 
            // hideSelectedThreadNamesMenuItem
            // 
            this.hideSelectedThreadNamesMenuItem.Name = "hideSelectedThreadNamesMenuItem";
            this.hideSelectedThreadNamesMenuItem.Size = new System.Drawing.Size(154, 22);
            this.hideSelectedThreadNamesMenuItem.Text = "Thread Names";
            this.commandProvider.SetUICommand(this.hideSelectedThreadNamesMenuItem, null);
            this.hideSelectedThreadNamesMenuItem.Click += new System.EventHandler(this.hideSelectedThreadNamesMenuItem_Click);
            // 
            // hideSelectedThreadsMenuItem
            // 
            this.hideSelectedThreadsMenuItem.Name = "hideSelectedThreadsMenuItem";
            this.hideSelectedThreadsMenuItem.Size = new System.Drawing.Size(154, 22);
            this.hideSelectedThreadsMenuItem.Text = "Thread IDs";
            this.commandProvider.SetUICommand(this.hideSelectedThreadsMenuItem, null);
            this.hideSelectedThreadsMenuItem.Click += new System.EventHandler(this.hideSelectedThreadsMenuItem_Click);
            // 
            // loggersToolStripMenuItem
            // 
            this.loggersToolStripMenuItem.Name = "loggersToolStripMenuItem";
            this.loggersToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.loggersToolStripMenuItem.Text = "Loggers";
            this.commandProvider.SetUICommand(this.loggersToolStripMenuItem, null);
            this.loggersToolStripMenuItem.Click += new System.EventHandler(this.HideSelectedLoggersMenuItem_Click);
            // 
            // HideTraceLevelsMenuItem
            // 
            this.HideTraceLevelsMenuItem.Name = "HideTraceLevelsMenuItem";
            this.HideTraceLevelsMenuItem.Size = new System.Drawing.Size(154, 22);
            this.HideTraceLevelsMenuItem.Text = "Trace Levels";
            this.commandProvider.SetUICommand(this.HideTraceLevelsMenuItem, null);
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
            this.commandProvider.SetUICommand(this.bookmarkSelectedMenuItem, null);
            // 
            // bookmarkThreadsMenuItem
            // 
            this.bookmarkThreadsMenuItem.Name = "bookmarkThreadsMenuItem";
            this.bookmarkThreadsMenuItem.Size = new System.Drawing.Size(145, 22);
            this.bookmarkThreadsMenuItem.Text = "Threads";
            this.commandProvider.SetUICommand(this.bookmarkThreadsMenuItem, null);
            this.bookmarkThreadsMenuItem.Click += new System.EventHandler(this.bookmarkThreadsMenuItem_Click);
            // 
            // bookmarkLoggersMenuItem
            // 
            this.bookmarkLoggersMenuItem.Name = "bookmarkLoggersMenuItem";
            this.bookmarkLoggersMenuItem.Size = new System.Drawing.Size(145, 22);
            this.bookmarkLoggersMenuItem.Text = "Loggers";
            this.commandProvider.SetUICommand(this.bookmarkLoggersMenuItem, null);
            this.bookmarkLoggersMenuItem.Click += new System.EventHandler(this.bookmarkLoggersMenuItem_Click);
            // 
            // bookmarkTraceLevelsMenuItem
            // 
            this.bookmarkTraceLevelsMenuItem.Name = "bookmarkTraceLevelsMenuItem";
            this.bookmarkTraceLevelsMenuItem.Size = new System.Drawing.Size(145, 22);
            this.bookmarkTraceLevelsMenuItem.Text = "Trace Levels";
            this.commandProvider.SetUICommand(this.bookmarkTraceLevelsMenuItem, null);
            this.bookmarkTraceLevelsMenuItem.Click += new System.EventHandler(this.bookmarkTraceLevelsMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(157, 6);
            this.commandProvider.SetUICommand(this.toolStripSeparator4, null);
            // 
            // showCallStackMenuItem
            // 
            this.showCallStackMenuItem.Name = "showCallStackMenuItem";
            this.showCallStackMenuItem.Size = new System.Drawing.Size(160, 22);
            this.showCallStackMenuItem.Text = "View Call Stack...";
            this.commandProvider.SetUICommand(this.showCallStackMenuItem, null);
            this.showCallStackMenuItem.Click += new System.EventHandler(this.showCallStackMenuItem_Click);
            // 
            // viewTextWindowToolStripMenuItem
            // 
            this.viewTextWindowToolStripMenuItem.Name = "viewTextWindowToolStripMenuItem";
            this.viewTextWindowToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.viewTextWindowToolStripMenuItem.Text = "View Text Window...";
            this.commandProvider.SetUICommand(this.viewTextWindowToolStripMenuItem, null);
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
            this.commandProvider.SetUICommand(this.goToToolStripMenuItem, null);
            // 
            // callerMenuItem
            // 
            this.callerMenuItem.Name = "callerMenuItem";
            this.callerMenuItem.Size = new System.Drawing.Size(155, 22);
            this.callerMenuItem.Text = "Caller";
            this.commandProvider.SetUICommand(this.callerMenuItem, null);
            this.callerMenuItem.Click += new System.EventHandler(this.callerMenuItem_Click);
            // 
            // endOfMethodMenuItem
            // 
            this.endOfMethodMenuItem.Name = "endOfMethodMenuItem";
            this.endOfMethodMenuItem.Size = new System.Drawing.Size(155, 22);
            this.endOfMethodMenuItem.Text = "End of method";
            this.commandProvider.SetUICommand(this.endOfMethodMenuItem, null);
            this.endOfMethodMenuItem.Click += new System.EventHandler(this.endOfMethodMenuItem_Click);
            // 
            // setZeroTimeToolStripMenuItem
            // 
            this.setZeroTimeToolStripMenuItem.Name = "setZeroTimeToolStripMenuItem";
            this.setZeroTimeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.setZeroTimeToolStripMenuItem.Text = "Set Zero Time";
            this.commandProvider.SetUICommand(this.setZeroTimeToolStripMenuItem, null);
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
            this.imageList1.Images.SetKeyName(9, "Filter.png");
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
            this.commandProvider.SetUICommand(this.statusStrip1, null);
            // 
            // filenameLabel
            // 
            this.filenameLabel.Name = "filenameLabel";
            this.filenameLabel.Size = new System.Drawing.Size(0, 17);
            this.commandProvider.SetUICommand(this.filenameLabel, null);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.AutoToolTip = true;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.commandProvider.SetUICommand(this.toolStripProgressBar1, null);
            // 
            // pctLabel
            // 
            this.pctLabel.Name = "pctLabel";
            this.pctLabel.Size = new System.Drawing.Size(0, 17);
            this.commandProvider.SetUICommand(this.pctLabel, null);
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
            this.commandProvider.SetUICommand(this.buttonStop, null);
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
            this.commandProvider.SetUICommand(this.menuStrip1, null);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.propertiesToolStripMenuItem,
            this.recentlyViewedToolStripMenuItem,
            this.recentlyCreatedToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.commandProvider.SetUICommand(this.fileToolStripMenuItem, null);
            this.fileToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.RecentMenu_DropDownItemClicked);
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::TracerX.Properties.Resources.OpenFolder;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.commandProvider.SetUICommand(this.openToolStripMenuItem, this.openFileCmd);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.commandProvider.SetUICommand(this.closeToolStripMenuItem, null);
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Enabled = false;
            this.propertiesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("propertiesToolStripMenuItem.Image")));
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.commandProvider.SetUICommand(this.propertiesToolStripMenuItem, this.propertiesCmd);
            // 
            // recentlyViewedToolStripMenuItem
            // 
            this.recentlyViewedToolStripMenuItem.Name = "recentlyViewedToolStripMenuItem";
            this.recentlyViewedToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.recentlyViewedToolStripMenuItem.Text = "Recently Viewed";
            this.commandProvider.SetUICommand(this.recentlyViewedToolStripMenuItem, null);
            this.recentlyViewedToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.RecentMenu_DropDownItemClicked);
            // 
            // recentlyCreatedToolStripMenuItem
            // 
            this.recentlyCreatedToolStripMenuItem.Name = "recentlyCreatedToolStripMenuItem";
            this.recentlyCreatedToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.recentlyCreatedToolStripMenuItem.Text = "Recently Created";
            this.commandProvider.SetUICommand(this.recentlyCreatedToolStripMenuItem, null);
            this.recentlyCreatedToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.RecentMenu_DropDownItemClicked);
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
            this.commandProvider.SetUICommand(this.editToolStripMenuItem, null);
            this.editToolStripMenuItem.DropDownOpening += new System.EventHandler(this.editToolStripMenuItem_DropDownOpening);
            this.editToolStripMenuItem.DropDownClosed += new System.EventHandler(this.editToolStripMenuItem_DropDownClosed);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Enabled = false;
            this.findToolStripMenuItem.Image = global::TracerX.Properties.Resources.find;
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.findToolStripMenuItem.Text = "Find...";
            this.commandProvider.SetUICommand(this.findToolStripMenuItem, this.findCmd);
            // 
            // findNextToolStripMenuItem
            // 
            this.findNextToolStripMenuItem.Enabled = false;
            this.findNextToolStripMenuItem.Image = global::TracerX.Properties.Resources.findNext;
            this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
            this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findNextToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.findNextToolStripMenuItem.Text = "Find Next";
            this.commandProvider.SetUICommand(this.findNextToolStripMenuItem, this.findNextCmd);
            // 
            // findPreviousToolStripMenuItem
            // 
            this.findPreviousToolStripMenuItem.Enabled = false;
            this.findPreviousToolStripMenuItem.Image = global::TracerX.Properties.Resources.findPrev;
            this.findPreviousToolStripMenuItem.Name = "findPreviousToolStripMenuItem";
            this.findPreviousToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.findPreviousToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.findPreviousToolStripMenuItem.Text = "Find Previous";
            this.commandProvider.SetUICommand(this.findPreviousToolStripMenuItem, this.findPrevCmd);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(242, 6);
            this.commandProvider.SetUICommand(this.toolStripSeparator2, null);
            // 
            // bookmarkToggle
            // 
            this.bookmarkToggle.Enabled = false;
            this.bookmarkToggle.Image = global::TracerX.Properties.Resources.BookmarkToggle2;
            this.bookmarkToggle.Name = "bookmarkToggle";
            this.bookmarkToggle.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
            this.bookmarkToggle.Size = new System.Drawing.Size(245, 22);
            this.bookmarkToggle.Text = "Toggle Bookmark";
            this.commandProvider.SetUICommand(this.bookmarkToggle, this.bookmarkToggleCmd);
            // 
            // nextBookmarkToolStripMenuItem
            // 
            this.nextBookmarkToolStripMenuItem.Enabled = false;
            this.nextBookmarkToolStripMenuItem.Image = global::TracerX.Properties.Resources.BookmarkNext;
            this.nextBookmarkToolStripMenuItem.Name = "nextBookmarkToolStripMenuItem";
            this.nextBookmarkToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.nextBookmarkToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.nextBookmarkToolStripMenuItem.Text = "Next Bookmark";
            this.commandProvider.SetUICommand(this.nextBookmarkToolStripMenuItem, this.bookmarkNextCmd);
            // 
            // previousBookmarkToolStripMenuItem
            // 
            this.previousBookmarkToolStripMenuItem.Enabled = false;
            this.previousBookmarkToolStripMenuItem.Image = global::TracerX.Properties.Resources.BookmarkPrev;
            this.previousBookmarkToolStripMenuItem.Name = "previousBookmarkToolStripMenuItem";
            this.previousBookmarkToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
            this.previousBookmarkToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.previousBookmarkToolStripMenuItem.Text = "Previous Bookmark";
            this.commandProvider.SetUICommand(this.previousBookmarkToolStripMenuItem, this.bookmarkPrevCmd);
            // 
            // clearAllBookmarksToolStripMenuItem
            // 
            this.clearAllBookmarksToolStripMenuItem.Enabled = false;
            this.clearAllBookmarksToolStripMenuItem.Image = global::TracerX.Properties.Resources.BookmarkClear;
            this.clearAllBookmarksToolStripMenuItem.Name = "clearAllBookmarksToolStripMenuItem";
            this.clearAllBookmarksToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.clearAllBookmarksToolStripMenuItem.Text = "Clear All Bookmarks";
            this.commandProvider.SetUICommand(this.clearAllBookmarksToolStripMenuItem, this.bookmarkClearCmd);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(242, 6);
            this.commandProvider.SetUICommand(this.toolStripSeparator3, null);
            // 
            // copyTextMenuItem
            // 
            this.copyTextMenuItem.Name = "copyTextMenuItem";
            this.copyTextMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyTextMenuItem.Size = new System.Drawing.Size(245, 22);
            this.copyTextMenuItem.Text = "Copy Text";
            this.commandProvider.SetUICommand(this.copyTextMenuItem, null);
            this.copyTextMenuItem.Click += new System.EventHandler(this.copyTextMenuItem_Click);
            // 
            // copyColsMenuItem
            // 
            this.copyColsMenuItem.Name = "copyColsMenuItem";
            this.copyColsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
                        | System.Windows.Forms.Keys.C)));
            this.copyColsMenuItem.Size = new System.Drawing.Size(245, 22);
            this.copyColsMenuItem.Text = "Copy Visible Columns";
            this.commandProvider.SetUICommand(this.copyColsMenuItem, null);
            this.copyColsMenuItem.Click += new System.EventHandler(this.copyColsMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterToolStripMenuItem,
            this.clearFilterMenuItem,
            this.columnsToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.refreshMenuItem,
            this.closeAllWindowsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "View";
            this.commandProvider.SetUICommand(this.viewToolStripMenuItem, null);
            this.viewToolStripMenuItem.DropDownOpening += new System.EventHandler(this.viewToolStripMenuItem_DropDownOpening);
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.Enabled = false;
            this.filterToolStripMenuItem.Image = global::TracerX.Properties.Resources.Filter1;
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.filterToolStripMenuItem.Text = "Filter...";
            this.commandProvider.SetUICommand(this.filterToolStripMenuItem, this.filterDlgCmd);
            // 
            // clearFilterMenuItem
            // 
            this.clearFilterMenuItem.Enabled = false;
            this.clearFilterMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("clearFilterMenuItem.Image")));
            this.clearFilterMenuItem.Name = "clearFilterMenuItem";
            this.clearFilterMenuItem.Size = new System.Drawing.Size(171, 22);
            this.clearFilterMenuItem.Text = "Clear All Filtering";
            this.commandProvider.SetUICommand(this.clearFilterMenuItem, this.filterClearCmd);
            // 
            // columnsToolStripMenuItem
            // 
            this.columnsToolStripMenuItem.Image = global::TracerX.Properties.Resources.Columns;
            this.columnsToolStripMenuItem.Name = "columnsToolStripMenuItem";
            this.columnsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.columnsToolStripMenuItem.Text = "Columns...";
            this.commandProvider.SetUICommand(this.columnsToolStripMenuItem, this.columnsCmd);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Image = global::TracerX.Properties.Resources.Options;
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.optionsToolStripMenuItem.Text = "Options...";
            this.commandProvider.SetUICommand(this.optionsToolStripMenuItem, this.optionsCmd);
            // 
            // refreshMenuItem
            // 
            this.refreshMenuItem.Enabled = false;
            this.refreshMenuItem.Image = global::TracerX.Properties.Resources.Refresh;
            this.refreshMenuItem.Name = "refreshMenuItem";
            this.refreshMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshMenuItem.Size = new System.Drawing.Size(171, 22);
            this.refreshMenuItem.Text = "Refresh";
            this.commandProvider.SetUICommand(this.refreshMenuItem, this.refreshCmd);
            // 
            // closeAllWindowsToolStripMenuItem
            // 
            this.closeAllWindowsToolStripMenuItem.Name = "closeAllWindowsToolStripMenuItem";
            this.closeAllWindowsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.closeAllWindowsToolStripMenuItem.Text = "Close All Windows";
            this.commandProvider.SetUICommand(this.closeAllWindowsToolStripMenuItem, null);
            this.closeAllWindowsToolStripMenuItem.Click += new System.EventHandler(this.closeAllWindowsToolStripMenuItem_Click);
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
            this.commandProvider.SetUICommand(this.helpToolStripMenuItem, null);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.commandProvider.SetUICommand(this.aboutToolStripMenuItem, null);
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // licenseToolStripMenuItem
            // 
            this.licenseToolStripMenuItem.Name = "licenseToolStripMenuItem";
            this.licenseToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.licenseToolStripMenuItem.Text = "License...";
            this.commandProvider.SetUICommand(this.licenseToolStripMenuItem, null);
            this.licenseToolStripMenuItem.Click += new System.EventHandler(this.licenseToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileButton,
            this.propertiesButton,
            this.toolStripSeparator11,
            this.openFilters,
            this.clearFilterButton,
            this.toolStripSeparator6,
            this.refreshButton,
            this.startAutoRefresh,
            this.stopAutoRefresh,
            this.toolStripSeparator7,
            this.bookmarkToggleButton,
            this.bookmarkPrev,
            this.bookmarkNext,
            this.bookmarkClear,
            this.toolStripSeparator8,
            this.findButton,
            this.findPrevButton,
            this.findNextButton,
            this.toolStripSeparator10,
            this.optionsButton,
            this.columnsButton,
            this.toolStripSeparator12,
            this.expandAllButton,
            this.relativeTimeButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(886, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            this.commandProvider.SetUICommand(this.toolStrip1, null);
            // 
            // openFileButton
            // 
            this.openFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openFileButton.Image = global::TracerX.Properties.Resources.OpenFolder;
            this.openFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openFileButton.Name = "openFileButton";
            this.openFileButton.Size = new System.Drawing.Size(23, 22);
            this.openFileButton.Text = "Open file";
            this.commandProvider.SetUICommand(this.openFileButton, this.openFileCmd);
            // 
            // propertiesButton
            // 
            this.propertiesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.propertiesButton.Enabled = false;
            this.propertiesButton.Image = ((System.Drawing.Image)(resources.GetObject("propertiesButton.Image")));
            this.propertiesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.propertiesButton.Name = "propertiesButton";
            this.propertiesButton.Size = new System.Drawing.Size(23, 22);
            this.propertiesButton.Text = "File properties";
            this.commandProvider.SetUICommand(this.propertiesButton, this.propertiesCmd);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 25);
            this.commandProvider.SetUICommand(this.toolStripSeparator11, null);
            // 
            // openFilters
            // 
            this.openFilters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openFilters.Enabled = false;
            this.openFilters.Image = global::TracerX.Properties.Resources.Filter1;
            this.openFilters.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openFilters.Name = "openFilters";
            this.openFilters.Size = new System.Drawing.Size(23, 22);
            this.openFilters.Text = "Open filter dialog";
            this.commandProvider.SetUICommand(this.openFilters, this.filterDlgCmd);
            // 
            // clearFilterButton
            // 
            this.clearFilterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearFilterButton.Enabled = false;
            this.clearFilterButton.Image = ((System.Drawing.Image)(resources.GetObject("clearFilterButton.Image")));
            this.clearFilterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearFilterButton.Name = "clearFilterButton";
            this.clearFilterButton.Size = new System.Drawing.Size(23, 22);
            this.clearFilterButton.Text = "toolStripButton1";
            this.clearFilterButton.ToolTipText = "Clear all filtering";
            this.commandProvider.SetUICommand(this.clearFilterButton, this.filterClearCmd);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            this.commandProvider.SetUICommand(this.toolStripSeparator6, null);
            // 
            // refreshButton
            // 
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.Enabled = false;
            this.refreshButton.Image = global::TracerX.Properties.Resources.Refresh;
            this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(23, 22);
            this.refreshButton.Text = "Refresh";
            this.commandProvider.SetUICommand(this.refreshButton, this.refreshCmd);
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
            this.commandProvider.SetUICommand(this.startAutoRefresh, null);
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
            this.commandProvider.SetUICommand(this.stopAutoRefresh, null);
            this.stopAutoRefresh.Click += new System.EventHandler(this.stopAutoRefresh_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            this.commandProvider.SetUICommand(this.toolStripSeparator7, null);
            // 
            // bookmarkToggleButton
            // 
            this.bookmarkToggleButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bookmarkToggleButton.Enabled = false;
            this.bookmarkToggleButton.Image = global::TracerX.Properties.Resources.BookmarkToggle2;
            this.bookmarkToggleButton.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.bookmarkToggleButton.Name = "bookmarkToggleButton";
            this.bookmarkToggleButton.Size = new System.Drawing.Size(23, 22);
            this.bookmarkToggleButton.Text = "toolStripButton1";
            this.bookmarkToggleButton.ToolTipText = "Toggle bookmark";
            this.commandProvider.SetUICommand(this.bookmarkToggleButton, this.bookmarkToggleCmd);
            // 
            // bookmarkPrev
            // 
            this.bookmarkPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bookmarkPrev.Enabled = false;
            this.bookmarkPrev.Image = global::TracerX.Properties.Resources.BookmarkPrev;
            this.bookmarkPrev.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.bookmarkPrev.Name = "bookmarkPrev";
            this.bookmarkPrev.Size = new System.Drawing.Size(23, 22);
            this.bookmarkPrev.Text = "toolStripButton1";
            this.bookmarkPrev.ToolTipText = "Previous bookmark";
            this.commandProvider.SetUICommand(this.bookmarkPrev, this.bookmarkPrevCmd);
            // 
            // bookmarkNext
            // 
            this.bookmarkNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bookmarkNext.Enabled = false;
            this.bookmarkNext.Image = global::TracerX.Properties.Resources.BookmarkNext;
            this.bookmarkNext.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.bookmarkNext.Name = "bookmarkNext";
            this.bookmarkNext.Size = new System.Drawing.Size(23, 22);
            this.bookmarkNext.Text = "toolStripButton1";
            this.bookmarkNext.ToolTipText = "Next bookmark";
            this.commandProvider.SetUICommand(this.bookmarkNext, this.bookmarkNextCmd);
            // 
            // bookmarkClear
            // 
            this.bookmarkClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bookmarkClear.Enabled = false;
            this.bookmarkClear.Image = global::TracerX.Properties.Resources.BookmarkClear;
            this.bookmarkClear.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.bookmarkClear.Name = "bookmarkClear";
            this.bookmarkClear.Size = new System.Drawing.Size(23, 22);
            this.bookmarkClear.Text = "toolStripButton1";
            this.bookmarkClear.ToolTipText = "Clear all bookmarks";
            this.commandProvider.SetUICommand(this.bookmarkClear, this.bookmarkClearCmd);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 25);
            this.commandProvider.SetUICommand(this.toolStripSeparator8, null);
            // 
            // findButton
            // 
            this.findButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.findButton.Enabled = false;
            this.findButton.Image = global::TracerX.Properties.Resources.find;
            this.findButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.findButton.Name = "findButton";
            this.findButton.Size = new System.Drawing.Size(23, 22);
            this.findButton.Text = "toolStripButton1";
            this.findButton.ToolTipText = "Find";
            this.commandProvider.SetUICommand(this.findButton, this.findCmd);
            // 
            // findPrevButton
            // 
            this.findPrevButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.findPrevButton.Enabled = false;
            this.findPrevButton.Image = global::TracerX.Properties.Resources.findPrev;
            this.findPrevButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.findPrevButton.Name = "findPrevButton";
            this.findPrevButton.Size = new System.Drawing.Size(23, 22);
            this.findPrevButton.Text = "toolStripButton1";
            this.findPrevButton.ToolTipText = "Find previous";
            this.commandProvider.SetUICommand(this.findPrevButton, this.findPrevCmd);
            // 
            // findNextButton
            // 
            this.findNextButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.findNextButton.Enabled = false;
            this.findNextButton.Image = global::TracerX.Properties.Resources.findNext;
            this.findNextButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new System.Drawing.Size(23, 22);
            this.findNextButton.Text = "toolStripButton2";
            this.findNextButton.ToolTipText = "Find next";
            this.commandProvider.SetUICommand(this.findNextButton, this.findNextCmd);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 25);
            this.commandProvider.SetUICommand(this.toolStripSeparator10, null);
            // 
            // optionsButton
            // 
            this.optionsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.optionsButton.Image = global::TracerX.Properties.Resources.Options;
            this.optionsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.optionsButton.Name = "optionsButton";
            this.optionsButton.Size = new System.Drawing.Size(23, 22);
            this.optionsButton.Text = "Options";
            this.commandProvider.SetUICommand(this.optionsButton, this.optionsCmd);
            // 
            // columnsButton
            // 
            this.columnsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.columnsButton.Image = global::TracerX.Properties.Resources.Columns;
            this.columnsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.columnsButton.Name = "columnsButton";
            this.columnsButton.Size = new System.Drawing.Size(23, 22);
            this.columnsButton.Text = "Columns";
            this.commandProvider.SetUICommand(this.columnsButton, this.columnsCmd);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 25);
            this.commandProvider.SetUICommand(this.toolStripSeparator12, null);
            // 
            // expandAllButton
            // 
            this.expandAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.expandAllButton.Image = ((System.Drawing.Image)(resources.GetObject("expandAllButton.Image")));
            this.expandAllButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.expandAllButton.Name = "expandAllButton";
            this.expandAllButton.Size = new System.Drawing.Size(23, 22);
            this.expandAllButton.Text = "Expand all";
            this.commandProvider.SetUICommand(this.expandAllButton, null);
            this.expandAllButton.Click += new System.EventHandler(this.expandAllButton_Click);
            // 
            // relativeTimeButton
            // 
            this.relativeTimeButton.CheckOnClick = true;
            this.relativeTimeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.relativeTimeButton.Image = global::TracerX.Properties.Resources.Stopwatch;
            this.relativeTimeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.relativeTimeButton.Name = "relativeTimeButton";
            this.relativeTimeButton.Size = new System.Drawing.Size(23, 22);
            this.relativeTimeButton.Text = "Show relative times";
            this.commandProvider.SetUICommand(this.relativeTimeButton, null);
            this.relativeTimeButton.Click += new System.EventHandler(this.relativeTimeButton_Click);
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
            this.commandProvider.SetUICommand(this.columnContextMenu, null);
            // 
            // colMenuFilterItem
            // 
            this.colMenuFilterItem.Name = "colMenuFilterItem";
            this.colMenuFilterItem.Size = new System.Drawing.Size(151, 22);
            this.colMenuFilterItem.Text = "Filter...";
            this.commandProvider.SetUICommand(this.colMenuFilterItem, null);
            this.colMenuFilterItem.Click += new System.EventHandler(this.colMenuFilterItem_Click);
            // 
            // colMenuRemoveItem
            // 
            this.colMenuRemoveItem.Name = "colMenuRemoveItem";
            this.colMenuRemoveItem.Size = new System.Drawing.Size(151, 22);
            this.colMenuRemoveItem.Text = "Remove from Filter";
            this.commandProvider.SetUICommand(this.colMenuRemoveItem, null);
            this.colMenuRemoveItem.Click += new System.EventHandler(this.colMenuRemoveItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(148, 6);
            this.commandProvider.SetUICommand(this.toolStripSeparator5, null);
            // 
            // colMenuOptionsItem
            // 
            this.colMenuOptionsItem.Name = "colMenuOptionsItem";
            this.colMenuOptionsItem.Size = new System.Drawing.Size(151, 22);
            this.colMenuOptionsItem.Text = "Options...";
            this.commandProvider.SetUICommand(this.colMenuOptionsItem, null);
            this.colMenuOptionsItem.Click += new System.EventHandler(this.colMenuOptionsItem_Click);
            // 
            // colMenuColumnItem
            // 
            this.colMenuColumnItem.Name = "colMenuColumnItem";
            this.colMenuColumnItem.Size = new System.Drawing.Size(151, 22);
            this.colMenuColumnItem.Text = "Columns...";
            this.commandProvider.SetUICommand(this.colMenuColumnItem, null);
            this.colMenuColumnItem.Click += new System.EventHandler(this.ExecuteColumns);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            this.commandProvider.SetUICommand(this.toolStripSeparator9, null);
            // 
            // openFileCmd
            // 
            this.openFileCmd.Enabled = true;
            this.openFileCmd.Execute += new System.EventHandler(this.ExecuteOpenFile);
            // 
            // propertiesCmd
            // 
            this.propertiesCmd.Enabled = false;
            this.propertiesCmd.Execute += new System.EventHandler(this.ExecuteProperties);
            // 
            // findCmd
            // 
            this.findCmd.Enabled = false;
            this.findCmd.Execute += new System.EventHandler(this.ExecuteFind);
            // 
            // findNextCmd
            // 
            this.findNextCmd.Enabled = false;
            this.findNextCmd.Execute += new System.EventHandler(this.ExecuteFindNext);
            // 
            // findPrevCmd
            // 
            this.findPrevCmd.Enabled = false;
            this.findPrevCmd.Execute += new System.EventHandler(this.ExecuteFindPrevious);
            // 
            // bookmarkToggleCmd
            // 
            this.bookmarkToggleCmd.Enabled = false;
            this.bookmarkToggleCmd.Execute += new System.EventHandler(this.ExecuteToggleBookmark);
            // 
            // bookmarkNextCmd
            // 
            this.bookmarkNextCmd.Enabled = false;
            this.bookmarkNextCmd.Execute += new System.EventHandler(this.ExecuteNextBookmark);
            // 
            // bookmarkPrevCmd
            // 
            this.bookmarkPrevCmd.Enabled = false;
            this.bookmarkPrevCmd.Execute += new System.EventHandler(this.ExecutePrevBookmark);
            // 
            // bookmarkClearCmd
            // 
            this.bookmarkClearCmd.Enabled = false;
            this.bookmarkClearCmd.Execute += new System.EventHandler(this.ExecuteClearBookmarks);
            // 
            // filterDlgCmd
            // 
            this.filterDlgCmd.Enabled = false;
            this.filterDlgCmd.Execute += new System.EventHandler(this.ExecuteOpenFilterDialog);
            // 
            // filterClearCmd
            // 
            this.filterClearCmd.Enabled = false;
            this.filterClearCmd.Execute += new System.EventHandler(this.ExecuteClearFilter);
            // 
            // columnsCmd
            // 
            this.columnsCmd.Enabled = true;
            this.columnsCmd.Execute += new System.EventHandler(this.ExecuteColumns);
            // 
            // optionsCmd
            // 
            this.optionsCmd.Enabled = true;
            this.optionsCmd.Execute += new System.EventHandler(this.ExecuteOptions);
            // 
            // refreshCmd
            // 
            this.refreshCmd.Enabled = false;
            this.refreshCmd.Execute += new System.EventHandler(this.ExecuteRefresh);
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
            this.commandProvider.SetUICommand(this, null);
            this.Activated += new System.EventHandler(this.MainForm_Activated);
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
        private System.Windows.Forms.ToolStripButton clearFilterButton;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearFilterMenuItem;
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
        private System.Windows.Forms.ToolStripButton bookmarkToggleButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton bookmarkPrev;
        private System.Windows.Forms.ToolStripButton bookmarkNext;
        private System.Windows.Forms.ToolStripButton bookmarkClear;
        private Commander.UICommandProvider commandProvider;
        private Commander.UICommand filterDlgCmd;
        private Commander.UICommand filterClearCmd;
        private Commander.UICommand bookmarkToggleCmd;
        private Commander.UICommand bookmarkClearCmd;
        private Commander.UICommand bookmarkPrevCmd;
        private Commander.UICommand bookmarkNextCmd;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripButton findButton;
        private Commander.UICommand findCmd;
        private Commander.UICommand findPrevCmd;
        private System.Windows.Forms.ToolStripButton findPrevButton;
        private Commander.UICommand findNextCmd;
        private System.Windows.Forms.ToolStripButton findNextButton;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private Commander.UICommand refreshCmd;
        private Commander.UICommand optionsCmd;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripButton optionsButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripButton columnsButton;
        private Commander.UICommand columnsCmd;
        private System.Windows.Forms.ToolStripButton openFileButton;
        private Commander.UICommand openFileCmd;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripButton propertiesButton;
        private Commander.UICommand propertiesCmd;
        private System.Windows.Forms.ToolStripButton expandAllButton;
        private System.Windows.Forms.ToolStripButton relativeTimeButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem recentlyViewedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentlyCreatedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllWindowsToolStripMenuItem;
    }
}
