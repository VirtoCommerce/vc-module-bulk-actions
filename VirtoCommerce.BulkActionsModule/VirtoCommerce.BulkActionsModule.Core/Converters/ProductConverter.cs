namespace VirtoCommerce.BulkActionsModule.Core.Converters
{
    using System.Collections.Generic;
    using System.Linq;

    using Omu.ValueInjecter;

    using VirtoCommerce.BulkActionsModule.Core.Models;
    using VirtoCommerce.Platform.Core.Assets;
    using VirtoCommerce.Platform.Core.Common;

    using Property = VirtoCommerce.BulkActionsModule.Core.Models.Property;
    using PropertyValue = VirtoCommerce.BulkActionsModule.Core.Models.PropertyValue;
    using VC = VirtoCommerce.Domain.Catalog.Model;

    public static class ProductConverter
    {
        public static VC.CatalogProduct ToModuleModel(this Product product, IBlobUrlResolver blobUrlResolver)
        {
            var result = product.ToModel(AbstractTypeFactory<VC.CatalogProduct>.TryCreateInstance());
            result.InjectFrom(product);
            result.SeoInfos = product.SeoInfos;

            if (product.Images != null)
            {
                result.Images = product.Images.Select(image => image.ToCoreModel()).ToList();
            }

            if (product.Assets != null)
            {
                result.Assets = product.Assets.Select(asset => asset.ToCoreModel()).ToList();
            }

            if (product.Properties != null)
            {
                result.PropertyValues = new List<VC.PropertyValue>();
                foreach (var property in product.Properties)
                {
                    if (property.Values != null)
                    {
                        foreach (var propValue in property.Values)
                        {
                            // need populate required fields
                            propValue.PropertyName = property.Name;
                            propValue.ValueType = property.ValueType;
                            result.PropertyValues.Add(propValue.ToCoreModel());
                        }
                    }
                }
            }

            if (product.Variations != null)
            {
                result.Variations = product.Variations.Select(x => x.ToModuleModel(blobUrlResolver)).ToList();
            }

            if (product.Links != null)
            {
                result.Links = product.Links.Select(link => link.ToCoreModel()).ToList();
            }

            if (product.Reviews != null)
            {
                result.Reviews = product.Reviews.Select(review => review.ToCoreModel()).ToList();
            }

            if (product.Associations != null)
            {
                result.Associations = product.Associations.Select(association => association.ToCoreModel()).ToList();
                var index = 0;
                foreach (var association in result.Associations)
                {
                    association.Priority = index++;
                }
            }

            result.MainProductId = product.TitularItemId;
            return result;
        }

        public static Product ToWebModel(this VC.CatalogProduct product, IBlobUrlResolver blobUrlResolver)
        {
            var result = AbstractTypeFactory<Product>.TryCreateInstance().FromModel(product);
            result.Id = product.Id;
            result.CatalogId = product.CatalogId;
            result.CategoryId = product.CategoryId;
            result.Code = product.Code;
            result.CreatedBy = product.CreatedBy;
            result.CreatedDate = product.CreatedDate;
            result.DownloadExpiration = product.DownloadExpiration;
            result.DownloadType = product.DownloadType;
            result.EnableReview = product.EnableReview;
            result.Gtin = product.Gtin;
            result.HasUserAgreement = product.HasUserAgreement;
            result.Height = product.Height;
            result.IndexingDate = product.IndexingDate;
            result.IsActive = product.IsActive;
            result.IsBuyable = product.IsBuyable;
            result.Length = product.Length;
            result.ManufacturerPartNumber = product.ManufacturerPartNumber;
            result.MaxNumberOfDownload = product.MaxNumberOfDownload;
            result.MaxQuantity = product.MaxQuantity;
            result.MeasureUnit = product.MeasureUnit;
            result.MinQuantity = product.MinQuantity;
            result.ModifiedBy = product.ModifiedBy;
            result.ModifiedDate = product.ModifiedDate;
            result.Name = product.Name;
            result.PackageType = product.PackageType;
            result.Priority = product.Priority;
            result.ProductType = product.ProductType;
            result.TaxType = product.TaxType;
            result.TrackInventory = product.TrackInventory;
            result.Vendor = product.Vendor;
            result.Weight = product.Weight;
            result.WeightUnit = product.WeightUnit;
            result.Width = product.Width;
            result.StartDate = product.StartDate;
            result.EndDate = product.EndDate;

            result.SeoInfos = product.SeoInfos;

            if (!product.Outlines.IsNullOrEmpty())
            {
                // minimize outline size
                result.Outlines = product.Outlines.Select(outline => outline.ToWebModel()).ToList();
            }

            if (product.Images != null)
            {
                result.Images = product.Images.Select(image => image.ToWebModel(blobUrlResolver)).ToList();
            }

            if (product.Assets != null)
            {
                result.Assets = product.Assets.Select(asset => asset.ToWebModel(blobUrlResolver)).ToList();
            }

            if (product.Variations != null)
            {
                result.Variations = product.Variations.Select(x => x.ToWebModel(blobUrlResolver)).ToList();

                // for nested variations leave only variation properties to decrease resulting JSON
                foreach (var variation in result.Variations)
                {
                    if (variation.Properties != null)
                    {
                        variation.Properties = variation.Properties
                            .Where(property => property.Type == VC.PropertyType.Variation).ToList();
                    }
                }
            }

            if (product.Links != null)
            {
                result.Links = product.Links.Select(link => link.ToWebModel()).ToList();
            }

            if (product.Reviews != null)
            {
                result.Reviews = product.Reviews.Select(review => review.ToWebModel()).ToList();
            }

            if (product.Associations != null)
            {
                result.Associations = product.Associations
                    .Select(association => association.ToWebModel(blobUrlResolver)).ToList();
            }

            if (product.ReferencedAssociations != null)
            {
                result.ReferencedAssociations = product.ReferencedAssociations
                    .Select(association => association.ToWebModel(blobUrlResolver)).ToList();
            }

            // init outline and path
            if (product.Category != null)
            {
                var parents = new List<VC.Category>();
                if (product.Category.Parents != null)
                {
                    parents.AddRange(product.Category.Parents);
                }

                parents.Add(product.Category);

                result.Outline = string.Join("/", parents.Select(parent => parent.Id));
                result.Path = string.Join("/", parents.Select(parent => parent.Name));
            }

            result.TitularItemId = product.MainProductId;

            result.Properties = new List<Property>();

            // need add property for each meta info
            if (product.Properties != null)
            {
                foreach (var property in product.Properties)
                {
                    var webModelProperty = property.ToWebModel();
                    webModelProperty.Values = new List<PropertyValue>();
                    webModelProperty.IsManageable = true;
                    webModelProperty.IsReadOnly = property.Type != VC.PropertyType.Product
                                                  && property.Type != VC.PropertyType.Variation;
                    result.Properties.Add(webModelProperty);
                }
            }

            // populate property values
            if (product.PropertyValues != null)
            {
                var sort = false;
                foreach (var propertyValue in product.PropertyValues.Select(x => x.ToWebModel()))
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
                        property = new Property(propertyValue, product.CatalogId, VC.PropertyType.Product);
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

            return result;
        }
    }
}