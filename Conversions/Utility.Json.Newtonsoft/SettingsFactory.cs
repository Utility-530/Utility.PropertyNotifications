using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Conversions.Json.Newtonsoft
{
    public static class SettingsFactory
    {
        static JsonSerializerSettings combined;

        public static JsonSerializerSettings Combined
        {
            get
            {
                if (combined != null)
                    return combined;

                var _settings = new JsonSerializerSettings
                { 
                    TypeNameHandling = TypeNameHandling.All, 
                    CheckAdditionalContent = false
                };

                foreach (var setting in converters())
                {
                    _settings.Converters.Add(setting);
                }
                return combined = _settings;

                IEnumerable<JsonConverter> converters()
                {
                    yield return new AssemblyJsonConverter();
                    yield return (new PropertyInfoJsonConverter());
                    yield return (new MethodInfoJsonConverter());
                    yield return (new ParameterInfoJsonConverter());
                    yield return (new AttributeCollectionConverter());
                    yield return (new Utility.Conversions.Json.Newtonsoft.DescriptorConverter());
                    yield return (new StringToGuidConverter());
                }
            }
        }

    }
}
