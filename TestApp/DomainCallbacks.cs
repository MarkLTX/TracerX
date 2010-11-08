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
            Logger.BinaryFileLogging.Directory = "%EXEDIR%";
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
        public bool Use_00;
        public bool UseKbForSize;

        public void LoadConfig() {
            Logger.Xml.Configure("..\\..\\TestAppConfig.xml");
        }

        public void Empty() {
            Logger.BinaryFileLogging.Name = "Empty.tx1";
            Logger.StandardData.BinaryFileTraceLevel = TracerX.TraceLevel.Off;
            Logger.BinaryFileLogging.Open();
            Logger.BinaryFileLogging.Close();

            Logger.TextFileLogging.Name = "Empty";
            Log.TextFileTraceLevel = TracerX.TraceLevel.Verbose;
            Logger.TextFileLogging.Open();
            Logger.TextFileLogging.Close();

            MessageBox.Show("Generated " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));
        }

        public void OneLine() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.BinaryFileLogging.Name = "OneLine";
            Logger.StandardData.BinaryFileTraceLevel = TracerX.TraceLevel.Off;
            Logger.BinaryFileLogging.Open();

            Logger.TextFileLogging.Name = "OneLine";
            Log.TextFileTraceLevel = TracerX.TraceLevel.Verbose;
            Logger.TextFileLogging.Open();

            Log.Warn("One line from ", AppDomain.CurrentDomain.FriendlyName);

            Logger.BinaryFileLogging.Close();
            Logger.TextFileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));
        }

        public void FiveThreadsNonCircular() {
            iterations = 1000;
            pauseEvery = 10;
            semaphore = new Semaphore(0, 5);


            Log.BinaryFileTraceLevel = TracerX.TraceLevel.Debug;
            Logger.BinaryFileLogging.Name = "FiveThreadsNonCircular.tx1";
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 0;
            Logger.BinaryFileLogging.CircularStartSizeKb = 0;
            Logger.BinaryFileLogging.Open();

            Log.TextFileTraceLevel = TracerX.TraceLevel.Verbose;
            Logger.TextFileLogging.Name = "FiveThreadsNonCircular";
            Logger.TextFileLogging.Open();

            ThreadPool.QueueUserWorkItem(new WaitCallback(LogNLines), "One");
            ThreadPool.QueueUserWorkItem(new WaitCallback(LogNLines), "Two");
            ThreadPool.QueueUserWorkItem(new WaitCallback(LogNLines), "Three");
            ThreadPool.QueueUserWorkItem(new WaitCallback(LogNLines), "Four");
            ThreadPool.QueueUserWorkItem(new WaitCallback(LogNLines), "Five");

            for (int i = 5; i > 0; --i) semaphore.WaitOne();

            Logger.BinaryFileLogging.Close();
            Logger.TextFileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));
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
            Logger.BinaryFileLogging.Name = "StopAtCircular.tx1";
            Logger.BinaryFileLogging.CircularStartSizeKb = 100;
            Logger.BinaryFileLogging.Open();

            Log.Info("Circular log should start at ", 100 * 1024, " bytes.");

            while (!Logger.BinaryFileLogging.CircularStarted) {
                Log.InfoFormat("File size is {0}", Logger.BinaryFileLogging.CurrentSize);
            }

            Logger.BinaryFileLogging.Close();
            TextStopAtCircular();
            MessageBox.Show("Generated " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));
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
            Logger.BinaryFileLogging.Name = "CircularWith1Block.tx1";
            Logger.BinaryFileLogging.MaxSizeMb = 1;
            Logger.BinaryFileLogging.CircularStartSizeKb = 1000;
            Logger.BinaryFileLogging.Open();

            Log.Info("Circular log should start at ", 1000 * 1024, " bytes.");
            Log.Info("There should be one message in the circular part, with some messages lost.");

            while (!Logger.BinaryFileLogging.CircularStarted) {
                Log.InfoFormat("File size is {0}", Logger.BinaryFileLogging.CurrentSize);
            }

            string big = new string('a', 25000);
            Log.Info(big);

            big = new string('b', 25000);
            Log.Info(big);

            //big = new string('c', 25000);
            //Log.Info(big);

            Logger.BinaryFileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));
        }

        public void OneThreadWraps() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.BinaryFileLogging.Name = "OneThreadWraps.tx1";
            Logger.BinaryFileLogging.MaxSizeMb = 2;
            Logger.BinaryFileLogging.Open();

            while (!Logger.BinaryFileLogging.Wrapped) {
                Log.InfoFormat("File size is {0}", Logger.BinaryFileLogging.CurrentSize);
            }

            for (int i = 0; i < 10000; ++i) {
                Log.InfoFormat("i = {0}, i squared = {1}", i, i * i);
            }

            Logger.BinaryFileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));
        }

        public void FiveThreadsWrap() {
            Log.BinaryFileTraceLevel = TracerX.TraceLevel.Debug;
            Logger.BinaryFileLogging.Name = "FiveThreadsWrap.tx1";
            Logger.StandardData.BinaryFileTraceLevel = TracerX.TraceLevel.Off;
            Logger.BinaryFileLogging.MaxSizeMb = 2;
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 0;
            Logger.BinaryFileLogging.CircularStartSizeKb = 250;
            Logger.BinaryFileLogging.Open();

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

            Logger.BinaryFileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));
        }

        public void StartCircularByTime() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.BinaryFileLogging.Name = "StartCircularByTime.tx1";
            Logger.BinaryFileLogging.MaxSizeMb = 20;
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 1;
            Logger.BinaryFileLogging.CircularStartSizeKb = 2000;
            Logger.BinaryFileLogging.Open();

            Log.Info("CircularStartDelaySeconds = ", Logger.BinaryFileLogging.CircularStartDelaySeconds);

            while (!Logger.BinaryFileLogging.CircularStarted) {
                Log.InfoFormat("File size is {0}", Logger.BinaryFileLogging.CurrentSize);
                Thread.Sleep(100);
            }

            Log.Info("Now in circular.");

            Logger.BinaryFileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));
        }

        public void MissingMethodEntry() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.BinaryFileLogging.Name = "MissingMethodEntry.tx1";
            Logger.BinaryFileLogging.MaxSizeMb = 1;
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 1;
            Logger.BinaryFileLogging.Open();

            Log.Info("Sleeping long enough for circular to start.");
            Thread.Sleep(1100);

            using (Log.InfoCall("Tom")) {
                while (!Logger.BinaryFileLogging.Wrapped) {
                    Log.WarnFormat("File size is {0}, position is {1}.", Logger.BinaryFileLogging.CurrentSize, Logger.BinaryFileLogging.CurrentPosition);
                }
            }

            Logger.BinaryFileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));
        }

        public void MissingMethodExit() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.BinaryFileLogging.Name = "MissingMethodExit.tx1";
            Logger.BinaryFileLogging.MaxSizeMb = 1;
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 1;
            Logger.EventLogging.MaxInternalEventNumber = 10000;
            Logger.BinaryFileLogging.Open();

            using (Log.InfoCall()) {
                Log.Info("Sleeping long enough for circular to start.");
                Thread.Sleep(1100);
                Log.Info("About to exit Main.");
            }

            while (!Logger.BinaryFileLogging.Wrapped) {
                Log.WarnFormat("File size is {0}, position is {1}.", Logger.BinaryFileLogging.CurrentSize, Logger.BinaryFileLogging.CurrentPosition);
            }

            Log.WarnFormat("First wrapped line: File size is {0}, position is {1}.", Logger.BinaryFileLogging.CurrentSize, Logger.BinaryFileLogging.CurrentPosition);

            Logger.BinaryFileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));
        }


        public void MissingEntriesAndExits() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.BinaryFileLogging.Name = "MissingEntriesAndExits.tx1";
            Logger.BinaryFileLogging.MaxSizeMb = 1;
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 1;
            Logger.BinaryFileLogging.Open();

            using (Log.InfoCall())
            using (Log.InfoCall("ShouldHaveMissingExit")) {
                Log.Info("Sleeping long enough for circular to start.");
                Thread.Sleep(1100);
                Log.Info("About to exit Main.");

            }

            using (Log.InfoCall("ShouldHaveMissingEntry1"))
            using (Log.InfoCall("ShouldHaveMissingEntry2")) {

                while (!Logger.BinaryFileLogging.Wrapped) {
                    Log.WarnFormat("File size is {0}, position is {1}.", Logger.BinaryFileLogging.CurrentSize, Logger.BinaryFileLogging.CurrentPosition);
                }

                Log.WarnFormat("First wrapped line: File size is {0}, position is {1}.", Logger.BinaryFileLogging.CurrentSize, Logger.BinaryFileLogging.CurrentPosition);
            }

            Logger.BinaryFileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));
        }

        public void AllUnicodeChars() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.BinaryFileLogging.Name = "AllUnicodeChars.tx1";
            Log.BinaryFileTraceLevel = TracerX.TraceLevel.Verbose;
            Logger.BinaryFileLogging.Open();

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

            Logger.BinaryFileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));
        }

        public void MoreThanUintMaxLines() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.BinaryFileLogging.Name = "MoreThanUintMaxLines.tx1";
            Logger.BinaryFileLogging.MaxSizeMb = 100;
            Logger.BinaryFileLogging.CircularStartSizeKb = 1;
            Logger.StandardData.BinaryFileTraceLevel = TracerX.TraceLevel.Off;
            Logger.BinaryFileLogging.Open();

            for (long i = 1; i < (long)uint.MaxValue + 99; ++i) {
                Log.WarnFormat("File size is {0}, position is {1}.", Logger.BinaryFileLogging.CurrentSize, Logger.BinaryFileLogging.CurrentPosition);
            }

            Logger.BinaryFileLogging.Close();
            MessageBox.Show("Generated " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));
        }

        public void ControlledLogging() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.BinaryFileLogging.Name = "ControlledLogging";
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 0;
            Logger.BinaryFileLogging.CircularStartSizeKb = 0;
            Logger.BinaryFileLogging.MaxSizeMb = 1;
            Logger.StandardData.BinaryFileTraceLevel = TracerX.TraceLevel.Off;
            Logger.BinaryFileLogging.Open();
            MessageBox.Show("Opened " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));

            var dlg = new ControlledLogging();
            dlg.ShowDialog();

            Logger.BinaryFileLogging.Close();
        }

        //public void Time1MillionWithWrapping() {
        //    if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
        //    Logger.BinaryFileLogging.Name = "Time1MillionWithWrapping";
        //    Logger.BinaryFileLogging.CircularStartDelaySeconds = 0;
        //    Logger.BinaryFileLogging.CircularStartSizeKb = 1;
        //    Logger.BinaryFileLogging.MaxSizeMb = 1;
        //    Logger.BinaryFileLogging.Open();
        //    MessageBox.Show("Opened " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));

        //    int i;
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();

        //    for (i = 0; i < 1000000; ++i) {
        //        Log.Info("i = ", i);
        //    }

        //    sw.Stop();
        //    Log.Info("Elapased time for ", i.ToString("N0"), " messages = ", sw.Elapsed.TotalSeconds);
        //    Logger.BinaryFileLogging.Close();

        //    MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
        //}

        //public void Time1MillionWithOutWrapping() {
        //    if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
        //    Logger.BinaryFileLogging.Name = "Time1MillionWithOutWrapping";
        //    Logger.BinaryFileLogging.CircularStartDelaySeconds = 0;
        //    Logger.BinaryFileLogging.CircularStartSizeKb = 0;
        //    Logger.BinaryFileLogging.MaxSizeMb = 100;
        //    Logger.BinaryFileLogging.Open();
        //    MessageBox.Show("Opened " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));

        //    int i;
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();

        //    for (i = 0; i < 1000000; ++i) {
        //        Log.Info("i = ", i);
        //    }

        //    sw.Stop();
        //    Log.Info("Elapased time for ", i.ToString("N0"), " messages = ", sw.Elapsed.TotalSeconds);
        //    Logger.BinaryFileLogging.Close();

        //    MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
        //}

        public void ThreadNames() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.BinaryFileLogging.Name = "ThreadNames";
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 0;
            Logger.BinaryFileLogging.CircularStartSizeKb = 1;
            Logger.BinaryFileLogging.MaxSizeMb = 1;
            Logger.BinaryFileLogging.Open();
            MessageBox.Show("Opened " + Logger.BinaryFileLogging.FullPath);

            Log.Info("No thread name set.");

            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";

            Log.Info("Thread.CurrentThread.Name = \"Main\"");

            Logger.ThreadName = "Override";

            Log.Info("Logger.ThreadName = \"Override\"");

            Logger.ThreadName = null;

            Log.Info("Logger.ThreadName = null");

            Logger.ThreadName = "Another";

            Log.Info("Logger.ThreadName = \"Another\"");

            Logger.BinaryFileLogging.Close();
        }

        private static string[] names = new string[] { "All", "Your", "Base", "Are", "Belong", "To", "Us" };
        private static Random random;
        private static int callDepth;

        public void RandomCall() {
            if (callDepth == 5) return;

            if (random == null) {
                random = new Random();
                if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
                Logger.BinaryFileLogging.Name = "RandomCalls";
                Logger.BinaryFileLogging.CircularStartDelaySeconds = 0;
                Logger.BinaryFileLogging.CircularStartSizeKb = 0;
                Logger.BinaryFileLogging.MaxSizeMb = 1;
                Logger.BinaryFileLogging.Open();
                MessageBox.Show("Opened " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));
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
            Logger.BinaryFileLogging.Name = "AppendToMe";
            Logger.BinaryFileLogging.MaxSizeMb = MaxSessionSizeMb;
            Logger.BinaryFileLogging.AppendIfSmallerThanMb = MaxAppendableFileMb;
            Logger.BinaryFileLogging.Use_00 = Use_00;
            Logger.BinaryFileLogging.UseKbForSize = UseKbForSize;
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 0;

            if (AppendTestMode == AppendMode.ExceedMaxMb || AppendTestMode == AppendMode.NoCircular) {
                Logger.BinaryFileLogging.CircularStartSizeKb = 0;
            } else {
                Logger.BinaryFileLogging.CircularStartSizeKb = CircularStartKb;
            }

            Logger.Root.BinaryFileTraceLevel = TracerX.TraceLevel.Verbose;
            Logger.StandardData.BinaryFileTraceLevel = TracerX.TraceLevel.Off;
        }

        private void TextInitAppendTest() {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.TextFileLogging.Name = "AppendToMe";
            Logger.TextFileLogging.MaxSizeMb = MaxSessionSizeMb;
            Logger.TextFileLogging.AppendIfSmallerThanMb = MaxAppendableFileMb;
            Logger.TextFileLogging.Use_00 = Use_00;

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
            Logger.BinaryFileLogging.AppendIfSmallerThanMb = 0; // Always create new file.
            LogToAppendFile();
            MessageBox.Show("Created and closed " + Logger.BinaryFileLogging.FullPath);
        }

        // This creates a new AppendToMe.tx1 file for another logging session to append to.
        public void CreateTextFileForAppending() {
            TextInitAppendTest();
            Logger.TextFileLogging.AppendIfSmallerThanMb = 0; // Always create new file.
            LogToAppendTextFile();
            MessageBox.Show("Created and closed " + Logger.TextFileLogging.FullPath);
        }

        private void LogToAppendFile() {
            Logger.BinaryFileLogging.Open();

            switch (AppendTestMode) {
                case AppendMode.Empty:
                    break;
                case AppendMode.NoCircular:
                    Log.Info("Should be only line in session, circular not started.");
                    if (Logger.BinaryFileLogging.CircularStarted) {
                        Log.Info("OOPS! Circular log started somehow.");
                    }
                    break;
                case AppendMode.CircularNoWrap:
                    while (!Logger.BinaryFileLogging.CircularStarted) {
                        Log.Debug("Logging till circular log starts at " + CircularStartKb + " Kb.");
                    }

                    Log.Debug("CircularStarted = ", Logger.BinaryFileLogging.CircularStarted);
                    break;
                case AppendMode.CircularWrap:
                    while (!Logger.BinaryFileLogging.Wrapped) {
                        Log.Debug("Logging till circular log wraps at " + MaxSessionSizeMb + " Mb.");
                    }

                    Log.Debug("Wrapped = ", Logger.BinaryFileLogging.Wrapped);
                    break;
                case AppendMode.ExceedMaxMb:
                    long prevSize = -1;
                    long curSize = Logger.BinaryFileLogging.CurrentSize;

                    while (prevSize < curSize) {
                        prevSize = curSize;
                        Log.Info("Logging until file stops growing, current size is ", Logger.BinaryFileLogging.CurrentSize);
                        curSize = Logger.BinaryFileLogging.CurrentSize;
                    }

                    Log.Info("File size did not grow, probably closed.");
                    break;
            }

            Logger.BinaryFileLogging.Close();
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
            MessageBox.Show("Finished logging to " + Logger.BinaryFileLogging.FullPath);
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

            TextWriterTraceListener myTextListener = SetupTraceListener("SimpleTextWriterLog.txt");

            // Log some stuff that should appear in the text file.
            Log.Verbose("The time is ", DateTime.Now);
            using (Log.DebugCall()) {
                Log.Info("This was logged from within a method.");
                Log.Info("The format string is ", Logger.DebugLogging.FormatString);
            }

            myTextListener.Writer.Close();
            myTextListener.Close();
        }

        private static TextWriterTraceListener SetupTraceListener(string outputFile)
        {
            // Create a file in the executable dir for output named TextWriterLog.txt.
            //string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, outputFile);
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
            return myTextListener;
        }

        public void LogToAllDestinations()
        {
            // Set up binary file destination.
            Logger.BinaryFileLogging.Name = "AllDestinationsBinaryFileOut";
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 0;
            Logger.BinaryFileLogging.CircularStartSizeKb = 1;
            Logger.BinaryFileLogging.MaxSizeMb = 1;
            Logger.Root.BinaryFileTraceLevel = TracerX.TraceLevel.Verbose;
            Logger.BinaryFileLogging.Open();
            MessageBox.Show("Opened " + Path.Combine(Logger.BinaryFileLogging.Directory, Logger.BinaryFileLogging.Name));

            // Set up Debug (Trace) listener destination.
            TextWriterTraceListener myTextListener = SetupTraceListener("AllDestinationsDebugOut.txt");

            // Set up EventHandler destination.
            SetupEventHandler();
            
            // Set up the text file destination.
            Log.TextFileTraceLevel = TracerX.TraceLevel.Verbose;
            Logger.TextFileLogging.Name = "AllDestinationsTextFileOut.txt";
            Logger.TextFileLogging.Open();
            MessageBox.Show("Opened " + Path.Combine(Logger.TextFileLogging.Directory, Logger.TextFileLogging.Name));

            // Set up the event log destination
            Logger.Root.EventLogTraceLevel = TracerX.TraceLevel.Verbose;
            Logger.EventLogging.EventLog.Source = "TracerX - All Destinations";

            // Generate some output.
            random = new Random();
            RandomCall();

            // Close output files.
            myTextListener.Writer.Close();
            myTextListener.Close();
        }

        static StreamWriter _eventHandlerOut;

        private static void SetupEventHandler()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AllDestinationsEventHandlerOut.txt");
            _eventHandlerOut = new StreamWriter(filePath);
            MessageBox.Show("Opened " + filePath);

            Logger.Root.EventHandlerTraceLevel = TracerX.TraceLevel.Verbose;
            Logger.MessageCreated += Logger_MessageCreated;
        }

        private static void Logger_MessageCreated(object sender, Logger.MessageCreatedEventArgs e)
        {
            Logger logger = sender as Logger;
            string space = new string(' ', 3 * e.Indentation);
            string msg = string.Format("{7}Logger = {6}, Thread = {0}, # = {1}, depth = {2}, method = {3}, level = {4}, message = {5}",
                Thread.CurrentThread.Name,
                e.ThreadNumber,
                e.Indentation,
                e.Method,
                e.TraceLevel,
                e.Message,
                logger.Name,
                space
                );

            _eventHandlerOut.WriteLine(msg);
        }

        // This opens a log file but does not close it.  The "main" AppDomain creates
        // a Logger and uses it to log to the file created here.
        public void CrossAppDomains()
        {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Named in domain that owns the log file.";
            Logger.BinaryFileLogging.Name = "CrossAppDomains";
            Logger.BinaryFileLogging.Open();
            MessageBox.Show("Opened " + Logger.BinaryFileLogging.FullPath);

            Logger LocalLog = Logger.GetLogger("Local Log");

            LocalLog.Info("Logged in local app domain.");
            Log.Info("Also logged in local app domain.");
        }

        public void OpenClose()
        {
            Logger.BinaryFileLogging.Name = "OpenClose";
            Logger.BinaryFileLogging.Directory = "%EXEDIR%";
            Logger.BinaryFileLogging.Use_00 = true;
            Logger.BinaryFileLogging.Archives = 3;
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 0;
            Logger.BinaryFileLogging.CircularStartSizeKb = 0;

            Logger Log2 = Logger.GetLogger("Log2");

            // Test what happens when method-entries are logged before the file is opened.
            using (Log.InfoCall("Meth-1"))
            {
                using (Log.InfoCall("Meth-2"))
                {
                    Log.Info("Logged inside Meth-2 before opening file");
                    Logger.BinaryFileLogging.Open();
                    Log.Info("Logged inside Meth-2 after opening first file");
                }
            }

            Log.Info("Not inside a method.");
            Log.Info("Closing the file.");
            Logger.BinaryFileLogging.Close();
            
            Logger.BinaryFileLogging.Open();
            Log.Info("Opened second file, entering a method.");

            using (Log.InfoCall("Meth-3"))
            {
                Log.Info("Closing file while in Meth-3.");
                Logger.BinaryFileLogging.Close();
                Logger.BinaryFileLogging.Open();
                Log.Info("Logged inside Meth-3 after opening third file.");
            }

            Log.Info("Closing last file.");
            Logger.BinaryFileLogging.Close();
        }
    }
}
