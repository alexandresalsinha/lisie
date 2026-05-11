namespace SpiroWeb.Objects
{
    public class ProductSearchResult
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Brand { get; set; }
        public string Weight { get; set; }
        public string Price { get; set; }

        public float PriceLiteral { get; set; }

        public string PriceWeight { get; set; }

        public float PriceWeightLiteral { get; set; }

        public string Store { get; set; }
        public string ImageUrl { get; set; }

        public string Category { get; set; }

        public bool IsSeperator { get; set; }
        public string SeparatorTitle { get; set; }
    }

    public class ProductSearchResultNew
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Brand { get; set; }
        public string Weight { get; set; }
        public string Price { get; set; }

        public float PriceLiteral { get; set; }

        public string PriceWeight { get; set; }

        public float PriceWeightLiteral { get; set; }

        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreProductId { get; set; }

        public string ImageUrl { get; set; }

        public string Category { get; set; }

        public bool IsSeperator { get; set; }
        public string SeparatorTitle { get; set; }
    }
}