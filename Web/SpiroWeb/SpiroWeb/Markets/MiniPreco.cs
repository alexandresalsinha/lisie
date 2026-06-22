using CsQuery;
using LisieStores.Extensibility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SpiroWeb.Markets
{
    [MarketAttr(5, "Mini Preço", "https://lojaonline.minipreco.pt", "#f45d5d", "https://lojaonline.minipreco.pt/search?text=")]
    public class MiniPreco : Market, IMarketFetcher
    {

        string IMarketFetcher.GetProductViewableUrl(string onlineProductId, string url)
        {
            if (!string.IsNullOrEmpty(onlineProductId))
            {
                //return this.StoreUrl + "/produtos/" + onlineProductId + ".html";
                return string.Empty;
            }
            if (!string.IsNullOrEmpty(url))
            {
                return this.StoreUrl + url;
            }
            return string.Empty;
        }
        async Task<LisieStores.Extensibility.ProductSearchResult> IMarketFetcher.GetProductMetadata(string url)
        {
            //string _htmlReturned = await FetchUrl("https://lojaonline.minipreco.pt/produtos/laticinios-e-ovos/manteigas-margarinas-e-cremes-para-barrar/margarina-e-banha/p/3780");
            try
            {
                string _htmlReturned = await FetchUrl(this.StoreUrl + url);
                if (_htmlReturned != null)
                {
                    CQ _dom = _htmlReturned;

                    CQ _priceWeight = _dom[".average-price"];
                    CQ _priceEl = _dom[".big-price"];

                    string _priceWeightText = _priceWeight.Text();
                    //cleanup _priceWeightText
                    _priceWeightText = _priceWeightText.Replace("\n", "").Replace("\t", "");

                    bool hasDiscount = _priceWeightText.Where(c => c == '(').Count() > 1 ? true : false;
                    //TODO - send in a variable in ProductSearchResult
                    string _priceWeightWithoutDiscountText = hasDiscount ? GetPriceWeightDiscount(_priceWeightText) : string.Empty;
                    if (hasDiscount)
                    {
                        _priceWeightText = _priceWeightText.Replace(RemovePriceWeightDiscountText(_priceWeightText), "");
                    }

                    int _startIndex = _htmlReturned.IndexOf("obj= {");
                    _startIndex += ("obj= ").Length;
                    if (_startIndex > -1)
                    {
                        int _endIndex = _htmlReturned.IndexOf("}", _startIndex);
                        if (_endIndex > _startIndex)
                        {
                            string _jsonText = _htmlReturned.Substring(_startIndex, _endIndex + 1 - _startIndex).Replace("\n", "").Replace("\t", "").Replace("[", "").Replace("]", "").Replace("// Incidencia DIAEC-584, EMF, 201507", "");
                            dynamic _jsonParsed = JObject.Parse(_jsonText);

                            string _name = _jsonParsed.fn;
                            string _price = _jsonParsed.prize;
                            _price = _price.Replace(".", ",");

                            //TODO - refactor code to function
                            if (hasDiscount)
                            {
                                _price = _priceEl.Get(1).InnerText;
                                _price = _price.Replace("\t", "").Replace("\n", "");
                                _price = _price.Substring(0, _price.IndexOf("&nbsp"));
                            }

                            string _json_CategoryName = _jsonParsed.categoryName;

                            string _imageUrl = _jsonParsed.photo;
                            if (_imageUrl.IndexOf("https://lojaonlin") > -1)
                            {
                                _imageUrl = _imageUrl.Replace("https://lojaonlin", "");
                            }
                            if (_imageUrl.IndexOf("https://wwhttps://") > -1)
                            {
                                _imageUrl = _imageUrl.Replace("https://wwhttps://", "https://");
                            }

                            return new LisieStores.Extensibility.ProductSearchResult
                            {
                                Brand = _jsonParsed.brand,
                                Category = GetCategory(_json_CategoryName),
                                ImageUrl = _imageUrl,
                                Name = _jsonParsed.fn,
                                Price = _price,
                                PriceWeight = GetPriceWeight(_priceWeightText),
                                StoreId = this.StoreId,
                                StoreName = this.StoreName,
                                StoreColor = this.StoreColor,
                                StoreProductId = _jsonParsed.productoid,
                                Url = url,
                                ViewableUrl = url,
                                Weight = GetWeight(_name),
                                OnlineProductId = _jsonParsed.productoid,
                                FullCategory = GetFullCategory(_json_CategoryName),
                                Unit = GetPriceUnit(_priceWeightText)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        async public Task<LisieStores.Extensibility.ProductSearchResult> GetProductMetadataById(string onlineProductId)
        {
            return null;
        }

        async Task<List<LisieStores.Extensibility.ProductSearchResult>> IMarketFetcher.GetSearchResults(string searchQuery)
        {
            List<LisieStores.Extensibility.ProductSearchResult> _productSearchResultList = new List<LisieStores.Extensibility.ProductSearchResult>();
            try
            {
                string _SearchResultsHtml = await FetchUrl("https://lojaonline.minipreco.pt/search?text=" + searchQuery.ToLower() + "&x=0&y=0");

                CQ _jumboDom = _SearchResultsHtml;
                CQ _produtos = _jumboDom[".prod_grid"];

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
                        //CQ _productInfo = ((CsQuery.Implementation.DomElement)(_productName[1])).InnerHTML;

                        //string _name = ((CsQuery.Implementation.DomElement)(_productInfo[0])).InnerHTML;
                        string _name = _productResultCQ["a"].First().Attr("title");
                        _name = _name.Replace("\n                            ", "");
                        _name = _name.Replace(" \n\n                    ", "");

                        //string _productBrand = _productResultCQ[".product-item-brand"].First().Text().Replace("\n", "").Trim();
                        string _productBrand = WordFilt(_name);

                        _name = _name.Replace(_productBrand, "");

                        string _productWeight = string.Empty;
                        if (_name.IndexOf('(') > -1)
                        {
                            _productWeight = GetSubstringByString("(", ")", _name);
                        }
                        else
                        {
                            string[] _words = _name.Split(' ');
                            string _weightTmp = _words[_words.Length - 2];
                            _weightTmp += " " + _words[_words.Length - 1];
                            _productWeight = _weightTmp;
                        }
                        _name = _name.Replace(_productWeight, "");

                        _name = _name.Trim(' ');

                        CQ _productPriceCQ = _productResultCQ[".price"].First();
                        string _productPrice = _productPriceCQ.Text().Replace("\n", "").Replace("\t", "").Replace("€", "").Trim(' ') + "€";
                        string _productPriceWithDiscount = string.Empty;
                        string[] _productPriceWords = _productPrice.Split(' ');
                        if (_productPriceWords.Length > 1)
                        {
                            _productPriceWithDiscount = _productPriceWords[0].Replace("€", "").Replace(" ", "") + "€";
                            _productPrice = _productPriceWords[_productPriceWords.Length - 1];
                        }

                        string _productPriceWeight = _productResultCQ[".pricePerKilogram"].First().Text().Replace("\n", "").Trim().Replace("(", "").Replace(")", "").Replace(".", "").Replace("\t", "");
                        //If wieght as more then two prices/wight is beacuse on is the old price, return only two last words
                        string[] _productPriceWeightWords = _productPriceWeight.Split(' ');
                        if (_productPriceWeightWords.Length > 2)
                        {
                            _productPriceWeight = _productPriceWeightWords[_productPriceWeightWords.Length - 2] + " " + _productPriceWeightWords[_productPriceWeightWords.Length - 1];
                        }

                        CQ _imageElem = _productResultCQ["img"];
                        string _imageElemOuterHtml = ((CsQuery.Implementation.DomElement)(_imageElem[0])).OuterHTML;
                        string _imageElemOuterHtmlParsed = _imageElemOuterHtml.Substring(_imageElemOuterHtml.IndexOf("data-original=") + ("data-original=").Length + 1);
                        string _imageElemOuterHtmlParsed2 = _imageElemOuterHtmlParsed.Substring(0, _imageElemOuterHtmlParsed.IndexOf("\"") + ("\"").Length - 1);
                        string _productImageUrl = _imageElemOuterHtmlParsed2;

                        if (!_productUrl.Equals("javascript:void(0);"))
                            _productSearchResultList.Add(new LisieStores.Extensibility.ProductSearchResult
                            {
                                Name = _name,
                                Brand = _productBrand,
                                Price = _productPrice.Replace("€", "").Replace(" ", "") + "€",
                                PriceWithoutDiscount = _productPriceWithDiscount,
                                PriceWeight = _productPriceWeight,
                                StoreName = this.StoreName,
                                StoreId = this.StoreId,
                                StoreColor = this.StoreColor,
                                Url = _productUrl,
                                ViewableUrl = _productUrl,
                                Weight = _productWeight,
                                ImageUrl = _productImageUrl
                            });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                return _productSearchResultList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return _productSearchResultList;
            }
        }

        static string WordFilt(string mystr)
        {
            string _result = string.Empty;
            foreach (char _char in mystr)
            {
                if (String.Equals(_char.ToString(), _char.ToString().ToUpper()))
                {
                    _result += _char;
                }
                if (String.Equals(_char, ' '))
                {
                    _result += _char;
                }
                if (String.Equals(_char.ToString(), _char.ToString().ToLower()))
                {
                    break;
                }
            }

            return _result.Trim(' ');

        }

        public string GetSubstringByString(string a, string b, string c)
        {
            return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
        }

        public string GetWeight(string phrase)
        {
            //Maybe?
            //string _weight = string.Empty;
            //string[] _words = phrase.Split(' ');
            //string _lastWord = _words[_words.Length - 1];
            //string _beforeLastWord = _words[_words.Length - 2];
            //if (char.IsNumber(_lastWord[0]))
            //{
            //    _weight = _lastWord;
            //}
            //else
            //{
            //    if (char.IsNumber(_beforeLastWord[0]))
            //    {
            //        _weight = _beforeLastWord + " " + _lastWord;
            //    }
            //}

            //return _weight;

            string _productWeight = string.Empty;
            if (phrase.IndexOf('(') > -1)
            {
                _productWeight = GetSubstringByString("(", ")", phrase);
            }
            else
            {
                string[] _words = phrase.Split(' ');
                string _weightTmp = _words[_words.Length - 2];
                _weightTmp += " " + _words[_words.Length - 1];
                _productWeight = _weightTmp;
            }
            return _productWeight;
        }

        public string GetPriceWeight(string text)
        {
            string[] _productPriceWeightWords = text.Split(' ');
            string _productPriceWeight = string.Empty;
            if (_productPriceWeightWords.Length > 2)
            {
                _productPriceWeight = _productPriceWeightWords[_productPriceWeightWords.Length - 2] + " " + _productPriceWeightWords[_productPriceWeightWords.Length - 1];
            }
            else
            {
                _productPriceWeight = text.Replace("(", "").Replace(")", "").Replace(".", "");
                string[] _productPriceWeightWordsSplitted = _productPriceWeight.Split('/');
                if (_productPriceWeightWordsSplitted.Length > 1)
                    _productPriceWeight = _productPriceWeightWordsSplitted[0].Replace("€", "");
            }
            return _productPriceWeight;
        }

        public string GetPriceWeightDiscount(string text)
        {
            int firstEndingParenthisis = text.IndexOf(')');
            //check if it´s not the last one, text has no discount price weight
            if (firstEndingParenthisis < text.Length - 1)
            {
                string _priceDiscount = text.Substring(0, firstEndingParenthisis + 1);
                _priceDiscount = text.Replace("(", "").Replace(")", "").Replace(".", "");
                string[] _productPriceWeightWordsSplitted = _priceDiscount.Split('/');
                if (_productPriceWeightWordsSplitted.Length > 1)
                    _priceDiscount = _productPriceWeightWordsSplitted[0].Replace("€", "");
                return _priceDiscount;
            }

            return string.Empty;
        }

        public string RemovePriceWeightDiscountText(string text)
        {
            int firstEndingParenthisis = text.IndexOf(')');
            //check if it´s not the last one, text has no discount price weight
            if (firstEndingParenthisis < text.Length - 1)
            {
                string _priceDiscount = text.Substring(firstEndingParenthisis + 1);
                return text.Replace(_priceDiscount, "");
            }

            return string.Empty;
        }

        public string GetPriceUnit(string text)
        {
            string[] _productPriceWeightWords = text.Split(' ');
            string _productPriceUnit = string.Empty;
            if (_productPriceWeightWords.Length > 2)
            {
                _productPriceUnit = _productPriceWeightWords[_productPriceWeightWords.Length - 2] + " " + _productPriceWeightWords[_productPriceWeightWords.Length - 1];
            }
            else
            {
                _productPriceUnit = text.Replace("(", "").Replace(")", "").Replace(".", "");
                string[] _productPriceWeightWordsSplitted = _productPriceUnit.Split('/');
                if (_productPriceWeightWordsSplitted.Length > 1)
                    _productPriceUnit = _productPriceWeightWordsSplitted[1].ToLower();
            }
            return _productPriceUnit;
        }

        public string GetBrand(string phrase)
        {
            return WordFilt(phrase);
        }

        public string GetCategory(string text)
        {
            int _productCategoriesCounter = 1;
            string _productCategory = "";
            string[] _productCategoriesWords = text.Split(',');
            foreach (var _productCategoryWord in _productCategoriesWords)
            {
                string _finalCategory = _productCategoryWord.Replace("'", "");
                if (_productCategoriesCounter == 2)
                {
                    _productCategory = _finalCategory;
                    break;
                }
                _productCategoriesCounter++;
            }
            return _productCategory;
        }

        public string GetFullCategory(string text)
        {
            int _productCategoriesCounter = 1;
            string _productCategoriesFull = "";
            string[] _productCategoriesWords = text.Split(',');
            foreach (var _productCategoryWord in _productCategoriesWords)
            {
                string _finalCategory = _productCategoryWord.Replace("'", "");
                if (_productCategoriesCounter > 1 && _productCategoriesCounter < _productCategoriesWords.Count())
                {
                    _productCategoriesFull += _finalCategory + " > ";
                }
                else if (_productCategoriesCounter == _productCategoriesWords.Count())
                {
                    _productCategoriesFull += _finalCategory;
                }
                _productCategoriesCounter++;
            }
            return _productCategoriesFull;
        }

        public Task<bool> AddProductsToOnlineStoreCart(List<ProductAddToOnlineStore> products, string userId, string storeUsername, string storePassword)
        {
            throw new NotImplementedException();
        }

        public async Task<LisieStores.Extensibility.ProductSearchResult> GetProductMetadataByBarcode(string barcode)
        {
            return null;
        }

        public Task<ProductSearchResult> FindProductAI(string name, string brand, string weight, string barcode = "")
        {
            throw new NotImplementedException();
        }
    }
}