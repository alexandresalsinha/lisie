using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class WeekTaskEvents
    {
        public List<WeekTask> events { get; set; }
    }

    public class WeekTask
    {
        public int id { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string title { get; set; }
        public string category { get; set; }
        public int taskId { get; set; }

        public bool isDone { get; set; }
    }
}