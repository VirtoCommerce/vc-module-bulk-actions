namespace VirtoCommerce.BulkActionsModule.Core.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represent move operation detail
    /// </summary>
    public class MoveOperationContext
    {
        /// <summary>
        /// Gets or sets the catalog.
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the list entries.
        /// </summary>
        public ICollection<ListEntry> Entries { get; set; }
    }
}