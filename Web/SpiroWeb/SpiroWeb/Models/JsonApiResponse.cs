namespace SpiroWeb.Models
{
    //[Serializable]
    public class JsonApiResponse
    {
        public bool Success { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
    }
}