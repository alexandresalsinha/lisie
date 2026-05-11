using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpiroStockManagement
{
    public partial class ItemInfoAndChangeQuantity : Form
    {
        public SpiroStockManagmentDatabaseClass.Objects.Product CurrentProduct = null;
        string CurrentListName = "";
        public bool QuantityChanged = false;
        public int startX, startY = 0;

        public ItemInfoAndChangeQuantity()
        {
            InitializeComponent();
        }

        public void Initialize(SpiroStockManagmentDatabaseClass.Objects.Product product, string listName, bool showImage)
        {
            CurrentProduct = product;
            CurrentListName = listName;

            label_Brand.Text = product.Brand;
            label_Category.Text = product.categoryString;
            textBox_Name.Text = product.Name;
            label_InfoPackage.Text = product.PackageInfo;
            label_Price.Text = product.Price.ToString();
            label_PriceWeight.Text = product.VariableWeightPrice;
            label_barCode.Text = product.BarCode.ToString();

            //quantity info
            if (listName.ToLower() != "all")
            {
                if (listName.ToLower() == "in")
                {
                    numericUpDown_Quantity.Value = product.QuantityIn;
                    numericUpDown_quantityWeight.Value = decimal.Parse(product.QuantityWeightIn.ToString());
                }
                else
                {
                    numericUpDown_Quantity.Value = product.QuantityOut;
                    numericUpDown_quantityWeight.Value = decimal.Parse(product.QuantityWeightOut.ToString());
                }
            }
            else
            {
                //remove the quantity part
                groupBox1.Dispose();
                button_Change.Dispose();
            }

            if (showImage)
            {
                string _pathOfImage = GlobalVariables.ProductImagesPath + product.PictureSmallFilename;
                try
                {
                    pictureBox1.Image = Image.FromFile(_pathOfImage);
                }
                catch (Exception ex)
                {
                } 
            }
            else
            {
                tableLayoutPanel1.Controls.Remove(pictureBox1);
            }
            timer_CheckHover.Enabled = true;
            timer_CheckHover.Start();
        }

        private void timer_CheckHover_Tick(object sender, EventArgs e)
        {
            if (!this.ClientRectangle.Contains(PointToClient(Control.MousePosition)))
                this.Close();
        }

        
        private void button_Change_Click(object sender, EventArgs e)
        {
            if (CurrentListName.ToLower() == "in")
            {
                CurrentProduct.QuantityIn = int.Parse(numericUpDown_Quantity.Value.ToString());
                CurrentProduct.QuantityWeightIn = float.Parse(numericUpDown_quantityWeight.Value.ToString());
            }
            if (CurrentListName.ToLower() == "out")
            {
                CurrentProduct.QuantityOut = int.Parse(numericUpDown_Quantity.Value.ToString());
                CurrentProduct.QuantityWeightOut = float.Parse(numericUpDown_quantityWeight.Value.ToString());
            }
            GlobalVariables.SpiroStockManagmentDatabaseProcedures.InsertNewItem(CurrentProduct);
            QuantityChanged = true;
            this.Close();
        }

        private void numericUpDown_Quantity_ValueChanged(object sender, EventArgs e)
        {
            decimal _totalPrice = 0;
            if (numericUpDown_Quantity.Value > 0)
                _totalPrice += decimal.Parse(CurrentProduct.Price.ToString()) * numericUpDown_Quantity.Value;
            if (numericUpDown_quantityWeight.Value > 0)
                _totalPrice += numericUpDown_quantityWeight.Value *  decimal.Parse(CurrentProduct.VariableWeightPrice.Split('/')[0].ToString());

            if (_totalPrice.ToString().Length - (_totalPrice.ToString().IndexOf(',') + 1) == 3)
                label_TotalPrice.Text = _totalPrice.ToString().Remove(_totalPrice.ToString().Length - 1) + "€";
            else
            {
                label_TotalPrice.Text = _totalPrice.ToString() + "€";
            }
        }

        
        private void ItemInfoAndChangeQuantity_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(startX, startY);
            //MessageBox.Show(this.Location.X.ToString() + " " + this.Location.Y.ToString());
        }

    }
}
