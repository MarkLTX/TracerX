using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TracerX;
using System.Threading;
using System.IO;

namespace TestApp
{
    public partial class ControlledLogging : Form
    {
        private static readonly Logger Log = Logger.GetLogger("ControlledLogging");

        private System.Threading.Timer _timer;
        private int _tickCount;
        private uint _nearMaxSize = (Logger.DefaultBinaryFile.MaxSizeMb << 20) - 1000;

        private WorkerData _workerData1 = new WorkerData("Worker1");
        private WorkerData _workerData2 = new WorkerData("Worker2");

        private FileInfo _fileInfo;

        private class WorkerData
        {
            public WorkerData(string name)
            {
                Name = name;
            }

            public string Name;
            public AutoResetEvent Event = new AutoResetEvent(false);
            public int TargetDepth = 0;
            public int CurrentDepth = 0;
        }

        public ControlledLogging()
        {
            InitializeComponent();

            Log.BinaryFileTraceLevel = TraceLevel.Verbose;

            // Start the timer used to log a line every N milliseconds.
            _timer = new System.Threading.Timer(new TimerCallback(TimerTick));

            _fileInfo = new FileInfo(Logger.DefaultBinaryFile.FullPath);
            UpdateStats();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _timer.Dispose();
        }

        private void UpdateStats()
        {
            if (InvokeRequired)
            {
                if (!IsDisposed)
                {
                    Action del = UpdateStats;
                    try
                    {
                        Invoke(del);
                    }
                    catch
                    {
                        // Sometimes get ObjectDisposedException
                    }
                }

                return;
            }

            circularBtn.Enabled = !Logger.DefaultBinaryFile.CircularStarted;
            wrapBtn.Enabled = Logger.DefaultBinaryFile.CircularStarted;
            wrap2Btn.Enabled = Logger.DefaultBinaryFile.CircularStarted;
            button2.Enabled = Logger.DefaultBinaryFile.CurrentPosition < _nearMaxSize;

            _fileInfo.Refresh();
            lastWriteBox.Text = _fileInfo.LastWriteTime.ToLongTimeString();
            sizeBox.Text = Logger.DefaultBinaryFile.CurrentSize.ToString("N0");
            positionBox.Text = Logger.DefaultBinaryFile.CurrentPosition.ToString("N0");
            circularBox.Text = Logger.DefaultBinaryFile.CircularStarted.ToString();
            wrappedBox.Text = Logger.DefaultBinaryFile.Wrapped.ToString();
            blockBox.Text = Logger.DefaultBinaryFile.CurrentBlock.ToString();
        }

        void TimerTick(object o)
        {
            Log.Info("Timer tick number ", ++_tickCount);
            UpdateStats();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Log.WarnFormat("Size = {0:N0}, Position = {1:N0}, InCircular = {2}, Wrapped = {3}", Logger.DefaultBinaryFile.CurrentSize, Logger.DefaultBinaryFile.CurrentPosition, Logger.DefaultBinaryFile.CircularStarted, Logger.DefaultBinaryFile.Wrapped);
            UpdateStats();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = !checkBox1.Checked;

            if (checkBox1.Checked)
            {
                int ms;

                if (int.TryParse(textBox1.Text, out ms))
                {
                    if (ms < 10)
                    {
                        MessageBox.Show("The minumum value is 10ms.");
                        checkBox1.Checked = false;
                    }
                    else
                    {
                        _timer.Change(ms, ms);
                    }
                }
                else
                {
                    MessageBox.Show("Could not parse string to an int.");
                    checkBox1.Checked = false;
                }
            }
            else
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void circularBtn_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            Logger.DefaultBinaryFile.CircularStartDelaySeconds = 1;
            Thread.Sleep(1000);
            Log.Warn("Starting circular.");
            UpdateStats();
            Cursor = Cursors.Default;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            while (Logger.DefaultBinaryFile.CurrentPosition < _nearMaxSize)
            {
                Log.WarnFormat("Size = {0}, Position = {1}, InCircular = {2}, wrapped = {3}", Logger.DefaultBinaryFile.CurrentSize, Logger.DefaultBinaryFile.CurrentPosition, Logger.DefaultBinaryFile.CircularStarted, Logger.DefaultBinaryFile.Wrapped);
            }

            UpdateStats();

            Cursor = Cursors.Default;
        }

