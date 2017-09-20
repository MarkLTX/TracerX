namespace TracerX {
    partial class FilterDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.traceLevelPage = new System.Windows.Forms.TabPage();
            this.invertTraceLevels = new System.Windows.Forms.Button();
            this.clearAllTraceLevels = new System.Windows.Forms.Button();
            this.selectAllTraceLevels = new System.Windows.Forms.Button();
            this.traceLevelListBox = new System.Windows.Forms.CheckedListBox();
            this.sessionPage = new System.Windows.Forms.TabPage();
            this.invertSessions = new System.Windows.Forms.Button();
            this.uncheckAllSessions = new System.Windows.Forms.Button();
            this.checkAllSessions = new System.Windows.Forms.Button();
            this.sessionListView = new System.Windows.Forms.ListView();
            this.sessionCheckCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sessionCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.loggerPage = new System.Windows.Forms.TabPage();
            this.loggerControl = new TracerX.ListAndTreeOfFilterable();
            this.methodPage = new System.Windows.Forms.TabPage();
            this.methodControl = new TracerX.ListAndTreeOfFilterable();
            this.calledMethodsChk = new System.Windows.Forms.CheckBox();
            this.threadNamePage = new System.Windows.Forms.TabPage();
            this.invertThreadNames = new System.Windows.Forms.Button();
            this.uncheckAllThreadNames = new System.Windows.Forms.Button();
            this.checkAllThreadNames = new System.Windows.Forms.Button();
            this.threadNameListView = new System.Windows.Forms.ListView();
            this.threadNameCheckCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.threadNameNameCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.threadIdPage = new System.Windows.Forms.TabPage();
            this.invertThreadIds = new System.Windows.Forms.Button();
            this.uncheckAllThreadIds = new System.Windows.Forms.Button();
            this.checkAllThreadIds = new System.Windows.Forms.Button();
            this.threadIdListView = new System.Windows.Forms.ListView();
            this.threadCheckCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.threadIdCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textPage = new System.Windows.Forms.TabPage();
            this.radRegex = new System.Windows.Forms.RadioButton();
            this.radWildcard = new System.Windows.Forms.RadioButton();
            this.radNormal = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.chkCase = new System.Windows.Forms.CheckBox();
            this.txtDoesNotContain = new System.Windows.Forms.TextBox();
            this.chkDoesNotContain = new System.Windows.Forms.CheckBox();
            this.txtContains = new System.Windows.Forms.TextBox();
            this.chkContain = new System.Windows.Forms.CheckBox();
            this.ok = new System.Windows.Forms.Button();
            this.apply = new System.Windows.Forms.Button();
            this.close = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.traceLevelPage.SuspendLayout();
            this.sessionPage.SuspendLayout();
            this.loggerPage.SuspendLayout();
            this.methodPage.SuspendLayout();
            this.threadNamePage.SuspendLayout();
            this.threadIdPage.SuspendLayout();
            this.textPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.traceLevelPage);
            this.tabControl1.Controls.Add(this.sessionPage);
            this.tabControl1.Controls.Add(this.loggerPage);
            this.tabControl1.Controls.Add(this.methodPage);
            this.tabControl1.Controls.Add(this.threadNamePage);
            this.tabControl1.Controls.Add(this.threadIdPage);
            this.tabControl1.Controls.Add(this.textPage);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(473, 251);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            // 
            // traceLevelPage
            // 
            this.traceLevelPage.Controls.Add(this.invertTraceLevels);
            this.traceLevelPage.Controls.Add(this.clearAllTraceLevels);
            this.traceLevelPage.Controls.Add(this.selectAllTraceLevels);
            this.traceLevelPage.Controls.Add(this.traceLevelListBox);
            this.traceLevelPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.traceLevelPage.Location = new System.Drawing.Point(4, 22);
            this.traceLevelPage.Name = "traceLevelPage";
            this.traceLevelPage.Padding = new System.Windows.Forms.Padding(3);
            this.traceLevelPage.Size = new System.Drawing.Size(465, 225);
            this.traceLevelPage.TabIndex = 0;
            this.traceLevelPage.Text = "Trace Levels";
            this.traceLevelPage.UseVisualStyleBackColor = true;
            // 
            // invertTraceLevels
            // 
            this.invertTraceLevels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.invertTraceLevels.Location = new System.Drawing.Point(382, 64);
            this.invertTraceLevels.Name = "invertTraceLevels";
            this.invertTraceLevels.Size = new System.Drawing.Size(75, 23);
            this.invertTraceLevels.TabIndex = 4;
            this.invertTraceLevels.Text = "Invert";
            this.invertTraceLevels.UseVisualStyleBackColor = true;
            this.invertTraceLevels.Click += new System.EventHandler(this.invertTraceLevels_Click);
            // 
            // clearAllTraceLevels
            // 
            this.clearAllTraceLevels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearAllTraceLevels.Location = new System.Drawing.Point(382, 35);
            this.clearAllTraceLevels.Name = "clearAllTraceLevels";
            this.clearAllTraceLevels.Size = new System.Drawing.Size(75, 23);
            this.clearAllTraceLevels.TabIndex = 3;
            this.clearAllTraceLevels.Text = "Uncheck All";
            this.clearAllTraceLevels.UseVisualStyleBackColor = true;
            this.clearAllTraceLevels.Click += new System.EventHandler(this.clearAllTraceLevels_Click);
            // 
            // selectAllTraceLevels
            // 
            this.selectAllTraceLevels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectAllTraceLevels.Location = new System.Drawing.Point(382, 6);
            this.selectAllTraceLevels.Name = "selectAllTraceLevels";
            this.selectAllTraceLevels.Size = new System.Drawing.Size(75, 23);
            this.selectAllTraceLevels.TabIndex = 2;
            this.selectAllTraceLevels.Text = "Check All";
            this.selectAllTraceLevels.UseVisualStyleBackColor = true;
            this.selectAllTraceLevels.Click += new System.EventHandler(this.selectAllTraceLevels_Click);
            // 
            // traceLevelListBox
            // 
            this.traceLevelListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.traceLevelListBox.CheckOnClick = true;
            this.traceLevelListBox.FormattingEnabled = true;
            this.traceLevelListBox.IntegralHeight = false;
            this.traceLevelListBox.Location = new System.Drawing.Point(3, 3);
            this.traceLevelListBox.Name = "traceLevelListBox";
            this.traceLevelListBox.Size = new System.Drawing.Size(373, 216);
            this.traceLevelListBox.TabIndex = 1;
            this.traceLevelListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.traceLevelListBox_ItemCheck);
            this.traceLevelListBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.traceLevelListBox_Format);
            // 
            // sessionPage
            // 
            this.sessionPage.Controls.Add(this.invertSessions);
            this.sessionPage.Controls.Add(this.uncheckAllSessions);
            this.sessionPage.Controls.Add(this.checkAllSessions);
            this.sessionPage.Controls.Add(this.sessionListView);
            this.sessionPage.Location = new System.Drawing.Point(4, 22);
            this.sessionPage.Name = "sessionPage";
            this.sessionPage.Padding = new System.Windows.Forms.Padding(3);
            this.sessionPage.Size = new System.Drawing.Size(465, 225);
            this.sessionPage.TabIndex = 7;
            this.sessionPage.Text = "Sessions";
            this.sessionPage.UseVisualStyleBackColor = true;
            // 
            // invertSessions
            // 
            this.invertSessions.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.invertSessions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.invertSessions.Location = new System.Drawing.Point(382, 64);
            this.invertSessions.Name = "invertSessions";
            this.invertSessions.Size = new System.Drawing.Size(75, 23);
            this.invertSessions.TabIndex = 7;
            this.invertSessions.Text = "Invert";
            this.invertSessions.UseVisualStyleBackColor = true;
            this.invertSessions.Click += new System.EventHandler(this.invertSessions_Click);
            // 
            // uncheckAllSessions
            // 
            this.uncheckAllSessions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uncheckAllSessions.Location = new System.Drawing.Point(382, 35);
            this.uncheckAllSessions.Name = "uncheckAllSessions";
            this.uncheckAllSessions.Size = new System.Drawing.Size(75, 23);
            this.uncheckAllSessions.TabIndex = 6;
            this.uncheckAllSessions.Text = "Uncheck All";
            this.uncheckAllSessions.UseVisualStyleBackColor = true;
            this.uncheckAllSessions.Click += new System.EventHandler(this.uncheckAllSessionss_Click);
            // 
            // checkAllSessions
            // 
            this.checkAllSessions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkAllSessions.Location = new System.Drawing.Point(382, 6);
            this.checkAllSessions.Name = "checkAllSessions";
            this.checkAllSessions.Size = new System.Drawing.Size(75, 23);
            this.checkAllSessions.TabIndex = 5;
            this.checkAllSessions.Text = "Check All";
            this.checkAllSessions.UseVisualStyleBackColor = true;
            this.checkAllSessions.Click += new System.EventHandler(this.checkAllSessions_Click);
            // 
            // sessionListView
            // 
            this.sessionListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sessionListView.CheckBoxes = true;
            this.sessionListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sessionCheckCol,
            this.sessionCol});
            this.sessionListView.FullRowSelect = true;
            this.sessionListView.Location = new System.Drawing.Point(3, 3);
            this.sessionListView.Name = "sessionListView";
            this.sessionListView.Size = new System.Drawing.Size(373, 216);
            this.sessionListView.TabIndex = 0;
            this.sessionListView.UseCompatibleStateImageBehavior = false;
            this.sessionListView.View = System.Windows.Forms.View.Details;
            this.sessionListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.sessionListView_ColumnClick);
            this.sessionListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.AnyListView_ItemChecked);
            // 
            // sessionCheckCol
            // 
            this.sessionCheckCol.Text = "";
            this.sessionCheckCol.Width = 24;
            // 
            // sessionCol
            // 
            this.sessionCol.Text = "Session";
            this.sessionCol.Width = 63;
            // 
            // loggerPage
            // 
            this.loggerPage.Controls.Add(this.loggerControl);
            this.loggerPage.Location = new System.Drawing.Point(4, 22);
            this.loggerPage.Name = "loggerPage";
            this.loggerPage.Size = new System.Drawing.Size(465, 225);
            this.loggerPage.TabIndex = 2;
            this.loggerPage.Text = "Loggers";
            this.loggerPage.UseVisualStyleBackColor = true;
            // 
            // loggerControl
            // 
            this.loggerControl.ColumnHeaderText = "Logger Name";
            this.loggerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loggerControl.Location = new System.Drawing.Point(0, 0);
            this.loggerControl.Name = "loggerControl";
            this.loggerControl.Size = new System.Drawing.Size(465, 225);
            this.loggerControl.TabIndex = 0;
            this.loggerControl.ItemChecked += new System.EventHandler(this.loggerControl_ItemChecked);
            // 
            // methodPage
            // 
            this.methodPage.Controls.Add(this.methodControl);
            this.methodPage.Controls.Add(this.calledMethodsChk);
            this.methodPage.Location = new System.Drawing.Point(4, 22);
            this.methodPage.Name = "methodPage";
            this.methodPage.Size = new System.Drawing.Size(465, 225);
            this.methodPage.TabIndex = 5;
            this.methodPage.Text = "Methods";
            this.methodPage.UseVisualStyleBackColor = true;
            // 
            // methodControl
            // 
            this.methodControl.ColumnHeaderText = "Method Name";
            this.methodControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.methodControl.Location = new System.Drawing.Point(0, 0);
            this.methodControl.Name = "methodControl";
            this.methodControl.Size = new System.Drawing.Size(465, 198);
            this.methodControl.TabIndex = 17;
            this.methodControl.ItemChecked += new System.EventHandler(this.methodControl_ItemChecked);
            // 
            // calledMethodsChk
            // 
            this.calledMethodsChk.AutoSize = true;
            this.calledMethodsChk.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.calledMethodsChk.Location = new System.Drawing.Point(0, 198);
            this.calledMethodsChk.Name = "calledMethodsChk";
            this.calledMethodsChk.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.calledMethodsChk.Size = new System.Drawing.Size(465, 27);
            this.calledMethodsChk.TabIndex = 16;
            this.calledMethodsChk.Text = "Show output from nested calls.";
            this.calledMethodsChk.UseVisualStyleBackColor = true;
            // 
            // threadNamePage
            // 
            this.threadNamePage.Controls.Add(this.invertThreadNames);
            this.threadNamePage.Controls.Add(this.uncheckAllThreadNames);
            this.threadNamePage.Controls.Add(this.checkAllThreadNames);
            this.threadNamePage.Controls.Add(this.threadNameListView);
            this.threadNamePage.Location = new System.Drawing.Point(4, 22);
            this.threadNamePage.Name = "threadNamePage";
            this.threadNamePage.Size = new System.Drawing.Size(465, 225);
            this.threadNamePage.TabIndex = 3;
            this.threadNamePage.Text = "Thread Names";
            this.threadNamePage.UseVisualStyleBackColor = true;
            // 
            // invertThreadNames
            // 
            this.invertThreadNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.invertThreadNames.Location = new System.Drawing.Point(382, 64);
            this.invertThreadNames.Name = "invertThreadNames";
            this.invertThreadNames.Size = new System.Drawing.Size(75, 23);
            this.invertThreadNames.TabIndex = 11;
            this.invertThreadNames.Text = "Invert";
            this.invertThreadNames.UseVisualStyleBackColor = true;
            this.invertThreadNames.Click += new System.EventHandler(this.invertThreadNames_Click);
            // 
            // uncheckAllThreadNames
            // 
            this.uncheckAllThreadNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uncheckAllThreadNames.Location = new System.Drawing.Point(382, 35);
            this.uncheckAllThreadNames.Name = "uncheckAllThreadNames";
            this.uncheckAllThreadNames.Size = new System.Drawing.Size(75, 23);
            this.uncheckAllThreadNames.TabIndex = 10;
            this.uncheckAllThreadNames.Text = "Uncheck All";
            this.uncheckAllThreadNames.UseVisualStyleBackColor = true;
            this.uncheckAllThreadNames.Click += new System.EventHandler(this.uncheckAllThreadNames_Click);
            // 
            // checkAllThreadNames
            // 
            this.checkAllThreadNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkAllThreadNames.Location = new System.Drawing.Point(382, 6);
            this.checkAllThreadNames.Name = "checkAllThreadNames";
            this.checkAllThreadNames.Size = new System.Drawing.Size(75, 23);
            this.checkAllThreadNames.TabIndex = 9;
            this.checkAllThreadNames.Text = "Check All";
            this.checkAllThreadNames.UseVisualStyleBackColor = true;
            this.checkAllThreadNames.Click += new System.EventHandler(this.checkAllThreadNames_Click);
            // 
            // threadNameListView
            // 
            this.threadNameListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.threadNameListView.CheckBoxes = true;
            this.threadNameListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.threadNameCheckCol,
            this.threadNameNameCol});
            this.threadNameListView.FullRowSelect = true;
            this.threadNameListView.Location = new System.Drawing.Point(3, 3);
            this.threadNameListView.Name = "threadNameListView";
            this.threadNameListView.Size = new System.Drawing.Size(373, 219);
            this.threadNameListView.TabIndex = 8;
            this.threadNameListView.UseCompatibleStateImageBehavior = false;
            this.threadNameListView.View = System.Windows.Forms.View.Details;
            this.threadNameListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.threadNameListView_ColumnClick);
            this.threadNameListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.AnyListView_ItemChecked);
            // 
            // threadNameCheckCol
            // 
            this.threadNameCheckCol.Text = "";
            this.threadNameCheckCol.Width = 24;
            // 
            // threadNameNameCol
            // 
            this.threadNameNameCol.Text = "Thread Name";
            this.threadNameNameCol.Width = 308;
            // 
            // threadIdPage
            // 
            this.threadIdPage.Controls.Add(this.invertThreadIds);
            this.threadIdPage.Controls.Add(this.uncheckAllThreadIds);
            this.threadIdPage.Controls.Add(this.checkAllThreadIds);
            this.threadIdPage.Controls.Add(this.threadIdListView);
            this.threadIdPage.Location = new System.Drawing.Point(4, 22);
            this.threadIdPage.Name = "threadIdPage";
            this.threadIdPage.Padding = new System.Windows.Forms.Padding(3);
            this.threadIdPage.Size = new System.Drawing.Size(465, 225);
            this.threadIdPage.TabIndex = 1;
            this.threadIdPage.Text = "Thread IDs";
            this.threadIdPage.UseVisualStyleBackColor = true;
            // 
            // invertThreadIds
            // 
            this.invertThreadIds.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.invertThreadIds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.invertThreadIds.Location = new System.Drawing.Point(382, 64);
            this.invertThreadIds.Name = "invertThreadIds";
            this.invertThreadIds.Size = new System.Drawing.Size(75, 23);
            this.invertThreadIds.TabIndex = 7;
            this.invertThreadIds.Text = "Invert";
            this.invertThreadIds.UseVisualStyleBackColor = true;
            this.invertThreadIds.Click += new System.EventHandler(this.invertThreadIDs_Click);
            // 
            // uncheckAllThreadIds
            // 
            this.uncheckAllThreadIds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uncheckAllThreadIds.Location = new System.Drawing.Point(382, 35);
            this.uncheckAllThreadIds.Name = "uncheckAllThreadIds";
            this.uncheckAllThreadIds.Size = new System.Drawing.Size(75, 23);
            this.uncheckAllThreadIds.TabIndex = 6;
            this.uncheckAllThreadIds.Text = "Uncheck All";
            this.uncheckAllThreadIds.UseVisualStyleBackColor = true;
            this.uncheckAllThreadIds.Click += new System.EventHandler(this.uncheckAllThreadIds_Click);
            // 
            // checkAllThreadIds
            // 
            this.checkAllThreadIds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkAllThreadIds.Location = new System.Drawing.Point(382, 6);
            this.checkAllThreadIds.Name = "checkAllThreadIds";
            this.checkAllThreadIds.Size = new System.Drawing.Size(75, 23);
            this.checkAllThreadIds.TabIndex = 5;
            this.checkAllThreadIds.Text = "Check All";
            this.checkAllThreadIds.UseVisualStyleBackColor = true;
            this.checkAllThreadIds.Click += new System.EventHandler(this.checkAllThreadIds_Click);
            // 
            // threadIdListView
            // 
            this.threadIdListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.threadIdListView.CheckBoxes = true;
            this.threadIdListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.threadCheckCol,
            this.threadIdCol});
            this.threadIdListView.FullRowSelect = true;
            this.threadIdListView.Location = new System.Drawing.Point(3, 3);
            this.threadIdListView.Name = "threadIdListView";
            this.threadIdListView.Size = new System.Drawing.Size(373, 216);
            this.threadIdListView.TabIndex = 0;
            this.threadIdListView.UseCompatibleStateImageBehavior = false;
            this.threadIdListView.View = System.Windows.Forms.View.Details;
            this.threadIdListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.threadIdListView_ColumnClick);
            this.threadIdListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.AnyListView_ItemChecked);
            // 
            // threadCheckCol
            // 
            this.threadCheckCol.Text = "";
            this.threadCheckCol.Width = 24;
            // 
            // threadIdCol
            // 
            this.threadIdCol.Text = "Thread ID";
            this.threadIdCol.Width = 63;
            // 
            // textPage
            // 
            this.textPage.Controls.Add(this.radRegex);
            this.textPage.Controls.Add(this.radWildcard);
            this.textPage.Controls.Add(this.radNormal);
            this.textPage.Controls.Add(this.label1);
            this.textPage.Controls.Add(this.chkCase);
            this.textPage.Controls.Add(this.txtDoesNotContain);
            this.textPage.Controls.Add(this.chkDoesNotContain);
            this.textPage.Controls.Add(this.txtContains);
            this.textPage.Controls.Add(this.chkContain);
            this.textPage.Location = new System.Drawing.Point(4, 22);
            this.textPage.Name = "textPage";
            this.textPage.Size = new System.Drawing.Size(465, 225);
            this.textPage.TabIndex = 4;
            this.textPage.Text = "Text";
            this.textPage.UseVisualStyleBackColor = true;
            // 
            // radRegex
            // 
            this.radRegex.AutoSize = true;
            this.radRegex.Location = new System.Drawing.Point(141, 152);
            this.radRegex.Name = "radRegex";
            this.radRegex.Size = new System.Drawing.Size(115, 17);
            this.radRegex.TabIndex = 10;
            this.radRegex.Text = "Regular expression";
            this.radRegex.UseVisualStyleBackColor = true;
            this.radRegex.CheckedChanged += new System.EventHandler(this.Text_CheckboxChanged);
            // 
            // radWildcard
            // 
            this.radWildcard.AutoSize = true;
            this.radWildcard.Location = new System.Drawing.Point(68, 151);
            this.radWildcard.Name = "radWildcard";
            this.radWildcard.Size = new System.Drawing.Size(67, 17);
            this.radWildcard.TabIndex = 9;
            this.radWildcard.Text = "Wildcard";
            this.radWildcard.UseVisualStyleBackColor = true;
            this.radWildcard.CheckedChanged += new System.EventHandler(this.Text_CheckboxChanged);
            // 
            // radNormal
            // 
            this.radNormal.AutoSize = true;
            this.radNormal.Checked = true;
            this.radNormal.Location = new System.Drawing.Point(4, 152);
            this.radNormal.Name = "radNormal";
            this.radNormal.Size = new System.Drawing.Size(58, 17);
            this.radNormal.TabIndex = 8;
            this.radNormal.TabStop = true;
            this.radNormal.Text = "Normal";
            this.radNormal.UseVisualStyleBackColor = true;
            this.radNormal.CheckedChanged += new System.EventHandler(this.Text_CheckboxChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Lines must meet each condition to be shown.";
            // 
            // chkCase
            // 
            this.chkCase.AutoSize = true;
            this.chkCase.Location = new System.Drawing.Point(4, 129);
            this.chkCase.Name = "chkCase";
            this.chkCase.Size = new System.Drawing.Size(82, 17);
            this.chkCase.TabIndex = 5;
            this.chkCase.Text = "Match case";
            this.chkCase.UseVisualStyleBackColor = true;
            this.chkCase.CheckedChanged += new System.EventHandler(this.Text_CheckboxChanged);
            // 
            // txtDoesNotContain
            // 
            this.txtDoesNotContain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDoesNotContain.Enabled = false;
            this.txtDoesNotContain.Location = new System.Drawing.Point(23, 97);
            this.txtDoesNotContain.Name = "txtDoesNotContain";
            this.txtDoesNotContain.Size = new System.Drawing.Size(398, 20);
            this.txtDoesNotContain.TabIndex = 3;
            this.txtDoesNotContain.TextChanged += new System.EventHandler(this.Text_FilterTextChanged);
            // 
            // chkDoesNotContain
            // 
            this.chkDoesNotContain.AutoSize = true;
            this.chkDoesNotContain.Location = new System.Drawing.Point(4, 79);
            this.chkDoesNotContain.Name = "chkDoesNotContain";
            this.chkDoesNotContain.Size = new System.Drawing.Size(172, 17);
            this.chkDoesNotContain.TabIndex = 2;
            this.chkDoesNotContain.Text = "Show lines that do not contain:";
            this.chkDoesNotContain.UseVisualStyleBackColor = true;
            this.chkDoesNotContain.CheckedChanged += new System.EventHandler(this.Text_CheckboxChanged);
            // 
            // txtContains
            // 
            this.txtContains.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContains.Enabled = false;
            this.txtContains.Location = new System.Drawing.Point(23, 48);
            this.txtContains.Name = "txtContains";
            this.txtContains.Size = new System.Drawing.Size(398, 20);
            this.txtContains.TabIndex = 1;
            this.txtContains.TextChanged += new System.EventHandler(this.Text_FilterTextChanged);
            // 
            // chkContain
            // 
            this.chkContain.AutoSize = true;
            this.chkContain.Location = new System.Drawing.Point(4, 30);
            this.chkContain.Name = "chkContain";
            this.chkContain.Size = new System.Drawing.Size(139, 17);
            this.chkContain.TabIndex = 0;
            this.chkContain.Text = "Show lines that contain:";
            this.chkContain.UseVisualStyleBackColor = true;
            this.chkContain.CheckedChanged += new System.EventHandler(this.Text_CheckboxChanged);
            // 
            // ok
            // 
            this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ok.Enabled = false;
            this.ok.Location = new System.Drawing.Point(224, 261);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 1;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // apply
            // 
            this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.apply.Enabled = false;
            this.apply.Location = new System.Drawing.Point(305, 261);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(75, 23);
            this.apply.TabIndex = 2;
            this.apply.Text = "Apply";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.close.Location = new System.Drawing.Point(386, 261);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(75, 23);
            this.close.TabIndex = 3;
            this.close.Text = "Cancel";
            this.close.UseVisualStyleBackColor = true;
            // 
            // FilterDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 296);
            this.Controls.Add(this.close);
            this.Controls.Add(this.apply);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(489, 334);
            this.Name = "FilterDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TracerX Filters";
            this.tabControl1.ResumeLayout(false);
            this.traceLevelPage.ResumeLayout(false);
            this.sessionPage.ResumeLayout(false);
            this.loggerPage.ResumeLayout(false);
            this.methodPage.ResumeLayout(false);
            this.methodPage.PerformLayout();
            this.threadNamePage.ResumeLayout(false);
            this.threadIdPage.ResumeLayout(false);
            this.textPage.ResumeLayout(false);
            this.textPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage traceLevelPage;
        private System.Windows.Forms.TabPage threadIdPage;
        private System.Windows.Forms.CheckedListBox traceLevelListBox;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Button apply;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.ListView threadIdListView;
        private System.Windows.Forms.ColumnHeader threadIdCol;
        private System.Windows.Forms.Button invertTraceLevels;
        private System.Windows.Forms.Button clearAllTraceLevels;
        private System.Windows.Forms.Button selectAllTraceLevels;
        private System.Windows.Forms.Button invertThreadIds;
        private System.Windows.Forms.Button uncheckAllThreadIds;
        private System.Windows.Forms.Button checkAllThreadIds;
        private System.Windows.Forms.ColumnHeader threadCheckCol;
        private System.Windows.Forms.TabPage loggerPage;
        private System.Windows.Forms.TabPage threadNamePage;
        private System.Windows.Forms.Button invertThreadNames;
        private System.Windows.Forms.Button uncheckAllThreadNames;
        private System.Windows.Forms.Button checkAllThreadNames;
        private System.Windows.Forms.ListView threadNameListView;
        private System.Windows.Forms.ColumnHeader threadNameCheckCol;
        private System.Windows.Forms.ColumnHeader threadNameNameCol;
        private System.Windows.Forms.TabPage textPage;
        private System.Windows.Forms.TextBox txtDoesNotContain;
        private System.Windows.Forms.CheckBox chkDoesNotContain;
        private System.Windows.Forms.TextBox txtContains;
        private System.Windows.Forms.CheckBox chkContain;
        private System.Windows.Forms.CheckBox chkCase;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage methodPage;
        private System.Windows.Forms.CheckBox calledMethodsChk;
        private System.Windows.Forms.RadioButton radRegex;
        private System.Windows.Forms.RadioButton radWildcard;
        private System.Windows.Forms.RadioButton radNormal;
        private System.Windows.Forms.TabPage sessionPage;
        private System.Windows.Forms.Button invertSessions;
        private System.Windows.Forms.Button uncheckAllSessions;
        private System.Windows.Forms.Button checkAllSessions;
        private System.Windows.Forms.ListView sessionListView;
        private System.Windows.Forms.ColumnHeader sessionCheckCol;
        private System.Windows.Forms.ColumnHeader sessionCol;
        private ListAndTreeOfFilterable loggerControl;
        private ListAndTreeOfFilterable methodControl;
    }
}