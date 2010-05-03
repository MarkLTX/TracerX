using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TracerX.Viewer;
using TracerX.Properties;
//he currently visible records (based on filtering and expanded/collapsed methods) will be exported. The currently visible columns will be exported in the order they are displayed.

namespace TracerX.Forms {
    // UI and code for exporting the log file to a comma-separated value file.
    // Only the currently visible records are exported (i.e. filtering and collapsing matter).
    // Only the currently visible columns are exported, in the order they are displayed.
    internal partial class ExportCSVForm : Form {
        // Call this method to show the form.
        public static DialogResult ShowModal() {
            ExportCSVForm form = new ExportCSVForm();
            return form.ShowDialog();
        }

        // Ctor is private. Call ShowModal to display this form.
        private ExportCSVForm() {
            InitializeComponent();
            LoadSettings();
            this.Icon = Properties.Resources.scroll_view;
        }

        private void LoadSettings() {
            fileBox.Text = Settings.Default.ExportFile;
            indentChk.Checked = Settings.Default.ExportIndentation;
            headerChk.Checked = Settings.Default.ExportHeader;
            separatorChk.Checked = Settings.Default.ExportSeparators;
            omitDupTimeChk.Checked = !Settings.Default.ExportDupTimes;
            timeAsIntRad.Checked = !Settings.Default.ExportTimeAsText;
            timeAsTextRad.Checked = Settings.Default.ExportTimeAsText;
            timeWithBlankChk.Checked = Settings.Default.ExportTimeWithBlank;
        }

        // Browses for output file.
        private void browseBtn_Click(object sender, EventArgs e) {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.DefaultExt = ".csv";
            dlg.OverwritePrompt = false;
            dlg.CheckPathExists = false;
            dlg.Filter = "CSV files|*.csv|All files|*.*";
            dlg.FilterIndex = 0;
            dlg.InitialDirectory = fileBox.Text;

            if (DialogResult.OK == dlg.ShowDialog()) {
                fileBox.Text = dlg.FileName;
            }
        }

        //// Returns the specified directory if it exists, else checks the parent directories
        //// recursively and returns the "lowest" existing directory found. Returns null if none
        //// exist or the specified directory has invalid syntax (e.g. an empty string).
        //private static string GetLowestExistingDir(string dir) {
        //    try {
        //        while (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) {
        //            dir = Path.GetDirectoryName(dir); // Can return null.  Throws exception if format is bad.
        //        }

        //        return dir;
        //    } catch (Exception) {
        //        // Happens when dir format is invalid.
        //        return null;
        //    }
        //}

        // OK button validates the output file and calls the Export method.
        private void okBtn_Click(object sender, EventArgs e) {
            Cursor originalCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            try {
                if (ValidFile()) {
                    SaveSettings();
                    Export();
                } else {
                    DialogResult = DialogResult.None;
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
                DialogResult = DialogResult.None;
            } finally {
                this.Cursor = originalCursor;
            }
        }

        private void SaveSettings() {
            Settings.Default.ExportFile = fileBox.Text;
            Settings.Default.ExportIndentation = indentChk.Checked;
            Settings.Default.ExportHeader = headerChk.Checked;
            Settings.Default.ExportSeparators = separatorChk.Checked;
            Settings.Default.ExportDupTimes  = !omitDupTimeChk.Checked;
            Settings.Default.ExportTimeAsText = timeAsTextRad.Checked;
            Settings.Default.ExportTimeWithBlank = timeWithBlankChk.Checked;
        }

        // Returns true if the output file is valid.
        // Asks for permission to replace existing output file.
        // Asks for permission to create new directory.
        private bool ValidFile() {
            bool result = true;

            if (fileBox.Text.Trim() == string.Empty) {
                MessageBox.Show("A file name is required.");
                result = false;
            } else if (File.Exists(fileBox.Text)) {
                if (DialogResult.Yes != MessageBox.Show("Replace existing file?", "TracerX", MessageBoxButtons.YesNo)) {
                    result = false;
                }
            } else if (!Directory.Exists(Path.GetDirectoryName(fileBox.Text))) {
                if (DialogResult.Yes == MessageBox.Show("The specified directory does not exist.  Create it?", "TracerX", MessageBoxButtons.YesNo)) {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileBox.Text));
                } else {
                    result = false;
                }
            }

            return result;
        }

        // Modifies a string for CSV format.  Doubles any quotes.
        // Adds quotes if leading blanks, trailing blanks, quotes, 
        // commas, or newlines are found.
        private string EscapeForCSV(string str, bool forceQuotes) {
            if (str.Contains("\"")) {
                forceQuotes = true;
                str = str.Replace("\"", "\"\"");
            }

            if (!forceQuotes) {
                if (str.StartsWith(" ") ||
                    str.EndsWith(" ") ||
                    str.Contains(",") ||
                    str.Contains("\n"))//
                {
                    forceQuotes = true;
                }
            }

            if (forceQuotes) str = "\"" + str + "\"";

            return str;
        }

