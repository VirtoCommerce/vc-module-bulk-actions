namespace VirtoCommerce.BulkActionsModule.Data.Services
{
    using System.Collections.Generic;

    using VirtoCommerce.BulkActionsModule.Core.Models;
    using VirtoCommerce.Platform.Core.Common;

    public interface IMover<T>
        where T : class, IEntity
    {
        void Confirm(IEnumerable<T> entities);

        List<T> Prepare(MoveOperationContext moveOperationContext);
    }
}