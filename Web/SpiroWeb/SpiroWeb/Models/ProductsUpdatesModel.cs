using System;

namespace SpiroWeb.Models
{
    public class ProductsUpdatesModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Store { get; set; }
        public int StoreId { get; set; }
        public string StoreUrl { get; set; }
        public string OldPrice { get; set; }
        public string NewPrice { get; set; }
        public DateTime PriceUpdateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool NeedsUpdate { get; set; }
    }
}