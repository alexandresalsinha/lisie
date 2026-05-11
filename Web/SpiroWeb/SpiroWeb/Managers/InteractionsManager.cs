using ClassLibrary1;
using SpiroWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiroWeb.Managers
{
    public static class InteractionsManager
    {
        static private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        static public Interactions Add(string userId, string name, string extra)
        {
            try
            {
                using (SpiroStockManagementEntities _db = new SpiroStockManagementEntities())
                {
                    Interactions _interaction = new Interactions
                    {
                        UserId = userId,
                        Name = name,
                        Extra = extra,
                        CreateDate = DateTime.Now
                    };

                    _db.Interactions.Add(_interaction);
                    _db.SaveChanges();

                    if (userId != "9ff8224f-17cf-49fb-b555-05779a13eb40")
                    {
                        Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastNewInteractions("9ff8224f-17cf-49fb-b555-05779a13eb40", GetTotal());
                    }
                    Microsoft.AspNet.SignalR.StockTicker.StockTicker.Instance.BroadcastInteraction("9ff8224f-17cf-49fb-b555-05779a13eb40", _interaction);

                    //check if last interaction was more than one hour ago
                    Interactions _lastInteraction = GetLastInteraction(_db, userId, string.Empty);
                    if (_lastInteraction != null && HasInteractionBeenBefore(userId, _lastInteraction, 60))
                    {
                        Controllers.UserListsController _UserListsController = new Controllers.UserListsController();
                        _UserListsController.RequestUserShoppingListProductsUpdatePrices(userId);
                    }

                    return _interaction;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("Error adding user interaction" + ex.Message);
                return null;
            }
        }

        static public bool HasInteractionBeenBefore(string userId, Interactions interaction, int minutes)
        {
            DateTime _nowMinusMinutes = DateTime.Now.AddMinutes(-minutes);
            if (interaction.CreateDate < _nowMinusMinutes)
                return true;
            else
                return false;
        }

        static public Interactions GetLastInteraction(SpiroStockManagementEntities context, string userId, string name)
        {
            try
            {
                if (name == string.Empty)
                    return context.Interactions.Where(c => c.UserId.Equals(userId)).OrderByDescending(c => c.Id).First();
                else
                    return context.Interactions.Where(c => c.UserId.Equals(userId) && c.Name.ToLower() == name.ToLower()).OrderByDescending(c => c.Id).First();

            }
            catch (Exception)
            {
                return null;
            }

        }

        static public List<Interactions> GetOfUser(string userId)
        {
            try
            {
                return db.Interactions.Where(c => c.UserId.Equals(userId)).OrderByDescending(c => c.CreateDate).ToList();
            }
            catch (Exception ex)
            {
                return new List<Interactions>();
            }
        }

        static public int GetTotal()
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                return db.Interactions.Count();
            }
        }

        static public List<Interactions> GetAll()
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                return db.Interactions.OrderByDescending(c => c.Id).ToList();
            }

        }
    }
}