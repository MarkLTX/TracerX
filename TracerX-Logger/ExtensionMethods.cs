using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TracerX.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static string Fmt(this string s, params object[] args)
        {
            return string.Format(s, args);
        }

        public static bool NullOrWhiteSpace(this string s)
        {
            // To avoid requiring .NET 4.0, we do our own check for whitespace.

            if (string.IsNullOrEmpty(s))
            {
                return true;
            }
            else
            {
                for (int i = 0; i < s.Length; ++i)
                {
                    if (!char.IsWhiteSpace(s, i))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public static bool NullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Appends a path to this string using Path.Combine().
        /// </summary>
        public static string AddPath(this string s, string path)
        {
            // If the second string passed to Path.Combine() starts with '\', 
            // the result is equal to the second string!
            // Therefore, remove \ and / from the second string.
            return System.IO.Path.Combine(s, path.TrimStart('\\', '/'));
        }


        /// <summary>
        /// Returns the directory portion of a path using Path.GetDirectoryName().
        /// </summary>
        public static string PathDir(this string s)
        {
            return System.IO.Path.GetDirectoryName(s);
        }

        ///// <summary>
        ///// Returns the directory portion of a path using Path.GetDirectoryName(). If 'levels' is
        ///// less than zero, that many levels are removed from the end of the path. If levels is
        ///// greater than zero, that many levels are kept from the root down. If levels is zero, the
        ///// input path is returned unchanged.
        ///// </summary>
        //public static string PathDir(this string s, int levels)
        //{
        //    if (levels <= 0)
        //    {
        //        while (levels < 0)
        //        {
        //            s = System.IO.Path.GetDirectoryName(s);
        //            ++levels;
        //        }

        //        return s;
        //    }
        //    else // Keep first N levels...
        //    {
        //        List<string> parts = new List<string>();

        //        while (!s.NullOrEmpty())
        //        {
        //            parts.Add(s);
        //            s = System.IO.Path.GetDirectoryName(s);
        //        }

        //        if (parts.Count >= levels)
        //        {
        //            return parts[parts.Count - levels];
        //        }
        //        else
        //        {
        //            throw new InvalidOperationException("The specified path doesn't have the number of levels requested.");
        //        }
        //    }
        //}

        /// <summary>
        /// Changes the directory portion of a path to the specified directory,
        /// leaving the leaf portion intact.
        /// </summary>
        public static string ChangePathDir(this string s, string newDir)
        {
            return System.IO.Path.Combine(newDir, System.IO.Path.GetFileName(s));
        }

        /// <summary>
        /// Returns the leaf portion of a path using Path.GetFileName().
        /// </summary>
        public static string PathLeaf(this string s)
        {
            return System.IO.Path.GetFileName(s);
        }

    }
}
