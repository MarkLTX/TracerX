using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Reflection;

namespace TracerX
{
    /// <summary>
    /// Implements the IFileEnum interface used by TracerX clients (e.g. TracerX-Viewer) to enumerate the available log files.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, MaxItemsInObjectGraph = 1000000000, UseSynchronizationContext=false)]
    public class ImplFileEnum : IFileEnum
    {
        static readonly Logger Log = Logger.GetLogger("TX1.Service.ImplFileEnum");

        int _clientVersion;

        private static void LogIdentity()
        {
            Log.Debug("WindowsIdentity = ", WindowsIdentity.GetCurrent().Name);
            Log.Debug("Thread CurrentPrincipal = ", Thread.CurrentPrincipal.Identity.Name);
            Log.Debug(() => ServiceSecurityContext.Current.PrimaryIdentity.Name);
            Log.Debug(() => ServiceSecurityContext.Current.WindowsIdentity.Name);
        }

        #region IFileEnum Members

        public int ExchangeVersion(int clientVersion)
        {
            using (Log.DebugCall())
            {
                LogIdentity();
                _clientVersion = clientVersion;
                Log.Debug("Client version is ", clientVersion, " returning 5");
                return 5;
            }
        }

        // This was added in interface version 3.
        public void GetServiceHostInfo(out string executable, out string executableVersion, out string processUser)
        {
            // Try various ways of getting the executable path and version.

            try
            {
                var mainModule = System.Diagnostics.Process.GetCurrentProcess().MainModule;
                executable = mainModule.FileName;
                executableVersion = mainModule.FileVersionInfo.FileVersion;
            }
            catch (Exception ex)
            {
                try
                {
                    var entryAssembly = Assembly.GetEntryAssembly();
                    executable = entryAssembly.Location;
                    executableVersion = entryAssembly.GetName().Version.ToString();
                }
                catch
                {
                    try
                    {
                        executable = Environment.GetCommandLineArgs()[0];
                        executableVersion = "unknown";
                    }
                    catch
                    {
                        executable = Logger.AppName;
                        executableVersion = "unknown";
                    }
                }
            }

            try
            {
                processUser = WindowsIdentity.GetCurrent().Name;
            }
            catch
            {
                processUser = "unknown";
            }
        }

        // This was added in interface version 5.
        public void GetServiceHostInfo2(out string executable, out string executableVersion, out string processUser, out bool isImpersonatingClients)
        {
            isImpersonatingClients = TracerXServices.IsImpersonateClients;
            GetServiceHostInfo(out executable, out executableVersion, out processUser);
        }

        public List<TXFileInfo> GetRecentFilesAndFolders(IEnumerable<string> extraFiles, IEnumerable<string> extraFolders)
        {
            using (Log.DebugCall())
            {
                try
                {
                    string[] recentFileNames = null;
                    string[] recentFolderNames = null;
                    FileFolderChecker checker = new FileFolderChecker();

                    // Read both files under the mutex, then process the contents.

                    RecentlyCreated.GetFilesAndFolders(Log, out recentFileNames, out recentFolderNames);

                    if (recentFileNames != null)
                    {
                        checker.CheckFiles(recentFileNames, fromRecentFiles: true);
                    }

                    if (recentFolderNames != null)
                    {
                        checker.CheckFolders(recentFolderNames, fromRecentFiles: true);
                    }

                    // The "extra" files and folders are paths that might or might not be
                    // the recent files/folders lists, but the client wants us to check on.
                    // Typically they are from the Viewer's "recently viewed" lists.

                    if (extraFiles != null)
                    {
                        checker.CheckFiles(extraFiles, fromRecentFiles: false);
                    }

                    if (extraFolders != null)
                    {
                        checker.CheckFolders(extraFolders, fromRecentFiles: false);
                    }

                    Log.Debug("Returning ", checker.CombinedResult.Count, " items.");
                    return checker.CombinedResult;
                }
                catch (Exception ex)
                {
                    Log.Error("Exception in GetRecentFilesAndFolders(): ", ex);
                    throw;
                    //throw new FaultException<ExceptionDetail>(new ExceptionDetail(ex), "Exception in GetRecentFilesAndFolders() (on the server).");
                }
            }
        }

