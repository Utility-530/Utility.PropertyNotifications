using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using SQLite;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Reactives;
using Utility.Structs.Repos;
using static System.Environment;
using Guid = System.Guid;

namespace Utility.Repos
{
    public class ValueRepository : TypeRepository, IValueRepository
    {
        private record Values
        {
            public Guid Guid { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public int? TypeId { get; set; }
            public DateTime Added { get; set; }
        }

        private Dictionary<Guid, Dictionary<string, DateValue>> values = new();

        private readonly string valuesTableName;
        Task initialisationTask;

        public ValueRepository(string? dbDirectory, string? valuesTableName = nameof(Values)) : base(dbDirectory)
        {
            initialisationTask = initialise();
            this.valuesTableName = valuesTableName;
        }

        private bool IsInitialised { get; set; }

        private Task initialise()
        {
            if (IsInitialised == false)
            {
                IsInitialised = true;
                var task = new Task(() =>
                {
                    init();
                });
                task.Start(TaskScheduler.FromCurrentSynchronizationContext());
                return task;
            }
            return initialisationTask;

            void init()
            {
                connection.Execute($"CREATE TABLE IF NOT EXISTS \"{valuesTableName}\" (  " +
                    " \"Guid\" varchar(36), " +
                    "\"Name\" varchar,   " +
                    "\"Value\" varchar,    " +
                    "\"TypeId\" integer, " +
                    "\"Added\" datetime);");
                foreach (var value in connection.Query<Values>(SqlQueries.SelectValues(valuesTableName)).ToList())
                {
                    if (value.TypeId.HasValue == false)
                        continue;
                    System.Type? type = ToType(value.TypeId.Value);
                    object? _value;
                    try
                    {
                        _value = JsonConvert.DeserializeObject(value.Value, type, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                    }
                    catch (JsonReaderException ex)
                    {
                        _value = value.Value;
                    }

                    var dateValue = new DateValue(value.Guid, value.Name, value.Added, _value);
                    if (values.ContainsKey(value.Guid) == false)
                    {
                        values[value.Guid] = new() { { value.Name, dateValue } };
                    }
                    else if (values[value.Guid].ContainsKey(value.Name) == false)
                    {
                        values[value.Guid].Add(value.Name, dateValue);
                    }
                }
            }
        }

        public virtual async void Set(Guid guid, string name, object value, DateTime dateTime)
        {
            await initialisationTask;
            string text;
            if (values.ContainsKey(guid) && values[guid].ContainsKey(name) && values[guid][name].Value?.Equals(value) == true)
                return;

            text = JsonConvert.SerializeObject(value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            var query = $"SELECT * FROM '{valuesTableName}' WHERE Guid = ? AND Name = ? AND Value = ?";
            var typeId = FromType(value?.GetType());
            if (connection.Query<Values>(query, guid, name, text).Count == 0)
            {
                connection.Insert(new Values { Guid = guid, Value = text, Name = name, Added = dateTime, TypeId = typeId });
            }
            else
            {
                connection.Execute($"UPDATE '{valuesTableName}' SET Added = '{SQLiteHelper.date()}' WHERE Guid = '{guid}' AND Name = '{name}' AND Value = '{text}'");
            }
            values.Get(guid)[name] = new(guid, name, dateTime, value);
        }

        public virtual IObservable<DateValue> Get(Guid guid, string? name = null)
        {

            return Observable.Create<DateValue>(observer =>
            {
                return initialisationTask.ToObservable().Subscribe(a =>
                {
                    if (values.ContainsKey(guid))
                    {
                        if (name != null)
                        {
                            if (values[guid].ContainsKey(name))
                                observer.OnNext((DateValue)values[guid][name]);
                            else
                                observer.OnNext(new DateValue(guid, name, default, null));
                        }
                        else
                        {
                            foreach (var value in values[guid].Values)
                            {
                                observer.OnNext((DateValue)value);
                            }
                        }
                    }
                    else
                    {
                        observer.OnNext(new DateValue(guid, name, default, null));
                    }
                    observer.OnCompleted();
                });
            });
        }

        public void Copy(Guid guid, Guid newGuid)
        {
            var table = connection.FindWithQuery<Values>("SELECT * FROM ? WHERE Guid = ?", valuesTableName, guid);
            if (table is Values { Value: { } text, TypeId: { } typeId })
            {
                connection.InsertOrReplace(new Values { Guid = newGuid, Value = text, TypeId = typeId });
            }
        }

        public bool ValueExists(Guid guid)
        {
            if (values.ContainsKey(guid))
                return true;
            var tables = connection.Query<Values>($"SELECT * FROM {valuesTableName} WHERE {nameof(Values.Guid)} = ?", guid);
            return tables.Any();
        }
    }

    public static class SqlQueries
    {
        // Generic
        public const string SelectAllFromTable = "SELECT * FROM {0}";

        public const string SelectByGuid = "SELECT * FROM {0} WHERE Guid = ?";
        public const string SelectByParent = "SELECT * FROM {0} WHERE Parent = ?";
        public const string SelectByParentAndName = "SELECT * FROM {0} WHERE Parent = ? AND Name = ? ORDER BY _Index";

        public const string SelectByParentGuidRecursive = @"
        WITH RECURSIVE descendants_inclusive AS (
            SELECT Guid, Parent, Name, TypeId, Removed, 0 as Level
            FROM {0} WHERE Parent = ?
            UNION ALL
            SELECT t.Guid, t.Parent, t.Name, t.TypeId, t.Removed, Level + 1 as Level
            FROM {0} t
            JOIN descendants_inclusive d ON t.Parent = d.Guid
        )
        SELECT Guid, Parent, Name, TypeId, Removed, Level as _Index
        FROM descendants_inclusive
        ORDER BY _Index;";

        // Insert
        public const string InsertRelationship =
            "INSERT INTO {0} (Guid, Name, _Index, Parent, Added, TypeId) VALUES (?, ?, ?, ?, ?, ?)";

        public const string InsertValue =
            "INSERT INTO Values (Guid, Name, Value, TypeId, Added) VALUES (?, ?, ?, ?, ?)";

        // Update
        public const string UpdateRemoved =
            "UPDATE {0} SET Removed = ? WHERE Guid = ?";

        public const string UpdateValue =
            "UPDATE Values SET Added = ? WHERE Guid = ? AND Name = ? AND Value = ?";

        public const string UpdateName =
            "UPDATE {0} SET Name = ? WHERE Guid = ?";

        // Max index
        public const string SelectMaxIndex =
            "SELECT MAX(_Index) FROM {0} WHERE Parent = ?";

        public const string SelectMaxIndexWithName =
            "SELECT MAX(_Index) FROM {0} WHERE Parent = ? AND Name = ?";

        // Schema lookups
        public const string FindTablesWithRemoved =
            "SELECT name FROM sqlite_master WHERE type ='table' AND sql LIKE '%Removed%' AND tbl_name != 'Relationships'";

        public const string SchemaCheck =
            "SELECT sql as Name FROM sqlite_schema WHERE type ='table' AND name LIKE ?";

        public const string SchemaCopyFromRelationships =
            "SELECT sql as Name FROM sqlite_schema WHERE type ='table' AND name LIKE 'Relationships'";

        // Types
        public const string SelectTypeByDefinition =
            "SELECT * FROM Type WHERE Assembly = ? AND Namespace = ? AND Name = ? AND GenericTypeNames IS ?";

        public const string SelectTypeId =
            "SELECT Id FROM Type WHERE Assembly = ? AND Namespace = ? AND Name = ? AND GenericTypeNames IS ?";

        public static string SelectValues(string valuesTableName) =>
            $"SELECT v.* FROM \"{valuesTableName}\" v " +
            $"JOIN (SELECT Guid, Name, MAX(Added) AS MaxAdded " +
            $"FROM \"{valuesTableName}\" " +
            $"GROUP BY Guid, Name) latest " +
            $"ON v.Guid = latest.Guid AND v.Name = latest.Name AND v.Added = latest.MaxAdded;";
    }

    public class TypeRepository
    {
        private record Types
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public string? Assembly { get; set; }
            public string? Namespace { get; set; }
            public string Name { get; set; }
            public string? GenericTypeNames { get; set; }
        }

        protected readonly SQLiteConnection connection;
        private Dictionary<int, System.Type> types = new();

        private bool IsInitialised { get; set; }

        public TypeRepository(string? dbDirectory = null)
        {
            this.connection = connect(dbDirectory);
            initialise();

            static SQLiteConnection connect(string? dbDirectory)
            {
                //if (dbDirectory == null)
                //{
                //    return new SQLiteConnection(":memory:");
                //}     
                if (dbDirectory == null)
                    return new SQLiteConnection("data.sqlite", false);
                else
                {
                    //if file name
                    if (string.IsNullOrEmpty(System.IO.Path.GetExtension(dbDirectory)) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(dbDirectory));
                        return new SQLiteConnection(dbDirectory, false);
                    }
                    // if directory name
                    else
                    {
                        Directory.CreateDirectory(dbDirectory);
                        return new SQLiteConnection(Path.Combine(dbDirectory, "data.sqlite"), false);
                    }
                }
            }

            void initialise()
            {
                if (IsInitialised == false)
                {
                    IsInitialised = true;
                    connection.CreateTable<Types>();
                    foreach (var type in connection.Table<Types>().ToList())
                    {
                        if (types.TryAdd(type.Id, convert(type)) == false)
                        {
                        }
                    }
                }
            }
        }

