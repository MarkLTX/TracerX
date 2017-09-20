using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;

namespace TracerX
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Single)]
    public class ImplFileMonitor : IFileMonitor, IDisposable
    {
        static readonly Logger Log = Logger.GetLogger("TX1.Service.ImplFileMonitor");
        private IFileMonitorCallback _callback;
        private LocalFileWatcher _watcher;
        private string _filePath;
        private object _lock = new object();

        #region IFileMonitor Members

        public int ExchangeVersion(int clientVersion)
        {
            return 1;
        }

        // Stop watching the current file, if any, and start
        // watching the specified file.
        public void StartWatching(string filePath)
        {
            StartWatching2(filePath, Guid.Empty);
        }

        // Stop watching the current file, if any, and start
        // watching the specified file.
        public void StartWatching2(string filePath, Guid guidForEvents)
        {
            using (Log.InfoCall())
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new FaultException("The file path to watch is null or empty.");
                }
                else if (!filePath.EndsWith(".tx1", StringComparison.OrdinalIgnoreCase))
                {
                    throw new FaultException("The file extension of the file to watch must be \".tx1\".");
                }
                else
                {
                    try
                    {
                        lock (_lock)
                        {
                            StopWatching();

                            Log.Info("Will start watching file: ", filePath);
                            _filePath = filePath;
                            _watcher = new LocalFileWatcher(filePath, guidForEvents);

                            // When the file changes, we'll notify the client via this callback object.
                            _callback = OperationContext.Current.GetCallbackChannel<IFileMonitorCallback>();

                            (_callback as ICommunicationObject).Closing += (sender, e) => Log.Debug("Callback channel is closing.");
                            (_callback as ICommunicationObject).Closed += (sender, e) => Log.Debug("Callback channel is closed.");
                            (_callback as ICommunicationObject).Faulted += (sender, e) => Log.Debug("Callback channel is faulted.");

                            _watcher.Changed += _watcher_Changed;
                            _watcher.Renamed += _watcher_Renamed;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        throw;
                        //throw new FaultException<ExceptionDetail>(new ExceptionDetail(ex), "Exception watching file '" + (filePath ?? "<null>") + "' (on the server).");
                    }
                }
            }
        }

        // Stop watch the current file, if any.
        public void StopWatching()
        {
            try
            {
                LocalFileWatcher temp = null;

                lock (_lock)
                {
                    if (_watcher != null)
                    {
                        Log.Info("Will stop watching file: ", _filePath);
                        temp = _watcher;
                        _watcher = null;
                    }
                }

                if (temp != null)
                {
                    temp.Stop();
                    Log.Info("Stopped the local watcher.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
                //throw new FaultException<ExceptionDetail>(new ExceptionDetail(ex), "Exception stopping watching file '" + (_filePath ?? "<null>") + "' (on the server).");
            }
        }

        #endregion

        void _watcher_Renamed(object sender, RenamedEventArgs e)
        {
            //lock (_lock)
            {
                // Sometimes the watcher keeps raising the event long after
                // we've called _watcher.Stop() and set _watcher to null,
                // so confirm we're still watching the file before calling back.

                if (sender == _watcher)
                {
                    try
                    {
                        Log.Debug("Calling back file rename.");
                        _callback.FileRenamed(e.ChangeType, e.FullPath, e.OldFullPath);
                    }
                    catch (Exception ex)
                    {
                        // Have gotten TimeoutException here.
                        Log.Error("Exception calling _callback.FileRenamed(), state is ", (_callback as ICommunicationObject).State, ": ", ex);
                    }
                }
            }
        }

        void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            // Sometimes the watcher keeps raising the event long after
            // we've called _watcher.Stop() and set _watcher to null,
            // so confirm we're still watching the file before calling back.

            if (sender == _watcher)
            {
                using (Log.VerboseCall("FileChangedCallback"))
                {
                    try
                    {
                        _callback.FileChanged(e.ChangeType, e.FullPath);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Exception calling _callback.FileChanged(), state is ", (_callback as ICommunicationObject).State, ": ", ex);
                    }
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            StopWatching();
            Log.Debug("Disposing an ImplFileMonitor.");
        }

        #endregion
    }
}
