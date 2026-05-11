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

namespace SpiroStockManagement
{
    public partial class BuyList : UserControl
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        public event ChangeStatusBarEventHandler UpdateStatusBar;
        string CurrentListName = "";

        private ListViewColumnSorter lvwColumnSorter;

        public BuyList()
        {
            InitializeComponent();
            lvwColumnSorter = new ListViewColumnSorter();
            this.listView_Details.ListViewItemSorter = lvwColumnSorter;
        }

        public void Initialize(string listName)
        {
            CurrentListName = listName;
            if (listName == "in")
            {
                //groupBox1.Text = "Inventorio";
                groupBox1.Text = "Inventory List";
            }
            if (listName == "out")
            {
                //groupBox1.Text = "Lista de Compras";
                groupBox1.Text = "Shopping Cart List";
            }
            if (listName == "all")
            {
                //groupBox1.Text = "Produtos";
                groupBox1.Text = "Products";

                //remove unecessary columns
                foreach (ColumnHeader _ColumnHeader in listView_Details.Columns)
                {
                    //if (_ColumnHeader.Text.ToLower() == "quantidade" || _ColumnHeader.Text.ToLower() == "preço total")
                    if (_ColumnHeader.Text.ToLower() == "quantity" || _ColumnHeader.Text.ToLower() == "total price")
                    {
                        listView_Details.Columns.Remove(_ColumnHeader);
                    }
                }
                adicionarÁListaDeComprasToolStripMenuItem.Visible = true;
                adicionarÁListaDeInventórioToolStripMenuItem.Visible = true;
                apagarDaListaToolStripMenuItem.Visible = false;
            }
            InitializeCategories();
            RefreshList();
        }

        void InitializeCategories()
        {
            comboBox_Category.Items.Clear();
            //comboBox_Category.Items.Add("Todas");
            comboBox_Category.Items.Add("All");
            foreach (string _category in GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllProductCategories())
            {
                if (_category != string.Empty)
                    comboBox_Category.Items.Add(_category);
            }
            comboBox_Category.SelectedIndex = 0;
        }

