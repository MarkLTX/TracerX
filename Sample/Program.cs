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

            // Override some settings loaded from LoggerConfig.xml.
            Logger.FileLogging.Name = "x2";
            Logger.FileLogging.MaxSizeMb = 1;
            Logger.FileLogging.CircularStartSizeKb = 1;

            // Open the output file.
            return Logger.FileLogging.Open();
        }

        static void Main(string[] args) 
        {
            using (Log.InfoCall()) 
            {
                //Debug.Print("FullName " + AppDomain.CurrentDomain.ApplicationIdentity.FullName); // null reference
                //Debug.Print("CodeBase " + AppDomain.CurrentDomain.ApplicationIdentity.CodeBase); // null reference
                Log.Info("FriendlyName = ", AppDomain.CurrentDomain.FriendlyName);
                Log.Info("BaseDirectory = ", AppDomain.CurrentDomain.BaseDirectory);

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
