namespace VirtoCommerce.BulkActionsModule.Tests
{
    using System;
    using System.Net;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;

    using FluentAssertions;

    using Moq;

    using VirtoCommerce.BulkActionsModule.Core;
    using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
    using VirtoCommerce.BulkActionsModule.Core.Security;
    using VirtoCommerce.BulkActionsModule.Data.Security;
    using VirtoCommerce.BulkActionsModule.Tests.Models;
    using VirtoCommerce.BulkActionsModule.Web.BackgroundJobs;
    using VirtoCommerce.BulkActionsModule.Web.Controllers.Api;
    using VirtoCommerce.Platform.Core.Security;
    using VirtoCommerce.Platform.Core.Web.Security;

    using Xunit;

    public class BulkActionsControllerTests
    {
        private readonly Mock<IBackgroundJobExecutor> _backgroundExecutor;

        private readonly Mock<IBulkActionProviderStorage> _bulkActionProviderStorage;

        private readonly BulkActionsController _controller;

        private readonly Type _controllerType = typeof(BulkActionsController);

        private readonly Mock<ISecurityHandlerFactory> _securityHandlerFactory;

        private readonly Mock<IUserNameResolver> _userNameResolver;

        public BulkActionsControllerTests()
        {
            _bulkActionProviderStorage = new Mock<IBulkActionProviderStorage>();
            _userNameResolver = new Mock<IUserNameResolver>();
            _securityHandlerFactory = new Mock<ISecurityHandlerFactory>();
            _backgroundExecutor = new Mock<IBackgroundJobExecutor>();
            _controller = new BulkActionsController(
                _bulkActionProviderStorage.Object,
                _userNameResolver.Object,
                _securityHandlerFactory.Object,
                _backgroundExecutor.Object);
        }

        [Fact]
        public void Cancel_Should_Return_NotContent_Result()
        {
            // arrange

            // act
            var result = (StatusCodeResult)_controller.Cancel(string.Empty);

            // assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public void Cancel_Should_Return_Result_of_Type()
        {
            // arrange

            // act
            var result = _controller.Cancel(string.Empty);

            // assert
            result.Should().BeOfType<StatusCodeResult>();
        }

        [Theory]
        [InlineData(typeof(HttpDeleteAttribute))]
        [InlineData(typeof(CheckPermissionAttribute))]
        [InlineData(typeof(RouteAttribute))]
        public void Cancel_ShouldBeMarkedAttribute(Type attributeType)
        {
            // arrange
            var methodName = nameof(BulkActionsController.Cancel);
            var args = new Type[] { new TypeDelegator(typeof(string)) };
            var method = _controllerType.GetRuntimeMethod(methodName, args);

            // act
            var result = method.GetCustomAttribute(attributeType);

            // assert
            result.Should().NotBeNull();
        }

        [Theory]
        [InlineData(typeof(HttpGetAttribute))]
        [InlineData(typeof(CheckPermissionAttribute))]
        [InlineData(typeof(RouteAttribute))]
        [InlineData(typeof(ResponseTypeAttribute))]
        public void GeRegisteredActions_ShouldBeMarkedAttribute(Type attributeType)
        {
            // arrange
            var methodName = nameof(BulkActionsController.GetRegisteredActions);
            var args = new Type[] { };
            var method = _controllerType.GetRuntimeMethod(methodName, args);

            // act
            var result = method.GetCustomAttribute(attributeType);

            // assert
            result.Should().NotBeNull();
        }

        [Theory]
        [InlineData(typeof(HttpPostAttribute))]
        [InlineData(typeof(CheckPermissionAttribute))]
        [InlineData(typeof(RouteAttribute))]
        public void GetActionData_ShouldBeMarkedAttribute(Type attributeType)
        {
            // arrange
            var methodName = nameof(BulkActionsController.GetActionData);
            var args = new Type[] { new TypeDelegator(typeof(BulkActionContext)) };
            var method = _controllerType.GetRuntimeMethod(methodName, args);

            // act
            var result = method.GetCustomAttribute(attributeType);

            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void GetActionData_ShouldThrowArgumentNullException()
        {
            // arrange

            // act
            var action = new Action(
                () =>
                {
                    _controller.GetActionData(null);
                });

            // assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetActionsData_BulkActionFactoryShouldInvokeMethod()
        {
            // arrange
            var provider = BuildProvider();
            _bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);
            var mock = Mock.Get(provider.BulkActionFactory);

            // act
            _controller.GetActionData(Mock.Of<TestBulkActionContext>());

            // assert
            mock.Verify(t => t.Create(It.IsAny<BulkActionContext>()), Times.Exactly(1));
        }

        [Fact]
        public void GetActionsData_BulkActionProviderStorageShouldInvokeMethod()
        {
            // arrange
            var provider = BuildProvider();
            _bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);

            // act
            _controller.GetActionData(Mock.Of<TestBulkActionContext>());

            // assert
            _bulkActionProviderStorage.Verify(t => t.Get(It.IsAny<string>()), Times.Exactly(1));
        }

        [Fact]
        public void GetActionsData_BulkActionShouldInvokeMethod()
        {
            // arrange
            var provider = BuildProvider();
            _bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);
            var mock = Mock.Get(provider.BulkActionFactory);
            var bulkActionMock = Mock.Get(Mock.Of<IBulkAction>());
            bulkActionMock.Setup(t => t.GetActionData());
            mock.Setup(t => t.Create(It.IsAny<BulkActionContext>())).Returns(bulkActionMock.Object);

            // act
            _controller.GetActionData(Mock.Of<TestBulkActionContext>());

            // assert
            bulkActionMock.Verify(t => t.GetActionData(), Times.Exactly(1));
        }

