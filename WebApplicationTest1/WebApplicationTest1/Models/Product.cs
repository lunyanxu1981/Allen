using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplicationTest1.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }

        [AllowHtml]
        public string PayerAuthHTML { get; set; }

        [AllowHtml]
        public string PayerAuthHTMLEncrypted { get; set; }
    }
}