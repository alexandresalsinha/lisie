using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LisieStores.Extensibility
{
    public interface IMarketFetcher
    {
        int StoreId { get; set; }
        string StoreName { get; set; }
        string StoreUrl { get; set; }
        string StoreColor { get; set; }
        Task<List<ProductSearchResult>> GetSearchResults(string searchQuery);
        Task<ProductSearchResult> GetProductMetadata(string url);
        Task<ProductSearchResult> GetProductMetadataById(string onlineProductId);
        Task<ProductSearchResult> GetProductMetadataByBarcode(string barcode);
        Task<ProductSearchResult> FindProductAI(string name, string brand,string weight, string barcode = "");
        Task<ProductSearchResult> ExtractProductInfoAI(string url);
        Task<bool> AddProductsToOnlineStoreCart(List<ProductAddToOnlineStore> products, string userId, string storeUsername, string storePassword);
        string GetProductViewableUrl(string onlineProductId, string url);

    }
}
