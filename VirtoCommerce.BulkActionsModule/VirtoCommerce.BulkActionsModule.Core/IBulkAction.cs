namespace VirtoCommerce.BulkActionsModule.Core
{
    using System.Collections.Generic;

    using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
    using VirtoCommerce.Platform.Core.Common;

    public interface IBulkAction
    {
        BulkActionContext Context { get; }

        BulkActionResult Execute(IEnumerable<IEntity> entities);

        object GetActionData();

        BulkActionResult Validate();
    }
}