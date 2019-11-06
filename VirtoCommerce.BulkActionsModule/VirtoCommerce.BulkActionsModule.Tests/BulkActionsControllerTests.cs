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
        public void Cancel_EmptyString_NotContentResult()
        {
            // arrange

            // act
            var result = (StatusCodeResult)_controller.Cancel(string.Empty);

            // assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public void Cancel_EmptyString_StatusCodeResult()
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
        public void Cancel_CheckAttribute_ShouldBeMarkedAttribute(Type attributeType)
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
        public void GeRegisteredActions_CheckAttribute_ShouldBeMarkedAttribute(Type attributeType)
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
        public void GetActionData_CheckAttribute_ShouldBeMarkedAttribute(Type attributeType)
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
        public void GetActionData_NullArg_ThrowArgumentNullException()
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
        public void GetActionsData_BulkActionFactory_InvokeCreate()
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
        public void GetActionsData_BulkActionProviderStorage_InvokeGet()
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
        public void GetActionsData_BulkAction_InvokeGetActionData()
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
        public void GetActionsData_SecurityHandlerFactory_InvokeCreate()
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
        public void GetActionsData_AuthorizationFail_UnauthorizedResult()
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
        public void GetRegisteredActions_ShouldReturn_OkNegotiatedContentResult()
        {
            // arrange

            // act
            var result = _controller.GetRegisteredActions();

            // assert
            result.Should().BeOfType<OkNegotiatedContentResult<IBulkActionProvider[]>>();
        }

        [Fact]
        public void Run_BulkActionProviderStorage_InvokeGet()
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
        public void Run_CheckAttribute_ShouldBeMarkedAttribute(Type attributeType)
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
        public void Run_ShouldReturn_OkNegotiatedResult()
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
        public void Run_ShouldReturn_UnauthorizedResult()
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
        public void Run_UserNameResolver_InvokeGetCurrentUserName()
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