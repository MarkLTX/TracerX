using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace TracerX
{
    // Represents a file or folder displayed in the Files or Folders list on the start page.
    internal class PathItem : IEquatable<PathItem>, IComparable<PathItem>
    {
        /// <summary>
        /// This ctor is for a local file/folder, for which we already have a FileInfo/DirectoryInfo.
        /// </summary>
        public PathItem(FileSystemInfo path, bool isFromRecentlyCreated)
        {
            // Do not reference path.Exists, path.LastWriteTime, or other properties
            // that depend on the actual existence of the file/folder, because it
            // may take a long time if the file/folder is not reachable.

            FullPath = path.FullName;
            File = path as FileInfo;
            IsFromRecentlyCreatedList = isFromRecentlyCreated;

            if (File == null)
            {
                IsFolder = true;
                Folder = (DirectoryInfo)path;
                ItemName = Folder.Name;
                ContainerName = Folder.Parent.FullName;
            }
            else
            {
                IsFolder = false;
                ItemName = Path.GetFileNameWithoutExtension(File.Name);
                ContainerName = File.DirectoryName;
            }

            Init();
        }

        /// <summary>
        /// This ctor is for a LOCAL file/folder.  It will throw an exception if
        /// the path is not well-formed.
        /// </summary>
        public PathItem(string pathStr, bool isFolder, bool isFromRecentlyCreated)
        {
            IsFolder = isFolder;
            FullPath = pathStr;
            IsFromRecentlyCreatedList = isFromRecentlyCreated;

            if (isFolder)
            {
                Folder = new DirectoryInfo(FullPath);
                ItemName = Folder.Name;
                ContainerName = Folder.Parent.FullName;
            }
            else
            {
                File = new FileInfo(FullPath);
                ItemName = Path.GetFileNameWithoutExtension(FullPath);
                ContainerName = File.DirectoryName;
            }

            Init();

            // Do not reference path.Exists, path.LastModified, or other properties
            // that depend on the actual existence of the file/folder, because it
            // may take a long time if the file/folder is on a remote share or is unreachable.
            // It will be done by a worker thread.
        }

        /// <summary>
        /// This ctor is for a local file/folder.  It will throw an exception if
        /// the path is not well-formed.
        /// </summary>
        public PathItem(ViewedPath viewedPath, bool isFolder)
            : this(viewedPath.Path, isFolder, false)
        {
            ViewTime = viewedPath.ViewTime;

            // Do not reference path.Exists, path.LastModified, or other properties
            // that depend on the actual existence of the file/folder, because it
            // may take a long time if the file/folder is on a remote share or is unreachable.
            // It will be done by a worker thread.
        }

        /// <summary>
        /// This ctor is for a REMOTE file/folder.
        /// </summary>
        public PathItem(TXFileInfo remoteTXInfo, ViewedPath viewedPath = null)
        {
            IsFolder = remoteTXInfo.IsFolder;
            FullPath = remoteTXInfo.FullPath;
            IsFromRecentlyCreatedList = remoteTXInfo.FoundInRecentFiles;

            if (IsFolder)
            {
                ItemName = Path.GetFileName(remoteTXInfo.FullPath);
                ContainerName = Path.GetDirectoryName(remoteTXInfo.FullPath);
            }
            else
            {
                ItemName = Path.GetFileNameWithoutExtension(remoteTXInfo.FullPath);
                ContainerName = Path.GetDirectoryName(remoteTXInfo.FullPath);
            }

            WriteTime = remoteTXInfo.LastModified;
            CreateTime = remoteTXInfo.Created;
            Size = remoteTXInfo.Size;

            if (viewedPath != null)
            {
                ViewTime = viewedPath.ViewTime;
            }

            Init();
        }

        private void Init()
        {
            if (IsFolder)
            {
                IsTracerXPath =
                    FullPath.EndsWith("\\TracerX\\ViewerLogs", StringComparison.OrdinalIgnoreCase) ||
                    FullPath.EndsWith("\\TracerX\\ServiceLogs", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                IsTracerXPath =
                    ContainerName.EndsWith("\\TracerX\\ViewerLogs", StringComparison.OrdinalIgnoreCase) ||
                    ContainerName.EndsWith("\\TracerX\\ServiceLogs", StringComparison.OrdinalIgnoreCase);

                // This is an archive file if the file name ends in "_num" where num is 
                // one or more numeric chars and at least one of them is non-zero.
                // Start at the end of the name search backwards for '_' or the first non-number.

                bool foundNonZero = false;

                for (int i = ItemName.Length - 1; i > 0; --i)
                {
                    char c = ItemName[i];

                    if (c == '_')
                    {
                        IsArchive = foundNonZero;
                        break;
                    }
                    else if (c < '0' || c > '9')
                    {
                        // We found a non-numeric char before finding '_' so this 
                        // can't be an archive file.  Leave IsArchive == false;
                        break;
                    }
                    else if (c != '0')
                    {
                        foundNonZero = true;
                    }
                }
            }
        }

        // Given a name like LogName_n, LogName_nnnn, or just plain LogName, this returns LogName.
        public string GetBaseName()
        {
            string result = ItemName;

            // Start with the last char and scan backwards.
            // If the first or last char in ItemName is '_' we'll return ItemName as is.

            for (int i = ItemName.Length - 1; i > 0; --i)
            {
                char c = ItemName[i];

                if (c == '_')
                {
                    if (i == ItemName.Length - 1)
                    {
                        // The '_' is at the very end so it's part of the base name.
                    }
                    else
                    {
                        result = ItemName.Remove(i);
                    }

                    break;
                }
                else if (c < '0' || c > '9')
                {
                    // We found a non-numeric char before finding '_' so this 
                    // can't be an archive or "*_00" file.
                    break;
                }
            }

            return result;
        }

        public string FullPath;
        public bool IsFolder;
        public bool IsTracerXPath;
        public bool IsArchive;
        public bool IsFromRecentlyCreatedList;
        public string ItemName;
        public string ContainerName;
        public DateTime WriteTime = DateTime.MinValue;
        public DateTime CreateTime = DateTime.MinValue;
        public DateTime ViewTime = DateTime.MinValue;
        public long Size = 0;
        public FileInfo File;
        public DirectoryInfo Folder;

        // If true we know the file doesn't exist.  If false we haven't checked yet.
        public bool IsMissing;

        // GridRow is null unless this PathItem is displayed on a PathControl.
        public PathGridRow GridRow;

        private object _refreshLock = new object();

        public void StartCheck()
        {
            if (File == null && Folder == null)
            {
                throw new Exception("Can't check a PathItem for a remote path.");
            }
            else
            {
                ThreadPool.QueueUserWorkItem(CheckPath);
            }
        }

        // This filters out any invalid paths in the array.
        public static List<PathItem> MakePathItems(IEnumerable<ViewedPath> paths, bool pathsAreFolders)
        {
            List<PathItem> result = null;

            if (paths != null)
            {
                result = new List<PathItem>();

                foreach (ViewedPath viewedPath in paths)
                {
                    try
                    {
                        result.Add(new PathItem(viewedPath, pathsAreFolders));
                    }
                    catch (Exception ex)
                    {
                        // Not a well-formed path.
                        Debug.Write(ex);
                    }
                }
            }

            return result;
        }

        // This filters out any invalid paths in the array.
        public static List<PathItem> MakeRecentlyCreatedPathItems(IEnumerable<string> paths, bool pathsAreFolders)
        {
            List<PathItem> result = null;

            if (paths != null)
            {
                result = new List<PathItem>();

                foreach (string pathStr in paths)
                {
                    try
                    {
                        result.Add(new PathItem(pathStr, pathsAreFolders, true));
                    }
                    catch (Exception ex)
                    {
                        // Not a well-formed path.
                        Debug.Write(ex);
                    }
                }
            }

            return result;
        }

        public override string ToString()
        {
            return FullPath;
        }

        // Should be called in a worker thread.
        private void CheckPath(object notused)
        {
            if (Monitor.TryEnter(_refreshLock))
            {
                // Checking if the file/folder exists and getting it's properties could take a long time to
                // work (or time out) if it's on a remote server/share.

                try
                {
                    if (IsFolder)
                    {
                        Folder.Refresh();

                        if (Folder.Exists)
                        {
                            IsMissing = false;
                            Size = Folder.GetFiles("*.tx1").Length;
                            WriteTime = Folder.LastWriteTime;
                        }
                        else
                        {
                            IsMissing = true;
                            Size = 0;
                            WriteTime = DateTime.MinValue;
                        }
                    }
                    else
                    {
                        File.Refresh();

                        if (File.Exists)
                        {
                            IsMissing = false;
                            Size = File.Length;
                            WriteTime = File.LastWriteTime;
                            CreateTime = File.CreationTime;
                        }
                        else
                        {
                            IsMissing = true;
                            Size = 0;
                            WriteTime = DateTime.MinValue;
                            CreateTime = DateTime.MinValue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print("{0}", ex);
                }
                finally
                {
                    Monitor.Exit(_refreshLock);
                }
            }
            else
            {
                // This method is already running in another thread and we don't need to run.
            }
        }


        #region IComparable<PathItem> Members

        public int CompareTo(PathItem other)
        {
            return string.Compare(this.FullPath, other.FullPath, true);
        }

        #endregion

        #region IEquatable<PathItem> Members

        public bool Equals(PathItem other)
        {
            return this.IsFolder == other.IsFolder && this.FullPath.Equals(other.FullPath, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
