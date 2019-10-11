namespace VirtoCommerce.BulkActionsModule.Core.BulkActionAbstractions
{
    using System.Collections.Generic;

    using VirtoCommerce.BulkActionsModule.Core.BulkActionModels;

    public interface IBulkActionRegistrar
    {
        IEnumerable<BulkActionDefinition> GetAll();

        BulkActionDefinition GetByName(string name);

        BulkActionDefinition Register(BulkActionDefinition definition);
    }
}