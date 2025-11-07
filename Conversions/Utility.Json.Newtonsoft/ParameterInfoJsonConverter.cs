using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Utility.Conversions.Json.Newtonsoft
{
    public class ParameterInfoJsonConverter : JsonConverter<ParameterInfo>
    {
        public override void WriteJson(JsonWriter writer, ParameterInfo value, JsonSerializer serializer)
        {
            // Begin object
            writer.WriteStartObject();

            writer.WritePropertyName("$type");
            writer.WriteValue("System.Reflection.ParameterInfo");

            // Write each property to JSON
            writer.WritePropertyName("TypeName");
            writer.WriteValue((value.Member as MethodInfo)?.DeclaringType?.AssemblyQualifiedName ?? string.Empty);

            // Write each property to JSON
            writer.WritePropertyName("MethodName");
            writer.WriteValue(value.Member.Name ?? string.Empty);

            bool isReturn = value.Name == null && (value.Position == -1 || value.IsRetval);
            writer.WritePropertyName("IsReturn");
            writer.WriteValue(isReturn);

            if (isReturn == false)
            {
                writer.WritePropertyName("ParameterName");
                writer.WriteValue(value.Name);
            }
            // End object
            writer.WriteEndObject();
        }

        public override ParameterInfo ReadJson(JsonReader reader, Type objectType, ParameterInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
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
            var methodName = jObject["MethodName"]?.ToString();
            if (string.IsNullOrEmpty(methodName))
            {
                throw new JsonSerializationException("Method name is missing or invalid.");
            }

            var method = type.GetMethods().FirstOrDefault(a => a.Name == methodName);

            if (jObject["IsReturn"].Value<bool>())
            {
                return method?.ReturnParameter;
            }
            else
            {
                // Try to get the property from the resolved type
                var parameterName = jObject["ParameterName"]?.ToString();
                if (string.IsNullOrEmpty(parameterName))
                {
                    throw new JsonSerializationException("Parameter name is missing or invalid.");
                }
                var parameters = method?.GetParameters();
                var parameterInfo = parameters.Single(a => a.Name == parameterName);
                if (parameterInfo == null)
                {
                    throw new JsonSerializationException($"Parameter '{parameterName}' not found on type '{type.FullName}' of method {methodName}.");
                }
                return parameterInfo;
            }
        }
    }
}