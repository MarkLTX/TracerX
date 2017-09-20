using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;

namespace TracerX
{
    // This class reads the log file into Record objects.
    internal partial class Reader
    {
        // The minimum and maximum file format versions supported
        // by the viewer/reader.
        private const int _minVersion = 5;
        private const int _maxVersion = 8;

        public int FormatVersion { get; private set; }

        // File name passed to ctor (originalFile).
        public string OriginalFile { get; private set; }

        // File name passed to ctor (tempFile).
        public string TempFile { get; private set; }

        // Which file (OriginalFile or TempFile) to use.
        public string CurrentFile;

        // The FileGuid from the first session.  Basis of named events used by logger and LocalFileWatcher.
        public Guid FileGuid;

        // The RemoteServer to read the file from.  Null for local file system.
        public RemoteServer FileServer { get; private set; }

        // File size in bytes.
        public long InitialSize;

        // Approximate number of bytes read from the file, to report percent loaded.
        public long BytesRead;

        // Keeps track of thread IDs we have found while reading the file.  Each ReaderThreadInfo object
        // stores the "per thread" info that is not usually written to the log when a thread switch occurs.
        private Dictionary<int, ReaderThreadInfo> _foundThreadIds = new Dictionary<int, ReaderThreadInfo>();

        // When refreshing a file, the ThreadObjects from the old file are put here and reused if the same
        // thread IDs are found in the new file.  This is how filtering is persisted across refreshes.
        private Dictionary<int, ThreadObject> _oldThreadIds = new Dictionary<int, ThreadObject>();

        private Dictionary<string, ThreadName> _foundThreadNames = new Dictionary<string, ThreadName>();
        private Dictionary<string, ThreadName> _oldThreadNames = new Dictionary<string, ThreadName>();

        private Dictionary<string, LoggerObject> _foundLoggers = new Dictionary<string, LoggerObject>();
        private Dictionary<string, LoggerObject> _oldLoggers = new Dictionary<string, LoggerObject>();

        private Dictionary<string, MethodObject> _foundMethods = new Dictionary<string, MethodObject>();
        private Dictionary<string, MethodObject> _oldMethods = new Dictionary<string, MethodObject>();

        // Used for checking password.
        private static byte[] _lastHash;

        // The System.IO.BinaryReader used to read the file.
        public BinaryReader InternalReader;

        public bool IsOpen { get { return InternalReader != null; } }

        // Is it OK to leave the file open when not actively reading it?
        // (It's OK if the file was opened with FileShare.Delete.)
        public bool CanLeaveOpen { get; private set; }

        // If reading a remote file, does the remote TracerX-Service support
        // the use of a named system event for file-change notifications?
        public bool CanUseChangeEvent { get; private set; }

        // The session currently being read.  
        public Session CurrentSession;

        // The file position of the next session in the log file.  This is used
        // by NextSession() to read the session preamble that should be found there.
        private long _nextSessionPos;

        // This is added to the thread IDs of the current session so they are unique
        // from the thread IDs of all previous sessions.
        private int _maxThreadIDFromPrevSession;

        private DateTime _openTime;

        // If tempFile != originalFile, tempFile is a temporary copy of originalFile.
        // When OpenLogFile is called, tempFile is opened.
        public Reader(string originalFile, string tempFile)
        {
            OriginalFile = originalFile;
            TempFile = tempFile;
            CurrentFile = tempFile;
        }

        public Reader(RemoteServer server, string file)
        {
            // Remember the server and the file.  Reference them in Open().
            FileServer = server;
            OriginalFile = file;
            TempFile = file;
            CurrentFile = file;
        }

        public void ReuseFilters()
        {
            // Save off the old filter objects (thread IDs, thread names, methods, and loggers) and 
            // reuse any that are also found in the new file.  We don't reuse the session filter.

            lock (ThreadObjects.Lock)
            {
                foreach (ThreadObject threadObject in ThreadObjects.AllThreadObjects) _oldThreadIds.Add(threadObject.Id, threadObject);
            }

            lock (ThreadNames.Lock)
            {
                foreach (ThreadName threadName in ThreadNames.AllThreadNames) _oldThreadNames.Add(threadName.Name, threadName);
            }

            lock (LoggerObjects.Lock)
            {
                foreach (LoggerObject logger in LoggerObjects.AllLoggers) _oldLoggers.Add(logger.Name, logger);
            }

            lock (MethodObjects.Lock)
            {
                foreach (MethodObject method in MethodObjects.AllMethods) _oldMethods.Add(method.Name, method);
            }

            //lock (TraceLevelObjects.Lock)
            //{
            //    foreach (TraceLevelObject level in TraceLevelObjects.AllTraceLevels) _oldTraceLevels.Add(level.TLevel, level);
            //}
        }

