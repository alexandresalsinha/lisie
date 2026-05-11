using ClassLibrary1;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class NotificationsController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        // GET: Products1
        [System.Web.Mvc.Authorize]
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

        [System.Web.Mvc.Authorize]
        public ActionResult SendToAll(string title, string body, string data)
        {
            Managers.NotificationsManager.SendToAll(title, body, data);
            ViewBag.OnePageOfUsers = title;
            ViewBag.OnePageOfUsers = body;
            ViewBag.OnePageOfUsers = data;
            return RedirectToAction("Index");
        }

        public ActionResult SendToUser(string userId, string title2, string body2, string data2)
        {
            Managers.NotificationsManager.SendToUser(userId, title2, body2, data2);
            ViewBag.OnePageOfUsers = title2;
            ViewBag.OnePageOfUsers = body2;
            ViewBag.OnePageOfUsers = data2;
            return RedirectToAction("Index");
        }

    }
}