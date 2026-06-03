using CsQuery;
using LisieStores.Extensibility;
using SpiroWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SpiroWeb.Markets
{
    [MarketAttr(7, "El Corte Ingles", "https://www.elcorteingles.pt", "#000000", "https://www.elcorteingles.pt/supermercado/pesquisar/?term=")]
    public class ElCorteIngles : Market, IMarketFetcher
    {
        string IMarketFetcher.GetProductViewableUrl(string onlineProductId, string url)
        {
            if (!string.IsNullOrEmpty(onlineProductId))
            {
                return this.StoreUrl + "/" + onlineProductId;
            }
            if (!string.IsNullOrEmpty(url))
            {
                return this.StoreUrl + url;
            }
            return string.Empty;
        }

        async public Task<bool> AddProductsToOnlineStoreCart(List<ProductAddToOnlineStore> products, string userId, string storeUsername, string storePassword)
        {
            //foreach (var _product in products)
            //{
            //    if (_product.Url.IndexOf("Auchan_Amadora?") == -1)
            //        _product.Url += "/Auchan_Amadora?sid=14a1f7c0-f5bd-4b08-8bf0-f95f908d41dc_1";
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
            try
            {
                string response = cli.UploadString("https://puppeteer-lisie.herokuapp.com/addProductsToIntermarche/" + userId, json);
            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }

        public async Task<LisieStores.Extensibility.ProductSearchResult> GetProductMetadataByBarcode(string barcode)
        {
            var _SearchResults = await GetSearchResults(barcode);
            if (_SearchResults != null && _SearchResults.Count > 0)
            {
                return await GetMetadata(_SearchResults[0].Url);
            }
            return null;
        }

        async public Task<LisieStores.Extensibility.ProductSearchResult> GetProductMetadataById(string onlineProductId)
        {
            return null;
        }

        async Task<LisieStores.Extensibility.ProductSearchResult> IMarketFetcher.GetProductMetadata(string url)
        {
            return await GetMetadata(url);
        }

        async Task<List<LisieStores.Extensibility.ProductSearchResult>> IMarketFetcher.GetSearchResults(string searchQuery)
        {
            return await GetSearchResults(searchQuery);
        }

        async Task<List<LisieStores.Extensibility.ProductSearchResult>> GetSearchResults(string searchQuery)
        {
            string _SearchResultsHtml = await FetchUrlMoreBypasses("https://www.elcorteingles.pt/supermercado/pesquisar/?term=" + searchQuery);

            List<LisieStores.Extensibility.ProductSearchResult> _productSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();

            CQ _Dom = _SearchResultsHtml;
            CQ _produtos = _Dom[".grid-item"];

            List<IDomObject> _productsList = _produtos.ToList();

            foreach (IDomObject _productResult in _productsList)
            {
                Console.WriteLine(_productResult.InnerHTML);

                CQ _productResultCQ = _productResult.OuterHTML;

                var _json = _productResultCQ.Attr("data-json");
                var _ProductItemJson = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(_json);

                var _name = _ProductItemJson["name"].ToString();
                var _brand = _ProductItemJson["brand"].ToString();
                var _priceEntry = _ProductItemJson["price"] as Dictionary<string, object>;
                var _price = _priceEntry["final"].ToString();
                string _priceWeight = _productResultCQ[".prices-price._pum"].Text();
                _priceWeight = _priceWeight.Replace("€", "").Replace("(", "").Replace(")", "").Trim();
                var _productPriceWeightSpl = _priceWeight.Split('/');
                _priceWeight = _productPriceWeightSpl[0].Trim();
                var _unit = _productPriceWeightSpl[1].Trim();
                //var _Test = _ProductItem[4];
                var _onlineProductId = _ProductItemJson["id"].ToString().Replace("___", "");
                string _image = "https:" + _productResultCQ["img"].First().Attr("src");
                string _url = _productResultCQ["a"].First().Attr("href");

                _productSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
                {
                    //Barcode = _productBarcode,
                    Name = _name,
                    Brand = _brand,
                    Price = _price,
                    PriceWithoutDiscount = "",
                    PriceWeight = _priceWeight,
                    StoreId = this.StoreId,
                    StoreName = this.StoreName,
                    StoreColor = this.StoreColor,
                    Url = _url,
                    ViewableUrl = "https://www.elcorteingles.pt" + _url,
                    Weight = "",
                    ImageUrl = _image,
                    PriceLiteral = 0,
                    PriceWeightLiteral = 0,
                    Category = "",
                    FullCategory = "",
                    Unit = _unit,
                    OnlineProductId = _onlineProductId
                });
            }
            return _productSearchResultList;
        }

        async Task<LisieStores.Extensibility.ProductSearchResult> GetMetadata(string url)
        {
            try
            {
                var _product = new LisieStores.Extensibility.ProductSearchResult();
                _product.StoreColor = this.StoreColor;
                _product.StoreId = this.StoreId;
                _product.StoreName = this.StoreName;

                string _html = await FetchUrlMoreBypasses("https://www.elcorteingles.pt" + url);

                CQ _Dom = _html;
                CQ _produto = _Dom[".food-pdp-page-content-grid"];
                string _htmlSrc = _produto.Html();

                //var _onlineProductId = _produto.Attr("data-product-id").Replace("___", "");
                var _onlineProductId = "None";
                string _image = _produto["._hasSecondaryImages"].First().Attr("src");
                var _barcode = _produto[".additional-info-modal__content ul li"].First().Text().Replace("EAN:  ", "");
                var _name = _produto[".food-pdp-page-content-grid-detail-description h1"].First().Text().Trim();
                var _brand = _produto[".food-pdp-page-content-grid-detail-brand"].First().Text();
                
                var _weightText = _produto[".food-pdp-page-content-grid-detail-presentation"].First().Text();
                var _weightTextArray = _weightText.Split('|').ToList();
                _weightTextArray.RemoveAt(0);
                var _weight = String.Join(" ", _weightTextArray.ToArray()).Trim();
                //prices      
                var _productPriceWithDiscount = _produto[".food-prices__price--original"].First().Text().Replace("€", "").Trim();
                
                var _productPrice = _produto[".food-prices__offer"].First().Text().Replace("€", "");
                if (string.IsNullOrEmpty(_productPriceWithDiscount))
                {
                    _productPrice = _produto[".food-prices__price"].First().Text();
                }
                var _productPriceWeight = _produto[".food-prices__measurement-unit"].First().Text().Replace("(", "").Replace(")", "").Trim();
                var _PriceWeightLiteral = _productPriceWeight;
                _productPrice = _productPrice.Replace("€", "").Trim();
                _productPriceWeight = _productPriceWeight.Replace("€", "").Replace("(", "").Replace(")", "").Replace(" ", "").Trim();
                var _productPriceWeightSpl = _productPriceWeight.Split('/');
                _productPriceWeight = _productPriceWeightSpl.Length > 0 ? _productPriceWeightSpl[0] : string.Empty;
                _productPriceWeight = _productPriceWeight.Trim();
                var _unit = _productPriceWeightSpl.Length > 1 ? _productPriceWeightSpl[1].Trim() : string.Empty;
                //if (!string.IsNullOrEmpty(_productPriceWithDiscount))
                //{
                //    _productPrice = _productPriceWithDiscount.Trim();
                //}

               
                //var _sec = __name["p"];
                //var __sec = _sec["span"].First().Text();
                //var _nameText = _sec.First().Text();

                //var _name = _produto["[itemprop=name]"].First().Text();
                //var _brand = _produto["[itemprop=brand]"].First().Text();
                //var _weightHtml = _produto["[itemprop=description]"].First().Html();
                //var _weightStartIndex = _weightHtml.LastIndexOf("</span>") + ("</span>").Length;
                //var _weight = _weightHtml.Substring(_weightStartIndex, (_weightHtml.Length - 1) - _weightStartIndex);
                //_weight = _weight.Replace("\n", " ").Trim();

                //category
                var _categoryHtml = _Dom[".breadcrumbs_pdp li"];
                var _category = "";
                var _categoryFull = "";
                var _count = 0;
                var _elementsCount = _categoryHtml.Count();
                foreach (var _element in _categoryHtml.Elements)
                {
                    CQ _elementCQ = _element.InnerHTML;
                    var __text = _elementCQ.Text().Replace("/", "".Replace(" ", "")).Trim();
                    if (_count == 0)
                    {
                        _count++;
                        continue;
                    }
                    if (_count == _elementsCount - 2)
                    {
                        _category = __text;
                    }
                    if (_count <= _elementsCount - 2)
                    {
                        _categoryFull += __text + (_count != _elementsCount - 2 ? " > " : "");
                    }
                    _count++;
                }

                if (string.IsNullOrEmpty(_name) && string.IsNullOrEmpty(_barcode))
                {
                    return null;
                }

                _product = new LisieStores.Extensibility.ProductSearchResult
                {
                    Barcode = _barcode,
                    Name = _name,
                    Brand = _brand,
                    Price = _productPrice,
                    PriceWithoutDiscount = _productPriceWithDiscount,
                    PriceWeight = _productPriceWeight,
                    PriceWeightLiteral = 0,
                    StoreId = this.StoreId,
                    StoreName = this.StoreName,
                    StoreColor = this.StoreColor,
                    Url = url,
                    ViewableUrl = "https://www.elcorteingles.pt" + url,
                    Weight = _weight,
                    ImageUrl = _image,
                    PriceLiteral = 0,
                    Category = _category,
                    FullCategory = _categoryFull,
                    Unit = _unit,
                    OnlineProductId = _onlineProductId
                };

                return _product;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}