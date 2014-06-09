namespace DNSChecker
{
    partial class Form1
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
            this.lblAbfrageart = new System.Windows.Forms.Label();
            this.lblHost = new System.Windows.Forms.Label();
            this.lblServer = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.cboAbfrageart = new System.Windows.Forms.ComboBox();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblAbfrageart
            // 
            this.lblAbfrageart.AutoSize = true;
            this.lblAbfrageart.Location = new System.Drawing.Point(13, 27);
            this.lblAbfrageart.Name = "lblAbfrageart";
            this.lblAbfrageart.Size = new System.Drawing.Size(59, 13);
            this.lblAbfrageart.TabIndex = 0;
            this.lblAbfrageart.Text = "Abfrageart:";
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(13, 73);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(101, 13);
            this.lblHost.TabIndex = 1;
            this.lblHost.Text = "Hostname/Adresse:";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(12, 113);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(67, 13);
            this.lblServer.TabIndex = 2;
            this.lblServer.Text = "DNS-Server:";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(202, 145);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // cboAbfrageart
            // 
            this.cboAbfrageart.FormattingEnabled = true;
            this.cboAbfrageart.Location = new System.Drawing.Point(168, 19);
            this.cboAbfrageart.Name = "cboAbfrageart";
            this.cboAbfrageart.Size = new System.Drawing.Size(121, 21);
            this.cboAbfrageart.TabIndex = 4;
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(168, 70);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(152, 20);
            this.txtHost.TabIndex = 5;
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(168, 110);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(152, 20);
            this.txtServer.TabIndex = 6;
            this.txtServer.Text = "8.8.8.8";
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(440, 37);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.Size = new System.Drawing.Size(165, 109);
            this.txtResult.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(104, 189);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(413, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Hint: Please be careful with copy-paste. Often it will insert new lines without s" +
    "howing it.";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 211);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.txtHost);
            this.Controls.Add(this.cboAbfrageart);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.lblServer);
            this.Controls.Add(this.lblHost);
            this.Controls.Add(this.lblAbfrageart);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAbfrageart;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ComboBox cboAbfrageart;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label label1;


    }
}

