using Microsoft.AspNet.Identity;
using Owin;

namespace Microsoft.AspNet.SignalR.StockTicker
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            // your logic to fetch a user identifier goes here.

            // for example:
            //try
            //{
            string userId = request.User.Identity.GetUserId();
            return userId.ToString();
            //}
            //catch (Exception ex)
            //{
            //    return string.Empty;
            //}
        }
    }
    public enum CookieSecureOption
    {
        SameAsRequest,
        Never,
        Always
    }
    public static class Startup
    {
        public static void ConfigureSignalR(IAppBuilder app)
        {
            // For more information on how to configure your application using OWIN startup, visit http://go.microsoft.com/fwlink/?LinkID=316888
            var idProvider = new CustomUserIdProvider();
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);
            app.MapSignalR();
        }
    }
}