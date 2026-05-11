using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiroWeb.Managers
{
    public static class UserUpdatePricesRequestsManager
    {
        static private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        static public UserUpdatePricesRequests Add(string userId)
        {
            if (userId != string.Empty)
            {
                var _exists = db.UserUpdatePricesRequests.Where(c => c.UserId.Equals(userId) && c.Answered == false).FirstOrDefault();
                if (_exists != null)
                    return _exists;

                try
                {
                    UserUpdatePricesRequests _UserUpdatePricesRequests = new UserUpdatePricesRequests();
                    _UserUpdatePricesRequests.UserId = userId;
                    _UserUpdatePricesRequests.Answered = false;
                    _UserUpdatePricesRequests.RequestDate = DateTime.Now;
                    db.UserUpdatePricesRequests.Add(_UserUpdatePricesRequests);
                    db.SaveChanges();
                    return _UserUpdatePricesRequests;
                }
                catch (Exception ex)
                {
                    Helpers.Logger.Debug("Error - " + ex.InnerException.Message);
                    return null;
                }

            }
            return null;
        }
        static public List<UserUpdatePricesRequests> Get(bool answered = false)
        {
            return db.UserUpdatePricesRequests.Where(c => c.Answered == answered).ToList();
        }

        static public UserUpdatePricesRequests SetAsAnswered(int id)
        {
            try
            {
                var _exists = db.UserUpdatePricesRequests.Where(c => c.Id == id).FirstOrDefault();
                if (_exists != null)
                {
                    _exists.Answered = true;
                    db.SaveChanges();
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Helpers.Logger.Debug("Error - " + ex.InnerException.Message);
                return null;
            }
            return null;
        }
    }
}