using System;

namespace SpiroWeb.Models
{
    public class UserInteractionsModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }

        public DateTime CreateDate { get; set; }
        public bool Confirmed { get; set; }

        public int Interactions { get; set; }

        public string LastInteractionName { get; set; }

        public DateTime LastInteractionDate { get; set; }

        public int ProductsAddedToMarket { get; set; }

        public int ProductsAddedToConsumed { get; set; }

        public int ProductsAddedToShoppingList { get; set; }

        public int LHbarcodeScanned { get; set; }

        public int LHProductsAddedToConsumed { get; set; }
        //TODO - change to UserReports

    }
}