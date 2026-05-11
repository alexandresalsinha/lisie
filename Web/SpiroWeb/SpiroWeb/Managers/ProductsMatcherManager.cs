namespace SpiroWeb.Managers
{
    public static class ProductsMatcherManager
    {
        //static private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        //static public Interactions Add(string userId, string name, string extra)
        //{
        //    try
        //    {
        //        Interactions _interaction = new Interactions
        //        {
        //            UserId = userId,
        //            Name = name,
        //            Extra = extra,
        //            CreateDate = DateTime.Now
        //        };
        //        db.Interactions.Add(_interaction);
        //        db.SaveChanges();
        //        return _interaction;
        //    }
        //    catch (Exception)
        //    {
        //        Logger.Debug("Error adding user interaction");
        //        return null;
        //    }
        //}

        //static public List<UserProductsList> GetOfUser(string userId, string list)
        //{
        //    //TODO - other lists
        //    if (list == "shoppingList")
        //        return db.UserProductsList.Where(c => c.UserId.Equals(userId)).ToList();
        //    else
        //        return new List<UserProductsList>();
        //}

        //static public List<string> GetUsersIdsWithProductInList(int productId, string list)
        //{

        //    //TODO - other lists
        //    if (list == "shoppingList")
        //        return db.UserProductsList.Where(c => c.ProductId == productId && c.ListName.ToLower() == "in") //TODO - pass to shoppingList
        //            .ToList()
        //            .Select(m => m.UserId).ToList();
        //    else
        //        return new List<string>();
        //}


        //static public bool DeleteOfUser(int userProductId)
        //{
        //    try
        //    {
        //        var _userProduct = db.UserProductsList.Where(c => c.Id == userProductId).FirstOrDefault();

        //        //if is in inventory, remove from it and add do consumed list
        //        if(_userProduct.ListName == "inventory")
        //        {
        //            for (int i = 0; i < _userProduct.Quantity; i++)
        //            {
        //                UserProductsConsumed _UserProductsConsumed = new UserProductsConsumed();
        //                _UserProductsConsumed.ProductId = _userProduct.ProductId;
        //                _UserProductsConsumed.Quantity = 1;
        //                _UserProductsConsumed.UserId = _userProduct.UserId;
        //                _UserProductsConsumed.CreateDate = DateTime.Now;
        //                db.UserProductsConsumed.Add(_UserProductsConsumed);
        //            }

        //        }

        //        if (_userProduct != null)
        //            db.UserProductsList.Remove(_userProduct);
        //        db.SaveChanges();
        //        return true;


        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Debug("error deleting user inventory product: " + ex.Message);
        //        return false;
        //    }

        //}

        //static public bool SubtractQuantity(int userProductId)
        //{
        //    try
        //    {
        //        var _userProducts = db.UserProductsList.Where(c => c.Id == userProductId).FirstOrDefault();
        //        if (_userProducts.Quantity == 1) db.UserProductsList.Remove(_userProducts);
        //        else _userProducts.Quantity--;

        //        db.SaveChanges();
        //        return true;


        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Debug("error deleting user inventory product: " + ex.Message);
        //        return false;

        //    }
        //}



        //static public bool AddQuantity(int userProductId)
        //{
        //    try
        //    {
        //        var _userProducts = db.UserProductsList.Where(c => c.Id == userProductId).FirstOrDefault();

        //        if (_userProducts != null) _userProducts.Quantity++;
        //        else return false;

        //        db.SaveChanges();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Debug("error deleting user inventory product: " + ex.Message);
        //        return false;
        //    }

        //}

        //static public int CheckoutProducts(List<int> userProducsIds, string userId, bool addToInventory)
        //{
        //    try
        //    {
        //        var _userProducts = db.UserProductsList.Where(c => userProducsIds.Contains(c.Id) && c.UserId.Equals(userId) && c.ListName.ToLower() == "in").ToList();
        //        foreach (var _userProductShoppingList in _userProducts)
        //        {

        //            //remove from shopping list and add with same quantity to "Despensa" and to history ans bought
        //            db.UserProductsList.Remove(_userProductShoppingList);

        //            if (addToInventory)
        //            {
        //                //Add to inventory list
        //                UserProductsList _inventoryPoductExists = (from c in db.UserProductsList
        //                                                           where c.ProductId.Equals(_userProductShoppingList.ProductId) &&
        //                                                           c.UserId.Equals(userId) &&
        //                                                           c.ListName.ToLower().Equals("inventory")
        //                                                           select c).FirstOrDefault();

        //                //Exist in User Lists , change quantity
        //                if (_inventoryPoductExists != null)
        //                {
        //                    _inventoryPoductExists.Quantity = _inventoryPoductExists.Quantity + _userProductShoppingList.Quantity;
        //                    db.UserProductsList.Attach(_inventoryPoductExists);
        //                    var entry = db.Entry(_inventoryPoductExists);
        //                    //TO REMEMBER
        //                    entry.Property(y => y.Quantity).IsModified = true;
        //                }
        //                //add new product to user In List
        //                else
        //                {
        //                    UserProductsList _UserProductsList = new UserProductsList();
        //                    _UserProductsList.ProductId = _userProductShoppingList.ProductId;
        //                    _UserProductsList.UserId = userId;
        //                    _UserProductsList.Quantity = 1;
        //                    _UserProductsList.ListName = "inventory";

        //                    db.UserProductsList.Add(_UserProductsList);
        //                }


        //                //Add to History - Inventory 

        //                UserProductsListHistory _UserProductsListHistoryInventory = new UserProductsListHistory();
        //                _UserProductsListHistoryInventory.ProductId = _userProductShoppingList.ProductId;
        //                _UserProductsListHistoryInventory.UserId = userId;
        //                _UserProductsListHistoryInventory.Quantity = _userProductShoppingList.Quantity;
        //                _UserProductsListHistoryInventory.ListName = "inventory";
        //                _UserProductsListHistoryInventory.InsertDate = DateTime.Now;
        //                db.UserProductsListHistory.Add(_UserProductsListHistoryInventory);
        //            }

        //            //add to bought history
        //            UserProductsListHistory _UserProductsListHistoryBought = new UserProductsListHistory();
        //            _UserProductsListHistoryBought.ProductId = _userProductShoppingList.ProductId;
        //            _UserProductsListHistoryBought.UserId = userId;
        //            _UserProductsListHistoryBought.Quantity = _userProductShoppingList.Quantity;
        //            _UserProductsListHistoryBought.ListName = "bought";
        //            _UserProductsListHistoryBought.InsertDate = DateTime.Now;
        //            db.UserProductsListHistory.Add(_UserProductsListHistoryBought);
        //        }

        //        db.SaveChanges();
        //        return _userProducts.Count();

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Debug("error checking out products: " + ex.Message);
        //        return 0;

        //    }
        //}

        //static public List<UserProductListStorePricesModel> GetBuyStoresPrices(string userId, string list)
        //{
        //    List<UserProductListStorePricesModel> _UserProductListStorePricesModelList = new List<UserProductListStorePricesModel>();
        //    try
        //    {
        //        var userShoppingList = from m in db.UserProductsList where m.UserId == userId && m.ListName.ToLower() == "in" select m;
        //        Dictionary<int, double> _storeTotalPrices = new Dictionary<int, double>();
        //        foreach (var productUserList in userShoppingList)
        //        {
        //            var productUserListStores = from m in db.StoreProducts where m.ProductId == productUserList.ProductId select m;
        //            if (productUserListStores.Count() > 0)
        //            {
        //                foreach (var productUserListStore in productUserListStores)
        //                {
        //                    var __UserProductListStorePricesModelList = _UserProductListStorePricesModelList.Where(c => c.StoreId == productUserListStore.StoreId).FirstOrDefault();
        //                    if (__UserProductListStorePricesModelList != null)
        //                    {
        //                        __UserProductListStorePricesModelList.ProductsCounter++;
        //                        __UserProductListStorePricesModelList.TotalPrice += Math.Round(productUserListStore.Price.Value * productUserList.Quantity.Value, 2);
        //                    }
        //                    else
        //                        _UserProductListStorePricesModelList.Add(new UserProductListStorePricesModel { UserId = userId, ListName = "in", StoreId = productUserListStore.StoreId, ProductsCounter = 1, TotalPrice = Math.Round(productUserListStore.Price.Value * productUserList.Quantity.Value, 2)});
        //                }
        //            }
        //        }

        //        foreach (var _UserProductListStorePricesModel in _UserProductListStorePricesModelList)
        //        {
        //            switch (_UserProductListStorePricesModel.StoreId)
        //            {
        //                case 1:
        //                    _UserProductListStorePricesModel.StoreName = "Jumbo";
        //                    break;
        //                case 2:
        //                    _UserProductListStorePricesModel.StoreName = "Continente";
        //                    break;
        //                case 3:
        //                    _UserProductListStorePricesModel.StoreName = "Pingo Doce";
        //                    break;
        //                default:
        //                    _UserProductListStorePricesModel.StoreName = "";
        //                    break;
        //            }
        //        }
        //        return _UserProductListStorePricesModelList;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Debug("error returning user producs list store price totals: " + ex.Message);
        //        return null;

        //    }
        //}


    }
}