using System;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Runtime.CompilerServices;

namespace TracerX
{
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
    /// to a higher value enables more output (i.e. more detail) to be sent to that destination, assuming there are any messages being logged at
    /// the higher levels.
    /// </para>
    /// <para>
    /// The <see cref="BinaryFileTraceLevel"/> property specifies the maximum level of output sent to the binary log file.
    /// The default value for this property is TraceLevel.Info (inherited from the Root Logger), which means Debug and Verbose
    /// output will be suppressed unless you set this to a higher value.  Each Logger can have it's own binary file, specified via 
    /// the <see cref="Logger.BinaryFile"/> property, or several Loggers can share the same binary file.  By default all Loggers share
    /// to the single binary file represented by the static <see cref="Logger.DefaultBinaryFile"/> property.  Thus, at minimum,
    /// you must call Logger.DefaultBinaryFile.Open() to get any output in the binary file.  You may wish to configure the DefaultBinaryFile
    /// object (e.g. set the Directory and Name properties) before calling Open().
    /// </para>
    /// <para>
    /// Binary files must be viewed with the TracerX-Viewer 
    /// application, which has powerful filtering, navigating, coloring and other features.  The viewer does not work with text files.
    /// </para>
    /// <para>
    /// The <see cref="TextFileTraceLevel"/> property specifies the maximum level of output sent to the text log file. 
    /// The default value for this property is TraceLevel.Off (inherited from the Root Logger).
    /// Each Logger can have it's own text file, specified via 
    /// the <see cref="Logger.TextFile"/> property, or several Loggers can share the same text file.  By default all Loggers share
    /// to the single text file represented by the static <see cref="Logger.DefaultTextFile"/> property.  Thus, at minimum,
    /// you must call Logger.DefaultTextFile.Open() to get any output in the text file.  You may wish to configure the DefaultTextFile
    /// object (e.g. set the Directory and Name properties) before calling Open().
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
    /// By handling this event, you can perform custom processing for each log message such as playing a sound, sending an email, writing to a database, or even cancelling the message.
    /// </para>
    /// <para>
    /// Setting any of the above TraceLevels to TraceLevel.Inherited causes the effective value to be inherited from the parent Logger (possibly
    /// all the way up to Logger.Root).  All these TraceLevel properties are initialized to Inherited in all Loggers you create, meaning their effective values
    /// are inherited from Root.  You can control the logging of whole branches of the hierarchy by setting the TraceLevels in upper-Level 
    /// Loggers like Logger.Root.
    /// </para>
    /// </remarks>    
    public partial class Logger : MarshalByRefObject
    {
        #region Public
        #region Logger hierarchy (GetLogger)
        /// <summary>
        /// Gets the <see cref="Logger"/> instance with the specified name, creating it if necessary.
        /// Loggers created this way have the Inherited <see cref="TraceLevel"/> for all destinations, causing
        /// the effective TraceLevels to be inherited from the parent 
        /// logger (ultimately the <see cref="Root"/> logger).
        /// </summary>
        public static Logger GetLogger(string name)
        {
            Logger logger;
            lock (_loggers)
            {
                if (!_loggers.TryGetValue(name, out logger))
                {
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
        public static Logger GetLogger(Type type)
        {
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
        public static string GetHierarchy()
        {
            lock (_loggers)
            {
                StringBuilder builder = new StringBuilder(500);
                Root.AppendHierarchy(0, builder);
                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets the collection of Logger objects.  Note that iterating over this list is
        /// not thread-safe since any thread can add Loggers at any time.
        /// </summary>
        public static Dictionary<string, Logger>.ValueCollection LoggerList
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
        /// TracerX's best guess at a reasonable name for the calling application/process.
        /// </summary>
        public static readonly string AppName = GetAppName();

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
        public static uint MaxUnhandledExceptionsLogged
        {
            get { return _maxExceptionsLogged; }
            set { _maxExceptionsLogged = value; }
        }

        /// <summary>
        /// Gets and sets the thread name written to the log file for the current thread.  
        /// Setting this effectively overrides (but does not change) Thread.CurrentThread.Name 
        /// within TracerX.  Unlike Thread.CurrentThread.Name, this can be changed many times.  
        /// If set to null (the default value), TracerX uses Thread.CurrentThread.Name.  
        /// </summary>
        public static string ThreadName
        {
            get { return ThreadData.CurrentThreadData.Name; }
            set { ThreadData.CurrentThreadData.Name = value; }
        }

        /// <summary>
        /// Gets the thread number assigned to the calling thread by TracerX.
        /// </summary>
        public static int ThreadNumber
        {
            get { return ThreadData.CurrentThreadData.TracerXID; }
        }

        /// <summary>
        /// Deprecated.  Use <see cref="DefaultBinaryFile"/> instead.
        /// </summary>
        [Obsolete("Use DefaultBinaryFile instead.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public static BinaryFile FileLogging { get { return DefaultBinaryFile; } }

        /// <summary>
        /// Deprecated.  Use <see cref="DefaultBinaryFile"/> instead.
        /// </summary>
        [Obsolete("Use DefaultBinaryFile instead.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public static BinaryFile BinaryFileLogging { get { return DefaultBinaryFile; } }

        /// <summary>
        /// Deprecated.  Use <see cref="DefaultTextFile"/> instead.
        /// </summary>
        [Obsolete("Use DefaultTextFile instead.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public static TextFile TextFileLogging { get { return DefaultTextFile; } }

        #endregion Static properties

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
        public TraceLevel BinaryFileTraceLevel
        {
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

        /// <summary>
        /// Initially false, this becomes true when this Logger first writes to its TextFile.  After that,
        /// TextFile can't be changed.
        /// </summary>
        public bool IsTextFileCommitted
        {
            get { return _isTextFileCommitted; }
        }


        /// <summary>
        /// Similar to <see cref="BinaryFileTraceLevel"/>, but applies to text file output.
        /// See the <see cref="TextFile"/> property.
        /// </summary>
        public TraceLevel TextFileTraceLevel
        {
            get { return TextFileLevels.EffectiveLevel; }
            set { SetTraceLevel(value, TextFileIndex); }
        }

        /// <summary>
        /// Similar to <see cref="BinaryFileTraceLevel"/>, but applies to console output. 
        /// See the <see cref="ConsoleLogging"/> class.
        /// </summary>
        public TraceLevel ConsoleTraceLevel
        {
            get { return ConsoleLevels.EffectiveLevel; }
            set { SetTraceLevel(value, ConsoleIndex); }
        }

        /// <summary>
        /// Similar to <see cref="BinaryFileTraceLevel"/>, but applies to Debug output (i.e. passed to Trace.WriteLine()).
        /// See the <see cref="DebugLogging"/> class.
        /// </summary>
        public TraceLevel DebugTraceLevel
        {
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
        /// Deprecated.  Use <see cref="BinaryFileTraceLevel"/> instead.
        /// </summary>
        [Obsolete("Use BinaryFileTraceLevel instead.", true)]
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
        public void Fatal(string msg)
        {
            AtLevel(TraceLevel.Fatal, msg);
        }

        /// <summary>
        /// Logs many objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Fatal(params object[] items)
        {
            AtLevel(TraceLevel.Fatal, items);
        }

        /// <summary>
        /// Logs one or more lambda expressions as 'ExpressionBody = "value of expression"'.
        /// </summary>
        public void Fatal(params Expression<Func<object>>[] lambdas)
        {
            AtLevel(TraceLevel.Fatal, lambdas);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void FatalFormat(string fmt, params object[] parms)
        {
            AtLevelFormat(TraceLevel.Fatal, fmt, parms);
        }

#if NET35
        /// <summary>
        /// Logs the entry of a method call.  The method name is determined automatically if not specified.
        /// Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder FatalCall(string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Fatal, methodName, threadName: null);
        }

        /// <summary>
        /// Changes the calling thread's name temporarily and logs the entry of the calling method.
        /// The method name may be specified manually or, if null, it is determined automatically.
        /// When the returned object's Dispose() method is called, the calling thread's current name
        /// and method name are restored to their original values.
        /// </summary>
        public CallEnder FatalCallThread(string threadName, string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Fatal, methodName, threadName);
        }
#else
        /// <summary>
        /// Logs the entry of a method call.  The method name is determined automatically if not specified.
        /// Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder FatalCall([CallerMemberName] string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Fatal, methodName, threadName: null);
        }

        /// <summary>
        /// Changes the calling thread's name temporarily and logs the entry of the calling method.
        /// The method name may be specified manually or, if null, it is determined automatically.
        /// When the returned object's Dispose() method is called, the calling thread's current name
        /// and method name are restored to their original values.
        /// </summary>
        public CallEnder FatalCallThread(string threadName, [CallerMemberName] string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Fatal, methodName, threadName);
        }
#endif
        #endregion

        #region Error logging
        /// <summary>
        /// True if Error level logging is enabled for any destination.
        /// </summary>
        public bool IsErrorEnabled { get { return IsLevelEnabled(TraceLevel.Error); } }

        /// <summary>
        /// Logs a single string message.
        /// </summary>
        public void Error(string msg)
        {
            AtLevel(TraceLevel.Error, msg);
        }

        /// <summary>
        /// Logs many objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Error(params object[] items)
        {
            AtLevel(TraceLevel.Error, items);
        }

        /// <summary>
        /// Logs one or more lambda expressions as 'ExpressionBody = "value of expression"'.
        /// </summary>
        public void Error(params Expression<Func<object>>[] lambdas)
        {
            AtLevel(TraceLevel.Error, lambdas);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void ErrorFormat(string fmt, params object[] parms)
        {
            AtLevelFormat(TraceLevel.Error, fmt, parms);
        }

#if NET35
        /// <summary>
        /// Logs the entry of a method call.  The method name is determined automatically if not specified.
        /// Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder ErrorCall(string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Error, methodName, threadName: null);
        }

        /// <summary>
        /// Changes the calling thread's name temporarily and logs the entry of the calling method.
        /// The method name may be specified manually or, if null, it is determined automatically.
        /// When the returned object's Dispose() method is called, the calling thread's current name
        /// and method name are restored to their original values.
        /// </summary>
        public CallEnder ErrorCallThread(string threadName, string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Error, methodName, threadName);
        }
#else
        /// <summary>
        /// Logs the entry of a method call.  The method name is determined automatically if not specified.
        /// Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder ErrorCall([CallerMemberName] string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Error, methodName: methodName, threadName: null);
        }

        /// <summary>
        /// Changes the calling thread's name temporarily and logs the entry of the calling method.
        /// The method name may be specified manually or, if null, it is determined automatically.
        /// When the returned object's Dispose() method is called, the calling thread's current name
        /// and method name are restored to their original values.
        /// </summary>
        public CallEnder ErrorCallThread(string threadName, [CallerMemberName] string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Error, methodName, threadName);
        }
#endif
        #endregion

        #region Warn logging
        /// <summary>
        /// True if Warn level logging is enabled for any destination.
        /// </summary>
        public bool IsWarnEnabled { get { return IsLevelEnabled(TraceLevel.Warn); } }

        /// <summary>
        /// Logs a single string message.
        /// </summary>
        public void Warn(string msg)
        {
            AtLevel(TraceLevel.Warn, msg);
        }

        /// <summary>
        /// Logs many objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Warn(params object[] items)
        {
            AtLevel(TraceLevel.Warn, items);
        }

        /// <summary>
        /// Logs one or more lambda expressions as 'ExpressionBody = "value of expression"'.
        /// </summary>
        public void Warn(params Expression<Func<object>>[] lambdas)
        {
            AtLevel(TraceLevel.Warn, lambdas);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void WarnFormat(string fmt, params object[] parms)
        {
            AtLevelFormat(TraceLevel.Warn, fmt, parms);
        }

#if NET35
        /// <summary>
        /// Logs the entry of a method call.  The method name is determined automatically if not specified.
        /// Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder WarnCall(string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Warn, methodName, threadName: null);
        }

        /// <summary>
        /// Changes the calling thread's name temporarily and logs the entry of the calling method.
        /// The method name may be specified manually or, if null, it is determined automatically.
        /// When the returned object's Dispose() method is called, the calling thread's current name
        /// and method name are restored to their original values.
        /// </summary>
        public CallEnder WarnCallThread(string threadName, string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Warn, methodName, threadName);
        }
#else
        /// <summary>
        /// Logs the entry of a method call.  The method name is determined automatically if not specified.
        /// Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder WarnCall([CallerMemberName] string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Warn, methodName: methodName, threadName: null);
        }

        /// <summary>
        /// Changes the calling thread's name temporarily and logs the entry of the calling method.
        /// The method name may be specified manually or, if null, it is determined automatically.
        /// When the returned object's Dispose() method is called, the calling thread's current name
        /// and method name are restored to their original values.
        /// </summary>
        public CallEnder WarnCallThread(string threadName, [CallerMemberName] string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Warn, methodName, threadName);
        }
#endif
        #endregion

        #region Info logging
        /// <summary>
        /// True if Info level logging is enabled for any destination.
        /// </summary>
        public bool IsInfoEnabled { get { return IsLevelEnabled(TraceLevel.Info); } }

        /// <summary>
        /// Logs a single string message.
        /// </summary>
        public void Info(string msg)
        {
            AtLevel(TraceLevel.Info, msg);
        }

        /// <summary>
        /// Logs many objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Info(params object[] items)
        {
            AtLevel(TraceLevel.Info, items);
        }

        /// <summary>
        /// Logs one or more lambda expressions as 'ExpressionBody = "value of expression"'.
        /// </summary>
        public void Info(params Expression<Func<object>>[] lambdas)
        {
            AtLevel(TraceLevel.Info, lambdas);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void InfoFormat(string fmt, params object[] parms)
        {
            AtLevelFormat(TraceLevel.Info, fmt, parms);
        }

#if NET35
        /// <summary>
        /// Logs the entry of a method call.  The method name is determined automatically if not specified.
        /// Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder InfoCall(string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Info, methodName, threadName: null);
        }

        /// <summary>
        /// Changes the calling thread's name temporarily and logs the entry of the calling method.
        /// The method name may be specified manually or, if null, it is determined automatically.
        /// When the returned object's Dispose() method is called, the calling thread's current name
        /// and method name are restored to their original values.
        /// </summary>
        public CallEnder InfoCallThread(string threadName, string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Info, methodName, threadName);
        }
#else
        /// <summary>
        /// Logs the entry of a method call.  The method name is determined automatically if not specified.
        /// Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder InfoCall([CallerMemberName] string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Info, methodName, threadName: null);
        }

        /// <summary>
        /// Changes the calling thread's name temporarily and logs the entry of the calling method.
        /// The method name may be specified manually or, if null, it is determined automatically.
        /// When the returned object's Dispose() method is called, the calling thread's current name
        /// and method name are restored to their original values.
        /// </summary>
        public CallEnder InfoCallThread(string threadName, [CallerMemberName] string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Info, methodName, threadName);
        }
#endif

        #endregion

        #region Debug logging
        /// <summary>
        /// True if Debug level logging is enabled for any destination.
        /// </summary>
        public bool IsDebugEnabled { get { return IsLevelEnabled(TraceLevel.Debug); } }

        /// <summary>
        /// Logs a single string message.
        /// </summary>
        public void Debug(string msg)
        {
            AtLevel(TraceLevel.Debug, msg);
        }

        /// <summary>
        /// Logs many objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Debug(params object[] items)
        {
            AtLevel(TraceLevel.Debug, items);
        }

        /// <summary>
        /// Logs one or more lambda expressions as 'ExpressionBody = "value of expression"'.
        /// </summary>
        public void Debug(params Expression<Func<object>>[] lambdas)
        {
            AtLevel(TraceLevel.Debug, lambdas);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void DebugFormat(string fmt, params object[] parms)
        {
            AtLevelFormat(TraceLevel.Debug, fmt, parms);
        }

#if NET35
        /// <summary>
        /// Logs the entry of a method call.  The method name is determined automatically if not specified.
        /// Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder DebugCall(string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Debug, methodName, threadName: null);
        }

        /// <summary>
        /// Changes the calling thread's name temporarily and logs the entry of the calling method.
        /// The method name may be specified manually or, if null, it is determined automatically.
        /// When the returned object's Dispose() method is called, the calling thread's current name
        /// and method name are restored to their original values.
        /// </summary>
        public CallEnder DebugCallThread(string threadName, string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Debug, methodName, threadName);
        }
#else
        /// <summary>
        /// Logs the entry of a method call.  The method name is determined automatically if not specified.
        /// Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder DebugCall([CallerMemberName] string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Debug, methodName, threadName: null);
        }

