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

namespace TracerX.Viewer
{
    // This class looks for a newer version of TracerX on the CodePlex website using a
    // worker thread to get the HTML for the TracerX page.
    internal static class VersionChecker
    {
        public static event EventHandler NewVersionFound;

        const string _url = "http://tracerx.codeplex.com/documentation";

        public static void CheckForNewVersion()
        {
            if (Settings.Default.VersionCheckingAllowed &&
                Settings.Default.VersionLastChecked.Date.AddDays(Settings.Default.VersionCheckInterval) < DateTime.Now) //
            {
                // Download the TracerX webpage in a worker thread.
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                bw.RunWorkerAsync();
            }
        }

        static void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            // Download the TracerX webpage from Codeplex.com.
            // Find the "Latest viewer version" number in the downloaded HTML, 
            // compare it to this assembly's version number, and tell the user 
            // if a new version is available. This requires manually updating 
            // the version number following the text "Latest viewer version: " 
            // on the _url page.

            WebClient client = new WebClient();
            string webPage = client.DownloadString(_url);
            string key = "Latest viewer version: ";

            // example: "Latest viewer version: 2.1.0809.18205<"
            int pos = webPage.IndexOf(key);

            if (pos != -1)
            {
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
                    }
                }
            }
        }

        static void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Settings.Default.VersionLastChecked = DateTime.Now;

                if (e.Error == null && !e.Cancelled && e.Result != null && (bool)e.Result)
                {
                    //DisplayMessage();
                    if (NewVersionFound != null)
                    {
                        NewVersionFound(null, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }

            // We only call it once, so drop the reference.
            NewVersionFound = null;
        }
    }
}
