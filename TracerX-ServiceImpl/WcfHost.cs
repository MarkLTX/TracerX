using System;
using System.ServiceModel;
using System.Threading;
using System.ServiceModel.Description;

namespace TracerX
{
    /// <summary>
    /// Encapsulates instances of ServiceHost for TracerX's various WCF services (opens and closes them).
    /// The services can generate a lot of logging but all of the internal Loggers are children of "TX1", so
    /// you can easily disable all logging with this: Logger.GetLogger("TX1").BinaryFileTraceLevel = TraceLevel.Off;
    /// </summary>
    public static class TracerXServices
    {
        public static bool IsImpersonateClients
        {
            get;
            private set;
        }

        private static Logger Log = Logger.GetLogger("TX1.Service");
        private static ServiceHost _wcfFileEnumHost;
        private static ServiceHost _wcfFileReaderHost;
        private static ServiceHost _wcfFileMonitorHost;
        private static Semaphore _sem;
        private static object _lock = new object();

        /// <summary>
        /// Starts the ServiceHosts for the various WCF services. May throw an exception (e.g. if the
        /// specified port is already used by another process).
        /// </summary>
        /// <param name="port">Port number to listen on.  Default = 25120.</param>
        /// <param name="impersonateClients">True if WCF clients should be impersonated for security purposes when accessing log files.</param>
        public static void Startup(int port = 25120, bool impersonateClients = false)
        {
            using (Log.InfoCall())
            {
                // See comment at the end of this file for the equivalent XML configuration.

                NetTcpBinding tcpBinding = MakeBinding();
                IsImpersonateClients = impersonateClients;

                Log.Info("Starting with port = ", port, " and impersonateClients = ", impersonateClients);

                lock (_lock)
                {
                    OpenFileMonitor(port, tcpBinding);
                    OpenFileReader(port, tcpBinding);
                    OpenFileEnum(port, tcpBinding);
                }
            }
        }

        public static void Shutdown(int timeoutSeconds)
        {
            using (Log.InfoCall())
            {
                lock (_lock)
                {                    
                    // We use a worker thread for each ServiceHost to close them concurrently.
                    // To wait for all three threads to finish, we create a Semaphore with 0 "resources"
                    // remaining and wait for each thread to release one "resource".

                    _sem = new Semaphore(0, 3);

                    Log.Debug("Starting closer threads.");

                    ThreadPool.QueueUserWorkItem(notUsed => TryClose(_wcfFileEnumHost, "FileEnum", timeoutSeconds));
                    ThreadPool.QueueUserWorkItem(notUsed => TryClose(_wcfFileReaderHost, "FileReader", timeoutSeconds));
                    ThreadPool.QueueUserWorkItem(notUsed => TryClose(_wcfFileMonitorHost, "FileMonitor", timeoutSeconds));

                    Log.Debug("Waiting for closer threads.");

                    _sem.WaitOne();
                    _sem.WaitOne();
                    _sem.WaitOne();
                }
            }
        }

        private static void OpenFileMonitor(int port, NetTcpBinding tcpBinding)
        {
            _wcfFileMonitorHost = new ServiceHost(typeof(ImplFileMonitor));

            ((ServiceDebugBehavior)_wcfFileMonitorHost.Description.Behaviors[typeof(ServiceDebugBehavior)]).IncludeExceptionDetailInFaults = true;
            _wcfFileMonitorHost.OpenTimeout = TimeSpan.FromSeconds(12);
            _wcfFileMonitorHost.Closing += (sender, e) => Log.Info("_wcfFileMonitorHost is closing.");
            _wcfFileMonitorHost.Closed += (sender, e) => Log.Info("_wcfFileMonitorHost is closed.");
            _wcfFileMonitorHost.Faulted += (sender, e) => Log.Warn("_wcfFileMonitorHost is faulted.");

            _wcfFileMonitorHost.AddServiceEndpoint(
                typeof(IFileMonitor),
                tcpBinding,
               "net.tcp://localhost:" + port + "/TracerX-Service/FileMonitor");

            Log.Info("Opening the WCF ServiceHost for ImplFileMonitor.");
            _wcfFileMonitorHost.Open();
        }

        private static void OpenFileReader(int port, NetTcpBinding tcpBinding)
        {
            _wcfFileReaderHost = new ServiceHost(typeof(ImplFileReader));

            ((ServiceDebugBehavior)_wcfFileReaderHost.Description.Behaviors[typeof(ServiceDebugBehavior)]).IncludeExceptionDetailInFaults = true;
            _wcfFileReaderHost.OpenTimeout = TimeSpan.FromSeconds(12);
            _wcfFileReaderHost.Closing += (sender, e) => Log.Info("_wcfFileReaderHost is closing.");
            _wcfFileReaderHost.Closed += (sender, e) => Log.Info("_wcfFileReaderHost is closed.");
            _wcfFileReaderHost.Faulted += (sender, e) => Log.Warn("_wcfFileReaderHost is faulted.");

            _wcfFileReaderHost.AddServiceEndpoint(
                typeof(IFileReader),
                tcpBinding,
               "net.tcp://localhost:" + port + "/TracerX-Service/FileReader");

            Log.Info("Opening the WCF ServiceHost for ImplFileReader.");
            _wcfFileReaderHost.Open();
        }

