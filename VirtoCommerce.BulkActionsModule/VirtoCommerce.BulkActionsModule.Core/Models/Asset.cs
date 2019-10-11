namespace VirtoCommerce.BulkActionsModule.Core.Models
{
    /// <summary>
    /// Asset containing any content.
    /// </summary>
    public class Asset : AssetBase
    {
        public Asset()
        {
            TypeId = "asset";
        }

        /// <summary>
        /// Gets or sets the mime type.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the readable size.
        /// </summary>
        public string ReadableSize { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public long Size { get; set; }
    }
}