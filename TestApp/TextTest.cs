using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TracerX;
using System.Diagnostics;
using System.Threading;

namespace TestApp
{
    public partial class TextTest : Form
    {
        private Logger Log = Logger.GetLogger("TextTest");

        public TextTest()
        {
            InitializeComponent();

            cmboPolicy.DataSource = Enum.GetValues(typeof(FullFilePolicy));
            
            Log.TextFileTraceLevel = TracerX.TraceLevel.Verbose;
            
            Log.TextFile = new TextFile();
            Log.TextFile.Name = "TextTest";
            Log.TextFile.Directory = "%EXEDIR%";
        }

        private void SetProperties() 
        {
            Log.TextFile.Close();
            Log.BinaryFile.Close();                

            Log.TextFile.Use_00 = chkUse_00.Checked;
            Log.TextFile.FullFilePolicy = (FullFilePolicy)cmboPolicy.SelectedItem;
            Log.TextFile.UseKbForSize = chkUseKb.Checked;
            Log.TextFile.AppendIfSmallerThanMb = uint.Parse(txtAppendSize.Text);
            Log.TextFile.MaxSizeMb = uint.Parse(txtMaxSize.Text);
            Log.TextFile.CircularStartSizeKb = uint.Parse(txtCircularSize.Text);
            Log.TextFile.CircularStartDelaySeconds = 0;
            Log.TextFile.Archives = 3;
        }

        string GetPropertyState()
        {
            return string.Format("Use_00 = {0}, UseKbForSize = {1},  AppendIfSmallerThanMb = {2}, MaxSizeMb  = {3}, CircularStartSizeKb = {4}, FullFilePolicy = {5}", Log.TextFile.Use_00, Log.TextFile.UseKbForSize, Log.TextFile.AppendIfSmallerThanMb, Log.TextFile.MaxSizeMb, Log.TextFile.CircularStartSizeKb, Log.TextFile.FullFilePolicy);
        }

        private void cmboPolicy_SelectedIndexChanged(object sender, EventArgs e)
        {
            FullFilePolicy selected = (FullFilePolicy)cmboPolicy.SelectedItem;

            btnLogTilClosed.Enabled = selected == FullFilePolicy.Close || selected == FullFilePolicy.Roll;
            btnLogTilWrapped.Enabled = selected == FullFilePolicy.Wrap;
        }

        private void btnOpenAndClose_Click(object sender, EventArgs e)
        {
            SetProperties();
            Log.TextFile.Open();
            Log.TextFile.Close();
            MessageBox.Show("Created file\n" + Log.TextFile.FullPath);
        }

        private void btnLog1_Click(object sender, EventArgs e)
        {
            SetProperties();
            Log.TextFile.Open();
            Log.Info("'Log 1 Line' was clicked. ", GetPropertyState());
            Log.TextFile.Close();
            MessageBox.Show("Created file\n" + Log.TextFile.FullPath);
        }

        private void btnLogTilClosed_Click(object sender, EventArgs e)
        {
            bool isClosed = false;

            SetProperties();
            Log.TextFile.Closed += (object s, EventArgs evt) => isClosed = true;
            Log.TextFile.Open();

            Log.Info("'Log Until File Closes' was clicked. ", GetPropertyState());

            while (!isClosed)
            {
                Log.Debug("'Log Until File Closes' was clicked. File position before logging this line was ", Log.TextFile.CurrentPosition);
            }

            // This is not redundant!  File may have been automatically reopened/rolled.
            Log.TextFile.Close();
            MessageBox.Show("Created file\n" + Log.TextFile.FullPath);
        }

        private void btnLogTilWrapped_Click(object sender, EventArgs e)
        {
            SetProperties();
            Log.TextFile.Open();
            Log.Info("'Log Until File Wraps' was clicked. ", GetPropertyState());

            while (!Log.TextFile.Wrapped && Log.TextFile.IsOpen)
            {
                Log.Debug("'Log Until File Wraps' was clicked. File position before logging this line was ", Log.TextFile.CurrentPosition);
            }

            Log.TextFile.Close();
            MessageBox.Show("Created file\n" + Log.TextFile.FullPath);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Logger otherLog = Logger.GetLogger("Other Logger");

            if (otherLog.TextFile == Logger.DefaultTextFile) otherLog.TextFile = new TextFile();
            otherLog.TextFile.Name = "TextTestOther.txt";
            otherLog.TextFile.Directory = "%EXEDIR%";

            SetProperties();

            Log.TextFile.Open();
            otherLog.TextFile.Open();
            
            Log.Info("'Log Cals to 2 Files' was clicked. ", GetPropertyState());

            Log.TextFileTraceLevel = TracerX.TraceLevel.Verbose;
            otherLog.TextFileTraceLevel = TracerX.TraceLevel.Verbose;

            using (Log.InfoCall("Default Depth 1"))
            {
                otherLog.Info("This should not have a method, and be in ", otherLog.TextFile.Name);
                    
                Log.Info("About to 'call' a method whose entry should appear in 'other' file.");
                using (otherLog.InfoCall("Other Depth 1"))
                {
                    Thread.Sleep(10);

                    Log.Info("This should have method 'Default Depth 1', and be in ", Log.TextFile.Name);
                    otherLog.Info("About to 'call' a method whose entry should appear in 'default' file.");
                    using (Log.DebugCall("Default Depth 2"))
                    {
                        Log.Debug("About to 'call' a method whose entry should appear in 'other' file.");
                        using (otherLog.DebugCall("Other Depth 2"))
                        {
                            otherLog.Debug("Now exiting all calls.");
                        }
                    }
                    Log.Info("This should have method 'Default Depth 1', and be in ", Log.TextFile.Name);
                }
            }

            otherLog.Info("All calls have exited.");
            Log.Info("All calls have exited.");

            Log.TextFile.Close();
            otherLog.TextFile.Close();

            MessageBox.Show("Created file\n" + Log.TextFile.FullPath + "\nand\n" + otherLog.TextFile.FullPath);

        }
    }
}
