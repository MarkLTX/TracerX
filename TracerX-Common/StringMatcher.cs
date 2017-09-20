using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TracerX
{
    /// <summary>
    /// Used when constructing a StringMatcher to specify kind of match to be performed.
    /// </summary>
    public enum MatchType { Simple, Wildcard, RegularExpression };


    /// <summary>
    /// This immutable class is used to determine whether candidate strings
    /// match a given regular expression, wildcard, or simple string.
    /// </summary>
    public class StringMatcher
    {
        /// <summary>
        /// This constructor accepts the regular expression, wildcard, or simple string
        /// that candidate strings (passed to Matches()) are compared to.
        /// </summary>
        /// <param name="searchFor">Text to search for in a candidate string.  When using wildcards, the '\' char always escapes the next char and is required to search for '\', '*', or '?'.</param>
        /// <param name="matchCase">True for case-sensitive, false for case-insensitive.</param>
        /// <param name="compareType">Wildcard, RegularExpression, or Simple text search.</param>
        /// <param name="matchWholeString">True if candidate strings must completely match searchFor, false if they need only contain searchFor to match.  Does not apply to regular expressions because this option is specified within the searchFor string.</param>
        public StringMatcher(string searchFor, bool matchCase, MatchType compareType, bool matchWholeString = false)
        {
            OriginalText = searchFor;
            _stringComparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            _wholeString = matchWholeString;

            switch (compareType)
            {
                case MatchType.Simple:
                    _simpleString = searchFor;
                    break;
                case MatchType.Wildcard:
                    if (searchFor.IndexOfAny(_wildChars) == -1)
                    {
                        // Since no wildcard chars ('*' and '?') are specified we can use "simple" string matching for better efficiency.
                        // However, '\' chars are escape chars for wildcards and must be handled.  We can't just remove every '\'
                        // because double '\'s must be collapsed to single '\'s (the first one escapes the second one).  Start by 
                        // temporarily converting each double '\' to a placeholder char we know isn't already present (i.e. '*').

                        _simpleString = searchFor.Replace(@"\\", "*"); // Replace double '\' with placeholder.
                        _simpleString = _simpleString.Replace("\\", ""); // Remove remaining single '\'s that escape the next char.
                        _simpleString = _simpleString.Replace('*', '\\'); // Insert single '\'s where double '\'s existed.
                    }
                    else
                    {
                        // Convert the wildcard string to a Regex.
                        _regex = new Regex(WildcardToRegex(searchFor, matchWholeString), Options(matchCase));
                    }
                    break;
                case MatchType.RegularExpression:
                    _regex = new Regex(searchFor, Options(matchCase));
                    break;
            }
        }
        /// <summary>
        ///  Gets the text to search for, originally passed to the constructor.
        /// </summary>
        public string OriginalText { get; private set; }

        public bool MatchCase
        {
            get { return _stringComparison == StringComparison.Ordinal; }
        }

        private bool _wholeString;
        private StringComparison _stringComparison;
        private Regex _regex;
        private string _simpleString;
        private static readonly char[] _wildChars = new char[] { '*', '?' };

        /// <summary>
        ///  Returns true if the candidate string matches or contains the original searchFor string 
        ///  passed to the constructor based on the other options specified in the constructor.
        /// </summary>
        public bool Matches(string candidate)
        {
            if (_regex == null)
            {
                if (_wholeString)
                {
                    return string.Equals(_simpleString, candidate, _stringComparison);
                }
                else
                {
                    return (candidate.IndexOf(_simpleString, _stringComparison) != -1);
                }
            }
            else
            {
                return _regex.IsMatch(candidate);
            }
        }

        private RegexOptions Options(bool matchCase)
        {
            if (matchCase)
            {
                return RegexOptions.Compiled;
            }
            else
            {
                return RegexOptions.Compiled | RegexOptions.IgnoreCase;
            }
        }

        // Convert a string with wildcards to a regular expression string.
        // The '\' char always escapes the next char and is required to
        // search for '\', '*', or '?'.
        private static string WildcardToRegex(string wildcard, bool wholeString)
        {
            StringBuilder sb = new StringBuilder(wildcard.Length + 8);

            if (wholeString) sb.Append("^");

            for (int i = 0; i < wildcard.Length; i++)
            {
                char c = wildcard[i];
                switch (c)
                {
                    case '*':
                        sb.Append(".*");
                        break;
                    case '?':
                        sb.Append(".");
                        break;
                    case '\\':
                        if (i < wildcard.Length - 1)
                            sb.Append(Regex.Escape(wildcard[++i].ToString()));
                        break;
                    default:
                        sb.Append(Regex.Escape(c.ToString()));
                        break;
                }
            }

            if (wholeString) sb.Append("$");

            return sb.ToString();
        }
    }
}
