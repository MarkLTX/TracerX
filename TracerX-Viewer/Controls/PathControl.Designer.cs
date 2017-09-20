namespace TracerX
{
    partial class PathControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.theGrid = new System.Windows.Forms.DataGridView();
            this.fileCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createdCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.modCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ViewedCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sizeCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.containerCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fillCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ctxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuExplore = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExploreContainer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDownload = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeleteFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeleteInFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeleteRelated = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRemoveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpenNew = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbtnAgo = new System.Windows.Forms.ToolStripButton();
            this.tsbtnColumns = new System.Windows.Forms.ToolStripButton();
            this.tsbtnShowArchives = new System.Windows.Forms.ToolStripButton();
            this.tsbtnRefresh = new System.Windows.Forms.ToolStripButton();
            this.tsbtnClean = new System.Windows.Forms.ToolStripButton();
            this.tslblFilter = new System.Windows.Forms.ToolStripLabel();
            this.tstxtFilter = new System.Windows.Forms.ToolStripTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.theGrid)).BeginInit();
            this.ctxMenu.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // theGrid
            // 
            this.theGrid.AllowUserToAddRows = false;
            this.theGrid.AllowUserToDeleteRows = false;
            this.theGrid.AllowUserToOrderColumns = true;
            this.theGrid.AllowUserToResizeRows = false;
            this.theGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.theGrid.BackgroundColor = System.Drawing.Color.GreenYellow;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LemonChiffon;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.theGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.theGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.theGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fileCol,
            this.createdCol,
            this.modCol,
            this.ViewedCol,
            this.sizeCol,
            this.containerCol,
            this.fillCol});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.theGrid.DefaultCellStyle = dataGridViewCellStyle4;
            this.theGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.theGrid.EnableHeadersVisualStyles = false;
            this.theGrid.GridColor = System.Drawing.Color.Tan;
            this.theGrid.Location = new System.Drawing.Point(0, 29);
            this.theGrid.MultiSelect = false;
            this.theGrid.Name = "theGrid";
            this.theGrid.ReadOnly = true;
            this.theGrid.RowHeadersVisible = false;
            this.theGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.theGrid.Size = new System.Drawing.Size(500, 99);
            this.theGrid.TabIndex = 2;
            this.theGrid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.theGrid_CellFormatting);
            this.theGrid.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.theGrid_CellMouseClick);
            this.theGrid.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.theGrid_CellMouseEnter);
            this.theGrid.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.theGrid_CellMouseLeave);
            this.theGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.theGrid_CellValueChanged);
            this.theGrid.ColumnDisplayIndexChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.theGrid_ColumnDisplayIndexChanged);
            this.theGrid.SelectionChanged += new System.EventHandler(this.theGrid_SelectionChanged);
            this.theGrid.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.theGrid_SortCompare);
            this.theGrid.Sorted += new System.EventHandler(this.theGrid_Sorted);
            this.theGrid.MouseEnter += new System.EventHandler(this.theGrid_MouseEnter);
            this.theGrid.MouseLeave += new System.EventHandler(this.theGrid_MouseLeave);
            // 
            // fileCol
            // 
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Blue;
            this.fileCol.DefaultCellStyle = dataGridViewCellStyle2;
            this.fileCol.HeaderText = ".TX1 File";
            this.fileCol.Name = "fileCol";
            this.fileCol.ReadOnly = true;
            this.fileCol.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.fileCol.Width = 74;
            // 
            // createdCol
            // 
            this.createdCol.HeaderText = "Created";
            this.createdCol.Name = "createdCol";
            this.createdCol.ReadOnly = true;
            this.createdCol.Width = 69;
            // 
            // modCol
            // 
            this.modCol.HeaderText = "Modified";
            this.modCol.Name = "modCol";
            this.modCol.ReadOnly = true;
            this.modCol.Width = 72;
            // 
            // ViewedCol
            // 
            this.ViewedCol.HeaderText = "Viewed";
            this.ViewedCol.Name = "ViewedCol";
            this.ViewedCol.ReadOnly = true;
            this.ViewedCol.Width = 67;
            // 
            // sizeCol
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "N0";
            dataGridViewCellStyle3.NullValue = null;
            this.sizeCol.DefaultCellStyle = dataGridViewCellStyle3;
            this.sizeCol.HeaderText = "Size";
            this.sizeCol.Name = "sizeCol";
            this.sizeCol.ReadOnly = true;
            this.sizeCol.Width = 52;
            // 
            // containerCol
            // 
            this.containerCol.HeaderText = "Containing Folder";
            this.containerCol.MinimumWidth = 115;
            this.containerCol.Name = "containerCol";
            this.containerCol.ReadOnly = true;
            this.containerCol.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.containerCol.Width = 115;
            // 
            // fillCol
            // 
            this.fillCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.fillCol.HeaderText = "";
            this.fillCol.MinimumWidth = 2;
            this.fillCol.Name = "fillCol";
            this.fillCol.ReadOnly = true;
            this.fillCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(500, 4);
            this.panel1.TabIndex = 3;
            // 
            // ctxMenu
            // 
            this.ctxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuExplore,
            this.menuExploreContainer,
            this.menuDownload,
            this.menuCopyToClipboard,
            this.menuDeleteFile,
            this.menuDeleteInFolder,
            this.menuDeleteRelated,
            this.menuRemoveFile,
            this.menuOpenNew});
            this.ctxMenu.Name = "ctxMenu";
            this.ctxMenu.ShowImageMargin = false;
            this.ctxMenu.Size = new System.Drawing.Size(201, 224);
            this.ctxMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.ctxMenu_Closed);
            this.ctxMenu.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.ctxMenu_Closing);
            // 
            // menuExplore
            // 
            this.menuExplore.Name = "menuExplore";
            this.menuExplore.Size = new System.Drawing.Size(200, 22);
            this.menuExplore.Text = "Explore";
            this.menuExplore.Click += new System.EventHandler(this.menuExplore_Click);
            // 
            // menuExploreContainer
            // 
            this.menuExploreContainer.Name = "menuExploreContainer";
            this.menuExploreContainer.Size = new System.Drawing.Size(200, 22);
            this.menuExploreContainer.Text = "Explore Container";
            this.menuExploreContainer.Click += new System.EventHandler(this.menuExploreContainer_Click);
            // 
            // menuDownload
            // 
            this.menuDownload.Name = "menuDownload";
            this.menuDownload.Size = new System.Drawing.Size(200, 22);
            this.menuDownload.Text = "Download";
            this.menuDownload.Click += new System.EventHandler(this.menuDownload_Click);
            // 
            // menuCopyToClipboard
            // 
            this.menuCopyToClipboard.Name = "menuCopyToClipboard";
            this.menuCopyToClipboard.Size = new System.Drawing.Size(200, 22);
            this.menuCopyToClipboard.Text = "Copy";
            this.menuCopyToClipboard.Click += new System.EventHandler(this.menuCopyToClipboard_Click);
            // 
            // menuDeleteFile
            // 
            this.menuDeleteFile.Name = "menuDeleteFile";
            this.menuDeleteFile.Size = new System.Drawing.Size(200, 22);
            this.menuDeleteFile.Text = "Delete";
            this.menuDeleteFile.Click += new System.EventHandler(this.menuDeleteFile_Click);
            // 
            // menuDeleteInFolder
            // 
            this.menuDeleteInFolder.Name = "menuDeleteInFolder";
            this.menuDeleteInFolder.Size = new System.Drawing.Size(200, 22);
            this.menuDeleteInFolder.Text = "Delete Logs in Folder..";
            this.menuDeleteInFolder.Click += new System.EventHandler(this.menuDeleteInFolder_Click);
            // 
            // menuDeleteRelated
            // 
            this.menuDeleteRelated.Name = "menuDeleteRelated";
            this.menuDeleteRelated.Size = new System.Drawing.Size(200, 22);
            this.menuDeleteRelated.Text = "Delete (and All Related Logs)";
            this.menuDeleteRelated.Click += new System.EventHandler(this.menuDeleteRelated_Click);
            // 
            // menuRemoveFile
            // 
            this.menuRemoveFile.Name = "menuRemoveFile";
            this.menuRemoveFile.Size = new System.Drawing.Size(200, 22);
            this.menuRemoveFile.Text = "Forget Log Was  Viewed";
            this.menuRemoveFile.Click += new System.EventHandler(this.menuForgetFile_Click);
            // 
            // menuOpenNew
            // 
            this.menuOpenNew.Name = "menuOpenNew";
            this.menuOpenNew.Size = new System.Drawing.Size(200, 22);
            this.menuOpenNew.Text = "Open in New Window";
            this.menuOpenNew.Click += new System.EventHandler(this.menuOpenNew_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Window;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnAgo,
            this.tsbtnColumns,
            this.tsbtnShowArchives,
            this.tsbtnRefresh,
            this.tsbtnClean,
            this.tslblFilter,
            this.tstxtFilter});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(500, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbtnAgo
            // 
            this.tsbtnAgo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnAgo.Image = global::TracerX.Properties.Resources.Stopwatch;
            this.tsbtnAgo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnAgo.Name = "tsbtnAgo";
            this.tsbtnAgo.Size = new System.Drawing.Size(23, 22);
            this.tsbtnAgo.Text = "Display DateTimes as\"time span ago\"";
            this.tsbtnAgo.Click += new System.EventHandler(this.tsbtnAgo_Click);
            // 
            // tsbtnColumns
            // 
            this.tsbtnColumns.BackColor = System.Drawing.Color.Transparent;
            this.tsbtnColumns.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnColumns.Image = global::TracerX.Properties.Resources.Columns;
            this.tsbtnColumns.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnColumns.Name = "tsbtnColumns";
            this.tsbtnColumns.Size = new System.Drawing.Size(23, 22);
            this.tsbtnColumns.Text = "Manage columns...";
            this.tsbtnColumns.Click += new System.EventHandler(this.tsbtnColumns_Click);
            // 
            // tsbtnShowArchives
            // 
            this.tsbtnShowArchives.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnShowArchives.Image = global::TracerX.Properties.Resources._01;
            this.tsbtnShowArchives.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnShowArchives.Name = "tsbtnShowArchives";
            this.tsbtnShowArchives.Size = new System.Drawing.Size(23, 22);
            this.tsbtnShowArchives.Text = "Include recently viewed archive files (*_01, *_02, etc.)";
            this.tsbtnShowArchives.Click += new System.EventHandler(this.tsbtnShowArchives_Click);
            // 
            // tsbtnRefresh
            // 
            this.tsbtnRefresh.BackColor = System.Drawing.SystemColors.Window;
            this.tsbtnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnRefresh.Image = global::TracerX.Properties.Resources.Refresh;
            this.tsbtnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnRefresh.Name = "tsbtnRefresh";
            this.tsbtnRefresh.Size = new System.Drawing.Size(23, 22);
            this.tsbtnRefresh.Text = "Refresh files and folders";
            // 
            // tsbtnClean
            // 
            this.tsbtnClean.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnClean.Image = global::TracerX.Properties.Resources.broom;
            this.tsbtnClean.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnClean.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnClean.Name = "tsbtnClean";
            this.tsbtnClean.Size = new System.Drawing.Size(23, 22);
            this.tsbtnClean.Text = "Clean up log files...";
            this.tsbtnClean.Click += new System.EventHandler(this.tsbtnClean_Click);
            // 
            // tslblFilter
            // 
            this.tslblFilter.Name = "tslblFilter";
            this.tslblFilter.Size = new System.Drawing.Size(48, 22);
            this.tslblFilter.Text = "    Filter:";
            // 
            // tstxtFilter
            // 
            this.tstxtFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tstxtFilter.Name = "tstxtFilter";
            this.tstxtFilter.Size = new System.Drawing.Size(125, 25);
            this.tstxtFilter.TextChanged += new System.EventHandler(this.tstxtFilter_TextChanged);
            // 
            // PathControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.theGrid);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PathControl";
            this.Size = new System.Drawing.Size(500, 128);
            this.BackColorChanged += new System.EventHandler(this.PathGrid_BackColorChanged);
            this.BackgroundImageChanged += new System.EventHandler(this.PathGrid_BackgroundImageChanged);
            ((System.ComponentModel.ISupportInitialize)(this.theGrid)).EndInit();
            this.ctxMenu.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolTip toolTip1;
        internal System.Windows.Forms.DataGridView theGrid;
        private System.Windows.Forms.ContextMenuStrip ctxMenu;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteFile;
        private System.Windows.Forms.ToolStripMenuItem menuCopyToClipboard;
        private System.Windows.Forms.ToolStripMenuItem menuExploreContainer;
        private System.Windows.Forms.ToolStripMenuItem menuRemoveFile;
        private System.Windows.Forms.ToolStripMenuItem menuExplore;
        private System.Windows.Forms.ToolStripMenuItem menuDownload;
        private System.Windows.Forms.DataGridViewTextBoxColumn fileCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn modCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn ViewedCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn sizeCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn containerCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn fillCol;
        private System.Windows.Forms.ToolStripMenuItem menuOpenNew;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbtnAgo;
        private System.Windows.Forms.ToolStripButton tsbtnColumns;
        private System.Windows.Forms.ToolStripButton tsbtnShowArchives;
        private System.Windows.Forms.ToolStripButton tsbtnRefresh;
        private System.Windows.Forms.ToolStripLabel tslblFilter;
        public System.Windows.Forms.ToolStripTextBox tstxtFilter;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteRelated;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteInFolder;
        private System.Windows.Forms.ToolStripButton tsbtnClean;
    }
}
