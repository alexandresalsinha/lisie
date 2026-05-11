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
    public partial class AddToOnlineStore : Form
    {
        public AddToOnlineStore()
        {
            InitializeComponent();
            listView1.OwnerDraw = true;
        }

        ListView CurrentListView = null;
        public void Initialize(ListView listview)
        {
            CurrentListView = listview;
            InitializeListView();

        }

        void InitializeListView()
        {
            listView1.Items.Clear();
            listView1.BeginUpdate();
            listView1.Columns.Clear();
            listView1.Groups.Clear();

            Dictionary<string, int> _DictionaryGroupsIndex = new Dictionary<string, int>();
            int _categoriesCountIndex = 0;

            //groups
            foreach (ListViewGroup _ListViewGroup in CurrentListView.Groups)
            {
                listView1.Groups.Add(new ListViewGroup { Header = _ListViewGroup.Header, HeaderAlignment = _ListViewGroup.HeaderAlignment });
                _DictionaryGroupsIndex.Add(_ListViewGroup.Header, _categoriesCountIndex++);
            }
            //columns
            foreach (ColumnHeader _ColumnHeader in CurrentListView.Columns)
            {
                listView1.Columns.Add(new ColumnHeader { Text = "  " + _ColumnHeader.Text, Width = _ColumnHeader.Width });
            }

            //items
            foreach (ListViewItem _ListViewItem in CurrentListView.Items)
            {
                ListViewItem _newListViewItem = new ListViewItem();
                _newListViewItem.Text = _ListViewItem.Text;
                _newListViewItem.Tag = _ListViewItem.Tag;
                int _temp = 0;
                foreach (System.Windows.Forms.ListViewItem.ListViewSubItem _subItem in _ListViewItem.SubItems)
                {
                    if (_temp != 0)
                    {
                        _newListViewItem.SubItems.Add(_subItem.Text);
                    }
                    else
                        ++_temp;
                }
                if (_ListViewItem.Group != null)
                {
                    if (_DictionaryGroupsIndex.ContainsKey(_ListViewItem.Group.Header)) // True
                        _newListViewItem.Group = listView1.Groups[_DictionaryGroupsIndex[_ListViewItem.Group.Header]];
                }
                _newListViewItem.Checked = true;
                listView1.Items.Add(_newListViewItem);
            }

            listView1.EndUpdate();
            listView1.ShowGroups = listView1.ShowGroups;
        }

        int currentProccessingItemIndex = 0;
        bool loginSuccesful, isProccessingProducts = false;
        SpiroStockManagmentDatabaseClass.Objects.Product CurrentProduct = null;
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Document Loaded" + webBrowser1.Url.AbsolutePath);
            //HtmlElement _popudDiv = webBrowser1.Document.GetElementById("dPopupBox");

            //if (_popudDiv != null)
            //{
            //    _popudDiv.AttachEventHandler("onpropertychange", new EventHandler(handler));
            //}
            //if (webBrowser1.Url.AbsolutePath != "http://www.jumbo.pt") return;
            if (webBrowser1.Url.AbsoluteUri.IndexOf("/Frontoffice/ContentPages/JumboNetWelcome.aspx") > -1 && loginSuccesful == false)
            {
                HtmlElementCollection elements = webBrowser1.Document.GetElementsByTagName("input");
                HtmlElement _emailElement = webBrowser1.Document.GetElementById("txtEmail");
                HtmlElement _passwordElement = webBrowser1.Document.GetElementById("txtPass");
                HtmlElement _submitElement = webBrowser1.Document.GetElementById("M_M_rPH_cLogin_lnkDoLogin");
                if (_emailElement != null && _passwordElement != null && _submitElement != null)
                {
                    _emailElement.SetAttribute("value", "alexandresalsinha@gmail.com");
                    _passwordElement.SetAttribute("value", "1234qwrefdsa");
                    _submitElement.InvokeMember("click");
                }
                //check if login was sucesfull
                else
                {
                    if (webBrowser1.Document.Body.InnerHtml.ToLower().IndexOf("alexandre") > -1)
                    {
                        loginSuccesful = true;
                        //delete shopping car
                        if (checkBox1.Checked)
                        {
                            //isTryingToAccessBuyingBasket = true;
                            webBrowser1.Navigate("https://www.jumbo.pt/Frontoffice/ContentPages/BuyShoppingBasket.aspx");
                            return;
                        }
                        timer_NavigateToNextProduct.Enabled = true;
                        timer_NavigateToNextProduct.Start();
                        //NavigateToNextProduct();
                    }
                }
            }
            if (webBrowser1.Url.AbsoluteUri.IndexOf("/Frontoffice/ContentPages/JumboNetWelcome.aspx") > -1 && loginSuccesful == true && isProccessingProducts == false)
            {
                NavigateToNextProduct();
            }
            //if (isTryingToAccessBuyingBasket)
            //{
            //    isTryingToAccessBuyingBasket = false;
            //    NavigateToNextProduct();
            //}
            //delete shopping car
            if (webBrowser1.Url.AbsoluteUri.IndexOf("/Frontoffice/ContentPages/BuyShoppingBasket.aspx") > -1)
            {


                //id chkSelAll
                //class btRemover
                HtmlElement _checkAllElement = webBrowser1.Document.GetElementById("chkSelAll");
                if (_checkAllElement != null)
                {
                    _checkAllElement.InvokeMember("click");
                }
                HtmlElement _elements = webBrowser1.Document.GetElementById("M_lPH_SB_ordRev_dToolBar");
                HtmlElementCollection _elementsLink = _elements.GetElementsByTagName("A");

                foreach (HtmlElement _HtmlElementitem in _elementsLink)
                {
                    if (_HtmlElementitem.OuterHtml != null)
                    {
                        string _temp = _HtmlElementitem.OuterHtml.Substring(_HtmlElementitem.OuterHtml.IndexOf("class=") + 6);

                        string _temp2 = "";
                        if (_temp.IndexOf(">") < _temp.IndexOf(" "))
                            _temp2 = _temp.Substring(0, _temp.IndexOf(">"));
                        else
                            _temp2 = _temp.Substring(0, _temp.IndexOf(" "));

                        if (_temp2 == "btRemover")
                        {
                            HtmlElement _popudDiv = webBrowser1.Document.GetElementById("dPopupBox");

                            if (_popudDiv != null)
                            {
                                _popudDiv.AttachEventHandler("onpropertychange", new EventHandler(handler));
                            }
                            _HtmlElementitem.InvokeMember("click");
                            //HtmlElementCollection _elements = webBrowser1.Document.GetElementsByTagName("btBSBDeleteDialogGo");

                        }
                    }
                }
            }
            //is product
            if (webBrowser1.Url.AbsolutePath.IndexOf("/Frontoffice/ContentPages/CatalogProduct.aspx") > -1)
            {
                //return;
                //add cureent product to list
                //qty48949 inputnumber
                //M_M_lPH_lCPH_cPrdDet_lAddBask ahref
                System.Diagnostics.Debug.WriteLine("Processing Product : " + CurrentProduct.Name);
                HtmlElementCollection _numericInputC = webBrowser1.Document.GetElementById("dUpDow").GetElementsByTagName("input");
                HtmlElement _numericInput = _numericInputC[0];
                HtmlElement _addToBasketLink = webBrowser1.Document.GetElementById("M_M_lPH_lCPH_cPrdDet_lAddBask");
                if (_numericInput == null || _addToBasketLink == null)
                {
                    timer_NavigateToNextProduct.Enabled = true;
                    timer_NavigateToNextProduct.Start();
                    //NavigateToNextProduct();
                    return;
                }
                if (productQuantityOutProcessed == false)
                {

                    if ((CurrentProduct.QuantityWeightOut > 0 && CurrentProduct.QuantityOut == 0) || (CurrentProduct.QuantityWeightOut > 0 && lockProductProcessing == true))
                    {
                        HtmlElementCollection _tempHtmlC = webBrowser1.Document.GetElementById("dUpDow").GetElementsByTagName("a");
                        HtmlElement _popudDiv = webBrowser1.Document.GetElementById("dPopupBox");
                        if (_popudDiv != null)
                        {
                            _popudDiv.AttachEventHandler("onpropertychange", new EventHandler(handler));
                        }
                        foreach (HtmlElement _htmlElement in _tempHtmlC)
                        {
                            if (_htmlElement.InnerText != null && _htmlElement.InnerText.ToLower() == "comprar ao peso")
                            {
                                isToAddProductQuantity = true;
                                //productQuantityOutProcessed = true;
                                _htmlElement.InvokeMember("click");
                                return;
                            }
                        }
                    }

                }

                //System.Threading.Thread.Sleep(2000);
                if (!lockProductProcessing && !isToAddProductQuantity)
                {
                    lockProductProcessing = true;
                    _numericInput.SetAttribute("value", CurrentProduct.QuantityOut.ToString());
                    _HtmlElementQuantityOutLink = _addToBasketLink;
                    timer_AddQauntityOut.Enabled = true;
                    timer_AddQauntityOut.Start();
                    return;
                    //_addToBasketLink.InvokeMember("click");
                }

                timer_NavigateToNextProduct.Enabled = true;
                timer_NavigateToNextProduct.Start();
                //System.Threading.Thread.Sleep(2000);
                //NavigateToNextProduct();

            }

        }
        bool lockProductProcessing, isToAddProductQuantity, productQuantityOutProcessed = false;
        private void handler(Object sender, EventArgs e)
        {
            if (isToAddProductQuantity)
            {
                HtmlElement _popudDiv = webBrowser1.Document.GetElementById("dPopupBox");
                HtmlElementCollection _input = _popudDiv.GetElementsByTagName("input");
                HtmlElementCollection _a = _popudDiv.GetElementsByTagName("a");
                HtmlElement _ahref = (_a.Count > 1) ? _a[1] : null;
                HtmlElement _titAdicionar = webBrowser1.Document.GetElementById("titAdicionar");
                HtmlElement _txtPeso = webBrowser1.Document.GetElementById("txtPeso");

                if (_txtPeso != null)
                {
                    float _float, _float2 = 0;

                    _float = float.Parse(CurrentProduct.QuantityWeightOut.ToString());
                    _float2 = _float * 1000;


                    _txtPeso.SetAttribute("value", _float2.ToString());
                    if (_ahref != null)
                    {
                        isToAddProductQuantity = false;
                        productQuantityOutProcessed = true;
                        _HtmlElementQuantityWeightOutLink = _ahref;
                        _popudDiv.DetachEventHandler("onpropertychange", new EventHandler(handler));
                        timer_QuantityWeightAddClick.Enabled = true;
                        timer_QuantityWeightAddClick.Start();
                        //_ahref.InvokeMember("click");
                    }
                }

            }
            else
            {
                HtmlElement _popudDiv = webBrowser1.Document.GetElementById("dPopupBox");
                HtmlElement _okButton = webBrowser1.Document.GetElementById("btBSBDeleteDialogGo");
                if (_okButton != null)
                {
                    _popudDiv.DetachEventHandler("onpropertychange", new EventHandler(handler));
                    _okButton.InvokeMember("click");
                }
            }
            //btBSBDeleteDialogGo
        }
        void NavigateToNextProduct()
        {
            if (currentProccessingItemIndex == listView1.Items.Count - 1) return;
            for (int i = currentProccessingItemIndex; i <= listView1.Items.Count - 1; )
            {
                if (listView1.Items[currentProccessingItemIndex] != null && listView1.Items[currentProccessingItemIndex].Checked == true)
                {
                    SpiroStockManagmentDatabaseClass.Objects.Product _currentProduct = (listView1.Items[currentProccessingItemIndex].Tag as SpiroStockManagmentDatabaseClass.Objects.Product);
                    currentProccessingItemIndex++;
                    System.Diagnostics.Debug.WriteLine("navigate - " + _currentProduct.Name);
                    if (_currentProduct != null)
                    {
                        if (_currentProduct.MarketItemUrl != string.Empty && (_currentProduct.QuantityOut > 0 || _currentProduct.QuantityWeightOut > 0))
                        {
                            if (_currentProduct.MarketItemUrl.IndexOf("CatalogSearch.aspx") > -1)
                            {
                                timer_NavigateToNextProduct.Enabled = true;
                                timer_NavigateToNextProduct.Start();
                                //NavigateToNextProduct();
                                return;
                            }
                            lockProductProcessing = false;
                            CurrentProduct = _currentProduct;
                            isProccessingProducts = true;
                            //timer1.Enabled = true;
                            //timer1.Start();
                            webBrowser1.Navigate(_currentProduct.MarketItemUrl);
                            break;
                        }
                        break;
                    }
                }
                currentProccessingItemIndex++;
            }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            webBrowser1.Navigate("http://www.jumbo.pt/");
            //webBrowser1.Navigate("https://www.jumbo.pt/Frontoffice/ContentPages/BuyShoppingBasket.aspx");
        }

        bool SelectAllCheckboxSatate = true;
        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {

                CheckBox cck = new CheckBox
                {
                    Text = "",
                    Visible = true,
                    Checked = SelectAllCheckboxSatate
                };



                cck.CheckedChanged += new EventHandler(cck_CheckedChanged);
                listView1.SuspendLayout();
                e.DrawBackground();
                cck.BackColor = e.BackColor;
                cck.UseVisualStyleBackColor = true;
                cck.SetBounds(e.Bounds.X, e.Bounds.Y, cck.GetPreferredSize(new Size(e.Bounds.Width, e.Bounds.Height)).Width, cck.GetPreferredSize(new Size(e.Bounds.Width, e.Bounds.Height)).Width);
                cck.Size = new Size(cck.GetPreferredSize(new Size(e.Bounds.Width - 1, e.Bounds.Height)).Width + 1, e.Bounds.Height);
                cck.Location = new Point(e.Bounds.Location.X + 1, 0);
                //Padding myPadding = new Padding();
                //myPadding.All = 3;

                //cck.Margin = myPadding;
                listView1.Controls.Add(cck);
                cck.Show();
                cck.BringToFront();
            }

            if (e.ColumnIndex != 0) e.DrawBackground();
            e.DrawText(TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding);
            listView1.ResumeLayout(true);
        }

        void cck_CheckedChanged(object sender, EventArgs e)
        {
            SelectAllCheckboxSatate = (sender as CheckBox).Checked;
            foreach (ListViewItem _ListViewItem in listView1.Items)
            {
                _ListViewItem.Checked = SelectAllCheckboxSatate;
            }

        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NavigateToNextProduct();
            //webBrowser1.Navigate(CurrentProduct.MarketItemUrl);
            timer_NavigateToNextProduct.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NavigateToNextProduct();
        }

        HtmlElement _HtmlElementQuantityWeightOutLink, _HtmlElementQuantityOutLink = null;
        private void timer_QuantityWeightAddClick_Tick(object sender, EventArgs e)
        {
            _HtmlElementQuantityWeightOutLink.InvokeMember("click");
            timer_QuantityWeightAddClick.Stop();
        }

        private void timer_AddQauntityOut_Tick(object sender, EventArgs e)
        {
            _HtmlElementQuantityOutLink.InvokeMember("click");
            timer_AddQauntityOut.Stop();
        }

    }
}
