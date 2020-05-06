using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.BulkActionsModule.Data.Authorization;
using VirtoCommerce.Platform.Security.Authorization;

namespace VirtoCommerce.BulkActionsModule.Web.Authorization
{
    public class BulkActionsAuthorizationHandler : PermissionAuthorizationHandlerBase<BulkActionsAuthorizationRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BulkActionsAuthorizationRequirement requirement)
        {
            await base.HandleRequirementAsync(context, requirement);

            if (!context.HasSucceeded)
            {

            }
        }
    }
}
