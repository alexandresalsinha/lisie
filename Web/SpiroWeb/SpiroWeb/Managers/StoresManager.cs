using ClassLibrary1;
using System.Collections.Generic;
using System.Linq;

namespace SpiroWeb.Managers
{
    public static class StoresManager
    {
        static private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        static public List<Stores> GetAll()
        {
            return db.Stores.ToList();
        }

        static public List<LisieStores.Extensibility.Market> GetAllByExtensibility()
        {
            return Helpers.Extensibility.GetStoreFetchers().OrderBy(c => c.StoreId).ToList();
        }
    }
}