namespace MirageGUI.Forms
{
    partial class ConnectForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.RemotePort = new System.Windows.Forms.MaskedTextBox();
            this.RemoteHost = new System.Windows.Forms.TextBox();
            this.CancelButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.Login = new System.Windows.Forms.TextBox();
            this.Password = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.RememberPassword = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Remote Host:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Remote Port:";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(60, 150);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(75, 23);
            this.ConnectButton.TabIndex = 4;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // RemotePort
            // 
            this.RemotePort.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MirageGUI.MirageGUI.Default, "RemotePort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.RemotePort.Location = new System.Drawing.Point(91, 36);
            this.RemotePort.Mask = "00000";
            this.RemotePort.Name = "RemotePort";
            this.RemotePort.Size = new System.Drawing.Size(100, 20);
            this.RemotePort.TabIndex = 5;
            this.RemotePort.Text = global::MirageGUI.MirageGUI.Default.RemotePort;
            this.RemotePort.ValidatingType = typeof(int);
            // 
            // RemoteHost
            // 
            this.RemoteHost.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MirageGUI.MirageGUI.Default, "RemoteHost", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.RemoteHost.Location = new System.Drawing.Point(91, 9);
            this.RemoteHost.Name = "RemoteHost";
            this.RemoteHost.Size = new System.Drawing.Size(189, 20);
            this.RemoteHost.TabIndex = 2;
            this.RemoteHost.Text = global::MirageGUI.MirageGUI.Default.RemoteHost;
            // 
            // CancelButton
            // 
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(157, 149);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 6;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Login:";
            // 
            // Login
            // 
            this.Login.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MirageGUI.MirageGUI.Default, "Login", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Login.Location = new System.Drawing.Point(91, 62);
            this.Login.Name = "Login";
            this.Login.Size = new System.Drawing.Size(100, 20);
            this.Login.TabIndex = 8;
            this.Login.Text = global::MirageGUI.MirageGUI.Default.Login;
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(91, 88);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(100, 20);
            this.Password.TabIndex = 9;
            this.Password.UseSystemPasswordChar = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Password:";
            // 
            // RememberPassword
            // 
            this.RememberPassword.AutoSize = true;
            this.RememberPassword.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.RememberPassword.Checked = global::MirageGUI.MirageGUI.Default.RememberPassword;
            this.RememberPassword.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::MirageGUI.MirageGUI.Default, "RememberPassword", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.RememberPassword.Location = new System.Drawing.Point(91, 114);
            this.RememberPassword.Name = "RememberPassword";
            this.RememberPassword.Size = new System.Drawing.Size(15, 14);
            this.RememberPassword.TabIndex = 11;
            this.RememberPassword.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(12, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 32);
            this.label5.TabIndex = 12;
            this.label5.Text = "Remember Password?";
            // 
            // ConnectForm
            // 
            this.AcceptButton = this.ConnectButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(291, 184);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.RememberPassword);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.Login);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.RemotePort);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.RemoteHost);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ConnectForm";
            this.Text = "Connect To Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox RemoteHost;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.MaskedTextBox RemotePort;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Login;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox RememberPassword;
        private System.Windows.Forms.Label label5;
    }
}