using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;

namespace TracerX
{
    /// <summary>
    /// Watches for changes to a file by combining FileSystemWatcher with polling.
    /// </summary>
    public class LocalFileWatcher :  IFileWatcher
    {
        public event FileSystemEventHandler Changed;
        public event RenamedEventHandler Renamed;
        public bool Stopped { get; private set; }

        private readonly Logger Log;

        private readonly object _lock = new object();
        private readonly FileSystemWatcher _fsw;
        private readonly string _filename;
        private readonly string _eventNameBase;

        // Event set by logger to notify us that the log file has changed.
        // If the logger supports this event we don't need _pollTimer, etc.
        private EventWaitHandle _viewersEvent;
        private RegisteredWaitHandle _viewersRegisteredWaitHandle;

        // These member are used together or not at all.  They are only needed if 
        // we're unable to set up the named system events that a sufficiently
        // recent version of the logger uses to signal use when it changes the file.
        // The _pollTimer is used to periodically check if the file's LastWriteTime or
        // size has changed.
        private readonly Timer _pollTimer;
        private readonly FileInfo _fileInfo;
        private DateTime _lastWriteTime;
        private long _lastSize;
        private int _pollInterval = 250;
        private const int _maxInterval = 2000;

        public LocalFileWatcher(string filepath, Guid guidForEvents)
        {
            _filename =  Path.GetFileNameWithoutExtension(filepath);
            _eventNameBase = guidForEvents.ToString();

            // The Logger name we use for logging is based on the name of the file we watch.
            Log = Logger.GetLogger("TX1.Watcher." +_filename);

            // Change events are processed one at a time by ChangeWorker() in a dedicated thread.
            ThreadPool.QueueUserWorkItem(ChangeWorker);

            // We need to know if the file is renamed or moved so create a 
            // FileSystemWatcher and set the appropriate flags in NotifyFilter.
            _fsw = new FileSystemWatcher(Path.GetDirectoryName(filepath), Path.GetFileName(filepath));
            
            // FileSystemWatcher is flaky and unreliable about detecting when a file is changed so hopefully the
            // logger is the "new version" that signals a named system event when it changes the file.
            // The names of the named system events used by this process and the logger's
            // process are based on the guidForEvents, which is really the file's guid.

            if (guidForEvents != Guid.Empty && SetupNamedEvents())
            {
                // Only use the FileSystemWatcher to detect when the file is renamed, moved, or deleted.
                // The logger will signal _viewersEvent when it changes the file.
                _fsw.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName;
            }
            else
            {
                // This means we can't rely on named system events being signaled by the logger to tell us when it
                // changes the log file.  Ask the FileSytemWatcher to notify us when the file's size or timestamp changes
                // and, since that's not very reliable, use _pollTimer to check if the file's size or LastWriteTime changes
                // when a certain amount of time elapses without _fsw raising an event.

                Log.Info("Unable to set up named events. Will watch file's LastWriteTime and size using FileSystemWatcher.");
                _fsw.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;

                _fileInfo = new FileInfo(filepath);
                _lastWriteTime = _fileInfo.LastWriteTime;
                _lastSize = _fileInfo.Length;
                Log.Info("Watcher created for ", _filename, " size = ", _fileInfo.Length, " write time = ", _fileInfo.LastWriteTime.ToString("hh:mm:ss.fff"));
            }

            _fsw.Changed += _fsw_Changed;
            _fsw.Created += _fsw_Changed;
            _fsw.Deleted += _fsw_Changed;
            _fsw.Renamed += _fsw_Renamed;
            _fsw.Error += _fsw_Error;

            _fsw.IncludeSubdirectories = false;
            _fsw.EnableRaisingEvents = true;

            if (_fileInfo != null)
            {
                // Since FileSystemWatcher isn't 100% reliable we also poll the file's size and LastWriteTime to detect changes
                _pollTimer = new Timer(new TimerCallback(PollTimer), null, _pollInterval, _pollInterval);
                Log.Debug("Started timer to poll file's size and LastWriteTime.");
            }
            else
            {
                // We don't need _pollTimer because the logger will  signal  _viewersEvent when it changes the file.
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                Stopped = true;
                Log.Info("Stopping file watcher for ", _filename);
                _fsw.EnableRaisingEvents = false;

                if (_pollTimer != null)
                {
                    _pollTimer.Dispose();
                }

                if (_viewersRegisteredWaitHandle != null)
                {
                    try
                    {
                        _viewersRegisteredWaitHandle.Unregister(null);
                    }
                    catch (Exception)
                    { }
                    finally
                    {
                        _viewersRegisteredWaitHandle = null;
                    }
                }

                if (_viewersEvent != null)
                {
                    try
                    {
                        _viewersEvent.Close();

                        // Notify the logger that we made a change so the logger can release it's handle to _viewersEvent.

                        EventWaitHandle loggersEvent = GetLoggersEvent();

                        if (loggersEvent != null)
                        {
                            Log.Debug("Signaling the file logger's named event.");
                            loggersEvent.Set();
                            loggersEvent.Close();
                        }
                    }
                    catch (Exception)
                    { }
                    finally
                    {
                        _viewersEvent = null;
                    }
                }
            }

            //_fsw.Dispose(); // Causes deadlocks.
        }

