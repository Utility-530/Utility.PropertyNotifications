using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Splat;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Repos;
using Utility.Structs.Repos;
using System.Text.RegularExpressions;

namespace Utility.Nodes.Demo.Queries
{
    public class Repository : ITreeRepository
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
            if (name == null)
                return Observable.Return<Key?>(default);
            else
            {
                if (mainViewmModel.Value.Filter is FilterEntity { Body: { } body } entity)
                {
                    JObject jObject = JObject.Parse(body);
                    // find the node
                    var token = jObject.DescendantsAndSelf().SingleOrDefault(a => a.SelectToken("Key")?.Value<string>() == parentGuid.ToString());
                    if (token != null)
                    {
                        var _token = token.SelectTokens("Items[*].Data.Name").ToArray().SingleOrDefault(a => a.Value<string>() == name)?.Parent.Parent;
                        if (_token != null)
                        {
                            //return Observable.Return<Key?>(new Key { Guid = Guid.Parse(token["Key"].Value<string>()), Name =name, ParentGuid = parentGuid, Type = Type.GetType(token["$type"].Value<string>()) });
                            return Observable.Empty<Key?>();
                        }
                        else
                        {
                            throw new Exception(" 344322");
                        }
                    }
                }
                var _guid = Guid.NewGuid();
                guids.Add(_guid);
                return Observable.Return<Key?>(new Key { Guid = _guid });
            }
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
            if (guids.Contains(guid))
                return Observable.Empty<DateValue>();

            return Observable.Create<DateValue>(observer =>
            {
                if (mainViewmModel.Value.Filter is FilterEntity { Body: { } body } entity)
                {
                    JObject jObject = JObject.Parse(body);
                    // find the node
                    var token = jObject.DescendantsAndSelf().SingleOrDefault(a => a.SelectToken("Key")?.Value<string>() == guid.ToString());
                    var data = token["Data"];
                    // serialise
                    var model = (IValue)data.ToObject(Type.GetType(data["$type"].Value<string>()));
                    // get the value
                    observer.OnNext(new DateValue { Value = model.Value });
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
                var innerToken = token["Data"][name];
                var _value = JToken.FromObject(value).ToString();
                var serialisedValue = JsonConvert.SerializeObject(value.ToString());

                if (innerToken.Type == JTokenType.Object)
                {
                    if (innerToken.ToString() != _value)
                    {
                        token["Data"][name] = serialisedValue;
                        entity.Body = jObject.ToString();
                    }
                }
                else if (innerToken.Type == JTokenType.String)
                {
                    // need to escape the string
                    if (Regex.Unescape(innerToken.ToString()) != _value)
                    {
                        token["Data"][name] = serialisedValue;
                        entity.Body = jObject.ToString();
                    }
                }
                else if (innerToken.Type == JTokenType.Integer)
                {
                    if (innerToken.ToString() != _value)
                    {
                        token["Data"][name] = serialisedValue;
                        entity.Body = jObject.ToString();
                    }
                }
                else
                {
                    throw new Exception("333;;;f");
                }
            }

        }
    }
}
