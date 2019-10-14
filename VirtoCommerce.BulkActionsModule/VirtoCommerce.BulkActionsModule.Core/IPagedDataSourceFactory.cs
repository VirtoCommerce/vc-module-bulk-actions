namespace VirtoCommerce.BulkActionsModule.Core
{
    using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;

    public interface IPagedDataSourceFactory
    {
        IPagedDataSource Create(BulkActionContext context);
    }
}