namespace VirtoCommerce.BulkActionsModule.Core.Models
{
    using System.Collections.Generic;

    public class SearchResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResult"/> class.
        /// </summary>
        public SearchResult()
        {
            Entries = new List<ListEntry>();
        }

        /// <summary>
        /// Gets or sets the list entries.
        /// </summary>
        /// <value>
        /// The list entries.
        /// </value>
        public ICollection<ListEntry> Entries { get; set; }

        /// <summary>
        /// Gets or sets the total entries count matching the search criteria.
        /// </summary>
        /// <value>
        /// The total count.
        /// </value>
        public int TotalCount { get; set; }
    }
}