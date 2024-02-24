using Bogus.DataSets;
using System;
using Utility.Descriptors.Types;
using Guid = System.Guid;

namespace Utility.Descriptors.Repositorys
{
    //public class GuidRepository
    //{
    //    public record Table
    //    {
    //        [PrimaryKey, AutoIncrement]
    //        public int Id { get; set; }

    //        public Guid Guid { get; set; }

    //        public Guid? Parent { get; set; }

    //        public string Name { get; set; }
    //    }

    //    private readonly SQLiteAsyncConnection connection;
    //    private readonly Task initialisationTask;

    //    public GuidRepository(string? dbDirectory = default)
    //    {
    //        if (dbDirectory != default)
    //            Directory.CreateDirectory(dbDirectory);
    //        connection = new SQLiteAsyncConnection(Path.Combine(dbDirectory ?? string.Empty, "data" + "." + "sqlite"));
    //        initialisationTask = Task.WhenAll(
    //            new[]{
    //                connection.CreateTableAsync<Table>(),
    //            });
    //    }

    //    public async Task<IReadOnlyCollection<Guid>> Find(Guid parentGuid)
    //    {
    //        await initialisationTask;
    //        var tables = await connection.QueryAsync<Table>($"Select * from 'Table' where Parent = '{parentGuid}'");
    //        List<Guid> childKeys = new();
    //        foreach (var table in tables)
    //        {
    //            childKeys.Add(table.Guid);
    //        }
    //        return childKeys;
    //    }

    //    public async Task<Guid> Find(Guid parentGuid, string? localName)
    //    {
    //        await initialisationTask;
    //        var tables = await connection.QueryAsync<Table>($"Select * from 'Table' where Parent = '{parentGuid}' AND Name = '{localName}'");
    //        if (tables.Count == 0)
    //        {
    //            var guid = Guid.NewGuid();
    //            await connection.RunInTransactionAsync(c =>
    //            {
    //                var tables = c.Query<Table>($"Select * from 'Table' where Parent = '{parentGuid}' AND Name = '{localName}'");
    //                if (tables.Count != 0)
    //                    return;

    //                var i = c.Insert(new Table { Guid = guid, Name = localName, Parent = parentGuid });
    //            });
    //            return guid;
    //        }
    //        if (tables.Count == 1)
    //        {
    //            var table = tables.Single();
    //            return table.Guid;
    //        }
    //        else
    //        {
    //            throw new Exception("3e909re 4323");
    //        }
    //    }

    //    public static GuidRepository Instance { get; } = new();
    //}   


    public class GuidRepository
    {
        public record Relationships
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            public Guid Guid { get; set; }

            public Guid? Parent { get; set; }

            public string Name { get; set; }

            public int? _Index { get; set; }
            public DateTime Added { get; set; }
            public DateTime? Removed { get; set; }

            public int? TypeId { get; set; }
        }

        public record Values
        {
            [PrimaryKey]

            public Guid Guid { get; set; }

            public string Value { get; set; }

            public int TypeId { get; set; }
        }

