using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Specialized;
using System.IO;
using TracerX.ExtensionMethods;

namespace TracerX
{
    // This class is responsible for parsing the command line 
    // and storing the results in its fields/properties.
    internal static class AppArgs
    {
        public static string FilePath;  // The only non-switch arg allowed.
        public static string ServerName; // From the -server switch.
        public static string FileFilter; // From the -filefilter switch.
        public static bool IsHelpWanted;  // From the -?, -h, or -help switch.

        public static string ErrorMessage;

        private static Logger Log = Logger.GetLogger("AppArgs");

        // Displays help about the syntax of this program's command line parameters, 
        // preceeded by an optional error message.
        public static void ShowHelp(string errMsg = null)
        {
            using (Log.InfoCall())
            {
                StringBuilder sb = new StringBuilder();

                if (!errMsg.NullOrWhiteSpace()) sb.AppendLine(errMsg);

                sb.AppendLine();
                sb.AppendLine("Command syntax:");
                sb.AppendLine("     TracerX-Viewer [<FilePath>] [-server:<ServerName>] ");
                sb.AppendLine("     [-filefilter:<Filter>]");
                sb.AppendLine();
                sb.AppendLine("  <FilePath>");
                sb.AppendLine("     Any non-switch argument is considered a FilePath, which specifies");
                sb.AppendLine("     the file to open. Only one non-switch argument (i.e. one FilePath)");
                sb.AppendLine("     is allowed. Use quotes if FilePath has embedded blanks.");
                sb.AppendLine("     If the -server (or -s) switch is present, FilePath");
                sb.AppendLine("     refers to a file on the specified ServerName.");
                sb.AppendLine();
                sb.AppendLine("  -server:<ServerName>");
                sb.AppendLine("     You may also use \"-s\".");
                sb.AppendLine("     Specifies the server to connect to.  The server must be ");
                sb.AppendLine("     running TracerX-Service.exe or another process that hosts");
                sb.AppendLine("     TracerX-ServiceImpl.dll.  First, the servers in the saved");
                sb.AppendLine("     settings are searched for one with a matching display name.");
                sb.AppendLine("     Then they are searched for a matching server");
                sb.AppendLine("     name/address.  If a saved server isn't found one is created.");
                sb.AppendLine();
                sb.AppendLine("  -filefilter:<Filter>");
                sb.AppendLine("     You may also use \"-ff\".");
                sb.AppendLine("     Specifies a filter string to use in the \"Filter:\" field on the Files tab.");
                sb.AppendLine("     Only files whose names contain the <Filter> string will appear.  ");
                sb.AppendLine("     This is useful if you want to help the user find a");
                sb.AppendLine("     particular file but you're not sure about the full path.");
                sb.AppendLine();
                sb.AppendLine("  -h | -help | -?");
                sb.AppendLine("     Any of these switches causes this help message to be displayed and ");
                sb.AppendLine("     all other arguments to be ignored.");
                sb.AppendLine();
                sb.AppendLine("  All arguments are optional.  Order is not significant.");
                sb.AppendLine("  Switch arguments such as \"-server\" may start with '-' or '/'.");
                sb.AppendLine("  The separator between a switch and its value may be ':' or '='.");
                sb.AppendLine("  Switch values that include blanks should be quoted.");
                sb.AppendLine();
                sb.AppendLine("  Example: ");
                sb.AppendLine("     TracerX-Viewer \"C:\\Log Folder\\Log File.tx1\" -server:\"The Server\"");

                // Since this is a GUI app with no console, we display a message box instead of writing to the console.

                MainForm.ShowMessageBox(sb.ToString());
            }
        }

