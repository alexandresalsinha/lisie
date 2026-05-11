using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class WebController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("https://lisiev2.netlify.app/");
        }

    }
}