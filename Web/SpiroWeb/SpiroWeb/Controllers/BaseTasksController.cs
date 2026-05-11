using System.Collections.Generic;
using System.Web.Mvc;


namespace SpiroWeb.Controllers
{
    public class BaseTasksController : Controller
    {
        public List<Models.Tasks> GetTasksLists(string category)
        {
            List<Models.Tasks> _listForType = new List<Models.Tasks>();
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
    }
}
