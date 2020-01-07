using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace AllenHome.AbpIOSolution.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpIOSolutionEntityFrameworkCoreModule)
        )]
    public class AbpIOSolutionEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<AbpIOSolutionMigrationsDbContext>();
        }
    }
}
