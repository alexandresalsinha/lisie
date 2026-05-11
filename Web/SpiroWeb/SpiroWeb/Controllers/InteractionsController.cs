using ClassLibrary1;
using SpiroWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class InteractionsController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        [Authorize]
        // GET: Interactions
        public ActionResult Index()
        {
            return View(db.Interactions.OrderByDescending(c => c.CreateDate).ToList());
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
            Statistics _currentStatistic = Managers.StatisticsManager.GetCurrent();
            List<Statistics> _Statistics = Managers.StatisticsManager.GetAll();
            List<Statistics> _result = new List<Statistics>();
            _result.Add(_currentStatistic);
            _result.AddRange(_Statistics);

            //get all users
            //TotalsInteractionsModel _TotalsInteractionsModel = new TotalsInteractionsModel();
            //_TotalsInteractionsModel.Users = db.AspNetUsers.Count();
            //_TotalsInteractionsModel.Products = db.Products.Count();
            //_TotalsInteractionsModel.Interactions = db.Interactions.Count();
            //_TotalsInteractionsModel.ProductsAddedToConsumed = db.UserProductsListHistory.Where(c => c.ListName == "consumed").Count();
            //_TotalsInteractionsModel.ProductsAddedToShoppingList = db.UserProductsListHistory.Where(c => c.ListName == "shoppingList").Count();
            //_TotalsInteractionsModel.ProductsAddedToInventory = db.UserProductsListHistory.Where(c => c.ListName == "inventory").Count();
            //_TotalsInteractionsModel.LisieHomeBarcodeScanned = db.Interactions.Where(c => c.Name == "LisieHomeBarcodeScanned").Count();
            //_TotalsInteractionsModel.LisieHomeProductsAddedToConsumed = db.Interactions.Where(c => c.Name == "LisieHomeConsumedProductAdded").Count();

            //Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastNewInteractions("9ff8224f-17cf-49fb-b555-05779a13eb40", "25", "4646", "34533");
            //Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastNewProduct("9ff8224f-17cf-49fb-b555-05779a13eb40", Managers.ProductsManager.GetTotal() + 1);
            //Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastNewStoreProduct("9ff8224f-17cf-49fb-b555-05779a13eb40", Managers.ProductsManager.GetTotalStoreProducts() + 1);
            //Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastNewUser("9ff8224f-17cf-49fb-b555-05779a13eb40", Managers.UsersManager.GetTotal() + 1);
            return View(_result);
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

        [HttpGet]
        public ActionResult RecordCurrentTotals()
        {
            bool _sucess = Managers.StatisticsManager.Record();
            return Json(new Models.JsonBotResponse
            {
                Success = _sucess
            }, JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        public ActionResult Realtime(string orderBy = "")
        {
            //get all users
            List<Interactions> _interactions = Managers.InteractionsManager.GetAll();
            return View(_interactions.Take(20));
        }
    }
}
