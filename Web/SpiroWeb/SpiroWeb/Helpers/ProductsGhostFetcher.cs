using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpiroWeb.Helpers
{
    public class ProductsGhostFetcher
    {
        public ProductsGhostFetcher()
        {

        }
        public async Task<bool> GoGetter()
        {
            string _categoriesHtml = await FetchUrl("https://lojaonline.intermarche.pt/9-sao-domingos-de-rana/");
            CQ _dom = _categoriesHtml;

            CQ _produtos = _dom[".separateur ul li"];
            List<IDomObject> _productsList = _produtos.ToList();

            int _productCounter = 1;
            List<string> _categories = new List<string>();
            foreach (IDomObject _productResult in _productsList)
            {
                if (((CsQuery.Implementation.DomElement)_productResult).ClassName == string.Empty)
                {
                    //string _productUrl = _productResult.Attributes["href"];

                    string s = ((CsQuery.Implementation.DomElement)((CsQuery.Implementation.DomContainer<CsQuery.Implementation.DomElement>)_productResult).FirstElementChild).Attributes["href"];
                    _categories.Add(s);

                }
                _productCounter++;
            }

            //get products of each category
            foreach (var _category in _categories)
            {
                string _categoryHtml = await FetchUrl("https://lojaonline.intermarche.pt" + _category);
                CQ _categoryDom = _categoryHtml;
                CQ _categoryProdutos = _categoryDom[".vignette_produit_info"];
                List<IDomObject> _categoryProdutosList = _categoryProdutos.ToList();
                foreach (var _productOfStore in _categoryProdutosList)
                {
                    int _startIndex = _productOfStore.InnerHTML.IndexOf("idproduit =\"") + ("idproduit =\"").Length;
                    int _endIndex = _productOfStore.InnerHTML.IndexOf("\"", _startIndex);
                    string _idProduct = _productOfStore.InnerHTML.Substring(_startIndex, _endIndex - _startIndex);
                }
            }
            string response = _categoriesHtml;
            return true;
        }

        public async Task<string> FetchUrl(String url)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            string response = await client.GetStringAsync(url);
            return response;
        }
    }
}