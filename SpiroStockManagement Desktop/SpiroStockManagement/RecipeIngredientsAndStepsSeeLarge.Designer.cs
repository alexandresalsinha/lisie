namespace SpiroStockManagement
{
    partial class RecipeIngredientsAndStepsSeeLarge
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView_Ingredients = new SpiroStockManagement.DataGridViewWithReorder();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.label15 = new System.Windows.Forms.Label();
            this.numericUpDown_QuantityProportion = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dataGridView_Steps = new SpiroStockManagement.DataGridViewWithReorder();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Ingredients)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_QuantityProportion)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Steps)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Size = new System.Drawing.Size(553, 443);
            this.splitContainer1.SplitterDistance = 219;
            this.splitContainer1.TabIndex = 39;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel4);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(553, 219);
            this.groupBox1.TabIndex = 31;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ingredientes";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.dataGridView_Ingredients, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(547, 200);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // dataGridView_Ingredients
            // 
            this.dataGridView_Ingredients.AllowDrop = true;
            this.dataGridView_Ingredients.AllowUserToResizeRows = false;
            this.dataGridView_Ingredients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Ingredients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Ingredients.Location = new System.Drawing.Point(3, 38);
            this.dataGridView_Ingredients.Name = "dataGridView_Ingredients";
            this.dataGridView_Ingredients.ReadOnly = true;
            this.dataGridView_Ingredients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView_Ingredients.Size = new System.Drawing.Size(541, 159);
            this.dataGridView_Ingredients.TabIndex = 1;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 3;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 653F));
            this.tableLayoutPanel5.Controls.Add(this.label15, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.numericUpDown_QuantityProportion, 1, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(541, 29);
            this.tableLayoutPanel5.TabIndex = 2;
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 8);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(131, 13);
            this.label15.TabIndex = 2;
            this.label15.Text = "Quantidade em Proporção";
            // 
            // numericUpDown_QuantityProportion
            // 
            this.numericUpDown_QuantityProportion.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.numericUpDown_QuantityProportion.DecimalPlaces = 1;
            this.numericUpDown_QuantityProportion.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDown_QuantityProportion.Location = new System.Drawing.Point(140, 4);
            this.numericUpDown_QuantityProportion.Name = "numericUpDown_QuantityProportion";
            this.numericUpDown_QuantityProportion.Size = new System.Drawing.Size(35, 20);
            this.numericUpDown_QuantityProportion.TabIndex = 1;
            this.numericUpDown_QuantityProportion.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_QuantityProportion.ValueChanged += new System.EventHandler(this.numericUpDown_QuantityProportion_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataGridView_Steps);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(553, 220);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Passos";
            // 
            // dataGridView_Steps
            // 
            this.dataGridView_Steps.AllowDrop = true;
            this.dataGridView_Steps.AllowUserToResizeRows = false;
            this.dataGridView_Steps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Steps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Steps.Location = new System.Drawing.Point(3, 16);
            this.dataGridView_Steps.Name = "dataGridView_Steps";
            this.dataGridView_Steps.ReadOnly = true;
            this.dataGridView_Steps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView_Steps.Size = new System.Drawing.Size(547, 201);
            this.dataGridView_Steps.TabIndex = 0;
            // 
            // RecipeIngredientsAndStepsSeeLarge
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 443);
            this.Controls.Add(this.splitContainer1);
            this.Name = "RecipeIngredientsAndStepsSeeLarge";
            this.Text = "RecipeIngredientsAndStepsSeeLarge";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Ingredients)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_QuantityProportion)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Steps)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private DataGridViewWithReorder dataGridView_Ingredients;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown numericUpDown_QuantityProportion;
        private System.Windows.Forms.GroupBox groupBox2;
        private DataGridViewWithReorder dataGridView_Steps;
    }
}