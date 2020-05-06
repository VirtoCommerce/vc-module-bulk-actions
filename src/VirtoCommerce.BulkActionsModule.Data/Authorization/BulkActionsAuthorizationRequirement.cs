using VirtoCommerce.Platform.Security.Authorization;

namespace VirtoCommerce.BulkActionsModule.Data.Authorization
{
    public class BulkActionsAuthorizationRequirement : PermissionAuthorizationRequirement
    {
        public BulkActionsAuthorizationRequirement(string permission) : base(permission)
        {
        }
    }
}
