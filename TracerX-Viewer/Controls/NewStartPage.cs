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
using TracerX.ExtensionMethods;
using System.ServiceModel.Security;
using System.Security.Authentication;

namespace TracerX
{
    internal partial class NewStartPage : UserControl
    {
        public NewStartPage()
        {
            InitializeComponent();
            DoubleBuffered = true;

            if (!DesignMode)
            {
                pnlVersion.Hide();

                // TODO: Figure out how to check for new version on GitHub and uncomment this.
                //VersionChecker.NewVersionFound += (sender, e) => pnlVersion.Show();
                //VersionChecker.CheckForNewVersion();

                _remoteServers = RemoteServer.CreateListFromSettings();

                try
                {
                    // Try to make the SplitContainer double buffered to reduce flicker.
                    var propInfo = typeof(SplitContainer).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    propInfo.SetValue(splitContainer1, true, null);
                }
                catch (Exception ex)
                {
                    // Not that important.  Probably never happens.
                    Debug.Write(ex);
                }

                Settings.Default.PropertyChanged += SettingsPropertyChanged;

                _refreshTimer.Tick +=new EventHandler(_refreshTimer_Tick);
                _refreshTimer.Interval = 2000;
            }
        }

        [Browsable(true)]
        public Color ColumnHeaderBackColor
        {
            get { return filesGrid.ColumnHeaderBackColor; }
            
            set 
            {
                filesGrid.ColumnHeaderBackColor = value;
                foldersGrid.ColumnHeaderBackColor = value;
            }
        }

        /// <summary>
        /// Path of file or folder that the user clicked to raise FolderClicked or FileClicked.
        /// </summary>
        public string ClickedPath
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the currently selected RemoteServer from the combo box (comboServers),
        /// or null if LocalHost (or nothing) is selected.
        /// </summary>
        public RemoteServer CurRemoteServer
        {
            get
            {
                if (_curServer == _localHost)
                {
                    return null;
                }
                else
                {
                    return _curServer;
                }
            }
        }

        public event EventHandler FolderClicked;
        public event EventHandler FileClicked;
        public event EventHandler ServerChanged;
        
        // Every two seconds (or whatever), this timer's Tick handler starts
        // a worker thread for every path currently displayed on every PathGrid
        // to update the cells in the grid.
        private System.Windows.Forms.Timer _refreshTimer = new System.Windows.Forms.Timer();

        private System.Windows.Forms.Timer _filterTimer;
        private RemoteServer _localHost = new RemoteServer("LocalHost");
        private RemoteServer _curServer; 
        private List<RemoteServer> _remoteServers; // Does not include _localHost.
        private static Logger Log = Logger.GetLogger("StartPage");

