namespace VirtoCommerce.BulkActionsModule.Core
{
    using System;

    using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
    using VirtoCommerce.Platform.Core.Common;

    public interface IBulkActionExecutor
    {
        void Execute(
            BulkActionContext context,
            Action<BulkActionProgressContext> progressCallback,
            ICancellationToken token);
    }
}