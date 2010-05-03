using System;
using System.Collections.Generic;
using System.Text;
using TracerX.Forms;

namespace TracerX.Viewer {

    // This class manages the collection of ThreadObjects for the viewer.
    internal static class ThreadObjects {

        // Lock this when accessing the collection.
        public static object Lock = new object();

        // List of all created ThreadObjects.
        public static List<ThreadObject> AllThreadObjects = new List<ThreadObject>();

        public static void IncrementInvisibleCount() {
            lock (Lock) {
                if (++_invisibleCount == 1) {
                    OnAllVisibleChanged();
                }
            }
        }

        public static void DecrementInvisibleCount() {
            lock (Lock) {
                if (--_invisibleCount == 0) {
                    OnAllVisibleChanged();
                }
            }
        }

        // How many threads are invisible (filtered out)?
        private static int _invisibleCount;

        // Count the visible threads in AllThreadObjects, set the internal count (_invisibleThreads) accordingly, 
        // and raise the AllVisibleChanged event if the new count differs from the current count.
        public static void RecountThreads() {
            lock (Lock) {
                int oldCount = _invisibleCount;
                _invisibleCount = 0;

                foreach (ThreadObject th in AllThreadObjects) {
                    if (!th.Visible) ++_invisibleCount;
                }

                if (_invisibleCount != oldCount) {
                    if (_invisibleCount == 0 || oldCount == 0) {
                        OnAllVisibleChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Are all threads visible?
        /// </summary>
        public static bool AllVisible { get { lock (Lock) return _invisibleCount == 0; } }

        /// <summary>
        /// Event called when AllVisible changes.
        /// </summary>
        public static event EventHandler AllVisibleChanged;

        public static void HideAllThreads() {
            lock (Lock) {
                foreach (ThreadObject t in AllThreadObjects) {
                    t.Visible = false;
                }
            }
        }

        public static void ShowAllThreads() {
            lock (Lock) {
                foreach (ThreadObject t in AllThreadObjects) {
                    t.Visible = true;
                }
            }
        }

        private static void OnAllVisibleChanged() {
            if (AllVisibleChanged != null) {
                AllVisibleChanged(null, null);
            }
        }
    }

    // One of these exists for every unique thread id found in the file.
    internal class ThreadObject : IFilterable {
        // The thread's ID number (not the ManagedThreadId).
        public int Id;

        public string Name
        {
            get { return Id.ToString(); }
            set { }
        }

        // What colors, if any does this thread have?  May be null.
        public ColorRulesDialog.ColorPair Colors { get; set; }

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

                    // Track the number of filtered (invisible) threads.
                    if (_visible) {
                        ThreadObjects.DecrementInvisibleCount();
                    } else {
                        ThreadObjects.IncrementInvisibleCount();
                    }
                }
            }
        }
    }
}
