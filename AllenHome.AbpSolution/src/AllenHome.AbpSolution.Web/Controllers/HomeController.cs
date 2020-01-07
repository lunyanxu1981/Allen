using Microsoft.AspNetCore.Mvc;

namespace AllenHome.AbpSolution.Web.Controllers
{
    public class HomeController : AbpSolutionControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}