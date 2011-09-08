using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System;
using ConsoleLogging = TracerX.Logger.ConsoleLogging;
using DebugLogging = TracerX.Logger.DebugLogging;
using EventHandlerLogging = TracerX.Logger.EventHandlerLogging;
using System.Collections.Generic;

namespace TracerX 
{
    // Each instance of ThreadData has one of these for each destination.
    internal class DestinationState
    {
        // The stack depth of method calls logged to the destination.
        internal byte StackDepth;

        // The curent method name for the thread.
        internal string CurrentMethod = string.Empty;
    }

    // Each instance of ThreadData has one of these for each TextFile 
    // the thread writes to.
    internal class TextFileState : DestinationState
    {
        internal TextFile File;
    }

    // Each instance of ThreadData has one of these for each BinaryFile 
    // the thread writes to.
    internal class BinaryFileState : DestinationState
    {
        // Which instance of BinaryFile the current state data is for.
        internal BinaryFile File;

        // How often the file has been opened.
        internal int LastFileNumber;

        // The trace Level of the last line logged by this thread.  TraceLevel.Off means no lines logged yet.
        internal TraceLevel LastTraceLevel = TraceLevel.Off;

        // The thread's name the last time it logged a line.
        internal string LastThreadName;

        // The method name for the thread's previous line.
        internal string LastMethod = string.Empty;

        // The logger for the last line logged by the thread.
        internal Logger LastLogger;

        // The last block logged to by this thread.  0 means no lines were logged yet.
        internal uint LastBlock = 0;
    }

    // Instances of StackEntry are used to keep track of the call stack
    // based on calls to ErrorCall, DebugCall, etc..
    // Each thread (instance of ThreadData) has its own stack of these.
    internal class StackEntry
    {
        internal StackEntry(Logger logger, TraceLevel level, string method, StackEntry caller, Destinations destinations)
        {
            Logger = logger;
            Level = level;
            MethodName = method;
            Caller = caller;
            Destinations = destinations;
        }

        internal StackEntry Caller; // The method that called this one.
        internal Destinations Destinations; // Which destinations the call was logged to.
        internal BinaryFileState BinaryFileState; // If one of the destinations was BinaryFile, this is its thread-specific state data.
        internal ulong EntryLine;   // Line number of method entry in Binary log file.
        internal TextFileState TextFileState; // If one of the destinations was TextFile, this is its thread-specific state data.
        internal string MethodName; // The method that was called.
        internal TraceLevel Level;  // TraceLevel at which the call was called.
        internal Logger Logger;     // The Logger used to log the call.
    }

    /// <summary>
    /// Contains information about a logging thread stored 
    /// in ThreadStatic (i.e. thread-local) memory.
    /// An instance is created for each thread that uses TracerX.
    /// 
    /// Testing has shown that the instance is released when the thread exits and that
    /// instances associated with ThreadPool threads DO NOT get released
    /// when the thread is returned to the pool.  
    /// Testing has shown that when a ThreadPool thread is recycled, 
    ///   1) Its ManagedThreadId remains the same.
    ///   2) Its ThreadStatic storage remains allocated and associated with the thread.
    ///   3) Its Name is reset to null and a new name can be assigned.
    /// 
    /// TracerX does not use the ManagedThreadId because the CLR
    /// appears to recycle the IDs.  That is, a new thread will often be assigned the
    /// same ManagedThreadId as another thread that recently terminated.  This means
    /// ManagedThreadId isn't unique for the life of the process (and therefore the log).
    /// </summary>
    internal class ThreadData {

        //~ThreadData() {
        //    // This proves that the ThreadData object is finalized after the thread terminates.
        //    Debug.WriteLine("ThreadData finalized.");
        //}

        /// <summary>
        /// This returns the thread-local (i.e. ThreadStatic) instance of ThreadData
        /// for the calling thread, creating it on the first reference from a given thread. 
        /// </summary>
        internal static ThreadData CurrentThreadData {
            get {
                // I read somewhere that it takes 60 times as long to reference a [ThreadStatic] field
                // as a regular field, so this is coded to minimize such references.  That same article
                // said [ThreadStatic] is the fastest way to do this.
                var result = _threadData;

                if (result == null)
                {
                    result = new ThreadData();
                    _threadData = result;
                }

                return result;
            }
        }

        [ThreadStatic]
        private static ThreadData _threadData;

