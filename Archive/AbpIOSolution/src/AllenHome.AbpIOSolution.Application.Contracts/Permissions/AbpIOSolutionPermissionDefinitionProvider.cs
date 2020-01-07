using AllenHome.AbpIOSolution.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace AllenHome.AbpIOSolution.Permissions
{
    public class AbpIOSolutionPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(AbpIOSolutionPermissions.GroupName);

            //Define your own permissions here. Example:
            //myGroup.AddPermission(AbpIOSolutionPermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<AbpIOSolutionResource>(name);
        }
    }
}
