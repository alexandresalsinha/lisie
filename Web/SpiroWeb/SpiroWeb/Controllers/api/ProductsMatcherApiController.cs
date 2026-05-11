using ClassLibrary1;
using SpiroWeb.Helpers;
using SpiroWeb.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SpiroWeb.Controllers
{
    public class ProductsMatcherApiController : ApiController
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
            OnlineProducts _OnlineProducts = new OnlineProducts();

            //List<Objects.ProductSearchResult> _ProductSearchResultList = new List<ProductSearchResult>();
            List<LisieStores.Extensibility.ProductSearchResult> _NewProductSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();
            //List<ProductSearchMatchResult> _ProductSearchMatchResultList = new List<ProductSearchMatchResult>();

            List<LisieStores.Extensibility.ProductSearchResult> _jumboProductSearchResultList = await _OnlineProducts.GetJumboOnlineProductSearchResults(search);
            List<LisieStores.Extensibility.ProductSearchResult> _continenteProductSearchResultList = await _OnlineProducts.GetContinenteOnlineProductSearchResultsHeroku(search);
            List<LisieStores.Extensibility.ProductSearchResult> _pingoDoceProductSearchResultList = await _OnlineProducts.GetPingoDoceOnlineProductSearchResults(search);

            LisieStores.Extensibility.ProductSearchResult _selectedProductSearch = null;

            switch (selectedStoreId)
            {
                case 1: //Jumbo 
                    //_NewProductSearchResultList.AddRange(_jumboProductSearchResultList);
                    selectedResultUrl = selectedResultUrl.Remove(selectedResultUrl.LastIndexOf("/"));
                    _selectedProductSearch = _jumboProductSearchResultList.Where(c => c.Url.StartsWith(selectedResultUrl)).FirstOrDefault();
                    break;
                case 2:  //Continente

                    //_NewProductSearchResultList.AddRange(_continenteProductSearchResultList);
                    _selectedProductSearch = _continenteProductSearchResultList.Where(c => c.Url.Equals(selectedResultUrl)).FirstOrDefault();
                    break;
                case 3: //Pingo Doce
                        //_NewProductSearchResultList.AddRange(_pingoDoceProductSearchResultList);
                    _selectedProductSearch = _pingoDoceProductSearchResultList.Where(c => c.Url.Equals(selectedResultUrl)).FirstOrDefault();
                    break;
                default:
                    break;
            }

            //Download original image to media / temp
            Guid _guid = Guid.NewGuid();
            var sourceImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\Media\\Temp\\" + _guid.ToString();
            Bitmap _sourceImage;
            try
            {
                //_sourceImage = await GetOnlineImage(_selectedProductSearch.ImageUrl);
                //fix https in imageUrl
                _sourceImage = await GetOnlineImage(_selectedProductSearch.ImageUrl.Replace("https://", "http://"));
                //_sourceImage.Save(sourceImagePath);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }

            List<LisieStores.Extensibility.ProductSearchResult> _newJumboProductSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();
            List<LisieStores.Extensibility.ProductSearchResult> _newContinenteProductSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();
            List<LisieStores.Extensibility.ProductSearchResult> _newPingoDoceProductSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();

            switch (selectedStoreId)
            {
                case 1: //Jumbo 
                    //Get from Continente
                    if (_continenteProductSearchResultList.Count > 0)
                        _newContinenteProductSearchResultList.AddRange(await GetProductsStoreSimilatiry(_selectedProductSearch, _continenteProductSearchResultList, _sourceImage));
                    //Get from Pingo Doce
                    if (_pingoDoceProductSearchResultList.Count > 0)
                        _newPingoDoceProductSearchResultList.AddRange(await GetProductsStoreSimilatiry(_selectedProductSearch, _pingoDoceProductSearchResultList, _sourceImage));
                    break;
                case 2:  //Continente
                    //Get from Jumbo
                    if (_jumboProductSearchResultList.Count > 0)
                        _newJumboProductSearchResultList.AddRange(await GetProductsStoreSimilatiry(_selectedProductSearch, _jumboProductSearchResultList, _sourceImage));
                    //Get from Pingo Doce
                    if (_pingoDoceProductSearchResultList.Count > 0)
                        _newPingoDoceProductSearchResultList.AddRange(await GetProductsStoreSimilatiry(_selectedProductSearch, _pingoDoceProductSearchResultList, _sourceImage));
                    break;
                case 3: //Pingo Doce
                    //Get from Jumbo
                    if (_jumboProductSearchResultList.Count > 0)
                        _newJumboProductSearchResultList.AddRange(await GetProductsStoreSimilatiry(_selectedProductSearch, _jumboProductSearchResultList, _sourceImage));
                    //Get from Continente
                    if (_continenteProductSearchResultList.Count > 0)
                        _newContinenteProductSearchResultList.AddRange(await GetProductsStoreSimilatiry(_selectedProductSearch, _continenteProductSearchResultList, _sourceImage));
                    break;
                default:
                    break;
            }


            _NewProductSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
            {
                IsSeperator = true,
                SeparatorTitle = "Jumbo (" + _jumboProductSearchResultList.Count + ")"
            });

            _NewProductSearchResultList.AddRange((selectedStoreId == 1) ? _jumboProductSearchResultList : _newJumboProductSearchResultList);

            _NewProductSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
            {
                IsSeperator = true,
                SeparatorTitle = "Continente (" + _continenteProductSearchResultList.Count + ")"
            });

            _NewProductSearchResultList.AddRange((selectedStoreId == 2) ? _continenteProductSearchResultList : _newContinenteProductSearchResultList);

            _NewProductSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
            {
                IsSeperator = true,
                SeparatorTitle = "Pingo Doce (" + _pingoDoceProductSearchResultList.Count + ")"
            });

            _NewProductSearchResultList.AddRange((selectedStoreId == 3) ? _pingoDoceProductSearchResultList : _newPingoDoceProductSearchResultList);

            return Request.CreateResponse(HttpStatusCode.OK, _NewProductSearchResultList);
        }

        public async Task<List<LisieStores.Extensibility.ProductSearchResult>> GetProductsStoreSimilatiry(LisieStores.Extensibility.ProductSearchResult selectedProductSearch, List<LisieStores.Extensibility.ProductSearchResult> storeProductSearchResult, Bitmap selectedProductImage)
        {
            List<ProductSearchMatchResult> _ProductSearchMatchResult = new List<ProductSearchMatchResult>();
            foreach (var _currentProductsResult in storeProductSearchResult)
            {
                double percentageEquality = GetProductsMatchingPercentage(selectedProductSearch, _currentProductsResult);
                double percentageTextEquality = GetProductsTextMatchingPercentage(selectedProductSearch, _currentProductsResult);
                double percentageTextTogetherEquality = GetProductsTextTogetherMatchingPercentage(selectedProductSearch, _currentProductsResult);

                int imageSimilarity = await CalculateImageSimilatiry(selectedProductImage, selectedProductSearch.ImageUrl);
                _ProductSearchMatchResult.Add(new ProductSearchMatchResult
                {
                    Name = _currentProductsResult.Name,
                    Brand = _currentProductsResult.Brand,
                    Weight = _currentProductsResult.Weight,
                    Price = _currentProductsResult.Price,
                    PriceWeight = _currentProductsResult.PriceWeight,
                    //Store = _currentProductsResult.Store,
                    ImageUrl = _currentProductsResult.ImageUrl,
                    EqualsPercentage = percentageEquality,
                    TextEqualsPercentage = percentageTextEquality,
                    TextTogetherEqualsPercentage = percentageTextTogetherEquality,
                    ImageTextEqualsPercentage = (percentageTextEquality * 12 + imageSimilarity) / 2,
                    Url = _currentProductsResult.Url,
                    Category = _currentProductsResult.Category,
                    PriceLiteral = _currentProductsResult.PriceLiteral,
                    PriceWeightLiteral = _currentProductsResult.PriceWeightLiteral,
                    ImageEqualsPercentage = imageSimilarity

                });

            }
            _ProductSearchMatchResult = _ProductSearchMatchResult.OrderByDescending(c => c.ImageTextEqualsPercentage).ToList();
            return (from c in _ProductSearchMatchResult
                    orderby c.ImageTextEqualsPercentage descending
                    select new LisieStores.Extensibility.ProductSearchResult
                    {
                        Name = c.Name,
                        Brand = c.Brand,
                        Price = c.Price,
                        PriceWeight = c.PriceWeight,
                        //Store = c.Store,
                        Url = c.Url,
                        Weight = c.Weight,
                        ImageUrl = c.ImageUrl,
                        IsSeperator = false,
                        Category = c.Category,
                        PriceLiteral = c.PriceLiteral,
                        PriceWeightLiteral = c.PriceWeightLiteral,
                    }).ToList();
        }
        public double GetProductsMatchingPercentage(LisieStores.Extensibility.ProductSearchResult productOriginal, LisieStores.Extensibility.ProductSearchResult productToCompare)
        {
            double nameEquality = CalculateSimilarity(productOriginal.Name.ToLower(), productToCompare.Name.ToLower());
            double brandEquality = CalculateSimilarity(productOriginal.Brand.ToLower(), productToCompare.Brand.ToLower());
            double weightEquality = CalculateSimilarity(productOriginal.Weight.ToLower(), productToCompare.Weight.ToLower());
            double weightPriceEquality = CalculateSimilarity(productOriginal.PriceWeight.ToLower(), productToCompare.PriceWeight.ToLower());

            string stringTogetherOriginal = productOriginal.Name.ToLower() + " " + (productOriginal.Brand.ToLower() + " " + productOriginal.Weight.ToLower());
            string stringTogetherToCompare = productToCompare.Name.ToLower() + " " + (productToCompare.Brand.ToLower() + " " + productToCompare.Weight.ToLower());

            double stringTogetherEquality = CalculateSimilarity(stringTogetherOriginal, stringTogetherToCompare);

            double finalPercentage = (productOriginal.StoreId == 1) ? //"Jumbo"
                                                                      //(nameEquality + brandEquality) / 2 :
                                                                      //(nameEquality + brandEquality + weightEquality) / 3;
                (nameEquality * 100 + brandEquality * 100 + CalculateSimilarity(productOriginal.Name.ToLower(), productToCompare.Weight.ToLower()) * 100) / 3 :
                (nameEquality * 100 + brandEquality * 100 + weightEquality * 100) / 3;



            //double finalPercentage = (nameEquality + brandEquality + weightEquality + weightPriceEquality) / 4;

            double finalFinalPercentage = (finalPercentage + stringTogetherEquality * 100) / 2;

            return finalFinalPercentage;
        }

        public double GetProductsTextMatchingPercentage(LisieStores.Extensibility.ProductSearchResult productOriginal, LisieStores.Extensibility.ProductSearchResult productToCompare)
        {
            double nameEquality = CalculateSimilarity(productOriginal.Name.ToLower(), productToCompare.Name.ToLower());
            double brandEquality = CalculateSimilarity(productOriginal.Brand.ToLower(), productToCompare.Brand.ToLower());
            double weightEquality = CalculateSimilarity(productOriginal.Weight.ToLower(), productToCompare.Weight.ToLower());
            double weightPriceEquality = CalculateSimilarity(productOriginal.PriceWeight.ToLower(), productToCompare.PriceWeight.ToLower());

            double finalPercentage = (productOriginal.StoreId == 1) ? //"Jumbo"
                 (nameEquality * 100 + brandEquality * 100 + CalculateSimilarity(productOriginal.Name.ToLower(), productToCompare.Weight.ToLower()) * 100) / 3 :
                 (nameEquality * 100 + brandEquality * 100 + weightEquality * 100) / 3;

            return finalPercentage;
        }

        public double GetProductsTextTogetherMatchingPercentage(LisieStores.Extensibility.ProductSearchResult productOriginal, LisieStores.Extensibility.ProductSearchResult productToCompare)
        {
            double nameEquality = CalculateSimilarity(productOriginal.Name.ToLower(), productToCompare.Name.ToLower());
            double brandEquality = CalculateSimilarity(productOriginal.Brand.ToLower(), productToCompare.Brand.ToLower());
            double weightEquality = CalculateSimilarity(productOriginal.Weight.ToLower(), productToCompare.Weight.ToLower());
            double weightPriceEquality = CalculateSimilarity(productOriginal.PriceWeight.ToLower(), productToCompare.PriceWeight.ToLower());

            string stringTogetherOriginal = productOriginal.Name.ToLower() + " " + (productOriginal.Brand.ToLower() + " " + productOriginal.Weight.ToLower());
            string stringTogetherToCompare = productToCompare.Name.ToLower() + " " + (productToCompare.Brand.ToLower() + " " + productToCompare.Weight.ToLower());

            double stringTogetherEquality = CalculateSimilarity(stringTogetherOriginal, stringTogetherToCompare);



            return stringTogetherEquality * 100;
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

        async Task<int> CalculateImageSimilatiry(Bitmap sourceDiskPath, string targetUrl)
        {
            //List<bool> iHash1 = GetHash(new Bitmap(@"D:\My Creative Projects\SpiroStockManagement Web\Images and Barcodes for testing\chocapic 375g jumbo.jpg"));
            //List<bool> iHash2 = GetHash(new Bitmap(@"D:\My Creative Projects\SpiroStockManagement Web\Images and Barcodes for testing\chocapic chococruh 410g continente.jpg"));

            //fix https image 
            targetUrl = targetUrl.Replace("https://", "http://");

            List<bool> iHash1 = GetHash(sourceDiskPath);
            List<bool> iHash2 = GetHash(await GetOnlineImage(targetUrl));

            //determine the number of equal pixel (x of 256)
            int equalElements = iHash1.Zip(iHash2, (i, j) => i == j).Count(eq => eq);

            return equalElements;
        }

        public static List<bool> GetHash(Bitmap bmpSource)
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

        public async Task<Bitmap> GetOnlineImage(string url)
        {
            var images = new List<Bitmap>();
            using (var client = new HttpClient())
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
    }
}