        // A counter used to ensure each thread has a unique ID despite the fact that
        // .NET recycles the managed thread IDs.  
        private static int _threadCounter = 0;

        // The thread's ID as determined by TracerX (as opposed to the CLR).
        // A uint would be preferrable but Interlocked.Increment doesn't operate
        // on uints.  The number of unique thread IDs is the same either way.
        internal int TracerXID = Interlocked.Increment(ref _threadCounter);

        // The overridden thread name or Thread.CurrentThread.Name if not overridden.
        internal string Name {
            get { return _name ?? Thread.CurrentThread.Name; }
            set { _name = value; }
        }

        // The managed thread ID of the thread.  This is often the same as another thread that
        // terminated earlier (.NET recycles the managed IDs).  
        internal int ManagedId = Thread.CurrentThread.ManagedThreadId;

        // The top of the call stack for this thread.
        internal StackEntry TopStackEntry;

        // MasterStackDepth is incremented by LogCallEntry and decremented by LogCallExit.
        // It's the number of entries in the linked list starting with TopStackEntry.
        internal byte MasterStackDepth;

        internal Logger LastLoggerAnyDest = Logger.Root;

        private string _name;

        #region BinaryFile data

        // TODO: Replace this with a ThreadLocal<> member in BinaryFile when we use C# 4.0.
        internal BinaryFileState BinaryFileState { get; private set; }

        private Dictionary<BinaryFile, BinaryFileState> _binaryFileStatesForThisThread = new Dictionary<BinaryFile, BinaryFileState>();

        // Looks up the thread-specific state for the specified binary file,
        // creating a new one if necessary.
        internal BinaryFileState GetBinaryFileState(BinaryFile file)
        {
            BinaryFileState result = BinaryFileState;

            if (BinaryFileState == null || BinaryFileState.File != file)
            {
                // Look up or create the state data for the specified file.
                if (!_binaryFileStatesForThisThread.TryGetValue(file, out result))
                {
                    result = new BinaryFileState();
                    result.File = file;
                    _binaryFileStatesForThisThread.Add(file, result);
                }

                BinaryFileState = result;
            }
            else
            {
                // We already have the data for the specified file.
            }

            return result;
        }

        // Resets all state data that might be associated with an earlier binary file
        // and prepares for the next (specified) opening of the file.
        internal void ResetBinaryFileStateData(int fileNum, DataFlags flags)
        {
            BinaryFileState.LastFileNumber = fileNum;
            BinaryFileState.LastTraceLevel = TraceLevel.Off;
            BinaryFileState.LastThreadName = null;
            BinaryFileState.LastMethod = string.Empty;
            BinaryFileState.LastLogger = null;
            BinaryFileState.LastBlock = 0;

            // For simplicity, abandon the stack rather than deal with the fact that there
            // may be a deep call stack when the file is closed and reopened.

            BinaryFileState.StackDepth = 0;

            for (StackEntry stackEntry = TopStackEntry; stackEntry != null; stackEntry = stackEntry.Caller)
            {
                if (stackEntry.BinaryFileState == BinaryFileState)
                {
                    stackEntry.Destinations &= ~Destinations.BinaryFile;
                    stackEntry.BinaryFileState = null;
                }
            }

            // I'm not sure why we wouldn't always do this.
            if ((flags & DataFlags.MethodEntry) == 0) BinaryFileState.CurrentMethod = string.Empty;
        }

        #endregion

        #region TextFile data

        internal TextFileState TextFileState { get; private set; }

        private Dictionary<TextFile, TextFileState> _textFileStatesForThisThread = new Dictionary<TextFile, TextFileState>();

        // Looks up the thread-specific state for the specified binary file,
        // creating a new one if necessary.
        internal TextFileState GetTextFileState(TextFile file)
        {
            TextFileState result = TextFileState;

            if (TextFileState == null || TextFileState.File != file)
            {
                // Look up or create the state data for the specified file.
                if (!_textFileStatesForThisThread.TryGetValue(file, out result))
                {
                    result = new TextFileState();
                    result.File = file;
                    _textFileStatesForThisThread.Add(file, result);
                }

                TextFileState = result;
            }
            else
            {
                // We already have the data for the specified file.
            }

            return result;
        }

        #endregion

