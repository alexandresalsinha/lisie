namespace SpiroWeb.Models
{
    public class TotalsInteractionsModel
    {
        public int Users { get; set; }
        public int MonthlyActiveUsers { get; set; }
        public int Products { get; set; }
        public int Interactions { get; set; }

        public int ProductsAddedToConsumed { get; set; }

        public int ProductsAddedToShoppingList { get; set; }
        public int ProductsAddedToInventory { get; set; }
        public int ProductsAddedToBought { get; set; }

        public int LisieHomeBarcodeScanned { get; set; }

        public int LisieHomeProductsAddedToConsumed { get; set; }
        public int LisieHomeProductsAddedToShoppingList { get; set; }
        public int LisieHomeProductsAddedToInventory { get; set; }
        public int LisieHomeProductsAddedToBought { get; set; }
    }
}