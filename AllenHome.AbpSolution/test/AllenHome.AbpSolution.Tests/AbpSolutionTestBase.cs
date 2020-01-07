using System;
using System.Threading.Tasks;
using Abp.TestBase;
using AllenHome.AbpSolution.EntityFrameworkCore;
using AllenHome.AbpSolution.Tests.TestDatas;

namespace AllenHome.AbpSolution.Tests
{
    public class AbpSolutionTestBase : AbpIntegratedTestBase<AbpSolutionTestModule>
    {
        public AbpSolutionTestBase()
        {
            UsingDbContext(context => new TestDataBuilder(context).Build());
        }

        protected virtual void UsingDbContext(Action<AbpSolutionDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<AbpSolutionDbContext>())
            {
                action(context);
                context.SaveChanges();
            }
        }

        protected virtual T UsingDbContext<T>(Func<AbpSolutionDbContext, T> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<AbpSolutionDbContext>())
            {
                result = func(context);
                context.SaveChanges();
            }

            return result;
        }

        protected virtual async Task UsingDbContextAsync(Func<AbpSolutionDbContext, Task> action)
        {
            using (var context = LocalIocManager.Resolve<AbpSolutionDbContext>())
            {
                await action(context);
                await context.SaveChangesAsync(true);
            }
        }

        protected virtual async Task<T> UsingDbContextAsync<T>(Func<AbpSolutionDbContext, Task<T>> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<AbpSolutionDbContext>())
            {
                result = await func(context);
                context.SaveChanges();
            }

            return result;
        }
    }
}
