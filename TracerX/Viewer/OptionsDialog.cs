using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TracerX.Viewer {
    public partial class OptionsDialog : Form {
        public OptionsDialog() {
            InitializeComponent();

            this.Icon = Properties.Resources.scroll_view;
            InitLine();
            InitTime();
            InitText();
            InitAutoRefresh();
            InitVersionCheck();
        }

        public OptionsDialog(ColumnHeader header) : this() {
            if (header == MainForm.TheMainForm.headerTime) {
                tabControl1.SelectedTab = timePage;
            } else if (header == MainForm.TheMainForm.headerLine) {
                tabControl1.SelectedTab = linePage;
            } else if (header == MainForm.TheMainForm.headerText) {
                tabControl1.SelectedTab = textPage;
            }
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            ok.Enabled = false;
            apply.Enabled = false;
        }

        private void SomethingChanged(object sender, EventArgs e) {
            ok.Enabled = true;
            apply.Enabled = true;

            if (indentChar.Text == string.Empty)
                hex.Text = "An indentation character is required.";
            else
                hex.Text = string.Format("0x{0:X2}", (int)indentChar.Text[0]);        
        }

        private void ok_Click(object sender, EventArgs e) {
            if (ApplyChanges()) {
                DialogResult = DialogResult.OK;
            } else {
                DialogResult = DialogResult.None;
            }
        }

        private void apply_Click(object sender, EventArgs e) {
            ApplyChanges();
        }

        private bool ApplyChanges() {
            if (VerifyText(true) && VerifyAutoRefresh(true) && VerifyVersionCheck(true)) {
                ApplyLine();
                ApplyTime();
                ApplyText();
                ApplyAutoRefresh();
                ApplyVersionCheck();

                // We're not hiding or showing anything, so don't call RebuildRows.
                // Just make sure any visible rows get redrawn.
                MainForm.TheMainForm.InvalidateTheListView();

                ok.Enabled = false;
                apply.Enabled = false;

                return true;
            } else {
                return false;
            }
        }

        private void tabControl1_Deselecting(object sender, TabControlCancelEventArgs e) {
            // Don't let the user leave the page if there is an error on the page.
            // Currently, only the text page requires verification.
            if (e.TabPage == textPage) {
                e.Cancel = !VerifyText(true);
            } else if (e.TabPage == autoRefreshPage) {
                e.Cancel = !VerifyAutoRefresh(true);
            } else if (e.TabPage == versionPage) {
                e.Cancel = !VerifyVersionCheck(true);
            }
        }

        #region Line number
        private void InitLine() {
            checkBox1.Checked = Settings1.Default.LineNumSeparator;
        }

        private void ApplyLine() {
            Settings1.Default.LineNumSeparator = checkBox1.Checked;
        }
        #endregion Line number

        #region Time
        private void InitTime() {
            showDuplicateTimes.Checked = Settings1.Default.DuplicateTimes;
            dontShowDuplicateTimes.Checked = !Settings1.Default.DuplicateTimes;
            showRelative.Checked = Settings1.Default.RelativeTime;
            showAbsolute.Checked = !Settings1.Default.RelativeTime;
        }

        private void ApplyTime() {
            Settings1.Default.DuplicateTimes = showDuplicateTimes.Checked;
            Settings1.Default.RelativeTime = showRelative.Checked;
        }
        #endregion Time

        #region Text
        private void InitText() {
            expandLinesWithNewlines.Checked = Settings1.Default.ExpandNewlines;
            indentChar.Text = Settings1.Default.IndentChar.ToString();
            indentAmount.Text = Settings1.Default.IndentAmount.ToString();
        }

        private bool VerifyText(bool showErrors) {
            bool verified = true;

            if (indentChar.Text == string.Empty) {
                if (showErrors) MessageBox.Show("Indentation character required.");
                verified = false;
            } else if (indentAmount.Text == string.Empty) {
                if (showErrors) MessageBox.Show("Indentation amount required.");
                verified = false;
            } else if (indentAmount.Text[0] < '0' || indentAmount.Text[0] > '9') {
                if (showErrors) MessageBox.Show("Indentation amount must be a number.");
                verified = false;
            }

            return verified;
        }

        private void ApplyText() {
            Settings1.Default.ExpandNewlines = expandLinesWithNewlines.Checked;
            Settings1.Default.IndentChar = indentChar.Text[0];
            Settings1.Default.IndentAmount = Convert.ToInt32(indentAmount.Text);
        }
        #endregion Text

        #region Auto refresh
        private void InitAutoRefresh() {
            refreshSeconds.Text = Settings1.Default.AutoRefreshInterval.ToString();
            reapplyFilter.Checked = Settings1.Default.KeepFilter;
        }

        private bool VerifyAutoRefresh(bool showErrors) {
            int temp;
            bool ret = true;

            if (int.TryParse(refreshSeconds.Text, out temp)) {
                if (temp <= 0) {
                    ret = false;
                    if (showErrors) MessageBox.Show("The refresh interval must be greater than zero.");
                }
            } else {
                ret = false;
                if (showErrors) MessageBox.Show("The refresh interval must be a number.");
            }

            return ret;
        }

        private void ApplyAutoRefresh() {
            Settings1.Default.AutoRefreshInterval = int.Parse(refreshSeconds.Text);
            Settings1.Default.KeepFilter = reapplyFilter.Checked;
        }
        #endregion Auto refresh

        #region Version Check
        private void InitVersionCheck() {
            txtVersionInterval.Text = Settings1.Default.VersionCheckInterval.ToString();
        }

        private bool VerifyVersionCheck(bool showErrors) {
            int temp;
            bool ret = true;

            if (int.TryParse(txtVersionInterval.Text, out temp)) {
                if (temp < 0) {
                    ret = false;
                    if (showErrors) MessageBox.Show("The version checking interval must not be negative.");
                }
            } else {
                ret = false;
                if (showErrors) MessageBox.Show("The version checking interval must be a number.");
            }

            return ret;
        }

        private void ApplyVersionCheck() {
            Settings1.Default.VersionCheckInterval = int.Parse(txtVersionInterval.Text);
        }
        #endregion Version Check
    }
}