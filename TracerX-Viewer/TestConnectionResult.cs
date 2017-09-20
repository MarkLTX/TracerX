using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TracerX
{
    class TestConnectionResult
    {
        public string HostAndPort;
        public Exception Exception;
        public int ServiceVersion;
        public string HostExe;
        public string HostAccount;
        public string HostVersion;
        public bool? IsImpersonatingClients;

        public void Show()
        {
            string msg = "";

            if (Exception == null)
            {
                msg = "Connection to " + HostAndPort + " succeeded.";

                if (HostExe != null)
                {
                    msg += "\n\nServiceExecutable: " + HostExe;

                    if (HostVersion != null)
                    {
                        msg += " (version " + HostVersion + ")";
                    }
                }

                if (HostAccount != null)
                {
                    msg += "\n\nService Account: " + HostAccount;
                }

                if (IsImpersonatingClients.HasValue)
                {
                    if (IsImpersonatingClients.Value)
                    {
                        msg += "\n\nClient impersonation is enabled.";
                    }
                    else
                    {
                        msg += "\n\nClient impersonation is disabled.";
                    }
                }
                else
                {
                    msg += "\n\nClient impersonation is not supported.";
                }
            }
            else
            {
                msg = "Connection to " + HostAndPort + " failed!";
                Exception ex = Exception;

                while (ex != null)
                {
                    msg += "\n\n" + ex.Message;
                    ex = ex.InnerException;
                }
            }

            MainForm.ShowMessageBox(msg);
        }
    }
}
