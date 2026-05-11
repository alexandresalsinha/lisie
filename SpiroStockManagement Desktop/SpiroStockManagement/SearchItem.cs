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
    public partial class SearchItem : Form
    {
        public SearchItem()
        {
            InitializeComponent();
        }

        public void Initialize(string searchText)
        {
            SearchItemText = searchText;
            //webBrowser1.Navigate("http://www.continente.pt/Search.aspx?search=" + searchText);
            webBrowser_Jumbo.Navigate("http://www.jumbo.pt/Frontoffice/ContentPages/CatalogSearch.aspx?Q=" + SearchItemText);
        }

        public string ItemLink, ItemName, ItemPrice, ItemPictureUrl, ItemBrand, ItemVariableWeightPrice, ItemcategoryString, ItemPictureSmallUrl, ItemPackageInfo, InformationTakenFrom = "";
        public Image ItemPictureSmallImage = null;

        private void button1_Click(object sender, EventArgs e)
        {
            InformationTakenFrom = tabControl1.SelectedTab.Text;
            //get the name, price, and image of item
            switch (tabControl1.SelectedTab.Text.ToLower())
            {
                case "continente":
                    ItemLink = webBrowser1.Url.AbsoluteUri;

                    HtmlElement _ElementName = webBrowser1.Document.GetElementById("ProductDetail1_lbDisplayName");
                    ItemName = _ElementName.InnerText;

                    try
                    {
                        HtmlElement _ElementImageUrl = webBrowser1.Document.GetElementById("ProductDetail1_hplMagicZoom");
                        ItemPictureUrl = _ElementImageUrl.GetAttribute("href");
                    }
                    catch (Exception)
                    {
                    }

                    HtmlElement _ElementImageUrlSmall = webBrowser1.Document.GetElementById("ProductDetail1_Large_Image");
                    ItemPictureSmallUrl = _ElementImageUrlSmall.GetAttribute("src");



                    HtmlElement _ElementPrice = webBrowser1.Document.GetElementById("ProductDetail1_lbCy_List_Price");
                    ItemPrice = _ElementPrice.InnerText;

                    HtmlElement _ElementBrand = webBrowser1.Document.GetElementById("ProductDetail1_lbBranch");
                    ItemBrand = _ElementBrand.InnerText;

                    try
                    {
                        HtmlElement _ElementVariableWeight = webBrowser1.Document.GetElementById("ProductDetail1_lblVariableWeight");
                        ItemVariableWeightPrice = _ElementVariableWeight.InnerText;
                    }
                    catch (Exception)
                    {
                    }

                    //category
                    HtmlElementCollection _elementsCategory = webBrowser1.Document.GetElementsByTagName("a");
                    foreach (HtmlElement _HtmlElement in _elementsCategory)
                    {

                        if (_HtmlElement.GetAttribute("class") == "produtoPath")
                        {
                            ItemcategoryString += _HtmlElement.InnerText + " - ";
                        }
                    }
                    HtmlElementCollection _elementsCategorySpan = webBrowser1.Document.GetElementsByTagName("span");
                    foreach (HtmlElement _HtmlElement in _elementsCategory)
                    {

                        if (_HtmlElement.GetAttribute("class") == "produtoPathActive")
                        {
                            ItemcategoryString += _HtmlElement.InnerText;
                        }
                    }


                    //package info
                    HtmlElement _ElementPackageInfo = webBrowser1.Document.GetElementById("ProductDetail1_lbComplement");
                    ItemPackageInfo += _ElementPackageInfo.InnerText;

                    //downaload image
                    ItemPictureSmallImage = GlobalProcedures.DownloadImage(ItemPictureSmallUrl);

                    break;
                //produtoDetalheDescricaoEmb
                //produtoPathActive
                //ProductDetail1_lbBranch
                //ProductDetail1_lblVariableWeight
                //ProductDetail1_lbCy_List_Price
                //webBrowser1.DocumentText.IndexOf
                case "jumbo":
                    //choosenLink = webBrowser_Jumbo.Url.AbsoluteUri;
                    break;
                default:
                    break;
            }

            this.Close();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            button1.Enabled = true;
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            button1.Enabled = false;
        }

        string SearchItemText = "";
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    if (webBrowser1.Document == null)
                        webBrowser1.Navigate("http://www.continente.pt/Search.aspx?search=" + SearchItemText);
                    break;
                case 1:
                    //if (webBrowser_Jumbo.Document == null)
                    //    webBrowser_Jumbo.Navigate("http://www.jumbo.pt/Frontoffice/ContentPages/CatalogSearch.aspx?k=" + SearchItemText + "&IsExpress=false");
                    break;
                default:
                    break;
            }
        }

        private void webBrowser_Jumbo_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string _url = e.Url.ToString();

            //is product page load info
            if (_url.IndexOf("CatalogProduct.aspx?id") > 0)
            {
                HtmlElement div = webBrowser_Jumbo.Document.GetElementById("leftContent");

                HtmlAgilityPack.HtmlDocument _doc = new HtmlAgilityPack.HtmlDocument();
                _doc.LoadHtml(div.InnerHtml);
                HtmlAgilityPack.HtmlNodeCollection _nameElement = _doc.DocumentNode.SelectNodes("//span[@class='titProd']");
                HtmlAgilityPack.HtmlNodeCollection _brandElement = _doc.DocumentNode.SelectNodes("//div[@class='titMarca']");
                HtmlAgilityPack.HtmlNodeCollection _packageInfoElement = _doc.DocumentNode.SelectNodes("//div[@class='gr']");
                HtmlAgilityPack.HtmlNodeCollection _priceElement = _doc.DocumentNode.SelectNodes("//div[@class='preco']");
                HtmlAgilityPack.HtmlNodeCollection _VariableWeightElement = _doc.DocumentNode.SelectNodes("//div[@class='prodkg']");
                HtmlAgilityPack.HtmlNodeCollection _ImageElement = _doc.DocumentNode.SelectNodes("//div[@class='holderImg']//img");

                HtmlAgilityPack.HtmlNodeCollection _CategoryElement = _doc.DocumentNode.SelectNodes("//div[@id='M_M_lPH_lCPH_cBC_uBC']//a");
                
                //HtmlAgilityPack.HtmlNodeCollection _nameSecondElement = _doc.DocumentNode.SelectNodes("//td[@class='leadProduto']");
                //HtmlAgilityPack.HtmlNodeCollection _Pricelement = _doc.DocumentNode.SelectNodes("//span[@class='leadPreco']");
                //HtmlAgilityPack.HtmlNodeCollection _ImageElement2 = _ImageElement[0].SelectNodes("//img[@src=]");
                //HtmlAgilityPack.HtmlNodeCollection _ImageElement2 = _ImageElement[0].SelectNodes("//img[@src='/FrontOffice/CatalogImages/Products/Large/*']");
                //HtmlAgilityPack.HtmlNodeCollection _ImageElement2 = _ImageElement[0].SelectNodes("//img[@src='/FrontOffice/CatalogImages/Products/*']");

                ItemName = _nameElement[0].InnerText;
                //ItemName = ItemName.Remove(ItemName.IndexOf("<b>"), ItemName.IndexOf("</b>") + 4 - ItemName.IndexOf("<b>"));
                //ItemName = System.Text.RegularExpressions.Regex.Replace(ItemName, @"<(.|\n)*?>", string.Empty);
                //ItemName = ItemName.Replace("&nbsp;", " ");
                //_nameSecondElement[0].InnerHtml.

                //ItemName = _nameElement[0].InnerText.Substring(0, _nameElement[0].InnerText.IndexOf("&")) + " " +
                //_nameSecondElement[1].InnerHtml.Substring(_nameSecondElement[1].InnerHtml.LastIndexOf("<br><br>") + 8);
                ItemcategoryString = _CategoryElement[0].InnerText;

                ItemBrand = _brandElement[0].InnerText;
                if (_packageInfoElement.Count > 0)
                    ItemPackageInfo = _packageInfoElement[0].InnerText;

                ItemName = ItemName.ToLower().Replace(ItemBrand.ToLower(), "");
                ItemName = ItemName.ToLower().Replace(ItemPackageInfo.ToLower(), "");
                ItemName = ItemName.Replace("  ", " ");
                ItemName = ItemName.Trim();
                ItemName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ItemName);
                ItemPrice = _priceElement[0].InnerText.Replace("€", "");
                ItemVariableWeightPrice = _VariableWeightElement[0].InnerText.Replace("€", "");
                ItemPictureSmallUrl = _ImageElement[0].Attributes["src"].Value;
                ItemLink = webBrowser_Jumbo.Url.AbsoluteUri;
                ItemPictureSmallImage = GlobalProcedures.DownloadImage("http://www.jumbo.pt" + ItemPictureSmallUrl);

                button1.Enabled = true;
            }
            return;

            HtmlElement _target = webBrowser_Jumbo.Document.GetElementById("M_bPH_UPCatalogSearch");

            if (_target != null)
            {
                _target.AttachEventHandler("onpropertychange", new EventHandler(handler));
            }

            button1.Enabled = true;
        }

        bool GetJumboDataEnded = false;
        private void handler(Object sender, EventArgs e)
        {
            //12079|updatePanel|M_bPH_UPCatalogSearch|
            //webBrowser_Jumbo.Document.inn
            //HtmlElement div = webBrowser_Jumbo.Document.GetElementById("M_bPH_UPCatalogSearch");
            HtmlElement div = webBrowser_Jumbo.Document.GetElementById("leftContent");
            
            if (div == null) return;



            try
            {
                if (GetJumboDataEnded == false)
                {
                    HtmlAgilityPack.HtmlDocument _doc = new HtmlAgilityPack.HtmlDocument();
                    _doc.LoadHtml(div.InnerHtml);
                    HtmlAgilityPack.HtmlNodeCollection _nameElement = _doc.DocumentNode.SelectNodes("//span[@class='titProd']");
                    HtmlAgilityPack.HtmlNodeCollection _brandElement = _doc.DocumentNode.SelectNodes("//span[@class='titMarca']");
                    HtmlAgilityPack.HtmlNodeCollection _packageInfoElement = _doc.DocumentNode.SelectNodes("//div[@class='gr']");
                    HtmlAgilityPack.HtmlNodeCollection _priceElement = _doc.DocumentNode.SelectNodes("//div[@class='preco']");
                    HtmlAgilityPack.HtmlNodeCollection _VariableWeightElement = _doc.DocumentNode.SelectNodes("//div[@class='prodkg']");

                    HtmlAgilityPack.HtmlNodeCollection _nameSecondElement = _doc.DocumentNode.SelectNodes("//td[@class='leadProduto']");
                    HtmlAgilityPack.HtmlNodeCollection _Pricelement = _doc.DocumentNode.SelectNodes("//span[@class='leadPreco']");
                    HtmlAgilityPack.HtmlNodeCollection _ImageElement = _doc.DocumentNode.SelectNodes("//div[@class='holderImg']//img");
                    //HtmlAgilityPack.HtmlNodeCollection _ImageElement2 = _ImageElement[0].SelectNodes("//img[@src=]");
                    //HtmlAgilityPack.HtmlNodeCollection _ImageElement2 = _ImageElement[0].SelectNodes("//img[@src='/FrontOffice/CatalogImages/Products/Large/*']");
                    HtmlAgilityPack.HtmlNodeCollection _ImageElement2 = _ImageElement[0].SelectNodes("//img[@src='/FrontOffice/CatalogImages/Products/*']");


                    //div[@class='photoBox pB-ms']/a[@href]
                    //HtmlAgilityPack.HtmlNodeCollection _Pricelement = _doc.DocumentNode.SelectNodes("//span[@class='leadProduto']");
                    //leadPreco
                    //leadPaginaca

                    //take the name
                    //take the brand of the string
                    ItemName = _nameSecondElement[1].InnerHtml;
                    ItemName = ItemName.Remove(ItemName.IndexOf("<b>"), ItemName.IndexOf("</b>") + 4 - ItemName.IndexOf("<b>"));
                    ItemName = System.Text.RegularExpressions.Regex.Replace(ItemName, @"<(.|\n)*?>", string.Empty);
                    ItemName = ItemName.Replace("&nbsp;", " ");
                    //_nameSecondElement[0].InnerHtml.

                    //ItemName = _nameElement[0].InnerText.Substring(0, _nameElement[0].InnerText.IndexOf("&")) + " " +
                    //_nameSecondElement[1].InnerHtml.Substring(_nameSecondElement[1].InnerHtml.LastIndexOf("<br><br>") + 8);
                    
                    ItemBrand = _nameElement[0].InnerText.Substring(_nameElement[0].InnerText.IndexOf(";") + 1);
                    if(_nameElement.Count > 1)
                        ItemPackageInfo = _nameElement[1].InnerText;
                    
                    ItemPrice = _Pricelement[0].InnerText.Replace("€", "");
                    ItemVariableWeightPrice = _VariableWeightElement[0].InnerText.Replace("€", "€ ").Replace("/", " / ");
                    ItemPictureSmallUrl = _ImageElement[0].Attributes["src"].Value;
                }

                GetJumboDataEnded = true;
            }
            catch (Exception ex)
            {
                GetJumboDataEnded = false;
            }
        }
    }
}
