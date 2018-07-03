using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;

namespace TracerX
{
    /// <summary>
    /// This is the base class for <see cref="BinaryFile"/> and <see cref="TextFile"/>.
    /// It can't be used directly, but there doesn't seem to be a way to exclude it from
    /// the help document without also preventing its public members from appearing in
    /// the help for the derived classes.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public abstract class FileBase
    {
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
        public string Directory
        {
            get { return _logDirectory; }
            set
            {
                lock (_fileLocker)
                {
                    if (!IsOpen)
                    {
                        try
                        {
                            _logDirectory = ExpandDirectoryString(value);
                        }
                        catch (Exception ex)
                        {
                            Logger.EventLogging.Log("An error occurred while replacing environment variables in BinaryFile.Directory value " + value + "\r\n" + ex.Message, Logger.EventLogging.ExceptionInLogger);
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
        public string Name
        {
            get
            {
                if (Use_00)
                {
                    if (Archives < 10)
                    {
                        return _logFileName + "_0" + _extension;
                    }
                    else if (Archives < 100)
                    {
                        return _logFileName + "_00" + _extension;
                    }
                    else if (Archives < 1000)
                    {
                        return _logFileName + "_000" + _extension;
                    }
                    else
                    {
                        // Use 4 digits since 9999 is the max value of Archives.
                        return _logFileName + "_0000" + _extension;
                    }
                }
                else
                {
                    return _logFileName + _extension;
                }
            }

            set
            {
                lock (_fileLocker)
                {
                    if (!IsOpen)
                    {
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
        /// Windows Explorer when archived files (_01, _02, etc.) are also present.
        /// Default = false.
        /// Attempts to change this property while the log file is open are ignored.
        /// </summary>
        public bool Use_00
        {
            get { return _use_00; }
            set
            {
                lock (_fileLocker)
                {
                    if (!IsOpen)
                    {
                        _use_00 = value;
                    }
                }
            }
        }
        private bool _use_00;

        /// <summary>
        /// Gets the full path of the log file based on Directory, Name, and Use_00.
        /// </summary>
        public string FullPath
        {
            get
            {
                return Path.Combine(Directory, Name);
            }
        }

        /// <summary>
        /// If true, <see cref="MaxSizeMb"/> and <see cref="AppendIfSmallerThanMb"/> have units of kilobytes (2**10 bytes) instead of megabytes.
        /// Default = false.
        /// </summary>
        public bool UseKbForSize
        {
            get { return _shift == 10; }
            set
            {
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
        /// Attempts to change this property while the log file is open are ignored.
        /// </summary>
        public uint MaxSizeMb
        {
            get { return _maxSizeMb; }
            set
            {
                lock (_fileLocker)
                {
                    if (!IsOpen)
                    {
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
        public FullFilePolicy FullFilePolicy
        {
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
        /// If the log file already exists and is smaller than both AppendIfSmallerThanMb
        /// and <see cref="MaxSizeMb"/>, it will be opened in Append mode.  Otherwise, a new file
        /// will be created.  A new file is always created if this is 0 (the default).
        /// If <see cref="UseKbForSize"/> is true, this specifies Kilobytes. Otherwise, it
        /// specifies Megabytes. Attempts to change this property while the log file is open are ignored.
        /// </summary>
        public uint AppendIfSmallerThanMb
        {
            get { return _appendIfSmallerThanMb; }
            set
            {
                lock (_fileLocker)
                {
                    if (!IsOpen)
                    {
                        _appendIfSmallerThanMb = value;
                    }
                }
            }
        }
        private uint _appendIfSmallerThanMb = 0;

        /// <summary>
        /// How many backups of the output log file to keep (max of 9999, default is 3).
        /// If the output file already exists and isn't opened in Append mode, 
        /// it will become archive _01 (unless Archives == 0) and any existing 
        /// archive files will be renamed with higher numbers. Existing 
        /// archive files with archive numbers greater than the specified
        /// value are always deleted (even in Append mode).  
        /// Attempts to change this property while the log file is open are ignored.
        /// </summary>
        public uint Archives
        {
            get { return _archives; }
            set
            {
                lock (_fileLocker)
                {
                    if (!IsOpen)
                    {
                        // Do not support more than 9999 archives.
                        if (value > 9999) value = 9999;
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
        /// this amount, unless already started.  Default = 300 KB.
        /// Set this to 0 to disable this feature.
        /// Attempts to change this value are ignored after circular logging starts.
        /// </summary>
        public uint CircularStartSizeKb
        {
            get { return _circularStartSizeKb; }
            set
            {
                lock (_fileLocker)
                {
                    if (!CircularStarted)
                    {
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
        /// The default is 60.
        /// Set this to 0 to disable this feature.
        /// Attempts to change this value are ignored after circular logging starts.
        /// </summary>
        public uint CircularStartDelaySeconds
        {
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
        /// Non-zero does NOT mean the file is currently open (use IsOpen for that).
        /// </summary>
        public int CurrentFile { get; protected set; }

        /// <summary>
        /// The current absolute size of the output file.
        /// </summary>
        public uint CurrentSize
        {
            get
            {
                lock (_fileLocker)
                {
                    if (IsOpen)
                    {
                        return (uint)BaseStream.Length;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// The current absolute file position (used by test drivers).
        /// </summary>
        public uint CurrentPosition
        {
            get
            {
                lock (_fileLocker)
                {
                    if (IsOpen)
                    {
                        return (uint)BaseStream.Position;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// Event raised just before the file is opened by Open().  
        /// Last chance to set file related properties like Directory, Name, MaxSizeMb, etc..  
        /// The "sender" is an instance of BinaryFile or TextFile, depending on which file is being opened.
        /// </summary>
        public event CancelEventHandler Opening;

        /// <summary>
        /// Event raised after file is opened by Open().
        /// The "sender" is an instance of BinaryFile or TextFile depending on which file was opened.
        /// </summary>
        public event EventHandler Opened;

        /// <summary>
        /// Event raised just before file is closed by Close().
        /// The "sender" is an instance of BinaryFile or TextFile depending on which file is being closed.
        /// </summary>
        public event EventHandler Closing;

        /// <summary>
        /// Event raised just after file is closed by Close().
        /// The "sender" is an instance of BinaryFile or TextFile depending on which file was closed.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// The stream used for writing to the log file.  In derived classes,
        /// this is BinaryWriter.BaseStream or StreamWriter.BaseStream.
        /// </summary>
        protected abstract Stream BaseStream { get; }

        // Object used with the lock keyword to serialize file I/O.
        internal readonly object _fileLocker = new object();

        protected readonly string _extension;

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

        protected FileBase(string extension)
        {
            _extension = extension;
            _logFileName = Logger.AppName;
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
                        Logger.EventLogging.Log("Open() called after log file was already opened.", Logger.EventLogging.FileAlreadyOpen);
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
                            string msg = string.Format("The following exception occurred attempting to open the log file\n\"{0}\"\n{1}", FullPath, ex);
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
        private void SetCircularDelay(uint seconds, ref uint configVal)
        {
            lock (_fileLocker)
            {
                if (!CircularStarted)
                {
                    configVal = seconds;
                    if (IsOpen)
                    {
                        if (seconds == 0)
                        {
                            _circularStartTime = DateTime.MaxValue;
                        }
                        else
                        {
                            _circularStartTime = _openTimeUtc.AddSeconds(seconds);
                        }
                    }
                }
            }
        }

        // This "rolls" the archive files (*_01, *_02, etc.).
        // Parameter renamedOutFile is what the old output file was renamed 
        // to if it existed (has extension ".tempname_1" or ".tempname_2", etc.).
        // It was the x_00.tx1 file and must become the x_01.tx1 file.
        // if renamedOutFile is null, the old output file wasn't replaced (didn't
        // exist or was opened in append mode), and no rolling is necessary,
        // but we still delete files with numbers larger than Archives.
        protected void RollArchives(string renamedOutFile)
        {
            // Get the existing archive files for the current _logFileName.  
            // Their file names have the format <logFileName>_<numString><_extension>
            // where <numString> is a number possibly padded with zeros and <_extension> includes a leading '.'. 

            string filespec = _logFileName + "_*" + _extension;
            string[] files = System.IO.Directory.GetFiles(Directory, filespec, SearchOption.TopDirectoryOnly);
            List<KeyValuePair<int, string>> numberedFiles = new List<KeyValuePair<int, string>>();
            string currentlyOpenedFile = this.FullPath;

            // Consider only the files with valid archive numbers and put them in numberedFiles to be sorted, then
            // renamed or deleted. Note that we don't assume unique file numbers or a consistent number of digits (e.g.
            // there might be an _1 and _01 file) because files might get renamed, deleted, copied, etc. outside of our control.

            foreach (string curFile in files)
            {
                // Skip the file we just opened because we don't want to rename or delete it (and can't anyway because it's open).

                if (curFile != currentlyOpenedFile)
                {
                    // Note that curFile should have the format <logFileName>_<numString><_extension>. Strip off the
                    // "<_extension>" and "<logFileName>_" to get "<numString>" and confirm "<numString>" is a valid
                    // archive number (i.e. an integer >= 0).

                    string namePart = Path.GetFileNameWithoutExtension(curFile);
                    string numPart = namePart.Substring(_logFileName.Length + 1);
                    int num;

                    if (int.TryParse(numPart, out num) && num >= 0)
                    {
                        numberedFiles.Add(new KeyValuePair<int, string>(num, curFile));
                    }
                }
            }

            // We now have numberedFiles.Count files plus the renamedOutFile, if given, to renumber from 1 to N. Start with
            // the highest number first so that file can be deleted or renamed to make a spot for the next lower number,
            // and so on. We should end up with sequentially numbered files even if there are gaps in the existing file names.

            int newNum = (renamedOutFile == null) ? numberedFiles.Count : numberedFiles.Count + 1;
            string bareFilePath = Path.Combine(Directory, _logFileName);

            // Sort the files to process them in order of highest number to lowest number so each numbered file is
            // renamed to the next highest number (assuming they were named with sequential numbers to begin with).
            foreach (string oldFile in numberedFiles.OrderByDescending(f => f.Key).Select(f => f.Value))
            {
                if (newNum > Archives)
                {
                    // The file's new archive number is more than the user wants to keep, so delete it.

                    try
                    {
                        File.Delete(oldFile); // Sometimes throws inexplicable System.UnauthorizedAccessException
                    }
                    catch (Exception ex)
                    {
                        string msg = string.Format("An exception occurred while deleting the old log file \n{0} \n\n{1}", oldFile, ex);
                        Logger.EventLogging.Log(msg, Logger.EventLogging.ExceptionInArchive);
                    }
                }
                else
                {
                    // Possibly rename the file with it's new archive number.  It might actually 
                    // get the same number again, but padded with a different number of zeros.
                    TryRename(oldFile, bareFilePath, newNum);
                }

                --newNum;
            }

            // Finally, rename the most recent log file, if given, to the *_01 file.

            if (renamedOutFile != null)
            {
                TryRename(renamedOutFile, bareFilePath, 1);
            }
        }

        private void TryRename(string from, string basePath, int num)
        {
            string newFile = MakeNumberedFileName(basePath, num);

            if (newFile != from)
            {
                try
                {
                    DateTime originalCreateTime = File.GetCreationTime(from);
                    File.Move(from, newFile);
                    File.SetCreationTime(newFile, originalCreateTime);
                }
                catch (Exception ex)
                {
                    string msg = string.Format("An exception occurred while renaming the old log file \n{0} \nto \n{1} \n\n{2}", from, newFile, ex);
                    Logger.EventLogging.Log(msg, Logger.EventLogging.ExceptionInArchive);

                    // Since we couldn't move/rename the file try to simply delete it to free up the name for the next file.

                    try
                    {
                        File.Delete(from); // Sometimes throws inexplicable System.UnauthorizedAccessException
                    }
                    catch (Exception delex)
                    {
                        string msg2 = string.Format("An exception occurred while deleting the old log file \n{0} \n\n{1}", from, delex);
                        Logger.EventLogging.Log(msg2, Logger.EventLogging.ExceptionInArchive);
                    }

                    //if (ex is IOException)
                    //{
                    //    // If the exception message is "Cannot create a file when that file already exists" then problem is the 
                    //    uint hresult = 0;

                    //    if (ex.Message == "Cannot create a file when that file already exists.")
                    //    {
                    //        hresult = 0x800700B7;
                    //    }
                    //    else
                    //    {
                    //        // Try to extract ex.HResult, which is protected before .NET Framework 4.5.

                    //        try
                    //        {
                    //            var info = new System.Runtime.Serialization.SerializationInfo(typeof(IOException), new System.Runtime.Serialization.FormatterConverter());
                    //            ex.GetObjectData(info, new System.Runtime.Serialization.StreamingContext());
                    //            hresult = info.GetUInt32("HResult");
                    //        }
                    //        catch (Exception ex2)
                    //        {
                    //            Logger.EventLogging.Log("Failed to extract HResult: " + ex2, Logger.EventLogging.ExceptionInArchive);
                    //        }
                    //    }

                    //    if (hresult == 0x800700B7)
                    //    {
                    //        // TODO: Try to delete the file.
                    //    }
                    //}
                }
            }
        }

        private string MakeNumberedFileName(string basePath, int num)
        {
            // Convert num to a string padded to the number of digits required to hold the highest num allowed (Archives).

            string numberedFileName = null;

            Debug.Assert(num <= Archives);

            if (Archives < 10)
            {
                numberedFileName = string.Format("{0}_{1:D1}{2}", basePath, num, _extension);
            }
            else if (Archives < 100)
            {
                numberedFileName = string.Format("{0}_{1:D2}{2}", basePath, num, _extension);
            }
            else if (Archives < 1000)
            {
                numberedFileName = string.Format("{0}_{1:D3}{2}", basePath, num, _extension);
            }
            else
            {
                numberedFileName = string.Format("{0}_{1:D4}{2}", basePath, num, _extension);
            }

            return numberedFileName;
        }

        private static string ExpandDirectoryString(string dir)
        {
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
        private static string GetDefaultDir()
        {
            try
            {
                // This always returns null in web apps, sometimes returns
                // null in the winforms designer.
                if (Assembly.GetEntryAssembly() == null)
                {
                    // We might be in a web app, but this will throw an
                    // exception if we're not.
                    return Logger.GetWebAppDir();
                }
                else
                {
                    // This generally means we're in a winforms app.
                    return Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                }
            }
            catch (Exception)
            {
                // Getting here means we're probably not in a web app.
                try
                {
                    return Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                }
                catch (Exception)
                {
                    // Give up. Return something to avoid an exception.
                    return "C:\\";
                }
            }
        }

        // Get the directory the EXE or website is in.
        private static string GetAppDir()
        {
            try
            {
                // This throws an exception in web apps and sometimes in
                // the winforms designer.
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            catch (Exception)
            {
                try
                {
                    // This SEEMS to return the correct value for both web apps
                    // and winforms apps.
                    return AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\', '/');
                }
                catch (Exception)
                {
                    try
                    {
                        // Expect an exception if we're not a web app.
                        return Logger.GetWebAppDir();
                    }
                    catch (Exception)
                    {
                        try
                        {
                            // Punt.
                            return Environment.CurrentDirectory;
                        }
                        catch (Exception)
                        {
                            // Give up.  Return something to avoid an exception.
                            return "C:\\";
                        }
                    }
                }
            }
        }

        protected abstract void InternalOpen();

        // Raises the Opening event and returns true if not canceled.
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