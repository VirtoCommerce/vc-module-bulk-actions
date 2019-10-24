namespace VirtoCommerce.BulkActionsModule.Core
{
    public interface IBulkActionProvider
    {
        string[] ApplicableTypes { get; set; }

        IBulkActionFactory BulkActionFactory { get; set; }

        string ContextTypeName { get; set; }

        IDataSourceFactory DataSourceFactory { get; set; }

        string Name { get; set; }

        string[] Permissions { get; set; }
    }
}