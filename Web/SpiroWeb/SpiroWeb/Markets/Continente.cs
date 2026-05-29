using CsQuery;
using LisieStores.Extensibility;
using SpiroWeb.Controllers;
using SpiroWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

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
                string _productName = _productResultCQ[".col-tile--description"].Text();
                string _productBrand = _productResultCQ[".col-tile--brand"].First().Text().Replace("\n", "").Trim();
                string _productImageUrl = _productResultCQ[".ct-tile-image"].First().Attr("data-src");
                var _productWeight = _productResultCQ[".col-tile--quantity"].First().Text().Replace("\n", "").Trim();
                var _productPrimaryPriceValue = _productResultCQ[".ct-price-formatted"].Text().Replace("\n", "").Replace("€", "").Trim();

                var _productPriceWithDiscountValue = _productResultCQ[".value.ct-tile--price-value"].First().Attr("content");

                var _productSecundaryPriceValue = _productResultCQ[".ct-price-value"].Text().Replace("\n", "").Replace("€", "").Trim();
                //var _productPrimaryPriceUnit = _productResultCQ[".ct-tile--price-primary .pwc-m-unit"].Text().Replace("\n", "").Replace("/", "").Trim();
                var _productSecundaryPriceUnit = _productResultCQ[".col-tile--price-secondary .pwc-m-unit"].Text().Replace("\n", "").Replace("/", "").Trim();

                var _productPrice = _productPrimaryPriceValue;
                var _productPriceWeight = _productSecundaryPriceValue;
                var _prodyctUnitRatio = _productSecundaryPriceUnit;
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
                        if(_indexEndArray < _indexEnd)
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
                var _productPrimaryPriceUnit = _productPrimaryPriceValue.Split('/')[1];

                var _productSecondaryPriceValue = _productDom[".pwc-tile--price-secondary"].First().Text().Replace("\n", "").Replace("€", "");
                var _productSecondaryPriceUnit = _productSecondaryPriceValue.Split('/')[1].Replace(" ", "");

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
    }
}