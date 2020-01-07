using Volo.Abp.Settings;

namespace AllenHome.AbpIOSolution.Settings
{
    public class AbpIOSolutionSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(AbpIOSolutionSettings.MySetting1));
        }
    }
}
