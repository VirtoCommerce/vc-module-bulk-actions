namespace VirtoCommerce.BulkActionsModule.Core.Models
{
    using VirtoCommerce.Platform.Core.Common;

    /// <summary>
    /// Base class for assets.
    /// </summary>
    public class AssetBase : Entity
    {
        /// <summary>
        /// Gets or sets the asset group name.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is inherited.
        /// </summary>
        public bool IsInherited { get; set; }

        /// <summary>
        /// Gets or sets the asset language.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the asset name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the relative url.
        /// </summary>
        public string RelativeUrl { get; set; }

        /// <summary>
        /// Gets or sets the asset type identifier.
        /// </summary>
        /// <value>
        /// The type identifier.
        /// </value>
        public string TypeId { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public string Url { get; set; }
    }
}