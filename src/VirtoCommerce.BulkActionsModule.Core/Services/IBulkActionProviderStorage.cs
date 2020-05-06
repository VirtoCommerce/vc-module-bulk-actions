using System.Collections.Generic;

namespace VirtoCommerce.BulkActionsModule.Core.Services
{
    public interface IBulkActionProviderStorage
    {
        IBulkActionProvider Add(IBulkActionProvider provider);

        IBulkActionProvider Get(string name);

        IEnumerable<IBulkActionProvider> GetAll();
    }
}
