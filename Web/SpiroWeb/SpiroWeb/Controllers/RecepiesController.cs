using ClassLibrary1;
using Microsoft.AspNet.Identity;
using PagedList;
using SpiroWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SpiroWeb.Controllers
{
    public class RecepiesController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        // GET: Recepies
        public ActionResult Index()
        {
            return View(db.Recepies.ToList());
        }

        // GET: Recepies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recepies recepies = db.Recepies.Find(id);
            if (recepies == null)
            {
                return HttpNotFound();
            }
            Session["lastRecepyAccessed"] = id;
            ViewBag.userId = User.Identity.GetUserId();
            return View(recepies);
        }

        // GET: Recepies/Create
        public ActionResult Create()
        {
            return View(new Recepies());
        }

        // POST: Recepies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Recepies recepies, string recipeIngredientsJson, string recipeDirectionsJson, HttpPostedFileBase postedFile)
        {
            int _recipedId = CreateRecipe(recepies, recipeIngredientsJson, recipeDirectionsJson, postedFile);
            return RedirectToAction("Edit", "Recepies", new { id = _recipedId });
        }

        // GET: Recepies/Edit/5
        public ActionResult Edit(int? id)
        {
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Recepies recepies = db2.Recepies.Where(c => c.Id == id).Include("RecipeIngredients").FirstOrDefault();
                if (recepies == null)
                {
                    return HttpNotFound();
                }
                //Map RecipeIngredients to ViewBag
                var _recipeIngredientsViewModel = recepies.RecipeIngredients.Select(c => new RecipeIngredientViewModel
                {
                    Id = c.Id,
                    IngredientId = c.IngredientId.Value,
                    Amount = c.Amount.Trim(),
                    Information = c.Information,
                    Name = c.Ingredients.Name,
                    Units = c.Units.Trim()
                });

                var _recipeDirectionsViewModel = recepies.RecipeDirections.Select(c => new RecipeDirectionsViewModel
                {
                    Id = c.Id,
                    Direction = c.Direction,
                    StepNumber = c.StepNumber
                });

                ViewBag.RecipeIngredients = _recipeIngredientsViewModel.ToList();
                ViewBag.RecipeDirections = _recipeDirectionsViewModel.ToList();

                var _results = db2.Recepies.Where(c => !string.IsNullOrEmpty(c.Category)).DistinctBy(c => c.Category);
                if (_results.Count() > 0)
                {
                    var _resultsJson = _results.Select(m => m.Category);
                    ViewBag.RecepiesCategories = _resultsJson.ToList();
                }


                return View(recepies);
            }

        }

        // POST: Recepies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(Recepies recepies, string recipeIngredientsJson, string recipeDirectionsJson, HttpPostedFileBase postedFile)
        {
            UpdateRecipe(recepies, recipeIngredientsJson, recipeDirectionsJson, postedFile);
            return RedirectToAction("Edit", "Recepies", new { id = recepies.Id });
        }

        // GET: Recepies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recepies recepies = db.Recepies.Find(id);
            if (recepies == null)
            {
                return HttpNotFound();
            }
            return View(recepies);
        }

        // POST: Recepies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Recepies recepies = db.Recepies.Find(id);
            var _RecipeDirections = db.RecipeDirections.Where(c => c.RecipeId == id);
            var _RecipeIngredients = db.RecipeIngredients.Where(c => c.RecipeId == id);
            db.RecipeDirections.RemoveRange(_RecipeDirections);
            db.RecipeIngredients.RemoveRange(_RecipeIngredients);
            db.Recepies.Remove(recepies);
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



        [Authorize]
        public ActionResult AssociateProduct(int? page, string orderBy, string searchQuery, int ingredientId)
        {
            //var products = (string.IsNullOrEmpty(orderBy)) ? db.Products.ToList() : db.Products.OrderBy(c => c.Name).ToList();
            //using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            //{
            IQueryable<Products> products = Enumerable.Empty<Products>().AsQueryable();

            //List<Products> products = new List<Products>();
            var pageNumber = page ?? 1;

            if (Session["productsIndexCurrentPage"] != null && Session["goToSavedproductsIndexPage"] != null)
            {
                pageNumber = Convert.ToInt32(Session["productsIndexCurrentPage"]);
                Session["goToSavedproductsIndexPage"] = null;
            }

            if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(searchQuery))
            {
                products = db.Products.Where(c => c.Name.ToLower().Contains(searchQuery)).OrderBy(c => c.Name);
            }
            else if (!string.IsNullOrEmpty(orderBy))
            {
                products = db.Products.OrderBy(c => c.Name);
            }
            else if (!string.IsNullOrEmpty(searchQuery))
            {
                string[] _searchWords = searchQuery.ToLower().Trim(' ').Split(' ');
                products = db.Products.Where(c => (_searchWords.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z))))
                    .OrderBy(c => c.Name);
                //products = db.Products.Where(c => c.Name.ToLower().Contains(searchQuery.ToLower()) ||
                //                             c.CategoryString.ToLower().Contains(searchQuery.ToLower()) ||
                //                             c.Brand.ToLower().Contains(searchQuery.ToLower())).OrderBy(c => c.Name).Skip((pageNumber - 1) * 25).Take(25);

            }
            else
            {
                products = db.Products.OrderBy(c => c.Name);
            }



            //var onePageOfProducts = products.Skip((pageNumber - 1) * 25).Take(25);
            Session["productsIndexCurrentPage"] = pageNumber;


            ViewBag.OnePageOfProducts = products.ToPagedList(pageNumber, 25);
            ViewBag.ingredientId = ingredientId;
            return View();
            //}
        }
        public ActionResult RemoveProductAssociation(int id)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _ingredientProduct = db.IngredientProducts.Where(c => c.Id == id).FirstOrDefault();
                if (_ingredientProduct != null)
                {
                    db.IngredientProducts.Remove(_ingredientProduct);
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = Session["lastRecepyAccessed"] });
                }
                return RedirectToAction("Index");
            }

            //var onePageOfProducts = products.Skip((pageNumber - 1) * 25).Take(25);
            //Session["productsIndexCurrentPage"] = pageNumber;


            //ViewBag.OnePageOfProducts = products.ToPagedList(pageNumber, 25);
            //ViewBag.ingredientId = ingredientId;
            //return View();
            //}
        }

        public ActionResult FinalizeIngredientAssociation(int ingredientId, int productId)
        {

            //var exists = d
            string userId = User.Identity.GetUserId();

            var ingredientProduct = Managers.RecepiesManager.AssociateProductToIngredient(userId, productId, ingredientId);

            if (Session["lastRecepyAccessed"] != null)
                return Redirect("/Recepies/Details/" + Session["lastRecepyAccessed"]);
            else return Redirect("/Recepies");
        }

        public ActionResult AddRecipeToUserShoppingCart(int recipeId)
        {
            string userId = User.Identity.GetUserId();
            var recipe = db.Recepies.Where(c => c.Id == recipeId).FirstOrDefault();
            if (recipe != null)
            {
                foreach (var _recipeIngredient in recipe.RecipeIngredients)
                {
                    var _ingredientProduct = db.IngredientProducts.Where(c => c.IngredientId == _recipeIngredient.IngredientId && c.UserId == userId).FirstOrDefault();
                    if (_ingredientProduct != null)
                    {
                        ShoppingCartController _ShoppingCartController = new ShoppingCartController();
                        int userProductListId = _ShoppingCartController.AddProductToShoppingCart(_ingredientProduct.ProductId, 1, null, true, userId);
                    }
                }

                WarnMeOfAddRecipeToUserShoppingCart(userId, recipe.Name);
            }


            if (Session["lastRecepyAccessed"] != null)
                return Redirect("/Recepies/Details/" + Session["lastRecepyAccessed"]);
            else return Redirect("/Recepies");
        }



        public void WarnMeOfAddRecipeToUserShoppingCart(string userId, string recipeName)
        {
            //get user last device token

            var user = db.AspNetUsers.Where(c => c.Id.Equals(userId)).First();
            if (user != null)
            {
                DataManager.UserDevicesManager _userDevicesManager = new DataManager.UserDevicesManager();
                List<ClassLibrary1.UserDevices> _userDevicesTokens = _userDevicesManager.GetUserDevicesTokens(userId);

                if (_userDevicesTokens.Count() > 0)
                {
                    foreach (ClassLibrary1.UserDevices _userDevice in _userDevicesTokens)
                    {
                        Helpers.FirebaseAndroid.SendNotificationToAndroidPhone(_userDevice.DeviceToken, "addRecipeToUserShoppingCart:Receita " + recipeName + " adicionada á lista");
                    }
                }
            }

        }

        public ActionResult GetAll(string userId)
        {
            Managers.InteractionsManager.Add(userId, "/Recepies/GetAll", "");

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _recepies = db.Recepies.Where(c => c.Visible.HasValue && c.Visible.Value).OrderBy(c => c.Category).Include("RecipeIngredients");

                //List<RecipeModel> _recepiesList = db.Recepies.Where(c => c.Visible.HasValue && c.Visible.Value).OrderBy(c => c.Category).Select(c => new RecipeModel
                //{
                //    Id = c.Id,
                //    Name = c.Name,
                //    Category = c.Category,
                //    Cuisine = c.Cuisine,
                //    ItemType = "recipe"
                //}).ToList();

                List<RecipeModel> _recepiesList = _recepies.Select(c => new RecipeModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Category = c.Category,
                    Cuisine = c.Cuisine,
                    ItemType = "recipe"
                }).ToList();

                List<RecipeModel> _recepiesFinal = new List<RecipeModel>();
                string _currentCategory = "";

                int _index = 0;
                foreach (var __recipe in _recepies)
                {
                    if (string.IsNullOrEmpty(__recipe.Category)) continue;
                    if (_currentCategory != __recipe.Category)
                    {
                        _currentCategory = __recipe.Category;
                        _recepiesFinal.Add(new RecipeModel
                        {
                            Category = __recipe.Category,
                            ItemType = "category"
                        });
                    }
                    List<int> _ingredientsIds = __recipe.RecipeIngredients.Select(c => c.IngredientId.Value).ToList();

                    if (_ingredientsIds.Count > 0)
                    {
                        var __ingredientsProductsId = db.IngredientProducts.Where(c => _ingredientsIds.Contains(c.IngredientId))
                                                    // && c.UserId = userId
                                                    .ToList();
                        List<int> _ingredientsProductsId = db.IngredientProducts.Where(c => _ingredientsIds.Contains(c.IngredientId)).Select(c => c.ProductId)
                                                     // && c.UserId = userId
                                                     .ToList();

                        if (_ingredientsProductsId.Count > 0)
                        {
                            var _ingredientsInInventory = db.UserProductsList
                                     .Where(u => u.UserId.Equals(userId))
                                     .Where(u => u.ListName.ToLower().Equals("inventory"))
                                     .Where(u => _ingredientsProductsId.Contains(u.ProductId)).Count();

                            var _ingredientsInShoppingList = db.UserProductsList
                                     .Where(u => u.UserId.Equals(userId))
                                     .Where(u => u.ListName.ToLower().Equals("in"))
                                     .Where(u => _ingredientsProductsId.Contains(u.ProductId)).Count();

                            _recepiesList[_index].TotalIngredients = _ingredientsIds.Count;
                            _recepiesList[_index].ExistingIngredients = _ingredientsInInventory;
                            _recepiesList[_index].InShoppingListIngredients = _ingredientsInShoppingList;
                            _recepiesList[_index].MissingIngredients = _ingredientsIds.Count - _ingredientsInInventory;
                        }
                    }

                    _recepiesFinal.Add(_recepiesList[_index]);

                    _index++;
                }

                //foreach (var _recipe in _recepies)
                //{
                //    if (string.IsNullOrEmpty(_recipe.Category)) continue;

                //    if (_currentCategory != _recipe.Category)
                //    {
                //        _currentCategory = _recipe.Category;
                //        _recepiesFinal.Add(new RecipeModel
                //        {
                //            Category = _recipe.Category,
                //            ItemType = "category"
                //        });
                //    }

                //    var idList = ids.ToList();
                //    return CurrentSession.Query<LineItem>()
                //                         .Where(item => idList.Contains(item.Id))
                //                         .ToList();
                //    _recepiesFinal.Add(_recipe);

                //}
                return Json(_recepiesFinal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddRecipeToUserShoppingCartJson(string userId, int recipeId, bool addIngredientIfExistsInInventory)
        {
            Managers.InteractionsManager.Add(userId, "/Recepies/AddRecipeToUserShoppingCartJson", "RecipeId - " + recipeId);

            var recipe = db.Recepies.Where(c => c.Id == recipeId).FirstOrDefault();

            List<int> _userProductsIdsAdded = new List<int>();
            List<int> _userProductsIdsNotAdded = new List<int>();
            if (recipe != null)
            {
                foreach (var _recipeIngredient in recipe.RecipeIngredients)
                {
                    //TODO - user associated ingredients
                    //var _ingredientProduct = db.IngredientProducts.Where(c => c.IngredientId == _recipeIngredient.IngredientId && c.UserId == userId).FirstOrDefault();
                    var _ingredientProduct = db.IngredientProducts.Where(c => c.IngredientId == _recipeIngredient.IngredientId).FirstOrDefault();
                    if (_ingredientProduct != null)
                    {
                        if (!addIngredientIfExistsInInventory)
                        {
                            //check if exists in inventory
                            UserProductsList _inventoryProduct = (from c in db.UserProductsList
                                                                  where c.ProductId.Equals(_ingredientProduct.ProductId) &&
                                                                  c.UserId.Equals(userId) &&
                                                                  c.ListName.ToLower().Equals("inventory")
                                                                  select c).FirstOrDefault();
                            if (_inventoryProduct != null)
                            {
                                _userProductsIdsNotAdded.Add(_inventoryProduct.ProductId);
                                continue;
                            }
                        }

                        ShoppingCartController _ShoppingCartController = new ShoppingCartController();
                        int userProductListId = _ShoppingCartController.AddProductToShoppingCart(_ingredientProduct.ProductId, 1, null, true, userId);
                        if (userProductListId != -1)
                            _userProductsIdsAdded.Add(_ingredientProduct.ProductId);
                        else
                            _userProductsIdsNotAdded.Add(_ingredientProduct.ProductId);
                    }
                }

                //WarnMeOfAddRecipeToUserShoppingCart(userId, recipe.Name);
            }


            return Json(new
            {
                ProductsAdded = _userProductsIdsAdded,
                ProductsNotAdded = _userProductsIdsNotAdded
            },
                JsonRequestBehavior.AllowGet);
        }

        //TODO
        public ActionResult GetRecipeUserProductsToAdd(int recipeId)
        {
            string userId = User.Identity.GetUserId();
            var recipe = db.Recepies.Where(c => c.Id == recipeId).FirstOrDefault();

            List<int> _userProductsIdsAdded = new List<int>();
            if (recipe != null)
            {
                foreach (var _recipeIngredient in recipe.RecipeIngredients)
                {
                    var _ingredientProduct = db.IngredientProducts.Where(c => c.IngredientId == _recipeIngredient.IngredientId && c.UserId == userId).FirstOrDefault();
                    if (_ingredientProduct != null)
                    {
                        //check if exists in inventory

                        ShoppingCartController _ShoppingCartController = new ShoppingCartController();
                        int userProductListId = _ShoppingCartController.AddProductToShoppingCart(_ingredientProduct.ProductId, 1, null, true, userId);
                        _userProductsIdsAdded.Add(userProductListId);
                    }
                }

                WarnMeOfAddRecipeToUserShoppingCart(userId, recipe.Name);
            }


            return Json(_userProductsIdsAdded, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRecipe(string userId, int recipeId)
        {
            Managers.InteractionsManager.Add(userId, "/Recepies/GetRecipe", recipeId.ToString());

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {

                var _recipe = db.Recepies.Where(c => c.Id == recipeId).Include("RecipeIngredients").Include("RecipeDirections").FirstOrDefault();
                if (_recipe != null)
                {
                    var __recipe = new RecipeViewModel
                    {
                        Id = _recipe.Id,
                        Category = _recipe.Category,
                        Commentary = _recipe.Commentary,
                        Cuisine = _recipe.Cuisine,
                        Description = _recipe.Description,
                        Name = _recipe.Name,
                        Rating = _recipe.Rating.HasValue ? _recipe.Rating.Value : 0,
                        TimeCooking = _recipe.TimeCooking.HasValue ? _recipe.TimeCooking.Value : 0,
                        TimePreparing = _recipe.TimePreparing.HasValue ? _recipe.TimePreparing.Value : 0,
                        TimeReady = _recipe.TimeReady.HasValue ? _recipe.TimeReady.Value : 0,
                        Visible = _recipe.Visible.HasValue ? _recipe.Visible.Value : false
                    };

                    //Fill Directions
                    __recipe.RecipeDirections = _recipe.RecipeDirections.Select(c => new RecipeDirectionsViewModel
                    {
                        Direction = c.Direction,
                        Id = c.Id,
                        StepNumber = c.StepNumber
                    }).ToList();

                    __recipe.RecipeIngredients = new List<RecipeIngredientViewModel>();

                    //Fill Ingredients
                    foreach (var _ingredient in _recipe.RecipeIngredients)
                    {
                        //Add item ingredient
                        var __ingredient = new RecipeIngredientViewModel
                        {
                            Id = _ingredient.Id,
                            IngredientId = _ingredient.IngredientId.HasValue ? _ingredient.IngredientId.Value : 0,
                            Amount = _ingredient.Amount + " " + _ingredient.Units,
                            Information = _ingredient.Information,
                            ItemType = "ingredient"
                        };

                        //Get Ingredient Name
                        var _ingredientName = db.Ingredients.Where(c => c.Id == _ingredient.IngredientId).Select(c => c.Name).FirstOrDefault();
                        __ingredient.Name = _ingredientName;

                        //add to return list the ingredient
                        __recipe.RecipeIngredients.Add(__ingredient);

                        //Add item ingredientProduct
                        var _ingredientProducts = db.IngredientProducts.Where(c => c.IngredientId == _ingredient.IngredientId)
                                                  // && c.UserId = userId
                                                  .ToList();

                        bool _ingredientsInInventory = false;
                        bool _ingredientsInShoppingList = false;
                        foreach (IngredientProducts _ingredientProduct in _ingredientProducts)
                        {
                            //Check if product exists in user inventory
                            var _ingredientsInInventoryCount = db.UserProductsList
                                     .Where(u => u.UserId.Equals(userId))
                                     .Where(u => u.ListName.ToLower().Equals("inventory"))
                                     .Where(u => u.ProductId == _ingredientProduct.ProductId).Count();

                            //Check if product exists in shopping list
                            var _ingredientsInShoppingListCount = db.UserProductsList
                                     .Where(u => u.UserId.Equals(userId))
                                     .Where(u => u.ListName.ToLower().Equals("in"))
                                     .Where(u => u.ProductId == _ingredientProduct.ProductId).Count();

                            if (_ingredientsInInventoryCount > 0 && !_ingredientsInInventory)
                                _ingredientsInInventory = true;

                            if (_ingredientsInShoppingListCount > 0 && !_ingredientsInShoppingList)
                                _ingredientsInShoppingList = true;


                            //add to return list ingredient product
                            var _product = db.Products.Where(c => c.Id == _ingredientProduct.ProductId).FirstOrDefault();
                            if (_product != null)
                            {
                                var _ingredientProductViewModel = new RecipeIngredientProductViewModel
                                {
                                    Id = _ingredientProduct.Id,
                                    IngredientId = _ingredientProduct.IngredientId,
                                    Brand = _product.Brand,
                                    ProductId = _ingredientProduct.ProductId,
                                    Name = _product.Name,
                                    Quantity = _ingredientProduct.Quantity.HasValue ? _ingredientProduct.Quantity.Value : 1,
                                    Weight = _product.Weight
                                };
                                var _productStores = from m in db.StoreProducts where m.ProductId == _product.Id select m;
                                if (_productStores.Count() > 0)
                                {
                                    if (_ingredientProductViewModel.PriceList == null) _ingredientProductViewModel.PriceList = new List<Models.StoreProduct>();
                                    foreach (var storeProduct in _productStores)
                                    {
                                        _ingredientProductViewModel.PriceList.Add(new Models.StoreProduct
                                        {
                                            Id = storeProduct.Id,
                                            Price = Math.Round(storeProduct.Price.Value, 2),
                                            StoreId = storeProduct.StoreId,
                                            Url = storeProduct.Url,
                                            CreatedByUserId = storeProduct.UserId,
                                            NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
                                        });
                                    }
                                }

                                __recipe.RecipeIngredients.Add(new RecipeIngredientViewModel
                                {
                                    Id = _ingredient.Id,
                                    IngredientId = _ingredient.IngredientId.HasValue ? _ingredient.IngredientId.Value : 0,
                                    ItemType = "ingredientProduct",
                                    Product = _ingredientProductViewModel
                                });
                            }
                        }
                        __ingredient.ExistsInInventory = _ingredientsInInventory;
                        __ingredient.ExistsInShoppingList = _ingredientsInShoppingList;
                    }

                    return Json(__recipe, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
        }

        //public ActionResult AddRecipeProductsToUserShoppingCart(string userId, List<int> productsIds)
        //{
        //    Managers.InteractionsManager.Add(userId, "/Recepies/AddRecipeProductsToUserShoppingCart", "");
        //    List<int> _userProductsIdsAdded = new List<int>();
        //    List<int> _userProductsIdsNotAdded = new List<int>();
        //    try
        //    {

        //        foreach (var _productId in productsIds)
        //        {
        //                ShoppingCartController _ShoppingCartController = new ShoppingCartController();
        //                int userProductListId = _ShoppingCartController.AddProductToShoppingCart(_productId, 1, null, true, userId);
        //                if (userProductListId != -1)
        //                    _userProductsIdsAdded.Add(_productId);
        //                else
        //                    _userProductsIdsNotAdded.Add(_productId);
        //        }

        //        //WarnMeOfAddRecipeToUserShoppingCart(userId, recipe.Name);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Debug("Error:" + ex.InnerException.Message);
        //    }
        //    return Json(new
        //    {
        //        ProductsAdded = _userProductsIdsAdded,
        //        ProductsNotAdded = _userProductsIdsNotAdded
        //    },
        //       JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult UploadRecipeImage(HttpPostedFileBase postedFile, int recipeId)
        {
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                Recepies _recipe = db.Recepies.Where(c => c.Id == recipeId).FirstOrDefault();
                if (_recipe != null)
                {
                    _recipe.Picture = bytes;
                    db.SaveChanges();
                    //return Json(_recipe, JsonRequestBehavior.AllowGet);
                }
            }
            //return Json(null, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Edit", new { id = recipeId });
        }

        [HttpGet]
        public JsonResult GetIngredientsAutocomplete(string term)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _ingredients = db.Ingredients.Where(c => c.Name.ToLower().IndexOf(term.ToLower()) > -1);
                if (_ingredients.Count() > 0)
                {
                    var _ingredientsJson = _ingredients.Select(m => new
                    {
                        id = m.Id,
                        value = m.Name,
                        label = m.Name
                    });
                    return Json(_ingredientsJson.ToList(), JsonRequestBehavior.AllowGet);
                }
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetIngredientUnitsAutocomplete(string term)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _results = db.RecipeIngredients.Where(c => c.Units.ToLower().IndexOf(term.ToLower()) > -1);
                if (_results.Count() > 0)
                {
                    var _resultsJson = _results.Select(m => new
                    {
                        id = m.Id,
                        value = m.Units.Trim(),
                        label = m.Units.Trim()
                    }).DistinctBy(c => c.label);
                    return Json(_resultsJson.ToList(), JsonRequestBehavior.AllowGet);
                }
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCategoriesAutocomplete(string term)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _results = db.Recepies.Where(c => !string.IsNullOrEmpty(c.Category)).DistinctBy(c => c.Category);
                if (_results.Count() > 0)
                {
                    var _resultsJson = _results.Select(m => new
                    {
                        id = m.Id,
                        value = m.Category.Trim(),
                        label = m.Category.Trim()
                    });
                    return Json(_resultsJson.ToList(), JsonRequestBehavior.AllowGet);
                }
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        public int UpdateRecipe(Recepies recepies, string recipeIngredientsJson, string recipeDirectionsJson, HttpPostedFileBase postedFile)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                Recepies _recipe = db.Recepies.Where(c => c.Id == recepies.Id).FirstOrDefault();
                if (_recipe != null)
                {
                    _recipe.Name = recepies.Name;
                    _recipe.Rating = recepies.Rating;
                    _recipe.TimeCooking = recepies.TimeCooking;
                    _recipe.TimePreparing = recepies.TimePreparing;
                    _recipe.TimeReady = recepies.TimeReady;
                    _recipe.Yield = recepies.Yield;
                    _recipe.Visible = recepies.Visible;
                    _recipe.Category = recepies.Category;
                    _recipe.Commentary = recepies.Commentary;
                    _recipe.Cuisine = recepies.Cuisine;
                    _recipe.Description = recepies.Description;
                    _recipe.Visible = recepies.Visible;

                    if (postedFile != null)
                    {
                        byte[] bytes;
                        using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                        {
                            bytes = br.ReadBytes(postedFile.ContentLength);
                        }
                        _recipe.Picture = bytes;
                    }

                    var serializer = new JavaScriptSerializer();
                    List<SpiroWeb.Models.RecipeIngredientViewModel> _recipeIngredients = serializer.Deserialize<List<SpiroWeb.Models.RecipeIngredientViewModel>>(recipeIngredientsJson);
                    List<SpiroWeb.Models.RecipeDirectionsViewModel> _recipeDirections = serializer.Deserialize<List<SpiroWeb.Models.RecipeDirectionsViewModel>>(recipeDirectionsJson);

                    if (_recipeIngredients != null && _recipeIngredients.Count() > 0)
                    {
                        foreach (var _recipeIngredient in _recipeIngredients)
                        {
                            if (_recipeIngredient.ItemType != null && _recipeIngredient.ItemType == "remove") //remove ingredient from recipe
                            {
                                var _toRemove = _recipe.RecipeIngredients.Where(c => c.Id == _recipeIngredient.Id).FirstOrDefault();
                                if (_toRemove != null)
                                {
                                    db.RecipeIngredients.Remove(_toRemove);
                                    db.SaveChanges();
                                }
                            }
                            else if (_recipeIngredient.Id == 0) //create new ingredient 
                            {
                                var _ingredientId = _recipeIngredient.IngredientId;

                                //If Ingredient is new, check if already exists in db, if not create new Ingredient
                                if (_recipeIngredient.IngredientId == -1)
                                {
                                    var _ingredientExists = db.Ingredients.Where(c => c.Name.ToLower().Trim() == _recipeIngredient.Name.ToLower().Trim()).FirstOrDefault();
                                    if (_ingredientExists != null) //it exists
                                    {
                                        _ingredientId = _ingredientExists.Id;
                                    }
                                    else //create new one
                                    {
                                        Ingredients _newIngredient = new Ingredients
                                        {
                                            Name = _recipeIngredient.Name.Trim()
                                        };
                                        db.Ingredients.Add(_newIngredient);
                                        db.SaveChanges();
                                        _ingredientId = _newIngredient.Id;
                                    }
                                }
                                RecipeIngredients _toCreate = new RecipeIngredients
                                {
                                    Amount = _recipeIngredient.Amount,
                                    Information = _recipeIngredient.Information,
                                    IngredientId = _ingredientId,
                                    RecipeId = _recipe.Id,
                                    Units = _recipeIngredient.Units
                                };
                                _recipe.RecipeIngredients.Add(_toCreate);
                                db.SaveChanges();
                            }
                            else if (_recipeIngredient.ItemType == null) //update ingredient
                            {
                                var _toEdit = _recipe.RecipeIngredients.Where(c => c.Id == _recipeIngredient.Id).FirstOrDefault();
                                if (_toEdit != null)
                                {
                                    _toEdit.Amount = _recipeIngredient.Amount;
                                    _toEdit.Information = _recipeIngredient.Information;
                                    _toEdit.IngredientId = _recipeIngredient.IngredientId;
                                    _toEdit.RecipeId = _recipe.Id;
                                    _toEdit.Units = _recipeIngredient.Units;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }


                    if (_recipeDirections != null && _recipeDirections.Count() > 0)
                    {
                        var _lastStepNumber = 0;
                        var _lastStepNumberAll = _recipe.RecipeDirections.Where(c => c.RecipeId == _recipe.Id);
                        if (_lastStepNumberAll.Count() > 0)
                        {
                            _lastStepNumber = _lastStepNumberAll.Max(c => c.StepNumber);
                        }
                        foreach (var _recipeDirection in _recipeDirections)
                        {
                            if (_recipeDirection.ItemType != null && _recipeDirection.ItemType == "remove") //remove ingredient from recipe
                            {
                                var _toRemove = _recipe.RecipeDirections.Where(c => c.Id == _recipeDirection.Id).FirstOrDefault();
                                if (_toRemove != null)
                                {
                                    _recipe.RecipeDirections.Remove(_toRemove);
                                    db.SaveChanges();
                                }
                            }
                            if (_recipeDirection.Id == 0) //create new ingredient 
                            {
                                int _stepNumber = -1;
                                if (_recipeDirection.StepNumber != 0)
                                {
                                    _stepNumber = _recipeDirection.StepNumber;
                                }
                                else
                                {
                                    _lastStepNumber++;
                                    _stepNumber = _lastStepNumber;
                                }
                                RecipeDirections _toCreate = new RecipeDirections
                                {
                                    Direction = _recipeDirection.Direction,
                                    StepNumber = _stepNumber,
                                    RecipeId = _recipe.Id
                                };
                                _recipe.RecipeDirections.Add(_toCreate);
                                db.SaveChanges();
                            }
                            else //update ingredient
                            {
                                var _toEdit = _recipe.RecipeDirections.Where(c => c.Id == _recipeDirection.Id).FirstOrDefault();
                                if (_toEdit != null)
                                {
                                    _toEdit.Direction = _recipeDirection.Direction;
                                    _toEdit.StepNumber = _recipeDirection.StepNumber;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
                return _recipe.Id;
            }
        }

        public int CreateRecipe(Recepies recepies, string recipeIngredientsJson, string recipeDirectionsJson, HttpPostedFileBase postedFile)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                Recepies _recipe = new Recepies();
                _recipe.Name = recepies.Name;
                _recipe.Rating = recepies.Rating;
                _recipe.TimeCooking = recepies.TimeCooking;
                _recipe.TimePreparing = recepies.TimePreparing;
                _recipe.TimeReady = recepies.TimeReady;
                _recipe.Yield = recepies.Yield;
                _recipe.Visible = recepies.Visible;
                _recipe.Category = recepies.Category;
                _recipe.Commentary = recepies.Commentary;
                _recipe.Cuisine = recepies.Cuisine;
                _recipe.Description = recepies.Description;
                _recipe.Visible = recepies.Visible;
                if (postedFile != null)
                {
                    byte[] bytes;
                    using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                    {
                        bytes = br.ReadBytes(postedFile.ContentLength);
                    }
                    _recipe.Picture = bytes;
                }
                db.Recepies.Add(_recipe);
                db.SaveChanges();

                var serializer = new JavaScriptSerializer();
                List<SpiroWeb.Models.RecipeIngredientViewModel> _recipeIngredients = serializer.Deserialize<List<SpiroWeb.Models.RecipeIngredientViewModel>>(recipeIngredientsJson);
                List<SpiroWeb.Models.RecipeDirectionsViewModel> _recipeDirections = serializer.Deserialize<List<SpiroWeb.Models.RecipeDirectionsViewModel>>(recipeDirectionsJson);

                foreach (var _recipeIngredient in _recipeIngredients)
                {
                    var _ingredientId = _recipeIngredient.IngredientId;

                    //If Ingredient is new, check if already exists in db, if not create new Ingredient
                    if (_recipeIngredient.IngredientId == -1)
                    {
                        var _ingredientExists = db.Ingredients.Where(c => c.Name.ToLower().Trim() == _recipeIngredient.Name.ToLower().Trim()).FirstOrDefault();
                        if (_ingredientExists != null) //it exists
                        {
                            _ingredientId = _ingredientExists.Id;
                        }
                        else //create new one
                        {
                            Ingredients _newIngredient = new Ingredients
                            {
                                Name = _recipeIngredient.Name.Trim()
                            };
                            db.Ingredients.Add(_newIngredient);
                            db.SaveChanges();
                            _ingredientId = _newIngredient.Id;
                        }
                    }
                    RecipeIngredients _toCreate = new RecipeIngredients
                    {
                        Amount = _recipeIngredient.Amount,
                        Information = _recipeIngredient.Information,
                        IngredientId = _ingredientId,
                        RecipeId = _recipe.Id,
                        Units = _recipeIngredient.Units
                    };
                    _recipe.RecipeIngredients.Add(_toCreate);
                    db.SaveChanges();
                }

                var _lastStepNumber = 0;
                foreach (var _recipeDirection in _recipeDirections)
                {
                    _lastStepNumber++;
                    RecipeDirections _toCreate = new RecipeDirections
                    {
                        Direction = _recipeDirection.Direction,
                        StepNumber = _lastStepNumber,
                        RecipeId = _recipe.Id
                    };
                    _recipe.RecipeDirections.Add(_toCreate);
                    db.SaveChanges();

                }
                return _recipe.Id;
            }
        }
    }

    public static class Test
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
   (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
