using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TracerX.Properties;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Linq;
using TracerX;
using TracerX.Viewer;
using System.IO;

namespace TracerX.Forms {
    internal partial class ColorRulesDialog : Form {

        // ColorTab identifies each of the tabs on the ColorRulesDialog.
        public enum ColorTab { Custom, TraceLevels, Loggers, ThreadNames, ThreadIDs, Methods, Sessions };

        public class ColorPair {
            public ColorPair() {
                BackColor = Color.White;
                ForeColor = Color.Black;
            }

            public ColorPair(Color backColor) {
                BackColor = backColor;
                ForeColor = Color.Black;
            }

            public ColorPair(Color backColor, Color foreColor) {
                BackColor = backColor;
                ForeColor = foreColor;
            }

            // The Enabled field only applies to TraceLevels.
            public bool Enabled = false;
            public Color ForeColor;
            public Color BackColor;
        }

        // Colors used for everything except TraceLevels.
        public static readonly ColorPair[] Palette;

        // The CurrentTab determines which color rules are applied.
        public static ColorTab CurrentTab
        {
            get { return _currentTab; }
            set { 
                _currentTab = value;   
            }
        }

        private static ColorTab _currentTab = ColorTab.Custom;

        public static bool ColorCalledMethods = true;

        // This is where the color for each TraceLevel is stored.  
        // This is not persisted.
        public static Dictionary<TraceLevel, ColorPair> TraceLevelColors = new Dictionary<TraceLevel, ColorPair>();

        private ListViewItemSorter _sessionSorter;
        private ListViewItemSorter _threadIdSorter;
        private ListViewItemSorter _threadNameSorter;
        private ListViewItemSorter _loggerSorter;
        private ListViewItemSorter _methodSorter;
        private ListViewItemSorter _traceLevelSorter;
        private ColoringRule _currentRule;
        private bool _suppressEvents = true;

        // Tracks the ColorPair chosen for each thread, logger, etc. until the user clicks Apply.
        private Dictionary<Reader.Session, ColorPair> _sessionColors = new Dictionary<Reader.Session, ColorPair>();
        private Dictionary<ThreadObject, ColorPair> _threadIDColors = new Dictionary<ThreadObject, ColorPair>();
        private Dictionary<ThreadName, ColorPair> _threadNameColors = new Dictionary<ThreadName, ColorPair>();
        private Dictionary<LoggerObject, ColorPair> _loggerColors = new Dictionary<LoggerObject, ColorPair>();
        private Dictionary<MethodObject, ColorPair> _methodColors = new Dictionary<MethodObject, ColorPair>();

        public static DialogResult ShowModal() {
            ColorRulesDialog form = new ColorRulesDialog();
            return form.ShowDialog();
        }

        public ColorRulesDialog()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.scroll_view;
            enableChk.Checked = Settings.Default.ColoringEnabled;

            switch (CurrentTab)
            {
                case ColorTab.Custom:
                    tabControl1.SelectedTab = customPage;
                    break;
                case ColorTab.Sessions:
                    tabControl1.SelectedTab = sessionPage;
                    break;
                case ColorTab.Loggers:
                    tabControl1.SelectedTab = loggerPage;
                    break;
                case ColorTab.Methods:
                    tabControl1.SelectedTab = methodPage;
                    break;
                case ColorTab.TraceLevels:
                    tabControl1.SelectedTab = traceLevelPage;
                    break;
                case ColorTab.ThreadIDs:
                    tabControl1.SelectedTab = threadIDPage;
                    break;
                case ColorTab.ThreadNames:
                    tabControl1.SelectedTab = threadNamePage;
                    break;
            }

            importBtn.Visible = CurrentTab == ColorTab.Custom;
            exportBtn.Visible = CurrentTab == ColorTab.Custom;

            InitCustomRules();
            InitSessions();
            InitTraceLevels();
            InitLoggers();
            InitThreadIds();
            InitThreadNames();
            InitMethods();
        }

        static ColorRulesDialog() {
            TraceLevelColors[TraceLevel.Fatal] = new ColorPair(Color.Red);
            TraceLevelColors[TraceLevel.Error] = new ColorPair(Color.Red);
            TraceLevelColors[TraceLevel.Warn] = new ColorPair(Color.Orange);
            TraceLevelColors[TraceLevel.Info] = new ColorPair(Color.LightBlue);
            TraceLevelColors[TraceLevel.Debug] = new ColorPair(Color.LightGreen);
            TraceLevelColors[TraceLevel.Verbose] = new ColorPair(Color.Gainsboro);

            // List of ARGB colors generated with the Palette form.
            int[] rgb = new int[] { -16711936, -129, -65281, -8421505, -8388609, -8388737, -32897, -16711809, -16711681, -8421377, -8388864, -65409, -33024, -32769, -256, -16744449, -65536};
            Palette = new ColorPair[rgb.Length];
            for (int i = 0; i<rgb.Length; ++i) {
                Palette[i] = new ColorPair(Color.FromArgb(rgb[i]));
            }
        }

