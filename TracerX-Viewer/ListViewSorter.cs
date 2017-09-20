using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace TracerX
{
    /// <summary>
    /// To add ascending and descending sorting to a ListView, create an instance of this 
    /// for the ListView (pass the ListView to the constructor).  It will handle the ColumnClick
    /// events, draw the litte arrows in the headers, and toggle between ascending and descending 
    /// sorts when any column header is clicked. By default, this sorts the ListView by comparing 
    /// the text values in whatever column is clicked.  To use a custom sort (e.g. comparing 
    /// integers) for a given column, add a RowComparer delegate for that column to the
    /// CustomComparers property.
    /// </summary>
    public class ListViewSorter : IComparer
    {
        /// <summary>
        /// If it is not sufficient to sort a given column by passing the column text
        /// to string.Compare(), implement your own instance of RowComparer 
        /// and add it to CustomComparers.
        /// </summary>
        public Dictionary<ColumnHeader, RowComparer> CustomComparers { get; private set; }

        /// <summary>
        /// Delegates of this type are used for comparing rows.  
        /// </summary>
        public delegate int RowComparer(ListViewItem x, ListViewItem y);

        /// <summary>
        /// Enables and disables automatic sorting when the ListView's ColumnClick event is raised. 
        /// </summary>
        public bool HandleClicks { get; set; }

        /// <summary>
        /// True for the duration of the Sort method.
        /// </summary>
        public bool IsSorting { get; private set; }

        /// <summary>
        /// Gets the current sort direction (true for ascending, false for descending).
        /// </summary>
        public SortOrder SortOrder
        {
            get { return _sortAscending ? SortOrder.Ascending : SortOrder.Descending; }
        }

        /// <summary>
        /// Gets the index of the current sort column.  -1 if never sorted.
        /// </summary>
        public int SortedColIndex
        {
            get { return _col; }
        }

        // The default compare algorithm used for most columns just uses string.Compare().
        private RowComparer _defaultComparer;

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
        public ListViewSorter(ListView listView)
        {
            _listView = listView;
            _listView.ColumnClick += new ColumnClickEventHandler(_listView_ColumnClick);

            CustomComparers = new Dictionary<ColumnHeader, RowComparer>();
            HandleClicks = true;

            // This is the default comparer used if a custom comparer is not found.
            _defaultComparer = delegate(ListViewItem x, ListViewItem y)
            {
                return string.Compare(x.SubItems[_col].Text, y.SubItems[_col].Text);
            };
        }

        /// <summary>
        /// Sorts on the specified column (0 based index).  
        /// Specify a sortOrder of None to let the sort direction be chosen automatically
        /// (toggles the current sort direction if already sorted on tha same column).
        /// </summary>
        public void Sort(int colIndex, SortOrder sortOrder = SortOrder.None)
        {
            try
            {
                IsSorting = true;

                // If the sort column has changed, force ascending sort.  
                // Otherwise, toggle the sort order.
                if (_col == colIndex)
                {
                    // Sorting same column again. Toggle the sort order.
                    _sortAscending = !_sortAscending;
                }
                else
                {
                    // Sorting a different column.  Use ascending sort
                    // and switch to the appropriate RowComparer for the column.
                    _sortAscending = true;
                    _col = colIndex;

                    if (!CustomComparers.TryGetValue(_listView.Columns[_col], out _comparer))
                    {
                        _comparer = _defaultComparer;
                    }
                }

                switch (sortOrder)
                {
                    case SortOrder.None:
                        break;
                    case SortOrder.Ascending:
                        _sortAscending = true;
                        break;
                    case SortOrder.Descending:
                        _sortAscending = false;
                        break;
                }

                // The list view tends to sort automatically when 
                // ListView.Sorting or ListView.ListViewItemSorter changes.
                // It's a little unpredictable, so we use _didSort to keep
                // track of whether the sort occurs or not to avoid
                // unnecessary sorting. Comparer sets it to true.
                _didSort = false;

                // Always set _listView.Sorting because some users set it
                // to None between sorts to cause new items to appear
                // at the bottom.  
                // This sometimes initiates the sort, setting _didSort to true.
                if (_sortAscending)
                {
                    _listView.Sorting = SortOrder.Ascending;
                }
                else
                {
                    _listView.Sorting = SortOrder.Descending;
                }

                _listView.SetSortIcon(colIndex, _listView.Sorting);

                // This assignment sometimes initiates the sort, like it or not.
                _listView.ListViewItemSorter = this;

                // Avoid re-sorting unnecessarily, but make sure
                // it happens at least once.
                if (!_didSort)
                {
                    // This results in multiple calls to Compare().
                    _listView.Sort();
                }
            }
            finally
            {
                IsSorting = false;
            }
        } // Sort

        // IComparer.Compare
        public int Compare(object x, object y)
        {
            // The Sort() method will have set _comparer to the appropriate delegate for the
            // column being sorted.
            int result = _comparer((ListViewItem)x, (ListViewItem)y);

            if (_listView.Sorting == SortOrder.Descending)
            {
                result = -result;
            }

            // Debugging aid, could be deleted.
            //if (!_didSort) {
            //    Debug.WriteLine("Sorting column " + _col.ToString() + ", order = " + _listView.Sorting.ToString());
            //}

            _didSort = true; // Inform other code that the sort occurred.
            return result;
        }

        private void _listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (HandleClicks)
            {
                Sort(e.Column);
            }
        }

    }

    /// <summary>
    /// Displays the sort glyph (arrow) in the column header of a ListView control.
    /// Just call YourListView.SetSortIcon(colIndex, SortOrder.Ascending);
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ListViewExtensions
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct HDITEM
        {
            public Mask mask;
            public int cxy;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszText;
            public IntPtr hbm;
            public int cchTextMax;
            public Format fmt;
            public IntPtr lParam;
            // _WIN32_IE >= 0x0300 
            public int iImage;
            public int iOrder;
            // _WIN32_IE >= 0x0500
            public uint type;
            public IntPtr pvFilter;
            // _WIN32_WINNT >= 0x0600
            public uint state;

            [Flags]
            public enum Mask
            {
                Format = 0x4,       // HDI_FORMAT
            };

            [Flags]
            public enum Format
            {
                SortDown = 0x200,   // HDF_SORTDOWN
                SortUp = 0x400,     // HDF_SORTUP
            };
        };

        private const int LVM_FIRST = 0x1000;
        private const int LVM_GETHEADER = LVM_FIRST + 31;

        private const int HDM_FIRST = 0x1200;
        private const int HDM_GETITEM = HDM_FIRST + 11;
        private const int HDM_SETITEM = HDM_FIRST + 12;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, ref HDITEM lParam);

        public static void SetSortIcon(this ListView listViewControl, int columnIndex, SortOrder order)
        {
            IntPtr columnHeader = SendMessage(listViewControl.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
            for (int columnNumber = 0; columnNumber <= listViewControl.Columns.Count - 1; columnNumber++)
            {
                var columnPtr = new IntPtr(columnNumber);
                var item = new HDITEM
                {
                    mask = HDITEM.Mask.Format
                };

                if (SendMessage(columnHeader, HDM_GETITEM, columnPtr, ref item) == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                if (order != SortOrder.None && columnNumber == columnIndex)
                {
                    switch (order)
                    {
                        case SortOrder.Ascending:
                            item.fmt &= ~HDITEM.Format.SortDown;
                            item.fmt |= HDITEM.Format.SortUp;
                            break;
                        case SortOrder.Descending:
                            item.fmt &= ~HDITEM.Format.SortUp;
                            item.fmt |= HDITEM.Format.SortDown;
                            break;
                    }
                }
                else
                {
                    item.fmt &= ~HDITEM.Format.SortDown & ~HDITEM.Format.SortUp;
                }

                if (SendMessage(columnHeader, HDM_SETITEM, columnPtr, ref item) == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }
            }
        }
    }

}
