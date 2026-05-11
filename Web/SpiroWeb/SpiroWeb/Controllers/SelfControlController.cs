using ClassLibrary1;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;


namespace SpiroWeb.Controllers
{
    public class SelfControlController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();


        // GET: SmokeCigarette
        [HttpGet]
        public ActionResult SmokeCigarette()
        {
            try
            {
                var result = (from a in db.SmokeCigaretteHistory
                              where (DbFunctions.TruncateTime(a.SmokeDate).Value == DateTime.Today)
                              select a);
                var smokesOfToday = result.ToList();
                if (smokesOfToday.Count() == 0 || smokesOfToday.Count() < 10)
                {
                    SmokeCigaretteHistory _SmokeCigaretteHistory = new SmokeCigaretteHistory
                    {
                        SmokeDate = DateTime.Now
                    };
                    db.SmokeCigaretteHistory.Add(_SmokeCigaretteHistory);
                    db.SaveChanges();
                    return Json("You´ve smoked today " + smokesOfToday.Count() + ", after this one you still can smoke " + (10 - smokesOfToday.Count() - 1) + ", but are you shure? Look at the sun, or do some push-ups Alexander. You can do it!", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("You´ve reached you´re smoking limit today Alexander. Remember, you have to remain alive for you´re loved ones. And you will breath better. Hang on until tomorrow. We´re together! Be strog!", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }
        }

        // GET: CanISmokeCigarette
        [HttpGet]
        public ActionResult CanSmokeCigarette()
        {
            try
            {
                var result = (from a in db.SmokeCigaretteHistory
                              where (DbFunctions.TruncateTime(a.SmokeDate).Value == DateTime.Today)
                              select a);
                var smokesOfToday = result.ToList();
                if (smokesOfToday.Count() == 0 || smokesOfToday.Count() < 10)
                {

                    return Json("You´ve smoked today " + smokesOfToday.Count() + ", you still can smoke " + (10 - smokesOfToday.Count()) + " cigarettes today. But still, try to look at the sun, or do some push-ups Alexander. If you can resist for some minutes the craving will go! Believe me.", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("You´ve reached you´re smoking limit today Alexander. Remember, you have to remain alive for you´re loved ones. And you will breath better. Hang on until tomorrow. We´re together! Be strog!", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }
        }

        // GET: CanISmokeCigarette
        [HttpGet]
        public ActionResult DailyBuddhistPrayer()
        {
            try
            {
                WebRequest request = WebRequest.Create("http://www.beliefnet.com/faiths/buddhism/daily-buddhist-prayer.aspx");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // Display the status.
                Console.WriteLine(response.StatusDescription);
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();

                int foundPrayerIndex = responseFromServer.IndexOf("quote-body no-quotes");
                int quoteBeginningIndex = responseFromServer.IndexOf("<p>", foundPrayerIndex) + 3;
                int quoteEndIndex = responseFromServer.IndexOf("</p>", quoteBeginningIndex);
                string subString = responseFromServer.Substring(quoteBeginningIndex, quoteEndIndex - quoteBeginningIndex);
                string endText = Regex.Replace(subString, "<.*?>", String.Empty);
                //Regex re = new Regex("\r\n$");
                endText = endText.Replace("\r\n", "");
                return Json(endText, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveDailyBuddhistPrayer()
        {
            try
            {
                WebRequest request = WebRequest.Create("http://www.beliefnet.com/faiths/buddhism/daily-buddhist-prayer.aspx");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // Display the status.
                Console.WriteLine(response.StatusDescription);
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();

                int foundPrayerIndex = responseFromServer.IndexOf("quote-body no-quotes");
                int quoteBeginningIndex = responseFromServer.IndexOf("<p>", foundPrayerIndex) + 3;
                int quoteEndIndex = responseFromServer.IndexOf("</p>", quoteBeginningIndex);
                string subString = responseFromServer.Substring(quoteBeginningIndex, quoteEndIndex - quoteBeginningIndex);
                string endText = Regex.Replace(subString, "<.*?>", String.Empty);
                //Regex re = new Regex("\r\n$");
                endText = endText.Replace("\r\n", "");

                //Save to dabase
                return Json(endText, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }
        }

    }
}
