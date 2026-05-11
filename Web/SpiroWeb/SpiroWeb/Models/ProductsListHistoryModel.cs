using System;

namespace SpiroWeb.Models
{
    public class ProductsListHistoryModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        //public string UserId { get; set; }
        public string ListName { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> QuantityWeight { get; set; }

        public string Name { get; set; }
        public string Weight { get; set; }

        public string Brand { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }

        public string OldPrice { get; set; }
        public string NewPrice { get; set; }
        public string PriceChange { get; set; } //Up or Down
        public System.DateTime InsertDate { get; set; }

    }
}