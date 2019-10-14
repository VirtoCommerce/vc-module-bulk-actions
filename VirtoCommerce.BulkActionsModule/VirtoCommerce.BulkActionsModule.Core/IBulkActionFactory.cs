namespace VirtoCommerce.BulkActionsModule.Core
{
    using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;

    public interface IBulkActionFactory
    {
        IBulkAction Create(BulkActionContext context);
    }
}