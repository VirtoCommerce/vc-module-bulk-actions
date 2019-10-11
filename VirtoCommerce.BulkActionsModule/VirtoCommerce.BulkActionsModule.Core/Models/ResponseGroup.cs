namespace VirtoCommerce.BulkActionsModule.Core.Models
{
    using System;

    [Flags]
    public enum ResponseGroup
    {
        /// <summary>
        /// The with products.
        /// </summary>
        WithProducts = 1,

        /// <summary>
        /// The with categories.
        /// </summary>
        WithCategories = 2,

        /// <summary>
        /// The with properties.
        /// </summary>
        WithProperties = 4,

        /// <summary>
        /// The with catalogs.
        /// </summary>
        WithCatalogs = 8,

        /// <summary>
        /// The with variations.
        /// </summary>
        WithVariations = 16,
    }
}