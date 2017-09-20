using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Reflection;
using System.IO;
using System.Configuration.Install;
using System.ServiceModel;
using System.Diagnostics;
using TracerX.ExtensionMethods;
using System.Threading;

namespace TracerX
{
    // This program is little more than a wrapper for TracerX-ServiceImpl.dll, which contains all the
    // implementation of the WCF service(s). Other programs may also host the DLL. This program is
    // normally run as a service but may be run as a console app to install the service (-i switch),
    // uninstall the service (-u switch) or simply run the WCF services in a console app (-c switch).
    // Use the -h switch to display all command line options (see AppArgs.cs). Some
    // configuration settings are also read from the .config file since services can't use command
    // line arguments.
    static class Program
    {
        public static readonly string ExePath = Assembly.GetEntryAssembly().Location;
        public static readonly string ExeDir = ExePath.PathDir();
        public static readonly string ExeName = ExePath.PathLeaf();
        public static readonly Version ExeVersion = Assembly.GetEntryAssembly().GetName().Version;
        public static readonly string DataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).AddPath("TracerX");
        public static string ServiceName;

        // All Loggers in this service and TracerX-ServiceImpl.dll are children of "TX1"
        // so they can easily be turned off via the TX1 Logger.
        private static Logger Log = Logger.GetLogger("TX1.Service.Program");
        private static bool _didOpenLog = InitLogging(); // Opens log file before Main().
        private static object _lock = new object();
        private static System.Timers.Timer _wcfRetryTimer;

        private static System.Timers.Timer _selfUpdateTimer;
        private static int _updateIntervalMinutes = Properties.Settings.Default.SelfUpdateIntervalMinutes;
        private static string _updateSourcePath = Properties.Settings.Default.SelfUpdateSourcePath;

        static void Main()
        {
            using (Log.InfoCall())
            {
                try
                {
                    if (AppArgs.ParseCommandLine())
                    {
                        if (AppArgs.IsInstall)
                        {
                            // Install this program as a service.
                            Log.Info("Installing the service.");
                            Console.WriteLine("Installing the service.");
                            ManagedInstallerClass.InstallHelper(new string[] { ExePath });
                        }
                        else if (AppArgs.IsUninstall)
                        {
                            // Uninstall this service.
                            Log.Info("Uninstalling the service.");
                            Console.WriteLine("Uninstalling the service.");
                            ManagedInstallerClass.InstallHelper(new string[] { "/u", ExePath });
                        }
                        else if (AppArgs.IsConsoleApp)
                        {
                            // Run as a console app by starting the WCF services.  Then wait for
                            // the user to press a key to keep the process running.
                            Start();
                            Console.WriteLine("Running as a console app using port {0}.  Press any key to exit.", AppArgs.Port);

                            Log.Info("Waiting for key press.");
                            var key = Console.ReadKey();
                            Log.Info("User pressed '", key.Key, "', exiting.");

                            // Stop the WCF services and allow the process to terminate.
                            Stop();
                        }
                        else
                        {
                            // Run as a service.  Service1.OnStart() will call our Start() method.
                            Log.Info("Running as a service.");
                            Console.WriteLine("Running as a service.");
                            var service = new Service1();
                            ServiceName = service.ServiceName;
                            ServiceBase[] ServicesToRun = new ServiceBase[] { service };
                            ServiceBase.Run(ServicesToRun);
                        }
                    }
                    else
                    {
                        // The error and/or help message has already been displayed.
                    }
                }
                catch (Exception ex)
                {
                    if (AppArgs.IsConsoleApp) Console.WriteLine(ex.Message);
                    Log.Fatal(ex);
                }
            }
        }

        // Whether we're a console app or a Windows service this is called to start the WCF service hosts.
        // May also start the retry and/or self-update timers.
        internal static void Start()
        {
            using (Log.InfoCall())
            {
                lock (_lock)
                {
                    MaybeStartSelfUpdateTimer();

                    if (AppArgs.RetryInterval <= 0)
                    {
                        // Retries are disabled.  If this throws an exception, e.g. if the port is in use by another process, we're done.
                        TracerXServices.Startup(AppArgs.Port, AppArgs.IsImpersonate);
                    }
                    else
                    {
                        // If the port is currently in use by another process start a timer to 
                        // retry acquiring the port every AppArgs.RetryInterval minutes.

                        try
                        {
                            TracerXServices.Startup(AppArgs.Port, AppArgs.IsImpersonate);
                        }
                        catch (System.ServiceModel.AddressAlreadyInUseException)
                        {
                            if (AppArgs.IsConsoleApp)
                            {
                                Console.WriteLine("Starting the retry timer because the port ({0}) used by the", AppArgs.Port);
                                Console.WriteLine("TracerX WCF service is currently in use by another process.");
                                Console.WriteLine("The retry interval is {0} minutes.", AppArgs.RetryInterval);
                            }

                            Log.Warn("Starting the retry timer because the port used by the TracerX WCF service is currently in use by another process.  Port = " + AppArgs.Port);
                            _wcfRetryTimer = new System.Timers.Timer(60000.0 * AppArgs.RetryInterval);
                            _wcfRetryTimer.AutoReset = false;
                            _wcfRetryTimer.Elapsed += RetryWcfService; 
                            _wcfRetryTimer.Start();
                        }
                    }
                }
            }
        }