        /// <summary>
        /// Changes the calling thread's name temporarily and logs the entry of the calling method.
        /// The method name may be specified manually or, if null, it is determined automatically.
        /// When the returned object's Dispose() method is called, the calling thread's current name
        /// and method name are restored to their original values.
        /// </summary>
        public CallEnder DebugCallThread(string threadName, [CallerMemberName] string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Debug, methodName, threadName);
        }
#endif
        #endregion

        #region Verbose logging
        /// <summary>
        /// True if Verbose level logging is enabled for any destination.
        /// </summary>
        public bool IsVerboseEnabled { get { return IsLevelEnabled(TraceLevel.Verbose); } }

        /// <summary>
        /// Logs a single string message.
        /// </summary>
        public void Verbose(string msg)
        {
            AtLevel(TraceLevel.Verbose, msg);
        }

        /// <summary>
        /// Logs many objects concatenated together (rendered by calling ToString() unless the object's type is in the RendererMap).
        /// </summary>
        public void Verbose(params object[] items)
        {
            AtLevel(TraceLevel.Verbose, items);
        }

        /// <summary>
        /// Logs one or more lambda expressions as 'ExpressionBody = "value of expression"'.
        /// </summary>
        public void Verbose(params Expression<Func<object>>[] lambdas)
        {
            AtLevel(TraceLevel.Verbose, lambdas);
        }

