using AllenHome.AbpIOSolution.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace AllenHome.AbpIOSolution.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpIOSolutionEntityFrameworkCoreDbMigrationsModule),
        typeof(AbpIOSolutionApplicationContractsModule)
        )]
    public class AbpIOSolutionDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
