using LisieStores.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SpiroWeb.Markets
{
    //[MarketAttr(3, "Pingo Doce", "https://www.pingodoce.pt", "#8BC53F")]
    [MarketAttr(3, "Pingo Doce", "https://www.pingodoce.pt", "#8BC53F", "https://www.pingodoce.pt/on/demandware.store/Sites-pingo-doce-Site/default/Search-Show?q=")]
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

        public async Task<ProductSearchResult> FindProductAI(string name, string brand, string weight, string barcode = "")
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(15); // Set a long timeout to prevent request timeout

                    var requestBody = new
                    {
                        prompt = "start a fresh browser instance using playwright with no open pages. Then go to this website, \"https://www.pingodoce.pt/\", and follow the instructions bellow in sequential order:\r\n\r\n- if there´s a cookie dialog, accept all cookies\r\n- look for the product with this characteristics\r\n\t" +
                        "- Name: \"" + name + "\"\r\n\t" +
                        "- Brand: \"" + brand + "\"\r\n\t" +
                        "- Weight: \"" + weight + "\"\r\n" +
                        "- extract the product information and output the data in JSON format, using this example for the properties to extract and the JSON schema. If barcode property doesn´t exist in the page, fill it with tan empty value. Only output the json, with no text behind or after\r\n\t-  {\"barcode\": \"5601244500063\",\"name\": \"Leite de Pastagem Meio Gordo\",\"brand\": \"Terra Nostra\",\"url\": \"https://www.elcorteingles.pt/supermercado/B052020616600167-terra-nostra-leite-de-pastagem-meio-gordo-1-l/\",\"imageUrl\": \"https://sgfm.elcorteingles.es/SGFM/dctm/MEDIA03/202109/29/05220912200803____10__1200x1200.jpg\"\"productId\": \"B052020616600167\",\"weight\":\"Quant. Mínima = 600 gr (3 un)\",\"price\": {\"amount\": 1.09,\"currency\": \"EUR\",\"formatted\": \"1,09 €\"},\"pricePerUnit\": {\"amount\": 1.09,\"unit\": \"Litro\",\"formatted\": \"1,09 € / Litro\"},\"\"priceWithoutDiscount\": {\"\"amount\": 1.09,\"\"currency\": \"EUR\",\"\"formatted\": \"1,09 €\"\"},\"quantity\": {\"value\": 1,\"unit\": \"Litro\",\"formatted\": \"1 l\"},\"categories\": [\"Supermercado\",\"Lacticínios e ovos\",\"Leite\",\"Leite UHT\",\"Leite UHT meio gordo\"]}"
                    };

                    var json = new JavaScriptSerializer().Serialize(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("http://localhost:4000/prompt", content);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //var responseContent = @"{\"answer\":\"```json\n{\n  \"barcode\": \"4072700003649\",\n  \"name\": \"Cerveja c/ álcool branca\",\n  \"brand\": \"Franziskan\",\n  \"weight\": \"Garrafa de 500 ml\",\n  \"url\": \"https://www.intermarche.pt/product/cerveja-c-ou-alcool-branca/4072700003649\",\n  \"imageUrl\": \"https://driveimg1.intermarche.com/pt/Content/images/boitmal/produit/zoom/557C857D9C4BA3F29BBB282C0FDF517E.jpg\",\n  \"productId\": \"4072700003649\",\n  \"price\": {\n    \"amount\": 2.19,\n    \"currency\": \"EUR\",\n    \"formatted\": \"2,19 €\"\n  },\n  \"pricePerUnit\": {\n    \"amount\": 4.38,\n    \"unit\": \"Litro\",\n    \"formatted\": \"4,38 € / Litro\"\n  },\n  \"quantity\": {\n    \"value\": 1,\n    \"unit\": \"Garrafa\",\n    \"formatted\": \"Garrafa de 500 ml\"\n  },\n  \"categories\": [\n    \"Bebidas\",\n    \"Cervejas e Sidras\",\n    \"Cervejas Estrangeiras e Artesanais\"\n  ]\n}\n```\"}";

                    Console.WriteLine(responseContent);

                    var serializer = new JavaScriptSerializer();
                    dynamic result = serializer.Deserialize<dynamic>(responseContent);
                    var _answer = result["answer"];
                    var jsonStartIndex = _answer.IndexOf("```json\n");
                    if (jsonStartIndex != -1)
                    {
                        _answer = _answer.Substring(jsonStartIndex + "```json\n".Length);
                        _answer = _answer.Replace("```", "").Trim();
                    }
                    dynamic _answerDynamic = serializer.Deserialize<dynamic>(_answer);
                    var _barcode = _answerDynamic["barcode"];
                    var _name = _answerDynamic["name"];
                    var _brand = _answerDynamic["brand"];
                    var _weight = _answerDynamic["weight"];
                    var _url = _answerDynamic["url"];
                    var _imageUrl = _answerDynamic["imageUrl"];
                    var _price = _answerDynamic["price"]["amount"];
                    var _priceWeight = _answerDynamic["pricePerUnit"]["amount"];
                    var _priceWeightUnit = _answerDynamic["pricePerUnit"]["unit"];
                    var _categoriesList = _answerDynamic["categories"];
                    var _priceWithoutDiscount = _answerDynamic["priceWithoutDiscount"]["amount"];


                    //List<string> items = new List<string> { "string 1", "string 2", "string 3" };
                    string _categoriesAll = string.Join(" > ", _categoriesList);

                    var _categories = ((object[])_categoriesList).Select(x => x.ToString()).ToList();


                    // Result: "string 1 > string 2 > string 3"
                    return new ProductSearchResult
                    {
                        Barcode = _barcode,
                        Name = _name,
                        Brand = _brand,
                        Weight = _weight,
                        Url = _url.Replace("https://www.pingodoce.pt", ""),
                        ImageUrl = _imageUrl,
                        OnlineProductId = string.Empty,
                        Category = _categories[_categories.Count() - 1],
                        Price = _price.ToString(),
                        PriceLiteral = (float)_price,
                        PriceWeightLiteral = (float)_priceWeight,
                        PriceWeight = _priceWeight.ToString(),
                        PriceWithoutDiscount = _priceWithoutDiscount.ToString(),
                        Unit = _priceWeightUnit,
                        StoreId = this.StoreId,
                        StoreName = this.StoreName,
                        FullCategory = _categoriesAll,
                        ViewableUrl = _url,
                        StoreColor = this.StoreColor,

                    };
                    //return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in FindProductAI: " + ex.Message);
                return null;
            }
        }

    }
}