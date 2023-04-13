using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace TracerX
{
    /// <summary>
    /// Methods and configuration for logging to a text file.
    /// </summary>
    public sealed class TextFile : FileBase
    {
        private string _formatString = "{time:HH:mm:ss.fff} {level} {thname} {logger}+{method} {ind}{msg}";

        private string _internalFormatString;

        private StreamWriter _logfile;

        private long _lastPhysicalLinePos = long.MaxValue;

        private bool _wrapped = false;

        /// <summary>
        /// Constructs an unopened TracerX TextFile with default Name and Directory.
        /// </summary>
        public TextFile()
            : base(".txt")
        {
            _internalFormatString = Logger.ParseFormatString(_formatString);
        }

        /// <summary>
        /// Controls which fields are written to the text file by all loggers.
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
        public string FormatString
        {
            get { return _formatString; }
            set
            {
                lock (_fileLocker)
                {
                    _formatString = value;
                    _internalFormatString = Logger.ParseFormatString(value);
                }
            }
        }

        /// <summary>
        /// Is the output file currently open?
        /// </summary>
        public override bool IsOpen { get { return _logfile != null; } }

        /// <summary>
        /// Has circular logging started (not necessarily wrapped)?
        /// </summary>
        public override bool CircularStarted { get { return IsOpen && _positionOfCircularPart > 0; } }

        /// <summary>
        /// Returns true if the file size has exceeded the max size.  Once this becomes
        /// true, future output replaces old output.
        /// </summary>
        public override bool Wrapped { get { return IsOpen && _wrapped; } }

        /// <summary>
        /// _logfile.BaseStream.
        /// </summary>
        protected override Stream BaseStream
        {
            get { return _logfile.BaseStream; }
        }

        /// <summary>
        /// Closes the log file.  It should not be reopened.
        /// </summary>
        public override void Close()
        {
            lock (_fileLocker)
            {
                if (_logfile != null)
                {
                    OnClosing();

                    _logfile.Flush();
                    BaseStream.Flush();
                    _logfile.Dispose();
                    _logfile = null;
                    _lastPhysicalLinePos = long.MaxValue;
                    _wrapped = false;

                    base.Close();

                    OnClosed();
                }
            }
        }

        // This either opens the originally specified log file, opens an
        // alternate log file, or throws an exception.
        protected override void InternalOpen()
        {
            // Use this to generate alternate file names A-Z if file can't be opened.
            char c = 'A';
            string renamedFile = null;
            string simpleName = _logFileName;

            while (_logfile == null)
            {
                try
                {
                    var outFile = new FileInfo(FullPath);
                    bool appending = false;

                    if (outFile.Exists)
                    {
                        if (outFile.Length < AppendIfSmallerThanMb << _shift)
                        {
                            // Open in append mode.
                            appending = true;
                        }
                        else
                        {
                            if (Archives > 0)
                            {
                                renamedFile = FullPath + ".tempname";
                                File.Delete(renamedFile); // MSDN says no exception if file doesn't exist.

                                // If the file is in use, this throws an exception.
                                outFile.MoveTo(renamedFile);
                            }
                        }
                    }

                    // If the file is in use, this throws an exception.
                    _logfile = OpenStreamWriter(appending);
                }
                catch (System.IO.IOException)
                {
                    // File is probably in use, try next alternate name.
                    if (c >= 'Z')
                    {
                        // That was the last chance.  Rethrow the exception to
                        // end the loop and cause the exception to be logged.
                        throw;
                    }
                    else
                    {
                        // Try the next alternative file name, up to Z.
                        // Changing _logFileName also changes Name and FullName.
                        // Note that _logFileName has no extension or _00.
                        _logFileName = string.Format("{0}({1})", simpleName, c);
                        ++c;
                        renamedFile = null;
                        continue;
                    }
                }
            } // while

            ++CurrentFile;

            // This guarantees the file won't be emtpy, and may help user understand what's going on with rolling or wrapping.
            _logfile.WriteLine("TracerX: Log file opened at {0}.  FullFilePolicy = {1}, Use_00 = {2}, MaxSize = {3} {4}, AppendIfSmallerThan = {5} {4}, initial size = {6}.", _openTimeUtc.ToLocalTime(), FullFilePolicy, Use_00, MaxSizeMb, UseKbForSize ? "KB" : "MB", AppendIfSmallerThanMb, BaseStream.Length);

            RollArchives(renamedFile);
        }

        //// Manages the archive files (*_01, *_02, etc.).
        //// Parameter renamedFile is what the old output file was renamed 
        //// to if it existed (with extension .tempname).
        //// It must become the _01 file.
        //// if renamedFile is null, the old output file wasn't replaced (did't
        //// exist or was opened in append mode), and no renaming is necessary,
        //// but we still delete files with numbers higher than Archives.
        //private void ManageArchives(string renamedFile)
        //{
        //    string bareFilePath = Path.Combine(Directory, _logFileName);
        //    int highestNumKept = (renamedFile == null) ? (int)Archives : (int)Archives - 1;

        //    // This gets the archived files in reverse order.
        //    string[] files = EnumOldFiles("_??.txt");

        //    if (files != null)
        //    {
        //        foreach (string oldFile in files)
        //        {
        //            // Extract the archive number that comes after "bareFileName_".
        //            // The number must be two numeric chars or it's not one of our files.
        //            string plain = Path.GetFileNameWithoutExtension(oldFile);
        //            string numPart = plain.Substring(_logFileName.Length + 1);

        //            if (numPart.Length == 2)
        //            {
        //                int num;

        //                if (int.TryParse(numPart, out num) && num > 0)
        //                {
        //                    if (num > highestNumKept)
        //                    {
        //                        // The archive number is more than the user wants to keep, so delete it.
        //                        try
        //                        {
        //                            File.Delete(oldFile);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            string msg = string.Format("An exception occurred while deleting the old log file\n{0}\n\n{1}", oldFile, ex);
        //                            Logger.EventLogging.Log(msg, Logger.EventLogging.ExceptionInArchive);
        //                        }
        //                    }
        //                    else if (renamedFile != null)
        //                    {
        //                        // Rename (increment the file's archive number by 1).
        //                        TryRename(oldFile, bareFilePath, num + 1);
        //                    }
        //                }
        //            }
        //        }

        //        // Finally, rename the most recent log file, if it exists.
        //        if (renamedFile != null)
        //        {
        //            TryRename(renamedFile, bareFilePath, 1);
        //        }
        //    }
        //}

        // Logs a message to the text file, possibly wrapping or starting a new file.  
        internal void LogMsg(Logger logger, ThreadData threadData, TraceLevel msgLevel, string msg)
        {
            lock (_fileLocker)
            {
                try
                {
                    if (IsOpen)
                    {
                        DateTime utcNow = DateTime.UtcNow;
                        DateTime localNow = utcNow.ToLocalTime();

                        //System.Diagnostics.Debug.Print("Position = " + BaseStream.Position);

                        if (BaseStream.Position >= _maxFilePosition)
                        {
                            // The previously written line reached the max file size, so apply the FullFilePolicy.

                            switch (FullFilePolicy)
                            {
                                case FullFilePolicy.Close:
                                    // This should really never happen because we check for this condition
                                    // immediately after writing the line.
                                    Close();
                                    break;
                                case FullFilePolicy.Roll:
                                    RestartFile();
                                    WriteLine(logger, threadData, msgLevel, localNow, msg);
                                    break;
                                case FullFilePolicy.Wrap:
                                    if (CircularStarted)
                                    {
                                        string wrapMsg = String.Format("TracerX: Returning (wrapping) to file position {0} for next log message.", _positionOfCircularPart);
                                        WriteLine(logger, threadData, msgLevel, localNow, wrapMsg);
                                        BaseStream.Position = _positionOfCircularPart;

                                        if (!_wrapped)
                                        {
                                            _wrapped = true;
                                            Logger.EventLogging.Log("The text file wrapped for the first time: " + FullPath, Logger.EventLogging.FirstWrap);
                                        }

                                        WriteLine(logger, threadData, msgLevel, localNow, msg);
                                    }
                                    else
                                    {
                                        // Since the circular didn't start, there's no where to wrap to.  Log an error and close the file.

                                        string closeMsg = string.Format("TracerX: File is closing (not wrapping) because FullFilePolicy = Wrap and the file reached maximum size ({0}) before circular logging started.  ", _maxFilePosition);
                                        Logger.EventLogging.Log(closeMsg + "\n" + FullPath, Logger.EventLogging.MaxFileSizeReached);

                                        WriteLine(logger, threadData, msgLevel, localNow, closeMsg);

                                        closeMsg = string.Format("MaxSizeMb = {0}, AppendIfSmallerThanMb = {1}, UseKbForSize = {2}", MaxSizeMb, AppendIfSmallerThanMb, UseKbForSize);
                                        WriteLine(logger, threadData, msgLevel, localNow, closeMsg);

                                        Close();
                                    }

                                    break;
                            }
                        }
                        else
                        {
                            // Possibly start the circular log based on the current time and/or file size.
                            // Note that _circularStartTime is UTC.

                            if (FullFilePolicy == FullFilePolicy.Wrap && !CircularStarted &&
                                (utcNow >= _circularStartTime ||
                                (CircularStartSizeKb > 0 && (BaseStream.Position - _openSize) >= CircularStartSizeKb << 10)))
                            {
                                // Start the circular log, which basically means setting _positionOfCircularPart so we know where
                                // to set the file position when it's time to wrap in the future.

                                WriteLine(logger, threadData, msgLevel, localNow, "TracerX: Last line before circular log starts.");
                                _positionOfCircularPart = BaseStream.Position;
                                WriteLine(logger, threadData, msgLevel, localNow, "TracerX: First line in circular portion of log (never wrapped if you see this whole line).");

                                if (BaseStream.Position >= _maxFilePosition)
                                {
                                    // The messages we just wrote exceeded the max file size, which means there really wasn't enough room to do circular logging.
                                    Logger.EventLogging.Log("Circular logging would have started, but there was not enough room left in the file: " + FullPath, Logger.EventLogging.TooLateForCircular);
                                    WriteLine(logger, threadData, msgLevel, localNow, "TracerX: Max file size exceeded.  FullFilePolicy = Wrap, but insufficient space to start the circular log.  Closing file (not rolling).");

                                    Close();
                                }
                                else
                                {
                                    string eventMsg = String.Format("Circular logging has started for text file '{0}' with {1:N0} bytes remaining.", FullPath, _maxFilePosition - CurrentPosition);
                                    Logger.EventLogging.Log(eventMsg, Logger.EventLogging.CircularLogStarted);
                                }
                            }

                            WriteLine(logger, threadData, msgLevel, localNow, msg);

                            if (FullFilePolicy == FullFilePolicy.Close && IsOpen && BaseStream.Position >= _maxFilePosition)
                            {
                                string closeMsg = string.Format("TracerX: Closing file (not rolling or wrapping) because the maximum size was reached and FullFilePolicy = Close, MaxSizeMb = {0}, AppendIfSmallerThanMb = {1}, UseKbForSize = {2}", MaxSizeMb, AppendIfSmallerThanMb, UseKbForSize);
                                WriteLine(logger, threadData, msgLevel, localNow, closeMsg);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Give up!  close the log file.
                    Logger.EventLogging.Log("An exception was thrown while logging to the text file: " + ex.ToString(), Logger.EventLogging.ExceptionInLogger);
                    Close();
                }
            }
        }

        private void RestartFile()
        {
            // Log an event.
            string msg = "The following text log file is being closed and reopened (rolled):\n" + FullPath;
            Logger.EventLogging.Log(msg, Logger.EventLogging.LogFileReopening);

            // Write an explanation at the end of the current file.
            msg = string.Format("TracerX: Rolling the file.  FullFilePolicy = {0}, Use_00 = {1}, MaxSize = {2} {3}.", FullFilePolicy, Use_00, MaxSizeMb, UseKbForSize ? "KB" : "MB");
            _logfile.WriteLine(msg);

            // Close, roll, and reopen.
            Close();
            Open();

            // Write an explanation at the top of the new file.
            msg = string.Format("TracerX: Log file reopened (rolled).  FullFilePolicy = {0}, Use_00 = {1}, MaxSize = {2} {3}.", FullFilePolicy, Use_00, MaxSizeMb, UseKbForSize ? "KB" : "MB");
            _logfile.WriteLine(msg);
        }

        private StreamWriter OpenStreamWriter(bool append)
        {
            // Use an EncoderReplacementFallback to replace any invalid UTF-16 chars
            // found in logged strings with '?' (System.String uses UTF-16 internally).
            var EncoderFallback = new EncoderReplacementFallback("?");
            var utf8WithFallback = Encoding.GetEncoding("UTF-8", EncoderFallback, new DecoderExceptionFallback());

            // If the file is in use, this throws an exception.
            var result = new StreamWriter(FullPath, append, utf8WithFallback);

            _openSize = result.BaseStream.Length;
            _openTimeUtc = DateTime.UtcNow;
            _maxFilePosition = MaxSizeMb << _shift;
            _lastPhysicalLinePos = long.MaxValue;
            _positionOfCircularPart = 0;
            _wrapped = false;

            if (CircularStartDelaySeconds == 0)
            {
                _circularStartTime = DateTime.MaxValue;
            }
            else
            {
                // This isn't the only place where _circularStartTime is set, and the
                // other place uses UTC, so we must also.
                _circularStartTime = _openTimeUtc.AddSeconds(CircularStartDelaySeconds);
            }

            // Without AutoFlush, BaseStream.Position doesn't change with every write.
            result.AutoFlush = true;

            return result;
        }

        private void WriteLine(Logger logger, ThreadData threadData, TraceLevel msgLevel, DateTime now, string msg)
        {
            long startPos = BaseStream.Position;
            string indent = "";

            threadData.GetTextFileState(logger.TextFile); // Sets threadData.TextFileState.

            if (threadData.TextFileState.StackDepth > 0)
            {
                indent = new string(' ', 3 * threadData.TextFileState.StackDepth);
            }

            ++_lineCnt;

            _logfile.WriteLine(_internalFormatString,
                _lineCnt,
                Enum.GetName(typeof(TraceLevel), msgLevel),
                logger.Name,
                threadData.TracerXID,
                threadData.Name ?? "<null>",
                now,
                threadData.TextFileState.CurrentMethod ?? "<null>",
                indent,
                msg ?? "<null>"
                );

            if (startPos <= _lastPhysicalLinePos && BaseStream.Position > _lastPhysicalLinePos)
            {
                // We just overwrote the start of last physcial line in the file, so
                // truncate the file in case that line was very long.
                BaseStream.SetLength(BaseStream.Position);

                // There is no last physical line until we reach _maxFilePosition again.
                _lastPhysicalLinePos = long.MaxValue;
            }

            if (startPos < _maxFilePosition && BaseStream.Position >= _maxFilePosition)
            {
                // The line we just wrote is the one that crossed the threshold.  Remember its location.
                _lastPhysicalLinePos = startPos;
            }
        }
    }
}