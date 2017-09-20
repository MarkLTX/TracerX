using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Net;
using System.IO;

namespace TracerX
{
    public class ProxyFileEnum : ClientBaseEx<IFileEnum>, IFileEnum, IDisposable
    {
        public int ExchangeVersion(int clientVersion)
        {
            return base.Channel.ExchangeVersion(clientVersion);
        }

        public void GetServiceHostInfo(out string executable, out string executableVersion, out string processUser)
        {
            base.Channel.GetServiceHostInfo(out executable, out executableVersion, out processUser);
        }

        // Added in interface version 5 (returned by ExchangeVersion)
        public void GetServiceHostInfo2(out string executable, out string executableVersion, out string processUser, out bool isImpersonatingClients)
        {
            base.Channel.GetServiceHostInfo2(out executable, out executableVersion, out processUser, out isImpersonatingClients);
        }

        public List<TXFileInfo> GetRecentFilesAndFolders(IEnumerable<string> extraFiles, IEnumerable<string> extraFolders)
        {
            return base.Channel.GetRecentFilesAndFolders(extraFiles, extraFolders);
        }

        public List<TXFileInfo> GetFilesInFolder(string folder)
        {
            return base.Channel.GetFilesInFolder(folder);
        }

        public void DeleteFile(string filePath)
        {
            base.Channel.DeleteFile(filePath);
        }

        public List<string> DeleteAllRelated(string folderPath, string baseLogName, bool deleteEmptyFolderAndParents)
        {
            return base.Channel.DeleteAllRelated(folderPath, baseLogName, deleteEmptyFolderAndParents);
        }

        public List<string> DeleteMany(string fileSpec, string folderSpec, DateTime minTimestamp, DateTime maxTimestamp, long minSize, long maxSize, bool tx1Files, bool txtFiles, bool deleteEmptyFolderAndParents, bool listOnly)
        {
            return base.Channel.DeleteMany(fileSpec, folderSpec, minTimestamp, maxTimestamp, minSize, maxSize, tx1Files, txtFiles, deleteEmptyFolderAndParents, listOnly);
        }

    }

}
