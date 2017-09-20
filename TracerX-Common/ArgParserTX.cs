using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Deployment.Application;
using System.Diagnostics;
using TracerX.ExtensionMethods;

namespace TracerX
{
    /// <summary>
    /// Class for getting, splitting, and parsing command line arguments.  
    /// Supports GUI apps, console apps, ClickOnce apps and non-ClickOnce apps.
    /// </summary>
    public static class ArgParserTX
    {
        private static readonly Logger Log = Logger.GetLogger("TX1.ArgParser");

        /// <summary>
        /// This splits the app's arguments into a string[].  Works for "regular" apps and ClickOnce apps.
        /// It handles a regular argument string passed to a regular exe (result does not include the exe path),
        /// a query string passed to a ClickOnce app's URI, or a query string passed to a ClickOnce app's .appref-ms file.
        /// </summary>
        public static string[] GetArgs(bool keepQuotes = false)
        {
            using (Log.InfoCall())
            {
                string[] args = null;

                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    // This means we were called via the ClickOnce shortcut or URL.

                    Log.Info("ApplicationDeployment.IsNetworkDeployed = true.");
                    args = GetClickOnceArgs(keepQuotes);
                }
                else
                {
                    Log.Info("ApplicationDeployment.IsNetworkDeployed = false.");
                    Log.Info("Command line: ", Environment.CommandLine);
                    args = ArgParserTX.SplitCommandLine(Environment.CommandLine, keepQuotes).Skip(1).ToArray();
                }

                return args ?? new string[0];
            }
        }

