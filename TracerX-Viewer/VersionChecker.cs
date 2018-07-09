using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using TracerX.Properties;
using System.Threading;
using System.ComponentModel;
using System.IO;

namespace TracerX
{
    // This class looks for a newer version of the TracerX binaries on GitHub using a
    // worker thread to get the HTML for the TracerX releases page.
    // It would be better to use the GitHub API but that would require
    // adding a dependency to a NuGet package like Octokit, I think.
    internal static class VersionChecker
    {
        /// <summary>
        /// Event raised on the GUI thread if the worker thread started by CheckForNewVersion() finds a new version.
        /// </summary>
        public static event EventHandler NewVersionFound;

        const string _url = "http://github.com/MarkLTX/TracerX/releases";

        /// <summary>
        /// Starts a worker thread that checks if a new version of the TracerX binaries is available on GitHub.
        /// May lead to NewVersionFound being raised on the GUI thread.
        /// </summary>
        public static void CheckForNewVersion()
        {
            if (Settings.Default.VersionCheckingAllowed &&
                Settings.Default.VersionLastChecked.Date.AddDays(Settings.Default.VersionCheckInterval) < DateTime.Now) 
            {
                // Download the TracerX releases webpage in a worker thread.
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                bw.RunWorkerAsync();
            }
        }

        static void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Download the TracerX releases webpage from github.com.
                // Find the strings that look like release numbers and compare the
                // "largest" release found to the version number of the current executable. 
                // Each release should have a link like this.
                // <a href="/MarkLTX/TracerX/releases/tag/v7.1">TracerX 7.1</a>

                // These two statements prevent an exception with this error message: "The request was aborted: Could not create SSL/TLS secure channel."
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // Added to .NET Framework 4.5

                WebClient client = new WebClient();
                string webPage = client.DownloadString(_url);
                string key = "/MarkLTX/TracerX/releases/tag/v";
                int pos = webPage.IndexOf(key);

                // Assume the latest release is listed first, which is the only one needed.

                if (pos != -1)
                {
                    // Get the next substring of characters that are numbers and periods and attempt to parse it into a Version.

                    string sVer = null;
                    pos += key.Length;

                    for (int pos2 = pos + 1; pos2 < webPage.Length; ++pos2)
                    {
                        char c = webPage[pos2];

                        if ((c < '0' || c > '9') && c != '.')
                        {
                            // We found the end of the version string.
                            sVer = webPage.Substring(pos, pos2 - pos);
                            break;
                        }
                    }

                    if (sVer != null)
                    {
                        Version newestVer = new Version(sVer);

                        if (newestVer > Assembly.GetExecutingAssembly().GetName().Version)
                        {
                            e.Result = true;
                            //break;
                        }
                    }

                    //pos = webPage.IndexOf(key, pos);
                }
            }
            catch (Exception ex)
            {
                Debug.Print("{0}", ex);
            }
        }

        static void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Settings.Default.VersionLastChecked = DateTime.Now;

                if (e.Error == null)
                {

                    if (!e.Cancelled && e.Result != null && (bool)e.Result)
                    {
                        if (NewVersionFound != null)
                        {
                            NewVersionFound(null, EventArgs.Empty);
                        }
                    }
                }
                else
                {
                    Debug.Print("{0}", e.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.Print("{0}", ex);
            }

            // We only call it once, so drop the reference to the event handler.
            NewVersionFound = null;
        }
    }
}
