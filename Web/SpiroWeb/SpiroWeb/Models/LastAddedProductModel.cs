using System;
using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class LastAddedProductModel
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Weight { get; set; }
        public Nullable<double> Price { get; set; }
        public Dictionary<string, double> PriceList { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string ItemType { get; set; }
        public string Url { get; set; }
    }
}