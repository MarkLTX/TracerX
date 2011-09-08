using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Linq;

namespace TracerX.Viewer
{
    public partial class PasswordDialog : Form
    {
        private byte[] _fileHash;

        public byte[] EncryptionKey
        {
            get;
            private set;
        }

        public ICryptoTransform CryptoTransform
        {
            get;
            private set;
        }

        public PasswordDialog(byte[] fileHash, string hint)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.scroll_view;
            _fileHash = fileHash;

            if (string.IsNullOrEmpty(hint))
            {
                // Hide the hint controls and shrink the size.
                // Do not allow the form to resize higher, but do allow wider
                lblHint.Visible = false;
                txtHint.Visible = false;
                Size = MinimumSize;
                MaximumSize = new Size(9999, Size.Height);
            }
            else
            {
                // Display the hint and don't allow the for to resize any
                // smaller.  
                txtHint.Text = hint;
                MinimumSize = Size;
                MaximumSize = new Size(9999, 9999);
            }
        }

        private void ok_Click(object sender, EventArgs e)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] pwBytes = System.Text.Encoding.Unicode.GetBytes(this.textBox1.Text);
            byte[] pwHash = sha1.ComputeHash(pwBytes);
            
            if (_fileHash.SequenceEqual(pwHash))
            {
                // Create the encryption key from the password and 'salt'.  The salt
                // can be any byte array, but it has to be something we can acquire
                // again in the viewer.
                Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes(this.textBox1.Text, pwHash);
                EncryptionKey = keyGenerator.GetBytes(16);

                Close();
            }
            else
            {
                DialogResult = DialogResult.None;
                MessageBox.Show(this, "The password is invalid.", "Invalid Password");
            }
        }
    }
}