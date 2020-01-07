using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Components;
using Volo.Abp.DependencyInjection;

namespace AllenHome.AbpIOSolution.Web
{
    [Dependency(ReplaceServices = true)]
    public class AbpIOSolutionBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "AbpIOSolution";
    }
}
