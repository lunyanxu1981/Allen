using Abp.Modules;
using Abp.Reflection.Extensions;
using AllenHome.AbpSolution.Localization;

namespace AllenHome.AbpSolution
{
    public class AbpSolutionCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            AbpSolutionLocalizationConfigurer.Configure(Configuration.Localization);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpSolutionCoreModule).GetAssembly());
        }
    }
}