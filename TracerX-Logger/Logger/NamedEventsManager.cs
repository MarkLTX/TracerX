using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;

namespace TracerX
{
    // Each BinaryFile contains an instance of this class and uses it to notify viewers 
    // when messages are written to the log file by signaling named system events.  
    // This feature works as follows.  
    //
    // The logger creates a system event (_loggersEvent) whose name is based on 
    // the file guid and registers a handler (LoggersEventHandler()) to be called 
    // whenever the event is signaled by a viewer.  
    //
    // When a viewer opens the file (either locally or through the TracerX-Service)
    // it extracts the file guid and creates its own system event whose name is 
    // based on the guid and includes a sequence number.  The viewer simply tries
    // creating events with sequence numbers 0-9 until it successfully creates a 
    // new one so each viewer has its own dedicated event.  After the viewer
    // creates the event and is watching for it to be signaled it signals the
    // logger's event.  It also signals the logger's event when it releases it's event 
    // and stops watching the file.
    //
    // The logger's handler (called when its event is signaled) closes any handles
    // it may have for viewer events so the OS will release the ones that have 
    // also been closed by the viewers.  Then it attempts to open each of the 10
    // possible viewer events to determine which ones are in use.  It keeps a list
    // of the in-use ones and signals them whenever new messages are logged.
    //
    // Since a malicious or rogue process could signal the logger's event very
    // frequently the handler has a 500 msec delay to prevent excessive processing.
    internal class NamedEventsManager
    {
        public NamedEventsManager(Guid fileGuid, string filePath)
        {
            _fileGuidString = fileGuid.ToString();
            //MaybeCreateMetaLog(filePath);
            CreateLoggersEvent();
        }

        const int _maxViewerEvents = 10;
        private string _fileGuidString;
        private object _eventsLock = new object();
        private EventWaitHandle _loggersEvent;
        private RegisteredWaitHandle _loggersRegisteredWaitHandle;
        private string[] _viewerEventNames;
        private List<EventWaitHandle> _viewerEvents;
        private AutoResetEvent _internalEvent;
        private bool _waitingToCheckViewers;
        private DateTime _deferCheckingOnViewersUntilTime;
        private DateTime _recheckViewersAfterTime;
        //private Logger MetaLog;

        // Use a timer to avoid signaling the events more than once per 100 ms.
        //private System.Timers.Timer _signalTimer;

        // The BinaryFile writer calls this every time it writes a message to the log file.
        // If any viewers have events in _viewerEvents, this will signal them in 100 ms or less.
        public void SignalEvents() 
        {
            // If this is a DEBUG build and a debugger is attached signal the viewers in this
            // thread right now so, if the user is stepping over a call to a logging method,
            // the output will immediately appear in the viewer.
#if DEBUG
            if (Debugger.IsAttached)
            {
                // This seems to have a big impact on performance.

                lock (_eventsLock)
                {
                    if (_viewerEvents != null && _viewerEvents.Count > 0)
                    {
                        SignalTheViewers();
                    }
                }

                return;
            }
#endif
            AutoResetEvent temp = _internalEvent;

            if (temp != null)
            {
                //MetaLog?.Info("Setting _internalEvent");
                temp.Set();
            }
        }

        //private void MaybeCreateMetaLog(string binaryFilePath)
        //{
        //    try
        //    {
        //        DateTime now = DateTime.Now;

        //        if (now.Year == 2017 && now.Month == 8 && MetaLog == null)
        //        {
        //            MetaLog = Logger.GetLogger("NamedEventsManager");

        //            // MetaLog uses a text file to avoid infinite recursion that could occur if we wrote to a binary file.

        //            MetaLog.BinaryFileTraceLevel = TraceLevel.Off;
        //            MetaLog.TextFileTraceLevel = TraceLevel.Info;
        //            MetaLog.TextFile = new TextFile()
        //            {
        //                Archives = 3,
        //                Directory = System.IO.Path.GetDirectoryName(binaryFilePath),
        //                Name = System.IO.Path.GetFileNameWithoutExtension(binaryFilePath) + " - NamedEventsManager",
        //                FullFilePolicy = FullFilePolicy.Close,
        //                MaxSizeMb = 1,
        //                Use_00 = true,
        //            };

