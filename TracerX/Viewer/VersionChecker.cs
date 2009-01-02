using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace TracerX.Viewer {
    // This class looks for a newer version of TracerX on the CodePlex website using a
    // worker thread to get the HTML for the TracerX page.
    internal static class VersionChecker {
        const string _url = "http://www.codeplex.com/TracerX";

        public static void CheckForNewVersion() {
            if (Settings1.Default.VersionLastChecked.Date.AddDays(Settings1.Default.VersionCheckInterval) < DateTime.Now) {
                WebClient client = new WebClient();
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
                client.DownloadStringAsync(new Uri(_url));
                Settings1.Default.VersionLastChecked = DateTime.Now;
            }
        }

        // Find the "Current Release" number in the downloaded HTML, compare it to this assembly's
        // version number, and tell the user if a new version is available.
        private static void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e) {
            try {
                if (e.Error == null && !e.Cancelled) {
                    int pos = e.Result.IndexOf(">Current Release<");
                    int pos2 = -1;
                    if (pos != -1) pos = e.Result.IndexOf("ReleaseId=", pos);
                    if (pos != -1) pos = e.Result.IndexOf('>', pos);
                    if (pos != -1) pos2 = e.Result.IndexOfAny(new char[] { ' ', '<' }, pos);
                    if (pos2 != -1) {
                        string sVer = e.Result.Substring(pos + 1, pos2 - pos - 1);
                        Version newestVer = new Version(sVer);
                        if (newestVer > Assembly.GetExecutingAssembly().GetName().Version) {
                            string msg = string.Format(
                                "A newer version of TracerX is available at {0}.\n\n" +
                                "Yes = View the TracerX website.\n" +
                                "No = Edit the version checking settings.\n" +
                                "Cancel = Do nothing.",
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
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
