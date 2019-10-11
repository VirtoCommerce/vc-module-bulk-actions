namespace VirtoCommerce.BulkActionsModule.Core.BulkActionAbstractions
{
    using VirtoCommerce.BulkActionsModule.Core.BulkActionModels;

    public interface IBulkActionFactory
    {
        IBulkAction Create(BulkActionContext context);
    }
}