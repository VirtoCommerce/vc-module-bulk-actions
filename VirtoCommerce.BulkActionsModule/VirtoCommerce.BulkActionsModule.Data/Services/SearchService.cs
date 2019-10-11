namespace VirtoCommerce.BulkActionsModule.Data.Services
{
    using System;
    using System.Linq;

    using VirtoCommerce.BulkActionsModule.Core.Converters;
    using VirtoCommerce.BulkActionsModule.Core.Models;
    using VirtoCommerce.Domain.Catalog.Model;
    using VirtoCommerce.Domain.Catalog.Services;
    using VirtoCommerce.Platform.Core.Assets;
    using VirtoCommerce.Platform.Core.Common;

    using SearchCriteria = VirtoCommerce.Domain.Catalog.Model.SearchCriteria;
    using SearchResult = VirtoCommerce.BulkActionsModule.Core.Models.SearchResult;

    public class SearchService : ISearchService
    {
        private readonly IBlobUrlResolver _blobUrlResolver;

        private readonly ICatalogSearchService _searchService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchService"/> class.
        /// </summary>
        /// <param name="searchService">
        /// The search service.
        /// </param>
        /// <param name="blobUrlResolver">
        /// The blob url resolver.
        /// </param>
        public SearchService(ICatalogSearchService searchService, IBlobUrlResolver blobUrlResolver)
        {
            _searchService = searchService;
            _blobUrlResolver = blobUrlResolver;
        }

        public SearchResult Search(SearchCriteria criteria)
        {
            var result = new SearchResult();
            var categorySkip = 0;
            var categoryTake = 0;

            // because products and categories represent in search result as two separated collections for handle paging request 
            // we should join two resulting collection artificially

            // search categories
            var copyRespGroup = criteria.ResponseGroup;
            if ((criteria.ResponseGroup & SearchResponseGroup.WithCategories) == SearchResponseGroup.WithCategories)
            {
                criteria.ResponseGroup &= ~SearchResponseGroup.WithProducts;
                var categoriesSearchResult = _searchService.Search(criteria);
                var categoriesTotalCount = categoriesSearchResult.Categories.Count;

                categorySkip = Math.Min(categoriesTotalCount, criteria.Skip);
                categoryTake = Math.Min(criteria.Take, Math.Max(0, categoriesTotalCount - criteria.Skip));
                var categories = categoriesSearchResult.Categories.Skip(categorySkip).Take(categoryTake)
                    .Select(category => new ListEntryCategory(category.ToWebModel(_blobUrlResolver))).ToList();

                result.TotalCount = categoriesTotalCount;
                result.Entries.AddRange(categories);
            }

            criteria.ResponseGroup = copyRespGroup;

            // search products
            if ((criteria.ResponseGroup & SearchResponseGroup.WithProducts) == SearchResponseGroup.WithProducts)
            {
                criteria.ResponseGroup &= ~SearchResponseGroup.WithCategories;
                criteria.Skip -= categorySkip;
                criteria.Take -= categoryTake;
                var productsSearchResult = _searchService.Search(criteria);

                var products = productsSearchResult.Products.Select(
                    product => new ListEntryProduct(product.ToWebModel(_blobUrlResolver)));

                result.TotalCount += productsSearchResult.ProductsTotalCount;
                result.Entries.AddRange(products);
            }

            return result;
        }
    }
}