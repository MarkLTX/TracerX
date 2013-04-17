using System;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;

namespace TracerX {
    /// <summary>
    /// The Logger class is your primary interface to TracerX.  
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each Logger you create is automatically inserted into a hierarchy of Loggers.  That is, each Logger has a parent Logger
    /// (except the static <see cref="Logger.Root"/> Logger) and possibly children as well.
    /// The hierarchy is determined by the "dotted" substrings of each Logger's name
    /// (e.g. "A.B" is the parent of "A.B.C"). The static member <see cref="Logger.Root"/> is the root (ultimate 
    /// parent) of all Loggers.  
    /// </para>
    /// <para>
    /// If desired, all logging can be done through the static Logger instance <see cref="Logger.Root"/>.  However, standard practice is to
    /// create your own instances by calling the static method <see cref="Logger.GetLogger(string)"/>, which allows you to specify the
    /// returned Logger's name.  The viewer's "Logger" column displays the name of the Logger object that created each row of output.
    /// You can also create and configure Loggers via the XML configuration file, but you must still call GetLogger() with
    /// the name specified in the XML file to get references to those instances in your code.
    /// </para>
    /// <para>
    /// Actual logging is done by calling instance methods such as <see cref="Info(object)"/> and <see cref="Debug(object)"/>,
    /// whose names imply the <see cref="TraceLevel"/> of the log message. "Higher" TraceLevels generally correspond to higher
    /// levels of detail/volume of output, and to less important messages.  
    /// </para>
    /// <para>
    /// Each log message can go to a combination of several destinations.  Each destination has a property in the Logger class that 
    /// specifies the maximum TraceLevel sent to that destination by that Logger instance. Setting a given destination's maximum TraceLevel 
    /// to a higher value causes more output to be sent to that destination, assuming there are any messages being logged at
    /// the higher levels.
    /// </para>
    /// <para>
    /// The <see cref="BinaryFileTraceLevel"/> property specifies the maximum level of output sent to the binary log file.
    /// The default value for this property is TraceLevel.Info (inherited from the Root Logger), which means Debug and Verbose
    /// output will be suppressed unless you set this to a higher value. The binary file must be viewed with the TracerX-Viewer 
    /// application, which has powerful filtering, navigating, coloring and other features.  Since the viewer
    /// only works with the binary file, it should be your primary logging destination.  Additional configuration of the binary
    /// file, such as the file name and location, is specified via the <see cref="Logger.BinaryFile"/> object.
    /// Also, you must call BinaryFile.Open() to open the file.
    /// </para>
    /// <para>
    /// The <see cref="TextFileTraceLevel"/> property specifies the maximum level of output sent to the text log file. 
    /// The default value for this property is TraceLevel.Off (inherited from the Root Logger).
    /// Additional configuration of the text file, such as the file name, location, and format string, is specified via the
    /// static <see cref="Logger.TextFileLogging"/> object. Also, you must call Logger.TextFileLogging.Open() to open the file.
    /// </para>
    /// <para>
    /// The <see cref="ConsoleTraceLevel"/> property specifies the maximum level of output sent to the console (i.e. command window). 
    /// The default value for this property is TraceLevel.Off (inherited from the Root Logger).
    /// Additional configuration of the console destination, such as the format string, is specified via the
    /// static <see cref="Logger.ConsoleLogging"/> object.
    /// </para>
    /// <para>
    /// The <see cref="DebugTraceLevel"/> property specifies the maximum level of output logged by calling Trace.WriteLine(). 
    /// The default value for this property is TraceLevel.Off (inherited from the Root Logger).
    /// Additional configuration of the debug destination, such as the format string, is specified via the
    /// static <see cref="Logger.DebugLogging"/> class.
    /// </para>
    /// <para>
    /// The <see cref="EventLogTraceLevel"/> property specifies the maximum level of output sent to the event log. 
    /// The default value for this property is TraceLevel.Off (inherited from the Root Logger).
    /// Additional configuration of the event log destination, such as which event log, the event source, and the format string, are specified via the
    /// static <see cref="Logger.EventLogging"/> class.
    /// </para>
    /// <para>
    /// The <see cref="EventHandlerTraceLevel"/> property specifies the maximum level of output that raises the static <see cref="Logger.MessageCreated"/> event. 
    /// The default value for this property is TraceLevel.Off (inherited from the Root Logger).
    /// By handling this event, you can perform custom processing for each log message such as playing a sound sending an email, or even cancelling the message.
    /// </para>
    /// <para>
    /// Setting any of the above TraceLevels to TraceLevel.Inherited causes the effective value to be inherited from the parent Logger (possibly
    /// all the way up to Logger.Root).  All these TraceLevel properties are initialized to Inherited in all Loggers you create, meaning their effective values
    /// are inherited from Root.  You can control the logging of whole branches of the hierarchy by setting the TraceLevels in in upper-Level 
    /// Loggers, including Logger.Root.
    /// </para>
    /// </remarks>    
    public partial class Logger : MarshalByRefObject {
        #region Public
        #region Logger hierarchy (GetLogger)
        /// <summary>
        /// Gets the <see cref="Logger"/> instance with the specified name, creating it if necessary.
        /// Loggers created this way have the Inherited <see cref="TraceLevel"/> for all destinations, causing
        /// the effective TraceLevels to be inheritted from the parent 
        /// logger (ultimately the <see cref="Root"/> logger).
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
        /// Gets the Logger with the name equal to type.ToString(), creating it if necessary.
        /// </summary>
        public static Logger GetLogger(Type type) {
            return GetLogger(type.ToString());
        }

