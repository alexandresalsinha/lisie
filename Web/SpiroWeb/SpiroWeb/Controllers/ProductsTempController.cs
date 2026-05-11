using ClassLibrary1;
using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class ProductsTempController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        // GET: Products1
        [Authorize]
        public ActionResult Index(string orderBy, string searchQuery, int page = 1)
        {
            //var products = (string.IsNullOrEmpty(orderBy)) ? db.Products.ToList() : db.Products.OrderBy(c => c.Name).ToList();
            IQueryable<Products> products = db.Products.Where(c => c.IsTemp.HasValue);

            //if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(searchQuery))
            //{
            //    products = products.Where(c => c.Name.ToLower().Contains(searchQuery)).OrderBy(c => c.Name);
            //}
            //else 

            if (!string.IsNullOrEmpty(searchQuery))
            {
                string[] _searchWords = searchQuery.ToLower().Trim(' ').Split(' ');
                products = products.Where(c => (_searchWords.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z))))
                    .OrderBy(c => c.Name);

                //products = db.Products.Where(c => c.Name.ToLower().Contains(searchQuery.ToLower()) ||
                //                             c.CategoryString.ToLower().Contains(searchQuery.ToLower()) ||
                //                             c.Brand.ToLower().Contains(searchQuery.ToLower()));
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "name":
                        products = products.OrderBy(c => c.Name);
                        break;
                    case "insertdate":
                        products = products.OrderByDescending(c => c.InsertDate);
                        break;
                    default:
                        break;
                }
            }
            if (string.IsNullOrEmpty(searchQuery) && string.IsNullOrEmpty(orderBy))
            {
                products = products.OrderBy(c => c.Id);
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
                var onePageOfProducts = products.ToPagedList(pageNumber, 25);
                Session["productsIndexCurrentPage"] = pageNumber;
                ViewBag.OnePageOfProducts = onePageOfProducts;
            }
            catch (Exception ex)
            {
                string stop = "";
            }

            ViewBag.searchQuery = searchQuery;
            return View();
        }

        // GET: Products1/Delete/5
        public ActionResult Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            products.IsTemp = null;
            db.Entry(products).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: Products1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var products = db.Products.Include("StoreProducts").Where(c=> c.Id.Equals(id)).FirstOrDefault();
            var products = Managers.ProductsManager.GetDTOById(id.Value);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: Products1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products products = db.Products.Find(id);
            db.Products.Remove(products);

            //remove from UserProductsConsumed
            var _userProductsConsumed = db.UserProductsConsumed.Where(c => c.ProductId == products.Id);
            db.UserProductsConsumed.RemoveRange(_userProductsConsumed);

            //remove from UserProductsConsumed
            var _productStores = db.StoreProducts.Where(c => c.ProductId == products.Id);
            db.StoreProducts.RemoveRange(_productStores);

            db.SaveChanges();

            Session["goToSavedproductsIndexPage"] = true;

            return RedirectToAction("Index");
        }

    }
}