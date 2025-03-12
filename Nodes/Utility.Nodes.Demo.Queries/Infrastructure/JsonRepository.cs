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
    public class JsonRepository : ITreeRepository
    {
        Lazy<IMainViewModel> mainViewmModel = new(() => Locator.Current.GetService<IMainViewModel>());

        List<Guid> guids = new();

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
            if (mainViewmModel.Value.Filter is FilterEntity { Body: { } body } entity)
            {
                JObject jObject = JObject.Parse(body);
                // find the node
                var token = jObject.DescendantsAndSelf().SingleOrDefault(a => a.SelectToken("Key")?.Value<string>() == parentGuid.ToString());
                if (token != null)
                {

                    if (name != null)
                    {
                        var _token = token.SelectTokens("Items[*].Data.Name").ToArray().SingleOrDefault(a => a.Value<string>() == name)?.Parent.Parent;
                        if (_token != null)
                        {
                            return Observable.Return<Key?>(new Key { Guid = Guid.Parse(token["Key"].Value<string>()), Name = name, ParentGuid = parentGuid, Type = Type.GetType(token["$type"].Value<string>()) });
                        }
                        else
                        {

                            return Observable.Return<Key?>(new Key(Guid.NewGuid(), parentGuid, type, name, index, null));
                        }

                    }
                    else
                    {
                        return Observable.Empty<Key?>();
                        //return token
                        //    .SelectToken("Items")
                        //    .Select(a => new Key(Guid.Parse(a["Key"].Value<string>()), parentGuid, Type.GetType(a["Data"]["$type"].Value<string>()), a["Data"]["Name"].Value<string>(), null, null))
                        //    .Cast<Key?>()
                        //    .ToObservable();
                    }
                }
            }
            if (name != null)
            {
                return Observable.Return<Key?>(new Key(Guid.NewGuid(), parentGuid, type, name, index, null));
            }
            return Observable.Return<Key?>(null);
        }

        public IObservable<Guid> InsertByParent(Guid parentGuid, string name, string? table_name = null, int? typeId = null, int? index = null)
        {
            throw new NotImplementedException();
        }

        public IObservable<Key?> InsertRoot(Guid guid, string name, Type type)
        {
            return Observable.Empty<Key?>();
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
            if (guids.Contains(guid))
                return Observable.Empty<DateValue>();

            return Observable.Create<DateValue>(observer =>
            {
                if (mainViewmModel.Value.Filter is FilterEntity { Body: { } body } entity)
                {
                    JObject jObject = JObject.Parse(body);
                    // find the node
                    var token = jObject.DescendantsAndSelf().SingleOrDefault(a => a.SelectToken("Key")?.Value<string>() == guid.ToString());
                    if (token == null)
                    {
                        observer.OnCompleted();
                    }
                    else
                    {
                        var data = token["Data"];
                        // serialise
                        if (data.ToObject(Type.GetType(data["$type"].Value<string>())) is IValue model)
                        {
                            // get the value
                            observer.OnNext(new DateValue { Value = model.Value });
                            observer.OnCompleted();
                        }
                        else
                        {
                            observer.OnCompleted();
                        }
                    }
                }
                return Disposable.Empty;
            });
        }

        public void Set(Guid guid, string name, object value, DateTime dateTime)
        {
            if (guids.Contains(guid))
                return;

            if (mainViewmModel.Value.Filter is FilterEntity { Body: { } body } entity)
            {
                JObject jObject = JObject.Parse(body);
                var token = jObject.DescendantsAndSelf().SingleOrDefault(a => a.SelectToken("Key")?.Value<string>() == guid.ToString());
                if (token == null)
                    return;
                var _value = JToken.FromObject(value);
                //var serialisedValue = JsonConvert.SerializeObject(value.ToString());
                var innerToken = token.SelectToken(name);

                if (Utility.Helpers.TypeHelper.IsValueOrString(value.GetType()) == false)
                {
                    innerToken.Replace(_value);
                }
                else
                {
                    (innerToken as JValue).Value = value;
                }
                entity.Body = jObject.ToString();

            }

        }
    }
}
