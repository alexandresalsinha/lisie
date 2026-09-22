using CsQuery;
using LisieStores.Extensibility;
using SpiroWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
                var _onlineProductId = "";
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


        public async Task<ProductSearchResult> FindProductAI(string name, string brand, string weight, string barcode = "")
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(15); // Set a long timeout to prevent request timeout

                    var requestBody = new
                    {
                        prompt = "start a fresh browser instance using playwright with no open pages. Then go to this website, \"https://www.elcorteingles.pt/supermercado\", and follow the instructions bellow in sequential order:\r\n\r\n- if there´s a cookie dialog, accept all cookies\r\n- look for the product with this characteristics\r\n\t" +
                        "- Name: \"" + name + "\r\n\t" +
                        "- Brand: \"" + brand + "\r\n\t" +
                        "- Weight: \"" + weight + "\r\n" +
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