using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using System.Security.Cryptography;
using System.Linq;

namespace TracerX
{
    /// <summary>
    /// Methods and configuration for logging to the full-featured binary file supported by the viewer.
    /// </summary>
    /// <remarks>
    /// Many of the properties cannot be changed while the file is open.  The Opening event is the last
    /// chance to set such properties.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public sealed class BinaryFile : FileBase
    {

        #region Public

        public BinaryFile()
            : base(".tx1")
        {
        }

        /// <summary>
        /// Opens the log file using the current values of various properties.  Raises the Opening and Opened events.
        /// </summary>
        public override bool Open()
        {
            bool result = base.Open();

            if (result)
            {
                if (AddToListOfRecentlyCreatedFiles)
                {
                    AddToRecentlyCreated();
                }
            }

            return result;
        }

        /// <summary>
        /// Closes the log file. Raises the Closing and Closed events.  If you intend to reopen the file, 
        /// consider calling Roll() instead (it's thread safe).
        /// </summary>
        public override void Close()
        {
            lock (_fileLocker)
            {
                if (_logfile != null) // Same as IsOpen
                {
                    OnClosing();
                    _logfile.Close();

                    // Reset most fields except those set by end-user.
                    _logfile = null;
                    _curTime = DateTime.MinValue;
                    _lastThread = null;
                    _everWrapped = false;
                    _curBlock = 1;
                    _lastBlock = 0;
                    _blockSize = 0;
                    _bytesInBlock = 0;
                    _curBlockPosition = 0;
                    _lastBlockPosition = 0;
                    _maxBlockPosition = 0;
                    _staleTimestampTimer = null;
                    _timerIsTicking = false;
                    _fileHandle = null;

                    base.Close();

                    OnClosed();
                }
            }
        }
        /// <summary>
        /// The format version of the binary file created by this assembly, which sometimes changes with new releases of TracerX
        /// and also depending on which features are used.
        /// </summary>
        public int FormatVersion
        {
            get
            {
                // The only difference between 6 and 7 is that in version 6,
                // the max file size in the preamble is in Mb, and in version 7,
                // it's in Kb.

                // The difference between 7 and 8 is that in 8, _hasPassword is true
                // and therefore the SHA1 hash of the password is included in the preamble.

                lock (_fileLocker)
                {
                    if (_hasPassword)
                    {
                        return 8;
                        // Viewer will expect file size in Kb and the
                        // password hash to use SHA1.
                    }
                    else if (UseKbForSize)
                    {
                        return 7;
                        // Viewer will expect file size in Kb.
                    }
                    else
                    {
                        return 6;
                        // Viewer will expect file size in Mb.
                    }
                }
            }
        }

        /// <summary>
        /// Gets the unique GUID assigned to the file when it is opened.
        /// </summary>
        public Guid FileGuid { get; private set; }

        /// <summary>
        /// Is the output file currently open?
        /// </summary>
        public override bool IsOpen { get { return _logfile != null; } }

        /// <summary>
        /// Has circular logging started (not necessarily wrapped)?
        /// </summary>
        public override bool CircularStarted { get { return _curBlock > 1; } }

        /// <summary>
        /// Returns true if the file size has exceeded the max size.  Once this becomes
        /// true, future output replaces old output.
        /// </summary>
        public override bool Wrapped { get { return _everWrapped; } }

        protected override Stream BaseStream
        {
            get { return _logfile.BaseStream; }
        }

        /// <summary>
        /// Current block number (used only by test drivers).
        /// </summary>
        public uint CurrentBlock { get { return _curBlock; } }

        /// <summary>
        /// If a password is set before the file is opened, the viewer will
        /// require the user to enter the same password to open and decrypt the file.
        /// This only applies when creating a new file, not
        /// when appending to an existing file.
        /// Only a hash of the specified password is stored in the file.
        /// </summary>
        public string Password
        {
            set
            {
                lock (_fileLocker)
                {
                    if (!IsOpen)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            _hasPassword = false;
                            _sha1PasswordHash = null;
                        }
                        else
                        {
                            // The sha1 hash is stored in the file so we can determine
                            // if the user of the viewer knows the password without having
                            // to store the password.
                            SHA1 sha1 = new SHA1CryptoServiceProvider();
                            byte[] pwBytes = System.Text.Encoding.Unicode.GetBytes(value);
                            _sha1PasswordHash = sha1.ComputeHash(pwBytes);

                            // Create the encryption key from the password and 'salt'.  The salt
                            // can be any byte array, but it has to be something we can acquire
                            // again in the viewer.
                            Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes(value, _sha1PasswordHash);
                            _encryptionKey = keyGenerator.GetBytes(16);

                            _hasPassword = true;
                        }
                    }
                }
            }
        }

        private bool _hasPassword;
        private byte[] _sha1PasswordHash;
        private byte[] _encryptionKey;
        private Encryptor _encryptor;

        /// <summary>
        /// If you set Password, you may also want to supply an optional password hint to be displayed
        /// by the viewer when it prompts the user for the pasword.  This is reset to null when the file is opened.
        /// </summary>
        public string PasswordHint
        {
            get { return _passwordHint;}

            set
            {
                lock (_fileLocker)
                {
                    if (!IsOpen)
                    {
                        if (value != null && value.Length > 1000)
                        {
                            throw new ArgumentOutOfRangeException("The maximum length of the PasswordHint is 1000 characters.");
                        }
                        else
                        {
                            _passwordHint = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If true (default), the log file will appear in the list of recently
        /// created files on the host computer.
        /// </summary>
        public bool AddToListOfRecentlyCreatedFiles
        {
            get { return _addToListOfRecentlyCreatedFiles; }
            set { _addToListOfRecentlyCreatedFiles = value; }
        }
        private bool _addToListOfRecentlyCreatedFiles = true;

        #endregion

        #region Data members

        // The output log file.  Null until OpenLog() succeeds.
        // User output goes through this object.
        private BinaryWriter _logfile;

        // Backer for PasswordHint property.
        private string _passwordHint;

        // Time of last logged line;
        private DateTime _curTime = DateTime.MinValue;

        // The thread the last line was written by.
        private ThreadData _lastThread;

        // Have we ever wrapped?
        private bool _everWrapped;

        // Current block being written. 1 means non-circular area.
        // Incremented whenever a new block is started.
        private uint _curBlock = 1;

        // The block the previous line was written to (copied from _curBlock).
        // 0 means nothing has been written.
        private uint _lastBlock = 0;

        // Minimum number of bytes written to each circular block.
        private uint _blockSize;

        // Number of bytes written to the current block.
        private uint _bytesInBlock;

        // File position of the currently active block.
        private long _curBlockPosition = 0;

        // File position of the previously active block.
        private long _lastBlockPosition = 0;

        // File position of the block that exends past max file size.
        // 0 means there is no such block.
        // When this file position is overwritten, the file is truncated since
        // all subsequent data in the file is ignored by the viewer.
        private long _maxBlockPosition = 0;

        // Used to ensure the file's LastWriteTime gets changed soon after every line
        // is written in the circular log.
        private Timer _staleTimestampTimer;

        // Indicates if _staleTimestampTimer is active or not.
        private bool _timerIsTicking;

        // Used to update the file's LastWriteTime.
        private SafeFileHandle _fileHandle;

        #endregion

        #region PrivateMethods

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetFileTime(SafeHandle hFile, IntPtr lpCreationTime, IntPtr lpLastAccessTime, ref long lpLastWriteTime);

        #region Open log

        // This either opens the originally specified log file, opens an
        // alternate log file, or throws an exception.
        protected override void InternalOpen()
        {
            // Use this to generate alternate file names A-Z if file can't be opened.
            char c = 'A';
            string renamedFile = null;
            FileStream fileStream = null;
            bool appending = false;
            string simpleName = _logFileName; // no extension, no (A), no _00.

            while (fileStream == null)
            {
                try
                {
                    FileInfo outFile = new FileInfo(FullPath);

                    if (outFile.Exists)
                    {
                        if (outFile.Length < AppendIfSmallerThanMb << _shift)
                        {
                            // Open in append mode.
                            // If the file is in use, this throws an exception.
                            fileStream = new FileStream(FullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);

                            // Check the format version of the existing file.
                            var reader = new BinaryReader(fileStream);
                            string msg = null;
                            fileStream.Position = 0;
                            int oldVersion = reader.ReadInt32();

                            if (oldVersion == FormatVersion)
                            {
                                // Verify that the same password is being used.
                                bool oldHasPassword = reader.ReadBoolean();

                                if (oldHasPassword && _hasPassword)
                                {
                                    // Check that passwords (actually the hashes) match.
                                    byte[] oldHash = reader.ReadBytes(20);

                                    if (!oldHash.SequenceEqual(_sha1PasswordHash))
                                    {
                                        // We don't allow different sessions to have different passwords.  
                                        msg = string.Format("TracerX could not append to the existing log file\n{0}\nbecause it has a different password than the current file.", FullPath);
                                        Logger.EventLogging.Log(msg, Logger.EventLogging.AppendPasswordConflict);
                                    }
                                }
                                else if (!oldHasPassword && !_hasPassword)
                                {
                                    // It's OK if both don't have passwords.
                                    msg = null;
                                }
                                else
                                {
                                    // We don't allow different sessions to have different passwords.  
                                    msg = string.Format("TracerX could not append to the existing log file\n{0}\nbecause it has a different password than the current file.", FullPath);
                                    Logger.EventLogging.Log(msg, Logger.EventLogging.AppendPasswordConflict);
                                }
                            }
                            else
                            {
                                // We don't mix format versions in one file.  
                                msg = string.Format("TracerX could not append to the existing log file\n{0}\nbecause its format version ({1}) was not equal to the current format version ({2}).", FullPath, oldVersion, FormatVersion);
                                Logger.EventLogging.Log(msg, Logger.EventLogging.AppendVersionConflict);
                            }

                            if (msg == null)
                            {
                                // Success!
                                // We'll start writing at the end.
                                fileStream.Position = fileStream.Length;
                                _openSize = fileStream.Length;
                                appending = true;
                            }
                            else
                            {
                                // Failure! Turn Append off and try again.
                                AppendIfSmallerThanMb = 0;
                                fileStream.Dispose();
                                fileStream = null;
                                continue;
                            }
                        }
                        else
                        {
                            if (Archives > 0)
                            {
                                renamedFile = FullPath + ".tempname";
                                if (File.Exists(renamedFile)) File.Delete(renamedFile);

                                // DO NOT use outFile.Exists or outFile.Move() in here.
                                for (int attempts = 3; attempts > 0 && File.Exists(FullPath); --attempts)
                                {
                                    try
                                    {
                                        // If the file is in use (including by the viewer), this throws an exception.
                                        File.Move(FullPath, renamedFile);
                                    }
                                    catch (Exception ex)
                                    {
                                        if (attempts <= 1)
                                        {
                                            // That was the last try.
                                            throw;
                                        }
                                        else
                                        {
                                            // Give whoever has the file open (possibly the viewer) a
                                            // chance to close it.
                                            Thread.Sleep(100);
                                        }
                                    }
                                }
                            }

                            fileStream = new FileStream(FullPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                        }
                    }
                    else
                    {
                        // If the file is in use, this throws an exception.
                        fileStream = new FileStream(FullPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    }
                }
                catch (System.IO.IOException ex)
                {
                    // File is probably in use, try next alternate name.
                    if (c > 'Z')
                    {
                        // That was the last chance.  Rethrow the exception to
                        // end the loop and cause the exception to be logged.
                        throw;
                    }
                    else
                    {
                        // Try the next alternative file name, up to Z.
                        // Changing _logFileName also changes Name and FullName.
                        // Note that _logFileName has no extension or _00.

                        Debug.Print("Opening alternate file name due to exception:\n" + ex.ToString());

                        _logFileName = string.Format("{0}({1})", simpleName, c);
                        ++c;
                        renamedFile = null;
                        continue;
                    }
                }
            } // while

            // Use an EncoderReplacementFallback to replace any invalid UTF-16 chars
            // found in logged strings with '?' (System.String uses UTF-16 internally).
            var EncoderFallback = new EncoderReplacementFallback("?");
            var utf8WithFallback = Encoding.GetEncoding("UTF-8", EncoderFallback, new DecoderExceptionFallback());

            _logfile = new BinaryWriter(fileStream, utf8WithFallback);
            _fileHandle = fileStream.SafeFileHandle;
            _maxFilePosition = _openSize + (MaxSizeMb << _shift);
            _openTimeUtc = DateTime.UtcNow;

            if (CircularStartDelaySeconds == 0)
            {
                _circularStartTime = DateTime.MaxValue;
            }
            else
            {
                _circularStartTime = _openTimeUtc.AddSeconds(CircularStartDelaySeconds);
            }

            WritePreamble(appending);
            ++CurrentFile;

            if (_hasPassword)
            {
                _encryptor = new Encryptor(_logfile, _encryptionKey);
            }

            if (this == Logger.DefaultBinaryFile) Logger.StandardData.LogEnvironmentInfo();
            ManageArchives(renamedFile);
        }

        // Add the log file path to the list of files persisted for the viewer to read.
        private void AddToRecentlyCreated()
        {
            try
            {
                string listFile = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "TracerX\\RecentlyCreated.txt"
                    );
                string[] files;

                // First read the existing list of files.
                try
                {
                    files = File.ReadAllLines(listFile);
                }
                catch (Exception)
                {
                    // Most likely, the file doesn't exist.
                    // Make sure the directory exists so we can create the file there.

                    files = null;
                    string dir = Path.GetDirectoryName(listFile);
                    if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                }

                if (files == null)
                {
                    // No lines read, so just create the file with the current
                    // FullPath as its only content.
                    File.WriteAllText(listFile, FullPath);
                }
                else
                {
                    // Overwrite the file, putting the most recent file name (FullPath) at the top.
                    using (StreamWriter writer = new StreamWriter(listFile, false))
                    {
                        writer.WriteLine(FullPath);

                        // Write up to 8 of the previously read file names, omitting
                        // any that match the file name we just wrote (i.e. prevent duplicates).
                        // Thus, the file will contain at most 9 lines.
                        int i = 0;
                        foreach (string filename in files)
                        {
                            if (!string.IsNullOrEmpty(filename) && !string.Equals(filename, FullPath, StringComparison.InvariantCultureIgnoreCase))
                            {
                                writer.WriteLine(filename);
                                if (i++ == 8) break;
                            }
                        }
                    }
                }

                // Now attempt to grant all users read and write permission.
                // First get a FileSecurity object that represents the 
                // current security settings.
                FileSecurity fSecurity = File.GetAccessControl(listFile);

                var sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);

                // Add the FileSystemAccessRule to the security settings. 
                fSecurity.AddAccessRule(new FileSystemAccessRule(sid,
                    FileSystemRights.FullControl, AccessControlType.Allow));

                // Set the new access settings.
                File.SetAccessControl(listFile, fSecurity);
            }
            catch (Exception)
            {
                // Nothing to do, really.
                System.Diagnostics.Debug.Print("Exception in AddToRecentlyCreated.");
            }
        }

        //private void LogStandardData() {
        //    Logger Log = Logger.StandardData;

        //    using (Log.InfoCall()) {
        //        Assembly entryAssembly = Assembly.GetEntryAssembly();

        //        if (entryAssembly == null)
        //        {
        //            Log.Info("Assembly.GetEntryAssembly() returned null.");
        //        }
        //        else
        //        {
        //            Log.Info("EntryAssembly.Location = ", entryAssembly.Location);
        //            Log.Info("EntryAssembly.FullName = ", entryAssembly.FullName); // Includes assembly version.

        //            try
        //            {
        //                // Try to get the file version.
        //                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(entryAssembly.Location);
        //                Log.Info("FileVersionInfo.FileVersion = ", fvi.FileVersion);
        //                Log.Info("FileVersionInfo.ProductVersion = ", fvi.ProductVersion);
        //            }
        //            catch (Exception)
        //            {
        //            }
        //        }

        //        try {
        //            Log.Info("AppDomain.FriendlyName = ", AppDomain.CurrentDomain.FriendlyName);
        //            Log.Info("AppDomain.IsDefaultAppDomain = ", AppDomain.CurrentDomain.IsDefaultAppDomain());
        //            Log.Info("AppDomain.BaseDirectory = ", AppDomain.CurrentDomain.BaseDirectory);
        //        } catch (Exception) {
        //        }

        //        Log.Info("Environment.OSVersion = ", Environment.OSVersion);
        //        Log.Info("Environment.CurrentDirectory = ", Environment.CurrentDirectory);
        //        Log.Info("Environment.UserInteractive = ", Environment.UserInteractive);

        //        Log.Debug("Environment.CommandLine = ", Environment.CommandLine);

        //        Log.Verbose("Environment.MachineName = ", Environment.MachineName);
        //        Log.Verbose("Environment.UserDomainName = ", Environment.UserDomainName);
        //        Log.Verbose("Environment.UserName = ", Environment.UserName);
        //    }
        //}

        // Write the header/preamble information to the file.  
        private void WritePreamble(bool appending)
        {
            DateTime local = _openTimeUtc.ToLocalTime();
            Version asmVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            FileGuid = Guid.NewGuid();

            if (appending)
            {
                _logfile.Write((ushort)DataFlags.NewSession);
            }
            else
            {
                // Format version should be the first
                // item in the file so the viewer knows how to read the rest.
                _logfile.Write(FormatVersion);

                _logfile.Write(_hasPassword);

                if (_hasPassword)
                {
                    _logfile.Write(_sha1PasswordHash);
                    _sha1PasswordHash = null;

                    _logfile.Write(_passwordHint ?? "");
                    _passwordHint = null;
                }
            }

            _logfile.Write(asmVersion.ToString()); // Added in file format version 3.

            if (FormatVersion == 8)
            {
                if (UseKbForSize)
                {
                    // MaxSizeMb is in units of Kb which is what the 
                    // viewer expects for version 8.
                    _logfile.Write(MaxSizeMb);
                }
                else
                {
                    // MaxSizeMb is in units of Mb, but viewer expects Kb in version 8. 
                    _logfile.Write(MaxSizeMb << 10);
                }
            }
            else
            {
                // MaxSizeMb is in the units expected
                // by the viewer (Kb or Mb) based on FormatVersion.
                _logfile.Write(MaxSizeMb);
            }

            _logfile.Write(_maxFilePosition);       // Added in version 6.  Tells viewer where the file wraps instead of MaxMb.
            _logfile.Write(FileGuid.ToByteArray()); // Added in version 6.
            _logfile.Write(_openTimeUtc.Ticks);
            _logfile.Write(local.Ticks);
            _logfile.Write(local.IsDaylightSavingTime());
            _logfile.Write(TimeZone.CurrentTimeZone.StandardName);
            _logfile.Write(TimeZone.CurrentTimeZone.DaylightName);
            _logfile.Flush();

            // The next thing written should be first log record.
        }

        // Manages the archive files (*_01, *_02, etc.).
        // Parameter renamedFile is what the old output file was renamed 
        // to if it existed (with extension .tempname).
        // It must become the _01 file.
        // if renamedFile is null, the old output file wasn't replaced (did't
        // exist or was opened in append mode), and no renaming is necessary,
        // but we still delete files with numbers larger than Archives.
        private void ManageArchives(string renamedFile)
        {
            string bareFilePath = Path.Combine(Directory, _logFileName);
            int highestNumKept = (renamedFile == null) ? (int)Archives : (int)Archives - 1;

            // This gets the archived files in reverse order.
            // Logfiles from older versions may only have one numeric character in the filename suffix.
            string[] files = EnumOldFiles("_?*.tx1");

            if (files != null)
            {
                foreach (string oldFile in files)
                {
                    // Extract the archive number that comes after "<_logFileName>_".
                    // The number must be one or two chars or it's not one of our files.
                    string plain = Path.GetFileNameWithoutExtension(oldFile);
                    string numPart = plain.Substring(_logFileName.Length + 1);

                    if (numPart.Length == 1 || numPart.Length == 2)
                    {
                        int num;

                        if (int.TryParse(numPart, out num) && num > 0)
                        {
                            if (num > highestNumKept)
                            {
                                // The archive number is more than the user wants to keep, so delete it.
                                try
                                {
                                    File.Delete(oldFile);
                                }
                                catch (Exception ex)
                                {
                                    string msg = string.Format("An exception occurred while deleting the old log file\n{0}\n\n{1}", oldFile, ex);
                                    Logger.EventLogging.Log(msg, Logger.EventLogging.ExceptionInArchive);
                                }
                            }
                            else if (renamedFile != null)
                            {
                                // Rename (increment the file's archive number by 1).
                                TryRename(oldFile, bareFilePath, num + 1);
                            }
                        }
                    }
                }

                // Finally, rename the most recent log file, if it exists.
                if (renamedFile != null)
                {
                    TryRename(renamedFile, bareFilePath, 1);
                }

            }
        }

        #endregion

        // If DateTime.UtcNow differs from _curTime, update _curtime
        // and return true; else return false.
        private bool IsNewTime(DateTime utcNow)
        {
            if (utcNow == DateTime.MinValue)
            {
                utcNow = DateTime.UtcNow;
            }

            if (utcNow == _curTime)
            {
                return false;
            }
            else
            {
                _curTime = utcNow;
                return true;
            }
        }

        // Log the entry (start) of a method call.
        // stackEntry is not yet on the stack.
        internal void LogEntry(ThreadData threadData, StackEntry stackEntry)
        {
            // Remember the line number where the MethodEntry flag is written so
            // we can write it into the log when the method exits.
            stackEntry.EntryLine = WriteLine(DataFlags.MethodEntry, threadData, stackEntry.Logger, stackEntry.Level, DateTime.MinValue, null, false);
        }

        // Log the exit of a method call on the top of the stack for the thread.
        // stackEntry is still on the stack.        
        internal bool LogExit(ThreadData threadData)
        {
            lock (_fileLocker)
            {
                if (threadData.BinaryFileState.LastFileNumber == CurrentFile)
                {
                    WriteLine(DataFlags.MethodExit, threadData, threadData.TopStackEntry.Logger, threadData.TopStackEntry.Level, DateTime.MinValue, null, false);
                    return true;
                }
                else
                {
                    // This is the first call for a new file.  Method-entries for any items on the
                    // call stack were logged in another file.  For simplicity, we clear all stack 
                    // entries as if they never happened, which prevents the corresponding future 
                    // method-exits from being logged to this file.

                    threadData.ResetBinaryFileStateData(CurrentFile, DataFlags.MethodExit);
                    return false;
                }
            }
        }

        // Log a string message.
        internal void LogMsg(Logger logger, ThreadData threadData, TraceLevel lineLevel, string msg)
        {
            WriteLine(DataFlags.Message, threadData, logger, lineLevel, DateTime.MinValue, msg, false);
        }

        // Log a string message with an explicitly specified timestamp.
        internal void LogMsg(DateTime explicitUtcTime, Logger logger, TraceLevel lineLevel, string msg)
        {
            WriteLine(DataFlags.Message, ThreadData.CurrentThreadData, logger, lineLevel, explicitUtcTime, msg, false);
        }

        // Log a message from TracerX itself.
        private void Metalog(Logger logger, TraceLevel lineLevel, string msg)
        {
            WriteLine(DataFlags.Message, ThreadData.CurrentThreadData, logger, lineLevel, DateTime.MinValue, "TracerX: " + msg, true);
        }

        // Determine what data needs to be written based on Flags, 
        // whether circular logging has or should be started,
        // and whether we're starting a new circular block.  
        // Write the output to the file.  Manage the circular part of the log.
        // Return the line number just written.
        private ulong WriteLine(DataFlags flags, ThreadData threadData, Logger logger, TraceLevel lineLevel, DateTime explicitUtcTime, string msg, bool recursive)
        {
            BinaryFileState fileThreadState = threadData.GetBinaryFileState(this);

            lock (_fileLocker)
            {
                try
                {
                    if (IsOpen)
                    {
                        if (fileThreadState.LastFileNumber != CurrentFile)
                        {
                            // First time writing to this file.
                            threadData.ResetBinaryFileStateData(CurrentFile, flags);
                        }

                        // Calling IsNewTime() can change _curTime.
                        if (IsNewTime(explicitUtcTime) || recursive)
                        {
                            // Time differs from previous line.
                            // Set the flag indicating it will be written
                            flags |= DataFlags.Time;
                        }

                        // Possibly start the circular log based on _curTime and/or file size.
                        // Put this after calling IsNewTime() so _curTime will have 
                        // the latest DateTime value.
                        if (FullFilePolicy == FullFilePolicy.Wrap && !recursive && !CircularStarted &&
                            (_curTime >= _circularStartTime || (CircularStartSizeKb > 0 && (_logfile.BaseStream.Position - _openSize) >= CircularStartSizeKb << 10)))
                        {
                            // This will start the circular part of the log if there is enough
                            // room based on current file position and max file size.
                            // It will increment _curBlock if it starts the circular log.
                            // It will also make a recursive call to this method via Metalog.
                            StartCircular(logger, lineLevel);
                        }

                        // Set bits in Flags that indicate what data should be written for this line.
                        flags = SetDataFlags(flags, threadData, fileThreadState, logger, lineLevel);

                        // We need to know the start position of the line we're about
                        // to write to determine if it overwrites the beginning of the oldest block.
                        long startPos = _logfile.BaseStream.Position;

                        // We capture the size of the file before writing the message so we can tell
                        // if the size changes.  
                        long startSize = _logfile.BaseStream.Length;

                        // Write the Flags to the file followed by the data the Flags say to log.
                        WriteData(flags, threadData, fileThreadState, logger, lineLevel, msg);

                        if (CircularStarted)
                        {
                            ManageCircularPart(startPos, startSize);
                        }
                        else if (_logfile.BaseStream.Position >= _maxFilePosition)
                        {
                            // We can't do any meta-logging here because the viewer expects the first record to reach
                            // _maxFilePosition (which we just wrote) to be the last.  Writing another record would cause errors.

                            switch (FullFilePolicy)
                            {
                                case FullFilePolicy.Close:
                                    Close();
                                    break;
                                case FullFilePolicy.Roll:
                                    // If logging a method-entry or method-exit, wait until the next call
                                    // to close and open. 
                                    if ((flags & (DataFlags.MethodEntry | DataFlags.MethodExit)) == 0)
                                    {
                                        // These calls may raise events whose handlers modify our properties.
                                        Close();
                                        Open();
                                    }
                                    break;
                                case FullFilePolicy.Wrap:
                                    // Reaching max file size/position without being in circular mode means we'll never write to
                                    // this file again, so we might as well close it.  Since this is probably not what the user intended,
                                    // also log an event.
                                    string errmsg = "The maximum file size of " + _maxFilePosition + " was reached before circular logging was engaged.  The log file is " + FullPath;
                                    Logger.EventLogging.Log(errmsg, Logger.EventLogging.MaxFileSizeReached);
                                    Close();
                                    break;
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    // Give up!  close the log file, free whatever memory we can.
                    Logger.EventLogging.Log("An exception was thrown while logging: " + ex.ToString(), Logger.EventLogging.ExceptionInLogger);
                    Close();
                }

                return _lineCnt;
            }
        }

        // This sets bits in the flags parameter that specify what data to include with the line.
        private DataFlags SetDataFlags(DataFlags flags, ThreadData threadData, BinaryFileState fileThreadState, Logger logger, TraceLevel lineLevel)
        {
            if (_lastBlock != _curBlock)
            {
                // The very first line in each block (regardless of thread)
                // includes the time in case this line ends up being the first line due to wrapping.
                flags |= DataFlags.Time;

                if (CircularStarted)
                {
                    flags |= DataFlags.BlockStart;
                }
            }

            if (fileThreadState.LastBlock != _curBlock)
            {
                // First line in current block for the thread.  Include all per-thread data.
                flags |= DataFlags.StackDepth | DataFlags.MethodName | DataFlags.TraceLevel | DataFlags.ThreadId | DataFlags.LoggerName;

                if (threadData.Name != null)
                {
                    flags |= DataFlags.ThreadName;
                }
            }
            else
            {
                if (_lastThread != threadData)
                {
                    // This line's thread differs from the last line's thread.
                    flags |= DataFlags.ThreadId;
                }

                if (fileThreadState.LastThreadName != threadData.Name)
                {
                    // Thread's name has changed.
                    flags |= DataFlags.ThreadId | DataFlags.ThreadName;
                }

                if (fileThreadState.CurrentMethod != fileThreadState.LastMethod)
                {
                    // We have a new method name for this thread.
                    flags |= DataFlags.MethodName;
                }

                if (fileThreadState.LastTraceLevel != lineLevel)
                {
                    // This line's trace Level differs from the previous line
                    // logged by this thread.
                    flags |= DataFlags.TraceLevel;
                }

                if (fileThreadState.LastLogger != logger)
                {
                    // This line's logger name differs from the previous line
                    // logged by this thread.
                    flags |= DataFlags.LoggerName;
                }
            }

            return flags;
        }

        // This is what actually writes the output. The Flags parameter specifies what to write.
        private void WriteData(DataFlags flags, ThreadData threadData, BinaryFileState fileThreadState, Logger logger, TraceLevel lineLevel, string msg)
        {
            ++_lineCnt;

            // Write the flags first so the viewer will know what else the record contains.
            _logfile.Write((ushort)flags);

            if (CircularStarted)
            {
                _logfile.Write(_curBlock);

                if ((flags & DataFlags.BlockStart) != DataFlags.None)
                {
                    // This will be the first record in the block.
                    // This stuff helps the viewer find the first chronological block
                    // even after wrapping.  Writting _lastBlockPosition forms a linked
                    // list of blocks that the viewer can follow.

                    //System.Diagnostics.Debug.Print("Block {0} starting at line {1}, position {2}", _curBlock, _lineCnt, _logfile.BaseStream.Position);
                    _logfile.Write(_lineCnt);
                    _logfile.Write(_lastBlockPosition);
                }
            }

            if ((flags & DataFlags.Time) != DataFlags.None)
            {
                _logfile.Write(_curTime.Ticks);
            }

            if ((flags & DataFlags.ThreadId) != DataFlags.None)
            {
                _logfile.Write(threadData.TracerXID);
            }

            if ((flags & DataFlags.ThreadName) != DataFlags.None)
            {
                // ThreadPool thread names get reset to null when a thread is returned
                // to the pool and reused later.
                if (_hasPassword)
                {
                    _encryptor.Encrypt(threadData.Name ?? string.Empty);

                }
                else
                {
                    _logfile.Write(threadData.Name ?? string.Empty);
                }
            }

            if ((flags & DataFlags.TraceLevel) != DataFlags.None)
            {
                _logfile.Write((byte)lineLevel);
            }

            // In format version 5 and later, the viewer subtracts 1 from the stack depth on
            // MethodExit lines instead of the logger, so just write the depth as-is.
            if ((flags & DataFlags.StackDepth) != DataFlags.None)
            {
                _logfile.Write(fileThreadState.StackDepth);

                if (CircularStarted)
                {
                    // In the circular part, include the thread's call stack with the first line
                    // logged for each thread in each block.  This enables the viewer to 
                    // regenerate method entry/exit lines lost due to wrapping.
                    // Added in format version 5.
                    int count = 0;
                    for (StackEntry stackEntry = threadData.TopStackEntry; stackEntry != null; stackEntry = stackEntry.Caller)
                    {
                        if (stackEntry.BinaryFileState == fileThreadState)
                        {
                            ++count;
                            _logfile.Write(stackEntry.EntryLine); // Changed to ulong in version 6.
                            _logfile.Write((byte)stackEntry.Level);

                            if (_hasPassword)
                            {
                                Debug.Assert(stackEntry.Logger.Name != null);
                                _encryptor.Encrypt(stackEntry.Logger.Name);

                                Debug.Assert(stackEntry.MethodName != null);
                                _encryptor.Encrypt(stackEntry.MethodName);
                            }
                            else
                            {
                                _logfile.Write(stackEntry.Logger.Name);
                                _logfile.Write(stackEntry.MethodName);
                            }
                        }
                    }

                    // The StackDepth we wrote previously is how the viewer will know how many 
                    // stack entries to read.
                    System.Diagnostics.Debug.Assert(count == fileThreadState.StackDepth);
                }
            }

            if ((flags & DataFlags.LoggerName) != DataFlags.None)
            {
                if (_hasPassword)
                {
                    _encryptor.Encrypt(logger.Name);
                }
                else
                {
                    _logfile.Write(logger.Name);
                }
            }

            if ((flags & DataFlags.MethodName) != DataFlags.None)
            {
                if (_hasPassword)
                {
                    _encryptor.Encrypt(fileThreadState.CurrentMethod);
                }
                else
                {
                    _logfile.Write(fileThreadState.CurrentMethod);
                }

                fileThreadState.LastMethod = fileThreadState.CurrentMethod;
            }

            if ((flags & DataFlags.Message) != DataFlags.None)
            {
                if (_hasPassword)
                {
                    _encryptor.Encrypt(msg ?? "");
                }
                else
                {
                    _logfile.Write(msg);
                }
            }

            _lastBlock = _curBlock;
            _lastThread = threadData;
            fileThreadState.LastBlock = _curBlock;
            fileThreadState.LastThreadName = threadData.Name;
            fileThreadState.LastTraceLevel = lineLevel;
            fileThreadState.LastLogger = logger;
        }

        // Called immediately after writing a line of output in circular mode.
        // This may wrap and/or truncate the file, but does not write any output.
        // startPos = the file position of the beginning of the line just written.
        private void ManageCircularPart(long startPos, long startSize)
        {
            long endPos = _logfile.BaseStream.Position;

            // Truncate the file if we just overwrote the block that extends past max file size, since
            // the viewer can't access that info, it can be arbitrarily large, and it screws things up
            // if another logging session appends output to the file.
            if (_maxBlockPosition >= startPos && _maxBlockPosition < endPos)
            {
                //System.Diagnostics.Debug.Print("Last physical block start was overwritten at " + _maxBlockPosition + ".  Truncating file at " + endPos);
                _maxBlockPosition = 0;
                _logfile.BaseStream.SetLength(endPos);
            }

            if (endPos >= _maxFilePosition)
            {
                // Since we've reached or exceeded the max file size/position, it's time to wrap and
                // start a new block even if the current block only has one line.  
                //System.Diagnostics.Debug.Print("File position exceeded max size.");

                // Remember the file location of this block because it extends
                // past the max file size.             
                _maxBlockPosition = _curBlockPosition;

                // Wrap back to the beginning of the circular part.
                _logfile.BaseStream.Position = _positionOfCircularPart;

                // Cause the current block to end and a new block to start.
                EndCurrentBlock();

                if (!_everWrapped)
                {
                    Logger.EventLogging.Log("The output file wrapped for the first time: " + FullPath, Logger.EventLogging.FirstWrap);
                    _everWrapped = true;

                    // Now that we've wrapped, the file size and time stamp doesn't change when
                    // we write to the middle of the file.  This timer periodically updates
                    // the timestamp so the viewer will load new lines.
                    _staleTimestampTimer = new Timer(new TimerCallback(UpdateFileTime));
                }
            }
            else
            {
                // Keep track of how many bytes are in this block so we can determine
                // if it's time to start a new one based on block size.
                _bytesInBlock += (uint)(endPos - startPos);

                // Now check if the current block has enough bytes to start a new block.
                if (_bytesInBlock >= _blockSize)
                {
                    EndCurrentBlock();
                }
            }

            if (_everWrapped)
            {
                // On XP, the file's LastWriteTime changes automatically when the logger writes to the
                // file ONLY IF the size changes too.  If the file has wrapped (meaning the size rarely changes),
                // we "manually" update the LastWriteTime.  Thus, on XP, both properties change until
                // the file wraps, and then only the LastWriteTime changes.
                // On Vista, the LastWriteTime doesn't change automatically until the logger closes the 
                // file (even if the size does change).  However, the size will change until the file wraps, 
                // then we start "manually" setting the LastWriteTime.  Thus, on Vista, only the size changes 
                // until the file wraps, then only the LastWriteTime changes.  The viewer monitors both properties.
                if (_logfile.BaseStream.Length == startSize)
                {
                    // The line we just wrote did not change the file size.
                    // Use a timer to force the LastWriteTime to change no
                    // later than 250 ms from now so the viewer will notice the file has changed.
                    if (!_timerIsTicking)
                    {
                        _staleTimestampTimer.Change(250, Timeout.Infinite);
                        _timerIsTicking = true;
                    }
                }
                else if (_timerIsTicking)
                {
                    // The file size changed by itself (so to speak), so cancel the timer
                    // until the next line is written.
                    _staleTimestampTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    _timerIsTicking = false;
                }
            }
        }

        // Called by a worker thread via _staleTimestampTimer to set the LastWriteTime so
        // the viewer will notice the file has changed.
        private void UpdateFileTime(object o)
        {
            lock (_fileLocker)
            {
                if (_timerIsTicking)
                {
                    try
                    {
                        long writeTime = DateTime.Now.ToFileTime();
                        _staleTimestampTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        _timerIsTicking = false;

                        SetFileTime(_fileHandle, IntPtr.Zero, IntPtr.Zero, ref writeTime);
                    }
                    catch (Exception)
                    {
                        // The file was probably closed.
                        _staleTimestampTimer.Dispose();
                    }
                }
            }
        }

        private void EndCurrentBlock()
        {
            ++_curBlock;
            _lastBlockPosition = _curBlockPosition;
            _curBlockPosition = _logfile.BaseStream.Position;
            _bytesInBlock = 0;
        }

        // Start circular logging. Thread-safety provided by callers.
        private void StartCircular(Logger logger, TraceLevel level)
        {
            if (!CircularStarted)
            {
                uint minBlockSize = 10000;
                uint maxBlocks = 200;
                uint minNeeded = 2 * minBlockSize; // Ensure there's room for at least two blocks.
                long bytesLeft = _maxFilePosition - _logfile.BaseStream.Position;

                if (bytesLeft < minNeeded)
                {
                    // Not enough room means it's too late to start circular logging.
                    Logger.EventLogging.Log("Circular logging would have started, but there was not enough room left in the file: " + FullPath, Logger.EventLogging.TooLateForCircular);

                    // Set the circular start thresholds such that we won't
                    // ever try to start circular mode again.
                    _circularStartTime = DateTime.MaxValue;
                    CircularStartSizeKb = 0;

                    // If there's enough room, meta-log a message about this.

                    string msg = string.Format("Circular logging would have started here, but only {0} bytes remain before reaching the maximum file position of {1}.  TracerX requires at least {2} for circular logging.", bytesLeft, _maxFilePosition, minNeeded);

                    if (bytesLeft > msg.Length + 50)
                    {
                        Metalog(logger, level, msg);
                    }
                }
                else
                {
                    // Figure out what block size will be no less than minBlockSize
                    // bytes and yield no more than maxBlocks blocks.
                    _blockSize = (uint)(bytesLeft / maxBlocks + 1);
                    if (_blockSize < minBlockSize)
                    {
                        _blockSize = minBlockSize;
                    }

                    // Meta-log the last line in the non-circular part.  The bytesLeft reported here is slightly 
                    // inaccurate because the message itself consumes some of those bytes.
                    Metalog(logger, level, string.Format("This is the last line before the circular part of the log.  Block size = {0}, bytes left = {1}.", _blockSize, bytesLeft));
                    Logger.EventLogging.Log("Circular logging has started for binary file " + FullPath + " with " + bytesLeft + " bytes remaining.  Block size is " + _blockSize, Logger.EventLogging.CircularLogStarted);

                    EndCurrentBlock();
                    _positionOfCircularPart = _logfile.BaseStream.Position;

                    // Meta-log first line in circular part.
                    Metalog(logger, level, "This is the first line in the circular part of the log (never wrapped).");
                }
            }
        }
        #endregion
    }
}