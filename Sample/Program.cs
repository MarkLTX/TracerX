using System;
using System.Threading;
using System.IO;
using TracerX;
//using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace Sample 
{
    class Program 
    {
        private static bool b = WatchAssemblies();

        private static bool WatchAssemblies() {
                AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
                return true;
        }

        private static readonly Logger Log = Logger.GetLogger("Program");

        // Just one way to initialize TracerX early.
        private static bool LogFileOpened = InitLogging();

        // Initialize the TracerX logging system.
        private static bool InitLogging() 
        {
            Thread.CurrentThread.Name = "MainThread";
            Logger.Xml.Configure(new FileInfo("LoggerConfig.xml"));
            return Logger.FileLogging.Open();
        }

        static void Main(string[] args) 
        {
            using (Log.InfoCall()) 
            {
                Helper.Bar();
                Helper.Foo();

                //System.Web.HttpContext.Current;
                //System.Web.HttpRuntime;
                //Log.Info("AppDomain.CurrentDomain.ApplicationIdentity.CodeBase = ", AppDomain.CurrentDomain.ApplicationIdentity.CodeBase);
                //Log.Info("AppDomain.CurrentDomain.ApplicationIdentity.FullName = ", AppDomain.CurrentDomain.ApplicationIdentity.FullName);
                Log.Info("Process.GetCurrentProcess().ProcessName = ", Process.GetCurrentProcess().ProcessName);
                Log.Info("Assembly.GetEntryAssembly().CodeBase = ", Assembly.GetEntryAssembly().CodeBase);
                Log.Info("Assembly.GetEntryAssembly().Location = ", Assembly.GetEntryAssembly().Location);
                Log.Info("Assembly.GetEntryAssembly().GetName().Version = ", Assembly.GetEntryAssembly().GetName().Version);
                Log.Info("Assembly.GetEntryAssembly().GetName().Name = ", Assembly.GetEntryAssembly().GetName().Name);
                Log.Info("AppDomain.CurrentDomain.BaseDirectory = ", AppDomain.CurrentDomain.BaseDirectory);
                Log.Info("AppDomain.CurrentDomain.FriendlyName = ", AppDomain.CurrentDomain.FriendlyName);
                //Log.Info("Application.StartupPath = ", Application.StartupPath);
                //Log.Info("Application.CommonAppDataPath = ", Application.CommonAppDataPath);
                //Log.Info("Application.ExecutablePath = ", Application.ExecutablePath);
                //Log.Info("Application.LocalUserAppDataPath = ", Application.LocalUserAppDataPath);
                //Log.Info("Application.UserAppDataPath = ", Application.UserAppDataPath);
                Log.Info("SpecialFolder.ApplicationData = ", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Log.Info("SpecialFolder.CommonApplicationData = ", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
                Log.Info("SpecialFolder.CommonProgramFiles = ", Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
                Log.Info("SpecialFolder.Desktop = ", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                Log.Info("SpecialFolder.DesktopDirectory = ", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                Log.Info("SpecialFolder.LocalApplicationData = ", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                Log.Info("SpecialFolder.MyComputer = ", Environment.GetFolderPath(Environment.SpecialFolder.MyComputer));
                Log.Info("SpecialFolder.MyDocuments = ", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                Log.Info("SpecialFolder.Personal = ", Environment.GetFolderPath(Environment.SpecialFolder.Personal));

                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
                    Console.WriteLine(asm.GetName().Name);
                }

            }
        }

        static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args) {
            Debug.Print(args.LoadedAssembly.FullName);
        }
    }

    class Helper {
        private static readonly Logger Log = Logger.GetLogger("Helper");

        public static void Foo() 
        {
            using (Log.DebugCall()) 
            {
                Log.Debug(DateTime.Now, " is the current time.");
                Bar();
            }
        }

        public static void Bar() 
        {
            using (Log.DebugCall()) 
            {
                Log.Debug("This object's type is ", typeof(Helper));
            }
        }
    }
}
