using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AllenHome.AbpSolution.EntityFrameworkCore
{
    public class AbpSolutionDbContext : AbpDbContext
    {
        //Add DbSet properties for your entities...

        public AbpSolutionDbContext(DbContextOptions<AbpSolutionDbContext> options) 
            : base(options)
        {

        }
    }
}
