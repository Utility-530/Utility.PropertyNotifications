using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using Utility.Helpers;
using Utility.Helpers.Reflection;

namespace Utility.Conversions.Json.Newtonsoft
{
    public class TypeConverter : JsonConverter<Type>
    {

        public override void WriteJson(JsonWriter writer, Type value, JsonSerializer serializer)
        {
            // Begin object
            writer.WriteStartObject();

            writer.WritePropertyName("$type");
            writer.WriteValue("System.Type");

            writer.WritePropertyName("Assembly");
            writer.WriteValue(value.Assembly.FullName);

            writer.WritePropertyName("Namespace");
            writer.WriteValue(value.Namespace);

            writer.WritePropertyName("Name");
            writer.WriteValue(value.Name);

            var str = value.GenericTypeArguments.Any() ? string.Join("|", value.GenericTypeArguments.Select(TypeHelper.AsString)) : null;

            if (str != null)
            {
                writer.WritePropertyName("GenericTypeNames");
                writer.WriteValue(str);
            }
            // End object
            writer.WriteEndObject();
        }

        public override Type ReadJson(JsonReader reader, Type objectType, Type existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            // Read the JSON object
            var jObject = JObject.Load(reader);

            var assembly = jObject["Assembly"]?.ToString();
            var nameSpace = jObject["Namespace"]?.ToString();
            var name = jObject["Name"]?.ToString();  
            var genericTypeNames = jObject["GenericTypeNames"]?.ToString();  


            string assemblyQualifiedName = Assembly.CreateQualifiedName(assembly, $"{nameSpace}.{name}");
            var baseType = System.Type.GetType(assemblyQualifiedName);
            var typeArguments = genericTypeNames?.Split('|').Select(a => System.Type.GetType(a)).ToArray();
            System.Type constructedType = typeArguments == null ? baseType : baseType.MakeGenericType(typeArguments);
            if (constructedType == null)
                throw new Exception($"Type, {assemblyQualifiedName},  does not exist");
            return constructedType;
        
        }
    }
}
