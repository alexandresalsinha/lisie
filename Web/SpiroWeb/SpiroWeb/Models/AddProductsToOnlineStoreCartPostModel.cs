using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class AddProductsToOnlineStoreCartPostModel
    {
        public string UserId { get; set; }
        public int StoreId { get; set; }
        public string StoreUsername { get; set; }
        public string StorePassword { get; set; }
        public List<int> UserProductsIds { get; set; }
    }
}