        public System.Type? ToType(int typeId)
        {
            if (typeId == -1)
                return null;

            if (types.ContainsKey(typeId))
                return types[typeId];

            var type = connection.Table<Types>().Where(v => v.Id.Equals(typeId)).First();
            var systemType = convert(type);

            lock (types)
                types[typeId] = systemType;

            return systemType;
        }

        public int FromType(System.Type? type)
        {
            if (type == null)
                return -1;
            if (this.types.ToArray().FirstOrDefault(x => x.Value == type) is { Key: { } key, Value: { } value })
                return key;
            if (type.GenericTypeArguments.Any())
            {
            }
            int typeId = 0;
            var str = type.GenericTypeArguments.Any() ? string.Join('|', type.GenericTypeArguments.Select(a => TypeSerialization.TypeSerializer.Serialize(a))) : null;
            connection.RunInTransaction(() =>
            {
                connection.Insert(new Types { Assembly = type.Assembly.FullName, Namespace = type.Namespace, Name = type.Name, GenericTypeNames = str });
                var types = connection.Query<Types>($"SELECT * FROM '{nameof(Types)}' WHERE Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}' AND {nameof(Types.GenericTypeNames)} {SQLiteHelper.ToComparisonAndValue(str)}");
                if (types.Count > 1)
                    throw new Exception("fds ");
            });

            typeId = connection.ExecuteScalar<int>($"SELECT Id FROM '{nameof(Types)}' WHERE Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}' AND {nameof(Types.GenericTypeNames)} {SQLiteHelper.ToComparisonAndValue(str)}");
            this.types[typeId] = type;
            return typeId;
        }


