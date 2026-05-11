using ClassLibrary1;
using System;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class InstallController : Controller
    {
        public ActionResult Index()
        {
            //save to database

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _newMarketingEntry = new Marketing
                {
                    Campaign = "Amoreiras",
                    CreateDate = DateTime.Now
                };
                db.Marketing.Add(_newMarketingEntry);
                db.SaveChanges();
            }
            return View();
        }
    }
}