namespace VirtoCommerce.BulkActionsModule.Core.BulkActionAbstractions
{
    using System.Collections.Generic;

    using VirtoCommerce.BulkActionsModule.Core.BulkActionModels;
    using VirtoCommerce.Platform.Core.Common;

    public interface IBulkAction
    {
        BulkActionContext Context { get; }

        BulkActionResult Execute(IEnumerable<IEntity> entities);

        object GetActionData();

        BulkActionResult Validate();
    }
}