        List<ListViewItem> CurrentListViewItemsDetails = new List<System.Windows.Forms.ListViewItem>();
        List<ListViewItem> CurrentListViewItemsLargeIcon = new List<System.Windows.Forms.ListViewItem>();
        public void RefreshList()
        {
            //Initialize details view
            listView_Details.Items.Clear();
            listView_Details.BeginUpdate();

            listView_LargeIcon.Items.Clear();
            listView_LargeIcon.BeginUpdate();
            imageList1.Images.Clear();

            int _itemIndex = 0;
            float _finaltotalPrice = 0;

            CurrentListViewItemsDetails.Clear();
            CurrentListViewItemsLargeIcon.Clear();


            IEnumerable<XElement> _list = null;
            List<string> _categoriesOfList = null;
            if (CurrentListName == "in")
            {
                _list = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllInItems();
                _categoriesOfList = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllProductCategoriesInList();
            }
            if (CurrentListName == "out")
            {
                _list = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllOutItems();
                _categoriesOfList = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllProductCategoriesOutList();
            }

            if (CurrentListName == "all")
            {
                _list = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllProducts();
                _categoriesOfList = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllProductCategories();
            }

            //Initialize the groups (categories)
            listView_Details.Groups.Clear();
            listView_LargeIcon.Groups.Clear();
            Dictionary<string, int> _DictionaryCategoryIndex = new Dictionary<string, int>();
            int _categoriesCountIndex = 0;
            foreach (string _category in _categoriesOfList)
            {
                if (_category != string.Empty)
                {
                    listView_Details.Groups.Add(new ListViewGroup(_category, HorizontalAlignment.Left));
                    listView_LargeIcon.Groups.Add(new ListViewGroup(_category, HorizontalAlignment.Left));
                    _DictionaryCategoryIndex.Add(_category, _categoriesCountIndex);
                }
                else
                {
                    //listView_Details.Groups.Add(new ListViewGroup("Nenhum", HorizontalAlignment.Left));
                    //listView_LargeIcon.Groups.Add(new ListViewGroup("Nenhum", HorizontalAlignment.Left));
                    //_DictionaryCategoryIndex.Add("Nenhum", _categoriesCountIndex);
                    listView_Details.Groups.Add(new ListViewGroup("None", HorizontalAlignment.Left));
                    listView_LargeIcon.Groups.Add(new ListViewGroup("None", HorizontalAlignment.Left));
                    _DictionaryCategoryIndex.Add("None", _categoriesCountIndex);
                }
                _categoriesCountIndex++;
            }

            foreach (XElement _item in _list)
            {
                SpiroStockManagmentDatabaseClass.Objects.Product _currentItem = SpiroStockManagmentDatabaseClass.XmlSerializerExtension.DeSerializer(_item);
                ListViewItem _lvItem = new ListViewItem();
                _lvItem.Text = _currentItem.Name;
                _lvItem.SubItems.Add(_currentItem.Brand);
                _lvItem.SubItems.Add(_currentItem.PackageInfo);

                string _QuantityItemText = "";

                if (CurrentListName.ToLower() != "all")
                {
                    if (CurrentListName.ToLower() == "in")
                    {
                        if (_currentItem.QuantityWeightIn > 0 && _currentItem.QuantityIn > 0)
                            //_QuantityItemText = _currentItem.QuantityIn.ToString() + " emb + " + _currentItem.QuantityWeightIn + " kg";
                            _QuantityItemText = _currentItem.QuantityIn.ToString() + " package + " + _currentItem.QuantityWeightIn + " kg";
                        else
                        {
                            if (_currentItem.QuantityIn > 0)
                                _QuantityItemText = _currentItem.QuantityIn.ToString();
                            if (_currentItem.QuantityWeightIn > 0)
                                _QuantityItemText = _currentItem.QuantityWeightIn + " kg";
                        }
                    }
                    if (CurrentListName.ToLower() == "out")
                    {
                        if (_currentItem.QuantityWeightOut > 0 && _currentItem.QuantityOut > 0)
                            //_QuantityItemText = _currentItem.QuantityOut.ToString() + " emb + " + _currentItem.QuantityWeightOut + " kg";
                            _QuantityItemText = _currentItem.QuantityOut.ToString() + " package + " + _currentItem.QuantityWeightOut + " kg";
                        else
                        {
                            if (_currentItem.QuantityOut > 0)
                                _QuantityItemText = _currentItem.QuantityOut.ToString();
                            if (_currentItem.QuantityWeightOut > 0)
                                _QuantityItemText = _currentItem.QuantityWeightOut + " kg";
                        }
                    }
                    _lvItem.SubItems.Add(_QuantityItemText);
                }
                _lvItem.SubItems.Add(_currentItem.Price.ToString());

                //price weight
                _lvItem.SubItems.Add(_currentItem.VariableWeightPrice);


                //total price
                float _totalPrice = 0;
                if (CurrentListName.ToLower() != "all")
                {
                    if (CurrentListName.ToLower() == "in")
                    {
                        if (_currentItem.QuantityIn > 0)
                            _totalPrice = _currentItem.Price * _currentItem.QuantityIn;

                        if (_currentItem.QuantityWeightIn > 0)
                        {
                            float variableWeightByKg = GlobalProcedures.GetVariableWeightByKgOfString(_currentItem.VariableWeightPrice);
                            _totalPrice += variableWeightByKg * _currentItem.QuantityWeightIn;
                        }
                    }
                    if (CurrentListName.ToLower() == "out")
                    {
                        if (_currentItem.QuantityOut > 0)
                            _totalPrice = _currentItem.Price * _currentItem.QuantityOut;

                        if (_currentItem.QuantityWeightOut > 0)
                        {
                            float variableWeightByKg = GlobalProcedures.GetVariableWeightByKgOfString(_currentItem.VariableWeightPrice);
                            _totalPrice += variableWeightByKg * _currentItem.QuantityWeightOut;
                        }
                    }


                    _lvItem.SubItems.Add(_totalPrice + " €");

                    //increse total price
                    _finaltotalPrice += _totalPrice;
                }

                //add the image to the imageList
                bool _imageLoaded = false;
                try
                {
                    imageList1.Images.Add(_currentItem.Id.ToString(), Image.FromFile(GlobalVariables.ProductImagesPath + _currentItem.PictureSmallFilename));
                    _imageLoaded = true;
                }
                catch (Exception ex)
                {
                    _imageLoaded = false;
                }


                //assign group
                if (_currentItem.categoryString != null && _currentItem.categoryString != string.Empty)
                {
                    if (_DictionaryCategoryIndex.ContainsKey(_currentItem.categoryString)) // True
                        _lvItem.Group = listView_Details.Groups[_DictionaryCategoryIndex[_currentItem.categoryString]];
                }
                else
                {
                    //_lvItem.Group = listView_Details.Groups[_DictionaryCategoryIndex["Nenhum"]];
                    _lvItem.Group = listView_Details.Groups[_DictionaryCategoryIndex["None"]];
                }

                _lvItem.Tag = _currentItem;

                if (CurrentListName.ToLower() == "all"
                    || (CurrentListName.ToLower() == "in" && (_currentItem.QuantityIn > 0 || _currentItem.QuantityWeightIn > 0))
                    || (CurrentListName.ToLower() == "out" && (_currentItem.QuantityOut > 0 || _currentItem.QuantityWeightOut > 0)))
                {
                    listView_Details.Items.Add(_lvItem);
                    try
                    {
                        ListViewItem _clonedLVItem = (ListViewItem)_lvItem.Clone();
                        if (_imageLoaded == true)
                            _clonedLVItem.ImageIndex = _itemIndex;
                        else
                            _clonedLVItem.ImageIndex = -1;

                        if (CurrentListName.ToLower() != "all")
                            _clonedLVItem.Text += " " + _currentItem.PackageInfo + " " + _currentItem.Brand + " (" + _QuantityItemText + ")";

                        //group
                        if (_currentItem.categoryString != null && _currentItem.categoryString != string.Empty)
                        {
                            if (_DictionaryCategoryIndex.ContainsKey(_currentItem.categoryString)) // True
                                _clonedLVItem.Group = listView_LargeIcon.Groups[_DictionaryCategoryIndex[_currentItem.categoryString]];
                        }
                        else
                        {
                            //_clonedLVItem.Group = listView_LargeIcon.Groups[_DictionaryCategoryIndex["Nenhum"]];
                            _clonedLVItem.Group = listView_LargeIcon.Groups[_DictionaryCategoryIndex["None"]];
                        }

                        listView_LargeIcon.Items.Add(_clonedLVItem);
                        CurrentListViewItemsLargeIcon.Add((ListViewItem)_clonedLVItem.Clone());
                    }
                    catch (Exception ex)
                    {
                        string s = ex.Message;
                    }
                }


                //copy the item to the list for searching
                CurrentListViewItemsDetails.Add((ListViewItem)_lvItem.Clone());

                if (_imageLoaded)
                    _itemIndex = ++_itemIndex;
            }
            listView_LargeIcon.LargeImageList = imageList1;
            listView_LargeIcon.EndUpdate();
            listView_Details.EndUpdate();

            //save listviewitemcolletion for searching puorposes
            //listView_Details.Items.CopyTo(
            textBox_Search.Text = "";

            CurrentTotalPrice = _finaltotalPrice;

            label_TotalPrice.Text = CurrentTotalPrice.ToString() + " €";
            //if (UpdateStatusBar != null)
            //    UpdateStatusBar(this, "Total da Lista de Compras : " + _finaltotalPrice.ToString() + " €");
        }

