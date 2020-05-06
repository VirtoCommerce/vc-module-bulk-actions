using System.Collections.Generic;
using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.BulkActionsModule.Core.Services
{
    public interface IBulkAction
    {
        BulkActionContext Context { get; }

        BulkActionResult Execute(IEnumerable<IEntity> entities);

        object GetActionData();

        BulkActionResult Validate();
    }
}
