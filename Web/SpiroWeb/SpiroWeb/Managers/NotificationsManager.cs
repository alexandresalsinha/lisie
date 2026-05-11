using ClassLibrary1;
using SpiroWeb.Controllers.api;
using SpiroWeb.Helpers;
using SpiroWeb.Models;
using System;
using System.Linq;
using System.Web.Script.Serialization;

namespace SpiroWeb.Managers
{
    public static class NotificationsManager
    {
        static public bool SendToUser(string userId, string title, string body, string data)
        {
            //var products = (string.IsNullOrEmpty(orderBy)) ? db.Products.ToList() : db.Products.OrderBy(c => c.Name).ToList();
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                if (string.IsNullOrEmpty(data))
                {
                    data = "{}";
                }
                var _businessDeviceTokens = db.UserDevices.Where(d => d.UserId == userId && !string.IsNullOrEmpty(d.DeviceToken)).Select(d => d.DeviceToken).ToArray();
                if (_businessDeviceTokens.Length > 0)
                {
                    ExpoNotifications.Send(new ExpoNotificationModel
                    {
                        to = _businessDeviceTokens,
                        sound = "default",
                        title = title,
                        body = body,
                        data = data
                    });
                    return true;
                }
                return false;
            }
        }

        static public bool SendToUsers(string title, string body, string data, string[] userIds = null, string[] expoTokens = null)
        {
            //var products = (string.IsNullOrEmpty(orderBy)) ? db.Products.ToList() : db.Products.OrderBy(c => c.Name).ToList();
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                if (userIds != null)
                {
                    var _businessDeviceTokens = db.UserDevices.Where(d => userIds.Contains(d.UserId) && !string.IsNullOrEmpty(d.DeviceToken)).Select(d => d.DeviceToken).ToArray();
                    if (_businessDeviceTokens.Length > 0)
                    {
                        ExpoNotifications.Send(new ExpoNotificationModel
                        {
                            to = _businessDeviceTokens,
                            sound = "default",
                            title = title,
                            body = body,
                            data = data
                        });
                        return true;
                    }
                }
                if (expoTokens != null)
                {
                    ExpoNotifications.Send(new ExpoNotificationModel
                    {
                        to = expoTokens,
                        title = title,
                        body = body,
                        data = data
                    });
                    return true;
                }
                return false;
            }
        }

        static public bool SendToAll(string title, string body, string data)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _allUsers = db.AspNetUsers.Select(c => c.Id).ToList();
                var _businessDeviceTokens = db.UserDevices.Where(d => _allUsers.Contains(d.UserId) && !string.IsNullOrEmpty(d.DeviceToken)).Select(d => d.DeviceToken).ToArray();
                if (_businessDeviceTokens.Length > 0)
                {
                    ExpoNotifications.Send(new ExpoNotificationModel
                    {
                        to = _businessDeviceTokens,
                        title = title,
                        body = body,
                        data = data
                    });
                    return true;
                }
            }
            return false;
        }

        static public bool SendMaxSavingsNotifications(string title, string body, string data)
        {
            using (SpiroStockManagementEntities db = new SpiroStockManagementEntities())
            {
                var _allUsers = db.AspNetUsers.Select(c => c.Id).ToList();
                var _businessDeviceTokens = db.UserDevices.Where(d => _allUsers.Contains(d.UserId) && !string.IsNullOrEmpty(d.DeviceToken)).Select(d => d.DeviceToken).ToArray();
                if (_businessDeviceTokens.Length > 0)
                {
                    ExpoNotifications.Send(new ExpoNotificationModel
                    {
                        to = _businessDeviceTokens,
                        title = title,
                        body = body,
                        data = data
                    });
                    return true;
                }
            }
            return false;
        }

        static public bool FinishedAddingProductsToOnlineStore(FinishedAddingProductsToOnlineStoresModel model)
        {
            try
            {
                dynamic _data = new
                {
                    action = "FinishedAddingProductsToOnlineStore",
                    argument = model.FinishedStores
                };
                return SendToUser(model.UserId, "Acabei!", "Adicionei os teus produtos aos mercado online", new JavaScriptSerializer().Serialize(_data));
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