        // If the DLL that's writing to the log file is new enough it will have created a global system event named TX-<fileGuid>.
        // If that event exists then we can create our own event named TX<n>-<fileGuid> (where <n> is the first unused name
        // we can find), signal the logger's event, and then the logger will signal our event when it updates the file.  This is
        // more reliable than the definitely unreliable FileSystemWatcher class.
        private bool SetupNamedEvents()
        {
            using (Log.InfoCall())
            {
                EventWaitHandle loggersEvent = GetLoggersEvent();

                if (loggersEvent != null)
                {
                    // The logger did create its event so we should create our event.
                    // We don't know how many other viewers have created their own 
                    // events so try creating numbered events 0-9 until we make a new one.  This
                    // means there can be up to 10 viewers with events for the current file.

                    // Set the security so the logger, whose ID is not known, can set/signal the event.

                    bool createdNew;
                    EventWaitHandleSecurity security = new EventWaitHandleSecurity();
                    SecurityIdentifier authenticatedUsers = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);

                    security.AddAccessRule(new EventWaitHandleAccessRule(authenticatedUsers, EventWaitHandleRights.Modify, AccessControlType.Allow));
                    security.AddAccessRule(new EventWaitHandleAccessRule(authenticatedUsers, EventWaitHandleRights.ReadPermissions, AccessControlType.Allow));

                    // TODO: Granting the Synchronize right is only needed because the logger, when running in .NET Core, can't specify just Modify
                    // when it calls OpenExisting() to open the event.  Figure out how the logger can open the event without Synchronize access, then delete this statement.
                    security.AddAccessRule(new EventWaitHandleAccessRule(authenticatedUsers, EventWaitHandleRights.Synchronize, AccessControlType.Allow));

                    for (int i = 0; i < 10; ++i)
                    {
                        try
                        {
                            _viewersEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "Global\\TX" + i + '-' + _eventNameBase, out createdNew, security);

                            if (createdNew)
                            {
                                Log.Info("Created new named event for viewer with sequence number ", i);

                                // Use ThreadPool.RegisterWaitForSingleObject() to arrange for ViewersEventHandler to be called 
                                // when the logger signals _viewersEvent.  Then signal the logger's event to tell the logger to look for
                                // the existence of _viewersEvent and begin signaling it when messages are logged.

                                _viewersRegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject(_viewersEvent, ViewersEventHandler, null, -1, false);
                                loggersEvent.Set();
                                loggersEvent.Close();
                                break;
                            }
                            else
                            {
                                // We got some other viewer's event so forget it and keep trying to create a new one.
                                _viewersEvent.Close();
                                _viewersEvent = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Warn("Exception creating our (the viewer's) event: ", ex);
                            _viewersEvent = null;
                        }
                    }
                }

                return _viewersRegisteredWaitHandle != null;
            }
        }