        float CurrentTotalPrice = 0;
        private void apagarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if is product

            //detalhes
            if (tabControl2.SelectedIndex == 1)
            {
                foreach (ListViewItem _item in listView_Details.SelectedItems)
                {
                    GlobalVariables.SpiroStockManagmentDatabaseProcedures.DeleteProduct((_item.Tag as SpiroStockManagmentDatabaseClass.Objects.Product).Id);
                }
            }
            //Icons
            else
            {
                foreach (ListViewItem _item in listView_LargeIcon.SelectedItems)
                {
                    GlobalVariables.SpiroStockManagmentDatabaseProcedures.DeleteProduct((_item.Tag as SpiroStockManagmentDatabaseClass.Objects.Product).Id);
                }
            }

            RefreshList();
        }

        private void button_AddItem_Click(object sender, EventArgs e)
        {
            InsertItem _InsertItem = new InsertItem();
            if (CurrentListName.ToLower() == "in")
                _InsertItem.Initialize("in");
            if (CurrentListName.ToLower() == "out")
                _InsertItem.Initialize("out");

            GlobalVariables.CurrentInsertItemDialog = _InsertItem;
            _InsertItem.ShowDialog();

            
            RefreshList();
            this.Parent.Focus();
        }

        private void apagarDaListaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //detalhes
            if (tabControl2.SelectedIndex == 1)
            {
                //if (MessageBox.Show("Tem a certeza que quer apagar este/s " + listView_Details.SelectedItems.Count + " da " + groupBox1.Text, "Confirmação", MessageBoxButtons.YesNo) == DialogResult.No) return;
                if (MessageBox.Show("Are you shure you want to delete this " + listView_Details.SelectedItems.Count + " products of " + groupBox1.Text, "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

                foreach (ListViewItem _item in listView_Details.SelectedItems)
                {
                    GlobalVariables.SpiroStockManagmentDatabaseProcedures.DeleteItemFromList((_item.Tag as SpiroStockManagmentDatabaseClass.Objects.Product).Id, CurrentListName);
                    CurrentListViewItemsDetails.RemoveAt(_item.Index);
                    _item.Remove();
                }
            }
            //Icons
            else
            {
                //if (MessageBox.Show("Tem a certeza que quer apagar este/s " + listView_LargeIcon.SelectedItems.Count + " da " + groupBox1.Text, "Confirmação", MessageBoxButtons.YesNo) == DialogResult.No) return;
                if (MessageBox.Show("Are you shure you want to delete this " + listView_LargeIcon.SelectedItems.Count + " products of " + groupBox1.Text, "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;
                
                foreach (ListViewItem _item in listView_LargeIcon.SelectedItems)
                {
                    GlobalVariables.SpiroStockManagmentDatabaseProcedures.DeleteItemFromList((_item.Tag as SpiroStockManagmentDatabaseClass.Objects.Product).Id, CurrentListName);
                    CurrentListViewItemsLargeIcon.RemoveAt(_item.Index);
                    _item.Remove();
                }
            }
            label_TotalPrice.Text = GetTotalListPrice() + " €";
            //RefreshList();
        }

        private void button_Print_Click(object sender, EventArgs e)
        {
            Print();
        }

        public void Print()
        {
            //listViewPrinter1.PrintPreview();
            PrintListView _PrintListViewForm = new PrintListView();
            _PrintListViewForm.Initialize(listView_Details);
            _PrintListViewForm.FormClosed += new FormClosedEventHandler(_PrintListViewForm_FormClosed);
            _PrintListViewForm.Show();
        }

        void _PrintListViewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            tabControl2.Focus();
            //this.ParentForm.Focus();
        }

        public void SelectAllItems()
        {
            //detalhes
            if (tabControl2.SelectedIndex == 1)
            {
                for (int i = 0; i < listView_Details.Items.Count; i++)
                {
                    listView_Details.Items[i].Selected = true;
                }
                listView_Details.Focus();
            }
            else
            {
                for (int i = 0; i < listView_LargeIcon.Items.Count; i++)
                {
                    listView_LargeIcon.Items[i].Selected = true;       
                }
                listView_LargeIcon.Focus();
            }
        }

        private void listView_Details_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; MO to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            (sender as ListView).Sort();
        }

        private void textBox_Search_TextChanged(object sender, EventArgs e)
        {
            string _textToSearch = textBox_Search.Text.ToLower();

            if (_textToSearch == string.Empty)
            {
                CurrentlySearching = false;
                RefreshList();
                return;
            }
            else
            {
                CurrentlySearching = true;
                comboBox_Category.SelectedIndex = 0;
            }


            //is details
            if (tabControl2.SelectedIndex == 1)
            {
                if (CurrentListViewItemsDetails.Count > 0)
                {
                    listView_Details.Items.Clear();
                    listView_Details.BeginUpdate();

                    foreach (ListViewItem _item in CurrentListViewItemsDetails)
                    {
                        if (_item.Text.ToLower().IndexOf(_textToSearch) > -1)
                        {
                            listView_Details.Items.Add((ListViewItem)_item.Clone());
                            continue;
                        }
                        for (int i = 0; i < _item.SubItems.Count; i++)
                        {

                            if (_item.SubItems[i].Text.ToLower().IndexOf(_textToSearch) > -1)
                            {
                                listView_Details.Items.Add((ListViewItem)_item.Clone());
                                continue;
                            }
                        }
                    }
                    listView_Details.EndUpdate();
                }
            }
            //is LargeIcon
            else
            {
                if (CurrentListViewItemsLargeIcon.Count > 0)
                {
                    listView_LargeIcon.Items.Clear();
                    listView_LargeIcon.BeginUpdate();

                    foreach (ListViewItem _item in CurrentListViewItemsLargeIcon)
                    {
                        if (_item.Text.ToLower().IndexOf(_textToSearch) > -1)
                        {
                            listView_LargeIcon.Items.Add((ListViewItem)_item.Clone());
                            continue;
                        }
                        for (int i = 0; i < _item.SubItems.Count; i++)
                        {

                            if (_item.SubItems[i].Text.ToLower().IndexOf(_textToSearch) > -1)
                            {
                                listView_LargeIcon.Items.Add((ListViewItem)_item.Clone());
                                continue;
                            }
                        }
                    }
                    listView_LargeIcon.EndUpdate();
                }
            }
        }

        private void adicionarÁListaDeInventórioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //detalhes
            if (tabControl2.SelectedIndex == 1)
            {
                foreach (ListViewItem _ListViewItem in listView_Details.SelectedItems)
                {
                    InsertItem _InsertItemDialog = new InsertItem();
                    _InsertItemDialog.Initialize((SpiroStockManagmentDatabaseClass.Objects.Product)_ListViewItem.Tag, "in");
                    GlobalVariables.CurrentInsertItemDialog = _InsertItemDialog;
                    _InsertItemDialog.FormClosed += new FormClosedEventHandler(_InsertItemDialog_FormClosed);
                    _InsertItemDialog.ShowDialog();
                }
            }
            //Icons
            else
            {
                foreach (ListViewItem _ListViewItem in listView_LargeIcon.SelectedItems)
                {
                    InsertItem _InsertItemDialog = new InsertItem();
                    _InsertItemDialog.Initialize((SpiroStockManagmentDatabaseClass.Objects.Product)_ListViewItem.Tag, "in");
                    _InsertItemDialog.ShowDialog();
                }
            }
        }

        void _InsertItemDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (GlobalVariables.CurrentInsertItemDialog.DataHasBeenChanged) RefreshList();
            GlobalVariables.CurrentInsertItemDialog = null;
        }

        private void adicionarÁListaDeComprasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //detalhes
            if (tabControl2.SelectedIndex == 1)
            {
                foreach (ListViewItem _ListViewItem in listView_Details.SelectedItems)
                {
                    InsertItem _InsertItemDialog = new InsertItem();
                    _InsertItemDialog.Initialize((SpiroStockManagmentDatabaseClass.Objects.Product)_ListViewItem.Tag, "out");
                    _InsertItemDialog.FormClosed += new FormClosedEventHandler(_InsertItemDialog_FormClosed);
                    GlobalVariables.CurrentInsertItemDialog = _InsertItemDialog;
                    _InsertItemDialog.ShowDialog();
                }
            }
            //Icons
            else
            {
                foreach (ListViewItem _ListViewItem in listView_LargeIcon.SelectedItems)
                {
                    InsertItem _InsertItemDialog = new InsertItem();
                    _InsertItemDialog.Initialize((SpiroStockManagmentDatabaseClass.Objects.Product)_ListViewItem.Tag, "out");
                    _InsertItemDialog.FormClosed += new FormClosedEventHandler(_InsertItemDialog_FormClosed);
                    GlobalVariables.CurrentInsertItemDialog = _InsertItemDialog;
                    _InsertItemDialog.ShowDialog();
                }
            }
        }

        ItemInfoAndChangeQuantity CurrentItemInfoAndChangeQuantity = null;
        ListViewItem CurrentListViewDetailsSelectedItem = null;
        Point CurrentListViewItemHoverLocation = new Point(0, 0);
        private void listView_Details_ItemMouseHover(object sender, System.Windows.Forms.ListViewItemMouseHoverEventArgs e)
        {
            if (CurrentItemInfoAndChangeQuantity != null) CurrentItemInfoAndChangeQuantity.Close();

            //this.Cursor = new Cursor(Cursor.Current.Handle);
            //Cursor.Position = new Point(Cursor.Position.X - 50, Cursor.Position.Y - 50);
            //Cursor.Clip = new Rectangle(this.Location, this.Size);

            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            if ((sender as ListView).SelectedItems.Count > 0)
                CurrentListViewDetailsSelectedItem = (sender as ListView).SelectedItems[0];

            CurrentListViewDetailsSelectedItem = e.Item;

            //Point _p = Cursor.Position;
            Point _p = new Point(w32Mouse.X, w32Mouse.Y);
            CurrentListViewItemHoverLocation = _p;

            timer_ItemHover.Enabled = true;
            timer_ItemHover.Start();

            return;

            int _screenWidth = Screen.PrimaryScreen.Bounds.Width;
            ItemInfoAndChangeQuantity _ItemInfoAndChangeQuantity = new ItemInfoAndChangeQuantity();

            _ItemInfoAndChangeQuantity.Initialize((SpiroStockManagmentDatabaseClass.Objects.Product)e.Item.Tag, CurrentListName, true);
            //_ItemInfoAndChangeQuantity.Location = _p;
            if (_ItemInfoAndChangeQuantity.Width + _p.X > _screenWidth)
            {
                _ItemInfoAndChangeQuantity.startX = _p.X - _ItemInfoAndChangeQuantity.Width - 5;
            }
            else
            {
                _ItemInfoAndChangeQuantity.startX = _p.X + 5;
            }
            if (_p.Y + _ItemInfoAndChangeQuantity.Height > this.ParentForm.Height)
            {
                _ItemInfoAndChangeQuantity.startY = _p.Y - _ItemInfoAndChangeQuantity.Height;
            }
            else
            {
                _ItemInfoAndChangeQuantity.startY = _p.Y;
            }
            //_ItemInfoAndChangeQuantity.startY = _p.Y;
            _ItemInfoAndChangeQuantity.FormClosed += new System.Windows.Forms.FormClosedEventHandler(_ItemInfoAndChangeQuantity_FormClosed);
            _ItemInfoAndChangeQuantity.Show();
            CurrentItemInfoAndChangeQuantity = _ItemInfoAndChangeQuantity;
        }

        void _ItemInfoAndChangeQuantity_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            //change just the item
            //CurrentListViewDetailsSelectedItem
            ItemInfoAndChangeQuantity _currentItemInfoAndChangeQuantity = ((ItemInfoAndChangeQuantity)sender);
            string _QuantityItemText = "";

            if (_currentItemInfoAndChangeQuantity.QuantityChanged)
            {
                if (CurrentListName.ToLower() != "all")
                {
                    //_productItem = (SpiroStockManagmentDatabaseClass.Objects.Item)(from n in _currentItem.ItemLists where n.ListName == CurrentListName select n).First();
                    if (CurrentListName.ToLower() == "in")
                    {
                        //if zero delete
                        if (_currentItemInfoAndChangeQuantity.CurrentProduct.QuantityWeightIn == 0 && _currentItemInfoAndChangeQuantity.CurrentProduct.QuantityIn == 0)
                        {
                            CurrentListViewDetailsSelectedItem.Remove();
                        }

                        if (_currentItemInfoAndChangeQuantity.CurrentProduct.QuantityWeightIn > 0 && _currentItemInfoAndChangeQuantity.CurrentProduct.QuantityIn > 0)
                            _QuantityItemText = _currentItemInfoAndChangeQuantity.CurrentProduct.QuantityIn.ToString() + " emb + " + _currentItemInfoAndChangeQuantity.CurrentProduct.QuantityWeightIn + " kg";
                        else
                        {
                            if (_currentItemInfoAndChangeQuantity.CurrentProduct.QuantityIn > 0)
                                _QuantityItemText = _currentItemInfoAndChangeQuantity.CurrentProduct.QuantityIn.ToString();
                            if (_currentItemInfoAndChangeQuantity.CurrentProduct.QuantityWeightIn > 0)
                                _QuantityItemText = _currentItemInfoAndChangeQuantity.CurrentProduct.QuantityWeightIn + " kg";
                        }

                        label_TotalPrice.Text = GetTotalListPrice().ToString() + " €";
                        //change total price
                        //if (UpdateStatusBar != null)
                        //    UpdateStatusBar(this, "Total da Lista de Compras : " + GetTotalListPrice().ToString() + " €");
                    }
                    if (CurrentListName.ToLower() == "out")
                    {
                        //if zero delete
                        if (_currentItemInfoAndChangeQuantity.CurrentProduct.QuantityWeightOut == 0 && _currentItemInfoAndChangeQuantity.CurrentProduct.QuantityOut == 0)
                        {
                            CurrentListViewDetailsSelectedItem.Remove();
                        }
                        if (_currentItemInfoAndChangeQuantity.CurrentProduct.QuantityWeightOut > 0 && _currentItemInfoAndChangeQuantity.CurrentProduct.QuantityOut > 0)
                            //_QuantityItemText = _currentItemInfoAndChangeQuantity.CurrentProduct.QuantityOut.ToString() + " emb + " + _currentItemInfoAndChangeQuantity.CurrentProduct.QuantityWeightOut + " kg";
                            _QuantityItemText = _currentItemInfoAndChangeQuantity.CurrentProduct.QuantityOut.ToString() + " package + " + _currentItemInfoAndChangeQuantity.CurrentProduct.QuantityWeightOut + " kg";
                        else
                        {
                            if (_currentItemInfoAndChangeQuantity.CurrentProduct.QuantityOut > 0)
                                _QuantityItemText = _currentItemInfoAndChangeQuantity.CurrentProduct.QuantityOut.ToString();
                            if (_currentItemInfoAndChangeQuantity.CurrentProduct.QuantityWeightOut > 0)
                                _QuantityItemText = _currentItemInfoAndChangeQuantity.CurrentProduct.QuantityWeightOut + " kg";
                        }

                        //change total price
                        //change total price
                        label_TotalPrice.Text = GetTotalListPrice().ToString() + " €";
                        //if (UpdateStatusBar != null)
                        //    UpdateStatusBar(this, "Total da Lista de Compras : " + GetTotalListPrice().ToString() + " €");
                    }
                }
                CurrentListViewDetailsSelectedItem.Text = _currentItemInfoAndChangeQuantity.CurrentProduct.Name + " " + _currentItemInfoAndChangeQuantity.CurrentProduct.Brand + " ( " + _QuantityItemText + " )";
                CurrentListViewDetailsSelectedItem.Tag = _currentItemInfoAndChangeQuantity.CurrentProduct;
                //RefreshList();
            }
        }

        private void button_SeeInText_Click(object sender, EventArgs e)
        {
            ListView _listViewToCopyItemsToText = null;
            //details
            if (tabControl2.SelectedIndex == 1)
                _listViewToCopyItemsToText = listView_Details;
            //LargeIcon
            else
                _listViewToCopyItemsToText = listView_LargeIcon;

            List<string> _text = new List<string>();
            foreach (ListViewItem _ListViewItem in _listViewToCopyItemsToText.Items)
            {
                SpiroStockManagmentDatabaseClass.Objects.Product _product = (SpiroStockManagmentDatabaseClass.Objects.Product)_ListViewItem.Tag;
                //SpiroStockManagmentDatabaseClass.Objects.Item _productItem = (SpiroStockManagmentDatabaseClass.Objects.Item)(from n in _product.ItemLists where n.ListName == CurrentListName select n).First();

                string _string = _product.Name + " " + _product.Brand;
                string _QuantityItemText = "";

                if (CurrentListName.ToLower() == "in")
                {
                    if (_product.QuantityWeightIn > 0 && _product.QuantityIn > 0)
                        //_QuantityItemText = _product.QuantityIn.ToString() + " emb + " + _product.QuantityWeightIn + " kg";
                        _QuantityItemText = _product.QuantityIn.ToString() + " package + " + _product.QuantityWeightIn + " kg";
                    else
                    {
                        if (_product.QuantityIn > 0)
                            _QuantityItemText = _product.QuantityIn.ToString();
                        if (_product.QuantityWeightIn > 0)
                            _QuantityItemText = _product.QuantityWeightIn + " kg";
                    }
                }
                if (CurrentListName.ToLower() == "out")
                {
                    if (_product.QuantityWeightOut > 0 && _product.QuantityOut > 0)
                        //_QuantityItemText = _product.QuantityOut.ToString() + " emb + " + _product.QuantityWeightOut + " kg";
                        _QuantityItemText = _product.QuantityOut.ToString() + " package + " + _product.QuantityWeightOut + " kg";
                    else
                    {
                        if (_product.QuantityOut > 0)
                            _QuantityItemText = _product.QuantityOut.ToString();
                        if (_product.QuantityWeightOut > 0)
                            _QuantityItemText = _product.QuantityWeightOut + " kg";
                    }
                }
                _string += " (" + _QuantityItemText + ")";

                _text.Add(_string);
            }
            if (_text.Count > 0)
            {
                ViewListInText _ViewListInText = new ViewListInText();
                _ViewListInText.Initialize(_text);
                _ViewListInText.ShowDialog();
            }
        }

        private void apagarProdutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //detalhes
            if (tabControl2.SelectedIndex == 1)
            {
                //if (MessageBox.Show("Tem a certeza que quer apagar este/s " + listView_Details.SelectedItems.Count + " permanentemente?", "Confirmação", MessageBoxButtons.YesNo) == DialogResult.No) return;
                if (MessageBox.Show("Are you shure you want to delete this/s " + listView_Details.SelectedItems.Count + " permanently?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;
                
                foreach (ListViewItem _item in listView_Details.SelectedItems)
                {
                    GlobalVariables.SpiroStockManagmentDatabaseProcedures.DeleteProduct((_item.Tag as SpiroStockManagmentDatabaseClass.Objects.Product).Id);
                }
            }
            //Icons
            else
            {
                //if (MessageBox.Show("Tem a certeza que quer apagar este/s " + listView_LargeIcon.SelectedItems.Count + " permanentemente?", "Confirmação", MessageBoxButtons.YesNo) == DialogResult.No) return;
                if (MessageBox.Show("Are you shure you want to delete this/s " + listView_LargeIcon.SelectedItems.Count + " permanently?", "Confirmação", MessageBoxButtons.YesNo) == DialogResult.No) return;
                foreach (ListViewItem _item in listView_LargeIcon.SelectedItems)
                {
                    GlobalVariables.SpiroStockManagmentDatabaseProcedures.DeleteProduct((_item.Tag as SpiroStockManagmentDatabaseClass.Objects.Product).Id);
                }
            }

            RefreshList();
        }

        private void listView_Details_AfterLabelEdit(object sender, System.Windows.Forms.LabelEditEventArgs e)
        {
            e.CancelEdit = true;
        }

        private void listView_LargeIcon_AfterLabelEdit(object sender, System.Windows.Forms.LabelEditEventArgs e)
        {
            e.CancelEdit = true;
        }

        private void editarProdutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl2.SelectedIndex == 1)
            {
                if (listView_Details.SelectedItems.Count > 0)
                {
                    InsertItem _InsertItem = new InsertItem();
                    _InsertItem.Initialize(listView_Details.SelectedItems[0].Tag as SpiroStockManagmentDatabaseClass.Objects.Product, "all");
                    _InsertItem.FormClosed += new FormClosedEventHandler(_InsertItemDialog_FormClosed);
                    GlobalVariables.CurrentInsertItemDialog = _InsertItem;
                    _InsertItem.ShowDialog();
                }
            }
            //Icons
            else
            {
                if (listView_LargeIcon.SelectedItems.Count > 0)
                {
                    InsertItem _InsertItem = new InsertItem();
                    _InsertItem.Initialize(listView_LargeIcon.SelectedItems[0].Tag as SpiroStockManagmentDatabaseClass.Objects.Product, "all");
                    _InsertItem.FormClosed += new FormClosedEventHandler(_InsertItemDialog_FormClosed);
                    GlobalVariables.CurrentInsertItemDialog = _InsertItem;
                    _InsertItem.ShowDialog();
                }
            }
            RefreshList();
        }

        bool CurrentlySearching = false;
        private void comboBox_Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentlySearching) return;

            string _selectedCategory = comboBox_Category.SelectedItem.ToString();
            RefreshList();
            //is details
            if (tabControl2.SelectedIndex == 1)
            {
                if (CurrentListViewItemsDetails.Count > 0)
                {
                    listView_Details.Items.Clear();
                    listView_Details.BeginUpdate();

                    foreach (ListViewItem _item in CurrentListViewItemsDetails)
                    {
                        //if (_item.Group.Header.ToLower() == _selectedCategory.ToLower() || _selectedCategory == "Todas")
                        if (_item.Group.Header.ToLower() == _selectedCategory.ToLower() || _selectedCategory == "All")
                        {
                            listView_Details.Items.Add((ListViewItem)_item.Clone());
                            continue;
                        }
                    }
                    for (int i = 0; i < listView_Details.Groups.Count; i++)
                    {
                        if (listView_Details.Groups[i].Items.Count == 0)
                        {
                            listView_Details.Groups.RemoveAt(i--);
                        }
                    }
                    listView_Details.EndUpdate();
                }
            }
            //is LargeIcon
            else
            {
                if (CurrentListViewItemsLargeIcon.Count > 0)
                {
                    listView_LargeIcon.Items.Clear();
                    listView_LargeIcon.BeginUpdate();

                    foreach (ListViewItem _item in CurrentListViewItemsLargeIcon)
                    {
                        //if (_item.Group.Header.ToLower() == _selectedCategory.ToLower() || _selectedCategory == "Todas")
                        if (_item.Group.Header.ToLower() == _selectedCategory.ToLower() || _selectedCategory == "All")
                        {
                            listView_LargeIcon.Items.Add((ListViewItem)_item.Clone());
                            continue;
                        }
                    }

                    for (int i = 0; i < listView_LargeIcon.Groups.Count; i++)
                    {
                        if (listView_LargeIcon.Groups[i].Items.Count == 0)
                        {
                            listView_LargeIcon.Groups.RemoveAt(i--);
                        }
                    }
                    listView_LargeIcon.EndUpdate();
                }
            }
        }


        public float GetTotalListPrice()
        {
            IEnumerable<XElement> _list = null;
            List<string> _categoriesOfList = null;
            float _totalPrice = 0;


            if (CurrentListName == "in")
            {
                _list = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllInItems();
                _categoriesOfList = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllProductCategoriesInList();


            }
            if (CurrentListName == "out")
            {
                _list = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllOutItems();
                _categoriesOfList = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllProductCategoriesOutList();
            }

            if (CurrentListName == "all")
            {
                _list = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllProducts();
                _categoriesOfList = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllProductCategories();
            }

            foreach (XElement _item in _list)
            {
                SpiroStockManagmentDatabaseClass.Objects.Product _currentItem = SpiroStockManagmentDatabaseClass.XmlSerializerExtension.DeSerializer(_item);

                if (CurrentListName.ToLower() != "all")
                {
                    if (CurrentListName.ToLower() == "in")
                    {
                        if (_currentItem.QuantityIn > 0)
                            _totalPrice += _currentItem.Price * _currentItem.QuantityIn;

                        if (_currentItem.QuantityWeightIn > 0)
                        {
                            float variableWeightByKg = GlobalProcedures.GetVariableWeightByKgOfString(_currentItem.VariableWeightPrice);
                            _totalPrice += variableWeightByKg * _currentItem.QuantityWeightIn;
                        }
                    }
                    if (CurrentListName.ToLower() == "out")
                    {
                        if (_currentItem.QuantityOut > 0)
                            _totalPrice += _currentItem.Price * _currentItem.QuantityOut;

                        if (_currentItem.QuantityWeightOut > 0)
                        {
                            float variableWeightByKg = GlobalProcedures.GetVariableWeightByKgOfString(_currentItem.VariableWeightPrice);
                            _totalPrice += variableWeightByKg * _currentItem.QuantityWeightOut;
                        }
                    }
                }
            }
            return _totalPrice;
        }

        private void timer_ItemHover_Tick(object sender, EventArgs e)
        {
            timer_ItemHover.Stop();

            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            Point _p = new Point(w32Mouse.X, w32Mouse.Y);
            //CurrentListViewItemHoverLocation = _p;

            int _XDifference = _p.X - CurrentListViewItemHoverLocation.X;
            int _YDifference = _p.Y - CurrentListViewItemHoverLocation.Y;

            if ((_XDifference <= 10 && _XDifference >= -10) && (_YDifference <= 10 && _YDifference >= -10))
            {
                int _screenWidth = Screen.PrimaryScreen.Bounds.Width;
                ItemInfoAndChangeQuantity _ItemInfoAndChangeQuantity = new ItemInfoAndChangeQuantity();

                _ItemInfoAndChangeQuantity.Initialize((SpiroStockManagmentDatabaseClass.Objects.Product)CurrentListViewDetailsSelectedItem.Tag, CurrentListName, true);
                //_ItemInfoAndChangeQuantity.Location = _p;
                if (_ItemInfoAndChangeQuantity.Width + _p.X > _screenWidth)
                {
                    _ItemInfoAndChangeQuantity.startX = _p.X - _ItemInfoAndChangeQuantity.Width - 5;
                }
                else
                {
                    _ItemInfoAndChangeQuantity.startX = _p.X + 5;
                }
                if (_p.Y + _ItemInfoAndChangeQuantity.Height > this.ParentForm.Height)
                {
                    _ItemInfoAndChangeQuantity.startY = _p.Y - _ItemInfoAndChangeQuantity.Height;
                }
                else
                {
                    _ItemInfoAndChangeQuantity.startY = _p.Y;
                }
                //_ItemInfoAndChangeQuantity.startY = _p.Y;
                _ItemInfoAndChangeQuantity.FormClosed += new System.Windows.Forms.FormClosedEventHandler(_ItemInfoAndChangeQuantity_FormClosed);
                _ItemInfoAndChangeQuantity.Show();
                CurrentItemInfoAndChangeQuantity = _ItemInfoAndChangeQuantity;
            }
        }

        private void adicionarÀListaDeComprasOnlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddToOnlineStore _AddToOnlineStore = new AddToOnlineStore();
            _AddToOnlineStore.Initialize(listView_Details);
            _AddToOnlineStore.Show();
        }
        //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        //{
        //    return true;
        //    return base.ProcessCmdKey(ref msg, keyData);
        //}
    }
}
