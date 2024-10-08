using System.Threading.Tasks;

namespace VirtoCommerce.BulkActionsModule.Web.BackgroundJobs
{
    using System;
    using System.Linq.Expressions;

    public interface IBackgroundJobExecutor
    {
        void Delete(string jobId);

        string Enqueue<T>(Expression<Func<T, Task>> expression);
    }
}
