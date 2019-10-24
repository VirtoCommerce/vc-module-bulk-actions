namespace VirtoCommerce.BulkActionsModule.Data.Security
{
    using VirtoCommerce.BulkActionsModule.Core.Security;

    public interface ISecurityHandlerFactory
    {
        ISecurityHandler Create(params string[] permissions);
    }
}