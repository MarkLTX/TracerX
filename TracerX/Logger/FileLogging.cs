using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Reflection;

namespace TracerX {
    public partial class Logger {
        /// <summary>
        /// Methods and configuration for logging to a file.
        /// </summary>
        public static class FileLogging {
            #region Public
            /// <summary>
            /// Open the log file using the current value of various properties.
            /// </summary>
            public static bool Open() {
                lock (_fileLocker) {
                    if (IsOpen) {
                        EventLogging.Log("OpenLog called after log file was already opened.", EventLogging.FileAlreadyOpen);
                    } else if (MaxSizeMb == 0) {
                        EventLogging.Log("The log file was not opened because MaxSizeMb was 0 when FileLogging.Open was called.", EventLogging.MaxSizeZero);
                    } else {
                        try {
                            if (!System.IO.Directory.Exists(Directory)) {
                                System.IO.Directory.CreateDirectory(Directory);
                            }

                            InternalOpen();

                            string msg = "The following log file was opened:\n" + FullPath;
                            EventLogging.Log(msg, EventLogging.LogFileOpened);
                        } catch (Exception ex) {
                            string msg = string.Format("The following exception occurred attempting to open the log file\n{0}\n\n{1}", FullPath, ex);
                            EventLogging.Log(msg, EventLogging.ExceptionInOpen);
                            Close();
                        }
                    }

                    return IsOpen;
                }
            }

            /// <summary>
            /// Close the log file.  It cannot be reopened.
            /// </summary>
            public static void Close() {
                System.Diagnostics.Debug.WriteLine("FileLogging.Close was called.");
                lock (_fileLocker) {
                    // Not sure it matters, but close the file streams in the reverse order they were opened.

                    if (_oldArea != null) {
                        _oldArea.Close();
                        _oldArea = null;
                    }

                    if (_logfile != null) {
                        _logfile.Close();
                        _logfile = null;
                    }

                    _blockLocs = null;
                    _lastThread = null;
                }
            }

            /// <summary>
            /// The format version of the file.
            /// </summary>
            public static int FormatVersion { get { return _formatVersion; } }

            /// <summary>
            /// Is the output file currently open?
            /// </summary>
            public static bool IsOpen { get { return _logfile != null; } }

            /// <summary>
            /// Has circular logging started (not necessarily wrapped)?
            /// </summary>
            public static bool CircularStarted { get { return _blockLocs != null; } }

            /// <summary>
            /// Returns true if the file size has exceeded the max size.  Once this becomes
            /// true, future output replaces old output.
            /// </summary>
            public static bool Wrapped { get { return _everWrapped; } }

            /// <summary>
            /// The current size of the output file.
            /// </summary>
            public static uint CurrentSize {
                get {
                    lock (_fileLocker) {
                        if (IsOpen) {
                            return (uint)_logfile.BaseStream.Length;
                        } else {
                            return 0;
                        }
                    }
                }
            }

