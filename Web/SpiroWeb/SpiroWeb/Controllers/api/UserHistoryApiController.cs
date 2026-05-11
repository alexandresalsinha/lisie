using ClassLibrary1;
using SpiroWeb.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SpiroWeb.Controllers
{
    public class UserHistoryApiController : ApiController
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        // GET: api/UserLists
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/UserLists/5
        [HttpGet]
        public HttpResponseMessage GetOfMonthYear(string userId, int month, int year)
        {
            List<UserProductsListHistoryModel> _UserProductsListHistoryList = Managers.UserHistoryManager.GetOfMonthYear(userId, month, year);
            if (_UserProductsListHistoryList != null)
                return Request.CreateResponse(HttpStatusCode.OK, _UserProductsListHistoryList);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        public HttpResponseMessage GetTotalsOfProduct(string userId, int productId, int storeId, string action)
        {
            UserProductHistoryModel _UserProductHistoryModel = Managers.UserHistoryManager.GetTotalsOfProduct(userId, productId, storeId, action);
            if (_UserProductHistoryModel != null)
                return Request.CreateResponse(HttpStatusCode.OK, _UserProductHistoryModel);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [HttpGet]
        [HttpPost]
        public HttpResponseMessage Delete(int id, string userId)
        {
            if (Managers.UserHistoryManager.DeleteOfUser(id, userId))
                return Request.CreateResponse(HttpStatusCode.OK);
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError);

        }

        // POST: api/UserLists
        //public HttpResponseMessage Post([FromBody]Models.UserProductListsPostModel data)
        //{
        //    Managers.InteractionsManager.Add(data.UserId, "api/UserLists/Post", new JavaScriptSerializer().Serialize(data));
        //    if (data != null)
        //    {
        //        //TODO - send response with a complex object with all the userProductListId for each list - UserProductListsRspModel
        //        int _lastUserProductListId = -1;
        //        foreach (string list in data.Lists)
        //        {
        //            _lastUserProductListId = Managers.UserListsManager.AddProductToList(data.ProductId, data.ProductName, list, data.Quantity, data.QuantityWeight, true, data.UserId);
        //        }
        //        return Request.CreateResponse(HttpStatusCode.Created, _lastUserProductListId);

        //    }
        //    return Request.CreateResponse(HttpStatusCode.BadRequest);
        //}

    }
}
