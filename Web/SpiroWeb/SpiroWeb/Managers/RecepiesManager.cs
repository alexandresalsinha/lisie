using ClassLibrary1;
using SpiroWeb.Controllers;
using SpiroWeb.Helpers;
using SpiroWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiroWeb.Managers
{
    public static class RecepiesManager
    {
        static private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        static public IngredientProducts AssociateProductToIngredient(string userId, int productId, int ingredientId)
        {
            //In future compare userId
            //var _ingredientProduct = db.IngredientProducts.Where(c => c.UserId == userId && c.IngredientId == ingredientId).FirstOrDefault();
            var _ingredientProduct = db.IngredientProducts.Where(c => c.IngredientId == ingredientId && c.ProductId == productId).FirstOrDefault();
            if (_ingredientProduct == null)
            {
                IngredientProducts _newIngredientProducts = new IngredientProducts
                {
                    IngredientId = ingredientId,
                    ProductId = productId,
                    UserId = userId
                };
                db.IngredientProducts.Add(_newIngredientProducts);
                db.SaveChanges();
                return _newIngredientProducts;
            }
            return null;
        }

        static public IngredientProducts GetIngredientProduct(int ingredientId, string userId)
        {
            var _ingredientProduct = db.IngredientProducts.Where(c => c.UserId == userId && c.IngredientId == ingredientId).FirstOrDefault();
            if (_ingredientProduct != null)
            {
                return _ingredientProduct;
            }
            return null;
        }

        static public List<IngredientProducts> GetIngredientProducts(int ingredientId, string userId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                //In the future
                //return db.IngredientProducts.Where(c => c.UserId == userId && c.IngredientId == ingredientId).ToList();
                return db.IngredientProducts.Where(c => c.IngredientId == ingredientId).ToList();
            }
        }

        static public string GetIngredientName(int ingredientId)
        {
            var _ingredient = db.Ingredients.Where(c => c.Id == ingredientId).FirstOrDefault();
            if (_ingredient != null)
            {
                return _ingredient.Name;
            }
            return string.Empty;
        }

        static public int AddRecipeProductsToUserShoppingCart(string userId, List<IngredientProductToAddPostModel> products)
        {
            List<int> _userProductsIdsAdded = new List<int>();
            List<int> _userProductsIdsNotAdded = new List<int>();
            try
            {

                foreach (var _product in products)
                {
                    ShoppingCartController _ShoppingCartController = new ShoppingCartController();
                    int userProductListId = _ShoppingCartController.AddProductToShoppingCart(_product.ProductId, _product.Quantity, null, true, userId);
                    if (userProductListId != -1)
                        _userProductsIdsAdded.Add(_product.ProductId);
                    else
                        _userProductsIdsNotAdded.Add(_product.ProductId);
                }

                //WarnMeOfAddRecipeToUserShoppingCart(userId, recipe.Name);
            }
            catch (Exception ex)
            {
                Logger.Debug("Error:" + ex.InnerException.Message);
            }

            return _userProductsIdsAdded.Count;
        }
    }
}