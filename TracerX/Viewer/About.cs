using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace TracerX.Viewer {
    internal partial class About : Form {
        public About() {
            InitializeComponent();
            Assembly thisAsm = Assembly.GetExecutingAssembly();
            
            this.Icon = Properties.Resources.scroll_view;
            this.asmVer.Text = thisAsm.GetName().Version.ToString();
            this.loadDir.Text = Path.GetDirectoryName(Application.ExecutablePath);
            this.minFileVer.Text = Reader.MinFormatVersion.ToString();
            this.curFileVer.Text = Logger.FileLogging.FormatVersion.ToString();


        }
    }
}