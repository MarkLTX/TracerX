using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;

namespace TracerX
{
    /// <summary>
    /// Implements the IFileReader WCF service.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single, MaxItemsInObjectGraph = 1000000000, UseSynchronizationContext = false)]
    public class ImplFileReader : IFileReader, IDisposable
    {
        static readonly Logger Log = Logger.GetLogger("TX1.Service.ImplFileReader");

        // Created by OpenFile().
        private FileStream _fileStream;

        #region IFileReader members

        public int ExchangeVersion(int clientVersion)
        {
            // return 1;
            // return 2; // Added FileShare.Delete when opening the file so the file can be kept open.
            return 3; // Added IFileMonitor.StartWatching(string filePath, Guid guidForEvents).
        }

        /// <summary>
        /// Opens the specified file for reading.  The open file is associated with the current WCF session.  The file extension must be ".tx1".
        /// </summary>
        public void OpenFile(string file)
        {
            using (Log.InfoCall())
            {
                Log.Info("File = ", file);
                Log.Info("Calling user is ", ServiceSecurityContext.Current?.WindowsIdentity?.Name);

                if (string.IsNullOrEmpty(file))
                {
                    throw new FaultException("The file path is null or empty.");
                }
                else if (!file.EndsWith(".tx1", StringComparison.OrdinalIgnoreCase))
                {
                    throw new FaultException("The file extension must be \".tx1\".");
                }
                else if (_fileStream != null)
                {
                    throw new FaultException("A file is already open for this session.");
                }
                else
                {
                    try
                    {
                        if (TracerXServices.IsImpersonateClients)
                        {
                            using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                            {
                                _fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                                // Have seen System.IO.IOException with message: Either a required impersonation level was not provided, or the provided impersonation level is invalid.
                            }
                        }
                        else
                        {
                            _fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                        }

                        Log.Info("Opened file of length ", _fileStream.Length);
                    }
                    catch (Exception ex)
                    {
                        // Log the details and rethrow.  The client will see this as a FaultException<ExceptionDetail> exception whose Detail property is a summary of ex.
                        Log.Error(ex);
                        throw;
                    }
                }
            }
        }

        public void CloseFile()
        {
            using (Log.InfoCall())
            {
                try
                {
                    if (_fileStream != null)
                    {
                        _fileStream.Dispose();
                        _fileStream = null;
                    }
                }
                catch (Exception ex)
                {
                    // Log the details and rethrow.  The client will see this as a FaultException<ExceptionDetail> exception whose Detail property is a summary of ex.
                    Log.Error(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Reads and returns bytes from the file.
        /// May return fewer bytes than requested.
        /// </summary>
        public byte[] ReadBytes(long position, int count, out long fileLength)
        {
            using (Log.VerboseCall())
            {
                Log.Verbose("Requested position is ", position, ", byte count is ", count);

                if (_fileStream == null)
                {
                    throw new FaultException("No file is open for this session.");
                }
                else
                {
                    try
                    {
                        var result = new byte[count];
                        _fileStream.Position = position;
                        int len = _fileStream.Read(result, 0, count);

                        if (len != count)
                        {
                            var shortResult = new byte[len];
                            Array.Copy(result, shortResult, len);
                            result = shortResult;
                        }

                        // In an earlier version of this method the fileLength parameter was set to _fileStream.Length at the
                        // top of the method and that caused the returned byte[] to sometimes be longer than returned fileLength
                        // (thus causing errors in the viewer).  Try to get a more accurate length by getting it just before we
                        // return and also ensure it's big enough to account for the returned byte[].

                        fileLength = _fileStream.Length;
                        if (len > 0 && fileLength < position + len) fileLength = position + len;

                        if (len == count)
                        {
                            Log.Debug("Returning file length ", fileLength, " and  all ", count , " requested bytes");
                        }
                        else
                        {
                            Log.Debug("Returning file length ", fileLength, " and ", len, " of ", count, " requested bytes");
                        }

                        return result;
                    }
                    catch (Exception ex)
                    {
                        // Log the details and rethrow.  The client will see this as a FaultException<ExceptionDetail> exception whose Detail property is a summary of ex.
                        Log.Error(ex);
                        throw;
                    }
                }
            }
        }

        public long GetLength()
        {
            //using (Log.InfoCall())
            {
                if (_fileStream == null)
                {
                    throw new FaultException("No file is open for this session.");
                }
                else
                {
                    try
                    {
                        var result = _fileStream.Length;
                        Log.Debug("GetLength() returning ", result);
                        return result;

                    }
                    catch (Exception ex)
                    {
                        // Log the details and rethrow.  The client will see this as a FaultException<ExceptionDetail> exception whose Detail property is a summary of ex.
                        Log.Error(ex);
                        throw;
                    }
                }
            }
        }

        // Not typically used because ReadBytes() takes a position parameter.
        public void SetPosition(long pos)
        {
            using (Log.VerboseCall())
            {
                Log.Debug("Setting position to ", pos);

                if (_fileStream == null)
                {
                    throw new FaultException("No file is open for this session.");
                }
                else
                {
                    try
                    {
                        _fileStream.Position = pos;
                    }
                    catch (Exception ex)
                    {
                        // Log the details and rethrow.  The client will see this as a FaultException<ExceptionDetail> exception whose Detail property is a summary of ex.
                        Log.Error(ex);
                        throw;
                    }
                }
            }
        }

        #endregion

        // WCF will call this when the client disconnects.
        public void Dispose()
        {
            using (Log.InfoCall())
            {
                try
                {
                    var temp = _fileStream;

                    if (temp != null)
                    {
                        Log.Info("Disposing the FileStream.");
                        temp.Dispose();
                        _fileStream = null;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

    }
}
