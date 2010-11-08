using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System;
//using FileLogging = TracerX.Logger.FileLogging;
using ConsoleLogging = TracerX.Logger.ConsoleLogging;
using DebugLogging = TracerX.Logger.DebugLogging;
using EventHandlerLogging = TracerX.Logger.EventHandlerLogging;

namespace TracerX {
    [Flags]
    internal enum Destinations {
        None = 0,
        BinaryFile = 1,
        TextFile = 2,
        Console = 4,
        Debug = 8,
        EventLog = 16,
        EventHandler = 32
    }

    // Instances of StackEntry are used to keep track of the call stack
    // based on calls to ErrorCall, DebugCall, etc..
    internal class StackEntry
    {
        internal StackEntry(Logger logger, TraceLevel level, string method, StackEntry caller, Destinations destinations) {
            Destinations = destinations;
            Logger = logger;
            Level = level;
            MethodName = method;
            Caller = caller;
        }

        internal ulong EntryLine;   // Line number of method entry in Binary log file.
        //internal int EntryFileNum;  // The binary file number the method entry was written to.
        internal Destinations Destinations; // Flags that indicate which destinations this was logged for.
        internal string MethodName; // The method that was called.
        internal TraceLevel Level;  // TraceLevel at which the call was called.
        internal Logger Logger;     // The Logger used to log the call.
        internal StackEntry Caller; // The method that called this one
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

        // A counter use to ensure each thread has a unique ID despite the fact that
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

        // StackDepth is incremented by LogCallEntry and decremented by LogCallExit.
        internal byte MasterStackDepth;

        // The top of the call stack for this thread.
        internal StackEntry TopStackEntry;

        internal Logger LastLoggerAnyDest = Logger.Root;

        private string _name;

        #region BinaryFile data

        // Resets all state data that might be associated with an earlier binary file.
        internal void ResetBinaryFileStateData(int fileNum, DataFlags flags)
        {
            if ((flags & DataFlags.MethodEntry) == 0) CurrentBinaryFileMethod = string.Empty;

            LastBinaryFileNumber = fileNum;
            BinaryFileStackDepth = 0;
            LastTraceLevel = TraceLevel.Off;
            LastThreadName = null;
            LastMethod = string.Empty;
            LastLogger = null;
            LastBlock = 0;

            // Remove BinaryFile from all destinations in the call stack so
            // we don't try to log method-exits when the methods exit
            // (because the method-entries were logged in an earlier file).
            for (StackEntry stackEntry = TopStackEntry; stackEntry != null; stackEntry = stackEntry.Caller)
            {
                stackEntry.Destinations &= ~Destinations.BinaryFile;
            }
        }

        // Which instance of BinaryFile the current state data is for.
        internal int LastBinaryFileNumber;
        
        // The curent method name for the thread.
        internal string CurrentBinaryFileMethod = string.Empty;

        // The stack depth of method calls logged to the file.
        internal byte BinaryFileStackDepth;

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

        #endregion

        #region TextFile data
        // The last method logged to the text file by LogCallEntry.
        internal string CurrentTextFileMethod = string.Empty;

        // The stack depth of method calls logged to the text file.
        internal byte TextFileStackDepth;
        #endregion

        #region Console data
        // The last method logged to the console by LogCallEntry.
        internal string CurrentConsoleMethod = string.Empty;

        // The stack depth of method calls logged to the console.
        internal byte ConsoleStackDepth;
        #endregion

        #region Debug data
        // The last method logged via Trace.WriteLine by LogCallEntry.
        internal string CurrentDebugMethod = string.Empty;

        // The stack depth of method calls logged via Trace.WriteLine.
        internal byte DebugStackDepth;
        #endregion

        #region EventLog data
        // The last method logged to the event log.
        internal string CurrentEventMethod = string.Empty;

        // The stack depth of method calls logged to the event log.
        internal byte EventStackDepth;
        #endregion

        #region EventHandler data
        // The last method logged to the EventHandler log.
        internal string CurrentEventHandlerMethod = string.Empty;

        // The stack depth of method calls logged to the EventHandler log.
        internal byte EventHandlerStackDepth;
        #endregion

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
                    string originalMethod = CurrentEventHandlerMethod;
                    CurrentEventHandlerMethod = method;

                    // The LogMsg method returns true if the event is cancelled.
                    result = !EventHandlerLogging.LogMsg(logger, this, level, method + " entered", true, false);

