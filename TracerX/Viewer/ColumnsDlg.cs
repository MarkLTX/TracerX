using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BBS.TracerX.Viewer {
    internal partial class ColumnsDlg : Form {
        public ColumnsDlg() {
            InitializeComponent();
        }

        public ColumnsDlg(MainForm mainForm) {
            InitializeComponent();

            _mainForm = mainForm;

            // Sort the columns by their display index before adding them
            // to the CheckedListBox.
            _sortedColumns = (ColumnHeader[])_mainForm.OriginalColumns.Clone();
            Array.Sort(_sortedColumns, new Sorter());
            foreach (ColumnHeader hdr in _sortedColumns) {
                checkedListBox1.Items.Add(new Wrapper(hdr), hdr.ListView == _mainForm.TheListView);
            }
        }

        // All available columns sorted by DisplayIndex.
        private ColumnHeader[] _sortedColumns;

        private MainForm _mainForm;

        // Sort ColumnHeaders by their DisplayIndex property.
        private class Sorter : IComparer<ColumnHeader> {
            public int Compare(ColumnHeader x, ColumnHeader y) {
                return x.DisplayIndex - y.DisplayIndex;
            }
        }

        // The purpose of this wrapper class to have an object whose ToString returns
        // the ColumnHeader's Text property.
        private class Wrapper {
            public ColumnHeader Hdr;
            public Wrapper(ColumnHeader hdr) { Hdr = hdr; }
            public override string ToString() {
                return Hdr.Text;
            }
        }

        // This is called by both the OK and Apply buttons, which differ in their DialogResult property.
        private void ok_Click(object sender, EventArgs e) {
            _mainForm.TheListView.BeginUpdate();
            _mainForm.TheListView.Columns.Clear();
            
            // Add the checked headers in the same order they originally
            // appeared as required by other logic.
            foreach (ColumnHeader hdr in _mainForm.OriginalColumns) {
                foreach (Wrapper wrapper in this.checkedListBox1.CheckedItems) {
                    if (wrapper.Hdr == hdr) {
                        _mainForm.TheListView.Columns.Add(hdr);
                    }
                }
            }

            // Now set each item's DisplayIndex according to the order it
            // appears in the CheckedListBox.
            int ndx = 0;
            foreach (Wrapper wrapper in this.checkedListBox1.CheckedItems) {
                wrapper.Hdr.DisplayIndex = ndx;
                ++ndx;
            }

            // Any cached items are now invalid due to columns change.
            _mainForm.ClearItemCache();
            if (_mainForm.TheListView.VirtualListSize > 0) {
                _mainForm.TheListView.RedrawItems(_mainForm.TheListView.TopItem.Index, _mainForm.FindLastVisibleItem(), true);
            }

            _mainForm.TheListView.EndUpdate();
        }

        // Prevent Text column from being unchecked.
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e) {
            Wrapper item = this.checkedListBox1.Items[e.Index] as Wrapper;
            if (item.Hdr.Index == 0) {
                e.NewValue = CheckState.Checked;
            }
        }
    }
}