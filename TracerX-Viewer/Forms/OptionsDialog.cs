using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TracerX.Properties;

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

            if (!Settings.Default.VersionCheckingAllowed) {
                tabControl1.TabPages.Remove(versionPage);
            }
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
            if (VerifyText(true) && VerifyVersionCheck(true)) {
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
                e.Cancel = false;
            } else if (e.TabPage == versionPage) {
                e.Cancel = !VerifyVersionCheck(true);
            }
        }

        #region Line number
        private void InitLine() {
            checkBox1.Checked = Settings.Default.LineNumSeparator;
        }

        private void ApplyLine() {
            Settings.Default.LineNumSeparator = checkBox1.Checked;
        }
        #endregion Line number

        #region Time
        private void InitTime() {
            showDuplicateTimes.Checked = Settings.Default.DuplicateTimes;
            dontShowDuplicateTimes.Checked = !Settings.Default.DuplicateTimes;
            showRelative.Checked = Settings.Default.RelativeTime;
            showAbsolute.Checked = !Settings.Default.RelativeTime;
        }

        private void ApplyTime() {
            Settings.Default.DuplicateTimes = showDuplicateTimes.Checked;
            Settings.Default.RelativeTime = showRelative.Checked;
        }
        #endregion Time

        #region Text
        private void InitText() {
            expandLinesWithNewlines.Checked = Settings.Default.ExpandNewlines;
            indentChar.Text = Settings.Default.IndentChar.ToString();
            indentAmount.Text = Settings.Default.IndentAmount.ToString();
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
            Settings.Default.ExpandNewlines = expandLinesWithNewlines.Checked;
            Settings.Default.IndentChar = indentChar.Text[0];
            Settings.Default.IndentAmount = Convert.ToInt32(indentAmount.Text);
        }
        #endregion Text

        #region Auto refresh
        private void InitAutoRefresh() {
            reapplyFilter.Checked = Settings.Default.KeepFilter;
            autoUpdate.Checked = Settings.Default.AutoUpdate;
        }


        private void ApplyAutoRefresh() {
            Settings.Default.KeepFilter = reapplyFilter.Checked;
            Settings.Default.AutoUpdate = autoUpdate.Checked;
        }
        #endregion Auto refresh

        #region Version Check
        private void InitVersionCheck() {
            if (Settings.Default.VersionCheckingAllowed) {
                txtVersionInterval.Text = Settings.Default.VersionCheckInterval.ToString();
            }
        }

        private bool VerifyVersionCheck(bool showErrors) {
            int temp;
            bool ret = true;

            if (Settings.Default.VersionCheckingAllowed) {
                if (int.TryParse(txtVersionInterval.Text, out temp)) {
                    if (temp < 0) {
                        ret = false;
                        if (showErrors) MessageBox.Show("The version checking interval must not be negative.");
                    }
                } else {
                    ret = false;
                    if (showErrors) MessageBox.Show("The version checking interval must be a number.");
                }
            }

            return ret;
        }

        private void ApplyVersionCheck() {
            if (Settings.Default.VersionCheckingAllowed) {
                Settings.Default.VersionCheckInterval = int.Parse(txtVersionInterval.Text);
            }
        }
        #endregion Version Check
    }
}