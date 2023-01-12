using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace TracerX
{
    // Form for editing or creating a remote server connection.
    public partial class ServerConnEditor : Form
    {
        public ServerConnEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The new or edited server is returned by this property.
        /// </summary>
        public SavedServer NewServer 
        {
            get;
            private set;
        }

        public bool ShowConnectButton
        {
            get { return btnOkConnect.Visible; }
            set { btnOkConnect.Visible = value; }
        }

        public bool DoConnect
        {
            get;
            private set;
        }

        // After calling Init(), _editServer is null if the form is in "add" mode, 
        // non-null if in "edit" mode.
        private SavedServer _editServer;
        private IEnumerable<string> _otherServerNames;
        private bool _pwChanged;
        private string _newPW;

        private bool _keepNameEqualToAddress;
        private bool _isUserChange = true;

        /// <summary>
        /// If adding a new server pass null for serverToEdit.  This will cause
        /// a new SavedServer object to be created when the user clicks OK.
        /// To edit an existing server pass it in via the serverToEdit parameter.
        /// The specified server will be modified in-place when the user clicks OK.
        /// The otherNames parameter is used to prevent creating two servers
        /// with the same display name.
        /// </summary>
        public void Init(IEnumerable<string> categories, IEnumerable<string> otherNames, SavedServer serverToEdit, bool allowAddressEdit = true)
        {
            _otherServerNames = otherNames;
            comboCategories.Items.AddRange(categories.OrderBy(s=>s).ToArray());

            if (serverToEdit == null)
            {
                // We'll be starting with empty controls. The "server address" will be 
                // copied to the "display name" as the user types until he explicitly
                // changes the "display name".

                _keepNameEqualToAddress = true;
            }
            else
            {
                // Initialize the controls from serverToEdit.  

                _keepNameEqualToAddress = false;
                _editServer = serverToEdit;
                txtAddress.Text = serverToEdit.HostAddress;
                txtDispName.Text = serverToEdit.HostName;

                if (serverToEdit.Port > 0)
                {
                    txtPort.Text = serverToEdit.Port.ToString();
                }

                comboCategories.SelectedItem = serverToEdit.Category;
                txtUserID.Text = serverToEdit.UserId;
                _newPW = serverToEdit.PW;

                if (!string.IsNullOrWhiteSpace(serverToEdit.PW))
                {
                    // Indicate the existence of a password with a placeholder string.
                    txtPassword.Text = "XXXXXXXXXX";
                }
            }

            txtAddress.Enabled = allowAddressEdit;
            txtPort.Enabled = allowAddressEdit;

            _pwChanged = false;
        }

        private void txtDispName_TextChanged(object sender, EventArgs e)
        {
            // Once the user manually edits the txtDispName box 
            // (i.e. _isUserChange is true) we stop copying
            // txtAddress to txtDispName .

            _keepNameEqualToAddress = _keepNameEqualToAddress &&  !_isUserChange;
        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {
            if (_keepNameEqualToAddress)
            {
                _isUserChange = false;
                txtDispName.Text = txtAddress.Text;
                _isUserChange = true;
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            _pwChanged = true;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (CheckProps() && CheckCreds())
            {
                var tempSavedServer = new SavedServer();
                SetServerProperties(tempSavedServer);
                var tempRemoteServer = new RemoteServer(tempSavedServer);
                tempRemoteServer.TestConnection().Show();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (CheckProps() && CheckCreds())
            {
                if (_editServer == null)
                {
                    // We're in "add" mode.
                    NewServer = new SavedServer();
                    SetServerProperties(NewServer);
                }
                else
                {
                    // We're in "edit" mode.
                    SetServerProperties(_editServer);
                    NewServer = _editServer;
                }

                DoConnect = sender == btnOkConnect;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void SetServerProperties(SavedServer savedServer)
        {
            savedServer.HostAddress = txtAddress.Text;
            savedServer.HostName = txtDispName.Text;
            savedServer.UserId = txtUserID.Text;
            savedServer.PW = _newPW;
            savedServer.Category = comboCategories.Text.Trim();

            if (txtPort.Text == "")
            {
                savedServer.Port = -1;
            }
            else
            {
                savedServer.Port = int.Parse(txtPort.Text);
            }
        }

        private bool CheckProps()
        {
            bool result = false;
            int portNum = -1;

            txtAddress.Text = txtAddress.Text.Trim();
            txtPort.Text = txtPort.Text.Trim();
            txtDispName.Text = txtDispName.Text.Trim();

            if (txtDispName.Text == "")
            {
                MainForm.ShowMessageBox("Please enter the server's display name");
            }
            else if (_otherServerNames.Contains(txtDispName.Text, StringComparer.OrdinalIgnoreCase))
            {
                MainForm.ShowMessageBox("The specified display name is already in use.");
            }
            else if (txtAddress.Text == "")
            {
                MainForm.ShowMessageBox("Please enter a server name or address.");
            }
            else if (txtPort.Text != "" && !int.TryParse(txtPort.Text, out portNum))
            {
                MainForm.ShowMessageBox("The specified port is not a number.");
            }
            else if (txtPort.Text != "" && portNum < 1 || portNum > 65535)
            {
                MainForm.ShowMessageBox("The specified port number is not in the range 1 - 65535.");
            }
            else
            {
                result = true;
            }

            return result;
        }

        private bool CheckCreds()
        {
            bool result;
            txtUserID.Text = txtUserID.Text.Trim();

            if (txtUserID.Text == "")
            {
                // No user ID means no password, even if the user entered one.

                _newPW = null;
                result = true;
            }
            else if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MainForm.ShowMessageBox("A password is required when a non-blank User ID is specified.");
                result = false;
            }
            else
            {
                if (_pwChanged)
                {
                    _newPW = CredentialsDialog.Encrypt64(txtPassword.Text);
                }
                else
                {
                    // The textbox still holds the placeholder text and
                    // _newPW still holds the original password (so leave it there).
                }

                result = true;
            }

            return result;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void chkShow_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShow.Checked;
        }
    }
}
