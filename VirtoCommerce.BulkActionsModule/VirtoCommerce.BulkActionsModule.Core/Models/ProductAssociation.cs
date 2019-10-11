namespace VirtoCommerce.BulkActionsModule.Core.Models
{
    /// <summary>
    /// Class containing associated product information like 'Accessory', 'Related Item', etc.
    /// </summary>
    public class ProductAssociation
    {
        /// <summary>
        /// Gets or sets a primary key of associated object
        /// Each link element can have an associated object like Product, Category, etc.
        /// </summary>
        public string AssociatedObjectId { get; set; }

        /// <summary>
        /// Gets or sets associated object image URL
        /// </summary>
        public string AssociatedObjectImg { get; set; }

        /// <summary>
        /// Gets or sets the display name for associated object
        /// </summary>
        public string AssociatedObjectName { get; set; }

        /// <summary>
        /// Gets or sets the associated object type
        /// </summary>
        public string AssociatedObjectType { get; set; }

        /// <summary>
        /// Gets or sets the order in which the associated product is displayed.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the quantity for associated object
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// Gets or sets the association tags
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// Gets or sets the ProductAssociation type.
        /// </summary>
        /// <value>
        /// Accessories, Up-Sales, Cross-Sales, Related etc
        /// </value>
        public string Type { get; set; }
    }
}