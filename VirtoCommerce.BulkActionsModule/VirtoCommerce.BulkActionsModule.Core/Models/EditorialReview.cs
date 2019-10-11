namespace VirtoCommerce.BulkActionsModule.Core.Models
{
    using VirtoCommerce.Platform.Core.Common;

    /// <summary>
    /// Editorial review for an item.
    /// </summary>
    public class EditorialReview : Entity
    {
        /// <summary>
        /// Gets or sets the review content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is inherited.
        /// </summary>
        public bool IsInherited { get; set; }

        /// <summary>
        /// Gets or sets the review language.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the type of the review.
        /// </summary>
        /// <value>
        /// The type of the review.
        /// </value>
        public string ReviewType { get; set; }
    }
}