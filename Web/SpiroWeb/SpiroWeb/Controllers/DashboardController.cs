using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class DashboardController : Controller
    {

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Marisol()
        {
            return View();
        }
    }
}