        /// <summary>
        /// Given a switch argument (must begin with '-' or '/') returns true if the argument's syntax is valid.
        /// Valid syntax is something like "-switchPart=valuePart" or just "-switchPart". The switch may begin
        /// with '-' or '/'.  The switchPart must be a "word" (in Regex terms) or a '?' char (usually means "help").
        /// The separator between switchPart and valuePart may be ':' or '='.  The valuePart
        /// may contain any character, but must be in quotes if it contains blanks.  If the syntax is valid, switchPart and valuePart
        /// are returned.  Otherwise, errorMsg is returned.  
        /// Examples of invalid syntax are "HasNoPrefix", "-", "--TwoDashes", "-MissingValue=".
        /// </summary>
        /// <param name="switchArg">Typically one of the elements of the string[] elements returned by SplitCommandLine.</param>
        /// <param name="switchPart">Returns the switch part of the argument.</param>
        /// <param name="valuePart">Returns the value part of the argument or empty if no value present.</param>
        /// <param name="errorMsg">Returns an error message if the syntax of switchArg is not valid.</param>
        /// <returns>True if switchArg is parsed successfully, false if not.</returns>
        public static bool ParseSwitch(string switchArg, out string switchPart, out string valuePart, out string errorMsg)
        {
            // A 'switch' has the pattern <prefix><switch>[<separator><value>] where
            // <prefix> is '-' or '/',
            // <switch> is any word matching the "/w" class for Regex or a '?',
            // <separator> is ':' or '=', and
            // <value> is anything after the separator.
            // The [<separator><value>] part is optional.
            // An argument that doesn't begin with a prefix is a non-switch (for lack of a better term).
            // For example, the source and destination filename arguments of the DOS COPY
            // command are non-switches.

            Regex swtch = new Regex(@"^[-/](?<switch>\w+|\?)([:=](?<value>.+))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            bool result;

            switchPart = null;
            valuePart = null;
            errorMsg = null;

            if (switchArg.StartsWith("-") || switchArg.StartsWith("/"))
            {
                var match = swtch.Match(switchArg);

                if (match.Success)
                {
                    result = true;
                    switchPart = match.Groups["switch"].Value.ToLower();
                    valuePart = match.Groups["value"].Value;
                }
                else
                {
                    result = false;
                    errorMsg = "Invalid switch syntax: '{0}'".Fmt(switchArg);
                }
            }
            else
            {
                result = false;
                errorMsg = "Argument '{0}' is not a switch (does not begin with '-' or '/')".Fmt(switchArg);
            }

            return result;
        }

        /// <summary>
        /// Splits a query string into a string[].  The split is done on '&' chars.
        /// The parts are not percent-decoded.
        /// </summary>
        private static string[] SplitQueryString(string qs, bool keepQuotes)
        {
            // This must not return null due to logic in GetArgs().

            if (qs.NullOrWhiteSpace())
            {
                return new string[0];
            }
            else
            {
                // It can't be empty.
                // Remove leading '?' if present.

                if (qs[0] == '?') qs = qs.Remove(0, 1);

                string argString = qs.Replace('&', ' ');
                Log.Debug("Query string converted to: ", argString);
                return SplitCommandLine(argString, keepQuotes);
            }
        }

        /// <summary>
        /// Applies "percent encoding" to a normal string. See http://en.wikipedia.org/wiki/Percent-encoding
        /// </summary>
        public static string PercentEncode(string normal)
        {
            string result = "";
            var odds = "-_.~".ToCharArray(); // Odd unreserved chars.

            if (normal != null)
            {
                // Replace each char that isn't unreserved with %nn.

                StringBuilder str = new StringBuilder(normal);

                for (int j = str.Length - 1; j >= 0; --j)
                {
                    char c = str[j];

                    if ((c >= 'a' && c <= 'z') ||
                        (c >= 'A' && c <= 'Z') ||
                        (c >= '0' && c <= '9') ||
                        odds.Contains(c))
                    {
                        // Leave it alone
                    }
                    else
                    {
                        string hex = "%" + Convert.ToString(c, 16).PadLeft(2, '0');
                        str.Remove(j, 1);
                        str.Insert(j, hex);
                    }
                }

                result = str.ToString();
            }

            return result;
        }

        /// <summary>
        /// Converts a percent-encoded string back to a normal string.
        /// </summary>
        public static string PercentDecode(string qs)
        {
            string result = "";

            if (!qs.NullOrWhiteSpace())
            {
                StringBuilder sb = new StringBuilder(qs);

                sb.Replace('&', ' ');

                int pctPos = qs.LastIndexOf('%');

                while (pctPos != -1)
                {
                    string hex = qs.Substring(pctPos + 1, 2);
                    int num = Convert.ToInt32(hex, 16);
                    string chr = char.ConvertFromUtf32(num);

                    sb.Remove(pctPos, 3);
                    sb.Insert(pctPos, chr);

                    if (pctPos > 0)
                    {
                        pctPos = qs.LastIndexOf('%', pctPos - 1);
                    }
                    else
                    {
                        break;
                    }
                }

                result = sb.ToString();
            }

            return result;
        }

        // The SplitCommandLine() and RemoveMatchingQuotes() methods
        // were taken from http://jake.ginnivan.net/c-sharp-argument-parser

        /// <summary>
        /// Splits the argument string of a non-ClickOnce app.
        /// Works better than the args parameter of main(string[] args) because
        /// in the args parameter, escaped quotes (e.g. a path like "c:\folder\")
        /// will consume all the following command line arguments as the one argument. 
        /// This function ignores escaped quotes making handling paths much easier.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        /// <param name="keepQuotes">Specifies whether to retain quotes around quoted args.</param>
        /// <returns></returns>
        private static string[] SplitCommandLine(string commandLine, bool keepQuotes = false)
        {
            var translatedArguments = new StringBuilder(commandLine);
            var quoted = false;
            for (var i = 0; i < translatedArguments.Length; i++)
            {
                if (translatedArguments[i] == '"')
                {
                    quoted = !quoted;
                }

                // Replace unquoted blanks with newlines, then split the string at the newlines.

                if (translatedArguments[i] == ' ' && !quoted)
                {
                    translatedArguments[i] = '\n';
                }
            }

            var toReturn = translatedArguments.ToString().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (!keepQuotes)
            {
                for (var i = 0; i < toReturn.Length; i++)
                {
                    toReturn[i] = RemoveMatchingQuotes(toReturn[i]);
                }
            }

            return toReturn;
        }

        private static string[] GetClickOnceArgs(bool keepQuotes)
        {
            string[] args = null;
            ApplicationDeployment currentDeployment = ApplicationDeployment.CurrentDeployment;

            Log.Info("CurrentDeployment.CurrentVersion = ", currentDeployment.CurrentVersion);
            Log.Info("CurrentDeployment.IsFirstRun = ", currentDeployment.IsFirstRun);

            try
            {
                // Try to get the query string.

                if (currentDeployment.ActivationUri == null)
                {
                    Log.Info("CurrentDeployment.ActivationUri is null!");
                }
                else
                {
                    var querystring = currentDeployment.ActivationUri.Query;
                 
                    Log.Info("querystring = ", querystring);

                    if (querystring != null)
                    {
                        args = SplitQueryString(querystring, keepQuotes);
                    }
                }
            }
            catch (Exception ex)
            {
                // Usually the URI has an invalid format because the 'user' didn't pass a URI.
                Log.Warn("Unable to get the query string. ", ex);
            }

            if (args == null)
            {
                // Failed to get the query string, so try this.

                if (AppDomain.CurrentDomain.SetupInformation == null) Log.Warn("AppDomain.CurrentDomain.SetupInformation = null!");
                else if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments == null) Log.Warn("AppDomain.CurrentDomain.SetupInformation.ActivationArguments = null!");
                else if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData == null) Log.Warn("AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData = null!");
                else
                {
                    string[] inputArgs = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;

                    Log.Info("ActivationData has ",  inputArgs.Length, " elements...");

                    if (inputArgs.Length > 0)
                    {
                        for (int i = 0; i < inputArgs.Length; ++ i )
                        {
                            Log.InfoFormat("ActivationData[{0}] = {1}", i, inputArgs[i]);
                        }

                        // In my experience, inputArgs.Length is always 1 and inputArgs[0] contains everything we're ever going to get.
                        // The way to pass multiple args this way is to percent-encode the arg string to eliminate blanks.  Otherwise,
                        // we only get the first blank-delimited substring.

                        args = SplitQueryString(inputArgs[0], keepQuotes);
                    }
                }
            }

            return args;
        }

        private static string RemoveMatchingQuotes(string stringToTrim)
        {
            var firstQuoteIndex = stringToTrim.IndexOf('"');
            var lastQuoteIndex = stringToTrim.LastIndexOf('"');
            while (firstQuoteIndex != lastQuoteIndex)
            {
                stringToTrim = stringToTrim.Remove(firstQuoteIndex, 1);
                stringToTrim = stringToTrim.Remove(lastQuoteIndex - 1, 1); //-1 because we've shifted the indicies left by one
                firstQuoteIndex = stringToTrim.IndexOf('"');
                lastQuoteIndex = stringToTrim.LastIndexOf('"');
            }

            return stringToTrim;
        }
    }
}

