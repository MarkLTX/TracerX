// The viewer's enums.
using System;

namespace TracerX {
    // Values must match those in Logger project.
    // Marked "internal" in viewer, "public" in logger.
    [Flags]
    internal enum TraceLevel : byte {
        /// <summary> 
        /// A Logger with an Inherited trace level inherits its TraceLevel from its parent. 
        /// </summary>
        Inherited = 0,

        /// <summary>
        /// Replaced by Inherited.
        /// </summary>
        [Obsolete("Use 'Inherited' instead of 'Undefined'", false)]
        Undefined = Inherited,

        /// <summary> Turns logging off for the Logger/destination with this level. </summary>
        Off = 1,

        /// <summary> Allows only Fatal level messages to be logged. </summary>
        Fatal = 2,

        /// <summary> Allows only Fatal and Error level messages to be logged. </summary>
        Error = 4,

        /// <summary> Allows only Fatal through Warn level messages to be logged. </summary>
        Warn = 8,

        /// <summary> Allows only Fatal through Info level messages to be logged. </summary>
        Info = 16,

        /// <summary> Allows only Fatal through Debug level messages to be logged. </summary>
        Debug = 32,

        /// <summary> Allows only Fatal through Verbose level messages to be logged. </summary>
        Verbose = 64,
    }

    /// <summary> One of these is prepended to every logged message to indicate what data is present. </summary>
    [Flags]
    internal enum DataFlags : ushort {
        None =        0,      // No bits are set.
        Zero1 =       1,      // This bit should always be 0.
        Zero2 =       1 << 1, // This bit should always be 0.
        Message =     1 << 2, // Line has non-empty message text.
        NewSession =  1 << 3, // Indicates the start of a new session (appended log), but should always be 0 otherwise.
        Time =        1 << 4, // Line contains 8-byte tick value compatible with DateTime.UTC.
        BlockStart =  1 << 5, // Indicates the first line in a block in the circular part of the file. 
        CircularStart =  BlockStart, // Indicates the start of the circular part of the file.  Replaced by BlockStart in version 6+.
        MethodEntry = 1 << 6, // Indicates stack depth should increase.
        MethodExit =  1 << 7, // Indicates stack depth should decrease. Includes line number of corresponding MethodEntry.
        MethodName =  1 << 8, // Line includes method name (e.g. because it's the first line from the method in the block or it differs from the previous line).
        ThreadId =    1 << 9, // TracerX's internal thread ID is included when the thread for this line is different from the previous line and on the first line in each block.
        ThreadName =  1 << 10,// .NET thread name is included when the thread's name has changed and on each thread's first line in each block.
        TraceLevel =  1 << 11,// TraceLevel is included when it differs from the previous line logged by the calling thread and on the first line logged to the current block by the calling thread.
        LoggerName =  1 << 12,// Included when the logger name changes from line to line for the calling thread and on the first line for each thread in each block.
        StackDepth =  1 << 13,// Included when the stack depth can't be calculated from other data. I.E. on each thread's first line in each block.
        Zero3 =       1 << 14,// This bit should always be 0.
        Zero4 =       1 << 15,// This bit should always be 0.
        InvalidOnes = Zero1 | Zero2 | Zero3 | Zero4, // Invalid value if any are set.
    }

}