        // Parses the command line and sets public fields/properties including IsHelpWanted and ErrorMessage.
        // If canDisplayMsg is true, a message box will be displayed if an error is found or the -help switch is found.  
        // Otherwise, the caller should display any error found and/or call ShowHelp().
        // Returns true if no error was found, false if the ErrorMessage property was set.
        public static bool ParseCommandLine(bool canDisplayMsg)
        {
            using (Log.InfoCall())
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
                        // It's a switch.
                        // In general switches can have optional values, like "-switch:value".  ParseSwitch() sets switchPart
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
                                    IsHelpWanted = true;
                                    break;

                                case "s":
                                case "server":
                                    if (valuePart.NullOrWhiteSpace())
                                    {
                                        SetError("The '{0}{1}' switch requires a value (i.e. the server name).".Fmt(switchFlag, switchPart));
                                    }
                                    else if (ServerName == null)
                                    {
                                        if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                                        {
                                            ServerName = ArgParserTX.PercentDecode(valuePart);
                                        }
                                        else
                                        {
                                            ServerName = valuePart;
                                        }
                                    }
                                    else
                                    {
                                        SetError("Two or more server names have been specified: '{0}' and '{1}'.".Fmt(ServerName, valuePart));
                                    }
                                    break;

                                case "ff":
                                case "filefilter":
                                    if (valuePart.NullOrWhiteSpace())
                                    {
                                        SetError("The '{0}{1}' switch requires a value (i.e. a file name filter).".Fmt(switchFlag, switchPart));
                                    }
                                    else if (FileFilter == null)
                                    {
                                        if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                                        {
                                            FileFilter = ArgParserTX.PercentDecode(valuePart);
                                        }
                                        else
                                        {
                                            FileFilter = valuePart;
                                        }
                                    }
                                    else
                                    {
                                        SetError("Two or more file filters have been specified: '{0}' and '{1}'.".Fmt(FileFilter, valuePart));
                                    }
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
                        // It's a non-switch.  This program allows a single non-switch to specify the file to view.

                        if (FilePath == null)
                        {
                            FilePath = arg;
                        }
                        else
                        {
                            SetError("More than one <FilePath> argument was specfied.  Are quotes needed?\n\n  First: {0}\n  Second: {1}".Fmt(FilePath, arg));
                        }
                    }
                } // foreach arg

                if (FilePath != null)
                {
                    if (FilePath.EndsWith(".application", StringComparison.OrdinalIgnoreCase))
                    {
                        // Assume this means we were launched via our own TracerX-Viewer.application file, when means don't actually have a file arg.
                        FilePath = null;
                    }
                    else
                    {
                        // If this app is installed as a ClickOnce app and the user double-clicks a
                        // file, the FilePath argument will be a URI such as
                        // "file:///c:\logs\may%20have%20blanks.tx1". That is, any blanks will be
                        // percent-encoded. We can construct a Uri object from the string and get the
                        // Url.LocalPath property.

                        Uri uri;
                        string localPath = FilePath;

                        if (Uri.TryCreate(FilePath, UriKind.Absolute, out uri))
                        {
                            localPath = uri.LocalPath;
                        }

                        // Constructing a FileInfo is the best way I know 
                        // to check if the file path syntax is valid.

                        try
                        {
                            FileInfo test = new FileInfo(localPath);
                            FilePath = localPath;
                        }
                        catch (Exception ex)
                        {
                            // Use the original argument in the error message.

                            SetError("Invalid file path syntax:\n\n  {0}".Fmt(FilePath));
                        }
                    }
                }

                if (IsHelpWanted || ErrorMessage != null)
                {
                    // Clear any args that were found so they are not acted on.

                    ServerName = null;
                    FilePath = null;

                    if (canDisplayMsg)
                    {
                        ShowHelp(ErrorMessage);
                    }
                }
                else
                {
                    Log.Info("AppArgs.FilePath = ", FilePath);
                    Log.Info("AppArgs.ServerName = ", ServerName);
                }

                return ErrorMessage == null;
            } // using Log
        }

        private static void SetError(string errorMessage)
        {
            // Every error found is logged but we only keep (and thus display) the first one.

            Log.Error(errorMessage);

            if (ErrorMessage == null)
            {
                ErrorMessage = errorMessage;
            }
        }
    }
}
