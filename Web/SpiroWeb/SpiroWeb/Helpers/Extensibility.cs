using LisieStores.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SpiroWeb.Helpers
{
    public static class AttributeExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }
    }

    public class Extensibility
    {
        private static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
              assembly.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                      .ToArray();
        }

        public static List<Market> GetStoreFetchers()
        {
            List<Market> _markets = new List<Market>();

            Type[] typelist = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "SpiroWeb.Markets");
            for (int i = 0; i < typelist.Length; i++)
            {
                Type _currentType = typelist[i];
                if (!_currentType.FullName.Contains("+"))
                {
                    MarketAttr _MarketAttr = (MarketAttr)Attribute.GetCustomAttribute(_currentType, typeof(MarketAttr));
                    if (_MarketAttr.GetId() == 6)
                    {
                        continue;
                    }
                    if (_MarketAttr != null)
                    {
                        _markets.Add(new Market
                        {
                            StoreId = _MarketAttr.GetId(),
                            StoreName = _MarketAttr.GetName(),
                            StoreUrl = _MarketAttr.GetUrl(),
                            StoreColor = _MarketAttr.GetColor(),
                            StoreSearchUrl = _MarketAttr.GetSearchUrl(),
                            ClassType = _currentType
                        });
                    }
                }
            }
            return _markets;
        }

        public static IMarketFetcher GetStoreFetcher(int storeId)
        {
            List<LisieStores.Extensibility.Market> _stores = Helpers.Extensibility.GetStoreFetchers();
            LisieStores.Extensibility.Market _currentStore = _stores.Find(c => c.StoreId == storeId);
            if (_currentStore != null)
            {
                LisieStores.Extensibility.IMarketFetcher _marketFetcher = (LisieStores.Extensibility.IMarketFetcher)Activator.CreateInstance(_currentStore.ClassType);
                _marketFetcher.StoreId = _currentStore.StoreId;
                _marketFetcher.StoreName = _currentStore.StoreName;
                _marketFetcher.StoreColor = _currentStore.StoreColor;
                _marketFetcher.StoreUrl = _currentStore.StoreUrl;

                return _marketFetcher;
            }
            else
            {
                return null;
            }
        }

        public static async Task<LisieStores.Extensibility.ProductSearchResult> GetProductStoreMetadata(int storeId, string productUrl)
        {
            LisieStores.Extensibility.IMarketFetcher _IMarketFetcher = Helpers.Extensibility.GetStoreFetcher(storeId);
            try
            {
                LisieStores.Extensibility.ProductSearchResult _ProductSearchResult = await _IMarketFetcher.GetProductMetadata(productUrl);

                if (_ProductSearchResult != null && _ProductSearchResult.Price != string.Empty) //Product online found
                {
                    return _ProductSearchResult;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class StoresMatchingFieldPriority
    {
        int[] Name = new int[] { 3, 4, 5 };
        int[] Brand = new int[] { 3, 4, 5 };
        int[] Weight = new int[] { 3, 4, 5 };
        int[] VariableWeightPrice = new int[] { 3, 4, 5 };
        int[] Image = new int[] { 3, 4, 5 };
    }
}