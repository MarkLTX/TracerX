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
using System.Reflection;
using Microsoft.VisualBasic.FileIO;
using System.ServiceModel;

// TODO: On filter dialog, display loggers in a tree view.
//       If find or next bookmark passes end of file, display (passed end of file).

// TODO: Option to make line numbers continuous or restart at sessions.

// See http://blogs.msdn.com/cumgranosalis/archive/2006/03/06/VirtualListViewUsage.aspx
// for a good article on using ListView in virtual mode.

namespace TracerX
{
    // This is the main form for the TracerX log viewer.
    [System.Diagnostics.DebuggerDisplay("MainForm")] // Helps prevent debugger from freezing in the worker thread.
    internal partial class MainForm : Form
    {
        #region Ctor/init

        // Constructor.  
        public MainForm()
        {
            using (Log.InfoCall())
            {
                InitializeComponent();

                TheMainForm = this;
                TheListView.MainForm = this;
                crumbBar1.Clear();

                // Requires that TheMainForm be set.
                AppArgs.ParseCommandLine(true);

                _originalTitle = this.Text;
                this.Icon = Properties.Resources.scroll_view;
                if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";

                SessionObjects.AllVisibleChanged += FilterAddedOrRemoved;
                ThreadNames.AllVisibleChanged += FilterAddedOrRemoved;
                ThreadObjects.AllVisibleChanged += FilterAddedOrRemoved;
                LoggerObjects.AllVisibleChanged += FilterAddedOrRemoved;
                TraceLevelObjects.AllVisibleChanged += FilterAddedOrRemoved;
                MethodObjects.AllVisibleChanged += FilterAddedOrRemoved;
                FilterDialog.TextFilterOnOff += FilterAddedOrRemoved;

                InitColumns();

                try
                {
                    if (Settings.Default.IndentChar == '\0') Settings.Default.IndentChar = ' ';
                }
                catch (Exception)
                {
                    Settings.Default.IndentChar = ' ';
                }

                // Setting the FileState affects many menu items and buttons.
                _FileState = FileState.NoFile;

                relativeTimeButton.Checked = Settings.Default.RelativeTime;
                dupTimeButton.Checked = Settings.Default.DuplicateTimes;
                timeUnitCombo.SelectedIndex = Settings.Default.TimeUnits;
                enableColors.Checked = Settings.Default.ColoringEnabled;
                ApplyBoldSetting();

                Settings.Default.PropertyChanged += Settings_PropertyChanged;

                if (Settings.Default.ColoringRules != null)
                {
                    foreach (ColoringRule rule in Settings.Default.ColoringRules) rule.MakeReady();
                }

                if (Settings.Default.MaxRecords < 0)
                {
                    // Default value hasn't been set.  It's based on RAM size.
                    // User can change it via Options dialog.
                    Microsoft.VisualBasic.Devices.ComputerInfo info = new Microsoft.VisualBasic.Devices.ComputerInfo();

                    if (info.TotalPhysicalMemory >= 2000000000)
                    {
                        // 2 gigabytes or more, use about 1,000,000 records per gig.
                        // Keep only 2 signigicant digits (for readability).

                        double maxRecords = (double)(info.TotalPhysicalMemory) / 1024;
                        double scale = Math.Pow(10, Math.Floor(Math.Log10(maxRecords)) + 1);
                        int sigDigits = 2;
                        maxRecords = scale * Math.Round(maxRecords / scale, sigDigits);
                        Settings.Default.MaxRecords = (int)maxRecords;
                    }
                    else
                    {
                        // Under 2 gigabytes, use 500,000.
                        Settings.Default.MaxRecords = 500000;
                    }
                }

                _fileChangedDelegate = new FileSystemEventHandler(FileChanged);
                _fileRenamedDelegate = new RenamedEventHandler(FileRenamed);

                theStartPage.FolderClicked += new EventHandler(theStartPage_FolderClicked);
                theStartPage.FileClicked += new EventHandler(theStartPage_FileClicked);
                theStartPage.ServerChanged += new EventHandler(theStartPage_ServerChanged);

                // Don't show the "New Window" command unless this assembly is the main assembly.
                Assembly thisAsm = Assembly.GetExecutingAssembly();
                Assembly entryAsm = Assembly.GetEntryAssembly();
                newWindowToolStripMenuItem.Visible = (thisAsm == entryAsm);
            }
        }

        void theStartPage_ServerChanged(object sender, EventArgs e)
        {
            using (Log.InfoCall())
            {
                if (theStartPage.CurRemoteServer == null)
                {
                    serverLabel.Text = "LocalHost";
                    openFileCmd.Enabled = true;
                }
                else
                {
                    serverLabel.Text = theStartPage.CurRemoteServer.HostName;
                    openFileCmd.Enabled = false;
                }
            }
        }

        void theStartPage_FileClicked(object sender, EventArgs e)
        {
            using (Log.InfoCall())
            {
                StartReading(theStartPage.ClickedPath, theStartPage.CurRemoteServer);
            }
        }

        public static bool CheckFolderAccess(string folderPath, bool showMessage = true)
        {
            bool result = false;

            if (Directory.Exists(folderPath))
            {
                result = true;
            }
            else
            {
                // Directory.Exists() returns false if the directory exists but we lack permission to read it, so
                // we have to actually attempt to read it to determine whether it doesn't exist or we just can't
                // read it.

                try
                {
                    // This should throw an exception that explains why Directory.Exists() returned
                    // false.  If it doesn't just display a message with no explanation.

                    Directory.GetFiles(folderPath, "just.testing.for.read.access");
                    if (showMessage) ShowMessageBox("Unable to access the folder.\n\n" + folderPath);
                }
                catch (DirectoryNotFoundException)
                {
                    if (showMessage) ShowMessageBox("The selected folder no longer exists.\n\n" + folderPath);
                }
                catch (UnauthorizedAccessException)
                {
                    if (showMessage) ShowMessageBox("Access to the folder is denied.\n\n" + folderPath);
                }
                catch (Exception ex)
                {
                    if (showMessage) ShowMessageBox(ex.Message);
                }
            }

            return result;
        }

        void theStartPage_FolderClicked(object sender, EventArgs e)
        {
            using (Log.InfoCall())
            {
                if (theStartPage.CurRemoteServer == null)
                {
                    // It's a local folder so use the standard OpenFileDialog.                     

                    if (CheckFolderAccess(theStartPage.ClickedPath))
                    {
                        BrowseForFile(theStartPage.ClickedPath);
                    }
                }
                else
                {
                    // It's a remote folder, so we get the files from the remote
                    // TracerX service and display them in our own dialog.

                    Cursor restoreCursor = this.Cursor;
                    PathSelector pathSelector = new PathSelector();

                    try
                    {
                        this.Cursor = Cursors.WaitCursor;
                        pathSelector.Init(theStartPage.CurRemoteServer, theStartPage.ClickedPath);
                    }
                    finally
                    {
                        this.Cursor = restoreCursor;
                    }

                    if (pathSelector.ShowDialog() == DialogResult.OK)
                    {
                        StartReading(pathSelector.SelectedFile, theStartPage.CurRemoteServer);
                    }
                }
            }
        }

        public static void DisableFlicker(System.Windows.Forms.Control ctrl)
        {
            MethodInfo method = ctrl.GetType().GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method != null)
            {
                //method.Invoke(ctrl, new object[] { ControlStyles.OptimizedDoubleBuffer, true });
                method.Invoke(ctrl, new object[] { ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true });
            }
        }

