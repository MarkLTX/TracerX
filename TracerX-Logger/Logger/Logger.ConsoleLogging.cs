using System;
using System.Collections.Generic;
using System.Threading;

namespace TracerX
{
    public partial class Logger
    {

        /// <summary>
        /// Methods and configuration for logging to the console.
        /// </summary>
        public static class ConsoleLogging
        {
            /// <summary>
            /// Controls which fields are written to the console by all all loggers
            /// where Logger.ConsoleTraceLevel is greater than or equal to the log statement's level.
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
            public static string FormatString
            {
                get { return _consoleFormatString; }
                set
                {
                    lock (_consoleLocker)
                    {
                        _consoleFormatString = value;
                        _internalConsoleFormatString = ParseFormatString(value);
                    }
                }
            }
            private static string _consoleFormatString = "{time:HH:mm:ss.fff} {level} {thname} {logger}+{method} {ind}{msg}";
            private static string _internalConsoleFormatString = ParseFormatString(_consoleFormatString);

            // Counts the number of calls to LogMsg.
            private static int _lineCount;

            // Provides thread-safety for the console logger.
            private static object _consoleLocker = new object();

            // Logs a message to the console.
            internal static void LogMsg(Logger logger, ThreadData threadData, TraceLevel msgLevel, string msg)
            {
                lock (_consoleLocker)
                {
                    string indent = new string(' ', 3 * threadData.ConsoleState.StackDepth);
                    ++_lineCount;

                    Console.WriteLine(Logger.ConsoleLogging._internalConsoleFormatString,
                        _lineCount,
                        Enum.GetName(typeof(TraceLevel), msgLevel),
                        logger.Name,
                        threadData.TracerXID,
                        threadData.Name ?? "<null>",
                        DateTime.Now,
                        threadData.ConsoleState.CurrentMethod ?? "<null>",
                        indent,
                        msg == null ? "<null>" : msg
                        );
                }
            }
        }
    }
}