        [Fact]
        public void GetActionsData_SecurityHandlerFactoryShouldInvokeMethod()
        {
            // arrange
            var provider = BuildProvider();
            provider.Permissions = new[] { "permission1" };
            _bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);
            _securityHandlerFactory.Setup(t => t.Create(provider.Permissions)).Returns(Mock.Of<ISecurityHandler>());

            // act
            _controller.GetActionData(Mock.Of<TestBulkActionContext>());

            // assert
            _securityHandlerFactory.Verify(t => t.Create(It.IsAny<string[]>()), Times.Exactly(1));
        }

        [Fact]
        public void GetActionsData_ShouldReturnUnauthorizedResult()
        {
            // arrange
            var provider = BuildProvider();
            provider.Permissions = new[] { "permission1" };
            _bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);
            _securityHandlerFactory.Setup(t => t.Create(provider.Permissions)).Returns(Mock.Of<ISecurityHandler>());

            // act
            var result = _controller.GetActionData(Mock.Of<TestBulkActionContext>());

            // assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void GetRegisteredActions_ShouldReturnOkNegotiatedContentResult()
        {
            // arrange

            // act
            var result = _controller.GetRegisteredActions();

            // assert
            result.Should().BeOfType<OkNegotiatedContentResult<IBulkActionProvider[]>>();
        }

        [Fact]
        public void Run_BulkActionProviderStorageShouldInvokeMethod()
        {
            // arrange
            var provider = BuildProvider();
            _bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);

            // act
            _controller.Run(Mock.Of<TestBulkActionContext>());

            // assert
            _bulkActionProviderStorage.Verify(t => t.Get(It.IsAny<string>()), Times.Exactly(1));
        }

        [Theory]
        [InlineData(typeof(HttpPostAttribute))]
        [InlineData(typeof(CheckPermissionAttribute))]
        [InlineData(typeof(RouteAttribute))]
        public void Run_ShouldBeMarkedAttribute(Type attributeType)
        {
            // arrange
            var methodName = nameof(BulkActionsController.Run);
            var args = new Type[] { new TypeDelegator(typeof(BulkActionContext)) };
            var method = _controllerType.GetRuntimeMethod(methodName, args);

            // act
            var result = method.GetCustomAttribute(attributeType);

            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Run_ShouldReturnOkNegotiatedResult()
        {
            // arrange
            var provider = BuildProvider();
            _bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);
            _securityHandlerFactory.Setup(t => t.Create(provider.Permissions)).Returns(Mock.Of<ISecurityHandler>());

            // act
            var result = _controller.Run(Mock.Of<TestBulkActionContext>());

            // assert
            result.Should().BeOfType<OkNegotiatedContentResult<BulkActionPushNotification>>();
        }

        [Fact]
        public void Run_ShouldReturnUnauthorizedResult()
        {
            // arrange
            var provider = BuildProvider();
            provider.Permissions = new[] { "permission1" };
            _bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);
            _securityHandlerFactory.Setup(t => t.Create(provider.Permissions)).Returns(Mock.Of<ISecurityHandler>());

            // act
            var result = _controller.Run(Mock.Of<TestBulkActionContext>());

            // assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void Run_UserNameResolverShouldInvokeMethod()
        {
            // arrange
            var provider = BuildProvider();
            _bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);
            _securityHandlerFactory.Setup(t => t.Create(provider.Permissions)).Returns(Mock.Of<ISecurityHandler>());
            _userNameResolver.Setup(t => t.GetCurrentUserName());

            // act
            _controller.Run(Mock.Of<TestBulkActionContext>());

            // assert
            _userNameResolver.Verify(t => t.GetCurrentUserName(), Times.Once);
        }

        private static IBulkActionProvider BuildProvider()
        {
            var bulkActionFactory = Mock.Of<IBulkActionFactory>(MockBehavior.Loose);
            Mock.Get(bulkActionFactory).Setup(t => t.Create(It.IsAny<BulkActionContext>()))
                .Returns(Mock.Of<IBulkAction>());

            var provider = Mock.Of<TestBulkActionProvider>(
                t => t.BulkActionFactory == bulkActionFactory,
                MockBehavior.Loose);

            return provider;
        }
    }
}