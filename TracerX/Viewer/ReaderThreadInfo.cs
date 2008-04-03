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

        // NonCircularStack is a stack of MethodEntry records found in the log whose
        // corresponding MethodExit records were not found prior to entering the circular part of the log.
        // This was added in format version 5 to detect MethodEntry
        // and MethodExit records lost due to wrapping so they can be 
        // generated for the viewer.
        public Stack<Record> NonCircularStack = new Stack<Record>();

        // Missing entry and exit records are generated when we enter the circular part of the log by
        // comparing the call stack from the noncircular part of the log to the actual call
        // stack recorded in the circular part of the log with the first line of output
        // for each thread.
        public List<Record> MissingEntryRecords;
        public List<Record> MissingExitRecords;

        // Called when a MethodEntry line is read.
        public void Push(Record entryRec) {
            if (NonCircularStack != null) NonCircularStack.Push(entryRec);
        }

        // Called when a MethodExit line is called.
        // The matchingEntryLine parameter is the line
        // number of the MethodEntry that corresponds to the exitRec.
        public void Pop() {
            if (NonCircularStack != null) NonCircularStack.Pop();
        }

        // Called when we enter the circular part of the log and the thread's true call stack
        // (logged with each thread's first record in each block) is read from the log.  
        // This is enough info to generate replacement entry/exit records that were lost when
        // the log wrapped, but whose counterparts were not lost.  
        internal void MakeMissingRecords(ReaderStackEntry[] circularStack) {
            if (NonCircularStack != null) {
                MissingEntryRecords = new List<Record>();
                MissingExitRecords = new List<Record>();

                // The deepest entry comes last in circularStack.
                int trueDepth = circularStack == null ? 0 : circularStack.Length ;

                // If the true stack depth is greater than the stack depth we got
                // by pushing MehtodEntry records and popping MethodExit records,
                // we must generate the missing MethodEntry records.
                while (trueDepth > NonCircularStack.Count) {
                    // This constructs an entry record.
                    MissingEntryRecords.Add(new Record(this, circularStack[--trueDepth]));
                }

                // If the true stack depth is less than the stack depth we got
                // by pushing MehtodEntry records and popping MethodExit records,
                // we must generate the missing MethodExit records.
                while (trueDepth < NonCircularStack.Count) {
                    // This constructs an exit record.
                    MissingExitRecords.Add(new Record(NonCircularStack.Pop()));
                }

                // Now the two stacks are the same depth.  
                // Wherever two entries don't match, we are missing
                // both an entry and an exit record.
                while (NonCircularStack.Count > 0) {
                    Record parsedEntry = NonCircularStack.Pop(); 
                    ReaderStackEntry trueEntry = circularStack[--trueDepth];

                    if (parsedEntry.MsgNum == trueEntry.EntryLineNum) {
                        // All the other ones will also match.
                        break;
                    } else {
                        MissingEntryRecords.Add(new Record(this, trueEntry));
                        MissingExitRecords.Add(new Record(parsedEntry));
                    }
                } 

                NonCircularStack = null; // We're done.
            }
        }
    }

    internal class ReaderStackEntry {
        public uint EntryLineNum;
        public byte Depth;
        public TraceLevel Level;
        public LoggerObject Logger;
        public string Method;
    }

}
