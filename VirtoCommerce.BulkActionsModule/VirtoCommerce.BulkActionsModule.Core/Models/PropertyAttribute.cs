namespace VirtoCommerce.BulkActionsModule.Core.Models
{
    using VirtoCommerce.Platform.Core.Common;

    /// <summary>
    /// Additional meta information for a Property
    /// </summary>
    public class PropertyAttribute : Entity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        public Property Property { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }
    }
}