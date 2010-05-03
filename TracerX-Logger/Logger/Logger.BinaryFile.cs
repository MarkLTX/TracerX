using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace TracerX {
    public partial class Logger {
        /// <summary>
        /// Methods and configuration for logging to the full-featured binary file supported by the viewer
        /// </summary>
        public sealed class BinaryFile : FileBase {

            #region Public

            /// <summary>
            /// Opens the log file using the current value of various properties.
            /// </summary>
            public override bool Open() {
                bool result = base.Open();

                if (result && AddToListOfRecentlyCreatedFiles) {
                    AddToRecentlyCreated();
                }

                return result;
            }

            /// <summary>
            /// Closes the log file.  It should not be reopened.
            /// </summary>
            /// <remarks>
            /// This closes the file.  However, it doesn't free the ThreadStatic memory previously allocated  
            /// for all the threads may have been logging, nor does it remove Logger objects
            /// that have been created.  Since those structures cannot be reset or cleared,
            /// the log file should not be reopened because it won't start in a "clean" state.
            /// </remarks>
            public override void Close() {
                lock (_fileLocker) {
                    if (_logfile != null) {
                        _logfile.Close();
                        _logfile = null;
                    }

                    _lastThread = null;
                }
            }

            /// <summary>
            /// The format version of the file, which often changes with new releases of TracerX.
            /// </summary>
            public int FormatVersion { get { return _formatVersion; } }

            /// <summary>
            /// Unique GUID assigned to the file when it is opened.
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

            protected override Stream BaseStream {
                get { return _logfile.BaseStream; }
            }

            /// <summary>
            /// Current block number (used only by test drivers).
            /// </summary>
            public uint CurrentBlock { get { return _curBlock; } }

            /// <summary>
            /// If a password is set before the file is opened, the viewer will
            /// require the user to enter the same password to open the file.
            /// This only applies when creating a new file, not
            /// when appending to an existing file.
            /// </summary>
            public string Password {
                set {
                    if (string.IsNullOrEmpty(value)) {
                        _hasPassword = false;
                    } else {
                        _passwordHash = value.GetHashCode();
                        _hasPassword = true;
                    }
                }
            }
            private bool _hasPassword;
            private int _passwordHash;

            /// <summary>
            /// If true, the log file will appear in the list of recently
            /// created files on the host computer.
            /// </summary>
            public bool AddToListOfRecentlyCreatedFiles {
                get { return _addToListOfRecentlyCreatedFiles; }
                set { _addToListOfRecentlyCreatedFiles = value; }
            }
            private bool _addToListOfRecentlyCreatedFiles = true;
            #endregion

            #region Data members
            // The version of the log file format created by this assembly.
            private const int _formatVersion = 6; // incremented 12/17/09

            // The output log file.  Null until OpenLog() succeeds.
            // User output goes through this object.
            private BinaryWriter _logfile;

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

            #region Singleton

            // Private ctor to guarantee singleton.
            private BinaryFile()
                : base(".tx1") {
            }

            static internal BinaryFile Singleton {
                get { return _singleton; }
            }

            private static BinaryFile _singleton = new BinaryFile();

            #endregion Singleton

            #region PrivateMethods

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool SetFileTime(SafeHandle hFile, IntPtr lpCreationTime, IntPtr lpLastAccessTime, ref long lpLastWriteTime);

            #region Open log

            // This either opens the originally specified log file, opens an
            // alternate log file, or throws an exception.
            protected override void InternalOpen() {
                // Use this to generate alternate file names A-Z if file can't be opened.
                char c = 'A';
                string renamedFile = null;
                FileStream fileStream = null;
                bool appending = false;

                while (fileStream == null) {
                    try {
                        FileInfo outFile = new FileInfo(FullPath);

                        if (outFile.Exists) {
                            if (outFile.Length < AppendIfSmallerThanMb << 20) {
                                // Open in append mode.
                                // If the file is in use, this throws an exception.
                                fileStream = new FileStream(FullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);

                                // Check the format version of the existing file.
                                var reader = new BinaryReader(fileStream);
                                fileStream.Position = 0;
                                int oldVersion = reader.ReadInt32();

                                if (oldVersion == FormatVersion) {
                                    // Success!
                                    // We'll start writing at the end.
                                    fileStream.Position = fileStream.Length;
                                    _openSize = fileStream.Length;
                                    appending = true;
                                } else {
                                    // We don't mix format versions in one file.  Turn Append off and try again.
                                    var msg = string.Format("TracerX could not append to the existing log file\n{0}\nbecause its format version ({1}) was not equal to the current format version ({2}).", FullPath, oldVersion, FormatVersion);
                                    EventLogging.Log(msg, EventLogging.AppendVersionConflict);
                                    AppendIfSmallerThanMb = 0;
                                    fileStream.Dispose();
                                    fileStream = null;
                                    continue;
                                }
                            } else {
                                if (Archives > 0) {
                                    renamedFile = FullPath + ".tempname";
                                    if (File.Exists(renamedFile)) File.Delete(renamedFile);

                                    // If the file is in use, this throws an exception.
                                    outFile.MoveTo(renamedFile);
                                }

                                fileStream = new FileStream(FullPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                            }
                        } else {
                            // If the file is in use, this throws an exception.
                            fileStream = new FileStream(FullPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                        }
                    } catch (System.IO.IOException) {
                        // File is probably in use, try next alternate name.
                        if (c > 'Z') {
                            // That was the last chance.  Rethrow the exception to
                            // end the loop and cause the exception to be logged.
                            throw;
                        } else {
                            // Try the next alternative file name, up to Z.
                            // Changing Name also changes FullName.
                            string bareFileName = Path.GetFileNameWithoutExtension(Name);
                            Name = string.Format("{0}({1})", bareFileName, c);
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
                _maxFilePosition = _openSize + (MaxSizeMb << 20);
                _openTimeUtc = DateTime.UtcNow;

                if (CircularStartDelaySeconds == 0) {
                    _circularStartTime = DateTime.MaxValue;
                } else {
                    _circularStartTime = _openTimeUtc.AddSeconds(CircularStartDelaySeconds);
                }

                WritePreamble(appending);
                LogStandardData();
                ManageArchives(renamedFile);
            }

            // Add the log file path to the list of files persisted for the viewer to read.
            private void AddToRecentlyCreated() {
                try {
                    string listFile = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        "TracerX\\RecentlyCreated.txt"
                        );
                    string[] files;

                    // First read the existing list of files.
                    try {
                        files = File.ReadAllLines(listFile);
                    } catch (Exception) {
                        // Most likely, the file doesn't exist.
                        // Make sure the directory exists so we can create the file there.

                        files = null;
                        string dir = Path.GetDirectoryName(listFile);
                        if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                    }

                    if (files == null) {
                        // No lines read, so just create the file with the current
                        // FullPath as its only content.
                        File.WriteAllText(listFile, FullPath);
                    } else {
                        // Overwrite the file, putting the most recent file name (FullPath) at the top.
                        using (StreamWriter writer = new StreamWriter(listFile, false)) {
                            writer.WriteLine(FullPath);

                            // Write up to 8 of the previously read file names, omitting
                            // any that match the file name we just wrote (i.e. prevent duplicates).
                            // Thus, the file will contain at most 9 lines.
                            int i = 0;
                            foreach (string filename in files) {
                                if (!string.IsNullOrEmpty(filename) && !string.Equals(filename, FullPath, StringComparison.InvariantCultureIgnoreCase)) {
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
                } catch (Exception) {
                    // Nothing to do, really.
                    System.Diagnostics.Debug.Print("Exception in AddToRecentlyCreated.");
                }
            }

            private void LogStandardData() {
                Logger Log = Logger.StandardData;

                using (Log.InfoCall()) {
                    Assembly entryAssembly = Assembly.GetEntryAssembly();

                    if (entryAssembly != null) {
                        Log.Info("EntryAssembly.Location = ", entryAssembly.Location);
                        Log.Info("EntryAssembly.FullName = ", entryAssembly.FullName); // Includes assembly version.

                        try {
                            // Try to get the file version.
                            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(entryAssembly.Location);
                            Log.Info("FileVersionInfo.FileVersion = ", fvi.FileVersion);
                            Log.Info("FileVersionInfo.ProductVersion = ", fvi.ProductVersion);
                        } catch (Exception) {
                        }
                    }

                    try {
                        Log.Info("AppDomain = ", AppDomain.CurrentDomain.FriendlyName);
                    } catch (Exception) {
                    }

                    Log.Info("Environment.OSVersion = ", Environment.OSVersion);
                    Log.Info("Environment.CurrentDirectory = ", Environment.CurrentDirectory);
                    Log.Info("Environment.UserInteractive = ", Environment.UserInteractive);

                    Log.Debug("Environment.CommandLine = ", Environment.CommandLine);

                    Log.Verbose("Environment.MachineName = ", Environment.MachineName);
                    Log.Verbose("Environment.UserDomainName = ", Environment.UserDomainName);
                    Log.Verbose("Environment.UserName = ", Environment.UserName);
                }
            }

            // Write the header/preamble information to the file.  
            private void WritePreamble(bool appending) {
                DateTime local = _openTimeUtc.ToLocalTime();
                Version asmVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                FileGuid = Guid.NewGuid();

                if (appending) {
                    _logfile.Write((ushort)DataFlags.NewSession);
                } else {
                    // Format version should be the first
                    // item in the file so the viewer knows how to read the rest.
                    _logfile.Write(_formatVersion);
                    _logfile.Write(_hasPassword); // Added in version 5.
                    if (_hasPassword) _logfile.Write(_passwordHash); // Added in version 4.
                }

                _logfile.Write(asmVersion.ToString()); // Added in file format version 3.
                _logfile.Write(MaxSizeMb);
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
            // exist or was opened in append mode), and no renaming is necessary.
            private void ManageArchives(string renamedFile) {
                string bareFileName = Path.GetFileNameWithoutExtension(Name);
                string bareFilePath = Path.Combine(Directory, bareFileName);
                int highestNumKept = (renamedFile == null) ? (int)Archives : (int)Archives - 1;

                // This gets the archived files in reverse order.
                // Logfiles from older versions may only have one numeric character in the filename suffix.
                string[] files = EnumOldFiles(bareFileName, "_?*.tx1");

                if (files != null) {
                    foreach (string oldFile in files) {
                        // Extract the archive number that comes after "bareFileName_".
                        // The number must be one or two chars or it's not one of our files.
                        string plain = Path.GetFileNameWithoutExtension(oldFile);
                        string numPart = plain.Substring(bareFileName.Length + 1);

                        if (numPart.Length == 1 || numPart.Length == 2) {
                            int num;

                            if (int.TryParse(numPart, out num) && num > 0) {
                                if (num > highestNumKept) {
                                    // The archive number is more than the user wants to keep, so delete it.
                                    try {
                                        File.Delete(oldFile);
                                    } catch (Exception ex) {
                                        string msg = string.Format("An exception occurred while deleting the old log file\n{0}\n\n{1}", oldFile, ex);
                                        EventLogging.Log(msg, EventLogging.ExceptionInArchive);
                                    }
                                } else if (renamedFile != null) {
                                    // Rename (increment the file's archive number by 1).
                                    TryRename(oldFile, bareFilePath, num + 1);
                                }
                            }
                        }
                    }

                    // Finally, rename the most recent log file, if it exists.
                    if (renamedFile != null) {
                        TryRename(renamedFile, bareFilePath, 1);
                    }

                }
            }

            #endregion

            // If DateTime.UtcNow differs from _curTime, update _curtime
            // and return true; else return false.
            private bool IsNewTime() {
                DateTime now = DateTime.UtcNow;
                if (now == _curTime) {
                    return false;
                } else {
                    _curTime = now;
                    return true;
                }
            }

            // Log the entry (start) of a method call.
            internal void LogEntry(ThreadData threadData, StackEntry stackEntry) {
                // stackEntry is not yet on the stack.
                // Remember the line number where the MethodEntry flag is written so
                // we can write it into the log when the method exits.
                stackEntry.EntryLine = WriteLine(DataFlags.MethodEntry, threadData, stackEntry.Logger, stackEntry.Level, null, false);
            }

            // Log the exit of a method call.
            internal void LogExit(ThreadData threadData, StackEntry stackEntry) {
                // stackEntry is still on the stack.
                WriteLine(DataFlags.MethodExit, threadData, stackEntry.Logger, stackEntry.Level, null, false);
            }

            // Log a string message.
            internal void LogMsg(Logger logger, ThreadData threadData, TraceLevel lineLevel, string msg) {
                WriteLine(DataFlags.Message, threadData, logger, lineLevel, msg, false);
            }

            // Log a message from TracerX itself.
            private void Metalog(Logger logger, TraceLevel lineLevel, string msg) {
                WriteLine(DataFlags.Message, ThreadData.CurrentThreadData, logger, lineLevel, "TracerX: " + msg, true);
            }

            // Determine what data needs to be written based on Flags, 
            // whether circular logging has or should be started,
            // and whether we're starting a new circular block.  
            // Write the output to the file.  Manage the circular part of the log.
            // Return the line number just written.
            private ulong WriteLine(DataFlags flags, ThreadData threadData, Logger logger, TraceLevel lineLevel, string msg, bool recursive) {
                lock (_fileLocker) {
                    try {
                        if (IsOpen) {
                            // Calling IsNewTime() can change _curTime.
                            if (IsNewTime() || recursive) {
                                // Time differs from previous line.
                                // Set the flag indicating it will be written
                                flags |= DataFlags.Time;
                            }

                            // Possibly start the circular log based on _curTime and/or file size.
                            // Put this after calling IsNewTime() so _curTime will have 
                            // the latest DateTime value.
                            if (!recursive && !CircularStarted && (_curTime >= _circularStartTime ||
                                (CircularStartSizeKb > 0 && (_logfile.BaseStream.Position - _openSize) >= CircularStartSizeKb << 10))) //
                            {
                                // This will increment _curBlock if it starts the circular log.
                                // It will also make a recursive call to this method via Metalog.
                                StartCircular(logger, lineLevel);
                            }

                            // Set bits in Flags that indicate what data should be written for this line.
                            flags = SetDataFlags(flags, threadData, logger, lineLevel);

                            // We need to know the start position of the line we're about
                            // to write to determine if it overwrites the beginning of the oldest block.
                            long startPos = _logfile.BaseStream.Position;

                            // We capture the size of the file before writing the message so we can tell
                            // if the size changes.  
                            long startSize = _logfile.BaseStream.Length;

                            // Write the Flags to the file followed by the data the Flags say to log.
                            WriteData(flags, threadData, logger, lineLevel, msg);

                            if (CircularStarted) {
                                ManageCircularPart(startPos, startSize);
                            } else if (_logfile.BaseStream.Position >= _maxFilePosition) {
                                // Reaching max file size/position without being in circular mode means we'll never write to
                                // this file again, which is probably not what the user intended.
                                string errmsg = "The maximum file size of " + MaxSizeMb + " Mb was reached before circular logging was engaged.  The log file is " + FullPath;
                                EventLogging.Log(errmsg, EventLogging.MaxFileSizeReached);
                                Close();
                            }
                        }
                    } catch (Exception ex) {
                        // Give up!  close the log file, free whatever memory we can.
                        EventLogging.Log("An exception was thrown while logging: " + ex.ToString(), EventLogging.ExceptionInLogger);
                        Close();
                    }

                    return _lineCnt;
                }
            }

            // This sets bits in the flags parameter that specify what data to include with the line.
            private DataFlags SetDataFlags(DataFlags flags, ThreadData threadData, Logger logger, TraceLevel lineLevel) {
                if (_lastBlock != _curBlock) {
                    // The very first line in each block (regardless of thread)
                    // includes the time in case this line ends up being the first line due to wrapping.
                    flags |= DataFlags.Time;

                    if (CircularStarted) {
                        flags |= DataFlags.BlockStart;
                    }
                }

                if (threadData.LastBlock != _curBlock) {
                    // First line in current block for the thread.  Include all per-thread data.
                    flags |= DataFlags.StackDepth | DataFlags.MethodName | DataFlags.TraceLevel | DataFlags.ThreadId | DataFlags.LoggerName;

                    if (Thread.CurrentThread.Name != null) {
                        flags |= DataFlags.ThreadName;
                    }
                } else {
                    if (_lastThread != threadData) {
                        // This line's thread differs from the last line's thread.
                        flags |= DataFlags.ThreadId;
                    }

                    if (threadData.LastName != Thread.CurrentThread.Name) {
                        // Thread's name has changed.
                        flags |= DataFlags.ThreadId | DataFlags.ThreadName;
                    }

                    if (threadData.CurrentFileMethod != threadData.LastMethod) {
                        // We have a new method name for this thread.
                        flags |= DataFlags.MethodName;
                    }

                    if (threadData.LastTraceLevel != lineLevel) {
                        // This line's trace Level differs from the previous line
                        // logged by this thread.
                        flags |= DataFlags.TraceLevel;
                    }

                    if (threadData.LastLogger != logger) {
                        // This line's logger name differs from the previous line
                        // logged by this thread.
                        flags |= DataFlags.LoggerName;
                    }
                }

                return flags;
            }

            // This is what actually writes the output. The Flags parameter specifies what to write.
            private void WriteData(DataFlags flags, ThreadData threadData, Logger logger, TraceLevel lineLevel, string msg) {
                ++_lineCnt;

                _logfile.Write((ushort)flags);

                if (CircularStarted) {
                    if ((flags & DataFlags.BlockStart) == DataFlags.None) {
                        _logfile.Write(_curBlock);
                    } else {
                        //System.Diagnostics.Debug.Print("Block {0} starting at line {1}, position {2}", _curBlock, _lineCnt, _logfile.BaseStream.Position);
                        _logfile.Write(_curBlock);
                        _logfile.Write(_lineCnt);
                        _logfile.Write(_lastBlockPosition);
                    }
                }

                if ((flags & DataFlags.Time) != DataFlags.None) {
                    _logfile.Write(_curTime.Ticks);
                }

                if ((flags & DataFlags.ThreadId) != DataFlags.None) {
                    _logfile.Write(threadData.TracerXID);
                }

                if ((flags & DataFlags.ThreadName) != DataFlags.None) {
                    // ThreadPool thread names get reset to null when a thread is returned
                    // to the pool and reused later.
                    if (Thread.CurrentThread.Name == null) {
                        _logfile.Write(string.Empty);
                    } else {
                        _logfile.Write(Thread.CurrentThread.Name);
                    }
                }

                if ((flags & DataFlags.TraceLevel) != DataFlags.None) {
                    _logfile.Write((byte)lineLevel);
                }

                // In format version 5 and later, the viewer subtracts 1 from the stack depth on
                // MethodExit lines instead of the logger, so just write the depth as-is.
                if ((flags & DataFlags.StackDepth) != DataFlags.None) {
                    _logfile.Write(threadData.FileStackDepth);

                    if (CircularStarted) {
                        // In the circular part, include the thread's call stack with the first line
                        // logged for each thread in each block.  This enables the viewer to 
                        // regenerate method entry/exit lines lost due to wrapping.
                        // Added in format version 5.
                        int count = 0;
                        for (StackEntry stackEntry = threadData.TopStackEntry; stackEntry != null; stackEntry = stackEntry.Caller) {
                            if ((stackEntry.Destinations & Destination.File) == Destination.File) {
                                ++count;
                                _logfile.Write(stackEntry.EntryLine); // Changed to ulong in version 6.
                                _logfile.Write((byte)stackEntry.Level);
                                _logfile.Write(stackEntry.Logger.Name);
                                _logfile.Write(stackEntry.MethodName);
                            }
                        }

                        // The FileStackDepth we wrote previously is how the viewer will know how many 
                        // stack entries to read.
                        System.Diagnostics.Debug.Assert(count == threadData.FileStackDepth);
                    }
                }

                if ((flags & DataFlags.LoggerName) != DataFlags.None) {
                    _logfile.Write(logger.Name);
                }

                if ((flags & DataFlags.MethodName) != DataFlags.None) {
                    _logfile.Write(threadData.CurrentFileMethod);
                    threadData.LastMethod = threadData.CurrentFileMethod;
                }

                if ((flags & DataFlags.Message) != DataFlags.None) {
                    _logfile.Write(msg);
                }

                _lastBlock = _curBlock;
                _lastThread = threadData;
                threadData.LastBlock = _curBlock;
                threadData.LastName = Thread.CurrentThread.Name;
                threadData.LastTraceLevel = lineLevel;
                threadData.LastLogger = logger;
            }

            // Called immediately after writing a line of output in circular mode.
            // This may wrap and/or truncate the file, but does not write any output.
            // startPos = the file position of the beginning of the line just written.
            private void ManageCircularPart(long startPos, long startSize) {
                long endPos = _logfile.BaseStream.Position;

                // Truncate the file if we just overwrote the block that extends past max file size, since
                // the viewer can't access that info, it can be arbitrarily large, and it screws things up
                // if another logging session appends output to the file.
                if (_maxBlockPosition >= startPos && _maxBlockPosition < endPos) {
                    //System.Diagnostics.Debug.Print("Last physical block start was overwritten at " + _maxBlockPosition + ".  Truncating file at " + endPos);
                    _maxBlockPosition = 0;
                    _logfile.BaseStream.SetLength(endPos);
                }

                if (endPos >= _maxFilePosition) {
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

                    if (!_everWrapped) {
                        EventLogging.Log("The output file wrapped for the first time: " + FullPath, EventLogging.FirstWrap);
                        _everWrapped = true;

                        // Now that we've wrapped, the file size and time stamp doesn't change when
                        // we write to the middle of the file.  This timer periodically updates
                        // the timestamp so the viewer will load new lines.
                        _staleTimestampTimer = new Timer(new TimerCallback(UpdateFileTime));
                    }
                } else {
                    // Keep track of how many bytes are in this block so we can determine
                    // if it's time to start a new one based on block size.
                    _bytesInBlock += (uint)(endPos - startPos);

                    // Now check if the current block has enough bytes to start a new block.
                    if (_bytesInBlock >= _blockSize) {
                        EndCurrentBlock();
                    }
                }

                if (_everWrapped) {
                    // On XP, the file's LastWriteTime changes automatically when the logger writes to the
                    // file ONLY IF the size changes too.  If the file has wrapped (meaning the size rarely changes),
                    // we "manually" update the LastWriteTime.  Thus, on XP, both properties change until
                    // the file wraps, and then only the LastWriteTime changes.
                    // On Vista, the LastWriteTime doesn't change automatically until the logger closes the 
                    // file (even if the size does change).  However, the size will change until the file wraps, 
                    // then we start "manually" setting the LastWriteTime.  Thus, on Vista, only the size changes 
                    // until the file wraps, then only the LastWriteTime changes.  The viewer monitors both properties.
                    if (_logfile.BaseStream.Length == startSize) {
                        // The line we just wrote did not change the file size.
                        // Use a timer to force the LastWriteTime to change no
                        // later than 250 ms from now so the viewer will notice the file has changed.
                        if (!_timerIsTicking) {
                            _staleTimestampTimer.Change(250, Timeout.Infinite);
                            _timerIsTicking = true;
                        }
                    } else if (_timerIsTicking) {
                        // The file size changed by itself (so to speak), so cancel the timer
                        // until the next line is written.
                        _staleTimestampTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        _timerIsTicking = false;
                    }
                }
            }

            // Called by a worker thread via _staleTimestampTimer to set the LastWriteTime so
            // the viewer will notice the file has changed.
            private void UpdateFileTime(object o) {
                lock (_fileLocker) {
                    if (_timerIsTicking) {
                        try {
                            long writeTime = DateTime.Now.ToFileTime();
                            _staleTimestampTimer.Change(Timeout.Infinite, Timeout.Infinite);
                            _timerIsTicking = false;

                            SetFileTime(_fileHandle, IntPtr.Zero, IntPtr.Zero, ref writeTime);
                        } catch (Exception) {
                            // The file was probably closed.
                            _staleTimestampTimer.Dispose();
                        }
                    }
                }
            }

            private void EndCurrentBlock() {
                ++_curBlock;
                _lastBlockPosition = _curBlockPosition;
                _curBlockPosition = _logfile.BaseStream.Position;
                _bytesInBlock = 0;
            }

            // Start circular logging. Thread-safety provided by callers.
            private void StartCircular(Logger logger, TraceLevel level) {
                if (!CircularStarted) {
                    uint minBlockSize = 10000;
                    uint maxBlocks = 200;
                    uint minNeeded = 2 * minBlockSize; // Ensure there's room for at least two blocks.
                    long bytesLeft = _maxFilePosition - _logfile.BaseStream.Position;

                    if (bytesLeft < minNeeded) {
                        // Not enough room means it's too late to start circular logging.
                        EventLogging.Log("Circular logging would have started, but there was not enough room left in the file: " + FullPath, EventLogging.TooLateForCircular);

                        // Set the circular start thresholds such that we won't
                        // ever try to start circular mode again.
                        _circularStartTime = DateTime.MaxValue;
                        CircularStartSizeKb = 0;
                        return;
                    }

                    // Figure out what block size will be no less than minBlockSize
                    // bytes and yield no more than maxBlocks blocks.
                    _blockSize = (uint)(bytesLeft / maxBlocks + 1);
                    if (_blockSize < minBlockSize) {
                        _blockSize = minBlockSize;
                    }

                    // Meta-log the last line in the non-circular part.  The bytesLeft reported here is slightly 
                    // inaccurate because the message itself consumes some of those bytes.
                    Metalog(logger, level, string.Format("This is the last line before the circular part of the log.  Block size = {0}, bytes left = {1}.", _blockSize, bytesLeft));
                    EventLogging.Log("Circular logging has started for binary file " + FullPath + " with " + bytesLeft + " bytes remaining.  Block size is " + _blockSize, EventLogging.CircularLogStarted);

                    EndCurrentBlock();
                    _positionOfCircularPart = _logfile.BaseStream.Position;

                    // Meta-log first line in circular part.
                    Metalog(logger, level, "This is the first line in the circular part of the log (never wrapped).");
                }
            }
            #endregion
        }
    }
}