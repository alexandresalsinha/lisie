using ClassLibrary1;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
//using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class UserProductsListsController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        // GET: UserProductsLists
        [Authorize]
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            var userProductsList = db.UserProductsList.Include(u => u.Products).Include(u => u.AspNetUsers).Where(u => u.UserId.Equals(userId));
            return View(userProductsList.ToList());
        }

        public ActionResult All(string orderBy, string searchQuery, int page = 1)
        {
            var userProductsList = db.UserProductsList.OrderByDescending(c => c.Id);
            //_distinctProductId[0].
            //var _oldestUnUpdatedStoreProducts =
            //                from prod in db.Products
            //                join storeProduct in _distinctProductId on prod.Id equals storeProduct.ProductId
            //                where storeProduct.UpdateDate != null
            //                orderby storeProduct.UpdateDate
            //                select new
            //                {
            //                    ProductId = prod.Id
            //                };

            //IQueryable<ClassLibrary1.UserProductsList> userProductsList = db.UserProductsList.Include(u => u.Products).DistinctBy(c=>c.ProductId).OrderByDescending(c=>c.Id);

            var pageNumber = page;

            if (Session["storeProductsIndexCurrentPage"] != null && Session["goToSavedStoreProductsIndexPage"] != null)
            {
                pageNumber = Convert.ToInt32(Session["storeProductsIndexCurrentPage"]);
                Session["goToSavedStoreProductsIndexPage"] = null;
            }

            try
            {
                var onePageOfProducts = userProductsList.ToPagedList(pageNumber, 25);
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

        // GET: UserProductsLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProductsList userProductsList = db.UserProductsList.Find(id);
            if (userProductsList == null)
            {
                return HttpNotFound();
            }
            return View(userProductsList);
        }

        // GET: UserProductsLists/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            ViewBag.UserId = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: UserProductsLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProductId,UserId,ListName,Quantity,QuantityWeight")] UserProductsList userProductsList)
        {
            if (ModelState.IsValid)
            {
                db.UserProductsList.Add(userProductsList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", userProductsList.ProductId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Name", userProductsList.UserId);
            return View(userProductsList);
        }

        // GET: UserProductsLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProductsList userProductsList = db.UserProductsList.Find(id);
            if (userProductsList == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", userProductsList.ProductId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Name", userProductsList.UserId);
            return View(userProductsList);
        }

        // POST: UserProductsLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductId,UserId,ListName,Quantity,QuantityWeight")] UserProductsList userProductsList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userProductsList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", userProductsList.ProductId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Name", userProductsList.UserId);
            return View(userProductsList);
        }

        // GET: UserProductsLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProductsList userProductsList = db.UserProductsList.Find(id);
            if (userProductsList == null)
            {
                return HttpNotFound();
            }
            return View(userProductsList);
        }

        // POST: UserProductsLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserProductsList userProductsList = db.UserProductsList.Find(id);
            db.UserProductsList.Remove(userProductsList);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize]
        public ActionResult TotalSavings(string userId = "", int pageSize = 40, int page = 1)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    return View(db.UserTotalSavings.Where(c => c.UserId == userId).OrderByDescending(c => c.CreateDate).Skip((page - 1) * pageSize).Take(pageSize).ToList());
                }
                else
                {
                    return View(db.UserTotalSavings.OrderByDescending(c => c.CreateDate).Skip((page - 1) * pageSize).Take(pageSize).ToList());
                }
            }
        }


        [Authorize]
        public ActionResult TotalSavingsReports(string userId, string dateStart = "", string dateEnd = "")
        {
            DateTime _startDate = (dateStart != string.Empty) ? DateTime.Parse(dateStart) : DateTime.MinValue;
            DateTime _endDate = (dateEnd != string.Empty) ? DateTime.Parse(dateEnd) : DateTime.MinValue;
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                if (_startDate == DateTime.MinValue)
                {
                    return View(db.UserTotalSavings.Where(c => c.UserId == userId).OrderByDescending(c => c.CreateDate).ToList());
                }
                else
                {
                    DateTime _startDateParsed = new DateTime(_startDate.Year, _startDate.Month, _startDate.Day, 0, 0, 0);
                    DateTime _endDateParsed = new DateTime(_endDate.Year, _endDate.Month, _endDate.Day, 23, 59, 59);

                    //var _list = db.UserTotalSavings.Where(c => c.UserId == userId).OrderByDescending(c => c.CreateDate).ToList();
                    //var _newList = new List<UserTotalSavings>();
                    //for (int i = 0; i < _list.Count; i++)
                    //{
                    //    var _item = _list[i];
                    //    var _nextItem = _list[i + 1];

                    //    TimeSpan _ts = _item. 
                    //}
                    var _data = db.UserTotalSavings.Where(c => c.UserId == userId && c.CreateDate >= _startDateParsed && c.CreateDate <= _endDateParsed).OrderByDescending(c => c.CreateDate).ToList();
                    return View(_data);

                }


            }
        }
    }
}
