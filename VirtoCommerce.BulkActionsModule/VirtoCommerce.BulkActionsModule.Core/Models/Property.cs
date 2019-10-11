namespace VirtoCommerce.BulkActionsModule.Core.Models
{
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using VirtoCommerce.Domain.Catalog.Model;
    using VirtoCommerce.Platform.Core.Common;

    /// <summary>
    /// Property is meta information record about what additional information merchandising item can be characterized.
    /// It's not inheritable and can be defined in catalog, category, product or variation level.
    /// </summary>
    public class Property : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        public Property()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="propValue">
        /// The prop value.
        /// </param>
        /// <param name="catalogId">
        /// The catalog id.
        /// </param>
        /// <param name="propertyType">
        /// The property type.
        /// </param>
        public Property(PropertyValue propValue, string catalogId, PropertyType propertyType)
        {
            Id = propValue.Id;
            CatalogId = catalogId;
            IsManageable = false;
            Multilanguage = !string.IsNullOrEmpty(propValue.LanguageCode);
            Multivalue = propValue.PropertyMultivalue;
            Name = propValue.PropertyName;
            Type = propertyType;
            ValueType = propValue.ValueType;
            Values = new List<PropertyValue>();
        }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public ICollection<PropertyAttribute> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the catalog id that this product belongs to.
        /// </summary>
        /// <value>
        /// The catalog identifier.
        /// </value>
        public string CatalogId { get; set; }

        /// <summary>
        /// Gets or sets the category id that this product belongs to.
        /// </summary>
        /// <value>
        /// The category identifier.
        /// </value>
        public string CategoryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Property"/> is dictionary.
        /// </summary>
        /// <value>
        ///   <c>true</c> if dictionary; otherwise, <c>false</c>.
        /// </value>
        public bool Dictionary { get; set; }

        /// <summary>
        /// Gets or sets the display names.
        /// </summary>
        /// <value>
        /// The display names.
        /// </value>
        public ICollection<PropertyDisplayName> DisplayNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Property"/> is hidden.
        /// </summary>
        /// <value>
        ///   <c>true</c> if hidden; otherwise, <c>false</c>.
        /// </value>
        public bool Hidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is inherited.
        /// </summary>
        public bool IsInherited { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user can change property metadata or remove this property. 
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is manageable; otherwise, <c>false</c>.
        /// </value>
        public bool IsManageable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is new. A new property should be created on server site instead of trying to update it.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is new; otherwise, <c>false</c>.
        /// </value>
        public bool IsNew { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user can change property value.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Property"/> is multilingual.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multilanguage; otherwise, <c>false</c>.
        /// </value>
        public bool Multilanguage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Property"/> supports multiple values.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multivalue; otherwise, <c>false</c>.
        /// </value>
        public bool Multivalue { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Property"/> is required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if required; otherwise, <c>false</c>.
        /// </value>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the type of object this property is applied to.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public PropertyType Type { get; set; }

        /// <summary>
        /// Gets or sets validation rules
        /// </summary>
        public PropertyValidationRule ValidationRule { get; set; }

        /// <summary>
        /// Gets or sets the current property value. Collection is used as a general placeholder to store both single and multi-value values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public ICollection<PropertyValue> Values { get; set; }

        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        /// <value>
        /// The type of the value.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public PropertyValueType ValueType { get; set; }
    }
}