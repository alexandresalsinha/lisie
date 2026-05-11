using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SpiroWeb.Controllers
{
    public class LisieHomeController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetUserMode(string userId)
        {
            Managers.InteractionsManager.Add(userId, "/LisieHome/GetUserMode", string.Empty);

            var _result = Managers.LisieHomeManager.GetUserState(userId);
            return Request.CreateResponse(HttpStatusCode.OK, _result);
        }

        [HttpGet]
        public HttpResponseMessage SetUserMode(string userId, string mode)
        {
            Managers.InteractionsManager.Add(userId, "/LisieHome/SetUserMode", mode);

            if (mode.Trim().ToLower() == "consumed" || mode.Trim().ToLower() == "bought")
            {
                var _result = Managers.LisieHomeManager.SetUserState(userId, mode);
                return Request.CreateResponse(HttpStatusCode.OK, _result);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, "mode has to be 'consumed' or 'bought'");
        }

        [HttpGet]
        public async Task<HttpResponseMessage> AddProduct(string userId, string barCode)
        {
            int _UserProductsListId = await Helpers.ProductsQueue.ProcessProductNew(barCode, userId, true);

            //TODO - IMPROVE NOTIFICATINS, WHERE OR THERE?!
            if (_UserProductsListId != -1)
            {
                Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCart(_UserProductsListId, userId);
            }
            else
            {
                Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCartProductsInQueue(barCode, userId);
            }

            return Request.CreateResponse(HttpStatusCode.OK, _UserProductsListId);
        }
    }
}
