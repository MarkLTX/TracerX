using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace TracerX {
    public partial class Logger {
        /// <summary>
        /// Methods and configuration for logging to an event log.
        /// </summary>
        public static class EventLogging {
            static EventLogging() {
                ushort arbitrary = 10000;
                foreach (TraceLevel level in Enum.GetValues(typeof(TraceLevel))) {
                    EventIdMap[level] = arbitrary++;
                    EventTypeMap[level] = EventLogEntryType.Information;
                }

                EventTypeMap[TraceLevel.Fatal] = EventLogEntryType.Error;
                EventTypeMap[TraceLevel.Error] = EventLogEntryType.Error;
                EventTypeMap[TraceLevel.Warn] = EventLogEntryType.Warning;
            }

            /// <summary>
            /// The EventLog object used for all TracerX event logging.  Set this before opening the
            /// log file, since that action can log events.  By default, TracerX logs to the
            /// application event log on the local computer using the source "TracerX - 'exe name'".
            /// Cannot be set in XML.
            /// </summary>
            public static EventLog EventLog {
                get { return _eventLog; }
                set { _eventLog = value; }
            }
            private static EventLog _eventLog = new EventLog("Application", ".", "TracerX - " + Path.GetFileNameWithoutExtension(Application.ExecutablePath));

            /// <summary>
            /// Controls which fields are written to the event log by all loggers where
            /// Logger.EventLogTraceLevel is greater than or equal to the log statement's level. See ConsoleFormatString for the
            /// substitution parameters.
            /// </summary>
            public static string FormatString {
                get { return _eventFormatString; }
                set {
                    lock (_eventLocker) {
                        _eventFormatString = value;
                        _internalEventFormatString = ParseFormatString(value);
                    }
                }
            }
            private static string _eventFormatString = "{time:HH:mm:ss.fff} {thname} {logger}+{method} {msg}";
            private static string _internalEventFormatString = ParseFormatString(_eventFormatString);

            /// <summary>
            /// This specifies what event ID number to use for each TraceLevel.
            /// Cannot be set in XML.
            /// </summary>
            public static Dictionary<TraceLevel, ushort> EventIdMap { get { return _eventIdMap; } }
            private static Dictionary<TraceLevel, ushort> _eventIdMap = new Dictionary<TraceLevel, ushort>();

            /// <summary>
            /// This specifies what EventTypeMap to use for each TraceLevel.
            /// Cannot be set in XML.
            /// </summary>
            public static Dictionary<TraceLevel, EventLogEntryType> EventTypeMap { get { return _eventTypeMap; } }
            private static Dictionary<TraceLevel, EventLogEntryType> _eventTypeMap = new Dictionary<TraceLevel, EventLogEntryType>();

            /// <summary>
            /// The maximum internal event number that will be logged by TracerX.  
            /// 1-100 for errors, 101-200 for warnings, 201-300 for info.
            /// </summary>
            public static uint MaxInternalEventNumber {
                get { return _maxEventNumber; }
                set { _maxEventNumber = value; }
            }
            private static uint _maxEventNumber = 200;

            /// <summary>
            /// The number that is added to internal event numbers just before they are logged.
            /// </summary>
            public static uint InternalEventOffset {
                get { return _internalEventOffset; }
                set { _internalEventOffset = value; }
            }
            private static uint _internalEventOffset = 1000;

            // Counts the number of calls to LogMsg.
            private static int _lineCount;

            // Provides thread-safety for the event logger.
            private static object _eventLocker = new object();

            // Logs a message to the event log.
            internal static void LogMsg(Logger logger, ThreadData threadData, TraceLevel msgLevel, string msg) {
                lock (_eventLocker) {
                    string indent = new string(' ', 3 * threadData.EventStackDepth);
                    ++_lineCount;

                    string output = string.Format(Logger.EventLogging._internalEventFormatString,
                        _lineCount,
                        msgLevel,
                        logger.Name,
                        threadData.TracerXID,
                        Thread.CurrentThread.Name == null ? "<null>" : Thread.CurrentThread.Name,
                        DateTime.Now,
                        threadData.CurrentEventMethod == null ? "<null>" : threadData.CurrentEventMethod,
                        indent,
                        msg == null ? "<null>" : msg
                        );

                    EventLogEntryType eventType = EventLogEntryType.Information;
                    EventTypeMap.TryGetValue(msgLevel, out eventType);

                    ushort eventId = 10999;
                    EventIdMap.TryGetValue(msgLevel, out eventId);

                    Logger.EventLogging.EventLog.WriteEntry(output, eventType, eventId);
                }
            }

            #region Internal event logging
            // 1-100 = error
            internal const int ExceptionInOpen = 1;  // An exception occurred opening or archiving the log file.
            //internal const int ErrorReadingRegistry = 2;
            internal const int ExceptionInLogger = 3;
            internal const int XmlConfigError = 4; // Failed to load the TracerX element from XML.
            internal const int MaxSizeZero = 5; // Log file was not opened because the max size was zero.

            // 101-200 = warning
            internal const int FileAlreadyOpen = 101;  // Tried to open the file twice.
            internal const int ErrorWritingRegistry = 102;
            internal const int TooLateForCircular = 103; // Circular logging started too late.
            internal const int MaxFileSizeReached = 104; // Reached max size before circular logging started.
            internal const int XmlConfigWarning = 105; // The TracerX element was loaded from XML with warnings.
            internal const int ExceptionInArchive = 106; // An  exception occurred while archiving the old output file.
            internal const int UnhandledExceptionInApp = 120; // An unhandled exception occurred somewhere in the app.

            // 201-300 = info
            internal const int LogFileOpened = 201;
            internal const int CircularLogStarted = 202;
            internal const int FirstWrap = 203;
            internal const int ConfigFileChanged = 204;

            internal static void Log(string msg, int num) {
                if (num <= Logger.EventLogging.MaxInternalEventNumber) {
                    try {
                        if (num <= 100) {
                            Logger.EventLogging.EventLog.WriteEntry(msg, System.Diagnostics.EventLogEntryType.Error, (int)InternalEventOffset + num);
                        } else if (num <= 200) {
                            Logger.EventLogging.EventLog.WriteEntry(msg, System.Diagnostics.EventLogEntryType.Warning, (int)InternalEventOffset + num);
                        } else {
                            Logger.EventLogging.EventLog.WriteEntry(msg, System.Diagnostics.EventLogEntryType.Information, (int)InternalEventOffset + num);
                        }
                    } catch (Exception) {
                        // Have seen "event log is full" exception here.  Can't really do anything about that.
                    }
                }
            }
            #endregion
        }
    }
}