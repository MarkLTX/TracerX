using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace TracerX.Viewer
{
    internal partial class License : Form
    {
        public License()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Stream licStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TracerX.NotTheLicense.txt");
            StreamReader reader = new StreamReader(licStream);
            string lic = reader.ReadToEnd();
            reader.Close();

            FullText dlg = new FullText(lic);
            dlg.ShowDialog();
        }
    }
}