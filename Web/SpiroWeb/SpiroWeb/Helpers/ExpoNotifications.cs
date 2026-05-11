using SpiroWeb.Models;
using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace SpiroWeb.Helpers
{
    static public class ExpoNotifications
    {
        public static JsonApiResponse Send(Models.ExpoNotificationModel model)
        {
            try
            {
                var url = "https://exp.host/--/api/v2/push/send";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";

                httpRequest.ContentType = "application/json";
                var json = new JavaScriptSerializer().Serialize(model);
                var data = json;
                //example of working string
                //var data = @"{
                //  ""to"": ""ExponentPushToken[dgYGScJoPlmkhIFXZGMEZx]"",
                //  ""title"":""hello"",
                //  ""body"": ""world""
                //}";

                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return new JsonApiResponse
                    {
                        Code = 1,
                        Success = true,
                        Data = result,
                        Message = "Success"
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
                return new JsonApiResponse
                {
                    Success = false,
                    Code = -10,
                    Message = "Error: " + ex.Message
                };
            }
        }
    }
    //public class FCMResponse
    //{
    //    public long multicast_id { get; set; }
    //    public int success { get; set; }
    //    public int failure { get; set; }
    //    public int canonical_ids { get; set; }
    //    public List<FCMResult> results { get; set; }
    //}
    //public class FCMResult
    //{
    //    public string message_id { get; set; }
    //}
}