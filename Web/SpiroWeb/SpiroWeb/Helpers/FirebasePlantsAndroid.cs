using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SpiroWeb.Helpers
{
    static public class FirebasePlantsAndroid
    {
        static public bool SendNotificationToAndroidPhone(string deviceToken, string dataToSend)
        {
            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = "application/json";
            var objNotification = new
            {
                //to = "dZv63ZrBY4M:APA91bHbDy4iMppQuirgMmhf5lpBKbTXCAfJ7HKnl0zbdGuh8C1weBOqgzMLf7N6liGI1jNd0C6oF5igBT860KjIbENK83UPFzv3Nqf_nUYOaVQQy7gYy5igt5i8UEY7KdMzSQebJolU",
                to = deviceToken,
                //data = dataToSend
                data = new
                {
                    body = dataToSend
                }
            };
            string jsonNotificationFormat = Newtonsoft.Json.JsonConvert.SerializeObject(objNotification);


            Byte[] byteArray = Encoding.UTF8.GetBytes(jsonNotificationFormat);
            //tRequest.Headers.Add(string.Format("Authorization: key={0}", "dZv63ZrBY4M:APA91bHbDy4iMppQuirgMmhf5lpBKbTXCAfJ7HKnl0zbdGuh8C1weBOqgzMLf7N6liGI1jNd0C6oF5igBT860KjIbENK83UPFzv3Nqf_nUYOaVQQy7gYy5igt5i8UEY7KdMzSQebJolU"));
            tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAfPkjPrA:APA91bE1q8Sf7iHiRb6m2buyRxhNahmvlfRuzU8Z3wCft2MgGy9DmR7geoaqZCw4V08P-TCMuGQ3fOpLpUqJD18LbFbJagcSR42TnU8zObUoT3jNn_GVfM9kBMU30DeoagcAC3-xGKyD"));
            //tRequest.Headers.Add(string.Format("Sender: id={0}", "107209686968"));
            tRequest.Headers.Add(string.Format("Sender: id={0}", "536755781296"));
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

        static public bool SendNotificationToAndroidUserPhone(string userId, string dataToSend, bool onlyLastDevice = false)
        {

            //get user last device token
            DataManager.UserDevicesManager _userDevicesManager2 = new DataManager.UserDevicesManager();
            List<ClassLibrary1.UserDevices> _userDevicesTokens2 = _userDevicesManager2.GetUserDevicesTokens(userId);

            if (_userDevicesTokens2.Count() > 0)
            {
                foreach (ClassLibrary1.UserDevices _userDevice in _userDevicesTokens2)
                {
                    SendNotificationToAndroidPhone(_userDevice.DeviceToken, dataToSend);
                    if (onlyLastDevice) return true;
                }
            }

            return true;
        }
    }
}