        private static void OpenFileEnum(int port, NetTcpBinding tcpBinding)
        {
            _wcfFileEnumHost = new ServiceHost(typeof(ImplFileEnum));

            ((ServiceDebugBehavior)_wcfFileEnumHost.Description.Behaviors[typeof(ServiceDebugBehavior)]).IncludeExceptionDetailInFaults = true;
            _wcfFileEnumHost.OpenTimeout = TimeSpan.FromSeconds(12);
            _wcfFileEnumHost.Closing += (sender, e) => Log.Info("_wcfFileEnumHost is closing.");
            _wcfFileEnumHost.Closed += (sender, e) => Log.Info("_wcfFileEnumHost is closed.");
            _wcfFileEnumHost.Faulted += (sender, e) => Log.Warn("_wcfFileEnumHost is faulted.");

            _wcfFileEnumHost.AddServiceEndpoint(
                typeof(IFileEnum),
                tcpBinding,
               "net.tcp://localhost:" + port + "/TracerX-Service/FileEnum");

            Log.Info("Opening the WCF ServiceHost for ImplFileEnum.");
            _wcfFileEnumHost.Open();
        }

        private static NetTcpBinding MakeBinding()
        {
            NetTcpBinding binding = new NetTcpBinding()
            {
                Name = "BufferedBinding",
                TransferMode = TransferMode.Buffered,
                MaxBufferSize = 32 * 1000 * 1000,
                MaxConnections = 25,
                MaxReceivedMessageSize = 32 * 1000 * 1000,
            };

            binding.ReaderQuotas.MaxDepth = 1000;
            binding.ReaderQuotas.MaxStringContentLength = 256 * 1000;
            binding.ReaderQuotas.MaxArrayLength = 256 * 1000;
            binding.ReaderQuotas.MaxBytesPerRead = 32 * 1000 * 1000;

            return binding;
        }

        private static void TryClose(ServiceHost host, string description, int timeoutSeconds)
        {
            try
            {
                if (host == null)
                {
                    Log.Warn(description, " host is null!");
                }
                else
                {
                    Log.Info(description, " state is ", host.State);

                    if (host.State == CommunicationState.Created ||
                        host.State == CommunicationState.Opened ||
                        host.State == CommunicationState.Opening)
                    {
                        Log.Info("Closing ", description, " whose state is ", host.State);
                        host.Close(TimeSpan.FromSeconds(timeoutSeconds));
                    }
                    else
                    {
                        Log.Info("NOT closing ", description, " because its state is ", host.State);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            finally
            {
                _sem.Release();
            }
        }
    }
}
  // The configuration was previously specified in the following XML.
  //  
  //<system.serviceModel>
  //  <bindings>
  //    <netTcpBinding>
  //      <binding name="BufferedBinding" transferMode="Buffered" maxBufferSize="32000000" maxConnections="25" maxReceivedMessageSize="32000000">
  //        <readerQuotas maxDepth="1000" maxStringContentLength="256000" maxArrayLength="256000" maxBytesPerRead="32000000"/>
  //      </binding>
  //    </netTcpBinding>
  //  </bindings>
  //  <services>
  //    <!-- The three services use the same port, but implement different contracts. -->

  //    <!-- Used by TracerX-Viewer to monitor a file on the server (watch for changes). -->
  //    <service name="TracerX.ImplFileMonitor" behaviorConfiguration="TracerXServiceBehavior">
  //      <host>
  //        <baseAddresses>
  //          <add baseAddress="net.tcp://localhost:25120/TracerX-Service/FileMonitor"/>
  //        </baseAddresses>
  //        <timeouts openTimeout="00:00:12"/>
  //      </host>
  //      <endpoint address="" binding="netTcpBinding" bindingConfiguration="BufferedBinding" contract="TracerX.IFileMonitor">
  //        <!--<identity>
  //          <dns value="localhost"/>
  //        </identity>-->
  //      </endpoint>
  //    </service>

  //    <!-- Used by TracerX-Viewer to enumerate files and folders on the server. -->
  //    <service name="TracerX.ImplFileEnum" behaviorConfiguration="TracerXServiceBehavior">
  //      <host>
  //        <baseAddresses>
  //          <add baseAddress="net.tcp://localhost:25120/TracerX-Service/FileEnum"/>
  //        </baseAddresses>
  //        <timeouts openTimeout="00:00:12"/>
  //      </host>
  //      <endpoint address="" binding="netTcpBinding" bindingConfiguration="BufferedBinding" contract="TracerX.IFileEnum">
  //        <!--<identity>
  //          <dns value="localhost"/>
  //        </identity>-->
  //      </endpoint>
  //    </service>

  //    <!-- Used by TracerX-Viewer to read a file from the server. -->
  //    <service name="TracerX.ImplFileReader" behaviorConfiguration="TracerXServiceBehavior">
  //      <host>
  //        <baseAddresses>
  //          <add baseAddress="net.tcp://localhost:25120/TracerX-Service/FileReader"/>
  //        </baseAddresses>
  //        <timeouts openTimeout="00:00:12"/>
  //      </host>
  //      <endpoint address="" binding="netTcpBinding" bindingConfiguration="BufferedBinding" contract="TracerX.IFileReader">
  //        <!--<identity>
  //          <dns value="localhost"/>
  //        </identity>-->
  //      </endpoint>
  //    </service>      
  //  </services>
    
  //  <behaviors>
  //    <serviceBehaviors>
  //      <behavior name="TracerXServiceBehavior">
  //        <serviceDebug includeExceptionDetailInFaults="true"/>
  //      </behavior>
  //    </serviceBehaviors>
  //  </behaviors>
  //</system.serviceModel>