        // Called to stop the retry and/or self-update timers and to close the WCF service hosts so
        // the program can terminate (whether it's a console app or a service).
        internal static void Stop() 
        {
            using (Log.InfoCall())
            {
                lock (_lock)
                {
                    if (_selfUpdateTimer != null)
                    {
                        _selfUpdateTimer.Dispose();
                        _selfUpdateTimer = null;
                    }

                    if (_wcfRetryTimer != null)
                    {
                        _wcfRetryTimer.Dispose();
                        _wcfRetryTimer = null;
                    }

                    // This can take a while (actually time out) if a client is still connected.
                    // Write a message to the console to explain the delay, which is 
                    // obvious when running in console mode.

                    Console.WriteLine("Closing ServiceHosts...");
                    TracerXServices.Shutdown(timeoutSeconds: 10);
                }
            }
        }

        // Initializes our own TracerX logging.
        private static bool InitLogging()
        {
            Logger.ThreadName = "Main";

            Logger.Root.BinaryFileTraceLevel = TraceLevel.Debug;
            //Logger.EventLogging.MaxInternalEventNumber = 9999;

            Logger.DefaultBinaryFile.MaxSizeMb = 25;
            Logger.DefaultBinaryFile.Use_00 = true;
            Logger.DefaultBinaryFile.Archives = 5;
            Logger.DefaultBinaryFile.Directory = "%LOCAL_APPDATA%\\TracerX\\ServiceLogs";

            // If the logging config file exists, use it.  This may override some of
            // the properties we just set above.

            FileInfo configFile = new FileInfo(ExeDir.AddPath("TracerX-Service_LoggingConfig.xml"));

            if (configFile.Exists)
            {
                Logger.Xml.ConfigureAndWatch(configFile);
            }

            bool result = Log.BinaryFile.Open(); // Should be the same as Logger.DefaultBinaryFile, used by all Loggers in this program.
            Logger.GrantReadAccess(Log.BinaryFile.Directory, authenticatedUsers:true);

            Log.Info(configFile, " exists = ", configFile.Exists);
            Log.Info("Framework version = ", Environment.Version);
            Log.Info("Log directory = ", Logger.DefaultBinaryFile.Directory);

            return result;
        }

        // If the first attempt to start the WCF service fails due to AddressAlreadyInUseException
        // and retrying is enabled, this will get called every AppArgs.RetryInterval minutes to retry
        // as long it continues to fail due to AddressAlreadyInUseException.
        public static void RetryWcfService(object sender, System.Timers.ElapsedEventArgs e)
        {
            // This is called via _wcfRetryTimer on a ThreadPool thread.  Temporarily change the thread name for logging purposes.

            using (Log.ThreadNameForCall("TracerX Starter"))
            {
                lock (_lock)
                {
                    if (_wcfRetryTimer != null && _wcfRetryTimer.Enabled)
                    {
                        try
                        {
                            Log.Debug("Attempting to start TracerX WCF services.");
                            TracerXServices.Startup(AppArgs.Port, AppArgs.IsImpersonate);

                            // Getting here means Startup() succeeded (no exception) so we can stop/dispose/release the timer.

                            if (AppArgs.IsConsoleApp) Console.WriteLine("The TracerX WCF service was started.");
                            Log.Info("TracerX WCF service was started.");
                            _wcfRetryTimer.Dispose();
                            _wcfRetryTimer = null;
                        }
                        catch (System.ServiceModel.AddressAlreadyInUseException ex)
                        {
                            Log.Info("The TracerX WCF service port is still in use by another process.");

                            // Schedule another retry. Timer does not AutoReset so we have to start
                            // it explicitly.

                            _wcfRetryTimer.Interval = 60000.0 * AppArgs.RetryInterval;
                            _wcfRetryTimer.Start();
                        }
                        catch (Exception ex)
                        {
                            // This is unexpected and probably means we'll never be able to
                            // start the TracerX service host so just give up and stop the program.

                            Log.Fatal("Unexpected exception trying to start TracerX WCF service host: ", ex);
                            Stop();
                        }
                    }
                }
            }
        }

        private static void MaybeStartSelfUpdateTimer()
        {
            if (AppArgs.IsConsoleApp)
            {
                // Self-updating isn't done in console app mode.
            }
            else if (_updateIntervalMinutes > 0 && !_updateSourcePath.NullOrWhiteSpace())
            {
                // Self update is enabled.
                // Create the self-update timer that periodically checks if we need to self-update.
                // Schedule the first call for 1 second from now. 

                Log.Info("Starting self-update timer.");
                _selfUpdateTimer = new System.Timers.Timer(1000);
                _selfUpdateTimer.Elapsed += _selfUpdateTimer_Elapsed;
                _selfUpdateTimer.AutoReset = false;
                _selfUpdateTimer.Start();
            }
            else
            {
                Log.Info("Self-update is not enabled.");
            }
        }

