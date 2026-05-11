using System;

namespace SpiroWeb.Models
{
    public class HistoryPoints
    {
        public int id { get; set; }
        public int taskId { get; set; }
        public string category { get; set; }
        public string attribute { get; set; }
        public DateTime date { get; set; }
        public int points { get; set; }
        public int newAttributePoints { get; set; }
        public int newCategoryPoints { get; set; }
        public int newGlobalPoints { get; set; }
        public int newGlobalLevel { get; set; }
    }
}