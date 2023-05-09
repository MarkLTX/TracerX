using System;
using System.ComponentModel;

namespace TracerX
{

    /// <summary>
    /// The Logger.*Call methods log a call to a method and return an instance of this class. 
    /// Its Dispose method logs the exit of the call.
    /// Users should not create instances of this object or call its methods.  
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public class CallEnder : MarshalByRefObject, IDisposable  // MarshalByRefObject is needed because InfoCall() returns CallEnder, and this supports passing Loggers to other AppDomains.
    {
        /// <summary>
        /// Internal ctor keeps logging clients from creating instances.
        /// </summary>
        internal CallEnder(ThreadData threadData) 
        {
            ThreadData = threadData;
        }

        internal ThreadData ThreadData;

        /// <summary>
        /// If MaybeLogCall() logged entry into a call, this logs the exit.
        /// </summary>
        public void Dispose()
        {
            ThreadData.LogCallExit();
        }
    } // CallEnder
}
