using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TracerX.Viewer {
    internal partial class FileProperties : Form {
        public FileProperties() {
            InitializeComponent();
            this.Icon = Properties.Resources.scroll_view;
        }

        public FileProperties(Reader reader) {
            InitializeComponent();
            this.Icon = Properties.Resources.scroll_view;

            listView1.Items.Add(new ListViewItem(new string[] { "Creation time (UTC)", reader.OpenTimeUtc.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Creation time (logger's TZ)", reader.Log_CreationTimeInAppsTZ }));
            listView1.Items.Add(new ListViewItem(new string[] { "Creation time (local TZ)", reader.Log_CreationTimeInViewersTZ.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Loggers assembly version", reader.Logger_AssemblyVersion }));
            listView1.Items.Add(new ListViewItem(new string[] { "Format version", reader.FormatVersion.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Max size (MB)", reader.MaxMb.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Size (bytes)", reader.Size.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Elapsed time", reader.File_Timespan.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Last record number", reader.Records_LastNumber.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Last timestamp", reader.Records_LastTimestamp.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Circular logging started", reader.InCircularPart.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Records lost by wrapping", reader.Records_LostViaWrapping.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Record count", reader.RecordsRead.ToString() }));

            nameCol.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            valueCol.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
    }
}