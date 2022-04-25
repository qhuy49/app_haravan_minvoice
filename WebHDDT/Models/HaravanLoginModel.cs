using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHDDT.Models
{
    public class HaravanLoginModel
    {
        public string OriginId { get; set; }
        public string id_token { get; set; }
        public string access_token { get; set; }

        public string name { get; set; }
        public string email { get; set; }
    }
}