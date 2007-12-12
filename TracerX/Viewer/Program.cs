using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using BBS.TracerX.Viewer;

namespace BBS.TracerX.Viewer {
    class Program {
        [STAThread()]
        static void Main(string[] args) {
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            try {
                Application.Run(new MainForm(args));
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            Debug.Print(e.ExceptionObject.ToString());
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
            Debug.Print(e.Exception.ToString());
        }
    }
}
