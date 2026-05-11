using ClassLibrary1;
using SpiroWeb.Helpers;
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

namespace SpiroWeb.Controllers
{
    /// <summary>
    /// TO GO IN FUTURE TO OBSOLETE- Go to api/ProductsController
    /// </summary>
    public class ProductsApiController : ApiController
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        // GET: api/ProductsApi
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/ProductsApi/5
        //TODO - add userId for interactions
        //OBSOLETE
        //[HttpGet]
        //public HttpResponseMessage Get(int id = -1, string barcode = "")
        //{
        //    Products product;

        //    if (id != -1)
        //        product = Managers.ProductsManager.GetById(id);
        //    else if (barcode != string.Empty)
        //        product = Managers.ProductsManager.GetByBarcode(barcode);
        //    else
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);

        //    if (product == null)
        //        return Request.CreateResponse(HttpStatusCode.NotFound);


        //    var _productDTO = new Products()
        //    {
        //        Id = product.Id,
        //        Barcode = product.Barcode,
        //        Brand = product.Brand,
        //        CategoryString = product.CategoryString,
        //        InsertDate = product.InsertDate,
        //        Name = product.Name,
        //        //Picture = Helpers.Settings.WebURL + "/handlers/getproductImage.ashx?productId=" + product.Id,
        //        Price = product.Price,
        //        VariableWeightPrice = product.VariableWeightPrice,
        //        StoreProducts = GetStoreProductsCopy(product.StoreProducts),
        //        Weight = product.Weight,
        //        AddedByUserId = product.AddedByUserId,
        //        CreatedByUserId = product.CreatedByUserId
        //    };

        //    return Request.CreateResponse(HttpStatusCode.OK, _productDTO);
        //}


        //Legacy, to be absolete. in app now call UserManager.AddProductByBarcode
        [HttpGet]
        public async Task<HttpResponseMessage> Get(int id = -1, string barcode = "")
        {
            Products product;

            if (id != -1)
                product = Managers.ProductsManager.GetById(id);
            else if (barcode != string.Empty)
                product = await Managers.ProductsManager.GetByBarcodeAndSearchOnlineAndAddIfFound(barcode, "9ff8224f-17cf-49fb-b555-05779a13eb40");
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
        public HttpResponseMessage GetAll(int page, string query, bool withoutBarcode = false)
        {
            //Managers.InteractionsManager.Add(userId, "api/ProductsWatchersController/Get", userId);

            try
            {
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
        //TODO - add userId for interactions
        public HttpResponseMessage GetAutocomplete(string search, bool withoutBarcode = false)
        {
            if (search != string.Empty)
            {
                List<Products> _products = Managers.ProductsManager.GetProductsAutocomplete(search, withoutBarcode);
                var matchingAllProductsJson = _products.Select(m => new
                {
                    ProductId = m.Id,
                    Description = m.Name + " " + m.Brand + " " + m.Weight
                });
                return Request.CreateResponse(HttpStatusCode.OK, matchingAllProductsJson);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        //TODO - add userId for interactions
        public HttpResponseMessage AssociateProductsWithBarcode(int productId, string barcode)
        {
            if (productId != -1 && barcode != string.Empty)
            {
                Products _product = Managers.ProductsManager.AssociateProductWithBarcode(productId, barcode);
                if (_product != null)
                    return Request.CreateResponse(HttpStatusCode.OK, _product.Id);
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound, -1);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, -1);
        }


        private List<StoreProducts> GetStoreProductsCopy(ICollection<StoreProducts> storeProducts)
        {
            List<StoreProducts> copy = new List<StoreProducts>();
            foreach (StoreProducts item in storeProducts)
            {
                copy.Add(new StoreProducts
                {
                    Id = item.Id,
                    CreateDate = item.CreateDate,
                    Price = item.Price,
                    ProductId = item.ProductId,
                    StoreId = item.StoreId,
                    Url = item.Url,
                    UserId = item.UserId
                });
            }
            return copy;
        }

        // POST: api/ProductsApi
        public async Task<HttpResponseMessage> Post([FromBody] ProductItem product)
        {
            Managers.InteractionsManager.Add(product.UserId, "api/ProductsApi/Post", new JavaScriptSerializer().Serialize(product));
            //TODO - make badRequest response
            Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");

            Logger.Debug(new JavaScriptSerializer().Serialize(product));


            OnlineProducts _OnlineProducts = new OnlineProducts();
            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();


            int _userProductId = -1;

            try
            {
                foreach (var productResult in product.SelectedResults)
                {
                    switch (productResult.StoreName.ToLower())
                    {
                        case "jumbo":
                            _ProductSearchResults.Add(await _OnlineProducts.GetJumboProductMetadata(productResult.Url));
                            break;
                        case "continente":
                            _ProductSearchResults.Add(await _OnlineProducts.GetContinenteProductMetadata(productResult.Url));
                            break;
                        case "pingo doce":
                            _ProductSearchResults.Add(await _OnlineProducts.GetPingoDoceProductMetadata(productResult.Url));
                            break;
                        default:
                            break;
                    }
                }

                //check if any of the results exists
                if (_ProductSearchResults.Count() > 0)
                {
                    var productsFound = (product.IsToOverwrite && product.ProductId != -1) ?
                        db.Products.Where(c => c.Id == product.ProductId).ToList() :
                        db.Products.Where(c => c.Barcode.Equals(product.Barcode)).ToList();

                    //see if exists product with StoreUrl to autocomplete results

                    Products _newProduct = this.GetOptimizedProductInfo(product, product.SelectedResults);
                    int _newProductId = -1;

                    //no product  found
                    if (productsFound.Count() == 0)
                    {
                        _newProductId = Managers.ProductsManager.AddNewProduct(_newProduct);
                        //WarnMeOfNewProductAdded(product.UserId, _newProduct.Name);
                    }
                    //product with barcode found - update data
                    else
                    {
                        _newProductId = productsFound[0].Id;

                        if (product.IsToOverwrite)
                        {
                            Managers.ProductsManager.DeleteStoreProductsOfProduct(_newProductId);

                            string _AppDataPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/tempimg.temp");
                            Managers.ProductsManager.CopyProduct(productsFound[0], _newProduct, _AppDataPath);
                            db.SaveChanges();
                        }
                    }

                    //Update or create Product Store Info id isToOverwrite?
                    //if (product.IsToOverwrite)
                    //{
                    //}
                    foreach (var productResult in product.SelectedResults)
                    {
                        var _store = db.Stores.Where(c => c.Name.ToLower() == productResult.StoreName.ToLower()).First();

                        StoreProducts _storeProduct = new StoreProducts();
                        if (_store != null) _storeProduct.StoreId = _store.Id;
                        Managers.ProductsManager.CreateOrUpdateStoreProductNew(productResult, _newProductId, product.UserId, _store.Id);
                    }


                    //Add product to different lists

                    foreach (string _list in product.Lists)
                    {
                        _userProductId = Managers.UserListsManager.AddProductToList(_newProductId, _newProduct.Name, _list, 1, null, true, product.UserId);

                    }
                    if (product.Lists.Count == 0)
                        return Request.CreateResponse(HttpStatusCode.Created, -2); //sucess code for only updatedproduct
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.InnerException.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, _userProductId);
            }
            return Request.CreateResponse(HttpStatusCode.Created, _userProductId);
        }

        // POST: api/ProductsApi
        //[HttpGet]
        //[HttpPost]
        //public async Task<HttpResponseMessage> PostNew([FromBody]ProductItemCreate product)
        //{
        //    Managers.InteractionsManager.Add(product.UserId, "api/ProductsApi/Post", new JavaScriptSerializer().Serialize(product));
        //    //TODO - make badRequest response
        //    Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");

        //    //Logger.Debug(new JavaScriptSerializer().Serialize(product));

        //    int _userProductId = await Managers.ProductsManager.Create(product);

        //    if (_userProductId == -1)
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, "");

        //    return Request.CreateResponse(HttpStatusCode.Created, _userProductId);
        //}

        public int PostNewFunc(ProductItemNew product) //returns new userProductId
        {
            int _userProductId = -1;
            try
            {
                //CHeck if product already exists
                var _productFound = (product.IsToOverwrite && product.ProductId != -1) ?
                    db.Products.Where(c => c.Id == product.ProductId).FirstOrDefault() :
                    Managers.ProductsManager.GetByBarcode(product.Barcode);


                //TODO - see if exists product with StoreUrl (AVOID DUPLICATES)
                int _productExistsInStoresId = Managers.ProductsManager.CheckIfProductExistsInStores(product.SelectedResults);
                if (_productExistsInStoresId != -1)
                {
                    return -1;
                }

                //Products _newProduct = this.GetOptimizedProductInfoNew(product, product.SelectedResults);
                Products _newProduct = this.GetProductOfFirstSelectedStoreId(
                    product.FirstAddedProductFromStoreId,
                    product.SelectedResults,
                    (!string.IsNullOrEmpty(product.Barcode) ? product.Barcode : "0"),
                    product.UserId);

                int _newProductId = -1;

                //no product  found
                if (_productFound == null)
                {
                    _newProductId = Managers.ProductsManager.AddNewProduct(_newProduct);
                }
                //product with barcode found - update data
                else
                {
                    _newProductId = _productFound.Id;

                    if (product.IsToOverwrite)
                    {
                        Managers.ProductsManager.DeleteStoreProductsOfProduct(_newProductId);

                        string _AppDataPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/tempimg.temp");
                        Managers.ProductsManager.CopyProduct(_productFound, _newProduct, _AppDataPath);
                        db.SaveChanges();
                    }
                }


                foreach (var productResult in product.SelectedResults)
                {
                    Managers.ProductsManager.CreateOrUpdateStoreProductNew(productResult, _newProductId, product.UserId, productResult.StoreId);
                }


                //Add product to different lists

                foreach (string _list in product.Lists)
                {
                    _userProductId = Managers.UserListsManager.AddProductToList(_newProductId, _newProduct.Name, _list, 1, null, true, product.UserId);

                }
                if (product.Lists.Count == 0)
                    return -2; //sucess code for only updatedproduct
            }
            catch (Exception ex)
            {
                //Logger.Debug(ex.InnerException.Message);
                //return Request.CreateResponse(HttpStatusCode.InternalServerError, _userProductId);
                return -9;

            }
            return _userProductId;
        }

        // POST: api/ProductsApi
        [HttpGet]
        [HttpPost]
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



        public async Task<HttpResponseMessage> Update([FromBody] ProductItem product)
        {
            Managers.InteractionsManager.Add(product.UserId, "api/ProductsApi/Update", new JavaScriptSerializer().Serialize(product));
            //TODO - make badRequest response
            Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");

            Logger.Debug(new JavaScriptSerializer().Serialize(product));


            OnlineProducts _OnlineProducts = new OnlineProducts();
            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();


            try
            {
                //TODO - put to new system
                foreach (var productResult in product.SelectedResults)
                {
                    switch (productResult.StoreName.ToLower())
                    {
                        case "jumbo":
                            _ProductSearchResults.Add(await _OnlineProducts.GetJumboProductMetadata(productResult.Url));
                            break;
                        case "continente":
                            _ProductSearchResults.Add(await _OnlineProducts.GetContinenteProductMetadata(productResult.Url));
                            break;
                        case "pingo doce":
                            _ProductSearchResults.Add(await _OnlineProducts.GetPingoDoceProductMetadata(productResult.Url));
                            break;
                        default:
                            break;
                    }
                }

                //check if any of the results exists
                if (_ProductSearchResults.Count() > 0)
                {
                    var productsFound = db.Products.Where(c => c.Id == product.ProductId).ToList();

                    int _productId = -1;

                    //no product found
                    if (productsFound.Count() != 0)
                    {
                        _productId = productsFound[0].Id;
                        foreach (var productResult in product.SelectedResults)
                        {
                            var _store = db.Stores.Where(c => c.Name.ToLower() == productResult.StoreName.ToLower()).First();
                            if (_store == null) continue;

                            Managers.ProductsManager.CreateOrUpdateStoreProductNew(productResult, _productId, product.UserId, _store.Id, true);
                        }

                        //db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.InnerException.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, product.ProductId);
            }

            return Request.CreateResponse(HttpStatusCode.Created, product.ProductId);
        }

        public async Task<HttpResponseMessage> UpdateNew(string userId, [FromBody] ProductItem product)
        {
            Managers.InteractionsManager.Add(product.UserId, "api/ProductsApi/Update", new JavaScriptSerializer().Serialize(product));
            //TODO - make badRequest response
            Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");

            Logger.Debug(new JavaScriptSerializer().Serialize(product));


            OnlineProducts _OnlineProducts = new OnlineProducts();
            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();

            int storeProductsCreatedOrUpdated = 0;

            try
            {
                //TODO - put to new system
                foreach (var productResult in product.SelectedResults)
                {
                    switch (productResult.StoreName.ToLower())
                    {
                        case "jumbo":
                            _ProductSearchResults.Add(await _OnlineProducts.GetJumboProductMetadata(productResult.Url));
                            break;
                        case "continente":
                            _ProductSearchResults.Add(await _OnlineProducts.GetContinenteProductMetadata(productResult.Url));
                            break;
                        case "pingo doce":
                            _ProductSearchResults.Add(await _OnlineProducts.GetPingoDoceProductMetadata(productResult.Url));
                            break;
                        default:
                            break;
                    }
                }

                //check if any of the results exists
                if (_ProductSearchResults.Count() > 0)
                {
                    var productsFound = db.Products.Where(c => c.Id == product.ProductId).ToList();

                    int _productId = -1;

                    //no product found
                    if (productsFound.Count() != 0)
                    {
                        _productId = productsFound[0].Id;
                        foreach (var productResult in product.SelectedResults)
                        {
                            var _store = db.Stores.Where(c => c.Name.ToLower() == productResult.StoreName.ToLower()).First();
                            if (_store == null) continue;
                            //TODO - for now only create new 
                            //TODO - add useres of type moderator
                            //TODO - only let to update if user is the creator or moderator
                            //if (userId.Equals("d3d48305-4527-49ac-a930-49e4a511af14") || userId.Equals(_store.)
                            //{
                            var sucess = Managers.ProductsManager.CreateOrUpdateStoreProductNew(productResult, _productId, userId, _store.Id, false);
                            if (sucess)
                                storeProductsCreatedOrUpdated++;

                            //}
                            //else //if not permission, only add new prices, don´t ovewrite
                            //{
                            //var sucess = Managers.ProductsManager.CreateOrUpdateStoreProduct(productResult, _productId, product.UserId, _store.Id, true);
                            //}
                        }

                        //db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.InnerException.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, product.ProductId);
            }

            return Request.CreateResponse(HttpStatusCode.OK, storeProductsCreatedOrUpdated);
        }

        //public async Task<HttpResponseMessage> UpdateNew2(string userId, [FromBody]ProductItemNew product)
        //{
        //    Managers.InteractionsManager.Add(product.UserId, "api/ProductsApi/UpdateNew2", new JavaScriptSerializer().Serialize(product));
        //    //TODO - make badRequest response
        //    //Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");

        //    //Logger.Debug(new JavaScriptSerializer().Serialize(product));


        //    using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
        //    {
        //        OnlineProducts _OnlineProducts = new OnlineProducts();
        //        List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();

        //        int storeProductsCreatedOrUpdated = 0;
        //        int storeProductsDeleted = 0;

        //        try
        //        {
        //            //check if any of the results exists
        //            //if (_ProductSearchResults.Count() > 0)
        //            //{
        //            var productFound = db2.Products.Where(c => c.Id == product.ProductId).FirstOrDefault();

        //            int _productId = -1;

        //            //no product found
        //            if (productFound != null)
        //            {
        //                _productId = productFound.Id;
        //                foreach (var productResult in product.SelectedResults)
        //                {
        //                    var sucess = Managers.ProductsManager.CreateOrUpdateStoreProductNew(productResult, _productId, userId, productResult.StoreId, false);
        //                    if (sucess)
        //                        storeProductsCreatedOrUpdated++;
        //                }

        //                //Remove Store Ids
        //                foreach (var _storeIdToRemove in product.StoreIdsToRemove)
        //                {
        //                    var sucess = Managers.ProductsManager.DeleteStoreProductOfProductNew(product.ProductId, userId, _storeIdToRemove);
        //                    storeProductsDeleted++;
        //                }

        //                //db2.SaveChanges();
        //            }
        //            //}
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Debug(ex.InnerException.Message);
        //            return Request.CreateResponse(HttpStatusCode.InternalServerError, product.ProductId);
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK, storeProductsCreatedOrUpdated);
        //    }
        //}
        public async Task<HttpResponseMessage> UpdateNew2(string userId, [FromBody] ProductItemNew product)
        {
            Managers.InteractionsManager.Add(product.UserId, "api/Products/UpdateStores", new JavaScriptSerializer().Serialize(product));
            //TODO - make badRequest response

            int _storesAffected = await Managers.ProductsManager.UpdateStores(userId, product);
            if (_storesAffected == -1)
                return Request.CreateResponse(HttpStatusCode.InternalServerError);

            return Request.CreateResponse(HttpStatusCode.OK, _storesAffected);
        }

        public Products GetOptimizedProductInfo(ProductItem productItem, List<LisieStores.Extensibility.ProductSearchResult> storesSelectedProducts)
        {
            Products _newProduct = new Products();
            //TODO - fix concurrency with filename - add userId to filename?
            string _AppDataPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/tempimg.temp");

            if (productItem.SelectedResults.Count() > 0)
            {
                _newProduct.Barcode = !string.IsNullOrEmpty(productItem.Barcode) ? productItem.Barcode : "0";
                _newProduct.Name = productItem.SelectedResults[0].Name;
                _newProduct.Price = Math.Round(double.Parse(productItem.SelectedResults[0].Price.Replace("€", "").Trim()), 2);
                _newProduct.VariableWeightPrice = productItem.SelectedResults[0].PriceWeight;
                _newProduct.CategoryString = productItem.SelectedResults[0].Category;
                _newProduct.Brand = productItem.SelectedResults[0].Brand;
                _newProduct.Weight = productItem.SelectedResults[0].Weight;
                _newProduct.InsertDate = DateTime.Now;
                _newProduct.CreatedByUserId = productItem.UserId;
                WebClient _client = new WebClient();
                //_client.DownloadFile(new Uri(productItem.SelectedResults[0].ImageUrl), _AppDataPath);
                //byte[] _imageInBase64 = ManageImage.GetBase64OfImagePath(_AppDataPath);
                //_newProduct.Picture = _imageInBase64;
                //System.IO.File.Delete(_AppDataPath);


                bool _jumboStoreExists = productItem.SelectedResults.Exists(c => c.StoreName.ToLower().Equals("jumbo"));
                bool _continenteStoreExists = productItem.SelectedResults.Exists(c => c.StoreName.ToLower().Equals("continente"));
                bool _pingoSoceStoreExists = productItem.SelectedResults.Exists(c => c.StoreName.ToLower().Equals("pingo doce"));
                int _propertyIndex = 0;
                while (true)
                {
                    switch (_propertyIndex)
                    {

                        case 0: //Name
                            if (_continenteStoreExists)
                            {
                                _newProduct.Name = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("continente")).Name;
                            }
                            else if (_pingoSoceStoreExists)
                            {
                                _newProduct.Name = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("pingo doce")).Name;
                            }
                            else if (_jumboStoreExists)
                            {
                                _newProduct.Name = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("jumbo")).Name;
                            }
                            break;
                        case 1: //Image
                            if (_jumboStoreExists)
                            {
                                _client.DownloadFile(new Uri(productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("jumbo")).ImageUrl.Replace("https://", "http://")), _AppDataPath);
                                byte[] _imageInBase64 = ManageImage.GetBase64OfImagePath(_AppDataPath);
                                _newProduct.Picture = _imageInBase64;
                            }
                            else if (_continenteStoreExists)
                            {
                                _client.DownloadFile(new Uri(productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("continente")).ImageUrl), _AppDataPath);
                                byte[] _imageInBase64 = ManageImage.GetBase64OfImagePath(_AppDataPath);
                                _newProduct.Picture = _imageInBase64;
                            }
                            else if (_pingoSoceStoreExists)
                            {
                                _client.DownloadFile(new Uri(productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("pingo doce")).ImageUrl), _AppDataPath);
                                byte[] _imageInBase64 = ManageImage.GetBase64OfImagePath(_AppDataPath);
                                _newProduct.Picture = _imageInBase64;
                            }
                            System.IO.File.Delete(_AppDataPath);
                            break;
                        case 2: //Brand
                            if (_jumboStoreExists)
                            {
                                _newProduct.Brand = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("jumbo")).Brand;
                            }
                            else if (_pingoSoceStoreExists)
                            {
                                _newProduct.Brand = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("pingo doce")).Brand;
                            }
                            else if (_continenteStoreExists)
                            {
                                _newProduct.Brand = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("continente")).Brand;
                            }
                            break;
                        case 3: //Capacity
                            if (_pingoSoceStoreExists)
                            {
                                _newProduct.Weight = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("pingo doce")).Weight;
                            }
                            else if (_continenteStoreExists)
                            {
                                _newProduct.Weight = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("continente")).Weight;
                            }
                            else if (_jumboStoreExists)
                            {
                                _newProduct.Weight = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("jumbo")).Weight;
                            }
                            break;
                        default:
                            break;
                    }

                    _propertyIndex++;
                    if (_propertyIndex == 4) break;
                }
                //TODO - get best data of all stores
                //foreach (var productResult in productItem.SelectedResults)
                //{
                //    switch (productResult.Store.ToLower())
                //    {
                //        case "jumbo":
                //            //image 1st
                //            //name 3rd
                //            //brand - 1st
                //            break;
                //        case "continente":
                //            //image 2nd
                //            //name! 1st (if brand found on name, don´t use)
                //            //capacity - 2st
                //            //brand 3rd
                //            break;
                //        case "pingo doce":
                //            //name 2nd (if brand found on name, don´t use)
                //            //brand - 2nd
                //            //capacity - 1st
                //            break;
                //        default:
                //            break;
                //    }
                //}

                return _newProduct;
            }
            else
            {
                return null;
            }
        }

        //TODO
        public Products GetOptimizedProductInfoNew(ProductItemNew productItem, List<LisieStores.Extensibility.ProductSearchResult> storesSelectedProducts)
        {
            Products _newProduct = new Products();
            //TODO - fix concurrency with filename - add userId to filename?
            string _AppDataPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/tempimg.temp");

            if (productItem.SelectedResults.Count() > 0)
            {
                _newProduct.Barcode = !string.IsNullOrEmpty(productItem.Barcode) ? productItem.Barcode : "0";
                _newProduct.Name = productItem.SelectedResults[0].Name;
                _newProduct.Price = Math.Round(double.Parse(productItem.SelectedResults[0].Price.Replace("€", "").Trim()), 2);
                _newProduct.VariableWeightPrice = productItem.SelectedResults[0].PriceWeight;
                _newProduct.CategoryString = productItem.SelectedResults[0].Category;
                _newProduct.Brand = productItem.SelectedResults[0].Brand;
                _newProduct.Weight = productItem.SelectedResults[0].Weight;
                _newProduct.InsertDate = DateTime.Now;
                _newProduct.CreatedByUserId = productItem.UserId;
                WebClient _client = new WebClient();
                //_client.DownloadFile(new Uri(productItem.SelectedResults[0].ImageUrl), _AppDataPath);
                //byte[] _imageInBase64 = ManageImage.GetBase64OfImagePath(_AppDataPath);
                //_newProduct.Picture = _imageInBase64;
                //System.IO.File.Delete(_AppDataPath);


                bool _jumboStoreExists = productItem.SelectedResults.Exists(c => c.StoreName.ToLower().Equals("jumbo"));
                bool _continenteStoreExists = productItem.SelectedResults.Exists(c => c.StoreName.ToLower().Equals("continente"));
                bool _pingoSoceStoreExists = productItem.SelectedResults.Exists(c => c.StoreName.ToLower().Equals("pingo doce"));
                int _propertyIndex = 0;
                while (true)
                {
                    switch (_propertyIndex)
                    {

                        case 0: //Name
                            if (_continenteStoreExists)
                            {
                                _newProduct.Name = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("continente")).Name;
                            }
                            else if (_pingoSoceStoreExists)
                            {
                                _newProduct.Name = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("pingo doce")).Name;
                            }
                            else if (_jumboStoreExists)
                            {
                                _newProduct.Name = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("jumbo")).Name;
                            }
                            break;
                        case 1: //Image
                            if (_jumboStoreExists)
                            {
                                _client.DownloadFile(new Uri(productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("jumbo")).ImageUrl.Replace("https://", "http://")), _AppDataPath);
                                byte[] _imageInBase64 = ManageImage.GetBase64OfImagePath(_AppDataPath);
                                _newProduct.Picture = _imageInBase64;
                            }
                            else if (_continenteStoreExists)
                            {
                                _client.DownloadFile(new Uri(productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("continente")).ImageUrl), _AppDataPath);
                                byte[] _imageInBase64 = ManageImage.GetBase64OfImagePath(_AppDataPath);
                                _newProduct.Picture = _imageInBase64;
                            }
                            else if (_pingoSoceStoreExists)
                            {
                                _client.DownloadFile(new Uri(productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("pingo doce")).ImageUrl), _AppDataPath);
                                byte[] _imageInBase64 = ManageImage.GetBase64OfImagePath(_AppDataPath);
                                _newProduct.Picture = _imageInBase64;
                            }
                            System.IO.File.Delete(_AppDataPath);
                            break;
                        case 2: //Brand
                            if (_jumboStoreExists)
                            {
                                _newProduct.Brand = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("jumbo")).Brand;
                            }
                            else if (_pingoSoceStoreExists)
                            {
                                _newProduct.Brand = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("pingo doce")).Brand;
                            }
                            else if (_continenteStoreExists)
                            {
                                _newProduct.Brand = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("continente")).Brand;
                            }
                            break;
                        case 3: //Capacity
                            if (_pingoSoceStoreExists)
                            {
                                _newProduct.Weight = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("pingo doce")).Weight;
                            }
                            else if (_continenteStoreExists)
                            {
                                _newProduct.Weight = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("continente")).Weight;
                            }
                            else if (_jumboStoreExists)
                            {
                                _newProduct.Weight = productItem.SelectedResults.FirstOrDefault(c => c.StoreName.ToLower().Equals("jumbo")).Weight;
                            }
                            break;
                        default:
                            break;
                    }

                    _propertyIndex++;
                    if (_propertyIndex == 4) break;
                }
                //TODO - get best data of all stores
                //foreach (var productResult in productItem.SelectedResults)
                //{
                //    switch (productResult.Store.ToLower())
                //    {
                //        case "jumbo":
                //            //image 1st
                //            //name 3rd
                //            //brand - 1st
                //            break;
                //        case "continente":
                //            //image 2nd
                //            //name! 1st (if brand found on name, don´t use)
                //            //capacity - 2st
                //            //brand 3rd
                //            break;
                //        case "pingo doce":
                //            //name 2nd (if brand found on name, don´t use)
                //            //brand - 2nd
                //            //capacity - 1st
                //            break;
                //        default:
                //            break;
                //    }
                //}

                return _newProduct;
            }
            else
            {
                return null;
            }
        }

        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ProductsApi/5
        public void Delete(int id)
        {
        }

        //[HttpGet]
        //public async Task<HttpResponseMessage> UpdatePrices(int productId)
        //{
        //    if (productId != -1)
        //    {
        //        try
        //        {
        //            //var (productPricesUpdates, details) = await Managers.ProductsManager.UpdatePrices(productId);
        //            var (productPricesUpdates, details) = await Managers.ProductsManager.UpdatePricesNew(productId);
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
        public async Task<HttpResponseMessage> GetUpdateStoresProductItem(int productId)
        {
            ProductItem _productItem = new ProductItem();
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
                        Url = c.Url

                    }).ToList();

                    foreach (var storeProduct in _productStores)
                    {

                        LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = null;
                        switch (storeProduct.StoreId)
                        {
                            case 1: //Jumbo
                                //string _storeProductUrl = storeProduct.Url.Remove(storeProduct.Url.IndexOf("?sid="));
                                _ProductSearchResult = await _OnlineProducts.GetJumboProductMetadata(storeProduct.Url);
                                if (_ProductSearchResult == null || _ProductSearchResult.Price == string.Empty)
                                {
                                    _ProductSearchResult = new LisieStores.Extensibility.ProductSearchResult
                                    {
                                        Brand = _product.Brand,
                                        Category = _product.CategoryString,
                                        ImageUrl = "https://lisie.app/handlers/GetProductImage.ashx?productId=" + _product.Id.ToString(),
                                        IsSeperator = false,
                                        Name = _product.Name,
                                        StoreName = storeProduct.Stores.Name,
                                        StoreId = storeProduct.Stores.Id,
                                        Url = storeProduct.Url,
                                        Weight = _product.Weight,
                                        Price = Math.Round(storeProduct.Price.Value, 2).ToString(),
                                        SeparatorTitle = "NotFound"
                                    };
                                }
                                break;
                            case 2: //Continente
                                _ProductSearchResult = await _OnlineProducts.GetContinenteProductMetadata(storeProduct.Url);
                                if (_ProductSearchResult == null || _ProductSearchResult.Price == string.Empty)
                                {
                                    _ProductSearchResult = new LisieStores.Extensibility.ProductSearchResult
                                    {
                                        Brand = _product.Brand,
                                        Category = _product.CategoryString,
                                        ImageUrl = "https://lisie.app/handlers/GetProductImage.ashx?productId=" + _product.Id.ToString(),
                                        IsSeperator = false,
                                        Name = _product.Name,
                                        StoreName = storeProduct.Stores.Name,
                                        StoreId = storeProduct.Stores.Id,
                                        Url = storeProduct.Url,
                                        Weight = _product.Weight,
                                        Price = Math.Round(storeProduct.Price.Value, 2).ToString(),
                                        SeparatorTitle = "NotFound"
                                    };
                                }
                                break;
                            case 3: //Pingo Doce
                                _ProductSearchResult = await _OnlineProducts.GetPingoDoceProductMetadata(storeProduct.Url);
                                if (_ProductSearchResult == null || _ProductSearchResult.Price == string.Empty)
                                {
                                    _ProductSearchResult = new LisieStores.Extensibility.ProductSearchResult
                                    {
                                        Brand = _product.Brand,
                                        Category = _product.CategoryString,
                                        ImageUrl = "https://lisie.app/handlers/GetProductImage.ashx?productId=" + _product.Id.ToString(),
                                        IsSeperator = false,
                                        Name = _product.Name,
                                        StoreName = storeProduct.Stores.Name,
                                        StoreId = storeProduct.Stores.Id,
                                        Url = storeProduct.Url,
                                        Weight = _product.Weight,
                                        Price = Math.Round(storeProduct.Price.Value, 2).ToString(),
                                        SeparatorTitle = "NotFound"
                                    };
                                }
                                break;
                            default:
                                break;
                        }

                        if (_ProductSearchResult != null)
                        {
                            _ProductSearchResult.Price = _ProductSearchResult.Price.Replace(" ", "");
                            _productItem.SelectedResults.Add(_ProductSearchResult);

                            if (_ProductSearchResult.SeparatorTitle != "NotFound")
                            {
                                _productItem.SearchResults.Add(_ProductSearchResult);
                            }
                        }
                    }
                    db.SaveChanges();

                    //Ask to update price 
                    //TODO - remove this update prices because makes a duplicate call to the stores, it done already above
                    var resultUpdatedPrices = Managers.ProductsManager.UpdatePrices(productId);

                    //Go get search results
                    List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();
                    List<LisieStores.Extensibility.ProductSearchResult> _jumboProductSearchResultList = await _OnlineProducts.GetJumboOnlineProductSearchResults(_product.Brand);
                    List<LisieStores.Extensibility.ProductSearchResult> _continenteProductSearchResultList = await _OnlineProducts.GetContinenteOnlineProductSearchResults(_product.Brand);
                    List<LisieStores.Extensibility.ProductSearchResult> _pingoDoceProductSearchResultList = await _OnlineProducts.GetPingoDoceOnlineProductSearchResults(_product.Brand);
                    _productItem.SearchResults.AddRange(_jumboProductSearchResultList);
                    _productItem.SearchResults.AddRange(_continenteProductSearchResultList);
                    _productItem.SearchResults.AddRange(_pingoDoceProductSearchResultList);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, _productItem);
        }

        //OBSOLETE - passes to ProductsController
        [HttpGet]
        public async Task<HttpResponseMessage> GetUpdateStoresProductItemNew(int productId)
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
                                    OnlineProductId = _currentStoreProduct.OnlineProductId
                                };
                            //Auchan specific viewableUrl
                            if (_market.StoreId == 1)
                            {
                                _ProductResult.Url = (_ProductResult.Url.IndexOf("?sid=") == -1) ? _ProductResult.Url : _ProductResult.Url.Substring(0, _ProductResult.Url.IndexOf("?sid="));
                                _ProductResult.ViewableUrl = (_ProductResult.Url.IndexOf("/Auchan_") == -1) ? _ProductResult.Url + "/Auchan_Amadora" : _ProductResult.Url;
                            }
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

        public async Task<List<LisieStores.Extensibility.ProductSearchResult>> GetProductSearchResults(string searchQuery)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();

            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();

            List<LisieStores.Extensibility.ProductSearchResult> _jumboProductSearchResultList = await _OnlineProducts.GetJumboOnlineProductSearchResults(searchQuery);
            List<LisieStores.Extensibility.ProductSearchResult> _continenteProductSearchResultList = await _OnlineProducts.GetContinenteOnlineProductSearchResults(searchQuery);
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

            return _ProductSearchResultList;
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetContinenteProductMetadata(string productUrl)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            var _productMetadata = await _OnlineProducts.GetContinenteProductMetadata(productUrl);
            return Request.CreateResponse(HttpStatusCode.OK, _productMetadata);
        }

        public Products GetProductOfFirstSelectedStoreId(int FirstAddedProductFromStoreId, List<LisieStores.Extensibility.ProductSearchResult> SelectedResults, string barcode, string userId)
        {
            string _AppDataPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/tempimg.temp");

            LisieStores.Extensibility.ProductSearchResult _productSearchResult = SelectedResults.Where(c => c.StoreId == FirstAddedProductFromStoreId).FirstOrDefault();
            if (_productSearchResult != null)
            {
                Products _newProduct = new Products();
                _newProduct.Name = _productSearchResult.Name;
                _newProduct.Price = 0;
                //_newProduct.Price = Math.Round(double.Parse(_productSearchResult.Price.Replace("€", "").Trim()), 2);
                //double _Price = Math.Round(double.Parse(_productSearchResult.Price.Replace("€", "").Trim()), 2);
                //_newProduct.Price = double.Parse(_productSearchResult.Price.Replace("€", "").Trim());
                _newProduct.VariableWeightPrice = _productSearchResult.PriceWeight;
                _newProduct.CategoryString = _productSearchResult.Category;
                _newProduct.FullCategory = _productSearchResult.FullCategory;
                _newProduct.Brand = _productSearchResult.Brand;
                _newProduct.Weight = _productSearchResult.Weight;
                _newProduct.Barcode = barcode;
                _newProduct.CreatedByUserId = userId;
                _newProduct.InsertDate = DateTime.Now;

                //Get image to base64
                WebClient _client = new WebClient();
                _client.DownloadFile(new Uri(_productSearchResult.ImageUrl.Replace("https://", "http://")), _AppDataPath);
                byte[] _imageInBase64 = ManageImage.GetBase64OfImagePath(_AppDataPath);
                _newProduct.Picture = _imageInBase64;

                return _newProduct;
            }
            return null;
        }


        //[HttpGet]
        //[HttpPost]
        //public HttpResponseMessage PostNewIntermarcheGoGetter([FromBody] LisieStores.Extensibility.ProductSearchResult productResult)
        //{
        //    //Managers.InteractionsManager.Add(product.UserId, "api/ProductsApi/Post", new JavaScriptSerializer().Serialize(product));
        //    //TODO - make badRequest response
        //    //Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");

        //    ////Logger.Debug(new JavaScriptSerializer().Serialize(product));
        //    try
        //    {
        //        var _productFound = (!string.IsNullOrEmpty(productResult.Barcode)) ?
        //            db.ProductsNew.Where(c => c.Barcode.Equals(productResult.Barcode)).FirstOrDefault() :
        //            null;

        //        //see if exists product with StoreUrl to autocomplete results
        //        ProductsNew _newProduct = null;
        //        int _newProductId = -1;
        //        if (_productFound != null) //roduct with barcode already exists
        //        {
        //            _newProductId = _productFound.Id;
        //        }
        //        else //is to add new
        //        {
        //            _newProduct = this.GetOptimizedProductInfoNew(productResult, "9ff8224f-17cf-49fb-b555-05779a13eb40");
        //            db.ProductsNew.Add(_newProduct);
        //            db.SaveChanges();
        //            _newProductId = _newProduct.Id;
        //        }

        //        //Update or create Product Store Info id isToOverwrite?
        //        Managers.ProductsManager.CreateOrUpdateStoreProductNewGoGetter(productResult, _newProductId, "9ff8224f-17cf-49fb-b555-05779a13eb40", productResult.StoreId);

        //        return Request.CreateResponse(HttpStatusCode.Created, -2); //sucess code for only updatedproduct
        //    }
        //    catch (Exception ex)
        //    {
        //        //Logger.Debug(ex.InnerException.Message);
        //        //return Request.CreateResponse(HttpStatusCode.InternalServerError, _userProductId);
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);

        //    }
        //}

        [HttpGet]
        public HttpResponseMessage GetLastUnUpdatedProductIds(int count)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                //var _notUpdatedStoreProducts =
                //            from prod in db.Products
                //            join storeProduct in db.StoreProducts on prod.Id equals storeProduct.ProductId
                //            where storeProduct.UpdateDate == null
                //            select new
                //            {
                //                ProductId = prod.Id,
                //            };
                //if (_notUpdatedStoreProducts.Count() > 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.OK, _notUpdatedStoreProducts.DistinctBy(c => c.ProductId).Take(count).ToList());
                //}
                //else //if not UpdateDate == null, go get the oldest UnUpdated ProductIds
                //{
                var _oldestUnUpdatedStoreProducts =
                        from prod in db.Products
                        join storeProduct in db.StoreProducts on prod.Id equals storeProduct.ProductId
                        where storeProduct.UpdateDate != null
                        orderby storeProduct.UpdateDate
                        select new
                        {
                            ProductId = prod.Id
                        };
                return Request.CreateResponse(HttpStatusCode.OK, _oldestUnUpdatedStoreProducts.DistinctBy(c => c.ProductId).Take(count).ToList());
                //}
            }

            //try
            //{
            //    var json = new JavaScriptSerializer().Serialize(_list);
            //    string response = cli.UploadString("https://puppeteer-lisie.herokuapp.com/updateUsersShoppingListProductsPrices", json);
            //    //string response = cli.UploadString("http://localhost:3000/updateUsersShoppingListProductsPrices", json);
            //    return Request.CreateResponse(HttpStatusCode.OK, _list);
            //}
            //catch (Exception ex)
            //{
            //    Logger.Debug("Error:" + ex.InnerException.Message);
            //    return Request.CreateResponse(HttpStatusCode.InternalServerError);
            //}

        }

        [HttpGet]
        public HttpResponseMessage FixMiniPrecoImageUrl()
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _toFix = db.StoreProducts.Where(c => c.StoreId == 5 && c.ImageUrl.StartsWith("https://wwhttps://"));
                int _Count = _toFix.Count();
                foreach (var _strPrd in _toFix)
                {
                    _strPrd.ImageUrl = _strPrd.ImageUrl.Replace("https://wwhttps://", "https://");
                    db.Entry(_strPrd).State = System.Data.Entity.EntityState.Modified;

                }
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, _Count);
            }
        }
    }
}
