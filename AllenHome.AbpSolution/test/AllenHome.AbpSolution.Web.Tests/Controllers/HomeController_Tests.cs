using System.Threading.Tasks;
using AllenHome.AbpSolution.Web.Controllers;
using Shouldly;
using Xunit;

namespace AllenHome.AbpSolution.Web.Tests.Controllers
{
    public class HomeController_Tests: AbpSolutionWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(2,2,4)]
        [InlineData(3,7,10)]
        public void Add(int x, int y, int expected)
        {
            int z = x + y;
            Assert.True(z == expected);
        }
    }
}
