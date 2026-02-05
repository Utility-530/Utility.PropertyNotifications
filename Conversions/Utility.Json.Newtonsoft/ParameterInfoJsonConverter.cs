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

            writer.WritePropertyName("Position");
            writer.WriteValue(value.Position);

            if (value.Name == null && (value.Position == -1 || value.IsRetval))
            {
                // return parameter
            }
            else
            {
                writer.WritePropertyName("ParameterName");
                writer.WriteValue(value.Name);

                writer.WritePropertyName("Types");
                if (value.Member is MethodInfo method)
                {
                    writer.WriteStartArray();

                    foreach (var type in method.GetParameters().Select(p => p.ParameterType))
                    {
                        writer.WriteValue(type.AssemblyQualifiedName);
                    }

                    writer.WriteEndArray();
                }
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
            var parameterName = jObject["ParameterName"]?.ToString();
            int? position = jObject["Position"]?.ToString() is string pos ? int.Parse(pos) : null;
            var types = jObject["Types"]?.Select(a => Type.GetType(a.ToString())).ToArray();

            var methods = type.GetMethods().Where(a => a.Name == methodName).ToArray();

            foreach (var method in methods)
            {
                if (jObject["Position"].Value<int>() == -1)
                {
                    return method?.ReturnParameter;
                }
                else
                {
                    // Try to get the property from the resolved type
                    if (string.IsNullOrEmpty(parameterName))
                    {
                        throw new JsonSerializationException("Parameter name is missing or invalid.");
                    }
                    var parameters = method?.GetParameters();
                    var parameterInfo = parameters.SingleOrDefault(a =>
                    a.Name == parameterName &&
                    (position.HasValue ? a.Position == position : true) &&
                    (types != null ? method.GetParameters().Select(a => a.ParameterType).SequenceEqual(types) : true));
                    if (parameterInfo != null)
                    {
                        return parameterInfo;
                    }

                }
            }
            throw new JsonSerializationException($"Parameter '{parameterName}' not found on type '{type.FullName}' of method {methodName}.");
        }
    }
}