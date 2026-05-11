using ClassLibrary1;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class UsersController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        // GET: Products1
        [Authorize]
        public ActionResult Index(int? page, string orderBy, string searchQuery)
        {
            //var products = (string.IsNullOrEmpty(orderBy)) ? db.Products.ToList() : db.Products.OrderBy(c => c.Name).ToList();
            List<AspNetUsers> users = new List<AspNetUsers>();

            if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(searchQuery))
            {
                users = db.AspNetUsers.Where(c => c.Email.ToLower().Contains(searchQuery.ToLower())).OrderBy(c => c.Email).ToList();
            }
            else if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "id":
                        users = db.AspNetUsers.OrderBy(c => c.Id).ToList();
                        break;
                    case "email":
                        users = db.AspNetUsers.OrderBy(c => c.Email).ToList();
                        break;
                    default:
                        break;
                }
            }
            else if (!string.IsNullOrEmpty(searchQuery))
            {
                users = db.AspNetUsers.Where(c => c.Email.ToLower().Contains(searchQuery.ToLower())).ToList();
            }
            else
            {
                users = db.AspNetUsers.ToList();
            }

            //var pageNumber = page ?? 1;

            //if (Session["productsIndexCurrentPage"] != null && Session["goToSavedproductsIndexPage"] != null)
            //{
            //    pageNumber = Convert.ToInt32(Session["productsIndexCurrentPage"]);
            //    Session["goToSavedproductsIndexPage"] = null;
            //}

            //var onePageOfProducts = products.ToPagedList(pageNumber, 25);

            //Session["productsIndexCurrentPage"] = pageNumber;


            ViewBag.OnePageOfUsers = users;
            return View();
        }

        // GET: Products1/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers users = db.AspNetUsers.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // GET: Users/Delete/5
        [HttpGet]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AspNetUsers user = db.AspNetUsers.Where(c => c.Id == id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            else
            {

                //First delete UserProductsList
                var _UserProductsList = db.UserProductsList.Where(c => c.UserId == id);
                db.UserProductsList.RemoveRange(_UserProductsList);

                //First delete UserProductsSimple
                var _UserProductsSimple = db.UserProductsSimple.Where(c => c.UserId == id);
                db.UserProductsSimple.RemoveRange(_UserProductsSimple);

                //First delete UserProductsSimple
                var _UserProductsListHistory = db.UserProductsListHistory.Where(c => c.UserId == id);
                db.UserProductsListHistory.RemoveRange(_UserProductsListHistory);

                db.AspNetUsers.Remove(user);
                db.SaveChanges();
            }
            return RedirectToAction("Users", "Interactions");
        }


        // POST: Products1/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Products products = db.Products.Find(id);
        //    db.Products.Remove(products);

        //    //remove from UserProductsConsumed
        //    var _userProductsConsumed = db.UserProductsConsumed.Where(c => c.ProductId == products.Id);
        //    db.UserProductsConsumed.RemoveRange(_userProductsConsumed);

        //    //remove from UserProductsConsumed
        //    var _productStores = db.StoreProducts.Where(c => c.ProductId == products.Id);
        //    db.StoreProducts.RemoveRange(_productStores);

        //    db.SaveChanges();

        //    Session["goToSavedproductsIndexPage"] = true;

        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult DeleteInstructions()
        {
            return View();
        }

    }
}