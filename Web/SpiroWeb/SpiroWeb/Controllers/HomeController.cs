using ClassLibrary1;
using SpiroStockManagmentDatabaseClass;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace SpiroWeb.Controllers
{
    public class HomeController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        //[Authorize]
        public ActionResult Index()
        {
            //return RedirectToAction("Index", "ShoppingCart");
            //return new FilePathResult("~/Lisie/simple/index.html", "text/html");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult ExportProducts()
        {
            SpiroStockManagmentDatabaseClass.Procedures _SpiroDatabaseProcedures = new SpiroStockManagmentDatabaseClass.Procedures();

            DataManager.ProductsManager _productsManager = new DataManager.ProductsManager();
            string a = HttpContext.Server.MapPath("~/App_Data/InventoryItems.xml");
            _SpiroDatabaseProcedures.XmlDatabaseFilePath = a;
            _SpiroDatabaseProcedures.Initialize();
            IEnumerable<XElement> _allProducts = _SpiroDatabaseProcedures.GetAllProducts();

            string ImagesPath = @"C:\Users\ArdethMain\Dropbox\Spiro Stock Management 1.1\Databases\ItemsImages\";
            foreach (XElement _xElement in _allProducts)
            {
                SpiroStockManagmentDatabaseClass.Objects.Product _productDS = XmlSerializerExtension.DeSerializer(_xElement);
                string _imagepath = ImagesPath + _productDS.PictureSmallFilename;
                Products _newProduct = new Products();
                _newProduct.Barcode = _productDS.BarCode;
                _newProduct.CategoryString = _productDS.categoryString;
                _newProduct.InsertDate = Convert.ToDateTime(_productDS.InsertDate);
                _newProduct.Name = _productDS.Name;
                _newProduct.Picture = GetBase64OfImage(_imagepath);
                _newProduct.Price = _productDS.Price;
                _newProduct.VariableWeightPrice = _productDS.VariableWeightPrice;


                _productsManager.InsertProduct(_newProduct);
            }

            return Json("dasdas", JsonRequestBehavior.AllowGet);
        }

        public JsonResult FixPrices()
        {
            List<Products> _queryProducts = (from c in db.Products
                                             select c).ToList();

            foreach (Products _product in _queryProducts)
            {
                string _productPriceString = _product.Price.ToString();
                if (_productPriceString != "0" && _productPriceString.Length > 3)
                {
                    double _newPrice = double.Parse(_productPriceString.Substring(0, 4));
                    _product.Price = _newPrice;
                    db.Products.Attach(_product);
                    var entry = db.Entry(_product);
                    entry.Property(y => y.Price).IsModified = true;
                    // other changed properties

                }

            }
            db.SaveChanges();
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public byte[] GetBase64OfImage(string imagePath)
        {
            if (System.IO.File.Exists(imagePath))
            {
                using (Image image = Image.FromFile(imagePath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        return imageBytes;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public JsonResult ExportRecepies()
        {
            SpiroStockManagmentDatabaseClass.Procedures _SpiroDatabaseProcedures = new SpiroStockManagmentDatabaseClass.Procedures();

            DataManager.ProductsManager _productsManager = new DataManager.ProductsManager();
            string a = HttpContext.Server.MapPath("~/App_Data/Recepies.xml");
            _SpiroDatabaseProcedures.XmlDatabaseRecepiesPath = a;
            _SpiroDatabaseProcedures.Initialize();
            IEnumerable<XElement> _allProducts = _SpiroDatabaseProcedures.GetAllRecepies();

            string ImagesPath = @"C:\RecipeImages\";

            SpiroStockManagementEntities db = new SpiroStockManagementEntities();
            foreach (XElement _xElement in _allProducts)
            {
                SpiroStockManagmentDatabaseClass.Objects.Recipe _productDS = (SpiroStockManagmentDatabaseClass.Objects.Recipe)XmlSerializerExtension.DeSerializerToObject(_xElement, new SpiroStockManagmentDatabaseClass.Objects.Recipe());

                Recepies _newRecepy = new Recepies();
                _newRecepy.Name = _productDS.Name;
                _newRecepy.Category = _productDS.Category;
                _newRecepy.Commentary = _productDS.Commentary;
                _newRecepy.Cuisine = _productDS.Cuisine;
                _newRecepy.Description = _productDS.Description;
                _newRecepy.Picture = GetBase64OfImage(ImagesPath + _productDS.Id.ToString() + ".jpg");
                _newRecepy.Rating = _productDS.Rating;
                // _newRecepy.
                _newRecepy.TimeCooking = decimal.Parse(_productDS.TimeCooking.ToString());
                _newRecepy.TimePreparing = decimal.Parse(_productDS.TimePreparing.ToString());
                _newRecepy.TimeReady = decimal.Parse(_productDS.TimeReady.ToString());
                _newRecepy.Yield = _productDS.Yield;

                //check if ingredient exists, and if not creat it
                foreach (var _ingredient in _productDS.IngredientList)
                {
                    Ingredients _ingreditentExists = db.Ingredients.First(c => c.Name.ToLower().Equals(_ingredient.Name));
                    if (_ingredient != null)
                    {
                        _newRecepy.RecipeIngredients.Add(new RecipeIngredients
                        {
                            IngredientId = _ingreditentExists.Id,
                            Units = _ingredient.Units,
                            Amount = _ingredient.Amount
                        });
                    }
                    else
                    {
                        Ingredients _newIngredient = new Ingredients();
                        _newIngredient.Name = _ingredient.Name;
                        db.SaveChanges();

                        //Add to recipe ingredients
                        _newRecepy.RecipeIngredients.Add(new RecipeIngredients
                        {
                            IngredientId = _newIngredient.Id,
                            Units = _ingredient.Units,
                            Amount = _ingredient.Amount
                        });
                    }
                }

                db.Recepies.Add(_newRecepy);
                db.SaveChanges();

                //direction
                int counter = 1;
                foreach (var direction in _productDS.Directions)
                {
                    db.RecipeDirections.Add(new RecipeDirections
                    {
                        RecipeId = _newRecepy.Id,
                        StepNumber = counter,
                        Direction = direction.Value
                    });
                    counter++;
                }
                db.SaveChanges();

            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return Json("dasdas", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportIngredients()
        {
            SpiroStockManagmentDatabaseClass.Procedures _SpiroDatabaseProcedures = new SpiroStockManagmentDatabaseClass.Procedures();

            //DataManager.ProductsManager _productsManager = new DataManager.ProductsManager();
            string a = HttpContext.Server.MapPath("~/App_Data/Ingredients.xml");
            _SpiroDatabaseProcedures.XmlDatabaseIngredientsPath = a;
            //_SpiroDatabaseProcedures.XmlDatabaseFilePath = a;
            _SpiroDatabaseProcedures.Initialize();

            //IEnumerable<XElement> _allProducts = _SpiroDatabaseProcedures.GetAllProducts();

            IEnumerable<XElement> _allIngredients = _SpiroDatabaseProcedures.GetAllIngredients();

            //string ImagesPath = @"C:\Users\ArdethMain\Dropbox\Spiro Stock Management 1.1\Databases\ItemsImages\";
            foreach (XElement _xElement in _allIngredients)
            {
                SpiroStockManagmentDatabaseClass.Objects.Ingredient _IngredientDS = new SpiroStockManagmentDatabaseClass.Objects.Ingredient();
                _IngredientDS = (SpiroStockManagmentDatabaseClass.Objects.Ingredient)XmlSerializerExtension.DeSerializerToObject(_xElement, _IngredientDS);

                Ingredients _newIngredient = new Ingredients();
                _newIngredient.Id = _IngredientDS.Id;
                _newIngredient.Name = _IngredientDS.Name;

                db.Ingredients.Add(_newIngredient);
                //_IngredientDS.
                //string _imagepath = ImagesPath + _productDS.PictureSmallFilename;
                //Products _newProduct = new Products();
                //_newProduct.Barcode = _productDS.BarCode;
                //_newProduct.CategoryString = _productDS.categoryString;
                //_newProduct.InsertDate = Convert.ToDateTime(_productDS.InsertDate);
                //_newProduct.Name = _productDS.Name;
                //_newProduct.Picture = GetBase64OfImage(_imagepath);
                //_newProduct.Price = _productDS.Price;
                //_newProduct.VariableWeightPrice = _productDS.VariableWeightPrice;


                //_productsManager.InsertProduct(_newProduct);
            }
            db.SaveChanges();

            return Json("dasdas", JsonRequestBehavior.AllowGet);
        }
    }
}