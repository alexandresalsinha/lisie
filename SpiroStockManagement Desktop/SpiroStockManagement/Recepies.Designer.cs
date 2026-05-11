namespace SpiroStockManagement
{
    partial class Recepies
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Recepies));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Todas");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Categorias");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Cozinhas");
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.textBox_Search = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_New = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_Ingridients = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView_Categories = new System.Windows.Forms.TreeView();
            this.imageList_RecepiesPhotoHack = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip_Recepies = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.apagarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adicionarÁListaDeComprasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.listView_Recepies = new EXControls.EXListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip_Recepies.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(835, 533);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 4;
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel4, 2);
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 769F));
            this.tableLayoutPanel4.Controls.Add(this.textBox_Search, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.label1, 2, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 28);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(829, 31);
            this.tableLayoutPanel4.TabIndex = 7;
            // 
            // textBox_Search
            // 
            this.textBox_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Search.Location = new System.Drawing.Point(63, 5);
            this.textBox_Search.Name = "textBox_Search";
            this.textBox_Search.Size = new System.Drawing.Size(763, 20);
            this.textBox_Search.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Procurar";
            // 
            // toolStrip1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.toolStrip1, 2);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_New,
            this.toolStripButton_Ingridients});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(835, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_New
            // 
            this.toolStripButton_New.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_New.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_New.Image")));
            this.toolStripButton_New.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_New.Name = "toolStripButton_New";
            this.toolStripButton_New.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_New.Text = "Nova";
            this.toolStripButton_New.Click += new System.EventHandler(this.toolStripButton_New_Click);
            // 
            // toolStripButton_Ingridients
            // 
            this.toolStripButton_Ingridients.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_Ingridients.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Ingridients.Image")));
            this.toolStripButton_Ingridients.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Ingridients.Name = "toolStripButton_Ingridients";
            this.toolStripButton_Ingridients.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_Ingridients.Text = "Ingredientes";
            this.toolStripButton_Ingridients.Click += new System.EventHandler(this.toolStripButton_Ingridients_Click);
            // 
            // splitContainer1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.splitContainer1, 2);
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 65);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView_Categories);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listView_Recepies);
            this.splitContainer1.Size = new System.Drawing.Size(829, 465);
            this.splitContainer1.SplitterDistance = 111;
            this.splitContainer1.TabIndex = 8;
            // 
            // treeView_Categories
            // 
            this.treeView_Categories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_Categories.HideSelection = false;
            this.treeView_Categories.Location = new System.Drawing.Point(0, 0);
            this.treeView_Categories.Name = "treeView_Categories";
            treeNode1.Name = "Todas";
            treeNode1.Text = "Todas";
            treeNode2.Name = "Categorias";
            treeNode2.Text = "Categorias";
            treeNode3.Name = "Cozinhas";
            treeNode3.Text = "Cozinhas";
            this.treeView_Categories.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.treeView_Categories.Size = new System.Drawing.Size(111, 465);
            this.treeView_Categories.TabIndex = 1;
            this.treeView_Categories.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Categories_AfterSelect);
            // 
            // imageList_RecepiesPhotoHack
            // 
            this.imageList_RecepiesPhotoHack.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList_RecepiesPhotoHack.ImageSize = new System.Drawing.Size(16, 120);
            this.imageList_RecepiesPhotoHack.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // contextMenuStrip_Recepies
            // 
            this.contextMenuStrip_Recepies.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.apagarToolStripMenuItem,
            this.adicionarÁListaDeComprasToolStripMenuItem});
            this.contextMenuStrip_Recepies.Name = "contextMenuStrip_Recepies";
            this.contextMenuStrip_Recepies.Size = new System.Drawing.Size(229, 48);
            // 
            // apagarToolStripMenuItem
            // 
            this.apagarToolStripMenuItem.Name = "apagarToolStripMenuItem";
            this.apagarToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.apagarToolStripMenuItem.Text = "Apagar";
            this.apagarToolStripMenuItem.Click += new System.EventHandler(this.apagarToolStripMenuItem_Click);
            // 
            // adicionarÁListaDeComprasToolStripMenuItem
            // 
            this.adicionarÁListaDeComprasToolStripMenuItem.Name = "adicionarÁListaDeComprasToolStripMenuItem";
            this.adicionarÁListaDeComprasToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.adicionarÁListaDeComprasToolStripMenuItem.Text = "Adicionar á Lista de Compras";
            this.adicionarÁListaDeComprasToolStripMenuItem.Click += new System.EventHandler(this.adicionarÁListaDeComprasToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // listView_Recepies
            // 
            this.listView_Recepies.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView_Recepies.ControlPadding = 2;
            this.listView_Recepies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_Recepies.FullRowSelect = true;
            this.listView_Recepies.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView_Recepies.Location = new System.Drawing.Point(0, 0);
            this.listView_Recepies.Name = "listView_Recepies";
            this.listView_Recepies.OwnerDraw = true;
            this.listView_Recepies.Size = new System.Drawing.Size(714, 465);
            this.listView_Recepies.StateImageList = this.imageList_RecepiesPhotoHack;
            this.listView_Recepies.TabIndex = 2;
            this.listView_Recepies.UseCompatibleStateImageBehavior = false;
            this.listView_Recepies.View = System.Windows.Forms.View.Details;
            this.listView_Recepies.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_Recepies_KeyUp);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 0;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Receitas";
            this.columnHeader2.Width = 654;
            // 
            // Recepies
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Recepies";
            this.Size = new System.Drawing.Size(835, 533);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip_Recepies.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TextBox textBox_Search;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripButton toolStripButton_New;
        private System.Windows.Forms.ToolStripButton toolStripButton_Ingridients;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView_Categories;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Recepies;
        private System.Windows.Forms.ToolStripMenuItem apagarToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList_RecepiesPhotoHack;
        private EXControls.EXListView listView_Recepies;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ToolStripMenuItem adicionarÁListaDeComprasToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}
