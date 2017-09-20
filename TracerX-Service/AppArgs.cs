using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using TracerX.ExtensionMethods;
using TracerX.Properties;

namespace TracerX
{
    // This class is responsible for parsing the command line 
    // and storing the results in its fields/properties.
    internal static class AppArgs
    {
        // Note that _settings contains properties read from the .config file.
        public static Settings _settings = Settings.Default;

        public static bool IsHelpWanted;  // From the -?, -h, or -help switch.
        public static bool IsInstall;          // From the -i or -install switch.
        public static bool IsUninstall;       // From the -u or -uninstall switch.
        public static bool IsConsoleApp;   // From the -c or -console switch.
        public static bool IsImpersonate; // From the -impersonate switch, or else the .config file.
        public static int Port;                 // From the -p or -port switch, or else the .config file.
        public static int RetryInterval;     // From the -r or -retry switch, or else the .config file.

        // These private nullables are used to track whether a given switch occurs more than once.
        private static bool? _isInstall;         
        private static bool? _isUninstall;      
        private static bool? _isConsoleApp;  
        private static bool? _isImpersonate; 
        private static bool? _isHelpWanted;  
        private static int? _port;
        private static int? _retryInterval;

        public static string ErrorMessage;

        private static Logger Log = Logger.GetLogger("TX1.Service.Args");
        private enum ValReq
        {
            Disallowed, // Indicates the switch doesn't allow a value.
            Allowed,    // Indicates the switch allows an optional value.
            Required    // Indicates the switch required a value.
        };

        // Writes a message to the console about the syntax of this program's command line parameters, 
        // preceded by an optional introduction message (intro).
        public static void ShowHelp(string intro = null)
        {
            Log.Warn("Showing help message.");
            StringBuilder sb = new StringBuilder();

            if (!intro.NullOrWhiteSpace()) sb.AppendLine(intro);

            sb.AppendLine();
            sb.AppendLine(Program.ExeName + " may be run as a Windows service");
            sb.AppendLine("or a console app.  The following switches are allowed.");
            sb.AppendLine();
            sb.AppendLine("  -h | -help | -?");
            sb.AppendLine("     Any of these switches causes this help message to be");
            sb.AppendLine("     displayed and all other arguments to be ignored.");
            sb.AppendLine();
            sb.AppendLine("  -console");
            sb.AppendLine("     Runs as a console app instead of a Windows service.");
            sb.AppendLine("     The app will run and accept connections and requests");
            sb.AppendLine("     from WCF clients (e.g. TracerX-Viewer) until any key");
            sb.AppendLine("     is pressed.  Can be used with -port, -retry.");
            sb.AppendLine();
            sb.AppendLine("  -install");
            sb.AppendLine("     Installs the .exe as a Windows service configured to");
            sb.AppendLine("     run on the Local System account and terminates.");
            sb.AppendLine("     Administrative privileges are required.  Check the");
            sb.AppendLine("     output from this command to determine if it succeeded.");
            sb.AppendLine("     Not compatible with any other switch.");
            sb.AppendLine();
            sb.AppendLine("  -uninstall");
            sb.AppendLine("     Removes the Windows service and terminates.");
            sb.AppendLine("     Sufficient privileges are required.  Monitor the output");
            sb.AppendLine("     from this command to determine if it succeeded.");
            sb.AppendLine("     Not compatible with any other switch.");
            sb.AppendLine();
            sb.AppendLine("  -impersonate");
            sb.AppendLine("     Causes WCF operations that open/read log files to");
            sb.AppendLine("     impersonate the client for security purposes.  Otherwise,");
            sb.AppendLine("     files are accessed using the service's logon account.");
            sb.AppendLine();
            sb.AppendLine("  -port:<PortNumber>");
            sb.AppendLine("     Specifies the port number to listen on");
            sb.AppendLine("     for incoming TCP connections.  Overrides");
            sb.AppendLine("     the Port setting in the .config file. ");
            sb.AppendLine("     Can only be used with -console.");
            sb.AppendLine();
            sb.AppendLine("  -retry:<RetryInterval>");
            sb.AppendLine("     Specifies the interval in minutes between retries");
            sb.AppendLine("     of starting the WCF service, which can fail if");
            sb.AppendLine("     another process is using the port.  Overrides the");
            sb.AppendLine("     RetryIntervalMinutes setting in the .config file.");
            sb.AppendLine("     Specify 0 to disable retrying after the first try.");
            sb.AppendLine("     Can only be used with -console.");
            sb.AppendLine();
            sb.AppendLine("  Switch arguments may start with '-' or '/'.");
            sb.AppendLine("  The separator between a switch and its value");
            sb.AppendLine("  (if any) may be ':' or '='.");

            Console.WriteLine(sb);
        }

