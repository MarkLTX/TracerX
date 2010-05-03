using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using TracerX;
using System.IO;
using System.Diagnostics;

namespace TestApp {
    // Most methods in this class are intended to be called in a "helper" AppDomain via
    // AppDomain.DoCallBack().  Since the class is not MarshallByReferenceObject,
    // the helper AppDomain gets copy-by-value copy of the instance passed to DoCallBack().
    // This allows us to pass "parameters" in to the AppDomain by setting the class members,
    // but does not allow us to get results back.  Static members are always reinitialized
    // in the helper AppDomain (each AppDomain has its own statics).  
    [Serializable]
    internal class DomainCallbacks {

        static DomainCallbacks() {
            Logger.FileLogging.Directory = "%EXEDIR%";
            Logger.TextFileLogging.Directory = "%EXEDIR%";
        }

        private static readonly Logger Log = Logger.GetLogger("TestApp");
        private static long iterations;
        private static long pauseEvery;
        private static Semaphore semaphore;

        // AppendMode is used for testing the file append feature.
        public enum AppendMode { Empty, NoCircular, CircularNoWrap, CircularWrap, ExceedMaxMb };
        public AppendMode AppendTestMode;
        public uint MaxSessionSizeMb;
        public uint MaxAppendableFileMb;
        public uint CircularStartKb;

        public void LoadConfig() {
            Logger.Xml.Configure("..\\..\\TestAppConfig.xml");
        }

        public void Empty() {
            Logger.FileLogging.Name = "Empty.tx1";
            Logger.StandardData.FileTraceLevel = TracerX.TraceLevel.Off;
            Logger.FileLogging.Open();
            Logger.FileLogging.Close();

            Logger.TextFileLogging.Name = "Empty";
            Log.TextFileTraceLevel = TracerX.TraceLevel.Verbose;
            Logger.TextFileLogging.Open();
            Logger.TextFileLogging.Close();

            MessageBox.Show("Generated " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));
        }

        public void OneLine() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.FileLogging.Name = "OneLine";
            Logger.StandardData.FileTraceLevel = TracerX.TraceLevel.Off;
            Logger.FileLogging.Open();

            Logger.TextFileLogging.Name = "OneLine";
            Log.TextFileTraceLevel = TracerX.TraceLevel.Verbose;
            Logger.TextFileLogging.Open();

            Log.Warn("One line from ", AppDomain.CurrentDomain.FriendlyName);

