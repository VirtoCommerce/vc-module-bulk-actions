namespace VirtoCommerce.BulkActionsModule.Tests
{
    using System;

    using Moq;

    using VirtoCommerce.BulkActionsModule.Core;
    using VirtoCommerce.BulkActionsModule.Data.Services;

    using Xunit;

    public class BulkActionProviderStorageTests
    {
        [Fact]
        public void Add_ShouldReturn_NotNull()
        {
            // arrange
            var bulkActionProvider = Mock.Of<IBulkActionProvider>(t => t.Name == "test1");
            var actionProviderStorage = new BulkActionProviderStorage();

            // act
            var result = actionProviderStorage.Add(bulkActionProvider);

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Add_ShouldThrow_ArgumentNullException()
        {
            // arrange
            var bulkActionProvider = Mock.Of<IBulkActionProvider>();
            var actionProviderStorage = new BulkActionProviderStorage();

            // act
            var action = new Action(() => actionProviderStorage.Add(bulkActionProvider));

            // assert
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void Get_ShouldReturn_NotNull()
        {
            // arrange
            var actionProviderStorage = new BulkActionProviderStorage();
            actionProviderStorage.Add(Mock.Of<IBulkActionProvider>(t => t.Name == "test1"));

            // act
            var result = actionProviderStorage.Get("test1");

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Get_ShouldThrow_ArgumentException()
        {
            // arrange
            var actionProviderStorage = new BulkActionProviderStorage();

            // act
            var action = new Action(() => actionProviderStorage.Get("Unknown_action"));

            // assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void GetAll_ResultShouldBe_Single()
        {
            // arrange
            var actionProviderStorage = new BulkActionProviderStorage();
            actionProviderStorage.Add(Mock.Of<IBulkActionProvider>(t => t.Name == "test1"));

            // act
            var result = actionProviderStorage.GetAll();

            // assert
            Assert.Single(result);
        }

        [Fact]
        public void GetAll_ResultShouldBeType_IBulkActionProviderArray()
        {
            // arrange
            var actionProviderStorage = new BulkActionProviderStorage();

            // act
            var result = actionProviderStorage.GetAll();

            // assert
            Assert.IsType<IBulkActionProvider[]>(result);
        }
    }
}