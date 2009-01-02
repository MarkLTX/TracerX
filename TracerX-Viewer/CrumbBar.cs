using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using TracerX.Viewer;
using System.Diagnostics;

namespace TracerX {
    internal partial class CrumbBar : UserControl {
        public CrumbBar() {
            InitializeComponent();
            _autoRepeatTimer.Tick += _autoRepeatTimer_Tick;
            _autoRepeatTimer.Interval = 15;
        }

        // Used to keep the crumb bar intact when the user navigates via the crumb bar.
        private bool _keepCrumbBar;

        // The Row from which the stack originates.  Special because it may not be a
        // MethodEntry Row, and may correspond to a subline of a Record.
        private Row _origin;

        // All Records from the log file.
        private List<Record> _records;

        // The linkLabel's scroll position is <= 0.
        private int _scrollPos;

        // Timer for repeated scrolling while mouse button is held down.
        Timer _autoRepeatTimer = new Timer();

        // Amount to scroll depends on whether scroll event is from single-click
        // or from auto-repeat timer.
        int _scrollAmount;

        public void Clear() {
            linkLabel1.Links.Clear();
            linkLabel1.Text = null;
            _scrollPos = 0;
        }

        // Puts a list of links in the crumb bar, representing the current call stack.
        // Only works with file version 5+ because Record.Caller is always null for
        // earlier versions.
        public void BuildCrumbBar(Row origin, List<Record> records) {
            // Leave the crumb bar alone if _keepCrumbBar is true.
            if (_keepCrumbBar) return;

            Clear();
            _origin = origin;
            _records = records;

            if (origin == null) return;

            StringBuilder builder = new StringBuilder();

            foreach (Record rec in GetCallStack(origin)) {
                AddToCrumbBar(builder, rec, 0);
            }

            // Now include the current row.
            AddToCrumbBar(builder, origin.Rec, origin.Line);

            // Setting the text changes the size (because AutoSize is true)
            // and therefore calls the Resize handler.
            linkLabel1.Text = builder.ToString();

            // Ensure the right end of the text is visible.
            int rightEdge = rightBtn.Visible ? rightBtn.Left : this.Width;
            if (linkLabel1.Right > rightEdge) {
                _scrollPos -= linkLabel1.Right - rightEdge;
                SetLocation();
            }
        }

        // Handler for both this and this.linkLabel1
        private void CrumbBar_Resize(object sender, EventArgs e) {
            if (linkLabel1.Width > this.Width) {
                leftBtn.Visible = true;
                rightBtn.Visible = true;

                // Don't allow any space between the LinkLabel and the right button.
                if (rightBtn.Left > linkLabel1.Right) _scrollPos += rightBtn.Left - linkLabel1.Right;
            } else {
                leftBtn.Visible = false;
                rightBtn.Visible = false;
                _scrollPos = 0;
            }

            SetLocation();
        }

        // Sets the location of the LinkLabel based on _scrollPos.
        private void SetLocation() {
            if (leftBtn.Visible) linkLabel1.Left = _scrollPos + leftBtn.Right;
            else linkLabel1.Left = _scrollPos;

            leftBtn.Enabled = linkLabel1.Left < leftBtn.Right;
            rightBtn.Enabled = linkLabel1.Right > rightBtn.Left;
        }

        // Adds a link to the crumbBar for the specified record.  Adds appropriate text
        // (method name or line number) to the StringBuilder.  The lineNum parameter is
        // only relevant if the Record is not a MethodEntry record.
        private void AddToCrumbBar(StringBuilder builder, Record rec, int lineNum) {
            string crumb;

            if (rec.IsEntry) {
                crumb = rec.MethodName;
            } else {
                crumb = string.Format("Line {0}", rec.GetRecordNum(lineNum));
            }

            LinkLabel.Link link = new LinkLabel.Link(builder.Length, crumb.Length, rec);
            link.Enabled = rec.IsVisible && rec.IsEntry;
            link.Name = "M"; // Anything but string.Empty.
            linkLabel1.Links.Add(link);
            builder.Append(crumb);

            // We always add a separator arrow after each method (even the last one),
            // which the user can click to get a list of methods called by the method.
            if (rec.IsEntry) {
                linkLabel1.Links.Add(builder.Length + 1, 2, rec);
                builder.Append(" -> ");
            }
        }