        /// <summary>
        /// Gets the Logger with the specified name from the specified AppDomain, creating it
        /// in that AppDomain if it doesn't already exist.  Any logging done through the returned
        /// Logger will be sent to the file, event log, or other destinations as configured by
        /// the specified AppDomain.  This allows multiple AppDomains to log to a single file.
        /// </summary>
        /// <param name="name">The name of the Logger to get from the specified AppDomain.</param>
        /// <param name="appDomain">The AppDomain in which to create the Logger.</param>
        public static Logger GetLogger(string name, AppDomain appDomain)
        {
            AppDomainHelper helper = (AppDomainHelper)appDomain.CreateInstanceAndUnwrap(
                typeof(AppDomainHelper).Assembly.FullName,
                typeof(AppDomainHelper).FullName
                );

            return helper.GetLogger(name);
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

        /// <summary>
        /// Gets the collection of Logger objects.  Note that iterating over this list is
        /// not thread-safe since any thread can add Loggers at any time.
        /// </summary>
        public static Dictionary<string,Logger>.ValueCollection LoggerList
        {
            get
            {
                return _loggers.Values;
            }
        }

        #endregion

        #region Static properties

        /// <summary>
        /// The default binary output file used by all Logger instances.
        /// This is the full-featured binary file supported by the viewer.
        /// Output to this file is filtered by the <see cref="Logger.BinaryFileTraceLevel"/> property.
        /// Individual Loggers can log to a different file by setting the <see cref="BinaryFile"/> property.
        /// </summary>
        public static BinaryFile DefaultBinaryFile { get; private set; }

        /// <summary>
        /// The default text output file used by all Logger instances.
        /// Output to this file is filtered by the <see cref="Logger.TextFileTraceLevel"/> property.
        /// </summary>
        public static TextFile DefaultTextFile { get; private set; }

        /// <summary>
        /// Text to substitute for null strings and other objects passed to logging methods.
        /// Default is &lt;null&gt;,but can be anything except null (including string.Empty).
        /// </summary>
        public static string TextForNull 
        {
            get { return _textForNull; }
            
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("TextForNull", "Logger.TextForNull can't be null.");
                }
                else
                {
                    _textForNull = value;
                }
            }
        }

        /// <summary>
        /// This event is raised when a <see cref="Logger"/> is passed a message whose <see cref="TraceLevel"/> is
        /// less than or equal to the <see cref="Logger.EventHandlerTraceLevel"/> property.
        /// The <see cref="MessageCreatedEventArgs"/> passed to the handlers contains the message text, TraceLevel,
        /// and other information about the message.  The event is raised on the same thread that logged the message.
        /// The event source is the Logger the message was logged through.
        /// </summary>
        public static event EventHandler<MessageCreatedEventArgs> MessageCreated;

        /// <summary>
        /// The parent/ancestor of all Loggers.  If desired, all logging can be done through this logger.
        /// </summary>
        public static readonly Logger Root; // Initialized by static ctor.

        /// <summary>
        /// The Logger used to log standard environment data when the log file is opened.
        /// You can configure this Logger to reduce or complement suppress that output.
        /// </summary>
        public static readonly Logger StandardData; // Initialized by static ctor.

        /// <summary>
        /// The Current Logger is the last Logger that generated any output (to
        /// any destination) for the calling thread.  Use this Logger when you want to use the same Logger as
        /// earlier code used.  For example, you might use it in common code called from
        /// many places when you want the common code to use the same Logger as the 
        /// calling code.  Initialized to Logger.Root for each thread.
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

