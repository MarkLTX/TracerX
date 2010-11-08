using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TracerX.Viewer;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;

namespace TracerX {
    /// <summary>
    /// Slightly customized version of ListView
    /// </summary>
    internal partial class ListViewTx : ListView
    {

        public MainForm MainForm;

        // Cache of ListViewItems used to improve the performance of the ListView control
        // in virtual mode (it tends to request the same item many times).
        private List<ViewItem> _itemCache = new List<ViewItem>(50);

        // Index of first item in _itemCache.
        private int _firstItemIndex = -1;

        // This lets us use reflection to update the ListView's private virtualListSize field, 
        // bypassing the VirtualListSize property.  This reduces flicker when the ListView is
        // updated when new records are read from the file.
        private FieldInfo listViewVirtualListSizeField;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ListViewTx()
        {
            InitializeComponent();

            DoubleBuffered = true;

            // Since we're using the ListView in virtual mode, DoubleBuffered isn't enough to
            // prevent flicker.  We use reflection to bypass the VirtualListSize property and
            // set the underlying field directly to eliminate flicker.
            listViewVirtualListSizeField = GetVirtualListSizeField();
        }

        public void ClearItemCache()
        {
            Debug.Print("Clearing _itemCache completely.");
            _itemCache.Clear();
        }

        public void ClearItemCache(int startRowIndex)
        {
            Debug.Print("Clearing _itemCache starting with " + startRowIndex);

            if (_itemCache.Any())
            {
                if (startRowIndex <= _firstItemIndex)
                {
                    Debug.Print("Removing all cached items.");
                    _itemCache.Clear();

                }
                else
                {
                    int startRemove = startRowIndex - _firstItemIndex;
                    int lengthRemove = _itemCache.Count - startRemove;

                    if (startRemove < _itemCache.Count)
                    {
                        Debug.Print("Removing {0} cached items.", lengthRemove);
                        _itemCache.RemoveRange(startRemove, lengthRemove);
                    }
                }
            }
        }

        // Setting the virtual list size this way eliminates flickering.
        // It only works properly when new items are added to the end.
        // This idea came from http://www.dotnetmonster.com/Uwe/Forum.aspx/winform-controls/5181/Ugly-flicker-when-updating-a-ListView-in-VirtualMode
        public void SetVirtualListSizeWithoutRefresh(int count)
        {
            if (listViewVirtualListSizeField == null)
            {
                VirtualListSize = count;
            }
            else
            {
                try
                {
                    SendMessage(Handle,
                        ListViewMessages.LVM_SETITEMCOUNT,
                        count,
                        ListViewSetItemCountFlags.LVSICF_NOINVALIDATEALL |
                        ListViewSetItemCountFlags.LVSICF_NOSCROLL);

                    // The putlic ListView.VirtualListSize property drives a private member 
                    // named virtualListSize that is used in the implementation of 
                    // ListViewItemCollection (returned by ListView.Items) to validate
                    // indices. If this is not updated, spurious ArgumentOutOfRangeExceptions
                    // may be raised by functions and properties using the indexing
                    // operator on ListView.Items, for instance FocusedItem.
                    listViewVirtualListSizeField.SetValue(this, count);
                }
                catch
                {
                    VirtualListSize = count;
                }
            }

        }

        // Set the backcolor and forecolor of items in the cache so selected items
        // remain prominent even when the form loses focus.  I tried just setting
        // HideSelection to false, but the items become gray instead of highlighted
        // and the gray is nearly invisible on some monitors.
        // Called when selection(s) change.
        public void SetItemCacheColors(bool formActive)
        {
            //Debug.Print("formActive = " + formActive);
            if (_itemCache.Any())
            {
                BeginUpdate();
                foreach (ViewItem item in _itemCache) item.SetItemColors(formActive);
                EndUpdate();
            }
        }

        protected override void OnCacheVirtualItems(CacheVirtualItemsEventArgs e)
        {
            // Only recreate the cache if we need to.

            if (!_itemCache.Any() || e.StartIndex < _firstItemIndex || e.EndIndex > _firstItemIndex + _itemCache.Count - 1)
            {
                bool newItem;
                List<ViewItem> newItems = new List<ViewItem>(e.EndIndex - e.StartIndex + 1);

                Debug.Print("Building item cache " + e.StartIndex + " - " + e.EndIndex);

                // Do not alter _itemCache or _firstItemIndex until we're done
                // calling GetListItem(), since it references them.
                // This may reuse some existing ViewItems in _itemCache.
                for (int i = e.StartIndex; i <= e.EndIndex; ++i)
                {
                    newItems.Add(GetListItem(i, out newItem));
                }

                _itemCache = newItems;
                _firstItemIndex = e.StartIndex;
            }

            base.OnCacheVirtualItems(e); // Probably unnecessary.
        }

        protected override void OnRetrieveVirtualItem(RetrieveVirtualItemEventArgs e)
        {
            base.OnRetrieveVirtualItem(e);

            bool newItem;
            ViewItem item = GetListItem(e.ItemIndex, out newItem);
            e.Item = item;

            // When the main form is not active, the blue highlighting for selected items disappears.
            // To prevent that, we explicitly set the item's colors.  The selected items (and 
            // the scroll position) can change
            // while the form is not active (e.g. when the user clicks Find Next in the find dialog),
            // so it is not sufficient to just set the colors in the Activated and Deactivated events.
            // New items are created with the correct colors, so we only call SetItemColors for
            // existing items.
            if (!newItem && MainForm != Form.ActiveForm) item.SetItemColors(false);
        }

        private ViewItem GetListItem(int i, out bool newItem)
        {
            // If we have the item cached, return it. Otherwise, recreate it.
            if (_itemCache.Any() &&
                i >= _firstItemIndex &&
                i < _firstItemIndex + _itemCache.Count)
            {
                //Debug.Print("Returning cached item " + i);
                newItem = false;
                return _itemCache[i - _firstItemIndex];
            }
            else
            {
                // Create a new item.
                newItem = true;

                if (i == 0)
                {
                    return MainForm.Rows[i].MakeItem(null);
                }
                else
                {
                    return MainForm.Rows[i].MakeItem(MainForm.Rows[i - 1]);
                }
            }
        }

        [Flags]
        private enum ListViewSetItemCountFlags
        {
            //#if (_WIN32_IE >= 0x0300)
            // these flags only apply to LVS_OWNERDATA listviews in report or list mode
            LVSICF_NOINVALIDATEALL = 0x00000001,
            LVSICF_NOSCROLL = 0x00000002,
            //#endif
        }

        private enum ListViewMessages
        {
            LVM_FIRST = 0x1000,      // ListView messages
            LVM_SETITEMCOUNT = (LVM_FIRST + 47),
        }

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, ListViewMessages msg, int wParam, ListViewSetItemCountFlags lParam);

        private FieldInfo GetVirtualListSizeField()
        {
            FieldInfo result = typeof(ListView).GetField("virtualListSize", BindingFlags.Instance | BindingFlags.NonPublic);
            //FieldInfo result = GetType().GetField("virtualListSize", BindingFlags.Instance | BindingFlags.NonPublic);
            Debug.Assert(result != null, "System.Windows.Forms.ListView class no longer has a virtualListSize field.");
            return result;
        }
    }
}
