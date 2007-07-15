namespace MirageGUIClient
{
    partial class BuilderPane
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
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OuterSplit = new System.Windows.Forms.SplitContainer();
            this.InnerSplit = new System.Windows.Forms.SplitContainer();
            this.ConnectedLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.AreaTree = new System.Windows.Forms.TreeView();
            this.EditorTabs = new System.Windows.Forms.TabControl();
            this.StatusStrip.SuspendLayout();
            this.MainMenu.SuspendLayout();
            this.OuterSplit.Panel1.SuspendLayout();
            this.OuterSplit.SuspendLayout();
            this.InnerSplit.Panel1.SuspendLayout();
            this.InnerSplit.Panel2.SuspendLayout();
            this.InnerSplit.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConnectedLabel});
            this.StatusStrip.Location = new System.Drawing.Point(0, 341);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(558, 22);
            this.StatusStrip.TabIndex = 2;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(558, 24);
            this.MainMenu.TabIndex = 3;
            this.MainMenu.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileMenuItem.Text = "File";
            // 
            // connectMenuItem
            // 
            this.connectMenuItem.Name = "connectMenuItem";
            this.connectMenuItem.Size = new System.Drawing.Size(152, 22);
            this.connectMenuItem.Text = "Connect";
            this.connectMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // OuterSplit
            // 
            this.OuterSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OuterSplit.Location = new System.Drawing.Point(0, 24);
            this.OuterSplit.Name = "OuterSplit";
            this.OuterSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // OuterSplit.Panel1
            // 
            this.OuterSplit.Panel1.Controls.Add(this.InnerSplit);
            this.OuterSplit.Size = new System.Drawing.Size(558, 317);
            this.OuterSplit.SplitterDistance = 158;
            this.OuterSplit.TabIndex = 4;
            // 
            // InnerSplit
            // 
            this.InnerSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InnerSplit.Location = new System.Drawing.Point(0, 0);
            this.InnerSplit.Name = "InnerSplit";
            // 
            // InnerSplit.Panel1
            // 
            this.InnerSplit.Panel1.Controls.Add(this.AreaTree);
            // 
            // InnerSplit.Panel2
            // 
            this.InnerSplit.Panel2.Controls.Add(this.EditorTabs);
            this.InnerSplit.Size = new System.Drawing.Size(558, 158);
            this.InnerSplit.SplitterDistance = 129;
            this.InnerSplit.TabIndex = 1;
            // 
            // ConnectedLabel
            // 
            this.ConnectedLabel.Name = "ConnectedLabel";
            this.ConnectedLabel.Size = new System.Drawing.Size(79, 17);
            this.ConnectedLabel.Text = "Not Connected";
            // 
            // AreaTree
            // 
            this.AreaTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AreaTree.Location = new System.Drawing.Point(0, 0);
            this.AreaTree.Name = "AreaTree";
            this.AreaTree.Size = new System.Drawing.Size(129, 158);
            this.AreaTree.TabIndex = 0;
            this.AreaTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.AreaTree_NodeMouseDoubleClick);
            // 
            // EditorTabs
            // 
            this.EditorTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EditorTabs.Location = new System.Drawing.Point(0, 0);
            this.EditorTabs.Name = "EditorTabs";
            this.EditorTabs.SelectedIndex = 0;
            this.EditorTabs.Size = new System.Drawing.Size(425, 158);
            this.EditorTabs.TabIndex = 0;
            // 
            // BuilderPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 363);
            this.Controls.Add(this.OuterSplit);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.MainMenu);
            this.MainMenuStrip = this.MainMenu;
            this.Name = "BuilderPane";
            this.Text = "BuilderPane";
            this.Load += new System.EventHandler(this.BuilderPane_Load);
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.OuterSplit.Panel1.ResumeLayout(false);
            this.OuterSplit.ResumeLayout(false);
            this.InnerSplit.Panel1.ResumeLayout(false);
            this.InnerSplit.Panel2.ResumeLayout(false);
            this.InnerSplit.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectMenuItem;
        private System.Windows.Forms.SplitContainer OuterSplit;
        private System.Windows.Forms.SplitContainer InnerSplit;
        private System.Windows.Forms.ToolStripStatusLabel ConnectedLabel;
        private System.Windows.Forms.TreeView AreaTree;
        private System.Windows.Forms.TabControl EditorTabs;
    }
}