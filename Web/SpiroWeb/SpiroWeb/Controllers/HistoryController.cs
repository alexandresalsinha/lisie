using ClassLibrary1;
using Microsoft.AspNet.Identity;
using SpiroWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class HistoryController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        [Authorize]
        public ActionResult Index(string userId = "", string type = "all", int month = -1, int year = -1)
        {
            string _userId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (userId == string.Empty)
                userId = _userId;


            var _userHistory = new List<UserProductsListHistoryModel>();
            if (month == -1 && year == -1)
            {
                _userHistory = Managers.UserHistoryManager.GetAll(userId);
            }
            else
            {
                if (month == -1)
                    month = DateTime.Now.Month;
                if (year == -1)
                    year = DateTime.Now.Year;

                _userHistory = Managers.UserHistoryManager.GetOfMonthYear(userId, month, year);
            }

            return View(_userHistory);
        }

        [Authorize]
        public ActionResult Users(string orderBy = "")
        {
            //get all users
            List<UserInteractionsModel> _listUserInteractionsModel = new List<UserInteractionsModel>();
            foreach (var user in db.AspNetUsers)
            {
                UserInteractionsModel _UserInteractionsModel = new UserInteractionsModel();
                _UserInteractionsModel.UserId = user.Id;
                _UserInteractionsModel.Email = user.Email;
                _UserInteractionsModel.CreateDate = (user.CreateDate.HasValue) ? user.CreateDate.Value : DateTime.MinValue;
                _UserInteractionsModel.Confirmed = user.EmailConfirmed;
                _UserInteractionsModel.Interactions = db.Interactions.Where(c => c.UserId.Equals(user.Id)).Count();

                Interactions _lastInteraction = db.Interactions.Where(c => c.UserId.Equals(user.Id)).OrderByDescending(c => c.CreateDate).FirstOrDefault();
                if (_lastInteraction != null)
                {
                    _UserInteractionsModel.LastInteractionName = _lastInteraction.Name;
                    _UserInteractionsModel.LastInteractionDate = _lastInteraction.CreateDate;
                }
                _UserInteractionsModel.ProductsAddedToConsumed = db.UserProductsListHistory.Where(c => c.UserId.Equals(user.Id) && c.ListName == "consumed").Count();
                _UserInteractionsModel.ProductsAddedToShoppingList = db.UserProductsListHistory.Where(c => c.UserId.Equals(user.Id) && c.ListName == "shoppingList").Count();
                _UserInteractionsModel.ProductsAddedToMarket = db.Products.Where(c => c.CreatedByUserId.Equals(user.Id)).Count();

                int _lisieHomeBarcodeScanned = db.Interactions.Where(c => c.UserId.Equals(user.Id) && c.Name == "LisieHomeBarcodeScanned").Count();
                int _lisieHomeProductsAddedToConsumed = db.Interactions.Where(c => c.UserId.Equals(user.Id) && c.Name == "LisieHomeConsumedProductAdded").Count();

                _UserInteractionsModel.LHbarcodeScanned = _lisieHomeBarcodeScanned;
                _UserInteractionsModel.LHProductsAddedToConsumed = _lisieHomeProductsAddedToConsumed;

                _listUserInteractionsModel.Add(_UserInteractionsModel);
            }
            _listUserInteractionsModel = _listUserInteractionsModel.OrderByDescending(c => c.ProductsAddedToMarket).ToList();

            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "interactions":
                        _listUserInteractionsModel = _listUserInteractionsModel.OrderByDescending(c => c.Interactions).ToList();
                        break;
                    case "lastinteractiondate":
                        _listUserInteractionsModel = _listUserInteractionsModel.OrderByDescending(c => c.LastInteractionDate).ToList();
                        break;
                    case "barcodescanned":
                        _listUserInteractionsModel = _listUserInteractionsModel.OrderByDescending(c => c.LHbarcodeScanned).ToList();
                        break;
                    case "productsaddedtomarket":
                        _listUserInteractionsModel = _listUserInteractionsModel.OrderByDescending(c => c.ProductsAddedToMarket).ToList();
                        break;
                    case "createdate":
                        _listUserInteractionsModel = _listUserInteractionsModel.OrderByDescending(c => c.CreateDate).ToList();
                        break;
                    default:
                        break;
                }
            }

            //foreach, get totals
            return View(_listUserInteractionsModel);
        }

        [Authorize]
        public ActionResult Totals()
        {
            //get all users
            TotalsInteractionsModel _TotalsInteractionsModel = new TotalsInteractionsModel();
            _TotalsInteractionsModel.Users = db.AspNetUsers.Count();
            _TotalsInteractionsModel.Products = db.Products.Count();
            _TotalsInteractionsModel.Interactions = db.Interactions.Count();
            _TotalsInteractionsModel.ProductsAddedToConsumed = db.UserProductsListHistory.Where(c => c.ListName == "consumed").Count();
            _TotalsInteractionsModel.ProductsAddedToShoppingList = db.UserProductsListHistory.Where(c => c.ListName == "shoppingList").Count();
            _TotalsInteractionsModel.ProductsAddedToInventory = db.UserProductsListHistory.Where(c => c.ListName == "inventory").Count();
            _TotalsInteractionsModel.LisieHomeBarcodeScanned = db.Interactions.Where(c => c.Name == "LisieHomeBarcodeScanned").Count();
            _TotalsInteractionsModel.LisieHomeProductsAddedToConsumed = db.Interactions.Where(c => c.Name == "LisieHomeConsumedProductAdded").Count();

            return View(_TotalsInteractionsModel);
        }

        [Authorize]
        public ActionResult User(string userId)
        {
            return View(Managers.InteractionsManager.GetOfUser(userId));
        }

        [Authorize]
        // GET: Interactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Interactions interactions = db.Interactions.Find(id);
            if (interactions == null)
            {
                return HttpNotFound();
            }
            return View(interactions);
        }

        // GET: Interactions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Interactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Name,Extra,CreateDate")] Interactions interactions)
        {
            if (ModelState.IsValid)
            {
                db.Interactions.Add(interactions);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(interactions);
        }

        // GET: Interactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Interactions interactions = db.Interactions.Find(id);
            if (interactions == null)
            {
                return HttpNotFound();
            }
            return View(interactions);
        }

        // POST: Interactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Name,Extra,CreateDate")] Interactions interactions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(interactions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(interactions);
        }

        // GET: Interactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Interactions interactions = db.Interactions.Find(id);
            if (interactions == null)
            {
                return HttpNotFound();
            }
            return View(interactions);
        }

        // POST: Interactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Interactions interactions = db.Interactions.Find(id);
            db.Interactions.Remove(interactions);
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
    }
}
