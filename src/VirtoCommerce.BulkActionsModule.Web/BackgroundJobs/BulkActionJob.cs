using System;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Server;
using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
using VirtoCommerce.BulkActionsModule.Core.Services;
using VirtoCommerce.BulkActionsModule.Data.Extensions;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Hangfire;

namespace VirtoCommerce.BulkActionsModule.Web.BackgroundJobs
{
    public class BulkActionJob
    {
        private readonly IBulkActionExecutor _bulkActionExecutor;

        private readonly IPushNotificationManager _pushNotificationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkActionJob"/> class.
        /// </summary>
        /// <param name="pushNotificationManager">
        /// The push notification manager.
        /// </param>
        /// <param name="bulkActionExecutor">
        /// The bulk update action executor.
        /// </param>
        public BulkActionJob(IPushNotificationManager pushNotificationManager, IBulkActionExecutor bulkActionExecutor)
        {
            _pushNotificationManager = pushNotificationManager;
            _bulkActionExecutor = bulkActionExecutor;
        }

        public async Task ExecuteAsync(
            BulkActionContext bulkActionContext,
            BulkActionPushNotification notification,
            IJobCancellationToken cancellationToken,
            PerformContext performContext)
        {
            Validate(bulkActionContext);
            Validate(performContext);

            try
            {
                var tokenWrapper = new JobCancellationTokenWrapper(cancellationToken);
                await _bulkActionExecutor.ExecuteAsync(
                    bulkActionContext,
                    context =>
                    {
                        notification.Patch(context);
                        notification.JobId = performContext.BackgroundJob.Id;
                        _pushNotificationManager.Send(notification);
                    },
                    tokenWrapper);
            }
            catch (JobAbortedException)
            {
                // idle
            }
            catch (Exception exception)
            {
                notification.Errors.Add(exception.ExpandExceptionMessage());
            }
            finally
            {
                notification.Description = "Job finished";
                notification.Finished = DateTime.UtcNow;
                _pushNotificationManager.Send(notification);
            }
        }

        private static void Validate(object context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }
    }
}
