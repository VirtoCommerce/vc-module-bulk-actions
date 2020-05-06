namespace VirtoCommerce.BulkActionsModule.Web.BackgroundJobs
{
    using Hangfire;

    using VirtoCommerce.Platform.Core.Common;

    public class JobCancellationTokenWrapper : ICancellationToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobCancellationTokenWrapper"/> class.
        /// </summary>
        /// <param name="jobCancellationToken">
        /// The job cancellation token.
        /// </param>
        public JobCancellationTokenWrapper(IJobCancellationToken jobCancellationToken)
        {
            JobCancellationToken = jobCancellationToken;
        }

        public IJobCancellationToken JobCancellationToken { get; }

        public void ThrowIfCancellationRequested()
        {
            JobCancellationToken.ThrowIfCancellationRequested();
        }
    }
}