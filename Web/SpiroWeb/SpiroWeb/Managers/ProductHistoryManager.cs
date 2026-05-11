using ClassLibrary1;
using SpiroWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiroWeb.Managers
{
    public static class ProductHistoryManager
    {
        static public List<ProductsListHistoryModel> GetOfMonthYear(int productId, int month, int year)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                if (month == -1 || year == -1)
                {
                    var _matchingProducts =
                           from productPricesUpdates in db.ProductPricesUpdates
                           join prod in db.Products on productPricesUpdates.ProductId equals prod.Id
                           where productPricesUpdates.ProductId == productId
                           orderby productPricesUpdates.CreateDate descending
                           select new Models.ProductsListHistoryModel
                           {
                               Id = productPricesUpdates.Id,
                               ProductId = prod.Id,
                               //ListName = m.ListName,
                               InsertDate = productPricesUpdates.CreateDate,
                               Name = prod.Name,
                               Brand = prod.Brand,
                               Weight = prod.Weight,
                               StoreId = productPricesUpdates.StoreId.Value,
                               OldPrice = Math.Round(productPricesUpdates.OldPrice, 2).ToString(),
                               NewPrice = Math.Round(productPricesUpdates.NewPrice, 2).ToString(),
                               PriceChange = (productPricesUpdates.OldPrice < productPricesUpdates.NewPrice) ? "up" : "down"
                           };

                    if (_matchingProducts != null && _matchingProducts.Count() > 0)
                    {
                        return _matchingProducts.ToList();
                    }
                    else
                    {
                        return new List<ProductsListHistoryModel>();
                    }
                }
                else
                {
                    DateTime _dt_start = new DateTime(year, month, 1, 0, 0, 0);
                    DateTime _dt_end = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);

                    var _matchingProducts =
                           from productPricesUpdates in db.ProductPricesUpdates
                           join prod in db.Products on productPricesUpdates.ProductId equals prod.Id
                           where productPricesUpdates.ProductId == productId &&
                           productPricesUpdates.CreateDate > _dt_start &&
                           productPricesUpdates.CreateDate < _dt_end
                           orderby productPricesUpdates.CreateDate descending
                           select new Models.ProductsListHistoryModel
                           {
                               Id = productPricesUpdates.Id,
                               ProductId = prod.Id,
                               //ListName = m.ListName,
                               InsertDate = productPricesUpdates.CreateDate,
                               Name = prod.Name,
                               Brand = prod.Brand,
                               Weight = prod.Weight,
                               StoreId = productPricesUpdates.StoreId.Value,
                               OldPrice = Math.Round(productPricesUpdates.OldPrice, 2).ToString(),
                               NewPrice = Math.Round(productPricesUpdates.NewPrice, 2).ToString(),
                               PriceChange = (productPricesUpdates.OldPrice < productPricesUpdates.NewPrice) ? "up" : "down"
                           };

                    if (_matchingProducts != null && _matchingProducts.Count() > 0)
                    {
                        return _matchingProducts.ToList();
                    }
                    else
                    {
                        return new List<ProductsListHistoryModel>();
                    }
                }

            }
        }

        //static public List<UserProductsListHistoryModel> GetAll(string userId)
        //{

        //    List<UserProductsListHistory> _matchingProducts = db.UserProductsListHistory.Include("Products").Where(c => c.UserId == userId &&
        //    (c.ListName == "consumed" ||
        //    c.ListName == "bought")).OrderByDescending(c => c.InsertDate).ToList();

        //    if (_matchingProducts != null && _matchingProducts.Count > 0)
        //    {

        //        var matchingJson = _matchingProducts.Select(m => new UserProductsListHistoryModel
        //        {
        //            Id = m.Id,
        //            ProductId = m.ProductId,
        //            ListName = m.ListName,
        //            InsertDate = m.InsertDate,
        //            Quantity = m.Quantity,
        //            QuantityWeight = m.QuantityWeight,
        //            UserId = m.UserId,
        //            Name = m.Products.Name,
        //            Brand = m.Products.Brand,
        //            Weight = m.Products.Weight

        //        }).ToList();

        //        //TODO - save price when adding to history, and show those prices, not the real in time prices
        //        foreach (var _matchingJson in matchingJson)
        //        {
        //            var _StoreProducts = from m in db.StoreProducts where m.ProductId == _matchingJson.ProductId select m;
        //            if (_StoreProducts.Count() > 0)
        //            {
        //                foreach (var storeProduct in _StoreProducts)
        //                {
        //                    if (_matchingJson.PriceList == null) _matchingJson.PriceList = new List<Models.StoreProduct>();
        //                    _matchingJson.PriceList.Add(new Models.StoreProduct
        //                    {
        //                        Id = storeProduct.Id,
        //                        Price = Math.Round(storeProduct.Price.Value * _matchingJson.Quantity.Value, 2),
        //                        StoreId = storeProduct.StoreId,
        //                        Url = storeProduct.Url,
        //                        CreatedByUserId = storeProduct.UserId,
        //                        NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
        //                    });
        //                }
        //            }
        //        }
        //        return matchingJson;
        //    }
        //    else
        //    {
        //        return new List<UserProductsListHistoryModel>();
        //    }

        //}

        //static public bool DeleteOfUser(int id, string userId)
        //{
        //    try
        //    {
        //        var _history = db.UserProductsListHistory.Where(c => c.Id == id && c.UserId == userId).FirstOrDefault();

        //        if (_history != null)
        //            db.UserProductsListHistory.Remove(_history);
        //        else return false;

        //        db.SaveChanges();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

    }
}