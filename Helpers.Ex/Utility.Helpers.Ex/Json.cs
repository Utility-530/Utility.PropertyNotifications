#nullable enable

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Utility.Helpers.Ex
{
    public static class JsonHelper
    {
        /// <summary>
        /// Deserialize JSON data into an T object
        /// </summary>
        /// <param name="data">The JSON data that should be deserialized</param>
        /// <returns>The T object that was deserialized</returns>
        public static T? Deserialize<T>(string data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length == 0) throw new ArgumentException(nameof(data));

            var deserializeObject = JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
            });
            return deserializeObject;
        }

        /// <summary>
        /// Deserialize JSON data into an T object asynchronously
        /// </summary>
        /// <param name="data">The JSON data that should be deserialized</param>
        /// <returns>The T object that was deserialized</returns>
        public static async Task<T?> DeserializeAsync<T>(string data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length == 0) throw new ArgumentException(nameof(data));

            return await Task.Run(() => JsonConvert.DeserializeObject<T>(data));
        }

        public static T Deserialize<T>(Stream stream, JsonSerializerSettings? settings = null)
        {
            var serializer = settings == null ? new JsonSerializer() : JsonSerializer.Create(settings);
            using var sr = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(sr);
            return serializer.Deserialize<T>(jsonTextReader) ?? throw new InvalidOperationException();
        }

        public static void Serialize<T>(T combinedInformation, FileInfo filePath)
        {
            using StreamWriter sw = new StreamWriter(filePath.FullName);
            using JsonWriter writer = new JsonTextWriter(sw);
            Serializer.Serialize(writer, combinedInformation, typeof(T));
        }

        public static string Serialize<T>(T combinedInformation)
        {
            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                //TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
            };

            //serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());

            using StringWriter sw = new StringWriter();
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, combinedInformation, typeof(T));
            return sw.ToString();
        }

        public static JsonSerializerSettings SerializerSettings
        {
            get
            {
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };
                jsonSerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                return jsonSerializerSettings;
            }
        }

        public static JsonSerializer Serializer
        {
            get
            {
                var jsonSerializer = new JsonSerializer
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                };
                jsonSerializer.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                return jsonSerializer;
            }
        }


        public static void Reset(this JObject jObject)
        {
            foreach (var x in jObject.Properties())
            {
                if (x.Name == "$type")
                {
                    // do nothing
                }
                else if (IsEnum(x))
                {
                    x.Value = AsDefaultEnum(x).ToString();
                }

                else if (x.Value is JValue jValue)
                    switch (jValue.Type)
                    {
                        case JTokenType.String:
                            x.Value = string.Empty;
                            break;
                        case JTokenType.Float:
                            x.Value = 0.0;
                            break;
                        case JTokenType.Integer:
                            x.Value = 0;
                            break;
                        case JTokenType.TimeSpan:
                            x.Value = TimeSpan.Zero;
                            break;
                    }
                else if (x.Value is JToken token)
                {

                }
                else
                {
                    throw new Exception("EF 343");
                }
            }
        }

        public static bool IsEnum(this JProperty jProperty)
        {
            if (jProperty.Parent["$isenum"]?.Last?.Last is IEnumerable _e)
            {
                foreach (JValue x in _e)
                {
                    if (x.Value == jProperty.Name)
                        return true;
                }
            }
            return false;
        }
        public static Enum AsDefaultEnum(this JProperty jProperty)
        {

            var type = Type.GetType(jProperty.Parent["$type"].Value<string>());
            var prop = type.GetProperty(jProperty.Name);
            return Enum.GetValues(prop.PropertyType).Cast<Enum>().First();

        }
    }

    public class FileSystemInfoConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(FileSystemInfo).IsAssignableFrom(objectType);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            var jObject = JObject.Load(reader);
            var fullPath = (jObject["FullPath"] ?? throw new InvalidOperationException()).Value<string>();
            return Activator.CreateInstance(objectType, fullPath);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is FileSystemInfo info)
            {
                var token = JToken.FromObject(new { FullPath = info.FullName });
                token.WriteTo(writer);
            }
            else
            {
                throw new Exception($"Value is not of type, {nameof(FileSystemInfo)}");
            }
        }
    }
}