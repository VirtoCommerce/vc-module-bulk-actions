namespace VirtoCommerce.BulkActionsModule.Web.BackgroundJobs
{
    using System;
    using System.Linq.Expressions;

    using Hangfire;

    public class BackgroundJobExecutor : IBackgroundJobExecutor
    {
        public void Delete(string jobId)
        {
            BackgroundJob.Delete(jobId);
        }

        public string Enqueue<T>(Expression<Action<T>> expression)
        {
            return BackgroundJob.Enqueue(expression);
        }
    }
}