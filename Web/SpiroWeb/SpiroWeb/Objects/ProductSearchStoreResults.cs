using System.Collections.Generic;

namespace SpiroWeb.Objects
{
    public class ProductSearchStoreResults
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public List<ProductSearchResult> ProductSearchResults { get; set; }
    }
}