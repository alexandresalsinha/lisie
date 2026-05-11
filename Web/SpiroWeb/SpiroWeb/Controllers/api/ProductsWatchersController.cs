using ClassLibrary1;
using SpiroWeb.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SpiroWeb.Controllers
{
    public class ProductsWatchersController : ApiController
    {
        // GET: api/Notes/Get?userId=
        public HttpResponseMessage Get(string userId)
        {
            Managers.InteractionsManager.Add(userId, "api/ProductsWatchersController/Get", userId);

            if (userId != string.Empty)
            {
                List<Models.UserProductListCompleteModel2> _ProductsWatchers = Managers.ProductsWatchersManager.GetAll(userId);
                if (_ProductsWatchers != null)
                    return Request.CreateResponse(HttpStatusCode.OK, _ProductsWatchers);
                return Request.CreateResponse(HttpStatusCode.NotFound);

            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // POST: api/Notes/Post
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage Post([FromBody] ProductWatcherPostModel model)
        {

            if (model != null && !string.IsNullOrEmpty(model.UserId))
            {
                Managers.InteractionsManager.Add(model.UserId, "api/ProductsWatchersController/Post", "");

                ProductsWatchers _ProductsWatchers = Managers.ProductsWatchersManager.Create(model.UserId, model.ProductId);
                if (_ProductsWatchers != null)
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        _ProductsWatchers.Id,
                        _ProductsWatchers.ProductId,
                        _ProductsWatchers.UserId
                    });
                return Request.CreateResponse(HttpStatusCode.NotFound);

            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        public HttpResponseMessage Delete(string userId, int id)
        {
            Managers.InteractionsManager.Add(userId, "api/ProductsWatchersController/Delete", userId);

            if (userId != string.Empty)
            {
                bool _sucess = Managers.ProductsWatchersManager.Delete(userId, id);
                return Request.CreateResponse(HttpStatusCode.OK, _sucess);

            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
