namespace VirtoCommerce.BulkActionsModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Execute = "bulk-actions:execute";
                public const string Read = "bulk-actions:read";

                public static string[] AllPermissions { get; } = new[] { Read, Execute };
            }
        }
    }
}
