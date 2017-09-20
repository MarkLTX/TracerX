using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TracerX.Properties;

namespace TracerX
{
    public enum OptionsTab { Time, Misc, Text, Threads, StartPage, AutoUpdate, VersionChecking};
    public partial class OptionsDialog : Form
    {
        public OptionsDialog()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.scroll_view;
            InitStartPage();
            InitMisc();
            InitTime();
            InitText();
            InitAutoUpdate();
            InitThreads();
            InitVersionCheck();

            if (!Settings.Default.VersionCheckingAllowed)
            {
                tabControl1.TabPages.Remove(versionPage);
            }
        }

        public void SelectTab(OptionsTab tabID)
        {
            switch (tabID)
            {
                case OptionsTab.AutoUpdate:
                    tabControl1.SelectedTab = autoUpdatePage;
                    break;
                case OptionsTab.Misc:
                    tabControl1.SelectedTab = miscPage;
                    break;
                case OptionsTab.StartPage:
                    tabControl1.SelectedTab = startPagePage;
                    break;
                case OptionsTab.Text:
                    tabControl1.SelectedTab = textPage;
                    break;
                case OptionsTab.Threads:
                    tabControl1.SelectedTab = threadsPage;
                    break;
                case OptionsTab.Time:
                    tabControl1.SelectedTab = timePage;
                    break;
                case OptionsTab.VersionChecking:
                    tabControl1.SelectedTab = versionPage;
                    break;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ok.Enabled = false;
            apply.Enabled = false;
        }

        private void SomethingChanged(object sender, EventArgs e)
        {
            ok.Enabled = true;
            apply.Enabled = true;

            if (indentChar.Text == string.Empty)
                hex.Text = "An indentation character is required.";
            else
                hex.Text = string.Format("0x{0:X2}", (int)indentChar.Text[0]);

            if (sender == autoUpdate)
            {
                lblTip.Enabled = autoUpdate.Checked;
                lblMaxRecs.Enabled = autoUpdate.Checked;
                txtMaxRecords.Enabled = autoUpdate.Checked;
                autoReload.Enabled = autoUpdate.Checked;
            }
        }

        private void ok_Click(object sender, EventArgs e)
        {
            if (ApplyChanges())
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.None;
            }
        }

        private void apply_Click(object sender, EventArgs e)
        {
            ApplyChanges();
        }

        private bool ApplyChanges()
        {
            if (VerifyStartPage() && VerifyMisc() && VerifyText(true) && VerifyTime(true) && VerifyAutoUpdate() && VerifyVersionCheck(true))
            {
                ApplyStartPage();
                ApplyMisc();
                ApplyTime();
                ApplyText();
                ApplyAutoUpdate();
                ApplyThreads();
                ApplyVersionCheck();

                // We're not hiding or showing anything, so don't call RebuildRows.
                // Just make sure any visible rows get redrawn.
                MainForm.TheMainForm.InvalidateTheListView();

                ok.Enabled = false;
                apply.Enabled = false;

                return true;
            }
            else
            {
                return false;
            }
        }

