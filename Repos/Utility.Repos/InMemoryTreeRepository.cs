using System.Reactive.Disposables;
using Utility.Interfaces.Exs;
using Utility.Structs.Repos;
using static Utility.Repos.TreeRepository;

namespace Utility.Repos
{
    public class InMemoryTreeRepository : ITreeRepository
    {
        private Dictionary<string, List<Relationships>> relationships = new();
        private Dictionary<System.Type, int> types = new();
        private Dictionary<Guid, string> names = new();
        private List<Values> _values = new();

        public InMemoryTreeRepository()
        {
        }

        public void Copy(Guid guid, Guid newGuid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Duplication> Duplicate(Guid oldGuid, Guid? newParentGuid = null)
        {
            throw new NotImplementedException();
        }

        public IObservable<Key?> Find(Guid parentGuid, string? name = null, Guid? guid = null, System.Type? type = null, int? index = null)
        {
            if (parentGuid == default)
                throw new Exception($"{nameof(parentGuid)} is default");
            return Observable.Create<Key?>(observer =>
            {
                //return initialisationTask.ToObservable()
                //    .Subscribe(a =>
                //    {
                var table_name = getName(parentGuid);

                if (parentGuid == Guid.Empty)
                {
                }
                if (relationships.ContainsKey(table_name) == false)
                {
                    relationships[table_name] = [];
                }

                var typeId = type != null ? TypeId(type) : null;
                var tables = relationships[table_name].Where(a => a.Parent == parentGuid && guid != null ? a.Guid == guid : true && name != null ? a.Name == name : true && index != null ? a._Index == index : true && a.TypeId != default ? a.TypeId == typeId : true).ToList();
                if (tables.Count == 0)
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        observer.OnNext(null);
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }
                    InsertByParent(parentGuid, name, table_name, typeId, index)
                    .Subscribe(a =>
                    {
                        if (a is Guid guid)
                        {
                            observer.OnNext(new Key(guid, parentGuid, type, name, index, default));
                        }
                        else
                            throw new Exception("* 44 fd3323");
                        observer.OnCompleted();
                    });
                }
                else if (tables.Count == 1)
                {
                    var table = tables.Single();
                    setName(table.Guid, table_name);
                    observer.OnNext(new Key(table.Guid, parentGuid, table.TypeId.HasValue ? ToType(table.TypeId.Value) : null, table.Name, index, table.Removed));
                    observer.OnCompleted();
                }
                else if (name == null)
                {
                    foreach (var table in tables)
                    {
                        setName(table.Guid, table_name);
                        observer.OnNext(new Key(table.Guid, parentGuid, table.TypeId.HasValue ? ToType(table.TypeId.Value) : null, table.Name, index, table.Removed));
                    }
                    observer.OnCompleted();
                }
                else if (name != null)
                {
                    var table = tables.SingleOrDefault(a => a.Name == name) ?? throw new Exception("FD £££££");
                    {
                        setName(table.Guid, table_name);
                        observer.OnNext(new Key(table.Guid, parentGuid, table.TypeId.HasValue ? ToType(table.TypeId.Value) : null, table.Name, index, table.Removed));
                    }
                    observer.OnCompleted();
                }
                else if (guid.HasValue)
                {
                    var table = tables.SingleOrDefault(a => a.Guid == guid.Value) ?? throw new Exception("3FD £2ui£££44£");
                    {
                        setName(guid.Value, table_name);
                        observer.OnNext(new Key(table.Guid, parentGuid, table.TypeId.HasValue ? ToType(table.TypeId.Value) : null, table.Name, index, table.Removed));
                    }
                    observer.OnCompleted();
                }
                else
                {
                    throw new Exception("3e909re 4323");
                }
                return Disposable.Empty;
                //});
            });
        }

        private string getName(Guid guid)
        {
            return names[guid];
        }

        private void setName(Guid guid, string name)
        {
            names[guid] = name;
        }

        private int? TypeId(System.Type type)
        {
            return types.Get(type, (t) => types.Count);
        }

        private System.Type? ToType(int type)
        {
            return types.Where(a => a.Value == type).SingleOrDefault().Key;
        }

        private Dictionary<Guid, Dictionary<string, DateValue>> values = new();

