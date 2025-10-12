using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Utility.Attributes;
using Utility.Helpers;
using Utility.Helpers.Reflection;

namespace Utility.Conversions.Json.Newtonsoft
{
    public class MetadataConverter : JsonConverter
    {

        public const string IsEnum = "$isenum";
        public const string IsReadonly = "$isreadonly";
        public const string Type = "$type";

        public override bool CanConvert(Type objectType)
        {
            return
                objectType != typeof(Type) &&
                !objectType.Name.Contains("Runtime") &&
                !objectType.Name.Contains("Method") &&
                PropertyHelper.ValueTypes.Contains(objectType) == false &&
                objectType != typeof(string) &&
                typeof(IEnumerable).IsAssignableFrom(objectType) == false;
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

            writer.WritePropertyName(Type);
            writer.WriteValue(type.AsString());
            List<string> list = new();
            foreach (var prop in properties)
            {
                StringBuilder stringBuilder = new();
                var x = prop.GetCustomAttributes();
                if (x.Any(a => a is JsonIgnoreAttribute or System.Text.Json.Serialization.JsonIgnoreAttribute))
                {
                    continue;
                }
                if (prop.CanRead == false)
                {
                    continue;
                }
                var propName = prop.Name;
                stringBuilder.Append(propName);
                object propValue = null;
                try
                {
                    propValue = prop.GetValue(value);

                }
                catch (Exception ex)
                {
                    propValue = ex.Message;
                }

                // Write metadata about whether the property is readonly (no setter or private setter)
                bool isReadOnly = !prop.CanWrite || prop.SetMethod == null || !prop.SetMethod.IsPublic || x.Any(a => a is ReadOnlyAttribute { IsReadOnly: true });
                if (isReadOnly)
                {
                    stringBuilder.AppendLine(IsReadonly);
                }
        
                if (prop.PropertyType == typeof(IntPtr))
                {
                }
                else if (x.OfType<DataTypeAttribute>().SingleOrDefault() is { DataType: { } dataType })
                {
                    stringBuilder.AppendLine($"{Type}:{dataType}");
                    writer.WritePropertyName(stringBuilder.ToString());
                    writer.WriteValue(propValue);
                }
                else if (prop.PropertyType.IsEnum)
                {
                    stringBuilder.AppendLine(IsEnum);
                    writer.WritePropertyName(stringBuilder.ToString());
                    serializer.Serialize(writer, propValue);

                }
                else if (prop.PropertyType.FullName == "System.Windows.Point" || prop.PropertyType.FullName == "System.Windows.Media.Color")
                {
                    writer.WritePropertyName(stringBuilder.ToString());
                    serializer.Serialize(writer, propValue);
                }
                else if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                {
                    if (propValue == null)
                    {
                        var _type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        stringBuilder.AppendLine($"{Type}:{_type.Name}");
                    }
                    writer.WritePropertyName(stringBuilder.ToString());
                    writer.WriteValue(propValue);
                }
                else
                {
                    writer.WritePropertyName(stringBuilder.ToString());

                    serializer.Serialize(writer, propValue);
                }
            }
            writer.WriteEndObject();
        }
    }
}