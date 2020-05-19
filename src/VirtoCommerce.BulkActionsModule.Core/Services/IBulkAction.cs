using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.BulkActionsModule.Core.Services
{
    public interface IBulkAction
    {
        BulkActionContext Context { get; }

        Task<BulkActionResult> ExecuteAsync(IEnumerable<IEntity> entities);

        Task<object> GetActionDataAsync();

        Task<BulkActionResult> ValidateAsync();
    }
}