        /// <summary>
        /// Logs a message with the semantics of string.Format.
        /// </summary>
        public void VerboseFormat(string fmt, params object[] parms)
        {
            AtLevelFormat(TraceLevel.Verbose, fmt, parms);
        }

#if NET35
        /// <summary>
        /// Logs the entry of a method call.  The method name is determined automatically if not specified.
        /// Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder VerboseCall(string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Verbose, methodName, threadName: null);
        }

        /// <summary>
        /// Changes the calling thread's name temporarily and logs the entry of the calling method.
        /// The method name may be specified manually or, if null, it is determined automatically.
        /// When the returned object's Dispose() method is called, the calling thread's current name
        /// and method name are restored to their original values.
        /// </summary>
        public CallEnder VerboseCallThread(string threadName, string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Verbose, methodName, threadName);
        }
#else
        /// <summary>
        /// Logs the entry of a method call.  The method name is determined automatically if not specified.
        /// Use with "using" so the returned object's Dispose
        /// method, which logs the method's exit, will be called automatically.
        /// </summary>
        public CallEnder VerboseCall([CallerMemberName] string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Verbose, methodName, threadName: null);
        }

        /// <summary>
        /// Changes the calling thread's name temporarily and logs the entry of the calling method.
        /// The method name may be specified manually or, if null, it is determined automatically.
        /// When the returned object's Dispose() method is called, the calling thread's current name
        /// and method name are restored to their original values.
        /// </summary>
        public CallEnder VerboseCallThread(string threadName, [CallerMemberName] string methodName = null)
        {
            return MaybeLogCall(TraceLevel.Verbose, methodName, threadName);
        }
