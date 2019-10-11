namespace VirtoCommerce.BulkActionsModule.Core.Converters
{
    using System.Collections.Generic;
    using System.Linq;

    using Omu.ValueInjecter;

    using VirtoCommerce.Platform.Core.Assets;
    using VirtoCommerce.Platform.Core.Common;

    using Category = VirtoCommerce.BulkActionsModule.Core.Models.Category;
    using Property = VirtoCommerce.BulkActionsModule.Core.Models.Property;
    using PropertyValue = VirtoCommerce.BulkActionsModule.Core.Models.PropertyValue;
    using VC = VirtoCommerce.Domain.Catalog.Model;

    public static class CategoryConverter
    {
        public static VC.Category ToModuleModel(this Category category)
        {
            var result = category.ToModel(AbstractTypeFactory<VC.Category>.TryCreateInstance());

            result.InjectFrom(category);
            result.SeoInfos = category.SeoInfos;

            if (category.Links != null)
            {
                result.Links = category.Links.Select(link => link.ToCoreModel()).ToList();
            }

            if (category.Properties != null)
            {
                result.PropertyValues = new List<VC.PropertyValue>();
                foreach (var property in category.Properties)
                {
                    foreach (var propValue in property.Values ?? Enumerable.Empty<PropertyValue>())
                    {
                        propValue.ValueType = property.ValueType;

                        // need populate required fields
                        propValue.PropertyId = property.Id;
                        propValue.PropertyName = property.Name;
                        result.PropertyValues.Add(propValue.ToCoreModel());
                    }
                }
            }

            if (category.Images != null)
            {
                result.Images = category.Images.Select(x => x.ToCoreModel()).ToList();
            }

            return result;
        }

        public static Category ToWebModel(
            this VC.Category category,
            IBlobUrlResolver blobUrlResolver = null,
            bool convertProps = true)
        {
            var result = AbstractTypeFactory<Category>.TryCreateInstance().FromModel(category);

            // do not use omu.InjectFrom for performance reasons
            result.Id = category.Id;
            result.IsActive = category.IsActive;
            result.IsVirtual = category.IsVirtual;
            result.Name = category.Name;
            result.ParentId = category.ParentId;
            result.Path = category.Path;
            result.TaxType = category.TaxType;
            result.CatalogId = category.CatalogId;
            result.Code = category.Code;
            result.CreatedBy = category.CreatedBy;
            result.CreatedDate = category.CreatedDate;
            result.ModifiedBy = category.ModifiedBy;
            result.ModifiedDate = category.ModifiedDate;
            result.Priority = category.Priority;
            result.SeoInfos = category.SeoInfos;

            if (!category.Outlines.IsNullOrEmpty())
            {
                // minimize outline size
                result.Outlines = category.Outlines.Select(x => x.ToWebModel()).ToList();
            }

            // init outline and path
            if (category.Parents != null)
            {
                result.Outline = string.Join("/", category.Parents.Select(parent => parent.Id));
                result.Path = string.Join("/", category.Parents.Select(parent => parent.Name));
            }

            // for virtual category links not needed
            if (!category.IsVirtual && category.Links != null)
            {
                result.Links = category.Links.Select(link => link.ToWebModel()).ToList();
            }

            // need add property for each meta info
            result.Properties = new List<Property>();
            if (convertProps)
            {
                if (!category.Properties.IsNullOrEmpty())
                {
                    foreach (var property in category.Properties)
                    {
                        var webModelProperty = property.ToWebModel();
                        webModelProperty.Values = new List<PropertyValue>();
                        webModelProperty.IsManageable = true;
                        webModelProperty.IsReadOnly = property.Type != VC.PropertyType.Category;
                        result.Properties.Add(webModelProperty);
                    }
                }

                // populate property values
                if (category.PropertyValues != null)
                {
                    var sort = false;
                    foreach (var propertyValue in category.PropertyValues.Select(value => value.ToWebModel()))
                    {
                        var property = result.Properties.FirstOrDefault(p => p.Id == propertyValue.PropertyId);
                        if (property == null)
                        {
                            property = result.Properties.FirstOrDefault(
                                p => p.Name.EqualsInvariant(propertyValue.PropertyName));
                        }

                        if (property == null)
                        {
                            // need add dummy property for each value without property
                            property = new Property(propertyValue, category.CatalogId, VC.PropertyType.Category);
                            result.Properties.Add(property);
                            sort = true;
                        }

                        property.Values.Add(propertyValue);
                    }

                    if (sort)
                    {
                        result.Properties = result.Properties.OrderBy(property => property.Name).ToList();
                    }
                }
            }

            if (category.Images != null)
            {
                result.Images = category.Images.Select(image => image.ToWebModel(blobUrlResolver)).ToList();
            }

            return result;
        }
    }
}