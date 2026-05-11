using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class IngredientProductsToAddPostModel
    {
        public string UserId { get; set; }
        public List<IngredientProductToAddPostModel> Products { get; set; }
    }

    public class IngredientProductToAddPostModel
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}