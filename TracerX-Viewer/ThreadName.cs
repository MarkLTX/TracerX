using System;
using System.Collections.Generic;
using System.Text;
//using TracerX.Forms;

namespace TracerX
{

    internal static class ThreadNames
    {

        // Lock this when accessing the collection.
        public static object Lock = new object();

        // List of all created ThreadNames.
        public static List<ThreadName> AllThreadNames = new List<ThreadName>();

        private static int _invisibleCount;
        public static void Clear()
        {
            lock (Lock)
            {
                AllThreadNames = new List<ThreadName>();

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
                foreach (ThreadName tn in AllThreadNames)
                {
                    tn.SubitemColors = null;
                }
            }
        }

        public static void TrackSubitemColors(HashSet<ColorPair> usedColors)
        {
            lock (Lock)
            {
                foreach (ThreadName tn in AllThreadNames)
                {
                    usedColors.Add(tn.SubitemColors);
                }
            }
        }

        public static void Add(ThreadName tn)
        {
            lock (Lock)
            {
                AllThreadNames.Add(tn);

                if (!tn.Visible)
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
        /// Are all threads visible?
        /// </summary>
        public static bool AllVisible { get { lock (Lock) return _invisibleCount == 0; } }

        /// <summary>
        /// Event called when AllVisible changes.
        /// </summary>
        public static event EventHandler AllVisibleChanged;

        //// Count the visible threads in AllThreads, set the internal count (_invisibleThreads) accordingly, 
        //// and raise the AllVisibleChanged event if the new count differs from the current count.
        //public static void RecountThreads()
        //{
        //    lock (Lock)
        //    {
        //        int oldCount = _invisibleCount;
        //        _invisibleCount = 0;

        //        foreach (ThreadName th in AllThreadNames)
        //        {
        //            if (!th.Visible) ++_invisibleCount;
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

        public static void HideAllThreads()
        {
            lock (Lock)
            {
                foreach (ThreadName t in AllThreadNames)
                {
                    t.Visible = false;
                }
            }
        }

        public static void ShowAllThreads()
        {
            lock (Lock)
            {
                foreach (ThreadName t in AllThreadNames)
                {
                    t.Visible = true;
                }
            }
        }

        private static void OnAllVisibleChanged()
        {
            if (AllVisibleChanged != null)
            {
                AllVisibleChanged(null, null);
            }
        }
    }

    internal class ThreadName : IFilterable
    {
        public string Name { get; set; }

        public ColorPair RowColors { get; set; }
        public ColorPair SubitemColors { get; set; }

        protected bool _visible = true;

        /// <summary>
        /// Is the output from this thread visible?
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;

                    // Track the number of filtered threads.
                    if (_visible)
                    {
                        ThreadNames.DecrementInvisibleCount();
                    }
                    else
                    {
                        ThreadNames.IncrementInvisibleCount();
                    }
                }
            }
        }
    }
}
