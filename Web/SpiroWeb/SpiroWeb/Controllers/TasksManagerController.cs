using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SpiroWeb.Controllers
{
    public class GlobalPoints
    {
        public int Points { get; set; }
        public int Level { get; set; }
        public int Intellegence { get; set; }
        public int Perseverance { get; set; }
        public int Strength { get; set; }
        public int Vitality { get; set; }
        public int Creativity { get; set; }
        public int Charisma { get; set; }

        public int Work { get; set; }

        public int Social { get; set; }

        public int Personal { get; set; }
    }

    public class TasksManagerController : Controller
    {
        private SpiroStockManagementEntities db = new SpiroStockManagementEntities();


        public JsonResult UpdateCategoryPoints()
        {
            var dataFile = Server.MapPath("~/App_Data/PersonalTasks.json");
            int totalPersonalTasks = 0;
            if (System.IO.File.Exists(dataFile))
            {
                string jsonTasks = System.IO.File.ReadAllText(dataFile);
                JavaScriptSerializer jss = new JavaScriptSerializer();
                List<Models.Tasks> _globalPoints = jss.Deserialize<List<Models.Tasks>>(jsonTasks);

                var result = new
                {
                    Points = _globalPoints.Sum(t => t.importance * t.difficulty),

                };
                totalPersonalTasks = result.Points;
            }

            int totalSocialTasks = 0;
            dataFile = Server.MapPath("~/App_Data/SocialTasks.json");
            if (System.IO.File.Exists(dataFile))
            {
                string jsonTasks = System.IO.File.ReadAllText(dataFile);
                JavaScriptSerializer jss = new JavaScriptSerializer();
                List<Models.Tasks> _globalPoints = jss.Deserialize<List<Models.Tasks>>(jsonTasks);

                var result = new
                {
                    Points = _globalPoints.Sum(t => t.importance * t.difficulty),

                };
                totalSocialTasks = result.Points;
            }

            int totalWorkTasks = 0;
            dataFile = Server.MapPath("~/App_Data/WorkTasks.json");
            if (System.IO.File.Exists(dataFile))
            {
                string jsonTasks = System.IO.File.ReadAllText(dataFile);
                JavaScriptSerializer jss = new JavaScriptSerializer();
                List<Models.Tasks> _globalPoints = jss.Deserialize<List<Models.Tasks>>(jsonTasks);

                var result = new
                {
                    Points = _globalPoints.Sum(t => t.importance * t.difficulty),

                };
                totalWorkTasks = result.Points;
            }


            string pointsFile = Server.MapPath("~/App_Data/Points.json");
            if (System.IO.File.Exists(pointsFile))
            {
                string jsonPoints = System.IO.File.ReadAllText(pointsFile);
                if (jsonPoints != string.Empty)
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    GlobalPoints _globalPoints = jss.Deserialize<GlobalPoints>(jsonPoints);

                    _globalPoints.Social = totalSocialTasks;
                    _globalPoints.Personal = totalPersonalTasks;
                    _globalPoints.Work = totalWorkTasks;

                    System.IO.File.WriteAllText(pointsFile, new JavaScriptSerializer().Serialize(_globalPoints));
                }
            }


            return Json(null);
        }

        public ActionResult Index()
        {
            //Get Current Points and Level
#if RELEASE
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
#endif

            var pointsFile = Server.MapPath("~/App_Data/Points.json");

            if (System.IO.File.Exists(pointsFile))
            {
                string jsonPoints = System.IO.File.ReadAllText(pointsFile);
                if (jsonPoints != string.Empty)
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    GlobalPoints _globalPoints = jss.Deserialize<GlobalPoints>(jsonPoints);

                    ViewBag.Level = _globalPoints.Level;
                    ViewBag.Points = _globalPoints.Points;

                    ViewBag.Intellegence = _globalPoints.Intellegence;
                    ViewBag.IntellegencePointsPlus = (int)(_globalPoints.Intellegence / 500) + 1;

                    ViewBag.Perseverance = _globalPoints.Perseverance;
                    ViewBag.PerseverancePointsPlus = (int)(_globalPoints.Perseverance / 500) + 1;

                    ViewBag.Strength = _globalPoints.Strength;
                    ViewBag.StrengthPointsPlus = (int)(_globalPoints.Strength / 500) + 1;

                    ViewBag.Vitality = _globalPoints.Vitality;
                    ViewBag.VitalityPointsPlus = (int)(_globalPoints.Vitality / 500) + 1;

                    ViewBag.Creativity = _globalPoints.Creativity;
                    ViewBag.CreativityPointsPlus = (int)(_globalPoints.Creativity / 500) + 1;

                    ViewBag.Charisma = _globalPoints.Charisma;
                    ViewBag.CharismaPointsPlus = (int)(_globalPoints.Charisma / 500) + 1;

                    ViewBag.Social = _globalPoints.Social;
                    ViewBag.Work = _globalPoints.Work;
                    ViewBag.Personal = _globalPoints.Personal;

                    SetPointsPercentageForJavascript(_globalPoints);
                }


            }
            return View();
        }

        public void SetPointsPercentageForJavascript(GlobalPoints globalPoints)
        {
            //intellegence
            ViewBag.IntellegencePointsPercentage = CalculatePointsPercentage(globalPoints.Intellegence);
            ViewBag.PerseverancePointsPercentage = CalculatePointsPercentage(globalPoints.Perseverance);
            ViewBag.StrengthPointsPercentage = CalculatePointsPercentage(globalPoints.Strength);
            ViewBag.VitalityPointsPercentage = CalculatePointsPercentage(globalPoints.Vitality);
            ViewBag.CreativityPointsPercentage = CalculatePointsPercentage(globalPoints.Creativity);
            ViewBag.CharismaPointsPercentage = CalculatePointsPercentage(globalPoints.Charisma);
        }

        public int CalculatePointsPercentage(int points)
        {
            int _math = (int)(points / 500);
            _math = 500 * _math;
            _math = points - _math;

            decimal _percentage = (decimal)((decimal)_math / 500);
            _percentage = _percentage * 100;

            return (int)_percentage;
        }

        public JsonResult GetTasks()
        {
            var dataFile = Server.MapPath("~/App_Data/WorkTasks.json");

            if (System.IO.File.Exists(dataFile))
            {
                string jsonTasks = System.IO.File.ReadAllText(dataFile);
                if (jsonTasks != string.Empty)
                {
                    return Json(jsonTasks, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("[{\"id\":0,\"parentId\":null,\"label\":\"Item 1\",\"isDone\":false}," +
                                    "{\"id\":1,\"parentId\":null,\"label\":\"Item 2\",\"isDone\":false}," +
                                    "{\"id\":2,\"parentId\":0,\"label\":\"Item 1-1\",\"isDone\":true}," +
                                    "{\"id\":3,\"parentId\":0,\"label\":\"Item 1-2\",\"isDone\":false}," +
                                    "{\"id\":4,\"parentId\":3,\"label\":\"Item 1-2-1\",\"isDone\":true}," +
                                    "{\"id\":5,\"parentId\":1,\"label\":\"Item 2-1\",\"isDone\":false}," +
                                    "{\"id\":6,\"parentId\":1,\"label\":\"Item 2-2\",\"isDone\":false}," +
                                    "{\"id\":7,\"parentId\":4,\"label\":\"teste\",\"isDone\":false}," +
                                    "{\"id\":8,\"parentId\":3,\"label\":\"teste\",\"isDone\":false}," +
                                    "{\"id\":9,\"parentId\":2,\"label\":\"teste\",\"isDone\":false}]",
                                    JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSocialTasks()
        {
            var dataFile = Server.MapPath("~/App_Data/SocialTasks.json");

            if (System.IO.File.Exists(dataFile))
            {
                string jsonTasks = System.IO.File.ReadAllText(dataFile);
                if (jsonTasks != string.Empty)
                {
                    return Json(jsonTasks, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        [HttpPost]
        public JsonResult SaveTasks(string jsonItems)
        {
            //Session["agoraequee"] = jsonItems;
            var dataFile = Server.MapPath("~/App_Data/WorkTasks.json");
            System.IO.File.WriteAllText(dataFile, jsonItems);
            return null;
            //return Json(\"[{\\"id\\":0,\\"parentId\\":null,\\"label\\":\\"Item 1\\",\\"isDone\\":false},{\\"id\\":1,\\"parentId\\":null,\\"label\\":\\"Item 2\\",\\"isDone\\":false},{\\"id\\":2,\\"parentId\\":0,\\"label\\":\\"Item 1-1\\",\\"isDone\\":true},{\\"id\\":3,\\"parentId\\":0,\\"label\\":\\"Item 1-2\\",\\"isDone\\":false},{\\"id\\":4,\\"parentId\\":3,\\"label\\":\\"Item 1-2-1\\",\\"isDone\\":true},{\\"id\\":5,\\"parentId\\":1,\\"label\\":\\"Item 2-1\\",\\"isDone\\":false},{\\"id\\":6,\\"parentId\\":1,\\"label\\":\\"Item 2-2\\",\\"isDone\\":false},{\\"id\\":7,\\"parentId\\":4,\\"label\\":\\"testesdfsdf\\",\\"isDone\\":false}]\", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSocialTasks(string jsonItems)
        {
            var dataFile = Server.MapPath("~/App_Data/SocialTasks.json");
            System.IO.File.WriteAllText(dataFile, jsonItems);
            return null;
        }

        [HttpPost]
        public JsonResult SavePoints(int points, int level, int intelligencePoints, int perseverancePoints, int strengthPoints, int vitalityPoints, int creativityPoints, int charismaPoints, int workPoints, int socialPoints, int personalPoints)
        {

            GlobalPoints _gp = new GlobalPoints { Points = points, Level = level, Intellegence = intelligencePoints, Perseverance = perseverancePoints, Strength = strengthPoints, Vitality = vitalityPoints, Creativity = creativityPoints, Charisma = charismaPoints, Personal = personalPoints, Work = workPoints, Social = socialPoints };

            var dataFile = Server.MapPath("~/App_Data/Points.json");
            System.IO.File.WriteAllText(dataFile, new JavaScriptSerializer().Serialize(_gp));
            return null;
        }

        public JsonResult GetPersonalTasks()
        {
            var dataFile = Server.MapPath("~/App_Data/PersonalTasks.json");

            if (System.IO.File.Exists(dataFile))
            {
                string jsonTasks = System.IO.File.ReadAllText(dataFile);
                if (jsonTasks != string.Empty)
                {
                    return Json(jsonTasks, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        [HttpPost]
        public JsonResult SavePersonalTasks(string jsonItems)
        {
            var dataFile = Server.MapPath("~/App_Data/PersonalTasks.json");
            System.IO.File.WriteAllText(dataFile, jsonItems);
            return null;
        }


        public List<Models.Tasks> GetTasksLists(string category)
        {
            Models.Tasks _listForType = new Models.Tasks();
            //object _tasks = Helpers.Json.GetDeSerializedObjectFromFile(_listForType.GetType(), Server.MapPath("~/App_Data/WorkTasks.json"));

            switch (category)
            {
                case "work":
                    return (List<Models.Tasks>)Helpers.Json.GetDeSerializedObjectFromFile(_listForType.GetType(), Server.MapPath("~/App_Data/WorkTasks.json"));
                case "social":
                    return (List<Models.Tasks>)Helpers.Json.GetDeSerializedObjectFromFile(_listForType.GetType(), Server.MapPath("~/App_Data/SocialTasks.json"));
                case "personal":
                    return (List<Models.Tasks>)Helpers.Json.GetDeSerializedObjectFromFile(_listForType.GetType(), Server.MapPath("~/App_Data/PersonalTasks.json"));
                default:
                    break;
            }
            return new List<Models.Tasks>();
        }

        public void SaveTasks(List<Models.Tasks> tasks, string category)
        {
            string filePathToSave = string.Empty;
            switch (category)
            {
                case "work":
                    filePathToSave = Server.MapPath("~/App_Data/WorkTasks.json");
                    break;
                case "social":
                    filePathToSave = Server.MapPath("~/App_Data/SocialTasks.json");
                    break;
                case "personal":
                    filePathToSave = Server.MapPath("~/App_Data/PersonalTasks.json");
                    break;
                default:
                    break;
            }

            Helpers.Json.SaveObjectToJsonFile(tasks, filePathToSave);
        }

        #region Endpoints
        // GET: SaveFlowerState
        [HttpGet]
        public ActionResult AddTodayTask(string task)
        {
            try
            {
                db.TaskManagerTasks.Add(new TaskManagerTasks
                {
                    Task = task,
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
        public ActionResult TodayTaskDone(string task)
        {
            try
            {
                //check i a equal task exists
                //if it does don´t add it
                var result = (from a in db.TaskManagerTasks
                              where (a.Task.ToLower().Contains(task.ToLower()) && !(a.Done.HasValue ? a.Done.Value : false))
                              select a);
                List<TaskManagerTasks> tasks = result.ToList<TaskManagerTasks>();
                if (tasks.Count() == 0)
                {
                    return Json(new Models.JsonBotResponse
                    {
                        Success = true,
                        Message = "no_tasks",
                        Extra = task
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (tasks.Count() == 1)
                {
                    tasks[0].Done = true;
                    db.SaveChanges();

                    //ask task category
                    return Json(new Models.JsonBotResponse
                    {
                        Success = true,
                        Message = "add_task_to_cloud",
                        Extra = task
                    }, JsonRequestBehavior.AllowGet);
                    //return Json("Sucessful, " + tasks[0].Task + ", marked as done", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new Models.JsonBotResponse
                    {
                        Success = true,
                        Message = "multiple_tasks",
                        Extra = tasks.Count().ToString()
                    }, JsonRequestBehavior.AllowGet);
                    //foreach (TaskManagerTasks _task in tasks)
                    //{
                    //    _task.Done = true;
                    //}
                    //return Json("Sucessful, " + tasks.Count() + " marked as done", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new Models.JsonBotResponse
                {
                    Success = false
                });
                //return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult TaskDone(string task, string category)
        {
            try
            {
                List<Models.Tasks> _categoryTasks = GetTasksLists(category);

                var _Tasks = _categoryTasks.Where(c => c.label.ToLower().Contains(task.ToLower())).ToList();
                if (_Tasks.Count == 1)
                {
                    //mark task as done
                    //return congratulations message
                }
                SaveTasks(_categoryTasks, category);

                //One more step to greatness;) . Your 4 points closer to it, in your personal matters.And you´ve increased your Perseverance, Vitality. Fuck yeah!

                return Json(new { text = "Succesful!", extra = _Tasks }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }
        }


        // GET: SaveFlowerState
        [HttpGet]
        public ActionResult GetTodayTasks()
        {
            try
            {
                var result = (from a in db.TaskManagerTasks
                              where (DbFunctions.TruncateTime(a.CreateDate).Value == DateTime.Today && !(a.Done.HasValue ? a.Done.Value : false))
                              select a);
                var tasks = result.ToList();
                return Json(tasks, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }
        }

        // GET: SaveFlowerState
        [HttpGet]
        public ActionResult GetYesterdayTasks()
        {
            try
            {
                DateTime yesterday = DateTime.Now.AddDays(-1);
                var result = (from a in db.TaskManagerTasks
                              where (DbFunctions.TruncateTime(a.CreateDate).Value == DbFunctions.TruncateTime(yesterday).Value && !(a.Done.HasValue ? a.Done.Value : false))
                              select a);
                var tasks = result.ToList();
                return Json(tasks, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }
        }

        // GET: SaveFlowerState
        [HttpGet]
        public ActionResult UndoneDailyTasks()
        {
            try
            {
                var result = (from a in db.TaskManagerTasks
                              where (!(a.Done.HasValue ? a.Done.Value : false))
                              select a);
                var tasks = result.ToList();
                return Json(tasks, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }
        }


        // GET: JSON Add task to category
        [HttpGet]
        public ActionResult AddTaskToCategory(string task, string category)
        {
            string _newIdStr = "";
            try
            {
                List<Models.Tasks> _categoryTasks = GetTasksLists(category);
                int _newId = int.Parse(_categoryTasks.Last().id) + 1;
                _newIdStr = _newId.ToString();
                _categoryTasks.Add(new Models.Tasks
                {
                    id = _newId.ToString(),
                    label = task,
                    attribute = "Perseverance",
                    importance = 2,
                    difficulty = 2,
                    isRepeatable = false,
                    isDone = false

                });

                SaveTasks(_categoryTasks, category);

                return Json(new { text = "Succesful!", extra = _newId.ToString() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Unsucessful - " + ex.Message + " , " + _newIdStr, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: JSON Add task to category
        [HttpGet]
        public ActionResult AddAttributesToTask(string taskId, string category, string attributes)
        {
            try
            {
                List<Models.Tasks> _categoryTasks = GetTasksLists(category);

                var _Task = _categoryTasks.Find(c => c.id.Equals(taskId));

                string newAttributes = string.Empty;
                if (attributes.ToLower().Contains("intelligence"))
                {
                    newAttributes = "Intelligence";
                }
                if (attributes.ToLower().Contains("perseverance"))
                {
                    if (newAttributes.Length == 0) newAttributes = "Perseverance";
                    else newAttributes += ",Perseverance";
                }
                if (attributes.ToLower().Contains("strength"))
                {
                    if (newAttributes.Length == 0) newAttributes = "Strength";
                    else newAttributes += ",Strength";
                }
                if (attributes.ToLower().Contains("vitality"))
                {
                    if (newAttributes.Length == 0) newAttributes = "Vitality";
                    else newAttributes += ",Vitality";
                }
                if (attributes.ToLower().Contains("creativity"))
                {
                    if (newAttributes.Length == 0) newAttributes = "Creativity";
                    else newAttributes += ",Creativity";
                }
                if (attributes.ToLower().Contains("charisma"))
                {
                    if (newAttributes.Length == 0) newAttributes = "Charisma";
                    else newAttributes += ",Charisma";
                }

                _Task.attribute = newAttributes;
                SaveTasks(_categoryTasks, category);

                return Json(new { text = "Succesful!", extra = _Task }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Unsucessful - " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: SaveFlowerState
        [HttpGet]
        public ActionResult KeepALive()
        {
            return Json("Keeping it alive", JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetRandomImportantTask(string category)
        {
            try
            {
                List<Models.Tasks> _categoryTasks = GetTasksLists(category);
                var _importantTasks = _categoryTasks.Where(c => c.importance == 3 && !c.isDone).ToList();
                string _finalText = string.Empty;
                if (_importantTasks.Count > 0)
                {
                    Random rnd = new Random();
                    int _randomTaskIndex = rnd.Next(0, _importantTasks.Count - 1);
                    //_finalText = _importantTasks[_randomTaskIndex].label;
                    bool _parentExists = (_importantTasks[_randomTaskIndex].parentId != null);
                    string _parentId = _importantTasks[_randomTaskIndex].parentId;

                    if (_parentExists) _finalText = "Sub task, " + _importantTasks[_randomTaskIndex].label + ". " + _finalText;
                    else _finalText = "Main task, " + _importantTasks[_randomTaskIndex].label + ". " + _finalText;

                    while (_parentExists)
                    {
                        var _parentTask = _categoryTasks.Where(c => c.id.Equals(_parentId)).First();
                        if (_parentTask != null)
                        {
                            _parentExists = (_parentTask.parentId != null);
                            _parentId = _parentTask.parentId;

                            if (_parentExists) _finalText = "Sub task, " + _parentTask.label + ". " + _finalText;
                            else _finalText = "Main task, " + _parentTask.label + ". " + _finalText;
                        }
                    }
                }

                return Json(new { sucess = true, text = _finalText }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { sucess = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        //private string GetRandomQuoteFromApi()
        //{
        //    const string WEBSERVICE_URL = "<<Web service URL>>";
        //    try
        //    {
        //        var webRequest = System.Net.WebRequest.Create(WEBSERVICE_URL);
        //        if (webRequest != null)
        //        {
        //            webRequest.Method = "GET";
        //            webRequest.Timeout = 12000;
        //            webRequest.ContentType = "application/json";
        //            webRequest.Headers.Add("Authorization", "Basic dchZ2VudDM6cGFdGVzC5zc3dvmQ=");

        //            using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
        //            {
        //                using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
        //                {
        //                    var jsonResponse = sr.ReadToEnd();
        //                    Console.WriteLine(String.Format("Response: {0}", jsonResponse));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //}
        #endregion

        #region New Task Manager for Ad Astra Assistant in Sql without points

        public ActionResult NewTasksViewer(string category, bool seeOnlyUndone = false)
        {
            try
            {
                if (!seeOnlyUndone)
                {
                    var result = (from a in db.TaskManagerTasks
                                  where (a.Category.ToLower().Equals(category.ToLower()))
                                  orderby a.CreateDate descending
                                  select a);
                    var tasks = result.ToList();
                    return View("NewTasksViewer", tasks);
                }
                else
                {
                    var result = (from a in db.TaskManagerTasks
                                  where (a.Category.ToLower().Equals(category.ToLower()) && !(a.Done.HasValue ? a.Done.Value : false))
                                  orderby a.CreateDate descending
                                  select a);
                    var tasks = result.ToList();
                    return View("NewTasksViewer", tasks);
                }

            }
            catch (Exception ex)
            {
                return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult New_GetUndoneTasks(string category)
        {
            try
            {
                var result = (from a in db.TaskManagerTasks
                              where (a.Category.ToLower().Equals(category.ToLower()) && !(a.Done.HasValue ? a.Done.Value : false))
                              select a);
                var tasks = result.ToList();
                return Json(tasks, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult New_GetUndoneTasksText(string category)
        {
            try
            {
                var result = (from a in db.TaskManagerTasks
                              where (a.Category.ToLower().Equals(category.ToLower()) && !(a.Done.HasValue ? a.Done.Value : false))
                              select a);
                var tasks = result.ToList();
                string _text = "As tarefas que tens para " + category + " são: ";
                int _counter = 1;
                foreach (var item in result)
                {
                    _text += _counter.ToString() + " - " + item.Task + ".";
                    _counter++;
                }
                return Content(_text);

            }
            catch (Exception ex)
            {
                return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }
        }

        // GET: JSON Add task to category
        [HttpGet]
        public ActionResult New_AddTaskToCategory(string task, string category)
        {
            try
            {
                db.TaskManagerTasks.Add(new TaskManagerTasks
                {
                    Task = task,
                    Category = category,
                    Done = false,
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
        public ActionResult New_TaskDone(string category, int id = -1, string text = "")
        {
            try
            {
                if (text != string.Empty)
                {
                    var result = (from a in db.TaskManagerTasks
                                  where (a.Task.ToLower().Contains(text.ToLower()) && !(a.Done.HasValue ? a.Done.Value : false) && a.Category.ToLower() == category.ToLower())
                                  select a);
                    List<TaskManagerTasks> tasks = result.ToList<TaskManagerTasks>();
                    if (tasks.Count() == 0)
                    {
                        return Json(new Models.JsonBotResponse
                        {
                            Success = true,
                            Message = "no_tasks",
                            Extra = text
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else if (tasks.Count() == 1)
                    {
                        tasks[0].Done = true;
                        db.SaveChanges();

                        //ask task category
                        return Json(new Models.JsonBotResponse
                        {
                            Success = true,
                            Message = "task_marked_has_done",
                            Extra = text
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new Models.JsonBotResponse
                        {
                            Success = true,
                            Message = "multiple_tasks_marked_has_done",
                            Extra = tasks.Count().ToString() + " with text: " + text
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (id != -1)
                {
                    var result = (from a in db.TaskManagerTasks
                                  where (a.Id.Equals(id))
                                  select a);
                    List<TaskManagerTasks> tasks = result.ToList<TaskManagerTasks>();
                    if (tasks.Count() == 0)
                    {
                        return Json(new Models.JsonBotResponse
                        {
                            Success = true,
                            Message = "no_tasks"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //db.TaskManagerTasks.Remove(result.First());
                        tasks[0].Done = true;
                        db.SaveChanges();

                        //ask task category
                        return Json(new Models.JsonBotResponse
                        {
                            Success = true,
                            Message = "task_marked_has_done"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    return Json(new Models.JsonBotResponse
                    {
                        Success = false
                    });
            }
            catch (Exception ex)
            {
                return Json(new Models.JsonBotResponse
                {
                    Success = false
                });
                //return Json("Unsucessful", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult New_TaskDelete(int id)
        {
            try
            {
                if (id != -1)
                {
                    var result = (from a in db.TaskManagerTasks
                                  where (a.Id.Equals(id))
                                  select a).FirstOrDefault();

                    if (result != null)
                    {
                        db.TaskManagerTasks.Remove(result);
                        db.SaveChanges();
                        return Json(new Models.JsonBotResponse
                        {
                            Success = true,
                            Message = "task_deleted"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new Models.JsonBotResponse
                        {
                            Success = false,
                            Message = "task_not_found"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    return Json(new Models.JsonBotResponse
                    {
                        Success = false
                    });
            }
            catch (Exception ex)
            {
                return Json(new Models.JsonBotResponse
                {
                    Success = false,
                    Message = "error",
                    Extra = ex.Message
                });
            }
        }

        #endregion
    }
}
