namespace VirtoCommerce.BulkActionsModule.Core.Converters
{
    using System.Collections.Generic;
    using System.Linq;

    using Omu.ValueInjecter;

    using Property = VirtoCommerce.BulkActionsModule.Core.Models.Property;
    using VC = VirtoCommerce.Domain.Catalog.Model;

    public static class PropertyConverter
    {
        public static Property ToWebModel(this VC.Property property)
        {
            var result = new Property
            {
                Id = property.Id,
                Name = property.Name,
                Required = property.Required,
                Type = property.Type,
                Multivalue = property.Multivalue,
                CatalogId = property.CatalogId,
                CategoryId = property.CategoryId,
                Dictionary = property.Dictionary,
                ValueType = property.ValueType
            };
            result.Type = property.Type;
            result.Multilanguage = property.Multilanguage;
            result.IsInherited = property.IsInherited;
            result.Hidden = property.Hidden;
            result.ValueType = property.ValueType;
            result.Type = property.Type;
            result.Attributes = property.Attributes?.Select(x => x.ToWebModel()).ToList();
            result.DisplayNames = property.DisplayNames;
            result.ValidationRule = property.ValidationRules?.FirstOrDefault()?.ToWebModel();

            return result;
        }

        public static VC.Property ToCoreModel(this Property property)
        {
            var result = new VC.Property();

            result.InjectFrom(property);
            result.ValueType = property.ValueType;
            result.Type = property.Type;
            result.DisplayNames = property.DisplayNames;
            result.Hidden = property.Hidden;

            if (property.Attributes != null)
            {
                result.Attributes = property.Attributes.Select(x => x.ToCoreModel()).ToList();
            }

            if (property.ValidationRule != null)
            {
                result.ValidationRules = new List<VC.PropertyValidationRule> { property.ValidationRule.ToCoreModel() };
            }
            else
            {
                result.ValidationRules = new List<VC.PropertyValidationRule>();
            }

            return result;
        }
    }
}