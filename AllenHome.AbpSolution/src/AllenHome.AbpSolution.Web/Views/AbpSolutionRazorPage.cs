using Abp.AspNetCore.Mvc.Views;

namespace AllenHome.AbpSolution.Web.Views
{
    public abstract class AbpSolutionRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected AbpSolutionRazorPage()
        {
            LocalizationSourceName = AbpSolutionConsts.LocalizationSourceName;
        }
    }
}
