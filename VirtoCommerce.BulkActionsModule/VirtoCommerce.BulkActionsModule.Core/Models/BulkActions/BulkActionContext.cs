namespace VirtoCommerce.BulkActionsModule.Core.Models.BulkActions
{
    public abstract class BulkActionContext
    {
        /// <summary>
        /// Gets or sets the action name.
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// The context type name.
        /// </summary>
        public string ContextTypeName => GetType().Name;

        /// <summary>
        /// Gets or sets the data query.
        /// </summary>
        public DataQuery DataQuery { get; set; }
    }
}