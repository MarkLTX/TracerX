using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace TracerX
{
    /// <summary>
    /// A general-purpose column selector that works with any DataGridView.
    /// </summary>
    public partial class FormDataGridViewColumnSelector : Form
    {
        private DataGridView _grid;

        /// <summary>
        /// This is what you must call to use this class.
        /// The ctor is private.
        /// </summary>
        public static DialogResult ShowModal(string title, DataGridView grid, params DataGridViewColumn[] omittedCols)
        {
            var form = new FormDataGridViewColumnSelector();
            form.StartPosition = FormStartPosition.CenterParent;
            form.Text = title;
            form.Init(grid, omittedCols);
            return form.ShowDialog(grid);
        }

        private FormDataGridViewColumnSelector()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.scroll_view;
            listView1.Items.Clear();
        }

        private void Init(DataGridView grid, params DataGridViewColumn[] omittedCols)
        {
            _grid = grid;

            // First create an array of columns in display order.
            var displayOrder = new DataGridViewColumn[grid.Columns.Count];

            foreach (DataGridViewColumn col in _grid.Columns)
            {
                    displayOrder[col.DisplayIndex] = col;
            }

            // Add the columns to the list control in display order.
            foreach (DataGridViewColumn col in displayOrder)
            {
                if (!omittedCols.Contains(col))
                {
                    ListViewItem item = listView1.Items.Add(col.HeaderText);
                    item.Checked = col.Visible;
                    item.Tag = col;
                }
            }

            if (!grid.AllowUserToOrderColumns)
            {
                upBtn.Visible = false;
                downBtn.Visible = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            // Set the form height to accomodate all columns if reasonable.
            if (listView1.Items.Count > 1)
            {
                int itemHeight = listView1.Items[0].Bounds.Height;
                int newHeight = Height + ((listView1.Items.Count + 1) * itemHeight) - listView1.Height;

                if (newHeight > Height)
                {
                    this.Height = Math.Min(newHeight, 700);
                }
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            base.OnLoad(e);
        }

        private void checkAllBtn_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items) item.Checked = true;
        }

        private void uncheckAllBtn_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items) item.Checked = false;
        }

        private void invertBtn_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items) item.Checked = !item.Checked;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selCount = listView1.SelectedItems.Count;
            
            upBtn.Enabled =  _grid.AllowUserToOrderColumns && selCount > 0 && !listView1.Items[0].Selected;
            downBtn.Enabled = _grid.AllowUserToOrderColumns && selCount > 0 && !listView1.Items[listView1.Items.Count - 1].Selected;
        }

        private void upBtn_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                int ndx = item.Index;
                item.Remove();
                listView1.Items.Insert(ndx - 1, item);
            }
        }

        private void downBtn_Click(object sender, EventArgs e)
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

        private void okBtn_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count == 0)
            {
                MainForm.ShowMessageBox("You must check at least one column.");
                DialogResult = DialogResult.None;
            }
            else
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    DataGridViewColumn col = (DataGridViewColumn)item.Tag;
                    col.Visible = item.Checked;
                    col.DisplayIndex = item.Index;
                }
            }
        }
    }
}
