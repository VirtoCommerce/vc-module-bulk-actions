namespace VirtoCommerce.BulkActionsModule.Core.Models.BulkActions
{
    using System.Collections.Generic;

    public class BulkActionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkActionResult"/> class.
        /// </summary>
        public BulkActionResult()
        {
            Errors = new List<string>();
        }

        /// <summary>
        /// The success.
        /// </summary>
        public static BulkActionResult Success => new BulkActionResult { Succeeded = true };

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether succeeded.
        /// </summary>
        public bool Succeeded { get; set; }
    }
}