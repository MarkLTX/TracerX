using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
//using TracerX.Forms;

namespace TracerX
{
    internal static class LoggerObjects
    {

        // Lock this when accessing the AllLoggers collection.
        public static object Lock = new object();

        // List of all created LoggerObjects.
        public static List<LoggerObject> AllLoggers = new List<LoggerObject>();

        public static void Clear() 
        {
            lock (Lock)
            {
                AllLoggers = new List<LoggerObject>();

                // Since there collection is empty, none are invisible.

                if (_invisibleCount != 0)
                {
                    _invisibleCount = 0;
                    OnAllVisibleChanged();
                }
            }
        }

        public static void RemoveSubitemColors()
        {
            lock (Lock)
            {
                foreach (LoggerObject lo in AllLoggers)
                {
                    lo.SubitemColors = null;
                }
            }
        }

        public static void TrackSubitemColors(HashSet<ColorPair> usedColors)
        {
            lock (Lock)
            {
                foreach (LoggerObject lo in AllLoggers)
                {
                    usedColors.Add(lo.SubitemColors);
                }
            }
        }

        public static void Add(LoggerObject lo)
        {
            lock (Lock)
            {
                AllLoggers.Add(lo);

                if (!lo.Visible)
                {
                    IncrementInvisibleCount();
                }
            }
        }

        public static void IncrementInvisibleCount()
        {
            lock (Lock)
            {
                if (++_invisibleCount == 1)
                {
                    OnAllVisibleChanged();
                }
            }
        }

        public static void DecrementInvisibleCount()
        {
            lock (Lock)
            {
                if (--_invisibleCount == 0)
                {
                    OnAllVisibleChanged();
                }
            }
        }

        /// <summary>
        /// Are all loggers visible?
        /// </summary>
        public static bool AllVisible { get { lock (Lock) return _invisibleCount == 0; } }

        /// <summary>
        /// Event called when AllVisible changes.
        /// </summary>
        public static event EventHandler AllVisibleChanged;

        private static void OnAllVisibleChanged()
        {
            if (AllVisibleChanged != null)
            {
                AllVisibleChanged(null, null);
            }
        }

        //// Count the visible loggers in AllLoggers, set the internal count (invisibleCount) accordingly, 
        //// and raise the AllVisibleChanged event if the new count differs from the current count.
        //public static void RecountLoggers()
        //{
        //    lock (Lock)
        //    {
        //        int oldCount = _invisibleCount;
        //        _invisibleCount = 0;

        //        foreach (LoggerObject logger in AllLoggers)
        //        {
        //            if (!logger.Visible) ++_invisibleCount;
        //        }

        //        if (_invisibleCount != oldCount)
        //        {
        //            if (_invisibleCount == 0 || oldCount == 0)
        //            {
        //                OnAllVisibleChanged();
        //            }
        //        }
        //    }
        //}

        private static int _invisibleCount;

        public static void ShowAllLoggers()
        {
            lock (Lock)
            {
                foreach (LoggerObject l in AllLoggers)
                {
                    l.Visible = true;
                }
            }
        }

        public static void HideAllLoggers()
        {
            lock (Lock)
            {
                foreach (LoggerObject l in AllLoggers)
                {
                    l.Visible = false;
                }
            }
        }
    }

    class LoggerObject : IFilterable
    {
        public string Name { get; set; }

        public ColorPair RowColors { get; set; }
        public ColorPair SubitemColors { get; set; }

        protected bool _visible = true;

        /// <summary>
        /// Is the output from this logger visible?
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;

                    // Track the number of filtered loggers.
                    if (_visible)
                    {
                        LoggerObjects.DecrementInvisibleCount();
                    }
                    else
                    {
                        LoggerObjects.IncrementInvisibleCount();
                    }
                }
            }
        }
    }
}
