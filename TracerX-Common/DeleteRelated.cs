using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace TracerX
{
    // Class used by viewer and service for cleaning/deleting log files.
    public static class DeleteRelated
    {
        public static List<string> DeleteMany(string fileSpec, string folderSpec, DateTime minTimestamp, DateTime maxTimestamp, long minSize, long maxSize, bool tx1Files, bool txtFiles, bool deleteEmptyFolderAndParents, bool listOnly, Logger Log, WindowsIdentity userToImpersonate = null)
        {
            using (Log.InfoCall())
            {
                var result = new List<string>();

                try
                {
                    Log.Info("fileSpec = ", fileSpec);
                    Log.Info("folderSpec = ", folderSpec);
                    Log.Info("minTimestamp = ", minTimestamp);
                    Log.Info("maxTimestamp = ", maxTimestamp);
                    Log.Info("minSize = ", minSize);
                    Log.Info("maxSize = ", maxSize);
                    Log.Info("tx1Files = ", tx1Files);
                    Log.Info("txtFiles = ", txtFiles);
                    Log.Info("deleteEmptyFolderAndParents = ", deleteEmptyFolderAndParents);
                    Log.Info("listOnly = ", listOnly);

                    // Create a list of all distinct folder names from the two history files.

                    IEnumerable<string> folderList = null;

                    // Check for the presence of wild-cards.

                    if (folderSpec.IndexOfAny(new char[] { '*', '?' }) == -1)
                    {
                        // No wild-cards so just use the folder name as-is even if it isn't listed in the history files.
                        folderList = new string[] { folderSpec };
                    }
                    else
                    {
                        // The wild-carded folder spec is applied to the list of 
                        // folders in the history files so start by getting that list.

                        folderList = GetHistoricalFolderNames(Log);

                        if (folderSpec != "*")
                        {
                            // Since we're not simply processing all known folders we'll use the
                            // StringMatcher class in Wildcard mode to find the matching folders. In
                            // wildcard syntax, the '\' char is an escape char. However there is no
                            // legitimate use for escape chars in the folder spec since the only
                            // escapable characters are '*', '?', and '\' which are not valid in
                            // folder (or file) names. Thus, any '\' chars present must be subdir
                            // separators which must be escaped to prevent StringMatcher from
                            // treating them as escape chars and discarding them.

                            folderSpec = folderSpec.Replace("\\", "\\\\");
                            var matcher = new StringMatcher(folderSpec, false, MatchType.Wildcard);
                            folderList = folderList.Where(f => matcher.Matches(f));
                        }
                    }

                    foreach (string folder in folderList)
                    {
                        // Search for .txt and/or .tx1 files matching the fileSpec and delete them.  If folder becomes empty, delete it.

                        DirectoryInfo dirInfo = null;

                        try
                        {
                            dirInfo = new DirectoryInfo(folder);
                        }
                        catch
                        {
                            // Invalid path.
                            dirInfo = null;
                        }

                        if (dirInfo != null && dirInfo.Exists)
                        {
                            List<FileInfo> filesInFolder = new List<FileInfo>();
                            bool leftSome = false;

                            if (tx1Files) filesInFolder.AddRange(dirInfo.GetFiles(fileSpec + ".tx1"));
                            if (txtFiles) filesInFolder.AddRange(dirInfo.GetFiles(fileSpec + ".txt"));

                            foreach (FileInfo file in filesInFolder)
                            {
                                // For compatibility with the old DOS 8.3 file name format,
                                // GetFiles("*.tx1") will return files whose extension STARTS WITH
                                // ".tx1", such as "x.tx123". To exclude such files, verify we have a
                                // 4 character extension (includes the '.') in addition to checking
                                // the other criteria.

                                if (file.Extension.Length == 4 &&
                                    file.LastWriteTime >= minTimestamp && file.LastWriteTime <= maxTimestamp &&
                                    file.Length >= minSize && file.Length <= maxSize)
                                {
                                    if (listOnly)
                                    {
                                        result.Add(file.FullName);
                                    }
                                    else
                                    {
                                        using (userToImpersonate?.Impersonate())
                                        {
                                            TryDeleteFile(Log, result, file);
                                        }
                                    }
                                }
                                else
                                {
                                    leftSome = true;
                                }
                            }

                            if (!listOnly && !leftSome)
                            {
                                using (userToImpersonate?.Impersonate())
                                {
                                    TryDeleteFolder(dirInfo, Log);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Exception in DeleteMany(): ", ex);
                    throw;
                }

                return result;
            }
        }

        /// <summary>
        /// Returns true if the candidateFile name matches the format "[baseLogName].tx1"
        /// or "[baseLogName]_[num].tx1" where [num] is one or more numeric digits.
        /// The candidateFile may be a full path or just "something.tx1".
        /// </summary>
        public static bool IsFileRelated(string baseLogName, string candidateFile)
        {
            bool result = false;

            if (Path.GetExtension(candidateFile).Equals(".tx1", StringComparison.OrdinalIgnoreCase))
            {
                string plainName = Path.GetFileNameWithoutExtension(candidateFile);

                if (plainName.Length > baseLogName.Length + 1)
                {
                    string numpart = plainName.Substring(baseLogName.Length + 1);
                    int num;

                    result = (int.TryParse(numpart, out num) && num >= 0);
                }
                else
                {
                    result = plainName.Equals(baseLogName, StringComparison.OrdinalIgnoreCase);
                }
            }

            return result;
        }

        /// <summary>
        /// Given a base log name like "LogFile", this attempts to delete all related TX1 files such
        /// as LogFile.tx1, LogFile_n.tx1, LogFile_nn.tx1 and so on (where the 'n' in "_nnn" is a
        /// numeric char). Optionally, it will also delete the folder if it becomes empty, and then
        /// the parent, grandparent and so on.  The toImpersonate parameter specifies an 
        /// optional WindowsIdentity to impersonate when deleting files.
        /// A list of the deleted files is returned.
        /// </summary>
        public static List<string> DeleteAllRelated(string folderPath, string baseLogName, Logger Log, bool deleteEmptyFolderAndParents, WindowsIdentity userToImpersonate = null)
        {
            using (Log.InfoCall())
            {
                var result = new List<string>();

                try
                {
                    Log.Info("folderPath = ", folderPath);
                    Log.Info("baseFileName = ", baseLogName);
                    Log.Info("deleteEmptyFolderAndParents = ", deleteEmptyFolderAndParents);

                    if (baseLogName.Contains('*') || baseLogName.Contains('?'))
                    {
                        throw new ArgumentException("Argument baseLogName may not contain wildcards ('*' or '?').");
                    }
                    else
                    {
                        var dirInfo = new DirectoryInfo(folderPath);

                        if (dirInfo.Exists)
                        {
                            FileInfo[] candidates = dirInfo.GetFiles(baseLogName + "_*.tx1");

                            using (userToImpersonate?.Impersonate())
                            {
                                foreach (FileInfo fi in candidates)
                                {
                                    // Make sure we only delete related files and not files like
                                    // baseLogName_NotANumber.tx1 or baseLogName_00_01.tx1.

                                    if (IsFileRelated(baseLogName, fi.Name))
                                    {
                                        TryDeleteFile(Log, result, fi);
                                    }
                                }
                            }

                            var baseFile = new FileInfo(Path.Combine(folderPath, baseLogName + ".tx1"));

                            // The baseFile won't exist if the user set Use_00 when opening
                            // the log.  Also, it's likely to be open by the logger.

                            if (baseFile.Exists)
                            {
                                using (userToImpersonate?.Impersonate())
                                {
                                    TryDeleteFile(Log, result, baseFile);
                                }
                            }

                            if (deleteEmptyFolderAndParents)
                            {
                                using (userToImpersonate?.Impersonate())
                                {
                                    TryDeleteFolder(dirInfo, Log);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Exception in DeleteAllRelated(): ", ex);
                    throw;
                }

                return result;
            }
        }

        private static HashSet<string> GetHistoricalFolderNames(Logger Log)
        {
            HashSet<string> uniqueFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Read both files under the mutex.

            using (new NamedMutexWait(NamedMutexWait.DataDirMUtexName, timeoutMs: 5000, throwOnTimeout: false))
            {
                if (File.Exists(RecentlyCreated.RecentFilesFile))
                {
                    foreach (string file in File.ReadAllLines(RecentlyCreated.RecentFilesFile))
                    {
                        // Use a try/catch in case any of the paths have invalid syntax.
                        try
                        {
                            uniqueFolders.Add(Path.GetDirectoryName(file));
                        }
                        catch (Exception)
                        { }
                    }
                }
                else
                {
                    Log.Warn("The 'recent files' file was not found: ", RecentlyCreated.RecentFilesFile);
                }

                if (File.Exists(RecentlyCreated.RecentFoldersFile))
                {
                    foreach (string folder in File.ReadAllLines(RecentlyCreated.RecentFoldersFile))
                    {
                        uniqueFolders.Add(folder);
                    }
                }
                else
                {
                    Log.Warn("The 'recent folders' file was not found: ", RecentlyCreated.RecentFoldersFile);
                }
            }

            return uniqueFolders;
        }

        private static void TryDeleteFile(Logger Log, List<string> result, FileInfo fi)
        {
            try
            {
                fi.Delete();
                result.Add(fi.FullName);
                Log.Debug("Deleted file ", fi.FullName);
            }
            catch (Exception ex)
            {
                Log.Info("Unable to delete file '", fi.FullName, "' because: ", ex.Message);
            }
        }

        private static void TryDeleteFolder(DirectoryInfo dirInfo, Logger Log)
        {
            // We're only supposed to delete empty folders. I'm not sure if
            // GetFileSystemInfos() will return files or folders we have no
            // permission to access so let's just try to delete it, and it's parents,
            // until an exception occurs.

            try
            {
                do
                {
                    dirInfo.Delete(false);
                    Log.Info("Deleted folder ", dirInfo.FullName);
                    dirInfo = dirInfo.Parent;
                } while (dirInfo != null);
            }
            catch (Exception ex)
            {
                // Probably the folder isn't empty, which is expected at some point, but log it anyway.
                Log.Warn("Exception deleting folder '", dirInfo.FullName, "': ", ex.Message);
            }
        }

    }
}
