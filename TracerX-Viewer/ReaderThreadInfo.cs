using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TracerX.Viewer {
    // Used only while reading.
    internal class ReaderThreadInfo {
        public ThreadObject Thread;
        public ThreadName ThreadName;
        public LoggerObject Logger;
        public string MethodName;
        public TraceLevel Level;
        public byte Depth;
        public Record StackTop;

        // Missing entry and exit records are generated when we enter the circular part of the log by
        // comparing the call stack from the noncircular part of the log to the actual call
        // stack recorded in the circular part of the log with the first line of output
        // for each thread.
        public List<Record> MissingEntryRecords;
        public List<Record> MissingExitRecords;

        // Called when a MethodEntry line is read.
        public void Push(Record entryRec) {
            StackTop = entryRec;
        }

        // Called when a MethodExit line is read.
        public void Pop() {
            StackTop = StackTop.Caller;
        }

        // This generates replacements for missing entry/exit records that were lost when
        // the log wrapped, but whose counterparts were not lost.  
        // Called when we read the first line for this thread in the circular part of the log.
        // The thread's true call stack (logged with each thread's first record in each block) 
        // was just read from the log and is passed via the actualStack parameter.  
        internal void MakeMissingRecords(ExplicitStackEntry[] actualStack) {
            // We only do this once per thread, for the first record in the circular log for each
            // thread.  If MissingEntryRecords is not null, we already did it.
            if (MissingEntryRecords == null) {
                MissingEntryRecords = new List<Record>();
                MissingExitRecords = new List<Record>();

                // StackTop is the "top" entry in the stack determined by 
                // pushing MethodEntry records and then popping 
                // them off when MethodExit records are found in the log.
                // That stack becomes inaccurate
                // due to records being lost when the log wraps.
                // However, the true stack for this thread was explicitly
                // recorded in the circular part of the log and has now been
                // passed to this method as actualStack.
                // By comparing the items in the two stacks, we can generate
                // the missing MethodEntry records (for items that are in
                // actualStack but not in the StackTop stack) and the missing
                // MethodExit records (for items that are in the StackTop stack
                // but not the actualStack).  We also "fix" the StackTop stack.

                // this.Depth was set to actualStack.Length before this method was called.
                // The top stack entry comes first in actualStack.
                // If this.Depth is 0, actualStack is null.
                int actualStackIndex = 0;

                // The key property of each stack entry is the line number where
                // each method call starts in the log.  For example, the StackTop
                // stack and the actualStack may contain entries with the 
                // following line numbers.
                // StackTop:    100 200 300 400
                // actualStack: 100 500 600
                // In the example, the method call at line 100 occurred before the lost
                // part of the log and still has not exited (because it also appears
                // in actualStack).  The calls at 200, 300, and 400 must have exited 
                // in the lost part of the log, so we generate MethodExit records to
                // replace those that were lost.  We also pop the entries for 200, 300, 
                // and 400 off the StackTop stack.  The calls at 500 and 600 occurred
                // in the lost part of the log and have not exited yet, so we generate
                // MethodEntry records to replace those lost records.  We also push
                // the generated MethodEntry records onto the StackTop stack.
                // All the generated MethodExit records will be inserted before all the
                // generated MethodEntry records, and all pops must be done before all pushes.

                // We start at the top of each stack and loop until all entries are examined or
                // we find the point where both stacks match.
                while (StackTop != null || actualStackIndex < Depth) {
                    if (StackTop == null) {
                        MissingEntryRecords.Add(new Record(this, actualStack[actualStackIndex]));
                        ++actualStackIndex;
                    } else if (actualStackIndex == -1) {
                        MissingExitRecords.Add(new Record(StackTop));
                        Pop();
                    } else if (StackTop.MsgNum > actualStack[actualStackIndex].EntryLineNum) {
                        MissingExitRecords.Add(new Record(StackTop));
                        Pop();
                    } else if (StackTop.MsgNum < actualStack[actualStackIndex].EntryLineNum) {
                        MissingEntryRecords.Add(new Record(this, actualStack[actualStackIndex]));
                        ++actualStackIndex;
                    } else {
                        // Once they are equal, all others will be equal.
                        break;
                    }
                }

                // Now do the Pushes for the generated entry records.
                MissingEntryRecords.Reverse();
                foreach (Record entryRec in MissingEntryRecords) {
                    entryRec.Caller = StackTop;
                    Push(entryRec);
                }
            }
        }
    }

    // An array of these, representing the current call stack, is read from the
    // first record for each thread in each block in the circular log.
    internal class ExplicitStackEntry {
        public uint EntryLineNum;
        public byte Depth;
        public TraceLevel Level;
        public LoggerObject Logger;
        public string Method;
    }

}
