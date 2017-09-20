using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security;
using System.Net;
using System.IO;
using TracerX.ExtensionMethods;

namespace TracerX
{
    internal class RemoteServer
    {
        public RemoteServer(string hostAddress)
        {
            HostAddress = hostAddress;
            HostName = hostAddress;
            Log = Logger.GetLogger("RemoteServer." + hostAddress);

            _savedViewedFiles = new List<ViewedPath>();
            _savedViewedFolders = new List<ViewedPath>();
        }

        public RemoteServer(SavedServer savedServer)
        {            
            InitFromSavedServer(savedServer);
        }

        public string HostName;
        public string HostAddress;
        public int Port = -1;
        public string Category = "";
        public string UserId;
        public string PW; // encrypted

        public IEnumerable<PathItem> Files
        {
            get { return _dictFiles.Values; }
        }

        public IEnumerable<PathItem> Folders
        {
            get { return _dictFolders.Values; }
        }

        // These are the "recently viewed" files and folders loaded from saved
        // settings that may or may not still exist.
        private List<ViewedPath> _savedViewedFiles;
        private List<ViewedPath> _savedViewedFolders;

        // These are dictionaries of files and folders (both viewed and created) known to exist.  Not exhaustive.
        private Dictionary<string, PathItem> _dictFiles = new Dictionary<string,PathItem>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, PathItem> _dictFolders = new Dictionary<string,PathItem>(StringComparer.OrdinalIgnoreCase);

        private Logger Log = Logger.GetLogger("RemoteServer");

        /// <summary>
        /// Creates a list of RemoteServers from data stored in the user settings.
        /// </summary>
        public static List<RemoteServer> CreateListFromSettings()
        {
            var result = new List<RemoteServer>();

            if (Properties.Settings.Default.SavedRemoteServers != null)
            {
                DateTime cutoff = DateTime.Now.AddDays(-Properties.Settings.Default.DaysToSaveViewTimes);

                foreach (SavedServer savedServer in Properties.Settings.Default.SavedRemoteServers)
                {
                    result.Add(new RemoteServer(savedServer));
                }
            }

            return result;
        }

        public string HostAndPort
        {
            get
            {
                if (Port > 0)
                {
                    return HostAddress + ":" + Port.ToString();
                }
                else
                {
                    return HostAddress + ":25120";
                }
            }
        }

        public override string ToString()
        {
            return HostName;
        }

        public void InitFromSavedServer(SavedServer savedServer)
        {
            Log = Logger.GetLogger("RemoteServer." + savedServer.HostName);

            HostAddress = savedServer.HostAddress;
            Port = savedServer.Port;
            HostName = savedServer.HostName;

            Category = savedServer.Category;
            UserId = savedServer.UserId;
            PW = savedServer.PW;

            _savedViewedFiles = savedServer.ViewedFiles ?? new List<ViewedPath>();
            _savedViewedFolders = savedServer.ViewedFolders ?? new List<ViewedPath>();
        }

        /// <summary>
        /// Gets a RemoteFileStream for the specified file on this remote server.
        /// </summary>
        public RemoteFileStream GetRemoteFileStream(string filePath)
        {
            return new RemoteFileStream(filePath, HostAndPort, GetCreds());
        }

