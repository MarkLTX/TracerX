using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using TracerX.Properties;
using System.ServiceModel;
using TracerX.ExtensionMethods;

namespace TracerX
{
    // Enum for specifying which archive files to display.  An archive
    // file is one whose file name ends in _01, _02, etc. (not _00).
    internal enum ArchiveVisibility
    {
        NoArchives,     // Don't display any archives.
        ViewedArchives, // Display archives that have been viewed.
        AllArchives,    // Display all archives.
    }

    /// <summary>
    /// UserControl that displays a grid of paths (i.e. files or folders)
    /// under a heading.  Raises an event when one of them is clicked.
    /// </summary>
    internal partial class PathControl : UserControl
    {
        public PathControl()
        {
            InitializeComponent();
            theGrid.RowTemplate = new PathGridRow();
            DoubleBuffered = true;

            try
            {
                // Try to make the DataGridView double buffered to reduce flicker.
                var propInfo = typeof(DataGridView).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                propInfo.SetValue(theGrid, true, null);
            }
            catch (Exception)
            {
                // Not that important.
            }
        }

        private static Logger Log = Logger.GetLogger("PathControl");

        public RemoteServer RemoteServer
        {
            get;
            set;
        }

        public Color ColumnHeaderBackColor
        {
            get { return theGrid.ColumnHeadersDefaultCellStyle.BackColor; }
            set { theGrid.ColumnHeadersDefaultCellStyle.BackColor = value; }
        }

        public event EventHandler RefreshClicked
        {
            add
            {
                tsbtnRefresh.Click += value;
            }

            remove
            {
                tsbtnRefresh.Click -= value;
            }
        }

        /// <summary>
        /// Event raised when the user clicks a file or folder (link) in the grid.
        /// Handler should refer to the LastClickedItem to determine what was clicked.
        /// </summary>
        public event EventHandler LastClickedItemChanged;

        /// <summary>
        /// Contains the path of the most recently clicked file or folder
        /// prefixed with "file\n" or "folder\n".  Read this when the 
        /// LastClickedItemChanged event  is raised and check the prefix
        /// to determine if it's a file or a folder.
        /// </summary>
        [Browsable(false)]
        public string LastClickedItem
        {
            get;
            private set;
        }

        public ArchiveVisibility ArchiveVisibility
        {
            get
            {
                return _archiveVisibility;
            }

            set
            {

                if (value == ArchiveVisibility.NoArchives)
                {
                    tsbtnShowArchives.Checked = false;
                }
                else
                {
                    tsbtnShowArchives.Checked = true;
                }

                _archiveVisibility = value;
            }
        }
        
        /// <summary>
        /// True if the paths passsed to AddOrUpdatePaths() are folders, false if files.
        /// Set this before calling AddOrUpdatePaths() or OnLoad(), and don't ever change it.
        /// </summary>
        public bool PathsAreFolders
        {
            get;
            set;
        }

