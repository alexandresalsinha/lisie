namespace SpiroWeb.Models
{
    public class UserProductListStorePricesModel
    {
        public string UserId { get; set; }
        public string ListName { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public double TotalPrice { get; set; }


        public int ProductsCounter { get; set; }
        public int UserProductsCounter { get; set; }

        public UserProductListCompleteModel StoreUserProduct { get; set; }

        //to put on android side, computation is done over there

        //public List<UserProductListCompleteModel> StoreUserSameProducts { get; set; }
        //public double TotalSamePrice { get; set; }
        //public int ProductsSameCounter { get; set; }

        public string ItemType { get; set; }

    }
}