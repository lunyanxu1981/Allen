using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AllenHome.AbpIOSolution.Data
{
    /* This is used if database provider does't define
     * IAbpIOSolutionDbSchemaMigrator implementation.
     */
    public class NullAbpIOSolutionDbSchemaMigrator : IAbpIOSolutionDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}