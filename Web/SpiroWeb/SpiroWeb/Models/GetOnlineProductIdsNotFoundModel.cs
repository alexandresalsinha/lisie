using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class GetOnlineProductIdsNotFoundModel
    {
        public int StoreId { get; set; }
        public List<string> OnlineProductIds { get; set; }
    }
}