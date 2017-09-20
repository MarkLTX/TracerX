using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Net;

namespace TracerX
{
    public class ProxyFileMonitor : DuplexClientBaseEx<IFileMonitor>, IFileMonitor
    {
        public ProxyFileMonitor(IFileMonitorCallback callback)
            :base(callback)
        {
        }

        #region IFileMonitor Members

        public int ExchangeVersion(int clientVersion)
        {
            return base.Channel.ExchangeVersion(clientVersion);
        }

        public void StartWatching(string filePath)
        {
            base.Channel.StartWatching(filePath);
        }

        public void StartWatching2(string filePath, Guid guidForEvents)
        {
            base.Channel.StartWatching2(filePath, guidForEvents);
        }

        public void StopWatching()
        {
            base.Channel.StopWatching();
        }

        #endregion
    }
}
