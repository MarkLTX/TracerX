using System;
using System.Collections.Generic;
using System.Text;

namespace BBS.TracerX.Viewer {
    class LoggerObject {
        public string Name;
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
                        if (--_invisibleCount == 0) OnAllVisibleChanged();
                    } else {
                        if (++_invisibleCount == 1) OnAllVisibleChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Are all loggers visible?
        /// </summary>
        public static bool AllVisible { get { return _invisibleCount == 0; } }

        /// <summary>
        /// Event called when AllVisible changes.
        /// </summary>
        public static event EventHandler AllVisibleChanged;

        protected static void OnAllVisibleChanged() {
            if (AllVisibleChanged != null) {
                AllVisibleChanged(null, null);
            }
        }

        // Count the visible loggers in AllLoggers, set the internal count (invisibleCount) accordingly, 
        // and raise the AllVisibleChanged event if the new count differs from the current count.
        public static void RecountLoggers() {
            int invisibleCount = 0;

            foreach (LoggerObject logger in AllLoggers) {
                if (!logger.Visible) ++invisibleCount;
            }

            if (_invisibleCount != invisibleCount) {
                _invisibleCount = invisibleCount;
                OnAllVisibleChanged();
            }
        }

        protected static int _invisibleCount;

        public static void ShowAllLoggers() {
            foreach (LoggerObject l in AllLoggers) {
                l.Visible = true;
            }
        }

        public static void HideAllLoggers() {
            foreach (LoggerObject l in AllLoggers) {
                l.Visible = false;
            }
        }

        // List of all created LoggerObjects.
        public static List<LoggerObject> AllLoggers = new List<LoggerObject>();

    }
}
