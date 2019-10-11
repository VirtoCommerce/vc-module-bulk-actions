namespace VirtoCommerce.BulkActionsModule.Core.BulkActionModels
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using VirtoCommerce.Platform.Core.PushNotifications;

    public class BulkActionPushNotification : PushNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkActionPushNotification"/> class.
        /// </summary>
        /// <param name="creator">
        /// The creator.
        /// </param>
        public BulkActionPushNotification(string creator)
            : base(creator)
        {
            Errors = new List<string>();
        }

        /// <summary>
        /// The error count.
        /// </summary>
        [JsonProperty("errorCount")]
        public long ErrorCount => Errors?.Count ?? 0;

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        [JsonProperty("errors")]
        public ICollection<string> Errors { get; set; }

        /// <summary>
        /// Gets or sets the finished.
        /// </summary>
        [JsonProperty("finished")]
        public DateTime? Finished { get; set; }

        /// <summary>
        /// Gets or sets the job id.
        /// </summary>
        [JsonProperty("jobId")]
        public string JobId { get; set; }

        /// <summary>
        /// Gets or sets the processed count.
        /// </summary>
        [JsonProperty("processedCount")]
        public int? ProcessedCount { get; set; }

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        [JsonProperty("totalCount")]
        public int? TotalCount { get; set; }
    }
}