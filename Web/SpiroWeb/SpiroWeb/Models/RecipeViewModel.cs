using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class RecipeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Cuisine { get; set; }
        public string Category { get; set; }
        public string Yield { get; set; }
        public int Rating { get; set; }
        public decimal TimePreparing { get; set; }
        public decimal TimeCooking { get; set; }
        public decimal TimeReady { get; set; }
        public string Commentary { get; set; }
        public bool Visible { get; set; }
        public List<RecipeDirectionsViewModel> RecipeDirections { get; set; }
        public List<RecipeIngredientViewModel> RecipeIngredients { get; set; }

    }

    public class RecipeDirectionsViewModel
    {
        public int Id { get; set; }
        public int StepNumber { get; set; }
        public string Direction { get; set; }
        public string ItemType { get; set; } //to remove, or create
    }

    public class RecipeIngredientViewModel
    {
        public int Id { get; set; }
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public string Units { get; set; }
        public string Information { get; set; }
        public string ItemType { get; set; } //ingredient or ingredientProduct
        public bool ExistsInInventory { get; set; }
        public bool ExistsInShoppingList { get; set; }
        public RecipeIngredientProductViewModel Product { get; set; }

    }

    public class RecipeIngredientProductViewModel
    {
        public int Id { get; set; }
        public int IngredientId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Weight { get; set; }
        public List<StoreProduct> PriceList { get; set; }
    }
}