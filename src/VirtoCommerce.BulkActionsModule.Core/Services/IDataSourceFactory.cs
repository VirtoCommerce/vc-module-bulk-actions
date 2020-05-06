using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;

namespace VirtoCommerce.BulkActionsModule.Core.Services
{
    public interface IDataSourceFactory
    {
        IDataSource Create(BulkActionContext context);
    }
}
