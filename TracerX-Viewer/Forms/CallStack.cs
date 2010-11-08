using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TracerX.Viewer {
    // This form displays the call stack.
    internal partial class CallStack : Form {
        public CallStack() {
            InitializeComponent();
        }

        public CallStack(Row top, List<Record> stack) {
            InitializeComponent();

            _topStackRow = top;
            ListViewItem item;

            foreach (Record rec in stack) {
                item = new ListViewItem(rec.MsgNum.ToString());
                if (rec.IsEntry) item.SubItems.Add(rec.MethodName.Name);
                else item.SubItems.Add(rec.Lines[0]);
                item.Tag = rec;
                if (!rec.IsVisible) item.BackColor = Color.LightGray;
                listView1.Items.Add(item);
            }

            item = new ListViewItem(top.Rec.GetRecordNum(top.Line));
            if (top.Rec.IsEntry) item.SubItems.Add(top.Rec.MethodName.Name);
            else item.SubItems.Add(top.ToString());
            listView1.Items.Add(item);
        }        

        private Row _selectedRow;
        private Row _topStackRow;

        // When the user selects an item, scroll the main form to the corresponding row.
        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            if (listView1.SelectedItems.Count > 0) {
                ListViewItem item = listView1.SelectedItems[0];
                Record rec = item.Tag as Record;

                if (rec == null) {
                    _selectedRow = MainForm.TheMainForm.SelectRowIndex(_topStackRow.Index);
                } else if (rec.IsVisible) {
                    _selectedRow = MainForm.TheMainForm.SelectRowIndex(rec.FirstRowIndex);
                }
            }
        }
    }

}