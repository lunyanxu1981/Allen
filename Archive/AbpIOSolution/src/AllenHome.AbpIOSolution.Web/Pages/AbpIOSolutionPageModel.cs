using AllenHome.AbpIOSolution.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace AllenHome.AbpIOSolution.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class AbpIOSolutionPageModel : AbpPageModel
    {
        protected AbpIOSolutionPageModel()
        {
            LocalizationResourceType = typeof(AbpIOSolutionResource);
        }
    }
}