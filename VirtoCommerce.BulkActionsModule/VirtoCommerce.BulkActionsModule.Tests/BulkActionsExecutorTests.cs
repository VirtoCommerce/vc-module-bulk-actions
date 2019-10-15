namespace VirtoCommerce.BulkActionsModule.Tests
{
    using System;
    using System.Collections.Generic;

    using Moq;

    using VirtoCommerce.BulkActionsModule.Core;
    using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
    using VirtoCommerce.BulkActionsModule.Data.Services;
    using VirtoCommerce.Platform.Core.Common;

    using Xunit;

    public class BulkActionsExecutorTests
    {
        [Fact]
        public void Execute_ShouldInvokeMethod_Fetch()
        {
            // arrange
            var bulkAction = Mock.Of<IBulkAction>();
            var cancellationToken = Mock.Of<ICancellationToken>();
            var bulkActionProviderStorage = Mock.Of<IBulkActionProviderStorage>();
            var pagedDataSource = Mock.Of<IPagedDataSource>();
            var bulkActionProvider = Mock.Of<IBulkActionProvider>();
            var bulkActionValidationResult = Mock.Of<BulkActionResult>(t => t.Succeeded == true);
            var bulkActionResult = Mock.Of<BulkActionResult>(t => t.Succeeded == true);

            var bulkActionProviderStorageMock = Mock.Get(bulkActionProviderStorage);
            var bulkActionMock = Mock.Get(bulkAction);
            var bulkActionProviderMock = Mock.Get(bulkActionProvider);
            var pagedDataSourceMock = Mock.Get(pagedDataSource);

            bulkActionMock.Setup(t => t.Validate()).Returns(bulkActionValidationResult);
            bulkActionMock.Setup(t => t.Execute(It.IsAny<IEnumerable<IEntity>>())).Returns(bulkActionResult);
            bulkActionProviderMock.Setup(t => t.BulkActionFactory.Create(It.IsAny<BulkActionContext>())).Returns(bulkAction);
            bulkActionProviderMock.Setup(t => t.DataSourceFactory.Create(It.IsAny<BulkActionContext>())).Returns(pagedDataSource);
            bulkActionProviderStorageMock.Setup(t => t.Get(It.IsAny<string>())).Returns(bulkActionProvider);
            pagedDataSourceMock.SetupSequence(t => t.Fetch()).Returns(true).Returns(false);

            // act
            var bulkActionExecutor = new BulkActionExecutor(bulkActionProviderStorageMock.Object);
            bulkActionExecutor.Execute(Mock.Of<BulkActionContext>(), callback => { }, cancellationToken);

            // assert
            pagedDataSourceMock.Verify(t => t.Fetch(), Times.Exactly(2));
        }

        [Fact]
        public void Execute_ShouldInvokeMethod_ThrowIfCancellationRequested()
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

            bulkActionMock.Setup(t => t.Validate()).Returns(bulkActionValidationResult);
            bulkActionFactoryMock.Setup(t => t.Create(It.IsAny<BulkActionContext>())).Returns(bulkAction);
            bulkActionProviderStorageMock.Setup(t => t.Get(It.IsAny<string>())).Returns(bulkActionProvider);

            // act
            bulkActionExecutor.Execute(Mock.Of<BulkActionContext>(), callback => { }, cancellationToken);

            // assert
            cancellationTokenMock.Verify(token => token.ThrowIfCancellationRequested(), () => Times.Exactly(2));
        }

        [Fact]
        public void Execute_ShouldThrow_ArgumentNullException()
        {
            // arrange
            var cancellationToken = Mock.Of<ICancellationToken>();
            var bulkActionProviderStorage = Mock.Of<IBulkActionProviderStorage>();
            var bulkActionExecutor = new BulkActionExecutor(bulkActionProviderStorage);

            // act
            bulkActionExecutor.Execute(Mock.Of<BulkActionContext>(), callback => { }, cancellationToken);
            var action = new Action(() => bulkActionExecutor.Execute(null, null, null));

            // assert
            Assert.Throws<ArgumentNullException>(action);
        }
    }
}