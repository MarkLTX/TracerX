using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace TracerX
{

    /// <summary>
    /// The Logger.*Call methods log entry to a method and return an instance of this class. 
    /// Exit from the method is logged when Dispose() is called or when an "await" causes control
    /// to return to the caller, whichever happens first.
    /// Users should not create instances of this object or call its methods.  
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public class CallEnder : MarshalByRefObject, IDisposable
    {
        internal CallEnder(ThreadData threadData)
        {
            _threadData = threadData;

#if NET46 || NETCOREAPP3_1
            // AsyncLocal was added to .NET in .NET Framework 4.6.
            _asyncLocal = new AsyncLocal<string>(AsyncLocalChanged)  { Value = "Doesn't matter" };
#endif
        }

#if  NET46 || NETCOREAPP3_1
        private AsyncLocal<string> _asyncLocal;
        private void AsyncLocalChanged(AsyncLocalValueChangedArgs<string> obj)
        {
            if (obj.ThreadContextChanged && _threadData != null && _threadData == ThreadData.CurrentThreadData)
            {
                _threadData.LogCallExit(false);
                _threadData = null;
                _asyncLocal = null;
            }
        }

#endif

        private ThreadData _threadData;

        /// <summary>
        /// If MaybeLogCall() logged entry into a call, this logs the exit.
        /// </summary>
        public void Dispose()
        {
            _threadData?.LogCallExit(true);
            _threadData = null;

#if NET46 || NETCOREAPP3_1
            _asyncLocal = null;
#endif
        }
    } // CallEnder
}
