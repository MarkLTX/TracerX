using System;
using System.ComponentModel;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace TracerX
{
    /// <summary>
    /// Handy pattern for creating, acquiring, releasing, and disposing a named mutex.
    /// The constructor creates and acquires.  Dispose() releases and disposes.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)] // Hide this class from Intellisense.
    public class NamedMutexWait : IDisposable
    {
        /// <summary>
        /// Creates and acquires a Mutex with the specified name 
        /// (should be prefixed with "Global\" or "Local\").
        /// If timeoutMs is not greater than 0, the wait is infinite.
        /// </summary>
        public NamedMutexWait(string name, int timeoutMs, bool throwOnTimeout)
        {
            MutexAccessRule rule = new MutexAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MutexRights.Synchronize | MutexRights.Modify,
                AccessControlType.Allow);
            MutexSecurity mSec = new MutexSecurity();
            mSec.AddAccessRule(rule);

#if NETFRAMEWORK
            _mutex = new Mutex(false, name, out DidCreate, mSec);
#elif NETCOREAPP
            _mutex = new Mutex(false, name, out DidCreate);
            _mutex.SetAccessControl(mSec);
#endif

            try
            {
                if (timeoutMs <= 0)
                {
                    DidAcquire = _mutex.WaitOne(Timeout.Infinite, false);
                }
                else
                {
                    DidAcquire = _mutex.WaitOne(timeoutMs, false);
                }

                if (DidAcquire == false && throwOnTimeout)
                {
                    throw new TimeoutException("Timeout waiting for exclusive access on NamedMutex");
                }
            }
            catch (AbandonedMutexException)
            {
                DidAcquire = true;
            }
        }

        /// <summary>
        /// Name of global mutex that serializes access to files in the global TracerX data folder.
        /// </summary>
        public const string DataDirMUtexName = @"Global\TracerXDataDir";

        /// <summary>True if the mutex had to be created, i.e. didn't already exist. </summary>
        public readonly bool DidCreate;

        /// <summary>True if the mutex was successfully acquired. </summary>
        public readonly bool DidAcquire;
        
        private Mutex _mutex;

        /// <summary>
        /// Releases and disposes the mutex.
        /// </summary>
        public void Dispose()
        {
            if (_mutex != null)
            {
                if (DidAcquire)
                {
                    _mutex.ReleaseMutex();
                }

                (_mutex as IDisposable).Dispose();
            }
        }
    }
}
