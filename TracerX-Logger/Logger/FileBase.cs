using System;
using System.IO;
using System.Reflection;
using System.ComponentModel;

namespace TracerX {
    /// <summary>
    /// This is the base class for <see cref="BinaryFile"/> and <see cref="TextFile"/>.
    /// It can't be used directly, but there doesn't seem to be a way to exclude it from
    /// the help document without also preventing its public members from appearing in
    /// the help for the derived classes.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public abstract class FileBase {
        /// <summary>
        /// Directory of the output log file.  Environment variables are expanded
        /// when this is set.  The following strings (not really environment variables)
        /// are also expanded:
        /// %LOCAL_APPDATA% is expanded to the current user's local (i.e. non-roaming) 
        /// application data directory.
        /// %EXEDIR% is expanded to the directory of the executable.  
        /// Other special variables are %COMMON_APPDATA%, %DESKTOP%, and %MY_DOCUMENTS%.
        /// Attempts to change this property while the log file is open are ignored.
        /// </summary>
        public string Directory {
            get { return _logDirectory; }
            set {
                lock (_fileLocker) {
                    if (!IsOpen) {
                        try {
                            _logDirectory = ExpandDirectoryString(value);
                        } catch (Exception ex) {
                            Logger.EventLogging.Log("An error occurred while replacing environment variables in BinaryFile.Directory value " + value + "\r\n" + ex.Message,  Logger.EventLogging.ExceptionInLogger);
                        }
                    }
                }
            }
        }
        private string _logDirectory = GetDefaultDir();

        /// <summary>
        /// The name of the log file within the Directory.  The default is based on the
        /// running executable or AppDomain name.  The extension is always coerced to ".tx1" for
        /// the binary file and ".txt" for the text file.
        /// Attempts to change this property while the log file is open are ignored.
        /// </summary>
        public string Name {
            get {
                if (Use_00) {
                    return _logFileName + "_00" + _extension;
                } else {
                    return _logFileName + _extension;
                }
            }

            set {
                lock (_fileLocker) {
                    if (!IsOpen) {
                        // The name is stored without extension or number.
                        _logFileName = Path.GetFileNameWithoutExtension(value);
                    }
                }
            }
        }
        protected string _logFileName;

        /// <summary>
        /// If true when the file is opened, "_00" will be appended to the file name.
        /// This allows for more consistent sorting/grouping of file names in
        /// Windows Explorer when archived files (_01, _02, etc.) are present.
        /// Attempts to change this property while the log file is open are ignored.
        /// </summary>
        public bool Use_00 {
            get { return _use_00; }
            set {
                lock (_fileLocker) {
                    if (!IsOpen) {
                        _use_00 = value;
                    }
                }
            }
        }
        private bool _use_00;

        /// <summary>
        /// Gets the full path of the log file based on Directory, Name, and Use_00.
        /// </summary>
        public string FullPath {
            get {
                return Path.Combine(Directory, Name);
            }
        }

        /// <summary>
        /// If true, <see cref="MaxSizeMb"/> and <see cref="AppendIfSmallerThanMb"/> have units of kilobytes (2**10 bytes) instead of megabytes.
        /// </summary>
        public bool UseKbForSize
        {
            get { return _shift == 10; }
            set {
                lock (_fileLocker)
                {
                    if (!IsOpen)
                    {
                        if (value) _shift = 10;
                        else _shift = 20;
                    }
                }
            }
        }
        protected int _shift = 20; // How much to shift to convert Mb or Kb to bytes.

        /// <summary>
        /// Max size of the output log file in megabytes (1 Mb = 2**20 bytes).
        /// If <see cref="UseKbForSize"/> is true, this specifies kilobytes instead of megabytes.
        /// Values over 4095 are coerced to 4095.  The default is 20.
        /// in Append mode (see <see cref="AppendIfSmallerThanMb"/>), this specifies how much the file can grow rather 
        /// than the absolute maximum size.
        /// Attempts to change this property while the log file is open are ignored.
        /// </summary>
        public uint MaxSizeMb {
            get { return _maxSizeMb; }
            set {
                lock (_fileLocker) {
                    if (!IsOpen) {
                        if (value > 4095) value = 4095;
                        _maxSizeMb = value;
                    }
                }
            }
        }
        private uint _maxSizeMb = 20;

