using Abp.Application.Services;

namespace AllenHome.AbpSolution
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class AbpSolutionAppServiceBase : ApplicationService
    {
        protected AbpSolutionAppServiceBase()
        {
            LocalizationSourceName = AbpSolutionConsts.LocalizationSourceName;
        }
    }
}