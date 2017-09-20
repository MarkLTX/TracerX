using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace TracerX
{
    // This class (ServerTree) is the tree control that appears on the left side of the "start page"
    // (it derives from TreeView). It only has two levels of tree nodes: root nodes and child nodes.
    // The root-level nodes (e.g. the "All Servers" node) represent server categories, so they are
    // called category nodes. The child nodes represent servers (i.e. computers) running the TracerX
    // WCF service (basically a file server for TracerX logs). When the user selects a server node,
    // the SelectedServerChanged event is raised by OnAfterSelect(), causing the StartPage to display
    // the folders and files for that server.
    //
    // As a rule, the node for the "current" server remains selected/highlighted until the user
    // selects a different server. However, if the server's parent (category) node is collapsed, the
    // framework automatically selects the parent node since the child (server) node is no longer
    // visible. That's OK because it tells the user which node to expand to see the current server.
    //
    // The only time a category node can be selected is when the category (i.e. parent) node of the
    // current server node is collapsed. In that case the collapsed category node is automatically
    // selected because the child (server) node is no longer visible. If/when the user expands the
    // collapsed category node, the child node for the current server is automatically re-selected by OnAfterExpand().
    //
    // Whenever the mouse is over any selectable node, that node's BackColor is temporarily changed
    // to Gold. OnMouseMove() and OnMouseLeave() take care of this.
    //
    // Highlighting of the currently selected node is done explicitly by setting the SelectedNode's
    // BackColor and ForeColor because the framework doesn't maintain the highlighting when focus is
    // lost, regardless of whether HideSelection is true or false. We set HideSelection to false so
    // the framework doesn't interfere with this.

    internal class ServerTree : TreeView
    {
        public ServerTree() 
            :base()
        {
            InitializeComponent();
            DoubleBuffered = true;
            mnuEdit.Click += new EventHandler(mnuEdit_Click);
            mnuRemove.Click += new EventHandler(mnuRemove_Click);
            mnuTestConnection.Click += mnuTestConnection_Click;
        }

        private TreeNode _oldMouseNode;
        private ContextMenuStrip contextMenuForServers;
        private IContainer components;
        private ToolStripMenuItem mnuEdit;
        private ToolStripMenuItem mnuRemove;
        private TreeNode _curServerNode;
        private TreeNode _localNode;
        private TreeNode _allNode;
        private ToolStripMenuItem mnuTestConnection;
        private List<RemoteServer> _masterServerList; // Same object that NewStartPage has.
        string _filter = "";
        private HashSet<RemoteServer> _visibleServers = new HashSet<RemoteServer>();

        [Browsable(true)]
        public event EventHandler SelectedServerChanged;
        
        public RemoteServer SelectedServer
        {
            get
            {
                if (_curServerNode == null)
                {
                    return null;
                }
                else
                {
                    return _curServerNode.Tag as RemoteServer;
                }
            }

            set
            {
                SelectNodeForServer(value);
            }
        }

        public void PopulateServerTree(RemoteServer localHost, List<RemoteServer> remoteServers)
        {            
            _masterServerList = remoteServers;

            this.BeginUpdate();
            this.Nodes.Clear();

            // For all the nodes of the tree (at any level), 
            // the node's Key will be the same as its Text.

            // Create the special "Local Host" node (always the first node).
            // This is the only top-level node that isn't a category/group.

            _localNode = Nodes.Add("LocalHost", "LocalHost");
            _localNode.NodeFont = new System.Drawing.Font(this.Font, FontStyle.Bold);
            _localNode.Tag = localHost;

            // Create the special "All Remote Servers" category node. 

            _allNode = Nodes.Add("All Remote Servers", "All Remote Servers");
            _allNode.NodeFont = new System.Drawing.Font(this.Font, FontStyle.Bold);

            // Now add all the remote servers (that pass the filter) and their categories.

            _visibleServers.Clear();
            ApplyFilter(_filter);

            this.EndUpdate();
        }

        public void ApplyFilter(string filterString)
        {
            _filter = filterString;

            foreach (RemoteServer remsrv in _masterServerList)
            {
                if (_filter == "" || remsrv.HostName.IndexOf(_filter, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    // Passed the filter.
                    if (_visibleServers.Add(remsrv))
                    {
                        // Show it.
                        AddServerToTree(remsrv);
                    }
                    else
                    {
                        // Already visible.
                    }
                }
                else
                {
                    // Didn't pass the filter.

                    if (_visibleServers.Remove(remsrv))
                    {
                        // Hide it.

                        if (SelectedServer == remsrv)
                        {
                            // The server to be removed can't also be the SelectedServer,
                            // so select the LocalHost node.
                            this.SelectedNode = _localNode;
                        }
                        
                        RemoveServerFromTree(remsrv);
                    }
                    else
                    {
                        // Already hidden.
                    }
                }
            }
        }

        // Adds the server to both the "All Remote Servers" node and the server's
        // category node.
        public void AddServerToTree(RemoteServer server, bool changeMasterList = false)
        {
            if (changeMasterList)
            {
                _visibleServers.Add(server);
                _masterServerList.Add(server);
                SaveRemoteServers();
            }

            // First add the server to the "All Remote Servers" node.

            TreeNode serverNode = FindOrAddNode(_allNode.Nodes, server.HostName);
            serverNode.Tag = server;
            serverNode.ContextMenuStrip = contextMenuForServers;

            if (!string.IsNullOrWhiteSpace(server.Category))
            {
                // Find or add the server's category, then add the server to the category.

                TreeNode catNode = FindOrAddNode(this.Nodes, server.Category);
                catNode.NodeFont = _allNode.NodeFont;

                serverNode = FindOrAddNode(catNode.Nodes, server.HostName);
                serverNode.Tag = server;
                serverNode.ContextMenuStrip = contextMenuForServers;
            }
        }

        public bool ConfirmAndRemoveServer(RemoteServer server)
        {
            bool result = false;

            if (DialogResult.Yes == MainForm.ShowMessageBoxBtns("Remove '" + server.HostName + "'?", MessageBoxButtons.YesNo))
            {
                if (SelectedServer == server)
                {
                    // The server to be removed can't also be the SelectedServer,
                    // so select the LocalHost node.
                    this.SelectedNode = _localNode;
                }

                RemoveServerFromTree(server, changeMasterList: true);
                result = true;
            }

            return result;
        }

        // Removes the server's nodes from the "All Remote Servers"
        // node and the server's category node. 
        private void RemoveServerFromTree(RemoteServer server, bool changeMasterList = false)
        {
            if (changeMasterList)
            {
                _visibleServers.Remove(server);
                _masterServerList.Remove(server);
                SaveRemoteServers();
            }

            // The node must be removed from both the All Servers node and
            // the category node, if any.

            _allNode.Nodes.RemoveByKey(server.HostName);

            TreeNode categoryNode = Nodes[server.Category];

            if (categoryNode != null)
            {
                categoryNode.Nodes.RemoveByKey(server.HostName);

                if (categoryNode.Nodes.Count == 0)
                {
                    Nodes.Remove(categoryNode);
                }
            }
        }

        // Finds or adds the node with the specified name (i.e. the node's Text and Key)
        // in the specified nodeList.
        private TreeNode FindOrAddNode(TreeNodeCollection nodeList, string name)
        {
            TreeNode result = nodeList[name];

            if (result == null)
            {
                // Find the node to insert the new node before
                // (i.e. the first node whose Text/Key is
                // "greater than" than given name).

                foreach (TreeNode curNode in nodeList)
                {
                    if (curNode != _localNode && curNode != _allNode && curNode.Text.CompareTo(name) > 0)
                    {
                        result = nodeList.Insert(curNode.Index, name, name);
                        break;
                    }
                }

                if (result == null)
                {
                    result = nodeList.Add(name, name);
                }
            }

            return result;
        }
        
        private void SelectNodeForServer(RemoteServer server, bool forceAllServers = false)
        {
            if (server != null)
            {
                // Check for the special LocalHost node.

                if (server == _localNode.Tag)
                {
                    // This is the special LocalHost node.

                    this.SelectedNode = _localNode;
                }
                else
                {
                    TreeNode categoryNode = _allNode;

                    if (!forceAllServers)
                    {
                        // If the server's category is empty or null or missing, 
                        // this falls back to the "All Remote Servers" node.

                        categoryNode = Nodes[server.Category] ?? _allNode;
                    }

                    this.SelectedNode = categoryNode.Nodes[server.HostName];
                }
            }
        }

        void mnuTestConnection_Click(object sender, EventArgs e)
        {
            var node = contextMenuForServers.Tag as TreeNode;
            var server = node.Tag as RemoteServer;
            server.TestConnection().Show();
        }

        void mnuRemove_Click(object sender, EventArgs e)
        {
            var node = contextMenuForServers.Tag as TreeNode;
            var server = node.Tag as RemoteServer;

            ConfirmAndRemoveServer(server);
        }
        
        void mnuEdit_Click(object sender, EventArgs e)
        {
            var node = contextMenuForServers.Tag as TreeNode;
            var server = node.Tag as RemoteServer;
            bool isUnderAllServers = node.Parent == _allNode;
            bool isSelectedServer = server == SelectedServer;
            var editableServer = server.MakeSavedServer();
            var dlg = new ServerConnEditor();

             // Get list of unique categories.
            string[] categories =_masterServerList
                .Select(svr => svr.Category)
                .Where(cat => !string.IsNullOrWhiteSpace(cat))
                .Distinct()
                .ToArray();

            var otherServers = _masterServerList.Where(svr => svr != server).Select(svr => svr.HostName);

            dlg.Init(categories, otherServers, editableServer, allowAddressEdit: !isSelectedServer);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // User may have changed the "display name" and/or category, both
                // of which affect the server's location in the tree.  The easiest way
                // to handle this is to remove the old server and replace it with the
                // new one. Note that we can be editing the currently SelectedServer.

                _ignoreSelectionEvents = true;

                RemoveServerFromTree(server);
                server.InitFromSavedServer(editableServer);
                AddServerToTree(server);
                SaveRemoteServers();

                // If we just edited, removed, and re-added the current SelectedServer, we must also re-select its TreeNode.

                if (isSelectedServer)
                {
                    SelectNodeForServer(server, forceAllServers: isUnderAllServers);
                }
                else
                {
                    // Ask the user if he wants to connect to the edited server.

                    if (MainForm.ShowMessageBoxBtns("Connect to '" + server.HostName + "'?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        SelectedServer = server;
                    }
                }
                
                _ignoreSelectionEvents = false;
            }
        }

        /// <summary>
        /// Saves the list of remote servers and their recently viewed files/folders
        /// to Settings.Default.SavedRemoteServers.
        /// </summary>
        public void SaveRemoteServers()
        {
            if (_masterServerList == null)
            {
                Properties.Settings.Default.SavedRemoteServers = new SavedServer[0];
            }
            else
            {
                // Convert each RemoteServer to SavedServer and save the SavedServer[] in the settings.

                Properties.Settings.Default.SavedRemoteServers =
                    _masterServerList
                    .Select(rs => rs.MakeSavedServer())
                    .ToArray();
            }

            Properties.Settings.Default.Save();
        }

        // Colors the node the mouse is over, if any.
        protected override void OnMouseMove(MouseEventArgs e)
        {
            var newMouseNode = this.GetNodeAt(e.Location);

            // Un-color the previous mouse node, if any.
            if (_oldMouseNode != null && _oldMouseNode != newMouseNode)
            {
                _oldMouseNode.BackColor = Color.Empty;
                _oldMouseNode = null;
            }

            if (newMouseNode != null && newMouseNode != SelectedNode && newMouseNode.Tag != null && newMouseNode.BackColor != Color.Gold)
            {
                newMouseNode.BackColor = Color.Gold;
                _oldMouseNode = newMouseNode;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            // Un-color the previously colored mouse-node, if any.

            if (_oldMouseNode != null)
            {
                _oldMouseNode.BackColor = Color.Empty;
                _oldMouseNode = null;
            }

            base.OnMouseLeave(e);
        }

        bool _ignoreSelectionEvents;

        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            using (Logger.Current.InfoCall())
            {
                // Server nodes (Tag != null) can always be selected, but we
                // allow the selection of category nodes (Tag == null) only when the node
                // is collapsed and it's the parent of _curServerNode.

                bool selectable = e.Node.Tag != null || (_curServerNode != null && e.Node == _curServerNode.Parent && !e.Node.IsExpanded);

                if (!selectable)
                {
                    // This must be a category node, because server nodes have non-null tags.
                    // We don't allow the user to select category nodes so cancel the selection.
                    // Note that this event is not raised when a parent node is automatically
                    // selected by the framework when the parent node of the currently selected
                    // node is collapsed.

                    e.Cancel = true;
                }
                else if (SelectedNode != null)
                {
                    // When deselecting the currently selected node, restore it's default colors.
                    SelectedNode.BackColor = Color.Empty;
                    SelectedNode.ForeColor = Color.Empty;
                }

                base.OnBeforeSelect(e);
            }
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            using (Logger.Current.InfoCall())
            {
                var selectedServer = SelectedNode.Tag as RemoteServer;

                if (selectedServer != null && _curServerNode != SelectedNode)
                {
                    _curServerNode = SelectedNode;

                    if (SelectedServerChanged != null && !_ignoreSelectionEvents)
                    {
                        SelectedServerChanged(this, EventArgs.Empty);
                    }
                }

                // Since the framework doesn't properly highlight the SelectedNode when
                // we don't have the focus, set the highlighting colors explicitly.

                this.HideSelection = true;
                this.SelectedNode.BackColor = SystemColors.Highlight;
                this.SelectedNode.ForeColor = SystemColors.HighlightText;

                base.OnAfterSelect(e);
            }
        }

        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            using (Logger.Current.InfoCall())
            {
                Logger.Current.Info("Node text = ", e.Node.Text, ", action = ", e.Action);
                
                // If the expanded node contains the node for the current server, re-select it.

                if (e.Action == TreeViewAction.Expand && _curServerNode != null && _curServerNode.Parent == e.Node)
                {
                    this.SelectedNode = _curServerNode;
                }

                base.OnAfterExpand(e);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            //if (this.SelectedNode != null)
            //{
            //    this.SelectedNode.BackColor = Color.Empty;
            //}

            // Focus is needed to receive mouse wheel events.
            this.Focus();
            base.OnMouseEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            if (this.SelectedNode != null)
            {
                // Since the framework doesn't properly highlight the SelectedNode when
                // we don't have the focus, set the highlightling colors explicitly.

                this.HideSelection = true;
                this.SelectedNode.BackColor = SystemColors.Highlight;
                this.SelectedNode.ForeColor = SystemColors.HighlightText;
            }

            base.OnLeave(e);
        }

        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            using (Logger.Current.InfoCall())
            {
                Logger.Current.Info("Node text = ", e.Node.Text);
                base.OnNodeMouseClick(e);
                Logger.Current.Info("Back from base.ONodeMouseClick");

                if (e.Button == MouseButtons.Left)
                {
                    //if (e.Node.IsExpanded)
                    //{
                    //    Logger.Current.Info("Collapsing node: ", e.Node.Text);
                    //    e.Node.Collapse();
                    //}
                    //else
                    //{
                    //    Logger.Current.Info("Expanding node: ", e.Node.Text);
                    //    e.Node.Expand();
                    //}
                }
                else if (e.Button == MouseButtons.Right)
                {
                    contextMenuForServers.Tag = e.Node;
                }
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuForServers = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTestConnection = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuForServers.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuForServers
            // 
            this.contextMenuForServers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEdit,
            this.mnuRemove,
            this.mnuTestConnection});
            this.contextMenuForServers.Name = "contextMenuForServers";
            this.contextMenuForServers.Size = new System.Drawing.Size(161, 70);
            // 
            // mnuEdit
            // 
            this.mnuEdit.Name = "mnuEdit";
            this.mnuEdit.Size = new System.Drawing.Size(160, 22);
            this.mnuEdit.Text = "Edit";
            // 
            // mnuRemove
            // 
            this.mnuRemove.Name = "mnuRemove";
            this.mnuRemove.Size = new System.Drawing.Size(160, 22);
            this.mnuRemove.Text = "Remove";
            // 
            // mnuTestConnection
            // 
            this.mnuTestConnection.Name = "mnuTestConnection";
            this.mnuTestConnection.Size = new System.Drawing.Size(160, 22);
            this.mnuTestConnection.Text = "Test Connection";
            // 
            // ServerTree
            // 
            this.LineColor = System.Drawing.Color.Black;
            this.contextMenuForServers.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