        // Open the file, read the format version and password hash if there is one.
        // Return false if an error occurs, such as encountering an unsupported file version.
        public bool OpenLogFile()
        {
            _openTime = DateTime.Now;
            InternalOpen();

            if (IsOpen)
            {
                try
                {
                    FormatVersion = InternalReader.ReadInt32();

                    if (FormatVersion > _maxVersion || FormatVersion < _minVersion)
                    {
                        MessageBox.Show("The file has a format version of " + FormatVersion + ".  This version of the TracerX viewer only supports file format versions " + _minVersion + " through " + _maxVersion + ".");
                        CloseLogFile();
                    }
                    else if (!CheckPassword())
                    {
                        CloseLogFile();
                    }
                    else
                    {
                        _nextSessionPos = InternalReader.BaseStream.Position;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading log file '" + CurrentFile + "':\n\n" + ex.ToString());
                    InternalReader = null;
                }
            }

            return IsOpen;
        }

        private bool CheckPassword()
        {
            bool hasPassword = InternalReader.ReadBoolean();
            bool result = true;

            if (hasPassword)
            {
                if (FormatVersion < 8)
                {
                    MessageBox.Show("The file is password-protected and has a format version of " + FormatVersion + ".  This version of the TracerX viewer only supports password-protection in file format version 8 or above.");
                    result = false;
                }
                else
                {
                    result = PromptUserForPassword();
                }
            }
            else
            {
                Key = null;
                _lastHash = null;
            }

            return result;
        }

        // This opens the file and sets this.InitialSize.
        // Does no reads.
        private bool InternalOpen()
        {
            string error = "";

            try
            {
                if (FileServer == null)
                {
                    // The file path is relative to the local host (i.e. just another file).

                    error = "The file could not be opened.";
                    InternalReader = new BinaryReader(new FileStream(CurrentFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete));

                    // FileShare.Delete was added to the above call so we can keep the file open
                    // without preventing the logger from renaming or replacing it.

                    CanLeaveOpen = true;
                    CanUseChangeEvent = true;
                }
                else
                {
                    // The file is on a remote server that should be running the TracerX-Service.

                    error = "An error occurred on server " + FileServer.HostAddress + " when trying to open the file.";
                    RemoteFileStream rfs = FileServer.GetRemoteFileStream(CurrentFile);
                    InternalReader = new BinaryReader(rfs);

                    // The TracerX-Service began using FileShare.Delete in version 2.

                    CanLeaveOpen =  rfs.ServiceVersion >= 2;
                    CanUseChangeEvent = rfs.ServiceVersion >= 3;
                }

                if (InternalReader == null)
                {
                    MessageBox.Show("Could not open log file " + CurrentFile);
                }
                else
                {
                    Debug.Print("Viewer opened log file.");
                    InitialSize = InternalReader.BaseStream.Length;
                }
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                // This represents an unhandled exception in the remote service method.

                if (ex.Detail.Type == "IOException" && ex.Detail.Message.Contains("impersonation level"))
                {
                    // Have seen this message: "Either a required impersonation level was not provided, or the provided impersonation level is invalid."

                    error += "  If the error is related to impersonating the current user, "
                        + "it might be fixed by disabling client impersonation in the TracerX service, "
                        + "or changing the Local Security Policy on the server to grant the following rights to the service account, "
                        + "or using a service account that already has these rights (e.g. Local System)."
                        + "\n\n"
                        + "  \"Impersonate a client after authentication\"\n"
                        + "  \"Create global objects\""
                        + "\n\n"
                        + "This is the error that occurred..."
                        + "\n\n"
                        + Program.GetNestedDetails(ex.Detail);
                }
                else
                {
                    error += "\n\n" + Program.GetNestedDetails(ex.Detail);
                }

                MainForm.ShowMessageBox(error);
                InternalReader = null;
            }
            catch (FaultException fe)
            {
                // This is how the service returns explicit error messages.
                error += "\n\n" + fe.Message;
                MainForm.ShowMessageBox(error);
                InternalReader = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening log file '" + CurrentFile + "':\n\n" + ex.ToString());
                InternalReader = null;
            }

            return InternalReader != null;
        }

        /// <summary>
        /// If the file was just opened (i.e. we're about to read the first session) or we've read all records in the current session, then this ...
        ///     Opens the file (unless it's already open).   
        ///     Attempts to read the next session's preamble from the file.  
        ///     Closes the file (unless it was already open).
        /// </summary>
        public Session NextSession()
        {
            Session session = null;

            if (_nextSessionPos != 0)
            {
                // Create a Session and read the session's preamble.
                // Open the file if necessary, but leave the file
                // in the same state (open or closed) as now.

                bool alreadyOpen = InternalReader != null;
                var tempPos = _nextSessionPos;
                _nextSessionPos = 0;

                if (alreadyOpen || InternalOpen())
                {
                    try
                    {
                        lock (SessionObjects.Lock)
                        {
                            session = new Session(this);
                            session.ReadPreamble(tempPos); // Exception likely here.

                            if (CurrentSession == null)
                            {
                                // We just parsed the preamble of the first session.  Save its FileGuid to pass to the LocalFileWatcher class 
                                // which uses it to determine the names of the global system events used by the logger to notify it when 
                                // new messages are logged.
                                FileGuid = session.FileGuid;
                            }
                            else if (CurrentSession.MaxThreadID > 0)
                            {
                                // The thread IDs in the log file start at 1 for each session, but they're really separate
                                // threads, so the IDs read from each session are incremented by max ID from the previous session.
                                // Ignore sessions with no threads/records.

                                _maxThreadIDFromPrevSession = CurrentSession.MaxThreadID;
                            }

                            CurrentSession = session;
                            CurrentSession.Index = SessionObjects.AllSessionObjects.Count;
                            SessionObjects.AllSessionObjects.Add(CurrentSession);
                            CurrentSession.Name = SessionObjects.AllSessionObjects.Count.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        string msg = string.Format("Error opening log file session {0}.  File '{1}' may be corrupt.\n\n {2}", (SessionObjects.AllSessionObjects.Count + 1), CurrentFile, ex.ToString());
                        MessageBox.Show(msg, "TracerX-Viewer");
                        session = null;
                    }

                    if (!alreadyOpen)
                    {
                        InternalReader.Close();
                        InternalReader = null;
                    }
                }
            }

            return session;
        }

        public static byte[] Key
        {
            get;
            private set;
        }

        // Called if the file has a password.  Reads the password hash from the file
        // and prompts the user to enter a matching password.
        // Returns true if the user enters the correct password.
        private bool PromptUserForPassword()
        {
            var hash = InternalReader.ReadBytes(20);
            var hint = InternalReader.ReadString();

            if (_lastHash != null && hash.SequenceEqual(_lastHash))
            {
                // Either the same file is being loaded again (refreshed), 
                // or a different file has the same password.  Either way,
                // don't ask for the same password again.
                return true;
            }
            else
            {
                PasswordDialog dlg = new PasswordDialog(hash, hint);
                DialogResult result = dlg.ShowDialog();

                if (result == DialogResult.OK)
                {
                    _lastHash = hash;
                    Key = dlg.EncryptionKey;
                    return true;
                }
                else
                {
                    _lastHash = null;
                    Key = null;
                    return false;
                }
            }
        }

        public void CloseLogFile()
        {
            if (InternalReader != null)
            {
                Debug.Print("Viewer is closing log file.");
                InternalReader.Close();
                InternalReader = null;
            }
        }

        private ThreadName FindOrCreateThreadName(string name)
        {
            ThreadName threadName = null;

            if (!_foundThreadNames.TryGetValue(name, out threadName))
            {
                if (!_oldThreadNames.TryGetValue(name, out threadName))
                {
                    threadName = new ThreadName();
                    threadName.Name = name;
                }

                _foundThreadNames.Add(name, threadName);
                ThreadNames.Add(threadName);
            }

            return threadName;
        }

        // Gets or makes the LoggerObject with the specified name.
        private TraceLevelObject GetTraceLevel(TraceLevel tl)
        {
            lock (TraceLevelObjects.Lock)
            {
                return TraceLevelObjects.AllTraceLevels[tl];
            }
        }

        // Gets or makes the LoggerObject with the specified name.
        private LoggerObject GetLogger(string loggerName)
        {
            LoggerObject logger;

            if (!_foundLoggers.TryGetValue(loggerName, out logger))
            {
                if (!_oldLoggers.TryGetValue(loggerName, out logger))
                {
                    logger = new LoggerObject();
                    logger.Name = loggerName;
                }

                _foundLoggers.Add(loggerName, logger);
                LoggerObjects.Add(logger);
            }

            return logger;
        }

        // Gets or makes the MethodObject with the specified name.
        private MethodObject GetMethod(string methodName)
        {
            MethodObject method;

            if (!_foundMethods.TryGetValue(methodName, out method))
            {
                if (!_oldMethods.TryGetValue(methodName, out method))
                {
                    method = new MethodObject();
                    method.Name = methodName;
                }

                _foundMethods.Add(methodName, method);
                MethodObjects.Add(method);
            }

            return method;
        }
    }
}
