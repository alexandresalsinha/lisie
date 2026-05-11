using ClassLibrary1;
using CsQuery;
using SpiroWeb.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace SpiroWeb.Helpers
{
    public class OnlineProducts
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




        public async Task<List<LisieStores.Extensibility.ProductSearchResult>> GetJumboOnlineProductSearchResults(string searchQuery, int pageNumber)
        {
            string _searchQuery = string.IsNullOrEmpty(searchQuery) ? "" : searchQuery;
            string _jumboSearchResultsHtml = await FetchOnlineSearchResults("http://www.jumbo.pt/Frontoffice/ContentPages/CatalogSearch.aspx?Q=" + searchQuery);

            List<LisieStores.Extensibility.ProductSearchResult> _productSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();

            CQ _jumboDom = _jumboSearchResultsHtml;
            CQ _produtos = _jumboDom[".produtoGrelha"];

            List<IDomObject> _productsList = _produtos.ToList();

            foreach (IDomObject _productResult in _productsList)
            {
                Console.WriteLine(_productResult.InnerHTML);

                CQ _productResultCQ = _productResult.InnerHTML;


                string _productUrl = _productResultCQ["a"].First().Attr("href");
                string _productName = _productResultCQ[".titProd"].First().Text().Replace("\n", "").Trim();
                string _productBrand = _productResultCQ[".titMarca"].First().Text().Replace("\n", "").Trim();
                string _productWeight = _productResultCQ[".gr"].First().Text().Replace("\n", "").Trim();
                string _productPrice = _productResultCQ[".preco"].First().Text().Replace("\n", "").Trim();
                string _productPriceWeight = _productResultCQ[".prodkg"].First().Text().Replace("\n", "").Trim();
                string _productImageUrl = _productResultCQ["img"].First().Attr("src");
                _productImageUrl = "http://www.jumbo.pt" + _productImageUrl;

                _productSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult { Name = _productName, Brand = _productBrand, Price = _productPrice, PriceWeight = _productPriceWeight, StoreName = "Jumbo", Url = _productUrl, Weight = _productWeight, ImageUrl = _productImageUrl });
            }

            return _productSearchResultList;
        }


        /// <summary>
        /// New Version of Jumbo.pt
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public async Task<List<LisieStores.Extensibility.ProductSearchResult>> GetJumboOnlineProductSearchResults(string searchQuery)
        {
            HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("https://www.jumbo.pt/Frontoffice/search/" + searchQuery);
            string _htmlResult = string.Empty;
            try
            {
                webReq.CookieContainer = new CookieContainer();
                webReq.Method = "GET";
                using (WebResponse response = webReq.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        _htmlResult = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
            }

            List<LisieStores.Extensibility.ProductSearchResult> _productSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();

            CQ _jumboDom = _htmlResult;
            CQ _produtos = _jumboDom[".product-item-border"];

            List<IDomObject> _productsList = _produtos.ToList();

            int _productCOunter = 1;
            foreach (IDomObject _productResult in _productsList)
            {
                System.Diagnostics.Debug.WriteLine(_productCOunter);
                ++_productCOunter;
                try
                {
                    CQ _productResultCQ = _productResult.InnerHTML;


                    string _productUrl = _productResultCQ["a"].First().Attr("href");
                    CQ _productName = _productResultCQ["a"];
                    CQ _productInfo = ((CsQuery.Implementation.DomElement)(_productName[1])).InnerHTML;

                    //string _name = ((CsQuery.Implementation.DomElement)(_productInfo[0])).InnerHTML;
                    string _name = _productInfo.Text();
                    _name = _name.Replace("\n                            ", "");
                    _name = _name.Replace(" \n\n                    ", "");

                    string _productBrand = _productResultCQ[".product-item-brand"].First().Text().Replace("\n", "").Trim();
                    //string _productWeight = _name;
                    string _productWeight = string.Empty;
                    CQ _productPriceCQ = _productResultCQ[".product-item-price "].First();
                    string _productPrice = _productPriceCQ[0].FirstChild.NodeValue.Replace("\n", "").Trim();

                    string _productPriceWeight = _productResultCQ[".product-item-quantity-price"].First().Text().Replace("\n", "").Trim();

                    string _productImageUrl = _productResultCQ[".product-item-image"].First()["img"].Attr("src");

                    if (!_productUrl.Equals("javascript:void(0);"))
                        _productSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult { Name = _name, Brand = _productBrand, Price = _productPrice, PriceWeight = _productPriceWeight, StoreName = "Jumbo", StoreId = 1, Url = _productUrl, Weight = _productWeight, ImageUrl = _productImageUrl });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return _productSearchResultList;

        }

        public async Task<LisieStores.Extensibility.ProductSearchResult> GetJumboProductMetadata(string url)
        {
            LisieStores.Extensibility.ProductSearchResult _productSearchResult = new LisieStores.Extensibility.ProductSearchResult();

            if (url.IndexOf("https://www.jumbo.pt") == 0)
            {
                url = url.Remove(0, "https://www.jumbo.pt".Length);
            }
            HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("https://www.jumbo.pt" + url);
            string _htmlResult = string.Empty;
            try
            {
                webReq.CookieContainer = new CookieContainer();
                webReq.Method = "GET";
                using (WebResponse response = webReq.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        _htmlResult = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            CQ _jumboDom = _htmlResult;
            CQ _category = _jumboDom[".breadcrumb li:last-child"].Prev().Prev();
            CQ _divRow = _jumboDom[".row"];
            CQ _nameAndWeight = _divRow["h1"];
            CQ _brand = _divRow[".rating"].Next();
            CQ _priceWeight = _divRow[".product-item-quantity-price"].First();
            CQ _image = _divRow["#normalImage"];
            CQ _price = _divRow[".item-price"];

            _productSearchResult.Name = _nameAndWeight.Text();
            //_productSearchResult.Weight = _nameAndWeight.Text();
            string[] _nameAndWeightSplitted = _nameAndWeight.Text().Trim().Split(' ');
            _productSearchResult.Weight = _nameAndWeightSplitted[_nameAndWeightSplitted.Length - 1];
            _productSearchResult.Brand = _brand.Text();
            _productSearchResult.Category = _category.Text();
            _productSearchResult.PriceWeight = _priceWeight.Text();
            _productSearchResult.ImageUrl = _image.Attr("href");
            _productSearchResult.StoreName = "Jumbo";
            _productSearchResult.StoreId = 1;
            _productSearchResult.Price = _price.Text();
            _productSearchResult.Url = url;

            //foreach (IDomObject _productResult in _productsList)
            //{
            //    Console.WriteLine(_productResult.InnerHTML);

            //    CQ _productResultCQ = _productResult.InnerHTML;


            //    string _productUrl = _productResultCQ["a"].First().Attr("href");
            //    CQ _productName = _productResultCQ["a"];
            //    CQ _productInfo = ((CsQuery.Implementation.DomElement)(_productName[1])).InnerHTML;

            //    string _name = ((CsQuery.Implementation.DomElement)(_productInfo[0])).InnerHTML;

            //    string _productBrand = _productResultCQ[".product-item-brand"].First().Text().Replace("\n", "").Trim();
            //    //string _productWeight = _productResultCQ[".gr"].First().Text().Replace("\n", "").Trim();
            //    string _productWeight = _name;
            //    //string _productPrice = _productResultCQ[".product-item-price "].First().Text().Replace("\n", "").Trim();
            //    CQ _productPriceCQ = _productResultCQ[".product-item-price "].First();
            //    string _productPrice = _productPriceCQ[0].FirstChild.NodeValue.Replace("\n", "").Trim();

            //    string _productPriceWeight = _productResultCQ[".product-item-quantity-price"].First().Text().Replace("\n", "").Trim();

            //    //CQ _productImageUrl = _productResultCQ[".product-item-image hidden-print"].Attr("style");

            //    string _productImageUrl = _productResultCQ[".product-item-image"].First()["img"].Attr("src");

            //    //_productSearchResultList.Add(new ProductSearchResult { Name = _name, Brand = _productBrand, Price = _productPrice, PriceWeight = _productPriceWeight, Store = "Jumbo", Url = _productUrl, Weight = _productWeight, ImageUrl = _productImageUrl });
            //}

            //Fif for https image url, pass to http - maybe for the future
            //_productSearchResult.ImageUrl = _productSearchResult.ImageUrl.Replace("https://", "http://");

            return _productSearchResult;
        }


        #region Continente


        public async Task<List<LisieStores.Extensibility.ProductSearchResult>> GetContinenteOnlineProductSearchResults(string searchQuery)
        {
            string _searchQuery = string.IsNullOrEmpty(searchQuery) ? "" : searchQuery;
            //string _jumboSearchResultsHtml = await FetchOnlineSearchResults("http://www.continente.pt/pt-pt/public/Pages/searchresults.aspx?k=" + searchQuery);

            string _jumboSearchResultsHtml = string.Empty;

            //TODO - use chromium to fetch 40 results or page 2
            HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("https://www.continente.pt/pt-pt/public/Pages/searchresults.aspx?k=" + searchQuery + "#/?pl=40");
            webReq.Headers.Add("Cookie", " searchRefiner=%7B%22colgate%22%3A%7B%221579225697648%22%3A%5B%5D%2C%221579226313940%22%3A%5B%5D%7D%7D; f5_cspm=1234; f5avrbbbbbbbbbbbbbbbb=BODDLGCDLHCEDHDAJOILEDLDBMKIELEONMDCPAPNBMJOKADKCJJDNMEDNJEJLOOBPALDKBFFNEIFLJNCIMLAGMPIHBCJLFICIPOIAOFBKOPEOCGPODKMAPANJEGFCFHM; __cfduid=d0d4ff28474c8298b5da9f492b0cba3461553732474; AnonymousBasket=AAEAAAD/////AQAAAAAAAAAGAQAAACQxYmFiNzEwMS01OGQzLTQ4Y2YtYTZjMC0wNjBkNGI0YjkwY2QL; cPrompt_useCookies=1; byside_webcare_tuid=z1wfq9x4eokpq8nv0y182v93xcs53q6duh32y4sww3ub43r7hp; GCLB=CMi317Gl18_SNQ; rbzid=DIrAi0fb9Ihhiekaxh71UR4FGs2663S2iFVOEOq5wDdZyYUsH2V4c0Gb2YMczBMmtghp7JVVnVObNFtoz93W/aa+vZADJiD1WT/aL5V+w5bS6nekrZwAkTY5wUy66m1bElvcPaKma8oZ4sTS6/ZzQX2MKNh/r+wREyv3gLKGiJ6nlRoocnLyLxTgmYkvVz92QZTEjZuLSes71qfgAzKKAmPYuY5lcedMuDPs0Ll9DwdGWQRTfT4fwibP4Bf5Jo0S; rbzsessionid=1c19619bee280503da1abd35e703f714; __CommerceAnonymousShopper_ef77e72d-62b9-4b0f-8113-d111c9d6d7ce_Internet=024otpd4nPqeKPB0OBv4PY7NQ==PH5gcBXUA6irDZx2C4fDuDoUtPnABH5cx4xjEW1Fn3K5OSXLrTcUwKFWbrPpTEF4ngShLfVy3/nPwhlo2MpPWQb4v/9a1bVEnHzxVKIXS5WMhKpzhWNmR65Ek3SxCL+X6Xgi7+81u5rbvT+xBPdmrUu5Okc1DLmd/cckWfoldHWHdnGSJjFU6QdUZuMDM6t7e0ofQeiO/OIRJQ/furPRQA==; ASP.NET_SessionId=igyiw245ds2unj45gwrjp4v2; MSCSProfile=287001FD2674671CA5C152570F4E427711F73EFDEFAA23CC40CAC9E0E5A2FD5EE20AA5E1620C745E95D8748464A381F8E6F8C57B68311D4CFE81F97779FDDCC478A953628CD5460787DBB87CD9C42BCFB925FB496DA9D63BC1A225FE0005DC3C323389B841E039614C839288156F92575E862FE859EF35C6FFB2F21801AF2775; cpup=2; f5avrbbbbbbbbbbbbbbbb=MHILGJEIDPGNOOJGCBLMCHOGCNEIKECMNJHKGILACEDGPOCKDDLONHMCBHKPEABIPBFDHKNKPEGGHFOAGOMAMPCOHBFNDLGKJGIPODAAIDHHBJMJNAOKHLNJCHIHIJKH; CampaignHistory=717292,705203,719233,482264,717294,719233,705203,703639");
            webReq.Headers.Add("Cache-Control", "no-cache");
            webReq.Host = "www.continente.pt";
            webReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36";
            webReq.Accept = "*/*";
            //webReq.ServicePoint.ProtocolVersion\
            webReq.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateRemoteCertificate);

            try
            {
                webReq.CookieContainer = new CookieContainer();
                webReq.Method = "GET";
                using (WebResponse response = webReq.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        //_htmlResult = reader.ReadToEnd();
                        _jumboSearchResultsHtml = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            List<LisieStores.Extensibility.ProductSearchResult> _productSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();

            CQ _jumboDom = _jumboSearchResultsHtml;
            CQ _produtos = _jumboDom[".productItem"];

            List<IDomObject> _productsList = _produtos.ToList();

            foreach (IDomObject _productResult in _productsList)
            {
                Console.WriteLine(_productResult.InnerHTML);

                CQ _productResultCQ = _productResult.InnerHTML;

                string _productUrl = _productResultCQ["a"].First().Attr("href");
                string _productName = _productResultCQ[".title"].First().Text().Replace("\n", "").Trim();
                string _productBrand = _productResultCQ[".type"].First().Text().Replace("\n", "").Trim();
                //string _productPrice = _productResultCQ[".priceFirstRow"].First().Text().Replace("\n", "").Trim();

                string _productPrice = _productResultCQ[".priceFirstRow"].Text().Trim();
                _productPrice = _productPrice.Substring(0, _productPrice.IndexOf('\n'));

                string _productWeight = _productResultCQ[".subTitle"].Text();

                float _productPriceLiteral = 0;
                float _productPriceWeightLiteral = 0;

                try
                {
                    _productPriceLiteral = float.Parse(_productResultCQ[".OriginalListPrice"].Val().Replace('.', ','));
                    //_productPriceWeightLiteral = float.Parse(_productResultCQ[".PriceCapacityRatio"].Val().Replace('.', ','));
                }
                catch (Exception)
                {
                    //throw;
                }

                string _productPriceWeight = _productResultCQ[".priceSecondRow"].Text().Replace("\n", "").Replace(" ", "");
                string _productImageUrl = _productResultCQ["img"].Attr("data-original");
                //_productImageUrl = "http://www.jumbo.pt" + _productImageUrl;


                _productSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
                {
                    Name = _productName,
                    Brand = _productBrand,
                    Price = _productPrice,
                    PriceWeight = _productPriceWeight,
                    StoreName = "Continente",
                    StoreId = 2,
                    Url = _productUrl.Replace("https://www.continente.pt", ""),
                    Weight = _productWeight,
                    ImageUrl = _productImageUrl,
                    PriceLiteral = _productPriceLiteral,
                    PriceWeightLiteral = _productPriceWeightLiteral

                });
            }
            return _productSearchResultList;

        }

        public async Task<List<ProductSearchResult>> GetContinenteOnlineProductSearchResultsMyPc(string searchQuery)
        {
            string _searchQuery = string.IsNullOrEmpty(searchQuery) ? "" : searchQuery;
            string _jumboSearchResultsHtml = string.Empty;

            //TODO - use chromium to fetch 40 results or page 2
            HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("http://050d135e.ngrok.io/getContinenteSearchResultsHtml/" + searchQuery + "/40");
            webReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36";
            webReq.Accept = "*/*";
            //webReq.ServicePoint.ProtocolVersion\
            //webReq.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateRemoteCertificate);

            try
            {
                webReq.CookieContainer = new CookieContainer();
                webReq.Method = "GET";
                using (WebResponse response = webReq.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        //_htmlResult = reader.ReadToEnd();
                        _jumboSearchResultsHtml = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            List<ProductSearchResult> _productSearchResultList = new List<ProductSearchResult>();

            CQ _jumboDom = _jumboSearchResultsHtml;
            CQ _produtos = _jumboDom[".productItem"];

            List<IDomObject> _productsList = _produtos.ToList();

            foreach (IDomObject _productResult in _productsList)
            {
                Console.WriteLine(_productResult.InnerHTML);

                CQ _productResultCQ = _productResult.InnerHTML;

                string _productUrl = _productResultCQ["a"].First().Attr("href");
                string _productName = _productResultCQ[".title"].First().Text().Replace("\n", "").Trim();
                string _productBrand = _productResultCQ[".type"].First().Text().Replace("\n", "").Trim();
                //string _productPrice = _productResultCQ[".priceFirstRow"].First().Text().Replace("\n", "").Trim();

                string _productPrice = _productResultCQ[".priceFirstRow"].Text().Trim();
                _productPrice = _productPrice.Substring(0, _productPrice.IndexOf('\n'));

                string _productWeight = _productResultCQ[".subTitle"].Text();

                float _productPriceLiteral = 0;
                float _productPriceWeightLiteral = 0;

                try
                {
                    _productPriceLiteral = float.Parse(_productResultCQ[".OriginalListPrice"].Val().Replace('.', ','));
                    //_productPriceWeightLiteral = float.Parse(_productResultCQ[".PriceCapacityRatio"].Val().Replace('.', ','));
                }
                catch (Exception)
                {
                    //throw;
                }

                string _productPriceWeight = _productResultCQ[".priceSecondRow"].Text().Replace("\n", "").Replace(" ", "");
                string _productImageUrl = _productResultCQ["img"].Attr("data-original");
                //_productImageUrl = "http://www.jumbo.pt" + _productImageUrl;


                _productSearchResultList.Add(new ProductSearchResult
                {
                    Name = _productName,
                    Brand = _productBrand,
                    Price = _productPrice,
                    PriceWeight = _productPriceWeight,
                    Store = "Continente",
                    Url = _productUrl.Replace("https://www.continente.pt", ""),
                    Weight = _productWeight,
                    ImageUrl = _productImageUrl,
                    PriceLiteral = _productPriceLiteral,
                    PriceWeightLiteral = _productPriceWeightLiteral

                });
            }
            return _productSearchResultList;

        }

        public async Task<List<LisieStores.Extensibility.ProductSearchResult>> GetContinenteOnlineProductSearchResultsHeroku(string searchQuery)
        {
            string _searchQuery = string.IsNullOrEmpty(searchQuery) ? "" : searchQuery;
            string _jumboSearchResultsHtml = string.Empty;

            //TODO - use chromium to fetch 40 results or page 2
            HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("https://puppeteer-lisie.herokuapp.com/getContinenteSearchResultsHtml/" + searchQuery + "/40");
            webReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36";
            webReq.Accept = "*/*";
            //webReq.ServicePoint.ProtocolVersion\
            //webReq.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateRemoteCertificate);

            try
            {
                webReq.CookieContainer = new CookieContainer();
                webReq.Method = "GET";
                using (WebResponse response = webReq.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        //_htmlResult = reader.ReadToEnd();
                        _jumboSearchResultsHtml = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            List<LisieStores.Extensibility.ProductSearchResult> _productSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();

            CQ _jumboDom = _jumboSearchResultsHtml;
            CQ _produtos = _jumboDom[".productItem"];

            List<IDomObject> _productsList = _produtos.ToList();

            foreach (IDomObject _productResult in _productsList)
            {
                Console.WriteLine(_productResult.InnerHTML);

                CQ _productResultCQ = _productResult.InnerHTML;

                string _productUrl = _productResultCQ["a"].First().Attr("href");
                string _productName = _productResultCQ[".title"].First().Text().Replace("\n", "").Trim();
                string _productBrand = _productResultCQ[".type"].First().Text().Replace("\n", "").Trim();
                //string _productPrice = _productResultCQ[".priceFirstRow"].First().Text().Replace("\n", "").Trim();

                string _productPrice = _productResultCQ[".priceFirstRow"].Text().Trim();
                _productPrice = _productPrice.Substring(0, _productPrice.IndexOf('\n'));

                string _productWeight = _productResultCQ[".subTitle"].Text();

                float _productPriceLiteral = 0;
                float _productPriceWeightLiteral = 0;

                try
                {
                    _productPriceLiteral = float.Parse(_productResultCQ[".OriginalListPrice"].Val().Replace('.', ','));
                    //_productPriceWeightLiteral = float.Parse(_productResultCQ[".PriceCapacityRatio"].Val().Replace('.', ','));
                }
                catch (Exception)
                {
                    //throw;
                }

                string _productPriceWeight = _productResultCQ[".priceSecondRow"].Text().Replace("\n", "").Replace(" ", "");
                string _productImageUrl = _productResultCQ["img"].Attr("data-original");
                //_productImageUrl = "http://www.jumbo.pt" + _productImageUrl;


                _productSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
                {
                    Name = _productName,
                    Brand = _productBrand,
                    Price = _productPrice,
                    PriceWeight = _productPriceWeight,
                    StoreName = "Continente",
                    StoreId = 2,
                    Url = _productUrl.Replace("https://www.continente.pt", ""),
                    Weight = _productWeight,
                    ImageUrl = _productImageUrl,
                    PriceLiteral = _productPriceLiteral,
                    PriceWeightLiteral = _productPriceWeightLiteral

                });
            }
            return _productSearchResultList;

        }

        private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }
        //TODO - this method is called two times when calling http://localhost:4430/api/ProductsApi/GetUpdateStoresProductItem?productId=1019
        public async Task<LisieStores.Extensibility.ProductSearchResult> GetContinenteProductMetadata(string productUrl)
        {

            //string _returnedHtml = await FetchOnlineSearchResults(productUrl);

            string _returnedHtml = string.Empty;

            //HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("https://www.continente.pt" + productUrl);
            //string _htmlResult = string.Empty;
            //try
            //{
            //    webReq.CookieContainer = new CookieContainer();
            //    webReq.Method = "GET";
            //    webReq.Accept = "*/*";
            //    webReq.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateRemoteCertificate);
            //    using (WebResponse response = webReq.GetResponse())
            //    {
            //        using (Stream stream = response.GetResponseStream())
            //        {
            //            StreamReader reader = new StreamReader(stream);
            //            //_htmlResult = reader.ReadToEnd();
            //            _returnedHtml = reader.ReadToEnd();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}

            //TEST WITH COOKIE
            //HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("https://www.continente.pt" + productUrl);
            //string _htmlResult = string.Empty;
            //try
            //{
            //    webReq.CookieContainer = new CookieContainer();
            //    webReq.Method = "GET";
            //    webReq.Accept = "*/*";
            //    webReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36";
            //    webReq.Headers.Add("Cookie", "rbzid=UPee4f2A2WNavYP6YKhznW0ndCOWwp2z1pjoTNvbir7I9HXd/AUtRB94AmtNSc1XoJZu/k2tio9Q37kdEqYrp7kt7eND7t4+whTo0JmWRzHFuiu8GNvFUORvHuKhB5DmaRGCB5O19OejCMJe04TXXCj6S4lsPRqbZs/PdBmk5sS/adogxECu6Afup0jl3YmRshag3UurlaWAlBAEbl/4Zq2NTz0A/fcLik5h0ZbNUKFHetQueU9FWnh21iggUD9Ll/5NbsKgSRufWAvnCyY2sLQlyuUdCWmFQTFOGmQtUM0=");
            //    webReq.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateRemoteCertificate);
            //    using (WebResponse response = webReq.GetResponse())
            //    {
            //        using (Stream stream = response.GetResponseStream())
            //        {
            //            StreamReader reader = new StreamReader(stream);
            //            //_htmlResult = reader.ReadToEnd();
            //            _returnedHtml = reader.ReadToEnd();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}

            //THROUGH PUPETTER
            var finalProductUrl = "http://puppeteer-lisie.herokuapp.com/getContinenteProductMetadataHtml/" + HttpUtility.UrlEncode(productUrl);
            HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create(finalProductUrl);
            webReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36";
            webReq.Accept = "*/*";

            try
            {
                webReq.CookieContainer = new CookieContainer();
                webReq.Method = "GET";
                using (WebResponse response = webReq.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        //_htmlResult = reader.ReadToEnd();
                        _returnedHtml = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            LisieStores.Extensibility.ProductSearchResult _productSearchResultData = new LisieStores.Extensibility.ProductSearchResult();

            try
            {
                CQ _productDom = _returnedHtml;
                //CQ _produtos = _productDom[".produtoDetalhe"];

                //List<IDomObject> _productsList = _produtos.ToList();

                //CQ _productResultCQ = _productResult.InnerHTML;


                string _productUrl = productUrl;
                CQ _categoryElement = _productDom["._canonicalCategory"].Val();

                string _productCategoryString = _categoryElement["ri"].ElementAt(_categoryElement["ri"].Count() - 2).Attributes["v"];
                //_productCategoryString = _productDom[".Breadcrumb_canonicalcat li"].Children().Last().Text();
                string _productName = _productDom[".productTitle"].Text().Replace("\n", "").Trim();
                string _productBrand = _productDom[".productSubtitle"].Text().Replace("\n", "").Trim();
                string _productWeight = _productDom[".productSubsubtitle"].Text().Replace("\n", "").Trim();
                string _productPrice = _productDom[".updListPrice"].Text().Replace("\n", "").Trim().Replace(" ", "");
                string _productPriceWeight = _productDom[".pricePerLitre"].Text().Replace("\n", "").Trim().Replace(" ", "");
                string _productImageUrl = _productDom["#bigProduct"].Attr("href");

                float _productPriceLiteral = 0;
                float _productPriceWeightLiteral = 0;

                try
                {
                    _productPriceLiteral = float.Parse(_productDom[".OriginalListPrice"].Val().Replace('.', ','));
                    _productPriceWeightLiteral = float.Parse(_productDom[".PriceCapacityRatio"].Val().Replace('.', ','));
                }
                catch (Exception)
                {
                }


                _productSearchResultData = new LisieStores.Extensibility.ProductSearchResult
                {
                    Name = _productName,
                    Brand = _productBrand,
                    Price = _productPrice,
                    PriceWeight = _productPriceWeight,
                    StoreName = "Continente",
                    StoreId = 2,
                    Url = _productUrl.Replace("https://www.continente.pt", ""),
                    Weight = _productWeight,
                    ImageUrl = _productImageUrl,
                    Category = _productCategoryString,
                    PriceLiteral = _productPriceLiteral
                };
            }
            catch (Exception)
            {
                return null;
            }


            return _productSearchResultData;
        }

        #endregion

        #region Pingo Doce

        public async Task<List<LisieStores.Extensibility.ProductSearchResult>> GetPingoDoceOnlineProductSearchResults(string searchQuery)
        {
            string _searchQuery = string.IsNullOrEmpty(searchQuery) ? "" : searchQuery;
            string _searchResultsHtml = string.Empty;

            HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("https://mercadao.pt/api/catalogues/5afbf7f176f9b3001a672515/products/search?query=" + searchQuery);
            string _htmlResult = string.Empty;
            List<LisieStores.Extensibility.ProductSearchResult> _products = new List<LisieStores.Extensibility.ProductSearchResult>();
            try
            {
                webReq.CookieContainer = new CookieContainer();
                webReq.Method = "GET";
                using (WebResponse response = webReq.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        _searchResultsHtml = reader.ReadToEnd();

                        int startIndex = _searchResultsHtml.IndexOf("\"products\":[{");
                        int lastIndex = _searchResultsHtml.LastIndexOf("\"categories\":[{");
                        int lastIndex2 = _searchResultsHtml.Remove(lastIndex).LastIndexOf("]") + 1;

                        string lastJson = _searchResultsHtml.Substring(startIndex, lastIndex2 - startIndex);
                        //var values = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(_searchResultsHtml);


                        lastJson = "{" + lastJson + "}";

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        Models.PingoDoceSearch.ProductSerchResults _PingoDoceSearchResults = (Models.PingoDoceSearch.ProductSerchResults)js.Deserialize(lastJson, typeof(Models.PingoDoceSearch.ProductSerchResults));

                        foreach (Models.PingoDoceSearch.Product _product in _PingoDoceSearchResults.products)
                        {
                            _products.Add(new LisieStores.Extensibility.ProductSearchResult
                            {
                                Brand = _product._source.brand.name,
                                Category = _product._source.categories.Count > 0 ? _product._source.categories[0].name : string.Empty,
                                ImageUrl = "https://res.cloudinary.com/fonte-online/image/upload/c_fill,h_300,q_auto,w_300/v1/PDO_PROD/" + _product._source.sku + "_1",
                                Name = _product._source.firstName,
                                Price = "€ " + Math.Round(_product._source.buyingPrice, 2).ToString(),
                                PriceLiteral = float.Parse(_product._source.buyingPrice.ToString()),
                                PriceWeight = _product._source.buyingPrice.ToString() + " / " + _product._source.capacity,
                                PriceWeightLiteral = float.Parse(_product._source.buyingPrice.ToString()),
                                StoreName = "Pingo Doce",
                                StoreId = 3,
                                Url = "/api/catalogues/5afbf7f176f9b3001a672515/product/" + _product._source.slug,
                                Weight = _product._source.capacity

                            });
                        }
                        return _products;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return _products;
        }

        public async Task<LisieStores.Extensibility.ProductSearchResult> GetPingoDoceProductMetadata(string productUrl)
        {
            HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create("https://mercadao.pt" + productUrl);
            string _htmlResult = string.Empty;
            LisieStores.Extensibility.ProductSearchResult _productSearchResultData = new LisieStores.Extensibility.ProductSearchResult();
            try
            {
                webReq.CookieContainer = new CookieContainer();
                webReq.Method = "GET";
                using (WebResponse response = webReq.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        //_htmlResult = reader.ReadToEnd();
                        _htmlResult = reader.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        Models.PingoDoceProduct.Product _PingoDoceProduct = (Models.PingoDoceProduct.Product)js.Deserialize(_htmlResult, typeof(Models.PingoDoceProduct.Product));
                        _productSearchResultData = new LisieStores.Extensibility.ProductSearchResult
                        {
                            Brand = _PingoDoceProduct.brand.name,
                            Category = _PingoDoceProduct.categories.Count > 0 ? _PingoDoceProduct.categories[0].name : string.Empty,
                            ImageUrl = "https://res.cloudinary.com/fonte-online/image/upload/c_fill,h_300,q_auto,w_300/v1/PDO_PROD/" + _PingoDoceProduct.sku + "_1",
                            Name = _PingoDoceProduct.firstName,
                            Price = _PingoDoceProduct.buyingPrice.ToString(),
                            PriceLiteral = float.Parse(_PingoDoceProduct.buyingPrice.ToString()),
                            PriceWeight = _PingoDoceProduct.buyingPrice.ToString() + " / " + _PingoDoceProduct.capacity,
                            PriceWeightLiteral = float.Parse(_PingoDoceProduct.buyingPrice.ToString()),
                            StoreName = "Pingo Doce",
                            StoreId = 3,
                            Url = "/api/catalogues/5afbf7f176f9b3001a672515/product/" + _PingoDoceProduct.slug,
                            Weight = _PingoDoceProduct.capacity
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            return _productSearchResultData;
        }
        #endregion

        public async Task<List<LisieStores.Extensibility.ProductSearchResult>> GetSearchResults(string searchQuery)
        {
            List<LisieStores.Extensibility.ProductSearchResult> _allsearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();
            List<LisieStores.Extensibility.Market> _markets = Helpers.Extensibility.GetStoreFetchers();
            _markets = _markets.OrderBy(c => c.StoreId).ToList();

            foreach (var _market in _markets)
            {
                LisieStores.Extensibility.IMarketFetcher _marketFetcher = (LisieStores.Extensibility.IMarketFetcher)Activator.CreateInstance(_market.ClassType);
                _marketFetcher.StoreId = _market.StoreId;
                _marketFetcher.StoreName = _market.StoreName;
                _marketFetcher.StoreColor = _market.StoreColor;

                List<LisieStores.Extensibility.ProductSearchResult> _searchResults = await _marketFetcher.GetSearchResults(searchQuery);

                _allsearchResults.Add(new LisieStores.Extensibility.ProductSearchResult
                {
                    StoreId = _market.StoreId,
                    StoreName = _market.StoreName,
                    StoreColor = _market.StoreColor,
                    IsSeperator = true,
                    SeparatorTitle = _market.StoreName + " (" + _searchResults.Count + ")"
                });

                _allsearchResults.AddRange(_searchResults);
            }
            return _allsearchResults;

        }

        public async Task<List<Models.OnlineStoreSearchResults>> GetSearchResultsV2(string searchQuery)
        {
            List<Models.OnlineStoreSearchResults> _return = new List<Models.OnlineStoreSearchResults>();
            List<LisieStores.Extensibility.Market> _markets = Helpers.Extensibility.GetStoreFetchers();
            _markets = _markets.OrderBy(c => c.StoreId).ToList();

            foreach (var _market in _markets)
            {
                try
                {
                    LisieStores.Extensibility.IMarketFetcher _marketFetcher = (LisieStores.Extensibility.IMarketFetcher)Activator.CreateInstance(_market.ClassType);
                    _marketFetcher.StoreId = _market.StoreId;
                    _marketFetcher.StoreName = _market.StoreName;
                    _marketFetcher.StoreColor = _market.StoreColor;

                    List<LisieStores.Extensibility.ProductSearchResult> _searchResults = await _marketFetcher.GetSearchResults(searchQuery);

                    _return.Add(new Models.OnlineStoreSearchResults
                    {
                        StoreId = _market.StoreId,
                        StoreName = _market.StoreName,
                        Results = _searchResults
                    });
                }
                catch (Exception ex)
                {
                    continue;
                }


            }
            return _return;
        }

        public async Task<List<LisieStores.Extensibility.ProductSearchResult>> GetSearchResultsWithNoSeparators(string searchQuery)
        {
            List<LisieStores.Extensibility.ProductSearchResult> _allsearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();
            List<LisieStores.Extensibility.Market> _markets = Helpers.Extensibility.GetStoreFetchers();
            _markets = _markets.OrderBy(c => c.StoreId).ToList();

            foreach (var _market in _markets)
            {
                LisieStores.Extensibility.IMarketFetcher _marketFetcher = (LisieStores.Extensibility.IMarketFetcher)Activator.CreateInstance(_market.ClassType);
                _marketFetcher.StoreId = _market.StoreId;
                _marketFetcher.StoreName = _market.StoreName;
                _marketFetcher.StoreColor = _market.StoreColor;

                List<LisieStores.Extensibility.ProductSearchResult> _searchResults = await _marketFetcher.GetSearchResults(searchQuery);
                _allsearchResults.AddRange(_searchResults);
            }
            return _allsearchResults;

        }

        public async Task<List<LisieStores.Extensibility.ProductSearchResult>> GetSearchResultsWithNoSeparatorsOfStore(string searchQuery, int storeId)
        {
            List<LisieStores.Extensibility.ProductSearchResult> _allsearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();
            List<LisieStores.Extensibility.Market> _markets = Helpers.Extensibility.GetStoreFetchers();
            var _market = _markets.Where(c => c.StoreId == storeId).FirstOrDefault();
            if (_market != null)
            {
                LisieStores.Extensibility.IMarketFetcher _marketFetcher = (LisieStores.Extensibility.IMarketFetcher)Activator.CreateInstance(_market.ClassType);
                _marketFetcher.StoreId = _market.StoreId;
                _marketFetcher.StoreName = _market.StoreName;
                _marketFetcher.StoreColor = _market.StoreColor;

                List<LisieStores.Extensibility.ProductSearchResult> _searchResults = await _marketFetcher.GetSearchResults(searchQuery);
                _allsearchResults.AddRange(_searchResults);
            }
            return _allsearchResults;

        }

        public async Task<LisieStores.Extensibility.ProductSearchResult> GetProductMetadata(int storeId, string url)
        {
            LisieStores.Extensibility.ProductSearchResult _result = new LisieStores.Extensibility.ProductSearchResult();
            List<LisieStores.Extensibility.Market> _markets = Helpers.Extensibility.GetStoreFetchers();
            var _market = _markets.Where(c => c.StoreId == storeId).FirstOrDefault();
            if (_market != null)
            {
                LisieStores.Extensibility.IMarketFetcher _marketFetcher = (LisieStores.Extensibility.IMarketFetcher)Activator.CreateInstance(_market.ClassType);
                _marketFetcher.StoreId = _market.StoreId;
                _marketFetcher.StoreUrl = _market.StoreUrl;
                _marketFetcher.StoreName = _market.StoreName;
                _result = await _marketFetcher.GetProductMetadata(url);
            }
            return _result;
        }

        public async Task<bool> AddProductsToOnlineStoreCart(string userId, int storeId, string storeUsername, string storePassword, List<int> userProductsIds)
        {
            bool _result = false;
            List<LisieStores.Extensibility.Market> _markets = Helpers.Extensibility.GetStoreFetchers();
            List<LisieStores.Extensibility.ProductAddToOnlineStore> _productsToAdd = new List<LisieStores.Extensibility.ProductAddToOnlineStore>();

            var _currentMarket = _markets.Find(c => c.StoreId == storeId);
            if (_currentMarket != null)
            {
                using (ClassLibrary1.SpiroStockManagementEntities db = new ClassLibrary1.SpiroStockManagementEntities())
                {
                    foreach (var _userProductId in userProductsIds)
                    {
                        var _userProduct = db.UserProductsList.Where(u => u.UserId.Equals(userId)).Where(u => u.Id == _userProductId).FirstOrDefault();
                        if (_userProduct != null)
                        {
                            var userShoppingList = from m in db.StoreProducts where m.ProductId == _userProduct.ProductId select m;
                            if (userShoppingList.Count() > 0)
                            {
                                foreach (var storeProduct in userShoppingList)
                                {
                                    if (storeProduct.Stores.Id == storeId) _productsToAdd.Add(new LisieStores.Extensibility.ProductAddToOnlineStore
                                    {
                                        UserProductListId = _userProduct.Id,
                                        Name = storeProduct.Products.Name,
                                        OnlineProductId = storeProduct.OnlineProductId,
                                        Url = storeProduct.Url,
                                        Quantity = _userProduct.Quantity.Value
                                    });
                                }
                            }
                        }
                    }
                }

                LisieStores.Extensibility.IMarketFetcher _marketFetcher = (LisieStores.Extensibility.IMarketFetcher)Activator.CreateInstance(_currentMarket.ClassType);
                _result = await _marketFetcher.AddProductsToOnlineStoreCart(_productsToAdd, userId, storeUsername, storePassword);

            }
            return _result;
        }

        public async Task<bool> AddAllStoreProductsToOnlineStoreCart(string userId, int storeId, string storeUsername, string storePassword)
        {
            bool _result = false;
            List<LisieStores.Extensibility.Market> _markets = Helpers.Extensibility.GetStoreFetchers();
            List<LisieStores.Extensibility.ProductAddToOnlineStore> _productsToAdd = new List<LisieStores.Extensibility.ProductAddToOnlineStore>();

            var _currentMarket = _markets.Find(c => c.StoreId == storeId);
            if (_currentMarket != null)
            {
                using (ClassLibrary1.SpiroStockManagementEntities db = new ClassLibrary1.SpiroStockManagementEntities())
                {
                    var userProductsIds = db.UserProductsList.Where(c => c.UserId == userId && c.ListName.ToLower() == "in").Select(c => c.Id);
                    foreach (var _userProductId in userProductsIds)
                    {
                        var _userProduct = db.UserProductsList.Where(u => u.UserId.Equals(userId)).Where(u => u.Id == _userProductId).FirstOrDefault();
                        if (_userProduct != null)
                        {
                            var userShoppingList = from m in db.StoreProducts where m.ProductId == _userProduct.ProductId select m;
                            if (userShoppingList.Count() > 0)
                            {
                                foreach (var storeProduct in userShoppingList)
                                {
                                    if (storeProduct.Stores.Id == storeId) _productsToAdd.Add(new LisieStores.Extensibility.ProductAddToOnlineStore
                                    {
                                        UserProductListId = _userProduct.Id,
                                        Name = storeProduct.Products.Name,
                                        OnlineProductId = storeProduct.OnlineProductId,
                                        Url = storeProduct.Url,
                                        Quantity = _userProduct.Quantity.Value
                                    });
                                }
                            }
                        }
                    }
                }

                LisieStores.Extensibility.IMarketFetcher _marketFetcher = (LisieStores.Extensibility.IMarketFetcher)Activator.CreateInstance(_currentMarket.ClassType);
                _result = await _marketFetcher.AddProductsToOnlineStoreCart(_productsToAdd, userId, storeUsername, storePassword);

            }
            return _result;
        }


        public async Task<List<LisieStores.Extensibility.ProductAddToOnlineStore>> GetAllStoreProductsToAddToOnlineStoreCart(string userId, int storeId)
        {
            bool _result = false;
            List<LisieStores.Extensibility.Market> _markets = Helpers.Extensibility.GetStoreFetchers();
            List<LisieStores.Extensibility.ProductAddToOnlineStore> _productsToAdd = new List<LisieStores.Extensibility.ProductAddToOnlineStore>();

            var _currentMarket = _markets.Find(c => c.StoreId == storeId);
            if (_currentMarket != null)
            {
                using (ClassLibrary1.SpiroStockManagementEntities db = new ClassLibrary1.SpiroStockManagementEntities())
                {
                    var userProductsIds = db.UserProductsList.Where(c => c.UserId == userId && c.ListName.ToLower() == "in").Select(c => c.Id);
                    foreach (var _userProductId in userProductsIds)
                    {
                        var _userProduct = db.UserProductsList.Where(u => u.UserId.Equals(userId)).Where(u => u.Id == _userProductId).FirstOrDefault();
                        if (_userProduct != null)
                        {
                            var userShoppingList = from m in db.StoreProducts where m.ProductId == _userProduct.ProductId select m;
                            if (userShoppingList.Count() > 0)
                            {
                                foreach (var storeProduct in userShoppingList)
                                {
                                    if (storeProduct.Stores.Id == storeId) _productsToAdd.Add(new LisieStores.Extensibility.ProductAddToOnlineStore
                                    {
                                        UserProductListId = _userProduct.Id,
                                        Name = storeProduct.Products.Name,
                                        OnlineProductId = storeProduct.OnlineProductId,
                                        Url = storeProduct.Url,
                                        Quantity = _userProduct.Quantity.Value,
                                        StoreId = storeId
                                    });
                                }
                            }
                        }
                    }
                }
            }
            return _productsToAdd;
        }

        public async Task<LisieStores.Extensibility.ProductSearchResult> GetMarketProductByBarcode(int storeId, string barcode)
        {
            LisieStores.Extensibility.ProductSearchResult _result = new LisieStores.Extensibility.ProductSearchResult();
            LisieStores.Extensibility.Market _market = Helpers.Extensibility.GetStoreFetchers().Where(c => c.StoreId == storeId).FirstOrDefault();
            if (_market != null)
            {
                LisieStores.Extensibility.IMarketFetcher _marketFetcher = (LisieStores.Extensibility.IMarketFetcher)Activator.CreateInstance(_market.ClassType);
                _result = await _marketFetcher.GetProductMetadataByBarcode(barcode);
            }
            return _result;
        }

        public async Task<List<LisieStores.Extensibility.ProductSearchResult>> GetMarketsProductByBarcode(string barcode)
        {
            List<LisieStores.Extensibility.ProductSearchResult> _allsearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();
            List<LisieStores.Extensibility.Market> _markets = Helpers.Extensibility.GetStoreFetchers();
            _markets = _markets.OrderBy(c => c.StoreId).ToList();

            foreach (var _market in _markets)
            {
                LisieStores.Extensibility.IMarketFetcher _marketFetcher = (LisieStores.Extensibility.IMarketFetcher)Activator.CreateInstance(_market.ClassType);
                _marketFetcher.StoreId = _market.StoreId;
                _marketFetcher.StoreName = _market.StoreName;
                _marketFetcher.StoreColor = _market.StoreColor;

                LisieStores.Extensibility.ProductSearchResult _result = await _marketFetcher.GetProductMetadataByBarcode(barcode);
                if (_result != null)
                {
                    _allsearchResults.Add(_result);
                }

            }
            return _allsearchResults;
        }

        public async Task<LisieStores.Extensibility.ProductSearchResult> GetMarketProductByUrl(int storeId, string url)
        {
            LisieStores.Extensibility.ProductSearchResult _result = new LisieStores.Extensibility.ProductSearchResult();
            LisieStores.Extensibility.Market _market = Helpers.Extensibility.GetStoreFetchers().Where(c => c.StoreId == storeId).FirstOrDefault();
            if (_market != null)
            {
                LisieStores.Extensibility.IMarketFetcher _marketFetcher = (LisieStores.Extensibility.IMarketFetcher)Activator.CreateInstance(_market.ClassType);
                _result = await _marketFetcher.GetProductMetadata(url);
            }
            return _result;
        }

        public async Task<LisieStores.Extensibility.ProductSearchResult> GetMarketProductByOnlineId(int storeId, string onlineProductId)
        {
            LisieStores.Extensibility.ProductSearchResult _result = new LisieStores.Extensibility.ProductSearchResult();
            LisieStores.Extensibility.Market _market = Helpers.Extensibility.GetStoreFetchers().Where(c => c.StoreId == storeId).FirstOrDefault();
            if (_market != null)
            {
                LisieStores.Extensibility.IMarketFetcher _marketFetcher = (LisieStores.Extensibility.IMarketFetcher)Activator.CreateInstance(_market.ClassType);
                _result = await _marketFetcher.GetProductMetadataById(onlineProductId);
            }
            return _result;
        }


        public List<LisieStores.Extensibility.StoreAddToOnline> GetStoresProductsToAddToOnlineStoreCart(string userId, List<int> storeIds)
        {
            bool _result = false;
            List<LisieStores.Extensibility.Market> _markets = Helpers.Extensibility.GetStoreFetchers();
            List<LisieStores.Extensibility.ProductAddToOnlineStore> _productsToAdd = new List<LisieStores.Extensibility.ProductAddToOnlineStore>();

            //var _currentMarket = _markets.Find(c => c.StoreId == storeId);
            //if (_currentMarket != null)
            //{

            List<LisieStores.Extensibility.StoreAddToOnline> _toRet = new List<LisieStores.Extensibility.StoreAddToOnline>();
            foreach (var _storeId in storeIds)
            {
                _toRet.Add(new LisieStores.Extensibility.StoreAddToOnline
                {
                    StoreId = _storeId,
                    Products = new List<LisieStores.Extensibility.ProductAddToOnlineStore>()
                });
            }

            using (ClassLibrary1.SpiroStockManagementEntities db = new ClassLibrary1.SpiroStockManagementEntities())
            {
                var userProductsIds = db.UserProductsList.Where(c => c.UserId == userId && c.ListName.ToLower() == "in").OrderByDescending(c => c.Id).Select(c => c.Id);
                foreach (var _userProductId in userProductsIds)
                {
                    var _userProduct = db.UserProductsList.Where(u => u.UserId.Equals(userId)).Where(u => u.Id == _userProductId).FirstOrDefault();
                    if (_userProduct != null)
                    {
                        var userShoppingList = from m in db.StoreProducts where m.ProductId == _userProduct.ProductId select m;
                        if (userShoppingList.Count() > 0)
                        {
                            List<StoreProducts> _newStoreProducts = new List<StoreProducts>();
                            foreach (var storeProduct in userShoppingList)
                            {
                                if (storeIds.Contains(storeProduct.StoreId))
                                {
                                    storeProduct.Price = Math.Round(storeProduct.Price.Value, 2);
                                    _newStoreProducts.Add(storeProduct);
                                }
                            }
                            if (_newStoreProducts.Count > 1)
                            {
                                //get chepeast one
                                var _cheapestValue = _newStoreProducts.Min(c => c.Price);

                                //see if is more with the same cheapest value
                                var _chepestValues = _newStoreProducts.Where(c => c.Price.Value == _cheapestValue).ToList();
                                if (_chepestValues.Count == 1)
                                {
                                    LisieStores.Extensibility.StoreAddToOnline _toAddTo = _toRet.Where(c => c.StoreId == _chepestValues[0].StoreId).FirstOrDefault();
                                    _toAddTo.Products.Add(new LisieStores.Extensibility.ProductAddToOnlineStore
                                    {
                                        ProductId = _userProduct.ProductId,
                                        StoreId = _chepestValues[0].StoreId,
                                        Name = _userProduct.Products.Name,
                                        OnlineProductId = _chepestValues[0].OnlineProductId,
                                        Quantity = _userProduct.Quantity.Value,
                                        Price = _chepestValues[0].Price.Value * _userProduct.Quantity.Value,
                                        UserProductListId = _userProduct.Id,
                                        Url = _chepestValues[0].Url
                                    });
                                }
                                else if (_chepestValues.Count > 1)
                                {
                                    foreach (var _storeIdByOrder in storeIds)
                                    {
                                        var _chepeastValueExistsIn = _chepestValues.Find(c => c.StoreId == _storeIdByOrder);
                                        if (_chepeastValueExistsIn != null)
                                        {
                                            LisieStores.Extensibility.StoreAddToOnline _toAddTo = _toRet.Where(c => c.StoreId == _chepeastValueExistsIn.StoreId).FirstOrDefault();
                                            _toAddTo.Products.Add(new LisieStores.Extensibility.ProductAddToOnlineStore
                                            {
                                                ProductId = _userProduct.ProductId,
                                                StoreId = _chepeastValueExistsIn.StoreId,
                                                Name = _userProduct.Products.Name,
                                                OnlineProductId = _chepeastValueExistsIn.OnlineProductId,
                                                Quantity = _userProduct.Quantity.Value,
                                                Price = _chepeastValueExistsIn.Price.Value * _userProduct.Quantity.Value,
                                                UserProductListId = _userProduct.Id,
                                                Url = _chepeastValueExistsIn.Url
                                            });
                                            break;
                                        }
                                    }
                                }
                                //if yes, get in order of storeIds
                            }
                            else if (_newStoreProducts.Count == 1)
                            {
                                //add to correct store to add online
                                LisieStores.Extensibility.StoreAddToOnline _toAddTo = _toRet.Where(c => c.StoreId == _newStoreProducts[0].StoreId).FirstOrDefault();
                                _toAddTo.Products.Add(new LisieStores.Extensibility.ProductAddToOnlineStore
                                {
                                    ProductId = _userProduct.ProductId,
                                    StoreId = _newStoreProducts[0].StoreId,
                                    Name = _userProduct.Products.Name,
                                    OnlineProductId = _newStoreProducts[0].OnlineProductId,
                                    Quantity = _userProduct.Quantity.Value,
                                    Price = _newStoreProducts[0].Price.Value * _userProduct.Quantity.Value,
                                    UserProductListId = _userProduct.Id,
                                    Url = _newStoreProducts[0].Url
                                });
                            }

                        }
                    }
                }

                foreach (var _toRetItem in _toRet)
                {
                    _toRetItem.TotalProducts = _toRetItem.Products.Count;
                    _toRetItem.TotalPrice = Math.Round(_toRetItem.Products.Sum(c => c.Price), 2);
                }
            }

            return _toRet;
        }

        public LisieStores.Extensibility.StoreTotalSavings GetStoresProductsTotalSavings(string userId, List<int> storeIds)
        {
            LisieStores.Extensibility.StoreTotalSavings _totalSavings = new LisieStores.Extensibility.StoreTotalSavings
            {
                Cheapest = 0,
                Highest = 0,
                TotalProducts = 0
            };
            //List<LisieStores.Extensibility.StoreAddToOnline> _toRet = new List<LisieStores.Extensibility.StoreAddToOnline>();
            double _highestPrice = 0;
            double _lowestPrice = 0;

            //foreach (var _storeId in storeIds)
            //{
            //    _toRet.Add(new LisieStores.Extensibility.StoreAddToOnline
            //    {
            //        StoreId = _storeId,
            //        Products = new List<LisieStores.Extensibility.ProductAddToOnlineStore>()
            //    });
            //}

            using (ClassLibrary1.SpiroStockManagementEntities db = new ClassLibrary1.SpiroStockManagementEntities())
            {
                var userProductsIds = db.UserProductsList.Where(c => c.UserId == userId && c.ListName.ToLower() == "in").OrderByDescending(c => c.Id).Select(c => c.Id);
                foreach (var _userProductId in userProductsIds)
                {
                    var _userProduct = db.UserProductsList.Where(u => u.UserId.Equals(userId)).Where(u => u.Id == _userProductId).FirstOrDefault();
                    if (_userProduct != null)
                    {
                        var _productStores = from m in db.StoreProducts where m.ProductId == _userProduct.ProductId select m;
                        if (_productStores.Count() > 0)
                        {
                            List<StoreProducts> _newStoreProducts = new List<StoreProducts>();
                            foreach (var storeProduct in _productStores)
                            {
                                if (storeIds.Contains(storeProduct.StoreId))
                                {
                                    storeProduct.Price = Math.Round(storeProduct.Price.Value, 2);
                                    _newStoreProducts.Add(storeProduct);
                                }
                            }
                            if (_newStoreProducts.Count > 0)
                            {
                                var _cheapestValue = _newStoreProducts.Min(c => c.Price);
                                var _mostExpensiveValue = _newStoreProducts.Max(c => c.Price);

                                _totalSavings.Cheapest += _cheapestValue.Value * _userProduct.Quantity.Value;
                                _totalSavings.Highest += _mostExpensiveValue.Value * _userProduct.Quantity.Value;
                                _totalSavings.TotalProducts = _totalSavings.TotalProducts + 1;
                            }
                        }
                    }
                }

                //_lowestPrice = Math.Round(_lowestPrice, 2);
                //_highestPrice = Math.Round(_highestPrice, 2);
                var _savingsPercentage = Math.Floor((1 - _totalSavings.Cheapest / _totalSavings.Highest) * 100);
                _totalSavings.Percentage = Math.Round(_savingsPercentage, 2).ToString() + "%";
                _totalSavings.PercentageValue = Math.Round(_savingsPercentage, 2);
                _totalSavings.Cheapest = Math.Round(_totalSavings.Cheapest, 2);
                _totalSavings.Highest = Math.Round(_totalSavings.Highest, 2);
                _totalSavings.Highest = Math.Round(_totalSavings.Highest, 2);
                _totalSavings.PriceDifference = Math.Round(_totalSavings.Highest - _totalSavings.Cheapest, 2);
                return _totalSavings;
            }

            //return _toRet;
        }

    }
}