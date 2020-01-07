using AllenHome.AbpSolution.EntityFrameworkCore;

namespace AllenHome.AbpSolution.Tests.TestDatas
{
    public class TestDataBuilder
    {
        private readonly AbpSolutionDbContext _context;

        public TestDataBuilder(AbpSolutionDbContext context)
        {
            _context = context;
        }

        public void Build()
        {
            //create test data here...
        }
    }
}