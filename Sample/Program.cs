using System;
using System.Threading;
using System.IO;
using TracerX;

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
            Logger.Xml.Configure(new FileInfo("LoggerConfig.xml"));
            return Logger.FileLogging.Open();
        }

        static void Main(string[] args) 
        {
            using (Log.InfoCall()) 
            {
                Helper.Bar();
                Helper.Foo();
            }
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
