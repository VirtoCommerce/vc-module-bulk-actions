using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
using VirtoCommerce.BulkActionsModule.Core.Services;
using VirtoCommerce.BulkActionsModule.Data.Extensions;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Core.Jobs;
using VirtoCommerce.Platform.Core.PushNotifications;

namespace VirtoCommerce.BulkActionsModule.Web.BackgroundJobs
{
    public class BulkActionJobPayload
    {
        public BulkActionContext Context { get; set; }

        public BulkActionPushNotification Notification { get; set; }
    }

    /// <summary>
    /// Engine-agnostic bulk-action job. Replaces the former Hangfire job that used
    /// <c>PerformContext</c>/<c>IJobCancellationToken</c>: progress still flows through the module's own
    /// push-notification manager; the job id comes from <see cref="IJobExecutionContext.JobId"/> and
    /// cancellation from the handler's <see cref="CancellationToken"/>.
    /// </summary>
    public class BulkActionJob : IBackgroundJobHandler<BulkActionJobPayload>
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

        public async Task Execute(BulkActionJobPayload payload, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Validate(payload);
            Validate(payload.Context);

            var bulkActionContext = payload.Context;
            var notification = payload.Notification;

            try
            {
                await _bulkActionExecutor.ExecuteAsync(
                    bulkActionContext,
                    progressContext =>
                    {
                        notification.Patch(progressContext);
                        notification.JobId = context.JobId;
                        _pushNotificationManager.Send(notification);
                    },
                    cancellationToken);
            }
            catch (OperationCanceledException)
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
