using ClassLibrary1;
using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class ProductPriceUpdatesModel
    {
        public Products Product { get; set; }
        public List<ProductPricesUpdates> PriceUpdates { get; set; }
    }
}