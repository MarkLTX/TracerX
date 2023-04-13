using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace TracerX
{
    /// <summary>
    /// Methods for updating the two files (RecentlyCreated.txt and RecentFolders.txt)
    /// that are updated whenever a binary log file is opened.  The viewer app reads these
    /// files to populate the "Startup Screen".
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RecentlyCreated
    {
        // Directory used by TracerX for its data (e.g. the file containing the list of recently created files).
        public static readonly string TracerXDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TracerX");

        // Name of history file containing a list of folders where log files have been created.
        // The file is updated by TracerX-Logger.dll whenever it opens a log file.
        public static readonly string RecentFoldersFile = Path.Combine(TracerXDir, "RecentFolders.txt");

        // Name of history file containing a list of the most recently created log files.
        // The file is updated by TracerX-Logger.dll whenever it opens a log file.
        public static readonly string RecentFilesFile = Path.Combine(TracerXDir, "RecentlyCreated.txt");

        public static void GetFilesAndFolders(Logger Log, out string[] recentFileNames, out string[] recentFolderNames)
        {
            // Read both files under the mutex.

            using (new NamedMutexWait(NamedMutexWait.DataDirMUtexName, timeoutMs: 5000, throwOnTimeout: false))
            {
                if (File.Exists(RecentFilesFile))
                {
                    recentFileNames = File.ReadAllLines(RecentFilesFile);
                }
                else
                {
                    Log.Warn("The 'recent files' file was not found: ", RecentFilesFile);
                    recentFileNames = null;
                }

                if (File.Exists(RecentFoldersFile))
                {
                    recentFolderNames = File.ReadAllLines(RecentFoldersFile);
                }
                else
                {
                    Log.Warn("The 'recent folders' file was not found: ", RecentFoldersFile);
                    recentFolderNames = null;
                }
            }
        }

        // Adds the log file's full path to the list of files persisted for the viewer to read.
        // Adds the log file's folder to the list of folders persisted for the viewer to read.
        // Typically called in a worker thread.
        public static void AddToRecentlyCreated(string logFilePath)
        {
            // Note the reason for having a list of folders in addition to a list of files...
            // - Earlier versions of the logger truncated the list of files to a length of 10 (or was it 8?).
            //   If all those files are in the same folder, and the viewer gets its list of folders
            //   from the list of files, the viewer will know of only one folder.

            bool didAcquireMutex = false;

            try
            {
                Directory.CreateDirectory(TracerXDir);

                // Try to prevent multiple processes from updating the files concurrently,
                // but update the files even if we don't acquire the mutex in 10 seconds.

                using (var mutexWaiter = new NamedMutexWait(NamedMutexWait.DataDirMUtexName, timeoutMs: 10000, throwOnTimeout: false))
                {
                    // Update the recent folders file first, then the recent files file.  This way, the viewer
                    // can just watch the files file, then read both files when the files file changes.

                    didAcquireMutex = mutexWaiter.DidAcquire;

                    PrependToFile(RecentFoldersFile, Path.GetDirectoryName(logFilePath));
                    GrantPermissionsOnFile(RecentFoldersFile);

                    PrependToFile(RecentFilesFile, logFilePath);
                    GrantPermissionsOnFile(RecentFilesFile);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("Exception in AddToRecentlyCreated: " + ex);
                if (didAcquireMutex)
                {
                    Logger.EventLogging.Log("Exception in AddToRecentlyCreated (mutex was acquired): " + ex, Logger.EventLogging.NonFatalExceptionInLogger);
                }
                else
                {
                    Logger.EventLogging.Log("Exception in AddToRecentlyCreated (mutex was not acquired): " + ex, Logger.EventLogging.NonFatalExceptionInLogger);
                }
            }
        }

        private static void PrependToFile(string filePath, string lineOfText)
        {
            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(lineOfText))
            {
                for (int attemptNum = 0; attemptNum < 5; ++attemptNum)
                {
                    try
                    {
                        string[] curLines;

                        // First read the existing lines in the file.

                        try
                        {
                            curLines = File.ReadAllLines(filePath);
                        }
                        catch (FileNotFoundException)
                        {
                            // OK, We'll create it in a bit.
                            curLines = null;
                        }

                        if (curLines == null || !curLines.Any())
                        {
                            // No lines read, so just create the file with the current
                            // lineOfText as its only content.
                            File.WriteAllText(filePath, lineOfText);
                        }
                        else
                        {
                            // Check if lineOfText is already first.

                            if (!lineOfText.Equals(curLines[0], StringComparison.OrdinalIgnoreCase))
                            {
                                // Overwrite the file, putting lineOfText at the top.
                                using (StreamWriter writer = new StreamWriter(filePath, false))
                                {
                                    writer.WriteLine(lineOfText);

                                    // Write up to 999 of the previously read lines, omitting
                                    // any that match the line we just wrote (i.e. prevent duplicates).
                                    // Thus, the file will contain at most 1000 lines.
                                    int i = 0;
                                    foreach (string line in curLines)
                                    {
                                        if (!string.IsNullOrEmpty(line) && !lineOfText.Equals(line, StringComparison.OrdinalIgnoreCase))
                                        {
                                            writer.WriteLine(line);
                                            if (i++ == 999) break;
                                        }
                                    }
                                }
                            }
                        }

                        break; // Break out of retry loop.
                    }
                    catch (IOException ex)
                    {
                        // Common to get "The requested operation cannot be performed on a file with a user-mapped section open" despite
                        // holding the named mutex.  Google search indicates possible bug in the OS or .NET regarding File IO.  Try waiting
                        // to give the other process a chance to close the file, then retry.
                        Thread.Sleep(20);
                    }
                }
            }
        }

        // Grants all users full control over the
        // file so any user can update it.
        private static void GrantPermissionsOnFile(string filePath)
        {
            var sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);

#if NET35 || NET45 || NET46 
            FileSecurity fSecurity = File.GetAccessControl(filePath);
            fSecurity.AddAccessRule(new FileSystemAccessRule(sid, FileSystemRights.FullControl, AccessControlType.Allow));
            File.SetAccessControl(filePath, fSecurity);
#elif NETCOREAPP3_1
            FileInfo fi = new FileInfo(filePath);
            FileSecurity fSecurity = fi.GetAccessControl();
            fSecurity.AddAccessRule(new FileSystemAccessRule(sid, FileSystemRights.FullControl, AccessControlType.Allow));
            fi.SetAccessControl(fSecurity);
#endif

        }
    }
}
