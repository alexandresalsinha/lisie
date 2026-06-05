using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SpiroWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public Helpers.FirebaseTimer FirebaseTimerToExportToSql;

        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "productsWebServices",                                              // Route name
                "productsWebServices/{action}/{searchQuery}",                           // URL with parameters
                new { controller = "productsWebServices", action = "GetOnlineProductSearchResults", searchQuery = "" }  // Parameter defaults
            );
        }

        protected async void Application_Start()
        {
            // Register Web API routing support before anything else
            GlobalConfiguration.Configure(WebApiConfig.Register);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            //FirebaseTimerToExportToSql = new Helpers.FirebaseTimer();
            //FirebaseTimerToExportToSql.Timer();

            //put json as the default response
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            
            
            //var _products = Managers.UserListsManager.GetV4("9ff8224f-17cf-49fb-b555-05779a13eb40");
            //foreach (var product in _products)
            //{
            //    var _product = product;
            //    await Managers.ProductsManager.UpdatePricesNew(product.Id);

            //}
        }
    }
}
