using ClassLibrary1;
using PagedList;
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
    public class StoreProductsController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        [Authorize]
        public ActionResult Index(string orderBy, string searchQuery, int page = 1, bool notMine = false, bool updateDateIsNull = false, bool notMineAndIsTemp = false)
        {
            //var products = (string.IsNullOrEmpty(orderBy)) ? db.Products.ToList() : db.Products.OrderBy(c => c.Name).ToList();
            IQueryable<ClassLibrary1.StoreProducts> storeProducts = db.StoreProducts.OrderByDescending(c => c.Id);

            if (notMine)
            {
                storeProducts = storeProducts.Where(c => c.UserId != "9ff8224f-17cf-49fb-b555-05779a13eb40");
            }
            if (updateDateIsNull)
            {
                storeProducts = storeProducts.Where(c => c.UpdateDate == null);
            }
            if (notMineAndIsTemp)
            {
                storeProducts = storeProducts.Where(c => c.UserId != "9ff8224f-17cf-49fb-b555-05779a13eb40" && (c.IsTemp.HasValue && c.IsTemp.Value == true));
            }
            //if (!string.IsNullOrEmpty(orderBy))
            //{
            //    switch (orderBy.ToLower())
            //    {
            //        case "name":
            //            products = products.OrderBy(c => c.Name);
            //            break;
            //        case "insertdate":
            //            products = products.OrderByDescending(c => c.InsertDate);
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //if (string.IsNullOrEmpty(searchQuery) && string.IsNullOrEmpty(orderBy))
            //{
            //    products = db.Products.OrderBy(c => c.Id);
            //}

            //var pageNumber = page ?? 1;
            var pageNumber = page;

            if (Session["storeProductsIndexCurrentPage"] != null && Session["goToSavedStoreProductsIndexPage"] != null)
            {
                pageNumber = Convert.ToInt32(Session["storeProductsIndexCurrentPage"]);
                Session["goToSavedStoreProductsIndexPage"] = null;
            }

            try
            {
                var onePageOfProducts = storeProducts.ToPagedList(pageNumber, 25);
                Session["storeProductsIndexCurrentPage"] = pageNumber;
                ViewBag.OnePageOfProducts = onePageOfProducts;
            }
            catch (Exception ex)
            {
                string stop = "";
            }

            ViewBag.searchQuery = searchQuery;
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
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        public ActionResult RefuseTemp(int productId, int storeProductId)
        {
            var products = Managers.StoreProductsManager.RefuseTemp(storeProductId);
            return RedirectToAction("Details", "Products", new { id = productId });
        }
    }
}