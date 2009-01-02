namespace TracerX.Viewer {
    partial class FullText {
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.edit = new System.Windows.Forms.CheckBox();
            this.Wrap = new System.Windows.Forms.CheckBox();
            this.copy = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.follow = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(652, 147);
            this.textBox1.TabIndex = 1;
            this.textBox1.WordWrap = false;
            // 
            // edit
            // 
            this.edit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.edit.AutoSize = true;
            this.edit.Location = new System.Drawing.Point(188, 153);
            this.edit.Name = "edit";
            this.edit.Size = new System.Drawing.Size(44, 17);
            this.edit.TabIndex = 2;
            this.edit.Text = "Edit";
            this.edit.UseVisualStyleBackColor = true;
            this.edit.CheckedChanged += new System.EventHandler(this.edit_CheckedChanged);
            // 
            // Wrap
            // 
            this.Wrap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Wrap.AutoSize = true;
            this.Wrap.Location = new System.Drawing.Point(130, 153);
            this.Wrap.Name = "Wrap";
            this.Wrap.Size = new System.Drawing.Size(52, 17);
            this.Wrap.TabIndex = 3;
            this.Wrap.Text = "Wrap";
            this.Wrap.UseVisualStyleBackColor = true;
            this.Wrap.CheckedChanged += new System.EventHandler(this.Wrap_CheckedChanged);
            // 
            // copy
            // 
            this.copy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.copy.Location = new System.Drawing.Point(238, 151);
            this.copy.Name = "copy";
            this.copy.Size = new System.Drawing.Size(75, 23);
            this.copy.TabIndex = 4;
            this.copy.Text = "Copy All";
            this.copy.UseVisualStyleBackColor = true;
            this.copy.Click += new System.EventHandler(this.copy_Click);
            // 
            // ok
            // 
            this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ok.Location = new System.Drawing.Point(319, 151);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 0;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // follow
            // 
            this.follow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.follow.AutoSize = true;
            this.follow.Location = new System.Drawing.Point(12, 153);
            this.follow.Name = "follow";
            this.follow.Size = new System.Drawing.Size(112, 17);
            this.follow.TabIndex = 5;
            this.follow.Text = "Follow current row";
            this.follow.UseVisualStyleBackColor = true;
            this.follow.CheckedChanged += new System.EventHandler(this.follow_CheckedChanged);
            // 
            // FullText
            // 
            this.AcceptButton = this.ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ok;
            this.ClientSize = new System.Drawing.Size(652, 178);
            this.Controls.Add(this.follow);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.copy);
            this.Controls.Add(this.Wrap);
            this.Controls.Add(this.edit);
            this.Controls.Add(this.textBox1);
            this.MinimumSize = new System.Drawing.Size(320, 164);
            this.Name = "FullText";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TracerX Text Window";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox edit;
        private System.Windows.Forms.CheckBox Wrap;
        private System.Windows.Forms.Button copy;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.CheckBox follow;


    }
}