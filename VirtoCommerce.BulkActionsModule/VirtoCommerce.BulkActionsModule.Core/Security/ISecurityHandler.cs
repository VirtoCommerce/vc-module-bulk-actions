namespace VirtoCommerce.BulkActionsModule.Core.Security
{
    public interface ISecurityHandler
    {
        bool Authorize(string userName);
    }
}