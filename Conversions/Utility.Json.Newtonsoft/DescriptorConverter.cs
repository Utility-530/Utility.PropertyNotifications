using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Collections;
using Utility.Meta;
using System.Collections.ObjectModel;

namespace Utility.Conversions.Json.Newtonsoft
{
    public class DescriptorConverter : JsonConverter<PropertyDescriptor>
    {

        public override void WriteJson(JsonWriter writer, PropertyDescriptor value, JsonSerializer serializer)
        {
            // Begin object
            writer.WriteStartObject();

            writer.WritePropertyName("$type");
            writer.WriteValue("System.ComponentModel.PropertyDescriptor");

            writer.WritePropertyName("ComponentType");
            writer.WriteValue(value.ComponentType?.AssemblyQualifiedName);

            writer.WritePropertyName("PropertyType");
            writer.WriteValue(value.PropertyType?.AssemblyQualifiedName);

            writer.WritePropertyName("Name");
            writer.WriteValue(value.Name ?? string.Empty);

            // End object
            writer.WriteEndObject();
        }

        public override PropertyDescriptor ReadJson(JsonReader reader, Type objectType, PropertyDescriptor existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            // Read the JSON object
            var jObject = JObject.Load(reader);


            var propertyName = jObject["Name"]?.ToString();
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new JsonSerializationException("Property name is missing or invalid.");
            }

            var typeName = jObject["ComponentType"]?.ToString();
            if (string.IsNullOrEmpty(typeName))
            {
                var propertyTypeName = jObject["PropertyType"]?.ToString();
                var propertyType = Type.GetType(propertyTypeName);
                return new Utility.Meta.RootDescriptor(propertyType, name: propertyName);
            }

            if (Type.GetType(typeName) is Type type)
            {
                if (typeof(ICollection).IsAssignableFrom(type))
                {
                    var propertyTypeName = jObject["PropertyType"]?.ToString();
                    var propertyType = Type.GetType(propertyTypeName);
                    return new Utility.Meta.RootDescriptor(propertyType, name: propertyName);
                }

                if (TypeDescriptor.GetProperties(type)[propertyName] is PropertyDescriptor propertyDescriptor)
                    return propertyDescriptor;
                else
                    return new RootDescriptor(typeof(Collection<object>));
                    //throw new Exception($"{type}.{propertyName}");
            }
            else
                throw new JsonSerializationException($"Unable to resolve type: {typeName}");
        }
    }
}
