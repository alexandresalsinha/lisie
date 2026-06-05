using LisieStores.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SpiroWeb.Markets
{
    //[MarketAttr(3, "Pingo Doce", "https://mercadao.pt/store/pingo-doce", "#8BC53F")]
    [MarketAttr(3, "Pingo Doce", "https://mercadao.pt", "#8BC53F", "https://mercadao.pt/store/pingo-doce/search?queries=")]
    public class PingoDoce : Market, IMarketFetcher
    {

        string IMarketFetcher.GetProductViewableUrl(string onlineProductId, string url)
        {
            if (!string.IsNullOrEmpty(onlineProductId))
            {
                return string.Empty;
            }
            if (!string.IsNullOrEmpty(url))
            {
                return this.StoreUrl + "/store/pingo-doce/product" + url.Substring(url.LastIndexOf("/")); ;
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

            var _nodeRequest = new SpiroWeb.Controllers.NodeRequest
            {
                username = storeUsername,
                password = storePassword,
                products = products
            };
            var json = new JavaScriptSerializer().Serialize(_nodeRequest);
            try
            {
                string response = cli.UploadString("https://puppeteer-lisie.herokuapp.com/addProductsToPingoDoce/" + userId, json);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<ProductSearchResult> GetProductMetadataByBarcode(string barcode)
        {
            return null;
        }

        async Task<LisieStores.Extensibility.ProductSearchResult> IMarketFetcher.GetProductMetadata(string url)
        {
            return await GetMetadata(url);
            //string _htmlResult = await FetchUrl("https://mercadao.pt" + url);
            //if (_htmlResult == "null")
            //{
            //    _htmlResult = await FetchUrl("https://mercadao.pt" + url.Replace("5afbf7f176f9b3001a672515", "5b2d35b85ce104001af36fca"));
            //}
            //ProductSearchResult _productSearchResultData = new ProductSearchResult();
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //Models.PingoDoceProduct.Product _PingoDoceProduct = (Models.PingoDoceProduct.Product)js.Deserialize(_htmlResult, typeof(Models.PingoDoceProduct.Product));

            //if (_PingoDoceProduct == null) //if null return
            //    return null;

            ////Categories
            //int _productCategoriesCounter = 1;
            //string _productCategory = "";
            //string _productCategoriesFull = "";
            //foreach (var _produtosCategory in _PingoDoceProduct.ancestorsCategoriesArray.Reverse<string>())
            //{
            //    if (_productCategoriesCounter == 1)
            //    {
            //        _productCategory = _produtosCategory;
            //        break;
            //    }
            //}
            //foreach (var _produtosCategory in _PingoDoceProduct.categories.Reverse<Models.PingoDoceProduct.Category>())
            //{
            //    if (_productCategoriesCounter > 0 && _productCategoriesCounter < _PingoDoceProduct.categories.Count)
            //    {
            //        _productCategoriesFull += _produtosCategory.name + " > ";
            //    }
            //    else if (_productCategoriesCounter == _PingoDoceProduct.categories.Count)
            //    {
            //        _productCategoriesFull += _produtosCategory.name;
            //    }
            //    _productCategoriesCounter++;
            //}


            //_productSearchResultData = new ProductSearchResult
            //{
            //    Brand = _PingoDoceProduct.brand.name,
            //    //Category = _PingoDoceProduct.categories.Count > 0 ? _PingoDoceProduct.categories[0].name : string.Empty,
            //    Category = _productCategory,
            //    FullCategory = _productCategoriesFull,
            //    ImageUrl = "https://res.cloudinary.com/fonte-online/image/upload/c_fill,h_300,q_auto,w_300/v1/PDO_PROD/" + _PingoDoceProduct.sku + "_1",
            //    Name = _PingoDoceProduct.firstName,
            //    Price = _PingoDoceProduct.buyingPrice.ToString(),
            //    PriceLiteral = float.Parse(_PingoDoceProduct.buyingPrice.ToString()),
            //    PriceWeight = _PingoDoceProduct.buyingPrice.ToString(),
            //    Unit = _PingoDoceProduct.netContentUnit.ToLower().Replace("kgr", "kg").Replace("ltr", "lt").Replace("kgm", "kg").Replace("ltm", "lt"),
            //    PriceWeightLiteral = float.Parse(_PingoDoceProduct.buyingPrice.ToString()),
            //    StoreName = this.StoreName,
            //    StoreId = this.StoreId,
            //    StoreColor = this.StoreColor,
            //    //MAYBE USEFUL IN THE FUTURE
            //    //Url = "/api/catalogues/5afbf7f176f9b3001a672515/product/" + _PingoDoceProduct.slug,
            //    //ViewableUrl = "/api/catalogues/5afbf7f176f9b3001a672515/product/" + _PingoDoceProduct.slug,
            //    Url = url,
            //    ViewableUrl = "/store/pingo-doce/product/" + _PingoDoceProduct.slug,
            //    Weight = _PingoDoceProduct.capacity,
            //    OnlineProductId = _PingoDoceProduct.catalogueId
            //};
            //return _productSearchResultData;
        }

        async public Task<LisieStores.Extensibility.ProductSearchResult> GetProductMetadataById(string onlineProductId)
        {
            string url = "/api/products/" + onlineProductId;
            return await GetMetadata(url);
        }
        //async Task<List<LisieStores.Extensibility.ProductSearchResult>> IMarketFetcher.GetSearchResults(string searchQuery)
        //{
        //    string _searchQuery = string.IsNullOrEmpty(searchQuery) ? "" : searchQuery;
        //    string _searchResultsHtml = string.Empty;

        //    //HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("https://mercadao.pt/api/catalogues/5afbf7f176f9b3001a672515/products/search?query=" + searchQuery);
        //    HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("https://mercadao.pt/store/pingo-doce/search?queries=" + searchQuery);
        //    string _htmlResult = string.Empty;
        //    List<ProductSearchResult> _products = new List<ProductSearchResult>();
        //    try
        //    {
        //        webReq.CookieContainer = new CookieContainer();
        //        webReq.Method = "GET";
        //        using (WebResponse response = webReq.GetResponse())
        //        {
        //            using (Stream stream = response.GetResponseStream())
        //            {
        //                StreamReader reader = new StreamReader(stream);
        //                _searchResultsHtml = reader.ReadToEnd();

        //                CQ _dom = _searchResultsHtml;
        //                CQ _produtos = _dom[".pdo-product-item"];

        //                List<IDomObject> _productsList = _produtos.ToList();

        //                int _productCOunter = 1;
        //                foreach (IDomObject _productResult in _productsList)
        //                {
        //                    System.Diagnostics.Debug.WriteLine(_productCOunter);
        //                    ++_productCOunter;
        //                    try
        //                    {
        //                        CQ _productResultCQ = _productResult.InnerHTML;

        //                        //image - pdo - block media
        //                        //title  pdo-heading-s detail-title
        //                        //price weigh - pdo-product-price-per-unit
        //                        //price - detail-price

        //                        string _productUrl = _productResultCQ["a"].First().Attr("href");
        //                        CQ _productName = _productResultCQ["a"];
        //                        CQ _productInfo = ((CsQuery.Implementation.DomElement)(_productName[1])).InnerHTML;

        //                        //string _name = ((CsQuery.Implementation.DomElement)(_productInfo[0])).InnerHTML;
        //                        //string _name = _productInfo.Text();
        //                        //_name = _name.Replace("\n                            ", "");
        //                        //_name = _name.Replace(" \n\n                    ", "");
        //                        //_name = _name.Trim(' ');

        //                        //string _productBrand = _productResultCQ[".product-item-brand"].First().Text().Replace("\n", "").Trim();
        //                        ////string _productWeight = _name;
        //                        //string _productWeight = string.Empty;
        //                        //CQ _productPriceCQ = _productResultCQ[".product-item-price "].First();
        //                        //string _productPrice = _productPriceCQ[0].FirstChild.NodeValue.Replace("\n", "").Trim();

        //                        //string _productPriceWeight = _productResultCQ[".product-item-quantity-price"].First().Text().Replace("\n", "").Trim();

        //                        string _productImageUrl = _productResultCQ[".product-item-image"].First()["img"].Attr("src");
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Debug.WriteLine(ex.Message);
        //                    }
        //                }

        //                Models.PingoDoceSearch.ProductSerchResults _PingoDoceSearchResults = (Models.PingoDoceSearch.ProductSerchResults)js.Deserialize(lastJson, typeof(Models.PingoDoceSearch.ProductSerchResults));

        //                foreach (Models.PingoDoceSearch.Product _product in _PingoDoceSearchResults.products)
        //                {
        //                    _products.Add(new ProductSearchResult
        //                    {
        //                        Brand = _product._source.brand.name,
        //                        Category = _product._source.categories.Count > 0 ? _product._source.categories[0].name : string.Empty,
        //                        ImageUrl = "https://res.cloudinary.com/fonte-online/image/upload/c_fill,h_300,q_auto,w_300/v1/PDO_PROD/" + _product._source.sku + "_1",
        //                        Name = _product._source.firstName,
        //                        //Price = "€ " + Math.Round(_product._source.buyingPrice, 2).ToString(),
        //                        Price = Math.Round(_product._source.buyingPrice, 2).ToString() + "€",
        //                        PriceLiteral = float.Parse(_product._source.buyingPrice.ToString()),
        //                        PriceWeight = _product._source.buyingPrice.ToString() + " / " + _product._source.capacity,
        //                        PriceWeightLiteral = float.Parse(_product._source.buyingPrice.ToString()),
        //                        StoreName = this.StoreName,
        //                        StoreId = this.StoreId,
        //                        StoreColor = this.StoreColor,
        //                        Url = "/api/catalogues/5afbf7f176f9b3001a672515/product/" + _product._source.slug,
        //                        Weight = _product._source.capacity

        //                    });
        //                }
        //                return _products;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }

        //    return _products;
        //}


        ///DONT DELETE - HAS THE METHOD TO PASS JSON TO OBJECT
        ///VERY IMPORTANT
        ///

        async Task<List<LisieStores.Extensibility.ProductSearchResult>> IMarketFetcher.GetSearchResults(string searchQuery)
        {
            string _searchQuery = string.IsNullOrEmpty(searchQuery) ? "" : searchQuery;
            string _searchResultsHtml = string.Empty;

            //HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("https://mercadao.pt/api/catalogues/5afbf7f176f9b3001a672515/products/search?query=" + searchQuery);
            //HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("https://mercadao.pt/api/catalogues/5afbf7f176f9b3001a672515/products/search?query=" + searchQuery);
            //Last One
            //HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("https://mercadao.pt/api/catalogues/5b2d35b85ce104001af36fca/products/search?query=" + searchQuery);

            //is this one the generic one??? it was on incognito mode - 6107d28d72939a003ff6bf51
            HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("https://mercadao.pt/api/catalogues/6107d28d72939a003ff6bf51/products/search?esPreference=0.985508185983917&query=" + searchQuery);

            webReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36";
            webReq.Accept = "*/*";
            string _htmlResult = string.Empty;
            List<ProductSearchResult> _products = new List<ProductSearchResult>();
            try
            {
                webReq.CookieContainer = new CookieContainer();
                webReq.Method = "GET";

                using (WebResponse response = webReq.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        _searchResultsHtml = reader.ReadToEnd();

                        int startIndex = _searchResultsHtml.IndexOf("\"products\":[{");
                        int lastIndex = _searchResultsHtml.LastIndexOf("\"categories\":[{");
                        int lastIndex2 = _searchResultsHtml.Remove(lastIndex).LastIndexOf("]") + 1;

                        string lastJson = _searchResultsHtml.Substring(startIndex, lastIndex2 - startIndex);
                        //var values = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(_searchResultsHtml);


                        lastJson = "{" + lastJson + "}";

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        Models.PingoDoceSearch.ProductSerchResults _PingoDoceSearchResults = (Models.PingoDoceSearch.ProductSerchResults)js.Deserialize(lastJson, typeof(Models.PingoDoceSearch.ProductSerchResults));

                        foreach (Models.PingoDoceSearch.Product _product in _PingoDoceSearchResults.products)
                        {
                            _products.Add(new ProductSearchResult
                            {
                                Barcode = _product._source.eans != null && _product._source.eans.Count > 0 ? _product._source.eans[0] : string.Empty,
                                Brand = _product._source.brand.name,
                                Category = _product._source.categories.Count > 0 ? _product._source.categories[0].name : string.Empty,
                                ImageUrl = "https://res.cloudinary.com/fonte-online/image/upload/c_fill,h_300,q_auto,w_300/v1/PDO_PROD/" + _product._source.sku + "_1",
                                Name = _product._source.firstName,
                                //Price = "€ " + Math.Round(_product._source.buyingPrice, 2).ToString(),
                                Price = Math.Round(_product._source.buyingPrice, 2).ToString() + "€",
                                PriceWithoutDiscount = _product._source.buyingPrice != _product._source.regularPrice ? Math.Round(_product._source.regularPrice, 2).ToString() + "€" : string.Empty,
                                PriceLiteral = float.Parse(_product._source.buyingPrice.ToString()),
                                PriceWeight = _product._source.buyingPrice.ToString() + " / " + _product._source.capacity,
                                PriceWeightLiteral = float.Parse(_product._source.buyingPrice.ToString()),
                                StoreName = this.StoreName,
                                StoreId = this.StoreId,
                                StoreColor = this.StoreColor,
                                //OLD ONE, maybe i will still use
                                //Url = "/api/catalogues/5afbf7f176f9b3001a672515/product/" + _product._source.slug,
                                //ViewableUrl = "/api/catalogues/5afbf7f176f9b3001a672515/product/" + _product._source.slug,
                                //Url = "/api/catalogues/5b2d35b85ce104001af36fca/product/" + _product._source.slug,
                                Url = "/api/catalogues/6107d28d72939a003ff6bf51/product/" + _product._source.slug,
                                ViewableUrl = "/store/pingo-doce/product/" + _product._source.slug,
                                Weight = _product._source.capacity,
                                OnlineProductId = _product._id,
                                FullCategory = "",
                                Unit = ""
                            });
                        }
                        return _products;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return _products;
        }

        async Task<LisieStores.Extensibility.ProductSearchResult> GetMetadata(string url)
        {
            try
            {
                string _htmlResult = await FetchUrl("https://mercadao.pt" + url);
                //System.Diagnostics.Debug.WriteLine("TRYING: " + "https://mercadao.pt" + url);
                //System.Threading.Thread.Sleep(4000);
                //if (_htmlResult == "null")
                //{
                //    url = url.Replace("5afbf7f176f9b3001a672515", "6107d28d72939a003ff6bf51");
                //    _htmlResult = await FetchUrl("https://mercadao.pt" + url);
                //    System.Diagnostics.Debug.WriteLine("TRYING: " + "https://mercadao.pt" + url);
                //}
                if (_htmlResult == "null" || _htmlResult == string.Empty)
                {
                    int _startIndex = url.IndexOf("/catalogues/") + ("/catalogues/").Length;
                    int _endIndex = url.IndexOf("/product/", _startIndex);
                    url = url.Replace(url.Substring(_startIndex, _endIndex - _startIndex), "6107d28d72939a003ff6bf51");
                    //url = url.Replace("5afbf7f176f9b3001a672515", "6107d28d72939a003ff6bf51");
                    _htmlResult = await FetchUrl("https://mercadao.pt" + url);
                    //System.Diagnostics.Debug.WriteLine("TRYING: " + "https://mercadao.pt" + url);
                }
                ProductSearchResult _productSearchResultData = new ProductSearchResult();
                JavaScriptSerializer js = new JavaScriptSerializer();
                Models.PingoDoceProduct.Product _PingoDoceProduct = (Models.PingoDoceProduct.Product)js.Deserialize(_htmlResult, typeof(Models.PingoDoceProduct.Product));

                if (_PingoDoceProduct == null) //if null return
                    return null;


                //If is from /api/products/ , go also get from url, to get buyngPrice (price with discount) and PriceWeight and category
                if (url.ToLower().Contains("/api/products/"))
                {
                    //System.Threading.Thread.Sleep(4000);
                    string _CatalogueApiUrl = "/api/catalogues/" + _PingoDoceProduct.catalogueId + "/product/" + _PingoDoceProduct.slug;
                    _htmlResult = await FetchUrl("https://mercadao.pt" + _CatalogueApiUrl);
                    System.Diagnostics.Debug.WriteLine("TRYING: " + "https://mercadao.pt" + _CatalogueApiUrl);
                    if (!string.IsNullOrEmpty(_htmlResult))
                    {
                        Models.PingoDoceProduct.Product _PingoDoceProductOfCatalogue = (Models.PingoDoceProduct.Product)js.Deserialize(_htmlResult, typeof(Models.PingoDoceProduct.Product));

                        if (_PingoDoceProductOfCatalogue != null) //fill _PingoDoceProduct with missing properties
                        {
                            _PingoDoceProduct.buyingPrice = _PingoDoceProductOfCatalogue.buyingPrice;
                            _PingoDoceProduct.ancestorsCategoriesArray = _PingoDoceProductOfCatalogue.ancestorsCategoriesArray;
                            _PingoDoceProduct.categories = _PingoDoceProductOfCatalogue.categories;
                            _PingoDoceProduct.brand = _PingoDoceProductOfCatalogue.brand;

                            url = _CatalogueApiUrl;
                        }
                    }
                }

                //Categories
                int _productCategoriesCounter = 1;
                string _productCategory = "";
                string _productCategoriesFull = "";
                if (_PingoDoceProduct.ancestorsCategoriesArray != null)
                {
                    foreach (var _produtosCategory in _PingoDoceProduct.ancestorsCategoriesArray.Reverse<string>())
                    {
                        if (_productCategoriesCounter == 1)
                        {
                            _productCategory = _produtosCategory;
                            break;
                        }
                    }
                }
                if (_PingoDoceProduct.categories != null)
                {
                    foreach (var _produtosCategory in _PingoDoceProduct.categories.Reverse<Models.PingoDoceProduct.Category>())
                    {
                        if (_productCategoriesCounter > 0 && _productCategoriesCounter < _PingoDoceProduct.categories.Count)
                        {
                            _productCategoriesFull += _produtosCategory.name + " > ";
                        }
                        else if (_productCategoriesCounter == _PingoDoceProduct.categories.Count)
                        {
                            _productCategoriesFull += _produtosCategory.name;
                        }
                        _productCategoriesCounter++;
                    }
                }


                double _calculatedPriceWeight = 0;
                _calculatedPriceWeight = Math.Round(double.Parse(_PingoDoceProduct.buyingPrice.ToString()) / _PingoDoceProduct.netContent, 2);

                string _ean = _PingoDoceProduct.defaultEan;
                if (_ean == null || string.IsNullOrEmpty(_ean))
                {
                    if (_PingoDoceProduct.eans.Count > 0)
                    {
                        _ean = _PingoDoceProduct.eans[0];
                    }
                }

                _productSearchResultData = new ProductSearchResult
                {
                    Barcode = _ean,
                    Brand = _PingoDoceProduct.brand.name,
                    Category = _productCategory,
                    FullCategory = _productCategoriesFull,
                    ImageUrl = "https://res.cloudinary.com/fonte-online/image/upload/c_fill,h_300,q_auto,w_300/v1/PDO_PROD/" + _PingoDoceProduct.sku + "_1",
                    Name = _PingoDoceProduct.firstName,
                    Price = _PingoDoceProduct.buyingPrice.ToString() != "0" ? _PingoDoceProduct.buyingPrice.ToString() : _PingoDoceProduct.regularPrice.ToString(),
                    PriceLiteral = float.Parse(_PingoDoceProduct.buyingPrice.ToString()),
                    PriceWeight = _calculatedPriceWeight.ToString(),
                    Unit = _PingoDoceProduct.netContentUnit.ToLower().Replace("kgr", "kg").Replace("ltr", "lt").Replace("kgm", "kg").Replace("ltm", "lt"),
                    PriceWeightLiteral = float.Parse(_PingoDoceProduct.buyingPrice.ToString()),
                    StoreName = this.StoreName,
                    StoreId = this.StoreId,
                    StoreColor = this.StoreColor,
                    Url = url,
                    ViewableUrl = "/store/pingo-doce/product/" + _PingoDoceProduct.slug,
                    Weight = _PingoDoceProduct.capacity,
                    OnlineProductId = _PingoDoceProduct.id
                };
                return _productSearchResultData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Task<ProductSearchResult> FindProductAI(string name, string brand, string weight)
        {
            throw new NotImplementedException();
        }
    }
}