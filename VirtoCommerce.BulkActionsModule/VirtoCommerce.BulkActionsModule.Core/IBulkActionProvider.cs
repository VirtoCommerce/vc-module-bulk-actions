namespace VirtoCommerce.BulkActionsModule.Core
{
    public interface IBulkActionProvider
    {
        string[] ApplicableTypes { get; set; }

        IBulkActionFactory BulkActionFactory { get; set; }

        string ContextTypeName { get; set; }

        IPagedDataSourceFactory DataSourceFactory { get; set; }

        string Name { get; set; }
    }
}