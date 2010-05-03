using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TracerX.Viewer {
    internal partial class FileProperties : Form {
        private Reader _reader;

        public FileProperties() {
            InitializeComponent();
            this.Icon = Properties.Resources.scroll_view;
        }

        public FileProperties(Reader reader) {
            InitializeComponent();

            this.Icon = Properties.Resources.scroll_view;
            _reader = reader;

            commonListView.Items.Add(new ListViewItem(new string[] { "Location", Path.GetDirectoryName(reader.FileName) }));
            commonListView.Items.Add(new ListViewItem(new string[] { "Name", Path.GetFileName(reader.FileName) }));
            commonListView.Items.Add(new ListViewItem(new string[] { "Format version", reader.FormatVersion.ToString() }));
            commonListView.Items.Add(new ListViewItem(new string[] { "Size (bytes)", reader.Size.ToString("N0") }));

            commonNameCol.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            commonValueCol.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

            if (SessionObjects.AllSessionObjects.Count == 0) {
                sessionCombo.Enabled = false;
            } else {
                sessionCombo.Items.AddRange(SessionObjects.AllSessionObjects.ToArray());
                sessionCombo.SelectedIndex = 0;
            }
        }

        private static string ToLocalTZ(DateTime utc)
        {
            var local = utc.ToLocalTime();

            var localTZ = TimeZone.CurrentTimeZone.StandardName;

            if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(local))
            {
                localTZ = TimeZone.CurrentTimeZone.DaylightName;
            }

            return local.ToString() + " " + localTZ;
        }

        private void sessionCombo_SelectedIndexChanged(object sender, EventArgs e) {
            Reader.Session session = (Reader.Session)sessionCombo.SelectedItem;
            long sessionSize = 0;
            double sessionPercent = 0;

            if (session == SessionObjects.AllSessionObjects.Last()) {
                sessionSize = _reader.Size - session.SessionStartPos;
            } else {
                Reader.Session nextSession = SessionObjects.AllSessionObjects[session.Index + 1];
                sessionSize = nextSession.SessionStartPos - session.SessionStartPos;
            }

            sessionPercent = (double)sessionSize / (double)_reader.Size;
            string sizeMsg = string.Format("{0:N0} ({1:P1})", sessionSize, sessionPercent);

            sessionListView.Items.Clear();

            sessionListView.Items.Add(new ListViewItem(new string[] { "Creation time (UTC)", session.CreationTimeUtc.ToString() + " UTC" }));
            sessionListView.Items.Add(new ListViewItem(new string[] { "Creation time (logger's TZ)", session.CreationTimeLoggersTZ.ToString() + " " + session.LoggersTimeZone }));
            sessionListView.Items.Add(new ListViewItem(new string[] { "Creation time (local TZ)", ToLocalTZ(session.CreationTimeUtc) }));
            sessionListView.Items.Add(new ListViewItem(new string[] { "Last timestamp", ToLocalTZ(session.LastRecordTimeUtc) }));
            sessionListView.Items.Add(new ListViewItem(new string[] { "Elapsed time", (session.LastRecordTimeUtc - session.CreationTimeUtc).ToString() }));
            sessionListView.Items.Add(new ListViewItem(new string[] { "Circular logging started", session.InCircularPart.ToString() }));
            sessionListView.Items.Add(new ListViewItem(new string[] { "Last record number", session.LastRecordNum.ToString("N0") }));
            sessionListView.Items.Add(new ListViewItem(new string[] { "Record count", session.RecordsRead.ToString("N0") }));
            sessionListView.Items.Add(new ListViewItem(new string[] { "Records lost by wrapping", (session.LastRecordNum - session.RecordsRead).ToString("N0") }));
            sessionListView.Items.Add(new ListViewItem(new string[] { "Max size (MB)", session.MaxMb.ToString("N0") }));
            sessionListView.Items.Add(new ListViewItem(new string[] { "Size (bytes)", sizeMsg}));
            sessionListView.Items.Add(new ListViewItem(new string[] { "Loggers assembly version", session.LoggersAssemblyVersion }));
            sessionListView.Items.Add(new ListViewItem(new string[] { "GUID", session.FileGuid.ToString() }));

            sessionNameCol.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            sessionValueCol.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

        }
    }
}