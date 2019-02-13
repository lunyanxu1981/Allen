using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationTest1.Models
{
    public class VirtualCard
    {
        public string CardNo { get; set; }
        public string Password { get; set; }
        public string Ymd { get; set; }
        public string RequesString { get; set; }
        public string ResponseString { get; set; }
    }
}