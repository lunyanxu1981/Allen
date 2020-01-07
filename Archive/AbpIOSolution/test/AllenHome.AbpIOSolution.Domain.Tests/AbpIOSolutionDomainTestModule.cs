using AllenHome.AbpIOSolution.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace AllenHome.AbpIOSolution
{
    [DependsOn(
        typeof(AbpIOSolutionEntityFrameworkCoreTestModule)
        )]
    public class AbpIOSolutionDomainTestModule : AbpModule
    {

    }
}