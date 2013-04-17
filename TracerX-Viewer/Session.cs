using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TracerX.Viewer;
using System.Diagnostics;
using System.Security.Cryptography;
namespace TracerX.Viewer
{

    // This class manages the collection of Sessions for the viewer.
    internal static class SessionObjects
    {

        // Lock this when accessing the collection.
        public static object Lock = new object();

        // List of all Sessions.
        public static List<Reader.Session> AllSessionObjects = new List<Reader.Session>();

        public static void IncrementInvisibleCount()
        {
            lock (Lock)
            {
                if (++_invisibleCount == 1)
                {
                    OnAllVisibleChanged();
                }
            }
        }

        public static void DecrementInvisibleCount()
        {
            lock (Lock)
            {
                if (--_invisibleCount == 0)
                {
                    OnAllVisibleChanged();
                }
            }
        }

        // How many Sessions are invisible (filtered out)?
        private static int _invisibleCount;

        // Count the invisible Sessions in AllSessionObjects, set the internal count (_invisibleCount) accordingly, 
        // and raise the AllVisibleChanged event if the new count differs from the current count.
        public static void RecountSessions()
        {
            lock (Lock)
            {
                int oldCount = _invisibleCount;
                _invisibleCount = 0;

                foreach (Reader.Session s in AllSessionObjects)
                {
                    if (!s.Visible) ++_invisibleCount;
                }

                if (_invisibleCount != oldCount)
                {
                    if (_invisibleCount == 0 || oldCount == 0)
                    {
                        OnAllVisibleChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Are all threads visible?
        /// </summary>
        public static bool AllVisible { get { lock (Lock) return _invisibleCount == 0; } }

        /// <summary>
        /// Event called when AllVisible changes.
        /// </summary>
        public static event EventHandler AllVisibleChanged;

        public static void HideAllSessions()
        {
            lock (Lock)
            {
                foreach (Reader.Session s in AllSessionObjects)
                {
                    s.Visible = false;
                }
            }
        }

        public static void ShowAllSessions()
        {
            lock (Lock)
            {
                foreach (Reader.Session s in AllSessionObjects)
                {
                    s.Visible = true;
                }
            }
        }

        private static void OnAllVisibleChanged()
        {
            if (AllVisibleChanged != null)
            {
                AllVisibleChanged(null, null);
            }
        }
    }


    partial class Reader
    {
        // A "session" is created each time the logger opens the output file.  Each file has
        // at least one session plus one session for every time the file is opened in append mode.
        internal class Session : IFilterable
        {
            // Stuff read from the preamble.
            public Guid FileGuid { get; private set; }
            public string LoggersAssemblyVersion { get; private set; }
            public int MaxKb { get; private set; }
            public DateTime CreationTimeUtc { get; private set; }
            public DateTime CreationTimeLoggersTZ { get; private set; }
            private bool IsDST;
            private string _tzStandard;
            private string _tzDaylight;
            private long _maxFilePos;

            public string LoggersTimeZone
            {
                get
                {
                    if (IsDST)
                    {
                        return _tzDaylight;
                    }
                    else
                    {
                        return _tzStandard;
                    }
                }
            }

            // These members hold the most recently read data.
            public ulong LastRecordNum = 0;
            public DateTime LastRecordTimeUtc { get; private set; }
            public int _threadId;
            public string _msg;
            public ReaderThreadInfo _curThread;

            public bool InCircularPart
            {
                get { return _circularStartPos != 0; }
            }

            /// <summary>
            /// This is a list of method entry/exit records generated to replace entry/exit records
            /// that were lost in the circular part of the log.
            /// </summary>
            public List<Record> GeneratedRecords = new List<Record>();
            public Record LastNonCircularRecord;

            public ulong RecordsRead;

            // File position of last successfully read record.
            private long _lastRecordPos;
            private uint _lastBlockNum;
            private DataFlags _lastDataFlags;

            private long _circularStartPos;
            private long _firstRecordPos;

            public long SessionStartPos;

            // File position where the next session might start. Basically,
            // the end of the current session determined by reading the last
            // physical record in the circular part of the session.
            private long _possibleNextSessionPos;

            private BinaryReader _fileReader { get { return _reader._fileReader; } }
            private Reader _reader;
            public int MaxThreadID;
            public int Index;

            public Session(Reader reader)
            {
                _reader = reader;
            }

            #region IFilterable Members

            // Session Name is really just a counter.
            public string Name { get; set; }

            public TracerX.Forms.ColorRulesDialog.ColorPair Colors { get; set; }

            public bool Visible
            {
                get
                {
                    return _visible;
                }

                set
                {
                    if (_visible != value)
                    {
                        _visible = value;

                        // Track the number of filtered (invisible) sessions.
                        if (_visible)
                        {
                            SessionObjects.DecrementInvisibleCount();
                        }
                        else
                        {
                            SessionObjects.IncrementInvisibleCount();
                        }
                    }
                }
            }
            private bool _visible = true;

            #endregion

            // Used for displaying session number in properties dialog.
            public override string ToString()
            {
                return (Index + 1).ToString();
            }

            public void ReadPreamble(long filePos)
            {
                long ticks;

                _fileReader.BaseStream.Position = filePos;
                SessionStartPos = filePos;

                if (_reader.FormatVersion >= 3)
                {
                    // Logger version was added to the preamble in version 3.
                    LoggersAssemblyVersion = _fileReader.ReadString();
                }

                if (_reader.FormatVersion >= 7)
                {
                    MaxKb = _fileReader.ReadInt32();
                }
                else
                {
                    // Convert Mb to Kb.
                    MaxKb = _fileReader.ReadInt32() << 10;
                }

                if (_reader.FormatVersion >= 6)
                {
                    // These were added in version 6.
                    _maxFilePos = _fileReader.ReadInt64();
                    FileGuid = new Guid(_fileReader.ReadBytes(16));
                }
                else
                {
                    // Prior to file version 6, there was no "append" feature so the max size 
                    // for the session was also the max size for the file.  
                    _maxFilePos = MaxKb << 10;
                }

                ticks = _fileReader.ReadInt64();
                CreationTimeUtc = new DateTime(ticks);
                ticks = _fileReader.ReadInt64();
                CreationTimeLoggersTZ = new DateTime(ticks);
                IsDST = _fileReader.ReadBoolean();
                _tzStandard = _fileReader.ReadString();
                _tzDaylight = _fileReader.ReadString();

                LastRecordTimeUtc = CreationTimeUtc;
                _firstRecordPos = _fileReader.BaseStream.Position;
            }

            // Reads a WHOLE record, including data flags
            public Record ReadRecord()
            {
                // Read the DataFlags, then the data the DataFlags indicate is there.
                // Data must be read in the same order it was written (see FileLogging.WriteData in the logger).
                try
                {
                    DataFlags flags;
                    long thisRecordPos = _fileReader.BaseStream.Position;
                    ulong thisRecordNum;
                    uint thisBlockNum = 0;

                    if (_reader.FormatVersion < 6)
                    {
                        flags = GetFlags(out thisRecordNum);

                        if (flags == DataFlags.None)
                        {
                            // No more data in this session.
                            return null;
                        }
                    }
                    else
                    {
                        flags = GetFlags6(out thisRecordPos, out thisRecordNum, out thisBlockNum);

                        if (flags == DataFlags.None)
                        {
                            // No more data in this session, but
                            // it's possible a new session was appended to the physical end of the file
                            // while we were reading from the middle of the circular part.

                            if (_possibleNextSessionPos != 0)
                            {
                                if (_fileReader.BaseStream.Length > _possibleNextSessionPos + 2)
                                {
                                    _fileReader.BaseStream.Position = _possibleNextSessionPos;
                                    DataFlags possibleSession = (DataFlags)_fileReader.ReadInt16();
                                    if (possibleSession == DataFlags.NewSession)
                                    {
                                        _reader._nextSessionPos = _fileReader.BaseStream.Position;
                                        _possibleNextSessionPos = 0;
                                    }
                                    else
                                    {
                                        // I think this means we were reading within the last physical block of the current
                                        // session when the logger truncated the file and then appended a new session.
                                        // We should probably display a message suggesting that the user reload the file.
                                    }
                                }
                            }

                            return null;
                        }
                    }

                    Record record = ReadRecordData(flags, thisRecordNum, false);

                    if (!InCircularPart) LastNonCircularRecord = record;
                    _lastRecordPos = thisRecordPos;
                    _lastBlockNum = thisBlockNum;
                    _lastDataFlags = flags;
                    LastRecordNum = thisRecordNum;
                    ++RecordsRead;
                    return record;
                }
                catch (Exception)
                {
                    // The exception is either from reading past the physical end of the file,
                    // or reading a corrupted file.
                    // Either way, we're done.  Returning null tells the caller to give up.
                    return null;
                }
            }

            // This inserts an exit record for every entry record in the non-circular part of the log
            // whose corresponding exit record was lost due to wrapping. It also inserts an entry
            // record for every exit record in the circular part of the log whose corresponding entry
            // record was lost due to wrapping.
            public int InsertMissingRecords(List<Record> records)
            {
                int result = 0;

                if (GeneratedRecords.Count > 0 && LastNonCircularRecord != null)
                {
                    result = GeneratedRecords.Count;
                    Record firstCircularRecord = records[LastNonCircularRecord.Index + 1];

                    // Set certain fields of the generated records based on the last non-circular record.
                    ulong msgNum = LastNonCircularRecord.MsgNum;
                    foreach (Record missingRec in GeneratedRecords)
                    {
                        missingRec.MsgNum = ++msgNum;
                        missingRec.Time = LastNonCircularRecord.Time;
                    }

                    records.InsertRange(LastNonCircularRecord.Index + 1, GeneratedRecords);

                    // The Index of each inserted and subsequent record must be adjusted due to the insertion.
                    for (int i = LastNonCircularRecord.Index + 1; i < records.Count; ++i)
                    {
                        records[i].Index = i;
                    }

                    // If more generated records are inserted later, they should be inserted
                    // after the ones we just inserted.
                    LastNonCircularRecord = GeneratedRecords[GeneratedRecords.Count - 1];

                    GeneratedRecords = new List<Record>();
                }

                return result;
            }

            // Reads the rest of a record after GetFlags() has read the flags.
            // TODO: This really shouldn't modify any class members until it completes successfully.
            private Record ReadRecordData(DataFlags flags, ulong thisRecordNum, bool reReading)
            {
                long startPos = _fileReader.BaseStream.Position;

                if ((flags & DataFlags.Time) != DataFlags.None)
                {
                    LastRecordTimeUtc = new DateTime(_fileReader.ReadInt64());
                }

                if ((flags & DataFlags.ThreadId) != DataFlags.None)
                {
                    ReadThreadID();
                }

                if ((flags & DataFlags.ThreadName) != DataFlags.None)
                {
                    // A normal thread's name can only change from null to non-null.
                    // ThreadPool threads can alternate between null and non-null.
                    // If a thread's name changes from non-null to null, the logger
                    // writes string.Empty for the thread name.

                    string threadNameStr;

                    if (Reader.Key == null)
                    {
                        threadNameStr = _fileReader.ReadString();
                    }
                    else
                    {
                        threadNameStr = Decrypt();
                    }

                    if (threadNameStr == string.Empty) _curThread.ThreadName = _reader.FindOrCreateThreadName("Thread " + _curThread.Thread.Id);
                    else _curThread.ThreadName = _reader.FindOrCreateThreadName(threadNameStr);
                }
                else if (_curThread.ThreadName == null)
                {
                    _curThread.ThreadName = _reader.FindOrCreateThreadName("Thread " + _curThread.Thread.Id);
                }

                if ((flags & DataFlags.TraceLevel) != DataFlags.None)
                {
                    _curThread.Level = (TracerX.TraceLevel)_fileReader.ReadByte();
                    _reader.LevelsFound |= _curThread.Level;
                }

                if ((flags & DataFlags.StackDepth) != DataFlags.None)
                {
                    _curThread.Depth = _fileReader.ReadByte();

                    if (InCircularPart)
                    {
                        ReadStack();
                    }
                }

                // Starting in format version 5, the viewer decrements the depth on MethodExit
                // lines even if it was included on the line.
                if ((flags & DataFlags.MethodExit) != DataFlags.None && !reReading)
                {
                    --_curThread.Depth;
                }

                if ((flags & DataFlags.LoggerName) != DataFlags.None)
                {
                    string loggerName;

                    if (Reader.Key == null)
                    {
                        loggerName = _fileReader.ReadString();
                    }
                    else
                    {
                        loggerName = Decrypt();
                    }

                    _curThread.Logger = _reader.GetLogger(loggerName);
                }

                if ((flags & DataFlags.MethodName) != DataFlags.None)
                {
                    string methodName;

                    if (Reader.Key == null)
                    {
                        methodName = _fileReader.ReadString();
                    }
                    else
                    {
                        methodName = Decrypt();
                    }

                    _curThread.MethodName = _reader.GetMethod(methodName);
                }

                if ((flags & DataFlags.Message) != DataFlags.None)
                {
                    if (Reader.Key == null)
                    {
                        _msg = _fileReader.ReadString();
                    }
                    else
                    {
                        _msg = Decrypt();
                    }
                }

                // Construct the Record before incrementing depth.
                Record record = new Record(flags, thisRecordNum, LastRecordTimeUtc, _curThread, this, _msg);

                if (!reReading)
                {
                    if ((flags & DataFlags.MethodEntry) != DataFlags.None)
                    {
                        // Cause future records to be indented until a MethodExit is encountered.
                        ++_curThread.Depth;

                        // In format version 5+, we keep track of the call stack
                        // by "pushing" MethodEntry records and "popping" MethodExit records
                        if (_reader.FormatVersion >= 5)
                        {
                            _curThread.Push(record);
                        }
                    }
                    else if (_reader.FormatVersion >= 5 && (flags & DataFlags.MethodExit) != DataFlags.None)
                    {
                        _curThread.Pop();
                    }

                    _reader.BytesRead += _fileReader.BaseStream.Position - startPos;
                }

                if (InCircularPart && _fileReader.BaseStream.Position >= _maxFilePos)
                {
                    // We've read to the point where the log wraps.  
                    Wrap();
                }

                return record;
            }

            private static AesCryptoServiceProvider aesProvider;

            // Decrypts the next string in the file.
            private string Decrypt()
            {
                //try
                {
                    int msgBytesLength = _fileReader.ReadInt32(); // Number of bytes in original cleartext

                    if (msgBytesLength == 0)
                    {
                        return "";
                    }
                    else
                    {
                        int cipherlength = msgBytesLength;   // Number of ciphter bytes, including encrypted padding.

                        // If padding is required, we add our own.
                        if (cipherlength % 16 != 0)
                        {
                            cipherlength += 16 - cipherlength % 16;
                        }

                        Debug.Assert(cipherlength % 16 == 0);
                        Debug.Assert(cipherlength > 0);

                        byte[] cipherBytes = _fileReader.ReadBytes(cipherlength);

                        if (aesProvider == null)
                        {
                            aesProvider = new AesCryptoServiceProvider();
                            aesProvider.Padding = PaddingMode.None; // We do our own padding.
                        }

                        ICryptoTransform CryptoTransform = aesProvider.CreateDecryptor(Reader.Key, Reader.Key);
                        MemoryStream memStream = new MemoryStream(cipherBytes);
                        CryptoStream rStream = new CryptoStream(memStream, CryptoTransform, CryptoStreamMode.Read);
                        byte[] decryptedBytes = new byte[cipherlength];
                        rStream.Read(decryptedBytes, 0, cipherlength);
                        return Encoding.UTF8.GetString(decryptedBytes, 0, msgBytesLength);
                    }
                }
                //catch (Exception ex)
                //{
                //    string msg = "TracerX decryption error: " + ex.ToString();
                //    return msg;
                //}
            }


            private void ReadStack()
            {
                ExplicitStackEntry[] trueStack = null;

                if (_curThread.Depth > 0)
                {
                    // In format version 5, we began logging each thread's current call
                    // stack on the thread's first line in each block (i.e. when the
                    // StackDepth flag is set). This is the thread's true call stack at
                    // this point in the log. It reflects MethodEntry and MethodExit
                    // records that may have been lost when the log wrapped (as well
                    // as those that weren't lost).

                    trueStack = new ExplicitStackEntry[_curThread.Depth];
                    for (int i = 0; i < _curThread.Depth; ++i)
                    {
                        ExplicitStackEntry entry = new ExplicitStackEntry();
                        if (_reader.FormatVersion < 6)
                        {
                            entry.EntryLineNum = _fileReader.ReadUInt32(); // Changed from uint to ulong in version 6
                        }
                        else
                        {
                            // _reader.FormatVersion >= 6
                            entry.EntryLineNum = _fileReader.ReadUInt64(); // Changed from uint to ulong in version 6
                        }
                        entry.Level = (TracerX.TraceLevel)_fileReader.ReadByte();

                        if (Reader.Key == null)
                        {
                            entry.Logger = _reader.GetLogger(_fileReader.ReadString());
                            entry.Method = _reader.GetMethod(_fileReader.ReadString());
                        }
                        else
                        {
                            entry.Logger = _reader.GetLogger(Decrypt());
                            entry.Method = _reader.GetMethod(Decrypt());
                        }

                        entry.Depth = (byte)(_curThread.Depth - i - 1);
                        trueStack[i] = entry;
                    }
                }

                _curThread.MakeMissingRecords(trueStack, GeneratedRecords, this);
            }

            private void ReadThreadID()
            {
                _threadId = _fileReader.ReadInt32() + _reader._maxThreadIDFromPrevSession;
                if (_threadId > MaxThreadID) MaxThreadID = _threadId;

                // Look up or add the entry for this ThreadId.
                if (!_reader._foundThreadIds.TryGetValue(_threadId, out _curThread))
                {
                    // First occurrence of this id.
                    _curThread = new ReaderThreadInfo();

                    if (!_reader._oldThreadIds.TryGetValue(_threadId, out _curThread.Thread))
                    {
                        _curThread.Thread = new ThreadObject();
                    }

                    _curThread.Thread.Id = _threadId;

                    lock (ThreadObjects.Lock)
                    {
                        ThreadObjects.AllThreadObjects.Add(_curThread.Thread);
                    }

                    _reader._foundThreadIds[_threadId] = _curThread;
                }
            }

            // Called after reading the last physical record in the circular part of the file.
            private void Wrap()
            {
                // Support for multi-session logs was added in version 6.
                if (_reader.FormatVersion >= 6)
                {
                    // Before wrapping, check if there is another session appended to this one.

                    if (_fileReader.BaseStream.Length > _fileReader.BaseStream.Position + 2)
                    {
                        // There is more data in the file following the last physical record in 
                        // the current session.  It has to be a DataFlags.NewSession or 
                        // something is wrong.
                        var dataFlags = (DataFlags)_fileReader.ReadInt16();

                        Debug.Assert(dataFlags == DataFlags.NewSession);

                        if (dataFlags == DataFlags.NewSession)
                        {
                            // Remember the file position of the appended session so we
                            // can return to it after reading the rest of the current session.
                            _reader._nextSessionPos = _fileReader.BaseStream.Position;
                            _possibleNextSessionPos = 0;
                        }
                    }
                    else
                    {
                        // The logger might reopen the file in append mode while we're reading
                        // the rest of this session, so remember this file position so we can
                        // check for an appended session later.
                        _possibleNextSessionPos = _fileReader.BaseStream.Position;
                    }
                }

                // Wrap back to the beginning of the circular part of the file
                // and continue reading until we run out of properly formatted data.
                _fileReader.BaseStream.Position = _circularStartPos;
            }

            // This is called when reading the circular part of the log to 
            // double check that we have a valid record.
            private bool FlagsAreValid(DataFlags flags)
            {
                // InvalidOnes are the bits that must always be 0 (record is
                // invalid if any are 1).
                if ((flags & DataFlags.InvalidOnes) != 0)
                {
                    return false;
                }

                // MethodEntry and MethodExit should never appear together.
                if ((flags & DataFlags.MethodEntry) != 0 &&
                    (flags & DataFlags.MethodExit) != 0)
                {
                    return false;
                }

                if (_reader.FormatVersion < 6)
                {
                    // Circular flag should never appear in the circular part
                    if (InCircularPart && (flags & DataFlags.CircularStart) != 0)
                    {
                        return false;
                    }

                    if ((flags & DataFlags.NewSession) != 0)
                    {
                        return false;
                    }
                }
                else
                {
                    if ((flags & DataFlags.BlockStart) != 0)
                    {
                        // Certain bits must always be set with BlockStart.
                        const DataFlags mustBeSet = DataFlags.Time | DataFlags.StackDepth | DataFlags.MethodName | DataFlags.TraceLevel | DataFlags.ThreadId | DataFlags.LoggerName;

                        if ((flags & mustBeSet) != mustBeSet)
                        {
                            return false;
                        }
                    }

                    // If NewSession bit is set, it must be the only one.
                    if (((flags & DataFlags.NewSession) != 0) && (flags != DataFlags.NewSession))
                    {
                        return false;
                    }
                }

                return true;
            }

            // Gets the DataFlags for next record, possibly entering the circular part of the log.
            // Returns DataFlags.None or throws an exception if the last record has been read.
            // Used in file format version 5-.
            private DataFlags GetFlags(out ulong thisRecNum)
            {
                DataFlags flags = DataFlags.None;
                bool firstCircularRecord = false;

                thisRecNum = LastRecordNum + 1; // for now

                if (_circularStartPos == 0)
                {
                    // Not in circular part yet
                    flags = (DataFlags)_fileReader.ReadUInt16();
                    _reader.BytesRead += sizeof(DataFlags);

                    if ((flags & DataFlags.CircularStart) != DataFlags.None)
                    {
                        BeginCircular(); // Will set _circularStartPos to non-zero.
                        firstCircularRecord = true;
                    }
                }

                if (_circularStartPos != 0)
                {
                    // We're in the circular part, where every record starts with its
                    // UInt32 record number, before the data flags.
                    thisRecNum = _fileReader.ReadUInt32();
                    _reader.BytesRead += sizeof(UInt32);
                    if (firstCircularRecord)
                    {
                        // This is the first chronological record in the circular part,
                        // so accept the record number as-is.
                        flags = (DataFlags)_fileReader.ReadUInt16();
                        _reader.BytesRead += sizeof(DataFlags);
                    }
                    else
                    {
                        // thisRecNum must equal _recordNumber + 1 or we've read past the last record.
                        // If it's a good record, we need to increment _recordNumber.
                        if (thisRecNum == LastRecordNum + 1)
                        {
                            flags = (DataFlags)_fileReader.ReadUInt16();
                            _reader.BytesRead += sizeof(DataFlags);

                            // There's a slim chance we a read random data that contained the expected
                            // value. Therefore, also check for invalid flags.
                            if (!FlagsAreValid(flags))
                            {
                                flags = DataFlags.None;
                            }
                        }
                        else
                        {
                            // We've already read the last (newest) record, and now we're reading garbage.
                            flags = DataFlags.None;
                        }
                    }
                }

                return flags;
            }

            // Gets the DataFlags for next record, possibly entering the circular part of the log.
            // Returns DataFlags.None or throws an exception if the last record has been read.
            // Used in file format version 6+.
            private DataFlags GetFlags6(out long recordPos, out ulong thisRecNum, out uint thisBlockNum)
            {
                recordPos = _fileReader.BaseStream.Position;
                thisRecNum = LastRecordNum + 1;
                thisBlockNum = _lastBlockNum;
                DataFlags flags = (DataFlags)_fileReader.ReadUInt16();

                if (FlagsAreValid(flags))
                {
                    _reader.BytesRead += sizeof(DataFlags);

                    if ((flags & DataFlags.BlockStart) != 0)
                    {
                        thisBlockNum = _fileReader.ReadUInt32();
                        thisRecNum = _fileReader.ReadUInt64();
                        long nextBlockPosition = _fileReader.ReadInt64();

                        _reader.BytesRead += sizeof(UInt32) + sizeof(ulong) + sizeof(long);

                        if (InCircularPart)
                        {
                            if (thisRecNum != LastRecordNum + 1 || thisBlockNum != _lastBlockNum + 1)
                            {
                                // We just read garbage.
                                flags = DataFlags.None;
                            }
                        }
                        else
                        {
                            // Only records in the circular part of the log can have BlockStart set.
                            // Since this is the first such record found,
                            // this is the first physical record in the circular part.
                            // However it may not be the first chronological record.
                            _circularStartPos = recordPos; // Makes InCircularPart true.

                            // blockNum 1 represents the non-circular part of the log, so the circular log
                            // will start with blockNum 2 unless the log has wrapped.
                            if (thisBlockNum != 2)
                            {
                                // The log has wrapped.
                                // We must find the first chronological block.  FindFirstBlock 
                                // returns the data read from the first part of the block/record and
                                // leaves the file position ready to read the rest of the block.
                                recordPos = FindFirstBlock(nextBlockPosition, ref flags, ref thisRecNum, ref thisBlockNum);
                            }
                        }
                    }
                    else if (flags == DataFlags.NewSession)
                    {
                        // We're done reading the current session.  Remember the location of
                        // the next session.
                        _reader._nextSessionPos = _fileReader.BaseStream.Position;
                        _possibleNextSessionPos = 0;
                        flags = DataFlags.None;
                    }
                    else if (InCircularPart)
                    {
                        thisBlockNum = _fileReader.ReadUInt32();
                        _reader.BytesRead += sizeof(UInt32);

                        if (thisBlockNum != _lastBlockNum)
                        {
                            flags = DataFlags.None;
                        }
                    }
                }
                else
                {
                    flags = DataFlags.None;
                }

                return flags;
            }

            // This reads blocks starting at nextBlockPos and follows the chain of blocks back from there until an invalid block is
            // found.  All parameters are assumed to be valid when called.  Upon returning curFlags is set to the
            // last valid block found in the file.  The file position is left after the flags, block number, and
            // record number of the last valid block.  
            // Returns the file position of the DataFlags for the found block.
            // Used in file format version 6+.
            private long FindFirstBlock(long nextBlockPos, ref DataFlags curFlags, ref ulong curRecNum, ref uint curBlockNum)
            {
                // curFilePos is the file position of the last good block, after reading 
                // the flags, block num, and record num.
                long curFilePos = _fileReader.BaseStream.Position;
                bool good;

                _fileReader.BaseStream.Position = nextBlockPos;

                try
                {
                    do
                    {
                        good = false;
                        var flags = (DataFlags)_fileReader.ReadUInt16();

                        if (FlagsAreValid(flags))
                        {
                            var blockNum = _fileReader.ReadUInt32();

                            if (blockNum == curBlockNum - 1)
                            {
                                var recNum = _fileReader.ReadUInt64();
                                nextBlockPos = _fileReader.ReadInt64();

                                if (nextBlockPos >= _circularStartPos && nextBlockPos < _fileReader.BaseStream.Position)
                                {
                                    curFlags = flags;
                                    curRecNum = recNum;
                                    curBlockNum = blockNum;
                                    curFilePos = _fileReader.BaseStream.Position;
                                    _fileReader.BaseStream.Position = nextBlockPos;
                                    good = true;
                                }
                            }
                        }
                    } while (good);
                }
                catch (Exception)
                {
                }

                _fileReader.BaseStream.Position = curFilePos;

                // Return the file position of the DataFlags for the found block.
                return curFilePos - sizeof(DataFlags) - sizeof(UInt32) - sizeof(ulong) - sizeof(long);
            }

            // Called immediately after finding the CircularStart marker.
            // Used in file format version 5-.
            private void BeginCircular()
            {
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
                _reader.BytesRead += _circularStartPos - temp;
            }

            // Starting at the current file position, there is an area of the specified size
            // containing a series of 6-byte records.  Each record consists of a uint16 counter
            // followed by an UInt32 file position.  Use the counters to find the last record
            // written and return the corresponding file position.  This area was written in
            // a circular fashion allowing the counter to wrap.
            // Used in file format version 5-, discontinued in version 6.
            private long FindLastFilePos(uint areaSize)
            {
                long stopPos = _fileReader.BaseStream.Position + areaSize;
                UInt16 curNum, lastNum = _fileReader.ReadUInt16();
                UInt32 curVal, lastVal = _fileReader.ReadUInt32();
                Debug.Print("lastNum = " + lastNum + ", lastVal = " + lastVal);

                while (_fileReader.BaseStream.Position != stopPos)
                {
                    curNum = _fileReader.ReadUInt16();
                    curVal = _fileReader.ReadUInt32();

                    if (curNum == lastNum + 1)
                    {
                        lastNum = curNum;
                        lastVal = curVal;
                        Debug.Print("lastNum = " + lastNum + ", lastVal = " + lastVal);
                    }
                    else
                    {
                        Debug.Print("curNum = " + curNum + ", curVal = " + curVal);
                        break;
                    }
                }

                return lastVal;
            }

            // Called when the file changes. This attempts to read more records from the file.
            // It may also generate entry/exit records in this.GeneratedRecords.
            // If this returns null, the viewer should stop trying.
            // This stops reading after maxRecords are read or the end of the file/session is reached.
            public List<Record> ReadMoreRecords(int maxRecords)
            {
                List<Record> newRecords = null;

                _reader.InternalOpen(_reader.CurrentFile);

                if (_fileReader != null)
                {
                    // Start by verifying that the last we record we read is still there.
                    // If not, the logger probably overwrote it and we can't continue.
                    if (ReReadLastRecord())
                    {
                        newRecords = new List<Record>();
                        Record record = ReadRecord();

                        while (record != null && newRecords.Count < maxRecords)
                        {
                            newRecords.Add(record);
                            record = ReadRecord();
                        }
                    }
                }

                _reader.CloseLogFile();
                return newRecords;
            }

            private bool ReReadLastRecord()
            {
                bool result = false;
                ulong thisRecNum = LastRecordNum;
                uint thisBlockNum = _lastBlockNum;

                try
                {
                    if (_lastRecordPos == 0)
                    {
                        // There is no last record to re-read.  Cause reading to start
                        // with the first record.
                        _fileReader.BaseStream.Position = _firstRecordPos;
                        result = true;
                    }
                    else
                    {
                        // If anything goes wrong or the re-read data doesn't match the old data, 
                        // assume the record got overwritten and we should give up reading.

                        _fileReader.BaseStream.Position = _lastRecordPos;
                        var flags = (DataFlags)_fileReader.ReadUInt16();

                        if ((flags & DataFlags.BlockStart) != 0)
                        {
                            thisBlockNum = _fileReader.ReadUInt32();
                            thisRecNum = _fileReader.ReadUInt64();
                            long nextBlockPosition = _fileReader.ReadInt64();
                        }
                        else if (InCircularPart)
                        {
                            thisBlockNum = _fileReader.ReadUInt32();
                        }

                        if (flags == _lastDataFlags && thisRecNum == LastRecordNum && thisBlockNum == _lastBlockNum)
                        {
                            Record notQuiteDuplicate = ReadRecordData(flags, thisRecNum, true);
                            result = true;
                        }
                    }
                }
                catch (Exception)
                {
                }

                return result;
            }
        }
    }
}