namespace VirtoCommerce.BulkActionsModule.Core.Converters
{
    using Omu.ValueInjecter;

    using PropertyValue = VirtoCommerce.BulkActionsModule.Core.Models.PropertyValue;
    using VC = VirtoCommerce.Domain.Catalog.Model;

    public static class PropertyValueConverter
    {
        public static VC.PropertyValue ToCoreModel(this PropertyValue propertyValue)
        {
            var result = new VC.PropertyValue();
            result.InjectFrom(propertyValue);
            result.Value = propertyValue.Value;
            result.ValueType = propertyValue.ValueType;
            return result;
        }

        public static PropertyValue ToWebModel(this VC.PropertyValue propertyValue)
        {
            var result = new PropertyValue
                             {
                                 Id = propertyValue.Id,
                                 LanguageCode = propertyValue.LanguageCode,
                                 PropertyId = propertyValue.PropertyId,
                                 PropertyName = propertyValue.PropertyName,
                                 ValueId = propertyValue.ValueId,
                                 ValueType = propertyValue.ValueType,
                                 Alias = propertyValue.Alias,
                                 IsInherited = propertyValue.IsInherited
                             };

            if (propertyValue.Property != null)
            {
                result.PropertyId = propertyValue.Property.Id;
                result.PropertyMultivalue = propertyValue.Property.Multivalue;
            }

            result.Value = propertyValue.Value;
            return result;
        }
    }
}