        private EventWaitHandle GetLoggersEvent()
        {
            EventWaitHandle loggersEvent = null;

            try
            {
                loggersEvent = EventWaitHandle.OpenExisting("Global\\TX-" + _eventNameBase, EventWaitHandleRights.Modify);
                Log.Debug("Opened a handle to the file logger's named event.");
            }
            catch (WaitHandleCannotBeOpenedException ex) when (ex.Message == "No handle of the given name exists.")
            {
                // It's not unexpected for the event to not exist.
                Log.Info("The logger's event doesn't exist.");
            }
            catch (Exception ex)
            {
                Log.Warn("Exception opening the logger's event: ", ex);
            }

            return loggersEvent;
        }

        private void ResetPollInterval()
        {
            if (Stopped || _pollTimer == null) return;

            try
            {
                _pollTimer.Change(250, 250);
                _pollInterval = 250;
            }
            catch
            {
                // Sometimes get NullReferenceException even though _pollTimer is not null.
            }
        }

        private void NextPollInterval()
        {
            if (Stopped || _pollTimer == null) return;

            if (_pollInterval < _maxInterval)
            {
                _pollInterval = Math.Min(_maxInterval, _pollInterval * 2);
                _pollTimer.Change(_pollInterval, _pollInterval);
            }
        }

        void _fsw_Renamed(object sender, RenamedEventArgs e)
        {
            if (Stopped) return;

            using (Log.InfoCallThread("Watcher.Renamed"))
            {
                // Would love to know/log the new name, but it's not provided.
                Log.Info("Logfile was renamed.");
                Stop();
                OnRenamed(this, e);
            }
        }

        void _fsw_Error(object sender, ErrorEventArgs e)
        {
            Log.Error("Error in FileWatcher: ", e.GetException() ); 
            //Stop();
        }

        void _fsw_Changed(object sender, FileSystemEventArgs e)
        {
            if (Stopped) return;

            using (Log.VerboseCallThread("Watcher.Changed"))
            {
                Log.Verbose("File change event occurred for ", _filename, ", ChangeType = ", e.ChangeType);

                // I'm not sure it's possible for the ChangeType to be Changed when _fileInfo is null,
                // but LastWriteTimeChanged() requires _fileInfo to get the LastWriteTime.

                if (e.ChangeType == WatcherChangeTypes.Changed && _fileInfo != null)
                {
                    if (LastWriteTimeChanged(false))
                    {
                        OnChanged(e);
                    }
                }
                else
                {
                    OnChanged(e);
                }
            }
        }

        // Called in a random thread when the logger signals our _fileChangedEvent.
        private void ViewersEventHandler(object state, bool timedOut)
        {
            if (Stopped) return;

            using (Log.VerboseCallThread("Watcher.Changed"))
            {
                Log.Verbose("Named event set for ", _filename);

                // Pretend we got notified of a change to the file by the FileSystemWatcher.

                FileSystemEventArgs fakeArgs = new FileSystemEventArgs(WatcherChangeTypes.Changed, _fsw.Path, _fsw.Filter);
                OnChanged(fakeArgs);
            }
        }

        // This is the last FilesystemEventArgs received from _fsw, if any.
        private FileSystemEventArgs _lastEventArgs; 

        private void OnChanged(FileSystemEventArgs e)
        {
            if (Changed != null) 
            {
                // See "How to Use Wait and Pulse" at http://www.albahari.com/threading/part4.aspx

                lock (_lock)
                {
                    // _lastEventArgs is like a queue of length one.  It always holds the
                    // most recent EventArgs object (until ChangeWorker() starts processing it).
                    _lastEventArgs = e;

                    // If ChangeWorker() is waiting, this will release him to process _lastEventArgs.
                    // If ChangeWorker() is busy, this does nothing.  However, he will 
                    // process _lastEventArgs when done with his current work.
                    Monitor.Pulse(_lock);
                }
            }
            else
            {
                Log.Info("Changed event is null");
            }
        }

