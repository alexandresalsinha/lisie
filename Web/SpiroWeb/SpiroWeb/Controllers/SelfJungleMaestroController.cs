using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;


namespace SpiroWeb.Controllers
{
    public class SelfJungleMaestroController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();
        [Authorize]
        public ActionResult Plants()
        {
            //Helpers.FirebasePlantsAndroid.SendNotificationToAndroidPhone("dZv63ZrBY4M:APA91bHbDy4iMppQuirgMmhf5lpBKbTXCAfJ7HKnl0zbdGuh8C1weBOqgzMLf7N6liGI1jNd0C6oF5igBT860KjIbENK83UPFzv3Nqf_nUYOaVQQy7gYy5igt5i8UEY7KdMzSQebJolU", "plantStatus:");

            Helpers.FirebasePlantsAndroid.SendNotificationToAndroidPhone("d23GTM7M9I0:APA91bHvp5cD0Ovbc-O60ltcGV0TskTFTvNNNKYnOyLubfPWbqQQoDlmHx3hffb7NmZR6dR0O133TJnPI-AdB7oVO1pjCrnPkmHqmTiSR7_6V2d5c0cndEjHZOakrJuUQ2ecfsAHA1Iv", "plantStatus:");
            //get last room status
            SelfJungleMaestro_RoomStatus _lastRoomStatus = db.SelfJungleMaestro_RoomStatus.OrderByDescending(c => c.Id).FirstOrDefault();

            //get last room status
            List<SelfJungleMaestro_PlantStatus> _plantsStatusDistinct = db.SelfJungleMaestro_PlantStatus.GroupBy(c => c.Name).Select(c => c.FirstOrDefault()).ToList();
            List<SelfJungleMaestro_PlantStatus> _plantsStatusToReturn = new List<SelfJungleMaestro_PlantStatus>();

            foreach (SelfJungleMaestro_PlantStatus _plant in _plantsStatusDistinct)
            {
                SelfJungleMaestro_PlantStatus _plantStatus = db.SelfJungleMaestro_PlantStatus.Where(c => c.Name.ToLower() == _plant.Name.ToLower()).OrderByDescending(c => c.Id).FirstOrDefault();
                if (_plantStatus != null)
                {
                    _plantsStatusToReturn.Add(_plantStatus);
                }
            }

            return View(new Models.SelfJungleMaestroPlantsViewModel
            {
                RoomStatus = _lastRoomStatus,
                Plants = _plantsStatusToReturn
            });
        }

