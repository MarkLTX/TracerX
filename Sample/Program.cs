using System;
using System.Threading;
using System.IO;
using TracerX;
using System.Diagnostics;
using System.Reflection;

namespace Sample 
{
    class Program 
    {
        private static readonly Logger Log = Logger.GetLogger("Program");

        // Just one way to initialize TracerX early.
        private static bool LogFileOpened = InitLogging();

        // Initialize the TracerX logging system.
        private static bool InitLogging() 
        {
            Thread.CurrentThread.Name = "MainThread";
            Logger.Xml.Configure("LoggerConfig.xml");
            Logger.FileLogging.Name = "x2";
            Logger.FileLogging.MaxSizeMb = 1;
            Logger.FileLogging.CircularStartSizeKb = 1;
            return Logger.FileLogging.Open();
        }

        static void Main(string[] args) 
        {
            //Debug.Print("FullName " + AppDomain.CurrentDomain.ApplicationIdentity.FullName); // null reference
            //Debug.Print("CodeBase " + AppDomain.CurrentDomain.ApplicationIdentity.CodeBase); // null reference
            Console.WriteLine("FriendlyName = " + AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine("BaseDirectory = " + AppDomain.CurrentDomain.BaseDirectory);

            Console.WriteLine("GetAppDir = " + GetAppDir());
            Console.WriteLine("GetDefaultDir = " + GetDefaultDir());
            Console.WriteLine("GetAppName = " + GetAppName());
            
            Console.ReadKey();
            return;

            using (Log.InfoCall()) 
            {
                Recurse(0, 10);
                Helper.Foo();
            }
        }

        private static void Recurse(int i, int max) {
            using (Log.InfoCall(i.ToString())) {
                Log.Info("Depth = ", i);
                if (i == max) return;
                else Recurse(i + 1, max);
            }
        }
       
        private static string GetAppName() {
            try {
                // If this works, we're a web app.
                string dir = System.Web.HttpRuntime.AppDomainAppPath.TrimEnd('\\', '/');
                return Path.GetFileNameWithoutExtension(dir);
            } catch (Exception) {
                // Not a web app, probably a Windows.Forms app, but could be
                // something else like a Visio plugin (seriously).
                try {
                    Assembly entryAssembly = Assembly.GetEntryAssembly();

                    if (entryAssembly == null) {
                        // This seems to be a reasonable value in Windows.Forms apps,
                        // but it's not pretty in web apps.
                        return AppDomain.CurrentDomain.FriendlyName;
                    } else {
                        return entryAssembly.GetName().Name;
                    }
                } catch (Exception) {
                    // Give up. Return something to avoid an exception.
                    return "TracerX_App";
                }
            }
        }

        private static string GetAppDir() {
            try {
                // If this works, we're a web app.
                return System.Web.HttpRuntime.AppDomainAppPath.TrimEnd('\\', '/');
            } catch (Exception) {
                // Not a web app, probably a Windows.Forms app, but could be
                // something else like a Visio plugin (seriously).
                try {
                    Assembly entryAssembly = Assembly.GetEntryAssembly();

                    if (entryAssembly == null) {
                        // This seems to be the expected value for web and Windows.Forms apps.
                        return AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\', '/');
                    } else {
                        return Path.GetDirectoryName(entryAssembly.Location);
                    }
                } catch (Exception) {
                    try {
                        return Environment.CurrentDirectory;
                    } catch (Exception) {
                        // Give up.  Return something to avoid an exception.
                        return "C:\\";
                    }
                }
            }
        }

        private static string GetDefaultDir() {
            try {
                return System.Web.HttpRuntime.AppDomainAppPath.TrimEnd('\\', '/');
            } catch (Exception) {
                // Not a web app
                try {
                    return Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                } catch (Exception) {
                    // Give up. Return something to avoid an exception.
                    return "C:\\";
                }
            }
        }

    }

    class Helper {
        private static readonly Logger Log = Logger.GetLogger("Helper");
        private static string big = new string('x', 1000);

        public static void Foo() 
        {
            using (Log.DebugCall()) 
            {
                for (int i = 0; i < 1000; ++i) {
                    Log.Debug("i = ", i);
                    Bar(i);
                }
            }
        }

        public static void Bar(int i) 
        {
            using (Log.DebugCall()) 
            {
                Log.Debug("i*i = ", i * i);
                Log.Debug("big = ", big);
            }
        }
    }
}
