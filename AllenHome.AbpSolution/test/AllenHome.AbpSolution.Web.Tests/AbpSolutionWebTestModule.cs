using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AllenHome.AbpSolution.Web.Startup;
namespace AllenHome.AbpSolution.Web.Tests
{
    [DependsOn(
        typeof(AbpSolutionWebModule),
        typeof(AbpAspNetCoreTestBaseModule)
        )]
    public class AbpSolutionWebTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpSolutionWebTestModule).GetAssembly());
        }
    }
}