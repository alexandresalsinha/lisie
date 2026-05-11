using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class ReportsController : Controller
    {

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}