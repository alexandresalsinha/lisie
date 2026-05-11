using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisieStores.Extensibility
{
    public class StoreAddToOnline
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public int TotalProducts { get; set; }

        public double TotalPrice { get; set; }

        public List<ProductAddToOnlineStore> Products { get; set; }
    }

    public class ProductAddToOnlineStore
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string OnlineProductId { get; set; }
        public int UserProductListId { get; set; }
        public int StoreId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }

    public class StoreTotalSavings
    {
        public string Percentage { get; set; }
        public double PercentageValue { get; set; }
        public double Highest { get; set; }

        public double Cheapest { get; set; }
        public double PriceDifference { get; set; }
        public int TotalProducts { get; set; }
    }
}
