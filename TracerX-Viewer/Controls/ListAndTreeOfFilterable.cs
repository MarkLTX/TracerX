using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TracerX
{
    // Used on tab pages in the FilterDialog to show either a list or tree of IFilterable objects.

    internal partial class ListAndTreeOfFilterable : UserControl
    {
        public ListAndTreeOfFilterable()
        {
            InitializeComponent();
        }

        private bool _suppressEvents = true;
        private ListViewSorter _loggerSorter;
        private List<TreeNode> _loggerTreeNodes;

        public event EventHandler ItemChecked;

        public bool IsShowingTree // vs. show the list.
        {
            get;
            private set;
        }

        public bool IsAllChecked
        {
            get;
            private set;
        }

        public string ColumnHeaderText
        {
            get { return loggerNameCol.Text; }
            set { loggerNameCol.Text = value; }
        }

        public void InitViews(IEnumerable<IFilterable> loggers, bool showTree)
        {
            // There are two "views" of the loggers: a flat list and a tree view.  
            // The user checks a radio box to determine which one is visible.
            // First make both views (loggerListView and loggerTreeView) have the same size and location.

            _suppressEvents = true;

            int width = loggerTreeView.Right - loggerListView.Left;
            loggerListView.Width = width;
            loggerTreeView.Width = width;
            loggerTreeView.Left = loggerListView.Left;

            radLoggerList.Checked = !showTree;
            radLoggerTree.Checked = showTree;

            loggerListView.Visible = !showTree;
            loggerTreeView.Visible = showTree;
            expandLoggers.Enabled = showTree;
            collapseLoggers.Enabled = showTree;

            IsShowingTree = showTree;

            // Sort the loggers by name to make it easier to build the tree.
            var sortedLoggers = loggers.OrderBy(x => x.Name, StringComparer.Ordinal);

            IsAllChecked = true;

            foreach (IFilterable logger in sortedLoggers)
            {
                ListViewItem item = new ListViewItem(new string[] { string.Empty, logger.Name });
                IsAllChecked = IsAllChecked && logger.Visible;
                item.Checked = logger.Visible;
                item.Tag = logger;
                this.loggerListView.Items.Add(item);
            }

            loggerTreeView.Nodes.Clear();
            TreeNode lastNode = null;
            _loggerTreeNodes = new List<TreeNode>();

            foreach (IFilterable logger in sortedLoggers)
            {
                // If the logger has an ancestor (e.g. A is the ancestor of A.B.C if there's no A.B), it will be
                // the last node added or one of its ancestors.  Consider adding these loggers in the order listed...
                // A - Has no ancestor.
                // A.B.C - Ancestor A is lastNode.
                // A.X - Ancestor A is an ancestor of lastNode A.B.C.

                TreeNode ancestorNode = FindAncestorNode(lastNode, logger.Name);

                if (ancestorNode == null)
                {
                    // MakeTreeNode() will return a new TreeNode for the logger and
                    // set ancestorNode to a new TreeNode for us to insert at the root level.

                    lastNode = MakeTreeNode(logger, ref ancestorNode);
                    loggerTreeView.Nodes.Add(ancestorNode);
                }
                else
                {
                    // MakeTreeNode() will return a new TreeNode for the logger and
                    // make it a descendant of the ancestorNode.

                    lastNode = MakeTreeNode(logger, ref ancestorNode);
                }

                // Keep a list of all TreeNodes that have loggers attached.
                _loggerTreeNodes.Add(lastNode);
            }

            // Leave _suppressEvents == true until OnLoad().
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _suppressEvents = false;
        }

        public void ApplyCurrentSelection()
        {
            // Don't rely on loggerListView.Visible or loggerTreeView.Visible because both will
            // be false if the user is not on the Loggers tab.

            if (radLoggerList.Checked)
            {
                foreach (ListViewItem item in loggerListView.Items)
                {
                    IFilterable logger = (IFilterable)item.Tag;
                    logger.Visible = item.Checked;
                }
            }

            if (radLoggerTree.Checked)
            {
                foreach (TreeNode tn in _loggerTreeNodes)
                {
                    (tn.Tag as IFilterable).Visible = tn.Checked;
                }
            }
        }

        private TreeNode FindAncestorNode(TreeNode curnode, string name)
        {
            // Since curnode.Name should always end with '.', this will return null 
            // if name has no '.', forcing the caller to create a new root node.

            while (curnode != null && !name.StartsWith(curnode.Name))
            {
                curnode = curnode.Parent;
            }

            return curnode;
        }

        private TreeNode MakeTreeNode(IFilterable logger, ref TreeNode ancestorNode)
        {
            // If the logger's name is A.B.C.D and the ancestorNode's name is A., we
            // need to make these placeholder nodes between the A node and the D node...
            //  Name = "A.B.", Text = B
            //  Name = "A.B.C.", Text = C
            // If ancestorNode is null we need to create ancestor A as well.

            // If we're given an ancestorNode, skip past that part of given logger's name to 
            // get to the name of the first new node to create as a child of the given ancestor.

            int curStart = ancestorNode == null ? 0 : ancestorNode.Name.Length;
            int curEnd = logger.Name.IndexOf('.', curStart);
            TreeNode result = null;
            TreeNode lastParentNode = ancestorNode; // possibly null.

            while (curEnd != -1)
            {
                // Create the placeholder parent node of whatever comes after the '.' at curEnd.

                var newParentNode = new TreeNode()
                {
                    Name = logger.Name.Substring(0, curEnd + 1), // Ends with '.'
                    Text = logger.Name.Substring(curStart, curEnd - curStart),
                    Checked = false,
                    Tag = null,
                    ForeColor = Color.Gray
                };

                if (ancestorNode == null)
                {
                    // First time through.  The first parent is the "greatest" ancestor of our ultimate result node.
                    ancestorNode = newParentNode;
                }
                else
                {
                    // Make the current parent a child of the previous parent.
                    lastParentNode.Nodes.Add(newParentNode);
                }

                lastParentNode = newParentNode;
                curStart = curEnd + 1;
                curEnd = logger.Name.IndexOf('.', curStart);
            }

            result = new TreeNode()
            {
                Name = logger.Name + '.',
                Text = logger.Name.Substring(curStart),
                Checked = logger.Visible,
                Tag = logger,
            };

            if (ancestorNode == null)
            {
                // The while loop never executed because logger.Name has no '.'
                ancestorNode = result;
            }
            else
            {
                lastParentNode.Nodes.Add(result);
            }

            return result;
        }

        private void radLoggerTree_CheckedChanged(object sender, EventArgs e)
        {
            // For some reason, this event is not raised when the Tree radio button 
            // is unchecked, so we have to watch the List radio button too.

            if (!_suppressEvents)
            {
                ChangeView(radLoggerTree.Checked);
            }
        }

        private void radLoggerList_CheckedChanged(object sender, EventArgs e)
        {
            if (!_suppressEvents)
            {
                ChangeView(radLoggerTree.Checked);
            }
        }

        private void ChangeView(bool showTree)
        {
            // This toggles between showing the list and showing the tree.

            if (IsShowingTree != showTree)
            {
                IsShowingTree = showTree;

                loggerListView.Visible = !IsShowingTree;
                loggerTreeView.Visible = IsShowingTree;
                expandLoggers.Enabled = IsShowingTree;
                collapseLoggers.Enabled = IsShowingTree;

                HashSet<IFilterable> checkedLoggers = new HashSet<IFilterable>();

                _suppressEvents = true;

                if (IsShowingTree)
                {
                    // Switching from List to Tree.  Set the checkboxes in the Tree to match those in the List.

                    foreach (ListViewItem item in loggerListView.Items)
                    {
                        if (item.Checked) checkedLoggers.Add(item.Tag as IFilterable);
                    }

                    foreach (TreeNode tn in _loggerTreeNodes)
                    {
                        tn.Checked = checkedLoggers.Contains(tn.Tag as IFilterable);
                    }
                }
                else
                {
                    // Switching from Tree to List.  Set the checkboxes in the List to match those in the Tree.

                    foreach (TreeNode tn in _loggerTreeNodes)
                    {
                        if (tn.Checked) checkedLoggers.Add(tn.Tag as IFilterable);
                    }

                    foreach (ListViewItem item in loggerListView.Items)
                    {
                        item.Checked = checkedLoggers.Contains(item.Tag as IFilterable);
                    }
                }

                _suppressEvents = false;
            }
        }

        private void loggerTreeView_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            // Don't allow nodes without loggers to be checked.
            if (e.Node.Tag == null && !e.Node.Checked) e.Cancel = true;
        }

        private void loggerTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (_suppressEvents) return;
            IsAllChecked = _loggerTreeNodes.All(tn => tn.Checked);
            if (ItemChecked != null) ItemChecked(this, EventArgs.Empty);
        }

        private void loggerListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (_suppressEvents) return;

            var sorter = loggerListView.ListViewItemSorter as ListViewSorter;
            if (sorter != null && sorter.IsSorting) return;

            IsAllChecked = (loggerListView.CheckedItems.Count == loggerListView.Items.Count);
            
            if (ItemChecked != null) ItemChecked(this, EventArgs.Empty);
        }

        private void checkAllLoggers_Click(object sender, EventArgs e)
        {
            bool changed = false;

            _suppressEvents = true;

            if (loggerListView.Visible)
            {
                foreach (ListViewItem item in loggerListView.Items)
                {
                    changed = !item.Checked;
                    item.Checked = true;
                }
            }

            if (loggerTreeView.Visible)
            {
                foreach (TreeNode tn in _loggerTreeNodes)
                {
                    changed = !tn.Checked;
                    tn.Checked = true;
                }
            }
         
            _suppressEvents = false;
            IsAllChecked = true;
            if (changed && ItemChecked != null) ItemChecked(this, EventArgs.Empty);
        }

        private void uncheckAllLoggers_Click(object sender, EventArgs e)
        {
            bool changed = false;

            _suppressEvents = true;

            if (loggerListView.Visible)
            {
                foreach (ListViewItem item in loggerListView.Items)
                {
                    changed = item.Checked;
                    item.Checked = false;
                }
            }

            if (loggerTreeView.Visible)
            {
                foreach (TreeNode tn in _loggerTreeNodes)
                {
                    changed = tn.Checked;
                    tn.Checked = false;
                }
            }

            _suppressEvents = false;
            IsAllChecked = false;
            if (changed && ItemChecked != null) ItemChecked(this, EventArgs.Empty);
        }

        private void invertLoggers_Click(object sender, EventArgs e)
        {
            _suppressEvents = true;
            IsAllChecked = true;

            if (loggerListView.Visible)
            {
                foreach (ListViewItem item in loggerListView.Items)
                {
                    item.Checked = !item.Checked;
                    IsAllChecked = IsAllChecked && item.Checked;
                }
            }

            if (loggerTreeView.Visible)
            {
                foreach (TreeNode tn in _loggerTreeNodes)
                {
                    tn.Checked = !tn.Checked;
                    IsAllChecked = IsAllChecked && tn.Checked;
                }
            }

            _suppressEvents = false;
            if (ItemChecked != null) ItemChecked(this, EventArgs.Empty);
        }

        private void expandLoggers_Click(object sender, EventArgs e)
        {
            var topNode = loggerTreeView.TopNode;
            loggerTreeView.ExpandAll();
            loggerTreeView.TopNode = topNode;
        }

        private void collapseLoggers_Click(object sender, EventArgs e)
        {
            foreach (TreeNode tn in loggerTreeView.Nodes)
            {
                tn.Collapse();
            }
        }

        private void loggerListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            _suppressEvents = true;
            // Create the sorter object the first time it is required.
            if (_loggerSorter == null)
            {
                _loggerSorter = new ListViewSorter(loggerListView);
                _loggerSorter.CustomComparers[loggerCheckCol] = _checkComparer;
                _loggerSorter.Sort(e.Column);
            }

            _suppressEvents = false;
        }


        // Create a delegate to compare the Checked states of two ListViewItems
        // for sorting via ListViewItemSorter.
        private ListViewSorter.RowComparer _checkComparer = delegate(ListViewItem x, ListViewItem y)
        {
            if (x.Checked == y.Checked) return 0;
            if (x.Checked) return 1;
            else return -1;
        };
    }
}