        protected override void OnLoad(EventArgs e)
        {
            using (Log.InfoCall())
            {
                base.OnLoad(e);
                InitColors();

                if (AppArgs.ServerName == null)
                {
                    ShowServerPane(Settings.Default.ShowServers, _localHost);
                }
                else
                {
                    // Search first for a matching display name then for a matching "address". If not found create a new one.

                    RemoteServer firstServer = _remoteServers.FirstOrDefault(remsvr => remsvr.HostName.ToLower() == AppArgs.ServerName.ToLower());

                    if (firstServer == null)
                    {
                        firstServer = _remoteServers.FirstOrDefault(remsvr => remsvr.HostAddress.ToLower() == AppArgs.ServerName.ToLower());

                        if (firstServer == null)
                        {
                            // Create a new RemoteServer from the given AppArgs.ServerName.

                            firstServer = new RemoteServer(AppArgs.ServerName);
                            _remoteServers.Add(firstServer);

                            // Call ShowServerPane() with doShow:true to force it to show the server pane and populate it with _remoteServers so
                            //  1) the server we just added will be visible (and if necessary editable) and
                            //  2) we can call SaveRemoteServers() to save the list with the new server.

                            ShowServerPane(doShow: true, serverToSelect: firstServer);

                            // Save the list of servers because we just added one.

                            serverTree1.SaveRemoteServers();
                        }
                        else
                        {
                            ShowServerPane(doShow: true, serverToSelect: firstServer);
                        }
                    }
                    else
                    {
                        ShowServerPane(doShow: true, serverToSelect: firstServer);
                    }
                }

                if (AppArgs.FilePath != null)
                {
                    // This means a file path was specified in the command line args.  Pretend the 
                    // user user clicked the file so the event handler will attempt to read it.

                    ClickedPath = AppArgs.FilePath;
                    FileClicked(this, EventArgs.Empty);
                }

                //filesGrid.ArchiveVisibility = Settings.Default.ShowViewedArchives ? ArchiveVisibility.ViewedArchives : ArchiveVisibility.NoArchives;
                //filesGrid.ShowTimesAgo = Settings.Default.ShowFileTimesAgo;
                //foldersGrid.ShowTimesAgo = Settings.Default.ShowFolderTimesAgo;
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (_curServer != null && !DesignMode)
            {
                if (Visible)
                {
                    if (_curServer == _localHost)
                    {
                        StartLocalFileWatch(false);
                    }
                    else
                    {
                        StopLocalFileWatch();

                        filesGrid.AddOrUpdatePaths(_curServer.Files);
                        foldersGrid.AddOrUpdatePaths(_curServer.Folders);
                    }

                    StartRefreshTimer();
                }
                else
                {
                    // We don't watch for changes or auto-refresh anything while not visible.

                    StopLocalFileWatch();
                    _refreshTimer.Enabled = false;
                }
            }
        }

        private void InitColors()
        {
            // Propagate the BackColor to the appropriate child controls.

            pnlVersion.BackColor = this.BackColor;
            splitContainer1.Panel1.BackColor = serverTree1.BackColor;
            splitContainer1.Panel2.BackColor = this.BackColor;
            //splitContainer2.Panel1.BackColor = this.BackColor;
            //splitContainer2.Panel2.BackColor = this.BackColor;
            filesGrid.BackColor = this.BackColor;
            foldersGrid.BackColor = this.BackColor;
        }

        private void _refreshTimer_Tick(object sender, EventArgs e)
        {
            // This is called in the UI thread.

            if (_refreshTimer.Enabled)
            {
                filesGrid.RefreshPathItems();
                foldersGrid.RefreshPathItems();

                // Do it again in 2 seconds.
                _refreshTimer.Interval = 2000;
            }
            else
            {
                Log.Info("_updateTimer is disabled in _updateTimer_Tick()!");
            }
        }

        private void StartRefreshTimer() 
        {
            if (!_refreshTimer.Enabled)
            {
                // Do the first refresh in a short time, then slow down.
                _refreshTimer.Interval = 200;
                _refreshTimer.Enabled = true;
            }        
        }

        // Called when this control becomes visible.
        private void StartLocalFileWatch(bool forceSetPaths)
        {
            if (!RecentFilesAndFolders.IsWatching)
            {
                Debug.WriteLine("StartPage - start watching local files.");

                // This event is raised on a worker thread.
                RecentFilesAndFolders.FilesChanged += LocalFilesChanged;
                RecentFilesAndFolders.FoldersChanged += LocalFoldersChanged;
                RecentFilesAndFolders.IsWatching = true;
                RecentFilesAndFolders.ForceRefresh(forceSetPaths); // will raise the above event on first call or if files have changed or parameter is true.

                RefreshLocalViewedFiles();
                RefreshLocalViewedFolders();

                // Do the first refresh in a short time, then slow down.
                _refreshTimer.Interval = 200;
                _refreshTimer.Enabled = true;
            }
        }

        // Called when this control becomes not visible.
        private void StopLocalFileWatch()
        {
            Debug.WriteLine("StartPage - stop watching local files.");

            //_refreshTimer.Stop();
            RecentFilesAndFolders.IsWatching = false;
            RecentFilesAndFolders.FilesChanged -= LocalFilesChanged;
            RecentFilesAndFolders.FoldersChanged -= LocalFoldersChanged;
        }

        private void LocalFilesChanged(object sender, EventArgs e)
        {
            filesGrid.AddOrUpdatePaths(RecentFilesAndFolders.Files);
        }

        void LocalFoldersChanged(object sender, EventArgs e)
        {
            foldersGrid.AddOrUpdatePaths(RecentFilesAndFolders.Folders);
        }

        private void RefreshLocalViewedFiles()
        {
            try
            {
                if (Settings.Default.RecentFiles != null && Settings.Default.RecentFiles.Length > 0)
                {
                    List<PathItem> pathItems = PathItem.MakePathItems(Settings.Default.RecentFiles, false);
                    filesGrid.AddOrUpdatePaths(pathItems);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void RefreshLocalViewedFolders()
        {
            try
            {
                if (Settings.Default.RecentFolders != null && Settings.Default.RecentFolders.Length > 0)
                {
                    List<PathItem> pathItems = PathItem.MakePathItems(Settings.Default.RecentFolders, true);
                    foldersGrid.AddOrUpdatePaths(pathItems);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void linkWebsite_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                Process.Start("http://TracerX.CodePlex.com/releases");
            }
            catch (Exception)
            {
            }
        }

        private void linkLabel1_MouseClick(object sender, MouseEventArgs e)
        {
            OptionsDialog dlg = new OptionsDialog();
            dlg.SelectTab(OptionsTab.VersionChecking);
            dlg.ShowDialog();
        }

        // Called when any file or folder link is clicked on any of the PathControls.
        private void PathGrid_LastClickedItemChanged(object sender, EventArgs e)
        {
            var pathGrid = sender as PathControl;

            if (pathGrid.LastClickedItem.StartsWith("file\n") && FileClicked != null)
            {
                ClickedPath = pathGrid.LastClickedItem.Substring(5);
                FileClicked(this, EventArgs.Empty);
            }
            else if (pathGrid.LastClickedItem.StartsWith("folder\n") && FolderClicked != null)
            {
                ClickedPath = pathGrid.LastClickedItem.Substring(7);
                FolderClicked(this, EventArgs.Empty);
            }
        }

        private void SetCurServer(RemoteServer newServer)
        {
            using (Log.InfoCall())
            {
                Log.Info("newServer = ", newServer, "_curServer = ", _curServer);

                if (newServer != _curServer)
                {
                    Log.Info("Setting _curServer to ", newServer);

                    if (newServer == _localHost)
                    {
                        ClearAllPaths();
                        SetPathsAreLocal(true);
                        StartLocalFileWatch(forceSetPaths: true);
                    }
                    else
                    {
                        Cursor originalCursor = this.Cursor;
                        this.Cursor = Cursors.WaitCursor;

                        StopLocalFileWatch();
                        ClearAllPaths();
                        SetPathsAreLocal(false);

                        DialogResult dr = DialogResult.Yes;

                        // Retry connecting to the server while dr == Yes.

                        while (dr == DialogResult.Yes)
                        {
                            Application.DoEvents();

                            try
                            {
                                // newServer.Refresh() is likely to throw an exception because it performs
                                // WCF calls to the TracerX-Service on the remote host.

                                Log.Info("Connecting to server ", newServer);
                                newServer.Refresh();
                                RefreshGrids(newServer);
                                break;
                            }
                            catch (Exception ex)
                            {
                                Log.Warn("Exception getting files from server ", newServer, ": ", ex);
                                string msg = "Error getting file list from server '" + newServer + "'.";
                                MessageBoxButtons buttons = MessageBoxButtons.OK;

                                if (ex is SecurityNegotiationException && ex.InnerException is InvalidCredentialException)
                                {
                                    msg += "\nWould you like to specify credentials for the connection?";
                                    buttons = MessageBoxButtons.YesNo;
                                }

                                while (ex != null)
                                {
                                    msg += "\n\n" + ex.Message;
                                    ex = ex.InnerException;
                                }

                                dr = MainForm.ShowMessageBoxBtns(msg, buttons);

                                if (dr == DialogResult.Yes)
                                {
                                    var credDlg = new CredentialsDialog();

                                    credDlg.UserID = newServer.UserId;
                                    credDlg.PW = newServer.PW;

                                    if (credDlg.ShowDialog() == DialogResult.OK)
                                    {
                                        newServer.UserId = credDlg.UserID;

                                        if (credDlg.UserID == "")
                                        {
                                            newServer.PW = null;
                                        }
                                        else
                                        {
                                            newServer.PW = credDlg.PW;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        this.Cursor = originalCursor;
                    }

                    filesGrid.RemoteServer = newServer;
                    foldersGrid.RemoteServer = newServer;
                    _curServer = newServer;
                    OnServerChanged();
                }
            }
        }

        private void OnServerChanged()
        {
            if (ServerChanged != null)
            {
                ServerChanged(this, EventArgs.Empty);
            }
        }

        private void RefreshGrids(RemoteServer newServer)
        {
            filesGrid.AddOrUpdatePaths(newServer.Files);
            foldersGrid.AddOrUpdatePaths(newServer.Folders);
        }

        private void ClearAllPaths() 
        {
            filesGrid.AddOrUpdatePaths(null);
            foldersGrid.AddOrUpdatePaths(null);
        }

        private void SetPathsAreLocal(bool pathsAreLocal)
        {
            // This enables or prevents the PathGrids from trying to check if their
            // paths exist, get their size, etc.

            filesGrid.PathsAreLocal = pathsAreLocal;
            foldersGrid.PathsAreLocal = pathsAreLocal;
        }

        //private void comboServers_DropDownClosed(object sender, EventArgs e)
        //{
        //    splitContainer2.Focus();
        //}

        private void control_MouseEnter(object sender, EventArgs e)
        {
            (sender as Control).BackColor = Color.Gold;
        }

        private void control_MouseLeave(object sender, EventArgs e)
        {
            (sender as Control).BackColor = Color.Empty;
        }

        private void ManageServers()
        {
            var form = new ServerListEditor(_remoteServers);

            form.StartPosition = FormStartPosition.CenterParent;

            if (form.ShowDialog() == DialogResult.OK)
            {
                _remoteServers = form.NewServers;
                serverTree1.PopulateServerTree(_localHost, _remoteServers);
                serverTree1.SelectedServer = _curServer;

                if (serverTree1.SelectedServer == null)
                {
                    // _curServer must have been removed.
                    serverTree1.SelectedServer = _localHost;
                }

                serverTree1.SaveRemoteServers();
            }
        }

        private void SettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DaysToSaveViewTimes")
            {
                // Remove older viewed files for the remote servers and _localHost.
              
                DateTime cutoff = DateTime.Now.AddDays(-Settings.Default.DaysToSaveViewTimes);

                foreach (RemoteServer rs in _remoteServers)
                {
                    rs.ForgetOldViewTimes(cutoff);
                }

                _localHost.ForgetOldViewTimes(cutoff);

                RefreshFilesAndFolders();
            }
            else if (e.PropertyName == "ShowTracerxFiles")
            {
                filesGrid.FilterPathItems(force: true);
                foldersGrid.FilterPathItems(force: true);
            }
            else if (e.PropertyName == "ShowServers")
            {
                ShowServerPane(Settings.Default.ShowServers, _curServer);
            }
        }

        public void RefreshFilesAndFolders()
        {
            Cursor originalCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (_curServer == _localHost)
                {
                    StopLocalFileWatch();
                    ClearAllPaths();
                    StartLocalFileWatch(forceSetPaths: true);
                }
                else
                {
                    _curServer.Refresh();
                    ClearAllPaths();
                    RefreshGrids(_curServer);
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Exception getting files from server ", _curServer, ": ", ex);
                string msg = "Error getting file list from server '" + _curServer + "'.";

                while (ex != null)
                {
                    msg += "\n\n" + ex.Message;
                    ex = ex.InnerException;
                }

                MainForm.ShowMessageBox(msg);
            }
            finally
            {
                this.Cursor = originalCursor;
            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            splitContainer1.BackColor = Color.DarkGray;
        }

        private void splitContainer1_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            splitContainer1.BackColor = this.BackColor;
        }

        private void gridPanel_MouseEnter(object sender, EventArgs e)
        {
            // Try to make mouse wheel work...
            (sender as Control).Focus();
        }

        private void serverTree1_SelectedServerChanged(object sender, EventArgs e)
        {
            if (serverTree1.SelectedServer == null)
            {
                btnRemoveServer.Enabled = false;
            }
            else if (serverTree1.SelectedServer == _localHost)
            {
                btnRemoveServer.Enabled = false;
                SetCurServer(serverTree1.SelectedServer);
            }
            else
            {
                btnRemoveServer.Enabled = true;
                SetCurServer(serverTree1.SelectedServer);
            }
        }
     
        // Show or hide the server pane and connect to the specified server.
        private void ShowServerPane(bool doShow, RemoteServer serverToSelect)
        {
            if (doShow)
            {
                splitContainer1.Panel1Collapsed = false;
                serverTree1.PopulateServerTree(_localHost, _remoteServers);
                serverTree1.SelectedServer = serverToSelect;
            }
            else
            {
                splitContainer1.Panel1Collapsed = true;
                serverTree1.Nodes.Clear();
                SetCurServer(serverToSelect);
            }
        }

        private void btnLeftManageServers_Click(object sender, EventArgs e)
        {
            ManageServers();
        }
       
        private void filesGrid_RefreshClicked(object sender, EventArgs e)
        {
            RefreshFilesAndFolders();
        }

        private void btnAddServer_Click(object sender, EventArgs e)
        {
            // Get list of unique categories.
            string[] categories =_remoteServers
                .Select(server => server.Category)
                .Where(cat => !string.IsNullOrWhiteSpace(cat))
                .Distinct()
                .ToArray();

            var editDlg = new ServerConnEditor();
            editDlg.Init(categories, _remoteServers.Select(svr => svr.HostName), null);
            editDlg.ShowConnectButton = true;

            if (editDlg.ShowDialog() == DialogResult.OK)
            {
                var newRemoteServer = new RemoteServer(editDlg.NewServer);
                serverTree1.BeginUpdate();
                serverTree1.AddServerToTree(newRemoteServer, changeMasterList: true);
                serverTree1.EndUpdate();

                // Ask the user if he wants to connect to the new server.
                //if (MainForm.ShowMessageBoxBtns("Connect to '" + newRemoteServer.HostName + "'?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (editDlg.DoConnect)
                {
                    serverTree1.SelectedServer = newRemoteServer;
                }
            }
        }

        private void btnRemoveServer_Click(object sender, EventArgs e)
        {
            serverTree1.ConfirmAndRemoveServer(serverTree1.SelectedServer);
        }

        private void txtServerFilter_TextChanged(object sender, EventArgs e)
        {
            Debug.Print("txtServerFilter_TextChanged");

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
            _filterTimer.Enabled = false;

            // We don't trim or otherwise alter the filter textbox because the user
            // may still be typing and want leading, trailing, or embedded blanks.

            serverTree1.BeginUpdate();
            serverTree1.ApplyFilter(txtServerFilter.Text);
            serverTree1.EndUpdate();

            if (txtServerFilter.Text == "")
            {
                // No filter so use regular background color).
                txtServerFilter.BackColor = Color.Empty;
            }
            else
            {
                // Use yellow backcolor to indicate active filter.
                txtServerFilter.BackColor = Color.Yellow;
            }
        }
    }    
}
