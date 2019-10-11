namespace VirtoCommerce.BulkActionsModule.Core.DataSourceAbstractions
{
    using VirtoCommerce.BulkActionsModule.Core.BulkActionModels;

    public interface IPagedDataSourceFactory
    {
        IPagedDataSource Create(BulkActionContext context);
    }
}