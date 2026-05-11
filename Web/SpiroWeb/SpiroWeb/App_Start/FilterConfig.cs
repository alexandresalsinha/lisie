using System.Web.Mvc;

namespace SpiroWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new OutputCacheAttribute
            {
                NoStore = true,
                Duration = 0,
                VaryByParam = "*",
                Location = System.Web.UI.OutputCacheLocation.None
            });
        }
    }
}
