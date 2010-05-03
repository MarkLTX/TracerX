using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace TracerX {

    /// <summary>
    /// Watches for changes to a file by combining FileSystemWatcher with polling.
    /// </summary>
    internal class FileWatcher : IDisposable {
        public event FileSystemEventHandler Changed;
        public event RenamedEventHandler Renamed;
        public bool Stopped { get; private set; }

        private readonly object _lock = new object();
        private readonly FileSystemWatcher _fsw;
        private readonly Timer _pollTimer;
        private readonly FileInfo _fileInfo;
        private DateTime _lastWriteTime;
        private long _lastSize;
        private int _pollInterval = 500;
        private const int _maxInterval = 4000;

        public FileWatcher(string filepath) {
            _fsw = new FileSystemWatcher(Path.GetDirectoryName(filepath), Path.GetFileName(filepath));
            _fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.Size;
            _fsw.IncludeSubdirectories = false;

            _fsw.Changed += new FileSystemEventHandler(_fsw_Changed);
            _fsw.Created += _fsw_Changed;
            _fsw.Deleted += _fsw_Changed;
            _fsw.Renamed += new RenamedEventHandler(_fsw_Renamed);
            _fsw.Error += new ErrorEventHandler(_fsw_Error);

            _fsw.EnableRaisingEvents = true;

            _fileInfo = new FileInfo(filepath);
            _lastWriteTime  = _fileInfo.LastWriteTime;
            _lastSize = _fileInfo.Length;
            _pollTimer = new Timer(new TimerCallback(PollTimer), null, _pollInterval, _pollInterval);
        }

        public void Dispose() {
            Stop();
        }

        private void Stop() {
            lock (_lock) {
                Stopped = true;
                Debug.Print("Stopping file watcher.");
                _fsw.Dispose();
                _pollTimer.Dispose();
            }
        }

        private void ResetPollInterval() {
            if (Stopped) return;
            
            _pollTimer.Change(500, 500);
            _pollInterval = 500;
        }

        private void NextInterval() {
            if (Stopped) return;

            if (_pollInterval < _maxInterval) {
                _pollInterval = Math.Min(_maxInterval, _pollInterval * 2);
                _pollTimer.Change(_pollInterval, _pollInterval);
            }
        }

        void _fsw_Renamed(object sender, RenamedEventArgs e) {
            if (Stopped) return;
            Debug.Print("Logfile renamed.");
            Stop();
            OnRenamed(this, e);
        }

        void _fsw_Error(object sender, ErrorEventArgs e) {
            Debug.Print("Error in FileWatcher.");
            //Stop();
        }

        void _fsw_Changed(object sender, FileSystemEventArgs e) {
            if (Stopped) return;

            Debug.Print("File change event occurred.");

            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                if (LastWriteTimeChanged(false)) {
                    OnChanged(sender, e);
                }
            } else {
                OnChanged(sender, e);
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e) {
            if (Changed != null) Changed(this, e);
        }

        private void OnRenamed(object sender, RenamedEventArgs e) {
            if (Renamed != null) Renamed(this, e);
        }

        private void PollTimer(object o) {
            if (LastWriteTimeChanged(true)) {
                Debug.Print("File change detected by polling.");
                FileSystemEventArgs args = new FileSystemEventArgs(WatcherChangeTypes.Changed, _fsw.Path, _fsw.Filter);
                OnChanged(this, args);
            }
        }

        private bool LastWriteTimeChanged(bool calledByPoller) {
            // On XP, the file's LastWriteTime changes automatically when the logger writes to the
            // file ONLY IF the size changes too.  If the file has wrapped (meaning the size stops changing),
            // the logger "manually" updates the LastWriteTime.  Thus, on XP, both properties change until
            // the file wraps, and then only the LastWriteTime changes.
            // On Vista, the LastWriteTime doesn't change automatically until the logger closes the file, period.
            // However, the size will change until the file wraps, then the logger starts "manually" setting
            // the LastWriteTime.  Thus, on Vista, only the size changes until the file wraps, then
            // only the LastWriteTime changes.

            DateTime prevWriteTime = _lastWriteTime;
            long lastSize = _lastSize;

            lock (_lock) {

                try {
                    _fileInfo.Refresh();
                    _lastWriteTime = _fileInfo.LastWriteTime;
                    _lastSize = _fileInfo.Length;
                } catch (Exception ex) {
                    Stop();
                    return false;
                }

                if (prevWriteTime != _lastWriteTime || lastSize != _lastSize) {
                    ResetPollInterval();
                    return true;
                } else {
                    if (calledByPoller) {
                        NextInterval();
                    }

                    return false;
                }
            }
        }
    }
}
