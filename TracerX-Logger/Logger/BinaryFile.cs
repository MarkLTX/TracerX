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
using System.Collections;
using System.Collections.Generic;

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
                    // Update the list of recently created files in a worker thread, but don't use a
                    // ThreadPool thread because they are background threads, and some apps
                    // terminate so quickly that a background thread won't finish before being
                    // killed.

                    Thread thread = new Thread(() => RecentlyCreated.AddToRecentlyCreated(FullPath));
                    thread.IsBackground = false;
                    thread.Start();
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
                    _fileHandle = null;

                    if (_viewerSignaler != null)
                    {
                        _viewerSignaler.Close();
                        _viewerSignaler = null;
                    }

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

        // Used to update the file's LastWriteTime (and CreationTime).
        private SafeFileHandle _fileHandle;

        // Used to signal viewers when new messages are logged via named system events.
        private NamedEventsManager _viewerSignaler;

        #endregion

        #region PrivateMethods

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetFileTime(SafeHandle hFile, IntPtr lpCreationTime, IntPtr lpLastAccessTime, ref long lpLastWriteTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetFileTime(SafeHandle hFile, ref long lpCreationTime, IntPtr lpLastAccessTime, IntPtr lpLastWriteTime);

        #region Open log

        // This either opens the originally specified log file, opens an
        // alternate log file (e.g. with "(A)" or "(B)" in the name), or throws an exception.
        protected override void InternalOpen()
        {
            // This char is used to generate alternate file names with (A)-(Z) if the desired output file can't be opened.
            char c = 'A';

            // This will contain the path of the "temp" file that the existing output file gets
            // temporarily renamed to if it exists and needs to be replaced rather than
            // appended to.
            string renamedOutFile = null;

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
                        // The existing file should either be replaced or appended to.

                        if (outFile.Length < AppendIfSmallerThanMb << _shift &&
                            outFile.Length < MaxSizeMb << _shift)
                        {
                            // Open the file in append mode.

                            // If the file is in use, this throws an exception.
                            fileStream = new FileStream(FullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);

                            // Check the format version of the existing file.
                            var reader = new BinaryReader(fileStream);
                            string msg = null;
                            fileStream.Position = 0;
                            int oldVersion = reader.ReadInt32();

                            if (oldVersion == FormatVersion)
                            {
                                // If the original file is password-protected, verify the
                                // user specified the same password this time.

                                bool oldHasPassword = reader.ReadBoolean();

                                if (oldHasPassword && _hasPassword)
                                {
                                    // Check that passwords (actually the hashes) match.
                                    byte[] oldHash = reader.ReadBytes(20);

                                    if (oldHash.SequenceEqual(_sha1PasswordHash))
                                    {
                                        // Read and discard the password hint because we need to read the file GUID later.
                                        var hint = reader.ReadString();
                                    }
                                    else
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
                                // Success! We'll start writing at the end, but first we need to read the
                                // file's original GUID to use in the name of an event used to signal the
                                // viewer(s) when new messages are logged.

                                string asmversion = reader.ReadString();
                                uint maxsize = reader.ReadUInt32();
                                long maxfilepos = reader.ReadInt64();
                                FileGuid = new Guid(reader.ReadBytes(16));

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
                        } // Opening in append mode.
                        else
                        {
                            // Replace the existing output file with a new file.

                            if (Archives > 0)
                            {
                                // This condition means we keep "archives" (old _1, _2, etc. files)
                                // that need to be "rolled".  However we don't actually
                                // roll the archives until we know we can create the new file
                                // by actually creating it.  Yet we still need to rename the
                                // existing output file so it isn't lost when we open the new file. This new name
                                // is temporary until we get around to rolling all the archives.  See RollArchives()

                                renamedOutFile = RenameOutputFile(outFile.CreationTime);
                            }

                            // This will throw an exception if we lack write access or the file is in use.
                            fileStream = new FileStream(FullPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                        } // Creating new file.
                    }
                    else
                    {
                        // This will throw an exception if we lack write access or the file is in use.
                        // Note that File.Exists is false even if the file exists, if we lack read access.
                        fileStream = new FileStream(FullPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    }
                }
                catch (System.IO.IOException ex)
                {
                    // File is probably in use or we lack permission, try the next alternate name up to (Z).

                    if (c >= 'Z')
                    {
                        // That was the last chance.  Rethrow the exception to
                        // end the loop and cause the exception to be logged.
                        throw;
                    }
                    else
                    {
                        // Try the next alternative file name, up to Z.
                        // Changing _logFileName also changes Name and FullName.
                        // Note that _logFileName has no extension or _0.

                        Debug.Print("Opening alternate file name due to exception:\n" + ex.ToString());

                        _logFileName = string.Format("{0}({1})", simpleName, c);
                        ++c;
                        renamedOutFile = null;
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
            _maxFilePosition = MaxSizeMb << _shift;
            _openTimeUtc = DateTime.UtcNow;

            if (!appending)
            {
                // Due to a bizarre Windows feature called "File System Tunneling", the
                // file's creation time will be wrong unless we set it explicitly like this.
                long now = _openTimeUtc.ToLocalTime().ToFileTime();
                SetFileTime(_fileHandle, ref now, IntPtr.Zero, IntPtr.Zero);
            }

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

            // The _viewerSignaler is used to notify TracerX-Viewer and/or TracerX-Logger when
            // the output file is modified.  It must be created before the first log message 
            // is written because BinaryFile.WriteLine() references it.

            _viewerSignaler = new NamedEventsManager(FileGuid, FullPath);

            if (this == Logger.DefaultBinaryFile) Logger.StandardData.LogEnvironmentInfo();
            RollArchives(renamedOutFile);
        }

        // This attempts to rename the FullPath file to a temp name.  
        // This either throws an exception or returns the new file path.
        private string RenameOutputFile(DateTime originalCreateTime)
        {
            string newFilePath = null;
            string curFile;
            Exception lastException = null;

            // First we need an available temporary file name to rename FullPath to.  There
            // shouldn't be any temp files but if there are try to delete them.  The loop goes
            // in descending numerical order so renamedFile will be set to the lowest numbered file
            // that doesn't exist or is deleted successfully.

            for (int i = 9; i > 0; --i)
            {
                curFile = FullPath + ".tempname_" + i;

                try
                {
                    // No ".tempname_n" files shouldn't exist, but try to delete any that do.
                    // File.Delete() doesn't throw an exception if the file doesn't exist.
                    // If the file is in use, File.Delete() throws an exception.
                    // Sometimes File.Delete() throws an UnauthorizedAccessException that
                    // eventually stops happening if you wait long enough or restart the computer,
                    // but we don't have time to wait which is why we try 9 different files.

                    File.Delete(curFile);

                    // Getting here means the file didn't exist or was successfully deleted so we can use the name.
                    newFilePath = curFile;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }
            }

            if (newFilePath == null)
            {
                throw new Exception("Unable to delete old temp file.  See inner exception.", lastException);
            }
            else
            {
                // DO NOT use outFile.Exists or outFile.Move() in here.
                for (int attempts = 3; attempts > 0 && File.Exists(FullPath); --attempts)
                {
                    try
                    {
                        // If the file is in use, this throws an exception.
                        File.Move(FullPath, newFilePath);
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

            File.SetCreationTime(newFilePath, originalCreateTime);
            return newFilePath;
        }

        private void WritePreamble(bool appending)
        {
            DateTime local = _openTimeUtc.ToLocalTime();
            Version asmVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

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

                FileGuid = Guid.NewGuid();
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

        // Log the exit of the method call at the top of the threadData's stack.
        // stackEntry is still on the stack.        
        internal bool LogExit(ThreadData threadData, bool isNormal)
        {
            lock (_fileLocker)
            {
                if (threadData.BinaryFileState.LastFileNumber == CurrentFile)
                {
                    if (isNormal)
                        WriteLine(DataFlags.MethodExit, threadData, threadData.TopStackEntry.Logger, threadData.TopStackEntry.Level, DateTime.MinValue, null, false);
                    else
                        WriteLine(DataFlags.MethodExit | DataFlags.Message, threadData, threadData.TopStackEntry.Logger, threadData.TopStackEntry.Level, DateTime.MinValue, ", probably by await", false);
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

        // Determine what data needs to be written based on dataFlags, 
        // whether circular logging has or should be started,
        // and whether we're starting a new circular block.  
        // Write the output to the file.  Manage the circular part of the log.
        // Return the line number just written.
        private ulong WriteLine(DataFlags dataFlags, ThreadData threadData, Logger logger, TraceLevel lineLevel, DateTime explicitUtcTime, string msg, bool recursive)
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
                            threadData.ResetBinaryFileStateData(CurrentFile, dataFlags);
                        }

                        // Calling IsNewTime() can change _curTime.
                        if (IsNewTime(explicitUtcTime) || recursive)
                        {
                            // Time differs from previous line.
                            // Set the flag indicating it will be written
                            dataFlags |= DataFlags.Time;
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
                        dataFlags = SetDataFlags(dataFlags, threadData, fileThreadState, logger, lineLevel);

                        // We need to know the start position of the line we're about
                        // to write to determine if it overwrites the beginning of the oldest block.
                        long startPos = _logfile.BaseStream.Position;

                        // We capture the size of the file before writing the message so we can tell
                        // if the size changes.  
                        long startSize = _logfile.BaseStream.Length;

                        // Write the Flags to the file followed by the data the Flags say to log.
                        WriteData(dataFlags, threadData, fileThreadState, logger, lineLevel, msg);

                        // If the file is being viewed, this will notify the viewer that the file was changed.
                        _viewerSignaler.SignalEvents();

                        if (CircularStarted)
                        {
                            ManageCircularPart(startPos, startSize, dataFlags);
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
                                    if ((dataFlags & (DataFlags.MethodEntry | DataFlags.MethodExit)) == 0)
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

        // This is what actually writes the output. The dataFlags parameter specifies what to write.
        private void WriteData(DataFlags dataFlags, ThreadData threadData, BinaryFileState fileThreadState, Logger logger, TraceLevel lineLevel, string msg)
        {
            ++_lineCnt;

            // Write the flags first so the viewer will know what else the record contains.
            _logfile.Write((ushort)dataFlags);

            if (CircularStarted)
            {
                _logfile.Write(_curBlock);

                if ((dataFlags & DataFlags.BlockStart) != DataFlags.None)
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

            if ((dataFlags & DataFlags.Time) != DataFlags.None)
            {
                _logfile.Write(_curTime.Ticks);
            }

            if ((dataFlags & DataFlags.ThreadId) != DataFlags.None)
            {
                _logfile.Write(threadData.TracerXID);
            }

            if ((dataFlags & DataFlags.ThreadName) != DataFlags.None)
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

            if ((dataFlags & DataFlags.TraceLevel) != DataFlags.None)
            {
                _logfile.Write((byte)lineLevel);
            }

            // In format version 5 and later, the viewer subtracts 1 from the stack depth on
            // MethodExit lines instead of the logger, so just write the depth as-is.
            if ((dataFlags & DataFlags.StackDepth) != DataFlags.None)
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

            if ((dataFlags & DataFlags.LoggerName) != DataFlags.None)
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

            if ((dataFlags & DataFlags.MethodName) != DataFlags.None)
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

            if ((dataFlags & DataFlags.Message) != DataFlags.None)
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
        // This does update the file's LastWriteTime.
        // startPos = the file position of the beginning of the line just written.
        private void ManageCircularPart(long startPos, long startSize, DataFlags dataFlags)
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

                if (_logfile.BaseStream.Length == startSize && (dataFlags & DataFlags.Time) != DataFlags.None)
                {
                    // The line we just wrote did not change the file size so manually update the file's LastWiteTime.

                    long writeTime = _curTime.ToLocalTime().ToFileTime();
                    SetFileTime(_fileHandle, IntPtr.Zero, IntPtr.Zero, ref writeTime);
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