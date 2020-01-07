using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace AllenHome.AbpSolution.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpSolutionCoreModule), 
        typeof(AbpEntityFrameworkCoreModule))]
    public class AbpSolutionEntityFrameworkCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpSolutionEntityFrameworkCoreModule).GetAssembly());
        }
    }
}