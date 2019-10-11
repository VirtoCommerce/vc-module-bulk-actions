﻿namespace VirtoCommerce.BulkActionsModule.Core.BulkActionModels
{
    using Newtonsoft.Json;

    using VirtoCommerce.BulkActionsModule.Core.BulkActionAbstractions;
    using VirtoCommerce.BulkActionsModule.Core.DataSourceAbstractions;

    public class BulkActionDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkActionDefinition"/> class.
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
        public BulkActionDefinition(
            string name,
            string contextTypeName,
            string[] applicableTypes,
            IPagedDataSourceFactory dataSourceFactory,
            IBulkActionFactory bulkActionFactory)
        {
            Name = name;
            ContextTypeName = contextTypeName;
            ApplicableTypes = applicableTypes;
            DataSourceFactory = dataSourceFactory;
            BulkActionFactory = bulkActionFactory;
        }

        /// <summary>
        /// Gets or sets the entity types to which action could be applied: Category, Product, … 
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
        public IPagedDataSourceFactory DataSourceFactory { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }
}