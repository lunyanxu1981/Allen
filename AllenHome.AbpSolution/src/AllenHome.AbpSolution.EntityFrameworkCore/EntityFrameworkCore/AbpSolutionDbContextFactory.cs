using AllenHome.AbpSolution.Configuration;
using AllenHome.AbpSolution.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AllenHome.AbpSolution.EntityFrameworkCore
{
    /* This class is needed to run EF Core PMC commands. Not used anywhere else */
    public class AbpSolutionDbContextFactory : IDesignTimeDbContextFactory<AbpSolutionDbContext>
    {
        public AbpSolutionDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AbpSolutionDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            DbContextOptionsConfigurer.Configure(
                builder,
                configuration.GetConnectionString(AbpSolutionConsts.ConnectionStringName)
            );

            return new AbpSolutionDbContext(builder.Options);
        }
    }
}