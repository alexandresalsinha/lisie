using ClassLibrary1;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SpiroWeb.Controllers.api
{
    //TODO - refactor to here
    public class BroadcasterController : ApiController
    {

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage BroadcastStoreProductPriceUpdate(string userId, [FromBody] ProductPricesUpdates productPriceUpdate)
        {
            Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastStoreProductPriceUpdate(userId, productPriceUpdate);
            return Request.CreateResponse(HttpStatusCode.Created, productPriceUpdate);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage BroadcastStoreProductUpdate(string userId, [FromBody] StoreProducts storeProduct)
        {
            Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastStoreProductUpdate(userId, storeProduct);
            return Request.CreateResponse(HttpStatusCode.Created, storeProduct);
        }

    }
}
