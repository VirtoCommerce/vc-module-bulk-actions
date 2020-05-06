namespace VirtoCommerce.BulkActionsModule.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using VirtoCommerce.BulkActionsModule.Core;
    using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
    using VirtoCommerce.Platform.Core.Common;

    public class BulkActionExecutor : IBulkActionExecutor
    {
        private readonly IBulkActionProviderStorage _bulkActionProviderStorage;

        private Action<BulkActionProgressContext> _progressAction;

        private BulkActionProgressContext _progressContext;

        private ICancellationToken _token;

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
            Action<BulkActionProgressContext> progressAction,
            ICancellationToken token)
        {
            // initialize the common context
            _progressAction = progressAction;
            _token = token;
            _progressContext = new BulkActionProgressContext { Description = "Validation has started…" };
            var totalCount = 0;
            var processedCount = 0;

            // begin
            ValidateContext(context);
            SendFeedback();

            try
            {
                var action = GetAction(context);
                ValidateAction(action);
                SendFeedback();

                var dataSource = GetDataSource(context);
                totalCount = dataSource.GetTotalCount();

                SetProcessedCount(processedCount);
                SetTotalCount(totalCount);
                SetDescription("The process has been started…");
                SendFeedback();

                while (dataSource.Fetch())
                {
                    ThrowIfCancellationRequested();

                    var result = action.Execute(dataSource.Items);

                    if (result.Succeeded)
                    {
                        // idle
                    }
                    else
                    {
                        SetErrors(result.Errors);
                    }

                    processedCount += dataSource.Items.Count();
                    SetProcessedCount(processedCount);

                    if (processedCount == totalCount)
                    {
                        continue;
                    }

                    SetDescription($"{processedCount} out of {totalCount} have been updated.");
                    SendFeedback();
                }
            }
            catch (Exception exception)
            {
                SetError(exception.Message);
            }
            finally
            {
                const string ErrorsMessage = "The process has been completed with errors";
                const string CompleteMessage = "Process is completed";
                var message = IsContainsErrors() ? ErrorsMessage : CompleteMessage;
                SetDescription($"{message}: {processedCount} out of {totalCount} have been updated.");
                SendFeedback();
            }
        }

        private static void ValidateContext(object context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }

        private IBulkAction GetAction(BulkActionContext context)
        {
            var actionProvider = _bulkActionProviderStorage.Get(context.ActionName);
            return actionProvider.BulkActionFactory.Create(context);
        }

        private IDataSource GetDataSource(BulkActionContext context)
        {
            var actionProvider = _bulkActionProviderStorage.Get(context.ActionName);
            return actionProvider.DataSourceFactory.Create(context);
        }

        private bool IsContainsErrors()
        {
            return _progressContext.Errors?.Count > 0;
        }

        private void SendFeedback()
        {
            _progressAction(_progressContext);
        }

        private void SetDescription(string description)
        {
            _progressContext.Description = description;
        }

        private void SetError(string errorMessage)
        {
            _progressContext.Errors.Add(errorMessage);
        }

        private void SetErrors(IEnumerable<string> errorMessages)
        {
            _progressContext.Errors.AddRange(errorMessages);
        }

        private void SetProcessedCount(int processedCount)
        {
            _progressContext.ProcessedCount = processedCount;
        }

        private void SetTotalCount(int totalCount)
        {
            _progressContext.TotalCount = totalCount;
        }

        private void ThrowIfCancellationRequested()
        {
            _token.ThrowIfCancellationRequested();
        }

        private void ValidateAction(IBulkAction action)
        {
            var validationResult = action.Validate();
            var proceed = validationResult.Succeeded;

            ThrowIfCancellationRequested();

            if (proceed)
            {
                _progressContext.Description = "Validation successfully completed.";
            }
            else
            {
                _progressContext.Description = "Validation has been completed with errors.";
                _progressContext.Errors = validationResult.Errors;
            }
        }
    }
}