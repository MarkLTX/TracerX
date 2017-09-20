using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace TracerX
{
    public partial class Logger
    {
        /// <summary>
        /// Methods and configuration for logging to an event log.
        /// </summary>
        public static class EventLogging
        {
            static EventLogging()
            {
                ushort arbitrary = 10000;
                foreach (TraceLevel level in Enum.GetValues(typeof(TraceLevel)))
                {
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
            public static EventLog EventLog
            {
                get { return _eventLog; }
                set { _eventLog = value; }
            }
            private static EventLog _eventLog = new EventLog("Application", ".", "TracerX - " + AppName);

            /// <summary>
            /// Controls which fields are written to the event log by all loggers where
            /// <see cref="Logger.EventLogTraceLevel"/> is greater than or equal to the log statement's <see cref="TraceLevel"/>. 
            /// See <see cref="Logger.ConsoleLogging.FormatString"/> for the substitution parameters.
            /// </summary>
            public static string FormatString
            {
                get { return _eventFormatString; }
                set
                {
                    lock (_eventLocker)
                    {
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
            /// This specifies what EventLogEntryType to use for each TraceLevel.
            /// Cannot be set in XML.
            /// </summary>
            public static Dictionary<TraceLevel, EventLogEntryType> EventTypeMap { get { return _eventTypeMap; } }
            private static Dictionary<TraceLevel, EventLogEntryType> _eventTypeMap = new Dictionary<TraceLevel, EventLogEntryType>();

            /// <summary>
            /// The maximum internal event number that will be logged by TracerX.  
            /// 1-100 for errors, 101-200 for warnings, 201-300 for info.
            /// </summary>
            public static uint MaxInternalEventNumber
            {
                get { return _maxEventNumber; }
                set { _maxEventNumber = value; }
            }
            private static uint _maxEventNumber = 200;

            /// <summary>
            /// The number that is added to internal event numbers just before they are logged.
            /// </summary>
            public static uint InternalEventOffset
            {
                get { return _internalEventOffset; }
                set { _internalEventOffset = value; }
            }
            private static uint _internalEventOffset = 1000;

            // Counts the number of calls to LogMsg.
            private static int _lineCount;

            // Provides thread-safety for the event logger.
            private static object _eventLocker = new object();

            // Logs a message to the event log.
            internal static void LogMsg(Logger logger, ThreadData threadData, TraceLevel msgLevel, string msg)
            {
                lock (_eventLocker)
                {
                    string indent = new string(' ', 3 * threadData.EventLogState.StackDepth);
                    ++_lineCount;

                    string output = string.Format(_internalEventFormatString,
                        _lineCount,
                        Enum.GetName(typeof(TraceLevel), msgLevel),
                        logger.Name,
                        threadData.TracerXID,
                        threadData.Name ?? "<null>",
                        DateTime.Now,
                        threadData.EventLogState.CurrentMethod ?? "<null>",
                        indent,
                        msg ?? "<null>"
                        );

                    EventLogEntryType eventType = EventLogEntryType.Information;
                    EventTypeMap.TryGetValue(msgLevel, out eventType);

                    ushort eventId = 10999;
                    EventIdMap.TryGetValue(msgLevel, out eventId);

                    EventLog.WriteEntry(output, eventType, eventId);
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
            //internal const int ErrorWritingRegistry = 102;
            internal const int TooLateForCircular = 103; // Circular logging started too late.
            internal const int MaxFileSizeReached = 104; // Reached max size before circular logging started.
            internal const int XmlConfigWarning = 105; // The TracerX element was loaded from XML with warnings.
            internal const int ExceptionInArchive = 106; // An exception occurred while archiving the old output file.
            internal const int AppendVersionConflict = 107; // Could not append to existing log file because it had an old version.
            internal const int AppendPasswordConflict = 108; // Could not append to existing log file because passwords are different.
            internal const int NonFatalExceptionInLogger = 109; // Non-fatal exception occurred in TracerX.
            internal const int UnhandledExceptionInApp = 120; // An unhandled exception occurred somewhere in the app.

            // 201-300 = info
            internal const int LogFileOpened = 201;
            internal const int CircularLogStarted = 202;
            internal const int FirstWrap = 203;
            internal const int ConfigFileChanged = 204;
            internal const int LogFileReopening = 205; // Text file reached max size and is being restarted.

            private static int _internalMessageCount;
            private static string _internalLogFile;

            internal static void Log(string msg, int num)
            {
                // This is primarily called to log internal errors (e.g. can't open a log file).
                // To avoid flooding the event log, we only log the first 20 events.
                // Chances are the user doesn't have permission to write to the event log,
                // so we also pass the message to Trace.WriteLine() and write it to a
                // special file in the user's profile.

                try
                {
                    if (num <= MaxInternalEventNumber && _internalMessageCount < 20)
                    {
                        ++_internalMessageCount;

                        Trace.WriteLine(msg);

                        // Log the message to the application event log.
                        TryLogEvent(msg, num);

                        // Write the message to TracerXEvents.txt in the user's LocalApplicationData directory.
                        WriteToFallbackFile(msg);
                    }
                }
                catch (Exception)
                {
                    // Probably a permission issue, or maybe the fallback file is in use by another process.
                    // We've run out of places to log to.
                }
            }

            private static void TryLogEvent(string msg, int num)
            {
                try
                {
                    if (num <= 100)
                    {
                        EventLog.WriteEntry(msg, System.Diagnostics.EventLogEntryType.Error, (int)InternalEventOffset + num);
                    }
                    else if (num <= 200)
                    {
                        EventLog.WriteEntry(msg, System.Diagnostics.EventLogEntryType.Warning, (int)InternalEventOffset + num);
                    }
                    else
                    {
                        EventLog.WriteEntry(msg, System.Diagnostics.EventLogEntryType.Information, (int)InternalEventOffset + num);
                    }
                }
                catch (Exception)
                {
                    // Have seen "event log is full" exception and SecurityException here.  Can't really do anything about that,
                    // which is why we also write to Debug.Trace.WriteLine() and the file TracerXEvents.txt
                }
            }

            private static void WriteToFallbackFile(string msg)
            {
                // Write the message to TracerXEvents.txt in the user's LocalApplicationData directory.

                string time = DateTime.Now.ToString();

                if (_internalLogFile == null)
                {
                    // Since this is the first call/msg (for this process), initialize the full path.  
                    // On Windows 7, this is typically "C:\Users\<userid>\AppData\Local\TracerX\TracerXEvents.txt". 
                    // That directory is also used for TracerX-Service and TracerX-Viewer logs.

                    _internalLogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TracerX\\TracerXEvents.txt");
                    FileInfo fileInfo = new FileInfo(_internalLogFile);

                    // Ensure the directory exists or we can't create the file later.
                    fileInfo.Directory.Create();
                    GrantAccess(fileInfo.DirectoryName, fullControl: false, authenticatedUsers: true);

                    if (fileInfo.Exists && fileInfo.Length > 50000)
                    {
                        // Start a new file.
                        fileInfo.Delete();
                    }

                    try
                    {
                        // Delete old file we used to use.
                        string oldFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TracerXEvents.txt");
                        File.Delete(oldFile);
                    }
                    catch
                    {
                        // Not really important and probably no way to fix it.
                    }
                }

                // Include carriage returns because Notepad expects them.

                //string fileMsg = string.Format("---------------------------------------------------------------------------------------------------------\r\n{0} - Internal TracerX event (count = {1}) logged by '{2}' follows...\r\n{3}\r\n", time, _internalMessageCount, AppName, msg);
                StringBuilder fileMsg = new StringBuilder(msg.Length + 500);
                fileMsg.AppendLine("---------------------------------------------------------------------------------------------------------");
                fileMsg.AppendFormat("{0} - Internal TracerX event (count = {1}) logged by '{2}' follows...", time, _internalMessageCount, AppName);
                fileMsg.AppendLine();
                fileMsg.AppendLine(msg);

                if (_internalMessageCount == 20)
                {
                    // We've hit the limit and won't log any more events.
                    fileMsg.AppendFormat("'{0}'  will not log any more events unless restarted.", AppName);
                    fileMsg.AppendLine();
                }

                // This will create the file if it doesn't exist.
                File.AppendAllText(_internalLogFile, fileMsg.ToString());
            }

            #endregion

        }
    }
}