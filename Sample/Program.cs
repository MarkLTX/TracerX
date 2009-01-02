using System;
using System.Threading;
using System.IO;
using TracerX;
using System.Diagnostics;
using System.Reflection;

namespace Sample 
{
    // Demonstrate basic features of the TracerX logger.
    class Program 
    {
        // Declare a Logger instance for use by this class.
        private static readonly Logger Log = Logger.GetLogger("Program");

        // Just one way to initialize TracerX early.
        private static bool LogFileOpened = InitLogging();

        // Initialize the TracerX logging system.
        private static bool InitLogging() 
        {
            Thread.CurrentThread.Name = "MainThread";
            Logger.Xml.Configure("LoggerConfig.xml");

            // Override some settings loaded from LoggerConfig.xml.
            Logger.FileLogging.Name = "SampleLog";
            Logger.FileLogging.MaxSizeMb = 1;
            Logger.FileLogging.CircularStartSizeKb = 1;

            // Open the output file.
            return Logger.FileLogging.Open();
        }

        static void Main(string[] args) 
        {
            Log.Debug("A message logged at stack depth = 0.");

            using (Log.InfoCall()) 
            {
                Log.Info("FriendlyName = ", AppDomain.CurrentDomain.FriendlyName);
                Log.Info("BaseDirectory = ", AppDomain.CurrentDomain.BaseDirectory);

                Log.Info("A message \nwith multiple \nembedded \nnewlines.");

                Recurse(0, 10);
                Helper.Foo();
            }

            Log.Debug("Another message logged at stack depth = 0.");
        }

        private static void Recurse(int i, int max) {
            using (Log.InfoCall("Recurse " + i)) {
                Log.Info("Depth = ", i);
                if (i == max) return;
                else Recurse(i + 1, max);
            }
        }     
    }

    class Helper {
        // Declare a Logger instance for use by this class.
        private static readonly Logger Log = Logger.GetLogger("Helper");
        private static string big = new string('x', 1000);

        public static void Foo() 
        {
            using (Logger.Current.DebugCall()) 
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
