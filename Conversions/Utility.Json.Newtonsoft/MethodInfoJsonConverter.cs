using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;
using System;

namespace Utility.Conversions.Json.Newtonsoft
{

    public class MethodInfoJsonConverter : JsonConverter<MethodInfo>
    {

        public override void WriteJson(JsonWriter writer, MethodInfo value, JsonSerializer serializer)
        {

            // Begin object
            writer.WriteStartObject();

            writer.WritePropertyName("$type");
            writer.WriteValue("MethodInfo, System.Reflection");

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

        public override MethodInfo ReadJson(JsonReader reader, Type objectType, MethodInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
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
            var methodName = jObject["Name"]?.ToString();
            if (string.IsNullOrEmpty(methodName))
            {
                throw new JsonSerializationException("Method name is missing or invalid.");
            }

            var propertyInfo = type.GetMethod(methodName);
            if (propertyInfo == null)
            {
                throw new JsonSerializationException($"Method '{methodName}' not found on type '{type.FullName}'.");
            }

            return propertyInfo;
        }
    }
}