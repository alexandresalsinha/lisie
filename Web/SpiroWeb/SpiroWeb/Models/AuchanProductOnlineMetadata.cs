namespace SpiroWeb.Models.AuchanContinenteProductOnlineMetadata
{
    public class Brand
    {
        public string @type { get; set; }
        public string name { get; set; }

    }
    public class Url
    {

    }
    public class Offers
    {
        public Url url { get; set; }
        public string @type { get; set; }
        public string priceCurrency { get; set; }
        public string price { get; set; }
        public string availability { get; set; }

    }
    public class Product
    {
        public string @context { get; set; }
        public string @type { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string mpn { get; set; }
        public string sku { get; set; }
        public string[] image { get; set; }
        public Brand brand { get; set; }
        public Offers offers { get; set; }

    }

}