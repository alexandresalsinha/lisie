using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoresPuppetter
{
    public class AddToJumboOnlineFinishedModel
    {
        public int StoreId = 1;
        public string StoreName = "Jumbo";
        public string UserId { get; set; }
        public int TimespanSeconds { get; set; }
        public List<StoresPuppetter.Product> ProductsNotAdded { get; set; }
        public List<StoresPuppetter.Product> ProductsAdded { get; set; }
        public bool Sucess { get; set; }
        public bool AuthenticationSucess { get; set; }

        public int TotalProducts { get; set; }
    }
}
