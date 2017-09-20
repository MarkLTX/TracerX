using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TracerX
{
    // ColorDriver identifies which type of rule/object drives the full-row coloring.
    internal enum ColorDriver { Custom, TraceLevels, Loggers, ThreadNames, ThreadIDs, Methods, Sessions };

    internal static class ColorUtil
    {
        static ColorUtil()
        {
            TraceLevelPalette[TraceLevel.Fatal] = new ColorPair(Color.Red, Color.White);
            TraceLevelPalette[TraceLevel.Error] = new ColorPair(Color.Red,Color.AntiqueWhite);
            TraceLevelPalette[TraceLevel.Warn] = new ColorPair(Color.Orange);
            TraceLevelPalette[TraceLevel.Info] = new ColorPair(Color.LightBlue);
            TraceLevelPalette[TraceLevel.Debug] = new ColorPair(Color.LightGreen);
            TraceLevelPalette[TraceLevel.Verbose] = new ColorPair(Color.Gainsboro);

            // List of ARGB colors generated with the Palette form.
            int[] rgb = new int[] { -16711936, -129, -65281, -8421505, -8388609, -8388737, -32897, /*-16711809,*/ -16711681, -8421377, -8388864, -65409, -33024, -32769, -256, -16744449, -65536};
            Palette = new ColorPair[rgb.Length];
            for (int i = 0; i<rgb.Length; ++i) {
                Palette[i] = new ColorPair(Color.FromArgb(rgb[i]));
            }
        }

        // The RowColorDriver determines which color rules are applied (e.g. the ones
        // for Loggers vs. the ones for Threads) to whole rows in the main ListView.
        // The ColorRulesDialog has one tab per ColorDriver.
        public static ColorDriver RowColorDriver
        {
            get
            {
                try
                {
                    return (ColorDriver)Enum.Parse(typeof(ColorDriver), Properties.Settings.Default.RowColorDriver);
                }
                catch
                {
                    return ColorDriver.Custom;
                }
            }

            set { Properties.Settings.Default.RowColorDriver = value.ToString(); }
        }

        // Predefined colors used for everything except TraceLevels and custom rules.
        public static readonly ColorPair[] Palette;

        // Predefined colors used for TraceLevels (e.g. red for Error).
        public static readonly Dictionary<TraceLevel, ColorPair> TraceLevelPalette = new Dictionary<TraceLevel, ColorPair>();

        // List of colors currently used to color subitems (cells) in the main ListView.
        public static readonly HashSet<ColorPair> UsedSubitemColors = new HashSet<ColorPair>();

        // Returns the first unused color in the Pallete.
        public static ColorPair FirstAvailableColor()
        {
            return GetUnusedColors(RowColorDriver).FirstOrDefault();
        }

        private static bool _recursing;
        
        // Returns all unused colors in the Palette.
        public static IEnumerable<ColorPair> GetUnusedColors(ColorDriver driver)
        {
            var result = Palette.Except(UsedSubitemColors);

            if (driver != ColorDriver.Custom)
            {
                HashSet<ColorPair> usedRowColors = new HashSet<ColorPair>();

                foreach (IFilterable item in GetAllDriverItems(driver))
                {
                    if (item.RowColors != null)
                    {
                        usedRowColors.Add(item.RowColors);
                    }
                }

                result = result.Except(usedRowColors);
            }

            if (!result.Any() && !_recursing)
            {
                // Refresh UsedSubitemColors to make sure it's accurate, then try again with a recursive call.
                // Use _recursing to prevent the recursion from going more than one level deep.

                _recursing = true;
                RefreshUsedSubItemColors();
                result = GetUnusedColors(driver);
                _recursing = false;
            }

            return result;
        }

        // Removes SubItem colors (also called column colors) from all objects.
        public static void ClearSubItemColors()
        {
            UsedSubitemColors.Clear();

            TraceLevelObjects.RemoveSubitemColors();
            LoggerObjects.RemoveSubitemColors(); 
            MethodObjects.RemoveSubitemColors();
            SessionObjects.RemoveSubitemColors();
            ThreadObjects.RemoveSubitemColors();
            ThreadNames.RemoveSubitemColors();
        }

        private static void RefreshUsedSubItemColors()
        {
            UsedSubitemColors.Clear();

            LoggerObjects.TrackSubitemColors(UsedSubitemColors);
            MethodObjects.TrackSubitemColors(UsedSubitemColors);
            SessionObjects.TrackSubitemColors(UsedSubitemColors);
            ThreadObjects.TrackSubitemColors(UsedSubitemColors);
            ThreadNames.TrackSubitemColors(UsedSubitemColors);

            UsedSubitemColors.Remove(null);
        }

        private static IEnumerable<IFilterable> GetAllDriverItems(ColorDriver driver) 
        {
            // TODO: These calls aren't thread-safe!

            switch (driver)
            {
                case ColorDriver.Loggers:
                    return LoggerObjects.AllLoggers.Cast<IFilterable>();
                    break;
                case ColorDriver.Methods:
                    return MethodObjects.AllMethods.Cast<IFilterable>();
                    break;
                case ColorDriver.Sessions:
                    return SessionObjects.AllSessionObjects.Cast<IFilterable>();
                    break;
                case ColorDriver.ThreadIDs:
                    return ThreadObjects.AllThreadObjects.Cast<IFilterable>();
                    break;
                case ColorDriver.ThreadNames:
                    return ThreadNames.AllThreadNames.Cast<IFilterable>();
                    break;
                case ColorDriver.TraceLevels:
                    return TraceLevelObjects.AllTraceLevels.Values.Cast<IFilterable>();
                    break;
                default:
                    return null;
                    break;
            }
        }
    }
}
