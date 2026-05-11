using ClassLibrary1;
using Microsoft.AspNet.Identity;
using PagedList;
using SpiroWeb.Helpers;
using SpiroWeb.Models;
using SpiroWeb.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class ProductsController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        // GET: Products1
        [Authorize]
        public ActionResult Index(string orderBy, string searchQuery, int page = 1, bool notMine = false, bool notMineAndIsTemp = false)
        {
            //var products = (string.IsNullOrEmpty(orderBy)) ? db.Products.ToList() : db.Products.OrderBy(c => c.Name).ToList();
            IQueryable<Products> products = db.Products;

            //if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(searchQuery))
            //{
            //    products = products.Where(c => c.Name.ToLower().Contains(searchQuery)).OrderBy(c => c.Name);
            //}
            //else 

            if (!string.IsNullOrEmpty(searchQuery))
            {
                string[] _searchWords = searchQuery.ToLower().Trim(' ').Split(' ');
                products = products.Where(c => (_searchWords.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z))))
                    .OrderBy(c => c.Name);

                //products = db.Products.Where(c => c.Name.ToLower().Contains(searchQuery.ToLower()) ||
                //                             c.CategoryString.ToLower().Contains(searchQuery.ToLower()) ||
                //                             c.Brand.ToLower().Contains(searchQuery.ToLower()));
            }
            if (notMine)
            {
                products = products.Where(c => c.CreatedByUserId != "9ff8224f-17cf-49fb-b555-05779a13eb40");
            }
            if (notMineAndIsTemp)
            {
                products = products.Where(c => c.CreatedByUserId != "9ff8224f-17cf-49fb-b555-05779a13eb40" && (c.IsTemp.HasValue && c.IsTemp.Value == true));
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "name":
                        products = products.OrderBy(c => c.Name);
                        break;
                    case "insertdate":
                        products = products.OrderByDescending(c => c.InsertDate);
                        break;
                    default:
                        break;
                }
            }
            if (string.IsNullOrEmpty(searchQuery) && string.IsNullOrEmpty(orderBy))
            {
                products = products.OrderBy(c => c.Id);
            }

            ViewBag.TotalProducts = products.Count();

            //var pageNumber = page ?? 1;
            var pageNumber = page;

            if (Session["productsIndexCurrentPage"] != null && Session["goToSavedproductsIndexPage"] != null)
            {
                pageNumber = Convert.ToInt32(Session["productsIndexCurrentPage"]);
                Session["goToSavedproductsIndexPage"] = null;
            }

            try
            {
                var onePageOfProducts = products.ToPagedList(pageNumber, 25);
                Session["productsIndexCurrentPage"] = pageNumber;
                ViewBag.OnePageOfProducts = onePageOfProducts;
            }
            catch (Exception ex)
            {
                string stop = "";
            }

            ViewBag.searchQuery = searchQuery;
            return View();
        }

        // GET: Products1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var products = db.Products.Include("StoreProducts").Where(c=> c.Id.Equals(id)).FirstOrDefault();
            var products = Managers.ProductsManager.GetDTOById(id.Value);
            if (products == null)
            {
                return HttpNotFound();
            }

            //get User LIsts where this product is
            var _UserListsWhereProductIs = Managers.ProductsManager.GetUserListsWhereProductIs(products.Id);
            ViewBag.UserListsWhereProductIs = _UserListsWhereProductIs;
            return View(products);
        }

        // GET: Products1/Details/5
        public ActionResult PriceReports(int? id, string dateStart = "", string dateEnd = "")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var products = db.Products.Include("StoreProducts").Where(c=> c.Id.Equals(id)).FirstOrDefault();
            var products = Managers.ProductsManager.GetDTOById(id.Value);

            DateTime _startDate = (dateStart != string.Empty) ? DateTime.Parse(dateStart) : DateTime.MinValue;
            DateTime _endDate = (dateEnd != string.Empty) ? DateTime.Parse(dateEnd) : DateTime.MinValue;

            var _data = new ProductPriceUpdatesModel
            {
                Product = products,
                PriceUpdates = Managers.ProductsManager.GetProductPricesUpdates(id.Value, _startDate, _endDate)
            };
            return View(_data);
        }

        // GET: Products1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Barcode,Name,Price,VariableWeightPrice,CategoryString,Picture,InsertDate")] Products products)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(products);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(products);
        }

        // GET: Products1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);

            Image img = Helpers.ManageImage.byteArrayToImage(products.Picture);

            ViewBag.Logo = img;

            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: Products1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Barcode,Name,Price,VariableWeightPrice,CategoryString,Picture,InsertDate")] Products products)
        {
            if (ModelState.IsValid)
            {
                db.Entry(products).State = EntityState.Modified;
                db.SaveChanges();

                Session["goToSavedproductsIndexPage"] = true;

                return RedirectToAction("Index");
            }
            return View(products);
        }

        // GET: Products1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        public ActionResult DeleteAll(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IQueryable<UserProductsList> _UserProductsList = db.UserProductsList.Where(e => e.ProductId.Equals(id));
            if (_UserProductsList != null)
            {
                foreach (UserProductsList item in _UserProductsList)
                {
                    db.UserProductsList.Remove(item);
                }
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            db.Products.Remove(products);
            return View(products);
        }

        // POST: Products1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _deleted = Managers.ProductsManager.DeleteSafely(id);
            //_productsDeleted++;

            Products products = db.Products.Find(id);

            //remove from UserProductsConsumed
            var _userProductsConsumed = db.UserProductsConsumed.Where(c => c.ProductId == products.Id);
            db.UserProductsConsumed.RemoveRange(_userProductsConsumed);

            //remove from UserProductsConsumed
            var _productStores = db.StoreProducts.Where(c => c.ProductId == products.Id);
            db.StoreProducts.RemoveRange(_productStores);

            //remove product
            db.Products.Remove(products);

            db.SaveChanges();

            Session["goToSavedproductsIndexPage"] = true;

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

        public int AddNewProduct(Products product)
        {
            try
            {
                if (product != null)
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    return product.Id;
                }

                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public int RemoveBarcodeAssociation(string barCode)
        {
            try
            {
                var productsWithBarcode = db.Products.Where(c => c.Barcode.Equals(barCode));
                foreach (Products _product in productsWithBarcode)
                {
                    _product.Barcode = "0";
                }
                db.SaveChanges();
                return productsWithBarcode.Count();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public JsonResult GetListForAutocomplete(string term)
        {
            //Products[] matching = string.IsNullOrWhiteSpace(term) ?
            //    db.Products.ToArray() :
            //    db.Products.Where(p => p.Name.ToUpper().StartsWith(term.ToUpper())).ToArray() && prop => prop.;

            //List<Products> _matchingProducts = (from c in db.Products
            //                                                  where c.Name.ToLower().IndexOf(term.ToLower()) > 0
            //                                                  select c).ToList();

            List<Products> _matchingProducts = (from c in db.Products
                                                where c.Name.ToLower().StartsWith(term.ToLower())
                                                select c).ToList();
            if (_matchingProducts.Count() > 0)
            {

                var matchingJason = _matchingProducts.Select(m => new
                {
                    id = m.Id,
                    value = m.Name,
                    label = m.Name + " " + m.Brand + " " + m.Weight
                });

                return Json(matchingJason, JsonRequestBehavior.AllowGet);

            }
            else
            {
                if (term.Length > 2)
                {
                    List<Products> _matchingProductsAll = (from c in db.Products
                                                           where c.Name.ToLower().IndexOf(term.ToLower()) > 0
                                                           select c).ToList();
                    if (_matchingProductsAll.Count() > 0)
                    {

                        var matchingAllProductsJson = _matchingProductsAll.Select(m => new
                        {
                            id = m.Id,
                            value = m.Name,
                            label = m.Name + " " + m.Brand + " " + m.Weight
                        });

                        return Json(matchingAllProductsJson, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListForAutocompleteOfProductsWithoutBarode(string term)
        {
            List<Products> _matchingProducts = (from c in db.Products
                                                where c.Name.ToLower().StartsWith(term.ToLower()) && c.Barcode == "0"
                                                select c).ToList();
            if (_matchingProducts.Count() > 0)
            {

                var matchingJason = _matchingProducts.Select(m => new
                {
                    id = m.Id,
                    value = m.Name,
                    label = m.Name + " " + m.Brand + " " + m.Weight
                });

                return Json(matchingJason, JsonRequestBehavior.AllowGet);

            }
            else
            {
                if (term.Length > 2)
                {
                    List<Products> _matchingProductsAll = (from c in db.Products
                                                           where c.Name.ToLower().IndexOf(term.ToLower()) > 0
                                                           select c).ToList();
                    if (_matchingProductsAll.Count() > 0)
                    {

                        var matchingAllProductsJson = _matchingProductsAll.Select(m => new
                        {
                            id = m.Id,
                            value = m.Name,
                            label = m.Name + " " + m.Brand + " " + m.Weight
                        });

                        return Json(matchingAllProductsJson, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        //android get all products
        [HttpGet]
        public JsonResult GetAllProductsList(int page, string query)
        {
            List<Products> productsList = new List<Products>();
            List<dynamic> list = new List<dynamic>();
            if (string.IsNullOrEmpty(query))
            {
                //productsList = db.Products.OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6).ToList();
                productsList = db.Products.OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6).ToList();
            }
            else
            {
                //IN FUTURE MAYBE
                if (page > 0)
                {
                    productsList = db.Products.Where(c => c.Name.ToLower().Contains(query.ToLower()) ||
                        c.Brand.ToLower().Contains(query.ToLower()))
                        .OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6).ToList();
                }
                else //if -1 return all
                {
                    productsList = db.Products.Where(c => c.Name.ToLower().Contains(query.ToLower()) ||
                            c.Brand.ToLower().Contains(query.ToLower()))
                            .OrderBy(c => c.Name).ToList();
                }
            }
            foreach (Products product in productsList)
            {
                list.Add(new Products()
                {
                    Id = product.Id,
                    Barcode = product.Barcode,
                    Brand = product.Brand,
                    CategoryString = product.CategoryString,
                    InsertDate = product.InsertDate,
                    Name = product.Name,
                    //Picture = product.Picture,
                    Price = product.Price,
                    VariableWeightPrice = product.VariableWeightPrice,
                    Weight = product.Weight
                });
            }


            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllProductsListV2(int page, string query, bool withoutBarcode = false)
        {
            List<Products> productsList = new List<Products>();
            List<dynamic> list = new List<dynamic>();
            if (string.IsNullOrEmpty(query))
            {
                //productsList = db.Products.OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6).ToList();
                productsList = db.Products.OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6).ToList();
            }
            else
            {
                //IN FUTURE MAYBE
                if (page > 0)
                {
                    productsList = db.Products.Where(c => c.Name.ToLower().Contains(query.ToLower()) ||
                        c.Brand.ToLower().Contains(query.ToLower()))
                        .OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6).ToList();
                }
                else //if -1 return all
                {
                    var decomposed = query.Normalize(NormalizationForm.FormD);
                    var filtered = decomposed.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
                    var _normalizedQuery = new String(filtered.ToArray());

                    string[] _searchWords = query.ToLower().Trim(' ').Split(' ');
                    string[] _searchWordsNormalized = _normalizedQuery.ToLower().Trim(' ').Split(' ');
                    if (withoutBarcode)
                        //OBSOLETE- to delete in future
                        //productsList = db.Products.Where(c => (_searchWords.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z))) && c.Barcode.Equals("0"))
                        //    .OrderBy(c => c.Name).ToList();
                        productsList = db.Products.Where(c => _searchWords.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z)) ||
                        _searchWordsNormalized.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z)) && c.Barcode.Equals("0"))
                           .OrderBy(c => c.Name).ToList();
                    else
                    {
                        //OBSOLETE- to delete in future
                        //productsList = db.Products.Where(c => _searchWords.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z)))
                        //        .OrderBy(c => c.Name).ToList();
                        productsList = db.Products.Where(c => _searchWords.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z)) ||
                         _searchWordsNormalized.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z)))
                                .OrderBy(c => c.Name).ToList();
                    }
                }
            }
            List<Models.UserProductListCompleteModel2> _list = new List<Models.UserProductListCompleteModel2>();
            foreach (Products product in productsList)
            {
                _list.Add(new UserProductListCompleteModel2()
                {
                    ProductId = product.Id,
                    Barcode = product.Barcode,
                    Brand = product.Brand,
                    Category = product.CategoryString,
                    LastAddedDate = product.InsertDate.Value,
                    Name = product.Name,
                    Price = product.Price,
                    Weight = product.Weight
                });
            }

            foreach (var _product in _list)
            {
                var _storeProducts = from m in db.StoreProducts where m.ProductId == _product.ProductId select m;
                if (_storeProducts.Count() > 0)
                {
                    foreach (var storeProduct in _storeProducts)
                    {
                        if (_product.PriceList == null) _product.PriceList = new List<Models.StoreProduct>();
                        _product.PriceList.Add(new Models.StoreProduct
                        {
                            Id = storeProduct.Id,
                            Price = Math.Round(storeProduct.Price.Value, 2),
                            StoreId = storeProduct.StoreId,
                            Url = storeProduct.Url,
                            CreatedByUserId = storeProduct.UserId,
                            NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false),
                            UpdateDate = ((storeProduct.UpdateDate.HasValue) ? storeProduct.UpdateDate.Value : DateTime.MinValue)
                        });
                    }
                }
            }

            return Json(_list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UpdatePriceToTwoDecimalCases()
        {
            //var products = db.Products.ToList();
            //var products = db.Products.Find()

            List<Products> _matchingProductsAll = (from c in db.Products
                                                   where c.Id > 1000 && c.Id < 1200
                                                   select c).ToList();
            foreach (Products product in _matchingProductsAll)
            {
                if (product.Price.HasValue)
                {
                    string[] _splitted = product.Price.Value.ToString().Split('.');
                    if (_splitted.Length > 1)
                    {
                        if (_splitted[1].Length > 2)
                        {
                            product.Price = Math.Round(product.Price.Value, 2);
                        }
                    }
                }
            }
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetLastAddedProducts()
        {
            var lastProducts = db.Products.OrderByDescending(c => c.Id).Take(30).Include("AspNetUsers").ToList();
            List<Models.LastAddedProductModel> _LastAddedProductList = new List<Models.LastAddedProductModel>();
            foreach (var product in lastProducts)
            {
                var _LastAddedProductModel = new Models.LastAddedProductModel
                {
                    Barcode = product.Barcode,
                    Brand = product.Brand,
                    Name = product.Name,
                    Price = product.Price,
                    ProductId = product.Id
                };
                _LastAddedProductList.Add(_LastAddedProductModel);
            }

            foreach (var productCombined in _LastAddedProductList)
            {
                var userShoppingList = from m in db.StoreProducts where m.ProductId == productCombined.ProductId select m;
                if (userShoppingList.Count() > 0)
                {
                    foreach (var storeProduct in userShoppingList)
                    {
                        if (productCombined.PriceList == null) productCombined.PriceList = new Dictionary<string, double>();
                        productCombined.PriceList.Add(storeProduct.StoreId.ToString(), Math.Round(storeProduct.Price.Value, 2));
                    }
                }

            }
            return Json(_LastAddedProductList, JsonRequestBehavior.AllowGet);
        }

        public bool UpdateProductPrices(int productId)
        {
            return true;
        }



        [HttpGet]
        public JsonResult UpdateOldProducs()
        {
            //var products = db.Products.ToList();
            //var products = db.Products.Find()

            List<Products> _productsAll = (from c in db.Products
                                           select c).ToList();


            foreach (Products product in _productsAll)
            {
                if (product.Price.HasValue)
                {
                    string[] _splitted = product.Price.Value.ToString().Split('.');
                    if (_splitted.Length > 1)
                    {
                        if (_splitted[1].Length > 2)
                        {
                            product.Price = Math.Round(product.Price.Value, 2);
                        }
                    }
                }
            }
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteOldProducs() //with no StoreProducts
        {
            //var products = db.Products.ToList();
            //var products = db.Products.Find()

            List<Products> _productsAll = (from c in db.Products
                                           select c).ToList();


            int counter = 0;
            List<Products> _productsToDel = new List<Products>();
            foreach (Products product in _productsAll)
            {
                List<StoreProducts> _StoreProducts = (from c in db.StoreProducts
                                                      where c.ProductId == product.Id
                                                      select c).ToList();

                if (_StoreProducts.Count == 0)
                {
                    _productsToDel.Add(product);
                    counter++;
                }
            }

            foreach (Products _productToDel in _productsToDel)
            {
                //remove from UserProductsConsumed
                var _userProductsConsumed = db.UserProductsConsumed.Where(c => c.ProductId == _productToDel.Id);
                db.UserProductsConsumed.RemoveRange(_userProductsConsumed);

                db.Products.Remove(_productToDel);
            }
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> UpdateMetadata(int id)
        {
            string userId = User.Identity.GetUserId();
            if (id != -1)
            {
                try
                {
                    Products _newMetadata = await Managers.ProductsManager.UpdateMetadata(id, userId);
                    if (_newMetadata != null)
                        return RedirectToAction("Details", new { id = id });

                    return RedirectToAction("Details", new { id = id });
                }
                catch (Exception ex)
                {
                    Logger.Debug("Error:" + ex.InnerException.Message);
                    return RedirectToAction("Details", new { id = id });
                }

            }
            return RedirectToAction("Details", new { id = id });

        }

        // GET: Products1
        [Authorize]
        public ActionResult UpdatePricesLogs(string orderBy = "", string filter = "", string filterValue = "", int page = 1)
        {

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                IQueryable<StoreProducts> storeProducts = db.StoreProducts;
                if (!string.IsNullOrEmpty(orderBy))
                {
                    switch (orderBy.ToLower())
                    {
                        case "insertdate":
                            storeProducts = storeProducts.OrderByDescending(c => c.CreateDate);
                            break;
                        case "updatedate":
                            storeProducts = storeProducts.OrderByDescending(c => c.UpdateDate);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    storeProducts = storeProducts.OrderBy(c => c.UpdateDate);
                }

                if (!string.IsNullOrEmpty(filter))
                {
                    switch (filter.ToLower())
                    {
                        case "storeid":
                            storeProducts = storeProducts.Where(c => c.StoreId == int.Parse(filterValue));
                            break;
                        default:
                            break;
                    }
                }

                //var pageNumber = page ?? 1;
                var pageNumber = page;

                if (Session["UpdatePricesLogsCurrentPage"] != null && Session["goToUpdatePricesLogsPage"] != null)
                {
                    pageNumber = Convert.ToInt32(Session["UpdatePricesLogsCurrentPage"]);
                    Session["goToUpdatePricesLogsPage"] = null;
                }

                try
                {
                    var onePageOfProducts = storeProducts.ToPagedList(pageNumber, 25);
                    Session["UpdatePricesLogsCurrentPage"] = pageNumber;
                    ViewBag.OnePageOfProducts = onePageOfProducts;
                }
                catch (Exception ex)
                {
                    string stop = "";
                }

                ViewBag.orderBy = orderBy;
                ViewBag.filter = filter;
                return View();
            }
        }

        public ActionResult ShareProduct(int id)
        {

            if (Request.Browser.IsMobileDevice)
            {
                return Redirect("com.lisie.org://Product?productId=" + id);
            }
            else
            {
                return Redirect("https://6156328e7d57011c1d209b6f--lisie-v2.netlify.app/Product?productId=" + id);
            }
        }

        [Authorize]
        public ActionResult Reviews(int page = 1, int pageSize = 40)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                return View(db.ProductsReview.OrderByDescending(c => c.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList());
            }
        }

        //[Authorize]
        public ActionResult DayBestPriceProducts()
        {
            try
            {
                List<KeyValuePair<int, double>> _maxes = Managers.ProductsManager.GetDayBestPriceProducts();

                return View(_maxes);

            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public ActionResult AcceptTemp(int productId)
        {
            var products = Managers.ProductsManager.AcceptTemp(productId);
            return RedirectToAction("Details", new { id = productId });
        }

        public ActionResult RefuseTemp(int productId)
        {
            var products = Managers.ProductsManager.RefuseTemp(productId);
            return RedirectToAction("Details", new { id = productId });
        }

        public ActionResult OnUserLists(string orderBy, string searchQuery, int page = 1)
        {
            List<OnUserListsModel> _toRet = new List<OnUserListsModel>();

            var userProductsList = db.UserProductsList.DistinctBy(u => u.ProductId).OrderByDescending(c => c.ProductId).ToList();
            var _foundProductIds = userProductsList.Select(c => c.ProductId).ToList();
            //_distinctProductId[0].

            DateTime _lastWeek = DateTime.Now.AddDays(-7);
            DateTime _lastMonth = DateTime.Now.AddMonths(-1);

            var _ProductPricesUpdatesFailsOfProductsLastWeek = db.ProductPricesUpdatesFails
                .Where(c => _foundProductIds.Contains(c.ProductId) && c.CreateDate >= _lastWeek)
                .GroupBy(r => r.ProductId)
                .Select(grp => new ProductUpdatePriceFailsModel
                {
                    ProductId = grp.Key,
                    Count = grp.Count()
                })
                .OrderByDescending(o => o.Count).ToList();

            var _ProductPricesUpdatesFailsOfProductsLastMonth = db.ProductPricesUpdatesFails
               .Where(c => _foundProductIds.Contains(c.ProductId) && c.CreateDate >= _lastMonth)
               .GroupBy(r => r.ProductId)
               .Select(grp => new ProductUpdatePriceFailsModel
               {
                   ProductId = grp.Key,
                   Count = grp.Count()
               })
               .OrderByDescending(o => o.Count).ToList();


            foreach (var _userProductList in userProductsList)
            {
                OnUserListsModel _OnUserListsModel = new OnUserListsModel { UserProduct = _userProductList };

                var _foundInLastWeek = _ProductPricesUpdatesFailsOfProductsLastWeek.Where(c => c.ProductId == _userProductList.ProductId).FirstOrDefault();
                var _foundInLastMonth = _ProductPricesUpdatesFailsOfProductsLastMonth.Where(c => c.ProductId == _userProductList.ProductId).FirstOrDefault();
                _OnUserListsModel.LastWeekFailCount = _foundInLastWeek != null ? _foundInLastWeek.Count : 0;
                _OnUserListsModel.LastMonthFailCount = _foundInLastMonth != null ? _foundInLastMonth.Count : 0;
                _toRet.Add(_OnUserListsModel);
            }
            _toRet = _toRet.OrderByDescending(c => c.LastWeekFailCount).ToList();
            //IQueryable<ClassLibrary1.UserProductsList> userProductsList = db.UserProductsList.Include(u => u.Products).DistinctBy(c=>c.ProductId).OrderByDescending(c=>c.Id);

            var pageNumber = page;

            if (Session["storeProductsIndexCurrentPage"] != null && Session["goToSavedStoreProductsIndexPage"] != null)
            {
                pageNumber = Convert.ToInt32(Session["storeProductsIndexCurrentPage"]);
                Session["goToSavedStoreProductsIndexPage"] = null;
            }

            ViewBag.searchQuery = searchQuery;


            try
            {
                var onePageOfProducts = _toRet.ToPagedList(pageNumber, 25);
                Session["storeProductsIndexCurrentPage"] = pageNumber;
                ViewBag.OnePageOfProducts = onePageOfProducts;
                return View(_toRet.Skip((page - 1) * 25).Take(25).ToList());
            }
            catch (Exception ex)
            {
                string stop = "";
            }

            return View();

        }

        public ActionResult ProductUpdatePriceFailsWorst(int page = 1, bool lastWeek = false, bool lastMonth = false)
        {
            DateTime _lastWeek = DateTime.Now.AddDays(-7);
            DateTime _lastMonth = DateTime.Now.AddMonths(-1);
            List<ProductUpdatePriceFailsModel> userProductsList = new List<ProductUpdatePriceFailsModel>();
            if (!lastWeek && !lastMonth)
            {
                userProductsList = db.ProductPricesUpdatesFails
                .GroupBy(r => r.ProductId)
                .Select(grp => new ProductUpdatePriceFailsModel
                {
                    ProductId = grp.Key,
                    Count = grp.Count()
                })
                .OrderByDescending(o => o.Count).ToList();
            }

            if (lastWeek)
            {
                userProductsList = db.ProductPricesUpdatesFails
                .Where(c => c.CreateDate >= _lastWeek)

                .GroupBy(r => r.ProductId)
                .Select(grp => new ProductUpdatePriceFailsModel
                {
                    ProductId = grp.Key,
                    Count = grp.Count()
                })
                .OrderByDescending(o => o.Count).ToList();
            }

            if (lastMonth)
            {
                userProductsList = db.ProductPricesUpdatesFails
                .Where(c => c.CreateDate >= _lastMonth)
                .GroupBy(r => r.ProductId)
                .Select(grp => new ProductUpdatePriceFailsModel
                {
                    ProductId = grp.Key,
                    Count = grp.Count()
                })
                .OrderByDescending(o => o.Count).ToList();
            }






            //var userProductsList = db.ProductPricesUpdatesFails.DistinctBy(u => u.ProductId).
            //_distinctProductId[0].
            //var _oldestUnUpdatedStoreProducts =
            //                from prod in db.Products
            //                join storeProduct in _distinctProductId on prod.Id equals storeProduct.ProductId
            //                where storeProduct.UpdateDate != null
            //                orderby storeProduct.UpdateDate
            //                select new
            //                {
            //                    ProductId = prod.Id
            //                };

            //IQueryable<ClassLibrary1.UserProductsList> userProductsList = db.UserProductsList.Include(u => u.Products).DistinctBy(c=>c.ProductId).OrderByDescending(c=>c.Id);

            var pageNumber = page;

            if (Session["storeProductsIndexCurrentPage"] != null && Session["goToSavedStoreProductsIndexPage"] != null)
            {
                pageNumber = Convert.ToInt32(Session["storeProductsIndexCurrentPage"]);
                Session["goToSavedStoreProductsIndexPage"] = null;
            }

            try
            {
                var onePageOfProducts = userProductsList.ToPagedList(pageNumber, 25);
                Session["storeProductsIndexCurrentPage"] = pageNumber;
                ViewBag.OnePageOfProducts = onePageOfProducts;
                return View(userProductsList.Skip((page - 1) * 25).Take(25).ToList());
            }
            catch (Exception ex)
            {
                string stop = "";
            }

            //ViewBag.searchQuery = searchQuery;

            return View();
        }
        public async Task<ActionResult> SearchMarket(int id, int storeId, string search = "")
        {
            try
            {
                var _product = Managers.ProductsManager.GetById(id);
                if (_product != null)
                {
                    OnlineProducts _onlineProducts = new OnlineProducts();
                    var _searchResults = string.IsNullOrEmpty(search) ?
                        await _onlineProducts.GetSearchResultsWithNoSeparatorsOfStore(_product.Name + " " + _product.Brand, storeId)
                        :
                        await _onlineProducts.GetSearchResultsWithNoSeparatorsOfStore(search, storeId);

                    SearchMarketModel _ret = new SearchMarketModel
                    {
                        Product = _product,
                        SearchResults = _searchResults,
                        StoreId = storeId,
                        Search = _product.Name + " " + _product.Brand
                    };
                    return View(_ret);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }

        public async Task<ActionResult> ConfirmMarket(int productId, int storeId, string url, string onlineProductId, string userId = "9ff8224f-17cf-49fb-b555-05779a13eb40")
        {
            try
            {
                var _product = Managers.ProductsManager.GetById(productId);
                if (_product != null)
                {
                    OnlineProducts _onlineProducts = new OnlineProducts();
                    var _searchResult = await _onlineProducts.GetMarketProductByOnlineId(storeId, onlineProductId);
                    if (_searchResult == null)
                    {
                        _searchResult = await _onlineProducts.GetMarketProductByUrl(storeId, url);
                    }

                    SearchMarketModel _ret = new SearchMarketModel
                    {
                        Product = _product,
                        SearchResults = new List<LisieStores.Extensibility.ProductSearchResult>
                        {
                            _searchResult
                        },
                        StoreId = storeId,
                        Search = _product.Name + " " + _product.Brand
                    };
                    return View(_ret);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }
        public async Task<ActionResult> SetMarket(int productId, int storeId, string url, string onlineProductId, string userId = "9ff8224f-17cf-49fb-b555-05779a13eb40")
        {
            try
            {
                var _selectedResults = new List<LisieStores.Extensibility.ProductSearchResult>
                {
                    new LisieStores.Extensibility.ProductSearchResult { OnlineProductId = onlineProductId, Url = url, StoreId = storeId }
                };
                ProductItemNew _newProductMarket = new ProductItemNew
                {
                    ProductId = productId,
                    SelectedResults = _selectedResults
                };

                JsonApiResponse _response = await Managers.ProductsManager.UpdateStoresV2(userId, _newProductMarket);

                if (_response.Success)
                {

                }
                return RedirectToAction("Details", new { id = productId });
            }
            catch (Exception) { }
            return View();
        }

        public ActionResult SearchForProductsWithBarcodeMismatch(int page = 1, bool lastWeek = false, bool lastMonth = false)
        {
            var userProductsList =
                  from product in db.Products
                  join storePrd in db.StoreProducts on product.Id equals storePrd.ProductId
                  where !string.IsNullOrEmpty(storePrd.Barcode) && product.Barcode != storePrd.Barcode
                  orderby product.Id descending
                  select new UserProductListCompleteModel2
                  {
                      Id = product.Id,
                      Brand = product.Brand,
                      Barcode = product.Barcode,
                      IsTemp = product.IsTemp,
                      CreatedByUserId = product.CreatedByUserId,
                      Name = product.Name,
                      ProductId = product.Id,
                      Weight = product.Weight
                  };

            var _toList = userProductsList.ToList();


            var pageNumber = page;

            if (Session["storeProductsIndexCurrentPage"] != null && Session["goToSavedStoreProductsIndexPage"] != null)
            {
                pageNumber = Convert.ToInt32(Session["storeProductsIndexCurrentPage"]);
                Session["goToSavedStoreProductsIndexPage"] = null;
            }

            try
            {
                var onePageOfProducts = _toList.ToPagedList(pageNumber, 25);
                Session["storeProductsIndexCurrentPage"] = pageNumber;
                ViewBag.OnePageOfProducts = onePageOfProducts;
                return View(userProductsList.Skip((page - 1) * 25).Take(25).ToList());
            }
            catch (Exception ex)
            {
                string stop = "";
            }

            //ViewBag.searchQuery = searchQuery;

            return View();
        }


        //done and runned
        public ActionResult FixWeightBarcodes(int page = 1)
        {
            //update storeproduct and Product
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _tempProducts = db.Products.Where(c => c.Barcode.Substring(c.Barcode.Length - 6, 5) == "00000" && c.Barcode.Substring(c.Barcode.Length - 1, 1) != "0").Select(c => new UserProductListCompleteModel2
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Barcode = c.Barcode,
                    IsTemp = c.IsTemp,
                    CreatedByUserId = c.CreatedByUserId,
                    Name = c.Name,
                    ProductId = c.Id,
                    Weight = c.Weight
                }).ToList();
                //foreach (var _product in _tempProducts)
                //{
                //    if (_product.Barcode[_product.Barcode.Length - 1] != '0')
                //    {
                //        string i = "sd";
                //    }
                //    //var _storeProducts = db.StoreProducts.Where(c => c.ProductId == _product.Id).OrderBy(c => c.Id);
                //    //int _count = _storeProducts.Count();
                //    //foreach (var _storeProduct in _storeProducts)
                //    //{
                //    //    _storeProduct.IsTemp = false;
                //    //    break;
                //    //}
                //}
                //db.SaveChanges();


                var _toList = _tempProducts;

                ViewBag.TotalProducts = _toList.Count;

                var pageNumber = page;

                if (Session["storeProductsIndexCurrentPage"] != null && Session["goToSavedStoreProductsIndexPage"] != null)
                {
                    pageNumber = Convert.ToInt32(Session["storeProductsIndexCurrentPage"]);
                    Session["goToSavedStoreProductsIndexPage"] = null;
                }

                try
                {
                    var onePageOfProducts = _toList.ToPagedList(pageNumber, 25);
                    Session["storeProductsIndexCurrentPage"] = pageNumber;
                    ViewBag.OnePageOfProducts = onePageOfProducts;
                    return View(_tempProducts.Skip((page - 1) * 25).Take(25).ToList());
                }
                catch (Exception ex)
                {
                    string stop = "";
                }

                //ViewBag.searchQuery = searchQuery;

                return View();

            }
        }

        //done and runned
        public ActionResult FixElCorteInglesLastPrice()
        {
            //update storeproduct and Product
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _tempStoreProducts = db.StoreProducts.Where(c => c.StoreId == 7 && (c.Price.HasValue && c.Price.Value.Equals(17.54)));
                foreach (var _product in _tempStoreProducts)
                {
                    try
                    {
                        var _lastPriceCHange = db.ProductPricesUpdates.Where(c => c.ProductId == _product.ProductId && c.StoreId == 7).OrderByDescending(c => c.CreateDate).FirstOrDefault();
                        _product.Price = _lastPriceCHange.OldPrice;
                        db.ProductPricesUpdates.Remove(_lastPriceCHange);
                    }
                    catch (Exception)
                    {

                    }

                    //break;
                    //db.SaveChanges();
                }
                db.SaveChanges();


                //var _toList = _tempProducts;

                //ViewBag.TotalProducts = _toList.Count;

                //var pageNumber = page;

                //if (Session["storeProductsIndexCurrentPage"] != null && Session["goToSavedStoreProductsIndexPage"] != null)
                //{
                //    pageNumber = Convert.ToInt32(Session["storeProductsIndexCurrentPage"]);
                //    Session["goToSavedStoreProductsIndexPage"] = null;
                //}

                //try
                //{
                //    var onePageOfProducts = _toList.ToPagedList(pageNumber, 25);
                //    Session["storeProductsIndexCurrentPage"] = pageNumber;
                //    ViewBag.OnePageOfProducts = onePageOfProducts;
                //    return View(_tempProducts.Skip((page - 1) * 25).Take(25).ToList());
                //}
                //catch (Exception ex)
                //{
                //    string stop = "";
                //}

                ////ViewBag.searchQuery = searchQuery;

                return RedirectToAction("Index");
                //return View();

            }
        }

    }

    public class SearchMarketModel
    {
        public Products Product { get; set; }
        public List<LisieStores.Extensibility.ProductSearchResult> SearchResults { get; set; }
        public int StoreId { get; set; }
        public string Search { get; set; }
    }

    public class ProductUpdatePriceFailsModel
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
    }

    public class OnUserListsModel
    {
        public ClassLibrary1.UserProductsList UserProduct { get; set; }
        public int LastWeekFailCount { get; set; }
        public int LastMonthFailCount { get; set; }
    }
}