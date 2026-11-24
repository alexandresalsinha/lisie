using CsQuery;
using LisieStores.Extensibility;
using SpiroWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace SpiroWeb.Markets
{
    //[MarketAttr(4, "Intermarché", "https://lojaonline.intermarche.pt/62-arco-do-cego", "#000000")]
    [MarketAttr(4, "Intermarché", "https://www.loja-online.intermarche.pt", "#000000", "https://www.loja-online.intermarche.pt/search/")]
    public class Intermache : Market, IMarketFetcher
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

        //https://www.loja-online.intermarche.pt/product/ean/2892040000000
        public async Task<LisieStores.Extensibility.ProductSearchResult> GetProductMetadataByBarcode(string barcode)
        {
            var _url = "/product/ean/" + barcode;
            return await GetMetadata(_url);
        }

        async public Task<LisieStores.Extensibility.ProductSearchResult> GetProductMetadataById(string onlineProductId)
        {
            string url = "/" + onlineProductId;
            return await GetMetadata(url);
        }

        async Task<LisieStores.Extensibility.ProductSearchResult> IMarketFetcher.GetProductMetadata(string url)
        {
            return await GetMetadata(url);
        }

        async Task<List<LisieStores.Extensibility.ProductSearchResult>> IMarketFetcher.GetSearchResults(string searchQuery)
        {
            List<LisieStores.Extensibility.ProductSearchResult> _productSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();
            try
            {
                string _searchQuery = string.IsNullOrEmpty(searchQuery) ? "" : searchQuery;

                string _searchResultsHtml = await FetchUrl("http://localhost:3005/GetSearchResults?storeId=4&search=" + HttpUtility.UrlEncode(_searchQuery));

                if (string.IsNullOrEmpty(_searchResultsHtml))
                    return _productSearchResultList;

                CQ _Dom = _searchResultsHtml;
                string _nextDataJson = _Dom["#__NEXT_DATA__"].First().Text();

                if (string.IsNullOrEmpty(_nextDataJson))
                    return _productSearchResultList;

                JavaScriptSerializer js = new JavaScriptSerializer();
                dynamic _nextData = js.Deserialize<dynamic>(_nextDataJson);
                dynamic _list = _nextData["props"]["pageProps"]["initialProps"]["list"];
                object[] _products = _list != null && _list["produits"] != null ? (object[])_list["produits"] : null;

                if (_products == null)
                    return _productSearchResultList;

                foreach (var _productObj in _products)
                {
                    try
                    {
                        dynamic _product = _productObj;

                        string _productBarcode = _product["produitEan13"].ToString();
                        string _productName = _product["libelle"].ToString();
                        string _productBrand = _product["marque"] != null ? _product["marque"].ToString() : string.Empty;
                        string _productOnlineId = _product["itemId"] != null ? _product["itemId"].ToString() : string.Empty;

                        object[] _images = (object[])_product["images"];
                        string _productImageUrl = _images != null && _images.Length > 0 ? _images[0].ToString() : string.Empty;

                        double _productPriceNumeric = Convert.ToDouble(_product["prix"]);
                        double _productPriceWithoutDiscountNumeric = Convert.ToDouble(_product["prixBarre"]);
                        double _productUnitPriceNumeric = Convert.ToDouble(_product["unitPrice"]);
                        double _productCapacity = Convert.ToDouble(_product["capacite"]);

                        string _unitLabelRaw = _product["unitePrixVente"] != null ? _product["unitePrixVente"]["value"].ToString() : string.Empty; // e.g. "€/Kg"
                        string _productUnitRatio = _unitLabelRaw.IndexOf("/") > -1 ? _unitLabelRaw.Split('/')[1].Trim() : string.Empty;
                        _productUnitRatio = _productUnitRatio.ToLower();
                        if (_productUnitRatio == "l")
                            _productUnitRatio = "lt";

                        string _productWeight = _productCapacity > 0 ? (_productCapacity / 1000).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + (!string.IsNullOrEmpty(_productUnitRatio) ? " " + _productUnitRatio : string.Empty) : string.Empty;

                        string _productPrice = _productPriceNumeric.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture).Replace('.', ',');
                        string _productPriceWithDiscountValue = _productPriceWithoutDiscountNumeric > 0 && Math.Abs(_productPriceWithoutDiscountNumeric - _productPriceNumeric) > 0.001 ? _productPriceWithoutDiscountNumeric.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture).Replace('.', ',') : string.Empty;
                        string _productPriceWeight = _productUnitPriceNumeric.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

                        string _productUrl = "/product/ean/" + _productBarcode;

                        _productSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
                        {
                            Barcode = _productBarcode,
                            Name = _productName,
                            Brand = _productBrand,
                            Price = _productPrice,
                            PriceWithoutDiscount = _productPriceWithDiscountValue,
                            PriceWeight = _productPriceWeight,
                            StoreId = this.StoreId,
                            StoreName = this.StoreName,
                            StoreColor = this.StoreColor,
                            Url = _productUrl,
                            ViewableUrl = this.StoreUrl + _productUrl,
                            Weight = _productWeight,
                            ImageUrl = _productImageUrl,
                            Category = "",
                            FullCategory = "",
                            Unit = _productUnitRatio,
                            OnlineProductId = _productOnlineId
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                return _productSearchResultList;

            }
            catch (Exception ex)
            {
                return new List<ProductSearchResult>();
            }
        }

        //https://www.loja-online.intermarche.pt/product/iogurte-natural/5604260287284
        //async Task<LisieStores.Extensibility.ProductSearchResult> GetMetadata(string url)
        //{
        //    try
        //    {
        //        var _product = new LisieStores.Extensibility.ProductSearchResult();
        //        _product.StoreColor = this.StoreColor;
        //        _product.StoreId = this.StoreId;
        //        _product.StoreName = this.StoreName;

        //        //var cli = new WebClient();
        //        //cli.Encoding = System.Text.Encoding.UTF8;
        //        //cli.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/apng,*/*;q=0.8");
        //        //cli.Headers.Add("Accept-Language", "en-GB,en;q=0.9,en-US;q=0.8");
        //        //cli.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko; Google Page Speed Insights) Chrome/27.0.1453 Safari/537.36");
        //        ////cli.Headers[HttpRequestHeader.Cookie] = "itm_device_id=6493f98c-7250-4224-92f6-83ea81c6c4f7;itm_pdv=%7B%22ref%22%3A%2205047%22%2C%22isEcommerce%22%3Atrue%7D;itm_usid=6493f98c-7250-4224-92f6-83ea81c6c4f7\"";
        //        //cli.Headers[HttpRequestHeader.Cookie] = "itm_usid=49f7b8a7-84b5-41d1-9336-2bd6295b30e9;itm_device_id=083ffd05-2af7-4f25-ad0d-bfd7797343c4;itm_pdv={%22ref%22:%2209700%22%2C%22isEcommerce%22:true}";

        //        //string _html = cli.DownloadString("https://www.loja-online.intermarche.pt" + url);

        //        //var _url = "https://www.loja-online.intermarche.pt" + url;
        //        //string _html = await FetchUrl.MoreBypassesWithCookies(_url, "https://www.loja-online.intermarche.pt", "didomi_token=eyJ1c2VyX2lkIjoiMThmNzMyMjMtZTRhMi02MmZjLWI1YTktMzJlMTc2ZGQ0ZDgwIiwiY3JlYXRlZCI6IjIwMjQtMDUtMTNUMTg6MDY6MTUuMTE0WiIsInVwZGF0ZWQiOiIyMDI0LTA1LTEzVDE4OjA2OjI1LjUyNFoiLCJ2ZXJzaW9uIjoyLCJwdXJwb3NlcyI6eyJlbmFibGVkIjpbImdlb2xvY2F0aW9uX2RhdGEiLCJkZXZpY2VfY2hhcmFjdGVyaXN0aWNzIl19LCJ2ZW5kb3JzIjp7ImVuYWJsZWQiOlsiZ29vZ2xlIiwiYzpuZXN0bGUtUUxyVEx5OXQiLCJjOnNhbGVjeWNsZSIsImM6YmluZy1hZHMiLCJjOm1lZGlhbm9lLThLc3BUNVFaIiwiYzphYi10YXN0eSIsImM6cXVhbnR1bS1hZHZlcnRpc2luZyIsImM6dXNhYmlsbGEiLCJjOnByb2N0ZXJhbi1RM1ZFSk5pWSIsImM6c25hcGNoYXQtZnpOVUVpemoiLCJjOmRhdGFkb21lLWU2RGpnbXI3IiwiYzpkeW5hdHJhY2UtUVlGbWlUTUMiLCJjOnF1ZXVlaXQtV1laZkxSeEwiLCJjOmFkb3Rtb2IiLCJjOm1hdGNoYS1heXozQkxMOSIsImM6Y29udGVudHNxdWFyZSIsImM6bWljcm9zb2Z0IiwiYzpnb29nbGVhbmEtckp4emNjNjMiLCJjOnBpbnRlcmVzdCIsImM6bHVja3ljYXJ0LUxKYlBGclNqIl19LCJhYyI6IkM4R0FFQUZrRjRJQS5BQUFBIn0=;datadome=Glf6Tzot6N4McB0GAD_IHMBbYl~0ldZK7aA~7h5DuLwpY5uQJX6ue7IZAg7W8NwrgDQ7cVIOdvKc_VxeJlZqoxTxDur6oTdWscnr3zVn6KrjYroTtrfst_J5fJ~jHgSq;novaParams={%22pdvRef%22:%2209700%22};itm_device_id=fad3722a-22d1-47d2-a233-18fc87fbbab8;itm_usid=de528f2a-eb71-42fd-b0b2-b6016445f81c;itm_pdv={%22ref%22:%2209700%22%2C%22isEcommerce%22:true}");

        //        var _url = "http://localhost:3005/GetProductMetadata?storeId=4&url=" +HttpUtility.UrlEncode(url);
        //        string _html = await FetchUrl(_url);

        //        CQ _Dom = _html;
        //        CQ _produtos = _Dom[".productDetail__detail"];
        //        string _htmlSrc = _produtos.Html();

        //        //barcode
        //        var _barcodeFindIndex = _html.IndexOf("\"ean\":\"") + ("\"ean\":\"").Length;
        //        var _barcodeFindLastIndex = _html.IndexOf("\",", _barcodeFindIndex);
        //        var _barcode = _html.Substring(_barcodeFindIndex, _barcodeFindLastIndex - _barcodeFindIndex);

        //        //image
        //        var _imageFindIndex = _html.IndexOf("\"image\":");
        //        var _imageFindIndexLast = _html.IndexOf("\"src\":\"", _imageFindIndex) + ("\"src\":\"").Length;
        //        var _imageFindLast = _html.IndexOf("\"", _imageFindIndexLast);
        //        var _image = _html.Substring(_imageFindIndexLast, _imageFindLast - _imageFindIndexLast);


        //        //prices
        //        var _productAllHtml = _produtos[".productDetail__prices"].First().Html();
        //        var _productAllText = _produtos[".productDetail__prices"].First().Text();
        //        var _productPricesTextSpliteed = _productAllText.Split('€');
        //        var _price = string.Empty;
        //        var _priceWeight = string.Empty;
        //        var _unit = string.Empty;
        //        if (_productPricesTextSpliteed.Length < 4)
        //        {
        //            _price = _productPricesTextSpliteed[0].Replace(" ", "").Trim();
        //            _priceWeight = _productPricesTextSpliteed[1].Replace(" ", "").Trim();
        //            _unit = _productPricesTextSpliteed[2].Replace("/", "").Trim();
        //        }
        //        else
        //        {
        //            _price = _productPricesTextSpliteed[1].Replace(" ", "").Trim();
        //            _priceWeight = _productPricesTextSpliteed[2].Replace(" ", "").Trim();
        //            _unit = _productPricesTextSpliteed[3].Replace("/", "").Trim();
        //        }

        //        //OnlineProductId
        //        var _onlineProductIdFindIndex = _html.IndexOf("nfProductId\\\":") + ("nfProductId\\\":").Length;
        //        var _onlineProductIdFindLastIndex = _html.IndexOf("}", _onlineProductIdFindIndex);
        //        var _onlineProductId = _html.Substring(_onlineProductIdFindIndex, _onlineProductIdFindLastIndex - _onlineProductIdFindIndex);

        //        var _name = _produtos[".productDetail__main_title"].First().Text().Replace("\n", "").Trim();
        //        var _brand = _produtos[".productDetail__brand"].First().Text().Replace("\n", "").Trim();
        //        var _weight = _produtos[".productDetail__packaging"].First().Text().Replace("\n", "").Trim();


        //        //category
        //        var _categoryHtml = _Dom[".productDetail__pdpBreadcrumb__item"].First();
        //        var _category = _categoryHtml.Text();


        //        //Fix categories

        //        _product = new LisieStores.Extensibility.ProductSearchResult
        //        {
        //            Barcode = _barcode,
        //            Name = _name,
        //            Brand = _brand,
        //            Price = _price,
        //            PriceWeight = _priceWeight,
        //            StoreId = this.StoreId,
        //            StoreName = this.StoreName,
        //            StoreColor = this.StoreColor,
        //            Url = url,
        //            ViewableUrl = "https://www.loja-online.intermarche.pt" + url,
        //            Weight = _weight,
        //            ImageUrl = _image,
        //            PriceLiteral = 0,
        //            PriceWeightLiteral = 0,
        //            Category = "",
        //            FullCategory = "",
        //            Unit = _unit,
        //            OnlineProductId = _onlineProductId
        //        };

        //        return _product;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //new one with puppeteer bypasses
        async Task<LisieStores.Extensibility.ProductSearchResult> GetMetadata(string url)
        {
            try
            {
                var _url = "http://localhost:3005/GetProductMetadata?storeId=4&url=" + HttpUtility.UrlEncode(url);
                string _html = await FetchUrl(_url);

                if (string.IsNullOrEmpty(_html))
                    return null;

                CQ _Dom = _html;
                string _nextDataJson = _Dom["#__NEXT_DATA__"].First().Text();

                if (string.IsNullOrEmpty(_nextDataJson))
                    return null;

                JavaScriptSerializer js = new JavaScriptSerializer();
                dynamic _nextData = js.Deserialize<dynamic>(_nextDataJson);
                dynamic _product = _nextData["props"]["pageProps"]["product"];

                if (_product == null)
                    return null;

                string _barcode = _product["produitEan13"].ToString();
                string _name = _product["libelle"].ToString();
                string _brand = _product["marque"] != null ? _product["marque"].ToString() : string.Empty;
                string _onlineProductId = _product["itemId"] != null ? _product["itemId"].ToString() : string.Empty;

                object[] _images = (object[])_product["images"];
                string _imageUrl = _images != null && _images.Length > 0 ? _images[0].ToString() : string.Empty;

                double _price = Convert.ToDouble(_product["prix"]);
                double _priceWithoutDiscount = Convert.ToDouble(_product["prixBarre"]);
                double _unitPrice = Convert.ToDouble(_product["unitPrice"]);
                double _capacity = Convert.ToDouble(_product["capacite"]);

                string _unitLabelRaw = _product["unitePrixVente"] != null ? _product["unitePrixVente"]["value"].ToString() : string.Empty; // e.g. "€/L"
                string _unit = _unitLabelRaw.IndexOf("/") > -1 ? _unitLabelRaw.Split('/')[1].Trim() : string.Empty;
                _unit = _unit.ToLower();
                if (_unit == "l")
                    _unit = "lt";

                string _weight = _capacity > 0 ? (_capacity / 1000).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + (!string.IsNullOrEmpty(_unit) ? " " + _unit : string.Empty) : string.Empty;

                //category (breadcrumb array of { type, id, label })
                List<string> _categoryLabels = new List<string>();
                object[] _breadcrumbItems = (object[])_product["breadcrumb"];
                if (_breadcrumbItems != null)
                {
                    foreach (var _breadcrumbItem in _breadcrumbItems)
                    {
                        dynamic _breadcrumbItemDynamic = _breadcrumbItem;
                        _categoryLabels.Add(_breadcrumbItemDynamic["label"].ToString());
                    }
                }
                string _category = _categoryLabels.Count > 0 ? _categoryLabels[_categoryLabels.Count - 1] : string.Empty;
                string _categoryFull = string.Join(" > ", _categoryLabels);

                string _priceDisplay = _price.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture).Replace('.', ',');
                string _priceWithoutDiscountDisplay = _priceWithoutDiscount > 0 && Math.Abs(_priceWithoutDiscount - _price) > 0.001 ? _priceWithoutDiscount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture).Replace('.', ',') : string.Empty;
                string _priceWeightDisplay = _unitPrice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

                return new LisieStores.Extensibility.ProductSearchResult
                {
                    Barcode = _barcode,
                    Name = _name,
                    Brand = _brand,
                    Price = _priceDisplay,
                    PriceWithoutDiscount = _priceWithoutDiscountDisplay,
                    PriceWeight = _priceWeightDisplay,
                    StoreId = this.StoreId,
                    StoreName = this.StoreName,
                    StoreColor = this.StoreColor,
                    Url = url,
                    ViewableUrl = this.StoreUrl + url,
                    Weight = _weight,
                    ImageUrl = _imageUrl,
                    Category = _category,
                    FullCategory = _categoryFull,
                    Unit = _unit,
                    OnlineProductId = _onlineProductId
                };
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
                        prompt = "usa o playwright e vai a este website: \"https://www.intermarche.pt/\" .Usa sempre uma nova instancia de browser sem abas abertas. Se for preciso, Primeiro aceita as cookies. Depois, Onde diz código postal, introduz este valor \"1675-822\". quando aparecer a lista seleciona a loja \"INTERMARCHÉ SUPER FAMÕES\" . Mas se a loja \"Famões\" ou \"INTERMARCHÉ SUPER FAMÕES\" já estiver seleciona, não faças a instrução anterior.  Depois quando acabar de fazer loading a página, procura por este produto com o nome \"" + name + "\", de marca \"" + brand + "\", e de peso \"" + weight + "\".  procura com diversas combinações, até encontrares o mais similar do produto que referi.  retorna toda a informação possível do produto em formato JSON. Finge que és um browser normal. retorna o json com este esquema: {\"\"barcode\": \"5601244500063\",\"\"name\": \"Leite de Pastagem Meio Gordo\",\"\"brand\": \"Terra Nostra\",\"\"weight\": \"1lt\",\"\"url\": \"https://www.elcorteingles.pt/supermercado/B052020616600167-terra-nostra-leite-de-pastagem-meio-gordo-1-l/\",\"\"imageUrl\": \"https://sgfm.elcorteingles.es/SGFM/dctm/MEDIA03/202109/29/05220912200803____10__1200x1200.jpg\",\"\"productId\": \"B052020616600167\",\"\"price\": {\"\"amount\": 1.09,\"\"currency\": \"EUR\",\"\"formatted\": \"1,09 €\"\"},\"\"pricePerUnit\": {\"\"amount\": 1.09,\"\"unit\": \"Litro\",\"\"formatted\": \"1,09 € / Litro\"\"},\"\"priceWithoutDiscount\": {\"\"amount\": 1.09,\"\"currency\": \"EUR\",\"\"formatted\": \"1,09 €\"\"},\"\"quantity\": {\"\"value\": 1,\"\"unit\": \"Litro\",\"\"formatted\": \"1 l\"\"},\"\"categories\": [\"\"Supermercado\",\"\"Lacticínios e ovos\",\"\"Leite\",\"\"Leite UHT\",\"\"Leite UHT meio gordo\"\"]\"}\"}. Muito importante, não mostrar nenhum texto antes e depois do json, inclusive na propriedade \"answer\", retornar só mesmo o json."
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

                    var _categories =  ((object[])_categoriesList).Select(x => x.ToString()).ToList();


                    // Result: "string 1 > string 2 > string 3"
                    return new ProductSearchResult
                    {
                        Barcode = _barcode,
                        Name = _name,
                        Brand = _brand,
                        Weight = _weight,
                        Url = _url.Replace("https://www.intermarche.pt", ""),
                        ImageUrl = _imageUrl,
                        OnlineProductId = string.Empty,
                        Category = _categories[_categories.Count() - 1],
                        Price = _price.ToString(),
                        PriceLiteral = (float)_price,
                        PriceWeightLiteral= (float)_priceWeight,
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