using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using TracerX.Properties;
using TracerX.Forms;
using System.Linq;
using System.Collections.Generic;

namespace TracerX.Viewer {
    /// <summary>
    /// A Record object corresponds to a message logged by the logger.  The Record text may contain
    /// embedded newlines, which the viewer can expand into multiple Row objects.
    /// </summary>
    internal class Record {
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // Testing has proven that the ListView control will render only the
        // first 260 chars of any field.  Longer strings are simply truncated.
        // Strings exactly 260 chars long CRASH the viewer in comctl32.dll!
        // This may not happen depending on which version of comctl32.dll is present,
        // but has been observed on multiple machines.
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public static int MaxViewableChars = 259;

        public Record(DataFlags dataflags, ulong msgNum, DateTime time, ReaderThreadInfo threadInfo, Reader.Session session, string msg) {
            MsgNum = msgNum;
            Time = time;
            Thread = threadInfo.Thread;
            ThreadName = threadInfo.ThreadName;
            Level = threadInfo.Level;
            Logger = threadInfo.Logger;
            StackDepth = threadInfo.Depth;
            MethodName = threadInfo.MethodName;
            Caller = threadInfo.StackTop;
            Session = session;

            if ((dataflags & DataFlags.MethodEntry) != DataFlags.None) {
                // This is a method entry record.  It always contains exactly one line of text.
                IsEntry = true;
                Lines = new string[] { string.Format("{{{0}: entered", MethodName.Name) };
            } else if ((dataflags & DataFlags.MethodExit) != DataFlags.None) {
                // Method exit records always contain exactly one line of text.
                IsExit = true;
                Lines = new string[] { string.Format("}}{0}: exiting", MethodName.Name) };
            } else {
                // The message text for this record may contain newlines.  Split the
                // message into one or more lines.
                Lines = msg.Split(_splitArg);

                if (Lines.Length > 1) {
                    // It's common for a carriage return to exist at the 
                    // end of each line.  Remove them.
                    for (int i = 0; i < Lines.Length; ++i) {
                        Lines[i] = Lines[i].TrimEnd('\r');
                    }

                    // It's common for the last line to be empty.  If so, remove it.
                    if (Lines.Last().Trim() == string.Empty) {
                        Lines = Lines.Take(Lines.Length - 1).ToArray();
                    }
                }

                IsCollapsed = Lines.Length > 1 && !Settings.Default.ExpandNewlines;
            }

            // Each line also has a bool to indicate if it is bookmarked
            // and a row index it may map to.
            IsBookmarked = new bool[Lines.Length];
            RowIndices = new int[Lines.Length];
        }

        // This constructs the missing MethodExit Record for the given MethodEntry record.
        // This is for MethodExit records lost due to wrapping.
        public Record(Record counterpart) {
            Debug.Assert(counterpart.IsEntry);
            //IsEntry = false;
            IsExit = true;
            MsgNum = 0; // TBD
            Time = DateTime.MinValue; // TBD;
            Index = 0; // TBD
            Thread = counterpart.Thread;
            ThreadName = counterpart.ThreadName;
            Level = counterpart.Level;
            Logger = counterpart.Logger;
            StackDepth = counterpart.StackDepth;
            MethodName = counterpart.MethodName;
            Lines = new string[] { string.Format("}}{0}: exiting (replaces record lost due to wrapping)", MethodName.Name) };
            Caller = counterpart;
            Session = counterpart.Session;

            // Each record also has a bool to indicate if it is bookmarked
            // and a row index it may map to.
            IsBookmarked = new bool[1];
            RowIndices = new int[1];
        }

        // This constructs a missing MethodEntry Record from the given ReaderStackEntry.
        // This is for MethodEntry records lost due to wrapping.
        public Record(ReaderThreadInfo threadInfo, ExplicitStackEntry methodEntry, Reader.Session session) {
            IsEntry = true;
            //IsExit = false;
            MsgNum = 0; // TBD
            Time = DateTime.MinValue; // TBD
            Index = 0; // TBD
            Thread = threadInfo.Thread;
            ThreadName = threadInfo.ThreadName;
            Level = methodEntry.Level;
            Logger = methodEntry.Logger;
            StackDepth = methodEntry.Depth;
            MethodName = methodEntry.Method;
            Lines = new string[] { string.Format("{{{0}: entered (replaces record lost due to wrapping)", MethodName.Name) };
            Session = session;

            // Each record also has a bool to indicate if it is bookmarked
            // and a row index it may map to.
            IsBookmarked = new bool[1];
            RowIndices = new int[1];
        }