        // Gets the call stack leading to the current record, not including the current record.  
        // All Records returned will be MethodEntry Records.  Result is empty if CurrentRow is null or 
        // has no caller.
        private List<Record> GetCallStack(Row origin) {
            List<Record> result = new List<Record>();

            if (origin != null) {
                Record caller = origin.Rec.Caller;

                while (caller != null) {
                    result.Add(caller);
                    caller = caller.Caller;
                }

                result.Reverse();
            }

            return result;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (e.Link.Name == string.Empty) {
                ShowCalledMethodsForCrumbBarLink(e.Link);
            } else {
                SelectRowForCrumbBarLink(e.Link);
            }
        }

        // Called when the user clicks an arrow in the crumbBar.  This displays a
        // list of methods called by the method to the left of the arrow.  If the 
        // user selects one, we navigate to the the corresponding Record/Row in TheListView.
        private void ShowCalledMethodsForCrumbBarLink(LinkLabel.Link clickedLink) {
            Record methodRecord = (Record)clickedLink.LinkData;
            string lastMethod = null;
            int sequentialCount = 1;
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem menuItem = null;

            // Scan records starting after the record whose called methods we want.
            // Stop when we run out of records, reach the end of method whose called
            // records we want (based on StackDepth), or put 30 items in the context menu.
            for (int i = methodRecord.Index + 1;
                i < _records.Count && _records[i].StackDepth > methodRecord.StackDepth && menu.Items.Count < 30;
                ++i) //
            {
                if (_records[i].IsEntry && _records[i].Caller == methodRecord) {
                    if (_records[i].MethodName == lastMethod) {
                        // There may be many sequential calls to the same method.  Instead of creating
                        // a MenuItem for each, count the calls and include the count in a single 
                        // MenuItem's Text.
                        ++sequentialCount;
                    } else {
                        if (sequentialCount > 1) {
                            menuItem.Text = string.Format("{0} ({1} calls)", lastMethod, sequentialCount);
                            sequentialCount = 1;
                        }

                        lastMethod = _records[i].MethodName;
                        menuItem = new ToolStripMenuItem(lastMethod, null, CrumbBarMenuItemClicked);
                        menuItem.Enabled = _records[i].IsVisible;
                        menuItem.Tag = _records[i];
                        menuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
                        menu.Items.Add(menuItem);
                    }
                }
            }

            if (sequentialCount > 1) {
                menuItem.Text = string.Format("{0} ({1} calls)", lastMethod, sequentialCount);
            }

            menu.ShowCheckMargin = false;
            menu.ShowImageMargin = false;
            menu.ShowItemToolTips = false;

            if (menu.Items.Count == 0) {
                //clickedLink.Enabled = false;
                menuItem = new ToolStripMenuItem("No calls");
                menuItem.Enabled = false;
                menu.Items.Add(menuItem);
                //linkLabel1.Text = linkLabel1.Text.Remove(clickedLink.Start);
                linkLabel1.Links.Remove(clickedLink);
            }

            menu.Show(this, this.PointToClient(Control.MousePosition));
        }

        void CrumbBarMenuItemClicked(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            Record rec = (Record)menuItem.Tag;
            MainForm.TheMainForm.SelectSingleRow(rec.RowIndices[0]);
        }

