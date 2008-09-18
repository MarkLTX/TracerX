using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TracerX.Viewer {
    internal partial class FullText : Form {
        public FullText(string text) {
            InitializeComponent();
            this.Icon = Properties.Resources.scroll_view;
            textBox1.Text = text;

            _selectedIndexChanged = new EventHandler(SelectionChanged);
        }

        // Count of instances with the following checkbox checked.
        private static int following = 0;

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            // Clear the text selection.
            textBox1.SelectionStart = 0;

            Wrap.Checked = true;

            // New instances don't follow the current line if 
            // another instance already is.
            if (following == 0) follow.Checked = true;
        }

        protected override void OnClosing(CancelEventArgs e) {
            if (follow.Checked) --following;
            base.OnClosing(e);
        }

        private void edit_CheckedChanged(object sender, EventArgs e) {
            textBox1.ReadOnly = !edit.Checked;
        }

        private void Wrap_CheckedChanged(object sender, EventArgs e) {
            textBox1.WordWrap = Wrap.Checked;
        }

        private void follow_CheckedChanged(object sender, EventArgs e) {
            if (follow.Checked) {
                ++following;
                SelectionChanged(null, null);
                MainForm.TheMainForm.TheListView.SelectedIndexChanged += _selectedIndexChanged;
            } else {
                --following;
                MainForm.TheMainForm.TheListView.SelectedIndexChanged -= _selectedIndexChanged;
            }
        }

        private void copy_Click(object sender, EventArgs e) {
            Clipboard.SetText(textBox1.Text);
        }

        private void ok_Click(object sender, EventArgs e) {
            Close();
        }

        private EventHandler _selectedIndexChanged;

        private void SelectionChanged(object sender, EventArgs e) {
            Row row = MainForm.TheMainForm.FocusedRow;
            this.textBox1.Text = row.Rec.GetTextForWindow(row.Line);
        }
    }
}