        // Parses the command line and sets public fields/properties including IsHelpWanted and ErrorMessage.
        // If displayMsg is true, a message will be written to the console if an error is found or the -help switch is found.  
        // Otherwise, the caller can display any error found and/or call ShowHelp().
        // Returns true if no error was found, false if the ErrorMessage property was set.
        public static bool ParseCommandLine()
        {
            using (Log.InfoCall())
            {
                ParseArgs();

                IsHelpWanted = _isHelpWanted.GetValueOrDefault();

                if (ErrorMessage == null)
                {
                    // Set defaults for unspecified switches.

                    IsUninstall = _isUninstall.GetValueOrDefault();
                    IsInstall = _isInstall.GetValueOrDefault();
                    IsConsoleApp = _isConsoleApp.GetValueOrDefault();
                    IsImpersonate = _isImpersonate.GetValueOrDefault(_settings.ImpersonateClients);
                    Port = _port.GetValueOrDefault(_settings.Port);
                    RetryInterval = _retryInterval.GetValueOrDefault(_settings.PortRetryIntervalMinutes);

                    Log.Info("AppArgs.IsHelpWanted = ", IsHelpWanted);
                    Log.Info("AppArgs.Port = ", Port);
                    Log.Info("AppArgs.RetryInterval = ", RetryInterval);
                    Log.Info("AppArgs.IsImpersonate = ", IsImpersonate);
                    Log.Info("AppArgs.IsConsoleApp = ", IsConsoleApp);
                    Log.Info("AppArgs.IsInstall = ", IsInstall);
                    Log.Info("AppArgs.IsUninstall = ", IsUninstall);

                    // Check for incompatible switches and out-of-range values.

                    if (IsUninstall && (IsInstall || IsConsoleApp || _port.HasValue || _retryInterval.HasValue))
                    {
                        SetError("Incompatible switches and/or config file settings were specified.");
                    }
                    else if (IsInstall && (IsConsoleApp || _port.HasValue || _retryInterval.HasValue))
                    {
                        SetError("Incompatible switches and/or config file settings were specified.");
                    }
                    else if (IsImpersonate && (IsUninstall || IsInstall))
                    {
                        SetError("Incompatible switches and/or config file settings were specified.");
                    }
                    else if (Port < 1 || Port > 65535)
                    {
                        SetError("The port number is not in the valid range (1-65535).");
                    }
                }

                if (ErrorMessage != null)
                {
                    Console.WriteLine("ERROR: {0}", ErrorMessage);

                    if (!IsHelpWanted)
                    {
                        Console.WriteLine("Use the \"-help\" switch to get help.");
                    }
                }

                if (IsHelpWanted)
                {
                    ShowHelp();
                }

                return ErrorMessage == null && !IsHelpWanted;
            } // using Log
        }

        // Sets properties from command line args.  May set ErrorMessage.
        private static void ParseArgs()
        {
            string[] args = ArgParserTX.GetArgs(keepQuotes: false);

            foreach (string arg in args)
            {
                Log.Debug("Got argument: ", arg);

                // Arguments that start with - or / are switches.
                // Others are "non-switches".

                char switchFlag = arg[0];

                if (switchFlag == '-' || switchFlag == '/')
                {
                    // The arg is a switch.
                    // Switches can be bare such as "-switchpart" or have values such as "-switchpart:valuepart".
                    // ParseSwitch() checks the syntax of the switch argument and sets switchPart
                    // and valuePart.  If the syntax is incorrect, ParseSwitch() returns false and sets
                    // msg to an error message.

                    string switchPart;
                    string valuePart;
                    string msg;

                    if (ArgParserTX.ParseSwitch(arg, out switchPart, out valuePart, out msg))
                    {
                        switch (switchPart.ToLower())
                        {
                            case "?":
                            case "h":
                            case "help":
                                SetBool(ref _isHelpWanted, switchPart, valuePart);
                                break;

                            case "i":
                            case "install":
                                SetBool(ref _isInstall, switchPart, valuePart);
                                break;

                            case "u":
                            case "uninstall":
                                SetBool(ref _isUninstall, switchPart, valuePart);
                                break;

                            case "c":
                            case "console":
                                SetBool(ref _isConsoleApp, switchPart, valuePart);
                                break;

                            case "impersonate":
                                SetBool(ref _isImpersonate, switchPart, valuePart);
                                break;

                            case "p":
                            case "port":
                                SetInt(ref _port, switchPart, valuePart);
                                break;

                            case "r":
                            case "retry":
                                SetInt(ref _retryInterval, switchPart, valuePart);
                                break;

                            default:
                                SetError("Unrecognized switch: {0}{1}".Fmt(switchFlag, switchPart));
                                break;

                        } // switch (switchPart)
                    } // Valid switch format
                    else
                    {
                        // Some fundamental error in the format of a "-switch:value" pair
                        // such as the presence of "-switch:" with no value.

                        SetError(msg);
                    }
                } // Found switch arg
                else
                {
                    // It's a non-switch.

                    SetError("The following argument is not a switch and is therefore invalid.\n{0}".Fmt(arg));
                }
            } // foreach arg
        }

