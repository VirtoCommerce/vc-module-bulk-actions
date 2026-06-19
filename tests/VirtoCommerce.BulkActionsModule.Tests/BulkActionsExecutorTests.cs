using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
using VirtoCommerce.BulkActionsModule.Core.Services;
using VirtoCommerce.BulkActionsModule.Data.Services;
using VirtoCommerce.Platform.Core.Common;
using Xunit;

namespace VirtoCommerce.BulkActionsModule.Tests
{
    public class BulkActionsExecutorTests
    {
        [Fact]
        public async Task Execute_PagedDataSource_InvokeFetch()
        {
            // arrange
            var succeeded = true;
            var bulkAction = Mock.Of<IBulkAction>();
            var cancellationToken = CancellationToken.None;
            var bulkActionProviderStorage = Mock.Of<IBulkActionProviderStorage>();
            var pagedDataSource = Mock.Of<IDataSource>();
            var bulkActionProvider = Mock.Of<IBulkActionProvider>();
            var bulkActionValidationResult = Mock.Of<BulkActionResult>(t => t.Succeeded == succeeded);
            var bulkActionResult = Mock.Of<BulkActionResult>(t => t.Succeeded == succeeded);

            var bulkActionProviderStorageMock = Mock.Get(bulkActionProviderStorage);
            var bulkActionMock = Mock.Get(bulkAction);
            var bulkActionProviderMock = Mock.Get(bulkActionProvider);
            var pagedDataSourceMock = Mock.Get(pagedDataSource);

            bulkActionMock.Setup(t => t.ValidateAsync()).ReturnsAsync(bulkActionValidationResult);
            bulkActionMock.Setup(t => t.ExecuteAsync(It.IsAny<IEnumerable<IEntity>>())).ReturnsAsync(bulkActionResult);
            bulkActionProviderMock.Setup(t => t.BulkActionFactory.Create(It.IsAny<BulkActionContext>())).Returns(bulkAction);
            bulkActionProviderMock.Setup(t => t.DataSourceFactory.Create(It.IsAny<BulkActionContext>())).Returns(pagedDataSource);
            bulkActionProviderStorageMock.Setup(t => t.Get(It.IsAny<string>())).Returns(bulkActionProvider);
            pagedDataSourceMock.SetupSequence(t => t.FetchAsync()).ReturnsAsync(true).ReturnsAsync(false);

            // act
            var bulkActionExecutor = new BulkActionExecutor(bulkActionProviderStorageMock.Object);
            await bulkActionExecutor.ExecuteAsync(Mock.Of<BulkActionContext>(), callback => { }, cancellationToken);

            // assert
            pagedDataSourceMock.Verify(t => t.FetchAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task Execute_CanceledToken_ThrowsOperationCanceled()
        {
            // arrange
            var bulkAction = Mock.Of<IBulkAction>();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var cancellationToken = cancellationTokenSource.Token;
            var bulkActionValidationResult = Mock.Of<BulkActionResult>(t => t.Succeeded == true);
            var bulkActionFactory = Mock.Of<IBulkActionFactory>();
            var bulkActionProviderStorage = Mock.Of<IBulkActionProviderStorage>();
            var bulkActionExecutor = new BulkActionExecutor(bulkActionProviderStorage);
            var bulkActionProvider = Mock.Of<IBulkActionProvider>(t => t.BulkActionFactory == bulkActionFactory);

            var bulkActionFactoryMock = Mock.Get(bulkActionFactory);
            var bulkActionProviderStorageMock = Mock.Get(bulkActionProviderStorage);
            var bulkActionMock = Mock.Get(bulkAction);

            bulkActionMock.Setup(t => t.ValidateAsync()).ReturnsAsync(bulkActionValidationResult);
            bulkActionFactoryMock.Setup(t => t.Create(It.IsAny<BulkActionContext>())).Returns(bulkAction);
            bulkActionProviderStorageMock.Setup(t => t.Get(It.IsAny<string>())).Returns(bulkActionProvider);

            // act
            var act = async () => await bulkActionExecutor.ExecuteAsync(Mock.Of<BulkActionContext>(), callback => { }, cancellationToken);

            // assert
            await act.Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task Execute_NullableArgs_ThrowArgumentNullException()
        {
            // arrange
            var cancellationToken = CancellationToken.None;
            var bulkActionProviderStorage = Mock.Of<IBulkActionProviderStorage>();
            var bulkActionExecutor = new BulkActionExecutor(bulkActionProviderStorage);

            // act
            await bulkActionExecutor.ExecuteAsync(Mock.Of<BulkActionContext>(), callback => { }, cancellationToken);
            var action = new Action(() => bulkActionExecutor.ExecuteAsync(null, null, default).GetAwaiter().GetResult());

            // assert
            action.Should().Throw<ArgumentNullException>();
        }
    }
}
