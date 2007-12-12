using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

// TODO: Add support for methods.

namespace BBS.TracerX.Viewer {
    public partial class FilterDialog : Form {
        private MainForm _mainForm = MainForm.TheMainForm;
        private ColumnHeader _clickedHeader;
        private bool _suppressEvents = true;
        private ListViewItemSorter _threadIdSorter;
        private ListViewItemSorter _threadNameSorter;
        private ListViewItemSorter _loggerSorter;

        public FilterDialog() {
            InitializeComponent();

            this.Icon = Properties.Resources.scroll_view;
            InitTraceLevels();
            InitThreadIds();
            InitThreadNames();
            InitLoggers();
            InitText();
        }

        public FilterDialog(ColumnHeader clickedHeader) : this() {
            _clickedHeader = clickedHeader;

            if (_clickedHeader == _mainForm.headerLevel) {
                tabControl1.SelectedTab = traceLevelPage;
            } else if (_clickedHeader == _mainForm.headerLogger) {
                tabControl1.SelectedTab = loggerPage;
            } else if (_clickedHeader == _mainForm.headerThreadId) {
                tabControl1.SelectedTab = threadIdPage;
            } else if (_clickedHeader == _mainForm.headerThreadName) {
                tabControl1.SelectedTab = threadNamePage;
            } else if (_clickedHeader == _mainForm.headerText) {
                tabControl1.SelectedTab = textPage;
            }
        }

        // Create a delegate to compare the Checked states of two ListViewItems
        // for sorting via ListViewItemSorter.
        private ListViewItemSorter.RowComparer _checkComparer = delegate(ListViewItem x, ListViewItem y) {
            if (x.Checked == y.Checked) return 0;
            if (x.Checked) return 1;
            else return -1;
        };

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            ok.Enabled = false;   
            apply.Enabled = false; 
            _suppressEvents = false;
        }

