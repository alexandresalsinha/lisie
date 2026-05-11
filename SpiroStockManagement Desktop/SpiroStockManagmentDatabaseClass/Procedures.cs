using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.Reflection;
using System.IO;

namespace SpiroStockManagmentDatabaseClass
{
    public static class XmlSerializerExtension
    {
        public static XElement SerializeAsXElement(this XmlSerializer xs, object o)
        {
            XDocument d = new XDocument();
            using (XmlWriter w = d.CreateWriter()) xs.Serialize(w, o);
            XElement e = d.Root;
            e.Remove();
            return e;
        }

        public static Objects.Product DeSerializer(XElement element)
        {
            var serializer = new XmlSerializer(typeof(Objects.Product));
            return (Objects.Product)serializer.Deserialize(element.CreateReader());
        }

        public static object DeSerializerToObject(XElement element, object _object)
        {
            var serializer = new XmlSerializer(_object.GetType());
            return serializer.Deserialize(element.CreateReader());
        }
    }


    public class Procedures
    {

        public event EventHandler XmlDocumentSaved;
        protected virtual void OnXmlDocumentSaved(EventArgs e)
        {
            EventHandler eh = XmlDocumentSaved;
            if (eh != null)
            {
                eh(this, e);
            }
        }
        public XElement CopyObjectToXElement(object obj, XElement element)
        {
            PropertyInfo[] propertyInfos;
            //propertyInfos = typeof(Objects.Item).GetProperties(BindingFlags.Public | BindingFlags.);
            propertyInfos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                Console.WriteLine(propertyInfo.Name);
                try
                {
                    if (propertyInfo.GetValue(obj, null).ToString() != string.Empty)
                    {
                        
                        if(propertyInfo.PropertyType == typeof(bool))
                        {
                            element.Element(propertyInfo.Name).Value = element.Element(propertyInfo.Name).Value.ToLower();
                            continue;
                        }
                        if (propertyInfo.PropertyType == typeof(float))
                        {
                            //element.Element(propertyInfo.Name).Value = element.Element(propertyInfo.Name).Value.Replace(',', '.');
                            element.Element(propertyInfo.Name).Value = propertyInfo.GetValue(obj, null).ToString().Replace(',', '.');
                            continue;
                        }
                        //Ingredients List
                        if (propertyInfo.PropertyType == typeof(List<SpiroStockManagmentDatabaseClass.Objects.RecipeIngredient>))
                        {
                            string s = "";
                            List<Objects.RecipeIngredient> _listIngredients = (propertyInfo.GetValue(obj, null) as List<Objects.RecipeIngredient>);
                            if (_listIngredients.Count > 0)
                            {
                                element.Element(propertyInfo.Name).RemoveAll();
                                foreach (Objects.RecipeIngredient _Ingredient in _listIngredients)
                                {
                                    XmlSerializer xs = new XmlSerializer(typeof(Objects.RecipeIngredient));
                                    element.Element(propertyInfo.Name).Add(XmlSerializerExtension.SerializeAsXElement(xs, _Ingredient));
                                }
                            }
                            continue;
                        }
                        //Directions
                        if (propertyInfo.PropertyType == typeof(List<SpiroStockManagmentDatabaseClass.Objects.Step>))
                        {
                            List<Objects.Step> _listIngredients = (propertyInfo.GetValue(obj, null) as List<Objects.Step>);
                            if (_listIngredients.Count > 0)
                            {
                                element.Element(propertyInfo.Name).RemoveAll();
                                foreach (Objects.Step _Ingredient in _listIngredients)
                                {
                                    XmlSerializer xs = new XmlSerializer(typeof(Objects.Step));
                                    element.Element(propertyInfo.Name).Add(XmlSerializerExtension.SerializeAsXElement(xs, _Ingredient));
                                }
                            }
                            continue;
                        }
                        if (propertyInfo.PropertyType == typeof(List<SpiroStockManagmentDatabaseClass.Objects.Item>))
                        {
                            List<Objects.Item> _listItems = (propertyInfo.GetValue(obj, null) as List<Objects.Item>);
                            if (_listItems.Count > 0)
                            {
                                element.Element(propertyInfo.Name).RemoveAll();
                                

                                foreach (Objects.Item _item in _listItems)
                                {
                                    XElement _itemXElement = new XElement("Item");
                                    PropertyInfo[] _ItemPropertyInfos = _item.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                    foreach (PropertyInfo _ItemPropertyInfo in _ItemPropertyInfos)
                                    {
                                        if (_ItemPropertyInfo.PropertyType == typeof(float))
                                        {
                                            //element.Element(propertyInfo.Name).Value = element.Element(propertyInfo.Name).Value.Replace(',', '.');
                                            _itemXElement.Add(new XElement(_ItemPropertyInfo.Name, _ItemPropertyInfo.GetValue(_item, null).ToString().Replace(',', '.')));
                                            continue;
                                        }
                                        _itemXElement.Add(new XElement(_ItemPropertyInfo.Name, _ItemPropertyInfo.GetValue(_item, null).ToString()));
                                    }
                                   element.Element(propertyInfo.Name).Add(_itemXElement);
                                }
                                
                            }
                            else
                            {
                                element.Element(propertyInfo.Name).Value = "";
                            } 
                            continue;
                        }
                        //Ingredients
                        //IngredientProduct
                        if (propertyInfo.PropertyType == typeof(List<SpiroStockManagmentDatabaseClass.Objects.IngredientProduct>))
                        {
                            List<Objects.IngredientProduct> _listIngredients = (propertyInfo.GetValue(obj, null) as List<Objects.IngredientProduct>);
                            if (_listIngredients.Count > 0)
                            {
                                element.Element(propertyInfo.Name).RemoveAll();
                                foreach (Objects.IngredientProduct _Ingredient in _listIngredients)
                                {
                                    XmlSerializer xs = new XmlSerializer(typeof(Objects.IngredientProduct));
                                    element.Element(propertyInfo.Name).Add(XmlSerializerExtension.SerializeAsXElement(xs, _Ingredient));
                                }
                            }
                            continue;
                        }


                        //if property is not of the type of any of the above
                        //just copy the value, uf element soes not exist create it

                        if(element.Element(propertyInfo.Name) == null)
                            element.Add(new XElement(propertyInfo.Name, propertyInfo.GetValue(obj, null).ToString()));
                        else
                            element.Element(propertyInfo.Name).Value = propertyInfo.GetValue(obj, null).ToString();
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return element;
        }

        public string XmlDatabaseFilePath, XmlDatabaseRecepiesPath, XmlDatabaseIngredientsPath = string.Empty;
        public XDocument XmlDocument = null;
        public XDocument XmlDocumentRecepies = null;
        public XDocument XmlDocumentIngredients = null;
        
        public bool RecepiesXmlFileChangedByThisApplication = false;
        public void Initialize()
        {
            XmlDocument = GetXmlDocument(XmlDatabaseFilePath);
            XmlDocumentRecepies = GetXmlDocument(XmlDatabaseRecepiesPath);
            XmlDocumentIngredients = GetXmlDocument(XmlDatabaseIngredientsPath);
        }

        public void InitializeXmlDatabaseFile()
        {
            XmlDocument = GetXmlDocument(XmlDatabaseFilePath);
        }
        
        public void InitializeRecepies()
        {
            XmlDocumentRecepies = GetXmlDocument(XmlDatabaseRecepiesPath);
        }

        public XDocument GetXmlDocument()
        {
            if (XmlDatabaseFilePath == string.Empty) return null;
            try
            {
                XDocument doc = XDocument.Load(XmlDatabaseFilePath);
                return doc;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public XDocument GetXmlDocument(string xmlPath)
        {
            if (xmlPath == string.Empty) return null;
            try
            {
                XDocument doc = XDocument.Load(xmlPath);
                return doc;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void LoadXmlDocumentFromString(string xml)
        {
            TextReader tr = new StringReader(xml);
            XmlDocument = XDocument.Load(tr);
        }

        public void SaveXmlDocument(XDocument doc, string xmlFilePath)
        {
            if (xmlFilePath == string.Empty) return;
            doc.Save(xmlFilePath);
        }
        public void SaveXmlRecepiesDocument()
        {
            if (XmlDatabaseRecepiesPath == string.Empty) return;
            RecepiesXmlFileChangedByThisApplication = true;
            XmlDocumentRecepies.Save(XmlDatabaseRecepiesPath);
        }
        public void SaveXmlIngredientsDocument()
        {
            if (XmlDatabaseIngredientsPath == string.Empty) return;
            XmlDocumentIngredients.Save(XmlDatabaseIngredientsPath);
        }
        public void SaveXmlDocument()
        {
            if (XmlDatabaseFilePath == string.Empty) return;
            XmlDocument.Save(XmlDatabaseFilePath);

            OnXmlDocumentSaved(new EventArgs());
        }

        public void SaveXmlDocument(XDocument doc)
        {
            return;
        } 

        /// <summary>
        /// /
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int InsertNewItem(Objects.Product item)
        {
            int _itemID = 0;

            //var _alreadyExists = from c in XmlDocument.Descendants("Item") where c.Element("Name").Value.ToLower() == item.Name.ToLower() && c.Element("Brand").Value.ToLower() == item.Brand.ToLower() select c;
           
            //if exists
            if (item.Id != 0)
            {
                var _alreadyExists = from c in XmlDocument.Descendants("Product") where c.Element("Id").Value == item.Id.ToString() select c;
                if (_alreadyExists.Any())
                {
                    XElement _foundedItem = _alreadyExists.ElementAt(0);
                    Objects.Product _updateItem = XmlSerializerExtension.DeSerializer(_foundedItem);


                    //if (item.Quantity > 0)
                    //{
                    //    _updateItem.Quantity += item.Quantity;
                    //    //_foundedItem.Element("Quantity").Value = _updateItem.Quantity.ToString();
                    //}

                    ////Item Quantity Weight
                    //if (item.QuantityWeight > 0)
                    //{
                    //    try
                    //    {
                    //        _updateItem.QuantityWeight = _updateItem.QuantityWeight + item.QuantityWeight;
                    //    }
                    //    catch (Exception ex) { }
                    //}

                    //reflection copy object to XElement
                    _foundedItem = CopyObjectToXElement(item, _foundedItem);


                    _itemID = _updateItem.Id; 
                }
            }
            //if not create the item
            else
            {
                int _lastID = GetLastID();
                XmlSerializer xs = new XmlSerializer(typeof(Objects.Product));
                item.Id = ++_lastID;
                XElement _xs = xs.SerializeAsXElement(item);
                XmlDocument.Root.Add(_xs);
                _itemID = ++_lastID;
            }

            SaveXmlDocument();

            return _itemID;
        }

        public IEnumerable<XElement> GetAllOutItems()
        {
            //XDocument doc = GetXmlDocument();
            //var _queryItems = from c in XmlDocument.Descendants("Product")
            //                  where c.Element("ItemLists")
            //                         .Elements("Item").Any(x => x.Element("ListName").Value == "out")
            //                  select c;
            var _queryItems = from c in XmlDocument.Descendants("Product")
                              where int.Parse(c.Element("QuantityOut").Value.ToString()) > 0 || float.Parse(c.Element("QuantityWeightOut").Value.ToString()) > 0
                              select c;
            if (_queryItems.Any())
            {
                return _queryItems;
            }
            return (IEnumerable<XElement>)new List<XElement>();
        }

        public IEnumerable<XElement> GetAllInItems()
        {
            //XDocument doc = GetXmlDocument();
            //var _queryItems = from c in XmlDocument.Descendants("Product")
            //                  where c.Element("ItemLists")
            //                         .Elements("Item").Any(x => x.Element("ListName").Value == "in")
            //                  select c;
            var _queryItems = from c in XmlDocument.Descendants("Product")
                              where int.Parse(c.Element("QuantityIn").Value.ToString()) > 0 || float.Parse(c.Element("QuantityWeightIn").Value.ToString()) > 0
                              select c;
            if (_queryItems.Any())
            {
                return _queryItems;
            }

            return (IEnumerable<XElement>)new List<XElement>();
        }

        public IEnumerable<XElement> GetAllProducts()
        {
            //XDocument doc = GetXmlDocument();
            var _queryItems = from c in XmlDocument.Descendants("Product")
                              select c;
            if (_queryItems.Any())
            {
                return _queryItems;
            }

            return (IEnumerable<XElement>)new List<XElement>();
        }

        public Objects.Product GetItemByBarCode(string barcode)
        {
            var _alreadyExists = from c in XmlDocument.Descendants("Product") where c.Element("BarCode").Value == barcode select c;

            //if exists
            if (_alreadyExists.Any())
            {
                XElement _founded = _alreadyExists.ElementAt(0);
                //_foundedSong.Element("album").Value = album;
                //_foundedSong.Attribute("loved").Value = loved.ToString();
                return XmlSerializerExtension.DeSerializer(_founded);
            }
            return null;
        }

        public bool UpdateItemInOpossiteList(Objects.Product item)
        {
            string _typeOfOperationToSearch = "";
            //if (item.TypeOfOperation == "in")
            //    _typeOfOperationToSearch = "out";
            //else _typeOfOperationToSearch = "in";

            XElement _productX = GetItem(item.Id);
            //var _alreadyExists = from c in XmlDocument.Descendants("Product") where c.Element("Id").Value == item.Id.ToString() select c;

            //if exists
            if (_productX != null)
            {
                Objects.Product _updateItem = XmlSerializerExtension.DeSerializer(_productX);


                //if (item.Quantity > 0)
                //{
                //    if (_updateItem.Quantity - item.Quantity > 0)
                //        _updateItem.Quantity -= item.Quantity;
                //    else _updateItem.Quantity = 0;
                //    //_foundedItem.Element("Quantity").Value = _updateItem.Quantity.ToString();
                //}

                ////Item Quantity Weight
                //if (item.QuantityWeight > 0)
                //{
                //    try
                //    {
                //        if (_updateItem.QuantityWeight - item.QuantityWeight > 0)
                //            _updateItem.QuantityWeight = _updateItem.QuantityWeight - item.QuantityWeight;
                //        //if result is minus than 0, add to the opossite list
                //        else
                //        {
                //            _updateItem.Quantity = 0;
                //        }
                //    }
                //    catch (Exception ex) { }
                //}

                //reflection copy object to XElement
                _productX = CopyObjectToXElement(_updateItem, _productX);
            }
            SaveXmlDocument();
            return true;
        }

        public bool DeleteProduct(int ID)
        {
            XElement _Item = GetItem(ID);
            if (_Item != null)
            {
                _Item.Remove();
                SaveXmlDocument();
                return true;
            }
            return false;
        }

        public bool DeleteItemFromList(int ID, string listName)
        {
            XElement _Item = GetItem(ID);
            if (_Item != null)
            {
                Objects.Product _updateItem = XmlSerializerExtension.DeSerializer(_Item);

                //var _query = from c in _updateItem.ItemLists where c.ListName == listName select c;
                //if (_query.Any())
                //{
                //    _updateItem.ItemLists.Remove((_query.First() as SpiroStockManagmentDatabaseClass.Objects.Item));
                //}
                if (listName.ToLower() == "in")
                {
                    _updateItem.QuantityIn = 0;
                    _updateItem.QuantityWeightIn = 0;
                }
                else
                {
                    _updateItem.QuantityOut = 0;
                    _updateItem.QuantityWeightOut = 0;
                }

                _Item = CopyObjectToXElement(_updateItem, _Item);
                SaveXmlDocument();
                return true;
            }
            return false;
        }

        public int GetLastID()
        {
            //XDocument doc = GetXmlDocument();
            try
            {
                int max = (int)XmlDocument.Descendants("Product").Descendants("Id").Select(c => (int)c).Max();
                return max;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public XElement GetItem(int ID)
        {
            //XDocument doc = GetXmlDocument();
            var _alreadyExists = from c in XmlDocument.Descendants("Product") where c.Element("Id").Value == ID.ToString() select c;
            if (_alreadyExists.Any())
            {
                XElement _founded = _alreadyExists.ElementAt(0);
                return _founded;
            }
            else return null;
        }

        public Objects.Product GetProduct(int ID)
        {
            //XDocument doc = GetXmlDocument();
            var _alreadyExists = from c in XmlDocument.Descendants("Product") where c.Element("Id").Value == ID.ToString() select c;
            if (_alreadyExists.Any())
            {
                XElement _foundedItem = _alreadyExists.ElementAt(0);
                return XmlSerializerExtension.DeSerializer(_foundedItem);
            }
            else return null;
        }


        public List<string> GetAllProductCategories()
        {
            var tags = XmlDocument.Descendants("Product").Elements("categoryString")
                           .Select(c => (string)c)
                           .Distinct()
                           .OrderBy(c => c);
            return tags.ToList();
        }

        public List<string> GetAllProductCategoriesOutList()
        {
            var _queryItems = (from c in XmlDocument.Descendants("Product")
                               where int.Parse(c.Element("QuantityOut").Value.ToString()) > 0 || float.Parse(c.Element("QuantityWeightOut").Value.ToString()) > 0
                               select c.Element("categoryString").Value).Distinct().OrderBy(c => c);

            return _queryItems.ToList();
        }

        public List<string> GetAllProductCategoriesInList()
        {
            var _queryItems = (from c in XmlDocument.Descendants("Product")
                               where int.Parse(c.Element("QuantityIn").Value.ToString()) > 0 || float.Parse(c.Element("QuantityWeightIn").Value.ToString()) > 0
                               select c.Element("categoryString").Value).Distinct().OrderBy(c => c);

            return _queryItems.ToList();
        }


        //public List<string> GetAutocompleteTextboxDate()
        //{
        //    var _queryItems = from c in XmlDocument.Descendants("Product")
        //                      select c.Element("Name").Value + " (" + c.Element("Id").Value + ")";
        //    if (_queryItems.Any())
        //    {
        //        return _queryItems.ToList();
        //    }
        //    else return new List<string>();
        //}

        public List<Objects.AutoCompleteProductData> GetAutocompleteTextboxDate()
        {
            var _queryItems = from c in XmlDocument.Descendants("Product")
                              select new Objects.AutoCompleteProductData { ProductId = int.Parse(c.Element("Id").Value), ProductName = c.Element("Name").Value, ProductBrand = c.Element("Brand").Value };
            if (_queryItems.Any())
            {
                return (List<Objects.AutoCompleteProductData>)_queryItems.ToList();
            }
            else return new List<Objects.AutoCompleteProductData>();
        }




        //Recepies

        public IEnumerable<XElement> GetAllRecepies()
        {
            //XDocument doc = GetXmlDocument();
            var _queryItems = from c in XmlDocumentRecepies.Descendants("Recipe")
                              select c;
            if (_queryItems.Any())
            {
                return _queryItems;
            }

            return (IEnumerable<XElement>)new List<XElement>();
        }

        public IEnumerable<XElement> GetCategoryRecepies(string category)
        {
            //XDocument doc = GetXmlDocument();
            var _queryItems = from c in XmlDocumentRecepies.Descendants("Recipe") where c.Element("Category").Value.ToLower() == category.ToLower()
                              select c;
            if (_queryItems.Any())
            {
                return _queryItems;
            }

            return (IEnumerable<XElement>)new List<XElement>();
        }

        public IEnumerable<XElement> GetCuisineRecepies(string cuisine)
        {
            //XDocument doc = GetXmlDocument();
            var _queryItems = from c in XmlDocumentRecepies.Descendants("Recipe")
                              where c.Element("Cuisine").Value.ToLower() == cuisine.ToLower()
                              select c;
            if (_queryItems.Any())
            {
                return _queryItems;
            }

            return (IEnumerable<XElement>)new List<XElement>();
        }

        public IEnumerable<XElement> GetWithIngredientInRecepies(string ingredient)
        {
            //var _query = from c in XmlDocument.Descendants("music").Elements("tags").Elements("tag") where c.Value.ToLower() == tag.ToLower() select c;
            var _queryItems = from c in XmlDocumentRecepies.Descendants("Recipe").Elements("IngredientList").Elements("RecipeIngredient").Elements("Name") where c.Value.ToLower() == ingredient.ToLower() select c.Parent.Parent.Parent;

            //XDocument doc = GetXmlDocument();
            //var _queryItems = from c in XmlDocumentRecepies.Descendants("Recipe")
            //                  where c.Element("IngredientList").Element("RecipeIngredient").Element("Name").Value.ToLower() == ingredient.ToLower()
            //                  select c;
            if (_queryItems.Any())
            {
                return _queryItems;
            }

            return (IEnumerable<XElement>)new List<XElement>();
        }

        public Objects.Recipe GetRecipe(int ID)
        {
            //XDocument doc = GetXmlDocument();
            var _alreadyExists = from c in XmlDocumentRecepies.Descendants("Recipe") where c.Element("Id").Value == ID.ToString() select c;
            if (_alreadyExists.Any())
            {
                XElement _foundedItem = _alreadyExists.ElementAt(0);
                return (Objects.Recipe)XmlSerializerExtension.DeSerializerToObject(_foundedItem, new Objects.Recipe());
            }
            else return null;
        }

        public XElement GetXRecipe(int ID)
        {
            //XDocument doc = GetXmlDocument();
            var _alreadyExists = from c in XmlDocumentRecepies.Descendants("Recipe") where c.Element("Id").Value == ID.ToString() select c;
            if (_alreadyExists.Any())
            {
                XElement _founded = _alreadyExists.ElementAt(0);
                return _founded;
            }
            else return null;
        }

        public int GetLastRecipeID()
        {
            //XDocument doc = GetXmlDocument();
            try
            {
                int max = (int)XmlDocumentRecepies.Descendants("Recipe").Descendants("Id").Select(c => (int)c).Max();
                return max;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<string> GetRecipeCategoriesDistinct()
        {
            var tags = XmlDocumentRecepies.Descendants("Recipe").Elements("Category")
                           .Select(c => (string)c)
                           .Distinct()
                           .OrderBy(c => c);
            return tags.ToList();
        }

        public List<string> GetRecipeCuisineDisctinct()
        {
            var tags = XmlDocumentRecepies.Descendants("Recipe").Elements("Cuisine")
                           .Select(c => (string)c)
                           .Distinct()
                           .OrderBy(c => c);
            return tags.ToList();
        }

        public int InsertEditRecipe(Objects.Recipe recipe)
        {
            int _itemID = 0;

            //var _alreadyExists = from c in XmlDocument.Descendants("Item") where c.Element("Name").Value.ToLower() == item.Name.ToLower() && c.Element("Brand").Value.ToLower() == item.Brand.ToLower() select c;

            //if exists
            if (recipe.Id != 0)
            {
                var _alreadyExists = from c in XmlDocumentRecepies.Descendants("Recipe") where c.Element("Id").Value == recipe.Id.ToString() select c;
                if (_alreadyExists.Any())
                {
                    XElement _foundedItem = _alreadyExists.ElementAt(0);
                    Objects.Recipe _updateItem = new Objects.Recipe();
                    _updateItem = (Objects.Recipe)XmlSerializerExtension.DeSerializerToObject(_foundedItem, _updateItem);

                    //reflection copy object to XElement
                    _foundedItem = CopyObjectToXElement(recipe, _foundedItem);


                    _itemID = _updateItem.Id;
                }
            }
            //if not create the item
            else
            {
                int _lastID = GetLastRecipeID();
                XmlSerializer xs = new XmlSerializer(typeof(Objects.Recipe));
                recipe.Id = ++_lastID;
                XElement _xs = xs.SerializeAsXElement(recipe);
                XmlDocumentRecepies.Root.Add(_xs);
                _itemID = ++_lastID;
            }

            SaveXmlRecepiesDocument();

            return _itemID;
        }

        public bool DeleteRecipe(int ID)
        {
            XElement _Item = GetXRecipe(ID);
            if (_Item != null)
            {
                _Item.Remove();
                SaveXmlRecepiesDocument();
                return true;
            }
            return false;
        }

        public IEnumerable<XElement> GetRecipiesWithText(string text)
        {
            var _queryItems = from c in XmlDocumentRecepies.Descendants("Recipe")
                              where c.Value.ToLower().IndexOf(text.ToLower()) > -1
                              select c;
            if (_queryItems.Any())
            {
                return _queryItems;
            }

            return (IEnumerable<XElement>)new List<XElement>();
        }


        //Ingredients

        public IEnumerable<XElement> GetAllIngredients()
        {
            var _queryItems = (from c in XmlDocumentIngredients.Descendants("Ingredient")
                               select c).OrderBy(c => c.Element("Name").Value);


            if (_queryItems.Any())
            {
                return _queryItems;
            }

            return (IEnumerable<XElement>)new List<XElement>();
        }

        public List<string> GetAllIngredientsNamesAlphabetcly()
        {
            

            var _queryItems = XmlDocumentIngredients.Descendants("Ingredient").Elements("Name").Select(c => (string)c).OrderBy(c => c);


            if (_queryItems.Any())
            {
                return _queryItems.ToList<string>();
            }

            return (List<string>)new List<string>();
        }

        public IEnumerable<XElement> GetAllIngredientUnitys()
        {
            var _queryItems = (from c in XmlDocumentIngredients.Descendants("Ingredient")
                               select c).OrderBy(c => c.Element("Name").Value);


            if (_queryItems.Any())
            {
                return _queryItems;
            }

            return (IEnumerable<XElement>)new List<XElement>();
        }

        public List<string> GetAllProductIngredientUnits()
        {
            var Units = XmlDocumentRecepies.Descendants("Recepies").Descendants("IngredientList").Elements("RecipeIngredient").Elements("Units")
                           .Select(c => (string)c)
                           .Distinct()
                           .OrderBy(c => c);
            return Units.ToList();
        }

        //public List<string> GetAllProductCategoriesOutList()
        //{
        //    var _queryItems = (from c in XmlDocument.Descendants("Product")
        //                       where int.Parse(c.Element("QuantityOut").Value.ToString()) > 0 || float.Parse(c.Element("QuantityWeightOut").Value.ToString()) > 0
        //                       select c.Element("categoryString").Value).Distinct().OrderBy(c => c);

        //    return _queryItems.ToList();
        //}

        public Objects.Ingredient GetIngredient(int ID)
        {
            //XDocument doc = GetXmlDocument();
            var _alreadyExists = from c in XmlDocumentIngredients.Descendants("Ingredient") where c.Element("Id").Value == ID.ToString() select c;
            if (_alreadyExists.Any())
            {
                XElement _foundedItem = _alreadyExists.ElementAt(0);
                return (Objects.Ingredient)XmlSerializerExtension.DeSerializerToObject(_foundedItem, new Objects.Ingredient());
            }
            else return null;
        }

        public Objects.Ingredient GetIngredient(string name)
        {
            //XDocument doc = GetXmlDocument();
            var _alreadyExists = from c in XmlDocumentIngredients.Descendants("Ingredient") where c.Element("Name").Value.ToLower() == name.ToLower() select c;
            if (_alreadyExists.Any())
            {
                XElement _foundedItem = _alreadyExists.ElementAt(0);
                return (Objects.Ingredient)XmlSerializerExtension.DeSerializerToObject(_foundedItem, new Objects.Ingredient());
            }
            else return null;
        }

        public XElement GetXIngredient(int ID)
        {
            //XDocument doc = GetXmlDocument();
            var _alreadyExists = from c in XmlDocumentIngredients.Descendants("Ingredient") where c.Element("Id").Value == ID.ToString() select c;
            if (_alreadyExists.Any())
            {
                XElement _founded = _alreadyExists.ElementAt(0);
                return _founded;
            }
            else return null;
        }

        public int GetLastIngredientID()
        {
            //XDocument doc = GetXmlDocument();
            try
            {
                int max = (int)XmlDocumentIngredients.Descendants("Ingredient").Elements("Id").Select(c => (int)c).Max();
                return max;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int InsertEditIngredient(Objects.Ingredient ingredient)
        {
            int _itemID = 0;

            //var _alreadyExists = from c in XmlDocument.Descendants("Item") where c.Element("Name").Value.ToLower() == item.Name.ToLower() && c.Element("Brand").Value.ToLower() == item.Brand.ToLower() select c;

            //if exists
            if (ingredient.Id != 0)
            {
                var _alreadyExists = from c in XmlDocumentIngredients.Descendants("Ingredient") where c.Element("Id").Value == ingredient.Id.ToString() select c;
                if (_alreadyExists.Any())
                {
                    XElement _foundedItem = _alreadyExists.ElementAt(0);
                    Objects.Ingredient _updateItem = new Objects.Ingredient();
                    _updateItem = (Objects.Ingredient)XmlSerializerExtension.DeSerializerToObject(_foundedItem, _updateItem);

                    //reflection copy object to XElement
                    _foundedItem = CopyObjectToXElement(ingredient, _foundedItem);


                    _itemID = _updateItem.Id;
                }
            }
            //if not create the item ,  BUT FIRST CHECK IF A INGREDIENT WITH THE SAME NAME EXISTS
            else
            {
                var _alreadyExists = from c in XmlDocumentIngredients.Descendants("Ingredient") where c.Element("Name").Value.ToLower() == ingredient.Name.ToString().ToLower() select c;
                if (_alreadyExists.Any())
                {
                    XElement _foundedItem = _alreadyExists.ElementAt(0);
                    return int.Parse(_foundedItem.Element("Id").Value);
                }

                int _lastID = GetLastIngredientID();
                XmlSerializer xs = new XmlSerializer(typeof(Objects.Ingredient));
                ingredient.Id = ++_lastID;
                XElement _xs = xs.SerializeAsXElement(ingredient);
                XmlDocumentIngredients.Root.Add(_xs);
                _itemID = ++_lastID; 
            }

            SaveXmlIngredientsDocument();

            return _itemID;
        }

        public bool DeleteIngredient(int ID)
        {
            XElement _Item = GetXIngredient(ID);
            if (_Item != null)
            {
                _Item.Remove();
                SaveXmlIngredientsDocument();
                return true;
            }
            return false;
        }

        public bool CheckIfIngredientExists(string name)
        {
            var _alreadyExists = from c in XmlDocumentIngredients.Descendants("Ingredient") where c.Element("Name").Value.ToLower() == name.ToLower() select c;
            if (_alreadyExists.Any())
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// ////////
        /// ///////7
        /// ///////7
        /// ///////////






        //public int InsertNewMusic(string artist, string title, string album, bool loved)
        //{
        //    //XDocument doc = GetXmlDocument();
        //    int songID = 0;

        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("title").Value.ToLower() == title.ToLower() && c.Element("artist").Value.ToLower() == artist.ToLower() select c;

        //    //if exists
        //    if (_alreadyExists.Any())
        //    {
        //        XElement _foundedSong = _alreadyExists.ElementAt(0);
        //        _foundedSong.Element("album").Value = album;
        //        _foundedSong.Attribute("loved").Value = loved.ToString();
        //        songID = int.Parse(_foundedSong.Attribute("ID").Value.ToString());
        //    }
        //    //if not create the song
        //    else
        //    {
        //        int _lastSongID = GetLastSongID();
        //        XmlDocument.Root.Add(new XElement("music",
        //            new XAttribute("ID", ++_lastSongID),
        //            new XAttribute("loved", loved),
        //            new XElement("artist", artist),
        //            new XElement("title", title),
        //            new XElement("album", album),
        //            new XElement("genre", ""),
        //            new XElement("year", ""),
        //            new XElement("filename", ""),
        //            new XElement("tags", ""),
        //            new XElement("bookmarks", ""),
        //            new XElement("playlists", ""),
        //            new XElement("note", "")
        //        ));
        //        songID = _lastSongID;
        //    }

        //    SaveXmlDocument(XmlDocument);
        //    return songID;
        //}

        //public int InsertNewMusic(string artist, string title, string album, string genre, string year, bool loved)
        //{
        //    //XDocument doc = GetXmlDocument();
        //    int songID = 0;

        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("title").Value.ToLower() == title.ToLower() && c.Element("artist").Value.ToLower() == artist.ToLower() select c;

        //    //if exists
        //    if (_alreadyExists.Any())
        //    {
        //        XElement _foundedSong = _alreadyExists.ElementAt(0);
        //        _foundedSong.Element("album").Value = album;
        //        _foundedSong.Element("genre").Value = genre;
        //        _foundedSong.Element("year").Value = album;
        //        _foundedSong.Attribute("loved").Value = loved.ToString();
        //        songID = int.Parse(_foundedSong.Attribute("ID").Value.ToString());
        //    }
        //    //if not create the song
        //    else
        //    {
        //        int _lastSongID = GetLastSongID();
        //        XmlDocument.Root.Add(new XElement("music",
        //            new XAttribute("ID", ++_lastSongID),
        //            new XAttribute("loved", loved),
        //            new XElement("artist", artist),
        //            new XElement("title", title),
        //            new XElement("album", album),
        //            new XElement("genre", genre),
        //            new XElement("year", year),
        //            new XElement("filename", ""),
        //            new XElement("tags", ""),
        //            new XElement("bookmarks", ""),
        //            new XElement("playlists", ""),
        //            new XElement("note", "")
        //        ));
        //        songID = _lastSongID;
        //    }

        //    SaveXmlDocument(XmlDocument);
        //    return songID;
        //}

        //public int InsertNewMusic(string artist, string title, string album, string genre, string year, bool loved, string filename)
        //{
        //    //XDocument doc = GetXmlDocument();
        //    int songID = 0;

        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("title").Value.ToLower() == title.ToLower() && c.Element("artist").Value.ToLower() == artist.ToLower() select c;

        //    //if exists
        //    if (_alreadyExists.Any())
        //    {
        //        XElement _foundedSong = _alreadyExists.ElementAt(0);
        //        _foundedSong.Element("album").Value = album;
        //        _foundedSong.Element("genre").Value = album;
        //        _foundedSong.Element("year").Value = album;
        //        _foundedSong.Element("filename").Value = filename;
        //        _foundedSong.Attribute("loved").Value = loved.ToString();
        //        songID = int.Parse(_foundedSong.Attribute("ID").Value.ToString());
        //    }
        //    //if not create the song
        //    else
        //    {
        //        int _lastSongID = GetLastSongID();
        //        XmlDocument.Root.Add(new XElement("music",
        //            new XAttribute("ID", ++_lastSongID),
        //            new XAttribute("loved", loved),
        //            new XElement("artist", artist),
        //            new XElement("title", title),
        //            new XElement("album", album),
        //            new XElement("genre", genre),
        //            new XElement("year", year),
        //            new XElement("filename", filename),
        //            new XElement("tags", ""),
        //            new XElement("bookmarks", ""),
        //            new XElement("playlists", ""),
        //            new XElement("note", "")
        //        ));
        //        songID = _lastSongID;
        //    }

        //    SaveXmlDocument(XmlDocument);
        //    return songID;
        //}

        //public int InsertNewMusic(string artist, string title, string album, string genre, string year)
        //{
        //    //XDocument doc = GetXmlDocument();
        //    int songID = 0;

        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("title").Value.ToLower() == title.ToLower() && c.Element("artist").Value.ToLower() == artist.ToLower() select c;

        //    //if exists
        //    if (_alreadyExists.Any())
        //    {
        //        XElement _foundedSong = _alreadyExists.ElementAt(0);
        //        _foundedSong.Element("album").Value = album;
        //        _foundedSong.Element("genre").Value = genre;
        //        _foundedSong.Element("year").Value = year;
        //        songID = int.Parse(_foundedSong.Attribute("ID").Value.ToString());
        //    }
        //    //if not create the song
        //    else
        //    {
        //        int _lastSongID = GetLastSongID();
        //        XmlDocument.Root.Add(new XElement("music",
        //            new XAttribute("ID", ++_lastSongID),
        //            new XAttribute("loved", false),
        //            new XElement("artist", artist),
        //            new XElement("title", title),
        //            new XElement("album", album),
        //            new XElement("genre", genre),
        //            new XElement("year", year),
        //            new XElement("filename", ""),
        //            new XElement("tags", ""),
        //            new XElement("bookmarks", ""),
        //            new XElement("playlists", ""),
        //            new XElement("note", "")
        //        ));
        //        songID = _lastSongID;
        //    }

        //    SaveXmlDocument(XmlDocument);
        //    return songID;
        //}

        //public bool UpdateMusic(int songId, string artist, string title, string album, string genre, string year, bool loved)
        //{
        //    XElement _song = GetSong(songId);
        //    if (_song != null)
        //    {
        //        _song.Element("artist").Value = artist;
        //        _song.Element("title").Value = title;
        //        _song.Element("album").Value = album;
        //        _song.Element("genre").Value = genre;
        //        _song.Element("year").Value = year;
        //        _song.Attribute("loved").Value = loved.ToString();
        //        SaveXmlDocument();
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public int UpdateMusic(string artist, string title, string album, string genre, string year)
        //{
        //    return InsertNewMusic(artist, title, album, genre, year);
        //}

        //public bool UpdateMusicLovedAttribute(string artist, string title, bool loved)
        //{
        //    XElement _song = ReadSong(artist, title);
        //    if (_song != null)
        //    {
        //        _song.Attribute("loved").Value = loved.ToString();
        //        SaveXmlDocument(XmlDocument);
        //        return true;
        //    }
        //    return false;
        //}

        //public XElement ReadSong(string artist, string title)
        //{
        //    //XDocument doc = GetXmlDocument();
        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("title").Value.ToLower() == title.ToLower() && c.Element("artist").Value.ToLower() == artist.ToLower() select c;
        //    if (_alreadyExists.Any())
        //    {
        //        XElement _foundedSong = _alreadyExists.ElementAt(0);
        //        return _foundedSong;
        //    }
        //    else return null;
        //}
        //public bool SongExists(string artist, string title)
        //{
        //    //XDocument doc = GetXmlDocument();
        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("title").Value.ToLower() == title.ToLower() && c.Element("artist").Value.ToLower() == artist.ToLower() select c;
        //    if (_alreadyExists.Any())
        //    {
        //        return true;
        //    }
        //    else return false;
        //}

        //public XElement GetSong(int ID)
        //{
        //    //XDocument doc = GetXmlDocument();
        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Attribute("ID").Value == ID.ToString() select c;
        //    if (_alreadyExists.Any())
        //    {
        //        XElement _foundedSong = _alreadyExists.ElementAt(0);
        //        return _foundedSong;
        //    }
        //    else return null;
        //}

        //public bool DeleteSong(int ID)
        //{
        //    XElement _song = GetSong(ID);
        //    if (_song != null)
        //    {
        //        _song.Remove();
        //        SaveXmlDocument();
        //        return true;
        //    }
        //    return false;
        //}




        //public List<string> GetAllTagValuesDistinct()
        //{
        //    //XDocument doc = GetXmlDocument();
        //    if (XmlDocument != null)
        //    {

        //        var _tags = from c in XmlDocument.Descendants("music").Elements("tags").Elements("tag") select c;

        //        foreach (XElement _tag in _tags)
        //        {
        //            string s = _tag.Value;
        //        }

        //        var tags = XmlDocument.Descendants("music").Elements("tags").Elements("tag")
        //                   .Select(c => (string)c)
        //                   .Distinct()
        //                   .OrderBy(c => c);
        //        return tags.ToList();
        //    }
        //    return new List<string>();
        //}

        //public List<string> GetAllTagsValueOfSongDistinct(string artist, string title)
        //{
        //    //XDocument doc = GetXmlDocument();

        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("title").Value.ToLower() == title.ToLower() && c.Element("artist").Value.ToLower() == artist.ToLower() select c;
        //    if (_alreadyExists.Any())
        //    {
        //        var _tags = from c in _alreadyExists.Elements("tags").Elements("tag") select c;
        //        List<string> _list = new List<string>();
        //        foreach (XElement _tag in _tags)
        //        {
        //            _list.Add(_tag.Value.ToString());
        //        }
        //        return _list;
        //    }
        //    return null;
        //}

        ////public bool InsertTagToMusic(string artist, string title, string tag, string album, string filepath)
        ////{
        ////    //XDocument doc = GetXmlDocument();

        ////    var _alreadyExists = from c in doc.Descendants("music") where c.Element("title").Value.ToLower() == title.ToLower() && c.Element("artist").Value.ToLower() == artist.ToLower() select c;

        ////    //if exists add a tag
        ////    if (_alreadyExists.Any())
        ////    {
        ////        //TODO : check first if the tag exists
        ////        foreach (XElement _song in _alreadyExists)
        ////        {
        ////            _song.Element("tags").Add(new XElement("tag", tag));
        ////        }
        ////        //XElement _foundedSong = _alreadyExists.ElementAt(0);
        ////        //_foundedSong.Element("tags").Add(new XElement("tag", tag));
        ////    }
        ////    //if not create the song
        ////    else
        ////    {
        ////        int _lastSongID = GetLastSongID();
        ////        doc.Root.Add(new XElement("music",
        ////            new XAttribute("ID", ++_lastSongID),
        ////            new XAttribute("loved", "false"),
        ////            new XElement("artist", artist),
        ////            new XElement("title", title),
        ////            new XElement("album", album),
        ////            new XElement("filepath", filepath),
        ////            new XElement("tags",
        ////                new XElement("tag", tag)
        ////            ),
        ////            new XElement("bookmarks", ""),
        ////            new XElement("playlists", ""),
        ////            new XElement("note", "")
        ////        ));
        ////    }

        ////    SaveXmlDocument(doc);
        ////    return true;
        ////}
        //public bool InsertTagToMusic(int ID, string tag)
        //{
        //    //XDocument doc = GetXmlDocument();

        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Attribute("ID").Value == ID.ToString() select c;

        //    //if exists add a tag
        //    if (_alreadyExists.Any())
        //    {
        //        //TODO : check first if the tag exists
        //        foreach (XElement _song in _alreadyExists)
        //        {
        //            _song.Element("tags").Add(new XElement("tag", tag));
        //        }
        //        //XElement _foundedSong = _alreadyExists.ElementAt(0);
        //        //_foundedSong.Element("tags").Add(new XElement("tag", tag));
        //    }

        //    SaveXmlDocument(XmlDocument);
        //    return true;
        //}

        //public int GetTagCount(string tag)
        //{
        //    //XDocument doc = GetXmlDocument();
        //    if (XmlDocument != null)
        //    {
        //        var _query = from c in XmlDocument.Descendants("music").Elements("tags").Elements("tag") where c.Value.ToLower() == tag.ToLower() select c;
        //        return _query.Count();
        //    }
        //    else return 0;
        //}

        //public IEnumerable<XElement> GetSongsWithTag(string tag)
        //{
        //    //XDocument doc = GetXmlDocument();

        //    if (XmlDocument != null)
        //    {
        //        //does not work , dont know why, WORKAROUND
        //        //var _songs = from c in XmlDocument.Descendants("music") where c.Element("tags").Element("tag").Value.ToLower() == tag.ToLower() select c;

        //        List<XElement> _songsToReturn = new List<XElement>();
        //        var _songs = from c in XmlDocument.Descendants("music") select c;
        //        foreach (XElement _song in _songs)
        //        {
        //            var _queryForTag = from c in _song.Element("tags").Elements("tag") where c.Value.ToLower() == tag.ToLower() select c;
        //            if (_queryForTag.Any())
        //            {
        //                _songsToReturn.Add(_song);
        //            }
        //        }
        //        return _songsToReturn;
        //    }
        //    return null;
        //}

        //public IEnumerable<XElement> GetTagsOfMusic(int songID)
        //{
        //    //XDocument doc = GetXmlDocument();

        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Attribute("ID").Value == songID.ToString() select c;
        //    if (_alreadyExists.Any())
        //    {
        //        var _list = from c in _alreadyExists.Elements("tags").Elements("tag") select c;
        //        return _list;
        //    }
        //    return null;
        //}

        //public bool DeleteTagOfSong(int songId, string tag)
        //{
        //    XElement _song = GetSong(songId);
        //    if (_song != null)
        //    {
        //        var _queryTag = from c in _song.Element("tags").Elements("tag") where c.Value.ToLower() == tag.ToLower() select c;
        //        if (_queryTag.Any())
        //        {
        //            XElement _tag = _queryTag.First();
        //            _tag.Remove();
        //            SaveXmlDocument();
        //            return true;
        //        }
        //        return false;
        //    }
        //    return false;
        //}



        //public List<string> GetAllPlaylistValuesDistinct()
        //{
        //    if (XmlDocument != null)
        //    {
        //        var playlists = XmlDocument.Descendants("music").Elements("playlists").Elements("playlist")
        //                   .Select(c => (string)c)
        //                   .Distinct()
        //                   .OrderBy(c => c);
        //        return playlists.ToList();
        //    }
        //    return null;
        //}

        //public List<string> GetAllPlaylistValueOfSongDistinct(string artist, string title)
        //{
        //    //XDocument doc = GetXmlDocument();
        //    if (XmlDocument != null)
        //    {
        //        var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("title").Value.ToLower() == title.ToLower() && c.Element("artist").Value.ToLower() == artist.ToLower() select c;
        //        if (_alreadyExists.Any())
        //        {
        //            var _playlists = from c in _alreadyExists.Elements("playlists").Elements("playlist") select c;
        //            List<string> _list = new List<string>();
        //            foreach (XElement _playlist in _playlists)
        //            {
        //                _list.Add(_playlist.Value.ToString());
        //            }
        //            return _list;
        //        }
        //    }
        //    return null;
        //}

        //public bool InsertPlaylistToMusic(int ID, string playlist)
        //{
        //    //XDocument doc = GetXmlDocument();

        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Attribute("ID").Value == ID.ToString() select c;

        //    //if exists add a tag
        //    if (_alreadyExists.Any())
        //    {
        //        //TODO : check first if the playlists exists
        //        foreach (XElement _song in _alreadyExists)
        //        {
        //            _song.Element("playlists").Add(new XElement("playlist", playlist));
        //        }
        //    }

        //    SaveXmlDocument(XmlDocument);
        //    return true;
        //}

        //public IEnumerable<XElement> GetPlaylistsOfMusic(int songID)
        //{
        //    //XDocument doc = GetXmlDocument();
        //    if (XmlDocument != null)
        //    {
        //        var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Attribute("ID").Value == songID.ToString() select c;
        //        if (_alreadyExists.Any())
        //        {
        //            var _list = from c in _alreadyExists.Elements("playlists").Elements("playlist") select c;
        //            return _list;
        //        }
        //    }
        //    return null;
        //}

        //public IEnumerable<XElement> GetSongsWithPlaylist(string playlist)
        //{
        //    if (XmlDocument != null)
        //    {
        //        List<XElement> _songsToReturn = new List<XElement>();
        //        var _songs = from c in XmlDocument.Descendants("music") select c;
        //        foreach (XElement _song in _songs)
        //        {
        //            var _queryForTag = from c in _song.Element("playlists").Elements("playlist") where c.Value.ToLower() == playlist.ToLower() select c;
        //            if (_queryForTag.Any())
        //            {
        //                _songsToReturn.Add(_song);
        //            }
        //        }
        //        return _songsToReturn;
        //    }
        //    return null;
        //}

        //public int GetPlaylistCount(string playlist)
        //{
        //    //XDocument doc = GetXmlDocument();
        //    if (XmlDocument != null)
        //    {
        //        var _query = from c in XmlDocument.Descendants("music").Elements("playlists").Elements("playlist") where c.Value.ToLower() == playlist.ToLower() select c;
        //        return _query.Count();
        //    }
        //    else return 0;
        //}



        //public bool DeletePlaylistOfSong(int songId, string paylist)
        //{
        //    XElement _song = GetSong(songId);
        //    if (_song != null)
        //    {
        //        var _queryTag = from c in _song.Element("palylists").Elements("palylist") where c.Value.ToLower() == paylist.ToLower() select c;
        //        if (_queryTag.Any())
        //        {
        //            XElement _tag = _queryTag.First();
        //            _tag.Remove();
        //            SaveXmlDocument();
        //            return true;
        //        }
        //        return false;
        //    }
        //    return false;
        //}



        //public bool InsertBookmarkToMusic(int ID, string time, string note, string miliseconds)
        //{
        //    //XDocument doc = GetXmlDocument();

        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Attribute("ID").Value == ID.ToString() select c;

        //    //if exists add a tag
        //    if (_alreadyExists.Any())
        //    {
        //        //TODO : check first if the tag exists
        //        XElement _foundedSong = _alreadyExists.ElementAt(0);
        //        _foundedSong.Element("bookmarks").Add(new XElement("bookmark",
        //                                                new XElement("time", time),
        //                                                new XElement("note", note),
        //                                                new XElement("miliseconds", miliseconds)
        //                                            ));
        //    }
        //    SaveXmlDocument(XmlDocument);
        //    return true;
        //}

        //public IEnumerable<XElement> GetBookmarksOfMusic(string artist, string title)
        //{
        //    //XDocument doc = GetXmlDocument();

        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("title").Value.ToLower() == title.ToLower() && c.Element("artist").Value.ToLower() == artist.ToLower() select c;
        //    if (_alreadyExists.Any())
        //    {
        //        var _bookmarks = from c in _alreadyExists.Elements("bookmarks").Elements("bookmark") select c;
        //        return _bookmarks;
        //        //List<string> _list = new List<string>();
        //        //foreach (XElement _bookmark in _bookmarks)
        //        //{
        //        //    _list.Add(_bookmark.Value.ToString());
        //        //}
        //        //return _list;
        //    }
        //    return null;
        //}
        //public IEnumerable<XElement> GetBookmarksOfMusic(int ID)
        //{
        //    //XDocument doc = GetXmlDocument();

        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Attribute("ID").Value == ID.ToString() select c;
        //    if (_alreadyExists.Any())
        //    {
        //        var _bookmarks = from c in _alreadyExists.Elements("bookmarks").Elements("bookmark") select c;
        //        return _bookmarks;
        //    }
        //    return null;
        //}

        //public bool DeleteBookmarkOfSong(int songId, int miliseconds)
        //{
        //    XElement _song = GetSong(songId);
        //    if (_song != null)
        //    {
        //        var _queryTag = from c in _song.Element("bookmarks").Elements("bookmark") where c.Element("miliseconds").Value == miliseconds.ToString() select c;
        //        if (_queryTag.Any())
        //        {
        //            XElement _bookmark = _queryTag.First();
        //            _bookmark.Remove();
        //            SaveXmlDocument();
        //            return true;
        //        }
        //        return false;
        //    }
        //    return false;
        //}

        //public IEnumerable<XElement> GetAllSongs()
        //{
        //    //XDocument doc = GetXmlDocument();
        //    var _songs = from c in XmlDocument.Descendants("music") orderby (string)c.Element("artist") ascending select c;
        //    return _songs;
        //}

        //public IEnumerable<XElement> GetAllSongs(string search)
        //{
        //    //XDocument doc = GetXmlDocument();
        //    var _songs = from c in XmlDocument.Descendants("music")
        //                 where
        //                     c.Element("artist").Value.ToLower().Contains(search.ToLower()) ||
        //                     c.Element("album").Value.ToLower().Contains(search.ToLower()) ||
        //                     c.Element("title").Value.ToLower().Contains(search.ToLower()) ||
        //                     c.Element("genre").Value.ToLower().Contains(search.ToLower()) ||
        //                     c.Element("year").Value.ToLower().Contains(search.ToLower())
        //                 orderby (string)c.Element("artist") ascending
        //                 select c;
        //    return _songs;
        //}


        //public int GetLastSongID()
        //{
        //    //XDocument doc = GetXmlDocument();
        //    int max = (int)XmlDocument.Descendants("music").Attributes("ID").Select(c => (int)c).Max();
        //    return max;
        //}

        

        //public int GetSongID(string artist, string title)
        //{
        //    //XDocument doc = GetXmlDocument();
        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("title").Value.ToLower() == title.ToLower() && c.Element("artist").Value.ToLower() == artist.ToLower() select c;
        //    if (_alreadyExists.Any())
        //    {
        //        XElement _foundedSong = _alreadyExists.ElementAt(0);
        //        string _songID = _foundedSong.Attribute("ID").Value.ToString();
        //        return Int16.Parse(_songID);
        //    }
        //    else return 0;
        //}

        



        ////TEMP
        //public bool UpdateFilenameOfMusic(string artist, string title, string filename)
        //{
        //    //XDocument doc = GetXmlDocument();
        //    XElement _song = null;

        //    //var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("title").Value.ToLower() == title.ToLower() && c.Element("artist").Value.ToLower() == artist.ToLower() select c;
        //    //if (_alreadyExists.Any())
        //    //{
        //    //    _song = _alreadyExists.ElementAt(0);
        //    //}

        //    _song = ReadSong(artist, title);

        //    if (_song != null)
        //    {
        //        XElement _x = _song.Element("filename");
        //        if (_x == null)
        //        {
        //            _song.Add(new XElement("filename", filename));
        //            XElement _xx = _song.Element("filepath");
        //            if (_xx != null)
        //            {
        //                _song.Element("filepath").Remove();
        //            }

        //            SaveXmlDocument(XmlDocument);
        //            return true;
        //        }
        //        else
        //        {
        //            _song.Element("filename").Value = filename;
        //            SaveXmlDocument(XmlDocument);
        //            return false;
        //        }
        //    }
        //    return false;
        //}

        ////TEMP
        //public void UpdateAllSongsXmlUpdates(string musicFolder)
        //{
        //    var _songs = from c in XmlDocument.Descendants("music") select c;
        //    foreach (XElement _song in _songs)
        //    {
        //        //update the elements, go get the year and genre to the mp3 tags
        //        //string _songFilePath = _song.Element("filename") != null ? musicFolder + "\\" + _song.Element("filename").Value : "";

        //        //TagLib.File _songTags = null;
        //        //try
        //        //{
        //        //    if(System.IO.File.Exists(_songFilePath))
        //        //        _songTags = TagLib.File.Create(_songFilePath);
        //        //}
        //        //catch(Exception ex){}   

        //        XElement _x = _song.Element("filename");
        //        if (_x == null)
        //        {
        //            _song.Add(new XElement("filename", ""));
        //        }

        //        _x = _song.Element("filepath");
        //        if (_x != null)
        //        {
        //            _x.Remove();
        //        }
        //    }
        //    SaveXmlDocument(XmlDocument);
        //}

        //public IEnumerable<string> GetAllArtists()
        //{
        //    var _list = XmlDocument.Descendants("music").Elements("artist")
        //               .Select(c => (string)c)
        //               .Distinct()
        //               .OrderBy(c => c);
        //    return _list.ToList();
        //}

        //public IEnumerable<string> GetAllArtists(string search)
        //{
        //    var _query = (from c in XmlDocument.Descendants("music") where c.Element("artist").Value.ToLower().Contains(search.ToLower()) select c.Element("artist").Value).Distinct().OrderBy(c => c);
        //    return _query.ToList();
        //}

        //public IEnumerable<string> GetAllAlbums()
        //{
        //    var _list = XmlDocument.Descendants("music").Elements("album")
        //               .Select(c => (string)c)
        //               .Distinct()
        //               .OrderBy(c => c);
        //    return _list.ToList();
        //}

        //public IEnumerable<string> GetAllAlbums(string search)
        //{
        //    var _query = (from c in XmlDocument.Descendants("music") where c.Element("album").Value.ToLower().Contains(search.ToLower()) select c.Element("album").Value).Distinct().OrderBy(c => c);
        //    return _query.ToList();
        //}

        //public int GetArtistSongCount(string artist)
        //{
        //    var _query = (from c in XmlDocument.Descendants("music") where c.Element("artist").Value.ToLower() == artist.ToLower() select c).Count();
        //    return int.Parse(_query.ToString());
        //}

        //public int GetAlbumSongCount(string album)
        //{
        //    var _query = (from c in XmlDocument.Descendants("music") where c.Element("album").Value.ToLower() == album.ToLower() select c).Count();
        //    return int.Parse(_query.ToString());
        //}

        //public int GetArtistsCount()
        //{
        //    var _query = (from c in XmlDocument.Descendants("music").Elements("artist") where c.Value != string.Empty select c).Select(c => (string)c).Distinct().Count();
        //    return int.Parse(_query.ToString());
        //}

        //public int GetArtistsCount(string search)
        //{
        //    var _query = (from c in XmlDocument.Descendants("music").Elements("artist") where c.Value != string.Empty && c.Value.ToLower().Contains(search.ToLower()) select c).Select(c => (string)c).Distinct().Count();
        //    return int.Parse(_query.ToString());
        //}

        //public int GetAlbumsCount()
        //{
        //    //var _query = (from c in XmlDocument.Descendants("music").Elements("album") select c).Distinct().Count();
        //    var _query = (from c in XmlDocument.Descendants("music").Elements("album") where c.Value != string.Empty select c).Select(c => (string)c).Distinct().Count();
        //    return int.Parse(_query.ToString());
        //}

        //public int GetSongsCount()
        //{
        //    var _query = (from c in XmlDocument.Descendants("music") select c).Count();
        //    return int.Parse(_query.ToString());
        //}

        //public int GetSongsCount(string search, bool searchInArtist, bool searchInAlbum, bool searchInTitle)
        //{
        //    var _query = (from c in XmlDocument.Descendants("music") select c).Count();
        //    if (searchInArtist)
        //    {
        //        _query = (from c in XmlDocument.Descendants("music") where c.Element("artist").Value.ToLower().Contains(search.ToLower()) select c).Count();
        //    }
        //    if (searchInAlbum)
        //    {
        //        _query = (from c in XmlDocument.Descendants("music") where c.Element("album").Value.ToLower().Contains(search.ToLower()) select c).Count();
        //    }
        //    if (searchInTitle)
        //    {
        //        _query = (from c in XmlDocument.Descendants("music") where c.Element("title").Value.ToLower().Contains(search.ToLower()) select c).Count();
        //    }
        //    return int.Parse(_query.ToString());
        //}

        //public int GetAlbumsOfArtistCount(string artist)
        //{
        //    var _query = (from c in XmlDocument.Descendants("music") where c.Element("artist").Value.ToLower() == artist.ToLower() select c.Element("album")).Select(c => (string)c).Distinct().Count();
        //    return int.Parse(_query.ToString());
        //}

        //public IEnumerable<string> GetAllAlbumsOfArtist(string artist)
        //{
        //    var _query = (from c in XmlDocument.Descendants("music") where c.Element("artist").Value.ToLower() == artist.ToLower() select c.Element("album")).Select(c => (string)c).Distinct().OrderBy(c => c);
        //    return _query.ToList();
        //}

        //public IEnumerable<XElement> GetSongsOfArtist(string artist)
        //{
        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("artist").Value.ToLower() == artist.ToLower() select c;
        //    return _alreadyExists;
        //}

        //public IEnumerable<XElement> GetSongsOfArtist(string artist, string searchInTitle)
        //{
        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("artist").Value.ToLower() == artist.ToLower() select c;
        //    return _alreadyExists;
        //}

        //public IEnumerable<XElement> GetSongsOfArtistAndAlbum(string artist, string album)
        //{
        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("artist").Value.ToLower() == artist.ToLower() && c.Element("album").Value.ToLower() == album.ToLower() select c;
        //    return _alreadyExists;
        //}

        //public IEnumerable<XElement> GetSongsOfArtistAndAlbum(string artist, string album, string searchInTitle)
        //{
        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("artist").Value.ToLower() == artist.ToLower() && c.Element("album").Value.ToLower() == album.ToLower() && c.Element("title").Value.ToLower().Contains(searchInTitle.ToLower()) select c;
        //    return _alreadyExists;
        //}

        //public IEnumerable<XElement> GetSongsOfAlbum(string album)
        //{
        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("album").Value.ToLower() == album.ToLower() select c;
        //    return _alreadyExists;
        //}

        //public IEnumerable<XElement> GetSongsOfAlbum(string album, string searchInTitle)
        //{
        //    var _alreadyExists = from c in XmlDocument.Descendants("music") where c.Element("album").Value.ToLower() == album.ToLower() && c.Element("title").Value.ToLower().Contains(searchInTitle.ToLower()) select c;
        //    return _alreadyExists;
        //}

        //public bool IsSongDataDifferentFromData(int songId, string album, string genre, string year)
        //{
        //    XElement _song = GetSong(songId);
        //    if (_song != null)
        //    {
        //        if (_song.Element("album").Value.ToLower() != album.ToLower() ||
        //            _song.Element("genre").Value.ToLower() != genre.ToLower() ||
        //            _song.Element("year").Value != year)
        //        {
        //            return true;
        //        }
        //        else return false;
        //    }
        //    return false;
        //}

        ////TEMP
        //public bool CheckIfFileNameExistsInSong(int songID)
        //{
        //    XElement _song = GetSong(songID);
        //    XElement _x = _song.Element("filename");
        //    if (_x == null)
        //    {
        //        return true;
        //    }
        //    else return false;
        //}
    }
}