        // Perform column-related initialization.
        private void InitColumns()
        {
            // Keep a copy of the original list of columns since the only way to
            // hide a column in a ListView is to remove it.
            OriginalColumns = new ColumnHeader[TheListView.Columns.Count];
            for (int i = 0; i < TheListView.Columns.Count; ++i)
            {
                OriginalColumns[i] = TheListView.Columns[i];
            }

            // Apply the persisted column settings.

            if (Settings.Default.ColIndices != null &&
                Settings.Default.ColSelections != null &&
                Settings.Default.ColWidths != null &&
                Settings.Default.ColWidths.Length == OriginalColumns.Length)
            {
                TheListView.Columns.Clear();
                try
                {
                    for (int i = 0; i < OriginalColumns.Length; ++i)
                    {
                        if (Settings.Default.ColSelections[i])
                        {
                            TheListView.Columns.Add(OriginalColumns[i]);
                        }
                        OriginalColumns[i].Width = Settings.Default.ColWidths[i];
                    }

                    // We can't set the display index until after the column headers
                    // have been added.
                    for (int i = 0; i < OriginalColumns.Length; ++i)
                    {
                        OriginalColumns[i].DisplayIndex = Settings.Default.ColIndices[i];
                    }
                }
                catch
                {
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

        // The original columns in the ListView before any are hidden (by removing them).
        public ColumnHeader[] OriginalColumns;

        // The timestamp from the row selected to be the "zero time" row.
        public static DateTime ZeroTime = DateTime.MinValue;

        // List of rows being displayed by the ListView. 
        // This gets regenerated when the filter changes and
        // when rows are expanded or collapsed.
        public List<Row> Rows
        {
            get;
            private set;
        }

        // The Row corresponding to the the ListViewItem that has focus, or the first
        // row if no item has the focus.  Used as the start of a Find.
        public Row FocusedRow
        {
            get
            {
                if (TheListView.FocusedItem == null)
                {
                    if (NumRows == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return Rows[0];
                    }
                }
                else
                {
                    return Rows[TheListView.FocusedItem.Index];
                }
            }
        }

        // The Row corresponding to the the ListViewItem that has focus.
        // Null if no items are selected or no item has focus.
        public Row CurrentRow
        {
            get
            {
                if (TheListView.FocusedItem == null || TheListView.SelectedIndices.Count == 0)
                {
                    return null;
                }
                else
                {
                    return Rows[TheListView.FocusedItem.Index];
                }
            }
        }

        public int NumRows
        {
            get { return TheListView.VirtualListSize; }

            set
            {
                try
                {
                    TheListView.VirtualListSize = value;
                }
                catch (Exception ex)
                {
                    Debug.Print("Exception setting ListView.VirtualListSize: " + ex.ToString());
                }

                Debug.Print("NumRows now " + NumRows);

                // Disable Find and FindNext/F3 if no text is visible.
                UpdateFindCommands();
                UpdateThreadButtons();
                UpdateTimeButtons();
            }
        }

        // If the rowIndex is valid, selects that row and returns it.
        // Otherwise, returns null.
        public Row SelectRowIndex(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < Rows.Count)
            {
                SelectRow(Rows[rowIndex]);
                return Rows[rowIndex];
            }
            else
            {
                return null;
            }
        }

        public void InvalidateTheListView()
        {
            TheListView.ClearItemCache();
            TheListView.Invalidate();
        }

        // Called by the FindDialog and when F3 is pressed.
        // If the specified text is found (not bookmarked), selects the row/item
        // and returns the ListViewItem.
        public Row DoSearch(StringMatcher matcher, bool searchUp, bool bookmark)
        {
            Cursor restoreCursor = this.Cursor;
            Row startRow = FocusedRow;
            Row curRow = startRow;
            bool found = false;
            bool everWrapped = false;

            // Remember inputs in case user hits F3 or shift+F3.
            _textMatcher = matcher;

            UpdateFindCommands();
            statusMsg.Visible = false;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                do
                {
                    bool wrapped;
                    curRow = NextRow(curRow, searchUp, out wrapped);

                    everWrapped = everWrapped || wrapped;

                    if (matcher.Matches(curRow.ToString()))
                    {
                        found = true;
                        if (bookmark)
                        {
                            curRow.IsBookmarked = true;
                        }
                        else
                        {
                            SelectFoundIndex(curRow.Index);
                            if (everWrapped) ShowStatus("Passed end of file.", false);
                            return curRow;
                        }
                    }
                } while (curRow != startRow);
            }
            finally
            {
                this.Cursor = restoreCursor;
            }

            if (!found)
            {
                ShowStatus("Did not find: " + matcher.OriginalText, true);
            }
            else if (bookmark)
            {
                bookmarkNextCmd.Enabled = bookmarkPrevCmd.Enabled = bookmarkClearCmd.Enabled = true;
                InvalidateTheListView();
            }

            return null;
        }

        // Find the index of the last visible item displayed by TheListView.
        public int FindLastVisibleItem()
        {
            int ndx = TheListView.TopItem.Index;
            do
            {
                ++ndx;
            } while (ndx < NumRows && TheListView.ClientRectangle.IntersectsWith(TheListView.Items[ndx].Bounds));

            //Debug.Print("Last visible index = " + (ndx - 1));
            return ndx - 1;
        }

        // Called by the FilterDialog when the user clicks Apply or OK.
        public void RebuildAllRows()
        {
            if (_records != null && _records.Count > 0)
            {
                RebuildRows(0, _records[0]);
            }
        }

        #endregion

        private static Logger Log = Logger.GetLogger("MainForm");
        private enum FileState { NoFile, Loading, Loaded };
        private FileState _fileState = FileState.NoFile;

        // The current log file.
        private string _filepath;

        // This watches the file for changes so new log messages can be read
        // as soon as they are written.
        private IFileWatcher _watcher;
        private FileSystemEventHandler _fileChangedDelegate;
        private RenamedEventHandler _fileRenamedDelegate;

        // Helper that reads the log file.
        private Reader _reader;

        // List of records read from the log file.
        private List<Record> _records;

        // The row number (scroll position) to restore after refreshing the file.
        private int _rowNumToRestore;

        // The original title to which the file name is appended to be
        // displayed in the title bar.
        private string _originalTitle;

        // Bitmask of trace levels selected to be visible (i.e. not filtered out).
        //private TraceLevel _visibleTraceLevels;

        // Text search settings for F3 ("find next").
        StringMatcher _textMatcher;

        // The area occupied by the ListView header.  Used to determine which column header is
        // right-clicked so the appropriate context menu can be displayed.
        private Rectangle _headerRect;

        private ColumnHeader _clickedColumn;

        private FileState _FileState
        {
            get { return _fileState; }
            set
            {
                Log.Info("Setting _fileState to ", value);
                _fileState = value;
                autoUpdate.Enabled = (_fileState == FileState.Loaded && _reader.FormatVersion > 5);
                columnsCmd.Enabled = (_fileState == FileState.Loaded);
                dupTimeButton.Enabled = (_fileState == FileState.Loaded);
                relativeTimeButton.Enabled = (_fileState == FileState.Loaded);
                clearColumnColors.Enabled = (_fileState == FileState.Loaded);
                enableColors.Enabled = (_fileState == FileState.Loaded);
                editColors.Enabled = (_fileState == FileState.Loaded);
                boldBtn.Enabled = (_fileState == FileState.Loaded);
                timeUnitCombo.Enabled = (_fileState == FileState.Loaded);
                propertiesCmd.Enabled = (_fileState == FileState.Loaded);
                filterDlgCmd.Enabled = (_fileState == FileState.Loaded);
                //closeToolStripMenuItem.Enabled = (_fileState == FileState.Loaded);
                refreshCmd.Enabled = (_fileState == FileState.Loaded);
                expandAllButton.Enabled = (_FileState == FileState.Loaded);
                collapseAllButton.Enabled = (_FileState == FileState.Loaded);
                exportToCSVToolStripMenuItem.Enabled = (_FileState == FileState.Loaded);
                closeBtn.ToolTipText = (_fileState == FileState.NoFile ? "Close program" : "Close file. Return to Start Page.");

                btnCancel.Visible = (_fileState == FileState.Loading);

                openFileCmd.Enabled = (_fileState != FileState.Loading && theStartPage.CurRemoteServer == null); // Must be LocalHost to enable.
                crumbBar1.Visible = (_fileState != FileState.NoFile);
                theStartPage.Visible = (_fileState == FileState.NoFile);
                showTracerXLogsMenuItem.Enabled = (_fileState == FileState.NoFile);
                showServerPickerMenuItem.Enabled = (_fileState == FileState.NoFile);

                if (_fileState != FileState.Loaded)
                {
                    NumRows = 0;
                    Rows = null;
                    _records = null;
                    TheListView.ClearItemCache();

                    crumbBar1.Clear();

                    // Some commands are disabled when filestate != Loaded, but not necessarily
                    // enabled when filestate == Loaded.
                    filterClearCmd.Enabled = false;
                    bookmarkToggleCmd.Enabled = false;
                    bookmarkNextCmd.Enabled = false;
                    bookmarkPrevCmd.Enabled = false;
                    bookmarkClearCmd.Enabled = false;
                }

                if (_fileState == FileState.NoFile)
                {
                    filenameLabel.Text = "";
                    this.Text = _originalTitle;
                }

                UpdateFindCommands();
            }
        }

        // This returns an array of the ColumnHeaders in the order they are
        // displayed by the ListView.  Used to determine which column header
        // was right-clicked.
        public ColumnHeader[] OrderedHeaders
        {
            get
            {
                ColumnHeader[] arr = new ColumnHeader[TheListView.Columns.Count];

                foreach (ColumnHeader col in TheListView.Columns)
                {
                    arr[col.DisplayIndex] = col;
                }

                return arr;
            }
        }

        private Record _FocusedRec
        {
            get
            {
                Row focusedRow = FocusedRow;
                if (focusedRow == null)
                {
                    return null;
                }
                else
                {
                    return focusedRow.Rec;
                }
            }
        }

        // Increment or decrement _curRow depending on searchUp and handle wrapping.
        private Row NextRow(Row curRow, bool searchUp, out bool wrapped)
        {
            int ndx = curRow.Index;

            wrapped = false;

            if (searchUp)
            {
                --ndx;
                if (ndx < 0)
                {
                    wrapped = true;
                    ndx = NumRows - 1;
                }
            }
            else
            {
                ++ndx;
                if (ndx >= NumRows)
                {
                    wrapped = true;
                    ndx = 0;
                }
            }

            return Rows[ndx];
        }

        private void SelectRow(Row row)
        {
            // This clears the focus rectangles that otherwise remain when auto-refreshing.
            if (TheListView.FocusedItem != null) TheListView.FocusedItem.Focused = false;

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
        private int GetRowNumToRestore()
        {
            int ret = 0;
            if (TheListView.FocusedItem != null && TheListView.FocusedItem.Index == NumRows - 1)
            {
                ret = -1; // Special value meaning the very end.
            }
            else if (TheListView.TopItem != null)
            {
                ret = TheListView.TopItem.Index;
            }

            return ret;
        }

        // The server from which _filepath was loaded.  Null for localhost.
        private RemoteServer _fileServer;

        // Open the specified log file and, if successful, 
        // start the background thread that reads it.
        // A null filename means to refresh the current file.
        // A null fileServer means localhost.
        public bool StartReading(string filename, RemoteServer fileServer)
        {
            using (Log.InfoCall())
            {
                Log.Info("filename is ", filename, ", fileServer is ", fileServer);

                bool result = false;
                bool refreshing;

                StopFileWatcher();

                if (filename == null)
                {
                    filename = _filepath;
                    fileServer = _fileServer;

                    Log.Info("Will refresh filename ", filename, ", fileServer ", fileServer);

                    refreshing = true;
                    _rowNumToRestore = GetRowNumToRestore();
                }
                else
                {
                    refreshing = false;
                    _rowNumToRestore = 0;
                }

                // Remote files are assumed to exist, but local files are checked here.
                // Note that "remote file" means a file accessed via the TracerX service, and
                // a "local file" means a file accessed directly using its path even if the path is a remote share.

                if (fileServer != null || File.Exists(filename))
                {
                    string tempFile = fileServer == null ? MaybeCopyFile(filename) : filename;
                    Reader prospectiveReader = fileServer == null ? new Reader(filename, tempFile) : new Reader(fileServer, filename);

                    // If we can't open the new file, the old file (if any) stays loaded.
                    if (prospectiveReader.OpenLogFile())
                    {
                        Log.Info("File was opened.");

                        _reader = prospectiveReader;
                        _FileState = FileState.Loading;
                        _filepath = filename;
                        _fileServer = fileServer;

                        if (refreshing && Settings.Default.KeepFilter)
                        {
                            _reader.ReuseFilters();
                        }
                        else
                        {
                            FilterDialog.TextFilterDisable();
                        }

                        ResetFilters(reuseFilters: refreshing && Settings.Default.KeepFilter);

                        filterClearCmd.Enabled = false;

                        filenameLabel.Text = filename;
                        this.Text = Path.GetFileName(filename) + " - " + _originalTitle;

                        if (!refreshing)
                        {
                            if (fileServer == null)
                            {
                                AddFileToRecentlyViewed(filename);
                            }
                            else
                            {
                                fileServer.UpdateViewedFiles(filename);
                            }
                        }

                        Log.Info("Starting background worker to read file.");
                        backgroundWorker1.RunWorkerAsync();
                        result = true;
                    }
                    else
                    {
                        Log.Warn("File was NOT opened.");

                        // If we made a temporary copy, delete it.
                        if (tempFile != filename && File.Exists(tempFile))
                        {
                            File.Delete(tempFile);
                        }
                    }
                }
                else
                {
                    Log.Info("Showing 'file does not exist' message for: ", filename);
                    ShowMessageBox("File does not exist:\n\n" + filename);
                }

                return result;
            }
        }

        // If the filePath appears to be a UNC path, this copies it locally and returns
        // the local temp file.  If not, or copying fails, returns the input filename.
        private string MaybeCopyFile(string filePath)
        {
            string result = filePath;

            if (Settings.Default.MaxNetworkKB > 0 && filePath.StartsWith("\\\\"))
            {
                FileInfo fileInfo = new FileInfo(filePath);

                if (fileInfo.Length > (Settings.Default.MaxNetworkKB << 10))
                {
                    filenameLabel.Text = filePath;
                    ShowStatus("Copying locally for faster loading.", false);

                    try
                    {
                        result = Path.Combine(Path.GetTempPath(), Path.GetFileName(filePath));
                        if (File.Exists(result)) File.Delete(result);
                        FileSystem.CopyFile(filePath, result, UIOption.AllDialogs, UICancelOption.ThrowException);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.Message);
                        result = filePath;
                    }
                }
            }

            return result;
        }

        // This method runs in a worker thread.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            using (Log.InfoCallThread("TracerX Log Reader"))
            {
                int percent = 0;
                int lastPercent = 0;
                int rowCount = 0;
                long totalBytes = _reader.InitialSize;
                Action<int> ReportProgressDelegate = this.ReportProgress;
                DateTime nextReportTime = DateTime.Now.AddMilliseconds(250);

                Log.Info("File size is ", totalBytes);
                _records = new List<Record>((int)(totalBytes / 100)); // Guess at how many records

                for (var session = _reader.NextSession(); session != null; session = _reader.NextSession())
                {
                    using (Log.InfoCall("Reading session " + session.Name))
                    {
                        Record record = session.ReadRecord();

                        if (record == null)
                        {
                            Log.Warn("Failed to read first record!");
                        }
                        else
                        {
                            Log.Info("Got first record entering loop to read more.");
                        }

                        while (record != null)
                        {
                            rowCount += record.Lines.Length;
                            record.Index = _records.Count;
                            _records.Add(record);

                            percent = (int)((_reader.BytesRead * 100) / totalBytes);
                            if (percent > 100) percent = 100;

                            if (percent > lastPercent && DateTime.Now > nextReportTime)
                            {
                                lastPercent = percent;
                                nextReportTime = DateTime.Now.AddMilliseconds(250);
                                BeginInvoke(ReportProgressDelegate, percent);
                            }

                            record = session.ReadRecord();

                            // This Sleep call is critical.  Without it, the main thread doesn't seem to
                            // get any cpu time and the worker thread seems to go much slower.
                            Thread.Sleep(0);

                            if (backgroundWorker1.CancellationPending)
                            {
                                Log.Info("Background worker was cancelled.");
                                e.Cancel = true;
                                break;
                            }
                        }

                        // If the log has both a circular part and a non-circular part, there may
                        // be missing exit/entry records due to wrapping.
                        rowCount += session.InsertMissingRecords(_records);
                    }
                }

                Log.InfoFormat("total bytes = {0}, bytes read = {1}, ratio = {2}", totalBytes, _reader.BytesRead, (double)_reader.BytesRead / (double)totalBytes);

                if (!_reader.CanLeaveOpen) _reader.CloseLogFile();

                // Initially, there is a 1:1 relationship between each row and each record.  This will change
                // if the user collapses method calls (causing some records to be omitted from view) or
                // expands rows with embedded newlines.
                // Allocate enough rows to handle the case of all messages with embedded newlines being expanded.
                Rows = new List<Row>(rowCount);
            }
        }

        private void ReportProgress(int percent)
        {
            ShowStatus(percent.ToString() + "%", false);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            using (Log.InfoCall())
            {
                if (_reader.OriginalFile != _reader.TempFile)
                {
                    // This means we just finished loading a temporary copy.  Delete the
                    // copy and begin using/watching the original file.
                    if (File.Exists(_reader.TempFile)) File.Delete(_reader.TempFile);
                    _reader.CurrentFile = _reader.OriginalFile;
                }

                if (e.Cancelled)
                {
                    _FileState = FileState.NoFile;
                    ShowStatus("File open was cancelled.", false);
                }
                else
                {
                    Application.DoEvents();
                    statusMsg.Visible = false;

                    _FileState = FileState.Loaded;

                    // If any filters are set the buttons, menus, and column header images accordingly.
                    FilterAddedOrRemoved(null, null);

                    if (_records.Count == 0)
                    {
                        ShowMessageBox("No records were read from the file.");
                        if (_reader.CurrentSession != null) ZeroTime = _reader.CurrentSession.CreationTimeUtc;
                    }
                    else
                    {
                        ZeroTime = _records[0].Time;
                        RebuildAllRows();
                        RestoreScrollPosition(_rowNumToRestore);
                    }

                    if (Settings.Default.AutoUpdate && _reader.CurrentSession != null)
                    {
                        StartFileWatcher();
                    }
                }
            }
        }

        private void RestoreScrollPosition(int rowNum)
        {
            // Attempt to maintain the same scroll position as before the refresh.
            try
            {
                if (rowNum == -1 || rowNum >= NumRows)
                {
                    // Go to the very end and select the last row so the next refresh will also
                    // scroll to the end.
                    SelectRowIndex(NumRows - 1);
                }
                else
                {
                    // Scroll to the same index as before the refresh.
                    // For some reason, setting the TopItem once doesn't work.  Setting
                    // it three times usually does, so try up to four.
                    for (int i = 0; i < 4; ++i)
                    {
                        if (TheListView.TopItem.Index == rowNum) break;      // Exception reported here.
                        Debug.Print("Setting TopItem index to " + rowNum);
                        TheListView.TopItem = (TheListView.Items[rowNum]);
                    }
                }
            }
            catch (Exception)
            {
                // Restoring the scroll position isn't critical.  Just let it go.
            }
        }

        private bool WatchingFile { get { return !IsDisposed && _watcher != null && !_watcher.Stopped; } }

