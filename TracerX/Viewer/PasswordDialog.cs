using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace TracerX.Viewer {
    public partial class PasswordDialog : Form {
        public PasswordDialog(int fileHash) {
            InitializeComponent();
            this.Icon = Properties.Resources.scroll_view;
            _fileHash = fileHash;
        }

        private int _fileHash;

        private void ok_Click(object sender, EventArgs e) {
            if (this.textBox1.Text.GetHashCode() == _fileHash) {
                Close();
            } else {
                DialogResult = DialogResult.None;
                MessageBox.Show(this, "The password is invalid.", "Invalid Password");
            }
        }
    }
}