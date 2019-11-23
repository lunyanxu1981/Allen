using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unity;
using WebApplicationTest1.Models;

namespace WebApplicationTest1.Controllers
{
    public class ProductController : Controller
    {
        
        private readonly IProductRepository repository;


        //Property injection applies for modifier internal and public
        [Dependency("product")]
        internal IProductRepository Repository2 { get; set; }

        //Constructor injection
        [InjectionConstructor()]
        public ProductController([Dependency("product")] IProductRepository repository)
        {
            this.repository = repository;
        }

        /*
         * Method injection
        [InjectionMethod]
        public void Initializer([Dependency("product")]IProductRepository repository)
        {
            this.Repository2 = repository;
        }
        */
        [HttpGet]
        public ActionResult Index()
        {
            return View(this.Repository2.GetAll());
        }
    }
}