        //            MetaLog.TextFile.Open();
        //            MetaLog.Info("Log file created.");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        private void SignalTheViewers()
        {
            // Each item in _viewerEvents represents a viewer that wants to know the file has changed.

            //Debug.Print("Signaling {0} viewer events.", _viewerEvents.Count);
            ////MetaLog?.Info("Signaling ", _viewerEvents.Count, " viewer events.");

            foreach (EventWaitHandle evt in _viewerEvents)
            {
                try
                {
                    evt.Set();
                }
                catch (Exception ex)
                {
                    ////MetaLog?.Info("Exception setting viewers event: ", ex.GetType(), ", ", ex.Message);
                }
            }

            ////MetaLog?.Info("Done signaling");
        }

        public void Close()
        {
            //MetaLog?.Info("Closing logger's event.");
            lock (_eventsLock)
            {
                if (_loggersRegisteredWaitHandle != null)
                {
                    try
                    {
                        _loggersRegisteredWaitHandle.Unregister(null);
                    }
                    catch (Exception)
                    { }
                    finally
                    {
                        _loggersRegisteredWaitHandle = null;
                    }
                }

                if (_loggersEvent != null)
                {
                    try
                    {
                        _loggersEvent.Close();
                    }
                    catch (Exception)
                    { }
                    finally
                    {
                        _loggersEvent = null;
                    }
                }

                AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;

                if (_viewerEvents != null)
                {
                    // Signal the viewers in case any messages have been sent since the last signal.

                    SignalTheViewers();

                    foreach (EventWaitHandle evt in _viewerEvents)
                    {
                        evt.Close();
                    }

                    _viewerEvents.Clear();
                }
            }
        }

