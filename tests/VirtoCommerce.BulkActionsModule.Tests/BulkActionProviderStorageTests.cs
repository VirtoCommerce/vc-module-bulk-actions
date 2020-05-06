namespace VirtoCommerce.BulkActionsModule.Tests
{
    using System;

    using FluentAssertions;

    using Moq;

    using VirtoCommerce.BulkActionsModule.Core;
    using VirtoCommerce.BulkActionsModule.Data.Services;

    using Xunit;

    public class BulkActionProviderStorageTests
    {
        [Fact]
        public void Add_TryAdd_NotNull()
        {
            // arrange
            var bulkActionProvider = Mock.Of<IBulkActionProvider>(t => t.Name == "test1");
            var actionProviderStorage = new BulkActionProviderStorage();

            // act
            var result = actionProviderStorage.Add(bulkActionProvider);

            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Add_EmptyProviderName_ThrowArgumentNullException()
        {
            // arrange
            var bulkActionProvider = Mock.Of<IBulkActionProvider>();
            var actionProviderStorage = new BulkActionProviderStorage();

            // act
            var action = new Action(() => actionProviderStorage.Add(bulkActionProvider));

            // assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Add_IfContainsKey_ThrowArgumentException()
        {
            // arrange
            var bulkActionProvider = Mock.Of<IBulkActionProvider>(t => t.Name == "test");
            var actionProviderStorage = new BulkActionProviderStorage();
            actionProviderStorage.Add(bulkActionProvider);

            // act
            var action = new Action(() => actionProviderStorage.Add(bulkActionProvider));

            // assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Get_IfExists_NotNull()
        {
            // arrange
            var actionProviderStorage = new BulkActionProviderStorage();
            actionProviderStorage.Add(Mock.Of<IBulkActionProvider>(t => t.Name == "test1"));

            // act
            var result = actionProviderStorage.Get("test1");

            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Get_IfNotExists_ThrowArgumentException()
        {
            // arrange
            var actionProviderStorage = new BulkActionProviderStorage();

            // act
            var action = new Action(() => actionProviderStorage.Get("Unknown_action"));

            // assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetAll_HasSomeItems_ShouldBeSingle()
        {
            // arrange
            var actionProviderStorage = new BulkActionProviderStorage();
            actionProviderStorage.Add(Mock.Of<IBulkActionProvider>(t => t.Name == "test1"));

            // act
            var result = actionProviderStorage.GetAll();

            // assert
            result.Should().ContainSingle();
        }

        [Fact]
        public void GetAll_ResultType_IBulkActionProviderArray()
        {
            // arrange
            var actionProviderStorage = new BulkActionProviderStorage();

            // act
            var result = actionProviderStorage.GetAll();

            // assert
            result.Should().BeOfType<IBulkActionProvider[]>();
        }
    }
}