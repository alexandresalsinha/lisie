using ClassLibrary1;
using PagedList;
using Shipwreck.Phash;
using Shipwreck.Phash.Bitmaps;
using SpiroWeb.Helpers;
using SpiroWeb.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SpiroWeb.Controllers
{
    public class ProductsMatcherWebController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        // GET: Products1
        [Authorize]
        public async Task<ActionResult> Index(string searchQuery = "")
        {
            OnlineProducts _OnlineProducts = new OnlineProducts();
            List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();
            List<LisieStores.Extensibility.ProductSearchResult> _jumboProductSearchResultList = await _OnlineProducts.GetJumboOnlineProductSearchResults(searchQuery);
            List<LisieStores.Extensibility.ProductSearchResult> _continenteProductSearchResultList = await _OnlineProducts.GetContinenteOnlineProductSearchResultsHeroku(searchQuery);
            List<LisieStores.Extensibility.ProductSearchResult> _pingoDoceProductSearchResultList = await _OnlineProducts.GetPingoDoceOnlineProductSearchResults(searchQuery);

            _ProductSearchResultList.AddRange(_jumboProductSearchResultList);
            _ProductSearchResultList.AddRange(_continenteProductSearchResultList);
            _ProductSearchResultList.AddRange(_pingoDoceProductSearchResultList);

            Session["currentProductsResults"] = _ProductSearchResultList;
            return View(_ProductSearchResultList);
        }

        [Authorize]
        public ActionResult Products(string orderBy, string searchQuery, int page = 1)
        {
            //var products = (string.IsNullOrEmpty(orderBy)) ? db.Products.ToList() : db.Products.OrderBy(c => c.Name).ToList();
            IQueryable<Products> products = db.Products;

            //if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(searchQuery))
            //{
            //    products = products.Where(c => c.Name.ToLower().Contains(searchQuery)).OrderBy(c => c.Name);
            //}
            //else 

            if (!string.IsNullOrEmpty(searchQuery))
            {
                string[] _searchWords = searchQuery.ToLower().Trim(' ').Split(' ');
                products = products.Where(c => (_searchWords.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z))))
                    .OrderBy(c => c.Name);

                //products = db.Products.Where(c => c.Name.ToLower().Contains(searchQuery.ToLower()) ||
                //                             c.CategoryString.ToLower().Contains(searchQuery.ToLower()) ||
                //                             c.Brand.ToLower().Contains(searchQuery.ToLower()));
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "name":
                        products = products.OrderBy(c => c.Name);
                        break;
                    case "insertdate":
                        products = products.OrderByDescending(c => c.InsertDate);
                        break;
                    default:
                        break;
                }
            }
            if (string.IsNullOrEmpty(searchQuery) && string.IsNullOrEmpty(orderBy))
            {
                products = db.Products.OrderBy(c => c.Id);
            }

            //var pageNumber = page ?? 1;
            var pageNumber = page;

            if (Session["productsIndexCurrentPage"] != null && Session["goToSavedproductsIndexPage"] != null)
            {
                pageNumber = Convert.ToInt32(Session["productsIndexCurrentPage"]);
                Session["goToSavedproductsIndexPage"] = null;
            }

            try
            {
                var onePageOfProducts = products.ToPagedList(pageNumber, 25);
                Session["productsIndexCurrentPage"] = pageNumber;
                ViewBag.OnePageOfProducts = onePageOfProducts;
            }
            catch (Exception ex)
            {
                string stop = "";
            }

            ViewBag.searchQuery = searchQuery;
            return View();
        }
        public ActionResult UserProducts(string userId, string orderBy = "", int page = 1)
        {
            //var products = (string.IsNullOrEmpty(orderBy)) ? db.Products.ToList() : db.Products.OrderBy(c => c.Name).ToList();
            //IQueryable<Products> products = db.Products;

            //if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(searchQuery))
            //{
            //    products = products.Where(c => c.Name.ToLower().Contains(searchQuery)).OrderBy(c => c.Name);
            //}
            //else 

            var _userShoppingInventoryProducts = SpiroWeb.Managers.UserListsManager.GetOfUser(userId, "shoppingList").OrderByDescending(c => c.Id);
            var _userInventoryProducts = SpiroWeb.Managers.UserListsManager.GetOfUser(userId, "inventory").OrderByDescending(c => c.Id);
            //var _userConsumedProducts= SpiroWeb.Managers.UserListsManager.GetOfUserConsumed(userId);
            List<UserProductsList> _joinedLists = new List<UserProductsList>();
            _joinedLists.AddRange(_userShoppingInventoryProducts);
            _joinedLists.AddRange(_userInventoryProducts);
            //List<ClassLibrary1.Products> _productsList = new List<Products>();

            var _products = _joinedLists.Select(c => c.Products);
            //if (!string.IsNullOrEmpty(searchQuery))
            //{
            //    string[] _searchWords = searchQuery.ToLower().Trim(' ').Split(' ');
            //    products = products.Where(c => (_searchWords.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z))))
            //        .OrderBy(c => c.Name);

            //    //products = db.Products.Where(c => c.Name.ToLower().Contains(searchQuery.ToLower()) ||
            //    //                             c.CategoryString.ToLower().Contains(searchQuery.ToLower()) ||
            //    //                             c.Brand.ToLower().Contains(searchQuery.ToLower()));
            //}
            //if (!string.IsNullOrEmpty(orderBy))
            //{
            //    switch (orderBy.ToLower())
            //    {
            //        case "name":
            //            products = products.OrderBy(c => c.Name);
            //            break;
            //        case "insertdate":
            //            products = products.OrderByDescending(c => c.InsertDate);
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //if (string.IsNullOrEmpty(searchQuery) && string.IsNullOrEmpty(orderBy))
            //{
            //    products = db.Products.OrderBy(c => c.Id);
            //}

            //var pageNumber = page ?? 1;
            var pageNumber = page;

            if (Session["productsIndexCurrentPage"] != null && Session["goToSavedproductsIndexPage"] != null)
            {
                pageNumber = Convert.ToInt32(Session["productsIndexCurrentPage"]);
                Session["goToSavedproductsIndexPage"] = null;
            }

            try
            {
                var onePageOfProducts = _products.ToPagedList(pageNumber, 50);
                Session["productsIndexCurrentPage"] = pageNumber;
                ViewBag.OnePageOfProducts = onePageOfProducts;
            }
            catch (Exception ex)
            {
                string stop = "";
            }

            ViewBag.searchQuery = "";
            return View("Products");
        }

        public async Task<ActionResult> Select(int currentIndex = -1)
        {
            if (Session["currentProductsResults"] != null)
            {

                List<LisieStores.Extensibility.ProductSearchResult> _currentProductsResults = Session["currentProductsResults"] as List<LisieStores.Extensibility.ProductSearchResult>;
                Session["selectedProduct"] = _currentProductsResults[currentIndex];
                var _selectedProductSearch = _currentProductsResults[currentIndex];

                List<ProductSearchMatchResult> _ProductSearchMatchResultList = new List<ProductSearchMatchResult>();

                //Download original image to media/temp
                Guid _guid = Guid.NewGuid();
                var sourceImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\Media\\Temp\\" + _guid.ToString() + ".jpg";
                //var sourceImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\Media\\Temp\\" + _selectedProductSearch.Name + _selectedProductSearch.Store + ".jpg";
                try
                {
                    Bitmap _sourceImage = await GetOnlineImage(_selectedProductSearch.ImageUrl);
                    _sourceImage.Save(sourceImagePath);
                }
                catch (Exception ex)
                {
                    return Json("error: " + ex.InnerException.Message, JsonRequestBehavior.AllowGet);
                }

                Bitmap _originalImage = new Bitmap(sourceImagePath);

                foreach (var _currentProductsResult in _currentProductsResults)
                {
                    //If product from same store don´t match and add to list
                    if (_currentProductsResult.StoreId.Equals(_selectedProductSearch.StoreId))
                        continue;

                    (string stringTogetherOfCompare, double percentage) percentageEquality = GetProductsMatchingPercentage(_selectedProductSearch, _currentProductsResult);
                    double percentageTextEquality = GetProductsTextMatchingPercentage(_selectedProductSearch, _currentProductsResult);
                    (string stringTogetherOfCompare, double percentage) percentageTextTogetherEquality = GetProductsTextTogetherMatchingPercentage(_selectedProductSearch, _currentProductsResult);


                    Bitmap _compareImage = await GetOnlineImage(_currentProductsResult.ImageUrl);
                    int imageSimilarity = 0;
                    //float imageSimilarity2 = 0;
                    if (_originalImage != null && _compareImage != null)
                    {
                        imageSimilarity = await CalculateImageSimilatiry(_originalImage, _compareImage);
                        //imageSimilarity2 = await CalculateImageSimilatiry2(_originalImage, _compareImage);
                    }
                    _ProductSearchMatchResultList.Add(new ProductSearchMatchResult
                    {
                        Name = _currentProductsResult.Name,
                        Brand = _currentProductsResult.Brand,
                        Weight = _currentProductsResult.Weight,
                        Price = _currentProductsResult.Price,
                        PriceWeight = _currentProductsResult.PriceWeight,
                        StoreName = _currentProductsResult.StoreName,
                        StoreId = _currentProductsResult.StoreId,
                        ImageUrl = _currentProductsResult.ImageUrl,
                        EqualsPercentage = percentageEquality.percentage,
                        TextEqualsPercentage = percentageTextEquality,
                        TextTogetherEqualsPercentage = percentageTextTogetherEquality.percentage,
                        ImageTextEqualsPercentage = (percentageTextEquality * 12 + imageSimilarity) / 2,
                        Url = _currentProductsResult.Url,
                        Category = _currentProductsResult.Category,
                        PriceLiteral = _currentProductsResult.PriceLiteral,
                        PriceWeightLiteral = _currentProductsResult.PriceWeightLiteral,
                        ImageEqualsPercentage = imageSimilarity

                    });
                }

                return View("Matcher", _ProductSearchMatchResultList.OrderByDescending(c => c.ImageTextEqualsPercentage));
            }

            //matching algorithm 

            return View("Matcher", Session["currentProductsResults"] as List<ProductSearchResult>);
        }

        public async Task<ActionResult> AutoMatch(int productId = -1)
        {
            List<ProductSearchMatchResult> _ProductSearchMatchResultList = new List<ProductSearchMatchResult>();
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                Products _selectedProduct = db.Products.Where(c => c.Id == productId).FirstOrDefault();

                Guid _guid = Guid.NewGuid();
                var sourceImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\ProductsPicture\\" + _guid.ToString() + ".jpg";
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                string _imageUrl = baseUrl + "/handlers/GetProductImage.ashx?productId=" + _selectedProduct.Id.ToString();

                Bitmap _sourceImage = null;
                //Download original image to /App_Data/ProductsImages
                try
                {
                    _sourceImage = await GetOnlineImage(_imageUrl);
                    _sourceImage.Save(sourceImagePath);
                }
                catch (Exception ex)
                {
                    Logger.Debug("error: " + ex.InnerException.Message);
                    return View("AutoMatch", new List<ProductSearchMatchResult>());
                }

                LisieStores.Extensibility.ProductSearchResult _selectedProductSearch = new LisieStores.Extensibility.ProductSearchResult
                {
                    Barcode = _selectedProduct.Barcode,
                    Brand = _selectedProduct.Brand,
                    Category = _selectedProduct.CategoryString,
                    ImageUrl = _imageUrl,
                    Name = _selectedProduct.Name,
                    Weight = _selectedProduct.Weight,
                    PriceWeight = _selectedProduct.VariableWeightPrice,
                    OnlineProductId = _selectedProduct.Id.ToString()
                };
                //Session["selectedProduct"] = _selectedProductSearch;
                ViewBag.SelectedProduct = _selectedProductSearch;

                //TODO - stop if brand is of a unique market
                if (_selectedProductSearch.Brand.ToLower() == "porsi")
                    return View("AutoMatch", new List<ProductSearchMatchResult>());

                if (_selectedProduct != null)
                {
                    //Get product Stores
                    var _storeProducts = db.StoreProducts.Where(c => c.ProductId == productId);

                    List<LisieStores.Extensibility.ProductSearchResult> _allsearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();
                    List<LisieStores.Extensibility.Market> _markets = Helpers.Extensibility.GetStoreFetchers();
                    _markets = _markets.OrderBy(c => c.StoreId).ToList();


                    foreach (var _market in _markets)
                    {
                        //Check if market exists in storeProducts
                        //if (_storeProducts.Where(c => c.StoreId == _market.StoreId).FirstOrDefault() == null)
                        //{
                        List<ProductSearchMatchResult> _marketResults = await MatchProductInStore(_selectedProduct, _market, _sourceImage);
                        var _marketResultsSorted = _marketResults.OrderByDescending(c => c.ImageTogetherTextEqualsPercentage);

                        //put second biggest Images Together +
                        //ImagesTogetherPercentage
                        var _bestNameMatches = _marketResults.OrderByDescending(c => c.TxPoTxTogetherPo);
                        var _bestImageMatches = _marketResults.OrderByDescending(c => c.ImagesTogetherPercentage);
                        var _bestImageTogetherTextMatches = _marketResults.OrderByDescending(c => c.ImageTextEqualsPercentage);
                        var _bestImageTogetherTextMatch = _bestImageTogetherTextMatches.FirstOrDefault();
                        var _bestLast2AvgMatches = _marketResults.OrderByDescending(c => c.Last2Avg);

                        List<ProductSearchMatchResult> _toReturn = _marketResultsSorted.ToList();
                        //If  ImageA1 + Text(x12) %  not in first or second place, put in second
                        var _firstBestLast2Avg = _bestLast2AvgMatches.FirstOrDefault();
                        var _secondBestLast2Avg = _bestLast2AvgMatches.ElementAtOrDefault(1);

                        //Reorder with best Last2Avg for second spot
                        if (_marketResultsSorted.Count() > 1 && _firstBestLast2Avg != null)
                        {
                            var _marketResultsSorted_first = _marketResultsSorted.First();
                            var _marketResultsSorted_second = _marketResultsSorted.ElementAt(1);

                            if (!CompareEqualsProductSearchMatchResult(_marketResultsSorted_first, _firstBestLast2Avg))
                            {
                                _toReturn.Remove(_firstBestLast2Avg);
                                _toReturn.Insert(1, _firstBestLast2Avg);
                            }
                            else
                            {
                                if (_marketResultsSorted.Count() > 1 && _secondBestLast2Avg != null)
                                {
                                    //var _marketResultsSorted_second = _marketResultsSorted.ElementAt(1);

                                    if (!CompareEqualsProductSearchMatchResult(_marketResultsSorted_second, _secondBestLast2Avg))
                                    {
                                        _toReturn.Remove(_secondBestLast2Avg);
                                        _toReturn.Insert(1, _secondBestLast2Avg);
                                    }
                                }
                            }
                        }

                        //Reorder with best ImageTogetherTextMatches for third spot
                        if (_marketResultsSorted.Count() > 2 && _bestImageTogetherTextMatch != null)
                        {

                            if (!CompareEqualsProductSearchMatchResult(_toReturn[0], _bestImageTogetherTextMatch) &&
                                !CompareEqualsProductSearchMatchResult(_toReturn[1], _bestImageTogetherTextMatch) &&
                                !CompareEqualsProductSearchMatchResult(_toReturn[2], _bestImageTogetherTextMatch))
                            {
                                _toReturn.Remove(_bestImageTogetherTextMatch);
                                _toReturn.Insert(2, _bestImageTogetherTextMatch);
                            }
                            //else
                            //{
                            //    //var _marketResultsSorted_second = _marketResultsSorted.ElementAt(1);

                            //    if (!CompareEqualsProductSearchMatchResult(_toReturn[1], _bestImageTogetherTextMatches.ElementAt(1)))
                            //    {
                            //        _toReturn.Remove(_bestImageTogetherTextMatch);
                            //        _toReturn.Insert(2, _bestImageTogetherTextMatch);
                            //    }
                            //    else
                            //    {
                            //        if (!CompareEqualsProductSearchMatchResult(_toReturn[2], _bestImageTogetherTextMatches.ElementAt(2)))
                            //        {
                            //            _toReturn.Remove(_bestImageTogetherTextMatch);
                            //            _toReturn.Insert(2, _bestImageTogetherTextMatch);
                            //        }
                            //    }

                            //}
                        }

                        //If Images Together is 200, put in first place
                        var _firstBestImageMatch = _bestImageMatches.FirstOrDefault();
                        if (_firstBestImageMatch != null && _firstBestImageMatch.ImagesTogetherPercentage == 200)
                        {
                            _toReturn.Remove(_firstBestImageMatch);
                            _toReturn.Insert(0, _firstBestImageMatch);
                        }

                        //the collection is already order by ImageTextEqualsPercentage
                        //if (_bestNameMatches.First() != _bestImageMatches.First())
                        //{
                        //    _marketResults.
                        //}

                        _ProductSearchMatchResultList.AddRange(_toReturn);
                        //_ProductSearchMatchResultList.AddRange(_marketResultsSorted);
                        //_ProductSearchMatchResultList.AddRange(_marketResults);
                        //}
                    }
                }
            }

            //second most important weight
            //return View("AutoMatch", _ProductSearchMatchResultList.OrderByDescending(c => c.ImageTextEqualsPercentage));
            //most important Weight
            //return View("AutoMatch", _ProductSearchMatchResultList.OrderByDescending(c => c.ImageTogetherTextEqualsPercentage));
            return View("AutoMatch", _ProductSearchMatchResultList);
            //third most important weight
            //return View("AutoMatch", _ProductSearchMatchResultList.OrderByDescending(c => c.FinalWeight2));

        }

        //Visualization of all best 4 of each algorithm
        public async Task<ActionResult> AutoMatch2(int productId = -1)
        {
            List<ProductSearchMatchResult> _ProductSearchMatchResultList = new List<ProductSearchMatchResult>();
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                Products _selectedProduct = db.Products.Where(c => c.Id == productId).FirstOrDefault();

                Guid _guid = Guid.NewGuid();
                var sourceImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\ProductsPicture\\" + _guid.ToString() + ".jpg";
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                string _imageUrl = baseUrl + "/handlers/GetProductImage.ashx?productId=" + _selectedProduct.Id.ToString();

                Bitmap _sourceImage = null;
                //Download original image to /App_Data/ProductsImages
                try
                {
                    _sourceImage = await GetOnlineImage(_imageUrl);
                    _sourceImage.Save(sourceImagePath);
                }
                catch (Exception ex)
                {
                    Logger.Debug("error: " + ex.InnerException.Message);
                    return View("AutoMatch2", new List<ProductSearchMatchResult>());
                }

                LisieStores.Extensibility.ProductSearchResult _selectedProductSearch = new LisieStores.Extensibility.ProductSearchResult
                {
                    Barcode = _selectedProduct.Barcode,
                    Brand = _selectedProduct.Brand,
                    Category = _selectedProduct.CategoryString,
                    ImageUrl = _imageUrl,
                    Name = _selectedProduct.Name,
                    Weight = _selectedProduct.Weight,
                    PriceWeight = _selectedProduct.VariableWeightPrice,
                    OnlineProductId = _selectedProduct.Id.ToString()
                };
                //Session["selectedProduct"] = _selectedProductSearch;
                ViewBag.SelectedProduct = _selectedProductSearch;

                //TODO - stop if brand is of a unique market
                if (_selectedProductSearch.Brand.ToLower() == "porsi")
                    return View("AutoMatch", new List<ProductSearchMatchResult>());

                if (_selectedProduct != null)
                {
                    //Get product Stores
                    var _storeProducts = db.StoreProducts.Where(c => c.ProductId == productId);

                    List<LisieStores.Extensibility.ProductSearchResult> _allsearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();
                    List<LisieStores.Extensibility.Market> _markets = Helpers.Extensibility.GetStoreFetchers();
                    _markets = _markets.OrderBy(c => c.StoreId).ToList();

                    foreach (var _market in _markets)
                    {
                        List<ProductSearchMatchResult> _marketResults = await MatchProductInStore(_selectedProduct, _market, _sourceImage);
                        _ProductSearchMatchResultList.AddRange(_marketResults);
                    }
                }
            }
            return View("AutoMatch2", _ProductSearchMatchResultList);
        }

        //take the best ones of each algorithm, and get the most common between them, and with more points
        //place1 - 3points
        //place2 - 2 points
        //place3 - 1 point
        public async Task<ActionResult> AutoMatch3(int productId = -1)
        {
            List<ProductSearchMatchResult> _ProductSearchMatchResultList = new List<ProductSearchMatchResult>();
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                Products _selectedProduct = db.Products.Where(c => c.Id == productId).FirstOrDefault();

                Guid _guid = Guid.NewGuid();
                var sourceImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\ProductsPicture\\" + _guid.ToString() + ".jpg";
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                string _imageUrl = baseUrl + "/handlers/GetProductImage.ashx?productId=" + _selectedProduct.Id.ToString();

                Bitmap _sourceImage = null;
                //Download original image to /App_Data/ProductsImages
                try
                {
                    _sourceImage = await GetOnlineImage(_imageUrl);
                    _sourceImage.Save(sourceImagePath);
                }
                catch (Exception ex)
                {
                    Logger.Debug("error: " + ex.InnerException.Message);
                    return View("AutoMatch", new List<ProductSearchMatchResult>());
                }

                LisieStores.Extensibility.ProductSearchResult _selectedProductSearch = new LisieStores.Extensibility.ProductSearchResult
                {
                    Barcode = _selectedProduct.Barcode,
                    Brand = _selectedProduct.Brand,
                    Category = _selectedProduct.CategoryString,
                    ImageUrl = _imageUrl,
                    Name = _selectedProduct.Name,
                    Weight = _selectedProduct.Weight,
                    PriceWeight = _selectedProduct.VariableWeightPrice,
                    OnlineProductId = _selectedProduct.Id.ToString()
                };
                //Session["selectedProduct"] = _selectedProductSearch;
                ViewBag.SelectedProduct = _selectedProductSearch;

                //TODO - stop if brand is of a unique market
                if (_selectedProductSearch.Brand.ToLower() == "porsi" ||
                    _selectedProductSearch.Brand.ToLower() == "pingo doce" ||
                    _selectedProductSearch.Brand.ToLower() == "auchan"
                    )
                    return View("AutoMatch3", new List<ProductSearchMatchResult>());

                if (_selectedProduct != null)
                {
                    //Get product Stores
                    var _storeProducts = db.StoreProducts.Where(c => c.ProductId == productId);

                    //List<LisieStores.Extensibility.ProductSearchResult> _allsearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();
                    List<LisieStores.Extensibility.Market> _markets = Helpers.Extensibility.GetStoreFetchers();
                    _markets = _markets.OrderBy(c => c.StoreId).ToList();

                    foreach (var _market in _markets)
                    {
                        //if (_market.StoreId != 2 && _market.StoreId != 3) continue;
                        List<ProductSearchMatchResult> _marketResults = await MatchProductInStore(_selectedProduct, _market, _sourceImage);

                        var _bestNameMatches = _marketResults.OrderByDescending(c => c.TxPoTxTogetherPo).Take(4);
                        var _bestImageMatches = _marketResults.OrderByDescending(c => c.ImagesTogetherPercentage).Take(4);
                        var _bestImageTextEqualsPercentageMatches = _marketResults.OrderByDescending(c => c.ImageTextEqualsPercentage).Take(4);
                        var _bestLast2AvgMatches = _marketResults.OrderByDescending(c => c.Last2Avg).Take(4);

                        var _bestImageTogetherTextEqualsPercentageMatches = _marketResults.OrderByDescending(c => c.ImageTogetherTextEqualsPercentage).Take(4);

                        Dictionary<ProductSearchMatchResult, int> _productsWeights = new Dictionary<ProductSearchMatchResult, int>();
                        List<ProductSearchMatchResult> _toReturnInOrder = new List<ProductSearchMatchResult>();

                        //Image Together +Tx % +Tx Together % - ImageTogetherTextEqualsPercentage
                        int _currentPositionPoints = 8;
                        foreach (var _bestImageTogetherTextEqualsPercentageMatch in _bestImageTogetherTextEqualsPercentageMatches)
                        {
                            var _productExistsInDictionary = _productsWeights.Where(c => CompareEqualsProductSearchMatchResult(c.Key, _bestImageTogetherTextEqualsPercentageMatch)).Any();
                            if (_productExistsInDictionary)
                            {
                                //_productsWeights[_bestImageTogetherTextEqualsPercentageMatch] = _productsWeights[_bestImageTogetherTextEqualsPercentageMatch] + _currentPositionPoints;
                                var _pointsToAdd = _currentPositionPoints;
                                if (_currentPositionPoints == 8) _pointsToAdd += 6;
                                if (_currentPositionPoints == 6) _pointsToAdd += 4;
                                //if (_currentPositionPoints == 4) _pointsToAdd += 1;

                                if (_currentPositionPoints == 4) _pointsToAdd += 2;
                                if (_currentPositionPoints == 2) _pointsToAdd += 1;
                                _productsWeights[_bestImageTogetherTextEqualsPercentageMatch] = _productsWeights[_bestImageTogetherTextEqualsPercentageMatch] + _pointsToAdd;
                            }
                            else
                            {
                                _productsWeights.Add(_bestImageTogetherTextEqualsPercentageMatch, _currentPositionPoints);
                            }
                            _currentPositionPoints -= 2;
                        }

                        //TxPoTxTogetherPo
                        _currentPositionPoints = 8;
                        foreach (var _bestNameMatch in _bestNameMatches)
                        {
                            var _productExistsInDictionary = _productsWeights.Where(c => CompareEqualsProductSearchMatchResult(c.Key, _bestNameMatch)).Any();
                            if (_productExistsInDictionary)
                            {
                                var _pointsToAdd = _currentPositionPoints;
                                if (_currentPositionPoints == 8) _pointsToAdd += 6;
                                if (_currentPositionPoints == 6) _pointsToAdd += 4;
                                //if (_currentPositionPoints == 4) _pointsToAdd += 1;

                                if (_currentPositionPoints == 4) _pointsToAdd += 2;
                                if (_currentPositionPoints == 2) _pointsToAdd += 1;
                                _productsWeights[_bestNameMatch] = _productsWeights[_bestNameMatch] + _pointsToAdd;
                            }
                            else
                            {
                                _productsWeights.Add(_bestNameMatch, _currentPositionPoints);
                            }
                            _currentPositionPoints -= 2;
                        }
                        //ImagesTogetherPercentage
                        _currentPositionPoints = 8; //start two points above, because it has more wi
                        foreach (var _bestImageMatch in _bestImageMatches)
                        {
                            var _productExistsInDictionary = _productsWeights.Where(c => CompareEqualsProductSearchMatchResult(c.Key, _bestImageMatch)).Any();
                            if (_productExistsInDictionary)
                            {
                                var _pointsToAdd = _currentPositionPoints;
                                if (_currentPositionPoints == 8) _pointsToAdd += 6;
                                if (_currentPositionPoints == 6) _pointsToAdd += 4;
                                //if (_currentPositionPoints == 4) _pointsToAdd += 1;

                                if (_currentPositionPoints == 4) _pointsToAdd += 2;
                                if (_currentPositionPoints == 2) _pointsToAdd += 1;
                                _productsWeights[_bestImageMatch] = _productsWeights[_bestImageMatch] + _pointsToAdd;
                            }
                            else
                            {
                                _productsWeights.Add(_bestImageMatch, _currentPositionPoints);
                            }
                            _currentPositionPoints -= 2;
                        }
                        //ImageTextEqualsPercentage
                        _currentPositionPoints = 8;
                        foreach (var __bestImageTextEqualsPercentageMatch in _bestImageTextEqualsPercentageMatches)
                        {
                            var _productExistsInDictionary = _productsWeights.Where(c => CompareEqualsProductSearchMatchResult(c.Key, __bestImageTextEqualsPercentageMatch)).Any();
                            if (_productExistsInDictionary)
                            {
                                var _pointsToAdd = _currentPositionPoints;
                                if (_currentPositionPoints == 8) _pointsToAdd += 6;
                                if (_currentPositionPoints == 6) _pointsToAdd += 4;
                                //if (_currentPositionPoints == 4) _pointsToAdd += 1;

                                if (_currentPositionPoints == 4) _pointsToAdd += 2;
                                if (_currentPositionPoints == 2) _pointsToAdd += 1;
                                _productsWeights[__bestImageTextEqualsPercentageMatch] = _productsWeights[__bestImageTextEqualsPercentageMatch] + _pointsToAdd;
                            }
                            else
                            {
                                _productsWeights.Add(__bestImageTextEqualsPercentageMatch, _currentPositionPoints);
                            }
                            _currentPositionPoints -= 2;
                        }
                        //Last2Avg
                        _currentPositionPoints = 8;
                        foreach (var _bestLast2AvgMatch in _bestLast2AvgMatches)
                        {
                            var _productExistsInDictionary = _productsWeights.Where(c => CompareEqualsProductSearchMatchResult(c.Key, _bestLast2AvgMatch)).Any();
                            if (_productExistsInDictionary)
                            {
                                var _pointsToAdd = _currentPositionPoints;
                                if (_currentPositionPoints == 8) _pointsToAdd += 6;
                                if (_currentPositionPoints == 6) _pointsToAdd += 4;
                                //if (_currentPositionPoints == 4) _pointsToAdd += 1;

                                if (_currentPositionPoints == 4) _pointsToAdd += 2;
                                if (_currentPositionPoints == 2) _pointsToAdd += 1;
                                _productsWeights[_bestLast2AvgMatch] = _productsWeights[_bestLast2AvgMatch] + _pointsToAdd;
                            }
                            else
                            {
                                _productsWeights.Add(_bestLast2AvgMatch, _currentPositionPoints);
                            }
                            _currentPositionPoints -= 2;
                        }

                        var _orderedMatchResults = _productsWeights.OrderByDescending(c => c.Value).Select(c => c.Key);
                        foreach (var _orderedMatchResult in _orderedMatchResults)
                        {
                            _orderedMatchResult.SortedWeight = _productsWeights[_orderedMatchResult];
                        }

                        _toReturnInOrder = _orderedMatchResults.ToList();
                        var _firstBestImageMatch = _bestImageMatches.FirstOrDefault();
                        if (_firstBestImageMatch != null && _firstBestImageMatch.ImagesTogetherPercentage == 200)
                        {
                            _toReturnInOrder.Remove(_firstBestImageMatch);
                            _toReturnInOrder.Insert(0, _firstBestImageMatch);
                        }

                        _ProductSearchMatchResultList.AddRange(_toReturnInOrder);


                    }
                }
            }
            return View("AutoMatch3", _ProductSearchMatchResultList);
        }

        public async Task<ActionResult> AutoAutoMatch(string userId)
        {
            List<ProductSearchMatchResult> _ProductSearchMatchResultList = new List<ProductSearchMatchResult>();
            List<LisieStores.Extensibility.Market> _markets = SpiroWeb.Helpers.Extensibility.GetStoreFetchers();
            List<LisieStores.Extensibility.Market> _marketsToMatch = new List<LisieStores.Extensibility.Market>();

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _userShoppingInventoryProducts = SpiroWeb.Managers.UserListsManager.GetOfUser(userId, "shoppingList").OrderByDescending(c => c.Id);
                var _userInventoryProducts = SpiroWeb.Managers.UserListsManager.GetOfUser(userId, "inventory").OrderByDescending(c => c.Id);
                //var _userConsumedProducts= SpiroWeb.Managers.UserListsManager.GetOfUserConsumed(userId);
                List<UserProductsList> _joinedLists = new List<UserProductsList>();
                _joinedLists.AddRange(_userShoppingInventoryProducts);
                _joinedLists.AddRange(_userInventoryProducts);
                //List<ClassLibrary1.Products> _productsList = new List<Products>();

                Products _selectedProduct = null;
                int _currentStoreId = -1;

                var _products = _joinedLists.Select(c => c.Products);
                foreach (var _product in _products)
                {
                    //COntinue to next product if brand is of a unique market
                    if (_product.Brand.ToLower().Trim() == "porsi" ||
                        _product.Brand.ToLower().Trim() == "pingo doce" ||
                        _product.Brand.ToLower().Trim() == "dia" ||
                        _product.Brand.ToLower().Trim() == "auchan"
                        )
                        continue;
                    foreach (var _market in _markets.OrderBy(c => c.StoreId))
                    {
                        var _StoreProduct = db.StoreProducts.Where(c => c.ProductId == _product.Id && c.StoreId == _market.StoreId).FirstOrDefault();
                        if (_StoreProduct == null)
                        {
                            var _ProductAutoMatchedInStore = db.ProductsAutoMatched.Where(c => c.ProductId == _product.Id && c.StoreId == _market.StoreId).FirstOrDefault();
                            if (_ProductAutoMatchedInStore == null)
                            {
                                _currentStoreId = _market.StoreId;
                                _selectedProduct = _product;
                                break;
                            }
                        }
                    }
                    if (_selectedProduct != null) break;
                }

                //if no more products to auto match , return
                if (_selectedProduct == null) return View("AutoAutoMatch", new List<ProductSearchMatchResult>());

                ViewBag.CurrentStoreId = _currentStoreId;

                Guid _guid = Guid.NewGuid();
                var sourceImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\ProductsPicture\\" + _guid.ToString() + ".jpg";
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                string _imageUrl = baseUrl + "/handlers/GetProductImage.ashx?productId=" + _selectedProduct.Id.ToString();

                Bitmap _sourceImage = null;
                //Download original image to /App_Data/ProductsImages
                try
                {
                    _sourceImage = await GetOnlineImage(_imageUrl);
                    _sourceImage.Save(sourceImagePath);
                }
                catch (Exception ex)
                {
                    Logger.Debug("error: " + ex.InnerException.Message);
                    return View("AutoMatch", new List<ProductSearchMatchResult>());
                }

                LisieStores.Extensibility.ProductSearchResult _selectedProductSearch = new LisieStores.Extensibility.ProductSearchResult
                {
                    Barcode = _selectedProduct.Barcode,
                    Brand = _selectedProduct.Brand,
                    Category = _selectedProduct.CategoryString,
                    ImageUrl = _imageUrl,
                    Name = _selectedProduct.Name,
                    Weight = _selectedProduct.Weight,
                    PriceWeight = _selectedProduct.VariableWeightPrice,
                    OnlineProductId = _selectedProduct.Id.ToString()
                };
                //Session["selectedProduct"] = _selectedProductSearch;
                ViewBag.SelectedProduct = _selectedProductSearch;

                //TODO - stop if brand is of a unique market
                if (_selectedProductSearch.Brand.ToLower() == "porsi" ||
                    _selectedProductSearch.Brand.ToLower() == "pingo doce" ||
                    _selectedProductSearch.Brand.ToLower() == "auchan"
                    )
                    return View("AutoAutoMatch", new List<ProductSearchMatchResult>());

                if (_selectedProduct != null)
                {
                    //Get product Stores
                    var _storeProducts = db.StoreProducts.Where(c => c.ProductId == _selectedProduct.Id);

                    //List<LisieStores.Extensibility.ProductSearchResult> _allsearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();
                    _markets = _markets.OrderBy(c => c.StoreId).ToList();

                    foreach (var _market in _markets)
                    {
                        var _storeProduct2 = _storeProducts.Where(c => c.StoreId.Equals(_market.StoreId)).FirstOrDefault();
                        var _storeProduct2Count = _storeProducts.Where(c => c.StoreId.Equals(_market.StoreId)).Count(); ;
                        if (_storeProduct2 != null) //If it exists in StoreProducts don´t search in store
                            continue;

                        _marketsToMatch.Add(_market);

                        //if (_market.StoreId != 2 && _market.StoreId != 3) continue;
                        List<ProductSearchMatchResult> _marketResults = await MatchProductInStore(_selectedProduct, _market, _sourceImage);

                        //GOOD OLD ONE
                        //var _bestNameMatches = _marketResults.OrderByDescending(c => c.TxPoTxTogetherPo).Take(4);
                        //var _bestImageMatches = _marketResults.OrderByDescending(c => c.ImagesTogetherPercentage).Take(4);
                        //var _bestImageTextEqualsPercentageMatches = _marketResults.OrderByDescending(c => c.ImageTextEqualsPercentage).Take(4);
                        //var _bestLast2AvgMatches = _marketResults.OrderByDescending(c => c.Last2Avg).Take(4);
                        //var _bestImageTogetherTextEqualsPercentageMatches = _marketResults.OrderByDescending(c => c.ImageTogetherTextEqualsPercentage).Take(4);
                        var _bestNameMatches = _marketResults.OrderByDescending(c => c.TxPoTxTogetherPo).Take(8);
                        var _bestImageMatches = _marketResults.OrderByDescending(c => c.ImagesTogetherPercentage).Take(8);
                        var _bestImageTextEqualsPercentageMatches = _marketResults.OrderByDescending(c => c.ImageTextEqualsPercentage).Take(8);
                        var _bestLast2AvgMatches = _marketResults.OrderByDescending(c => c.Last2Avg).Take(8);
                        var _bestImageTogetherTextEqualsPercentageMatches = _marketResults.OrderByDescending(c => c.ImageTogetherTextEqualsPercentage).Take(8);

                        Dictionary<ProductSearchMatchResult, int> _productsWeights = new Dictionary<ProductSearchMatchResult, int>();
                        List<ProductSearchMatchResult> _toReturnInOrder = new List<ProductSearchMatchResult>();

                        //Image Together +Tx % +Tx Together % - ImageTogetherTextEqualsPercentage
                        int _currentPositionPoints = 8;
                        foreach (var _bestImageTogetherTextEqualsPercentageMatch in _bestImageTogetherTextEqualsPercentageMatches)
                        {
                            var _productExistsInDictionary = _productsWeights.Where(c => CompareEqualsProductSearchMatchResult(c.Key, _bestImageTogetherTextEqualsPercentageMatch)).Any();
                            if (_productExistsInDictionary)
                            {
                                //_productsWeights[_bestImageTogetherTextEqualsPercentageMatch] = _productsWeights[_bestImageTogetherTextEqualsPercentageMatch] + _currentPositionPoints;
                                var _pointsToAdd = _currentPositionPoints;
                                if (_currentPositionPoints == 8) _pointsToAdd += 6;
                                if (_currentPositionPoints == 6) _pointsToAdd += 4;
                                //if (_currentPositionPoints == 4) _pointsToAdd += 1;

                                if (_currentPositionPoints == 4) _pointsToAdd += 2;
                                if (_currentPositionPoints == 2) _pointsToAdd += 1;
                                _productsWeights[_bestImageTogetherTextEqualsPercentageMatch] = _productsWeights[_bestImageTogetherTextEqualsPercentageMatch] + _pointsToAdd;
                            }
                            else
                            {
                                _productsWeights.Add(_bestImageTogetherTextEqualsPercentageMatch, _currentPositionPoints);
                            }
                            _currentPositionPoints -= 2;
                        }

                        //TxPoTxTogetherPo
                        _currentPositionPoints = 8;
                        foreach (var _bestNameMatch in _bestNameMatches)
                        {
                            var _productExistsInDictionary = _productsWeights.Where(c => CompareEqualsProductSearchMatchResult(c.Key, _bestNameMatch)).Any();
                            if (_productExistsInDictionary)
                            {
                                var _pointsToAdd = _currentPositionPoints;
                                if (_currentPositionPoints == 8) _pointsToAdd += 6;
                                if (_currentPositionPoints == 6) _pointsToAdd += 4;
                                //if (_currentPositionPoints == 4) _pointsToAdd += 1;

                                if (_currentPositionPoints == 4) _pointsToAdd += 2;
                                if (_currentPositionPoints == 2) _pointsToAdd += 1;
                                _productsWeights[_bestNameMatch] = _productsWeights[_bestNameMatch] + _pointsToAdd;
                            }
                            else
                            {
                                _productsWeights.Add(_bestNameMatch, _currentPositionPoints);
                            }
                            _currentPositionPoints -= 2;
                        }
                        //ImagesTogetherPercentage
                        _currentPositionPoints = 8; //start two points above, because it has more wi
                        foreach (var _bestImageMatch in _bestImageMatches)
                        {
                            var _productExistsInDictionary = _productsWeights.Where(c => CompareEqualsProductSearchMatchResult(c.Key, _bestImageMatch)).Any();
                            if (_productExistsInDictionary)
                            {
                                var _pointsToAdd = _currentPositionPoints;
                                if (_currentPositionPoints == 8) _pointsToAdd += 6;
                                if (_currentPositionPoints == 6) _pointsToAdd += 4;
                                //if (_currentPositionPoints == 4) _pointsToAdd += 1;

                                if (_currentPositionPoints == 4) _pointsToAdd += 2;
                                if (_currentPositionPoints == 2) _pointsToAdd += 1;
                                _productsWeights[_bestImageMatch] = _productsWeights[_bestImageMatch] + _pointsToAdd;
                            }
                            else
                            {
                                _productsWeights.Add(_bestImageMatch, _currentPositionPoints);
                            }
                            _currentPositionPoints -= 2;
                        }
                        //ImageTextEqualsPercentage
                        _currentPositionPoints = 8;
                        foreach (var __bestImageTextEqualsPercentageMatch in _bestImageTextEqualsPercentageMatches)
                        {
                            var _productExistsInDictionary = _productsWeights.Where(c => CompareEqualsProductSearchMatchResult(c.Key, __bestImageTextEqualsPercentageMatch)).Any();
                            if (_productExistsInDictionary)
                            {
                                var _pointsToAdd = _currentPositionPoints;
                                if (_currentPositionPoints == 8) _pointsToAdd += 6;
                                if (_currentPositionPoints == 6) _pointsToAdd += 4;
                                //if (_currentPositionPoints == 4) _pointsToAdd += 1;

                                if (_currentPositionPoints == 4) _pointsToAdd += 2;
                                if (_currentPositionPoints == 2) _pointsToAdd += 1;
                                _productsWeights[__bestImageTextEqualsPercentageMatch] = _productsWeights[__bestImageTextEqualsPercentageMatch] + _pointsToAdd;
                            }
                            else
                            {
                                _productsWeights.Add(__bestImageTextEqualsPercentageMatch, _currentPositionPoints);
                            }
                            _currentPositionPoints -= 2;
                        }
                        //Last2Avg
                        _currentPositionPoints = 8;
                        foreach (var _bestLast2AvgMatch in _bestLast2AvgMatches)
                        {
                            var _productExistsInDictionary = _productsWeights.Where(c => CompareEqualsProductSearchMatchResult(c.Key, _bestLast2AvgMatch)).Any();
                            if (_productExistsInDictionary)
                            {
                                var _pointsToAdd = _currentPositionPoints;
                                if (_currentPositionPoints == 8) _pointsToAdd += 6;
                                if (_currentPositionPoints == 6) _pointsToAdd += 4;
                                //if (_currentPositionPoints == 4) _pointsToAdd += 1;

                                if (_currentPositionPoints == 4) _pointsToAdd += 2;
                                if (_currentPositionPoints == 2) _pointsToAdd += 1;
                                _productsWeights[_bestLast2AvgMatch] = _productsWeights[_bestLast2AvgMatch] + _pointsToAdd;
                            }
                            else
                            {
                                _productsWeights.Add(_bestLast2AvgMatch, _currentPositionPoints);
                            }
                            _currentPositionPoints -= 2;
                        }

                        var _orderedMatchResults = _productsWeights.OrderByDescending(c => c.Value).Select(c => c.Key);
                        foreach (var _orderedMatchResult in _orderedMatchResults)
                        {
                            _orderedMatchResult.SortedWeight = _productsWeights[_orderedMatchResult];
                        }

                        _toReturnInOrder = _orderedMatchResults.ToList();
                        var _firstBestImageMatch = _bestImageMatches.FirstOrDefault();
                        if (_firstBestImageMatch != null && _firstBestImageMatch.ImagesTogetherPercentage == 200)
                        {
                            _toReturnInOrder.Remove(_firstBestImageMatch);
                            _firstBestImageMatch.SortedWeight = 70;
                            _toReturnInOrder.Insert(0, _firstBestImageMatch);
                        }

                        var __bestNameMatch = _bestNameMatches.FirstOrDefault();
                        if (__bestNameMatch != null && __bestNameMatch.TxPoTxTogetherPo == 100)
                        {
                            _toReturnInOrder.Remove(__bestNameMatch);
                            __bestNameMatch.SortedWeight = 70;
                            _toReturnInOrder.Insert(0, __bestNameMatch);
                        }

                        _ProductSearchMatchResultList.AddRange(_toReturnInOrder);
                    }
                }
            }
            ViewBag.MarketsToMatch = _marketsToMatch.OrderBy(c => c.StoreId).Select(c => c.StoreId).ToList();
            return View("AutoAutoMatch", _ProductSearchMatchResultList);
        }

        public bool CompareEqualsProductSearchMatchResult(ProductSearchMatchResult productSearchMatchResult1, ProductSearchMatchResult productSearchMatchResult2)
        {
            if (productSearchMatchResult1.Brand == productSearchMatchResult2.Brand &&
                productSearchMatchResult1.Category == productSearchMatchResult2.Category &&
                productSearchMatchResult1.EqualsPercentage == productSearchMatchResult2.EqualsPercentage &&
                productSearchMatchResult1.EqualsPercentageText == productSearchMatchResult2.EqualsPercentageText &&
                productSearchMatchResult1.FinalWeight == productSearchMatchResult2.FinalWeight &&
                productSearchMatchResult1.FinalWeight2 == productSearchMatchResult2.FinalWeight2 &&
                productSearchMatchResult1.ImageEqualsPercentage == productSearchMatchResult2.ImageEqualsPercentage &&
                productSearchMatchResult1.ImageEqualsPercentage2 == productSearchMatchResult2.ImageEqualsPercentage2 &&
                productSearchMatchResult1.ImagesTogetherPercentage == productSearchMatchResult2.ImagesTogetherPercentage &&
                productSearchMatchResult1.ImageTextEqualsPercentage == productSearchMatchResult2.ImageTextEqualsPercentage &&
                productSearchMatchResult1.ImageTogetherTextEqualsPercentage == productSearchMatchResult2.ImageTogetherTextEqualsPercentage &&
                productSearchMatchResult1.ImageUrl == productSearchMatchResult2.ImageUrl &&
                productSearchMatchResult1.IsSeperator == productSearchMatchResult2.IsSeperator &&
                productSearchMatchResult1.Last2Avg == productSearchMatchResult2.Last2Avg &&
                productSearchMatchResult1.Name == productSearchMatchResult2.Name &&
                productSearchMatchResult1.Price == productSearchMatchResult2.Price &&
                productSearchMatchResult1.PriceLiteral == productSearchMatchResult2.PriceLiteral &&
                productSearchMatchResult1.PriceWeight == productSearchMatchResult2.PriceWeight &&
                productSearchMatchResult1.PriceWeightLiteral == productSearchMatchResult2.PriceWeightLiteral &&
                productSearchMatchResult1.Search == productSearchMatchResult2.Search &&
                productSearchMatchResult1.SeparatorTitle == productSearchMatchResult2.SeparatorTitle &&
                productSearchMatchResult1.StoreColor == productSearchMatchResult2.StoreColor &&
                productSearchMatchResult1.StoreId == productSearchMatchResult2.StoreId &&
                productSearchMatchResult1.StoreName == productSearchMatchResult2.StoreName &&
                productSearchMatchResult1.TextEqualsPercentage == productSearchMatchResult2.TextEqualsPercentage &&
                productSearchMatchResult1.TextTogetherEqualsPercentage == productSearchMatchResult2.TextTogetherEqualsPercentage &&
                productSearchMatchResult1.TextTogetherEqualsPercentageText == productSearchMatchResult2.TextTogetherEqualsPercentageText &&
                productSearchMatchResult1.TxAllTogetherPo == productSearchMatchResult2.TxAllTogetherPo &&
                productSearchMatchResult1.TxPoTxTogetherPo == productSearchMatchResult2.TxPoTxTogetherPo &&
                productSearchMatchResult1.Url == productSearchMatchResult2.Url &&
                productSearchMatchResult1.ViewableUrl == productSearchMatchResult2.ViewableUrl &&
                productSearchMatchResult1.Weight == productSearchMatchResult2.Weight
                )
            {
                return true;
            }
            else
                return false;
        }
        public async Task<ActionResult> Match(int productId, int storeId, string storeProductUrl)
        {
            List<ProductSearchMatchResult> _ProductSearchMatchResultList = new List<ProductSearchMatchResult>();

            LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = await Helpers.Extensibility.GetProductStoreMetadata(storeId, storeProductUrl);

            if (_ProductSearchResult != null)
            {
                Managers.ProductsManager.CreateOrUpdateStoreProductNew(_ProductSearchResult, productId, "9ff8224f-17cf-49fb-b555-05779a13eb40", storeId);
            }

            return RedirectToAction("AutoMatch", "ProductsMatcherWeb", new { productId = productId });
        }

        /// <summary>
        /// Good Old One
        /// </summary>
        /// <param name="productOriginal"></param>
        /// <param name="productToCompare"></param>
        /// <returns></returns>
        //public double GetProductsMatchingPercentage(LisieStores.Extensibility.ProductSearchResult productOriginal, LisieStores.Extensibility.ProductSearchResult productToCompare)
        //{
        //    double nameEquality = CalculateSimilarity(productOriginal.Name.ToLower(), productToCompare.Name.ToLower());
        //    double brandEquality = CalculateSimilarity(productOriginal.Brand.ToLower(), productToCompare.Brand.ToLower());
        //    double weightEquality = CalculateSimilarity(productOriginal.Weight.ToLower(), productToCompare.Weight.ToLower());
        //    double weightPriceEquality = CalculateSimilarity(productOriginal.PriceWeight.ToLower(), productToCompare.PriceWeight.ToLower());

        //    string stringTogetherOriginal = productOriginal.Name.ToLower() + " " + (productOriginal.Brand.ToLower() + " " + productOriginal.Weight.ToLower());
        //    string stringTogetherToCompare = productToCompare.Name.ToLower() + " " + (productToCompare.Brand.ToLower() + " " + productToCompare.Weight.ToLower());

        //    double stringTogetherEquality = CalculateSimilarity(stringTogetherOriginal, stringTogetherToCompare);


        //    //Now obsolete, because the stripping of auchan results now has the weight right
        //    //double finalPercentage = (productOriginal.StoreId == 1) ? //"Jumbo"
        //    //                                                          //(nameEquality + brandEquality) / 2 :
        //    //                                                          //(nameEquality + brandEquality + weightEquality) / 3;
        //    //    (nameEquality * 100 + brandEquality * 100 + CalculateSimilarity(productOriginal.Name.ToLower(), productToCompare.Weight.ToLower()) * 100) / 3 :
        //    //    (nameEquality * 100 + brandEquality * 100 + weightEquality * 100) / 3;
        //    double finalPercentage = (nameEquality * 100 + brandEquality * 100 + weightEquality * 100) / 3;



        //    //double finalPercentage = (nameEquality + brandEquality + weightEquality + weightPriceEquality) / 4;

        //    double finalFinalPercentage = (finalPercentage + stringTogetherEquality * 100) / 2;

        //    return finalFinalPercentage;
        //}
        public (string stringTogetherOfCompare, double percentage) GetProductsMatchingPercentage(LisieStores.Extensibility.ProductSearchResult productOriginal, LisieStores.Extensibility.ProductSearchResult productToCompare)
        {
            string _strippedNameOriginal = GetProductNameStripped(productOriginal).Trim(' ');
            string _strippedNameToCompare = GetProductNameStripped(productToCompare).Trim(' ');

            double nameEquality = CalculateSimilarity(_strippedNameOriginal, _strippedNameToCompare);
            double brandEquality = CalculateSimilarity(productOriginal.Brand.ToLower(), productToCompare.Brand.ToLower());
            double weightEquality = CalculateSimilarity(productOriginal.Weight.ToLower(), productToCompare.Weight.ToLower());
            //double weightPriceEquality = CalculateSimilarity(productOriginal.PriceWeight.ToLower(), productToCompare.PriceWeight.ToLower());

            string stringTogetherOriginal = _strippedNameOriginal + " " + productOriginal.Brand.ToLower() + " " + productOriginal.Weight.ToLower();
            string stringTogetherToCompare = _strippedNameToCompare + " " + productToCompare.Brand.ToLower() + " " + productToCompare.Weight.ToLower();

            double stringTogetherEquality = CalculateSimilarity(stringTogetherOriginal, stringTogetherToCompare);
            double finalPercentage = (nameEquality * 100 + brandEquality * 100 + weightEquality * 100) / 3;

            double finalFinalPercentage = (finalPercentage + stringTogetherEquality * 100) / 2;
            return (_strippedNameToCompare, finalFinalPercentage);
        }


        /// <summary>
        /// Good old one
        /// </summary>
        /// <param name="productOriginal"></param>
        /// <param name="productToCompare"></param>
        /// <returns></returns>
        //public double GetProductsTextMatchingPercentage(LisieStores.Extensibility.ProductSearchResult productOriginal, LisieStores.Extensibility.ProductSearchResult productToCompare)
        //{
        //    double nameEquality = CalculateSimilarity(productOriginal.Name.ToLower(), productToCompare.Name.ToLower());
        //    double brandEquality = CalculateSimilarity(productOriginal.Brand.ToLower(), productToCompare.Brand.ToLower());
        //    double weightEquality = CalculateSimilarity(productOriginal.Weight.ToLower(), productToCompare.Weight.ToLower());
        //    double weightPriceEquality = CalculateSimilarity(productOriginal.PriceWeight.ToLower(), productToCompare.PriceWeight.ToLower());

        //    //Now obsolete, because the stripping of auchan results now has the weight right
        //    //double finalPercentage = (productOriginal.StoreId == 1) ? //"Jumbo"
        //    //     (nameEquality * 100 + brandEquality * 100 + CalculateSimilarity(productOriginal.Name.ToLower(), productToCompare.Weight.ToLower()) * 100) / 3 :
        //    //     (nameEquality * 100 + brandEquality * 100 + weightEquality * 100) / 3;
        //    double finalPercentage = (nameEquality * 100 + brandEquality * 100 + weightEquality * 100) / 3;

        //    return finalPercentage;
        //}
        public double GetProductsTextMatchingPercentage(LisieStores.Extensibility.ProductSearchResult productOriginal, LisieStores.Extensibility.ProductSearchResult productToCompare)
        {
            string _strippedNameOriginal = GetProductNameStripped(productOriginal).Trim(' ');
            string _strippedNameToCompare = GetProductNameStripped(productToCompare).Trim(' ');

            double nameEquality = CalculateSimilarity(_strippedNameOriginal, _strippedNameToCompare);
            double brandEquality = CalculateSimilarity(productOriginal.Brand.ToLower(), productToCompare.Brand.ToLower());
            double weightEquality = CalculateSimilarity(productOriginal.Weight.ToLower(), productToCompare.Weight.ToLower());
            //double weightPriceEquality = CalculateSimilarity(productOriginal.PriceWeight.ToLower(), productToCompare.PriceWeight.ToLower());

            double finalPercentage = (nameEquality * 100 + brandEquality * 100 + weightEquality * 100) / 3;
            return finalPercentage;
        }

        /// <summary>
        /// Good Old one
        /// </summary>
        /// <param name="productOriginal"></param>
        /// <param name="productToCompare"></param>
        /// <returns></returns>
        //public double GetProductsTextTogetherMatchingPercentage(LisieStores.Extensibility.ProductSearchResult productOriginal, LisieStores.Extensibility.ProductSearchResult productToCompare)
        //{
        //    double nameEquality = CalculateSimilarity(productOriginal.Name.ToLower(), productToCompare.Name.ToLower());
        //    double brandEquality = CalculateSimilarity(productOriginal.Brand.ToLower(), productToCompare.Brand.ToLower());
        //    double weightEquality = CalculateSimilarity(productOriginal.Weight.ToLower(), productToCompare.Weight.ToLower());
        //    double weightPriceEquality = CalculateSimilarity(productOriginal.PriceWeight.ToLower(), productToCompare.PriceWeight.ToLower());

        //    string stringTogetherOriginal = productOriginal.Name.ToLower() + " " + (productOriginal.Brand.ToLower() + " " + productOriginal.Weight.ToLower());
        //    string stringTogetherToCompare = productToCompare.Name.ToLower() + " " + (productToCompare.Brand.ToLower() + " " + productToCompare.Weight.ToLower());

        //    double stringTogetherEquality = CalculateSimilarity(stringTogetherOriginal, stringTogetherToCompare);



        //    return stringTogetherEquality * 100;
        //}

        public (string stringTogetherOfCompare, double percentage) GetProductsTextTogetherMatchingPercentage(LisieStores.Extensibility.ProductSearchResult productOriginal, LisieStores.Extensibility.ProductSearchResult productToCompare)
        {
            //Remove brand and weight from name
            string _strippedNameOriginal = GetProductNameStripped(productOriginal).Trim(' ');
            string _strippedNameToCompare = GetProductNameStripped(productToCompare).Trim(' ');

            double nameEquality = CalculateSimilarity(_strippedNameOriginal, _strippedNameToCompare);
            double brandEquality = CalculateSimilarity(productOriginal.Brand.ToLower(), productToCompare.Brand.ToLower());
            double weightEquality = CalculateSimilarity(productOriginal.Weight.ToLower(), productToCompare.Weight.ToLower());
            //double weightPriceEquality = CalculateSimilarity(productOriginal.PriceWeight.ToLower(), productToCompare.PriceWeight.ToLower());

            string stringTogetherOriginal = _strippedNameOriginal + " " + (productOriginal.Brand.ToLower() + " " + productOriginal.Weight.ToLower());
            string stringTogetherToCompare = _strippedNameToCompare + " " + (productToCompare.Brand.ToLower() + " " + productToCompare.Weight.ToLower());

            double stringTogetherEquality = CalculateSimilarity(stringTogetherOriginal, stringTogetherToCompare);

            return (stringTogetherToCompare, stringTogetherEquality * 100);
        }

        /// <summary>
        /// Calculate percentage similarity of two strings
        /// <param name="source">Source String to Compare with</param>
        /// <param name="target">Targeted String to Compare</param>
        /// <returns>Return Similarity between two strings from 0 to 1.0</returns>
        /// </summary>
        double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
        }

        async Task<int> CalculateImageSimilatiry(Bitmap originalmage, Bitmap compareImage)
        {
            //List<bool> iHash1 = GetHash(new Bitmap(@"D:\My Creative Projects\SpiroStockManagement Web\Images and Barcodes for testing\chocapic 375g jumbo.jpg"));
            //List<bool> iHash2 = GetHash(new Bitmap(@"D:\My Creative Projects\SpiroStockManagement Web\Images and Barcodes for testing\chocapic chococruh 410g continente.jpg"));


            List<bool> iHash1 = GetHash(originalmage);
            List<bool> iHash2 = GetHash(compareImage);

            //if any of the hashes is null return 0
            if (iHash1 == null || iHash2 == null) return 0;

            //determine the number of equal pixel (x of 256)
            int equalElements = iHash1.Zip(iHash2, (i, j) => i == j).Count(eq => eq);

            //return equalElements;
            //return in percentage 
            return 100 * equalElements / 256;
        }
        async Task<float> CalculateImageSimilatiry2(Bitmap originalmage, Bitmap compareImage)
        {
            //List<bool> iHash1 = GetHash(new Bitmap(@"D:\My Creative Projects\SpiroStockManagement Web\Images and Barcodes for testing\chocapic 375g jumbo.jpg"));
            //List<bool> iHash2 = GetHash(new Bitmap(@"D:\My Creative Projects\SpiroStockManagement Web\Images and Barcodes for testing\chocapic chococruh 410g continente.jpg"));

            //TODO - improve dowload of images, just once for both algorithms
            var bitmap = originalmage;
            var bitmap2 = compareImage;


            var hash1 = ImagePhash.ComputeDigest(bitmap.ToLuminanceImage());
            var hash2 = ImagePhash.ComputeDigest(bitmap2.ToLuminanceImage());

            var score = ImagePhash.GetCrossCorrelation(hash1, hash2);

            return score * 100;
        }

        public static List<bool> GetHash(Bitmap bmpSource)
        {
            try
            {
                List<bool> lResult = new List<bool>();
                //create new image with 16x16 pixel
                Bitmap bmpMin = new Bitmap(bmpSource, new Size(16, 16));
                for (int j = 0; j < bmpMin.Height; j++)
                {
                    for (int i = 0; i < bmpMin.Width; i++)
                    {
                        //reduce colors to true / false                
                        lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                    }
                }
                return lResult;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return null;
            }
        }

        public async Task<Bitmap> GetOnlineImage(string url)
        {
            var images = new List<Bitmap>();
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(url);
                    //var bitmap = new Bitmap(
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            var memStream = new MemoryStream();
                            await stream.CopyToAsync(memStream);
                            memStream.Position = 0;
                            return new Bitmap(memStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex.Message);
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the number of steps required to transform the source string
        /// into the target string.
        /// </summary>
        int ComputeLevenshteinDistance(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;

            int sourceWordCount = source.Length;
            int targetWordCount = target.Length;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceWordCount; i++)
            {
                for (int j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
        }

        async Task<List<ProductSearchMatchResult>> MatchProductInStore(Products selectedProduct, LisieStores.Extensibility.Market market, Bitmap originalImage)
        {
            //List<LisieStores.Extensibility.ProductSearchResult> _allsearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();
            List<ProductSearchMatchResult> _ProductSearchMatchResultList = new List<ProductSearchMatchResult>();

            Guid _guid = Guid.NewGuid();
            var sourceImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\ProductsPicture\\" + _guid.ToString() + ".jpg";
            //var sourceImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\Media\\Temp\\" + _selectedProductSearch.Name + _selectedProductSearch.Store + ".jpg";
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            string _imageUrl = baseUrl + "/handlers/GetProductImage.ashx?productId=" + selectedProduct.Id.ToString();

            LisieStores.Extensibility.ProductSearchResult _selectedProductSearch = new LisieStores.Extensibility.ProductSearchResult
            {
                Barcode = selectedProduct.Barcode,
                Brand = selectedProduct.Brand,
                Category = selectedProduct.CategoryString,
                ImageUrl = _imageUrl,
                Name = selectedProduct.Name,
                Weight = selectedProduct.Weight,
                PriceWeight = selectedProduct.VariableWeightPrice
            };

            LisieStores.Extensibility.IMarketFetcher _marketFetcher = (LisieStores.Extensibility.IMarketFetcher)Activator.CreateInstance(market.ClassType);
            _marketFetcher.StoreId = market.StoreId;
            _marketFetcher.StoreName = market.StoreName;
            _marketFetcher.StoreColor = market.StoreColor;

            //Divide product name in words
            //string[] _searchWords = selectedProduct.Name.ToLower().Trim(' ').Split(' ');
            string _strippedName = GetProductNameStripped(_selectedProductSearch);
            string[] _searchWords = StripUnWordsSearchQuery(_strippedName).Trim(' ').Split(' ');
            //string[] _searchWords = GetProductNameStripped(_selectedProductSearch).Trim(' ').Split(' ');
            string _searchQuery = (_searchWords.Length > 1) ? _searchWords[0] + " " + _searchWords[1] + " " + selectedProduct.Brand.ToLower() : _searchWords[0] + " " + selectedProduct.Brand;

            for (int i = _searchWords.Length; i > 0; i--)
            {
                string _currentSearchQuery = string.Empty;
                for (int z = 0; z < i; z++)
                {
                    _currentSearchQuery += _searchWords[z] + " ";
                }
                _currentSearchQuery = _currentSearchQuery.Trim();

                //TODO - in the end search all again but without brand in the end
                if (_currentSearchQuery.ToLower().IndexOf(selectedProduct.Brand.ToLower()) == -1)
                {
                    _currentSearchQuery += " " + selectedProduct.Brand.ToLower();
                }

                System.Diagnostics.Debug.WriteLine(_marketFetcher.StoreName + " - " + _currentSearchQuery);
                List<LisieStores.Extensibility.ProductSearchResult> _searchResults = await _marketFetcher.GetSearchResults(_currentSearchQuery.Replace("/", " "));
                if (_searchResults != null && _searchResults.Count > 0)
                {
                    foreach (var _currentProductsResult in _searchResults)
                    {
                        //If product from same store don´t match and add to list
                        //if (_currentProductsResult.StoreId.Equals(_selectedProductSearch.StoreId))
                        //    continue;

                        //Obsolete
                        //double percentageEquality = GetProductsMatchingPercentage(_selectedProductSearch, _currentProductsResult);
                        //double percentageTextEquality = GetProductsTextMatchingPercentage(_selectedProductSearch, _currentProductsResult);
                        //double percentageTextTogetherEquality = GetProductsTextTogetherMatchingPercentage(_selectedProductSearch, _currentProductsResult);

                        (string stringTogetherOfCompare, double percentage) percentageEquality = GetProductsMatchingPercentage(_selectedProductSearch, _currentProductsResult);
                        double percentageTextEquality = GetProductsTextMatchingPercentage(_selectedProductSearch, _currentProductsResult);
                        (string stringTogetherOfCompare, double percentage) percentageTextTogetherEquality = GetProductsTextTogetherMatchingPercentage(_selectedProductSearch, _currentProductsResult);

                        System.Diagnostics.Debug.WriteLine(_currentProductsResult.ImageUrl);

                        Bitmap _compareImage = await GetOnlineImage(_currentProductsResult.ImageUrl);
                        int imageSimilarity = 0;
                        float imageSimilarity2 = 0;
                        if (originalImage != null && _compareImage != null)
                        {
                            imageSimilarity = await CalculateImageSimilatiry(originalImage, _compareImage);
                            imageSimilarity2 = await CalculateImageSimilatiry2(originalImage, _compareImage);
                        }

                        double _TxPoTxTogetherPo = ((percentageTextTogetherEquality.percentage + percentageTextEquality) / 2);
                        double _TxAllTogetherPlus = percentageTextTogetherEquality.percentage + percentageTextEquality + percentageEquality.percentage;

                        //double _TxAllTogetherPo = ((percentageTextTogetherEquality.percentage + percentageTextEquality + percentageEquality.percentage) / 3);

                        double _ImageTextEqualsPercentage = (percentageTextEquality * 12 + imageSimilarity) / 2;
                        double _ImagesTogetherPlus = imageSimilarity + imageSimilarity2;
                        double _ImageTogetherTextEqualsPercentage = (_TxPoTxTogetherPo + _ImagesTogetherPlus) / 2;

                        var _avgText = (percentageTextTogetherEquality.percentage + percentageTextEquality) / 2;
                        var _avgImg = (imageSimilarity + imageSimilarity2) / 2;
                        _avgImg *= 3;
                        //_avgImg = _avgImg + 1000;
                        var _avgTextPlusImg = (_ImageTextEqualsPercentage * 100) / 650;
                        //var _avgTextPlusImg = item.ImageTextEqualsPercentage;
                        var _finalWeight = (_avgImg + _avgText + _avgTextPlusImg) / 3;

                        var _last2Avg = (_ImageTextEqualsPercentage + _ImageTogetherTextEqualsPercentage * 2) / 2;
                        var _finalWeigh2 = (_ImageTextEqualsPercentage + _last2Avg + _finalWeight) / 3;

                        _ProductSearchMatchResultList.Add(new ProductSearchMatchResult
                        {
                            Name = _currentProductsResult.Name,
                            Search = _currentSearchQuery,
                            Brand = _currentProductsResult.Brand,
                            Weight = _currentProductsResult.Weight,
                            Price = _currentProductsResult.Price,
                            PriceWeight = _currentProductsResult.PriceWeight,
                            StoreName = _currentProductsResult.StoreName,
                            StoreId = _currentProductsResult.StoreId,
                            ImageUrl = _currentProductsResult.ImageUrl,
                            EqualsPercentage = percentageEquality.percentage,
                            EqualsPercentageText = percentageEquality.stringTogetherOfCompare,
                            TextEqualsPercentage = percentageTextEquality,
                            TextTogetherEqualsPercentage = percentageTextTogetherEquality.percentage,
                            TxPoTxTogetherPo = _TxPoTxTogetherPo,
                            TextTogetherEqualsPercentageText = percentageTextTogetherEquality.stringTogetherOfCompare,
                            ImageTextEqualsPercentage = _ImageTextEqualsPercentage,
                            //ImageTogetherTextEqualsPercentage = (percentageTextEquality * 12 + imageSimilarity + imageSimilarity2) / 3,
                            //ImageTogetherTextEqualsPercentage = (((percentageTextTogetherEquality.percentage + percentageTextEquality) / 2) * 12 + imageSimilarity + imageSimilarity2) / 3,
                            ImageTogetherTextEqualsPercentage = _ImageTogetherTextEqualsPercentage,
                            //ImageTextEqualsPercentage = (percentageTextEquality * 3 + imageSimilarity * 2) / 2,
                            Url = _currentProductsResult.Url,
                            Category = _currentProductsResult.Category,
                            PriceLiteral = _currentProductsResult.PriceLiteral,
                            PriceWeightLiteral = _currentProductsResult.PriceWeightLiteral,
                            ImageEqualsPercentage = imageSimilarity,
                            ImageEqualsPercentage2 = imageSimilarity2,
                            ImagesTogetherPercentage = imageSimilarity + imageSimilarity2,
                            ViewableUrl = market.StoreUrl + _currentProductsResult.ViewableUrl,
                            FinalWeight = _finalWeight,
                            FinalWeight2 = _finalWeigh2,
                            Last2Avg = _last2Avg,
                            TxAllTogetherPlus = _TxAllTogetherPlus
                            //TxAllTogetherPo = _TxAllTogetherPo
                        });
                    }
                    break;
                }
            }

            //If no results, search just for brand (divided by words), and by first 3 words of name, and diminishing
            if (_ProductSearchMatchResultList.Count == 0)
            {
                string[] _searchBrandWords = _selectedProductSearch.Brand.ToLower().Trim(' ').Split(' ');
                List<LisieStores.Extensibility.ProductSearchResult> _searchResults = new List<LisieStores.Extensibility.ProductSearchResult>();

                string _currentSearchQuery = string.Empty;

                //Name
                _strippedName = GetProductNameStripped(_selectedProductSearch);
                string[] _searchNameWords = StripUnWordsSearchQuery(_strippedName).Trim(' ').Split(' ');
                for (int i = (_searchNameWords.Length > 2 ? 3 : _searchNameWords.Length); i > 0; i--)
                {
                    _currentSearchQuery = string.Empty;
                    for (int z = 0; z < i; z++)
                    {
                        _currentSearchQuery += _searchWords[z] + " ";
                    }
                    _currentSearchQuery = _currentSearchQuery.Trim();
                    System.Diagnostics.Debug.WriteLine(_marketFetcher.StoreName + " - " + _currentSearchQuery);
                    List<LisieStores.Extensibility.ProductSearchResult> _productSearchResults = await _marketFetcher.GetSearchResults(_currentSearchQuery.Replace("/", " "));
                    if (_productSearchResults.Count > 0)
                    {
                        _searchResults.AddRange(_productSearchResults);
                        break;
                    }
                }
                //Brand - if still no search results
                if (_searchResults != null && _searchResults.Count == 0)
                {
                    _currentSearchQuery = string.Empty;
                    for (int i = _searchBrandWords.Length; i > 0; i--)
                    {
                        _currentSearchQuery = string.Empty;
                        for (int z = 0; z < i; z++)
                        {
                            _currentSearchQuery += _searchBrandWords[z] + " ";
                        }
                        _currentSearchQuery = _currentSearchQuery.Trim();

                        System.Diagnostics.Debug.WriteLine(_marketFetcher.StoreName + " - " + _currentSearchQuery);
                        List<LisieStores.Extensibility.ProductSearchResult> _productSearchResults = await _marketFetcher.GetSearchResults(_currentSearchQuery.Replace("/", " "));
                        if (_productSearchResults.Count > 0)
                        {
                            _searchResults.AddRange(_productSearchResults);
                            break;
                        }

                    }
                }
                //If search results, match
                if (_searchResults != null && _searchResults.Count > 0)
                {
                    foreach (var _currentProductsResult in _searchResults)
                    {
                        ProductSearchMatchResult _currentProductSearchMatchResult = await CalculateProductSearchMatchResult(_selectedProductSearch, _currentProductsResult, originalImage, market, _currentSearchQuery);
                        _ProductSearchMatchResultList.Add(_currentProductSearchMatchResult);
                    }
                }
            }

            return _ProductSearchMatchResultList;
        }

        public string GetProductNameStripped(LisieStores.Extensibility.ProductSearchResult productSearchResult)
        {
            string _toRet = string.Empty;
            try
            {
                _toRet = productSearchResult.Name.ToLower().Replace("  ", " ").Trim(' ');
                _toRet = (!string.IsNullOrEmpty(productSearchResult.Brand)) ? _toRet.Replace(productSearchResult.Brand.ToLower(), "").Replace("  ", " ").Trim(' ') : _toRet;
                _toRet = (!string.IsNullOrEmpty(productSearchResult.Weight)) ? _toRet.Replace(productSearchResult.Weight.ToLower(), "").Replace("  ", " ").Trim(' ') : _toRet;
                return _toRet;

            }
            catch (Exception ex)
            {
                return _toRet;
            }
        }

        async public Task<ProductSearchMatchResult> CalculateProductSearchMatchResult(LisieStores.Extensibility.ProductSearchResult _selectedProductSearch, LisieStores.Extensibility.ProductSearchResult _currentProductsResult, Bitmap originalImage, LisieStores.Extensibility.Market market, string _currentSearchQuery)
        {
            (string stringTogetherOfCompare, double percentage) percentageEquality = GetProductsMatchingPercentage(_selectedProductSearch, _currentProductsResult);
            double percentageTextEquality = GetProductsTextMatchingPercentage(_selectedProductSearch, _currentProductsResult);
            (string stringTogetherOfCompare, double percentage) percentageTextTogetherEquality = GetProductsTextTogetherMatchingPercentage(_selectedProductSearch, _currentProductsResult);

            System.Diagnostics.Debug.WriteLine(_currentProductsResult.ImageUrl);

            Bitmap _compareImage = await GetOnlineImage(_currentProductsResult.ImageUrl);
            int imageSimilarity = 0;
            float imageSimilarity2 = 0;
            if (originalImage != null && _compareImage != null)
            {
                imageSimilarity = await CalculateImageSimilatiry(originalImage, _compareImage);
                imageSimilarity2 = await CalculateImageSimilatiry2(originalImage, _compareImage);
            }

            double _TxPoTxTogetherPo = ((percentageTextTogetherEquality.percentage + percentageTextEquality) / 2);
            double _TxAllTogetherPlus = percentageTextTogetherEquality.percentage + percentageTextEquality + percentageEquality.percentage;
            double _ImageTextEqualsPercentage = (percentageTextEquality * 12 + imageSimilarity) / 2;
            double _ImagesTogetherPlus = imageSimilarity + imageSimilarity2;
            double _ImageTogetherTextEqualsPercentage = (_TxPoTxTogetherPo + _ImagesTogetherPlus) / 2;

            var _avgText = (percentageTextTogetherEquality.percentage + percentageTextEquality) / 2;
            var _avgImg = (imageSimilarity + imageSimilarity2) / 2;
            _avgImg *= 3;
            var _avgTextPlusImg = (_ImageTextEqualsPercentage * 100) / 650;
            var _finalWeight = (_avgImg + _avgText + _avgTextPlusImg) / 3;

            var _last2Avg = (_ImageTextEqualsPercentage + _ImageTogetherTextEqualsPercentage * 2) / 2;
            var _finalWeigh2 = (_ImageTextEqualsPercentage + _last2Avg + _finalWeight) / 3;

            return new ProductSearchMatchResult
            {
                Name = _currentProductsResult.Name,
                Search = _currentSearchQuery,
                Brand = _currentProductsResult.Brand,
                Weight = _currentProductsResult.Weight,
                Price = _currentProductsResult.Price,
                PriceWeight = _currentProductsResult.PriceWeight,
                StoreName = _currentProductsResult.StoreName,
                StoreId = _currentProductsResult.StoreId,
                ImageUrl = _currentProductsResult.ImageUrl,
                EqualsPercentage = percentageEquality.percentage,
                EqualsPercentageText = percentageEquality.stringTogetherOfCompare,
                TextEqualsPercentage = percentageTextEquality,
                TextTogetherEqualsPercentage = percentageTextTogetherEquality.percentage,
                TxPoTxTogetherPo = _TxPoTxTogetherPo,
                TextTogetherEqualsPercentageText = percentageTextTogetherEquality.stringTogetherOfCompare,
                ImageTextEqualsPercentage = _ImageTextEqualsPercentage,
                //ImageTogetherTextEqualsPercentage = (percentageTextEquality * 12 + imageSimilarity + imageSimilarity2) / 3,
                //ImageTogetherTextEqualsPercentage = (((percentageTextTogetherEquality.percentage + percentageTextEquality) / 2) * 12 + imageSimilarity + imageSimilarity2) / 3,
                ImageTogetherTextEqualsPercentage = _ImageTogetherTextEqualsPercentage,
                //ImageTextEqualsPercentage = (percentageTextEquality * 3 + imageSimilarity * 2) / 2,
                Url = _currentProductsResult.Url,
                Category = _currentProductsResult.Category,
                PriceLiteral = _currentProductsResult.PriceLiteral,
                PriceWeightLiteral = _currentProductsResult.PriceWeightLiteral,
                ImageEqualsPercentage = imageSimilarity,
                ImageEqualsPercentage2 = imageSimilarity2,
                ImagesTogetherPercentage = imageSimilarity + imageSimilarity2,
                ViewableUrl = market.StoreUrl + _currentProductsResult.ViewableUrl,
                FinalWeight = _finalWeight,
                FinalWeight2 = _finalWeigh2,
                Last2Avg = _last2Avg,
                TxAllTogetherPlus = _TxAllTogetherPlus
                //TxAllTogetherPo = _TxAllTogetherPo
            };
        }

        public string StripUnWordsSearchQuery(string searchQuery)
        {
            string _return = searchQuery.ToLower().Replace("p/", " ");
            _return = _return.ToLower().Replace("c/", " ");
            _return = _return.ToLower().Replace(" com ", " ");
            _return = _return.ToLower().Replace(" para ", " ");
            _return = _return.ToLower().Replace(" de ", " ");
            return _return;
        }
    }
}