        /// <summary>
        /// True if paths are valid on the local computer and therefore subject
        /// to checking their existence, size, and last write time.
        /// </summary>
        public bool PathsAreLocal
        {
            get { return _pathsAreLocal; }
            set
            {
                tsbtnRefresh.Visible = !value;
                _pathsAreLocal = value;
                
                if (_pathsAreLocal || !PathsAreFolders)
                {
                    containerCol.DefaultCellStyle.ForeColor = Color.Blue;
                }
                else
                {
                    // Make the links for the folder names black to indicate
                    // they are not functional.  
                    containerCol.DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        /// <summary>
        /// If true, the DateTimes for Created, Modified, and Viewed are displayed
        /// in the form of 'some time span' ago.
        /// </summary>
        public bool ShowTimesAgo
        {
            get { return tsbtnAgo.Checked; }
            
            set
            {
                tsbtnAgo.Checked = value;

                if (value)
                {
                    createdCol.HeaderText = "Created Ago";
                    modCol.HeaderText = "Modified Ago";
                    ViewedCol.HeaderText = "Viewed Ago";
                }
                else
                {
                    createdCol.HeaderText = "Created";
                    modCol.HeaderText = "Modified";
                    ViewedCol.HeaderText = "Viewed";
                }
            }
        }

        private enum SortMode { Allowed, Forbidden, Required };

        // Paths that need to be processed in the next MouseLeave event.
        private IEnumerable<PathItem> _deferredPaths;
        private bool _isMouseOver;
        private Dictionary<string, PathItem> _pathDict = new Dictionary<string, PathItem>(StringComparer.OrdinalIgnoreCase);
        private bool _pathsAreLocal;
        private bool _sortIsNeeded;
        private DateTime _lastSortedUtc;
        private bool _isSettingColumns;
        private ArchiveVisibility _archiveVisibility = ArchiveVisibility.NoArchives;
        private DataGridViewCell _cellForCtxMenu;
        private bool _scrollAfterSort = true;
        private System.Windows.Forms.Timer _filterTimer;
        private DateTime _disableFilterUntilTime = DateTime.MinValue;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            tstxtFilter.Text = AppArgs.FileFilter;
            
            // When displayed on a TabControl, this control sometimes inherit the TabControl's font.  Force the desired font here.

            if (this.Font.Size != 8.25)
            {
                this.Font = new Font(this.Font.Name, 8.25F);
            }

            if (theGrid.Font.Size != 9.75)
            {
                theGrid.Font = new Font(theGrid.Font.Name, 9.75F);
            }

            _isSettingColumns = true;

            if (PathsAreFolders)
            {
                ApplyColumnConfigSettings(Settings.Default.FoldersColumns);
                fileCol.HeaderText = "Folder";
                sizeCol.HeaderText = ".TX1 Files";
                tsbtnShowArchives.Visible = false;
                ShowTimesAgo = Settings.Default.ShowFolderTimesAgo;
                tsbtnClean.Visible = false;
                InitSort(Settings.Default.FolderSortCol, modCol, ListSortDirection.Descending);
            }
            else
            {
                ApplyColumnConfigSettings(Settings.Default.FilesColumns);

                if (ArchiveVisibility == ArchiveVisibility.AllArchives)
                {
                    // This means we're on the PathSelector form, selecting a 
                    // file from the folder of a remote server.

                    _scrollAfterSort = false;
                    theGrid.Sort(fileCol, ListSortDirection.Ascending);
                    _scrollAfterSort = true;
                }
                else
                {
                    // This means we're on the start page.

                    ArchiveVisibility = Settings.Default.ShowViewedArchives ? ArchiveVisibility.ViewedArchives : ArchiveVisibility.NoArchives;
                    ShowTimesAgo = Settings.Default.ShowFileTimesAgo;
                    InitSort(Settings.Default.FileSortCol, createdCol, ListSortDirection.Descending);
                }
            }

            _isSettingColumns = false;
        }

        private void InitSort(int sortSetting, DataGridViewColumn defaultCol, ListSortDirection defaultDirection)
        {
            // The sortSetting is 0 if it has never been saved (use the defaults).
            // Otherwise, it contains the index of the column to sort plus one (so it can't be 0).
            // A negative value indicates a descending sort.

            _scrollAfterSort = false;

            if (sortSetting == 0)
            {
                theGrid.Sort(defaultCol, defaultDirection);
            }
            else if (sortSetting < 0)
            {
                // Negative col index means to sort descending.
                sortSetting = -sortSetting;
                theGrid.Sort(theGrid.Columns[sortSetting - 1], ListSortDirection.Descending);
            }
            else
            {
                theGrid.Sort(theGrid.Columns[sortSetting - 1], ListSortDirection.Ascending);
            }

            _scrollAfterSort = true;
        }

        public void ChangeToFormMode()
        {
            // Make the "show times ago" button the only visible one.

            foreach (ToolStripItem ctl in toolStrip1.Items)
            {
                ctl.Visible = (ctl == tsbtnAgo);
            }

            ArchiveVisibility = ArchiveVisibility.AllArchives;
            theGrid.Visible = true;
            Visible = true;
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            theGrid.Dock = DockStyle.Fill;
            BorderStyle = System.Windows.Forms.BorderStyle.None;
            theGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            theGrid.ScrollBars = ScrollBars.Both;
            theGrid.BackgroundColor = SystemColors.Window;
            theGrid.GridColor = SystemColors.ControlDark;
            theGrid.DefaultCellStyle.BackColor = SystemColors.Window;
            theGrid.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            containerCol.Visible = false;
        }

        public void Dbg()
        {
            Debug.WriteLine("theGrid.BackgroundColor = {0}", theGrid.BackgroundColor);
            Debug.WriteLine("theGrid.DefaultCellStyle.BackColor = {0}", theGrid.DefaultCellStyle.BackColor);
            Debug.WriteLine("theGrid.Rows[0].DefaultCellStyle.BackColor = {0}", theGrid.Rows[0].DefaultCellStyle.BackColor);
        }

        // Refreshes the properties of all our PathItems.  Possibly add/removes grid rows.
        // Called by a Forms timer every 2 seconds.
        public void RefreshPathItems()
        {
            // The PathItem properties of remote paths don't change unless the user does a manual 
            // refresh (which reinitializes everything), so a lot of this code only applies to Local paths.

            if (PathsAreLocal)
            {
                FilterPathItems();

                // If any PathItem properties have changed, this will update the corresponding
                // cells in the grid and, if a cell in the current sort column changes, re-sort.

                RefreshCells(SortMode.Allowed);

                foreach (PathItem pathItem in _pathDict.Values)
                {
                    // This assigns a ThreadPool thread to check if the path still exists, get it's last modified time, etc. and
                    // update the PathItem's properties.  Hopefully the thread will complete before the next call so the above
                    // calls to RefreshCells() and IsDisplayable() will have fresh data to work with.

                    pathItem.StartCheck();
                }
            }

            // If displaying "times ago" the displayed time spans always change.

            if (ShowTimesAgo)
            {
                // This must be called in the main UI thread.
                // We invalidate the DateTime columns because
                // even though the underlying DateTime values
                // probably haven't changed, the calculated/displayed
                // result of (DateTime.Now - CellValue) has.

                theGrid.InvalidateColumn(ViewedCol.Index);
                theGrid.InvalidateColumn(modCol.Index);
                theGrid.InvalidateColumn(createdCol.Index);
            }
        }

        public void FilterPathItems(bool force = false)
        {
            // This method gets called by two different timers, one of which is _filterTimer.
            // If _filterTimer is enabled then we're going to get called by it in less than
            // one second so do nothing on this call to avoid excessive processing.  Also  
            // do nothing if we were called less than one second ago.

            if (force || ((_filterTimer == null || !_filterTimer.Enabled) && DateTime.UtcNow > _disableFilterUntilTime))
            {                
                // Adding and removing individual rows from theGrid.Rows is very slow so instead of doing that
                // we build a new list of displayable rows.  If that list is different from theGrid.Rows then we
                // do a "bulk" replacement of theGrid.Rows.

                List<PathGridRow> newRowList = new List<PathGridRow>(_pathDict.Count);
                int added = 0;
                int removed = 0;

                // First check the ones that are already in theGrid, possibly removing some.

                foreach (PathGridRow row in theGrid.Rows)
                {
                    if (IsDisplayable(row.PathItem))
                    {
                        newRowList.Add(row);
                    }
                    else
                    {
                        // An existing row got filtered out.
                        ++removed;
                    }
                }

                // Now check the ones that aren't in theGrid, possibly adding some.

                foreach (PathItem pathItem in _pathDict.Values)
                {
                    if (pathItem.GridRow == null)
                    {
                        // This PathItem isn't in theGrid so we may need to add it.
                        if (IsDisplayable(pathItem))
                        {
                            // This sets and initializes pathItem.GridRow.
                            var newRow = new PathGridRow(this, pathItem);
                            newRowList.Add(newRow);
                            ++added;
                        }
                    }
                    else
                    {
                        // We already checked this one because GridRow != null means it's in theGrid.Rows.
                    }
                }

                //if (!PathsAreFolders) Debug.Print("Added = {0}, Removed = {1}", added, removed);

                if ((added + removed) > 0)
                {
                    theGrid.Rows.Clear();
                    theGrid.Rows.AddRange(newRowList.ToArray());

                    if (added > 0)
                    {
                        // The added rows are at the end and may not belong there.
                        MaybeSort(forceSort: true);
                    }
                }
            
                // Don't do this again for at least one second.

                _disableFilterUntilTime = DateTime.UtcNow.AddSeconds(1);
            }
        }

        /// <summary>
        /// Called to add paths to our collection.  If we already have a given path
        /// is is updated rather than added.
        /// </summary>
        public void AddOrUpdatePaths(IEnumerable<PathItem> newPaths, bool forceUpdate = false)
        {
            if (InvokeRequired)
            {
                // We need to run in the UI thread to update the grid.
                Invoke(new Action<IEnumerable<PathItem>, bool>(AddOrUpdatePaths), newPaths, forceUpdate);
            }
            else if (forceUpdate || !_isMouseOver || theGrid.RowCount == 0)
            {
                _deferredPaths = null;
                ProcessNewPaths(newPaths);
            }
            else 
            {
                // Defer changing the list of files in the grid until the user moves the mouse away.
                _deferredPaths = newPaths;
            }
        }

        private void ProcessNewPaths(IEnumerable<PathItem> newPaths) 
        {
            using (Log.InfoCall())
            {
                if (newPaths == null)
                {
                    _pathDict = new Dictionary<string, PathItem>(StringComparer.OrdinalIgnoreCase);
                    theGrid.Rows.Clear();
                }
                else
                {
                    var newRows = new List<PathGridRow>();
                    bool showTracerxFiles = Settings.Default.ShowTracerxFiles;

                    foreach (PathItem newPath in newPaths)
                    {
                        //if (!newPath.IsTracerXPath || showTracerxFiles)
                        {
                            PathItem oldPath = null;

                            if (_pathDict.TryGetValue(newPath.FullPath, out oldPath))
                            {
                                // It's a path we already have.  Reuse the old object.
                                // Take the ViewTime from the input PathItem, but let
                                // the automatic update timer keep the other properties updated.

                                oldPath.ViewTime = newPath.ViewTime;
                                oldPath.IsFromRecentlyCreatedList |= newPath.IsFromRecentlyCreatedList;

                                if (IsDisplayable(oldPath))
                                {
                                    if (oldPath.GridRow == null)
                                    {
                                        // This sets and initializes oldPath.GridRow.
                                        newRows.Add(new PathGridRow(this, oldPath));
                                    }
                                    else
                                    {
                                        oldPath.GridRow.UpdateCells();
                                    }
                                }
                                else if (oldPath.GridRow != null)
                                {
                                    theGrid.Rows.Remove(oldPath.GridRow);
                                }

                                if (PathsAreLocal && oldPath.IsMissing)
                                {
                                    // The missing path might exist now, so re-check it.
                                    oldPath.StartCheck();
                                }
                            }
                            else
                            {
                                if (IsDisplayable(newPath))
                                {
                                    newRows.Add(new PathGridRow(this, newPath));
                                }

                                _pathDict.Add(newPath.FullPath, newPath);
                                if (PathsAreLocal) newPath.StartCheck();
                            }
                        }
                    }

                    if (newRows.Any())
                    {
                        theGrid.Rows.AddRange(newRows.ToArray());
                        RefreshCells(SortMode.Required);
                    }
                    else if (!PathsAreLocal && _sortIsNeeded)
                    {
                        // Since the paths aren't local, there isn't an
                        // automatic call to RefreshCells() every 2 seconds.
                        // Therefore, force the sort now.
                        //MaybeSort(true);
                    }
                }
            }
        }

        // Returns true if the pathItem should be displayed.
        private bool IsDisplayable(PathItem pathItem)
        {
            // We only show extant files.  
            // We only show archived files (i.e. the _01, _02 etc. files) if IsShowingArchives and the file has ever been viewed.
            return  !pathItem.IsMissing &&
                (tstxtFilter.Text == "" || pathItem.ItemName.IndexOf(tstxtFilter.Text, StringComparison.OrdinalIgnoreCase) != -1) &&
                (!pathItem.IsTracerXPath || Settings.Default.ShowTracerxFiles) &&
                (
                    !pathItem.IsArchive ||
                    ArchiveVisibility == ArchiveVisibility.AllArchives ||
                    (ArchiveVisibility == ArchiveVisibility.ViewedArchives && pathItem.ViewTime > DateTime.MinValue)
                );
        }

        private void theGrid_SelectionChanged(object sender, EventArgs e)
        {
            // We don't allow/support selections.
            theGrid.ClearSelection();
        }

        // This prevents the list from changing while the mouse is over the grid.
        private void theGrid_MouseEnter(object sender, EventArgs e)
        {
            //Debug.WriteLine("MouseEnter");
            theGrid.Focus(); // Try to make mouse wheel work.
            _isMouseOver = true;
        }

        private void theGrid_MouseLeave(object sender, EventArgs e)
        {
            //Debug.WriteLine("MouseLeave");
            _isMouseOver = false;
            MaybeDoDeferred();
        }

        private void MaybeDoDeferred()
        {
            if (_deferredPaths != null && (!_isMouseOver || theGrid.RowCount == 0))
            {
                // The paths changed while the mouse was over the grid or the grid
                // was hidden.  The new paths were cached in _deferredPaths.  Apply them now.

                AddOrUpdatePaths(_deferredPaths); // Clears _deferredPaths.
            }
        }
               
        private void PathGrid_BackgroundImageChanged(object sender, EventArgs e)
        {
            theGrid.BackgroundColor = this.BackColor;
        }

        private void PathGrid_BackColorChanged(object sender, EventArgs e)
        {
            theGrid.BackgroundColor = this.BackColor;
            theGrid.DefaultCellStyle.BackColor = this.BackColor;
        }

        private void theGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            using (Log.DebugCall())
            {
                if (e.RowIndex > -1)
                {                        
                    if (e.Button == MouseButtons.Left && LastClickedItemChanged != null)
                    {
                        // Set LastClickedItem and raise LastClickedItemChanged.

                        var row = theGrid.Rows[e.RowIndex] as PathGridRow;
                        var pathItem = row.PathItem;

                        if (e.ColumnIndex == fileCol.Index)
                        {
                            if (PathsAreFolders)
                            {
                                LastClickedItem = "folder\n" + pathItem.FullPath;
                            }
                            else
                            {
                                LastClickedItem = "file\n" + pathItem.FullPath;
                            }

                            SetLastClickedItemColor(e.RowIndex, e.ColumnIndex);
                            LastClickedItemChanged(this, EventArgs.Empty);
                        }
                        else if (e.ColumnIndex == containerCol.Index)
                        {
                            // User is not allowed to "explore" remote folders
                            // that aren't known to contain log files.
                            if (PathsAreLocal || !PathsAreFolders)
                            {
                                LastClickedItem = "folder\n" + pathItem.ContainerName;
                                SetLastClickedItemColor(e.RowIndex, e.ColumnIndex);
                                LastClickedItemChanged(this, EventArgs.Empty);
                            }
                        }
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        // Right button = Maybe display appropriate context menu.
                        MaybeDisplayContextMenu(theGrid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
                    }
                }
            }
        }

        private void SetLastClickedItemColor(int rowIndex, int colIndex)
        {
            // First restore the default ForeColor for all cells in the column.

            var defaultColor = theGrid.Columns[colIndex].DefaultCellStyle.BackColor;

            foreach (DataGridViewRow row in theGrid.Rows)
            {
                var cell = row.Cells[colIndex].Style.ForeColor = defaultColor;
            }

            // Now set the ForeColor for the specified cell.

            theGrid.Rows[rowIndex].Cells[colIndex].Style.ForeColor = Color.DarkGreen;
        }

        // If the context menu is applicable to the cell, returns true and sets path and isFolder.
        private bool GetContextMenuInfo(DataGridViewCell cell, out PathItem pathItem, out string clickedPath, out bool isFolder)
        {
            bool result = false;
            pathItem = null;
            clickedPath = null;
            isFolder = false;

            if (cell != null)
            {
                pathItem = (cell.OwningRow as PathGridRow).PathItem;

                if (PathsAreLocal || !PathsAreFolders || cell.OwningColumn == fileCol )
                {
                    if (cell.OwningColumn == fileCol)
                    {
                        result = true;
                        clickedPath = pathItem.FullPath;
                        isFolder = PathsAreFolders;
                    }
                    else if (cell.OwningColumn == containerCol)
                    {
                        result = true;
                        clickedPath = pathItem.ContainerName;
                        isFolder = true;
                    }
                }
            }

            return result;
        }

        // If the specified cell has any context menu actions, this
        // displays the context menu.
        private void MaybeDisplayContextMenu(DataGridViewCell cell)
        {
            string clickedPath;
            bool isFolder;
            PathItem pathItem;

            if (GetContextMenuInfo(cell, out pathItem, out clickedPath, out isFolder))
            {
                menuExplore.Visible = PathsAreLocal && isFolder;
                menuCopyToClipboard.Visible = PathsAreLocal && !isFolder;
                menuDeleteFile.Visible = !isFolder;
                menuDeleteRelated.Visible = !isFolder;
                menuDeleteInFolder.Visible = (cell.OwningColumn == fileCol && PathsAreFolders) || (cell.OwningColumn == containerCol && !PathsAreFolders) ;   // ( isFolder && !PathsAreFolders) || (PathsAreFolders && path == pathItem.FullPath);
                menuOpenNew.Visible = !isFolder;
                menuDownload.Visible = !PathsAreLocal && !isFolder;
                menuExploreContainer.Visible = PathsAreLocal && !isFolder;
                menuRemoveFile.Visible = !isFolder && pathItem.ViewTime != DateTime.MinValue && !pathItem.IsFromRecentlyCreatedList;

                _cellForCtxMenu = cell;
                ctxMenu.Show(Control.MousePosition);
                SetLastClickedItemColor(cell.RowIndex, cell.ColumnIndex);
            }
        }

        private int GetArchiveNumber(string filePath)
        {
            // If the file name ends with _n or _nn, return the integer value of n or nn.
            // Else, return 0.

            int result = 0;
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            int last_ = fileName.LastIndexOf('_');

            if (last_ != -1 && last_ < fileName.Length - 1)
            {
                string end = fileName.Substring(last_ + 1);
                int.TryParse(end, out result);
            }

            return result;
        }

        private DataGridViewCell _curHighlightedCell;

        private void theGrid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (_curHighlightedCell != null)
            {
                _curHighlightedCell.Style.BackColor = Color.Empty;
                _curHighlightedCell = null;
            }

            if (e.RowIndex > -1)
            {
                Debug.WriteLine("theGrid_CellMouseEnter");
                if (e.ColumnIndex == fileCol.Index)
                {
                    _curHighlightedCell = theGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    _curHighlightedCell.Style.BackColor = Color.Gold;
                }
                else if (e.ColumnIndex == containerCol.Index)
                {
                    // User is not allowed to "explore" remote folders
                    // that aren't known to contain log files.
                    if (PathsAreLocal || !PathsAreFolders)
                    {
                        _curHighlightedCell = theGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                        _curHighlightedCell.Style.BackColor = Color.Gold;
                    }
                }
            }
        }

        private void ctxMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (_curHighlightedCell != null)
            {
                _curHighlightedCell.Style.BackColor = Color.Empty;
                _curHighlightedCell = null;
            }
        }