        // Extracts the visible fields/columns from the specified Row and puts them in the
        // fields array according to their DisplayIndex.  Honors the various checkboxes
        // that control indentation, duplicates, etc.  Calls EscapeForCSV as needed.
        private void SetFields(string[] fields, Row row, Row previousRow, int indentAmount) {
            MainForm mainForm = MainForm.TheMainForm;
            Record Rec = row.Rec;
            Record previousRec = previousRow == null ? null : previousRow.Rec;
            string temp;

            // If a column has been removed from the ListView, 
            // its ListView property will be null.

            if (mainForm.headerText.ListView != null) {
                // Since many values in the Text column will need quotes, force all
                // of them to have quotes for consistency.
                temp = Rec.GetLine(row.Line, Settings.Default.IndentChar, indentAmount, false);
                fields[mainForm.headerText.DisplayIndex] = EscapeForCSV(temp, true);
            }

            if (mainForm.headerSession.ListView != null) {
                if (previousRec != null && previousRec.Session == Rec.Session) {
                    // Same value as previous record.
                    // Just leave the previous string value in place.
                } else {
                    // Value is different from previous record.
                    fields[mainForm.headerSession.DisplayIndex] = Rec.Session.Name;
                }
            }

            if (mainForm.headerLine.ListView != null) {
                temp = Rec.GetRecordNum(row.Line, separatorChk.Checked);
                if (separatorChk.Checked) temp = EscapeForCSV(temp, false);
                fields[mainForm.headerLine.DisplayIndex] = temp;
            }

            if (mainForm.headerLevel.ListView != null) {
                if (previousRec != null && previousRec.Level == Rec.Level) {
                    // Same value as previous record.
                        // Just leave the previous string value in place.
                } else {
                    // Value is different from previous record.
                    fields[mainForm.headerLevel.DisplayIndex] = Enum.GetName(typeof(TraceLevel), Rec.Level);
                }
            }

            if (mainForm.headerLogger.ListView != null) {
                if (previousRec != null && previousRec.Logger == Rec.Logger) {
                    // Same value as previous record.
                    // Just leave the previous string value in place.
                } else {
                    // Value is different from previous record.
                    fields[mainForm.headerLogger.DisplayIndex] = EscapeForCSV(Rec.Logger.Name, false);
                }
            }

            if (mainForm.headerThreadId.ListView != null) {
                if (previousRec != null && previousRec.ThreadId == Rec.ThreadId) {
                    // Same value as previous record.
                        // Just leave the previous string value in place.
                } else {
                    // Value is different from previous record.
                    fields[mainForm.headerThreadId.DisplayIndex] = Rec.ThreadId.ToString();
                }
            }

            if (mainForm.headerThreadName.ListView != null) {
                if (previousRec != null && previousRec.ThreadName == Rec.ThreadName) {
                    // Same value as previous record.
                        // Just leave the previous string value in place.
                } else {
                    // Value is different from previous record.
                    fields[mainForm.headerThreadName.DisplayIndex] = EscapeForCSV(Rec.ThreadName.Name, false);
                }
            }

            if (mainForm.headerMethod.ListView != null) {
                if (previousRec != null && previousRec.MethodName == Rec.MethodName) {
                    // Same value as previous record.
                        // Just leave the previous string value in place.
                } else {
                    // Value is different from previous record.
                    fields[mainForm.headerMethod.DisplayIndex] = EscapeForCSV(Rec.MethodName.Name, false);
                }
            }

            if (mainForm.headerTime.ListView != null) {
                if (previousRec != null && previousRec.Time == Rec.Time) {
                    // Same value as previous record.
                    if (omitDupTimeChk.Checked) {
                        fields[mainForm.headerTime.DisplayIndex] = string.Empty;
                    } else {
                        // Just leave the previous string value in place.
                    }
                } else {
                    // Value is different from previous record.
                    if (timeAsTextRad.Checked) {
                        if (timeWithBlankChk.Checked) {
                            // Include a leading blank so Excel treats it as text.
                            if (Settings.Default.RelativeTime) {
                                fields[mainForm.headerTime.DisplayIndex] = string.Format("\" {0}\"", Program.FormatTimeSpan(Rec.Time - MainForm.ZeroTime));
                            } else {
                                fields[mainForm.headerTime.DisplayIndex] = string.Format("\" {0}\"", Rec.Time.ToLocalTime().ToString(@"MM/dd/yy HH:mm:ss.fff"));
                            }
                        } else {
                            if (Settings.Default.RelativeTime) {
                                fields[mainForm.headerTime.DisplayIndex] = Program.FormatTimeSpan(Rec.Time - MainForm.ZeroTime);
                            } else {
                                fields[mainForm.headerTime.DisplayIndex] = Rec.Time.ToLocalTime().ToString(@"MM/dd/yy HH:mm:ss.fff");
                            }
                        }
                    } else if (timeAsIntRad.Checked) {
                        fields[mainForm.headerTime.DisplayIndex] = Rec.Time.Ticks.ToString();
                    }
                }
            }
        }

        // Opens the output file, writes the output, closes the file.
        private void Export() {
            string[] fields = new string[MainForm.TheMainForm.TheListView.Columns.Count];
            int indentAmount = Settings.Default.IndentAmount;
            Row previousRow = null;

            if (!indentChk.Checked) indentAmount = 0;

            using (StreamWriter writer = File.CreateText(fileBox.Text)) {
                if (headerChk.Checked) {
                    foreach (ColumnHeader col in MainForm.TheMainForm.TheListView.Columns) {
                        fields[col.DisplayIndex] = col.Text;
                    }

                    writer.WriteLine(string.Join(",", fields)); 
                }

                for (int i = 0; i < MainForm.TheMainForm.NumRows; ++i) {
                    Row row = MainForm.TheMainForm.Rows[i];
                    SetFields(fields, row, previousRow, indentAmount);
                    writer.WriteLine(string.Join(",", fields));
                    previousRow = row;
                }
            }
        }

        private void timeAsIntRad_CheckedChanged(object sender, EventArgs e) {
            timeWithBlankChk.Enabled = timeAsTextRad.Checked;
        }
    }
}