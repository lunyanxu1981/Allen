using Volo.Abp.Modularity;

namespace AllenHome.AbpIOSolution
{
    [DependsOn(
        typeof(AbpIOSolutionApplicationModule),
        typeof(AbpIOSolutionDomainTestModule)
        )]
    public class AbpIOSolutionApplicationTestModule : AbpModule
    {

    }
}