        // Array of strings obtained by splitting the record's message text at embedded newlines.
        public string[] Lines ;

        // Does the message text contain newlines?
	    public bool HasNewlines { get { return Lines.Length > 1; } }

        // Is this a MethodEntry record?
        public bool IsEntry;

        // Is this a MethodExit record?
        public bool IsExit;

        // The entry Record for the containing method.
        public Record Caller;

        // If IsEntry or HasNewlines, is the method call or message collapsed?
        public bool IsCollapsed;

        // Track which lines in this record are bookmarked.
        public bool[] IsBookmarked;

        // Index into the Rows collection of the
        // first visible line in this record.
        // -1 if no lines are visible.
        public int FirstRowIndex;

        // True if this Record has any visible lines.  Set by SetVisibleRows().
        public bool IsVisible { get { return FirstRowIndex != -1; } }

        // Which row indices do the Lines map to?
        // A given element is -1 if the corresponding Line is not visible.
        // Only valid if IsVisible.  Set by SetVisibleRows().
        public int[] RowIndices;

        // Index is this record's index in the array of all records.
        public int Index;

        public int ThreadId {
            get { return Thread.Id; }
        }

        // Gets the IFilterable object whose Colors property determines
        // this Record's colors. 
        public IFilterable ColorDriver
        {
            get
            {
                switch (ColorRulesDialog.CurrentTab)
                {
                    case ColorRulesDialog.ColorTab.Loggers:
                        return this.Logger;
                    case ColorRulesDialog.ColorTab.Methods:
                        return this.MethodName;
                    case ColorRulesDialog.ColorTab.ThreadIDs:
                        return this.Thread;
                    case ColorRulesDialog.ColorTab.Sessions:
                        return this.Session;
                    case ColorRulesDialog.ColorTab.ThreadNames:
                        return this.ThreadName;
                    case ColorRulesDialog.ColorTab.Custom:
                    case ColorRulesDialog.ColorTab.TraceLevels:
                        return null;
                    default:
                        return null;
                }
            }
        }

        // MsgNum is the message number from the log file.  
        // This may not be equal to Index if the log has wrapped.
        public ulong MsgNum ; 
        public DateTime Time ;
        public ThreadObject Thread;
        public ThreadName ThreadName;
        public TraceLevel Level;
        public LoggerObject Logger ;
        public MethodObject MethodName ;
        public byte StackDepth ;
        public Reader.Session Session;

        // When collapsing nested methods, this indicate the nesting depth.
        // 0 means the record is visible as far as collapsing goes, but it could
        // still be invisible due to filtering.
        public short CollapsedDepth = 0;

        public override string ToString() {
            return MsgNum.ToString() + ": " + (Lines[0] ?? "");
        }