        /// <summary>
        /// Gets and sets the thread name written to the log file for the current thread.  
        /// Setting this effectively overrides (but does not change) Thread.CurrentThread.Name 
        /// within TracerX.  Unlike Thread.CurrentThread.Name, this can be changed many times.  
        /// If set to null (the default value), TracerX uses Thread.CurrentThread.Name.  
        /// </summary>
        public static string ThreadName {
            get { return ThreadData.CurrentThreadData.Name; }
            set { ThreadData.CurrentThreadData.Name = value; }
        }

        /// <summary>
        /// Deprecated.  Use <see cref="DefaultBinaryFile"/> instead.
        /// </summary>
        [Obsolete("Use DefaultBinaryFile instead.", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public static BinaryFile FileLogging { get { return DefaultBinaryFile; } }

        /// <summary>
        /// Deprecated.  Use <see cref="DefaultBinaryFile"/> instead.
        /// </summary>
        [Obsolete("Use DefaultBinaryFile instead.", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public static BinaryFile BinaryFileLogging { get { return DefaultBinaryFile; } }

        /// <summary>
        /// Deprecated.  Use <see cref="DefaultTextFile"/> instead.
        /// </summary>
        [Obsolete("Use DefaultTextFile instead.", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public static TextFile TextFileLogging { get { return DefaultTextFile; } }

        #endregion

        #region Instance properties

        /// <summary>
        /// The name of the logger determines how the logger fits in the hierarchy based on
        /// it's dotted notation (e.g. A.B.C is a child or grandchild of A).
        /// The name is set in the call to <see cref="GetLogger(string)"/> and is read-only thereafter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The <see cref="BinaryFile"/> instance used by this Logger instance.  Initially equal to Logger.DefaultBinaryFile.
        /// Once the Logger writes to the file, BinaryFileIsCommitted becomes true and BinaryFile cannot be set to a different
        /// BinaryFile.  Attempting to do so throws an exception.
        /// </summary>
        public BinaryFile BinaryFile
        {
            get { return _binaryFile; } // No lock. C# spec says this is atomic.

            set
            {
                lock (_loggerLock)
                {
                    if (_binaryFile != value)
                    {                        
                        if (_isBinaryFileCommitted)
                        {
                            // The reason for this is that it would be difficult to support logging method-entries to one
                            // file and logging the corresponding method-exits to another file.
                            throw new Exception("Logger.BinaryFile cannot be changed after the Logger has written to the file.");
                        }
                        else
                        {
                            _binaryFile = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initially false, this becomes true when this Logger first writes to its BinaryFile.  After that,
        /// BinaryFile can't be changed.
        /// </summary>
        public bool IsBinaryFileCommitted
        {
            get { return _isBinaryFileCommitted; }
        }

        /// <summary>
        /// This controls which logging calls send output to the <see cref="BinaryFile"/>.  Only those calls at levels
        /// less than or equal to BinaryFileTraceLevel go to the file.  If BinaryFileTraceLevel is set to
        /// Inherited, the get accessor returns the trace Level inherited from the parent logger.
        /// </summary>
        public TraceLevel BinaryFileTraceLevel {
            get { return BinaryFileLevels.EffectiveLevel; }
            set { SetTraceLevel(value, BinaryFileIndex); }
        }


        /// <summary>
        /// The <see cref="TextFile"/> instance used by this Logger instance.  Initially equal to Logger.DefaultTextFile.
        /// Once the Logger writes to the file, TextFileIsCommitted becomes true and TextFile cannot be set to a different
        /// TextFile.  Attempting to do so throws an exception.
        /// </summary>
        public TextFile TextFile
        {
            get { return _textFile; }

            set
            {
                lock (_loggerLock)
                {
                    if (_textFile != value)
                    {                        
                        if (_isTextFileCommitted)
                        {
                            // The reason for this is that it would be difficult to support logging method-entries to one
                            // file and logging the corresponding method-exits to another file.
                            throw new Exception("Logger.BinaryFile cannot be changed after the Logger has written to the file.");
                        }
                        else
                        {
                            _textFile = value;
                        }
                    }
                }
            }
        }

        public bool IsTextFileCommitted
        {
            get { return _isTextFileCommitted; }
        }


        /// <summary>
        /// Similar to <see cref="BinaryFileTraceLevel"/>, but applies to text file output.
        /// See the <see cref="TextFile"/> property.
        /// </summary>
        public TraceLevel TextFileTraceLevel {
            get { return TextFileLevels.EffectiveLevel; }
            set { SetTraceLevel(value, TextFileIndex); }
        }

        /// <summary>
        /// Similar to <see cref="BinaryFileTraceLevel"/>, but applies to console output. 
        /// See the <see cref="ConsoleLogging"/> class.
        /// </summary>
        public TraceLevel ConsoleTraceLevel {
            get { return ConsoleLevels.EffectiveLevel; }
            set { SetTraceLevel(value, ConsoleIndex); }
        }

        /// <summary>
        /// Similar to <see cref="BinaryFileTraceLevel"/>, but applies to Debug output (i.e. passed to Trace.WriteLine()).
        /// See the <see cref="DebugLogging"/> class.
        /// </summary>
        public TraceLevel DebugTraceLevel {
            get { return DebugOutLevels.EffectiveLevel; }
            set { SetTraceLevel(value, DebugIndex); }
        }

        /// <summary>
        /// Similar to <see cref="BinaryFileTraceLevel"/>, but applies to event log output.
        /// See the <see cref="EventLogging"/> class.
        /// </summary>
        public TraceLevel EventLogTraceLevel
        {
            get { return EventLogLevels.EffectiveLevel; }
            set { SetTraceLevel(value, EventLogIndex); }
        }

        /// <summary>
        /// Similar to <see cref="BinaryFileTraceLevel"/>, but applies to the static <see cref="MessageCreated"/> event.
        /// </summary>
        public TraceLevel EventHandlerTraceLevel
        {
            get { return EventHandlerLevels.EffectiveLevel; }
            set { SetTraceLevel(value, EventHandlerIndex); }
        }

        /// <summary>
        /// Arbitrary data you can attach to a Logger object.
        /// </summary>
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Deprected.  Use <see cref="BinaryFileTraceLevel"/> instead.
        /// </summary>
        [Obsolete("Use BinaryFileTraceLevel instead.", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public TraceLevel FileTraceLevel
        {
            get { return BinaryFileTraceLevel; }
            set { BinaryFileTraceLevel = value; }
        }

        #endregion

        #region Logging methods

        /// <summary>
        /// Not intended for general use. Logs only to the binary file using the explicitly specified timestamp. 
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public void Explicit(TraceLevel level, DateTime explicitUtcTime, string msg)
        {
            if (BinaryFileTraceLevel >= level)
            {
                CommitToBinaryFile();
                BinaryFile.LogMsg(explicitUtcTime, this, level, msg);
            }
        }

        #region Fatal logging

        /// <summary>
        /// True if Fatal level logging is enabled for any destination.
        /// </summary>
        public bool IsFatalEnabled { get { return IsLevelEnabled(TraceLevel.Fatal); } }

        /// <summary>
        /// Logs a single string message.
        /// </summary>
        public void Fatal(string msg) {
            MaybeLog(TraceLevel.Fatal, msg);
        }

        /// <summary>
        /// Logs one object (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Fatal(object arg0) {
            MaybeLog(TraceLevel.Fatal, arg0);
        }

        /// <summary>
        /// Logs two objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Fatal(object arg0, object arg1) {
            MaybeLog(TraceLevel.Fatal, arg0, arg1);
        }

        /// <summary>
        /// Logs many objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Fatal(params object[] items) {
            MaybeLog(TraceLevel.Fatal, items);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void FatalFormat(string fmt, object obj0) {
            MaybeLogFormat(TraceLevel.Fatal, fmt, obj0);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void FatalFormat(string fmt, object obj0, object obj1) {
            MaybeLogFormat(TraceLevel.Fatal, fmt, obj0, obj1);
        }
        
        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void FatalFormat(string fmt, params object[] parms) {
            MaybeLogFormat(TraceLevel.Fatal, fmt, parms);
        }

        /// <summary>
        /// Logs the entry of a method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder FatalCall() {
            return MaybeLogCall(TraceLevel.Fatal, null);
        }

        /// <summary>
        /// Logs the entry of a manually specified method call. Use with "using" so the returned object's Dispose
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
        /// Logs a single string message.
        /// </summary>
        public void Error(string msg) {
            MaybeLog(TraceLevel.Error, msg);
        }

        /// <summary>
        /// Logs one object (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Error(object arg0) {
            MaybeLog(TraceLevel.Error, arg0);
        }

        /// <summary>
        /// Logs two objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Error(object arg0, object arg1) {
            MaybeLog(TraceLevel.Error, arg0, arg1);
        }

        /// <summary>
        /// Logs many objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Error(params object[] items) {
            MaybeLog(TraceLevel.Error, items);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void ErrorFormat(string fmt, object obj0) {
            MaybeLogFormat(TraceLevel.Error, fmt, obj0);
        }


        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void ErrorFormat(string fmt, object obj0, object obj1) {
            MaybeLogFormat(TraceLevel.Error, fmt, obj0, obj1);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void ErrorFormat(string fmt, params object[] parms) {
            MaybeLogFormat(TraceLevel.Error, fmt, parms);
        }

        /// <summary>
        /// Logs the entry of a method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder ErrorCall() {
            return MaybeLogCall(TraceLevel.Error, null);
        }

        /// <summary>
        /// Logs the entry of a manually specified method call. Use with "using" so the returned object's Dispose
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
        /// Logs a single string message.
        /// </summary>
        public void Warn(string msg) {
            MaybeLog(TraceLevel.Warn, msg);
        }

        /// <summary>
        /// Logs one object (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Warn(object arg0) {
            MaybeLog(TraceLevel.Warn, arg0);
        }

        /// <summary>
        /// Logs two objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Warn(object arg0, object arg1) {
            MaybeLog(TraceLevel.Warn, arg0, arg1);
        }

        /// <summary>
        /// Logs many objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Warn(params object[] items) {
            MaybeLog(TraceLevel.Warn, items);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void WarnFormat(string fmt, object obj0) {
            MaybeLogFormat(TraceLevel.Warn, fmt, obj0);
        }


        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void WarnFormat(string fmt, object obj0, object obj1) {
            MaybeLogFormat(TraceLevel.Warn, fmt, obj0, obj1);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void WarnFormat(string fmt, params object[] parms) {
            MaybeLogFormat(TraceLevel.Warn, fmt, parms);
        }

        /// <summary>
        /// Logs the entry of a method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder WarnCall() {
            return MaybeLogCall(TraceLevel.Warn, null);
        }

        /// <summary>
        /// Logs the entry of manually specified a method call. Use with "using" so the returned object's Dispose
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
        /// Logs a single string message.
        /// </summary>
        public void Info(string msg) {
            MaybeLog(TraceLevel.Info, msg);
        }

        /// <summary>
        /// Logs one object (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Info(object arg0) {
            MaybeLog(TraceLevel.Info, arg0);
        }

        /// <summary>
        /// Logs two objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Info(object arg0, object arg1) {
            MaybeLog(TraceLevel.Info, arg0, arg1);
        }

        /// <summary>
        /// Logs many objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Info(params object[] items) {
            MaybeLog(TraceLevel.Info, items);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void InfoFormat(string fmt, object obj0) {
            MaybeLogFormat(TraceLevel.Info, fmt, obj0);
        }
        
        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void InfoFormat(string fmt, object obj0, object obj1) {
            MaybeLogFormat(TraceLevel.Info, fmt, obj0, obj1);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void InfoFormat(string fmt, params object[] parms) {
            MaybeLogFormat(TraceLevel.Info, fmt, parms);
        }

        /// <summary>
        /// Logs the entry of a method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder InfoCall() {
            return MaybeLogCall(TraceLevel.Info, null);
        }

        /// <summary>
        /// Logs the entry of a manually specified method call. Use with "using" so the returned object's Dispose
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
        /// Logs a single string message.
        /// </summary>
        public void Debug(string msg) {
            MaybeLog(TraceLevel.Debug, msg);
        }

        /// <summary>
        /// Logs one object (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Debug(object arg0) {
            MaybeLog(TraceLevel.Debug, arg0);
        }

        /// <summary>
        /// Logs two objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Debug(object arg0, object arg1) {
            MaybeLog(TraceLevel.Debug, arg0, arg1);
        }

        /// <summary>
        /// Logs many objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Debug(params object[] items) {
            MaybeLog(TraceLevel.Debug, items);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void DebugFormat(string fmt, object obj0) {
            MaybeLogFormat(TraceLevel.Debug, fmt, obj0);
        }


        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void DebugFormat(string fmt, object obj0, object obj1) {
            MaybeLogFormat(TraceLevel.Debug, fmt, obj0, obj1);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void DebugFormat(string fmt, params object[] parms) {
            MaybeLogFormat(TraceLevel.Debug, fmt, parms);
        }

        /// <summary>
        /// Logs the entry of a method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder DebugCall() {
            return MaybeLogCall(TraceLevel.Debug, null);
        }

        /// <summary>
        /// Logs the entry of a manually specified method call. Use with "using" so the returned object's Dispose
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
        /// Logs a single string message.
        /// </summary>
        public void Verbose(string msg) {
            MaybeLog(TraceLevel.Verbose, msg);
        }

        /// <summary>
        /// Logs one object (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Verbose(object arg0) {
            MaybeLog(TraceLevel.Verbose, arg0);
        }

        /// <summary>
        /// Logs two objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Verbose(object arg0, object arg1) {
            MaybeLog(TraceLevel.Verbose, arg0, arg1);
        }

        /// <summary>
        /// Logs many objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Verbose(params object[] items) {
            MaybeLog(TraceLevel.Verbose, items);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void VerboseFormat(string fmt, object obj0) {
            MaybeLogFormat(TraceLevel.Verbose, fmt, obj0);
        }
        
        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void VerboseFormat(string fmt, object obj0, object obj1) {
            MaybeLogFormat(TraceLevel.Verbose, fmt, obj0, obj1);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void VerboseFormat(string fmt, params object[] parms) {
            MaybeLogFormat(TraceLevel.Verbose, fmt, parms);
        }

        /// <summary>
        /// Logs the entry of a method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder VerboseCall() {
            return MaybeLogCall(TraceLevel.Verbose, null);
        }

        /// <summary>
        /// Logs the entry of a manually specified method call. Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder VerboseCall(string methodName) {
            return MaybeLogCall(TraceLevel.Verbose, methodName);
        }

