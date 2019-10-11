namespace VirtoCommerce.BulkActionsModule.Core.Models
{
    using System;

    using VirtoCommerce.Domain.Catalog.Model;

    public class SearchCriteria
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchCriteria"/> class.
        /// </summary>
        public SearchCriteria()
        {
            Take = 20;
        }

        /// <summary>
        /// Gets or sets the catalog id.
        /// </summary>
        public string CatalogId { get; set; }

        /// <summary>
        /// Gets or sets the catalog ids.
        /// </summary>
        public string[] CatalogIds { get; set; }

        /// <summary>
        /// Gets or sets the category id.
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the category ids.
        /// </summary>
        public string[] CategoryIds { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// Product or category code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the end price.
        /// </summary>
        public decimal? EndPrice { get; set; }

        /// <summary>
        /// Gets or sets the facets collection
        /// Item format: name:value1,value2,value3
        /// </summary>
        public string[] Facets { get; set; }

        // Hides direct linked categories in virtual category displayed only linked category content without itself
        public bool HideDirectLinkedCategories { get; set; }

        /// <summary>
        /// Gets or sets the index date.
        /// All products have index date less that specified.
        /// </summary>
        public DateTime? IndexDate { get; set; }

        /// <summary>
        /// Gets or sets the keyword.
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the main product id.
        /// Search within variations of specified main product.
        /// </summary>
        public string MainProductId { get; set; }

        /// <summary>
        /// Gets or sets the only buyable.
        /// Search only buyable products.
        /// </summary>
        public bool? OnlyBuyable { get; set; }

        /// <summary>
        /// Gets or sets the only with tracking inventory.
        /// Search only inventory tracking products.
        /// </summary>
        public bool? OnlyWithTrackingInventory { get; set; }

        /// <summary>
        /// Gets or sets the outline.
        /// Category1/Category2
        /// </summary>
        public string Outline { get; set; }

        /// <summary>
        /// Gets or sets the pricelist id.
        /// </summary>
        public string PricelistId { get; set; }

        /// <summary>
        /// Gets or sets the pricelist ids.
        /// </summary>
        public string[] PricelistIds { get; set; }

        /// <summary>
        /// Gets or sets the product type.
        /// Search product with specified type.
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// Gets or sets the product types.
        /// Search product with specified types.
        /// </summary>
        public string[] ProductTypes { get; set; }

        /// <summary>
        /// Gets or sets the property values.
        /// For filtration by specified properties values.
        /// </summary>
        public PropertyValue[] PropertyValues { get; set; }

        /// <summary>
        /// Gets or sets the response group.
        /// </summary>
        public SearchResponseGroup ResponseGroup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether search in children.
        /// Search in all children categories for specified catalog or categories.
        /// </summary>
        public bool SearchInChildren { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether search in variations.
        /// Also, search in variations.
        /// </summary>
        public bool SearchInVariations { get; set; }

        /// <summary>
        /// Gets or sets the skip.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the sort.
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// Gets or sets the start date from.
        /// </summary>
        public DateTime? StartDateFrom { get; set; }

        /// <summary>
        /// Gets or sets the start price.
        /// </summary>
        public decimal? StartPrice { get; set; }

        /// <summary>
        /// Gets or sets the store id.
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the take.
        /// </summary>
        public int Take { get; set; }

        /// <summary>
        /// Gets or sets search terms collection
        /// Item format: name:value1,value2,value3
        /// </summary>
        public string[] Terms { get; set; }

        /// <summary>
        /// Gets or sets the vendor id.
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// Gets or sets the vendor ids.
        /// </summary>
        public string[] VendorIds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether with hidden.
        /// </summary>
        public bool WithHidden { get; set; }
    }
}