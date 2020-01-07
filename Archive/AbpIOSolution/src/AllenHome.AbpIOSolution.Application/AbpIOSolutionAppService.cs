using System;
using System.Collections.Generic;
using System.Text;
using AllenHome.AbpIOSolution.Localization;
using Volo.Abp.Application.Services;

namespace AllenHome.AbpIOSolution
{
    /* Inherit your application services from this class.
     */
    public abstract class AbpIOSolutionAppService : ApplicationService
    {
        protected AbpIOSolutionAppService()
        {
            LocalizationResource = typeof(AbpIOSolutionResource);
        }
    }
}
