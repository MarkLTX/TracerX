using System;
using System.Windows.Forms;
using System.Collections;
using System.Text;
using System.Diagnostics;

namespace TracerX.Viewer {
    /// <summary>
    /// Implements the sorting of a ListView by any column.
    /// By default, it sorts by the text property of the column 
    /// passed to the Sort method.  To override that, set the
    /// column's Tag to a RowComparer delegate.
    /// </summary>
    internal class ListViewItemSorter : IComparer {
        // The ListView whose rows are sorted by the Sort() method.
        private ListView _listView;

        // Tracks the previously sorted column to determine if we're sorting the same col or a different col.
        private int _col = -1;

        // Sort direction to use.
        private bool _sortAscending;

        // Helps prevent redundant sorting.
        private bool _didSort;

        // The appropriate comaparison method for the current column.
        private RowComparer _comparer;

        /// <summary>
        /// Ctor takes the ListView to be sorted as a parameter.
        /// </summary>
        public ListViewItemSorter(ListView listView) {
            _listView = listView;
            _defaultComparer = delegate(ListViewItem x, ListViewItem y) {
                return string.Compare(x.SubItems[_col].Text, y.SubItems[_col].Text);
            };
        }

        /// <summary>
        /// If it is not sufficient to sort a given column by passing the column text
        /// to string.Compare() (see DefaultComparer), implement your own delegate
        /// of this type and store a reference to it in the ColumnHeader.Tag property.
        /// </summary>
        public delegate int RowComparer(ListViewItem x, ListViewItem y);

        // The default compare algorithm used for most columns just uses string.Compare().
        private RowComparer _defaultComparer;

        // IComparer.Compare
        public int Compare(object x, object y) {
            // The Sort() method will have set _comparer to the appropriate delegate for the
            // column being sorted.
            int result = _comparer((ListViewItem)x, (ListViewItem)y);

            if (_listView.Sorting == SortOrder.Descending) {
                result = -result;
            }

            // Debugging aid, could be deleted.
            //if (!_didSort) {
            //    Debug.WriteLine("Sorting column " + _col.ToString() + ", order = " + _listView.Sorting.ToString());
            //}

            _didSort = true; // Inform other code that the sort occurred.
            return result;
        }

        public void Sort(ColumnClickEventArgs e) {
            // If the sort column has changed, force ascending sort.  
            // Otherwise, toggle the sort order.
            if (_col == e.Column) {
                // Sorting same column again. Toggle the sort order.
                _sortAscending = !_sortAscending;
            } else {
                // Sorting a different column.  Use ascending sort
                // and switch to the appropriate RowComparer for the column.
                _sortAscending = true;
                _col = e.Column;
                _comparer = _listView.Columns[e.Column].Tag as RowComparer;

                if (_comparer == null) {
                    _comparer = _defaultComparer;
                }
            }

            // The list view tends to sort automatically when 
            // ListView.Sorting or ListView.ListViewItemSorter changes.
            // It's a little unpredictable, so we use _didSort to keep
            // track of whether the sort occurs or not (Comparer sets 
            // it to true).
            _didSort = false;

            // Always set _listView.Sorting because some users set it
            // to None between sorts to cause new items to appear
            // at the bottom.  This sometimes initiates the sort, setting _didSort to true.
            if (_sortAscending) {
                _listView.Sorting = SortOrder.Ascending;
            } else {
                _listView.Sorting = SortOrder.Descending;
            }

            // This assignment sometimes initiates the sort, like it or not.
            _listView.ListViewItemSorter = this;

            // Avoid re-sorting unnecessarily, but make sure
            // it happens at least once.
            if (!_didSort) {
                // This results in multiple calls to Compare().
                _listView.Sort();
            }
        } // Sort

    }
}
