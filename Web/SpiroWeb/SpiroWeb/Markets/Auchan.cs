using CsQuery;
//using System.Web.Script.Serialization;
using LisieStores.Extensibility;
using SpiroWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SpiroWeb.Markets
{
    //VERY IMPORTANT - get product just by online id
    // - /on/demandware.store/Sites-AuchanPT-Site/pt_PT/Product-IncludeProductShow?pid =678660
    [MarketAttr(1, "Auchan", "https://www.auchan.pt", "#fcdf90", "https://www.auchan.pt/pt/pesquisa?q=")]
    public class Auchan : Market, IMarketFetcher
    {
        string IMarketFetcher.GetProductViewableUrl(string onlineProductId, string url)
        {
            if (!string.IsNullOrEmpty(onlineProductId))
            {
                return this.StoreUrl + "/produtos/" + onlineProductId + ".html";
            }
            if (!string.IsNullOrEmpty(url))
            {
                return this.StoreUrl + url;
            }
            return string.Empty;
        }

        async Task<LisieStores.Extensibility.ProductSearchResult> IMarketFetcher.GetProductMetadata(string url)
        {
            return await GetMetadata(url);
        }

        async public Task<LisieStores.Extensibility.ProductSearchResult> GetProductMetadataById(string onlineProductId)
        {
            string _url = "/on/demandware.store/Sites-AuchanPT-Site/pt_PT/Product-IncludeProductShow?pid=" + onlineProductId;
            //string _url = "/produtos/" + onlineProductId + ".html";
            return await GetMetadata(_url);
        }

        async Task<List<LisieStores.Extensibility.ProductSearchResult>> IMarketFetcher.GetSearchResults(string searchQuery)
        {
            try
            {
                //clean up search query
                searchQuery = searchQuery.Replace(".", "");
                string _SearchResultsHtml = await FetchUrl("https://www.auchan.pt/Frontoffice/search/" + searchQuery);
                List<LisieStores.Extensibility.ProductSearchResult> _productSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();

                CQ _jumboDom = _SearchResultsHtml;
                CQ _produtos = _jumboDom[".product"];

                List<IDomObject> _productsList = _produtos.ToList();

                int _productCOunter = 1;
                foreach (IDomObject _productResult in _productsList)
                {
                    System.Diagnostics.Debug.WriteLine(_productCOunter);
                    ++_productCOunter;

                    CQ _productResultCQ = _productResult.InnerHTML;


                    string _productUrl = _productResultCQ["a"].First().Attr("href");
                    CQ _productName = _productResultCQ["a"];
                    CQ _productInfo = ((CsQuery.Implementation.DomElement)(_productName[1])).InnerHTML;

                    //string _name = ((CsQuery.Implementation.DomElement)(_productInfo[0])).InnerHTML;
                    string _name = _productInfo.Text();
                    _name = _name.Replace("\n                            ", "");
                    _name = _name.Replace(" \n\n                    ", "");
                    _name = _name.Trim(' ');

                    string _productBrand = _productResultCQ[".product-item-brand"].First().Text().Replace("\n", "").Trim();
                    //string _productWeight = _name;
                    string _productWeight = string.Empty;
                    //CQ _productPriceCQ = _productResultCQ[".product-item-price "].First();
                    //string _productPrice = _productPriceCQ[0].FirstChild.NodeValue.Replace("\n", "").Trim();
                    string _productPrice = _productResultCQ[".sales"].Text().Replace("\n", "").Replace("€", "").Replace(" ", "").Trim();
                    string _productPriceWithoutDiscount = _productResultCQ[".strike-through.list > span"].FirstOrDefault() != null ? _productResultCQ[".strike-through.list > span"].Attr("content") + "€" : string.Empty;

                    string _productPriceWeight = _productResultCQ[".auc-measures--price-per-unit"].First().Text().Replace("\n", "").Trim();

                    string _productImageUrl = _productResultCQ[".image-container"].First()["img"].Attr("data-src");
                    if (_productImageUrl == null)
                    {
                        _productImageUrl = _productResultCQ[".tile-image"].First().Attr("data-src");
                    }

                    //get weight
                    string _weight = string.Empty;
                    string[] _words = _name.Split(' ');
                    string _lastWord = _words[_words.Length - 1];
                    string _beforeLastWord = _words[_words.Length - 2];
                    if (char.IsNumber(_lastWord[0]))
                    {
                        _weight = _lastWord;
                    }
                    else
                    {
                        //was giving error
                        //if (char.IsNumber(_beforeLastWord[0]))
                        //{
                        //    _weight = _beforeLastWord + " " + _lastWord;
                        //}
                        if (_beforeLastWord.Length > 0)
                        {
                            if (char.IsNumber(_beforeLastWord[0]))
                            {
                                _weight = _beforeLastWord + " " + _lastWord;
                            }
                        }
                        else
                        {
                            _weight = _lastWord;
                        }
                    }

                    if (_weight != string.Empty)
                    {
                        _name = _name.Replace(_weight, "");
                        _name = _name.Trim(' ');
                    }


                    //Get categories
                    //string _productCategories = _productResultCQ[".product-item-brand"].First().Text().Replace("\n", "").Trim();
                    //CQ _produtosCategories = _productResultCQ["#conteudo > ol > li"];

                    //List<IDomObject> _produtosCategoriesList = _produtosCategories.ToList();

                    //int _productCategoriesCounter = 1;
                    //foreach (IDomObject _produtosCategory in _produtosCategoriesList)
                    //{

                    //    _productCategoriesCounter++;
                    //}

                    //product url arrangement
                    string _finalUrl = _productUrl.Remove(_productUrl.LastIndexOf("/"));

                    if (!_productUrl.Equals("javascript:void(0);"))
                    {
                        LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = new LisieStores.Extensibility.ProductSearchResult()
                        {
                            //TODO - test / check if it works
                            Name = _name,
                            Brand = _productBrand,
                            Price = _productPrice.Replace("€", "").Trim(' ') + "€",
                            PriceWithoutDiscount = _productPriceWithoutDiscount,
                            PriceWeight = _productPriceWeight,
                            StoreName = this.StoreName,
                            StoreId = this.StoreId,
                            StoreColor = this.StoreColor,
                            //Url = _productUrl,
                            //Url = (_finalUrl.IndexOf("?sid=") == -1) ? _finalUrl : _finalUrl.Substring(0, _finalUrl.IndexOf("?sid=")),
                            //ViewableUrl = (_finalUrl.IndexOf("/Auchan_") == -1) ? _finalUrl + "/Auchan_Amadora" : _finalUrl,
                            Url = _productUrl,
                            ViewableUrl = _productUrl,
                            Weight = _weight,
                            ImageUrl = _productImageUrl,
                            Category = "",
                            FullCategory = "",
                            Unit = ""
                        };
                        _ProductSearchResult.Url = (_ProductSearchResult.Url.IndexOf("/Auchan_") == -1) ? _ProductSearchResult.Url : _ProductSearchResult.Url.Substring(0, _ProductSearchResult.Url.LastIndexOf("/"));
                        _productSearchResultList.Add(_ProductSearchResult);
                    }
                }
                return _productSearchResultList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        async Task<bool> IMarketFetcher.AddProductsToOnlineStoreCart(List<ProductAddToOnlineStore> products, string userId, string storeUsername, string storePassword)
        {
            //foreach (var _product in products)
            //{
            //    //if (_product.Url.IndexOf("Auchan_Amadora?") == -1)
            //    //    _product.Url += "/Auchan_Amadora?sid=14a1f7c0-f5bd-4b08-8bf0-f95f908d41dc_1";

            //    //FIX (OBSOLETE)
            //    if (_product.Url.IndexOf("Auchan_Amadora") == -1)
            //        _product.Url += "/Auchan_Amadora";
            //}
            var cli = new WebClient();
            cli.Headers[HttpRequestHeader.ContentType] = "application/json";

            var _nodeRequest = new NodeRequest
            {
                username = storeUsername,
                password = storePassword,
                products = products
            };
            var json = new JavaScriptSerializer().Serialize(_nodeRequest);
            //string response = cli.UploadString("https://puppeteer-lisie.herokuapp.com/addUserListToJumbo/" + userId, json);
            string response = cli.UploadString("https://lisie.herokuapp.com/addUserListToJumbo/" + userId, json);
            //string response = cli.UploadString("http://localhost:3000/addUserListToJumbo/" + userId, json);

            ////WarnMeOfAddingToJumboOnline(userId, storeUsername);
            //return Json(_nodeRequest, JsonRequestBehavior.AllowGet);
            return true;
        }

        async public Task<LisieStores.Extensibility.ProductSearchResult> GetProductMetadataByBarcode(string barcode)
        {
            try
            {
                //clean up search query
                string _SearchResultsHtml = await FetchUrl("https://www.auchan.pt/Frontoffice/search/" + barcode);
                LisieStores.Extensibility.ProductSearchResult _productSearchResult = null;

                CQ _jumboDom = _SearchResultsHtml;
                CQ _produtos = _jumboDom[".product"];

                List<IDomObject> _productsList = _produtos.ToList();
                string _productUrl = string.Empty;
                int _productCOunter = 1;
                foreach (IDomObject _productResult in _productsList)
                {
                    System.Diagnostics.Debug.WriteLine(_productCOunter);
                    ++_productCOunter;

                    CQ _productResultCQ = _productResult.InnerHTML;


                    _productUrl = _productResultCQ["a"].First().Attr("href");
                    break;
                }
                if (_productUrl != string.Empty)
                {
                    _productSearchResult = await GetMetadata(_productUrl);
                }
                return _productSearchResult;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        async Task<LisieStores.Extensibility.ProductSearchResult> GetMetadata(string url)
        {
            LisieStores.Extensibility.ProductSearchResult _productSearchResult = new LisieStores.Extensibility.ProductSearchResult();

            try
            {
                var newUrl = url;
                if (newUrl.IndexOf("https://www.jumbo.pt") == 0)
                {
                    newUrl = newUrl.Remove(0, "https://www.jumbo.pt".Length);
                }
                //string _finalUrl = "https://www.auchan.pt" + ((url.IndexOf("/Auchan_") == -1) ? url + "/Auchan_Amadora" : url);


                //put in new way
                if (newUrl.IndexOf("/Frontoffice") > -1)
                {
                    newUrl = url.ToLower();
                    newUrl = newUrl.Replace("/frontoffice", "/pt");
                    newUrl = newUrl.Replace("_", "-");
                    if (newUrl.IndexOf("auchan-amadora") > -1)
                    {
                        newUrl = newUrl.Substring(0, newUrl.LastIndexOf("/"));
                    }
                    newUrl += ".html";
                }

                string _finalUrl = "https://www.auchan.pt" + newUrl;
                string _htmlResult = await FetchUrl(_finalUrl);

                //if retun empty return null
                if (string.IsNullOrEmpty(_htmlResult)) return null;

                //Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");
                //Logger.Debug(_finalUrl, "auchan.txt");
                //Logger.Debug(_htmlResult, "auchan.txt");
                //Get JSON
                int _indexBeg = _htmlResult.IndexOf("<script type=\"application/ld+json\">") + "<script type=\"application/ld+json\">".Length;
                int _indexEnd = _htmlResult.IndexOf("</script>", _indexBeg);
                string _productJson = _htmlResult.Substring(_indexBeg, _indexEnd - _indexBeg);

                JavaScriptSerializer js = new JavaScriptSerializer();
                Models.AuchanContinenteProductOnlineMetadata.Product _auchanProduct = (Models.AuchanContinenteProductOnlineMetadata.Product)js.Deserialize(_productJson, typeof(Models.AuchanContinenteProductOnlineMetadata.Product));

                CQ _jumboDom = _htmlResult;
                CQ _category = _jumboDom[".breadcrumb li:last-child"].Prev().Prev();
                CQ _divRow = _jumboDom[".row"];
                CQ _nameAndWeight = _divRow["h1"];
                CQ _brand = _divRow[".rating"].Next();
                CQ _priceWeight = _divRow[".auc-measures--price-per-unit"].First();
                CQ _image = _jumboDom[".carousel > div > div > picture > img"];
                CQ _price = _divRow[".item-price"];
                CQ _barcode = _jumboDom[".product-ean"];
                //CQ _productDetail = _jumboDom[".product-detail"]; //In FUTURE USE - has a lot of important urls
                //CQ _barcodeAttr = _productDetail.Attr("data-ean"); //GO FETCH THE NEW WAY
                string _finalBarcode = _barcode.Text();
                //Testing see if is a barcode by weight - ends in 000000
                //CQ _isWeightBarcode = _jumboDom["#maincontent > div.container.product-detail.product-wrapper.auc-pdp__body > div.row.no-gutters.auc-pdp__upper-section.auc-pdp__upper-section--grocery > div.col-12.col-md-7.col-xl-6.auc-pdp__right-section > div > div.auc-pdp__middle-section.row.no-gutters > div.prices-add-to-cart-actions.col-12.col-xl-5 > div > div > div > div > div > div.auc-qty-selector--unit-selectors__selectors > button.auc-button__circle.auc-qty-selector--unit-selectors__selectors--selected"];
                CQ _isWeightBarcode = _jumboDom[".auc-qty-selector--unit-selectors__selectors--selected"];

                CQ _avgWeight = _jumboDom[".auc-avgWeight"];
                bool _isWeightBarcodeOfAvgHeight = _avgWeight.Length > 0;

                //chanche barcode in case is a weight barcode
                if (_isWeightBarcode.Length > 0)
                {
                    string _begBarcode = _finalBarcode.Substring(0, _finalBarcode.Length - 6);
                    _finalBarcode = _begBarcode + "000000";
                }
                else
                {
                    if (_isWeightBarcodeOfAvgHeight)
                    {
                        string _begBarcode = _finalBarcode.Substring(0, _finalBarcode.Length - 6);
                        _finalBarcode = _begBarcode + "000000";
                    }
                }



                CQ _metas = _jumboDom["meta"];
                List<IDomObject> _metasList = _metas.ToList();
                string _redirectedUrl = string.Empty;
                foreach (IDomObject _meta in _metasList)
                {
                    if (_meta.Attributes["property"] == "og:url")
                    {
                        _redirectedUrl = _meta.Attributes["content"];
                        int _removeDomainStartIndex = _redirectedUrl.IndexOf("auchan.pt");
                        if (_removeDomainStartIndex > -1)
                        {
                            _removeDomainStartIndex += +"auchan.pt".Length;
                            _redirectedUrl = _redirectedUrl.Substring(_removeDomainStartIndex);
                            if (!string.IsNullOrEmpty(_redirectedUrl))
                                newUrl = _redirectedUrl;
                        }
                    }
                }

                _productSearchResult.Name = _nameAndWeight.Text();

                //get weight
                string _weight = string.Empty;
                string[] _words = _productSearchResult.Name.TrimEnd(' ').Split(' ');
                string _lastWord = _words[_words.Length - 1];
                string _beforeLastWord = _words[_words.Length - 2];
                if (char.IsNumber(_lastWord[0]))
                {
                    _weight = _lastWord;
                }
                else
                {
                    //was giving error
                    //if (char.IsNumber(_beforeLastWord[0]))
                    //{
                    //    _weight = _beforeLastWord + " " + _lastWord;
                    //}
                    if (_beforeLastWord.Length > 0)
                    {
                        if (char.IsNumber(_beforeLastWord[0]))
                        {
                            _weight = _beforeLastWord + " " + _lastWord;
                        }
                    }
                    else
                    {
                        _weight = _lastWord;
                    }
                }

                //get price weight and unit
                string _priceWeightText = _priceWeight.Text();
                string[] _priceWeightTextWords = _priceWeightText.ToLower().Trim().Split('/');
                string _productUnit = _priceWeightTextWords[_priceWeightTextWords.Length - 1];
                if (_priceWeightTextWords.Length > 1)
                    _priceWeightText = _priceWeightTextWords[_priceWeightTextWords.Length - 2].Replace("€", "");


                //Get categories
                CQ _produtosCategories = _jumboDom[".breadcrumb-item"];

                List<IDomObject> _produtosCategoriesList = _produtosCategories.ToList();

                int _productCategoriesCounter = 1;
                string _productCategory = "";
                string _productCategoriesFull = "";
                foreach (IDomObject _produtosCategory in _produtosCategoriesList)
                {
                    CQ _produtosCategoryCQ = _produtosCategory.InnerHTML;
                    if (_productCategoriesCounter == 1) //Do nothing, this category is irrelevant
                    {
                        _productCategory = _produtosCategoryCQ.Text();
                        _productCategoriesFull = _productCategory.Replace("\n", "") + " > ";

                    }
                    else if (_productCategoriesCounter == 2)
                    {
                        _productCategory = _produtosCategoryCQ.Text().Replace("\n", "");
                        _productCategoriesFull += _productCategory + " > ";
                    }
                    else
                    {
                        _productCategoriesFull += _produtosCategoryCQ.Text().Replace("\n", "") + " > ";
                    }

                    _productCategoriesCounter++;
                }

                //_productSearchResult.Weight = _nameAndWeight.Text();
                //string[] _nameAndWeightSplitted = _nameAndWeight.Text().Trim().Split(' ');

                //remove weight and brand from name
                if (!string.IsNullOrEmpty(_weight)) _productSearchResult.Name = _productSearchResult.Name.Replace(_weight, "");
                if (_auchanProduct.brand != null && !string.IsNullOrEmpty(_auchanProduct.brand.name))
                {
                    _productSearchResult.Name = _productSearchResult.Name.ToLower().Replace(_auchanProduct.brand.name.ToLower(), "");
                }

                _productSearchResult.Weight = _weight.Trim();
                _productSearchResult.Name = _productSearchResult.Name.Trim();
                _productSearchResult.Name = Regex.Replace(_productSearchResult.Name, " {2,}", " ");
                _productSearchResult.Brand = _auchanProduct.brand != null ? _auchanProduct.brand.name : "Auchan";
                _productSearchResult.Category = _productCategory.Trim();
                _productSearchResult.FullCategory = _productCategoriesFull;
                _productSearchResult.PriceWeight = _priceWeightText.Trim();
                _productSearchResult.ImageUrl = _image.Attr("src");
                _productSearchResult.StoreName = this.StoreName;
                _productSearchResult.StoreId = this.StoreId;
                _productSearchResult.StoreColor = this.StoreColor;
                _productSearchResult.Price = _auchanProduct.offers?.price;
                _productSearchResult.Url = newUrl;
                _productSearchResult.ViewableUrl = newUrl;
                _productSearchResult.Unit = _productUnit.Trim();
                _productSearchResult.OnlineProductId = _auchanProduct.sku;
                _productSearchResult.StoreProductId = _auchanProduct.sku;
                _productSearchResult.Barcode = _finalBarcode;
            }
            catch (Exception ex)
            {
                return null;
            }

            return _productSearchResult;
        }

       

        public Task<ProductSearchResult> FindProductAI(string name, string brand, string weight, string barcode = "")
        {
            throw new NotImplementedException();
        }
    }
}