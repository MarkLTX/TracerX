using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;

namespace TracerX
{
    /// <summary>
    /// Allows the client to monitor a file on the server for changes and renames, 
    /// which are reported via the CallbackContract IFileMonitorCallback.
    /// </summary>
    [ServiceContract(CallbackContract=typeof(IFileMonitorCallback), SessionMode= SessionMode.Required)]
    public interface IFileMonitor
    {
        /// <summary>
        /// Allows client to exchange version number with server.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        int ExchangeVersion(int clientVersion);

        /// <summary>
        /// Stops watching the current file, if any, and starts watching the specified file.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        void StartWatching(string filePath);

        /// <summary>
        /// Stops watching the current file, if any, and starts watching the specified file.
        /// Tries to set up event-based file change notification using the specified Guid
        /// as the basis of the names of named system events used for signaling between
        /// the logger and LocalFileWatcher that watches for changes.
        /// Added in version 3 (the version number returned by IFileReader.ExchangeVersion()).
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        void StartWatching2(string filePath, Guid guidForEvents);

        /// <summary>
        /// Stops watching the current file, if any.
        /// </summary>
        [OperationContract]
        //[FaultContract(typeof(ServiceExceptionFault))]
        void StopWatching();
    }

    public interface IFileMonitorCallback
    {
        /// <summary>
        /// Called by the server to notify the client that the file has changed.
        /// </summary>
        [OperationContract(IsOneWay = true)] // IsOneWay somehow prevents 5-10 second delay the first time this is called.
        void FileChanged(WatcherChangeTypes changeType, string path);

        /// <summary>
        /// Called by the server to notify the client that the file has been renamed.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void FileRenamed(WatcherChangeTypes changeType, string path, string oldPath);
    }
}
