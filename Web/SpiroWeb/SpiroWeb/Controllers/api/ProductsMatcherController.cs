using ClassLibrary1;
using SpiroWeb.Helpers;
using SpiroWeb.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SpiroWeb.Controllers
{
    public class ProductsMatcherController : ApiController
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        // GET: api/UserLists
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/UserLists/5
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> Get(string search, string selectedResultUrl, int selectedStoreId)
        {
            ProductsMatcher _ProductsMatcher = new ProductsMatcher();
            LisieStores.Extensibility.ProductSearchResult _selectedProductSearch = null;
            List<LisieStores.Extensibility.ProductSearchResult> _newSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();
            Bitmap _sourceImage = null;

            List<LisieStores.Extensibility.Market> _markets = Extensibility.GetStoreFetchers();
            _markets = _markets.OrderBy(c => c.StoreId).ToList();

            //Get Selected Store results and selected product and download image of product
            LisieStores.Extensibility.Market _selectedMarket = _markets.Find(c => c.StoreId == selectedStoreId);
            if (_selectedMarket != null)
            {
                //Get Store Search Results
                LisieStores.Extensibility.IMarketFetcher _marketFetcher = (LisieStores.Extensibility.IMarketFetcher)Activator.CreateInstance(_selectedMarket.ClassType);
                _marketFetcher.StoreId = _selectedMarket.StoreId;
                _marketFetcher.StoreName = _selectedMarket.StoreName;
                _marketFetcher.StoreColor = _selectedMarket.StoreColor;
                List<LisieStores.Extensibility.ProductSearchResult> _searchResults = await _marketFetcher.GetSearchResults(search);

                //Add store separator
                _newSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
                {
                    StoreId = _selectedMarket.StoreId,
                    StoreName = _selectedMarket.StoreName,
                    StoreColor = _selectedMarket.StoreColor,
                    IsSeperator = true,
                    SeparatorTitle = _selectedMarket.StoreName + " (" + _searchResults.Count + ")"
                });
                _newSearchResultList.AddRange(_searchResults);

                //Get selected Product

                try
                {
                    //OBSOLETE
                    //If Store is Auchan remove everything after de last '/'
                    //if (_selectedMarket.StoreId == 1)
                    //{

                    //    selectedResultUrl = selectedResultUrl.Remove(selectedResultUrl.LastIndexOf("/"));
                    //    _selectedProductSearch = _newSearchResultList.Where(c => c.Url != null && c.Url.StartsWith(selectedResultUrl)).FirstOrDefault();
                    //}
                    //else
                    //{
                    //TODO TEST
                    _selectedProductSearch = _newSearchResultList.Where(c => c.Url != null && c.Url.StartsWith(selectedResultUrl)).FirstOrDefault();
                    //}
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }


                //Download selected product original image to media / temp
                Guid _guid = Guid.NewGuid();
                var sourceImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\Media\\Temp\\" + _guid.ToString();
                try
                {
                    _sourceImage = await _ProductsMatcher.GetOnlineImage(_selectedProductSearch.ImageUrl.Replace("https://", "http://"));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
                }
            }

            //For matching against markets that are not the selected one
            if (_selectedProductSearch != null && _sourceImage != null)
            {
                foreach (var _market in _markets)
                {
                    if (_market.StoreId != selectedStoreId)
                    {
                        LisieStores.Extensibility.IMarketFetcher _marketFetcher = (LisieStores.Extensibility.IMarketFetcher)Activator.CreateInstance(_market.ClassType);
                        _marketFetcher.StoreId = _market.StoreId;
                        _marketFetcher.StoreName = _market.StoreName;
                        _marketFetcher.StoreColor = _market.StoreColor;
                        List<LisieStores.Extensibility.ProductSearchResult> _searchResults = await _marketFetcher.GetSearchResults(search);
                        //Add store separator
                        _newSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
                        {
                            StoreId = _market.StoreId,
                            StoreName = _market.StoreName,
                            StoreColor = _market.StoreColor,
                            IsSeperator = true,
                            SeparatorTitle = _market.StoreName + " (" + _searchResults.Count + ")"
                        });
                        //Add search results sorted by matching
                        _newSearchResultList.AddRange(await _ProductsMatcher.GetProductsStoreSimilatiry(_selectedProductSearch, _searchResults, _sourceImage));

                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, _newSearchResultList);
        }

        //public async Task<List<ProductSearchResult>> GetProductsStoreSimilatiry(ProductSearchResult selectedProductSearch, List<Objects.ProductSearchResult> storeProductSearchResult, Bitmap selectedProductImage)
        //{
        //    List<ProductSearchMatchResult> _ProductSearchMatchResult = new List<ProductSearchMatchResult>();
        //    foreach (var _currentProductsResult in storeProductSearchResult)
        //    {
        //        double percentageEquality = GetProductsMatchingPercentage(selectedProductSearch, _currentProductsResult);
        //        double percentageTextEquality = GetProductsTextMatchingPercentage(selectedProductSearch, _currentProductsResult);
        //        double percentageTextTogetherEquality = GetProductsTextTogetherMatchingPercentage(selectedProductSearch, _currentProductsResult);

        //        int imageSimilarity = await CalculateImageSimilatiry(selectedProductImage, selectedProductSearch.ImageUrl);
        //        _ProductSearchMatchResult.Add(new ProductSearchMatchResult
        //        {
        //            Name = _currentProductsResult.Name,
        //            Brand = _currentProductsResult.Brand,
        //            Weight = _currentProductsResult.Weight,
        //            Price = _currentProductsResult.Price,
        //            PriceWeight = _currentProductsResult.PriceWeight,
        //            Store = _currentProductsResult.Store,
        //            ImageUrl = _currentProductsResult.ImageUrl,
        //            EqualsPercentage = percentageEquality,
        //            TextEqualsPercentage = percentageTextEquality,
        //            TextTogetherEqualsPercentage = percentageTextTogetherEquality,
        //            ImageTextEqualsPercentage = (percentageTextEquality * 12 + imageSimilarity) / 2,
        //            Url = _currentProductsResult.Url,
        //            Category = _currentProductsResult.Category,
        //            PriceLiteral = _currentProductsResult.PriceLiteral,
        //            PriceWeightLiteral = _currentProductsResult.PriceWeightLiteral,
        //            ImageEqualsPercentage = imageSimilarity

        //        });

        //    }
        //    _ProductSearchMatchResult = _ProductSearchMatchResult.OrderByDescending(c => c.ImageTextEqualsPercentage).ToList();
        //    return (from c in _ProductSearchMatchResult
        //            orderby c.ImageTextEqualsPercentage descending
        //            select new ProductSearchResult
        //            {
        //                Name = c.Name,
        //                Brand = c.Brand,
        //                Price = c.Price,
        //                PriceWeight = c.PriceWeight,
        //                Store = c.Store,
        //                Url = c.Url,
        //                Weight = c.Weight,
        //                ImageUrl = c.ImageUrl,
        //                IsSeperator = false,
        //                Category = c.Category,
        //                PriceLiteral = c.PriceLiteral,
        //                PriceWeightLiteral = c.PriceWeightLiteral,
        //            }).ToList();
        //}
        //public double GetProductsMatchingPercentage(ProductSearchResult productOriginal, ProductSearchResult productToCompare)
        //{
        //    double nameEquality = CalculateSimilarity(productOriginal.Name.ToLower(), productToCompare.Name.ToLower());
        //    double brandEquality = CalculateSimilarity(productOriginal.Brand.ToLower(), productToCompare.Brand.ToLower());
        //    double weightEquality = CalculateSimilarity(productOriginal.Weight.ToLower(), productToCompare.Weight.ToLower());
        //    double weightPriceEquality = CalculateSimilarity(productOriginal.PriceWeight.ToLower(), productToCompare.PriceWeight.ToLower());

        //    string stringTogetherOriginal = productOriginal.Name.ToLower() + " " + (productOriginal.Brand.ToLower() + " " + productOriginal.Weight.ToLower());
        //    string stringTogetherToCompare = productToCompare.Name.ToLower() + " " + (productToCompare.Brand.ToLower() + " " + productToCompare.Weight.ToLower());

        //    double stringTogetherEquality = CalculateSimilarity(stringTogetherOriginal, stringTogetherToCompare);

        //    double finalPercentage = (productOriginal.Store == "Jumbo") ?
        //        //(nameEquality + brandEquality) / 2 :
        //        //(nameEquality + brandEquality + weightEquality) / 3;
        //        (nameEquality * 100 + brandEquality * 100 + CalculateSimilarity(productOriginal.Name.ToLower(), productToCompare.Weight.ToLower()) * 100) / 3 :
        //        (nameEquality * 100 + brandEquality * 100 + weightEquality * 100) / 3;



        //    //double finalPercentage = (nameEquality + brandEquality + weightEquality + weightPriceEquality) / 4;

        //    double finalFinalPercentage = (finalPercentage + stringTogetherEquality * 100) / 2;

        //    return finalFinalPercentage;
        //}

        //public double GetProductsTextMatchingPercentage(ProductSearchResult productOriginal, ProductSearchResult productToCompare)
        //{
        //    double nameEquality = CalculateSimilarity(productOriginal.Name.ToLower(), productToCompare.Name.ToLower());
        //    double brandEquality = CalculateSimilarity(productOriginal.Brand.ToLower(), productToCompare.Brand.ToLower());
        //    double weightEquality = CalculateSimilarity(productOriginal.Weight.ToLower(), productToCompare.Weight.ToLower());
        //    double weightPriceEquality = CalculateSimilarity(productOriginal.PriceWeight.ToLower(), productToCompare.PriceWeight.ToLower());

        //    double finalPercentage = (productOriginal.Store == "Jumbo") ?
        //         (nameEquality * 100 + brandEquality * 100 + CalculateSimilarity(productOriginal.Name.ToLower(), productToCompare.Weight.ToLower()) * 100) / 3 :
        //         (nameEquality * 100 + brandEquality * 100 + weightEquality * 100) / 3;

        //    return finalPercentage;
        //}

        //public double GetProductsTextTogetherMatchingPercentage(ProductSearchResult productOriginal, ProductSearchResult productToCompare)
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

        ///// <summary>
        ///// Calculate percentage similarity of two strings
        ///// <param name="source">Source String to Compare with</param>
        ///// <param name="target">Targeted String to Compare</param>
        ///// <returns>Return Similarity between two strings from 0 to 1.0</returns>
        ///// </summary>
        //double CalculateSimilarity(string source, string target)
        //{
        //    if ((source == null) || (target == null)) return 0.0;
        //    if ((source.Length == 0) || (target.Length == 0)) return 0.0;
        //    if (source == target) return 1.0;

        //    int stepsToSame = ComputeLevenshteinDistance(source, target);
        //    return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
        //}

        //async Task<int> CalculateImageSimilatiry(Bitmap sourceDiskPath, string targetUrl)
        //{
        //    //List<bool> iHash1 = GetHash(new Bitmap(@"D:\My Creative Projects\SpiroStockManagement Web\Images and Barcodes for testing\chocapic 375g jumbo.jpg"));
        //    //List<bool> iHash2 = GetHash(new Bitmap(@"D:\My Creative Projects\SpiroStockManagement Web\Images and Barcodes for testing\chocapic chococruh 410g continente.jpg"));

        //    //fix https image 
        //    targetUrl = targetUrl.Replace("https://", "http://");

        //    List<bool> iHash1 = GetHash(sourceDiskPath);
        //    List<bool> iHash2 = GetHash(await GetOnlineImage(targetUrl));

        //    //determine the number of equal pixel (x of 256)
        //    int equalElements = iHash1.Zip(iHash2, (i, j) => i == j).Count(eq => eq);

        //    return equalElements;
        //}

        //public static List<bool> GetHash(Bitmap bmpSource)
        //{
        //    List<bool> lResult = new List<bool>();
        //    //create new image with 16x16 pixel
        //    Bitmap bmpMin = new Bitmap(bmpSource, new Size(16, 16));
        //    for (int j = 0; j < bmpMin.Height; j++)
        //    {
        //        for (int i = 0; i < bmpMin.Width; i++)
        //        {
        //            //reduce colors to true / false                
        //            lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
        //        }
        //    }
        //    return lResult;
        //}

        //public async Task<Bitmap> GetOnlineImage(string url)
        //{
        //    var images = new List<Bitmap>();
        //    using (var client = new HttpClient())
        //    {
        //        var response = await client.GetAsync(url);
        //        //var bitmap = new Bitmap(
        //        if (response != null && response.StatusCode == HttpStatusCode.OK)
        //        {
        //            using (var stream = await response.Content.ReadAsStreamAsync())
        //            {
        //                var memStream = new MemoryStream();
        //                await stream.CopyToAsync(memStream);
        //                memStream.Position = 0;
        //                return new Bitmap(memStream);
        //            }
        //        }
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Returns the number of steps required to transform the source string
        ///// into the target string.
        ///// </summary>
        //int ComputeLevenshteinDistance(string source, string target)
        //{
        //    if ((source == null) || (target == null)) return 0;
        //    if ((source.Length == 0) || (target.Length == 0)) return 0;
        //    if (source == target) return source.Length;

        //    int sourceWordCount = source.Length;
        //    int targetWordCount = target.Length;

        //    // Step 1
        //    if (sourceWordCount == 0)
        //        return targetWordCount;

        //    if (targetWordCount == 0)
        //        return sourceWordCount;

        //    int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

        //    // Step 2
        //    for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
        //    for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

        //    for (int i = 1; i <= sourceWordCount; i++)
        //    {
        //        for (int j = 1; j <= targetWordCount; j++)
        //        {
        //            // Step 3
        //            int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

        //            // Step 4
        //            distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
        //        }
        //    }

        //    return distance[sourceWordCount, targetWordCount];
        //}

        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> ProductAutoMatchDone(int productId)
        {
            List<LisieStores.Extensibility.Market> _markets = SpiroWeb.Helpers.Extensibility.GetStoreFetchers();
            int _addedToProductsAutoMatched = 0;
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                foreach (var _market in _markets.OrderBy(c => c.StoreId))
                {
                    var _StoreProduct = db.StoreProducts.Where(c => c.ProductId == productId && c.StoreId == _market.StoreId).FirstOrDefault();
                    if (_StoreProduct == null)
                    {
                        var _ProductAutoMatchedInStore = db.ProductsAutoMatched.Where(c => c.ProductId == productId && c.StoreId == _market.StoreId).FirstOrDefault();
                        if (_ProductAutoMatchedInStore == null)
                        {
                            db.ProductsAutoMatched.Add(new ProductsAutoMatched
                            {
                                MatchType = "manual",
                                ProductId = productId,
                                StoreId = _market.StoreId,
                                CreateDate = DateTime.Now
                            });
                            _addedToProductsAutoMatched++;
                        }
                    }
                }
                if (_addedToProductsAutoMatched > 0)
                    db.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.OK, _addedToProductsAutoMatched);
        }

        //[HttpGet]
        //[HttpPost]
        //public async Task<HttpResponseMessage> ProductStoreAutoMatchDone(int productId, int storeId, double ImageTogetherTextEqualsPercentage, double Last2Avg, double, )
        //{
        //    List<LisieStores.Extensibility.Market> _markets = SpiroWeb.Helpers.Extensibility.GetStoreFetchers();
        //    int _addedToProductsAutoMatched = 0;
        //    using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
        //    {

        //        var _ProductAutoMatchedInStore = db.ProductsAutoMatched.Where(c => c.ProductId == productId && c.StoreId == storeId).FirstOrDefault();
        //        if (_ProductAutoMatchedInStore == null)
        //        {
        //            db.ProductsAutoMatched.Add(new ProductsAutoMatched
        //            {
        //                MatchType = "manual",
        //                ProductId = productId,
        //                StoreId = storeId,
        //                CreateDate = DateTime.Now
        //            });
        //            _addedToProductsAutoMatched++;
        //        }
        //        if (_addedToProductsAutoMatched > 0)
        //            db.SaveChanges();
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, _addedToProductsAutoMatched);
        //}

        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> ProductStoreAutoMatchDone(ProductStoreAutoMatchDoneModel productStoreAutoMatchDone)
        {
            List<LisieStores.Extensibility.Market> _markets = SpiroWeb.Helpers.Extensibility.GetStoreFetchers();
            int _addedToProductsAutoMatched = 0;
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {

                var _ProductAutoMatchedInStore = db.ProductsAutoMatched.Where(c => c.ProductId == productStoreAutoMatchDone.ProductId && c.StoreId == productStoreAutoMatchDone.StoreId).FirstOrDefault();
                if (_ProductAutoMatchedInStore == null)
                {
                    db.ProductsAutoMatched.Add(new ProductsAutoMatched
                    {
                        MatchType = "manual",
                        ProductId = productStoreAutoMatchDone.ProductId,
                        StoreId = productStoreAutoMatchDone.StoreId,
                        ImagesTogetherPercentage = Math.Round(productStoreAutoMatchDone.MatchResult.ImagesTogetherPercentage, 2),
                        ImageTextEqualsPercentage = Math.Round(productStoreAutoMatchDone.MatchResult.ImageTextEqualsPercentage, 2),
                        ImageTogetherTextEqualsPercentage = Math.Round(productStoreAutoMatchDone.MatchResult.ImageTogetherTextEqualsPercentage, 2),
                        Last2Avg = Math.Round(productStoreAutoMatchDone.MatchResult.Last2Avg, 2),
                        TxPoTxTogetherPo = Math.Round(productStoreAutoMatchDone.MatchResult.TxPoTxTogetherPo, 2),
                        SortWeight = productStoreAutoMatchDone.MatchResult.SortedWeight,
                        CreateDate = DateTime.Now
                    });
                    _addedToProductsAutoMatched++;
                }
                if (_addedToProductsAutoMatched > 0)
                    db.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.OK, _addedToProductsAutoMatched);
        }

        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> MatchProduct(int productId, int storeId, string url)
        {
            List<ProductSearchMatchResult> _ProductSearchMatchResultList = new List<ProductSearchMatchResult>();
            LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = await Helpers.Extensibility.GetProductStoreMetadata(storeId, url);

            if (_ProductSearchResult != null)
            {
                bool _sucess = Managers.ProductsManager.CreateOrUpdateStoreProductNew(_ProductSearchResult, productId, "9ff8224f-17cf-49fb-b555-05779a13eb40", storeId);
                return Request.CreateResponse(HttpStatusCode.OK, _sucess);
            }
            return Request.CreateResponse(HttpStatusCode.OK, false);
        }
    }

    public class ProductStoreAutoMatchDoneModel
    {
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public ProductSearchMatchResult MatchResult { get; set; }
    }
}
