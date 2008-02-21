namespace MirageGUI.Forms
{
    partial class EditorForm
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
            this.TableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.EditButtonPanel = new System.Windows.Forms.Panel();
            this.CloseButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.SaveCloseButton = new System.Windows.Forms.Button();
            this.EditButtonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableLayout
            // 
            this.TableLayout.ColumnCount = 2;
            this.TableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayout.Location = new System.Drawing.Point(0, 0);
            this.TableLayout.Margin = new System.Windows.Forms.Padding(3, 3, 3, 35);
            this.TableLayout.Name = "TableLayout";
            this.TableLayout.Padding = new System.Windows.Forms.Padding(0, 0, 0, 35);
            this.TableLayout.RowCount = 1;
            this.TableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TableLayout.Size = new System.Drawing.Size(366, 267);
            this.TableLayout.TabIndex = 0;
            // 
            // EditButtonPanel
            // 
            this.EditButtonPanel.Controls.Add(this.CloseButton);
            this.EditButtonPanel.Controls.Add(this.CancelButton);
            this.EditButtonPanel.Controls.Add(this.SaveButton);
            this.EditButtonPanel.Controls.Add(this.SaveCloseButton);
            this.EditButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.EditButtonPanel.Location = new System.Drawing.Point(0, 237);
            this.EditButtonPanel.Name = "EditButtonPanel";
            this.EditButtonPanel.Size = new System.Drawing.Size(366, 30);
            this.EditButtonPanel.TabIndex = 1;
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.CloseButton.Location = new System.Drawing.Point(279, 3);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 3;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.Close_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.CancelButton.Location = new System.Drawing.Point(191, 3);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(82, 23);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.SaveButton.Location = new System.Drawing.Point(103, 3);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(82, 23);
            this.SaveButton.TabIndex = 1;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.Save_Click);
            // 
            // SaveCloseButton
            // 
            this.SaveCloseButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.SaveCloseButton.Location = new System.Drawing.Point(15, 3);
            this.SaveCloseButton.Name = "SaveCloseButton";
            this.SaveCloseButton.Size = new System.Drawing.Size(82, 23);
            this.SaveCloseButton.TabIndex = 0;
            this.SaveCloseButton.Text = "Save && Close";
            this.SaveCloseButton.UseVisualStyleBackColor = true;
            this.SaveCloseButton.Click += new System.EventHandler(this.SaveClose_Click);
            // 
            // EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 267);
            this.Controls.Add(this.EditButtonPanel);
            this.Controls.Add(this.TableLayout);
            this.Name = "EditorForm";
            this.Text = "EditorForm";
            this.Load += new System.EventHandler(this.EditorForm_Load);
            this.EditButtonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TableLayout;
        private System.Windows.Forms.Panel EditButtonPanel;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button SaveCloseButton;


    }
}