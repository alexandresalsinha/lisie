namespace SpiroWeb.Models
{
    public class UserProductListModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public string ListName { get; set; }
        public int Quantity { get; set; }
        public string ItemType { get; set; }
    }
}