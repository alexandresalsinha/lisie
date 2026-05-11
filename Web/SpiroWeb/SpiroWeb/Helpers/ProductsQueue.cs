using ClassLibrary1;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiroWeb.Helpers
{
    public static class ProductsQueue
    {
        //LEGACY
        public static int ProcessProduct(string barcode, string userId, bool addToHistory = true, string addType = "")
        {
            //Save Interaction Of scanning barcode in LisieHome
            Managers.InteractionsManager.Add(userId, "LisieHomeBarcodeScanned", "barcode:" + barcode);


            SpiroStockManagementEntities db = new SpiroStockManagementEntities();
            int _userListProductId = -1;

            var query = (from c in db.Products
                         where c.Barcode.ToString() == barcode
                         select new
                         {
                             c.Id,
                             c.Name
                         }).FirstOrDefault();
            if (query != null)
            {
                int _productId = query.Id;
                var _productName = query.Name;
                db.Dispose();
                _userListProductId = Managers.UserListsManager.AddProductToList(_productId, _productName, addType.ToLower(), 1, 0, addToHistory, userId, true);

            }
            //else brodcast to android devices
            else
            {
                //get user last device token
                DataManager.UserDevicesManager _userDevicesManager = new DataManager.UserDevicesManager();
                List<ClassLibrary1.UserDevices> _userDevicesTokens = _userDevicesManager.GetUserDevicesTokens(userId);

                if (_userDevicesTokens.Count() > 0)
                {
                    foreach (ClassLibrary1.UserDevices _userDevice in _userDevicesTokens)
                    {
                        Helpers.FirebaseAndroid.SendNotificationToAndroidPhone(_userDevice.DeviceToken, "processBarcode:" + barcode);
                    }
                }
            }


            return _userListProductId;
        }

        //Backup
        //public static int ProcessProductNew(string barcode, string userId, bool addToHistory = true, string addType = "")
        //{
        //    //Save Interaction Of scanning barcode in LisieHome
        //    Managers.InteractionsManager.Add(userId, "LisieHomeBarcodeScanned", "barcode:" + barcode);


        //    //NEW - Get Lisie user mode in db
        //    string _lisieUserState = Managers.LisieHomeManager.GetUserState(userId);
        //    if (!string.IsNullOrEmpty(_lisieUserState))
        //    {
        //        addType = _lisieUserState;
        //    }

        //    //SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        //    int _userListProductId = -1;

        //    var query = Managers.ProductsManager.GetByBarcode(barcode);


        //    //var query = (from c in db.Products
        //    //             where c.Barcode.ToString() == barcode
        //    //             select new
        //    //             {
        //    //                 c.Id,
        //    //                 c.Name
        //    //             }).FirstOrDefault();
        //    if (query != null)
        //    {
        //        int _productId = query.Id;
        //        var _productName = query.Name;
        //        //db.Dispose();
        //        _userListProductId = Managers.UserListsManager.AddProductToList(_productId, _productName, addType.ToLower(), 1, 0, addToHistory, userId);

        //    }
        //    //else brodcast to android devices
        //    else
        //    {
        //        //get user last device token
        //        DataManager.UserDevicesManager _userDevicesManager = new DataManager.UserDevicesManager();
        //        List<ClassLibrary1.UserDevices> _userDevicesTokens = _userDevicesManager.GetUserDevicesTokens(userId);

        //        if (_userDevicesTokens.Count() > 0)
        //        {
        //            foreach (ClassLibrary1.UserDevices _userDevice in _userDevicesTokens)
        //            {
        //                Helpers.FirebaseAndroid.SendNotificationToAndroidPhone(_userDevice.DeviceToken, "processBarcodeNew:" + barcode + "-" + addType.ToLower());
        //            }
        //        }
        //    }


        //    return _userListProductId;
        //}

        public async static Task<int> ProcessProductNew(string barcode, string userId, bool addToHistory = true)
        {
            //Save Interaction Of scanning barcode in LisieHome
            Managers.InteractionsManager.Add(userId, "LisieHomeBarcodeScanned", "barcode:" + barcode);

            //NEW - Get Lisie user mode in db
            string _lisieUserState = Managers.LisieHomeManager.GetUserState(userId);
            //if (!string.IsNullOrEmpty(_lisieUserState))
            int _userListProductId = await Managers.UserListsManager.AddProductByBarcodeFromLisieHome(barcode, userId, _lisieUserState);

            //If not found , brodcast to android devices
            if (_userListProductId == -1)
            {
                //get user last device token
                DataManager.UserDevicesManager _userDevicesManager = new DataManager.UserDevicesManager();
                List<ClassLibrary1.UserDevices> _userDevicesTokens = _userDevicesManager.GetUserDevicesTokens(userId);

                if (_userDevicesTokens.Count() > 0)
                {
                    foreach (ClassLibrary1.UserDevices _userDevice in _userDevicesTokens)
                    {
                        Helpers.FirebaseAndroid.SendNotificationToAndroidPhone(_userDevice.DeviceToken, "processBarcodeNew:" + barcode + "-" + _lisieUserState.ToLower());
                    }
                }
            }

            return _userListProductId;
        }
    }
}