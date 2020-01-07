using AllenHome.AbpIOSolution.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace AllenHome.AbpIOSolution.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class AbpIOSolutionController : AbpController
    {
        protected AbpIOSolutionController()
        {
            LocalizationResource = typeof(AbpIOSolutionResource);
        }
    }
}