#endif
        #endregion

        /// <summary>
        /// Log a single string message.
        /// </summary>
        private void AtLevel(TraceLevel msgLevel, string msg)
        {
            if (IsLevelEnabled(msgLevel))
            {
                // At least one destination is enabled at this level.
                LogToDestinations(ThreadData.CurrentThreadData, msgLevel, msg ?? _textForNull);
            }
        }

        /// <summary>
        /// Log many objects concatenated together.
        /// </summary>
        private void AtLevel(TraceLevel msgLevel, params object[] items)
        {
            if (IsLevelEnabled(msgLevel))
            {
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
        /// Log many objects concatenated together.
        /// </summary>
        private void AtLevel(TraceLevel msgLevel, params Expression<Func<object>>[] lambdas)
        {
            if (IsLevelEnabled(msgLevel))
            {
                ThreadData threadData = ThreadData.CurrentThreadData;

                RendererMap.FindAndRender(lambdas, threadData.StringWriter);
                string msg = threadData.ResetStringWriter();
                LogToDestinations(threadData, msgLevel, msg);
            }
        }

        /// <summary>
        /// Log a message with the semantics of string.Format.
        /// This does NOT use the RendererMap
        /// </summary>
        private void AtLevelFormat(TraceLevel msgLevel, string fmt, params object[] parms)
        {
            if (IsLevelEnabled(msgLevel))
            {
                LogToDestinations(ThreadData.CurrentThreadData, msgLevel, string.Format(fmt, parms));
            }
        }

        /// <summary>
        /// Use with "using".  Changes the calling thread's name to the specified name and
        /// changes it back to the original value when the returned object's Dispose()
        /// method is called.
        /// </summary>
        public CallEnder ThreadNameForCall(string threadName)
        {
            return MaybeLogCall(TraceLevel.Off, null, threadName);
        }

        /// <summary>
        /// Logs information about the execution environment, such as OS Version,
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

        /// <summary>
        /// Grants authenticated users and/or a specific list of users and groups Read access to the specified directory (usually the log file directory) and subdirectories.
        /// </summary>
        /// <param name="directoryPath">The directory to which access is granted</param>
        /// <param name="authenticatedUsers">If true access is granted to all authenticated users.</param>
        /// <param name="specificUsers">Optional list of users and groups to whom access is granted.</param>
        public static void GrantReadAccess(string directoryPath, bool authenticatedUsers, params string[] specificUsers)
        {
            using (Root.DebugCall("Logger.GrantReadAccess"))
            {
                GrantAccess(directoryPath, false, authenticatedUsers, specificUsers);
            }
        }

        /// <summary>
        /// Grants authenticated users and/or a specific list of users and groups Full Control access to the specified directory (usually the log file directory) and subdirectories.
        /// </summary>
        /// <param name="directoryPath">The directory to which access is granted</param>
        /// <param name="authenticatedUsers">If true access is granted to all authenticated users.</param>
        /// <param name="specificUsers">Optional list of users and groups to whom access is granted.</param>
        public static void GrantFullControl(string directoryPath, bool authenticatedUsers, params string[] specificUsers)
        {
            using (Root.DebugCall("Logger.GrantFullControl"))
            {
                GrantAccess(directoryPath, true, authenticatedUsers, specificUsers);
            }
        }

#endregion

#region Private/Internal

        private static string _textForNull = "<null>";

        /// <summary>
        /// Ctor is private.  GetLogger() should be the only caller.
        /// </summary>
        private Logger(string name)
        {
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

        /// <summary>
        /// Grants either Read or FullControl access to the specified directory (usually the log file directory) and subdirectories.
        /// Access is granted to all authenticated users and/or a specific list of users and groups.
        /// </summary>
        /// <param name="directoryPath">The directory to which access is granted</param>
        /// <param name="fullControl">If true "Full Control" is granted.  Otherwise Read access is granted.</param>
        /// <param name="authenticatedUsers">If true access is granted to all authenticated users.</param>
        /// <param name="specificUsers">Optional list of users and groups to whom access is granted.</param>
        private static void GrantAccess(string directoryPath, bool fullControl, bool authenticatedUsers, params string[] specificUsers)
        {
            try
            {
                if (fullControl)
                {
                    Root.Debug("Granting FullControl on path ", directoryPath);
                }
                else
                {
                    Root.Debug("Granting Read access on path ", directoryPath);
                }

#if NET35 || NET45
                DirectorySecurity security = Directory.GetAccessControl(directoryPath);
#elif NETCOREAPP3_1
                DirectoryInfo di = new DirectoryInfo(directoryPath);
                DirectorySecurity security = di.GetAccessControl();
#endif
                FileSystemRights rights = fullControl ? FileSystemRights.FullControl : FileSystemRights.ListDirectory | FileSystemRights.Read;
                bool changed = false;

                if (authenticatedUsers)
                {
                    try
                    {
                        // Grant rights to all authenticated users.

                        Root.Debug("Granting ", rights, " rights to authenticated users.");
                        var authenticatedUsersId = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);

                        security.AddAccessRule(new FileSystemAccessRule(
                            authenticatedUsersId,
                            rights,
                            InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                            PropagationFlags.None,
                            AccessControlType.Allow));

                        changed = true;
                    }

                    catch (Exception ex)
                    {
                        Root.WarnFormat("Failed to grant {0} permission to authenticated users: {1}", rights, ex.Message);
                    }
                }

                if (specificUsers != null)
                {
                    foreach (string account in specificUsers)
                    {
                        try
                        {
                            Root.Debug("Granting ", rights, " to: ", account);

                            security.AddAccessRule(new FileSystemAccessRule(
                                account,
                                rights,
                                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                                PropagationFlags.None,
                                AccessControlType.Allow));

                            changed = true;
                        }

                        catch (Exception ex)
                        {
                            Root.WarnFormat("Failed to grant {0} permission to '{1}': {2}", rights, account, ex.Message);
                        }
                    }
                }

                if (changed)
                {
                    Root.Debug("Calling SetAccessControl()");
#if NET35 || NET45
                    Directory.SetAccessControl(directoryPath, security);
#elif NETCOREAPP3_1
                    di.SetAccessControl(security);
#endif
                }
            }

            catch (Exception ex)
            {
                Root.WarnFormat("Failed to set access control on '{0}': {1}", directoryPath, ex.Message);
            }
        }

        internal static string ParseFormatString(string input)
        {
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
                    if (webAsm.FullName.StartsWith("System.Web, "))
                    {
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
        internal static string GetAppName()
        {
            string result;

            try
            {
                // Throws an exception if we are hosted by unmanaged code (e.g. IIS).
                result = Assembly.GetEntryAssembly().GetName().Name;
            }
            catch (Exception)
            {
                try
                {
                    // Expect an exception if we're not a web app.
                    result = Path.GetFileNameWithoutExtension(GetWebAppDir());
                }
                catch (Exception)
                {
                    try
                    {
                        // This is a good result for winforms, but not very
                        // pretty for web apps.
                        result = AppDomain.CurrentDomain.FriendlyName;
                    }
                    catch (Exception)
                    {
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
                // The lock serves as a memory barrier, guaranteeing all threads see the change immediately.
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
                // The lock serves as a memory barrier, guaranteeing all threads see the change immediately.
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
        private class LevelPair
        {
            public TraceLevel SetLevel = TraceLevel.Inherited;       // The explicitly set trace Level.
            public TraceLevel EffectiveLevel = TraceLevel.Inherited; // Possibly inherited trace Level.

            public bool MaybeInherit(LevelPair parentLevels)
            {
                if (SetLevel == TraceLevel.Inherited)
                {
                    EffectiveLevel = parentLevels.EffectiveLevel;
                    return true;
                }
                else
                {
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

        private void SetTraceLevel(TraceLevel value, int index)
        {
            // Lock _loggers to protect the collection of loggers,
            // the hierarchy of loggers, and the inherited trace levels.
            lock (_loggers)
            {
                if (DestinationLevels[index].SetLevel != value)
                {
                    if (this == Root && value == TraceLevel.Inherited)
                    {
                        // The Root logger is not allowed to have the Inherited trace level
                        return;
                    }

                    DestinationLevels[index].SetLevel = value;

                    if (value == TraceLevel.Inherited && parent != null)
                    {
                        DestinationLevels[index].EffectiveLevel = parent.DestinationLevels[index].EffectiveLevel;
                    }
                    else
                    {
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
        private bool IsLevelEnabled(TraceLevel msgLevel)
        {
            return this._maxLevel >= msgLevel;
        }
#endregion

#region Message logging

        // Determines which destinations the message should be logged to and calls
        // the appropriate method for each.
        private void LogToDestinations(ThreadData threadData, TraceLevel msgLevel, string msg)
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                EventLogging.Log("Exception in LogToDestinations: " + ex.ToString(), EventLogging.ExceptionInLogger);
            }
        }

#endregion

#region Method entry/exit logging

        // Constructs a flags enum indicating which destinations' levels are enabled.
        internal Destinations GetDestinations(TraceLevel level)
        {
            Destinations destinations = Destinations.None;

            if (BinaryFileTraceLevel >= level && BinaryFile.IsOpen) destinations |= Destinations.BinaryFile;
            if (TextFileTraceLevel >= level && TextFile.IsOpen) destinations |= Destinations.TextFile;
            if (ConsoleTraceLevel >= level) destinations |= Destinations.Console;
            if (DebugTraceLevel >= level) destinations |= Destinations.Debug;
            if (EventLogTraceLevel >= level) destinations |= Destinations.EventLog;
            if (EventHandlerTraceLevel >= level) destinations |= Destinations.EventHandler;

            return destinations;
        }

        // Changes the current method name and/or thread name of the calling thread.
        // The method name is only changed if at least one destination is enabled at
        // the specified TraceLevel.  The thread name is only changed if the threadName
        // parameter isn't null.  If either change occurs, the change must be reversed
        // by eventually calling the Dispose() method of the returned CallEnder object.
        // This returns null if neither change occurs because there will be nothing to undo.
        private CallEnder MaybeLogCall(TraceLevel level, string methodName, string threadName)
        {
            CallEnder result = null;
            Destinations destinations = level == TraceLevel.Off ? Destinations.None : GetDestinations(level);

            if (destinations != Destinations.None || threadName != null)
            {
                ThreadData threadData = ThreadData.CurrentThreadData;

                if (methodName == null && destinations != Destinations.None)
                {
                    methodName = GetCaller();
                }

                if (threadData.LogCallEntry(this, level, methodName, destinations, threadName))
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
        private static string GetCaller()
        {
            for (int depth = _callerDepth; depth < 4; ++depth)
            {
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
        internal void AddLogger(Logger logger)
        {
            AddLoggerPrivate(logger);

            for (int i = 0; i < DestinationLevels.Length; ++i)
            {
                // Either the new logger inherits from the its parent, or its children need to
                // inherit from it.
                if (logger.DestinationLevels[i].MaybeInherit(logger.parent.DestinationLevels[i]))
                {
                    logger.ComputeMaxLevel();
                }
                else
                {
                    // The new logger may have been inserted as the parent of some
                    // existing loggers. Any such children may need to inherit the 
                    // new logger's trace Level.
                    logger.SetInheritedTraceLevels(i);
                }
            }
        }

        private void AddLoggerPrivate(Logger logger)
        {
            // See if the new logger should be a child of any of our children.
            // If not, it will become our child.
            string newNameWithDot = logger.Name + ".";
            bool movedChildren = false;

            if (_children == null)
            {
                _children = new List<Logger>();
            }
            else
            {
                foreach (Logger child in _children)
                {
                    if (child.Name.StartsWith(newNameWithDot))
                    {
                        // The new logger should be a parent of the current child
                        // and should replace the current child in the list.
                        // Note that there may be several such children.  E.g.,
                        // we have children A.B.1 and A.B.2, and the new logger is A.B or A.
                        // Therefore, keep looping.
                        logger.AddLoggerPrivate(child);
                        movedChildren = true;
                    }
                    else if (!movedChildren)
                    {
                        string childNameWithDot = child.Name + ".";
                        if (logger.Name.StartsWith(childNameWithDot))
                        {
                            // The new logger should be a child of the current child.
                            child.AddLoggerPrivate(logger);
                            return;
                        }
                    }
                }

                // Getting here means the new logger will become our child.

                if (movedChildren)
                {
                    // Some of our children are now the new logger's children.
                    // Replace them with the new logger.
                    foreach (Logger child in logger._children)
                    {
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
        private void SetInheritedTraceLevels(int destinationIndex)
        {
            if (_children != null)
            {
                foreach (Logger child in _children)
                {
                    if (child.DestinationLevels[destinationIndex].MaybeInherit(this.DestinationLevels[destinationIndex]))
                    {
                        child.ComputeMaxLevel();
                        child.SetInheritedTraceLevels(destinationIndex);
                    }
                }
            }
        }

        private void ComputeMaxLevel()
        {
            _maxLevel = TraceLevel.Off;
            foreach (LevelPair pair in DestinationLevels)
            {
                if (pair.EffectiveLevel > _maxLevel) _maxLevel = pair.EffectiveLevel;
            }
        }

        // Append our information and our children's information to the builder.
        private void AppendHierarchy(int indent, StringBuilder builder)
        {
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
            if (_children != null)
            {
                foreach (Logger child in _children)
                {
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
            delegate(object sender, UnhandledExceptionEventArgs e)
            {
                if (_exceptionsLogged < MaxUnhandledExceptionsLogged)
                {
                    ++_exceptionsLogged;
                    EventLogging.Log("An unhandled exception was passed to TracerX's handler for the AppDomain.CurrentDomain.UnhandledException event.\n\n" + e.ExceptionObject.ToString(), EventLogging.UnhandledExceptionInApp);
                }
                else
                {
                    Logger.Root.Fatal("An unhandled exception was passed to TracerX's handler for the AppDomain.CurrentDomain.UnhandledException event, but was not logged to the event log.\n", e.ExceptionObject);
                }
            };
#endregion

#endregion
    }
}
