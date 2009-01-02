using System;
using System.Collections.Generic;
using System.Text;

namespace TracerX.Viewer {
    internal class ThreadName {
        public string Name;
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
        public static List<ThreadName> AllThreadNames = new List<ThreadName>();

        protected static int _invisibleCount;

        /// <summary>
        /// Are all threads visible?
        /// </summary>
        public static bool AllVisible { get { return _invisibleCount == 0; } }

        /// <summary>
        /// Event called when AllVisible changes.
        /// </summary>
        public static event EventHandler AllVisibleChanged;

        // Count the visible threads in AllThreads, set the internal count (_invisibleThreads) accordingly, 
        // and raise the AllVisibleChanged event if the new count differs from the current count.
        public static void RecountThreads() {
            int invisibleCount = 0;

            foreach (ThreadName thread in AllThreadNames) {
                if (!thread.Visible) ++invisibleCount;
            }

            if (_invisibleCount != invisibleCount) {
                _invisibleCount = invisibleCount;
                OnAllVisibleChanged();
            }
        }

        public static void HideAllThreads() {
            foreach (ThreadName t in AllThreadNames) {
                t.Visible = false;
            }
        }

        public static void ShowAllThreads() {
            foreach (ThreadName t in AllThreadNames) {
                t.Visible = true;
            }
        }

        protected static void OnAllVisibleChanged() {
            if (AllVisibleChanged != null) {
                AllVisibleChanged(null, null);
            }
        }
    }
}
