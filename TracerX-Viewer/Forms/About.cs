using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace TracerX
{
    internal partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            Assembly thisAsm = Assembly.GetExecutingAssembly();

            this.Icon = Properties.Resources.scroll_view;
            this.asmVer.Text = thisAsm.GetName().Version.ToString();
            this.loadDir.Text = Path.GetDirectoryName(Application.ExecutablePath);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/MarkLTX/TracerX");
        }
    }
}