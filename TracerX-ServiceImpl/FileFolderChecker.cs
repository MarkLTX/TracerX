using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TracerX
{
    // Class for checking if files and folders exist and combining the results into one list.

    internal class FileFolderChecker
    {
        // This result contains all the EXISTING file and folders
        // processed by ProcessFiles() and ProcessFolders().
        public readonly List<TXFileInfo> CombinedResult = new List<TXFileInfo>();

        // The HashSets help us to avoid checking the same files and folders more than once.
        private readonly HashSet<string> testedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> testedFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public void CheckFolders(IEnumerable<string> folders, bool fromRecentFiles)
        {
            foreach (string folderName in folders)
            {
                if (testedFolders.Add(folderName))
                {
                    // This is a folder we haven't tested before.
                    // Test its existence and get its properties.

                    try
                    {
                        var dirInfo = new DirectoryInfo(folderName);

                        if (dirInfo.Exists)
                        {
                            CombinedResult.Add(new TXFileInfo()
                            {
                                IsFolder = true,
                                FullPath = folderName,
                                Created = dirInfo.CreationTime,
                                LastModified = dirInfo.LastWriteTime,
                                Size = dirInfo.GetFiles("*.tx1").Length, // TODO: Look for faster/better way to count files.
                                FoundInRecentFiles = fromRecentFiles,
                            });
                        }
                    } 
                    catch (Exception ex)
                    {
                        // Have seen this occur due to garbage (invalid chars in path) in the RecentFolders.txt file.
                        Logger.Current.Warn("Exception checking existence of folder, ex: ", ex);
                        Logger.Current.Info("Folder path that caused the exception: ", folderName);
                    }
                }
            }
        }

        public void CheckFiles(IEnumerable<string> files, bool fromRecentFiles)
        {
            foreach (string fileName in files)
            {
                // Ensure we only process .tx1 files for security.

                if (fileName.EndsWith(".tx1", StringComparison.OrdinalIgnoreCase))
                {
                    // Process both folder and full file path.

                    if (testedFiles.Add(fileName))
                    {
                        // This is a file we haven't tested before.

                        try
                        {
                            var fileInfo = new FileInfo(fileName);

                            if (fileInfo.Exists)
                            {
                                CombinedResult.Add(new TXFileInfo()
                                {
                                    IsFolder = false,
                                    FullPath = fileName,
                                    Created = fileInfo.CreationTime,
                                    LastModified = fileInfo.LastWriteTime,
                                    Size = fileInfo.Length,
                                    FoundInRecentFiles = fromRecentFiles,
                                });
                            }

                            if (fromRecentFiles && testedFolders.Add(fileInfo.DirectoryName))
                            {
                                // This is a folder we haven't tested before.
                                // Test its existence and get its properties.

                                if (fileInfo.Directory.Exists)
                                {
                                    CombinedResult.Add(new TXFileInfo()
                                    {
                                        IsFolder = true,
                                        FullPath = fileInfo.DirectoryName,
                                        Created = fileInfo.Directory.CreationTime,
                                        LastModified = fileInfo.Directory.LastWriteTime,
                                        Size = fileInfo.Directory.GetFiles("*.tx1").Length, // TODO: Look for faster/better way to count files.
                                        FoundInRecentFiles = true,
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Have seen this occur due to garbage (invalid chars in path) in the RecentlyCreated.txt file.
                            Logger.Current.Warn("Exception checking existence of folder, ex: ", ex);
                            Logger.Current.Info("Folder path that caused the exception: ", fileName);
                        }
                    } // if not already tested
                } // if .tx1
            }
        }
    }
}
