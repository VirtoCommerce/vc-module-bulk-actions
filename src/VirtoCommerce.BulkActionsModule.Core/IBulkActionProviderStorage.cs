namespace VirtoCommerce.BulkActionsModule.Core
{
    using System.Collections.Generic;

    public interface IBulkActionProviderStorage
    {
        IBulkActionProvider Add(IBulkActionProvider provider);

        IBulkActionProvider Get(string name);

        IEnumerable<IBulkActionProvider> GetAll();
    }
}