using System;
using System.Data;

namespace SpiroWeb.Models
{
    public class FlashModel
    {
        public string Texto { get; set; }
        public DataTable DtResult { get; set; }
        public String NoResult { get; set; }
        public String Error { get; set; }
    }

}