        private void ctxMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            //_cellForCtxMenu = null;
        }

        private void theGrid_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            // This handler is called when the mouse is moved out of a cell
            // AND ALSO when the context menu is displayed.  
            // The order of calls when the user right clicks and the context menu opens is..
            //  ctxMenu_Opening
            //  ctxMenu_Opened
            //  theGrid_CellMouseLeave
            // We want to leave the cell highlighted while the context menu is open.

            //if (e.RowIndex > -1)
            //{
            //    Debug.WriteLine("theGrid_CellMouseLeave");
            //    DataGridViewCell exitCell = theGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];

            //    if (exitCell != _cellForCtxMenu)
            //    {
            //        exitCell.Style.BackColor = Color.Empty;
            //    }
            //}

            //if (_curHighlightedCell != null && _curHighlightedCell != _cellForCtxMenu)
            if (_curHighlightedCell != null && !ctxMenu.Visible)
            {
                // As far as I know, this is always redundant to setting the BackColor above.

                _curHighlightedCell.Style.BackColor = Color.Empty;
                _curHighlightedCell = null;
            }
        }

        private void theGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && theGrid.SortOrder != SortOrder.None && theGrid.SortedColumn.Index == e.ColumnIndex)
            {
                if (!_sortIsNeeded)
                {
                    //// Check if the new value would change the sort order.

                    //DataGridViewCell cellChanged = theGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    //if (cellChanged.ValueType == typeof(int))
                    //{
                        
                    //}

                    Log.Debug("Cell changed, maybe sorting.");
                    _sortIsNeeded = true;
                    //MaybeSort();
                }
            }
        }

        private void theGrid_Sorted(object sender, EventArgs e)
        {
            Log.Debug("Grid was sorted.");
            _sortIsNeeded = false;
            _lastSortedUtc = DateTime.UtcNow;

            if (_scrollAfterSort) ScrollToTop();

            if (ArchiveVisibility != ArchiveVisibility.AllArchives)
            {
                // We are on the Start Page, so persist the sort col index and direction in the saved settings.
                // We always add 1 to the col index so it won't be zero.
                // We use a negative value to indicate a descending sort.
                int sortSetting = theGrid.SortedColumn.Index + 1;
                if (theGrid.SortOrder == SortOrder.Descending) sortSetting = -sortSetting;

                if (PathsAreFolders)
                {
                    Settings.Default.FolderSortCol = sortSetting;
                }
                else
                {
                    Settings.Default.FileSortCol = sortSetting;
                }
            }
        }

        private void ScrollToTop()
        {
            // If the grid has any visible rows, scroll to the top.
            foreach (DataGridViewRow row in theGrid.Rows)
            {
                if (row.Visible)
                {
                    theGrid.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        private void theGrid_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            // Force the fillCol to stay in the last position.

            if (e.Column == fillCol)
            {
                Application.Idle += new EventHandler(Application_Idle);
            }

            if (!_isSettingColumns)
            {
                // User changed the column order, not us.

                if (PathsAreFolders)
                {
                    Settings.Default.FoldersColumns = GetColumnConfigSettings();
                }
                else
                {
                    Settings.Default.FilesColumns = GetColumnConfigSettings();
                }
            }
        }

        void Application_Idle(object sender, EventArgs e)
        {
            fillCol.DisplayIndex = theGrid.Columns.Count - 1;
            Application.Idle -= new EventHandler(Application_Idle);
        }

        private int[] GetColumnConfigSettings()
        {
            int[] result = new int[theGrid.ColumnCount];

            for (int i = 0; i < theGrid.ColumnCount; ++i)
            {
                DataGridViewColumn curCol = theGrid.Columns[i];

                // Save the column's display index.  Indicate which columns are
                // not visible with negative values.  Since 0 can't be negative,
                // add 1 to all display indexes.

                if (curCol.Visible)
                {
                    result[i] = curCol.DisplayIndex + 1;
                }
                else
                {
                    result[i] = -(curCol.DisplayIndex + 1);
                }
            }

            return result;
        }

        private void ApplyColumnConfigSettings(int[] config)
        {
            // See GetColumnConfigSettings() for how the config parameter is created.

            if (config != null && config.Length == theGrid.ColumnCount)
            {
                // This will contain the columns in order of their intended display index so we
                // can set the display indexes in order from first to last.
                var colsInDisplayOrder = new DataGridViewColumn[config.Length];

                for (int i = 0; i < theGrid.ColumnCount; ++i)
                {
                    DataGridViewColumn curCol = theGrid.Columns[i];
                    int displayIndex = config[i];

                    curCol.Visible = displayIndex > 0;
                    colsInDisplayOrder[Math.Abs(displayIndex) - 1] = curCol;
                }

                // Set the display indexes in order from first to last.

                for (int i = 0; i < theGrid.ColumnCount; ++i)
                {
                    colsInDisplayOrder[i].DisplayIndex = i;
                }
            }

            // Regardless of the config setting,
            // the fill column is always visible and last, 
            // the Created column is not visible for folders,
            // and the Container column is not visible on the PathSelector form.

            fillCol.Visible = true;
            fillCol.DisplayIndex = theGrid.ColumnCount - 1;
            if (PathsAreFolders) createdCol.Visible = false;
            if (ArchiveVisibility == ArchiveVisibility.AllArchives) containerCol.Visible = false;
        }
        
        private void tsbtnColumns_Click(object sender, EventArgs e)
        {
            _isSettingColumns = true;

            if (PathsAreFolders)
            {
                if (FormDataGridViewColumnSelector.ShowModal("\"Folders\" Columns", theGrid, fillCol, createdCol) == DialogResult.OK)
                {
                    Settings.Default.FoldersColumns = GetColumnConfigSettings();
                }
            }
            else
            {
                if (FormDataGridViewColumnSelector.ShowModal("\"Files\" Columns", theGrid, fillCol) == DialogResult.OK)
                {
                    Settings.Default.FilesColumns = GetColumnConfigSettings();
                }
            }

            _isSettingColumns = false;
        }
        
        //private void btnColumns_Click(object sender, EventArgs e)
        //{            
        //    _isSettingColumns = true;

        //    if (PathsAreFolders)
        //    {
        //        if (FormDataGridViewColumnSelector.ShowModal("\"Folders\" Columns", theGrid, fillCol, createdCol) == DialogResult.OK)
        //        {
        //            Settings.Default.FoldersColumns = GetColumnConfigSettings();
        //        }
        //    }
        //    else
        //    {
        //        if (FormDataGridViewColumnSelector.ShowModal("\"Files\" Columns", theGrid, fillCol) == DialogResult.OK)
        //        {
        //            Settings.Default.FilesColumns = GetColumnConfigSettings();
        //        }
        //    }

        //    _isSettingColumns = false;
        //}

        private void tsbtnShowArchives_Click(object sender, EventArgs e)
        {
            // Toggle the IsChecked state and show/hide the archive files
            // that have been viewed.

            tsbtnShowArchives.Checked = !tsbtnShowArchives.Checked;

            if (tsbtnShowArchives.Checked)
            {
                ArchiveVisibility = ArchiveVisibility.ViewedArchives;
                Settings.Default.ShowViewedArchives = true;
            }
            else
            {
                ArchiveVisibility = ArchiveVisibility.NoArchives;
                Settings.Default.ShowViewedArchives = false;
            }

            // This will add or remove the affected rows.
            FilterPathItems(force: true);

            // Take the focus off the button, because it looks weird.
            theGrid.Focus();
        }
                
        private void tsbtnAgo_Click(object sender, EventArgs e)
        {
            ShowTimesAgo = !ShowTimesAgo;

            if (theGrid.SortedColumn == ViewedCol ||
                theGrid.SortedColumn == createdCol ||
                theGrid.SortedColumn == modCol)
            {
                // Reverse the direction of the sort.  This should leave the rows in the
                // same order because we are now sorting by (DateTime.Now - CellValue)
                // instead of CellValue.

                _scrollAfterSort = false;

                if (theGrid.SortOrder == SortOrder.Ascending)
                {
                    theGrid.Sort(theGrid.SortedColumn, ListSortDirection.Descending);
                }
                else if (theGrid.SortOrder == SortOrder.Descending)
                {
                    theGrid.Sort(theGrid.SortedColumn, ListSortDirection.Ascending);
                }

                _scrollAfterSort = true;
            }

            // Take the focus off the button, because it looks weird.
            theGrid.Focus();

            if (PathsAreFolders)
            {
                Settings.Default.ShowFolderTimesAgo = ShowTimesAgo;
            }
            else
            {
                Settings.Default.ShowFileTimesAgo = ShowTimesAgo;
            }
        }

        private void tsbtnClean_Click(object sender, EventArgs e)
        {
            CleanUpFilesDialog dlg;

            if (PathsAreLocal)
            {
                dlg = new CleanUpFilesDialog(null, "*");
            }
            else
            {
                dlg = new CleanUpFilesDialog(RemoteServer, "*");
            }

            dlg.ShowDialog();

            if (dlg.DidDeleteFiles)
            {
                tsbtnRefresh.PerformClick();
            }
        }

        private void theGrid_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (ShowTimesAgo)
            {
                if (e.Column == ViewedCol ||
                    e.Column == createdCol ||
                    e.Column == modCol)
                {
                    // We actually want to sort by TimeSpans calculated by (DateTime.Now - CellValue) for
                    // each cell, but we can get the same result by comparing the values in reverse order.

                    e.SortResult = DateTime.Compare((DateTime)e.CellValue2, (DateTime)e.CellValue1);
                    e.Handled = true;
                }
            }
        }

        private void theGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null)
            {
                if (e.Value is DateTime)
                {
                    if ((DateTime)e.Value == DateTime.MinValue)
                    {
                        e.Value = "Never";
                        e.FormattingApplied = true;
                    }
                    else if (ShowTimesAgo)
                    {
                        var timeSpan = DateTime.Now - (DateTime)e.Value;
                        e.Value = FormatTimeSpan(timeSpan);
                        e.FormattingApplied = true;
                    }
                }
            }
        }

        private string FormatTimeSpan(TimeSpan ts)
        {
            string result = "";

            if (ts.TotalMinutes < 1)
            {
                result = string.Format("{0} seconds", ts.Seconds);
            }
            else if (ts.TotalHours < 1)
            {
                result = string.Format("{0:N1} minutes", ts.TotalMinutes);
            }
            else if (ts.TotalDays < 1)
            {
                result = string.Format("{0:N1} hours", ts.TotalHours);
            }
            else
            {
                result = string.Format("{0:N1} days", ts.TotalDays);
            }

            return result;
        }

        private void RefreshCells(SortMode sortMode)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<SortMode>(RefreshCells), sortMode);
            }
            else
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                if (sortMode != SortMode.Allowed)
                {
                    // Since sorting is either forbidden or is going to be forced,
                    // there's no point in tracking every cell change.

                    theGrid.CellValueChanged -= theGrid_CellValueChanged;
                }

                foreach (PathGridRow row in theGrid.Rows)
                {
                    // This assigns each row's cells from the properties of it's
                    // corresponding PathItem, which may raise the CellValueChanged
                    // event, which may set _sortIsNeeded.
                    row.UpdateCells();
                }

                if (sortMode == SortMode.Allowed)
                {
                    MaybeSort(false);
                }
                else
                {
                    theGrid.CellValueChanged += theGrid_CellValueChanged;

                    if (sortMode == SortMode.Required)
                    {
                        MaybeSort(true);
                    }
                }

                stopwatch.Stop();

                if (stopwatch.ElapsedMilliseconds > 100) Log.Debug("RefreshCells() took ", stopwatch.ElapsedMilliseconds, " ms");
            }
        }

        // If a sort is needed and we haven't sorted in over 2 seconds, do a sort.
        private void MaybeSort(bool forceSort = false)
        {
            using (Log.DebugCall())
            {
                Log.Debug("_sortIsNeeded = ", _sortIsNeeded);

                if (_sortIsNeeded || forceSort)
                {
                    double secondsSinceSort = (DateTime.UtcNow - _lastSortedUtc).TotalSeconds;

                    if (secondsSinceSort > 2 || forceSort)
                    {
                        if (InvokeRequired)
                        {
                            Log.Debug("Re-invoking on UI thread.");
                            Invoke(new Action<bool>(MaybeSort), forceSort);
                        }
                        else
                        {
                            int scrollPos = theGrid.FirstDisplayedScrollingRowIndex;
                            _scrollAfterSort = false;

                            switch (theGrid.SortOrder)
                            {
                                case SortOrder.None:
                                    break;
                                case SortOrder.Ascending:
                                    Log.Debug("Sorting ascending.");
                                    theGrid.Sort(theGrid.SortedColumn, ListSortDirection.Ascending);
                                    break;
                                case SortOrder.Descending:
                                    Log.Debug("Sorting descending.");
                                    theGrid.Sort(theGrid.SortedColumn, ListSortDirection.Descending);
                                    break;
                            }

                            _scrollAfterSort = true;

                            if (scrollPos > 0)
                            {
                                try
                                {
                                    // This is here because otherwise, the scroll position spontaneously changes when
                                    // the current sort column is "Viewed Ago" in ascending order and the user has
                                    // scrolled to the very bottom.  
                                    theGrid.FirstDisplayedScrollingRowIndex = scrollPos;
                                }
                                catch (Exception ex)
                                {
                                    // The exception I've seen here is something like "scroll position cannot be set to an invisible row".
                                    // Doesn't happen often!.
                                    Log.Debug(ex);
                                }
                            }
                        }
                    }
                    else
                    {
                        Log.Debug("Deferring sort.");
                    }
                }
            }
        }

        private void menuCopyToClipboard_Click(object sender, EventArgs e)
        {
            string path;
            bool isfolder;
            PathItem pathItem;

            // The path must be a file, not a folder.

            if (GetContextMenuInfo(_cellForCtxMenu, out pathItem, out path, out isfolder) && !isfolder)
            {
                var list = new System.Collections.Specialized.StringCollection();
                list.Add(path);
                Clipboard.SetFileDropList(list);
            }
        }

        private void menuExplore_Click(object sender, EventArgs e)
        {
            string path;
            bool isfolder;
            PathItem pathItem;

            // The path must be a folder or "Explore" wouldn't have appeared in the context menu.

            if (GetContextMenuInfo(_cellForCtxMenu, out pathItem, out path, out isfolder) && isfolder)
            {
                if (MainForm.CheckFolderAccess(path))
                {
                    Process.Start("explorer.exe", path);
                }
            }
        }

        private void menuExploreContainer_Click(object sender, EventArgs e)
        {
            string path;
            bool isfolder;
            PathItem pathItem;

            // The path must be a file, not a folder.

            if (GetContextMenuInfo(_cellForCtxMenu, out pathItem, out path, out isfolder) && !isfolder)
            {
                if (MainForm.CheckFolderAccess(pathItem.ContainerName))
                {
                    Process.Start("explorer.exe", string.Format("/select,\"{0}\"", path));
                }
            }
        }

        private void menuOpenNew_Click(object sender, EventArgs e)
        {
            string path;
            bool isfolder;
            PathItem pathItem;

            Log.Info("menuOpenNew_Click");
            
            if (GetContextMenuInfo(_cellForCtxMenu, out pathItem, out path, out isfolder) && !isfolder)
            {
                if (PathsAreLocal)
                {
                    DeploymentDescription.StartNewViewer( path );
                }
                else
                {
                    DeploymentDescription.StartNewViewer(path, RemoteServer.HostName);
                }
            }
        }


        private void menuDeleteInFolder_Click(object sender, EventArgs e)
        {
            string path;
            bool isfolder;
            PathItem pathItem;

            if (GetContextMenuInfo(_cellForCtxMenu, out pathItem, out path, out isfolder) && isfolder)
            {
                // Display the CleanUpFilesDialog with the folder path filled in.

                CleanUpFilesDialog dlg;

                if (PathsAreLocal)
                {
                    dlg = new CleanUpFilesDialog(null, path);
                }
                else
                {
                    dlg = new CleanUpFilesDialog(RemoteServer, path);
                }

                dlg.ShowDialog();

                if (dlg.DidDeleteFiles)
                {
                    tsbtnRefresh.PerformClick();
                }
            }
        }

        private void menuDeleteRelated_Click(object sender, EventArgs e)
        {
            // Deleting all related files means that, given a file name like LogName_00.tx1 or
            // LogName.txt, all files matching LogName_<number>.tx1 should be deleted.

            string path;
            bool isfolder;
            PathItem pathItem;

            if (GetContextMenuInfo(_cellForCtxMenu, out pathItem, out path, out isfolder) && !isfolder)
            {
                string baseLogName = pathItem.GetBaseName();
                string msg = string.Format("Delete \"{0}.tx1\" and archives (\"{0}_<num>.tx1\")\nfrom folder \"{1}\"?\n\nThe folder will also be deleted if it becomes empty.", baseLogName, pathItem.ContainerName);

                if (PathsAreLocal)
                {
                    if (DialogResult.Yes == MainForm.ShowMessageBoxBtns(msg, MessageBoxButtons.YesNo))
                    {
                        try
                        {
                            foreach (string deletedFile in DeleteRelated.DeleteAllRelated(pathItem.ContainerName, baseLogName, Log, true))
                            {
                                ForgetFile(path);
                            }
                        }
                        catch (Exception ex)
                        {
                            MainForm.ShowMessageBox("An error occurred trying to delete all files related to\n" + path + "\n\n" + ex.ToString());
                        }
                    }
                }
                else
                {
                    try
                    {
                        using (ProxyFileEnum serviceProxy = new ProxyFileEnum())
                        {
                            // Need to use host:port and credentials.
                            serviceProxy.SetHost(RemoteServer.HostAndPort);
                            serviceProxy.SetCredentials(RemoteServer.GetCreds());
                            int serverVersion = serviceProxy.ExchangeVersion(1);

                            if (serverVersion >= 4)
                            {
                                // This version of the server has a single method for deleting
                                // all the files and parent folder(s) in one call, so display the
                                // appropriate message and call that method.

                                if (DialogResult.Yes == MainForm.ShowMessageBoxBtns(msg, MessageBoxButtons.YesNo))
                                {
                                    List<string> deletedFiles = serviceProxy.DeleteAllRelated(pathItem.ContainerName, baseLogName, true);

                                    // Delete the files from the grid.

                                    foreach (string deletedFile in deletedFiles)
                                    {
                                        ForgetFile(deletedFile);
                                    }
                                }
                            }
                            else
                            {
                                // This version of the server does not have a way to delete the
                                // parent folder(s) so the confirmation message shouldn't say it
                                // will. Also it requires a separate call to delete each file so
                                // we will enumerate the files now to be sure we have an accurate list.

                                msg = string.Format("Delete \"{0}.tx1\" and archives (\"{0}_*.tx1\")\nfrom folder \"{1}\"?", baseLogName, pathItem.ContainerName);

                                if (DialogResult.Yes == MainForm.ShowMessageBoxBtns(msg, MessageBoxButtons.YesNo))
                                {
                                    List<TXFileInfo> files = serviceProxy.GetFilesInFolder(pathItem.ContainerName);

                                    // If files == null, the folder doesn't exist.

                                    if (files != null)
                                    {
                                        string exactFile = baseLogName + ".tx1";
                                        foreach (TXFileInfo file in files)
                                        {
                                            if (DeleteRelated.IsFileRelated(baseLogName, file.FullPath))
                                            {
                                                serviceProxy.DeleteFile(file.FullPath);
                                                ForgetFile(file.FullPath);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (FaultException<ExceptionDetail> ex)
                    {
                        // This represents an unhandled exception in the remote service method.
                        string error = "";
                        Log.Error(ex.Detail.ToString());

                        if (ex.Detail.Type == "IOException" && ex.Detail.Message.Contains("impersonation level"))
                        {
                            // Have seen this message: "Either a required impersonation level was not provided, or the provided impersonation level is invalid."

                            error = "An error occurred trying to delete files on server {0}.  If the error is related to impersonating the current user, "
                                + "it might be fixed by disabling client impersonation in the TracerX service, "
                                + "or changing the Local Security Policy on the server to grant the following rights to the service account, "
                                + "or using a service account that already has these rights (e.g. Local System)."
                                + "\n\n"
                                + "  \"Impersonate a client after authentication\"\n"
                                + "  \"Create global objects\""
                                + "\n\n"
                                + "This is the error that occurred..."
                                + "\n\n"
                                + Program.GetNestedDetails(ex.Detail);
                        }
                        else
                        {
                            error = "An error occurred trying to delete files on server {0}.\n\n{1}".Fmt(RemoteServer.HostAddress, Program.GetNestedDetails(ex.Detail));
                        }

                        MainForm.ShowMessageBox(error);
                    }
                    catch (FaultException fe)
                    {
                        // This is how the service returns explicit error messages.
                        string error = "An error occurred trying to delete files on server {0}.\n\n{1}".Fmt(RemoteServer.HostAddress, fe.Message);
                        Log.Error(error);
                        MainForm.ShowMessageBox(error);
                    }
                    catch (Exception ex)
                    {
                        // An unexpected error not from the remote service (e.g. connecting to or calling the service).
                        Log.Error(ex);
                        string error = "An unexpected {0} occurred connecting to or calling the remote server {1}.\n\n{2}".Fmt(ex.GetType(), RemoteServer.HostAddress, ex.Message);
                        MainForm.ShowMessageBox(error);
                    }
                }
            }
        }

        private void menuDeleteFile_Click(object sender, EventArgs e)
        {
            string path;
            bool isfolder;
            PathItem pathItem;

            if (GetContextMenuInfo(_cellForCtxMenu, out pathItem, out path, out isfolder) && !isfolder)
            {
                if (DialogResult.Yes == MainForm.ShowMessageBoxBtns("Delete file?\n\n" + path, MessageBoxButtons.YesNo))
                {
                    if (PathsAreLocal)
                    {
                        try
                        { 
                            File.Delete(path);
                            ForgetFile(path);
                        }
                        catch (Exception ex)
                        {
                            MainForm.ShowMessageBox("An error occurred trying to delete file\n" + path + "\n\n" + ex.ToString());
                        }
                    }
                    else
                    {
                        try
                        {
                            using (ProxyFileEnum serviceProxy = new ProxyFileEnum())
                            {
                                // Need to use host:port and credentials.
                                serviceProxy.SetHost(RemoteServer.HostAndPort);
                                serviceProxy.SetCredentials(RemoteServer.GetCreds());
                                int serverVersion = serviceProxy.ExchangeVersion(1);
                                serviceProxy.DeleteFile(path);

                                // Delete/hide the file from the grid.
                                ForgetFile(path);
                            }
                        }
                        catch (FaultException<ExceptionDetail> ex)
                        {
                            // This represents an unhandled exception in the remote service method.
                            string error = "";
                            Log.Error(ex.Detail.ToString());

                            if (ex.Detail.Type == "IOException" && ex.Detail.Message.Contains("impersonation level"))
                            {
                                // Have seen this message: "Either a required impersonation level was not provided, or the provided impersonation level is invalid."

                                error = "An error occurred trying to delete the file on server {0}.  If the error is related to impersonating the current user, "
                                    + "it might be fixed by disabling client impersonation in the TracerX service, "
                                    + "or changing the Local Security Policy on the server to grant the following rights to the service account, "
                                    + "or using a service account that already has these rights (e.g. Local System)."
                                    + "\n\n"
                                    + "  \"Impersonate a client after authentication\"\n"
                                    + "  \"Create global objects\""
                                    + "\n\n"
                                    + "This is the error that occurred..."
                                    + "\n\n"
                                    + Program.GetNestedDetails(ex.Detail);
                            }
                            else
                            {
                                error = "An error occurred trying to delete the file on server {0}.\n\n{1}".Fmt(RemoteServer.HostAddress, Program.GetNestedDetails(ex.Detail));
                            }

                            MainForm.ShowMessageBox(error);
                        }
                        catch (FaultException fe)
                        {
                            // This is how the service returns explicit error messages.
                            string error = "An error occurred trying to delete the file on server {0}.\n\n{1}".Fmt(RemoteServer.HostAddress, fe.Message);
                            Log.Error(error);
                            MainForm.ShowMessageBox(error);
                        }
                        catch (Exception ex)
                        {
                            // An unexpected error not from the remote service (e.g. connecting to or calling the service).
                            Log.Error(ex);
                            string error = "An unexpected {0} occurred connecting to or calling the remote server {1}.\n\n{2}".Fmt(ex.GetType(), RemoteServer.HostAddress, ex.Message);
                            MainForm.ShowMessageBox(error);
                        }
                    }
                }
            }
        }

        private void menuForgetFile_Click(object sender, EventArgs e)
        {
            string path;
            bool isfolder;
            PathItem pathItem;

            // The path must be a file, not a folder.

            if (GetContextMenuInfo(_cellForCtxMenu, out pathItem, out path, out isfolder) && !isfolder)
            {
                // This is only called for files that are in the "recently viewed" list and not in the 
                // "recently created" list.  The reason a user might want to "forget" such a file was 
                // recently viewed is to de-clutter the grid.

                ForgetFile(path);
            }
        }

        private void ForgetFile(string path)
        {
            PathItem pathItem;

            if (_pathDict.TryGetValue(path, out pathItem))
            {
                _pathDict.Remove(pathItem.FullPath);

                if (pathItem.GridRow != null)
                {
                    theGrid.Rows.RemoveAt(pathItem.GridRow.Index);
                }
            }

            if (PathsAreLocal)
            {
                Settings.Default.RecentFiles = Settings.Default.RecentFiles.Where(viewedPath => viewedPath.Path != path).ToArray();
            }
            else
            {
                RemoteServer.ForgetViewedFile(path);
            }
        }

        private void menuDownload_Click(object sender, EventArgs e)
        {
            string path;
            bool isfolder;
            PathItem pathItem;

            // The path must be a file, not a folder.

            if (GetContextMenuInfo(_cellForCtxMenu, out pathItem, out path, out isfolder) && !isfolder)
            {
                string tempFile = DownloadFile(path);

                if (tempFile != null)
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.AddExtension = true;
                    dlg.AutoUpgradeEnabled = true;
                    dlg.CheckFileExists = false;
                    dlg.CheckPathExists = false;
                    dlg.CreatePrompt = false;
                    dlg.DefaultExt = "tx1";
                    dlg.Filter = "TracerX log (*.tx1)|*.tx1";
                    dlg.OverwritePrompt = true;
                    dlg.Title = "Save Log File As...";
                    dlg.ValidateNames = true;
                    dlg.FileName = Path.GetFileName(path);

                    if (dlg.ShowDialog(MainForm.TheMainForm) == DialogResult.OK)
                    {
                        File.Delete(dlg.FileName);
                        File.Move(tempFile, dlg.FileName);
                    }
                    else
                    {
                        File.Delete(tempFile);
                    }
                }
            }
        }

        // Copies the specified file from the remote server to a local temp file
        // and returns the temp file path.  Returns null if operation fails.
        private string DownloadFile(string remoteSrc)
        {
            string localDest = null;

            try
            {
                MainForm.TheMainForm.UseWaitCursor = true;
                Application.DoEvents();

                localDest = Path.GetTempFileName();

                using (ProxyFileReader reader = new ProxyFileReader())
                {
                    reader.SetHost(RemoteServer.HostAndPort);
                    reader.SetCredentials(RemoteServer.GetCreds());
                    int serverVersion = reader.ExchangeVersion(1);
                    reader.OpenFile(remoteSrc);

                    using (var writer = new FileStream(localDest, FileMode.Create))
                    {
                        long fileSize;
                        long stopCount;
                        int byteCnt = 0;

                        byte[] bytes = reader.ReadBytes(0, 100000, out fileSize);
                        stopCount = fileSize;

                        // If the file is being grown by the logger, we don't want
                        // to loop forever (reading a few new bytes each time).

                        while (bytes.Length > 0 && byteCnt < stopCount)
                        {
                            writer.Write(bytes, 0, bytes.Length);
                            byteCnt += bytes.Length;
                            bytes = reader.ReadBytes(byteCnt, 100000, out fileSize);

                            if (fileSize < stopCount)
                            {
                                stopCount = fileSize;
                            }
                        }
                    }
                }
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                // This represents an unhandled exception in the remote service method.
                string error = "";
                Log.Error(ex.Detail.ToString());

                if (ex.Detail.Type == "IOException" && ex.Detail.Message.Contains("impersonation level"))
                {
                    // Have seen this message: "Either a required impersonation level was not provided, or the provided impersonation level is invalid."

                    error = "An error occurred on server {0} when trying to download the file.  If the error is related to impersonating the current user, "
                        + "it might be fixed by disabling client impersonation in the TracerX service, "
                        + "or changing the Local Security Policy on the server to grant the following rights to the service account, "
                        + "or using a service account that already has these rights (e.g. Local System)."
                        + "\n\n"
                        + "  \"Impersonate a client after authentication\"\n"
                        + "  \"Create global objects\""
                        + "\n\n"
                        + "This is the error that occurred..."
                        + "\n\n"
                        + Program.GetNestedDetails(ex.Detail);
                }
                else
                {
                    error = "An error occurred on server {0} when trying to download the file.\n\n{1}".Fmt(RemoteServer.HostAddress, Program.GetNestedDetails(ex.Detail));
                }

                MainForm.TheMainForm.UseWaitCursor = false;
                MainForm.ShowMessageBox(error);
                localDest = null;
            }
            catch (FaultException fe)
            {
                // This is how the service returns explicit error messages.
                string error = "An error occurred on server {0} when trying to download the file.\n\n{1}".Fmt(RemoteServer.HostAddress, fe.Message);
                Log.Error(error);
                MainForm.TheMainForm.UseWaitCursor = false;
                MainForm.ShowMessageBox(error);
                localDest = null;
            }
            catch (Exception ex)
            {
                // An unexpected error not from the remote service (e.g. connecting to or calling the service).
                Log.Error(ex);
                string error = "An unexpected {0} occurred trying to download the file from remote server {1}.\n\n{2}".Fmt(ex.GetType(), RemoteServer.HostAddress, ex.Message);
                MainForm.TheMainForm.UseWaitCursor = false;
                MainForm.ShowMessageBox(error);
                localDest = null;
            }

            MainForm.TheMainForm.UseWaitCursor = false;
            return localDest;
        }

        private void tstxtFilter_TextChanged(object sender, EventArgs e)
        {
           Debug.Print("tstxtFilter_TextChanged");

            if (_filterTimer == null)
            {
                _filterTimer = new System.Windows.Forms.Timer();
                _filterTimer.Interval = 1000;
                _filterTimer.Tick += _filterTimer_Tick;
            }

            // This should reset the timer back to it's full interval.

            _filterTimer.Enabled = false;
            _filterTimer.Enabled = true;
        }

        void _filterTimer_Tick(object sender, EventArgs e)
        {
            Debug.Print("_filterTimer_Tick");
            _filterTimer.Enabled = false;

            // We don't trim or otherwise alter the filter textbox because the user
            // may still be typing and want leading, trailing, or embedded blanks.

            FilterPathItems();

            if (tstxtFilter.Text == "")
            {
                // No filter so use regular background color.
                tstxtFilter.BackColor = Color.Empty;
            }
            else
            {
                // Use yellow backcolor to indicate active filter.
                tstxtFilter.BackColor = Color.Yellow;
            }
        }
    }
}
