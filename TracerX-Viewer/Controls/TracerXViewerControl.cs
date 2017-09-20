using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TracerX
{
    /// <summary>
    /// This user control displays an instance of the TracerX-Viewer main form, allowing it to be hosted
    /// on other forms.
    /// </summary>
    public partial class TracerXViewerControl : UserControl
    {
        private MainForm _form;

        public TracerXViewerControl()
        {
            InitializeComponent();

            _form = new MainForm();

            _form.TopLevel = false;
            _form.Dock = DockStyle.Fill;
            _form.FormBorderStyle = FormBorderStyle.None;
            AutoScrollMinSize = _form.MinimumSize;
            Controls.Add(_form);
            _form.Show();        
        }

        /// <summary>
        /// Opens the specified file and attempts to parse it.  Returns true
        /// if the file is opened successfully (not necessarily parsed successfully).
        /// </summary>
        public bool LoadFile(string filePath)
        {
            filePath = Path.GetFullPath(filePath);
            return _form.StartReading(filePath, null);
        }

        /// <summary>
        /// Closes the log file if one is open.
        /// </summary>
        public void CloseFile()
        {
            _form.CloseFile();
        }
    }
}