        // This delegate compares the Checked states of two ListViewItems
        // for sorting via ListViewItemSorter.
        private ListViewItemSorter.RowComparer _checkComparer = delegate(ListViewItem x, ListViewItem y)
        {
            if (x.Checked == y.Checked) return 0;
            if (x.Checked) return 1;
            else return -1;
        };

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Since the Custom tab has so many controls, we just always leave the
            // OK and Apply buttons enabled rather than trying to detect every change.
            okBtn.Enabled = CurrentTab == ColorTab.Custom;
            applyBtn.Enabled = CurrentTab == ColorTab.Custom;

            _suppressEvents = false;
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (_suppressEvents) return;

            if (e.TabPage == customPage) CurrentTab = ColorTab.Custom;
            else if (e.TabPage == traceLevelPage) CurrentTab = ColorTab.TraceLevels;
            else if (e.TabPage == loggerPage) CurrentTab = ColorTab.Loggers;
            else if (e.TabPage == methodPage) CurrentTab = ColorTab.Methods;
            else if (e.TabPage == threadIDPage) CurrentTab = ColorTab.ThreadIDs;
            else if (e.TabPage == threadNamePage) CurrentTab = ColorTab.ThreadNames;
            else if (e.TabPage == sessionPage) CurrentTab = ColorTab.Sessions;

            // The import and export buttons only apply to the Custom tab.
            importBtn.Visible = CurrentTab == ColorTab.Custom;
            exportBtn.Visible = CurrentTab == ColorTab.Custom;

