using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class UserProductListsCheckoutPostModel2
    {
        public string UserId { get; set; }
        public List<int> UserProductIds { get; set; }

        public bool AddToInventory { get; set; }
    }
}