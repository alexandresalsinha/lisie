using System.Web.Mvc;
using System.Web.Routing;

namespace SpiroWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    name: "GetProductMetadata",
            //    url: "{controller}/{action}/{url}",
            //    defaults: new { controller = "ProductsWebServices", action = "GetMetadataFromJumboProduct", url= UrlParameter.Optional }
            //);
        }
    }
}
