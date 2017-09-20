using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using TracerX.Forms;

namespace TracerX
{
    internal static class MethodObjects
    {
        // Lock this when accessing the AllMethods collection.
        public static object Lock = new object();

        // List of all created MethodObjects.
        public static List<MethodObject> AllMethods = new List<MethodObject>();

        public static void Clear()
        {
            lock (Lock)
            {
                AllMethods = new List<MethodObject>();

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
                foreach (MethodObject mo in AllMethods)
                {
                    mo.SubitemColors = null;
                }
            }
        }

        public static void TrackSubitemColors(HashSet<ColorPair> usedColors)
        {
            lock (Lock)
            {
                foreach (MethodObject mo in AllMethods)
                {
                    usedColors.Add(mo.SubitemColors);
                }
            }
        }

        public static void Add(MethodObject mo)
        {
            lock (Lock)
            {
                AllMethods.Add(mo);

                if (!mo.Visible)
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
        /// Are all methods visible?
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

        //// Count the visible methods in AllMethods, set the internal count (invisibleCount) accordingly, 
        //// and raise the AllVisibleChanged event if the new count differs from the current count.
        //public static void RecountMethods()
        //{
        //    lock (Lock)
        //    {
        //        int oldCount = _invisibleCount;
        //        _invisibleCount = 0;

        //        foreach (MethodObject m in AllMethods)
        //        {
        //            if (!m.Visible) ++_invisibleCount;
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

        public static void ShowAllMethods()
        {
            lock (Lock)
            {
                foreach (MethodObject l in AllMethods)
                {
                    l.Visible = true;
                }
            }
        }

        public static void HideAllMethods()
        {
            lock (Lock)
            {
                foreach (MethodObject l in AllMethods)
                {
                    l.Visible = false;
                }
            }
        }
    }

    class MethodObject : IFilterable
    {
        public string Name { get; set; }

        public ColorPair RowColors { get; set; }
        public ColorPair SubitemColors { get; set; }

        protected bool _visible = true;

        /// <summary>
        /// Is the output from this method visible?
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;

                    // Track the number of filtered methods.
                    if (_visible)
                    {
                        MethodObjects.DecrementInvisibleCount();
                    }
                    else
                    {
                        MethodObjects.IncrementInvisibleCount();
                    }
                }
            }
        }
    }
}
