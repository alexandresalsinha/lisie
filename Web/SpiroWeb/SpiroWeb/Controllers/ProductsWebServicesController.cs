using ClassLibrary1;
using CsQuery;
using DataManager;
using SpiroWeb.Helpers;
using SpiroWeb.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class ProductsWebServicesController : Controller
    {
        //LEGACY
        [HttpGet]
        public async Task<JsonResult> GetOnlineProductSearchResults(string searchQuery)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();

            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();

            List<LisieStores.Extensibility.ProductSearchResult> _jumboProductSearchResultList = await _OnlineProducts.GetJumboOnlineProductSearchResults(searchQuery);
            List<LisieStores.Extensibility.ProductSearchResult> _continenteProductSearchResultList = await _OnlineProducts.GetContinenteOnlineProductSearchResults(searchQuery);
            //List<Objects.ProductSearchResult> _continenteProductSearchResultList = await _OnlineProducts.GetContinenteOnlineProductSearchResultsMyPc(searchQuery);
            //List<Objects.ProductSearchResult> _continenteProductSearchResultList = await _OnlineProducts.GetContinenteOnlineProductSearchResultsHeroku(searchQuery);
            List<LisieStores.Extensibility.ProductSearchResult> _pingoDoceProductSearchResultList = await _OnlineProducts.GetPingoDoceOnlineProductSearchResults(searchQuery);


            _ProductSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
            {
                IsSeperator = true,
                SeparatorTitle = "Jumbo (" + _jumboProductSearchResultList.Count + ")"
            });
            _ProductSearchResultList.AddRange(_jumboProductSearchResultList);

            _ProductSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
            {
                IsSeperator = true,
                SeparatorTitle = "Continente (" + _continenteProductSearchResultList.Count + ")"
            });
            _ProductSearchResultList.AddRange(_continenteProductSearchResultList);

            if (_pingoDoceProductSearchResultList != null)
            {
                _ProductSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
                {
                    IsSeperator = true,
                    SeparatorTitle = "Pingo Doce (" + _pingoDoceProductSearchResultList.Count + ")"
                });
                _ProductSearchResultList.AddRange(_pingoDoceProductSearchResultList);
            }

            return Json(_ProductSearchResultList, JsonRequestBehavior.AllowGet);
        }

        //NEW - with dynamic stores
        [HttpGet]
        public async Task<JsonResult> GetOnlineProductSearchResults2(string searchQuery)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();

            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();

            List<LisieStores.Extensibility.ProductSearchResult> _jumboProductSearchResultList = await _OnlineProducts.GetJumboOnlineProductSearchResults(searchQuery);
            //This method is not working anymore, use heroku proxy
            //List<Objects.ProductSearchResult> _continenteProductSearchResultList = await _OnlineProducts.GetContinenteOnlineProductSearchResults(searchQuery);
            List<LisieStores.Extensibility.ProductSearchResult> _continenteProductSearchResultList = await _OnlineProducts.GetContinenteOnlineProductSearchResultsHeroku(searchQuery);
            List<LisieStores.Extensibility.ProductSearchResult> _pingoDoceProductSearchResultList = await _OnlineProducts.GetPingoDoceOnlineProductSearchResults(searchQuery);
            Markets.Intermache _intermarcheMarket = new Markets.Intermache();
            //List<Objects.ProductSearchResultNew> __intermarcheMarketSearchResultList = await _intermarcheMarket.GetSearchResults(searchQuery);


            _ProductSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
            {
                IsSeperator = true,
                SeparatorTitle = "Jumbo (" + _jumboProductSearchResultList.Count + ")"
            });
            _ProductSearchResultList.AddRange(_jumboProductSearchResultList);

            _ProductSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
            {
                IsSeperator = true,
                SeparatorTitle = "Continente (" + _continenteProductSearchResultList.Count + ")"
            });
            _ProductSearchResultList.AddRange(_continenteProductSearchResultList);

            if (_pingoDoceProductSearchResultList != null)
            {
                _ProductSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
                {
                    IsSeperator = true,
                    SeparatorTitle = "Pingo Doce (" + _pingoDoceProductSearchResultList.Count + ")"
                });
                _ProductSearchResultList.AddRange(_pingoDoceProductSearchResultList);
            }

            //if (__intermarcheMarketSearchResultList != null)
            //{
            //    _ProductSearchResultList.Add(new ProductSearchResult
            //    {
            //        IsSeperator = true,
            //        SeparatorTitle = "Intermarché (" + __intermarcheMarketSearchResultList.Count + ")"
            //    });
            //    _ProductSearchResultList.AddRange(__intermarcheMarketSearchResultList.Select(c=> new ProductSearchResult {
            //        Brand = c.Brand,
            //        Category = c.Category,
            //        ImageUrl = c.ImageUrl,
            //        Name = c.Name,
            //        Price = c.Price,
            //        PriceLiteral = c.PriceLiteral,
            //        PriceWeight = c.PriceWeight,
            //        PriceWeightLiteral = c.PriceWeightLiteral,
            //        Store = c.StoreName,
            //        Url = c.Url,
            //        Weight = c.Weight

            //    }));
            //}

            return Json(_ProductSearchResultList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetByBarCode(long barCode)
        {
            ProductsManager _ProductsManager = new ProductsManager();
            Products _product = _ProductsManager.GetByBarCode(barCode);
            return Json(_product, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// to remove - just for testing
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetPingoDoceSearchResults(string searchQuery)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();

            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResultList = await _OnlineProducts.GetPingoDoceOnlineProductSearchResults(searchQuery);
            return Json(_ProductSearchResultList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetContinenteSearchResults(string searchQuery)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();

            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResultList = await _OnlineProducts.GetContinenteOnlineProductSearchResults(searchQuery);
            return Json(_ProductSearchResultList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetJumboSearchResults(string searchQuery)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();

            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResultList = await _OnlineProducts.GetJumboOnlineProductSearchResults(searchQuery);
            return Json(_ProductSearchResultList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// to remove- just for testing
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetPingoDoceProduct(string productUrl)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();

            LisieStores.Extensibility.ProductSearchResult _pingoDoceProductSearchResultList = await _OnlineProducts.GetPingoDoceProductMetadata(productUrl);
            return Json(_pingoDoceProductSearchResultList, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public async Task<JsonResult> GetMetadataFromJumboProduct(string url)
        {

            HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("https://www.jumbo.pt" + url);
            string _htmlResult = string.Empty;
            try
            {
                webReq.CookieContainer = new CookieContainer();
                webReq.Method = "GET";
                using (WebResponse response = webReq.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        _htmlResult = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
            }

            List<ProductSearchResult> _productSearchResultList = new List<ProductSearchResult>();

            CQ _jumboDom = _htmlResult;
            //CQ _jumboDom = _jumboSearchResultsHtml;
            //CQ  _produtos = _jumboDom[".produtoGrelha"];
            CQ _produtos = _jumboDom[".product-item-border"];

            List<IDomObject> _productsList = _produtos.ToList();

            foreach (IDomObject _productResult in _productsList)
            {
                Console.WriteLine(_productResult.InnerHTML);

                CQ _productResultCQ = _productResult.InnerHTML;


                string _productUrl = _productResultCQ["a"].First().Attr("href");
                CQ _productName = _productResultCQ["a"];
                CQ _productInfo = ((CsQuery.Implementation.DomElement)(_productName[1])).InnerHTML;

                string _name = ((CsQuery.Implementation.DomElement)(_productInfo[0])).InnerHTML;

                string _productBrand = _productResultCQ[".product-item-brand"].First().Text().Replace("\n", "").Trim();
                //string _productWeight = _productResultCQ[".gr"].First().Text().Replace("\n", "").Trim();
                //string _productWeight = _name;
                string _productWeight = string.Empty;
                //string _productPrice = _productResultCQ[".product-item-price "].First().Text().Replace("\n", "").Trim();
                CQ _productPriceCQ = _productResultCQ[".product-item-price "].First();
                string _productPrice = _productPriceCQ[0].FirstChild.NodeValue.Replace("\n", "").Trim();

                string _productPriceWeight = _productResultCQ[".product-item-quantity-price"].First().Text().Replace("\n", "").Trim();

                //CQ _productImageUrl = _productResultCQ[".product-item-image hidden-print"].Attr("style");

                string _productImageUrl = _productResultCQ[".product-item-image"].First()["img"].Attr("src");

                _productUrl = _productUrl.Replace("http://www.jumbo.pt", "");
                _productSearchResultList.Add(new ProductSearchResult { Name = _name, Brand = _productBrand, Price = _productPrice, PriceWeight = _productPriceWeight, Store = "Jumbo", Url = _productUrl, Weight = _productWeight, ImageUrl = _productImageUrl });
            }

            return Json(_productSearchResultList, JsonRequestBehavior.AllowGet);
        }


        //LEGACY
        [HttpGet]
        public JsonResult AddProductToQueue(string barCode, string userId, string addType = "")
        {
            int _UserProductsListId = Helpers.ProductsQueue.ProcessProduct(barCode, userId, true, addType);

            //TODO - IMPROVE NOTIFICATINS, WHERE OR THERE?!
            if (_UserProductsListId != -1)
            {
                Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCart(_UserProductsListId, userId);
            }
            else
            {
                Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCartProductsInQueue(barCode, userId);
            }

            return Json(_UserProductsListId, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> AddProduct(string barCode, string userId, string addType = "")
        {
            int _UserProductsListId = await Helpers.ProductsQueue.ProcessProductNew(barCode, userId, true);

            //TODO - IMPROVE NOTIFICATINS, WHERE OR THERE?!
            if (_UserProductsListId != -1)
            {
                Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCart(_UserProductsListId, userId);
            }
            else
            {
                Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCartProductsInQueue(barCode, userId);
            }

            return Json(_UserProductsListId, JsonRequestBehavior.AllowGet);
        }


        //public JsonResult AddProductToQueue()
        //{
        //    //int _return = Helpers.ProductsQueue.ProcessProduct(barCode, userId, true);

        //    return Json("", JsonRequestBehavior.AllowGet);
        //}

        public async Task<string> FetchOnlineSearchResults(String url)
        {
            var client = new HttpClient();

            // Create the HttpContent for the form to be posted. and with this user PostAsync
            //var requestContent = new FormUrlEncodedContent(new[] {
            //new KeyValuePair<string, string>("text", "This is a block of text"),
            //});

            //Fake a Browser Request
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");


            // Get the response.
            //HttpResponseMessage response = await client.GetStringAsync("http://www.jumbo.pt/Frontoffice/ContentPages/CatalogSearch.aspx?Q=" + HttpUtility.UrlEncode(searchquery));
            //HttpResponseMessage response = await client.GetStringAsync("http://www.jumbo.pt");
            //string response = await client.GetStringAsync("http://www.google.pt");
            //string response = await client.GetStringAsync("http://www.jumbo.pt/Frontoffice/ContentPages/CatalogSearch.aspx?Q=" + searchquery);

            string response = await client.GetStringAsync(url);

            return response;
            // Get the response content.
            //HttpContent responseContent = response.Content;

            //// Get the stream of the content.
            //using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            //{
            //    // Write the output.
            //    string s = await reader.ReadToEndAsync();
            //    return s;
            //    //Console.WriteLine(await reader.ReadToEndAsync());
            //}
            //return "asdf";
        }

        //[HttpGet]
        //public async Task<JsonResult> GetMarketProductSearchResults(string searchQuery)
        //{
        //    OnlineProducts _OnlineProducts = new OnlineProducts();

        //    List<Objects.ProductSearchResult> _ProductSearchResultList = new List<ProductSearchResult>();

        //    List<Objects.ProductSearchResult> _jumboProductSearchResultList = await _OnlineProducts.GetJumboOnlineProductSearchResults(searchQuery);
        //    List<Objects.ProductSearchResult> _continenteProductSearchResultList = await _OnlineProducts.GetContinenteOnlineProductSearchResults(searchQuery);
        //    //List<Objects.ProductSearchResult> _continenteProductSearchResultList = await _OnlineProducts.GetContinenteOnlineProductSearchResultsMyPc(searchQuery);
        //    //List<Objects.ProductSearchResult> _continenteProductSearchResultList = await _OnlineProducts.GetContinenteOnlineProductSearchResultsHeroku(searchQuery);
        //    List<Objects.ProductSearchResult> _pingoDoceProductSearchResultList = await _OnlineProducts.GetPingoDoceOnlineProductSearchResults(searchQuery);


        //    _ProductSearchResultList.Add(new ProductSearchResult
        //    {
        //        IsSeperator = true,
        //        SeparatorTitle = "Jumbo (" + _jumboProductSearchResultList.Count + ")"
        //    });
        //    _ProductSearchResultList.AddRange(_jumboProductSearchResultList);

        //    _ProductSearchResultList.Add(new ProductSearchResult
        //    {
        //        IsSeperator = true,
        //        SeparatorTitle = "Continente (" + _continenteProductSearchResultList.Count + ")"
        //    });
        //    _ProductSearchResultList.AddRange(_continenteProductSearchResultList);

        //    if (_pingoDoceProductSearchResultList != null)
        //    {
        //        _ProductSearchResultList.Add(new ProductSearchResult
        //        {
        //            IsSeperator = true,
        //            SeparatorTitle = "Pingo Doce (" + _pingoDoceProductSearchResultList.Count + ")"
        //        });
        //        _ProductSearchResultList.AddRange(_pingoDoceProductSearchResultList);
        //    }

        //    return Json(_ProductSearchResultList, JsonRequestBehavior.AllowGet);
        //}
    }
}
