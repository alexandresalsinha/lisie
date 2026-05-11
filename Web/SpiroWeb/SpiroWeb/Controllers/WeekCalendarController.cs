using SpiroWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SpiroWeb.Controllers
{
    public class WeekCalendarController : BaseTasksController
    {
        //
        // GET: /WeekCalendar/

        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetWeeklyCalendarJson()
        {

            if (System.IO.File.Exists(Server.MapPath("~/App_Data/WeekCalendar.json")))
            {
                string jsonTasks = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/WeekCalendar.json"));
                if (jsonTasks != string.Empty)
                {
                    return Json(jsonTasks, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        [HttpPost]
        public JsonResult AddTaskToWeek(WeekTask newTask)
        {
            WeekTaskEvents _WeekTaskEvents = GetListFromJsonFile();
            int _lastIndex = 0;
            if (_WeekTaskEvents.events.Count > 0)
            {
                _lastIndex = _WeekTaskEvents.events.Max(e => e.id);
            }

            newTask.id = ++_lastIndex;
            _WeekTaskEvents.events.Add(newTask);

            SaveListToJsonFile(_WeekTaskEvents);

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateScheduledTask(WeekTask newTask)
        {
            WeekTaskEvents _WeekTaskEvents = GetListFromJsonFile();
            WeekTask _weekTask = _WeekTaskEvents.events.Find(e => e.id == newTask.id);
            //int _lastIndex = _WeekTaskEvents.events.Max(e => e.id);

            DateTime _dtStart = DateTime.Parse(newTask.start.Remove(newTask.start.IndexOf("GMT"), newTask.start.Length - newTask.start.IndexOf("GMT")));
            DateTime _dtEnd = DateTime.Parse(newTask.end.Remove(newTask.start.IndexOf("GMT"), newTask.end.Length - newTask.end.IndexOf("GMT")));

            string _newStart = _dtStart.ToString("yyyy-MM-dd") + "T" + _dtStart.ToString("HH:mm") + ".00.000+10:00";
            string _newEnd = _dtEnd.ToString("yyyy-MM-dd") + "T" + _dtEnd.ToString("HH:mm") + ".00.000+10:00";
            _weekTask.start = _newStart;
            _weekTask.end = _newEnd;

            SaveListToJsonFile(_WeekTaskEvents);

            return Json("", JsonRequestBehavior.AllowGet);
        }
        private Models.WeekTaskEvents GetListFromJsonFile()
        {
            var _jsonsFile = Server.MapPath("~/App_Data/WeekCalendar.json");

            if (System.IO.File.Exists(_jsonsFile))
            {
                string _json = System.IO.File.ReadAllText(_jsonsFile);
                if (_json != string.Empty)
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    WeekTaskEvents _WeekTaskEvents = jss.Deserialize<WeekTaskEvents>(_json);

                    return _WeekTaskEvents;
                }
            }
            return null;
        }



        private bool SaveListToJsonFile(WeekTaskEvents weekTaskEvents)
        {
            try
            {
                var dataFile = Server.MapPath("~/App_Data/WeekCalendar.json");
                System.IO.File.WriteAllText(dataFile, new JavaScriptSerializer().Serialize(weekTaskEvents));
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        [HttpPost]
        public JsonResult ChangeTaskDoneStatus(int id, int taskId, bool isDone, string category)
        {
            List<Tasks> _tasks = GetTasksLists(category);
            Tasks _taskWithId = _tasks.Find(e => e.id == taskId.ToString());

            if (_taskWithId != null)
            {
                _taskWithId.isDone = isDone;
            }
            SaveTasks(_tasks, category);

            //change week task
            WeekTaskEvents _WeekTaskEvents = GetListFromJsonFile();
            WeekTask _weekTask = _WeekTaskEvents.events.Find(e => e.id == id);
            _weekTask.isDone = isDone;
            SaveListToJsonFile(_WeekTaskEvents);

            return Json(_taskWithId, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteScheduleTask(int id)
        {
            WeekTaskEvents _WeekTaskEvents = GetListFromJsonFile();
            WeekTask _weekTask = _WeekTaskEvents.events.Find(e => e.id == id);
            //int _lastIndex = _WeekTaskEvents.events.Max(e => e.id);

            _WeekTaskEvents.events.Remove(_weekTask);

            SaveListToJsonFile(_WeekTaskEvents);

            return Json("", JsonRequestBehavior.AllowGet);
        }

        //public List<Models.Tasks> GetTasksLists(string category)
        //{
        //    //List<Models.Tasks> _listForType = new List<Models.Tasks>();
        //    //List<Models.Tasks> _tasks =  (List<Models.Tasks>)Helpers.Json.GetDeSerializedObjectFromFile(_listForType.GetType(), Server.MapPath("~/App_Data/WorkTasks.json"));

        //    Models.Tasks _listForType = new Models.Tasks();
        //    object _tasks = Helpers.Json.GetDeSerializedObjectFromFile(_listForType.GetType(), Server.MapPath("~/App_Data/WorkTasks.json"));

        //    switch (category)
        //    {
        //        case "work":
        //            return (List<Models.Tasks>)Helpers.Json.GetDeSerializedObjectFromFile(_listForType.GetType(), Server.MapPath("~/App_Data/WorkTasks.json"));
        //        case "social":
        //            return (List<Models.Tasks>)Helpers.Json.GetDeSerializedObjectFromFile(_listForType.GetType(), Server.MapPath("~/App_Data/SocialTasks.json"));
        //        case "personal":
        //            return (List<Models.Tasks>)Helpers.Json.GetDeSerializedObjectFromFile(_listForType.GetType(), Server.MapPath("~/App_Data/PersonalTasks.json"));
        //        default:
        //            break;
        //    }
        //    return new List<Models.Tasks>();
        //}

        //public void SaveTasks(List<Models.Tasks> tasks, string category)
        //{
        //    string filePathToSave = string.Empty;
        //    switch (category)
        //    {
        //        case "work":
        //            filePathToSave = Server.MapPath("~/App_Data/WorkTasks.json");
        //            break;
        //        case "social":
        //            filePathToSave = Server.MapPath("~/App_Data/SocialTasks.json");
        //            break;
        //        case "personal":
        //            filePathToSave = Server.MapPath("~/App_Data/PersonalTasks.json");
        //            break;
        //        default:
        //            break;
        //    }

        //    Helpers.Json.SaveObjectToJsonFile(tasks, filePathToSave);
        //}
    }
}
