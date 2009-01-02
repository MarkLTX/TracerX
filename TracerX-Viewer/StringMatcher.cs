using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TracerX {
    // Public so it can be serialized/deserialized (XML)
    public enum MatchType { Simple, Wildcard, RegularExpression };

    // This immutable class helps with finding text (i.e. the Find dialog), 
    // text filtering, and matching ColoringRules.
    internal class StringMatcher {
        public StringMatcher(string needle, bool matchCase, MatchType compareType) {
            RegexOptions regexOptions;

            Needle = needle;

            if (matchCase) {
                regexOptions = RegexOptions.Compiled;
            } else {
                regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;
            }

            switch (compareType) {
                case MatchType.Simple:
                    sc = matchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
                    break;
                case MatchType.Wildcard:
                    regex = new Regex(WildcardToRegex(needle), regexOptions);
                    break;
                case MatchType.RegularExpression:
                    regex = new Regex(needle, regexOptions);
                    break;
            }
        }

        public bool Matches(string candidate) {
            if (regex == null) {
                return (candidate.IndexOf(Needle, sc) != -1);
            } else {
                return regex.IsMatch(candidate);
            }
        }

        public readonly string Needle;
        public readonly StringComparison sc = StringComparison.CurrentCultureIgnoreCase;
        public readonly Regex regex;

        // Convert a string with wildcards to a regular expression string.
        // The '\' char always escapes the next char and is required to
        // search for '\', '*', or '?'.
        private static string WildcardToRegex(string wildcard) {
            StringBuilder sb = new StringBuilder(wildcard.Length + 8);

            //sb.Append("^");

            for (int i = 0; i < wildcard.Length; i++) {
                char c = wildcard[i];
                switch (c) {
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

            //sb.Append("$");

            return sb.ToString();
        }

    }
}
