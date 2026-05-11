using System;
using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class UserProductsListHistoryModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public string ListName { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> QuantityWeight { get; set; }
        public System.DateTime InsertDate { get; set; }

        public string Name { get; set; }
        public string Weight { get; set; }
        public Nullable<double> Price { get; set; }
        public List<StoreProduct> PriceList { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }

    }

    public class UserProductHistoryModel
    {
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public string Action { get; set; }
        public double TotalPrice { get; set; }
        public List<UserProductsListHistoryModel> UserProductsListHistory { get; set; }
    }
}