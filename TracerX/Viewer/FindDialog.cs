using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace TracerX.Viewer {
    internal partial class FindDialog : Form {
        public FindDialog() {
            InitializeComponent();
            this.Icon = Properties.Resources.scroll_view;
        }

        public FindDialog(MainForm mainForm) {
            InitializeComponent();
            this.Icon = Properties.Resources.scroll_view;
            _mainForm = mainForm;

            if (Settings1.Default.FindStrings != null) {
                foreach (string str in Settings1.Default.FindStrings) {
                    comboBox1.Items.Add(str);
                }
            }

            comboBox1.Text = mainForm.FocusedRow.ToString();
            comboBox1.SelectAll();
            comboBox1.Focus();
            comboBox1.Select();
        }

        private MainForm _mainForm;

        private void DoSearch(bool bookmark) {
            Cursor restoreCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            UpdateComboList();

            try {
                StringComparison sc = StringComparison.CurrentCultureIgnoreCase;
                RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;
                Regex regex = null;

                if (matchCase.Checked) {
                    sc = StringComparison.CurrentCulture;
                    regexOptions = RegexOptions.Compiled;
                }

                if (useWildcards.Checked) {
                    regex = new Regex(FilterDialog.WildcardToRegex(comboBox1.Text), regexOptions);
                }

                _mainForm.DoSearch(comboBox1.Text, sc, regex, searchUp.Checked, bookmark);
            } finally {
                this.Cursor = restoreCursor;
            }
        }

        private void UpdateComboList() {
            string str = comboBox1.Text;
            if (comboBox1.Items.Contains(str)) {
                // Move the found text to the top of the list.
                comboBox1.Items.Remove(str);
            }
             
            comboBox1.Items.Insert(0, str);
            comboBox1.Text = str;

            // Allow no more than 10 search strings to be saved.
            while (comboBox1.Items.Count > 10) comboBox1.Items.RemoveAt(10);
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            //if (_foundItem != null) {
            //    _foundItem.SimulateSelected(false);
            //}

            // Persist the list of search strings.
            Settings1.Default.FindStrings = new System.Collections.Specialized.StringCollection();
            foreach (string str in comboBox1.Items) {
                Settings1.Default.FindStrings.Add(str);
            }
        }

        private void findNext_Click(object sender, EventArgs e) {
            DoSearch(false);
        }

        private void bookmarkAll_Click(object sender, EventArgs e) {
            DoSearch(true);
        }

        private void close_Click(object sender, EventArgs e) {
            Close();
        }
    }
}