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

        public ProductItem Item { get; set; }

        public bool IsValidItem
        {
            get
            {
                return Item == null? false : Item.IsValid;
            }
        }

        [AllowHtml]
        public string PayerAuthHTML { get; set; }

        [AllowHtml]
        public string PayerAuthHTMLEncrypted { get; set; }
    }

    public class ProductItem
    {
        public string Name { get; set; }
        public bool IsValid { get; set; }
    }
}