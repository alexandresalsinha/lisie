using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
namespace SpiroStockManagement
{
    public partial class Ingredients : Form
    {
        SpiroStockManagmentDatabaseClass.Objects.Ingredient SelectedIngredient = null;
        List<SpiroStockManagmentDatabaseClass.Objects.Ingredient> IngredientsToShow = null;
        public Ingredients()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            InitializeProductsList();
            //InitializeProducts();
            InitializeIngredients();
        }

        public void Initialize(List<SpiroStockManagmentDatabaseClass.Objects.Ingredient> ingredientsToShow)
        {
            InitializeProductsList();
            //InitializeProducts();
            
            IngredientsToShow = ingredientsToShow;
            InitializeIngredients();
        }


        public void InitializeProductsList()
        {
            comboBox_Product.Items.Clear();
            foreach (SpiroStockManagmentDatabaseClass.Objects.AutoCompleteProductData _item in GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAutocompleteTextboxDate())
            {
                comboBox_Product.Items.Add(new ComboItem(_item.ProductName + " (" + _item.ProductBrand + ")", _item.ProductId));
            }

            comboBox_Product.SelectedIndex = 0;
        }
            //InitializeProducts();
        public void InitializeIngredients()
        {
            listView_Ingredients.Items.Clear();
            listView_Ingredients.BeginUpdate();
            if (IngredientsToShow != null)
            {
                foreach (SpiroStockManagmentDatabaseClass.Objects.Ingredient _ingredient in IngredientsToShow)
                {
                    ListViewItem _lvItem = new ListViewItem();
                    _lvItem.Text = _ingredient.Name;
                    _lvItem.Tag = _ingredient;
                    if (_ingredient.Products == null || _ingredient.Products.Count == 0)
                    {
                        _lvItem.ForeColor = Color.Red;
                    }
                    listView_Ingredients.Items.Add(_lvItem);
                }
            }
            else
            {
                foreach (XElement _XIngredient in GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllIngredients())
                {
                    SpiroStockManagmentDatabaseClass.Objects.Ingredient _currentIngredient = new SpiroStockManagmentDatabaseClass.Objects.Ingredient();
                    _currentIngredient = (SpiroStockManagmentDatabaseClass.Objects.Ingredient)SpiroStockManagmentDatabaseClass.XmlSerializerExtension.DeSerializerToObject(_XIngredient, _currentIngredient);
                    ListViewItem _lvItem = new ListViewItem();
                    _lvItem.Text = _currentIngredient.Name;
                    _lvItem.Tag = _currentIngredient;
                    if (_currentIngredient.Products == null || _currentIngredient.Products.Count == 0)
                    {
                        _lvItem.ForeColor = Color.Red;
                    }
                    listView_Ingredients.Items.Add(_lvItem);
                }
            }
            
            listView_Ingredients.EndUpdate();
            comboBox_Product.SelectedIndex = -1;
        }

        void InitializeIngredientProducts(SpiroStockManagmentDatabaseClass.Objects.Ingredient _ingredient)
        {
            if (_ingredient.Products != null)
            {
                if (_ingredient.Products.Count > 0)
                {
                    listView_IngredientsProducts.Items.Clear();
                    foreach (SpiroStockManagmentDatabaseClass.Objects.IngredientProduct _IngredientProduct in _ingredient.Products)
                    {
                        SpiroStockManagmentDatabaseClass.Objects.Product _ingredientProduct = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetProduct(_IngredientProduct.Id);
                        ListViewItem _lvItem = new ListViewItem();
                        _lvItem.Text = _ingredientProduct.Name + " (" + _ingredientProduct.Brand + ")";
                        _lvItem.Tag = _IngredientProduct;
                        listView_IngredientsProducts.Items.Add(_lvItem);
                    }
                }
                comboBox_Product.SelectedIndex = -1;
            }
        }