        [Authorize]
        public ActionResult MirandaGarden()
        {
            //get last room status
            SelfJungleMaestro_RoomStatus _lastRoomStatus = db.SelfJungleMaestro_RoomStatus.Where(c => c.Name.Equals("mirandaGarden")).OrderByDescending(c => c.Id).FirstOrDefault();

            //get last room status
            List<SelfJungleMaestro_PlantStatus> _plantsStatusDistinct = db.SelfJungleMaestro_PlantStatus.GroupBy(c => c.Name).Select(c => c.FirstOrDefault()).ToList();
            List<SelfJungleMaestro_PlantStatus> _plantsStatusToReturn = new List<SelfJungleMaestro_PlantStatus>();

            foreach (SelfJungleMaestro_PlantStatus _plant in _plantsStatusDistinct)
            {
                if (_plant.Name.Equals("1 - SH                                                                                              ") ||
                    _plant.Name.Equals("2 - SH                                                                                              ") ||
                    _plant.Name.Equals("3 - SH                                                                                              ") ||
                    _plant.Name.Equals("4 - AM                                                                                              ") ||
                    _plant.Name.Equals("5 - AM                                                                                              ") ||
                    _plant.Name.Equals("6 - AM                                                                                              ") ||
                    _plant.Name.Equals("7 - CR                                                                                              ") ||
                    _plant.Name.Equals("8 - MO                                                                                              ") ||
                    _plant.Name.Equals("9 - SK                                                                                              "))
                {
                    SelfJungleMaestro_PlantStatus _plantStatus = db.SelfJungleMaestro_PlantStatus.Where(c => c.Name.ToLower() == _plant.Name.ToLower()).OrderByDescending(c => c.Id).FirstOrDefault();
                    if (_plantStatus != null)
                    {
                        _plantsStatusToReturn.Add(_plantStatus);
                    }
                }

            }

            return View("plants", new Models.SelfJungleMaestroPlantsViewModel
            {
                RoomStatus = _lastRoomStatus,
                Plants = _plantsStatusToReturn
            });
        }
        // GET: SelfJungleMaestro
        [Authorize]
        public ActionResult Plant(string name)
        {
            string dateAxis = "[";
            string temperatureAxis = "[";
            string moistureAxis = "[";
            string fertilityAxis = "[";
            string sunlightAxis = "[";

            foreach (SelfJungleMaestro_PlantStatus _pantStatus in db.SelfJungleMaestro_PlantStatus.Where(c => c.Name.ToLower() == name))
            {
                dateAxis += "\"" + _pantStatus.CreateDate.Value.ToShortDateString() + " " + _pantStatus.CreateDate.Value.ToShortTimeString() + "\",";
                temperatureAxis += _pantStatus.Temperature.ToString() + ",";
                moistureAxis += _pantStatus.Moisture.ToString() + ",";
                fertilityAxis += _pantStatus.Fertility.ToString() + ",";
                sunlightAxis += _pantStatus.Sunlight.ToString() + ",";
            }

            dateAxis = dateAxis.Remove(dateAxis.Length - 1) + "]";
            temperatureAxis = temperatureAxis.Remove(temperatureAxis.Length - 1) + "]";
            moistureAxis = moistureAxis.Remove(moistureAxis.Length - 1) + "]";
            fertilityAxis = fertilityAxis.Remove(fertilityAxis.Length - 1) + "]";
            sunlightAxis = sunlightAxis.Remove(sunlightAxis.Length - 1) + "]";

            return View(new Models.SelfJungleMaestroViewModel
            {
                PlantName = name,
                DateAxis = dateAxis,
                TemperatureAxis = temperatureAxis,
                MoistureAxis = moistureAxis,
                FertilityAxis = fertilityAxis,
                SunlightAxis = sunlightAxis
            });
        }

        [Authorize]
        public ActionResult RoomStatus()
        {
            string dateAxis = "[";
            string temperatureAxis = "[";
            string moistureAxis = "[";
            string fertilityAxis = "[";
            string sunlightAxis = "[";
            foreach (SelfJungleMaestro_RoomStatus _roomStatus in db.SelfJungleMaestro_RoomStatus.ToList())
            {
                dateAxis += "\"" + _roomStatus.CreateDate.Value.ToShortDateString() + " " + _roomStatus.CreateDate.Value.ToShortTimeString() + "\",";
                temperatureAxis += _roomStatus.Temperature.ToString() + ",";
                moistureAxis += _roomStatus.Humidity.ToString() + ",";
            }

            dateAxis = dateAxis.Remove(dateAxis.Length - 1) + "]";
            temperatureAxis = temperatureAxis.Remove(temperatureAxis.Length - 1) + "]";
            moistureAxis = moistureAxis.Remove(moistureAxis.Length - 1) + "]";

            return View(new Models.SelfJungleMaestroViewModel
            {
                DateAxis = dateAxis,
                TemperatureAxis = temperatureAxis,
                MoistureAxis = moistureAxis
            });
        }