        // This function runs in two worker threads and increases/decreases its
        // stack depth by calling itself recursively or returning when the 
        // corresponding buttons are clicked.
        private void WorkerFunc(object o)
        {
            var data = (WorkerData)o;

            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = data.Name;

            while (data.CurrentDepth <= data.TargetDepth)
            {
                if (data.CurrentDepth == data.TargetDepth)
                {
                    // Wait for target depth to change, then
                    // go deeper or return.
                    Log.Debug("Waiting at depth ", data.CurrentDepth);
                    UpdateStats();
                    data.Event.WaitOne();
                }

                if (data.CurrentDepth < data.TargetDepth)
                {
                    using (Log.DebugCall(data.Name))
                    {
                        ++data.CurrentDepth;
                        WorkerFunc(data);
                        --data.CurrentDepth;
                    }
                }
            }

        }

        // Causes worker1 to increase stack depth by 1.
        private void depth1Plus_Click(object sender, EventArgs e)
        {
            ++_workerData1.TargetDepth;
            _workerData1.Event.Set();
        }

        // Causes worker1 to decrease stack depth by 1.
        private void depth1Minus_Click(object sender, EventArgs e)
        {
            --_workerData1.TargetDepth;
            _workerData1.Event.Set();
        }

        // Causes worker2 to increase stack depth by 1.
        private void depth2Plus_Click(object sender, EventArgs e)
        {
            ++_workerData2.TargetDepth;
            _workerData2.Event.Set();
        }

        // Causes worker2 to decrease stack depth by 1.
        private void depth2Minus_Click(object sender, EventArgs e)
        {
            --_workerData2.TargetDepth;
            _workerData2.Event.Set();
        }

        private void wrapBtn_Click(object sender, EventArgs e)
        {
            uint startPos = Logger.DefaultBinaryFile.CurrentPosition;

            Cursor = Cursors.WaitCursor;

            while (Logger.DefaultBinaryFile.CurrentPosition >= startPos)
            {
                Log.InfoFormat("Size = {0}, Position = {1}, InCircular = {2}, wrapped = {3}", Logger.DefaultBinaryFile.CurrentSize, Logger.DefaultBinaryFile.CurrentPosition, Logger.DefaultBinaryFile.CircularStarted, Logger.DefaultBinaryFile.Wrapped);
            }

            Log.Info("Now back at beginning of circular part.");

            while (Logger.DefaultBinaryFile.CurrentPosition < startPos)
            {
                Log.InfoFormat("Size = {0}, Position = {1}, InCircular = {2}, wrapped = {3}", Logger.DefaultBinaryFile.CurrentSize, Logger.DefaultBinaryFile.CurrentPosition, Logger.DefaultBinaryFile.CircularStarted, Logger.DefaultBinaryFile.Wrapped);
            }

            Log.Info("Starting position now overwritten.");
            UpdateStats();
            Cursor = Cursors.Default;
        }

        private void wrap2Btn_Click(object sender, EventArgs e)
        {
            uint startPos = Logger.DefaultBinaryFile.CurrentPosition;

            Cursor = Cursors.WaitCursor;

            while (Logger.DefaultBinaryFile.CurrentPosition >= startPos)
            {
                Log.InfoFormat("Size = {0}, Position = {1}, InCircular = {2}, wrapped = {3}", Logger.DefaultBinaryFile.CurrentSize, Logger.DefaultBinaryFile.CurrentPosition, Logger.DefaultBinaryFile.CircularStarted, Logger.DefaultBinaryFile.Wrapped);
            }

            Log.Info("Now back at beginning of circular part.");
            UpdateStats();
            Cursor = Cursors.Default;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Start the two threads whose stack depths are controlled via buttons.
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkerFunc), _workerData1);
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkerFunc), _workerData2);

            button3.Enabled = false;
            depth1Minus.Enabled = true;
            depth1Plus.Enabled = true;
            depth2Minus.Enabled = true;
            depth2Plus.Enabled = true;

            Thread.Sleep(250);
            UpdateStats();
        }

        private void btnLog10_Click(object sender, EventArgs e)
        {
            for (int i = 1; i <= 10; ++i)
            {
                Log.Debug("Blasting line ", i, " of 10");
            }

            UpdateStats();
        }
    }
}
