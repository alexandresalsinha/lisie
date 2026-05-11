using ClassLibrary1;
using SpiroWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiroWeb.Managers
{
    public static class UserHistoryManager
    {
        static private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        static public List<UserProductsListHistoryModel> GetOfMonthYear(string userId, int month, int year)
        {
            DateTime _dt_start = new DateTime(year, month, 1, 0, 0, 0);
            DateTime _dt_end = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);

            //List<UserProductsListHistory> _matchingProductsOld = (from c in db.UserProductsListHistory
            //                                                   where c.UserId == userId &&
            //c.InsertDate > _dt_start &&
            //c.InsertDate < _dt_end &&
            //(c.ListName == "consumed" ||
            //c.ListName == "bought")
            //                                                   orderby c.InsertDate descending
            //                                                   select c).ToList();

            List<UserProductsListHistory> _matchingProducts = db.UserProductsListHistory.Include("Products").Where(c => c.UserId == userId &&
            c.InsertDate > _dt_start &&
            c.InsertDate < _dt_end &&
            (c.ListName == "consumed" ||
            c.ListName == "bought")).OrderByDescending(c => c.InsertDate).ToList();

            if (_matchingProducts != null && _matchingProducts.Count > 0)
            {

                var matchingJson = _matchingProducts.Select(m => new UserProductsListHistoryModel
                {
                    Id = m.Id,
                    ProductId = m.ProductId ?? -1,
                    ListName = m.ListName,
                    InsertDate = m.InsertDate,
                    Quantity = m.Quantity,
                    QuantityWeight = m.QuantityWeight ?? null,
                    UserId = m.UserId,
                    Name = m.Products != null ? m.Products.Name : m.ProductName,
                    Brand = m.Products?.Brand,
                    Weight = m.Products?.Weight

                }).ToList();

                //TODO - save price when adding to history, and show those prices, not the real in time prices
                foreach (var _matchingJson in matchingJson)
                {
                    if (_matchingJson.ProductId != -1)
                    {
                        var _StoreProducts = from m in db.StoreProducts where m.ProductId == _matchingJson.ProductId select m;
                        if (_StoreProducts.Count() > 0)
                        {
                            foreach (var storeProduct in _StoreProducts)
                            {
                                if (_matchingJson.PriceList == null) _matchingJson.PriceList = new List<Models.StoreProduct>();
                                _matchingJson.PriceList.Add(new Models.StoreProduct
                                {
                                    Id = storeProduct.Id,
                                    Price = Math.Round(storeProduct.Price.Value * _matchingJson.Quantity.Value, 2),
                                    StoreId = storeProduct.StoreId,
                                    Url = storeProduct.Url,
                                    CreatedByUserId = storeProduct.UserId,
                                    NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
                                });
                            }
                        }
                    }
                }
                return matchingJson;
            }
            else
            {
                return new List<UserProductsListHistoryModel>();
            }

        }

        static public List<UserProductsListHistoryModel> GetAll(string userId)
        {

            List<UserProductsListHistory> _matchingProducts = db.UserProductsListHistory.Include("Products").Where(c => c.UserId == userId &&
            (c.ListName == "consumed" ||
            c.ListName == "bought")).OrderByDescending(c => c.InsertDate).ToList();

            if (_matchingProducts != null && _matchingProducts.Count > 0)
            {

                var matchingJson = _matchingProducts.Select(m => new UserProductsListHistoryModel
                {
                    Id = m.Id,
                    ProductId = m.ProductId.Value,
                    ListName = m.ListName,
                    InsertDate = m.InsertDate,
                    Quantity = m.Quantity,
                    QuantityWeight = m.QuantityWeight,
                    UserId = m.UserId,
                    Name = m.Products.Name,
                    Brand = m.Products.Brand,
                    Weight = m.Products.Weight

                }).ToList();

                //TODO - save price when adding to history, and show those prices, not the real in time prices
                foreach (var _matchingJson in matchingJson)
                {
                    var _StoreProducts = from m in db.StoreProducts where m.ProductId == _matchingJson.ProductId select m;
                    if (_StoreProducts.Count() > 0)
                    {
                        foreach (var storeProduct in _StoreProducts)
                        {
                            if (_matchingJson.PriceList == null) _matchingJson.PriceList = new List<Models.StoreProduct>();
                            _matchingJson.PriceList.Add(new Models.StoreProduct
                            {
                                Id = storeProduct.Id,
                                Price = Math.Round(storeProduct.Price.Value * _matchingJson.Quantity.Value, 2),
                                StoreId = storeProduct.StoreId,
                                Url = storeProduct.Url,
                                CreatedByUserId = storeProduct.UserId,
                                NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
                            });
                        }
                    }
                }
                return matchingJson;
            }
            else
            {
                return new List<UserProductsListHistoryModel>();
            }

        }

        static public UserProductsListHistoryModel GetLastEntry(string userId, string list)
        {

            UserProductsListHistory _matchingProduct = db.UserProductsListHistory.Include("Products").Where(c => c.UserId == userId &&
            (c.ListName == list)).OrderByDescending(c => c.InsertDate).FirstOrDefault();

            if (_matchingProduct != null)
            {
                return new UserProductsListHistoryModel
                {
                    Id = _matchingProduct.Id,
                    ProductId = _matchingProduct.ProductId.Value,
                    ListName = _matchingProduct.ListName,
                    InsertDate = _matchingProduct.InsertDate,
                    Quantity = _matchingProduct.Quantity,
                    QuantityWeight = _matchingProduct.QuantityWeight,
                    UserId = _matchingProduct.UserId,
                    Name = _matchingProduct.Products.Name,
                    Brand = _matchingProduct.Products.Brand,
                    Weight = _matchingProduct.Products.Weight
                };
            }
            else
            {
                return null;
            }


        }

        static public bool DeleteOfUser(int id, string userId)
        {
            try
            {
                var _history = db.UserProductsListHistory.Where(c => c.Id == id && c.UserId == userId).FirstOrDefault();

                if (_history != null)
                    db.UserProductsListHistory.Remove(_history);
                else return false;

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        static public UserProductHistoryModel GetTotalsOfProduct(string userId, int productId, int storeId, string action = "consumed")
        {
            //DateTime _dt_start = new DateTime(year, month, 1, 0, 0, 0);
            //DateTime _dt_end = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);

            List<UserProductsListHistory> _matchingProducts = db.UserProductsListHistory.Include("Products").Where(c => c.UserId == userId &&
            c.ProductId == productId &&
            //c.InsertDate > _dt_start &&
            //c.InsertDate < _dt_end &&
            c.ListName == action
            ).OrderByDescending(c => c.InsertDate).ToList();

            if (_matchingProducts != null && _matchingProducts.Count > 0)
            {

                var matchingJson = _matchingProducts.Select(m => new UserProductsListHistoryModel
                {
                    Id = m.Id,
                    ProductId = m.ProductId ?? -1,
                    ListName = m.ListName,
                    InsertDate = m.InsertDate,
                    Quantity = m.Quantity,
                    QuantityWeight = m.QuantityWeight ?? null,
                    UserId = m.UserId,
                    Name = m.Products != null ? m.Products.Name : m.ProductName,
                    Brand = m.Products?.Brand,
                    Weight = m.Products?.Weight

                }).ToList();

                double _totalStorePrice = 0.0;
                //TODO - save price when adding to history, and show those prices, not the real in time prices
                foreach (var _matchingJson in matchingJson)
                {
                    if (_matchingJson.ProductId != -1)
                    {
                        var _StoreProducts = from m in db.StoreProducts where m.ProductId == _matchingJson.ProductId select m;
                        if (_StoreProducts.Count() > 0)
                        {
                            foreach (var storeProduct in _StoreProducts)
                            {
                                if (_matchingJson.PriceList == null) _matchingJson.PriceList = new List<Models.StoreProduct>();
                                _matchingJson.PriceList.Add(new Models.StoreProduct
                                {
                                    Id = storeProduct.Id,
                                    Price = Math.Round(storeProduct.Price.Value * _matchingJson.Quantity.Value, 2),
                                    StoreId = storeProduct.StoreId,
                                    Url = storeProduct.Url,
                                    CreatedByUserId = storeProduct.UserId,
                                    NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
                                });
                                if (storeProduct.StoreId == storeId)
                                {
                                    _totalStorePrice += Math.Round(storeProduct.Price.Value * _matchingJson.Quantity.Value, 2);
                                }
                            }
                        }
                    }
                }
                return new UserProductHistoryModel
                {
                    Action = action,
                    ProductId = productId,
                    StoreId = storeId,
                    TotalPrice = _totalStorePrice,
                    UserProductsListHistory = matchingJson
                };
            }
            else
            {
                return new UserProductHistoryModel();
            }

        }
    }
}