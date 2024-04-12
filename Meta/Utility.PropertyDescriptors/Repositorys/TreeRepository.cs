using System;
using Utility.Descriptors.Types;
using Guid = System.Guid;

namespace Utility.Descriptors.Repositorys
{
    public readonly record struct Duplication(Guid Old, Guid New);
    public readonly record struct DateValue(DateTime DateTime, object Value);
    public readonly record struct Key(Guid Guid, Guid ParentGuid, Type Type, string Name, int? Index)
    {
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }


    public class TreeRepository : ITreeRepository
    {

        Dictionary<Guid, DateValue> values = new();
        Dictionary<int, System.Type> types = new();
        Dictionary<Guid, string> tablelookup = new();


        public record Relationships
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public Guid Guid { get; set; }
            public Guid Parent { get; set; }
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
            public DateTime Added { get; set; }

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

        public record String
        {
            public string Name { get; set; }
        }

        private readonly SQLiteConnection connection;
        private readonly Task initialisationTask;


        private TreeRepository(string? dbDirectory = default)
        {
            if (dbDirectory != default)
                Directory.CreateDirectory(dbDirectory);
            connection = new SQLiteConnection(Path.Combine(dbDirectory ?? string.Empty, "data" + "." + "sqlite"));
            connection.CreateTable<Relationships>();
            connection.CreateTable<Values>();
            connection.CreateTable<Type>();
            initialisationTask = Initialise();
        }

        public bool IsInitialised { get; set; }

        public Task Initialise()
        {
            if (IsInitialised == false)
            {
                return Task.Run(() =>
                {
                    foreach (var relationship in connection.Table<Relationships>().ToList())
                    {
                        tablelookup[relationship.Guid] = relationship.Name;
                    }
                    IsInitialised = true;
                });
            }
            return Task.CompletedTask;
        }

        //public Task<IReadOnlyCollection<Key>> Keys(string? name = default)
        //{
        //    var tables = connection.Table<Relationships>();
        //    List<Key> childKeys = new();
        //    foreach (var table in tables)
        //    {
        //        var type = ToType(table.TypeId ?? throw new Exception("DF 32cd"));
        //        if (type == null)
        //            throw new Exception("3 333 ff");
        //        tablelookup[table.Guid] = table.Name;
        //        childKeys.Add(new(table.Guid, type, table.Name));
        //    }
        //    return Task.FromResult((IReadOnlyCollection<Key>)childKeys);
        //}

        public Task<IReadOnlyCollection<Key>> SelectKeys(Guid? parentGuid = null, string? name = null, string? table_name = default)
        {
            List<Relationships> tables;

            if (table_name != default)
            {
                string query = $"SELECT * FROM '{table_name}'";
                tables = connection.Query<Relationships>(query);
            }
            else if (parentGuid.HasValue)
            {
                table_name = tablelookup[parentGuid.Value];
                string query = $"SELECT * FROM '{table_name}' WHERE Parent = '{parentGuid}'" + (name == null ? string.Empty : $" AND Name = '{name}' ORDER BY {nameof(Relationships._Index)}");
                tables = connection.Query<Relationships>(query);
            }
            else
            {
                tables = connection.Table<Relationships>().ToList();
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
                //if (type == null)
                //    throw new Exception("3 333 ff");
                tablelookup[table.Guid] = table.Name;
                selections.Add(new(table.Guid, table.Parent, type, table.Name, table._Index));
            }
            return Task.FromResult((IReadOnlyCollection<Key>)selections);
        }

