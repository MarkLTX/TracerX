using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TracerX.Viewer {
    internal partial class CallStack : Form {
        public CallStack() {
            InitializeComponent();
        }

        public CallStack(Row top, List<Record> stack) {
            InitializeComponent();

            _topStackRow = top;
            ListViewItem item = new ListViewItem(top.Rec.GetRecordNum(top.Line));
            item.SubItems.Add(top.ToString());
            listView1.Items.Add(item);

            foreach (Record rec in stack) {
                item = new ListViewItem(rec.MsgNum.ToString());
                item.SubItems.Add(rec.Lines[0]);
                item.Tag = rec;
                if (!rec.IsVisible) item.BackColor = Color.LightGray;
                listView1.Items.Add(item);
            }
        }        

        private Row _selectedRow;
        private Row _topStackRow;

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);

            //if (_selectedRow != null) {
            //    _selectedRow.SimulateSelected(false);
            //}
        }

        // When the user selects an item, scroll the main form to the corresponding row.
        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            if (listView1.SelectedItems.Count > 0) {
                ListViewItem item = listView1.SelectedItems[0];
                Record rec = item.Tag as Record;

                if (rec == null) {
                    //if (_selectedRow != null) {
                    //    _selectedRow.SimulateSelected(false);
                    //}
                    _selectedRow = MainForm.TheMainForm.SelectSingleItem(_topStackRow.Index);
                    //_selectedRow.SimulateSelected(true);
                } else if (rec.IsVisible) {
                    //if (_selectedRow != null) {
                    //    _selectedRow.SimulateSelected(false);
                    //}
                    _selectedRow = MainForm.TheMainForm.SelectSingleItem(rec.FirstRowIndex);
                    //_selectedRow.SimulateSelected(true);
                }
            }
        }
    }

}