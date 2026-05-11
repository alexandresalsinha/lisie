namespace SpiroStockManagement
{
    partial class Ingredients
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ingredients));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_AddIngredient = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_DeleteIngredient = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.listView_Ingredients = new System.Windows.Forms.ListView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_NewProduct = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_DeleteIngredientProduct = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.button_AssociateProduct = new System.Windows.Forms.Button();
            this.comboBox_Product = new System.Windows.Forms.ComboBox();
            this.listView_IngredientsProducts = new System.Windows.Forms.ListView();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(686, 547);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(327, 541);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Lista de Ingredientes";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.listView_Ingredients, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(321, 522);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_AddIngredient,
            this.toolStripButton2,
            this.toolStripButton_DeleteIngredient});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(321, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_AddIngredient
            // 
            this.toolStripButton_AddIngredient.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripButton_AddIngredient.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_AddIngredient.Image")));
            this.toolStripButton_AddIngredient.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_AddIngredient.Name = "toolStripButton_AddIngredient";
            this.toolStripButton_AddIngredient.Size = new System.Drawing.Size(56, 22);
            this.toolStripButton_AddIngredient.Text = "Novo";
            this.toolStripButton_AddIngredient.Click += new System.EventHandler(this.toolStripButton_AddIngredient_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(57, 22);
            this.toolStripButton2.Text = "Editar";
            // 
            // toolStripButton_DeleteIngredient
            // 
            this.toolStripButton_DeleteIngredient.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripButton_DeleteIngredient.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_DeleteIngredient.Image")));
            this.toolStripButton_DeleteIngredient.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_DeleteIngredient.Name = "toolStripButton_DeleteIngredient";
            this.toolStripButton_DeleteIngredient.Size = new System.Drawing.Size(65, 22);
            this.toolStripButton_DeleteIngredient.Text = "Apagar";
            this.toolStripButton_DeleteIngredient.Click += new System.EventHandler(this.toolStripButton_DeleteIngredient_Click);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.textBox1, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 28);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(315, 29);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Procurar";
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(56, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(256, 20);
            this.textBox1.TabIndex = 1;
            // 
            // listView_Ingredients
            // 
            this.listView_Ingredients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_Ingredients.HideSelection = false;
            this.listView_Ingredients.Location = new System.Drawing.Point(3, 63);
            this.listView_Ingredients.MultiSelect = false;
            this.listView_Ingredients.Name = "listView_Ingredients";
            this.listView_Ingredients.Size = new System.Drawing.Size(315, 456);
            this.listView_Ingredients.TabIndex = 2;
            this.listView_Ingredients.UseCompatibleStateImageBehavior = false;
            this.listView_Ingredients.View = System.Windows.Forms.View.List;
            this.listView_Ingredients.SelectedIndexChanged += new System.EventHandler(this.listView_Ingredients_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel3);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.groupBox2.Location = new System.Drawing.Point(356, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(327, 541);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Produtos Associados";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.toolStrip2, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.listView_IngredientsProducts, 0, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(321, 522);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_NewProduct,
            this.toolStripButton_DeleteIngredientProduct});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(321, 25);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButton_NewProduct
            // 
            this.toolStripButton_NewProduct.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripButton_NewProduct.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_NewProduct.Image")));
            this.toolStripButton_NewProduct.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_NewProduct.Name = "toolStripButton_NewProduct";
            this.toolStripButton_NewProduct.Size = new System.Drawing.Size(102, 22);
            this.toolStripButton_NewProduct.Text = "Novo Produto";
            this.toolStripButton_NewProduct.Click += new System.EventHandler(this.toolStripButton_NewProduct_Click);
            // 
            // toolStripButton_DeleteIngredientProduct
            // 
            this.toolStripButton_DeleteIngredientProduct.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripButton_DeleteIngredientProduct.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_DeleteIngredientProduct.Image")));
            this.toolStripButton_DeleteIngredientProduct.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_DeleteIngredientProduct.Name = "toolStripButton_DeleteIngredientProduct";
            this.toolStripButton_DeleteIngredientProduct.Size = new System.Drawing.Size(65, 22);
            this.toolStripButton_DeleteIngredientProduct.Text = "Apagar";
            this.toolStripButton_DeleteIngredientProduct.Click += new System.EventHandler(this.toolStripButton_DeleteIngredientProduct_Click);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.button_AssociateProduct, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.comboBox_Product, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 28);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(315, 29);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // button_AssociateProduct
            // 
            this.button_AssociateProduct.Enabled = false;
            this.button_AssociateProduct.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_AssociateProduct.Location = new System.Drawing.Point(237, 3);
            this.button_AssociateProduct.Name = "button_AssociateProduct";
            this.button_AssociateProduct.Size = new System.Drawing.Size(75, 23);
            this.button_AssociateProduct.TabIndex = 2;
            this.button_AssociateProduct.Text = "Associar";
            this.button_AssociateProduct.UseVisualStyleBackColor = true;
            this.button_AssociateProduct.Click += new System.EventHandler(this.button_AssociateProduct_Click);
            // 
            // comboBox_Product
            // 
            this.comboBox_Product.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBox_Product.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox_Product.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox_Product.FormattingEnabled = true;
            this.comboBox_Product.Location = new System.Drawing.Point(3, 3);
            this.comboBox_Product.Name = "comboBox_Product";
            this.comboBox_Product.Size = new System.Drawing.Size(228, 21);
            this.comboBox_Product.TabIndex = 3;
            this.comboBox_Product.SelectedIndexChanged += new System.EventHandler(this.comboBox_Product_SelectedIndexChanged);
            // 
            // listView_IngredientsProducts
            // 
            this.listView_IngredientsProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_IngredientsProducts.FullRowSelect = true;
            this.listView_IngredientsProducts.HideSelection = false;
            this.listView_IngredientsProducts.Location = new System.Drawing.Point(3, 63);
            this.listView_IngredientsProducts.Name = "listView_IngredientsProducts";
            this.listView_IngredientsProducts.Size = new System.Drawing.Size(315, 456);
            this.listView_IngredientsProducts.TabIndex = 2;
            this.listView_IngredientsProducts.UseCompatibleStateImageBehavior = false;
            this.listView_IngredientsProducts.View = System.Windows.Forms.View.List;
            // 
            // Ingredients
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 547);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Ingredients";
            this.Text = "Ingredients";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton_AddIngredient;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton_DeleteIngredient;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ListView listView_Ingredients;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButton_DeleteIngredientProduct;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.ListView listView_IngredientsProducts;
        private System.Windows.Forms.Button button_AssociateProduct;
        private System.Windows.Forms.ComboBox comboBox_Product;
        private System.Windows.Forms.ToolStripButton toolStripButton_NewProduct;
    }
}