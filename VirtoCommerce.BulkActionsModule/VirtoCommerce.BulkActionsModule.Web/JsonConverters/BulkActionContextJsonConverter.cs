namespace VirtoCommerce.BulkActionsModule.Web.JsonConverters
{
    using System;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using VirtoCommerce.BulkActionsModule.Core.BulkActionModels;
    using VirtoCommerce.Platform.Core.Common;

    public class BulkActionContextJsonConverter : JsonConverter
    {
        private static readonly Type[] _knownTypes = { typeof(BulkActionContext) };

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return _knownTypes.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var typeName = objectType.Name;
            var contextTypeName = obj["contextTypeName"];
            if (contextTypeName != null)
            {
                typeName = contextTypeName.Value<string>();
            }

            var result = AbstractTypeFactory<BulkActionContext>.TryCreateInstance(typeName);
            if (result == null)
            {
                throw new NotSupportedException("Unknown BulkActionContext type: " + typeName);
            }

            serializer.Populate(obj.CreateReader(), result);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}