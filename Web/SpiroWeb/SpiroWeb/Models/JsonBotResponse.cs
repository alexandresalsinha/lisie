using System;

namespace SpiroWeb.Models
{
    [Serializable]
    public class JsonBotResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Extra { get; set; }
    }
}