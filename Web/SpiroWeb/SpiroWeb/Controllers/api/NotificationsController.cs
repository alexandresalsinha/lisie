using SpiroWeb.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SpiroWeb.Controllers.api
{
    //TODO - refactor to here
    public class NotificationsController : ApiController
    {

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> SendExpoNotification(ExpoNotificationModel model)
        {
            var _response = Helpers.ExpoNotifications.Send(model);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> SendToUser(ExpoNotificationModel model)
        {
            var _response = Managers.NotificationsManager.SendToUser(model.to[0], model.title, model.body, model.data);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> SendToAll(ExpoNotificationModel model)
        {
            var _response = Managers.NotificationsManager.SendToAll(model.title, model.body, model.data);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> FinishedAddingProductsToOnlineStore([FromBody] FinishedAddingProductsToOnlineStoresModel model)
        {
            var _response = Managers.NotificationsManager.FinishedAddingProductsToOnlineStore(model);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }



        //[HttpGet]
        //[HttpPost]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        //public async Task<HttpResponseMessage> SendMaxSavingsNotifications()
        //{
        //    var _response = Managers.NotificationsManager.SendToAll(model.title, model.body, model.data);
        //    return Request.CreateResponse(HttpStatusCode.OK, _response);
        //}
    }

    public class FinishedAddingProductsToOnlineStoresModel
    {
        public string UserId { get; set; }

        public List<FinishedAddingProductsToOnlineStoreModel> FinishedStores { get; set; }
    }

    public class FinishedAddingProductsToOnlineStoreModel
    {

        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public bool Success { get; set; }
        public List<int> ProductsNotAdded { get; set; }
    }


}
