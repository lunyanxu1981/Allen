using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AllenHome.AbpIOSolution.Data;
using Volo.Abp.DependencyInjection;

namespace AllenHome.AbpIOSolution.EntityFrameworkCore
{
    [Dependency(ReplaceServices = true)]
    public class EntityFrameworkCoreAbpIOSolutionDbSchemaMigrator 
        : IAbpIOSolutionDbSchemaMigrator, ITransientDependency
    {
        private readonly AbpIOSolutionMigrationsDbContext _dbContext;

        public EntityFrameworkCoreAbpIOSolutionDbSchemaMigrator(AbpIOSolutionMigrationsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task MigrateAsync()
        {
            await _dbContext.Database.MigrateAsync();
        }
    }
}