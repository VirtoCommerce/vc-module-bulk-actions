namespace VirtoCommerce.BulkActionsModule.Core.Converters
{
    using System.Collections.Generic;
    using System.Linq;

    using VirtoCommerce.Domain.Commerce.Model;

    using VC = VirtoCommerce.Domain.Catalog.Model;

    public static class OutlineConverter
    {
        public static VC.Outline ToWebModel(this VC.Outline outline)
        {
            var result = new VC.Outline { Items = new List<VC.OutlineItem>() };

            foreach (var item in outline.Items)
            {
                var newItem = new VC.OutlineItem
                                  {
                                      Id = item.Id,
                                      HasVirtualParent = item.HasVirtualParent,
                                      SeoObjectType = item.SeoObjectType
                                  };

                if (item.SeoInfos != null)
                {
                    newItem.SeoInfos = item.SeoInfos.Select(
                        seoInfo => new SeoInfo
                                       {
                                           IsActive = seoInfo.IsActive,
                                           LanguageCode = seoInfo.LanguageCode,
                                           SemanticUrl = seoInfo.SemanticUrl,
                                           StoreId = seoInfo.StoreId
                                       }).ToList();
                }

                result.Items.Add(newItem);
            }

            return result;
        }
    }
}