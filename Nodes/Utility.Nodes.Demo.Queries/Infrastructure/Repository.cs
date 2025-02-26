using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Splat;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Repos;
using Utility.Structs.Repos;

namespace Utility.Nodes.Demo.Queries
{
    public class Repository : ITreeRepository
    {
        Lazy<ILiteRepository> _lazyRepository = new(() => Locator.Current.GetService<ILiteRepository>());
        Lazy<IMainViewModel> mainViewmModel = new(() => Locator.Current.GetService<IMainViewModel>());

        public void Copy(Guid guid, Guid newGuid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Duplication> Duplicate(Guid oldGuid, Guid? newParentGuid = null)
        {
            throw new NotImplementedException();
        }

        public IObservable<Key?> Find(Guid parentGuid, string? name = null, Guid? guid = null, Type? type = null, int? index = null)
        {
            throw new NotImplementedException();
        }

        public IObservable<Guid> InsertByParent(Guid parentGuid, string name, string? table_name = null, int? typeId = null, int? index = null)
        {
            throw new NotImplementedException();
        }

        public IObservable<Key?> InsertRoot(Guid guid, string name, Type type)
        {
            throw new NotImplementedException();
        }

        public int? MaxIndex(Guid parentGuid, string? name = null)
        {
            throw new NotImplementedException();
        }

        public DateTime Remove(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public IObservable<IReadOnlyCollection<Key>> SelectKeys(Guid? parentGuid = null, string? name = null, string? table_name = null)
        {
            throw new NotImplementedException();
        }

        public IObservable<DateValue> Get(Guid guid, string? name = null)
        {
            return Observable.Create<DateValue>(observer =>
            {
                if (mainViewmModel.Value.Filter is FilterEntity { Body: { } body } entity)
                {

                    // Parse the JSON
                    JObject jObject = JObject.Parse(body);

                    // Find token by path (e.g., 'Address.City')
                    var token = jObject.DescendantsAndSelf().SingleOrDefault(a => a.SelectToken("Key")?.Value<string>() == guid.ToString());
                    var data = token["Data"];
                    var model = (IValue)data.ToObject(Type.GetType(data["$type"].Value<string>()));

                    observer.OnNext(new DateValue { Value = model.Value });

                    //Console.WriteLine(cityToken);  // Outputs: Anytown
                }
                return Disposable.Empty;
            });
        }

        public void Set(Guid guid, string name, object value, DateTime dateTime)
        {

            if (mainViewmModel.Value.Filter is FilterEntity { Body: { } body } entity)
            {

                // Parse the JSON
                JObject jObject = JObject.Parse(body);

                // Find token by path (e.g., 'Address.City')
                var token = jObject.DescendantsAndSelf().SingleOrDefault(a => a.SelectToken("Key")?.Value<string>() == guid.ToString());
                var innerToken = token["Data"][name];

                if (innerToken.Type == JTokenType.Object)
                {
                    if (innerToken.ToString() != JToken.FromObject(value).ToString())
                    {
                        token["Data"][name] = JsonConvert.SerializeObject(value.ToString());
                        entity.Body = jObject.ToString();
                        //Locator.Current.GetService<ILiteRepository>().Update(entity);
                    }
                }else if(innerToken.Type == JTokenType.String)
                {
                    // need to espace the string
                    if (System.Text.RegularExpressions.Regex.Unescape(innerToken.ToString()) != JToken.FromObject(value).ToString())
                    {
                        token["Data"][name] = JsonConvert.SerializeObject(value.ToString());
                        entity.Body = jObject.ToString();
                        //Locator.Current.GetService<ILiteRepository>().Update(entity);
                    }
                }
                else if (innerToken.Type == JTokenType.Integer)
                {
                    if (innerToken.ToString() != JToken.FromObject(value).ToString())
                    {
                        token["Data"][name] = JsonConvert.SerializeObject(value);
                        entity.Body = jObject.ToString();
                        //Locator.Current.GetService<ILiteRepository>().Update(entity);
                    }
                }
                else
                {

                }
                //Console.WriteLine(cityToken);  // Outputs: Anytown
            }

        }
    }
}
