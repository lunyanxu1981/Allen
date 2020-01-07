using Microsoft.EntityFrameworkCore;

namespace AllenHome.AbpSolution.EntityFrameworkCore
{
    public static class DbContextOptionsConfigurer
    {
        public static void Configure(
            DbContextOptionsBuilder<AbpSolutionDbContext> dbContextOptions, 
            string connectionString
            )
        {
            /* This is the single point to configure DbContextOptions for AbpSolutionDbContext */
            dbContextOptions.UseSqlServer(connectionString);
        }
    }
}
