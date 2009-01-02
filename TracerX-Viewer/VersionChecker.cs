using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using TracerX.Properties;

namespace TracerX.Viewer {
    // This class looks for a newer version of TracerX on the CodePlex website using a
    // worker thread to get the HTML for the TracerX page.
    internal static class VersionChecker {
        const string _url = "http://www.codeplex.com/TracerX";

        public static void CheckForNewVersion() {
            if (Settings.Default.VersionCheckingAllowed &&
                Settings.Default.VersionLastChecked.Date.AddDays(Settings.Default.VersionCheckInterval) < DateTime.Now) //
            {
                WebClient client = new WebClient();
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
                client.DownloadStringAsync(new Uri(_url));
                Settings.Default.VersionLastChecked = DateTime.Now;
            }
        }

        // Find the "Latest viewer version" number in the downloaded HTML, 
        // compare it to this assembly's version number, and tell the user 
        // if a new version is available. This requires manually updating 
        // the version number following the text "Latest viewer version: " 
        // on the TracerX home page.
        private static void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e) {
            // example: "Latest viewer version: 2.1.0809.18205<"
            try {
                if (e.Error == null && !e.Cancelled) {
                    string key = "Latest viewer version: ";
                    int pos = e.Result.IndexOf(key);
                    if (pos != -1) {
                        string sVer = null;

                        pos += key.Length;
                        for (int pos2 = pos + 1; pos2 < e.Result.Length; ++pos2) {
                            char c = e.Result[pos2];
                            if ((c < '0' || c > '9') && c != '.') {
                                // We found the end of the version string.
                                sVer = e.Result.Substring(pos, pos2 - pos);
                                break;
                            }
                        }

                        if (sVer != null) {
                            Version newestVer = new Version(sVer);
                            if (newestVer > Assembly.GetExecutingAssembly().GetName().Version) {
                                string msg = string.Format(
                                    "A newer version of TracerX is available at {0}.\n\n" +
                                    "Yes = View the TracerX website.\n" +
                                    "No = Edit the version checking settings.\n" +
                                    "Cancel = Continue using the viewer.",
                                    _url);
                                DialogResult dr = MessageBox.Show(MainForm.TheMainForm, msg, "TracerX", MessageBoxButtons.YesNoCancel);

                                switch (dr) {
                                    case DialogResult.Yes:
                                        Process.Start(_url);
                                        break;
                                    case DialogResult.No:
                                        OptionsDialog dlg = new OptionsDialog();
                                        dlg.tabControl1.SelectedTab = dlg.versionPage;
                                        dlg.ShowDialog();
                                        break;
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                //MessageBox.Show(ex.ToString());
            }
        }
    }
}
