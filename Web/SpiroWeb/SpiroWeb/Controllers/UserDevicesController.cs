using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class UserDevicesController : Controller
    {
        [HttpGet]
        public JsonResult UpdateUserDevice(string userId, string deviceId, string deviceToken, string operativeSystem)
        {
            DataManager.UserDevicesManager _UserDevicesManger = new DataManager.UserDevicesManager();

            bool _success = _UserDevicesManger.UpdateUserDevice(userId, deviceId, deviceToken, operativeSystem);
            return Json(_success, JsonRequestBehavior.AllowGet);
        }
    }
}