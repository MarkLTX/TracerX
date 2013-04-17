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
using TracerX.Viewer;

namespace TracerX
{
    public partial class StartPage : UserControl
    {
        public StartPage()
        {
            InitializeComponent();

            if (!DesignMode)
            {
                pnlVersion.Hide();
                VersionChecker.NewVersionFound += (sender, e) => pnlVersion.Show();
                VersionChecker.CheckForNewVersion();
            }
        }

        // Set before showing.  The "Recently Created" files are read from here.
        public string RecentlyCreatedListFile;

        /// <summary>
        /// Number of files to show in the "Recently Created" list. 
        /// Set before showing.
        /// </summary>
        [Browsable(true)]
        [Description("Max number of files/folders to display in each list.")]
        public int MaxNumPaths
        {
            get;
            set;
        }

        public string ClickedPath
        {
            get;
            private set;
        }

        public event EventHandler FolderClicked;
        public event EventHandler FileClicked;

        // Stuff for watching/monitoring RecentlyCreated.txt.
        FileInfo _fileInfo;
        DateTime _lastTimestamp = DateTime.MinValue;
        FileSystemWatcher _fileWatcher;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
            {
                recentlyCreatedFiles.Paths = new string[] { "a list of files appears here" };
                recentlyCreatedFiles.Show();
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (!DesignMode)
            {
                if (Visible)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
        }

        // Called when this control becomes visible.
        private void Start()
        {
            // The existence of _fileInfo indicates if we've already started or not.

            if (_fileInfo == null)
            {
                Debug.WriteLine("StartPage - start watching");

                try
                {
                    _fileInfo = new FileInfo(RecentlyCreatedListFile);

                    if (_fileInfo.Exists)
                    {
                        _lastTimestamp = DateTime.MinValue;
                        _fileWatcher = new FileSystemWatcher(_fileInfo.DirectoryName, "*.txt");
                        _fileWatcher.Changed += new FileSystemEventHandler(_fileWatcher_Changed);
                        _fileWatcher.EnableRaisingEvents = true;

                        RefreshRecentlyCreated();
                        //recentlyViewedFiles.SendToBack();
                        //recentlyCreatedFiles.SendToBack();
                        //recentlyViewedFiles.SendToBack();
                        //recentlyCreatedFiles.SendToBack();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

                RefreshRecentlyViewedFiles();
                RefreshRecentlyViewedFolders();

                Properties.Settings.Default.PropertyChanged += new PropertyChangedEventHandler(Settings_PropertyChanged);
            }
        }

        // Called when this control becomes not visible.
        private void Stop()
        {
            // The existence of _fileInfo indicates if we've already started or not.

            if (_fileInfo != null)
            {
                Debug.WriteLine("StartPage - stop watching");

                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Dispose();
                _fileWatcher = null;
                Properties.Settings.Default.PropertyChanged -= Settings_PropertyChanged;
                _fileInfo = null;
            }
        }

        private void RefreshRecentlyViewedFiles()
        {
            try
            {
                if (Settings.Default.MRU == null || Settings.Default.MRU.Count == 0)
                {
                    recentlyViewedFiles.Paths = null;
                }
                else
                {
                    recentlyViewedFiles.Paths = Settings.Default.MRU.Cast<string>().Reverse().Where(file => File.Exists(file)).Take(MaxNumPaths);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void RefreshRecentlyViewedFolders()
        {
            try
            {
                if (Settings.Default.RecentFolders == null || Settings.Default.RecentFolders.Count == 0)
                {
                    recentlyViewedFolders.Paths = null;
                }
                else
                {
                    recentlyViewedFolders.Paths = Settings.Default.RecentFolders.Cast<string>().Reverse().Where(folder => Directory.Exists(folder)).Take(MaxNumPaths);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MRU":
                    RefreshRecentlyViewedFiles();
                    break;
                case "RecentFolders":
                    RefreshRecentlyViewedFolders();
                    break;
            }
        }

        // Called when RecentlyCreated.txt changes (typically twice for some reason).
        void _fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath == RecentlyCreatedListFile)
            {
                // If we don't sleep, the file will be 'in use by another process' when we try to open it.
                // This method runs in a worker thread.

                Thread.Sleep(200);
                Invoke(new Action(RefreshRecentlyCreated));
            }
        }

        private void RefreshRecentlyCreated()
        {
            try
            {
                // This tends to get called twice every time RecentlyCreated.txt changes, so
                // check the timestamp to verify it has really changed.

                _fileInfo.Refresh();

                if (_fileInfo.Exists &&
                    _fileInfo.LastWriteTimeUtc > _lastTimestamp &&
                    MaxNumPaths > 0 )
                {
                    List<string> keepers = new List<string>();

                    _lastTimestamp = _fileInfo.LastWriteTimeUtc;

                    foreach (string file in File.ReadAllLines(RecentlyCreatedListFile))
                    {
                        if (File.Exists(file))
                        {
                            keepers.Add(file);

                            if (keepers.Count == MaxNumPaths)
                            {
                                break;
                            }
                        }
                    }

                    recentlyCreatedFiles.Paths = keepers;
                }
                else
                {
                    recentlyCreatedFiles.Paths = null;
                }
            }
            catch (Exception ex)
            {
                recentlyCreatedFiles.Paths = null;
                Debug.WriteLine(ex.ToString());
            }
        }
        
        // Event handler called when any path is clicked on any of the PathLists.
        private void recentlyCreatedPaths_LastClickedPathChanged(object sender, EventArgs e)
        {
            var pathList = sender as PathList;

            ClickedPath = pathList.LastClickedPath;

            if (FileClicked != null && !pathList.PathsAreFolders)
            {
                FileClicked(this, EventArgs.Empty);
            }
            else if (FolderClicked != null && pathList.PathsAreFolders)
            {
                FolderClicked(this, EventArgs.Empty);
            }
        }

        private void linkWebsite_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                Process.Start("http://TracerX.CodePlex.com/releases");
            }
            catch (Exception )
            {
            }
        }

        private void linkLabel1_MouseClick(object sender, MouseEventArgs e)
        {
            OptionsDialog dlg = new OptionsDialog();
            dlg.tabControl1.SelectedTab = dlg.versionPage;
            dlg.ShowDialog();
        }
    }
}
