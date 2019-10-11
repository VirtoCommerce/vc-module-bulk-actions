namespace VirtoCommerce.BulkActionsModule.Core.Converters
{
    using System.Web;

    using Omu.ValueInjecter;

    using VirtoCommerce.Platform.Core.Assets;
    using VirtoCommerce.Platform.Core.Common;

    using Asset = VirtoCommerce.BulkActionsModule.Core.Models.Asset;
    using Image = VirtoCommerce.BulkActionsModule.Core.Models.Image;
    using VC = VirtoCommerce.Domain.Catalog.Model;

    public static class AssetConverter
    {
        public static VC.Image ToCoreModel(this Image image)
        {
            var result = new VC.Image();
            result.InjectFrom(image);
            result.Url = image.RelativeUrl;
            return result;
        }

        public static VC.Asset ToCoreModel(this Asset asset)
        {
            var result = new VC.Asset();
            result.InjectFrom(asset);
            result.Url = asset.RelativeUrl;
            return result;
        }

        public static Image ToWebModel(this VC.Image image, IBlobUrlResolver blobUrlResolver)
        {
            // do not use omu.InjectFrom for performance reasons
            var result = new Image
                             {
                                 Group = image.Group,
                                 Id = image.Id,
                                 LanguageCode = image.LanguageCode,
                                 Name = image.Name,
                                 IsInherited = image.IsInherited,
                                 SortOrder = image.SortOrder
                             };

            if (blobUrlResolver != null)
            {
                result.Url = blobUrlResolver.GetAbsoluteUrl(image.Url);
            }

            result.RelativeUrl = image.Url;
            return result;
        }

        public static Asset ToWebModel(this VC.Asset asset, IBlobUrlResolver blobUrlResolver)
        {
            var result = new Asset();
            result.InjectFrom(asset);
            if (asset.Name == null)
            {
                result.Name = HttpUtility.UrlDecode(System.IO.Path.GetFileName(asset.Url));
            }

            if (asset.MimeType == null)
            {
                result.MimeType = MimeTypeResolver.ResolveContentType(asset.Name);
            }

            if (blobUrlResolver != null)
            {
                result.Url = blobUrlResolver.GetAbsoluteUrl(asset.Url);
            }

            result.RelativeUrl = asset.Url;
            result.ReadableSize = result.Size.ToHumanReadableSize();
            return result;
        }
    }
}