using System.Collections.Generic;

namespace SpiroWeb.Models.PingoDoceProduct
{
    public class Category
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Brand
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class LeafCategory
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Promotion
    {
        public object type { get; set; }
        public object amount { get; set; }
        public object takeAmount { get; set; }
        public object payAmount { get; set; }
        public object beginDate { get; set; }
        public object endDate { get; set; }
        public object id { get; set; }
    }

    public class Contexts
    {
        public string catalogue { get; set; }
    }

    public class SuggestCatalogue
    {
        public object input { get; set; }
        public int weight { get; set; }
        public Contexts contexts { get; set; }
    }

    public class Product
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string secondName { get; set; }
        public string thirdName { get; set; }
        public string longDescription { get; set; }
        public string shortDescription { get; set; }
        public string sku { get; set; }
        public int imagesNumber { get; set; }
        public double grossWeight { get; set; }
        public string capacity { get; set; }
        public double netContent { get; set; }
        public string netContentUnit { get; set; }
        public int averageWeight { get; set; }
        public string onlineStatus { get; set; }
        public string status { get; set; }
        public string slug { get; set; }
        public string defaultEan { get; set; }
        public List<object> tags { get; set; }
        public List<Category> categories { get; set; }
        public List<string> eans { get; set; }
        public Brand brand { get; set; }
        public string catalogueId { get; set; }
        public List<string> categoriesArray { get; set; }
        public List<LeafCategory> leafCategories { get; set; }
        public bool isPerishable { get; set; }
        public List<string> ancestorsCategoriesArray { get; set; }
        public double regularPrice { get; set; }
        public object campaignPrice { get; set; }
        public double buyingPrice { get; set; }
        public Promotion promotion { get; set; }
        public int minimumOrderableQuantity { get; set; }
        public int maximumOrderableQuantity { get; set; }
        public List<object> qualitativeIcons { get; set; }
        public List<object> countriesOfOrigin { get; set; }
        public string additionalInfo { get; set; }
        public int durabilityDays { get; set; }
        public bool activePromotion { get; set; }
        public List<SuggestCatalogue> suggest_catalogue { get; set; }
    }


}