using ClassLibrary1;
using LisieStores.Extensibility;
using Microsoft.Ajax.Utilities;
using SpiroWeb.Helpers;
using SpiroWeb.Models;
using SpiroWeb.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Xml.Linq;

namespace SpiroWeb.Managers
{
    public static class ProductsManager
    {
        static private SpiroStockManagementEntities db = new SpiroStockManagementEntities();



        static public void UpdateProductWithSearchResult(int productId, LisieStores.Extensibility.ProductSearchResult searchResult, string userId, string barCode, string appDataPath, bool updateBarcode)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                Products _product = db.Products.Where(c => c.Id == productId).FirstOrDefault();
                if (_product != null)
                {
                    if (updateBarcode) _product.Barcode = !string.IsNullOrEmpty(barCode) ? barCode : "0";
                    _product.Name = searchResult.Name;
                    _product.Price = double.Parse(searchResult.Price.Replace("€", "").Trim());
                    _product.VariableWeightPrice = searchResult.PriceWeight;
                    _product.CategoryString = searchResult.Category;
                    _product.Brand = searchResult.Brand;
                    _product.Weight = searchResult.Weight;
                    _product.InsertDate = DateTime.Now;
                    _product.CreatedByUserId = userId;
                    WebClient _client = new WebClient();
                    _client.DownloadFile(new Uri(searchResult.ImageUrl), appDataPath);
                    byte[] _imageInBase64 = ManageImage.GetBase64OfImagePath(appDataPath);
                    _product.Picture = _imageInBase64;
                }
            }

        }

        static public void CopyProduct(Products product, Products sourceProduct, string appDataPath)
        {
            product.Barcode = sourceProduct.Barcode;
            product.Name = sourceProduct.Name;
            product.Price = sourceProduct.Price;
            product.VariableWeightPrice = sourceProduct.VariableWeightPrice;
            product.CategoryString = sourceProduct.CategoryString;
            product.Brand = sourceProduct.Brand;
            product.Weight = sourceProduct.Weight;
            product.InsertDate = DateTime.Now;
            product.CreatedByUserId = sourceProduct.CreatedByUserId;
            product.Picture = sourceProduct.Picture;
        }

        static public void CreateStoreProduct(Objects.ProductSearchResult searchResult, int productId, string userId, string productUrl, int storeId)
        {
            StoreProducts _storeProduct = new StoreProducts();
            _storeProduct.Url = productUrl;
            _storeProduct.Price = double.Parse(searchResult.Price.Replace("€", "").Trim());
            _storeProduct.UserId = userId;
            _storeProduct.ProductId = productId;
            _storeProduct.CreateDate = DateTime.Now;
            _storeProduct.StoreId = storeId;
            db.StoreProducts.Add(_storeProduct);
            db.SaveChanges();
        }

        //static public bool CreateOrUpdateStoreProduct(Objects.ProductSearchResult searchResult, int productId, string userId, int storeId, bool ifExistsDontUpdate = false)
        //{
        //    var _storeProductExists = db.StoreProducts.Where(c => c.ProductId == productId && c.StoreId == storeId).FirstOrDefault();
        //    if (_storeProductExists != null)
        //    {
        //        if (ifExistsDontUpdate == false)
        //        {
        //            _storeProductExists.Url = searchResult.Url;
        //            _storeProductExists.Price = Math.Round(double.Parse(searchResult.Price.Replace("€", "").Trim()), 2);
        //            _storeProductExists.UpdateDate = DateTime.Now;
        //            _storeProductExists.NeedsUpdate = false;
        //            db.SaveChanges();
        //            return true;
        //        }
        //        return false;
        //    }
        //    else
        //    {
        //        StoreProducts _storeProduct = new StoreProducts();
        //        _storeProduct.Url = searchResult.Url;
        //        _storeProduct.Price = Math.Round(double.Parse(searchResult.Price.Replace("€", "").Trim()), 2);
        //        _storeProduct.UserId = userId;
        //        _storeProduct.ProductId = productId;
        //        _storeProduct.CreateDate = DateTime.Now;
        //        _storeProduct.StoreId = storeId;
        //        _storeProduct.NeedsUpdate = false;
        //        db.StoreProducts.Add(_storeProduct);
        //        db.SaveChanges();
        //        return true;
        //    }
        //}

        //Obsolete
        static public bool CreateOrUpdateStoreProduct(Objects.ProductSearchResult searchResult, int productId, string userId, int storeId, bool ifExistsDontUpdate = false, bool updateIfUserHasPermission = true)
        {
            var _storeProductExists = db.StoreProducts.Where(c => c.ProductId == productId && c.StoreId == storeId).FirstOrDefault();
            if (_storeProductExists != null)
            {
                if (ifExistsDontUpdate == false)
                {
                    if (!updateIfUserHasPermission) //permission not requires
                    {
                        _storeProductExists.Url = searchResult.Url;
                        _storeProductExists.Price = Math.Round(double.Parse(searchResult.Price.Replace("€", "").Trim()), 2);
                        _storeProductExists.UpdateDate = DateTime.Now;
                        _storeProductExists.NeedsUpdate = false;
                        db.SaveChanges();
                        return true;
                    }
                    else //permission requires
                    {
                        //check if user hás permission
                        //is user a moderador?
                        var _userIsModerator = UserPermissionsManager.IsUserModerator(userId);
                        //var _userIsModerator = false;
                        if (_userIsModerator || _storeProductExists.UserId.Equals(userId))
                        {
                            _storeProductExists.Url = searchResult.Url;
                            _storeProductExists.Price = Math.Round(double.Parse(searchResult.Price.Replace("€", "").Trim()), 2);
                            _storeProductExists.UpdateDate = DateTime.Now;
                            _storeProductExists.NeedsUpdate = false;
                            db.SaveChanges();
                            return true;
                        }
                        else
                            return false; //user has no permission

                    }
                }
                return false; //It exists, but because of ifExistsDontUpdate=true, don´t update
            }
            else
            {
                StoreProducts _storeProduct = new StoreProducts();
                _storeProduct.Url = searchResult.Url;
                _storeProduct.Price = Math.Round(double.Parse(searchResult.Price.Replace("€", "").Trim()), 2);
                _storeProduct.UserId = userId;
                _storeProduct.ProductId = productId;
                _storeProduct.CreateDate = DateTime.Now;
                _storeProduct.StoreId = storeId;
                _storeProduct.NeedsUpdate = false;
                db.StoreProducts.Add(_storeProduct);
                db.SaveChanges();
                return true;
            }
        }

        static public bool CreateOrUpdateStoreProductNew(LisieStores.Extensibility.ProductSearchResult searchResult, int productId, string userId, int storeId, bool ifExistsDontUpdate = false, bool updateIfUserHasPermission = true, bool isTemp = true)
        {
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                var _storeProductExists = db2.StoreProducts.Where(c => c.ProductId == productId && c.StoreId == storeId).FirstOrDefault();
                if (_storeProductExists != null)
                {
                    if (ifExistsDontUpdate == false)
                    {
                        if (!updateIfUserHasPermission) //permission not requires
                        {
                            _storeProductExists.Url = searchResult.Url;

                            //FOR LOCAL AND PRODUCTION
                            //if (HttpContext.Current.Request.IsLocal)
                            //    _storeProductExists.Price = searchResult.Price != null ? Math.Round(double.Parse(searchResult.Price.Replace("€", "").Replace(",", ".").Trim()), 2) : 0;
                            //else
                            //    _storeProductExists.Price = searchResult.Price != null ? Math.Round(double.Parse(searchResult.Price.Replace("€", "").Replace(".", ",").Trim()), 2) : 0;
                            if (HttpContext.Current.Request.IsLocal)
                                _storeProductExists.Price = searchResult.Price != null ? TextTools.ParsePriceLocal(searchResult.Price) : 0;
                            else
                                _storeProductExists.Price = searchResult.Price != null ? TextTools.ParsePriceProduction(searchResult.Price) : 0;

                            _storeProductExists.UpdateDate = DateTime.Now;
                            _storeProductExists.NeedsUpdate = false;
                            _storeProductExists.OnlineProductId = searchResult.OnlineProductId;

                            //New fields (for comparising purposes)
                            if (!string.IsNullOrEmpty(searchResult.Name))
                                _storeProductExists.Name = searchResult.Name;
                            if (!string.IsNullOrEmpty(searchResult.Brand))
                                _storeProductExists.Brand = searchResult.Brand;
                            if (!string.IsNullOrEmpty(searchResult.Weight))
                                _storeProductExists.Weight = searchResult.Weight;
                            if (!string.IsNullOrEmpty(searchResult.ImageUrl))
                                _storeProductExists.ImageUrl = searchResult.ImageUrl;

                            //FOR LOCAL AND PRODUCTION
                            //if (HttpContext.Current.Request.IsLocal)
                            //    _storeProductExists.PriceRatio = searchResult.PriceWeight != null ? Math.Round(double.Parse(searchResult.PriceWeight.Replace("€", "").Replace(",", ".").Trim()), 2) : 0;
                            //else
                            //    _storeProductExists.PriceRatio = searchResult.PriceWeight != null ? Math.Round(double.Parse(searchResult.PriceWeight.Replace("€", "").Replace(".", ",").Trim()), 2) : 0;
                            if (HttpContext.Current.Request.IsLocal)
                                _storeProductExists.PriceRatio = searchResult.PriceWeight != null ? TextTools.ParsePriceLocal(searchResult.PriceWeight) : 0;
                            else
                                _storeProductExists.PriceRatio = searchResult.PriceWeight != null ? TextTools.ParsePriceProduction(searchResult.PriceWeight) : 0;

                            _storeProductExists.Unit = searchResult.Unit != null ? searchResult.Unit : "un";
                            db2.SaveChanges();
                            return true;
                        }
                        else //permission requires
                        {
                            //check if user hás permission
                            //is user a moderador?
                            var _userIsModerator = UserPermissionsManager.IsUserModerator(userId);
                            //var _userIsModerator = false;
                            if (_userIsModerator || _storeProductExists.UserId.Equals(userId))
                            {
                                _storeProductExists.Url = searchResult.Url;

                                //FOR LOCAL AND PRODUCTION
                                //if (HttpContext.Current.Request.IsLocal)
                                //    _storeProductExists.Price = searchResult.Price != null ? Math.Round(double.Parse(searchResult.Price.Replace("€", "").Replace(",", ".").Trim()), 2) : 0;
                                //else
                                //    _storeProductExists.Price = searchResult.Price != null ? Math.Round(double.Parse(searchResult.Price.Replace("€", "").Replace(".", ",").Trim()), 2) : 0;
                                if (HttpContext.Current.Request.IsLocal)
                                    _storeProductExists.Price = searchResult.Price != null ? TextTools.ParsePriceLocal(searchResult.Price) : 0;
                                else
                                    _storeProductExists.Price = searchResult.Price != null ? TextTools.ParsePriceProduction(searchResult.Price) : 0;

                                _storeProductExists.OnlineProductId = searchResult.OnlineProductId;
                                _storeProductExists.UpdateDate = DateTime.Now;
                                _storeProductExists.NeedsUpdate = false;


                                //New fields (for comparising purposes)
                                if (!string.IsNullOrEmpty(searchResult.Name))
                                    _storeProductExists.Name = searchResult.Name;
                                if (!string.IsNullOrEmpty(searchResult.Brand))
                                    _storeProductExists.Brand = searchResult.Brand;
                                if (!string.IsNullOrEmpty(searchResult.Weight))
                                    _storeProductExists.Weight = searchResult.Weight;
                                if (!string.IsNullOrEmpty(searchResult.ImageUrl))
                                    _storeProductExists.ImageUrl = searchResult.ImageUrl;

                                //FOR LOCAL AND PRODUCTION
                                //if (HttpContext.Current.Request.IsLocal)
                                //    _storeProductExists.PriceRatio = searchResult.PriceWeight != null ? Math.Round(double.Parse(searchResult.PriceWeight.Replace("€", "").Replace(",", ".").Trim()), 2) : 0;
                                //else
                                //    _storeProductExists.PriceRatio = searchResult.PriceWeight != null ? Math.Round(double.Parse(searchResult.PriceWeight.Replace("€", "").Replace(".", ",").Trim()), 2) : 0;
                                if (HttpContext.Current.Request.IsLocal)
                                    _storeProductExists.PriceRatio = searchResult.PriceWeight != null ? TextTools.ParsePriceLocal(searchResult.PriceWeight) : 0;
                                else
                                    _storeProductExists.PriceRatio = searchResult.PriceWeight != null ? TextTools.ParsePriceProduction(searchResult.PriceWeight) : 0;

                                _storeProductExists.Unit = searchResult.Unit != null ? searchResult.Unit : "un";
                                db2.SaveChanges();
                                return true;
                            }
                            else
                                return false; //user has no permission

                        }
                    }
                    return false; //It exists, but because of ifExistsDontUpdate=true, don´t update
                }
                else
                {
                    StoreProducts _storeProduct = new StoreProducts();
                    _storeProduct.Url = searchResult.Url;

                    //FOR LOCAL AND PRODUCTION
                    //if (HttpContext.Current.Request.IsLocal)
                    //    _storeProduct.Price = searchResult.Price != null ? Math.Round(double.Parse(searchResult.Price.Replace("€", "").Replace(",", ".").Trim()), 2) : 0;
                    //else
                    //    _storeProduct.Price = searchResult.Price != null ? Math.Round(double.Parse(searchResult.Price.Replace("€", "").Replace(".", ",").Trim()), 2) : 0;
                    if (HttpContext.Current.Request.IsLocal)
                        _storeProduct.Price = searchResult.Price != null ? TextTools.ParsePriceLocal(searchResult.Price) : 0;
                    else
                        _storeProduct.Price = searchResult.Price != null ? TextTools.ParsePriceProduction(searchResult.Price) : 0;


                    _storeProduct.OnlineProductId = searchResult.OnlineProductId;
                    _storeProduct.UserId = userId;
                    _storeProduct.ProductId = productId;
                    _storeProduct.CreateDate = DateTime.Now;
                    _storeProduct.StoreId = storeId;
                    _storeProduct.NeedsUpdate = false;
                    //_storeProduct.IsTemp = true;
                    _storeProduct.IsTemp = isTemp;

                    //New fields (for comparising purposes)
                    if (!string.IsNullOrEmpty(searchResult.Name))
                        _storeProduct.Name = searchResult.Name;
                    if (!string.IsNullOrEmpty(searchResult.Brand))
                        _storeProduct.Brand = searchResult.Brand;
                    if (!string.IsNullOrEmpty(searchResult.Weight))
                        _storeProduct.Weight = searchResult.Weight;
                    if (!string.IsNullOrEmpty(searchResult.ImageUrl))
                        _storeProduct.ImageUrl = searchResult.ImageUrl;

                    //FOR LOCAL AND PRODUCTION
                    //if (!string.IsNullOrEmpty(searchResult.PriceWeight))
                    //{
                    //    if (HttpContext.Current.Request.IsLocal)
                    //        _storeProduct.PriceRatio = searchResult.PriceWeight != null ? Math.Round(double.Parse(searchResult.PriceWeight.Replace("€", "").Replace(",", ".").Trim()), 2) : 0;
                    //    else
                    //        _storeProduct.PriceRatio = searchResult.PriceWeight != null ? Math.Round(double.Parse(searchResult.PriceWeight.Replace("€", "").Replace(".", ",").Trim()), 2) : 0;
                    //}
                    if (!string.IsNullOrEmpty(searchResult.PriceWeight))
                    {
                        if (HttpContext.Current.Request.IsLocal)
                            _storeProduct.PriceRatio = searchResult.PriceWeight != null ? TextTools.ParsePriceLocal(searchResult.PriceWeight) : 0;
                        else
                            _storeProduct.PriceRatio = searchResult.PriceWeight != null ? TextTools.ParsePriceProduction(searchResult.PriceWeight) : 0;
                    }

                    _storeProduct.Unit = searchResult.Unit != null ? searchResult.Unit : "un";
                    db2.StoreProducts.Add(_storeProduct);
                    db2.SaveChanges();
                    Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastNewStoreProduct("9ff8224f-17cf-49fb-b555-05779a13eb40", GetTotalStoreProducts());
                    return true;
                }
            }
        }

        //For Go Getter purposes
        //static public bool CreateOrUpdateStoreProductNewGoGetter(LisieStores.Extensibility.ProductSearchResult searchResult, int productId, string userId, int storeId, bool ifExistsDontUpdate = false, bool updateIfUserHasPermission = true)
        //{
        //    var _storeProductExists = db.StoreProductsNew.Where(c => c.ProductId == productId && c.StoreId == storeId).FirstOrDefault();
        //    if (_storeProductExists != null)
        //    {
        //        if (ifExistsDontUpdate == false)
        //        {
        //            if (!updateIfUserHasPermission) //permission not requires
        //            {
        //                _storeProductExists.Url = searchResult.Url;
        //                //FOR LOCAL
        //                //_storeProductExists.Price = Math.Round(double.Parse(searchResult.Price.Replace("€", "").Replace(",", ".").Trim()), 2);
        //                //FOR PRODUCTION
        //                _storeProductExists.Price = Math.Round(double.Parse(searchResult.Price.Replace("€", "").Replace(",", ".").Trim()), 2);
        //                _storeProductExists.UpdateDate = DateTime.Now;
        //                _storeProductExists.NeedsUpdate = false;
        //                _storeProductExists.OnlineProductId = searchResult.OnlineProductId;
        //                db.SaveChanges();
        //                return true;
        //            }
        //            else //permission requires
        //            {
        //                //check if user hás permission
        //                //is user a moderador?
        //                var _userIsModerator = UserPermissionsManager.IsUserModerator(userId);
        //                //var _userIsModerator = false;
        //                if (_userIsModerator || _storeProductExists.UserId.Equals(userId))
        //                {
        //                    _storeProductExists.Url = searchResult.Url;

        //                    //FOR LOCAL
        //                    //_storeProductExists.Price = Math.Round(double.Parse(searchResult.Price.Replace("€", "").Replace(",", ".").Trim()), 2);
        //                    //FOR PRODUCTION
        //                    _storeProductExists.Price = Math.Round(double.Parse(searchResult.Price.Replace("€", "").Replace(",", ".").Trim()), 2);
        //                    _storeProductExists.OnlineProductId = searchResult.OnlineProductId;
        //                    _storeProductExists.UpdateDate = DateTime.Now;
        //                    _storeProductExists.NeedsUpdate = false;
        //                    db.SaveChanges();
        //                    return true;
        //                }
        //                else
        //                    return false; //user has no permission

        //            }
        //        }
        //        return false; //It exists, but because of ifExistsDontUpdate=true, don´t update
        //    }
        //    else
        //    {
        //        StoreProductsNew _storeProduct = new StoreProductsNew();
        //        _storeProduct.Url = searchResult.Url;
        //        //FOR LOCAL
        //        //_storeProduct.Price = Math.Round(double.Parse(searchResult.Price.Replace("€", "").Replace(",", ".").Trim()), 2);
        //        //FOR PRODUCTION
        //        _storeProduct.Price = Math.Round(double.Parse(searchResult.Price.Replace("€", "").Replace(",", ".").Trim()), 2);

        //        _storeProduct.UserId = userId;
        //        _storeProduct.ProductId = productId;
        //        _storeProduct.CreateDate = DateTime.Now;
        //        _storeProduct.StoreId = storeId;
        //        _storeProduct.NeedsUpdate = false;
        //        _storeProduct.OnlineProductId = searchResult.OnlineProductId;
        //        db.StoreProductsNew.Add(_storeProduct);
        //        db.SaveChanges();
        //        return true;
        //    }
        //}

        public static int AddProductSimpleToUserList(ProductSimpleItem productSimple)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _exists = db.UserProductsSimple.Where(c => c.Name.ToLower().Trim() == productSimple.Name.ToLower().Trim() && c.UserId == productSimple.UserId).FirstOrDefault();
                if (_exists != null)
                {
                    _exists.Quantity++;
                    db.SaveChanges();
                    return _exists.Id;
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
                    return _UserProductsSimpleNew.Id;
                }
            }
        }

        public static string AddProductRecognizedIoTToUserList(string userId, string productRecognized)
        {
            //make translations here
            switch (productRecognized.ToLower().Replace("_", " "))
            {
                case "apple":
                    productRecognized = "Maça";
                    break;
                case "orange":
                    productRecognized = "Laranja";
                    break;
                case "bell pepper red":
                    productRecognized = "Pimento vermelho";
                    break;
                default:
                    break;
            }

            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _exists = db.UserProductsSimple.Where(c => c.Name.ToLower().Trim() == productRecognized.ToLower().Trim() && c.UserId == userId).FirstOrDefault();
                if (_exists != null)
                {
                    _exists.Quantity++;
                    db.SaveChanges();
                    return productRecognized;
                }
                else
                {
                    UserProductsSimple _UserProductsSimpleNew = new UserProductsSimple
                    {
                        Name = productRecognized,
                        ImageUrl = string.Empty,
                        Quantity = 1,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        UserId = userId
                    };
                    db.UserProductsSimple.Add(_UserProductsSimpleNew);
                    db.SaveChanges();
                    return productRecognized;
                }
            }
        }


        static public void DeleteStoreProductsOfProduct(int productId)
        {
            var storeProducts = db.StoreProducts.Where(c => c.ProductId == productId).ToList();
            if (storeProducts.Count > 0) db.StoreProducts.RemoveRange(storeProducts);
            db.SaveChanges();
        }

        static public bool DeleteStoreProductOfProductNew(int productId, string userId, int storeId, bool deleteIfUserHasPermission = true)
        {
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                var _storeProduct = db2.StoreProducts.Where(c => c.ProductId == productId && c.StoreId == storeId).FirstOrDefault();
                if (_storeProduct != null)
                {
                    if (deleteIfUserHasPermission) //Permission to delete requires
                    {
                        var _userIsModerator = UserPermissionsManager.IsUserModerator(userId);
                        if (_userIsModerator || _storeProduct.UserId.Equals(userId)) //has permission
                        {
                            db2.StoreProducts.Remove(_storeProduct);
                            db2.SaveChanges();
                            return true;
                        }
                        else
                            return false;
                    }
                    else //Permission not requires
                    {
                        db2.StoreProducts.Remove(_storeProduct);
                        db2.SaveChanges();
                        return true;
                    }
                }
                else
                    return false;
            }
        }

        static public string GetProductJumboStoreUrl(int productId)
        {
            var userShoppingList2 = from m in db.StoreProducts where m.ProductId == productId && m.StoreId == 1 select m.Url;
            if (userShoppingList2.Count() > 0)
            {
                return "http://www.auchan.pt" + userShoppingList2.First();
            }
            else return string.Empty;
        }

        static public string GetProductContinenteStoreUrl(int productId)
        {
            var userShoppingList2 = from m in db.StoreProducts where m.ProductId == productId && m.StoreId == 2 select m.Url;
            if (userShoppingList2.Count() > 0)
            {
                return "http://www.continente.pt" + userShoppingList2.First();
            }
            else return string.Empty;
        }

        static public string GetProductPingoDoceStoreUrl(int productId)
        {
            var userShoppingList2 = from m in db.StoreProducts where m.ProductId == productId && m.StoreId == 3 select m.Url;
            if (userShoppingList2.Count() > 0)
            {
                return "http://www.mercadao.pt" + userShoppingList2.First();
            }
            else return string.Empty;
        }

        static public int AddNewProduct(Products newProduct)
        {
            try
            {
                if (newProduct != null && !string.IsNullOrEmpty(newProduct.Barcode))
                {
                    db.Products.Add(newProduct);
                    db.SaveChanges();
                    Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastNewProduct("9ff8224f-17cf-49fb-b555-05779a13eb40", GetTotal());
                    return newProduct.Id;
                }

                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        static public Products GetById(int id)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                return db.Products.Include("StoreProducts").Where(c => c.Id.Equals(id)).FirstOrDefault();
            }
        }

        static public Products GetByBarcode(string barcode)
        {
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                var _product = db.Products.Include("StoreProducts").Where(c => c.Barcode.Equals(barcode)).FirstOrDefault();
                if (_product != null)
                {
                    return _product;
                }
                else if (barcode.StartsWith("0"))
                {
                    _product = db.Products.Include("StoreProducts").Where(c => c.Barcode.Equals(barcode.Remove(0, 1))).FirstOrDefault();
                    return _product;
                }
                else //Find weight barcodes 
                {
                    _product = db.Products.Include("StoreProducts").Where(c => c.Barcode.StartsWith(barcode.Substring(0, 7)) && c.Barcode.EndsWith("000000")).FirstOrDefault();
                    if (_product != null)
                    {
                        return _product;
                    }
                }
                return null;
            }
        }

        static public Products GetByBarcodeV2(string barcode, string userId = "")
        {
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                var _product = db.Products.Include("StoreProducts")
                    .Where(c => c.Barcode.Equals(barcode) && (!c.IsTemp.HasValue || c.IsTemp == false))
                    .FirstOrDefault();
                if (_product != null)
                {
                    return _product;
                }

                if (barcode.StartsWith("0"))
                {
                    _product = db.Products.Include("StoreProducts")
                        .Where(c => c.Barcode.Equals(barcode.Remove(0, 1)) && (!c.IsTemp.HasValue || c.IsTemp == false))
                        .FirstOrDefault();
                    if (_product != null)
                    {
                        return _product;
                    }

                }

                _product = db.Products.Include("StoreProducts").Where(c => c.Barcode.StartsWith(barcode.Substring(0, 7)) && c.Barcode.EndsWith("000000")).FirstOrDefault();
                if (_product != null)
                {
                    return _product;
                }

                //see in user temp products
                if (!string.IsNullOrEmpty(userId))
                {
                    _product = db.Products.Include("StoreProducts")
                    .Where(c => c.Barcode.Equals(barcode) &&
                               (c.IsTemp.HasValue || c.IsTemp == true) && c.CreatedByUserId == userId)
                    .FirstOrDefault();
                    if (_product != null)
                    {
                        return _product;
                    }

                    if (barcode.StartsWith("0"))
                    {
                        _product = db.Products.Include("StoreProducts")
                             .Where(c => c.Barcode.Equals(barcode.Remove(0, 1)) &&
                                        (c.IsTemp.HasValue || c.IsTemp == true) && c.CreatedByUserId == userId)
                             .FirstOrDefault();
                        if (_product != null)
                        {
                            return _product;
                        }
                    }

                    _product = db.Products.Include("StoreProducts")
                        .Where(c => c.Barcode.StartsWith(barcode.Substring(0, 7)) &&
                                    c.Barcode.EndsWith("000000") &&
                                   (c.IsTemp.HasValue || c.IsTemp == true) && c.CreatedByUserId == userId)
                    .FirstOrDefault();
                    if (_product != null)
                    {
                        return _product;
                    }
                }

                return null;
            }
        }


        static public Products GetDTOById(int id)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var product = db.Products.Include("StoreProducts").Where(c => c.Id.Equals(id)).FirstOrDefault();
                if (product != null)
                {
                    return new Products()
                    {
                        Id = product.Id,
                        Barcode = product.Barcode,
                        Brand = product.Brand,
                        CategoryString = product.CategoryString,
                        InsertDate = product.InsertDate,
                        Name = product.Name,
                        Price = product.Price,
                        VariableWeightPrice = product.VariableWeightPrice,
                        StoreProducts = Managers.ProductsManager.GetStoreProductsCopy(product.StoreProducts),
                        Weight = product.Weight,
                        AddedByUserId = product.AddedByUserId,
                        CreatedByUserId = product.CreatedByUserId,
                        IsTemp = product.IsTemp,
                        FullCategory = product.FullCategory
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        static public Products GetByStoreIdAndOnlineProductId(int storeId, string onlineProductId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _storeProduct = db.StoreProducts.Include("Products").Where(c => c.StoreId == storeId && c.OnlineProductId == onlineProductId).FirstOrDefault();
                if (_storeProduct != null)
                {
                    return _storeProduct.Products;
                }
                else
                {
                    return null;
                }
            }
        }

        static public Products GetByStoreIdAndUrl(int storeId, string url)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _storeProduct = db.StoreProducts.Include("Products").Where(c => c.StoreId == storeId && c.Url == url).FirstOrDefault();
                if (_storeProduct != null)
                {
                    return _storeProduct.Products;
                }
                else
                {
                    return null;
                }
            }
        }

        //if it finds online, it creates new product in database
        static async public Task<Products> GetByBarcodeAndSearchOnlineAndAddIfFound(string barcode, string userId)
        {
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                List<LisieStores.Extensibility.ProductSearchResult> _results = null;

                var _product = GetByBarcode(barcode);
                if (_product != null)
                {
                    return _product;
                }
                else //Find online 
                {
                    _results = await GetByBarcodeOnline(barcode);
                }

                //if found online, create new product and return it
                if (_results != null && _results.Count > 0)
                {
                    ProductItemCreate _ProductItemCreate = new ProductItemCreate();
                    _ProductItemCreate.Barcode = _results[0].Barcode;
                    _ProductItemCreate.FirstAddedProductFromStoreId = _results[0].StoreId;
                    _ProductItemCreate.UserId = userId;
                    _ProductItemCreate.SelectedResults = new List<LisieStores.Extensibility.ProductSearchResult>();
                    foreach (var _result in _results)
                    {
                        _ProductItemCreate.SelectedResults.Add(new LisieStores.Extensibility.ProductSearchResult
                        {
                            StoreId = _result.StoreId,
                            Url = _result.Url
                        });
                    }
                    int _newProductId = await CreateV2(_ProductItemCreate);
                    if (_newProductId > 0)
                    {
                        return GetById(_newProductId);
                    }
                }

                return null;
            }
        }

        static async public Task<List<LisieStores.Extensibility.ProductSearchResult>> GetByBarcodeOnline(string barcode)
        {
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                OnlineProducts _OnlineProducts = new OnlineProducts();
                List<LisieStores.Extensibility.ProductSearchResult> _results = null;
                if (barcode.StartsWith("0")) //try first without 0 before, than with
                {
                    _results = await _OnlineProducts.GetMarketsProductByBarcode(barcode.Remove(0, 1)); //without 0
                    if (_results == null || _results.Count == 0)//try with 0
                        _results = await _OnlineProducts.GetMarketsProductByBarcode(barcode);
                }
                else
                    _results = await _OnlineProducts.GetMarketsProductByBarcode(barcode);

                if (_results == null || _results.Count == 0) //try to get by weight barcode
                {
                    string _BarcodeWeight = barcode;
                    if (_BarcodeWeight.StartsWith("0"))
                        _BarcodeWeight = _BarcodeWeight.Remove(0, 1);
                    _BarcodeWeight = _BarcodeWeight.Substring(0, 7) + "000000";
                    _results = await _OnlineProducts.GetMarketsProductByBarcode(_BarcodeWeight);
                }

                return _results;
            }
        }

        static public List<Products> GetProductsAutocomplete(string term, bool withoutBarcode)
        {
            if (withoutBarcode)
                return (from c in db.Products
                        where (c.Name.ToLower().Contains(term.ToLower()) ||
                               c.Brand.ToLower().Contains(term.ToLower())) &&
                               c.Barcode == "0"
                        select c).ToList();
            else
                return (from c in db.Products
                        where c.Name.ToLower().Contains(term.ToLower()) ||
                               c.Brand.ToLower().Contains(term.ToLower())
                        select c).ToList();
        }

        static public List<Models.UserProductListCompleteModel2> GetAll(int page, string query, bool withoutBarcode = false)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                //List<Products> productsList = new List<Products>();
                //IQueryable<Products> productsList = new IQueryable<Products>();
                IQueryable productsList = Enumerable.Empty<Products>().AsQueryable();
                if (string.IsNullOrEmpty(query))
                {
                    //productsList = db.Products.OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6).ToList();
                    productsList = db.Products.OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6);
                }
                else
                {
                    //IN FUTURE MAYBE
                    if (page > 0)
                    {
                        productsList = db.Products.Where(c => c.Name.ToLower().Contains(query.ToLower()) ||
                            c.Brand.ToLower().Contains(query.ToLower()))
                            .OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6);
                    }
                    else //if -1 return all
                    {
                        var decomposed = query.Normalize(NormalizationForm.FormD);
                        var filtered = decomposed.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
                        var _normalizedQuery = new String(filtered.ToArray());

                        string[] _searchWords = query.ToLower().Trim(' ').Split(' ');
                        string[] _searchWordsNormalized = _normalizedQuery.ToLower().Trim(' ').Split(' ');
                        if (withoutBarcode)
                        {
                            productsList = db.Products.Where(c => _searchWords.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z) &&
                            c.Barcode.Equals("0"))
                            ||
                            _searchWordsNormalized.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z))
                            && c.Barcode.Equals("0"))
                               .OrderBy(c => c.Name);
                        }
                        else
                        {
                            productsList = db.Products.Where(c => _searchWords.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z)) ||
                             _searchWordsNormalized.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z)))
                                    .OrderBy(c => c.Name);
                        }
                    }
                }
                List<Models.UserProductListCompleteModel2> _list = new List<Models.UserProductListCompleteModel2>();
                foreach (Products product in productsList)
                {
                    _list.Add(new UserProductListCompleteModel2()
                    {
                        ProductId = product.Id,
                        Barcode = product.Barcode,
                        Brand = product.Brand,
                        Category = product.CategoryString,
                        LastAddedDate = product.InsertDate.Value,
                        Name = product.Name,
                        Price = product.Price,
                        Weight = product.Weight
                    });
                }

                foreach (var _product in _list)
                {
                    var _storeProducts = from m in db.StoreProducts where m.ProductId == _product.ProductId select m;
                    if (_storeProducts.Count() > 0)
                    {
                        foreach (var storeProduct in _storeProducts)
                        {
                            if (_product.PriceList == null) _product.PriceList = new List<Models.StoreProduct>();
                            _product.PriceList.Add(new Models.StoreProduct
                            {
                                Id = storeProduct.Id,
                                Price = Math.Round(storeProduct.Price.Value, 2),
                                PriceRatio = storeProduct.PriceRatio.HasValue ? Math.Round(storeProduct.PriceRatio.Value, 2) : 0,
                                StoreId = storeProduct.StoreId,
                                Url = storeProduct.Url,
                                CreatedByUserId = storeProduct.UserId,
                                NeedsUpdate = ((storeProduct.NeedsUpdate.HasValue) ? storeProduct.NeedsUpdate.Value : false),
                                UpdateDate = ((storeProduct.UpdateDate.HasValue) ? storeProduct.UpdateDate.Value : DateTime.MinValue),
                                Unit = storeProduct.Unit
                            });
                        }
                    }
                }
                return _list;
            }
        }

        //with inner join
        static public List<Models.UserProductListCompleteModel2> GetAllV2(int page, string query, bool withoutBarcode = false)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                //List<Products> productsList = new List<Products>();
                //IQueryable<Products> productsList = new IQueryable<Products>();
                IQueryable<Products> productsList = Enumerable.Empty<Products>().AsQueryable();
                if (string.IsNullOrEmpty(query))
                {
                    //productsList = db.Products.OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6).ToList();
                    productsList = db.Products.OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6);
                }
                else
                {
                    //IN FUTURE MAYBE
                    if (page > 0)
                    {
                        productsList = db.Products.Where(c => c.Name.ToLower().Contains(query.ToLower()) ||
                            c.Brand.ToLower().Contains(query.ToLower()))
                            .OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6);
                    }
                    else //if -1 return all
                    {
                        var decomposed = query.Normalize(NormalizationForm.FormD);
                        var filtered = decomposed.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
                        var _normalizedQuery = new String(filtered.ToArray());

                        string[] _searchWords = query.ToLower().Trim(' ').Split(' ');
                        string[] _searchWordsNormalized = _normalizedQuery.ToLower().Trim(' ').Split(' ');
                        if (withoutBarcode)
                        {
                            productsList = db.Products.Where(c => _searchWords.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z) &&
                            c.Barcode.Equals("0"))
                            ||
                            _searchWordsNormalized.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z))
                            && c.Barcode.Equals("0"))
                               .OrderBy(c => c.Name);
                        }
                        else
                        {
                            productsList = db.Products.Where(c => _searchWords.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z)) ||
                             _searchWordsNormalized.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z)))
                                    .OrderBy(c => c.Name);
                        }
                    }
                }

                //try to inner join
                var _temp =
                    from m in productsList
                    join storePrd in db.StoreProducts on m.Id equals storePrd.ProductId
                    //orderby m.Id descending
                    select new UserProductListCompleteTempModel
                    {
                        Id = storePrd.Id,
                        ProductId = m.Id,
                        //Quantity = m.Quantity.Value,
                        Barcode = m.Barcode,
                        Brand = m.Brand,
                        ItemType = "product",
                        Name = m.Name,
                        Category = m.CategoryString,
                        //Price = 0,
                        StorePrice = Math.Round(storePrd.Price.Value, 2),
                        StoreId = storePrd.StoreId,
                        StoreProductId = storePrd.Id,
                        NeedsUpdate = storePrd.NeedsUpdate,
                        Url = storePrd.Url,
                        CreatedByUserId = storePrd.UserId,
                        Weight = m.Weight,
                        Unit = storePrd.Unit,
                        UpdateDate = storePrd.UpdateDate.HasValue ? storePrd.UpdateDate.Value : DateTime.MinValue
                    };

                //var _distinctCount = _distinc.Count();
                //get distinct UserProduct Id
                //var _list = _temp.ToList();
                var _distinc = _temp.GroupBy(c => c.ProductId).ToList();

                List<Models.UserProductListCompleteModel2> _listToReturn = new List<UserProductListCompleteModel2>();
                //int _lastId = -1;
                foreach (var _dist in _distinc)
                {

                    UserProductListCompleteTempModel _UserProductListCompleteTempModel = _dist.First();
                    UserProductListCompleteModel2 _UserProductListCompleteModel2New = new UserProductListCompleteModel2
                    {
                        Id = 0,
                        //Id = _UserProductListCompleteTempModel.ProductId,
                        ProductId = _UserProductListCompleteTempModel.ProductId,
                        //Quantity = item.Quantity,
                        Barcode = _UserProductListCompleteTempModel.Barcode,
                        Brand = _UserProductListCompleteTempModel.Brand,
                        //ItemType = "product",
                        Name = _UserProductListCompleteTempModel.Name,
                        Category = _UserProductListCompleteTempModel.Category,
                        Weight = _UserProductListCompleteTempModel.Weight,
                        Price = 0,
                        //PriceList = new List<StoreProduct>(),
                        PriceList = _dist.Select(c => new StoreProduct
                        {
                            Id = c.StoreProductId,
                            StoreId = c.StoreId,
                            Price = c.StorePrice,
                            NeedsUpdate = c.NeedsUpdate.HasValue ? c.NeedsUpdate.Value : true,
                            Url = c.Url,
                            CreatedByUserId = c.CreatedByUserId,
                            UpdateDate = c.UpdateDate,
                            Unit = c.Unit
                        }).ToList(),
                        LastAddedDate = _UserProductListCompleteTempModel.LastAddedDate
                    };
                    _listToReturn.Add(_UserProductListCompleteModel2New);

                }

                return _listToReturn;
            }
        }

        static public List<Models.UserProductListCompleteModel2> GetAllV3(int page, string query, bool withoutBarcode = false, string userId = "")
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                //List<Products> productsList = new List<Products>();
                //IQueryable<Products> productsList = new IQueryable<Products>();
                IQueryable<Products> productsList = Enumerable.Empty<Products>().AsQueryable();
                if (string.IsNullOrEmpty(query))
                {
                    //productsList = db.Products.OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6).ToList();
                    productsList = db.Products.OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6);
                }
                else
                {
                    //IN FUTURE MAYBE
                    if (page > 0)
                    {
                        productsList = db.Products.Where(c => c.Name.ToLower().Contains(query.ToLower()) ||
                            c.Brand.ToLower().Contains(query.ToLower()))
                            .OrderBy(c => c.Name).Skip((page - 1) * 6).Take(6);
                    }
                    else //if -1 return all
                    {
                        var decomposed = query.Normalize(NormalizationForm.FormD);
                        var filtered = decomposed.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
                        var _normalizedQuery = new String(filtered.ToArray());

                        string[] _searchWords = query.ToLower().Trim(' ').Split(' ');
                        string[] _searchWordsNormalized = _normalizedQuery.ToLower().Trim(' ').Split(' ');
                        if (withoutBarcode)
                        {
                            productsList = db.Products.Where(c => _searchWords.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z) &&
                            c.Barcode.Equals("0"))
                            ||
                            _searchWordsNormalized.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z))
                            && c.Barcode.Equals("0"))
                               .OrderBy(c => c.Name);
                        }
                        else
                        {
                            productsList = db.Products.Where(c => _searchWords.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z)) ||
                             _searchWordsNormalized.All(z => (c.Name.ToLower() + " " + c.Brand.ToLower()).Contains(z)))
                                    .OrderBy(c => c.Name);
                            //if (!string.IsNullOrEmpty(userId))
                            //{
                            //    //productsList = productsList.Where(c => !c.IsTemp.HasValue || c.IsTemp.HasValue && c.CreatedByUserId == userId);
                            //    productsList = productsList.Where(c => !c.IsTemp.HasValue || c.IsTemp == false || (c.IsTemp.Value && c.CreatedByUserId == userId));

                            //}
                            //else
                            //{
                            //    productsList = productsList.Where(c => !c.IsTemp.HasValue);

                            //}
                        }
                    }
                }

                //var _temp45 = productsList.ToList();

                var _temp =
                from m in productsList
                join storePrd in db.StoreProducts on m.Id equals storePrd.ProductId
                where (!string.IsNullOrEmpty(userId) ? (!storePrd.IsTemp.HasValue || storePrd.IsTemp == false || (storePrd.IsTemp.Value && storePrd.UserId == userId)) : true)
                //orderby m.Id descending
                select new UserProductListCompleteTempModel
                {
                    Id = storePrd.Id,
                    ProductId = m.Id,
                    //Quantity = m.Quantity.Value,
                    Barcode = m.Barcode,
                    Brand = m.Brand,
                    ItemType = "product",
                    Name = m.Name,
                    Category = m.CategoryString,
                    //Price = 0,
                    StorePrice = Math.Round(storePrd.Price.Value, 2),
                    StoreId = storePrd.StoreId,
                    StoreProductId = storePrd.Id,
                    NeedsUpdate = storePrd.NeedsUpdate,
                    Url = storePrd.Url,
                    CreatedByUserId = m.CreatedByUserId,
                    Weight = m.Weight,
                    Unit = storePrd.Unit,
                    UpdateDate = storePrd.UpdateDate.HasValue ? storePrd.UpdateDate.Value : DateTime.MinValue,
                    ProductIsTemp = m.IsTemp.HasValue ? m.IsTemp.Value : false,
                    StoreProductIsTemp = storePrd.IsTemp.HasValue ? storePrd.IsTemp.Value : false,
                    StoreProductCreatedByUserId = storePrd.UserId
                };

                //var _distinctCount = _distinc.Count();
                //get distinct UserProduct Id
                //var _list = _temp.ToList();
                var _distinc = _temp.GroupBy(c => c.ProductId).ToList();

                List<Models.UserProductListCompleteModel2> _listToReturn = new List<UserProductListCompleteModel2>();
                //int _lastId = -1;
                foreach (var _dist in _distinc)
                {

                    UserProductListCompleteTempModel _UserProductListCompleteTempModel = _dist.First();
                    UserProductListCompleteModel2 _UserProductListCompleteModel2New = new UserProductListCompleteModel2
                    {
                        Id = 0,
                        ProductId = _UserProductListCompleteTempModel.ProductId,
                        Barcode = _UserProductListCompleteTempModel.Barcode,
                        Brand = _UserProductListCompleteTempModel.Brand,
                        Name = _UserProductListCompleteTempModel.Name,
                        Category = _UserProductListCompleteTempModel.Category,
                        Weight = _UserProductListCompleteTempModel.Weight,
                        Price = 0,
                        PriceList = _dist.Select(c => new StoreProduct
                        {
                            Id = c.StoreProductId,
                            StoreId = c.StoreId,
                            Price = c.StorePrice,
                            NeedsUpdate = c.NeedsUpdate.HasValue ? c.NeedsUpdate.Value : true,
                            Url = c.Url,
                            CreatedByUserId = c.StoreProductCreatedByUserId,
                            UpdateDate = c.UpdateDate,
                            Unit = c.Unit,
                            IsTemp = c.StoreProductIsTemp
                        }).ToList(),
                        LastAddedDate = _UserProductListCompleteTempModel.LastAddedDate,
                        IsTemp = _UserProductListCompleteTempModel.ProductIsTemp,
                        CreatedByUserId = _UserProductListCompleteTempModel.CreatedByUserId,
                    };
                    _listToReturn.Add(_UserProductListCompleteModel2New);

                }

                return _listToReturn;
            }
        }

        static public Products AssociateProductWithBarcode(int productId, string barcode)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var product = db.Products.Where(c => c.Id == productId && c.Barcode == "0").FirstOrDefault();
                if (product != null)
                {
                    product.Barcode = barcode;
                    db.SaveChanges();
                    return product;
                }
                else
                    return null;
            }
        }

        //Legacy - old
        async static public Task<(List<ProductPricesUpdates> productPricesUpdates, List<string> details)> UpdatePrices(int productId)
        {

            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                OnlineProducts _OnlineProducts = new OnlineProducts();
                var _storeProducts = db2.StoreProducts.Where(c => c.ProductId == productId).Include(c => c.Stores).ToList();

                List<ProductPricesUpdates> _productPricesUpdate = new List<ProductPricesUpdates>();
                List<string> _productPricesUpdateDetails = new List<string>();



                string _productdUpdatedMessage = string.Empty;

                foreach (var _storeProduct in _storeProducts)
                {
                    //TODO - try to remove this line now that we are with using
                    ((IObjectContextAdapter)db2)
                                  .ObjectContext
                                  .Refresh(RefreshMode.StoreWins, _storeProduct);
                    switch (_storeProduct.StoreId)
                    {
                        case 1:
                            try
                            {
                                LisieStores.Extensibility.ProductSearchResult _jumboProductSearchResult = await _OnlineProducts.GetJumboProductMetadata(_storeProduct.Url);
                                if (_jumboProductSearchResult != null)
                                {
                                    double _newPrice = double.Parse(_jumboProductSearchResult.Price.Replace("€", "").Trim());
                                    if (_storeProduct.Price != _newPrice)
                                    {
                                        ProductPricesUpdates _ProductPricesUpdate = new ProductPricesUpdates
                                        {
                                            OldPrice = _storeProduct.Price.Value,
                                            NewPrice = _newPrice,
                                            CreateDate = DateTime.Now,
                                            CreatedByUserId = "0",
                                            ProductId = productId,
                                            StoreId = _storeProduct.StoreId
                                        };
                                        db2.ProductPricesUpdates.Add(_ProductPricesUpdate);

                                        _productPricesUpdateDetails.Add(_productdUpdatedMessage =
                                           _storeProduct.Stores.Name + " - " +
                                           (_storeProduct.Price > _newPrice ? "Desceu" : "Subiu")
                                           + " de preço, " + _storeProduct.Products.Name);

                                        _productPricesUpdate.Add(_ProductPricesUpdate);

                                        _storeProduct.Price = _newPrice;
                                        _storeProduct.UpdateDate = DateTime.Now;
                                        _storeProduct.NeedsUpdate = false;
                                        db2.SaveChanges();

                                    }
                                    else
                                    {
                                        _storeProduct.Price = _newPrice;
                                        _storeProduct.UpdateDate = DateTime.Now;
                                        _storeProduct.NeedsUpdate = false;
                                        db2.SaveChanges();
                                    }
                                }
                                else
                                {
                                    _storeProduct.UpdateDate = DateTime.Now;
                                    _storeProduct.NeedsUpdate = true;
                                    db2.SaveChanges();
                                }
                                break;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);

                                _storeProduct.UpdateDate = DateTime.Now;
                                _storeProduct.NeedsUpdate = true;
                                db2.SaveChanges();

                                break;
                            }

                        case 2:
                            try
                            {
                                LisieStores.Extensibility.ProductSearchResult _continenteProductSearchResult = await _OnlineProducts.GetContinenteProductMetadata(_storeProduct.Url);
                                if (_continenteProductSearchResult != null)
                                {
                                    double _newPrice = double.Parse(_continenteProductSearchResult.Price.Replace("€", "").Trim());
                                    if (_storeProduct.Price != _newPrice)
                                    {
                                        ProductPricesUpdates _ProductPricesUpdate = new ProductPricesUpdates
                                        {
                                            OldPrice = _storeProduct.Price.Value,
                                            NewPrice = _newPrice,
                                            CreateDate = DateTime.Now,
                                            CreatedByUserId = "0",
                                            ProductId = productId,
                                            StoreId = _storeProduct.StoreId

                                        };
                                        db2.ProductPricesUpdates.Add(_ProductPricesUpdate);

                                        _productPricesUpdateDetails.Add(_productdUpdatedMessage =
                                           _storeProduct.Stores.Name + " - " +
                                           (_storeProduct.Price > _newPrice ? "Desceu" : "Subiu")
                                           + " de preço, " + _storeProduct.Products.Name);
                                        _productPricesUpdate.Add(_ProductPricesUpdate);


                                        _storeProduct.Price = _newPrice;
                                        _storeProduct.UpdateDate = DateTime.Now;
                                        _storeProduct.NeedsUpdate = false;
                                        db2.SaveChanges();
                                    }
                                    else
                                    {
                                        _storeProduct.Price = _newPrice;
                                        _storeProduct.UpdateDate = DateTime.Now;
                                        _storeProduct.NeedsUpdate = false;
                                        db2.SaveChanges();
                                    }
                                }
                                else
                                {
                                    _storeProduct.UpdateDate = DateTime.Now;
                                    _storeProduct.NeedsUpdate = true;
                                    db2.SaveChanges();
                                }
                                break;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);

                                _storeProduct.UpdateDate = DateTime.Now;
                                _storeProduct.NeedsUpdate = true;
                                db2.SaveChanges();

                                break;
                            }
                        case 3:
                            try
                            {
                                LisieStores.Extensibility.ProductSearchResult _pingoDoceProductSearchResult = await _OnlineProducts.GetPingoDoceProductMetadata(_storeProduct.Url);
                                if (_pingoDoceProductSearchResult != null)
                                {
                                    double _newPrice = Math.Round(double.Parse(_pingoDoceProductSearchResult.Price.Replace("€", "").Trim()), 2);
                                    if (_storeProduct.Price != _newPrice)
                                    {
                                        ProductPricesUpdates _ProductPricesUpdate = new ProductPricesUpdates
                                        {
                                            OldPrice = _storeProduct.Price.Value,
                                            NewPrice = _newPrice,
                                            CreateDate = DateTime.Now,
                                            CreatedByUserId = "0",
                                            ProductId = productId,
                                            StoreId = _storeProduct.StoreId
                                        };
                                        db2.ProductPricesUpdates.Add(_ProductPricesUpdate);


                                        _productPricesUpdateDetails.Add(_productdUpdatedMessage =
                                           _storeProduct.Stores.Name + " - " +
                                           (_storeProduct.Price > _newPrice ? "Desceu" : "Subiu")
                                           + " de preço, " + _storeProduct.Products.Name);
                                        _productPricesUpdate.Add(_ProductPricesUpdate);


                                        _storeProduct.Price = _newPrice;
                                        _storeProduct.UpdateDate = DateTime.Now;
                                        _storeProduct.NeedsUpdate = false;
                                        db2.SaveChanges();
                                    }
                                    else
                                    {
                                        _storeProduct.Price = _newPrice;
                                        _storeProduct.UpdateDate = DateTime.Now;
                                        _storeProduct.NeedsUpdate = false;
                                        db2.SaveChanges();
                                    }
                                }
                                else
                                {
                                    _storeProduct.UpdateDate = DateTime.Now;
                                    _storeProduct.NeedsUpdate = true;
                                    db2.SaveChanges();
                                }
                                break;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);

                                _storeProduct.UpdateDate = DateTime.Now;
                                _storeProduct.NeedsUpdate = true;
                                db2.SaveChanges();

                                break;
                            }

                        default:
                            break;
                    }
                }
                return (productPricesUpdates: _productPricesUpdate, details: _productPricesUpdateDetails);
            }

        }

        //BACKUP - this was working good
        //async static public Task<(List<ProductPricesUpdates> productPricesUpdates, List<string> details)> UpdatePricesNew(int productId)
        //{
        //    Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");
        //    Logger.Debug("#UpdatePricesNew RUNNING for ProductID - " + productId, "pricesupdate.txt");
        //    using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
        //    {
        //        OnlineProducts _OnlineProducts = new OnlineProducts();
        //        var _storeProducts = db2.StoreProducts.Where(c => c.ProductId == productId).Include(c => c.Stores).ToList();

        //        List<ProductPricesUpdates> _productPricesUpdate = new List<ProductPricesUpdates>();
        //        List<string> _productPricesUpdateDetails = new List<string>();

        //        string _productdUpdatedMessage = string.Empty;

        //        foreach (var _storeProduct in _storeProducts)
        //        {
        //            //TODO - try to remove this line now that we are with using
        //            ((IObjectContextAdapter)db2)
        //                          .ObjectContext
        //                          .Refresh(RefreshMode.StoreWins, _storeProduct);

        //            try
        //            {
        //                Logger.Debug("storeId - " + _storeProduct.StoreId, "pricesupdate.txt");

        //                //if (_storeProduct.StoreId == 1)
        //                //{
        //                //    //put in new way
        //                //    _storeProduct.Url = _storeProduct.Url.ToLower();
        //                //    if (_storeProduct.Url.IndexOf("/frontoffice") > -1)
        //                //    {
        //                //        _storeProduct.Url = _storeProduct.Url.Replace("/frontoffice", "/pt");
        //                //        _storeProduct.Url = _storeProduct.Url.Replace("_", "-");
        //                //        if (_storeProduct.Url.IndexOf("auchan-amadora") > -1)
        //                //        {
        //                //            _storeProduct.Url = _storeProduct.Url.Substring(0, _storeProduct.Url.LastIndexOf("/"));
        //                //        }
        //                //        _storeProduct.Url += ".html";
        //                //    }
        //                //}

        //                LisieStores.Extensibility.IMarketFetcher _IMarketFetcher = Helpers.Extensibility.GetStoreFetcher(_storeProduct.StoreId);
        //                LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = await _IMarketFetcher.GetProductMetadata(_storeProduct.Url);
        //                Logger.Debug(new JavaScriptSerializer().Serialize(_ProductSearchResult), "pricesupdate.txt");

        //                if (_ProductSearchResult != null && _ProductSearchResult.Price != string.Empty) //Product online found
        //                {
        //                    Logger.Debug("_ProductSearchResult.Price - " + _ProductSearchResult.Price, "pricesupdate.txt");

        //                    //!MPORTANT
        //                    //FOR PRODUCTION
        //                    //double _newPrice = double.Parse(_ProductSearchResult.Price.Replace("€", "").Trim());
        //                    double _newPrice = 0;
        //                    if (HttpContext.Current.Request.IsLocal) //For LOCAL
        //                    {
        //                        _newPrice = double.Parse(_ProductSearchResult.Price.Replace("€", "").Replace(",", ".").Trim());
        //                    }
        //                    else
        //                    {
        //                        _newPrice = double.Parse(_ProductSearchResult.Price.Replace("€", "").Replace(".", ",").Trim());
        //                    }


        //                    //FOR LOCAL
        //                    //double _newPrice = double.Parse(_ProductSearchResult.Price.Replace("€", "").Replace(",", ".").Trim());

        //                    Logger.Debug("_newPrice - " + _newPrice.ToString(), "pricesupdate.txt");

        //                    //if (_storeProduct.Price != _newPrice)
        //                    if (_newPrice != 0 && Math.Round(_storeProduct.Price.Value, 2) != Math.Round(_newPrice, 2))
        //                    {
        //                        //Add to prices update history
        //                        ProductPricesUpdates _ProductPricesUpdate = new ProductPricesUpdates
        //                        {
        //                            OldPrice = _storeProduct.Price.Value,
        //                            NewPrice = _newPrice,
        //                            CreateDate = DateTime.Now,
        //                            CreatedByUserId = "0",
        //                            ProductId = productId,
        //                            StoreId = _storeProduct.StoreId
        //                        };
        //                        db2.ProductPricesUpdates.Add(_ProductPricesUpdate);

        //                        //Add to the return
        //                        _productPricesUpdateDetails.Add(_productdUpdatedMessage =
        //                           _storeProduct.Stores.Name + " - " +
        //                           (_storeProduct.Price > _newPrice ? "Desceu" : "Subiu")
        //                           + " de preço, " + _storeProduct.Products.Name);

        //                        _productPricesUpdate.Add(_ProductPricesUpdate);

        //                        _storeProduct.Price = _newPrice;
        //                        double _priceRatio = 0;
        //                        if (HttpContext.Current.Request.IsLocal) //For LOCAL
        //                        {
        //                            _priceRatio = double.Parse(_ProductSearchResult.PriceWeight.Replace("€", "").Replace(",", ".").Trim());
        //                        }
        //                        else
        //                        {
        //                            _priceRatio = double.Parse(_ProductSearchResult.PriceWeight.Replace("€", "").Replace(".", ",").Trim());
        //                        }
        //                        _storeProduct.PriceRatio = _priceRatio;
        //                        _storeProduct.Unit = _ProductSearchResult.Unit.ToLower();
        //                        _storeProduct.UpdateDate = DateTime.Now;
        //                        _storeProduct.NeedsUpdate = false;

        //                        //if url from store GetMetadata, is different, update it
        //                        if (!string.IsNullOrEmpty(_ProductSearchResult.Url) && _ProductSearchResult.Url != _storeProduct.Url)
        //                            _storeProduct.Url = _ProductSearchResult.Url;
        //                        if (!string.IsNullOrEmpty(_ProductSearchResult.OnlineProductId) && _ProductSearchResult.OnlineProductId != _storeProduct.OnlineProductId)
        //                            _storeProduct.OnlineProductId = _ProductSearchResult.OnlineProductId;
        //                        db2.SaveChanges();

        //                    }
        //                    else //Price is not different, check if url,OnlineProductId, PriceRatio, Unit are different, if they are, update it
        //                    {
        //                        //_storeProduct.Price = _newPrice;
        //                        _storeProduct.UpdateDate = DateTime.Now;
        //                        _storeProduct.NeedsUpdate = false;

        //                        //Url
        //                        if (!string.IsNullOrEmpty(_ProductSearchResult.Url) && _ProductSearchResult.Url != _storeProduct.Url)
        //                            _storeProduct.Url = _ProductSearchResult.Url;
        //                        //OnlineProductId
        //                        if (!string.IsNullOrEmpty(_ProductSearchResult.OnlineProductId) && _storeProduct.OnlineProductId == null) //if OnlineProductId exists and StoreProduct OnlineProductId is null, update it
        //                            _storeProduct.OnlineProductId = _ProductSearchResult.OnlineProductId;
        //                        else if (!string.IsNullOrEmpty(_ProductSearchResult.OnlineProductId) && _storeProduct.OnlineProductId.Trim() != _ProductSearchResult.OnlineProductId.Trim())
        //                            _storeProduct.OnlineProductId = _ProductSearchResult.OnlineProductId;
        //                        //PriceRatio
        //                        double _priceRatio = 0;
        //                        if (HttpContext.Current.Request.IsLocal) //For LOCAL
        //                            _priceRatio = double.Parse(_ProductSearchResult.PriceWeight.Replace("€", "").Replace(",", ".").Trim());
        //                        else
        //                            _priceRatio = double.Parse(_ProductSearchResult.PriceWeight.Replace("€", "").Replace(".", ",").Trim());
        //                        if (!string.IsNullOrEmpty(_ProductSearchResult.PriceWeight) && _storeProduct.PriceRatio == null) //if PriceWeigh exists and StoreProduct PriceRatio is null, update it
        //                            _storeProduct.PriceRatio = _priceRatio;
        //                        else if (!string.IsNullOrEmpty(_ProductSearchResult.PriceWeight) && _storeProduct.PriceRatio.ToString().Trim() != _ProductSearchResult.PriceWeight.Trim())
        //                            _storeProduct.PriceRatio = _priceRatio;
        //                        //Unit
        //                        if (!string.IsNullOrEmpty(_ProductSearchResult.Unit) && _storeProduct.Unit == null) //if Unit exists and StoreProduct Unit is null, update it
        //                            _storeProduct.Unit = _ProductSearchResult.Unit;
        //                        else if (!string.IsNullOrEmpty(_ProductSearchResult.Unit) && _ProductSearchResult.Unit.ToLower().Trim() != _storeProduct.Unit.ToLower().Trim())
        //                            _storeProduct.Unit = _ProductSearchResult.Unit;
        //                        db2.SaveChanges();
        //                    }
        //                }
        //                else
        //                {
        //                    Logger.Debug("ProductSearchResult not filled not found", "pricesupdate.txt");
        //                    //Logger.Debug(new JavaScriptSerializer().Serialize(_ProductSearchResult));

        //                    _storeProduct.UpdateDate = DateTime.Now;
        //                    _storeProduct.NeedsUpdate = true;
        //                    db2.SaveChanges();
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Logger.Debug(ex.Message, "pricesupdate.txt");


        //                Console.WriteLine(ex.Message);

        //                _storeProduct.UpdateDate = DateTime.Now;
        //                _storeProduct.NeedsUpdate = true;
        //                db2.SaveChanges();

        //                break;
        //            }
        //        }
        //        return (productPricesUpdates: _productPricesUpdate, details: _productPricesUpdateDetails);
        //    }

        //}

        async static public Task<(List<ProductPricesUpdates> productPricesUpdates, List<string> details)> UpdatePricesNew(int productId)
        {
            Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");
            //Logger.Debug("#UpdatePricesNew RUNNING for ProductID - " + productId, "pricesupdate.txt");
            using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
            {
                OnlineProducts _OnlineProducts = new OnlineProducts();
                var _storeProducts = db2.StoreProducts.Where(c => c.ProductId == productId).Include(c => c.Stores).ToList();

                List<ProductPricesUpdates> _productPricesUpdate = new List<ProductPricesUpdates>();
                List<string> _productPricesUpdateDetails = new List<string>();

                string _productdUpdatedMessage = string.Empty;
                string _productEanFound = string.Empty;
                foreach (var _storeProduct in _storeProducts)
                {
                    //TODO - try to remove this line now that we are with using
                    ((IObjectContextAdapter)db2)
                                  .ObjectContext
                                  .Refresh(RefreshMode.StoreWins, _storeProduct);

                    try
                    {
                        //Logger.Debug("storeId - " + _storeProduct.StoreId, "pricesupdate.txt");

                        LisieStores.Extensibility.IMarketFetcher _IMarketFetcher = Helpers.Extensibility.GetStoreFetcher(_storeProduct.StoreId);
                        LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = null;
                        if (!string.IsNullOrEmpty(_storeProduct.OnlineProductId)) //first try with id
                        {
                            _ProductSearchResult = await _IMarketFetcher.GetProductMetadataById(_storeProduct.OnlineProductId);
                            if (_ProductSearchResult == null) //if null, try with url
                            {
                                _ProductSearchResult = await _IMarketFetcher.GetProductMetadata(_storeProduct.Url);
                            }
                        }
                        else if (!string.IsNullOrEmpty(_storeProduct.Url)) //try with url
                        {   
                            _ProductSearchResult = await _IMarketFetcher.GetProductMetadata(_storeProduct.Url);
                        }


                        //Logger.Debug(new JavaScriptSerializer().Serialize(_ProductSearchResult), "pricesupdate.txt");

                        if (_ProductSearchResult != null && _ProductSearchResult.Price != string.Empty) //Product online found
                        {
                            Logger.Debug("_ProductSearchResult.Price - " + _ProductSearchResult.Price, "pricesupdate.txt");

                            //check if barcode exists, if it does save it
                            if (_productEanFound == string.Empty && !string.IsNullOrEmpty(_ProductSearchResult.Barcode))
                            {
                                _productEanFound = _ProductSearchResult.Barcode;
                            }

                            //NEW - with culture invariant
                            double _newPrice = TextTools.ParsePrice(_ProductSearchResult.Price);

                            //PRICE IS DIFFERENT, update
                            //Logger.Debug("_newPrice - " + _newPrice.ToString(), "pricesupdate.txt");
                            if (_newPrice != 0 && Math.Round(_storeProduct.Price.Value, 2) != Math.Round(_newPrice, 2))
                            {
                                //Add to prices update history
                                ProductPricesUpdates _ProductPricesUpdate = new ProductPricesUpdates
                                {
                                    OldPrice = _storeProduct.Price.Value,
                                    NewPrice = _newPrice,
                                    CreateDate = DateTime.Now,
                                    CreatedByUserId = "0",
                                    ProductId = productId,
                                    StoreId = _storeProduct.StoreId
                                };
                                db2.ProductPricesUpdates.Add(_ProductPricesUpdate);

                                //Send signalR to /scoreboard/ProductPricesUpdates
                                //Logger.Debug("Sending SignalR", "pricesupdate.txt");
                                Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastStoreProductPriceUpdate("9ff8224f-17cf-49fb-b555-05779a13eb40", _ProductPricesUpdate);

                                //Add to the return
                                _productPricesUpdateDetails.Add(_productdUpdatedMessage =
                                   _storeProduct.Stores.Name + " - " +
                                   (_storeProduct.Price > _newPrice ? "Desceu" : "Subiu")
                                   + " de preço, " + _storeProduct.Products.Name);

                                _productPricesUpdate.Add(_ProductPricesUpdate);

                                //Logger.Debug("Price Ratio", "pricesupdate.txt");
                                _storeProduct.Price = _newPrice;
                                double _priceRatio = 0;
                                if (!string.IsNullOrEmpty(_ProductSearchResult.PriceWeight))
                                {
                                    //NEW - with culture invariant
                                    _priceRatio = TextTools.ParsePrice(_ProductSearchResult.PriceWeight);
                                }
                                //Logger.Debug("Price Ratio ended", "pricesupdate.txt");
                                _storeProduct.PriceRatio = _priceRatio;
                                _storeProduct.Unit = _ProductSearchResult.Unit.ToLower();
                                _storeProduct.UpdateDate = DateTime.Now;
                                _storeProduct.NeedsUpdate = false;

                                //if url from store GetMetadata, is different, update it
                                if (!string.IsNullOrEmpty(_ProductSearchResult.Url) && _ProductSearchResult.Url != _storeProduct.Url)
                                    _storeProduct.Url = _ProductSearchResult.Url;
                                if (!string.IsNullOrEmpty(_ProductSearchResult.OnlineProductId) && _ProductSearchResult.OnlineProductId != _storeProduct.OnlineProductId)
                                    _storeProduct.OnlineProductId = _ProductSearchResult.OnlineProductId;

                                //New fields
                                if (_storeProduct.Name == null && !string.IsNullOrEmpty(_ProductSearchResult.Name))
                                    _storeProduct.Name = _ProductSearchResult.Name;
                                if (_storeProduct.Brand == null && !string.IsNullOrEmpty(_ProductSearchResult.Brand))
                                    _storeProduct.Brand = _ProductSearchResult.Brand;
                                if (_storeProduct.Weight == null && !string.IsNullOrEmpty(_ProductSearchResult.Weight))
                                    _storeProduct.Weight = _ProductSearchResult.Weight;
                                if (_storeProduct.ImageUrl == null && !string.IsNullOrEmpty(_ProductSearchResult.ImageUrl))
                                    _storeProduct.ImageUrl = _ProductSearchResult.ImageUrl;
                                if (_storeProduct.Barcode == null && !string.IsNullOrEmpty(_ProductSearchResult.Barcode))
                                    _storeProduct.Barcode = _ProductSearchResult.Barcode;

                                //Category
                                if ((_storeProduct.Category == null || string.IsNullOrEmpty(_ProductSearchResult.Category)) && !string.IsNullOrEmpty(_ProductSearchResult.Category))
                                    _storeProduct.Category = _ProductSearchResult.Category;
                                if ((_storeProduct.CategoryFull == null || string.IsNullOrEmpty(_ProductSearchResult.FullCategory)) && !string.IsNullOrEmpty(_ProductSearchResult.FullCategory))
                                    _storeProduct.CategoryFull = _ProductSearchResult.FullCategory;

                                _storeProduct.LastSuccessfulUpdateDate = DateTime.Now;
                                db2.SaveChanges();

                            }
                            else //Price is not different, check if url,OnlineProductId, PriceRatio, Unit are different, if they are, update it
                            {
                                //_storeProduct.Price = _newPrice;
                                _storeProduct.UpdateDate = DateTime.Now;
                                _storeProduct.NeedsUpdate = false;

                                //Url
                                if (!string.IsNullOrEmpty(_ProductSearchResult.Url) && _ProductSearchResult.Url != _storeProduct.Url)
                                    _storeProduct.Url = _ProductSearchResult.Url;
                                //OnlineProductId
                                if (!string.IsNullOrEmpty(_ProductSearchResult.OnlineProductId) && _storeProduct.OnlineProductId == null) //if OnlineProductId exists and StoreProduct OnlineProductId is null, update it
                                    _storeProduct.OnlineProductId = _ProductSearchResult.OnlineProductId;
                                else if (!string.IsNullOrEmpty(_ProductSearchResult.OnlineProductId) && _storeProduct.OnlineProductId.Trim() != _ProductSearchResult.OnlineProductId.Trim())
                                    _storeProduct.OnlineProductId = _ProductSearchResult.OnlineProductId;
                                //PriceRatio
                                double _priceRatio = 0;
                                if (!string.IsNullOrEmpty(_ProductSearchResult.PriceWeight))
                                {
                                    //NEW - with culture invariant
                                    _priceRatio = TextTools.ParsePrice(_ProductSearchResult.PriceWeight);

                                    if (!string.IsNullOrEmpty(_ProductSearchResult.PriceWeight) && _storeProduct.PriceRatio == null) //if PriceWeigh exists and StoreProduct PriceRatio is null, update it
                                        _storeProduct.PriceRatio = _priceRatio;
                                    else if (!string.IsNullOrEmpty(_ProductSearchResult.PriceWeight) && _storeProduct.PriceRatio.ToString().Trim() != _ProductSearchResult.PriceWeight.Trim())
                                        _storeProduct.PriceRatio = _priceRatio;
                                }
                                //Unit
                                if (!string.IsNullOrEmpty(_ProductSearchResult.Unit) && _storeProduct.Unit == null) //if Unit exists and StoreProduct Unit is null, update it
                                    _storeProduct.Unit = _ProductSearchResult.Unit;
                                else if (!string.IsNullOrEmpty(_ProductSearchResult.Unit) && _ProductSearchResult.Unit.ToLower().Trim() != _storeProduct.Unit.ToLower().Trim())
                                    _storeProduct.Unit = _ProductSearchResult.Unit;

                                //New fields
                                if (_storeProduct.Name == null && !string.IsNullOrEmpty(_ProductSearchResult.Name))
                                    _storeProduct.Name = _ProductSearchResult.Name;
                                if (_storeProduct.Brand == null && !string.IsNullOrEmpty(_ProductSearchResult.Brand))
                                    _storeProduct.Brand = _ProductSearchResult.Brand;
                                if (_storeProduct.Weight == null && !string.IsNullOrEmpty(_ProductSearchResult.Weight))
                                    _storeProduct.Weight = _ProductSearchResult.Weight;
                                if (_storeProduct.ImageUrl == null && !string.IsNullOrEmpty(_ProductSearchResult.ImageUrl))
                                    _storeProduct.ImageUrl = _ProductSearchResult.ImageUrl;
                                if (_storeProduct.Barcode == null && !string.IsNullOrEmpty(_ProductSearchResult.Barcode))
                                    _storeProduct.Barcode = _ProductSearchResult.Barcode;

                                //Category
                                if ((_storeProduct.Category == null || string.IsNullOrEmpty(_ProductSearchResult.Category)) && !string.IsNullOrEmpty(_ProductSearchResult.Category))
                                    _storeProduct.Category = _ProductSearchResult.Category;
                                if ((_storeProduct.CategoryFull == null || string.IsNullOrEmpty(_ProductSearchResult.FullCategory)) && !string.IsNullOrEmpty(_ProductSearchResult.FullCategory))
                                    _storeProduct.CategoryFull = _ProductSearchResult.FullCategory;

                                _storeProduct.LastSuccessfulUpdateDate = DateTime.Now;
                                db2.SaveChanges();
                            }
                        }
                        else
                        {
                            //Logger.Debug("ProductSearchResult not filled not found", "pricesupdate.txt");
                            //Logger.Debug(new JavaScriptSerializer().Serialize(_ProductSearchResult));
                            //Add to ProductPricesUpdatesFails.
                            ProductPricesUpdatesFails _newProductPricesUpdatesFails = new ProductPricesUpdatesFails
                            {
                                StoreProductId = _storeProduct.Id,
                                CreateDate = DateTime.Now,
                                ProductId = productId,
                                StoreId = _storeProduct.StoreId
                            };
                            db2.ProductPricesUpdatesFails.Add(_newProductPricesUpdatesFails);

                            _storeProduct.UpdateDate = DateTime.Now;
                            _storeProduct.NeedsUpdate = true;
                            db2.SaveChanges();
                        }

                        Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastStoreProductUpdate("9ff8224f-17cf-49fb-b555-05779a13eb40", _storeProduct);
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug(ex.Message, "pricesupdate.txt");

                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);

                        //Add to ProductPricesUpdatesFails.
                        ProductPricesUpdatesFails _newProductPricesUpdatesFails = new ProductPricesUpdatesFails
                        {
                            StoreProductId = _storeProduct.Id,
                            CreateDate = DateTime.Now,
                            ProductId = productId,
                            StoreId = _storeProduct.StoreId
                        };
                        db2.ProductPricesUpdatesFails.Add(_newProductPricesUpdatesFails);

                        _storeProduct.UpdateDate = DateTime.Now;
                        _storeProduct.NeedsUpdate = true;
                        db2.SaveChanges();

                        Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastStoreProductUpdate("9ff8224f-17cf-49fb-b555-05779a13eb40", _storeProduct);

                        break;
                    }
                }

                //if found EAN in product metadata's, check if product EAN is 0, if it is update with newly found barcode
                if (_productEanFound != string.Empty)
                {
                    Products _product = db.Products.Include("StoreProducts").Where(c => c.Id == productId).FirstOrDefault();
                    if (_product != null && _product.Barcode == "0")
                    {
                        var _productWithBarcodeAlreadyExists = db.Products.Include("StoreProducts").Where(c => c.Barcode == _productEanFound).FirstOrDefault();
                        if (_productWithBarcodeAlreadyExists == null)
                        {
                            _product.Barcode = _productEanFound;
                            db.Entry(_product).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            //product already exists, delete _product
                            //merge both StoreProducts to product with barcode already
                            foreach (var _oldStoreProduct in _product.StoreProducts)
                            {
                                //if it doesen´t exists in _productWithBarcodeAlreadyExists, create it
                                if (!_productWithBarcodeAlreadyExists.StoreProducts.Where(c => c.StoreId == _oldStoreProduct.StoreId).Any())
                                {
                                    StoreProducts _StoreProductsNew = new StoreProducts
                                    {
                                        CreateDate = _oldStoreProduct.CreateDate,
                                        NeedsUpdate = _oldStoreProduct.NeedsUpdate,
                                        OnlineProductId = _oldStoreProduct.OnlineProductId,
                                        Price = _oldStoreProduct.Price,
                                        PriceRatio = _oldStoreProduct.PriceRatio,
                                        ProductId = _productWithBarcodeAlreadyExists.Id,
                                        StoreId = _oldStoreProduct.StoreId,
                                        Unit = _oldStoreProduct.Unit,
                                        UpdateDate = _oldStoreProduct.UpdateDate,
                                        Url = _oldStoreProduct.Url,
                                        UserId = _oldStoreProduct.UserId
                                    };
                                    db.StoreProducts.Add(_StoreProductsNew);
                                    db.SaveChanges();
                                }
                            }
                            DeleteSafely(_product.Id);
                        }
                    }
                }

                return (productPricesUpdates: _productPricesUpdate, details: _productPricesUpdateDetails);
            }

        }

        //Newer, testing double.parse with invariante culture - TO DELETE
        //async static public Task<(List<ProductPricesUpdates> productPricesUpdates, List<string> details)> UpdatePricesNew2(int productId)
        //{
        //    Logger.FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs");
        //    //Logger.Debug("#UpdatePricesNew RUNNING for ProductID - " + productId, "pricesupdate.txt");
        //    using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
        //    {
        //        OnlineProducts _OnlineProducts = new OnlineProducts();
        //        var _storeProducts = db2.StoreProducts.Where(c => c.ProductId == productId).Include(c => c.Stores).ToList();

        //        List<ProductPricesUpdates> _productPricesUpdate = new List<ProductPricesUpdates>();
        //        List<string> _productPricesUpdateDetails = new List<string>();

        //        string _productdUpdatedMessage = string.Empty;
        //        string _productEanFound = string.Empty;
        //        foreach (var _storeProduct in _storeProducts)
        //        {
        //            //TODO - try to remove this line now that we are with using
        //            ((IObjectContextAdapter)db2)
        //                          .ObjectContext
        //                          .Refresh(RefreshMode.StoreWins, _storeProduct);

        //            try
        //            {
        //                //Logger.Debug("storeId - " + _storeProduct.StoreId, "pricesupdate.txt");

        //                LisieStores.Extensibility.IMarketFetcher _IMarketFetcher = Helpers.Extensibility.GetStoreFetcher(_storeProduct.StoreId);
        //                LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = null;
        //                if (!string.IsNullOrEmpty(_storeProduct.OnlineProductId)) //first try with id
        //                {
        //                    _ProductSearchResult = await _IMarketFetcher.GetProductMetadataById(_storeProduct.OnlineProductId);
        //                    if (_ProductSearchResult == null) //if null, try with url
        //                    {
        //                        _ProductSearchResult = await _IMarketFetcher.GetProductMetadata(_storeProduct.Url);
        //                    }
        //                }
        //                else if (!string.IsNullOrEmpty(_storeProduct.Url)) //try with url
        //                {
        //                    _ProductSearchResult = await _IMarketFetcher.GetProductMetadata(_storeProduct.Url);
        //                }


        //                //Logger.Debug(new JavaScriptSerializer().Serialize(_ProductSearchResult), "pricesupdate.txt");

        //                if (_ProductSearchResult != null && _ProductSearchResult.Price != string.Empty) //Product online found
        //                {
        //                    //Logger.Debug("_ProductSearchResult.Price - " + _ProductSearchResult.Price, "pricesupdate.txt");

        //                    //check if barcode exists, if it does save it
        //                    if (_productEanFound == string.Empty && !string.IsNullOrEmpty(_ProductSearchResult.Barcode))
        //                    {
        //                        _productEanFound = _ProductSearchResult.Barcode;
        //                    }

        //                    //!MPORTANT
        //                    double _newPrice = TextTools.ParsePrice(_ProductSearchResult.Price);

        //                    //PRICE IS DIFFERENT, update
        //                    //Logger.Debug("_newPrice - " + _newPrice.ToString(), "pricesupdate.txt");
        //                    if (_newPrice != 0 && Math.Round(_storeProduct.Price.Value, 2) != Math.Round(_newPrice, 2))
        //                    {
        //                        //Add to prices update history
        //                        ProductPricesUpdates _ProductPricesUpdate = new ProductPricesUpdates
        //                        {
        //                            OldPrice = _storeProduct.Price.Value,
        //                            NewPrice = _newPrice,
        //                            CreateDate = DateTime.Now,
        //                            CreatedByUserId = "0",
        //                            ProductId = productId,
        //                            StoreId = _storeProduct.StoreId
        //                        };
        //                        db2.ProductPricesUpdates.Add(_ProductPricesUpdate);

        //                        //Send signalR to /scoreboard/ProductPricesUpdates
        //                        //Logger.Debug("Sending SignalR", "pricesupdate.txt");
        //                        Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastStoreProductPriceUpdate("9ff8224f-17cf-49fb-b555-05779a13eb40", _ProductPricesUpdate);

        //                        //Add to the return
        //                        _productPricesUpdateDetails.Add(_productdUpdatedMessage =
        //                           _storeProduct.Stores.Name + " - " +
        //                           (_storeProduct.Price > _newPrice ? "Desceu" : "Subiu")
        //                           + " de preço, " + _storeProduct.Products.Name);

        //                        _productPricesUpdate.Add(_ProductPricesUpdate);

        //                        //Logger.Debug("Price Ratio", "pricesupdate.txt");
        //                        _storeProduct.Price = _newPrice;
        //                        double _priceRatio = 0;
        //                        if (!string.IsNullOrEmpty(_ProductSearchResult.PriceWeight))
        //                        {
        //                            //if (HttpContext.Current.Request.IsLocal) //For LOCAL
        //                            //{
        //                            //    Logger.Debug("IsLocal", "pricesupdate.txt");
        //                            //    _priceRatio = TextTools.ParsePriceLocal(_ProductSearchResult.PriceWeight);
        //                            //    //_priceRatio = double.Parse(_ProductSearchResult.PriceWeight.Replace("€", "").Replace(",", ".").Trim());
        //                            //}
        //                            //else
        //                            //{
        //                            //    Logger.Debug("IsProd", "pricesupdate.txt");
        //                            //    Logger.Debug("_ProductSearchResult.PriceWeight - " + _ProductSearchResult.PriceWeight, "pricesupdate.txt");
        //                            //    _priceRatio = TextTools.ParsePriceProduction(_ProductSearchResult.PriceWeight);
        //                            //    //_priceRatio = double.Parse(_ProductSearchResult.PriceWeight.Replace("€", "").Replace(".", ",").Trim());
        //                            //}
        //                            _priceRatio = TextTools.ParsePrice(_ProductSearchResult.PriceWeight);
        //                        }
        //                        //Logger.Debug("Price Ratio ended", "pricesupdate.txt");
        //                        _storeProduct.PriceRatio = _priceRatio;
        //                        _storeProduct.Unit = _ProductSearchResult.Unit.ToLower();
        //                        _storeProduct.UpdateDate = DateTime.Now;
        //                        _storeProduct.NeedsUpdate = false;

        //                        //if url from store GetMetadata, is different, update it
        //                        if (!string.IsNullOrEmpty(_ProductSearchResult.Url) && _ProductSearchResult.Url != _storeProduct.Url)
        //                            _storeProduct.Url = _ProductSearchResult.Url;
        //                        if (!string.IsNullOrEmpty(_ProductSearchResult.OnlineProductId) && _ProductSearchResult.OnlineProductId != _storeProduct.OnlineProductId)
        //                            _storeProduct.OnlineProductId = _ProductSearchResult.OnlineProductId;

        //                        //New fields
        //                        if (_storeProduct.Name == null && !string.IsNullOrEmpty(_ProductSearchResult.Name))
        //                            _storeProduct.Name = _ProductSearchResult.Name;
        //                        if (_storeProduct.Brand == null && !string.IsNullOrEmpty(_ProductSearchResult.Brand))
        //                            _storeProduct.Brand = _ProductSearchResult.Brand;
        //                        if (_storeProduct.Weight == null && !string.IsNullOrEmpty(_ProductSearchResult.Weight))
        //                            _storeProduct.Weight = _ProductSearchResult.Weight;
        //                        if (_storeProduct.ImageUrl == null && !string.IsNullOrEmpty(_ProductSearchResult.ImageUrl))
        //                            _storeProduct.ImageUrl = _ProductSearchResult.ImageUrl;

        //                        db2.SaveChanges();

        //                    }
        //                    else //Price is not different, check if url,OnlineProductId, PriceRatio, Unit are different, if they are, update it
        //                    {
        //                        //_storeProduct.Price = _newPrice;
        //                        _storeProduct.UpdateDate = DateTime.Now;
        //                        _storeProduct.NeedsUpdate = false;

        //                        //Url
        //                        if (!string.IsNullOrEmpty(_ProductSearchResult.Url) && _ProductSearchResult.Url != _storeProduct.Url)
        //                            _storeProduct.Url = _ProductSearchResult.Url;
        //                        //OnlineProductId
        //                        if (!string.IsNullOrEmpty(_ProductSearchResult.OnlineProductId) && _storeProduct.OnlineProductId == null) //if OnlineProductId exists and StoreProduct OnlineProductId is null, update it
        //                            _storeProduct.OnlineProductId = _ProductSearchResult.OnlineProductId;
        //                        else if (!string.IsNullOrEmpty(_ProductSearchResult.OnlineProductId) && _storeProduct.OnlineProductId.Trim() != _ProductSearchResult.OnlineProductId.Trim())
        //                            _storeProduct.OnlineProductId = _ProductSearchResult.OnlineProductId;
        //                        //PriceRatio
        //                        double _priceRatio = 0;
        //                        if (!string.IsNullOrEmpty(_ProductSearchResult.PriceWeight))
        //                        {
        //                            //if (HttpContext.Current.Request.IsLocal) //For LOCAL
        //                            //                                         //_priceRatio = double.Parse(_ProductSearchResult.PriceWeight.Replace("€", "").Replace(",", ".").Trim());
        //                            //    _priceRatio = TextTools.ParsePriceLocal(_ProductSearchResult.PriceWeight);
        //                            //else
        //                            //    //_priceRatio = double.Parse(_ProductSearchResult.PriceWeight.Replace("€", "").Replace(".", ",").Trim());
        //                            //    _priceRatio = TextTools.ParsePriceProduction(_ProductSearchResult.PriceWeight);
        //                            _priceRatio = TextTools.ParsePrice(_ProductSearchResult.PriceWeight);

        //                            if (!string.IsNullOrEmpty(_ProductSearchResult.PriceWeight) && _storeProduct.PriceRatio == null) //if PriceWeigh exists and StoreProduct PriceRatio is null, update it
        //                                _storeProduct.PriceRatio = _priceRatio;
        //                            else if (!string.IsNullOrEmpty(_ProductSearchResult.PriceWeight) && _storeProduct.PriceRatio.ToString().Trim() != _ProductSearchResult.PriceWeight.Trim())
        //                                _storeProduct.PriceRatio = _priceRatio;
        //                        }
        //                        //Unit
        //                        if (!string.IsNullOrEmpty(_ProductSearchResult.Unit) && _storeProduct.Unit == null) //if Unit exists and StoreProduct Unit is null, update it
        //                            _storeProduct.Unit = _ProductSearchResult.Unit;
        //                        else if (!string.IsNullOrEmpty(_ProductSearchResult.Unit) && _ProductSearchResult.Unit.ToLower().Trim() != _storeProduct.Unit.ToLower().Trim())
        //                            _storeProduct.Unit = _ProductSearchResult.Unit;

        //                        //New fields
        //                        if (_storeProduct.Name == null && !string.IsNullOrEmpty(_ProductSearchResult.Name))
        //                            _storeProduct.Name = _ProductSearchResult.Name;
        //                        if (_storeProduct.Brand == null && !string.IsNullOrEmpty(_ProductSearchResult.Brand))
        //                            _storeProduct.Brand = _ProductSearchResult.Brand;
        //                        if (_storeProduct.Weight == null && !string.IsNullOrEmpty(_ProductSearchResult.Weight))
        //                            _storeProduct.Weight = _ProductSearchResult.Weight;
        //                        if (_storeProduct.ImageUrl == null && !string.IsNullOrEmpty(_ProductSearchResult.ImageUrl))
        //                            _storeProduct.ImageUrl = _ProductSearchResult.ImageUrl;

        //                        db2.SaveChanges();
        //                    }
        //                }
        //                else
        //                {
        //                    //Logger.Debug("ProductSearchResult not filled not found", "pricesupdate.txt");
        //                    //Logger.Debug(new JavaScriptSerializer().Serialize(_ProductSearchResult));

        //                    _storeProduct.UpdateDate = DateTime.Now;
        //                    _storeProduct.NeedsUpdate = true;
        //                    db2.SaveChanges();
        //                }

        //                Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastStoreProductUpdate("9ff8224f-17cf-49fb-b555-05779a13eb40", _storeProduct);
        //            }
        //            catch (Exception ex)
        //            {
        //                Logger.Debug(ex.Message, "pricesupdate.txt");

        //                Console.WriteLine(ex.Message);
        //                Console.WriteLine(ex.StackTrace);

        //                _storeProduct.UpdateDate = DateTime.Now;
        //                _storeProduct.NeedsUpdate = true;
        //                db2.SaveChanges();

        //                Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastStoreProductUpdate("9ff8224f-17cf-49fb-b555-05779a13eb40", _storeProduct);

        //                break;
        //            }
        //        }

        //        return (productPricesUpdates: _productPricesUpdate, details: _productPricesUpdateDetails);
        //    }

        //}

        static public List<Models.StoreProduct> GetPricesListOfProduct(int productId, int quantity, string userId = "")
        {
            List<Models.StoreProduct> _storeProducts = new List<Models.StoreProduct>();
            if (string.IsNullOrEmpty(userId))
            {
                _storeProducts = db.StoreProducts.Where(c => c.ProductId == productId).Select(c => new Models.StoreProduct
                {
                    Id = c.Id,
                    CreatedByUserId = c.UserId,
                    NeedsUpdate = c.NeedsUpdate.HasValue ? c.NeedsUpdate.Value : false,
                    StoreId = c.StoreId,
                    Price = Math.Round(c.Price.Value * quantity, 2),
                    PriceBase = c.Price.Value,
                    Url = c.Url,
                    OnlineProductId = c.OnlineProductId,
                    Brand = c.Brand,
                    Name = c.Name,
                    Unit = c.Unit,
                    PriceRatio = c.PriceRatio.HasValue ? c.PriceRatio.Value : 0,
                    PriceRatioBase = c.PriceRatio.HasValue ? c.PriceRatio.Value : 0,
                    UpdateDate = c.UpdateDate.HasValue ? c.UpdateDate.Value : DateTime.MinValue,
                    Weight = c.Weight

                }).ToList();
            }
            else
            {
                _storeProducts = db.StoreProducts.Where(c =>
                c.ProductId == productId &&
                (!c.IsTemp.HasValue || c.IsTemp == false || (c.IsTemp.Value && c.UserId == userId))
                ).Select(c => new Models.StoreProduct
                {
                    Id = c.Id,
                    CreatedByUserId = c.UserId,
                    NeedsUpdate = c.NeedsUpdate.HasValue ? c.NeedsUpdate.Value : false,
                    StoreId = c.StoreId,
                    Price = Math.Round(c.Price.Value * quantity, 2),
                    PriceBase = c.Price.Value,
                    Url = c.Url,
                    OnlineProductId = c.OnlineProductId,
                    Brand = c.Brand,
                    Name = c.Name,
                    Unit = c.Unit,
                    PriceRatio = c.PriceRatio.HasValue ? c.PriceRatio.Value : 0,
                    PriceRatioBase = c.PriceRatio.HasValue ? c.PriceRatio.Value : 0,
                    UpdateDate = c.UpdateDate.HasValue ? c.UpdateDate.Value : DateTime.MinValue,
                    Weight = c.Weight
                }).ToList();
            }
            return _storeProducts;
        }

        static public int CheckIfProductExistsInStores(List<LisieStores.Extensibility.ProductSearchResult> onlineStoreProducts)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                foreach (var _onlineStoreProduct in onlineStoreProducts)
                {
                    StoreProducts _storeProductExists = null;
                    if (_onlineStoreProduct.OnlineProductId == null)
                    {
                        _storeProductExists = db.StoreProducts.Where(c =>
                        c.StoreId == _onlineStoreProduct.StoreId
                        &&
                        (c.Url == _onlineStoreProduct.Url || c.Url.StartsWith(_onlineStoreProduct.Url)))
                        .FirstOrDefault();
                    }
                    else
                    {
                        _storeProductExists = db.StoreProducts.Where(c =>
                        c.StoreId == _onlineStoreProduct.StoreId
                        &&
                        (c.Url == _onlineStoreProduct.Url
                        || c.Url.StartsWith(_onlineStoreProduct.Url)
                        || c.OnlineProductId == _onlineStoreProduct.OnlineProductId))
                        .FirstOrDefault();
                    }

                    if (_storeProductExists != null) return _storeProductExists.ProductId.Value;
                }
                return -1;
            }
        }

        static public int CheckIfProductExistsInStore(string url)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _storeProductExists = db.StoreProducts.Where(c => c.Url == url || c.Url.StartsWith(url)).FirstOrDefault();
                if (_storeProductExists != null) return _storeProductExists.ProductId.Value;
                else return -1;
            }
        }

        static public int CheckIfProductExistsInStore(int storeId, string url)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _storeProductExists = db.StoreProducts.Where(c => c.StoreId == storeId
                && (c.Url == url
                //|| c.Url.StartsWith(url)
                )).FirstOrDefault();
                if (_storeProductExists != null) return _storeProductExists.ProductId.Value;
                else return -1;
            }
        }

        static public int CheckIfProductExistsInStore(int storeId, string onlineProductId, string url)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _storeProductExists = db.StoreProducts.Where(c => c.StoreId == storeId
                && ((!string.IsNullOrEmpty(url) ? c.Url == url : false)
                //|| c.Url.StartsWith(url) , was fucking things up
                || (!string.IsNullOrEmpty(onlineProductId) ? c.OnlineProductId == onlineProductId : false)
                )).FirstOrDefault();
                if (_storeProductExists != null) return _storeProductExists.ProductId.Value;
                else return -1;
            }
        }

        static public bool CheckIfProductExistsInStore(int productId, int storeId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                return db.StoreProducts.Any(c => c.ProductId.Equals(productId) && c.StoreId.Equals(storeId));
            }
        }

        static public Products UpdateProductCategory(int productId, string category, string fullCategory)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _productFound = db.Products.Where(c => c.Id.Equals(productId)).FirstOrDefault();
                    if (_productFound != null)
                    {
                        _productFound.CategoryString = category;
                        _productFound.FullCategory = fullCategory;
                        db.SaveChanges();
                        return _productFound;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        static public Products UpdateProductBarcode(int productId, string newBarcode)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _productFound = db.Products.Where(c => c.Id.Equals(productId)).FirstOrDefault();
                    if (_productFound != null)
                    {
                        _productFound.Barcode = newBarcode;
                        db.SaveChanges();
                        return _productFound;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        static public async Task<Products> UpdateMetadata(int productId, string userId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                Products _product = db.Products.Where(c => c.Id == productId).FirstOrDefault();
                if (_product != null)
                {
                    var _storeProducts = db.StoreProducts.Where(c => c.ProductId == productId).Include(c => c.Stores);

                    List<int> _orderStoreFetching = new List<int>();
                    _orderStoreFetching.Add(1); //auchan
                    _orderStoreFetching.Add(2); //continente
                    _orderStoreFetching.Add(4); //intermarche
                    _orderStoreFetching.Add(3); //pingo doce
                    _orderStoreFetching.Add(5); //mini preço

                    //List<LisieStores.Extensibility.ProductSearchResult> _searchResults = new List<LisieStores.Extensibility.ProductSearchResult>();
                    foreach (var _storeFetching in _orderStoreFetching)
                    {
                        var _storeProduct = _storeProducts.Where(c => c.StoreId == _storeFetching).FirstOrDefault();
                        if (_storeProduct != null) //found storeProduct, now fetch searchResult
                        {
                            LisieStores.Extensibility.IMarketFetcher _IMarketFetcher = Helpers.Extensibility.GetStoreFetcher(_storeProduct.StoreId);
                            LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = await _IMarketFetcher.GetProductMetadata(_storeProduct.Url);
                            //_searchResults.Add(_ProductSearchResult);
                            if (_ProductSearchResult != null)
                            {
                                Products _newProduct = GetProductOfSearchResult(_ProductSearchResult);
                                if (_newProduct != null)
                                {
                                    _product.Name = _newProduct.Name;
                                    _product.Brand = _newProduct.Brand;
                                    _product.Weight = _newProduct.Weight;
                                    _product.Picture = _newProduct.Picture;
                                    _product.CategoryString = _newProduct.CategoryString;
                                    _product.FullCategory = _newProduct.CategoryString;
                                    //TODO - in close future
                                    //_product.UpdateDate = DateTime.Now;
                                    db.SaveChanges(); //save product with new metadada
                                    return _product;
                                }
                            }
                        }
                    }
                    return _product;
                }
                return null;
            }

        }

        static public Products GetProductOfSearchResult(LisieStores.Extensibility.ProductSearchResult searchResult)
        {
            string _AppDataPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/tempimg.temp");

            if (searchResult != null)
            {
                Products _newProduct = new Products();
                _newProduct.Barcode = searchResult.Barcode;
                _newProduct.Name = searchResult.Name;
                _newProduct.Price = 0;
                _newProduct.VariableWeightPrice = searchResult.PriceWeight;
                _newProduct.CategoryString = searchResult.Category;
                _newProduct.FullCategory = searchResult.FullCategory;
                _newProduct.Brand = searchResult.Brand;
                _newProduct.Weight = searchResult.Weight;
                _newProduct.InsertDate = DateTime.Now;

                //Get image to base64
                WebClient _client = new WebClient();
                _client.DownloadFile(new Uri(searchResult.ImageUrl.Replace("https://", "http://")), _AppDataPath);
                byte[] _imageInBase64 = ManageImage.GetBase64OfImagePath(_AppDataPath);
                _newProduct.Picture = _imageInBase64;

                return _newProduct;
            }
            return null;
        }

        static public Products GetProductOfFirstSelectedStoreId(int FirstAddedProductFromStoreId, List<LisieStores.Extensibility.ProductSearchResult> SelectedResults, string barcode, string userId)
        {
            string _AppDataPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/tempimg.temp");

            LisieStores.Extensibility.ProductSearchResult _productSearchResult = SelectedResults.Where(c => c.StoreId == FirstAddedProductFromStoreId).FirstOrDefault();
            if (_productSearchResult != null)
            {
                Products _newProduct = new Products();
                _newProduct.Name = _productSearchResult.Name;
                _newProduct.Price = 0;
                //_newProduct.Price = Math.Round(double.Parse(_productSearchResult.Price.Replace("€", "").Trim()), 2);
                //double _Price = Math.Round(double.Parse(_productSearchResult.Price.Replace("€", "").Trim()), 2);
                //_newProduct.Price = double.Parse(_productSearchResult.Price.Replace("€", "").Trim());
                _newProduct.VariableWeightPrice = _productSearchResult.PriceWeight;
                _newProduct.CategoryString = _productSearchResult.Category;
                _newProduct.FullCategory = _productSearchResult.FullCategory;
                _newProduct.Brand = _productSearchResult.Brand;
                _newProduct.Weight = _productSearchResult.Weight;
                _newProduct.Barcode = barcode;
                _newProduct.CreatedByUserId = userId;
                _newProduct.InsertDate = DateTime.Now;

                //Get image to base64
                WebClient _client = new WebClient();
                _client.DownloadFile(new Uri(_productSearchResult.ImageUrl.Replace("https://", "http://")), _AppDataPath);
                byte[] _imageInBase64 = ManageImage.GetBase64OfImagePath(_AppDataPath);
                _newProduct.Picture = _imageInBase64;

                return _newProduct;
            }
            return null;
        }

        static public List<StoreProducts> GetStoreProductsCopy(ICollection<StoreProducts> storeProducts, string userId = "")
        {
            List<StoreProducts> copy = new List<StoreProducts>();
            if (!string.IsNullOrEmpty(userId))
            {
                storeProducts = storeProducts.Where(c => !c.IsTemp.HasValue || c.IsTemp == false || (c.IsTemp.Value && c.UserId == userId)).ToList();
            }
            foreach (StoreProducts item in storeProducts)
            {
                copy.Add(new StoreProducts
                {
                    Id = item.Id,
                    CreateDate = item.CreateDate,
                    Price = item.Price,
                    ProductId = item.ProductId,
                    StoreId = item.StoreId,
                    Url = Helpers.Extensibility.GetStoreFetcher(item.StoreId).GetProductViewableUrl("", item.Url),
                    UserId = item.UserId,
                    NeedsUpdate = item.NeedsUpdate,
                    OnlineProductId = item.OnlineProductId,
                    PriceRatio = item.PriceRatio,
                    Unit = item.Unit,
                    UpdateDate = item.UpdateDate,
                    Name = item.Name,
                    Brand = item.Brand,
                    Weight = item.Weight,
                    ImageUrl = item.ImageUrl,
                    IsTemp = item.IsTemp,
                    Barcode = item.Barcode,
                    Category = item.Category,
                    CategoryFull = item.CategoryFull,
                    LastSuccessfulUpdateDate = item.LastSuccessfulUpdateDate,
                });
            }
            return copy;
        }

        static async public Task<int> Create(ProductItemNew product) //returns new userProductId
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                int _userProductId = -1;
                try
                {
                    //Check if product already exists
                    var _productFound = (product.IsToOverwrite && product.ProductId != -1) ?
                        db.Products.Where(c => c.Id == product.ProductId).FirstOrDefault() :
                        GetByBarcode(product.Barcode);

                    //See if exists product with StoreUrl (AVOID DUPLICATES)
                    //TODO - Add Barcoes Table , to associate new barcodes to existing products
                    int _productExistsInStoresId = CheckIfProductExistsInStores(product.SelectedResults);
                    if (_productExistsInStoresId != -1)
                    {
                        return -1;
                    }

                    //Products _newProduct = this.GetOptimizedProductInfoNew(product, product.SelectedResults);
                    //Products _newProduct = GetProductOfFirstSelectedStoreId(
                    //    product.FirstAddedProductFromStoreId,
                    //    product.SelectedResults,
                    //    (!string.IsNullOrEmpty(product.Barcode) ? product.Barcode : "0"),
                    //    product.UserId);
                    LisieStores.Extensibility.IMarketFetcher _IMarketFetcher = Helpers.Extensibility.GetStoreFetcher(product.FirstAddedProductFromStoreId);
                    var _firstSelectedStoreProductResult = product.SelectedResults.Where(c => c.StoreId == product.FirstAddedProductFromStoreId).FirstOrDefault();
                    LisieStores.Extensibility.ProductSearchResult _firstProductSearchResult = await _IMarketFetcher.GetProductMetadata(_firstSelectedStoreProductResult.Url);
                    if (_firstProductSearchResult == null)
                        return -9;


                    Products _newProduct = GetProductOfSearchResult(_firstProductSearchResult);
                    _newProduct.IsTemp = true;

                    _newProduct.Barcode = !string.IsNullOrEmpty(product.Barcode) ?
                        product.Barcode :
                        !string.IsNullOrEmpty(_newProduct.Barcode) ? _newProduct.Barcode : "0";
                    _newProduct.CreatedByUserId = product.UserId;

                    int _newProductId = -1;

                    //no product  found
                    if (_productFound == null)
                    {
                        _newProductId = AddNewProduct(_newProduct);
                    }
                    //product with barcode found - update data
                    else
                    {
                        _newProductId = _productFound.Id;

                        if (product.IsToOverwrite)
                        {
                            DeleteStoreProductsOfProduct(_newProductId);

                            string _AppDataPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/tempimg.temp");
                            CopyProduct(_productFound, _newProduct, _AppDataPath);
                            db.SaveChanges();
                        }
                    }


                    foreach (var productResult in product.SelectedResults)
                    {
                        _IMarketFetcher = Helpers.Extensibility.GetStoreFetcher(productResult.StoreId);
                        LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = null;

                        //Don´t re-get first search result
                        if (productResult.StoreId == product.FirstAddedProductFromStoreId)
                            _ProductSearchResult = _firstProductSearchResult;
                        else
                            _ProductSearchResult = await _IMarketFetcher.GetProductMetadata(productResult.Url);

                        if (_ProductSearchResult != null)
                        {
                            CreateOrUpdateStoreProductNew(_ProductSearchResult, _newProductId, product.UserId, productResult.StoreId);
                        }
                    }


                    //Add product to different lists

                    foreach (string _list in product.Lists)
                    {
                        _userProductId = Managers.UserListsManager.AddProductToList(_newProductId, _newProduct.Name, _list, 1, null, true, product.UserId);

                    }
                    if (product.Lists.Count == 0)
                        return -2; //sucess code for only updatedproduct
                }
                catch (Exception ex)
                {
                    return -9; //return error code
                }
                return _userProductId;
            }
        }

        //if lists exists, returs userProductId
        //if it doesn´t , returns productId
        static async public Task<int> CreateV2(ProductItemCreate product) //returns new userProductId
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                int _userProductId = -1;
                string _finalBarcode = product.Barcode;
                try
                {
                    //Check if product with ean already exists , if it does add to lists and return last userProductId
                    Products _productFound = null;
                    if (!string.IsNullOrEmpty(product.Barcode))
                    {
                        _productFound = GetByBarcodeV2(product.Barcode, product.UserId);
                        if (_productFound != null)
                        {
                            if (product.Lists == null || product.Lists.Count == 0)
                                return _productFound.Id;
                            return UserListsManager.AddProductToLists(_productFound, product.Lists, product.UserId);
                        }
                    }

                    //See if exists product with StoreUrl, and StoreProductId (AVOID DUPLICATES)
                    ///if it exists, add to lists and return userProductId of last list added
                    //TODO(maybe) - Add Barcoes Table , to associate new barcodes to existing products
                    int _productExistsInStoresId = CheckIfProductExistsInStores(product.SelectedResults);
                    if (_productExistsInStoresId != -1)
                    {
                        _productFound = db.Products.Where(c => c.Id == _productExistsInStoresId).FirstOrDefault();
                        if (product.Lists == null || product.Lists.Count == 0)
                            return _productFound.Id;
                        else
                            return UserListsManager.AddProductToLists(_productFound, product.Lists, product.UserId);
                    }

                    //get metadata from all selectedResults
                    List<LisieStores.Extensibility.ProductSearchResult> _productsStoreMetadata = new List<LisieStores.Extensibility.ProductSearchResult>();
                    foreach (var _productStoreMetadata in product.SelectedResults)
                    {
                        LisieStores.Extensibility.IMarketFetcher _ImarketFetcher = Helpers.Extensibility.GetStoreFetcher(_productStoreMetadata.StoreId);
                        LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = null;
                        if (!string.IsNullOrEmpty(_productStoreMetadata.Url))
                            _ProductSearchResult = await _ImarketFetcher.GetProductMetadata(_productStoreMetadata.Url);
                        else if (!string.IsNullOrEmpty(_productStoreMetadata.OnlineProductId))
                            _ProductSearchResult = await _ImarketFetcher.GetProductMetadataById(_productStoreMetadata.OnlineProductId);

                        if (_ProductSearchResult != null)
                        {
                            //See if exists product with StoreUrl, and StoreProductId
                            int _productIdExistsInStores = CheckIfProductExistsInStore(_ProductSearchResult.StoreId, _ProductSearchResult.OnlineProductId, _ProductSearchResult.Url);
                            if (_productIdExistsInStores != -1)
                            {
                                //Product already exists
                                _productFound = db.Products.Where(c => c.Id == _productIdExistsInStores).FirstOrDefault();
                                if (product.Lists == null || product.Lists.Count == 0)
                                    return _productFound.Id;
                                else
                                    return UserListsManager.AddProductToLists(_productFound, product.Lists, product.UserId);
                            }

                            //Add result do list
                            _productsStoreMetadata.Add(_ProductSearchResult);

                            //if finalBarcode is null and this is not, set with this
                            if (string.IsNullOrEmpty(_finalBarcode) &&
                                !string.IsNullOrEmpty(_ProductSearchResult.Barcode))
                            {
                                _finalBarcode = _ProductSearchResult.Barcode;
                            }
                        }
                    }

                    //Go get the FirstAddedProductFromStoreId selected by user of ProductStoreetadata
                    LisieStores.Extensibility.ProductSearchResult _firstSelectedStoreProductResult = _productsStoreMetadata.Where(c => c.StoreId == product.FirstAddedProductFromStoreId).FirstOrDefault();
                    if (_firstSelectedStoreProductResult == null)
                    {
                        //if the first result cannot be get, try the next that it´s not null
                        if (_productsStoreMetadata.Count > 0)
                            _firstSelectedStoreProductResult = _productsStoreMetadata[0];
                        else //no more results, return with error
                            return -9;

                    }

                    //if first result has barcode, and _finalBarcode is null, add this on
                    if (string.IsNullOrEmpty(product.Barcode) &&
                    !string.IsNullOrEmpty(_firstSelectedStoreProductResult.Barcode))
                        _finalBarcode = _firstSelectedStoreProductResult.Barcode;

                    //If finalBarcode Check if product already exists
                    if (!string.IsNullOrEmpty(_finalBarcode))
                        _productFound = GetByBarcode(_finalBarcode);
                    //check again if product with same EAN exists, if it does, add to lists and return last userProductId
                    if (_productFound != null)
                    {
                        if (product.Lists == null || product.Lists.Count == 0)
                        {
                            //Create stores that doesn´t exist
                            foreach (var productResult in _productsStoreMetadata)
                            {
                                if (productResult != null)
                                {
                                    CreateOrUpdateStoreProductNew(productResult, _productFound.Id, product.UserId, productResult.StoreId, true);
                                }
                            }
                            return _productFound.Id;
                        }
                        else
                            return UserListsManager.AddProductToLists(_productFound, product.Lists, product.UserId);
                    }

                    Products _newProduct = GetProductOfSearchResult(_firstSelectedStoreProductResult);
                    _newProduct.IsTemp = true;

                    var _productWithBarcodeAlreadyExists = db.Products.Where(c => c.Barcode == _finalBarcode).FirstOrDefault();
                    if (_productWithBarcodeAlreadyExists == null)
                    {
                        _newProduct.Barcode = _finalBarcode;
                    }
                    else //if finalBarcode exists, return it
                    {
                        if (product.Lists == null || product.Lists.Count == 0)
                        {
                            //Create stores that doesn´t exist
                            foreach (var productResult in _productsStoreMetadata)
                            {
                                if (productResult != null)
                                {
                                    CreateOrUpdateStoreProductNew(productResult, _productFound.Id, product.UserId, productResult.StoreId, true);
                                }
                            }
                            return _productFound.Id;
                        }
                        else
                            return UserListsManager.AddProductToLists(_productWithBarcodeAlreadyExists, product.Lists, product.UserId);
                    }
                    _newProduct.CreatedByUserId = product.UserId;
                    int _newProductId = AddNewProduct(_newProduct);

                    //If producted not added, ex: has no barcode, return -10
                    if (_newProductId == -1)
                        return -10;

                    //Create ProductStores
                    foreach (var productResult in _productsStoreMetadata)
                    {
                        if (productResult != null)
                        {
                            CreateOrUpdateStoreProductNew(productResult, _newProductId, product.UserId, productResult.StoreId);
                        }
                    }

                    //Add product to different lists
                    if (product.Lists != null && product.Lists.Count > 0)
                    {
                        foreach (string _list in product.Lists)
                        {
                            _userProductId = UserListsManager.AddProductToList(_newProductId, _newProduct.Name, _list, 1, null, true, product.UserId);
                        }
                    }
                    else
                    {
                        return _newProductId;
                    }

                }
                catch (Exception ex)
                {
                    Logger.Debug(ex.InnerException.Message);
                    return -9; //return error code
                }
                return _userProductId;
            }
        }


        static async public Task<int> CreateV3(ProductItemCreate product) //returns new userProductId
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                int _userProductId = -1;
                string _finalBarcode = product.Barcode;
                try
                {
                    //Check if product with ean already exists , if it does add to lists and return last userProductId
                    Products _productFound = null;
                    if (!string.IsNullOrEmpty(product.Barcode))
                    {
                        _productFound = GetByBarcodeV2(product.Barcode, product.UserId);
                        if (_productFound != null)
                        {
                            if (product.Lists == null || product.Lists.Count == 0)
                                return _productFound.Id;
                            return UserListsManager.AddProductToLists(_productFound, product.Lists, product.UserId);
                        }
                    }

                    //See if exists product with StoreUrl, and StoreProductId (AVOID DUPLICATES)
                    ///if it exists, add to lists and return userProductId of last list added
                    //TODO(maybe) - Add Barcoes Table , to associate new barcodes to existing products
                    int _productExistsInStoresId = CheckIfProductExistsInStores(product.SelectedResults);
                    if (_productExistsInStoresId != -1)
                    {
                        _productFound = db.Products.Where(c => c.Id == _productExistsInStoresId).FirstOrDefault();
                        if (product.Lists == null || product.Lists.Count == 0)
                            return _productFound.Id;
                        else
                            return UserListsManager.AddProductToLists(_productFound, product.Lists, product.UserId);
                    }

                    //get metadata from all selectedResults
                    List<LisieStores.Extensibility.ProductSearchResult> _productsStoreMetadata = new List<LisieStores.Extensibility.ProductSearchResult>();
                    foreach (var _productStoreMetadata in product.SelectedResults)
                    {
                        LisieStores.Extensibility.IMarketFetcher _ImarketFetcher = Helpers.Extensibility.GetStoreFetcher(_productStoreMetadata.StoreId);
                        LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = null;
                        if (!string.IsNullOrEmpty(_productStoreMetadata.Url))
                            _ProductSearchResult = await _ImarketFetcher.GetProductMetadata(_productStoreMetadata.Url);
                        else if (!string.IsNullOrEmpty(_productStoreMetadata.OnlineProductId))
                            _ProductSearchResult = await _ImarketFetcher.GetProductMetadataById(_productStoreMetadata.OnlineProductId);

                        if (_ProductSearchResult != null)
                        {
                            //See if exists product with StoreUrl, and StoreProductId
                            int _productIdExistsInStores = CheckIfProductExistsInStore(_ProductSearchResult.StoreId, _ProductSearchResult.OnlineProductId, _ProductSearchResult.Url);
                            if (_productIdExistsInStores != -1)
                            {
                                //Product already exists
                                _productFound = db.Products.Where(c => c.Id == _productIdExistsInStores).FirstOrDefault();
                                if (product.Lists == null || product.Lists.Count == 0)
                                    return _productFound.Id;
                                else
                                    return UserListsManager.AddProductToLists(_productFound, product.Lists, product.UserId);
                            }

                            //Add result do list
                            _productsStoreMetadata.Add(_ProductSearchResult);

                            //if finalBarcode is null and this is not, set with this
                            if (string.IsNullOrEmpty(_finalBarcode) &&
                                !string.IsNullOrEmpty(_ProductSearchResult.Barcode))
                            {
                                _finalBarcode = _ProductSearchResult.Barcode;
                            }
                        }
                    }

                    //Go get the FirstAddedProductFromStoreId selected by user of ProductStoreetadata
                    LisieStores.Extensibility.ProductSearchResult _firstSelectedStoreProductResult = _productsStoreMetadata.Where(c => c.StoreId == product.FirstAddedProductFromStoreId).FirstOrDefault();
                    if (_firstSelectedStoreProductResult == null)
                    {
                        //if the first result cannot be get, try the next that it´s not null
                        if (_productsStoreMetadata.Count > 0)
                            _firstSelectedStoreProductResult = _productsStoreMetadata[0];
                        else //no more results, return with error
                            return -9;

                    }

                    //if first result has barcode, and _finalBarcode is null, add this on
                    if (string.IsNullOrEmpty(product.Barcode) &&
                    !string.IsNullOrEmpty(_firstSelectedStoreProductResult.Barcode))
                        _finalBarcode = _firstSelectedStoreProductResult.Barcode;

                    //If finalBarcode Check if product already exists
                    if (!string.IsNullOrEmpty(_finalBarcode))
                        _productFound = GetByBarcodeV2(_finalBarcode, product.UserId);
                    //check again if product with same EAN exists, if it does, add to lists and return last userProductId
                    if (_productFound != null)
                    {
                        if (product.Lists == null || product.Lists.Count == 0)
                        {
                            //Create stores that doesn´t exist
                            foreach (var productResult in _productsStoreMetadata)
                            {
                                if (productResult != null)
                                {
                                    CreateOrUpdateStoreProductNew(productResult, _productFound.Id, product.UserId, productResult.StoreId, true);
                                }
                            }
                            return _productFound.Id;
                        }
                        else
                            return UserListsManager.AddProductToLists(_productFound, product.Lists, product.UserId);
                    }

                    Products _newProduct = GetProductOfSearchResult(_firstSelectedStoreProductResult);
                    _newProduct.IsTemp = true;

                    var _productWithBarcodeAlreadyExists = db.Products.Where(c => c.Barcode == _finalBarcode).FirstOrDefault();
                    if (_productWithBarcodeAlreadyExists == null)
                    {
                        _newProduct.Barcode = _finalBarcode;
                    }
                    else //if finalBarcode exists, return it
                    {
                        if (product.Lists == null || product.Lists.Count == 0)
                        {
                            //Create stores that doesn´t exist
                            foreach (var productResult in _productsStoreMetadata)
                            {
                                if (productResult != null)
                                {
                                    CreateOrUpdateStoreProductNew(productResult, _productFound.Id, product.UserId, productResult.StoreId, true);
                                }
                            }
                            return _productFound.Id;
                        }
                        else
                            return UserListsManager.AddProductToLists(_productWithBarcodeAlreadyExists, product.Lists, product.UserId);
                    }
                    _newProduct.CreatedByUserId = product.UserId;
                    int _newProductId = AddNewProduct(_newProduct);

                    //If producted not added, ex: has no barcode, return -10
                    if (_newProductId == -1)
                        return -10;

                    //Create ProductStores
                    foreach (var productResult in _productsStoreMetadata)
                    {
                        if (productResult != null)
                        {
                            CreateOrUpdateStoreProductNew(productResult, _newProductId, product.UserId, productResult.StoreId);
                        }
                    }

                    //Add product to different lists
                    if (product.Lists != null && product.Lists.Count > 0)
                    {
                        foreach (string _list in product.Lists)
                        {
                            _userProductId = UserListsManager.AddProductToList(_newProductId, _newProduct.Name, _list, 1, null, true, product.UserId);
                        }
                    }
                    else
                    {
                        return _newProductId;
                    }

                }
                catch (Exception ex)
                {
                    Logger.Debug(ex.InnerException.Message);
                    return -9; //return error code
                }
                return _userProductId;
            }
        }

        static async public Task<JsonApiResponse> CreateV4(ProductItemCreate product) //returns new userProductId
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                JsonApiResponse _response = new JsonApiResponse();

                int _userProductId = -1;
                string _finalBarcode = product.Barcode;
                try
                {
                    //Check if product with ean already exists , if it does add to lists and return last userProductId
                    Products _productFound = null;
                    if (!string.IsNullOrEmpty(product.Barcode))
                    {
                        _productFound = GetByBarcode(product.Barcode);
                        if (_productFound != null)
                        {
                            if (product.Lists == null || product.Lists.Count == 0)
                            {
                                return new JsonApiResponse
                                {
                                    Success = false,
                                    Code = -1,
                                    Message = "Product found but no list specified to add to list",
                                    Data = new { ProductIdFound = _productFound.Id }
                                };
                            }
                            var _added_UserProductId = UserListsManager.AddProductToLists(_productFound, product.Lists, product.UserId); ;
                            var _completeModel = Managers.UserListsManager.GetCompleteModelV2(_added_UserProductId, product.UserId);
                            return new JsonApiResponse
                            {
                                Success = true,
                                Code = 1,
                                Message = "Product found and added to user list",
                                Data = _completeModel
                            };

                        }
                        //else //see if product already added but waiting for review
                        //{
                        //    _productFound = GetByBarcode(product.Barcode);
                        //    if (_productFound != null)
                        //    {
                        //        return new JsonApiResponse
                        //        {
                        //            Success = false,
                        //            Code = -2,
                        //            Message = "Product found but added by another user and not yet reviewed",
                        //            Data = new { ProductIdFound = _productFound.Id }
                        //        };
                        //    }
                        //}
                    }

                    //See if exists product with StoreUrl, and StoreProductId (AVOID DUPLICATES)
                    ///if it exists, add to lists and return userProductId of last list added
                    //TODO(maybe) - Add Barcoes Table , to associate new barcodes to existing products
                    int _productExistsInStoresId = CheckIfProductExistsInStores(product.SelectedResults);
                    if (_productExistsInStoresId != -1)
                    {
                        _productFound = db.Products.Where(c => c.Id == _productExistsInStoresId).FirstOrDefault();
                        if (product.Lists == null || product.Lists.Count == 0)
                        {
                            return new JsonApiResponse
                            {
                                Success = false,
                                Code = -3,
                                Message = "Product found in stores but no list specified to add to list",
                                Data = new { ProductIdFound = _productFound.Id }
                            };
                        }
                        else
                        {
                            var _added_UserProductId = UserListsManager.AddProductToLists(_productFound, product.Lists, product.UserId);
                            var _completeModel = Managers.UserListsManager.GetCompleteModelV2(_added_UserProductId, product.UserId);
                            return new JsonApiResponse
                            {
                                Success = true,
                                Code = 1,
                                Message = "Product found in stores and added to user list",
                                Data = _completeModel
                            };
                        }
                    }

                    //get metadata from all selectedResults
                    List<LisieStores.Extensibility.ProductSearchResult> _productsStoreMetadata = new List<LisieStores.Extensibility.ProductSearchResult>();
                    foreach (var _productStoreMetadata in product.SelectedResults)
                    {
                        LisieStores.Extensibility.IMarketFetcher _ImarketFetcher = Helpers.Extensibility.GetStoreFetcher(_productStoreMetadata.StoreId);
                        LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = null;
                        if (!string.IsNullOrEmpty(_productStoreMetadata.Url))
                            _ProductSearchResult = await _ImarketFetcher.GetProductMetadata(_productStoreMetadata.Url);
                        else if (!string.IsNullOrEmpty(_productStoreMetadata.OnlineProductId))
                            _ProductSearchResult = await _ImarketFetcher.GetProductMetadataById(_productStoreMetadata.OnlineProductId);

                        if (_ProductSearchResult != null)
                        {
                            //See if exists product with StoreUrl, and StoreProductId
                            int _productIdExistsInStores = CheckIfProductExistsInStore(_ProductSearchResult.StoreId, _ProductSearchResult.OnlineProductId, _ProductSearchResult.Url);
                            if (_productIdExistsInStores != -1)
                            {
                                //Product already exists
                                _productFound = db.Products.Where(c => c.Id == _productIdExistsInStores).FirstOrDefault();
                                if (product.Lists == null || product.Lists.Count == 0)
                                {
                                    return new JsonApiResponse
                                    {
                                        Success = false,
                                        Code = -4,
                                        Message = "Product found in stores but no list specified to add to list",
                                        Data = new { ProductIdFound = _productFound.Id }
                                    };
                                }
                                else
                                {
                                    var _added_UserProductId = UserListsManager.AddProductToLists(_productFound, product.Lists, product.UserId); ;
                                    var _completeModel = Managers.UserListsManager.GetCompleteModelV2(_added_UserProductId, product.UserId);
                                    return new JsonApiResponse
                                    {
                                        Success = true,
                                        Code = 1,
                                        Message = "Product found in stores and added to user list",
                                        Data = _completeModel
                                    };
                                }
                            }

                            //Add result do list
                            _productsStoreMetadata.Add(_ProductSearchResult);

                            //if finalBarcode is null and this is not, set with this
                            if (string.IsNullOrEmpty(_finalBarcode) &&
                                !string.IsNullOrEmpty(_ProductSearchResult.Barcode))
                            {
                                _finalBarcode = _ProductSearchResult.Barcode;
                            }
                        }
                    }

                    //Go get the FirstAddedProductFromStoreId selected by user of ProductStoreetadata
                    LisieStores.Extensibility.ProductSearchResult _firstSelectedStoreProductResult = _productsStoreMetadata.Where(c => c.StoreId == product.FirstAddedProductFromStoreId).FirstOrDefault();
                    if (_firstSelectedStoreProductResult == null)
                    {
                        //if the first result cannot be get, try the next that it´s not null
                        if (_productsStoreMetadata.Count > 0)
                            _firstSelectedStoreProductResult = _productsStoreMetadata[0];
                        else //no more results, return with error
                        {
                            return new JsonApiResponse
                            {
                                Success = false,
                                Code = -5,
                                Message = "_productsStoreMetadata.Count count is 0",
                                Data = null
                            };
                        }

                    }

                    //if first result has barcode, and _finalBarcode is null, add this on
                    if (string.IsNullOrEmpty(product.Barcode) &&
                    !string.IsNullOrEmpty(_firstSelectedStoreProductResult.Barcode))
                        _finalBarcode = _firstSelectedStoreProductResult.Barcode;

                    //If finalBarcode Check if product already exists
                    if (!string.IsNullOrEmpty(_finalBarcode))
                        _productFound = GetByBarcode(_finalBarcode);
                    //check again if product with same EAN exists, if it does, add to lists and return last userProductId
                    if (_productFound != null)
                    {
                        if (product.Lists == null || product.Lists.Count == 0)
                        {
                            //Create stores that doesn´t exist
                            foreach (var productResult in _productsStoreMetadata)
                            {
                                if (productResult != null)
                                {
                                    CreateOrUpdateStoreProductNew(productResult, _productFound.Id, product.UserId, productResult.StoreId, false);
                                }
                            }
                            return new JsonApiResponse
                            {
                                Success = true,
                                Code = 3,
                                Message = "Product found in stores but no list specified to add to list. if new stores exist, they were added",
                                Data = new { ProductIdFound = _productFound.Id }
                            };
                        }
                        else
                        {
                            var _added_UserProductId = UserListsManager.AddProductToLists(_productFound, product.Lists, product.UserId); ;
                            var _completeModel = Managers.UserListsManager.GetCompleteModelV2(_added_UserProductId, product.UserId);
                            return new JsonApiResponse
                            {
                                Success = true,
                                Code = 1,
                                Message = "Product found and added to user list",
                                Data = _completeModel
                            };
                        }
                    }

                    Products _newProduct = GetProductOfSearchResult(_firstSelectedStoreProductResult);
                    _newProduct.IsTemp = true;

                    var _productWithBarcodeAlreadyExists = db.Products.Where(c => c.Barcode == _finalBarcode).FirstOrDefault();
                    if (_productWithBarcodeAlreadyExists == null)
                    {
                        _newProduct.Barcode = _finalBarcode;
                    }
                    else //if finalBarcode exists, return it
                    {
                        if (product.Lists == null || product.Lists.Count == 0)
                        {
                            //Create stores that doesn´t exist
                            foreach (var productResult in _productsStoreMetadata)
                            {
                                if (productResult != null)
                                {
                                    CreateOrUpdateStoreProductNew(productResult, _productFound.Id, product.UserId, productResult.StoreId, true, false, false);
                                }
                            }
                            return new JsonApiResponse
                            {
                                Success = true,
                                Code = 3,
                                Message = "Product found in stores but no list specified to add to list. if new stores exist, they were added",
                                Data = new { ProductIdFound = _productFound.Id }
                            };
                        }
                        else
                        {
                            var _added_UserProductId = UserListsManager.AddProductToLists(_productWithBarcodeAlreadyExists, product.Lists, product.UserId);
                            var _completeModel = Managers.UserListsManager.GetCompleteModelV2(_added_UserProductId, product.UserId);
                            return new JsonApiResponse
                            {
                                Success = true,
                                Code = 1,
                                Message = "Product found and added to user list",
                                Data = _completeModel
                            };
                        }
                    }
                    _newProduct.CreatedByUserId = product.UserId;
                    int _newProductId = AddNewProduct(_newProduct);

                    //If producted not added, ex: has no barcode, return -10
                    if (_newProductId == -1)
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -8,
                            Message = "Tried to add new product, without sucess.",
                        };
                    }

                    //Create ProductStores
                    foreach (var productResult in _productsStoreMetadata)
                    {
                        if (productResult != null)
                        {
                            CreateOrUpdateStoreProductNew(productResult, _newProductId, product.UserId, productResult.StoreId, false, true, false);
                        }
                    }

                    //Add product to different lists
                    if (product.Lists != null && product.Lists.Count > 0)
                    {
                        foreach (string _list in product.Lists)
                        {
                            _userProductId = UserListsManager.AddProductToList(_newProductId, _newProduct.Name, _list, 1, null, true, product.UserId);
                        }
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Success = true,
                            Code = 2,
                            Message = "New product added, but not added to any list because Lists was not specified",
                            Data = new { NewProductId = _newProductId }
                        };
                    }

                }
                catch (Exception ex)
                {
                    Logger.Debug(ex.InnerException.Message);
                    return new JsonApiResponse
                    {
                        Success = false,
                        Code = -10,
                        Message = "Error: " + ex.InnerException.Message,
                    };
                }
                var _completeModel2 = Managers.UserListsManager.GetCompleteModelV2(_userProductId, product.UserId);
                return new JsonApiResponse
                {
                    Success = true,
                    Code = 1,
                    Message = "New product added and added to user list",
                    Data = _completeModel2
                };
            }
        }

        static public async Task<int> UpdateStores(string userId, ProductItemNew product)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                OnlineProducts _OnlineProducts = new OnlineProducts();
                List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();

                int storeProductsCreatedOrUpdated = 0;
                int storeProductsDeleted = 0;
                try
                {
                    var productFound = db.Products.Where(c => c.Id == product.ProductId).FirstOrDefault();
                    int _productId = -1;

                    //no product found
                    if (productFound != null)
                    {
                        _productId = productFound.Id;
                        foreach (var productResult in product.SelectedResults)
                        {
                            //Get product metadata with MarketFetcher because of category and price weight and unit
                            LisieStores.Extensibility.IMarketFetcher _IMarketFetcher = Helpers.Extensibility.GetStoreFetcher(productResult.StoreId);
                            //LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = await _IMarketFetcher.GetProductMetadataById(productResult.OnlineProductId);
                            LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = null;
                            if (!string.IsNullOrEmpty(productResult.OnlineProductId))
                                _ProductSearchResult = await _IMarketFetcher.GetProductMetadataById(productResult.OnlineProductId);
                            if (!string.IsNullOrEmpty(productResult.Url) && _ProductSearchResult == null)
                                _ProductSearchResult = await _IMarketFetcher.GetProductMetadata(productResult.Url);

                            if (_ProductSearchResult != null)
                            {
                                //First check if Store product exists in other Products
                                int _existingProductId = CheckIfProductExistsInStore(productResult.StoreId, productResult.OnlineProductId, productResult.Url);
                                //if exists don´t do nothing
                                if (_existingProductId == -1)
                                {
                                    var sucess = Managers.ProductsManager.CreateOrUpdateStoreProductNew(_ProductSearchResult, _productId, userId, productResult.StoreId, false);
                                    if (sucess)
                                        storeProductsCreatedOrUpdated++;
                                }
                            }
                        }

                        //Remove Store Ids
                        if (product.StoreIdsToRemove != null)
                        {
                            foreach (var _storeIdToRemove in product.StoreIdsToRemove)
                            {
                                var sucess = Managers.ProductsManager.DeleteStoreProductOfProductNew(product.ProductId, userId, _storeIdToRemove);
                                storeProductsCreatedOrUpdated++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex.InnerException.Message);
                    return -1;
                }

                return storeProductsCreatedOrUpdated;
            }
        }

        static public async Task<JsonApiResponse> UpdateStoresV2(string userId, ProductItemNew product)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                JsonApiResponse _response = new JsonApiResponse();

                OnlineProducts _OnlineProducts = new OnlineProducts();
                List<LisieStores.Extensibility.ProductSearchResult> _ProductSearchResults = new List<LisieStores.Extensibility.ProductSearchResult>();

                int storeProductsCreatedOrUpdated = 0;
                int storeProductsDeleted = 0;
                try
                {
                    var productFound = db.Products.Where(c => c.Id == product.ProductId).FirstOrDefault();
                    int _productId = -1;

                    //no product found
                    if (productFound != null)
                    {
                        _productId = productFound.Id;
                        foreach (var productResult in product.SelectedResults)
                        {
                            //Get product metadata with MarketFetcher because of category and price weight and unit
                            LisieStores.Extensibility.IMarketFetcher _IMarketFetcher = Helpers.Extensibility.GetStoreFetcher(productResult.StoreId);
                            //LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = await _IMarketFetcher.GetProductMetadataById(productResult.OnlineProductId);
                            LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = null;
                            if (!string.IsNullOrEmpty(productResult.OnlineProductId))
                                _ProductSearchResult = await _IMarketFetcher.GetProductMetadataById(productResult.OnlineProductId);
                            if (!string.IsNullOrEmpty(productResult.Url) && _ProductSearchResult == null)
                                _ProductSearchResult = await _IMarketFetcher.GetProductMetadata(productResult.Url);

                            if (_ProductSearchResult != null)
                            {
                                //First check if Store product exists in other Products
                                int _existingProductId = CheckIfProductExistsInStore(productResult.StoreId, productResult.OnlineProductId, productResult.Url);
                                //if exists don´t do nothing
                                if (_existingProductId == -1)
                                {
                                    var sucess = Managers.ProductsManager.CreateOrUpdateStoreProductNew(_ProductSearchResult, _productId, userId, productResult.StoreId, false);
                                    if (sucess)
                                        storeProductsCreatedOrUpdated++;
                                }
                                else //Store product already exists
                                {
                                    return new JsonApiResponse
                                    {
                                        Success = false,
                                        Code = -3,
                                        Message = "Already found a product with id " + _existingProductId + " of store with id " + productResult.StoreId,
                                        Data = new { ProductIdFound = _existingProductId, StoreId = productResult.StoreId }
                                    };
                                }
                            }
                        }

                        //Remove Store Ids
                        if (product.StoreIdsToRemove != null)
                        {
                            foreach (var _storeIdToRemove in product.StoreIdsToRemove)
                            {
                                var sucess = Managers.ProductsManager.DeleteStoreProductOfProductNew(product.ProductId, userId, _storeIdToRemove);
                                storeProductsCreatedOrUpdated++;
                            }
                        }
                    }
                    else
                    {
                        return new JsonApiResponse
                        {
                            Success = false,
                            Code = -2,
                            Message = "No product with that id found"
                        };
                    }
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex.InnerException.Message);
                    return new JsonApiResponse
                    {
                        Success = false,
                        Code = -1,
                        Message = ex.InnerException.Message
                    };
                }

                return new JsonApiResponse
                {
                    Success = true,
                    Code = 1,
                    Message = storeProductsCreatedOrUpdated + " product stores updated",
                    Data = storeProductsCreatedOrUpdated
                }; ;
            }
        }

        static public bool DeleteSafely(int productId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    Products products = db.Products.Find(productId);

                    //remove from UserProductsConsumed
                    var _UserProductsConsumed = db.UserProductsConsumed.Where(c => c.ProductId == products.Id);
                    db.UserProductsConsumed.RemoveRange(_UserProductsConsumed);

                    //remove from User Lists
                    var _userProducts = db.UserProductsList.Where(c => c.ProductId == products.Id);
                    db.UserProductsList.RemoveRange(_userProducts);

                    //remove from Store Products
                    var _productStores = db.StoreProducts.Where(c => c.ProductId == products.Id);
                    db.StoreProducts.RemoveRange(_productStores);

                    //remove from History
                    var _UserProductsListHistory = db.UserProductsListHistory.Where(c => c.ProductId == products.Id);
                    db.UserProductsListHistory.RemoveRange(_UserProductsListHistory);

                    //remove from price updates
                    var _ProductPricesUpdates = db.ProductPricesUpdates.Where(c => c.ProductId == products.Id);
                    db.ProductPricesUpdates.RemoveRange(_ProductPricesUpdates);

                    db.Products.Remove(products);


                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        static public int GetTotal()
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                return db.Products.Count();
            }
        }

        static public int GetTotalStoreProducts()
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                return db.StoreProducts.Count();
            }
        }

        static public List<string> GetOnlineProductIdsNotFound(GetOnlineProductIdsNotFoundModel model)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _ExistingIds = db.StoreProducts.Where(c => model.OnlineProductIds.Contains(c.OnlineProductId) && c.StoreId == model.StoreId).Select(c => c.OnlineProductId).Distinct().ToList();
                var _nonExistingIds = model.OnlineProductIds.Where(c => !_ExistingIds.Contains(c)).ToList();
                return _nonExistingIds;
            }
        }

        static public ProductsReview AddProductReview(string userId, int productId, string info)
        {

            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    ProductsReview _newProductsReview = new ProductsReview
                    {
                        UserId = userId,
                        ProductId = productId,
                        Info = info,
                        CreateDate = DateTime.Now
                    };
                    db.ProductsReview.Add(_newProductsReview);
                    db.SaveChanges();
                    return _newProductsReview;
                }
            }
            catch (Exception)
            {

                return null;
            }

        }


        static public List<KeyValuePair<int, double>> GetDayBestPriceProducts()
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _todayProductPriceUpdates = db.ProductPricesUpdates.Where(c => c.CreateDate.Day == DateTime.Now.Day && c.CreateDate.Month == DateTime.Now.Month && c.CreateDate.Year == DateTime.Now.Year);

                Dictionary<int, double> _allDiscounts = new Dictionary<int, double>();
                foreach (var _todayProductPriceUpdate in _todayProductPriceUpdates)
                {
                    var _storeProducts = db.StoreProducts.Where(c => c.ProductId == _todayProductPriceUpdate.ProductId);
                    if (_storeProducts.Count() > 0)
                    {


                        var _maxPrice = _storeProducts.Max(c => c.Price);
                        var _minPrice = _storeProducts.Min(c => c.Price);
                        //System.Diagnostics.Debug.WriteLine("ProductId -" + _todayProductPriceUpdate.ProductId);
                        var _discount = Math.Floor((1 - _minPrice.Value / _maxPrice.Value) * 100);
                        if (!_allDiscounts.ContainsKey(_todayProductPriceUpdate.ProductId))
                        {
                            _allDiscounts.Add(_todayProductPriceUpdate.ProductId, _discount);
                        }
                    }

                }
                var _maxes = _allDiscounts.OrderByDescending(c => c.Value).ToList();
                //var _maxes = _allDiscounts.Where(x => x.Value == _allDiscounts.Values.Max()).Select(x => x.Key).ToList();
                //var _maxKey = _allDiscounts.Max(kvp => kvp.Value).Key;

                //var _max = _allDiscounts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

                return _maxes;
            }
        }

        static public bool AcceptTemp(int productId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _product = db.Products.Where(c => c.Id == productId).FirstOrDefault();
                if (_product != null)
                {
                    _product.IsTemp = false;

                    var _StoreProducts = db.StoreProducts.Where(c => c.ProductId == productId && (!c.IsTemp.HasValue || c.IsTemp.Value));
                    ; foreach (var _StoreProduct in _StoreProducts)
                    {
                        _StoreProduct.IsTemp = false;
                    }

                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        static public bool RefuseTemp(int productId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _product = db.Products.Where(c => c.Id == productId).FirstOrDefault();
                if (_product != null)
                {
                    _product.IsTemp = true;

                    //var _StoreProducts = db.StoreProducts.Where(c => c.ProductId == productId && (!c.IsTemp.HasValue || c.IsTemp.Value));
                    //; foreach (var _StoreProduct in _StoreProducts)
                    //{
                    //    _StoreProduct.IsTemp = true;
                    //}

                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        static public List<ProductPricesUpdates> GetProductPricesUpdates(int id, DateTime dateStart, DateTime dateEnd)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                if (dateStart == DateTime.MinValue)
                {
                    var productPriceUpdates = db.ProductPricesUpdates.Where(c => c.ProductId.Equals(id)).OrderBy(c => c.CreateDate).ToList();
                    return productPriceUpdates;
                }
                else
                {
                    var productPriceUpdates = db.ProductPricesUpdates.Where(c => c.ProductId.Equals(id) &&
                    c.CreateDate >= dateStart &&
                    c.CreateDate <= dateEnd).OrderBy(c => c.CreateDate).ToList();
                    return productPriceUpdates;
                }

            }
        }

        static public List<UserProductsList> GetUserListsWhereProductIs(int productId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _userProductsList = db.UserProductsList.Where(c => c.ProductId == productId).DistinctBy(c => c.UserId).ToList();
                return _userProductsList;
            }
        }

        static public string CalculateProductCategory(int productId)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    //var _found = db.StoreProducts.Where(c => c.Category.Length > 0).ToList();
                    var _productCategories = db.ProductCategories.ToList();

                    var _storeProducts = db.StoreProducts.Where(c => c.ProductId == productId).ToList();
                    List<ProductCategoryScore> _ProductCategoryScore = new List<ProductCategoryScore>();
                    foreach (var _storeProduct in _storeProducts)
                    {

                        foreach (var _category in _productCategories)
                        {
                            var _synonymous = !string.IsNullOrEmpty(_category.Synonymous) ? _category.Synonymous.Split(';') : new string[0];
                            var _ProductCategoryScoreExists = _ProductCategoryScore.Where(c => c.Category == _category.Name).FirstOrDefault();
                            if (_ProductCategoryScoreExists == null)
                            {
                                ProductCategoryScore _newProductCategoryScore = new ProductCategoryScore { Category = _category.Name, Score = 0 };
                                _ProductCategoryScore.Add(_newProductCategoryScore);
                                _ProductCategoryScoreExists = _newProductCategoryScore;
                            }

                            if (!string.IsNullOrEmpty(_storeProduct.CategoryFull))
                            {
                                bool _existsInText = TextTools.SearchInText(_storeProduct.CategoryFull, _category.Name);

                                _ProductCategoryScoreExists.Score += _existsInText ? 1 : 0;

                                //now the synonymous
                                foreach (var _synonym in _synonymous)
                                {
                                    _existsInText = TextTools.SearchInText(_storeProduct.CategoryFull, _synonym);
                                    _ProductCategoryScoreExists.Score += _existsInText ? 1 : 0;
                                }
                            }
                            if (!string.IsNullOrEmpty(_storeProduct.Category))
                            {
                                double _score = TextTools.CalculateSimilarity(_storeProduct.Category, _category.Name);
                                _ProductCategoryScoreExists.Score += _score;

                                //now the synonymous
                                foreach (var _synonym in _synonymous)
                                {
                                    var _existsInText = TextTools.SearchInText(_storeProduct.Category, _synonym);
                                    _ProductCategoryScoreExists.Score += _existsInText ? 1 : 0;
                                }
                            }
                        }
                    }
                    var _x = _ProductCategoryScore.Where(c => c.Score == _ProductCategoryScore.Max(x => x.Score)).FirstOrDefault();
                    if (_x.Score == 0) return string.Empty;

                    var _getBestScoreCategory = _x.Category;
                    return _getBestScoreCategory;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        static async public Task<List<StoreProducts>> FindStoreProductsWithAI(int productId)
        {
            List<StoreProducts> _storeProductsToReturn = new List<StoreProducts>();
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _storeProducts = db.StoreProducts.Where(c => c.ProductId == productId).ToList();
                var _product = db.Products.Where(c => c.Id == productId).FirstOrDefault();
                if (_product != null || _storeProducts.Count != 0)
                {
                    List<LisieStores.Extensibility.Market> _Markets = Helpers.Extensibility.GetStoreFetchers();
                    foreach (var _Market in _Markets)
                    {
                        //if (_Market.StoreId == 5)
                        if (_Market.StoreId != 1 && _Market.StoreId != 2)
                        {
                            continue;
                        }

                        
                        var _storeProduct = _storeProducts.Where(c => c.StoreId == _Market.StoreId).FirstOrDefault();

                        if (_storeProduct != null)
                        {
                            LisieStores.Extensibility.IMarketFetcher _IMarketFetcher = Helpers.Extensibility.GetStoreFetcher(_storeProduct.StoreId);

                            //fetch from database from table ProductPricesUpdatesFails where ProductID and store id equals the variables above, see how many fails de store product had in the last week , and if it had more than 3 fails, set LastPriceUpdateSuccess to false, otherwise true

                            //var _storeProductFails = db.ProductPricesUpdatesFails.Where(c => c.ProductId == productId && c.StoreId == _IMarketFetcher.StoreId).Count();
                            //bool _storeProductUpdatedMoreThanTwoWeeksAgo = _storeProduct.LastSuccessfulUpdateDate < DateTime.Now.AddDays(-14);
                            //if (_storeProductFails > 3 && _storeProductUpdatedMoreThanTwoWeeksAgo)
                            //    if (_storeProductUpdatedMoreThanTwoWeeksAgo)
                            //    {
                            //        continue;
                            //    }
                            if (_storeProduct.NeedsUpdate.Value)
                            {
                                //_storeProduct.LastPriceUpdateSuccess = false;
                                var _productSearchResult = await _IMarketFetcher.ExtractProductInfoAI(_storeProduct.Stores.Url + _storeProduct.Url);
                                if (_productSearchResult != null)
                                {
                                    bool _success =  CreateOrUpdateStoreProductNew(_productSearchResult, productId, "9ff8224f-17cf-49fb-b555-05779a13eb40", _storeProduct.StoreId, ifExistsDontUpdate:false);
                                }
                                else
                                {
                                    var __productSearchResult = await _IMarketFetcher.FindProductAI(_product.Name, _product.Brand, _product.Weight);
                                    if (__productSearchResult != null)
                                    {
                                        bool _success = CreateOrUpdateStoreProductNew(__productSearchResult, productId, "9ff8224f-17cf-49fb-b555-05779a13eb40", _Market.StoreId, ifExistsDontUpdate: false);
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                        } 
                        else
                        {
                            LisieStores.Extensibility.IMarketFetcher _IMarketFetcher = Helpers.Extensibility.GetStoreFetcher(_Market.StoreId);

                            var _productSearchResult = await _IMarketFetcher.FindProductAI(_product.Name, _product.Brand, _product.Weight);
                            if (_productSearchResult != null)
                            {
                                bool _success = CreateOrUpdateStoreProductNew(_productSearchResult, productId, "9ff8224f-17cf-49fb-b555-05779a13eb40", _Market.StoreId, ifExistsDontUpdate: false);
                            }
                        }
                    }
                }
                return null;
            }
        }

        static async public Task<List<StoreProducts>> ExtractProductInfoAI(int productId,string userId, int storeId)
        {
            List<StoreProducts> _storeProductsToReturn = new List<StoreProducts>();
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _storeProducts = db.StoreProducts.Where(c => c.ProductId == productId).ToList();
                var _product = db.Products.Where(c => c.Id == productId).FirstOrDefault();
                if (_product != null || _storeProducts.Count != 0)
                {
                    List<LisieStores.Extensibility.Market> _Markets = Helpers.Extensibility.GetStoreFetchers();
                    LisieStores.Extensibility.IMarketFetcher _IMarketFetcher = Helpers.Extensibility.GetStoreFetcher(storeId);

                        var _storeProduct = _storeProducts.Where(c => c.StoreId == storeId).FirstOrDefault();

                        if (_storeProduct != null)
                        {

                                var _productSearchResult = await _IMarketFetcher.ExtractProductInfoAI(_storeProduct.Stores.Url + _storeProduct.Url);
                                if (_productSearchResult != null)
                                {
                                    bool _success = CreateOrUpdateStoreProductNew(_productSearchResult, productId, userId, _storeProduct.StoreId, ifExistsDontUpdate: false);
                                }
                        }
                    
                }
                return null;
            }
        }

        public class ProductCategoryScore
        {
            public string Category { get; set; }
            public double Score { get; set; }
        }
    }
}