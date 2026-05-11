using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiroWeb.Managers
{
    public static class StatisticsManager
    {
        static private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        static public bool Record()

        {
            try
            {
                var _users = db.AspNetUsers.Count();
                var _Products = db.Products.Count();
                var _Interactions = db.Interactions.Count();
                var _ProductsAddedToConsumed = db.UserProductsListHistory.Where(c => c.ListName == "consumed").Count();
                var _ProductsAddedToShoppingList = db.UserProductsListHistory.Where(c => c.ListName == "shoppingList").Count();
                var _ProductsAddedToInventory = db.UserProductsListHistory.Where(c => c.ListName == "inventory").Count();
                var _ProductsAddedToBought = db.UserProductsListHistory.Where(c => c.ListName == "bought").Count();
                var _LHBarcodeScanned = db.Interactions.Where(c => c.Name == "LisieHomeBarcodeScanned").Count();
                var _LHProductsAddedToConsumed = db.UserProductsListHistory.Where(c => c.ListName == "consumed" && (c.LisieHome.HasValue && c.LisieHome.Value == true)).Count();
                var _LHProductsAddedToShoppingList = db.UserProductsListHistory.Where(c => c.ListName == "shoppingList" && (c.LisieHome.HasValue && c.LisieHome.Value == true)).Count();
                var _LHProductsAddedToInventory = db.UserProductsListHistory.Where(c => c.ListName == "inventory" && (c.LisieHome.HasValue && c.LisieHome.Value == true)).Count();
                var _LHProductsAddedToBought = db.UserProductsListHistory.Where(c => c.ListName == "bought" && (c.LisieHome.HasValue && c.LisieHome.Value == true)).Count();

                var _oneMonthAgo = DateTime.Now.AddMonths(-1);
                var _MonthlyActiveUsers = db.Interactions.Where(c => c.CreateDate >= _oneMonthAgo).GroupBy(c => c.UserId).Count();

                Statistics _new = new Statistics
                {
                    InsertDate = DateTime.Now,
                    Users = _users,
                    Interactions = _Interactions,
                    Products = _Products,
                    ProductsAddedToConsumed = _ProductsAddedToConsumed,
                    ProductsAddedToShoppingList = _ProductsAddedToShoppingList,
                    ProductsAddedToInventory = _ProductsAddedToInventory,
                    ProductsAddedToBought = _ProductsAddedToBought,
                    LHBarcodesScanned = _LHBarcodeScanned,
                    LHProductsAddedToConsumed = _LHProductsAddedToConsumed,
                    LHProductsAddedToInventory = _LHProductsAddedToInventory,
                    LHProductsAddedToBought = _LHProductsAddedToBought,
                    LHAddedToShoppingList = _LHProductsAddedToShoppingList,
                    UsersMonthlyActive = _MonthlyActiveUsers
                };
                db.Statistics.Add(_new);
                db.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        static public List<Statistics> GetAll()
        {
            try
            {
                return db.Statistics.OrderByDescending(c => c.InsertDate).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        static public Statistics GetCurrent()
        {
            try
            {
                var _users = db.AspNetUsers.Count();
                var _Products = db.Products.Count();
                var _Interactions = db.Interactions.Count();
                var _ProductsAddedToConsumed = db.UserProductsListHistory.Where(c => c.ListName == "consumed").Count();
                var _ProductsAddedToShoppingList = db.UserProductsListHistory.Where(c => c.ListName == "shoppingList").Count();
                var _ProductsAddedToInventory = db.UserProductsListHistory.Where(c => c.ListName == "inventory").Count();
                var _ProductsAddedToBought = db.UserProductsListHistory.Where(c => c.ListName == "bought").Count();
                var _LHBarcodeScanned = db.Interactions.Where(c => c.Name == "LisieHomeBarcodeScanned").Count();
                var _LHProductsAddedToConsumed = db.UserProductsListHistory.Where(c => c.ListName == "consumed" && (c.LisieHome.HasValue && c.LisieHome.Value == true)).Count();
                var _LHProductsAddedToShoppingList = db.UserProductsListHistory.Where(c => c.ListName == "shoppingList" && (c.LisieHome.HasValue && c.LisieHome.Value == true)).Count();
                var _LHProductsAddedToInventory = db.UserProductsListHistory.Where(c => c.ListName == "inventory" && (c.LisieHome.HasValue && c.LisieHome.Value == true)).Count();
                var _LHProductsAddedToBought = db.UserProductsListHistory.Where(c => c.ListName == "bought" && (c.LisieHome.HasValue && c.LisieHome.Value == true)).Count();

                var _oneMonthAgo = DateTime.Now.AddMonths(-1);
                var _MonthlyActiveUsers = db.Interactions.Where(c => c.CreateDate >= _oneMonthAgo).GroupBy(c => c.UserId).Count();

                Statistics _nowStatistics = new Statistics
                {
                    InsertDate = DateTime.Now,
                    Users = _users,
                    Interactions = _Interactions,
                    Products = _Products,
                    ProductsAddedToConsumed = _ProductsAddedToConsumed,
                    ProductsAddedToShoppingList = _ProductsAddedToShoppingList,
                    ProductsAddedToInventory = _ProductsAddedToInventory,
                    ProductsAddedToBought = _ProductsAddedToBought,
                    LHBarcodesScanned = _LHBarcodeScanned,
                    LHProductsAddedToConsumed = _LHProductsAddedToConsumed,
                    LHProductsAddedToInventory = _LHProductsAddedToInventory,
                    LHProductsAddedToBought = _LHProductsAddedToBought,
                    LHAddedToShoppingList = _LHProductsAddedToShoppingList,
                    UsersMonthlyActive = _MonthlyActiveUsers
                };
                return _nowStatistics;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}