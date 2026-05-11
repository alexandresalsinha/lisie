using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SpiroWeb.Helpers
{
    public static class RenderMvcView
    {
        public static string GetRazorViewAsString(object model, string filePath)
        {
            var st = new StringWriter();
            var context = new HttpContextWrapper(HttpContext.Current);
            var routeData = new RouteData();

            routeData.Values.Add("controller", "ShoppingCart");

            var controllerContext = new ControllerContext(new RequestContext(context, routeData), new FakeController());


            //RouteData routeData = new RouteData();
            //routeData.Values.Add("someRouteDataProperty", "someValue");
            //ControllerContext controllerContext = new ControllerContext { RouteData = routeData };
            //controller.ControllerContext = controllerContext;

            var razor = new RazorView(controllerContext, filePath, null, false, null);
            razor.Render(new ViewContext(controllerContext, razor, new ViewDataDictionary(model), new TempDataDictionary(), st), st);
            return st.ToString();
        }
    }

    public class FakeController : Controller
    {
    }
}