using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisieStores.Extensibility
{
    // Multiuse attribute.  
    [System.AttributeUsage(System.AttributeTargets.Class |
                           System.AttributeTargets.Struct,
                           AllowMultiple = true)]  // Multiuse attribute.  
    public class MarketAttr : System.Attribute
    {
        int StoreId { get; set; }
        string StoreName { get; set; }
        string StoreUrl { get; set; }
        string StoreSearchUrl { get; set; }
        string StoreColor { get; set; }

        public MarketAttr(int storeId, string storeName, string storeUrl, string storeColor, string storeSearchUrl)
        {
            this.StoreId = storeId;
            this.StoreName = storeName;
            this.StoreUrl = storeUrl;
            this.StoreSearchUrl = storeSearchUrl;
            this.StoreColor = storeColor;
        }

        public string GetName()
        {
            return StoreName;
        }
        public int GetId()
        {
            return StoreId;
        }
        public string GetUrl()
        {
            return StoreUrl;
        }
        public string GetSearchUrl()
        {
            return StoreSearchUrl;
        }

        public string GetColor()
        {
            return StoreColor;
        }
    }
}
