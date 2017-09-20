using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using TracerX.ExtensionMethods;

namespace TracerX
{
    internal partial class ServerListEditor : Form
    {
        public ServerListEditor(List<RemoteServer> remoteServers)
        {
            InitializeComponent();
            _inputServers = remoteServers;

            foreach (RemoteServer originalObject in _inputServers)
            {
                // The user's edits will be kept in the editable objects (keys of _newToOld)
                // until he clicks OK, then the properties of each editableObject will be
                // copied to the corresponding originalObject.

                SavedServer editableObject = originalObject.MakeSavedServer();
                _newToOld[editableObject] = originalObject;
                AddServerItem(editableObject);
            }

            _sorter = new ListViewSorter(listView1);
            _sorter.Sort(colDispName.Index);
        }

        /// <summary>
        /// Gets the edited servers.
        /// </summary>
        public List<RemoteServer> NewServers
        {
            get;
            private set;
        }

        private List<RemoteServer> _inputServers;
        private Dictionary<SavedServer, RemoteServer> _newToOld = new Dictionary<SavedServer, RemoteServer>();
        private ListViewSorter _sorter;
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            NewServers = new List<RemoteServer>();

            foreach (ServerListItem item in listView1.Items)
            {
                // The item may correspond to a new (added) server,
                // or to an old (edited) server.

                RemoteServer oldObj;

                if (_newToOld.TryGetValue(item.Server, out oldObj))
                {
                    // Update the old server with the possibly changed properties.

                    oldObj.HostName = item.Server.HostName;
                    oldObj.HostAddress = item.Server.HostAddress;
                    oldObj.Port = item.Server.Port;
                    oldObj.Category = item.Server.Category;
                    oldObj.UserId = item.Server.UserId;
                    oldObj.PW = item.Server.PW;
                }
                else
                {
                    // The item must represent a new (added) server, so create one.

                    oldObj = new RemoteServer(item.Server);
                }

                NewServers.Add(oldObj);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        // Gets an array of unique category names.
        private string[] GetUniqueCategories()
        {
            var cats = new HashSet<string>();

            foreach (ServerListItem item in listView1.Items)
            {
                string cat = item.Server.Category ;
                if (!string.IsNullOrWhiteSpace(cat)) cats.Add(cat);
            }

            return cats.ToArray();
        }

        // Returns a HashSet of all the server display names except the specified one.
        private HashSet<string> GetDisplayNames(ServerListItem exceptThisOne)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (ServerListItem item in listView1.Items)
            {
                result.Add(item.Server.HostName);
            }

            if (exceptThisOne != null)
            {
                result.Remove(exceptThisOne.Server.HostName);
            }

            return result;
        }

        // Sets the credentials used for multiple servers.
        private void setCredentialsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SavedServer firstSelectedServer = (listView1.SelectedItems[0] as ServerListItem).Server;
            var credDlg = new CredentialsDialog();

            credDlg.UserID = firstSelectedServer.UserId;
            credDlg.PW = firstSelectedServer.PW;

            if (credDlg.ShowDialog() == DialogResult.OK)
            {
                foreach (ServerListItem item in listView1.SelectedItems)
                {
                    item.Server.UserId = credDlg.UserID;

                    if (credDlg.UserID == "")
                    {
                        item.Server.PW = null;
                    }
                    else
                    {
                        item.Server.PW = credDlg.PW;
                    }

                    item.SubItems[colUserID.Index].Text = item.Server.UserId;
                }

                btnOK.Enabled = true;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfirmAndRemoveSelectedServers();
        }

        private void ConfirmAndRemoveSelectedServers()
        {
            // Confirm that user wants to delete the selected servers.

            if (DialogResult.Yes == MainForm.ShowMessageBoxBtns("Remove " + listView1.SelectedItems.Count + " selected servers?", MessageBoxButtons.YesNo))
                {
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    listView1.Items.Remove(item);
                }

                btnOK.Enabled = true;
            }
        }

