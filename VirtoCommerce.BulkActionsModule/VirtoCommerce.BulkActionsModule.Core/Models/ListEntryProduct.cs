namespace VirtoCommerce.BulkActionsModule.Core.Models
{
    using System.Linq;

    /// <summary>
    /// Product ListEntry record.
    /// </summary>
    public class ListEntryProduct : ListEntry
    {
        public const string TypeName = "product";

        /// <summary>
        /// Initializes a new instance of the <see cref="ListEntryProduct"/> class.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        public ListEntryProduct(Product product)
            : base(TypeName, product)
        {
            ProductType = product.ProductType;

            ImageUrl = product.ImgSrc;
            Code = product.Code;
            Name = product.Name;
            IsActive = product.IsActive ?? true;

            if (!string.IsNullOrEmpty(product.Outline))
            {
                Outline = product.Outline.Split('/').Select(segment => segment).ToArray();
            }

            if (!string.IsNullOrEmpty(product.Path))
            {
                Path = product.Path.Split('/').Select(segment => segment).ToArray();
            }

            if (product.Links != null)
            {
                Links = product.Links.Select(link => new ListEntryLink(link)).ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the product type.
        /// </summary>
        public string ProductType { get; set; }
    }
}