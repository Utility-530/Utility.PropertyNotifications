using Newtonsoft.Json;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Utility.Nodify.Base;

namespace Utility.Nodify.Repository
{
    public class JsonReflectionConverter
    {
        public string Convert(object instance)
        {
            return instance switch
            {
                PropertyDescriptor propertyDescriptor => $"PropertyDescriptor|{JsonConvert.SerializeObject(propertyDescriptor)}",
                MethodInfo methodInfo => $"MethodInfo|{JsonConvert.SerializeObject(methodInfo)}",
                PropertyInfo propertyInfo => $"PropertyInfo|{JsonConvert.SerializeObject(propertyInfo)}",
                ParameterInfo parameterInfo => $"ParameterInfo|{JsonConvert.SerializeObject(parameterInfo)}",
                _ => throw new ArgumentException($"Unsupported instance type: {instance?.GetType()?.Name ?? "null"}")
            };
        }

        public IObservable<InstanceKey> ConvertBack(string compositeKey)
        {
            var (typePrefix, key) = parseKey(compositeKey);

            return Observable.Create<InstanceKey>(observer =>
            {
                observer.OnNext(new InstanceKey(key, JsonConvert.DeserializeObject(key, type(typePrefix))));
                return Disposable.Empty;
            });

            static Type type(string typePrefix) => typePrefix switch
            {
                "PropertyDescriptor" => typeof(PropertyDescriptor),
                "MethodInfo" => typeof(MethodInfo),
                "PropertyInfo" => typeof(PropertyInfo),
                "ParameterInfo" => typeof(ParameterInfo),
                _ => throw new ArgumentException($"Unsupported type prefix: {typePrefix}")
            };


            // Two-way conversion helper methods
            static (string TypePrefix, string Key) parseKey(string compositeKey)
            {
                var parts = compositeKey.Split('|', 2);
                if (parts.Length != 2)
                    throw new ArgumentException($"Invalid key format: {compositeKey}. Expected 'TypePrefix|Key'");
                return (parts[0], parts[1]);
            }
        }
    }
}