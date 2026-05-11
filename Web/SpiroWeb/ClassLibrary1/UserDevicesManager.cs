using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1;

namespace DataManager
{
    public class UserDevicesManager
    {
        public SpiroStockManagementEntities db = new SpiroStockManagementEntities();

        public bool UpdateUserDevice(string userId, string deviceId, string deviceToken, string operativeSystem)
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
                _newUserDevice.CreateDate = DateTime.Now;
                db.UserDevices.Add(_newUserDevice);
                db.SaveChanges();
            }
            return true;
        }

        public bool AddUserDevice(string userId, string deviceId, string deviceToken, string operativeSystem)
        {
            ClassLibrary1.UserDevices _userDevice = db.UserDevices.Where(c => c.UserId == userId && c.DeviceToken == deviceToken).FirstOrDefault();
            if (_userDevice == null)
            {
                ClassLibrary1.UserDevices _newUserDevice = new ClassLibrary1.UserDevices();
                _newUserDevice.UserId = userId;
                _newUserDevice.DeviceId = deviceId;
                _newUserDevice.DeviceToken = deviceToken;
                _newUserDevice.OperativeSystem = operativeSystem;
                _newUserDevice.CreateDate = DateTime.Now;
                db.UserDevices.Add(_newUserDevice);
                db.SaveChanges();
            } else
            {
                return false;
            }
            return true;
        }

        public List<UserDevices> GetUserDevicesTokens(string userId)
        {
            return db.UserDevices.Where(c => c.UserId == userId).ToList();
        }
    }
}
