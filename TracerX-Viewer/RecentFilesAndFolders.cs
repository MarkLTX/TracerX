using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace TracerX
{
    // Watches RecentlyCreated.txt in the TracerX data directory.  When it changes, reads it and RecentFolders.txt
    // and raises the FilesChanged event.
    // The Logger updates these files when it opens a log file.  We (the viewer) read them to populate
    // the start page.  
    internal static class RecentFilesAndFolders
    {
        static RecentFilesAndFolders()
        {
            Files = new List<PathItem>();
            Folders = new List<PathItem>();
            _watcher.Changed += new FileSystemEventHandler(_watcher_Changed);
        }

        public static bool IsWatching
        {
            get { return _watcher.EnableRaisingEvents; }
            set { _watcher.EnableRaisingEvents = value; }
        }

        public static event EventHandler FilesChanged;
        public static event EventHandler FoldersChanged;

        public static List<PathItem> Files
        {
            get { lock (_filesLock) return _files; }
            
            private set
            {
                lock (_filesLock)
                {
                    _files = value;
                    var eventHandlers = FilesChanged;

                    if (eventHandlers != null)
                    {
                        eventHandlers(null, EventArgs.Empty);
                    }
                }
            }
        }

        public static List<PathItem> Folders
        {
            get { lock (_foldersLock) return _folders; }

            private set
            {
                lock (_foldersLock)
                {
                    _folders = value;
                    var eventHandlers = FoldersChanged;

                    if (eventHandlers != null)
                    {
                        eventHandlers(null, EventArgs.Empty);
                    }
                }
            }
        }

        // Directory where TracerX stores its "global" data files.
        private static readonly string _dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TracerX");
        private static readonly FileSystemWatcher _watcher = new FileSystemWatcher(_dataDir, "RecentlyCreated.txt");

        // File that stores the list of recently created files.
        private static readonly FileInfo _filesFile = new FileInfo(Path.Combine(_dataDir, "RecentlyCreated.txt"));
        private static DateTime _filesTimestamp = DateTime.MinValue;
        private static List<PathItem> _files;
        private static readonly object _filesLock = new object();

        // File that stores the list of recently updated folders.
        private static readonly FileInfo _foldersFile = new FileInfo(Path.Combine(_dataDir, "RecentFolders.txt"));
        private static DateTime _foldersTimestamp = DateTime.MinValue;
        private static List<PathItem> _folders;
        private static readonly object _foldersLock = new object();

        private static Logger Log = Logger.GetLogger("RecentFilesAndFolders");

        public static void ForceRefresh(bool forceRaiseEvents)
        {
            using (Log.DebugCall())
            {
                Log.Debug(() => forceRaiseEvents);
                _watcher_Changed(forceRaiseEvents, null);
                //ThreadPool.QueueUserWorkItem((notused) => _watcher_Changed(forceRaiseEvents, null));
            }
        }

        // Called when RecentlyCreated.tx changes (typically twice for some reason).
        // This method runs in a worker thread and may run in multiple threads concurrently.
        private static void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            using (Log.DebugCall())
            {
                try
                {
                    List<PathItem> files = null;
                    List<PathItem> folders = null;
                    bool didRaiseFilesEvent = false;
                    bool didRaiseFoldersEvent = false;

                    // Try not to read the files at the same time they're being written by the logger, but don't wait forever.
                    using (new NamedMutexWait(NamedMutexWait.DataDirMUtexName, timeoutMs: 10000, throwOnTimeout: false))
                    {
                        var lines = MaybeReadFile(_foldersFile, ref _foldersTimestamp);
                        folders = PathItem.MakeRecentlyCreatedPathItems(lines, true);

                        lines = MaybeReadFile(_filesFile, ref _filesTimestamp);
                        files = PathItem.MakeRecentlyCreatedPathItems(lines, false);
                    }

                    if (files != null && folders != null)
                    {
                        // Old versions of the logger don't update the folder list, so there might be folders in the
                        // file list that aren't in the folder list.  Combine the folders from both lists.
                        var combinedFolders = new List<PathItem>();
                        var hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // for filtering duplicates.

                        foreach (PathItem file in files)
                        {
                            if (hashSet.Add(file.File.Directory.FullName))
                            {
                                combinedFolders.Add(new PathItem(file.File.Directory, true));
                            }
                        }

                        foreach (PathItem folder in folders)
                        {
                            if (hashSet.Add(folder.Folder.FullName))
                            {
                                combinedFolders.Add(folder);
                            }
                        }

                        if (!Folders.SequenceEqual(combinedFolders))
                        {
                            didRaiseFoldersEvent = true;
                            Folders = combinedFolders;
                        }
                    }

                    if (files != null && !Files.SequenceEqual(files))
                    {
                        didRaiseFilesEvent = true;
                        Files = files;
                    }

                    if (sender is bool && (bool)sender)
                    {
                        if (!didRaiseFilesEvent) Files = Files;
                        if (!didRaiseFoldersEvent) Folders = Folders;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        private static string[] MaybeReadFile(FileInfo listFile, ref DateTime timestamp)
        {
            string[] result = null;

            try
            {
                //Debug.Print("In RefreshFiles, checking file {0}", listFile);

                listFile.Refresh();

                //Debug.Print("  exists = {0}", listFile.Exists);

                if (listFile.Exists)
                {
                    // This tends to get called twice every time the file changes, so
                    // check the timestamp to verify it has really changed.

                    //Debug.Print("  Last write = {0}, prev timestamp = {1}", listFile.LastWriteTimeUtc, timestamp);

                    if (listFile.LastWriteTimeUtc > timestamp)
                    {
                        timestamp = listFile.LastWriteTimeUtc;
                        result = File.ReadAllLines(listFile.FullName);
                        //Debug.Print("  {0} lines read", result.Length);

                        // Don't check if every file/folder exists here because there could be a lot of them,
                        // and it's possible some were created on remote servers/shares that are no longer
                        // reachable, causing long time-outs.
                    }
                }
                else
                {
                    result = new string[0];
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return result;
        }
    }
}
