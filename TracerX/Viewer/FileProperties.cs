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
            propertyGrid1.SelectedObject = reader;
            this.Icon = Properties.Resources.scroll_view;
        }
    }
}