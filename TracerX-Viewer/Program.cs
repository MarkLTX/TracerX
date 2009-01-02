using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using TracerX.Viewer;

namespace TracerX.Viewer {
    class Program {
        [STAThread()]
        static void Main(string[] args) {
            //Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                Application.Run(new MainForm(args));
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            Debug.Print(e.ExceptionObject.ToString());
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
            Debug.Print(e.Exception.ToString());
        }


        public static string FormatTimeSpan(TimeSpan ts) {
            string raw = ts.ToString();
            int colon = raw.IndexOf(':');
            int period = raw.IndexOf('.', colon);

            if (period == -1) {
                return raw;
            } else if (period + 4 >= raw.Length) {
                return raw;
            } else {
                return raw.Remove(period + 4);
            }
        }

    }
}