        private void toolStripButton_AddIngredient_Click(object sender, EventArgs e)
        {
            string _ingredientName = Microsoft.VisualBasic.Interaction.InputBox("Introduza o nome do ingrediente", "Novo Ingrediente");
            if (_ingredientName != string.Empty)
            {
                SpiroStockManagmentDatabaseClass.Objects.Ingredient _newIngredient = new SpiroStockManagmentDatabaseClass.Objects.Ingredient();
                _newIngredient.Name = _ingredientName;
                _newIngredient.Products = new List<SpiroStockManagmentDatabaseClass.Objects.IngredientProduct>();
                GlobalVariables.SpiroStockManagmentDatabaseProcedures.InsertEditIngredient(_newIngredient);
                InitializeIngredients();
            }
        }

        private void listView_Ingredients_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView_Ingredients.SelectedItems.Count > 0)
            {
                SpiroStockManagmentDatabaseClass.Objects.Ingredient _currentIngredient = (SpiroStockManagmentDatabaseClass.Objects.Ingredient)listView_Ingredients.SelectedItems[0].Tag;
                InitializeIngredientProducts(_currentIngredient);
                SelectedIngredient = _currentIngredient;
            }
            else
            {
                listView_IngredientsProducts.Items.Clear();
            }
        }

        private void button_AssociateProduct_Click(object sender, EventArgs e)
        {
            if (listView_Ingredients.SelectedItems.Count > 0)
            {
                SpiroStockManagmentDatabaseClass.Objects.Ingredient _currentIngredient = (SpiroStockManagmentDatabaseClass.Objects.Ingredient)listView_Ingredients.SelectedItems[0].Tag;
                if (_currentIngredient.Products == null)
                {
                    _currentIngredient.Products = new List<SpiroStockManagmentDatabaseClass.Objects.IngredientProduct>();
                }
                
                if (comboBox_Product.SelectedItem != null)
                {
                    _currentIngredient.Products.Add(new SpiroStockManagmentDatabaseClass.Objects.IngredientProduct{ Id = int.Parse((comboBox_Product.SelectedItem as ComboItem).ProductID.ToString())});
                    GlobalVariables.SpiroStockManagmentDatabaseProcedures.InsertEditIngredient(_currentIngredient);
                    listView_Ingredients.SelectedItems[0].Tag = _currentIngredient;
                    listView_Ingredients.SelectedItems[0].ForeColor = System.Drawing.SystemColors.ControlText;
                    InitializeIngredientProducts(_currentIngredient);
                }
            }
        }

        private void toolStripButton_DeleteIngredient_Click(object sender, EventArgs e)
        {

        }

        private void comboBox_Product_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_Product.SelectedItem != null)
            {
                button_AssociateProduct.Enabled = true;
            }
        }

        private void toolStripButton_DeleteIngredientProduct_Click(object sender, EventArgs e)
        {
            if (SelectedIngredient != null && listView_IngredientsProducts.SelectedItems.Count > 0)
            {
                foreach (ListViewItem _ListViewItem in listView_IngredientsProducts.SelectedItems)
                {
                    SpiroStockManagmentDatabaseClass.Objects.IngredientProduct _IngredientProduct = (SpiroStockManagmentDatabaseClass.Objects.IngredientProduct)_ListViewItem.Tag;
                    if (_IngredientProduct != null)
                    {
                        SelectedIngredient.Products.Remove(_IngredientProduct);
                    }
                }
                GlobalVariables.SpiroStockManagmentDatabaseProcedures.InsertEditIngredient(SelectedIngredient);
                InitializeIngredientProducts(SelectedIngredient);
            }
        }

        private void toolStripButton_NewProduct_Click(object sender, EventArgs e)
        {
            InsertItem _InsertItem = new InsertItem();
            _InsertItem.Initialize("all", comboBox_Product.Text);
            string _inputedText = comboBox_Product.Text;
            _InsertItem.ShowDialog();
            InitializeProductsList();
            comboBox_Product.Text = _inputedText;
            //comboBox_Product.te
        }
    }
}
