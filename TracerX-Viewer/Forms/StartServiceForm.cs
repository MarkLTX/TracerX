using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TracerX;

namespace TracerX
{
    public partial class StartServiceForm : Form
    {
        public StartServiceForm()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            int port = 25120;

            if (radSpecifiedPort.Checked)
            {
                if (int.TryParse(txtPort.Text, out port))
                {
                    if (port < 0 || port > 65535)
                    {
                        MainForm.ShowMessageBox("The port must be in the range 0 - 65535.");
                        port = -1;
                    }
                }
                else
                {
                    MainForm.ShowMessageBox("The port must be a number in the range 0 - 65535.");
                    port = -1;
                }
            }

            if (port != -1)
            {
                try
                {
                    TracerXServices.Startup(port, radDoImpersonate.Checked);
                    DialogResult = DialogResult.OK;
                }
                catch (System.ServiceModel.AddressAlreadyInUseException)
                {
                    // Another process is using the port.  Check if it's the TracerX WCF service.

                    TryConnecting(port);

                }
                catch (Exception ex)
                {
                    MainForm.ShowMessageBox("Could not start the TracerX service due to error: " + ex.Message);
                }
            }
        }

        // Determines if the specified port is in use by the TracerX service by attempting to connect to it.
        private static void TryConnecting(int port)
        {
            try
            {
                using (ProxyFileEnum serviceProxy = new ProxyFileEnum())
                {
                    serviceProxy.SetHost("localhost:" + port);
                    int serviceInterfaceVersion = serviceProxy.ExchangeVersion(1);

                    // Getting here without an exception means the TracerX service is listening on the port.
                    // Depending on the version, we may be able to get additional info.

                    if (serviceInterfaceVersion < 3)
                    {
                        // That's all we can get (the interface version).
                        MainForm.ShowMessageBox("The specified port is in use by another process that's running the TracerX service.");
                    }
                    else
                    {
                        string processExe;
                        string processVersion;
                        string processAccount;

                        if (serviceInterfaceVersion < 5)
                        {
                            serviceProxy.GetServiceHostInfo(out processExe, out processVersion, out processAccount);
                        }
                        else
                        {
                            // As of version 5, we can get whether the service is impersonating 
                            // clients or not with GetServiceHostInfo2, but we don't need to.
                            serviceProxy.GetServiceHostInfo(out processExe, out processVersion, out processAccount);
                        }

                        string msg = "The specified port is in use by another process that's running the TracerX service.";

                        if (processExe != null)
                        {
                            msg += "\n\nExecutable: " + processExe;

                            if (processVersion != null)
                            {
                                msg += " (version " + processVersion + ")";
                            }
                        }

                        if (processAccount != null)
                        {
                            msg += "\n\nAccount: " + processAccount;
                        }

                        MainForm.ShowMessageBox(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                // Assume this means the process using the port is not TracerX.
                MainForm.ShowMessageBox("The specified port is in use by another process.");
            }
        }
    }
}