            Logger.FileLogging.Close();
            Logger.TextFileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));
        }

        public void FiveThreadsNonCircular() {
            iterations = 1000;
            pauseEvery = 10;
            semaphore = new Semaphore(0, 5);


            Log.FileTraceLevel = TracerX.TraceLevel.Debug;
            Logger.FileLogging.Name = "FiveThreadsNonCircular.tx1";
            Logger.FileLogging.CircularStartDelaySeconds = 0;
            Logger.FileLogging.CircularStartSizeKb = 0;
            Logger.FileLogging.Open();

            Log.TextFileTraceLevel = TracerX.TraceLevel.Verbose;
            Logger.TextFileLogging.Name = "FiveThreadsNonCircular";
            Logger.TextFileLogging.Open();

            ThreadPool.QueueUserWorkItem(new WaitCallback(LogNLines), "One");
            ThreadPool.QueueUserWorkItem(new WaitCallback(LogNLines), "Two");
            ThreadPool.QueueUserWorkItem(new WaitCallback(LogNLines), "Three");
            ThreadPool.QueueUserWorkItem(new WaitCallback(LogNLines), "Four");
            ThreadPool.QueueUserWorkItem(new WaitCallback(LogNLines), "Five");

            for (int i = 5; i > 0; --i) semaphore.WaitOne();

            Logger.FileLogging.Close();
            Logger.TextFileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));
        }

        public void LogNLines(object name) {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = (string)name;

            using (Log.DebugCall((string)name)) {
                for (long i = 1; i <= iterations; ++i) {
                    Log.Info(i, " of ", iterations);
                    if (i % pauseEvery == 0) Thread.Sleep(1);
                }
            }

            semaphore.Release();
        }

        public void StopAtCircular() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.FileLogging.Name = "StopAtCircular.tx1";
            Logger.FileLogging.CircularStartSizeKb = 100;
            Logger.FileLogging.Open();

            Log.Info("Circular log should start at ", 100 * 1024, " bytes.");

            while (!Logger.FileLogging.CircularStarted) {
                Log.InfoFormat("File size is {0}", Logger.FileLogging.CurrentSize);
            }

            Logger.FileLogging.Close();
            TextStopAtCircular();
            MessageBox.Show("Generated " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));
        }

        public void TextStopAtCircular() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Log.TextFileTraceLevel = TracerX.TraceLevel.Verbose;
            Logger.TextFileLogging.Name = "StopAtCircular.tx1";
            Logger.TextFileLogging.CircularStartSizeKb = 100;
            Logger.TextFileLogging.Open();

            Log.Info("Circular log should start at ", 100 * 1024, " bytes.");

            while (!Logger.TextFileLogging.CircularStarted) {
                Log.InfoFormat("File size is {0}", Logger.TextFileLogging.CurrentSize);
            }

            Logger.TextFileLogging.Close();
        }

        public void CircularWith1Block() {
            // When the circular log starts, this writes several large messages, each large enough
            // to wrap the file so the result is only one block in the circular area.
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.FileLogging.Name = "CircularWith1Block.tx1";
            Logger.FileLogging.MaxSizeMb = 1;
            Logger.FileLogging.CircularStartSizeKb = 1000;
            Logger.FileLogging.Open();

            Log.Info("Circular log should start at ", 1000 * 1024, " bytes.");
            Log.Info("There should be one message in the circular part, with some messages lost.");

            while (!Logger.FileLogging.CircularStarted) {
                Log.InfoFormat("File size is {0}", Logger.FileLogging.CurrentSize);
            }

            string big = new string('a', 25000);
            Log.Info(big);

            big = new string('b', 25000);
            Log.Info(big);

            //big = new string('c', 25000);
            //Log.Info(big);

            Logger.FileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));
        }

        public void OneThreadWraps() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.FileLogging.Name = "OneThreadWraps.tx1";
            Logger.FileLogging.MaxSizeMb = 2;
            Logger.FileLogging.Open();

            while (!Logger.FileLogging.Wrapped) {
                Log.InfoFormat("File size is {0}", Logger.FileLogging.CurrentSize);
            }

            for (int i = 0; i < 10000; ++i) {
                Log.InfoFormat("i = {0}, i squared = {1}", i, i * i);
            }

            Logger.FileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));
        }

        public void FiveThreadsWrap() {
            Log.FileTraceLevel = TracerX.TraceLevel.Debug;
            Logger.FileLogging.Name = "FiveThreadsWrap.tx1";
            Logger.StandardData.FileTraceLevel = TracerX.TraceLevel.Off;
            Logger.FileLogging.MaxSizeMb = 2;
            Logger.FileLogging.CircularStartDelaySeconds = 0;
            Logger.FileLogging.CircularStartSizeKb = 250;
            Logger.FileLogging.Open();

            iterations = 25000;
            pauseEvery = 100;
            semaphore = new Semaphore(0, 5);

            Log.Info("Circular log should start at ", 250 * 1024, " bytes");

            ThreadPool.QueueUserWorkItem(new WaitCallback(LogNLines), "One");
            ThreadPool.QueueUserWorkItem(new WaitCallback(LogNLines), "Two");
            ThreadPool.QueueUserWorkItem(new WaitCallback(LogNLines), "Three");
            ThreadPool.QueueUserWorkItem(new WaitCallback(LogNLines), "Four");
            ThreadPool.QueueUserWorkItem(new WaitCallback(LogNLines), "Five");

            for (int i = 5; i > 0; --i) semaphore.WaitOne();

            Logger.FileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));
        }

        public void StartCircularByTime() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.FileLogging.Name = "StartCircularByTime.tx1";
            Logger.FileLogging.MaxSizeMb = 20;
            Logger.FileLogging.CircularStartDelaySeconds = 1;
            Logger.FileLogging.CircularStartSizeKb = 2000;
            Logger.FileLogging.Open();

            Log.Info("CircularStartDelaySeconds = ", Logger.FileLogging.CircularStartDelaySeconds);

            while (!Logger.FileLogging.CircularStarted) {
                Log.InfoFormat("File size is {0}", Logger.FileLogging.CurrentSize);
                Thread.Sleep(100);
            }

            Log.Info("Now in circular.");

            Logger.FileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));
        }

        public void MissingMethodEntry() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.FileLogging.Name = "MissingMethodEntry.tx1";
            Logger.FileLogging.MaxSizeMb = 1;
            Logger.FileLogging.CircularStartDelaySeconds = 1;
            Logger.FileLogging.Open();

            Log.Info("Sleeping long enough for circular to start.");
            Thread.Sleep(1100);

            using (Log.InfoCall("Tom")) {
                while (!Logger.FileLogging.Wrapped) {
                    Log.WarnFormat("File size is {0}, position is {1}.", Logger.FileLogging.CurrentSize, Logger.FileLogging.CurrentPosition);
                }
            }

            Logger.FileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));
        }

        public void MissingMethodExit() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.FileLogging.Name = "MissingMethodExit.tx1";
            Logger.FileLogging.MaxSizeMb = 1;
            Logger.FileLogging.CircularStartDelaySeconds = 1;
            Logger.EventLogging.MaxInternalEventNumber = 10000;
            Logger.FileLogging.Open();

            using (Log.InfoCall()) {
                Log.Info("Sleeping long enough for circular to start.");
                Thread.Sleep(1100);
                Log.Info("About to exit Main.");
            }

            while (!Logger.FileLogging.Wrapped) {
                Log.WarnFormat("File size is {0}, position is {1}.", Logger.FileLogging.CurrentSize, Logger.FileLogging.CurrentPosition);
            }

            Log.WarnFormat("First wrapped line: File size is {0}, position is {1}.", Logger.FileLogging.CurrentSize, Logger.FileLogging.CurrentPosition);

            Logger.FileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));
        }


        public void MissingEntriesAndExits() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.FileLogging.Name = "MissingEntriesAndExits.tx1";
            Logger.FileLogging.MaxSizeMb = 1;
            Logger.FileLogging.CircularStartDelaySeconds = 1;
            Logger.FileLogging.Open();

            using (Log.InfoCall())
            using (Log.InfoCall("ShouldHaveMissingExit")) {
                Log.Info("Sleeping long enough for circular to start.");
                Thread.Sleep(1100);
                Log.Info("About to exit Main.");

            }

            using (Log.InfoCall("ShouldHaveMissingEntry1"))
            using (Log.InfoCall("ShouldHaveMissingEntry2")) {

                while (!Logger.FileLogging.Wrapped) {
                    Log.WarnFormat("File size is {0}, position is {1}.", Logger.FileLogging.CurrentSize, Logger.FileLogging.CurrentPosition);
                }

                Log.WarnFormat("First wrapped line: File size is {0}, position is {1}.", Logger.FileLogging.CurrentSize, Logger.FileLogging.CurrentPosition);
            }

            Logger.FileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));
        }

        public void AllUnicodeChars() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.FileLogging.Name = "AllUnicodeChars.tx1";
            Log.FileTraceLevel = TracerX.TraceLevel.Verbose;
            Logger.FileLogging.Open();

            Log.Info("This log contains ALL Unicode code points in the range 0 - 0x10FFFF.");
            Log.Info("This includes the invalid non-paired surrogates, which should render as '?',");
            Log.Info("and many unassigned and non-printable code points.");

            System.Text.StringBuilder str = new System.Text.StringBuilder();
            int start = 0;
            int stop = 15;

            while (stop <= 0x10FFFF) {
                str.Length = 0;

                for (int i = start; i <= stop; ++i) {
                    string utf16 = "";

                    try {
                        utf16 = char.ConvertFromUtf32(i);
                    } catch (Exception ex) {
                        if (i <= char.MaxValue) {
                            utf16 = new string((char)i, 1);
                        } else {
                            // This never happens.
                            throw;
                        }
                    }

                    str.Append(utf16);
                }

                Log.DebugFormat("U+{0:X} - U+{1:X}: {2}", start, stop, str);
                start += 16;
                stop += 16;
            }

            Logger.FileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));
        }

        public void MoreThanUintMaxLines() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.FileLogging.Name = "MoreThanUintMaxLines.tx1";
            Logger.FileLogging.MaxSizeMb = 100;
            Logger.FileLogging.CircularStartSizeKb = 1;
            Logger.StandardData.FileTraceLevel = TracerX.TraceLevel.Off;
            Logger.FileLogging.Open();

            for (long i = 1; i < (long)uint.MaxValue + 99; ++i) {
                Log.WarnFormat("File size is {0}, position is {1}.", Logger.FileLogging.CurrentSize, Logger.FileLogging.CurrentPosition);
            }

            Logger.FileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));
        }

        public void ControlledLogging() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.FileLogging.Name = "ControlledLogging";
            Logger.FileLogging.CircularStartDelaySeconds = 0;
            Logger.FileLogging.CircularStartSizeKb = 0;
            Logger.FileLogging.MaxSizeMb = 1;
            Logger.StandardData.FileTraceLevel = TracerX.TraceLevel.Off;
            Logger.FileLogging.Open();
            MessageBox.Show("Opened " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));

            var dlg = new ControlledLogging();
            dlg.ShowDialog();

            Logger.FileLogging.Close();
        }

        public void Time1MillionWithWrapping() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.FileLogging.Name = "Time1MillionWithWrapping";
            Logger.FileLogging.CircularStartDelaySeconds = 0;
            Logger.FileLogging.CircularStartSizeKb = 1;
            Logger.FileLogging.MaxSizeMb = 1;
            Logger.FileLogging.Open();
            MessageBox.Show("Opened " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));

            int i;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (i = 0; i < 1000000; ++i) {
                Log.Info("i = ", i);
            }

            sw.Stop();
            Log.Info("Elapased time for ", i.ToString("N0"), " messages = ", sw.Elapsed.TotalSeconds);
            Logger.FileLogging.Close();

            MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
        }

        public void Time1MillionWithOutWrapping() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.FileLogging.Name = "Time1MillionWithOutWrapping";
            Logger.FileLogging.CircularStartDelaySeconds = 0;
            Logger.FileLogging.CircularStartSizeKb = 0;
            Logger.FileLogging.MaxSizeMb = 100;
            Logger.FileLogging.Open();
            MessageBox.Show("Opened " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));

            int i;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (i = 0; i < 1000000; ++i) {
                Log.Info("i = ", i);
            }

            sw.Stop();
            Log.Info("Elapased time for ", i.ToString("N0"), " messages = ", sw.Elapsed.TotalSeconds);
            Logger.FileLogging.Close();

            MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
        }

        private static string[] names = new string[] { "All", "Your", "Base", "Are", "Belong", "To", "Us" };
        private static Random random;
        private static int callDepth;

        public void RandomCall() {
            if (callDepth == 5) return;

            if (random == null) {
                random = new Random();
                if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
                Logger.FileLogging.Name = "RandomCalls";
                Logger.FileLogging.CircularStartDelaySeconds = 0;
                Logger.FileLogging.CircularStartSizeKb = 0;
                Logger.FileLogging.MaxSizeMb = 1;
                Logger.FileLogging.Open();
                MessageBox.Show("Opened " + Path.Combine(Logger.FileLogging.Directory, Logger.FileLogging.Name));
            }

            int ndx = random.Next(names.Length);

            using (Log.InfoCall(names[ndx])) {
                ++callDepth;
                Log.Info("Call depth = ", callDepth);
                RandomCall();
                RandomCall();
                RandomCall();
                --callDepth;
            }
        }

        private void InitAppendTest() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.FileLogging.Name = "AppendToMe";
            Logger.FileLogging.MaxSizeMb = MaxSessionSizeMb;
            Logger.FileLogging.AppendIfSmallerThanMb = MaxAppendableFileMb;

            if (AppendTestMode == AppendMode.ExceedMaxMb || AppendTestMode == AppendMode.NoCircular) {
                Logger.FileLogging.CircularStartSizeKb = 0;
            } else {
                Logger.FileLogging.CircularStartSizeKb = CircularStartKb;
            }

            Logger.Root.FileTraceLevel = TracerX.TraceLevel.Verbose;
            Logger.StandardData.FileTraceLevel = TracerX.TraceLevel.Off;
        }

        private void TextInitAppendTest() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.TextFileLogging.Name = "AppendToMe";
            Logger.TextFileLogging.MaxSizeMb = MaxSessionSizeMb;
            Logger.TextFileLogging.AppendIfSmallerThanMb = MaxAppendableFileMb;

            if (AppendTestMode == AppendMode.ExceedMaxMb || AppendTestMode == AppendMode.NoCircular) {
                Logger.TextFileLogging.CircularStartSizeKb = 0;
            } else {
                Logger.TextFileLogging.CircularStartSizeKb = CircularStartKb;
            }

            Logger.TextFileLogging.FormatString = "{line} {msg}";
            Logger.Root.TextFileTraceLevel = TracerX.TraceLevel.Verbose;
        }

        // This creates a new AppendToMe.tx1 file for another logging session to append to.
        public void CreateFileForAppending() {
            InitAppendTest();
            Logger.FileLogging.AppendIfSmallerThanMb = 0; // Always create new file.
            LogToAppendFile();
            MessageBox.Show("Created and closed " + Logger.FileLogging.FullPath);
        }

        // This creates a new AppendToMe.tx1 file for another logging session to append to.
        public void CreateTextFileForAppending() {
            TextInitAppendTest();
            Logger.TextFileLogging.AppendIfSmallerThanMb = 0; // Always create new file.
            LogToAppendTextFile();
            MessageBox.Show("Created and closed " + Logger.TextFileLogging.FullPath);
        }

        private void LogToAppendFile() {
            Logger.FileLogging.Open();

            switch (AppendTestMode) {
                case AppendMode.Empty:
                    break;
                case AppendMode.NoCircular:
                    Log.Info("Should be only line in session, circular not started.");
                    if (Logger.FileLogging.CircularStarted) {
                        Log.Info("OOPS! Circular log started somehow.");
                    }
                    break;
                case AppendMode.CircularNoWrap:
                    while (!Logger.FileLogging.CircularStarted) {
                        Log.Debug("Logging till circular log starts at " + CircularStartKb + " Kb.");
                    }

                    Log.Debug("CircularStarted = ", Logger.FileLogging.CircularStarted);
                    break;
                case AppendMode.CircularWrap:
                    while (!Logger.FileLogging.Wrapped) {
                        Log.Debug("Logging till circular log wraps at " + MaxSessionSizeMb + " Mb.");
                    }

                    Log.Debug("Wrapped = ", Logger.FileLogging.Wrapped);
                    break;
                case AppendMode.ExceedMaxMb:
                    long prevSize = -1;
                    long curSize = Logger.FileLogging.CurrentSize;

                    while (prevSize < curSize) {
                        prevSize = curSize;
                        Log.Info("Logging until file stops growing, current size is ", Logger.FileLogging.CurrentSize);
                        curSize = Logger.FileLogging.CurrentSize;
                    }

                    Log.Info("File size did not grow, probably closed.");
                    break;
            }

            Logger.FileLogging.Close();
        }

        private void LogToAppendTextFile() {
            Logger.TextFileLogging.Open();

            switch (AppendTestMode) {
                case AppendMode.Empty:
                    break;
                case AppendMode.NoCircular:
                    Log.Info("Should be only line in session, circular not started.");
                    if (Logger.TextFileLogging.CircularStarted) {
                        Log.Info("OOPS! Circular log started somehow.");
                    }
                    break;
                case AppendMode.CircularNoWrap:
                    while (!Logger.TextFileLogging.CircularStarted) {
                        Log.Debug("Logging till circular log starts at " + CircularStartKb + " Kb.");
                    }

                    Log.Debug("CircularStarted = ", Logger.TextFileLogging.CircularStarted);
                    break;
                case AppendMode.CircularWrap:
                    while (!Logger.TextFileLogging.Wrapped) {
                        Log.Debug("Logging till circular log wraps at " + MaxSessionSizeMb + " Mb.");
                    }

                    Log.Debug("Wrapped = ", Logger.TextFileLogging.Wrapped);
                    break;
                case AppendMode.ExceedMaxMb:
                    long stopSize = Logger.TextFileLogging.CurrentSize + (MaxSessionSizeMb << 20);
                    long prevSize = -1;
                    long curSize = Logger.TextFileLogging.CurrentSize;

                    while (prevSize < curSize) {
                        prevSize = curSize;
                        Log.Info("Logging until file is reopened, current size is ", Logger.TextFileLogging.CurrentSize);
                        curSize = Logger.TextFileLogging.CurrentSize;
                    }

                    Log.Info("File size did not grow, probably restarted.");
                    break;
            }

            Logger.TextFileLogging.Close();
        }

        // This appends to the AppendToMe.tx1 file unless it's already too big.
        // It grows the file by 1 Mb.
        public void AppendToFile() {
            InitAppendTest();
            LogToAppendFile();
            MessageBox.Show("Finished logging to " + Logger.FileLogging.FullPath);
        }

        // This appends to the AppendToMe.tx1 file unless it's already too big.
        // It grows the file by 1 Mb.
        public void AppendToTextFile() {
            TextInitAppendTest();
            LogToAppendTextFile();
            MessageBox.Show("Finished logging to " + Logger.TextFileLogging.FullPath);
        }

        public void TextWriter() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";

            // Create a file in the executable dir for output named TextWriterLog.txt.
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TextWriterLog.txt");
            FileStream myFile = File.Create(filePath);

            MessageBox.Show("Opened " + filePath);

            // Create a new text writer using the output stream, and add it to
            // the trace listeners.
            TextWriterTraceListener myTextListener = new TextWriterTraceListener(myFile);
            Trace.Listeners.Add(myTextListener);

            // TracerX won't call Trace.WriteLine() unless we set the DebugTraceLevel
            // to something other than Off.
            Logger.Root.DebugTraceLevel = TracerX.TraceLevel.Verbose;

            Logger.DebugLogging.FormatString = "{time,-12:HH:mm:ss.fff} {level,-8} {msg}";

            // Log some stuff that should appear in the text file.
            Log.Verbose("The time is ", DateTime.Now);
            using (Log.DebugCall()) {
                Log.Info("This was logged from within a method.");
                Log.Info("The format string is ", Logger.DebugLogging.FormatString);
            }

            myTextListener.Close();
        }
    }
}