        private System.Type convert(Types type)
        {
            try
            {
                var ass = Assembly.Load(type.Assembly);
            }
            catch (Exception ex)
            {
                string assemblyFileName = type.Assembly.Split(',')[0];
                foreach (var file in System.IO.Directory.EnumerateFiles(AssembliesPath, "*.dll"))
                {
                    if (file.Contains(assemblyFileName))
                    {
                        var ass = Assembly.LoadFrom(file);
                        break;
                    }
                }
            }

            string assemblyQualifiedName = Assembly.CreateQualifiedName(type.Assembly, $"{type.Namespace}.{type.Name}");
            var baseType = System.Type.GetType(assemblyQualifiedName);
            var typeArguments = type.GenericTypeNames?.Split('|').Select(a => new TypeSerialization.TypeDeserializer().Deserialize(a)).ToArray();
            System.Type constructedType = typeArguments == null ? baseType : baseType.MakeGenericType(typeArguments);
            if (constructedType == null)
                throw new Exception($"Type, {assemblyQualifiedName},  does not exist");
            return constructedType;
        }

        public static readonly string AssembliesPath = System.IO.Path.Combine(ProgramData, Utility, "Assemblies");

        public const string Utility = nameof(Utility);

        public static readonly string ProgramData = GetFolderPath(SpecialFolder.CommonApplicationData);
    }

