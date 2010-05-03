﻿using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ComponentModel;

namespace TracerX {
    public partial class Logger {
        /// <summary>
        /// Methods and configuration for logging to a text file.
        /// </summary>
        public sealed class TextFile : FileBase {
            private string _formatString = "{time:HH:mm:ss.fff} {level} {thname} {logger}+{method} {ind}{msg}";
            
            private string _internalFormatString;

            private StreamWriter _logfile;

            private long _lastPhysicalLinePos = long.MaxValue;

            private bool _wrapped = false;

            #region Singleton

            // Private ctor to guarantee singleton.
            private TextFile()
                :base(".txt")
            {
                _internalFormatString = ParseFormatString(_formatString);
            }

            static internal TextFile Singleton {
                get { return _singleton; }
            }

            private static TextFile _singleton = new TextFile();

            #endregion Singleton

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
            public string FormatString {
                get { return _formatString; }
                set {
                    lock (_fileLocker) {
                        _formatString = value;
                        _internalFormatString = ParseFormatString(value);
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
            protected override Stream BaseStream {
                get { return _logfile.BaseStream; }
            }

            /// <summary>
            /// Closes the log file.  It should not be reopened.
            /// </summary>
            public override void Close() {
                lock (_fileLocker) {
                    if (_logfile != null) {
                        _logfile.Flush();
                        BaseStream.Flush();
                        _logfile.Dispose();
                        _logfile = null;
                    }
                }
            }

            // This either opens the originally specified log file, opens an
            // alternate log file, or throws an exception.
            protected override void InternalOpen() {
                // Use this to generate alternate file names A-Z if file can't be opened.
                char c = 'A';
                string renamedFile = null;

                while (_logfile == null) {
                    try {
                        var outFile = new FileInfo(FullPath);
                        bool appending = false;

                        if (outFile.Exists) {
                            if (outFile.Length < AppendIfSmallerThanMb << 20) {
                                // Open in append mode.
                                appending = true;
                            } else {
                                if (Archives > 0) {
                                    renamedFile = FullPath + ".tempname";
                                    if (File.Exists(renamedFile)) File.Delete(renamedFile);

                                    // If the file is in use, this throws an exception.
                                    outFile.MoveTo(renamedFile);
                                }
                            }
                        }

                        // If the file is in use, this throws an exception.
                        _logfile = OpenStreamWriter(appending);
                    } catch (System.IO.IOException) {
                        // File is probably in use, try next alternate name.
                        if (c > 'Z') {
                            // That was the last chance.  Rethrow the exception to
                            // end the loop and cause the exception to be logged.
                            throw;
                        } else {
                            // Try the next alternative file name, up to Z.
                            // Changing Name also changes FullName.
                            string bareFileName = Path.GetFileNameWithoutExtension(Name);
                            Name = string.Format("{0}({1})", bareFileName, c);
                            ++c;
                            renamedFile = null;
                            continue;
                        }
                    }
                } // while

                // This guarantees there is something before the circular part.
                _logfile.WriteLine("Log file opened at {0}", _openTimeUtc.ToLocalTime());

                ManageArchives(renamedFile);
            }

            // Manages the archive files (*_01, *_02, etc.).
            // Parameter renamedFile is what the old output file was renamed 
            // to if it existed (with extension .tempname).
            // It must become the _01 file.
            // if renamedFile is null, the old output file wasn't replaced (did't
            // exist or was opened in append mode), and no renaming is necessary.
            private void ManageArchives(string renamedFile) {
                string bareFileName = Path.GetFileNameWithoutExtension(Name);
                string bareFilePath = Path.Combine(Directory, bareFileName);
                int highestNumKept = (renamedFile == null) ? (int)Archives : (int)Archives - 1;

                // This gets the archived files in reverse order.
                string[] files = EnumOldFiles(bareFileName, "_??.txt");

                if (files != null) {
                    foreach (string oldFile in files) {
                        // Extract the archive number that comes after "bareFileName_".
                        // The number must be two numeric chars or it's not one of our files.
                        string plain = Path.GetFileNameWithoutExtension(oldFile);
                        string numPart = plain.Substring(bareFileName.Length + 1);

                        if (numPart.Length == 2) {
                            int num;

                            if (int.TryParse(numPart, out num) && num > 0) {
                                if (num > highestNumKept) {
                                    // The archive number is more than the user wants to keep, so delete it.
                                    try {
                                        File.Delete(oldFile);
                                    } catch (Exception ex) {
                                        string msg = string.Format("An exception occurred while deleting the old log file\n{0}\n\n{1}", oldFile, ex);
                                        EventLogging.Log(msg, EventLogging.ExceptionInArchive);
                                    }
                                } else if (renamedFile != null) {
                                    // Rename (increment the file's archive number by 1).
                                    TryRename(oldFile, bareFilePath, num + 1);
                                }
                            }
                        }
                    }

                    // Finally, rename the most recent log file, if it exists.
                    if (renamedFile != null) {
                        TryRename(renamedFile, bareFilePath, 1);
                    }
                }
            }

            // Logs a message to the text file, possibly wrapping or starting a new file.  
            internal void LogMsg(Logger logger, ThreadData threadData, TraceLevel msgLevel, string msg) {
                lock (_fileLocker) {
                    try {
                        if (IsOpen) {
                            DateTime utcNow = DateTime.UtcNow;
                            DateTime localNow = utcNow.ToLocalTime();

                            //System.Diagnostics.Debug.Print("Position = " + BaseStream.Position);

                            if (BaseStream.Position >= _maxFilePosition) {
                                // The previously written line reached the max file size, so we
                                // must either wrap or start a new file.
                                if (CircularStarted) {
                                    // Wrap
                                    string wrapMsg = String.Format("TracerX: Returning (wrapping) to file position {0} for next log message.", _positionOfCircularPart);
                                    WriteLine(logger, threadData, msgLevel, localNow, wrapMsg);
                                    BaseStream.Position = _positionOfCircularPart;
                                    
                                    if (!_wrapped) {
                                        _wrapped = true;
                                        EventLogging.Log("The text file wrapped for the first time: " + FullPath, EventLogging.FirstWrap);
                                    }
                                } else {
                                    RestartFile();
                                }
                            } else {
                                // Possibly start the circular log based on the current time and/or file size.
                                // Note that _circularStartTime is UTC.
                                if (!CircularStarted && (utcNow >= _circularStartTime ||
                                    (CircularStartSizeKb > 0 && (BaseStream.Position - _openSize) >= CircularStartSizeKb << 10))) //
                                {
                                    WriteLine(logger, threadData, msgLevel, localNow, "TracerX: Last line before circular log starts.");
                                    _positionOfCircularPart = BaseStream.Position;
                                    WriteLine(logger, threadData, msgLevel, localNow, "TracerX: First line in circular portion of log (never wrapped if you see this).");

                                    if (BaseStream.Position >= _maxFilePosition) {
                                        EventLogging.Log("Circular logging would have started, but there was not enough room left in the file: " + FullPath, EventLogging.TooLateForCircular);
                                        WriteLine(logger, threadData, msgLevel, localNow, "TracerX: Max file size exceeded.  Insufficient space to start the circular log.  Closing file.");
                                        RestartFile();
                                    } else {
                                        string eventMsg = String.Format("Circular logging has started for text file '{0}' with {1:N0} bytes remaining.", FullPath, _maxFilePosition - CurrentPosition);
                                        EventLogging.Log(eventMsg, EventLogging.CircularLogStarted);
                                    }
                                }
                            }

                            WriteLine(logger, threadData, msgLevel, localNow, msg);
                        }
                    } catch (Exception ex) {
                        // Give up!  close the log file.
                        EventLogging.Log("An exception was thrown while logging to the text file: " + ex.ToString(), EventLogging.ExceptionInLogger);
                        Close();
                    }
                }
            }

            private void RestartFile() {
                string msg = "The following text log file is being closed and reopened:\n" + FullPath;
                EventLogging.Log(msg, EventLogging.LogFileReopening);

                Close();
                ManageArchives(FullPath);
                _logfile = OpenStreamWriter(false);

                // This guarantees there is something before the circular part.
                _logfile.WriteLine("TracerX: Log file reopened.");
            }

            private StreamWriter OpenStreamWriter(bool append) {
                // Use an EncoderReplacementFallback to replace any invalid UTF-16 chars
                // found in logged strings with '?' (System.String uses UTF-16 internally).
                var EncoderFallback = new EncoderReplacementFallback("?");
                var utf8WithFallback = Encoding.GetEncoding("UTF-8", EncoderFallback, new DecoderExceptionFallback());

                // If the file is in use, this throws an exception.
                var result = new StreamWriter(FullPath, append, utf8WithFallback);

                _openSize = result.BaseStream.Length;
                _openTimeUtc = DateTime.UtcNow;
                _maxFilePosition = _openSize + (MaxSizeMb << 20);
                _lastPhysicalLinePos = long.MaxValue;
                _positionOfCircularPart = 0;
                _wrapped = false;

                if (CircularStartDelaySeconds == 0) {
                    _circularStartTime = DateTime.MaxValue;
                } else {
                    // This isn't the only place where _circularStartTime is set, and the
                    // other place uses UTC, so we must also.
                    _circularStartTime = _openTimeUtc.AddSeconds(CircularStartDelaySeconds);
                }

                // Without AutoFlush, BaseStream.Position doesn't change with every write.
                result.AutoFlush = true;

                return result;
            }

            private void WriteLine(Logger logger, ThreadData threadData, TraceLevel msgLevel, DateTime now, string msg) {
                long startPos = BaseStream.Position;
                string indent = "";

                if (threadData.TextFileStackDepth > 0) {
                    indent = new string(' ', 3 * threadData.TextFileStackDepth);
                }
                
                ++_lineCnt;

                _logfile.WriteLine(_internalFormatString,
                    _lineCnt,
                    Enum.GetName(typeof(TraceLevel), msgLevel),
                    logger.Name,
                    threadData.TracerXID,
                    Thread.CurrentThread.Name ?? "<null>",
                    now,
                    threadData.CurrentTextFileMethod ?? "<null>",
                    indent,
                    msg ?? "<null>"
                    );

                if (startPos <= _lastPhysicalLinePos && BaseStream.Position > _lastPhysicalLinePos) {
                    // We just overwrote the start of last physcial line in the file, so
                    // truncate the file in case that line was very long.
                    BaseStream.SetLength(BaseStream.Position);

                    // There is no last physical line until we reach _maxFilePosition again.
                    _lastPhysicalLinePos = long.MaxValue;
                }

                if (startPos < _maxFilePosition && BaseStream.Position >= _maxFilePosition) {
                    // The line we just wrote is the one that crossed the threshold.  Remember its location.
                    _lastPhysicalLinePos = startPos;
                }
            }
        }
    }
}