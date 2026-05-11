using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SpiroWeb.Startup))]
namespace SpiroWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            Microsoft.AspNet.SignalR.StockTicker.Startup.ConfigureSignalR(app);
        }
    }
}
