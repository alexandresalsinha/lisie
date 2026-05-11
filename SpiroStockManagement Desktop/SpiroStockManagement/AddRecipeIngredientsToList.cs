using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SpiroStockManagement
{
    public partial class AddRecipeIngredientsToList : Form
    {
        List<string> IngredientsList = new List<string>();
        List<string> ReportInfo = new List<string>();

        public AddRecipeIngredientsToList()
        {
            InitializeComponent();
        }

        public SpiroStockManagmentDatabaseClass.Objects.Recipe CurrentRecipe = null;
        public void Initialize(SpiroStockManagmentDatabaseClass.Objects.Recipe recipe)
        {
            CurrentRecipe = recipe;
            textBox_RecipeName.Text = recipe.Name;
            checkBox1.Checked = Settings1.Default.BuyRecipeIngredientsAutoCompleteProductQuantity;

            InitializeIngredientsList();
            InitializeIngredients();
            CalculateTotalPrice();


            foreach (SpiroStockManagmentDatabaseClass.Objects.AutoCompleteProductData _item in GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAutocompleteTextboxDate())
            {
                productDataSet1.Product.AddProductRow(_item.ProductId, _item.ProductName);
                //_productListing.Items.Add(new ComboItem(_item.ProductName, _item.ProductId));

            }
        }

        void InitializeIngredientsList()
        {
            List<string> _IngredientList = new List<string>();
            foreach (XElement _XIngredient in GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllIngredients())
            {
                SpiroStockManagmentDatabaseClass.Objects.Ingredient _currentIngredient = new SpiroStockManagmentDatabaseClass.Objects.Ingredient();
                _currentIngredient = (SpiroStockManagmentDatabaseClass.Objects.Ingredient)SpiroStockManagmentDatabaseClass.XmlSerializerExtension.DeSerializerToObject(_XIngredient, _currentIngredient);
                _IngredientList.Add(_currentIngredient.Name);
            }
            IngredientsList = _IngredientList;
        }

        void InitializeIngredients()
        {
            dataGridView_IngredientsProducts.Rows.Clear();
            InitialIngredientsAmountValues.Clear();
            int _rowIndex = 0;
            foreach (SpiroStockManagmentDatabaseClass.Objects.RecipeIngredient _RecipeIngredient in CurrentRecipe.IngredientList)
            {
                float _tempFloat = 0;
                InitialIngredientsAmountValues.Add(_rowIndex, (float.TryParse(_RecipeIngredient.Amount.ToString(), out _tempFloat) ? _tempFloat : 0));

                DataGridViewRow dataGridRow = new DataGridViewRow();


                DataGridViewCheckBoxCell _ingredientIsToBuy = new DataGridViewCheckBoxCell();
                DataGridViewTextBoxCell _ingredientC = new DataGridViewTextBoxCell();
                DataGridViewTextBoxCell _ingredientQuantityC = new DataGridViewTextBoxCell();
                DataGridViewTextBoxCell _ingredientUnitC = new DataGridViewTextBoxCell();
                DataGridViewComboBoxCell _productListing = new DataGridViewComboBoxCell();
                DataGridViewIconTextCell _productPackageInfo = new DataGridViewIconTextCell();
                DataGridViewIconTextCell _productInventoryInfo = new DataGridViewIconTextCell();
                DataGridViewIconTextCell _productShoopingInfo = new DataGridViewIconTextCell();

                DataGridViewTextBoxCell _productQuantity = new DataGridViewTextBoxCell();
                DataGridViewComboBoxCell _productUnitListing = new DataGridViewComboBoxCell();

                DataGridViewTextBoxCell _productPrice = new DataGridViewTextBoxCell();

                _productInventoryInfo.Value = 0;
                _productShoopingInfo.Value = 0;
                _productQuantity.Value = 1;


                _ingredientIsToBuy.Value = true;

                _ingredientC.Value = _RecipeIngredient.Name;

                _ingredientQuantityC.Value = _RecipeIngredient.Amount;
                _ingredientUnitC.Value = _RecipeIngredient.Units;

                _productListing.DisplayMember = "ProductName";
                //_productListing.dat
                _productListing.ValueMember = "ProductID";


                _productListing.Items.Clear();

                List<SpiroStockManagmentDatabaseClass.Objects.IngredientProduct> _IngredientProducts = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetIngredient(_RecipeIngredient.Name).Products;
                if (_IngredientProducts.Count > 0)
                {
                    foreach (SpiroStockManagmentDatabaseClass.Objects.IngredientProduct _IngredientProduct in _IngredientProducts)
                    {
                        SpiroStockManagmentDatabaseClass.Objects.Product _product = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetProduct(_IngredientProduct.Id);
                        _productListing.Items.Add(new ComboItem(_product.Name + " (" + _product.Brand + ")", _product.Id));
                    }
                    if (_productListing.Items.Count > 0)
                    {
                        SpiroStockManagmentDatabaseClass.Objects.Product _selectedProduct = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetProduct((_productListing.Items[0] as ComboItem).ProductID);
                        _productListing.Value = _productListing.Items[0];
                        _productPackageInfo.Value = _selectedProduct.PackageInfo;
                        _productPrice.Value = _selectedProduct.Price.ToString() + "€";
                        //inventory info
                        string _QuantityInventoryItemText = "";
                        if (_selectedProduct.QuantityWeightIn > 0 && _selectedProduct.QuantityIn > 0)
                            _QuantityInventoryItemText = _selectedProduct.QuantityIn.ToString() + " uni + " + _selectedProduct.QuantityWeightIn + " kg";
                        else
                        {
                            if (_selectedProduct.QuantityIn > 0)
                                _QuantityInventoryItemText = _selectedProduct.QuantityIn.ToString();
                            if (_selectedProduct.QuantityWeightIn > 0)
                                _QuantityInventoryItemText = _selectedProduct.QuantityWeightIn + " kg";
                        }
                        _productInventoryInfo.Value = (_QuantityInventoryItemText != "") ? _QuantityInventoryItemText : "0";

                        //shooping info
                        string _QuantityShoopingItemText = "";
                        if (_selectedProduct.QuantityWeightIn > 0 && _selectedProduct.QuantityIn > 0)
                            _QuantityShoopingItemText = _selectedProduct.QuantityOut.ToString() + " uni + " + _selectedProduct.QuantityWeightOut + " kg";
                        else
                        {
                            if (_selectedProduct.QuantityOut > 0)
                                _QuantityShoopingItemText = _selectedProduct.QuantityOut.ToString();
                            if (_selectedProduct.QuantityWeightOut > 0)
                                _QuantityShoopingItemText = _selectedProduct.QuantityWeightOut + " kg";
                        }
                        _productShoopingInfo.Value = (_QuantityShoopingItemText != "") ? _QuantityShoopingItemText : "0";
                    }
                }
                else
                {
                    foreach (SpiroStockManagmentDatabaseClass.Objects.AutoCompleteProductData _item in GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAutocompleteTextboxDate())
                    {
                        _productListing.Items.Add(new ComboItem(_item.ProductName, _item.ProductId));
                    }
                    _productInventoryInfo.Value = "";
                    _productShoopingInfo.Value = "";
                    _productQuantity.Value = "";
                    _ingredientIsToBuy.Value = false;
                }

                //_productInventoryInfo.Value = "2";

                _productUnitListing.Items.Add("Unidade");
                _productUnitListing.Items.Add("Kg");
                _productUnitListing.Items.Add("L");
                _productUnitListing.Value = _productUnitListing.Items[0];


                dataGridRow.Cells.Add(_ingredientIsToBuy);
                dataGridRow.Cells.Add(_ingredientC);
                dataGridRow.Cells.Add(_ingredientQuantityC);
                dataGridRow.Cells.Add(_ingredientUnitC);
                dataGridRow.Cells.Add(_productListing);
                dataGridRow.Cells.Add(_productPackageInfo);
                dataGridRow.Cells.Add(_productInventoryInfo);
                dataGridRow.Cells.Add(_productShoopingInfo);
                dataGridRow.Cells.Add(_productQuantity);
                dataGridRow.Cells.Add(_productUnitListing);
                dataGridRow.Cells.Add(_productPrice);


                dataGridView_IngredientsProducts.Rows.Add(dataGridRow);
                _rowIndex++;
            }
            CalculateQuantityToBuyFromIngredientQuantity(-1);
        }

        void CalculateQuantityToBuyFromIngredientQuantity(int row)
        {
            if (!checkBox1.Checked) return;

            if (row == -1)
            {
                int _rowIndex = 0;
                foreach (DataGridViewRow _row in dataGridView_IngredientsProducts.Rows)
                {
                    CalculateQuantityToBuyFromIngredientQuantity(_rowIndex);
                    _rowIndex++;
                }
            }
            else
            {

                float _ingredientAmount, _ProductPackageAmount = 0;
                float.TryParse(dataGridView_IngredientsProducts.Rows[row].Cells[2].Value.ToString(), out _ingredientAmount);
                string _ingredientUnit = dataGridView_IngredientsProducts.Rows[row].Cells[3].Value.ToString();
                //int _selectedProductId = GetSelectedProductIdOfRow(row);
                string _ProductPackageInfo = (dataGridView_IngredientsProducts.Rows[row].Cells[5].Value != null) ? dataGridView_IngredientsProducts.Rows[row].Cells[5].Value.ToString() : string.Empty;
                string _ProductQuantityUnit = "";
                //float _ProductPackageQuantity = 0;

                string __ingredientUnit2 = _ingredientUnit.ToLower();
                if (__ingredientUnit2 != "g" && __ingredientUnit2 != "gr" && __ingredientUnit2 != "kg" && __ingredientUnit2 != "l" && __ingredientUnit2 != "ml")
                {
                    dataGridView_IngredientsProducts.Rows[row].Cells[8].Value = 1;
                    dataGridView_IngredientsProducts.Rows[row].Cells[9].Value = "Unidade";
                    return;
                }
                if (_ProductPackageInfo == string.Empty) return;

                if (_ProductPackageInfo.ToLower().IndexOf("kg") > -1)
                    _ProductQuantityUnit = "kg";
                else
                {
                    if (_ProductPackageInfo.ToLower().IndexOf("g") > -1 ||
                        _ProductPackageInfo.ToLower().IndexOf("gr") > -1)
                        _ProductQuantityUnit = "g";
                }

                if (_ProductPackageInfo.ToLower().IndexOf("ml") > -1)
                    _ProductQuantityUnit = "ml";
                else
                {
                    if (_ProductPackageInfo.ToLower().IndexOf("l") > -1 ||
                        _ProductPackageInfo.ToLower().IndexOf("lt") > -1)
                        _ProductQuantityUnit = "l";
                }

                //remove all letters os package info
                string _ProductPackageInfoWithoutLetters = "";
                for (int i = 0; i <= _ProductPackageInfo.Length - 1; i++)
                {
                    if (!Char.IsLetter(_ProductPackageInfo[i]) && _ProductPackageInfo[i] != ' ')
                        _ProductPackageInfoWithoutLetters += _ProductPackageInfo[i];
                }

                //if theres a plus symbol add the quantities
                if (_ProductPackageInfoWithoutLetters.IndexOf("+") > 0)
                {
                    string[] _splitted = _ProductPackageInfoWithoutLetters.Split('+');
                    if (_splitted.Length == 2)
                    {
                        float _value1, _value2 = 0;
                        float.TryParse(_splitted[0], out _value1);
                        float.TryParse(_splitted[0], out _value2);
                        _ProductPackageAmount = _value1 + _value2;
                    }
                }
                else
                {
                    float.TryParse(_ProductPackageInfoWithoutLetters.ToString(), out _ProductPackageAmount);
                }

                //make the conversions of ingredient quantity
                if (_ingredientUnit.ToLower() == "g" || _ingredientUnit.ToLower() == "ml")
                {
                    _ingredientAmount = _ingredientAmount * float.Parse("0,001");
                }
                //make the conversions to of product quantity
                if (_ProductQuantityUnit.ToLower() == "g" || _ProductQuantityUnit.ToLower() == "ml")
                {
                    _ProductPackageAmount = _ProductPackageAmount * float.Parse("0,001");
                }

                if (_ProductPackageInfo.IndexOf("=") > 0)
                {
                    dataGridView_IngredientsProducts.Rows[row].Cells[8].Value = _ingredientAmount;
                    dataGridView_IngredientsProducts.Rows[row].Cells[9].Value = "Kg";
                    return;
                }
                //calculate by units
                if (_ingredientAmount <= _ProductPackageAmount)
                {
                    dataGridView_IngredientsProducts.Rows[row].Cells[8].Value = 1;
                }
                else
                {
                    float _quantity = _ingredientAmount / _ProductPackageAmount;
                    int _quantityWithoutDecimal = int.Parse(Decimal.Round(decimal.Parse(_quantity.ToString())).ToString());
                    if (_quantity > _quantityWithoutDecimal)
                    {
                        _quantityWithoutDecimal += 1;
                    }
                    dataGridView_IngredientsProducts.Rows[row].Cells[8].Value = _quantityWithoutDecimal;
                }
            }
        }

        //right event
        private void dataGridView_IngredietnsProducts_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            string _currentColumnName = dataGridView_IngredientsProducts.Columns[dataGridView_IngredientsProducts.CurrentCell.ColumnIndex].HeaderText;
            if (e.Control is DataGridViewComboBoxEditingControl && _currentColumnName.ToLower() == "produto")
            {
                DataGridViewComboBoxEditingControl te =
                (DataGridViewComboBoxEditingControl)e.Control;
                //CurrentComboBox = (TextBox)sender;
                te.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                te.AutoCompleteSource = AutoCompleteSource.ListItems;
                //(e.Control as ComboBox).Tag = e.
                (e.Control as ComboBox).SelectedIndexChanged += new EventHandler(AddRecipeIngredientsToList_SelectedIndexChanged);
            }
            if (e.Control is DataGridViewComboBoxEditingControl && _currentColumnName.ToLower() == "unidade produto")
            {
                DataGridViewComboBoxEditingControl te =
                (DataGridViewComboBoxEditingControl)e.Control;
                //(e.Control as ComboBox).Tag = e.
                (e.Control as ComboBox).SelectedIndexChanged += new EventHandler(ProductUnityComboBox_SelectedIndexChanged);
            }
        }
        void ProductUnityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //return;
            //string _currentColumnName = dataGridView_IngredientsProducts.Columns[dataGridView_IngredientsProducts.CurrentRow .ColumnIndex].HeaderText;
            ComboBox _comboBox = (sender as ComboBox);
            if (checkBox1.Checked)
            {
                //DataGridViewComboBoxCell _DataGridViewComboBoxCellProductUnity = (DataGridViewComboBoxCell)dataGridView_IngredientsProducts.Rows[dataGridView_IngredientsProducts.CurrentRow.Index].Cells[8];
                DataGridViewTextBoxCell _DataGridViewTextBoxCellIIngredientQuantity = (DataGridViewTextBoxCell)dataGridView_IngredientsProducts.Rows[dataGridView_IngredientsProducts.CurrentRow.Index].Cells[2];
                DataGridViewTextBoxCell _DataGridViewTextBoxCellIngredientUnity = (DataGridViewTextBoxCell)dataGridView_IngredientsProducts.Rows[dataGridView_IngredientsProducts.CurrentRow.Index].Cells[3];

                //string _productUnity = _DataGridViewComboBoxCellProductUnity.Value.ToString().ToLower();
                string _productUnity = _comboBox.SelectedItem.ToString().ToLower();
                string _ingredientUnity = _DataGridViewTextBoxCellIngredientUnity.Value.ToString().ToLower();
                float _ingredientQuantity, _ProductQuantity = 0;
                float.TryParse(_DataGridViewTextBoxCellIIngredientQuantity.Value.ToString(), out _ingredientQuantity);

                switch (_productUnity)
                {
                    case "kg":
                        if (_ingredientUnity == "g")
                        {
                            _ProductQuantity = _ingredientQuantity * float.Parse("0,001");
                            break;
                        }
                        if (_ingredientUnity == "kg")
                            _ProductQuantity = _ingredientQuantity;

                        //reset value«
                        dataGridView_IngredientsProducts.Rows[dataGridView_IngredientsProducts.CurrentRow.Index].Cells[8].Value = 0;
                        break;
                    case "l":
                        if (_ingredientUnity == "ml")
                        {
                            _ProductQuantity = _ingredientQuantity * float.Parse("0,001");
                            break;
                        }
                        if (_ingredientUnity == "l" || _ingredientUnity == "lt")
                            _ProductQuantity = _ingredientQuantity;

                        //reset value
                        dataGridView_IngredientsProducts.Rows[dataGridView_IngredientsProducts.CurrentRow.Index].Cells[8].Value = 0;
                        break;
                    case "unidade":
                        CalculateQuantityToBuyFromIngredientQuantity(dataGridView_IngredientsProducts.CurrentRow.Index);
                        return;
                    default:
                        break;
                }
                if (_ProductQuantity != 0)
                {
                    dataGridView_IngredientsProducts.Rows[dataGridView_IngredientsProducts.CurrentRow.Index].Cells[8].Value = _ProductQuantity;
                }

            }
        }

        void AddRecipeIngredientsToList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //return;
            //string _currentColumnName = dataGridView_IngredientsProducts.Columns[dataGridView_IngredientsProducts.CurrentRow .ColumnIndex].HeaderText;
            ComboBox _comboBox = (sender as ComboBox);
            if (_comboBox.SelectedItem != null)
            {
                ComboItem _ComboItem = (_comboBox.SelectedItem as ComboItem);
                if (_ComboItem != null)
                {
                    SpiroStockManagmentDatabaseClass.Objects.Product _selectedProduct = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetProduct(_ComboItem.ProductID);
                    dataGridView_IngredientsProducts.Rows[dataGridView_IngredientsProducts.CurrentRow.Index].Cells[5].Value = _selectedProduct.PackageInfo;
                    //inventory info
                    string _QuantityInventoryItemText = "";
                    if (_selectedProduct.QuantityWeightIn > 0 && _selectedProduct.QuantityIn > 0)
                        _QuantityInventoryItemText = _selectedProduct.QuantityIn.ToString() + " uni + " + _selectedProduct.QuantityWeightIn + " kg";
                    else
                    {
                        if (_selectedProduct.QuantityIn > 0)
                            _QuantityInventoryItemText = _selectedProduct.QuantityIn.ToString();
                        if (_selectedProduct.QuantityWeightIn > 0)
                            _QuantityInventoryItemText = _selectedProduct.QuantityWeightIn + " kg";
                    }
                    //isToChangeProductInfo = true;
                    dataGridView_IngredientsProducts.Rows[dataGridView_IngredientsProducts.CurrentRow.Index].Cells[dataGridView_IngredientsProducts.CurrentCell.ColumnIndex + 2].Value = (_QuantityInventoryItemText != "") ? _QuantityInventoryItemText : "0";
                    //inventoryProductInfo = _QuantityInventoryItemText;
                    //_DataGridViewTextBoxCellInventory.Value = _QuantityInventoryItemText;

                    //shooping info
                    string _QuantityShoopingItemText = "";
                    if (_selectedProduct.QuantityWeightIn > 0 && _selectedProduct.QuantityIn > 0)
                        _QuantityShoopingItemText = _selectedProduct.QuantityOut.ToString() + " uni + " + _selectedProduct.QuantityWeightOut + " kg";
                    else
                    {
                        if (_selectedProduct.QuantityOut > 0)
                            _QuantityShoopingItemText = _selectedProduct.QuantityOut.ToString();
                        if (_selectedProduct.QuantityWeightOut > 0)
                            _QuantityShoopingItemText = _selectedProduct.QuantityWeightOut + " kg";
                    }
                    dataGridView_IngredientsProducts.Rows[dataGridView_IngredientsProducts.CurrentRow.Index].Cells[dataGridView_IngredientsProducts.CurrentCell.ColumnIndex + 3].Value = (_QuantityShoopingItemText != "") ? _QuantityShoopingItemText : "0";
                    shoopingProductInfo = _QuantityShoopingItemText;
                    //isToChangeProductInfo = false;

                    //TODO set quantity to one, further will have an Algorithm
                    dataGridView_IngredientsProducts.Rows[dataGridView_IngredientsProducts.CurrentRow.Index].Cells[0].Value = true;
                    dataGridView_IngredientsProducts.Rows[dataGridView_IngredientsProducts.CurrentRow.Index].Cells[8].Value = 1;
                }
            }
            //dataGridView_IngredientsProducts
        }

        private void button_Insert_Click(object sender, EventArgs e)
        {
            //do the validation
            List<SpiroStockManagmentDatabaseClass.Objects.Product> _productsToAdd = new List<SpiroStockManagmentDatabaseClass.Objects.Product>();
            Dictionary<string, int> _ingredientsProductsToAssociate = new Dictionary<string, int>();

            //insert products
            foreach (DataGridViewRow _row in dataGridView_IngredientsProducts.Rows)
            {
                if (_row.Cells[0].Value != null && bool.Parse(_row.Cells[0].Value.ToString()) == true)
                {
                    //get the data needed

                    DataGridViewTextBoxCell _IngredientName = _row.Cells[1] as DataGridViewTextBoxCell;
                    DataGridViewComboBoxCell _productListing = _row.Cells[4] as DataGridViewComboBoxCell;
                    DataGridViewTextBoxCell _productQuantity = _row.Cells[8] as DataGridViewTextBoxCell;
                    DataGridViewComboBoxCell _productUnitListing = _row.Cells[9] as DataGridViewComboBoxCell;

                    //check if a product for the ingredient has been selected
                    int _selectedProductId = -1;
                    if (_productListing.Value != null)
                    {
                        if (_productListing.Value.GetType() == typeof(int))
                        {
                            _selectedProductId = int.Parse(_productListing.Value.ToString());
                        }
                        if (_productListing.Value.GetType() == typeof(ComboItem))
                        {
                            _selectedProductId = (_productListing.Value as ComboItem).ProductID;
                        }
                    }
                    //ComboItem _selectedComboItem = (_productListing.Value != null) ? (_productListing.Value as ComboItem) : null;
                    //_selectedProductId = (_selectedComboItem != null) ? _selectedComboItem.ProductID : -1;
                    //if(_productListing.Value != null)
                    //    int.TryParse(_productListing.Value.ToString(), out _selectedProductId);
                    if (_selectedProductId == -1)
                    {
                        MessageBox.Show("Tem que tirar a selecção ou tem que escolher pelo menos um produto no ingrediente " + _IngredientName.Value.ToString());
                        dataGridView_IngredientsProducts.CurrentCell = _productListing;
                        dataGridView_IngredientsProducts.BeginEdit(true);
                        return;
                    }

                    //check if ingredient has products associated
                    List<SpiroStockManagmentDatabaseClass.Objects.IngredientProduct> _IngredientProducts = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetIngredient(_IngredientName.Value.ToString()).Products;
                    if (_IngredientProducts.Count == 0)
                    {
                        var _query = from c in _IngredientProducts where c.Id == _selectedProductId select c;
                        if (_query.Any() == false) _ingredientsProductsToAssociate.Add(_IngredientName.Value.ToString(), _selectedProductId);
                    }


                    SpiroStockManagmentDatabaseClass.Objects.Product _product = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetProduct(_selectedProductId);
                    if (_product != null)
                    {
                        SpiroStockManagmentDatabaseClass.Objects.Item _productItemList = new SpiroStockManagmentDatabaseClass.Objects.Item();

                        _productItemList.ListName = "out";

                        if (_productUnitListing.Value.ToString() == "Unidade")
                        {
                            try
                            {
                                if (_productQuantity.Value == null)
                                {
                                    MessageBox.Show("Tem que introduzir a quantidade do produto " + _product.Name);
                                    dataGridView_IngredientsProducts.CurrentCell = _productQuantity;
                                    dataGridView_IngredientsProducts.BeginEdit(true);
                                    return;
                                }
                                _product.QuantityOut += int.Parse(_productQuantity.Value.ToString());
                                _productItemList.Quantity = int.Parse(_productQuantity.Value.ToString());
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("A quantidade do produto " + _product.Name + " , tem que ser um numero inteiro");
                                dataGridView_IngredientsProducts.CurrentCell = _productQuantity;
                                dataGridView_IngredientsProducts.BeginEdit(true);
                                return;
                            }
                        }
                        else
                        {
                            try
                            {
                                _product.QuantityWeightOut += float.Parse(_productQuantity.Value.ToString());
                                _productItemList.QuantityWeight = float.Parse(_productQuantity.Value.ToString());
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("A quantidade do produto " + _product.Name + " , tem que ser um numero decimal. ex : 0.350");
                                dataGridView_IngredientsProducts.CurrentCell = _productQuantity;
                                dataGridView_IngredientsProducts.BeginEdit(true);
                                return;
                            }
                        }
                        _productItemList.InsertDate = DateTime.Now.ToString("s");
                        _product.History.Add(_productItemList);
                        _productsToAdd.Add(_product);

                    }
                }
            }
            foreach (SpiroStockManagmentDatabaseClass.Objects.Product _product in _productsToAdd)
            {
                GlobalVariables.SpiroStockManagmentDatabaseProcedures.InsertNewItem(_product);
                ReportInfo.Add(_product.Name + " adicionado á lista de compras");
            }
            foreach (KeyValuePair<string, int> _ingredientProductToAssociate in _ingredientsProductsToAssociate)
            {
                SpiroStockManagmentDatabaseClass.Objects.Ingredient _IngredientToUpdate = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetIngredient(_ingredientProductToAssociate.Key);
                _IngredientToUpdate.Products.Add(new SpiroStockManagmentDatabaseClass.Objects.IngredientProduct { Id = _ingredientProductToAssociate.Value });
                GlobalVariables.SpiroStockManagmentDatabaseProcedures.InsertEditIngredient(_IngredientToUpdate);

                ReportInfo.Add("Produto " + GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetProduct(_ingredientProductToAssociate.Value).Name + " associado ao ingrediente " + _ingredientProductToAssociate.Key);
            }
            var message = string.Join(Environment.NewLine, ReportInfo);
            MessageBox.Show(message);
            this.Close();
        }

        int GetSelectedProductIdOfRow(int row)
        {
            int _selectedProductId = -1;
            DataGridViewComboBoxCell _DataGridViewTextBoxCell = (DataGridViewComboBoxCell)dataGridView_IngredientsProducts.Rows[row].Cells[4];
            if (_DataGridViewTextBoxCell.Value != null)
            {
                if (_DataGridViewTextBoxCell.Value.GetType() == typeof(int))
                {
                    _selectedProductId = int.Parse(_DataGridViewTextBoxCell.Value.ToString());
                }
                if (_DataGridViewTextBoxCell.Value.GetType() == typeof(ComboItem))
                {
                    _selectedProductId = (_DataGridViewTextBoxCell.Value as ComboItem).ProductID;
                }
            }

            return _selectedProductId;

        }

        //private void dataGridView_IngredientsProducts_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
        //    return;
        //    if (e.ColumnIndex == 4 && e.RowIndex != -1)
        //    {
        //        DataGridViewComboBoxCell _DataGridViewTextBoxCell = (DataGridViewComboBoxCell)dataGridView_IngredientsProducts.Rows[e.RowIndex].Cells[e.ColumnIndex];
        //        DataGridViewTextBoxCell _DataGridViewTextBoxCellInventory = (DataGridViewTextBoxCell)dataGridView_IngredientsProducts.Rows[e.RowIndex].Cells[e.ColumnIndex + 1];
        //        DataGridViewTextBoxCell _DataGridViewTextBoxCellShooping = (DataGridViewTextBoxCell)dataGridView_IngredientsProducts.Rows[e.RowIndex].Cells[e.ColumnIndex + 1];

        //        //check if a product for the ingredient has been selected
        //        int _selectedProductId = GetSelectedProductIdOfRow(e.RowIndex);
        //        //if (_DataGridViewTextBoxCell.Value != null)
        //        //{
        //        //    if (_DataGridViewTextBoxCell.Value.GetType() == typeof(int))
        //        //    {
        //        //        _selectedProductId = int.Parse(_DataGridViewTextBoxCell.Value.ToString());
        //        //    }
        //        //    if (_DataGridViewTextBoxCell.Value.GetType() == typeof(ComboItem))
        //        //    {
        //        //        _selectedProductId = (_DataGridViewTextBoxCell.Value as ComboItem).ProductID;
        //        //    }
        //        //}

        //        if (_selectedProductId != -1)
        //        {
        //            SpiroStockManagmentDatabaseClass.Objects.Product _selectedProduct = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetProduct(_selectedProductId);

        //            //inventory info
        //            string _QuantityInventoryItemText = "";
        //            if (_selectedProduct.QuantityWeightIn > 0 && _selectedProduct.QuantityIn > 0)
        //                _QuantityInventoryItemText = _selectedProduct.QuantityIn.ToString() + " uni + " + _selectedProduct.QuantityWeightIn + " kg";
        //            else
        //            {
        //                if (_selectedProduct.QuantityIn > 0)
        //                    _QuantityInventoryItemText = _selectedProduct.QuantityIn.ToString();
        //                if (_selectedProduct.QuantityWeightIn > 0)
        //                    _QuantityInventoryItemText = _selectedProduct.QuantityWeightIn + " kg";
        //            }
        //            isToChangeProductInfo = true;
        //            //dataGridView_IngredientsProducts.Rows[_DataGridViewTextBoxCellInventory.RowIndex].Cells[_DataGridViewTextBoxCellInventory.ColumnIndex].Value = _QuantityInventoryItemText;
        //            inventoryProductInfo = _QuantityInventoryItemText;
        //            //_DataGridViewTextBoxCellInventory.Value = _QuantityInventoryItemText;

        //            //shooping info
        //            string _QuantityShoopingItemText = "";
        //            if (_selectedProduct.QuantityWeightIn > 0 && _selectedProduct.QuantityIn > 0)
        //                _QuantityShoopingItemText = _selectedProduct.QuantityOut.ToString() + " uni + " + _selectedProduct.QuantityWeightOut + " kg";
        //            else
        //            {
        //                if (_selectedProduct.QuantityOut > 0)
        //                    _QuantityShoopingItemText = _selectedProduct.QuantityOut.ToString();
        //                if (_selectedProduct.QuantityWeightOut > 0)
        //                    _QuantityShoopingItemText = _selectedProduct.QuantityWeightOut + " kg";
        //            }
        //            shoopingProductInfo = _QuantityShoopingItemText;
        //            //isToChangeProductInfo = false;
        //        }
        //    }
        //    //Procuct Unity change
        //    if (e.ColumnIndex == 9 && e.RowIndex != -1)
        //    {
        //        if (checkBox1.Checked)
        //        {
        //            DataGridViewComboBoxCell _DataGridViewComboBoxCellProductUnity = (DataGridViewComboBoxCell)dataGridView_IngredientsProducts.Rows[e.RowIndex].Cells[e.ColumnIndex];
        //            DataGridViewTextBoxCell _DataGridViewTextBoxCellIIngredientQuantity = (DataGridViewTextBoxCell)dataGridView_IngredientsProducts.Rows[e.RowIndex].Cells[2];
        //            DataGridViewTextBoxCell _DataGridViewTextBoxCellIngredientUnity = (DataGridViewTextBoxCell)dataGridView_IngredientsProducts.Rows[e.RowIndex].Cells[3];

        //            string _productUnity = _DataGridViewComboBoxCellProductUnity.Value.ToString().ToLower() ;
        //            string _ingredientUnity = _DataGridViewTextBoxCellIngredientUnity.Value.ToString().ToLower();
        //            float _ingredientQuantity, _ProductQuantity = 0;
        //            float.TryParse(_DataGridViewTextBoxCellIIngredientQuantity.Value.ToString(), out _ingredientQuantity);

        //            switch (_productUnity)
        //            {
        //                case "kg":
        //                    if (_ingredientUnity == "g")
        //                    {
        //                        _ProductQuantity = _ingredientQuantity * float.Parse("0,001");
        //                        break;
        //                    }
        //                    if (_ingredientUnity == "kg")
        //                        _ProductQuantity = _ingredientQuantity;

        //                    //reset value«
        //                    dataGridView_IngredientsProducts.Rows[e.RowIndex].Cells[8].Value = 0;
        //                    break;
        //                case "lt":
        //                    if (_ingredientUnity == "ml")
        //                    {
        //                        _ProductQuantity = _ingredientQuantity * float.Parse("0,001");
        //                        break;
        //                    }
        //                    if (_ingredientUnity == "l" || _ingredientUnity == "lt")
        //                        _ProductQuantity = _ingredientQuantity;

        //                    //reset value
        //                    dataGridView_IngredientsProducts.Rows[e.RowIndex].Cells[8].Value = 0;
        //                    break;
        //                case "unidade":
        //                    CalculateQuantityToBuyFromIngredientQuantity(e.RowIndex);
        //                    return;
        //                default:
        //                    break;
        //            }
        //            if (_ProductQuantity != 0)
        //            {
        //                dataGridView_IngredientsProducts.Rows[e.RowIndex].Cells[8].Value = _ProductQuantity;
        //            }
        //        }
        //    }

        //}


        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView_IngredientsProducts.Rows[1].Cells[5].Value = "2";
        }

        bool isToChangeProductInfo = false;
        string inventoryProductInfo, shoopingProductInfo = "";
        //private void dataGridView_IngredientsProducts_CellValidated(object sender, DataGridViewCellEventArgs e)
        //{
        //    return;
        //    //if (e.ColumnIndex == 4)
        //    //{
        //    //    int _selectedProductId = -1;
        //    //    DataGridViewComboBoxCell _DataGridViewTextBoxCell = (DataGridViewComboBoxCell)dataGridView_IngredientsProducts.Rows[e.RowIndex].Cells[e.ColumnIndex];
        //    //    if (_DataGridViewTextBoxCell.Value != null)
        //    //    {
        //    //        if (_DataGridViewTextBoxCell.Value.GetType() == typeof(int))
        //    //        {
        //    //            _selectedProductId = int.Parse(_DataGridViewTextBoxCell.Value.ToString());
        //    //        }
        //    //        if (_DataGridViewTextBoxCell.Value.GetType() == typeof(ComboItem))
        //    //        {
        //    //            _selectedProductId = (_DataGridViewTextBoxCell.Value as ComboItem).ProductID;
        //    //        }
        //    //    }

        //    //    if (_selectedProductId != -1)
        //    //    {
        //    //    }
        //    //}
        //    if (isToChangeProductInfo)
        //    {

        //        dataGridView_IngredientsProducts.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value = inventoryProductInfo;
        //        //inventoryProductInfo = "";

        //        dataGridView_IngredientsProducts.Rows[e.RowIndex].Cells[e.ColumnIndex + 2].Value = shoopingProductInfo;
        //        shoopingProductInfo = "";

        //        isToChangeProductInfo = false;
        //        //dataGridView_IngredientsProducts.Rows[1].Cells[5].Value = inventoryProductInfo;
        //    }
        //}

        Dictionary<int, float> InitialIngredientsAmountValues = new Dictionary<int, float>();
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_QuantityProportion.Value > 1 && numericUpDown_QuantityProportion.Value < 2)
            {
                numericUpDown_QuantityProportion.Increment = 1;
                numericUpDown_QuantityProportion.Value = 2;
            }
            if (numericUpDown_QuantityProportion.Value == 1)
            {
                numericUpDown_QuantityProportion.Increment = decimal.Parse("0,1");
            }
            int _rowIndex = 0;
            foreach (DataGridViewRow _row in dataGridView_IngredientsProducts.Rows)
            {
                float _currentAmountDecimal = (InitialIngredientsAmountValues.ContainsKey(_rowIndex)) ? InitialIngredientsAmountValues[_rowIndex] : 0;

                if (_currentAmountDecimal > 0)
                {
                    _row.Cells[2].Value = float.Parse(numericUpDown_QuantityProportion.Value.ToString()) * _currentAmountDecimal;
                }

                _rowIndex++;
            }
            CalculateQuantityToBuyFromIngredientQuantity(-1);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings1.Default.BuyRecipeIngredientsAutoCompleteProductQuantity = checkBox1.Checked;
            if (checkBox1.Checked) CalculateQuantityToBuyFromIngredientQuantity(-1);
            Settings1.Default.Save();
        }

        int _rowIndexToInvertBoolean = -1;
        void CalculateTotalPrice()
        {
            float _totalPrice = 0;
            foreach (DataGridViewRow _row in dataGridView_IngredientsProducts.Rows)
            {
                //hack becaiuse the last checkbox value changed is not saved, so we have to invert it
                bool _rowIndexIsInvertedValueAndIsAGo = false;
                //if (_rowIndexToInvertBoolean != -1)
                //{
                //    if (_row.Index == _rowIndexToInvertBoolean)
                //    {
                //        _rowIndexIsInvertedValueAndIsAGo = !bool.Parse(_row.Cells[0].Value.ToString());
                //    }
                //    _rowIndexToInvertBoolean = -1;
                //}
                if ((_row.Cells[0].Value != null && bool.Parse(_row.Cells[0].Value.ToString()) == true) || _rowIndexIsInvertedValueAndIsAGo)
                {

                    DataGridViewComboBoxCell _productListing = _row.Cells[4] as DataGridViewComboBoxCell;
                    DataGridViewTextBoxCell _productQuantityCell = _row.Cells[8] as DataGridViewTextBoxCell;
                    DataGridViewComboBoxCell _productUnitListing = _row.Cells[9] as DataGridViewComboBoxCell;
                    float _productQuantity = 0;
                    float.TryParse(_productQuantityCell.Value.ToString(), out _productQuantity);

                    //check if a product for the ingredient has been selected
                    int _selectedProductId = -1;
                    if (_productListing.Value != null)
                    {
                        if (_productListing.Value.GetType() == typeof(int))
                        {
                            _selectedProductId = int.Parse(_productListing.Value.ToString());
                        }
                        if (_productListing.Value.GetType() == typeof(ComboItem))
                        {
                            _selectedProductId = (_productListing.Value as ComboItem).ProductID;
                        }
                    }
                    //ComboItem _selectedComboItem = (_productListing.Value != null) ? (_productListing.Value as ComboItem) : null;
                    //_selectedProductId = (_selectedComboItem != null) ? _selectedComboItem.ProductID : -1;
                    //if(_productListing.Value != null)
                    //    int.TryParse(_productListing.Value.ToString(), out _selectedProductId);
                    if (_selectedProductId != -1)
                    {
                        SpiroStockManagmentDatabaseClass.Objects.Product _product = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetProduct(_selectedProductId);
                        //if (_product.QuantityOut > 0)
                        //    _totalPrice += _product.Price * _product.QuantityOut;

                        //if (_product.QuantityWeightOut > 0)
                        //{
                        //    float variableWeightByKg = GlobalProcedures.GetVariableWeightByKgOfString(_product.VariableWeightPrice);
                        //    _totalPrice += variableWeightByKg * _product.QuantityWeightOut;
                        //}
                        switch (_productUnitListing.Value.ToString().ToLower())
                        {
                            case "unidade":
                                _totalPrice += _product.Price * _productQuantity;
                                break;
                            case "kg":
                                if (_product.VariableWeightPrice.IndexOf("kg") > -1)
                                {
                                    float variableWeightByKg = GlobalProcedures.GetVariableWeightByKgOfString(_product.VariableWeightPrice);
                                    _totalPrice += variableWeightByKg * _productQuantity;
                                }
                                break;
                            case "l":
                                if (_product.VariableWeightPrice.IndexOf("l") > -1)
                                {
                                    float variableWeightByKg = GlobalProcedures.GetVariableWeightByKgOfString(_product.VariableWeightPrice);
                                    _totalPrice += variableWeightByKg * _productQuantity;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                label_TotalPrice.Text = _totalPrice.ToString() + "€";
            }
        }

        private void dataGridView_IngredientsProducts_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 8 && e.RowIndex != -1)
            {
                CalculateTotalPrice();
            }
            if (e.ColumnIndex == 0)
            {

                DataGridViewCheckBoxCell checkCell =
                    (DataGridViewCheckBoxCell)dataGridView_IngredientsProducts.
                    Rows[e.RowIndex].Cells[0];
                bool _checked = bool.Parse(checkCell.Value.ToString());
                //dataGridView_IngredientsProducts.Invalidate();
                _rowIndexToInvertBoolean = e.RowIndex;
                CalculateTotalPrice();
            }
            //if (e.ColumnIndex == 9 && e.RowIndex != -1)
            //{
            //    CalculateTotalPrice();
            //}
        }

        private void dataGridView_IngredientsProducts_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView_IngredientsProducts.IsCurrentCellDirty)
            {
                dataGridView_IngredientsProducts.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        //private void dataGridView_IngredientsProducts_DataError(object sender, DataGridViewDataErrorEventArgs e)
        //{

        //    //dataGridView_IngredientsProducts.EndEdit();
        //    e.ThrowException = false;
        //}

        //private void dataGridView_IngredientsProducts_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        //{
        //    if (dataGridView_IngredientsProducts.CurrentCell.OwningColumn is DataGridViewComboBoxColumn)
        //    {
        //        DataGridViewComboBoxEditingControl editingControl =
        //                 (DataGridViewComboBoxEditingControl)dataGridView_IngredientsProducts.EditingControl;
        //        e.Value = editingControl.SelectedItem;
        //        e.ParsingApplied = true;
        //    }
        //}

        //private void dataGridView_IngredientsProducts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        //{
        //    if (e.Value != null)
        //    {
        //        e.Value = e.Value.ToString();
        //        e.FormattingApplied = true;
        //    }
        //}
    }
}
