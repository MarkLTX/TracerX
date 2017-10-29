namespace TracerX
{
    partial class StartServiceForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.radSpecifiedPort = new System.Windows.Forms.RadioButton();
            this.radDefaultPort = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radDoImpersonate = new System.Windows.Forms.RadioButton();
            this.radDontImpersonate = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(5, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(274, 47);
            this.label1.TabIndex = 0;
            this.label1.Text = "This will run the TracerX service within the viewer process so users on other com" +
    "puters can connect and view local log files.";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPort);
            this.groupBox1.Controls.Add(this.radSpecifiedPort);
            this.groupBox1.Controls.Add(this.radDefaultPort);
            this.groupBox1.Location = new System.Drawing.Point(8, 55);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 71);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Port";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(74, 41);
            this.txtPort.MaxLength = 5;
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(55, 20);
            this.txtPort.TabIndex = 2;
            // 
            // radSpecifiedPort
            // 
            this.radSpecifiedPort.AutoSize = true;
            this.radSpecifiedPort.Location = new System.Drawing.Point(6, 42);
            this.radSpecifiedPort.Name = "radSpecifiedPort";
            this.radSpecifiedPort.Size = new System.Drawing.Size(69, 17);
            this.radSpecifiedPort.TabIndex = 1;
            this.radSpecifiedPort.Text = "This port:";
            this.radSpecifiedPort.UseVisualStyleBackColor = true;
            // 
            // radDefaultPort
            // 
            this.radDefaultPort.AutoSize = true;
            this.radDefaultPort.Checked = true;
            this.radDefaultPort.Location = new System.Drawing.Point(6, 19);
            this.radDefaultPort.Name = "radDefaultPort";
            this.radDefaultPort.Size = new System.Drawing.Size(113, 17);
            this.radDefaultPort.TabIndex = 0;
            this.radDefaultPort.TabStop = true;
            this.radDefaultPort.Text = "Default port 25120";
            this.radDefaultPort.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radDoImpersonate);
            this.groupBox2.Controls.Add(this.radDontImpersonate);
            this.groupBox2.Location = new System.Drawing.Point(8, 142);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(268, 87);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Impersonation";
            // 
            // radDoImpersonate
            // 
            this.radDoImpersonate.AutoSize = true;
            this.radDoImpersonate.Location = new System.Drawing.Point(6, 60);
            this.radDoImpersonate.Name = "radDoImpersonate";
            this.radDoImpersonate.Size = new System.Drawing.Size(250, 17);
            this.radDoImpersonate.TabIndex = 2;
            this.radDoImpersonate.Text = "Impersonate remote caller when accessing files.";
            this.radDoImpersonate.UseVisualStyleBackColor = true;
            // 
            // radDontImpersonate
            // 
            this.radDontImpersonate.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.radDontImpersonate.Checked = true;
            this.radDontImpersonate.Location = new System.Drawing.Point(6, 19);
            this.radDontImpersonate.Name = "radDontImpersonate";
            this.radDontImpersonate.Size = new System.Drawing.Size(256, 35);
            this.radDontImpersonate.TabIndex = 1;
            this.radDontImpersonate.TabStop = true;
            this.radDontImpersonate.Text = "None: All operations execute in the current security context.";
            this.radDontImpersonate.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.radDontImpersonate.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(201, 239);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(93, 239);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(102, 23);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start Service";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // StartServiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 275);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "StartServiceForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Start TracerX Service";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.RadioButton radSpecifiedPort;
        private System.Windows.Forms.RadioButton radDefaultPort;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radDoImpersonate;
        private System.Windows.Forms.RadioButton radDontImpersonate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnStart;
    }
}