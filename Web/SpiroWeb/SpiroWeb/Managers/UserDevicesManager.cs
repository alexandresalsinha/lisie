using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiroWeb.Managers
{
    public static class UserDevicesManager
    {
        static public bool RecordUserStoresProductsTotalSavings(string userId, string storeIds, double minPrice, double maxPrice, double priceDifference, int totalProducts, int savings)
        {
            try
            {
                using (SpiroStockManagementEntities db2 = new SpiroStockManagementEntities())
                {
                    UserTotalSavings _newUserTotalSavings = new UserTotalSavings
                    {
                        UserId = userId,
                        StoreIds = storeIds,
                        MinPrice = minPrice,
                        MaxPrice = maxPrice,
                        PriceDifference = priceDifference,
                        TotalProducts = totalProducts,
                        Savings = savings,
                        CreateDate = DateTime.Now
                    };
                    db2.UserTotalSavings.Add(_newUserTotalSavings);
                    db2.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        static public bool UpdateUserDevice(string userId, string deviceId, string deviceToken, string operativeSystem, string modelName)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    ClassLibrary1.UserDevices _userDevice = db.UserDevices.Where(c => c.UserId == userId && c.DeviceId == deviceId).FirstOrDefault();
                    if (_userDevice != null)
                    {
                        _userDevice.DeviceToken = deviceToken;
                        _userDevice.UpdateDate = DateTime.Now;
                        db.UserDevices.Attach(_userDevice);
                        var entry = db.Entry(_userDevice);
                        entry.Property(y => y.DeviceToken).IsModified = true;
                        entry.Property(y => y.UpdateDate).IsModified = true;
                        // other changed properties

                        db.SaveChanges();
                    }
                    else
                    {
                        ClassLibrary1.UserDevices _newUserDevice = new ClassLibrary1.UserDevices();
                        _newUserDevice.UserId = userId;
                        _newUserDevice.DeviceId = deviceId;
                        _newUserDevice.DeviceToken = deviceToken;
                        _newUserDevice.OperativeSystem = operativeSystem;
                        _newUserDevice.ModelName = modelName;
                        _newUserDevice.CreateDate = DateTime.Now;
                        db.UserDevices.Add(_newUserDevice);
                        db.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        static public bool AddUserDevice(string userId, string deviceId, string deviceToken, string operativeSystem, string modelName, string lisieVersion)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    ClassLibrary1.UserDevices _userDevice = null;
                    if (!string.IsNullOrEmpty(deviceToken))
                    {
                        _userDevice = db.UserDevices.Where(c => c.UserId == userId && c.DeviceToken == deviceToken).FirstOrDefault();
                    }
                    else
                    {
                        _userDevice = db.UserDevices.Where(c => c.UserId == userId).FirstOrDefault();
                    }

                    if (_userDevice == null)
                    {
                        ClassLibrary1.UserDevices _newUserDevice = new ClassLibrary1.UserDevices();
                        _newUserDevice.UserId = userId;
                        _newUserDevice.DeviceId = deviceId;
                        _newUserDevice.DeviceToken = deviceToken;
                        _newUserDevice.OperativeSystem = operativeSystem;
                        _newUserDevice.ModelName = modelName;
                        _newUserDevice.LisieVersion = lisieVersion;
                        _newUserDevice.CreateDate = DateTime.Now;
                        db.UserDevices.Add(_newUserDevice);
                        db.SaveChanges();
                    }
                    else
                    {
                        _userDevice.OperativeSystem = operativeSystem;
                        _userDevice.ModelName = modelName;
                        _userDevice.LisieVersion = lisieVersion;
                        _userDevice.UpdateDate = DateTime.Now;
                        db.UserDevices.Attach(_userDevice);
                        var entry = db.Entry(_userDevice);
                        entry.Property(y => y.UpdateDate).IsModified = true;
                        entry.Property(y => y.ModelName).IsModified = true;
                        entry.Property(y => y.LisieVersion).IsModified = true;
                        entry.Property(y => y.OperativeSystem).IsModified = true;
                        db.SaveChanges();
                        return true; //it was updated not created
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        static public bool AddUserDeviceV2(string userId, string deviceId, string deviceToken, string operativeSystem, string modelName, string lisieVersion)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    ClassLibrary1.UserDevices _userDevice = null;
                    if (!string.IsNullOrEmpty(deviceId))
                    {
                        _userDevice = db.UserDevices.Where(c => c.UserId == userId && c.DeviceId == deviceId).FirstOrDefault();
                    }
                    //else
                    //{
                    //    _userDevice = db.UserDevices.Where(c => c.UserId == userId).FirstOrDefault();
                    //}

                    if (_userDevice == null)
                    {
                        ClassLibrary1.UserDevices _newUserDevice = new ClassLibrary1.UserDevices();
                        _newUserDevice.UserId = userId;
                        _newUserDevice.DeviceId = deviceId;
                        _newUserDevice.DeviceToken = deviceToken;
                        _newUserDevice.OperativeSystem = operativeSystem;
                        _newUserDevice.ModelName = modelName;
                        _newUserDevice.LisieVersion = lisieVersion;
                        _newUserDevice.UpdateDate = DateTime.Now;
                        _newUserDevice.CreateDate = DateTime.Now;
                        db.UserDevices.Add(_newUserDevice);
                        db.SaveChanges();
                    }
                    else
                    {
                        _userDevice.DeviceToken = deviceToken;
                        _userDevice.OperativeSystem = operativeSystem;
                        _userDevice.ModelName = modelName;
                        _userDevice.LisieVersion = lisieVersion;
                        _userDevice.UpdateDate = DateTime.Now;
                        db.UserDevices.Attach(_userDevice);
                        var entry = db.Entry(_userDevice);
                        entry.Property(y => y.DeviceToken).IsModified = true;
                        entry.Property(y => y.UpdateDate).IsModified = true;
                        entry.Property(y => y.ModelName).IsModified = true;
                        entry.Property(y => y.LisieVersion).IsModified = true;
                        entry.Property(y => y.OperativeSystem).IsModified = true;
                        db.SaveChanges();
                        return true; //it was updated not created
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        static public List<UserDeviceDTO> GetUserDevicesTokens(string userId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                return db.UserDevices.Where(c => c.UserId == userId).Select(c => new UserDeviceDTO
                {
                    Id = c.Id,
                    DeviceId = c.DeviceId,
                    CreateDate = c.CreateDate,
                    DeviceToken = c.DeviceToken,
                    OperativeSystem = c.OperativeSystem,
                    UpdateDate = c.UpdateDate,
                    UserId = c.UserId
                }).ToList();
            }
        }
    }

    public class UserDeviceDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceToken { get; set; }
        public string OperativeSystem { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}