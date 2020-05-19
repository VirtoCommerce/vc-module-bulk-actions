using VirtoCommerce.BulkActionsModule.Core.Services;

namespace VirtoCommerce.BulkActionsModule.Core.Models.BulkActions
{
    using Newtonsoft.Json;

    public class BulkActionProvider : IBulkActionProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkActionProvider"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="contextTypeName">
        /// The context type name.
        /// </param>
        /// <param name="applicableTypes">
        /// The applicable types.
        /// </param>
        /// <param name="dataSourceFactory">
        /// The data source factory.
        /// </param>
        /// <param name="bulkActionFactory">
        /// The bulk action factory.
        /// </param>
        /// <param name="permissions">
        /// The required Permissions.
        /// </param>
        public BulkActionProvider(
            string name,
            string contextTypeName,
            string[] applicableTypes,
            IDataSourceFactory dataSourceFactory,
            IBulkActionFactory bulkActionFactory,
            string[] permissions)
        {
            Name = name;
            ContextTypeName = contextTypeName;
            ApplicableTypes = applicableTypes;
            DataSourceFactory = dataSourceFactory;
            BulkActionFactory = bulkActionFactory;
            Permissions = permissions;
        }

        /// <summary>
        /// Gets or sets the entity types to which action could be applied: Category, Product, ….
        /// </summary>
        public string[] ApplicableTypes { get; set; }

        /// <summary>
        /// Gets or sets the factory.
        /// </summary>
        [JsonIgnore]
        public IBulkActionFactory BulkActionFactory { get; set; }

        /// <summary>
        /// Gets or sets the context type name.
        /// </summary>
        public string ContextTypeName { get; set; }

        /// <summary>
        /// Gets or sets the data source factory.
        /// </summary>
        [JsonIgnore]
        public IDataSourceFactory DataSourceFactory { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the required permissions.
        /// </summary>
        public string[] Permissions { get; set; }
    }
}
