namespace VirtoCommerce.BulkActionsModule.Core
{
    using System.Collections.Generic;

    using VirtoCommerce.Platform.Core.Common;

    public interface IPagedDataSource
    {
        IEnumerable<IEntity> Items { get; }

        bool Fetch();

        int GetTotalCount();
    }
}