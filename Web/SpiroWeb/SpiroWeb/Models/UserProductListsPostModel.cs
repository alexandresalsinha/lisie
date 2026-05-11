using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class UserProductListsPostModel
    {
        public string UserId { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public List<string> Lists { get; set; }
        public int Quantity { get; set; }
        public decimal QuantityWeight { get; set; }
    }
}