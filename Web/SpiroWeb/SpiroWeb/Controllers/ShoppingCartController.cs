using ClassLibrary1;
using Microsoft.AspNet.Identity;
using SpiroWeb.Helpers;
using SpiroWeb.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SpiroWeb.Controllers
{
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
            base.OnActionExecuting(filterContext);
        }
    }

    public class ShoppingCartController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        // GET: ShoppingCart
        [Authorize]
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            var userProductsList = db.UserProductsList.Include(u => u.Products).Include(u => u.AspNetUsers).Where(u => u.UserId.Equals(userId));


            var _productsInQueue = (from c in db.UserProductsQueue
                                    where c.UserId.Equals(userId) &&
                                    c.ListName.Equals("In")
                                    select c);
            ViewBag.ProductsInQueueCount = _productsInQueue.Count();
            return View(userProductsList.ToList());
        }

        public ActionResult Dummy()
        {
            return View();
        }

        // GET: ShoppingCart/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProductsList userProductsList = db.UserProductsList.Find(id);
            if (userProductsList == null)
            {
                return HttpNotFound();
            }
            return View(userProductsList);
        }

        // GET: ShoppingCart/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            return View();
        }

        // POST: ShoppingCart/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProductId,UserId,ListName,Quantity,QuantityWeight")] UserProductsList userProductsList)
        {
            if (ModelState.IsValid)
            {
                db.UserProductsList.Add(userProductsList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", userProductsList.UserId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", userProductsList.ProductId);
            return View(userProductsList);
        }

        // GET: ShoppingCart/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProductsList userProductsList = db.UserProductsList.Find(id);
            if (userProductsList == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", userProductsList.UserId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", userProductsList.ProductId);
            return View(userProductsList);
        }

        // POST: ShoppingCart/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductId,UserId,ListName,Quantity,QuantityWeight")] UserProductsList userProductsList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userProductsList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", userProductsList.UserId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", userProductsList.ProductId);
            return View(userProductsList);
        }

        // GET: ShoppingCart/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProductsList userProductsList = db.UserProductsList.Find(id);
            if (userProductsList == null)
            {
                return HttpNotFound();
            }
            return View(userProductsList);
        }

        // POST: ShoppingCart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserProductsList userProductsList = db.UserProductsList.Find(id);
            db.UserProductsList.Remove(userProductsList);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public int AddProductToShoppingCart(int productId, int quantity, decimal? quantityWeight, bool addToHistory, string refUserId = "")
        {
            string userId = (!string.IsNullOrEmpty(refUserId)) ? refUserId : User.Identity.GetUserId();
            int _userListProductId = -1;
            //check if already exists in a user product list
            UserProductsList queryExistsInUserList = (from c in db.UserProductsList
                                                      where c.ProductId.Equals(productId) &&
                                                      c.UserId.Equals(userId) &&
                                                      c.ListName.Equals("In")
                                                      select c).FirstOrDefault();

            //Exist in User Lists , change quantity
            if (queryExistsInUserList != null)
            {
                //int newQuantity = queryExistsInUserList.Quantity + 1;
                queryExistsInUserList.Quantity = queryExistsInUserList.Quantity + quantity;
                db.UserProductsList.Attach(queryExistsInUserList);
                var entry = db.Entry(queryExistsInUserList);
                entry.Property(y => y.Quantity).IsModified = true;
                // other changed properties

                db.SaveChanges();
                _userListProductId = queryExistsInUserList.Id;
            }
            //add new product to user In List
            else
            {
                UserProductsList _UserProductsList = new UserProductsList();
                _UserProductsList.ProductId = productId;
                _UserProductsList.UserId = userId;
                _UserProductsList.Quantity = 1;
                _UserProductsList.ListName = "In";

                db.UserProductsList.Add(_UserProductsList);
                db.SaveChanges();
                _userListProductId = _UserProductsList.Id;
            }

            if (addToHistory)
            {
                //Add to History
                UserProductsListHistory _UserProductsListHistory = new UserProductsListHistory();
                _UserProductsListHistory.ProductId = productId;
                _UserProductsListHistory.UserId = userId;
                _UserProductsListHistory.Quantity = quantity;
                _UserProductsListHistory.ListName = "shoppingList";
                _UserProductsListHistory.InsertDate = DateTime.Now;
                db.UserProductsListHistory.Add(_UserProductsListHistory);
                db.SaveChanges();
            }


            return _userListProductId;
        }

        public int AddProductToShoppingCartGET(int productId, int quantity, decimal? quantityWeight, bool addToHistory, string userId)
        {
            //string userId = User.Identity.GetUserId();
            int _userListProductId = -1;
            //check if already exists in a user product list
            UserProductsList queryExistsInUserList = (from c in db.UserProductsList
                                                      where c.ProductId.Equals(productId) &&
                                                      c.UserId.Equals(userId) &&
                                                      c.ListName.Equals("In")
                                                      select c).FirstOrDefault();

            //Exist in User Lists , change quantity
            if (queryExistsInUserList != null)
            {
                //int newQuantity = queryExistsInUserList.Quantity + 1;
                queryExistsInUserList.Quantity = queryExistsInUserList.Quantity + quantity;
                queryExistsInUserList.LastAddedDate = DateTime.Now;
                db.UserProductsList.Attach(queryExistsInUserList);
                var entry = db.Entry(queryExistsInUserList);
                entry.Property(y => y.Quantity).IsModified = true;
                entry.Property(y => y.LastAddedDate).IsModified = true;
                // other changed properties

                db.SaveChanges();
                _userListProductId = queryExistsInUserList.Id;
            }
            //add new product to user In List
            else
            {
                UserProductsList _UserProductsList = new UserProductsList();
                _UserProductsList.ProductId = productId;
                _UserProductsList.UserId = userId;
                _UserProductsList.Quantity = quantity;
                _UserProductsList.ListName = "In";
                _UserProductsList.LastAddedDate = DateTime.Now;

                db.UserProductsList.Add(_UserProductsList);
                db.SaveChanges();
                _userListProductId = _UserProductsList.Id;
            }

            if (addToHistory)
            {
                //Add to History
                UserProductsListHistory _UserProductsListHistory = new UserProductsListHistory();
                _UserProductsListHistory.ProductId = productId;
                _UserProductsListHistory.UserId = userId;
                _UserProductsListHistory.Quantity = quantity;
                _UserProductsListHistory.ListName = "shoppingList";
                _UserProductsListHistory.InsertDate = DateTime.Now;
                db.UserProductsListHistory.Add(_UserProductsListHistory);
                db.SaveChanges();
            }

            Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCart(_userListProductId, userId);
            return _userListProductId;
        }

        #region Associate Product To Bar Code Popup WebServices

        [HttpPost]
        public JsonResult AddProductToShoppingCart(int productId, bool addToHistory)
        {
            int _userProductListId = AddProductToShoppingCart(productId, 1, null, addToHistory);
            return Json(_userProductListId, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AddProductToShoppingCartGET(int productId, bool addToHistory, string userId)
        {
            int _userProductListId = AddProductToShoppingCartGET(productId, 1, null, addToHistory, userId);
            return Json(_userProductListId, JsonRequestBehavior.AllowGet);
        }

        //1st Step
        public JsonResult GetProductsQueueIndex()
        {
            string userId = User.Identity.GetUserId();
            List<UserProductsQueue> _UserProductsQueueList = (from c in db.UserProductsQueue
                                                              where c.UserId.Equals(userId) &&
                                                              c.ListName.Equals("In")
                                                              select c).ToList();

            if (_UserProductsQueueList.Count == 1)
            {
                string _htmlToReturn = Helpers.RenderMvcView.GetRazorViewAsString(_UserProductsQueueList[0].BarCode.ToString(), "~/Views/ShoppingCart/_productQueueSearch.cshtml");
                return Json(_htmlToReturn, JsonRequestBehavior.AllowGet);
            }

            if (_UserProductsQueueList.Count > 0)
            {
                string _htmlToReturn = Helpers.RenderMvcView.GetRazorViewAsString(_UserProductsQueueList, "~/Views/ShoppingCart/_productQueueIndex.cshtml");
                return Json(_htmlToReturn, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        //2nd Step
        [HttpPost]
        public JsonResult GetProductsQueueSearchHtml(string barCode)
        {
            string _htmlToReturn = Helpers.RenderMvcView.GetRazorViewAsString(barCode, "~/Views/ShoppingCart/_productQueueSearch.cshtml");
            return Json(_htmlToReturn, JsonRequestBehavior.AllowGet);
        }

        //3rd Step
        [HttpPost]
        public async Task<JsonResult> GetProductsQueueSearchResults(string searchText)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResultList = await _OnlineProducts.GetJumboOnlineProductSearchResults(searchText);


            //get from Continente
            List<LisieStores.Extensibility.ProductSearchResult> _continenteProductSearchResultList = await _OnlineProducts.GetContinenteOnlineProductSearchResults(searchText);


            _ProductSearchResultList.AddRange(_continenteProductSearchResultList);

            //List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResultList = await _OnlineProducts.GetOnlineProductSearchResults(searchText);
            string _htmlToReturn = Helpers.RenderMvcView.GetRazorViewAsString(_ProductSearchResultList, "~/Views/ShoppingCart/_productQueueSearchResults.cshtml");
            //string _htmlToReturn = Helpers.RenderMvcView.GetRazorViewAsString(_ProductSearchResultList, "~/Views/ShoppingCart/_productQueueSearchResults_GridResponsive.cshtml");

            return Json(_htmlToReturn, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetProductsQueueSearchMoreResults(string searchText, int pageNumber)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResultList = await _OnlineProducts.GetJumboOnlineProductSearchResults(searchText, pageNumber);
            string _htmlToReturn = Helpers.RenderMvcView.GetRazorViewAsString(_ProductSearchResultList, "~/Views/ShoppingCart/_productQueueSearchResults.cshtml");
            //string _htmlToReturn = Helpers.RenderMvcView.GetRazorViewAsString(_ProductSearchResultList, "~/Views/ShoppingCart/_productQueueSearchResults_GridResponsive.cshtml");

            return Json(_htmlToReturn, JsonRequestBehavior.AllowGet);
        }



        //4th Step
        [HttpPost]
        public async Task<JsonResult> GetSelectedProductData(string productUrl, string store)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            LisieStores.Extensibility.ProductSearchResult _ProductSearchResultData = new LisieStores.Extensibility.ProductSearchResult();
            switch (store)
            {
                case "jumbo":
                    _ProductSearchResultData = await _OnlineProducts.GetJumboProductMetadata(productUrl);
                    break;
                case "continente":
                    _ProductSearchResultData = await _OnlineProducts.GetContinenteProductMetadata(productUrl);
                    break;
                default:
                    break;
            }
            //Objects.ProductSearchResult _ProductSearchResultData = await _OnlineProducts.GetJumboProductMetadata(productUrl);

            string _htmlToReturn = Helpers.RenderMvcView.GetRazorViewAsString(_ProductSearchResultData, "~/Views/ShoppingCart/_productQueueCommitMetadata.cshtml");

            return Json(_htmlToReturn, JsonRequestBehavior.AllowGet);
        }


        //5th Step
        [HttpPost]
        public async Task<JsonResult> SubmitNewProduct(string productUrl, string barCode)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            LisieStores.Extensibility.ProductSearchResult _ProductSearchResultData = await _OnlineProducts.GetJumboProductMetadata(productUrl);

            if (_ProductSearchResultData != null)
            {
                //Add new product to db
                Products _newProduct = new Products();
                _newProduct.Barcode = (!string.IsNullOrEmpty(barCode)) ? barCode : "0";
                _newProduct.Name = _ProductSearchResultData.Name;
                _newProduct.Price = double.Parse(_ProductSearchResultData.Price.Replace("€", "").Replace(',', '.'));
                _newProduct.VariableWeightPrice = _ProductSearchResultData.PriceWeight;
                _newProduct.CategoryString = _ProductSearchResultData.Category;
                _newProduct.Brand = _ProductSearchResultData.Brand;
                _newProduct.Weight = _ProductSearchResultData.Weight;
                _newProduct.InsertDate = DateTime.Now;

                WebClient _client = new WebClient();
                string _AppDataPath = Server.MapPath("~/App_Data/tempimg.temp");
                _client.DownloadFile(new Uri(_ProductSearchResultData.ImageUrl), _AppDataPath);


                byte[] _imageInBase64 = ManageImage.GetBase64OfImagePath(_AppDataPath);

                _newProduct.Picture = _imageInBase64;

                ProductsController _ProductsController = new ProductsController();
                int _newProductId = _ProductsController.AddNewProduct(_newProduct);

                System.IO.File.Delete(_AppDataPath);

                //delete from productsQueue
                if (!string.IsNullOrEmpty(barCode))
                {
                    string userId = User.Identity.GetUserId();
                    long _parsedBarCode = long.Parse(barCode);
                    var _productsInQueue = (from c in db.UserProductsQueue
                                            where c.BarCode == _parsedBarCode &&
                                            c.UserId.Equals(userId) &&
                                            c.ListName.Equals("In")
                                            select c);

                    foreach (var _productInQueue in _productsInQueue.ToList())
                    {
                        db.UserProductsQueue.Remove(_productInQueue);
                    }
                }

                db.SaveChanges();
            }

            string _htmlToReturn = Helpers.RenderMvcView.GetRazorViewAsString("Product Added to the database. Thank you :) !", "~/Views/ShoppingCart/_productQueueConfirmation.cshtml");

            return Json(_htmlToReturn, JsonRequestBehavior.AllowGet);
        }

        //TODEL - Obsolete
        [HttpPost]
        public async Task<JsonResult> SubmitNewProductAndToShoppingCart(string productUrl, string store, string barCode)
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            //Objects.ProductSearchResult _ProductSearchResultData = await _OnlineProducts.GetJumboProductMetadata(productUrl);
            LisieStores.Extensibility.ProductSearchResult _ProductSearchResultData = null;

            switch (store.ToLower())
            {
                case "jumbo":
                    _ProductSearchResultData = await _OnlineProducts.GetJumboProductMetadata(productUrl);
                    break;
                case "continente":
                    _ProductSearchResultData = await _OnlineProducts.GetContinenteProductMetadata(productUrl);
                    break;
                default:
                    break;
            }

            int _userProductId = -1;

            if (_ProductSearchResultData != null)
            {
                //Add new product to db
                Products _newProduct = new Products();
                _newProduct.Barcode = !string.IsNullOrEmpty(barCode) ? barCode : "0";
                _newProduct.Name = _ProductSearchResultData.Name;
                //_newProduct.Price = double.Parse(_ProductSearchResultData.Price.Replace("€", "").Replace(',', '.'));

                //if (_ProductSearchResultData.PriceLiteral != 0)
                //    _newProduct.Price = _ProductSearchResultData.PriceLiteral;
                //else

                _newProduct.Price = double.Parse(_ProductSearchResultData.Price.Replace("€", "").Trim());

                //_newProduct.Price = float.Parse(_ProductSearchResultData.Price.Replace("€", "").Replace(',', '.'));
                _newProduct.VariableWeightPrice = _ProductSearchResultData.PriceWeight;
                _newProduct.CategoryString = _ProductSearchResultData.Category;
                _newProduct.Brand = _ProductSearchResultData.Brand;
                _newProduct.Weight = _ProductSearchResultData.Weight;
                _newProduct.InsertDate = DateTime.Now;

                WebClient _client = new WebClient();
                string _AppDataPath = Server.MapPath("~/App_Data/tempimg.temp");
                _client.DownloadFile(new Uri(_ProductSearchResultData.ImageUrl), _AppDataPath);


                byte[] _imageInBase64 = ManageImage.GetBase64OfImagePath(_AppDataPath);

                _newProduct.Picture = _imageInBase64;


                //string query = "SELECT COUNT(*) "
                //             + "FROM UserProductsQueue "
                //             + "WHERE BarCode = " + barCode;
                //IEnumerable<int> data = db.Database.SqlQuery<int>(query);



                ProductsController _ProductsController = new ProductsController();
                int _newProductId = _ProductsController.AddNewProduct(_newProduct);
                db.SaveChanges();
                System.IO.File.Delete(_AppDataPath);


                int _quantityToAdd = 1;

                //If barcode != null -  Remove from ProductsQueue Current Barcode
                if (!string.IsNullOrEmpty(barCode))
                {

                    long _parsedBarCode = long.Parse(barCode);
                    string userId = User.Identity.GetUserId();
                    var _productsInQueue = (from c in db.UserProductsQueue
                                            where c.BarCode == _parsedBarCode &&
                                            c.UserId.Equals(userId)
                                            select c);



                    if (_productsInQueue != null)
                        _quantityToAdd = _productsInQueue.Count();

                    //AddProductToShoppingCart(_newProduct.Id, _quantityToAdd, null);

                    foreach (var _productInQueue in _productsInQueue.ToList())
                    {
                        db.UserProductsQueue.Remove(_productInQueue);
                    }
                }
                //else
                //{
                _userProductId = AddProductToShoppingCart(_newProduct.Id, _quantityToAdd, null, true);
                //}

                db.SaveChanges();
                //delete from productsQueue
            }

            //string _htmlToReturn = Helpers.RenderMvcView.GetRazorViewAsString("Product Added to the database and to your shopping cart. Thank you :) !", "~/Views/ShoppingCart/_productQueueConfirmation.cshtml");

            return Json(_userProductId, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //TODEL - Obsolete
        //public async Task<JsonResult> SubmitNewProductAndToShoppingCartGET(string userId, string productUrl, string barCode, string store, int? productId, bool? isToReviewProduct, bool? isToUpdateProduct, bool? isToOverwriteMainProductData)
        public async Task<JsonResult> SubmitProductGet(string product)
        {
            ProductItem _ProductItem = new JavaScriptSerializer().Deserialize<ProductItem>(product);
            return Json(_ProductItem, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //TODEL - Obsolete
        //TODO - IMPORTANT - FIX POST PRODUCTION ENVIROMENT PROBLEM
        //public async Task<JsonResult> SubmitNewProductAndToShoppingCartGET(string userId, string productUrl, string barCode, string store, int? productId, bool? isToReviewProduct, bool? isToUpdateProduct, bool? isToOverwriteMainProductData)
        public async Task<JsonResult> SubmitProduct(string productJson)
        {
            Logger.FolderPath = Server.MapPath("~/Logs");
            Logger.Debug(productJson);

            ProductItem product = new JavaScriptSerializer().Deserialize<ProductItem>(productJson);

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
                        db.Products.Where(c => c.Id.Equals(product.ProductId)).ToList() :
                        db.Products.Where(c => c.Barcode.Equals(product.Barcode)).ToList();

                    //see if exists product with StoreUrl to autocomplete results

                    //Products _newProduct = this.GetOptimizedProductInfo(product);
                    Products _newProduct = new Products();
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

                            string _AppDataPath = Server.MapPath("~/App_Data/tempimg.temp");
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                throw ex;
            }

            return Json(_userProductId, JsonRequestBehavior.AllowGet);
        }

        //public Products GetOptimizedProductInfo(ProductItem productItem)
        //{
        //    Products _newProduct = new Products();
        //    //TODO - fix concurrency with filename - add userId to filename?
        //     string _AppDataPath = Server.MapPath("~/App_Data/tempimg.temp");

        //    if (productItem.SelectedResults.Count() > 0)
        //    {
        //        _newProduct.Barcode = !string.IsNullOrEmpty(productItem.Barcode) ? productItem.Barcode : "0";
        //        _newProduct.Name = productItem.SelectedResults[0].Name;
        //        _newProduct.Price = double.Parse(productItem.SelectedResults[0].Price.Replace("€", "").Trim());
        //        _newProduct.VariableWeightPrice = productItem.SelectedResults[0].PriceWeight;
        //        _newProduct.CategoryString = productItem.SelectedResults[0].Category;
        //        _newProduct.Brand = productItem.SelectedResults[0].Brand;
        //        _newProduct.Weight = productItem.SelectedResults[0].Weight;
        //        _newProduct.InsertDate = DateTime.Now;
        //        _newProduct.CreatedByUserId = productItem.UserId;
        //        WebClient _client = new WebClient();
        //        _client.DownloadFile(new Uri(productItem.SelectedResults[0].ImageUrl), _AppDataPath);
        //        byte[] _imageInBase64 = ManageImage.GetBase64OfImagePath(_AppDataPath);
        //        _newProduct.Picture = _imageInBase64;
        //        System.IO.File.Delete(_AppDataPath);

        //        //TODO - get best data of all stores
        //        foreach (var productResult in productItem.SelectedResults)
        //        {
        //            switch (productResult.Store.ToLower())
        //            {
        //                case "jumbo":
        //                    break;
        //                case "continente":
        //                    break;
        //                case "pingo doce":
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }

        //        return _newProduct;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        [HttpGet]
        public async Task<JsonResult> TestWarnMeOfNewProductAdded(string userId, string productName)
        {
            WarnMeOfNewProductAdded(userId, productName);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public void WarnMeOfNewProductAdded(string userId, string productName)
        {
            //get user last device token

            var user = db.AspNetUsers.Where(c => c.Id.Equals(userId)).First();
            if (user != null)
            {
                DataManager.UserDevicesManager _userDevicesManager2 = new DataManager.UserDevicesManager();
                List<ClassLibrary1.UserDevices> _userDevicesTokens2 = _userDevicesManager2.GetUserDevicesTokens("d3d48305-4527-49ac-a930-49e4a511af14");

                if (_userDevicesTokens2.Count() > 0)
                {
                    foreach (ClassLibrary1.UserDevices _userDevice in _userDevicesTokens2)
                    {
                        Helpers.FirebaseAndroid.SendNotificationToAndroidPhone(_userDevice.DeviceToken, "newProductAdded:" + productName + " added by " + user.Email);
                    }
                }
            }

        }

        //TODO - In client Side Delete TR of the remove button
        public async Task<JsonResult> DeleteProductInQueue(string barCode)
        {
            string userId = User.Identity.GetUserId();
            long _parsedBarCode = long.Parse(barCode);
            List<UserProductsQueue> _UserProductsQueueListToDel = (from c in db.UserProductsQueue
                                                                   where c.UserId.Equals(userId) &&
                                                                   c.BarCode == _parsedBarCode &&
                                                                   c.ListName.Equals("In")
                                                                   select c).ToList();

            foreach (UserProductsQueue _UserProductInQueue in _UserProductsQueueListToDel)
            {
                db.UserProductsQueue.Remove(_UserProductInQueue);
            }
            db.SaveChanges();

            List<UserProductsQueue> _UserProductsQueueList = (from c in db.UserProductsQueue
                                                              where c.UserId.Equals(userId) &&
                                                              c.ListName.Equals("In")
                                                              select c).ToList();

            if (_UserProductsQueueList.Count > 0)
            {
                string _htmlToReturn = Helpers.RenderMvcView.GetRazorViewAsString(_UserProductsQueueList, "~/Views/ShoppingCart/_productQueueIndex.cshtml");
                return Json(_htmlToReturn, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }


        #endregion

        [HttpPost]
        public JsonResult GetShoppingCartListTable()
        {
            string userId = User.Identity.GetUserId();
            var userProductsList = db.UserProductsList.Include(u => u.Products).Include(u => u.AspNetUsers).Where(u => u.UserId.Equals(userId)).Where(u => u.ListName.ToLower().Equals("in"));


            //string _htmlToReturn = Helpers.RenderMvcView.GetRazorViewAsString(userProductsList, "~/Views/ShoppingCart/_shoppingCartList.cshtml");
            string _htmlToReturn = Helpers.RenderMvcView.GetRazorViewAsString(userProductsList, "~/Views/ShoppingCart/_shoppingCartList_GridResponsive.cshtml");
            JsonResult _result = Json(_htmlToReturn, JsonRequestBehavior.AllowGet);
            _result.MaxJsonLength = int.MaxValue;
            //return Json(_htmlToReturn, JsonRequestBehavior.AllowGet, );
            return _result;
        }

        [HttpGet]
        public async Task<JsonResult> GetShoppingCartListOfUser(string userId, string listName, string searchQuery = "")
        {
            Managers.InteractionsManager.Add(userId, "GetShoppingCartListOfUser", "listName:" + listName);
            UserUpdatePricesRequests _UserUpdatePricesRequests = Managers.UserUpdatePricesRequestsManager.Add(userId);

            switch (listName.ToLower())
            {
                case "all":
                    //var userShoppingList = db.UserProductsList.Include(u => u.Products).Include(u => u.AspNetUsers).Where(u => u.UserId.Equals(userId)).Where(u => u.ListName.ToLower().Equals("in"));
                    List<Models.UserProductListCompleteModel> combinedLists = new List<Models.UserProductListCompleteModel>();

                    //TODO - delete because shopping list wont have search?
                    if (searchQuery != null && !string.IsNullOrEmpty(searchQuery))
                    {
                        var userShoppingList = from m in db.UserProductsList where m.UserId == userId && m.ListName.ToLower() == "in" && m.Products.Name.ToLowerInvariant().Contains(searchQuery.ToLowerInvariant()) orderby m.LastAddedDate descending select new SpiroWeb.Models.UserProductListModel { UserId = userId, ListName = "in", ItemType = "shoppingList", ProductId = m.ProductId, Quantity = m.Quantity ?? 1 };

                        var userConsumedProducts = from m in db.UserProductsConsumed where m.UserId == userId && m.ActionTakenByUser == null && m.Products.Name.ToLowerInvariant().Contains(searchQuery.ToLowerInvariant()) select new SpiroWeb.Models.UserProductListModel { UserId = userId, ListName = "consumed", ItemType = "consumed", ProductId = m.ProductId, Quantity = m.Quantity ?? 1 };
                        var userConsumedProductsGrouped = (from m in userConsumedProducts
                                                           group m by new { m.ProductId, m.UserId } into g
                                                           select g);

                        var consumedProductsInnerJoinQuery =
                            from userConsumedProduct in userConsumedProductsGrouped
                            join prod in db.Products on userConsumedProduct.Key.ProductId equals prod.Id
                            select new Models.UserProductListCompleteModel
                            {
                                ProductId = prod.Id,
                                Quantity = userConsumedProduct.Sum(x => x.Quantity),
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "consumed",
                                Name = prod.Name,
                                Category = prod.CategoryString,
                                Price = prod.Price * userConsumedProduct.Sum(x => x.Quantity)
                            };

                        var shoppingListProductsInnerJoinQuery =
                            from userShoppingListProduct in userShoppingList
                            join prod in db.Products on userShoppingListProduct.ProductId equals prod.Id
                            orderby userShoppingListProduct.Id descending
                            select new Models.UserProductListCompleteModel
                            {
                                Id = userShoppingListProduct.Id,
                                ProductId = prod.Id,
                                Quantity = userShoppingListProduct.Quantity,
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "shoppingList",
                                Name = prod.Name,
                                Category = prod.CategoryString,
                                Price = prod.Price
                            };
                        if (consumedProductsInnerJoinQuery.Count() > 0)
                        {
                            Models.UserProductListCompleteModel _UserProductListCompleteModel = new Models.UserProductListCompleteModel { ItemType = "separator", Name = "Consumed List" };
                            //var consumedLisTotalPrice = consumedProductsInnerJoinQuery.Select(g => g.Quantity * g.Price).Sum();
                            var consumedLisTotalPrice = consumedProductsInnerJoinQuery.Select(g => g.Price).Sum();
                            _UserProductListCompleteModel.Price = consumedLisTotalPrice;
                            combinedLists.Add(_UserProductListCompleteModel);
                        }

                        combinedLists.AddRange(consumedProductsInnerJoinQuery);

                        if (shoppingListProductsInnerJoinQuery.Count() > 0)
                        {
                            Models.UserProductListCompleteModel _UserProductListCompleteModel = new Models.UserProductListCompleteModel { ItemType = "separator", Name = "Shopping List" };
                            var shoppingListTotalPrice = shoppingListProductsInnerJoinQuery.Select(g => g.Price).Sum();
                            _UserProductListCompleteModel.Price = shoppingListTotalPrice;
                            combinedLists.Add(_UserProductListCompleteModel);

                        }
                        combinedLists.AddRange(shoppingListProductsInnerJoinQuery);
                    }
                    else
                    {
                        var userShoppingList = from m in db.UserProductsList where m.UserId == userId && m.ListName.ToLower() == "in" select m;
                        var userConsumedProducts = from m in db.UserProductsConsumed where m.UserId == userId && m.ActionTakenByUser == null select new SpiroWeb.Models.UserProductListModel { Id = m.Id, UserId = userId, ListName = "consumed", ItemType = "consumed", ProductId = m.ProductId, Quantity = m.Quantity ?? 1 };
                        var userConsumedProductsGrouped = (from m in userConsumedProducts
                                                           group m by new { m.ProductId, m.UserId } into g
                                                           select g);
                        var consumedProductsInnerJoinQuery =
                            from userConsumedProduct in userConsumedProductsGrouped
                            join prod in db.Products on userConsumedProduct.Key.ProductId equals prod.Id
                            select new Models.UserProductListCompleteModel
                            {
                                ProductId = prod.Id,
                                Quantity = userConsumedProduct.Sum(x => x.Quantity),
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "consumed",
                                Name = prod.Name,
                                Weight = prod.Weight,
                                Category = prod.CategoryString,
                                Price = Math.Round(prod.Price.Value * userConsumedProduct.Sum(x => x.Quantity), 2)
                            };

                        var shoppingListProductsInnerJoinQuery =
                            from userShoppingListProduct in userShoppingList
                            join prod in db.Products on userShoppingListProduct.ProductId equals prod.Id
                            orderby userShoppingListProduct.Id descending
                            select new Models.UserProductListCompleteModel
                            {
                                Id = userShoppingListProduct.Id,
                                ProductId = prod.Id,
                                Quantity = userShoppingListProduct.Quantity ?? 1,
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "shoppingList",
                                Name = prod.Name,
                                Weight = prod.Weight,
                                Category = prod.CategoryString,
                                //Price = prod.Price
                                Price = Math.Round(prod.Price.Value * userShoppingListProduct.Quantity ?? 1, 2)
                            };

                        //TODO - seperators removed with total for now
                        //if (consumedProductsInnerJoinQuery.Count() > 0)
                        //{
                        //    Models.UserProductListCompleteModel _UserProductListCompleteModel = new Models.UserProductListCompleteModel { ItemType = "separator", Name = "Lista de Consumidos" };
                        //    //var consumedLisTotalPrice = consumedProductsInnerJoinQuery.Select(g => g.Quantity * g.Price).Sum();
                        //    var consumedLisTotalPrice = consumedProductsInnerJoinQuery.Select(g => g.Price).Sum();
                        //    _UserProductListCompleteModel.Price = consumedLisTotalPrice;
                        //    combinedLists.Add(_UserProductListCompleteModel);
                        //}

                        if (consumedProductsInnerJoinQuery.Count() > 0)
                        {
                            Models.UserProductListCompleteModel _UserProductListCompleteModelConsumedLegend = new Models.UserProductListCompleteModel
                            {
                                ItemType = "consumedlegend",
                                Name = "Legenda Consumidos"
                            };
                            combinedLists.Add(_UserProductListCompleteModelConsumedLegend);
                        }
                        combinedLists.AddRange(consumedProductsInnerJoinQuery);


                        if (shoppingListProductsInnerJoinQuery.Count() > 0)
                        {

                            Models.UserProductListCompleteModel _UserProductListCompleteModel = new Models.UserProductListCompleteModel
                            {
                                ItemType = "legend",
                                Name = "Legenda"
                            };
                            combinedLists.Add(_UserProductListCompleteModel);
                            //if (shoppingListProductsInnerJoinQuery.Count() > 0)
                            //{
                            //    Models.UserProductListCompleteModel _UserProductListCompleteModel = new Models.UserProductListCompleteModel { ItemType = "separator", Name = "Lista de Compras" };
                            //    //var shoppingListTotalPrice = shoppingListProductsInnerJoinQuery.Select(g => g.Quantity * g.Price).Sum();
                            //    var shoppingListTotalPrice = shoppingListProductsInnerJoinQuery.Select(g => g.Price).Sum();
                            //    _UserProductListCompleteModel.Price = shoppingListTotalPrice;
                            //    combinedLists.Add(_UserProductListCompleteModel);

                            //}
                            combinedLists.AddRange(shoppingListProductsInnerJoinQuery);

                            Models.UserProductListCompleteModel _UserProductListCompleteModelBuyOnline = new Models.UserProductListCompleteModel
                            {
                                ItemType = "buyOnline",
                                Name = "Comprar Online"
                            };
                            combinedLists.Add(_UserProductListCompleteModelBuyOnline);

                            Models.UserProductListCompleteModel _UserProductListCompleteModelCheckout = new Models.UserProductListCompleteModel
                            {
                                ItemType = "checkout",
                                Name = "Confirmar"
                            };
                            combinedLists.Add(_UserProductListCompleteModelCheckout);


                        }

                        Models.UserProductListCompleteModel _UserProductListCompleteEmpty = new Models.UserProductListCompleteModel
                        {
                            ItemType = "empty",
                            Name = "Vazio"
                        };
                        combinedLists.Add(_UserProductListCompleteEmpty);

                    }

                    foreach (var productCombined in combinedLists)
                    {
                        var userShoppingList = from m in db.StoreProducts where m.ProductId == productCombined.ProductId select m;
                        if (userShoppingList.Count() > 0)
                        {
                            foreach (var storeProduct in userShoppingList)
                            {
                                if (productCombined.PriceList == null) productCombined.PriceList = new Dictionary<string, double>();
                                if (!productCombined.PriceList.ContainsKey(storeProduct.StoreId.ToString()))
                                    productCombined.PriceList.Add(storeProduct.StoreId.ToString(), Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2));

                                if (storeProduct.Stores.Name == "Jumbo") productCombined.Url = storeProduct.Url;
                            }
                        }
                    }
                    return Json(combinedLists, JsonRequestBehavior.AllowGet);
                case "shoppinglist":
                    var userProductsList = db.UserProductsList.Include(u => u.Products).Include(u => u.AspNetUsers).Where(u => u.UserId.Equals(userId)).Where(u => u.ListName.ToLower().Equals("in"));
                    List<dynamic> _listToReturn = new List<dynamic>();

                    if (searchQuery != null && !string.IsNullOrEmpty(searchQuery))
                    {
                        foreach (var _product in userProductsList.ToList())
                        {
                            if (_product.Products.Name.ToLowerInvariant().Contains(searchQuery.ToLowerInvariant()))
                                _listToReturn.Add(new
                                {
                                    id = _product.Id,
                                    productId = _product.ProductId,
                                    name = _product.Products.Name,
                                    weight = _product.QuantityWeight.HasValue ? _product.QuantityWeight.Value.ToString() : string.Empty,
                                    quantity = _product.Quantity,
                                    price = _product.Products.Price,
                                    barcode = _product.Products.Barcode,
                                    brand = _product.Products.Brand,
                                    category = _product.Products.CategoryString
                                });
                        }
                    }
                    else
                    {
                        foreach (var _product in userProductsList.ToList())
                        {
                            _listToReturn.Add(new
                            {
                                id = _product.Id,
                                productId = _product.ProductId,
                                name = _product.Products.Name,
                                weight = _product.QuantityWeight,
                                quantity = _product.Quantity,
                                price = _product.Products.Price,
                                barcode = _product.Products.Barcode,
                                brand = _product.Products.Brand,
                                category = _product.Products.CategoryString
                            });
                        }
                    }
                    return Json(_listToReturn, JsonRequestBehavior.AllowGet);
                case "inventory":
                    var userInventoryProductsList = db.UserProductsList
                        .Include(u => u.Products)
                        .Include(u => u.AspNetUsers)
                        .Where(u => u.UserId.Equals(userId))
                        .Where(u => u.ListName.ToLower().Equals("inventory"));
                    List<Models.UserProductListCompleteModel> _listToReturn2 = new List<Models.UserProductListCompleteModel>();

                    Models.UserProductListCompleteModel _UserProductListCompleteModelInventory = new Models.UserProductListCompleteModel
                    {
                        ItemType = "legend",
                        Name = "Legenda"
                    };
                    _listToReturn2.Add(_UserProductListCompleteModelInventory);

                    //_UserProductListCompleteModelInventory
                    List<Models.UserProductListCompleteModel> _listToReverse = new List<Models.UserProductListCompleteModel>();
                    foreach (var _product in userInventoryProductsList.ToList())
                    {
                        _listToReverse.Add(new Models.UserProductListCompleteModel()
                        {
                            Id = _product.Id,
                            ProductId = _product.ProductId,
                            ItemType = "inventory",
                            Name = _product.Products.Name,
                            Weight = _product.QuantityWeight.HasValue ? _product.QuantityWeight.Value.ToString() : string.Empty,
                            Quantity = _product.Quantity.Value,
                            Price = _product.Products.Price,
                            Barcode = _product.Products.Barcode,
                            Brand = _product.Products.Brand,
                            Category = _product.Products.CategoryString
                        });
                    }
                    _listToReverse.Reverse();
                    _listToReturn2.AddRange(_listToReverse);

                    foreach (var productCombined in _listToReturn2)
                    {
                        var userShoppingList = from m in db.StoreProducts where m.ProductId == productCombined.ProductId select m;
                        if (userShoppingList.Count() > 0)
                        {
                            foreach (var storeProduct in userShoppingList)
                            {
                                if (productCombined.PriceList == null) productCombined.PriceList = new Dictionary<string, double>();
                                if (!productCombined.PriceList.ContainsKey(storeProduct.StoreId.ToString()))
                                    productCombined.PriceList.Add(storeProduct.StoreId.ToString(), Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2));

                                if (storeProduct.Stores.Name == "Jumbo") productCombined.Url = storeProduct.Url;
                            }
                        }
                    }

                    Models.UserProductListCompleteModel _UserProductListCompleteModelMoveToShoopingList = new Models.UserProductListCompleteModel
                    {
                        ItemType = "moveToShoppingList",
                        Name = "Mover para lista de compras"
                    };
                    _listToReturn2.Add(_UserProductListCompleteModelMoveToShoopingList);

                    return Json(_listToReturn2, JsonRequestBehavior.AllowGet);
                case "out":
                    return Json(new List<Models.UserProductListModel>(), JsonRequestBehavior.AllowGet);
                default:
                    return Json(new List<Models.UserProductListModel>(), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public async Task<JsonResult> GetShoppingCartListOfUser2(string userId, string listName, string searchQuery = "")
        {
            Managers.InteractionsManager.Add(userId, "GetShoppingCartListOfUser", "listName:" + listName);
            UserUpdatePricesRequests _UserUpdatePricesRequests = Managers.UserUpdatePricesRequestsManager.Add(userId);

            switch (listName.ToLower())
            {
                case "all":
                    //var userShoppingList = db.UserProductsList.Include(u => u.Products).Include(u => u.AspNetUsers).Where(u => u.UserId.Equals(userId)).Where(u => u.ListName.ToLower().Equals("in"));
                    List<Models.UserProductListCompleteModel2> combinedLists = new List<Models.UserProductListCompleteModel2>();

                    //TODO - delete because shopping list wont have search?
                    if (searchQuery != null && !string.IsNullOrEmpty(searchQuery))
                    {
                        var userShoppingList = from m in db.UserProductsList where m.UserId == userId && m.ListName.ToLower() == "in" && m.Products.Name.ToLowerInvariant().Contains(searchQuery.ToLowerInvariant()) orderby m.LastAddedDate descending select new SpiroWeb.Models.UserProductListModel { UserId = userId, ListName = "in", ItemType = "shoppingList", ProductId = m.ProductId, Quantity = m.Quantity ?? 1 };

                        var userConsumedProducts = from m in db.UserProductsConsumed where m.UserId == userId && m.ActionTakenByUser == null && m.Products.Name.ToLowerInvariant().Contains(searchQuery.ToLowerInvariant()) select new SpiroWeb.Models.UserProductListModel { UserId = userId, ListName = "consumed", ItemType = "consumed", ProductId = m.ProductId, Quantity = m.Quantity ?? 1 };
                        var userConsumedProductsGrouped = (from m in userConsumedProducts
                                                           group m by new { m.ProductId, m.UserId } into g
                                                           select g);

                        var consumedProductsInnerJoinQuery =
                            from userConsumedProduct in userConsumedProductsGrouped
                            join prod in db.Products on userConsumedProduct.Key.ProductId equals prod.Id
                            select new Models.UserProductListCompleteModel2
                            {
                                ProductId = prod.Id,
                                Quantity = userConsumedProduct.Sum(x => x.Quantity),
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "consumed",
                                Name = prod.Name,
                                Category = prod.CategoryString,
                                Price = prod.Price * userConsumedProduct.Sum(x => x.Quantity)
                            };

                        var shoppingListProductsInnerJoinQuery =
                            from userShoppingListProduct in userShoppingList
                            join prod in db.Products on userShoppingListProduct.ProductId equals prod.Id
                            orderby userShoppingListProduct.Id descending
                            select new Models.UserProductListCompleteModel2
                            {
                                Id = userShoppingListProduct.Id,
                                ProductId = prod.Id,
                                Quantity = userShoppingListProduct.Quantity,
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "shoppingList",
                                Name = prod.Name,
                                Category = prod.CategoryString,
                                Price = prod.Price
                            };
                        if (consumedProductsInnerJoinQuery.Count() > 0)
                        {
                            Models.UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2 { ItemType = "separator", Name = "Consumed List" };
                            //var consumedLisTotalPrice = consumedProductsInnerJoinQuery.Select(g => g.Quantity * g.Price).Sum();
                            var consumedLisTotalPrice = consumedProductsInnerJoinQuery.Select(g => g.Price).Sum();
                            _UserProductListCompleteModel.Price = consumedLisTotalPrice;
                            combinedLists.Add(_UserProductListCompleteModel);
                        }

                        combinedLists.AddRange(consumedProductsInnerJoinQuery);

                        if (shoppingListProductsInnerJoinQuery.Count() > 0)
                        {
                            Models.UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2 { ItemType = "separator", Name = "Shopping List" };
                            var shoppingListTotalPrice = shoppingListProductsInnerJoinQuery.Select(g => g.Price).Sum();
                            _UserProductListCompleteModel.Price = shoppingListTotalPrice;
                            combinedLists.Add(_UserProductListCompleteModel);

                        }
                        combinedLists.AddRange(shoppingListProductsInnerJoinQuery);
                    }
                    else
                    {
                        var userShoppingList = from m in db.UserProductsList where m.UserId == userId && m.ListName.ToLower() == "in" select m;
                        var userConsumedProducts = from m in db.UserProductsConsumed where m.UserId == userId && m.ActionTakenByUser == null select new SpiroWeb.Models.UserProductListModel { Id = m.Id, UserId = userId, ListName = "consumed", ItemType = "consumed", ProductId = m.ProductId, Quantity = m.Quantity ?? 1 };
                        var userConsumedProductsGrouped = (from m in userConsumedProducts
                                                           group m by new { m.ProductId, m.UserId } into g
                                                           select g);
                        var consumedProductsInnerJoinQuery =
                            from userConsumedProduct in userConsumedProductsGrouped
                            join prod in db.Products on userConsumedProduct.Key.ProductId equals prod.Id
                            select new Models.UserProductListCompleteModel2
                            {
                                ProductId = prod.Id,
                                Quantity = userConsumedProduct.Sum(x => x.Quantity),
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "consumed",
                                Name = prod.Name,
                                Weight = prod.Weight,
                                Category = prod.CategoryString,
                                Price = Math.Round(prod.Price.Value * userConsumedProduct.Sum(x => x.Quantity), 2)
                            };

                        var shoppingListProductsInnerJoinQuery =
                            from userShoppingListProduct in userShoppingList
                            join prod in db.Products on userShoppingListProduct.ProductId equals prod.Id
                            orderby userShoppingListProduct.Id descending
                            select new Models.UserProductListCompleteModel2
                            {
                                Id = userShoppingListProduct.Id,
                                ProductId = prod.Id,
                                Quantity = userShoppingListProduct.Quantity ?? 1,
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "shoppingList",
                                Name = prod.Name,
                                Weight = prod.Weight,
                                Category = prod.CategoryString,
                                //Price = prod.Price
                                Price = Math.Round(prod.Price.Value * userShoppingListProduct.Quantity ?? 1, 2)
                            };

                        if (consumedProductsInnerJoinQuery.Count() > 0)
                        {
                            Models.UserProductListCompleteModel2 _UserProductListCompleteModelConsumedLegend = new Models.UserProductListCompleteModel2
                            {
                                ItemType = "consumedlegend",
                                Name = "Legenda Consumidos"
                            };
                            combinedLists.Add(_UserProductListCompleteModelConsumedLegend);
                        }
                        combinedLists.AddRange(consumedProductsInnerJoinQuery);


                        if (shoppingListProductsInnerJoinQuery.Count() > 0)
                        {

                            Models.UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2
                            {
                                ItemType = "legend",
                                Name = "Legenda"
                            };
                            combinedLists.Add(_UserProductListCompleteModel);



                            combinedLists.AddRange(shoppingListProductsInnerJoinQuery);



                            Models.UserProductListCompleteModel2 _UserProductListCompleteModelBuyOnline = new Models.UserProductListCompleteModel2
                            {
                                ItemType = "buyOnline",
                                Name = "Comprar Online"
                            };
                            combinedLists.Add(_UserProductListCompleteModelBuyOnline);

                            Models.UserProductListCompleteModel2 _UserProductListCompleteModelCheckout = new Models.UserProductListCompleteModel2
                            {
                                ItemType = "checkout",
                                Name = "Confirmar"
                            };
                            combinedLists.Add(_UserProductListCompleteModelCheckout);

                            Models.UserProductListCompleteModel2 _UserProductListCompleteModelShareList = new Models.UserProductListCompleteModel2
                            {
                                ItemType = "shareList",
                                Name = "Partilhar"
                            };
                            combinedLists.Add(_UserProductListCompleteModelShareList);


                        }

                        Models.UserProductListCompleteModel2 _UserProductListCompleteEmpty = new Models.UserProductListCompleteModel2
                        {
                            ItemType = "empty",
                            Name = "Vazio"
                        };
                        combinedLists.Add(_UserProductListCompleteEmpty);

                    }

                    foreach (var productCombined in combinedLists)
                    {
                        var userShoppingList = from m in db.StoreProducts where m.ProductId == productCombined.ProductId select m;
                        if (userShoppingList.Count() > 0)
                        {
                            foreach (var storeProduct in userShoppingList)
                            {
                                if (productCombined.PriceList == null) productCombined.PriceList = new List<Models.StoreProduct>();
                                //if (!productCombined.PriceList.ContainsKey(storeProduct.StoreId.ToString()))
                                productCombined.PriceList.Add(new Models.StoreProduct
                                {
                                    Id = storeProduct.Id,
                                    Price = Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2),
                                    StoreId = storeProduct.StoreId,
                                    Url = storeProduct.Url,
                                    CreatedByUserId = storeProduct.UserId,
                                    NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
                                });
                                //storeProduct.StoreId.ToString(), Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2));

                                ///if (storeProduct.Stores.Name == "Jumbo") productCombined.Url = storeProduct.Url;
                            }
                        }
                    }
                    return Json(combinedLists, JsonRequestBehavior.AllowGet);
                case "shoppinglist":
                    var userProductsList = db.UserProductsList.Include(u => u.Products).Include(u => u.AspNetUsers).Where(u => u.UserId.Equals(userId)).Where(u => u.ListName.ToLower().Equals("in"));
                    List<dynamic> _listToReturn = new List<dynamic>();

                    if (searchQuery != null && !string.IsNullOrEmpty(searchQuery))
                    {
                        foreach (var _product in userProductsList.ToList())
                        {
                            if (_product.Products.Name.ToLowerInvariant().Contains(searchQuery.ToLowerInvariant()))
                                _listToReturn.Add(new
                                {
                                    id = _product.Id,
                                    productId = _product.ProductId,
                                    name = _product.Products.Name,
                                    weight = _product.QuantityWeight,
                                    quantity = _product.Quantity,
                                    price = _product.Products.Price,
                                    barcode = _product.Products.Barcode,
                                    brand = _product.Products.Brand,
                                    category = _product.Products.CategoryString
                                });
                        }
                    }
                    else
                    {
                        foreach (var _product in userProductsList.ToList())
                        {
                            _listToReturn.Add(new
                            {
                                id = _product.Id,
                                productId = _product.ProductId,
                                name = _product.Products.Name,
                                weight = _product.QuantityWeight,
                                quantity = _product.Quantity,
                                price = _product.Products.Price,
                                barcode = _product.Products.Barcode,
                                brand = _product.Products.Brand,
                                category = _product.Products.CategoryString
                            });
                        }
                    }
                    return Json(_listToReturn, JsonRequestBehavior.AllowGet);
                case "inventory":
                    var userInventoryProductsList = db.UserProductsList
                        .Include(u => u.Products)
                        .Include(u => u.AspNetUsers)
                        .Where(u => u.UserId.Equals(userId))
                        .Where(u => u.ListName.ToLower().Equals("inventory"));
                    List<Models.UserProductListCompleteModel2> _listToReturn2 = new List<Models.UserProductListCompleteModel2>();

                    Models.UserProductListCompleteModel2 _UserProductListCompleteModelInventory = new Models.UserProductListCompleteModel2
                    {
                        ItemType = "legend",
                        Name = "Legenda"
                    };
                    _listToReturn2.Add(_UserProductListCompleteModelInventory);

                    //_UserProductListCompleteModelInventory
                    List<Models.UserProductListCompleteModel2> _listToReverse = new List<Models.UserProductListCompleteModel2>();
                    foreach (var _product in userInventoryProductsList.ToList())
                    {
                        _listToReverse.Add(new Models.UserProductListCompleteModel2()
                        {
                            Id = _product.Id,
                            ProductId = _product.ProductId,
                            ItemType = "inventory",
                            Name = _product.Products.Name,
                            Weight = _product.QuantityWeight.HasValue ? _product.QuantityWeight.Value.ToString() : string.Empty,
                            Quantity = _product.Quantity.Value,
                            Price = _product.Products.Price,
                            Barcode = _product.Products.Barcode,
                            Brand = _product.Products.Brand,
                            Category = _product.Products.CategoryString
                        });
                    }
                    _listToReverse.Reverse();
                    _listToReturn2.AddRange(_listToReverse);


                    foreach (var productCombined in _listToReturn2)
                    {
                        var userShoppingList = from m in db.StoreProducts where m.ProductId == productCombined.ProductId select m;
                        if (userShoppingList.Count() > 0)
                        {
                            foreach (var storeProduct in userShoppingList)
                            {
                                if (productCombined.PriceList == null) productCombined.PriceList = new List<Models.StoreProduct>();
                                //if (!productCombined.PriceList.ContainsKey(storeProduct.StoreId.ToString()))
                                productCombined.PriceList.Add(new Models.StoreProduct
                                {
                                    Id = storeProduct.Id,
                                    Price = Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2),
                                    StoreId = storeProduct.StoreId,
                                    Url = storeProduct.Url,
                                    CreatedByUserId = storeProduct.UserId,
                                    NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
                                });
                                //storeProduct.StoreId.ToString(), Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2));

                                ///if (storeProduct.Stores.Name == "Jumbo") productCombined.Url = storeProduct.Url;
                            }
                        }
                    }

                    Models.UserProductListCompleteModel2 _UserProductListCompleteModelMoveToShoopingList = new Models.UserProductListCompleteModel2
                    {
                        ItemType = "moveToShoppingList",
                        Name = "Mover para lista de compras"
                    };
                    _listToReturn2.Add(_UserProductListCompleteModelMoveToShoopingList);

                    return Json(_listToReturn2, JsonRequestBehavior.AllowGet);
                case "out":
                    return Json(new List<Models.UserProductListModel>(), JsonRequestBehavior.AllowGet);
                default:
                    return Json(new List<Models.UserProductListModel>(), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        [AllowCrossSiteJson]
        public async Task<JsonResult> GetShoppingCartListOfUserV2(string userId, string listName, string searchQuery = "") //for the new app
        {
            Managers.InteractionsManager.Add(userId, "GetShoppingCartListOfUser", "listName:" + listName);
            UserUpdatePricesRequests _UserUpdatePricesRequests = Managers.UserUpdatePricesRequestsManager.Add(userId);

            switch (listName.ToLower())
            {
                case "all":
                    //var userShoppingList = db.UserProductsList.Include(u => u.Products).Include(u => u.AspNetUsers).Where(u => u.UserId.Equals(userId)).Where(u => u.ListName.ToLower().Equals("in"));
                    List<Models.UserProductListCompleteModel2> combinedLists = new List<Models.UserProductListCompleteModel2>();

                    //TODO - delete because shopping list wont have search?
                    if (searchQuery != null && !string.IsNullOrEmpty(searchQuery))
                    {
                        var userShoppingList = from m in db.UserProductsList where m.UserId == userId && m.ListName.ToLower() == "in" && m.Products.Name.ToLowerInvariant().Contains(searchQuery.ToLowerInvariant()) orderby m.LastAddedDate descending select new SpiroWeb.Models.UserProductListModel { UserId = userId, ListName = "in", ItemType = "shoppingList", ProductId = m.ProductId, Quantity = m.Quantity ?? 1 };

                        var userConsumedProducts = from m in db.UserProductsConsumed where m.UserId == userId && m.ActionTakenByUser == null && m.Products.Name.ToLowerInvariant().Contains(searchQuery.ToLowerInvariant()) select new SpiroWeb.Models.UserProductListModel { UserId = userId, ListName = "consumed", ItemType = "consumed", ProductId = m.ProductId, Quantity = m.Quantity ?? 1 };
                        var userConsumedProductsGrouped = (from m in userConsumedProducts
                                                           group m by new { m.ProductId, m.UserId } into g
                                                           select g);

                        var consumedProductsInnerJoinQuery =
                            from userConsumedProduct in userConsumedProductsGrouped
                            join prod in db.Products on userConsumedProduct.Key.ProductId equals prod.Id
                            select new Models.UserProductListCompleteModel2
                            {
                                ProductId = prod.Id,
                                Quantity = userConsumedProduct.Sum(x => x.Quantity),
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "consumed",
                                Name = prod.Name,
                                Category = prod.CategoryString,
                                Price = prod.Price * userConsumedProduct.Sum(x => x.Quantity)
                            };

                        var shoppingListProductsInnerJoinQuery =
                            from userShoppingListProduct in userShoppingList
                            join prod in db.Products on userShoppingListProduct.ProductId equals prod.Id
                            orderby userShoppingListProduct.Id descending
                            select new Models.UserProductListCompleteModel2
                            {
                                Id = userShoppingListProduct.Id,
                                ProductId = prod.Id,
                                Quantity = userShoppingListProduct.Quantity,
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "shoppingList",
                                Name = prod.Name,
                                Category = prod.CategoryString,
                                Price = prod.Price
                            };
                        if (consumedProductsInnerJoinQuery.Count() > 0)
                        {
                            Models.UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2 { ItemType = "separator", Name = "Consumed List" };
                            //var consumedLisTotalPrice = consumedProductsInnerJoinQuery.Select(g => g.Quantity * g.Price).Sum();
                            var consumedLisTotalPrice = consumedProductsInnerJoinQuery.Select(g => g.Price).Sum();
                            _UserProductListCompleteModel.Price = consumedLisTotalPrice;
                            combinedLists.Add(_UserProductListCompleteModel);
                        }

                        combinedLists.AddRange(consumedProductsInnerJoinQuery);

                        if (shoppingListProductsInnerJoinQuery.Count() > 0)
                        {
                            Models.UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2 { ItemType = "separator", Name = "Shopping List" };
                            var shoppingListTotalPrice = shoppingListProductsInnerJoinQuery.Select(g => g.Price).Sum();
                            _UserProductListCompleteModel.Price = shoppingListTotalPrice;
                            combinedLists.Add(_UserProductListCompleteModel);

                        }
                        combinedLists.AddRange(shoppingListProductsInnerJoinQuery);
                    }
                    else
                    {
                        var userShoppingList = from m in db.UserProductsList where m.UserId == userId && m.ListName.ToLower() == "in" select m;
                        var userConsumedProducts = from m in db.UserProductsConsumed where m.UserId == userId && m.ActionTakenByUser == null select new SpiroWeb.Models.UserProductListModel { Id = m.Id, UserId = userId, ListName = "consumed", ItemType = "consumed", ProductId = m.ProductId, Quantity = m.Quantity ?? 1 };
                        var userConsumedProductsGrouped = (from m in userConsumedProducts
                                                           group m by new { m.ProductId, m.UserId } into g
                                                           select g);
                        var consumedProductsInnerJoinQuery =
                            from userConsumedProduct in userConsumedProductsGrouped
                            join prod in db.Products on userConsumedProduct.Key.ProductId equals prod.Id
                            select new Models.UserProductListCompleteModel2
                            {
                                ProductId = prod.Id,
                                Quantity = userConsumedProduct.Sum(x => x.Quantity),
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "consumed",
                                Name = prod.Name,
                                Weight = prod.Weight,
                                Category = prod.CategoryString,
                                Price = Math.Round(prod.Price.Value * userConsumedProduct.Sum(x => x.Quantity), 2)
                            };

                        var shoppingListProductsInnerJoinQuery =
                            from userShoppingListProduct in userShoppingList
                            join prod in db.Products on userShoppingListProduct.ProductId equals prod.Id
                            orderby userShoppingListProduct.Id descending
                            select new Models.UserProductListCompleteModel2
                            {
                                Id = userShoppingListProduct.Id,
                                ProductId = prod.Id,
                                Quantity = userShoppingListProduct.Quantity ?? 1,
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "shoppingList",
                                Name = prod.Name,
                                Weight = prod.Weight,
                                Category = prod.CategoryString,
                                //Price = prod.Price
                                Price = Math.Round(prod.Price.Value * userShoppingListProduct.Quantity ?? 1, 2)
                            };
                        combinedLists.AddRange(consumedProductsInnerJoinQuery);


                        if (shoppingListProductsInnerJoinQuery.Count() > 0)
                        {
                            combinedLists.AddRange(shoppingListProductsInnerJoinQuery);

                        }

                    }

                    foreach (var productCombined in combinedLists)
                    {
                        var userShoppingList = from m in db.StoreProducts where m.ProductId == productCombined.ProductId select m;
                        if (userShoppingList.Count() > 0)
                        {
                            foreach (var storeProduct in userShoppingList)
                            {
                                if (productCombined.PriceList == null) productCombined.PriceList = new List<Models.StoreProduct>();
                                productCombined.PriceList.Add(new Models.StoreProduct
                                {
                                    Id = storeProduct.Id,
                                    Price = Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2),
                                    StoreId = storeProduct.StoreId,
                                    Url = storeProduct.Url,
                                    CreatedByUserId = storeProduct.UserId,
                                    NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
                                });
                            }
                        }
                    }
                    return Json(combinedLists, JsonRequestBehavior.AllowGet);
                case "shoppinglist":
                    var userProductsList = db.UserProductsList.Include(u => u.Products).Include(u => u.AspNetUsers).Where(u => u.UserId.Equals(userId)).Where(u => u.ListName.ToLower().Equals("in"));
                    List<dynamic> _listToReturn = new List<dynamic>();

                    if (searchQuery != null && !string.IsNullOrEmpty(searchQuery))
                    {
                        foreach (var _product in userProductsList.ToList())
                        {
                            if (_product.Products.Name.ToLowerInvariant().Contains(searchQuery.ToLowerInvariant()))
                                _listToReturn.Add(new
                                {
                                    id = _product.Id,
                                    productId = _product.ProductId,
                                    name = _product.Products.Name,
                                    weight = _product.QuantityWeight,
                                    quantity = _product.Quantity,
                                    price = _product.Products.Price,
                                    barcode = _product.Products.Barcode,
                                    brand = _product.Products.Brand,
                                    category = _product.Products.CategoryString
                                });
                        }
                    }
                    else
                    {
                        foreach (var _product in userProductsList.ToList())
                        {
                            _listToReturn.Add(new
                            {
                                id = _product.Id,
                                productId = _product.ProductId,
                                name = _product.Products.Name,
                                weight = _product.QuantityWeight,
                                quantity = _product.Quantity,
                                price = _product.Products.Price,
                                barcode = _product.Products.Barcode,
                                brand = _product.Products.Brand,
                                category = _product.Products.CategoryString
                            });
                        }
                    }
                    return Json(_listToReturn, JsonRequestBehavior.AllowGet);
                case "inventory":
                    var userInventoryProductsList = db.UserProductsList
                        .Include(u => u.Products)
                        .Include(u => u.AspNetUsers)
                        .Where(u => u.UserId.Equals(userId))
                        .Where(u => u.ListName.ToLower().Equals("inventory"));
                    List<Models.UserProductListCompleteModel2> _listToReturn2 = new List<Models.UserProductListCompleteModel2>();


                    //_UserProductListCompleteModelInventory
                    List<Models.UserProductListCompleteModel2> _listToReverse = new List<Models.UserProductListCompleteModel2>();
                    foreach (var _product in userInventoryProductsList.ToList())
                    {
                        _listToReverse.Add(new Models.UserProductListCompleteModel2()
                        {
                            Id = _product.Id,
                            ProductId = _product.ProductId,
                            ItemType = "inventory",
                            Name = _product.Products.Name,
                            Weight = _product.QuantityWeight.HasValue ? _product.QuantityWeight.Value.ToString() : string.Empty,
                            Quantity = _product.Quantity.Value,
                            Price = _product.Products.Price,
                            Barcode = _product.Products.Barcode,
                            Brand = _product.Products.Brand,
                            Category = _product.Products.CategoryString
                        });
                    }
                    _listToReverse.Reverse();
                    _listToReturn2.AddRange(_listToReverse);


                    foreach (var productCombined in _listToReturn2)
                    {
                        var userShoppingList = from m in db.StoreProducts where m.ProductId == productCombined.ProductId select m;
                        if (userShoppingList.Count() > 0)
                        {
                            foreach (var storeProduct in userShoppingList)
                            {
                                if (productCombined.PriceList == null) productCombined.PriceList = new List<Models.StoreProduct>();
                                productCombined.PriceList.Add(new Models.StoreProduct
                                {
                                    Id = storeProduct.Id,
                                    Price = Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2),
                                    StoreId = storeProduct.StoreId,
                                    Url = storeProduct.Url,
                                    CreatedByUserId = storeProduct.UserId,
                                    NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
                                });
                            }
                        }
                    }

                    return Json(_listToReturn2, JsonRequestBehavior.AllowGet);
                case "out":
                    return Json(new List<Models.UserProductListModel>(), JsonRequestBehavior.AllowGet);
                default:
                    return Json(new List<Models.UserProductListModel>(), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult AddDummyProductToQueue(string barCode)
        {
            string userId = User.Identity.GetUserId();
            long _barcode = (!string.IsNullOrEmpty(barCode) ? long.Parse(barCode) : 0);

            //db.UserProductsQueue.Add(new UserProductsQueue{ BarCode = _barcode, UserId = userId, ListName = "In", InsertDate = DateTime.Now });
            //db.SaveChanges();

            int _UserProductsListId = Helpers.ProductsQueue.ProcessProduct(barCode, userId, true);

            if (_UserProductsListId != -1)
            {
                Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCart(_UserProductsListId, userId);
            }
            else
            {
                Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCartProductsInQueue(barCode, userId);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddQuantityToShoppingListItem(int userProductListId)
        {
            UserProductsList queryExistsInUserList = (from c in db.UserProductsList
                                                      where c.Id.Equals(userProductListId)
                                                      select c).FirstOrDefault();
            if (queryExistsInUserList != null)
            {
                //int newQuantity = queryExistsInUserList.Quantity + 1;
                queryExistsInUserList.Quantity = queryExistsInUserList.Quantity + 1;
                db.UserProductsList.Attach(queryExistsInUserList);
                var entry = db.Entry(queryExistsInUserList);
                entry.Property(y => y.Quantity).IsModified = true;
                // other changed properties

                db.SaveChanges();
                //_userListProductId = queryExistsInUserList.Id;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AddQuantityToShoppingListItemGET(int userProductListId, string userId)
        {
            UserProductsList queryExistsInUserList = (from c in db.UserProductsList
                                                      where c.Id.Equals(userProductListId)
                                                      select c).FirstOrDefault();
            if (queryExistsInUserList != null)
            {
                //int newQuantity = queryExistsInUserList.Quantity + 1;
                queryExistsInUserList.Quantity = queryExistsInUserList.Quantity + 1;
                db.UserProductsList.Attach(queryExistsInUserList);
                var entry = db.Entry(queryExistsInUserList);
                entry.Property(y => y.Quantity).IsModified = true;
                // other changed properties

                db.SaveChanges();
                //_userListProductId = queryExistsInUserList.Id;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SubtractQuantityToShoppingListItem(int userProductListId)
        {
            UserProductsList queryExistsInUserList = (from c in db.UserProductsList
                                                      where c.Id.Equals(userProductListId)
                                                      select c).FirstOrDefault();
            if (queryExistsInUserList != null)
            {
                //int newQuantity = queryExistsInUserList.Quantity + 1;
                if (queryExistsInUserList.Quantity - 1 != 0)
                {
                    queryExistsInUserList.Quantity = queryExistsInUserList.Quantity - 1;
                    db.UserProductsList.Attach(queryExistsInUserList);
                    var entry = db.Entry(queryExistsInUserList);
                    entry.Property(y => y.Quantity).IsModified = true;
                    // other changed properties

                    db.SaveChanges();
                }
                else
                {
                    db.UserProductsList.Remove(queryExistsInUserList);
                    db.SaveChanges();
                }
                //_userListProductId = queryExistsInUserList.Id;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SubtractQuantityToShoppingListItemGET(int userProductListId, string userId)
        {
            UserProductsList queryExistsInUserList = (from c in db.UserProductsList
                                                      where c.Id.Equals(userProductListId)
                                                      select c).FirstOrDefault();
            if (queryExistsInUserList != null)
            {
                //int newQuantity = queryExistsInUserList.Quantity + 1;
                if (queryExistsInUserList.Quantity - 1 != 0)
                {
                    queryExistsInUserList.Quantity = queryExistsInUserList.Quantity - 1;
                    db.UserProductsList.Attach(queryExistsInUserList);
                    var entry = db.Entry(queryExistsInUserList);
                    entry.Property(y => y.Quantity).IsModified = true;
                    // other changed properties

                    db.SaveChanges();
                }
                else
                {
                    db.UserProductsList.Remove(queryExistsInUserList);
                    db.SaveChanges();
                }
                //_userListProductId = queryExistsInUserList.Id;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        ///TODO - Add to inventory
        [HttpGet]
        public JsonResult RemoveUserMultipleProductsFromShoppingList(string userId, string userProductsListId)
        {
            string[] _productsId = userProductsListId.Split(':');

            foreach (string _userProductId in _productsId)
            {
                int userProductId = int.Parse(_userProductId);
                UserProductsList queryExistsInUserList = (from c in db.UserProductsList
                                                          where c.Id.Equals(userProductId)
                                                          select c).FirstOrDefault();
                if (queryExistsInUserList != null)
                {
                    db.UserProductsList.Remove(queryExistsInUserList);

                }
            }
            db.SaveChanges();
            Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCart(-1, userId);

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RemoveUserProductFromShoppingList(int userProductListId, string userId)
        {
            UserProductsList queryExistsInUserList = (from c in db.UserProductsList
                                                      where c.Id.Equals(userProductListId)
                                                      select c).FirstOrDefault();
            if (queryExistsInUserList != null)
            {
                db.UserProductsList.Remove(queryExistsInUserList);
                db.SaveChanges();
            }
            Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCart(-1, userId);

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RemoveUserProductSimpleFromShoppingList(int userProductSimpleId, string userId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    UserProductsSimple _queryExists = (from c in db.UserProductsSimple
                                                       where c.Id.Equals(userProductSimpleId)
                                                       select c).FirstOrDefault();
                    if (_queryExists != null)
                    {
                        db.UserProductsSimple.Remove(_queryExists);
                        db.SaveChanges();
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                    //Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCart(-1, userId);
                }
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        ///consumed products
        ///
        //[HttpGet]
        //[HttpPost]
        public JsonResult AddConsumedProductToShoppingListGET(int productId, string userId)
        {
            try
            {
                List<UserProductsConsumed> queryExistsInUserList = (from c in db.UserProductsConsumed
                                                                    where c.ProductId.Equals(productId) && c.UserId.Equals(userId)
                                                                    && c.ActionTakenByUser == null
                                                                    select c).ToList();

                //add product to shoppingList
                //TODO - improve code
                var userConsumedProducts = from m in db.UserProductsConsumed where m.ProductId.Equals(productId) && m.UserId.Equals(userId) && m.ActionTakenByUser == null select new SpiroWeb.Models.UserProductListModel { UserId = userId, ListName = "consumed", ItemType = "consumed", ProductId = m.ProductId, Quantity = m.Quantity ?? 1 };
                var userConsumedProductsGrouped = (from m in userConsumedProducts
                                                   group m by new { m.ProductId, m.UserId } into g
                                                   select g);



                if (userConsumedProductsGrouped.Count() > 0)
                {
                    AddProductToShoppingCartGET(productId, userConsumedProducts.ToList().Count, null, true, userId);
                }
                //if (consumedProductQuantity != null)
                //{
                //    AddProductToShoppingCart(productId, consumedProductQuantity.Quantity ?? 1, null, true);
                //}
                foreach (UserProductsConsumed userProductsConsumed in queryExistsInUserList)
                {
                    userProductsConsumed.ActionTakenByUser = "added";
                    userProductsConsumed.ActionTakenByUserDate = DateTime.Now;
                    db.Entry(userProductsConsumed).State = EntityState.Modified;
                }


                db.SaveChanges();

                return Json(queryExistsInUserList.Count(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message + " - " + ex.InnerException.Message + " - " + ex.InnerException.InnerException.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult RemoveConsumedProductFromList(int productId, string userId)
        {
            try
            {
                List<UserProductsConsumed> queryExistsInUserList = (from c in db.UserProductsConsumed
                                                                    where c.ProductId.Equals(productId) && c.UserId.Equals(userId)
                                                                    && c.ActionTakenByUser == null
                                                                    select c).ToList();

                foreach (UserProductsConsumed userProductsConsumed in queryExistsInUserList)
                {
                    userProductsConsumed.ActionTakenByUser = "notAdded";
                    userProductsConsumed.ActionTakenByUserDate = DateTime.Now;
                }
                db.SaveChanges();

                return Json(queryExistsInUserList.Count(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }

        //New way with product simple
        [HttpGet]
        public JsonResult AddSpokenProductToShoppingCart(string userId, string productSpokenName, bool isFromApp = false) //isFromApp is for legacy purposes
        {
            Managers.InteractionsManager.Add(userId, "ShoppingCart/AddSpokenProductToShoppingCart", productSpokenName);
            Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");
            int _userProductSimpleId = -1;
            try
            {
                ProductSimpleItem _ProductSimpleItem = new ProductSimpleItem();
                _ProductSimpleItem.Name = productSpokenName;
                _ProductSimpleItem.UserId = userId;
                _ProductSimpleItem.ImageUrl = string.Empty;
                _ProductSimpleItem.List = "shoppingList";
                _userProductSimpleId = Managers.ProductsManager.AddProductSimpleToUserList(_ProductSimpleItem);
                if (_userProductSimpleId == -1)
                    return Json(-1, JsonRequestBehavior.AllowGet);
                else
                    return Json(_userProductSimpleId, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.InnerException.Message);
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }
        //WORKING - Old way (without simple products)
        //[HttpGet]
        //public JsonResult AddSpokenProductToShoppingCart(string userId, string productSpokenName, bool isFromApp = false) //isFromApp is for legacy purposes
        //{
        //    try
        //    {
        //        //int _userProductListId = AddProductToShoppingCartGET(994, 1, null, false, userId);

        //        //see if association already exists
        //        UserSpokenProducts _queryExists = (from c in db.UserSpokenProducts
        //                                           where c.UserId.Equals(userId) &&
        //                                           c.ProductSpokenName.ToLower().Equals(productSpokenName.ToLower())
        //                                           select c).FirstOrDefault();

        //        //if yes add product and add it to the shopping list
        //        if (_queryExists != null)
        //        {
        //            AddProductToShoppingCartGET(_queryExists.ProductId, true, userId);

        //            if (isFromApp == false) //is from lisie home 
        //            {
        //                //Notify android
        //                DataManager.UserDevicesManager _userDevicesManager2 = new DataManager.UserDevicesManager();
        //                List<ClassLibrary1.UserDevices> _userDevicesTokens2 = _userDevicesManager2.GetUserDevicesTokens(userId);

        //                if (_userDevicesTokens2.Count() > 0)
        //                {
        //                    foreach (ClassLibrary1.UserDevices _userDevice in _userDevicesTokens2)
        //                    {
        //                        Helpers.FirebaseAndroid.SendNotificationToAndroidPhone(_userDevice.DeviceToken, "refreshListShoppingList:" + productSpokenName);
        //                    }
        //                } 
        //            }

        //            //return Json("Spoken Product " + productSpokenName + " added to Shopping List", JsonRequestBehavior.AllowGet);
        //            return Json(_queryExists.ProductId, JsonRequestBehavior.AllowGet);

        //        }
        //        //If no send notification to android to associate spoken product
        //        else
        //        {
        //            if (isFromApp == false) //is from lisie home , send notification to android app
        //            {
        //                //get user last device token and send notification to associate spoken product
        //                DataManager.UserDevicesManager _userDevicesManager2 = new DataManager.UserDevicesManager();
        //                List<ClassLibrary1.UserDevices> _userDevicesTokens2 = _userDevicesManager2.GetUserDevicesTokens(userId);

        //                if (_userDevicesTokens2.Count() > 0)
        //                {
        //                    foreach (ClassLibrary1.UserDevices _userDevice in _userDevicesTokens2)
        //                    {
        //                        Helpers.FirebaseAndroid.SendNotificationToAndroidPhone(_userDevice.DeviceToken, "spokenProductToAssociate:" + productSpokenName);
        //                    }
        //                }
        //            }
        //            //return Json("Spoken Product Association Requested for " + productSpokenName, JsonRequestBehavior.AllowGet);
        //            return Json(-1, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpGet]
        public JsonResult RemoveSpokenProductAssociation(string userId, string productSpokenName, bool isFromApp = false)
        {
            try
            {
                //int _userProductListId = AddProductToShoppingCartGET(994, 1, null, false, userId);

                //see if association already exists
                UserSpokenProducts _queryExists = (from c in db.UserSpokenProducts
                                                   where c.UserId.Equals(userId) &&
                                                   c.ProductSpokenName.ToLower().Equals(productSpokenName.ToLower())
                                                   select c).FirstOrDefault();

                //if yes remove association
                if (_queryExists != null)
                {
                    var _userSpokenProductId = _queryExists.Id;
                    db.UserSpokenProducts.Remove(_queryExists);
                    db.SaveChanges();

                    //if (isFromApp == false) //is from lisie home 
                    //{
                    //    //Notify android
                    //    DataManager.UserDevicesManager _userDevicesManager2 = new DataManager.UserDevicesManager();
                    //    List<ClassLibrary1.UserDevices> _userDevicesTokens2 = _userDevicesManager2.GetUserDevicesTokens(userId);

                    //    if (_userDevicesTokens2.Count() > 0)
                    //    {
                    //        foreach (ClassLibrary1.UserDevices _userDevice in _userDevicesTokens2)
                    //        {
                    //            Helpers.FirebaseAndroid.SendNotificationToAndroidPhone(_userDevice.DeviceToken, "refreshListShoppingList:" + productSpokenName);
                    //        }
                    //    }
                    //}

                    //return Json("Spoken Product " + productSpokenName + " added to Shopping List", JsonRequestBehavior.AllowGet);
                    return Json(_userSpokenProductId, JsonRequestBehavior.AllowGet);

                }
                //If no send notification to android to associate spoken product
                else
                {
                    //if (isFromApp == false) //is from lisie home , send notification to android app
                    //{
                    //    //get user last device token and send notification to associate spoken product
                    //    DataManager.UserDevicesManager _userDevicesManager2 = new DataManager.UserDevicesManager();
                    //    List<ClassLibrary1.UserDevices> _userDevicesTokens2 = _userDevicesManager2.GetUserDevicesTokens(userId);

                    //    if (_userDevicesTokens2.Count() > 0)
                    //    {
                    //        foreach (ClassLibrary1.UserDevices _userDevice in _userDevicesTokens2)
                    //        {
                    //            Helpers.FirebaseAndroid.SendNotificationToAndroidPhone(_userDevice.DeviceToken, "spokenProductToAssociate:" + productSpokenName);
                    //        }
                    //    }
                    //}
                    //return Json("Spoken Product Association Requested for " + productSpokenName, JsonRequestBehavior.AllowGet);
                    return Json(-1, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult AssociateSpokenProduct(string userId, int productId, string spokenProductName)
        {
            try
            {
                //associate product
                UserSpokenProducts _newUserSpokenProducts = new UserSpokenProducts();
                _newUserSpokenProducts.ProductId = productId;
                _newUserSpokenProducts.ProductSpokenName = spokenProductName;
                _newUserSpokenProducts.UserId = userId;

                db.UserSpokenProducts.Add(_newUserSpokenProducts);
                db.SaveChanges();

                //add to shopping cart
                AddProductToShoppingCartGET(productId, true, userId);

                //return Json("Product " + productName + " requested to be added to shopping list", JsonRequestBehavior.AllowGet);
                return Json("Sucess associating spoken product" + spokenProductName, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json("Error associating spoken product:" + ex.Message + " to shopping cart", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> UpdateUserListProductPrices(string userId)
        {
            try
            {

                DateTime before = DateTime.Now;

                OnlineProducts _OnlineProducts = new OnlineProducts();
                List<Models.UserProductListCompleteModel> _productsOfUser = GetProductsOfUser(userId);

                int _differentPricesCounter = 0;
                string _productsNamesUpdates = string.Empty;
                foreach (var _productOfUser in _productsOfUser)
                {
                    var _storeProducts = db.StoreProducts.Where(c => c.ProductId == _productOfUser.ProductId).Include(c => c.Stores).ToList();
                    foreach (var _storeProduct in _storeProducts)
                    {
                        switch (_storeProduct.StoreId)
                        {
                            case 1:
                                try
                                {
                                    LisieStores.Extensibility.ProductSearchResult _jumboProductSearchResult = await _OnlineProducts.GetJumboProductMetadata(_storeProduct.Url);
                                    if (_jumboProductSearchResult != null)
                                    {
                                        double _newPrice = double.Parse(_jumboProductSearchResult.Price.Replace("€", "").Trim());
                                        if (_storeProduct.Price != _newPrice)
                                        {
                                            ProductPricesUpdates _ProductPricesUpdate = new ProductPricesUpdates
                                            {
                                                OldPrice = _storeProduct.Price.Value,
                                                NewPrice = _newPrice,
                                                CreateDate = DateTime.Now,
                                                CreatedByUserId = userId,
                                                ProductId = _productOfUser.ProductId,
                                                StoreId = _storeProduct.StoreId
                                            };
                                            db.ProductPricesUpdates.Add(_ProductPricesUpdate);

                                            _storeProduct.Price = _newPrice;
                                            db.SaveChanges();

                                            _productsNamesUpdates += _differentPricesCounter == 0 ? _storeProduct.Products.Name : "," + _storeProduct.Products.Name;
                                            _differentPricesCounter++;
                                        }
                                    }
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    break;
                                }

                            case 2:
                                try
                                {
                                    LisieStores.Extensibility.ProductSearchResult _continenteProductSearchResult = await _OnlineProducts.GetContinenteProductMetadata(_storeProduct.Url);
                                    if (_continenteProductSearchResult != null)
                                    {
                                        double _newPrice = double.Parse(_continenteProductSearchResult.Price.Replace("€", "").Trim());
                                        if (_storeProduct.Price != _newPrice)
                                        {
                                            ProductPricesUpdates _ProductPricesUpdate = new ProductPricesUpdates
                                            {
                                                OldPrice = _storeProduct.Price.Value,
                                                NewPrice = _newPrice,
                                                CreateDate = DateTime.Now,
                                                CreatedByUserId = userId,
                                                ProductId = _productOfUser.ProductId,
                                                StoreId = _storeProduct.StoreId

                                            };
                                            db.ProductPricesUpdates.Add(_ProductPricesUpdate);

                                            _storeProduct.Price = _newPrice;
                                            db.SaveChanges();

                                            _productsNamesUpdates += _differentPricesCounter == 0 ? _storeProduct.Products.Name : "," + _storeProduct.Products.Name;
                                            _differentPricesCounter++;
                                        }
                                    }
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    break;
                                }
                            case 3:
                                try
                                {
                                    LisieStores.Extensibility.ProductSearchResult _pingoDoceProductSearchResult = await _OnlineProducts.GetPingoDoceProductMetadata(_storeProduct.Url);
                                    if (_pingoDoceProductSearchResult != null)
                                    {
                                        double _newPrice = double.Parse(_pingoDoceProductSearchResult.Price.Replace("€", "").Trim());
                                        if (_storeProduct.Price != _newPrice)
                                        {
                                            ProductPricesUpdates _ProductPricesUpdate = new ProductPricesUpdates
                                            {
                                                OldPrice = _storeProduct.Price.Value,
                                                NewPrice = _newPrice,
                                                CreateDate = DateTime.Now,
                                                CreatedByUserId = userId,
                                                ProductId = _productOfUser.ProductId,
                                                StoreId = _storeProduct.StoreId
                                            };
                                            db.ProductPricesUpdates.Add(_ProductPricesUpdate);

                                            _storeProduct.Price = _newPrice;
                                            db.SaveChanges();

                                            _differentPricesCounter++;
                                        }
                                    }
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    break;
                                }

                            default:
                                break;
                        }
                    }

                }
                DateTime after = DateTime.Now;
                TimeSpan duration = after.Subtract(before);

                Helpers.FirebaseAndroid.SendNotification(userId, "productsPricesUpdated:Foram atualizado/s " + _differentPricesCounter + " preço/s de produto em " + duration.Seconds.ToString() + "/s");

                return Json("Sucess", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json("Error - " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        //public async void UpdatePrices(string userId, List<Models.UserProductListCompleteModel> ProductsOfUser)
        //{
        //    try
        //    {
        //        OnlineProducts _OnlineProducts = new OnlineProducts();

        //        int _differentPricesCounter = 0;
        //        foreach (var _productOfUser in ProductsOfUser)
        //        {
        //            if (_productOfUser.ItemType == "separator") continue;
        //            var _storeProducts = db.StoreProducts.Where(c => c.ProductId == _productOfUser.ProductId).Include(c => c.Stores).ToList();
        //            foreach (var _storeProduct in _storeProducts)
        //            {
        //                switch (_storeProduct.StoreId)
        //                {
        //                    case 1:
        //                        Objects.ProductSearchResult _jumboProductSearchResult = await _OnlineProducts.GetJumboProductMetadata(_storeProduct.Url);
        //                        if (_jumboProductSearchResult != null)
        //                        {
        //                            double _newPrice = double.Parse(_jumboProductSearchResult.Price.Replace("€", "").Trim());
        //                            if (_storeProduct.Price.Value != _newPrice)
        //                            {
        //                                ProductPricesUpdates _ProductPricesUpdates = new ProductPricesUpdates {
        //                                    OldPrice = _storeProduct.Price.Value,
        //                                    NewPrice = _newPrice,
        //                                    CreateDate = DateTime.Now,
        //                                    CreatedByUserId = userId,
        //                                    StoreId = _storeProduct.StoreId
        //                                };
        //                                db.ProductPricesUpdates.Add(_ProductPricesUpdates);
        //                                _storeProduct.Price = _newPrice;
        //                                db.SaveChanges();
        //                                _differentPricesCounter++;
        //                            }
        //                        }
        //                        break;
        //                    case 2:
        //                        Objects.ProductSearchResult _continenteProductSearchResult = await _OnlineProducts.GetContinenteProductMetadata(_storeProduct.Url);
        //                        if (_continenteProductSearchResult != null)
        //                        {
        //                            double _newPrice = double.Parse(_continenteProductSearchResult.Price.Replace("€", "").Trim());
        //                            if (_storeProduct.Price.Value != _newPrice)
        //                            {
        //                                ProductPricesUpdates _ProductPricesUpdates = new ProductPricesUpdates
        //                                {
        //                                    OldPrice = _storeProduct.Price.Value,
        //                                    NewPrice = _newPrice,
        //                                    CreateDate = DateTime.Now,
        //                                    CreatedByUserId = userId,
        //                                    StoreId = _storeProduct.StoreId
        //                                };
        //                                db.ProductPricesUpdates.Add(_ProductPricesUpdates);
        //                                _storeProduct.Price = _newPrice;
        //                                db.SaveChanges();
        //                                _differentPricesCounter++;
        //                            }
        //                        }
        //                        break;
        //                    case 3:
        //                        Objects.ProductSearchResult _pingoDoceProductSearchResult = await _OnlineProducts.GetPingoDoceProductMetadata(_storeProduct.Url);
        //                        if (_pingoDoceProductSearchResult != null)
        //                        {
        //                            double _newPrice = double.Parse(_pingoDoceProductSearchResult.Price.Replace("€", "").Trim());
        //                            if (_storeProduct.Price.Value != _newPrice)
        //                            {
        //                                ProductPricesUpdates _ProductPricesUpdates = new ProductPricesUpdates
        //                                {
        //                                    OldPrice = _storeProduct.Price.Value,
        //                                    NewPrice = _newPrice,
        //                                    CreateDate = DateTime.Now,
        //                                    CreatedByUserId = userId,
        //                                    StoreId = _storeProduct.StoreId
        //                                };
        //                                db.ProductPricesUpdates.Add(_ProductPricesUpdates);
        //                                _storeProduct.Price = _newPrice;
        //                                db.SaveChanges();
        //                                _differentPricesCounter++;
        //                            }
        //                        }
        //                        break;
        //                    default:
        //                        break;
        //                }
        //            }

        //        }
        //        Helpers.FirebaseAndroid.SendNotification(userId, "productsPricesUpdated:" + _differentPricesCounter);

        //        //return Json("Sucess", JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        //return Json("Error - " + ex.Message, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public List<Models.UserProductListCompleteModel> GetProductsOfUser(string userId)
        {
            List<Models.UserProductListCompleteModel> combinedLists = new List<Models.UserProductListCompleteModel>();


            var userShoppingList = from m in db.UserProductsList where m.UserId == userId && m.ListName.ToLower() == "in" select m;
            var userConsumedProducts = from m in db.UserProductsConsumed where m.UserId == userId && m.ActionTakenByUser == null select new SpiroWeb.Models.UserProductListModel { UserId = userId, ListName = "consumed", ItemType = "consumed", ProductId = m.ProductId, Quantity = m.Quantity ?? 1 };
            var userConsumedProductsGrouped = (from m in userConsumedProducts
                                               group m by new { m.ProductId, m.UserId } into g
                                               select g);
            var consumedProductsInnerJoinQuery =
                from userConsumedProduct in userConsumedProductsGrouped
                join prod in db.Products on userConsumedProduct.Key.ProductId equals prod.Id
                select new Models.UserProductListCompleteModel
                {
                    ProductId = prod.Id,
                    Quantity = userConsumedProduct.Sum(x => x.Quantity),
                    Barcode = prod.Barcode,
                    Brand = prod.Brand,
                    ItemType = "consumed",
                    Name = prod.Name,
                    Weight = prod.Weight,
                    Category = prod.CategoryString,
                    Price = Math.Round(prod.Price.Value * userConsumedProduct.Sum(x => x.Quantity), 2)
                };

            var shoppingListProductsInnerJoinQuery =
                from userShoppingListProduct in userShoppingList
                join prod in db.Products on userShoppingListProduct.ProductId equals prod.Id
                orderby userShoppingListProduct.Id descending
                select new Models.UserProductListCompleteModel
                {
                    Id = userShoppingListProduct.Id,
                    ProductId = prod.Id,
                    Quantity = userShoppingListProduct.Quantity ?? 1,
                    Barcode = prod.Barcode,
                    Brand = prod.Brand,
                    ItemType = "shoppingList",
                    Name = prod.Name,
                    Weight = prod.Weight,
                    Category = prod.CategoryString,
                    //Price = prod.Price
                    Price = Math.Round(prod.Price.Value * userShoppingListProduct.Quantity ?? 1, 2)
                };

            combinedLists.AddRange(shoppingListProductsInnerJoinQuery);



            foreach (var productCombined in combinedLists)
            {
                var userShoppingList2 = from m in db.StoreProducts where m.ProductId == productCombined.ProductId select m;
                if (userShoppingList2.Count() > 0)
                {
                    foreach (var storeProduct in userShoppingList2)
                    {
                        if (productCombined.PriceList == null) productCombined.PriceList = new Dictionary<string, double>();
                        productCombined.PriceList.Add(storeProduct.StoreId.ToString(), Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2));

                        if (storeProduct.Stores.Name == "Jumbo") productCombined.Url = storeProduct.Url;
                    }
                }

            }
            return combinedLists;
        }

        [HttpGet]
        public ActionResult SharedList(string id)
        {
            //id is userId
            string userId = id;
            var userProductsList = db.UserProductsList.Include(u => u.Products).Include(u => u.AspNetUsers).Where(u => u.UserId.Equals(userId) && u.ListName.Equals("In"));
            return View(userProductsList.ToList());
        }
    }
}

