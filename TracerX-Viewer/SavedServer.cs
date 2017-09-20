using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TracerX
{
    /// <summary>
    /// A collection of these objects is serialized in the user's saved settings to save the list of remote servers.
    /// </summary>
    public class SavedServer
    {
        public string HostAddress { get; set; }
        public int Port { get; set; }
        public string Category { get; set; }
        public string UserId { get; set; }
        public string PW { get; set; } // encrypted
        
        public List<ViewedPath> ViewedFiles { get; set; }
        public List<ViewedPath> ViewedFolders { get; set; }

        private string _hostName;

        public string HostName
        {
            get { return _hostName; }
            
            set
            {
                // We may be deserializing an older version of this class that was serialized
                // before HostAddress existed.  What used to be stored in HostName now
                // belongs in HostAddress.

                _hostName = value;

                if (HostAddress == null)
                {
                    HostAddress = value;
                }
            }
        }

        /// <summary>
        /// Used in the "Address" column of the ServerListEditor.
        /// </summary>
        public string HostAndPort
        {
            get
            {
                if (Port > 0)
                {
                    // The user  explicitly specified a port so we should display it.
                    return HostAddress + ":" + Port.ToString();
                }
                else
                {
                    return HostAddress;
                }
            }
        }

        public string Domain
        {
            get 
            {
                int ndx = HostAddress.IndexOf('.') + 1;

                if (ndx == 0 || ndx >= HostAddress.Length)
                {
                    return "";
                }
                else
                {
                    return HostAddress.Substring(ndx);
                }
            }
        }

    }

    public class ViewedPath
    {
        public string Path { get; set; }
        public DateTime ViewTime { get; set; }
    }
}
