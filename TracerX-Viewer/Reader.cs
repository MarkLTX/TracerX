using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;

namespace TracerX.Viewer
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

        // File size in bytes.
        public long Size;

        // Approximate number of bytes read from the file, to report percent loaded.
        public long BytesRead;

        // Bitmap indicating all TraceLevels found in the file.
        public TraceLevel LevelsFound;

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

        public BinaryReader _fileReader;

        // The session currently being read.  
        public Session CurrentSession;

        // The file position of the next session in the log file.  This is used
        // by NextSession() to read the session preamble that should be found there.
        private long _nextSessionPos;

        // This is added to the thread IDs of the current session so they are unique
        // from the thread IDs of all previous sessions.
        private int _maxThreadIDFromPrevSession;

        // If tempFile != originalFile, tempFile is a temporary copy of originalFile.
        // When OpenLogFile is called, tempFile is opened.
        public Reader(string originalFile, string tempFile)
        {
            OriginalFile = originalFile;
            TempFile = tempFile;
            CurrentFile = tempFile;
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
        }

        // Open the file, read the format version and password hash if there is one.
        // Return false if an error occurs, such as encountering an unsupported file version.
        public bool OpenLogFile()
        {
            InternalOpen(CurrentFile);

            if (_fileReader != null)
            {
                try
                {
                    FormatVersion = _fileReader.ReadInt32();

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
                        _nextSessionPos = _fileReader.BaseStream.Position;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading log file '" + CurrentFile + "':\n\n" + ex.ToString());
                    _fileReader = null;
                }
            }
            return _fileReader != null;
        }

        private bool CheckPassword()
        {
            bool hasPassword = _fileReader.ReadBoolean();
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

        //// Open the file, read the format version and preamble.
        //// Return false if an error occurs, such as encountering an unsupported file version.
        //public bool OpenLogFile()
        //{
        //    InternalOpen(CurrentFile);

        //    if (_fileReader != null)
        //    {
        //        try
        //        {
        //            FormatVersion = _fileReader.ReadInt32();

        //            if (FormatVersion > _maxVersion || FormatVersion < _minVersion)
        //            {
        //                MessageBox.Show("The file has a format version of " + FormatVersion + ".  This program only supports format version " + _maxVersion + ".");
        //                CloseLogFile();
        //            }
        //            else if (FormatVersion == 4 && !PromptUserForPassword())
        //            {
        //                CloseLogFile();
        //            }
        //            else if (FormatVersion >= 5 && _fileReader.ReadBoolean() && !PromptUserForPassword())
        //            {
        //                CloseLogFile();
        //            }
        //            else
        //            {
        //                _nextSessionPos = _fileReader.BaseStream.Position;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Error reading log file '" + CurrentFile + "':\n\n" + ex.ToString());
        //            _fileReader = null;
        //        }
        //    }
        //    return _fileReader != null;
        //}

        private bool InternalOpen(string filename)
        {
            try
            {
                _fileReader = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

                if (_fileReader == null)
                {
                    MessageBox.Show("Could not open log file " + filename);
                }
                else
                {
                    Debug.Print("Viewer opened log file.");
                    Size = _fileReader.BaseStream.Length;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening log file '" + filename + "':\n\n" + ex.ToString());
                _fileReader = null;
            }

            return _fileReader != null;
        }

        // TODO: Store decryption info in the returned session.
        // May need to make logger require that all sessions use the
        // same password, or that encrypted files have only one session.

        public Session NextSession()
        {
            Session session = null;

            if (_nextSessionPos != 0)
            {
                // Create a Session and read the session's preamble.
                // Open the file if necessary, but leave the file
                // in the same state (open or closed) as now.

                bool alreadyOpen = _fileReader != null;
                var tempPos = _nextSessionPos;
                _nextSessionPos = 0;

                if (alreadyOpen || InternalOpen(CurrentFile))
                {
                    try
                    {
                        lock (SessionObjects.Lock)
                        {
                            session = new Session(this);
                            session.ReadPreamble(tempPos); // Exception likely here.

                            // The thread IDs in the log file start at 1 for each session, but they're really separate
                            // threads, so the IDs read from each session are incremented by max ID from the previous session.
                            // Ignore sessions with no threads/records.
                            if (CurrentSession != null && CurrentSession.MaxThreadID > 0)
                            {
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
                        _fileReader.Close();
                        _fileReader = null;
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

        //public MemoryStream DecryptedStream;

        // Called if the file has a password.  Reads the password hash from the file
        // and prompts the user to enter a matching password.
        // Returns true if the user enters the correct password.
        private bool PromptUserForPassword()
        {
            var hash = _fileReader.ReadBytes(20);
            var hint = _fileReader.ReadString();

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
            if (_fileReader != null)
            {
                Debug.Print("Viewer is closing log file.");
                _fileReader.Close();
                _fileReader = null;
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

                lock (ThreadNames.Lock)
                {
                    ThreadNames.AllThreadNames.Add(threadName);
                }
            }

            return threadName;
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

                lock (LoggerObjects.Lock)
                {
                    LoggerObjects.AllLoggers.Add(logger);
                }
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

                lock (MethodObjects.Lock)
                {
                    MethodObjects.AllMethods.Add(method);
                }
            }

            return method;
        }
    }
}
