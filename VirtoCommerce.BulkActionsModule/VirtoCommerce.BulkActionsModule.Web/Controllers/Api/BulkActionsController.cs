namespace VirtoCommerce.BulkActionsModule.Web.Controllers.Api
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;

    using Hangfire;

    using VirtoCommerce.BulkActionsModule.Core;
    using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
    using VirtoCommerce.BulkActionsModule.Web.BackgroundJobs;
    using VirtoCommerce.Platform.Core.Security;
    using VirtoCommerce.Platform.Core.Web.Security;

    [RoutePrefix("api/bulk")]
    public class BulkActionsController : ApiController
    {
        private readonly IUserNameResolver _userNameResolver;

        private readonly IBulkActionProviderStorage _bulkActionProviderStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkActionsController"/> class.
        /// </summary>
        /// <param name="bulkActionProviderStorage">
        /// The bulk update action registrar.
        /// </param>
        /// <param name="userNameResolver">
        /// The user name resolver.
        /// </param>
        public BulkActionsController(
            IBulkActionProviderStorage bulkActionProviderStorage,
            IUserNameResolver userNameResolver)
        {
            _bulkActionProviderStorage = bulkActionProviderStorage;
            _userNameResolver = userNameResolver;
        }

        /// <summary>
        /// Attempts to cancel running task.
        /// </summary>
        /// <param name="jobId">
        /// The job id.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [HttpDelete]
        [Route]
        [CheckPermission(Permission = BulkActionPredefinedPermissions.Execute)]
        public IHttpActionResult Cancel(string jobId)
        {
            BackgroundJob.Delete(jobId);
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Gets action initialization data (could be used to initialize UI).
        /// </summary>
        /// <param name="context">Context for which we want initialization data.</param>
        /// <returns>Initialization data for the given context.</returns>
        [HttpPost]
        [Route("data")]
        [CheckPermission(Permission = BulkActionPredefinedPermissions.Read)]
        public IHttpActionResult GetActionData([FromBody] BulkActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var actionProvider = _bulkActionProviderStorage.Get(context.ActionName);

            if (Authorize(actionProvider, context))
            {
                var factory = actionProvider.BulkActionFactory;
                var action = factory.Create(context);
                var actionData = action.GetActionData();
                return Ok(actionData);
            }

            return Unauthorized();
        }

        /// <summary>
        /// Gets the list of all registered actions
        /// </summary>
        /// <returns>The list of registered actions</returns>
        [HttpGet]
        [Route]
        [ResponseType(typeof(BulkActionProvider[]))]
        [CheckPermission(Permission = BulkActionPredefinedPermissions.Read)]
        public IHttpActionResult GetRegisteredActions()
        {
            var all = _bulkActionProviderStorage.GetAll();
            var array = all.ToArray();
            return Ok(array);
        }

        /// <summary>
        /// Starts bulk action background job.
        /// </summary>
        /// <param name="context">Execution context.</param>
        /// <returns>Notification with job id.</returns>
        [HttpPost]
        [Route]
        [CheckPermission(Permission = BulkActionPredefinedPermissions.Execute)]
        [ResponseType(typeof(BulkActionPushNotification))]
        public IHttpActionResult Run([FromBody] BulkActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var actionProvider = _bulkActionProviderStorage.Get(context.ActionName);

            if (Authorize(actionProvider, context))
            {
                var creator = _userNameResolver.GetCurrentUserName();
                var notification = new BulkActionPushNotification(creator)
                                       {
                                           Title = $"{context.ActionName}", Description = "Starting…"
                                       };

                notification.JobId = BackgroundJob.Enqueue<BulkActionJob>(job => job.Execute(context, notification, JobCancellationToken.Null, null));

                return Ok(notification);
            }

            return Unauthorized();
        }


        /// <summary>
        /// Performs all security handlers checks, and returns true if all are succeeded.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="context"></param>
        /// <returns>True if all checks are succeeded, otherwise false.</returns>
        [SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "<Pending>")]
        private bool Authorize(IBulkActionProvider provider, BulkActionContext context)
        {
            // TechDebt: Need to add permission and custom authorization for bulk update.
            // For that we could use IExportSecurityHandler and IPermissionExportSecurityHandlerFactory
            // - just need to move them to platform and remove export specific objects
            return true;
        }
    }
}