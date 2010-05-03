using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using TracerX.Properties;
using TracerX.Forms; 

// TODO: Option to make line numbers continuous or restart at sessions.
// TODO: Indicate which column (thread, logger, etc) is used for coloring.
// TODO: Add way to navigate by time so user can quickly get to desired time.

// See http://blogs.msdn.com/cumgranosalis/archive/2006/03/06/VirtualListViewUsage.aspx
// for a good article on using ListView in virtual mode.

// SvnBridge: https://tfs05.codeplex.com, id_cp

namespace TracerX.Viewer {
    // This is the main form for the TracerX log viewer.
    [System.Diagnostics.DebuggerDisplay("MainForm")] // Helps prevent debugger from freezing in the worker thread.
    internal partial class MainForm : Form {
        #region Ctor/init
        // Constructor.  args[0] may contain the log file path to load.
        public MainForm(string[] args) {
            InitializeComponent();

            crumbBar1.Clear();

            _originalTitle = this.Text;
            this.Icon = Properties.Resources.scroll_view;
            Thread.CurrentThread.Name = "Main";
            TheMainForm = this;

            //EventHandler filterChange = new EventHandler(FilterAddedOrRemoved);
            SessionObjects.AllVisibleChanged += FilterAddedOrRemoved;
            ThreadNames.AllVisibleChanged += FilterAddedOrRemoved;
            ThreadObjects.AllVisibleChanged += FilterAddedOrRemoved;
            LoggerObjects.AllVisibleChanged += FilterAddedOrRemoved;
            MethodObjects.AllVisibleChanged += FilterAddedOrRemoved;
            FilterDialog.TextFilterOnOff += FilterAddedOrRemoved;

            InitColumns();

            if (Settings.Default.IndentChar == '\0') Settings.Default.IndentChar = ' ';

            // Setting the FileState affects many menu items and buttons.
            _FileState = FileState.NoFile;

            relativeTimeButton.Checked = Settings.Default.RelativeTime;

            enableColors.Checked = Settings.Default.ColoringEnabled;

            Settings.Default.PropertyChanged += Settings_PropertyChanged;

            if (Settings.Default.ColoringRules != null) {
                foreach (ColoringRule rule in Settings.Default.ColoringRules) rule.MakeReady();
            }

            if (args.Length > 0) {
                StartReading(args[0]);
            }

            VersionChecker.CheckForNewVersion();
        }

        // Perform column-related initialization.
        private void InitColumns() {
            // Keep a copy of the original list of columns since the only way to
            // hide a column in a ListView is to remove it.
            OriginalColumns = new ColumnHeader[TheListView.Columns.Count];
            for (int i = 0; i < TheListView.Columns.Count; ++i) {
                OriginalColumns[i] = TheListView.Columns[i];
            }

            // Apply the persisted column settings.
            if (Settings.Default.ColIndices != null &&
                Settings.Default.ColSelections != null &&
                Settings.Default.ColWidths != null &&
                Settings.Default.ColWidths.Length == OriginalColumns.Length) {
                TheListView.Columns.Clear();
                try {
                    for (int i = 0; i < OriginalColumns.Length; ++i) {
                        if (Settings.Default.ColSelections[i]) {
                            TheListView.Columns.Add(OriginalColumns[i]);
                        }
                        OriginalColumns[i].Width = Settings.Default.ColWidths[i];
                    }

                    // We can't set the display index until after the column headers
                    // have been added.
                    for (int i = 0; i < OriginalColumns.Length; ++i) {
                        OriginalColumns[i].DisplayIndex = Settings.Default.ColIndices[i];
                    }
                } catch {
                    // If something goes wrong, just show all columns.
                    TheListView.Columns.Clear();
                    TheListView.Columns.AddRange(OriginalColumns);
                }
            }
        }
        #endregion

        #region Public
        // This gives other classes access to the MainForm instance.
        public static MainForm TheMainForm;

        // The original columns in the ListView before any are hidden.
        public ColumnHeader[] OriginalColumns;

        // These are the trace levels that exist in the current file.
        public TraceLevel ValidTraceLevels {
            get { return _reader.LevelsFound; }
        }

        // The timestamp from the row selected to be the "zero time" row.
        public static DateTime ZeroTime = DateTime.MinValue;

        public List<Row> Rows { get { return _rows; } }

        // Track which trace levels are visible (not filtered out).
        public TraceLevel VisibleTraceLevels {
            get { return _visibleTraceLevels; }
            set {
                if (_visibleTraceLevels != value) {
                    // If changing to or from all levels being visible (_reader.LevelsFound),
                    // call FilterAddedOrRemoved.
                    TraceLevel oldVal = _visibleTraceLevels;
                    _visibleTraceLevels = value;
                    if (_visibleTraceLevels == _reader.LevelsFound || oldVal == _reader.LevelsFound) {
                        FilterAddedOrRemoved(null, null);
                    }
                }
            }
        }

        // The Row corresponding to the the ListViewItem that has focus, or the first
        // row if no item has the focus.  Used as the start of a Find.
        public Row FocusedRow {
            get {
                if (TheListView.FocusedItem == null) {
                    return _rows[0];
                } else {
                    return _rows[TheListView.FocusedItem.Index];
                }
            }
        }

        // The Row corresponding to the the ListViewItem that has focus.
        // Null if no items are selected or no item has focus.
        public Row CurrentRow {
            get {
                if (TheListView.FocusedItem == null || TheListView.SelectedIndices.Count == 0) {
                    return null;
                } else {
                    return _rows[TheListView.FocusedItem.Index];
                }
            }
        }

        public int NumRows {
            get { return TheListView.VirtualListSize; }

            set {
                try {
                    // This can throw an exception when the user selects a row near the
                    // end of the file and hides a thread with lots of records.  It seems
                    // to be a bug in ListView.  The app seems to function OK as long as we catch
                    // and ignore the exception.
                    TheListView.VirtualListSize = value;
                } catch (Exception ex) {
                    Debug.Print("Exception setting ListView.VirtualListSize: " + ex.ToString());
                }

                Debug.Print("NumRows now " + NumRows);

                // Disable Find and FindNext/F3 if no text is visible.
                UpdateFindCommands();

                UpdateThreadButtons();

            }
        }

        public Row SelectSingleRow(int rowIndex) {
            SelectSingleItem(_rows[rowIndex]);
            return _rows[rowIndex];
        }

        public void ClearItemCache() {
            Debug.Print("Clearing _itemCache.");
            _itemCache = null; // Cause cache to be rebuilt.
        }

        public void GetVirtualItem(Object sender, RetrieveVirtualItemEventArgs e) {
            bool newItem;
            ViewItem item = GetListItem(e.ItemIndex, out newItem);
            e.Item = item;

            // When the main form is not active, the blue highlighting for selected items disappears.
            // To prevent that, we explicitly set the item's colors.  The selected items (and 
            // the scroll position) can change
            // while the form is not active (e.g. when the user clicks Find Next in the find dialog),
            // so it is not sufficient to just set the colors in the Activated and Deactivated events.
            // New items are created with the correct colors, so we only call SetItemColors for
            // existing items.
            if (!newItem && this != Form.ActiveForm) item.SetItemColors(false);
        }

        public void InvalidateTheListView() {
            ClearItemCache();
            TheListView.Invalidate();
        }

        // Called by the FindDialog and when F3 is pressed.
        // If the specified text is found (not bookmarked), selects the row/item
        // and returns the ListViewItem.
        public Row DoSearch(StringMatcher matcher, bool searchUp, bool bookmark) {
            Cursor restoreCursor = this.Cursor;
            Row startRow = FocusedRow;
            Row curRow = startRow;
            bool found = false;

            // Remember inputs in case user hits F3 or shift+F3.
            _textMatcher = matcher;

            UpdateFindCommands();

            try {
                this.Cursor = Cursors.WaitCursor;

                do {
                    curRow = NextRow(curRow, searchUp);

                    if (matcher.Matches(curRow.ToString())) {
                        found = true;
                        if (bookmark) {
                            curRow.IsBookmarked = true;
                        } else {
                            SelectSingleItem(curRow);
                            return curRow;
                        }
                    }
                } while (curRow != startRow);
            } finally {
                this.Cursor = restoreCursor;
            }

            if (!found) {
                ShowMessageBox("Did not find: " + matcher.Needle);
            } else if (bookmark) {
                bookmarkNextCmd.Enabled = bookmarkPrevCmd.Enabled = bookmarkClearCmd.Enabled = true;
                InvalidateTheListView();
            }

            return null;
        }

        // Find the index of the last visible item displayed by TheListView.
        public int FindLastVisibleItem() {
            int ndx = TheListView.TopItem.Index;
            do {
                ++ndx;
            } while (ndx < NumRows && TheListView.ClientRectangle.IntersectsWith(TheListView.Items[ndx].Bounds));

            //Debug.Print("Last visible index = " + (ndx - 1));
            return ndx - 1;
        }

        // Called by the FilterDialog when the user clicks Apply or OK.
        public void RebuildAllRows() {
            if (_records != null && _records.Count > 0) {
                RebuildRows(0, _records[0]);
            }
        }

        #endregion

        private enum FileState { NoFile, Loading, Loaded };
        private FileState _fileState = FileState.NoFile;

        // The current log file.
        private string _filepath;

        // This watches the file for changes so new log messages can be read
        // as soon as they are written.
        private FileWatcher _watcher;

        // Helper that reads the log file.
        private Reader _reader;

        // List of records read from the log file.
        private List<Record> _records;

        // List of rows being displayed by the ListView. 
        // This gets regenerated when the filter changes and
        // when rows are expanded or collapsed.
        private List<Row> _rows;

        // The row number (scroll position) to restore after refreshing the file.
        private int _rowNumToRestore;

        // The original title to which the file name is appended to be
        // displayed in the title bar.
        private string _originalTitle;

