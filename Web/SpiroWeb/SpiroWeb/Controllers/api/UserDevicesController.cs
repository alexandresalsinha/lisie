using SpiroWeb.Helpers;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SpiroWeb.Controllers.Api
{

    public class UserDevicesController : ApiController
    {
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage Get(string userId)
        {
            if (userId != string.Empty)
            {
                var _userDevices = Managers.UserDevicesManager.GetUserDevicesTokens(userId);
                return Request.CreateResponse(HttpStatusCode.Created, _userDevices);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage Add([FromBody] AddUserDeviceModel model)
        {
            try
            {
                var _addedUserDevice = Managers.UserDevicesManager.AddUserDevice(model.UserId, model.DeviceId, model.DeviceToken, model.OperativeSystem, model.ModelName, model.LisieVersion);
                return Request.CreateResponse(HttpStatusCode.Created, _addedUserDevice);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.InnerException.Message);
                return Request.CreateResponse(HttpStatusCode.Created, false);
            }
        }

        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage AddV2([FromBody] AddUserDeviceModel model)
        {
            try
            {
                var _addedUserDevice = Managers.UserDevicesManager.AddUserDeviceV2(model.UserId, model.DeviceId, model.DeviceToken, model.OperativeSystem, model.ModelName, model.LisieVersion);
                return Request.CreateResponse(HttpStatusCode.Created, _addedUserDevice);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.InnerException.Message);
                return Request.CreateResponse(HttpStatusCode.Created, false);
            }
        }
    }

    public class AddUserDeviceModel
    {
        public string UserId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceToken { get; set; }
        public string OperativeSystem { get; set; }
        public string ModelName { get; set; }
        public string LisieVersion { get; set; }
    }
}
