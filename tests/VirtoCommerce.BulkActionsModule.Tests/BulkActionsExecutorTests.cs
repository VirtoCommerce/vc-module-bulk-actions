using System.Threading.Tasks;
using VirtoCommerce.BulkActionsModule.Core.Services;

namespace VirtoCommerce.BulkActionsModule.Tests
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Moq;

    using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
    using VirtoCommerce.BulkActionsModule.Data.Services;
    using VirtoCommerce.Platform.Core.Common;

    using Xunit;

    public class BulkActionsExecutorTests
    {
        [Fact]
        public async Task Execute_PagedDataSource_InvokeFetch()
        {
            // arrange
            var succeeded = true;
            var bulkAction = Mock.Of<IBulkAction>();
            var cancellationToken = Mock.Of<ICancellationToken>();
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
        public async Task Execute_CancellationToken_InvokeThrowIfCancellationRequested()
        {
            // arrange
            var bulkAction = Mock.Of<IBulkAction>();
            var cancellationToken = Mock.Of<ICancellationToken>();
            var bulkActionValidationResult = Mock.Of<BulkActionResult>();
            var bulkActionFactory = Mock.Of<IBulkActionFactory>();
            var bulkActionProviderStorage = Mock.Of<IBulkActionProviderStorage>();
            var bulkActionExecutor = new BulkActionExecutor(bulkActionProviderStorage);
            var bulkActionProvider = Mock.Of<IBulkActionProvider>(t => t.BulkActionFactory == bulkActionFactory);

            var cancellationTokenMock = Mock.Get(cancellationToken);
            var bulkActionFactoryMock = Mock.Get(bulkActionFactory);
            var bulkActionProviderStorageMock = Mock.Get(bulkActionProviderStorage);
            var bulkActionMock = Mock.Get(bulkAction);

            bulkActionMock.Setup(t => t.ValidateAsync()).ReturnsAsync(bulkActionValidationResult);
            bulkActionFactoryMock.Setup(t => t.Create(It.IsAny<BulkActionContext>())).Returns(bulkAction);
            bulkActionProviderStorageMock.Setup(t => t.Get(It.IsAny<string>())).Returns(bulkActionProvider);

            // act
            await bulkActionExecutor.ExecuteAsync(Mock.Of<BulkActionContext>(), callback => { }, cancellationToken);

            // assert
            cancellationTokenMock.Verify(token => token.ThrowIfCancellationRequested(), () => Times.Exactly(1));
        }

        [Fact]
        public async Task Execute_NullableArgs_ThrowArgumentNullException()
        {
            // arrange
            var cancellationToken = Mock.Of<ICancellationToken>();
            var bulkActionProviderStorage = Mock.Of<IBulkActionProviderStorage>();
            var bulkActionExecutor = new BulkActionExecutor(bulkActionProviderStorage);

            // act
            await bulkActionExecutor.ExecuteAsync(Mock.Of<BulkActionContext>(), callback => { }, cancellationToken);
            var action = new Action(() => bulkActionExecutor.ExecuteAsync(null, null, null).GetAwaiter().GetResult());

            // assert
            action.Should().Throw<ArgumentNullException>();
        }
    }
}
