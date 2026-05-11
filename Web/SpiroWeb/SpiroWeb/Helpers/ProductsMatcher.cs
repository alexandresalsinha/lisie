using SpiroWeb.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpiroWeb.Helpers
{
    public class ProductsMatcher
    {
        public async Task<string> FetchOnlineSearchResults(String url)
        {
            var client = new HttpClient();

            //Fake a Browser Request
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");


            // Get the response.
            //HttpResponseMessage response = await client.GetStringAsync("http://www.jumbo.pt/Frontoffice/ContentPages/CatalogSearch.aspx?Q=" + HttpUtility.UrlEncode(searchquery));
            //HttpResponseMessage response = await client.GetStringAsync("http://www.jumbo.pt");
            //string response = await client.GetStringAsync("http://www.google.pt");
            //string response = await client.GetStringAsync("http://www.jumbo.pt/Frontoffice/ContentPages/CatalogSearch.aspx?Q=" + searchquery);

            string response = await client.GetStringAsync(url);

            return response;
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
                    StoreName = _currentProductsResult.StoreName,
                    StoreId = _currentProductsResult.StoreId,
                    StoreColor = _currentProductsResult.StoreColor,
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
                        StoreId = c.StoreId,
                        StoreName = c.StoreName,
                        StoreColor = c.StoreColor,
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

            double finalPercentage = (productOriginal.StoreId == 1) ?
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

            double finalPercentage = (productOriginal.StoreId == 1) ?
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