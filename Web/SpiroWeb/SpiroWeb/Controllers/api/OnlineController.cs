using ClassLibrary1;
using SpiroWeb.Helpers;
using SpiroWeb.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;

namespace SpiroWeb.Controllers.api
{
    //TODO - refactor to here
    public class OnlineController : ApiController
    {
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<LisieStores.Extensibility.ProductSearchResult> GetProductMetadata(int storeId, string url)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            LisieStores.Extensibility.ProductSearchResult _result = await _OnlineProducts.GetProductMetadata(storeId, url);
            return _result;

        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<LisieStores.Extensibility.ProductSearchResult> FindProductAI(int storeId, string name, string brand, string weight)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            LisieStores.Extensibility.ProductSearchResult _result = await _OnlineProducts.FindProductAI(storeId, name, brand, weight);
            return _result;

        }

        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        //public async Task<LisieStores.Extensibility.ProductSearchResult> FindProductAI(int productId)
        //{
        //    OnlineProducts _OnlineProducts = new OnlineProducts();
        //    LisieStores.Extensibility.ProductSearchResult _result = await _OnlineProducts.FindProductAI(productId);
        //    return _result;

        //}

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<LisieStores.Extensibility.ProductSearchResult> GetMarketProductByOnlineId(int storeId, string onlineProductId)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            LisieStores.Extensibility.ProductSearchResult _result = await _OnlineProducts.GetMarketProductByOnlineId(storeId, onlineProductId);
            return _result;

        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<List<LisieStores.Extensibility.ProductSearchResult>> GetMarketsSearchResults(string searchQuery)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchMatchResult = await _OnlineProducts.GetSearchResults(searchQuery);
            return _ProductSearchMatchResult;
        }
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<List<Models.OnlineStoreSearchResults>> GetMarketsSearchResultsV2(string searchQuery)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            List<Models.OnlineStoreSearchResults> _ProductSearchMatchResult = await _OnlineProducts.GetSearchResultsV2(searchQuery);
            return _ProductSearchMatchResult;
        }
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<List<LisieStores.Extensibility.ProductSearchResult>> GetMarketSearchResults(int storeId, string searchQuery)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchMatchResult = await _OnlineProducts.GetSearchResultsWithNoSeparatorsOfStore(searchQuery, storeId);
            return _ProductSearchMatchResult;
        }
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> AddProductsToOnlineStoreCart([FromBody] Models.AddProductsToOnlineStoreCartPostModel data)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            bool _result = await _OnlineProducts.AddProductsToOnlineStoreCart(data.UserId, data.StoreId, data.StoreUsername, data.StorePassword, data.UserProductsIds);
            return Request.CreateResponse(HttpStatusCode.Created, _result);
        }
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> AddAllStoreProductsToOnlineStoreCart(string userId, int storeId, string storeUsername, string storePassword)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            bool _result = await _OnlineProducts.AddAllStoreProductsToOnlineStoreCart(userId, storeId, storeUsername, storePassword);
            return Request.CreateResponse(HttpStatusCode.Created, _result);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> GetAllStoreProductsToAddToOnlineStoreCart(string userId, int storeId)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            List<LisieStores.Extensibility.ProductAddToOnlineStore> _result = await _OnlineProducts.GetAllStoreProductsToAddToOnlineStoreCart(userId, storeId);
            return Request.CreateResponse(HttpStatusCode.Created, _result);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<LisieStores.Extensibility.ProductSearchResult> GetMarketProductByBarcode(int storeId, string barcode)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            LisieStores.Extensibility.ProductSearchResult _ProductSearchMatchResult = await _OnlineProducts.GetMarketProductByBarcode(storeId, barcode);
            return _ProductSearchMatchResult;
        }
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<List<LisieStores.Extensibility.ProductSearchResult>> GetMarketsProductByBarcode(string barcode)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchMatchResult = await _OnlineProducts.GetMarketsProductByBarcode(barcode);
            return _ProductSearchMatchResult;
        }


        //MAYBE USE IN FUTURW, DONT DELETE
        //[HttpGet]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        //public async Task<HttpResponseMessage> UpdateAuchanUrlAndOnlineProductIds()
        //{

        //    using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
        //    {
        //        //auchan
        //        var _auchanStoreProducts = db.StoreProducts.Where(c => c.StoreId == 1 && c.Url.StartsWith("/Frontoffice")).OrderByDescending(c => c.Id);
        //        var counter = _auchanStoreProducts.Count();
        //        string newUrl = string.Empty;
        //        string _parsedOnlineProductId = string.Empty;
        //        List<string> _results = new List<string>();
        //        List<int> _productIdWithBarcodeUpdated = new List<int>();
        //        List<int> _productIdStoreUpdated = new List<int>();
        //        int _counter = 0;
        //        foreach (var _auchanStoreProduct in _auchanStoreProducts)
        //        {

        //            newUrl = _auchanStoreProduct.Url;
        //            if (newUrl.IndexOf("/Auchan_Amadora") > -1)
        //            {
        //                newUrl = newUrl.Remove(newUrl.IndexOf("/Auchan_Amadora"));
        //            }
        //            _parsedOnlineProductId = newUrl.Substring(newUrl.LastIndexOf("/") + 1);

        //            newUrl = "/on/demandware.store/Sites-AuchanPT-Site/pt_PT/Product-IncludeProductShow?pid=" + _parsedOnlineProductId;

        //            //update store product
        //            _results.Add("old url : " + _auchanStoreProduct.Url);
        //            _auchanStoreProduct.Url = newUrl;
        //            _auchanStoreProduct.OnlineProductId = _parsedOnlineProductId;
        //            db.Entry(_auchanStoreProduct).State = System.Data.Entity.EntityState.Modified;
        //            _productIdStoreUpdated.Add(_auchanStoreProduct.ProductId.Value);
        //            _results.Add("ProductId " + _auchanStoreProduct.ProductId.Value + " - auchan store updated - id: " + _auchanStoreProduct.Id);
        //            _results.Add("with new url " + newUrl);
        //            OnlineProducts _OnlineProducts = new OnlineProducts();
        //            LisieStores.Extensibility.ProductSearchResult _result = await _OnlineProducts.GetProductMetadata(1, newUrl);
        //            if (_result != null) //if it got the online metadata
        //            {
        //                _results.Add("Found online");
        //                if (!string.IsNullOrEmpty(_result.Barcode)) // check if product has barcode, if it doe´sn´t, add it
        //                {
        //                    Products _product = db.Products.Where(c => c.Id == _auchanStoreProduct.ProductId.Value).FirstOrDefault();
        //                    if (_product != null && _product.Barcode == "0")
        //                    {
        //                        //before updating check if barcode already exists in db
        //                        var _productWithBarcodeExists = db.Products.Where(c => c.Barcode == _result.Barcode).FirstOrDefault();
        //                        if (_productWithBarcodeExists == null)
        //                        {
        //                            _product.Barcode = _result.Barcode;
        //                            db.Entry(_product).State = System.Data.Entity.EntityState.Modified;
        //                            _productIdWithBarcodeUpdated.Add(_product.Id);
        //                            _results.Add("Barcode of ProductId " + _product.Id + " updated to " + _result.Barcode);
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                _results.Add("NOT found online");
        //            }
        //            //System.Threading.Thread.Sleep(500);
        //            _results.Add("-----------------------------------");
        //            _counter++;
        //            if (_counter == 50)
        //            {
        //                break;
        //            }
        //        }
        //        db.SaveChanges();

        //        //now do price updates for all
        //        foreach (var _productId in _productIdStoreUpdated)
        //        {
        //            var _res = await ProductsManager.UpdatePricesNew(_productId);
        //            System.Threading.Thread.Sleep(500);
        //        }

        //        //continente
        //        return Request.CreateResponse(HttpStatusCode.Created, _results);
        //    }
        //}


        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetProductsWithBarcodeWithoutAuchanStoreCount()
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _alreadySearched = db.ProductsBarcodeFoundAuchan.Select(c => c.ProductId).ToList();
                    var _products = db.Products.Include("StoreProducts").Where(c => !_alreadySearched.Contains(c.Id) && c.Barcode != "0" && c.StoreProducts.Count(y => y.StoreId == 1) == 0).OrderByDescending(c => c.Id);
                    var _productsCount = _products.Count();
                    return Request.CreateResponse(HttpStatusCode.Created, _productsCount);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetProductsWithBarcodeWithoutAuchanStore()
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _alreadySearched = db.ProductsBarcodeFoundAuchan.Select(c => c.ProductId);
                    var _products = db.Products.Include("StoreProducts").Where(c => !_alreadySearched.Contains(c.Id) && c.Barcode != "0" && c.StoreProducts.Count(y => y.StoreId == 1) == 0).OrderByDescending(c => c.Id).Select(c => c.Id).Take(5000).ToList();
                    var _productsCount = _products.Count();
                    return Request.CreateResponse(HttpStatusCode.OK, _products);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> UpdateProductWithBarcodeWithAuchanStore(int productId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _product = db.Products.Where(c => c.Id == productId).FirstOrDefault();

                    string _result = string.Empty;
                    OnlineProducts _OnlineProducts = new OnlineProducts();

                    var _productOnlineFound = await _OnlineProducts.GetMarketProductByBarcode(1, _product.Barcode);
                    if (_productOnlineFound != null)
                    {
                        bool _sucess = ProductsManager.CreateOrUpdateStoreProductNew(_productOnlineFound, _product.Id, "9ff8224f-17cf-49fb-b555-05779a13eb40", 1);
                        _result = "ProductId: " + _product.Id + " with Barcode - " + _product.Barcode + " - Updated:" + _sucess;

                        db.ProductsBarcodeFoundAuchan.Add(new ProductsBarcodeFoundAuchan
                        {
                            ProductId = _product.Id,
                            Found = false,
                            CreateDate = DateTime.Now
                        });
                        db.SaveChanges();
                    }
                    else //not found online
                    {
                        _result = "ProductId: " + _product.Id + " with Barcode - " + _product.Barcode + " Not founded";

                        db.ProductsBarcodeFoundAuchan.Add(new ProductsBarcodeFoundAuchan
                        {
                            ProductId = _product.Id,
                            Found = false,
                            CreateDate = DateTime.Now
                        });
                        db.SaveChanges();
                    }

                    System.Threading.Thread.Sleep(5000);
                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    resp.Content = new StringContent(_result, System.Text.Encoding.UTF8, "text/plain");
                    return resp;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> GetStoreGetterLastCategory(int storeId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.OK);

                    var _category = db.StoreGetterLastCategory.Where(c => c.StoreId == storeId).Select(c => c.LastCategory).FirstOrDefault();
                    if (_category != null)
                        resp.Content = new StringContent(_category, System.Text.Encoding.UTF8, "text/plain");
                    else
                        resp.Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "text/plain");

                    return resp;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> SetStoreGetterLastCategory(int storeId, string lastCategory)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _exists = db.StoreGetterLastCategory.Where(c => c.StoreId == storeId).FirstOrDefault();
                    if (_exists != null)
                    {
                        _exists.LastCategory = lastCategory;
                        _exists.UpdateDate = DateTime.Now;
                        db.Entry(_exists).Property(y => y.LastCategory).IsModified = true;
                        db.Entry(_exists).Property(y => y.UpdateDate).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.StoreGetterLastCategory.Add(new StoreGetterLastCategory
                        {
                            StoreId = storeId,
                            LastCategory = lastCategory,
                            UpdateDate = DateTime.Now
                        });
                        db.SaveChanges();
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, true);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        //[HttpGet]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        //public async Task<HttpResponseMessage> UpdateContinenteUrlAndOnlineProductIds()
        //{

        //    using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
        //    {
        //        //auchan
        //        var _StoreProducts = db.StoreProducts.Where(c => c.StoreId == 2 && c.Url.StartsWith("/stores")).OrderByDescending(c => c.Id);
        //        //var _StoreProducts = db.StoreProducts.Where(c => c.StoreId == 2 && c.Url.IndexOf("(eCsf_RetekProductCatalog_MegastoreContinenteOnline_Continente)") > -1).OrderByDescending(c => c.Id);
        //        var counter = _StoreProducts.Count();
        //        string newUrl = string.Empty;
        //        string _parsedOnlineProductId = string.Empty;
        //        List<string> _results = new List<string>();
        //        List<int> _productIdWithBarcodeUpdated = new List<int>();
        //        List<int> _productIdStoreUpdated = new List<int>();
        //        int _counter = 0;
        //        foreach (var _StoreProduct in _StoreProducts)
        //        {
        //            newUrl = _StoreProduct.Url;
        //            //_parsedOnlineProductId = newUrl.Substring(newUrl.LastIndexOf("=") + 1);

        //            newUrl = "/produto/" + _StoreProduct.OnlineProductId + ".html";
        //            _results.Add("old url : " + _StoreProduct.Url);
        //            _StoreProduct.Url = newUrl;
        //            //update store product
        //            db.Entry(_StoreProduct).State = System.Data.Entity.EntityState.Modified;
        //            _productIdStoreUpdated.Add(_StoreProduct.ProductId.Value);
        //            _results.Add("ProductId " + _StoreProduct.ProductId.Value + " - Continente store updated - id: " + _StoreProduct.Id);
        //            _results.Add("with new url " + newUrl);

        //            _counter++;
        //            if (_counter == 5000)
        //            {
        //                break;
        //            }
        //            //break;
        //        }
        //        db.SaveChanges();

        //        //now do price updates for all
        //        //foreach (var _productId in _productIdStoreUpdated)
        //        //{
        //        //    var _res = await ProductsManager.UpdatePricesNew(_productId);
        //        //    System.Threading.Thread.Sleep(500);
        //        //}

        //        //continente
        //        return Request.CreateResponse(HttpStatusCode.Created, _results);
        //    }
        //}

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> UpdatePingoDoceOldProductWithNewUrl()
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _storeProducts = db.StoreProducts.Where(c => c.StoreId == 3 && c.NeedsUpdate.Value == true);
                    //var _storeProducts = db.StoreProducts.Where(c => c.Id == 244);
                    var _count = _storeProducts.Count();
                    string _result = string.Empty;
                    OnlineProducts _OnlineProducts = new OnlineProducts();

                    foreach (var _storeProduct in _storeProducts)
                    {
                        try
                        {
                            LisieStores.Extensibility.ProductSearchResult _productOnlineFound = null;
                            if (!string.IsNullOrEmpty(_storeProduct.OnlineProductId))
                            {
                                _productOnlineFound = await _OnlineProducts.GetMarketProductByOnlineId(3, _storeProduct.OnlineProductId);
                            }
                            else if (!string.IsNullOrEmpty(_storeProduct.Url))
                            {
                                System.Threading.Thread.Sleep(4000);
                                _productOnlineFound = await _OnlineProducts.GetMarketProductByUrl(3, _storeProduct.Url);
                            }
                            if (_productOnlineFound != null)
                            {
                                ProductsManager.CreateOrUpdateStoreProductNew(_productOnlineFound, _storeProduct.ProductId.Value, "9ff8224f-17cf-49fb-b555-05779a13eb40", 3);
                                System.Diagnostics.Debug.WriteLine("Updating StoreProduct ID: " + _storeProduct.Id);
                            }
                            //else
                            //{
                            //    //try diferrent combinations
                            //    //remove last -X
                            //    System.Threading.Thread.Sleep(4000);
                            //    _productOnlineFound = await _OnlineProducts.GetMarketProductByUrl(3, _storeProduct.Url.Remove(_storeProduct.Url.LastIndexOf('-')));

                            //    if (_productOnlineFound != null)
                            //    {
                            //        ProductsManager.CreateOrUpdateStoreProductNew(_productOnlineFound, _storeProduct.ProductId.Value, "9ff8224f-17cf-49fb-b555-05779a13eb40", 3);
                            //        System.Diagnostics.Debug.WriteLine("Updating StoreProduct ID: " + _storeProduct.Id);
                            //    }
                            //    else
                            //    {
                            //        System.Threading.Thread.Sleep(4000);
                            //        _productOnlineFound = await _OnlineProducts.GetMarketProductByUrl(3, _storeProduct.Url.Remove(_storeProduct.Url.LastIndexOf('-')).Replace("5afbf7f176f9b3001a672515", "5b2d35b85ce104001af36fca"));
                            //        if (_productOnlineFound != null)
                            //        {
                            //            ProductsManager.CreateOrUpdateStoreProductNew(_productOnlineFound, _storeProduct.ProductId.Value, "9ff8224f-17cf-49fb-b555-05779a13eb40", 3);
                            //            System.Diagnostics.Debug.WriteLine("Updating StoreProduct ID: " + _storeProduct.Id);
                            //        }
                            //        else
                            //        {
                            //            System.Threading.Thread.Sleep(4000);
                            //            _productOnlineFound = await _OnlineProducts.GetMarketProductByUrl(3, _storeProduct.Url.Replace("5afbf7f176f9b3001a672515", "5b2d35b85ce104001af36fca"));
                            //            if (_productOnlineFound != null)
                            //            {
                            //                ProductsManager.CreateOrUpdateStoreProductNew(_productOnlineFound, _storeProduct.ProductId.Value, "9ff8224f-17cf-49fb-b555-05779a13eb40", 3);
                            //                System.Diagnostics.Debug.WriteLine("Updating StoreProduct ID: " + _storeProduct.Id);
                            //            }
                            //        }
                            //    }
                            //}
                        }
                        catch (Exception ex)
                        {

                            System.Diagnostics.Debug.WriteLine("ERROR " + ex.Message);
                        }
                        System.Threading.Thread.Sleep(4000);
                    }


                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    resp.Content = new StringContent(_result, System.Text.Encoding.UTF8, "text/plain");
                    return resp;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> UpdateMiniPrecoImageUrlBug()
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _storeProducts = db.StoreProducts.Where(c => c.StoreId == 5);
                    //var _storeProducts = db.StoreProducts.Where(c => c.Id == 244);
                    var _count = _storeProducts.Count();

                    foreach (var _storeProduct in _storeProducts)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(_storeProduct.ImageUrl))
                            {
                                if (_storeProduct.ImageUrl.IndexOf("https://lojaonlin") > -1)
                                {
                                    _storeProduct.ImageUrl = _storeProduct.ImageUrl.Replace("https://lojaonlin", "");
                                    db.Entry(_storeProduct).State = System.Data.Entity.EntityState.Modified;
                                    //db.SaveChanges();
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                            System.Diagnostics.Debug.WriteLine("ERROR " + ex.Message);
                        }
                        //System.Threading.Thread.Sleep(4000);
                    }
                    db.SaveChanges();

                    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                    resp.Content = new StringContent("ok", System.Text.Encoding.UTF8, "text/plain");
                    return resp;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }



        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetStoresProductsToAddToOnlineStoreCart(string userId, string storeIds)
        {
            var _splitted = storeIds.Split(',');
            List<int> _storeIds = new List<int>();
            foreach (var _split in _splitted)
            {
                _storeIds.Add(int.Parse(_split));
            }
            OnlineProducts _OnlineProducts = new OnlineProducts();
            List<LisieStores.Extensibility.StoreAddToOnline> _result = _OnlineProducts.GetStoresProductsToAddToOnlineStoreCart(userId, _storeIds);
            return Request.CreateResponse(HttpStatusCode.Created, _result);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetStoresProductsTotalSavings(string userId, string storeIds)
        {
            var _splitted = storeIds.Split(',');
            List<int> _storeIds = new List<int>();
            foreach (var _split in _splitted)
            {
                _storeIds.Add(int.Parse(_split));
            }
            OnlineProducts _OnlineProducts = new OnlineProducts();
            var _result = _OnlineProducts.GetStoresProductsTotalSavings(userId, _storeIds);
            return Request.CreateResponse(HttpStatusCode.Created, _result);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage RecordUserStoresProductsTotalSavings(string userId, string storeIds)
        {
            var _splitted = storeIds.Split(',');
            List<int> _storeIds = new List<int>();
            foreach (var _split in _splitted)
            {
                _storeIds.Add(int.Parse(_split));
            }
            OnlineProducts _OnlineProducts = new OnlineProducts();
            var _result = _OnlineProducts.GetStoresProductsTotalSavings(userId, _storeIds);
            if (_result != null)
            {
                UserListsManager.RecordUserStoresProductsTotalSavings(userId, storeIds, _result.Cheapest, _result.Highest, _result.PriceDifference, _result.TotalProducts, int.Parse(_result.PercentageValue.ToString()));
            }
            return Request.CreateResponse(HttpStatusCode.Created, _result);
        }


        [HttpPost]
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> AddUserProductsToOnlineCartsLegacy(string userId, string storeIds)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36");
            try
            {
                string response = await client.GetStringAsync("http://localhost:3005/AddUserProductsToOnlineCarts?userId=" + userId + "&storeIds=" + storeIds);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> AddUserProductsToOnlineCarts([FromBody] AddUserProductsToOnlineCartsModel model)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36");
            try
            {
                var cli = new WebClient();
                cli.Headers[HttpRequestHeader.ContentType] = "application/json";
                try
                {
                    var json = new JavaScriptSerializer().Serialize(model);
                    string response = cli.UploadString("http://localhost:3005/AddUserProductsToOnlineCartsV2", json);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                catch (Exception ex)
                {
                    Logger.Debug("Error:" + ex.InnerException.Message);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }

        [HttpPost]
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> LoginMarket([FromBody] LoginMarketModel model)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36");
            try
            {
                string response = await client.GetStringAsync("http://localhost:3005/LoginMarket?userId=" + model.UserId + "&storeId=" + model.StoreId + "&username=" + model.Username + "&password=" + model.Password);

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }


        [HttpPost]
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> GetIntermarcheProductMetadata(string url)
        {
            var client = new HttpClient();
            //client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36");
            try
            {
                string response = await client.GetStringAsync("http://localhost:3000/getIntermarcheProductMetadata" + url);

                JavaScriptSerializer _JavaScriptSerializer = new JavaScriptSerializer();
                LisieStores.Extensibility.ProductSearchResult _parsedJson = _JavaScriptSerializer.Deserialize<LisieStores.Extensibility.ProductSearchResult>(response);

                return Request.CreateResponse(HttpStatusCode.OK, _parsedJson);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }

        [HttpPost]
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> GetIntermarcheSearchResults(string search)
        {
            var client = new HttpClient();
            //client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36");
            try
            {
                string response = await client.GetStringAsync("http://localhost:3000/getIntermarcheSearchResults?search=" + search);

                JavaScriptSerializer _JavaScriptSerializer = new JavaScriptSerializer();
                List<LisieStores.Extensibility.ProductSearchResult> _parsedJson = _JavaScriptSerializer.Deserialize<List<LisieStores.Extensibility.ProductSearchResult>>(response);

                return Request.CreateResponse(HttpStatusCode.OK, _parsedJson);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }





    }
    public class LoginMarketModel
    {
        public string UserId { get; set; }
        public int StoreId { get; set; }
        public string Username { get; set; }

        public string Password { get; set; }

    }

    public class AddUserProductsToOnlineCartsModel
    {
        public string UserId { get; set; }
        public List<MarketData> Stores { get; set; }

    }

    public class MarketData
    {
        public int StoreId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<MarketProduct> Products { get; set; }
    }

    public class MarketProduct
    {
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public int UserProductListId { get; set; }
        public string Name { get; set; }
        public string OnlineProductId { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Url { get; set; }
    }


}
