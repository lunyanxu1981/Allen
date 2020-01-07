using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace AllenHome.AbpSolution
{
    [DependsOn(
        typeof(AbpSolutionCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class AbpSolutionApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpSolutionApplicationModule).GetAssembly());
        }
    }
}