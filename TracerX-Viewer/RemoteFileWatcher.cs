using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;
using System.Net;

namespace TracerX
{
    /// <summary>
    /// Watches a remote TracerX file for changes/renames.
    /// </summary>
    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Single)]
    internal class RemoteFileWatcher : IFileWatcher, IFileMonitorCallback
    {
        // The file path is something that will be valid on the remote host.
        // The guidForEvents will be the file's guid read from the preamble of the 
        // first Session if the remote TracerX-Service implements the version of
        // IFileMonitor that includes the StartWatching(string filePath, Guid guidForEvents) method.
        // The hostAndPort has the form "host[:port]", where the port is optional
        public RemoteFileWatcher(string path, Guid guidForEvents, string hostAndPort, NetworkCredential credentials)
        {
            using (Log.DebugCall())
            {
                _path = path;
                _hostAndPort = hostAndPort;
                _creds = credentials;
                _guidForEvents = guidForEvents;

                Start();
            }
        }

        private ProxyFileMonitor _service;
        private string _path;
        private Guid _guidForEvents;
        private string _hostAndPort;
        private NetworkCredential _creds;
        private bool _recursing;
        private object _lock = new object();

        private static Logger Log = Logger.GetLogger("RemoteFileWatcher");

        private void Start()
        {
            // This class (RemoteFileWatcher) is both the client and the callback object.
            _service = new ProxyFileMonitor(this);

            // Must come before assigning the event handlers.
            Log.Debug("Setting host to ", _hostAndPort);
            _service.SetHost(_hostAndPort);
            _service.SetCredentials(_creds);

            _service.InnerChannel.Opening += (sender, e) => Log.Debug("RemoteFileWatcher opening.");
            _service.InnerChannel.Opened += (sender, e) => Log.Debug("RemoteFileWatcher opened.");
            _service.InnerChannel.Closing += (sender, e) => Log.Debug("RemoteFileWatcher closing.");
            _service.InnerChannel.Closed += (sender, e) => Log.Debug("RemoteFileWatcher closed.");

            Log.Debug("Calling StartWatching() for file ", _path);

            if (_guidForEvents == Guid.Empty)
            {
                _service.StartWatching(_path);// This will throw an exception if the server is offline, the service is not running, etc.
            }else
            {
                _service.StartWatching2(_path, _guidForEvents);// This will throw an exception if the server is offline, the service is not running, etc.
            }

            // We established a connection, now watch for it to fault.
            _service.InnerChannel.Faulted += InnerChannel_Faulted;
        }

        void InnerChannel_Faulted(object sender, EventArgs e)
        {
            using (Log.DebugCall())
            {
                if (_recursing)
                {
                    Log.Debug("Ignoring recursive call.");
                }
                else
                {
                    _recursing = true;

                    // It's hard to know why the channel faulted, but possible reasons are
                    //  1) The watched file didn't change for so long there was a timeout.
                    //  2) The service was stopped or crashed.
                    //  3) The remote server went offline.
                    //  4) A "network error" occurred.

                    lock (_lock)
                    {
                        try
                        {
                            Log.Info("Attempting to restart the watcher on ", _hostAndPort);
                            Start();
                        }
                        catch (Exception ex)
                        {
                            Log.Info("Exception trying to reconnect to ", _hostAndPort, " after channel faulted: ", ex);
                            string msg = string.Format("The connection to {0} failed and could not be re-established due to this error:\n\n{1}", _hostAndPort, ex.Message);
                            MainForm.ShowMessageBox(msg);
                            Stop();
                        }

                        if (!Stopped)
                        {
                            // The file may have changed while we weren't watching it, so cause the viewer to check it.
                            (this as IFileMonitorCallback).FileChanged(WatcherChangeTypes.Changed, _path);
                        }
                    }

                    _recursing = false;
                }
            }
        }

        #region IFileWatcher Members

        public event System.IO.FileSystemEventHandler Changed;
        public event System.IO.RenamedEventHandler Renamed;

        public bool Stopped
        {
            get { return _service == null; }
        }

        public void Stop()
        {
            using (Log.DebugCall())
            {
                lock (_lock)
                {
                    if (_service != null)
                    {
                        // Since we're stopping we don't need to handle the Faulted event any more.
                        // Also if we continue to handle the Faulted event it can cause recursion
                        // and try to restart the connection if it gets raised in here.

                        _service.InnerChannel.Faulted -= InnerChannel_Faulted;

                        if (_service.State == CommunicationState.Opened)
                        {
                            try
                            {
                                Log.Debug("Calling _service.StopWatching()");
                                _service.StopWatching(); // Can raise the Faulted event.
                            }
                            catch (Exception ex)
                            {
                                // Most likely, the proxy is faulted.
                                Log.Debug(ex);
                            }
                        }

                        // We have our own implementation of Dispose() that's 
                        // safe to call even if the proxy is faulted.

                        Log.Debug("Calling _service.Dispose()");
                        _service.Dispose();

                        _service = null;
                    }
                }
            }
        }

        #endregion

        #region IFileMonitorCallback Members

        void IFileMonitorCallback.FileChanged(WatcherChangeTypes changeType, string path)
        {
            using (Log.DebugCall())
            {
                if (Changed != null && !Stopped)
                {
                    FileSystemEventArgs args = new FileSystemEventArgs(changeType, Path.GetDirectoryName(path), Path.GetFileName(path));
                    Log.Debug("Raising Changed event");
                    Changed(this, args);
                }
                else
                {
                    Log.Debug("NOT raising the changed event.  Stopped = ", Stopped);
                }

                //return Stopped;
            }
        }

        void IFileMonitorCallback.FileRenamed(WatcherChangeTypes changeType, string path, string oldPath)
        {
            if (Renamed != null && !Stopped)
            {
                RenamedEventArgs args = new RenamedEventArgs(changeType, Path.GetDirectoryName(path), Path.GetFileName(path), Path.GetFileName(oldPath));
                Log.Debug("Raising Renamed event");
                Renamed(this, args);
            }
        }

        #endregion
    }
}
