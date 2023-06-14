using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Utility.Helpers.Ex
{
    public static class BsonHelper
    {
        public static object? Deserialise(string str, params string[] ignoreProperties)
        {
            return JsonConvert.DeserializeObject(str, new JsonSerializerSettings()
            {
                //ContractResolver = new IgnorePropertiesResolver(Array.Empty<string>(), ignoreProperties)
                Converters = new JsonConverter[] { new StringEnumConverter(), new GuidConverter() },
            });
        }

        public static object? Deserialise(string str, Type type, params string[] ignoreProperties)
        {
            //var reader = new Newtonsoft.Json.Bson.BsonDataReader(new MemoryStream(Encoding.UTF8.GetBytes(str)));
            //reader.
            BsonDataReader reader = new BsonDataReader(new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                SupportMultipleContent = true,
                DateTimeKindHandling = DateTimeKind.Utc
            };

            JsonSerializer serialiser = new();
            var tst1A = serialiser.Deserialize(reader, type);

            return JsonConvert.DeserializeObject(str, type, new JsonSerializerSettings()
            {
                //ContractResolver = new IgnorePropertiesResolver(Array.Empty<string>(), ignoreProperties),
                Converters = new JsonConverter[] { new StringEnumConverter(), new GuidConverter() },
            });
        }

        public static T? Deserialise<T>(string str, params string[] ignoreProperties)
        {
            return JsonConvert.DeserializeObject<T>(str, new JsonSerializerSettings()
            {
                //ContractResolver = new IgnorePropertiesResolver(Array.Empty<string>(), ignoreProperties)
                Converters = new JsonConverter[] { new StringEnumConverter(), new GuidConverter() },
            });
        }
    }

    /// <summary>
    /// <a href="https://stackoverflow.com/questions/10169648/how-to-exclude-property-from-json-serialization"></a>
    /// </summary>
    //short helper class to ignore some properties from serialization
    public class IgnorePropertiesResolver : DefaultContractResolver
    {
        private readonly HashSet<string> deserialisationPropertyNamesToIgnore;
        private readonly HashSet<string> serialisationPropertyNamesToIgnore;

        public IgnorePropertiesResolver(IReadOnlyCollection<string> serialisationPropertyNamesToIgnore, IReadOnlyCollection<string> deserialisationPropertyNamesToIgnore)
        {
            this.deserialisationPropertyNamesToIgnore = new HashSet<string>(deserialisationPropertyNamesToIgnore);
            this.serialisationPropertyNamesToIgnore = new HashSet<string>(serialisationPropertyNamesToIgnore);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            //if (this.deserialisationPropertyNamesToIgnore.Contains(property.PropertyName))
            //{
            //    property.ShouldDeserialize = _ => false;
            //}
            //if (this.serialisationPropertyNamesToIgnore.Contains(property.PropertyName))
            //{
            //    property.ShouldSerialize = _ => false;
            //}
            property.ShouldDeserialize = _ => false;
            property.ShouldSerialize = _ => false;
            return property;
        }
    }
}