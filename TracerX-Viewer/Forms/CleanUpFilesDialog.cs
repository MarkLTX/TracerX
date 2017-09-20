using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TracerX
{
    internal partial class CleanUpFilesDialog : Form
    {
        // The remoteServer will be null for localhost.
        public CleanUpFilesDialog(RemoteServer remoteServer, string defaultFolderSpec)
        {
            InitializeComponent();
            _server = remoteServer;
            txtFolderSpec.Text = defaultFolderSpec;
            this.Icon = Properties.Resources.scroll_view;

            // Set to the "to time" to now truncated to the next minute.
            DateTime now = DateTime.Now;
            dtpToTime.Value = DateTime.Now.Date.AddHours(now.Hour).AddMinutes(now.Minute + 1);
        }

        private static Logger Log = Logger.GetLogger("CleanupFilesDialog");
        private  RemoteServer _server;
        
        public bool DidDeleteFiles
        {
            get;
            private set;
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                Go(true);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                Go(false);
            }
        }

        // Returns true if any files were deleted or listed.
        private void Go(bool listFiles)
        {
            try
            {
                List<string> files = null;

                if (_server == null)
                {
                    // Process local files directly.

                    if (listFiles || DialogResult.Yes == MainForm.ShowMessageBoxBtns("Delete specified files?", MessageBoxButtons.YesNo))
                    {
                        files = DeleteRelated.DeleteMany(
                            txtFileSpec.Text,
                            txtFolderSpec.Text,
                            dtpFromTime.Value,
                            dtpToTime.Value,
                            int.Parse(txtFromSize.Text) * 1024,
                            int.Parse(txtToSize.Text) * 1024,
                            chkBinary.Checked,
                            chkText.Checked,
                            chkDeleteFolders.Checked,
                            listFiles,
                            Log);
                    }
                }
                else
                {
                    // Process remote files via the WCF server.
                    using (ProxyFileEnum serviceProxy = new ProxyFileEnum())
                    {
                        // Need to use host:port and credentials.
                        serviceProxy.SetHost(_server.HostAndPort);
                        serviceProxy.SetCredentials(_server.GetCreds());
                        int serverVersion = serviceProxy.ExchangeVersion(1);

                        if (serverVersion >= 4)
                        {
                            // This version of the server has a method for deleting all the
                            // matching files and parent folder(s) in one call, so display the
                            // appropriate message and call that method.

                            if (listFiles || DialogResult.Yes == MainForm.ShowMessageBoxBtns("Delete specified files?", MessageBoxButtons.YesNo))
                            {
                                files = serviceProxy.DeleteMany(
                                    txtFileSpec.Text,
                                    txtFolderSpec.Text,
                                    dtpFromTime.Value,
                                    dtpToTime.Value,
                                    int.Parse(txtFromSize.Text) * 1024,
                                    int.Parse(txtToSize.Text) * 1024,
                                    chkBinary.Checked,
                                    chkText.Checked,
                                    chkDeleteFolders.Checked,
                                    listFiles);
                            }
                        }
                        else
                        {
                            string msg = "Sorry, the TracerX-Service you're connected to is an old version that doesn't support this feature.";
                            MainForm.ShowMessageBox(msg);
                        }
                    }
                }

                if (files != null)
                {
                    Log.Info(files.Count, " files were returned by DeleteMany().");

                    if (files.Any())
                    {
                        if (listFiles)
                        {
                            // Display the files.
                            var dlg = new FullText(string.Join("\r\n", files), false, true);
                            dlg.Height = 500;
                            dlg.Text = "Matching Log Files (" + files.Count + ")";
                            dlg.ShowDialog();
                        }
                        else
                        {
                            MainForm.ShowMessageBox(files.Count.ToString() +  " files were deleted.");
                            DidDeleteFiles = true;
                        }
                    }
                    else
                    {
                        MainForm.ShowMessageBox("No files matched the specified criteria.");
                    }
                }
            }
            catch (Exception ex)
            {
                MainForm.ShowMessageBox("An error occurred in the Clean Up Files dialog.\n\n" + ex.ToString());
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool ValidateInputs()
        {
            bool result = true;
            long fromSize;
            long toSize;
            long maxKB = long.MaxValue / 1024;

            txtFileSpec.Text = txtFileSpec.Text.Trim();
            txtFolderSpec.Text = txtFolderSpec.Text.Trim();

            if (txtFileSpec.Text == "")
            {
                MainForm.ShowMessageBox("File spec cannot be empty.");
                result = false;
            }
            else if (!chkBinary.Checked && !chkText.Checked)
            {
                MainForm.ShowMessageBox("Please select (check) binary and/or text files.");
                result = false;
            }
            else if (txtFolderSpec.Text == "")
            {
                MainForm.ShowMessageBox("Folder spec cannot be empty.");
                result = false;
            }
            else if (dtpToTime.Value <= dtpFromTime.Value)
            {
                MainForm.ShowMessageBox("The 'to time' must be greater than the 'from time'.");
                result = false;
            }
            else if (!long.TryParse(txtFromSize.Text, out fromSize) || fromSize < 0 || fromSize > maxKB)
            {
                MainForm.ShowMessageBox("The 'from size' must be an integer between 0 and " + maxKB);
                result = false;
            }
            else if (!long.TryParse(txtToSize.Text, out toSize) || toSize < 1 || toSize > maxKB)
            {
                MainForm.ShowMessageBox("The 'to size' must be an integer between 0 and " + maxKB);
                result = false;
            }
            else if (toSize <= fromSize)
            {
                MainForm.ShowMessageBox("The 'to size' must be greater than the 'from size'.");
                result = false;
            }
            return result;
        }
    }
}
