using ClassLibrary1;
using SpiroWeb.Helpers;
using SpiroWeb.Managers;
using SpiroWeb.Models;
using SpiroWeb.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;

namespace SpiroWeb.Controllers.Api
{
    /// <summary>
    /// TO GO IN FUTURE TO OBSOLETE- Go to api/ProductsController
    /// </summary>


    public class ProductsController : ApiController
    {
        // GET: api/ProductsApi/5
        //TODO - add userId for interactions
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> Get(int id = -1, string barcode = "", string userId = "")
        {
            Products product;

            if (id != -1)
                product = Managers.ProductsManager.GetById(id);
            else if (barcode != string.Empty)
                product = Managers.ProductsManager.GetByBarcode(barcode);
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            if (product == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            //First update prices before returning product - TEMPORARY , to do with push notifications also
            //await Managers.ProductsManager.UpdatePricesNew(product.Id);
            //if (id != -1)
            //    product = Managers.ProductsManager.GetById(id);
            //else if (barcode != string.Empty)
            //    product = Managers.ProductsManager.GetByBarcode(barcode);
            //else
            //    return Request.CreateResponse(HttpStatusCode.BadRequest);

            var _productDTO = new Products()
            {
                Id = product.Id,
                Barcode = product.Barcode,
                Brand = product.Brand,
                CategoryString = product.CategoryString,
                InsertDate = product.InsertDate,
                Name = product.Name,
                //Picture = Helpers.Settings.WebURL + "/handlers/getproductImage.ashx?productId=" + product.Id,
                Price = product.Price,
                VariableWeightPrice = product.VariableWeightPrice,
                StoreProducts = Managers.ProductsManager.GetStoreProductsCopy(product.StoreProducts, userId),
                Weight = product.Weight,
                AddedByUserId = product.AddedByUserId,
                CreatedByUserId = product.CreatedByUserId,
                IsTemp = product.IsTemp
            };

            return Request.CreateResponse(HttpStatusCode.OK, _productDTO);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> GetUpdated(int id = -1, string barcode = "", string userId = "")
        {
            Products product;

            //First update prices before returning product - TEMPORARY , to do with push notifications also
            await Managers.ProductsManager.UpdatePricesNew(id);
            if (id != -1)
                product = Managers.ProductsManager.GetById(id);
            else if (barcode != string.Empty)
                product = Managers.ProductsManager.GetByBarcode(barcode);
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var _productDTO = new Products()
            {
                Id = product.Id,
                Barcode = product.Barcode,
                Brand = product.Brand,
                CategoryString = product.CategoryString,
                InsertDate = product.InsertDate,
                Name = product.Name,
                //Picture = Helpers.Settings.WebURL + "/handlers/getproductImage.ashx?productId=" + product.Id,
                Price = product.Price,
                VariableWeightPrice = product.VariableWeightPrice,
                StoreProducts = Managers.ProductsManager.GetStoreProductsCopy(product.StoreProducts.OrderBy(c => c.Price).ToList(), userId),
                Weight = product.Weight,
                AddedByUserId = product.AddedByUserId,
                CreatedByUserId = product.CreatedByUserId
            };

            return Request.CreateResponse(HttpStatusCode.OK, _productDTO);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> GetUpdatedV2(int productId, string userId)
        {
            await Managers.ProductsManager.UpdatePricesNew(productId);
            var _completeModelV2 = UserListsManager.GetCompleteModelV2(-1, userId, productId);

            return Request.CreateResponse(HttpStatusCode.OK, _completeModelV2);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> FindStoreProductsWithAI(int productId, string userId)
        {
            var _y = await ProductsManager.FindStoreProductsWithAI(productId);
            var _completeModelV2 = UserListsManager.GetCompleteModelV2(-1, userId, productId);

            return Request.CreateResponse(HttpStatusCode.OK, _completeModelV2);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> ExtractProductInfoAI(int productId, string userId, int storeId)
        {
            var _y = await ProductsManager.ExtractProductInfoAI(productId, userId, storeId);
            var _completeModelV2 = UserListsManager.GetCompleteModelV2(-1, userId, productId);

            return Request.CreateResponse(HttpStatusCode.OK, _completeModelV2);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> GetByBarcodeAndSearchOnlineAndAddIfFound(string barcode, string userId)
        {
            Products product;

            if (barcode != string.Empty)
                product = await Managers.ProductsManager.GetByBarcodeAndSearchOnlineAndAddIfFound(barcode, userId);
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            if (product == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            var _productDTO = new Products()
            {
                Id = product.Id,
                Barcode = product.Barcode,
                Brand = product.Brand,
                CategoryString = product.CategoryString,
                InsertDate = product.InsertDate,
                Name = product.Name,
                //Picture = Helpers.Settings.WebURL + "/handlers/getproductImage.ashx?productId=" + product.Id,
                Price = product.Price,
                VariableWeightPrice = product.VariableWeightPrice,
                StoreProducts = Managers.ProductsManager.GetStoreProductsCopy(product.StoreProducts),
                Weight = product.Weight,
                AddedByUserId = product.AddedByUserId,
                CreatedByUserId = product.CreatedByUserId
            };

            return Request.CreateResponse(HttpStatusCode.OK, _productDTO);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> GetByBarcodeOnline(string barcode)
        {
            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResult;

            if (barcode != string.Empty)
                _ProductSearchResult = await Managers.ProductsManager.GetByBarcodeOnline(barcode);
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            if (_ProductSearchResult == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            return Request.CreateResponse(HttpStatusCode.OK, _ProductSearchResult);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetAll(int page, string query, bool withoutBarcode = false)
        {
            //Managers.InteractionsManager.Add(userId, "api/ProductsWatchersController/Get", userId);
            Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");
            try
            {
                //List<Models.UserProductListCompleteModel2> _products = Managers.ProductsManager.GetAll(page, query, withoutBarcode);
                List<Models.UserProductListCompleteModel2> _products = Managers.ProductsManager.GetAllV2(page, query, withoutBarcode);
                if (_products != null)
                    return Request.CreateResponse(HttpStatusCode.OK, _products);
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.InnerException.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetAllV1(int page, string query, bool withoutBarcode = false)
        {
            //Managers.InteractionsManager.Add(userId, "api/ProductsWatchersController/Get", userId);

            try
            {
                //List<Models.UserProductListCompleteModel2> _products = Managers.ProductsManager.GetAll(page, query, withoutBarcode);
                List<Models.UserProductListCompleteModel2> _products = Managers.ProductsManager.GetAll(page, query, withoutBarcode);
                if (_products != null)
                    return Request.CreateResponse(HttpStatusCode.OK, _products);
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetAllV2(int page, string query, bool withoutBarcode = false)
        {
            //Managers.InteractionsManager.Add(userId, "api/ProductsWatchersController/Get", userId);

            try
            {
                List<Models.UserProductListCompleteModel2> _products = Managers.ProductsManager.GetAllV2(page, query, withoutBarcode);
                if (_products != null)
                    return Request.CreateResponse(HttpStatusCode.OK, _products);
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }


        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetAllV3(int page, string query, bool withoutBarcode = false, string userId = "")
        {
            //Managers.InteractionsManager.Add(userId, "api/ProductsWatchersController/Get", userId);

            try
            {
                //List<Models.UserProductListCompleteModel2> _products = Managers.ProductsManager.GetAllV3(page, query, withoutBarcode, userId);
                List<Models.UserProductListCompleteModel2> _products = Managers.ProductsManager.GetAllV3(page, query, withoutBarcode);
                if (_products != null)
                    return Request.CreateResponse(HttpStatusCode.OK, _products);
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetAllV4(int page, string query, bool withoutBarcode = false, string userId = "")
        {
            //Managers.InteractionsManager.Add(userId, "api/ProductsWatchersController/Get", userId);

            try
            {
                List<Models.UserProductListCompleteModel2> _products = Managers.ProductsManager.GetAllV3(page, query, withoutBarcode, userId);
                if (_products != null)
                    return Request.CreateResponse(HttpStatusCode.OK, _products);
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> Post([FromBody] ProductItemNew product)
        {
            Managers.InteractionsManager.Add(product.UserId, "api/ProductsApi/Post", new JavaScriptSerializer().Serialize(product));
            //TODO - make badRequest response

            int _userProductId = -1;

            //LEGACY PUPRPOSES
            ProductItemCreate _ProductItemCreate = new ProductItemCreate
            {
                Barcode = product.Barcode,
                FirstAddedProductFromStoreId = product.FirstAddedProductFromStoreId,
                Lists = product.Lists,
                UserId = product.UserId,
                SelectedResults = product.SelectedResults
            };
            //_userProductId = await Managers.ProductsManager.Create(product);
            _userProductId = await Managers.ProductsManager.CreateV2(_ProductItemCreate);

            if (_userProductId == -1)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "");

            return Request.CreateResponse(HttpStatusCode.Created, _userProductId);
        }

        // POST: api/Products/PostV2
        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> PostV2([FromBody] ProductItemCreate product)
        {
            Managers.InteractionsManager.Add(product.UserId, "api/ProductsApi/Post", new JavaScriptSerializer().Serialize(product));
            //TODO - make badRequest response

            int _userProductId = -1;

            _userProductId = await Managers.ProductsManager.CreateV2(product);

            if (_userProductId == -1)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "");

            return Request.CreateResponse(HttpStatusCode.Created, _userProductId);
        }

        // POST: api/Products/PostV3
        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> PostV3([FromBody] ProductItemCreate product)
        {
            Managers.InteractionsManager.Add(product.UserId, "api/ProductsApi/Post", new JavaScriptSerializer().Serialize(product));
            //TODO - make badRequest response

            int _userProductId = -1;

            _userProductId = await Managers.ProductsManager.CreateV2(product);

            if (_userProductId == -1)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "");

            var _userProductCompleteModel = Managers.UserListsManager.GetCompleteModel(_userProductId);
            return Request.CreateResponse(HttpStatusCode.Created, _userProductCompleteModel);
        }

        // POST: api/Products/PostV3
        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> PostV4([FromBody] ProductItemCreate product)
        {
            Managers.InteractionsManager.Add(product.UserId, "api/ProductsApi/Post", new JavaScriptSerializer().Serialize(product));
            //TODO - make badRequest response

            int _userProductId = -1;

            _userProductId = await Managers.ProductsManager.CreateV3(product);

            if (_userProductId == -1)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "");

            var _userProductCompleteModel = Managers.UserListsManager.GetCompleteModel(_userProductId);
            return Request.CreateResponse(HttpStatusCode.Created, _userProductCompleteModel);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> PostV5([FromBody] ProductItemCreate product)
        {
            Managers.InteractionsManager.Add(product.UserId, "api/ProductsApi/Post", new JavaScriptSerializer().Serialize(product));
            //TODO - make badRequest response

            var _response = await Managers.ProductsManager.CreateV4(product);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> UpdateStores(string userId, [FromBody] ProductItemNew product)
        {
            Managers.InteractionsManager.Add(product.UserId, "api/Products/UpdateStores", new JavaScriptSerializer().Serialize(product));
            //TODO - make badRequest response

            int _storesAffected = await Managers.ProductsManager.UpdateStores(userId, product);
            if (_storesAffected == -1)
                return Request.CreateResponse(HttpStatusCode.InternalServerError);

            return Request.CreateResponse(HttpStatusCode.OK, _storesAffected);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> UpdateStoresV2(string userId, [FromBody] ProductItemNew product)
        {
            Managers.InteractionsManager.Add(product.UserId, "api/Products/UpdateStoresV2", new JavaScriptSerializer().Serialize(product));
            //TODO - make badRequest response

            int _storesAffected = await Managers.ProductsManager.UpdateStores(userId, product);
            if (_storesAffected == -1)
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            else if (_storesAffected > 0)
            {
                var _product = Managers.ProductsManager.GetDTOById(product.ProductId);
                return Request.CreateResponse(HttpStatusCode.OK, _product);
            }

            return Request.CreateResponse(HttpStatusCode.OK, _storesAffected);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> UpdateStoresV3(string userId, [FromBody] ProductItemNew product)
        {
            Managers.InteractionsManager.Add(product.UserId, "api/Products/UpdateStoresV3", new JavaScriptSerializer().Serialize(product));
            //TODO - make badRequest response

            JsonApiResponse _response = await Managers.ProductsManager.UpdateStoresV2(userId, product);

            if (_response.Success)
            {
                var _product = Managers.ProductsManager.GetDTOById(product.ProductId);
                return Request.CreateResponse(HttpStatusCode.OK, new JsonApiResponse
                {
                    Code = 1,
                    Success = true,
                    Data = _product,
                    Message = _response.Data + " stores updated"
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }


        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> UpdateMetadata(int productId, string userId)
        {
            if (productId != -1)
            {
                try
                {
                    Products _newMetadata = await Managers.ProductsManager.UpdateMetadata(productId, userId);
                    if (_newMetadata != null)
                        return Request.CreateResponse(HttpStatusCode.OK, _newMetadata);
                    return Request.CreateResponse(HttpStatusCode.NotFound, _newMetadata);
                }
                catch (Exception ex)
                {
                    Logger.Debug("Error:" + ex.InnerException.Message);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);

        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> UpdatePrices(int productId)
        {
            if (productId != -1)
            {
                try
                {
                    //var (productPricesUpdates, details) = await Managers.ProductsManager.UpdatePrices(productId);
                    var (productPricesUpdates, details) = await Managers.ProductsManager.UpdatePricesNew(productId);
                    int _index = 0;
                    foreach (var _productPricesUpdate in productPricesUpdates)
                    {
                        //Send only to user products watchers, and only if new price is lower
                        if (Math.Round(_productPricesUpdate.NewPrice, 2) < Math.Round(_productPricesUpdate.OldPrice, 2))
                        {
                            //List<string> _uersIdsWithProductInList = Managers.UserListsManager.GetUsersIdsWithProductInList(productId, "shoppingList");
                            List<string> _uersIdsWithProductInList = Managers.ProductsWatchersManager.GetUserIdsWithProductWatcher(productId);

                            //TODO - only send before 11pm, and after 11am
                            foreach (var _userId in _uersIdsWithProductInList)
                            {
                                Helpers.FirebaseAndroid.SendNotification(_userId, "productsPricesUpdated:" + details[_index] + ";" + productId);
                            }
                        }

                        _index++;
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, productPricesUpdates);
                }
                catch (Exception ex)
                {
                    Logger.Debug("Error:" + ex.InnerException.Message);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);

        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> UpdatePrices(int storeId, string onlineProductId = "", string url = "")
        {
            if (storeId != -1)
            {
                try
                {
                    int _foundProductId = -1;
                    if (!string.IsNullOrEmpty(onlineProductId))
                    {
                        var _product = Managers.ProductsManager.GetByStoreIdAndOnlineProductId(storeId, onlineProductId);
                        if (_product != null)
                        {
                            _foundProductId = _product.Id;
                        }
                    }
                    if (!string.IsNullOrEmpty(url))
                    {
                        var _product = Managers.ProductsManager.GetByStoreIdAndUrl(storeId, url);
                        if (_product != null)
                        {
                            _foundProductId = _product.Id;
                        }
                    }
                    if (_foundProductId != -1)
                    {
                        var (productPricesUpdates, details) = await Managers.ProductsManager.UpdatePricesNew(_foundProductId);
                        int _index = 0;
                        foreach (var _productPricesUpdate in productPricesUpdates)
                        {
                            //Send only to user products watchers, and only if new price is lower
                            if (Math.Round(_productPricesUpdate.NewPrice, 2) < Math.Round(_productPricesUpdate.OldPrice, 2))
                            {
                                //List<string> _uersIdsWithProductInList = Managers.UserListsManager.GetUsersIdsWithProductInList(productId, "shoppingList");
                                List<string> _uersIdsWithProductInList = Managers.ProductsWatchersManager.GetUserIdsWithProductWatcher(_foundProductId);

                                //TODO - only send before 11pm, and after 11am
                                foreach (var _userId in _uersIdsWithProductInList)
                                {
                                    Helpers.FirebaseAndroid.SendNotification(_userId, "productsPricesUpdated:" + details[_index] + ";" + _foundProductId);
                                }
                            }

                            _index++;
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, productPricesUpdates);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    //var (productPricesUpdates, details) = await Managers.ProductsManager.UpdatePrices(productId);

                }
                catch (Exception ex)
                {
                    Logger.Debug("Error:" + ex.InnerException.Message);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);

        }

        //[HttpGet]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        //public async Task<HttpResponseMessage> UpdatePrices2(int productId)
        //{
        //    if (productId != -1)
        //    {
        //        try
        //        {
        //            //var (productPricesUpdates, details) = await Managers.ProductsManager.UpdatePrices(productId);
        //            var (productPricesUpdates, details) = await Managers.ProductsManager.UpdatePricesNew2(productId);
        //            int _index = 0;
        //            foreach (var _productPricesUpdate in productPricesUpdates)
        //            {
        //                //Send only to user products watchers, and only if new price is lower
        //                if (Math.Round(_productPricesUpdate.NewPrice, 2) < Math.Round(_productPricesUpdate.OldPrice, 2))
        //                {
        //                    //List<string> _uersIdsWithProductInList = Managers.UserListsManager.GetUsersIdsWithProductInList(productId, "shoppingList");
        //                    List<string> _uersIdsWithProductInList = Managers.ProductsWatchersManager.GetUserIdsWithProductWatcher(productId);

        //                    //TODO - only send before 11pm, and after 11am
        //                    foreach (var _userId in _uersIdsWithProductInList)
        //                    {
        //                        Helpers.FirebaseAndroid.SendNotification(_userId, "productsPricesUpdated:" + details[_index] + ";" + productId);
        //                    }
        //                }

        //                _index++;
        //            }
        //            return Request.CreateResponse(HttpStatusCode.OK, productPricesUpdates);
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Debug("Error:" + ex.InnerException.Message);
        //            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        //        }

        //    }
        //    else
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);

        //}

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage PostRecognizedIoT(string userId, string productRecognized)
        {
            Managers.InteractionsManager.Add(userId, "api/ProductsApi/Post", new JavaScriptSerializer().Serialize(productRecognized));
            Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");
            string _userProductSimpleName = string.Empty;
            try
            {
                _userProductSimpleName = Managers.ProductsManager.AddProductRecognizedIoTToUserList(userId, productRecognized);
                string _vocalResponse = _userProductSimpleName + " adicionado à lista de compras";
                return Request.CreateResponse(HttpStatusCode.Created, _vocalResponse);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.InnerException.Message);
                string _vocalResponse = "ocorreu um erro a tentar adicionar o produto reconheçido";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, _vocalResponse);
            }
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage PostSimple([FromBody] ProductSimpleItem productSimple)
        {
            Managers.InteractionsManager.Add(productSimple.UserId, "api/ProductsApi/Post", new JavaScriptSerializer().Serialize(productSimple));
            Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");
            int _userProductSimpleId = -1;
            try
            {
                _userProductSimpleId = Managers.ProductsManager.AddProductSimpleToUserList(productSimple);
                if (_userProductSimpleId == -1)
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, -1);
                else
                    return Request.CreateResponse(HttpStatusCode.Created, _userProductSimpleId);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.InnerException.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> GetUpdateStores(int productId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                ProductItemNew _productItem = new ProductItemNew();
                _productItem.ProductId = productId;

                //get Product info
                Products _product = null;

                if (productId != -1)
                    _product = Managers.ProductsManager.GetById(productId);

                if (_product != null)
                {
                    //set main product info
                    _productItem.Name = _product.Name;
                    _productItem.Brand = _product.Brand;
                    _productItem.Weight = _product.Weight;
                    _productItem.ImageUrl = "https://lisie.app/handlers/GetProductImage.ashx?productId=" + _product.Id.ToString();


                    //Update store prices and state (found or not found)
                    await Managers.ProductsManager.UpdatePricesNew(productId);

                    OnlineProducts _OnlineProducts = new OnlineProducts();

                    //get product PricesList to add to SelectedResults
                    var _productStores = from m in db.StoreProducts where m.ProductId == productId select m;
                    if (_productStores.Count() > 0)
                    {
                        _productItem.SelectedResults = new List<LisieStores.Extensibility.ProductSearchResult>();
                        _productItem.SearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();
                        _productItem.StoreProducts = _productStores.Select(c => new Models.StoreProduct
                        {
                            Id = c.Id,
                            CreatedByUserId = c.UserId,
                            NeedsUpdate = c.NeedsUpdate.HasValue ? c.NeedsUpdate.Value : false,
                            Price = c.Price.Value,
                            StoreId = c.StoreId,
                            Url = c.Url,
                            OnlineProductId = c.OnlineProductId

                        }).ToList();

                        List<LisieStores.Extensibility.Market> _stores = Helpers.Extensibility.GetStoreFetchers();
                        foreach (LisieStores.Extensibility.Market _market in _stores.OrderBy(c => c.StoreId))
                        {
                            StoreProducts _currentStoreProduct = _productStores.Where(c => c.StoreId == _market.StoreId).FirstOrDefault();
                            if (_currentStoreProduct != null)
                            {
                                LisieStores.Extensibility.ProductSearchResult _ProductResult =
                                    new LisieStores.Extensibility.ProductSearchResult
                                    {
                                        Barcode = _product.Barcode,
                                        Brand = _product.Brand,
                                        Category = _product.CategoryString,
                                        ImageUrl = "https://lisie.app/handlers/GetProductImage.ashx?productId=" + _product.Id.ToString(),
                                        IsSeperator = false,
                                        Name = _product.Name,
                                        StoreName = _market.StoreName,
                                        StoreId = _market.StoreId,
                                        StoreColor = _market.StoreColor,
                                        Url = _currentStoreProduct.Url,
                                        ViewableUrl = _currentStoreProduct.Url,
                                        Weight = _product.Weight,
                                        Price = Math.Round(_currentStoreProduct.Price.Value, 2).ToString(),
                                        OnlineProductId = _currentStoreProduct.OnlineProductId,
                                        StoreProductId = _currentStoreProduct.OnlineProductId
                                    };
                                //Auchan specific viewableUrl
                                //if (_market.StoreId == 1)
                                //{
                                //    _ProductResult.Url = (_ProductResult.Url.IndexOf("?sid=") == -1) ? _ProductResult.Url : _ProductResult.Url.Substring(0, _ProductResult.Url.IndexOf("?sid="));
                                //    _ProductResult.ViewableUrl = (_ProductResult.Url.IndexOf("/Auchan_") == -1) ? _ProductResult.Url + "/Auchan_Amadora" : _ProductResult.Url;
                                //}
                                //PingoDoce specific viewableUrl
                                if (_market.StoreId == 3)
                                {
                                    _ProductResult.ViewableUrl = "/store/pingo-doce/product" + _ProductResult.Url.Substring(_ProductResult.Url.LastIndexOf("/"));
                                }
                                if (_currentStoreProduct.NeedsUpdate == true)
                                {
                                    _ProductResult.SeparatorTitle = "NotFound";

                                    //if product not found add search results
                                    try
                                    {
                                        _productItem.SearchResults.AddRange(await _OnlineProducts.GetSearchResultsWithNoSeparatorsOfStore((!string.IsNullOrEmpty(_product.Brand) ? _product.Brand : _product.Name), _market.StoreId));
                                    }
                                    catch (Exception ex)
                                    {
                                        string stop = "";
                                    }
                                }

                                _ProductResult.Price = _ProductResult.Price.Replace(" ", "");
                                //if (_ProductResult.StoreId == 1)
                                //{
                                //    _ProductResult.ViewableUrl = _ProductResult.Url + "/Auchan_Amadora";
                                //}
                                _productItem.SelectedResults.Add(_ProductResult);


                            }
                            else //if store product dosen´t exists add store online search results
                            {
                                _productItem.SearchResults.AddRange(await _OnlineProducts.GetSearchResultsWithNoSeparatorsOfStore((!string.IsNullOrEmpty(_product.Brand) ? _product.Brand : _product.Name), _market.StoreId));
                            }
                        }
                    }
                }
                _productItem.StoreIdsToRemove = new List<int>();
                return Request.CreateResponse(HttpStatusCode.OK, _productItem);
            }
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> GetUpdateStoresV2(int productId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                ProductItemNew _productItem = new ProductItemNew();
                _productItem.ProductId = productId;

                //get Product info
                Products _product = null;

                if (productId != -1)
                    _product = Managers.ProductsManager.GetById(productId);

                if (_product != null)
                {
                    //set main product info
                    _productItem.Name = _product.Name;
                    _productItem.Brand = _product.Brand;
                    _productItem.Weight = _product.Weight;
                    _productItem.ImageUrl = "https://lisie.app/handlers/GetProductImage.ashx?productId=" + _product.Id.ToString();
                    _productItem.StoreProducts = new List<StoreProduct>();
                    _productItem.SearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();
                    _productItem.StoreIdsToRemove = new List<int>();

                    OnlineProducts _OnlineProducts = new OnlineProducts();

                    List<LisieStores.Extensibility.Market> _stores = Helpers.Extensibility.GetStoreFetchers();
                    //Itenerate all Markets
                    foreach (LisieStores.Extensibility.Market _market in _stores.OrderBy(c => c.StoreId))
                    {
                        //check if StoreProduct exists
                        var _productOfStore = (from m in db.StoreProducts where m.ProductId == productId && m.StoreId == _market.StoreId select m).FirstOrDefault();
                        if (_productOfStore != null)
                        {
                            //get local and online info also
                            var _storeOnlineProduct = await _OnlineProducts.GetProductMetadata(_market.StoreId, _productOfStore.Url);
                            var _needsUpdate = _productOfStore.NeedsUpdate.HasValue ? _productOfStore.NeedsUpdate.Value : false;
                            //if no online result put needsUpdate = true
                            if (_storeOnlineProduct == null) _needsUpdate = true;
                            _productItem.StoreProducts.Add(new Models.StoreProduct
                            {
                                Id = _productOfStore.Id,
                                CreatedByUserId = _productOfStore.UserId,
                                NeedsUpdate = _needsUpdate,
                                Price = _productOfStore.Price.Value,
                                StoreId = _productOfStore.StoreId,
                                Url = _productOfStore.Url,
                                OnlineProductId = _productOfStore.OnlineProductId,
                                UpdateDate = _productOfStore.UpdateDate ?? DateTime.MinValue,
                                Name = _storeOnlineProduct?.Name ?? _product.Name,
                                Brand = _storeOnlineProduct?.Brand ?? _product.Brand,
                                ImageUrl = _storeOnlineProduct != null ? _storeOnlineProduct.ImageUrl : "https://lisie.app/handlers/GetProductImage.ashx?productId=" + _product.Id.ToString(),
                                Weight = _storeOnlineProduct?.Weight ?? _product.Weight,
                                IsTemp = _productOfStore.IsTemp.HasValue ? _productOfStore.IsTemp.Value : false
                            });
                        }
                        //if not found, add online search results
                        else
                        {
                            _productItem.SearchResults.AddRange(await _OnlineProducts.GetSearchResultsWithNoSeparatorsOfStore((!string.IsNullOrEmpty(_product.Brand) ? _product.Brand : _product.Name), _market.StoreId));
                        }

                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, _productItem);
            }
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> UpdatePricesOfBarcodeNull()
        {
            List<int> _productIdsToUpdate = new List<int>();

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _productsWithBarcode0 = db.Products.Where(c => c.Barcode == "0");
                int _count = _productsWithBarcode0.Count();
                //int _counter = 0;
                foreach (var _product in _productsWithBarcode0)
                {
                    _productIdsToUpdate.Add(_product.Id);
                }
            }
            foreach (var _productId in _productIdsToUpdate)
            {
                var _res = await Managers.ProductsManager.UpdatePricesNew(_productId);
                System.Threading.Thread.Sleep(500);
            }
            return Request.CreateResponse(HttpStatusCode.OK, _productIdsToUpdate);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage Test()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "1111");
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetOnlineProductIdsNotFound([FromBody] GetOnlineProductIdsNotFoundModel product)
        {
            List<string> _toReturn = Managers.ProductsManager.GetOnlineProductIdsNotFound(product);
            return Request.CreateResponse(HttpStatusCode.OK, _toReturn);
        }


        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage AddProductReview([FromBody] ProducReviewCreate productReview)
        {
            Managers.InteractionsManager.Add(productReview.UserId, "api/Products/AddProductReview", new JavaScriptSerializer().Serialize(productReview));
            //TODO - make badRequest response

            var _newProductReview = Managers.ProductsManager.AddProductReview(productReview.UserId, productReview.ProductId, productReview.Info);

            if (_newProductReview == null)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "");

            return Request.CreateResponse(HttpStatusCode.Created, _newProductReview);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetDayBestPriceProducts()
        {
            try
            {
                List<KeyValuePair<int, double>> _maxes = Managers.ProductsManager.GetDayBestPriceProducts();
                return Request.CreateResponse(HttpStatusCode.Created, _maxes);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetLastUnUpdatedProductIds(int count, string userId, bool isFromLisieHome)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {

                //first check if user has pending updates to do
                var _productsBeingUpdatedByUser = db.ProductsUpdating.Where(c => c.UserId == userId && c.IsFromLisieHome == isFromLisieHome).Select(c => c.ProductId);
                if (_productsBeingUpdatedByUser.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _productsBeingUpdatedByUser.Select(c => new { ProductId = c }).ToList());
                }

                var _oldestUnUpdatedStoreProducts =
                        from prod in db.Products
                        join storeProduct in db.StoreProducts on prod.Id equals storeProduct.ProductId
                        where storeProduct.UpdateDate != null
                        orderby storeProduct.UpdateDate
                        select new
                        {
                            ProductId = prod.Id
                        };

                var _productIds = _oldestUnUpdatedStoreProducts.DistinctBy(c => c.ProductId).Take(count * 2).ToList();
                var _productsBeingUpdated = db.ProductsUpdating.Select(c => c.ProductId).ToList();

                var _finalPoductIds = _productIds.Where(c => !_productsBeingUpdated.Contains(c.ProductId)).Take(count).ToList();

                //add wich products are updating to db
                var fadd = from field in _finalPoductIds
                           select new ProductsUpdating
                           {
                               UserId = userId,
                               DateRequested = DateTime.Now,
                               IsFromLisieHome = isFromLisieHome,
                               ProductId = field.ProductId
                           };
                db.ProductsUpdating.AddRange(fadd);
                db.SaveChanges();
                //var _finalPoductIds = _oldestUnUpdatedStoreProducts.DistinctBy(c => c.ProductId).Where(c => _productsBeingUpdated.Contains(c.ProductId)).Take(count).ToList();
                //var  = _oldestUnUpdated=StoreProducts.DistinctBy(c => c.ProductId).Select(c=> c).Except(_productsBeingUpdated.Select(c => c);

                //var _productIds = _oldestUnUpdatedStoreProducts.DistinctBy(c => c.ProductId).Take(count).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, _finalPoductIds);
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteProductUpdating(int productId, string userId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _toRemove = db.ProductsUpdating.Where(c => c.ProductId == productId).ToList();
                if (_toRemove.Count > 0)
                {
                    db.ProductsUpdating.RemoveRange(_toRemove);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, false);
                }
            }
        }

        //for databse purposes
        [HttpGet]
        public HttpResponseMessage UpdateProductsWIthOnlyOneStoreAtIsTempTrue()
        {
            //update storeproduct and Product
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _tempProducts = db.Products.Where(c => c.IsTemp.HasValue && c.IsTemp.Value).ToList();
                foreach (var _product in _tempProducts)
                {
                    var _storeProducts = db.StoreProducts.Where(c => c.ProductId == _product.Id).OrderBy(c => c.Id);
                    int _count = _storeProducts.Count();
                    foreach (var _storeProduct in _storeProducts)
                    {
                        _storeProduct.IsTemp = false;
                        break;
                    }
                }
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, true);

            }
        }


        //for databse purposes
        [HttpGet]
        public HttpResponseMessage FixWeightBarcodes()
        {
            //update storeproduct and Product
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _tempProducts = db.Products.Where(c => c.Barcode.Substring(c.Barcode.Length - 6, 5) == "00000").ToList();
                foreach (var _product in _tempProducts)
                {
                    if (_product.Barcode[_product.Barcode.Length - 1] != '0')
                    {
                        string i = "sd";
                    }
                    //var _storeProducts = db.StoreProducts.Where(c => c.ProductId == _product.Id).OrderBy(c => c.Id);
                    //int _count = _storeProducts.Count();
                    //foreach (var _storeProduct in _storeProducts)
                    //{
                    //    _storeProduct.IsTemp = false;
                    //    break;
                    //}
                }
                //db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, true);

            }
        }
    }
}
