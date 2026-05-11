using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SpiroWeb.Helpers
{
    static public class FirebaseAndroid
    {
        static public bool SendNotification(string userId, string dataToSend)
        {
            try
            {
                DataManager.UserDevicesManager _userDevicesManager = new DataManager.UserDevicesManager();
                List<ClassLibrary1.UserDevices> _userDevicesTokens = _userDevicesManager.GetUserDevicesTokens(userId);

                if (_userDevicesTokens.Count() > 0)
                {
                    foreach (ClassLibrary1.UserDevices _userDevice in _userDevicesTokens)
                    {
                        if (!string.IsNullOrEmpty(_userDevice.DeviceToken))
                        {
                            SendNotificationToAndroidPhone(_userDevice.DeviceToken, dataToSend);
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        static public bool SendNotificationToAndroidPhone(string deviceToken, string dataToSend)
        {
            try
            {
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var objNotification = new
                {
                    to = deviceToken,
                    data = new
                    {
                        body = dataToSend
                    }
                };
                string jsonNotificationFormat = Newtonsoft.Json.JsonConvert.SerializeObject(objNotification);

                Byte[] byteArray = Encoding.UTF8.GetBytes(jsonNotificationFormat);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAGPYx_7g:APA91bHXn_z_T7efw9Woo3ZMPmhufnx0mm-k2rWd5i5UknSo6-IzbqPZfbEls1keQ1bdug952LbKa99mKP6LcJd-SwIpwBVU1CGKiNdi1ZoxpkdjoUEq6fBhbIZVC7DSguOCNEHDdlYh"));
                tRequest.Headers.Add(string.Format("Sender: id={0}", "107209686968"));
                tRequest.ContentLength = byteArray.Length;
                tRequest.ContentType = "application/json";
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String responseFromFirebaseServer = tReader.ReadToEnd();

                                FCMResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<FCMResponse>(responseFromFirebaseServer);
                                if (response.success == 1)
                                {
                                    //new NotificationBLL().InsertNotificationLog(dayNumber, notification, true);
                                }
                                else if (response.failure == 1)
                                {
                                    //new NotificationBLL().InsertNotificationLog(dayNumber, notification, false);
                                    //sbLogger.AppendLine(string.Format("Error sent from FCM server, after sending request : {0} , for following device info: {1}", responseFromFirebaseServer, jsonNotificationFormat));

                                }

                            }
                        }

                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
    public class FCMResponse
    {
        public long multicast_id { get; set; }
        public int success { get; set; }
        public int failure { get; set; }
        public int canonical_ids { get; set; }
        public List<FCMResult> results { get; set; }
    }
    public class FCMResult
    {
        public string message_id { get; set; }
    }
}