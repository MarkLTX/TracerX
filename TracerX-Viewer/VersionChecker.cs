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
    // This class looks for a newer version of TracerX on GitHub using a
    // worker thread to get the HTML for the TracerX releases page.
    // It would be better to use the GitHub API but that would require
    // adding a dependency to a NuGet package like Octokit, I think.
    internal static class VersionChecker
    {
        public static event EventHandler NewVersionFound;

        const string _url = "http://github.com/MarkLTX/TracerX/releases";

        public static void CheckForNewVersion()
        {
            //if (Settings.Default.VersionCheckingAllowed &&
            //    Settings.Default.VersionLastChecked.Date.AddDays(Settings.Default.VersionCheckInterval) < DateTime.Now) 
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
                // Find the strings that look like release numbers an compare the
                // "largest" release found to the version number of the current executable. 
                // Each release should have a link like this.
                // <a href="/MarkLTX/TracerX/releases/tag/v7.1">TracerX 7.1</a>

                // These two statements prevent an exception with error message: The request was aborted: Could not create SSL/TLS secure channel.
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                WebClient client = new WebClient();
                string webPage = client.DownloadString(_url);
                string key = "/MarkLTX/TracerX/releases/tag/v";
                int pos = webPage.IndexOf(key);

                while (pos != -1)
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
                            break;
                        }
                    }

                    pos = webPage.IndexOf(key, pos);
                }
            }
            catch (Exception ex)
            {
                Debug.Print("{0}", ex);
            }
        }

        private static string DownloadUrl()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(_url);
            req.Method = "GET";

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Skip validation of SSL/TLS certificate
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            WebResponse respon = req.GetResponse();
            Stream res = respon.GetResponseStream();
            string ret = "";
            byte[] buffer = new byte[1048];
            int read = 0;

            while ((read = res.Read(buffer, 0, buffer.Length)) > 0)
            {
                //Console.Write(Encoding.ASCII.GetString(buffer, 0, read));
                ret += Encoding.ASCII.GetString(buffer, 0, read);
            }

            return ret;
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
