using System;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace TracerX {
    /// <summary>
    /// The Logger class is the user's primary interface to TracerX.  It manages a hierarchy
    /// of Logger instances that the user creates by calling the static GetLogger() method or
    /// by putting "Logger" nodes in the XML configuration file.
    /// The hierarchy is determined by the "dotted" substrings of each Logger's Name property
    /// (e.g. "A.B" is the parent of "A.B.C"). The static member Logger.Root is the root (ultimate 
    /// parent) of all loggers.  
    /// </summary>
    /// <remarks>
    /// Actual logging is done by calling instance methods such as Logger.Info() and Logger.Debug(),
    /// whose names imply the TraceLevel of the output.  The FileTraceLevel property determines 
    /// which calls send output to the log file (other properties control logging to the console,
    /// Trace.WriteLine, and the event log).  Setting FileTraceLevel higher results in more output.
    /// The default FileTraceLevel of every Logger except Root is Inherited.  Any logger a trace level 
    /// of Inherited inherits the trace level of its parent, all the way up to Root if needed.
    /// Users can control the logging of whole branches of the hierarchy by setting 
    /// FileTraceLevel to Debug, Info, Off, etc. in upper-Level loggers such as Logger.Root
    /// since the lower-Level loggers will inherit it unless explicitly overridden.
    /// </remarks>
    public partial class Logger {
        #region Public
        #region Logger hierarchy (GetLogger)
        /// <summary>
        /// Gets the logger with the specified name, creating it if necessary.
        /// Loggers created this way have the Inherited trace Level, causing
        /// the effective trace Level to be inheritted from the parent 
        /// logger (ultimately the Root logger).
        /// </summary>
        public static Logger GetLogger(string name) {
            Logger logger;
            lock (_loggers) {
                if (!_loggers.TryGetValue(name, out logger)) {
                    // Create a new logger and add it to the logger hierarchy.
                    logger = new Logger(name);
                    Logger.Root.AddLogger(logger);
                }
            }
            return logger;
        }

        /// <summary>
        /// Gets the logger with the name equal to type.ToString(), creating it if necessary.
        /// </summary>
        public static Logger GetLogger(Type type) {
            return GetLogger(type.ToString());
        }

        /// <summary>
        /// Returns info about all loggers in a string that contains the names, explicit trace levels, and
        /// effective trace levels in a hierarchical representation.  This is a debugging aid.
        /// </summary>
        public static string GetHierarchy() {
            lock (_loggers) {
                StringBuilder builder = new StringBuilder(500);
                Root.AppendHierarchy(0, builder);
                return builder.ToString();
            }
        }
        #endregion

        #region Static properties

        /// <summary>
        /// Methods and configuration for logging to the full-featured binary file supported by the viewer
        /// </summary>
        public static BinaryFile FileLogging { get { return BinaryFile.Singleton; } }

        /// <summary>
        /// Methods and configuration for logging to the text file (preferred by real men).
        /// </summary>
        public static TextFile TextFileLogging { get { return TextFile.Singleton; } }

        /// <summary>
        /// The parent/ancestor of all Loggers.  If desired, all logging can be done through this logger.
        /// </summary>
        public static readonly Logger Root; // Initialized by static ctor.

        /// <summary>
        /// The Logger used to log standard environment data when the log file is opened.
        /// </summary>
        public static readonly Logger StandardData; // Initialized by static ctor.

        /// <summary>
        /// The Current Logger is the last Logger that generated any output (to
        /// any destination) for the calling thread.  Use this Logger when you want to use the same Logger as
        /// earlier code used.  For example, you might use it in common code called from
        /// many places when you want the common code to use the same Logger as the 
        /// calling code.  Initialized to Root for each thread.
        /// </summary>
        public static Logger Current { get { return ThreadData.CurrentThreadData.LastLoggerAnyDest; } }

        /// <summary>
        /// The maximum number of events TracerX will log to the event log regarding
        /// unhandled exceptions that occur in the application.  TracerX detects these exceptions
        /// via the AppDomain.CurrentDomain.UnhandledException event.
        /// If you set this to zero, TracerX will not log these exeptions but unhandled exceptions 
        /// will be very hard to diagnose unless you have your own handler.
        /// </summary>
        public static uint MaxUnhandledExceptionsLogged {
            get { return _maxExceptionsLogged; }
            set { _maxExceptionsLogged = value; }
        }

        #endregion

        #region Instance properties

        /// <summary>
        /// The name of the logger determines how the logger fits in the hierarchy based on
        /// it's dotted notation (e.g. A.B.C is a child or grandchild of A).
        /// The name is set in the call to GetLogger and is read-only thereafter.
        /// </summary>
        public string Name { get { return _name; } }
        private readonly string _name;

        /// <summary>
        /// This controls which logging calls send output to the file.  Only those calls at levels
        /// less than or equal to FileTraceLevel go to the file.  If FileTraceLevel is set to
        /// Inherited, the getter returns the trace Level inherited from the parent logger.
        /// </summary>
        public TraceLevel FileTraceLevel {
            get { return FileLevels.EffectiveLevel; }
            set { SetTraceLevel(value, FileIndex); }
        }

        /// <summary>
        /// Similar to FileTraceLevel, but applies to console output.
        /// </summary>
        public TraceLevel TextFileTraceLevel {
            get { return TextFileLevels.EffectiveLevel; }
            set { SetTraceLevel(value, TextFileIndex); }
        }

        /// <summary>
        /// Similar to FileTraceLevel, but applies to console output.
        /// </summary>
        public TraceLevel ConsoleTraceLevel {
            get { return ConsoleLevels.EffectiveLevel; }
            set { SetTraceLevel(value, ConsoleIndex); }
        }

        /// <summary>
        /// Similar to FileTraceLevel, but applies to Debug output.
        /// </summary>
        public TraceLevel DebugTraceLevel {
            get { return DebugOutLevels.EffectiveLevel; }
            set { SetTraceLevel(value, DebugIndex); }
        }

        /// <summary>
        /// Similar to FileTraceLevel, but applies to event log output.
        /// </summary>
        public TraceLevel EventLogTraceLevel {
            get { return EventLogLevels.EffectiveLevel; }
            set { SetTraceLevel(value, EventIndex); }
        }
        #endregion

        #region Logging methods
        #region Fatal logging
        /// <summary>
        /// True if Fatal level logging is enabled for any destination.
        /// </summary>
        public bool IsFatalEnabled { get { return IsLevelEnabled(TraceLevel.Fatal); } }

        /// <summary>
        /// Log a single string message.
        /// </summary>
        public void Fatal(string msg) {
            MaybeLog(TraceLevel.Fatal, msg);
        }

        /// <summary>
        /// Log one object using the RendererMap.
        /// </summary>
        public void Fatal(object arg0) {
            MaybeLog(TraceLevel.Fatal, arg0);
        }

        /// <summary>
        /// Log two objects concatenated together using the RendererMap.
        /// </summary>
        public void Fatal(object arg0, object arg1) {
            MaybeLog(TraceLevel.Fatal, arg0, arg1);
        }

        /// <summary>
        /// Log many objects concatenated together using the RendererMap.
        /// </summary>
        public void Fatal(params object[] items) {
            MaybeLog(TraceLevel.Fatal, items);
        }

        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void FatalFormat(string fmt, object obj0) {
            MaybeLogFormat(TraceLevel.Fatal, fmt, obj0);
        }


        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void FatalFormat(string fmt, object obj0, object obj1) {
            MaybeLogFormat(TraceLevel.Fatal, fmt, obj0, obj1);
        }


        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void FatalFormat(string fmt, params object[] parms) {
            MaybeLogFormat(TraceLevel.Fatal, fmt, parms);
        }

        /// <summary>
        /// Log the entry of a method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder FatalCall() {
            return MaybeLogCall(TraceLevel.Fatal);
        }

        /// <summary>
        /// Log the entry of a manually specified method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder FatalCall(string methodName) {
            return MaybeLogCall(TraceLevel.Fatal, methodName);
        }
        #endregion

        #region Error logging
        /// <summary>
        /// True if Error level logging is enabled for any destination.
        /// </summary>
        public bool IsErrorEnabled { get { return IsLevelEnabled(TraceLevel.Error); } }

        /// <summary>
        /// Log a single string message.
        /// </summary>
        public void Error(string msg) {
            MaybeLog(TraceLevel.Error, msg);
        }

        /// <summary>
        /// Log one object using the RendererMap.
        /// </summary>
        public void Error(object arg0) {
            MaybeLog(TraceLevel.Error, arg0);
        }

        /// <summary>
        /// Log two objects concatenated together using the RendererMap.
        /// </summary>
        public void Error(object arg0, object arg1) {
            MaybeLog(TraceLevel.Error, arg0, arg1);
        }

        /// <summary>
        /// Log many objects concatenated together using the RendererMap.
        /// </summary>
        public void Error(params object[] items) {
            MaybeLog(TraceLevel.Error, items);
        }

        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void ErrorFormat(string fmt, object obj0) {
            MaybeLogFormat(TraceLevel.Error, fmt, obj0);
        }


        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void ErrorFormat(string fmt, object obj0, object obj1) {
            MaybeLogFormat(TraceLevel.Error, fmt, obj0, obj1);
        }

        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void ErrorFormat(string fmt, params object[] parms) {
            MaybeLogFormat(TraceLevel.Error, fmt, parms);
        }

        /// <summary>
        /// Log the entry of a method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder ErrorCall() {
            return MaybeLogCall(TraceLevel.Error);
        }

        /// <summary>
        /// Log the entry of a manually specified method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder ErrorCall(string methodName) {
            return MaybeLogCall(TraceLevel.Error, methodName);
        }

        #endregion

        #region Warn logging
        /// <summary>
        /// True if Warn level logging is enabled for any destination.
        /// </summary>
        public bool IsWarnEnabled { get { return IsLevelEnabled(TraceLevel.Warn); } }

        /// <summary>
        /// Log a single string message.
        /// </summary>
        public void Warn(string msg) {
            MaybeLog(TraceLevel.Warn, msg);
        }

        /// <summary>
        /// Log one object using the RendererMap.
        /// </summary>
        public void Warn(object arg0) {
            MaybeLog(TraceLevel.Warn, arg0);
        }

        /// <summary>
        /// Log two objects concatenated together using the RendererMap.
        /// </summary>
        public void Warn(object arg0, object arg1) {
            MaybeLog(TraceLevel.Warn, arg0, arg1);
        }

        /// <summary>
        /// Log many objects concatenated together using the RendererMap.
        /// </summary>
        public void Warn(params object[] items) {
            MaybeLog(TraceLevel.Warn, items);
        }

        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void WarnFormat(string fmt, object obj0) {
            MaybeLogFormat(TraceLevel.Warn, fmt, obj0);
        }


        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void WarnFormat(string fmt, object obj0, object obj1) {
            MaybeLogFormat(TraceLevel.Warn, fmt, obj0, obj1);
        }

        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void WarnFormat(string fmt, params object[] parms) {
            MaybeLogFormat(TraceLevel.Warn, fmt, parms);
        }

        /// <summary>
        /// Log the entry of a method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder WarnCall() {
            return MaybeLogCall(TraceLevel.Warn);
        }

        /// <summary>
        /// Log the entry of manually specified a method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder WarnCall(string methodName) {
            return MaybeLogCall(TraceLevel.Warn, methodName);
        }

        #endregion

        #region Info logging
        /// <summary>
        /// True if Info level logging is enabled for any destination.
        /// </summary>
        public bool IsInfoEnabled { get { return IsLevelEnabled(TraceLevel.Info); } }

        /// <summary>
        /// Log a single string message.
        /// </summary>
        public void Info(string msg) {
            MaybeLog(TraceLevel.Info, msg);
        }

        /// <summary>
        /// Log one object using the RendererMap.
        /// </summary>
        public void Info(object arg0) {
            MaybeLog(TraceLevel.Info, arg0);
        }

        /// <summary>
        /// Log two objects concatenated together using the RendererMap.
        /// </summary>
        public void Info(object arg0, object arg1) {
            MaybeLog(TraceLevel.Info, arg0, arg1);
        }

        /// <summary>
        /// Log many objects concatenated together using the RendererMap.
        /// </summary>
        public void Info(params object[] items) {
            MaybeLog(TraceLevel.Info, items);
        }

        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void InfoFormat(string fmt, object obj0) {
            MaybeLogFormat(TraceLevel.Info, fmt, obj0);
        }


        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void InfoFormat(string fmt, object obj0, object obj1) {
            MaybeLogFormat(TraceLevel.Info, fmt, obj0, obj1);
        }

        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void InfoFormat(string fmt, params object[] parms) {
            MaybeLogFormat(TraceLevel.Info, fmt, parms);
        }

        /// <summary>
        /// Log the entry of a method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder InfoCall() {
            return MaybeLogCall(TraceLevel.Info);
        }

        /// <summary>
        /// Log the entry of a manually specified method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder InfoCall(string methodName) {
            return MaybeLogCall(TraceLevel.Info, methodName);
        }

        #endregion

        #region Debug logging
        /// <summary>
        /// True if Debug level logging is enabled for any destination.
        /// </summary>
        public bool IsDebugEnabled { get { return IsLevelEnabled(TraceLevel.Debug); } }

        /// <summary>
        /// Log a single string message.
        /// </summary>
        public void Debug(string msg) {
            MaybeLog(TraceLevel.Debug, msg);
        }

        /// <summary>
        /// Log one object using the RendererMap.
        /// </summary>
        public void Debug(object arg0) {
            MaybeLog(TraceLevel.Debug, arg0);
        }

        /// <summary>
        /// Log two objects concatenated together using the RendererMap.
        /// </summary>
        public void Debug(object arg0, object arg1) {
            MaybeLog(TraceLevel.Debug, arg0, arg1);
        }

        /// <summary>
        /// Log many objects concatenated together using the RendererMap.
        /// </summary>
        public void Debug(params object[] items) {
            MaybeLog(TraceLevel.Debug, items);
        }

        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void DebugFormat(string fmt, object obj0) {
            MaybeLogFormat(TraceLevel.Debug, fmt, obj0);
        }


        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void DebugFormat(string fmt, object obj0, object obj1) {
            MaybeLogFormat(TraceLevel.Debug, fmt, obj0, obj1);
        }

        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void DebugFormat(string fmt, params object[] parms) {
            MaybeLogFormat(TraceLevel.Debug, fmt, parms);
        }

        /// <summary>
        /// Log the entry of a method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder DebugCall() {
            return MaybeLogCall(TraceLevel.Debug);
        }

        /// <summary>
        /// Log the entry of a method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder DebugCall(string methodName) {
            return MaybeLogCall(TraceLevel.Debug, methodName);
        }

        #endregion

        #region Verbose logging
        /// <summary>
        /// True if Verbose level logging is enabled for any destination.
        /// </summary>
        public bool IsVerboseEnabled { get { return IsLevelEnabled(TraceLevel.Verbose); } }

        /// <summary>
        /// Log a single string message.
        /// </summary>
        public void Verbose(string msg) {
            MaybeLog(TraceLevel.Verbose, msg);
        }

        /// <summary>
        /// Log one object using the RendererMap.
        /// </summary>
        public void Verbose(object arg0) {
            MaybeLog(TraceLevel.Verbose, arg0);
        }

        /// <summary>
        /// Log two objects concatenated together using the RendererMap.
        /// </summary>
        public void Verbose(object arg0, object arg1) {
            MaybeLog(TraceLevel.Verbose, arg0, arg1);
        }

        /// <summary>
        /// Log many objects concatenated together using the RendererMap.
        /// </summary>
        public void Verbose(params object[] items) {
            MaybeLog(TraceLevel.Verbose, items);
        }

        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void VerboseFormat(string fmt, object obj0) {
            MaybeLogFormat(TraceLevel.Verbose, fmt, obj0);
        }


        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void VerboseFormat(string fmt, object obj0, object obj1) {
            MaybeLogFormat(TraceLevel.Verbose, fmt, obj0, obj1);
        }

        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// </summary>
        public void VerboseFormat(string fmt, params object[] parms) {
            MaybeLogFormat(TraceLevel.Verbose, fmt, parms);
        }

        /// <summary>
        /// Log the entry of a method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder VerboseCall() {
            return MaybeLogCall(TraceLevel.Verbose);
        }

        /// <summary>
        /// Log the entry of a manually specified method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder VerboseCall(string methodName) {
            return MaybeLogCall(TraceLevel.Verbose, methodName);
        }

        #endregion
        #endregion
        #endregion

        #region Private/Internal
        // Set to true when System.Windows.Forms is detected.
        //private static bool _winFormsLoaded;

        //// Set to true by the static ctor if we seem be in a web app.
        //private static bool _webApp;

        /// <summary>
        /// Ctor is private.  GetLogger() should be the only caller.
        /// </summary>
        private Logger(string name) {
            _name = name;
            DestinationLevels = new LevelPair[] { FileLevels, TextFileLevels, ConsoleLevels, DebugOutLevels, EventLogLevels };
            _loggers.Add(name, this);
        }
        
        // The static ctor creates the Root logger.
        static Logger() {
            Root = new Logger("Root");
            Root.FileTraceLevel = TraceLevel.Info;
            Root.TextFileTraceLevel = TraceLevel.Off;
            Root.ConsoleTraceLevel = TraceLevel.Off;
            Root.DebugTraceLevel = TraceLevel.Off;
            Root.EventLogTraceLevel = TraceLevel.Off;

            // The StandardData logger is special in that it is initialized with a FileTraceLevel
            // of Verbose.  The user can change it programatically or via XML.
            StandardData = GetLogger("StandardData");
            StandardData.FileTraceLevel = TraceLevel.Verbose;

            AppDomain.CurrentDomain.UnhandledException += _appDomainExceptionHandler;
        }

        private static string ParseFormatString(string input) {
            StringBuilder builder = new StringBuilder(input);

            builder.Replace("{line", "{0");
            builder.Replace("{level", "{1");
            builder.Replace("{logger", "{2");
            builder.Replace("{thnum", "{3");
            builder.Replace("{thname", "{4");
            builder.Replace("{time", "{5");
            builder.Replace("{method", "{6");
            builder.Replace("{ind", "{7");
            builder.Replace("{msg", "{8");

            return builder.ToString();
        }

        // If we are not a web app, this will throw an exception the
        // caller must handle.  This will load the System.Web assembly
        // if it hasn't already been loaded.
        private static string GetWebAppDir() {
            return System.Web.HttpRuntime.AppDomainAppPath.TrimEnd('\\', '/');
        }

        // Get the application name without loading System.Windows.Forms.dll
        // or calling Process.GetProcess(), which requires significant permission.
        private static string GetAppName() {
            try {
                // This usually throws an exception in web apps and sometimes in
                // the winforms designer.
                return Assembly.GetEntryAssembly().GetName().Name;
            } catch (Exception) {
                try {
                    // Expect an exception if we're not a web app.
                    return Path.GetFileNameWithoutExtension(GetWebAppDir());
                } catch (Exception) {
                    try {
                        // This is a good result for winforms, but not very
                        // pretty for web apps.
                        return AppDomain.CurrentDomain.FriendlyName;
                    } catch (Exception) {
                        // Can't think of anything else to try, but we must
                        // return something and not allow an exception.
                        return "TracerX_App";
                    }
                }
            }
        }

        #region Trace levels
        // Each logging destination has an explicit trace level and an effective trace level that is
        // inherited from the parent logger if the explicit trace level is Inherited.
        private class LevelPair {
            public TraceLevel SetLevel = TraceLevel.Inherited;       // The explicitly set trace Level.
            public TraceLevel EffectiveLevel = TraceLevel.Inherited; // Possibly inherited trace Level.

            public bool MaybeInherit(LevelPair parentLevels) {
                if (SetLevel == TraceLevel.Inherited) {
                    EffectiveLevel = parentLevels.EffectiveLevel;
                    return true;
                } else {
                    EffectiveLevel = SetLevel;
                }

                return false;
            }
        }

        // The trace levels for each destination.
        private LevelPair FileLevels = new LevelPair();
        private LevelPair TextFileLevels = new LevelPair();
        private LevelPair ConsoleLevels = new LevelPair();
        private LevelPair DebugOutLevels = new LevelPair();
        private LevelPair EventLogLevels = new LevelPair();

        LevelPair[] DestinationLevels;
        const int FileIndex = 0;
        const int TextFileIndex = 1;
        const int ConsoleIndex = 2;
        const int DebugIndex = 3;
        const int EventIndex = 4;

        private TraceLevel _maxLevel; // Max effective trace level of all destinations.

        private void SetTraceLevel(TraceLevel value, int index) {
            // Lock _loggers to protect the collection of loggers,
            // the hierarchy of loggers, and the inherited trace levels.
            lock (_loggers) {
                if (DestinationLevels[index].SetLevel != value) {
                    if (this == Root && value == TraceLevel.Inherited) {
                        // The Root logger is not allowed to have the Inherited trace level
                        return;
                    }

                    DestinationLevels[index].SetLevel = value;

                    if (value == TraceLevel.Inherited && parent != null) {
                        DestinationLevels[index].EffectiveLevel = parent.DestinationLevels[index].EffectiveLevel;
                    } else {
                        DestinationLevels[index].EffectiveLevel = value;
                    }

                    ComputeMaxLevel();
                    SetInheritedTraceLevels(index);
                }
            }
        }

        /// <summary>
        /// Returns true if the specified message TraceLevel is enabled for any destination.
        /// </summary>
        private bool IsLevelEnabled(TraceLevel msgLevel) {
            return this._maxLevel >= msgLevel;
        }
        #endregion

        #region Message logging
        private void LogToDestinations(ThreadData threadData, TraceLevel msgLevel, string msg) {
            try {
                if (FileTraceLevel >= msgLevel) FileLogging.LogMsg(this, threadData, msgLevel, msg);
                if (TextFileTraceLevel >= msgLevel) TextFileLogging.LogMsg(this, threadData, msgLevel, msg);
                if (ConsoleTraceLevel >= msgLevel) ConsoleLogging.LogMsg(this, threadData, msgLevel, msg);
                if (DebugTraceLevel >= msgLevel) DebugLogging.LogMsg(this, threadData, msgLevel, msg);
                if (EventLogTraceLevel >= msgLevel) EventLogging.LogMsg(this, threadData, msgLevel, msg);

                threadData.LastLoggerAnyDest = this;
            } catch (Exception) {
                // TODO: What?
            }
        }

        /// <summary>
        /// Log a single string message.
        /// </summary>
        private void MaybeLog(TraceLevel msgLevel, string msg) {
            if (IsLevelEnabled(msgLevel)) {
                // At least one destination is enabled at this level.
                LogToDestinations(ThreadData.CurrentThreadData, msgLevel, msg);
            }
        }

        /// <summary>
        /// Log one object using the RendererMap.
        /// </summary>
        private void MaybeLog(TraceLevel msgLevel, object arg0) {
            ThreadData threadData = ThreadData.CurrentThreadData;
            if (IsLevelEnabled(msgLevel)) {
                RendererMap.FindAndRender(arg0, threadData.StringWriter);
                string msg = threadData.ResetStringWriter();
                LogToDestinations(threadData, msgLevel, msg);
            }
        }

        /// <summary>
        /// Log two objects concatenated together using the RendererMap.
        /// </summary>
        private void MaybeLog(TraceLevel msgLevel, object arg0, object arg1) {
            ThreadData threadData = ThreadData.CurrentThreadData;
            if (IsLevelEnabled(msgLevel)) {
                RendererMap.FindAndRender(arg0, threadData.StringWriter);
                RendererMap.FindAndRender(arg1, threadData.StringWriter);
                string msg = threadData.ResetStringWriter();
                LogToDestinations(threadData, msgLevel, msg);
            }
        }

        /// <summary>
        /// Log many objects concatenated together using the RendererMap.
        /// </summary>
        private void MaybeLog(TraceLevel msgLevel, params object[] items) {
            ThreadData threadData = ThreadData.CurrentThreadData;
            if (IsLevelEnabled(msgLevel)) {
                foreach (object o in items) {
                    RendererMap.FindAndRender(o, threadData.StringWriter);
                }
                string msg = threadData.ResetStringWriter();
                LogToDestinations(threadData, msgLevel, msg);
            }
        }

        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// This does NOT use the RendererMap.
        /// </summary>
        private void MaybeLogFormat(TraceLevel msgLevel, string fmt, object obj0) {
            if (IsLevelEnabled(msgLevel)) {
                LogToDestinations(ThreadData.CurrentThreadData, msgLevel, string.Format(fmt, obj0));
            }
        }

        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// This does NOT use the RendererMap.
        /// </summary>
        private void MaybeLogFormat(TraceLevel msgLevel, string fmt, object obj0, object obj1) {
            if (IsLevelEnabled(msgLevel)) {
                LogToDestinations(ThreadData.CurrentThreadData, msgLevel, string.Format(fmt, obj0, obj1));
            }
        }

        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// This does NOT use the RendererMap
        /// </summary>
        private void MaybeLogFormat(TraceLevel msgLevel, string fmt, params object[] parms) {
            if (IsLevelEnabled(msgLevel)) {
                LogToDestinations(ThreadData.CurrentThreadData, msgLevel, string.Format(fmt, parms));
            }
        }
        #endregion

        #region Method entry/exit logging
        // Constructs a flag enum indicating which destinations' levels are enabled.
        internal Destination Destinations(TraceLevel level) {
            Destination destinations = Destination.None;

            if (FileTraceLevel >= level) destinations |= Destination.File;
            if (TextFileTraceLevel >= level) destinations |= Destination.TextFile;
            if (ConsoleTraceLevel >= level) destinations |= Destination.Console;
            if (DebugTraceLevel >= level) destinations |= Destination.Debug;
            if (EventLogTraceLevel >= level) destinations |= Destination.EventLog;

            return destinations;
        }

        private CallEnder MaybeLogCall(TraceLevel level) {
            ThreadData threadData = ThreadData.CurrentThreadData;
            if (IsLevelEnabled(level)) {
                string caller = GetCaller();
                if (threadData.LogCallEntry(this, level, caller)) {
                    // MyCallEnder will log the exit when it is disposed.
                    return MyCallEnder;
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        private CallEnder MaybeLogCall(TraceLevel level, string methodName) {
            ThreadData threadData = ThreadData.CurrentThreadData;
            if (IsLevelEnabled(level)) {
                if (threadData.LogCallEntry(this, level, methodName)) {
                    // MyCallEnder will log the exit when it is disposed.
                    return MyCallEnder;
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        private static int _callerDepth = 1;

        /// <summary>
        /// Searches the call stack for the first StackFrame that is not
        /// for a method from this class.  We can't hard-code the depth
        /// because the optimizer sometimes inlines the calling method(s)
        /// from this class.
        /// </summary>
        private static string GetCaller() {
            for (int depth =_callerDepth; depth < 4; ++depth) {
                StackFrame frame = new StackFrame(depth);
                System.Reflection.MethodBase method = frame.GetMethod();
                Type type = method.DeclaringType;

                if (type != typeof(TracerX.Logger)) {
                    _callerDepth = depth;
                    return type.Name + '.' + method.Name;
                }
            }

            return "Unknown Method";
        }

        /// <exclude/>
        /// <summary>
        /// The Logger.*Call methods log a call to a method and return an instance of this class. 
        /// Its Dispose method logs the exit of the call.
        /// Users should not create instances of this object.  
        /// </summary>
        public class CallEnder : IDisposable {
            /// <summary>
            /// Internal ctor keeps logging clients from creating instances.
            /// </summary>
            internal CallEnder() { }

            /// <summary>
            /// If MaybeLogCall logged entry into a call, this logs the exit.
            /// </summary>
            public void Dispose() {
                ThreadData.CurrentThreadData.LogCallExit();
            }
        } // BlockEnder

        // The only instance ever needed.
        private static readonly CallEnder MyCallEnder = new CallEnder();
        #endregion

        #region Logger hierarchy
        // Dictionary of all loggers.
        private static readonly Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>();
        private List<Logger> _children;         // Null if no children.
        private Logger parent;                  // Only null in Root Logger.

        /// <summary>
        /// Makes the specified Logger this logger's child or sub-child and sets
        /// _effectiveTraceLevel for the specified Logger and any children.
        /// Thread safety is ensured by the caller.
        /// </summary>
        internal void AddLogger(Logger logger) {
            AddLoggerPrivate(logger);

            for (int i = 0; i < DestinationLevels.Length; ++i) {
                // Either the new logger inherits from the its parent, or its children need to
                // inherit from it.
                if (logger.DestinationLevels[i].MaybeInherit(logger.parent.DestinationLevels[i])) {
                    logger.ComputeMaxLevel();
                } else {
                    // The new logger may have been inserted as the parent of some
                    // existing loggers. Any such children may need to inherit the 
                    // new logger's trace Level.
                    logger.SetInheritedTraceLevels(i);
                }
            }
        }

        private void AddLoggerPrivate(Logger logger) {
            // See if the new logger should be a child of any of our children.
            // If not, it will become our child.
            string newNameWithDot = logger.Name + ".";
            bool movedChildren = false;

            if (_children == null) {
                _children = new List<Logger>();
            } else {
                foreach (Logger child in _children) {
                    if (child.Name.StartsWith(newNameWithDot)) {
                        // The new logger should be a parent of the current child
                        // and should replace the current child in the list.
                        // Note that there may be several such children.  E.g.,
                        // we have children A.B.1 and A.B.2, and the new logger is A.B or A.
                        // Therefore, keep looping.
                        logger.AddLoggerPrivate(child);
                        movedChildren = true;
                    } else if (!movedChildren) {
                        string childNameWithDot = child.Name + ".";
                        if (logger.Name.StartsWith(childNameWithDot)) {
                            // The new logger should be a child of the current child.
                            child.AddLoggerPrivate(logger);
                            return;
                        }
                    }
                }

                // Getting here means the new logger will become our child.
                
                if (movedChildren) {
                    // Some of our children are now the new logger's children.
                    // Replace them with the new logger.
                    foreach (Logger child in logger._children) {
                        _children.Remove(child);
                    }
                }
            }

            _children.Add(logger);
            logger.parent = this;
        }

        // Push our effective trace levels down to any children that have an Inherited trace level.
        // Called when any trace level changes or a new logger with an explicit trace level is
        // inserted into the hierarchy.
        private void SetInheritedTraceLevels(int destinationIndex) {
            if (_children != null) {
                foreach (Logger child in _children) {
                    if (child.DestinationLevels[destinationIndex].MaybeInherit(this.DestinationLevels[destinationIndex])) {
                        child.ComputeMaxLevel();
                        child.SetInheritedTraceLevels(destinationIndex);
                    }
                }
            }
        }

        private void ComputeMaxLevel() {
            _maxLevel = TraceLevel.Off;
            foreach (LevelPair pair in DestinationLevels) {
                if (pair.EffectiveLevel > _maxLevel) _maxLevel = pair.EffectiveLevel;
            }
        }

        // Append our information and our children's information to the builder.
        private void AppendHierarchy(int indent, StringBuilder builder) {
            builder.AppendFormat("{0}{1} {2}/{3}, {4}/{5}, {6}/{7}, {8}/{9} , {10}/{11} \n",
                new string(' ', indent),     
                Name,
                FileLevels.SetLevel, FileLevels.EffectiveLevel,
                TextFileLevels.SetLevel, TextFileLevels.EffectiveLevel,
                ConsoleLevels.SetLevel, ConsoleLevels.EffectiveLevel,
                DebugOutLevels.SetLevel, DebugOutLevels.EffectiveLevel,
                EventLogLevels.SetLevel, EventLogLevels.EffectiveLevel
                );
            if (_children != null) {
                foreach (Logger child in _children) {
                    child.AppendHierarchy(indent + 2, builder);
                }
            }
        }
        #endregion
        
        #region Unhandled exception logging
        // Count the number of unhandled exceptions logged by TracerX and don't exceed the max.
        private static int _exceptionsLogged;
        private static uint _maxExceptionsLogged = 3;

        //private static System.Threading.ThreadExceptionEventHandler _threadExceptionHandler = 
        //    delegate (object sender, System.Threading.ThreadExceptionEventArgs e) {
        //        if (_exceptionsLogged < MaxUnhandledExceptionsLogged) {
        //            ++_exceptionsLogged;
        //            EventLogging.Log("An unhandled exception was passed to TracerX's handler for the Application.ThreadException event.\n\n" + e.Exception.ToString(), EventLogging.UnhandledExceptionInApp);
        //        } else {
        //            Logger.Root.Fatal("An unhandled exception was passed to TracerX's handler for the Application.ThreadException event, but was not logged to the event log.\n", e.Exception);
        //        }
        //    };

        private static UnhandledExceptionEventHandler _appDomainExceptionHandler =
            delegate(object sender, UnhandledExceptionEventArgs e) {
                if (_exceptionsLogged < MaxUnhandledExceptionsLogged) {
                    ++_exceptionsLogged;
                    EventLogging.Log("An unhandled exception was passed to TracerX's handler for the AppDomain.CurrentDomain.UnhandledException event.\n\n" + e.ExceptionObject.ToString(), EventLogging.UnhandledExceptionInApp);
                } else {
                    Logger.Root.Fatal("An unhandled exception was passed to TracerX's handler for the AppDomain.CurrentDomain.UnhandledException event, but was not logged to the event log.\n", e.ExceptionObject);
                }
            };       
        #endregion 
        #endregion
    }
}
