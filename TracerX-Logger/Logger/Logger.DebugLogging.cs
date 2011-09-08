using System;
using System.Threading;

namespace TracerX {
    public partial class Logger {
        /// <summary>
        /// Methods and configuration for logging via Trace.WriteLine().
        /// </summary>
        public static class DebugLogging {
            /// <summary>
            /// Controls which fields are passed to Trace.WriteLine by all loggers
            /// where Logger.DebugTraceLevel is greater than or equal to the log statement's level.
            /// Uses the following substitution parameters.
            /// {line} = Line number 
            /// {level} = Trace level 
            /// {logger} = Logger name 
            /// {thnum} = Thread number (not thread ID). 
            /// {thname} = Thread name  
            /// {time} = Time stamp   
            /// {method} = Method name  
            /// {ind} = Indentation  
            /// {msg} = Message text 
            /// </summary>
            public static string FormatString {
                get { return _debugFormatString; }
                set {
                    lock (_debugLocker) {
                        _debugFormatString = value;
                        _internalDebugFormatString = ParseFormatString(value);
                    }
                }
            }
            private static string _debugFormatString = "{time:HH:mm:ss.fff} {level} {thname} {logger}+{method} {ind}{msg}";
            private static string _internalDebugFormatString = ParseFormatString(_debugFormatString);

            // Counts the number of calls to LogMsg.
            private static int _lineCount;

            // Provides thread-safety for the debug logger.
            private static object _debugLocker = new object();

            // Logs a message via Trace.WriteLine.
            internal static void LogMsg(Logger logger, ThreadData threadData, TraceLevel msgLevel, string msg) {
                lock (_debugLocker) {
                    string indent = new string(' ', 3 * threadData.DebugState.StackDepth);
                    ++_lineCount;

                    msg = string.Format(Logger.DebugLogging._internalDebugFormatString,
                        _lineCount,
                        Enum.GetName(typeof(TraceLevel), msgLevel),
                        logger.Name,
                        threadData.TracerXID,
                        threadData.Name ?? "<null>",
                        DateTime.Now,
                        threadData.DebugState.CurrentMethod ?? "<null>" ,
                        indent,
                        msg ?? "<null>"
                        );

                    System.Diagnostics.Trace.WriteLine(msg);
                }
            }
        }
    }
}
