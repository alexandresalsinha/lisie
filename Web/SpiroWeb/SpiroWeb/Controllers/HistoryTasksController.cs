using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SpiroWeb.Controllers
{
    public class HistoryTasksController : BaseTasksController
    {
        public ActionResult Index()
        {
#if RELEASE
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
#endif

            var _fileText = Server.MapPath("~/App_Data/HistoryTasks.json");

            List<Models.HistoryTask> _HistoryTasks = new List<Models.HistoryTask>();
            if (System.IO.File.Exists(_fileText))
            {
                string _json = System.IO.File.ReadAllText(_fileText);
                if (_json != string.Empty)
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    _HistoryTasks = jss.Deserialize<List<Models.HistoryTask>>(_json);

                    _HistoryTasks = _HistoryTasks.OrderByDescending(c => c.date).ToList();
                }
            }
            return View(_HistoryTasks);
        }

        public string GetHistoryTasksJson()
        {
            var dataFile = Server.MapPath("~/App_Data/HistoryTasks.json");

            if (System.IO.File.Exists(dataFile))
            {
                string jsonTasks = System.IO.File.ReadAllText(dataFile);
                if (jsonTasks != string.Empty)
                {
                    return jsonTasks;
                }
            }
            return string.Empty;
        }

        public string GetHistoryPointsJson()
        {
            var dataFile = Server.MapPath("~/App_Data/HistoryPoints.json");

            if (System.IO.File.Exists(dataFile))
            {
                string jsonTasks = System.IO.File.ReadAllText(dataFile);
                if (jsonTasks != string.Empty)
                {
                    return jsonTasks;
                }
            }
            return string.Empty;
        }

        [HttpPost]
        public JsonResult AddTaskToHistory(int taskId, string taskLabel, string taskCategory, string taskAttribute, bool isDone, int points, string levels)
        {
            JavaScriptSerializer _JavaScriptSerializer = new JavaScriptSerializer();
            string[] stringArray = _JavaScriptSerializer.Deserialize<string[]>(levels);

            Models.HistoryTask _HistoryTask = new Models.HistoryTask();
            _HistoryTask.id = GetNextHistoryId();
            _HistoryTask.taskId = taskId;
            _HistoryTask.label = taskLabel;
            _HistoryTask.category = taskCategory;
            _HistoryTask.attribute = taskAttribute;
            _HistoryTask.isDone = isDone;
            _HistoryTask.date = DateTime.Now;
            _HistoryTask.points = points;
            int _counter = 1;
            if (stringArray != null)
            {
                _HistoryTask.levels = new List<Models.HistoryTaskLevels>();
                foreach (string level in stringArray)
                {
                    _HistoryTask.levels.Add(new Models.HistoryTaskLevels { level = _counter, label = level });
                    _counter++;
                }
            }

            //JavaScriptSerializer _JavaScriptSerializer = new JavaScriptSerializer();
            string historyTaskSerialized = _JavaScriptSerializer.Serialize(_HistoryTask);

            string existingHistoryTasks = GetHistoryTasksJson();

            List<Models.HistoryTask> _historyTasks = _JavaScriptSerializer.Deserialize<List<Models.HistoryTask>>(existingHistoryTasks);
            _historyTasks.Add(_HistoryTask);

            //existingHistoryTasks = existingHistoryTasks.Insert(existingHistoryTasks.Length - 2, "," + historyTaskSerialized);

            var dataFile = Server.MapPath("~/App_Data/HistoryTasks.json");
            //System.IO.File.WriteAllText(dataFile, existingHistoryTasks);
            System.IO.File.WriteAllText(dataFile, _JavaScriptSerializer.Serialize(_historyTasks));
            return null;
        }

        [HttpPost]
        public JsonResult RemoveTaskFromHistory(int taskId, string taskCategory)
        {
            var dataFile = Server.MapPath("~/App_Data/HistoryTasks.json");

            if (System.IO.File.Exists(dataFile))
            {
                string jsonTasks = System.IO.File.ReadAllText(dataFile);
                if (jsonTasks != string.Empty)
                {
                    JavaScriptSerializer _JavaScriptSerializer = new JavaScriptSerializer();
                    List<Models.HistoryTask> _historyTasks = _JavaScriptSerializer.Deserialize<List<Models.HistoryTask>>(jsonTasks);
                    Models.HistoryTask _taskToRemove = (from c in _historyTasks where c.taskId.Equals(taskId) && c.category.Equals(taskCategory) select c).First();
                    if (_taskToRemove != null)
                    {
                        _historyTasks.Remove(_taskToRemove);
                        string serializedHIstoryTasks = _JavaScriptSerializer.Serialize(_historyTasks);
                        System.IO.File.WriteAllText(dataFile, serializedHIstoryTasks);
                    }
                    return null;
                }
            }

            return null;
        }

        [HttpPost]
        public JsonResult AddPointsToHistory(int taskId, string taskCategory, string taskAttribute, int taskPoints, int newAttributePoints, int newCategoryPoints, int newGlobalPoints, int newGlobalLevel)
        {
            JavaScriptSerializer _JavaScriptSerializer = new JavaScriptSerializer();

            Models.HistoryPoints _HistoryPoints = new Models.HistoryPoints();
            _HistoryPoints.id = GetNextPointsHistoryId();
            _HistoryPoints.taskId = taskId;
            _HistoryPoints.category = taskCategory;
            _HistoryPoints.attribute = taskAttribute;
            _HistoryPoints.date = DateTime.Now;
            _HistoryPoints.points = taskPoints;
            _HistoryPoints.newCategoryPoints = newCategoryPoints;
            _HistoryPoints.newAttributePoints = newAttributePoints;
            _HistoryPoints.newGlobalLevel = newGlobalLevel;
            _HistoryPoints.newGlobalPoints = newGlobalPoints;
            _HistoryPoints.newCategoryPoints = newCategoryPoints;

            string historyPointSerialized = _JavaScriptSerializer.Serialize(_HistoryPoints);

            string existingHistoryPoints = GetHistoryPointsJson();

            List<Models.HistoryPoints> _historyPoints = _JavaScriptSerializer.Deserialize<List<Models.HistoryPoints>>(existingHistoryPoints);
            _historyPoints.Add(_HistoryPoints);


            var dataFile = Server.MapPath("~/App_Data/HistoryPoints.json");
            System.IO.File.WriteAllText(dataFile, _JavaScriptSerializer.Serialize(_historyPoints));
            return null;
        }


        public JsonResult RemovePointsFromHistory(int taskId, string taskCategory)
        {
            var dataFile = Server.MapPath("~/App_Data/HistoryPoints.json");

            if (System.IO.File.Exists(dataFile))
            {
                string jsonTasks = System.IO.File.ReadAllText(dataFile);
                if (jsonTasks != string.Empty)
                {
                    JavaScriptSerializer _JavaScriptSerializer = new JavaScriptSerializer();
                    List<Models.HistoryPoints> _historyPoints = _JavaScriptSerializer.Deserialize<List<Models.HistoryPoints>>(jsonTasks);
                    List<Models.HistoryPoints> _tasksToRemove = (from c in _historyPoints where c.taskId.Equals(taskId) && c.category.Equals(taskCategory) select c).ToList();
                    if (_tasksToRemove != null)
                    {
                        foreach (var _taskToRemove in _tasksToRemove)
                        {
                            _historyPoints.Remove(_taskToRemove);
                        }

                        string serializedHIstoryTasks = _JavaScriptSerializer.Serialize(_historyPoints);
                        System.IO.File.WriteAllText(dataFile, serializedHIstoryTasks);
                    }
                    return null;
                }
            }

            return null;
        }

        public int GetNextHistoryId()
        {
            var dataFile = Server.MapPath("~/App_Data/HistoryTasks.json");

            if (System.IO.File.Exists(dataFile))
            {
                string jsonTasks = System.IO.File.ReadAllText(dataFile);
                if (jsonTasks != string.Empty)
                {
                    JavaScriptSerializer _JavaScriptSerializer = new JavaScriptSerializer();
                    List<Models.HistoryTask> _historyTasks = _JavaScriptSerializer.Deserialize<List<Models.HistoryTask>>(jsonTasks);
                    return _historyTasks.Count + 1;
                }
            }
            return -1;
        }

        public int GetNextPointsHistoryId()
        {
            var dataFile = Server.MapPath("~/App_Data/HistoryPoints.json");

            if (System.IO.File.Exists(dataFile))
            {
                string jsonTasks = System.IO.File.ReadAllText(dataFile);
                if (jsonTasks != string.Empty)
                {
                    JavaScriptSerializer _JavaScriptSerializer = new JavaScriptSerializer();
                    List<Models.HistoryPoints> _historyPoints = _JavaScriptSerializer.Deserialize<List<Models.HistoryPoints>>(jsonTasks);
                    return _historyPoints.Count + 1;
                }
            }
            return -1;
        }

    }
}
