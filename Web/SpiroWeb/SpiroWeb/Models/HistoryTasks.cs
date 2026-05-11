using System;
using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class HistoryTask
    {
        public int id { get; set; }
        public int taskId { get; set; }
        public string label { get; set; }
        public string category { get; set; }
        public string attribute { get; set; }
        public DateTime date { get; set; }
        public bool isDone { get; set; }
        public int points { get; set; }
        public List<HistoryTaskLevels> levels { get; set; }
    }

    public class HistoryTaskLevels
    {
        public int level { get; set; }
        public string label { get; set; }
    }
}