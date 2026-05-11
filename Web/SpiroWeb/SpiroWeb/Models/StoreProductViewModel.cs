using System;

namespace SpiroWeb.Models
{
    public class StoreProductViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Weight { get; set; }
        public double StorePrice { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreOnlineProductId { get; set; }
        public string Brand { get; set; }
        public string Url { get; set; }
        public Nullable<bool> NeedsUpdate { get; set; }
        public string CreatedByUserId { get; set; }
        public string Unit { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}