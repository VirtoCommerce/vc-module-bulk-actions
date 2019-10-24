namespace VirtoCommerce.BulkActionsModule.Web.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;

    using Hangfire;

    using VirtoCommerce.BulkActionsModule.Core;
    using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
    using VirtoCommerce.BulkActionsModule.Core.Security;
    using VirtoCommerce.BulkActionsModule.Data.Security;
    using VirtoCommerce.BulkActionsModule.Web.BackgroundJobs;
    using VirtoCommerce.Platform.Core.Common;
    using VirtoCommerce.Platform.Core.Security;
    using VirtoCommerce.Platform.Core.Web.Security;

    [RoutePrefix("api/bulk/actions")]
    public class BulkActionsController : ApiController
    {
        private readonly IBulkActionProviderStorage _bulkActionProviderStorage;

        private readonly ISecurityHandlerFactory _securityHandlerFactory;

        private readonly IUserNameResolver _userNameResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkActionsController"/> class.
        /// </summary>
        /// <param name="bulkActionProviderStorage">
        /// The bulk action provider storage.
        /// </param>
        /// <param name="userNameResolver">
        /// The user name resolver.
        /// </param>
        /// <param name="securityHandlerFactory">
        /// The security handler factory.
        /// </param>
        public BulkActionsController(
            IBulkActionProviderStorage bulkActionProviderStorage,
            IUserNameResolver userNameResolver,
            ISecurityHandlerFactory securityHandlerFactory)
        {
            _bulkActionProviderStorage = bulkActionProviderStorage;
            _userNameResolver = userNameResolver;
            _securityHandlerFactory = securityHandlerFactory;
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

            if (IsAuthorizedUserHasPermissions(actionProvider.Permissions))
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
            var allActions = _bulkActionProviderStorage.GetAll();
            return Ok(allActions.ToArray());
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

            if (IsAuthorizedUserHasPermissions(actionProvider.Permissions))
            {
                var creator = _userNameResolver.GetCurrentUserName();
                var notification = new BulkActionPushNotification(creator)
                {
                    Title = $"{context.ActionName}",
                    Description = "Starting…"
                };

                notification.JobId = BackgroundJob.Enqueue<BulkActionJob>(
                    job => job.Execute(context, notification, JobCancellationToken.Null, null));

                return Ok(notification);
            }

            return Unauthorized();
        }

        /// <summary>
        /// Performs all security handlers checks, and returns true if all are succeeded.
        /// </summary>
        /// <param name="permissions">
        /// The permissions.
        /// </param>
        /// <returns>
        /// True if all checks are succeeded, otherwise false
        /// </returns>
        private bool IsAuthorizedUserHasPermissions(string[] permissions)
        {
            var handlers = new List<ISecurityHandler>();

            if (!permissions.IsNullOrEmpty())
            {
                var securityHandler = _securityHandlerFactory.Create(permissions);
                handlers.Add(securityHandler);
            }
            else
            {
                // idle
            }

            return handlers.All(handler => handler.Authorize(User.Identity.Name));
        }
    }
}