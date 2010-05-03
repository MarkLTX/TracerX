using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TracerX;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace TestApp {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }


        private DomainCallbacks _domCallbacks = new DomainCallbacks();

        private void viewerBtn_Click(object sender, EventArgs e) {
            //string curdir = Directory.GetCurrentDirectory();
            Process.Start("..\\..\\..\\TracerX-Viewer\\bin\\debug\\TracerX-Viewer.exe");
        }

        private void emptyBtn_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("Empty");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.Empty));
            AppDomain.Unload(otherDomain);
        }
       

        private void oneLineBtn_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("OneLine");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.OneLine));
            AppDomain.Unload(otherDomain);
        }

        private void button1_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("FiveThreadsNonCircular");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.FiveThreadsNonCircular));
            AppDomain.Unload(otherDomain);
        }

        private void button2_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("StopAtCircular");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.StopAtCircular));
            AppDomain.Unload(otherDomain);
        }

        private void button3_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("CircularWith1Block");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.CircularWith1Block));
            AppDomain.Unload(otherDomain);
        }

        private void button4_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("OneThreadWraps");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.OneThreadWraps));
            AppDomain.Unload(otherDomain);
        }

        private void button5_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("FiveThreadsWrap");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.FiveThreadsWrap));
            AppDomain.Unload(otherDomain);
        }

        private void button6_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("StartCircularByTime");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.StartCircularByTime));
            AppDomain.Unload(otherDomain);
        }

        private void button7_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("MissingMethodEntry");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.MissingMethodEntry));
            AppDomain.Unload(otherDomain);
        }

        private void button8_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("MissingMethodExit");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.MissingMethodExit));
            AppDomain.Unload(otherDomain);
        }

        private void button9_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("MissingEntriesAndExits");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.MissingEntriesAndExits));
            AppDomain.Unload(otherDomain);
        }

        private void button10_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("AllUnicodeChars");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.AllUnicodeChars));
            AppDomain.Unload(otherDomain);
        }

        private void button11_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("MoreThanUintMaxLines");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.MoreThanUintMaxLines));
            AppDomain.Unload(otherDomain);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            AppDomain otherDomain = AppDomain.CreateDomain("ControlledLogging");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.ControlledLogging));
            AppDomain.Unload(otherDomain);
        }

        private void button13_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            AppDomain otherDomain = AppDomain.CreateDomain("Time1MillionWithWrapping");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.Time1MillionWithWrapping));
            AppDomain.Unload(otherDomain);
            Cursor = Cursors.Default;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            AppDomain otherDomain = AppDomain.CreateDomain("Time1MillionWithOutWrapping");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.Time1MillionWithOutWrapping));
            AppDomain.Unload(otherDomain);
            Cursor = Cursors.Default;
        }

        private void button15_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            AppDomain otherDomain = AppDomain.CreateDomain("RandomCalls");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.RandomCall));
            AppDomain.Unload(otherDomain);
            Cursor = Cursors.Default;
        }

        private void createAppendBtn_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            SetAppendMode();
            AppDomain otherDomain = AppDomain.CreateDomain("CreateAppend");

            if (textFileChk.Checked) {
                otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.CreateTextFileForAppending));
            } else {
                otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.CreateFileForAppending));
            }

            AppDomain.Unload(otherDomain);
            Cursor = Cursors.Default;
        }

        private void appendToFileBtn_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            SetAppendMode();
            AppDomain otherDomain = AppDomain.CreateDomain("AppendToFile");

            if (textFileChk.Checked) {
                otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.AppendToTextFile));
            } else {
                otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.AppendToFile));
            } 

            AppDomain.Unload(otherDomain);
            Cursor = Cursors.Default;
        }

        private void SetAppendMode() {
            if (emptyRad.Checked) _domCallbacks.AppendTestMode = DomainCallbacks.AppendMode.Empty;
            if (noCircularRad.Checked) _domCallbacks.AppendTestMode = DomainCallbacks.AppendMode.NoCircular;
            if (noWrapRad.Checked) _domCallbacks.AppendTestMode = DomainCallbacks.AppendMode.CircularNoWrap;
            if (circularWrapsRad.Checked) _domCallbacks.AppendTestMode = DomainCallbacks.AppendMode.CircularWrap;
            if (exceedMaxMbRad.Checked) _domCallbacks.AppendTestMode = DomainCallbacks.AppendMode.ExceedMaxMb;

            _domCallbacks.MaxAppendableFileMb = uint.Parse(maxAppendSizeBox.Text);
            _domCallbacks.MaxSessionSizeMb = uint.Parse(maxSessionSizeBox.Text);
            _domCallbacks.CircularStartKb = uint.Parse(circularStartSizeBox.Text);
        }

        private void maxAppendSizeBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void textWriterTraceListenerBtn_Click(object sender, EventArgs e)
        {
            AppDomain otherDomain = AppDomain.CreateDomain("TextWriter");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.TextWriter));
            AppDomain.Unload(otherDomain);
        }

        private void readConfigBtn_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("ReadConfig");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.LoadConfig));
            AppDomain.Unload(otherDomain);
        }

    }
}
