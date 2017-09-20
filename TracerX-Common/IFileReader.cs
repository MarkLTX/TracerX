using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace TracerX
{
    [ServiceContract]
    public interface IFileReader
    {
        /// <summary>
        /// Allows client to exchange version number with server.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        int ExchangeVersion(int clientVersion);
        
        /// <summary>
        /// Opens the file for reading.  Only one file can be opened at
        /// once in a given session.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        void OpenFile(string file);

        /// <summary>
        /// Closes the currently open file if there is one.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        void CloseFile();

        /// <summary>
        /// Reads up to the specified number of bytes from the file, starting at the
        /// specified position.  May return fewer bytes than requested.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        byte[] ReadBytes(long position, int count, out long fileLength);

        /// <summary>
        /// Gets the file size.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        long GetLength();

        /// <summary>
        /// Sets the seek position.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        void SetPosition(long pos);
    }
}
