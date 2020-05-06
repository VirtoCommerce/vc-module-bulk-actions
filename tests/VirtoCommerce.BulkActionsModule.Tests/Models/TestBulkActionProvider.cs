namespace VirtoCommerce.BulkActionsModule.Tests.Models
{
    using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;

    public class TestBulkActionProvider : BulkActionProvider
    {
        public TestBulkActionProvider()
            : base(string.Empty, string.Empty, new[] { "test" }, null, null, null)
        {
        }
    }
}