            /// <summary>
            /// Directory of the output log file.  Environment variables are expanded
            /// when this is set.  
            /// %LOCAL_APPDATA% (not a real environment variable) 
            /// is expanded to the current user's local (i.e. non-roaming) 
            /// application data directory.
            /// %EXEDIR% (not a real environment variable) is expanded to the directory
            /// of the executable.  Other special variables are %COMMON_APPDATA%,
            /// %DESKTOP%, and %MY_DOCUMENTS%.
            /// Attempts to change this property after the log file is open are ignored.
            /// </summary>
            public static string Directory {
                get { return _logDirectory; }
                set {
                    lock (FileLogging._fileLocker) {
                        if (!FileLogging.IsOpen) {
                            try {
                                if (value.Contains("%LOCAL_APPDATA%")) value = value.Replace("%LOCAL_APPDATA%", Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData));
                                if (value.Contains("%LOCALAPPDATA%")) value = value.Replace("%LOCALAPPDATA%", Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData));
                                if (value.Contains("%COMMON_APPDATA%")) value = value.Replace("%COMMON_APPDATA%", Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData));
                                if (value.Contains("%DESKTOP%")) value = value.Replace("%DESKTOP%", Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop));
                                if (value.Contains("%MY_DOCUMENTS%")) value = value.Replace("%MY_DOCUMENTS%", Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments));
                                if (value.Contains("%EXEDIR%")) value = value.Replace("%EXEDIR%", GetAppDir());
                                _logDirectory = Environment.ExpandEnvironmentVariables(value);
                            } catch (Exception ex) {
                                EventLogging.Log("An error occurred while replacing environment variables in FileLogging.Directory value " + value + "\r\n" + ex.Message, EventLogging.ExceptionInLogger);
                            }
                        }
                    }
                }
            }

             private static string _logDirectory = GetDefaultDir();

            // The default output dir for a winforms app is the local AppData dir.
            // For a web app, it's the dir containing the web site.
             private static string GetDefaultDir() {
                 try {
                     // This always returns null in web apps, sometimes returns
                     // null in the winforms desiger.
                     if (Assembly.GetEntryAssembly() == null) {
                         // We might be in a web app, but this will throw an
                         // exception if we're not.
                         return GetWebAppDir();
                     } else {
                         // This generally means we're in a winforms app.
                         return Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                     }
                 } catch (Exception) {
                     // Getting here means we're probably not in a web app.
                     try {
                         return Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                     } catch (Exception) {
                         // Give up. Return something to avoid an exception.
                         return "C:\\";
                     }
                 }
             }

            /// <summary>
            /// The name of the log file within the LogDirectory.  The default is based on the
            /// running executable name.  The extension is always coerced to ".tx1".
            /// Attempts to change this property after the log file is open are ignored.
            /// </summary>
            public static string Name {
                get { return _logFileName; }
                set {
                    lock (FileLogging._fileLocker) {
                        if (!FileLogging.IsOpen) {
                            _logFileName = Path.ChangeExtension(value, ".tx1");
                        }
                    }
                }
            }
            private static string _logFileName = GetAppName() + ".tx1";

            /// <summary>
            /// Full path of the log file.
            /// </summary>
            public static string FullPath {
                get { return Path.Combine(Directory, Name); }
            }

            /// <summary>
            /// Max size of the output log file in megabytes (1 Mb = 2**20 bytes).
            /// Values over 4095 are coerced to 4095.
            /// Attempts to change this property after the log file is open are ignored.
            /// </summary>
            public static uint MaxSizeMb {
                get { return _maxSizeMb; }
                set {
                    lock (FileLogging._fileLocker) {
                        if (!FileLogging.IsOpen) {
                            if (value > 4095) value = 4095;
                            _maxSizeMb = value;
                        }
                    }
                }
            }
            private static uint _maxSizeMb = 10;


            /// <summary>
            /// How many backups of the output log file to keep (max of 9).
            /// Attempts to change this property after the log file is open are ignored.
            /// </summary>
            public static uint Archives {
                get { return _archives; }
                set {
                    lock (FileLogging._fileLocker) {
                        if (!FileLogging.IsOpen) {
                            // Do not support more than 9 archives.
                            if (value > 9) value = 9;
                            if (value < 0) value = 0;
                            _archives = value;
                        }
                    }
                }
            }
            private static uint _archives = 3;

            /// <summary>
            /// Circular logging will start when the log file reaches this size, unless already started.
            /// Set this to 0 to disable this feature.
            /// Attempts to change this value are ignored after circular logging starts.
            /// </summary>
            public static uint CircularStartSizeKb {
                get { return _circularStartSizeKb; }
                set {
                    lock (FileLogging._fileLocker) {
                        if (!FileLogging.CircularStarted) {
                            if (value > 4193300) value = 4193300;
                            _circularStartSizeKb = value;
                        }
                    }
                }
            }
            private static uint _circularStartSizeKb = 300;


            /// <summary>
            /// Circular logging will start when the log file has been opened this many seconds, unless already started.
            /// Set this to 0 to disable this feature.
            /// Attempts to change this value are ignored after circular logging starts.
            /// </summary>
            public static uint CircularStartDelaySeconds {
                get { return _circularStartDelaySeconds; }
                set { FileLogging.SetCircularDelay(value, ref _circularStartDelaySeconds); } // Logger provides thread-safety.
            }
            private static uint _circularStartDelaySeconds = 60;

            /// <summary>
            /// If a password is set before the file is opened, the viewer will
            /// require the user to enter the same password to open the file.
            /// </summary>
            public static string Password {
                set {
                    if (string.IsNullOrEmpty(value)) { 
                        _hasPassword = false; 
                    } else {
                        _passwordHash = value.GetHashCode();
                        _hasPassword = true;
                    }
                }
            }
            private static bool _hasPassword;
            private static int _passwordHash;
            #endregion

            #region Data members
            // The version of the log file format created by this assembly.
            private const int _formatVersion = 5;

            // Object used with the lock keyword to serialize file I/O.
            private static readonly object _fileLocker = new object();

            // The output log file.  Null until OpenLog() succeeds.
            // User output goes through this object.
            private static BinaryWriter _logfile;

            // Handle to the area of the file used to track the oldest block
            // in the circular part of the log file.  This allows the viewer
            // to find the first line to display from the circular part.
            private static BinaryWriter _oldArea;

            // How big to make the old area.  Should be a multiple of 6.
            private const uint _oldAreaSize = 600; // 100 6-byte records.

            // File position of the old area.
            private static long _oldAreaStart;

            // Counter of records written to the old area.
            // This is allowed to wrap which is safe as long as the max value
            // of the type exceeds the number of records that fit the area.
            private static ushort _oldAreaWrites;

            // When the file was opened.
            private static DateTime _openTimeUtc;

            // Time of last logged line;
            private static DateTime _curTime = DateTime.MinValue;

            // The first line logged on or after this Time will start the circular log.
            private static DateTime _circularStartTime;

            // Have we ever wrapped?
            private static bool _everWrapped;

            // Count of lines written.
            private static uint _lineCnt = 0;

            // Current block being written. 1 means non-circular area.
            // Incremented whenever a new block is started.
            private static uint _curBlock = 1;

            // The block the previous line was written to (copied from _curBlock).
            // 0 means nothing has been written.
            private static uint _lastBlock = 0;

            // File positions of circular blocks.  Allocated when circular logging starts.
            // Also, the basis of CircularStarted.
            private static long[] _blockLocs;

            // The thread the last line was written by.
            private static ThreadData _lastThread;

            // Indices into _blockLocs so we can find the file position of the oldest and current blocks.
            private static int _oldBlockNdx, _curBlockNdx;

            // Minimum number of bytes written to each circular block.
            // If we write less than this per block, _blockLocs may not have enough elements.
            private static uint _blockSize;

            // Number of bytes written to the current block.
            private static uint _bytesInBlock;

            // File position where the circular part of the log physically starts.
            private static long _positionOfCircularPart;

            // File position of the block that exends past max file size.
            // 0 means there is no such block.
            // When this file position is overwritten, the file is truncated since
            // all subsequent data in the file is ignored by the viewer.
            private static long _maxBlockPosition;
            #endregion

            #region Methods
		    #region Open log
            // This either opens the originally specified log file, opens an
            // alternate log file, or throws an exception.
            private static void InternalOpen() {
                // Use this to generate alternate file names A-Z.
                char c = 'A';

                while (_logfile == null) {
                    // Attempt to rename the output file to determine if it already exists
                    // and if another process is using it.  Unfortunately, there is not an
                    // explicit exception for "in use by another process".

                    string tempPath = FullPath + ".tempname";

                    try {
                        if (File.Exists(tempPath)) File.Delete(tempPath);
                        File.Move(FullPath, tempPath);
                    } catch (System.IO.FileNotFoundException) {
                        // Not a problem.  The output file doesn't already exist.
                        tempPath = null;
                    } catch (System.IO.IOException) {
                        // Assume file is in use, try next alternate name.
                        if (c > 'Z') {
                            // That was the last chance.  Rethrow the exception to
                            // end the loop and cause the exception to be logged.
                            throw;
                        } else {
                            // Try the next alternative file name, up to Z.
                            string bareFileName = Path.GetFileNameWithoutExtension(Name);
                            Name = string.Format("{0}({1})", bareFileName, c);
                            ++c;
                            continue;
                        }
                    }

                    // Getting here means the output file was successfully renamed to tempPath
                    // or didn't already exist.  Either is good, as we can now open the
                    // output file without deleting the previous version of it.
                    // Granting a FileShare of ReadWrite seems to work better than just Write.
                    _logfile = new BinaryWriter(new FileStream(FullPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
                    _openTimeUtc = DateTime.UtcNow;

                    if (CircularStartDelaySeconds == 0) {
                        _circularStartTime = DateTime.MaxValue;
                    } else {
                        _circularStartTime = _openTimeUtc.AddSeconds(CircularStartDelaySeconds);
                    }

                    WritePreamble();
                    LogStandardData();
                    Archive(tempPath);
                }
            }

            private static void LogStandardData() {
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
            private static void WritePreamble() {
                DateTime local = _openTimeUtc.ToLocalTime();
                Version asmVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                // Format version should be the first
                // item in the file so the viewer knows how to read the rest.
                _logfile.Write(_formatVersion);
                _logfile.Write(_hasPassword); // Added in version 5.
                if (_hasPassword) _logfile.Write(_passwordHash); // Added in version 4.
                _logfile.Write(asmVersion.ToString()); // Added in file format version 3.
                _logfile.Write(MaxSizeMb);
                _logfile.Write(_openTimeUtc.Ticks);
                _logfile.Write(local.Ticks);
                _logfile.Write(local.IsDaylightSavingTime());
                _logfile.Write(TimeZone.CurrentTimeZone.StandardName);
                _logfile.Write(TimeZone.CurrentTimeZone.DaylightName);
                _logfile.Flush();
            }

            // Manages the archive files (*_1, *_2, etc.).
            // Parameter tempPath is what the old output file was renamed to as a test.
            // It must become the _1 file.
            private static void Archive(string tempPath) {
                string bareFileName = Path.GetFileNameWithoutExtension(Name);
                string bareFilePath = Path.Combine(Directory, bareFileName);
                string oldFile = string.Empty;
                string newFile = string.Empty;

                try {
                    // Delete old archive files that exceed the specified number of archives.
                    // Keep in mind we do not support more than 9 archives.
                    for (uint i = Archives; i < 10; ++i) {
                        oldFile = string.Format("{0}_{1}.tx1", bareFilePath, i);
                        if (File.Exists(oldFile)) {
                            File.Delete(oldFile);
                        }
                    }
                } catch (Exception ex) {
                    string msg = string.Format("An exception occurred while deleting the old log file\n{0}\n\n{1}", oldFile, ex);
                    EventLogging.Log(msg, EventLogging.ExceptionInArchive);
                }

                try {
                    // Now rename the remaining archive files, shifting _1 to _2, _2 to _3, etc.
                    // Actually, we go in reverse order.
                    if (Archives > 0) {
                        newFile = string.Format("{0}_{1}.tx1", bareFilePath, Archives);
                        for (uint i = Archives - 1; i > 0; --i) {
                            oldFile = string.Format("{0}_{1}.tx1", bareFilePath, i);
                            if (File.Exists(oldFile)) {
                                File.Move(oldFile, newFile);
                            }
                            newFile = oldFile;
                        }

                        // Finally, archive the most recent log file, if it exists.
                        if (tempPath != null) {
                            oldFile = tempPath; // oldFile is referenced in the Exception handler.
                            File.Move(oldFile, newFile);
                        }
                    }
                } catch (Exception ex) {
                    string msg = string.Format("An exception occurred while renaming the old log file\n{0}\nto\n{1}\n\n{2}", oldFile, newFile, ex);
                    EventLogging.Log(msg, EventLogging.ExceptionInArchive);
                }
            }
            #endregion

            // If circular logging hasn't already started, accept the new number of seconds
            // and update the configVal.
            private static void SetCircularDelay(uint seconds, ref uint configVal) {
                lock (_fileLocker) {
                    if (!CircularStarted) {
                        configVal = seconds;
                        if (IsOpen) {
                            if (seconds == 0) {
                                _circularStartTime = DateTime.MaxValue;
                            } else {
                                _circularStartTime = _openTimeUtc.AddSeconds(seconds);
                            }
                        }
                    }
                }
            }

            // If DateTime.UtcNow differs from _curTime, update _curtime
            // and return true; else return false.
            private static bool IsNewTime() {
                DateTime now = DateTime.UtcNow;
                if (now == _curTime) {
                    return false;
                } else {
                    _curTime = now;
                    return true;
                }
            }

            // Log the entry (start) of a method call.
            internal static void LogEntry(ThreadData threadData, StackEntry stackEntry) {
                // stackEntry is not yet on the stack.
                // Remember the line number where the MethodEntry flag is written so
                // we can write it into the log when the method exits.
                stackEntry.EntryLine = WriteLine(DataFlags.MethodEntry, threadData, stackEntry.Logger, stackEntry.Level, null, false);
            }

            // Log the exit of a method call.
            internal static void LogExit(ThreadData threadData, StackEntry stackEntry) {
                // stackEntry is still on the stack.
                WriteLine(DataFlags.MethodExit, threadData, stackEntry.Logger, stackEntry.Level, null, false);
            }

            // Log a string message.
            internal static void LogMsg(Logger logger, ThreadData threadData, TraceLevel lineLevel, string msg) {
                WriteLine(DataFlags.Message, threadData, logger, lineLevel, msg, false);
            }

            // Log a string message.
            private static void Metalog(Logger logger, TraceLevel lineLevel, string msg) {
                WriteLine(DataFlags.Message, ThreadData.CurrentThreadData, logger, lineLevel, "TracerX: " + msg, true);
            }

            // Determine what data needs to be written based on Flags, 
            // whether circular logging has or should be started,
            // and whether we're starting a new circular block.  
            // Write the output to the file.  Manage the circular part of the log.
            // Return the line number just written.
            private static uint WriteLine(DataFlags flags, ThreadData threadData, Logger logger, TraceLevel lineLevel, string msg, bool recursive) {
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
                                (CircularStartSizeKb > 0 && _logfile.BaseStream.Position >= CircularStartSizeKb << 10))) //
                            {
                                // This will increment _curBlock if it starts the circular log.
                                // It can also make a recursive call to this method via Metalog.
                                StartCircular(logger, lineLevel);
                            }

                            // Set bits in Flags that indicate what data should be written for this line.
                            flags = SetDataFlags(flags, threadData, logger, lineLevel);

                            // We need to know the start position of the line we're about
                            // to write to determine if it overwrites the beginning of the oldest block.
                            long startPos = _logfile.BaseStream.Position;

                            // Write the Flags to the file followed by the data the Flags say to log.
                            WriteData(flags, threadData, logger, lineLevel, msg);

                            if (CircularStarted) {
                                ManageCircularPart(startPos);
                            } else if (_logfile.BaseStream.Position >= MaxSizeMb << 20) {
                                // Reaching max file size without being in circular mode means we'll never write to
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
            private static DataFlags SetDataFlags(DataFlags flags, ThreadData threadData, Logger logger, TraceLevel lineLevel) {
                if (_lastBlock != _curBlock) {
                    // The very first line in each block (regardless of thread)
                    // includes the Time in case
                    // this line ends up being the first line due to wrapping.
                    flags |= DataFlags.Time;
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

            // This is what actually writes the output. The Flags parameter specifies what to
            // write.
            private static void WriteData(DataFlags flags, ThreadData threadData, Logger logger, TraceLevel lineLevel, string msg) {
                ++_lineCnt;

                if (CircularStarted) {
                    // In circular mode, we write the line number on every line before the DataFlags.
                    // The viewer finds the last line by finding the line with
                    // a line number that's not 1 more than the previous line.
                    _logfile.Write(_lineCnt);
                }

                _logfile.Write((ushort)flags);

                if ((flags & DataFlags.LineNumber) != DataFlags.None) {
                    _logfile.Write(_lineCnt);
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

                // Before format version 5.
                //if ((flags & DataFlags.StackDepth) != DataFlags.None) {
                //    if ((flags & DataFlags.MethodExit) != DataFlags.None) {
                //        // On MethodExit lines, threadData.FileStackDepth isn't decremented until after the line
                //        // is logged so any meta-logging has the right depth.
                //        // Must write 1 byte here.
                //        _logfile.Write((byte)(threadData.FileStackDepth - 1));
                //    } else {
                //        _logfile.Write(threadData.FileStackDepth);
                //    }
                //}

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
                                _logfile.Write(stackEntry.EntryLine);
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
            // This handles all wrapping and tracking of the oldest block/line so
            // the viewer can figure out where to start reading.
            // The startPos parameter is the file position of the beginning of
            // the line just written.
            private static void ManageCircularPart(long startPos) {
                long endPos = _logfile.BaseStream.Position;

                if (endPos >= MaxSizeMb << 20) {
                    // We just wrote past the max file size.
                    System.Diagnostics.Debug.Print("File position exceeded max size.");

                    // Remember the file location of the block that extends
                    // past the max file size.             
                    _maxBlockPosition = _blockLocs[_curBlockNdx];

                    // Since we've reached or exceeded the max size, it's time to wrap and
                    // start a new block even if the current block only has one line.  

                    // Cause the current block to end and a new block to start.
                    _bytesInBlock = _blockSize;

                    // Wrap back to the beginning of the circular part.
                    _logfile.BaseStream.Position = _positionOfCircularPart;

                    if (!_everWrapped) {
                        EventLogging.Log("The output file wrapped for the first time: " + FullPath, EventLogging.FirstWrap);
                        _everWrapped = true;
                    }
                } else {
                    // Keep track of how many bytes are in this block so we can determine
                    // if it's time to start a new one based on block size.
                    _bytesInBlock += (uint)(endPos - startPos);
                }

                MaybeTruncateFile(startPos, endPos);

                // Now that we're done writing, check if we've overwritten the oldest
                // line in the file and if so find the next oldest line (i.e. beginning
                // of a block).  This is not possible until we've wrapped the file, nor
                // does the function TrackOldestBlock work properly until then.
                if (_everWrapped) {
                    TrackOldestBlock(startPos, endPos);
                }

                // Now check if the current block has enough bytes to start a new block.
                if (_bytesInBlock >= _blockSize) {
                    ++_curBlock;
                    _bytesInBlock = 0;
                    _curBlockNdx = (_curBlockNdx + 1) % _blockLocs.Length;
                    _blockLocs[_curBlockNdx] = _logfile.BaseStream.Position;
                    System.Diagnostics.Debug.Print("_curBlockNdx changed to " + _curBlock + ", file position is " + _blockLocs[_curBlockNdx]);
                }
            }

            // Write the file position of the oldest known line in the circular part to
            // the current slot in the tracking area.
            private static void RecordOldestBlock() {
                System.Diagnostics.Debug.Print("Recording oldest block location = " + _blockLocs[_oldBlockNdx]);
                _oldArea.Write(_oldAreaWrites++);
                _oldArea.Write((uint)_blockLocs[_oldBlockNdx]);
                System.Diagnostics.Debug.Assert(_oldArea.BaseStream.Position <= _oldAreaStart + _oldAreaSize);

                if (_oldArea.BaseStream.Position == _oldAreaStart + _oldAreaSize) {
                    // We're at the end of the tracking area, so reset it to the 
                    // beginning for the next call.
                    _oldArea.BaseStream.Position = _oldAreaStart;
                }
            }

            // Given the starting and ending file positions of a just completed
            // write operation, determine if the current oldest block in the circular part
            // was overwritten and if so find the next one.
            private static void TrackOldestBlock(long startPos, long endPos) {
                System.Diagnostics.Debug.Print("TrackOldestBlock: startPos = " + startPos + ", endPos = " + endPos);

                // Find the oldest block that hasn't been overwritten and record its position in the file
                // unless it's the same as the current oldest block.
                int startNdx = _oldBlockNdx;
                System.Diagnostics.Debug.Print("_blockLocs[" + _oldBlockNdx + "] = " + _blockLocs[_oldBlockNdx]);
                while ((startPos <= _blockLocs[_oldBlockNdx] && endPos > _blockLocs[_oldBlockNdx]) ||
                       _blockLocs[_oldBlockNdx] == 0) //
            {
                    _blockLocs[_oldBlockNdx] = 0; // This location was overwritten and is no longer viable.

                    // Move to the next block in a circular fashion.
                    _oldBlockNdx = (_oldBlockNdx + 1) % _blockLocs.Length;
                    System.Diagnostics.Debug.Print("_blockLocs[" + _oldBlockNdx + "] = " + _blockLocs[_oldBlockNdx]);

                    if (_oldBlockNdx == startNdx) {
                        // We've checked every block without finding one that was not overwritten.
                        // This can happen if the user logs a message at the physical start of the
                        // circular part that overwrites all the known blocks.  Make the start of the
                        // new line the new oldest block.
                        System.Diagnostics.Debug.Print("All old blocks were overwritten.");
                        _blockLocs[_oldBlockNdx] = startPos;
                        RecordOldestBlock();
                        break;
                    }
                }

                if (_oldBlockNdx != startNdx) {
                    // The beginning of the current oldest block was overwritten.
                    // Record the position of the new oldest block.
                    RecordOldestBlock();
                }
            }

            // Truncate the file if we just overwrote the block that extends past max file size, since
            // the viewer can't access that info and it can be arbitrarily large.
            // startPos = starting file position of the record just written to the file.
            // endPos = ending file position (actually just after the last byte written).
            private static void MaybeTruncateFile(long startPos, long endPos) {
                if (_maxBlockPosition >= startPos && _maxBlockPosition < endPos) {
                    System.Diagnostics.Debug.Print("Last physical block was overwritten at " + _maxBlockPosition + ".  Truncating file at " + endPos);
                    _maxBlockPosition = 0;
                    _logfile.BaseStream.SetLength(endPos);
                }
            }

            // Start circular logging. Thread-safety provided by callers.
            private static void StartCircular(Logger logger, TraceLevel level) {
                if (!CircularStarted) {
                    uint minBlockSize = 10000;
                    uint maxBlocks = 200;
                    uint minNeeded = _oldAreaSize + (2 * minBlockSize); // We need room for at least two blocks.
                    long bytesLeft = (MaxSizeMb << 20) - _logfile.BaseStream.Position;

                    if (bytesLeft < minNeeded) {
                        // Not enough room means it's too late to start circular logging.
                        EventLogging.Log("Circular logging would have started, but there was not enough room left in the file: " + FullPath, EventLogging.TooLateForCircular);

                        // Set the circular start thresholds such that we won't
                        // try to start circular mode again.
                        _circularStartTime = DateTime.MaxValue;
                        CircularStartSizeKb = 0;
                        return;
                    }

                    // The amount of space that will be left for actual logging after
                    // the oldArea is reserved.
                    bytesLeft -= _oldAreaSize;

                    // Figure out what block size will be no less than minBlockSize
                    // bytes and yeild no more than maxBlocks blocks.
                    _blockSize = (uint)(bytesLeft / maxBlocks + 1);
                    if (_blockSize < minBlockSize) {
                        _blockSize = minBlockSize;
                        maxBlocks = (uint)(bytesLeft / minBlockSize) + 1; // At least 2.
                    }

                    System.Diagnostics.Debug.Assert(_blockSize >= 10000);
                    System.Diagnostics.Debug.Assert(maxBlocks >= 2);

                    // Meta-log the last line in the non-circular part.
                    Metalog(logger, level, string.Format("This is the last line before the circular part of the log.  Block size = {0}, bytes left = {1}, max blocks = {2}.", _blockSize, bytesLeft, maxBlocks));

                    // Open the stream we will use to write the position of the oldest block to the "old area".
                    // This was observed to fail until the sharing mode of the _logfile handle was changed
                    // from Write to ReadWrite.
                    _oldArea = new BinaryWriter(new FileStream(FullPath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite));
                    EventLogging.Log("Circular logging has started for file " + FullPath + " with " + bytesLeft + " bytes remaining.  Block size is " + _blockSize, EventLogging.CircularLogStarted);

                    // Tell the viewer that the circular log starts here and the next thing in the file
                    // is the size of the oldArea.  We must also write a uint 0 for backward compatibility
                    // with old viewers.
                    _logfile.Write((ushort)DataFlags.CircularStart);
                    _logfile.Write(_oldAreaSize);
                    _logfile.Write((uint)0);

                    // Advance the file position past the "old area" (where the position of the
                    // oldest block in the circular region is tracked).
                    // Advancing the Position past the current end-of-file seems to write 0s, 
                    // which is what we want.
                    _logfile.Flush();
                    _oldArea.BaseStream.Position = _oldAreaStart = _logfile.BaseStream.Position;
                    _logfile.BaseStream.Position = _oldAreaStart + _oldAreaSize;

                    // Initialize the array that tracks the file positions of the blocks.
                    _blockLocs = new long[maxBlocks];
                    _oldBlockNdx = _curBlockNdx = 0;
                    _blockLocs[0] = _positionOfCircularPart = _logfile.BaseStream.Position;
                    ++_curBlock;

                    // Record the location of the first circular block.  In the future, this only happens
                    // when the logger "catches up" to (and overwrites) the last file position recored this way.
                    RecordOldestBlock();

                    // Meta-log first line in circular part.
                    Metalog(logger, level, "This is the first line in the circular part of the log (never wrapped).");
                }
            }    
	        #endregion        
        }
    }
}