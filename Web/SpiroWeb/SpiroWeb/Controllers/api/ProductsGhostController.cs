using ClassLibrary1;
using LisieStores.Extensibility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpiroWeb.Helpers;
using SpiroWeb.Markets;
using SpiroWeb.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SpiroWeb.Controllers
{
    public class ProductsGhostController : ApiController
    {
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> GoGetter()
        {
            ProductsGhostFetcher _ProductsGhostFetcher = new ProductsGhostFetcher();
            bool _success = await _ProductsGhostFetcher.GoGetter();
            return Request.CreateResponse(HttpStatusCode.OK, _success);
        }

        //[HttpGet]
        //[HttpPost]
        //public async Task<HttpResponseMessage> IntermarcheProductsIdExists(string[] _productIds)
        //{
        //    //ProductsGhostFetcher _ProductsGhostFetcher = new ProductsGhostFetcher();
        //    //bool _success = await _ProductsGhostFetcher.GoGetter();

        //    using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
        //    {
        //        var _productsExists = db.StoreProductsNew.Where(c => _productIds.Contains(c.OnlineProductId)).Select(c => c.OnlineProductId).ToList();
        //        List<string> _productsThatDontExists = new List<string>();
        //        foreach (var _productId in _productIds)
        //        {
        //            if (!_productsExists.Contains(_productId))
        //            {
        //                _productsThatDontExists.Add(_productId);
        //            }
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, _productsThatDontExists);
        //    }
        //    //return Request.CreateResponse(HttpStatusCode.OK, _success);

        //}

        //[HttpGet]
        //[HttpPost]
        //public async Task<HttpResponseMessage> UpdateProductsWithNewStores()
        //{
        //    //ProductsGhostFetcher _ProductsGhostFetcher = new ProductsGhostFetcher();
        //    //bool _success = await _ProductsGhostFetcher.GoGetter();

        //    try
        //    {
        //        using (SpiroStockManagementEntities dbLocal = new SpiroStockManagementEntities(ConnectionHelper.GetCreateConnectionString()))
        //        {
        //            Console.Write("Starting");
        //            var _lastProductsNew = dbLocal.ProductsNew.Count();
        //            Console.WriteLine(_lastProductsNew);

        //            using (SpiroStockManagementEntities dbProd = new SpiroStockManagementEntities())
        //            {
        //                var _lastProductsOld = dbProd.ProductsNew.Count();
        //                Console.WriteLine(_lastProductsOld);

        //                var _products = dbLocal.ProductsNew; //Get all new products from local
        //                var _productsThatAlreadyExistWithBarcode = 0;
        //                foreach (var _product in _products)
        //                {
        //                    //if only store is pingo doce, continue
        //                    var _storesCount = dbLocal.StoreProductsNew.Where(c => c.ProductId == _product.Id).Count();
        //                    //var _storePIngoDoceExists = dbLocal.StoreProductsNew.Where(c => c.ProductId == _product.Id && c.StoreId == 3).Count();
        //                    if (_storesCount == 2)
        //                    {
        //                        string stop = "";
        //                    }

        //                    //Check if product with same barcode exists in PROD
        //                    var _productOldFound = (!string.IsNullOrEmpty(_product.Barcode)) ?
        //                    dbProd.Products.Where(c => c.Barcode.Equals(_product.Barcode)).FirstOrDefault() :
        //                    null;

        //                    var _productNewId = -1;
        //                    if (_productOldFound != null) //product with barcode already exists
        //                    {
        //                        _productNewId = _productOldFound.Id;
        //                        _productsThatAlreadyExistWithBarcode++;
        //                        Console.WriteLine("Old product with barcode found - " + _productNewId);
        //                    }
        //                    else //Product with barcode doesn´t exist
        //                    {
        //                        Products _newProduct = new Products
        //                        {
        //                            InsertDate = DateTime.Now,
        //                            Name = _product.Name,
        //                            Picture = _product.Picture,
        //                            CreatedByUserId = _product.CreatedByUserId,
        //                            Price = 0,
        //                            Barcode = _product.Barcode,
        //                            Brand = _product.Brand,
        //                            CategoryString = _product.CategoryString,
        //                            VariableWeightPrice = _product.VariableWeightPrice,
        //                            Weight = _product.Weight
        //                        };
        //                        dbProd.Products.Add(_newProduct);
        //                        dbProd.SaveChanges();
        //                        _productNewId = _newProduct.Id;
        //                        Console.WriteLine("New product added - " + _productNewId);
        //                    }

        //                    //get stores of new product in LOCAL
        //                    var _storesNew = dbLocal.StoreProductsNew.Where(c => c.ProductId == _product.Id);
        //                    foreach (var _storeNew in _storesNew)
        //                    {
        //                        if (_storeNew.StoreId == 3)
        //                        {
        //                            string stop2 = "";
        //                        }
        //                        //check if store exists in PROD
        //                        var _storeExists = dbProd.StoreProducts.Where(c => c.ProductId == _productNewId && c.StoreId == _storeNew.StoreId).FirstOrDefault();

        //                        if (_storeExists != null)
        //                        {
        //                            _storeExists.Price = _storeNew.Price.Value;
        //                            _storeExists.UpdateDate = DateTime.Now;
        //                            _storeExists.NeedsUpdate = false;
        //                            _storeExists.OnlineProductId = _storeNew.OnlineProductId;
        //                            _storeExists.Url = _storeNew.Url;
        //                            dbProd.SaveChanges();
        //                        }
        //                        else
        //                        {
        //                            StoreProducts _storeProduct = new StoreProducts();
        //                            _storeProduct.Url = _storeNew.Url;
        //                            _storeProduct.Price = _storeNew.Price;
        //                            _storeProduct.UserId = _storeNew.UserId;
        //                            _storeProduct.ProductId = _productNewId;
        //                            _storeProduct.CreateDate = DateTime.Now;
        //                            _storeProduct.StoreId = _storeNew.StoreId;
        //                            _storeProduct.NeedsUpdate = false;
        //                            _storeProduct.OnlineProductId = _storeNew.OnlineProductId;
        //                            dbProd.StoreProducts.Add(_storeProduct);
        //                            dbProd.SaveChanges();
        //                        }
        //                    }
        //                }
        //                return Request.CreateResponse(HttpStatusCode.OK, _productsThatAlreadyExistWithBarcode);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //}

        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> GoGetterContinente()
        {
            int _totalProducts = 0;
            int _totalFoundEANProducts = 0;
            int _totalFoundStoreProducts = 0;
            int _totalFoundOnline = 0;
            int _totalNotFoundOnline = 0;
            int _totalFoundStoreProductsWithDifBarcode = 0;
            int _totalFoundNoStoreProductsWithBarcode = 0;
            int _totalFoundStoreProductsAndBarcode = 0;
            int _totalFoundNoStoreProductsNoBarcode = 0;
            int _totalFoundWeightEAN = 0;
            int _totalUpdatedProductsWithEAN = 0;

            int _totalProductsAddedToDBNewProductAndStore = 0;
            int _totalProductsAddedToDBNewStore = 0;
            int _totalProductsAddedToDBNewBarcode = 0;
            int _totalProductsCategoriesUpdated = 0;

            bool _stopIteneration = false;
            try
            {

                using (var strreader = new StreamReader(HttpContext.Current.Server.MapPath("~/App_Data/continente.json")))
                {
                    using (var reader = new JsonTextReader(strreader))
                    {
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonToken.StartObject)
                            {
                                var _product = JObject.Load(reader);
                                //GYJ tiene atributo type que puede ser Cliente o Sucursal

                                if (_totalProducts < 22848)
                                {
                                    _totalProducts++;
                                    continue;
                                }
                                var _js_productName = _product.GetValue("title").ToString();
                                var _js_productBrand = _product.GetValue("brand").ToString();
                                var _js_productWeight = _product.GetValue("web_package_content_info").ToString();
                                var _js_productImageUrl = _product.GetValue("image_url").ToString();
                                var _js_productCategory = _product.GetValue("category").ToString();
                                var _js_productSubCategory1 = _product.GetValue("sub_category1").ToString();
                                var _js_productSubCategory2 = _product.GetValue("sub_category2").ToString();
                                var _js_productSubCategory3 = _product.GetValue("sub_category3").ToString();
                                var _js_productSubCategory4 = _product.GetValue("sub_category4").ToString();
                                var _js_productEAN = _product.GetValue("EAN").ToString();
                                var _js_productId = _product.GetValue("unique_id").ToString();
                                var _js_productUrl = _product.GetValue("product_url").ToString();
                                var _js_productUrlParsed = _js_productUrl.Replace("https://www.continente.pt", "");
                                var _js_catalog_id_url = "(" + _product.GetValue("catalog_id").ToString() + ")";
                                //_js_productUrlParsed += _js_catalog_id_url;
                                var _js_productPrice = _product.GetValue("price_including_tax").ToString();
                                double _js_productPriceValue_value = Math.Round(double.Parse(_product.GetValue("price_including_tax").ToString()), 2);
                                var _js_productSalesUnit = _product.GetValue("sales_unit").ToString(); //Unit, Gram
                                double _js_price_ratio = !string.IsNullOrEmpty(_product.GetValue("price_capacity_ratio").ToString()) ?
                                    Math.Round(double.Parse(_product.GetValue("price_capacity_ratio").ToString()), 2) :
                                    0;
                                var _js_productVariableWeightPriceUnit = _product.GetValue("price_capacity_ratio_unit").ToString().ToLower()
                                    .Replace("liter", "lt")
                                    .Replace("gram", "gr")
                                    .Replace("kilogr", "kg")
                                    .Replace("meter", "m")
                                    .Replace("unit", "un");
                                if (string.IsNullOrEmpty(_js_productVariableWeightPriceUnit))
                                {
                                    _js_productVariableWeightPriceUnit = "un";
                                }
                                var _js_productVariableWeightPrice = (_js_price_ratio != 0) ?
                                    _js_price_ratio + "/" + _js_productVariableWeightPriceUnit :
                                    _js_productVariableWeightPriceUnit;

                                var _fullCategory = _js_productSubCategory1 + " > " + _js_productSubCategory2 + " > " + _js_productSubCategory3 + " > " + _js_productSubCategory4;
                                System.Diagnostics.Debug.WriteLine("ID: " + _js_productId);
                                System.Diagnostics.Debug.WriteLine("EAN: " + _js_productEAN);
                                System.Diagnostics.Debug.WriteLine("NAM: " + _js_productName);
                                System.Diagnostics.Debug.WriteLine("BRA: " + _js_productBrand);
                                System.Diagnostics.Debug.WriteLine("WEI: " + _js_productWeight);
                                System.Diagnostics.Debug.WriteLine("IMG: " + _js_productImageUrl);
                                System.Diagnostics.Debug.WriteLine("CAT: " + _js_productSubCategory1);
                                System.Diagnostics.Debug.WriteLine("FCAT: " + _fullCategory);
                                System.Diagnostics.Debug.WriteLine("PRICE: " + _js_productPriceValue_value);
                                System.Diagnostics.Debug.WriteLine("UNI: " + _js_productVariableWeightPriceUnit);
                                System.Diagnostics.Debug.WriteLine("F RATIO PRICE: " + _js_productVariableWeightPrice);
                                System.Diagnostics.Debug.WriteLine("URL: " + _js_productUrl);
                                System.Diagnostics.Debug.WriteLine("URLP: " + _js_productUrlParsed);

                                var _productWithBarcodeFound = Managers.ProductsManager.GetByBarcode(_js_productEAN);
                                if (_productWithBarcodeFound != null)
                                {
                                    System.Diagnostics.Debug.WriteLine("+BARCODE EXISTS");
                                    _totalFoundEANProducts++;
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("-BARCODE NOT FOUND");
                                }

                                //!IMPORTANT - this is how to check if barcode is of weight 
                                if (_js_productEAN.EndsWith("000000"))
                                {
                                    _totalFoundWeightEAN++;
                                }

                                System.Diagnostics.Debug.WriteLine("Trying to get product " + _js_productName + " online");
                                IMarketFetcher _continenteMarketFetcher = new Continente();
                                var _onlineResult = await _continenteMarketFetcher.GetProductMetadata(_js_productUrlParsed);
                                if (_onlineResult != null)
                                {
                                    System.Diagnostics.Debug.WriteLine("+ONLINE");
                                    _totalFoundOnline++;
                                }
                                else
                                {
                                    //!IMPORTANT, if this happens don´t save procuct tp database
                                    System.Diagnostics.Debug.WriteLine("-NOT FOUND ONLINE");
                                    _totalNotFoundOnline++;

                                    _totalProducts++;
                                    continue;
                                }

                                int _productExistsInStoresId = Managers.ProductsManager.CheckIfProductExistsInStore(_js_productUrlParsed);
                                if (_productExistsInStoresId != -1) //test if storeProduct with url exists
                                {
                                    System.Diagnostics.Debug.WriteLine("+STORE PRODUCT EXISTS");
                                    if (_productWithBarcodeFound != null)
                                    {
                                        System.Diagnostics.Debug.WriteLine("of ProductId " + _productWithBarcodeFound.Id);
                                    }
                                    _totalFoundStoreProducts++;
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("-STORE PRODUCT NOT FOUND");
                                    if (_productWithBarcodeFound != null)
                                    {
                                        System.Diagnostics.Debug.WriteLine("of ProductId " + _productWithBarcodeFound.Id);
                                    }
                                }




                                //Final logic

                                if (_productWithBarcodeFound == null && _productExistsInStoresId != -1) //barcode not found but storeProduct found
                                {
                                    System.Diagnostics.Debug.WriteLine("+-STORE PRODUCT FOUND BUT BARCODE NOT FOUND");
                                    if (_productWithBarcodeFound != null)
                                    {
                                        System.Diagnostics.Debug.WriteLine("of ProductId " + _productExistsInStoresId);
                                    }

                                    Products _productWithId = Managers.ProductsManager.GetById(_productExistsInStoresId);
                                    //db.Products.Where(c => c.Id == _productExistsInStoresId).FirstOrDefault();
                                    if (_productWithId != null &&
                                        (_productWithId.Barcode.Equals("0") || _productWithId.Barcode == null))
                                    {
                                        Products _productBarcodeSaved = Managers.ProductsManager.UpdateProductBarcode(_productExistsInStoresId, _js_productEAN);
                                        //update barcode, if barcode is 0 or null
                                        //_productWithId.Barcode = _js_productEAN;
                                        //db.SaveChanges();
                                        if (_productBarcodeSaved != null)
                                            _totalProductsAddedToDBNewBarcode++;


                                        _totalUpdatedProductsWithEAN++;
                                        _totalProducts++;
                                        continue; //go to next product
                                    }
                                    if (_productWithId != null && _js_productEAN.EndsWith("000000")) //is weight EAN, update it to new form (ending in 000000)
                                    {
                                        //update barcode, if barcode is of weight
                                        Products _productBarcodeSaved = Managers.ProductsManager.UpdateProductBarcode(_productExistsInStoresId, _js_productEAN);
                                        if (_productBarcodeSaved != null)
                                            _totalProductsAddedToDBNewBarcode++;
                                        //_productWithId.Barcode = _js_productEAN;
                                        //db.SaveChanges();

                                        _totalUpdatedProductsWithEAN++;
                                        _totalProducts++;


                                        continue; //go to next product
                                    }

                                    _totalFoundStoreProductsWithDifBarcode++;
                                }
                                if (_productWithBarcodeFound == null && _productExistsInStoresId == -1) //save product and storeProduct
                                {
                                    System.Diagnostics.Debug.WriteLine("--STORE PRODUCT AND BARCODE NOT FOUND");
                                    _totalFoundNoStoreProductsNoBarcode++;

                                    _onlineResult.PriceWeight = _js_price_ratio.ToString();
                                    _onlineResult.Unit = _js_productVariableWeightPriceUnit;
                                    _onlineResult.OnlineProductId = _js_productId;

                                    //Create new product
                                    SpiroWeb.Controllers.ProductsApiController _ProductsApiController = new ProductsApiController();
                                    ProductItemNew _ProductItemNew = new ProductItemNew();
                                    _ProductItemNew.IsToOverwrite = false;
                                    if (_onlineResult != null) //product found online
                                    {

                                        _onlineResult.FullCategory = _fullCategory;
                                        _onlineResult.Category = _js_productSubCategory1;
                                        _onlineResult.StoreId = 2;
                                        _onlineResult.Barcode = _js_productEAN;
                                        _onlineResult.OnlineProductId = _js_productId;
                                        _onlineResult.PriceWeight = _js_price_ratio.ToString();
                                        _onlineResult.Unit = _js_productVariableWeightPriceUnit;

                                        _ProductItemNew.Barcode = _js_productEAN;
                                        _ProductItemNew.UserId = "9ff8224f-17cf-49fb-b555-05779a13eb40";
                                        _ProductItemNew.FirstAddedProductFromStoreId = 2;
                                        _ProductItemNew.Lists = new List<string>();
                                        _ProductItemNew.SelectedResults = new List<LisieStores.Extensibility.ProductSearchResult>() { _onlineResult };
                                        var _result = _ProductsApiController.PostNewFunc(_ProductItemNew);
                                        if (_result != -9)
                                            _totalProductsAddedToDBNewProductAndStore++;

                                    }
                                    else //Not found online - what do do? - Nothing
                                    {

                                    }
                                }
                                if (_productWithBarcodeFound != null && _productExistsInStoresId != -1) //Just update category of product
                                {
                                    System.Diagnostics.Debug.WriteLine("++STORE PRODUCT AND BARCODE FOUND");
                                    _totalFoundStoreProductsAndBarcode++;

                                    //update product category
                                    var _productSaved = Managers.ProductsManager.UpdateProductCategory(_productWithBarcodeFound.Id, _js_productSubCategory1, _fullCategory);
                                    if (_productSaved != null)
                                        _totalProductsCategoriesUpdated++;

                                }
                                if (_productWithBarcodeFound != null && _productExistsInStoresId == -1) //save storeProduct
                                {
                                    System.Diagnostics.Debug.WriteLine("-+STORE PRODUCT NOT FOUND AND BARCODE FOUND");
                                    _totalFoundNoStoreProductsWithBarcode++;

                                    //update product category
                                    var _productSaved = Managers.ProductsManager.UpdateProductCategory(_productWithBarcodeFound.Id, _js_productSubCategory1, _fullCategory);
                                    if (_productSaved != null)
                                        _totalProductsCategoriesUpdated++;
                                    //_productWithBarcodeFound.FullCategory = _fullCategory;
                                    //_productWithBarcodeFound.CategoryString = _js_productSubCategory1;
                                    //var entry = db.Entry(_productWithBarcodeFound);
                                    //entry.Property(y => y.FullCategory).IsModified = true;
                                    //entry.Property(y => y.CategoryString).IsModified = true;
                                    //db.SaveChanges();

                                    //Add New Store
                                    if (_onlineResult != null)
                                    {
                                        _onlineResult.PriceWeight = _js_price_ratio.ToString();
                                        _onlineResult.Unit = _js_productVariableWeightPriceUnit;
                                        _onlineResult.OnlineProductId = _js_productId;
                                        bool _sucess = Managers.ProductsManager.CreateOrUpdateStoreProductNew(_onlineResult, _productWithBarcodeFound.Id, "9ff8224f-17cf-49fb-b555-05779a13eb40", 2);
                                        if (_sucess)
                                            _totalProductsAddedToDBNewStore++;
                                    }
                                }





                                _totalProducts++;
                                System.Diagnostics.Debug.WriteLine(_totalProducts + "-----------------------------");
                                System.Threading.Thread.Sleep(500);

                                if (_stopIteneration)
                                {
                                    break;
                                }

                            }
                        }
                    }
                }
                var _return = new { TotalProducts = _totalProducts, TotalFoundEANProducts = _totalFoundEANProducts };
                return Request.CreateResponse(HttpStatusCode.OK, _return);

            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }

    public class ConnectionHelper
    {
        public static string CreateConnectionString(string metaData, string dataSource, string initialCatalog)
        {
            const string appName = "EntityFramework";
            const string providerName = "System.Data.SqlClient";

            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = dataSource;
            sqlBuilder.InitialCatalog = initialCatalog;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.IntegratedSecurity = true;
            sqlBuilder.ApplicationName = appName;

            EntityConnectionStringBuilder efBuilder = new EntityConnectionStringBuilder();
            efBuilder.Metadata = metaData;
            efBuilder.Provider = providerName;
            efBuilder.ProviderConnectionString = sqlBuilder.ConnectionString;

            return efBuilder.ConnectionString;
        }

        public static SpiroStockManagementEntities CreateConnection(string metaData, string dataSource, string initialCatalog)
        {
            return new SpiroStockManagementEntities(ConnectionHelper.CreateConnectionString(metaData, dataSource, initialCatalog));
        }

        public static string GetCreateConnectionString()
        {
            string metaData = "res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl";
            string dataSource = "localhost";
            string initialCatalog = "Lisie";
            string _connectionString = ConnectionHelper.CreateConnectionString(
                    metaData,
                    dataSource,
                    initialCatalog);
            return _connectionString;

        }
    }
}
