using FirebaseSharp.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseHelper
{
    public static class Helper
    {
        public static string Get(string urlPath)
        {
            string url = "SpiroStockManagement";
            //Firebase fb = new Firebase(new Uri("https://blazing-fire-2294.firebaseio.com"));


            string rootUri = "https://brilliant-torch-9476.firebaseio.com";
            string authToken = "D5xCysnt6IfFe9FFLG4WUYCcotv9H4VlmISwSVIP";

            Firebase fb = new Firebase(rootUri, authToken);
            string jsonData = fb.Get(urlPath);

            return jsonData;
        }

        public static bool Delete(string key)
        {
            try
            {
                string url = "SpiroStockManagement";
                //Firebase fb = new Firebase(new Uri("https://blazing-fire-2294.firebaseio.com"));


                string rootUri = "https://brilliant-torch-9476.firebaseio.com";
                string authToken = "D5xCysnt6IfFe9FFLG4WUYCcotv9H4VlmISwSVIP";

                Firebase fb = new Firebase(rootUri, authToken);
                fb.Delete(url + "/" + key);

                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }
    }
}
