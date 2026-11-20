using CsQuery;
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
            string url = "/on/demandware.store/Sites-pingo-doce-Site/default/Product-Show?pid=" + onlineProductId;
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
            string _searchResultsHtml = await FetchUrl("https://www.pingodoce.pt/on/demandware.store/Sites-pingo-doce-Site/default/Search-Show?q=" + _searchQuery);

            List<ProductSearchResult> _products = new List<ProductSearchResult>();
            try
            {
                CQ _dom = _searchResultsHtml;
                CQ _produtos = _dom[".product"];

                List<IDomObject> _productsList = _produtos.ToList();

                foreach (IDomObject _productResult in _productsList)
                {
                    try
                    {
                        CQ _productResultCQ = _productResult.OuterHTML;

                        string _productOnlineId = _productResultCQ.Attr("data-pid");
                        string _productUrl = _productResultCQ[".product-tile-image-link"].First().Attr("href");
                        string _productImageUrl = _productResultCQ[".product-tile-component-image"].First().Attr("src");
                        string _productName = _productResultCQ[".product-name-link"].Text().Replace("\n", "").Trim();
                        string _productBrand = _productResultCQ[".product-brand-name"].Text().Replace("\n", "").Trim();

                        string _productUnitText = _productResultCQ[".product-unit"].Text().Replace("\n", "").Trim();
                        string _productWeight = _productUnitText;
                        string _productPriceWeight = string.Empty;
                        string _productUnit = string.Empty;
                        if (_productUnitText.IndexOf("|") > -1)
                        {
                            string[] _unitParts = _productUnitText.Split('|');
                            _productWeight = _unitParts[0].Trim();
                            _productPriceWeight = _unitParts[1].Trim();
                            _productUnit = _productPriceWeight.IndexOf("/") > -1 ? _productPriceWeight.Split('/')[1].Trim() : string.Empty;
                        }

                        string _productPriceValue = _productResultCQ[".sales .value"].First().Attr("content");
                        string _productPriceWithoutDiscountValue = _productResultCQ[".strike-through .value"].First().Attr("content");

                        float _productPriceLiteral = 0;
                        float.TryParse(_productPriceValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out _productPriceLiteral);

                        _products.Add(new ProductSearchResult
                        {
                            Barcode = string.Empty,
                            Brand = _productBrand,
                            Category = "",
                            FullCategory = "",
                            ImageUrl = _productImageUrl,
                            Name = _productName,
                            Price = (!string.IsNullOrEmpty(_productPriceValue) ? _productPriceValue : "0") + "€",
                            PriceWithoutDiscount = !string.IsNullOrEmpty(_productPriceWithoutDiscountValue) ? _productPriceWithoutDiscountValue + "€" : string.Empty,
                            PriceLiteral = _productPriceLiteral,
                            PriceWeight = _productPriceWeight,
                            PriceWeightLiteral = _productPriceLiteral,
                            StoreName = this.StoreName,
                            StoreId = this.StoreId,
                            StoreColor = this.StoreColor,
                            Url = _productUrl,
                            ViewableUrl = this.StoreUrl + _productUrl,
                            Weight = _productWeight,
                            OnlineProductId = _productOnlineId,
                            Unit = _productUnit
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
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
                string _fullUrl = url.ToLower().StartsWith("http") ? url : "https://www.pingodoce.pt" + url;
                string _htmlResult = await FetchUrl(_fullUrl);

                if (string.IsNullOrEmpty(_htmlResult))
                    return null;

                CQ _dom = _htmlResult;
                CQ _productWrapper = _dom[".product-wrapper"].First();

                if (_productWrapper.Length == 0)
                    return null;

                string _productId = _productWrapper.Attr("data-pid");
                string _gtmInfoJson = _productWrapper.Attr("data-gtm-info");

                string _productCategory = string.Empty;
                string _productCategoryParent = string.Empty;
                if (!string.IsNullOrEmpty(_gtmInfoJson))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    dynamic _gtmInfo = js.Deserialize<dynamic>(_gtmInfoJson);
                    object[] _gtmItems = (object[])_gtmInfo["items"];
                    if (_gtmItems != null && _gtmItems.Length > 0)
                    {
                        dynamic _gtmItem = _gtmItems[0];
                        _productCategory = (string)_gtmItem["item_category"];
                        _productCategoryParent = (string)_gtmItem["item_category2"];
                    }
                }
                string _productCategoriesFull = !string.IsNullOrEmpty(_productCategoryParent) ? _productCategoryParent + " > " + _productCategory : _productCategory;

                string _productName = _dom[".product-name"].First().Text().Replace("\n", "").Trim();
                string _productBrand = _dom[".product-brand"].First().Text().Replace("\n", "").Trim();
                string _productImageUrl = _dom[".primary-images img"].First().Attr("src");
                string _productCanonicalUrl = _dom["link[rel='canonical']"].First().Attr("href");
                if (string.IsNullOrEmpty(_productCanonicalUrl))
                    _productCanonicalUrl = url;

                string _productPriceValue = _dom[".price .sales .value"].First().Attr("content");
                string _productPriceWithoutDiscountValue = _dom[".price .strike-through .value"].First().Attr("content");
                string _productSalesText = _dom[".price .sales"].First().Text().Replace("\n", "").Trim();

                string _productUnitText = _dom[".product-unit-measure"].First().Text().Replace("\n", "").Trim();
                string _productWeight = _productUnitText;
                string _productPriceWeight = string.Empty;
                string _productUnit = string.Empty;
                if (_productUnitText.IndexOf("|") > -1)
                {
                    string[] _unitParts = _productUnitText.Split('|');
                    _productWeight = _unitParts[0].Trim();
                    string _pricePerUnitText = _unitParts[1].Trim(); // e.g. "1,91 €/L"
                    _productPriceWeight = _pricePerUnitText.Split(' ')[0].Replace(',', '.'); // e.g. "1.91"
                    _productUnit = _pricePerUnitText.IndexOf("/") > -1 ? _pricePerUnitText.Split('/')[1].Trim() : string.Empty;
                }
                else if (_productSalesText.IndexOf("/") > -1)
                {
                    //price is already expressed per unit (products sold by weight, e.g. "19,89 €/Kg")
                    _productPriceWeight = _productPriceValue;
                    _productUnit = _productSalesText.Split('/')[1].Trim();
                }
                _productUnit = _productUnit.ToLower();
                if (_productUnit == "l")
                    _productUnit = "lt";

                string _productPriceDisplay = !string.IsNullOrEmpty(_productPriceValue) ? _productPriceValue.Replace('.', ',') : string.Empty;
                string _productPriceWithoutDiscountDisplay = !string.IsNullOrEmpty(_productPriceWithoutDiscountValue) ? _productPriceWithoutDiscountValue.Replace('.', ',') : string.Empty;

                ProductSearchResult _productSearchResultData = new ProductSearchResult
                {
                    Barcode = string.Empty,
                    Brand = _productBrand,
                    Category = _productCategory,
                    FullCategory = _productCategoriesFull,
                    ImageUrl = _productImageUrl,
                    Name = _productName,
                    Price = _productPriceDisplay,
                    PriceWithoutDiscount = _productPriceWithoutDiscountDisplay,
                    PriceWeight = _productPriceWeight,
                    StoreName = this.StoreName,
                    StoreId = this.StoreId,
                    StoreColor = this.StoreColor,
                    Url = _productCanonicalUrl,
                    ViewableUrl = this.StoreUrl + _productCanonicalUrl,
                    Weight = _productWeight,
                    OnlineProductId = _productId,
                    Unit = _productUnit
                };
                return _productSearchResultData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

        public async Task<ProductSearchResult> ExtractProductInfoAI(string url)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(15); // Set a long timeout to prevent request timeout

                    var requestBody = new
                    {
                        url,
                        storeId = this.StoreId,
                    };

                    var json = new JavaScriptSerializer().Serialize(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("http://localhost:4000/productMetadata", content);
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