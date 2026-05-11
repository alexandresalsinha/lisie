using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class OnlineStoreSearchResults
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public List<LisieStores.Extensibility.ProductSearchResult> Results { get; set; }
    }
}