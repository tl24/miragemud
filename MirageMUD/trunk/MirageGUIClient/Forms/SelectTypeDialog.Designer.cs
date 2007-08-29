namespace MirageGUI.Forms
{
    partial class SelectTypeDialog
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
            this.TypeTree = new System.Windows.Forms.TreeView();
            this.SelectBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TypeTree
            // 
            this.TypeTree.Location = new System.Drawing.Point(13, 13);
            this.TypeTree.Name = "TypeTree";
            this.TypeTree.Size = new System.Drawing.Size(267, 219);
            this.TypeTree.TabIndex = 0;
            // 
            // SelectBtn
            // 
            this.SelectBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SelectBtn.Location = new System.Drawing.Point(124, 238);
            this.SelectBtn.Name = "SelectBtn";
            this.SelectBtn.Size = new System.Drawing.Size(75, 23);
            this.SelectBtn.TabIndex = 1;
            this.SelectBtn.Text = "Select";
            this.SelectBtn.UseVisualStyleBackColor = true;
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(205, 238);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 2;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // SelectTypeDialog
            // 
            this.AcceptButton = this.SelectBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.SelectBtn);
            this.Controls.Add(this.TypeTree);
            this.Name = "SelectTypeDialog";
            this.Text = "SelectTypeDialog";
            this.Shown += new System.EventHandler(this.SelectTypeDialog_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView TypeTree;
        private System.Windows.Forms.Button SelectBtn;
        private System.Windows.Forms.Button CancelBtn;
    }
}