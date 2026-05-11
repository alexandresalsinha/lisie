using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace SpiroWeb.Helpers
{
    public static class Json
    {
        public static string GetTextFromFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
                return System.IO.File.ReadAllText(filePath);
            else
                return string.Empty;
        }

        public static object GetDeSerializedObjectFromFile(Type objectType, string filePath)
        {
            string jsonText = GetTextFromFile(filePath);

            JavaScriptSerializer jss = new JavaScriptSerializer();
            List<Models.Tasks> _listObj = new List<Models.Tasks>();
            object _deserializedObject = jss.Deserialize(jsonText, _listObj.GetType());

            return _deserializedObject;
        }

        public static bool SaveObjectToJsonFile(object objectToSave, string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    string _serializedObject = jss.Serialize(objectToSave);

                    System.IO.File.WriteAllText(filePath, _serializedObject);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }
            return false;
        }
    }

}