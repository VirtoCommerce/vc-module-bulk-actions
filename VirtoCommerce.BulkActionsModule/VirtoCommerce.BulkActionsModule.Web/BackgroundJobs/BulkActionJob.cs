namespace VirtoCommerce.BulkActionsModule.Web.BackgroundJobs
{
    using System;

    using Hangfire;
    using Hangfire.Server;

    using VirtoCommerce.BulkActionsModule.Core.BulkActionAbstractions;
    using VirtoCommerce.BulkActionsModule.Core.BulkActionModels;
    using VirtoCommerce.BulkActionsModule.Data.Extensions;
    using VirtoCommerce.Platform.Core.PushNotifications;
    using VirtoCommerce.Platform.Data.Common;

    public class BulkActionJob
    {
        private readonly IPushNotificationManager _pushNotificationManager;

        private readonly IBulkActionExecutor _bulkActionExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkActionJob"/> class.
        /// </summary>
        /// <param name="bulkActionProviderStorage">
        /// The bulk update action registrar.
        /// </param>
        /// <param name="pushNotificationManager">
        /// The push notification manager.
        /// </param>
        /// <param name="bulkActionExecutor">
        /// The bulk update action executor.
        /// </param>
        public BulkActionJob(
            IBulkActionProviderStorage bulkActionProviderStorage,
            IPushNotificationManager pushNotificationManager,
            IBulkActionExecutor bulkActionExecutor)
        {
            _pushNotificationManager = pushNotificationManager;
            _bulkActionExecutor = bulkActionExecutor;
        }

        public void Execute(
            BulkActionContext bulkActionContext,
            BulkActionPushNotification notification,
            IJobCancellationToken cancellationToken,
            PerformContext performContext)
        {
            if (bulkActionContext == null)
            {
                throw new ArgumentNullException(nameof(bulkActionContext));
            }

            if (performContext == null)
            {
                throw new ArgumentNullException(nameof(performContext));
            }

            void progressCallback(BulkActionProgressContext progressContext)
            {
                notification.Patch(progressContext);
                notification.JobId = performContext.BackgroundJob.Id;
                _pushNotificationManager.Upsert(notification);
            }

            try
            {
                _bulkActionExecutor.Execute(
                    bulkActionContext,
                    progressCallback,
                    new JobCancellationTokenWrapper(cancellationToken));
            }
            catch (JobAbortedException)
            {
                // idle
            }
            catch (Exception ex)
            {
                notification.Errors.Add(ex.ExpandExceptionMessage());
            }
            finally
            {
                notification.Description = "Job finished";
                notification.Finished = DateTime.UtcNow;
                _pushNotificationManager.Upsert(notification);
            }
        }
    }
}