            okBtn.Enabled = true;
            applyBtn.Enabled = true;
        }

        #region Custom tab

        private void InitCustomRules() {
            // For some reason, the X locations of these checkboxes change slightly
            // from what we specify in the designer.
            int indent = 20;
            textChk.Left = allChk.Left + indent;
            threadNameChk.Left = allChk.Left + indent;
            loggerChk.Left = allChk.Left + indent;
            methodChk.Left = allChk.Left + indent;
            levelChk.Left = allChk.Left + indent;

            customListBox.Items.Clear();
            if (Settings.Default.ColoringRules != null) {
                foreach (ColoringRule rule in Settings.Default.ColoringRules) {
                    // We make a copy of each rule in case the user cancels his edits.
                    customListBox.Items.Add(new ColoringRule(rule), rule.Enabled);
                }

                customListBox.SelectedIndex = 0;
            } else {
                EnableControls();
            }
        }

        private void customListBox_SelectedIndexChanged(object sender, EventArgs e) {
            // Do nothing if we just re-selected the current rule.
            if (customListBox.SelectedItem == _currentRule) return;
            string errorMsg = null;

            if (SaveRule(_currentRule, out errorMsg)) {
                EnableControls();

                _currentRule = customListBox.SelectedItem as ColoringRule;
                if (_currentRule != null) {
                    ShowRule(_currentRule);
                }
            } else {
                // Reselect the previous rule and show the
                // error message.
                customListBox.SelectedItem = _currentRule;
                MessageBox.Show(errorMsg);
            }
        }

        // Save the current control settings to the specified rule unless
        // there's a problem.
        private bool SaveRule(ColoringRule rule, out string errorMsg) {
            errorMsg = null;

            if (rule != null) {
                // Verify current settings are valid before
                // saving them to the specified rule.
                // If we're not saving to _currentRule, we don't validate 
                // nameBox because we're saving to a new rule whose name 
                // won't come from nameBox.
                errorMsg = GetErrorMessage(rule == _currentRule);

                if (errorMsg == null) {
                    // No error, save the settings.
                    rule.Name = nameBox.Text;
                    customListBox.Refresh();
                    rule.BackColor = backPanel.BackColor;
                    rule.TextColor = textPanel.BackColor;

                    if (chkContain.Checked) {
                        rule.ContainsText = txtContains.Text;
                    } else {
                        rule.ContainsText = null;
                    }

                    if (chkDoesNotContain.Checked) {
                        rule.LacksText = txtDoesNotContain.Text;
                    } else {
                        rule.LacksText = null;
                    }

                    rule.MatchCase = chkCase.Checked;

                    if (chkWild.Checked) rule.MatchType = MatchType.Wildcard;
                    else if (chkRegex.Checked) rule.MatchType = MatchType.RegularExpression;
                    else rule.MatchType = MatchType.Simple;

                    if (allChk.Checked) {
                        rule.Fields = ColoringFields.All;
                    } else {
                        rule.Fields = ColoringFields.None;
                        if (textChk.Checked) rule.Fields |= ColoringFields.Text;
                        if (threadNameChk.Checked) rule.Fields |= ColoringFields.ThreadName;
                        if (loggerChk.Checked) rule.Fields |= ColoringFields.Logger;
                        if (methodChk.Checked) rule.Fields |= ColoringFields.Method;
                        if (levelChk.Checked) rule.Fields |= ColoringFields.Level;
                    }

                    errorMsg = rule.MakeReady();
                }
            }

            return errorMsg == null;
        }

        private string GetErrorMessage(bool validateName) {
            if (validateName) {
                nameBox.Text = nameBox.Text.Trim();

                if (nameBox.Text == string.Empty) {
                    return "Please enter a rule name.";
                }
            }

            if (!chkContain.Checked && !chkDoesNotContain.Checked) {
                return "Please specify text to search for.";
            }

            if (chkContain.Checked && txtContains.Text == string.Empty) {
                ActiveControl = txtContains;
                return "Please specify text to search for.";
            }

            if (chkDoesNotContain.Checked && txtDoesNotContain.Text == string.Empty) {
                ActiveControl = txtDoesNotContain;
                return "Please specify text to search for.";
            }

            if (!allChk.Checked &&
                !textChk.Checked &&
                !threadNameChk.Checked &&
                !loggerChk.Checked &&
                !methodChk.Checked &&
                !levelChk.Checked) //
            {
                return "Please select one or more fields to search.";
            }

            return null;
        }

        private void EnableControls() {
            int count = customListBox.SelectedIndices.Count;
            removeBtn.Enabled = count > 0;
            exportBtn.Enabled = count > 0;
            upBtn.Enabled = count == 1 && customListBox.SelectedIndex > 0;
            downBtn.Enabled = count == 1 && customListBox.SelectedIndex < (customListBox.Items.Count - 1);

            txtContains.Enabled = chkContain.Checked;
            txtDoesNotContain.Enabled = chkDoesNotContain.Checked;
        }

        // Sets all controls to match current selection.
        private void ShowRule(ColoringRule rule) {
            nameBox.Text = rule.Name;

            textPanel.BackColor = rule.TextColor;
            backPanel.BackColor = rule.BackColor;
            exampleBox.BackColor = rule.BackColor;
            exampleBox.ForeColor = rule.TextColor;

            chkContain.Checked = !string.IsNullOrEmpty(rule.ContainsText);
            txtContains.Text = rule.ContainsText;
            chkDoesNotContain.Checked = !string.IsNullOrEmpty(rule.LacksText);
            txtDoesNotContain.Text = rule.LacksText;
            chkCase.Checked = rule.MatchCase;
            chkWild.Checked = rule.MatchType == MatchType.Wildcard;
            chkRegex.Checked = rule.MatchType == MatchType.RegularExpression;

            textChk.Checked = (rule.Fields & ColoringFields.Text) == ColoringFields.Text;
            threadNameChk.Checked = (rule.Fields & ColoringFields.ThreadName) == ColoringFields.ThreadName;
            loggerChk.Checked = (rule.Fields & ColoringFields.Logger) == ColoringFields.Logger;
            methodChk.Checked = (rule.Fields & ColoringFields.Method) == ColoringFields.Method;
            levelChk.Checked = (rule.Fields & ColoringFields.Level) == ColoringFields.Level;

            // These two need to be last when setting the field checks
            allChk.Checked = false;
            allChk.Checked = rule.Fields == ColoringFields.All;
        }

        // Generate the first rule name like "Rule N" that
        // doesn't already exist.
        private string GenerateName() {
            int i = 0;
            string result;

            do {
                result = string.Format("Rule {0}", ++i);

                foreach (ColoringRule rule in customListBox.Items) {
                    if (rule.Name == result) {
                        result = null;
                        break;
                    }
                }
            } while (result == null);

            return result;
        }

        private void newBtn_Click(object sender, EventArgs e) {
            string errorMsg;

            // First save the current settings to the _currentRule if there is one.
            if (SaveRule(_currentRule, out errorMsg)) {
                // Make a new rule and save the current settings to it.  If that succeeds,
                // the new rule becomes the _currentRule.
                ColoringRule newRule = new ColoringRule();

                if (SaveRule(newRule, out errorMsg)) {
                    // The fact that we set _currentRule before selecting it prevents
                    // the SelectionChanged handler from doing anything.
                    _currentRule = newRule;
                    _currentRule.Name = GenerateName();
                    customListBox.Items.Add(_currentRule, _currentRule.Enabled);
                    customListBox.SelectedItem = _currentRule;
                    EnableControls();
                    ShowRule(_currentRule);
                } else {
                    MessageBox.Show(errorMsg, "TracerX-Viewer");
                }
            } else {
                MessageBox.Show(errorMsg, "TracerX-Viewer");
            }
        }

        private void chkContain_CheckedChanged(object sender, EventArgs e) {
            txtContains.Enabled = chkContain.Checked;
        }

        private void chkDoesNotContain_CheckedChanged(object sender, EventArgs e) {
            txtDoesNotContain.Enabled = chkDoesNotContain.Checked;
        }

        private void allChk_CheckedChanged(object sender, EventArgs e) {
            if (allChk.Checked) {
                textChk.Checked = true;
                threadNameChk.Checked = true;
                loggerChk.Checked = true;
                methodChk.Checked = true;
                levelChk.Checked = true;
            }

            textChk.Enabled = !allChk.Checked;
            threadNameChk.Enabled = !allChk.Checked;
            loggerChk.Enabled = !allChk.Checked;
            methodChk.Enabled = !allChk.Checked;
            levelChk.Enabled = !allChk.Checked;
        }

        private void backColorBtn_Click(object sender, EventArgs e) {
            PickColor(backPanel);
            exampleBox.BackColor = backPanel.BackColor;
        }

        private void textColorBtn_Click(object sender, EventArgs e) {
            PickColor(textPanel);
            exampleBox.ForeColor = textPanel.BackColor;
        }

        private void PickColor(Panel panel) {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = panel.BackColor;
            dlg.AnyColor = true;
            DialogResult dialogResult = dlg.ShowDialog();

            if (dialogResult == DialogResult.OK) {
                panel.BackColor = dlg.Color;
                Debug.Print(dlg.Color.ToString());
            }
        }


        private void importBtn_Click(object sender, EventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.AddExtension = true;
            dlg.CheckFileExists = true;
            dlg.DefaultExt = "xml";
            dlg.Filter = "XML|*.xml|All files|*.*";
            dlg.FilterIndex = 0;
            dlg.Multiselect = false;
            DialogResult result = dlg.ShowDialog();

            if (result == DialogResult.OK) {
                try {
                    using (Stream stream = dlg.OpenFile()) {
                        XmlSerializer deserializer = new XmlSerializer(typeof(ColoringRule[]));
                        ColoringRule[] arr = (ColoringRule[])deserializer.Deserialize(stream);

                        customListBox.Items.Clear();
                        if (arr != null) {
                            foreach (ColoringRule rule in arr) {
                                // We make a copy of each rule in case the user cancels his edits.
                                customListBox.Items.Add(rule, rule.Enabled);
                            }

                            customListBox.SelectedIndex = 0;
                        } else {
                            EnableControls();
                        }
                    }
                } catch (Exception ex) {
                    MessageBox.Show(ex.ToString(), "TracerX-Viewer");
                }
            }
        }

        private void exportBtn_Click(object sender, EventArgs e) {
            string errorMsg;

            if (SaveRule(_currentRule, out errorMsg)) {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.AddExtension = true;
                dlg.DefaultExt = "xml";
                dlg.Filter = "XML|*.xml|All files|*.*";
                dlg.FilterIndex = 0;
                //dlg.CreatePrompt = true;
                dlg.OverwritePrompt = true;
                dlg.ValidateNames = true;
                DialogResult result = dlg.ShowDialog();

                if (result == DialogResult.OK) {
                    string file = dlg.FileName;
                    ColoringRule[] arr = new ColoringRule[customListBox.Items.Count];
                    for (int i = 0; i < customListBox.Items.Count; ++i) {
                        // Save copies of the rules in case the Apply button was clicked
                        // instead of the OK button so further edits can be cancelled.
                        ColoringRule rule = (ColoringRule)customListBox.Items[i];
                        rule.Enabled = customListBox.GetItemChecked(i);
                        arr[i] = rule;
                    }

                    try {
                        XmlSerializer serializer = new XmlSerializer(typeof(ColoringRule[]));
                        using (Stream stream = dlg.OpenFile()) {
                            serializer.Serialize(stream, arr);
                        }
                    } catch (Exception ex) {
                        MessageBox.Show(ex.ToString(), "TracerX-Viewer");
                    }
                }
            } else {
                MessageBox.Show(errorMsg, "TracerX-Viewer");
            }

        }

        private void removeBtn_Click(object sender, EventArgs e) {
            int ndx = customListBox.SelectedIndex;

            _currentRule = null; // Prevents trying to save to _currentRule
            customListBox.Items.RemoveAt(ndx);

            if (customListBox.Items.Count > 0) {
                if (ndx < customListBox.Items.Count) {
                    customListBox.SelectedIndex = ndx;
                } else {
                    customListBox.SelectedIndex = ndx - 1;
                }

                ActiveControl = removeBtn;
            } else {
                ActiveControl = newBtn;
            }

            EnableControls();
        }

        private void upBtn_Click(object sender, EventArgs e) {
            string errorMsg;
            if (SaveRule(_currentRule, out errorMsg)) {
                ColoringRule temp = _currentRule;
                int ndx = customListBox.SelectedIndex;
                bool enabled = customListBox.GetItemChecked(ndx);

                _currentRule = null; // Prevents trying to save to _currentRule
                customListBox.Items.RemoveAt(ndx);
                customListBox.Items.Insert(ndx - 1, temp);
                customListBox.SetItemChecked(ndx - 1, enabled);
                customListBox.SelectedIndex = ndx - 1;
                _currentRule = temp;
            } else {
                MessageBox.Show(errorMsg, "TracerX-Viewer");
            }
        }

        private void downBtn_Click(object sender, EventArgs e) {
            string errorMsg;
            if (SaveRule(_currentRule, out errorMsg)) {
                ColoringRule temp = _currentRule;
                int ndx = customListBox.SelectedIndex;
                bool enabled = customListBox.GetItemChecked(ndx);

                _currentRule = null; // Prevents trying to save to _currentRule
                customListBox.Items.RemoveAt(ndx);
                customListBox.Items.Insert(ndx + 1, temp);
                customListBox.SetItemChecked(ndx + 1, enabled);
                customListBox.SelectedIndex = ndx + 1;
                _currentRule = temp;
            } else {
                MessageBox.Show(errorMsg, "TracerX-Viewer");
            }

        }

        private void chkWild_CheckedChanged(object sender, EventArgs e) {
            if (chkWild.Checked) {
                chkRegex.Checked = false;
                chkRegex.Enabled = false;
            } else {
                chkRegex.Enabled = true;
            }
        }

        private void chkRegex_CheckedChanged(object sender, EventArgs e) {
            if (chkRegex.Checked) {
                chkWild.Checked = false;
                chkWild.Enabled = false;
            } else {
                chkWild.Enabled = true;
            }

        }

        private bool ApplyCustomRules() {
            string errorMsg;
            bool result;

            if (SaveRule(_currentRule, out errorMsg)) {
                result = true;

                if (customListBox.Items.Count > 0) {
                    Settings.Default.ColoringRules = new ColoringRule[customListBox.Items.Count];

                    for (int i = 0; i < customListBox.Items.Count; ++i) {
                        // Save copies of the rules in case the Apply button was clicked
                        // instead of the OK button so further edits can be cancelled.
                        ColoringRule rule = (ColoringRule)customListBox.Items[i];
                        rule = new ColoringRule(rule);
                        rule.Enabled = customListBox.GetItemChecked(i);
                        rule.MakeReady();
                        Settings.Default.ColoringRules[i] = rule;
                    }
                } else {
                    Settings.Default.ColoringRules = null;
                }
            } else {
                result = false;
                tabControl1.SelectedTab = customPage;
                MessageBox.Show(errorMsg, "TracerX-Viewer");
            }

            return result;
        }

        #endregion Custom tab

        #region Trace Levels

        private void InitTraceLevels() {
            traceLevelListView.Items.Clear();
            foreach (TraceLevel level in Enum.GetValues(typeof(TraceLevel))) {
                if (level != TraceLevel.Inherited && level != TraceLevel.Off) {
                    ListViewItem item = new ListViewItem(new string[] { string.Empty, Enum.GetName(typeof(TraceLevel), level) });
                    if (TraceLevelColors[level].Enabled) {
                        item.Checked = true; ;
                        item.ForeColor = TraceLevelColors[level].ForeColor;
                        item.BackColor = TraceLevelColors[level].BackColor;
                    }
                    item.Tag = level;
                    this.traceLevelListView.Items.Add(item);
                }
            }
        }

        private void traceLevelListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
            if (_suppressEvents) return;

            if (e.Item.Checked) {
                e.Item.ForeColor = TraceLevelColors[(TraceLevel)e.Item.Tag].ForeColor;
                e.Item.BackColor = TraceLevelColors[(TraceLevel)e.Item.Tag].BackColor;
            } else {
                e.Item.ForeColor = Color.Empty;
                e.Item.BackColor = Color.Empty;
            }

            applyBtn.Enabled = true;
            okBtn.Enabled = true;
        }

        private void selectAllTraceLevels_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in traceLevelListView.Items) {
                item.Checked = true;
            }
        }

        private void clearAllTraceLevels_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in traceLevelListView.Items) {
                item.Checked = false;
            }
        }

        private void invertTraceLevels_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in traceLevelListView.Items) {
                item.Checked = !item.Checked;
            }
        }

        private void traceLevelListView_ColumnClick(object sender, ColumnClickEventArgs e) {
            _suppressEvents = true;
            // Create the sorting objects the first time they are required.
            if (_traceLevelSorter == null) {
                // Create a delegate for comparing the IDs of the ThreadObjects that
                // correspond to two ListViewItems.
                ListViewItemSorter.RowComparer levelComparer = delegate(ListViewItem x, ListViewItem y) {
                    // The ListViewItem tags are ThreadObjects.
                    int xint = (int)(TraceLevel)x.Tag;
                    int yint = (int)(TraceLevel)y.Tag;

                    return xint - yint;
                };

                traceLevelNameCol.Tag = levelComparer;
                traceLevelCheckCol.Tag = _checkComparer;
                _traceLevelSorter = new ListViewItemSorter(traceLevelListView);
            }

            _traceLevelSorter.Sort(e.Column);
            _suppressEvents = false;
        }

        private bool ApplyTraceLevels() {
            foreach (ListViewItem item in traceLevelListView.Items) {
                TraceLevel level = (TraceLevel)(item.Tag);
                TraceLevelColors[level].Enabled = item.Checked;
            }

            return true;
        }

        #endregion Trace Levels

        #region Sessions

        private void InitSessions() {
            sessionListView.BeginUpdate();

            // Populate the session listview from SessionObjects.AllSessionObjects.
            lock (SessionObjects.Lock) {
                foreach (Reader.Session ses in SessionObjects.AllSessionObjects) {
                    ListViewItem item = new ListViewItem(new string[] { string.Empty, ses.Name });

                    if (ses.Colors != null) {
                        item.Checked = true;
                        _sessionColors[ses] = ses.Colors;
                        item.BackColor = ses.Colors.BackColor;
                        item.ForeColor = ses.Colors.ForeColor;
                    }

                    item.Tag = ses;
                    this.sessionListView.Items.Add(item);
                }
            }

            sessionListView.EndUpdate();
            checkAllSessions.Visible = SessionObjects.AllSessionObjects.Count <= Palette.Length;
            invertSessions.Visible = SessionObjects.AllSessionObjects.Count <= Palette.Length;
        }

        private void sessionListView_ItemCheck(object sender, ItemCheckEventArgs e) {
            if (_suppressEvents) return;

            lock (SessionObjects.Lock) {
                var item = sessionListView.Items[e.Index];
                var ses = (Reader.Session)item.Tag;

                if (e.NewValue == CheckState.Checked) {
                    if (_sessionColors.Count < Palette.Length) {
                        // Find the first unused color pair and associated it with the session.
                        var colorPair = (from cp in Palette
                                         where !_sessionColors.Values.Contains(cp)
                                         select cp).First();
                        _sessionColors[ses] = colorPair;
                        item.BackColor = colorPair.BackColor;
                        item.ForeColor = colorPair.ForeColor;

                        applyBtn.Enabled = true;
                        okBtn.Enabled = true;
                    } else {
                        var msg = string.Format("Only {0} items may be checked concurrently.", Palette.Length);
                        MessageBox.Show(this, msg, "TracerX-Viewer");
                    }
                } else {
                    _sessionColors.Remove(ses);
                    item.BackColor = Color.Empty;
                    item.ForeColor = Color.Empty;

                    applyBtn.Enabled = true;
                    okBtn.Enabled = true;
                }
            }
        }

        private bool ApplySessionSelection() {
            foreach (ListViewItem item in sessionListView.Items) {
                Reader.Session ses = (Reader.Session)item.Tag;
                ColorPair colors = null;
                _sessionColors.TryGetValue(ses, out colors);
                ses.Colors = colors;
            }

            return true;
        }

        private void checkAllSessions_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in sessionListView.Items) {
                item.Checked = true;
            }
        }

        private void uncheckAllSessions_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in sessionListView.Items) {
                item.Checked = false;
            }
        }

        private void invertSessions_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in sessionListView.Items) {
                item.Checked = !item.Checked;
            }
        }

        private void sessionListView_ColumnClick(object sender, ColumnClickEventArgs e) {
            _suppressEvents = true;
            // Create the sorting objects the first time they are required.
            if (_sessionSorter == null) {
                // Create a delegate for comparing the indexes of the Session objects that
                // correspond to two ListViewItems.
                ListViewItemSorter.RowComparer idComparer = delegate(ListViewItem x, ListViewItem y) {
                    // The ListViewItem tags are SessionObjects.
                    int xint = ((Reader.Session)x.Tag).Index;
                    int yint = ((Reader.Session)y.Tag).Index;

                    return xint - yint;
                };

                sessionCol.Tag = idComparer;
                sessionCheckCol.Tag = _checkComparer;
                _sessionSorter = new ListViewItemSorter(sessionListView);
            }

            _sessionSorter.Sort(e.Column);
            _suppressEvents = false;
        }

        #endregion Sessions

        #region Thread Ids

        private void InitThreadIds() {
            threadIdListView.BeginUpdate();

            // Populate the thread ID listview from ThreadObjects.AllThreads.
            lock (ThreadObjects.Lock) {
                foreach (ThreadObject thread in ThreadObjects.AllThreadObjects) {
                    ListViewItem item = new ListViewItem(new string[] { string.Empty, thread.Id.ToString() });

                    if (thread.Colors != null) {
                        item.Checked = true;
                        _threadIDColors[thread] = thread.Colors;
                        item.BackColor = thread.Colors.BackColor;
                        item.ForeColor = thread.Colors.ForeColor;
                    }

                    item.Tag = thread;
                    this.threadIdListView.Items.Add(item);
                }
            }

            threadIdListView.EndUpdate();
            checkAllThreadIds.Visible = ThreadObjects.AllThreadObjects.Count <= Palette.Length;
            invertThreadIds.Visible = ThreadObjects.AllThreadObjects.Count <= Palette.Length;
        }

        private void threadIdListView_ItemCheck(object sender, ItemCheckEventArgs e) {
            if (_suppressEvents) return;

            lock (ThreadObjects.Lock) {
                var item = threadIdListView.Items[e.Index];
                var thread = (ThreadObject)item.Tag;

                if (e.NewValue == CheckState.Checked) {
                    if (_threadIDColors.Count < Palette.Length) {
                        // Find the first unused color pair and associated it with the thread.
                        var colorPair = (from cp in Palette
                                         where !_threadIDColors.Values.Contains(cp)
                                         select cp).First();
                        _threadIDColors[thread] = colorPair;
                        item.BackColor = colorPair.BackColor;
                        item.ForeColor = colorPair.ForeColor;

                        applyBtn.Enabled = true;
                        okBtn.Enabled = true;
                    } else {
                        var msg = string.Format("Only {0} items may be checked concurrently.", Palette.Length);
                        MessageBox.Show(this, msg, "TracerX-Viewer");
                    }
                } else {
                    _threadIDColors.Remove(thread);
                    item.BackColor = Color.Empty;
                    item.ForeColor = Color.Empty;

                    applyBtn.Enabled = true;
                    okBtn.Enabled = true;
                }
            }
        }

        private bool ApplyThreadIdSelection() {
            foreach (ListViewItem item in threadIdListView.Items) {
                ThreadObject thread = (ThreadObject)item.Tag;
                ColorPair colors = null;
                _threadIDColors.TryGetValue(thread, out colors);
                thread.Colors = colors;
            }

            return true;
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

        private void invertThreadIDs_Click(object sender, EventArgs e) {
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

            _threadIdSorter.Sort(e.Column);
            _suppressEvents = false;
        }

        #endregion Thread Ids

        #region Thread Names

        private void InitThreadNames() {
            threadNameListView.BeginUpdate();

            // Populate the thread Name listview from ThreadNames.AllThreadNames.
            lock (ThreadNames.Lock) {
                foreach (ThreadName thread in ThreadNames.AllThreadNames) {
                    ListViewItem item = new ListViewItem(new string[] { string.Empty, thread.Name });

                    if (thread.Colors != null) {
                        item.Checked = true;
                        _threadNameColors[thread] = thread.Colors;
                        item.BackColor = thread.Colors.BackColor;
                        item.ForeColor = thread.Colors.ForeColor;
                    }

                    item.Tag = thread;
                    this.threadNameListView.Items.Add(item);
                }
            }

            threadNameListView.EndUpdate();
            checkAllThreadNames.Visible = ThreadNames.AllThreadNames.Count <= Palette.Length;
            invertThreadNames.Visible = ThreadNames.AllThreadNames.Count <= Palette.Length;
        }

        private void threadNameListView_ItemCheck(object sender, ItemCheckEventArgs e) {
            if (_suppressEvents) return;

            lock (ThreadNames.Lock) {
                var item = threadNameListView.Items[e.Index];
                var thread = (ThreadName)item.Tag;

                if (e.NewValue == CheckState.Checked) {
                    if (_threadNameColors.Count < Palette.Length) {
                        // Find the first unused color pair and associated it with the thread.
                        var colorPair = (from cp in Palette
                                         where !_threadNameColors.Values.Contains(cp)
                                         select cp).First();
                        _threadNameColors[thread] = colorPair;
                        item.BackColor = colorPair.BackColor;
                        item.ForeColor = colorPair.ForeColor;

                        applyBtn.Enabled = true;
                        okBtn.Enabled = true;
                    } else {
                        var msg = string.Format("Only {0} items may be checked concurrently.", Palette.Length);
                        MessageBox.Show(this, msg, "TracerX-Viewer");
                    }
                } else {
                    _threadNameColors.Remove(thread);
                    item.BackColor = Color.Empty;
                    item.ForeColor = Color.Empty;

                    applyBtn.Enabled = true;
                    okBtn.Enabled = true;
                }
            }
        }

        private bool ApplyThreadNameSelection() {
            foreach (ListViewItem item in threadNameListView.Items) {
                ThreadName thread = (ThreadName)item.Tag;
                ColorPair colors = null;
                _threadNameColors.TryGetValue(thread, out colors);
                thread.Colors = colors;
            }

            return true;
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

        private void invertThreadNamess_Click(object sender, EventArgs e) {
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

            _threadNameSorter.Sort(e.Column);
            _suppressEvents = false;
        }

        #endregion Thread Names

        #region Loggers

        private void InitLoggers() {
            loggerListView.BeginUpdate();

            // Populate the thread Name listview from .
            lock (LoggerObjects.Lock) {
                foreach (LoggerObject logger in LoggerObjects.AllLoggers) {
                    ListViewItem item = new ListViewItem(new string[] { string.Empty, logger.Name });

                    if (logger.Colors != null) {
                        item.Checked = true;
                        _loggerColors[logger] = logger.Colors;
                        item.BackColor = logger.Colors.BackColor;
                        item.ForeColor = logger.Colors.ForeColor;
                    }

                    item.Tag = logger;
                    this.loggerListView.Items.Add(item);
                }
            }

            loggerListView.EndUpdate();
            checkAllLoggers.Visible = LoggerObjects.AllLoggers.Count <= Palette.Length;
            invertLoggers.Visible = LoggerObjects.AllLoggers.Count <= Palette.Length;
        }

        private void loggerListView_ItemCheck(object sender, ItemCheckEventArgs e) {
            if (_suppressEvents) return;

            lock (LoggerObjects.Lock) {
                var item = loggerListView.Items[e.Index];
                var logger = (LoggerObject)item.Tag;

                if (e.NewValue == CheckState.Checked) {
                    if (_loggerColors.Count < Palette.Length) {
                        // Find the first unused color pair and associate it with the logger.
                        var colorPair = (from cp in Palette
                                         where !_loggerColors.Values.Contains(cp)
                                         select cp).First();
                        _loggerColors[logger] = colorPair;
                        item.BackColor = colorPair.BackColor;
                        item.ForeColor = colorPair.ForeColor;

                        applyBtn.Enabled = true;
                        okBtn.Enabled = true;
                    } else {
                        var msg = string.Format("Only {0} items may be checked concurrently.", Palette.Length);
                        MessageBox.Show(this, msg, "TracerX-Viewer");
                    }
                } else {
                    _loggerColors.Remove(logger);
                    item.BackColor = Color.Empty;
                    item.ForeColor = Color.Empty;

                    applyBtn.Enabled = true;
                    okBtn.Enabled = true;
                }
            }
        }

        private bool ApplyLoggerSelection() {
            foreach (ListViewItem item in loggerListView.Items) {
                LoggerObject logger = (LoggerObject)item.Tag;
                ColorPair colors = null;
                _loggerColors.TryGetValue(logger, out colors);
                logger.Colors = colors;
            }

            return true;
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

        private void loggerListView_ColumnClick(object sender, ColumnClickEventArgs e) {
            _suppressEvents = true;
            // Create the sorting objects the first time they are required.
            if (_loggerSorter == null) {
                _loggerSorter = new ListViewItemSorter(loggerListView);
                loggerCheckCol.Tag = _checkComparer;
            }

            _loggerSorter.Sort(e.Column);
            _suppressEvents = false;
        }

        #endregion Loggers

        #region Methods

        private void InitMethods() {
            methodListView.BeginUpdate();

            // Populate the thread Name listview from .
            lock (MethodObjects.Lock) {
                foreach (MethodObject method in MethodObjects.AllMethods) {
                    ListViewItem item = new ListViewItem(new string[] { string.Empty, method.Name });

                    if (method.Colors != null) {
                        item.Checked = true;
                        _methodColors[method] = method.Colors;
                        item.BackColor = method.Colors.BackColor;
                        item.ForeColor = method.Colors.ForeColor;
                    }

                    item.Tag = method;
                    this.methodListView.Items.Add(item);
                }
            }

            methodListView.EndUpdate();

            checkAllMethods.Visible = MethodObjects.AllMethods.Count <= Palette.Length;
            invertMethods.Visible = MethodObjects.AllMethods.Count <= Palette.Length;
            calledMethodsChk.Checked = ColorCalledMethods;
        }

        private void methodListView_ItemCheck(object sender, ItemCheckEventArgs e) {
            if (_suppressEvents) return;

            lock (MethodObjects.Lock) {
                var item = methodListView.Items[e.Index];
                var method = (MethodObject)item.Tag;

                if (e.NewValue == CheckState.Checked) {
                    if (_methodColors.Count < Palette.Length) {
                        // Find the first unused color pair and associate it with the logger.
                        var colorPair = (from cp in Palette
                                         where !_methodColors.Values.Contains(cp)
                                         select cp).First();
                        _methodColors[method] = colorPair;
                        item.BackColor = colorPair.BackColor;
                        item.ForeColor = colorPair.ForeColor;

                        applyBtn.Enabled = true;
                        okBtn.Enabled = true;
                    } else {
                        var msg = string.Format("Only {0} items may be checked concurrently.", Palette.Length);
                        MessageBox.Show(this, msg, "TracerX-Viewer");
                    }
                } else {
                    _methodColors.Remove(method);
                    item.BackColor = Color.Empty;
                    item.ForeColor = Color.Empty;

                    applyBtn.Enabled = true;
                    okBtn.Enabled = true;
                }
            }
        }

        private bool ApplyMethodSelection() {
            foreach (ListViewItem item in methodListView.Items) {
                MethodObject method = (MethodObject)item.Tag;
                ColorPair colors = null;
                _methodColors.TryGetValue(method, out colors);
                method.Colors = colors;
            }

            ColorCalledMethods = calledMethodsChk.Checked;

            return true;
        }

        private void checkAllMethods_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in methodListView.Items) {
                item.Checked = true;
            }
        }

        private void uncheckAllMethods_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in methodListView.Items) {
                item.Checked = false;
            }
        }

        private void invertMethods_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in methodListView.Items) {
                item.Checked = !item.Checked;
            }
        }

        private void methodListView_ColumnClick(object sender, ColumnClickEventArgs e) {
            _suppressEvents = true;
            // Create the sorting objects the first time they are required.
            if (_methodSorter == null) {
                _methodSorter = new ListViewItemSorter(methodListView);
                methodCheckCol.Tag = _checkComparer;
            }

            _methodSorter.Sort(e.Column);
            _suppressEvents = false;
        }

        private void calledMethodsChk_CheckedChanged(object sender, EventArgs e) {
            if (_suppressEvents) return;

            okBtn.Enabled = true;
            applyBtn.Enabled = true;
        }

        #endregion Methods

        private bool Apply() {
            bool result =
                ApplyCustomRules() &&
                ApplyTraceLevels() &&
                ApplySessionSelection() &&
                ApplyThreadNameSelection() &&
                ApplyLoggerSelection() &&
                ApplyMethodSelection() &&
                ApplyThreadIdSelection();

            if (result) {
                // Invalidating the ListView causes the visible rows to be regenerated using
                // the settings associated with CurrentTab.
                Settings.Default.ColoringEnabled = enableChk.Checked;
                MainForm.TheMainForm.InvalidateTheListView();

                // Since the Custom tab has so many controls, we just always leave the
                // OK and Apply buttons enabled rather than trying to detect every change.
                if (CurrentTab != ColorTab.Custom)
                {
                    okBtn.Enabled = false;
                    applyBtn.Enabled = false;
                }
            }

            return result;
        }

        private void applyBtn_Click(object sender, EventArgs e) {
            Apply();
        }

        private void okBtn_Click(object sender, EventArgs e) {
            if (!Apply()) {
                // This prevents the form from closing.
                DialogResult = DialogResult.None;
           }
        }

        private void paletteBtn_Click(object sender, EventArgs e) {
            var dlg = new PaletteViewer();
            dlg.ShowDialog();
        }

        private void enableChk_CheckedChanged(object sender, EventArgs e) {
            if (_suppressEvents) return;

            okBtn.Enabled = true;
            applyBtn.Enabled = true;
        }

        private void okBtn_EnabledChanged(object sender, EventArgs e) {
            Debug.Print("Enabled changed");
        }
    }
}