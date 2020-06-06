using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace TracerX
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        Logger Log = Logger.GetLogger("ServiceObject");

        protected override void OnStart(string[] args)
        {
            using (Log.InfoCall())
            {
                try
                {
                    Program.Start();
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex);
                    throw;
                }
            }
        }

        protected override void OnStop()
        {
            using (Log.InfoCall())
            {
                Program.Stop();
            }
        }
    }
}
