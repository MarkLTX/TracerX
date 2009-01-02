using System;
using System.Collections.Generic;
using System.Text;

namespace TracerX.Viewer {
    // One of these exists for every unique thread id found in the file.
    internal class ThreadObject {
        // The thread's ID number (not the ManagedThreadId).
        public int Id;

        // Is this thread visible (filtered in)?
        protected bool _visible = true;

        /// <summary>
        /// Is the output from this thread visible?
        /// </summary>
        public bool Visible {
            get { return _visible; }
            set {
                if (_visible != value) {
                    _visible = value;

                    // Track the number of filtered threads.
                    if (_visible) {
                        if (--_invisibleCount == 0) OnAllVisibleChanged();
                    } else {
                        if (++_invisibleCount == 1) OnAllVisibleChanged();
                    }
                }
            }
        }

        // List of all created ThreadObjects.
        public static List<ThreadObject> AllThreads = new List<ThreadObject>();

        // How many threads are invisible (filtered out)?
        protected static int _invisibleCount;

        // Count the visible threads in AllThreads, set the internal count (_invisibleThreads) accordingly, 
        // and raise the AllVisibleChanged event if the new count differs from the current count.
        public static void RecountThreads() {
            int invisibleCount = 0;

            foreach (ThreadObject thread in AllThreads) {
                if (!thread.Visible) ++invisibleCount;
            }

            if (_invisibleCount != invisibleCount) {
                _invisibleCount = invisibleCount;
                OnAllVisibleChanged();
            }
        }

        /// <summary>
        /// Are all threads visible?
        /// </summary>
        public static bool AllVisible { get { return _invisibleCount == 0; } }

        /// <summary>
        /// Event called when AllVisible changes.
        /// </summary>
        public static event EventHandler AllVisibleChanged;

        public static void HideAllThreads() {
            foreach (ThreadObject t in AllThreads) {
                t.Visible = false;
            }
        }

        public static void ShowAllThreads() {
            foreach (ThreadObject t in AllThreads) {
                t.Visible = true;
            }
        }
        
        /// <summary>
        /// Reset is called before loading a new logfile.
        /// </summary>
        public static void Reset() {
            AllThreads.Clear();
            if (_invisibleCount > 0) {
                _invisibleCount = 0;
                OnAllVisibleChanged();
            }
        }

        protected static void OnAllVisibleChanged() {
            if (AllVisibleChanged != null) {
                AllVisibleChanged(null, null);
            }
        }
    }
}