        /// <summary>
        /// Returns the paths of the .tx1 files in the specified folder.
        /// </summary>
        public List<TXFileInfo> GetFilesInFolder(string folder)
        {
            using (Log.DebugCall())
            {
                Log.Debug("Folder = ", folder);

                try
                {
                    var dirInfo = new DirectoryInfo(folder);

                    if (dirInfo.Exists)
                    {
                        var result = new List<TXFileInfo>();

                        foreach (FileInfo fi in dirInfo.GetFiles("*.tx1"))
                        {
                            result.Add(new TXFileInfo()
                            {
                                IsFolder = false,
                                FullPath = fi.FullName,
                                LastModified = fi.LastWriteTime,
                                Created = fi.CreationTime,
                                Size = fi.Length
                            });
                        }

                        Log.Debug("Returning ", result.Count, " files.");                        
                        return result;
                    }
                    else
                    {
                        // Tells the caller the folder doesn't exist.
                        Log.Debug("Folder doesn't exist, returning null.");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    throw;
                    //throw new FaultException<ExceptionDetail>(new ExceptionDetail(ex), "Exception getting files for folder '" + (folder ?? null) + "' (on the server).");
                }
            }
        }

        public void DeleteFile(string filePath)
        {
            using (Log.InfoCall())
            {
                // Ensure we only process .tx1 files for security.

                if (filePath.EndsWith(".tx1", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        if (TracerXServices.IsImpersonateClients)
                        {
                            Log.Info("Impersonating client ", ServiceSecurityContext.Current.WindowsIdentity.Name);

                            using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                            {
                                File.Delete(filePath);
                                // Have seen System.IO.IOException with message: Either a required impersonation level was not provided, or the provided impersonation level is invalid.
                            }
                        }
                        else
                        {
                            File.Delete(filePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the details and rethrow.  The client will see this as a FaultException<ExceptionDetail> exception whose Detail property is a summary of ex.
                        Log.Error("Exception in DeleteFile(): ", ex);
                        throw;
                    }
                }
                else
                {
                    Log.Warn("Throwing FaultException with message: Only *.tx1 files can be deleted.");
                    throw new FaultException("Only *.tx1 files can be deleted.");
                }
            }
        }

        // This was added in interface version 4.
        public List<string> DeleteAllRelated(string folderPath, string baseLogName, bool deleteEmptyFolderAndParents)
        {
            if (TracerXServices.IsImpersonateClients)
            {
                return DeleteRelated.DeleteAllRelated(folderPath, baseLogName, Log, deleteEmptyFolderAndParents, ServiceSecurityContext.Current.WindowsIdentity);
            }
            else
            {
                return DeleteRelated.DeleteAllRelated(folderPath, baseLogName, Log, deleteEmptyFolderAndParents, null);
            }
        }

        // This was added in interface version 4.
        public List<string> DeleteMany(string fileSpec, string folderSpec, DateTime minTimestamp, DateTime maxTimestamp, long minSize, long maxSize, bool tx1Files, bool txtFiles, bool deleteEmptyFolderAndParents, bool listOnly)
        {
            if (TracerXServices.IsImpersonateClients)
            {
                return DeleteRelated.DeleteMany(fileSpec, folderSpec, minTimestamp, maxTimestamp, minSize, maxSize, tx1Files, txtFiles, deleteEmptyFolderAndParents, listOnly, Log, ServiceSecurityContext.Current.WindowsIdentity);
            }
            else
            {
                return DeleteRelated.DeleteMany(fileSpec, folderSpec, minTimestamp, maxTimestamp, minSize, maxSize, tx1Files, txtFiles, deleteEmptyFolderAndParents, listOnly, Log);
            }
        }

        #endregion
    }
}
