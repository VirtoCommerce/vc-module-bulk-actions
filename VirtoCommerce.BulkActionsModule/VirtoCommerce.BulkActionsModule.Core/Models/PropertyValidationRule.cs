namespace VirtoCommerce.BulkActionsModule.Core.Models
{
    using VirtoCommerce.Platform.Core.Common;

    public class PropertyValidationRule : Entity
    {
        /// <summary>
        /// Gets or sets the char count max.
        /// </summary>
        public int? CharCountMax { get; set; }

        /// <summary>
        /// Gets or sets the char count min.
        /// </summary>
        public int? CharCountMin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is unique.
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Gets or sets the reg exp.
        /// </summary>
        public string RegExp { get; set; }
    }
}