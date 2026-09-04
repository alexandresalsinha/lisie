using ClassLibrary1;
using SpiroWeb.Helpers;
using SpiroWeb.Models;
using SpiroWeb.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;

namespace SpiroWeb.Controllers
{
    public static class UserListHelper
    {
        public static HttpResponseMessage NoCache(this HttpResponseMessage response)
        {
            response.Headers.Add("CACHE-CONTROL", "no-cache, no-store, must-revalidate");
            response.Headers.Add("Pragma", "no-cache");
            //response.Headers.Add("Expires", "0");
            //response.Headers.CacheControl = new CacheControlHeaderValue
            //{ Public = true, MaxAge = TimeSpan.FromSeconds(0), NoStore=true, NoCache=true };
            return response;
        }
    }

    public class UploadImageModel
    {
        public int Id { get; set; }
        public string Image { get; set; }
    }
    public class UpdateProductSimpleNameModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AddProductSimpleV2Model
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string List { get; set; }
        public string ImageBase64 { get; set; }
    }

    public class UserListsController : ApiController
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        // GET: api/UserLists
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/UserLists/5



        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage Get(string userId, string list)
        {
            if (userId != string.Empty && list != string.Empty)
            {
                Managers.InteractionsManager.Add(userId, "api/UserLists/Get", "listName:" + list);

                var response = new HttpResponseMessage();

                List<Models.UserProductListCompleteModel2> _userListProducts = Managers.UserListsManager.Get(userId, list);
                Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");
                Logger.Debug(new JavaScriptSerializer().Serialize(_userListProducts), "getuserlist.txt");
                //if (_userListProducts != null)
                //    return Request.CreateResponse(HttpStatusCode.OK, _userListProducts);
                //return Request.CreateResponse(HttpStatusCode.NotFound);
                if (_userListProducts != null)
                    response = Request.CreateResponse(HttpStatusCode.OK, _userListProducts);
                else
                    response = Request.CreateResponse(HttpStatusCode.NotFound);

                return response.NoCache();
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetV2(string userId, string list)
        {
            if (userId != string.Empty && list != string.Empty)
            {
                Managers.InteractionsManager.Add(userId, "api/UserLists/Get", "listName:" + list);

                var response = new HttpResponseMessage();
                List<Models.UserProductListCompleteModel2> _userListProducts = Managers.UserListsManager.GetV2(userId, list);
                //Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");
                //Logger.Debug(new JavaScriptSerializer().Serialize(_userListProducts), "getuserlist.txt");
                if (_userListProducts != null)
                    response = Request.CreateResponse(HttpStatusCode.OK, _userListProducts);
                else
                    response = Request.CreateResponse(HttpStatusCode.NotFound);

                return response.NoCache();
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }


        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetV3(string userId)
        {
            if (userId != string.Empty)
            {
                Managers.InteractionsManager.Add(userId, "api/UserLists/GetV3", "");

                var response = new HttpResponseMessage();
                List<Models.UserProductListCompleteModel2> _userListProducts = Managers.UserListsManager.GetV3(userId);
                //Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");
                //Logger.Debug(new JavaScriptSerializer().Serialize(_userListProducts), "getuserlist.txt");
                if (_userListProducts != null)
                    response = Request.CreateResponse(HttpStatusCode.OK, _userListProducts);
                else
                    response = Request.CreateResponse(HttpStatusCode.NotFound);

                return response.NoCache();
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }


        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetV4(string userId)
        {
            if (userId != string.Empty)
            {
                Managers.InteractionsManager.Add(userId, "api/UserLists/GetV4", "");

                var response = new HttpResponseMessage();
                List<Models.UserProductListCompleteModel2> _userListProducts = Managers.UserListsManager.GetV4(userId);
                if (_userListProducts != null)
                    response = Request.CreateResponse(HttpStatusCode.OK, _userListProducts);
                else
                    response = Request.CreateResponse(HttpStatusCode.NotFound);

                return response.NoCache();
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetUserListsLastAdded(string userId)
        {
            if (userId != string.Empty)
            {
                Managers.InteractionsManager.Add(userId, "api/UserLists/GetUserListsLastAdded", "");

                var response = new HttpResponseMessage();
                List<Models.UserProductListCompleteModel2> _userListProducts = Managers.UserListsManager.GetUserListsLastAdded(userId);
                if (_userListProducts != null)
                    response = Request.CreateResponse(HttpStatusCode.OK, _userListProducts);
                else
                    response = Request.CreateResponse(HttpStatusCode.NotFound);

                return response.NoCache();
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetV5(string userId, bool withCategories = false)
        {
            if (userId != string.Empty)
            {
                Managers.InteractionsManager.Add(userId, "api/UserLists/GetV5", "");

                var response = new HttpResponseMessage();
                var _userListProducts = Managers.UserListsManager.GetV5(userId);
                if (_userListProducts != null)
                    response = Request.CreateResponse(HttpStatusCode.OK, _userListProducts);
                else
                    response = Request.CreateResponse(HttpStatusCode.NotFound);

                return response.NoCache();
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage SetUserProductsCategories(string userId)
        {
            if (userId != string.Empty)
            {
                Managers.InteractionsManager.Add(userId, "api/UserLists/GetV5", "");

                var response = new HttpResponseMessage();
                Managers.UserListsManager.SetUserProductsCategories(userId);

                response = Request.CreateResponse(HttpStatusCode.OK, string.Empty);

                return response.NoCache();
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // POST: api/UserLists
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage Post([FromBody] Models.UserProductListsPostModel data)
        {
            Managers.InteractionsManager.Add(data.UserId, "api/UserLists/Post", new JavaScriptSerializer().Serialize(data));
            if (data != null)
            {
                //TODO - send response with a complex object with all the userProductListId for each list - UserProductListsRspModel
                int _lastUserProductListId = -1;
                foreach (string list in data.Lists)
                {
                    _lastUserProductListId = Managers.UserListsManager.AddProductToList(data.ProductId, data.ProductName, list, data.Quantity, data.QuantityWeight, true, data.UserId);
                }
                //HttpResponseMessage _toReturn = Request.CreateResponse(HttpStatusCode.Created, _lastUserProductListId);
                //_toReturn.Headers.Add("Access-Control-Allow-Origin", "*");
                //return _toReturn;
                return Request.CreateResponse(HttpStatusCode.Created, _lastUserProductListId);

            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        //[HttpPost]
        //public HttpResponseMessage GetUsersShoppingListProducts([FromBody]List<string> requestedUsersShoppingList)
        //{
        //    if (requestedUsersShoppingList != null && requestedUsersShoppingList.Count > 0)
        //    {
        //        List<UserProductsList> _usersProductsList = new List<UserProductsList>();
        //        foreach (var userId in requestedUsersShoppingList)
        //        {
        //            var _userShoppingList = Managers.UserListsManager.GetOfUser(userId, "shoppingList");
        //            _usersProductsList.AddRange(_usersProductsList);
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, _usersProductsList);

        //    }
        //    else
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, "Requested Users Shopping List null or empty");

        //}

        //[HttpGet]
        //public HttpResponseMessage AddRequestUsersShoppingListProductsUpdatePrices(string userId)
        //{
        //    if (userId != string.Empty)
        //    {
        //        UserUpdatePricesRequests _UserUpdatePricesRequests = Managers.UserUpdatePricesRequestsManager.Add(userId);
        //        if (_UserUpdatePricesRequests != null)
        //            return Request.CreateResponse(HttpStatusCode.Created, _UserUpdatePricesRequests);
        //        else
        //            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        //    }
        //    else
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //}

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage RequestUsersShoppingListProductsUpdatePrices()
        {
            //List<UserProductsList> _userProductsList = new List<UserProductsList>();

            //var _users = db.AspNetUsers.ToList();
            //Add User Products Watchers first
            //foreach (var _userUpdatePricesRequest in _users)
            //{
            //get observers
            //var _userProductsWatchers = Managers.ProductsWatchersManager.GetProductIdsOfUser(_userUpdatePricesRequest.Id);
            //if (_userProductsWatchers.Count() > 0)
            //{
            //    _userProductsList.AddRange(_userProductsWatchers.Select(c => new UserProductsList
            //    {
            //        ProductId = c.ProductId
            //    }));
            //}
            //}
            //Add User Products of Lists
            //foreach (var _userUpdatePricesRequest in _users)
            //{
            //    //get user products of all lists
            //    var _userShoppingList = Managers.UserListsManager.GetOfUser(_userUpdatePricesRequest.Id, "all");
            //    _userProductsList.AddRange(_userShoppingList);

            //}
            ////call pupetter service

            //var _list = _userProductsList.Select(item => new
            //{
            //    item.ProductId
            //});

            var _list = Managers.UserListsManager.GetProductIdsOfAllUsers();

            var __list = _list.Select(item => new
            {
                ProductId = item
            });

            var cli = new WebClient();
            cli.Headers[HttpRequestHeader.ContentType] = "application/json";

            //FOR NOW DON´T DO NOTHING
            //return Request.CreateResponse(HttpStatusCode.OK, _list);

            try
            {
                var json = new JavaScriptSerializer().Serialize(__list);
                //Heroku server 1
                string response = cli.UploadString("https://puppeteer-lisie.herokuapp.com/updateProductsPrices", json);
                //Heroku server 2
                //string response = cli.UploadString("https://lisie.herokuapp.com/updateProductsPrices", json);
                //Localhost server
                //string response = cli.UploadString("http://localhost:3000/updateProductsPrices", json);
                return Request.CreateResponse(HttpStatusCode.OK, _list);
            }
            catch (Exception ex)
            {
                Logger.Debug("Error:" + ex.InnerException.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage RequestUserShoppingListProductsUpdatePrices(string userId)
        {
            //List<UserUpdatePricesRequests> _userUpdatePricesRequestsList = Managers.UserUpdatePricesRequestsManager.Get(false);
            List<UserProductsList> _userProductsList = new List<UserProductsList>();

            var _users = db.AspNetUsers.ToList();

            var _userShoppingList = Managers.UserListsManager.GetOfUser(userId, "shoppingList");
            _userProductsList.AddRange(_userShoppingList);

            //call pupetter service
            var _list = _userProductsList.Select(item => new
            {
                item.Id,
                item.ListName,
                item.ProductId,
                item.Quantity,
                item.QuantityWeight,
                item.UserId
            });

            var cli = new WebClient();
            cli.Headers[HttpRequestHeader.ContentType] = "application/json";

            if (_list.Count() != 0)
            {
                try
                {
                    var json = new JavaScriptSerializer().Serialize(_list);
                    string response = cli.UploadString("https://puppeteer-lisie.herokuapp.com/updateProductsPrices", json);
                    //string response = cli.UploadString("http://localhost:3000/updateUsersShoppingListProductsPrices", json);
                    return Request.CreateResponse(HttpStatusCode.OK, _list.ToList());
                }
                catch (Exception ex)
                {
                    Logger.Debug("Error:" + ex.InnerException.Message);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, "No results");
            }

        }

        // PUT: api/UserLists/5
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpGet]
        [HttpPost]
        // DELETE: api/UserLists/5
        public HttpResponseMessage Delete(int id)
        {
            if (Managers.UserListsManager.DeleteOfUser(id))
                return Request.CreateResponse(HttpStatusCode.OK);
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError);

        }

        //[HttpPost]
        //[HttpGet]
        //public HttpResponseMessage SubtractQuantity(int id)
        //{
        //    UserProductsList _updatedUserProductsList = Managers.UserListsManager.SubtractQuantity(id);
        //    if (_updatedUserProductsList != null)
        //    {
        //        UserProductListCompleteModel _UserProductListCompleteModel = new Models.UserProductListCompleteModel
        //        {
        //            Id = _updatedUserProductsList.Id,
        //            ProductId = _updatedUserProductsList.ProductId,
        //            Quantity = _updatedUserProductsList.Quantity.Value,

        //            Barcode = _updatedUserProductsList.Products.Barcode,
        //            Brand = _updatedUserProductsList.Products.Brand,
        //            ItemType = _updatedUserProductsList.ListName,
        //            Name = _updatedUserProductsList.Products.Name,
        //            Weight = _updatedUserProductsList.Products.Weight,
        //            Category = _updatedUserProductsList.Products.CategoryString,
        //            //Price = prod.Price
        //            Price = Math.Round(_updatedUserProductsList.Products.Price.Value * _updatedUserProductsList.Quantity ?? 1, 2)
        //        };
        //        //Get new PriceList1
        //        var userShoppingList = from m in db.StoreProducts where m.ProductId == _UserProductListCompleteModel.ProductId select m;
        //        if (userShoppingList.Count() > 0)
        //        {
        //            foreach (var storeProduct in userShoppingList)
        //            {
        //                if (_UserProductListCompleteModel.PriceList == null) _UserProductListCompleteModel.PriceList = new Dictionary<string, double>();
        //                if (!_UserProductListCompleteModel.PriceList.ContainsKey(storeProduct.StoreId.ToString()))
        //                    _UserProductListCompleteModel.PriceList.Add(storeProduct.StoreId.ToString(), Math.Round(storeProduct.Price.Value * _UserProductListCompleteModel.Quantity, 2));
        //            }
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, _UserProductListCompleteModel);
        //    }
        //    else
        //        return Request.CreateResponse(HttpStatusCode.OK, "");
        //}

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //in use v2
        public HttpResponseMessage SubtractQuantity(int id, string userId = "", bool isProductSimple = false)
        {
            if (userId != "")
                Managers.InteractionsManager.Add(userId, "api/UserLists/SubtractQuantity", new JavaScriptSerializer().Serialize(id));

            if (!isProductSimple)
            {
                UserProductsList _updatedUserProductsList = Managers.UserListsManager.SubtractQuantity(id);
                if (_updatedUserProductsList != null)
                {
                    UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2
                    {
                        Id = _updatedUserProductsList.Id,
                        ProductId = _updatedUserProductsList.ProductId,
                        Quantity = _updatedUserProductsList.Quantity.Value,

                        Barcode = _updatedUserProductsList.Products.Barcode,
                        Brand = _updatedUserProductsList.Products.Brand,
                        ItemType = _updatedUserProductsList.ListName,
                        Name = _updatedUserProductsList.Products.Name,
                        Weight = _updatedUserProductsList.Products.Weight,
                        Category = _updatedUserProductsList.Products.CategoryString,
                        //PriceList = Managers.ProductsManager.GetPricesListOfProduct(_updatedUserProductsList.ProductId, _updatedUserProductsList.Quantity.Value, userId),
                        PriceList = Managers.ProductsManager.GetPricesListOfProduct(_updatedUserProductsList.ProductId, _updatedUserProductsList.Quantity.Value),
                        Price = Math.Round(_updatedUserProductsList.Products.Price.Value * _updatedUserProductsList.Quantity ?? 1, 2)
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, _UserProductListCompleteModel);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            else
            {
                int _newQuantity = Managers.UserListsManager.SubtractQuantityToProductSimple(id);
                if (_newQuantity > -1)
                {
                    UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2
                    {
                        Quantity = _newQuantity,
                        PriceList = new List<StoreProduct>(),
                    };
                    //Get new PriceList
                    return Request.CreateResponse(HttpStatusCode.OK, _UserProductListCompleteModel);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //in use v2
        public HttpResponseMessage SubtractQuantityV2(int id, string userId = "", bool isProductSimple = false)
        {
            if (userId != "")
                Managers.InteractionsManager.Add(userId, "api/UserLists/SubtractQuantityV2", new JavaScriptSerializer().Serialize(id));

            if (!isProductSimple)
            {
                UserProductsList _updatedUserProductsList = Managers.UserListsManager.SubtractQuantity(id);
                if (_updatedUserProductsList != null)
                {
                    UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2
                    {
                        Id = _updatedUserProductsList.Id,
                        ProductId = _updatedUserProductsList.ProductId,
                        Quantity = _updatedUserProductsList.Quantity.Value,

                        Barcode = _updatedUserProductsList.Products.Barcode,
                        Brand = _updatedUserProductsList.Products.Brand,
                        ItemType = _updatedUserProductsList.ListName,
                        Name = _updatedUserProductsList.Products.Name,
                        Weight = _updatedUserProductsList.Products.Weight,
                        Category = _updatedUserProductsList.Products.CategoryString,
                        PriceList = Managers.ProductsManager.GetPricesListOfProduct(_updatedUserProductsList.ProductId, _updatedUserProductsList.Quantity.Value, userId),
                        Price = Math.Round(_updatedUserProductsList.Products.Price.Value * _updatedUserProductsList.Quantity ?? 1, 2)
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, _UserProductListCompleteModel);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            else
            {
                int _newQuantity = Managers.UserListsManager.SubtractQuantityToProductSimple(id);
                if (_newQuantity > -1)
                {
                    UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2
                    {
                        Quantity = _newQuantity,
                        PriceList = new List<StoreProduct>(),
                    };
                    //Get new PriceList
                    return Request.CreateResponse(HttpStatusCode.OK, _UserProductListCompleteModel);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

        }

        //LEGACY - in ue in android v1
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage SubtractQuantityNew(int id)
        {
            UserProductsList _updatedUserProductsList = Managers.UserListsManager.SubtractQuantity(id);
            if (_updatedUserProductsList != null)
            {
                UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2
                {
                    Id = _updatedUserProductsList.Id,
                    ProductId = _updatedUserProductsList.ProductId,
                    Quantity = _updatedUserProductsList.Quantity.Value,

                    Barcode = _updatedUserProductsList.Products.Barcode,
                    Brand = _updatedUserProductsList.Products.Brand,
                    ItemType = _updatedUserProductsList.ListName,
                    Name = _updatedUserProductsList.Products.Name,
                    Weight = _updatedUserProductsList.Products.Weight,
                    Category = _updatedUserProductsList.Products.CategoryString,
                    PriceList = Managers.ProductsManager.GetPricesListOfProduct(_updatedUserProductsList.ProductId, _updatedUserProductsList.Quantity.Value),
                    Price = Math.Round(_updatedUserProductsList.Products.Price.Value * _updatedUserProductsList.Quantity ?? 1, 2)
                };
                return Request.CreateResponse(HttpStatusCode.OK, _UserProductListCompleteModel);
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK, "");
        }

        [HttpGet]
        [HttpPost]
        public HttpResponseMessage SubtractQuantityToProductSimple(int id)
        {
            int _updatedUserProductsList = Managers.UserListsManager.SubtractQuantityToProductSimple(id);
            //if (_updatedUserProductsList != -1)
            //{
            //UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2
            //{
            //    Id = _updatedUserProductsList.Id,
            //    Quantity = _updatedUserProductsList.Quantity,
            //    Name = _updatedUserProductsList.Name,
            //    LastAddedDate = _updatedUserProductsList.UpdateDate
            //};
            return Request.CreateResponse(HttpStatusCode.OK, _updatedUserProductsList);
            //}
            //else
            //    return Request.CreateResponse(HttpStatusCode.NotFound);
        }


        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //Refactored for v2
        public HttpResponseMessage AddQuantity(int id, string userId = "", bool isProductSimple = false)
        {
            if (userId != "")
                Managers.InteractionsManager.Add(userId, "api/UserLists/AddQuantity", new JavaScriptSerializer().Serialize(id));

            if (!isProductSimple)
            {
                UserProductsList _updatedUserProductsList = Managers.UserListsManager.AddQuantity(id);
                if (_updatedUserProductsList != null)
                {
                    //TODO - refactor in a single fucntion GetProductListComplete(userProduct)?
                    UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2
                    {
                        Id = _updatedUserProductsList.Id,
                        ProductId = _updatedUserProductsList.ProductId,
                        Quantity = _updatedUserProductsList.Quantity.Value,
                        Barcode = _updatedUserProductsList.Products.Barcode,
                        Brand = _updatedUserProductsList.Products.Brand,
                        ItemType = _updatedUserProductsList.ListName,
                        Name = _updatedUserProductsList.Products.Name,
                        Weight = _updatedUserProductsList.Products.Weight,
                        Category = _updatedUserProductsList.Products.CategoryString,
                        //PriceList = Managers.ProductsManager.GetPricesListOfProduct(_updatedUserProductsList.ProductId, _updatedUserProductsList.Quantity.Value, userId),
                        PriceList = Managers.ProductsManager.GetPricesListOfProduct(_updatedUserProductsList.ProductId, _updatedUserProductsList.Quantity.Value),
                        Price = Math.Round(_updatedUserProductsList.Products.Price.Value * _updatedUserProductsList.Quantity ?? 1, 2)
                    };
                    //Get new PriceList
                    return Request.CreateResponse(HttpStatusCode.OK, _UserProductListCompleteModel);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            else
            {
                int _newQuantity = Managers.UserListsManager.AddQuantityToProductSimple(id);
                if (_newQuantity > -1)
                {
                    UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2
                    {
                        Quantity = _newQuantity,
                        PriceList = new List<StoreProduct>(),
                    };
                    //Get new PriceList
                    return Request.CreateResponse(HttpStatusCode.OK, _UserProductListCompleteModel);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //Refactored for v2
        public HttpResponseMessage AddQuantityV2(int id, string userId = "", bool isProductSimple = false)
        {
            if (userId != "")
                Managers.InteractionsManager.Add(userId, "api/UserLists/AddQuantityV2", new JavaScriptSerializer().Serialize(id));

            if (!isProductSimple)
            {
                UserProductsList _updatedUserProductsList = Managers.UserListsManager.AddQuantity(id);
                if (_updatedUserProductsList != null)
                {
                    //TODO - refactor in a single fucntion GetProductListComplete(userProduct)?
                    UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2
                    {
                        Id = _updatedUserProductsList.Id,
                        ProductId = _updatedUserProductsList.ProductId,
                        Quantity = _updatedUserProductsList.Quantity.Value,
                        Barcode = _updatedUserProductsList.Products.Barcode,
                        Brand = _updatedUserProductsList.Products.Brand,
                        ItemType = _updatedUserProductsList.ListName,
                        Name = _updatedUserProductsList.Products.Name,
                        Weight = _updatedUserProductsList.Products.Weight,
                        Category = _updatedUserProductsList.Products.CategoryString,
                        PriceList = Managers.ProductsManager.GetPricesListOfProduct(_updatedUserProductsList.ProductId, _updatedUserProductsList.Quantity.Value, userId),
                        Price = Math.Round(_updatedUserProductsList.Products.Price.Value * _updatedUserProductsList.Quantity ?? 1, 2)
                    };
                    //Get new PriceList
                    return Request.CreateResponse(HttpStatusCode.OK, _UserProductListCompleteModel);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            else
            {
                int _newQuantity = Managers.UserListsManager.AddQuantityToProductSimple(id);
                if (_newQuantity > -1)
                {
                    UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2
                    {
                        Quantity = _newQuantity,
                        PriceList = new List<StoreProduct>(),
                    };
                    //Get new PriceList
                    return Request.CreateResponse(HttpStatusCode.OK, _UserProductListCompleteModel);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        //LEGACY , in use in android v1
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage AddQuantityNew(int id)
        {
            UserProductsList _updatedUserProductsList = Managers.UserListsManager.AddQuantity(id);
            if (_updatedUserProductsList != null)
            {
                //TODO - refactor in a single fucntion GetProductListComplete(userProduct)?
                UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2
                {
                    Id = _updatedUserProductsList.Id,
                    ProductId = _updatedUserProductsList.ProductId,
                    Quantity = _updatedUserProductsList.Quantity.Value,
                    Barcode = _updatedUserProductsList.Products.Barcode,
                    Brand = _updatedUserProductsList.Products.Brand,
                    ItemType = _updatedUserProductsList.ListName,
                    Name = _updatedUserProductsList.Products.Name,
                    Weight = _updatedUserProductsList.Products.Weight,
                    Category = _updatedUserProductsList.Products.CategoryString,
                    PriceList = Managers.ProductsManager.GetPricesListOfProduct(_updatedUserProductsList.ProductId, _updatedUserProductsList.Quantity.Value),
                    Price = Math.Round(_updatedUserProductsList.Products.Price.Value * _updatedUserProductsList.Quantity ?? 1, 2)
                };
                //Get new PriceList
                return Request.CreateResponse(HttpStatusCode.OK, _UserProductListCompleteModel);
            }
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError);

        }

        [HttpGet]
        [HttpPost]
        public HttpResponseMessage AddQuantityToProductSimple(int id)
        {
            int _updatedQuantity = Managers.UserListsManager.AddQuantityToProductSimple(id);
            return Request.CreateResponse(HttpStatusCode.OK, _updatedQuantity);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage Remove(int id, string userId)
        {
            if (userId != "")
                Managers.InteractionsManager.Add(userId, "api/UserLists/Remove", new JavaScriptSerializer().Serialize(id));

            int _result = Managers.UserListsManager.Remove(id);
            if (_result > 0)
                Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCart(-1, userId);
            return Request.CreateResponse(HttpStatusCode.OK, _result);

        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage RemoveProductSimple(int id, string userId)
        {
            if (userId != "")
                Managers.InteractionsManager.Add(userId, "api/UserLists/RemoveProductSimple", new JavaScriptSerializer().Serialize(id));

            int _result = Managers.UserListsManager.RemoveProductSimple(id);
            //if (_result > 0)
            //    Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCart(-1, userId);
            return Request.CreateResponse(HttpStatusCode.OK, _result);

        }

        // POST: api/UserLists/CheckoutProducts
        //public HttpResponseMessage CheckoutProducts([FromBody]UserProductListsCheckoutPostModel data)
        //{
        //    Managers.InteractionsManager.Add(data.UserId, "api/UserLists/CheckoutProducts", new JavaScriptSerializer().Serialize(data.ProductsIds));
        //    if (data.ProductsIds != null && data.ProductsIds.Count > 0)
        //    {
        //        int _checkoutCounter = Managers.UserListsManager.CheckoutProducts(data.ProductsIds, data.UserId, data.AddToInventory);
        //        return Request.CreateResponse(HttpStatusCode.Created, _checkoutCounter);
        //    }
        //    return Request.CreateResponse(HttpStatusCode.BadRequest);
        //}

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage Checkout(int id, string userId)
        {
            Managers.InteractionsManager.Add(userId, "api/UserLists/Checkout", new JavaScriptSerializer().Serialize(userId));
            if (id > 0 && userId != string.Empty)
            {
                var _result = Managers.UserListsManager.CheckoutProduct(id, userId);
                return Request.CreateResponse(HttpStatusCode.OK, _result);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage CheckoutV2(int id, string userId, bool emulate = false)
        {
            Managers.InteractionsManager.Add(userId, "api/UserLists/CheckoutV2", new JavaScriptSerializer().Serialize(userId));
            if (id > 0 && userId != string.Empty)
            {
                var _result = Managers.UserListsManager.CheckoutProductV2(id, userId, emulate);
                return Request.CreateResponse(HttpStatusCode.OK, _result);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage CheckoutV3(int id, string userId, bool emulate = false)
        {
            Managers.InteractionsManager.Add(userId, "api/UserLists/CheckoutV3", new JavaScriptSerializer().Serialize(userId));
            if (id > 0 && userId != string.Empty)
            {
                var _result = Managers.UserListsManager.CheckoutProductV3(id, userId, emulate);
                return Request.CreateResponse(HttpStatusCode.OK, _result);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage CheckoutSimple(int id, string userId)
        {
            Managers.InteractionsManager.Add(userId, "api/UserLists/CheckoutSimple", new JavaScriptSerializer().Serialize(userId));
            if (id > 0 && userId != string.Empty)
            {
                var _result = Managers.UserListsManager.CheckoutProductSimple(id, userId);
                return Request.CreateResponse(HttpStatusCode.OK, _result);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage CheckoutSimpleV2(int id, string userId, bool emulate = false)
        {
            Managers.InteractionsManager.Add(userId, "api/UserLists/CheckoutSimpleV2", new JavaScriptSerializer().Serialize(userId));
            if (id > 0 && userId != string.Empty)
            {
                var _result = Managers.UserListsManager.CheckoutProductSimpleV3(id, userId, emulate);
                return Request.CreateResponse(HttpStatusCode.OK, _result);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage CheckoutSimpleV3(int id, string userId, bool emulate = false)
        {
            Managers.InteractionsManager.Add(userId, "api/UserLists/CheckoutSimpleV3", new JavaScriptSerializer().Serialize(userId));
            if (id > 0 && userId != string.Empty)
            {
                var _result = Managers.UserListsManager.CheckoutProductSimpleV3(id, userId, emulate);
                return Request.CreateResponse(HttpStatusCode.OK, _result);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage CheckoutProducts([FromBody] UserProductListsCheckoutPostModel2 data)
        {
            Managers.InteractionsManager.Add(data.UserId, "api/UserLists/CheckoutProducts", new JavaScriptSerializer().Serialize(data.UserProductIds));
            if (data.UserProductIds != null && data.UserProductIds.Count > 0)
            {
                int _checkoutCounter = Managers.UserListsManager.CheckoutProducts(data.UserProductIds, data.UserId, data.AddToInventory);
                return Request.CreateResponse(HttpStatusCode.Created, _checkoutCounter);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //LEGACY
        public HttpResponseMessage CheckoutProductsNew([FromBody] UserProductListsCheckoutPostModel2 data)
        {
            Managers.InteractionsManager.Add(data.UserId, "api/UserLists/CheckoutProducts", new JavaScriptSerializer().Serialize(data.UserProductIds));
            if (data.UserProductIds != null && data.UserProductIds.Count > 0)
            {
                int _checkoutCounter = Managers.UserListsManager.CheckoutProducts(data.UserProductIds, data.UserId, data.AddToInventory);
                return Request.CreateResponse(HttpStatusCode.Created, _checkoutCounter);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // POST: api/UserLists/ProductsConsumedFromInventory
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage ProductsConsumedFromInventory([FromBody] UserProductListsCheckoutPostModel data)
        {
            Managers.InteractionsManager.Add(data.UserId, "api/UserLists/ProductsConsumedFromInventory", new JavaScriptSerializer().Serialize(data.ProductsIds));
            if (data.ProductsIds != null && data.ProductsIds.Count > 0)
            {
                int _checkoutCounter = Managers.UserListsManager.ProductsConsumedFromInventory(data.ProductsIds, data.UserId, data.AddToInventory);
                return Request.CreateResponse(HttpStatusCode.Created, _checkoutCounter);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetBuyStoresPrices(string userId, string list)
        {
            List<UserProductListStorePricesModel> _UserProductListStorePricesModelList = Managers.UserListsManager.GetBuyStoresPrices(userId, "in");
            if (_UserProductListStorePricesModelList != null)
                return Request.CreateResponse(HttpStatusCode.OK, _UserProductListStorePricesModelList);
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetBuyStores(string userId, string list)
        {
            List<UserProductListStorePricesModel> _UserProductListStorePricesModelList = Managers.UserListsManager.GetBuyStores(userId, list);
            if (_UserProductListStorePricesModelList != null)
                return Request.CreateResponse(HttpStatusCode.OK, _UserProductListStorePricesModelList);
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetBuyStoresV2(string userId, string list)
        {
            List<UserProductListStorePricesModel2> _UserProductListStorePricesModelList = Managers.UserListsManager.GetBuyStoresV2(userId, list);
            if (_UserProductListStorePricesModelList != null)
                return Request.CreateResponse(HttpStatusCode.OK, _UserProductListStorePricesModelList);
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetUserProductListCompleteModelOfIdsList([FromBody] UserProductIdsListPostModel data)
        {
            Managers.InteractionsManager.Add(data.UserId, "api/UserLists/CheckoutProducts", new JavaScriptSerializer().Serialize(data.UserProductIds));
            if (data.UserProductIds != null && data.UserProductIds.Count > 0)
            {
                var _userProducts = Managers.UserListsManager.GetUserProductListCompleteModelOfIdsList(data.UserProductIds, data.UserId);
                return Request.CreateResponse(HttpStatusCode.Created, _userProducts);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage AddSpokenProduct(string userId, string productSpokenName, string listName = "shoppingList") //isFromApp is for legacy purposes
        {
            Managers.InteractionsManager.Add(userId, "api/UserLists/AddSpokenProduct", productSpokenName);
            Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");
            //int _userProductSimpleId = -1;
            try
            {
                ProductSimpleItem _ProductSimpleItem = new ProductSimpleItem();
                _ProductSimpleItem.Name = productSpokenName;
                _ProductSimpleItem.UserId = userId;
                _ProductSimpleItem.ImageUrl = string.Empty;
                _ProductSimpleItem.List = listName;


                var _userProductSimple = Managers.UserListsManager.AddProductSimpleToUserList(_ProductSimpleItem);

                return Request.CreateResponse(HttpStatusCode.Created, _userProductSimple);
                //if (_userProductSimpleId == -1)
                //    return Json(-1, JsonRequestBehavior.AllowGet);
                //else
                //    return Json(_userProductSimpleId, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.InnerException.Message);
                return Request.CreateResponse(HttpStatusCode.Created, "");
            }
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage AddProductSimple(string userId, string productName, string listName = "shoppingList")
        {
            Managers.InteractionsManager.Add(userId, "api/UserLists/AddProductSimple", productName);
            Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");
            //int _userProductSimpleId = -1;
            try
            {
                ProductSimpleItem _ProductSimpleItem = new ProductSimpleItem();
                _ProductSimpleItem.Name = productName;
                _ProductSimpleItem.UserId = userId;
                _ProductSimpleItem.ImageUrl = string.Empty;
                _ProductSimpleItem.List = listName;


                var _userProductSimple = Managers.UserListsManager.AddProductSimpleToUserList(_ProductSimpleItem);

                return Request.CreateResponse(HttpStatusCode.Created, _userProductSimple);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.InnerException.Message);
                return Request.CreateResponse(HttpStatusCode.Created, "");
            }
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> AddProductByBarcode(string barcode, string userId, string list)
        {
            UserProductListCompleteModel2 _UserProductListCompleteModel2;

            if (barcode != string.Empty)
                _UserProductListCompleteModel2 = await Managers.UserListsManager.AddProductByBarcode(barcode, userId, list);
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            if (_UserProductListCompleteModel2 == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            return Request.CreateResponse(HttpStatusCode.OK, _UserProductListCompleteModel2);
        }


        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> AddProductByBarcodeV2(string barcode, string userId, string list)
        {
            UserProductListCompleteModel2 _UserProductListCompleteModel2;

            if (barcode != string.Empty)
                _UserProductListCompleteModel2 = await Managers.UserListsManager.AddProductByBarcodeV2(barcode, userId, list);
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            if (_UserProductListCompleteModel2 == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            return Request.CreateResponse(HttpStatusCode.OK, _UserProductListCompleteModel2);
        }


        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage AddProduct(string userId, int productId, int quantity = 1, string listName = "shoppingList")
        {
            Managers.InteractionsManager.Add(userId, "api/UserLists/AddProduct", productId.ToString());
            var _product = Managers.ProductsManager.GetById(productId);
            int _userProductListId = Managers.UserListsManager.AddProductToList(productId, _product.Name, listName, quantity, null, true, userId);
            var _userProductCompleteModel = Managers.UserListsManager.GetCompleteModel(_userProductListId, userId);
            return Request.CreateResponse(HttpStatusCode.Created, _userProductCompleteModel);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage AddProductV2(string userId, int productId, int quantity = 1, string listName = "shoppingList")
        {
            Managers.InteractionsManager.Add(userId, "api/UserLists/AddProduct", productId.ToString());
            var _product = Managers.ProductsManager.GetById(productId);
            int _userProductListId = Managers.UserListsManager.AddProductToList(productId, _product.Name, listName, quantity, null, true, userId);
            var _userProductCompleteModel = Managers.UserListsManager.GetCompleteModelV2(_userProductListId, userId);
            return Request.CreateResponse(HttpStatusCode.Created, _userProductCompleteModel);
        }

        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> UploadProductSimpleImage([FromBody] UploadImageModel model)
        {
            try
            {
                Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");
                Logger.Debug("UploadProductSimpleImage running", "upload.txt");
                Logger.Debug("Request Model", "upload.txt");
                Logger.Debug(new JavaScriptSerializer().Serialize(model), "upload.txt");
                //var teste = Request.Content.Headers;
                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                model.Image = regex.Replace(model.Image, string.Empty);
                var _bytes = ManageImage.Base64ToBytes(model.Image);
                Logger.Debug("converted to bytes lenght" + _bytes.Length.ToString(), "upload.txt");
                var _result = Managers.UserListsManager.SaveProductSimpleImage(model.Id, _bytes);
                Logger.Debug(new JavaScriptSerializer().Serialize(_result), "upload.txt");

                return Request.CreateResponse(HttpStatusCode.OK, _result);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, "upload.txt");
                Logger.Debug(ex.InnerException.Message, "upload.txt");
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }
        }

        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage UpdateProductSimpleName([FromBody] UpdateProductSimpleNameModel model)
        {
            if (model == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                var _result = Managers.UserListsManager.UpdateProductSimpleName(model.Id, model.Name);
                return Request.CreateResponse(HttpStatusCode.OK, _result);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, "erros.txt");
                Logger.Debug(ex.InnerException.Message, "erros.txt");
                return Request.CreateResponse(HttpStatusCode.OK, false);
            }
        }

        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage AddProductSimpleV2([FromBody] AddProductSimpleV2Model model)
        {
            Managers.InteractionsManager.Add(model.UserId, "api/UserLists/AddProductSimpleV2", model.Name);
            Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");
            //int _userProductSimpleId = -1;
            try
            {
                ProductSimpleItemV2 _ProductSimpleItem = new ProductSimpleItemV2();
                _ProductSimpleItem.Name = model.Name;
                _ProductSimpleItem.UserId = model.UserId;
                _ProductSimpleItem.ImageBase64 = model.ImageBase64;
                _ProductSimpleItem.List = model.List;


                var _userProductSimple = Managers.UserListsManager.AddProductSimpleToUserListV2(_ProductSimpleItem);

                return Request.CreateResponse(HttpStatusCode.Created, _userProductSimple);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.InnerException.Message);
                return Request.CreateResponse(HttpStatusCode.Created, "");
            }
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage ChangeQuantity(string userId, int userProductListId, int newQuantity = -1, double newQuantityWeight = -1)
        {
            if (newQuantity == -1 && newQuantityWeight == -1)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            //Managers.InteractionsManager.Add(userId, "api/UserLists/CheckoutV2", new JavaScriptSerializer().Serialize(userId));
            var _response = Managers.UserListsManager.ChangeQuantity(userId, userProductListId, newQuantity, newQuantityWeight);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage UpdateUserProductsWithAI(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            //Managers.InteractionsManager.Add(userId, "api/UserLists/CheckoutV2", new JavaScriptSerializer().Serialize(userId));
            var _response = Managers.UserListsManager.UpdateUserProductsWithAI(userId);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage UpdateUserProducts(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            //Managers.InteractionsManager.Add(userId, "api/UserLists/CheckoutV2", new JavaScriptSerializer().Serialize(userId));
            var _response = Managers.UserListsManager.UpdateUserProductsWithAI(userId);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }
    }
}