        // Bitmask of trace levels selected to be visible (i.e. not filtered out).
        private TraceLevel _visibleTraceLevels;

        // Text search settings for F3 ("find next").
        StringMatcher _textMatcher;

        // Cache of ListViewItems used to improve the performance of the ListView control
        // in virtual mode (it tends to request the same item many times).
        private ViewItem[] _itemCache;

        // Index of first item in _itemCache.
        private int _firstItemIndex;

        // The area occupied by the ListView header.  Used to determine which column header is
        // right-clicked so the appropriate context menu can be displayed.
        private Rectangle _headerRect;

        private FileState _FileState {
            get { return _fileState; }
            set {
                _fileState = value;
                autoUpdate.Enabled = (_fileState == FileState.Loaded && _reader.FormatVersion > 5);
                propertiesCmd.Enabled = (_fileState == FileState.Loaded);
                filterDlgCmd.Enabled = (_fileState == FileState.Loaded);
                closeToolStripMenuItem.Enabled = (_fileState == FileState.Loaded);
                refreshCmd.Enabled = (_fileState == FileState.Loaded);
                expandAllButton.Enabled = (_FileState == FileState.Loaded);
                exportToCSVToolStripMenuItem.Enabled = (_FileState == FileState.Loaded);

                buttonStop.Visible = (_fileState == FileState.Loading);

                openFileCmd.Enabled = (_fileState != FileState.Loading);

                if (_fileState != FileState.Loaded) {
                    NumRows = 0;
                    _rows = null;
                    _records = null;
                    ClearItemCache();
                    toolStripProgressBar1.Value = 0;

                    crumbBar1.Clear();

                    // Some commands are disabled when filestate != Loaded, but not necessarily
                    // enabled when filestate == Loaded.
                    filterClearCmd.Enabled = false;
                    bookmarkToggleCmd.Enabled = false;
                    bookmarkNextCmd.Enabled = false;
                    bookmarkPrevCmd.Enabled = false;
                    bookmarkClearCmd.Enabled = false;
                }

                UpdateFindCommands();
            }
        }

        // This returns an array of the ColumnHeaders in the order they are
        // displayed by the ListView.  Used to determine which column header
        // was right-clicked.
        public ColumnHeader[] OrderedHeaders {
            get {
                ColumnHeader[] arr = new ColumnHeader[TheListView.Columns.Count];

                foreach (ColumnHeader col in TheListView.Columns) {
                    arr[col.DisplayIndex] = col;
                }

                return arr;
            }
        }

        private Record _FocusedRec {
            get {
                Row focusedRow = FocusedRow;
                if (focusedRow == null) {
                    return null;
                } else {
                    return focusedRow.Rec;
                }
            }
        }

        // Increment or decrement _curRow depending on searchUp and handle wrapping.
        private Row NextRow(Row curRow, bool searchUp) {
            int ndx = curRow.Index;
            if (searchUp) {
                --ndx;
                if (ndx < 0) ndx = NumRows - 1;
            } else {
                ndx = (ndx + 1) % NumRows;
            }

            return _rows[ndx];
        }

        private void SelectSingleItem(Row row) {
            TheListView.SelectedIndices.Clear();
            TheListView.EnsureVisible(row.Index);
            ListViewItem item = TheListView.Items[row.Index];
            item.Focused = true;
            item.Selected = true;
            //if (this != Form.ActiveForm) 
            //    SetItemColors(item, true);
        }

        #region File loading

        // This gets the row number to restore after refreshing the file.
        private int GetRowNumToRestore() {
            int ret = 0;
            if (TheListView.FocusedItem != null && TheListView.FocusedItem.Index == NumRows - 1) {
                ret = -1; // Special value meaning the very end.
            } else if (TheListView.TopItem != null) {
                ret = TheListView.TopItem.Index;
            }

            return ret;
        }

        // Open the specified log file and, if successfull, 
        // start the background thread that reads it.
        // A null filename means to refresh the current file.
        private void StartReading(string filename) {
            StopFileWatcher();

            bool refreshing;

            if (filename == null) {
                filename = _filepath;
                refreshing = true;
                _rowNumToRestore = GetRowNumToRestore();
            } else {
                refreshing = false;
                _rowNumToRestore = 0;
            }

            // If we can't open the new file, the old file stays loaded.
            Reader prospectiveReader = new Reader();
            if (prospectiveReader.OpenLogFile(filename)) {
                _FileState = FileState.Loading;

                if (refreshing && Settings.Default.KeepFilter) {
                    // Set _visibleTraceLevels so that any new trace levels found while reading the 
                    // file will be visible, but the ones found in the current file (ValidTraceLevels) 
                    // that are currently hidden stay hidden.
                    _visibleTraceLevels |= ~ValidTraceLevels;
                    prospectiveReader.ReuseFilters();
                } else {
                    // Show all trace levels when the new file is loaded.
                    _visibleTraceLevels |= ~_visibleTraceLevels;
                    FilterDialog.TextFilterDisable();
                }

                _reader = prospectiveReader; // Must come after references to ValidTraceLevels.
                ResetFilters();
                _filepath = filename;

                filterClearCmd.Enabled = false;
                filenameLabel.Text = filename;

                backgroundWorker1.RunWorkerAsync();
            }
        }

        // This method runs in a worker thread.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "TracerX Log Reader";

            int percent = 0;
            int lastPercent = 0;
            int rowCount = 0;
            long totalBytes = _reader.Size;

            _records = new List<Record>((int)(totalBytes / 100)); // Guess at how many records

            for (var session = _reader.NextSession(); session != null; session = _reader.NextSession()) {
                Record record = session.ReadRecord();
                
                while (record != null) {
                    rowCount += record.Lines.Length;
                    record.Index = _records.Count;
                    _records.Add(record);

                    percent = (int)((_reader.BytesRead * 100) / totalBytes);
                    if (percent != lastPercent) {
                        lastPercent = percent;
                        backgroundWorker1.ReportProgress(percent);
                    }

                    record = session.ReadRecord();

                    // This Sleep call is critical.  Without it, the main thread doesn't seem to
                    // get any cpu time and the worker thread seems to go much slower.
                    Thread.Sleep(0);

                    if (backgroundWorker1.CancellationPending) {
                        Debug.Print("Background worker was cancelled.");
                        e.Cancel = true;
                        break;
                    }
                }

                // If the log has both a circular part and a non-circular part, there may
                // be missing exit/entry records due to wrapping.
                rowCount += session.InsertMissingRecords(_records);
            }

            // The logger can't open the file if we don't close it.
            _reader.CloseLogFile();

            // Initially, there is a 1:1 relationship between each row and each record.  This will change
            // if the user collapses method calls (causing some records to be omitted from view) or
            // expands rows with embedded newlines.
            // Allocate enough rows to handle the case of all messages with embedded newlines being expanded.
            _rows = new List<Row>(rowCount);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Error == null && !e.Cancelled) {
                toolStripProgressBar1.Value = 100;
                pctLabel.Text = "100%";
            }

            _FileState = FileState.Loaded;

            AddFileToRecentlyViewed(_filepath);
            this.Text = Path.GetFileName(_filepath) + " - " + _originalTitle;

            _visibleTraceLevels &= ValidTraceLevels;
            ThreadObjects.RecountThreads();
            ThreadNames.RecountThreads();
            LoggerObjects.RecountLoggers();
            MethodObjects.RecountMethods();
            SessionObjects.RecountSessions();

            // The above calls may have added or removed a class of filtering.  Set the
            // buttons, menus, and column header images accordingly.
            FilterAddedOrRemoved(null, null);

            if (_records.Count == 0) {
                ShowMessageBox("There are no records in the file.");
                ZeroTime = _reader.CurrentSession.CreationTimeUtc;
            } else {
                ZeroTime = _records[0].Time;
                RebuildAllRows();
                RestoreScrollPosition(_rowNumToRestore);
            }

