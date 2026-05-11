namespace SpiroWeb.Models
{
    public class RecipeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Cuisine { get; set; }
        public string ItemType { get; set; }
        public int TotalIngredients { get; set; }
        public int MissingIngredients { get; set; }
        public int ExistingIngredients { get; set; }
        public int InShoppingListIngredients { get; set; }

    }
}