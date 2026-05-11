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

namespace SpiroStockManagement
{
    public partial class StockList : UserControl
    {
        public StockList()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            //Initialize details view
            listView_Details.Items.Clear();
            listView_Details.BeginUpdate();

            listView_LargeIcon.Items.Clear();
            listView_LargeIcon.BeginUpdate();
            imageList1.Images.Clear();

            int _itemIndex = 0;
            foreach (XElement _item in GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllInItems())
            {
                SpiroStockManagmentDatabaseClass.Objects.Product _currentItem = SpiroStockManagmentDatabaseClass.XmlSerializerExtension.DeSerializer(_item);
                ListViewItem _lvItem = new ListViewItem();
                _lvItem.Text = _currentItem.Name;
                _lvItem.SubItems.Add(_currentItem.Brand);
                _lvItem.SubItems.Add(_currentItem.PackageInfo);

                SpiroStockManagmentDatabaseClass.Objects.Item _productItem = (SpiroStockManagmentDatabaseClass.Objects.Item)(from n in _currentItem.ItemLists where n.ListName == "in" select n).First();

                try
                {
                    string _QuantityItemText = "";

                    if (_productItem.QuantityWeight > 0 && _productItem.Quantity > 0)
                    {
                        _QuantityItemText = _productItem.Quantity.ToString() + " emb + " + _productItem.QuantityWeight + " kg";
                    }
                    else
                    {
                        if (_productItem.Quantity > 0)
                        {
                            _QuantityItemText = _productItem.Quantity.ToString();
                        }
                        if (_productItem.QuantityWeight > 0)
                        {
                            _QuantityItemText = _productItem.QuantityWeight + " kg";
                        }
                    }


                    _lvItem.SubItems.Add(_QuantityItemText);
                }
                catch (Exception ex)
                {
                    _lvItem.SubItems.Add(_productItem.QuantityWeight + "kg");
                }
                _lvItem.SubItems.Add(_currentItem.Price.ToString());

                //price wieght
                _lvItem.SubItems.Add(_currentItem.VariableWeightPrice);


                //total price
                float _totalPrice = 0;
                if (_productItem.Quantity > 0)
                    _totalPrice = _currentItem.Price * _productItem.Quantity;

                if (_productItem.QuantityWeight > 0)
                {
                    string variableWeightByKgString = _currentItem.VariableWeightPrice.Substring(2);
                    variableWeightByKgString = variableWeightByKgString.Substring(0, variableWeightByKgString.IndexOf('/'));
                    variableWeightByKgString = variableWeightByKgString.Trim();
                    float variableWeightByKg = float.Parse(variableWeightByKgString);




                    _totalPrice += variableWeightByKg * _productItem.QuantityWeight;

                }
                _lvItem.SubItems.Add(_totalPrice + " €");


                //add the image to the imageList
                imageList1.Images.Add(_currentItem.Id.ToString(), Image.FromFile(Application.StartupPath + "\\ItemsImages\\" + _currentItem.PictureSmallFilename));

                _lvItem.Tag = _currentItem.Id;

                if (_productItem.QuantityWeight != 0 || _productItem.Quantity != 0)
                {
                    listView_Details.Items.Add(_lvItem);

                    ListViewItem _clonedLVItem = (ListViewItem)_lvItem.Clone();
                    _clonedLVItem.ImageIndex = _itemIndex;
                    listView_LargeIcon.Items.Add(_clonedLVItem);
                }
                _itemIndex = ++_itemIndex;
            }
            listView_LargeIcon.LargeImageList = imageList1;
            listView_LargeIcon.EndUpdate();

            listView_Details.EndUpdate();
        }

        private void apagarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //detalhes
            if (tabControl2.SelectedIndex == 0)
            {
                foreach (ListViewItem _item in listView_Details.SelectedItems)
                {
                    GlobalVariables.SpiroStockManagmentDatabaseProcedures.DeleteItem((int)_item.Tag);
                }
            }
            //Icons
            else
            {
                foreach (ListViewItem _item in listView_LargeIcon.SelectedItems)
                {
                    GlobalVariables.SpiroStockManagmentDatabaseProcedures.DeleteItem((int)_item.Tag);
                }
            }
            Initialize();
        }

        private void apagarDaListaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //detalhes
            if (tabControl2.SelectedIndex == 0)
            {
                foreach (ListViewItem _item in listView_Details.SelectedItems)
                {
                    GlobalVariables.SpiroStockManagmentDatabaseProcedures.DeleteItemFromList((int)_item.Tag);
                }
            }
            //Icons
            else
            {
                foreach (ListViewItem _item in listView_LargeIcon.SelectedItems)
                {
                    GlobalVariables.SpiroStockManagmentDatabaseProcedures.DeleteItemFromList((int)_item.Tag);
                }
            }
            Initialize();
        }

        private void button_AddItem_Click(object sender, EventArgs e)
        {
            InsertItem _InsertItem = new InsertItem();
            _InsertItem.Initialize("in");
            _InsertItem.ShowDialog();
            Initialize();
        }
    }
}
