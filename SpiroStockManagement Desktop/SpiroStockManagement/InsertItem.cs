using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SpiroStockManagement
{


    public partial class InsertItem : Form
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(
              int hWnd,      // handle to destination window
              uint Msg,       // message
              long wParam,  // first message parameter
              long lParam   // second message parameter
              );


        bool _DataHasBeenChanged = false;
        public bool DataHasBeenChanged { get { return _DataHasBeenChanged; } set { _DataHasBeenChanged = value; } }

        public InsertItem()
        {
            InitializeComponent();
        }

        public void Initialize()
        {

            LoadCheckboxesState();

            checkBox_DontRemoveFromOtherList.Enabled = true;
            checkBox_DontRemoveFromOtherList.Checked = true;
            checkBox_RemoveFromOthersList.Enabled = true;


            comboBox_PriceCapacity.SelectedIndex = 0;

            autoCompleteMine1.Initialize(GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAutocompleteTextboxDate());
        }

        void LoadCheckboxesState()
        {
            //checkboxsAreInitilizing = true;

            //checkBox_RemoveFromOthersList.Checked = bool.Parse(Settings1.Default.ChekboxAddToStock);
            //checkBox_DontRemoveFromOtherList.Checked = bool.Parse(Settings1.Default.ChekboxRemoveFromStock);
            //checkBox2.Checked = bool.Parse(Settings1.Default.CheckboxAddToShoopingCart);
            //checkBox_RemoveFromShoopingList.Checked = bool.Parse(Settings1.Default.CheckboxRemoveFromShoopingCart);

            //checkboxsAreInitilizing = false;
        }

        bool checkboxsAreInitilizing = false;
        public void Initialize(string listName)
        {
            Initialize();
            CurrentListName = listName.ToLower();


            if (listName == "out")
            {
                //groupBox2.Text = "Adicionar á Lista de Compras";
                //this.Text = "Inserir Produto á Lista de Compras";
                groupBox2.Text = "Add to Shopping Cart";
                this.Text = "Insert Product to Shopping Cart";
            }

            if (listName == "in")
            {
                //groupBox2.Text = "Adicionar ao Inventário";
                //this.Text = "Inserir Produto ao Inventário";
                groupBox2.Text = "Add to Inventory";
                this.Text = "Insert Product in Inventory";
            }

            if (listName == "all")
            {
                groupBox2.Dispose();
                //button3.Text = "Salvar";
                button3.Text = "Save";
            }
        }

        public void Initialize(string listName, string searchString)
        {
            Initialize(listName);
            numericUpDown_Quantity.Value = 0;
        }

        public void Initialize(long barCode)
        {
            Initialize();

            SpiroStockManagmentDatabaseClass.Objects.Product _ItemExists = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetItemByBarCode(barCode.ToString());
            if (_ItemExists != null)
            {
                Initialize(_ItemExists);
            }
            else
                textBox_barCode.Text = barCode.ToString();
        }
        public void Initialize(long barCode, string listName)
        {
            Initialize();
            CurrentListName = listName.ToLower();

            SpiroStockManagmentDatabaseClass.Objects.Product _ItemExists = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetItemByBarCode(barCode.ToString());
            if (_ItemExists != null)
            {
                Initialize(_ItemExists);
            }
            else
                textBox_barCode.Text = barCode.ToString();
        }

        string CurrentListName = "";
        //bool DoNotInitialize = false;
        public void Initialize(SpiroStockManagmentDatabaseClass.Objects.Product item, string listName)
        {
            Initialize(listName);
            //DoNotInitialize = true;

            Initialize(item);
        }

        
        public void Initialize(SpiroStockManagmentDatabaseClass.Objects.Product item)
        {
            if (UpdateItem == null && textBox_barCode.Text != string.Empty && item.BarCode > 0)
            {
                //if (MessageBox.Show("Este produto já tem um código de barras. Quer substituir o existente pelo introduzido?", "Pergunta", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                if (MessageBox.Show("This product already has a bar code associated to it. Do you want to replace the existing one with the newly entered?", "Question", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    textBox_barCode.Text = item.BarCode.ToString();
            }

            UpdateItem = item;


            UpdateOthersListLabel();
            LoadCheckboxesState();

            //check if both checkboxes are checked
            //checkboxsAreInitilizing = true;
            //if (checkBox_RemoveFromOthersList.Checked && checkBox_DontRemoveFromOtherList.Enabled && checkBox_DontRemoveFromOtherList.Checked)
            //{
            //    checkBox_RemoveFromOthersList.Checked = true;
            //    checkBox_DontRemoveFromOtherList.Checked = false;
            //}
            //if (checkBox2.Checked && checkBox_RemoveFromShoopingList.Enabled && checkBox_RemoveFromShoopingList.Checked)
            //{
            //    checkBox2.Checked = true;
            //    checkBox_RemoveFromShoopingList.Checked = false;
            //}
            //checkboxsAreInitilizing = false;

            //if barcode is inserted but there is no current product associte the bar code


            textBox_Brand.Text = item.Brand;
            textBox_Category.Text = item.categoryString;
            textBox_InfoFrom.Text = item.InformationTakenFrom;
            textBox_MarketLink.Text = item.MarketItemUrl;
            autoCompleteMine1.TextInputted = item.Name;
            textBox_InfoPackage.Text = item.PackageInfo;
            textBox_barCode.Text = item.BarCode.ToString();
            numericUpDown_Price.Value = decimal.Parse(item.Price.ToString());
            //textBox_Price.Text = item.Price.ToString();


            //textBox_PriceWeight.Text = item.VariableWeightPrice;
            if (item.VariableWeightPrice != null && item.VariableWeightPrice != string.Empty)
            {
                string[] _array = item.VariableWeightPrice.Split('/');
                numericUpDown_PriceWeight.Value = decimal.Parse(_array[0]);
                if (_array[1].IndexOf("kg") > 0)
                    comboBox_PriceCapacity.SelectedIndex = comboBox_PriceCapacity.Items.IndexOf("kg");
                if (_array[1].IndexOf("l") > 0)
                    comboBox_PriceCapacity.SelectedIndex = comboBox_PriceCapacity.Items.IndexOf("l");

                //string variableWeightByKgString = item.VariableWeightPrice.Substring(2);
                //variableWeightByKgString = variableWeightByKgString.Substring(0, variableWeightByKgString.IndexOf('/'));
                //variableWeightByKgString = variableWeightByKgString.Trim();
                //float variableWeightByKg = float.Parse(variableWeightByKgString);
                //numericUpDown_PriceWeight.Value = decimal.Parse(variableWeightByKg.ToString());
                //if (item.VariableWeightPrice.IndexOf("kg") > 0)
                //    comboBox_PriceCapacity.SelectedIndex = comboBox_PriceCapacity.Items.IndexOf("kg");
                //if (item.VariableWeightPrice.IndexOf("l") > 0)
                //    comboBox_PriceCapacity.SelectedIndex = comboBox_PriceCapacity.Items.IndexOf("l"); 
            }
            UpdateCurrentPrice();

            pictureBox1.Image = null;
            string _pathOfImage = GlobalVariables.ProductImagesPath + item.PictureSmallFilename;
            try
            {
                Image _image = GlobalProcedures.ImageFromFileNoLock(_pathOfImage);


                Size _imageNewSize = GlobalProcedures.GetThumbnailSize(_image, 140);

                Image _ResizedImage = _image.GetThumbnailImage(_imageNewSize.Width, _imageNewSize.Height, null, IntPtr.Zero);
                pictureBox1.Image = _ResizedImage;
                pictureBox1.Height = _ResizedImage.Height;
                pictureBox1.Width = _ResizedImage.Width;
            }
            catch (Exception ex)
            {
            }
            //TypeOfOperation TODO - remember the last choosen
            //if (checkBox1.Checked) _newItem.TypeOfOperation = "in";
            //else _newItem.TypeOfOperation = "out";
        }




        public void UpdateOthersListLabel()
        {
            //label_StockProductInfo.Text = string.Empty;
            label_BuyListProductInfo.Text = string.Empty;
            if (UpdateItem != null)
            {
                //check the out list

                if (CurrentListName.ToLower() == "out" &&  (UpdateItem.QuantityIn > 0 || UpdateItem.QuantityWeightIn > 0))
                {
                    //label1_InfoRemoveFromOtherList.Text = "Este produto já existe no inventário. Com quantidade ";
                    label1_InfoRemoveFromOtherList.Text = "This product exists in the Inventory. With quantity ";
                    if (UpdateItem.QuantityIn > 0)
                    {
                        label1_InfoRemoveFromOtherList.Text += UpdateItem.QuantityIn.ToString();
                    }
                    else
                    {
                        label1_InfoRemoveFromOtherList.Text += UpdateItem.QuantityWeightIn.ToString() + " kg";
                    }
                    //label1_InfoRemoveFromOtherList.Text += ". Quer remover automaticamente do inventário com a mesma quantidade?";
                    label1_InfoRemoveFromOtherList.Text += ". Do you want to remove automatically of the Inventory with the same quantity?";
                    panel_Others.Visible = true;
                    return;
                    //checkBox_RemoveFromStock.Enabled = true;
                }
                //else
                //{
                //    checkBox_RemoveFromStock.Enabled = false;
                //}

                if (CurrentListName.ToLower() == "in" && (UpdateItem.QuantityOut > 0 || UpdateItem.QuantityWeightOut > 0))
                {
                    //label1_InfoRemoveFromOtherList.Text = "Este produto existe na lista de compras. Com quantidade ";
                    label1_InfoRemoveFromOtherList.Text = "This product exists in the Shopping Cart. With quantity ";
                    if (UpdateItem.QuantityOut > 0)
                    {
                        label1_InfoRemoveFromOtherList.Text += UpdateItem.QuantityOut.ToString();
                    }
                    else
                    {
                        label1_InfoRemoveFromOtherList.Text += UpdateItem.QuantityWeightOut.ToString() + " kg";
                    }
                    //label1_InfoRemoveFromOtherList.Text += ". Quer remover automaticamente da Lista de Compras com a mesma quantidade?";
                    label1_InfoRemoveFromOtherList.Text += ". . Do you want to remove automatically of the Shopping Cart with the same quantity?";
                    panel_Others.Visible = true;
                    return;
                }

                panel_Others.Visible = false;
                //else
                //{
                //    checkBox_RemoveFromShoopingList.Enabled = false;
                //}

            }
        }

        public SpiroStockManagmentDatabaseClass.Objects.Product UpdateItem = null;
        

        public void InsertCurrent()
        {
            //validation
            if (CurrentListName != "all" && numericUpDown_Quantity.Value == 0 && numericUpDown_quantityWeight.Value == 0)
            {
                //MessageBox.Show("Tem que introduzir uma quantidade para o produto. Em quantidade ou peso.");
                MessageBox.Show("You have do add the product quantity. In units ot weight.");
                return;
            }
            int _temp = 0;
            
            if (autoCompleteMine1.TextInputted == string.Empty || int.TryParse(autoCompleteMine1.TextInputted, out _temp))
            {
                this.Close();
                return;
            }

            //is a new insert
            if (UpdateItem == null)
            {
                
                //insert item to database
                SpiroStockManagmentDatabaseClass.Objects.Product _newItem = new SpiroStockManagmentDatabaseClass.Objects.Product
                {
                    Brand = textBox_Brand.Text,
                    categoryString = textBox_Category.Text,
                    InformationTakenFrom = textBox_InfoFrom.Text,
                    IsBlackListed = false,
                    MarketItemUrl = textBox_MarketLink.Text,
                    //Name = comboBox_Name.Text,
                    Name = autoCompleteMine1.TextInputted,
                    PackageInfo = textBox_InfoPackage.Text,
                    Price = float.Parse(numericUpDown_Price.Value.ToString())
                };

                if (numericUpDown_PriceWeight.Value > 0)
                    _newItem.VariableWeightPrice = numericUpDown_PriceWeight.Value.ToString() + "/" + comboBox_PriceCapacity.SelectedItem.ToString();
                _newItem.InsertDate = DateTime.Now.ToString("s");
                //barcode
                if (textBox_barCode.Text != string.Empty)
                {
                    _newItem.BarCode = long.Parse(textBox_barCode.Text);
                }

                //Product Quantity

                //is to remove from oposite list
                if (checkBox_RemoveFromOthersList.Checked)
                {
                    _newItem.QuantityIn = int.Parse(numericUpDown_Quantity.Value.ToString());
                    _newItem.QuantityWeightIn = float.Parse(numericUpDown_quantityWeight.Value.ToString());
                }

                //dont remove from oposite list
                if (checkBox_DontRemoveFromOtherList.Checked)
                {
                    _newItem.QuantityOut = int.Parse(numericUpDown_Quantity.Value.ToString());
                    _newItem.QuantityWeightOut = float.Parse(numericUpDown_quantityWeight.Value.ToString());
                }

                //Product Item History
                SpiroStockManagmentDatabaseClass.Objects.Item _productItemList = new SpiroStockManagmentDatabaseClass.Objects.Item();

                //List Name
                if (checkBox_RemoveFromOthersList.Checked) _productItemList.ListName = "in";
                else _productItemList.ListName = "out";

                //quantity
                _productItemList.Quantity = int.Parse(numericUpDown_Quantity.Value.ToString());
                _productItemList.QuantityWeight = float.Parse(numericUpDown_quantityWeight.Value.ToString());

                //Date
                _productItemList.InsertDate = DateTime.Now.ToString("s");

                //Add History to Product
                if (_newItem.History == null) _newItem.History = new List<SpiroStockManagmentDatabaseClass.Objects.Item>();
                _newItem.History.Add(_productItemList);



                //save image to folder
                int _newItemID = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetLastID() + 1;
                string _pathToSaveImage = "";
                //is to insert image
                if (UpdateItem == null)
                {
                    if (pictureBox1.Image != null)
                        _pathToSaveImage = GlobalVariables.ProductImagesPath + _newItemID.ToString() + "small." + pictureBox1.Tag.ToString();
                    _newItem.PictureSmallFilename = _pathToSaveImage.Substring(_pathToSaveImage.LastIndexOf('\\') + 1);
                }
                //is to update image
                else
                {
                    _pathToSaveImage = GlobalVariables.ProductImagesPath + UpdateItem.PictureSmallFilename;
                    _newItem.PictureSmallFilename = UpdateItem.PictureSmallFilename;
                }

                if (_pathToSaveImage != string.Empty)
                {
                    try
                    {
                        pictureBox1.Image.Save(_pathToSaveImage);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                GlobalVariables.SpiroStockManagmentDatabaseProcedures.InsertNewItem(_newItem);
                DataHasBeenChanged = true;
            }
            //is an update of quantity
            else
            {
                if (UpdateItem.History == null) UpdateItem.History = new List<SpiroStockManagmentDatabaseClass.Objects.Item>();


                UpdateItem.Brand = textBox_Brand.Text;
                UpdateItem.categoryString = textBox_Category.Text;
                UpdateItem.InformationTakenFrom = textBox_InfoFrom.Text;
                UpdateItem.MarketItemUrl = textBox_MarketLink.Text;
                UpdateItem.Name = autoCompleteMine1.TextInputted;
                UpdateItem.PackageInfo = textBox_InfoPackage.Text;
                UpdateItem.Price = float.Parse(numericUpDown_Price.Value.ToString());


                if (numericUpDown_PriceWeight.Value > 0)
                    UpdateItem.VariableWeightPrice = numericUpDown_PriceWeight.Value.ToString() + "/" + comboBox_PriceCapacity.SelectedItem.ToString();

                //barcode
                if (textBox_barCode.Text != string.Empty)
                {
                    UpdateItem.BarCode = long.Parse(textBox_barCode.Text);
                }

                if (CurrentListName != "all")
                {
                    int _quantityToUpdate = 0;
                    float _quantityWeightToUpdate = 0;
                    if (numericUpDown_Quantity.Value > 0)
                        _quantityToUpdate = int.Parse(numericUpDown_Quantity.Value.ToString());
                    if (numericUpDown_quantityWeight.Value > 0)
                        _quantityWeightToUpdate = float.Parse(numericUpDown_quantityWeight.Value.ToString());

                    //string _listName = "";
                    if (CurrentListName.ToLower() == "out")
                    {
                        UpdateItem.QuantityOut += _quantityToUpdate;
                        UpdateItem.QuantityWeightOut += _quantityWeightToUpdate;
                        if (checkBox_RemoveFromOthersList.Checked)
                        {
                            UpdateItem.QuantityIn -= _quantityToUpdate;
                            UpdateItem.QuantityWeightIn -= _quantityWeightToUpdate;
                        }
                    }

                    if (CurrentListName.ToLower() == "in")
                    {
                        UpdateItem.QuantityIn += _quantityToUpdate;
                        UpdateItem.QuantityWeightIn += _quantityWeightToUpdate;
                        if (checkBox_RemoveFromOthersList.Checked)
                        {
                            UpdateItem.QuantityOut -= _quantityToUpdate;
                            UpdateItem.QuantityWeightOut -= _quantityWeightToUpdate;
                        }
                    }

                    

                    //add to history
                    if (CurrentListName.ToLower() == "in")
                    {
                        SpiroStockManagmentDatabaseClass.Objects.Item _newUpdateItem = new SpiroStockManagmentDatabaseClass.Objects.Item();
                        _newUpdateItem.ListName = "in";

                        //quantity
                        _newUpdateItem.Quantity = _quantityToUpdate;
                        _newUpdateItem.QuantityWeight = _quantityWeightToUpdate;
                        _newUpdateItem.InsertDate = DateTime.Now.ToString("s");
                        UpdateItem.History.Add(_newUpdateItem); 
                    }
                    if (CurrentListName.ToLower() == "out")
                    {
                        SpiroStockManagmentDatabaseClass.Objects.Item _newUpdateItem = new SpiroStockManagmentDatabaseClass.Objects.Item();
                        _newUpdateItem.ListName = "out";

                        //quantity
                        _newUpdateItem.Quantity = _quantityToUpdate;
                        _newUpdateItem.QuantityWeight = _quantityWeightToUpdate;
                        _newUpdateItem.InsertDate = DateTime.Now.ToString("s");
                        UpdateItem.History.Add(_newUpdateItem);
                    }
                    if (checkBox_RemoveFromOthersList.Checked)
                    {
                        SpiroStockManagmentDatabaseClass.Objects.Item _newUpdateItem = new SpiroStockManagmentDatabaseClass.Objects.Item();
                        _newUpdateItem.ListName = (CurrentListName.ToLower() == "in") ?  "out" : "in" ;

                        //quantity
                        _newUpdateItem.Quantity = _quantityToUpdate;
                        _newUpdateItem.QuantityWeight = _quantityWeightToUpdate;
                        _newUpdateItem.InsertDate = DateTime.Now.ToString("s");
                        UpdateItem.History.Add(_newUpdateItem);
                    }
                }


                //Save Picture
                string _pathToSaveImage = "";
                //if (pictureBox1.Image != null)
                    //_pathToSaveImage = GlobalVariables.ProductImagesPath + UpdateItem.Id.ToString() + "small." + pictureBox1.Tag.ToString();
                UpdateItem.PictureSmallFilename = _pathToSaveImage.Substring(_pathToSaveImage.LastIndexOf('\\') + 1);

                if (_pathToSaveImage != string.Empty)
                {
                    try
                    {
                        pictureBox1.Image.Save(_pathToSaveImage);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                GlobalVariables.SpiroStockManagmentDatabaseProcedures.InsertNewItem(UpdateItem);
                DataHasBeenChanged = true;
            }
            this.Close();
        }

        public void InsertCurrentToList()
        {
            //is an update of quantity
            if (UpdateItem != null)
            {
                if (UpdateItem.History == null) UpdateItem.History = new List<SpiroStockManagmentDatabaseClass.Objects.Item>();


                UpdateItem.Brand = textBox_Brand.Text;
                UpdateItem.categoryString = textBox_Category.Text;
                UpdateItem.InformationTakenFrom = textBox_InfoFrom.Text;
                UpdateItem.MarketItemUrl = textBox_MarketLink.Text;
                UpdateItem.Name = autoCompleteMine1.TextInputted;
                UpdateItem.PackageInfo = textBox_InfoPackage.Text;
                UpdateItem.Price = float.Parse(numericUpDown_Price.Value.ToString());


                if (numericUpDown_PriceWeight.Value > 0)
                    UpdateItem.VariableWeightPrice = numericUpDown_PriceWeight.Value.ToString() + "/" + comboBox_PriceCapacity.SelectedItem.ToString();

                //barcode
                if (textBox_barCode.Text != string.Empty)
                {
                    UpdateItem.BarCode = long.Parse(textBox_barCode.Text);
                }

                if (CurrentListName != "all")
                {
                    int _quantityToUpdate = 0;
                    float _quantityWeightToUpdate = 0;
                    if (numericUpDown_Quantity.Value > 0)
                        _quantityToUpdate = int.Parse(numericUpDown_Quantity.Value.ToString());
                    if (numericUpDown_quantityWeight.Value > 0)
                        _quantityWeightToUpdate = float.Parse(numericUpDown_quantityWeight.Value.ToString());

                    //string _listName = "";
                    if (CurrentListName.ToLower() == "out")
                    {
                        UpdateItem.QuantityOut += _quantityToUpdate;
                        UpdateItem.QuantityWeightOut += _quantityWeightToUpdate;
                        if (checkBox_RemoveFromOthersList.Checked)
                        {
                            UpdateItem.QuantityIn -= _quantityToUpdate;
                            UpdateItem.QuantityWeightIn -= _quantityWeightToUpdate;
                        }
                    }

                    if (CurrentListName.ToLower() == "in")
                    {
                        UpdateItem.QuantityIn += _quantityToUpdate;
                        UpdateItem.QuantityWeightIn += _quantityWeightToUpdate;
                        if (checkBox_RemoveFromOthersList.Checked)
                        {
                            UpdateItem.QuantityOut -= _quantityToUpdate;
                            UpdateItem.QuantityWeightOut -= _quantityWeightToUpdate;
                        }
                    }



                    //add to history
                    if (CurrentListName.ToLower() == "in")
                    {
                        SpiroStockManagmentDatabaseClass.Objects.Item _newUpdateItem = new SpiroStockManagmentDatabaseClass.Objects.Item();
                        _newUpdateItem.ListName = "in";

                        //quantity
                        _newUpdateItem.Quantity = _quantityToUpdate;
                        _newUpdateItem.QuantityWeight = _quantityWeightToUpdate;
                        _newUpdateItem.InsertDate = DateTime.Now.ToString("s");
                        UpdateItem.History.Add(_newUpdateItem);
                    }
                    if (CurrentListName.ToLower() == "out")
                    {
                        SpiroStockManagmentDatabaseClass.Objects.Item _newUpdateItem = new SpiroStockManagmentDatabaseClass.Objects.Item();
                        _newUpdateItem.ListName = "out";

                        //quantity
                        _newUpdateItem.Quantity = _quantityToUpdate;
                        _newUpdateItem.QuantityWeight = _quantityWeightToUpdate;
                        _newUpdateItem.InsertDate = DateTime.Now.ToString("s");
                        UpdateItem.History.Add(_newUpdateItem);
                    }
                    if (checkBox_RemoveFromOthersList.Checked)
                    {
                        SpiroStockManagmentDatabaseClass.Objects.Item _newUpdateItem = new SpiroStockManagmentDatabaseClass.Objects.Item();
                        _newUpdateItem.ListName = (CurrentListName.ToLower() == "in") ? "out" : "in";

                        //quantity
                        _newUpdateItem.Quantity = _quantityToUpdate;
                        _newUpdateItem.QuantityWeight = _quantityWeightToUpdate;
                        _newUpdateItem.InsertDate = DateTime.Now.ToString("s");
                        UpdateItem.History.Add(_newUpdateItem);
                    }
                }


                //Save Picture
                string _pathToSaveImage = "";
                //if (pictureBox1.Image != null)
                //_pathToSaveImage = GlobalVariables.ProductImagesPath + UpdateItem.Id.ToString() + "small." + pictureBox1.Tag.ToString();
                UpdateItem.PictureSmallFilename = _pathToSaveImage.Substring(_pathToSaveImage.LastIndexOf('\\') + 1);

                if (_pathToSaveImage != string.Empty)
                {
                    try
                    {
                        pictureBox1.Image.Save(_pathToSaveImage);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                GlobalVariables.SpiroStockManagmentDatabaseProcedures.InsertNewItem(UpdateItem);
                DataHasBeenChanged = true;
            }
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                try
                {
                    Image _image = GlobalProcedures.ImageFromFileNoLock(openFileDialog1.FileName);


                    Size _imageNewSize = GlobalProcedures.GetThumbnailSize(_image, 140);

                    Image _ResizedImage = _image.GetThumbnailImage(_imageNewSize.Width, _imageNewSize.Height, null, IntPtr.Zero);
                    pictureBox1.Image = _ResizedImage;
                    pictureBox1.Height = _ResizedImage.Height;
                    pictureBox1.Width = _ResizedImage.Width;
                }
                catch (Exception)
                {
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (autoCompleteMine1.TextInputted == "") return;
            SearchItem _SearchItem = new SearchItem();
            _SearchItem.Initialize(autoCompleteMine1.TextInputted);
            _SearchItem.ShowDialog();
            if (_SearchItem.ItemLink != null && _SearchItem.ItemLink != string.Empty)
            {
                textBox_MarketLink.Text = _SearchItem.ItemLink;
                //comboBox_Name.Text = _SearchItem.ItemName;
                autoCompleteMine1.TextInputted = _SearchItem.ItemName;
                textBox_Brand.Text = _SearchItem.ItemBrand;
                if (_SearchItem.ItemPrice.IndexOf(" ") > 0)
                    //textBox_Price.Text = _SearchItem.ItemPrice.Substring(0, _SearchItem.ItemPrice.IndexOf(" "));
                    numericUpDown_Price.Value = decimal.Parse(_SearchItem.ItemPrice.Substring(0, _SearchItem.ItemPrice.IndexOf(" ")));
                else
                    //textBox_Price.Text = _SearchItem.ItemPrice;
                    numericUpDown_Price.Value = decimal.Parse(_SearchItem.ItemPrice);
                textBox_InfoPackage.Text = _SearchItem.ItemPackageInfo;
                textBox_Category.Text = _SearchItem.ItemcategoryString;

                if (_SearchItem.ItemVariableWeightPrice.IndexOf("Un") == -1)
                {
                    string[] _array = _SearchItem.ItemVariableWeightPrice.Split('/');
                    numericUpDown_PriceWeight.Value = decimal.Parse(_array[0]);
                    if (_array[1].IndexOf("kg") > 0)
                        comboBox_PriceCapacity.SelectedIndex = comboBox_PriceCapacity.Items.IndexOf("kg");
                    if (_array[1].IndexOf("l") > 0)
                        comboBox_PriceCapacity.SelectedIndex = comboBox_PriceCapacity.Items.IndexOf("l");

                    //string variableWeightByKgString = _SearchItem.ItemVariableWeightPrice.Substring(2);
                    //variableWeightByKgString = variableWeightByKgString.Substring(0, variableWeightByKgString.IndexOf('/'));
                    //variableWeightByKgString = variableWeightByKgString.Trim();
                    //float variableWeightByKg = float.Parse(variableWeightByKgString);
                    //numericUpDown_PriceWeight.Value = decimal.Parse(variableWeightByKg.ToString()); 
                }
                //load image


                Size _imageNewSize = GlobalProcedures.GetThumbnailSize(_SearchItem.ItemPictureSmallImage, 140);

                Image _ResizedImage = _SearchItem.ItemPictureSmallImage.GetThumbnailImage(_imageNewSize.Width, _imageNewSize.Height, null, IntPtr.Zero);


                pictureBox1.Image = _ResizedImage;
                pictureBox1.Height = _ResizedImage.Height;
                pictureBox1.Width = _ResizedImage.Width;
                pictureBox1.Tag = _SearchItem.ItemPictureSmallUrl.Substring(_SearchItem.ItemPictureSmallUrl.LastIndexOf(".") + 1);
                textBox_InfoFrom.Text = _SearchItem.InformationTakenFrom;
                UpdateCurrentPrice();
            }
        }


        //Insert Button
        private void button3_Click(object sender, EventArgs e)
        {
            InsertCurrent();
            //save checkbox states
            SaveCheckBoxStates();
        }

        void SaveCheckBoxStates()
        {
            //save settings
            //Settings1.Default.ChekboxAddToStock = checkBox_RemoveFromOthersList.Checked.ToString();
            //Settings1.Default.ChekboxRemoveFromStock = checkBox_DontRemoveFromOtherList.Checked.ToString();
            //Settings1.Default.CheckboxAddToShoopingCart = checkBox2.Checked.ToString();
            //Settings1.Default.CheckboxRemoveFromShoopingCart = checkBox_RemoveFromShoopingList.Checked.ToString();

            Settings1.Default.Save();
        }

        //private void checkBox1_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (!checkboxsAreInitilizing)
        //    {
        //        if (checkBox_RemoveFromOthersList.Checked && checkBox_DontRemoveFromOtherList.Checked)
        //        {
        //            if (checkBox_RemoveFromOthersList == sender)
        //            {
        //                if(checkBox_RemoveFromOthersList.Enabled) checkBox_RemoveFromOthersList.Checked = true;
        //                if (checkBox_DontRemoveFromOtherList.Enabled) checkBox_DontRemoveFromOtherList.Checked = false;
        //            }
        //            if (checkBox_DontRemoveFromOtherList == sender)
        //            {
        //                if (checkBox_RemoveFromOthersList.Enabled) checkBox_RemoveFromOthersList.Checked = false;
        //                if (checkBox_DontRemoveFromOtherList.Enabled) checkBox_DontRemoveFromOtherList.Checked = true;
        //            }
        //        } 
        //    }
        //}

        //private void checkBox2_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (!checkboxsAreInitilizing)
        //    {
        //        if (checkBox2.Checked && checkBox_RemoveFromShoopingList.Checked)
        //        {
        //            if (checkBox2 == sender)
        //            {
        //                if (checkBox2.Enabled) checkBox2.Checked = true;
        //                if (checkBox_RemoveFromShoopingList.Enabled) checkBox_RemoveFromShoopingList.Checked = false;
        //            }
        //            if (checkBox_RemoveFromShoopingList == sender)
        //            {
        //                if (checkBox2.Enabled) checkBox2.Checked = false;
        //                if (checkBox_RemoveFromShoopingList.Enabled) checkBox_RemoveFromShoopingList.Checked = true;
        //            }
        //        } 
        //    }

        //    //if (checkBox2.Checked == true)
        //    //{
        //    //    checkBox1.Checked = false;
        //    //}
        //    //else
        //    //{
        //    //    checkBox1.Checked = true;
        //    //    checkBox_Swap.Text = "Retirar automáticamente da lista de compras";
        //    //    //see and show if the product exists in the shooping list
        //    //    //UpdateOthersListLabel();
        //    //}
        //}


        private void InsertItem_Load(object sender, EventArgs e)
        {
            autoCompleteMine1.textBox1.Focus();
        }

        private void InsertItem_Shown(object sender, EventArgs e)
        {
            if (UpdateItem == null)
                autoCompleteMine1.textBox1.Focus();
            else
            {
                numericUpDown_Quantity.Focus();
                numericUpDown_Quantity.Select(0,numericUpDown_Quantity.Text.Length);
            }

        }

        private void textBox_Name_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                button2.PerformClick();
            }
        }

        //private void comboBox_Name_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (comboBox_Name.SelectedItem != null)
        //    {
        //        Initialize(GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetProduct(int.Parse((comboBox_Name.SelectedItem as ComboItem).ProductID.ToString())));
        //    }
        //}

        //string PreviousComboBoxText = "";
        //private void comboBox_Name_TextChanged(object sender, EventArgs e)
        //{
        //    if (comboBox_Name.Text != string.Empty)
        //    {
        //        if (System.Text.RegularExpressions.Regex.IsMatch(comboBox_Name.Text, @"\d"))
        //        {
        //            comboBox_Name.Text = PreviousComboBoxText;
        //        }
        //        PreviousComboBoxText = comboBox_Name.Text;
        //        button2.Enabled = true;
        //    }
        //}

        private void comboBox_PriceCapacity_SelectedIndexChanged(object sender, EventArgs e)
        {
            label_QuantityCapacity.Text = comboBox_PriceCapacity.SelectedItem.ToString();
        }

        private void numericUpDown_PriceWeight_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_PriceWeight.Value > 0)
                numericUpDown_quantityWeight.Enabled = true;
            else
                numericUpDown_quantityWeight.Enabled = false;
        }

        private void numericUpDown_Quantity_ValueChanged(object sender, EventArgs e)
        {
            UpdateCurrentPrice();
        }

        void UpdateCurrentPrice()
        {
            decimal _totalPrice = 0;
            if (numericUpDown_Quantity.Value > 0)
                _totalPrice += numericUpDown_Price.Value * numericUpDown_Quantity.Value;
            if (numericUpDown_quantityWeight.Value > 0)
                _totalPrice += numericUpDown_quantityWeight.Value * numericUpDown_PriceWeight.Value;

            if (_totalPrice.ToString().Length - (_totalPrice.ToString().IndexOf(',') + 1) == 3)
                label_TotalPrice.Text = _totalPrice.ToString().Remove(_totalPrice.ToString().Length - 1) + "€";
            else
            {
                label_TotalPrice.Text = _totalPrice.ToString() + "€";
            }
        }


        private void InsertItem_KeyDown(object sender, KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Passing insertitem key down");
            if (e.KeyCode == Keys.Escape)
            {
                if (autoCompleteMine1.AutoCompleteControlVisibility == true)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                    this.Close();
                }
            }
            if (e.KeyCode == Keys.Enter)
            {
                if (autoCompleteMine1.AutoCompleteControlVisibility == true)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                    InsertCurrent();
                }
            }
            //e.Handled = false;
        }

        private void autoCompleteMine1_ProductedSelectedChanged(object sender, EventArgs e)
        {
            if (autoCompleteMine1.SelectedProductId != -1)
            {
                Initialize(GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetProduct(autoCompleteMine1.SelectedProductId));
            }
        }

        private void checkBox_RemoveFromOthersList_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_RemoveFromOthersList.Checked)
                checkBox_DontRemoveFromOtherList.Checked = false;
            else
                checkBox_DontRemoveFromOtherList.Checked = true;
        }

        private void checkBox_DontRemoveFromOtherList_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_DontRemoveFromOtherList.Checked)
                checkBox_RemoveFromOthersList.Checked = false;
            else
                checkBox_RemoveFromOthersList.Checked = true;
        }

    }

    public class ComboItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //[DisplayName("Nome")]
        string _ProductName;
        int _ProductID;

        public String ProductName{
            get { return _ProductName; }
            set
            {
              _ProductName = value;
              this.NotifyPropertyChanged("ProductName");
            }
        }
        public int ProductID
        {
            get { return _ProductID; }
            set
            {
                _ProductID = value;
                this.NotifyPropertyChanged("ProductID");
            }
        }


        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public ComboItem(String productName, int productID)
        {
            ProductName = productName;
            ProductID = productID;
        }

        public override string ToString()
        {
            return _ProductName;
        }

    };
}
