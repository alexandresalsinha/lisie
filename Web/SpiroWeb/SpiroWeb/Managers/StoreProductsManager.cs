using ClassLibrary1;
using System.Linq;
using System.Threading.Tasks;

namespace SpiroWeb.Managers
{
    public static class StoreProductsManager
    {

        static public async Task<Products> UpdateMetadata(int storeProductId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                StoreProducts _storeProduct = db.StoreProducts.Where(c => c.Id == storeProductId).FirstOrDefault();

                if (_storeProduct != null)
                {
                    LisieStores.Extensibility.IMarketFetcher _ImarketFetcher = Helpers.Extensibility.GetStoreFetcher(_storeProduct.StoreId);
                    LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = null;
                    if (!string.IsNullOrEmpty(_storeProduct.OnlineProductId))
                        _ProductSearchResult = await _ImarketFetcher.GetProductMetadataById(_storeProduct.OnlineProductId);

                    if (_ProductSearchResult == null && !string.IsNullOrEmpty(_storeProduct.Url))
                        _ProductSearchResult = await _ImarketFetcher.GetProductMetadata(_storeProduct.Url);

                    if (_ProductSearchResult != null)
                    {
                        ProductsManager.CreateOrUpdateStoreProductNew(_ProductSearchResult, _storeProduct.ProductId.Value, _storeProduct.UserId, _storeProduct.StoreId);
                    }
                    return ProductsManager.GetDTOById(_storeProduct.ProductId.Value);
                }

                return null;
            }

        }

        static public bool AcceptTemp(int storeProductId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _storeProduct = db.StoreProducts.Where(c => c.Id == storeProductId).FirstOrDefault();
                if (_storeProduct != null)
                {
                    _storeProduct.IsTemp = false;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        static public bool RefuseTemp(int storeProductId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _storeProduct = db.StoreProducts.Where(c => c.Id == storeProductId).FirstOrDefault();
                if (_storeProduct != null)
                {
                    _storeProduct.IsTemp = true;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }
    }
}