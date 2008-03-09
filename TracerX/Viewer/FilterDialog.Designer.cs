namespace TracerX.Viewer {
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
            this.loggerPage = new System.Windows.Forms.TabPage();
            this.invertLoggers = new System.Windows.Forms.Button();
            this.uncheckAllLoggers = new System.Windows.Forms.Button();
            this.checkAllLoggers = new System.Windows.Forms.Button();
            this.loggerListView = new System.Windows.Forms.ListView();
            this.loggerCheckCol = new System.Windows.Forms.ColumnHeader();
            this.loggerNameCol = new System.Windows.Forms.ColumnHeader();
            this.threadIdPage = new System.Windows.Forms.TabPage();
            this.invertThreadIds = new System.Windows.Forms.Button();
            this.uncheckAllThreadIds = new System.Windows.Forms.Button();
            this.checkAllThreadIds = new System.Windows.Forms.Button();
            this.threadIdListView = new System.Windows.Forms.ListView();
            this.threadCheckCol = new System.Windows.Forms.ColumnHeader();
            this.threadIdCol = new System.Windows.Forms.ColumnHeader();
            this.threadNamePage = new System.Windows.Forms.TabPage();
            this.invertThreadNames = new System.Windows.Forms.Button();
            this.uncheckAllThreadNames = new System.Windows.Forms.Button();
            this.checkAllThreadNames = new System.Windows.Forms.Button();
            this.threadNameListView = new System.Windows.Forms.ListView();
            this.threadNameCheckCol = new System.Windows.Forms.ColumnHeader();
            this.threadNameNameCol = new System.Windows.Forms.ColumnHeader();
            this.textPage = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.chkCase = new System.Windows.Forms.CheckBox();
            this.chkWild = new System.Windows.Forms.CheckBox();
            this.txtDoesNotContain = new System.Windows.Forms.TextBox();
            this.chkDoesNotContain = new System.Windows.Forms.CheckBox();
            this.txtContains = new System.Windows.Forms.TextBox();
            this.chkContain = new System.Windows.Forms.CheckBox();
            this.ok = new System.Windows.Forms.Button();
            this.apply = new System.Windows.Forms.Button();
            this.close = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.traceLevelPage.SuspendLayout();
            this.loggerPage.SuspendLayout();
            this.threadIdPage.SuspendLayout();
            this.threadNamePage.SuspendLayout();
            this.textPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.traceLevelPage);
            this.tabControl1.Controls.Add(this.loggerPage);
            this.tabControl1.Controls.Add(this.threadIdPage);
            this.tabControl1.Controls.Add(this.threadNamePage);
            this.tabControl1.Controls.Add(this.textPage);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(361, 202);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // traceLevelPage
            // 
            this.traceLevelPage.Controls.Add(this.invertTraceLevels);
            this.traceLevelPage.Controls.Add(this.clearAllTraceLevels);
            this.traceLevelPage.Controls.Add(this.selectAllTraceLevels);
            this.traceLevelPage.Controls.Add(this.traceLevelListBox);
            this.traceLevelPage.Location = new System.Drawing.Point(4, 22);
            this.traceLevelPage.Name = "traceLevelPage";
            this.traceLevelPage.Padding = new System.Windows.Forms.Padding(3);
            this.traceLevelPage.Size = new System.Drawing.Size(353, 176);
            this.traceLevelPage.TabIndex = 0;
            this.traceLevelPage.Text = "Trace Levels";
            this.traceLevelPage.UseVisualStyleBackColor = true;
            // 
            // invertTraceLevels
            // 
            this.invertTraceLevels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.invertTraceLevels.Location = new System.Drawing.Point(270, 64);
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
            this.clearAllTraceLevels.Location = new System.Drawing.Point(270, 35);
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
            this.selectAllTraceLevels.Location = new System.Drawing.Point(270, 6);
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
            this.traceLevelListBox.IntegralHeight = false;
            this.traceLevelListBox.Location = new System.Drawing.Point(3, 3);
            this.traceLevelListBox.Name = "traceLevelListBox";
            this.traceLevelListBox.Size = new System.Drawing.Size(261, 168);
            this.traceLevelListBox.TabIndex = 1;
            this.traceLevelListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.traceLevelListBox_ItemCheck);
            // 
            // loggerPage
            // 
            this.loggerPage.Controls.Add(this.invertLoggers);
            this.loggerPage.Controls.Add(this.uncheckAllLoggers);
            this.loggerPage.Controls.Add(this.checkAllLoggers);
            this.loggerPage.Controls.Add(this.loggerListView);
            this.loggerPage.Location = new System.Drawing.Point(4, 22);
            this.loggerPage.Name = "loggerPage";
            this.loggerPage.Size = new System.Drawing.Size(353, 176);
            this.loggerPage.TabIndex = 2;
            this.loggerPage.Text = "Loggers";
            this.loggerPage.UseVisualStyleBackColor = true;
            // 
            // invertLoggers
            // 
            this.invertLoggers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.invertLoggers.Location = new System.Drawing.Point(270, 64);
            this.invertLoggers.Name = "invertLoggers";
            this.invertLoggers.Size = new System.Drawing.Size(75, 23);
            this.invertLoggers.TabIndex = 11;
            this.invertLoggers.Text = "Invert";
            this.invertLoggers.UseVisualStyleBackColor = true;
            this.invertLoggers.Click += new System.EventHandler(this.invertLoggers_Click);
            // 
            // uncheckAllLoggers
            // 
            this.uncheckAllLoggers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uncheckAllLoggers.Location = new System.Drawing.Point(270, 35);
            this.uncheckAllLoggers.Name = "uncheckAllLoggers";
            this.uncheckAllLoggers.Size = new System.Drawing.Size(75, 23);
            this.uncheckAllLoggers.TabIndex = 10;
            this.uncheckAllLoggers.Text = "Uncheck All";
            this.uncheckAllLoggers.UseVisualStyleBackColor = true;
            this.uncheckAllLoggers.Click += new System.EventHandler(this.uncheckAllLoggers_Click);
            // 
            // checkAllLoggers
            // 
            this.checkAllLoggers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkAllLoggers.Location = new System.Drawing.Point(270, 6);
            this.checkAllLoggers.Name = "checkAllLoggers";
            this.checkAllLoggers.Size = new System.Drawing.Size(75, 23);
            this.checkAllLoggers.TabIndex = 9;
            this.checkAllLoggers.Text = "Check All";
            this.checkAllLoggers.UseVisualStyleBackColor = true;
            this.checkAllLoggers.Click += new System.EventHandler(this.checkAllLoggers_Click);
            // 
            // loggerListView
            // 
            this.loggerListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.loggerListView.CheckBoxes = true;
            this.loggerListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.loggerCheckCol,
            this.loggerNameCol});
            this.loggerListView.FullRowSelect = true;
            this.loggerListView.Location = new System.Drawing.Point(3, 3);
            this.loggerListView.Name = "loggerListView";
            this.loggerListView.Size = new System.Drawing.Size(261, 170);
            this.loggerListView.TabIndex = 8;
            this.loggerListView.UseCompatibleStateImageBehavior = false;
            this.loggerListView.View = System.Windows.Forms.View.Details;
            this.loggerListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.AnyListView_ItemChecked);
            this.loggerListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.loggerListView_ColumnClick);
            // 
            // loggerCheckCol
            // 
            this.loggerCheckCol.Text = "";
            this.loggerCheckCol.Width = 19;
            // 
            // loggerNameCol
            // 
            this.loggerNameCol.Text = "Logger Name";
            this.loggerNameCol.Width = 198;
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
            this.threadIdPage.Size = new System.Drawing.Size(353, 176);
            this.threadIdPage.TabIndex = 1;
            this.threadIdPage.Text = "Thread IDs";
            this.threadIdPage.UseVisualStyleBackColor = true;
            // 
            // invertThreadIds
            // 
            this.invertThreadIds.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.invertThreadIds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.invertThreadIds.Location = new System.Drawing.Point(270, 64);
            this.invertThreadIds.Name = "invertThreadIds";
            this.invertThreadIds.Size = new System.Drawing.Size(75, 23);
            this.invertThreadIds.TabIndex = 7;
            this.invertThreadIds.Text = "Invert";
            this.invertThreadIds.UseVisualStyleBackColor = true;
            this.invertThreadIds.Click += new System.EventHandler(this.invertThreads_Click);
            // 
            // uncheckAllThreadIds
            // 
            this.uncheckAllThreadIds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uncheckAllThreadIds.Location = new System.Drawing.Point(270, 35);
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
            this.checkAllThreadIds.Location = new System.Drawing.Point(270, 6);
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
            this.threadIdListView.Size = new System.Drawing.Size(261, 167);
            this.threadIdListView.TabIndex = 0;
            this.threadIdListView.UseCompatibleStateImageBehavior = false;
            this.threadIdListView.View = System.Windows.Forms.View.Details;
            this.threadIdListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.AnyListView_ItemChecked);
            this.threadIdListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.threadIdListView_ColumnClick);
            // 
            // threadCheckCol
            // 
            this.threadCheckCol.Text = "";
            this.threadCheckCol.Width = 19;
            // 
            // threadIdCol
            // 
            this.threadIdCol.Text = "Thread ID";
            this.threadIdCol.Width = 63;
            // 
            // threadNamePage
            // 
            this.threadNamePage.Controls.Add(this.invertThreadNames);
            this.threadNamePage.Controls.Add(this.uncheckAllThreadNames);
            this.threadNamePage.Controls.Add(this.checkAllThreadNames);
            this.threadNamePage.Controls.Add(this.threadNameListView);
            this.threadNamePage.Location = new System.Drawing.Point(4, 22);
            this.threadNamePage.Name = "threadNamePage";
            this.threadNamePage.Size = new System.Drawing.Size(353, 176);
            this.threadNamePage.TabIndex = 3;
            this.threadNamePage.Text = "Thread Names";
            this.threadNamePage.UseVisualStyleBackColor = true;
            // 
            // invertThreadNames
            // 
            this.invertThreadNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.invertThreadNames.Location = new System.Drawing.Point(270, 64);
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
            this.uncheckAllThreadNames.Location = new System.Drawing.Point(270, 35);
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
            this.checkAllThreadNames.Location = new System.Drawing.Point(270, 6);
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
            this.threadNameListView.Size = new System.Drawing.Size(261, 167);
            this.threadNameListView.TabIndex = 8;
            this.threadNameListView.UseCompatibleStateImageBehavior = false;
            this.threadNameListView.View = System.Windows.Forms.View.Details;
            this.threadNameListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.AnyListView_ItemChecked);
            this.threadNameListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.threadNameListView_ColumnClick);
            // 
            // threadNameCheckCol
            // 
            this.threadNameCheckCol.Text = "";
            this.threadNameCheckCol.Width = 19;
            // 
            // threadNameNameCol
            // 
            this.threadNameNameCol.Text = "Thread Name";
            this.threadNameNameCol.Width = 212;
            // 
            // textPage
            // 
            this.textPage.Controls.Add(this.label1);
            this.textPage.Controls.Add(this.chkCase);
            this.textPage.Controls.Add(this.chkWild);
            this.textPage.Controls.Add(this.txtDoesNotContain);
            this.textPage.Controls.Add(this.chkDoesNotContain);
            this.textPage.Controls.Add(this.txtContains);
            this.textPage.Controls.Add(this.chkContain);
            this.textPage.Location = new System.Drawing.Point(4, 22);
            this.textPage.Name = "textPage";
            this.textPage.Size = new System.Drawing.Size(353, 176);
            this.textPage.TabIndex = 4;
            this.textPage.Text = "Text";
            this.textPage.UseVisualStyleBackColor = true;
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
            // chkWild
            // 
            this.chkWild.AutoSize = true;
            this.chkWild.Location = new System.Drawing.Point(4, 152);
            this.chkWild.Name = "chkWild";
            this.chkWild.Size = new System.Drawing.Size(92, 17);
            this.chkWild.TabIndex = 4;
            this.chkWild.Text = "Use wildcards";
            this.chkWild.UseVisualStyleBackColor = true;
            this.chkWild.CheckedChanged += new System.EventHandler(this.Text_CheckboxChanged);
            // 
            // txtDoesNotContain
            // 
            this.txtDoesNotContain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDoesNotContain.Enabled = false;
            this.txtDoesNotContain.Location = new System.Drawing.Point(23, 97);
            this.txtDoesNotContain.Name = "txtDoesNotContain";
            this.txtDoesNotContain.Size = new System.Drawing.Size(322, 20);
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
            this.txtContains.Size = new System.Drawing.Size(322, 20);
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
            this.ok.Location = new System.Drawing.Point(112, 212);
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
            this.apply.Location = new System.Drawing.Point(193, 212);
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
            this.close.Location = new System.Drawing.Point(274, 212);
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
            this.ClientSize = new System.Drawing.Size(361, 247);
            this.Controls.Add(this.close);
            this.Controls.Add(this.apply);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(369, 281);
            this.Name = "FilterDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "TracerX Filters";
            this.tabControl1.ResumeLayout(false);
            this.traceLevelPage.ResumeLayout(false);
            this.loggerPage.ResumeLayout(false);
            this.threadIdPage.ResumeLayout(false);
            this.threadNamePage.ResumeLayout(false);
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
        private System.Windows.Forms.Button invertLoggers;
        private System.Windows.Forms.Button uncheckAllLoggers;
        private System.Windows.Forms.Button checkAllLoggers;
        private System.Windows.Forms.ListView loggerListView;
        private System.Windows.Forms.ColumnHeader loggerCheckCol;
        private System.Windows.Forms.ColumnHeader loggerNameCol;
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
        private System.Windows.Forms.CheckBox chkWild;
        private System.Windows.Forms.Label label1;
    }
}