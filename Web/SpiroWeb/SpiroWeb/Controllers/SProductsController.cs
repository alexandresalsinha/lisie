using ClassLibrary1;
using PagedList;
using SpiroWeb.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class SProductsController : Controller
    {
        // GET: Products1
        [Authorize]
        public ActionResult Index(string orderBy, string searchQuery, int page = 1)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _temp =
                   from storePrd in db.StoreProducts
                   join prod in db.Products on storePrd.ProductId equals prod.Id
                   join store in db.Stores on storePrd.StoreId equals store.Id
                   //orderby m.Id descending
                   select new StoreProductViewModel
                   {
                       Id = storePrd.Id,
                       ProductId = prod.Id,
                       Barcode = prod.Barcode,
                       Brand = prod.Brand,
                       Name = prod.Name,
                       StorePrice = Math.Round(storePrd.Price.Value, 2),
                       StoreId = storePrd.StoreId,
                       StoreOnlineProductId = storePrd.OnlineProductId,
                       StoreName = store.Name,
                       Url = store.Url + storePrd.Url,
                       NeedsUpdate = storePrd.NeedsUpdate,
                       CreatedByUserId = storePrd.UserId,
                       Weight = prod.Weight,
                       Unit = storePrd.Unit,
                       UpdateDate = storePrd.UpdateDate.HasValue ? storePrd.UpdateDate.Value : DateTime.MinValue,
                       CreateDate = storePrd.CreateDate.HasValue ? storePrd.CreateDate.Value : DateTime.MinValue
                   };

                if (!string.IsNullOrEmpty(orderBy))
                {
                    switch (orderBy.ToLower())
                    {
                        case "id":
                            _temp = _temp.OrderByDescending(c => c.Id);
                            break;
                        case "createdate":
                            _temp = _temp.OrderByDescending(c => c.CreateDate);
                            break;
                        case "updatedate":
                            _temp = _temp.OrderByDescending(c => c.UpdateDate);
                            break;
                        case "needsupdate":
                            _temp = _temp.OrderByDescending(c => c.NeedsUpdate);
                            break;
                        default:
                            break;
                    }
                }
                if (string.IsNullOrEmpty(orderBy))
                {
                    _temp = _temp.OrderByDescending(c => c.CreateDate);
                }

                //var pageNumber = page ?? 1;
                var pageNumber = page;

                if (Session["productsIndexCurrentPage"] != null && Session["goToSavedproductsIndexPage"] != null)
                {
                    pageNumber = Convert.ToInt32(Session["productsIndexCurrentPage"]);
                    Session["goToSavedproductsIndexPage"] = null;
                }

                try
                {
                    var onePageOfProducts = _temp.ToPagedList(pageNumber, 25);
                    Session["productsIndexCurrentPage"] = pageNumber;
                    ViewBag.OnePageOfProducts = onePageOfProducts;
                }
                catch (Exception ex)
                {
                    string stop = "";
                }

                //ViewBag.searchQuery = searchQuery;
                return View();
            }

        }
    }
}