        /// <summary>
        /// Specifies what to do when the file reaches maximum size.  Default = Wrap.
        /// Attempts to change this property while the log file is open are ignored.
        /// </summary>
        public FullFilePolicy FullFilePolicy {
            get { return _fullFilePolicy; }
            set
            {
                lock (_fileLocker)
                {
                    if (!IsOpen)
                    {
                        _fullFilePolicy = value;
                    }
                }
            }
        }
        private FullFilePolicy _fullFilePolicy = FullFilePolicy.Wrap;

        /// <summary>
        /// Is the log file open?
        /// </summary>
        public abstract bool IsOpen { get; }

        /// <summary>
        /// If the log file already exists and is smaller than the specified number
        /// of Megabytes, it will be opened in Append mode.  Otherwise, a new file
        /// will be created.  A new file is always created if this is 0 (the default).
        /// If <see cref="UseKbForSize"/> is true, this specifies kilobytes instead of megabytes.
        /// Attempts to change this property while the log file is open are ignored.
        /// </summary>
        public uint AppendIfSmallerThanMb {
            get { return _appendIfSmallerThanMb; }
            set {
                lock (_fileLocker) {
                    if (!IsOpen) {
                        _appendIfSmallerThanMb = value;
                    }
                }
            }
        }
        private uint _appendIfSmallerThanMb = 0;

        /// <summary>
        /// How many backups of the output log file to keep (max of 99, default is 3).
        /// If the output file already exists and isn't opened in Append mode, 
        /// it will become archive _01 (unless Archives == 0) and any existing 
        /// archive files will be renamed with higher numbers. Existing 
        /// archive files with archive numbers greater than the specified
        /// value are always deleted (even in Append mode).  
        /// Attempts to change this property while the log file is open are ignored.
        /// </summary>
        public uint Archives {
            get { return _archives; }
            set {
                lock (_fileLocker) {
                    if (!IsOpen) {
                        // Do not support more than 99 archives.
                        if (value > 99) value = 99;
                        if (value < 0) value = 0;
                        _archives = value;
                    }
                }
            }
        }
        private uint _archives = 3;

        /// <summary>
        /// If FullFilePolicy is Wrap,
        /// circular logging will start when the log file size has increased by
        /// this amount, unless already started.
        /// Set this to 0 to disable this feature.
        /// Attempts to change this value are ignored after circular logging starts.
        /// </summary>
        public uint CircularStartSizeKb {
            get { return _circularStartSizeKb; }
            set {
                lock (_fileLocker) {
                    if (!CircularStarted) {
                        if (value > 4193300) value = 4193300;
                        _circularStartSizeKb = value;
                    }
                }
            }
        }
        private uint _circularStartSizeKb = 300;

        /// <summary>
        /// If FullFilePolicy is Wrap,
        /// circular logging will start when the log file has been opened this many seconds, unless already started.
        /// Set this to 0 to disable this feature.
        /// Attempts to change this value are ignored after circular logging starts.
        /// </summary>
        public uint CircularStartDelaySeconds {
            get { return _circularStartDelaySeconds; }
            set { SetCircularDelay(value, ref _circularStartDelaySeconds); } // Logger provides thread-safety.
        }
        private uint _circularStartDelaySeconds = 60;

        /// <summary>
        /// Has circular logging started (not necessarily wrapped)?
        /// </summary>
        public abstract bool CircularStarted { get; }

        /// <summary>
        /// Returns true if the file size has exceeded the max size.  Once this becomes
        /// true, future output replaces old output if circular logging is enabled.
        /// </summary>
        public abstract bool Wrapped { get; }

        /// <summary>
        /// Counts how many times Open() has been successfully called.
        /// Zero means the file has never been opened.
        /// Non-zero does not mean the file is currently open (use IsOpen for that).
        /// </summary>
        public int CurrentFile { get; protected set; }

