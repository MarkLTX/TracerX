using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TracerX.Viewer {
    public partial class FullText : Form {
        public FullText(string text) {
            InitializeComponent();
            this.Icon = Properties.Resources.scroll_view;
            textBox1.Text = text;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            // Clear the text selection.
            textBox1.SelectionStart = 0;

            Wrap.Checked = true;
        }

        private void edit_CheckedChanged(object sender, EventArgs e) {
            textBox1.ReadOnly = !edit.Checked;
        }

        private void Wrap_CheckedChanged(object sender, EventArgs e) {
            textBox1.WordWrap = Wrap.Checked;
        }

        private void copy_Click(object sender, EventArgs e) {
            Clipboard.SetText(textBox1.Text);
        }
    }
}