        /// <summary>
        /// Attempts to connect to the TraceX-Service on the specified server and
        /// returns true if successful.
        /// </summary>
        public bool CheckForService()
        {
            try
            {
                using (var proxy = new ProxyFileEnum())
                {
                    proxy.SetHost(HostAndPort);
                    proxy.SetCredentials(GetCreds());

                    Log.Info("Exchanging version numbers with ", HostAndPort);
                    int serverVersion = proxy.ExchangeVersion(1);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // The TracerX-Service probably just isn't running on the remote server.
                Log.Info("Exception checking for TracerX-Service on ", HostAndPort, ": ", ex.Message);
                return false;
            }
        }

        public void ForgetOldViewTimes(DateTime cutoff)
        {           
            _savedViewedFiles.RemoveAll(x => x.ViewTime < cutoff);
            _savedViewedFolders.RemoveAll(x => x.ViewTime < cutoff);
        }

        public void ForgetViewedFile(string filePath)
        {
            int ndx = _savedViewedFiles.FindIndex(item => item.Path.Equals(filePath, StringComparison.OrdinalIgnoreCase));

            if (ndx != -1)
            {
                _savedViewedFiles.RemoveAt(ndx);
            }
        }

        /// <summary>
        /// Calls the TracerX-Service on the remote server to get the latest
        /// lists of recently created files and modified folders.
        /// </summary>
        public void Refresh()
        {
            List<TXFileInfo> filesAndFolders = GetFilesAndFolders();

            _dictFiles.Clear();
            _dictFolders.Clear();

            // Create a PathItem for each file/folder.

            foreach (TXFileInfo txInfo in filesAndFolders)
            {
                var pathItem = new PathItem(txInfo);

                if (pathItem.IsFolder)
                {
                    _dictFolders[pathItem.FullPath] = pathItem;
                }
                else
                {
                    _dictFiles[pathItem.FullPath] = pathItem;
                }
            }

            // Get the PathItem for each recently viewed file and set its Viewtime.
            SetPathViewTimes(_dictFiles, _savedViewedFiles);

            // Get the PathItem for each recently viewed folder and set its ViewTime.
            SetPathViewTimes(_dictFolders, _savedViewedFolders);
        }

        public TestConnectionResult TestConnection()
        {
            var result = new TestConnectionResult()
                {
                    HostAndPort = this.HostAndPort
                };

            try
            {
                using (ProxyFileEnum serviceProxy = new ProxyFileEnum())
                {
                    serviceProxy.SetHost(HostAndPort);
                    serviceProxy.SetCredentials(GetCreds());
                    result.ServiceVersion = serviceProxy.ExchangeVersion(1);

                    if (result.ServiceVersion < 3)
                    {
                        // That's all we can get (the interface version).
                    }
                    else if (result.ServiceVersion < 5)
                    {
                        serviceProxy.GetServiceHostInfo(out result.HostExe, out result.HostVersion, out result.HostAccount);
                    }
                    else
                    {
                        // As of version 5, we can get whether or not the service is impersonating clients or not.
                        bool isImpersonating;
                        serviceProxy.GetServiceHostInfo2(out result.HostExe, out result.HostVersion, out result.HostAccount, out isImpersonating);
                        result.IsImpersonatingClients = isImpersonating;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        // Get the PathItem for each recently viewed file/folder and set its ViewTime from the saved view times.
        private static void SetPathViewTimes(Dictionary<string, PathItem> pathsToDisplay, List<ViewedPath> savedViewedPaths)
        {
            foreach (ViewedPath viewedPath in savedViewedPaths)
            {
                PathItem pathItem;

                if (pathsToDisplay.TryGetValue(viewedPath.Path, out pathItem))
                {
                    pathItem.ViewTime = viewedPath.ViewTime;
                }
            }
        }

        /// <summary>
        /// Calls the TracerX-Service on the remote server to 
        /// get the files in the specified folder.  Returns
        /// only the .tx1 files, if any.
        /// </summary>
        public List<PathItem> GetFilesInFolder(string folder)
        {
            var result = new List<PathItem>();
            List<TXFileInfo> files = null;

            using (var proxy = new ProxyFileEnum())
            {
                proxy.SetHost(HostAndPort);
                proxy.SetCredentials(GetCreds());

                Log.Info("Getting remote files from ", HostAndPort);
                files = proxy.GetFilesInFolder(folder);
            }

            foreach (TXFileInfo file in files)
            {
                ViewedPath viewedFile = _savedViewedFiles.FirstOrDefault(svf => svf.Path.Equals(file.FullPath, StringComparison.OrdinalIgnoreCase));
                PathItem pathItem;

                if (_dictFiles.TryGetValue(file.FullPath, out pathItem))
                {
                    // We already know about this file so just update the properties that might have changed.

                    pathItem.WriteTime = file.LastModified;
                    pathItem.CreateTime = file.Created;
                    pathItem.Size = file.Size;

                    if (viewedFile != null)
                    {
                        pathItem.ViewTime = viewedFile.ViewTime;
                    }
                }
                else
                {
                    pathItem = new PathItem(file, viewedFile);
                    _dictFiles.Add(pathItem.FullPath, pathItem);
                }

                // Create a new PathItem to return to the caller.
                // We never return an existing item from _dictFiles because that can cause the same
                // PathItem to be associated with two PathGridRows, which causes problems.

                pathItem = new PathItem(file, viewedFile);
                result.Add(pathItem);
            }

            return result;
        }

        // Called when the user views the specified file so we can keep track of which files
        // have been viewed and when.
        public void UpdateViewedFiles(string viewedFile)
        {
            // First, add/update the file in both file collections (_savedViewedFiles and _dictFiles).

            DateTime now = DateTime.Now;
            ViewedPath viewedFileObj = _savedViewedFiles.Find(item => item.Path.Equals(viewedFile, StringComparison.OrdinalIgnoreCase));

            if (viewedFileObj == null)
            {
                viewedFileObj = new ViewedPath { Path = viewedFile, ViewTime = now };
                _savedViewedFiles.Add(viewedFileObj);
            }
            else 
            {
                viewedFileObj.ViewTime = now;
            }

            PathItem filePathItem;

            if (!_dictFiles.TryGetValue(viewedFile, out filePathItem))
            { 
                // We assume the file exists because it was just loaded into the viewer.

                var txInfo = new TXFileInfo()
                {
                    IsFolder = false,
                    FullPath = viewedFile, 
                };

                filePathItem = new PathItem(txInfo);
                _dictFiles[viewedFile] = filePathItem;
            }

            filePathItem.ViewTime = now;

            // Now add/update the folder in both folder collections (_savedViewedFolders and _dictFolders).

            string folder = System.IO.Path.GetDirectoryName(viewedFile);
            ViewedPath viewedFolderObj = _savedViewedFolders.Find(item => item.Path.Equals(folder, StringComparison.OrdinalIgnoreCase));

            if (viewedFolderObj == null)
            {
                viewedFolderObj = new ViewedPath { Path = folder, ViewTime = now };
                _savedViewedFolders.Add(viewedFolderObj);
            }
            else
            {
                viewedFolderObj.ViewTime = now;
            }

            PathItem folderPathItem;

            if (!_dictFolders.TryGetValue(folder, out folderPathItem))
            {
                var txInfo = new TXFileInfo()
                {
                    IsFolder = true,
                    FullPath = folder
                };

                folderPathItem = new PathItem(txInfo);
                _dictFolders[folder] = folderPathItem;
            }

            folderPathItem.ViewTime = now;
        }

        /// <summary>
        /// Makes a SavedServer from this RemoteServer so the key properties 
        /// can be saved in the users settings.
        /// </summary>
        public SavedServer MakeSavedServer()
        {
            return new SavedServer
            {
                HostName = this.HostName,
                HostAddress = this.HostAddress,
                Port = this.Port,
                Category = this.Category,
                UserId = this.UserId,
                PW = this.PW,
                ViewedFiles = this._savedViewedFiles,
                ViewedFolders = this._savedViewedFolders,
            };
        }

        /// <summary>
        /// Gets NetworkCredential that encapsulates the Windows user ID and password
        /// to use for connecting to this server.  Returns null if UserId is null, meaning
        /// to use the current user's credentials.
        /// </summary>
        public NetworkCredential GetCreds()
        {
            if (string.IsNullOrWhiteSpace(UserId))
            {
                return null;
            }
            else if (PW.NullOrWhiteSpace())
            {
                throw new InvalidOperationException("Password is missing.  Password is required when \"Connect As\" is specified.");
            }
            else
            {
                return new NetworkCredential(UserId, Decrypt64(PW));
            }
        }

        /// <summary>
        /// Takes a base 64 string created by the Encrypt64 method and 
        /// converts it to a "decrypted" SecureString.
        /// </summary>
        private static SecureString Decrypt64(string base64)
        {
            SecureString result = new SecureString();
            byte[] bytes = Convert.FromBase64String(base64);
            bytes = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
            char[] chars = Encoding.Unicode.GetChars(bytes);

            // For good measure zero out the decrypted bytes ASAP.
            for (int i = 0; i < bytes.Length; ++i) bytes[i] = 0;

            for (int i = 0; i < chars.Length; ++i)
            {
                result.AppendChar(chars[i]);
                chars[i] = ' ';
            }

            return result;
        }

        private List<TXFileInfo> GetFilesAndFolders()
        {
            using (var proxy = new ProxyFileEnum())
            {
                proxy.SetHost(HostAndPort);
                proxy.SetCredentials(GetCreds());

                // If there's an exception, it probably occurs on the first call to the proxy.

                Log.Info("Exchanging version numbers with. ", HostAndPort);
                int serverVersion = proxy.ExchangeVersion(1);

                // We pass _savedViewedFiles and _savedViewedFolders to the service so it can
                // check if they still exist.  If they do, they will be in the result list.

                Log.Info("Getting remote files from ", HostName);
                List<TXFileInfo> filesAndFolders = proxy.GetRecentFilesAndFolders(_savedViewedFiles.Select(f => f.Path), _savedViewedFolders.Select(f => f.Path));
                Log.Info("Got ", filesAndFolders.Count, " results.");

                // TODO: This might be the place to remove files and folders that have
                // FoundInRecentFiles == false and whose last viewed time isn't very recent.
                
                return filesAndFolders;
            }
        }
    }
}
