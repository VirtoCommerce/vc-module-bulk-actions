using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;

namespace VirtoCommerce.BulkActionsModule.Core.Services
{
    public interface IBulkActionFactory
    {
        IBulkAction Create(BulkActionContext context);
    }
}
