using System.Threading.Tasks;

namespace AllenHome.AbpIOSolution.Data
{
    public interface IAbpIOSolutionDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
