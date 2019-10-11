namespace VirtoCommerce.BulkActionsModule.Core.Converters
{
    using PropertyValidationRule = VirtoCommerce.BulkActionsModule.Core.Models.PropertyValidationRule;
    using VC = VirtoCommerce.Domain.Catalog.Model;

    public static class PropertyValidationRuleConverter
    {
        public static VC.PropertyValidationRule ToCoreModel(this PropertyValidationRule validationRule)
        {
            var retVal = new VC.PropertyValidationRule
                             {
                                 Id = validationRule.Id,
                                 IsUnique = validationRule.IsUnique,
                                 CharCountMin = validationRule.CharCountMin,
                                 CharCountMax = validationRule.CharCountMax,
                                 RegExp = validationRule.RegExp
                             };

            return retVal;
        }

        public static PropertyValidationRule ToWebModel(this VC.PropertyValidationRule validationRule)
        {
            var result = new PropertyValidationRule
                             {
                                 Id = validationRule.Id,
                                 IsUnique = validationRule.IsUnique,
                                 CharCountMin = validationRule.CharCountMin,
                                 CharCountMax = validationRule.CharCountMax,
                                 RegExp = validationRule.RegExp
                             };

            return result;
        }
    }
}