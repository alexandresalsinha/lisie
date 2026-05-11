using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class UserProductListsCheckoutPostModel
    {
        public string UserId { get; set; }
        public List<int> ProductsIds { get; set; }

        public bool AddToInventory { get; set; }
    }
}