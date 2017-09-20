using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TracerX
{
    // Used only while reading.
    internal class ReaderThreadInfo
    {
        public ThreadObject Thread;
        public ThreadName ThreadName;
        public LoggerObject Logger;
        public MethodObject MethodName;
        public TraceLevelObject TLevel;
        //public TraceLevel Level;
        public byte Depth;
        public Record StackTop;

        private bool _missingRecsGenerated;

        // Called when a MethodEntry line is read.
        public void Push(Record entryRec)
        {
            StackTop = entryRec;
        }

        // Called when a MethodExit line is read.
        public void Pop()
        {
            StackTop = StackTop.Caller;
        }

        // This generates replacements for missing entry/exit records that were lost when
        // the log wrapped, but whose counterparts were not lost.  
        // Called when we read the first line for this thread in the circular part of the log.
        // The thread's true call stack (logged with each thread's first record in each block) 
        // was just read from the log and is passed via the actualStack parameter.  
        internal void MakeMissingRecords(ExplicitStackEntry[] actualStack, List<Record> generatedRecs, Reader.Session session)
        {
            // We only do this once per thread, for the first record in the circular log for each
            // thread.  If MissingEntryRecords is not null, we already did it.
            if (!_missingRecsGenerated)
            {
                _missingRecsGenerated = true;
                var MissingEntryRecords = new List<Record>();

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
                // If this.Depth is 0, actualStack is null.
                // The top stack entry comes first in actualStack.
                int actualStackIndex = 0;

                // The key property of each stack entry is the line number where
                // each method call starts in the log.  For example, suppose the StackTop
                // stack and the actualStack contain entries with the 
                // following line numbers.
                //
                // StackTop:    400 300 200 100
                // actualStack: 600 500 100
                //
                // In the example, the StackTop calls at 400, 300, and 200 must have exited 
                // in the lost part of the log (because they don't appear in actualStack),
                // so we generate MethodExit records to replace those that were lost.  
                // We also pop the entries for 400, 300, and 200 off the StackTop stack.  
                //
                // The calls at 600 and 500 occurred in the lost part of the log and have 
                // not exited yet, so we generate MethodEntry records to replace those lost 
                // records.  We also push the generated MethodEntry records onto the 
                // StackTop stack.
                //
                // The method call at line 100 occurred before the lost part of the log and 
                // still has not exited (because it appears in both stacks). 
                //
                // All the generated MethodExit records will be inserted before all the
                // generated MethodEntry records, and all pops must be done before all pushes.

                // We start at the top of each stack and loop until all entries are examined or
                // we find the point where both stacks match.
                while (StackTop != null || actualStackIndex < Depth)
                {
                    // At least one of the stacks is not exhausted.
                    if (StackTop == null)
                    {
                        // Only the actualStack has entries remaining, all of which represent
                        // methods whose method entry records were lost.
                        MissingEntryRecords.Add(new Record(this, actualStack[actualStackIndex], session));
                        ++actualStackIndex;
                    }
                    else if (actualStackIndex == Depth)
                    {
                        // Only the StackTop stack has entries remaining, all of which represent
                        // methods whose exits were lost.
                        generatedRecs.Add(new Record(StackTop));
                        Pop();
                    }
                    else if (StackTop.MsgNum > actualStack[actualStackIndex].EntryLineNum)
                    {
                        generatedRecs.Add(new Record(StackTop));
                        Pop();
                    }
                    else if (StackTop.MsgNum < actualStack[actualStackIndex].EntryLineNum)
                    {
                        MissingEntryRecords.Add(new Record(this, actualStack[actualStackIndex], session));
                        ++actualStackIndex;
                    }
                    else
                    {
                        // Once they are equal, all others will be equal.
                        break;
                    }
                }

                // Now do the Pushes for the generated entry records.
                MissingEntryRecords.Reverse();
                foreach (Record entryRec in MissingEntryRecords)
                {
                    entryRec.Caller = StackTop;
                    Push(entryRec);
                }

                generatedRecs.AddRange(MissingEntryRecords);
            }
        }
    }

    // An array of these, representing the current call stack, is read from the
    // first record for each thread in each block in the circular log.
    internal class ExplicitStackEntry
    {
        public ulong EntryLineNum;
        public byte Depth;
        //public TraceLevel Level;
        public TraceLevelObject TLevel;
        public LoggerObject Logger;
        public MethodObject Method;
    }
}