        // This creates the named system event that "belongs to" the logger (the logger has a handler for when
        // the event is signaled). The event will be signaled by TracerX-Service or TracerX-Viewer when the
        // file is viewed.
        private void CreateLoggersEvent()
        {
            lock (_eventsLock)
            {
                string step = "creating named event"; 
                string eventName = "Global\\TX-" + _fileGuidString;
                bool createdNew;

                try
                {
                    //MetaLog?.Info("Creating logger's event ", eventName);
                    _loggersEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName, out createdNew);
                    
                    // If createdNew = false, it probably means something bad but I'm not sure it's always bad. 
                    // For example, if the file was closed and reopened maybe we'll get the same event object again.

                    step = "registering callback";
                    //MetaLog?.Info("Calling RegisterWaitForSingleObject");
                    _loggersRegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject(_loggersEvent, LoggersEventHandler, null, -1, false);

                    // Set the security so any viewer can set/signal the event.

                    step = "setting named event security";
                    SetEventSecurity();

                    //Debug.Print("Logger's named event is ready: {0}", eventName);
                }
                catch (Exception ex)
                {
                    string msg = "Exception at step '" + step + "': " + ex.Message;

                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;

                        if (!string.IsNullOrEmpty(ex.Message))
                        {
                            msg += "\n" + ex.Message;
                        }
                    }

                    //MetaLog?.Info(msg);
                    Debug.Print(msg);
                    Logger.EventLogging.Log(msg, Logger.EventLogging.NonFatalExceptionInLogger);
                    //Close();
                }
            }
        }

        private void SetEventSecurity()
        {
            EventWaitHandleSecurity security = new EventWaitHandleSecurity();
            SecurityIdentifier authenticatedUsers = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);

            security.AddAccessRule(new EventWaitHandleAccessRule(authenticatedUsers, EventWaitHandleRights.Modify, AccessControlType.Allow));
            security.AddAccessRule(new EventWaitHandleAccessRule(authenticatedUsers, EventWaitHandleRights.ReadPermissions, AccessControlType.Allow));

            _loggersEvent.SetAccessControl(security);
        }

        // Called on an arbitrary thread when another process (presumably a log viewer)
        // signals the _loggersEvent to notify the logger that a viewer has either created
        // or released one of the viewer's events.  
        private void LoggersEventHandler(object state, bool timedOut)
        {
            //Debug.Print("The logger's named event was signaled.");
            //MetaLog?.Info("The logger's named event was signaled, timedOut = ", timedOut);

            // The _eventsLock protects _viewerEventNames, _viewerEvents, 
            // and other fields related to the named events feature.

            lock (_eventsLock)
            {
                //MetaLog?.Info("_eventsLock acquired.");

                // if _waitingToCheckViewers, then this method is already running in another
                // thread which will check for viewers within 500ms so we can skip it.

                if (!_waitingToCheckViewers)
                {
                    DateTime utcNow = DateTime.UtcNow;

                    if (utcNow >= _deferCheckingOnViewersUntilTime)
                    {
                        // It's been at least 500ms since the last call to CheckForViewers() so go ahead and call it.
                        CheckForViewers();
                    }
                    else
                    {
                        // Wait until _deferCheckingOnViewersUntilTime to call to CheckForViewers().
                        // Monitor.Wait() will unlock _eventsLock while waiting so other threads can acquire 
                        // the lock, check _waitingToCheckViewers, and return without calling CheckForViewers()
                        // since we're going to call it when the Wait() returns.

                        _waitingToCheckViewers = true;
                        Monitor.Wait(_eventsLock, _deferCheckingOnViewersUntilTime - utcNow);
                        CheckForViewers();
                        _waitingToCheckViewers = false;
                    }

                    // This will prevent CheckForViewers() from being called again for at least 500ms.
                    _deferCheckingOnViewersUntilTime = DateTime.UtcNow.AddMilliseconds(500);

                    // If we're still holding handles to any viewer events in 30 minutes, re-check them in 
                    // case the viewers abandoned them without notifying us by setting _loggersEvent.
                    _recheckViewersAfterTime = DateTime.UtcNow.AddMinutes(30);
                }
            } // lock
        }

        // Called under lock to check for the existence of named system events created viewers and populate
        // _viewerEvents with an EventWaitHandle for each named event found.  
        private void CheckForViewers()
        {
            //Debug.Print("CheckForViewers");
            //MetaLog?.Info("CheckForViewers");

            ClearViewerEvents();

            // At this point _viewerEventNames is populated and _viewerEvents exists but is empty.
            // Attempt to open any viewer events that might exist and add them to _viewerEvents.

            foreach (string eventName in _viewerEventNames)
            {
                // Possibly adds to _viewerEvents.
                TryOpenViewerEvent(eventName);
            }

            if (_viewerEvents.Any())
            {
                // Make sure the _internalEvent and the BackgroundEventSignaler thread exist so the viewers can be notified.

                if (_internalEvent == null)
                {
                    _internalEvent = new AutoResetEvent(false);
                    ThreadPool.QueueUserWorkItem(BackgroundEventSignaler);

                    // Handle the ProcessExit event so we can signal the viewers just before the process ends.
                    AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
                }
            }
            else
            {
                // We don't need the _internalEvent and the BackgroundEventSignaler thread because there are no viewers to notify.

                if (_internalEvent != null)
                {
                    AutoResetEvent temp = _internalEvent;
                    _internalEvent = null;
                    temp.Set();

                    AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
                }
            }
        }

        private void TryOpenViewerEvent(string eventName)
        {
            try
            {
                // OpenExisting() throws a WaitHandleCannotBeOpenedException if no event named eventName exists.

#if NET35 || NET45
                EventWaitHandle eventHandle = EventWaitHandle.OpenExisting(eventName, EventWaitHandleRights.Modify);
#elif NETCOREAPP3_1
                EventWaitHandle eventHandle = EventWaitHandle.OpenExisting(eventName);
#endif

                // Getting here without an exception means we found a viewer event that a viewer is waiting on.
                // Keep it so other code can signal it when new messages are logged.
                _viewerEvents.Add(eventHandle);

                //Debug.Print("Found event named  {0}", eventName);
                //MetaLog?.Info("Found event named  ", eventName);
            }
            catch (WaitHandleCannotBeOpenedException ex) when (ex.Message == "No handle of the given name exists.")
            {
                // This is expected to happen a lot because it's extremely rare for 10 viewers to be watching the file.
            }
            catch (Exception ex)
            {
                // This is unexpected.
                Debug.Print("Unexpected exception opening named event '{0}', {1}: {2}", eventName, ex.GetType(), ex.Message);
                //MetaLog?.Info("Error opening event named  ", eventName, ": ", ex.GetType(), ", ", ex.Message);
            }
        }

        // Creates _viewerEvents on the first call.  On subsequent calls, closes all items in _viewerEvents so
        // the OS can clean up any that are no longer needed, and clears _viewEvents.
        private void ClearViewerEvents()
        {
            if (_viewerEventNames == null)
            {
                // This is the first time the _loggersEvent has been signaled.
                // Generate the list of event names the viewer(s) can create.

                _viewerEventNames = new string[_maxViewerEvents];
                _viewerEvents = new List<EventWaitHandle>(_maxViewerEvents);

                for (int i = 0; i < _maxViewerEvents; ++i)
                {
                    _viewerEventNames[i] = "Global\\TX" + i + '-' + _fileGuidString;
                }
            }
            else
            {
                // Close the viewer event handles we are holding so the OS can dispose of any that are no longer held by a viewer process.
                // Other code will attempt to reopen all 10 of the named events that viewers might still be using to determine which ones are in use.

                foreach (EventWaitHandle evt in _viewerEvents)
                {
                    evt.Close();
                }

                _viewerEvents.Clear();
            }
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            // Signal the viewers in case any messages have been logged since the last signal.

            lock (_eventsLock)
            {
                SignalTheViewers();
            }
        }

        // This runs a loop in a worker thread.  Each iteration of the loop waits for
        // _internalEvent to be set, which is the signal for this method to signal the
        // viewers.  By using a worker thread for this the customer's thread isn't
        // affected and we don't have to be too paranoid about performance in here.
        // The loop also periodically checks if any viewers still exist.
        private void BackgroundEventSignaler(object unused)
        {
            try
            {
                // _internalEvent can be set to null by another thread when this thread doesn't hold the _eventsLock, including
                // while waiting in _internalEvent.WaitOne().  However, temp can't be changed by another thread.

                AutoResetEvent temp = _internalEvent;

                while (temp != null)
                {
                    TimeSpan waitUntilRecheck = _recheckViewersAfterTime - DateTime.UtcNow;

                    if (waitUntilRecheck < TimeSpan.Zero)
                    {
                        waitUntilRecheck = TimeSpan.Zero;
                    }

                    // Wait for the _internalEvent (i.e. temp) to be signaled or until it's time to check for abandoned viewer events.
                    // We mustn't hold the lock while waiting.

                    bool signaled = temp.WaitOne(waitUntilRecheck);

                    lock (_eventsLock)
                    {
                        if (_viewerEvents.Any())
                        {
                            if (signaled)
                            {
                                // Put a little delay to throttle the rate at which we signal the viewers in case the logger is generating messages very quickly.
                                Thread.Sleep(10);

                                // The _internalEvent (i.e. temp) could have been set many times while we were sleeping.  Reset it so we'll actually wait
                                // when we get back to calling WaitOne().  
                                temp.Reset();

                                SignalTheViewers();
                            }

                            if (DateTime.UtcNow >= _recheckViewersAfterTime)
                            {
                                // We may be holding and signaling viewer events that have been abandoned
                                // by the viewers.  Signal _loggersEvent to trigger the handler that will check 
                                // if we still need to hold/signal the viewer events.

                                //Debug.Print("Signaling _loggersEvent to force re-check of viewer's events");
                                _loggersEvent.Set();

                                // Don't recheck again for at least 30 minutes.

                                _recheckViewersAfterTime = DateTime.UtcNow.AddMinutes(30);
                            }
                        }
                        else
                        {
                            // This is the condition intended to terminate this thread.
                            break;
                        }

                    } // lock

                    // Once outside the lock, _internalEvent can be changed by another thread.  However temp
                    // is "stable" once we set it.

                    temp = _internalEvent;
                }
            }
            catch (Exception ex)
            {
                Debug.Print("BackgroundEventSignaler terminating due to {0}: {1}", ex.GetType(), ex.Message);
                //MetaLog?.InfoFormat("BackgroundEventSignaler terminating due to {0}: {1}", ex.GetType(), ex.Message);
            }

            //Debug.Print("BackgroundEventSignaler terminating.");
            //MetaLog?.Info("BackgroundEventSignaler terminating.");
        }
    }
}
