namespace VirtoCommerce.BulkActionsModule.Core.Models.BulkActions
{
    using VirtoCommerce.BulkActionsModule.Core.Models;

    public class DataQuery
    {
        /// <summary>
        /// Gets or sets the list entries.
        /// </summary>
        public ListEntry[] ListEntries { get; set; }

        /// <summary>
        /// Gets or sets the search criteria.
        /// </summary>
        public SearchCriteria SearchCriteria { get; set; }

        /// <summary>
        /// Gets or sets the skip.
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// Gets or sets the take.
        /// </summary>
        public int? Take { get; set; }
    }
}