using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace TracerX.Viewer {
    // This class reads the log file into Record objects.
    internal class Reader {
        // The minimum and maximum file format versions supported
        // by the viewer/reader.
        private const int _minVersion = 2;
        private const int _maxVersion = 4;

        private static int _lastHash;

        [Browsable(false)]
        public static int MinFormatVersion { get { return _minVersion; } }

        // Stuff read from the preamble.
        public int FormatVersion;
        private string _loggerVersion;
        private int MaxMb;
        public DateTime OpenTimeUtc;
        public DateTime OpenTimeLocal;
        public bool IsDST;
        public string TzStandard;
        public string TzDaylight;

        private string LoggerTimeZone {
            get {
                if (IsDST) {
                    return TzDaylight;
                } else {
                    return TzStandard;
                }
            }
            set { }
        }

        #region Properties for PropertyGrid
        public DateTime Log_CreationTimeInUtcTZ {
            get { return OpenTimeUtc; }
            set { }
        }

        public string Log_CreationTimeInAppsTZ {
            get { return OpenTimeLocal.ToString() + " " + LoggerTimeZone; }
            set { }
        }

        public DateTime Log_CreationTimeInViewersTZ {
            get { 
                return OpenTimeUtc.ToLocalTime();
            }
            set { }
        }

        public string Logger_AssemblyVersion {
            get { return _loggerVersion; }
            set {  }
        }

        public int Logger_FileFormatVersion {
            get { return FormatVersion; }
            set { }
        }

        public int File_MaxMegabytes {
            get { return MaxMb; }
            set { }
        }

        public long File_Size {
            get { return Size; }
            set { }
        }

        public TimeSpan File_Timespan {
            get { return Records_LastTimestamp - OpenTimeUtc; }
            set { }
        }

        public uint Records_LastNumber {
            get { return _recordNumber; }
            set { }
        }

        public DateTime Records_LastTimestamp {
            get { return _time; }
            set { }
        }

        /// <summary>
        /// Number of records lost due to wrapping in the circular part of the log.
        /// </summary>
        public uint Records_LostViaWrapping {
            get { return Records_LastNumber - Records_TotalRead; }
            set { }
        }

        /// <summary>
        /// Number of records read from the file.
        /// </summary>
        public uint Records_TotalRead {
            get { return _recordsRead; }
            set { }
        }
        #endregion

        private uint _recordsRead;
	
        // File size in bytes.
        private long Size;

        // Approximate number of bytes read from the file, to report percent loaded.
        public long BytesRead;

        // Bitmap indicating all TraceLevels found in the file.
        public TraceLevel LevelsFound;

        // These members hold the most recently read data.
        public uint _recordNumber = 0;
        public DateTime _time;
        public int _threadId;
        public string _msg;
        public ThreadInfo _curThread;

        // Keeps track of thread IDs we have found while reading the file.  Each ThreadInfo object
        // stores the "per thread" info that is not usually written to the log when a thread switch occurs.
        private Dictionary<int, ThreadInfo> _foundThreadIds = new Dictionary<int, ThreadInfo>();

        // When refreshing a file, the ThreadObjects from the old file are put here and reused if the same
        // thread IDs are found in the new file.  This is how filtering is persisted across refreshes.
        private Dictionary<int, ThreadObject> _oldThreadIds = new Dictionary<int, ThreadObject>();

        private Dictionary<string, ThreadName> _foundThreadNames = new Dictionary<string, ThreadName>();
        private Dictionary<string, ThreadName> _oldThreadNames = new Dictionary<string, ThreadName>();

        private Dictionary<string, LoggerObject> _foundLoggers = new Dictionary<string, LoggerObject>();
        private Dictionary<string, LoggerObject> _oldLoggers = new Dictionary<string, LoggerObject>();

        private long _circularStartPos;

        [Browsable(false)]
        public BinaryReader FileReader { get { return _fileReader; } }
        private BinaryReader _fileReader;

        public void ReuseFilters() {
            // Save off the old thread IDs and reuse any that are also found in the new file.
            // Do the same for thread names and loggers.
            foreach (ThreadObject threadObject in ThreadObject.AllThreads) _oldThreadIds.Add(threadObject.Id, threadObject);
            foreach (ThreadName threadName in ThreadName.AllThreadNames) _oldThreadNames.Add(threadName.Name, threadName);
            foreach (LoggerObject logger in LoggerObject.AllLoggers) _oldLoggers.Add(logger.Name, logger);
        }

        // Open the file, read the format version and preamble.
        // Return null if an error occurs, such as encountering an unsupported file version.
        public  bool OpenLogFile(string filename) {
            try {
                _fileReader = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                if (_fileReader == null) {
                    MessageBox.Show("Could not open log file " + filename);
                } else {
                    Size = _fileReader.BaseStream.Length;
                    FormatVersion = _fileReader.ReadInt32();

                    if (FormatVersion > _maxVersion || FormatVersion < _minVersion) {
                        MessageBox.Show("The file has a format version of " + FormatVersion + ".  This program only supports format version " + _maxVersion + ".");
                        _fileReader.Close();
                        _fileReader = null;
                    } else  if (FormatVersion == 4 && !PromptUserForPassword()) {
                        _fileReader.Close();
                        _fileReader = null;
                    } else {
                        ReadPreamble();
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Error opening log file '" + filename + "':\n\n" + ex.ToString());
                _fileReader = null;
            }

            return _fileReader != null;
        }

        // Called if the file has a password.  Reads the password hash from the file
        // and prompts the user to enter a matching password.
        // Returns true if the user enters the correct password.
        private bool PromptUserForPassword() {
            int hash = _fileReader.ReadInt32();

            if (hash == _lastHash) {
                // Either the same file is being loaded again (refreshed), 
                // or a different file has the same password.  Either way,
                // don't ask for the same password again.
                return true;
            } else {
                PasswordDialog dlg = new PasswordDialog(hash);
                DialogResult result = dlg.ShowDialog();
                if (result == DialogResult.OK) {
                    _lastHash = hash;
                    return true;
                } else {
                    return false;
                }
            }
        }

        public void CloseLogFile() {
            _fileReader.Close();
            _fileReader = null;
            _curThread = null;
            _foundThreadIds = null;
            _foundThreadNames = null;
            _oldThreadNames = null; 
            _oldThreadIds = null;
            _foundLoggers = null;
            _oldLoggers = null;
        }

        private void ReadPreamble() {
            long ticks;

            if (FormatVersion >= 3) {
                // Logger version was added to the preamble in version 3.
                _loggerVersion = _fileReader.ReadString();
            }
            MaxMb = _fileReader.ReadInt32();
            ticks = _fileReader.ReadInt64();
            OpenTimeUtc = new DateTime(ticks);
            ticks = _fileReader.ReadInt64();
            OpenTimeLocal = new DateTime(ticks);
            IsDST = _fileReader.ReadBoolean();
            TzStandard = _fileReader.ReadString();
            TzDaylight = _fileReader.ReadString();
        }

        private ThreadName FindOrCreateThreadName(string name) {
            ThreadName threadName = null;

            if (!_foundThreadNames.TryGetValue(name, out threadName)) {
                if (!_oldThreadNames.TryGetValue(name, out threadName)) {
                    threadName = new ThreadName();
                    threadName.Name = name;
                }

                _foundThreadNames.Add(name, threadName);
                ThreadName.AllThreadNames.Add(threadName);
            }

            return threadName;
        }

        public  Record ReadRecord() {
            // Read the DataFlags, then the data the Flags indicate is there.
            // Data must be read in the same order it was written (see FileLogging.WriteData).
            try {
                DataFlags flags = GetFlags();

                if (flags == DataFlags.None) {
                    return null;
                }

                long startPos = _fileReader.BaseStream.Position;

                if ((flags & DataFlags.LineNumber) != DataFlags.None) {
                    _recordNumber = _fileReader.ReadUInt32();
                } else if (_circularStartPos == 0) {
                    ++_recordNumber;
                } else {
                    // _recordNumber was incremented by GetFlags.
                }

                if ((flags & DataFlags.Time) != DataFlags.None) {
                    _time = new DateTime(_fileReader.ReadInt64());
                }

                if ((flags & DataFlags.ThreadId) != DataFlags.None) {
                    _threadId = _fileReader.ReadInt32();

                    // Look up or add the entry for this ThreadId.
                    if (!_foundThreadIds.TryGetValue(_threadId, out _curThread)) {
                        // First occurrence of this id.
                        _curThread = new ThreadInfo();

                        if (!_oldThreadIds.TryGetValue(_threadId, out _curThread.Thread)) {
                            _curThread.Thread = new ThreadObject();
                        }

                        _curThread.Thread.Id = _threadId;
                        ThreadObject.AllThreads.Add(_curThread.Thread);
                        _foundThreadIds[_threadId] = _curThread;
                    }
                }

                if ((flags & DataFlags.ThreadName) != DataFlags.None) {
                    // A normal thread's name can only change from null to non-null.  
                    // ThreadPool threads can alternate between null and non-null.
                    // If a thread's name changes from non-null to null, the logger
                    // writes string.Empty for the thread name.  
                    string threadNameStr = _fileReader.ReadString();
                    if (threadNameStr == string.Empty) _curThread.ThreadName = FindOrCreateThreadName("Thread " + _curThread.Thread.Id);
                    else _curThread.ThreadName = FindOrCreateThreadName(threadNameStr);
                } else if (_curThread.ThreadName == null) {
                    _curThread.ThreadName = FindOrCreateThreadName("Thread " + _curThread.Thread.Id);
                }

                if ((flags & DataFlags.TraceLevel) != DataFlags.None) {
                    _curThread.Level = (TracerX.TraceLevel)_fileReader.ReadByte();
                    LevelsFound |= _curThread.Level;
                } 

                if ((flags & DataFlags.StackDepth) != DataFlags.None) {
                    _curThread.Depth = _fileReader.ReadByte();
                } else if ((flags & DataFlags.MethodExit) != DataFlags.None) {
                    --_curThread.Depth;
                }

                if ((flags & DataFlags.LoggerName) != DataFlags.None) {
                    string loggerName = _fileReader.ReadString();
                    if (!_foundLoggers.TryGetValue(loggerName, out _curThread.Logger)) {
                        if (!_oldLoggers.TryGetValue(loggerName, out _curThread.Logger)) {
                            _curThread.Logger = new LoggerObject();
                            _curThread.Logger.Name = loggerName;
                        }

                        _foundLoggers.Add(loggerName, _curThread.Logger);
                        LoggerObject.AllLoggers.Add(_curThread.Logger);
                    }
                }

                if ((flags & DataFlags.MethodName) != DataFlags.None) {
                    _curThread.MethodName = _fileReader.ReadString();
                }

                if ((flags & DataFlags.Message) != DataFlags.None) {
                    _msg = _fileReader.ReadString();
                }

                // The Record ctor sets the members to the values just read for
                // the current or previous record.
                Record data = new Record(flags, this);

                if ((flags & DataFlags.MethodEntry) != DataFlags.None) {
                    // Cause future records to be indented until a MethodExit is encountered.
                    ++_curThread.Depth;
                }

                BytesRead += _fileReader.BaseStream.Position - startPos;

                if (_fileReader.BaseStream.Position >= MaxMb << 20 && _circularStartPos != 0) {
                    // We've read to the max file size in circular mode.  Wrap.
                    _fileReader.BaseStream.Position = _circularStartPos;
                }

                ++_recordsRead;
                return data;
            } catch (Exception) {
                // The exception is either end-of-file or a corrupt file.
                // Either way, we're done.  Returning null tells the caller to give up.
                return null;
            }
        }

        // This is called when reading the circular part of the log to 
        // double check that we have a valid record.
        private static bool FlagsAreValid(DataFlags flags) {
            if ((flags & DataFlags.InvalidOnes) != DataFlags.None) {
                return false;
            }

            if ((flags & DataFlags.CircularStart) != DataFlags.None) {
                // Circular flag should never appear twice.
                return false;
            }

            if ((flags & DataFlags.LineNumber) != DataFlags.None) {
                // LineNumber should never be set in the circular because
                // every record has a record in the circular part includes the line number.
                return false;
            }

            if ((flags & DataFlags.MethodEntry) == DataFlags.MethodEntry &&
                (flags & DataFlags.MethodExit) == DataFlags.MethodExit) {
                // These two should never appear together.
                return false;
            }

            return true;
        }

        // Gets the DataFlags for next record, possibly entering the circular part of the log.
        // Returns DataFlags.None or throws an exception if the last record has been read.
        private DataFlags GetFlags() {
            DataFlags flags = DataFlags.None;
            bool firstCircular = false;

            if (_circularStartPos == 0) {
                // Not in circular part yet
                flags = (DataFlags)_fileReader.ReadUInt16();
                BytesRead += sizeof(DataFlags);

                if ((flags & DataFlags.CircularStart) != DataFlags.None) {
                    BeginCircular(); // Will set _circularStartPos to non-zero.
                    firstCircular = true;
                }
            }

            if (_circularStartPos != 0) {
                // We're in the circular part, where every record starts with its
                // UInt32 record number, before the data flags.
                UInt32 num = _fileReader.ReadUInt32();
                BytesRead += sizeof(UInt32);
                if (firstCircular) {
                    // This is the first chronological record in the circular part,
                    // so accept the record number as-is.
                    _recordNumber = num;
                    flags = (DataFlags)_fileReader.ReadUInt16();
                    BytesRead += sizeof(DataFlags);
                } else {
                    // Num must equal _recordNumber + 1 or we've read past the last record.
                    // If it's a good record, we need to increment _recordNumber.
                    if (num == ++_recordNumber) {
                        flags = (DataFlags)_fileReader.ReadUInt16();
                        BytesRead += sizeof(DataFlags);

                        // There's a slim chance we a read random data that contained the expected
                        // value. Therefore, also check for invalid flags.
                        if (!FlagsAreValid(flags)) {
                            flags = DataFlags.None;
                        }
                    } else {
                        // We've already read the last (newest) record, and now we're reading garbage.
                        flags = DataFlags.None;
                    }
                }
            }

            return flags;
        }

        // Called immediately after finding the CircularStart marker.
        private void BeginCircular() {
            // Used to track total bytes read.
            long temp = _fileReader.BaseStream.Position;

            // The oldArea contains data about the position of the oldest record in the circular part.
            // Start by getting the size of the old area.
            uint oldAreaSize = _fileReader.ReadUInt32();
            uint unusedData = _fileReader.ReadUInt32(); 
            long start = _fileReader.BaseStream.Position;

            // Set the file position to that of the oldest record.  The caller will
            // begin reading there.
            _fileReader.BaseStream.Position = FindLastFilePos(oldAreaSize);

            // Remember the file position of the first physical record in the circular part.
            _circularStartPos = start + oldAreaSize;
            BytesRead += _circularStartPos - temp;
        }

        // Starting at the current file position, there is an area of the specified size
        // containing a series of 6-byte records.  Each record consists of a uint16 counter
        // followed by an UInt32 file position.  Use the counters to find the last record
        // written and return the corresponding file position.  This area was written to in
        // a circular fashion allowing the counter to wrap.
        private long FindLastFilePos(uint areaSize) {
            long stopPos = _fileReader.BaseStream.Position + areaSize;
            UInt16 curNum, lastNum = _fileReader.ReadUInt16();
            UInt32 curVal, lastVal = _fileReader.ReadUInt32();
            Debug.Print("lastNum = " + lastNum + ", lastVal = " + lastVal);

            while (_fileReader.BaseStream.Position != stopPos) {
                curNum = _fileReader.ReadUInt16();
                curVal = _fileReader.ReadUInt32();

                if (curNum == lastNum + 1) {
                    lastNum = curNum;
                    lastVal = curVal;
                    Debug.Print("lastNum = " + lastNum + ", lastVal = " + lastVal);
                } else {
                    Debug.Print("curNum = " + curNum + ", curVal = " + curVal);
                    break;
                }
            }

            return lastVal;
        }
    }

    // Used only by Reader.
    internal class ThreadInfo {
        public ThreadObject Thread ;
        public ThreadName ThreadName;
        public LoggerObject Logger;
        public string MethodName;
        public TraceLevel Level;
        public byte Depth;
    }
}
