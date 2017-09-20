using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
//using TracerX.Forms;

namespace TracerX
{
    internal static class TraceLevelObjects
    {
        static TraceLevelObjects()
        {
            // We pre-populate this so all the levels appear in the Filter and Color dialogs.

            AllTraceLevels.Add(TraceLevel.Fatal, new TraceLevelObject(TraceLevel.Fatal));
            AllTraceLevels.Add(TraceLevel.Error, new TraceLevelObject(TraceLevel.Error));
            AllTraceLevels.Add(TraceLevel.Warn, new TraceLevelObject(TraceLevel.Warn));
            AllTraceLevels.Add(TraceLevel.Info, new TraceLevelObject(TraceLevel.Info));
            AllTraceLevels.Add(TraceLevel.Debug, new TraceLevelObject(TraceLevel.Debug));
            AllTraceLevels.Add(TraceLevel.Verbose, new TraceLevelObject(TraceLevel.Verbose));
        }

        // Lock this when accessing the AllTraceLevels collection.
        public static object Lock = new object();

        public static Dictionary<TraceLevel, TraceLevelObject> AllTraceLevels = new Dictionary<TraceLevel, TraceLevelObject>();

        public static void RemoveSubitemColors()
        {
            lock (Lock)
            {
                foreach (TraceLevelObject lo in AllTraceLevels.Values)
                {
                    lo.SubitemColors = null;
                }
            }
        }

        public static void TrackSubitemColors(HashSet<ColorPair> usedColors)
        {
            lock (Lock)
            {
                foreach (TraceLevelObject lo in AllTraceLevels.Values)
                {
                    usedColors.Add(lo.SubitemColors);
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

        //// Count the visible trace levels in AllTraceLevels, set the internal count (invisibleCount) accordingly, 
        //// and raise the AllVisibleChanged event if the new count differs from the current count.
        //public static void RecountTraceLevels()
        //{
        //    lock (Lock)
        //    {
        //        int oldCount = _invisibleCount;
        //        _invisibleCount = 0;

        //        foreach (TraceLevelObject level in AllTraceLevels.Values)
        //        {
        //            if (!level.Visible) ++_invisibleCount;
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

        public static void ShowAllLevels()
        {
            lock (Lock)
            {
                foreach (TraceLevelObject level in AllTraceLevels.Values)
                {
                    level.Visible = true;
                }
            }
        }

        public static void HideAllLevels()
        {
            lock (Lock)
            {
                foreach (TraceLevelObject level in AllTraceLevels.Values)
                {
                    level.Visible = false;
                }
            }
        }
    }

    class TraceLevelObject : IFilterable
    {
        public TraceLevelObject(TraceLevel level)
        {
            TLevel = level;
            Name = level.ToString();

            if ((Properties.Settings.Default.ColoredLevels & (int)level) != 0)
            {
                RowColors = ColorUtil.TraceLevelPalette[level];
            }
        }

        public TraceLevel TLevel { get; set; }
        public string Name { get; set; }
        public ColorPair RowColors { get; set; }
        public ColorPair SubitemColors { get; set; }

        protected bool _visible = true;

        /// <summary>
        /// Is the output from this trace level visible?
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;

                    // Track the number of filtered trace levels.

                    if (_visible)
                    {
                        TraceLevelObjects.DecrementInvisibleCount();
                    }
                    else
                    {
                        TraceLevelObjects.IncrementInvisibleCount();
                    }
                }
            }
        }
    }
}
