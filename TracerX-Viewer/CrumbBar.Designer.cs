namespace TracerX {
    partial class CrumbBar {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.leftBtn = new System.Windows.Forms.Button();
            this.rightBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkColor = System.Drawing.Color.Blue;
            this.linkLabel1.Location = new System.Drawing.Point(15, 0);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(55, 13);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "linkLabel1";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // leftBtn
            // 
            this.leftBtn.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.leftBtn.Location = new System.Drawing.Point(0, 0);
            this.leftBtn.Margin = new System.Windows.Forms.Padding(0);
            this.leftBtn.Name = "leftBtn";
            this.leftBtn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.leftBtn.Size = new System.Drawing.Size(15, 15);
            this.leftBtn.TabIndex = 7;
            this.leftBtn.UseVisualStyleBackColor = true;
            this.leftBtn.Visible = false;
            this.leftBtn.Paint += new System.Windows.Forms.PaintEventHandler(this.leftBtn_Paint);
            this.leftBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.leftBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
            this.leftBtn.EnabledChanged += new System.EventHandler(this.leftBtn_EnabledChanged);
            // 
            // rightBtn
            // 
            this.rightBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rightBtn.Location = new System.Drawing.Point(776, 0);
            this.rightBtn.Name = "rightBtn";
            this.rightBtn.Size = new System.Drawing.Size(15, 15);
            this.rightBtn.TabIndex = 8;
            this.rightBtn.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.rightBtn.UseVisualStyleBackColor = true;
            this.rightBtn.Visible = false;
            this.rightBtn.Paint += new System.Windows.Forms.PaintEventHandler(this.rightBtn_Paint);
            this.rightBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseDown);
            this.rightBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Btn_MouseUp);
            this.rightBtn.EnabledChanged += new System.EventHandler(this.leftBtn_EnabledChanged);
            // 
            // CrumbBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rightBtn);
            this.Controls.Add(this.leftBtn);
            this.Controls.Add(this.linkLabel1);
            this.Name = "CrumbBar";
            this.Size = new System.Drawing.Size(791, 15);
            this.Resize += new System.EventHandler(this.CrumbBar_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Button leftBtn;
        private System.Windows.Forms.Button rightBtn;
    }
}
