namespace SpiroWeb.Models
{
    public class StoreProduct
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string OnlineProductId { get; set; }
        public double Price { get; set; }
        public double PriceBase { get; set; }
        public double PriceRatio { get; set; }
        public double PriceRatioBase { get; set; }
        public string Url { get; set; }
        public string CreatedByUserId { get; set; }
        public bool NeedsUpdate { get; set; }

        //new fields
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Weight { get; set; }
        public string ImageUrl { get; set; }
        public string Unit { get; set; }

        //Added
        public string LastPriceChange { get; set; } //down || up
        public System.DateTime UpdateDate { get; set; }
        public bool IsTemp { get; set; }

    }
}