        #endregion


        /// <summary>
        /// Logs a view lines about the execution environment, such as OS Version,
        /// assembly version/location, user name, and machine name.
        /// </summary>
        public void LogEnvironmentInfo()
        {
            using (InfoCall("EnvironmentInfo"))
            {
                Assembly entryAssembly = Assembly.GetEntryAssembly();

                if (entryAssembly == null)
                {
                    Info("Assembly.GetEntryAssembly() returned null.");
                }
                else
                {
                    Info("EntryAssembly.Location = ", entryAssembly.Location);
                    Info("EntryAssembly.FullName = ", entryAssembly.FullName); // Includes assembly version.

                    try
                    {
                        // Try to get the file version.
                        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(entryAssembly.Location);
                        Info("FileVersionInfo.FileVersion = ", fvi.FileVersion);
                        Info("FileVersionInfo.ProductVersion = ", fvi.ProductVersion);
                    }
                    catch (Exception)
                    {
                    }
                }

                try
                {
                    Info("AppDomain.FriendlyName = ", AppDomain.CurrentDomain.FriendlyName);
                    Info("AppDomain.IsDefaultAppDomain = ", AppDomain.CurrentDomain.IsDefaultAppDomain());
                    Info("AppDomain.BaseDirectory = ", AppDomain.CurrentDomain.BaseDirectory);
                }
                catch (Exception)
                {
                }

                Info("Environment.OSVersion = ", Environment.OSVersion);
                Info("Environment.CurrentDirectory = ", Environment.CurrentDirectory);
                Info("Environment.UserInteractive = ", Environment.UserInteractive);

                Debug("Environment.CommandLine = ", Environment.CommandLine);

                Verbose("Environment.MachineName = ", Environment.MachineName);
                Verbose("Environment.UserDomainName = ", Environment.UserDomainName);
                Verbose("Environment.UserName = ", Environment.UserName);
            }
        }

