using System;
using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class UserProductLists
    {
        public string ListName { get; set; }
        public List<UserProductListCategory> Categories { get; set; }
    }

    public class UserProductListCategory
    {
        public string CategoryName { get; set; }
        public List<UserProductListCompleteModel2> Products { get; set; }
    }

    public class UserProductListCompleteModel2
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double QuantityWeight { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Weight { get; set; }
        public Nullable<double> Price { get; set; }
        public List<StoreProduct> PriceList { get; set; }
        public Dictionary<string, double> TotalPriceList { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string ItemType { get; set; }
        public string Url { get; set; }
        public string CreatedByUserId { get; set; }

        public DateTime LastAddedDate { get; set; }
        public bool? IsTemp { get; set; }

    }

    public class UserProductListCompleteTempModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double QuantityWeight { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Weight { get; set; }
        public double StorePrice { get; set; }
        public double StorePriceRatio { get; set; }
        public string StorePriceUnit { get; set; }
        public int StoreId { get; set; }
        public int StoreProductId { get; set; }
        public bool ProductIsTemp { get; set; }
        public bool StoreProductIsTemp { get; set; }
        public string StoreProductCreatedByUserId { get; set; }


        public string Brand { get; set; }
        public string Category { get; set; }
        public string ItemType { get; set; }
        public string Url { get; set; }
        public Nullable<bool> NeedsUpdate { get; set; }
        public string CreatedByUserId { get; set; }
        public string Unit { get; set; }
        public string OnlineProductId { get; set; }


        public DateTime LastAddedDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}