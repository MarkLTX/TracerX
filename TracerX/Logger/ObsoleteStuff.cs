using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace BBS.TracerX {
    public partial class Logger {
        [Obsolete("Use FileTraceLevel instead.")]
        public TraceLevel MaxTraceLevel {
            get { return FileTraceLevel; }
            set { FileTraceLevel = value; }
        }

        [Obsolete("No longer supported!")]
        public static TraceLevel ThreadTraceLevel { get { return TraceLevel.Undefined; } }

        [Obsolete("Use Logger.FileLogging.Open instead.")]
        public static bool OpenLog() { return FileLogging.Open(); }

        [Obsolete("Use Logger.FileLogging.Close instead.")]
        public static void CloseLog() { FileLogging.Close(); }

        [Obsolete("Use Logger.FileLogging.CurrentSize instead.")]
        public static uint CurrentFileSize { get { return FileLogging.CurrentSize; } }

        [Obsolete("Use Logger.FileLogging.CircularStarted instead.")]
        public static bool CircularStarted { get { return FileLogging.CircularStarted; } }

        [Obsolete("Use Logger.FileLogging.Wrapped instead.")]
        public static bool Wrapped { get { return FileLogging.Wrapped; } }
    }

    [Obsolete("Use Logger.Xml instead.")]
    public static class XmlConfig {
        [Obsolete("Use Logger.Xml.Configure instead.")]
        static public bool Configure() { return Logger.Xml.Configure(); }

        [Obsolete("Use Logger.Xml.Configure instead.")]
        static public void Configure(FileInfo configFile) { Logger.Xml.Configure(configFile); }

        [Obsolete("Use Logger.Xml.Configure instead.")]
        static public bool Configure(Stream configStream) {return Logger.Xml.Configure(configStream); }

        [Obsolete("Use Logger.Xml.ConfigureFromXml instead.")]
        static public bool ConfigureFromXml(XmlElement element) { return Logger.Xml.ConfigureFromXml(element); }

        [Obsolete("Use Logger.Xml.ConfigureAndWatch instead.")]
        static public void ConfigureAndWatch(FileInfo configFile) { Logger.Xml.ConfigureAndWatch(configFile); }
    }

    public static class Configuration {
        [Obsolete("Use Logger.FileLogging.Directory instead.")]
        public static string LogDirectory {
            get { return Logger.FileLogging.Directory; }
            set { Logger.FileLogging.Directory = value; }
        }

        [Obsolete("Use Logger.FileLogging.Name instead.")]
        public static string LogFileName {
            get { return Logger.FileLogging.Name; }
            set { Logger.FileLogging.Name = value; }
        }

        [Obsolete("Use Logger.FileLogging.FullPath instead.")]
        public static string LogFilePath {
            get { return Logger.FileLogging.FullPath; }
        }

        [Obsolete("Use Logger.FileLogging.MaxSizeMb instead.")]
        public static uint MaxSizeMb {
            get { return Logger.FileLogging.MaxSizeMb; }
            set { Logger.FileLogging.MaxSizeMb = value; }
        }

        [Obsolete("Use Logger.FileLogging.Archives instead.")]
        public static uint Archives {
            get { return Logger.FileLogging.Archives; }
            set { Logger.FileLogging.Archives = value; }
        }

        [Obsolete("Use Logger.FileLogging.CircularStartSizeKb instead.")]
        public static uint CircularStartSizeKb {
            get { return Logger.FileLogging.CircularStartSizeKb; }
            set { Logger.FileLogging.CircularStartSizeKb = value; }
        }

        [Obsolete("Use Logger.FileLogging.CircularStartDelaySeconds instead.")]
        public static uint CircularStartDelaySeconds {
            get { return Logger.FileLogging.CircularStartDelaySeconds; }
            set { Logger.FileLogging.CircularStartDelaySeconds = value; }
        }

        [Obsolete("Configure the logger named 'StandardData' to control the startup logging.")]
        public static bool LogPotentiallySensitiveDataAtStartup {
            get { return true; }
            set { bool dummy = value; }
        }

        [Obsolete("Use Logger.EventLogging.MaxInternalEventNumber instead.")]
        public static uint MaxEventNumber {
            get { return Logger.EventLogging.MaxInternalEventNumber; }
            set { Logger.EventLogging.MaxInternalEventNumber = value; }
        }

        [Obsolete("Use Logger.Root.MaxConsoleLevel instead.")]
        public static TraceLevel ConsoleTraceLevel {
            get { return Logger.Root.ConsoleTraceLevel; }
            set { Logger.Root.ConsoleTraceLevel = value; }
        }

        [Obsolete("Use Logger.ConsoleLogging.FormatString instead.")]
        public static string ConsoleFormatString {
            get { return Logger.ConsoleLogging.FormatString; }
            set { Logger.ConsoleLogging.FormatString = value; }
        }

        [Obsolete("Use Logger.MaxUnhandledExceptionsLogged instead.")]
        public static uint MaxUnhandledExceptionsLogged {
            get { return Logger.MaxUnhandledExceptionsLogged; }
            set { Logger.MaxUnhandledExceptionsLogged = value; }
        }

    }

}
