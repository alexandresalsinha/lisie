using ClassLibrary1;
using System.Collections.Generic;
using System.Linq;
//using StoresPuppetter;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SpiroWeb.Controllers
{
    public class PuppetterController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        // GET: Puppetter
        public ActionResult Index()
        {
            return View();
        }

        //public async Task<JsonResult> GetGoogleHrefs()
        //{
        //    string test = await StoresPuppetter.Proccess.GetGoogleHrefs(Server.MapPath("~") + "\\Chromium");
        //    return Json(test, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public async Task<JsonResult> AddUserListToJumbo(string userId, string storeUsername, string storePassword)
        {
            var userProductsList = db.UserProductsList.Where(u => u.UserId.Equals(userId)).Where(u => u.ListName.ToLower().Equals("in"));
            List<LisieStores.Extensibility.ProductAddToOnlineStore> _productsToAdd = new List<LisieStores.Extensibility.ProductAddToOnlineStore>();
            foreach (var userProduct in userProductsList)
            {
                var userShoppingList = from m in db.StoreProducts where m.ProductId == userProduct.ProductId select m;
                if (userShoppingList.Count() > 0)
                {
                    foreach (var storeProduct in userShoppingList)
                    {
                        if (storeProduct.Stores.Id == 1) _productsToAdd.Add(new LisieStores.Extensibility.ProductAddToOnlineStore
                        {
                            UserProductListId = userProduct.Id,
                            Name = storeProduct.Products.Name,
                            Url = storeProduct.Url,
                            Quantity = userProduct.Quantity.Value
                        });
                    }
                }
            }

            var cli = new WebClient();
            cli.Headers[HttpRequestHeader.ContentType] = "application/json";

            var _nodeRequest = new NodeRequest
            {
                username = storeUsername,
                password = storePassword,
                products = _productsToAdd
            };
            var json = new JavaScriptSerializer().Serialize(_nodeRequest);
            string response = cli.UploadString("https://puppeteer-lisie.herokuapp.com/addUserListToJumbo/" + userId, json);

            //WarnMeOfAddingToJumboOnline(userId, storeUsername);
            return Json(_nodeRequest, JsonRequestBehavior.AllowGet);

            //bool result = await StoresPuppetter.Jumbo.AddProducts(_productsToAdd, Server.MapPath("~") + "\\Chromium");
            //return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AddUserListToContinente(string userId, string storeUsername, string storePassword)
        {
            var userProductsList = db.UserProductsList.Where(u => u.UserId.Equals(userId)).Where(u => u.ListName.ToLower().Equals("in"));
            List<LisieStores.Extensibility.ProductAddToOnlineStore> _productsToAdd = new List<LisieStores.Extensibility.ProductAddToOnlineStore>();
            foreach (var userProduct in userProductsList)
            {
                var userShoppingList = from m in db.StoreProducts where m.ProductId == userProduct.ProductId select m;
                if (userShoppingList.Count() > 0)
                {
                    foreach (var storeProduct in userShoppingList)
                    {
                        if (storeProduct.Stores.Name.ToLower() == "continente") _productsToAdd.Add(new LisieStores.Extensibility.ProductAddToOnlineStore
                        {
                            UserProductListId = userProduct.Id,
                            Name = storeProduct.Products.Name,
                            Url = storeProduct.Url,
                            Quantity = userProduct.Quantity.Value
                        });
                    }
                }
            }

            var cli = new WebClient();
            cli.Headers[HttpRequestHeader.ContentType] = "application/json";


            var _nodeRequest = new NodeRequest
            {
                username = storeUsername,
                password = storePassword,
                products = _productsToAdd
            };
            var json = new JavaScriptSerializer().Serialize(_nodeRequest);
            string response = cli.UploadString("https://puppeteer-lisie.herokuapp.com/addUserListToContinente/" + userId, json);
            //string response = cli.UploadString("http://localhost:3000/addUserListToContinente/" + userId, json);

            WarnMeOfAddingToJumboOnline(userId, storeUsername);
            return Json(_nodeRequest, JsonRequestBehavior.AllowGet);
        }


        public void WarnMeOfAddingToJumboOnline(string userId, string jumboUsername)
        {
            //get user last device token

            var user = db.AspNetUsers.Where(c => c.Id.Equals(userId)).First();
            if (user != null)
            {
                DataManager.UserDevicesManager _userDevicesManager2 = new DataManager.UserDevicesManager();
                List<ClassLibrary1.UserDevices> _userDevicesTokens2 = _userDevicesManager2.GetUserDevicesTokens("d3d48305-4527-49ac-a930-49e4a511af14");

                if (_userDevicesTokens2.Count() > 0)
                {
                    foreach (ClassLibrary1.UserDevices _userDevice in _userDevicesTokens2)
                    {
                        Helpers.FirebaseAndroid.SendNotificationToAndroidPhone(_userDevice.DeviceToken, "addingToJumboOnline:" + jumboUsername);
                    }
                }
            }

        }

        public void WarnMeOfAddingToContinente(string userId, string continenteUsername)
        {
            //get user last device token

            var user = db.AspNetUsers.Where(c => c.Id.Equals(userId)).First();
            if (user != null)
            {
                DataManager.UserDevicesManager _userDevicesManager2 = new DataManager.UserDevicesManager();
                List<ClassLibrary1.UserDevices> _userDevicesTokens2 = _userDevicesManager2.GetUserDevicesTokens("d3d48305-4527-49ac-a930-49e4a511af14");

                if (_userDevicesTokens2.Count() > 0)
                {
                    foreach (ClassLibrary1.UserDevices _userDevice in _userDevicesTokens2)
                    {
                        Helpers.FirebaseAndroid.SendNotificationToAndroidPhone(_userDevice.DeviceToken, "addingToContinenteOnline:" + continenteUsername);
                    }
                }
            }

        }

        [HttpGet]
        public async Task<JsonResult> GetProductsToAddToOnline(string userId, string storeUsername, string storePassword)
        {
            var userProductsList = db.UserProductsList.Where(u => u.UserId.Equals(userId)).Where(u => u.ListName.ToLower().Equals("in"));
            List<LisieStores.Extensibility.ProductAddToOnlineStore> _productsToAdd = new List<LisieStores.Extensibility.ProductAddToOnlineStore>();
            foreach (var userProduct in userProductsList)
            {
                var userShoppingList = from m in db.StoreProducts where m.ProductId == userProduct.ProductId select m;
                if (userShoppingList.Count() > 0)
                {
                    foreach (var storeProduct in userShoppingList)
                    {
                        if (storeProduct.Stores.Name == "Jumbo") _productsToAdd.Add(new LisieStores.Extensibility.ProductAddToOnlineStore
                        {
                            Url = storeProduct.Url,
                            Quantity = userProduct.Quantity.Value
                        });
                    }
                }
            }

            var json = new NodeRequest
            {
                username = storeUsername,
                password = storePassword,
                products = _productsToAdd
            };
            return Json(json, JsonRequestBehavior.AllowGet);

            //bool result = await StoresPuppetter.Jumbo.AddProducts(_productsToAdd, Server.MapPath("~") + "\\Chromium");
            //return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //public async Task<JsonResult> NotifyProductsAddedToJumboOnline(string userId, string timespanseconds)
        //{
        //    var user = db.AspNetUsers.Where(c => c.Id.Equals(userId)).First();
        //    if (user != null)
        //    {
        //        Helpers.FirebaseAndroid.SendNotification(userId, "productsAddedToJumboOnline:Acabei de adicionar a tua lista de compras ao Jumbo Online");
        //    }
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        //public async Task<JsonResult> NotifyProductsAddedToJumboOnline(StoresPuppetter.AddToJumboOnlineFinishedModel model)
        //{
        //    var user = db.AspNetUsers.Where(c => c.Id.Equals(model.UserId)).First();
        //    if (user != null)
        //    {
        //        var json = new JavaScriptSerializer().Serialize(model);
        //        Helpers.FirebaseAndroid.SendNotification(model.UserId, json);
        //    }
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public async Task<JsonResult> NotifyProductsAddedStore(string userId, string store, string timespanseconds)
        {
            var user = db.AspNetUsers.Where(c => c.Id.Equals(userId)).First();
            if (user != null)
            {
                //Helpers.FirebaseAndroid.SendNotification(userId, "productsAddedToStore:" + store + " - Done adding your product/s in " + timespanseconds + "s ;)");
                Helpers.FirebaseAndroid.SendNotification("9ff8224f-17cf-49fb-b555-05779a13eb40", "productsAddedToStore:" + user.UserName + " adicionou á loja " + store);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }


    }

    public class NodeRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public List<LisieStores.Extensibility.ProductAddToOnlineStore> products { get; set; }
    }
}