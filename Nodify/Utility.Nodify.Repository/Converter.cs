using Newtonsoft.Json;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Utility.Nodify.Base;

namespace Utility.Nodify.Repository
{
    public class Converter
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
            var (typePrefix, key) = ParseKey(compositeKey);

            return Observable.Create<InstanceKey>(observer =>
            {
                if (typePrefix == "PropertyDescriptor")
                {
                    var param = JsonConvert.DeserializeObject<PropertyDescriptor>(key);
                    observer.OnNext(new InstanceKey(key, param));
                    return Disposable.Empty;
                }
                else if (typePrefix == "MethodInfo")
                {
                    var param = JsonConvert.DeserializeObject<MethodInfo>(key);
                    observer.OnNext(new InstanceKey(key, param));
                    return Disposable.Empty;
                }
                else if (typePrefix == "PropertyInfo")
                {
                    var param = JsonConvert.DeserializeObject<PropertyInfo>(key);
                    observer.OnNext(new InstanceKey(key, param));
                    return Disposable.Empty;
                }
                else if (typePrefix == "ParameterInfo")
                {
                    var param = JsonConvert.DeserializeObject<ParameterInfo>(key);
                    observer.OnNext(new InstanceKey(key, param));
                    return Disposable.Empty;
                }

                throw new ArgumentException($"Unsupported type prefix: {typePrefix}");
            });



            // Two-way conversion helper methods
            (string TypePrefix, string Key) ParseKey(string compositeKey)
            {
                var parts = compositeKey.Split('|', 2);
                if (parts.Length != 2)
                    throw new ArgumentException($"Invalid key format: {compositeKey}. Expected 'TypePrefix|Key'");
                return (parts[0], parts[1]);
            }
        }
    }
}