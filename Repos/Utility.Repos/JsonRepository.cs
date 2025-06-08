using Newtonsoft.Json.Linq;
using Splat;
using System.Reactive.Disposables;
using Utility.Interfaces.Exs;
using Utility.Structs.Repos;

namespace Utility.Repos
{
    public interface IJObjectService
    {
        public JObject? Get();
        public void Set(JObject jObject);
    }

    public class JsonRepository : ITreeRepository
    {
        public readonly record struct JsonKey(Guid ParentGuid, string Name);
        Dictionary<JsonKey, Guid> keys = new();

        Lazy<IJObjectService> jObjectService = new(() => Locator.Current.GetService<IJObjectService>());

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

            if (name != null)
            {
                if (jObjectService.Value.Get() is JObject jObject)
                {
                    var token = jObject.DescendantsAndSelf().SingleOrDefault(a => a.SelectToken("Key")?.Value<string>() == parentGuid.ToString());

                    if (token != null)
                    {
                        //var _token = token.SelectTokens("Items[*].Data.Name").ToArray().SingleOrDefault(a => a.Value<string>() == name)?.Parent.Parent;
                        var names = jObject.DescendantsAndSelf().Where(a => a.SelectToken("Name")?.Value<string>() == name).ToArray();
                        if (names.Length > 0)
                        {
                            //return Observable.Return<Key?>(new Key { Guid = Guid.Parse(__token.Parent.Parent["Key"].Value<string>()), Name = name, ParentGuid = parentGuid, Type = Type.GetType(__token.Parent.First["$type"].Value<string>()) });
                            return Observable.Empty<Key?>();
                        }
                        else
                        {
                        }
                        //else
                        //{
                        //    //return Observable.Return<Key?>(new Key { Guid = Guid.Parse(token["Key"].Value<string>()), Name = name, ParentGuid = parentGuid, Type = Type.GetType(token["$type"].Value<string>()) });
                        //}
                    }
                }

                JsonKey key = new(parentGuid, name);

                if (!keys.ContainsKey(key))
                {
                    keys[key] = Guid.NewGuid();
                    return Observable.Return<Key?>(new Key(keys[key], parentGuid, type, name, index, null));
                }
                return Observable.Empty<Key?>();
            }

            //else
            //{
            //    return Observable.Return<Key?>(null);
            //    //return token
            //    //    .SelectToken("Items")
            //    //    .Select(a => new Key(Guid.Parse(a["Key"].Value<string>()), parentGuid, Type.GetType(a["Data"]["$type"].Value<string>()), a["Data"]["Name"].Value<string>(), null, null))
            //    //    .Cast<Key?>()
            //    //    .ToObservable();
            //}

            return Observable.Return<Key?>(null);
        }

        public IObservable<Guid> InsertByParent(Guid parentGuid, string name, string? table_name = null, int? typeId = null, int? index = null)
        {
            throw new NotImplementedException();
        }

        public IObservable<Key?> InsertRoot(Guid guid, string name, Type type)
        {
            return Observable.Return<Key?>(new Key(guid, default, type, name, 0, null));
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
                if (jObjectService.Value.Get() is JObject jObject)
                {
                    var token = jObject.DescendantsAndSelf().SingleOrDefault(a => a.SelectToken("Key")?.Value<string>() == guid.ToString());
                    if (token == null)
                    {
                    }
                    else
                    {
                        var data = token["Data"];
                        if (data.ToObject(Type.GetType(data["$type"].Value<string>())) is IValue model)
                        {
                            observer.OnNext(new DateValue { Value = model.Value });
                        }
                        else
                        {
                        }
                    }
                }
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

        public void Set(Guid guid, string name, object value, DateTime dateTime)
        {
            if (guids.Contains(guid))
                return;

            if (jObjectService.Value.Get() is JObject jObject)
            {
                var token = jObject.DescendantsAndSelf().SingleOrDefault(a => a.SelectToken("Key")?.Value<string>() == guid.ToString());
                if (token == null)
                    return;


                //var serialisedValue = JsonConvert.SerializeObject(value.ToString());
                var innerToken = token.SelectToken(name);

                if (value == null)
                {
                    innerToken.Replace(null);

                }
                else if (TypeHelper.IsValueOrString(value.GetType()) == false)
                {
                    var _value = JToken.FromObject(value);
                    innerToken.Replace(_value);
                }
                else
                {
                    (innerToken as JValue).Value = value;
                }
                //entity.Body = jObject.ToString();
                jObjectService.Value.Set(jObject);

            }

        }

        public void UpdateName(Guid parentGuid, Guid guid, string name, string newName)
        {
            throw new NotImplementedException();
        }

        public static JsonRepository Instance { get; } = new();
    }
}
