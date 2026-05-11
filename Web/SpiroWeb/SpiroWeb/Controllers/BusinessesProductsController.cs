using ClassLibrary1;
using SpiroWeb.Helpers;
using SpiroWeb.Managers;
using SpiroWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class BusinessesProductsController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        //[Authorize]
        public ActionResult Today(int b, string l = "consumed", string dateStart = "", string dateEnd = "", string category = "")
        {
            DateTime _startDate = (dateStart != string.Empty) ? DateTime.ParseExact(dateStart, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : DateTime.Now;
            DateTime _endDate = (dateEnd != string.Empty) ? DateTime.ParseExact(dateEnd, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : DateTime.Now;
            var _response = Managers.BusinessProductsManager.GetHistory(b, l, _startDate.ToShortDateString(), _endDate.ToShortDateString(), category);
            var _historyItems = _response.Data as List<BusinessProductHistoryDTO>;

            double _total = 0;
            foreach (var item in _historyItems)
            {
                _total += item.Price * item.Quantity;
            }
            ViewBag.Total = TextTools.ParsePrice(_total.ToString());
            ViewBag.Items = _historyItems;
            ViewBag.StartDate = _startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            ViewBag.EndDate = _endDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            List<SelectListItem> _categories = new List<SelectListItem>();
            var _categoriesDb = Managers.BusinessProductsManager._GetProductsCategories(b);
            foreach (var item in _categoriesDb)
            {
                var _selectListItem = new SelectListItem
                {
                    Text = item,
                    Value = item,
                    Selected = item.ToLower() == category.ToLower()
                };
                //if (string.IsNullOrEmpty(category) && item == "Tabaco")
                //{
                //    _selectListItem.Selected = true;
                //}   
                _categories.Add(_selectListItem);
            }
            _categories.Insert(0, new SelectListItem
            {
                Text = "Todas",
                Value = "Todas",
                Selected = category == "Todas"
            });
            ViewBag.Categories = _categories;

            return View();
        }

        public ActionResult StocksReport(int b)
        {
            var _response = Managers.BusinessProductsManager.GetStock(b);
            var _UserProducts = _response.Data as List<BusinessProductStockDTO>;

            double _total = 0;
            foreach (var item in _UserProducts)
            {
                _total += item.Price * item.Quantity;
            }
            ViewBag.Total = TextTools.ParsePrice(_total.ToString());
            ViewBag.Items = _UserProducts;
            return View();
        }

        public ActionResult ChangedQuantityReport(int b, string l = "bought", string dateStart = "", string dateEnd = "")
        {
            DateTime _startDate = (dateStart != string.Empty) ? DateTime.ParseExact(dateStart, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : DateTime.Now;
            DateTime _endDate = (dateEnd != string.Empty) ? DateTime.ParseExact(dateEnd, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : DateTime.Now;
            var _response = Managers.BusinessProductsManager.GetStockChangeHistory(b, l, _startDate.ToShortDateString(), _endDate.ToShortDateString());
            var _historyItems = _response.Data as List<BusinessProductStockChangesHistoryDTO>;

            //double _total = 0;
            //foreach (var item in _historyItems)
            //{
            //    _total += item.Price * item.Quantity;
            //}
            //ViewBag.Total = (System.Web.HttpContext.Current.Request.IsLocal) ? TextTools.ParsePriceLocal(_total.ToString()) :
            //        TextTools.ParsePriceProduction(_total.ToString());
            ViewBag.Items = _historyItems;
            ViewBag.StartDate = _startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            ViewBag.EndDate = _endDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            return View();
        }


        [Authorize]
        public ActionResult Updates()
        {
            List<Models.StoreProductsUpdateViewModel> _storeProductsUpdatedDateCount = new List<StoreProductsUpdateViewModel>();
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                //IMPORTANT - linq group by day date and get count
                _storeProductsUpdatedDateCount = db.StoreProducts
                    .Where(x => x.UpdateDate != null)
                    .GroupBy(x => DbFunctions.TruncateTime(x.UpdateDate).Value)
                    .Select(x => new Models.StoreProductsUpdateViewModel
                    {
                        Count = x.Count(),
                        Date = DbFunctions.TruncateTime(x.Key).Value
                    })
                    .Distinct()
                    .OrderByDescending(x => x.Date)
                    .ToList();

                var nullUpdateDatesCount = db.StoreProducts
                    .Where(x => x.UpdateDate == null)
                    .Count();

                _storeProductsUpdatedDateCount.Add(new StoreProductsUpdateViewModel { Count = nullUpdateDatesCount, Date = DateTime.MinValue });
                //ViewBag.updates = _storeProductsUpdatedDateCount;

            }


            //var result = db.StoreProducts
            //.GroupBy(x => x.UpdateDate)
            //.Select(x => new
            //{
            //    Count = x.Count(),
            //    Date = (DateTime)x.Key // or x.Key.Date (excluding time info) or x.Key.Date.ToString() (give only Date in string format) 
            //})
            //.ToList();




            return View(_storeProductsUpdatedDateCount);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                StoreProducts storeProduct = db.StoreProducts.Find(id);
                if (storeProduct == null)
                {
                    return HttpNotFound();
                }
                db.StoreProducts.Remove(storeProduct);
                db.SaveChanges();
            }
            catch (Exception)
            {
            }

            return View("Index");
        }

        public async Task<ActionResult> UpdateMetadata(int productId, int storeProductId)
        {
            try
            {
                await Managers.StoreProductsManager.UpdateMetadata(storeProductId);
            }
            catch (Exception)
            {
            }

            return RedirectToAction("Details", "Products", new { id = productId });
        }

        public ActionResult AcceptTemp(int productId, int storeProductId)
        {
            var products = Managers.StoreProductsManager.AcceptTemp(storeProductId);
            return RedirectToAction("Products", "Details", new { id = productId });
        }
    }
}