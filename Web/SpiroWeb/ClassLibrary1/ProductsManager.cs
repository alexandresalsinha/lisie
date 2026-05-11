using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    public class ProductsManager
    {
       public SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        public Products GetByBarCode(long barCode)
        {
            Products _product = db.Products.FirstOrDefault(i => i.Barcode.Equals(barCode));
            return _product;
        }

        public void InsertProduct(Products newProduct)
        {
            try
            {
                db.Products.Add(newProduct);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                string s = ex.Message;   
            }
        }

        public List<int> GetFirstProducts()
        {
            try
            {
                List<int> _productIds = new List<int>();
                //using (SpiroStockManagementEntities db = new SpiroStockManagementEntities("metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=cp117.webserver.pt;initial catalog=SpiroStockManagement;persist security info=True;user id=Spiro;password=NI/+70#_gUW4JGE;MultipleActiveResultSets=True;App=EntityFramework&quot;"))
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    var _lastProducts = db.Products.Take(5);
                    foreach (var item in _lastProducts)
                    {
                        _productIds.Add(item.Id);
                    }
                }
                return _productIds;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
