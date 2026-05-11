using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class SController : Controller
    {
        public ActionResult l(string i)
        {
            if (Request.Browser.IsMobileDevice)
            {
                return Redirect("com.lisie.org://ShoppingListShared?userId=" + i);
            }
            else
            {
                return Redirect("https://6156328e7d57011c1d209b6f--lisie-v2.netlify.app/ShoppingListShared?userId=" + i);
            }
        }

        public ActionResult r(string screen)
        {
            var _query = Request.QueryString.ToString();
            int _startIndex = _query.IndexOf("screen=");
            int _endIndex = _query.IndexOf("&", _startIndex) + 1;
            if (_endIndex > _startIndex)
            {
                _query = _query.Remove(_startIndex, _endIndex - _startIndex);
            }
            else
            {
                _query = _query.Remove(_startIndex);
            }

            if (!string.IsNullOrEmpty(_query))
            {
                _query = "?" + _query;
            }

            return Redirect("com.lisie.org://" + screen + _query);
        }

        public ActionResult p(int id)
        {
            string _urlRedirect = string.Empty;
            if (Request.Browser.IsMobileDevice)
            {
                //return Redirect("com.lisie.org://Product?productId=" + id);
                _urlRedirect = "com.lisie.org://Product?productId=" + id + "&isShared=true";
            }
            else
            {
                //return Redirect("https://6156328e7d57011c1d209b6f--lisie-v2.netlify.app/Product?productId=" + id);
                _urlRedirect = "https://6156328e7d57011c1d209b6f--lisie-v2.netlify.app/Product?productId=" + id + "&isShared=true";
            }
            ViewBag.UrlRedirect = _urlRedirect;
            ViewBag.Url = "https://lisie.app/s/p?id=" + id.ToString();

            var _product = Managers.ProductsManager.GetById(id);
            if (_product != null)
            {
                ViewBag.Title = _product.Name;
                ViewBag.Description = "Vê este produto na Lisie";
                ViewBag.Image = "https://lisie.app//handlers/GetProductImage.ashx?productId=" + id.ToString();
            }
            return View();
        }
    }
}