        public IObservable<DateValue> Get(Guid guid, string? name = null)
        {
            return Observable.Create<DateValue>(observer =>
            {
                if (values.ContainsKey(guid) && name != null && values[guid].ContainsKey(name))
                {
                    observer.OnNext(values[guid][name]);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                List<Values>? tables = null;
                if (name == null)
                    tables = _values.Where(a => a.Guid == guid).GroupBy(a => a.Name).Select(a => a.MaxBy(a => a.Added)).ToList();
                else
                    tables = _values.Where(a => a.Guid == guid && a.Name == name).OrderByDescending(a => a.Added).Take(1).ToList();

                if (tables.Count != 0)
                {
                    foreach (var table in tables)
                    {
                        if (table is Values { Value: { } text, Added: { } added, Name: { } _name, TypeId: { } typeId })
                        {
                            System.Type? type = ToType(typeId) ?? throw new Exception("sd s389989898");
                            object? value = null;

                            try
                            {
                                value = JsonConvert.DeserializeObject(text, type, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                            }
                            catch (JsonReaderException ex)
                            {
                                value = text;
                            }

                            var _value = new DateValue(guid, _name, added, value);
                            lock (values)
                                if (values.ContainsKey(guid) == false)
                                {
                                    values[guid] = new() { { _name, _value } };
                                }
                                else if (values[guid].ContainsKey(_name) == false)
                                {
                                    values[guid].Add(_name, _value);
                                }
                            observer.OnNext(_value);
                        }
                    }
                    observer.OnCompleted();
                }
                else
                {
                    observer.OnNext(new DateValue(guid, name, default, null));
                    observer.OnCompleted();
                }
                return Disposable.Empty;
            });
        }

        public IObservable<Guid> InsertByParent(Guid parentGuid, string name, string? table_name = null, int? typeId = null, int? index = null)
        {
            return Observable.Create<Guid>(observer =>
            {
                table_name ??= getName(parentGuid);
                var guid = Guid.NewGuid();
                setName(guid, table_name);
                relationships[table_name].Add(new Relationships { Guid = guid, Name = name, _Index = index, Parent = parentGuid, Added = DateTime.Now, TypeId = typeId });
                observer.OnNext(guid);
                return Disposable.Empty;
            });
        }

        public IObservable<Key?> InsertRoot(Guid guid, string name, System.Type type)
        {
            // create table if not exists

            if (relationships.ContainsKey(name) == false)
            {
                relationships[name] = [];
            }

            var typeId = types.Get(type, (t) => types.Count);
            var tables = relationships[name].Where(a => a.Name == name && a.Guid == guid && a.TypeId == typeId).ToList();
            //$"SELECT * FROM '{nameof(Relationships)}' WHERE Name = '{name}' AND Guid = '{guid}' AND TypeId = '{typeId}'");

            names[guid] = name;

            //this.name = name;
            if (tables.Count > 1)
                throw new Exception("dsf 33p[p[");
            else if (tables.Count == 0)
            {
                relationships[name].Add(new Relationships { Guid = guid, Name = name, TypeId = typeId });
                return Observable.Return<Key?>(new Key(guid, default, type, name, 0, null));
            }
            else
            {
                foreach (var item in relationships[name])
                {
                    names[item.Guid] = name;
                }
                //return Observable.Return<Key?>(null);
                return Observable.Return<Key?>(new Key(guid, default, type, name, null, null));
            }
        }

        public int? MaxIndex(Guid parentGuid, string? name = null)
        {
            throw new NotImplementedException();
        }

        public DateTime Remove(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IObservable<IReadOnlyCollection<Key>> SelectKeys(Guid? parentGuid = null, string? name = null, string? table_name = null)
        {
            List<Relationships> tables;

            if (table_name != default)
            {
                tables = relationships[table_name];
            }
            else if (parentGuid.HasValue)
            {
                table_name = getName(parentGuid.Value);
                tables = relationships[table_name].Where(a => a.Parent == parentGuid && name == null ? true : a.Name == name).OrderBy(a => a._Index).ToList();
            }
            else
            {
                throw new Exception("fkfo40033 ww");
                //tables = connection.Table<Relationships>().ToList();
                //int i = 0;
                //foreach (var table in tables)
                //{
                //    table._Index = i++;
                //}
            }
            List<Key> selections = new();
            foreach (var table in tables)
            {
                //if (table.TypeId.HasValue == false)
                //    throw new Exception("ds 332344");
                //var type = ToType(table.TypeId.Value);

                System.Type type = null;
                if (table.TypeId.HasValue)
                    type = ToType(table.TypeId.Value);

                object item = null;
                if (type?.ContainsGenericParameters != false)
                {
                    throw new Exception("dgfsd..lll");
                }
                else
                {
                    if (table_name != null)
                        setName(table.Guid, table_name);
                    selections.Add(new(table.Guid, table.Parent, type, table.Name, table._Index, table.Removed));
                }
            }

            return Observable.Return((IReadOnlyCollection<Key>)selections);
        }

        public void Set(Guid guid, string name, object value, DateTime dateTime)
        {
            string text;
            if (values.ContainsKey(guid) && values[guid].ContainsKey(name) && values[guid][name].Value.Equals(value))
                return;
            //if (value is not string str)
            text = JsonConvert.SerializeObject(value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            //else
            //    text = value?.ToString();
            var query = _values.Where(a => a.Guid == guid && a.Name == name && a.Value == text).ToList();// $"SELECT * FROM '{nameof(Values)}' WHERE Guid = '{guid}' AND Name = '{name}' AND Value = '{text}'";
            if (query.Any() == false)
            {
                var typeId = TypeId(value.GetType());
                //connection.Insert();
                _values.Add(new Values { Guid = guid, Value = text, Name = name, Added = dateTime, TypeId = typeId });
                values.Get(guid)[name] = new(guid, name, dateTime, value);
            }
        }

        public void Reset()
        {
            relationships.Clear();
            types.Clear();
            names.Clear();
            _values.Clear();
        }

        public void UpdateName(Guid parentGuid, Guid guid, string name, string newName)
        {
            throw new NotImplementedException();
        }

        public IObservable<Key> FindRecursive(Guid parentGuid, int? maxIndex = null)
        {
            throw new NotImplementedException();
        }
    }
}