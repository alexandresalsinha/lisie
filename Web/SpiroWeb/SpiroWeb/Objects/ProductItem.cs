using System.Collections.Generic;

namespace SpiroWeb.Objects
{
    //OBSOLETE
    public class ProductItem
    {
        public string Barcode { get; set; }
        public List<ProductSearchResult> FilteredResults { get; set; }
        public bool IsToOverwrite { get; set; }
        public List<string> Lists { get; set; }
        public int ProductId { get; set; }

        //SearchUpdateStores
        public string Name { get; set; }
        public string Weight { get; set; }
        public string Brand { get; set; }
        public string ImageUrl { get; set; }
        //SearchUpdateStores

        public string Search { get; set; }
        //public List<ProductSearchResult> SearchResults { get; set; }
        //public List<ProductSearchResult> SelectedResults { get; set; }

        public List<LisieStores.Extensibility.ProductSearchResult> SearchResults { get; set; }
        public List<LisieStores.Extensibility.ProductSearchResult> SelectedResults { get; set; }

        //SearchUpdateStores
        public List<Models.StoreProduct> StoreProducts { get; set; }
        //SearchUpdateStores
        public string UserId { get; set; }
    }

    public class ProductItemNew
    {
        public string Barcode { get; set; }
        public List<ProductSearchResult> FilteredResults { get; set; }
        public bool IsToOverwrite { get; set; }
        public List<string> Lists { get; set; }
        public int ProductId { get; set; }

        //SearchUpdateStores
        public string Name { get; set; }
        public string Weight { get; set; }
        public string Brand { get; set; }
        public string ImageUrl { get; set; }
        //SearchUpdateStores

        public string Search { get; set; }
        public List<LisieStores.Extensibility.ProductSearchResult> SearchResults { get; set; }
        public List<LisieStores.Extensibility.ProductSearchResult> SelectedResults { get; set; }
        public List<int> StoreIdsToRemove { get; set; }


        //SearchUpdateStores
        public List<Models.StoreProduct> StoreProducts { get; set; }
        //SearchUpdateStores
        public string UserId { get; set; }
        public int FirstAddedProductFromStoreId { get; set; }
    }

    public class ProductItemCreate
    {
        public string UserId { get; set; }
        public string Barcode { get; set; }
        public List<string> Lists { get; set; }
        public int FirstAddedProductFromStoreId { get; set; }
        public string Name { get; set; }
        public string Weight { get; set; }
        public string Brand { get; set; }
        public string ImageUrl { get; set; }
        public List<LisieStores.Extensibility.ProductSearchResult> SelectedResults { get; set; }
    }

    public class ProductSimpleItem
    {
        //SearchUpdateStores
        public string Name { get; set; }
        public string UserId { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public string List { get; set; }
    }

    public class ProductSimpleItemV2
    {
        //SearchUpdateStores
        public string Name { get; set; }
        public string UserId { get; set; }
        public int Quantity { get; set; }
        public string ImageBase64 { get; set; }
        public string List { get; set; }
    }

    public class ProducReviewCreate
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public string Info { get; set; }
    }
}