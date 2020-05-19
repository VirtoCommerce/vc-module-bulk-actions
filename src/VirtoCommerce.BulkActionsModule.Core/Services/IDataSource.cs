using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.BulkActionsModule.Core.Services
{
    public interface IDataSource
    {
        IEnumerable<IEntity> Items { get; }

        Task<bool> FetchAsync();

        Task<int> GetTotalCountAsync();
    }
}
