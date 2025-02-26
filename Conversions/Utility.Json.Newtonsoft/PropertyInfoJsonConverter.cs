using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;
using System;

namespace Utility.Conversions.Json.Newtonsoft
{
    public class NonGenericPropertyInfoJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PropertyInfo);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return PropertyInfoJsonConverter.Read(reader);

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            PropertyInfoJsonConverter.Write(writer, (PropertyInfo)value);
        }
    }

    public class PropertyInfoJsonConverter : JsonConverter<PropertyInfo>
    {

        public override void WriteJson(JsonWriter writer, PropertyInfo value, JsonSerializer serializer)
        {
            // Begin object
            Write(writer, value);
        }

        public static void Write(JsonWriter writer, PropertyInfo value)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("$type");
            writer.WriteValue("System.Reflection.PropertyInfo");

            writer.WritePropertyName("Name");
            writer.WriteValue(value.Name);

            // Write each property to JSON
            writer.WritePropertyName("TypeName");
            writer.WriteValue(value.DeclaringType?.AssemblyQualifiedName ?? string.Empty);

            // End object
            writer.WriteEndObject();
        }

        //public override PropertyInfo? ReadJson(JsonReader reader, Type objectType, PropertyInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
        //{
        //    if (reader.Value == null)
        //        return null;
        //    // Read the JSON object
        //    var jObject = JObject.Load(reader);
        //    return Type.GetType(jObject["TypeName"].ToString()).GetProperty(jObject["Name"].ToString());
        //}

        public override PropertyInfo ReadJson(JsonReader reader, Type objectType, PropertyInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return Read(reader);
        }

        public static PropertyInfo Read(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            // Read the JSON object
            var jObject = JObject.Load(reader);

            // Try to resolve the type from TypeName
            var typeName = jObject["TypeName"]?.ToString();
            if (string.IsNullOrEmpty(typeName))
            {
                throw new JsonSerializationException("TypeName is missing or invalid.");
            }

            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new JsonSerializationException($"Unable to resolve type: {typeName}");
            }

            // Try to get the property from the resolved type
            var propertyName = jObject["Name"]?.ToString();
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new JsonSerializationException("Property name is missing or invalid.");
            }

            var propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new JsonSerializationException($"Property '{propertyName}' not found on type '{type.FullName}'.");
            }

            return propertyInfo;
        }
    }
}