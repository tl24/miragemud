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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectTypeDialog));
            this.TypeTree = new System.Windows.Forms.TreeView();
            this.SelectBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.TreeImages = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // TypeTree
            // 
            this.TypeTree.ImageIndex = 0;
            this.TypeTree.ImageList = this.TreeImages;
            this.TypeTree.Location = new System.Drawing.Point(13, 13);
            this.TypeTree.Name = "TypeTree";
            this.TypeTree.SelectedImageIndex = 0;
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
            this.SelectBtn.Click += new System.EventHandler(this.SelectBtn_Click);
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
            // TreeImages
            // 
            this.TreeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TreeImages.ImageStream")));
            this.TreeImages.TransparentColor = System.Drawing.Color.Magenta;
            this.TreeImages.Images.SetKeyName(0, "class");
            this.TreeImages.Images.SetKeyName(1, "folder_open");
            this.TreeImages.Images.SetKeyName(2, "folder_closed");
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
        private System.Windows.Forms.ImageList TreeImages;
    }
}