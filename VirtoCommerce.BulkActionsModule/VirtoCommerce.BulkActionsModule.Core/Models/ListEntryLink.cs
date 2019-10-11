namespace VirtoCommerce.BulkActionsModule.Core.Models
{
    /// <summary>
    /// Information to define linking information from item or category to category.
    /// </summary>
    public class ListEntryLink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListEntryLink"/> class.
        /// </summary>
        public ListEntryLink()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListEntryLink"/> class.
        /// </summary>
        /// <param name="link">
        /// The link.
        /// </param>
        public ListEntryLink(CategoryLink link)
        {
            CatalogId = link.CatalogId;
            CategoryId = link.CategoryId;
            ListEntryId = link.SourceItemId;
        }

        /// <summary>
        /// Gets or sets the target catalog identifier.
        /// </summary>
        /// <value>
        /// The catalog identifier.
        /// </value>
        public string CatalogId { get; set; }

        /// <summary>
        /// Gets or sets the target category identifier.
        /// </summary>
        /// <value>
        /// The category identifier.
        /// </value>
        public string CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the list entry identifier.
        /// </summary>
        /// <value>
        /// The list entry identifier.
        /// </value>
        public string ListEntryId { get; set; }

        /// <summary>
        /// Gets or sets the type of the list entry. E.g. "product", "category"
        /// </summary>
        /// <value>
        /// The type of the list entry.
        /// </value>
        public string ListEntryType { get; set; }
    }
}