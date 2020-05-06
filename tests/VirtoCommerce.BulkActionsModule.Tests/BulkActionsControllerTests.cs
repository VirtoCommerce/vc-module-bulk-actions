using System;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
using VirtoCommerce.BulkActionsModule.Core.Services;
using VirtoCommerce.BulkActionsModule.Data.Authorization;
using VirtoCommerce.BulkActionsModule.Tests.Models;
using VirtoCommerce.BulkActionsModule.Web.BackgroundJobs;
using VirtoCommerce.BulkActionsModule.Web.Controllers.Api;
using VirtoCommerce.Platform.Core.Security;
using Xunit;

namespace VirtoCommerce.BulkActionsModule.Tests
{
    public class BulkActionsControllerTests
    {
        private readonly Type _controllerType = typeof(BulkActionsController);

        [Fact]
        public void Cancel_EmptyString_NotContentResult()
        {
            // arrange
            var controller = BuildController();

            // act
            var result = (StatusCodeResult)controller.Cancel(string.Empty);

            // assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Fact]
        public void Cancel_EmptyString_StatusCodeResult()
        {
            // arrange
            var controller = BuildController();

            // act
            var result = controller.Cancel(string.Empty);

            // assert
            result.Should().BeOfType<StatusCodeResult>();
        }

        [Theory]
        [InlineData(typeof(HttpDeleteAttribute))]
        [InlineData(typeof(AuthorizeAttribute))]
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
        [InlineData(typeof(AuthorizeAttribute))]
        [InlineData(typeof(RouteAttribute))]
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
        [InlineData(typeof(AuthorizeAttribute))]
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
            var controller = BuildController();

            // act
            var action = new Action(
                () =>
                {
                    controller.GetActionData(null);
                });

            // assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetActionsData_BulkActionFactory_InvokeCreate()
        {
            // arrange
            var bulkActionProviderStorage = new Mock<IBulkActionProviderStorage>();
            var controller = BuildController(bulkActionProviderStorage);
            var provider = BuildProvider();
            bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);
            var mock = Mock.Get(provider.BulkActionFactory);

            // act
            controller.GetActionData(Mock.Of<TestBulkActionContext>());

            // assert
            mock.Verify(t => t.Create(It.IsAny<BulkActionContext>()), Times.Exactly(1));
        }

        [Fact]
        public void GetActionsData_BulkActionProviderStorage_InvokeGet()
        {
            // arrange
            var bulkActionProviderStorage = new Mock<IBulkActionProviderStorage>();
            var controller = BuildController(bulkActionProviderStorage);
            var provider = BuildProvider();
            bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);

            // act
            controller.GetActionData(Mock.Of<TestBulkActionContext>());

            // assert
            bulkActionProviderStorage.Verify(t => t.Get(It.IsAny<string>()), Times.Exactly(1));
        }

        [Fact]
        public void GetActionsData_BulkAction_InvokeGetActionData()
        {
            // arrange
            var bulkActionProviderStorage = new Mock<IBulkActionProviderStorage>();
            var controller = BuildController(bulkActionProviderStorage);
            var provider = BuildProvider();
            bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);
            var mock = Mock.Get(provider.BulkActionFactory);
            var bulkActionMock = Mock.Get(Mock.Of<IBulkAction>());
            bulkActionMock.Setup(t => t.GetActionData());
            mock.Setup(t => t.Create(It.IsAny<BulkActionContext>())).Returns(bulkActionMock.Object);

            // act
            controller.GetActionData(Mock.Of<TestBulkActionContext>());

