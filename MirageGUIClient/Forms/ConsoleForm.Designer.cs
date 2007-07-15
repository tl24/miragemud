namespace MirageGUI.Forms
{
    partial class ConsoleForm
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
            this.SendButton = new System.Windows.Forms.Button();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.OutputText = new System.Windows.Forms.TextBox();
            this.InputText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Output";
            // 
            // SendButton
            // 
            this.SendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SendButton.Location = new System.Drawing.Point(353, 5);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(50, 23);
            this.SendButton.TabIndex = 2;
            this.SendButton.Text = "Send";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // fontDialog1
            // 
            this.fontDialog1.ShowApply = true;
            this.fontDialog1.ShowColor = true;
            this.fontDialog1.Apply += new System.EventHandler(this.ApplyFont);
            // 
            // OutputText
            // 
            this.OutputText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputText.BackColor = global::MirageGUI.MirageGUI.Default.ConsoleBackColor;
            this.OutputText.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::MirageGUI.MirageGUI.Default, "ConsoleFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.OutputText.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::MirageGUI.MirageGUI.Default, "ConsoleBackColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.OutputText.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::MirageGUI.MirageGUI.Default, "ConsoleForeColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.OutputText.Font = global::MirageGUI.MirageGUI.Default.ConsoleFont;
            this.OutputText.ForeColor = global::MirageGUI.MirageGUI.Default.ConsoleForeColor;
            this.OutputText.Location = new System.Drawing.Point(55, 34);
            this.OutputText.Multiline = true;
            this.OutputText.Name = "OutputText";
            this.OutputText.ReadOnly = true;
            this.OutputText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.OutputText.Size = new System.Drawing.Size(348, 161);
            this.OutputText.TabIndex = 3;
            this.OutputText.WordWrap = false;
            // 
            // InputText
            // 
            this.InputText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.InputText.BackColor = global::MirageGUI.MirageGUI.Default.ConsoleBackColor;
            this.InputText.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::MirageGUI.MirageGUI.Default, "ConsoleFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.InputText.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::MirageGUI.MirageGUI.Default, "ConsoleBackColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.InputText.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::MirageGUI.MirageGUI.Default, "ConsoleForeColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.InputText.Font = global::MirageGUI.MirageGUI.Default.ConsoleFont;
            this.InputText.ForeColor = global::MirageGUI.MirageGUI.Default.ConsoleForeColor;
            this.InputText.Location = new System.Drawing.Point(55, 6);
            this.InputText.MaximumSize = new System.Drawing.Size(800, 20);
            this.InputText.MinimumSize = new System.Drawing.Size(100, 20);
            this.InputText.Name = "InputText";
            this.InputText.Size = new System.Drawing.Size(292, 20);
            this.InputText.TabIndex = 1;
            this.InputText.WordWrap = false;
            // 
            // ConsoleForm
            // 
            this.AcceptButton = this.SendButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 207);
            this.Controls.Add(this.SendButton);
            this.Controls.Add(this.OutputText);
            this.Controls.Add(this.InputText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(225, 150);
            this.Name = "ConsoleForm";
            this.Text = "Mirage Builder Console";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox InputText;
        private System.Windows.Forms.TextBox OutputText;
        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}

