namespace VirtoCommerce.BulkActionsModule.Data.Services
{
    using VirtoCommerce.BulkActionsModule.Core.Models;

    public interface ISearchService
    {
        SearchResult Search(VirtoCommerce.Domain.Catalog.Model.SearchCriteria criteria);
    }
}