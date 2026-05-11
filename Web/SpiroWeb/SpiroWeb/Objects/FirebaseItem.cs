namespace SpiroWeb.Objects
{

    public class FirebaseData
    {
        public string Id { get; set; }
        public FirebaseItem FirebaseItem2 { get; set; }
    }

    public class FirebaseItem
    {
        public string barCode { get; set; }
        public string date { get; set; }
        public string isRegistered { get; set; }
    }


    public class Data
    {
        public string Id { get; set; }
        public Values Values { get; set; }
    }

    public class Values
    {
        public string barCode { get; set; }
        public string date { get; set; }
        public string isRegistered { get; set; }
    }
}
