using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utility.Helpers;

namespace Utility.Conversions.Json.Newtonsoft
{
    public class MetadataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType is not Type && Utility.Helpers.PropertyHelper.ValueTypes.Contains(objectType) == false && objectType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(objectType) == false;
        }


        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            var type = value.GetType();

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);


            writer.WritePropertyName("$type");
            writer.WriteValue(type.AsString());
            List<string> names = new List<string>();
            List<string> eNames = new List<string>();

            foreach (var prop in properties)
            {
                var propName = prop.Name;
                var propValue = prop.GetValue(value);

                writer.WritePropertyName(propName);
                if(prop.PropertyType.IsEnum)
                {
                    serializer.Serialize(writer, propValue);
                    eNames.Add(prop.Name);
                }
                else if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                    writer.WriteValue(propValue);
                else
                    serializer.Serialize(writer, propValue);


                // Write metadata about whether the property is readonly (no setter or private setter)
                bool isReadOnly = !prop.CanWrite || prop.SetMethod == null || !prop.SetMethod.IsPublic;
                if (isReadOnly)
                {
                    names.Add(prop.Name);
                }
            }

            writer.WritePropertyName($"$isreadonly");
            serializer.Serialize(writer, names);        
            
            writer.WritePropertyName($"$isenum");
            serializer.Serialize(writer, eNames);

            writer.WriteEndObject();
        }
    }
}