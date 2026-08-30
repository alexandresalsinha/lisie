using ClassLibrary1;
using Microsoft.Ajax.Utilities;
using SpiroWeb.Helpers;
using SpiroWeb.Models;
using SpiroWeb.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
//using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SpiroWeb.Managers
{
    public static class UserListsManager
    {
        static private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        static public Interactions Add(string userId, string name, string extra)
        {
            try
            {
                Interactions _interaction = new Interactions
                {
                    UserId = userId,
                    Name = name,
                    Extra = extra,
                    CreateDate = DateTime.Now
                };
                db.Interactions.Add(_interaction);
                db.SaveChanges();
                return _interaction;
            }
            catch (Exception)
            {
                Logger.Debug("Error adding user interaction");
                return null;
            }
        }

        static public List<Models.UserProductListCompleteModel2> Get(string userId, string list)
        {
            UserUpdatePricesRequests _UserUpdatePricesRequests = Managers.UserUpdatePricesRequestsManager.Add(userId);
            List<Models.UserProductListCompleteModel2> _listToReturn2 = new List<Models.UserProductListCompleteModel2>();

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                db.Configuration.AutoDetectChangesEnabled = true;
                db.Set<StoreProducts>().AsNoTracking();
                switch (list.ToLower())
                {
                    case "all":
                        List<Models.UserProductListCompleteModel2> combinedLists = new List<Models.UserProductListCompleteModel2>();


                        var userShoppingList = from m in db.UserProductsList where m.UserId == userId && m.ListName.ToLower() == "in" select m;
                        var userConsumedProducts = from m in db.UserProductsConsumed where m.UserId == userId && m.ActionTakenByUser == null select new SpiroWeb.Models.UserProductListModel { Id = m.Id, UserId = userId, ListName = "consumed", ItemType = "consumed", ProductId = m.ProductId, Quantity = m.Quantity ?? 1 };
                        var userConsumedProductsGrouped = (from m in userConsumedProducts
                                                           group m by new { m.ProductId, m.UserId } into g
                                                           select g);
                        var consumedProductsInnerJoinQuery =
                            from userConsumedProduct in userConsumedProductsGrouped
                            join prod in db.Products on userConsumedProduct.Key.ProductId equals prod.Id
                            select new Models.UserProductListCompleteModel2
                            {
                                ProductId = prod.Id,
                                Quantity = userConsumedProduct.Sum(x => x.Quantity),
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "consumed",
                                Name = prod.Name,
                                Weight = prod.Weight,
                                Category = prod.CategoryString,
                                Price = Math.Round(prod.Price.Value * userConsumedProduct.Sum(x => x.Quantity), 2)
                            };

                        var shoppingListProductsInnerJoinQuery =
                            from userShoppingListProduct in userShoppingList
                            join prod in db.Products on userShoppingListProduct.ProductId equals prod.Id
                            orderby userShoppingListProduct.Id descending
                            select new Models.UserProductListCompleteModel2
                            {
                                Id = userShoppingListProduct.Id,
                                ProductId = prod.Id,
                                Quantity = userShoppingListProduct.Quantity ?? 1,
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "shoppingList",
                                Name = prod.Name,
                                Weight = prod.Weight,
                                Category = prod.CategoryString,
                                //Price = prod.Price
                                Price = Math.Round(prod.Price.Value * userShoppingListProduct.Quantity ?? 1, 2)
                            };

                        var _userProductsSimple = db.UserProductsSimple.Where(c => c.UserId == userId && c.Quantity != 0).OrderByDescending(c => c.UpdateDate).Select(c => new Models.UserProductListCompleteModel2
                        {
                            Id = c.Id,
                            Quantity = c.Quantity,
                            ItemType = "productSimple",
                            Url = c.ImageUrl,
                            Name = c.Name,
                            LastAddedDate = c.CreateDate
                        });

                        //Add consumed
                        if (consumedProductsInnerJoinQuery.Count() > 0)
                        {
                            Models.UserProductListCompleteModel2 _UserProductListCompleteModelConsumedLegend = new Models.UserProductListCompleteModel2
                            {
                                ItemType = "consumedlegend",
                                Name = "Legenda Consumidos"
                            };
                            combinedLists.Add(_UserProductListCompleteModelConsumedLegend);
                        }
                        combinedLists.AddRange(consumedProductsInnerJoinQuery);
                        //Add products simple
                        if (_userProductsSimple.Count() > 0)
                        {
                            combinedLists.AddRange(_userProductsSimple);
                        }
                        //Add products full
                        if (shoppingListProductsInnerJoinQuery.Count() > 0)
                        {

                            Models.UserProductListCompleteModel2 _UserProductListCompleteModel = new Models.UserProductListCompleteModel2
                            {
                                ItemType = "legend",
                                Name = "Legenda"
                            };
                            combinedLists.Add(_UserProductListCompleteModel);



                            combinedLists.AddRange(shoppingListProductsInnerJoinQuery);



                            Models.UserProductListCompleteModel2 _UserProductListCompleteModelBuyOnline = new Models.UserProductListCompleteModel2
                            {
                                ItemType = "buyOnline",
                                Name = "Comprar Online"
                            };
                            combinedLists.Add(_UserProductListCompleteModelBuyOnline);

                            Models.UserProductListCompleteModel2 _UserProductListCompleteModelCheckout = new Models.UserProductListCompleteModel2
                            {
                                ItemType = "checkout",
                                Name = "Confirmar"
                            };
                            combinedLists.Add(_UserProductListCompleteModelCheckout);

                            Models.UserProductListCompleteModel2 _UserProductListCompleteModelShareList = new Models.UserProductListCompleteModel2
                            {
                                ItemType = "shareList",
                                Name = "Partilhar"
                            };
                            combinedLists.Add(_UserProductListCompleteModelShareList);


                        }
                        Models.UserProductListCompleteModel2 _UserProductListCompleteEmpty = new Models.UserProductListCompleteModel2
                        {
                            ItemType = "empty",
                            Name = "Vazio"
                        };
                        combinedLists.Add(_UserProductListCompleteEmpty);

                        //VERY IMPORTANT - CLEAR StorePRoductsCache
                        var count = db.StoreProducts.Local.Count; // number of items in cache (ex. 30)

                        db.StoreProducts.Local.ToList().ForEach(c =>
                        {
                            db.Entry(c).State = EntityState.Detached;
                        });


                        foreach (var productCombined in combinedLists)
                        {
                            var _userProductStores = from m in db.StoreProducts where m.ProductId == productCombined.ProductId select m;
                            db.Set<StoreProducts>().AsNoTracking();
                            if (_userProductStores.Count() > 0)
                            {

                                foreach (var storeProduct in _userProductStores)
                                {
                                    if (productCombined.PriceList == null) productCombined.PriceList = new List<Models.StoreProduct>();
                                    //if (!productCombined.PriceList.ContainsKey(storeProduct.StoreId.ToString()))
                                    productCombined.PriceList.Add(new Models.StoreProduct
                                    {
                                        Id = storeProduct.Id,
                                        Price = Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2),
                                        StoreId = storeProduct.StoreId,
                                        Url = storeProduct.Url,
                                        CreatedByUserId = storeProduct.UserId,
                                        NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
                                    });
                                    //storeProduct.StoreId.ToString(), Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2));

                                    ///if (storeProduct.Stores.Name == "Jumbo") productCombined.Url = storeProduct.Url;
                                }
                            }
                        }
                        return combinedLists;
                    case "shoppinglist":
                        var userProductsList = db.UserProductsList
                            .Include("Products")
                            .Include("AspNetUsers")
                            .Where(u => u.UserId.Equals(userId))
                            .Where(u => u.ListName.ToLower().Equals("in"));

                        foreach (var _product in userProductsList.ToList())
                        {
                            _listToReturn2.Add(new UserProductListCompleteModel2
                            {
                                Id = _product.Id,
                                ProductId = _product.ProductId,
                                Name = _product.Products.Name,
                                Weight = _product.Products.Weight,
                                Quantity = _product.Quantity.Value,
                                Price = _product.Products.Price,
                                Barcode = _product.Products.Barcode,
                                Brand = _product.Products.Brand,
                                Category = _product.Products.CategoryString
                            });
                        }

                        return _listToReturn2;
                    case "inventory":
                        var userInventoryProductsList = db.UserProductsList
                            .Include("Products")
                            .Include("AspNetUsers")
                            .Where(u => u.UserId.Equals(userId))
                            .Where(u => u.ListName.ToLower().Equals("inventory"));

                        Models.UserProductListCompleteModel2 _UserProductListCompleteModelInventory = new Models.UserProductListCompleteModel2
                        {
                            ItemType = "legend",
                            Name = "Legenda"
                        };
                        _listToReturn2.Add(_UserProductListCompleteModelInventory);

                        List<Models.UserProductListCompleteModel2> _listToReverse = new List<Models.UserProductListCompleteModel2>();
                        foreach (var _product in userInventoryProductsList.ToList())
                        {
                            _listToReverse.Add(new Models.UserProductListCompleteModel2()
                            {
                                Id = _product.Id,
                                ProductId = _product.ProductId,
                                ItemType = "inventory",
                                Name = _product.Products.Name,
                                Weight = _product.Products.Weight,
                                Quantity = _product.Quantity.Value,
                                Price = _product.Products.Price,
                                Barcode = _product.Products.Barcode,
                                Brand = _product.Products.Brand,
                                Category = _product.Products.CategoryString
                            });
                        }
                        _listToReverse.Reverse();
                        _listToReturn2.AddRange(_listToReverse);


                        foreach (var productCombined in _listToReturn2)
                        {
                            var _userProducStores = from m in db.StoreProducts where m.ProductId == productCombined.ProductId select m;
                            if (_userProducStores.Count() > 0)
                            {
                                foreach (var storeProduct in _userProducStores)
                                {
                                    if (productCombined.PriceList == null) productCombined.PriceList = new List<Models.StoreProduct>();
                                    productCombined.PriceList.Add(new Models.StoreProduct
                                    {
                                        Id = storeProduct.Id,
                                        Price = Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2),
                                        StoreId = storeProduct.StoreId,
                                        Url = storeProduct.Url,
                                        CreatedByUserId = storeProduct.UserId,
                                        NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
                                    });
                                }
                            }
                        }

                        Models.UserProductListCompleteModel2 _UserProductListCompleteModelMoveToShoopingList = new Models.UserProductListCompleteModel2
                        {
                            ItemType = "moveToShoppingList",
                            Name = "Mover para lista de compras"
                        };
                        _listToReturn2.Add(_UserProductListCompleteModelMoveToShoopingList);

                        return _listToReturn2;
                    case "out":
                        return _listToReturn2;
                    default:
                        return _listToReturn2;
                }
            }
        }

        static public List<Models.UserProductListCompleteModel2> GetV2(string userId, string list)
        {
            try
            {
                UserUpdatePricesRequests _UserUpdatePricesRequests = Managers.UserUpdatePricesRequestsManager.Add(userId);
            }
            catch (Exception)
            {
            }
            List<Models.UserProductListCompleteModel2> _listToReturn = new List<Models.UserProductListCompleteModel2>();

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                //Maybe in future remove this. I think with using it´s obsolete
                //VERY IMPORTANT - TESTING OF IT WORKS WITHOUT THIS
                //db.Configuration.AutoDetectChangesEnabled = true;
                //db.Set<StoreProducts>().AsNoTracking();
                switch (list.ToLower())
                {
                    case "all":
                        //ShoppingList Products Full
                        var _shoppingListProductsSimple = db.UserProductsSimple.Where(c => c.UserId == userId && c.Quantity != 0 && (c.ListName == null || c.ListName.Equals("shoppingList"))).OrderByDescending(c => c.UpdateDate).Select(c => new Models.UserProductListCompleteModel2
                        {
                            Id = c.Id,
                            Quantity = c.Quantity,
                            ItemType = "productSimple",
                            Url = c.ImageUrl,
                            Name = c.Name,
                            LastAddedDate = c.CreateDate
                        });
                        if (_shoppingListProductsSimple.Count() > 0)
                            _listToReturn.AddRange(_shoppingListProductsSimple);

                        var _userShoppingList = db.UserProductsList
                            .Where(m => m.UserId == userId && m.ListName.ToLower() == "in")
                            .Include("Products")
                            .OrderByDescending(c => c.LastAddedDate)
                            .Select(userShoppingListProduct => new Models.UserProductListCompleteModel2
                            {
                                Id = userShoppingListProduct.Id,
                                ProductId = userShoppingListProduct.Products.Id,
                                Quantity = userShoppingListProduct.Quantity ?? 1,
                                Barcode = userShoppingListProduct.Products.Barcode,
                                Brand = userShoppingListProduct.Products.Brand,
                                ItemType = "shoppingList",
                                Name = userShoppingListProduct.Products.Name,
                                Weight = userShoppingListProduct.Products.Weight,
                                Category = userShoppingListProduct.Products.CategoryString,
                                Price = Math.Round(userShoppingListProduct.Products.Price.Value * userShoppingListProduct.Quantity ?? 1, 2)
                            });
                        if (_userShoppingList.Count() > 0)
                            _listToReturn.AddRange(_userShoppingList);

                        //Inventory Product Simple
                        var _userProductsSimpleInventory = db.UserProductsSimple.Where(c => c.UserId == userId && c.Quantity != 0 && c.ListName.Equals("inventory")).OrderByDescending(c => c.UpdateDate).Select(c => new Models.UserProductListCompleteModel2
                        {
                            Id = c.Id,
                            Quantity = c.Quantity,
                            ItemType = "productSimpleInventory",
                            Url = c.ImageUrl,
                            Name = c.Name,
                            LastAddedDate = c.CreateDate
                        });
                        if (_userProductsSimpleInventory.Count() > 0)
                            _listToReturn.AddRange(_userProductsSimpleInventory);

                        //Inventory
                        var _userInventoryProductsList = db.UserProductsList
                           .Include("Products")
                           .Include("AspNetUsers")
                           .Where(u => u.UserId.Equals(userId))
                           .Where(u => u.ListName.ToLower().Equals("inventory"))
                           .OrderByDescending(u => u.LastAddedDate)
                           .Select(_product => new Models.UserProductListCompleteModel2()
                           {
                               Id = _product.Id,
                               ProductId = _product.ProductId,
                               ItemType = "inventory",
                               Name = _product.Products.Name,
                               Weight = _product.Products.Weight,
                               Quantity = _product.Quantity.Value,
                               Price = _product.Products.Price,
                               Barcode = _product.Products.Barcode,
                               Brand = _product.Products.Brand,
                               Category = _product.Products.CategoryString
                           });

                        if (_userInventoryProductsList != null &&
                            _userInventoryProductsList.Count() > 0)
                            _listToReturn.AddRange(_userInventoryProductsList);
                        //VERY IMPORTANT - CLEAR StorePRoductsCache - TESTING OF IT WORKS WITHOUT THIS
                        //var count = db.StoreProducts.Local.Count; // number of items in cache (ex. 30)

                        //db.StoreProducts.Local.ToList().ForEach(c =>
                        //{
                        //    db.Entry(c).State = EntityState.Detached;
                        //});
                        break;
                    case "shoppinglist":
                        //ShoppingList Product Simple
                        var _listProductsSimple = db.UserProductsSimple.Where(c => c.UserId == userId && c.Quantity != 0 && (c.ListName == null || c.ListName.Equals("shoppingList"))).OrderByDescending(c => c.UpdateDate).Select(c => new Models.UserProductListCompleteModel2
                        {
                            Id = c.Id,
                            Quantity = c.Quantity,
                            ItemType = "productSimple",
                            Url = c.ImageUrl,
                            Name = c.Name,
                            LastAddedDate = c.CreateDate
                        });
                        if (_listProductsSimple.Count() > 0)
                            _listToReturn.AddRange(_listProductsSimple);

                        var _shoppingList = db.UserProductsList
                            .Where(m => m.UserId == userId && m.ListName.ToLower() == "in")
                            .Include("Products")
                            .Select(userShoppingListProduct => new Models.UserProductListCompleteModel2
                            {
                                Id = userShoppingListProduct.Id,
                                ProductId = userShoppingListProduct.Products.Id,
                                Quantity = userShoppingListProduct.Quantity ?? 1,
                                Barcode = userShoppingListProduct.Products.Barcode,
                                Brand = userShoppingListProduct.Products.Brand,
                                ItemType = "shoppingList",
                                Name = userShoppingListProduct.Products.Name,
                                Weight = userShoppingListProduct.Products.Weight,
                                Category = userShoppingListProduct.Products.CategoryString,
                                Price = Math.Round(userShoppingListProduct.Products.Price.Value * userShoppingListProduct.Quantity ?? 1, 2)
                            });
                        if (_shoppingList.Count() > 0)
                            _listToReturn.AddRange(_shoppingList);

                        break;
                    case "inventory":
                        //Inventory Product Simple
                        var _ProductsSimpleInventory = db.UserProductsSimple.Where(c => c.UserId == userId && c.Quantity != 0 && c.ListName.Equals("inventory")).OrderByDescending(c => c.UpdateDate).Select(c => new Models.UserProductListCompleteModel2
                        {
                            Id = c.Id,
                            Quantity = c.Quantity,
                            ItemType = "productSimpleInventory",
                            Url = c.ImageUrl,
                            Name = c.Name,
                            LastAddedDate = c.CreateDate
                        });
                        if (_ProductsSimpleInventory.Count() > 0)
                            _listToReturn.AddRange(_ProductsSimpleInventory);

                        var _InventoryProductsList = db.UserProductsList
                          .Include("Products")
                          .Include("AspNetUsers")
                          .Where(u => u.UserId.Equals(userId))
                          .Where(u => u.ListName.ToLower().Equals("inventory"))
                          .OrderByDescending(u => u.LastAddedDate)
                          .Select(_product => new Models.UserProductListCompleteModel2()
                          {
                              Id = _product.Id,
                              ProductId = _product.ProductId,
                              ItemType = "inventory",
                              Name = _product.Products.Name,
                              Weight = _product.Products.Weight,
                              Quantity = _product.Quantity.Value,
                              Price = _product.Products.Price,
                              Barcode = _product.Products.Barcode,
                              Brand = _product.Products.Brand,
                              Category = _product.Products.CategoryString
                          });

                        if (_InventoryProductsList != null &&
                            _InventoryProductsList.Count() > 0)
                            _listToReturn.AddRange(_InventoryProductsList);

                        break;
                    default:
                        break;
                }

                foreach (var productCombined in _listToReturn)
                {
                    var _userProductStores = from m in db.StoreProducts where m.ProductId == productCombined.ProductId orderby m.Price select m;
                    db.Set<StoreProducts>().AsNoTracking();
                    if (_userProductStores.Count() > 0)
                    {
                        foreach (var storeProduct in _userProductStores)
                        {
                            if (productCombined.PriceList == null) productCombined.PriceList = new List<Models.StoreProduct>();
                            productCombined.PriceList.Add(new Models.StoreProduct
                            {
                                Id = storeProduct.Id,
                                Price = Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2),
                                StoreId = storeProduct.StoreId,
                                Url = storeProduct.Url,
                                OnlineProductId = storeProduct.OnlineProductId,
                                CreatedByUserId = storeProduct.UserId,
                                NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
                            });
                        }
                    }
                }
                return _listToReturn;
            }
        }

        //Let´s put this faster
        static public List<Models.UserProductListCompleteModel2> GetV3(string userId)
        {
            try
            {
                UserUpdatePricesRequests _UserUpdatePricesRequests = Managers.UserUpdatePricesRequestsManager.Add(userId);
            }
            catch (Exception)
            {
            }
            List<Models.UserProductListCompleteModel2> _listToReturn = new List<Models.UserProductListCompleteModel2>();

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {

                //ShoppingList Products Full
                var _shoppingListProductsSimple = db.UserProductsSimple.Where(c => c.UserId == userId && c.Quantity != 0 && (c.ListName == null || c.ListName.Equals("shoppingList"))).OrderByDescending(c => c.UpdateDate).Select(c => new Models.UserProductListCompleteModel2
                {
                    Id = c.Id,
                    Quantity = c.Quantity,
                    ItemType = "productSimple",
                    Url = c.ImageUrl,
                    Name = c.Name,
                    LastAddedDate = c.UpdateDate
                });
                if (_shoppingListProductsSimple.Count() > 0)
                    _listToReturn.AddRange(_shoppingListProductsSimple);


                var _tempShoppingList =
                   from userProduct in db.UserProductsList
                   join product in db.Products on userProduct.ProductId equals product.Id
                   join storePrd in db.StoreProducts on userProduct.ProductId equals storePrd.ProductId
                   where userProduct.UserId == userId && userProduct.ListName == "in"
                   //&&
                   //(!product.IsTemp.HasValue || !product.IsTemp.Value || (product.IsTemp.Value == true && product.CreatedByUserId == userId))
                   select new UserProductListCompleteTempModel
                   {
                       Id = userProduct.Id,
                       ProductId = userProduct.ProductId,
                       Quantity = userProduct.Quantity.Value,
                       Barcode = product.Barcode,
                       Brand = product.Brand,
                       ItemType = "shoppingList",
                       Name = product.Name,
                       Category = product.CategoryString,
                       StorePrice = Math.Round(storePrd.Price.Value, 2),
                       StoreId = storePrd.StoreId,
                       StoreProductId = storePrd.Id,
                       StoreProductCreatedByUserId = storePrd.UserId,
                       NeedsUpdate = storePrd.NeedsUpdate,
                       Url = storePrd.Url,
                       CreatedByUserId = storePrd.UserId,
                       Weight = product.Weight,
                       Unit = storePrd.Unit,
                       UpdateDate = storePrd.UpdateDate.HasValue ? storePrd.UpdateDate.Value : DateTime.MinValue,
                       LastAddedDate = userProduct.LastAddedDate.HasValue ? userProduct.LastAddedDate.Value : DateTime.MinValue,
                       OnlineProductId = storePrd.OnlineProductId,
                       StoreProductIsTemp = !storePrd.IsTemp.HasValue ? false : storePrd.IsTemp.Value
                   };

                var _distincShoppingList = _tempShoppingList.GroupBy(c => c.ProductId).ToList();
                foreach (var _dist in _distincShoppingList)
                {

                    UserProductListCompleteTempModel _UserProductListCompleteTempModel = _dist.First();
                    UserProductListCompleteModel2 _UserProductListCompleteModel2New = new UserProductListCompleteModel2
                    {
                        Id = _UserProductListCompleteTempModel.Id,
                        ProductId = _UserProductListCompleteTempModel.ProductId,
                        Quantity = _UserProductListCompleteTempModel.Quantity,
                        Barcode = _UserProductListCompleteTempModel.Barcode,
                        Brand = _UserProductListCompleteTempModel.Brand,
                        ItemType = "shoppingList",
                        Name = _UserProductListCompleteTempModel.Name,
                        Category = _UserProductListCompleteTempModel.Category,
                        Weight = _UserProductListCompleteTempModel.Weight,
                        Price = 0,
                        //PriceList = new List<StoreProduct>(),
                        //PriceList = _dist.Where(c=>c.StoreProductIsTemp == false || c.StoreProductIsTemp == true && c.StoreProductCreatedByUserId == userId).OrderBy(c => c.StorePrice).Select(c => new StoreProduct
                        PriceList = _dist.OrderBy(c => c.StorePrice).Select(c => new StoreProduct
                        {
                            Id = c.StoreProductId,
                            StoreId = c.StoreId,
                            Price = Math.Round(c.StorePrice * _UserProductListCompleteTempModel.Quantity, 2),
                            NeedsUpdate = c.NeedsUpdate.HasValue ? c.NeedsUpdate.Value : true,
                            //Url = c.Url,
                            Url = Helpers.Extensibility.GetStoreFetcher(c.StoreId).GetProductViewableUrl("", c.Url),
                            CreatedByUserId = c.CreatedByUserId,
                            UpdateDate = c.UpdateDate,
                            Unit = c.Unit,
                            OnlineProductId = c.OnlineProductId
                        }).ToList(),
                        LastAddedDate = _UserProductListCompleteTempModel.LastAddedDate
                    };
                    _listToReturn.Add(_UserProductListCompleteModel2New);

                }

                //var _userShoppingList = db.UserProductsList
                //    .Where(m => m.UserId == userId && m.ListName.ToLower() == "in")
                //    .Include("Products")
                //    .OrderByDescending(c => c.LastAddedDate)
                //    .Select(userShoppingListProduct => new Models.UserProductListCompleteModel2
                //    {
                //        Id = userShoppingListProduct.Id,
                //        ProductId = userShoppingListProduct.Products.Id,
                //        Quantity = userShoppingListProduct.Quantity ?? 1,
                //        Barcode = userShoppingListProduct.Products.Barcode,
                //        Brand = userShoppingListProduct.Products.Brand,
                //        ItemType = "shoppingList",
                //        Name = userShoppingListProduct.Products.Name,
                //        Weight = userShoppingListProduct.Products.Weight,
                //        Category = userShoppingListProduct.Products.CategoryString,
                //        Price = Math.Round(userShoppingListProduct.Products.Price.Value * userShoppingListProduct.Quantity ?? 1, 2),
                //        LastAddedDate = userShoppingListProduct.LastAddedDate.HasValue ? userShoppingListProduct.LastAddedDate.Value : DateTime.MinValue,
                //    });
                //if (_userShoppingList.Count() > 0)
                //    _listToReturn.AddRange(_userShoppingList);

                //Inventory Product Simple
                var _userProductsSimpleInventory = db.UserProductsSimple.Where(c => c.UserId == userId && c.Quantity != 0 && c.ListName.Equals("inventory")).OrderByDescending(c => c.UpdateDate).Select(c => new Models.UserProductListCompleteModel2
                {
                    Id = c.Id,
                    Quantity = c.Quantity,
                    ItemType = "productSimpleInventory",
                    Url = c.ImageUrl,
                    Name = c.Name,
                    LastAddedDate = c.UpdateDate
                });
                if (_userProductsSimpleInventory.Count() > 0)
                    _listToReturn.AddRange(_userProductsSimpleInventory);

                //Inventory
                //var _userInventoryProductsList = db.UserProductsList
                //   .Include("Products")
                //   .Include("AspNetUsers")
                //   .Where(u => u.UserId.Equals(userId))
                //   .Where(u => u.ListName.ToLower().Equals("inventory"))
                //   .OrderByDescending(u => u.LastAddedDate)
                //   .Select(_product => new Models.UserProductListCompleteModel2()
                //   {
                //       Id = _product.Id,
                //       ProductId = _product.ProductId,
                //       ItemType = "inventory",
                //       Name = _product.Products.Name,
                //       Weight = _product.QuantityWeight,
                //       Quantity = _product.Quantity.Value,
                //       Price = _product.Products.Price,
                //       Barcode = _product.Products.Barcode,
                //       Brand = _product.Products.Brand,
                //       Category = _product.Products.CategoryString,
                //       LastAddedDate = _product.LastAddedDate.HasValue ? _product.LastAddedDate.Value : DateTime.MinValue
                //   });

                //if (_userInventoryProductsList != null &&
                //    _userInventoryProductsList.Count() > 0)
                //    _listToReturn.AddRange(_userInventoryProductsList);



                //Inventory
                var _tempInventory =
                  from userProduct in db.UserProductsList
                  join product in db.Products on userProduct.ProductId equals product.Id
                  join storePrd in db.StoreProducts on userProduct.ProductId equals storePrd.ProductId
                  where userProduct.UserId == userId && userProduct.ListName == "inventory"
                  //orderby storePrd.Price
                  select new UserProductListCompleteTempModel
                  {
                      Id = userProduct.Id,
                      ProductId = userProduct.ProductId,
                      Quantity = userProduct.Quantity.Value,
                      Barcode = product.Barcode,
                      Brand = product.Brand,
                      ItemType = "inventory",
                      Name = product.Name,
                      Category = product.CategoryString,
                      StorePrice = Math.Round(storePrd.Price.Value, 2),
                      StoreId = storePrd.StoreId,
                      StoreProductId = storePrd.Id,
                      NeedsUpdate = storePrd.NeedsUpdate,
                      Url = storePrd.Url,
                      CreatedByUserId = storePrd.UserId,
                      Weight = product.Weight,
                      Unit = storePrd.Unit,
                      UpdateDate = storePrd.UpdateDate.HasValue ? storePrd.UpdateDate.Value : DateTime.MinValue,
                      LastAddedDate = userProduct.LastAddedDate.HasValue ? userProduct.LastAddedDate.Value : DateTime.MinValue,
                      OnlineProductId = storePrd.OnlineProductId
                  };

                var _distincInventory = _tempInventory.GroupBy(c => c.ProductId).ToList();
                foreach (var _dist in _distincInventory)
                {

                    UserProductListCompleteTempModel _UserProductListCompleteTempModel = _dist.First();
                    UserProductListCompleteModel2 _UserProductListCompleteModel2New = new UserProductListCompleteModel2
                    {
                        Id = _UserProductListCompleteTempModel.Id,
                        ProductId = _UserProductListCompleteTempModel.ProductId,
                        Quantity = _UserProductListCompleteTempModel.Quantity,
                        Barcode = _UserProductListCompleteTempModel.Barcode,
                        Brand = _UserProductListCompleteTempModel.Brand,
                        ItemType = "inventory",
                        Name = _UserProductListCompleteTempModel.Name,
                        Category = _UserProductListCompleteTempModel.Category,
                        Weight = _UserProductListCompleteTempModel.Weight,
                        Price = 0,
                        //PriceList = new List<StoreProduct>(),
                        PriceList = _dist.OrderBy(c => c.StorePrice).Select(c => new StoreProduct
                        {
                            Id = c.StoreProductId,
                            StoreId = c.StoreId,
                            Price = Math.Round(c.StorePrice * _UserProductListCompleteTempModel.Quantity, 2),
                            //Price = c.StorePrice,
                            NeedsUpdate = c.NeedsUpdate.HasValue ? c.NeedsUpdate.Value : true,
                            //Url = c.Url,
                            Url = Helpers.Extensibility.GetStoreFetcher(c.StoreId).GetProductViewableUrl("", c.Url),
                            CreatedByUserId = c.CreatedByUserId,
                            UpdateDate = c.UpdateDate,
                            Unit = c.Unit,
                            OnlineProductId = c.OnlineProductId
                        }).ToList(),
                        LastAddedDate = _UserProductListCompleteTempModel.LastAddedDate
                    };
                    _listToReturn.Add(_UserProductListCompleteModel2New);

                }

                //reorder all by LastAddedDate
                _listToReturn = _listToReturn.OrderByDescending(c => c.LastAddedDate).ToList();


                //foreach (var productCombined in _listToReturn)
                //{
                //    var _userProductStores = from m in db.StoreProducts where m.ProductId == productCombined.ProductId orderby m.Price select m;
                //    db.Set<StoreProducts>().AsNoTracking();
                //    if (_userProductStores.Count() > 0)
                //    {
                //        foreach (var storeProduct in _userProductStores)
                //        {
                //            if (productCombined.PriceList == null) productCombined.PriceList = new List<Models.StoreProduct>();
                //            productCombined.PriceList.Add(new Models.StoreProduct
                //            {
                //                Id = storeProduct.Id,
                //                Price = Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2),
                //                StoreId = storeProduct.StoreId,
                //                Url = storeProduct.Url,
                //                OnlineProductId = storeProduct.OnlineProductId,
                //                CreatedByUserId = storeProduct.UserId,
                //                NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
                //            });
                //        }
                //    }
                //}
                return _listToReturn;
            }
        }

        //with isTemp variations
        static public List<Models.UserProductListCompleteModel2> GetV4(string userId)
        {
            List<Models.UserProductListCompleteModel2> _listToReturn = new List<Models.UserProductListCompleteModel2>();

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {

                //ShoppingList Products Full
                var _shoppingListProductsSimple = db.UserProductsSimple.Where(c => c.UserId == userId && c.Quantity != 0 && (c.ListName == null || c.ListName.Equals("shoppingList"))).OrderByDescending(c => c.UpdateDate).Select(c => new Models.UserProductListCompleteModel2
                {
                    Id = c.Id,
                    Quantity = c.Quantity,
                    ItemType = "productSimple",
                    Url = c.ImageUrl,
                    Name = c.Name,
                    LastAddedDate = c.UpdateDate
                });
                if (_shoppingListProductsSimple.Count() > 0)
                    _listToReturn.AddRange(_shoppingListProductsSimple);


                var _tempShoppingList =
                   from userProduct in db.UserProductsList
                   join product in db.Products on userProduct.ProductId equals product.Id
                   join storePrd in db.StoreProducts on userProduct.ProductId equals storePrd.ProductId
                   where userProduct.UserId == userId && userProduct.ListName == "in"
                   //&&
                   //(!product.IsTemp.HasValue || !product.IsTemp.Value || (product.IsTemp.Value == true && product.CreatedByUserId == userId))
                   select new UserProductListCompleteTempModel
                   {
                       Id = userProduct.Id,
                       ProductId = userProduct.ProductId,
                       Quantity = userProduct.Quantity.Value,
                       Barcode = product.Barcode,
                       Brand = product.Brand,
                       ItemType = "shoppingList",
                       Name = product.Name,
                       Category = product.CategoryString,
                       StorePrice = storePrd.Price.HasValue ? Math.Round(storePrd.Price.Value, 2) : 0,
                       StorePriceRatio = storePrd.PriceRatio.HasValue ? storePrd.PriceRatio.Value : 0,
                       StorePriceUnit = storePrd.Unit,
                       StoreId = storePrd.StoreId,
                       StoreProductId = storePrd.Id,
                       StoreProductCreatedByUserId = storePrd.UserId,
                       NeedsUpdate = storePrd.NeedsUpdate,
                       Url = storePrd.Url,
                       CreatedByUserId = storePrd.UserId,
                       Weight = product.Weight,
                       Unit = storePrd.Unit,
                       UpdateDate = storePrd.UpdateDate.HasValue ? storePrd.UpdateDate.Value : DateTime.MinValue,
                       LastAddedDate = userProduct.LastAddedDate.HasValue ? userProduct.LastAddedDate.Value : DateTime.MinValue,
                       OnlineProductId = storePrd.OnlineProductId,
                       StoreProductIsTemp = !storePrd.IsTemp.HasValue ? false : storePrd.IsTemp.Value,
                       ProductIsTemp = product.IsTemp.HasValue ? product.IsTemp.Value : false
                   };

                var _distincShoppingList = _tempShoppingList.GroupBy(c => c.ProductId).ToList();
                foreach (var _dist in _distincShoppingList)
                {

                    UserProductListCompleteTempModel _UserProductListCompleteTempModel = _dist.First();
                    UserProductListCompleteModel2 _UserProductListCompleteModel2New = new UserProductListCompleteModel2
                    {
                        Id = _UserProductListCompleteTempModel.Id,
                        ProductId = _UserProductListCompleteTempModel.ProductId,
                        Quantity = _UserProductListCompleteTempModel.Quantity,
                        Barcode = _UserProductListCompleteTempModel.Barcode,
                        Brand = _UserProductListCompleteTempModel.Brand,
                        ItemType = "shoppingList",
                        Name = _UserProductListCompleteTempModel.Name,
                        Category = _UserProductListCompleteTempModel.Category,
                        Weight = _UserProductListCompleteTempModel.Weight,
                        Price = 0,
                        IsTemp = _UserProductListCompleteTempModel.ProductIsTemp,
                        //PriceList = new List<StoreProduct>(),
                        PriceList = _dist.Where(c => c.StoreProductIsTemp == false || c.StoreProductIsTemp == true && c.StoreProductCreatedByUserId == userId).OrderBy(c => c.StorePrice).Select(c => new StoreProduct
                        //PriceList = _dist.OrderBy(c => c.StorePrice).Select(c => new StoreProduct
                        {
                            Id = c.StoreProductId,
                            StoreId = c.StoreId,
                            Price = Math.Round(c.StorePrice * _UserProductListCompleteTempModel.Quantity, 2),
                            PriceRatio = Math.Round(c.StorePriceRatio * _UserProductListCompleteTempModel.Quantity, 2),
                            NeedsUpdate = c.NeedsUpdate.HasValue ? c.NeedsUpdate.Value : true,
                            //Url = c.Url,
                            Url = Helpers.Extensibility.GetStoreFetcher(c.StoreId).GetProductViewableUrl("", c.Url),
                            CreatedByUserId = c.CreatedByUserId,
                            UpdateDate = c.UpdateDate,
                            Unit = c.Unit,
                            OnlineProductId = c.OnlineProductId
                        }).ToList(),
                        LastAddedDate = _UserProductListCompleteTempModel.LastAddedDate
                    };
                    _listToReturn.Add(_UserProductListCompleteModel2New);

                }

                //Inventory Product Simple
                var _userProductsSimpleInventory = db.UserProductsSimple.Where(c => c.UserId == userId && c.Quantity != 0 && c.ListName.Equals("inventory")).OrderByDescending(c => c.UpdateDate).Select(c => new Models.UserProductListCompleteModel2
                {
                    Id = c.Id,
                    Quantity = c.Quantity,
                    ItemType = "productSimpleInventory",
                    Url = c.ImageUrl,
                    Name = c.Name,
                    LastAddedDate = c.UpdateDate
                });
                if (_userProductsSimpleInventory.Count() > 0)
                    _listToReturn.AddRange(_userProductsSimpleInventory);

                //Inventory
                var _tempInventory =
                  from userProduct in db.UserProductsList
                  join product in db.Products on userProduct.ProductId equals product.Id
                  join storePrd in db.StoreProducts on userProduct.ProductId equals storePrd.ProductId
                  where userProduct.UserId == userId && userProduct.ListName == "inventory"
                  //&&
                  //(!product.IsTemp.HasValue || !product.IsTemp.Value || (product.IsTemp.Value == true && product.CreatedByUserId == userId))
                  select new UserProductListCompleteTempModel
                  {
                      Id = userProduct.Id,
                      ProductId = userProduct.ProductId,
                      Quantity = userProduct.Quantity.Value,
                      Barcode = product.Barcode,
                      Brand = product.Brand,
                      ItemType = "inventory",
                      Name = product.Name,
                      Category = product.CategoryString,
                      StorePrice = Math.Round(storePrd.Price.Value, 2),
                      StoreId = storePrd.StoreId,
                      StoreProductId = storePrd.Id,
                      NeedsUpdate = storePrd.NeedsUpdate,
                      Url = storePrd.Url,
                      CreatedByUserId = storePrd.UserId,
                      Weight = product.Weight,
                      Unit = storePrd.Unit,
                      UpdateDate = storePrd.UpdateDate.HasValue ? storePrd.UpdateDate.Value : DateTime.MinValue,
                      LastAddedDate = userProduct.LastAddedDate.HasValue ? userProduct.LastAddedDate.Value : DateTime.MinValue,
                      OnlineProductId = storePrd.OnlineProductId,
                      StoreProductIsTemp = !storePrd.IsTemp.HasValue ? false : storePrd.IsTemp.Value,
                      StoreProductCreatedByUserId = storePrd.UserId,
                      ProductIsTemp = product.IsTemp.HasValue ? product.IsTemp.Value : false
                  };

                var _distincInventory = _tempInventory.GroupBy(c => c.ProductId).ToList();
                foreach (var _dist in _distincInventory)
                {

                    UserProductListCompleteTempModel _UserProductListCompleteTempModel = _dist.First();
                    UserProductListCompleteModel2 _UserProductListCompleteModel2New = new UserProductListCompleteModel2
                    {
                        Id = _UserProductListCompleteTempModel.Id,
                        ProductId = _UserProductListCompleteTempModel.ProductId,
                        Quantity = _UserProductListCompleteTempModel.Quantity,
                        Barcode = _UserProductListCompleteTempModel.Barcode,
                        Brand = _UserProductListCompleteTempModel.Brand,
                        ItemType = "inventory",
                        Name = _UserProductListCompleteTempModel.Name,
                        Category = _UserProductListCompleteTempModel.Category,
                        Weight = _UserProductListCompleteTempModel.Weight,
                        Price = 0,
                        IsTemp = _UserProductListCompleteTempModel.ProductIsTemp,
                        //PriceList = new List<StoreProduct>(),
                        PriceList = _dist.Where(c => c.StoreProductIsTemp == false || c.StoreProductIsTemp == true && c.StoreProductCreatedByUserId == userId).OrderBy(c => c.StorePrice).Select(c => new StoreProduct
                        //PriceList = _dist.OrderBy(c => c.StorePrice).Select(c => new StoreProduct
                        {
                            Id = c.StoreProductId,
                            StoreId = c.StoreId,
                            Price = Math.Round(c.StorePrice * _UserProductListCompleteTempModel.Quantity, 2),
                            //Price = c.StorePrice,
                            NeedsUpdate = c.NeedsUpdate.HasValue ? c.NeedsUpdate.Value : true,
                            //Url = c.Url,
                            Url = Helpers.Extensibility.GetStoreFetcher(c.StoreId).GetProductViewableUrl("", c.Url),
                            CreatedByUserId = c.CreatedByUserId,
                            UpdateDate = c.UpdateDate,
                            Unit = c.Unit,
                            OnlineProductId = c.OnlineProductId
                        }).ToList(),
                        LastAddedDate = _UserProductListCompleteTempModel.LastAddedDate
                    };
                    _listToReturn.Add(_UserProductListCompleteModel2New);

                }

                //reorder all by LastAddedDate
                _listToReturn = _listToReturn.OrderByDescending(c => c.LastAddedDate).ToList();

                return _listToReturn;
            }
        }

        /// <summary>
        /// based on FetV4
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        static public List<Models.UserProductListCompleteModel2> GetUserListsLastAdded(string userId)
        {
            List<Models.UserProductListCompleteModel2> _listToReturn = new List<Models.UserProductListCompleteModel2>();

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {

                //ShoppingList Products Full
                var _shoppingListProductsSimple = db.UserProductsSimple.Where(c => c.UserId == userId && c.Quantity != 0 && (c.ListName == null || c.ListName.Equals("shoppingList"))).OrderByDescending(c => c.UpdateDate).Select(c => new Models.UserProductListCompleteModel2
                {
                    Id = c.Id,
                    Quantity = c.Quantity,
                    ItemType = "productSimple",
                    Url = c.ImageUrl,
                    Name = c.Name,
                    LastAddedDate = c.UpdateDate
                });
                if (_shoppingListProductsSimple.Count() > 0)
                    _listToReturn.AddRange(_shoppingListProductsSimple);


                var _tempShoppingList =
                   from userProduct in db.UserProductsList
                   join product in db.Products on userProduct.ProductId equals product.Id
                   join storePrd in db.StoreProducts on userProduct.ProductId equals storePrd.ProductId
                   where userProduct.UserId == userId && userProduct.ListName == "in"
                   //&&
                   //(!product.IsTemp.HasValue || !product.IsTemp.Value || (product.IsTemp.Value == true && product.CreatedByUserId == userId))
                   select new UserProductListCompleteTempModel
                   {
                       Id = userProduct.Id,
                       ProductId = userProduct.ProductId,
                       Quantity = userProduct.Quantity.Value,
                       QuantityWeight = userProduct.QuantityWeight.HasValue ? userProduct.QuantityWeight.Value : 1,
                       Barcode = product.Barcode,
                       Brand = product.Brand,
                       ItemType = "shoppingList",
                       Name = product.Name,
                       Category = product.CategoryString,
                       StorePrice = storePrd.Price.HasValue ? storePrd.Price.Value : 0,
                       StorePriceRatio = storePrd.PriceRatio.HasValue ? storePrd.PriceRatio.Value : 1,
                       StorePriceUnit = storePrd.Unit,
                       StoreId = storePrd.StoreId,
                       StoreProductId = storePrd.Id,
                       StoreProductCreatedByUserId = storePrd.UserId,
                       NeedsUpdate = storePrd.NeedsUpdate,
                       Url = storePrd.Url,
                       CreatedByUserId = storePrd.UserId,
                       Weight = product.Weight,
                       Unit = storePrd.Unit,
                       UpdateDate = storePrd.UpdateDate.HasValue ? storePrd.UpdateDate.Value : DateTime.MinValue,
                       LastAddedDate = userProduct.LastAddedDate.HasValue ? userProduct.LastAddedDate.Value : DateTime.MinValue,
                       OnlineProductId = storePrd.OnlineProductId,
                       StoreProductIsTemp = !storePrd.IsTemp.HasValue ? false : storePrd.IsTemp.Value,
                       ProductIsTemp = product.IsTemp.HasValue ? product.IsTemp.Value : false
                   };

                var _distincShoppingList = _tempShoppingList.GroupBy(c => c.ProductId).ToList();
                foreach (var _dist in _distincShoppingList)
                {

                    UserProductListCompleteTempModel _UserProductListCompleteTempModel = _dist.First();
                    UserProductListCompleteModel2 _UserProductListCompleteModel2New = new UserProductListCompleteModel2
                    {
                        Id = _UserProductListCompleteTempModel.Id,
                        ProductId = _UserProductListCompleteTempModel.ProductId,
                        Quantity = _UserProductListCompleteTempModel.Quantity,
                        QuantityWeight = _UserProductListCompleteTempModel.QuantityWeight,
                        Barcode = _UserProductListCompleteTempModel.Barcode,
                        Brand = _UserProductListCompleteTempModel.Brand,
                        ItemType = "shoppingList",
                        Name = _UserProductListCompleteTempModel.Name,
                        Category = _UserProductListCompleteTempModel.Category,
                        Weight = _UserProductListCompleteTempModel.Weight,
                        Price = 0,
                        IsTemp = _UserProductListCompleteTempModel.ProductIsTemp,
                        //PriceList = new List<StoreProduct>(),
                        PriceList = _dist.Where(c => c.StoreProductIsTemp == false || c.StoreProductIsTemp == true && c.StoreProductCreatedByUserId == userId).OrderBy(c => c.StorePrice).Select(c => new StoreProduct
                        //PriceList = _dist.OrderBy(c => c.StorePrice).Select(c => new StoreProduct
                        {
                            Id = c.StoreProductId,
                            StoreId = c.StoreId,
                            Price = Math.Round(c.StorePrice * _UserProductListCompleteTempModel.Quantity, 2),
                            PriceBase = Math.Round(c.StorePrice, 2),
                            PriceRatioBase = Math.Round(c.StorePriceRatio, 2),
                            PriceRatio = Math.Round(c.StorePriceRatio * _UserProductListCompleteTempModel.QuantityWeight, 2),
                            NeedsUpdate = c.NeedsUpdate.HasValue ? c.NeedsUpdate.Value : true,
                            //Url = c.Url,
                            Url = Helpers.Extensibility.GetStoreFetcher(c.StoreId).GetProductViewableUrl("", c.Url),
                            CreatedByUserId = c.CreatedByUserId,
                            UpdateDate = c.UpdateDate,
                            Unit = c.Unit,
                            OnlineProductId = c.OnlineProductId
                        }).ToList(),
                        LastAddedDate = _UserProductListCompleteTempModel.LastAddedDate
                    };
                    _listToReturn.Add(_UserProductListCompleteModel2New);

                }

                //Inventory Product Simple
                var _userProductsSimpleInventory = db.UserProductsSimple.Where(c => c.UserId == userId && c.Quantity != 0 && c.ListName.Equals("inventory")).OrderByDescending(c => c.UpdateDate).Select(c => new Models.UserProductListCompleteModel2
                {
                    Id = c.Id,
                    Quantity = c.Quantity,
                    ItemType = "productSimpleInventory",
                    Url = c.ImageUrl,
                    Name = c.Name,
                    LastAddedDate = c.UpdateDate
                });
                if (_userProductsSimpleInventory.Count() > 0)
                    _listToReturn.AddRange(_userProductsSimpleInventory);

                //Inventory
                var _tempInventory =
                  from userProduct in db.UserProductsList
                  join product in db.Products on userProduct.ProductId equals product.Id
                  join storePrd in db.StoreProducts on userProduct.ProductId equals storePrd.ProductId
                  where userProduct.UserId == userId && userProduct.ListName == "inventory"
                  //&&
                  //(!product.IsTemp.HasValue || !product.IsTemp.Value || (product.IsTemp.Value == true && product.CreatedByUserId == userId))
                  select new UserProductListCompleteTempModel
                  {
                      Id = userProduct.Id,
                      ProductId = userProduct.ProductId,
                      Quantity = userProduct.Quantity.Value,
                      QuantityWeight = userProduct.QuantityWeight.HasValue ? userProduct.QuantityWeight.Value : 1,
                      Barcode = product.Barcode,
                      Brand = product.Brand,
                      ItemType = "inventory",
                      Name = product.Name,
                      Category = product.CategoryString,
                      StorePrice = storePrd.Price.HasValue ? storePrd.Price.Value : 0,
                      StorePriceRatio = storePrd.PriceRatio.HasValue ? storePrd.PriceRatio.Value : 0,
                      StoreId = storePrd.StoreId,
                      StoreProductId = storePrd.Id,
                      NeedsUpdate = storePrd.NeedsUpdate,
                      Url = storePrd.Url,
                      CreatedByUserId = storePrd.UserId,
                      Weight = product.Weight,
                      Unit = storePrd.Unit,
                      UpdateDate = storePrd.UpdateDate.HasValue ? storePrd.UpdateDate.Value : DateTime.MinValue,
                      LastAddedDate = userProduct.LastAddedDate.HasValue ? userProduct.LastAddedDate.Value : DateTime.MinValue,
                      OnlineProductId = storePrd.OnlineProductId,
                      StoreProductIsTemp = !storePrd.IsTemp.HasValue ? false : storePrd.IsTemp.Value,
                      StoreProductCreatedByUserId = storePrd.UserId,
                      ProductIsTemp = product.IsTemp.HasValue ? product.IsTemp.Value : false
                  };

                var _distincInventory = _tempInventory.GroupBy(c => c.ProductId).ToList();
                foreach (var _dist in _distincInventory)
                {

                    UserProductListCompleteTempModel _UserProductListCompleteTempModel = _dist.First();
                    UserProductListCompleteModel2 _UserProductListCompleteModel2New = new UserProductListCompleteModel2
                    {
                        Id = _UserProductListCompleteTempModel.Id,
                        ProductId = _UserProductListCompleteTempModel.ProductId,
                        Quantity = _UserProductListCompleteTempModel.Quantity,
                        Barcode = _UserProductListCompleteTempModel.Barcode,
                        Brand = _UserProductListCompleteTempModel.Brand,
                        ItemType = "inventory",
                        Name = _UserProductListCompleteTempModel.Name,
                        Category = _UserProductListCompleteTempModel.Category,
                        Weight = _UserProductListCompleteTempModel.Weight,
                        Price = 0,
                        IsTemp = _UserProductListCompleteTempModel.ProductIsTemp,
                        //PriceList = new List<StoreProduct>(),
                        PriceList = _dist.Where(c => c.StoreProductIsTemp == false || c.StoreProductIsTemp == true && c.StoreProductCreatedByUserId == userId).OrderBy(c => c.StorePrice).Select(c => new StoreProduct
                        //PriceList = _dist.OrderBy(c => c.StorePrice).Select(c => new StoreProduct
                        {
                            Id = c.StoreProductId,
                            StoreId = c.StoreId,
                            Price = Math.Round(c.StorePrice * _UserProductListCompleteTempModel.Quantity, 2),
                            PriceBase = Math.Round(c.StorePrice, 2),
                            PriceRatioBase = Math.Round(c.StorePriceRatio, 2),
                            PriceRatio = Math.Round(c.StorePriceRatio * _UserProductListCompleteTempModel.QuantityWeight, 2),
                            NeedsUpdate = c.NeedsUpdate.HasValue ? c.NeedsUpdate.Value : true,
                            //Url = c.Url,
                            Url = Helpers.Extensibility.GetStoreFetcher(c.StoreId).GetProductViewableUrl("", c.Url),
                            CreatedByUserId = c.CreatedByUserId,
                            UpdateDate = c.UpdateDate,
                            Unit = c.Unit,
                            OnlineProductId = c.OnlineProductId
                        }).ToList(),
                        LastAddedDate = _UserProductListCompleteTempModel.LastAddedDate
                    };
                    _listToReturn.Add(_UserProductListCompleteModel2New);

                }

                //reorder all by LastAddedDate
                _listToReturn = _listToReturn.OrderByDescending(c => c.LastAddedDate).ToList();

                return _listToReturn;
            }
        }

        //With categories
        static public List<Models.UserProductLists> GetV5(string userId)
        {
            List<Models.UserProductListCompleteModel2> _shoppingListToReturn = new List<Models.UserProductListCompleteModel2>();
            List<Models.UserProductListCompleteModel2> _inventoryListToReturn = new List<Models.UserProductListCompleteModel2>();
            List<Models.UserProductLists> _listsToReturn = new List<Models.UserProductLists>();
            List<Models.UserProductLists> _lists = new List<UserProductLists>();

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                UserProductLists _shoppingList = new UserProductLists
                {
                    ListName = "shoppingList",
                    Categories = new List<UserProductListCategory>()
                };
                UserProductLists _inventoryList = new UserProductLists
                {
                    ListName = "inventory",
                    Categories = new List<UserProductListCategory>()
                };
                _lists.Add(_shoppingList);
                _lists.Add(_inventoryList);

                //ShoppingList Products Full
                var _shoppingListProductsSimple = db.UserProductsSimple.Where(c => c.UserId == userId && c.Quantity != 0 && (c.ListName == null || c.ListName.Equals("shoppingList"))).OrderByDescending(c => c.UpdateDate).Select(c => new Models.UserProductListCompleteModel2
                {
                    Id = c.Id,
                    Quantity = c.Quantity,
                    ItemType = "productSimple",
                    Url = c.ImageUrl,
                    Name = c.Name,
                    LastAddedDate = c.UpdateDate,
                    Category = "Notas"
                });
                if (_shoppingListProductsSimple.Count() > 0)
                    _shoppingListToReturn.AddRange(_shoppingListProductsSimple);


                var _tempShoppingList =
                   from userProduct in db.UserProductsList
                   join product in db.Products on userProduct.ProductId equals product.Id
                   join storePrd in db.StoreProducts on userProduct.ProductId equals storePrd.ProductId
                   where userProduct.UserId == userId && userProduct.ListName == "in"
                   //&&
                   //(!product.IsTemp.HasValue || !product.IsTemp.Value || (product.IsTemp.Value == true && product.CreatedByUserId == userId))
                   select new UserProductListCompleteTempModel
                   {
                       Id = userProduct.Id,
                       ProductId = userProduct.ProductId,
                       Quantity = userProduct.Quantity.Value,
                       QuantityWeight = userProduct.QuantityWeight.HasValue ? userProduct.QuantityWeight.Value : 1,
                       Barcode = product.Barcode,
                       Brand = product.Brand,
                       ItemType = "shoppingList",
                       Name = product.Name,
                       Category = product.CategoryString,
                       StorePrice = storePrd.Price.HasValue ? storePrd.Price.Value : 0,
                       StorePriceRatio = storePrd.PriceRatio.HasValue ? storePrd.PriceRatio.Value : 1,
                       StorePriceUnit = storePrd.Unit,
                       StoreId = storePrd.StoreId,
                       StoreProductId = storePrd.Id,
                       StoreProductCreatedByUserId = storePrd.UserId,
                       NeedsUpdate = storePrd.NeedsUpdate,
                       Url = storePrd.Url,
                       CreatedByUserId = storePrd.UserId,
                       Weight = product.Weight,
                       Unit = storePrd.Unit,
                       UpdateDate = storePrd.UpdateDate.HasValue ? storePrd.UpdateDate.Value : DateTime.MinValue,
                       LastAddedDate = userProduct.LastAddedDate.HasValue ? userProduct.LastAddedDate.Value : DateTime.MinValue,
                       OnlineProductId = storePrd.OnlineProductId,
                       StoreProductIsTemp = !storePrd.IsTemp.HasValue ? false : storePrd.IsTemp.Value,
                       ProductIsTemp = product.IsTemp.HasValue ? product.IsTemp.Value : false
                   };

                var _distincShoppingList = _tempShoppingList.GroupBy(c => c.ProductId).ToList();
                foreach (var _dist in _distincShoppingList)
                {

                    UserProductListCompleteTempModel _UserProductListCompleteTempModel = _dist.First();
                    UserProductListCompleteModel2 _UserProductListCompleteModel2New = new UserProductListCompleteModel2
                    {
                        Id = _UserProductListCompleteTempModel.Id,
                        ProductId = _UserProductListCompleteTempModel.ProductId,
                        Quantity = _UserProductListCompleteTempModel.Quantity,
                        QuantityWeight = _UserProductListCompleteTempModel.QuantityWeight,
                        Barcode = _UserProductListCompleteTempModel.Barcode,
                        Brand = _UserProductListCompleteTempModel.Brand,
                        ItemType = "shoppingList",
                        Name = _UserProductListCompleteTempModel.Name,
                        Category = _UserProductListCompleteTempModel.Category,
                        Weight = _UserProductListCompleteTempModel.Weight,
                        Price = 0,
                        IsTemp = _UserProductListCompleteTempModel.ProductIsTemp,
                        //PriceList = new List<StoreProduct>(),
                        PriceList = _dist.Where(c => c.StoreProductIsTemp == false || c.StoreProductIsTemp == true && c.StoreProductCreatedByUserId == userId).OrderBy(c => c.StorePrice).Select(c => new StoreProduct
                        //PriceList = _dist.OrderBy(c => c.StorePrice).Select(c => new StoreProduct
                        {
                            Id = c.StoreProductId,
                            StoreId = c.StoreId,
                            Price = Math.Round(c.StorePrice * _UserProductListCompleteTempModel.Quantity, 2),
                            PriceBase = Math.Round(c.StorePrice, 2),
                            PriceRatioBase = Math.Round(c.StorePriceRatio, 2),
                            PriceRatio = Math.Round(c.StorePriceRatio * _UserProductListCompleteTempModel.QuantityWeight, 2),
                            NeedsUpdate = c.NeedsUpdate.HasValue ? c.NeedsUpdate.Value : true,
                            //Url = c.Url,
                            Url = Helpers.Extensibility.GetStoreFetcher(c.StoreId).GetProductViewableUrl("", c.Url),
                            CreatedByUserId = c.CreatedByUserId,
                            UpdateDate = c.UpdateDate,
                            Unit = c.Unit,
                            OnlineProductId = c.OnlineProductId
                        }).ToList(),
                        LastAddedDate = _UserProductListCompleteTempModel.LastAddedDate
                    };
                    _shoppingListToReturn.Add(_UserProductListCompleteModel2New);

                }

                //get categories distinct and arrange by categorie
                var _categoriesDistinct = _shoppingListToReturn.DistinctBy(c => c.Category).Select(c => c.Category).OrderBy(c => c).ToList();
                foreach (var _category in _categoriesDistinct)
                {
                    if (_category == null) continue;
                    var _newCategory = new UserProductListCategory
                    {
                        CategoryName = _category,
                        Products = new List<UserProductListCompleteModel2>()
                    };
                    _newCategory.Products = _shoppingListToReturn.Where(c => c.Category != null && c.Category.ToLower() == _category.ToLower()).ToList();
                    if (_newCategory.Products.Count > 0)
                    {
                        _shoppingList.Categories.Add(_newCategory);
                    }
                }



                //Inventory Product Simple
                var _userProductsSimpleInventory = db.UserProductsSimple.Where(c => c.UserId == userId && c.Quantity != 0 && c.ListName.Equals("inventory")).OrderByDescending(c => c.UpdateDate).Select(c => new Models.UserProductListCompleteModel2
                {
                    Id = c.Id,
                    Quantity = c.Quantity,
                    ItemType = "productSimpleInventory",
                    Url = c.ImageUrl,
                    Name = c.Name,
                    LastAddedDate = c.UpdateDate,
                    Category = "Notas"
                });
                if (_userProductsSimpleInventory.Count() > 0)
                    _inventoryListToReturn.AddRange(_userProductsSimpleInventory);

                //Inventory
                var _tempInventory =
                  from userProduct in db.UserProductsList
                  join product in db.Products on userProduct.ProductId equals product.Id
                  join storePrd in db.StoreProducts on userProduct.ProductId equals storePrd.ProductId
                  where userProduct.UserId == userId && userProduct.ListName == "inventory"
                  //&&
                  //(!product.IsTemp.HasValue || !product.IsTemp.Value || (product.IsTemp.Value == true && product.CreatedByUserId == userId))
                  select new UserProductListCompleteTempModel
                  {
                      Id = userProduct.Id,
                      ProductId = userProduct.ProductId,
                      Quantity = userProduct.Quantity.Value,
                      Barcode = product.Barcode,
                      Brand = product.Brand,
                      ItemType = "inventory",
                      Name = product.Name,
                      Category = product.CategoryString,
                      StorePrice = Math.Round(storePrd.Price.Value, 2),
                      StorePriceRatio = storePrd.PriceRatio.HasValue ? storePrd.PriceRatio.Value : 1,
                      StorePriceUnit = storePrd.Unit,
                      StoreId = storePrd.StoreId,
                      StoreProductId = storePrd.Id,
                      StoreProductCreatedByUserId = storePrd.UserId,
                      NeedsUpdate = storePrd.NeedsUpdate,
                      Url = storePrd.Url,
                      CreatedByUserId = storePrd.UserId,
                      Weight = product.Weight,
                      Unit = storePrd.Unit,
                      UpdateDate = storePrd.UpdateDate.HasValue ? storePrd.UpdateDate.Value : DateTime.MinValue,
                      LastAddedDate = userProduct.LastAddedDate.HasValue ? userProduct.LastAddedDate.Value : DateTime.MinValue,
                      OnlineProductId = storePrd.OnlineProductId,
                      StoreProductIsTemp = !storePrd.IsTemp.HasValue ? false : storePrd.IsTemp.Value,
                      ProductIsTemp = product.IsTemp.HasValue ? product.IsTemp.Value : false
                  };

                var _distincInventory = _tempInventory.GroupBy(c => c.ProductId).ToList();
                foreach (var _dist in _distincInventory)
                {

                    UserProductListCompleteTempModel _UserProductListCompleteTempModel = _dist.First();
                    UserProductListCompleteModel2 _UserProductListCompleteModel2New = new UserProductListCompleteModel2
                    {
                        Id = _UserProductListCompleteTempModel.Id,
                        ProductId = _UserProductListCompleteTempModel.ProductId,
                        Quantity = _UserProductListCompleteTempModel.Quantity,
                        QuantityWeight = _UserProductListCompleteTempModel.QuantityWeight,
                        Barcode = _UserProductListCompleteTempModel.Barcode,
                        Brand = _UserProductListCompleteTempModel.Brand,
                        ItemType = "inventory",
                        Name = _UserProductListCompleteTempModel.Name,
                        Category = _UserProductListCompleteTempModel.Category,
                        Weight = _UserProductListCompleteTempModel.Weight,
                        Price = 0,
                        IsTemp = _UserProductListCompleteTempModel.ProductIsTemp,
                        //PriceList = new List<StoreProduct>(),
                        PriceList = _dist.Where(c => c.StoreProductIsTemp == false || c.StoreProductIsTemp == true && c.StoreProductCreatedByUserId == userId).OrderBy(c => c.StorePrice).Select(c => new StoreProduct
                        //PriceList = _dist.OrderBy(c => c.StorePrice).Select(c => new StoreProduct
                        {
                            Id = c.StoreProductId,
                            StoreId = c.StoreId,
                            Price = Math.Round(c.StorePrice * _UserProductListCompleteTempModel.Quantity, 2),
                            PriceBase = Math.Round(c.StorePrice, 2),
                            PriceRatioBase = Math.Round(c.StorePriceRatio, 2),
                            PriceRatio = Math.Round(c.StorePriceRatio * _UserProductListCompleteTempModel.QuantityWeight, 2),
                            NeedsUpdate = c.NeedsUpdate.HasValue ? c.NeedsUpdate.Value : true,
                            //Url = c.Url,
                            Url = Helpers.Extensibility.GetStoreFetcher(c.StoreId).GetProductViewableUrl("", c.Url),
                            CreatedByUserId = c.CreatedByUserId,
                            UpdateDate = c.UpdateDate,
                            Unit = c.Unit,
                            OnlineProductId = c.OnlineProductId
                        }).ToList(),
                        LastAddedDate = _UserProductListCompleteTempModel.LastAddedDate
                    };
                    _inventoryListToReturn.Add(_UserProductListCompleteModel2New);

                }

                //get categories distinct and arrange by categorie
                var _categoriesInventoryDistinct = _inventoryListToReturn.DistinctBy(c => c.Category).Select(c => c.Category).OrderBy(c => c).ToList();
                foreach (var _category in _categoriesInventoryDistinct)
                {
                    if (_category == null) continue;
                    var _newCategory = new UserProductListCategory
                    {
                        CategoryName = _category,
                        Products = new List<UserProductListCompleteModel2>()
                    };
                    _newCategory.Products = _inventoryListToReturn.Where(c => c.Category != null && c.Category.ToLower() == _category.ToLower()).ToList();
                    if (_newCategory.Products.Count > 0)
                    {
                        _inventoryList.Categories.Add(_newCategory);
                    }
                }

                //reorder all by LastAddedDate
                //_shoppingListToReturn = _shoppingListToReturn.OrderByDescending(c => c.LastAddedDate).ToList();

                return _lists;
            }
        }

        static public UserProductListCompleteModel2 GetCompleteModel(int id, string userId = "")
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _reQuery = db.UserProductsList.Where(c => c.Id == id).Include("Products").FirstOrDefault();
                if (_reQuery != null)
                {
                    var _toRet = new UserProductListCompleteModel2()
                    {
                        Id = _reQuery.Id,
                        ProductId = _reQuery.ProductId,
                        ItemType = _reQuery.ListName.ToLower() == "in" ? "shoppingList" : "inventory",
                        Name = _reQuery.Products.Name,
                        Weight = _reQuery.Products.Weight,
                        Quantity = _reQuery.Quantity.Value,
                        Price = _reQuery.Products.Price,
                        Barcode = _reQuery.Products.Barcode,
                        Brand = _reQuery.Products.Brand,
                        Category = _reQuery.Products.CategoryString,
                        IsTemp = _reQuery.Products.IsTemp
                    };
                    //_toRet = FillProductStorePrices(_toRet, userId);
                    _toRet = FillProductStorePrices(_toRet);
                    return _toRet;
                }
                else
                {
                    return null;
                }

            }
        }

        static public UserProductListCompleteModel2 GetCompleteModelV2(int id, string userId = "", int productId = -1)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                ClassLibrary1.UserProductsList _reQuery = null;
                if (productId != -1)
                    _reQuery = db.UserProductsList.Where(c => c.ProductId == productId && c.UserId == userId).Include("Products").FirstOrDefault();
                else
                    _reQuery = db.UserProductsList.Where(c => c.Id == id).Include("Products").FirstOrDefault();
                if (_reQuery != null)
                {
                    var _toRet = new UserProductListCompleteModel2()
                    {
                        Id = _reQuery.Id,
                        ProductId = _reQuery.ProductId,
                        ItemType = _reQuery.ListName.ToLower() == "in" ? "shoppingList" : "inventory",
                        Name = _reQuery.Products.Name,
                        Weight = _reQuery.Products.Weight,
                        Quantity = _reQuery.Quantity.Value,
                        QuantityWeight = _reQuery.QuantityWeight.HasValue ? _reQuery.QuantityWeight.Value : 0,
                        Price = _reQuery.Products.Price,
                        Barcode = _reQuery.Products.Barcode,
                        Brand = _reQuery.Products.Brand,
                        Category = _reQuery.Products.CategoryString,
                        IsTemp = _reQuery.Products.IsTemp,
                        CreatedByUserId = _reQuery.Products.CreatedByUserId
                    };
                    _toRet = FillProductStorePrices(_toRet, userId);
                    //_toRet = FillProductStorePrices(_toRet);
                    return _toRet;
                }
                else
                {
                    return null;
                }

            }
        }

        static public List<UserProductsList> GetOfUser(string userId, string list)
        {
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {

                //TODO - other lists
                if (list == "shoppingList")
                    return db2.UserProductsList.Where(c => c.UserId.Equals(userId) && c.ListName.ToLower() == "in").ToList();
                if (list == "inventory")
                    return db2.UserProductsList.Where(c => c.UserId.Equals(userId) && c.ListName.ToLower() == "inventory").ToList();
                if (list == "all") //get from consumed also
                    return db2.UserProductsList.Where(c => c.UserId.Equals(userId)).ToList();
                if (list == "all+history") //get from consumed and history also
                    return db2.UserProductsList.Where(c => c.UserId.Equals(userId)).ToList();
                else
                    return new List<UserProductsList>();
            }
        }

        static public List<SpiroWeb.Models.UserProductListModel> GetOfUserConsumed(string userId)
        {
            var userConsumedProducts = from m in db.UserProductsConsumed
                                       where m.UserId == userId && m.ActionTakenByUser == null
                                       select new SpiroWeb.Models.UserProductListModel
                                       {
                                           Id = m.Id,
                                           UserId = userId,
                                           ListName = "consumed",
                                           ItemType = "consumed",
                                           ProductId = m.ProductId,
                                           Quantity = m.Quantity ?? 1
                                       };
            return userConsumedProducts.ToList();
        }

        static public List<string> GetUsersIdsWithProductInList(int productId, string list)
        {

            //TODO - other lists
            if (list == "shoppingList")
                return db.UserProductsList.Where(c => c.ProductId == productId && c.ListName.ToLower() == "in") //TODO - pass to shoppingList
                                                                                                                //.ToList()
                    .Select(m => m.UserId).ToList();
            if (list == "inventory")
                return db.UserProductsList.Where(c => c.ProductId == productId && c.ListName.ToLower() == "inventory")
                    .ToList().Select(m => m.UserId).ToList();
            if (list == "all") //get from consumed also
                return db.UserProductsList.Where(c => c.ProductId == productId).Select(m => m.UserId).ToList();
            if (list == "all+history") //get from consumed and history also
                return db.UserProductsList.Where(c => c.ProductId == productId).Select(m => m.UserId).ToList();
            else
                return new List<string>();
        }


        static public bool DeleteOfUser(int userProductId)
        {
            try
            {
                var _userProduct = db.UserProductsList.Where(c => c.Id == userProductId).FirstOrDefault();

                //if is in inventory, remove from it and add do consumed list
                if (_userProduct.ListName == "inventory")
                {
                    //OBSOLETE - adding to consumed
                    //for (int i = 0; i < _userProduct.Quantity; i++)
                    //{
                    //    UserProductsConsumed _UserProductsConsumed = new UserProductsConsumed();
                    //    _UserProductsConsumed.ProductId = _userProduct.ProductId;
                    //    _UserProductsConsumed.Quantity = 1;
                    //    _UserProductsConsumed.UserId = _userProduct.UserId;
                    //    _UserProductsConsumed.CreateDate = DateTime.Now;
                    //    db.UserProductsConsumed.Add(_UserProductsConsumed);
                    //    //db.SaveChanges();
                    //}

                    //NEW - add to shopping list
                    UserProductsList _queryExistsInUserList = (from c in db.UserProductsList
                                                               where c.ProductId.Equals(_userProduct.ProductId) &&
                                                               c.UserId.Equals(_userProduct.UserId) &&
                                                               c.ListName.Equals("In")
                                                               select c).FirstOrDefault();

                    //Exist in User Lists , change quantity
                    if (_queryExistsInUserList != null)
                    {
                        _queryExistsInUserList.Quantity = _queryExistsInUserList.Quantity + _userProduct.Quantity;
                        _queryExistsInUserList.LastAddedDate = DateTime.Now;
                        db.UserProductsList.Attach(_queryExistsInUserList);
                        var entry = db.Entry(_queryExistsInUserList);
                        entry.Property(y => y.Quantity).IsModified = true;
                        entry.Property(y => y.LastAddedDate).IsModified = true;
                    }
                    //add new product to user In List
                    else
                    {
                        UserProductsList _UserProductsList = new UserProductsList();
                        _UserProductsList.ProductId = _userProduct.ProductId;
                        _UserProductsList.UserId = _userProduct.UserId;
                        _UserProductsList.Quantity = _userProduct.Quantity;
                        _UserProductsList.ListName = "In";
                        _UserProductsList.LastAddedDate = DateTime.Now;

                        db.UserProductsList.Add(_UserProductsList);
                    }

                    //Add to History
                    UserProductsListHistory _UserProductsListHistory = new UserProductsListHistory();
                    _UserProductsListHistory.ProductId = _userProduct.ProductId;
                    _UserProductsListHistory.UserId = _userProduct.UserId;
                    _UserProductsListHistory.Quantity = _userProduct.Quantity;
                    _UserProductsListHistory.ListName = "consumed";
                    _UserProductsListHistory.InsertDate = DateTime.Now;
                    db.UserProductsListHistory.Add(_UserProductsListHistory);

                }

                if (_userProduct != null)
                    db.UserProductsList.Remove(_userProduct);

                db.SaveChanges();
                return true;


            }
            catch (Exception ex)
            {
                Logger.Debug("error deleting user inventory product: " + ex.Message);
                return false;
            }

        }

        static public UserProductsList SubtractQuantity(int userProductId)
        {
            try
            {
                var _userProduct = db.UserProductsList.Where(c => c.Id == userProductId).FirstOrDefault();


                //if user product is from inventory, add the same product to consumed list
                if (_userProduct.ListName == "inventory")
                {
                    //OBSOLETE - consumed 
                    //UserProductsConsumed _UserProductsConsumed = new UserProductsConsumed();
                    //_UserProductsConsumed.ProductId = _userProduct.ProductId;
                    //_UserProductsConsumed.Quantity = 1;
                    //_UserProductsConsumed.UserId = _userProduct.UserId;
                    //_UserProductsConsumed.CreateDate = DateTime.Now;
                    //db.UserProductsConsumed.Add(_UserProductsConsumed);

                    //NEW - add to shopping list
                    UserProductsList _queryExistsInUserList = (from c in db.UserProductsList
                                                               where c.ProductId.Equals(_userProduct.ProductId) &&
                                                               c.UserId.Equals(_userProduct.UserId) &&
                                                               c.ListName.Equals("In")
                                                               select c).FirstOrDefault();

                    //Exist in User Lists , change quantity
                    if (_queryExistsInUserList != null)
                    {
                        _queryExistsInUserList.Quantity = _queryExistsInUserList.Quantity + 1;
                        _queryExistsInUserList.LastAddedDate = DateTime.Now;
                        db.UserProductsList.Attach(_queryExistsInUserList);
                        var entry = db.Entry(_queryExistsInUserList);
                        entry.Property(y => y.Quantity).IsModified = true;
                        entry.Property(y => y.LastAddedDate).IsModified = true;
                    }
                    //add new product to user In List
                    else
                    {
                        UserProductsList _UserProductsList = new UserProductsList();
                        _UserProductsList.ProductId = _userProduct.ProductId;
                        _UserProductsList.UserId = _userProduct.UserId;
                        _UserProductsList.Quantity = 1;
                        _UserProductsList.ListName = "In";
                        _UserProductsList.LastAddedDate = DateTime.Now;

                        db.UserProductsList.Add(_UserProductsList);
                    }

                    //Add to History
                    UserProductsListHistory _UserProductsListHistory = new UserProductsListHistory();
                    _UserProductsListHistory.ProductId = _userProduct.ProductId;
                    _UserProductsListHistory.UserId = _userProduct.UserId;
                    _UserProductsListHistory.Quantity = 1;
                    _UserProductsListHistory.ListName = "consumed";
                    _UserProductsListHistory.InsertDate = DateTime.Now;
                    db.UserProductsListHistory.Add(_UserProductsListHistory);
                }

                if (_userProduct.Quantity - 1 == 0)
                {
                    db.UserProductsList.Remove(_userProduct);
                    db.SaveChanges();
                    return null;
                }
                else
                {
                    _userProduct.Quantity--;
                    db.SaveChanges();
                    return _userProduct;
                }

            }
            catch (Exception ex)
            {
                Logger.Debug("error Subtracting Quantity user inventory product: " + ex.Message);
                return null;

            }
        }

        static public int SubtractQuantityToProductSimple(int userProductId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    UserProductsSimple _userProduct = null;

                    _userProduct = db.UserProductsSimple.Where(c => c.Id == userProductId).FirstOrDefault();
                    if (_userProduct != null)
                    {
                        if (_userProduct.Quantity == 0) return 0;

                        _userProduct.Quantity--;
                        _userProduct.UpdateDate = DateTime.Now;
                        db.SaveChanges();

                        return _userProduct.Quantity;
                    }
                    else return -1;
                }

            }
            catch (Exception ex)
            {
                Logger.Debug("error Subtracting Quantity to user simple product: " + ex.Message);
                return -1;
            }
        }


        static public UserProductsList AddQuantity(int userProductId)
        {
            try
            {
                UserProductsList _userProduct = null;

                _userProduct = db.UserProductsList.Where(c => c.Id == userProductId).FirstOrDefault();

                if (_userProduct != null) _userProduct.Quantity++;
                else return null;

                db.SaveChanges();
                return _userProduct;

            }
            catch (Exception ex)
            {
                Logger.Debug("error deleting user inventory product: " + ex.Message);
                return null;
            }

        }

        static public int AddQuantityToProductSimple(int userProductId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _userProducts = db.UserProductsSimple.Where(c => c.Id == userProductId).FirstOrDefault();

                    if (_userProducts != null)
                    {
                        _userProducts.Quantity++;
                        _userProducts.UpdateDate = DateTime.Now;
                    }
                    else return -1;

                    db.SaveChanges();
                    return _userProducts.Quantity;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("error adding quantity to user simple product: " + ex.Message);
                return -1;
            }

        }

        //Returns new or existing inventory user product
        static public UserProductListCompleteModel2 CheckoutProduct(int userProductId, string userId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    UserProductsList _productToRet = null;

                    var _userProduct = db.UserProductsList.Where(c => c.Id == userProductId && c.UserId.Equals(userId)).FirstOrDefault();
                    if (_userProduct != null)
                    {
                        if (_userProduct.ListName.ToLower() == "in" || _userProduct.ListName.ToLower() == "shoppinglist") //checkout shopping list product
                        {
                            //remove from shopping list and add with same quantity to "Despensa" and to history ans bought
                            db.UserProductsList.Remove(_userProduct);

                            UserProductsList _inventoryPoductExists = (from c in db.UserProductsList
                                                                       where c.ProductId.Equals(_userProduct.ProductId) &&
                                                                       c.UserId.Equals(userId) &&
                                                                       c.ListName.ToLower().Equals("inventory")
                                                                       select c).FirstOrDefault();

                            //Exist in User Lists , change quantity
                            if (_inventoryPoductExists != null)
                            {

                                _inventoryPoductExists.Quantity = _inventoryPoductExists.Quantity + _userProduct.Quantity;
                                _inventoryPoductExists.LastAddedDate = DateTime.Now;
                                db.UserProductsList.Attach(_inventoryPoductExists);
                                var entry = db.Entry(_inventoryPoductExists);
                                //TO REMEMBER
                                entry.Property(y => y.Quantity).IsModified = true;
                                entry.Property(y => y.LastAddedDate).IsModified = true;
                                _productToRet = _inventoryPoductExists;
                            }
                            //add new product to user In List
                            else
                            {
                                UserProductsList _UserProductsList = new UserProductsList();
                                _UserProductsList.ProductId = _userProduct.ProductId;
                                _UserProductsList.UserId = userId;
                                _UserProductsList.Quantity = _userProduct.Quantity;
                                _UserProductsList.ListName = "inventory";
                                _UserProductsList.LastAddedDate = DateTime.Now;

                                db.UserProductsList.Add(_UserProductsList);
                                _productToRet = _UserProductsList;
                            }

                            //add to bought history
                            UserProductsListHistory _UserProductsListHistoryBought = new UserProductsListHistory();
                            _UserProductsListHistoryBought.ProductId = _userProduct.ProductId;
                            _UserProductsListHistoryBought.UserId = userId;
                            _UserProductsListHistoryBought.Quantity = _userProduct.Quantity;
                            _UserProductsListHistoryBought.ListName = "bought";
                            _UserProductsListHistoryBought.InsertDate = DateTime.Now;
                            db.UserProductsListHistory.Add(_UserProductsListHistoryBought);
                            db.SaveChanges();

                        }
                        else if (_userProduct.ListName.ToLower() == "inventory") //checkout inventory product
                        {
                            //remove from inventory and add with same quantity to "ShoppingList" and to history ans bought
                            db.UserProductsList.Remove(_userProduct);

                            UserProductsList _shoppingListPoductExists = (from c in db.UserProductsList
                                                                          where c.ProductId.Equals(_userProduct.ProductId) &&
                                                                          c.UserId.Equals(userId) &&
                                                                          c.ListName.ToLower().Equals("in")
                                                                          select c).FirstOrDefault();

                            //Exist in User shopping list , change quantity
                            if (_shoppingListPoductExists != null)
                            {

                                _shoppingListPoductExists.Quantity = _shoppingListPoductExists.Quantity + _userProduct.Quantity;
                                _shoppingListPoductExists.LastAddedDate = DateTime.Now;
                                db.UserProductsList.Attach(_shoppingListPoductExists);
                                var entry = db.Entry(_shoppingListPoductExists);
                                //TO REMEMBER
                                entry.Property(y => y.Quantity).IsModified = true;
                                entry.Property(y => y.LastAddedDate).IsModified = true;
                                _productToRet = _shoppingListPoductExists;
                            }
                            //add new product to user In List
                            else
                            {
                                UserProductsList _UserProductsList = new UserProductsList();
                                _UserProductsList.ProductId = _userProduct.ProductId;
                                _UserProductsList.UserId = userId;
                                _UserProductsList.Quantity = _userProduct.Quantity;
                                _UserProductsList.ListName = "in";
                                _UserProductsList.LastAddedDate = DateTime.Now;

                                db.UserProductsList.Add(_UserProductsList);
                                _productToRet = _UserProductsList;
                            }

                            //add to consumed history
                            UserProductsListHistory _UserProductsListHistoryBought = new UserProductsListHistory();
                            _UserProductsListHistoryBought.ProductId = _userProduct.ProductId;
                            _UserProductsListHistoryBought.UserId = userId;
                            _UserProductsListHistoryBought.Quantity = _userProduct.Quantity;
                            _UserProductsListHistoryBought.ListName = "consumed";
                            _UserProductsListHistoryBought.InsertDate = DateTime.Now;
                            db.UserProductsListHistory.Add(_UserProductsListHistoryBought);
                            db.SaveChanges();
                        }
                    }

                    //TODO - code to checkout from inventory back to shopping list, abstract code
                    //foreach (var _userProductShoppingList in _userProducts)
                    //{


                    //arrange right return type for client side parsing
                    var _reQuery = db.UserProductsList.Where(c => c.Id == _productToRet.Id).Include("Products").FirstOrDefault();
                    var _toRet = new UserProductListCompleteModel2()
                    {
                        Id = _reQuery.Id,
                        ProductId = _reQuery.ProductId,
                        ItemType = _reQuery.ListName.ToLower() == "in" ? "shoppingList" : "inventory",
                        Name = _reQuery.Products.Name,
                        Weight = _reQuery.Products.Weight,
                        Quantity = _reQuery.Quantity.Value,
                        Price = _reQuery.Products.Price,
                        Barcode = _reQuery.Products.Barcode,
                        Brand = _reQuery.Products.Brand,
                        Category = _reQuery.Products.CategoryString
                    };
                    _toRet = FillProductStorePrices(_toRet);
                    return _toRet;
                }

            }
            catch (Exception ex)
            {
                Logger.Debug("error checking out products: " + ex.Message);
                return null;

            }
        }

        static public UserProductListCompleteModel2 CheckoutProductV2(int userProductId, string userId, bool emulate = false)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    UserProductsList _productToRet = null;

                    var _userProduct = db.UserProductsList.Where(c => c.Id == userProductId && c.UserId.Equals(userId)).FirstOrDefault();
                    if (_userProduct != null)
                    {
                        if (_userProduct.ListName.ToLower() == "in" || _userProduct.ListName.ToLower() == "shoppinglist") //checkout shopping list product
                        {
                            //remove from shopping list and add with same quantity to "Despensa" and to history ans bought
                            db.UserProductsList.Remove(_userProduct);

                            UserProductsList _inventoryPoductExists = (from c in db.UserProductsList
                                                                       where c.ProductId.Equals(_userProduct.ProductId) &&
                                                                       c.UserId.Equals(userId) &&
                                                                       c.ListName.ToLower().Equals("inventory")
                                                                       select c).FirstOrDefault();

                            //Exist in User Lists , change quantity
                            if (_inventoryPoductExists != null)
                            {

                                _inventoryPoductExists.Quantity = _inventoryPoductExists.Quantity + _userProduct.Quantity;
                                _inventoryPoductExists.LastAddedDate = DateTime.Now;
                                db.UserProductsList.Attach(_inventoryPoductExists);
                                var entry = db.Entry(_inventoryPoductExists);
                                //TO REMEMBER
                                entry.Property(y => y.Quantity).IsModified = true;
                                entry.Property(y => y.LastAddedDate).IsModified = true;
                                _productToRet = _inventoryPoductExists;
                            }
                            //add new product to user In List
                            else
                            {
                                UserProductsList _UserProductsList = new UserProductsList();
                                _UserProductsList.ProductId = _userProduct.ProductId;
                                _UserProductsList.UserId = userId;
                                _UserProductsList.Quantity = _userProduct.Quantity;
                                _UserProductsList.ListName = "inventory";
                                _UserProductsList.LastAddedDate = DateTime.Now;

                                db.UserProductsList.Add(_UserProductsList);
                                _productToRet = _UserProductsList;
                            }

                            //add to bought history
                            UserProductsListHistory _UserProductsListHistoryBought = new UserProductsListHistory();
                            _UserProductsListHistoryBought.ProductId = _userProduct.ProductId;
                            _UserProductsListHistoryBought.UserId = userId;
                            _UserProductsListHistoryBought.Quantity = _userProduct.Quantity;
                            _UserProductsListHistoryBought.ListName = "bought";
                            _UserProductsListHistoryBought.InsertDate = DateTime.Now;
                            db.UserProductsListHistory.Add(_UserProductsListHistoryBought);
                            if (!emulate)
                                db.SaveChanges();

                        }
                        else if (_userProduct.ListName.ToLower() == "inventory") //checkout inventory product
                        {
                            //remove from inventory and add with same quantity to "ShoppingList" and to history ans bought
                            db.UserProductsList.Remove(_userProduct);

                            UserProductsList _shoppingListPoductExists = (from c in db.UserProductsList
                                                                          where c.ProductId.Equals(_userProduct.ProductId) &&
                                                                          c.UserId.Equals(userId) &&
                                                                          c.ListName.ToLower().Equals("in")
                                                                          select c).FirstOrDefault();

                            //Exist in User shopping list , change quantity
                            if (_shoppingListPoductExists != null)
                            {

                                _shoppingListPoductExists.Quantity = _shoppingListPoductExists.Quantity + _userProduct.Quantity;
                                _shoppingListPoductExists.LastAddedDate = DateTime.Now;
                                db.UserProductsList.Attach(_shoppingListPoductExists);
                                var entry = db.Entry(_shoppingListPoductExists);
                                //TO REMEMBER
                                entry.Property(y => y.Quantity).IsModified = true;
                                entry.Property(y => y.LastAddedDate).IsModified = true;
                                _productToRet = _shoppingListPoductExists;
                            }
                            //add new product to user In List
                            else
                            {
                                UserProductsList _UserProductsList = new UserProductsList();
                                _UserProductsList.ProductId = _userProduct.ProductId;
                                _UserProductsList.UserId = userId;
                                _UserProductsList.Quantity = _userProduct.Quantity;
                                _UserProductsList.ListName = "in";
                                _UserProductsList.LastAddedDate = DateTime.Now;

                                db.UserProductsList.Add(_UserProductsList);
                                _productToRet = _UserProductsList;
                            }

                            //add to consumed history
                            UserProductsListHistory _UserProductsListHistoryBought = new UserProductsListHistory();
                            _UserProductsListHistoryBought.ProductId = _userProduct.ProductId;
                            _UserProductsListHistoryBought.UserId = userId;
                            _UserProductsListHistoryBought.Quantity = _userProduct.Quantity;
                            _UserProductsListHistoryBought.ListName = "consumed";
                            _UserProductsListHistoryBought.InsertDate = DateTime.Now;
                            db.UserProductsListHistory.Add(_UserProductsListHistoryBought);
                            if (!emulate)
                                db.SaveChanges();
                        }
                    }

                    //TODO - code to checkout from inventory back to shopping list, abstract code
                    //foreach (var _userProductShoppingList in _userProducts)
                    //{


                    //arrange right return type for client side parsing
                    UserProductListCompleteModel2 _toRet = null;
                    if (_productToRet.Id != 0)
                    {
                        var _reQuery = db.UserProductsList.Where(c => c.Id == _productToRet.Id).Include("Products").FirstOrDefault();
                        _toRet = new UserProductListCompleteModel2()
                        {
                            Id = _reQuery.Id,
                            ProductId = _reQuery.ProductId,
                            ItemType = _reQuery.ListName.ToLower() == "in" ? "shoppingList" : "inventory",
                            Name = _reQuery.Products.Name,
                            Weight = _reQuery.Products.Weight,
                            Quantity = _reQuery.Quantity.Value,
                            Price = _reQuery.Products.Price,
                            Barcode = _reQuery.Products.Barcode,
                            Brand = _reQuery.Products.Brand,
                            Category = _reQuery.Products.CategoryString
                        };
                        //_toRet = FillProductStorePrices(_toRet, userId);
                        _toRet = FillProductStorePrices(_toRet);
                    }
                    else
                    {
                        if (emulate)
                        {
                            //get product
                            var _product = db.Products.Where(c => c.Id == _productToRet.ProductId).FirstOrDefault();
                            if (_product != null)
                            {
                                _toRet = new UserProductListCompleteModel2()
                                {
                                    Id = userProductId * -1,
                                    ProductId = _productToRet.ProductId,
                                    ItemType = _productToRet.ListName.ToLower() == "in" ? "shoppingList" : "inventory",
                                    Name = _product.Name,
                                    Weight = _product.Weight,
                                    Quantity = _productToRet.Quantity.Value,
                                    Price = _product.Price,
                                    Barcode = _product.Barcode,
                                    Brand = _product.Brand,
                                    Category = _product.CategoryString
                                };
                                //_toRet = FillProductStorePrices(_toRet, userId);
                                _toRet = FillProductStorePrices(_toRet);
                            }
                        }
                    }
                    return _toRet;
                }

            }
            catch (Exception ex)
            {
                Logger.Debug("error checking out products: " + ex.Message);
                return null;

            }
        }

        //with isTemp computations
        static public UserProductListCompleteModel2 CheckoutProductV3(int userProductId, string userId, bool emulate = false)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    UserProductsList _productToRet = null;

                    var _userProduct = db.UserProductsList.Where(c => c.Id == userProductId && c.UserId.Equals(userId)).FirstOrDefault();
                    if (_userProduct != null)
                    {
                        if (_userProduct.ListName.ToLower() == "in" || _userProduct.ListName.ToLower() == "shoppinglist") //checkout shopping list product
                        {
                            //remove from shopping list and add with same quantity to "Despensa" and to history ans bought
                            db.UserProductsList.Remove(_userProduct);

                            UserProductsList _inventoryPoductExists = (from c in db.UserProductsList
                                                                       where c.ProductId.Equals(_userProduct.ProductId) &&
                                                                       c.UserId.Equals(userId) &&
                                                                       c.ListName.ToLower().Equals("inventory")
                                                                       select c).FirstOrDefault();

                            //Exist in User Lists , change quantity
                            if (_inventoryPoductExists != null)
                            {

                                _inventoryPoductExists.Quantity = _inventoryPoductExists.Quantity + _userProduct.Quantity;
                                _inventoryPoductExists.LastAddedDate = DateTime.Now;
                                db.UserProductsList.Attach(_inventoryPoductExists);
                                var entry = db.Entry(_inventoryPoductExists);
                                //TO REMEMBER
                                entry.Property(y => y.Quantity).IsModified = true;
                                entry.Property(y => y.LastAddedDate).IsModified = true;
                                _productToRet = _inventoryPoductExists;
                            }
                            //add new product to user In List
                            else
                            {
                                UserProductsList _UserProductsList = new UserProductsList();
                                _UserProductsList.ProductId = _userProduct.ProductId;
                                _UserProductsList.UserId = userId;
                                _UserProductsList.Quantity = _userProduct.Quantity;
                                _UserProductsList.ListName = "inventory";
                                _UserProductsList.LastAddedDate = DateTime.Now;

                                db.UserProductsList.Add(_UserProductsList);
                                _productToRet = _UserProductsList;
                            }

                            //add to bought history
                            UserProductsListHistory _UserProductsListHistoryBought = new UserProductsListHistory();
                            _UserProductsListHistoryBought.ProductId = _userProduct.ProductId;
                            _UserProductsListHistoryBought.UserId = userId;
                            _UserProductsListHistoryBought.Quantity = _userProduct.Quantity;
                            _UserProductsListHistoryBought.ListName = "bought";
                            _UserProductsListHistoryBought.InsertDate = DateTime.Now;
                            db.UserProductsListHistory.Add(_UserProductsListHistoryBought);
                            if (!emulate)
                                db.SaveChanges();

                        }
                        else if (_userProduct.ListName.ToLower() == "inventory") //checkout inventory product
                        {
                            //remove from inventory and add with same quantity to "ShoppingList" and to history ans bought
                            db.UserProductsList.Remove(_userProduct);

                            UserProductsList _shoppingListPoductExists = (from c in db.UserProductsList
                                                                          where c.ProductId.Equals(_userProduct.ProductId) &&
                                                                          c.UserId.Equals(userId) &&
                                                                          c.ListName.ToLower().Equals("in")
                                                                          select c).FirstOrDefault();

                            //Exist in User shopping list , change quantity
                            if (_shoppingListPoductExists != null)
                            {

                                _shoppingListPoductExists.Quantity = _shoppingListPoductExists.Quantity + _userProduct.Quantity;
                                _shoppingListPoductExists.LastAddedDate = DateTime.Now;
                                db.UserProductsList.Attach(_shoppingListPoductExists);
                                var entry = db.Entry(_shoppingListPoductExists);
                                //TO REMEMBER
                                entry.Property(y => y.Quantity).IsModified = true;
                                entry.Property(y => y.LastAddedDate).IsModified = true;
                                _productToRet = _shoppingListPoductExists;
                            }
                            //add new product to user In List
                            else
                            {
                                UserProductsList _UserProductsList = new UserProductsList();
                                _UserProductsList.ProductId = _userProduct.ProductId;
                                _UserProductsList.UserId = userId;
                                _UserProductsList.Quantity = _userProduct.Quantity;
                                _UserProductsList.ListName = "in";
                                _UserProductsList.LastAddedDate = DateTime.Now;

                                db.UserProductsList.Add(_UserProductsList);
                                _productToRet = _UserProductsList;
                            }

                            //add to consumed history
                            UserProductsListHistory _UserProductsListHistoryBought = new UserProductsListHistory();
                            _UserProductsListHistoryBought.ProductId = _userProduct.ProductId;
                            _UserProductsListHistoryBought.UserId = userId;
                            _UserProductsListHistoryBought.Quantity = _userProduct.Quantity;
                            _UserProductsListHistoryBought.ListName = "consumed";
                            _UserProductsListHistoryBought.InsertDate = DateTime.Now;
                            db.UserProductsListHistory.Add(_UserProductsListHistoryBought);
                            if (!emulate)
                                db.SaveChanges();
                        }
                    }

                    //TODO - code to checkout from inventory back to shopping list, abstract code
                    //foreach (var _userProductShoppingList in _userProducts)
                    //{


                    //arrange right return type for client side parsing
                    UserProductListCompleteModel2 _toRet = null;
                    if (_productToRet.Id != 0)
                    {
                        var _reQuery = db.UserProductsList.Where(c => c.Id == _productToRet.Id).Include("Products").FirstOrDefault();
                        _toRet = new UserProductListCompleteModel2()
                        {
                            Id = _reQuery.Id,
                            ProductId = _reQuery.ProductId,
                            ItemType = _reQuery.ListName.ToLower() == "in" ? "shoppingList" : "inventory",
                            Name = _reQuery.Products.Name,
                            Weight = _reQuery.Products.Weight,
                            Quantity = _reQuery.Quantity.Value,
                            Price = _reQuery.Products.Price,
                            Barcode = _reQuery.Products.Barcode,
                            Brand = _reQuery.Products.Brand,
                            Category = _reQuery.Products.CategoryString
                        };
                        _toRet = FillProductStorePrices(_toRet, userId);
                    }
                    else
                    {
                        if (emulate)
                        {
                            //get product
                            var _product = db.Products.Where(c => c.Id == _productToRet.ProductId).FirstOrDefault();
                            if (_product != null)
                            {
                                _toRet = new UserProductListCompleteModel2()
                                {
                                    Id = userProductId * -1,
                                    ProductId = _productToRet.ProductId,
                                    ItemType = _productToRet.ListName.ToLower() == "in" ? "shoppingList" : "inventory",
                                    Name = _product.Name,
                                    Weight = _product.Weight,
                                    Quantity = _productToRet.Quantity.Value,
                                    Price = _product.Price,
                                    Barcode = _product.Barcode,
                                    Brand = _product.Brand,
                                    Category = _product.CategoryString
                                };
                                _toRet = FillProductStorePrices(_toRet, userId);
                            }
                        }
                    }
                    return _toRet;
                }

            }
            catch (Exception ex)
            {
                Logger.Debug("error checking out products: " + ex.Message);
                return null;

            }
        }

        //Returns new or existing inventory user product
        static public UserProductListCompleteModel2 CheckoutProductSimple(int userProductId, string userId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    UserProductsSimple _productToRet = null;

                    var _userProduct = db.UserProductsSimple.Where(c => c.Id == userProductId && c.UserId.Equals(userId)).FirstOrDefault();
                    if (_userProduct != null)
                    {
                        var _isShoppingList = _userProduct.ListName == null;
                        if (_userProduct.ListName != null && _userProduct.ListName == "shoppingList")
                            _isShoppingList = true;
                        //if (_userProduct.ListName == null || _userProduct.ListName.ToLower() == "shoppingList") //checkout shopping list product
                        if (_isShoppingList) //checkout shopping list product
                        {
                            //remove from shopping list and add with same quantity to Inventory and to history ans bought
                            db.UserProductsSimple.Remove(_userProduct);

                            UserProductsSimple _inventoryProductExists = (from c in db.UserProductsSimple
                                                                          where c.Name.ToLower().Equals(_userProduct.Name.ToLower()) &&
                                                                          c.UserId.Equals(userId) &&
                                                                          c.ListName.ToLower().Equals("inventory")
                                                                          select c).FirstOrDefault();

                            //Exist in User List , change quantity
                            if (_inventoryProductExists != null)
                            {

                                _inventoryProductExists.Quantity = _inventoryProductExists.Quantity + _userProduct.Quantity;
                                _inventoryProductExists.UpdateDate = DateTime.Now;
                                db.UserProductsSimple.Attach(_inventoryProductExists);
                                var entry = db.Entry(_inventoryProductExists);
                                //TO REMEMBER
                                entry.Property(y => y.Quantity).IsModified = true;
                                entry.Property(y => y.UpdateDate).IsModified = true;
                                _productToRet = _inventoryProductExists;
                            }
                            //add new product to user In List
                            else
                            {
                                UserProductsSimple _UserProductsList = new UserProductsSimple();
                                _UserProductsList.Name = _userProduct.Name;
                                _UserProductsList.ProductId = _userProduct.ProductId;
                                _UserProductsList.UserId = userId;
                                _UserProductsList.Quantity = _userProduct.Quantity;
                                _UserProductsList.ListName = "inventory";
                                _UserProductsList.UpdateDate = DateTime.Now;
                                _UserProductsList.CreateDate = DateTime.Now;
                                db.UserProductsSimple.Add(_UserProductsList);
                                _productToRet = _UserProductsList;
                            }

                            //add to bought history
                            UserProductsListHistory _UserProductsListHistoryBought = new UserProductsListHistory();
                            _UserProductsListHistoryBought.ProductId = _userProduct.ProductId ?? null;
                            _UserProductsListHistoryBought.ProductName = _userProduct.Name;
                            _UserProductsListHistoryBought.UserId = userId;
                            _UserProductsListHistoryBought.Quantity = _userProduct.Quantity;
                            _UserProductsListHistoryBought.ListName = "bought";
                            _UserProductsListHistoryBought.InsertDate = DateTime.Now;
                            db.UserProductsListHistory.Add(_UserProductsListHistoryBought);
                            db.SaveChanges();

                        }
                        else if (_userProduct.ListName.ToLower() == "inventory") //checkout inventory product
                        {
                            //remove from inventory and add with same quantity to "ShoppingList" and to history ans bought
                            db.UserProductsSimple.Remove(_userProduct);

                            UserProductsSimple _shoppingListPoductExists = (from c in db.UserProductsSimple
                                                                            where c.Name.ToLower().Equals(_userProduct.Name.ToLower()) &&
                                                                            c.UserId.Equals(userId) &&
                                                                            (c.ListName == null ||
                                                                            c.ListName.ToLower().Equals("shoppingList"))
                                                                            select c).FirstOrDefault();

                            //Exist in User shopping list , change quantity
                            if (_shoppingListPoductExists != null)
                            {

                                _shoppingListPoductExists.Quantity = _shoppingListPoductExists.Quantity + _userProduct.Quantity;
                                _shoppingListPoductExists.UpdateDate = DateTime.Now;
                                db.UserProductsSimple.Attach(_shoppingListPoductExists);
                                var entry = db.Entry(_shoppingListPoductExists);
                                //TO REMEMBER
                                entry.Property(y => y.Quantity).IsModified = true;
                                entry.Property(y => y.UpdateDate).IsModified = true;
                                _productToRet = _shoppingListPoductExists;
                            }
                            //add new product to user In List
                            else
                            {
                                UserProductsSimple _UserProductsSimple = new UserProductsSimple();
                                _UserProductsSimple.Name = _userProduct.Name;
                                _UserProductsSimple.ProductId = _userProduct.ProductId ?? null;
                                _UserProductsSimple.UserId = userId;
                                _UserProductsSimple.Quantity = _userProduct.Quantity;
                                _UserProductsSimple.ListName = "shoppingList";
                                _UserProductsSimple.UpdateDate = DateTime.Now;
                                _UserProductsSimple.CreateDate = DateTime.Now;
                                db.UserProductsSimple.Add(_UserProductsSimple);
                                _productToRet = _UserProductsSimple;
                            }

                            //add to consumed history
                            UserProductsListHistory _UserProductsListHistoryBought = new UserProductsListHistory();
                            _UserProductsListHistoryBought.ProductId = _userProduct.ProductId;
                            _UserProductsListHistoryBought.ProductName = _userProduct.Name;
                            _UserProductsListHistoryBought.UserId = userId;
                            _UserProductsListHistoryBought.Quantity = _userProduct.Quantity;
                            _UserProductsListHistoryBought.ListName = "consumed";
                            _UserProductsListHistoryBought.InsertDate = DateTime.Now;
                            db.UserProductsListHistory.Add(_UserProductsListHistoryBought);
                            db.SaveChanges();
                        }
                    }

                    //TODO - code to checkout from inventory back to shopping list, abstract code
                    //foreach (var _userProductShoppingList in _userProducts)
                    //{


                    //arrange right return type for client side parsing
                    var _reQuery = db.UserProductsSimple.Where(c => c.Id == _productToRet.Id).FirstOrDefault();

                    var _listName = string.Empty;

                    if (_reQuery.ListName == null) _listName = "productSimple";
                    else if (_reQuery.ListName == "in") _listName = "productSimple";
                    else if (_reQuery.ListName == "shoppingList") _listName = "productSimple";
                    else if (_reQuery.ListName == "inventory") _listName = "productSimpleInventory";

                    var _toRet = new UserProductListCompleteModel2()
                    {
                        Id = _reQuery.Id,
                        ProductId = _reQuery.ProductId ?? -1,
                        ItemType = _listName,
                        Name = _reQuery.Name,
                        Quantity = _reQuery.Quantity,
                        LastAddedDate = _reQuery.UpdateDate
                    };
                    //_toRet = FillProductStorePrices(_toRet);
                    return _toRet;
                }

            }
            catch (Exception ex)
            {
                Logger.Debug("error checking out products: " + ex.Message);
                return null;

            }
        }

        static public UserProductListCompleteModel2 CheckoutProductSimpleV2(int userProductId, string userId, bool emulate = false)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    UserProductsSimple _productToRet = null;

                    var _userProduct = db.UserProductsSimple.Where(c => c.Id == userProductId && c.UserId.Equals(userId)).FirstOrDefault();
                    if (_userProduct != null)
                    {
                        var _isShoppingList = _userProduct.ListName == null;
                        if (_userProduct.ListName != null && _userProduct.ListName == "shoppingList")
                            _isShoppingList = true;
                        if (_isShoppingList) //checkout shopping list product
                        {
                            //remove from shopping list and add with same quantity to Inventory and to history ans bought
                            db.UserProductsSimple.Remove(_userProduct);

                            UserProductsSimple _inventoryProductExists = (from c in db.UserProductsSimple
                                                                          where c.Name.ToLower().Equals(_userProduct.Name.ToLower()) &&
                                                                          c.UserId.Equals(userId) &&
                                                                          c.ListName.ToLower().Equals("inventory")
                                                                          select c).FirstOrDefault();

                            //Exist in User List , change quantity
                            if (_inventoryProductExists != null)
                            {

                                _inventoryProductExists.Quantity = _inventoryProductExists.Quantity + _userProduct.Quantity;
                                _inventoryProductExists.UpdateDate = DateTime.Now;
                                db.UserProductsSimple.Attach(_inventoryProductExists);
                                var entry = db.Entry(_inventoryProductExists);
                                //TO REMEMBER
                                entry.Property(y => y.Quantity).IsModified = true;
                                entry.Property(y => y.UpdateDate).IsModified = true;
                                _productToRet = _inventoryProductExists;
                            }
                            //add new product to user In List
                            else
                            {
                                UserProductsSimple _UserProductsList = new UserProductsSimple();
                                _UserProductsList.Name = _userProduct.Name;
                                _UserProductsList.ProductId = _userProduct.ProductId;
                                _UserProductsList.UserId = userId;
                                _UserProductsList.Quantity = _userProduct.Quantity;
                                _UserProductsList.ListName = "inventory";
                                _UserProductsList.UpdateDate = DateTime.Now;
                                _UserProductsList.CreateDate = DateTime.Now;
                                db.UserProductsSimple.Add(_UserProductsList);
                                _productToRet = _UserProductsList;
                            }

                            //add to bought history
                            UserProductsListHistory _UserProductsListHistoryBought = new UserProductsListHistory();
                            _UserProductsListHistoryBought.ProductId = _userProduct.ProductId ?? null;
                            _UserProductsListHistoryBought.ProductName = _userProduct.Name;
                            _UserProductsListHistoryBought.UserId = userId;
                            _UserProductsListHistoryBought.Quantity = _userProduct.Quantity;
                            _UserProductsListHistoryBought.ListName = "bought";
                            _UserProductsListHistoryBought.InsertDate = DateTime.Now;
                            db.UserProductsListHistory.Add(_UserProductsListHistoryBought);
                            if (!emulate)
                                db.SaveChanges();

                        }
                        else if (_userProduct.ListName.ToLower() == "inventory") //checkout inventory product
                        {
                            //remove from inventory and add with same quantity to "ShoppingList" and to history ans bought
                            db.UserProductsSimple.Remove(_userProduct);

                            UserProductsSimple _shoppingListPoductExists = (from c in db.UserProductsSimple
                                                                            where c.Name.ToLower().Equals(_userProduct.Name.ToLower()) &&
                                                                            c.UserId.Equals(userId) &&
                                                                            (c.ListName == null ||
                                                                            c.ListName.ToLower().Equals("shoppingList"))
                                                                            select c).FirstOrDefault();

                            //Exist in User shopping list , change quantity
                            if (_shoppingListPoductExists != null)
                            {

                                _shoppingListPoductExists.Quantity = _shoppingListPoductExists.Quantity + _userProduct.Quantity;
                                _shoppingListPoductExists.UpdateDate = DateTime.Now;
                                db.UserProductsSimple.Attach(_shoppingListPoductExists);
                                var entry = db.Entry(_shoppingListPoductExists);
                                //TO REMEMBER
                                entry.Property(y => y.Quantity).IsModified = true;
                                entry.Property(y => y.UpdateDate).IsModified = true;
                                _productToRet = _shoppingListPoductExists;
                            }
                            //add new product to user In List
                            else
                            {
                                UserProductsSimple _UserProductsSimple = new UserProductsSimple();
                                _UserProductsSimple.Name = _userProduct.Name;
                                _UserProductsSimple.ProductId = _userProduct.ProductId ?? null;
                                _UserProductsSimple.UserId = userId;
                                _UserProductsSimple.Quantity = _userProduct.Quantity;
                                _UserProductsSimple.ListName = "shoppingList";
                                _UserProductsSimple.UpdateDate = DateTime.Now;
                                _UserProductsSimple.CreateDate = DateTime.Now;
                                db.UserProductsSimple.Add(_UserProductsSimple);
                                _productToRet = _UserProductsSimple;
                            }

                            //add to consumed history
                            UserProductsListHistory _UserProductsListHistoryBought = new UserProductsListHistory();
                            _UserProductsListHistoryBought.ProductId = _userProduct.ProductId;
                            _UserProductsListHistoryBought.ProductName = _userProduct.Name;
                            _UserProductsListHistoryBought.UserId = userId;
                            _UserProductsListHistoryBought.Quantity = _userProduct.Quantity;
                            _UserProductsListHistoryBought.ListName = "consumed";
                            _UserProductsListHistoryBought.InsertDate = DateTime.Now;
                            db.UserProductsListHistory.Add(_UserProductsListHistoryBought);
                            if (!emulate)
                                db.SaveChanges();
                        }
                    }

                    //TODO - code to checkout from inventory back to shopping list, abstract code
                    //foreach (var _userProductShoppingList in _userProducts)
                    //{


                    //arrange right return type for client side parsing
                    UserProductListCompleteModel2 _toRet = null;
                    if (_productToRet.Id != 0)
                    {
                        var _reQuery = db.UserProductsSimple.Where(c => c.Id == _productToRet.Id).FirstOrDefault();

                        var _listName = string.Empty;
                        if (_reQuery.ListName == null) _listName = "productSimple";
                        else if (_reQuery.ListName == "in") _listName = "productSimple";
                        else if (_reQuery.ListName == "shoppingList") _listName = "productSimple";
                        else if (_reQuery.ListName == "inventory") _listName = "productSimpleInventory";

                        _toRet = new UserProductListCompleteModel2()
                        {
                            Id = _reQuery.Id,
                            ProductId = _reQuery.ProductId ?? -1,
                            ItemType = _listName,
                            Name = _reQuery.Name,
                            Quantity = _reQuery.Quantity,
                            LastAddedDate = _reQuery.UpdateDate
                        };
                    }
                    else
                    {
                        if (emulate)
                        {
                            var _listName = string.Empty;
                            if (_productToRet.ListName == null) _listName = "productSimple";
                            else if (_productToRet.ListName == "in") _listName = "productSimple";
                            else if (_productToRet.ListName == "shoppingList") _listName = "productSimple";
                            else if (_productToRet.ListName == "inventory") _listName = "productSimpleInventory";

                            _toRet = new UserProductListCompleteModel2()
                            {
                                Id = -1,
                                ProductId = _productToRet.ProductId ?? -1,
                                ItemType = _listName,
                                Name = _productToRet.Name,
                                Quantity = _productToRet.Quantity,
                                LastAddedDate = _productToRet.UpdateDate
                            };
                        }
                    }

                    return _toRet;
                }

            }
            catch (Exception ex)
            {
                Logger.Debug("error checking out products: " + ex.Message);
                return null;

            }
        }
        static public UserProductListCompleteModel2 CheckoutProductSimpleV3(int userProductId, string userId, bool emulate = false)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    UserProductsSimple _productToRet = null;

                    var _userProduct = db.UserProductsSimple.Where(c => c.Id == userProductId && c.UserId.Equals(userId)).FirstOrDefault();
                    if (_userProduct != null)
                    {
                        var _isShoppingList = _userProduct.ListName == null;
                        if (_userProduct.ListName != null && _userProduct.ListName == "shoppingList")
                            _isShoppingList = true;
                        if (_isShoppingList) //checkout shopping list product
                        {
                            //remove from shopping list and add with same quantity to Inventory and to history ans bought
                            db.UserProductsSimple.Remove(_userProduct);

                            //check first if a product simple with the same name exists, if yes only update
                            var _exists = db.UserProductsSimple.Where(c => c.Name.ToLower().Trim() == _userProduct.Name.ToLower().Trim() && c.ListName.ToLower() == "inventory" && c.UserId.Equals(userId)).FirstOrDefault();
                            if (_exists != null)
                            {
                                _exists.Quantity += _userProduct.Quantity;
                                _exists.UpdateDate = DateTime.Now;
                                db.Entry(_exists).State = System.Data.Entity.EntityState.Modified;
                                _productToRet = _exists;
                            }
                            else
                            {
                                //add to inventory new
                                UserProductsSimple _UserProductsList = new UserProductsSimple();
                                _UserProductsList.Name = _userProduct.Name;
                                _UserProductsList.ProductId = _userProduct.ProductId;
                                _UserProductsList.UserId = userId;
                                _UserProductsList.Quantity = _userProduct.Quantity;
                                _UserProductsList.ListName = "inventory";
                                _UserProductsList.Image = _userProduct.Image;
                                _UserProductsList.UpdateDate = DateTime.Now;
                                _UserProductsList.CreateDate = DateTime.Now;
                                db.UserProductsSimple.Add(_UserProductsList);
                                _productToRet = _UserProductsList;
                            }


                            //add to bought history
                            UserProductsListHistory _UserProductsListHistoryBought = new UserProductsListHistory();
                            _UserProductsListHistoryBought.ProductId = _userProduct.ProductId ?? null;
                            _UserProductsListHistoryBought.ProductName = _userProduct.Name;
                            _UserProductsListHistoryBought.UserId = userId;
                            _UserProductsListHistoryBought.Quantity = _userProduct.Quantity;
                            _UserProductsListHistoryBought.ListName = "bought";
                            _UserProductsListHistoryBought.InsertDate = DateTime.Now;
                            db.UserProductsListHistory.Add(_UserProductsListHistoryBought);

                            if (!emulate)
                                db.SaveChanges();

                        }
                        else if (_userProduct.ListName.ToLower() == "inventory") //checkout inventory product
                        {
                            //remove from inventory and add with same quantity to "ShoppingList" and to history ans bought
                            db.UserProductsSimple.Remove(_userProduct);

                            //check first if a product simple with the same name exists, if yes only update
                            var _exists = db.UserProductsSimple.Where(c => c.Name.ToLower().Trim() == _userProduct.Name.ToLower().Trim() && c.ListName.ToLower() == "shoppingList" && c.UserId.Equals(userId)).FirstOrDefault();
                            if (_exists != null)
                            {
                                _exists.Quantity += _userProduct.Quantity;
                                _exists.UpdateDate = DateTime.Now;
                                db.Entry(_exists).State = System.Data.Entity.EntityState.Modified;
                                _productToRet = _exists;
                            }
                            else
                            {

                                UserProductsSimple _UserProductsSimple = new UserProductsSimple();
                                _UserProductsSimple.Name = _userProduct.Name;
                                _UserProductsSimple.ProductId = _userProduct.ProductId ?? null;
                                _UserProductsSimple.UserId = userId;
                                _UserProductsSimple.Quantity = _userProduct.Quantity;
                                _UserProductsSimple.ListName = "shoppingList";
                                _UserProductsSimple.UpdateDate = DateTime.Now;
                                _UserProductsSimple.CreateDate = DateTime.Now;
                                _UserProductsSimple.Image = _userProduct.Image;
                                db.UserProductsSimple.Add(_UserProductsSimple);
                                _productToRet = _UserProductsSimple;
                            }

                            //add to consumed history
                            UserProductsListHistory _UserProductsListHistoryBought = new UserProductsListHistory();
                            _UserProductsListHistoryBought.ProductId = _userProduct.ProductId;
                            _UserProductsListHistoryBought.ProductName = _userProduct.Name;
                            _UserProductsListHistoryBought.UserId = userId;
                            _UserProductsListHistoryBought.Quantity = _userProduct.Quantity;
                            _UserProductsListHistoryBought.ListName = "consumed";
                            _UserProductsListHistoryBought.InsertDate = DateTime.Now;
                            db.UserProductsListHistory.Add(_UserProductsListHistoryBought);

                            if (!emulate)
                                db.SaveChanges();
                        }
                    }

                    //TODO - code to checkout from inventory back to shopping list, abstract code
                    //foreach (var _userProductShoppingList in _userProducts)
                    //{


                    //arrange right return type for client side parsing
                    UserProductListCompleteModel2 _toRet = null;
                    if (_productToRet.Id != 0)
                    {
                        var _reQuery = db.UserProductsSimple.Where(c => c.Id == _productToRet.Id).FirstOrDefault();

                        var _listName = string.Empty;
                        if (_reQuery.ListName == null) _listName = "productSimple";
                        else if (_reQuery.ListName == "in") _listName = "productSimple";
                        else if (_reQuery.ListName == "shoppingList") _listName = "productSimple";
                        else if (_reQuery.ListName == "inventory") _listName = "productSimpleInventory";

                        _toRet = new UserProductListCompleteModel2()
                        {
                            Id = _reQuery.Id,
                            ProductId = _reQuery.ProductId ?? -1,
                            ItemType = _listName,
                            Name = _reQuery.Name,
                            Quantity = _reQuery.Quantity,
                            LastAddedDate = _reQuery.UpdateDate
                        };
                    }
                    else
                    {
                        if (emulate)
                        {
                            var _listName = string.Empty;
                            if (_productToRet.ListName == null) _listName = "productSimple";
                            else if (_productToRet.ListName == "in") _listName = "productSimple";
                            else if (_productToRet.ListName == "shoppingList") _listName = "productSimple";
                            else if (_productToRet.ListName == "inventory") _listName = "productSimpleInventory";

                            _toRet = new UserProductListCompleteModel2()
                            {
                                Id = -1,
                                ProductId = _productToRet.ProductId ?? -1,
                                ItemType = _listName,
                                Name = _productToRet.Name,
                                Quantity = _productToRet.Quantity,
                                LastAddedDate = _productToRet.UpdateDate
                            };
                        }
                    }

                    return _toRet;
                }

            }
            catch (Exception ex)
            {
                Logger.Debug("error checking out products: " + ex.Message);
                return null;

            }
        }

        static public int CheckoutProducts(List<int> userProducsIds, string userId, bool addToInventory)
        {
            try
            {
                var _userProducts = db.UserProductsList.Where(c => userProducsIds.Contains(c.Id) && c.UserId.Equals(userId) && c.ListName.ToLower() == "in").ToList();
                foreach (var _userProductShoppingList in _userProducts)
                {

                    //remove from shopping list and add with same quantity to "Despensa" and to history ans bought
                    db.UserProductsList.Remove(_userProductShoppingList);

                    if (addToInventory)
                    {
                        //Add to inventory list
                        UserProductsList _inventoryPoductExists = (from c in db.UserProductsList
                                                                   where c.ProductId.Equals(_userProductShoppingList.ProductId) &&
                                                                   c.UserId.Equals(userId) &&
                                                                   c.ListName.ToLower().Equals("inventory")
                                                                   select c).FirstOrDefault();

                        //Exist in User Lists , change quantity
                        if (_inventoryPoductExists != null)
                        {
                            _inventoryPoductExists.Quantity = _inventoryPoductExists.Quantity + _userProductShoppingList.Quantity;
                            db.UserProductsList.Attach(_inventoryPoductExists);
                            var entry = db.Entry(_inventoryPoductExists);
                            //TO REMEMBER
                            entry.Property(y => y.Quantity).IsModified = true;
                        }
                        //add new product to user In List
                        else
                        {
                            UserProductsList _UserProductsList = new UserProductsList();
                            _UserProductsList.ProductId = _userProductShoppingList.ProductId;
                            _UserProductsList.UserId = userId;
                            _UserProductsList.Quantity = _userProductShoppingList.Quantity;
                            _UserProductsList.ListName = "inventory";

                            db.UserProductsList.Add(_UserProductsList);
                        }


                        //Add to History - Inventory 

                        UserProductsListHistory _UserProductsListHistoryInventory = new UserProductsListHistory();
                        _UserProductsListHistoryInventory.ProductId = _userProductShoppingList.ProductId;
                        _UserProductsListHistoryInventory.UserId = userId;
                        _UserProductsListHistoryInventory.Quantity = _userProductShoppingList.Quantity;
                        _UserProductsListHistoryInventory.ListName = "inventory";
                        _UserProductsListHistoryInventory.InsertDate = DateTime.Now;
                        db.UserProductsListHistory.Add(_UserProductsListHistoryInventory);
                    }

                    //add to bought history
                    UserProductsListHistory _UserProductsListHistoryBought = new UserProductsListHistory();
                    _UserProductsListHistoryBought.ProductId = _userProductShoppingList.ProductId;
                    _UserProductsListHistoryBought.UserId = userId;
                    _UserProductsListHistoryBought.Quantity = _userProductShoppingList.Quantity;
                    _UserProductsListHistoryBought.ListName = "bought";
                    _UserProductsListHistoryBought.InsertDate = DateTime.Now;
                    db.UserProductsListHistory.Add(_UserProductsListHistoryBought);
                }

                db.SaveChanges();
                return _userProducts.Count();

            }
            catch (Exception ex)
            {
                Logger.Debug("error checking out products: " + ex.Message);
                return 0;

            }
        }

        static public int ProductsConsumedFromInventory(List<int> userProducsIds, string userId, bool addToInventory)
        {
            try
            {
                var _userProducts = db.UserProductsList.Where(c => userProducsIds.Contains(c.ProductId) && c.UserId.Equals(userId) && c.ListName.ToLower() == "inventory").ToList();
                foreach (var _userProductShoppingList in _userProducts)
                {
                    //add to consumed list
                    for (int i = 0; i < _userProductShoppingList.Quantity; i++)
                    {
                        UserProductsConsumed _newUserProductsConsumed = new UserProductsConsumed();
                        _newUserProductsConsumed.ProductId = _userProductShoppingList.ProductId;
                        _newUserProductsConsumed.Quantity = 1;
                        _newUserProductsConsumed.UserId = userId;
                        _newUserProductsConsumed.CreateDate = DateTime.Now;
                        db.UserProductsConsumed.Add(_newUserProductsConsumed);
                        db.SaveChanges();
                    }
                    //remove from inventory list and add with same quantity to the shopping list and to history as consumed
                    db.UserProductsList.Remove(_userProductShoppingList);

                    //add to consumed history
                    UserProductsListHistory _UserProductsListHistoryBought = new UserProductsListHistory();
                    _UserProductsListHistoryBought.ProductId = _userProductShoppingList.ProductId;
                    _UserProductsListHistoryBought.UserId = userId;
                    _UserProductsListHistoryBought.Quantity = _userProductShoppingList.Quantity;
                    _UserProductsListHistoryBought.ListName = "consumed";
                    _UserProductsListHistoryBought.InsertDate = DateTime.Now;
                    db.UserProductsListHistory.Add(_UserProductsListHistoryBought);
                }

                db.SaveChanges();
                return _userProducts.Count();

            }
            catch (Exception ex)
            {
                Logger.Debug("error adding consumed products from inventory list" + ex.Message);
                return 0;

            }
        }

        //OLD ONE
        static public List<UserProductListStorePricesModel> GetBuyStoresPrices(string userId, string list)
        {
            List<UserProductListStorePricesModel> _UserProductListStorePricesModelList = new List<UserProductListStorePricesModel>();
            try
            {
                var userShoppingList = from m in db.UserProductsList where m.UserId == userId && m.ListName.ToLower() == "in" select m;
                Dictionary<int, double> _storeTotalPrices = new Dictionary<int, double>();
                foreach (var productUserList in userShoppingList)
                {
                    var productUserListStores = from m in db.StoreProducts where m.ProductId == productUserList.ProductId select m;
                    if (productUserListStores.Count() > 0)
                    {
                        foreach (var productUserListStore in productUserListStores)
                        {
                            var __UserProductListStorePricesModelList = _UserProductListStorePricesModelList.Where(c => c.StoreId == productUserListStore.StoreId).FirstOrDefault();
                            if (__UserProductListStorePricesModelList != null)
                            {
                                __UserProductListStorePricesModelList.ProductsCounter++;
                                __UserProductListStorePricesModelList.TotalPrice += Math.Round(productUserListStore.Price.Value * productUserList.Quantity.Value, 2);
                            }
                            else
                                _UserProductListStorePricesModelList.Add(new UserProductListStorePricesModel { UserId = userId, ListName = "in", StoreId = productUserListStore.StoreId, ProductsCounter = 1, TotalPrice = Math.Round(productUserListStore.Price.Value * productUserList.Quantity.Value, 2) });
                        }
                    }
                }

                foreach (var _UserProductListStorePricesModel in _UserProductListStorePricesModelList)
                {
                    switch (_UserProductListStorePricesModel.StoreId)
                    {
                        case 1:
                            _UserProductListStorePricesModel.StoreName = "Jumbo";
                            break;
                        case 2:
                            _UserProductListStorePricesModel.StoreName = "Continente";
                            break;
                        case 3:
                            _UserProductListStorePricesModel.StoreName = "Pingo Doce";
                            break;
                        default:
                            _UserProductListStorePricesModel.StoreName = "";
                            break;
                    }
                }
                return _UserProductListStorePricesModelList;
            }
            catch (Exception ex)
            {
                Logger.Debug("error returning user producs list store price totals: " + ex.Message);
                return null;

            }
        }

        //OLD ONE
        static public List<UserProductListStorePricesModel> GetBuyStores(string userId, string list)
        {
            List<UserProductListStorePricesModel> _finalList = new List<UserProductListStorePricesModel>();
            List<UserProductListStorePricesModel> _UserProductListStorePricesModelList = new List<UserProductListStorePricesModel>();
            try
            {
                var userShoppingList = from m in db.UserProductsList where m.UserId == userId && m.ListName.ToLower() == list select m;

                var _userShoppingListComplete = new List<UserProductListCompleteModel>();
                _userShoppingListComplete.AddRange(
                    GetUserProductListCompleteModel(userShoppingList.ToList()
                    ));


                Dictionary<int, double> _storeTotalPrices = new Dictionary<int, double>();
                var _userShoppingList = userShoppingList.ToList();
                foreach (var productUserList in _userShoppingList.ToList())
                {
                    var productUserListStores = from m in db.StoreProducts where m.ProductId == productUserList.ProductId select m;
                    if (productUserListStores.Count() > 0)
                    {
                        foreach (var productUserListStore in productUserListStores.ToList())
                        {
                            var __UserProductListStorePricesModelList = _UserProductListStorePricesModelList.Where(c => c.StoreId == productUserListStore.StoreId).FirstOrDefault();
                            if (__UserProductListStorePricesModelList != null)
                            {
                                __UserProductListStorePricesModelList.ProductsCounter++;
                                __UserProductListStorePricesModelList.TotalPrice += Math.Round(productUserListStore.Price.Value * productUserList.Quantity.Value, 2);
                                __UserProductListStorePricesModelList.TotalPrice = Math.Round(__UserProductListStorePricesModelList.TotalPrice, 2);
                            }
                            else
                            {
                                UserProductListStorePricesModel _UserProductListStorePricesModel = new UserProductListStorePricesModel
                                {
                                    ItemType = "store",
                                    UserId = userId,
                                    ListName = list,
                                    StoreName = productUserListStore.Stores.Name,
                                    StoreId = productUserListStore.StoreId,
                                    ProductsCounter = 1,
                                    UserProductsCounter = _userShoppingList.Count,
                                    TotalPrice = Math.Round(productUserListStore.Price.Value * productUserList.Quantity.Value, 2)
                                };
                                _UserProductListStorePricesModelList.Add(_UserProductListStorePricesModel);
                            }
                        }

                    }
                }


                //Item Filter ( common items )
                _finalList.Add(new UserProductListStorePricesModel
                {
                    ItemType = "filter"
                });

                //add store and products after corresponding stores
                foreach (var _storeItem in _UserProductListStorePricesModelList.OrderBy(c => c.StoreId).ToList())
                {
                    _finalList.Add(_storeItem);

                    var _storeProducts = _userShoppingListComplete.Where(c => c.PriceList.Keys.Any(y => y == _storeItem.StoreId.ToString()));
                    //Add store products
                    foreach (var _productItem in _storeProducts.ToList())
                    {
                        UserProductListStorePricesModel _UserProductListStorePricesModel = new UserProductListStorePricesModel();
                        _UserProductListStorePricesModel.ItemType = "storeProduct";
                        _UserProductListStorePricesModel.StoreUserProduct = _productItem;
                        _UserProductListStorePricesModel.ListName = list;
                        _UserProductListStorePricesModel.StoreName = _storeItem.StoreName;
                        _UserProductListStorePricesModel.StoreId = _storeItem.StoreId;
                        _UserProductListStorePricesModel.ProductsCounter = 1;

                        _finalList.Add(_UserProductListStorePricesModel);
                    }
                }

                //Item Filter ( common items )
                _finalList.Add(new UserProductListStorePricesModel
                {
                    ItemType = "confirm"
                });

                return _finalList;
            }
            catch (Exception ex)
            {
                Logger.Debug("error returning user producs list store price totals: " + ex.Message);
                return null;

            }
        }

        //NEW ONE
        static public List<UserProductListStorePricesModel2> GetBuyStoresV2(string userId, string list)
        {
            List<UserProductListStorePricesModel2> _finalList = new List<UserProductListStorePricesModel2>();
            List<UserProductListStorePricesModel2> _UserProductListStorePricesModel2List = new List<UserProductListStorePricesModel2>();
            try
            {
                var userShoppingList = from m in db.UserProductsList where m.UserId == userId && m.ListName.ToLower() == list select m;

                var _userShoppingListComplete = new List<UserProductListCompleteModel2>();
                _userShoppingListComplete.AddRange(
                    GetUserProductListCompleteModel2V2(userId, list)
                    );


                Dictionary<int, double> _storeTotalPrices = new Dictionary<int, double>();
                var _userShoppingList = userShoppingList;
                foreach (var productUserList in _userShoppingList)
                {
                    var productUserListStores = from m in db.StoreProducts where m.ProductId == productUserList.ProductId select m;
                    if (productUserListStores.Count() > 0)
                    {
                        foreach (var productUserListStore in productUserListStores)
                        {
                            var __UserProductListStorePricesModel2List = _UserProductListStorePricesModel2List.Where(c => c.StoreId == productUserListStore.StoreId).FirstOrDefault();
                            if (__UserProductListStorePricesModel2List != null)
                            {
                                __UserProductListStorePricesModel2List.ProductsCounter++;
                                __UserProductListStorePricesModel2List.TotalPrice += Math.Round(productUserListStore.Price.Value * productUserList.Quantity.Value, 2);
                                __UserProductListStorePricesModel2List.TotalPrice = Math.Round(__UserProductListStorePricesModel2List.TotalPrice, 2);
                            }
                            else
                            {
                                UserProductListStorePricesModel2 _UserProductListStorePricesModel2 = new UserProductListStorePricesModel2
                                {
                                    ItemType = "store",
                                    UserId = userId,
                                    ListName = list,
                                    StoreName = productUserListStore.Stores.Name,
                                    StoreId = productUserListStore.StoreId,
                                    ProductsCounter = 1,
                                    UserProductsCounter = _userShoppingList.Count(),
                                    TotalPrice = Math.Round(productUserListStore.Price.Value * productUserList.Quantity.Value, 2)
                                };
                                _UserProductListStorePricesModel2List.Add(_UserProductListStorePricesModel2);
                            }
                        }

                    }
                }


                //Item Filter ( common items and cheapest items)
                _finalList.Add(new UserProductListStorePricesModel2
                {
                    ItemType = "filter"
                });
                //Cheapest identical products Filter
                _finalList.Add(new UserProductListStorePricesModel2
                {
                    ItemType = "filterCheapestIdentical"
                });
                //Cheapest identical products Filter
                _finalList.Add(new UserProductListStorePricesModel2
                {
                    ItemType = "filterMaximumSavings"
                });

                //add store and products after corresponding stores
                foreach (UserProductListStorePricesModel2 _storeItem in _UserProductListStorePricesModel2List.OrderBy(c => c.StoreId))
                {
                    _finalList.Add(_storeItem);

                    var _storeProducts = _userShoppingListComplete.Where(c => c.PriceList.Exists(y => y.StoreId == _storeItem.StoreId));
                    //Add store products
                    foreach (var _productItem in _storeProducts)
                    {
                        UserProductListStorePricesModel2 _UserProductListStorePricesModel2 = new UserProductListStorePricesModel2();
                        _UserProductListStorePricesModel2.ItemType = "storeProduct";
                        _UserProductListStorePricesModel2.StoreUserProduct = _productItem;
                        _UserProductListStorePricesModel2.ListName = list;
                        _UserProductListStorePricesModel2.StoreName = _storeItem.StoreName;
                        _UserProductListStorePricesModel2.StoreId = _storeItem.StoreId;
                        _UserProductListStorePricesModel2.ProductsCounter = 1;

                        _finalList.Add(_UserProductListStorePricesModel2);
                    }
                }
                //Products selected total
                _finalList.Add(new UserProductListStorePricesModel2
                {
                    ItemType = "productsTotal"
                });
                //Item Filter ( common items )
                _finalList.Add(new UserProductListStorePricesModel2
                {
                    ItemType = "confirm"
                });

                return _finalList;
            }
            catch (Exception ex)
            {
                Logger.Debug("error returning user producs list store price totals: " + ex.Message);
                return null;

            }
        }

        static public List<UserProductListCompleteModel> GetUserProductListCompleteModel(List<UserProductsList> userProductList)
        {
            var shoppingListProductsInnerJoinQuery =
                            from userShoppingListProduct in userProductList
                            join prod in db.Products on userShoppingListProduct.ProductId equals prod.Id
                            orderby userShoppingListProduct.Id descending
                            select new Models.UserProductListCompleteModel
                            {
                                Id = userShoppingListProduct.Id,
                                ProductId = prod.Id,
                                Quantity = userShoppingListProduct.Quantity.Value,
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "shoppingList",
                                Name = prod.Name,
                                Category = prod.CategoryString,
                                Price = prod.Price
                            };

            List<Models.UserProductListCompleteModel> _listToReturn = shoppingListProductsInnerJoinQuery.ToList();

            foreach (var productCombined in _listToReturn)
            {
                var userShoppingList = from m in db.StoreProducts where m.ProductId == productCombined.ProductId select m;
                if (userShoppingList.Count() > 0)
                {
                    foreach (var storeProduct in userShoppingList.ToList())
                    {
                        if (productCombined.PriceList == null) productCombined.PriceList = new Dictionary<string, double>();
                        if (!productCombined.PriceList.ContainsKey(storeProduct.StoreId.ToString()))
                            productCombined.PriceList.Add(storeProduct.StoreId.ToString(), Math.Round(storeProduct.Price.Value, 2));

                        if (productCombined.TotalPriceList == null) productCombined.TotalPriceList = new Dictionary<string, double>();
                        if (!productCombined.TotalPriceList.ContainsKey(storeProduct.StoreId.ToString()))
                            productCombined.TotalPriceList.Add(storeProduct.StoreId.ToString(), Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2));


                        if (storeProduct.Stores.Name == "Jumbo") productCombined.Url = storeProduct.Url;
                    }
                }

            }

            if (_listToReturn.Count == 0)
                return null;

            return _listToReturn;
        }

        static public List<UserProductListCompleteModel2> GetUserProductListCompleteModel2(List<UserProductsList> userProductList)
        {
            var shoppingListProductsInnerJoinQuery =
                            from userShoppingListProduct in userProductList
                            join prod in db.Products on userShoppingListProduct.ProductId equals prod.Id
                            orderby userShoppingListProduct.Id descending
                            select new Models.UserProductListCompleteModel2
                            {
                                Id = userShoppingListProduct.Id,
                                ProductId = prod.Id,
                                Quantity = userShoppingListProduct.Quantity.Value,
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "shoppingList",
                                Name = prod.Name,
                                Category = prod.CategoryString,
                                Price = prod.Price
                            };

            List<Models.UserProductListCompleteModel2> _listToReturn = shoppingListProductsInnerJoinQuery.ToList();
            foreach (var productCombined in _listToReturn)
            {
                var userShoppingList = from m in db.StoreProducts where m.ProductId == productCombined.ProductId select m;
                if (userShoppingList.Count() > 0)
                {
                    foreach (var storeProduct in userShoppingList)
                    {
                        if (productCombined.PriceList == null) productCombined.PriceList = new List<Models.StoreProduct>();
                        productCombined.PriceList.Add(new Models.StoreProduct
                        {
                            Id = storeProduct.Id,
                            Price = Math.Round(storeProduct.Price.Value, 2),
                            StoreId = storeProduct.StoreId,
                            Url = storeProduct.Url,
                            CreatedByUserId = storeProduct.UserId,
                            NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
                        });
                    }
                }
            }

            if (_listToReturn.Count == 0)
                return null;

            return _listToReturn;
        }

        static public List<UserProductListCompleteModel2> GetUserProductListCompleteModel2V2(string userId, string list)
        {
            var shoppingListProductsInnerJoinQuery =
                            from m in db.UserProductsList
                            where m.UserId == userId && m.ListName.ToLower() == list
                            join prod in db.Products on m.ProductId equals prod.Id
                            join storePrd in db.StoreProducts on m.ProductId equals storePrd.ProductId
                            orderby m.Id descending
                            select new UserProductListCompleteTempModel
                            {
                                Id = m.Id,
                                ProductId = m.ProductId,
                                Quantity = m.Quantity.Value,
                                Barcode = prod.Barcode,
                                Brand = prod.Brand,
                                ItemType = "shoppingList",
                                Name = prod.Name,
                                Category = prod.CategoryString,
                                //Price = prod.Price,
                                StorePrice = Math.Round(storePrd.Price.Value, 2),
                                StoreId = storePrd.StoreId,
                                StoreProductId = storePrd.Id,
                                NeedsUpdate = storePrd.NeedsUpdate,
                                Url = storePrd.Url,
                                CreatedByUserId = storePrd.UserId
                            };

            var _distinc = shoppingListProductsInnerJoinQuery.GroupBy(c => c.Id);
            //var _distinctCount = _distinc.Count();
            //get distinct UserProduct Id
            var _list = shoppingListProductsInnerJoinQuery.ToList();

            List<Models.UserProductListCompleteModel2> _listToReturn = new List<UserProductListCompleteModel2>();
            int _lastId = -1;
            foreach (var item in _list)
            {
                //is different product
                if (_lastId == -1 || _lastId != item.Id)
                {
                    UserProductListCompleteModel2 _UserProductListCompleteModel2New = new UserProductListCompleteModel2
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Barcode = item.Barcode,
                        Brand = item.Brand,
                        ItemType = "shoppingList",
                        Name = item.Name,
                        Category = item.Category,
                        //Price = item.Price,
                        PriceList = new List<StoreProduct>(),
                        LastAddedDate = item.LastAddedDate
                    };
                    _UserProductListCompleteModel2New.PriceList.Add(new StoreProduct
                    {
                        Id = item.StoreProductId,
                        StoreId = item.StoreId,
                        Price = item.StorePrice,
                        NeedsUpdate = item.NeedsUpdate.Value,
                        Url = item.Url,
                        CreatedByUserId = item.CreatedByUserId
                    });
                    _listToReturn.Add(_UserProductListCompleteModel2New);
                }
                else
                {
                    _listToReturn[_listToReturn.Count - 1].PriceList.Add(new StoreProduct
                    {
                        StoreId = item.StoreId,
                        Price = item.StorePrice,
                        NeedsUpdate = item.NeedsUpdate.Value,
                        Url = item.Url,
                        CreatedByUserId = item.CreatedByUserId
                    });
                }
                _lastId = item.Id;
            }
            //List<Models.UserProductListCompleteModel2> _listToReturn = shoppingListProductsInnerJoinQuery.ToList();
            //foreach (var productCombined in _listToReturn)
            //{
            //    var userShoppingList = from m in db.StoreProducts where m.ProductId == productCombined.ProductId select m;
            //    if (userShoppingList.Count() > 0)
            //    {
            //        foreach (var storeProduct in userShoppingList)
            //        {
            //            if (productCombined.PriceList == null) productCombined.PriceList = new List<Models.StoreProduct>();
            //            productCombined.PriceList.Add(new Models.StoreProduct
            //            {
            //                Id = storeProduct.Id,
            //                Price = Math.Round(storeProduct.Price.Value, 2),
            //                StoreId = storeProduct.StoreId,
            //                Url = storeProduct.Url,
            //                CreatedByUserId = storeProduct.UserId,
            //                NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
            //            });
            //        }
            //    }
            //}

            if (_listToReturn.Count == 0)
                return null;

            return _listToReturn;
        }

        static public List<UserProductListCompleteModel2> GetUserProductListCompleteModelOfIdsList(List<int> userProducsIds, string userId)
        {
            try
            {
                var _userProductsInnerJoinQuery =
                             from userShoppingListProduct in db.UserProductsList
                             where userProducsIds.Contains(userShoppingListProduct.Id)
                             join prod in db.Products on userShoppingListProduct.ProductId equals prod.Id
                             orderby userShoppingListProduct.Id descending
                             select new Models.UserProductListCompleteModel2
                             {
                                 Id = userShoppingListProduct.Id,
                                 ProductId = prod.Id,
                                 Quantity = userShoppingListProduct.Quantity.Value,
                                 Barcode = prod.Barcode,
                                 Brand = prod.Brand,
                                 ItemType = "userProduct",
                                 Name = prod.Name,
                                 Category = prod.CategoryString,
                                 Price = prod.Price
                             };
                var __userProductsInnerJoinQuery = _userProductsInnerJoinQuery.ToList();
                foreach (var productCombined in __userProductsInnerJoinQuery)
                {
                    var userShoppingList = from m in db.StoreProducts where m.ProductId == productCombined.ProductId select m;
                    if (userShoppingList.Count() > 0)
                    {
                        foreach (var storeProduct in userShoppingList)
                        {
                            if (productCombined.PriceList == null) productCombined.PriceList = new List<Models.StoreProduct>();
                            productCombined.PriceList.Add(new Models.StoreProduct
                            {
                                Id = storeProduct.Id,
                                Price = Math.Round(storeProduct.Price.Value * productCombined.Quantity, 2),
                                StoreId = storeProduct.StoreId,
                                Url = storeProduct.Url,
                                CreatedByUserId = storeProduct.UserId,
                                NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false)
                            });
                        }
                    }
                }

                return __userProductsInnerJoinQuery;

            }
            catch (Exception ex)
            {
                Logger.Debug("error checking out products: " + ex.Message);
                return null;

            }
        }

        static public int Remove(int userProductId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    UserProductsList queryExistsInUserList = (from c in db.UserProductsList
                                                              where c.Id.Equals(userProductId)
                                                              select c).FirstOrDefault();
                    if (queryExistsInUserList != null)
                    {
                        db.UserProductsList.Remove(queryExistsInUserList);
                        db.SaveChanges();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("error removing product from user list" + ex.Message);
                return -1;
            }
        }

        static public int RemoveProductSimple(int userProductId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    UserProductsSimple queryExistsInUserList = (from c in db.UserProductsSimple
                                                                where c.Id.Equals(userProductId)
                                                                select c).FirstOrDefault();
                    if (queryExistsInUserList != null)
                    {
                        db.UserProductsSimple.Remove(queryExistsInUserList);
                        db.SaveChanges();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("error removing product from user list" + ex.Message);
                return -1;
            }
        }

        static public UserProductListCompleteModel2 FillProductStorePrices(UserProductListCompleteModel2 userProduct, string userId = "")
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                IOrderedQueryable<StoreProducts> _userProductStores = null;
                if (string.IsNullOrEmpty(userId))
                {
                    _userProductStores = from m in db.StoreProducts where m.ProductId == userProduct.ProductId orderby m.Price select m;

                }
                else
                {
                    _userProductStores = from m in db.StoreProducts
                                         where m.ProductId == userProduct.ProductId &&
                                         (!m.IsTemp.HasValue || m.IsTemp == false || (m.IsTemp.Value && m.UserId == userId))
                                         orderby m.Price
                                         select m;

                }
                if (_userProductStores.Count() > 0)
                {
                    foreach (var storeProduct in _userProductStores)
                    {
                        if (userProduct.PriceList == null) userProduct.PriceList = new List<Models.StoreProduct>();
                        userProduct.PriceList.Add(new Models.StoreProduct
                        {
                            Id = storeProduct.Id,
                            Price = Math.Round(storeProduct.Price.Value * userProduct.Quantity, 2),
                            PriceBase = storeProduct.Price.Value,
                            StoreId = storeProduct.StoreId,
                            Url = Helpers.Extensibility.GetStoreFetcher(storeProduct.StoreId).GetProductViewableUrl("", storeProduct.Url),
                            CreatedByUserId = storeProduct.UserId,
                            NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false),
                            OnlineProductId = storeProduct.OnlineProductId,
                            Brand = storeProduct.Brand,
                            Weight = storeProduct.Weight,
                            Name = storeProduct.Name,
                            Unit = storeProduct.Unit,
                            PriceRatio = storeProduct.PriceRatio.HasValue ? storeProduct.PriceRatio.Value : 0,
                            PriceRatioBase = storeProduct.PriceRatio.HasValue ? storeProduct.PriceRatio.Value : 0,
                            UpdateDate = storeProduct.UpdateDate.HasValue ? storeProduct.UpdateDate.Value : DateTime.MinValue,
                            IsTemp = storeProduct.IsTemp.HasValue ? storeProduct.IsTemp.Value : false,
                        });
                    }
                    return userProduct;
                }
                return null;
            }
        }

        static public UserProductListCompleteModel2 AddProductSimpleToUserList(ProductSimpleItem productSimple)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                UserProductsSimple _productToRet = null;

                var _exists = db.UserProductsSimple.Where(c => c.Name.ToLower().Trim() == productSimple.Name.ToLower().Trim() && c.ListName.ToLower() == productSimple.List.ToLower() && c.UserId == productSimple.UserId).FirstOrDefault();
                if (_exists != null)
                {
                    _exists.Quantity++;
                    _exists.UpdateDate = DateTime.Now;
                    db.SaveChanges();
                    _productToRet = _exists;
                    //return _exists.Id;
                }
                else
                {
                    UserProductsSimple _UserProductsSimpleNew = new UserProductsSimple
                    {
                        Name = productSimple.Name,
                        //ImageUrl = productSimple.ImageUrl,
                        Quantity = 1,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        ListName = productSimple.List,
                        UserId = productSimple.UserId
                    };
                    db.UserProductsSimple.Add(_UserProductsSimpleNew);
                    db.SaveChanges();
                    _productToRet = _UserProductsSimpleNew;
                    //return _UserProductsSimpleNew.Id;
                }
                //arrange right return type for client side parsing
                //var _reQuery = db.UserProductsSimple.Where(c => c.Id == _productToRet.Id).FirstOrDefault();

                var _listName = string.Empty;

                if (_productToRet.ListName == null) _listName = "productSimple";
                else if (_productToRet.ListName == "in") _listName = "productSimple";
                else if (_productToRet.ListName == "shoppingList") _listName = "productSimple";
                else if (_productToRet.ListName == "inventory") _listName = "productSimpleInventory";

                var _toRet = new UserProductListCompleteModel2()
                {
                    Id = _productToRet.Id,
                    ProductId = _productToRet.ProductId ?? -1,
                    ItemType = _listName,
                    Name = _productToRet.Name,
                    Quantity = _productToRet.Quantity,
                    LastAddedDate = _productToRet.UpdateDate
                };
                //_toRet = FillProductStorePrices(_toRet);
                return _toRet;
            }
        }

        static public UserProductListCompleteModel2 AddProductSimpleToUserListV2(ProductSimpleItemV2 productSimple)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                UserProductsSimple _productToRet = null;
                UserProductsSimple _UserProductsSimpleNew = new UserProductsSimple
                {
                    Name = productSimple.Name,
                    Quantity = 1,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    ListName = productSimple.List,
                    UserId = productSimple.UserId
                };
                if (!string.IsNullOrEmpty(productSimple.ImageBase64))
                {
                    var _bytes = ManageImage.Base64ToBytes(productSimple.ImageBase64);
                    _UserProductsSimpleNew.Image = _bytes;
                }
                db.UserProductsSimple.Add(_UserProductsSimpleNew);
                db.SaveChanges();
                _productToRet = _UserProductsSimpleNew;

                var _listName = string.Empty;
                if (_productToRet.ListName == null) _listName = "productSimple";
                else if (_productToRet.ListName == "in") _listName = "productSimple";
                else if (_productToRet.ListName == "shoppingList") _listName = "productSimple";
                else if (_productToRet.ListName == "inventory") _listName = "productSimpleInventory";

                var _toRet = new UserProductListCompleteModel2()
                {
                    Id = _productToRet.Id,
                    //ProductId = _productToRet.ProductId ?? -1,
                    ProductId = -1,
                    ItemType = _listName,
                    Name = _productToRet.Name,
                    Quantity = _productToRet.Quantity,
                    LastAddedDate = _productToRet.UpdateDate
                };
                return _toRet;
            }
        }

        static public int AddProductToList(int productId, string productName, string list, int quantity, decimal? quantityWeight, bool addToHistory, string userId, bool fromLisieHome = false)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                //TODO quantityWeight , insert in list and history
                int _userListProductId = -1;
                switch (list.ToLower())
                {
                    case "shoppinglist":
                        //check if already exists in a user product list
                        UserProductsList queryExistsInUserList = (from c in db.UserProductsList
                                                                  where c.ProductId.Equals(productId) &&
                                                                  c.UserId.Equals(userId) &&
                                                                  c.ListName.Equals("In")
                                                                  select c).FirstOrDefault();

                        //Exist in User Lists , change quantity
                        if (queryExistsInUserList != null)
                        {
                            queryExistsInUserList.Quantity = queryExistsInUserList.Quantity + quantity;
                            queryExistsInUserList.LastAddedDate = DateTime.Now;
                            db.UserProductsList.Attach(queryExistsInUserList);
                            var entry = db.Entry(queryExistsInUserList);
                            entry.Property(y => y.Quantity).IsModified = true;
                            entry.Property(y => y.LastAddedDate).IsModified = true;
                            db.SaveChanges();
                            _userListProductId = queryExistsInUserList.Id;
                        }
                        //add new product to user In List
                        else
                        {
                            UserProductsList _UserProductsList = new UserProductsList();
                            _UserProductsList.ProductId = productId;
                            _UserProductsList.UserId = userId;
                            _UserProductsList.Quantity = 1;
                            _UserProductsList.ListName = "In";
                            _UserProductsList.LastAddedDate = DateTime.Now;

                            db.UserProductsList.Add(_UserProductsList);
                            db.SaveChanges();
                            _userListProductId = _UserProductsList.Id;
                        }

                        Helpers.FirebaseAndroid.SendNotification(userId, "refreshListShoppingList:" + productName);
                        //Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCart(_userListProductId, userId);
                        break;
                    case "consumed": //Now consumed is obsolete, add to shopping cart, but still have the behaiviour of consumed (remove from inventory, and add to consumed history)
                                     //OBSOLETE
                                     //UserProductsConsumed _newUserProductsConsumed = new UserProductsConsumed();
                                     //_newUserProductsConsumed.ProductId = productId;
                                     //_newUserProductsConsumed.Quantity = 1;
                                     //_newUserProductsConsumed.UserId = userId;
                                     //_newUserProductsConsumed.CreateDate = DateTime.Now;
                                     //db.UserProductsConsumed.Add(_newUserProductsConsumed);
                                     //db.SaveChanges();

                        //NEW - add to shopping list
                        UserProductsList _queryExistsInUserList = (from c in db.UserProductsList
                                                                   where c.ProductId.Equals(productId) &&
                                                                   c.UserId.Equals(userId) &&
                                                                   c.ListName.Equals("In")
                                                                   select c).FirstOrDefault();

                        //Exist in User Lists , change quantity
                        if (_queryExistsInUserList != null)
                        {
                            _queryExistsInUserList.Quantity = _queryExistsInUserList.Quantity + quantity;
                            _queryExistsInUserList.LastAddedDate = DateTime.Now;
                            db.UserProductsList.Attach(_queryExistsInUserList);
                            var entry = db.Entry(_queryExistsInUserList);
                            entry.Property(y => y.Quantity).IsModified = true;
                            entry.Property(y => y.LastAddedDate).IsModified = true;
                            db.SaveChanges();
                            _userListProductId = _queryExistsInUserList.Id;
                        }
                        //add new product to user In List
                        else
                        {
                            UserProductsList _UserProductsList = new UserProductsList();
                            _UserProductsList.ProductId = productId;
                            _UserProductsList.UserId = userId;
                            _UserProductsList.Quantity = 1;
                            _UserProductsList.ListName = "In";
                            _UserProductsList.LastAddedDate = DateTime.Now;

                            db.UserProductsList.Add(_UserProductsList);
                            db.SaveChanges();
                            _userListProductId = _UserProductsList.Id;
                        }
                        db.SaveChanges();

                        //Check if item is in inventory and if it is, remove in the same quantity
                        UserProductsList _inventoryProduct = (from c in db.UserProductsList
                                                              where c.ProductId.Equals(productId) &&
                                                              c.UserId.Equals(userId) &&
                                                              c.ListName.ToLower().Equals("inventory")
                                                              select c).FirstOrDefault();

                        //Exist in Inventory , change quantity or remove
                        if (_inventoryProduct != null)
                        {
                            if (_inventoryProduct.Quantity - quantity != 0)
                            {
                                _inventoryProduct.Quantity = _inventoryProduct.Quantity - quantity;
                                db.UserProductsList.Attach(_inventoryProduct);
                                var entry = db.Entry(_inventoryProduct);
                                //TO REMEMBER
                                entry.Property(y => y.Quantity).IsModified = true;
                                db.SaveChanges();
                            }
                            else
                            {
                                db.UserProductsList.Remove(_inventoryProduct);
                                db.SaveChanges();
                            }
                            //-2 code for added to inventory and removed from shopping list
                            if (fromLisieHome)
                                _userListProductId = -2;
                        }

                        Helpers.FirebaseAndroid.SendNotification(userId, "refreshListConsumed:" + productName);
                        break;
                    case "bought":
                    case "inventory":
                        UserProductsList _inventoryPoductExists = (from c in db.UserProductsList
                                                                   where c.ProductId.Equals(productId) &&
                                                                   c.UserId.Equals(userId) &&
                                                                   c.ListName.ToLower().Equals("inventory")
                                                                   select c).FirstOrDefault();

                        //Exist in User Lists , change quantity
                        if (_inventoryPoductExists != null)
                        {
                            _inventoryPoductExists.Quantity = _inventoryPoductExists.Quantity + quantity;
                            _inventoryPoductExists.LastAddedDate = DateTime.Now;
                            db.UserProductsList.Attach(_inventoryPoductExists);
                            var entry = db.Entry(_inventoryPoductExists);
                            //TO REMEMBER
                            entry.Property(y => y.Quantity).IsModified = true;
                            entry.Property(y => y.LastAddedDate).IsModified = true;
                            db.SaveChanges();
                            _userListProductId = _inventoryPoductExists.Id;
                        }
                        //add new product to user In List
                        else
                        {
                            UserProductsList _UserProductsList = new UserProductsList();
                            _UserProductsList.ProductId = productId;
                            _UserProductsList.UserId = userId;
                            _UserProductsList.Quantity = 1;
                            _UserProductsList.ListName = "inventory";
                            _UserProductsList.LastAddedDate = DateTime.Now;
                            db.UserProductsList.Add(_UserProductsList);
                            db.SaveChanges();
                            _userListProductId = _UserProductsList.Id;
                        }

                        //if list is bought
                        //check if exists in shoppingList, if it exists remove
                        //Check if item is in inventory and if it is, remove in the same quantity
                        if (list.ToLower() == "bought")
                        {

                            UserProductsList _shoppingListProduct = (from c in db.UserProductsList
                                                                     where c.ProductId.Equals(productId) &&
                                                                     c.UserId.Equals(userId) &&
                                                                     c.ListName.ToLower().Equals("in")
                                                                     select c).FirstOrDefault();

                            //Exist in Inventory , change quantity or remove
                            if (_shoppingListProduct != null)
                            {
                                if (_shoppingListProduct.Quantity - quantity != 0)
                                {
                                    _shoppingListProduct.Quantity = _shoppingListProduct.Quantity - quantity;
                                    db.UserProductsList.Attach(_shoppingListProduct);
                                    var entry = db.Entry(_shoppingListProduct);
                                    //TO REMEMBER
                                    entry.Property(y => y.Quantity).IsModified = true;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    db.UserProductsList.Remove(_shoppingListProduct);
                                    db.SaveChanges();
                                }
                                //-2 code for added to inventory and removed from shopping list
                                if (fromLisieHome)
                                    _userListProductId = -3;
                            }
                            Helpers.FirebaseAndroid.SendNotification(userId, "refreshListBought:" + productName);
                        }
                        else
                        {
                            Helpers.FirebaseAndroid.SendNotification(userId, "refreshListInventory:" + productName);
                        }

                        //Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastUpdateShoppingCart(_userListProductId, userId);
                        break;
                    default:
                        break;
                }

                if (addToHistory)
                {
                    //Add to History
                    UserProductsListHistory _UserProductsListHistory = new UserProductsListHistory();
                    _UserProductsListHistory.ProductId = productId;
                    _UserProductsListHistory.UserId = userId;
                    _UserProductsListHistory.Quantity = quantity;
                    _UserProductsListHistory.ListName = list;
                    _UserProductsListHistory.InsertDate = DateTime.Now;
                    _UserProductsListHistory.LisieHome = fromLisieHome;
                    db.UserProductsListHistory.Add(_UserProductsListHistory);
                    db.SaveChanges();
                }

                return _userListProductId;
            }
        }

        static public int AddProductToLists(Products product, List<string> lists, string userId)
        {
            int _userProductId = -1;
            if (product != null)
            {
                foreach (string _list in lists)
                {
                    _userProductId = AddProductToList(product.Id, product.Name, _list, 1, null, true, userId);
                }
            }
            return _userProductId;
        }

        static async public Task<UserProductListCompleteModel2> AddProductByBarcode(string barcode, string userId, string list)
        {
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                List<LisieStores.Extensibility.ProductSearchResult> _results = null;

                //var _product = ProductsManager.GetByBarcode(barcode);
                var _product = ProductsManager.GetByBarcode(barcode);
                if (_product != null)
                {
                    //return GetCompleteModel(_product.Id);
                    var _userProductId = AddProductToList(_product.Id, _product.Name, list, 1, null, true, userId);
                    return GetCompleteModel(_userProductId);
                }
                else //Find online 
                {
                    _results = await ProductsManager.GetByBarcodeOnline(barcode);
                }

                //if found online, create new product and return it
                if (_results != null && _results.Count > 0)
                {
                    ProductItemCreate _ProductItemCreate = new ProductItemCreate();
                    _ProductItemCreate.Barcode = _results[0].Barcode;
                    _ProductItemCreate.FirstAddedProductFromStoreId = _results[0].StoreId;
                    _ProductItemCreate.UserId = userId;
                    _ProductItemCreate.Lists = new string[] { list }.ToList();
                    _ProductItemCreate.SelectedResults = new List<LisieStores.Extensibility.ProductSearchResult>();
                    foreach (var _result in _results)
                    {
                        _ProductItemCreate.SelectedResults.Add(new LisieStores.Extensibility.ProductSearchResult
                        {
                            StoreId = _result.StoreId,
                            Url = _result.Url
                        });
                    }
                    int _newUerProductId = await ProductsManager.CreateV2(_ProductItemCreate);
                    if (_newUerProductId > 0)
                    {
                        return UserListsManager.GetCompleteModel(_newUerProductId);
                    }
                }

                return null;
            }
        }


        //static async public Task<UserProductListCompleteModel2> AddProductByBarcodeV2(string barcode, string userId, string list)
        //{
        //    using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
        //    {
        //        List<LisieStores.Extensibility.ProductSearchResult> _results = null;

        //        //var _product = ProductsManager.GetByBarcode(barcode);
        //        var _product = ProductsManager.GetByBarcodeV2(barcode, userId);
        //        if (_product != null)
        //        {
        //            //return GetCompleteModel(_product.Id);
        //            var _userProductId = AddProductToList(_product.Id, _product.Name, list, 1, null, true, userId);
        //            return GetCompleteModel(_userProductId);
        //        }
        //        else //Find online 
        //        {
        //            _results = await ProductsManager.GetByBarcodeOnline(barcode);
        //        }

        //        //if found online, create new product and return it
        //        if (_results != null && _results.Count > 0)
        //        {
        //            ProductItemCreate _ProductItemCreate = new ProductItemCreate();
        //            _ProductItemCreate.Barcode = _results[0].Barcode;
        //            _ProductItemCreate.FirstAddedProductFromStoreId = _results[0].StoreId;
        //            _ProductItemCreate.UserId = userId;
        //            _ProductItemCreate.Lists = new string[] { list }.ToList();
        //            _ProductItemCreate.SelectedResults = new List<LisieStores.Extensibility.ProductSearchResult>();
        //            foreach (var _result in _results)
        //            {
        //                _ProductItemCreate.SelectedResults.Add(new LisieStores.Extensibility.ProductSearchResult
        //                {
        //                    StoreId = _result.StoreId,
        //                    Url = _result.Url
        //                });
        //            }
        //            int _newUerProductId = await ProductsManager.CreateV2(_ProductItemCreate);
        //            if (_newUerProductId > 0)
        //            {
        //                return UserListsManager.GetCompleteModel(_newUerProductId);
        //            }
        //        }

        //        return null;
        //    }
        //}


        static async public Task<UserProductListCompleteModel2> AddProductByBarcodeV2(string barcode, string userId, string list)
        {
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                List<LisieStores.Extensibility.ProductSearchResult> _results = null;

                var _product = ProductsManager.GetByBarcode(barcode);
                //var _product = ProductsManager.GetByBarcodeV2(barcode, userId);
                if (_product != null)
                {
                    //return GetCompleteModel(_product.Id);
                    var _userProductId = AddProductToList(_product.Id, _product.Name, list, 1, null, true, userId);
                    return GetCompleteModelV2(_userProductId, userId);
                }
                else //Find online 
                {
                    _results = await ProductsManager.GetByBarcodeOnline(barcode);
                }

                //if found online, create new product and return it
                if (_results != null && _results.Count > 0)
                {
                    ProductItemCreate _ProductItemCreate = new ProductItemCreate();
                    _ProductItemCreate.Barcode = _results[0].Barcode;
                    _ProductItemCreate.FirstAddedProductFromStoreId = _results[0].StoreId;
                    _ProductItemCreate.UserId = userId;
                    _ProductItemCreate.Lists = new string[] { list }.ToList();
                    _ProductItemCreate.SelectedResults = new List<LisieStores.Extensibility.ProductSearchResult>();
                    foreach (var _result in _results)
                    {
                        _ProductItemCreate.SelectedResults.Add(new LisieStores.Extensibility.ProductSearchResult
                        {
                            StoreId = _result.StoreId,
                            Url = _result.Url
                        });
                    }
                    var _response = await ProductsManager.CreateV4(_ProductItemCreate);
                    if (_response.Success)
                    {
                        return GetCompleteModelV2((_response.Data as UserProductListCompleteModel2).Id, userId);
                    }
                    else
                    {
                        return null;
                    }
                    //if (_newUerProductId > 0)
                    //{

                    //}
                }

                return null;
            }
        }

        static async public Task<int> AddProductByBarcodeFromLisieHome(string barcode, string userId, string list)
        {
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                List<LisieStores.Extensibility.ProductSearchResult> _results = null;

                var _product = ProductsManager.GetByBarcode(barcode);

                //check if last lisie home entry was more then 12 hours ago and list = "bought"
                //if true, change user list to "consumed"
                if (list == "bought")
                {
                    var _lastUserHistoryEntry = UserHistoryManager.GetLastEntry(userId, list);
                    if (_lastUserHistoryEntry != null)

                    {
                        var _hours = (DateTime.Now - _lastUserHistoryEntry.InsertDate).TotalHours;
                        if (_hours >= 12)
                        {
                            Managers.LisieHomeManager.SetUserState(userId, "consumed");
                            list = "consumed";
                        }
                    }
                }

                if (_product != null)
                {
                    return AddProductToList(_product.Id, _product.Name, list, 1, null, true, userId, true);
                }
                else //Find online 
                {
                    _results = await ProductsManager.GetByBarcodeOnline(barcode);
                }



                //if found online, create new product and return it
                if (_results != null && _results.Count > 0)
                {
                    ProductItemCreate _ProductItemCreate = new ProductItemCreate();
                    _ProductItemCreate.Barcode = _results[0].Barcode;
                    _ProductItemCreate.FirstAddedProductFromStoreId = _results[0].StoreId;
                    _ProductItemCreate.UserId = userId;
                    _ProductItemCreate.Lists = new string[] { list }.ToList();
                    _ProductItemCreate.SelectedResults = new List<LisieStores.Extensibility.ProductSearchResult>();
                    foreach (var _result in _results)
                    {
                        _ProductItemCreate.SelectedResults.Add(new LisieStores.Extensibility.ProductSearchResult
                        {
                            StoreId = _result.StoreId,
                            Url = _result.Url
                        });
                    }
                    int _newUerProductId = await ProductsManager.CreateV2(_ProductItemCreate);
                    if (_newUerProductId > 0)
                    {
                        return _newUerProductId;
                    }
                }

                return -1;
            }
        }

        static public bool SaveProductSimpleImage(int id, byte[] image)
        {
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                var _productSimple = db.UserProductsSimple.Where(c => c.Id == id).FirstOrDefault();
                if (_productSimple != null)
                {
                    _productSimple.Image = image;
                    db.Entry(_productSimple).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    return false;
                }
                return true;
            }
        }
        static public bool UpdateProductSimpleName(int id, string name)
        {
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                var _productSimple = db.UserProductsSimple.Where(c => c.Id == id).FirstOrDefault();
                if (_productSimple != null)
                {
                    _productSimple.Name = name;
                    db.Entry(_productSimple).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    return false;
                }
                return true;
            }
        }


        static public bool RecordUserStoresProductsTotalSavings(string userId, string storeIds, double minPrice, double maxPrice, double priceDifference, int totalProducts, int savings)
        {
            try
            {
                using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
                {
                    UserTotalSavings _newUserTotalSavings = new UserTotalSavings
                    {
                        UserId = userId,
                        StoreIds = storeIds,
                        MinPrice = minPrice,
                        MaxPrice = maxPrice,
                        PriceDifference = priceDifference,
                        TotalProducts = totalProducts,
                        Savings = savings,
                        CreateDate = DateTime.Now
                    };
                    db2.UserTotalSavings.Add(_newUserTotalSavings);
                    db2.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        static public List<int> GetProductIdsOfAllUsers()
        {
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                var _data =
                       from user in db.AspNetUsers
                       join userProduct in db.UserProductsList on user.Id equals userProduct.UserId
                       select new
                       {
                           ProductId = userProduct.ProductId
                       };

                var _distinct = _data.DistinctBy(c => c.ProductId).Select(c => c.ProductId).ToList();
                return _distinct;
            }
        }


        static public void SetUserProductsCategories(string userId)
        {
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                var userProductsList =
                        from userProductList in db.UserProductsList
                        join prod in db.Products on userProductList.ProductId equals prod.Id
                        where userProductList.UserId == userId && (!prod.CategoryCalculated.HasValue || !prod.CategoryCalculated.Value)
                        orderby userProductList.ProductId descending
                        select userProductList;
                //var userProductsList = db.UserProductsList.Where(c => c.UserId == userId ).DistinctBy(u => u.ProductId).OrderByDescending(c => c.ProductId);

                foreach (var userProduct in userProductsList)
                {
                    try
                    {
                        var _calculatedCategory = Managers.ProductsManager.CalculateProductCategory(userProduct.ProductId);
                        var _product = db.Products.Where(c => c.Id == userProduct.ProductId).FirstOrDefault();
                        if (_product != null)
                        {
                            _product.CategoryString = _calculatedCategory;
                            _product.CategoryCalculated = true;
                            //db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        //throw;
                    }

                }
                db.SaveChanges();

            }
        }

        static public JsonApiResponse ChangeQuantity(string userId, int userProductListId, int newQuantity = -1, double newQuantityWeight = -1)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {

                    var _product = db.UserProductsList.Where(c => c.UserId == userId && c.Id == userProductListId).FirstOrDefault();
                    if (_product != null)
                    {
                        if (newQuantity != -1)
                            _product.Quantity = newQuantity;
                        if (newQuantityWeight != -1)
                            _product.QuantityWeight = newQuantityWeight;
                        db.SaveChanges();
                        return new JsonApiResponse
                        {
                            Success = true,
                            Code = 1,
                            Message = "Product quantity changed. userProductListId: " + userProductListId,
                            Data = GetCompleteModelV2(userProductListId)
                        };
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -2,
                            Message = "Product of list not found",
                            Data = null
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("error> " + ex.Message);
                return new JsonApiResponse
                {
                    Success = false,
                    Code = -1,
                    Message = "Error: " + ex.Message,
                    Data = null
                };
            }
        }

        static async public Task<List<StoreProducts>>  UpdateUserProductsWithAI(string userId)
        {

            var userProducts = GetV4(userId);
            foreach (var userProduct in userProducts)
            {
                var storeProducts = await ProductsManager.FindStoreProductsWithAI(userProduct.ProductId);
                //return storeProducts;
            }
            return new List<StoreProducts>();
        }

    }
}