        static void _selfUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            using (Log.ThreadNameForCall("Self-Update"))
            {
                lock (_lock)
                {
                    // If the timer still exists then the service hasn't been asked to stop yet so check for an update.

                    if (_selfUpdateTimer != null)
                    {
                        // Look for a newer version of this EXE in the _updateSourcePath. An
                        // exception will occur if _updateSourcePath has invalid syntax, doesn't
                        // exist, doesn't contain a file with the same name as this executable,
                        // or we don't have read access.

                        try
                        {
                            Log.Info("Checking for newer EXE version in: ", _updateSourcePath);

                            string newExeFile = _updateSourcePath.AddPath(ExeName);
                            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(newExeFile);
                            Version newVersion = new Version(fvi.FileVersion);

                            if (newVersion > ExeVersion)
                            {
                                Log.Info("Found newer EXE version ", newVersion);
                                SelfUpdate();
                            }
                            else if (newVersion < ExeVersion)
                            {
                                Log.Warn("Found OLDER version ", newVersion);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Warn("Unable to self-update from path: ", _updateSourcePath);
                            Log.Warn(ex);
                        }

                        // Schedule another try at the configured interval.
                        _selfUpdateTimer.Interval = 60000.0 * _updateIntervalMinutes;
                        _selfUpdateTimer.Start();
                    }
                }                    
            }
        }

        private static void SelfUpdate()
        {
            // The _updateSourcePath is probably a remote share. Copy the files to a local temp
            // directory before we risk stopping this service/process. Also, we need a writable
            // location to generate a batch file in. I've had problems not having write access to
            // what Path.GetTempPath() returns so stay within the current user's profile.

            string tempDir = Environment
                .GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                .AddPath("TracerX-TempServiceUpdate");

            // We'll eventually run a batch file whose output will be
            // redirected to the following log file inside the temp dir.

            string batLog = tempDir.AddPath("SelfUpdateLog.txt");

            // If the temp dir exists we will delete it to ensure we don't end up copying any
            // "extra" files from it to our own executable's dir. Before doing that check if it
            // contains the batch file's output from the previous attempt and log
            // it's contents to this process' log file.  This makes it easier to debug any failures.

            Log.Info("Reading and logging the batch file's old output file (if it exists): ", batLog);
            LogTextFile(batLog);

            Log.Info("Deleting and creating temp dir: ", tempDir);
            DirectoryInfo tempDirInfo = new DirectoryInfo(tempDir);
            if (tempDirInfo.Exists) tempDirInfo.Delete(true);
            tempDirInfo.Create();

            // Wait 10 seconds to reduce the risk that files are still being copied into the
            // _updateSourcePath dir. We don't want to copy a mix of old and new files.

            Thread.Sleep(10000);
            Log.Info("Copying files from ", _updateSourcePath);

            foreach (string file in Directory.GetFiles(_updateSourcePath))
            {
                string fileName = file.PathLeaf();
                File.Copy(file, tempDir.AddPath(fileName));
            } 

            // Build a .bat file in the tempDir to stop, replace, and start the service.
            // Don't replace any files if the service fails to stop.
            // Note that "if errorlevel 1" actually means "if %errorlevel% >= 1".

            string batFile = tempDir.AddPath("SelfUpdate.bat");
            string content =
                "net stop {0} \n" +
                "if errorlevel 1 exit \n" +
                "copy /Y \"{1}\\*.*\"  \"{2}\" \n" +
                "net start {0} \n" +
                "exit";

            content = content.Fmt(ServiceName, tempDir, ExeDir);

            Log.Info("Creating .bat file to perform the update:\n", content);
            File.WriteAllText(batFile, content);

            // Launch the .bat file.  It will stop and restart this service.
            // Redirect the batch file's output for debugging purposes.  
            // The "2>&1" part redirects STDERR to the same place as STDOUT.  

            string cmdArgs = "> \"{0}\" 2>&1".Fmt(batLog);
            Log.InfoFormat("Starting update process: {0} {1}", batFile, cmdArgs);
            Process.Start(batFile, cmdArgs);

            // We can't check the output from the process we just started or wait for it to
            // terminate because the first thing it does is stop us and wait for us to terminate.
        }

        // This either reads the entire file into a string and logs it or
        // it logs the reason it couldn't read the file.
        private static void LogTextFile(string fullPath)
        {
            // If we get a FileNotFoundException or DirectoryNotFoundException
            // just log the self-explanatory exception name.
            // Any other exception is unexpected and fully logged.

            try
            {
                string result = System.IO.File.ReadAllText(fullPath);
                Log.Info("File's content is...");
                Log.Info(result);
            }
            catch (FileNotFoundException)
            {
                // The file doesn't exist.
                Log.Warn("Got FileNotFoundException reading file ", fullPath);
            }
            catch (DirectoryNotFoundException)
            {
                // The directory doesn't exist.
                Log.Warn("Got DirectoryNotFoundException reading file ", fullPath);
            }
            catch (Exception ex)
            {
                // Possibly "access is denied"
                Log.Warn("Failed to read file '", fullPath, "' due to the following exception...\n", ex);
            }
        }

    }
}