        #endregion
        #endregion

        #region Private/Internal

        private static string _textForNull = "<null>";

        /// <summary>
        /// Ctor is private.  GetLogger() should be the only caller.
        /// </summary>
        private Logger(string name) {
            Name = name;
            DestinationLevels = new LevelPair[] { BinaryFileLevels, TextFileLevels, ConsoleLevels, DebugOutLevels, EventLogLevels, EventHandlerLevels };
            _loggers.Add(name, this);
        }
        
        // The static ctor.
        static Logger()
        {
            DefaultBinaryFile = new BinaryFile();
            DefaultTextFile = new TextFile();

            Root = new Logger("Root");
            Root.BinaryFileTraceLevel = TraceLevel.Info;
            Root.TextFileTraceLevel = TraceLevel.Off;
            Root.ConsoleTraceLevel = TraceLevel.Off;
            Root.DebugTraceLevel = TraceLevel.Off;
            Root.EventLogTraceLevel = TraceLevel.Off;
            Root.EventHandlerTraceLevel = TraceLevel.Off;

            // The StandardData logger is special in that it is initialized with a BinaryFileTraceLevel
            // of Verbose.  The user can change it programatically or via XML.
            StandardData = GetLogger("StandardData");
            StandardData.BinaryFileTraceLevel = TraceLevel.Verbose;

            AppDomain.CurrentDomain.UnhandledException += _appDomainExceptionHandler;
        }

