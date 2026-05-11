using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisieStores.Extensibility
{
    public class ProductSearchResult
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ViewableUrl { get; set; }
        public string Brand { get; set; }
        public string Weight { get; set; }
        public string Price { get; set; }
        public string PriceWithoutDiscount { get; set; }

        public float PriceLiteral { get; set; }

        public string PriceWeight { get; set; }

        public float PriceWeightLiteral { get; set; }

        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreColor { get; set; }
        public string StoreProductId { get; set; } //OBSOLETE

        public string ImageUrl { get; set; }

        public string Category { get; set; }
        public string FullCategory { get; set; }

        public bool IsSeperator { get; set; }
        public string SeparatorTitle { get; set; }

        public string OnlineProductId { get; set; } //THE REAL DEAL
        public string Unit { get; set; }
    }
}
