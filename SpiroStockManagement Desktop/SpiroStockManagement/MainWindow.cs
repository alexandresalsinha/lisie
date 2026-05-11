using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using RawInput;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SpiroStockManagement
{
    public partial class MainWindow : Form, IMessageFilter
    {
        InputDevice id;
        int NumberOfKeyboards;

        string InputedBarCode = "";

        Syncing SyncingDatabases = new Syncing();

        public MainWindow()
        {
            InitializeComponent();
            Application.AddMessageFilter(this);

            SpiroStockManagmentDatabaseClass.Procedures _SpiroDatabaseProcedures = new SpiroStockManagmentDatabaseClass.Procedures();
            
            //DEBUG AND RELEASE
            _SpiroDatabaseProcedures.XmlDatabaseRecepiesPath = Application.StartupPath + "\\Databases\\Recepies.xml";
            _SpiroDatabaseProcedures.XmlDatabaseIngredientsPath = Application.StartupPath + "\\Databases\\Ingredients.xml";
            _SpiroDatabaseProcedures.XmlDatabaseFilePath = Application.StartupPath + "\\Databases\\InventoryItems.xml";
            //RELEASE WITH GOOGLE DRIVE
            //_SpiroDatabaseProcedures.XmlDatabaseRecepiesPath = @"C:\Users\Ardeth\Google Drive\Spiro Stock Management Databases\Recepies.xml";
            //_SpiroDatabaseProcedures.XmlDatabaseIngredientsPath = @"C:\Users\Ardeth\Google Drive\Spiro Stock Management Databases\Ingredients.xml";
            //_SpiroDatabaseProcedures.XmlDatabaseFilePath = @"C:\Users\Ardeth\Google Drive\Spiro Stock Management Databases\InventoryItems.xml";
            
            _SpiroDatabaseProcedures.Initialize();
            GlobalVariables.SpiroStockManagmentDatabaseProcedures = _SpiroDatabaseProcedures;
            GlobalVariables.SpiroStockManagmentDatabaseProcedures.XmlDocumentSaved += SpiroStockManagmentDatabaseProcedures_XmlDocumentSaved;

            //DEBUG AND RELEASE
            GlobalVariables.RecipeImagesPath = Application.StartupPath + "\\Databases\\RecipeImages\\";
            GlobalVariables.ProductImagesPath = Application.StartupPath + "\\Databases\\ItemsImages\\";
            //RELEASE WITH GOOGLE DRIVE
            //GlobalVariables.RecipeImagesPath = @"C:\Users\Ardeth\Google Drive\Spiro Stock Management Databases\RecipeImages\";
            //GlobalVariables.ProductImagesPath = @"C:\Users\Ardeth\Google Drive\Spiro Stock Management Databases\ItemsImages\";
            UpdateGUI();
            GlobalVariables.MainWindowHandle = this;

            
            //id = new InputDevice(Handle);
            //NumberOfKeyboards = id.EnumerateDevices();
            //id.KeyPressed += new InputDevice.DeviceEventHandler(m_KeyPressed);

            //tests _MyForm = new tests();
            //_MyForm.Show();

           //Set Sincing and watch for changes in the database
            //SyncingDatabases.DatabaseFileWasUpdated += SyncingDatabases_DatabaseFileWasUpdated;
            //List<string> _filePathsToWatch = new List<string>();
            //_filePathsToWatch.Add(_SpiroDatabaseProcedures.XmlDatabaseFilePath);

            ////SyncingDatabases.Initialize(_filePathsToWatch);

            //SyncingDatabases.Initialize(Application.StartupPath + "\\Databases\\");
            SyncingDatabases.Initialize(_SpiroDatabaseProcedures.XmlDatabaseFilePath);
            timer_CheckIfDatabaseChanged.Enabled = true;
            timer_CheckIfDatabaseChanged.Start();

        }

        void SpiroStockManagmentDatabaseProcedures_XmlDocumentSaved(object sender, EventArgs e)
        {
            GlobalVariables.LastTimeDatabaseWasChangedByMe = DateTime.Now;
        }

        //bool twoTimesBug = false;
        //bool DatabaseUpdated = false;
        //void SyncingDatabases_DatabaseFileWasUpdated(object sender, FileChangedEventArgs e)
        //{
        //    //TODO Pass this code to the Syncing class
        //    MessageBox.Show(e.TimeOfChange + "\n" + GlobalVariables.LastTimeDatabaseWasChangedByMe);
        //    if ((e.TimeOfChange - GlobalVariables.LastTimeDatabaseWasChangedByMe).TotalSeconds > 7)
        //    {
        //        //File was Changed by other program
        //        if (!twoTimesBug)
        //        {
        //           MessageBox.Show("entered the if");
        //           Invoke(new Action(() =>
        //           {
        //               //UpdateGUI();
        //               DatabaseUpdated = true;
        //           }));
                   
        //            //if(currentSelectedTab == "out")
        //              //  buyList1.Initialize("out");
        //           twoTimesBug = true;
        //        }
        //        else
        //        {
        //            twoTimesBug = false;
        //            return;
        //        }
        //    }

        //}

        //protected override void WndProc(ref Message message)
        //{
        //    if (id != null)
        //    {
        //        id.ProcessMessage(message);
        //    }
        //    base.WndProc(ref message);
        //}

        bool keyPressedIsFromBarCodeScanner = false;
        private void m_KeyPressed(object sender, InputDevice.KeyControlEventArgs e)
        {
            //Replace() is just a cosmetic fix to stop ampersands turning into underlines
            
            string s = e.Keyboard.deviceHandle.ToString();
            char _pressedkey = (char)e.Keyboard.key;
            if (e.Keyboard.Name.IndexOf("HID") > 0)
            {
                keyPressedIsFromBarCodeScanner = true;

                switch (_pressedkey)
                {
                    case '\r':
                        long _BarCodeNumber = -1;
                        if (long.TryParse(InputedBarCode.ToString(), out _BarCodeNumber))
                        {
                            InputedBarCode = "";

                            //SpiroStockManagmentDatabaseClass.Objects.Product _ItemExists = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetItemByBarCode(InputedBarCode);
                            //MessageBox.Show(InputedBarCode.ToString());
                            //if a insert dialog already exists, insert it and open a  new one
                            if (GlobalVariables.CurrentInsertItemDialog != null)
                            {
                                //GlobalVariables.CurrentInsertItemDialog = null;
                                if (GlobalVariables.CurrentInsertItemDialog.UpdateItem != null)
                                    GlobalVariables.CurrentInsertItemDialog.InsertCurrent();
                                GlobalVariables.CurrentInsertItemDialog.Dispose();
                                GlobalVariables.CurrentInsertItemDialog = null;
                            }

                            InsertItem _InsertItem = new InsertItem();
                            _InsertItem.Initialize(_BarCodeNumber);
                            GlobalVariables.CurrentInsertItemDialog = _InsertItem;
                            GlobalVariables.CurrentInsertItemDialog.FormClosed += new FormClosedEventHandler(_InsertItem_FormClosed);
                            InputedBarCode = string.Empty;
                            _InsertItem.ShowDialog();


                            UpdateGUI();
                            break;
                        }
                        break;
                    default:
                        InputedBarCode += _pressedkey.ToString();
                        break;
                }
            }
            //lbType.Text = e.Keyboard.deviceType;
            //lbName.Text = e.Keyboard.deviceName.Replace("&", "&&");
            //lbDescription.Text = e.Keyboard.Name;
            //lbKey.Text = e.Keyboard.key.ToString();
            //lbNumKeyboards.Text = NumberOfKeyboards.ToString();
            //lbVKey.Text = e.Keyboard.vKey;
        }

        private void MainWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
            //int _number = -1;
            //if (int.TryParse(e.KeyChar.ToString(), out _number))
            //{
            //    InputedBarCode += e.KeyChar.ToString();
            //}

            ////check if is a return, if it is , is because the bar code is complete
            //if (e.KeyChar == '\r')
            //{
            //    string s = InputedBarCode;

            //    SpiroStockManagmentDatabaseClass.Objects.Product _ItemExists = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetItemByBarCode(InputedBarCode);
            //    if (_ItemExists != null)
            //    {
            //        InsertItem _InsertItem = new InsertItem();
            //        _InsertItem.Initialize(_ItemExists);
            //        _InsertItem.ShowDialog();
            //    }
            //    else
            //    {
            //        InsertItem _InsertItem = new InsertItem();
            //        _InsertItem.Initialize(long.Parse(InputedBarCode));
            //        _InsertItem.ShowDialog();
            //    }

            //    UpdateGUI();
            //    InputedBarCode = string.Empty;


                //if is a new one, show the insert item form
                //if exist , show the confirmation window for some seconds and ask if the item is new or over
            //}

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateGUI();
        }

        string currentSelectedTab = "";
        void UpdateGUI()
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    buyList1.Initialize("out");
                    currentSelectedTab = "out";
                    break;
                case 1:
                    stockList1.Initialize("in");
                    break;
                case 2:
                    buyList_Products.Initialize("all");
                    break;
                case 3:
                    recepies1.Initialize();
                    break;
                default:
                    break;
            }
        }

        InsertItem CurrentInsertItemDialog = null;


        void _InsertItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (GlobalVariables.CurrentInsertItemDialog != null && GlobalVariables.CurrentInsertItemDialog.DataHasBeenChanged)
            {
                if(tabControl1.SelectedIndex == 0)
                    buyList1.RefreshList();
                if (tabControl1.SelectedIndex == 1)
                    stockList1.RefreshList();
            }
            GlobalVariables.CurrentInsertItemDialog = null;
            //((Form)sender).Close();
        }

        private void buyList1_UpdateStatusBar(object sender, string message)
        {
            statusStrip1.Items[0].Text = message;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void stockList1_Load(object sender, EventArgs e)
        {

        }

        private void toolStripButton_Recepies_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
        }

        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_LEFTMOUSEDOWN = 0x201;
        const int WM_LEFTMOUSEUP = 0x202;
        const int WM_LEFTMOUSEDBL = 0x203;
        const Int32 WM_UPDATESB = 0x112;
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_KEYDOWN)
            {
                if (keyPressedIsFromBarCodeScanner)
                {
                    keyPressedIsFromBarCodeScanner = false;
                    return true;
                }
                else return false;
            }
            if (m.Msg == WM_KEYUP)
            {
                //return true;
                if (keyPressedIsFromBarCodeScanner)
                {
                    keyPressedIsFromBarCodeScanner = false;
                    return true;
                }
                else
                    //return HandleKeys((Keys)(int)m.WParam & Keys.KeyCode);
                    return false;
            }
            else
                return false;
        }

        public bool HandleKeys(Keys keyCode)
        {
            System.Diagnostics.Debug.WriteLine("Passing handle keys");
            char _pressedkey = (char)keyCode;

            switch (keyCode)
            {
                case Keys.Enter:
                    //string s = InputedBarCode;

                    long _BarCodeNumber = -1;
                    if (!long.TryParse(InputedBarCode.ToString(), out _BarCodeNumber))
                    {
                        InputedBarCode = "";

                        //if inseertItem Dialog is activated, insert the current item
                        if (GlobalVariables.CurrentInsertItemDialog != null)
                        {
                            Control _c = GlobalVariables.CurrentInsertItemDialog.ActiveControl;
                            if (_c.GetType() == typeof(AutoCompleteMine))
                            {
                                if (GlobalVariables.EnterPressedIsToAddProduct == true && (_c as AutoCompleteMine).AutoCompleteControlVisibility == false)
                                {
                                    GlobalVariables.EnterPressedIsToAddProduct = false;
                                    GlobalVariables.CurrentInsertItemDialog.InsertCurrent();
                                    return false;
                                }
                                else GlobalVariables.EnterPressedIsToAddProduct = true;
                            }
                        }

                        break;
                    }

                    SpiroStockManagmentDatabaseClass.Objects.Product _ItemExists = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetItemByBarCode(InputedBarCode);
                    //MessageBox.Show(InputedBarCode.ToString());
                    //if a insert dialog already exists, insert it and open a  new one
                    if (GlobalVariables.CurrentInsertItemDialog != null)
                    {
                        //GlobalVariables.CurrentInsertItemDialog = null;
                        if (GlobalVariables.CurrentInsertItemDialog.UpdateItem != null)
                            GlobalVariables.CurrentInsertItemDialog.InsertCurrent();
                        GlobalVariables.CurrentInsertItemDialog.Dispose();
                        GlobalVariables.CurrentInsertItemDialog = null;
                    }

                    if (_ItemExists != null)
                    {
                        InsertItem _InsertItem = new InsertItem();
                        _InsertItem.Initialize(_ItemExists);
                        GlobalVariables.CurrentInsertItemDialog = _InsertItem;
                        GlobalVariables.CurrentInsertItemDialog.FormClosed += new FormClosedEventHandler(_InsertItem_FormClosed);
                        InputedBarCode = string.Empty;
                        _InsertItem.ShowDialog();

                    }
                    else
                    {
                        InsertItem _InsertItem = new InsertItem();
                        _InsertItem.Initialize(long.Parse(InputedBarCode));
                        GlobalVariables.CurrentInsertItemDialog = _InsertItem;
                        GlobalVariables.CurrentInsertItemDialog.FormClosed += new FormClosedEventHandler(_InsertItem_FormClosed);
                        InputedBarCode = string.Empty;
                        _InsertItem.ShowDialog();
                    }

                    UpdateGUI();
                    InputedBarCode = "";
                    break;
                case Keys.Escape:
                    break;
                default:
                    if ((Char.IsNumber(_pressedkey) || Char.IsLetter(_pressedkey)) && ControlKeyPressed == false)
                        InputedBarCode += _pressedkey.ToString();
                    else
                        ControlKeyPressed = false;
                    break;

            }
            //if (GlobalVariables.CurrentInsertItem != null)
            //{
            //    Control _c = GlobalVariables.CurrentInsertItem.ActiveControl;
            //    if (_c.GetType() == typeof(AutoCompleteMine))
            //    {
            //        return false;
            //    }
            //}
            return false;
        }

        public void UpdateStatusBarText(string text)
        {
            statusStrip1.Text = text;
        }

        bool ControlKeyPressed = false;
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
            {
                InsertItem _InsertItem = new InsertItem();
                _InsertItem.Initialize();
                GlobalVariables.CurrentInsertItemDialog = _InsertItem;
                GlobalVariables.CurrentInsertItemDialog.FormClosed += new FormClosedEventHandler(_InsertItem_FormClosed);
                InputedBarCode = "";
                e.Handled = true;
                ControlKeyPressed = true;
                _InsertItem.ShowDialog();
                return;
            }
            if (e.KeyCode == Keys.P && e.Modifiers == Keys.Control)
            {
                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        stockList1.Print();
                        break;
                    case 1:
                        buyList1.Print();
                        break;
                    default:
                        break;
                }


                e.Handled = true;
                ControlKeyPressed = true;
                return;
            }
            if (e.KeyCode == Keys.T && e.Modifiers == Keys.Control)
            {
                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        stockList1.SelectAllItems();
                        break;
                    case 1:
                        buyList1.SelectAllItems();
                        break;
                    default:
                        break;
                }
                
                e.Handled = true;
                ControlKeyPressed = true;
                return;
            }
            e.Handled = false;

        }

        private void sincronizarAgoraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Syncing.DownloadFiles();
        }

        private void definiçõesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Syncing.UploadFiles();
        }

        private void fazerUpdateAosIngredientesXmladicionarOsInexistentesDeReceitasJaExistentesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int _ingredientsAdded = 0;
            foreach (XElement _XRecipe in GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllRecepies())
            {

                SpiroStockManagmentDatabaseClass.Objects.Recipe _currentRecipe = new SpiroStockManagmentDatabaseClass.Objects.Recipe();
                _currentRecipe = (SpiroStockManagmentDatabaseClass.Objects.Recipe)SpiroStockManagmentDatabaseClass.XmlSerializerExtension.DeSerializerToObject(_XRecipe, _currentRecipe);
                if (_currentRecipe.IngredientList != null)
                {
                    foreach (SpiroStockManagmentDatabaseClass.Objects.RecipeIngredient _RecipeIngredient in _currentRecipe.IngredientList)
                    {
                        if (!GlobalVariables.SpiroStockManagmentDatabaseProcedures.CheckIfIngredientExists(_RecipeIngredient.Name))
                        {
                            SpiroStockManagmentDatabaseClass.Objects.Ingredient _Ingredient = new SpiroStockManagmentDatabaseClass.Objects.Ingredient();
                            _Ingredient.Name = _RecipeIngredient.Name;
                            _Ingredient.Products = new List<SpiroStockManagmentDatabaseClass.Objects.IngredientProduct>();
                            GlobalVariables.SpiroStockManagmentDatabaseProcedures.InsertEditIngredient(_Ingredient);
                            _ingredientsAdded++;
                        }
                    }
                }
            }
            if (_ingredientsAdded >0)
            {
                //MessageBox.Show(_ingredientsAdded + " ingrediente/s adicionados");
                MessageBox.Show(_ingredientsAdded + " ingredient/s added");
            }
        }

        private void timer_CheckIfDatabaseChanged_Tick(object sender, EventArgs e)
        {
            if (SyncingDatabases.HasTheDatabaseChanged())
            {
                GlobalVariables.SpiroStockManagmentDatabaseProcedures.InitializeXmlDatabaseFile();
                UpdateGUI();
            }
        }

        string lastBarCode = string.Empty;

        private void timer_checkFirebase_Tick(object sender, EventArgs e)
        {
            string json = FirebaseHelper.Helper.Get("SpiroStockManagement");

            Dictionary<string, string> _tempJson = new Dictionary<string, string>();
            if (json == "null") return;

            JObject objsTemp = JObject.Parse(json);

            
            foreach (var item in objsTemp)
            {
                string key = item.Key.ToString();
                string value = item.Value.ToString();
                _tempJson.Add(key, value);
            }

            timer_checkFirebase.Stop();

            foreach (var item in _tempJson)
            {
                FirebaseItem _itemToInsert = JsonConvert.DeserializeObject<FirebaseItem>(item.Value);

                Console.WriteLine(_itemToInsert.barCode);

                //check if it exists in database
                SpiroStockManagmentDatabaseClass.Objects.Product _ItemExists = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetItemByBarCode(_itemToInsert.barCode.ToString());
                if (_ItemExists != null)
                {
                    //if yes, just add to shopping card and don t show Insert Dialog
                    InsertItem _InsertItem = new InsertItem();
                    long _barcodeLong = long.Parse(_itemToInsert.barCode);
                    _InsertItem.Initialize(_barcodeLong, "out");
                    _InsertItem.InsertCurrentToList();
                    UpdateGUI();
                }
                else
                {
                    try
                    {
                        InsertItem _InsertItem = new InsertItem();
                        long _barcodeLong = long.Parse(_itemToInsert.barCode);
                        _InsertItem.Initialize(_barcodeLong, "out");
                        GlobalVariables.CurrentInsertItemDialog = _InsertItem;
                        GlobalVariables.CurrentInsertItemDialog.FormClosed += new FormClosedEventHandler(_InsertItem_FormClosed);
                        InputedBarCode = string.Empty;
                        //_InsertItem.ShowDialog();
                        _InsertItem.Show();
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
                //delete from database
                FirebaseHelper.Helper.Delete(item.Key);
            }
            timer_checkFirebase.Start();
        }

        void DeleteFirebaseItem(string key)
        {
        }
    }
}
