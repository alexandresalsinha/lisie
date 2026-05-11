using ClassLibrary1;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class ScoreboardController : Controller
    {
        [Authorize]
        public ActionResult Totals()
        {

            return View();
        }

        [Authorize]
        public ActionResult ProductUpdates()
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _query =
                            from _storeProduct in db.StoreProducts
                            join _prod in db.Products on _storeProduct.ProductId equals _prod.Id
                            join _store in db.Stores on _storeProduct.StoreId equals _store.Id
                            orderby _storeProduct.UpdateDate descending
                            select new Models.ProductsUpdatesModel
                            {
                                ProductId = _prod.Id,
                                Name = _prod.Name,
                                Brand = _prod.Brand,
                                Store = _store.Name,
                                UpdateDate = _storeProduct.UpdateDate.HasValue ? _storeProduct.UpdateDate.Value : DateTime.MinValue,
                                NeedsUpdate = _storeProduct.NeedsUpdate.HasValue ? _storeProduct.NeedsUpdate.Value : true,
                                StoreUrl = _store.Url + _storeProduct.Url
                            };
                var _list = _query.Take(20).ToList(); ;
                foreach (var _item in _list)
                {
                    var _ProductPricesUpdate = db.ProductPricesUpdates.Where(c => c.ProductId == _item.ProductId).OrderByDescending(c => c.CreateDate).FirstOrDefault();
                    if (_ProductPricesUpdate != null)
                    {
                        _item.OldPrice = Math.Round(_ProductPricesUpdate.OldPrice, 2).ToString();
                        _item.NewPrice = Math.Round(_ProductPricesUpdate.NewPrice, 2).ToString();
                        _item.PriceUpdateDate = _ProductPricesUpdate.CreateDate;
                    }
                }
                return View(_list);
            }
        }

        [Authorize]
        public ActionResult ProductPricesUpdates()
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _query =
                    from _priceUpdate in db.ProductPricesUpdates
                    join _prod in db.Products on _priceUpdate.ProductId equals _prod.Id
                    join _store in db.Stores on _priceUpdate.StoreId equals _store.Id
                    orderby _priceUpdate.CreateDate descending
                    select new Models.ProductsUpdatesModel
                    {
                        ProductId = _prod.Id,
                        Name = _prod.Name,
                        Brand = _prod.Brand,
                        Store = _store.Name,
                        StoreId = _store.Id,
                        PriceUpdateDate = _priceUpdate.CreateDate,
                        //NeedsUpdate = _storeProduct.NeedsUpdate.HasValue ? _storeProduct.NeedsUpdate.Value : true,
                        StoreUrl = _store.Url,
                        OldPrice = Math.Round(_priceUpdate.OldPrice, 2).ToString(),
                        NewPrice = Math.Round(_priceUpdate.NewPrice, 2).ToString()
                    };
                var _list = _query.Take(20).ToList(); ;
                foreach (var _item in _list)
                {
                    var _storeProduct = db.StoreProducts.Where(c => c.ProductId == _item.ProductId && c.StoreId == _item.StoreId).FirstOrDefault();
                    if (_storeProduct != null)
                    {
                        _item.NeedsUpdate = _storeProduct.NeedsUpdate.HasValue ? _storeProduct.NeedsUpdate.Value : true;
                        _item.StoreUrl += _storeProduct.Url;
                        _item.UpdateDate = _storeProduct.UpdateDate.HasValue ? _storeProduct.UpdateDate.Value : DateTime.MinValue;
                    }
                }
                return View(_list);
            }
        }
    }
}