        // Called when the user clicks a method name in the crumbBar.  This
        // selects the corresponding Record/Row in TheListView.
        private void SelectRowForCrumbBarLink(LinkLabel.Link clickedLink) {
            try {
                // Don't build a new crumbBar when we change the currently seleted row.
                _keepCrumbBar = true;

                // The stack origin (last entry in the stack, last link in the crumbBar) is
                // special because that linkRecord may be a Record that is expanded into
                // multiple Rows, and we need to select the right one of those Rows.
                Record linkRecord = (Record)clickedLink.LinkData;

                if (linkRecord == _origin.Rec) {
                    MainForm.TheMainForm.SelectSingleRow(_origin.Index);
                } else {
                    MainForm.TheMainForm.SelectSingleRow(linkRecord.RowIndices[0]);
                }

                // Disable links for invisible records.
                foreach (LinkLabel.Link link in linkLabel1.Links) {
                    linkRecord = link.LinkData as Record;
                    if (linkRecord != null) link.Enabled = linkRecord.IsVisible;
                }

                // Disable the link for the record we just selected.
                clickedLink.Enabled = false;
            } finally {
                _keepCrumbBar = false;
            }
        } 

        private void leftBtn_Paint(object sender, PaintEventArgs e) {
            // Draw a triangle pointing to the left.
            Brush brush = leftBtn.Enabled ? Brushes.Black : Brushes.DarkGray;
            const int margin = 3;

            Point p1 = new Point(leftBtn.Width - margin, margin);
            Point p2 = new Point(leftBtn.Width - margin, leftBtn.Height - margin);
            Point p3 = new Point(margin, leftBtn.Height / 2);
            Point[] points = new Point[] { p1, p2, p3 };
            e.Graphics.FillPolygon(brush, points);
        }

        private void rightBtn_Paint(object sender, PaintEventArgs e) {
            // Draw a triangle pointing to the right.
            Brush brush = rightBtn.Enabled ? Brushes.Black : Brushes.DarkGray;
            const int margin = 3;

            Point p1 = new Point(margin, margin);
            Point p2 = new Point(margin, rightBtn.Height - margin);
            Point p3 = new Point(rightBtn.Width - margin, rightBtn.Height / 2);
            Point[] points = new Point[] { p1, p2, p3 };
            e.Graphics.FillPolygon(brush, points);
        }

        // Handler for BOTH buttons.
        private void leftBtn_EnabledChanged(object sender, EventArgs e) {
            // Cause the changed button to be repainted.
            if (sender == leftBtn) leftBtn.Invalidate();
            else rightBtn.Invalidate();

            if (!((Button)sender).Enabled) _autoRepeatTimer.Stop();
        }

        // MouseDown handler for BOTH buttons.
        // Starts a Timer to scroll repeatedly while mouse button is down.
        private void Btn_MouseDown(object sender, MouseEventArgs e) {
            Debug.Print("Down");
            // Set the Tag to the button so the Tick handler knows which direction to scroll.
            _autoRepeatTimer.Tag = sender;
            _autoRepeatTimer.Start();
        }

        // MouseUp handler for BOTH buttons.
        private void Btn_MouseUp(object sender, MouseEventArgs e) {
            Debug.Print("Up");
            _autoRepeatTimer.Stop();
        }

        void _autoRepeatTimer_Tick(object sender, EventArgs e) {
            Debug.Print("Tick");
            Button btn = (Button)_autoRepeatTimer.Tag;
            const int scrollAmount = 6;

            // If the mouse button is still down on the scroll button, scroll.
            if (btn.RectangleToScreen(btn.ClientRectangle).Contains(Control.MousePosition)) {
                if (btn.Enabled) {
                    if (btn == leftBtn) {
                        int delta = Math.Min(scrollAmount, leftBtn.Right - linkLabel1.Left);
                        _scrollPos += delta;
                    } else {
                        int delta = Math.Min(scrollAmount, linkLabel1.Right - rightBtn.Left);
                        _scrollPos -= delta;
                    }

                    SetLocation();
                }
            } else {
                _autoRepeatTimer.Stop();
            }
        }
    }
}
