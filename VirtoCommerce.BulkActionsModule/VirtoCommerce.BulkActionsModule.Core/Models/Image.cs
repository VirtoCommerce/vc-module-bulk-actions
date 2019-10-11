namespace VirtoCommerce.BulkActionsModule.Core.Models
{
    /// <summary>
    /// Image asset
    /// </summary>
    public class Image : AssetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Image"/> class.
        /// </summary>
        public Image()
        {
            TypeId = "image";
            Group = "images";
        }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public int SortOrder { get; set; }
    }
}