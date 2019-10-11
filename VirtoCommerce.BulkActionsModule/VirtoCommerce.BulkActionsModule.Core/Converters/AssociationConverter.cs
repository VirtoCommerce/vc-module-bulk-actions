namespace VirtoCommerce.BulkActionsModule.Core.Converters
{
    using Omu.ValueInjecter;

    using VirtoCommerce.Platform.Core.Assets;

    using ProductAssociation = VirtoCommerce.BulkActionsModule.Core.Models.ProductAssociation;
    using VC = VirtoCommerce.Domain.Catalog.Model;

    public static class AssociationConverter
    {
        public static VC.ProductAssociation ToCoreModel(this ProductAssociation association)
        {
            var result = new VC.ProductAssociation();
            result.InjectFrom(association);
            result.Tags = association.Tags;
            return result;
        }

        public static ProductAssociation ToWebModel(
            this VC.ProductAssociation association,
            IBlobUrlResolver blobUrlResolver)
        {
            // do not use omu.InjectFrom for performance reasons
            var result = new ProductAssociation
                             {
                                 AssociatedObjectId = association.AssociatedObjectId,
                                 AssociatedObjectType = association.AssociatedObjectType,
                                 Quantity = association.Quantity,
                                 Tags = association.Tags,
                                 Type = association.Type,
                                 Priority = association.Priority
                             };

            result.Tags = association.Tags;

            if (association.AssociatedObject != null)
            {
                var product = association.AssociatedObject as VC.CatalogProduct;
                var category = association.AssociatedObject as VC.Category;
                if (product != null)
                {
                    var associatedProduct = product.ToWebModel(blobUrlResolver);
                    result.AssociatedObjectImg = associatedProduct.ImgSrc;
                    result.AssociatedObjectName = associatedProduct.Name;
                }

                if (category != null)
                {
                    var associatedCategory = category.ToWebModel(blobUrlResolver);
                    result.AssociatedObjectImg = associatedCategory.ImgSrc;
                    result.AssociatedObjectName = associatedCategory.Name;
                }
            }

            return result;
        }
    }
}