        /// <summary>
        /// Sets the intOpt or sets an error message depending on the valuePart and whether the value is valid, etc.
        /// </summary>
        /// <param name="intOpt">Nullable int whose value is to be set.</param>
        /// <param name="switchPart">Switch string parsed from args.  Used in error message if any.</param>
        /// <param name="valuePart">Value string parsed from args.  Used to set boolOpt or to build error message.</param>
        /// <param name="allowDup">Specifies whether the switch can appear more than once.</param>
        private static void SetInt(ref int? intOpt, string switchPart, string valuePart, bool allowDup = false)
        {
            int parsedVal;

            if (intOpt.HasValue && !allowDup)
            {
                SetError("The '{0}' switch may only be used once.".Fmt(switchPart));
            }
            else if (valuePart.NullOrEmpty())
            {
                SetError("The {0} switch requires a value.".Fmt(switchPart));
            }
            else if (int.TryParse(valuePart, out parsedVal))
            {
                intOpt = parsedVal;
            }
            else
            {
                SetError("The value specified for the {0} switch is not a valid integer.".Fmt(switchPart));
            }
        }

        /// <summary>
        /// Sets the boolOpt or sets an error message depending on the valuePart and whether the value is valid, etc.
        /// </summary>
        /// <param name="boolOpt">Nullable bool whose value is to be set.</param>
        /// <param name="switchPart">Switch string parsed from args.  Used in error message if any.</param>
        /// <param name="valuePart">Value string parsed from args.  Used to set boolOpt or to build error message.</param>
        /// <param name="valSpec">Specifies whether the switch accepts or requires a value.</param>
        /// <param name="allowDup">Specifies whether the switch can appear more than once.</param>
        private static void SetBool(ref bool? boolOpt, string switchPart, string valuePart, ValReq valSpec = ValReq.Disallowed, bool allowDup = false)
        {
            if (boolOpt.HasValue && !allowDup)
            {
                SetError("The '{0}' switch may only be used once.".Fmt(switchPart));
            }
            else if (valuePart.NullOrEmpty())
            {
                if (valSpec == ValReq.Required)
                {
                    SetError("The {0} switch requires a value.".Fmt(switchPart));
                }
                else
                {
                    // When a boolean switch is specified without a value it means "true".
                    boolOpt = true;
                }
            }
            else if (valSpec == ValReq.Disallowed)
            {
                SetError("The {0} switch doesn't accept a value (i.e. \"{1}\").".Fmt(switchPart, valuePart));
            }
            else if (valuePart == "-" || valuePart == "false")
            {
                boolOpt = false;
            }
            else if (valuePart == "+" || valuePart == "true")
            {
                boolOpt = true;
            }
            else
            {
                string err = "Invalid value \"{0}\" specified for boolean switch {1}.\nValid values are \"true\", \"false\", '+', '-'.".Fmt(valuePart, switchPart);

                if (valSpec != ValReq.Required)
                {
                    err += "\nThe value may also be omitted, which implies \"true\".";
                }

                SetError(err);
            }
        }

        private static void SetString(ref string strOpt, string switchPart, string valuePart, bool allowDup = false)
        {
            if (strOpt == null || allowDup)
            {
                if (valuePart == "")
                {
                    SetError("The '{0}' switch requires a value.".Fmt(switchPart));
                }
                else
                {
                    strOpt = valuePart;
                }
            }
            else
            {
                SetError("The '{0}' switch may only be used once.".Fmt(switchPart));
            }
        }

        private static void SetError(string errorMessage)
        {
            // Every error passed in is logged but we only keep and display the first one.

            Log.Error(errorMessage);

            if (ErrorMessage == null)
            {
                ErrorMessage = errorMessage;
            }
        }
    }
}
