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

        // The CallStack is a stack of MethodEntry records.
        // It was added in format version 5 to detect MethodEntry
        // and MethodExit records lost due to wrapping so they can be 
        // generated for the viewer.
        public Stack<Record> CallStack = new Stack<Record>();
        public List<Record> MissingEntries = new List<Record>();
        public List<Record> MissingExits = new List<Record>();

        // Called when a MethodEntry line is read.
        public void Push(Record entryRec) {
            CallStack.Push(entryRec);
        }

        // Called when a MethodExit line is called.
        // The matchingEntryLine parameter is the line
        // number of the MethodEntry that corresponds to the exitRec.
        public void Pop(Record exitRec, uint matchingEntryLine) {
            if (CallStack.Count == 0 || matchingEntryLine > CallStack.Peek().MsgNum) {
                // The corresponding entry record is missing. Generate it.
                MissingEntries.Add(new Record(exitRec));
            } else {
                // Pop MethodEntry records off the stack until we find the matching one.
                // Those that don't match have missing MethodExit records we need to generate.
                while (CallStack.Count > 0) {
                    Record popped = CallStack.Pop();
                    if (matchingEntryLine < popped.MsgNum) {
                        Record missingExit = new Record(popped);
                        //missingExit.StackDepth = (byte)CallStack.Count; // Hope this is always accurate.
                        MissingExits.Add(missingExit);
                    } else {
                        Debug.Assert(matchingEntryLine == popped.MsgNum);
                        break;
                    }
                }
            }
        }
    }
}
