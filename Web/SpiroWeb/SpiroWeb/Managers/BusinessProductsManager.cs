using ClassLibrary1;
using Microsoft.Ajax.Utilities;
using SpiroWeb.Helpers;
using SpiroWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace SpiroWeb.Managers
{
    public static class BusinessProductsManager
    {
        static public JsonApiResponse GetProductByBarcode(int businessId, string barcode)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _product = GetByBarcodeV2(barcode, businessId);
                    if (_product != null)
                    {
                        return new JsonApiResponse
                        {
                            Success = true,
                            Code = 1,
                            Message = "Sucess",
                            Data = _product
                        };
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -1,
                            Message = "Product not found"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);

                return new JsonApiResponse
                {
                    Success = false,
                    Code = -10,
                    Message = "Error: " + ex.Message
                };
            }
        }


        static public BusinessProducts GetByBarcodeV2(string barcode, int businessId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _product = db.BusinessProducts
                    .Where(c => c.Barcode.Equals(barcode) && c.BusinessId.Equals(businessId))
                    .FirstOrDefault();
                if (_product != null)
                {
                    return _product;
                }

                if (barcode.StartsWith("0"))
                {
                    _product = db.BusinessProducts
                        .Where(c => c.Barcode.Equals(barcode.Remove(0, 1)) && c.BusinessId.Equals(businessId))
                        .FirstOrDefault();
                    if (_product != null)
                    {
                        return _product;
                    }

                }
                return null;
            }
        }

        static public JsonApiResponse GetBusinessLists(int businessId)
        {
            try
            {
                var _lists = _GetBusinessLists(businessId);
                return new JsonApiResponse
                {
                    Success = true,
                    Code = 1,
                    Message = "Success",
                    Data = _lists
                };
            }
            catch (Exception ex)
            {

                Logger.Debug(ex.Message);

                return new JsonApiResponse
                {
                    Success = false,
                    Code = -10,
                    Message = "Error: " + ex.Message
                };
            }
        }


        static public List<BusinessProductListDTO> _GetBusinessLists(int businessId)
        {
            List<BusinessProductListDTO> _listToReturn = new List<BusinessProductListDTO>();

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {

                //ShoppingList Products Full
                //var _shoppingListProductsSimple = db.UserProductsSimple
                //    .Where(c => c.UserId == userId && c.Quantity != 0 && (c.ListName == null || c.ListName.Equals("shoppingList"))).OrderByDescending(c => c.UpdateDate).Select(c => new Models.UserProductListCompleteModel2
                //{
                //    Id = c.Id,
                //    Quantity = c.Quantity,
                //    ItemType = "productSimple",
                //    Url = c.ImageUrl,
                //    Name = c.Name,
                //    LastAddedDate = c.UpdateDate
                //});
                //if (_shoppingListProductsSimple.Count() > 0)
                //    _listToReturn.AddRange(_shoppingListProductsSimple);


                var _tempShoppingList =
                   from businessProduct in db.BusinessProductsList
                   join product in db.BusinessProducts on businessProduct.ProductId equals product.Id
                   where businessProduct.BusinessId == businessId && businessProduct.ListName == "shoppingList"
                   //orderby storePrd.Price
                   select new BusinessProductListDTO
                   {
                       Id = businessProduct.Id,
                       BusinessId = businessId,
                       ProductId = businessProduct.ProductId,
                       Quantity = businessProduct.Quantity.Value,
                       Barcode = product.Barcode,
                       //Brand = product.Brand,
                       ItemType = "shoppingList",
                       Name = product.Name,
                       Price = product.Price ?? 0,
                       LastAddedDate = businessProduct.LastAddedDate,
                       Category = product.Category,
                       //Category = product.CategoryString,
                       //CreatedByUserId = storePrd.UserId,
                       //Weight = product.Weight,
                   };

                _listToReturn.AddRange(_tempShoppingList.OrderByDescending(c => c.LastAddedDate));


                var _tempInventoryList =
                   from businessProduct in db.BusinessProductsList
                   join product in db.BusinessProducts on businessProduct.ProductId equals product.Id
                   where businessProduct.BusinessId == businessId && businessProduct.ListName == "inventory"
                   //orderby storePrd.Price
                   select new BusinessProductListDTO
                   {
                       Id = businessProduct.Id,
                       BusinessId = businessProduct.BusinessId,
                       ProductId = businessProduct.ProductId,
                       Quantity = businessProduct.Quantity.Value,
                       Barcode = product.Barcode,
                       //Brand = product.Brand,
                       ItemType = "inventory",
                       Name = product.Name,
                       Price = product.Price ?? 0,
                       LastAddedDate = businessProduct.LastAddedDate,
                       Category = product.Category,
                       //Category = product.CategoryString,
                       //CreatedByUserId = storePrd.UserId,
                       //Weight = product.Weight,
                   };

                _listToReturn.AddRange(_tempInventoryList.OrderByDescending(c => c.LastAddedDate));

                return _listToReturn;
            }
        }

        static public JsonApiResponse GetAll(int page, string query, int businessId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    IQueryable<BusinessProducts> productsList = Enumerable.Empty<BusinessProducts>().AsQueryable();
                    if (string.IsNullOrEmpty(query))
                    {
                        productsList = db.BusinessProducts.Where(c => c.BusinessId == businessId).OrderBy(c => c.Name);
                    }
                    else
                    {
                        //IN FUTURE MAYBE
                        if (page > 0)
                        {
                            productsList = db.BusinessProducts.Where(c => c.BusinessId == businessId && c.Name.ToLower().Contains(query.ToLower()))
                                .OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6);
                        }
                        else //if -1 return all
                        {
                            var decomposed = query.Normalize(NormalizationForm.FormD);
                            var filtered = decomposed.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
                            var _normalizedQuery = new String(filtered.ToArray());

                            string[] _searchWords = query.ToLower().Trim(' ').Split(' ');
                            string[] _searchWordsNormalized = _normalizedQuery.ToLower().Trim(' ').Split(' ');
                            productsList = db.BusinessProducts.Where(c => (c.BusinessId == businessId && (_searchWords.All(z => (c.Name.ToLower()).Contains(z)) ||
                             _searchWordsNormalized.All(z => (c.Name.ToLower()).Contains(z)))))
                                    .OrderBy(c => c.Name);
                        }
                    }


                    List<BusinessProductDTO> _listToReturn = new List<BusinessProductDTO>();
                    _listToReturn = productsList.Select(c => new BusinessProductDTO
                    {
                        Barcode = c.Barcode,
                        BusinessId = c.BusinessId,
                        Name = c.Name,
                        Price = c.Price ?? 0,
                        CreateDate = c.CreateDate,
                        ProductId = c.Id
                    }).ToList();

                    return new JsonApiResponse
                    {
                        Code = 1,
                        Success = true,
                        Data = _listToReturn,
                        Message = "Success"
                    };
                }
            }
            catch (Exception ex)
            {
                return new JsonApiResponse
                {
                    Code = -10,
                    Success = false,
                    Data = null,
                    Message = ex.Message
                };
                throw;
            }

        }

        static public JsonApiResponse CreateProduct(BusinessProductListPostModel product)
        {

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _productExists = GetByBarcodeV2(product.Barcode, product.BusinessId);
                if (_productExists != null)
                {
                    return new JsonApiResponse
                    {
                        Success = true,
                        Code = 1,
                        Message = "Product with barcode already found. Just added to list. ProductId " + _productExists.Id,
                        Data = AddToList(product.BusinessId, _productExists.Id, product.ItemList, 1).Data
                    };
                }

                BusinessProducts _newBusinessProduct = new BusinessProducts
                {
                    Barcode = product.Barcode,
                    Name = product.Name,
                    BusinessId = product.BusinessId,
                    Price = TextTools.ParsePrice(product.Price),
                    CreateDate = DateTime.Now,
                    Category = product.Category,
                };

                if (!string.IsNullOrEmpty(product.ImageBase64))
                {
                    var _bytes = ManageImage.Base64ToBytes(product.ImageBase64);
                    _newBusinessProduct.Image = _bytes;
                }
                db.BusinessProducts.Add(_newBusinessProduct);
                db.SaveChanges();

                return new JsonApiResponse
                {
                    Success = true,
                    Code = 2,
                    Message = "Product with new barcode created, and added to list. ProductId " + _newBusinessProduct.Id,
                    Data = AddToList(product.BusinessId, _newBusinessProduct.Id, product.ItemList, 1).Data
                };
            }
        }

        static public JsonApiResponse EditProduct(BusinessProductListPostModel product)
        {

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                try
                {
                    var _productExists = db.BusinessProducts.Where(c => c.Id == product.ProductId && c.BusinessId == product.BusinessId).FirstOrDefault();
                    if (_productExists == null)
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -1,
                            Message = "Product with id not found - " + product.ProductId,
                            Data = null
                        };
                    }

                    _productExists.Name = product.Name;
                    _productExists.Price = TextTools.ParsePrice(product.Price);

                    //if (HttpContext.Current.Request.IsLocal)
                    //    _productExists.Price = TextTools.ParsePriceLocal(product.Price);
                    //else
                    //    _productExists.Price = TextTools.ParsePriceProduction(product.Price);

                    if (!string.IsNullOrEmpty(product.ImageBase64))
                    {
                        var _bytes = ManageImage.Base64ToBytes(product.ImageBase64);
                        _productExists.Image = _bytes;
                    }

                    if (!string.IsNullOrEmpty(product.Category) || product.Category == string.Empty)
                    {
                        _productExists.Category = product.Category;
                    }

                    db.SaveChanges();

                    return new JsonApiResponse
                    {
                        Success = true,
                        Code = 1,
                        Message = "Product edited with success. ProductId " + _productExists.Id,
                        Data = new BusinessProductListDTO
                        {
                            Id = -1,
                            BusinessId = _productExists.BusinessId,
                            ProductId = _productExists.Id,
                            Quantity = -1,
                            Barcode = _productExists.Barcode,
                            //Brand = product.Brand,
                            ItemType = string.Empty,
                            Name = _productExists.Name,
                            Price = _productExists.Price ?? 0,
                            Category = _productExists.Category
                            //CreatedByUserId = ,
                            //Weight = product.Weight,
                        }
                    };
                }
                catch (Exception ex)
                {
                    return new JsonApiResponse
                    {
                        Success = false,
                        Code = -10,
                        Message = "Error: " + ex.Message,
                        Data = null
                    };
                }

            }
        }

        //static public JsonApiResponse AddToList(int businessId, int productId, string itemList, int quantity)
        //{
        //    using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
        //    {
        //        try
        //        {
        //            BusinessProductsList _productToRet = null;

        //            //first check if there is product with barcode
        //            var _productExists = db.BusinessProducts.Where(c => c.Id == productId).FirstOrDefault();
        //            if (_productExists != null)
        //            {
        //                var _businessProductInList = db.BusinessProductsList.Where(c => c.BusinessId == businessId && c.ProductId == productId && c.ListName == itemList).FirstOrDefault();
        //                if (_businessProductInList != null)
        //                {
        //                    _businessProductInList.Quantity = _businessProductInList.Quantity + quantity;
        //                    _businessProductInList.LastAddedDate = DateTime.Now;
        //                    var entry = db.Entry(_businessProductInList);
        //                    entry.Property(y => y.Quantity);
        //                    entry.Property(y => y.LastAddedDate);
        //                    db.SaveChanges();

        //                    _productToRet = _businessProductInList;
        //                }
        //                else
        //                {
        //                    var _newBusinessProductList = db.BusinessProductsList.Add(new BusinessProductsList
        //                    {
        //                        BusinessId = businessId,
        //                        ProductId = productId,
        //                        ListName = itemList,
        //                        Quantity = quantity,
        //                        LastAddedDate = DateTime.Now,
        //                    });
        //                    db.SaveChanges();
        //                    _productToRet = _newBusinessProductList;

        //                }
        //                //return AddToList(_productExists.Id, product.ItemList, 1);
        //            }

        //            var _toRet = new BusinessProductListDTO
        //            {
        //                Id = _productToRet.Id,
        //                BusinessId = _productToRet.BusinessId,
        //                Barcode = _productExists.Barcode,
        //                ProductId = _productExists.Id,
        //                ItemType = _productToRet.ListName,
        //                Name = _productExists.Name,
        //                Price = _productExists.Price ?? 0,
        //                LastAddedDate = _productToRet.LastAddedDate,
        //                Quantity = _productToRet.Quantity
        //            };
        //            return new JsonApiResponse
        //            {
        //                Success = true,
        //                Code = 1,
        //                Message = "Product added to list. ProductId " + _productExists.Id,
        //                Data = _toRet
        //            };
        //        }
        //        catch (Exception ex)
        //        {

        //            return new JsonApiResponse
        //            {
        //                Success = false,
        //                Code = -10,
        //                Message = "Error: " + ex.Message,
        //                Data = null
        //            };
        //        }

        //    }
        //}
        static public JsonApiResponse AddToList(int businessId, int productId, string itemList, int quantity, int businessListProductId = -1)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                try
                {
                    BusinessProductsList _productToRet = null;

                    //first check if there is product with barcode
                    var _productExists = db.BusinessProducts.Where(c => c.Id == productId).FirstOrDefault();
                    if (_productExists != null)
                    {
                        if (itemList == "shoppingList" || itemList == "inventory")
                        {
                            var _businessProductInList = db.BusinessProductsList.Where(c => c.BusinessId == businessId && c.ProductId == productId && c.ListName == itemList).FirstOrDefault();
                            if (_businessProductInList != null)
                            {
                                _businessProductInList.Quantity = _businessProductInList.Quantity + quantity;
                                _businessProductInList.LastAddedDate = DateTime.Now;
                                var entry = db.Entry(_businessProductInList);
                                entry.Property(y => y.Quantity);
                                entry.Property(y => y.LastAddedDate);
                                db.SaveChanges();

                                _productToRet = _businessProductInList;
                            }
                            else
                            {
                                var _newBusinessProductList = db.BusinessProductsList.Add(new BusinessProductsList
                                {
                                    BusinessId = businessId,
                                    ProductId = productId,
                                    ListName = itemList,
                                    Quantity = quantity,
                                    LastAddedDate = DateTime.Now,
                                });
                                db.SaveChanges();
                                _productToRet = _newBusinessProductList;
                            }

                            var _toRet = new BusinessProductListDTO
                            {
                                Id = _productToRet.Id,
                                BusinessId = _productToRet.BusinessId,
                                Barcode = _productExists.Barcode,
                                ProductId = _productExists.Id,
                                ItemType = _productToRet.ListName,
                                Name = _productExists.Name,
                                Price = _productExists.Price ?? 0,
                                LastAddedDate = _productToRet.LastAddedDate,
                                Quantity = _productToRet.Quantity,
                                Category = _productExists.Category
                            };

                            return new JsonApiResponse
                            {
                                Success = true,
                                Code = 1,
                                Message = "Sucess",
                                Data = _toRet
                            };

                        }
                        else //is consumed or bought
                        {
                            string _typeOfCheckout = itemList;
                            string _addInList = itemList;
                            string _removeFromList = itemList;
                            switch (_typeOfCheckout.ToLower())
                            {
                                case "consumed":
                                    _addInList = "shoppingList";
                                    _removeFromList = "inventory";
                                    break;
                                case "bought":
                                    _addInList = "inventory";
                                    _removeFromList = "shoppingList";
                                    break;
                                default:
                                    break;
                            }
                            var _businessProductInListAdd = db.BusinessProductsList.Where(c => c.ProductId == productId && c.BusinessId == businessId && c.ListName.ToLower() == _addInList).FirstOrDefault();
                            var _businessProductInListRemove = db.BusinessProductsList.Where(c => c.ProductId == productId && c.BusinessId == businessId && c.ListName.ToLower() == _removeFromList).FirstOrDefault();
                            var _businessProductInListRemoved = false;

                            if (_businessProductInListAdd != null)
                            {
                                _businessProductInListAdd.Quantity = _businessProductInListAdd.Quantity + quantity;
                                _businessProductInListAdd.LastAddedDate = DateTime.Now;
                                db.BusinessProductsList.Attach(_businessProductInListAdd);
                                var entry = db.Entry(_businessProductInListAdd);
                                //TO REMEMBER
                                entry.Property(y => y.Quantity).IsModified = true;
                                entry.Property(y => y.LastAddedDate).IsModified = true;
                            }
                            else
                            {
                                BusinessProductsList _newBusinessProductsList = new BusinessProductsList
                                {
                                    BusinessId = businessId,
                                    ListName = _addInList,
                                    LastAddedDate = DateTime.Now,
                                    ProductId = productId,
                                    Quantity = quantity,
                                };
                                db.BusinessProductsList.Add(_newBusinessProductsList);
                                _businessProductInListAdd = _newBusinessProductsList;
                            }

                            if (_businessProductInListRemove != null)
                            {
                                if (_businessProductInListRemove.Quantity - quantity < 1) //remove
                                {
                                    db.BusinessProductsList.Remove(_businessProductInListRemove);
                                    _businessProductInListRemoved = true;
                                }
                                else //update
                                {
                                    _businessProductInListRemove.Quantity = _businessProductInListRemove.Quantity - quantity;
                                    _businessProductInListRemove.LastAddedDate = DateTime.Now;
                                    db.BusinessProductsList.Attach(_businessProductInListRemove);
                                    var entry = db.Entry(_businessProductInListRemove);
                                    //TO REMEMBER
                                    entry.Property(y => y.Quantity).IsModified = true;
                                    entry.Property(y => y.LastAddedDate).IsModified = true;

                                }
                            }

                            db.SaveChanges();

                            AddToHistory(businessId, productId, _productExists.Name, _typeOfCheckout, _productExists.Price.Value, quantity, false);

                            //if (_businessProductInListRemoved)
                            //{
                            //    _businessProductInListRemove.Is = "removed";
                            //}

                            List<BusinessProductListDTO> _items = new List<BusinessProductListDTO>();

                            _items.Add(new BusinessProductListDTO
                            {
                                Id = _businessProductInListAdd.Id,
                                BusinessId = _businessProductInListAdd.BusinessId,
                                Barcode = _productExists.Barcode,
                                ProductId = _productExists.Id,
                                ItemType = _businessProductInListAdd.ListName,
                                Name = _productExists.Name,
                                Price = _productExists.Price ?? 0,
                                LastAddedDate = _businessProductInListAdd.LastAddedDate,
                                Quantity = _businessProductInListAdd.Quantity,
                            });

                            if (_businessProductInListRemove != null)
                            {
                                _items.Add(new BusinessProductListDTO
                                {
                                    Id = _businessProductInListRemove.Id,
                                    BusinessId = _businessProductInListRemove.BusinessId,
                                    Barcode = _productExists.Barcode,
                                    ProductId = _productExists.Id,
                                    ItemType = _businessProductInListRemove.ListName,
                                    Name = _productExists.Name,
                                    Price = _productExists.Price ?? 0,
                                    LastAddedDate = _businessProductInListRemove.LastAddedDate,
                                    Quantity = _businessProductInListRemove.Quantity,
                                    IsToRemove = _businessProductInListRemoved
                                });

                            }


                            return new JsonApiResponse
                            {
                                Success = true,
                                Code = 1,
                                Message = "Sucess",
                                Data = new
                                {
                                    Items = _items
                                }
                            };
                        }
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -1,
                            Message = "Product not found",
                            Data = null
                        };

                    }

                }
                catch (Exception ex)
                {

                    return new JsonApiResponse
                    {
                        Success = false,
                        Code = -10,
                        Message = "Error: " + ex.Message,
                        Data = null
                    };
                }

            }
        }

        static public JsonApiResponse AddToListHabitos(int businessId, int productId, string itemList, int quantity, int businessListProductId = -1)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                try
                {
                    BusinessProductsList _productToRet = null;

                    //first check if there is product with barcode
                    var _productExists = db.BusinessProducts.Where(c => c.Id == productId).FirstOrDefault();
                    if (_productExists != null)
                    {
                        if (itemList == "shoppingList" || itemList == "inventory")
                        {
                            var _businessProductInList = db.BusinessProductsList.Where(c => c.BusinessId == businessId && c.ProductId == productId && c.ListName == itemList).FirstOrDefault();
                            if (_businessProductInList != null)
                            {
                                _businessProductInList.Quantity = _businessProductInList.Quantity + quantity;
                                _businessProductInList.LastAddedDate = DateTime.Now;
                                var entry = db.Entry(_businessProductInList);
                                entry.Property(y => y.Quantity);
                                entry.Property(y => y.LastAddedDate);
                                db.SaveChanges();

                                _productToRet = _businessProductInList;
                            }
                            else
                            {
                                var _newBusinessProductList = db.BusinessProductsList.Add(new BusinessProductsList
                                {
                                    BusinessId = businessId,
                                    ProductId = productId,
                                    ListName = itemList,
                                    Quantity = quantity,
                                    LastAddedDate = DateTime.Now,
                                });
                                db.SaveChanges();
                                _productToRet = _newBusinessProductList;
                            }

                            var _toRet = new BusinessProductListDTO
                            {
                                Id = _productToRet.Id,
                                BusinessId = _productToRet.BusinessId,
                                Barcode = _productExists.Barcode,
                                ProductId = _productExists.Id,
                                ItemType = _productToRet.ListName,
                                Name = _productExists.Name,
                                Price = _productExists.Price ?? 0,
                                LastAddedDate = _productToRet.LastAddedDate,
                                Quantity = _productToRet.Quantity,
                                Category = _productExists.Category
                            };

                            return new JsonApiResponse
                            {
                                Success = true,
                                Code = 1,
                                Message = "Sucess",
                                Data = _toRet
                            };

                        }
                        else //is consumed or bought
                        {
                            string _typeOfCheckout = itemList;
                            string _addInList = itemList;
                            string _removeFromList = itemList;
                            switch (_typeOfCheckout.ToLower())
                            {
                                case "consumed":
                                    _addInList = "shoppingList";
                                    _removeFromList = "inventory";
                                    break;
                                case "bought":
                                    _addInList = "inventory";
                                    _removeFromList = "shoppingList";
                                    break;
                                default:
                                    break;
                            }
                            var _businessProductInListAdd = db.BusinessProductsList.Where(c => c.ProductId == productId && c.BusinessId == businessId && c.ListName.ToLower() == _addInList).FirstOrDefault();
                            var _businessProductInListRemove = db.BusinessProductsList.Where(c => c.ProductId == productId && c.BusinessId == businessId && c.ListName.ToLower() == _removeFromList).FirstOrDefault();
                            var _businessProductInListRemoved = false;

                            var _oldQuantity = 0;
                            var _newQuantity = 0;
                            if (_businessProductInListAdd != null)
                            {
                                _oldQuantity = _businessProductInListAdd.Quantity.Value;
                                bool _IsNegative = _businessProductInListAdd.Quantity < 0;
                                int _computedTotal = _IsNegative ? quantity : _businessProductInListAdd.Quantity.Value + quantity;
                                _newQuantity = _computedTotal;
                                _businessProductInListAdd.Quantity = _computedTotal;
                                _businessProductInListAdd.LastAddedDate = DateTime.Now;
                                db.BusinessProductsList.Attach(_businessProductInListAdd);
                                var entry = db.Entry(_businessProductInListAdd);
                                //TO REMEMBER
                                entry.Property(y => y.Quantity).IsModified = true;
                                entry.Property(y => y.LastAddedDate).IsModified = true;
                            }
                            else
                            {
                                BusinessProductsList _newBusinessProductsList = new BusinessProductsList
                                {
                                    BusinessId = businessId,
                                    ListName = _addInList,
                                    LastAddedDate = DateTime.Now,
                                    ProductId = productId,
                                    Quantity = quantity,
                                };
                                db.BusinessProductsList.Add(_newBusinessProductsList);
                                _businessProductInListAdd = _newBusinessProductsList;
                            }

                            if (_businessProductInListRemove != null)
                            {
                                //if (_businessProductInListRemove.Quantity - quantity < 1) //remove
                                //{
                                //    db.BusinessProductsList.Remove(_businessProductInListRemove);
                                //    _businessProductInListRemoved = true;
                                //}
                                //else //update
                                //{
                                _businessProductInListRemove.Quantity = _businessProductInListRemove.Quantity - quantity;
                                _businessProductInListRemove.LastAddedDate = DateTime.Now;
                                db.BusinessProductsList.Attach(_businessProductInListRemove);
                                var entry = db.Entry(_businessProductInListRemove);
                                //TO REMEMBER
                                entry.Property(y => y.Quantity).IsModified = true;
                                entry.Property(y => y.LastAddedDate).IsModified = true;

                                //}
                            }

                            db.SaveChanges();

                            AddToHistory(businessId, productId, _productExists.Name, _typeOfCheckout, _productExists.Price.Value, quantity, false);
                            AddStockChangeToHistory(businessId, productId, _productExists.Name, _typeOfCheckout, _productExists.Price.Value, _oldQuantity, _newQuantity);

                            //if (_businessProductInListRemoved)
                            //{
                            //    _businessProductInListRemove.Is = "removed";
                            //}

                            List<BusinessProductListDTO> _items = new List<BusinessProductListDTO>();

                            _items.Add(new BusinessProductListDTO
                            {
                                Id = _businessProductInListAdd.Id,
                                BusinessId = _businessProductInListAdd.BusinessId,
                                Barcode = _productExists.Barcode,
                                ProductId = _productExists.Id,
                                ItemType = _businessProductInListAdd.ListName,
                                Name = _productExists.Name,
                                Price = _productExists.Price ?? 0,
                                LastAddedDate = _businessProductInListAdd.LastAddedDate,
                                Quantity = _businessProductInListAdd.Quantity,
                            });

                            if (_businessProductInListRemove != null)
                            {
                                _items.Add(new BusinessProductListDTO
                                {
                                    Id = _businessProductInListRemove.Id,
                                    BusinessId = _businessProductInListRemove.BusinessId,
                                    Barcode = _productExists.Barcode,
                                    ProductId = _productExists.Id,
                                    ItemType = _businessProductInListRemove.ListName,
                                    Name = _productExists.Name,
                                    Price = _productExists.Price ?? 0,
                                    LastAddedDate = _businessProductInListRemove.LastAddedDate,
                                    Quantity = _businessProductInListRemove.Quantity,
                                    IsToRemove = _businessProductInListRemoved
                                });

                            }


                            return new JsonApiResponse
                            {
                                Success = true,
                                Code = 1,
                                Message = "Sucess",
                                Data = new
                                {
                                    Items = _items
                                }
                            };
                        }
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -1,
                            Message = "Product not found",
                            Data = null
                        };

                    }

                }
                catch (Exception ex)
                {

                    return new JsonApiResponse
                    {
                        Success = false,
                        Code = -10,
                        Message = "Error: " + ex.Message,
                        Data = null
                    };
                }

            }
        }

        static public BusinessProductListDTO GetBusinessProductListDTOById(int businessProductListId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {

                var _temp =
                   from businessProduct in db.BusinessProductsList
                   join _product in db.BusinessProducts on businessProduct.ProductId equals _product.Id
                   where businessProduct.Id == businessProductListId
                   //orderby storePrd.Price
                   select new BusinessProductListDTO
                   {
                       Id = businessProduct.Id,
                       BusinessId = businessProduct.BusinessId,
                       ProductId = businessProduct.ProductId,
                       Quantity = businessProduct.Quantity.Value,
                       Barcode = _product.Barcode,
                       //Brand = product.Brand,
                       ItemType = businessProduct.ListName,
                       Name = _product.Name,
                       Price = _product.Price ?? 0,
                       LastAddedDate = businessProduct.LastAddedDate
                       //Category = product.CategoryString,
                       //CreatedByUserId = storePrd.UserId,
                       //Weight = product.Weight,
                   };
                return _temp.FirstOrDefault();
            }
        }

        static public BusinessProductListDTO GetBusinessProductDTOById(int businessProducId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {

                var _temp =
                   from _product in db.BusinessProducts
                   where _product.Id == businessProducId
                   //orderby storePrd.Price
                   select new BusinessProductListDTO
                   {
                       Id = -1,
                       BusinessId = _product.BusinessId,
                       ProductId = _product.Id,
                       Quantity = -1,
                       Barcode = _product.Barcode,
                       //Brand = product.Brand,
                       ItemType = "",
                       Name = _product.Name,
                       Price = _product.Price ?? 0,
                       LastAddedDate = DateTime.MinValue
                       //Category = product.CategoryString,
                       //CreatedByUserId = storePrd.UserId,
                       //Weight = product.Weight,
                   };
                return _temp.FirstOrDefault();
            }
        }

        static public JsonApiResponse Delete(int businessListProductId, int businessId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                try
                {
                    var _product = db.BusinessProductsList.Where(c => c.Id == businessListProductId && c.BusinessId == businessId).FirstOrDefault();
                    if (_product != null)
                        db.BusinessProductsList.Remove(_product);
                    else
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -2,
                            Message = "Product of business list not found. businessProductListId: " + businessListProductId,
                            Data = null
                        };
                    }

                    db.SaveChanges();

                    return new JsonApiResponse
                    {
                        Success = true,
                        Code = 1,
                        Message = "Product of business list deleted. businessProductListId: " + businessListProductId,
                        Data = null
                    };


                }
                catch (Exception ex)
                {
                    Logger.Debug("error deleting user inventory product: " + ex.Message);
                    return new JsonApiResponse
                    {
                        Success = false,
                        Code = -1,
                        Message = "Error: " + ex.Message,
                        Data = null
                    };
                }
            }


        }

        static public JsonApiResponse SubtractQuantity(int businessListProductId, int businessId, int quantity = 1)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                try
                {
                    var _product = db.BusinessProductsList.Where(c => c.Id == businessListProductId && c.BusinessId == businessId).FirstOrDefault();
                    if (_product != null)
                    {
                        //if (_product.Quantity - quantity < 1)
                        //{
                        //    db.BusinessProductsList.Remove(_product);
                        //    db.SaveChanges();
                        //    return new JsonApiResponse
                        //    {
                        //        Success = true,
                        //        Code = 1,
                        //        Message = "Product of business list deleted because quantity was at 1. businessProductListId: " + businessListProductId,
                        //        Data = null
                        //    };
                        //}
                        //else
                        //{
                        var _oldQuantity = _product.Quantity;
                        var _newQuantity = _product.Quantity - 1;
                        _product.Quantity -= quantity;
                        db.SaveChanges();

                        var _productElement = db.BusinessProducts.Where(c => c.Id == _product.ProductId).FirstOrDefault();
                        AddStockChangeToHistory(businessId, _product.Id, _productElement.Name, _product.ListName, _productElement.Price.Value, _oldQuantity.Value, _newQuantity.Value);

                        return new JsonApiResponse
                        {
                            Success = true,
                            Code = 2,
                            Message = "Quantity subtracted of Product of business list. businessProductListId: " + businessListProductId,
                            Data = GetBusinessProductListDTOById(businessListProductId)
                        };
                        //}
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -2,
                            Message = "Business product of list not found.  " + businessListProductId,
                            Data = null
                        };
                    }

                }
                catch (Exception ex)
                {
                    Logger.Debug("error: " + ex.Message);
                    return new JsonApiResponse
                    {
                        Success = false,
                        Code = -1,
                        Message = "Error: " + ex.Message,
                        Data = null
                    };

                }
            }
        }

        static public JsonApiResponse AddQuantity(int businessListProductId, int businessId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {

                    var _product = db.BusinessProductsList.Where(c => c.Id == businessListProductId && c.BusinessId == businessId).FirstOrDefault();
                    if (_product != null)
                    {
                        var _oldQuantity = _product.Quantity;
                        var _newQuantity = _product.Quantity + 1;
                        _product.Quantity++;
                        db.SaveChanges();

                        var _productElement = db.BusinessProducts.Where(c => c.Id == _product.ProductId).FirstOrDefault();
                        AddStockChangeToHistory(businessId, _product.Id, _productElement.Name, _product.ListName, _productElement.Price.Value, _oldQuantity.Value, _newQuantity.Value);
                        return new JsonApiResponse
                        {
                            Success = true,
                            Code = 1,
                            Message = "Product of business list quantity added. businessProductListId: " + businessListProductId,
                            Data = GetBusinessProductListDTOById(businessListProductId)
                        };
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -2,
                            Message = "Business Product of list not found",
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

        static public JsonApiResponse ChangeQuantity(int businessListProductId, int businessId, int newQuantity)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {

                    var _product = db.BusinessProductsList.Where(c => c.Id == businessListProductId && c.BusinessId == businessId).FirstOrDefault();
                    if (_product != null)
                    {
                        var _oldQuantity = _product.Quantity.Value;
                        _product.Quantity = newQuantity;
                        db.SaveChanges();

                        var _productElement = db.BusinessProducts.Where(c => c.Id == _product.ProductId).FirstOrDefault();
                        AddStockChangeToHistory(businessId, _product.Id, _productElement.Name, "manual", _productElement.Price.Value, _oldQuantity, newQuantity);
                        return new JsonApiResponse
                        {
                            Success = true,
                            Code = 1,
                            Message = "Product of business list quantity changed. businessProductListId: " + businessListProductId,
                            Data = GetBusinessProductListDTOById(businessListProductId)
                        };
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -2,
                            Message = "Business Product of list not found",
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

        static public JsonApiResponse CheckoutProduct(int businessListProductId, int businessId, bool emulate = false)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    BusinessProductsList _BusinessProductsListToRet = null;
                    string _typeOfCheckout = "consumed";
                    var _businessProductInList = db.BusinessProductsList.Where(c => c.Id == businessListProductId && c.BusinessId == businessId).FirstOrDefault();
                    if (_businessProductInList != null)
                    {
                        //remove product to checkout
                        db.BusinessProductsList.Remove(_businessProductInList);

                        string _addOrUpdateInList = string.Empty;
                        switch (_businessProductInList.ListName.ToLower())
                        {
                            case "shoppinglist":
                                _addOrUpdateInList = "inventory";
                                _typeOfCheckout = "bought";
                                break;
                            case "inventory":
                                _addOrUpdateInList = "shoppingList";
                                _typeOfCheckout = "consumed";
                                break;
                            default:
                                break;
                        }

                        var _businessProductInOtherList = db.BusinessProductsList.Where(c => c.ProductId == _businessProductInList.ProductId && c.BusinessId == businessId && c.ListName == _addOrUpdateInList).FirstOrDefault();
                        if (_businessProductInOtherList != null) //update
                        {
                            _businessProductInOtherList.Quantity = _businessProductInOtherList.Quantity + _businessProductInList.Quantity;
                            _businessProductInOtherList.LastAddedDate = DateTime.Now;
                            db.BusinessProductsList.Attach(_businessProductInOtherList);
                            var entry = db.Entry(_businessProductInOtherList);
                            //TO REMEMBER
                            entry.Property(y => y.Quantity).IsModified = true;
                            entry.Property(y => y.LastAddedDate).IsModified = true;
                            _BusinessProductsListToRet = _businessProductInOtherList;
                        }
                        else //add new
                        {
                            BusinessProductsList _newBusinessProductsList = new BusinessProductsList
                            {
                                BusinessId = businessId,
                                ListName = _addOrUpdateInList,
                                LastAddedDate = DateTime.Now,
                                ProductId = _businessProductInList.ProductId,
                                Quantity = _businessProductInList.Quantity,
                            };
                            _BusinessProductsListToRet = _newBusinessProductsList;
                            db.BusinessProductsList.Add(_newBusinessProductsList);
                        }

                        if (!emulate)
                        {
                            db.SaveChanges();
                            var _product = db.BusinessProducts.Where(c => c.Id == _businessProductInList.ProductId).FirstOrDefault();
                            if (_product != null)
                            {
                                AddToHistory(businessId, _businessProductInList.ProductId, _product.Name, _typeOfCheckout, _product.Price ?? 0, _businessProductInList.Quantity.Value, false);
                            }
                        }


                        //arrange right return type for client side parsing
                        BusinessProductListDTO _toRet = null;
                        var _productData = db.BusinessProducts.Where(c => c.Id == _BusinessProductsListToRet.ProductId).FirstOrDefault();
                        if (_BusinessProductsListToRet.Id != 0)
                        {
                            _toRet = new BusinessProductListDTO()
                            {
                                Id = _BusinessProductsListToRet.Id,
                                ProductId = _BusinessProductsListToRet.ProductId,
                                BusinessId = _BusinessProductsListToRet.BusinessId,
                                Barcode = _productData.Barcode,
                                Name = _productData?.Name,
                                Price = _productData.Price ?? 0,
                                ItemType = _BusinessProductsListToRet.ListName,
                                LastAddedDate = _BusinessProductsListToRet.LastAddedDate,
                                Quantity = _BusinessProductsListToRet.Quantity

                            };
                        }
                        else //is emulated because Id is 0 because it was not saved
                        {
                            _toRet = new BusinessProductListDTO()
                            {
                                Id = businessListProductId * -1,
                                ProductId = _BusinessProductsListToRet.ProductId,
                                BusinessId = _BusinessProductsListToRet.BusinessId,
                                Barcode = _productData.Barcode,
                                Name = _productData?.Name,
                                Price = _productData.Price ?? 0,
                                ItemType = _BusinessProductsListToRet.ListName,
                                LastAddedDate = _BusinessProductsListToRet.LastAddedDate,
                                Quantity = _BusinessProductsListToRet.Quantity

                            };
                        }

                        return new JsonApiResponse
                        {
                            Success = true,
                            Code = 1,
                            Message = "Product quantity added. emulated: " + emulate.ToString(),
                            Data = _toRet
                        };
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -2,
                            Message = "Business Product of list not found",
                            Data = null
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("error checking out products: " + ex.Message);
                return new JsonApiResponse
                {
                    Success = false,
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                };

            }
        }

        static public JsonApiResponse AddProductByBarcode(string barcode, int businessId, string list, int quantity)
        {
            try
            {
                using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
                {
                    var _productExists = GetByBarcodeV2(barcode, businessId);
                    if (_productExists != null)
                    {
                        return AddToList(businessId, _productExists.Id, list, quantity);
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -1,
                            Message = "Product with barcode not found. Barcode " + barcode
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonApiResponse
                {
                    Success = false,
                    Code = -2,
                    Message = ex.Message
                };
            }
        }

        static public JsonApiResponse AddProductByBarcodeHabitos(string barcode, int businessId, string list, int quantity)
        {
            try
            {
                using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
                {
                    var _productExists = GetByBarcodeV2(barcode, businessId);
                    if (_productExists != null)
                    {
                        return AddToListHabitos(businessId, _productExists.Id, list, quantity);
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -1,
                            Message = "Product with barcode not found. Barcode " + barcode
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonApiResponse
                {
                    Success = false,
                    Code = -2,
                    Message = ex.Message
                };
            }
        }


        static public int LisieHomeAddProduct(int businessId, string barcode)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _product = GetByBarcodeV2(barcode, businessId);
                    if (_product != null)
                    {
                        int _toRet = 0;
                        BusinessProductListDTO _productToRet = null;
                        string _mode = LisieHomeGetMode(businessId);
                        JsonApiResponse _result = null;
                        switch (_mode)
                        {
                            case "consumed":
                                _result = AddToList(businessId, _product.Id, "shoppingList", 1);
                                if (_result.Success)
                                {
                                    _productToRet = _result.Data as BusinessProductListDTO;
                                    BusinessProductsList _existInInventory = db.BusinessProductsList.Where(c => c.BusinessId == businessId && c.ProductId == _product.Id && c.ListName.ToLower() == "inventory").FirstOrDefault();
                                    if (_existInInventory != null)
                                    {
                                        var __result = SubtractQuantity(_existInInventory.Id, businessId);
                                        if (__result.Success)
                                        {
                                            _toRet = -2;
                                        }
                                        else
                                        {
                                            _toRet = -9;

                                        }
                                    }
                                    _toRet = -4;
                                }
                                else
                                {
                                    _toRet = -9;
                                }
                                break;
                            case "shoppingList":
                                _result = AddToList(businessId, _product.Id, "shoppingList", 1);

                                if (_result.Success)
                                {
                                    _productToRet = _result.Data as BusinessProductListDTO;
                                    _toRet = -5;
                                }
                                else
                                {
                                    _toRet = -9;
                                }
                                break;
                            case "bought":
                                _result = AddToList(businessId, _product.Id, "inventory", 1);
                                if (_result.Success)
                                {
                                    _productToRet = _result.Data as BusinessProductListDTO;

                                    BusinessProductsList _existInShoppingList = db.BusinessProductsList
                                        .Where(c => c.BusinessId == businessId &&
                                        c.ProductId == _product.Id &&
                                        c.ListName.ToLower() == "shoppingList").FirstOrDefault();
                                    if (_existInShoppingList != null)
                                    {
                                        var __result = SubtractQuantity(_existInShoppingList.Id, businessId);
                                        if (__result.Success)
                                        {
                                            _toRet = -3;
                                        }
                                        else
                                        {
                                            _toRet = -9;
                                        }
                                    }
                                    _toRet = -6;
                                }
                                else
                                {
                                    _toRet = -9;
                                }
                                break;
                            case "inventory":
                                _result = AddToList(businessId, _product.Id, "inventory", 1);
                                if (_result.Success)
                                {
                                    _productToRet = _result.Data as BusinessProductListDTO;

                                    _toRet = -6;
                                }
                                else
                                {
                                    _toRet = -9;
                                }
                                break;
                            default:
                                _toRet = -9;
                                break;
                        }
                        AddToHistory(businessId, _product.Id, _product.Name, _mode, _product.Price.HasValue ? _product.Price.Value : 0, 1, true);
                        string[] _businessDeviceTokens = db.BusinessDevices.Where(d => d.BusinessId == businessId && !string.IsNullOrEmpty(d.DeviceToken)).Select(d => d.DeviceToken).ToArray();
                        if (_businessDeviceTokens.Length > 0)
                        {
                            dynamic _data = new
                            {
                                action = "lisieHomeProductAdded",
                                argument = _productToRet
                            };
                            ExpoNotifications.Send(new ExpoNotificationModel
                            {
                                to = _businessDeviceTokens,
                                title = "Produto adicionado",
                                body = _product.Name + " adicionado.",
                                data = new JavaScriptSerializer().Serialize(_data)
                            });
                        }
                        return _toRet;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("error checking out products: " + ex.Message);
                return -10;

            }
        }

        static public JsonApiResponse LisieHomeAddProductV2(int businessId, string barcode)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _product = GetByBarcodeV2(barcode, businessId);
                    if (_product != null)
                    {
                        var _productExistsInQueue = db.BusinessProductsOnQueue.FirstOrDefault();
                        //string[] _businessDeviceTokens = db.BusinessDevices.Where(d => d.BusinessId == businessId && !string.IsNullOrEmpty(d.DeviceToken) && d.ModelName.Contains("#TABLET")).Select(d => d.DeviceToken).ToArray();
                        string[] _businessDeviceTokens = db.BusinessDevices.Where(d => d.BusinessId == businessId && !string.IsNullOrEmpty(d.DeviceToken)).Select(d => d.DeviceToken).ToArray();
                        BusinessProductListDTO _productToRet = null;
                        if (_productExistsInQueue != null)
                        {
                            var _result = _LisieHomeAddProductV2(businessId, _productExistsInQueue.ProductId, _productExistsInQueue.Quantity.HasValue ? _productExistsInQueue.Quantity.Value : 1);
                            _productToRet = _result.Data as BusinessProductListDTO;
                            db.BusinessProductsOnQueue.Remove(_productExistsInQueue);
                            db.SaveChanges();
                        }
                        //else
                        //{
                        string _mode = LisieHomeGetMode(businessId);
                        var _new = new BusinessProductsOnQueue
                        {
                            BusinessId = businessId,
                            CreateDate = DateTime.Now,
                            ProductId = _product.Id,
                            Quantity = 1,
                            ListName = _mode
                        };
                        db.BusinessProductsOnQueue.Add(_new);
                        db.SaveChanges();

                        _productToRet = GetBusinessProductDTOById(_product.Id);
                        //}

                        if (_businessDeviceTokens.Length > 0)
                        {

                            dynamic _data = new
                            {
                                action = "lisieHomeRequestProductAdd",
                                argument = _productToRet
                            };
                            ExpoNotifications.Send(new ExpoNotificationModel
                            {
                                to = _businessDeviceTokens,
                                title = "Qual a quantidade?",
                                body = _product.Name,
                                data = new JavaScriptSerializer().Serialize(_data)
                            });
                        }
                        return new JsonApiResponse
                        {
                            Code = 1,
                            Message = "",
                            Success = true,
                            Data = _productToRet
                        };
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Code = -1,
                            Message = "product not found",
                            Success = false
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("error checking out products: " + ex.Message);
                return new JsonApiResponse
                {
                    Code = -10,
                    Message = ex.Message,
                    Success = false
                };
            }
        }

        static public JsonApiResponse _LisieHomeAddProductV2(int businessId, int productId, int quantity = 1)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _product = db.BusinessProducts.Where(c => c.Id == productId).FirstOrDefault();
                    if (_product != null)
                    {
                        int _toRet = 0;
                        BusinessProductListDTO _productToRet = null;
                        string _mode = LisieHomeGetMode(businessId);
                        JsonApiResponse _result = null;
                        switch (_mode)
                        {
                            case "consumed":
                                _result = AddToList(businessId, _product.Id, "shoppingList", quantity);
                                if (_result.Success)
                                {
                                    _productToRet = _result.Data as BusinessProductListDTO;
                                    BusinessProductsList _existInInventory = db.BusinessProductsList.Where(c => c.BusinessId == businessId && c.ProductId == _product.Id && c.ListName.ToLower() == "inventory").FirstOrDefault();
                                    if (_existInInventory != null)
                                    {
                                        var __result = SubtractQuantity(_existInInventory.Id, businessId, quantity);
                                        _result = __result;
                                    }
                                }
                                break;
                            case "shoppingList":
                                _result = AddToList(businessId, _product.Id, "shoppingList", quantity);
                                if (_result.Success)
                                {
                                    _productToRet = _result.Data as BusinessProductListDTO;
                                }
                                break;
                            case "bought":
                                _result = AddToList(businessId, _product.Id, "inventory", quantity);
                                if (_result.Success)
                                {
                                    _productToRet = _result.Data as BusinessProductListDTO;
                                    BusinessProductsList _existInShoppingList = db.BusinessProductsList
                                        .Where(c => c.BusinessId == businessId &&
                                        c.ProductId == _product.Id &&
                                        c.ListName.ToLower() == "shoppingList").FirstOrDefault();
                                    if (_existInShoppingList != null)
                                    {
                                        var __result = SubtractQuantity(_existInShoppingList.Id, businessId, quantity);
                                        if (__result.Success)
                                        {
                                            _result = __result;
                                        }
                                    }
                                }
                                break;
                            case "inventory":
                                _result = AddToList(businessId, _product.Id, "inventory", quantity);
                                if (_result.Success)
                                {
                                    _productToRet = _result.Data as BusinessProductListDTO;
                                }
                                break;
                            default:
                                _toRet = -9;
                                break;
                        }

                        var _newHistoryItem = AddToHistory(businessId, _product.Id, _product.Name, _mode, _product.Price.HasValue ? _product.Price.Value : 0, quantity, true);
                        string[] _businessDeviceTokens = db.BusinessDevices.Where(d => d.BusinessId == businessId && !string.IsNullOrEmpty(d.DeviceToken) && d.ModelName.Contains("#TABLET")).Select(d => d.DeviceToken).ToArray();
                        if (_businessDeviceTokens.Length > 0)
                        {
                            dynamic _data = new
                            {
                                action = "lisieHomeProductAdded",
                                argument = _productToRet
                            };
                            ExpoNotifications.Send(new ExpoNotificationModel
                            {
                                to = _businessDeviceTokens,
                                title = "Produto adicionado",
                                body = _product.Name + " adicionado. com quantidade " + quantity,
                                data = new JavaScriptSerializer().Serialize(_data)
                            });

                            //dynamic _daatHistory = new
                            //{
                            //    action = "productAddedToHistory",
                            //    argument = "",
                            //};

                            //SendHistoryItemNotification(businessId, _mode, _product.Id, quantity);
                            //if (_newHistoryItem != null)
                            //{
                            //    var _totalProductQuantity = db.BusinessProductsListHistory.Where(c => c.ProductId == _product.Id).Sum(c => c.Quantity);
                            //    ar _BusinessProductHistoryDTO = new BusinessProductHistoryDTO
                            //    {
                            //        Id = item.Id,
                            //        BusinessId = businessId,
                            //        Quantity = _totalProductQuantity.Value,
                            //        ProductId = item.ProductId.Value,
                            //        ItemType = item.ListName,
                            //        Name = item.ProductName,
                            //        Price = item.Price.Value,
                            //        Items = _history.Where(c => c.ProductId == item.ProductId).ToList()
                            //    });

                            //    dynamic __data = new
                            //    {
                            //        action = "productAddedToHistory",
                            //        argument = _BusinessProductHistoryDTO
                            //    };

                            //    SendToBusinessDevice(businessId, "Entrada de histórico adicionado", quantity + " de " + _product.Name,
                            //        new JavaScriptSerializer().Serialize(__data));
                            //};
                        }
                        return _result;
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Code = -1,
                            Success = false,
                            Message = "No product found"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("error checking out products: " + ex.Message);
                return new JsonApiResponse
                {
                    Code = -10,
                    Success = false,
                    Message = ex.Message
                };

            }
        }

        static public JsonApiResponse AddLisieHomeProductWithQueue(int businessId, int productId, int quantity)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _product = GetBusinessProductDTOById(productId);
                    if (_product != null)
                    {
                        var _productExistsInQueue = db.BusinessProductsOnQueue.Where(c => c.BusinessId == businessId && c.ProductId == productId).FirstOrDefault();
                        string[] _businessDeviceTokens = db.BusinessDevices.Where(d => d.BusinessId == businessId && !string.IsNullOrEmpty(d.DeviceToken)).Select(d => d.DeviceToken).ToArray();
                        BusinessProductListDTO _productToRet = null;
                        if (_productExistsInQueue != null)
                        {
                            var _result = _LisieHomeAddProductV2(businessId, _productExistsInQueue.ProductId, quantity);
                            _productToRet = _result.Data as BusinessProductListDTO;
                            db.BusinessProductsOnQueue.Remove(_productExistsInQueue);
                            db.SaveChanges();

                            return new JsonApiResponse
                            {
                                Code = 1,
                                Message = "",
                                Success = true,
                                Data = _result.Data
                            };
                        }
                        else
                        {

                            return new JsonApiResponse
                            {
                                Code = -2,
                                Message = "not found in queue",
                                Success = false
                            };
                        }
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Code = -1,
                            Message = "product not found",
                            Success = false
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("error checking out products: " + ex.Message);
                return new JsonApiResponse
                {
                    Code = -10,
                    Message = ex.Message,
                    Success = false
                };
            }
        }

        static public string LisieHomeGetMode(int businessId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _businessLisieHome = db.BusinessLisieHome.Where(c => c.BusinessId == businessId).FirstOrDefault();
                    if (_businessLisieHome != null)
                    {
                        return _businessLisieHome.Mode;
                    }
                    else
                    {
                        var _businessExists = db.Businesses.Where(c => c.Id.Equals(businessId)).FirstOrDefault();
                        if (_businessExists != null)
                        {
                            db.BusinessLisieHome.Add(new BusinessLisieHome
                            {
                                BusinessId = businessId,
                                CreateDate = DateTime.Now,
                                Mode = "consumed"
                            });
                            db.SaveChanges();
                        }
                    }
                    //default
                    return "consumed";
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return "consumed";
            }
        }
        static public JsonApiResponse LisieHomeSetMode(int businessId, string mode)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _businessLisieHome = db.BusinessLisieHome.Where(c => c.BusinessId == businessId).FirstOrDefault();
                    if (_businessLisieHome != null)
                    {
                        _businessLisieHome.Mode = mode;
                        _businessLisieHome.UpdateDate = DateTime.Now;
                        db.SaveChanges();
                        return new JsonApiResponse
                        {
                            Success = true,
                            Code = 1,
                            Message = "Entry updated"
                        };
                    }
                    else
                    {
                        var _businessExists = db.Businesses.Where(c => c.Id.Equals(businessId)).FirstOrDefault();
                        if (_businessExists != null)
                        {
                            db.BusinessLisieHome.Add(new BusinessLisieHome
                            {
                                BusinessId = businessId,
                                CreateDate = DateTime.Now,
                                Mode = "consumed"
                            });
                            db.SaveChanges();
                            return new JsonApiResponse
                            {
                                Success = true,
                                Code = 2,
                                Message = "Create new entry"
                            };
                        }
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -1,
                            Message = "Tried to create new entry, but business does not exist"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);

                return new JsonApiResponse
                {
                    Success = false,
                    Code = -9,
                    Message = "Error: " + ex.Message
                };
            }
        }

        static public JsonApiResponse GetLisieHomeState(int businessId)
        {

            string _mode = LisieHomeGetMode(businessId);
            return new JsonApiResponse
            {
                Success = true,
                Code = 1,
                Message = "Success",
                Data = _mode
            };
        }


        static public BusinessProductsListHistory AddToHistory(int businessId, int productId, string productName, string listName, double price, int quantity, bool isLisieHome = false)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _newHistoryItem = new BusinessProductsListHistory
                    {
                        BusinessId = businessId,
                        ProductName = productName,
                        ListName = listName,
                        Price = price,
                        Quantity = quantity,
                        ProductId = productId,
                        LisieHome = isLisieHome,
                        InsertDate = DateTime.Now,
                    };
                    var _added = db.BusinessProductsListHistory.Add(_newHistoryItem);
                    db.SaveChanges();
                    return _newHistoryItem;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return null;
            }
        }

        static public JsonApiResponse GetHistoryItem(int businessId, string list, string startDate, string endDate)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    DateTime _startDate = DateTime.MinValue;
                    DateTime _endDate = DateTime.MinValue;

                    if (!DateTime.TryParse(startDate, out _startDate) || !DateTime.TryParse(endDate, out _endDate))
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -1,
                            Message = "Dates not in correct format"
                        };
                    }

                    var _newEndDate = new DateTime(_endDate.Year, _endDate.Month, _endDate.Day, 23, 59, 59);

                    var _history = from historyProduct in db.BusinessProductsListHistory
                                       //join product in db.BusinessProducts on historyProduct.ProductId equals product.Id
                                   where historyProduct.BusinessId == businessId && historyProduct.ListName == list
                                   && historyProduct.InsertDate >= _startDate && historyProduct.InsertDate <= _newEndDate
                                   orderby historyProduct.InsertDate descending
                                   select historyProduct;
                    var _distinctProductIds = _history.DistinctBy(c => c.ProductId).ToList();
                    var _newHistory = new List<BusinessProductHistoryDTO>();

                    foreach (var item in _distinctProductIds)
                    {

                        var _totalProductQuantity = _history.Where(c => c.ProductId == item.ProductId).Sum(c => c.Quantity);
                        _newHistory.Add(new BusinessProductHistoryDTO
                        {
                            Id = item.Id,
                            BusinessId = businessId,
                            Quantity = _totalProductQuantity.Value,
                            ProductId = item.ProductId.Value,
                            ItemType = item.ListName,
                            Name = item.ProductName,
                            Price = item.Price.Value,
                            Items = _history.Where(c => c.ProductId == item.ProductId).ToList()
                        });
                    }
                    return new JsonApiResponse
                    {
                        Success = true,
                        Code = 1,
                        Message = "Sucess",
                        Data = _newHistory
                    };
                    //}


                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);

                return new JsonApiResponse
                {
                    Success = false,
                    Code = -10,
                    Message = "Error: " + ex.Message
                };
            }
        }
        static public JsonApiResponse GetHistory(int businessId, string list, string startDate, string endDate, string category = "")
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    DateTime _startDate = DateTime.MinValue;
                    DateTime _endDate = DateTime.MinValue;

                    if (!DateTime.TryParse(startDate, out _startDate) || !DateTime.TryParse(endDate, out _endDate))
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -1,
                            Message = "Dates not in correct format"
                        };
                    }

                    var _newEndDate = new DateTime(_endDate.Year, _endDate.Month, _endDate.Day, 23, 59, 59);

                    IOrderedQueryable<BusinessProductsListHistory> _history = null;
                    if (string.IsNullOrEmpty(category) || category.ToLower() == "todas")
                    {

                        _history = from historyProduct in db.BusinessProductsListHistory
                                       //join product in db.BusinessProducts on historyProduct.ProductId equals product.Id
                                   where historyProduct.BusinessId == businessId && historyProduct.ListName == list
                                   && historyProduct.InsertDate >= _startDate && historyProduct.InsertDate <= _newEndDate
                                   orderby historyProduct.InsertDate descending
                                   select historyProduct;
                    }
                    else
                    {
                        _history = (IOrderedQueryable<BusinessProductsListHistory>)(from historyProduct in db.BusinessProductsListHistory
                                                                                    join product in db.BusinessProducts on historyProduct.ProductId equals product.Id
                                                                                    where historyProduct.BusinessId == businessId && historyProduct.ListName == list
                                                                                    && historyProduct.InsertDate >= _startDate && historyProduct.InsertDate <= _newEndDate
                                                                                    && product.Category.ToLower() == category.ToLower()
                                                                                    orderby historyProduct.InsertDate descending
                                                                                    select historyProduct);
                    }

                    var _distinctProductIds = _history.DistinctBy(c => c.ProductId).ToList();
                    var _newHistory = new List<BusinessProductHistoryDTO>();

                    foreach (var item in _distinctProductIds)
                    {

                        var _totalProductQuantity = _history.Where(c => c.ProductId == item.ProductId).Sum(c => c.Quantity);
                        _newHistory.Add(new BusinessProductHistoryDTO
                        {
                            Id = item.Id,
                            BusinessId = businessId,
                            Quantity = _totalProductQuantity.Value,
                            ProductId = item.ProductId.Value,
                            ItemType = item.ListName,
                            Name = item.ProductName,
                            Price = item.Price.Value,
                            Items = _history.Where(c => c.ProductId == item.ProductId).ToList()
                        });
                    }
                    return new JsonApiResponse
                    {
                        Success = true,
                        Code = 1,
                        Message = "Sucess",
                        Data = _newHistory
                    };
                    //}


                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);

                return new JsonApiResponse
                {
                    Success = false,
                    Code = -10,
                    Message = "Error: " + ex.Message
                };
            }
        }

        static public JsonApiResponse DeleteProduct(int businessId, int productId)
        {
            try
            {
                var _data = _DeleteProduct(businessId, productId);
                return new JsonApiResponse
                {
                    Code = _data.Item2,
                    Success = _data.Item1,
                    Data = null,
                    Message = string.Empty
                };
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return new JsonApiResponse
                {
                    Success = false,
                    Code = -10,
                    Message = "Error: " + ex.Message
                };
            }
        }

        static public Tuple<bool, int> _DeleteProduct(int businessId, int productId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _product = db.BusinessProducts.Where(c => c.Id == productId && c.BusinessId == businessId).FirstOrDefault();
                if (_product != null)
                {
                    var _productsInLists = db.BusinessProductsList.Where(c => c.BusinessId == businessId && c.ProductId == productId);
                    //var _productsInHistory = db.BusinessProductsListHistory.Where(c=>c.BusinessId == businessId && c.ProductId == productId);
                    db.BusinessProductsList.RemoveRange(_productsInLists);
                    //db.BusinessProductsListHistory.RemoveRange(_productsInHistory);
                    db.BusinessProducts.Remove(_product);
                    db.SaveChanges();
                    return new Tuple<bool, int>(true, 1);

                }
                else
                {
                    return new Tuple<bool, int>(false, -1);
                }

            }
        }

        static public JsonApiResponse DeleteHistoryItem(int id, int businessId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _item = db.BusinessProductsListHistory.Where(c => c.BusinessId == businessId && c.Id == id).FirstOrDefault();
                    if (_item != null)
                    {
                        string _addOrUpdateInList = string.Empty;
                        string _removeOrUpdateInList = string.Empty;
                        switch (_item.ListName.ToLower())
                        {
                            case "consumed":
                                _addOrUpdateInList = "inventory";
                                _removeOrUpdateInList = "shoppingList";
                                break;
                            case "bought":
                                _addOrUpdateInList = "shoppingList";
                                _removeOrUpdateInList = "inventory";
                                break;
                            default:
                                break;
                        }

                        var _businessProductInOtherList = db.BusinessProductsList.Where(c => c.ProductId == _item.ProductId && c.BusinessId == businessId && c.ListName == _addOrUpdateInList).FirstOrDefault();

                        //Add to opossite list of HistoryItem
                        if (_businessProductInOtherList != null) //update
                        {
                            _businessProductInOtherList.Quantity = _businessProductInOtherList.Quantity + _item.Quantity;
                            _businessProductInOtherList.LastAddedDate = DateTime.Now;
                            db.BusinessProductsList.Attach(_businessProductInOtherList);
                            var entry = db.Entry(_businessProductInOtherList);
                            //TO REMEMBER
                            entry.Property(y => y.Quantity).IsModified = true;
                            entry.Property(y => y.LastAddedDate).IsModified = true;
                        }
                        else //add new
                        {
                            BusinessProductsList _newBusinessProductsList = new BusinessProductsList
                            {
                                BusinessId = businessId,
                                ListName = _addOrUpdateInList,
                                LastAddedDate = DateTime.Now,
                                ProductId = _item.ProductId.Value,
                                Quantity = _item.Quantity,
                            };
                            db.BusinessProductsList.Add(_newBusinessProductsList);
                        }

                        //Subtract/remove to list of HistoryItem
                        var _businessProductInList = db.BusinessProductsList.Where(c => c.ProductId == _item.ProductId && c.BusinessId == businessId && c.ListName == _removeOrUpdateInList).FirstOrDefault();
                        if (_businessProductInList != null) //update
                        {
                            if (_businessProductInList.Quantity - _item.Quantity < 1)
                            {
                                db.BusinessProductsList.Remove(_businessProductInList);
                            }
                            else
                            {
                                _businessProductInList.Quantity -= _item.Quantity;
                                _businessProductInList.LastAddedDate = DateTime.Now;
                            }
                        }

                        db.BusinessProductsListHistory.Remove(_item);
                        db.SaveChanges();
                        return new JsonApiResponse
                        {
                            Code = 1,
                            Success = true,
                            Data = null,
                            Message = "Success"
                        };
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Code = -1,
                            Success = false,
                            Data = null,
                            Message = "Not found"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return new JsonApiResponse
                {
                    Success = false,
                    Code = -10,
                    Message = "Error: " + ex.Message
                };
            }
        }


        static public JsonApiResponse AddBusinessDevice(int businessId, string deviceId, string deviceToken, string operativeSystem, string modelName, string lisieVersion)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    ClassLibrary1.BusinessDevices device = null;
                    if (!string.IsNullOrEmpty(deviceId))
                    {
                        device = db.BusinessDevices.Where(c => c.BusinessId == businessId && c.DeviceId == deviceId).FirstOrDefault();
                    }
                    //else
                    //{
                    //    device = db.BusinessDevices.Where(c => c.BusinessId == businessId).FirstOrDefault();
                    //}

                    if (device == null)
                    {
                        ClassLibrary1.BusinessDevices _newDevice = new ClassLibrary1.BusinessDevices();
                        _newDevice.BusinessId = businessId;
                        _newDevice.DeviceId = deviceId;
                        _newDevice.DeviceToken = deviceToken;
                        _newDevice.OperativeSystem = operativeSystem;
                        _newDevice.ModelName = modelName;
                        _newDevice.LisieVersion = lisieVersion;
                        _newDevice.UpdateDate = DateTime.Now;
                        _newDevice.CreateDate = DateTime.Now;
                        db.BusinessDevices.Add(_newDevice);
                        db.SaveChanges();

                        return new JsonApiResponse
                        {
                            Code = 1,
                            Success = true,
                            Data = _newDevice,
                            Message = "Success"
                        };
                    }
                    else
                    {
                        device.DeviceToken = deviceToken;
                        device.OperativeSystem = operativeSystem;
                        device.ModelName = modelName;
                        device.LisieVersion = lisieVersion;
                        device.UpdateDate = DateTime.Now;
                        db.BusinessDevices.Attach(device);
                        var entry = db.Entry(device);
                        entry.Property(y => y.DeviceToken).IsModified = true;
                        entry.Property(y => y.UpdateDate).IsModified = true;
                        entry.Property(y => y.ModelName).IsModified = true;
                        entry.Property(y => y.LisieVersion).IsModified = true;
                        entry.Property(y => y.OperativeSystem).IsModified = true;
                        db.SaveChanges();

                        return new JsonApiResponse
                        {
                            Code = 2,
                            Success = true,
                            Data = entry,
                            Message = "Success"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return new JsonApiResponse
                {
                    Success = false,
                    Code = -10,
                    Message = "Error: " + ex.Message
                };
            }
        }

        static public bool SendToBusinessDevice(int businessId, string title, string body, string data)
        {
            //var products = (string.IsNullOrEmpty(orderBy)) ? db.Products.ToList() : db.Products.OrderBy(c => c.Name).ToList();
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                if (string.IsNullOrEmpty(data))
                {
                    data = "{}";
                }
                var _businessDeviceTokens = db.BusinessDevices.Where(d => d.BusinessId == businessId && !string.IsNullOrEmpty(d.DeviceToken)).Select(d => d.DeviceToken).ToArray();
                if (_businessDeviceTokens.Length > 0)
                {
                    ExpoNotifications.Send(new ExpoNotificationModel
                    {
                        to = _businessDeviceTokens,
                        sound = "default",
                        title = title,
                        body = body,
                        data = data
                    });
                    return true;
                }
                return false;
            }
        }

        static public bool SendHistoryItemNotification(int businessId, string list, int productId, int quantity)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    DateTime _startDate = DateTime.Now;
                    DateTime _endDate = DateTime.Now;

                    var _newStartDate = new DateTime(_startDate.Year, _startDate.Month, _startDate.Day, 00, 00, 01);
                    var _newEndDate = new DateTime(_endDate.Year, _endDate.Month, _endDate.Day, 23, 59, 59);

                    var _history = from historyProduct in db.BusinessProductsListHistory
                                       //join product in db.BusinessProducts on historyProduct.ProductId equals product.Id
                                   where historyProduct.BusinessId == businessId && historyProduct.ListName == list
                                   && historyProduct.InsertDate >= _newStartDate && historyProduct.InsertDate <= _newEndDate
                                   && historyProduct.ProductId == productId
                                   orderby historyProduct.InsertDate descending
                                   select historyProduct;
                    var _distinctProductIds = _history.DistinctBy(c => c.ProductId).ToList();
                    var _newHistory = new BusinessProductHistoryDTO();

                    foreach (var item in _distinctProductIds)
                    {

                        var _totalProductQuantity = _history.Where(c => c.ProductId == item.ProductId).Sum(c => c.Quantity);
                        var _totalProductPrice = _history.Where(c => c.ProductId == item.ProductId).Sum(c => c.Price);
                        _newHistory = new BusinessProductHistoryDTO
                        {
                            Id = item.Id,
                            BusinessId = businessId,
                            Quantity = _totalProductQuantity.Value,
                            ProductId = item.ProductId.Value,
                            ItemType = item.ListName,
                            Name = item.ProductName,
                            Price = Helpers.TextTools.ParsePrice(_totalProductPrice.ToString()),
                            Items = _history.Where(c => c.ProductId == item.ProductId).ToList()
                        };

                        dynamic _data = new
                        {
                            action = "productAddedToHistory",
                            argument = _newHistory
                        };

                        SendToBusinessDevice(businessId, "Entrada de histórico adicionado", quantity + " de " + item.ProductName,
                            new JavaScriptSerializer().Serialize(_data));
                        return true;
                    }
                    return false;
                }

            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return false;
                throw;
            }
        }

        static public JsonApiResponse GetStock(int businessId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _products = from businessProduct in db.BusinessProductsList
                                    join product in db.BusinessProducts on businessProduct.ProductId equals product.Id
                                    where businessProduct.BusinessId == businessId && businessProduct.ListName == "inventory"
                                    orderby product.Name
                                    select new BusinessProductStockDTO
                                    {
                                        Name = product.Name,
                                        Quantity = businessProduct.Quantity.Value,
                                        Price = product.Price.Value
                                    };
                    return new JsonApiResponse
                    {
                        Success = true,
                        Code = 1,
                        Message = "Sucess",
                        Data = _products.ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);

                return new JsonApiResponse
                {
                    Success = false,
                    Code = -10,
                    Message = "Error: " + ex.Message
                };
            }
        }

        static public BusinessProductsStockChangesHistory AddStockChangeToHistory(int businessId, int productId, string productName, string listName, double price, int oldQuantity, int newQuantity)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _newHistoryItem = new BusinessProductsStockChangesHistory
                    {
                        BusinessId = businessId,
                        ProductName = productName,
                        ListName = listName,
                        Price = price,
                        OldQuantity = oldQuantity,
                        NewQuantity = newQuantity,
                        ProductId = productId,
                        InsertDate = DateTime.Now,
                    };
                    var _added = db.BusinessProductsStockChangesHistory.Add(_newHistoryItem);
                    db.SaveChanges();
                    return _newHistoryItem;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return null;
            }
        }

        static public JsonApiResponse GetStockChangeHistory(int businessId, string list, string startDate, string endDate)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    DateTime _startDate = DateTime.MinValue;
                    DateTime _endDate = DateTime.MinValue;

                    if (!DateTime.TryParse(startDate, out _startDate) || !DateTime.TryParse(endDate, out _endDate))
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -1,
                            Message = "Dates not in correct format"
                        };
                    }

                    var _newEndDate = new DateTime(_endDate.Year, _endDate.Month, _endDate.Day, 23, 59, 59);

                    var _history = from productsStockChangeHistory in db.BusinessProductsStockChangesHistory
                                       //join product in db.BusinessProducts on historyProduct.ProductId equals product.Id
                                   where productsStockChangeHistory.BusinessId == businessId && (productsStockChangeHistory.ListName == list
                                   || productsStockChangeHistory.ListName == "manual")
                                   && productsStockChangeHistory.InsertDate >= _startDate && productsStockChangeHistory.InsertDate <= _newEndDate
                                   orderby productsStockChangeHistory.InsertDate descending
                                   select productsStockChangeHistory;
                    var _newHistory = new List<BusinessProductStockChangesHistoryDTO>();

                    foreach (var item in _history)
                    {

                        _newHistory.Add(new BusinessProductStockChangesHistoryDTO
                        {
                            Id = item.Id,
                            BusinessId = businessId,
                            OldQuantity = item.OldQuantity ?? item.OldQuantity.Value,
                            NewQuantity = item.NewQuantity ?? item.NewQuantity.Value,
                            ProductId = item.ProductId.Value,
                            ItemType = item.ListName,
                            Name = item.ProductName,
                            Price = item.Price.Value,
                            InsertDate = item.InsertDate
                        });
                    }
                    return new JsonApiResponse
                    {
                        Success = true,
                        Code = 1,
                        Message = "Sucess",
                        Data = _newHistory
                    };
                    //}


                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);

                return new JsonApiResponse
                {
                    Success = false,
                    Code = -10,
                    Message = "Error: " + ex.Message
                };
            }
        }

        static public JsonApiResponse GetProductsCategories(int businessId)
        {
            try
            {
                var _lists = _GetProductsCategories(businessId);
                return new JsonApiResponse
                {
                    Success = true,
                    Code = 1,
                    Message = "Success",
                    Data = _lists
                };
            }
            catch (Exception ex)
            {

                Logger.Debug(ex.Message);

                return new JsonApiResponse
                {
                    Success = false,
                    Code = -10,
                    Message = "Error: " + ex.Message
                };
            }
        }

        static public List<string> _GetProductsCategories(int businessId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _categories = db.BusinessProducts.Where(c => !string.IsNullOrEmpty(c.Category)).DistinctBy(c => c.Category).Select(c => c.Category).ToList();
                    return _categories;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("error checking out products: " + ex.Message);
                return new List<string>();
            }
        }
    }

    public class BusinessProductListPostModel
    {
        public int ProductId { get; set; }
        public int BusinessId { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string ItemList { get; set; }
        public string Price { get; set; }
        public string ImageBase64 { get; set; }
        public string Category { get; set; }
    }

    public class BusinessProductListDTO
    {
        public int Id { get; set; }
        public int BusinessId { get; set; }
        public int ProductId { get; set; }
        public int? Quantity { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        //public string Weight { get; set; }
        //public Nullable<double> Price { get; set; }
        //public List<StoreProduct> PriceList { get; set; }
        //public Dictionary<string, double> TotalPriceList { get; set; }
        //public string Brand { get; set; }
        //public string Category { get; set; }
        public string ItemType { get; set; }
        public DateTime? LastAddedDate { get; set; }

        public bool IsToRemove { get; set; } = false;
        //public string ImageBase64 { get; set; }
    }

    public class BusinessProductDTO
    {
        public int BusinessId { get; set; }
        public int ProductId { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime? CreateDate { get; set; }
        //public string ImageBase64 { get; set; }
    }

    public class BusinessProductHistoryDTO
    {
        public int Id { get; set; }
        public int BusinessId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ItemType { get; set; }
        //public DateTime InsertDate { get; set; }
        public List<BusinessProductsListHistory> Items { get; set; }
    }

    public class BusinessProductStockChangesHistoryDTO
    {
        public int Id { get; set; }
        public int BusinessId { get; set; }
        public int ProductId { get; set; }
        public int NewQuantity { get; set; }
        public int OldQuantity { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ItemType { get; set; }
        public DateTime InsertDate { get; set; }
    }

    public class BusinessProductStockDTO
    {
        public int Id { get; set; }
        public int BusinessId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }

}