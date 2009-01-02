using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System;
using FileLogging = TracerX.Logger.FileLogging;
using ConsoleLogging = TracerX.Logger.ConsoleLogging;
using DebugLogging = TracerX.Logger.DebugLogging;

namespace TracerX {
    [Flags]
    internal enum Destination {
        None = 0,
        File = 1,
        Console = 2,
        Debug = 4,
        EventLog = 8
    }

    // Instances of StackEntry are used to keep track of the call stack
    // based on calls to ErrorCall, DebugCall, etc..
    internal class StackEntry
    {
        internal StackEntry(Logger logger, TraceLevel level, string method, StackEntry caller, Destination destinations) {
            Destinations = destinations;
            Logger = logger;
            Level = level;
            MethodName = method;
            Caller = caller;
        }

        internal uint EntryLine; // Line number of method entry in log file.
        internal Destination Destinations; // Flags that indicate which destinations this was logged for.
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
                if (_threadData == null) {
                    _threadData = new ThreadData();
                }

                return _threadData;
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

        // The managed thread ID of the thread.  This is often the same as another thread that
        // terminated earlier (.NET recycles the managed IDs).  
        internal int ManagedId = Thread.CurrentThread.ManagedThreadId;

        // StackDepth is incremented by LogCallEntry and decremented by LogCallExit.
        internal byte MasterStackDepth;

        // The top of the call stack for this thread.
        internal StackEntry TopStackEntry;

        internal Logger LastLoggerAnyDest = Logger.Root;

        #region File data
        // The curent method name for the thread.
        internal string CurrentFileMethod = string.Empty;

        // The stack depth of method calls logged to the file.
        internal byte FileStackDepth;

        // The trace Level of the last line logged by this thread.  TraceLevel.Off means no lines logged yet.
        internal TraceLevel LastTraceLevel = TraceLevel.Off;

        // The thread's name the last time it logged a line.
        internal string LastName;

        // The method name for the thread's previous line.
        internal string LastMethod = string.Empty;

        // The logger for the last line logged by the thread.
        internal Logger LastLogger;

        // The last block logged to by this thread.  0 means no lines were logged yet.
        internal uint LastBlock = 0;        
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

        #region Event data
        // The last method logged to the event log.
        internal string CurrentEventMethod = string.Empty;

        // The stack depth of method calls logged to the event log.
        internal byte EventStackDepth;
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

        // Log the entry of a method call.
        internal bool LogCallEntry(Logger logger, TraceLevel level, string method) {
            if (MasterStackDepth < byte.MaxValue) {
                Destination destinations = logger.Destinations(level);
                StackEntry stackEntry = new StackEntry(logger, level, method, TopStackEntry, destinations);
                
                if ((destinations & Destination.File) == Destination.File) {
                    CurrentFileMethod = method;
                    FileLogging.LogEntry(this, stackEntry);
                    ++FileStackDepth;
                }

                if ((destinations & Destination.Console) == Destination.Console) {
                    CurrentConsoleMethod = method;
                    ConsoleLogging.LogMsg(logger, this, level, method + " entered");
                    ++ConsoleStackDepth;
                }

                if ((destinations & Destination.Debug) == Destination.Debug) {
                    CurrentDebugMethod = method;
                    DebugLogging.LogMsg(logger, this, level, method + " entered");
                    ++DebugStackDepth;
                }

                if ((destinations & Destination.EventLog) == Destination.EventLog) {
                    CurrentEventMethod = method;
                    //EventDestination.LogMsg(logger, this, level, method + " entered");
                    ++EventStackDepth;
                }
                
                ++MasterStackDepth;
                TopStackEntry = stackEntry;

                return true;
            } else {
                // Already at max depth, so don't log this.  Return false so
                // the corresponding future call to LogCallExit won't happen either.
                return false;
            }
        }


        // Log the exit of a method call.
        internal void LogCallExit() {
            string newMethod = TopStackEntry.Caller == null ? String.Empty : TopStackEntry.Caller.MethodName;
            
            if ((TopStackEntry.Destinations & Destination.File) == Destination.File) {
                // FileStackDepth depth is decremented after logging so any meta-logging has the right depth.
                FileLogging.LogExit(this, TopStackEntry);
                --FileStackDepth;
                CurrentFileMethod = newMethod;
            }

            if ((TopStackEntry.Destinations & Destination.Console) == Destination.Console) {
                --ConsoleStackDepth;
                ConsoleLogging.LogMsg(TopStackEntry.Logger, this, TopStackEntry.Level, TopStackEntry.MethodName + " exiting");
                CurrentConsoleMethod = newMethod;
            }

            if ((TopStackEntry.Destinations & Destination.Debug) == Destination.Debug) {
                --DebugStackDepth;
                DebugLogging.LogMsg(TopStackEntry.Logger, this, TopStackEntry.Level, TopStackEntry.MethodName + " exiting");
                CurrentDebugMethod = newMethod;
            }

            if ((TopStackEntry.Destinations & Destination.EventLog) == Destination.EventLog) {
                --DebugStackDepth;
                //EventDestination.LogMsg(TopStackEntry.Logger, this, TopStackEntry.Level, TopStackEntry.MethodName + " exiting");
                CurrentEventMethod = newMethod;
            }
            
            --MasterStackDepth;
            TopStackEntry = TopStackEntry.Caller;
        }

    }
}
