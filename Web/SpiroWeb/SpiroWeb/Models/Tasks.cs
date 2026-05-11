using System;

namespace SpiroWeb.Models
{
    [Serializable]
    public class Tasks
    {
        public string id { get; set; }
        public string parentId { get; set; }
        public string label { get; set; }
        public bool isDone { get; set; }

        public string attribute { get; set; }
        //TO REMOVE ?
        public string intelligence { get; set; }
        public int importance { get; set; }
        public int difficulty { get; set; }

        public bool isRepeatable { get; set; }
    }
}