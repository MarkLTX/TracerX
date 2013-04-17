using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TracerX.Viewer;

namespace TracerX.Viewer
{
    class Program
    {
        [STAThread()]
        static void Main()
        {
            string arg = GetArg();
            Application.EnableVisualStyles();
            Application.Run(new MainForm(arg));
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.Print(e.ExceptionObject.ToString());
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Debug.Print(e.Exception.ToString());
        }


        public static string FormatTimeSpan(TimeSpan ts)
        {
            string raw = ts.ToString();
            int colon = raw.IndexOf(':');
            int period = raw.IndexOf('.', colon);

            if (period == -1)
            {
                return raw;
            }
            else if (period + 4 >= raw.Length)
            {
                return raw;
            }
            else
            {
                return raw.Remove(period + 4);
            }
        }

        // Since we only accept one argument (a file path), this gets the first command line argument, if any.  
        // It handles arguments passed via a ClickOnce shortcut as well as "regular" arguments.
        private static string GetArg()
        {
            string result = null;

            try
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    // This means we were called via the ClickOnce shortcut, and we have to get our fileName this way.

                    if (AppDomain.CurrentDomain.SetupInformation != null &&
                        AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null &&
                        AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null)
                    {
                        string[] inputArgs = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;

                        if (inputArgs.Length > 0)
                        {
                            // Always, inputArgs.Length == 1 and inputArgs[0] contains everything we're ever going to get.
                            // The way to pass multiple fileName this way is to put everything in quotes on the command line.
                            // This means inputArgs[0] may actually contain multiple blank-separated arguments, but cannot
                            // contain any quotes.  

                            if (inputArgs[0].ToLower().StartsWith("http://"))
                            {
                                // This is the first (automatic) execution from being installed via clickOnce.  Ignore it.
                            }
                            else
                            {
                                // The file name will contain "%20" for blanks, and maybe other wierdness.
                                // This URI treatment should fix that.

                                Uri uri = new Uri(inputArgs[0]);
                                result = uri.LocalPath;
                            }
                        }
                    }
                }
                else
                {
                    // Return the second argument (or null).  The first argument is the executable name.

                    string[] array = Environment.GetCommandLineArgs();
                    result = array.Skip(1).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }

            return result;
        }
    }
}