        public record Type
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public string? Assembly { get; set; }
            public string? Namespace { get; set; }
            public string Name { get; set; }
            public string? ClassName { get; set; }
            public Kind Kind { get; set; }
        }

        private readonly SQLiteConnection connection;
        private readonly Task initialisationTask;

        private GuidRepository(string? dbDirectory = default)
        {
            if (dbDirectory != default)
                Directory.CreateDirectory(dbDirectory);
            connection = new SQLiteConnection(Path.Combine(dbDirectory ?? string.Empty, "data" + "." + "sqlite"));
            connection.CreateTable<Relationships>();
            connection.CreateTable<Values>();
            connection.CreateTable<Type>();
        }

        public Task<IReadOnlyCollection<(Guid guid, System.Type type, int? index)>> Select(Guid? parentGuid = null, string? name = null)
        {
            string query = $"SELECT * FROM '{nameof(Relationships)}' WHERE Parent {ToComparison(parentGuid)}";
            var tables = connection.Query<Relationships>(query + (name == null ? string.Empty : $" AND Name = '{name}'"));
            List<(Guid, System.Type, int?)> childKeys = new();
            foreach (var table in tables)
            {
                if (table.TypeId.HasValue == false)
                    throw new Exception("ds 332344");
                var type = ToType(table.TypeId.Value);
                if (type == null)
                    throw new Exception("3 333 ff");
                childKeys.Add((table.Guid, type, table._Index));
            }
            return Task.FromResult((IReadOnlyCollection<(Guid guid, System.Type type, int? index)>)childKeys);
        }

        public IEnumerable<(Guid old, Guid @new)> Duplicate(Guid oldGuid, Guid? newParentGuid = default)
        {
            var _tables = newParentGuid != default ? connection.Query<Relationships>($"SELECT * FROM '{nameof(Relationships)}' WHERE Parent = '{newParentGuid}'") : (IList<Relationships>)Array.Empty<Relationships>();

            Guid guid;
            if (_tables.Count == 0)
            {
                var table = connection.Query<Relationships>($"SELECT * FROM '{nameof(Relationships)}' WHERE Guid = '{oldGuid}'").Single();
                if (table.Parent.HasValue == false)
                    throw new Exception("F £\\");

                var lastIndex = MaxIndex(table.Parent.Value) + 1;
                guid = Guid.NewGuid();
                var i = connection.Insert(new Relationships { Guid = guid, Name = table.Name, _Index = lastIndex, Parent = newParentGuid ?? table.Parent, Added = DateTime.Now });
            }
            else if (_tables.Count == 1)
            {
                guid = _tables.Single().Guid;
            }
            else
            {
                throw new Exception("BG £££");
            }

            yield return (oldGuid, guid);
            string query = $"SELECT * FROM '{nameof(Relationships)}' WHERE Parent = '{oldGuid}'";
            var tables = connection.Query<Relationships>(query);

            foreach (var t in tables)
            {
                foreach (var x in Duplicate(t.Guid, guid))
                    yield return x;
            }
        }

        public Task<Guid> Find(Guid parentGuid, string name, System.Type? type = null, int? index = null)
        {
            var typeId = type != null ? (int?)TypeId(type) : null;
            var tables = connection.Query<Relationships>($"SELECT * FROM '{nameof(Relationships)}' WHERE Parent = '{parentGuid}' AND Name = '{name}' AND _Index {ToComparison(index)} AND TypeId {ToComparison(typeId)}");
            if (tables.Count == 0)
            {
                if (InsertParent(parentGuid, name, typeId, index) is Guid guid)
                    return Task.FromResult(guid);
            }
            else if (tables.Count == 1)
            {
                var table = tables.Single();
                return Task.FromResult(table.Guid);
            }
            else
            {
                throw new Exception("3e909re 4323");
            }
            throw new Exception("09re 4323");
        }

        public Guid? InsertParent(Guid parentGuid, string name, int? typeId = null, int? index = null)
        {
            var tables = connection.Query<Relationships>($"SELECT * FROM '{nameof(Relationships)}' WHERE Parent = '{parentGuid}' AND Name = '{name}' AND _Index {ToComparison(index)} AND TypeId {ToComparison(typeId)}");
            if (tables.Count != 0)
                return null;
            var guid = Guid.NewGuid();
            var i = connection.Insert(new Relationships { Guid = guid, Name = name, _Index = index, Parent = parentGuid, Added = DateTime.Now, TypeId = typeId });
            return guid;
        }

        public int Insert(Guid guid, string name, System.Type type, Guid? parentGuid = null, int? index = null)
        {
            var typeId = (int)TypeId(type);
            var tables = connection.Query<Relationships>($"SELECT * FROM '{nameof(Relationships)}' WHERE Name = '{name}' AND Guid = '{guid}' AND TypeId = '{typeId}' AND Parent {ToComparison(parentGuid)} AND _Index {ToComparison(index)}");
            if (tables.Count != 0)
                return 0;
            return connection.Insert(new Relationships { Guid = guid, Name = name, _Index = index, Parent = parentGuid, Added = DateTime.Now, TypeId = typeId });
        }


        //public int InsertAncestor(Guid guid, string name, System.Type type, Guid? parentGuid = null, int? index = null)
        //{
        //    var typeId = (int)TypeId(type);
        //    var tables = connection.Query<Relationships>($"SELECT * FROM '{nameof(Relationships)}' WHERE Name = '{name}' AND Guid = '{guid}' AND TypeId = '{typeId}' AND Parent {ToComparison(parentGuid)} AND _Index {ToComparison(index)}");
        //    if (tables.Count != 0)
        //        return 0;
        //    return connection.Insert(new Relationships { Guid = guid, Name = name, _Index = index, Parent = parentGuid, Added = DateTime.Now, TypeId = typeId });
        //}

        public int? MaxIndex(Guid parentGuid, string? name = default)
        {
            string query = $"SELECT MAX({nameof(Relationships._Index)}) FROM '{nameof(Relationships)}' WHERE {nameof(Relationships.Parent)} = '{parentGuid}'";
            var index = connection.ExecuteScalar<int?>(query + (name == default ? string.Empty : $"AND {nameof(Relationships.Name)} = '{name}'"));
            return index;
        }

        public void Remove(Guid guid)
        {
            var find = connection.FindWithQuery<Relationships>($"SELECT * FROM '{nameof(Relationships)}' WHERE Guid = '{guid}'");
            find.Removed = DateTime.Now;
            connection.Update(find);

        }

        private static string ToComparison(object? value)
        {
            return value == null ? "is null" : $"= '{value}'";
        }

        public void Set(Guid guid, object value)
        {
            var typeId = TypeId(value.GetType());
            var text = JsonConvert.SerializeObject(value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            if (connection.Query<Values>($"SELECT * FROM '{nameof(Values)}' WHERE Guid = '{guid}' AND Value = '{text}'").Any() == false)
                connection.InsertOrReplace(new Values { Guid = guid, Value = text, TypeId = typeId });
        }

        public object? Get(Guid guid)
        {
            var table = connection.Find<Values>(guid);
            if (table is Values { Value: { } text, TypeId: { } typeId })
            {
                System.Type? type = ToType(typeId);
                if (type == null)
                    throw new Exception("sd s389989898");

                var value = JsonConvert.DeserializeObject(text, type);
                return value;
            }
            return null;
        }

        public void Copy(Guid guid, Guid newGuid)
        {
            var table = connection.Find<Values>(guid);
            if (table is Values { Value: { } text, TypeId: { } typeId })
            {
                connection.InsertOrReplace(new Values { Guid = newGuid, Value = text, TypeId = typeId });
            }
        }


        public async void Register(Guid guid, INotifyPropertyCalled propertyCalled)
        {
            //await initialisationTask;
            propertyCalled
                .WhenCalled()
                .Subscribe(a =>
                {
                    if (Get(guid) is { } value)
                        if (value.Equals(a.Value) == false)
                            if (propertyCalled is IRaisePropertyChanged changed)
                            {
                                changed.RaisePropertyChanged(value);
                            }

                });
        }


        public async void Register(Guid guid, INotifyPropertyReceived propertyReceived)
        {
            //await initialisationTask;
            propertyReceived
                .WhenReceived()
                .Subscribe(a =>
                {
                    Set(guid, a.Value);
                    //var typeId = TypeId(a.Value.GetType());
                    //var text = JsonConvert.SerializeObject(a.Value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                    //connection.InsertOrReplace(new Table { Guid = guid, Value = text, TypeId = typeId });
                });
        }

        System.Type? ToType(int typeId)
        {
            var type = connection.Table<Type>().Where(v => v.Id.Equals(typeId)).First();
            //Regex regex = new ("(_property_)|(_method_)");
            //var match = regex.Match(type.Namespace);
            //string assemblyQualifiedName;
            //if (match.Success)
            //{
            //    var _namespace = regex.Replace(type.Namespace, string.Empty);
            //    assemblyQualifiedName = Assembly.CreateQualifiedName(type.Assembly, _namespace);
            //    var parent = System.Type.GetType(assemblyQualifiedName);

            //    switch (match.Groups[0].Value)
            //    {
            //        case "_property_":
            //            {
            //                return new PropertyType(parent, type.Name);
            //            }
            //        case "_method_":
            //            {
            //                return new MethodType(parent, type.Name);
            //            }
            //    }

            //}

            var assemblyQualifiedName = Assembly.CreateQualifiedName(type.Assembly, $"{type.Namespace}.{type.Name}");
            return System.Type.GetType(assemblyQualifiedName);
        }

        int TypeId(System.Type type)
        {
            var types = connection.Query<Type>($"SELECT * FROM '{nameof(Type)}' WHERE Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}'");
            var singleType = types.SingleOrDefault();
            if (singleType == default)
            {
                connection.RunInTransaction(() =>
                {
                    connection.Insert(new Type { Assembly = type.Assembly.FullName, Namespace = type.Namespace, Name = type.Name });
                    types = connection.Query<Type>($"SELECT * FROM '{nameof(Type)}' WHERE Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}'");
                    if (types.Count > 1)
                        throw new Exception("fds ");
                });
                return connection.ExecuteScalar<int>($"SELECT Id FROM '{nameof(Type)}' WHERE Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}' ");
            }
            else
            {
                return singleType.Id;
            }

            Kind? GetKind()
            {
                if (type is IKind { Kind: { } kind })
                {
                    return kind;
                }
                return null;
            }


            string? GetClassName()
            {
                if (type is IClassName { Name: { } name })
                {
                    return name;
                }
                return null;
            }
        }


        public static GuidRepository Instance { get; } = new("c:/ProgramData/Utility");
    }
}
