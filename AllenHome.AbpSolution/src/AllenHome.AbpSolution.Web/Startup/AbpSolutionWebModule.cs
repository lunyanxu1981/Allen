using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AllenHome.AbpSolution.Configuration;
using AllenHome.AbpSolution.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace AllenHome.AbpSolution.Web.Startup
{
    [DependsOn(
        typeof(AbpSolutionApplicationModule), 
        typeof(AbpSolutionEntityFrameworkCoreModule), 
        typeof(AbpAspNetCoreModule))]
    public class AbpSolutionWebModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public AbpSolutionWebModule(IHostingEnvironment env)
        {
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName);
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(AbpSolutionConsts.ConnectionStringName);

            Configuration.Navigation.Providers.Add<AbpSolutionNavigationProvider>();

            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(AbpSolutionApplicationModule).GetAssembly()
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpSolutionWebModule).GetAssembly());
        }
    }
}