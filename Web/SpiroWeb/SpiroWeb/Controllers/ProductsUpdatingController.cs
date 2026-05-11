using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class ProductsUpdatingController : Controller
    {
        // GET: Products1
        [Authorize]
        public ActionResult Index(string orderBy, string searchQuery, int page = 1)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _usersIdDistinct = db.ProductsUpdating.DistinctBy(c => c.UserId).ToList();
                    Dictionary<string, int> _dict = new Dictionary<string, int>();
                    int _totalCount = 0;
                    foreach (var _userId in _usersIdDistinct)
                    {
                        var _count = db.ProductsUpdating.Where(c => c.UserId == _userId.UserId).Count();
                        _dict.Add(_userId.UserId, _count);
                        _totalCount += _count;
                    }
                    _dict.Add("Total", _totalCount);

                    return View(_dict);
                }
            }
            catch (Exception)
            {

                return View();
            }
        }

        [Authorize]
        public ActionResult Delete(string serverName)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _rowsToDelete = db.ProductsUpdating.Where(c => c.UserId == serverName);
                    db.ProductsUpdating.RemoveRange(_rowsToDelete);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");

            }
            catch (Exception)
            {

                return View();
            }
        }
    }
}