namespace VirtoCommerce.BulkActionsModule.Core
{
    using System.Collections.Generic;

    using VirtoCommerce.Platform.Core.Common;

    public interface IDataSource
    {
        IEnumerable<IEntity> Items { get; }

        bool Fetch();

        int GetTotalCount();
    }
}