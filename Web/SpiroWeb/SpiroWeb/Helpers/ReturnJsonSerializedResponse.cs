using System.Web;

namespace SpiroWeb.Helpers
{

    public abstract class JsonSerializedResponse
    {
        protected static void ReturnSerializedResponse(object responseObject)
        {
            var serializedObject = System.Web.Helpers.Json.Encode(responseObject);
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.Write(serializedObject);
            //HttpContext.Current.Response.End();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}