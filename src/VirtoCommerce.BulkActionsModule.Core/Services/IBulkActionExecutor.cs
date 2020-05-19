using System;
using System.Threading.Tasks;
using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.BulkActionsModule.Core.Services
{
    public interface IBulkActionExecutor
    {
        Task ExecuteAsync(BulkActionContext context,
            Action<BulkActionProgressContext> progressAction,
            ICancellationToken token);
    }
}
