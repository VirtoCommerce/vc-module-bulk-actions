namespace VirtoCommerce.BulkActionsModule.Core.Models
{
    using VirtoCommerce.Platform.Core.Common;

    /// <summary>
    /// Base class for all entries used in catalog categories browsing.
    /// </summary>
    public class ListEntry : AuditableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListEntry"/> class.
        /// </summary>
        public ListEntry()
        {
        }

        public ListEntry(string typeName, AuditableEntity entity)
        {
            Type = typeName;
            if (entity != null)
            {
                Id = entity.Id;
                CreatedDate = entity.CreatedDate;
                ModifiedDate = entity.ModifiedDate;
                CreatedBy = entity.CreatedBy;
                ModifiedBy = entity.ModifiedBy;
            }
            else
            {
                // what else?
                // idle
            }
        }

        /// <summary>
        /// Gets or sets the catalog id.
        /// </summary>
        public string CatalogId { get; set; }

        /// <summary>
        /// Gets or sets the entry code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this entry is active.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        public ListEntryLink[] Links { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the all entry parents ids.
        /// </summary>
        public string[] Outline { get; set; }

        /// <summary>
        /// Gets or sets the all entry parents names
        /// </summary>
        public string[] Path { get; set; }

        /// <summary>
        /// Gets or sets the type. E.g. "product", "category"
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }
    }
}