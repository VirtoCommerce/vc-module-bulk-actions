using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.BulkActionsModule.Core.Services
{
    public interface IDataSource
    {
        IEnumerable<IEntity> Items { get; }

        bool Fetch();

        int GetTotalCount();
    }
}
