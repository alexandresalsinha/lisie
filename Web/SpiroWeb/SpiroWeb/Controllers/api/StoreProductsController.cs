using SpiroWeb.Helpers;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SpiroWeb.Controllers.Api
{
    /// <summary>
    /// TO GO IN FUTURE TO OBSOLETE- Go to api/ProductsController
    /// </summary>


    public class StoreProductsController : ApiController
    {
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> UpdateMetadata(int storeProductId)
        {
            if (storeProductId != -1)
            {
                try
                {
                    var _newMetadata = await Managers.StoreProductsManager.UpdateMetadata(storeProductId);
                    if (_newMetadata != null)
                        return Request.CreateResponse(HttpStatusCode.OK, _newMetadata);
                    return Request.CreateResponse(HttpStatusCode.NotFound, _newMetadata);
                }
                catch (Exception ex)
                {
                    Logger.Debug("Error:" + ex.InnerException.Message);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);

        }

    }
}
