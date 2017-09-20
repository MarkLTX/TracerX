using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;

namespace TracerX
{
    public class ProxyFileReader : ClientBaseEx<IFileReader>, IFileReader, IDisposable
    {
        public int ExchangeVersion(int clientVersion)
        {
            return base.Channel.ExchangeVersion(clientVersion);
        }

        public void OpenFile(string file)
        {
            base.Channel.OpenFile(file);
        }

        public void CloseFile()
        {
            base.Channel.CloseFile();
        }

        public byte[] ReadBytes(long position, int count, out long fileLength)
        {
            return base.Channel.ReadBytes(position, count, out fileLength);
        }

        public long GetLength()
        {
            return base.Channel.GetLength();
        }

        public void SetPosition(long pos)
        {
            base.Channel.SetPosition(pos);
        }
    }
}