    public class TreeRepository : TypeRepository, ITreeRepository
    {
        private Dictionary<Guid, string> tablelookup = new();
        public const string No_Existing_Table_No_Name_To_Create_New_One = "ioioi* 22144 fd3323";
        private readonly Task initialisationTask;

        private record Relationships
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

        public record String
        {
            public string Name { get; set; }
        }

        public TreeRepository(string? dbDirectory = default) : base(dbDirectory)
        {
            connection.CreateTable<Relationships>();
            initialisationTask = Initialise();
        }

        public bool IsInitialised { get; set; }

        public Task Initialise()
        {
            if (IsInitialised == false)
            {
                var task = new Task(() =>
                {
                    foreach (var relationship in connection.Table<Relationships>().ToList())
                    {
                        setName(relationship.Guid, relationship.Name);
                    }
                    IsInitialised = true;
                });
                task.Start(TaskScheduler.FromCurrentSynchronizationContext());
                return task;
            }
            return Task.CompletedTask;
        }

        public virtual IObservable<Changes.Change<Key>> Find(Guid? parentGuid = default, string? name = null, Guid? guid = null, System.Type? type = null, int? index = null)
        {
            return Observable.Create<Changes.Change<Key>>(observer =>
            {
                var typeId = type != null ? (int?)FromType(type) : null;

                if (parentGuid.HasValue == false)
                    if (guid.HasValue)
                        if (findAll(observer, guid.Value, name, typeId, index))
                            return Disposable.Empty;
                        else
                            throw new Exception($"{nameof(guid)} is default");
                    else
                    {
                        foreach(var table in connection.Table<Relationships>())
                        {
                            if (table.TypeId.HasValue)
                                type = ToType(table.TypeId.Value);

                            if (type?.ContainsGenericParameters != false)
                                throw new Exception("Invalid generic type");

                            //setName(table.Guid, table.Name);
                            //if (table_name != null)
                            //    setName(table.Guid, table_name);
                            //else
                            //{
                            //}

                            observer.OnNext(Changes.Change.Add<Key>(new(table.Guid, table.Parent, type, table.Name, table._Index, table.Removed)));
                        }
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }
                //throw new Exception($"{nameof(parentGuid)} is default");

                return
                initialisationTask
                .ToObservable()
                .Subscribe(a =>
                {
                    var table_name = getName(parentGuid.Value);

                    if (parentGuid == Guid.Empty)
                    {
                    }
                    string query = $"SELECT * FROM '{table_name}' WHERE Parent = '{parentGuid}' {includeClause($"AND Guid {SQLiteHelper.ToComparisonAndValue(guid)}", guid)} {includeClause($"AND Name {SQLiteHelper.ToComparisonAndValue(name)}", name)}  {includeClause($"AND _Index {SQLiteHelper.ToComparisonAndValue(index)}", index)}  {includeClause($"AND TypeId {SQLiteHelper.ToComparisonAndValue(typeId)}", type)}";
                    var tables = connection.Query<Relationships>(query);
                    if (tables.Count == 0)
                    {
                        if (string.IsNullOrEmpty(name))
                        {
                            observer.OnNext(Changes.Change.None<Key>());
                            observer.OnCompleted();
                            return;
                        }
                        insertByParent(parentGuid.Value, name, table_name, typeId, index)
                        .Subscribe(a =>
                        {
                            if (a is Guid guid)
                            {
                                setName(guid, table_name);
                                observer.OnNext(Changes.Change.Add(new Key(guid, parentGuid.Value, type, name, index, default)));
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
                        observer.OnNext(Changes.Change.Add(new Key(table.Guid, parentGuid.Value, table.TypeId.HasValue ? ToType(table.TypeId.Value) : null, table.Name, index, table.Removed)));
                        observer.OnCompleted();
                    }
                    else if (name == null)
                    {
                        foreach (var table in tables)
                        {
                            setName(table.Guid, table_name);
                            observer.OnNext(Changes.Change.Add(new Key(table.Guid, parentGuid.Value, table.TypeId.HasValue ? ToType(table.TypeId.Value) : null, table.Name, index, table.Removed)));
                        }
                        observer.OnCompleted();
                    }
                    else if (name != null)
                    {
                        var table = tables.SingleOrDefault(a => a.Name == name) ?? throw new Exception("FD £££££");
                        {
                            setName(table.Guid, table_name);
                            observer.OnNext(Changes.Change.Add(new Key(table.Guid, parentGuid.Value, table.TypeId.HasValue ? ToType(table.TypeId.Value) : null, table.Name, index, table.Removed)));
                        }
                        observer.OnCompleted();
                    }
                    else if (guid.HasValue)
                    {
                        var table = tables.SingleOrDefault(a => a.Guid == guid.Value) ?? throw new Exception("3FD £2ui£££44£");
                        {
                            setName(guid.Value, table_name);
                            observer.OnNext(Changes.Change.Add(new Key(table.Guid, parentGuid.Value, table.TypeId.HasValue ? ToType(table.TypeId.Value) : null, table.Name, index, table.Removed)));
                        }
                        observer.OnCompleted();
                    }
                    else
                    {
                        throw new Exception("3e909re 4323");
                    }
                });
            });

            bool findAll(IObserver<Changes.Change<Key>> observer, Guid guid, string? name = null, int? typeId = null, int? index = null)
            {
                var sql = "SELECT name FROM sqlite_master WHERE type ='table' AND sql LIKE '%Removed%' AND tbl_name != 'Relationships'";
                foreach (var table_name in connection.Query<String>(sql))
                {
                    var tables = connection.Query<Relationships>($"SELECT * FROM '{table_name}' {includeClause($"AND Guid {SQLiteHelper.ToComparisonAndValue(guid)}", guid)} {includeClause($"AND Name {SQLiteHelper.ToComparisonAndValue(name)}", name)}  {includeClause($"AND _Index {SQLiteHelper.ToComparisonAndValue(index)}", index)}  {includeClause($"AND TypeId {SQLiteHelper.ToComparisonAndValue(typeId)}", type)}");

                    if (tables.Count == 0)
                        continue;

                    var table = tables.Single();

                    setName(table.Guid, table_name.Name);
                    observer.OnNext(Changes.Change.Add(new Key(table.Guid, table.Parent, table.TypeId.HasValue ? ToType(table.TypeId.Value) : null, table.Name, index, table.Removed)));
                    observer.OnCompleted();
                    return true;
                }
                return false;
            }

            IObservable<Guid> insertByParent(Guid parentGuid, string name, string? table_name = null, int? typeId = null, int? index = null)
            {
                return Observable.Create<Guid>(observer =>
                {
                    return initialisationTask.ToObservable().Subscribe(a =>
                    {
                        table_name ??= getName(parentGuid);
                        var guid = Guid.NewGuid();
                        setName(guid, table_name);
                        //var i = connection.Insert(new Relationships { Guid = guid, Name = name, _Index = index, Parent = parentGuid, Added = DateTime.Now, TypeId = typeId });
                        var query = $"INSERT INTO '{table_name}' (Guid, Name, _Index, Parent, Added, TypeId) VALUES('{guid}', '{name}', {SQLiteHelper.ToValue(index)}, '{parentGuid}', '{SQLiteHelper.date()}', {SQLiteHelper.ToValue(typeId)});";
                        var i = connection.Execute(query);
                        observer.OnNext(guid);
                    });
                });
            }
        }

        //todo change this to insert - should be able to insert at any point in database-tree
        public IObservable<Key?> InsertRoot(Guid guid, string name, System.Type type)
        {
            return Observable.Create<Key?>(observer =>
            {
                return initialisationTask.ToObservable().Subscribe(a =>
                {
                    var typeId = FromType(type);

                    var notTables = connection.Query<Relationships>($"SELECT * FROM '{nameof(Relationships)}' WHERE Name = '{name}' AND Guid <> '{guid}'  AND TypeId = '{typeId}'");
                    var tables = connection.Query<Relationships>($"SELECT * FROM '{nameof(Relationships)}' WHERE Name = '{name}' AND Guid = '{guid}' AND TypeId = '{typeId}'");
                    //var tables3 = connection.Query<Relationships>($"SELECT * FROM '{nameof(Relationships)}' WHERE Name = '{name}' And _Index {ToComparisonAndValue(index)} AND TypeId = '{typeId}'");
                    int? index = tables.SingleOrDefault()?._Index;
                    if (notTables.Count > 0 && tables.Count == 0)
                    {
                        if (index != null)
                            throw new Exception("F 44433222");
                        index = notTables.Count;
                    }

                    var name_index = name + (index.HasValue ? "_" + index.ToString() : string.Empty);

                    setName(guid, name_index);

                    if (tables.Count > 1)
                        throw new Exception("dsf 33p[p[");
                    else if (tables.Count == 0)
                    {
                        // create table if not exists
                        if (connection.Query<String>($"SELECT sql as Name FROM sqlite_schema WHERE type ='table' AND name LIKE '{name_index}'").Any() == false)
                        {
                            var statement = connection.FindWithQuery<String>($"SELECT sql as Name FROM sqlite_schema WHERE type ='table' AND name LIKE '{nameof(Relationships)}'");

                            var newStatement = statement.Name.Replace($"{nameof(Relationships)}", name_index);
                            connection.Execute(newStatement);
                        }

                        connection.Insert(new Relationships { Guid = guid, Name = name, TypeId = typeId, Added = DateTime.Now, _Index = index });
                        setName(guid, name);
                        observer.OnNext(new Key(guid, default, type, name, index, null));
                    }
                    else
                    {
                        var all = connection.Query<Relationships>($"SELECT * FROM '{name_index}'");
                        foreach (var item in all)
                        {
                            setName(item.Guid, name);
                        }
                        //return Observable.Return<Key?>(null);
                        observer.OnNext(new Key(guid, default, type, name, notTables.Count == 0 ? null : notTables.Count - 1, null));
                    }
                });
            });
        }

        public IObservable<Guid> FindParent(Guid guid)
        {
            if (guid == default)
                throw new Exception($"{nameof(guid)} is default");

            return Observable.Create<Guid>(observer =>
            {
                return initialisationTask.ToObservable()
                    .Subscribe(a =>
                    {
                        var table_name = getName(guid);
                        var sql = string.Format(SqlQueries.SelectByGuid, table_name);
                        var tables = connection.Query<Relationships>(sql, guid);

                        if (tables.Count == 1)
                        {
                            observer.OnNext(tables.Single().Parent);
                        }
                        else if (tables.Count > 1)
                        {
                            throw new Exception("Multiple parents found");
                        }
                    });
            });
        }

        public IEnumerable<Duplication> Duplicate(Guid oldGuid, Guid? newParentGuid = default)
        {
            var table_name = getName(oldGuid);
            var query = newParentGuid != default
                ? string.Format(SqlQueries.SelectByParent, table_name)
                : string.Format(SqlQueries.SelectAllFromTable, table_name);

            var _tables = newParentGuid != default
                ? connection.Query<Relationships>(query, newParentGuid.Value)
                : Array.Empty<Relationships>() as IReadOnlyCollection<Relationships>;

            Guid guid;
            if (_tables.Count == 0)
            {
                var row = connection.Query<Relationships>(
                    string.Format(SqlQueries.SelectByGuid, table_name),
                    oldGuid).Single();

                var lastIndex = MaxIndex(row.Parent) + 1;
                guid = Guid.NewGuid();
                setName(guid, table_name);
                connection.Execute(
                    string.Format(SqlQueries.InsertRelationship, table_name),
                    guid, row.Name, lastIndex, newParentGuid ?? row.Parent, DateTime.Now, row.TypeId);
            }
            else if (_tables.Count == 1)
            {
                guid = _tables.Single().Guid;
            }
            else
            {
                throw new Exception("Multiple duplicates found");
            }

            yield return new(oldGuid, guid);

            var childQuery = string.Format(SqlQueries.SelectByParent, table_name);
            var children = connection.Query<Relationships>(childQuery, oldGuid);
            foreach (var t in children)
            {
                foreach (var x in Duplicate(t.Guid, guid))
                    yield return x;
            }
        }


        public int? MaxIndex(Guid parentGuid, string? name = default)
        {
            if (tablelookup.TryGetValue(parentGuid, out string? value) == false)
            {
                return null;
            }

            string query = $"SELECT MAX({nameof(Relationships._Index)}) FROM '{value}' WHERE {nameof(Relationships.Parent)} = '{parentGuid}'";
            var index = connection.ExecuteScalar<int?>(query + (name == default ? string.Empty : $"AND {nameof(Relationships.Name)} = '{name}'"));
            return index;
        }

        public DateTime Remove(Guid guid)
        {
            string table_name;
            // need to check if removing a root since getName will return wrong value for root values
            var x = connection.FindWithQuery<Relationships>($"SELECT * FROM {nameof(Relationships)} WHERE {nameof(Relationships.Guid)} = ?", guid);
            if (x != null)
            {
                table_name = nameof(Relationships);
            }
            else
                table_name = getName(guid);
            var _date = DateTime.Now;
            string cmd = $"UPDATE '{table_name}' SET Removed = '{SQLiteHelper.date(_date)}' WHERE Guid = '{guid}'";
            connection.Execute(cmd);
            return _date;
        }

        public System.Type? GetType(Guid guid, string tableName)
        {
            var tables = connection.Query<Relationships>($"SELECT * FROM '{tableName}' WHERE Guid = '{guid}'");

            var single = tables.SingleOrDefault()?.TypeId;

            if (single.HasValue)
            {
                return ToType(single.Value);
            }
            return null;
        }

        //todo combine this with find with switch for recursive
        public virtual IObservable<Key> FindRecursive(Guid parentGuid, int? maxIndex = null)
        {
            return Observable.Create<Key>(observer =>
            {
                return initialisationTask.ToObservable()
                    .Subscribe(a =>
                    {
                        var table_name = getName(parentGuid);
                        var stmt = string.Format(SqlQueries.SelectByParentGuidRecursive, table_name);
                        var rows = connection.Query<Relationships>(stmt, parentGuid);

                        foreach (var row in rows)
                        {
                            setName(row.Guid, table_name);
                            observer.OnNext(new Key(
                                row.Guid,
                                row.Parent,
                                row.TypeId.HasValue ? ToType(row.TypeId.Value) : null,
                                row.Name,
                                row._Index,
                                row.Removed));
                        }
                        observer.OnCompleted();
                    });
            });
        }


        private string includeClause<T>(string clause, T? ss = default)
        {
            return ss == null ? string.Empty : clause;
        }

        private void setName(Guid guid, string name)
        {
            //lock (tablelookup)
            tablelookup[guid] = name;
        }

        private string getName(Guid guid)
        {
            if (tablelookup.TryGetValue(guid, out string value))
            {
                return value;
            }

            throw new Exception("Have you created a root?");
        }

        public void Reset()
        {
        }

        public void UpdateName(Guid parentGuid, Guid guid, string name, string newName)
        {
            var table_name = getName(parentGuid);
            connection.Execute($"UPDATE {table_name} SET {nameof(Relationships.Name)} = '{newName}' WHERE Guid = '{guid}'");
            //setName(guid, newName);
        }

        public static readonly string DataPath = System.IO.Path.Combine(ProgramData, Utility);

        public static TreeRepository Instance { get; } = new("../../../Data");

        private static readonly Dictionary<string, TreeRepository> dictionary = new();
    }

    internal static class SQLiteHelper
    {
        public static string date(DateTime? date = null)
        {
            //return (date ?? DateTime.Now).ToString("'yyyy-MM-dd HH:mm:ss'");
            return (date ?? DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ss.fff");
        }

        public static string ToComparisonAndValue(object? value)
        {
            return ToComparison(value) + " " + ToValue(value);
        }

        public static string ToComparison(object? value)
        {
            return value == null ? "is" : $"=";
        }

        public static string ToValue(object? value)
        {
            return value == null ? "null" : $"'{value}'";
        }
    }
}