        private void tabControl1_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            // Don't let the user leave the page if there is an error on the page.
            // Currently, only the text page requires verification.
            if (e.TabPage == textPage)
            {
                e.Cancel = !VerifyText(true);
            }
            else if (e.TabPage == startPagePage)
            {
                e.Cancel = !VerifyStartPage();
            }
            else if (e.TabPage == versionPage)
            {
                e.Cancel = !VerifyVersionCheck(true);
            }
        }

        #region Start page

        private void InitStartPage()
        {
            txtTimestampDays.Text = Settings.Default.DaysToSaveViewTimes.ToString();
            chkTracerxLogs.Checked = Settings.Default.ShowTracerxFiles;
            chkServersPane.Checked = Settings.Default.ShowServers;
        }

        private bool VerifyStartPage()
        {
            int converted;
            bool result = true;

            txtTimestampDays.Text = txtTimestampDays.Text.Trim();

            if (int.TryParse(txtTimestampDays.Text, out converted))
            {
                if (converted < 0)
                {
                    result = false;
                    tabControl1.SelectedTab = startPagePage;
                    txtTimestampDays.Focus();
                    MainForm.ShowMessageBox("The number of days must not be negative.");
                }
            }
            else
            {
                result = false;
                tabControl1.SelectedTab = startPagePage;
                txtTimestampDays.Focus();
                MainForm.ShowMessageBox("The number of days is not valid.");
            }

            return result;
        }

        private void ApplyStartPage()
        {
            Settings.Default.DaysToSaveViewTimes = int.Parse(txtTimestampDays.Text);
            Settings.Default.ShowTracerxFiles = chkTracerxLogs.Checked;
            Settings.Default.ShowServers = chkServersPane.Checked;
        }

        #endregion Start page

        #region Line numbers

        private static readonly bool _isVistaOrHigher = (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major >= 6);

        private void InitMisc()
        {
            chkDigitGrouping.Checked = Settings.Default.LineNumSeparator;
            chkReapplyFilter.Checked = Settings.Default.KeepFilter;
            chkFastScrolling.Checked = Settings.Default.UseFastScrollingKluge;
            chkFastScrolling.Visible = _isVistaOrHigher;
            txtFileSize.Text = Settings.Default.MaxNetworkKB.ToString();
        }

        private bool VerifyMisc()
        {
            int converted;
            bool result = true;

            txtFileSize.Text = txtFileSize.Text.Trim();

            if (int.TryParse(txtFileSize.Text, out converted))
            {
                if (converted < 0)
                {
                    result = false;
                    tabControl1.SelectedTab = miscPage;
                    txtFileSize.Focus();
                    MessageBox.Show(this, "The file size value must not be negative.", "TracerX-Viewer");
                }
            }
            else
            {
                result = false;
                tabControl1.SelectedTab = miscPage;
                txtFileSize.Focus();
                MessageBox.Show(this, "The file size value is invalid.", "TracerX-Viewer");
            }

            return result;
        }

        private void ApplyMisc()
        {
            Settings.Default.LineNumSeparator = chkDigitGrouping.Checked;
            Settings.Default.KeepFilter = chkReapplyFilter.Checked;
            Settings.Default.MaxNetworkKB = int.Parse(txtFileSize.Text);
            Settings.Default.UseFastScrollingKluge = chkFastScrolling.Checked;
        }
        #endregion Line number

        #region Time
        private void InitTime()
        {
            chkDuplicateTimes.Checked = Settings.Default.DuplicateTimes;
            showRelative.Checked = Settings.Default.RelativeTime;
            showAbsolute.Checked = !Settings.Default.RelativeTime;
            chkUseCustomTime.Checked = Settings.Default.UseCustomTimeFormat;
            txtCustomTimeFormat.Text = Settings.Default.CustomTimeFormat;
        }

        private bool VerifyTime(bool showErrors)
        {
            bool result = false;

            try
            {
                string s = DateTime.Now.ToString(txtCustomTimeFormat.Text);
                result = true;
            }
            catch (Exception ex)
            {
                if (showErrors)
                {
                    tabControl1.SelectedTab = timePage;
                    txtCustomTimeFormat.Focus();
                    MessageBox.Show(this, "The custom time format string is invalid.", "TracerX-Viewer");
                }
            }

            return result;
        }

        private void ApplyTime()
        {
            Settings.Default.DuplicateTimes = chkDuplicateTimes.Checked;
            Settings.Default.RelativeTime = showRelative.Checked;
            Settings.Default.UseCustomTimeFormat = chkUseCustomTime.Checked;
            Settings.Default.CustomTimeFormat = txtCustomTimeFormat.Text;
        }
        #endregion Time

        #region Text
        private void InitText()
        {
            expandLinesWithNewlines.Checked = Settings.Default.ExpandNewlines;
            indentChar.Text = Settings.Default.IndentChar.ToString();
            indentAmount.Text = Settings.Default.IndentAmount.ToString();
        }

        private bool VerifyText(bool showErrors)
        {
            bool verified = true;

            if (indentChar.Text == string.Empty)
            {
                verified = false;
                if (showErrors)
                {
                    tabControl1.SelectedTab = textPage;
                    indentChar.Focus();
                    MessageBox.Show("Indentation character required.");
                }
            }
            else if (indentAmount.Text == string.Empty)
            {
                verified = false;
                if (showErrors)
                {
                    tabControl1.SelectedTab = textPage;
                    indentAmount.Focus();
                    MessageBox.Show("Indentation amount required.");
                }
            }
            else if (indentAmount.Text[0] < '0' || indentAmount.Text[0] > '9')
            {
                verified = false;
                if (showErrors)
                {
                    tabControl1.SelectedTab = textPage;
                    indentAmount.Focus();
                    MessageBox.Show("Indentation amount must be a number.");
                }
            }

            return verified;
        }

        private void ApplyText()
        {
            Settings.Default.ExpandNewlines = expandLinesWithNewlines.Checked;
            Settings.Default.IndentChar = indentChar.Text[0];
            Settings.Default.IndentAmount = Convert.ToInt32(indentAmount.Text);
        }
        #endregion Text

        #region Auto-Update
        private void InitAutoUpdate()
        {
            autoUpdate.Checked = Settings.Default.AutoUpdate;
            txtMaxRecords.Text = Settings.Default.MaxRecords.ToString();
            autoReload.Checked = Settings.Default.AutoReload;
        }

        private bool VerifyAutoUpdate()
        {
            int converted;
            bool result = true;

            txtMaxRecords.Text = txtMaxRecords.Text.Trim();

            if (int.TryParse(txtMaxRecords.Text, out converted))
            {
                if (converted < 0)
                {
                    result = false;
                    tabControl1.SelectedTab = autoUpdatePage;
                    txtMaxRecords.Focus();
                    MessageBox.Show(this, "The number of records must not be negative.", "TracerX-Viewer");
                }
            }
            else
            {
                result = false;
                tabControl1.SelectedTab = autoUpdatePage;
                txtMaxRecords.Focus();
                MessageBox.Show(this, "The number of records is invalid.", "TracerX-Viewer");
            }

            return result;
        }

        private void ApplyAutoUpdate()
        {
            // Set MaxRecords first, because auto-update will resume in the event handler that
            // gets called when AutoUpdate is set to true.
            Settings.Default.MaxRecords = int.Parse(txtMaxRecords.Text);
            Settings.Default.AutoReload = autoReload.Checked;
            Settings.Default.AutoUpdate = autoUpdate.Checked;
        }
        #endregion Auto-Update

        #region Threads

        private void InitThreads()
        {
            radThreadName.Checked = Settings.Default.SearchThreadsByName;
            radThreadNumber.Checked = !Settings.Default.SearchThreadsByName;
        }

        private void ApplyThreads()
        {
            Settings.Default.SearchThreadsByName = radThreadName.Checked;
        }

        #endregion

        #region Version Check
        private void InitVersionCheck()
        {
            if (Settings.Default.VersionCheckingAllowed)
            {
                txtVersionInterval.Text = Settings.Default.VersionCheckInterval.ToString();
            }
        }

        private bool VerifyVersionCheck(bool showErrors)
        {
            int temp;
            bool ret = true;

            if (Settings.Default.VersionCheckingAllowed)
            {
                if (int.TryParse(txtVersionInterval.Text, out temp))
                {
                    if (temp < 0)
                    {
                        ret = false;
                        if (showErrors)
                        {
                            tabControl1.SelectedTab = versionPage;
                            txtVersionInterval.Focus();
                            MessageBox.Show("The version checking interval must not be negative.");
                        }
                    }
                }
                else
                {
                    ret = false;
                    if (showErrors)
                    {
                        tabControl1.SelectedTab = versionPage;
                        txtVersionInterval.Focus();
                        MessageBox.Show("The version checking interval must be a number.");
                    }
                }
            }

            return ret;
        }

        private void ApplyVersionCheck()
        {
            if (Settings.Default.VersionCheckingAllowed)
            {
                Settings.Default.VersionCheckInterval = int.Parse(txtVersionInterval.Text);
            }
        }
        #endregion Version Check
    }
}