namespace VirtoCommerce.BulkActionsModule.Core.BulkActionAbstractions
{
    using VirtoCommerce.BulkActionsModule.Core.DataSourceAbstractions;

    public interface IBulkActionProvider
    {
        string[] ApplicableTypes { get; set; }

        IBulkActionFactory BulkActionFactory { get; set; }

        string ContextTypeName { get; set; }

        IPagedDataSourceFactory DataSourceFactory { get; set; }

        string Name { get; set; }
    }
}