        /// <summary>
        /// The current absolute size of the output file.
        /// </summary>
        public uint CurrentSize {
            get {
                lock (_fileLocker) {
                    if (IsOpen) {
                        return (uint)BaseStream.Length;
                    } else {
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// The current absolute file position (used by test drivers).
        /// </summary>
        public uint CurrentPosition {
            get {
                lock (_fileLocker) {
                    if (IsOpen) {
                        return (uint)BaseStream.Position;
                    } else {
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// Event raised just before the file is opened by Open().  
        /// Last chance to set file related properties like Directory, Name, MaxSizeMb, etc..  
        /// The "sender" is either BinaryFileLogging or TextFileLogging, depending on which file is being opened.
        /// </summary>
        public event CancelEventHandler Opening;

        /// <summary>
        /// Event raised after file is opened by Open().
        /// The "sender" is either BinaryFileLogging or TextFileLogging depending on which file was opened.
        /// </summary>
        public event EventHandler Opened;

        /// <summary>
        /// Event raised just before file is closed by Close().
        /// The "sender" is either BinaryFileLogging or TextFileLogging depending on which file is being closed.
        /// </summary>
        public event EventHandler Closing;

        /// <summary>
        /// Event raised just after file is closed by Close().
        /// The "sender" is either BinaryFileLogging or TextFileLogging depending on which file was closed.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// The stream used for writing to the log file.  In derived classes,
        /// this is BinaryWriter.BaseStream or StreamWriter.BaseStream.
        /// </summary>
        protected abstract Stream BaseStream { get; }

        // Object used with the lock keyword to serialize file I/O.
        internal readonly object _fileLocker = new object();

        private string _extension;

        // Count of lines written.
        protected ulong _lineCnt = 0;

        // When the file was opened.
        protected DateTime _openTimeUtc;

        // The first line logged on or after this Time will start the circular log.
        protected DateTime _circularStartTime = DateTime.MaxValue;

        // The size of file when first opened.
        protected long _openSize = 0;

        // Wrap when this file position is reached.
        protected long _maxFilePosition = 0;

        // File position where the circular part of the log physically starts.
        protected long _positionOfCircularPart;

        protected FileBase(string extension) {
            _extension = extension;
            _logFileName = Logger.GetAppName();
        }
      

        /// <summary>
        /// Opens the log file using the configuration specified by various properties.  Raises Opening and Opened events under the internal lock.
        /// </summary>
        public virtual bool Open()
        {
            lock (_fileLocker)
            {
                if (OnOpening())
                {
                    if (IsOpen)
                    {
                        Logger.EventLogging.Log("OpenLog called after log file was already opened.", Logger.EventLogging.FileAlreadyOpen);
                    }
                    else if (MaxSizeMb == 0)
                    {
                        Logger.EventLogging.Log("The log file was not opened because MaxSizeMb was 0 when Open was called.", Logger.EventLogging.MaxSizeZero);
                    }
                    else
                    {
                        try
                        {
                            if (!System.IO.Directory.Exists(Directory))
                            {
                                System.IO.Directory.CreateDirectory(Directory);
                            }

                            InternalOpen();

                            string msg = "The following log file was opened:\n" + FullPath;
                            Logger.EventLogging.Log(msg, Logger.EventLogging.LogFileOpened);
                            OnOpened();
                        }
                        catch (Exception ex)
                        {
                            string msg = string.Format("The following exception occurred attempting to open the log file\n{0}\n\n{1}", FullPath, ex);
                            Logger.EventLogging.Log(msg, Logger.EventLogging.ExceptionInOpen);
                            Close();
                        }
                    }
                }

                return IsOpen;
            }
        }

        /// <summary>
        /// Resets private/protected fields in preparation for reopening.
        /// </summary>
        public virtual void Close()
        {
            _circularStartTime = DateTime.MaxValue;
            _openSize = 0;
            _maxFilePosition = 0;
            _positionOfCircularPart = 0;
            _lineCnt = 0;
        }

        /// <summary>
        /// Closes and re-opens the file under the internal thread-safe lock to ensure no output is lost while the file is closed.
        /// Archived files are shifted (_00 to _01 to _02, etc.) and associated events are raised.
        /// </summary>
        public void CloseAndReopen()
        {
            lock (_fileLocker)
            {
                Close();
                Open();
            }
        }

        // If circular logging hasn't already started, accept the new number of seconds
        // and update the configVal.
        private void SetCircularDelay(uint seconds, ref uint configVal) {
            lock (_fileLocker) {
                if (!CircularStarted) {
                    configVal = seconds;
                    if (IsOpen) {
                        if (seconds == 0) {
                            _circularStartTime = DateTime.MaxValue;
                        } else {
                            _circularStartTime = _openTimeUtc.AddSeconds(seconds);
                        }
                    }
                }
            }
        }

        protected void TryRename(string from, string template, int num) {
            string newFile = string.Format("{0}_{1:D2}{2}", template, num, _extension);

            try {
                File.Move(from, newFile);
            } catch (Exception ex) {
                string msg = string.Format("An exception occurred while renaming the old log file\n{0}\nto\n{1}\n\n{2}", from, newFile, ex);
                 Logger.EventLogging.Log(msg,  Logger.EventLogging.ExceptionInArchive);
            }
        }

        // This returns a list of archived versions (e.g. logFileName_01.tx1) in
        // reverse alphabetical order.
        protected string[] EnumOldFiles(string suffixPattern) {
            string[] files = null;

            try {
                // Get the archived log files in reverse order.
                // There must be two characters after the '_'.
                string wild = _logFileName + suffixPattern;
                files = System.IO.Directory.GetFiles(Directory, wild, SearchOption.TopDirectoryOnly);
                Array.Sort(files, StringComparer.OrdinalIgnoreCase);
                Array.Reverse(files);
            } catch (Exception ex) {
                string msg = string.Format("An exception occurred enumerating old log files in\n{0}\n\n{1}", Directory, ex);
                 Logger.EventLogging.Log(msg,  Logger.EventLogging.ExceptionInArchive);
            }

            return files;
        }

        private static string ExpandDirectoryString(string dir) {
            if (dir.Contains("%LOCAL_APPDATA%")) dir = dir.Replace("%LOCAL_APPDATA%", Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData));
            if (dir.Contains("%LOCALAPPDATA%")) dir = dir.Replace("%LOCALAPPDATA%", Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData));
            if (dir.Contains("%COMMON_APPDATA%")) dir = dir.Replace("%COMMON_APPDATA%", Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData));
            if (dir.Contains("%DESKTOP%")) dir = dir.Replace("%DESKTOP%", Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop));
            if (dir.Contains("%MY_DOCUMENTS%")) dir = dir.Replace("%MY_DOCUMENTS%", Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments));
            if (dir.Contains("%EXEDIR%")) dir = dir.Replace("%EXEDIR%", GetAppDir());
            dir = Environment.ExpandEnvironmentVariables(dir);

            return dir;
        }

        // The default output dir for a winforms app is the local AppData dir.
        // For a web app, it's the dir containing the web site.
        private static string GetDefaultDir() {
            try {
                // This always returns null in web apps, sometimes returns
                // null in the winforms designer.
                if (Assembly.GetEntryAssembly() == null) {
                    // We might be in a web app, but this will throw an
                    // exception if we're not.
                    return Logger.GetWebAppDir();
                } else {
                    // This generally means we're in a winforms app.
                    return Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                }
            } catch (Exception) {
                // Getting here means we're probably not in a web app.
                try {
                    return Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                } catch (Exception) {
                    // Give up. Return something to avoid an exception.
                    return "C:\\";
                }
            }
        }

        // Get the directory the EXE or website is in.
        private static string GetAppDir() {
            try {
                // This throws an exception in web apps and sometimes in
                // the winforms designer.
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            } catch (Exception) {
                try {
                    // This SEEMS to return the correct value for both web apps
                    // and winforms apps.
                    return AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\', '/');
                } catch (Exception) {
                    try {
                        // Expect an exception if we're not a web app.
                        return Logger.GetWebAppDir();
                    } catch (Exception) {
                        try {
                            // Punt.
                            return Environment.CurrentDirectory;
                        } catch (Exception) {
                            // Give up.  Return something to avoid an exception.
                            return "C:\\";
                        }
                    }
                }
            }
        }

        protected abstract void InternalOpen();

        // Raises the Opening event and returns true if not cancelled.
        private bool OnOpening()
        {
            var handlers = Opening;

            if (handlers != null)
            {
                CancelEventArgs args = new CancelEventArgs();
                handlers(this, args);
                return !args.Cancel;
            }
            else
            {
                return true;
            }
        }

        // Raises the Opened event.
        private void OnOpened()
        {
            var handlers = Opened;

            if (handlers != null)
            {
                handlers(this, EventArgs.Empty);
            }
        }

        // Raises the Closing event.
        protected void OnClosing()
        {
            var handlers = Closing;

            if (handlers != null)
            {
                handlers(this, EventArgs.Empty);
            }
        }

        // Raises the Closed event.
        protected void OnClosed()
        {
            var handlers = Closed;

            if (handlers != null)
            {
                handlers(this, EventArgs.Empty);
            }
        }
    }
}