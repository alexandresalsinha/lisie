using ClassLibrary1;
using SpiroWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace SpiroWeb.Controllers
{
    public class RecipesApiController : ApiController
    {
        //private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        public HttpResponseMessage GetRecipe(string userId, int recipeId)
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

                    return Request.CreateResponse(HttpStatusCode.OK, __recipe);
                    //return Json(__recipe, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "-1");
                }
            }
        }

        // POST: api/UserLists/ProductsConsumedFromInventory
        public HttpResponseMessage AddRecipeProductsToUserShoppingCart([FromBody] IngredientProductsToAddPostModel data)
        {
            Managers.InteractionsManager.Add(data.UserId, "api/Recipes/AddRecipeProductsToUserShoppingCart", new JavaScriptSerializer().Serialize(data));
            if (data.Products != null && data.Products.Count > 0)
            {
                int _productsAdded = Managers.RecepiesManager.AddRecipeProductsToUserShoppingCart(data.UserId, data.Products);
                return Request.CreateResponse(HttpStatusCode.Created, _productsAdded);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }

}