        private void StartFileWatcher()
        {
            using (Log.InfoCall())
            {
                if (_reader.FormatVersion < 6) return;

                autoUpdate.Checked = true;

                if (WatchingFile)
                {
                    Log.Info("Already watching file.");
                }
                else
                {
                    if (_fileServer == null)
                    {
                        Log.Info("Starting local file watcher.");
                        _watcher = new LocalFileWatcher(_filepath, _reader.FileGuid);
                    }
                    else
                    {
                        try
                        {
                            Log.Info("Starting remote file watcher.");

                            if (_reader.CanUseChangeEvent)
                            {
                                _watcher = new RemoteFileWatcher(_filepath, _reader.FileGuid, _fileServer.HostAndPort, _fileServer.GetCreds());
                            }
                            else
                            {
                                _watcher = new RemoteFileWatcher(_filepath, Guid.Empty, _fileServer.HostAndPort, _fileServer.GetCreds());
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowMessageBox("An error occurred trying to monitor the file " + _filepath + " on server " + _fileServer.HostAndPort + ":\n\n" + ex.Message);
                            return;
                        }
                    }

                    // The FileWatcher events are called on a worker thread.  For thread-safety, it seems
                    // better to handle the events in the main GUI thread by calling Invoke() so the events
                    // are handled serially.  BeginInvoke() was tried, but it caused the GUI to freeze and/or
                    // flicker when the file was updated frequently.

                    _watcher.Changed += new FileSystemEventHandler(_watcher_Changed);
                    _watcher.Renamed += new RenamedEventHandler(_watcher_Renamed);

                    // Simulate the Changed event in case we missed any changes while we weren't watching.
                    FileChanged(null, new FileSystemEventArgs(WatcherChangeTypes.Changed, Path.GetDirectoryName(_filepath), Path.GetFileName(_filepath)));
                }
            }
        }

        void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            using (Log.InfoCall())
            {
                if (IsDisposed)
                {
                    Log.Warn("MainForm has been disposed.");
                }
                else
                {
                    try
                    {
                        // Calls FileChanged().  Sometimes throws an exception because the 
                        // form has been disposed, despite the above check.
                        Invoke(_fileChangedDelegate, sender, e);
                    }
                    catch { }
                }
            }
        }

