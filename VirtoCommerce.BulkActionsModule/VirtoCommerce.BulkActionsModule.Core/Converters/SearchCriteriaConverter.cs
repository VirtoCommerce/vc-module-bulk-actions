namespace VirtoCommerce.BulkActionsModule.Core.Converters
{
    using System.Linq;

    using Omu.ValueInjecter;

    using VirtoCommerce.Platform.Core.Common;

    using SearchCriteria = VirtoCommerce.BulkActionsModule.Core.Models.SearchCriteria;
    using VC = VirtoCommerce.Domain.Catalog.Model;

    public static class SearchCriteriaConverter
    {
        public static VC.SearchCriteria ToCoreModel(this SearchCriteria criteria)
        {
            var result = new VC.SearchCriteria();

            result.InjectFrom(criteria);

            result.ResponseGroup = criteria.ResponseGroup;
            result.CategoryIds = criteria.CategoryIds;
            result.CatalogIds = criteria.CatalogIds;
            result.PricelistIds = criteria.PricelistIds;
            result.Terms = criteria.Terms;
            result.Facets = criteria.Facets;
            result.ProductTypes = criteria.ProductTypes;
            result.VendorIds = criteria.VendorIds;

            if (!criteria.PropertyValues.IsNullOrEmpty())
            {
                result.PropertyValues = criteria.PropertyValues.Select(propertyValue => propertyValue.ToCoreModel())
                    .ToArray();
            }

            return result;
        }
    }
}