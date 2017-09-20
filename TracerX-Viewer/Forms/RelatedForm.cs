using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TracerX
{
    /// <summary>
    /// Form for convenient access to files and folders related to the TracerX viewer and logger.
    /// </summary>
    public partial class RelatedForm : Form
    {
        public RelatedForm()
        {
            InitializeComponent();
            
            // The "data" directory, shared by all users, where the TracerX
            // logger puts the two files containing the recently created files
            // and recently used folders.

            linkProgramData.Text = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "TracerX\\"
                );

            // The data directory for the current user.

            linkLocalAppData.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TracerX\\");

            lblCurUser.Text = string.Format(lblCurUser.Text, SystemInformation.UserName);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            lblAllUsers.Left = linkProgramData.Right + 4;
            lblCurUser.Left = linkLocalAppData.Right + 4;
        }
        private void linkProgramData_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // The link's Text property contains the path of the folder to open in Windows Explorer.
            Process.Start(linkProgramData.Text);
        }

        private void linkRecentlyCreated_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string filePath = Path.Combine(linkProgramData.Text, "RecentlyCreated.txt");
            Process.Start(filePath);
        }

        private void linkRecentFolders_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string filePath = Path.Combine(linkProgramData.Text, "RecentFolders.txt");
            Process.Start(filePath);
        }

        private void linkLocalAppData_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // The link's Text property contains the path of the folder to open in Windows Explorer.
            Process.Start(linkLocalAppData.Text);
        }

        private void linkEvents_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string filePath = Path.Combine(linkLocalAppData.Text, "TracerXEvents.txt");
            Process.Start(filePath);
        }
    }
}