            // assert
            bulkActionMock.Verify(t => t.GetActionData(), Times.Exactly(1));
        }

        [Fact]
        public void GetActionsData_authorizationService_InvokeCreate()
        {
            // arrange
            var bulkActionProviderStorage = new Mock<IBulkActionProviderStorage>();
            var authorizationService = new Mock<IAuthorizationService>();
            var controller = BuildController(bulkActionProviderStorage, authorizationService);
            var provider = BuildProvider();
            provider.Permissions = new[] { "permission1" };
            bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);
            authorizationService.Setup(t =>
                    t.AuthorizeAsync(ClaimsPrincipal.Current, It.IsAny<object>(), new BulkActionsAuthorizationRequirement("permission1")))
                .ReturnsAsync(Mock.Of<AuthorizationResult>());

            // act
            controller.GetActionData(Mock.Of<TestBulkActionContext>());

            // assert
            authorizationService.Verify(t =>
                t.AuthorizeAsync(ClaimsPrincipal.Current, It.IsAny<object>(), new BulkActionsAuthorizationRequirement(It.IsAny<string>())), Times.Exactly(1));
        }

        [Fact]
        public void GetActionsData_AuthorizationFail_UnauthorizedResult()
        {
            // arrange
            var bulkActionProviderStorage = new Mock<IBulkActionProviderStorage>();
            var authorizationService = new Mock<IAuthorizationService>();
            var controller = BuildController(bulkActionProviderStorage, authorizationService);
            var provider = BuildProvider();
            provider.Permissions = new[] { "permission1" };
            bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);
            authorizationService.Setup(t =>
                    t.AuthorizeAsync(ClaimsPrincipal.Current, It.IsAny<object>(), new BulkActionsAuthorizationRequirement("permission1")))
                .ReturnsAsync(Mock.Of<AuthorizationResult>());

            // act
            var result = controller.GetActionData(Mock.Of<TestBulkActionContext>());

            // assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void GetRegisteredActions_ShouldReturn_OkNegotiatedContentResult()
        {
            // arrange
            var controller = BuildController();

            // act
            var result = controller.GetRegisteredActions();

            // assert
            result.Should().BeOfType<ActionResult<IBulkActionProvider[]>>();
        }

        [Fact]
        public void Run_BulkActionProviderStorage_InvokeGet()
        {
            // arrange
            var bulkActionProviderStorage = new Mock<IBulkActionProviderStorage>();
            var controller = BuildController(bulkActionProviderStorage);
            var provider = BuildProvider();
            bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);

            // act
            controller.Run(Mock.Of<TestBulkActionContext>());

            // assert
            bulkActionProviderStorage.Verify(t => t.Get(It.IsAny<string>()), Times.Exactly(1));
        }

        [Theory]
        [InlineData(typeof(HttpPostAttribute))]
        [InlineData(typeof(AuthorizeAttribute))]
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
            var bulkActionProviderStorage = new Mock<IBulkActionProviderStorage>();
            var authorizationService = new Mock<IAuthorizationService>();
            var controller = BuildController(bulkActionProviderStorage, authorizationService);
            var provider = BuildProvider();
            bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);
            authorizationService.Setup(t =>
                    t.AuthorizeAsync(ClaimsPrincipal.Current, It.IsAny<object>(), new BulkActionsAuthorizationRequirement("")))
                .ReturnsAsync(Mock.Of<AuthorizationResult>());

            // act
            var result = controller.Run(Mock.Of<TestBulkActionContext>());

            // assert
            result.Should().BeOfType<ActionResult<BulkActionPushNotification>>();
        }

        [Fact]
        public void Run_ShouldReturn_UnauthorizedResult()
        {
            // arrange
            var bulkActionProviderStorage = new Mock<IBulkActionProviderStorage>();
            var authorizationService = new Mock<IAuthorizationService>();
            var controller = BuildController(bulkActionProviderStorage, authorizationService);
            var provider = BuildProvider();
            provider.Permissions = new[] { "permission1" };
            bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);
            authorizationService.Setup(t =>
                    t.AuthorizeAsync(ClaimsPrincipal.Current, It.IsAny<object>(), new BulkActionsAuthorizationRequirement("permission1")))
                .ReturnsAsync(Mock.Of<AuthorizationResult>());

            // act
            var result = controller.Run(Mock.Of<TestBulkActionContext>());

            // assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void Run_UserNameResolver_InvokeGetCurrentUserName()
        {
            // arrange
            var bulkActionProviderStorage = new Mock<IBulkActionProviderStorage>();
            var authorizationService = new Mock<IAuthorizationService>();
            var userNameResolver = new Mock<IUserNameResolver>();
            var controller = BuildController(bulkActionProviderStorage, authorizationService, userNameResolver);
            var provider = BuildProvider();
            bulkActionProviderStorage.Setup(t => t.Get(It.IsAny<string>())).Returns(provider);
            authorizationService.Setup(t =>
                    t.AuthorizeAsync(ClaimsPrincipal.Current, It.IsAny<object>(), new BulkActionsAuthorizationRequirement("permission1")))
                .ReturnsAsync(Mock.Of<AuthorizationResult>());
            userNameResolver.Setup(t => t.GetCurrentUserName());

            // act
            controller.Run(Mock.Of<TestBulkActionContext>());

            // assert
            userNameResolver.Verify(t => t.GetCurrentUserName(), Times.Once);
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

        private static BulkActionsController BuildController()
        {
            var bulkActionProviderStorage = new Mock<IBulkActionProviderStorage>();
            var userNameResolver = new Mock<IUserNameResolver>();
            var authorizationService = new Mock<IAuthorizationService>();
            var backgroundExecutor = new Mock<IBackgroundJobExecutor>();
            var controller = new BulkActionsController(
                bulkActionProviderStorage.Object,
                userNameResolver.Object,
                backgroundExecutor.Object,
                authorizationService.Object);

            return controller;
        }

        private static BulkActionsController BuildController(IMock<IBulkActionProviderStorage> bulkActionProviderStorage)
        {
            var userNameResolver = new Mock<IUserNameResolver>();
            var authorizationService = new Mock<IAuthorizationService>();
            var backgroundExecutor = new Mock<IBackgroundJobExecutor>();
            var controller = new BulkActionsController(
                bulkActionProviderStorage.Object,
                userNameResolver.Object,
                backgroundExecutor.Object,
                authorizationService.Object);

            return controller;
        }

        private static BulkActionsController BuildController(
            IMock<IBulkActionProviderStorage> bulkActionProviderStorage,
            IMock<IAuthorizationService> authorizationService)
        {
            var userNameResolver = new Mock<IUserNameResolver>();
            var backgroundExecutor = new Mock<IBackgroundJobExecutor>();
            var controller = new BulkActionsController(
                bulkActionProviderStorage.Object,
                userNameResolver.Object,
                backgroundExecutor.Object,
                authorizationService.Object);

            return controller;
        }

        private static BulkActionsController BuildController(
            IMock<IBulkActionProviderStorage> bulkActionProviderStorage,
            IMock<IAuthorizationService> authorizationService,
            IMock<IUserNameResolver> userNameResolver)
        {
            var backgroundExecutor = new Mock<IBackgroundJobExecutor>();
            var controller = new BulkActionsController(
                bulkActionProviderStorage.Object,
                userNameResolver.Object,
                backgroundExecutor.Object,
                authorizationService.Object);

            return controller;
        }
    }
}
