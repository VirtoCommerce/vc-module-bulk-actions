using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.BulkActionsModule.Core;
using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
using VirtoCommerce.BulkActionsModule.Core.Services;
using VirtoCommerce.BulkActionsModule.Data.Authorization;
using VirtoCommerce.BulkActionsModule.Web.BackgroundJobs;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.BulkActionsModule.Web.Controllers.Api
{
    [Authorize]
    [Route("api/bulk/actions")]
    public class BulkActionsController : Controller
    {
        private readonly IBackgroundJobExecutor _backgroundJobExecutor;
        private readonly IBulkActionProviderStorage _bulkActionProviderStorage;
        private readonly IUserNameResolver _userNameResolver;
        private readonly IAuthorizationService _authorizationService;

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
        /// <param name="backgroundJobExecutor">
        /// The background job executor.
        /// </param>
        public BulkActionsController(
            IBulkActionProviderStorage bulkActionProviderStorage,
            IUserNameResolver userNameResolver,
            IBackgroundJobExecutor backgroundJobExecutor,
            IAuthorizationService authorizationService)
        {
            _bulkActionProviderStorage = bulkActionProviderStorage;
            _userNameResolver = userNameResolver;
            _backgroundJobExecutor = backgroundJobExecutor;
            _authorizationService = authorizationService;
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
        [Authorize(ModuleConstants.Security.Permissions.Execute)]
        public ActionResult Cancel(string jobId)
        {
            _backgroundJobExecutor.Delete(jobId);
            return NoContent();
        }

        /// <summary>
        /// Gets action initialization data (could be used to initialize UI).
        /// </summary>
        /// <param name="context">Context for which we want initialization data.</param>
        /// <returns>Initialization data for the given context.</returns>
        [HttpPost]
        [Route("data")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult> GetActionData([FromBody] BulkActionContext context)
        {
            ValidateContext(context);

            var actionProvider = _bulkActionProviderStorage.Get(context.ActionName);

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, context, new BulkActionsAuthorizationRequirement(ModuleConstants.Security.Permissions.Read));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            var factory = actionProvider.BulkActionFactory;
            var action = factory.Create(context);
            var actionData = action.GetActionData();

            return Ok(actionData);
        }

        /// <summary>
        /// Gets the list of all registered actions.
        /// </summary>
        /// <returns>The list of registered actions.</returns>
        [HttpGet]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public ActionResult<BulkActionProvider[]> GetRegisteredActions()
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
        [Authorize(ModuleConstants.Security.Permissions.Execute)]
        public async Task<ActionResult<BulkActionPushNotification>> Run([FromBody] BulkActionContext context)
        {
            ValidateContext(context);

            var actionProvider = _bulkActionProviderStorage.Get(context.ActionName);

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, context, new BulkActionsAuthorizationRequirement(ModuleConstants.Security.Permissions.Read));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            var creator = _userNameResolver.GetCurrentUserName();
            var notification = new BulkActionPushNotification(creator)
            {
                Title = $"{context.ActionName}",
                Description = "Starting…"
            };

            notification.JobId = _backgroundJobExecutor.Enqueue<BulkActionJob>(job => job.Execute(context, notification, JobCancellationToken.Null, null));

            return Ok(notification);
        }

        private static void ValidateContext(BulkActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }
    }
}