        // Copies hostnames to clipboard.
        private void exportServersMenuItem_Click(object sender, EventArgs e)
        {
            // Save the selected servers to an XML file.

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.DefaultExt = ".xml";
            dlg.OverwritePrompt = true;
            dlg.CheckPathExists = false;
            dlg.Filter = "XML files|*.xml|All files|*.*";
            dlg.FilterIndex = 0;
            //dlg.InitialDirectory = fileBox.Text;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                var exportList = new List<ExportableServer>(listView1.SelectedItems.Count);

                foreach (ServerListItem item in listView1.SelectedItems)
                {
                    exportList.Add(
                        new ExportableServer()
                        {
                            HostName = item.Server.HostName,
                            HostAddress = item.Server.HostAddress,
                            Port = item.Server.Port,
                            Category = item.Server.Category,
                        }
                        );
                }

                using (var stream = dlg.OpenFile())
                {
                    XmlSerializer serializer = new XmlSerializer(exportList.GetType());
                    serializer.Serialize(stream, exportList);
                }

                string msg = string.Format("{0} servers saved to {1}", exportList.Count, dlg.FileName);
                MainForm.ShowMessageBox(msg);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog()
            {
                AddExtension = true,
                CheckFileExists = true,
                DefaultExt = "xml",
                Filter = "Exported XML files|*.xml|Text (one server per line) files|*.txt",
                FilterIndex = 0,
            };

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    string extension = Path.GetExtension(dlg.FileName).ToUpper();

                    if (extension == ".XML")
                    {
                        ImportXmlFile(dlg);
                    }
                    else if (extension == ".TXT")
                    {
                        ImportTextFile(dlg);
                    }
                    else
                    {
                        MainForm.ShowMessageBox("Invalid file extension: " + extension);
                    }
                }
                catch (Exception ex)
                {
                    MainForm.ShowMessageBox("Error importing XML file: " + ex.Message);
                }
            }
        }

        private void ImportXmlFile(OpenFileDialog dlg)
        {
            HashSet<string> existingNames = GetDisplayNames(null);
            int dupCount = 0;
            int keptCount = 0;

            using (var stream = dlg.OpenFile())
            {
                var serializer = new XmlSerializer(typeof(List<ExportableServer>));
                var servers = serializer.Deserialize(stream) as List<ExportableServer>;

                foreach (ExportableServer import in servers)
                {
                    if (existingNames.Add(import.HostName))
                    {
                        if (!import.HostName.NullOrWhiteSpace())
                        {
                            if (!import.HostAddress.NullOrWhiteSpace())
                            {
                                SavedServer newServer = new SavedServer()
                                {
                                    Category = import.Category,
                                    HostName = import.HostName,
                                    HostAddress = import.HostAddress,
                                    Port = import.Port,
                                };

                                AddServerItem(newServer);
                                btnOK.Enabled = true;
                                ++keptCount;
                            }
                        }
                    }
                    else
                    {
                        ++dupCount;
                    }
                }

                ShowImportResults(servers.Count, dupCount, keptCount);
            } // using stream
        }

        private void ImportTextFile(OpenFileDialog dlg)
        {
            HashSet<string> existingNames = GetDisplayNames(null);
            int dupCount = 0;
            int keptCount = 0;

            foreach (string rawLine in File.ReadAllLines(dlg.FileName))
            {
                string line = rawLine.Trim();

                if (!line.NullOrWhiteSpace())
                {
                    if (existingNames.Add(line))
                    {
                        SavedServer newServer = new SavedServer()
                        {
                            HostName = line,
                            HostAddress = line,
                        };

                        AddServerItem(newServer);
                        btnOK.Enabled = true;
                        ++keptCount;
                    }
                    else
                    {
                        ++dupCount;
                    }
                }
            }

            ShowImportResults(dupCount + keptCount, dupCount, keptCount);
        }

        private static void ShowImportResults(int serverCount, int dupCount, int keptCount)
        {
            string msg = "The selected file contained {0} servers.  {1} were imported.".Fmt(serverCount, keptCount);

            if (dupCount > 0)
            {
                msg += "\n{0} servers were rejected due to duplicate display names.".Fmt(dupCount);
            }

            int otherErrorCount = serverCount - dupCount - keptCount;

            if (otherErrorCount > 0)
            {
                msg += "\n{0} servers were rejected for unspecified reasons.".Fmt(otherErrorCount);
            }

            MainForm.ShowMessageBox(msg);
        }

        // Sets the category of multiple server entries.
        private void setCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var catDlg = new CategoryDialog();

            catDlg.SetCategories(GetUniqueCategories());

            if (catDlg.ShowDialog() == DialogResult.OK)
            {
                foreach (ServerListItem item in listView1.SelectedItems)
                {
                    item.Server.Category = catDlg.Category;
                    item.SubItems[colCategory.Index].Text = catDlg.Category;
                }

                btnOK.Enabled = true;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var editDlg = new ServerConnEditor();

            editDlg.Init(GetUniqueCategories(), GetDisplayNames(null), null);

            if (editDlg.ShowDialog() == DialogResult.OK)
            {
                AddServerItem(editDlg.NewServer);
                btnOK.Enabled = true;
            }
        }

        private void AddServerItem(SavedServer newServer)
        {
            ServerListItem item = new ServerListItem(new string[] { newServer.HostName, newServer.HostAndPort, newServer.Domain, newServer.Category, newServer.UserId, "" });
            item.Server = newServer;
            item.UseItemStyleForSubItems = false; // So we can set the ForeColor of the test result.
            listView1.Items.Add(item);
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 1)
            {
                ServerListItem clickedItem = listView1.GetItemAt(e.X, e.Y) as ServerListItem;

                if (clickedItem != null && clickedItem.Index >= 0)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        // Check for the TestResult column and check if it's a "link".
                        if ( clickedItem.TestConnectionResult != null)
                        {
                            // We're on a row that has a TestConnectionResult.  See if we're
                            // on the cell for the TestConnectionResult.

                            var subitem = clickedItem.GetSubItemAt(e.X, e.Y);

                            if (subitem == clickedItem.SubItems[colTestResult.Index])
                            {
                                clickedItem.TestConnectionResult.Show();
                            }
                        }
                    }
                    else if (e.Button == MouseButtons.Right && clickedItem.Selected && _waitingTests == 0)
                    {
                        editServerMenuItem.Enabled = listView1.SelectedItems.Count == 1;
                        contextMenuStrip1.Show(listView1, e.Location);
                    }
                }
            }
        }

        private void editServerMenuItem_Click(object sender, EventArgs e)
        {
            EditServerForItem(listView1.SelectedItems[0] as ServerListItem);
        }

        private void EditServerForItem(ServerListItem item)
        {
            var editDlg = new ServerConnEditor();

            editDlg.Init(GetUniqueCategories(), GetDisplayNames(item), item.Server);

            if (editDlg.ShowDialog() == DialogResult.OK)
            {
                item.SubItems[colDispName.Index].Text = item.Server.HostName;
                item.SubItems[colAddress.Index].Text = item.Server.HostAndPort;
                item.SubItems[colCategory.Index].Text = item.Server.Category;
                item.SubItems[colUserID.Index].Text = item.Server.UserId;

                btnOK.Enabled = true;
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2 && e.Button == MouseButtons.Left && _waitingTests == 0)
            {
                var clickedItem = listView1.GetItemAt(e.X, e.Y) as ServerListItem;

                if (clickedItem != null && clickedItem.Index >= 0 && clickedItem.Selected)
                {
                    EditServerForItem(clickedItem);
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnTest.Enabled = listView1.SelectedItems.Count > 0 && _waitingTests == 0;
        }

        private void TestConnection(ServerListItem item)
        {
            var remoteServer = new RemoteServer(item.Server);
            item.TestConnectionResult = remoteServer.TestConnection();

            if (this.IsHandleCreated)
            {
                try
                {
                    Invoke(new Action(() => ShowTestResult(item)));
                }
                catch (Exception ex)
                {                    
                    // User probably closed the form.
                    Debug.WriteLine("Exception: {0}", ex);
                }
            }
            else
            {
                // User probably closed the form.
                Debug.WriteLine("Form has no handle.");
            }
        }

        // Called in the GUI thread.
        private void ShowTestResult(ServerListItem lvi)
        {
            ListViewItem.ListViewSubItem cell = lvi.SubItems[colTestResult.Index];
            cell.ForeColor = Color.Blue;

            if (lvi.TestConnectionResult.Exception == null)
            {
                cell.Text = "Succeeded";
            }
            else
            {
                //cell.Text = "Failed";
                cell.Text = lvi.TestConnectionResult.Exception.Message;
            }

            if (--_waitingTests == 0)
            {
                // We just processed the last result we were waiting for, so controls can be re-enabled.

                btnTest.Enabled = listView1.SelectedItems.Count > 0;

            }
        }

        private int _waitingTests;

        private void btnTest_Click(object sender, EventArgs e)
        {
            List<Task> tasks = new List<Task> ();

            _waitingTests = 0;

            foreach (ServerListItem item in listView1.Items)
            {
                ListViewItem.ListViewSubItem testResultSubItem = item.SubItems[colTestResult.Index];
                
                testResultSubItem.ForeColor = Color.Black;
                item.TestConnectionResult = null;

                if (item.Selected)
                {
                    testResultSubItem.Text = "Waiting";
                    ++_waitingTests;
                    Task.Factory.StartNew(() => TestConnection(item));
                }
                else
                {
                    testResultSubItem.Text = "";
                }
            }

            if (_waitingTests > 0)
            {
                btnTest.Enabled = false;
            }
        }

        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {
            // Change the cursor to the "hand" when the mouse is over a cell for a TestConnectionResult.

            ServerListItem mouseItem = listView1.GetItemAt(e.X, e.Y) as ServerListItem;

            if (mouseItem == null || mouseItem.TestConnectionResult == null)
            {
                Cursor = Cursors.Default;
            }
            else 
            {
                // We're on a row that has a TestConnectionResult.  See if we're
                // on the cell for the TestConnectionResult.
                var subitem = mouseItem.GetSubItemAt(e.X, e.Y);

                if (subitem == mouseItem.SubItems[colTestResult.Index])
                {
                    Cursor = Cursors.Hand;
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            }
            
        }

        private void listView1_MouseLeave(object sender, EventArgs e)
        {
            // MouseMove should do this but sometimes it doesn't.
            Cursor = Cursors.Default;
        }
    }

    internal class ServerListItem : ListViewItem
    {
        public ServerListItem() : base() { }
        public ServerListItem(string[] subitems) : base(subitems) { }
        public ServerListItem(string[] subitems, int imageIndex) : base(subitems, imageIndex) { }
        public ServerListItem(ListViewItem.ListViewSubItem[] subitems, int imageIndex) : base(subitems, imageIndex) { }

        // Information about a server that is saved in the user's settings.
        public SavedServer Server;

        // Result of attempting to connect to the Server.
        public TestConnectionResult TestConnectionResult;        
    }

    // Information about a Server that is exported/imported.  Does not include the password!
    public class ExportableServer
    {
        public string HostName { get; set; }
        public string HostAddress { get; set; }
        public int Port { get; set; }
        public string Category { get; set; }
    }
}
