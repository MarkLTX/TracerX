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
using TracerX;
using TracerX.Viewer;
using System.IO;

namespace TracerX.Forms {
    public partial class ColorRulesDialog : Form {
        public ColorRulesDialog() {
            InitializeComponent();

            this.Icon = Properties.Resources.scroll_view;

            // For some reason, the X locations of these checkboxes change slightly
            // from what we specify in the designer.
            int indent = 20;
            textChk.Left = allChk.Left + indent;
            threadNameChk.Left = allChk.Left + indent;
            loggerChk.Left = allChk.Left + indent;
            methodChk.Left = allChk.Left + indent;
            levelChk.Left = allChk.Left + indent;

            listBox.Items.Clear();
            if (Settings.Default.ColoringRules != null) {
                foreach (ColoringRule rule in Settings.Default.ColoringRules) {
                    // We make a copy of each rule in case the user cancels his edits.
                    listBox.Items.Add(new ColoringRule(rule), rule.Enabled);
                }

                listBox.SelectedIndex = 0;
            } else {
                EnableControls();
            }
        }

        public static DialogResult ShowModal() {
            // TODO: Add master switch to dialog.
            ColorRulesDialog form = new ColorRulesDialog();
            return form.ShowDialog();
        }

        private ColoringRule _currentRule;

        private void listBox_SelectedIndexChanged(object sender, EventArgs e) {
            // Do nothing if we just re-selected the current rule.
            if (listBox.SelectedItem == _currentRule) return;
            string errorMsg = null;

            if (SaveRule(_currentRule, out errorMsg)) {
                EnableControls();

                _currentRule = listBox.SelectedItem as ColoringRule;
                if (_currentRule != null) {
                    ShowRule(_currentRule);
                }
            } else {
                // Reselect the previous rule and show the
                // error message.
                listBox.SelectedItem = _currentRule;
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
                    listBox.Refresh();
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
            int count = listBox.SelectedIndices.Count;
            removeBtn.Enabled = count > 0;
            exportBtn.Enabled = count > 0;
            upBtn.Enabled = count == 1 && listBox.SelectedIndex > 0;
            downBtn.Enabled = count == 1 && listBox.SelectedIndex < (listBox.Items.Count - 1);

            txtContains.Enabled = chkContain.Checked;
            txtDoesNotContain.Enabled = chkDoesNotContain.Checked;

            //nameBox.Enabled = count == 1;

            //textColorBtn.Enabled = count == 1;
            //backColorBtn.Enabled = count == 1;

            //chkContain.Enabled = count == 1;
            //chkDoesNotContain.Enabled = count == 1;
            //txtContains.Enabled = count == 1;
            //txtDoesNotContain.Enabled = count == 1;
            //chkCase.Enabled = count == 1;
            //chkWild.Enabled = count == 1;

            //allChk.Enabled = count == 1;
            //textChk.Enabled = count == 1;
            //threadNameChk.Enabled = count == 1;
            //loggerChk.Enabled = count == 1;
            //methodChk.Enabled = count == 1;
            //levelChk.Enabled = count == 1;
        }

        // Sets all controls to match current selection.
        private void ShowRule(  ColoringRule rule) {
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

                foreach (ColoringRule rule in listBox.Items) {
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
                    listBox.Items.Add(_currentRule, _currentRule.Enabled);
                    listBox.SelectedItem = _currentRule;
                    EnableControls();
                    ShowRule(_currentRule);
                } else {
                    MessageBox.Show(errorMsg);
                }
            } else {
                MessageBox.Show(errorMsg);
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

        private bool Apply() {
            string errorMsg;
            bool result;

            if (SaveRule(_currentRule, out errorMsg)) {
                result = true;

                if (listBox.Items.Count > 0) {
                    Settings.Default.ColoringRules = new ColoringRule[listBox.Items.Count];

                    for (int i = 0; i < listBox.Items.Count; ++i) {
                        // Save copies of the rules in case the Apply button was clicked
                        // instead of the OK button so further edits can be cancelled.
                        ColoringRule rule = (ColoringRule)listBox.Items[i];
                        rule = new ColoringRule(rule);
                        rule.Enabled = listBox.GetItemChecked(i);
                        rule.MakeReady();
                        Settings.Default.ColoringRules[i] = rule;
                    }
                } else {
                    Settings.Default.ColoringRules = null;
                }

                MainForm.TheMainForm.InvalidateTheListView();
            } else {
                result = false;
                MessageBox.Show(errorMsg);
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
                    ColoringRule[] arr = new ColoringRule[listBox.Items.Count];
                    for (int i = 0; i < listBox.Items.Count; ++i) {
                        // Save copies of the rules in case the Apply button was clicked
                        // instead of the OK button so further edits can be cancelled.
                        ColoringRule rule = (ColoringRule)listBox.Items[i];
                        rule.Enabled = listBox.GetItemChecked(i);
                        arr[i] = rule;
                    }

                    try {
                        XmlSerializer serializer = new XmlSerializer(typeof(ColoringRule[]));
                        using (Stream stream = dlg.OpenFile()) {
                            serializer.Serialize(stream, arr);
                        }
                    } catch (Exception ex) {
                        MessageBox.Show(ex.ToString());
                    }
                }
            } else {
                MessageBox.Show(errorMsg);
            }

        }

        private void removeBtn_Click(object sender, EventArgs e) {
            int ndx = listBox.SelectedIndex;

            _currentRule = null; // Prevents trying to save to _currentRule
            listBox.Items.RemoveAt(ndx);

            if (listBox.Items.Count > 0) {
                if (ndx < listBox.Items.Count) {
                    listBox.SelectedIndex = ndx;
                } else {
                    listBox.SelectedIndex = ndx - 1;
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
                int ndx = listBox.SelectedIndex;
                bool enabled = listBox.GetItemChecked(ndx);

                _currentRule = null; // Prevents trying to save to _currentRule
                listBox.Items.RemoveAt(ndx);
                listBox.Items.Insert(ndx - 1, temp);
                listBox.SetItemChecked(ndx - 1, enabled);
                listBox.SelectedIndex = ndx - 1;
                _currentRule = temp;
            } else {
                MessageBox.Show(errorMsg);
            }
        }

        private void downBtn_Click(object sender, EventArgs e) {
            string errorMsg;
            if (SaveRule(_currentRule, out errorMsg)) {
                ColoringRule temp = _currentRule;
                int ndx = listBox.SelectedIndex;
                bool enabled = listBox.GetItemChecked(ndx);

                _currentRule = null; // Prevents trying to save to _currentRule
                listBox.Items.RemoveAt(ndx);
                listBox.Items.Insert(ndx + 1, temp);
                listBox.SetItemChecked(ndx + 1, enabled);
                listBox.SelectedIndex = ndx + 1;
                _currentRule = temp;
            } else {
                MessageBox.Show(errorMsg);
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

                        listBox.Items.Clear();
                        if (arr != null) {
                            foreach (ColoringRule rule in arr) {
                                // We make a copy of each rule in case the user cancels his edits.
                                listBox.Items.Add(rule, rule.Enabled);
                            }

                            listBox.SelectedIndex = 0;
                        } else {
                            EnableControls();
                        }
                    }
                } catch (Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}