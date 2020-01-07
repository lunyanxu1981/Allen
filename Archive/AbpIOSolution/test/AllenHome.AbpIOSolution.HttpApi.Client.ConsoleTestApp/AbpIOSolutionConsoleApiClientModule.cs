using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace AllenHome.AbpIOSolution.HttpApi.Client.ConsoleTestApp
{
    [DependsOn(
        typeof(AbpIOSolutionHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class AbpIOSolutionConsoleApiClientModule : AbpModule
    {
        
    }
}
