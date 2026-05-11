using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataManager
{
    public class SelfJungleMaestroManager
    {
        public SpiroStockManagementEntities db = new SpiroStockManagementEntities();


        public bool InsertFlowerStatus(SelfJungleMaestro_PlantStatus plantStatus)
        {
            try
            {
                db.SelfJungleMaestro_PlantStatus.Add(plantStatus);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InsertRoomStatus(SelfJungleMaestro_RoomStatus roomStatus)
        {
            try
            {
                db.SelfJungleMaestro_RoomStatus.Add(roomStatus);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<UserDevices> GetUserDevicesTokens(string userId)
        {
            return db.UserDevices.Where(c => c.UserId == userId).ToList();
        }
    }
}
