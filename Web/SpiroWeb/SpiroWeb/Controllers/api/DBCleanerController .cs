using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SpiroWeb.Controllers.api
{
    //TODO - refactor to here
    public class DBCleanerController : ApiController
    {

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage CleanDatabase()
        {
            int _productsDeleted = 0;
            using (ClassLibrary1.SpiroStockManagementEntities db = new ClassLibrary1.SpiroStockManagementEntities())
            {
                //Delete Products wihout barcode, or equal to 0
                var _productsWithoutBarcode = db.Products.Where(c => c.Barcode == "" || c.Barcode == "0").ToList();
                foreach (var _productWithoutBarcode in _productsWithoutBarcode)
                {
                    if (Managers.ProductsManager.DeleteSafely(_productWithoutBarcode.Id))
                        _productsDeleted++;
                }

                //Remove Products witout StoreProducts
                var userProductsIds = db.Products.Select(c => new { Id = c.Id, Count = c.StoreProducts.Count() }).ToList();
                var _withoutStoreProducts = userProductsIds.Where(c => c.Count == 0).ToList();
                foreach (var _withoutStoreProduct in _withoutStoreProducts)
                {
                    if (Managers.ProductsManager.DeleteSafely(_withoutStoreProduct.Id))
                        _productsDeleted++;

                }
            }
            return Request.CreateResponse(HttpStatusCode.Created, _productsDeleted);

        }
    }
}
