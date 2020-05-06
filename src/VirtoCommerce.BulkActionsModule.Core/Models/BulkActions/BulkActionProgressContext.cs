namespace VirtoCommerce.BulkActionsModule.Core.Models.BulkActions
{
    using System.Collections.Generic;

    public class BulkActionProgressContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkActionProgressContext"/> class.
        /// </summary>
        public BulkActionProgressContext()
        {
            Errors = new List<string>();
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Gets or sets the processed count.
        /// </summary>
        public int? ProcessedCount { get; set; }

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        public int? TotalCount { get; set; }
    }
}