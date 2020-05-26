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

        /// <summary>
        /// Returns the Nth (0 based) field of 'this' string, where fieldSeparator specifies
        /// the char that separates fields.  Does not use string.Split().
        /// The 'this' string must not be null, but can be empty.
        /// This overload treats sequential instances of the fieldSeparator char as a single
        /// instances (i.e. it logically collapses them).
        /// All input strings, including empty ones, are considered to have at least one field.
        /// The total number of fields in a string is 1 plus the count of (collapsed) fieldSeparator
        /// chars in the string.  
        /// If fieldNum is greater than or equal to the number of
        /// fields, a null string is returned.  
        /// See the TestField() method for examples.
        /// </summary>
        /// <param name="s">The 'this' string.  Must not be null.</param>
        /// <param name="fieldNum">The 0-based number of the field (substring) to return.</param>
        /// <param name="fieldSeparator">The character that separates the input into fields.</param>
        /// <returns></returns>
        public static string Field(this string s, int fieldNum, char fieldSeparator)
        {
            return s.Field(fieldNum, fieldSeparator, true);
        }

        /// <summary>
        /// Returns the Nth (0 based) field of 'this' string, where fieldSeparator specifies
        /// the char that separates fields.  Does not use string.Split().
        /// The 'this' string must not be null, but can be empty.
        /// All input strings, including empty ones, are considered to have at least one field.
        /// The total number of fields in a string is 1 plus the count of fieldSeparator
        /// chars in the string (unless collapseSeparators is true).  
        /// If fieldNum is greater than or equal to the number of
        /// fields, a null string is returned.  
        /// See the TestField() method for examples.
        /// </summary>
        /// <param name="s">The 'this' string.  Must not be null.</param>
        /// <param name="fieldNum">The 0-based number of the field (substring) to return.</param>
        /// <param name="fieldSeparator">The character that separates the input into fields.</param>
        /// <param name="collapseSeparators">If true, sequences of multiple fieldSeparator chars are treated as a single instance.
        /// If false, adjacent fieldSeparator chars are considered to have empty fields between them.</param>
        /// <returns></returns>
        public static string Field(this string s, int fieldNum, char fieldSeparator, bool collapseSeparators)
        {
            if (s == null) throw new ArgumentNullException("Can't extract a field from a null string.");
            if (fieldNum < 0) throw new ArgumentException("Parameter fieldNum cannot be negative.");

            int start = 0;
            int length = 0;
            int count = 0;

            for (int i = 0; i < s.Length; ++i)
            {
                if (count == fieldNum)
                {
                    // We already found the start of the desired field, now looking for the end.
                    if (s[i] == fieldSeparator)
                    {
                        return s.Substring(start, length);
                    }
                    else
                    {
                        ++length;
                    }
                }
                else if (s[i] == fieldSeparator)
                {
                    ++count;
                    start = i + 1;

                    if (collapseSeparators)
                    {
                        while (start < s.Length && s[start] == fieldSeparator)
                        {
                            ++i;
                            ++start;
                        }
                    }
                }
            }

            if (count != fieldNum)
            {
                return null;
            }
            else if (length == 0 || start == s.Length)
            {
                return "";
            }
            else
            {
                return s.Substring(start, length);
            }
        }

        /// <summary>
        /// Returns the number of "fields" in the input string, where fieldSeparator specifies
        /// the char that delimits fields.  Null strings have 0 fields.  Empty strings have 1
        /// field.  For other strings, this returns the number of (possibly empty) fields delimited by one more fieldSeparator chars.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="fieldSeparator">The char that separates the input string into fields.</param>
        /// <returns></returns>
        public static int FieldCount(this string s, char fieldSeparator)
        {
            return s.FieldCount(fieldSeparator, true);
        }

        /// <summary>
        /// Returns the number of "fields" in the input string, where fieldSeparator specifies
        /// the char that separates fields.  Null strings have 0 fields.  Empty strings have 1
        /// field.  For other strings, this returns the number of fieldSeparator chars found plus 1,
        /// unless collapseSeparators is true.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="fieldSeparator">The char that separates the input string into fields.</param>
        /// <param name="collapseSeparators">If true, multiple adjacent instances of the fieldSeparator char are treated as one instance.
        /// If false, each instance increments the count.</param>
        /// <returns></returns>
        public static int FieldCount(this string s, char fieldSeparator, bool collapseSeparators)
        {
            if (s == null) return 0;

            // Init prevChar to something other than fieldSeparator;
            char prevChar = (char)~fieldSeparator;
            int result = 0;

            foreach (char c in s)
            {
                if (c == fieldSeparator && (!collapseSeparators || prevChar != fieldSeparator))
                {
                    ++result;
                }

                prevChar = c;
            }

            return result + 1;
        }
        
        /// <summary>
        /// Returns the first numChars chars of the string.
        /// Same as Remove(), but no exception if Length is less than numChars.
        /// </summary>
        public static string First(this string s, int numChars)
        {
            if (s.Length > numChars)
            {
                return s.Remove(numChars);
            }
            else
            {
                return s;
            }
        }

    }
}
