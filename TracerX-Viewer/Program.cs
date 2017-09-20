using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using TracerX.ExtensionMethods;

namespace TracerX
{
    class Program
    {
        private static Logger Log = Logger.GetLogger("Program");
        private static bool _didOpenLog = InitLogging();

        [STAThread()]
        static void Main()
        {
            _didOpenLog = !!_didOpenLog; // InitLogging() doesn't get called without this reference to _didOpenLog!

            try
            {
                Application.EnableVisualStyles();
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                Log.Error("Exception in Main: ", ex);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("Unhandled exception passed to CurrentDomain_UnhandledException:", e.ExceptionObject);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Log.Error("Unhandled exception passed to Application_ThreadException:", e.Exception);
        }

        private static bool InitLogging()
        {
            if (System.Threading.Thread.CurrentThread.Name == null)
            {
                System.Threading.Thread.CurrentThread.Name = "Main Thread";
            }

            Logger.Root.BinaryFileTraceLevel = TraceLevel.Debug;
            Logger.Root.DebugTraceLevel = TraceLevel.Warn;
            Logger.DefaultBinaryFile.MaxSizeMb = 10;
            Logger.DefaultBinaryFile.CircularStartSizeKb = 20;
            Logger.DefaultBinaryFile.Directory = "%LOCAL_APPDATA%\\TracerX\\ViewerLogs";

            // Open the output file.
            bool result = Logger.DefaultBinaryFile.Open();
            Logger.GrantReadAccess(Logger.DefaultBinaryFile.Directory,  authenticatedUsers: true);
            Log.Info("Log file path = ", Logger.DefaultBinaryFile.FullPath);

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            return result;
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

        // Renders an ExceptionDetail and its nested InnerExceptions into a 
        // single string containing the exception types and messages.
        public static string GetNestedDetails(ExceptionDetail detail)
        {
            int depth = 0;
            string result = null;

            if (detail.InnerException == null)
            {
                result = detail.Type + ": " + detail.Message;
            }
            else
            {
                result = "Outer " + detail.Type + ": " + detail.Message;
                detail = detail.InnerException;

                while (detail != null)
                {
                    ++depth;
                    result += "\nInner (depth {0}) {1}: {2}".Fmt(depth, detail.Type, detail.Message ?? "<null message>");
                    detail = detail.InnerException;
                }

            }

            return result;
        }
    }
}
