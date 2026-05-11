using ClassLibrary1;
using System;
using System.Linq;

namespace SpiroWeb.Managers
{
    public static class UsersManager
    {
        static public int GetTotal()
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                return db.AspNetUsers.Count();
            }
        }

        static public UsersApple AddAppleUser(string appleUserId, string appleUserEmail)
        {
            try
            {
                using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
                {
                    UsersApple _new = new UsersApple();
                    _new.AppleUserId = appleUserId;
                    _new.Email = appleUserEmail;
                    _new.CreateDate = DateTime.Now;
                    db.UsersApple.Add(_new);
                    db.SaveChanges();
                    return _new;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        static public UsersApple GetAppleUser(string appleUserId)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                return db.UsersApple.Where(c => c.AppleUserId == appleUserId).FirstOrDefault();
            }
        }
    }
}