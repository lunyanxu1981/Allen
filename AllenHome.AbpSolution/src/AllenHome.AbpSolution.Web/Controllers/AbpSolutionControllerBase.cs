using Abp.AspNetCore.Mvc.Controllers;

namespace AllenHome.AbpSolution.Web.Controllers
{
    public abstract class AbpSolutionControllerBase: AbpController
    {
        protected AbpSolutionControllerBase()
        {
            LocalizationSourceName = AbpSolutionConsts.LocalizationSourceName;
        }
    }
}