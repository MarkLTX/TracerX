namespace TracerX {
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHideSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShowSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBookmarkSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuColorSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.uncolorSelectedMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.showCallStackMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTextWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startOfMethodMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.endOfMethodMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setZeroTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.serverLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.filenameLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnCancel = new System.Windows.Forms.ToolStripDropDownButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.clearColumnColorsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.coloringMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableColoringMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.columnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.showTracerXLogsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showServerPickerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commandLineMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.relatedFilesFoldersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.openFileButton = new System.Windows.Forms.ToolStripButton();
            this.propertiesButton = new System.Windows.Forms.ToolStripButton();
            this.closeBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.openFilters = new System.Windows.Forms.ToolStripButton();
            this.clearFilterButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.autoUpdate = new System.Windows.Forms.ToolStripButton();
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
            this.prevAnyThreadBtn = new System.Windows.Forms.ToolStripButton();
            this.nextAnyThreadBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.prevSameThreadBtn = new System.Windows.Forms.ToolStripButton();
            this.nextSameThreadBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.optionsButton = new System.Windows.Forms.ToolStripButton();
            this.columnsButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.dupTimeButton = new System.Windows.Forms.ToolStripButton();
            this.relativeTimeButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearColumnColorsBtn = new System.Windows.Forms.ToolStripButton();
            this.enableColorsBtn = new System.Windows.Forms.ToolStripButton();
            this.editColorsBtn = new System.Windows.Forms.ToolStripButton();
            this.boldBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.expandAllButton = new System.Windows.Forms.ToolStripButton();
            this.collapseAllButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.prevTimeButton = new System.Windows.Forms.ToolStripButton();
            this.nextTimeButton = new System.Windows.Forms.ToolStripButton();
            this.timeUnitCombo = new System.Windows.Forms.ToolStripComboBox();
            this.columnContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.colMenuFilterItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colMenuRemoveItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.colMenuOptionsItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colMenuColumnItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colMenuHideItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.theStartPage = new TracerX.NewStartPage();
            this.TheListView = new TracerX.ListViewTx();
            this.headerText = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerSession = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerLine = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerLevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerLogger = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerThreadId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerThreadName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerMethod = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.crumbBar1 = new TracerX.CrumbBar();
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
            this.clearColumnColors = new Commander.UICommand(this.components);
            this.editColors = new Commander.UICommand(this.components);
            this.enableColors = new Commander.UICommand(this.components);
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
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator4,
            this.mnuHideSelected,
            this.mnuShowSelected,
            this.mnuBookmarkSelected,
            this.mnuColorSelected,
            this.uncolorSelectedMenu,
            this.toolStripSeparator16,
            this.showCallStackMenuItem,
            this.viewTextWindowToolStripMenuItem,
            this.goToToolStripMenuItem,
            this.setZeroTimeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(168, 214);
            this.commandProvider.SetUICommand(this.contextMenuStrip1, null);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(164, 6);
            this.commandProvider.SetUICommand(this.toolStripSeparator4, null);
            // 
            // mnuHideSelected
            // 
            this.mnuHideSelected.Name = "mnuHideSelected";
            this.mnuHideSelected.Size = new System.Drawing.Size(167, 22);
            this.mnuHideSelected.Text = "Hide Selected";
            this.commandProvider.SetUICommand(this.mnuHideSelected, null);
            this.mnuHideSelected.Click += new System.EventHandler(this.mnuHideSelected_Click);
            // 
            // mnuShowSelected
            // 
            this.mnuShowSelected.Name = "mnuShowSelected";
            this.mnuShowSelected.Size = new System.Drawing.Size(167, 22);
            this.mnuShowSelected.Text = "Show Only Selected";
            this.commandProvider.SetUICommand(this.mnuShowSelected, null);
            this.mnuShowSelected.Click += new System.EventHandler(this.mnuShowSelected_Click);
            // 
            // mnuBookmarkSelected
            // 
            this.mnuBookmarkSelected.Name = "mnuBookmarkSelected";
            this.mnuBookmarkSelected.Size = new System.Drawing.Size(167, 22);
            this.mnuBookmarkSelected.Text = "Bookmark Selected";
            this.commandProvider.SetUICommand(this.mnuBookmarkSelected, null);
            this.mnuBookmarkSelected.Click += new System.EventHandler(this.mnuBookmarkSelected_Click);
            // 
            // mnuColorSelected
            // 
            this.mnuColorSelected.Name = "mnuColorSelected";
            this.mnuColorSelected.Size = new System.Drawing.Size(167, 22);
            this.mnuColorSelected.Text = "Color Selected";
            this.commandProvider.SetUICommand(this.mnuColorSelected, null);
            this.mnuColorSelected.Click += new System.EventHandler(this.mnuColorSelected_Click);
            // 
            // uncolorSelectedMenu
            // 
            this.uncolorSelectedMenu.Name = "uncolorSelectedMenu";
            this.uncolorSelectedMenu.Size = new System.Drawing.Size(167, 22);
            this.uncolorSelectedMenu.Text = "Uncolor Selected";
            this.commandProvider.SetUICommand(this.uncolorSelectedMenu, null);
            this.uncolorSelectedMenu.Click += new System.EventHandler(this.uncolorSelectedMenu_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(164, 6);
            this.commandProvider.SetUICommand(this.toolStripSeparator16, null);
            // 
            // showCallStackMenuItem
            // 
            this.showCallStackMenuItem.Name = "showCallStackMenuItem";
            this.showCallStackMenuItem.Size = new System.Drawing.Size(167, 22);
            this.showCallStackMenuItem.Text = "View Call Stack...";
            this.commandProvider.SetUICommand(this.showCallStackMenuItem, null);
            this.showCallStackMenuItem.Click += new System.EventHandler(this.showCallStackMenuItem_Click);
            // 
            // viewTextWindowToolStripMenuItem
            // 
            this.viewTextWindowToolStripMenuItem.Name = "viewTextWindowToolStripMenuItem";
            this.viewTextWindowToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.viewTextWindowToolStripMenuItem.Text = "View in Text Window...";
            this.commandProvider.SetUICommand(this.viewTextWindowToolStripMenuItem, null);
            this.viewTextWindowToolStripMenuItem.Click += new System.EventHandler(this.viewTextWindowToolStripMenuItem_Click);
            // 
            // goToToolStripMenuItem
            // 
            this.goToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startOfMethodMenuItem,
            this.endOfMethodMenuItem});
            this.goToToolStripMenuItem.Name = "goToToolStripMenuItem";
            this.goToToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.goToToolStripMenuItem.Text = "Go To";
            this.commandProvider.SetUICommand(this.goToToolStripMenuItem, null);
            // 
            // startOfMethodMenuItem
            // 
            this.startOfMethodMenuItem.Name = "startOfMethodMenuItem";
            this.startOfMethodMenuItem.Size = new System.Drawing.Size(153, 22);
            this.startOfMethodMenuItem.Text = "Caller";
            this.commandProvider.SetUICommand(this.startOfMethodMenuItem, null);
            this.startOfMethodMenuItem.Click += new System.EventHandler(this.startOfMethodMenuItem_Click);
            // 
            // endOfMethodMenuItem
            // 
            this.endOfMethodMenuItem.Name = "endOfMethodMenuItem";
            this.endOfMethodMenuItem.Size = new System.Drawing.Size(153, 22);
            this.endOfMethodMenuItem.Text = "End of method";
            this.commandProvider.SetUICommand(this.endOfMethodMenuItem, null);
            this.endOfMethodMenuItem.Click += new System.EventHandler(this.endOfMethodMenuItem_Click);
            // 
            // setZeroTimeToolStripMenuItem
            // 
            this.setZeroTimeToolStripMenuItem.Name = "setZeroTimeToolStripMenuItem";
            this.setZeroTimeToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.setZeroTimeToolStripMenuItem.Text = "Set Zero Time";
            this.commandProvider.SetUICommand(this.setZeroTimeToolStripMenuItem, null);
            this.setZeroTimeToolStripMenuItem.Click += new System.EventHandler(this.setZeroTimeToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Bookmark.ico");
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
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverLabel,
            this.filenameLabel,
            this.statusMsg,
            this.btnCancel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 642);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(886, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            this.commandProvider.SetUICommand(this.statusStrip1, null);
            // 
            // serverLabel
            // 
            this.serverLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(4, 17);
            this.commandProvider.SetUICommand(this.serverLabel, null);
            // 
            // filenameLabel
            // 
            this.filenameLabel.Name = "filenameLabel";
            this.filenameLabel.Size = new System.Drawing.Size(816, 17);
            this.filenameLabel.Spring = true;
            this.filenameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.commandProvider.SetUICommand(this.filenameLabel, null);
            this.filenameLabel.VisibleChanged += new System.EventHandler(this.filenameLabel_VisibleChanged);
            // 
            // statusMsg
            // 
            this.statusMsg.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.statusMsg.Name = "statusMsg";
            this.statusMsg.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.statusMsg.Size = new System.Drawing.Size(4, 17);
            this.commandProvider.SetUICommand(this.statusMsg, null);
            this.statusMsg.VisibleChanged += new System.EventHandler(this.statusMsg_VisibleChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Orange;
            this.btnCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.ShowDropDownArrow = false;
            this.btnCancel.Size = new System.Drawing.Size(47, 20);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.ToolTipText = "Stop loading file";
            this.commandProvider.SetUICommand(this.btnCancel, null);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            this.newWindowToolStripMenuItem,
            this.exportToCSVToolStripMenuItem,
            this.propertiesToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.commandProvider.SetUICommand(this.fileToolStripMenuItem, null);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::TracerX.Properties.Resources.OpenFolder;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.ToolTipText = "Open file";
            this.commandProvider.SetUICommand(this.openToolStripMenuItem, this.openFileCmd);
            // 
            // newWindowToolStripMenuItem
            // 
            this.newWindowToolStripMenuItem.Name = "newWindowToolStripMenuItem";
            this.newWindowToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.newWindowToolStripMenuItem.Text = "New Window";
            this.commandProvider.SetUICommand(this.newWindowToolStripMenuItem, null);
            this.newWindowToolStripMenuItem.Click += new System.EventHandler(this.newWindowToolStripMenuItem_Click);
            // 
            // exportToCSVToolStripMenuItem
            // 
            this.exportToCSVToolStripMenuItem.Name = "exportToCSVToolStripMenuItem";
            this.exportToCSVToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.exportToCSVToolStripMenuItem.Text = "Export to CSV...";
            this.commandProvider.SetUICommand(this.exportToCSVToolStripMenuItem, null);
            this.exportToCSVToolStripMenuItem.Click += new System.EventHandler(this.exportToCSVToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Enabled = false;
            this.propertiesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("propertiesToolStripMenuItem.Image")));
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.propertiesToolStripMenuItem.Text = "Properties...";
            this.propertiesToolStripMenuItem.ToolTipText = "File properties";
            this.commandProvider.SetUICommand(this.propertiesToolStripMenuItem, this.propertiesCmd);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = global::TracerX.Properties.Resources.Close_X_Red_16;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.closeToolStripMenuItem.Text = "Exit";
            this.commandProvider.SetUICommand(this.closeToolStripMenuItem, null);
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
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
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            this.commandProvider.SetUICommand(this.editToolStripMenuItem, null);
            this.editToolStripMenuItem.DropDownClosed += new System.EventHandler(this.editToolStripMenuItem_DropDownClosed);
            this.editToolStripMenuItem.DropDownOpening += new System.EventHandler(this.editToolStripMenuItem_DropDownOpening);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Enabled = false;
            this.findToolStripMenuItem.Image = global::TracerX.Properties.Resources.find;
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.findToolStripMenuItem.Text = "Find...";
            this.findToolStripMenuItem.ToolTipText = "Find";
            this.commandProvider.SetUICommand(this.findToolStripMenuItem, this.findCmd);
            // 
            // findNextToolStripMenuItem
            // 
            this.findNextToolStripMenuItem.Enabled = false;
            this.findNextToolStripMenuItem.Image = global::TracerX.Properties.Resources.findNext;
            this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
            this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findNextToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.findNextToolStripMenuItem.Text = "Find Next";
            this.findNextToolStripMenuItem.ToolTipText = "Find next";
            this.commandProvider.SetUICommand(this.findNextToolStripMenuItem, this.findNextCmd);
            // 
            // findPreviousToolStripMenuItem
            // 
            this.findPreviousToolStripMenuItem.Enabled = false;
            this.findPreviousToolStripMenuItem.Image = global::TracerX.Properties.Resources.findPrev;
            this.findPreviousToolStripMenuItem.Name = "findPreviousToolStripMenuItem";
            this.findPreviousToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.findPreviousToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.findPreviousToolStripMenuItem.Text = "Find Previous";
            this.findPreviousToolStripMenuItem.ToolTipText = "Find previous";
            this.commandProvider.SetUICommand(this.findPreviousToolStripMenuItem, this.findPrevCmd);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(252, 6);
            this.commandProvider.SetUICommand(this.toolStripSeparator2, null);
            // 
            // bookmarkToggle
            // 
            this.bookmarkToggle.Enabled = false;
            this.bookmarkToggle.Image = global::TracerX.Properties.Resources.BookmarkToggle2;
            this.bookmarkToggle.Name = "bookmarkToggle";
            this.bookmarkToggle.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
            this.bookmarkToggle.Size = new System.Drawing.Size(255, 22);
            this.bookmarkToggle.Text = "Toggle Bookmark";
            this.bookmarkToggle.ToolTipText = "Toggle bookmark";
            this.commandProvider.SetUICommand(this.bookmarkToggle, this.bookmarkToggleCmd);
            // 
            // nextBookmarkToolStripMenuItem
            // 
            this.nextBookmarkToolStripMenuItem.Enabled = false;
            this.nextBookmarkToolStripMenuItem.Image = global::TracerX.Properties.Resources.BookmarkNext;
            this.nextBookmarkToolStripMenuItem.Name = "nextBookmarkToolStripMenuItem";
            this.nextBookmarkToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.nextBookmarkToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.nextBookmarkToolStripMenuItem.Text = "Next Bookmark";
            this.nextBookmarkToolStripMenuItem.ToolTipText = "Next bookmark";
            this.commandProvider.SetUICommand(this.nextBookmarkToolStripMenuItem, this.bookmarkNextCmd);
            // 
            // previousBookmarkToolStripMenuItem
            // 
            this.previousBookmarkToolStripMenuItem.Enabled = false;
            this.previousBookmarkToolStripMenuItem.Image = global::TracerX.Properties.Resources.BookmarkPrev;
            this.previousBookmarkToolStripMenuItem.Name = "previousBookmarkToolStripMenuItem";
            this.previousBookmarkToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
            this.previousBookmarkToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.previousBookmarkToolStripMenuItem.Text = "Previous Bookmark";
            this.previousBookmarkToolStripMenuItem.ToolTipText = "Previous bookmark";
            this.commandProvider.SetUICommand(this.previousBookmarkToolStripMenuItem, this.bookmarkPrevCmd);
            // 
            // clearAllBookmarksToolStripMenuItem
            // 
            this.clearAllBookmarksToolStripMenuItem.Enabled = false;
            this.clearAllBookmarksToolStripMenuItem.Image = global::TracerX.Properties.Resources.BookmarkClear;
            this.clearAllBookmarksToolStripMenuItem.Name = "clearAllBookmarksToolStripMenuItem";
            this.clearAllBookmarksToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.clearAllBookmarksToolStripMenuItem.Text = "Clear All Bookmarks";
            this.clearAllBookmarksToolStripMenuItem.ToolTipText = "Clear all bookmarks";
            this.commandProvider.SetUICommand(this.clearAllBookmarksToolStripMenuItem, this.bookmarkClearCmd);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(252, 6);
            this.commandProvider.SetUICommand(this.toolStripSeparator3, null);
            // 
            // copyTextMenuItem
            // 
            this.copyTextMenuItem.Name = "copyTextMenuItem";
            this.copyTextMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyTextMenuItem.Size = new System.Drawing.Size(255, 22);
            this.copyTextMenuItem.Text = "Copy Text";
            this.commandProvider.SetUICommand(this.copyTextMenuItem, null);
            this.copyTextMenuItem.Click += new System.EventHandler(this.copyTextMenuItem_Click);
            // 
            // copyColsMenuItem
            // 
            this.copyColsMenuItem.Name = "copyColsMenuItem";
            this.copyColsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.C)));
            this.copyColsMenuItem.Size = new System.Drawing.Size(255, 22);
            this.copyColsMenuItem.Text = "Copy Visible Columns";
            this.commandProvider.SetUICommand(this.copyColsMenuItem, null);
            this.copyColsMenuItem.Click += new System.EventHandler(this.copyColsMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterToolStripMenuItem,
            this.clearFilterMenuItem,
            this.clearColumnColorsMenuItem,
            this.coloringMenuItem,
            this.enableColoringMenu,
            this.columnsToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.refreshMenuItem,
            this.closeAllWindowsToolStripMenuItem,
            this.toolStripSeparator18,
            this.showTracerXLogsMenuItem,
            this.showServerPickerMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            this.commandProvider.SetUICommand(this.viewToolStripMenuItem, null);
            this.viewToolStripMenuItem.DropDownOpening += new System.EventHandler(this.viewToolStripMenuItem_DropDownOpening);
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.Enabled = false;
            this.filterToolStripMenuItem.Image = global::TracerX.Properties.Resources.Filter1;
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.filterToolStripMenuItem.Text = "Filter...";
            this.filterToolStripMenuItem.ToolTipText = "Open filter dialog";
            this.commandProvider.SetUICommand(this.filterToolStripMenuItem, this.filterDlgCmd);
            // 
            // clearFilterMenuItem
            // 
            this.clearFilterMenuItem.Enabled = false;
            this.clearFilterMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("clearFilterMenuItem.Image")));
            this.clearFilterMenuItem.Name = "clearFilterMenuItem";
            this.clearFilterMenuItem.Size = new System.Drawing.Size(184, 22);
            this.clearFilterMenuItem.Text = "Clear All Filtering";
            this.clearFilterMenuItem.ToolTipText = "Clear all filtering";
            this.commandProvider.SetUICommand(this.clearFilterMenuItem, this.filterClearCmd);
            // 
            // clearColumnColorsMenuItem
            // 
            this.clearColumnColorsMenuItem.Image = global::TracerX.Properties.Resources.ClearColumnColors;
            this.clearColumnColorsMenuItem.Name = "clearColumnColorsMenuItem";
            this.clearColumnColorsMenuItem.Size = new System.Drawing.Size(184, 22);
            this.clearColumnColorsMenuItem.Text = "Clear Column Colors";
            this.clearColumnColorsMenuItem.ToolTipText = "Clear column colors";
            this.commandProvider.SetUICommand(this.clearColumnColorsMenuItem, this.clearColumnColors);
            // 
            // coloringMenuItem
            // 
            this.coloringMenuItem.Image = global::TracerX.Properties.Resources.colors_edit;
            this.coloringMenuItem.Name = "coloringMenuItem";
            this.coloringMenuItem.Size = new System.Drawing.Size(184, 22);
            this.coloringMenuItem.Text = "Coloring Rules...";
            this.coloringMenuItem.ToolTipText = "Edit color rules";
            this.commandProvider.SetUICommand(this.coloringMenuItem, this.editColors);
            // 
            // enableColoringMenu
            // 
            this.enableColoringMenu.Image = global::TracerX.Properties.Resources.colors_on;
            this.enableColoringMenu.Name = "enableColoringMenu";
            this.enableColoringMenu.Size = new System.Drawing.Size(184, 22);
            this.enableColoringMenu.Text = "Enable Coloring";
            this.enableColoringMenu.ToolTipText = "Enable coloring";
            this.commandProvider.SetUICommand(this.enableColoringMenu, this.enableColors);
            // 
            // columnsToolStripMenuItem
            // 
            this.columnsToolStripMenuItem.Image = global::TracerX.Properties.Resources.Columns;
            this.columnsToolStripMenuItem.Name = "columnsToolStripMenuItem";
            this.columnsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.columnsToolStripMenuItem.Text = "Columns...";
            this.columnsToolStripMenuItem.ToolTipText = "Columns";
            this.commandProvider.SetUICommand(this.columnsToolStripMenuItem, this.columnsCmd);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Image = global::TracerX.Properties.Resources.Options;
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.optionsToolStripMenuItem.Text = "Options...";
            this.optionsToolStripMenuItem.ToolTipText = "Options";
            this.commandProvider.SetUICommand(this.optionsToolStripMenuItem, this.optionsCmd);
            // 
            // refreshMenuItem
            // 
            this.refreshMenuItem.Enabled = false;
            this.refreshMenuItem.Image = global::TracerX.Properties.Resources.Refresh;
            this.refreshMenuItem.Name = "refreshMenuItem";
            this.refreshMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshMenuItem.Size = new System.Drawing.Size(184, 22);
            this.refreshMenuItem.Text = "Reload";
            this.refreshMenuItem.ToolTipText = "Reload";
            this.commandProvider.SetUICommand(this.refreshMenuItem, this.refreshCmd);
            // 
            // closeAllWindowsToolStripMenuItem
            // 
            this.closeAllWindowsToolStripMenuItem.Name = "closeAllWindowsToolStripMenuItem";
            this.closeAllWindowsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.closeAllWindowsToolStripMenuItem.Text = "Close All Windows";
            this.commandProvider.SetUICommand(this.closeAllWindowsToolStripMenuItem, null);
            this.closeAllWindowsToolStripMenuItem.Click += new System.EventHandler(this.closeAllWindowsToolStripMenuItem_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(181, 6);
            this.commandProvider.SetUICommand(this.toolStripSeparator18, null);
            // 
            // showTracerXLogsMenuItem
            // 
            this.showTracerXLogsMenuItem.Name = "showTracerXLogsMenuItem";
            this.showTracerXLogsMenuItem.Size = new System.Drawing.Size(184, 22);
            this.showTracerXLogsMenuItem.Text = "Show TracerX Logs";
            this.commandProvider.SetUICommand(this.showTracerXLogsMenuItem, null);
            this.showTracerXLogsMenuItem.Click += new System.EventHandler(this.showTracerXLogsMenuItem_Click);
            // 
            // showServerPickerMenuItem
            // 
            this.showServerPickerMenuItem.Name = "showServerPickerMenuItem";
            this.showServerPickerMenuItem.Size = new System.Drawing.Size(184, 22);
            this.showServerPickerMenuItem.Text = "Show \"Servers\" Pane";
            this.commandProvider.SetUICommand(this.showServerPickerMenuItem, null);
            this.showServerPickerMenuItem.Click += new System.EventHandler(this.showServerPickerMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.licenseToolStripMenuItem,
            this.commandLineMenuItem,
            this.relatedFilesFoldersToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            this.commandProvider.SetUICommand(this.helpToolStripMenuItem, null);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.commandProvider.SetUICommand(this.aboutToolStripMenuItem, null);
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // licenseToolStripMenuItem
            // 
            this.licenseToolStripMenuItem.Name = "licenseToolStripMenuItem";
            this.licenseToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.licenseToolStripMenuItem.Text = "License...";
            this.commandProvider.SetUICommand(this.licenseToolStripMenuItem, null);
            this.licenseToolStripMenuItem.Click += new System.EventHandler(this.licenseToolStripMenuItem_Click);
            // 
            // commandLineMenuItem
            // 
            this.commandLineMenuItem.Name = "commandLineMenuItem";
            this.commandLineMenuItem.Size = new System.Drawing.Size(202, 22);
            this.commandLineMenuItem.Text = "Command Line...";
            this.commandProvider.SetUICommand(this.commandLineMenuItem, null);
            this.commandLineMenuItem.Click += new System.EventHandler(this.commandLineMenuItem_Click);
            // 
            // relatedFilesFoldersToolStripMenuItem
            // 
            this.relatedFilesFoldersToolStripMenuItem.Name = "relatedFilesFoldersToolStripMenuItem";
            this.relatedFilesFoldersToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.relatedFilesFoldersToolStripMenuItem.Text = "Related Files && Folders...";
            this.commandProvider.SetUICommand(this.relatedFilesFoldersToolStripMenuItem, null);
            this.relatedFilesFoldersToolStripMenuItem.Click += new System.EventHandler(this.relatedFilesFoldersToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AllowDrop = true;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileButton,
            this.propertiesButton,
            this.closeBtn,
            this.toolStripSeparator11,
            this.openFilters,
            this.clearFilterButton,
            this.toolStripSeparator6,
            this.refreshButton,
            this.autoUpdate,
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
            this.prevAnyThreadBtn,
            this.nextAnyThreadBtn,
            this.toolStripSeparator15,
            this.prevSameThreadBtn,
            this.nextSameThreadBtn,
            this.toolStripSeparator14,
            this.optionsButton,
            this.columnsButton,
            this.toolStripSeparator12,
            this.dupTimeButton,
            this.relativeTimeButton,
            this.toolStripSeparator1,
            this.clearColumnColorsBtn,
            this.enableColorsBtn,
            this.editColorsBtn,
            this.boldBtn,
            this.toolStripSeparator13,
            this.expandAllButton,
            this.collapseAllButton,
            this.toolStripSeparator17,
            this.prevTimeButton,
            this.nextTimeButton,
            this.timeUnitCombo});
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
            this.openFileButton.ToolTipText = "Open file";
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
            this.propertiesButton.ToolTipText = "File properties";
            this.commandProvider.SetUICommand(this.propertiesButton, this.propertiesCmd);
            // 
            // closeBtn
            // 
            this.closeBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.closeBtn.Image = global::TracerX.Properties.Resources.Close_X_Red_16;
            this.closeBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(23, 22);
            this.closeBtn.Text = "toolStripButton1";
            this.closeBtn.ToolTipText = "Close file or program";
            this.commandProvider.SetUICommand(this.closeBtn, null);
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
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
            this.openFilters.ToolTipText = "Open filter dialog";
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
            this.refreshButton.Text = "Reload";
            this.refreshButton.ToolTipText = "Reload";
            this.commandProvider.SetUICommand(this.refreshButton, this.refreshCmd);
            // 
            // autoUpdate
            // 
            this.autoUpdate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.autoUpdate.Enabled = false;
            this.autoUpdate.Image = ((System.Drawing.Image)(resources.GetObject("autoUpdate.Image")));
            this.autoUpdate.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.autoUpdate.Name = "autoUpdate";
            this.autoUpdate.Size = new System.Drawing.Size(23, 22);
            this.autoUpdate.Text = "Auto update";
            this.autoUpdate.ToolTipText = "Auto-update (\"tail\" the file)";
            this.commandProvider.SetUICommand(this.autoUpdate, null);
            this.autoUpdate.Click += new System.EventHandler(this.autoUpdate_Click);
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
            // prevAnyThreadBtn
            // 
            this.prevAnyThreadBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.prevAnyThreadBtn.Image = global::TracerX.Properties.Resources.AnyThreadPrev;
            this.prevAnyThreadBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.prevAnyThreadBtn.Name = "prevAnyThreadBtn";
            this.prevAnyThreadBtn.Size = new System.Drawing.Size(23, 22);
            this.prevAnyThreadBtn.Text = "Previous block from different thread";
            this.commandProvider.SetUICommand(this.prevAnyThreadBtn, null);
            this.prevAnyThreadBtn.Click += new System.EventHandler(this.prevThreadBtn_Click);
            // 
            // nextAnyThreadBtn
            // 
            this.nextAnyThreadBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextAnyThreadBtn.Image = global::TracerX.Properties.Resources.AnyThreadNext;
            this.nextAnyThreadBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextAnyThreadBtn.Name = "nextAnyThreadBtn";
            this.nextAnyThreadBtn.Size = new System.Drawing.Size(23, 22);
            this.nextAnyThreadBtn.Text = "Next block from different thread";
            this.commandProvider.SetUICommand(this.nextAnyThreadBtn, null);
            this.nextAnyThreadBtn.Click += new System.EventHandler(this.nextThreadBtn_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(6, 25);
            this.commandProvider.SetUICommand(this.toolStripSeparator15, null);
            // 
            // prevSameThreadBtn
            // 
            this.prevSameThreadBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.prevSameThreadBtn.Image = global::TracerX.Properties.Resources.SameThreadPrev;
            this.prevSameThreadBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.prevSameThreadBtn.Name = "prevSameThreadBtn";
            this.prevSameThreadBtn.Size = new System.Drawing.Size(23, 22);
            this.prevSameThreadBtn.Text = "Previous block from same thread";
            this.commandProvider.SetUICommand(this.prevSameThreadBtn, null);
            this.prevSameThreadBtn.Click += new System.EventHandler(this.prevThreadBtn_Click);
            // 
            // nextSameThreadBtn
            // 
            this.nextSameThreadBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextSameThreadBtn.Image = global::TracerX.Properties.Resources.SameThreadNext;
            this.nextSameThreadBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextSameThreadBtn.Name = "nextSameThreadBtn";
            this.nextSameThreadBtn.Size = new System.Drawing.Size(23, 22);
            this.nextSameThreadBtn.Text = "Next block from same thread";
            this.commandProvider.SetUICommand(this.nextSameThreadBtn, null);
            this.nextSameThreadBtn.Click += new System.EventHandler(this.nextThreadBtn_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(6, 25);
            this.commandProvider.SetUICommand(this.toolStripSeparator14, null);
            // 
            // optionsButton
            // 
            this.optionsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.optionsButton.Image = global::TracerX.Properties.Resources.Options;
            this.optionsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.optionsButton.Name = "optionsButton";
            this.optionsButton.Size = new System.Drawing.Size(23, 22);
            this.optionsButton.Text = "Options";
            this.optionsButton.ToolTipText = "Options";
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
            this.columnsButton.ToolTipText = "Columns";
            this.commandProvider.SetUICommand(this.columnsButton, this.columnsCmd);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 25);
            this.commandProvider.SetUICommand(this.toolStripSeparator12, null);
            // 
            // dupTimeButton
            // 
            this.dupTimeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.dupTimeButton.Image = global::TracerX.Properties.Resources.clockPlus;
            this.dupTimeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.dupTimeButton.Name = "dupTimeButton";
            this.dupTimeButton.Size = new System.Drawing.Size(23, 22);
            this.dupTimeButton.Text = "Show duplicate timestamps";
            this.commandProvider.SetUICommand(this.dupTimeButton, null);
            this.dupTimeButton.Click += new System.EventHandler(this.dupTimeButton_Click);
            // 
            // relativeTimeButton
            // 
            this.relativeTimeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.relativeTimeButton.Image = global::TracerX.Properties.Resources.Stopwatch;
            this.relativeTimeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.relativeTimeButton.Name = "relativeTimeButton";
            this.relativeTimeButton.Size = new System.Drawing.Size(23, 22);
            this.relativeTimeButton.Text = "Show relative times";
            this.commandProvider.SetUICommand(this.relativeTimeButton, null);
            this.relativeTimeButton.Click += new System.EventHandler(this.relativeTimeButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            this.commandProvider.SetUICommand(this.toolStripSeparator1, null);
            // 
            // clearColumnColorsBtn
            // 
            this.clearColumnColorsBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearColumnColorsBtn.Image = global::TracerX.Properties.Resources.ClearColumnColors;
            this.clearColumnColorsBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearColumnColorsBtn.Name = "clearColumnColorsBtn";
            this.clearColumnColorsBtn.Size = new System.Drawing.Size(23, 22);
            this.clearColumnColorsBtn.Text = "Clear column colors";
            this.clearColumnColorsBtn.ToolTipText = "Clear column colors";
            this.commandProvider.SetUICommand(this.clearColumnColorsBtn, this.clearColumnColors);
            // 
            // enableColorsBtn
            // 
            this.enableColorsBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.enableColorsBtn.Image = global::TracerX.Properties.Resources.colors_on;
            this.enableColorsBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.enableColorsBtn.Name = "enableColorsBtn";
            this.enableColorsBtn.Size = new System.Drawing.Size(23, 22);
            this.enableColorsBtn.Text = "toolStripButton1";
            this.enableColorsBtn.ToolTipText = "Enable coloring";
            this.commandProvider.SetUICommand(this.enableColorsBtn, this.enableColors);
            // 
            // editColorsBtn
            // 
            this.editColorsBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.editColorsBtn.Image = global::TracerX.Properties.Resources.colors_edit;
            this.editColorsBtn.Name = "editColorsBtn";
            this.editColorsBtn.Size = new System.Drawing.Size(23, 22);
            this.editColorsBtn.Text = "Edit color rules";
            this.editColorsBtn.ToolTipText = "Edit color rules";
            this.commandProvider.SetUICommand(this.editColorsBtn, this.editColors);
            // 
            // boldBtn
            // 
            this.boldBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.boldBtn.Image = global::TracerX.Properties.Resources.Bold;
            this.boldBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.boldBtn.Name = "boldBtn";
            this.boldBtn.Size = new System.Drawing.Size(23, 22);
            this.boldBtn.Text = "Bold font";
            this.commandProvider.SetUICommand(this.boldBtn, null);
            this.boldBtn.Click += new System.EventHandler(this.boldBtn_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(6, 25);
            this.commandProvider.SetUICommand(this.toolStripSeparator13, null);
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
            // collapseAllButton
            // 
            this.collapseAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.collapseAllButton.Image = ((System.Drawing.Image)(resources.GetObject("collapseAllButton.Image")));
            this.collapseAllButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.collapseAllButton.Name = "collapseAllButton";
            this.collapseAllButton.Size = new System.Drawing.Size(23, 22);
            this.collapseAllButton.Text = "Collapse All";
            this.commandProvider.SetUICommand(this.collapseAllButton, null);
            this.collapseAllButton.Click += new System.EventHandler(this.collapseAllButton_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(6, 25);
            this.commandProvider.SetUICommand(this.toolStripSeparator17, null);
            // 
            // prevTimeButton
            // 
            this.prevTimeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.prevTimeButton.Image = global::TracerX.Properties.Resources.clockPrev;
            this.prevTimeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.prevTimeButton.Name = "prevTimeButton";
            this.prevTimeButton.Size = new System.Drawing.Size(23, 22);
            this.prevTimeButton.Text = "toolStripButton1";
            this.commandProvider.SetUICommand(this.prevTimeButton, null);
            this.prevTimeButton.Click += new System.EventHandler(this.prevTimeButton_Click);
            // 
            // nextTimeButton
            // 
            this.nextTimeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextTimeButton.Image = global::TracerX.Properties.Resources.clockNext;
            this.nextTimeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextTimeButton.Name = "nextTimeButton";
            this.nextTimeButton.Size = new System.Drawing.Size(23, 22);
            this.nextTimeButton.Text = "toolStripButton1";
            this.commandProvider.SetUICommand(this.nextTimeButton, null);
            this.nextTimeButton.Click += new System.EventHandler(this.nextTimeButton_Click);
            // 
            // timeUnitCombo
            // 
            this.timeUnitCombo.AutoSize = false;
            this.timeUnitCombo.DropDownWidth = 60;
            this.timeUnitCombo.Items.AddRange(new object[] {
            "Second",
            "Minute",
            "Hour",
            "Day"});
            this.timeUnitCombo.Name = "timeUnitCombo";
            this.timeUnitCombo.Size = new System.Drawing.Size(62, 23);
            this.timeUnitCombo.ToolTipText = "Specifies time unit for next/prev time buttons.";
            this.commandProvider.SetUICommand(this.timeUnitCombo, null);
            this.timeUnitCombo.SelectedIndexChanged += new System.EventHandler(this.timeUnitCombo_SelectedIndexChanged);
            this.timeUnitCombo.Click += new System.EventHandler(this.timeUnitCombo_Click);
            // 
            // columnContextMenu
            // 
            this.columnContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.colMenuFilterItem,
            this.colMenuRemoveItem,
            this.toolStripSeparator5,
            this.colMenuOptionsItem,
            this.colMenuColumnItem,
            this.colMenuHideItem});
            this.columnContextMenu.Name = "columnContextMenu";
            this.columnContextMenu.ShowImageMargin = false;
            this.columnContextMenu.Size = new System.Drawing.Size(151, 120);
            this.commandProvider.SetUICommand(this.columnContextMenu, null);
            // 
            // colMenuFilterItem
            // 
            this.colMenuFilterItem.Name = "colMenuFilterItem";
            this.colMenuFilterItem.Size = new System.Drawing.Size(150, 22);
            this.colMenuFilterItem.Text = "Filter...";
            this.commandProvider.SetUICommand(this.colMenuFilterItem, null);
            this.colMenuFilterItem.Click += new System.EventHandler(this.colMenuFilterItem_Click);
            // 
            // colMenuRemoveItem
            // 
            this.colMenuRemoveItem.Name = "colMenuRemoveItem";
            this.colMenuRemoveItem.Size = new System.Drawing.Size(150, 22);
            this.colMenuRemoveItem.Text = "Remove from Filter";
            this.commandProvider.SetUICommand(this.colMenuRemoveItem, null);
            this.colMenuRemoveItem.Click += new System.EventHandler(this.colMenuRemoveItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(147, 6);
            this.commandProvider.SetUICommand(this.toolStripSeparator5, null);
            // 
            // colMenuOptionsItem
            // 
            this.colMenuOptionsItem.Name = "colMenuOptionsItem";
            this.colMenuOptionsItem.Size = new System.Drawing.Size(150, 22);
            this.colMenuOptionsItem.Text = "Options...";
            this.commandProvider.SetUICommand(this.colMenuOptionsItem, null);
            this.colMenuOptionsItem.Click += new System.EventHandler(this.colMenuOptionsItem_Click);
            // 
            // colMenuColumnItem
            // 
            this.colMenuColumnItem.Name = "colMenuColumnItem";
            this.colMenuColumnItem.Size = new System.Drawing.Size(150, 22);
            this.colMenuColumnItem.Text = "Columns...";
            this.commandProvider.SetUICommand(this.colMenuColumnItem, null);
            this.colMenuColumnItem.Click += new System.EventHandler(this.ExecuteColumns);
            // 
            // colMenuHideItem
            // 
            this.colMenuHideItem.Name = "colMenuHideItem";
            this.colMenuHideItem.Size = new System.Drawing.Size(150, 22);
            this.colMenuHideItem.Text = "Hide Column";
            this.commandProvider.SetUICommand(this.colMenuHideItem, null);
            this.colMenuHideItem.Click += new System.EventHandler(this.colMenuHideItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            this.commandProvider.SetUICommand(this.toolStripSeparator9, null);
            // 
            // theStartPage
            // 
            this.theStartPage.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.theStartPage.BackColor = System.Drawing.SystemColors.Window;
            this.theStartPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.theStartPage.ColumnHeaderBackColor = System.Drawing.Color.PapayaWhip;
            this.theStartPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.theStartPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.theStartPage.Location = new System.Drawing.Point(0, 64);
            this.theStartPage.Name = "theStartPage";
            this.theStartPage.Size = new System.Drawing.Size(886, 578);
            this.theStartPage.TabIndex = 0;
            this.commandProvider.SetUICommand(this.theStartPage, null);
            // 
            // TheListView
            // 
            this.TheListView.AllowColumnReorder = true;
            this.TheListView.BackColor = System.Drawing.SystemColors.Window;
            this.TheListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.headerText,
            this.headerSession,
            this.headerLine,
            this.headerLevel,
            this.headerLogger,
            this.headerThreadId,
            this.headerThreadName,
            this.headerTime,
            this.headerMethod});
            this.TheListView.ContextMenuStrip = this.contextMenuStrip1;
            this.TheListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TheListView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TheListView.FullRowSelect = true;
            this.TheListView.HScrollPos = 0;
            this.TheListView.Location = new System.Drawing.Point(0, 64);
            this.TheListView.Name = "TheListView";
            this.TheListView.ShowItemToolTips = true;
            this.TheListView.Size = new System.Drawing.Size(886, 578);
            this.TheListView.SmallImageList = this.imageList1;
            this.TheListView.TabIndex = 0;
            this.commandProvider.SetUICommand(this.TheListView, null);
            this.TheListView.UseCompatibleStateImageBehavior = false;
            this.TheListView.View = System.Windows.Forms.View.Details;
            this.TheListView.VirtualMode = true;
            this.TheListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.TheListView_ColumnClick);
            this.TheListView.SelectedIndexChanged += new System.EventHandler(this.TheListView_SelectedIndexChanged);
            this.TheListView.VirtualItemsSelectionRangeChanged += new System.Windows.Forms.ListViewVirtualItemsSelectionRangeChangedEventHandler(this.TheListView_VirtualItemsSelectionRangeChanged);
            this.TheListView.Leave += new System.EventHandler(this.TheListView_Leave);
            this.TheListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TheListView_MouseClick);
            this.TheListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TheListView_MouseDoubleClick);
            // 
            // headerText
            // 
            this.headerText.DisplayIndex = 8;
            this.headerText.Text = "Text";
            this.headerText.Width = 298;
            // 
            // headerSession
            // 
            this.headerSession.DisplayIndex = 0;
            this.headerSession.Text = "Session";
            // 
            // headerLine
            // 
            this.headerLine.DisplayIndex = 1;
            this.headerLine.Text = "Line #";
            // 
            // headerLevel
            // 
            this.headerLevel.DisplayIndex = 2;
            this.headerLevel.Text = "Level";
            this.headerLevel.Width = 48;
            // 
            // headerLogger
            // 
            this.headerLogger.DisplayIndex = 3;
            this.headerLogger.Text = "Logger";
            this.headerLogger.Width = 87;
            // 
            // headerThreadId
            // 
            this.headerThreadId.DisplayIndex = 4;
            this.headerThreadId.Text = "Th#";
            this.headerThreadId.Width = 45;
            // 
            // headerThreadName
            // 
            this.headerThreadName.DisplayIndex = 5;
            this.headerThreadName.Text = "ThName";
            this.headerThreadName.Width = 84;
            // 
            // headerTime
            // 
            this.headerTime.DisplayIndex = 6;
            this.headerTime.Text = "Time";
            this.headerTime.Width = 117;
            // 
            // headerMethod
            // 
            this.headerMethod.DisplayIndex = 7;
            this.headerMethod.Text = "Method";
            this.headerMethod.Width = 78;
            // 
            // crumbBar1
            // 
            this.crumbBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.crumbBar1.Location = new System.Drawing.Point(0, 49);
            this.crumbBar1.Name = "crumbBar1";
            this.crumbBar1.Size = new System.Drawing.Size(886, 15);
            this.crumbBar1.TabIndex = 7;
            this.commandProvider.SetUICommand(this.crumbBar1, null);
            // 
            // openFileCmd
            // 
            this.openFileCmd.Checked = false;
            this.openFileCmd.Enabled = true;
            this.openFileCmd.Image = global::TracerX.Properties.Resources.OpenFolder;
            this.openFileCmd.ToolTipText = "Open file";
            this.openFileCmd.Execute += new System.EventHandler(this.ExecuteOpenFile);
            // 
            // propertiesCmd
            // 
            this.propertiesCmd.Checked = false;
            this.propertiesCmd.Enabled = false;
            this.propertiesCmd.Image = ((System.Drawing.Image)(resources.GetObject("propertiesCmd.Image")));
            this.propertiesCmd.ToolTipText = "File properties";
            this.propertiesCmd.Execute += new System.EventHandler(this.ExecuteProperties);
            // 
            // findCmd
            // 
            this.findCmd.Checked = false;
            this.findCmd.Enabled = false;
            this.findCmd.Image = global::TracerX.Properties.Resources.find;
            this.findCmd.ToolTipText = "Find";
            this.findCmd.Execute += new System.EventHandler(this.ExecuteFind);
            // 
            // findNextCmd
            // 
            this.findNextCmd.Checked = false;
            this.findNextCmd.Enabled = false;
            this.findNextCmd.Image = global::TracerX.Properties.Resources.findNext;
            this.findNextCmd.ToolTipText = "Find next";
            this.findNextCmd.Execute += new System.EventHandler(this.ExecuteFindNext);
            // 
            // findPrevCmd
            // 
            this.findPrevCmd.Checked = false;
            this.findPrevCmd.Enabled = false;
            this.findPrevCmd.Image = global::TracerX.Properties.Resources.findPrev;
            this.findPrevCmd.ToolTipText = "Find previous";
            this.findPrevCmd.Execute += new System.EventHandler(this.ExecuteFindPrevious);
            // 
            // bookmarkToggleCmd
            // 
            this.bookmarkToggleCmd.Checked = false;
            this.bookmarkToggleCmd.Enabled = false;
            this.bookmarkToggleCmd.Image = global::TracerX.Properties.Resources.BookmarkToggle2;
            this.bookmarkToggleCmd.ToolTipText = "Toggle bookmark";
            this.bookmarkToggleCmd.Execute += new System.EventHandler(this.ExecuteToggleBookmark);
            // 
            // bookmarkNextCmd
            // 
            this.bookmarkNextCmd.Checked = false;
            this.bookmarkNextCmd.Enabled = false;
            this.bookmarkNextCmd.Image = global::TracerX.Properties.Resources.BookmarkNext;
            this.bookmarkNextCmd.ToolTipText = "Next bookmark";
            this.bookmarkNextCmd.Execute += new System.EventHandler(this.ExecuteNextBookmark);
            // 
            // bookmarkPrevCmd
            // 
            this.bookmarkPrevCmd.Checked = false;
            this.bookmarkPrevCmd.Enabled = false;
            this.bookmarkPrevCmd.Image = global::TracerX.Properties.Resources.BookmarkPrev;
            this.bookmarkPrevCmd.ToolTipText = "Previous bookmark";
            this.bookmarkPrevCmd.Execute += new System.EventHandler(this.ExecutePrevBookmark);
            // 
            // bookmarkClearCmd
            // 
            this.bookmarkClearCmd.Checked = false;
            this.bookmarkClearCmd.Enabled = false;
            this.bookmarkClearCmd.Image = global::TracerX.Properties.Resources.BookmarkClear;
            this.bookmarkClearCmd.ToolTipText = "Clear all bookmarks";
            this.bookmarkClearCmd.Execute += new System.EventHandler(this.ExecuteClearBookmarks);
            // 
            // filterDlgCmd
            // 
            this.filterDlgCmd.Checked = false;
            this.filterDlgCmd.Enabled = false;
            this.filterDlgCmd.Image = global::TracerX.Properties.Resources.Filter1;
            this.filterDlgCmd.ToolTipText = "Open filter dialog";
            this.filterDlgCmd.Execute += new System.EventHandler(this.ExecuteOpenFilterDialog);
            // 
            // filterClearCmd
            // 
            this.filterClearCmd.Checked = false;
            this.filterClearCmd.Enabled = false;
            this.filterClearCmd.Image = ((System.Drawing.Image)(resources.GetObject("filterClearCmd.Image")));
            this.filterClearCmd.ToolTipText = "Clear all filtering";
            this.filterClearCmd.Execute += new System.EventHandler(this.ExecuteClearFilter);
            // 
            // clearColumnColors
            // 
            this.clearColumnColors.Checked = false;
            this.clearColumnColors.Enabled = true;
            this.clearColumnColors.Image = global::TracerX.Properties.Resources.ClearColumnColors;
            this.clearColumnColors.ToolTipText = "Clear column colors";
            this.clearColumnColors.Execute += new System.EventHandler(this.clearColumnColors_Execute);
            // 
            // editColors
            // 
            this.editColors.Checked = false;
            this.editColors.Enabled = true;
            this.editColors.Image = global::TracerX.Properties.Resources.colors_edit;
            this.editColors.ToolTipText = "Edit color rules";
            this.editColors.Execute += new System.EventHandler(this.coloringCmd_Execute);
            // 
            // enableColors
            // 
            this.enableColors.Checked = false;
            this.enableColors.Enabled = true;
            this.enableColors.Image = global::TracerX.Properties.Resources.colors_on;
            this.enableColors.ToolTipText = "Enable coloring";
            this.enableColors.Execute += new System.EventHandler(this.enableColors_Execute);
            // 
            // columnsCmd
            // 
            this.columnsCmd.Checked = false;
            this.columnsCmd.Enabled = true;
            this.columnsCmd.Image = global::TracerX.Properties.Resources.Columns;
            this.columnsCmd.ToolTipText = "Columns";
            this.columnsCmd.Execute += new System.EventHandler(this.ExecuteColumns);
            // 
            // optionsCmd
            // 
            this.optionsCmd.Checked = false;
            this.optionsCmd.Enabled = true;
            this.optionsCmd.Image = global::TracerX.Properties.Resources.Options;
            this.optionsCmd.ToolTipText = "Options";
            this.optionsCmd.Execute += new System.EventHandler(this.ExecuteOptions);
            // 
            // refreshCmd
            // 
            this.refreshCmd.Checked = false;
            this.refreshCmd.Enabled = false;
            this.refreshCmd.Image = global::TracerX.Properties.Resources.Refresh;
            this.refreshCmd.ToolTipText = "Reload";
            this.refreshCmd.Execute += new System.EventHandler(this.ExecuteRefresh);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 664);
            this.Controls.Add(this.theStartPage);
            this.Controls.Add(this.TheListView);
            this.Controls.Add(this.crumbBar1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(100, 150);
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
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton btnCancel;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bookmarkToggle;
        private System.Windows.Forms.ToolStripMenuItem nextBookmarkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previousBookmarkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearAllBookmarksToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton clearFilterButton;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearFilterMenuItem;
        private System.Windows.Forms.ToolStripMenuItem columnsToolStripMenuItem;
        public ListViewTx TheListView;
        private System.Windows.Forms.ToolStripMenuItem showCallStackMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startOfMethodMenuItem;
        private System.Windows.Forms.ToolStripMenuItem endOfMethodMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyTextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyColsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripStatusLabel statusMsg;
        private System.Windows.Forms.ToolStripStatusLabel filenameLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem colMenuOptionsItem;
        public System.Windows.Forms.ColumnHeader headerLine;
        public System.Windows.Forms.ColumnHeader headerTime;
        public System.Windows.Forms.ColumnHeader headerText;
        public System.Windows.Forms.ColumnHeader headerMethod;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton autoUpdate;
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
        private System.Windows.Forms.ToolStripMenuItem closeAllWindowsToolStripMenuItem;
        private Commander.UICommand editColors;
        private Commander.UICommand enableColors;
        private System.Windows.Forms.ToolStripMenuItem coloringMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableColoringMenu;
        private System.Windows.Forms.ToolStripButton editColorsBtn;
        private System.Windows.Forms.ToolStripButton enableColorsBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem exportToCSVToolStripMenuItem;
        private CrumbBar crumbBar1;
        private System.Windows.Forms.ToolStripMenuItem uncolorSelectedMenu;
        private System.Windows.Forms.ToolStripButton nextAnyThreadBtn;
        private System.Windows.Forms.ToolStripButton prevAnyThreadBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripButton prevSameThreadBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripButton nextSameThreadBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        public System.Windows.Forms.ColumnHeader headerSession;
        private System.Windows.Forms.ToolStripButton boldBtn;
        private System.Windows.Forms.ToolStripMenuItem colMenuHideItem;
        private System.Windows.Forms.ToolStripButton dupTimeButton;
        private System.Windows.Forms.ToolStripComboBox timeUnitCombo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripButton prevTimeButton;
        private System.Windows.Forms.ToolStripButton nextTimeButton;
        private NewStartPage theStartPage;
        private System.Windows.Forms.ToolStripMenuItem newWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton closeBtn;
        private System.Windows.Forms.ToolStripMenuItem clearColumnColorsMenuItem;
        private Commander.UICommand clearColumnColors;
        private System.Windows.Forms.ToolStripButton clearColumnColorsBtn;
        private System.Windows.Forms.ToolStripStatusLabel serverLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem showTracerXLogsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showServerPickerMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commandLineMenuItem;
        private System.Windows.Forms.ToolStripButton collapseAllButton;
        private System.Windows.Forms.ToolStripMenuItem mnuShowSelected;
        private System.Windows.Forms.ToolStripMenuItem mnuHideSelected;
        private System.Windows.Forms.ToolStripMenuItem mnuBookmarkSelected;
        private System.Windows.Forms.ToolStripMenuItem mnuColorSelected;
        private System.Windows.Forms.ToolStripMenuItem relatedFilesFoldersToolStripMenuItem;
    }
}
