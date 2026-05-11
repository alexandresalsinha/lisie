using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class UserProductIdsListPostModel
    {
        public string UserId { get; set; }
        public List<int> UserProductIds { get; set; }
    }
}