        private void AnyListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
            if (_suppressEvents) return;
            ok.Enabled = true;
            apply.Enabled = true;
        }

        // For some reason, the AnyListView_ItemChecked event occurs when switching
        // between tabs that have ListViews.  Use the Selecting and SelectedIndexChanged
        // events to prevent OK and Apply from getting enabled just by switching tabs.
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e) {
            _suppressEvents = true;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e) {
            _suppressEvents = false;
        }

        private void ok_Click(object sender, EventArgs e) {
            apply_Click(null, null);
        }

        private void apply_Click(object sender, EventArgs e) {
            _mainForm.VisibleTraceLevels = SelectedTraceLevels;
            ApplyThreadIdSelection();
            ApplyThreadNameSelection();
            ApplyLoggerSelection();
            ApplyTextSelection();
            // TODO: Apply other pages.

            ok.Enabled = false;
            apply.Enabled = false; 
            _mainForm.RebuildAllRows();
        }

        #region Trace Levels        
        private void InitTraceLevels() {
            // Display only the trace levels that actually exist in the file.
            int index = 0;
            traceLevelListBox.Items.Clear();
            foreach (TraceLevel level in Enum.GetValues(typeof(TraceLevel))) {
                if (level != TraceLevel.Undefined && (level & _mainForm.ValidTraceLevels) == level) {
                    traceLevelListBox.Items.Add(level);                                                
                    traceLevelListBox.SetItemChecked(index, (level & _mainForm.VisibleTraceLevels) == level);
                    ++index;
                } 
            }
        }

        public TraceLevel SelectedTraceLevels {
            get {
                TraceLevel retval = TraceLevel.Undefined; // I.e. 0.
                foreach (TraceLevel level in traceLevelListBox.CheckedItems) {
                    retval |= level;
                }

                return retval;
            }
        }

        private void traceLevelListBox_ItemCheck(object sender, ItemCheckEventArgs e) {
            if (_suppressEvents) return;
            //ok.Enabled = e.NewValue == CheckState.Checked || traceLevelListBox.CheckedItems.Count > 1;
            apply.Enabled = true;
            ok.Enabled = true;
        }

        private void selectAllTraceLevels_Click(object sender, EventArgs e) {
            for (int i = 0; i < traceLevelListBox.Items.Count; ++i) {
                traceLevelListBox.SetItemChecked(i, true);
            }
        }

        private void clearAllTraceLevels_Click(object sender, EventArgs e) {
            for (int i = 0; i < traceLevelListBox.Items.Count; ++i) {
                traceLevelListBox.SetItemChecked(i, false);
            }
        }

        private void invertTraceLevels_Click(object sender, EventArgs e) {
            for (int i = 0; i < traceLevelListBox.Items.Count; ++i) {
                bool x = traceLevelListBox.GetItemChecked(i);
                traceLevelListBox.SetItemChecked(i, !x);
            }
        }
        #endregion Trace Levels

        #region Thread Ids
        private void InitThreadIds() {
            threadIdListView.BeginUpdate();

            // Populate the thread ID listview from ThreadObject.AllThreads.
            foreach (ThreadObject thread in ThreadObject.AllThreads) {
                ListViewItem item = new ListViewItem(new string[] { string.Empty, thread.Id.ToString() });
                item.Checked = thread.Visible;
                item.Tag = thread;
                this.threadIdListView.Items.Add(item);
            }

            threadCheckCol.Width = -1;
            //threadIdCol.Width = -1;
            threadIdListView.EndUpdate();
        }

        private void ApplyThreadIdSelection() {
            foreach (ListViewItem item in threadIdListView.Items) {
                ThreadObject thread = (ThreadObject)item.Tag;
                thread.Visible = item.Checked;
            }
        }

        private void checkAllThreadIds_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in threadIdListView.Items) {
                item.Checked = true;
            }
        }

        private void uncheckAllThreadIds_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in threadIdListView.Items) {
                item.Checked = false;
            }
        }

        private void invertThreads_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in threadIdListView.Items) {
                item.Checked = !item.Checked;
            }
        }

        private void threadIdListView_ColumnClick(object sender, ColumnClickEventArgs e) {
            _suppressEvents = true;
            // Create the sorting objects the first time they are required.
            if (_threadIdSorter == null) {
                // Create a delegate for comparing the IDs of the ThreadObjects that
                // correspond to two ListViewItems.
                ListViewItemSorter.RowComparer idComparer = delegate(ListViewItem x, ListViewItem y) {
                    // The ListViewItem tags are ThreadObjects.
                    int xint = ((ThreadObject)x.Tag).Id;
                    int yint = ((ThreadObject)y.Tag).Id;

                    return xint - yint;
                };

                threadIdCol.Tag = idComparer;
                threadCheckCol.Tag = _checkComparer;
                _threadIdSorter = new ListViewItemSorter(threadIdListView);
            }

            _threadIdSorter.Sort(e);
            _suppressEvents = false;
        }
        #endregion Thread Ids

        #region Thread Names
        private void InitThreadNames() {
            threadNameListView.BeginUpdate();

            // Populate the thread name listview from ThreadName.AllThreads.
            foreach (ThreadName thread in ThreadName.AllThreadNames) {
                ListViewItem item = new ListViewItem(new string[] { string.Empty, thread.Name });
                item.Checked = thread.Visible;
                item.Tag = thread;
                this.threadNameListView.Items.Add(item);
            }

            threadNameCheckCol.Width = -1;
            //threadNameNameCol.Width = -1;
            
            threadNameListView.EndUpdate();
        }

        private void ApplyThreadNameSelection() {
            foreach (ListViewItem item in threadNameListView.Items) {
                ThreadName thread = (ThreadName)item.Tag;
                thread.Visible = item.Checked;
            }
        }

        private void checkAllThreadNames_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in threadNameListView.Items) {
                item.Checked = true;
            }
        }

        private void uncheckAllThreadNames_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in threadNameListView.Items) {
                item.Checked = false;
            }
        }

        private void invertThreadNames_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in threadNameListView.Items) {
                item.Checked = !item.Checked;
            }
        }

        private void threadNameListView_ColumnClick(object sender, ColumnClickEventArgs e) {
            _suppressEvents = true;
            // Create the sorting objects the first time they are required.
            if (_threadNameSorter == null) {
                _threadNameSorter = new ListViewItemSorter(threadNameListView);
                threadNameCheckCol.Tag = _checkComparer;
            }

            _threadNameSorter.Sort(e);
            _suppressEvents = false;
        }
        #endregion Thread Names

        #region Loggers
        private void InitLoggers() {
            foreach (LoggerObject logger in LoggerObject.AllLoggers) {
                ListViewItem item = new ListViewItem(new string[] { string.Empty, logger.Name });
                item.Checked = logger.Visible;
                item.Tag = logger;
                this.loggerListView.Items.Add(item);
            }
        }

        private void checkAllLoggers_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in loggerListView.Items) {
                item.Checked = true;
            }
        }

        private void uncheckAllLoggers_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in loggerListView.Items) {
                item.Checked = false;
            }  
        }

        private void invertLoggers_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in loggerListView.Items) {
                item.Checked = !item.Checked;
            }
        }

        private void ApplyLoggerSelection() {
            foreach (ListViewItem item in loggerListView.Items) {
                LoggerObject logger = (LoggerObject)item.Tag;
                logger.Visible = item.Checked;
            }
        }

        private void loggerListView_ColumnClick(object sender, ColumnClickEventArgs e) {
            _suppressEvents = true;
            // Create the sorter object the first time it is required.
            if (_loggerSorter == null) {
                // Create a delegate for comparing the IDs of the ThreadObjects that
                // correspond to two ListViewItems.
                loggerCheckCol.Tag = _checkComparer;
                _loggerSorter = new ListViewItemSorter(loggerListView); 
            }

            _loggerSorter.Sort(e);
            _suppressEvents = false;

        }
        #endregion Loggers

        #region Text

        // Text filter settings live here.  Others have their own classes.
        private static string _includeText;
        private static string _excludeText;
        private static bool _includeChecked;
        private static bool _excludeChecked;
        private static StringComparison _compareType = StringComparison.CurrentCultureIgnoreCase;
        private static Regex _includeRegex;
        private static Regex _excludeRegex;

        public static event EventHandler TextFilterOnOff;

        /// <summary>True if text filtering is in effect.</summary>
        public static bool TextFilterOn {
            get { return _includeChecked || _excludeChecked; }
        }

        /// <summary>Turns text filtering off.</summary>
        public static void TextFilterDisable() {
            if (TextFilterOn) {
                _includeChecked = false;
                _excludeChecked = false;
                OnTextFilterOnOff(null);
            }
        }

        /// <summary>Determines if the string passes the text filter.</summary>
        public static bool TextFilterTestString(string line) {
            bool pass = true;
            if (_includeChecked) {
                if (_includeRegex == null) {
                    pass = (line.IndexOf(_includeText, _compareType) != -1);
                } else {
                    pass = _includeRegex.IsMatch(line);
                }
            }

            if (pass && _excludeChecked) {
                if (_excludeRegex == null) {
                    pass = (line.IndexOf(_excludeText, _compareType) == -1);
                } else {
                    pass = !_excludeRegex.IsMatch(line);
                }
            }

            return pass;
        }

        // Convert a string with wildcards to a regular expression string.
        public static string WildcardToRegex(string wildcard) {
            StringBuilder sb = new StringBuilder(wildcard.Length + 8);

            //sb.Append("^");

            for (int i = 0; i < wildcard.Length; i++) {
                char c = wildcard[i];
                switch (c) {
                    case '*':
                        sb.Append(".*");
                        break;
                    case '?':
                        sb.Append(".");
                        break;
                    case '\\':
                        if (i < wildcard.Length - 1)
                            sb.Append(Regex.Escape(wildcard[++i].ToString()));
                        break;
                    default:
                        sb.Append(Regex.Escape(c.ToString()));
                        break;
                }
            }

            //sb.Append("$");

            return sb.ToString();
        }

        private static void OnTextFilterOnOff(object sender) {
            if (TextFilterOnOff != null)
                TextFilterOnOff(sender, EventArgs.Empty);
        }

        // Initialize the controls based on the static properties.
        private void InitText() {
            txtContains.Text = _includeText;
            chkContain.Checked = _includeChecked;

            txtDoesNotContain.Text = _excludeText;
            chkDoesNotContain.Checked = _excludeChecked;

            chkCase.Checked = _compareType == StringComparison.CurrentCulture;
            chkWild.Checked = (_includeRegex != null || _excludeRegex != null);
        }

        // Set the static properties based on the controls.
        private void ApplyTextSelection() {
            bool wasOn = TextFilterOn;
            RegexOptions regexOptions;

            _includeChecked = chkContain.Checked && !string.IsNullOrEmpty(txtContains.Text);
            _includeText = txtContains.Text;

            _excludeChecked = chkDoesNotContain.Checked && !string.IsNullOrEmpty(txtDoesNotContain.Text);
            _excludeText = txtDoesNotContain.Text;

            if (chkCase.Checked) {
                _compareType = StringComparison.CurrentCulture;
                regexOptions = RegexOptions.Compiled;
            } else {
                _compareType = StringComparison.CurrentCultureIgnoreCase;
                regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;
            }
            
            _includeRegex = null;
            _excludeRegex = null;

            if (chkWild.Checked) {
                if (_includeChecked) {
                    _includeRegex = new Regex(WildcardToRegex(_includeText), regexOptions);
                }
                if (_excludeChecked) {
                    _excludeRegex = new Regex(WildcardToRegex(_excludeText), regexOptions);
                }
            } 

            if (TextFilterOn != wasOn) 
                OnTextFilterOnOff(this);

        }

        private void Text_CheckboxChanged(object sender, EventArgs e) {
            txtContains.Enabled = chkContain.Checked;
            txtDoesNotContain.Enabled = chkDoesNotContain.Checked;

            if (_suppressEvents) return;
            
            ok.Enabled = true;
            apply.Enabled = true;
        }

        private void Text_FilterTextChanged(object sender, EventArgs e) {
            if (_suppressEvents) return;
            ok.Enabled = true;
            apply.Enabled = true;
        }
        #endregion Text
    }
}