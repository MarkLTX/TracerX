using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;

namespace TracerX
{
    [ServiceContract]
    public interface IFileEnum
    {
        /// <summary>
        /// Allows client to exchange version number with server.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        int ExchangeVersion(int clientVersion);

        /// <summary>
        /// Allows client to determine the EXE hosting the server.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        void GetServiceHostInfo(out string executable, out string executableVersion, out string processUser);

        /// <summary>
        /// Allows client to determine the EXE hosting the server.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        void GetServiceHostInfo2(out string executable, out string executableVersion, out string processUser, out bool isImpersonatingClients);

        /// <summary>
        /// Gets "recently created" files and folders and data about "extra" files and folders.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        List<TXFileInfo> GetRecentFilesAndFolders(IEnumerable<string> extraFiles, IEnumerable<string> extraFolders);

        /// <summary>
        /// Returns the paths of the .tx1 files in the specified folder.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        List<TXFileInfo> GetFilesInFolder(string folder);

        /// <summary>
        /// Deletes the specified file, which must have a .tx1 extension.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        void DeleteFile(string filePath);

        /// <summary>
        /// Deletes [baseLogName].tx1 and all [baseLogName]_*.tx1 files from the specified folder. If
        /// the folder becomes empty and deleteEmptyFolderAndParents is true this will delete the
        /// folder and it's parent, grandparent, etc. (if they become empty).  Returns a list of the deleted files.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        List<string> DeleteAllRelated(string folderPath, string baseLogName, bool deleteEmptyFolderAndParents);

        /// <summary>
        /// Deletes files in folders that match the specified criteria.  The fileSpec and folderSpec may contain wild-cards.  
        /// If folderSpec contains a wild-card char ('*' or '?') only folders listed in the TracerX history files are considered.
        /// Otherwise the folderSpec is processed as-is.
        /// If deleteEmptyFolderAndParents is true then any folder that becomes empty due to
        /// deleting files will also be deleted. Parent folders will be deleted recursively if they
        /// become empty. Returns a list of the deleted files.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ExceptionDetail))]
        List<string> DeleteMany(string fileSpec, string folderSpec, DateTime minTimestamp, DateTime maxTimestamp, long minSize, long maxSize, bool tx1Files, bool txtFiles, bool deleteEmptyFolderAndParents, bool listOnly);

    }
}
