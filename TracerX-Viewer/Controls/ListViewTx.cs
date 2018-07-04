using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;

namespace TracerX
{
    /// <summary>
    /// Slightly customized version of ListView
    /// </summary>
    internal partial class ListViewTx : ListView
    {
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

            //OwnerDraw = true;
        }

        private static readonly bool _isVistaOrHigher = (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major >= 6);

        // This improves page-scrolling performance, hurts line-scrolling,
        // and eliminates the weird half-drawn lines that appear while scrolling.
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (_isVistaOrHigher && Properties.Settings.Default.UseFastScrollingKluge) cp.ExStyle |= 0x02000000; // Causes CPU to peg on XP
                return cp;
            }
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            //base.OnDrawColumnHeader(e);

            //using (StringFormat sf = new StringFormat())
            //{
            //    // Store the column text alignment, letting it default
            //    // to Left if it has not been set to Center or Right.
            //    switch (e.Header.TextAlign)
            //    {
            //        case HorizontalAlignment.Center:
            //            sf.Alignment = StringAlignment.Center;
            //            break;
            //        case HorizontalAlignment.Right:
            //            sf.Alignment = StringAlignment.Far;
            //            break;
            //    }

            //    // Draw the standard header background.
            //    e.DrawBackground();
                
            //    // Draw the header text.
            //    using (Font headerFont =
            //                new Font("Helvetica", 10, FontStyle.Bold))
            //    {
            //        e.Graphics.DrawString(e.Header.Text, headerFont,
            //            Brushes.Black, e.Bounds, sf);
            //    }
            //}
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            Debug.WriteLine("OnDrawItem {0}", e.ItemIndex);
            if ((e.State | ListViewItemStates.Selected) == ListViewItemStates.Selected)
            {
                Debug.WriteLine("Found a selected item.");
                e.DrawFocusRectangle();
                e.DrawDefault = false;
            }
            else
            {
                e.DrawDefault = false;
            }
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            Debug.WriteLine("OnDrawSubItem {0} - {1}", e.ColumnIndex, e.SubItem.Text ?? "<null>");
            //if ((e.ItemState | ListViewItemStates.Selected) == ListViewItemStates.Selected)
            //{
            //    Debug.WriteLine("Found a selected item.");
            //    // Draw the background and focus rectangle for a selected item.
            //    e.Graphics.FillRectangle(Brushes.Maroon, e.Bounds);
            //}
            //else
            //{
            //    e.Graphics.FillRectangle(Brushes.Maroon, e.Bounds);
            //}

            //if (e.SubItem.BackColor == Color.Empty)
            //{
            //    e.DrawDefault = true;
            //}
            //else
            //{
            //    e.Graphics.FillRectangle(new SolidBrush(e.SubItem.BackColor), e.Bounds);
            //    e.DrawText(TextFormatFlags.Left);
            //}

            e.DrawDefault = true;

        }

        // Forces each row to repaint itself the first time the mouse moves over 
        // it, compensating for an extra DrawItem event sent by the wrapped 
        // Win32 control. This issue occurs each time the ListView is invalidated.
        protected override void OnMouseMove(MouseEventArgs e)
        {
            ListViewItem item = GetItemAt(e.X, e.Y);
            if (item != null && item.Tag == null)
            {
                Invalidate(item.Bounds);
            }
        }


        // Forces the entire control to repaint if a column width is changed.
        protected override void OnColumnWidthChanged(ColumnWidthChangedEventArgs e)
        {
            Invalidate();
        }

        
        public MainForm MainForm;

        /// <summary>
        /// Gets and Sets the Horizontal Scroll position of the control.
        /// </summary>
        public int HScrollPos
        {
            get { return GetScrollPos(this.Handle, System.Windows.Forms.Orientation.Horizontal); }
            set { SetScrollPos(this.Handle, System.Windows.Forms.Orientation.Horizontal, value, true); }
        }

        public void SetHScrollPos(int pos)
        {
            SetScrollPos(this.Handle, System.Windows.Forms.Orientation.Horizontal, pos, true);
        }

        // Cache of ListViewItems used to improve the performance of the ListView control
        // in virtual mode (it tends to request the same item many times).
        private List<ViewItem> _itemCache = new List<ViewItem>(50);

        // Index of first item in _itemCache.
        private int _firstItemIndex = -1;

        // This lets us use reflection to update the ListView's private virtualListSize field, 
        // bypassing the VirtualListSize property.  This reduces flicker when the ListView is
        // updated when new records are read from the file.
        private FieldInfo listViewVirtualListSizeField;

        public void ClearItemCache()
        {
            _itemCache.Clear();
        }

        public void ClearItemCache(int startRowIndex)
        {
            if (_itemCache.Any())
            {
                if (startRowIndex <= _firstItemIndex)
                {
                    _itemCache.Clear();

                }
                else
                {
                    int startRemove = startRowIndex - _firstItemIndex;
                    int lengthRemove = _itemCache.Count - startRemove;

                    if (startRemove < _itemCache.Count)
                    {
                        _itemCache.RemoveRange(startRemove, lengthRemove);
                    }
                }
            }
        }

        // Setting the virtual list size this way eliminates flickering (when invalidate = false)
        // and prevents the control from scrolling to the right.
        // This idea came from http://www.dotnetmonster.com/Uwe/Forum.aspx/winform-controls/5181/Ugly-flicker-when-updating-a-ListView-in-VirtualMode
        public void SetVirtualListSize(int count, bool invalidate)
        {
            if (listViewVirtualListSizeField == null)
            {
                VirtualListSize = count;
            }
            else
            {
                try
                {
                    ListViewSetItemCountFlags flags = ListViewSetItemCountFlags.LVSICF_NOSCROLL;

                    if (!invalidate)
                    {
                        flags |= ListViewSetItemCountFlags.LVSICF_NOINVALIDATEALL;
                    }

                    SendMessage(Handle, ListViewMessages.LVM_SETITEMCOUNT, count, flags);

                    // The public ListView.VirtualListSize property drives a private member 
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
            LVM_SCROLL = (LVM_FIRST + 20),
        }

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, ListViewMessages msg, int wParam, ListViewSetItemCountFlags lParam);

        [DllImport("user32.dll")]
        static extern int GetScrollPos(IntPtr hWnd, System.Windows.Forms.Orientation nBar);

        [DllImport("user32.dll")]
        public static extern int SetScrollPos(IntPtr hWnd, System.Windows.Forms.Orientation nBar, int nPos, bool bRedraw);

        private FieldInfo GetVirtualListSizeField()
        {
            FieldInfo result = typeof(ListView).GetField("virtualListSize", BindingFlags.Instance | BindingFlags.NonPublic);
            //FieldInfo result = GetType().GetField("virtualListSize", BindingFlags.Instance | BindingFlags.NonPublic);
            Debug.Assert(result != null, "System.Windows.Forms.ListView class no longer has a virtualListSize field.");
            return result;
        }
    }
}
