using System;
using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.BulkActionsModule.Core.Services
{
    public interface IBulkActionExecutor
    {
        void Execute(
            BulkActionContext context,
            Action<BulkActionProgressContext> progressAction,
            ICancellationToken token);
    }
}
