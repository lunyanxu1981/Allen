using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using AllenHome.AbpIOSolution.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace AllenHome.AbpIOSolution.Web.Pages
{
    /* Inherit your UI Pages from this class. To do that, add this line to your Pages (.cshtml files under the Page folder):
     * @inherits AllenHome.AbpIOSolution.Web.Pages.AbpIOSolutionPage
     */
    public abstract class AbpIOSolutionPage : AbpPage
    {
        [RazorInject]
        public IHtmlLocalizer<AbpIOSolutionResource> L { get; set; }
    }
}
