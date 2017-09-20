using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using TracerX.Properties;

namespace TracerX
{
    internal partial class FindDialog : Form
    {
        public FindDialog(MainForm mainForm)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.scroll_view;
            _mainForm = mainForm;

            if (Settings.Default.FindStrings != null)
            {
                foreach (string str in Settings.Default.FindStrings)
                {
                    comboBox1.Items.Add(str);
                }
            }

            if (mainForm.FocusedRow != null) comboBox1.Text = mainForm.FocusedRow.ToString();
            comboBox1.SelectAll();
            comboBox1.Focus();
            comboBox1.Select();

            _minWidth = Width;
        }

        private MainForm _mainForm;
        private int _minWidth;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            wildHelpLabel.Visible = true;
            wildHelpLabel.VisibleChanged += wildHelpLabel_VisibleChanged;
            wildHelpLabel.Visible = false;
        }

        private void DoSearch(bool bookmark)
        {
            Cursor restoreCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            UpdateComboList();

            try
            {
                MatchType matchType = MatchType.Simple;

                if (wildcardRad.Checked)
                {
                    matchType = MatchType.Wildcard;
                }
                else if (regexRad.Checked)
                {
                    matchType = MatchType.RegularExpression;
                }

                StringMatcher matcher = new StringMatcher(comboBox1.Text, matchCase.Checked, matchType);

                _mainForm.DoSearch(matcher, searchUp.Checked, bookmark);
            }
            finally
            {
                this.Cursor = restoreCursor;
            }
        }

        private void UpdateComboList()
        {
            string str = comboBox1.Text;
            if (comboBox1.Items.Contains(str))
            {
                // Move the found text to the top of the list.
                comboBox1.Items.Remove(str);
            }

            comboBox1.Items.Insert(0, str);
            comboBox1.Text = str;

            // Allow no more than 10 search strings to be saved.
            while (comboBox1.Items.Count > 10) comboBox1.Items.RemoveAt(10);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Persist the list of search strings.
            Settings.Default.FindStrings = new System.Collections.Specialized.StringCollection();
            foreach (string str in comboBox1.Items)
            {
                Settings.Default.FindStrings.Add(str);
            }
        }

        private void findNext_Click(object sender, EventArgs e)
        {
            DoSearch(false);
        }

        private void bookmarkAll_Click(object sender, EventArgs e)
        {
            DoSearch(true);
        }

        private void close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void wildHelpLabel_VisibleChanged(object sender, EventArgs e)
        {
            int newHeight = this.Height;
            Size min = MinimumSize;
            Size max = MaximumSize;

            if (wildHelpLabel.Visible)
            {
                newHeight += wildHelpLabel.Height;

                max.Height = newHeight;
                MaximumSize = max;

                Height = newHeight;

                min.Height = newHeight;
                MinimumSize = min;
            }
            else
            {
                newHeight -= wildHelpLabel.Height;

                min.Height = newHeight;
                MinimumSize = min;

                Height = newHeight;

                max.Height = newHeight;
                MaximumSize = max;
            }
        }

        private void wildcardRad_CheckedChanged(object sender, EventArgs e)
        {
            wildHelpLabel.Visible = wildcardRad.Checked;
        }
    }
}