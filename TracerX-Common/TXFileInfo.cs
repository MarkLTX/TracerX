using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TracerX
{
    /// <summary>
    /// Basic info about a file or folder
    /// </summary>
    [DataContract]
    public class TXFileInfo
    {
        [DataMember]
        public bool IsFolder;

        [DataMember]
        public string FullPath;

        [DataMember]
        public DateTime Created;

        [DataMember]
        public DateTime LastModified;

        /// <summary>
        /// When returned from a remote server, indicates if this file path was found
        /// in the RecentlyCreated file.  Does not apply to folders.
        /// </summary>
        [DataMember]
        public bool FoundInRecentFiles;

        [DataMember]
        public long Size = -1;

        public override string ToString()
        {
            return FullPath;
        }
    }
}
