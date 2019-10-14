namespace VirtoCommerce.BulkActionsModule.Data.Services
{
    using System;
    using System.Linq;

    using VirtoCommerce.BulkActionsModule.Core;
    using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
    using VirtoCommerce.Platform.Core.Common;

    public class BulkActionExecutor : IBulkActionExecutor
    {
        private readonly IBulkActionProviderStorage _bulkActionProviderStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkActionExecutor"/> class.
        /// </summary>
        /// <param name="bulkActionProviderStorage">
        /// The bulk update action registrar.
        /// </param>
        public BulkActionExecutor(IBulkActionProviderStorage bulkActionProviderStorage)
        {
            _bulkActionProviderStorage = bulkActionProviderStorage;
        }

        public virtual void Execute(
            BulkActionContext context,
            Action<BulkActionProgressContext> progressCallback,
            ICancellationToken token)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            token.ThrowIfCancellationRequested();

            var totalCount = 0;
            var processedCount = 0;

            var progressContext = new BulkActionProgressContext { Description = "Validation has started…", };
            progressCallback(progressContext);

            try
            {
                var actionProvider = _bulkActionProviderStorage.Get(context.ActionName);
                var action = actionProvider.BulkActionFactory.Create(context);

                var validationResult = action.Validate();
                var proceed = validationResult.Succeeded;

                token.ThrowIfCancellationRequested();

                if (proceed)
                {
                    progressContext.Description = "Validation successfully completed.";
                }
                else
                {
                    progressContext.Description = "Validation has been completed with errors.";
                    progressContext.Errors = validationResult.Errors;
                }

                progressCallback(progressContext);

                if (proceed)
                {
                    var dataSource = actionProvider.DataSourceFactory.Create(context);
                    totalCount = dataSource.GetTotalCount();
                    processedCount = 0;

                    progressContext.ProcessedCount = processedCount;
                    progressContext.TotalCount = totalCount;
                    progressContext.Description = "The process has been started…";
                    progressCallback(progressContext);

                    while (dataSource.Fetch())
                    {
                        token.ThrowIfCancellationRequested();

                        var result = action.Execute(dataSource.Items);

                        if (result.Succeeded)
                        {
                            // idle
                        }
                        else
                        {
                            progressContext.Errors.AddRange(result.Errors);
                        }

                        processedCount += dataSource.Items.Count();
                        progressContext.ProcessedCount = processedCount;

                        if (processedCount == totalCount)
                        {
                            continue;
                        }

                        progressContext.Description = $"{processedCount} out of {totalCount} have been updated.";
                        progressCallback(progressContext);
                    }
                }
                else
                {
                    // idle
                }
            }
            catch (Exception e)
            {
                progressContext.Errors.Add(e.Message);
            }
            finally
            {
                var message = progressContext.Errors?.Count > 0
                                  ? "The process has been completed with errors"
                                  : "Process is completed";
                progressContext.Description = $"{message}: {processedCount} out of {totalCount} have been updated.";
                progressCallback(progressContext);
            }
        }
    }
}