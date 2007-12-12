using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace BBS.TracerX.Viewer {
    public partial class About : Form {
        public About() {
            InitializeComponent();
            Assembly thisAsm = Assembly.GetExecutingAssembly();
            
            this.Icon = Properties.Resources.scroll_view;
            this.asmVer.Text = thisAsm.GetName().Version.ToString();
            this.loadDir.Text = Path.GetDirectoryName(Application.ExecutablePath);
            this.minFileVer.Text = Reader.MinFormatVersion.ToString();
            this.curFileVer.Text = Logger.FileLogging.FormatVersion.ToString();


        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Stream licStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BBS.TracerX.Apache_License.txt");
            StreamReader reader = new StreamReader(licStream);
            string lic = reader.ReadToEnd();
            reader.Close();

            FullText dlg = new FullText(lic);
            dlg.ShowDialog();
        }
    }
}