                    if (result)
                    {
                        ++EventHandlerStackDepth;
                    }
                    else
                    {
                        // Event was cancelled, so restore the original method.
                        CurrentEventHandlerMethod = originalMethod;
                    }
                }

                if (result)
                {
                    StackEntry stackEntry = new StackEntry(logger, level, method, TopStackEntry, destinations);

                    if ((destinations & Destinations.BinaryFile) == Destinations.BinaryFile)
                    {
                        CurrentBinaryFileMethod = method;
                        Logger.BinaryFileLogging.LogEntry(this, stackEntry);
                        ++BinaryFileStackDepth;
                    }

                    if ((destinations & Destinations.TextFile) == Destinations.TextFile)
                    {
                        CurrentTextFileMethod = method;
                        Logger.TextFileLogging.LogMsg(logger, this, level, method + " entered");
                        ++TextFileStackDepth;
                    }

                    if ((destinations & Destinations.Console) == Destinations.Console)
                    {
                        CurrentConsoleMethod = method;
                        ConsoleLogging.LogMsg(logger, this, level, method + " entered");
                        ++ConsoleStackDepth;
                    }

                    if ((destinations & Destinations.Debug) == Destinations.Debug)
                    {
                        CurrentDebugMethod = method;
                        DebugLogging.LogMsg(logger, this, level, method + " entered");
                        ++DebugStackDepth;
                    }

                    if ((destinations & Destinations.EventLog) == Destinations.EventLog)
                    {
                        CurrentEventMethod = method;
                        //EventLogging.LogMsg(logger, this, level, method + " entered");
                        ++EventStackDepth;
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


        // Log the exit of a method call.
        internal void LogCallExit()
        {
            if ((TopStackEntry.Destinations & Destinations.EventHandler) == Destinations.EventHandler)
            {
                // Although this LogMsg() call raises a cancellable event, method-exit messages aren't really cancellable because
                // we must "balance" the original method-entry message.
                --EventHandlerStackDepth;
                EventHandlerLogging.LogMsg(TopStackEntry.Logger, this, TopStackEntry.Level, TopStackEntry.MethodName + " exiting", false, true);
                CurrentEventHandlerMethod = GetCaller(Destinations.EventHandler);
            }

            if ((TopStackEntry.Destinations & Destinations.BinaryFile) == Destinations.BinaryFile)
            {
                if (Logger.BinaryFileLogging.LogExit(this, TopStackEntry))
                {
                    // BinaryFileStackDepth depth is decremented after logging so any meta-logging has the right depth.
                    --BinaryFileStackDepth;
                    CurrentBinaryFileMethod = GetCaller(Destinations.BinaryFile);
                }
            }

            if ((TopStackEntry.Destinations & Destinations.TextFile) == Destinations.TextFile)
            {
                --TextFileStackDepth;
                Logger.TextFileLogging.LogMsg(TopStackEntry.Logger, this, TopStackEntry.Level, TopStackEntry.MethodName + " exiting");
                CurrentTextFileMethod = GetCaller(Destinations.TextFile);
            }

            if ((TopStackEntry.Destinations & Destinations.Console) == Destinations.Console)
            {
                --ConsoleStackDepth;
                ConsoleLogging.LogMsg(TopStackEntry.Logger, this, TopStackEntry.Level, TopStackEntry.MethodName + " exiting");
                CurrentConsoleMethod = GetCaller(Destinations.Console);
            }

            if ((TopStackEntry.Destinations & Destinations.Debug) == Destinations.Debug)
            {
                --DebugStackDepth;
                DebugLogging.LogMsg(TopStackEntry.Logger, this, TopStackEntry.Level, TopStackEntry.MethodName + " exiting");
                CurrentDebugMethod = GetCaller(Destinations.Debug);
            }

            if ((TopStackEntry.Destinations & Destinations.EventLog) == Destinations.EventLog)
            {
                --EventStackDepth;
                //EventLogging.LogMsg(TopStackEntry.Logger, this, TopStackEntry.Level, TopStackEntry.MethodName + " exiting");
                CurrentEventMethod = GetCaller(Destinations.EventLog);
            }

            LastLoggerAnyDest = TopStackEntry.Logger;
            TopStackEntry = TopStackEntry.Caller;
            --MasterStackDepth;
        }

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

    }
}
