using CsQuery;
using LisieStores.Extensibility;
using SpiroWeb.Controllers;
using SpiroWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace SpiroWeb.Markets
{
    [MarketAttr(2, "Continente", "https://www.continente.pt", "#567adc", "https://www.continente.pt/pt-pt/public/Pages/searchresults.aspx?k=")]
    public class Continente : Market, IMarketFetcher
    {
        string IMarketFetcher.GetProductViewableUrl(string onlineProductId, string url)
        {
            if (!string.IsNullOrEmpty(onlineProductId))
            {
                return this.StoreUrl + "/produto/" + onlineProductId + ".html";
            }
            if (!string.IsNullOrEmpty(url))
            {
                return this.StoreUrl + url;
            }
            return string.Empty;
        }

        async public Task<bool> AddProductsToOnlineStoreCart(List<ProductAddToOnlineStore> products, string userId, string storeUsername, string storePassword)
        {
            //foreach (var _product in products)
            //{
            //    if (_product.Url.IndexOf("Auchan_Amadora?") == -1)
            //        _product.Url += "/Auchan_Amadora?sid=14a1f7c0-f5bd-4b08-8bf0-f95f908d41dc_1";
            //}
            var cli = new WebClient();
            cli.Headers[HttpRequestHeader.ContentType] = "application/json";

            var _nodeRequest = new NodeRequest
            {
                username = storeUsername,
                password = storePassword,
                products = products
            };
            var json = new JavaScriptSerializer().Serialize(_nodeRequest);
            string response = cli.UploadString("https://puppeteer-lisie.herokuapp.com/addUserListToContinente/" + userId, json);

            return true;
        }

        public async Task<LisieStores.Extensibility.ProductSearchResult> GetProductMetadataByBarcode(string barcode)
        {
            string _SearchResultsHtml = await FetchUrl("https://www.continente.pt/pesquisa/?q=" + barcode);
            List<LisieStores.Extensibility.ProductSearchResult> _productSearchResultList = null;

            CQ _jumboDom = _SearchResultsHtml;
            CQ _produtos = _jumboDom[".product"];

            List<IDomObject> _productsList = _produtos.ToList();
            string _productUrl = string.Empty;
            foreach (IDomObject _productResult in _productsList)
            {
                Console.WriteLine(_productResult.InnerHTML);

                CQ _productResultCQ = _productResult.OuterHTML;

                var _productOnlineId = _productResultCQ.Attr("data-pid");
                _productUrl = _productResultCQ["a"].First().Attr("href");
                break;
            }
            if (!string.IsNullOrEmpty(_productUrl))
                return await GetMetadata(_productUrl);

            return null;
        }

        async Task<LisieStores.Extensibility.ProductSearchResult> IMarketFetcher.GetProductMetadata(string url)
        {
            return await GetMetadata(url);
        }
        //async Task<LisieStores.Extensibility.ProductSearchResult> IMarketFetcher.GetProductMetadata(string url)
        //{
        //    var newUrl = url;
        //    if (newUrl.ToLower().IndexOf("/stores/") > -1)
        //    {
        //        int _startIndex = newUrl.IndexOf("ProductId=") + "ProductId=".Length;
        //        string _productId = newUrl.Substring(_startIndex);
        //        if (_productId.IndexOf("(") > -1)
        //        {
        //            int _startIndex2 = _productId.IndexOf("(");
        //            _productId = _productId.Substring(0, _startIndex2);
        //            newUrl = "/produto/" + _productId + ".html";
        //        }
        //    }
        //    //Heroku server 1
        //    //string _returnedHtml = await FetchUrl("http://puppeteer-lisie.herokuapp.com/getContinenteProductMetadataHtml/" + HttpUtility.UrlEncode(url));
        //    //Heroku server 2
        //    //string _returnedHtml = await FetchUrl("http://lisie.herokuapp.com/getContinenteProductMetadataHtml/" + HttpUtility.UrlEncode(url));
        //    //Localhost server
        //    //string _returnedHtml= await FetchUrl("http://localhost:3000/getContinenteProductMetadataHtml/" + HttpUtility.UrlEncode(url));
        //    string _returnedHtml = await FetchUrl("https://www.continente.pt" + newUrl);

        //    int _indexBeg = _returnedHtml.IndexOf("<script type=\"application/ld+json\">") + "<script type=\"application/ld+json\">".Length;
        //    int _indexEnd = _returnedHtml.IndexOf("</script>", _indexBeg);
        //    string _productJson = _returnedHtml.Substring(_indexBeg, _indexEnd - _indexBeg);

        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    Models.AuchanContinenteProductOnlineMetadata.Product _onlineProduct = (Models.AuchanContinenteProductOnlineMetadata.Product)js.Deserialize(_productJson, typeof(Models.AuchanContinenteProductOnlineMetadata.Product));

        //    LisieStores.Extensibility.ProductSearchResult _productSearchResultData = new LisieStores.Extensibility.ProductSearchResult();

        //    try
        //    {
        //        //get Barcode
        //        _indexBeg = _returnedHtml.IndexOf("&amp;ean=") + "&amp;ean=".Length;
        //        _indexEnd = _returnedHtml.IndexOf("&amp;", _indexBeg);
        //        string _productBarcode = string.Empty;
        //        if (_indexBeg > -1 && _indexEnd > -1 && _indexEnd > _indexBeg)
        //        {
        //            _productBarcode = _returnedHtml.Substring(_indexBeg, _indexEnd - _indexBeg);
        //        }

        //        CQ _productDom = _returnedHtml;

        //        var _productWeight = _productDom[".ct-pdp--unit"].First().Text().Replace("\n", "").Trim();
        //        var _productPrimaryPriceValue = _productDom[".ct-tile--price-primary .ct-price-formatted"].Text().Replace("\n", "").Replace("€", "").Trim();
        //        var _productSecundaryPriceValue = _productDom[".ct-tile--price-secondary .ct-price-value"].Text().Replace("\n", "").Replace("€", "").Trim();
        //        var _productPrimaryPriceUnit = _productDom[".ct-tile--price-primary .ct-m-unit"].Text().Replace("\n", "").Replace("/", "").Trim();
        //        var _productSecundaryPriceUnit = _productDom[".ct-tile--price-secondary .ct-m-unit"].Text().Replace("\n", "").Replace("/", "").Trim();

        //        var _productPrice = _productPrimaryPriceUnit.ToLower() == "un" ? _productPrimaryPriceValue : _productSecundaryPriceValue;
        //        var _productPriceWeight = _productPrimaryPriceUnit.ToLower() == "un" ? _productSecundaryPriceValue : _productPrimaryPriceValue;
        //        var _prodyctUnitRatio = _productPrimaryPriceUnit.ToLower() == "un" ? _productSecundaryPriceUnit : _productPrimaryPriceUnit;

        //        //Category
        //        CQ _categories = _productDom[".breadcrumbs-item"];

        //        List<IDomObject> _categoriesList = _categories.ToList();
        //        int _productCategoriesCounter = 1;
        //        string _productCategory = "";
        //        string _productCategoriesFull = "";
        //        foreach (IDomObject _category in _categories)
        //        {
        //            CQ _categoryCQ = _category.OuterHTML;
        //            string _categoryText = _categoryCQ["span"].Text().Replace("\n", "").Trim();

        //            if (_productCategoriesCounter == 3) //Do nothing, this category is irrelevant
        //            {
        //                _productCategory = _categoryText;
        //                _productCategoriesFull = _categoryText + " > ";

        //            }
        //            else if (_productCategoriesCounter > 3 && !string.IsNullOrEmpty(_categoryText))
        //            {
        //                if (_productCategoriesCounter != _categories.Length)
        //                    _productCategoriesFull += _categoryText + " > ";
        //                else
        //                    _productCategoriesFull += _categoryText;
        //            }
        //            _productCategoriesCounter++;
        //        }

        //        _productSearchResultData = new LisieStores.Extensibility.ProductSearchResult
        //        {
        //            Barcode = _productBarcode,
        //            Name = _onlineProduct.name.Trim(),
        //            Brand = _onlineProduct.brand.name.Trim(),
        //            Price = _productPrice.Trim(),
        //            PriceWeight = _productPriceWeight.Trim(),
        //            StoreId = this.StoreId,
        //            StoreName = this.StoreName,
        //            StoreColor = this.StoreColor,
        //            Url = newUrl,
        //            ViewableUrl = newUrl,
        //            Weight = _productWeight.Trim(),
        //            ImageUrl = _onlineProduct.image[0],
        //            PriceLiteral = 0,
        //            PriceWeightLiteral = 0,
        //            Category = _productCategory.Trim(),
        //            FullCategory = _productCategoriesFull.Trim(),
        //            Unit = _prodyctUnitRatio.Trim(),
        //            OnlineProductId = _onlineProduct.sku.Trim()
        //        };
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //    return _productSearchResultData;
        //}

        //OLD WEBSITE
        //async Task<List<LisieStores.Extensibility.ProductSearchResult>> IMarketFetcher.GetSearchResults(string searchQuery)
        //{
        //    string _searchQuery = string.IsNullOrEmpty(searchQuery) ? "" : searchQuery;
        //    string _SearchResultsHtml = await FetchUrl("https://puppeteer-lisie.herokuapp.com/getContinenteSearchResultsHtml/" + searchQuery + "/40");
        //    //string _SearchResultsHtml = await FetchUrl("https://lisie.herokuapp.com/getContinenteSearchResultsHtml/" + searchQuery + "/40");
        //    //string _SearchResultsHtml = await FetchUrl("https://www.continente.pt/pt-pt/public/Pages/searchresults.aspx?k=" + searchQuery + "#/?pl=4040");
        //    //string _SearchResultsHtml = await FetchUrl("http://localhost:3000/getContinenteSearchResultsHtml/" + searchQuery.Replace("/", "") + "/40");

        //    List <LisieStores.Extensibility.ProductSearchResult> _productSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();

        //    CQ _jumboDom = _SearchResultsHtml;
        //    CQ _produtos = _jumboDom[".productItem"];

        //    List<IDomObject> _productsList = _produtos.ToList();

        //    foreach (IDomObject _productResult in _productsList)
        //    {
        //        Console.WriteLine(_productResult.InnerHTML);

        //        CQ _productResultCQ = _productResult.InnerHTML;

        //        string _productUrl = _productResultCQ["a"].First().Attr("href");
        //        string _productName = _productResultCQ[".title"].First().Text().Replace("\n", "").Trim();
        //        string _productBrand = _productResultCQ[".type"].First().Text().Replace("\n", "").Trim();
        //        //string _productPrice = _productResultCQ[".priceFirstRow"].First().Text().Replace("\n", "").Trim();

        //        string _productPrice = _productResultCQ[".priceFirstRow"].Text().Trim();
        //        _productPrice = _productPrice.Substring(0, _productPrice.IndexOf('\n'));

        //        string _productWeight = _productResultCQ[".subTitle"].Text();

        //        float _productPriceLiteral = 0;
        //        float _productPriceWeightLiteral = 0;

        //        try
        //        {
        //            _productPriceLiteral = float.Parse(_productResultCQ[".OriginalListPrice"].Val().Replace('.', ','));
        //            //_productPriceWeightLiteral = float.Parse(_productResultCQ[".PriceCapacityRatio"].Val().Replace('.', ','));
        //        }
        //        catch (Exception)
        //        {
        //            //throw;
        //        }

        //        string _productPriceWeight = _productResultCQ[".priceSecondRow"].Text().Replace("\n", "").Replace(" ", "");
        //        _productPriceWeight = _productPriceWeight.Substring(0, _productPriceWeight.IndexOf('/'));
        //        string _productImageUrl = _productResultCQ["img"].Attr("data-original");


        //        _productSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
        //        {
        //            Name = _productName,
        //            Brand = _productBrand,
        //            Price = _productPrice.Replace("€", "").Trim(' ') + "€",
        //            PriceWeight = _productPriceWeight,
        //            StoreId = this.StoreId,
        //            StoreName = this.StoreName,
        //            StoreColor = this.StoreColor,
        //            Url = _productUrl.Replace("https://www.continente.pt", ""),
        //            ViewableUrl = _productUrl.Replace("https://www.continente.pt", ""),
        //            Weight = _productWeight,
        //            ImageUrl = _productImageUrl,
        //            PriceLiteral = _productPriceLiteral,
        //            PriceWeightLiteral = _productPriceWeightLiteral,
        //            Category = "",
        //            FullCategory = "",
        //            Unit = ""
        //    });
        //    }
        //    return _productSearchResultList;
        //}

        async Task<List<LisieStores.Extensibility.ProductSearchResult>> IMarketFetcher.GetSearchResults(string searchQuery)
        {
            string _searchQuery = string.IsNullOrEmpty(searchQuery) ? "" : searchQuery;
            //string _SearchResultsHtml = await FetchUrl("https://puppeteer-lisie.herokuapp.com/getContinenteSearchResultsHtml/" + searchQuery + "/40");
            //string _SearchResultsHtml = await FetchUrl("https://lisie.herokuapp.com/getContinenteSearchResultsHtml/" + searchQuery + "/40");
            string _SearchResultsHtml = await FetchUrl("https://www.continente.pt/pesquisa/?q=" + searchQuery);
            //string _SearchResultsHtml = await FetchUrl("http://localhost:3000/getContinenteSearchResultsHtml/" + searchQuery.Replace("/", "") + "/40");

            List<LisieStores.Extensibility.ProductSearchResult> _productSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();

            CQ _jumboDom = _SearchResultsHtml;
            CQ _produtos = _jumboDom[".product"];

            List<IDomObject> _productsList = _produtos.ToList();

            foreach (IDomObject _productResult in _productsList)
            {
                Console.WriteLine(_productResult.InnerHTML);

                CQ _productResultCQ = _productResult.OuterHTML;

                var _productOnlineId = _productResultCQ.Attr("data-pid");
                string _productUrl = _productResultCQ["a"].First().Attr("href");
                string _productName = _productResultCQ[".pwc-tile--description "].Text().Replace("\n", "").Trim();
                //string _productBrand = _productResultCQ[".col-tile--brand"].First().Text().Replace("\n", "").Trim();
                string _productBrand = ""; //doesn´t appear on html
                string _productImageUrl = _productResultCQ[".ct-tile-image"].First().Attr("data-src");
                var _productWeight = _productResultCQ[".pwc-tile--quantity"].First().Text().Replace("\n", "").Trim();
                var _productPrimaryPriceValue = _productResultCQ[".pwc-tile--price-primary"].Text().Replace("\n", "").Replace("€", "").Trim();

                var _productPriceWithDiscountValue = _productResultCQ[".list"].First().Text().Replace("\n", "").Replace("PVPR", "").Trim();

                var _productSecundaryPriceValue = _productResultCQ[".pwc-tile--price-secondary"].Text().Replace("\n", "").Replace("€", "").Trim();
                //var _productSecundaryPriceUnit = _productResultCQ[".col-tile--price-secondary .pwc-m-unit"].Text().Replace("\n", "").Replace("/", "").Trim();
                var _productSecondaryPriceUnit = _productSecundaryPriceValue.IndexOf('/') > -1 ? _productSecundaryPriceValue.Split('/')[1].Replace(" ", "") : "";


                var _productPrice = _productPrimaryPriceValue;
                var _productPriceWeight = _productSecundaryPriceValue;
                var _prodyctUnitRatio = _productSecondaryPriceUnit;
                _productSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
                {
                    Name = _productName,
                    Brand = _productBrand,
                    Price = _productPrice + "€",
                    PriceWithoutDiscount = _productPriceWithDiscountValue != null ? _productPriceWithDiscountValue + "€" : string.Empty,
                    PriceWeight = _productPriceWeight,
                    StoreId = this.StoreId,
                    StoreName = this.StoreName,
                    StoreColor = this.StoreColor,
                    Url = "/produto/" + _productOnlineId + ".html",
                    ViewableUrl = "/produto/" + _productOnlineId + ".html",
                    Weight = _productWeight,
                    ImageUrl = _productImageUrl,
                    PriceLiteral = 0,
                    PriceWeightLiteral = 0,
                    Category = "",
                    FullCategory = "",
                    Unit = _prodyctUnitRatio,
                    OnlineProductId = _productOnlineId
                });
            }
            return _productSearchResultList;
        }

        async Task<LisieStores.Extensibility.ProductSearchResult> GetMetadata(string url)
        {
            try
            {
                var newUrl = url;
                if (newUrl.IndexOf("https://www.continente.pt") > -1)
                    newUrl = newUrl.Replace("https://www.continente.pt", "");

                if (newUrl.ToLower().IndexOf("/stores/") > -1)
                {
                    int _startIndex = newUrl.IndexOf("ProductId=") + "ProductId=".Length;
                    string _productId = newUrl.Substring(_startIndex);
                    if (_productId.IndexOf("(") > -1)
                    {
                        int _startIndex2 = _productId.IndexOf("(");
                        _productId = _productId.Substring(0, _startIndex2);
                        newUrl = "/produto/" + _productId + ".html";
                    }
                }
                //Heroku server 1
                //string _returnedHtml = await FetchUrl("http://puppeteer-lisie.herokuapp.com/getContinenteProductMetadataHtml/" + HttpUtility.UrlEncode(url));
                //Heroku server 2
                //string _returnedHtml = await FetchUrl("http://lisie.herokuapp.com/getContinenteProductMetadataHtml/" + HttpUtility.UrlEncode(url));
                //Localhost server
                //string _returnedHtml= await FetchUrl("http://localhost:3000/getContinenteProductMetadataHtml/" + HttpUtility.UrlEncode(url));

                string _returnedHtml = await FetchUrl("https://www.continente.pt" + newUrl);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Models.AuchanContinenteProductOnlineMetadata.Product _onlineProduct = null;
                string _productBarcode = string.Empty;
                var _productOnlineProductId = string.Empty;

                try
                {

                    //get Barcode
                    int _indexBeg = _returnedHtml.IndexOf("&amp;ean=") + "&amp;ean=".Length;
                    int _indexEndArray = _returnedHtml.IndexOf("%7c", _indexBeg); //in case the barcode value is various barcodes in a array
                    int _indexEnd = _returnedHtml.IndexOf("&amp;", _indexBeg);
                    if (_indexBeg > -1 && _indexEnd > -1 && _indexEnd > _indexBeg)
                    {
                        if((_indexEndArray < _indexEnd) && _indexEndArray != -1)
                        {
                            _indexEnd = _indexEndArray;
                        }
                        _productBarcode = _returnedHtml.Substring(_indexBeg, _indexEnd - _indexBeg);
                    }

                    //get product sku/id
                    _indexBeg = _returnedHtml.IndexOf("id: \"") + "id: \"".Length;
                    _indexEnd = _returnedHtml.IndexOf("\"", _indexBeg);
                    _productOnlineProductId = _returnedHtml.Substring(_indexBeg, _indexEnd - _indexBeg);

                    _indexBeg = _returnedHtml.IndexOf("<script type=\"application/ld+json\">") + "<script type=\"application/ld+json\">".Length;
                    _indexEnd = _returnedHtml.IndexOf("</script>", _indexBeg);
                    string _productJson = _returnedHtml.Substring(_indexBeg, _indexEnd - _indexBeg);

                    _onlineProduct = (Models.AuchanContinenteProductOnlineMetadata.Product)js.Deserialize(_productJson, typeof(Models.AuchanContinenteProductOnlineMetadata.Product));

                }
                catch (Exception ex)
                {
                    Helpers.Logger.Debug(ex.Message);
                }

                LisieStores.Extensibility.ProductSearchResult _productSearchResultData = new LisieStores.Extensibility.ProductSearchResult();

                CQ _productDom = _returnedHtml;

                var _productName = _productDom[".product-name"].First().Text().Replace("\n", "").Trim();
                var _productBrand = _productDom[".ct-pdp--brand"].First().Text().Replace("\n", "").Trim();
                var _productWeight = _productDom[".ct-pdp--unit"].First().Text().Replace("\n", "").Trim();
                var _productImage = _productDom[".ct-product-image"].First().Attr("src");

                //var _productPrimaryPriceValue = _productDom[".ct-tile--price-primary .ct-price-formatted"].Text().Replace("\n", "").Replace("€", "").Trim();
                //var _productSecundaryPriceValue = _productDom[".ct-tile--price-secondary .ct-price-value"].Text().Replace("\n", "").Replace("€", "").Trim();
                //var _productPrimaryPriceUnit = _productDom[".ct-tile--price-primary .ct-m-unit"].Text().Replace("\n", "").Replace("/", "").Trim();
                //var _productSecundaryPriceUnit = _productDom[".ct-tile--price-secondary .ct-m-unit"].Text().Replace("\n", "").Replace("/", "").Trim();

                //var _productPrice = _productPrimaryPriceUnit.ToLower() == "un" ? _productPrimaryPriceValue : _productSecundaryPriceValue;
                //var _productPriceWeight = _productPrimaryPriceUnit.ToLower() == "un" ? _productSecundaryPriceValue : _productPrimaryPriceValue;
                //var _prodyctUnitRatio = _productPrimaryPriceUnit.ToLower() == "un" ? _productSecundaryPriceUnit : _productPrimaryPriceUnit;

                //var _productPriceWeightValue = _productDom["#maincontent > div > div > div.row.no-gutters.product-images-container > div.col-12.col-sm-7.col-md-6.product-name-details > div.product-name-details--wrapper.pwc-border--radius > div.attributes > div.row.no-gutters.prices-add-to-cart-actions > div.col-auto.ct-pdp--prices > div > div > div.pwc-tile--price-secondary.col-tile--price-secondary > span.ct-price-value"].Text().Replace("\n", "").Replace("€", "").Trim();
                var _productPrimaryPriceValue = _productDom[".pwc-tile--price-primary"].First().Text().Replace("\n", "").Replace("€", "").Replace(" ", "");
                var _productPrimaryPriceUnit = _productPrimaryPriceValue.IndexOf('/') > -1 ? _productPrimaryPriceValue.Split('/')[1] : "";

                var _productSecondaryPriceValue = _productDom[".pwc-tile--price-secondary"].First().Text().Replace("\n", "").Replace("€", "").Replace(" ", "");
                var _productSecondaryPriceUnit = _productSecondaryPriceValue.IndexOf('/') > -1 ? _productSecondaryPriceValue.Split('/')[1].Replace(" ", "") : "";

                if (_productSecondaryPriceValue.IndexOf(_productSecondaryPriceUnit) > -1)
                {
                    _productSecondaryPriceValue = _productSecondaryPriceValue.Replace("/" + _productSecondaryPriceUnit, "");
                }

                var _productPrice = string.Empty;
                var _productPriceWeight = string.Empty;
                var _prodyctUnitRatio = string.Empty;

                if (TextTools.IsBarcodeOfWeightType(_productBarcode))
                {
                    _productPrice = _productPrimaryPriceValue;
                    _productPriceWeight = _productPrimaryPriceValue;
                    _prodyctUnitRatio = _productPrimaryPriceUnit.Replace("/", "");

                }
                else
                {
                    _productPrice = _productPrimaryPriceValue;
                    _productPriceWeight = _productSecondaryPriceValue;
                    _prodyctUnitRatio = _productSecondaryPriceUnit.Replace("/", "");
                }

                //Category
                CQ _categories = _productDom[".breadcrumbs-item"];

                List<IDomObject> _categoriesList = _categories.ToList();
                int _productCategoriesCounter = 1;
                string _productCategory = "";
                string _productCategoriesFull = "";
                foreach (IDomObject _category in _categories)
                {
                    CQ _categoryCQ = _category.OuterHTML;
                    string _categoryText = _categoryCQ["span"].Text().Replace("\n", "").Trim();

                    if (_productCategoriesCounter == 3) //Do nothing, this category is irrelevant
                    {
                        _productCategory = _categoryText;
                        _productCategoriesFull = _categoryText + " > ";

                    }
                    else if (_productCategoriesCounter > 3 && !string.IsNullOrEmpty(_categoryText))
                    {
                        if (_productCategoriesCounter != _categories.Length)
                            _productCategoriesFull += _categoryText + " > ";
                        else
                            _productCategoriesFull += _categoryText;
                    }
                    _productCategoriesCounter++;
                }

                //fixe url
                newUrl = newUrl.Replace("?cgid=home", "");
                if (_onlineProduct != null)
                {
                    _productSearchResultData = new LisieStores.Extensibility.ProductSearchResult
                    {
                        Barcode = _productBarcode,
                        Name = _onlineProduct.name.Trim(),
                        Brand = _onlineProduct.brand?.name.Trim() ?? string.Empty,
                        Price = _productPrice.Trim(),
                        PriceWeight = _productPriceWeight.Trim(),
                        StoreId = this.StoreId,
                        StoreName = this.StoreName,
                        StoreColor = this.StoreColor,
                        Url = newUrl,
                        ViewableUrl = newUrl,
                        Weight = _productWeight.Trim(),
                        ImageUrl = _onlineProduct.image[0],
                        PriceLiteral = 0,
                        PriceWeightLiteral = 0,
                        Category = _productCategory.Trim(),
                        FullCategory = _productCategoriesFull.Trim(),
                        Unit = _prodyctUnitRatio.Trim(),
                        OnlineProductId = _onlineProduct.sku.Trim()
                    };
                }
                else
                {
                    _productSearchResultData = new LisieStores.Extensibility.ProductSearchResult
                    {
                        Barcode = _productBarcode,
                        Name = _productName.Trim(),
                        Brand = _productBrand.Trim() ?? string.Empty,
                        Price = _productPrice.Trim(),
                        PriceWeight = _productPriceWeight.Trim(),
                        StoreId = this.StoreId,
                        StoreName = this.StoreName,
                        StoreColor = this.StoreColor,
                        Url = newUrl,
                        ViewableUrl = newUrl,
                        Weight = _productWeight.Trim(),
                        ImageUrl = _productImage,
                        PriceLiteral = 0,
                        PriceWeightLiteral = 0,
                        Category = _productCategory.Trim(),
                        FullCategory = _productCategoriesFull.Trim(),
                        Unit = _prodyctUnitRatio.Trim(),
                        OnlineProductId = _productOnlineProductId.Trim()
                    };
                }

                return _productSearchResultData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        async public Task<LisieStores.Extensibility.ProductSearchResult> GetProductMetadataById(string onlineProductId)
        {
            string _url = "/produto/" + onlineProductId + ".html";
            return await GetMetadata(_url);
        }

        public async Task<ProductSearchResult> FindProductAI(string name, string brand, string weight, string barcode = "")
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(15); // Set a long timeout to prevent request timeout

                    var requestBody = new
                    {
                        prompt = "start a fresh browser instance using playwright with no open pages. Then go to this website, \"https://www.continente.pt\", and follow the instructions bellow in sequential order:\r\n\r\n- if there´s a cookie dialog, accept all cookies\r\n- look for the product with this characteristics\r\n\t" +
                       "- Name: \"" + name + "\r\n\t" +
                       "- Brand: \"" + brand + "\r\n\t" +
                       "- Weight: \"" + weight + "\r\n" +
                       "- extract the product information and output the data in JSON format, using this example for the properties to extract and the JSON schema. If barcode property doesn´t exist in the page, fill it with tan empty value. Only output the json, with no text behind or after\r\n\t-  {\"barcode\": \"5601244500063\",\"name\": \"Leite de Pastagem Meio Gordo\",\"brand\": \"Terra Nostra\",\"url\": \"https://www.elcorteingles.pt/supermercado/B052020616600167-terra-nostra-leite-de-pastagem-meio-gordo-1-l/\",\"imageUrl\": \"https://sgfm.elcorteingles.es/SGFM/dctm/MEDIA03/202109/29/05220912200803____10__1200x1200.jpg\"\"productId\": \"B052020616600167\",\"weight\":\"Quant. Mínima = 600 gr (3 un)\",\"price\": {\"amount\": 1.09,\"currency\": \"EUR\",\"formatted\": \"1,09 €\"},\"pricePerUnit\": {\"amount\": 1.09,\"unit\": \"Litro\",\"formatted\": \"1,09 € / Litro\"},\"\"priceWithoutDiscount\": {\"\"amount\": 1.09,\"\"currency\": \"EUR\",\"\"formatted\": \"1,09 €\"\"},\"quantity\": {\"value\": 1,\"unit\": \"Litro\",\"formatted\": \"1 l\"},\"categories\": [\"Supermercado\",\"Lacticínios e ovos\",\"Leite\",\"Leite UHT\",\"Leite UHT meio gordo\"]}"
                    };
                    //requestBody.prompt = requestBody.prompt.Replace("\n", " ").Replace("\r", " ").Trim();

                    var json = new JavaScriptSerializer().Serialize(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("http://localhost:4000/prompt", content);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //var responseContent = @"{\"answer\":\"```json\n{\n  \"barcode\": \"4072700003649\",\n  \"name\": \"Cerveja c/ álcool branca\",\n  \"brand\": \"Franziskan\",\n  \"weight\": \"Garrafa de 500 ml\",\n  \"url\": \"https://www.intermarche.pt/product/cerveja-c-ou-alcool-branca/4072700003649\",\n  \"imageUrl\": \"https://driveimg1.intermarche.com/pt/Content/images/boitmal/produit/zoom/557C857D9C4BA3F29BBB282C0FDF517E.jpg\",\n  \"productId\": \"4072700003649\",\n  \"price\": {\n    \"amount\": 2.19,\n    \"currency\": \"EUR\",\n    \"formatted\": \"2,19 €\"\n  },\n  \"pricePerUnit\": {\n    \"amount\": 4.38,\n    \"unit\": \"Litro\",\n    \"formatted\": \"4,38 € / Litro\"\n  },\n  \"quantity\": {\n    \"value\": 1,\n    \"unit\": \"Garrafa\",\n    \"formatted\": \"Garrafa de 500 ml\"\n  },\n  \"categories\": [\n    \"Bebidas\",\n    \"Cervejas e Sidras\",\n    \"Cervejas Estrangeiras e Artesanais\"\n  ]\n}\n```\"}";

                    Console.WriteLine(responseContent);

                    var serializer = new JavaScriptSerializer();
                    dynamic result = serializer.Deserialize<dynamic>(responseContent);
                    var _answer = result["answer"];
                    var jsonStartIndex = _answer.IndexOf("```json\n");
                    if (jsonStartIndex != -1)
                    {
                        _answer = _answer.Substring(jsonStartIndex + "```json\n".Length);
                        _answer = _answer.Replace("```", "").Trim();
                    }
                    dynamic _answerDynamic = serializer.Deserialize<dynamic>(_answer);
                    var _barcode = _answerDynamic["barcode"];
                    var _name = _answerDynamic["name"];
                    var _brand = _answerDynamic["brand"];
                    var _weight = _answerDynamic["weight"];
                    var _url = _answerDynamic["url"];
                    var _imageUrl = _answerDynamic["imageUrl"];
                    var _price = _answerDynamic["price"]["amount"];
                    var _priceWeight = _answerDynamic["pricePerUnit"]["amount"];
                    var _priceWeightUnit = _answerDynamic["pricePerUnit"]["unit"];
                    var _categoriesList = _answerDynamic["categories"];
                    var _priceWithoutDiscount = _answerDynamic["priceWithoutDiscount"]["amount"];


                    //List<string> items = new List<string> { "string 1", "string 2", "string 3" };
                    string _categoriesAll = string.Join(" > ", _categoriesList);

                    var _categories = ((object[])_categoriesList).Select(x => x.ToString()).ToList();


                    // Result: "string 1 > string 2 > string 3"
                    return new ProductSearchResult
                    {
                        Barcode = _barcode,
                        Name = _name,
                        Brand = _brand,
                        Weight = _weight,
                        Url = _url.Replace("https://www.intermarche.pt", ""),
                        ImageUrl = _imageUrl,
                        OnlineProductId = string.Empty,
                        Category = _categories[_categories.Count() - 1],
                        Price = _price.ToString(),
                        PriceLiteral = (float)_price,
                        PriceWeightLiteral = (float)_priceWeight,
                        PriceWeight = _priceWeight.ToString(),
                        PriceWithoutDiscount = _priceWithoutDiscount.ToString(),
                        Unit = _priceWeightUnit,
                        StoreId = this.StoreId,
                        StoreName = this.StoreName,
                        FullCategory = _categoriesAll,
                        ViewableUrl = _url,
                        StoreColor = this.StoreColor,

                    };
                    //return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in FindProductAI: " + ex.Message);
                return null;
            }
        }

        public async Task<ProductSearchResult> FindProductAIByBarcode(string barcode)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(15); // Set a long timeout to prevent request timeout

                    var requestBody = new
                    {
                        prompt = "usa o playwright e vai a este website: \"https://www.continente.pt/\" .Usa sempre uma nova instancia de browser " +
                        "sem abas abertas." +
                        "Depois quando acabar de fazer loading a página, procura por este produto com o código de barras \"" + barcode + ". " +
                        "retorna toda a informação possível do produto em formato JSON. Finge que és um browser normal. " +
                        "retorna o json com este esquema: {\"\"barcode\": \"5601244500063\",\"\"name\": \"Leite de Pastagem Meio Gordo\",\"\"brand\": \"Terra Nostra\",\"\"weight\": \"1lt\",\"\"url\": \"https://www.elcorteingles.pt/supermercado/B052020616600167-terra-nostra-leite-de-pastagem-meio-gordo-1-l/\",\"\"imageUrl\": \"https://sgfm.elcorteingles.es/SGFM/dctm/MEDIA03/202109/29/05220912200803____10__1200x1200.jpg\",\"\"productId\": \"B052020616600167\",\"\"price\": {\"\"amount\": 1.09,\"\"currency\": \"EUR\",\"\"formatted\": \"1,09 €\"\"},\"\"pricePerUnit\": {\"\"amount\": 1.09,\"\"unit\": \"Litro\",\"\"formatted\": \"1,09 € / Litro\"\"},\"\"priceWithoutDiscount\": {\"\"amount\": 1.09,\"\"currency\": \"EUR\",\"\"formatted\": \"1,09 €\"\"},\"\"quantity\": {\"\"value\": 1,\"\"unit\": \"Litro\",\"\"formatted\": \"1 l\"\"},\"\"categories\": [\"\"Supermercado\",\"\"Lacticínios e ovos\",\"\"Leite\",\"\"Leite UHT\",\"\"Leite UHT meio gordo\"\"]\"}\"}. Muito importante, não mostrar nenhum texto antes e depois do json, inclusive na propriedade \"answer\", retornar só mesmo o json."
                    };


                    var json = new JavaScriptSerializer().Serialize(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("http://localhost:4000/prompt", content);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //var responseContent = @"{\"answer\":\"```json\n{\n  \"barcode\": \"4072700003649\",\n  \"name\": \"Cerveja c/ álcool branca\",\n  \"brand\": \"Franziskan\",\n  \"weight\": \"Garrafa de 500 ml\",\n  \"url\": \"https://www.intermarche.pt/product/cerveja-c-ou-alcool-branca/4072700003649\",\n  \"imageUrl\": \"https://driveimg1.intermarche.com/pt/Content/images/boitmal/produit/zoom/557C857D9C4BA3F29BBB282C0FDF517E.jpg\",\n  \"productId\": \"4072700003649\",\n  \"price\": {\n    \"amount\": 2.19,\n    \"currency\": \"EUR\",\n    \"formatted\": \"2,19 €\"\n  },\n  \"pricePerUnit\": {\n    \"amount\": 4.38,\n    \"unit\": \"Litro\",\n    \"formatted\": \"4,38 € / Litro\"\n  },\n  \"quantity\": {\n    \"value\": 1,\n    \"unit\": \"Garrafa\",\n    \"formatted\": \"Garrafa de 500 ml\"\n  },\n  \"categories\": [\n    \"Bebidas\",\n    \"Cervejas e Sidras\",\n    \"Cervejas Estrangeiras e Artesanais\"\n  ]\n}\n```\"}";

                    Console.WriteLine(responseContent);

                    var serializer = new JavaScriptSerializer();
                    dynamic result = serializer.Deserialize<dynamic>(responseContent);
                    var _answer = result["answer"];
                    var jsonStartIndex = _answer.IndexOf("```json\n");
                    if (jsonStartIndex != -1)
                    {
                        _answer = _answer.Substring(jsonStartIndex + "```json\n".Length);
                        _answer = _answer.Replace("```", "").Trim();
                    }
                    dynamic _answerDynamic = serializer.Deserialize<dynamic>(_answer);
                    var _barcode = _answerDynamic["barcode"];
                    var _name = _answerDynamic["name"];
                    var _brand = _answerDynamic["brand"];
                    var _weight = _answerDynamic["weight"];
                    var _url = _answerDynamic["url"];
                    var _imageUrl = _answerDynamic["imageUrl"];
                    var _price = _answerDynamic["price"]["amount"];
                    var _priceWeight = _answerDynamic["pricePerUnit"]["amount"];
                    var _priceWeightUnit = _answerDynamic["pricePerUnit"]["unit"];
                    var _categoriesList = _answerDynamic["categories"];
                    var _priceWithoutDiscount = _answerDynamic["priceWithoutDiscount"]["amount"];


                    //List<string> items = new List<string> { "string 1", "string 2", "string 3" };
                    string _categoriesAll = string.Join(" > ", _categoriesList);

                    var _categories = ((object[])_categoriesList).Select(x => x.ToString()).ToList();


                    // Result: "string 1 > string 2 > string 3"
                    return new ProductSearchResult
                    {
                        Barcode = _barcode,
                        Name = _name,
                        Brand = _brand,
                        Weight = _weight,
                        Url = _url.Replace("https://www.intermarche.pt", ""),
                        ImageUrl = _imageUrl,
                        OnlineProductId = string.Empty,
                        Category = _categories[_categories.Count() - 1],
                        Price = _price.ToString(),
                        PriceLiteral = (float)_price,
                        PriceWeightLiteral = (float)_priceWeight,
                        PriceWeight = _priceWeight.ToString(),
                        PriceWithoutDiscount = _priceWithoutDiscount.ToString(),
                        Unit = _priceWeightUnit,
                        StoreId = this.StoreId,
                        StoreName = this.StoreName,
                        FullCategory = _categoriesAll,
                        ViewableUrl = _url,
                        StoreColor = this.StoreColor,

                    };
                    //return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in FindProductAI: " + ex.Message);
                return null;
            }
        }

        public async Task<ProductSearchResult> FindProductAIByText(string name, string brand, string weight)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(15); // Set a long timeout to prevent request timeout

                    var requestBody = new
                    {
                        prompt = "usa o playwright e vai a este website: \"https://www.continente.pt/\" .Usa sempre uma nova instancia de browser " +
                        "sem abas abertas. Se for preciso, Primeiro aceita as cookies. Depois, Onde diz código postal, introduz este valor \"1675-822\". " +
                        "quando aparecer a lista seleciona a loja \"INTERMARCHÉ SUPER FAMÕES\" . Mas se a loja \"Famões\" ou \"INTERMARCHÉ SUPER FAMÕES\" já estiver seleciona, não faças a instrução anterior.  " +
                        "Depois quando acabar de fazer loading a página, procura por este produto com o nome \"" + name + "\", de marca \"" + brand + "\", e de peso \"" + weight + "\".  procura com diversas combinações, até encontrares o mais similar do produto que referi.  " +
                        "retorna toda a informação possível do produto em formato JSON. Finge que és um browser normal. retorna o json com este esquema: {\"\"barcode\": \"5601244500063\",\"\"name\": \"Leite de Pastagem Meio Gordo\",\"\"brand\": \"Terra Nostra\",\"\"weight\": \"1lt\",\"\"url\": \"https://www.elcorteingles.pt/supermercado/B052020616600167-terra-nostra-leite-de-pastagem-meio-gordo-1-l/\",\"\"imageUrl\": \"https://sgfm.elcorteingles.es/SGFM/dctm/MEDIA03/202109/29/05220912200803____10__1200x1200.jpg\",\"\"productId\": \"B052020616600167\",\"\"price\": {\"\"amount\": 1.09,\"\"currency\": \"EUR\",\"\"formatted\": \"1,09 €\"\"},\"\"pricePerUnit\": {\"\"amount\": 1.09,\"\"unit\": \"Litro\",\"\"formatted\": \"1,09 € / Litro\"\"},\"\"priceWithoutDiscount\": {\"\"amount\": 1.09,\"\"currency\": \"EUR\",\"\"formatted\": \"1,09 €\"\"},\"\"quantity\": {\"\"value\": 1,\"\"unit\": \"Litro\",\"\"formatted\": \"1 l\"\"},\"\"categories\": [\"\"Supermercado\",\"\"Lacticínios e ovos\",\"\"Leite\",\"\"Leite UHT\",\"\"Leite UHT meio gordo\"\"]\"}\"}. Muito importante, não mostrar nenhum texto antes e depois do json, inclusive na propriedade \"answer\", retornar só mesmo o json."
                    };

                
                    var json = new JavaScriptSerializer().Serialize(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("http://localhost:4000/prompt", content);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //var responseContent = @"{\"answer\":\"```json\n{\n  \"barcode\": \"4072700003649\",\n  \"name\": \"Cerveja c/ álcool branca\",\n  \"brand\": \"Franziskan\",\n  \"weight\": \"Garrafa de 500 ml\",\n  \"url\": \"https://www.intermarche.pt/product/cerveja-c-ou-alcool-branca/4072700003649\",\n  \"imageUrl\": \"https://driveimg1.intermarche.com/pt/Content/images/boitmal/produit/zoom/557C857D9C4BA3F29BBB282C0FDF517E.jpg\",\n  \"productId\": \"4072700003649\",\n  \"price\": {\n    \"amount\": 2.19,\n    \"currency\": \"EUR\",\n    \"formatted\": \"2,19 €\"\n  },\n  \"pricePerUnit\": {\n    \"amount\": 4.38,\n    \"unit\": \"Litro\",\n    \"formatted\": \"4,38 € / Litro\"\n  },\n  \"quantity\": {\n    \"value\": 1,\n    \"unit\": \"Garrafa\",\n    \"formatted\": \"Garrafa de 500 ml\"\n  },\n  \"categories\": [\n    \"Bebidas\",\n    \"Cervejas e Sidras\",\n    \"Cervejas Estrangeiras e Artesanais\"\n  ]\n}\n```\"}";

                    Console.WriteLine(responseContent);

                    var serializer = new JavaScriptSerializer();
                    dynamic result = serializer.Deserialize<dynamic>(responseContent);
                    var _answer = result["answer"];
                    var jsonStartIndex = _answer.IndexOf("```json\n");
                    if (jsonStartIndex != -1)
                    {
                        _answer = _answer.Substring(jsonStartIndex + "```json\n".Length);
                        _answer = _answer.Replace("```", "").Trim();
                    }
                    dynamic _answerDynamic = serializer.Deserialize<dynamic>(_answer);
                    var _barcode = _answerDynamic["barcode"];
                    var _name = _answerDynamic["name"];
                    var _brand = _answerDynamic["brand"];
                    var _weight = _answerDynamic["weight"];
                    var _url = _answerDynamic["url"];
                    var _imageUrl = _answerDynamic["imageUrl"];
                    var _price = _answerDynamic["price"]["amount"];
                    var _priceWeight = _answerDynamic["pricePerUnit"]["amount"];
                    var _priceWeightUnit = _answerDynamic["pricePerUnit"]["unit"];
                    var _categoriesList = _answerDynamic["categories"];
                    var _priceWithoutDiscount = _answerDynamic["priceWithoutDiscount"]["amount"];


                    //List<string> items = new List<string> { "string 1", "string 2", "string 3" };
                    string _categoriesAll = string.Join(" > ", _categoriesList);

                    var _categories = ((object[])_categoriesList).Select(x => x.ToString()).ToList();


                    // Result: "string 1 > string 2 > string 3"
                    return new ProductSearchResult
                    {
                        Barcode = _barcode,
                        Name = _name,
                        Brand = _brand,
                        Weight = _weight,
                        Url = _url.Replace("https://www.intermarche.pt", ""),
                        ImageUrl = _imageUrl,
                        OnlineProductId = string.Empty,
                        Category = _categories[_categories.Count() - 1],
                        Price = _price.ToString(),
                        PriceLiteral = (float)_price,
                        PriceWeightLiteral = (float)_priceWeight,
                        PriceWeight = _priceWeight.ToString(),
                        PriceWithoutDiscount = _priceWithoutDiscount.ToString(),
                        Unit = _priceWeightUnit,
                        StoreId = this.StoreId,
                        StoreName = this.StoreName,
                        FullCategory = _categoriesAll,
                        ViewableUrl = _url,
                        StoreColor = this.StoreColor,

                    };
                    //return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in FindProductAI: " + ex.Message);
                return null;
            }
        }

    }
}