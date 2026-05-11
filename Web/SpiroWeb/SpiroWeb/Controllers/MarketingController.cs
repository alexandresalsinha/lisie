using ClassLibrary1;
using System.Linq;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class MarketingController : Controller
    {
        public ActionResult Index()
        {
            //save to database

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _entries = db.Marketing.OrderByDescending(c => c.CreateDate).ToList();
                return View(_entries);
            }
        }
    }
}