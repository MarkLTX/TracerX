using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace TracerX
{
    public partial class CredentialsDialog : Form
    {
        public CredentialsDialog()
        {
            InitializeComponent();
        }

        public string UserID
        {
            get { return txtUser.Text; }
            set { txtUser.Text = value; }
        }

        public string PW
        {
            get;
            set;
        }

        private bool _pwChanged;
        
        protected override void OnLoad(EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PW))
            {
                txtPW.Text = "XXXXXXXXXX";
            }
            
            _pwChanged = false;

            base.OnLoad(e);
        }

        /// <summary>
        /// Encrypts a clear text string (e.g. a password) and
        /// converts the cipher-bytes to a base 64 string.
        /// </summary>
        public static string Encrypt64(string clearText)
        {
            // All advice says never to store a password in a string object,
            // but how else to get it from a TextBox?

            byte[] bytes = Encoding.Unicode.GetBytes(clearText);
            bytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            string result = Convert.ToBase64String(bytes);
            return result;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            txtUser.Text = txtUser.Text.Trim();

            if (txtUser.Text == "")
            {
                PW = "";
                DialogResult = DialogResult.OK;
                Close();
            }
            else if (txtPW.Text == "")
            {
                MainForm.ShowMessageBox("A password is required when a non-blank User ID is specified.");
            }
            else
            {
                if (_pwChanged)
                {
                    PW = Encrypt64(txtPW.Text);
                    txtPW.Text = "";
                }

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtPW_TextChanged(object sender, EventArgs e)
        {
            _pwChanged = true;
        }
    }
}
