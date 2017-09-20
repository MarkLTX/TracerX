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
    internal partial class FilterDialog : Form
    {
        private MainForm _mainForm = MainForm.TheMainForm;
        private ColumnHeader _clickedHeader;
        private bool _suppressEvents = true;
        private ListViewSorter _threadIdSorter;
        private ListViewSorter _sessionSorter;
        private ListViewSorter _threadNameSorter;

        public FilterDialog()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.scroll_view;
            InitTraceLevels();
            InitSessions();
            InitThreadIds();
            InitThreadNames();
            lock (LoggerObjects.Lock) loggerControl.InitViews(LoggerObjects.AllLoggers, Settings.Default.ShowLoggerTree);
            lock (MethodObjects.Lock) methodControl.InitViews(MethodObjects.AllMethods, Settings.Default.ShowMethodTree);
            InitText();
        }

        public FilterDialog(ColumnHeader clickedHeader)
            : this()
        {
            _clickedHeader = clickedHeader;

            if (_clickedHeader == _mainForm.headerLevel)
            {
                tabControl1.SelectedTab = traceLevelPage;
            }
            else if (_clickedHeader == _mainForm.headerSession)
            {
                tabControl1.SelectedTab = sessionPage;
            }
            else if (_clickedHeader == _mainForm.headerLogger)
            {
                tabControl1.SelectedTab = loggerPage;
            }
            else if (_clickedHeader == _mainForm.headerMethod)
            {
                tabControl1.SelectedTab = methodPage;
            }
            else if (_clickedHeader == _mainForm.headerThreadId)
            {
                tabControl1.SelectedTab = threadIdPage;
            }
            else if (_clickedHeader == _mainForm.headerThreadName)
            {
                tabControl1.SelectedTab = threadNamePage;
            }
            else if (_clickedHeader == _mainForm.headerText)
            {
                tabControl1.SelectedTab = textPage;
            }
        }

        // Create a delegate to compare the Checked states of two ListViewItems
        // for sorting via ListViewItemSorter.
        private ListViewSorter.RowComparer _checkComparer = delegate(ListViewItem x, ListViewItem y)
        {
            if (x.Checked == y.Checked) return 0;
            if (x.Checked) return 1;
            else return -1;
        };

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ok.Enabled = false;
            apply.Enabled = false;
            _suppressEvents = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Settings.Default.ShowLoggerTree = loggerControl.IsShowingTree;
            Settings.Default.ShowMethodTree = methodControl.IsShowingTree;
        }

        private void AnyListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (_suppressEvents) return;

            var sorter = ((sender as ListView).ListViewItemSorter) as ListViewSorter;
            if (sorter != null && sorter.IsSorting) return;

            ok.Enabled = true;
            apply.Enabled = true;

            IndicateTabFilter(sessionPage, sessionListView);
            IndicateTabFilter(threadNamePage, threadNameListView);
            IndicateTabFilter(threadIdPage, threadIdListView);
        }

        // Displays a * in the TabPage header if anything is unchecked (i.e. filtered out).
        private void IndicateTabFilter(TabPage page, ListView listView)
        {
            bool isAllChecked = listView.CheckedItems.Count == listView.Items.Count;
            IndicateTabFilter(page, isAllChecked);
        }

        // Displays a * in the TabPage header if anything is unchecked (i.e. filtered out).
        private void IndicateTabFilter(TabPage page, bool isAllChecked)
        {
            if (page.Text[0] == '*')
            {
                if (isAllChecked)
                {
                    page.Text = page.Text.Trim('*');
                }
            }
            else
            {
                if (!isAllChecked)
                {
                    page.Text = "*" + page.Text;
                }
            }
        }

        // For some reason, the AnyListView_ItemChecked event occurs when switching
        // between tabs that have ListViews.  Use the Selecting and SelectedIndexChanged
        // events to prevent OK and Apply from getting enabled just by switching tabs.
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            _suppressEvents = true;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _suppressEvents = false;
        }

        private void ok_Click(object sender, EventArgs e)
        {
            apply_Click(null, null);
        }

        private void apply_Click(object sender, EventArgs e)
        {
            ApplyTraceLevelSelection();
            ApplySessionSelection();
            ApplyThreadIdSelection();
            ApplyThreadNameSelection();
            loggerControl.ApplyCurrentSelection();
            methodControl.ApplyCurrentSelection();
            Settings.Default.ShowCalledMethods = calledMethodsChk.Checked;
            ApplyTextSelection();

            ok.Enabled = false;
            apply.Enabled = false;
            _mainForm.RebuildAllRows();
        }

        #region Trace Levels

        private void InitTraceLevels()
        {
            int index = 0;
            traceLevelListBox.Items.Clear();

            lock (TraceLevelObjects.Lock)
            {
                foreach (TraceLevelObject tlo in TraceLevelObjects.AllTraceLevels.Values)
                {
                    traceLevelListBox.Items.Add(tlo);
                    traceLevelListBox.SetItemChecked(index, tlo.Visible);
                    ++index;
                }
            }

            IndicateTraceLevelFilter(!TraceLevelObjects.AllVisible);
        }

        private void IndicateTraceLevelFilter(bool isFiltered)
        {
            if (traceLevelPage.Text[0] == '*')
            {
                if (!isFiltered)
                {
                    traceLevelPage.Text = traceLevelPage.Text.Trim('*');
                }
            }
            else
            {
                if (isFiltered)
                {
                    traceLevelPage.Text = "*" + traceLevelPage.Text;
                }
            }
        }

        private void ApplyTraceLevelSelection()
        {
            foreach (TraceLevelObject tlo in traceLevelListBox.Items)
            {
                tlo.Visible = traceLevelListBox.CheckedItems.Contains(tlo);
            }
        }

        private void traceLevelListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_suppressEvents) return;
            //ok.Enabled = e.NewValue == CheckState.Checked || traceLevelListBox.CheckedItems.Count > 1;
            apply.Enabled = true;
            ok.Enabled = true;

            int checkedCount = traceLevelListBox.CheckedItems.Count;

            if (e.NewValue == CheckState.Checked)
            {
                ++checkedCount;
            }
            else
            {
                --checkedCount;
            }

            IndicateTraceLevelFilter(checkedCount != traceLevelListBox.Items.Count);
        }

        private void selectAllTraceLevels_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < traceLevelListBox.Items.Count; ++i)
            {
                traceLevelListBox.SetItemChecked(i, true);
            }
        }

        private void clearAllTraceLevels_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < traceLevelListBox.Items.Count; ++i)
            {
                traceLevelListBox.SetItemChecked(i, false);
            }
        }

        private void invertTraceLevels_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < traceLevelListBox.Items.Count; ++i)
            {
                bool x = traceLevelListBox.GetItemChecked(i);
                traceLevelListBox.SetItemChecked(i, !x);
            }
        }

        private void traceLevelListBox_Format(object sender, ListControlConvertEventArgs e)
        {
            //e.Value = Enum.GetName(typeof(TraceLevel), e.ListItem);
            e.Value = ((TraceLevelObject)e.ListItem).Name;
        }
        #endregion Trace Levels

        #region Sessions

        private void InitSessions()
        {
            sessionListView.BeginUpdate();

            // Populate the session listview from SessionObjects.AllSessions.
            lock (SessionObjects.Lock)
            {
                foreach (Reader.Session ses in SessionObjects.AllSessionObjects)
                {
                    ListViewItem item = new ListViewItem(new string[] { string.Empty, ses.Name });
                    item.Checked = ses.Visible;
                    item.Tag = ses;
                    this.sessionListView.Items.Add(item);
                }
            }

            sessionCheckCol.Width = -1;
            sessionListView.EndUpdate();
            IndicateTabFilter(sessionPage, sessionListView);
        }

        private void ApplySessionSelection()
        {
            foreach (ListViewItem item in sessionListView.Items)
            {
                Reader.Session ses = (Reader.Session)item.Tag;
                ses.Visible = item.Checked;
            }
        }

        private void checkAllSessions_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in sessionListView.Items)
            {
                item.Checked = true;
            }
        }

        private void uncheckAllSessionss_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in sessionListView.Items)
            {
                item.Checked = false;
            }
        }

        private void invertSessions_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in sessionListView.Items)
            {
                item.Checked = !item.Checked;
            }
        }

        private void sessionListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            _suppressEvents = true;
            // Create the sorting objects the first time they are required.
            if (_sessionSorter == null)
            {
                // Create a delegate for comparing the IDs of the Session objects that
                // correspond to two ListViewItems.
                ListViewSorter.RowComparer idComparer = delegate(ListViewItem x, ListViewItem y)
                {
                    // The ListViewItem tags are ThreadObjects.
                    int xint = ((Reader.Session)x.Tag).Index;
                    int yint = ((Reader.Session)y.Tag).Index;

                    return xint - yint;
                };

                _sessionSorter = new ListViewSorter(sessionListView);
                _sessionSorter.CustomComparers[sessionCol] = idComparer;
                _sessionSorter.CustomComparers[sessionCheckCol] = _checkComparer;
                _sessionSorter.Sort(e.Column);
            }

            _suppressEvents = false;
        }
        #endregion Sessions

        #region Thread Ids

        private void InitThreadIds()
        {
            threadIdListView.BeginUpdate();

            // Populate the thread ID listview from ThreadObjects.AllThreads.
            lock (ThreadObjects.Lock)
            {
                foreach (ThreadObject thread in ThreadObjects.AllThreadObjects)
                {
                    ListViewItem item = new ListViewItem(new string[] { string.Empty, thread.Id.ToString() });
                    item.Checked = thread.Visible;
                    item.Tag = thread;
                    this.threadIdListView.Items.Add(item);
                }
            }

            threadCheckCol.Width = -1;
            //threadIdCol.Width = -1;
            threadIdListView.EndUpdate();
            IndicateTabFilter(threadIdPage, threadIdListView);
        }

        private void ApplyThreadIdSelection()
        {
            foreach (ListViewItem item in threadIdListView.Items)
            {
                ThreadObject thread = (ThreadObject)item.Tag;
                thread.Visible = item.Checked;
            }
        }

        private void checkAllThreadIds_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in threadIdListView.Items)
            {
                item.Checked = true;
            }
        }

        private void uncheckAllThreadIds_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in threadIdListView.Items)
            {
                item.Checked = false;
            }
        }

        private void invertThreadIDs_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in threadIdListView.Items)
            {
                item.Checked = !item.Checked;
            }
        }

        private void threadIdListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            _suppressEvents = true;
            // Create the sorting objects the first time they are required.
            if (_threadIdSorter == null)
            {
                // Create a delegate for comparing the IDs of the ThreadObjects that
                // correspond to two ListViewItems.
                ListViewSorter.RowComparer idComparer = delegate(ListViewItem x, ListViewItem y)
                {
                    // The ListViewItem tags are ThreadObjects.
                    int xint = ((ThreadObject)x.Tag).Id;
                    int yint = ((ThreadObject)y.Tag).Id;

                    return xint - yint;
                };

                _threadIdSorter = new ListViewSorter(threadIdListView);
                _threadIdSorter.CustomComparers[threadIdCol] = idComparer;
                _threadIdSorter.CustomComparers[threadCheckCol] = _checkComparer;
                _threadIdSorter.Sort(e.Column);
            }

            _suppressEvents = false;
        }
        #endregion Thread Ids

        #region Thread Names
        private void InitThreadNames()
        {
            threadNameListView.BeginUpdate();

            // Populate the thread name listview from ThreadNames.AllThreads.
            lock (ThreadNames.Lock)
            {
                foreach (ThreadName thread in ThreadNames.AllThreadNames)
                {
                    ListViewItem item = new ListViewItem(new string[] { string.Empty, thread.Name });
                    item.Checked = thread.Visible;
                    item.Tag = thread;
                    this.threadNameListView.Items.Add(item);
                }
            }

            threadNameCheckCol.Width = -1;
            //threadNameNameCol.Width = -1;            
            threadNameListView.EndUpdate();
            IndicateTabFilter(threadNamePage, threadNameListView);
        }

        private void ApplyThreadNameSelection()
        {
            foreach (ListViewItem item in threadNameListView.Items)
            {
                ThreadName thread = (ThreadName)item.Tag;
                thread.Visible = item.Checked;
            }
        }

        private void checkAllThreadNames_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in threadNameListView.Items)
            {
                item.Checked = true;
            }
        }

        private void uncheckAllThreadNames_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in threadNameListView.Items)
            {
                item.Checked = false;
            }
        }

        private void invertThreadNames_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in threadNameListView.Items)
            {
                item.Checked = !item.Checked;
            }
        }

        private void threadNameListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            _suppressEvents = true;
            // Create the sorting objects the first time they are required.
            if (_threadNameSorter == null)
            {
                _threadNameSorter = new ListViewSorter(threadNameListView);
                _threadNameSorter.CustomComparers[threadNameCheckCol] = _checkComparer;
                _threadNameSorter.Sort(e.Column);
            }

            _suppressEvents = false;
        }
        #endregion Thread Names

        private void loggerControl_ItemChecked(object sender, EventArgs e)
        {
            ok.Enabled = true;
            apply.Enabled = true;
            IndicateTabFilter(loggerPage, loggerControl.IsAllChecked);
        }

        private void methodControl_ItemChecked(object sender, EventArgs e)
        {
            ok.Enabled = true;
            apply.Enabled = true;
            IndicateTabFilter(methodPage, methodControl.IsAllChecked);
        }

        #region Text

        // Text filter settings live here.  Others have their own classes.
        private static string _includeText;
        private static string _excludeText;
        private static bool _includeChecked;
        private static bool _excludeChecked;
        private static bool _caseChecked;
        private static bool _wildChecked;
        private static bool _regexChecked;
        private static StringMatcher _includeMatcher;
        private static StringMatcher _excludeMatcher;

        public static event EventHandler TextFilterOnOff;

        /// <summary>True if text filtering is in effect.</summary>
        public static bool TextFilterOn
        {
            get { return _includeChecked || _excludeChecked; }
        }

        /// <summary>Turns text filtering off.</summary>
        public static void TextFilterDisable()
        {
            if (TextFilterOn)
            {
                _includeChecked = false;
                _excludeChecked = false;
                OnTextFilterOnOff(null);
            }
        }

        /// <summary>Determines if the string passes the text filter.</summary>
        public static bool TextFilterTestString(string line)
        {
            bool pass = true;

            if (_includeChecked)
            {
                pass = _includeMatcher.Matches(line);
            }

            if (pass && _excludeChecked)
            {
                pass = !_excludeMatcher.Matches(line);
            }

            return pass;
        }

        private static void OnTextFilterOnOff(object sender)
        {
            if (TextFilterOnOff != null)
                TextFilterOnOff(sender, EventArgs.Empty);
        }

        // Initialize the controls based on the static properties.
        private void InitText()
        {
            txtContains.Text = _includeText;
            chkContain.Checked = _includeChecked;

            txtDoesNotContain.Text = _excludeText;
            chkDoesNotContain.Checked = _excludeChecked;

            chkCase.Checked = _caseChecked;
            radWildcard.Checked = _wildChecked;
            radRegex.Checked = _regexChecked;

            //if (TextFilterOn) textPage.Text = "*" + textPage.Text;
        }

        // Set the static properties based on the controls.
        private void ApplyTextSelection()
        {
            bool wasOn = TextFilterOn;
            MatchType matchType;

            _includeChecked = chkContain.Checked && !string.IsNullOrEmpty(txtContains.Text);
            _includeText = txtContains.Text;

            _excludeChecked = chkDoesNotContain.Checked && !string.IsNullOrEmpty(txtDoesNotContain.Text);
            _excludeText = txtDoesNotContain.Text;

            _caseChecked = chkCase.Checked;
            _wildChecked = radWildcard.Checked;
            _regexChecked = radRegex.Checked;

            // TODO: radNormal.Checked?

            if (_regexChecked)
            {
                matchType = MatchType.RegularExpression;
            }
            else if (_wildChecked)
            {
                matchType = MatchType.Wildcard;
            }
            else
            {
                matchType = MatchType.Simple;
            }

            if (_includeChecked)
            {
                _includeMatcher = new StringMatcher(_includeText, chkCase.Checked, matchType);
            }

            if (_excludeChecked)
            {
                _excludeMatcher = new StringMatcher(_excludeText, chkCase.Checked, matchType);
            }

            if (TextFilterOn != wasOn)
                OnTextFilterOnOff(this);

        }

        private void Text_CheckboxChanged(object sender, EventArgs e)
        {
            txtContains.Enabled = chkContain.Checked;
            txtDoesNotContain.Enabled = chkDoesNotContain.Checked;

            if (_suppressEvents) return;

            ok.Enabled = true;
            apply.Enabled = true;

            textPage.Text = textPage.Text.Trim('*');

            if (chkContain.Checked || chkDoesNotContain.Checked)
            {
                textPage.Text = "*" + textPage.Text;
            }
        }

        private void Text_FilterTextChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;
            ok.Enabled = true;
            apply.Enabled = true;
        }
        #endregion Text

    }
}