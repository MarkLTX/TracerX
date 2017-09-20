namespace TracerX
{
    partial class RelatedForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.linkProgramData = new System.Windows.Forms.LinkLabel();
            this.linkRecentFolders = new System.Windows.Forms.LinkLabel();
            this.linkRecentFiles = new System.Windows.Forms.LinkLabel();
            this.linkLocalAppData = new System.Windows.Forms.LinkLabel();
            this.linkEvents = new System.Windows.Forms.LinkLabel();
            this.lblAllUsers = new System.Windows.Forms.Label();
            this.lblCurUser = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // linkProgramData
            // 
            this.linkProgramData.AutoSize = true;
            this.linkProgramData.Location = new System.Drawing.Point(12, 36);
            this.linkProgramData.Name = "linkProgramData";
            this.linkProgramData.Size = new System.Drawing.Size(69, 13);
            this.linkProgramData.TabIndex = 0;
            this.linkProgramData.TabStop = true;
            this.linkProgramData.Text = "ProgramData";
            this.linkProgramData.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkProgramData_LinkClicked);
            // 
            // linkRecentFolders
            // 
            this.linkRecentFolders.AutoSize = true;
            this.linkRecentFolders.Location = new System.Drawing.Point(24, 68);
            this.linkRecentFolders.Name = "linkRecentFolders";
            this.linkRecentFolders.Size = new System.Drawing.Size(90, 13);
            this.linkRecentFolders.TabIndex = 1;
            this.linkRecentFolders.TabStop = true;
            this.linkRecentFolders.Text = "RecentFolders.txt";
            this.linkRecentFolders.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkRecentFolders_LinkClicked);
            // 
            // linkRecentFiles
            // 
            this.linkRecentFiles.AutoSize = true;
            this.linkRecentFiles.Location = new System.Drawing.Point(24, 52);
            this.linkRecentFiles.Name = "linkRecentFiles";
            this.linkRecentFiles.Size = new System.Drawing.Size(100, 13);
            this.linkRecentFiles.TabIndex = 2;
            this.linkRecentFiles.TabStop = true;
            this.linkRecentFiles.Text = "RecentlyCreated.txt";
            this.linkRecentFiles.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkRecentlyCreated_LinkClicked);
            // 
            // linkLocalAppData
            // 
            this.linkLocalAppData.AutoSize = true;
            this.linkLocalAppData.Location = new System.Drawing.Point(12, 104);
            this.linkLocalAppData.Name = "linkLocalAppData";
            this.linkLocalAppData.Size = new System.Drawing.Size(75, 13);
            this.linkLocalAppData.TabIndex = 3;
            this.linkLocalAppData.TabStop = true;
            this.linkLocalAppData.Text = "LocalAppData";
            this.linkLocalAppData.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLocalAppData_LinkClicked);
            // 
            // linkEvents
            // 
            this.linkEvents.AutoSize = true;
            this.linkEvents.Location = new System.Drawing.Point(24, 120);
            this.linkEvents.Name = "linkEvents";
            this.linkEvents.Size = new System.Drawing.Size(92, 13);
            this.linkEvents.TabIndex = 4;
            this.linkEvents.TabStop = true;
            this.linkEvents.Text = "TracerXEvents.txt";
            this.linkEvents.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkEvents_LinkClicked);
            // 
            // lblAllUsers
            // 
            this.lblAllUsers.AutoSize = true;
            this.lblAllUsers.Location = new System.Drawing.Point(87, 36);
            this.lblAllUsers.Name = "lblAllUsers";
            this.lblAllUsers.Size = new System.Drawing.Size(136, 13);
            this.lblAllUsers.TabIndex = 5;
            this.lblAllUsers.Text = "(folder for all TracerX users)";
            // 
            // lblCurUser
            // 
            this.lblCurUser.AutoSize = true;
            this.lblCurUser.Location = new System.Drawing.Point(87, 104);
            this.lblCurUser.Name = "lblCurUser";
            this.lblCurUser.Size = new System.Drawing.Size(71, 13);
            this.lblCurUser.TabIndex = 6;
            this.lblCurUser.Text = "(folder for {0})";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(130, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "(list of files created by logger)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(120, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(218, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "(list of folders where logs have been created)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(122, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(183, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "(event log msgs are also logged here)";
            // 
            // RelatedForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 261);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblCurUser);
            this.Controls.Add(this.lblAllUsers);
            this.Controls.Add(this.linkEvents);
            this.Controls.Add(this.linkLocalAppData);
            this.Controls.Add(this.linkRecentFiles);
            this.Controls.Add(this.linkRecentFolders);
            this.Controls.Add(this.linkProgramData);
            this.Name = "RelatedForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Related Files & Folders";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkProgramData;
        private System.Windows.Forms.LinkLabel linkRecentFolders;
        private System.Windows.Forms.LinkLabel linkRecentFiles;
        private System.Windows.Forms.LinkLabel linkLocalAppData;
        private System.Windows.Forms.LinkLabel linkEvents;
        private System.Windows.Forms.Label lblAllUsers;
        private System.Windows.Forms.Label lblCurUser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}