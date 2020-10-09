using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace TracerX
{
    internal partial class PathSelector : Form
    {
        public PathSelector()
        {
            InitializeComponent();
            Icon = Properties.Resources.scroll_view;
            pathGrid1.PathsAreLocal = false;
            pathGrid1.ShowTimesAgo = Properties.Settings.Default.ShowFileTimesAgo;
            pathGrid1.ChangeToFormMode();
        }

        public string SelectedFile
        {
            get;
            private set;
        }

        public void Init(RemoteServer remoteServer, string folder)
        {
            List<PathItem> files = remoteServer.GetFilesInFolder(folder);

            Debug.Print("{0} files returned from {1}", files.Count, remoteServer);

            label1.Text = string.Format(label1.Text, remoteServer.HostName, folder);
            pathGrid1.RemoteServer = remoteServer;
            pathGrid1.AddOrUpdatePaths(files, true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void pathGrid1_LastClickedItemChanged(object sender, EventArgs e)
        {
            // The LastClickedItem should always start with "file\n".
            SelectedFile = pathGrid1.LastClickedItem.Substring(5);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
