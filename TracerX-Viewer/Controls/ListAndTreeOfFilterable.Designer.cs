namespace TracerX
{
    partial class ListAndTreeOfFilterable
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Tree");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Of");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Loggers");
            this.collapseLoggers = new System.Windows.Forms.Button();
            this.expandLoggers = new System.Windows.Forms.Button();
            this.radLoggerTree = new System.Windows.Forms.RadioButton();
            this.radLoggerList = new System.Windows.Forms.RadioButton();
            this.loggerTreeView = new TracerX.NoDblClickTreeView();
            this.invertLoggers = new System.Windows.Forms.Button();
            this.uncheckAllLoggers = new System.Windows.Forms.Button();
            this.checkAllLoggers = new System.Windows.Forms.Button();
            this.loggerListView = new System.Windows.Forms.ListView();
            this.loggerCheckCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.loggerNameCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // collapseLoggers
            // 
            this.collapseLoggers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.collapseLoggers.Location = new System.Drawing.Point(382, 169);
            this.collapseLoggers.Name = "collapseLoggers";
            this.collapseLoggers.Size = new System.Drawing.Size(75, 23);
            this.collapseLoggers.TabIndex = 25;
            this.collapseLoggers.Text = "Collapse All";
            this.collapseLoggers.UseVisualStyleBackColor = true;
            this.collapseLoggers.Click += new System.EventHandler(this.collapseLoggers_Click);
            // 
            // expandLoggers
            // 
            this.expandLoggers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.expandLoggers.Location = new System.Drawing.Point(382, 140);
            this.expandLoggers.Name = "expandLoggers";
            this.expandLoggers.Size = new System.Drawing.Size(75, 23);
            this.expandLoggers.TabIndex = 24;
            this.expandLoggers.Text = "Expand All";
            this.expandLoggers.UseVisualStyleBackColor = true;
            this.expandLoggers.Click += new System.EventHandler(this.expandLoggers_Click);
            // 
            // radLoggerTree
            // 
            this.radLoggerTree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radLoggerTree.AutoSize = true;
            this.radLoggerTree.Location = new System.Drawing.Point(382, 117);
            this.radLoggerTree.Name = "radLoggerTree";
            this.radLoggerTree.Size = new System.Drawing.Size(47, 17);
            this.radLoggerTree.TabIndex = 23;
            this.radLoggerTree.TabStop = true;
            this.radLoggerTree.Text = "Tree";
            this.radLoggerTree.UseVisualStyleBackColor = true;
            this.radLoggerTree.Click += new System.EventHandler(this.radLoggerTree_CheckedChanged);
            // 
            // radLoggerList
            // 
            this.radLoggerList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radLoggerList.AutoSize = true;
            this.radLoggerList.Checked = true;
            this.radLoggerList.Location = new System.Drawing.Point(382, 94);
            this.radLoggerList.Name = "radLoggerList";
            this.radLoggerList.Size = new System.Drawing.Size(41, 17);
            this.radLoggerList.TabIndex = 22;
            this.radLoggerList.TabStop = true;
            this.radLoggerList.Text = "List";
            this.radLoggerList.UseVisualStyleBackColor = true;
            this.radLoggerList.CheckedChanged += new System.EventHandler(this.radLoggerList_CheckedChanged);
            // 
            // loggerTreeView
            // 
            this.loggerTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loggerTreeView.CheckBoxes = true;
            this.loggerTreeView.ForeColor = System.Drawing.SystemColors.WindowText;
            this.loggerTreeView.Location = new System.Drawing.Point(164, 3);
            this.loggerTreeView.Name = "loggerTreeView";
            treeNode1.Name = "Node0";
            treeNode1.Text = "Tree";
            treeNode2.Name = "Node1";
            treeNode2.Text = "Of";
            treeNode3.Name = "Node2";
            treeNode3.Text = "Loggers";
            this.loggerTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.loggerTreeView.Size = new System.Drawing.Size(212, 199);
            this.loggerTreeView.TabIndex = 21;
            this.loggerTreeView.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.loggerTreeView_BeforeCheck);
            this.loggerTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.loggerTreeView_AfterCheck);
            // 
            // invertLoggers
            // 
            this.invertLoggers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.invertLoggers.Location = new System.Drawing.Point(382, 64);
            this.invertLoggers.Name = "invertLoggers";
            this.invertLoggers.Size = new System.Drawing.Size(75, 23);
            this.invertLoggers.TabIndex = 20;
            this.invertLoggers.Text = "Invert";
            this.invertLoggers.UseVisualStyleBackColor = true;
            this.invertLoggers.Click += new System.EventHandler(this.invertLoggers_Click);
            // 
            // uncheckAllLoggers
            // 
            this.uncheckAllLoggers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uncheckAllLoggers.Location = new System.Drawing.Point(382, 35);
            this.uncheckAllLoggers.Name = "uncheckAllLoggers";
            this.uncheckAllLoggers.Size = new System.Drawing.Size(75, 23);
            this.uncheckAllLoggers.TabIndex = 19;
            this.uncheckAllLoggers.Text = "Uncheck All";
            this.uncheckAllLoggers.UseVisualStyleBackColor = true;
            this.uncheckAllLoggers.Click += new System.EventHandler(this.uncheckAllLoggers_Click);
            // 
            // checkAllLoggers
            // 
            this.checkAllLoggers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkAllLoggers.Location = new System.Drawing.Point(382, 6);
            this.checkAllLoggers.Name = "checkAllLoggers";
            this.checkAllLoggers.Size = new System.Drawing.Size(75, 23);
            this.checkAllLoggers.TabIndex = 18;
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
            this.loggerListView.Size = new System.Drawing.Size(155, 199);
            this.loggerListView.TabIndex = 17;
            this.loggerListView.UseCompatibleStateImageBehavior = false;
            this.loggerListView.View = System.Windows.Forms.View.Details;
            this.loggerListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.loggerListView_ColumnClick);
            this.loggerListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.loggerListView_ItemChecked);
            // 
            // loggerCheckCol
            // 
            this.loggerCheckCol.Text = "";
            this.loggerCheckCol.Width = 24;
            // 
            // loggerNameCol
            // 
            this.loggerNameCol.Text = "Logger Name";
            this.loggerNameCol.Width = 198;
            // 
            // ListAndTreeOfFilterable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.collapseLoggers);
            this.Controls.Add(this.expandLoggers);
            this.Controls.Add(this.radLoggerTree);
            this.Controls.Add(this.radLoggerList);
            this.Controls.Add(this.loggerTreeView);
            this.Controls.Add(this.invertLoggers);
            this.Controls.Add(this.uncheckAllLoggers);
            this.Controls.Add(this.checkAllLoggers);
            this.Controls.Add(this.loggerListView);
            this.Name = "ListAndTreeOfFilterable";
            this.Size = new System.Drawing.Size(465, 205);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button collapseLoggers;
        private System.Windows.Forms.Button expandLoggers;
        private System.Windows.Forms.RadioButton radLoggerTree;
        private System.Windows.Forms.RadioButton radLoggerList;
        private NoDblClickTreeView loggerTreeView;
        private System.Windows.Forms.Button invertLoggers;
        private System.Windows.Forms.Button uncheckAllLoggers;
        private System.Windows.Forms.Button checkAllLoggers;
        private System.Windows.Forms.ListView loggerListView;
        private System.Windows.Forms.ColumnHeader loggerCheckCol;
        private System.Windows.Forms.ColumnHeader loggerNameCol;
    }
}
