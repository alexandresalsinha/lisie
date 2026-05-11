using ClassLibrary1;
using SpiroWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SpiroWeb.Managers
{
    public static class ProductsWatchersManager
    {
        //static private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        static public List<Models.UserProductListCompleteModel2> GetAll(string userId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    if (userId != string.Empty)
                    {
                        UserUpdatePricesRequests _UserUpdatePricesRequests = Managers.UserUpdatePricesRequestsManager.Add(userId);
                        List<Models.UserProductListCompleteModel2> _listToReturn2 = new List<Models.UserProductListCompleteModel2>();

                        db.Configuration.AutoDetectChangesEnabled = true;
                        db.Set<StoreProducts>().AsNoTracking();

                        List<Models.UserProductListCompleteModel2> combinedLists = new List<Models.UserProductListCompleteModel2>();


                        var _userProductsWatchers = db.ProductsWatchers.Where(c => c.UserId == userId);

                        var _userProductsWatchersInnerJoin =
                            from userShoppingListProduct in _userProductsWatchers
                            join prod in db.Products on userShoppingListProduct.ProductId equals prod.Id
                            orderby userShoppingListProduct.Id descending
                            select new Models.UserProductListCompleteModel2
                            {
                                Id = userShoppingListProduct.Id,
                                ProductId = prod.Id,
                                Brand = prod.Brand,
                                ItemType = "product",
                                Name = prod.Name,
                                Weight = prod.Weight,
                                Category = prod.CategoryString,
                                Price = 0
                            };

                        if (_userProductsWatchersInnerJoin.Count() > 0)
                        {
                            combinedLists.AddRange(_userProductsWatchersInnerJoin);
                        }

                        Models.UserProductListCompleteModel2 _UserProductListCompleteEmpty = new Models.UserProductListCompleteModel2
                        {
                            ItemType = "empty",
                            Name = "Vazio"
                        };
                        combinedLists.Add(_UserProductListCompleteEmpty);

                        //VERY IMPORTANT - CLEAR StorePRoductsCache
                        var count = db.StoreProducts.Local.Count; // number of items in cache (ex. 30)

                        db.StoreProducts.Local.ToList().ForEach(c =>
                        {
                            db.Entry(c).State = EntityState.Detached;
                        });


                        foreach (var productCombined in combinedLists)
                        {
                            var _userProductStores = from m in db.StoreProducts where m.ProductId == productCombined.ProductId select m;
                            db.Set<StoreProducts>().AsNoTracking();
                            if (_userProductStores.Count() > 0)
                            {

                                foreach (var storeProduct in _userProductStores.ToList())
                                {
                                    if (productCombined.PriceList == null) productCombined.PriceList = new List<Models.StoreProduct>();

                                    //Get last price fluctuation
                                    var _lastProductPriceUpdate = db.ProductPricesUpdates.Where(c => c.ProductId == productCombined.ProductId && c.StoreId == storeProduct.StoreId).OrderByDescending(c => c.CreateDate).FirstOrDefault();
                                    string _lastPriceChange = "";
                                    if (_lastProductPriceUpdate != null)
                                    {
                                        if (_lastProductPriceUpdate.NewPrice > _lastProductPriceUpdate.OldPrice) _lastPriceChange = "up";
                                        if (_lastProductPriceUpdate.NewPrice < _lastProductPriceUpdate.OldPrice) _lastPriceChange = "down";
                                        if (_lastProductPriceUpdate.NewPrice == _lastProductPriceUpdate.OldPrice) _lastPriceChange = "same";
                                    }

                                    productCombined.PriceList.Add(new Models.StoreProduct
                                    {
                                        Id = storeProduct.Id,
                                        Price = Math.Round(storeProduct.Price.Value, 2),
                                        StoreId = storeProduct.StoreId,
                                        Url = storeProduct.Url,
                                        CreatedByUserId = storeProduct.UserId,
                                        NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false),
                                        LastPriceChange = _lastPriceChange,
                                        OnlineProductId = storeProduct.OnlineProductId,
                                        UpdateDate = (storeProduct.UpdateDate.HasValue) ? storeProduct.UpdateDate.Value : DateTime.MinValue
                                    });
                                }
                            }
                        }
                        return combinedLists;

                    }
                    return new List<Models.UserProductListCompleteModel2>();
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return new List<Models.UserProductListCompleteModel2>();
            }
        }
        static public List<ProductsWatchers> GetProductIdsOfUser(string userId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    if (userId != string.Empty)
                    {
                        var _userProductsWatchers = db.ProductsWatchers.Where(c => c.UserId == userId);


                        return _userProductsWatchers.ToList();

                    }
                    return new List<ProductsWatchers>();
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return new List<ProductsWatchers>();
            }
        }

        static public List<string> GetUserIdsWithProductWatcher(int productId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _userIds = db.ProductsWatchers.Where(c => c.ProductId == productId).Select(c => c.UserId).ToList();
                    return _userIds;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return new List<string>();
            }
        }


        //static public ProductsWatchers Create(Models.ProductWatcherPostModel _productWatcherPostModel)
        static public ProductsWatchers Create(string userId, int productId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _existsProductsWatchers = db.ProductsWatchers.Where(c => c.UserId.Equals(userId) && c.ProductId.Equals(productId)).FirstOrDefault();
                    if (_existsProductsWatchers == null)
                    {
                        ProductsWatchers _newProductsWatchers = new ProductsWatchers();
                        _newProductsWatchers.UserId = userId;
                        _newProductsWatchers.ProductId = productId;
                        _newProductsWatchers.CreateDate = DateTime.Now;
                        db.ProductsWatchers.Add(_newProductsWatchers);
                        db.SaveChanges();
                        return _newProductsWatchers;
                    }
                    else
                    {
                        return _existsProductsWatchers;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return null;
            }
        }

        static public bool Delete(string userId, int id)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _existsProductsWatchers = db.ProductsWatchers.Where(c => c.UserId.Equals(userId) && c.Id == id).FirstOrDefault();
                    if (_existsProductsWatchers != null)
                    {
                        db.ProductsWatchers.Remove(_existsProductsWatchers);
                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return false;
            }
        }
    }
}