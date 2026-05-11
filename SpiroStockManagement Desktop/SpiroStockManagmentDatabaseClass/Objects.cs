using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiroStockManagmentDatabaseClass.Objects
{
    //xml serialization
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string VariableWeightPrice { get; set; }
        public string Brand { get; set; }
        public string PackageInfo { get; set; }
        public string categoryString { get; set; }
        public string PictureSmallFilename { get; set; }
        public string PictureUrl { get; set; }
        public string InformationTakenFrom { get; set; }
        public string MarketItemUrl { get; set; }
        public string BarCode { get; set; }
        public bool IsBlackListed { get; set; }
        public string InsertDate { get; set; }
        public string Description { get; set; }
        public int QuantityIn { get; set; }
        public float QuantityWeightIn { get; set; }
        public int QuantityOut { get; set; }
        public float QuantityWeightOut { get; set; }
        public List<Item> History { get; set; }
    }

    public class Item
    {
        public int Quantity { get; set; }
        public float QuantityWeight { get; set; }
        public string ListName { get; set; } //the item is going In or Out
        public string InsertDate { get; set; }
    }

    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }
        public string Video { get; set; }
        public string VideoPhoto { get; set; }
        public string Cuisine { get; set; }
        public string Category { get; set; }
        public int Servings { get; set; }
        public string Yield { get; set; }
        public int Rating { get; set; }
        public float TimePreparing { get; set; }
        public float TimeCooking { get; set; }
        public float TimeReady { get; set; }
        public string InsertDate { get; set; }
        public string Commentary { get; set; }
        public bool IsBlackListed { get; set; }
        public List<RecipeIngredient> IngredientList { get; set; }
        public List<Step> Directions { get; set; }
        public string Tags { get; set; }
    }

    public class Step
    {
        public string Value { get; set; }
    }

    public class RecipeIngredient
    {
        public string Name { get; set; }
        public string Amount { get; set; }
        public string Units { get; set; }
        public string Information { get; set; }
    }
    

    //ingredients
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<IngredientProduct> Products { get; set; }
    }

    public class IngredientProduct
    {
        public int Id { get; set; }
    }


    //others

    public class AutoCompleteProductData
    {
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public string ProductBrand { get; set; }
    }
}