        public IEnumerable<Duplication> Duplicate(Guid oldGuid, Guid? newParentGuid = default)
        {
            var table_name = tablelookup[oldGuid];

            var _tables = newParentGuid != default ? connection.Query<Relationships>($"SELECT * FROM '{table_name}' WHERE Parent = '{newParentGuid}'") : (IList<Relationships>)Array.Empty<Relationships>();

            Guid guid;
            if (_tables.Count == 0)
            {
                var table = connection.Query<Relationships>($"SELECT * FROM '{table_name}' WHERE Guid = '{oldGuid}'").Single();

                var lastIndex = MaxIndex(table.Parent) + 1;
                guid = Guid.NewGuid();
                tablelookup[guid] = table_name;
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

            yield return new(oldGuid, guid);
            string query = $"SELECT * FROM '{table_name}' WHERE Parent = '{oldGuid}'";
            var tables = connection.Query<Relationships>(query);

            foreach (var t in tables)
            {
                foreach (var x in Duplicate(t.Guid, guid))
                    yield return x;
            }
        }

        public async Task<Guid> Find(Guid parentGuid, string name, System.Type? type = null, int? index = null)
        {
            await initialisationTask;

            var table_name = tablelookup[parentGuid];

            if (parentGuid == Guid.Empty)
            {

            }
            var typeId = type != null ? (int?)TypeId(type) : null;
            var tables = connection.Query<Relationships>($"SELECT * FROM '{table_name}' WHERE Parent = '{parentGuid}' AND Name = '{name}' AND _Index {ToComparisonAndValue(index)} AND TypeId {ToComparisonAndValue(typeId)}");
            if (tables.Count == 0)
            {
                if (await InsertByParent(parentGuid, name, table_name, typeId, index) is Guid guid)
                    return guid;
                else
                    throw new Exception("* 44 fd3323");
            }
            else if (tables.Count == 1)
            {
                var table = tables.Single();
                tablelookup[table.Guid] = table_name;
                return table.Guid;
            }
            else
            {
                throw new Exception("3e909re 4323");
            }
            throw new Exception("09re 4323");
        }

        public async Task<Guid> InsertByParent(Guid parentGuid, string name, string? table_name = null, int? typeId = null, int? index = null)
        {
            await initialisationTask;
            table_name ??= tablelookup[parentGuid];
            var guid = Guid.NewGuid();
            tablelookup[guid] = table_name;
            //var i = connection.Insert(new Relationships { Guid = guid, Name = name, _Index = index, Parent = parentGuid, Added = DateTime.Now, TypeId = typeId });
            var query = $"INSERT INTO {table_name} (Guid, Name, _Index, Parent, Added, TypeId) VALUES('{guid}', '{name}', {ToValue(index)}, '{parentGuid}', '{DateTime.Now}', {ToValue(typeId)});";
            var i = connection.Execute(query);
            return guid;
        }


        //public Guid? InsertByParent(Guid parentGuid, string name, string table_name, int? typeId = null, int? index = null)
        //{
        //    var tables = connection.Query<Relationships>($"SELECT * FROM '{table_name}' WHERE Parent = '{parentGuid}' AND Name = '{name}' AND _Index {ToComparisonAndValue(index)} AND TypeId {ToComparisonAndValue(typeId)}");
        //    if (tables.Count != 0)
        //        return null;
        //    var guid = Guid.NewGuid();
        //    tablelookup[guid] = table_name;
        //    //var i = connection.Insert(new Relationships { Guid = guid, Name = name, _Index = index, Parent = parentGuid, Added = DateTime.Now, TypeId = typeId });
        //    var query = $"INSERT INTO {table_name} (Guid, Name, _Index, Parent, Added, TypeId) VALUES('{guid}', '{name}', {ToValue(index)}, '{parentGuid}', '{DateTime.Now}', {ToValue(typeId)});";
        //    var i = connection.Execute(query);
        //    return guid;
        //}

        //public int Insert(Guid guid, string name, System.Type type, Guid parentGuid, int? index = null)
        //{
        //    var table_name = tablelookup[parentGuid];
        //    var typeId = (int)TypeId(type);
        //    var tables = connection.Query<Relationships>($"SELECT * FROM '{table_name}' WHERE Name = '{name}' AND Guid = '{guid}' AND TypeId = '{typeId}' AND Parent {ToComparisonAndValue(parentGuid)} AND _Index {ToComparisonAndValue(index)}");
        //    if (tables.Count != 0)
        //        return 0;
        //    return connection.Insert(new Relationships { Guid = guid, Name = name, _Index = index, Parent = parentGuid, Added = DateTime.Now, TypeId = typeId });
        //}


        public Task<Key> InsertRoot(Guid guid, string name, System.Type type)
        {

            // create table if not exists
            if (connection.Query<String>($"SELECT sql as Name FROM sqlite_schema WHERE type ='table' AND name LIKE '{name}'").Any() == false)
            {
                var statement = connection.FindWithQuery<String>($"SELECT sql as Name FROM sqlite_schema WHERE type ='table' AND name LIKE '{nameof(Relationships)}'");
                var newStatement = statement.Name.Replace($"{nameof(Relationships)}", name);
                connection.Execute(newStatement);
            }

            var typeId = TypeId(type);
            var tables = connection.Query<Relationships>($"SELECT * FROM '{nameof(Relationships)}' WHERE Name = '{name}' AND Guid = '{guid}' AND TypeId = '{typeId}'");

            //this.name = name;
            if (tables.Count > 1)
                throw new Exception("dsf 33p[p[");
            else if (tables.Count == 0)
            {
                connection.Insert(new Relationships { Guid = guid, Name = name, TypeId = typeId });
            }

            tablelookup[guid] = name;
            var all = connection.Query<Relationships>($"SELECT * FROM '{name}'");
            foreach (var item in all)
            {
                tablelookup[item.Guid] = name;
            }

            return Task.FromResult(new Key(guid, default, type, name, 0));
        }



        public int? MaxIndex(Guid parentGuid, string? name = default)
        {
            var proto_name = tablelookup[parentGuid];

            string query = $"SELECT MAX({nameof(Relationships._Index)}) FROM '{proto_name}' WHERE {nameof(Relationships.Parent)} = '{parentGuid}'";
            var index = connection.ExecuteScalar<int?>(query + (name == default ? string.Empty : $"AND {nameof(Relationships.Name)} = '{name}'"));
            return index;
        }

        public void Remove(Guid guid)
        {
            var proto_name = tablelookup[guid];

            var find = connection.FindWithQuery<Relationships>($"SELECT * FROM '{proto_name}' WHERE Guid = '{guid}'");
            find.Removed = DateTime.Now;
            connection.Update(find);

        }

        private static string ToComparisonAndValue(object? value)
        {
            return ToComparison(value) + " " + ToValue(value);
        }

        private static string ToComparison(object? value)
        {
            return value == null ? "is" : $"=";
        }

        private static string ToValue(object? value)
        {
            return value == null ? "null" : $"'{value}'";
        }

        public void Set(Guid guid, object value, DateTime dateTime)
        {
            var text = JsonConvert.SerializeObject(value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            if (connection.Query<Values>($"SELECT * FROM '{nameof(Values)}' WHERE Guid = '{guid}' AND Value = '{text}'").Any() == false)
            {
                var typeId = TypeId(value.GetType());
                connection.InsertOrReplace(new Values { Guid = guid, Value = text, Added = dateTime, TypeId = typeId });
            }
        }

        public System.Type GetType(Guid guid)
        {
            if (values.ContainsKey(guid))
                return values[guid].Value.GetType();

            var table = connection.Find<Values>(guid);
            if (table is Values { Value: { } text, Added: { } added, TypeId: { } typeId })
            {
                System.Type? type = ToType(typeId);
                if (type == null)
                    throw new Exception("sd s389989898");
                return type;
            }
            return null;
        }


        public DateValue? Get(Guid guid)
        {
            if (values.ContainsKey(guid))
                return values[guid];

            var table = connection.Find<Values>(guid);
            if (table is Values { Value: { } text, Added: { } added, TypeId: { } typeId })
            {
                System.Type? type = ToType(typeId);
                if (type == null)
                    throw new Exception("sd s389989898");

                var value = JsonConvert.DeserializeObject(text, type);
                var _value = new DateValue(added, value);
                values.Add(guid, _value);
                return _value;
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

        public System.Type? ToType(int typeId)
        {
            if (types.ContainsKey(typeId))
                return types[typeId];

            var type = connection.Table<Type>().Where(v => v.Id.Equals(typeId)).First();
            var assemblyQualifiedName = Assembly.CreateQualifiedName(type.Assembly, $"{type.Namespace}.{type.Name}");
            var systemType = System.Type.GetType(assemblyQualifiedName);
            types[typeId] = systemType;
            return systemType;
        }

        public int TypeId(System.Type type)
        {
            if (this.types.FirstOrDefault(x => x.Value == type) is { Key: { } key, Value: { } value })
                return key;

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
                var typeId = connection.ExecuteScalar<int>($"SELECT Id FROM '{nameof(Type)}' WHERE Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}' ");
                this.types[typeId] = type;
                return typeId;
            }
            else
            {
                return singleType.Id;
            }
        }


        public static TreeRepository Instance { get; } = new("c:/ProgramData/Utility");
    }
}