        void _watcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (!IsDisposed) Invoke(_fileRenamedDelegate, sender, e);
        }

        private void StopFileWatcher()
        {
            if (!InvokeRequired) autoUpdate.Checked = false;

            if (WatchingFile)
            {
                // This seems backwards, but we do not want to call _watcher.Stop() in the main
                // GUI thread.  Doing so leads to deadlocks.  The typical scenario is that the _watcher.Change
                // or _watcher.Rename event is called on a worker thread, which then calls Invoke() to
                // do most of the processing in the GUI thread.  The GUI thread may call StopFileWatcher(), which 
                // calls _watcher.Stop, which blocks until the event handler returns.  However, the event
                // handler thread is waiting for the Invoke() call to return, which is stuck in Stop().
                if (InvokeRequired)
                {
                    Debug.Print("Stopping watcher.");
                    var temp = _watcher;
                    _watcher = null;
                    temp.Stop();
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(StopFileWatcher));
                }
            }
        }

        private void StopFileWatcher(object o)
        {
            StopFileWatcher();
        }

        void FileChanged(object sender, FileSystemEventArgs e)
        {
            using (Log.DebugCall())
            {
                try
                {
                    if (!WatchingFile) return;

                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    Log.Debug(e.Name + " " + e.ChangeType);

                    if (e.ChangeType == WatcherChangeTypes.Changed)
                    {
                        var scrollPos = GetRowNumToRestore();
                        var originalCount = _records.Count;
                        var session = _reader.CurrentSession;
                        var originalLastNonCircularRecord = session.LastNonCircularRecord;
                        Record firstGeneratedRec = null;
                        bool firstIteration = true;

                        while (WatchingFile && session != null)
                        {
                            if (_records.Count < Settings.Default.MaxRecords)
                            {
                                var newRecords = session.ReadMoreRecords(Settings.Default.MaxRecords - _records.Count);

                                if (firstIteration)
                                {
                                    firstIteration = false;
                                    firstGeneratedRec = session.GeneratedRecords.FirstOrDefault();
                                    //Record firstGeneratedRec = session.GeneratedRecords.Count > 0 ? session.GeneratedRecords[0] : null;
                                }

                                if (newRecords == null)
                                {
                                    autoUpdate.Enabled = false;
                                    StopFileWatcher();
                                    ShowMessageBox("An error occurred reading the log file.  The logger may have overwritten the location currently being read (due to wrapping).  The file will no longer be monitored for changes unless you reload/refresh it.");
                                }
                                else
                                {
                                    Log.DebugFormat("{0} records exist, {1} new records read.", _records.Count, newRecords.Count);

                                    if (newRecords.Count > 0)
                                    {
                                        // Set the collapse depth of the new and generated records.
                                        foreach (Record rec in newRecords) SetCollapseDepth(rec);
                                        foreach (Record rec in session.GeneratedRecords) SetCollapseDepth(rec);

                                        for (int i = 0; i < newRecords.Count; ++i) newRecords[i].Index = i + _records.Count;
                                        _records.AddRange(newRecords);
                                        session.InsertMissingRecords(_records);
                                    }
                                    else
                                    {
                                        // Nothing to do.
                                        Log.Debug("0 new records read.");
                                    }
                                }

                            }

                            if (_records.Count < Settings.Default.MaxRecords)
                            {
                                // This will get a new Session object if we've read all the records
                                // in the current session and there another session in the file.

                                session = _reader.NextSession();
                            }
                            else
                            {
                                StopFileWatcher();
                                ShowMessageBox("Auto-update has been turned off to prevent the TracerX viewer from using too much memory.  To see the most recent output added to the file, either reload the file or increase the maximum number of records on the Auto-Update tab of the Options dialog.");

                            }
                        } // while

                        if (_records.Count > originalCount)
                        {
                            if (originalCount == 0)
                            {
                                Log.Debug("Rebuilding all rows.");
                                RebuildAllRows();
                            }
                            else
                            {
                                if (firstGeneratedRec == null)
                                {
                                    // Since no generated records were inserted in the first iteration, we will rebuild the rows starting
                                    // with the first new one.
                                    RebuildRows(NumRows, _records[originalCount]);
                                }
                                else
                                {
                                    int visibleRecNdx = FindFirstVisibleUp(originalLastNonCircularRecord.Index);

                                    if (visibleRecNdx == -1)
                                    {
                                        RebuildRows(0, firstGeneratedRec);
                                    }
                                    else
                                    {
                                        RebuildRows(_records[visibleRecNdx].LastRowIndex + 1, firstGeneratedRec);
                                    }
                                }

                                RestoreScrollPosition(scrollPos);
                            }
                        }

                        Log.DebugFormat("Time to handle new records = {0}ms.", sw.ElapsedMilliseconds);
                    }
                    else
                    {
                        // Stop watching when file is renamed, replaced, etc.
                        autoUpdate.Enabled = false;
                        StopFileWatcher();
                        ShowMessageBox("The log file has been renamed, replaced, or deleted.  It will no longer be monitored for changes unless you reload/refresh it.");
                    }
                }
                catch (Exception ex)
                {
                    // This used to occur when all rows were hidden due to filtering.  Fixed in releast 4.1.
                    Log.Warn("Exception in FileChanged: " + ex.ToString());
                }
            }
        }

        void FileRenamed(object sender, RenamedEventArgs e)
        {
            using (Log.InfoCall())
            {
                Debug.Print(String.Format("{0} renamed to {1}({2})", e.OldName, e.Name, e.ChangeType));
                autoUpdate.Enabled = false;
                StopFileWatcher();

                if (Settings.Default.AutoReload)
                {
                    // Allow some time for new log file to be started.

                    Thread.Sleep(500);
                    Application.DoEvents();
                    StartReading(null, null); // Null means refresh the current file.
                    RestoreScrollPosition(-1);
                }
                else
                {
                    ShowMessageBox("The log file has been renamed.  It will no longer be monitored for changes unless you reload/refresh it.");
                }
            }
        }

        private void SetCollapseDepth(Record rec)
        {
            rec.CollapsedDepth = 0;
            Record caller = rec.Caller;

            while (caller != null)
            {
                if (caller.IsCollapsed)
                {
                    ++rec.CollapsedDepth;
                }

                caller = caller.Caller;
            }
        }

        private static void ResetFilters(bool reuseFilters)
        {
            ThreadObjects.Clear();
            ThreadNames.Clear();
            LoggerObjects.Clear();
            MethodObjects.Clear();
            SessionObjects.Clear();

            if (!reuseFilters)
            {
                TraceLevelObjects.ShowAllLevels();
            }
        }

        #endregion

        // Increment or decrement the CollapsedDepth of every record between this one and 
        // the corresponding Exit record (for the same thread).
        private void ExpandCollapseMethod(Record methodEntryRecord)
        {
            short diff = (short)(methodEntryRecord.IsCollapsed ? -1 : 1);

            methodEntryRecord.IsCollapsed = !methodEntryRecord.IsCollapsed;

            for (int i = methodEntryRecord.Index + 1; i < _records.Count; ++i)
            {
                Record current = _records[i];
                if (current.ThreadId == methodEntryRecord.ThreadId)
                {
                    // Current record is from the same thread as the clicked record.
                    if (current.StackDepth == methodEntryRecord.StackDepth)
                    {
                        // This is the Exit record.
                        break;
                    }
                    else
                    {
                        current.CollapsedDepth += diff;
                    }
                }
            }
        }

        private void expandAllButton_Click(object sender, EventArgs e)
        {
            foreach (Record rec in _records)
            {
                rec.IsCollapsed = false;
                rec.CollapsedDepth = 0;
            }

            RebuildAllRows();
        }

        private void collapseAllButton_Click(object sender, EventArgs e)
        {
            foreach (Record rec in _records)
            {
                // Setting IsCollapsed = true only makes sense for method-entry
                // records and records with embedded newlines.

                if (rec.IsEntry || rec.HasNewlines)
                {
                    rec.IsCollapsed = true;
                }

                rec.CollapsedDepth = rec.StackDepth;
            }

            RebuildAllRows();
        }

        // Reset the _rows elements from startRow forward.
        // The specified Record (rec) is the first Record whose
        // visibility may need recalculating.  The specified
        // startRow will be mapped to the first visible Record found.
        private void RebuildRows(int startRow, Record rec)
        {
            Debug.Print("RebuildRows entered");

            // Display the wait cursor if processing takes more than 500 milliseconds.
            DateTime timeForWaitCursor = DateTime.Now.AddMilliseconds(500);
            Cursor restoreCursor = null;

            // Try to restore the scroll position when we're finished processing.
            Record showRec = null; // The record whose line has the focus.
            int showLine = 0;      // The line within the record that has the focus.
            int offset = 0;        // The offset of the focused row from the top row.

            // If startRow = NumRows, we're only adding rows (probably called by FileChanged).
            // In that case, the scroll position and SelectedItems won't be affected.
            // Also, TheListView.TopItem must not be null because it's the basis of the scroll position.
            if (startRow < NumRows && TheListView.TopItem != null)
            {
                // If the ListViewItem with the focus is currently visible, remember its offset from
                // the top of TheListView so we can later scroll it back to the same spot.

                if (TheListView.FocusedItem != null &&
                    TheListView.ClientRectangle.IntersectsWith(TheListView.FocusedItem.Bounds))
                {
                    Row row = Rows[TheListView.FocusedItem.Index];
                    showRec = row.Rec;
                    showLine = row.Line;
                    offset = TheListView.FocusedItem.Index - TheListView.TopItem.Index;
                }
                else
                {
                    // The focused item isn't visible, so just plan on scrolling the
                    // the current top item back to the top position.

                    Row row = Rows[TheListView.TopItem.Index];
                    showRec = row.Rec;
                    showLine = row.Line;
                    offset = 0;
                }
            }

            int curRow = startRow;
            int curRec = rec.Index;
            char indentChar = Settings.Default.IndentChar;
            int indentRate = Settings.Default.IndentAmount;

            while (curRec < _records.Count)
            {
                if (restoreCursor == null && DateTime.Now > timeForWaitCursor)
                {
                    restoreCursor = this.Cursor;
                    this.Cursor = Cursors.WaitCursor;
                }

                curRow = _records[curRec].SetVisibleRows(Rows, curRow, indentChar, indentRate);
                ++curRec;
            }

            TheListView.ClearItemCache(startRow);

            if (startRow == NumRows)
            {
                // In this case we are adding new rows for new records rather
                // than unhiding existing records.  We can therefore use this
                // kludge that prevents flickering by not invalidating.
                TheListView.SetVirtualListSize(curRow, false);
            }
            else
            {
                // In this case we are probably expanding/collapsing a method call
                // or changing the filter.  Allow it to invalidate.
                TheListView.SetVirtualListSize(curRow, true);
                //NumRows = curRow;
            }

            // Disable Find and FindNext/F3 if no text is visible.
            UpdateFindCommands();
            UpdateThreadButtons();
            UpdateTimeButtons();

            if (showRec != null)
            {
                int showRow = showRec.RowIndices[showLine];

                // If showRec is no longer visible, search up for the nearest record that is.
                while (!showRec.IsVisible && showRec.Index > 0)
                {
                    showRec = _records[showRec.Index - 1];
                    showRow = showRec.FirstRowIndex;
                }

                // If we found a visible record, scroll to it.
                if (showRec.IsVisible)
                {
                    if (showRow == -1 || !showRec.HasNewlines || showRec.IsCollapsed)
                        showRow = showRec.FirstRowIndex;

                    SelectRowIndex(showRow);
                    int top = showRow - offset;
                    if (top > 0 && top < TheListView.Items.Count)
                    {
                        //Debug.Print("setting top " + top);
                        TheListView.TopItem = TheListView.Items[top];
                    }
                }
            }

            if (restoreCursor != null)
            {
                this.Cursor = restoreCursor;
            }

            Debug.Print("RebuildRows exiting");
        }

        private int FindFirstVisibleUp(int recNdx)
        {
            while (recNdx >= 0 && !_records[recNdx].IsVisible)
            {
                --recNdx;
            }

            return recNdx;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            TheMainForm = null;
            StopFileWatcher();
            base.OnClosing(e);
        }

        // OnHandleDestroyed seems to get called regardless of whether this form is
        // treated as a user control or not.
        protected override void OnHandleDestroyed(EventArgs e)
        {
            TheMainForm = null;
            StopFileWatcher();
            SaveSettings();
            base.OnHandleDestroyed(e);
        }

        private void SaveSettings()
        {
            // Persist column widths in Settings.
            int[] widths = new int[OriginalColumns.Length];
            int[] indices = new int[OriginalColumns.Length];
            bool[] selections = new bool[OriginalColumns.Length];
            for (int i = 0; i < OriginalColumns.Length; ++i)
            {
                widths[i] = OriginalColumns[i].Width;
                indices[i] = OriginalColumns[i].DisplayIndex;
                selections[i] = TheListView.Columns.Contains(OriginalColumns[i]);
            }
            Settings.Default.ColWidths = widths;
            Settings.Default.ColIndices = indices;
            Settings.Default.ColSelections = selections;

            Settings.Default.Save();
        }

        private void ExecuteOpenFile(object sender, EventArgs e)
        {
            BrowseForFile(Settings.Default.OpenDir);
        }

        private void BrowseForFile(string initialDir)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            if (initialDir != null && initialDir != string.Empty && Directory.Exists(initialDir))
                dlg.InitialDirectory = initialDir;
            else
                dlg.InitialDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);

            dlg.AddExtension = true;
            dlg.DefaultExt = ".tx1";
            dlg.Filter = "TracerX files (*.tx1)|*.tx1|All files (*.*)|*.*";
            dlg.Multiselect = false;
            dlg.Title = Application.ProductName;

            if (dlg.ShowDialog() == true)
            {
                Settings.Default.OpenDir = Path.GetDirectoryName(dlg.FileName);
                StartReading(dlg.FileName, null);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (_FileState)
            {
                case FileState.NoFile:
                    this.Close();
                    break;
                case FileState.Loaded:
                    CloseFile();
                    this.Close();
                    break;
                case FileState.Loading:
                    this.Close();
                    break;
            }
        }

        public void CloseFile()
        {
            if (_FileState == FileState.Loaded)
            {
                StopFileWatcher();
                _FileState = FileState.NoFile;
                statusMsg.Visible = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void TheListView_MouseClick(object sender, MouseEventArgs e)
        {
            Debug.Print("TheListView_MouseClick");

            // The only supported single-click action is to expand or collapse a
            // method call or multi-line message.  This only applies if the user
            // clicks the image area (where the '+' and '-' icons appear) of the
            // text field.

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ListViewHitTestInfo hitTestInfo = TheListView.HitTest(e.X, e.Y);

                if (hitTestInfo != null && hitTestInfo.Item != null && hitTestInfo.SubItem != null)
                {
                    ViewItem item = hitTestInfo.Item as ViewItem;
                    Row row = item.Row;
                    Record record = row.Rec;
                    int ndx = item.SubItems.IndexOf(hitTestInfo.SubItem);

                    if (ndx == headerText.Index)
                    {
                        // We know the user clicked the Text field, but did he click in the first 20 pixels?
                        // If he did, a HitTest done 20 pixels to the left would not return the same subitem.

                        ListViewHitTestInfo hitTest2 = TheListView.HitTest(e.X - 20, e.Y);

                        if (hitTest2.SubItem != hitTestInfo.SubItem)
                        {
                            var restorCursor = this.Cursor;
                            this.Cursor = Cursors.WaitCursor;

                            if (record.IsEntry)
                            {
                                // Expand or collapse the method call.
                                ExpandCollapseMethod(record);
                                RebuildRows(row.Index, row.Rec);
                            }
                            else if (record.HasNewlines && row.Index == record.FirstRowIndex)
                            {
                                // Expand or collapse the record with embedded newlines.
                                record.IsCollapsed = !record.IsCollapsed;
                                RebuildRows(row.Index, row.Rec);
                            }

                            this.Cursor = restorCursor;
                        }
                    }
                }
            }
        }

        private void Colorize(Record record, ColorDriver driverType)
        {
            //if (Settings.Default.ColoringEnabled)
            {
                IFilterable colorableItem = record.GetColorableItem(driverType);

                // ColorableItem will be null if user clicked something that isn't IFilterable
                // (like the line number or text field).

                if (colorableItem != null)
                {
                    if (colorableItem.SubitemColors == null)
                    {
                        if (colorableItem == record.TLevel)
                        {
                            // For Trace Levels, always use the predefined color assigned to that level.
                            record.TLevel.SubitemColors = ColorUtil.TraceLevelPalette[record.TLevel.TLevel];
                        }
                        else
                        {
                            colorableItem.SubitemColors = ColorUtil.FirstAvailableColor();

                            if (colorableItem.SubitemColors == null)
                            {
                                ShowMessageBox("Sorry, all the colors in the color palette have been used.");
                            }
                            else
                            {
                                ColorUtil.UsedSubitemColors.Add(colorableItem.SubitemColors);
                            }
                        }
                    }
                    else
                    {
                        // Color was already set, toggle it off.
                        ColorUtil.UsedSubitemColors.Remove(colorableItem.SubitemColors);
                        colorableItem.SubitemColors = null;
                    }

                    InvalidateTheListView();
                }
            }
        }

        private void TheListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hitTestInfo = TheListView.HitTest(e.X, e.Y);

            if (hitTestInfo != null && hitTestInfo.Item != null)
            {
                ViewItem item = hitTestInfo.Item as ViewItem;
                Row row = item.Row;
                Record record = row.Rec;
                int ndx = item.SubItems.IndexOf(hitTestInfo.SubItem);

                if (ndx == headerLevel.Index)
                {
                    Colorize(record, ColorDriver.TraceLevels);
                }
                else if (ndx == headerLogger.Index)
                {
                    Colorize(record, ColorDriver.Loggers);
                }
                else if (ndx == headerMethod.Index)
                {
                    Colorize(record, ColorDriver.Methods);
                }
                else if (ndx == headerSession.Index)
                {
                    Colorize(record, ColorDriver.Sessions);
                }
                else if (ndx == headerThreadId.Index)
                {
                    Colorize(record, ColorDriver.ThreadIDs);
                }
                else if (ndx == headerThreadName.Index)
                {
                    Colorize(record, ColorDriver.ThreadNames);
                }
                else if (ndx == headerLine.Index)
                {
                    // Currently, no action is defined for double clicking a line number
                }
                else if (ndx == headerText.Index)
                {
                    // If the user clicked to the right of the image area (20 pixels), display the record text in a window.

                    ListViewHitTestInfo hitTest2 = TheListView.HitTest(e.X - 20, e.Y);

                    if (hitTest2.SubItem == hitTestInfo.SubItem)
                    {
                        row.ShowFullText();
                    }
                }
                else if (ndx == headerTime.Index)
                {
                    // Cycle between absolute time, time relative to clicked row, and time relative to first row.

                    if (Settings.Default.RelativeTime)
                    {
                        if (record.Time == ZeroTime && record.Time != _records[0].Time)
                        {
                            // Change to first record's time.
                            ZeroTime = _records[0].Time;
                            InvalidateTheListView();
                        }
                        else
                        {
                            // Change to absolute time.
                            Settings.Default.RelativeTime = false;
                        }
                    }
                    else
                    {
                        // Change to clicked record's time.
                        ZeroTime = record.Time;
                        Settings.Default.RelativeTime = true;
                    }
                }
            }
        }

        private void ExecuteProperties(object sender, EventArgs e)
        {
            FileProperties dlg = new FileProperties(_reader);
            dlg.ShowDialog();
        }

        #region Bookmarks
        private void ExecuteToggleBookmark(object sender, EventArgs e)
        {
            if (TheListView.FocusedItem != null)
            {
                Row row = TheListView.FocusedItem.Tag as Row;
                row.IsBookmarked = !row.IsBookmarked;

                if (row.IsBookmarked)
                {
                    bookmarkPrevCmd.Enabled = true;
                    bookmarkNextCmd.Enabled = true;
                    bookmarkClearCmd.Enabled = true;
                }

                // row.ImageIndex is determined by IsBookmarked.
                TheListView.FocusedItem.ImageIndex = row.ImageIndex;
                TheListView.Invalidate(TheListView.FocusedItem.GetBounds(ItemBoundsPortion.Icon));
            }
        }

        private void ExecuteClearBookmarks(object sender, EventArgs e)
        {
            // We must visit every record, including those that are collapsed/hidden.
            foreach (Record rec in _records)
            {
                for (int i = 0; i < rec.IsBookmarked.Length; ++i)
                {
                    rec.IsBookmarked[i] = false;
                }
            }

            bookmarkPrevCmd.Enabled = false;
            bookmarkNextCmd.Enabled = false;
            bookmarkClearCmd.Enabled = false;

            InvalidateTheListView();
        }

        // Search for a bookmarked row from start to just before end.
        private bool FindBookmark(int start, int end)
        {
            int moveBy = (start < end) ? 1 : -1;

            statusMsg.Visible = false;

            for (int i = start; i != end; i += moveBy)
            {
                if (Rows[i].IsBookmarked)
                {
                    SelectFoundIndex(i);
                    return true;
                }
            }

            return false;
        }

        private void ExecuteNextBookmark(object sender, EventArgs e)
        {
            int start = 0;

            if (TheListView.FocusedItem != null)
            {
                start = TheListView.FocusedItem.Index + 1;
            }

            if (!FindBookmark(start, NumRows))
            {
                // Wrap back to the first row.
                if (FindBookmark(0, start))
                {
                    ShowStatus("Passed end of file.", false);
                }
                else
                {
                    ShowStatus("No bookmarks.", true);
                }
            }
        }

        private void ExecutePrevBookmark(object sender, EventArgs e)
        {
            int start = NumRows - 1;

            if (TheListView.FocusedItem != null)
            {
                start = TheListView.FocusedItem.Index - 1;
            }

            if (!FindBookmark(start, -1))
            {
                // Wrap back to the last row.
                if (FindBookmark(NumRows - 1, start))
                {
                    ShowStatus("Passed end of file.", false);
                }
                else
                {
                    ShowStatus("No bookmarks.", true);
                }
            }
        }
        #endregion Bookmarks

        // Display the Find dialog.
        private void ExecuteFind(object sender, EventArgs e)
        {
            FindDialog dlg = new FindDialog(this);
            dlg.Show(this);
        }

        // Search down for the current search string.
        private void ExecuteFindNext(object sender, EventArgs e)
        {
            if (_textMatcher != null) DoSearch(_textMatcher, false, false);
        }

        // Search up for the current search string.
        private void ExecuteFindPrevious(object sender, EventArgs e)
        {
            if (_textMatcher != null) DoSearch(_textMatcher, true, false);
        }

        // Clear all filtering.
        private void ExecuteClearFilter(object sender, EventArgs e)
        {
            SessionObjects.ShowAllSessions();
            ThreadObjects.ShowAllThreads();
            ThreadNames.ShowAllThreads();
            LoggerObjects.ShowAllLoggers();
            MethodObjects.ShowAllMethods();
            TraceLevelObjects.ShowAllLevels();
            FilterDialog.TextFilterDisable();

            int horizontalScroll = TheListView.HScrollPos;
            RebuildAllRows();
            //TheListView.HScrollPos = horizontalScroll;
        }

        // Called when the first filter is added or the last filter is
        // removed for a class of objects such as loggers or threads,
        // or whenever the presence/status of the filter icons that appear
        // in the column headers and the "clear all filtering" commands 
        // may need to be updated.
        private void FilterAddedOrRemoved(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                // This means we were called in a worker thread that's reading the file.
                // The main thread will call again when loading is finished.

                //Invoke(new EventHandler(FilterAddedOrRemoved), sender, e);
                return;
            }

            filterClearCmd.Enabled = false;

            if (TraceLevelObjects.AllVisible)
            {
                headerLevel.ImageIndex = -1;
                headerLevel.TextAlign = headerLevel.TextAlign; // Fixes problem with wrong image appearing in Vista+.
            }
            else
            {
                headerLevel.ImageIndex = 9;
                filterClearCmd.Enabled = true;
            }

            if (SessionObjects.AllVisible)
            {
                headerSession.ImageIndex = -1;
                headerSession.TextAlign = headerSession.TextAlign; // Fixes problem with wrong image appearing in Vista+.
            }
            else
            {
                // Show a filter in the header so user knows a filter is applied to the column.
                headerSession.ImageIndex = 9;
                filterClearCmd.Enabled = true;
            }

            if (ThreadNames.AllVisible)
            {
                headerThreadName.ImageIndex = -1;
                headerThreadName.TextAlign = headerThreadName.TextAlign; // Fixes problem with wrong image appearing in Vista+.
            }
            else
            {
                // Show a filter in the header so user knows a filter is applied to the column.
                headerThreadName.ImageIndex = 9;
                filterClearCmd.Enabled = true;
            }

            if (ThreadObjects.AllVisible)
            {
                headerThreadId.ImageIndex = -1;
                headerThreadId.TextAlign = headerThreadId.TextAlign; // Fixes problem with wrong image appearing in Vista+.
            }
            else
            {
                // Show a filter in the header so user knows a filter is applied to the column.
                headerThreadId.ImageIndex = 9;
                filterClearCmd.Enabled = true;
            }

            if (LoggerObjects.AllVisible)
            {
                headerLogger.ImageIndex = -1;
                headerLogger.TextAlign = headerLogger.TextAlign; // Fixes problem with wrong image appearing in Vista+.
            }
            else
            {
                // Show a filter in the header so user knows a filter is applied to the column.
                headerLogger.ImageIndex = 9;
                filterClearCmd.Enabled = true;
            }

            if (MethodObjects.AllVisible)
            {
                headerMethod.ImageIndex = -1;
                headerMethod.TextAlign = headerMethod.TextAlign; // Fixes problem with wrong image appearing in Vista+.
            }
            else
            {
                // Show a filter in the header so user knows a filter is applied to the column.
                headerMethod.ImageIndex = 9;
                filterClearCmd.Enabled = true;
            }

            if (FilterDialog.TextFilterOn)
            {
                // Show a filter in the header so user knows a filter is applied to the column.
                headerText.ImageIndex = 9;
                filterClearCmd.Enabled = true;
            }
            else
            {
                headerText.ImageIndex = -1;
                headerText.TextAlign = headerText.TextAlign; // Fixes problem with wrong image appearing in Vista+.
            }
        }

        private void mnuShowSelected_Click(object sender, EventArgs e)
        {
            // Show all but the selected <whatever>.

            QuickFilter(showSelected:true);
        }
        
        private void mnuHideSelected_Click(object sender, EventArgs e)
        {
            // Hide the selected <whatever>.

            QuickFilter(showSelected: false);
        }

        private void QuickFilter(bool showSelected)
        {
            if (_clickedColumn == headerLevel)
            {
                if (showSelected) TraceLevelObjects.HideAllLevels();

                foreach (int index in TheListView.SelectedIndices)
                {
                    Rows[index].Rec.TLevel.Visible = showSelected;
                }
            }
            else if (_clickedColumn == headerLine)
            {
                // Not supported and shouldn't be possible.
            }
            else if (_clickedColumn == headerLogger)
            {
                if (showSelected) LoggerObjects.HideAllLoggers();

                foreach (int index in TheListView.SelectedIndices)
                {
                    Rows[index].Rec.Logger.Visible = showSelected;
                }
            }
            else if (_clickedColumn == headerMethod)
            {
                if (showSelected) MethodObjects.HideAllMethods();

                foreach (int index in TheListView.SelectedIndices)
                {
                    Rows[index].Rec.MethodName.Visible = showSelected;
                }
            }
            else if (_clickedColumn == headerSession)
            {
                if (showSelected) SessionObjects.HideAllSessions();

                foreach (int index in TheListView.SelectedIndices)
                {
                    Rows[index].Rec.Session.Visible = showSelected;
                }
            }
            else if (_clickedColumn == headerText)
            {
                // Not supported and shouldn't be possible.
            }
            else if (_clickedColumn == headerThreadId)
            {
                if (showSelected) ThreadObjects.HideAllThreads();

                foreach (int index in TheListView.SelectedIndices)
                {
                    Rows[index].Rec.Thread.Visible = showSelected;
                }
            }
            else if (_clickedColumn == headerThreadName)
            {
                if (showSelected) ThreadNames.HideAllThreads();

                foreach (int index in TheListView.SelectedIndices)
                {
                    Rows[index].Rec.ThreadName.Visible = showSelected;
                }
            }
            else if (_clickedColumn == headerTime)
            {
                // Not supported and shouldn't be possible.
            }

            RebuildAllRows();
        }        

        // Show the column selection dialog.
        private void ExecuteColumns(object sender, EventArgs e)
        {
            ColumnsDlg dlg = new ColumnsDlg(this);
            dlg.ShowDialog(this);
        }

        // Display the call stack of the selected record.
        private void showCallStackMenuItem_Click(object sender, EventArgs e)
        {
            // Search for the callers (call stack) of the currently selected record by
            // looking at the StackDepth of preceding records from the same thread.
            // This was implemented before the Caller field was added to the Record class,
            // and works for file versions before 5.

            // This is the list of records that comprise the stack.
            List<Record> stack = new List<Record>();
            Row startRow = Rows[TheListView.SelectedIndices[0]];
            Cursor restoreCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                Record curRec = FindCaller(startRow.Rec);

                while (curRec != null)
                {
                    stack.Add(curRec);
                    curRec = FindCaller(curRec);
                }
            }
            finally
            {
                this.Cursor = restoreCursor;
            }

            stack.Reverse();
            CallStack dlg = new CallStack(startRow, stack);
            dlg.ShowDialog(this);
        }

        private Record FindCaller(Record start)
        {
            // We are only interested in records whose stack depth is less than curDepth.
            int curDepth = start.StackDepth;

            if (start.IsExit) ++curDepth;

            for (int index = start.Index - 1; index > -1 && curDepth > 0; --index)
            {
                Record curRec = _records[index];
                if (curRec.Thread == start.Thread)
                {
                    if (curRec.StackDepth < curDepth)
                    {
                        curDepth = curRec.StackDepth;
                        if (curRec.IsEntry)
                        {
                            return curRec;
                        }
                    }
                }
            }

            // Did not find the caller.
            return null;
        }

        // Scroll to the start of the method for the current record if visible.
        private void startOfMethodMenuItem_Click(object sender, EventArgs e)
        {
            Row startRow = Rows[TheListView.SelectedIndices[0]];
            Record caller;
            Cursor restoreCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            statusMsg.Visible = false;

            try
            {
                caller = FindCaller(startRow.Rec);
            }
            finally
            {
                this.Cursor = restoreCursor;
            }

            if (caller == null)
            {
                // The caller was possibly lost in circular wrapping.
                ShowStatus("The caller was not found.", true);
            }
            else if (caller.IsVisible)
            {
                // Scroll to and select the row/item for the caller record.
                this.SelectFoundIndex(caller.FirstRowIndex);
            }
            else
            {
                ShowStatus("The caller is not visible due to filtering.", false);
            }
        }

        // Find the end of the current method call and scroll to it.
        private void endOfMethodMenuItem_Click(object sender, EventArgs e)
        {
            Record startRec = CurrentRow.Rec;
            Cursor restoreCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            statusMsg.Visible = false;

            // If the current record is a MethodEntry record, the end of the method is the next
            // record at the same stack depth.  Otherwise, the end of the method is the next
            // record at a lesser stack depth.
            int triggerDepth = startRec.StackDepth;

            if (startRec.IsEntry) ++triggerDepth;

            try
            {
                for (int index = startRec.Index + 1; index < _records.Count; ++index)
                {
                    Record curRec = _records[index];
                    if (curRec.Thread == startRec.Thread)
                    {
                        if (curRec.StackDepth < triggerDepth)
                        {
                            if (curRec.IsVisible)
                            {
                                this.SelectFoundIndex(curRec.FirstRowIndex);
                            }
                            else
                            {
                                ShowStatus("The end of the method call is not visible due to filtering.", false);
                            }
                            return;
                        }
                    }
                }

                ShowStatus("The end of the method call was not found.", true);
            }
            finally
            {
                this.Cursor = restoreCursor;
            }
        }

        private void copyTextMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            foreach (int index in TheListView.SelectedIndices)
            {
                ViewItem item = TheListView.Items[index] as ViewItem;
                builder.Append(item.Row.GetFullIndentedText());
                builder.Append("\n");
            }

            // Remove last newline.
            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 1;
                Clipboard.SetText(builder.ToString());
            }
        }

        private void UpdateMenuItems()
        {
            int selectCount = TheListView.SelectedIndices.Count;
            Record currentRec = CurrentRow == null ? null : CurrentRow.Rec;

            Debug.Print("selectCount: " + selectCount);

            mnuBookmarkSelected.Visible = false;
            mnuHideSelected.Visible = false;
            mnuShowSelected.Visible = false;
            mnuColorSelected.Visible = false;

            if (selectCount == 0 || _clickedColumn == null)
            {
                mnuBookmarkSelected.Visible = false;
                mnuHideSelected.Visible = false;
                mnuShowSelected.Visible = false;
                mnuColorSelected.Visible = false;
            }
            else if (_clickedColumn == headerLevel)
            {
                mnuBookmarkSelected.Visible = true;
                mnuHideSelected.Visible = true;
                mnuShowSelected.Visible = true;
                mnuColorSelected.Visible = true;

                mnuBookmarkSelected.Text = "Bookmark Selected Trace Levels";
                mnuHideSelected.Text = "Hide Selected Trace Levels";
                mnuShowSelected.Text = "Show Only Selected Trace Levels";
                mnuColorSelected.Text = "Color Selected Trace Levels";
            }
            else if (_clickedColumn == headerLine)
            {
                // Nothing for line numbers

                mnuBookmarkSelected.Visible = false;
                mnuHideSelected.Visible = false;
                mnuShowSelected.Visible = false;
                mnuColorSelected.Visible = false;
            }
            else if (_clickedColumn == headerLogger)
            {
                mnuBookmarkSelected.Visible = true;
                mnuHideSelected.Visible = true;
                mnuShowSelected.Visible = true;
                mnuColorSelected.Visible = true;

                mnuBookmarkSelected.Text = "Bookmark Selected Loggers";
                mnuHideSelected.Text = "Hide Selected Loggers";
                mnuShowSelected.Text = "Show Only Selected Loggers";
                mnuColorSelected.Text = "Color Selected Loggers";
            }
            else if (_clickedColumn == headerMethod)
            {
                mnuBookmarkSelected.Visible = false;
                mnuHideSelected.Visible = false;
                mnuShowSelected.Visible = true;
                mnuColorSelected.Visible = true;

                mnuBookmarkSelected.Text = "Bookmark Selected Methods";
                mnuHideSelected.Text = "Hide Selected Methods";
                mnuColorSelected.Text = "Color Selected Methods";

                if (Settings.Default.ShowCalledMethods)
                {
                    mnuShowSelected.Text = "Show Only Selected Methods (and Nested Calls)";
                }
                else
                {
                    mnuShowSelected.Text = "Show Only Selected Methods";
                }
            }
            else if (_clickedColumn == headerSession)
            {
                mnuBookmarkSelected.Visible = false;
                mnuHideSelected.Visible = true;
                mnuShowSelected.Visible = true;
                mnuColorSelected.Visible = true;

                mnuHideSelected.Text = "Hide Selected Sessions";
                mnuShowSelected.Text = "Show Only Selected Sessions";
                mnuColorSelected.Text = "Color Selected Sessions";
            }
            else if (_clickedColumn == headerText)
            {
                // Nothing for the Text column.
            
                mnuBookmarkSelected.Visible = false;
                mnuHideSelected.Visible = false;
                mnuShowSelected.Visible = false;
                mnuColorSelected.Visible = false;
            }
            else if (_clickedColumn == headerThreadId)
            {
                mnuBookmarkSelected.Visible = true;
                mnuHideSelected.Visible = true;
                mnuShowSelected.Visible = true;
                mnuColorSelected.Visible = true;

                mnuBookmarkSelected.Text = "Bookmark Selected Thread IDs";
                mnuHideSelected.Text = "Hide Selected Thread IDs";
                mnuShowSelected.Text = "Show Only Selected Thread IDs";
                mnuColorSelected.Text = "Color Selected Thread IDs";
            }
            else if (_clickedColumn == headerThreadName)
            {
                mnuBookmarkSelected.Visible = true;
                mnuHideSelected.Visible = true;
                mnuShowSelected.Visible = true;
                mnuColorSelected.Visible = true;

                mnuBookmarkSelected.Text = "Bookmark Selected Thread Names";
                mnuHideSelected.Text = "Hide Selected Thread Names";
                mnuShowSelected.Text = "Show Only Selected Thread Names";
                mnuColorSelected.Text = "Color Selected Thread Names";
            }
            else if (_clickedColumn == headerTime)
            {
                // Nothing for the Time column.
            
                mnuBookmarkSelected.Visible = false;
                mnuHideSelected.Visible = false;
                mnuShowSelected.Visible = false;
                mnuColorSelected.Visible = false;
            }
            else
            {
                mnuBookmarkSelected.Visible = false;
                mnuHideSelected.Visible = false;
                mnuShowSelected.Visible = false;
                mnuColorSelected.Visible = false;
            }

            copyTextMenuItem.Enabled = selectCount > 0;
            copyColsMenuItem.Enabled = selectCount > 0;
            showCallStackMenuItem.Enabled = selectCount == 1;
            startOfMethodMenuItem.Enabled = selectCount == 1;
            endOfMethodMenuItem.Enabled = selectCount == 1;
            viewTextWindowToolStripMenuItem.Enabled = selectCount == 1;
            setZeroTimeToolStripMenuItem.Enabled = selectCount == 1;

            if (selectCount == 1)
            {
                if (currentRec.StackDepth == 0)
                {
                    showCallStackMenuItem.Enabled = currentRec.IsExit;
                    startOfMethodMenuItem.Enabled = currentRec.IsExit;
                    endOfMethodMenuItem.Enabled = currentRec.IsEntry;
                }
            }

            // Try to include the method name in the "Goto start of method" menu item.
            if (startOfMethodMenuItem.Enabled)
            {
                if (currentRec.IsEntry)
                {
                    // In file format 5, Record.Caller is a direct reference to the caller of the 
                    // current line.  If that's available, use it.  If not, don't take the time to
                    // search for the caller just to set the menu item text.

                    if (currentRec.Caller == null)
                    {
                        startOfMethodMenuItem.Text = "Start of caller";
                    }
                    else
                    {
                        startOfMethodMenuItem.Text = "Start of caller: " + currentRec.Caller.MethodName.Name;
                    }
                }
                else
                {
                    startOfMethodMenuItem.Text = "Start of method: " + currentRec.MethodName.Name;
                }
            }
            else
            {
                startOfMethodMenuItem.Text = "Start of method";
            }

            // Try to include the method name in the "Goto end of method" menu item.
            if (endOfMethodMenuItem.Enabled)
            {
                if (currentRec.IsExit)
                {
                    // We're already at the end of the current method, so this command will actually
                    // scroll to the end of the caller.  Try to get the caller's name.

                    if (currentRec.Caller == null || currentRec.Caller.Caller == null)
                    {
                        endOfMethodMenuItem.Text = "End of caller";
                    }
                    else
                    {
                        endOfMethodMenuItem.Text = "End of caller: " + currentRec.Caller.Caller.MethodName.Name;
                    }
                }
                else
                {
                    endOfMethodMenuItem.Text = "End of method: " + currentRec.MethodName.Name;
                }
            }
            else
            {
                endOfMethodMenuItem.Text = "End of method";
            }

            UpdateUncolorMenu();
        }

        // Enable or disable find, find next, find prev.  It is not sufficient to only do this when 
        // the Edit menu is opening because these commands also have shortcut keys and toolbar buttons.
        private void UpdateFindCommands()
        {
            findCmd.Enabled = _FileState == FileState.Loaded && NumRows > 0;
            findNextCmd.Enabled = findPrevCmd.Enabled = (findCmd.Enabled && _textMatcher != null);
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateMenuItems();
        }

        private ColumnHeader GetColumnAt(Point screenPoint, out bool isInHeader)
        {
            ColumnHeader result = null;

            // This call indirectly calls EnumWindowCallBack which sets _headerRect
            // to the area occupied by the ListView's header bar.
            NativeMethods.EnumChildWindows(TheListView.Handle, new NativeMethods.EnumWinCallBack(EnumWindowCallBack), IntPtr.Zero);

            isInHeader = _headerRect.Contains(screenPoint);

            // The xoffset is how far the point is from the left edge of the header rect.
            int xoffset = screenPoint.X - _headerRect.Left;

            // Iterate through the columns in the order they are displayed, adding up
            // their widths as we go.  When the sum exceeds the xoffset, we know the mouse
            // is on the current column. 
            int sum = 0;
            foreach (ColumnHeader col in OrderedHeaders)
            {
                sum += col.Width;
                if (sum > xoffset)
                {
                    result = col;
                    break;
                }
            }

            return result;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            bool headerWasClicked;
            _clickedColumn = GetColumnAt(Control.MousePosition, out headerWasClicked);

            // If the mouse position is in the header bar, cancel the display
            // of the regular context menu and display the column header context menu instead.
            if (headerWasClicked)
            {
                e.Cancel = true;
                ShowMenuForColumnHeader();
            }
            else
            {
                // Update the items in the default context menu and allow it to be displayed.
                UpdateMenuItems();
            }
        }

        // This should get called with the only child window of the listview,
        // which should be the header bar.
        private bool EnumWindowCallBack(IntPtr hwnd, IntPtr lParam)
        {
            // Determine the rectangle of the header bar and save it in a member variable.
            NativeMethods.RECT rct;

            if (!NativeMethods.GetWindowRect(hwnd, out rct))
            {
                _headerRect = Rectangle.Empty;
            }
            else
            {
                _headerRect = new Rectangle(rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top);
            }
            return false; // Stop the enum
        }

        // Keep certain commands enabled while the menu is closed
        private void editToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            copyTextMenuItem.Enabled = true;
            copyColsMenuItem.Enabled = true;
        }

        private void copyColsMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            string[] fields = new string[TheListView.Columns.Count];

            foreach (int index in TheListView.SelectedIndices)
            {
                ListViewItem item = TheListView.Items[index];
                foreach (ColumnHeader hdr in TheListView.Columns)
                {
                    if (hdr == headerText)
                    {
                        // This is a special case because the text 
                        // message might be truncated in the ListView.
                        fields[hdr.DisplayIndex] = Rows[index].GetFullIndentedText();
                    }
                    else
                    {
                        fields[hdr.DisplayIndex] = item.SubItems[hdr.Index].Text;
                    }
                }

                foreach (string str in fields)
                {
                    builder.Append(str);
                    builder.Append(", ");
                }

                // Replace the last ", " with a newline.
                builder.Length = builder.Length - 2;
                builder.Append("\n");
            }

            // Remove last newline.
            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 1;
                Clipboard.SetText(builder.ToString());
            }
        }

        private void mnuBookmarkSelected_Click(object sender, EventArgs e)
        {
            if (_clickedColumn == headerLevel)
            {
                BookmarkSelectedTraceLevels();
            }
            else if (_clickedColumn == headerLine)
            {

            }
            else if (_clickedColumn == headerLogger)
            {
                BookmarkSelectedLoggers();
            }
            else if (_clickedColumn == headerMethod)
            {

            }
            else if (_clickedColumn == headerSession)
            {

            }
            else if (_clickedColumn == headerText)
            {

            }
            else if (_clickedColumn == headerThreadId)
            {
                BookmarkSelectedThreads();
            }
            else if (_clickedColumn == headerThreadName)
            {
                BookmarkSelectedThreadsNames();
            }
            else if (_clickedColumn == headerTime)
            {

            }
        }

        private void BookmarkSelectedThreadsNames()
        {
            // First make a list of the selected threads.
            List<ThreadName> threads = new List<ThreadName>();
            foreach (int index in TheListView.SelectedIndices)
            {
                threads.Add(Rows[index].Rec.ThreadName);
            }

            // Now set the IsBookmarked flag for every line of every record whose
            // thread is in the list we just made.
            bool bookmarked = false;
            foreach (Record rec in _records)
            {
                if (threads.Contains(rec.ThreadName))
                {
                    for (int i = 0; i < rec.IsBookmarked.Length; ++i)
                    {
                        rec.IsBookmarked[i] = true;
                        bookmarked = true;
                    }
                }
            }

            if (bookmarked)
            {
                bookmarkPrevCmd.Enabled = true;
                bookmarkNextCmd.Enabled = true;
                bookmarkClearCmd.Enabled = true;
            }

            // We're not hiding or showing anything, so don't call RebuildRows.
            // Just make sure any visible rows get their images redrawn.
            InvalidateTheListView();
        }

        private void BookmarkSelectedThreads()
        {
            // First make a list of the selected threads.
            List<ThreadObject> threads = new List<ThreadObject>();
            foreach (int index in TheListView.SelectedIndices)
            {
                threads.Add(Rows[index].Rec.Thread);
            }

            // Now set the IsBookmarked flag for every line of every record whose
            // thread is in the list we just made.
            bool bookmarked = false;
            foreach (Record rec in _records)
            {
                if (threads.Contains(rec.Thread))
                {
                    for (int i = 0; i < rec.IsBookmarked.Length; ++i)
                    {
                        rec.IsBookmarked[i] = true;
                        bookmarked = true;
                    }
                }
            }

            if (bookmarked)
            {
                bookmarkPrevCmd.Enabled = true;
                bookmarkNextCmd.Enabled = true;
                bookmarkClearCmd.Enabled = true;
            }

            // We're not hiding or showing anything, so don't call RebuildRows.
            // Just make sure any visible rows get their images redrawn.
            InvalidateTheListView();
        }

        private void BookmarkSelectedLoggers()
        {
            // First make a list of the selected loggers.
            List<LoggerObject> loggers = new List<LoggerObject>();
            foreach (int index in TheListView.SelectedIndices)
            {
                loggers.Add(Rows[index].Rec.Logger);
            }

            // Now set the IsBookmarked flag for every line of every record whose
            // logger is in the list we just made.
            bool bookmarked = false;
            foreach (Record rec in _records)
            {
                if (loggers.Contains(rec.Logger))
                {
                    for (int i = 0; i < rec.IsBookmarked.Length; ++i)
                    {
                        rec.IsBookmarked[i] = true;
                        bookmarked = true;
                    }
                }
            }

            if (bookmarked)
            {
                bookmarkPrevCmd.Enabled = true;
                bookmarkNextCmd.Enabled = true;
                bookmarkClearCmd.Enabled = true;
            }

            // We're not hiding or showing anything, so don't call RebuildRows.
            // Just make sure any visible rows get their images redrawn.
            InvalidateTheListView();
        }

        private void BookmarkSelectedTraceLevels()
        {
            // First make a list of the selected trace levels.
            List<TraceLevelObject> levels = new List<TraceLevelObject>();
            foreach (int index in TheListView.SelectedIndices)
            {
                levels.Add(Rows[index].Rec.TLevel);
            }

            // Now set the IsBookmarked flag for every line of every record whose
            // trace level is in the bitmask we just made.
            bool bookmarked = false;
            foreach (Record rec in _records)
            {
                if (levels.Contains(rec.TLevel))
                {
                    for (int i = 0; i < rec.IsBookmarked.Length; ++i)
                    {
                        rec.IsBookmarked[i] = true;
                        bookmarked = true;
                    }
                }
            }

            if (bookmarked)
            {
                bookmarkPrevCmd.Enabled = true;
                bookmarkNextCmd.Enabled = true;
                bookmarkClearCmd.Enabled = true;
            }

            // We're not hiding or showing anything, so don't call RebuildRows.
            // Just make sure any visible rows get their images redrawn.
            InvalidateTheListView();
        }

        private void ExecuteRefresh(object sender, EventArgs e)
        {
            Debug.Print("Refresh");
            ReportCurrentRow();
            StartReading(null, null); // Null means refresh the current file.
        }

        private void ReportCurrentRow()
        {
            Debug.Print("Selected count = " + TheListView.SelectedIndices.Count);
            Debug.Print("FocusedItem index = " + (TheListView.FocusedItem == null ? "null" : TheListView.FocusedItem.Index.ToString()));
            Debug.Print("CurrentRow = " + (CurrentRow == null ? "null" : CurrentRow.Index.ToString()));
        }

        private void viewTextWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Row row = Rows[TheListView.SelectedIndices[0]];
            row.ShowFullText();
        }

        private void ExecuteOpenFilterDialog(object sender, EventArgs e)
        {
            FilterDialog dialog = new FilterDialog();
            dialog.ShowDialog(this);
        }

        private void ExecuteOptions(object sender, EventArgs e)
        {
            OptionsDialog dlg = new OptionsDialog();
            dlg.ShowDialog(this);
        }

        private void setZeroTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Row row = Rows[TheListView.SelectedIndices[0]];
            ZeroTime = row.Rec.Time;
            Settings.Default.RelativeTime = true;
        }

        #region Column header context menu
        // Called when a column header is left-clicked
        private void TheListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            _clickedColumn = TheListView.Columns[e.Column];
            ShowMenuForColumnHeader();
        }

        // Shows the context menu for the specified column header.
        private void ShowMenuForColumnHeader()
        {
            //First, enable the appropriate menu items for the specified header.
            MaybeEnableRemoveFromFilter(_clickedColumn);
            MaybeEnableFilter(_clickedColumn);
            MaybeEnableOptions(_clickedColumn);

            columnContextMenu.Show(Control.MousePosition);
        }

        private void MaybeEnableOptions(ColumnHeader header)
        {
            if (_reader == null)
            {
                this.colMenuOptionsItem.Enabled = false;
            }
            else if (header == this.headerLine ||
                     header == this.headerTime ||
                     header == this.headerThreadId ||
                     header == this.headerThreadName ||
                     header == this.headerText) //
            {
                this.colMenuOptionsItem.Enabled = true;
            }
            else
            {
                this.colMenuOptionsItem.Enabled = false;
            }
        }

        private void MaybeEnableFilter(ColumnHeader header)
        {
            if (_reader == null)
            {
                this.colMenuFilterItem.Enabled = false;
            }
            else if (header == this.headerLevel ||
                     header == this.headerLogger ||
                     header == this.headerSession ||
                     header == this.headerMethod ||
                     header == this.headerThreadName ||
                     header == this.headerText ||
                     header == this.headerThreadId) //
            {
                this.colMenuFilterItem.Enabled = true;
            }
            else
            {
                this.colMenuFilterItem.Enabled = false;
            }
        }

        private void MaybeEnableRemoveFromFilter(ColumnHeader header)
        {
            colMenuRemoveItem.Enabled = false;

            if (header == this.headerThreadId && !ThreadObjects.AllVisible)
            {
                colMenuRemoveItem.Enabled = true;
            }

            if (header == this.headerThreadName && !ThreadNames.AllVisible)
            {
                colMenuRemoveItem.Enabled = true;
            }

            if (header == this.headerLogger && !LoggerObjects.AllVisible)
            {
                colMenuRemoveItem.Enabled = true;
            }

            if (header == this.headerMethod && !MethodObjects.AllVisible)
            {
                colMenuRemoveItem.Enabled = true;
            }

            if (header == this.headerSession && !SessionObjects.AllVisible)
            {
                colMenuRemoveItem.Enabled = true;
            }

            if (header == this.headerLevel && !TraceLevelObjects.AllVisible)
            {
                colMenuRemoveItem.Enabled = true;
            }

            if (header == this.headerText && FilterDialog.TextFilterOn)
            {
                colMenuRemoveItem.Enabled = true;
            }
        }

        private void colMenuFilterItem_Click(object sender, EventArgs e)
        {
            FilterDialog dialog = new FilterDialog(_clickedColumn);
            dialog.ShowDialog(this);
        }

        private void colMenuRemoveItem_Click(object sender, EventArgs e)
        {
            ColumnHeader header = _clickedColumn;

            if (header == headerLevel)
            {
                TraceLevelObjects.ShowAllLevels();
                RebuildAllRows();
            }
            else if (header == headerThreadId)
            {
                ThreadObjects.ShowAllThreads();
                RebuildAllRows();
            }
            else if (header == headerSession)
            {
                SessionObjects.ShowAllSessions();
                RebuildAllRows();
            }
            else if (header == headerThreadName)
            {
                ThreadNames.ShowAllThreads();
                RebuildAllRows();
            }
            else if (header == headerLogger)
            {
                LoggerObjects.ShowAllLoggers();
                RebuildAllRows();
            }
            else if (header == headerMethod)
            {
                MethodObjects.ShowAllMethods();
                RebuildAllRows();
            }
            else if (header == headerText)
            {
                FilterDialog.TextFilterDisable();
                RebuildAllRows();
            }
        }

        private void colMenuHideItem_Click(object sender, EventArgs e)
        {
            TheListView.Columns.Remove(_clickedColumn);

            // Any cached items are now invalid due to columns change.
            TheListView.ClearItemCache();
            if (TheListView.VirtualListSize > 0)
            {
                TheListView.RedrawItems(TheListView.TopItem.Index, FindLastVisibleItem(), true);
            }
        }

        private void colMenuOptionsItem_Click(object sender, EventArgs e)
        {
            OptionsDialog dialog = new OptionsDialog();

            if (_clickedColumn == MainForm.TheMainForm.headerTime)
            {
                dialog.SelectTab(OptionsTab.Time);
            }
            else if (_clickedColumn == MainForm.TheMainForm.headerLine)
            {
                dialog.SelectTab(OptionsTab.Misc);
            }
            else if (_clickedColumn == MainForm.TheMainForm.headerText)
            {
                dialog.SelectTab(OptionsTab.Text);
            }
            else if (_clickedColumn == MainForm.TheMainForm.headerThreadId || _clickedColumn == MainForm.TheMainForm.headerThreadName)
            {
                dialog.SelectTab(OptionsTab.Threads);
            }

            dialog.ShowDialog(this);
        }

        #endregion Column header context menu

        #region Recently Viewed/Created
        // Add the file to the list of recently viewed files.
        // Applies to localhost files only.
        private void AddFileToRecentlyViewed(string filename)
        {
            DateTime now = DateTime.Now;
            DateTime dummyTime = now;
            var viewedFile = new ViewedPath { Path = filename, ViewTime = now };

            // In the past, duplicate file paths that differ only by case have gotten into the list.
            // Use a HashSet to filter out the duplicates.

            var dupFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var keepers = new List<ViewedPath>();

            dupFilter.Add(filename);
            keepers.Add(viewedFile);

            if (Settings.Default.RecentFiles != null)
            {
                foreach (ViewedPath oldFileNameTime in Settings.Default.RecentFiles)
                {
                    if (dupFilter.Add(oldFileNameTime.Path))
                    {
                        keepers.Add(oldFileNameTime);
                    }
                }
            }

            Settings.Default.RecentFiles = keepers.Take(200).ToArray();

            // now update the folders and save.
            AddToRecentFolders(Path.GetDirectoryName(filename), now);
            Settings.Default.Save();
        }

        // Add the folder to the list of recent folders.
        private void AddToRecentFolders(string folder, DateTime now)
        {
            DateTime dummyTime = now;
            var viewedFolder = new ViewedPath { Path = folder, ViewTime = now };

            // In the past, duplicate folders that differ only by case have gotten into the list.
            // Use a HashSet to filter out the duplicates.

            var dupFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var keepers = new List<ViewedPath>();

            dupFilter.Add(folder);
            keepers.Add(viewedFolder);

            if (Settings.Default.RecentFolders != null)
            {
                foreach (ViewedPath oldFolderNameTime in Settings.Default.RecentFolders)
                {

                    if (dupFilter.Add(oldFolderNameTime.Path))
                    {
                        keepers.Add(oldFolderNameTime);
                    }
                }
            }

            Settings.Default.RecentFolders = keepers.Take(200).ToArray();
        }

        #endregion

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog(this);
        }

        private void licenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            License dlg = new License();
            dlg.ShowDialog(this);
        }

        private void TheListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TheListView.FocusedItem == null)
            {
                //Debug.Print("SelectedIndexChanged, focused = null ");
                //nextAnyThreadBtn.Enabled = false;
                //prevAnyThreadBtn.Enabled = false;
            }
            else
            {
                //Debug.Print("SelectedIndexChanged, focused index = " + TheListView.FocusedItem.Index);
            }

            crumbBar1.SetCurrentRow(CurrentRow, _records);
            bookmarkToggleCmd.Enabled = TheListView.FocusedItem != null;
            UpdateThreadButtons();
            UpdateTimeButtons();

            // When the main form is not active, we do our own highlighting of selected items.
            if (this != Form.ActiveForm) TheListView.SetItemCacheColors(false);
        }

        private void TheListView_VirtualItemsSelectionRangeChanged(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e)
        {
            if (TheListView.FocusedItem == null)
            {
                //Debug.Print("VirtualItemsSelectionRange, focused = null ");
            }
            else
            {
                //Debug.Print("VirtualItemsSelectionRange, focused index = " + TheListView.FocusedItem.Index);
            }

            // When the main form is not active, we do our own highlighting of selected items.
            if (this != Form.ActiveForm) TheListView.SetItemCacheColors(false);
        }

        private void UpdateThreadButtons()
        {
            if (TheListView.SelectedIndices.Count == 1)
            {
                int ndx = TheListView.SelectedIndices[0];
                prevAnyThreadBtn.Enabled = ndx > 0;
                prevSameThreadBtn.Enabled = ndx > 0;
                nextAnyThreadBtn.Enabled = ndx != NumRows - 1;
                nextSameThreadBtn.Enabled = ndx != NumRows - 1;
            }
            else
            {
                prevAnyThreadBtn.Enabled = false;
                prevSameThreadBtn.Enabled = false;
                nextAnyThreadBtn.Enabled = false;
                nextSameThreadBtn.Enabled = false;
            }

            if (Settings.Default.SearchThreadsByName)
            {
                prevAnyThreadBtn.ToolTipText = "Find previous output from different thread name.";
                prevSameThreadBtn.ToolTipText = "Find previous output from same thread name.";
                nextAnyThreadBtn.ToolTipText = "Find next output from different thread name.";
                nextSameThreadBtn.ToolTipText = "Find next output from same thread name.";
            }
            else
            {
                prevAnyThreadBtn.ToolTipText = "Find previous output from different thread ID.";
                prevSameThreadBtn.ToolTipText = "Find previous output from same thread ID.";
                nextAnyThreadBtn.ToolTipText = "Find next output from different thread ID.";
                nextSameThreadBtn.ToolTipText = "Find next output from same thread ID.";
            }
        }

        private void UpdateTimeButtons()
        {
            if (TheListView.SelectedIndices.Count == 1)
            {
                int ndx = TheListView.SelectedIndices[0];
                prevTimeButton.Enabled = ndx > 0;
                nextTimeButton.Enabled = ndx != NumRows - 1;
            }
            else
            {
                prevTimeButton.Enabled = false;
                nextTimeButton.Enabled = false;
            }
        }

        void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SearchThreadsByName":
                    UpdateThreadButtons();
                    break;
                case "DuplicateTimes":
                    dupTimeButton.Checked = Settings.Default.DuplicateTimes;
                    InvalidateTheListView();
                    break;
                case "RelativeTime":
                    relativeTimeButton.Checked = Settings.Default.RelativeTime;
                    InvalidateTheListView();
                    break;
                case "ColoringEnabled":
                    enableColors.Checked = Settings.Default.ColoringEnabled;
                    InvalidateTheListView();
                    break;
                case "AutoUpdate":
                    if (autoUpdate.Enabled && Settings.Default.AutoUpdate)
                    {
                        StartFileWatcher();
                    }
                    else
                    {
                        StopFileWatcher();
                    }
                    break;
            }
        }

        private void relativeTimeButton_Click(object sender, EventArgs e)
        {
            Settings.Default.RelativeTime = !Settings.Default.RelativeTime;
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            //Debug.Print("MainForm_Activated, " + TheListView.SelectedIndices.Count + " selected");
            TheListView.SetItemCacheColors(true);
        }

        private void closeAllWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Form> toClose = new List<Form>();

            foreach (Form form in Application.OpenForms)
            {
                if (form != this) toClose.Add(form);
            }

            foreach (Form form in toClose) form.Close();
        }

        private void viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            closeAllWindowsToolStripMenuItem.Enabled = Application.OpenForms.Count > 1;
            showServerPickerMenuItem.Checked = Settings.Default.ShowServers;
            showTracerXLogsMenuItem.Checked = Settings.Default.ShowTracerxFiles;
        }

        private void TheListView_Leave(object sender, EventArgs e)
        {

            if (this.TopLevel && this._FileState == FileState.Loaded)
            {
                // The crumbBar gets the focus whenever the user clicks one of its links (including
                // disabled links).  When this happens and the viewer is a TopLevel form, the selected 
                // items in the TheListView are no longer highlighted.  Return the focus to TheListView 
                // so the selected rows remain highlighted.  Doing this when the viewer is a
                // UserControl causes the app to freeze.
                this.ActiveControl = TheListView;
            }
        }

        private void coloringCmd_Execute(object sender, EventArgs e)
        {
            ColorRulesDialog.ShowModal();
        }

        private void enableColors_Execute(object sender, EventArgs e)
        {
            Settings.Default.ColoringEnabled = !Settings.Default.ColoringEnabled;
        }

        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportCSVForm.ShowModal();
        }

        public static void ShowMessageBox(string text)
        {
            if (TheMainForm.InvokeRequired)
            {
                TheMainForm.Invoke(new Action<string>(ShowMessageBox), text);
                return;
            }

            MessageBox.Show(TheMainForm, text, "TracerX-Viewer");
        }

        public static DialogResult ShowMessageBoxBtns(string text, MessageBoxButtons buttons)
        {
            if (TheMainForm.InvokeRequired)
            {
                object result = TheMainForm.Invoke(new Func<string, MessageBoxButtons, DialogResult>(ShowMessageBoxBtns), text, buttons);
                return (DialogResult)result;
            }

            return MessageBox.Show(TheMainForm, text, "TracerX-Viewer", buttons);
        }

        private void autoUpdate_Click(object sender, EventArgs e)
        {
            autoUpdate.Checked = !autoUpdate.Checked;
            Settings.Default.AutoUpdate = autoUpdate.Checked;
        }

        // Show the "Uncolor Selected <item>s menu item only if at least one of the
        // selected rows is colored due to a coloring rule (other than Custom rules).
        private void UpdateUncolorMenu()
        {
            uncolorSelectedMenu.Visible = false; // for now

            foreach (int ndx in TheListView.SelectedIndices)
            {
                var record = Rows[ndx].Rec;

                switch (ColorUtil.RowColorDriver)
                {
                    case ColorDriver.Custom:
                        break;
                    case ColorDriver.Loggers:
                        if (record.Logger.RowColors != null)
                        {
                            uncolorSelectedMenu.Visible = true;
                            uncolorSelectedMenu.Text = "Uncolor Selected Loggers";
                        }
                        break;
                    case ColorDriver.Methods:
                        if (record.MethodName.RowColors != null)
                        {
                            uncolorSelectedMenu.Visible = true;
                            uncolorSelectedMenu.Text = "Uncolor Selected Methods";
                        }
                        else if (ColorRulesDialog.ColorCalledMethods)
                        {
                            for (Record caller = record.Caller; caller != null; caller = caller.Caller)
                            {
                                if (caller.MethodName.RowColors != null)
                                {
                                    uncolorSelectedMenu.Visible = true;
                                    uncolorSelectedMenu.Text = "Uncolor Selected Methods";
                                    break;
                                }
                            }
                        }
                        break;
                    case ColorDriver.TraceLevels:
                        if (record.TLevel.RowColors != null)
                        {
                            uncolorSelectedMenu.Visible = true;
                            uncolorSelectedMenu.Text = "Uncolor Selected Trace Levels";
                        }
                        break;
                    case ColorDriver.ThreadIDs:
                        if (record.Thread.RowColors != null)
                        {
                            uncolorSelectedMenu.Visible = true;
                            uncolorSelectedMenu.Text = "Uncolor Selected Thread IDs";
                        }
                        break;
                    case ColorDriver.Sessions:
                        if (record.Session.RowColors != null)
                        {
                            uncolorSelectedMenu.Visible = true;
                            uncolorSelectedMenu.Text = "Uncolor Selected Sessions";
                        }
                        break;
                    case ColorDriver.ThreadNames:
                        if (record.ThreadName.RowColors != null)
                        {
                            uncolorSelectedMenu.Visible = true;
                            uncolorSelectedMenu.Text = "Uncolor Selected Thread Names";
                        }
                        break;
                }

                if (uncolorSelectedMenu.Visible) break;
            }
        }

        private void uncolorSelectedMenu_Click(object sender, EventArgs e)
        {
            foreach (int ndx in TheListView.SelectedIndices)
            {
                var record = Rows[ndx].Rec;

                switch (ColorUtil.RowColorDriver)
                {
                    case ColorDriver.Custom:
                        // Should be impossible.
                        break;
                    case ColorDriver.Loggers:
                        record.Logger.RowColors = null;
                        break;
                    case ColorDriver.Methods:
                        record.MethodName.RowColors = null;

                        if (ColorRulesDialog.ColorCalledMethods)
                        {
                            for (Record caller = record.Caller; caller != null; caller = caller.Caller)
                            {
                                caller.MethodName.RowColors = null;
                            }
                        }
                        break;
                    case ColorDriver.TraceLevels:
                        record.TLevel.RowColors = null;
                        break;
                    case ColorDriver.ThreadIDs:
                        record.Thread.RowColors = null;
                        break;
                    case ColorDriver.Sessions:
                        record.Session.RowColors = null;
                        break;
                    case ColorDriver.ThreadNames:
                        record.ThreadName.RowColors = null;
                        break;
                }
            }

            // Cause visible rows to be recreated.
            InvalidateTheListView();

        }

        private void ColorStuff(IEnumerable<IFilterable> itemsToColor, ColorDriver driver)
        {
            var availableColors = ColorUtil.GetUnusedColors(driver).ToList();

            foreach (var filterable in itemsToColor.Distinct())
            {
                if (filterable.RowColors == null)
                {
                    if (availableColors.Any())
                    {
                        filterable.RowColors = availableColors.First();
                        availableColors.Remove(filterable.RowColors);
                    }
                    else
                    {
                        ShowMessageBox("Sorry, the color palette doesn't contain enough colors for all selected items.");
                        break;
                    }
                }
            }

            Settings.Default.ColoringEnabled = true;
            ColorUtil.RowColorDriver = driver;

            // Cause visible rows to be recreated.
            InvalidateTheListView();
        }

        private void mnuColorSelected_Click(object sender, EventArgs e)
        {
            var selectedItems = new List<IFilterable>();

            if (_clickedColumn == headerLevel)
            {
                foreach (int index in TheListView.SelectedIndices)
                {
                    selectedItems.Add(Rows[index].Rec.TLevel);
                }

                ColorStuff(selectedItems, ColorDriver.TraceLevels);
            }
            else if (_clickedColumn == headerLine)
            {

            }
            else if (_clickedColumn == headerLogger)
            {
                foreach (int index in TheListView.SelectedIndices)
                {
                    selectedItems.Add(Rows[index].Rec.Logger);
                }

                ColorStuff(selectedItems, ColorDriver.Loggers);
            }
            else if (_clickedColumn == headerMethod)
            {
                foreach (int index in TheListView.SelectedIndices)
                {
                    selectedItems.Add(Rows[index].Rec.MethodName);
                }

                ColorStuff(selectedItems, ColorDriver.Methods);
            }
            else if (_clickedColumn == headerSession)
            {
                foreach (int index in TheListView.SelectedIndices)
                {
                    selectedItems.Add(Rows[index].Rec.Session);
                }

                ColorStuff(selectedItems, ColorDriver.Sessions);
            }
            else if (_clickedColumn == headerText)
            {

            }
            else if (_clickedColumn == headerThreadId)
            {
                foreach (int index in TheListView.SelectedIndices)
                {
                    selectedItems.Add(Rows[index].Rec.Thread);
                }

                ColorStuff(selectedItems, ColorDriver.ThreadIDs);
            }
            else if (_clickedColumn == headerThreadName)
            {
                foreach (int index in TheListView.SelectedIndices)
                {
                    selectedItems.Add(Rows[index].Rec.ThreadName);
                }

                ColorStuff(selectedItems, ColorDriver.ThreadNames);
            }
            else if (_clickedColumn == headerTime)
            {

            }
        }

        // Selects the row with the specified roundNdx, positioning it in the middle
        // of the screen if it's off the current page.
        private void SelectFoundIndex(int foundNdx)
        {
            int topNdx = TheListView.TopItem.Index;

            if (foundNdx < topNdx)
            {
                // The found row is above the top of the page, so when we scroll to it, position it
                // in the middle of the page.
                int itemsVisible = FindLastVisibleItem() - topNdx + 1;
                int newTop = foundNdx - itemsVisible / 2;
                TheListView.TopItem = TheListView.Items[Math.Max(0, newTop)];
            }
            else
            {
                int lastNdx = FindLastVisibleItem();

                if (foundNdx > lastNdx)
                {
                    // The found row is below the bottom the page, so when we scroll to it, position it
                    // in the middle of the page.
                    int itemsVisible = lastNdx - TheListView.TopItem.Index + 1;
                    int newTop = Math.Min(NumRows - itemsVisible, foundNdx - itemsVisible / 2);

                    // For some reason, setting the TopItem once doesn't work here.  Setting
                    // it three times usually does, so try up to four.
                    for (int i = 0; i < 4; ++i)
                    {
                        if (TheListView.TopItem.Index == newTop) break;
                        TheListView.TopItem = (TheListView.Items[newTop]);
                    }
                }
            }

            SelectRowIndex(foundNdx);
        }

        // Handles finde prev same thread and prev different thread.
        private void prevThreadBtn_Click(object sender, EventArgs e)
        {
            statusMsg.Visible = false;

            if (TheListView.SelectedIndices.Count > 0)
            {
                int startNdx = TheListView.SelectedIndices[0];
                var startRec = Rows[startNdx].Rec;
                int foundNdx;

                // First find a thread that's different from the starting thread.
                for (foundNdx = startNdx - 1; foundNdx > 0 && Rows[foundNdx].Rec.SameThreadAs(startRec); --foundNdx) ;

                if (Rows[foundNdx].Rec.SameThreadAs(startRec))
                {
                    ShowStatus("Search reached first visible row.", true);
                    return;
                }

                if (sender == prevSameThreadBtn)
                {

                    // Find the starting thread again.
                    for (; foundNdx > 0 && !Rows[foundNdx].Rec.SameThreadAs(startRec); --foundNdx) ;

                    if (!Rows[foundNdx].Rec.SameThreadAs(startRec))
                    {
                        ShowStatus("Search reached first visible row.", true);
                        return;
                    }
                }

                SelectFoundIndex(foundNdx);
            }
        }

        // Handles find next same thread and next different thread.
        private void nextThreadBtn_Click(object sender, EventArgs e)
        {
            statusMsg.Visible = false;

            if (TheListView.SelectedIndices.Count > 0)
            {
                int startNdx = TheListView.SelectedIndices[0];

                if (startNdx < NumRows - 1)
                {
                    var startRec = Rows[startNdx].Rec;
                    int foundNdx;

                    // First find a different thread.
                    for (foundNdx = startNdx + 1; foundNdx < NumRows - 1 && Rows[foundNdx].Rec.SameThreadAs(startRec); ++foundNdx) ;

                    if (Rows[foundNdx].Rec.SameThreadAs(startRec))
                    {
                        ShowStatus("Search reached last visible row.", true);
                        return;
                    }

                    if (sender == nextSameThreadBtn)
                    {
                        // Find the starting thread again.
                        for (; foundNdx < NumRows - 1 && !Rows[foundNdx].Rec.SameThreadAs(startRec); ++foundNdx) ;

                        if (!Rows[foundNdx].Rec.SameThreadAs(startRec))
                        {
                            ShowStatus("Search reached last visible row.", true);
                            return;
                        }
                    }

                    SelectFoundIndex(foundNdx);
                    Debug.Print("Actual top = " + TheListView.TopItem.Index);
                }
            }
        }

        // Shows a message in the status bar.
        // Any operation that could show a message (e.g. find)
        // should first clear the message.
        private void ShowStatus(string text, bool error)
        {
            statusMsg.Text = text;
            statusMsg.Visible = true;

            if (statusMsg.Bounds.Y > 20)
            {
                // This means the statusMsg control isn't visible because
                // there isn't enough room to display it and the filenameLabel.
                // Hide the filenameLabel so the statusMessage will appear.
                filenameLabel.Visible = false;
            }

            if (error)
            {
                statusMsg.BackColor = Color.OrangeRed;
                System.Media.SystemSounds.Beep.Play();
            }
            else
            {
                statusMsg.BackColor = Color.SkyBlue;// SystemColors.Control;
            }
        }

        private void statusMsg_VisibleChanged(object sender, EventArgs e)
        {
            if (!statusMsg.Visible)
            {
                // Since the statusMsg is not visible, there is no need to hide filenameLabel.
                filenameLabel.Visible = true;
            }
        }

        private void boldBtn_Click(object sender, EventArgs e)
        {
            Settings.Default.Bold = !Settings.Default.Bold;
            ApplyBoldSetting();
        }

        private void ApplyBoldSetting()
        {
            if (Settings.Default.Bold)
            {
                TheListView.Font = new Font(TheListView.Font, FontStyle.Bold);
                boldBtn.Checked = true;
            }
            else
            {
                int style = (int)TheListView.Font.Style & ~(int)FontStyle.Bold;

                TheListView.Font = new Font(TheListView.Font, (FontStyle)style);
                boldBtn.Checked = false;
            }
        }

        private void dupTimeButton_Click(object sender, EventArgs e)
        {
            Settings.Default.DuplicateTimes = !Settings.Default.DuplicateTimes;
        }

        private void timeUnitCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.TimeUnits = timeUnitCombo.SelectedIndex;
            prevTimeButton.ToolTipText = "Previous " + timeUnitCombo.SelectedItem.ToString().ToLower();
            nextTimeButton.ToolTipText = "Next " + timeUnitCombo.SelectedItem.ToString().ToLower();
        }

        private void timeUnitCombo_Click(object sender, EventArgs e)
        {
            timeUnitCombo.DroppedDown = true;
        }

        private void prevTimeButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            statusMsg.Visible = false;

            try
            {
                DateTime targetTime = GetTargetTime(0);
                int curRow;

                for (curRow = FocusedRow.Index; curRow >= 0 && Rows[curRow].Rec.Time >= targetTime; --curRow) ;

                if (curRow < 0)
                {
                    // not found
                    ShowStatus("Search reached first visible row.", true);
                }
                else
                {
                    // Select found row.
                    SelectFoundIndex(curRow);
                }
            }
            catch { }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void nextTimeButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            statusMsg.Visible = false;

            try
            {
                DateTime targetTime = GetTargetTime(1);
                int curRow;

                for (curRow = FocusedRow.Index; curRow < NumRows && Rows[curRow].Rec.Time < targetTime; ++curRow) ;

                if (curRow == NumRows)
                {
                    // not found
                    ShowStatus("Search reached last visible row.", true);
                }
                else
                {
                    // Select found row.
                    SelectFoundIndex(curRow);
                }
            }
            catch { }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private DateTime GetTargetTime(int increment)
        {
            DateTime startTime = _FocusedRec.Time;
            DateTime result = DateTime.MinValue;

            if (Settings.Default.RelativeTime)
            {
                TimeSpan startDiff = startTime - ZeroTime;
                TimeSpan targetDiff = startDiff;

                if (startTime < ZeroTime)
                {
                    // This means we're searching through negative diffs.
                    --increment;
                }

                switch (timeUnitCombo.SelectedIndex)
                {
                    case 0:
                        // second
                        targetDiff = new TimeSpan(startDiff.Days, startDiff.Hours, startDiff.Minutes, startDiff.Seconds).Add(TimeSpan.FromSeconds(increment));
                        break;
                    case 1:
                        // minute
                        targetDiff = new TimeSpan(startDiff.Days, startDiff.Hours, startDiff.Minutes, 0).Add(TimeSpan.FromMinutes(increment));
                        break;
                    case 2:
                        // hour
                        targetDiff = new TimeSpan(startDiff.Days, startDiff.Hours, 0, 0).Add(TimeSpan.FromHours(increment));
                        break;
                    case 3:
                        // day
                        targetDiff = new TimeSpan(startDiff.Days, 0, 0, 0).Add(TimeSpan.FromDays(increment));
                        break;
                }

                result = ZeroTime + targetDiff;
            }
            else
            {
                startTime = startTime.ToLocalTime();

                switch (timeUnitCombo.SelectedIndex)
                {
                    case 0:
                        // second
                        result = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, startTime.Second).AddSeconds(increment);
                        break;
                    case 1:
                        // minute
                        result = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, 0).AddMinutes(increment);
                        break;
                    case 2:
                        // hour
                        result = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, 0, 0).AddHours(increment);
                        break;
                    case 3:
                        // day
                        result = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0).AddDays(increment);
                        break;
                }

                result = result.ToUniversalTime();
            }

            return result;
        }

        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeploymentDescription.StartNewViewer();
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            switch (_FileState)
            {
                case FileState.NoFile:
                    this.Close();
                    break;
                case FileState.Loaded:
                    CloseFile();
                    break;
                case FileState.Loading:
                    break;
            }
        }

        private void clearColumnColors_Execute(object sender, EventArgs e)
        {
            ColorUtil.ClearSubItemColors();
            InvalidateTheListView();
        }

        private void showTracerXLogsMenuItem_Click(object sender, EventArgs e)
        {
            showTracerXLogsMenuItem.Checked = !showTracerXLogsMenuItem.Checked;
            Settings.Default.ShowTracerxFiles = showTracerXLogsMenuItem.Checked; // Handled by PropertyChanged handler in NewStartPage.
        }

        private void showServerPickerMenuItem_Click(object sender, EventArgs e)
        {
            showServerPickerMenuItem.Checked = !showServerPickerMenuItem.Checked;
            Settings.Default.ShowServers = showServerPickerMenuItem.Checked; // Handled by PropertyChanged handler in NewStartPage.
            //theStartPage.ShowServerPicker(showServerPickerMenuItem.Checked);
        }

        private void commandLineMenuItem_Click(object sender, EventArgs e)
        {
            AppArgs.ShowHelp();
        }

        private void filenameLabel_VisibleChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("filenameLabel.Visible = {0}", filenameLabel.Visible);
        }

        private void relatedFilesFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new RelatedForm();
            dlg.ShowDialog(this);
        }
    }
}