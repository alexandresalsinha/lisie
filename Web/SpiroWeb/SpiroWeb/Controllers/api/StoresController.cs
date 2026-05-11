using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SpiroWeb.Controllers
{
    public class StoresController : ApiController
    {
        //private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetAll()
        {
            List<LisieStores.Extensibility.Market> _markets = Managers.StoresManager.GetAllByExtensibility();
            //var _intermarche = _markets.Find(c => c.StoreId == 4);
            //if (_intermarche != null)
            //    _intermarche.StoreUrl = "https://puppeteer-lisie.herokuapp.com/getIntermarcheProductMetadata";
            if (_markets != null)
                return Request.CreateResponse(HttpStatusCode.OK, _markets);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }
    }
}
