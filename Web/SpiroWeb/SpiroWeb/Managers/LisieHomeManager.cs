using ClassLibrary1;
using SpiroWeb.Helpers;
using System;
using System.Data.Entity;
using System.Linq;

namespace SpiroWeb.Managers
{
    public static class LisieHomeManager
    {
        static public string GetUserState(string userId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _userLisieHome = db.LisieHome.Where(c => c.UserId == userId).FirstOrDefault();
                if (_userLisieHome != null)
                {
                    return _userLisieHome.Mode.Trim().ToLower();
                }
                else
                {
                    //add new
                    LisieHome _newLisieHomeState = new LisieHome
                    {
                        UserId = userId,
                        CreateDate = DateTime.Now,
                        Mode = "consumed"
                    };
                    db.LisieHome.Add(_newLisieHomeState);
                    db.SaveChanges();
                    return "consumed";
                }
            }
        }

        static public int SetUserState(string userId, string mode)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                try
                {
                    var _userLisieHome = db.LisieHome.Where(c => c.UserId == userId).FirstOrDefault();
                    if (_userLisieHome != null)
                    {
                        _userLisieHome.Mode = mode;
                        _userLisieHome.UpdateDate = DateTime.Now;
                        db.Entry(_userLisieHome).State = EntityState.Modified;
                        db.SaveChanges();
                        return 1;
                    }
                    else //if it doens´t exist create it
                    {
                        LisieHome _newLisieHome = new LisieHome
                        {
                            CreateDate = DateTime.Now,
                            Mode = mode.Trim().ToLower(),
                            UserId = userId
                        };
                        db.LisieHome.Add(_newLisieHome);
                        db.SaveChanges();
                        return 1;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex.InnerException.Message);
                    return -1;
                }
            }
        }
    }
}