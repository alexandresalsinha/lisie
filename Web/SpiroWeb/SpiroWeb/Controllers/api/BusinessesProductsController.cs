using SpiroWeb.Managers;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
//using System.Web.Script.Serialization;

namespace SpiroWeb.Controllers.Api
{
    /// <summary>
    /// TO GO IN FUTURE TO OBSOLETE- Go to api/ProductsController
    /// </summary>


    public class BusinessesProductsController : ApiController
    {
        // GET: api/ProductsApi/5
        //TODO - add userId for interactions
        //[HttpGet]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        //public async Task<HttpResponseMessage> Get(int id = -1, string barcode = "")
        //{
        //    Products product;

        //    if (id != -1)
        //        product = Managers.BusinessProductsManager.GetById(id);
        //    else if (barcode != string.Empty)
        //        product = Managers.BusinessProductsManager.GetByBarcode(barcode);
        //    else
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);

        //    if (product == null)
        //        return Request.CreateResponse(HttpStatusCode.NotFound);

        //    //First update prices before returning product - TEMPORARY , to do with push notifications also
        //    //await Managers.ProductsManager.UpdatePricesNew(product.Id);
        //    //if (id != -1)
        //    //    product = Managers.ProductsManager.GetById(id);
        //    //else if (barcode != string.Empty)
        //    //    product = Managers.ProductsManager.GetByBarcode(barcode);
        //    //else
        //    //    return Request.CreateResponse(HttpStatusCode.BadRequest);

        //    var _productDTO = new BusinessProducts()
        //    {
        //        Id = product.Id,
        //        Barcode = product.Barcode,
        //        //CreateDate = product.InsertDate,
        //        Name = product.Name,
        //        //Picture = Helpers.Settings.WebURL + "/handlers/getproductImage.ashx?productId=" + product.Id,
        //        //Price = product.Price,
        //        //VariableWeightPrice = product.VariableWeightPrice,
        //        //StoreProducts = Managers.ProductsManager.GetStoreProductsCopy(product.StoreProducts),
        //        //Weight = product.Weight,
        //        //AddedByUserId = product.AddedByUserId,
        //        //CreatedByUserId = product.CreatedByUserId
        //    };