        // GET: SaveFlowerState
        [HttpGet]
        public ActionResult SaveFlowerState(string flowerName, string temperature, string moisture, string fertility, string sunlight)
        {
            try
            {
                db.SelfJungleMaestro_PlantStatus.Add(new SelfJungleMaestro_PlantStatus
                {
                    Name = flowerName,
                    Temperature = int.Parse(temperature),
                    Moisture = int.Parse(moisture),
                    Fertility = int.Parse(fertility),
                    Sunlight = int.Parse(sunlight),
                    CreateDate = DateTime.Now
                });
                db.SaveChanges();

                //if moisture below 26 send message
                if (int.Parse(moisture) < 26)
                {
                    if (Session[flowerName + "Moisture"] == null)
                    {
                        //Helpers.FirebasePlantsAndroid.SendNotificationToAndroidPhone("dZv63ZrBY4M:APA91bHbDy4iMppQuirgMmhf5lpBKbTXCAfJ7HKnl0zbdGuh8C1weBOqgzMLf7N6liGI1jNd0C6oF5igBT860KjIbENK83UPFzv3Nqf_nUYOaVQQy7gYy5igt5i8UEY7KdMzSQebJolU", "plantStatus:" + flowerName + " is at " + moisture.ToString() + "%. It needs you´re nourishment!");
                        //Save to session
                        Session[flowerName + "Moisture"] = int.Parse(moisture);
                    }
                    //only show if it´s different
                    else if (int.Parse(Session[flowerName + "Moisture"].ToString()) != int.Parse(moisture))
                    {
                        //Helpers.FirebasePlantsAndroid.SendNotificationToAndroidPhone("dZv63ZrBY4M:APA91bHbDy4iMppQuirgMmhf5lpBKbTXCAfJ7HKnl0zbdGuh8C1weBOqgzMLf7N6liGI1jNd0C6oF5igBT860KjIbENK83UPFzv3Nqf_nUYOaVQQy7gYy5igt5i8UEY7KdMzSQebJolU", "plantStatus:" + flowerName + " is at " + moisture.ToString() + "%. It needs you´re nourishment!");
                        //Save to session
                        Session[flowerName + "Moisture"] = int.Parse(moisture);
                    }
                }
                return Json("Succesful!", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public void SendSms(string phoneNumber, string message)
        {
            string URL = "https://rest.nexmo.com/sms/json";
            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(URL);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            Stream reqStream = webRequest.GetRequestStream();
            string postData = "from=Acme Inc&text=" + message + "&to=" + phoneNumber + "&api_key=f53a7864&api_secret=KXSGuZTsgYDYR44X";

            byte[] postArray = Encoding.ASCII.GetBytes(postData);
            reqStream.Write(postArray, 0, postArray.Length);
            reqStream.Close();
            StreamReader sr = new StreamReader(webRequest.GetResponse().GetResponseStream());
            string Result = sr.ReadToEnd();

            //using (TextWriter tw = new StreamWriter("c:\\result.csv", true))
            //{
            //    tw.Write(Result);
            //}
            sr.Close();
            reqStream.Close();
        }

        // GET: SaveRoomTemperatureAndHumidity
        [HttpGet]
        public ActionResult SaveRoomTemperatureAndHumidity(string roomName, string temperature, string humidity)
        {
            try
            {
                db.SelfJungleMaestro_RoomStatus.Add(new SelfJungleMaestro_RoomStatus
                {
                    Name = roomName,
                    Temperature = int.Parse(temperature),
                    Humidity = int.Parse(humidity),
                    CreateDate = DateTime.Now
                });
                db.SaveChanges();
                return Json("Succesful!", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetPlants()
        {
            try
            {
                List<SelfJungleMaestro_PlantStatus> _plantsStatusDistinct = db.SelfJungleMaestro_PlantStatus.GroupBy(c => c.Name).Select(c => c.FirstOrDefault()).ToList();
                List<SelfJungleMaestro_PlantStatus> _plantsStatusToReturn = new List<SelfJungleMaestro_PlantStatus>();

                foreach (SelfJungleMaestro_PlantStatus _plant in _plantsStatusDistinct)
                {
                    SelfJungleMaestro_PlantStatus _plantStatus = db.SelfJungleMaestro_PlantStatus.Where(c => c.Name.ToLower() == _plant.Name.ToLower()).OrderByDescending(c => c.Id).FirstOrDefault();
                    if (_plantStatus != null)
                    {
                        _plantsStatusToReturn.Add(_plantStatus);
                    }
                }
                return Json(_plantsStatusToReturn, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }


        }
    }
}