namespace SpiroStockManagement
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Recepies = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.sincronizaçãoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sincronizarAgoraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.definiçõesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateDeBaseDeDadosDevelopmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fazerUpdateAosIngredientesXmladicionarOsInexistentesDeReceitasJaExistentesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updatePrductsInformationOnlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer_CheckIfDatabaseChanged = new System.Windows.Forms.Timer(this.components);
            this.timer_checkFirebase = new System.Windows.Forms.Timer(this.components);
            this.buyList1 = new SpiroStockManagement.BuyList();
            this.stockList1 = new SpiroStockManagement.BuyList();
            this.buyList_Products = new SpiroStockManagement.BuyList();
            this.recepies1 = new SpiroStockManagement.Recepies();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.statusStrip1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(804, 518);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.toolStripButton4,
            this.toolStripSeparator2,
            this.toolStripButton_Recepies});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(804, 71);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(68, 68);
            this.toolStripButton2.Text = "Shopping Cart";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(68, 68);
            this.toolStripButton1.Text = "Inventory";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 71);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(68, 68);
            this.toolStripButton4.Text = "Products";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 71);
            // 
            // toolStripButton_Recepies
            // 
            this.toolStripButton_Recepies.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_Recepies.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Recepies.Image")));
            this.toolStripButton_Recepies.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Recepies.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Recepies.Name = "toolStripButton_Recepies";
            this.toolStripButton_Recepies.Size = new System.Drawing.Size(68, 68);
            this.toolStripButton_Recepies.Text = "Recepies";
            this.toolStripButton_Recepies.Visible = false;
            this.toolStripButton_Recepies.Click += new System.EventHandler(this.toolStripButton_Recepies_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 496);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(804, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "Em qulaquer momento passe um produto pelo scanner de codigo de barras";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(298, 17);
            this.toolStripStatusLabel1.Text = "At any Time scan a product with your bar code scanner";
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.ItemSize = new System.Drawing.Size(0, 1);
            this.tabControl1.Location = new System.Drawing.Point(3, 74);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(798, 419);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.buyList1);
            this.tabPage1.Location = new System.Drawing.Point(4, 5);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(790, 410);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Stock";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.stockList1);
            this.tabPage2.Location = new System.Drawing.Point(4, 5);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(790, 410);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Buy List";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.buyList_Products);
            this.tabPage3.Location = new System.Drawing.Point(4, 5);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(790, 410);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Products";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.recepies1);
            this.tabPage4.Location = new System.Drawing.Point(4, 5);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(790, 410);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sincronizaçãoToolStripMenuItem,
            this.updateDeBaseDeDadosDevelopmentToolStripMenuItem,
            this.updateToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(804, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // sincronizaçãoToolStripMenuItem
            // 
            this.sincronizaçãoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sincronizarAgoraToolStripMenuItem,
            this.definiçõesToolStripMenuItem});
            this.sincronizaçãoToolStripMenuItem.Name = "sincronizaçãoToolStripMenuItem";
            this.sincronizaçãoToolStripMenuItem.Size = new System.Drawing.Size(92, 20);
            this.sincronizaçãoToolStripMenuItem.Text = "Sincronização";
            this.sincronizaçãoToolStripMenuItem.Visible = false;
            // 
            // sincronizarAgoraToolStripMenuItem
            // 
            this.sincronizarAgoraToolStripMenuItem.Name = "sincronizarAgoraToolStripMenuItem";
            this.sincronizarAgoraToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.sincronizarAgoraToolStripMenuItem.Text = "Sincronizar Agora";
            this.sincronizarAgoraToolStripMenuItem.Click += new System.EventHandler(this.sincronizarAgoraToolStripMenuItem_Click);
            // 
            // definiçõesToolStripMenuItem
            // 
            this.definiçõesToolStripMenuItem.Name = "definiçõesToolStripMenuItem";
            this.definiçõesToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.definiçõesToolStripMenuItem.Text = "Definições";
            this.definiçõesToolStripMenuItem.Click += new System.EventHandler(this.definiçõesToolStripMenuItem_Click);
            // 
            // updateDeBaseDeDadosDevelopmentToolStripMenuItem
            // 
            this.updateDeBaseDeDadosDevelopmentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fazerUpdateAosIngredientesXmladicionarOsInexistentesDeReceitasJaExistentesToolStripMenuItem});
            this.updateDeBaseDeDadosDevelopmentToolStripMenuItem.Name = "updateDeBaseDeDadosDevelopmentToolStripMenuItem";
            this.updateDeBaseDeDadosDevelopmentToolStripMenuItem.Size = new System.Drawing.Size(239, 20);
            this.updateDeBaseDeDadosDevelopmentToolStripMenuItem.Text = "Update de Base de Dados ( development )";
            this.updateDeBaseDeDadosDevelopmentToolStripMenuItem.Visible = false;
            // 
            // fazerUpdateAosIngredientesXmladicionarOsInexistentesDeReceitasJaExistentesToolStripMenuItem
            // 
            this.fazerUpdateAosIngredientesXmladicionarOsInexistentesDeReceitasJaExistentesToolStripMenuItem.Name = "fazerUpdateAosIngredientesXmladicionarOsInexistentesDeReceitasJaExistentesToolStr" +
    "ipMenuItem";
            this.fazerUpdateAosIngredientesXmladicionarOsInexistentesDeReceitasJaExistentesToolStripMenuItem.Size = new System.Drawing.Size(457, 22);
            this.fazerUpdateAosIngredientesXmladicionarOsInexistentesDeReceitasJaExistentesToolStripMenuItem.Text = "Fazer update aos Ingredientes.Xml (adicionar os inexistentes das receitas)";
            this.fazerUpdateAosIngredientesXmladicionarOsInexistentesDeReceitasJaExistentesToolStripMenuItem.Click += new System.EventHandler(this.fazerUpdateAosIngredientesXmladicionarOsInexistentesDeReceitasJaExistentesToolStripMenuItem_Click);
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updatePrductsInformationOnlineToolStripMenuItem});
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.updateToolStripMenuItem.Text = "Update";
            // 
            // updatePrductsInformationOnlineToolStripMenuItem
            // 
            this.updatePrductsInformationOnlineToolStripMenuItem.Name = "updatePrductsInformationOnlineToolStripMenuItem";
            this.updatePrductsInformationOnlineToolStripMenuItem.Size = new System.Drawing.Size(266, 22);
            this.updatePrductsInformationOnlineToolStripMenuItem.Text = "Update Products Information Online";
            // 
            // timer_CheckIfDatabaseChanged
            // 
            this.timer_CheckIfDatabaseChanged.Interval = 5000;
            this.timer_CheckIfDatabaseChanged.Tick += new System.EventHandler(this.timer_CheckIfDatabaseChanged_Tick);
            // 
            // timer_checkFirebase
            // 
            this.timer_checkFirebase.Enabled = true;
            this.timer_checkFirebase.Interval = 5000;
            this.timer_checkFirebase.Tick += new System.EventHandler(this.timer_checkFirebase_Tick);
            // 
            // buyList1
            // 
            this.buyList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buyList1.Location = new System.Drawing.Point(3, 3);
            this.buyList1.Name = "buyList1";
            this.buyList1.Size = new System.Drawing.Size(784, 404);
            this.buyList1.TabIndex = 2;
            // 
            // stockList1
            // 
            this.stockList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stockList1.Location = new System.Drawing.Point(3, 3);
            this.stockList1.Name = "stockList1";
            this.stockList1.Size = new System.Drawing.Size(784, 404);
            this.stockList1.TabIndex = 1;
            // 
            // buyList_Products
            // 
            this.buyList_Products.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buyList_Products.Location = new System.Drawing.Point(0, 0);
            this.buyList_Products.Name = "buyList_Products";
            this.buyList_Products.Size = new System.Drawing.Size(790, 410);
            this.buyList_Products.TabIndex = 0;
            this.buyList_Products.UpdateStatusBar += new SpiroStockManagement.ChangeStatusBarEventHandler(this.buyList1_UpdateStatusBar);
            // 
            // recepies1
            // 
            this.recepies1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recepies1.Location = new System.Drawing.Point(3, 3);
            this.recepies1.Name = "recepies1";
            this.recepies1.Size = new System.Drawing.Size(784, 404);
            this.recepies1.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 542);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "Spiro Stock Management";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyDown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton_Recepies;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.TabPage tabPage3;
        private BuyList buyList_Products;
        private System.Windows.Forms.TabPage tabPage4;
        private Recepies recepies1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem sincronizaçãoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sincronizarAgoraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem definiçõesToolStripMenuItem;
        private BuyList buyList1;
        private BuyList stockList1;
        private System.Windows.Forms.ToolStripMenuItem updateDeBaseDeDadosDevelopmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fazerUpdateAosIngredientesXmladicionarOsInexistentesDeReceitasJaExistentesToolStripMenuItem;
        private System.Windows.Forms.Timer timer_CheckIfDatabaseChanged;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updatePrductsInformationOnlineToolStripMenuItem;
        private System.Windows.Forms.Timer timer_checkFirebase;
    }
}