        // ChangeWorker() runs in a dedicated worker thread, processing EventArgs objects
        // placed in _lastEventArgs by OnChanged().  If multiple change events occur while
        // ChangeWorker() is busy, ChangeWorker() process the last one received when it
        // finishes the current one.
        private void ChangeWorker(object notused)
        {
            // See "How to Use Wait and Pulse" at http://www.albahari.com/threading/part4.aspx

            using (Log.VerboseCallThread("Watcher.ChangeWorker"))
            {
                while (true)
                {
                    FileSystemEventArgs evtArgs = null;

                    lock (_lock)
                    {
                        // Wait to receive EventArgs or be stopped.
                        while (_lastEventArgs == null && !Stopped) Monitor.Wait(_lock);

                        evtArgs = _lastEventArgs;
                        _lastEventArgs = null;
                    }

                    if (Stopped)
                    {
                        // This endeth the thread.
                        return;
                    }
                    else if (Changed != null)
                    {
                        Log.Verbose("Raising the Changed event");
                        Changed(this, evtArgs);
                    }
                    else
                    {
                        Log.Info("The Changed event is null!");
                    }
                }
            }
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (Renamed != null) Renamed(this, e);
        }

        private void PollTimer(object o)
        {
            if (LastWriteTimeChanged(true))
            {
                Log.Debug("File change detected by polling: ", _filename);
                FileSystemEventArgs fakeArgs = new FileSystemEventArgs(WatcherChangeTypes.Changed, _fsw.Path, _fsw.Filter);
                OnChanged(fakeArgs);
            }
        }

        private bool LastWriteTimeChanged(bool calledByPoller)
        {
            // On XP, the file's LastWriteTime changes automatically when the logger writes to the
            // file ONLY IF the size changes too.  If the file has wrapped (meaning the size stops changing),
            // the logger "manually" updates the LastWriteTime.  Thus, on XP, both properties change until
            // the file wraps, and then only the LastWriteTime changes.
            // On Vista, the LastWriteTime doesn't change automatically until the logger closes the file, period.
            // However, the size will change until the file wraps, then the logger starts "manually" setting
            // the LastWriteTime.  Thus, on Vista, only the size changes until the file wraps, then
            // only the LastWriteTime changes.

            DateTime prevWriteTime;
            long lastSize;

            lock (_lock)
            {
                try
                {
                    prevWriteTime = _lastWriteTime;
                    lastSize = _lastSize;

                    _fileInfo.Refresh();

                    _lastWriteTime = _fileInfo.LastWriteTime;
                    _lastSize = _fileInfo.Length;
                }
                catch (Exception ex)
                {
                    Log.Error("Exception in LastWriteTimeChanged: ", ex);
                    Stop();
                    return false;
                }

                // When watching a remote file via its UNC path (logger and viewer on Windows 7), 
                // the timestamp and length never seem to change faster than every 10 seconds
                // even though the file is changing more often and we are receiving fsw change notifications.
                // Therefore if this is a fsw change notification (not called by the poller), assume it's a real change.

                if (!calledByPoller)
                {
                    //Log.Verbose("LastWriteTimeChanged (not called by poller) resetting poll interval and returning true for ", _filename);
                    ResetPollInterval();
                    return true;
                }
                else if (prevWriteTime != _lastWriteTime || lastSize != _lastSize)
                {
                    Log.Verbose("LastWriteTimeChanged (called by poller) resetting poll interval and returning true for ", _filename);
                    ResetPollInterval();
                    return true;
                }
                else
                {
                    NextPollInterval();
                    return false;
                }
            }
        }
    }
}
