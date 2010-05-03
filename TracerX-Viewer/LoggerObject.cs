using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TracerX.Forms;

namespace TracerX.Viewer {
    internal static class LoggerObjects {

        // Lock this when accessing the AllLoggers collection.
        public static object Lock = new object();

        // List of all created LoggerObjects.
        public static List<LoggerObject> AllLoggers = new List<LoggerObject>();

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

        /// <summary>
        /// Are all loggers visible?
        /// </summary>
        public static bool AllVisible { get { lock(Lock) return _invisibleCount == 0; } }

        /// <summary>
        /// Event called when AllVisible changes.
        /// </summary>
        public static event EventHandler AllVisibleChanged;

        private static void OnAllVisibleChanged() {
            if (AllVisibleChanged != null) {
                AllVisibleChanged(null, null);
            }
        }

        // Count the visible loggers in AllLoggers, set the internal count (invisibleCount) accordingly, 
        // and raise the AllVisibleChanged event if the new count differs from the current count.
        public static void RecountLoggers() {
            lock (Lock) {
                int oldCount = _invisibleCount;
                _invisibleCount = 0;

                foreach (LoggerObject logger in AllLoggers) {
                    if (!logger.Visible) ++_invisibleCount;
                }

                if (_invisibleCount != oldCount) {
                    if (_invisibleCount == 0 || oldCount == 0) {
                        OnAllVisibleChanged();
                    }
                }
            }
        }

        private static int _invisibleCount;

        public static void ShowAllLoggers() {
            lock (Lock) {
                foreach (LoggerObject l in AllLoggers) {
                    l.Visible = true;
                }
            }
        }

        public static void HideAllLoggers() {
            lock (Lock) {
                foreach (LoggerObject l in AllLoggers) {
                    l.Visible = false;
                }
            }
        }
    }

    class LoggerObject : IFilterable {
        public string Name { get; set; }

        public ColorRulesDialog.ColorPair Colors { get; set; }

        protected bool _visible = true;

        /// <summary>
        /// Is the output from this logger visible?
        /// </summary>
        public bool Visible {
            get { return _visible; }
            set {
                if (_visible != value) {
                    _visible = value;

                    // Track the number of filtered loggers.
                    if (_visible) {
                        LoggerObjects.DecrementInvisibleCount();
                    } else {
                        LoggerObjects.IncrementInvisibleCount();
                    }
                }
            }
        }
    }
}