        // The state data for the other destinations is simple because there is only one instance of each.
        internal DestinationState ConsoleState = new DestinationState();
        internal DestinationState DebugState = new DestinationState();
        internal DestinationState EventLogState = new DestinationState();
        internal DestinationState EventHandlerState = new DestinationState();

        // A thread-safe place to append several message parts into one message before logging it.
        internal readonly StringWriter StringWriter = new StringWriter();

        internal string ResetStringWriter() {
            // Clear the StringWriter's underlying StringBuilder in preparation for the next message.
            // Return the string so it can be logged.
            StringBuilder builder = StringWriter.GetStringBuilder();
            string str = builder.ToString();
            builder.Length = 0;
            if (builder.Capacity > 512) {
                // Free the excess memory.
                builder.Capacity = 256;
            }

            return str;
        }

        // Possibly logs the entry of a method call. Returns true if the message is logged, meaning
        // the corresponding method-exit should also be logged when it occurs.
        internal bool LogCallEntry(Logger logger, TraceLevel level, string method, Destinations destinations)
        {
            bool result = true;

            if (MasterStackDepth < byte.MaxValue)
            {
                if ((destinations & Destinations.EventHandler) == Destinations.EventHandler)
                {
                    string originalMethod = EventHandlerState.CurrentMethod;
                    EventHandlerState.CurrentMethod = method;

                    // The LogMsg method returns true if the event is cancelled.
                    result = !EventHandlerLogging.LogMsg(logger, this, level, method + " entered", true, false);

                    if (result)
                    {
                        ++EventHandlerState.StackDepth;
                    }
                    else
                    {
                        // Event was cancelled, so restore the original method.
                        EventHandlerState.CurrentMethod = originalMethod;
                    }
                }

                if (result)
                {
                    StackEntry stackEntry = new StackEntry(logger, level, method, TopStackEntry, destinations);

                    // Don't put the stack entry on the stack (i.e. set TopStackEntry) until after the method-entry
                    // is logged because of code in BinaryFile.WriteLine().

                    if ((destinations & Destinations.BinaryFile) == Destinations.BinaryFile)
                    {
                        // This may be the first output to be written to the file, so commit to it.
                        logger.CommitToBinaryFile();

                        // Lookup the BinaryFileState that applies to this thread and file.
                        stackEntry.BinaryFileState = GetBinaryFileState(logger.BinaryFile);

                        BinaryFileState.CurrentMethod = method;
                        logger.BinaryFile.LogEntry(this, stackEntry);
                        ++BinaryFileState.StackDepth;
                    }

                    if ((destinations & Destinations.TextFile) == Destinations.TextFile)
                    {
                        // This may be the first output to be written to the file, so commit to it.
                        logger.CommitToTextFile();

                        stackEntry.TextFileState = GetTextFileState(logger.TextFile);

                        TextFileState.CurrentMethod = method;
                        logger.TextFile.LogMsg(logger, this, level, method + " entered");
                        ++TextFileState.StackDepth;
                    }

                    if ((destinations & Destinations.Console) == Destinations.Console)
                    {
                        ConsoleState.CurrentMethod = method;
                        ConsoleLogging.LogMsg(logger, this, level, method + " entered");
                        ++ConsoleState.StackDepth;
                    }

                    if ((destinations & Destinations.Debug) == Destinations.Debug)
                    {
                        DebugState.CurrentMethod = method;
                        DebugLogging.LogMsg(logger, this, level, method + " entered");
                        ++DebugState.StackDepth;
                    }

                    if ((destinations & Destinations.EventLog) == Destinations.EventLog)
                    {
                        // Method-entry messages seem like a dumb thing to put in the event log,
                        // but we'll keep track of the stack anyway.

                        EventLogState.CurrentMethod = method;
                        //EventLogging.LogMsg(logger, this, level, method + " entered");
                        ++EventLogState.StackDepth;
                    }

                    ++MasterStackDepth;
                    TopStackEntry = stackEntry;
                }
            }
            else
            {
                // Already at max depth, so don't log this.  Return false so
                // the corresponding future call to LogCallExit won't happen either.
                result = false;
            }

            LastLoggerAnyDest = logger;
            return result;
        }