        // Sets FirstRowIndex, IsVisible and RowIndices based on current filter settings and collapsed depth.
        // Creates or sets an element in the rows array for each visible line in this Record.
        // Parameter curRowIndex will be the row index of the first visible
        // line in this record, if there is one.  If any lines are visible, this increments
        // curRowIndex and returns the index of the next row to be set.
        public int SetVisibleRows(List<Row> rows, int curRowIndex) {
            int firstRowIndex = curRowIndex;
            FirstRowIndex = -1; // None visible for now.

            try
            {
                if (CollapsedDepth == 0 &&
                    Session.Visible &&
                    ThreadName.Visible &&
                    Thread.Visible &&
                    Logger.Visible &&
                    (MethodName.Visible || CallerIsVisible()) &&
                    (Level & MainForm.TheMainForm.VisibleTraceLevels) != 0) //
                {
                    if (HasNewlines && IsCollapsed)
                    {
                        // Test all the lines appended together, as the user will see it.
                        string allTogether = GetLine(0, ' ', 0, false);
                        if (FilterDialog.TextFilterTestString(allTogether))
                        {
                            SetRow(rows, curRowIndex, 0);
                            RowIndices[0] = curRowIndex; // Maybe we should set all of them?
                            FirstRowIndex = curRowIndex;
                            ++curRowIndex;
                        }
                    }
                    else
                    {
                        // Check if each line passes the text filter and associate each
                        // visible line with a Row in the rows array.
                        for (int lineNum = 0; lineNum < Lines.Length; ++lineNum)
                        {
                            if (FilterDialog.TextFilterTestString(Lines[lineNum]))
                            {
                                SetRow(rows, curRowIndex, lineNum);
                                RowIndices[lineNum] = curRowIndex;
                                FirstRowIndex = firstRowIndex;
                                ++curRowIndex;
                            }
                            else
                            {
                                RowIndices[lineNum] = -1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }

            return curRowIndex;
        }

        private bool CallerIsVisible() {
            bool result = false;

            if (Settings.Default.ShowCalledMethods) {
                Record caller = Caller;

                while (caller != null) {
                    if (caller.MethodName.Visible) {
                        result = true;
                        break;
                    }

                    caller = caller.Caller;
                }
            }

            return result;
        }

        public int LastRowIndex {
            get {
                if (IsCollapsed || !HasNewlines) {
                    return FirstRowIndex;
                } else {
                    int result = -1;
                    for (int lineNum = Lines.Length - 1; result == -1 && lineNum >= 0; --lineNum) {
                            result = RowIndices[lineNum];
                    }

                    return result;
                }
            }
        }

        public bool SameThreadAs(Record other) {
            if (Settings.Default.SearchThreadsByName) {
                return other != null && other.ThreadName == this.ThreadName;
            } else {
                return other != null && other.Thread == this.Thread;
            }
        }

        // Assign the record to the specified row, reusing the
        // Row object or creating a new one if needed.
        private void SetRow(List<Row> rows, int rowNum, int lineNum) {
            if (rowNum >= rows.Count) {
                rows.Add(new Row(this, rowNum, lineNum));
            } else if (rows[rowNum] == null) {
                rows[rowNum] = new Row(this, rowNum, lineNum);
            } else {
                rows[rowNum].Init(this, lineNum);
            }
        }

        // Gets the string to display in the Line Number column
        // for the specified sub-line number.
        public string GetRecordNum(int lineNum) {
            return GetRecordNum(lineNum, Settings.Default.LineNumSeparator);
        }

        // Gets the string to display in the Line Number column
        // for the specified sub-line number.
        public string GetRecordNum(int lineNum, bool useSeparator) {
            if (HasNewlines && !IsCollapsed) {
                if (useSeparator) return string.Format("{0:N0}.{1:N0}", MsgNum, lineNum);
                else return string.Format("{0}.{1}", MsgNum, lineNum);
            } else {
                if (useSeparator) return MsgNum.ToString("N0");
                else return MsgNum.ToString();
            }
        }

        // Append all lines together to be displayed in the full text window.
        // Ensure the lines are separated by "\r\n", not just '\n'.
        public string GetTextForWindow(int lineNum) {
            if (HasNewlines && IsCollapsed) {
                bool hasCR = true;
                StringBuilder builder = new StringBuilder(500);
                foreach (string line in Lines) {
                    if (builder.Length != 0) {
                        if (hasCR)
                            builder.Append('\n');
                        else
                            builder.Append("\r\n");
                    }

                    builder.Append(line);
                    hasCR = line.EndsWith("\r");
                }

                return builder.ToString();
            } else {
                return GetIndentedLine(lineNum, ' ', 0, false);
            }
        }

        // Gets the specified sub-line of this record's message.
        // Optionally truncates at 259 chars to prevent crash in comctrl32.dll.
        public string GetLine(int lineNum, char indentChar, int indentRate, bool truncate) {
            if (HasNewlines) {
                if (IsCollapsed) {
                    // Construct a string with all lines concatenated together as originally logged.
                    StringBuilder builder = new StringBuilder(500);
                    builder.Append(GetIndentedLine(0, indentChar, indentRate, truncate));
                    for (int i = 1; i < Lines.Length && (!truncate || builder.Length < MaxViewableChars); ++i) {
                        builder.Append("\n");
                        builder.Append(Lines[i]);
                    }

                    if (truncate && builder.Length > MaxViewableChars) builder.Length = MaxViewableChars;
                    return builder.ToString();
                } else {
                    // Return the specified Line, indented appropriately.
                    return GetIndentedLine(lineNum, indentChar, indentRate, truncate);
                }
            } else {
                // No newlines means entire message text is in Lines[0].
                return GetIndentedLine(0, indentChar, indentRate, truncate);
            }
        }

        // Return the specified Line, indented appropriately, possibly truncated to MaxViewableChars.
        private string GetIndentedLine(int lineNum, char indentChar, int indentRate, bool truncate) {
            string retval;

            if (indentRate == 0) {
                retval = Lines[lineNum];
            } else {
                retval = Lines[lineNum].PadLeft(indentRate * StackDepth + Lines[lineNum].Length, indentChar);
            }

            if (truncate && retval.Length > MaxViewableChars)
                retval = retval.Remove(MaxViewableChars);

            return retval;
        }

        private static readonly char[] _splitArg = new char[] {'\n'};

    }
}
