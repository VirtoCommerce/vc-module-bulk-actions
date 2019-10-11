namespace VirtoCommerce.BulkActionsModule.Core.BulkActionAbstractions
{
    using System;

    using VirtoCommerce.BulkActionsModule.Core.BulkActionModels;
    using VirtoCommerce.Platform.Core.Common;

    public interface IBulkActionExecutor
    {
        void Execute(
            BulkActionContext context,
            Action<BulkActionProgressContext> progressCallback,
            ICancellationToken token);
    }
}