            if (Settings.Default.AutoUpdate) {
                StartFileWatcher();
            }
        }

        private void RestoreScrollPosition(int rowNum) {
            // Attempt to maintain the same scroll position as before the refresh.
            if (rowNum == -1 || rowNum >= NumRows) {
                // Go to the very end and select the last row so the next refresh will also
                // scroll to the end.
                SelectSingleRow(NumRows - 1);
            } else {
                // Scroll to the same index as before the refresh.
                // For some reason, setting the TopItem once doesn't work.  Setting
                // it three times usually does, so try up to four.
                for (int i = 0; i < 4; ++i) {
                    if (TheListView.TopItem.Index == rowNum) break;
                    TheListView.TopItem = (TheListView.Items[rowNum]);
                }
            }
        }

        private bool WatchingFile { get { return !IsDisposed && _watcher != null && !_watcher.Stopped; } }

        private void StartFileWatcher() {
            if (_reader.FormatVersion < 6) return;

            autoUpdate.Checked = true;

            if (WatchingFile) {
                Debug.Print("Already watching file.");
            } else {
                Debug.Print("Starting file watcher.");
                _watcher = new FileWatcher(_filepath);

                // The FileWatcher events are called on a worker thread.  For thread-safety, it seems
                // better to handle the events in the main GUI thread by calling Invoke() so the events
                // are handled serially.  BeginInvoke() was tried, but it caused the GUI to freeze and/or
                // flicker when the file was updated frequently.
                FileSystemEventHandler changeDel = new FileSystemEventHandler(_watcher_Changed);
                RenamedEventHandler renameDel = new RenamedEventHandler(_watcher_Renamed);

                _watcher.Changed += (sender, e) => { if (!IsDisposed) Invoke(changeDel, sender, e); };
                _watcher.Renamed += (sender, e) => { if (!IsDisposed) Invoke(renameDel, sender, e); };

                // Simulate the Changed event in case we missed any changes while we weren't watching.
                _watcher_Changed(null, new FileSystemEventArgs(WatcherChangeTypes.Changed, Path.GetDirectoryName(_filepath), Path.GetFileName(_filepath)));
            }
        }

        private void StopFileWatcher() {
            if (!InvokeRequired) autoUpdate.Checked = false;

            if (WatchingFile) {
                // This seems backwards, but we do not want to call _watcher.Dispose() in the main
                // GUI thread.  Doing so leads to deadlocks.  The typical scenario is that the _watcher.Change
                // or _watcher.Rename event is called on a worker thread, which then calls Invoke() to
                // do most of the processing in the GUI thread.  The GUI thread may call StopFileWatcher(), which 
                // calls _watcher.Dispose, which blocks until the event handler returns.  However, the event
                // handler thread is waiting for the Invoke() call to return, which is stuck in Dispose().
                if (InvokeRequired) {
                    Debug.Print("Stopping watcher.");
                    _watcher.Dispose();
                    _watcher = null;
                } else {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(StopFileWatcher));
                }
            }
        }

        private void StopFileWatcher(object o) {
            StopFileWatcher();
        }

        void _watcher_Changed(object sender, FileSystemEventArgs e) {
            if (!WatchingFile) return;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Debug.Print(e.Name + " " + e.ChangeType);

            if (e.ChangeType == WatcherChangeTypes.Changed) {
                var scrollPos = GetRowNumToRestore();
                var originalCount = _records.Count;
                var originalTraceLevels = _reader.LevelsFound;
                var session = _reader.CurrentSession;
                var originalLastNonCircularRecord = session.LastNonCircularRecord;
                Record firstGeneratedRec = null;
                bool firstIteration = true;

                while (WatchingFile && session != null) {
                    var newRecords = session.ReadMoreRecords();

                    if (firstIteration) {
                        firstIteration = false;
                        firstGeneratedRec = session.GeneratedRecords.FirstOrDefault();
                        //Record firstGeneratedRec = session.GeneratedRecords.Count > 0 ? session.GeneratedRecords[0] : null;
                    }

                    if (newRecords != null) {
                        Debug.Print("{0} records exist, {1} new records read.", _records.Count, newRecords.Count);

                        if (newRecords.Count > 0) {
                            // Set the collapse depth of the new and generated records.
                            foreach (Record rec in newRecords) SetCollapseDepth(rec);
                            foreach (Record rec in session.GeneratedRecords) SetCollapseDepth(rec);

                            for (int i = 0; i < newRecords.Count; ++i) newRecords[i].Index = i + _records.Count;
                            _records.AddRange(newRecords);
                            session.InsertMissingRecords(_records);
                        } else {
                            // Nothing to do.
                            Debug.Print("0 new records read.");
                        }
                    } else {
                        autoUpdate.Enabled = false;
                        StopFileWatcher();
                        ShowMessageBox("An error occurred reading the log file.  The logger probably overwrote the location currently being read.  The file will no longer be monitored for changes unless you reload/refresh it.");
                    }

                    session = _reader.NextSession();
                } // while

                if (_records.Count > originalCount) {
                    // Set _visibleTraceLevels so that any new trace levels found while reading the 
                    // file will be visible, but the ones originally hidden stay hidden. 
                    _visibleTraceLevels |= originalTraceLevels ^ _reader.LevelsFound;

                    if (originalCount == 0) {
                        Debug.Print("Rebuilding all rows.");
                        RebuildAllRows();
                    } else {
                        if (firstGeneratedRec == null) {
                            // Since no generated records were inserted in the first iteration, we will rebuild the rows starting
                            // with the first new one.
                            RebuildRows(NumRows, _records[originalCount]);
                        } else {
                            int visibleRecNdx = FindFirstVisibleUp(originalLastNonCircularRecord.Index);

                            if (visibleRecNdx == -1) {
                                RebuildRows(0, firstGeneratedRec);
                            } else {
                                RebuildRows(_records[visibleRecNdx].LastRowIndex + 1, firstGeneratedRec);
                            }
                        }

                        RestoreScrollPosition(scrollPos);
                    }
                }

                Debug.Print("Time to handle new records = {0}ms.", sw.ElapsedMilliseconds);
            } else {
                // Stop watching when file is renamed, replaced, etc.
                autoUpdate.Enabled = false;
                StopFileWatcher();
                ShowMessageBox("The log file has been renamed, replaced, or deleted.  It will no longer be monitored for changes unless you reload/refresh it.");
            }
        }

        void _watcher_Renamed(object sender, RenamedEventArgs e) {
            Debug.Print(e.OldName + " renamed to " + e.Name + "(" + e.ChangeType + ")");
            autoUpdate.Enabled = false;
            StopFileWatcher();
            ShowMessageBox("The log file has been renamed.  It will no longer be monitored for changes unless you reload/refresh it.");
        }

        private void SetCollapseDepth(Record rec) {
            rec.CollapsedDepth = 0;
            Record caller = rec.Caller;

            while (caller != null) {
                if (caller.IsCollapsed) {
                    ++rec.CollapsedDepth;
                }

                caller = caller.Caller;
            }
        }

        private static void ResetFilters() {
            lock (ThreadObjects.Lock) {
                ThreadObjects.AllThreadObjects = new List<ThreadObject>();
            }

            lock (ThreadNames.Lock) {
                ThreadNames.AllThreadNames = new List<ThreadName>();
            }

            lock (LoggerObjects.Lock) {
                LoggerObjects.AllLoggers = new List<LoggerObject>();
            }

            lock (MethodObjects.Lock) {
                MethodObjects.AllMethods = new List<MethodObject>();
            }

            lock (SessionObjects.Lock) {
                SessionObjects.AllSessionObjects = new List<Reader.Session>();
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            int percent = Math.Min(100, e.ProgressPercentage);
            toolStripProgressBar1.Value = percent;
            pctLabel.Text = e.ProgressPercentage.ToString() + "%";
        }

        #endregion

        // Increment or decrement the CollapsedDepth of every record between this one and 
        // the corresponding Exit record (for the same thread).
        private void ExpandCollapseMethod(Record methodEntryRecord) {
            short diff = (short)(methodEntryRecord.IsCollapsed ? -1 : 1);

            methodEntryRecord.IsCollapsed = !methodEntryRecord.IsCollapsed;

            for (int i = methodEntryRecord.Index + 1; i < _records.Count; ++i) {
                Record current = _records[i];
                if (current.ThreadId == methodEntryRecord.ThreadId) {
                    // Current record is from the same thread as the clicked record.
                    if (current.StackDepth == methodEntryRecord.StackDepth) {
                        // This is the Exit record.
                        break;
                    } else {
                        current.CollapsedDepth += diff;
                    }
                }
            }
        }

        private void expandAllButton_Click(object sender, EventArgs e) {
            foreach (Record rec in _records) {
                rec.IsCollapsed = false;
                rec.CollapsedDepth = 0;
            }

            RebuildAllRows();
        }

        // Reset the _rows elements from startRow forward.
        // The specified Record (rec) is the first Record whose
        // visibility may need recalculating.  The specified
        // startRow will be mapped to the first visible Record found.
        private void RebuildRows(int startRow, Record rec) {
            Debug.Print("RebuildRows entered");

            // Display the wait cursor while we process the rows.
            Cursor restoreCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            // Try to restore the scroll position when we're finished processing.
            Record showRec = null; // The record that has the focus.
            int showLine = 0;      // The line within the record that has the focus.
            int offset = 0;        // The offset of the focused row from the top row.
            if (TheListView.TopItem != null && FocusedRow != null) {
                showRec = FocusedRow.Rec;
                showLine = FocusedRow.Line;
                offset = FocusedRow.Index - TheListView.TopItem.Index;
            }

            int curRow = startRow;
            int curRec = rec.Index;

            while (curRec < _records.Count) {
                curRow = _records[curRec].SetVisibleRows(_rows, curRow);
                ++curRec;
            }

            ClearItemCache();
            NumRows = curRow;

            if (showRec != null) {
                int showRow = showRec.RowIndices[showLine];

                // If showRec is no longer visible, find the nearest record that is.
                while (!showRec.IsVisible && showRec.Index > 0) {
                    showRec = _records[showRec.Index - 1];
                    showRow = showRec.FirstRowIndex;
                }

                // If we found a visible record, scroll to it.
                if (showRec.IsVisible) {
                    if (showRow == -1 || !showRec.HasNewlines || showRec.IsCollapsed)
                        showRow = showRec.FirstRowIndex;

                    //Debug.Print("selecting item " + showRow);
                    SelectSingleRow(showRow);
                    int top = showRow - offset;
                    if (top > 0) {
                        //Debug.Print("setting top " + top);
                        TheListView.TopItem = TheListView.Items[top];
                    }
                }
            }

            this.Cursor = restoreCursor;
            Debug.Print("RebuildRows exiting");
        }

        private int FindFirstVisibleUp(int recNdx) {
            while (recNdx >= 0 && !_records[recNdx].IsVisible) {
                --recNdx;
            }

            return recNdx;
        }

        protected override void OnClosing(CancelEventArgs e) {
            StopFileWatcher();

            // Persist column widths in Settings.
            int[] widths = new int[OriginalColumns.Length];
            int[] indices = new int[OriginalColumns.Length];
            bool[] selections = new bool[OriginalColumns.Length];
            for (int i = 0; i < OriginalColumns.Length; ++i) {
                widths[i] = OriginalColumns[i].Width;
                indices[i] = OriginalColumns[i].DisplayIndex;
                selections[i] = TheListView.Columns.Contains(OriginalColumns[i]);
            }
            Settings.Default.ColWidths = widths;
            Settings.Default.ColIndices = indices;
            Settings.Default.ColSelections = selections;
            Settings.Default.Save();

            base.OnClosing(e);
        }

        private ViewItem GetListItem(int i, out bool newItem) {
            // If we have the item cached, return it. Otherwise, recreate it.
            if (_itemCache != null &&
                i >= _firstItemIndex &&
                i < _firstItemIndex + _itemCache.Length) {
                //Debug.Print("Returning cached item " + i);
                newItem = false;
                return _itemCache[i - _firstItemIndex];
            } else {
                // Create a new item.
                newItem = true;

                if (i == 0) {
                    return _rows[i].MakeItem(null);
                } else {
                    return _rows[i].MakeItem(_rows[i - 1]);
                }
            }
        }

        private void listView1_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e) {
            // Only recreate the cache if we need to.
            if (_itemCache != null) {
                if (e.StartIndex >= _firstItemIndex &&
                    e.EndIndex <= _firstItemIndex + _itemCache.Length) {
                    return;
                }
            }

            //Debug.Print("Building item cache " + e.StartIndex + " - " + e.EndIndex);
            bool newItem;
            ViewItem[] newCache = new ViewItem[e.EndIndex - e.StartIndex + 1];
            for (int i = 0; i < newCache.Length; i++) {
                // This will copy items from the old cache if it overlaps the new one.
                newCache[i] = GetListItem(e.StartIndex + i, out newItem);
            }

            _firstItemIndex = e.StartIndex;
            _itemCache = newCache;
        }

        private void ExecuteOpenFile(object sender, EventArgs e) {
            string openDir = Settings.Default.OpenDir;
            OpenFileDialog dlg = new OpenFileDialog();

            if (openDir != null && openDir != string.Empty && Directory.Exists(openDir))
                dlg.InitialDirectory = openDir;
            else
                dlg.InitialDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);

            dlg.AddExtension = true;
            dlg.DefaultExt = ".tx1";
            dlg.Filter = "TracerX files (*.tx1)|*.tx1|All files (*.*)|*.*";
            dlg.Multiselect = false;
            dlg.Title = Application.ProductName;

            if (DialogResult.OK == dlg.ShowDialog()) {
                Settings.Default.OpenDir = Path.GetDirectoryName(dlg.FileName);
                StartReading(dlg.FileName);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e) {
            StopFileWatcher();
            _FileState = FileState.NoFile;
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e) {
            backgroundWorker1.CancelAsync();
        }

        private void TheListView_MouseDoubleClick(object sender, MouseEventArgs e) {
            ListViewHitTestInfo hitTestInfo = TheListView.HitTest(e.X, e.Y);
            if (hitTestInfo != null && hitTestInfo.Item != null) {
                DateTime start = DateTime.Now;
                ViewItem item = hitTestInfo.Item as ViewItem;
                Row row = item.Row;
                Record record = row.Rec;

                this.TheListView.BeginUpdate();

                if (record.IsEntry) {
                    // Expand or collapse the method call.
                    ExpandCollapseMethod(record);
                    RebuildRows(row.Index, row.Rec);
                } else if (record.HasNewlines && row.Index == record.FirstRowIndex) {
                    // Expand or collapse the record with embedded newlines.
                    record.IsCollapsed = !record.IsCollapsed;
                    RebuildRows(row.Index, row.Rec);
                } else {
                    // Display the record text in a window.
                    row.ShowFullText();
                }

                this.TheListView.EndUpdate();
                //Debug.Print("Double click handled, rows = " + NumRows + ", time = " + (DateTime.Now - start));
            }

        }

        private void ExecuteProperties(object sender, EventArgs e) {
            FileProperties dlg = new FileProperties(_reader);
            dlg.ShowDialog();
        }

        #region Bookmarks
        private void ExecuteToggleBookmark(object sender, EventArgs e) {
            if (TheListView.FocusedItem != null) {
                Row row = TheListView.FocusedItem.Tag as Row;
                row.IsBookmarked = !row.IsBookmarked;

                if (row.IsBookmarked) {
                    bookmarkPrevCmd.Enabled = true;
                    bookmarkNextCmd.Enabled = true;
                    bookmarkClearCmd.Enabled = true;
                }

                // row.ImageIndex is determined by IsBookmarked.
                TheListView.FocusedItem.ImageIndex = row.ImageIndex;
                TheListView.Invalidate(TheListView.FocusedItem.GetBounds(ItemBoundsPortion.Icon));
            }
        }

        private void ExecuteClearBookmarks(object sender, EventArgs e) {
            // We must visit every record, including those that are collapsed/hidden.
            foreach (Record rec in _records) {
                for (int i = 0; i < rec.IsBookmarked.Length; ++i) {
                    rec.IsBookmarked[i] = false;
                }
            }

            bookmarkPrevCmd.Enabled = false;
            bookmarkNextCmd.Enabled = false;
            bookmarkClearCmd.Enabled = false;

            InvalidateTheListView();
        }

        // Search for a bookmarked row from start to just before end.
        private bool FindBookmark(int start, int end) {
            int moveBy = (start < end) ? 1 : -1;
            for (int i = start; i != end; i += moveBy) {
                if (_rows[i].IsBookmarked) {
                    SelectSingleRow(i);

                    // Commented out old behavior that just focused the row, but did
                    // not select it.
                    //TheListView.EnsureVisible(i);
                    //if (TheListView.FocusedItem != null) TheListView.FocusedItem.Focused = false;
                    //TheListView.Items[i].Focused = true;

                    return true;
                }
            }

            return false;
        }

        private void ExecuteNextBookmark(object sender, EventArgs e) {
            int start = 0;

            if (TheListView.FocusedItem != null) {
                start = TheListView.FocusedItem.Index + 1;
            }

            if (!FindBookmark(start, NumRows)) {
                // Wrap back to the first row.
                FindBookmark(0, start);
            }
        }

        private void ExecutePrevBookmark(object sender, EventArgs e) {
            int start = NumRows - 1;

            if (TheListView.FocusedItem != null) {
                start = TheListView.FocusedItem.Index - 1;
            }

            if (!FindBookmark(start, -1)) {
                // Wrap back to the last row.
                FindBookmark(NumRows - 1, start);
            }
        }
        #endregion Bookmarks

        // Display the Find dialog.
        private void ExecuteFind(object sender, EventArgs e) {
            FindDialog dlg = new FindDialog(this);
            dlg.Show(this);
        }

        // Search down for the current search string.
        private void ExecuteFindNext(object sender, EventArgs e) {
            if (_textMatcher != null) DoSearch(_textMatcher, false, false);
        }

        // Search up for the current search string.
        private void ExecuteFindPrevious(object sender, EventArgs e) {
            if (_textMatcher != null) DoSearch(_textMatcher, true, false);
        }

        // Clear all filtering.
        private void ExecuteClearFilter(object sender, EventArgs e) {
            SessionObjects.ShowAllSessions();
            ThreadObjects.ShowAllThreads();
            ThreadNames.ShowAllThreads();
            LoggerObjects.ShowAllLoggers();
            MethodObjects.ShowAllMethods();
            VisibleTraceLevels = _reader.LevelsFound;
            FilterDialog.TextFilterDisable();
            RebuildAllRows();
        }

        // Called when the first filter is added or the last filter is
        // removed for a class of objects such as loggers or threads,
        // or whenever the presence/status of the filter icons that appear
        // in the column headers and the "clear all filtering" commands 
        // may need to be updated.
        private void FilterAddedOrRemoved(object sender, EventArgs e) {
            filterClearCmd.Enabled = false;

            if (VisibleTraceLevels == ValidTraceLevels) {
                headerLevel.ImageIndex = -1;
            } else {
                headerLevel.ImageIndex = 9;
                filterClearCmd.Enabled = true;
            }

            if (SessionObjects.AllVisible) {
                headerSession.ImageIndex = -1;
            } else {
                // Show a filter in the header so user knows a filter is applied to the column.
                headerSession.ImageIndex = 9;
                filterClearCmd.Enabled = true;
            }

            if (ThreadNames.AllVisible) {
                headerThreadName.ImageIndex = -1;
            } else {
                // Show a filter in the header so user knows a filter is applied to the column.
                headerThreadName.ImageIndex = 9;
                filterClearCmd.Enabled = true;
            }

            if (ThreadObjects.AllVisible) {
                headerThreadId.ImageIndex = -1;
            } else {
                // Show a filter in the header so user knows a filter is applied to the column.
                headerThreadId.ImageIndex = 9;
                filterClearCmd.Enabled = true;
            }

            if (LoggerObjects.AllVisible) {
                headerLogger.ImageIndex = -1;
            } else {
                // Show a filter in the header so user knows a filter is applied to the column.
                headerLogger.ImageIndex = 9;
                filterClearCmd.Enabled = true;
            }

            if (MethodObjects.AllVisible) {
                headerMethod.ImageIndex = -1;
            } else {
                // Show a filter in the header so user knows a filter is applied to the column.
                headerMethod.ImageIndex = 9;
                filterClearCmd.Enabled = true;
            }

            if (FilterDialog.TextFilterOn) {
                // Show a filter in the header so user knows a filter is applied to the column.
                headerText.ImageIndex = 9;
                filterClearCmd.Enabled = true;
            } else {
                headerText.ImageIndex = -1;
            }
        }

        // Hide selected thread names
        private void hideSelectedThreadNamesMenuItem_Click(object sender, EventArgs e) {
            foreach (int index in TheListView.SelectedIndices) {
                _rows[index].Rec.ThreadName.Visible = false;
            }

            RebuildAllRows();

        }

        // Show only selected thread names
        private void showSelectedThreadNamesMenuItem_Click(object sender, EventArgs e) {
            ThreadNames.HideAllThreads();

            foreach (int index in TheListView.SelectedIndices) {
                _rows[index].Rec.ThreadName.Visible = true;
            }

            RebuildAllRows();
        }

        // Hide selected thread IDs.
        private void hideSelectedThreadsMenuItem_Click(object sender, EventArgs e) {
            foreach (int index in TheListView.SelectedIndices) {
                _rows[index].Rec.Thread.Visible = false;
            }

            RebuildAllRows();
        }

        // Hide all but the selected thread IDs.
        private void showSelectedThreadsMenuItem_Click(object sender, EventArgs e) {
            ThreadObjects.HideAllThreads();

            foreach (int index in TheListView.SelectedIndices) {
                _rows[index].Rec.Thread.Visible = true;
            }

            RebuildAllRows();
        }

        // Show the column selection dialog.
        private void ExecuteColumns(object sender, EventArgs e) {
            ColumnsDlg dlg = new ColumnsDlg(this);
            dlg.ShowDialog(this);
        }

        private void HideSelectedLoggersMenuItem_Click(object sender, EventArgs e) {
            foreach (int index in TheListView.SelectedIndices) {
                _rows[index].Rec.Logger.Visible = false;
            }

            RebuildAllRows();

        }

        private void ShowSelectedLoggersMenuItem_Click(object sender, EventArgs e) {
            LoggerObjects.HideAllLoggers();

            foreach (int index in TheListView.SelectedIndices) {
                _rows[index].Rec.Logger.Visible = true;
            }

            RebuildAllRows();

        }

        // Hide rows with the same TraceLevel as the selected rows.
        private void HideTraceLevelsMenuItem_Click(object sender, EventArgs e) {
            TraceLevel newSetting = VisibleTraceLevels;

            foreach (int index in TheListView.SelectedIndices) {
                newSetting &= ~(_rows[index].Rec.Level);
            }

            VisibleTraceLevels = newSetting;
            RebuildAllRows();
        }


        // Show only the rows that have the same TraceLevel as the selected rows.
        private void ShowTraceLevelsMenuItem_Click(object sender, EventArgs e) {
            TraceLevel newSetting = TraceLevel.Inherited; // I.e. none.

            foreach (int index in TheListView.SelectedIndices) {
                newSetting |= _rows[index].Rec.Level;
            }

            // It's possible the selected trace levels are already the only
            // ones visible, in which case do nothing.
            if (VisibleTraceLevels != newSetting) {
                // Some trace levels got removed.
                VisibleTraceLevels = newSetting;
                RebuildAllRows();
            }
        }

        // Display the call stack of the selected record.
        private void showCallStackMenuItem_Click(object sender, EventArgs e) {
            // Search for the callers (call stack) of the currently selected record by
            // looking at the StackDepth of preceding records from the same thread.
            // This was implemented before the Caller field was added to the Record class,
            // and works for file versions before 5.

            // This is the list of records that comprise the stack.
            List<Record> stack = new List<Record>();
            Row startRow = _rows[TheListView.SelectedIndices[0]];
            Cursor restoreCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            try {
                Record curRec = FindCaller(startRow.Rec);

                while (curRec != null) {
                    stack.Add(curRec);
                    curRec = FindCaller(curRec);
                }
            } finally {
                this.Cursor = restoreCursor;
            }

            stack.Reverse();
            CallStack dlg = new CallStack(startRow, stack);
            dlg.ShowDialog(this);
        }

        private Record FindCaller(Record start) {
            // We are only interested in records whose stack depth is less than curDepth.
            int curDepth = start.StackDepth;

            if (start.IsExit) ++curDepth;

            for (int index = start.Index - 1; index > -1 && curDepth > 0; --index) {
                Record curRec = _records[index];
                if (curRec.Thread == start.Thread) {
                    if (curRec.StackDepth < curDepth) {
                        curDepth = curRec.StackDepth;
                        if (curRec.IsEntry) {
                            return curRec;
                        }
                    }
                }
            }

            // Did not find the caller.
            return null;
        }

        // Scroll to the start of the method for the current record if visible.
        private void startOfMethodMenuItem_Click(object sender, EventArgs e) {
            Row startRow = _rows[TheListView.SelectedIndices[0]];
            Record caller;
            Cursor restoreCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            try {
                caller = FindCaller(startRow.Rec);
            } finally {
                this.Cursor = restoreCursor;
            }

            if (caller == null) {
                // The caller was possibly lost in circular wrapping.
                ShowMessageBox("The caller was not found.");
            } else if (caller.IsVisible) {
                // Scroll to and select the row/item for the caller record.
                this.SelectSingleRow(caller.FirstRowIndex);
            } else {
                ShowMessageBox("The caller was found, but is not visible due to filtering.");
            }
        }

        // Find the end of the current method call and scroll to it.
        private void endOfMethodMenuItem_Click(object sender, EventArgs e) {
            Record startRec = CurrentRow.Rec;
            Cursor restoreCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            // If the current record is a MethodEntry record, the end of the method is the next
            // record at the same stack depth.  Otherwise, the end of the method is the next
            // record at a lesser stack depth.
            int triggerDepth = startRec.StackDepth;

            if (startRec.IsEntry) ++triggerDepth;

            try {
                for (int index = startRec.Index + 1; index < _records.Count; ++index) {
                    Record curRec = _records[index];
                    if (curRec.Thread == startRec.Thread) {
                        if (curRec.StackDepth < triggerDepth) {
                            if (curRec.IsVisible) {
                                this.SelectSingleRow(curRec.FirstRowIndex);
                            } else {
                                ShowMessageBox("The end of the method call was found, but is not visible due to filtering.");
                            }
                            return;
                        }
                    }
                }

                ShowMessageBox("The end of the method call was not found.");
            } finally {
                this.Cursor = restoreCursor;
            }
        }

        private void copyTextMenuItem_Click(object sender, EventArgs e) {
            StringBuilder builder = new StringBuilder();
            foreach (int index in TheListView.SelectedIndices) {
                ViewItem item = TheListView.Items[index] as ViewItem;
                builder.Append(item.Row.GetFullIndentedText());
                builder.Append("\n");
            }

            // Remove last newline.
            if (builder.Length > 0) {
                builder.Length = builder.Length - 1;
                Clipboard.SetText(builder.ToString());
            }
        }

        private void UpdateMenuItems() {
            int selectCount = TheListView.SelectedIndices.Count;
            Record currentRec = CurrentRow == null ? null : CurrentRow.Rec;

            Debug.Print("selectCount: " + selectCount);

            hideSelectedToolStripMenuItem.Enabled = selectCount > 0;
            showOnlySelectedToolStripMenuItem.Enabled = selectCount > 0;
            copyTextMenuItem.Enabled = selectCount > 0;
            copyColsMenuItem.Enabled = selectCount > 0;
            bookmarkSelectedMenuItem.Enabled = selectCount > 0;
            showCallStackMenuItem.Enabled = selectCount == 1;
            startOfMethodMenuItem.Enabled = selectCount == 1;
            endOfMethodMenuItem.Enabled = selectCount == 1;
            viewTextWindowToolStripMenuItem.Enabled = selectCount == 1;
            setZeroTimeToolStripMenuItem.Enabled = selectCount == 1;

            if (selectCount == 1) {
                if (currentRec.StackDepth == 0) {
                    showCallStackMenuItem.Enabled = currentRec.IsExit;
                    startOfMethodMenuItem.Enabled = currentRec.IsExit;
                    endOfMethodMenuItem.Enabled = currentRec.IsEntry;
                }
            }

            // Try to include the method name in the "Goto start of method" menu item.
            if (startOfMethodMenuItem.Enabled) {
                if (currentRec.IsEntry) {
                    // In file format 5, Record.Caller is a direct reference to the caller of the 
                    // current line.  If that's available, use it.  If not, don't take the time to
                    // search for the caller just to set the menu item text.

                    if (currentRec.Caller == null) {
                        startOfMethodMenuItem.Text = "Start of caller";
                    } else {
                        startOfMethodMenuItem.Text = "Start of caller: " + currentRec.Caller.MethodName.Name;
                    }
                } else {
                    startOfMethodMenuItem.Text = "Start of method: " + currentRec.MethodName.Name;
                }
            } else {
                startOfMethodMenuItem.Text = "Start of method";
            }

            // Try to include the method name in the "Goto end of method" menu item.
            if (endOfMethodMenuItem.Enabled) {
                if (currentRec.IsExit) {
                    // We're already at the end of the current method, so this command will actually
                    // scroll to the end of the caller.  Try to get the caller's name.

                    if (currentRec.Caller == null || currentRec.Caller.Caller == null) {
                        endOfMethodMenuItem.Text = "End of caller";
                    } else {
                        endOfMethodMenuItem.Text = "End of caller: " + currentRec.Caller.Caller.MethodName.Name;
                    }
                } else {
                    endOfMethodMenuItem.Text = "End of method: " + currentRec.MethodName.Name;
                }
            } else {
                endOfMethodMenuItem.Text = "End of method";
            }

            UpdateUncolorMenu();
        }

        // Enable or disable find, find next, find prev.  It is not sufficient to only do this when 
        // the Edit menu is opening because these commands also have shortcut keys and toolbar buttons.
        private void UpdateFindCommands() {
            findCmd.Enabled = _FileState == FileState.Loaded && NumRows > 0;
            findNextCmd.Enabled = findPrevCmd.Enabled = (findCmd.Enabled && _textMatcher != null);
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            UpdateMenuItems();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e) {
            // This call indirectly calls EnumWindowCallBack which sets _headerRect
            // to the area occupied by the ListView's header bar.
            NativeMethods.EnumChildWindows(TheListView.Handle, new NativeMethods.EnumWinCallBack(EnumWindowCallBack), IntPtr.Zero);

            // If the mouse position is in the header bar, cancel the display
            // of the regular context menu and display the column header context menu instead.
            if (_headerRect.Contains(Control.MousePosition)) {
                e.Cancel = true;

                // The xoffset is how far the mouse is from the left edge of the header.
                int xoffset = Control.MousePosition.X - _headerRect.Left;

                // Iterate through the columns in the order they are displayed, adding up
                // their widths as we go.  When the sum exceeds the xoffset, we know the mouse
                // is on the current column. 
                int sum = 0;
                foreach (ColumnHeader col in OrderedHeaders) {
                    sum += col.Width;
                    if (sum > xoffset) {
                        ShowMenuForColumnHeader(col);
                        break;
                    }
                }
            } else {
                // Update the items in the default context menu and allow it to be displayed.
                UpdateMenuItems();
            }
        }

        // This should get called with the only child window of the listview,
        // which should be the header bar.
        private bool EnumWindowCallBack(IntPtr hwnd, IntPtr lParam) {
            // Determine the rectangle of the header bar and save it in a member variable.
            NativeMethods.RECT rct;

            if (!NativeMethods.GetWindowRect(hwnd, out rct)) {
                _headerRect = Rectangle.Empty;
            } else {
                _headerRect = new Rectangle(rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top);
            }
            return false; // Stop the enum
        }

        // Keep certain commands enabled while the menu is closed
        private void editToolStripMenuItem_DropDownClosed(object sender, EventArgs e) {
            copyTextMenuItem.Enabled = true;
            copyColsMenuItem.Enabled = true;
        }

        private void copyColsMenuItem_Click(object sender, EventArgs e) {
            StringBuilder builder = new StringBuilder();
            string[] fields = new string[TheListView.Columns.Count];

            foreach (int index in TheListView.SelectedIndices) {
                ListViewItem item = TheListView.Items[index];
                foreach (ColumnHeader hdr in TheListView.Columns) {
                    if (hdr == headerText) {
                        // This is a special case because the text 
                        // message might be truncated in the ListView.
                        fields[hdr.DisplayIndex] = _rows[index].GetFullIndentedText();
                    } else {
                        fields[hdr.DisplayIndex] = item.SubItems[hdr.Index].Text;
                    }
                }

                foreach (string str in fields) {
                    builder.Append(str);
                    builder.Append(", ");
                }

                // Replace the last ", " with a newline.
                builder.Length = builder.Length - 2;
                builder.Append("\n");
            }

            // Remove last newline.
            if (builder.Length > 0) {
                builder.Length = builder.Length - 1;
                Clipboard.SetText(builder.ToString());
            }

        }

        // Bookmark all records/rows associated with the selected threads.
        private void bookmarkThreadsMenuItem_Click(object sender, EventArgs e) {
            // First make a list of the selected threads.
            List<ThreadObject> threads = new List<ThreadObject>();
            foreach (int index in TheListView.SelectedIndices) {
                threads.Add(_rows[index].Rec.Thread);
            }

            // Now set the IsBookmarked flag for every line of every record whose
            // thread is in the list we just made.
            bool bookmarked = false;
            foreach (Record rec in _records) {
                if (threads.Contains(rec.Thread)) {
                    for (int i = 0; i < rec.IsBookmarked.Length; ++i) {
                        rec.IsBookmarked[i] = true;
                        bookmarked = true;
                    }
                }
            }

            if (bookmarked) {
                bookmarkPrevCmd.Enabled = true;
                bookmarkNextCmd.Enabled = true;
                bookmarkClearCmd.Enabled = true;
            }

            // We're not hiding or showing anything, so don't call RebuildRows.
            // Just make sure any visible rows get their images redrawn.
            InvalidateTheListView();
        }

        private void bookmarkLoggersMenuItem_Click(object sender, EventArgs e) {
            // First make a list of the selected loggers.
            List<LoggerObject> loggers = new List<LoggerObject>();
            foreach (int index in TheListView.SelectedIndices) {
                loggers.Add(_rows[index].Rec.Logger);
            }

            // Now set the IsBookmarked flag for every line of every record whose
            // logger is in the list we just made.
            bool bookmarked = false;
            foreach (Record rec in _records) {
                if (loggers.Contains(rec.Logger)) {
                    for (int i = 0; i < rec.IsBookmarked.Length; ++i) {
                        rec.IsBookmarked[i] = true;
                        bookmarked = true;
                    }
                }
            }

            if (bookmarked) {
                bookmarkPrevCmd.Enabled = true;
                bookmarkNextCmd.Enabled = true;
                bookmarkClearCmd.Enabled = true;
            }

            // We're not hiding or showing anything, so don't call RebuildRows.
            // Just make sure any visible rows get their images redrawn.
            InvalidateTheListView();

        }

        private void bookmarkTraceLevelsMenuItem_Click(object sender, EventArgs e) {
            TraceLevel levels = TraceLevel.Inherited; // I.e. none.

            foreach (int index in TheListView.SelectedIndices) {
                levels |= _rows[index].Rec.Level;
            }

            // Now set the IsBookmarked flag for every line of every record whose
            // trace level is in the bitmask we just made.
            bool bookmarked = false;
            foreach (Record rec in _records) {
                if ((rec.Level & levels) != TraceLevel.Inherited) {
                    for (int i = 0; i < rec.IsBookmarked.Length; ++i) {
                        rec.IsBookmarked[i] = true;
                        bookmarked = true;
                    }
                }
            }

            if (bookmarked) {
                bookmarkPrevCmd.Enabled = true;
                bookmarkNextCmd.Enabled = true;
                bookmarkClearCmd.Enabled = true;
            }

            // We're not hiding or showing anything, so don't call RebuildRows.
            // Just make sure any visible rows get their images redrawn.
            InvalidateTheListView();
        }

        private void ExecuteRefresh(object sender, EventArgs e) {
            Debug.Print("Refresh");
            ReportCurrentRow();
            StartReading(null); // Null means refresh the current file.
        }

        private void ReportCurrentRow() {
            Debug.Print("Selected count = " + TheListView.SelectedIndices.Count);
            Debug.Print("FocusedItem index = " + (TheListView.FocusedItem == null ? "null" : TheListView.FocusedItem.Index.ToString()));
            Debug.Print("CurrentRow = " + (CurrentRow == null ? "null" : CurrentRow.Index.ToString()));
        }

        private void viewTextWindowToolStripMenuItem_Click(object sender, EventArgs e) {
            Row row = _rows[TheListView.SelectedIndices[0]];
            row.ShowFullText();
        }

        private void ExecuteOpenFilterDialog(object sender, EventArgs e) {
            FilterDialog dialog = new FilterDialog();
            dialog.ShowDialog(this);
        }

        private void ExecuteOptions(object sender, EventArgs e) {
            OptionsDialog dlg = new OptionsDialog();
            dlg.ShowDialog(this);
        }

        private void setZeroTimeToolStripMenuItem_Click(object sender, EventArgs e) {
            Row row = _rows[TheListView.SelectedIndices[0]];
            ZeroTime = row.Rec.Time;
            Settings.Default.RelativeTime = true;
        }

        #region Column header context menu
        // Called when a column header is left-clicked
        private void TheListView_ColumnClick(object sender, ColumnClickEventArgs e) {
            ColumnHeader header = TheListView.Columns[e.Column];
            ShowMenuForColumnHeader(header);
        }

        // Shows the context menu for the specified column header.
        private void ShowMenuForColumnHeader(ColumnHeader header) {
            //First, enable the appropriate menu items for the specified header.
            MaybeEnableRemoveFromFilter(header);
            MaybeEnableFilter(header);
            MaybeEnableOptions(header);

            // Set the context menu tag to the specified header so the handler for 
            // whatever command the user clicks can know the column.
            columnContextMenu.Tag = header;
            columnContextMenu.Show(Control.MousePosition);
        }

        private void MaybeEnableOptions(ColumnHeader header) {
            if (_reader == null) {
                this.colMenuOptionsItem.Enabled = false;
            } else if (header == this.headerLine ||
                       header == this.headerTime ||
                       header == this.headerText) //
            {
                this.colMenuOptionsItem.Enabled = true;
            } else {
                this.colMenuOptionsItem.Enabled = false;
            }
        }

        private void MaybeEnableFilter(ColumnHeader header) {
            if (_reader == null) {
                this.colMenuFilterItem.Enabled = false;
            } else if (header == this.headerLevel ||
                       header == this.headerLogger ||
                       header == this.headerSession ||
                       header == this.headerMethod ||
                       header == this.headerThreadName ||
                       header == this.headerText ||
                       header == this.headerThreadId) //
            {
                this.colMenuFilterItem.Enabled = true;
            } else {
                this.colMenuFilterItem.Enabled = false;
            }
        }

        private void MaybeEnableRemoveFromFilter(ColumnHeader header) {
            colMenuRemoveItem.Enabled = false;

            if (header == this.headerThreadId && !ThreadObjects.AllVisible) {
                colMenuRemoveItem.Enabled = true;
            }

            if (header == this.headerThreadName && !ThreadNames.AllVisible) {
                colMenuRemoveItem.Enabled = true;
            }

            if (header == this.headerLogger && !LoggerObjects.AllVisible) {
                colMenuRemoveItem.Enabled = true;
            }

            if (header == this.headerMethod && !MethodObjects.AllVisible) {
                colMenuRemoveItem.Enabled = true;
            }

            if (header == this.headerSession && !SessionObjects.AllVisible) {
                colMenuRemoveItem.Enabled = true;
            }

            if (header == this.headerLevel && _reader != null &&
                ((VisibleTraceLevels & _reader.LevelsFound) != _reader.LevelsFound)) {
                colMenuRemoveItem.Enabled = true;
            }

            if (header == this.headerText && FilterDialog.TextFilterOn) {
                colMenuRemoveItem.Enabled = true;
            }
        }

        private void colMenuFilterItem_Click(object sender, EventArgs e) {
            // columnContextMenu.Tag tells us which column header was clicked to
            // display the column context menu.  Pass it to the filter dialog to
            // specify the initial tab page.
            ColumnHeader header = (ColumnHeader)columnContextMenu.Tag;
            FilterDialog dialog = new FilterDialog(header);
            dialog.ShowDialog(this);
        }

        private void colMenuRemoveItem_Click(object sender, EventArgs e) {
            ColumnHeader header = (ColumnHeader)columnContextMenu.Tag;

            if (header == headerLevel) {
                VisibleTraceLevels = _reader.LevelsFound;
                RebuildAllRows();
            } else if (header == headerThreadId) {
                ThreadObjects.ShowAllThreads();
                RebuildAllRows();
            } else if (header == headerSession) {
                SessionObjects.ShowAllSessions();
                RebuildAllRows();
            } else if (header == headerThreadName) {
                ThreadNames.ShowAllThreads();
                RebuildAllRows();
            } else if (header == headerLogger) {
                LoggerObjects.ShowAllLoggers();
                RebuildAllRows();
            } else if (header == headerMethod) {
                MethodObjects.ShowAllMethods();
                RebuildAllRows();
            } else if (header == headerText) {
                FilterDialog.TextFilterDisable();
                RebuildAllRows();
            }
        }

        private void colMenuOptionsItem_Click(object sender, EventArgs e) {
            // columnContextMenu.Tag tells us which column header was clicked to
            // display the column context menu.
            ColumnHeader header = (ColumnHeader)columnContextMenu.Tag;
            OptionsDialog dialog = new OptionsDialog(header);
            dialog.ShowDialog(this);
        }
        #endregion Column header context menu

        #region Recently Viewed/Created
        // Add the file to the list of recently viewed files.
        private void AddFileToRecentlyViewed(string filename) {
            if (Settings.Default.MRU == null) Settings.Default.MRU = new System.Collections.Specialized.StringCollection();

            if (Settings.Default.MRU.Contains(filename)) {
                // Remove the file we just loaded from Settings.Default.MRU.
                // It will be re-added at the end (most recent).
                Settings.Default.MRU.Remove(filename);
            }

            while (Settings.Default.MRU.Count > 6) {
                // Remove the oldest file in the list.
                Settings.Default.MRU.RemoveAt(0);
            }

            // Add the file we just loaded to the position of the most recent file in the MRU list.
            Settings.Default.MRU.Add(filename);
            Settings.Default.Save();
        }

        // Handler for opening the File menu.  Currently just sets 
        // the "Recently Viewed" and "Recently Created" menus.
        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            FillRecentlyViewedMenu();
            FillRecentlyCreatedMenu();
        }

        // Populate the Recently Viewed menu from the Settings.Default.MRU collection.
        private void FillRecentlyViewedMenu() {
            recentlyViewedToolStripMenuItem.DropDownItems.Clear();

            if (Settings.Default.MRU == null || Settings.Default.MRU.Count == 0) {
                recentlyViewedToolStripMenuItem.Enabled = false;
            } else {
                recentlyViewedToolStripMenuItem.Enabled = true;

                // Add a menu item for each file in Settings.Default.MRU.
                // The most recently opened file appears at the end of Settings.Default.MRU and
                // at the beginning of the MRU section of the File menu.
                foreach (string recentFile in Settings.Default.MRU) {
                    ToolStripMenuItem item = new ToolStripMenuItem(recentFile);
                    item.Tag = recentFile;
                    recentlyViewedToolStripMenuItem.DropDownItems.Insert(0, item);
                }
            }
        }

        // Populate the Recently Created menu from the RecentlyCreated.txt file.
        private void FillRecentlyCreatedMenu() {
            recentlyCreatedToolStripMenuItem.DropDownItems.Clear();

            // Get the list of recently created files from RecentlyCreated.txt.
            // The logger modifies this file each time it opens a file.
            try {
                string listFile = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "TracerX\\RecentlyCreated.txt"
                    );
                string[] files = File.ReadAllLines(listFile);

                if (files.Length > 0) {
                    foreach (string file in files) {
                        ToolStripMenuItem item = new ToolStripMenuItem(file);
                        item.Tag = file;
                        recentlyCreatedToolStripMenuItem.DropDownItems.Add(item);
                    }
                }
            } catch (Exception) {
                // The file containing the list of recently created filenames
                // probably doesn't exist.
            }

            recentlyCreatedToolStripMenuItem.Enabled = (recentlyCreatedToolStripMenuItem.DropDownItems.Count > 0);
        }

        // This handles selecting from the MRU file lists.
        private void RecentMenu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            if (e.ClickedItem.Tag != null) {
                string filename = (string)e.ClickedItem.Tag;

                // Remove the file from the "recently viewed" list now.  It
                // will be added back to the top of the list when file is finished loading.
                if (Settings.Default.MRU != null) Settings.Default.MRU.Remove(filename);

                StartReading(filename);
            }
        }
        #endregion

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            About about = new About();
            about.ShowDialog(this);
        }

        private void licenseToolStripMenuItem_Click(object sender, EventArgs e) {
            License dlg = new License();
            dlg.ShowDialog(this);
        }

        private void TheListView_SelectedIndexChanged(object sender, EventArgs e) {
            if (TheListView.FocusedItem == null) {
                //Debug.Print("SelectedIndexChanged, focused = null ");
                nextAnyThreadBtn.Enabled = false;
                prevAnyThreadBtn.Enabled = false;
            } else {
                //Debug.Print("SelectedIndexChanged, focused index = " + TheListView.FocusedItem.Index);
            }

            crumbBar1.SetCurrentRow(CurrentRow, _records);
            bookmarkToggleCmd.Enabled = TheListView.FocusedItem != null;
            UpdateThreadButtons();

            // When the main form is not active, we do our own highlighting of selected items.
            if (this != Form.ActiveForm) SetItemCacheColors(false);
        }

        private void TheListView_VirtualItemsSelectionRangeChanged(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e) {
            if (TheListView.FocusedItem == null) {
                //Debug.Print("VirtualItemsSelectionRange, focused = null ");
            } else {
                //Debug.Print("VirtualItemsSelectionRange, focused index = " + TheListView.FocusedItem.Index);
            }

            // When the main form is not active, we do our own highlighting of selected items.
            if (this != Form.ActiveForm) SetItemCacheColors(false);
        }

        private void UpdateThreadButtons() {
            if (TheListView.SelectedIndices.Count == 1) {
                int ndx = TheListView.SelectedIndices[0];
                prevAnyThreadBtn.Enabled = ndx > 0;
                prevSameThreadBtn.Enabled = ndx > 0;
                nextAnyThreadBtn.Enabled = ndx != NumRows - 1;
                nextSameThreadBtn.Enabled = ndx != NumRows - 1;
            } else {
                prevAnyThreadBtn.Enabled = false;
                prevSameThreadBtn.Enabled = false;
                nextAnyThreadBtn.Enabled = false;
                nextSameThreadBtn.Enabled = false;
            }
        }

        // Set the backcolor and forecolor of items in the cache so selected items
        // remain prominent even when the form loses focus.  I tried just setting
        // HideSelection to false, but the items become gray instead of highlighted
        // and the gray is nearly invisible on some monitors.
        // Called when selection(s) change.
        private void SetItemCacheColors(bool formActive) {
            //Debug.Print("formActive = " + formActive);
            if (_itemCache != null) {
                TheListView.BeginUpdate();
                foreach (ViewItem item in _itemCache) item.SetItemColors(formActive);
                TheListView.EndUpdate();
            }
        }

        void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case "RelativeTime":
                    relativeTimeButton.Checked = Settings.Default.RelativeTime;
                    InvalidateTheListView();
                    break;
                case "ColoringEnabled":
                    enableColors.Checked = Settings.Default.ColoringEnabled;
                    InvalidateTheListView();
                    break;
                case "AutoUpdate":
                    if (autoUpdate.Enabled && Settings.Default.AutoUpdate) {
                        StartFileWatcher();
                    } else {
                        StopFileWatcher();
                    }
                    break;
            }
        }

        private void relativeTimeButton_Click(object sender, EventArgs e) {
            Settings.Default.RelativeTime = !Settings.Default.RelativeTime;
        }

        private void MainForm_Activated(object sender, EventArgs e) {
            //Debug.Print("MainForm_Activated, " + TheListView.SelectedIndices.Count + " selected");
            SetItemCacheColors(true);
        }

        private void closeAllWindowsToolStripMenuItem_Click(object sender, EventArgs e) {
            List<Form> toClose = new List<Form>();

            foreach (Form form in Application.OpenForms) {
                if (form != this) toClose.Add(form);
            }

            foreach (Form form in toClose) form.Close();
        }

        private void viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            closeAllWindowsToolStripMenuItem.Enabled = Application.OpenForms.Count > 1;
        }

        private void TheListView_Leave(object sender, EventArgs e) {
            // The crumbBar gets the focus whenever the user clicks one of its links (including
            // disabled links).  When this happens, the selected items in the TheListView
            // are no longer highlighted.  Return the focus to TheListView so the selected
            // rows remain highlighted.
            this.ActiveControl = TheListView;
        }

        private void coloringCmd_Execute(object sender, EventArgs e) {
            ColorRulesDialog.ShowModal();
        }

        private void enableColors_Execute(object sender, EventArgs e) {
            Settings.Default.ColoringEnabled = !Settings.Default.ColoringEnabled;
        }

        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e) {
            ExportCSVForm.ShowModal();
        }

        private void ShowMessageBox(string text) {
            if (InvokeRequired) {
                Invoke(new Action<string>(ShowMessageBox), text);
                return;
            }

            MessageBox.Show(this, text, "TracerX-Viewer");
        }

        private void autoUpdate_Click(object sender, EventArgs e) {
            autoUpdate.Checked = !autoUpdate.Checked;
            Settings.Default.AutoUpdate = autoUpdate.Checked;
        }

        // Show the "Uncolor Selected <item>s menu item only if at least one of the
        // selected rows is colored due to a coloring rule (other than Custom rules).
        private void UpdateUncolorMenu() {
            uncolorSelectedMenu.Visible = false; // for now

            foreach (int ndx in TheListView.SelectedIndices) {
                var record = _rows[ndx].Rec;

                switch (ColorRulesDialog.CurrentTab) {
                    case ColorRulesDialog.ColorTab.Custom:
                        break;
                    case ColorRulesDialog.ColorTab.Loggers:
                        if (record.Logger.Colors != null) {
                            uncolorSelectedMenu.Visible = true;
                            uncolorSelectedMenu.Text = "Uncolor Selected Loggers";
                        }
                        break;
                    case ColorRulesDialog.ColorTab.Methods:
                        if (record.MethodName.Colors != null) {
                            uncolorSelectedMenu.Visible = true;
                            uncolorSelectedMenu.Text = "Uncolor Selected Methods";
                        } else if (ColorRulesDialog.ColorCalledMethods) {
                            for (Record caller = record.Caller; caller != null; caller = caller.Caller) {
                                if (caller.MethodName.Colors != null) {
                                    uncolorSelectedMenu.Visible = true;
                                    uncolorSelectedMenu.Text = "Uncolor Selected Methods";
                                    break;
                                }
                            }
                        }
                        break;
                    case ColorRulesDialog.ColorTab.TraceLevels:
                        if (ColorRulesDialog.TraceLevelColors[record.Level].Enabled) {
                            uncolorSelectedMenu.Visible = true;
                            uncolorSelectedMenu.Text = "Uncolor Selected Trace Levels";
                        }
                        break;
                    case ColorRulesDialog.ColorTab.ThreadIDs:
                        if (record.Thread.Colors != null) {
                            uncolorSelectedMenu.Visible = true;
                            uncolorSelectedMenu.Text = "Uncolor Selected Thread IDs";
                        }
                        break;
                    case ColorRulesDialog.ColorTab.Sessions:
                        if (record.Session.Colors != null) {
                            uncolorSelectedMenu.Visible = true;
                            uncolorSelectedMenu.Text = "Uncolor Selected Sessions";
                        }
                        break;
                    case ColorRulesDialog.ColorTab.ThreadNames:
                        if (record.ThreadName.Colors != null) {
                            uncolorSelectedMenu.Visible = true;
                            uncolorSelectedMenu.Text = "Uncolor Selected Thread Names";
                        }
                        break;
                }

                if (uncolorSelectedMenu.Visible) break;
            }
        }

        private void uncolorSelectedMenu_Click(object sender, EventArgs e) {
            foreach (int ndx in TheListView.SelectedIndices) {
                var record = _rows[ndx].Rec;

                switch (ColorRulesDialog.CurrentTab) {
                    case ColorRulesDialog.ColorTab.Custom:
                        // Should be impossible.
                        break;
                    case ColorRulesDialog.ColorTab.Loggers:
                        record.Logger.Colors = null;
                        break;
                    case ColorRulesDialog.ColorTab.Methods:
                        record.MethodName.Colors = null;

                        if (ColorRulesDialog.ColorCalledMethods) {
                            for (Record caller = record.Caller; caller != null; caller = caller.Caller) {
                                caller.MethodName.Colors = null;
                            }
                        }
                        break;
                    case ColorRulesDialog.ColorTab.TraceLevels:
                        ColorRulesDialog.TraceLevelColors[record.Level].Enabled = false;
                        break;
                    case ColorRulesDialog.ColorTab.ThreadIDs:
                        record.Thread.Colors = null;
                        break;
                    case ColorRulesDialog.ColorTab.Sessions:
                        record.Session.Colors = null;
                        break;
                    case ColorRulesDialog.ColorTab.ThreadNames:
                        record.ThreadName.Colors = null;
                        break;
                }
            }

            // Cause visible rows to be recreated.
            InvalidateTheListView();

        }

        private void ColorStuff(IEnumerable<IFilterable> itemsToColor, IEnumerable<IFilterable> allItems) {
            var availableColors = ColorRulesDialog.Palette.ToList();

            // Determine which colors are already used.
            foreach (var filterable in allItems) {
                if (filterable.Colors != null) {
                    availableColors.Remove(filterable.Colors);
                }
            }

            foreach (var filterable in itemsToColor.Distinct()) {
                if (filterable.Colors == null) {
                    if (availableColors.Any()) {
                        filterable.Colors = availableColors.First();
                        availableColors.Remove(filterable.Colors);
                    } else {
                        MessageBox.Show(this, "Sorry, the color palette does not contain enough colors for all selected items.", "TracerX-Viewer");
                        break;
                    }
                }
            }

            Settings.Default.ColoringEnabled = true;

            // Cause visible rows to be recreated.
            InvalidateTheListView();
        }

        private void colorSelectedThreadNamesMenu_Click(object sender, EventArgs e) {
            var selectedItems = new List<IFilterable>();

            foreach (int index in TheListView.SelectedIndices) {
                selectedItems.Add(_rows[index].Rec.ThreadName);
            }

            ColorRulesDialog.CurrentTab = ColorRulesDialog.ColorTab.ThreadNames;
            ColorStuff(selectedItems, ThreadNames.AllThreadNames.Cast<IFilterable>());
        }

        private void colorSelectedThreadIDsMenu_Click(object sender, EventArgs e) {
            var selectedItems = new List<IFilterable>();

            foreach (int index in TheListView.SelectedIndices) {
                selectedItems.Add(_rows[index].Rec.Thread);
            }

            ColorRulesDialog.CurrentTab = ColorRulesDialog.ColorTab.ThreadIDs;
            ColorStuff(selectedItems, ThreadObjects.AllThreadObjects.Cast<IFilterable>());
        }

        private void colorSelectedSessionsMenu_Click(object sender, EventArgs e) {
            var selectedItems = new List<IFilterable>();

            foreach (int index in TheListView.SelectedIndices) {
                selectedItems.Add(_rows[index].Rec.Session);
            }

            ColorRulesDialog.CurrentTab = ColorRulesDialog.ColorTab.Sessions;
            ColorStuff(selectedItems, SessionObjects.AllSessionObjects.Cast<IFilterable>());
        }

        private void colorSelectedTraceLevelsMenu_Click(object sender, EventArgs e) {
            foreach (int index in TheListView.SelectedIndices) {
                ColorRulesDialog.TraceLevelColors[_rows[index].Rec.Level].Enabled = true;
            }

            ColorRulesDialog.CurrentTab = ColorRulesDialog.ColorTab.TraceLevels;
            Settings.Default.ColoringEnabled = true;

            // Cause visible rows to be recreated.
            InvalidateTheListView();
        }

        private void colorSelectedLoggersMenu_Click(object sender, EventArgs e) {
            var selectedItems = new List<IFilterable>();

            foreach (int index in TheListView.SelectedIndices) {
                selectedItems.Add(_rows[index].Rec.Logger);
            }

            ColorRulesDialog.CurrentTab = ColorRulesDialog.ColorTab.Loggers;
            ColorStuff(selectedItems, LoggerObjects.AllLoggers.Cast<IFilterable>());
        }

        private void colorSelectedMethodsMenu_Click(object sender, EventArgs e) {
            var selectedItems = new List<IFilterable>();

            foreach (int index in TheListView.SelectedIndices) {
                selectedItems.Add(_rows[index].Rec.MethodName);
            }

            ColorRulesDialog.CurrentTab = ColorRulesDialog.ColorTab.Methods;
            ColorStuff(selectedItems, MethodObjects.AllMethods.Cast<IFilterable>());
        }

        private void prevThreadBtn_Click(object sender, EventArgs e) {
            if (TheListView.SelectedIndices.Count > 0) {
                int startNdx = TheListView.SelectedIndices[0];
                var thread = _rows[startNdx].Rec.Thread;
                int topNdx = TheListView.TopItem.Index;

                int foundNdx;
                for (foundNdx = startNdx - 1; foundNdx > 0 && _rows[foundNdx].Rec.Thread == thread; --foundNdx) ;

                if (sender == prevSameThreadBtn) {
                    // Find the starting thread again.
                    for (; foundNdx > 0 && _rows[foundNdx].Rec.Thread != thread; --foundNdx) ;
                }

                if (foundNdx <= topNdx) {
                    // The found row is off the page, so when we scroll to it, position it
                    // in the middle of the page.
                    int itemsVisible = FindLastVisibleItem() - topNdx + 1;
                    int newTop = foundNdx - itemsVisible / 2;
                    TheListView.TopItem = TheListView.Items[Math.Max(0, newTop)];
                }

                SelectSingleRow(foundNdx);
            }
        }

        private void nextThreadBtn_Click(object sender, EventArgs e) {
            if (TheListView.SelectedIndices.Count > 0) {
                int startNdx = TheListView.SelectedIndices[0];

                if (startNdx < NumRows - 1) {
                    var thread = _rows[startNdx].Rec.Thread;
                    int lastNdx = FindLastVisibleItem();

                    int foundNdx;

                    // First find a different thread.
                    for (foundNdx = startNdx + 1; foundNdx < NumRows - 1 && _rows[foundNdx].Rec.Thread == thread; ++foundNdx) ;

                    if (sender == nextSameThreadBtn) {
                        // Find the starting thread again.
                        for (; foundNdx < NumRows - 1 && _rows[foundNdx].Rec.Thread != thread; ++foundNdx) ;
                    }

                    if (foundNdx >= lastNdx) {
                        // The found row is off the page, so when we scroll to it, position it
                        // in the middle of the page.
                        int itemsVisible = lastNdx - TheListView.TopItem.Index + 1;
                        int newTop = Math.Min(NumRows - itemsVisible, foundNdx - itemsVisible / 2);
                        
                        // For some reason, setting the TopItem once doesn't work here.  Setting
                        // it three times usually does, so try up to four.
                        for (int i = 0; i < 4; ++i) {
                            if (TheListView.TopItem.Index == newTop) break;
                            TheListView.TopItem = (TheListView.Items[newTop]);
                        }
                    }

                    SelectSingleRow(foundNdx);
                    Debug.Print("Actual top = " + TheListView.TopItem.Index);
                }
            }
        }

    }
}