        // Helper class that allows other AppDomains to call static Logger methods.
        private class AppDomainHelper : MarshalByRefObject
        {
            public Logger GetLogger(string name)
            {
                return Logger.GetLogger(name);
            }
        }

        internal static string ParseFormatString(string input) {
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

        private static string _webAppPath = null;

        // If we are not a web app, this will throw an exception the
        // caller must handle.  
        internal static string GetWebAppDir()
        {
            if (_webAppPath == null)
            {
                // First call to this method.
                // If this doesn't work, we'll leave
                // _webAppPath = "" so we won't
                // try again on future calls.
                _webAppPath = "";

                // We don't want to depend on System.Web or load it,
                // but if System.Web has already been loaded then
                // use System.Web.HttpRuntime.AppDomainAppPath to check if we're a web app
                // and return the app's path. 

                foreach (Assembly webAsm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (webAsm.FullName.StartsWith("System.Web, ")) {
                        Type httpruntime = webAsm.GetType("System.Web.HttpRuntime");
                        PropertyInfo prop = httpruntime.GetProperty("AppDomainAppPath", BindingFlags.Public | BindingFlags.Static);

                        // Exception if not a web app.
                        string s = (string)prop.GetValue(null, null);
                        _webAppPath = s.TrimEnd('\\', '/');

                        return _webAppPath;
                    }
                }

                throw new Exception("System.Web was not already loaded.");
            }
            else if (_webAppPath == "")
            {
                // This means we already experienced an exception in 
                // the above code on the first call, so no need
                // to try again.
                throw new Exception("Not a web app.");
            }
            else
            {
                return _webAppPath;
            }
        }

        // Get the application name without loading System.Windows.Forms.dll
        // or calling Process.GetProcess(), which requires significant permission.
        internal static string GetAppName() {
            string result;

            try {
                result = Assembly.GetEntryAssembly().GetName().Name;
            } catch (Exception) {
                try {
                    // Expect an exception if we're not a web app.
                    result = Path.GetFileNameWithoutExtension(GetWebAppDir());
                } catch (Exception) {
                    try {
                        // This is a good result for winforms, but not very
                        // pretty for web apps.
                        result = AppDomain.CurrentDomain.FriendlyName;
                    } catch (Exception) {
                        // Can't think of anything else to try, but we must
                        // return something and not allow an exception.
                        result = "TracerX_App";
                    }
                }
            }

            return result;
        }

        // This is called just before writing to the binary file. It ensures
        // the BinaryFile property can't change afterward.
        internal void CommitToBinaryFile()
        {
            // Once _binaryFileIsCommitted, it stays true forever.
            if (!_isBinaryFileCommitted)
            {
                lock (_loggerLock)
                {
                    _isBinaryFileCommitted = true;
                }
            }
        }

        // This is called just before writing to the text file. It ensures
        // the TextFile property can't change afterward.
        internal void CommitToTextFile()
        {
            // Once _isTextFileCommitted, it stays true forever.
            if (!_isTextFileCommitted)
            {
                lock (_loggerLock)
                {
                    _isTextFileCommitted = true;
                }
            }
        }

        private volatile BinaryFile _binaryFile = DefaultBinaryFile;
        private volatile bool _isBinaryFileCommitted;

        private volatile TextFile _textFile = DefaultTextFile;
        private volatile bool _isTextFileCommitted;
        private object _loggerLock = new object();

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
        private LevelPair BinaryFileLevels = new LevelPair();
        private LevelPair TextFileLevels = new LevelPair();
        private LevelPair ConsoleLevels = new LevelPair();
        private LevelPair DebugOutLevels = new LevelPair();
        private LevelPair EventLogLevels = new LevelPair();
        private LevelPair EventHandlerLevels = new LevelPair();

        LevelPair[] DestinationLevels;
        const int BinaryFileIndex = 0;
        const int TextFileIndex = 1;
        const int ConsoleIndex = 2;
        const int DebugIndex = 3;
        const int EventLogIndex = 4;
        const int EventHandlerIndex = 5;

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

        // Determines which destinations the message should be logged to and calls
        // the appropriate method for each.
        private void LogToDestinations(ThreadData threadData, TraceLevel msgLevel, string msg) {
            try {
                bool cancelled = false;

                // The EventHandler destination is first so if the handler cancels the event,
                // the message won't be logged to any other destinations.
                if (EventHandlerTraceLevel >= msgLevel) cancelled = EventHandlerLogging.LogMsg(this, threadData, msgLevel, msg, false, false);

                if (!cancelled)
                {
                    if (BinaryFileTraceLevel >= msgLevel)
                    {
                        CommitToBinaryFile();
                        BinaryFile.LogMsg(this, threadData, msgLevel, msg);
                    }

                    if (TextFileTraceLevel >= msgLevel)
                    {
                        CommitToTextFile();
                        TextFile.LogMsg(this, threadData, msgLevel, msg);
                    }
                   
                    if (ConsoleTraceLevel >= msgLevel) ConsoleLogging.LogMsg(this, threadData, msgLevel, msg);
                    if (DebugTraceLevel >= msgLevel) DebugLogging.LogMsg(this, threadData, msgLevel, msg);
                    if (EventLogTraceLevel >= msgLevel) EventLogging.LogMsg(this, threadData, msgLevel, msg);
                }

                threadData.LastLoggerAnyDest = this;
            } catch (Exception) {
                // TODO: What, exactly?
            }
        }

        /// <summary>
        /// Log a single string message.
        /// </summary>
        private void MaybeLog(TraceLevel msgLevel, string msg) {
            if (IsLevelEnabled(msgLevel)) {
                // At least one destination is enabled at this level.
                LogToDestinations(ThreadData.CurrentThreadData, msgLevel, msg ?? _textForNull);
            }
        }

        /// <summary>
        /// Log one object using the RendererMap.
        /// </summary>
        private void MaybeLog(TraceLevel msgLevel, object arg0) {
            if (IsLevelEnabled(msgLevel)) {
                ThreadData threadData = ThreadData.CurrentThreadData;

                RendererMap.FindAndRender(arg0 ?? _textForNull, threadData.StringWriter);
                string msg = threadData.ResetStringWriter();
                LogToDestinations(threadData, msgLevel, msg);
            }
        }

        /// <summary>
        /// Log two objects concatenated together using the RendererMap.
        /// </summary>
        private void MaybeLog(TraceLevel msgLevel, object arg0, object arg1) {
            if (IsLevelEnabled(msgLevel)) {
                ThreadData threadData = ThreadData.CurrentThreadData;

                RendererMap.FindAndRender(arg0 ?? _textForNull, threadData.StringWriter);
                RendererMap.FindAndRender(arg1 ?? _textForNull, threadData.StringWriter);
                string msg = threadData.ResetStringWriter();
                LogToDestinations(threadData, msgLevel, msg);
            }
        }

        /// <summary>
        /// Log many objects concatenated together using the RendererMap.
        /// </summary>
        private void MaybeLog(TraceLevel msgLevel, params object[] items) {
            if (IsLevelEnabled(msgLevel)) {
                ThreadData threadData = ThreadData.CurrentThreadData;

                foreach (object o in items)
                {
                    RendererMap.FindAndRender(o ?? _textForNull, threadData.StringWriter);
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

        // Constructs a flags enum indicating which destinations' levels are enabled.
        internal Destinations GetDestinations(TraceLevel level) {
            Destinations destinations = Destinations.None;

            if (BinaryFileTraceLevel >= level && BinaryFile.IsOpen) destinations |= Destinations.BinaryFile;
            if (TextFileTraceLevel >= level && TextFile.IsOpen) destinations |= Destinations.TextFile;
            if (ConsoleTraceLevel >= level) destinations |= Destinations.Console;
            if (DebugTraceLevel >= level) destinations |= Destinations.Debug;
            if (EventLogTraceLevel >= level) destinations |= Destinations.EventLog;
            if (EventHandlerTraceLevel >= level) destinations |= Destinations.EventHandler;

            return destinations;
        }

        private CallEnder MaybeLogCall(TraceLevel level, string methodName) {
            CallEnder result = null;
            Destinations destinations = GetDestinations(level);

            if (destinations != Destinations.None)
            {
                ThreadData threadData = ThreadData.CurrentThreadData;

                if (methodName == null)
                {
                    methodName = GetCaller();
                }

                if (threadData.LogCallEntry(this, level, methodName, destinations))
                {
                    // MyCallEnder will log the exit when it is disposed.
                    result = MyCallEnder;
                }
            }

            return result;
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

                if (frame == null)
                {
                    // Don't think this ever happens.
                    return "Unknown Method";
                }
                else
                {
                    System.Reflection.MethodBase method = frame.GetMethod();

                    if (method == null)
                    {
                        // This happens when we're called
                        // from another AppDomain.
                        return "Unknown (remote?) Method";
                    }
                    else
                    {
                        Type type = method.DeclaringType;

                        if (type != typeof(TracerX.Logger))
                        {
                            _callerDepth = depth;
                            return type.Name + '.' + method.Name;
                        }
                    }
                }
            }

            return "Unknown Method";
        }

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
            builder.AppendFormat("{0}{1} {2}/{3}, {4}/{5}, {6}/{7}, {8}/{9}, {10}/{11}, {12}/{13} \n",
                new string(' ', indent),     
                Name,
                BinaryFileLevels.SetLevel, BinaryFileLevels.EffectiveLevel,
                TextFileLevels.SetLevel, TextFileLevels.EffectiveLevel,
                ConsoleLevels.SetLevel, ConsoleLevels.EffectiveLevel,
                DebugOutLevels.SetLevel, DebugOutLevels.EffectiveLevel,
                EventLogLevels.SetLevel, EventLogLevels.EffectiveLevel,
                EventHandlerLevels.SetLevel, EventHandlerLevels.EffectiveLevel
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
