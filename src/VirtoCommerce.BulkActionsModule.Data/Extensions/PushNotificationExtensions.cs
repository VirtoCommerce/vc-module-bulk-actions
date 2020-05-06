using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;

namespace VirtoCommerce.BulkActionsModule.Data.Extensions
{
    public static class PushNotificationExtensions
    {
        public static void Patch(this BulkActionPushNotification target, BulkActionProgressContext source)
        {
            target.Description = source.Description;
            target.Errors = source.Errors;
            target.ProcessedCount = source.ProcessedCount;
            target.TotalCount = source.TotalCount;
        }
    }
}