        //    return Request.CreateResponse(HttpStatusCode.OK, _productDTO);
        //}

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetProductByBarcode(int businessId, string barcode)
        {
            var _response = Managers.BusinessProductsManager.GetProductByBarcode(businessId, barcode);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetLists(int businessId)
        {
            var _response = Managers.BusinessProductsManager.GetBusinessLists(businessId);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetAll(int page, string query, int businessId)
        {
            var _response = Managers.BusinessProductsManager.GetAll(page, query, businessId);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        // POST: api/Products/PostV3
        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> CreateProduct([FromBody] BusinessProductListPostModel product)
        {
            //Managers.InteractionsManager.Add(product.UserId, "api/ProductsApi/Post", new JavaScriptSerializer().Serialize(product));
            var _response = Managers.BusinessProductsManager.CreateProduct(product);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> EditProduct([FromBody] BusinessProductListPostModel product)
        {
            //Managers.InteractionsManager.Add(product.UserId, "api/ProductsApi/Post", new JavaScriptSerializer().Serialize(product));
            var _response = Managers.BusinessProductsManager.EditProduct(product);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }


        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage AddProductByBarcode(string barcode, int businessId, string list, int quantity = 1)
        {
            //Managers.InteractionsManager.Add(userId, "api/UserLists/CheckoutV2", new JavaScriptSerializer().Serialize(userId));
            var _response = Managers.BusinessProductsManager.AddProductByBarcode(barcode, businessId, list, quantity);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }


        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage AddProductByBarcodeHabitos(string barcode, int businessId, string list, int quantity = 1)
        {
            //Managers.InteractionsManager.Add(userId, "api/UserLists/CheckoutV2", new JavaScriptSerializer().Serialize(userId));
            var _response = Managers.BusinessProductsManager.AddProductByBarcodeHabitos(barcode, businessId, list, quantity);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }


        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage Checkout(int businessListProductId, int businessId, bool emulate = false)
        {
            //Managers.InteractionsManager.Add(userId, "api/UserLists/CheckoutV2", new JavaScriptSerializer().Serialize(userId));
            var _response = Managers.BusinessProductsManager.CheckoutProduct(businessListProductId, businessId, emulate);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        // DELETE: api/UserLists/5
        public HttpResponseMessage Delete(int businessListProductId, int businessId)
        {
            var _response = Managers.BusinessProductsManager.Delete(businessListProductId, businessId);
            return Request.CreateResponse(HttpStatusCode.Created, _response);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage AddProductToList(int businessId, int productId, int quantity = 1, string listName = "shoppingList")
        {
            //Managers.InteractionsManager.Add(userId, "api/UserLists/AddProduct", productId.ToString());
            var _response = Managers.BusinessProductsManager.AddToList(businessId, productId, listName, quantity);
            return Request.CreateResponse(HttpStatusCode.Created, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //Refactored for v2
        public HttpResponseMessage AddQuantity(int businessListProductId, int businessId)
        {
            var _response = Managers.BusinessProductsManager.AddQuantity(businessListProductId, businessId);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //in use v2
        public HttpResponseMessage SubtractQuantity(int businessListProductId, int businessId)
        {
            var _response = Managers.BusinessProductsManager.SubtractQuantity(businessListProductId, businessId);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //in use v2
        public HttpResponseMessage LisieHomeAddProduct(int businessId, string barcode)
        {
            var _response = Managers.BusinessProductsManager.LisieHomeAddProductV2(businessId, barcode);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //in use v2
        public HttpResponseMessage LisieHomeAddProductV2(int businessId, string barcode)
        {
            var _response = Managers.BusinessProductsManager.LisieHomeAddProductV2(businessId, barcode);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //in use v2
        public HttpResponseMessage LisieHomeSetMode(int businessId, string mode)
        {
            var _response = Managers.BusinessProductsManager.LisieHomeSetMode(businessId, mode);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //in use v2
        public HttpResponseMessage GetLisieHomeState(int businessId)
        {
            var _response = Managers.BusinessProductsManager.GetLisieHomeState(businessId);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetHistory(int businessId, string list, string startDate, string endDate)
        {
            //Managers.InteractionsManager.Add(userId, "api/UserLists/CheckoutV2", new JavaScriptSerializer().Serialize(userId));
            var _response = Managers.BusinessProductsManager.GetHistory(businessId, list, startDate, endDate);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage DeleteHistoryItem(int id, int businessId)
        {
            //Managers.InteractionsManager.Add(userId, "api/UserLists/CheckoutV2", new JavaScriptSerializer().Serialize(userId));
            var _response = Managers.BusinessProductsManager.DeleteHistoryItem(id, businessId);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage DeleteProduct(int businessId, int productId)
        {
            //Managers.InteractionsManager.Add(userId, "api/UserLists/CheckoutV2", new JavaScriptSerializer().Serialize(userId));
            var _response = Managers.BusinessProductsManager.DeleteProduct(businessId, productId);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }


        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage ChangeQuantity(int businessId, int businessListProductId, int newQuantity)
        {
            //Managers.InteractionsManager.Add(userId, "api/UserLists/CheckoutV2", new JavaScriptSerializer().Serialize(userId));
            var _response = Managers.BusinessProductsManager.ChangeQuantity(businessListProductId, businessId, newQuantity);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage AddBusinessDevice([FromBody] AddBusinessDeviceModel model)
        {
            var _response = Managers.BusinessProductsManager.AddBusinessDevice(model.BusinessId, model.DeviceId, model.DeviceToken, model.OperativeSystem, model.ModelName, model.LisieVersion);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //in use v2
        public HttpResponseMessage AddLisieHomeProductWithQueue(int businessId, int productId, int quantity)
        {
            var _response = Managers.BusinessProductsManager.AddLisieHomeProductWithQueue(businessId, productId, quantity);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage GetProductsCategories(int businessId)
        {
            var _response = Managers.BusinessProductsManager.GetProductsCategories(businessId);
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }
    }

    public class AddBusinessDeviceModel
    {
        public int BusinessId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceToken { get; set; }
        public string OperativeSystem { get; set; }
        public string ModelName { get; set; }
        public string LisieVersion { get; set; }
    }

}
