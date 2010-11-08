using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TracerX.Viewer
{
    internal partial class ColumnsDlg : Form
    {
        public ColumnsDlg()
        {
            InitializeComponent();
        }

        public ColumnsDlg(MainForm mainForm)
        {
            InitializeComponent();

            _mainForm = mainForm;

            // Sort the columns by their display index before adding them
            // to the CheckedListBox.
            _sortedColumns = (ColumnHeader[])_mainForm.OriginalColumns.Clone();
            Array.Sort(_sortedColumns, new Sorter());
            //foreach (ColumnHeader hdr in _sortedColumns) {
            //    checkedListBox1.Items.Add(new Wrapper(hdr), hdr.ListView == _mainForm.TheListView);
            //}

            // Add the columns to the list control in display order.
            foreach (ColumnHeader hdr in _sortedColumns)
            {
                {
                    ListViewItem item = listView1.Items.Add(hdr.Text);
                    item.Checked = hdr.ListView != null;
                    item.Tag = hdr;
                }

            }
        }

        // All available columns sorted by DisplayIndex.
        private ColumnHeader[] _sortedColumns;

        private MainForm _mainForm;

        // Sort ColumnHeaders by their DisplayIndex property.
        private class Sorter : IComparer<ColumnHeader>
        {
            public int Compare(ColumnHeader x, ColumnHeader y)
            {
                return x.DisplayIndex - y.DisplayIndex;
            }
        }

        //// The purpose of this wrapper class to have an object whose ToString returns
        //// the ColumnHeader's Text property.
        //private class Wrapper {
        //    public ColumnHeader Hdr;
        //    public Wrapper(ColumnHeader hdr) { Hdr = hdr; }
        //    public override string ToString() {
        //        return Hdr.Text;
        //    }
        //}

        // This is called by both the OK and Apply buttons, which differ in their DialogResult property.
        private void ok_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count == 0)
            {
                MessageBox.Show("You must check at least one column.", "TracerX-Viewer");
                DialogResult = DialogResult.None;
            }
            else
            {
                _mainForm.TheListView.BeginUpdate();

                // ListView columns can't be hidden, so the unchecked ones are removed
                // from the Columns collection by clearing the collection and adding the checked ones.
                // Other logic requires they be added in their original order.
                _mainForm.TheListView.Columns.Clear();

                foreach (ColumnHeader hdr in _mainForm.OriginalColumns)
                {
                    foreach (ListViewItem item in listView1.CheckedItems)
                    {
                        if (item.Tag == hdr)
                        {
                            _mainForm.TheListView.Columns.Add(hdr);
                        }
                    }
                }

                // Now that the desired columns have been added, it's 
                // safe to set their DisplayIndex.
                for (int i = 0; i < listView1.CheckedItems.Count; ++i)
                {
                    ColumnHeader col = (ColumnHeader)listView1.CheckedItems[i].Tag;
                    col.DisplayIndex = i;
                }

                // Any cached items are now invalid due to columns change.
                _mainForm.TheListView.ClearItemCache();
                if (_mainForm.TheListView.VirtualListSize > 0)
                {
                    _mainForm.TheListView.RedrawItems(_mainForm.TheListView.TopItem.Index, _mainForm.FindLastVisibleItem(), true);
                }

                _mainForm.TheListView.EndUpdate();
            }
        }

        // Prevent Text column from being unchecked.
        private void listView1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (listView1.Items[e.Index].Tag == _mainForm.headerText)
            {
                e.NewValue = CheckState.Checked;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selCount = listView1.SelectedItems.Count;

            up.Enabled = selCount > 0 && !listView1.Items[0].Selected;
            down.Enabled = selCount > 0 && !listView1.Items[listView1.Items.Count - 1].Selected;
        }

        private void up_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                int ndx = item.Index;
                item.Remove();
                listView1.Items.Insert(ndx - 1, item);
            }
        }

        private void down_Click(object sender, EventArgs e)
        {
            List<ListViewItem> selected = new List<ListViewItem>();

            foreach (ListViewItem item in listView1.SelectedItems)
            {
                selected.Add(item);
            }

            selected.Reverse();

            foreach (ListViewItem item in selected)
            {
                int ndx = item.Index;
                item.Remove();
                listView1.Items.Insert(ndx + 1, item);
            }
        }
    }
}