        // Log the exit of a method call to each destination indicated by TopStackEntry.
        internal void LogCallExit()
        {
            if ((TopStackEntry.Destinations & Destinations.EventHandler) != 0)
            {
                // Although this LogMsg() call raises a cancellable event, method-exit messages aren't really cancellable because
                // we must "balance" the original method-entry message.
                --EventHandlerState.StackDepth;
                EventHandlerLogging.LogMsg(TopStackEntry.Logger, this, TopStackEntry.Level, TopStackEntry.MethodName + " exiting", false, true);
                EventHandlerState.CurrentMethod = GetCaller(Destinations.EventHandler);
            }

            if ((TopStackEntry.Destinations & Destinations.BinaryFile) != 0)
            {
                // Make sure BinaryFileState corresponds to the BinaryFile instance
                // the method-exit should be logged to (the same BinaryFile the
                // method-entry for TopStackEntry was logged to).
                BinaryFileState = TopStackEntry.BinaryFileState;

                if (TopStackEntry.Logger.BinaryFile.LogExit(this))
                {
                    // BinaryFileState.StackDepth depth is decremented after logging so any meta-logging has the right depth.
                    // GetCaller() also depends on the stack depth.
                    BinaryFileState.CurrentMethod = GetCaller(BinaryFileState);
                    --BinaryFileState.StackDepth;
                }
            }

            if ((TopStackEntry.Destinations & Destinations.TextFile) != 0)
            {
                // Make sure BinaryFileState corresponds to the BinaryFile instance
                // the method-exit should be logged to (the same BinaryFile the
                // method-entry for TopStackEntry was logged to).
                TextFileState = TopStackEntry.TextFileState;

                --TextFileState.StackDepth;
                TopStackEntry.Logger.TextFile.LogMsg(TopStackEntry.Logger, this, TopStackEntry.Level, TopStackEntry.MethodName + " exiting");
                TextFileState.CurrentMethod = GetCaller(TextFileState);
            }

            if ((TopStackEntry.Destinations & Destinations.Console) != 0)
            {
                --ConsoleState.StackDepth;
                ConsoleLogging.LogMsg(TopStackEntry.Logger, this, TopStackEntry.Level, TopStackEntry.MethodName + " exiting");
                ConsoleState.CurrentMethod = GetCaller(Destinations.Console);
            }

            if ((TopStackEntry.Destinations & Destinations.Debug) != 0)
            {
                --DebugState.StackDepth;
                DebugLogging.LogMsg(TopStackEntry.Logger, this, TopStackEntry.Level, TopStackEntry.MethodName + " exiting");
                DebugState.CurrentMethod = GetCaller(Destinations.Debug);
            }

            if ((TopStackEntry.Destinations & Destinations.EventLog) != 0)
            {
                --EventLogState.StackDepth;
                //EventLogging.LogMsg(TopStackEntry.Logger, this, TopStackEntry.Level, TopStackEntry.MethodName + " exiting");
                EventLogState.CurrentMethod = GetCaller(Destinations.EventLog);
            }

            LastLoggerAnyDest = TopStackEntry.Logger;
            TopStackEntry = TopStackEntry.Caller;
            --MasterStackDepth;
        }

        // Search down the stack for the next caller for the specified destination.
        private string GetCaller(Destinations destination)
        {
            for (StackEntry caller = TopStackEntry.Caller; caller != null; caller = caller.Caller)
            {
                if ((caller.Destinations & destination) == destination)
                {
                    return caller.MethodName;
                }
            }

            return "";
        }

        // Search down the stack for the next caller for the specified binary file.
        private string GetCaller(BinaryFileState fileData)
        {
            // For binary files, the stack depth is decremented AFTER this is called,
            // so check stackdepth > 1.
            if (fileData.StackDepth > 1)
            {
                for (StackEntry caller = TopStackEntry.Caller; caller != null; caller = caller.Caller)
                {
                    if (caller.BinaryFileState == fileData)
                    {
                        return caller.MethodName;
                    }
                }

                Debug.Assert(false, "A caller should have been found.");
            }

            return "";
        }

        // Search down the stack for the next caller for the specified text file.
        private string GetCaller(TextFileState fileData)
        {
            // For text files, the stack depth is decremented BEFORE this is called,
            // so check stackdepth > 0.
            if (fileData.StackDepth > 0)
            {
                for (StackEntry caller = TopStackEntry.Caller; caller != null; caller = caller.Caller)
                {
                    if (caller.TextFileState == fileData)
                    {
                        return caller.MethodName;
                    }
                }

                Debug.Assert(false, "A caller should have been found.");
            }

            return "";
        }

    }
}
