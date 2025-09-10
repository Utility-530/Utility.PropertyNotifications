using SQLite;
using System.Reactive.Disposables;
using Utility.Interfaces.Exs;
using Utility.Reactives;
using Utility.Structs.Repos;
using static System.Environment;
using Guid = System.Guid;

namespace Utility.Repos
{
    public class TreeRepository : ITreeRepository
    {

        Dictionary<Guid, Dictionary<string, DateValue>> values = new();
        Dictionary<int, System.Type> types = new();
        Dictionary<Guid, string> tablelookup = new();

        public const string No_Existing_Table_No_Name_To_Create_New_One = "ioioi* 22144 fd3323";

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
            public Guid Guid { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public int? TypeId { get; set; }
            public DateTime Added { get; set; }

        }

        public record Type
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public string? Assembly { get; set; }
            public string? Namespace { get; set; }
            public string Name { get; set; }
            public string? GenericTypeNames { get; set; }
        }

        public record String
        {
            public string Name { get; set; }
        }

        private readonly SQLiteConnection connection;
        private readonly Task initialisationTask;

        public TreeRepository(string? dbDirectory = default)
        {
            if (dbDirectory == null)
                connection = new SQLiteConnection("data" + "." + "sqlite", false);
            else
            {

                //if file name
                if (string.IsNullOrEmpty(System.IO.Path.GetExtension(dbDirectory)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(dbDirectory));
                    connection = new SQLiteConnection(dbDirectory, false);
                }
                // if directory name
                else
                {
                    Directory.CreateDirectory(dbDirectory);
                    connection = new SQLiteConnection(Path.Combine(dbDirectory, "data" + "." + "sqlite"), false);
                }
            }

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
                var task = new Task(() =>
                {
                    foreach (var relationship in connection.Table<Relationships>().ToList())
                    {
                        setName(relationship.Guid, relationship.Name);
                    }
                    foreach (var type in connection.Table<Type>().ToList())
                    {
                        types.Add(type.Id, convert(type));
                    }
                    IsInitialised = true;
                });
                task.Start(TaskScheduler.FromCurrentSynchronizationContext());
                return task;
            }
            return Task.CompletedTask;
        }


        public virtual IObservable<IReadOnlyCollection<Key>> SelectKeys(Guid? parentGuid = null, string? name = null, string? table_name = default)
        {
            return Observable.Create<IReadOnlyCollection<Key>>(observer =>
            {
                return initialisationTask.ToObservable().Subscribe(a =>
                {
                    List<Relationships> tables;

                    if (table_name != default)
                    {
                        string query = $"SELECT * FROM '{table_name}'";
                        tables = connection.Query<Relationships>(query);
                    }
                    else if (parentGuid.HasValue)
                    {
                        table_name = getName(parentGuid.Value);
                        string query = $"SELECT * FROM '{table_name}' WHERE Parent = '{parentGuid}'" +
                                        (name == null ? string.Empty : $" AND Name = '{name}'") +
                                        $" ORDER BY {nameof(Relationships._Index)}";
                        tables = connection.Query<Relationships>(query);
                    }
                    else
                    {
                        tables = connection.Table<Relationships>().ToList();
                        int i = 0;
                        foreach (var table in tables)
                        {
                            table._Index = i++;
                        }
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
                    observer.OnNext((IReadOnlyCollection<Key>)selections);
                });
            });
        }

        public bool ValueExists(Guid guid)
        {
            if (values.ContainsKey(guid))
                return true;

            var table = connection.Find<Values>(guid);
            if (table is Values { Value: { } text, Added: { } added, TypeId: { } typeId })
            {
                return true;
            }
            return false;
        }

        public IEnumerable<Duplication> Duplicate(Guid oldGuid, Guid? newParentGuid = default)
        {
            var table_name = getName(oldGuid);

            var _tables = newParentGuid != default ? connection.Query<Relationships>($"SELECT * FROM '{table_name}' WHERE Parent = '{newParentGuid}'") : (IList<Relationships>)Array.Empty<Relationships>();

            Guid guid;
            if (_tables.Count == 0)
            {
                var table = connection.Query<Relationships>($"SELECT * FROM '{table_name}' WHERE Guid = '{oldGuid}'").Single();
                var lastIndex = MaxIndex(table.Parent) + 1;
                guid = Guid.NewGuid();
                setName(guid, table_name);
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

                        if (guid == Guid.Empty)
                        {
                        }

                        //var typeId = type != null ? (int?)TypeId(type) : null;
                        var tables = connection.Query<Relationships>($"SELECT * FROM '{table_name}' WHERE Guid = '{guid}'");
                        if (tables.Count == 0)
                        {
                            //InsertByParent(parentGuid, name, table_name, typeId, index)
                            //.Subscribe(a =>
                            //{
                            //    if (a is Guid guid)
                            //    {
                            //        setName(guid, table_name);
                            //        observer.OnNext(guid);
                            //    }
                            //    else
                            //        throw new Exception("* 44 fd3323");
                            //    observer.OnCompleted();
                            //});
                        }
                        else if (tables.Count == 1)
                        {
                            observer.OnNext(tables.Single().Parent);
                        }
                        else
                        {
                            throw new Exception("3e909re 4323");
                        }
                        //throw new Exception("09re 4323");
                    });
            });
        }

        public virtual IObservable<Key> FindRecursive(Guid parentGuid, int? maxIndex = null)
        {
            return Observable.Create<Key>(observer =>
            {
                return
                    initialisationTask.ToObservable()
                    .Subscribe(a =>
                    {
                        var table_name = getName(parentGuid);
                        var stmt =
                        "WITH RECURSIVE descendants_inclusive AS (\r\n" +
                        "SELECT Guid, Parent, Name, TypeId, Removed, 0  as Level    \r\n" +
                        $"FROM {table_name} WHERE Parent = '{parentGuid.ToString()}'    \r\n    " +
                        "UNION ALL       \r\n\t" +
                        "SELECT t.Guid, t.Parent, t.Name, t.TypeId, t.Removed, Level + 1  as Level  \r\n\t" +
                        $"FROM {table_name} t    \r\n\t" +
                        $"JOIN descendants_inclusive d \r\n\t" +
                        "ON t.Parent = d.Guid)\r\n\t" +
                        "SELECT Guid, Parent, Name, TypeId, Removed, Level as _Index " +
                        "FROM descendants_inclusive " +
                        "ORDER BY _Index;";

                        var rows = connection.Query<Relationships>(stmt);


                        foreach (var row in rows)
                        {
                            setName(row.Guid, table_name);
                            observer.OnNext(new Key(row.Guid, row.Parent, row.TypeId.HasValue ? ToType(row.TypeId.Value) : null, row.Name, row._Index, row.Removed));

                        }
                        observer.OnCompleted();

                    });
            });
        }

        public virtual IObservable<Key?> Find(Guid parentGuid, string? name = null, Guid? guid = null, System.Type? type = null, int? index = null)
        {
            return Observable.Create<Key?>(observer =>
            {
                var typeId = type != null ? (int?)TypeId(type) : null;

                if (parentGuid == default)
                    if (guid.HasValue)
                        if (findAll(observer, guid.Value, name, typeId, index))
                            return Disposable.Empty;
                        else
                            throw new Exception($"{nameof(guid)} is default");
                    else
                        throw new Exception($"{nameof(parentGuid)} is default");

                return
                initialisationTask.ToObservable()
                .Subscribe(a =>
                {
                    var table_name = getName(parentGuid);

                    if (parentGuid == Guid.Empty)
                    {

                    }
                    var tables = connection.Query<Relationships>($"SELECT * FROM '{table_name}' WHERE Parent = '{parentGuid}' {includeClause($"AND Guid {ToComparisonAndValue(guid)}", guid)} {includeClause($"AND Name {ToComparisonAndValue(name)}", name)}  {includeClause($"AND _Index {ToComparisonAndValue(index)}", index)}  {includeClause($"AND TypeId {ToComparisonAndValue(typeId)}", type)}");
                    if (tables.Count == 0)
                    {
                        if (string.IsNullOrEmpty(name))
                        {
                            observer.OnNext(null);
                            observer.OnCompleted();
                            return;
                        }
                        InsertByParent(parentGuid, name, table_name, typeId, index)
                        .Subscribe(a =>
                        {
                            if (a is Guid guid)
                            {
                                setName(guid, table_name);
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
                });
            });

            bool findAll(IObserver<Key?> observer, Guid guid, string? name = null, int? typeId = null, int? index = null)
            {
                var sql = "SELECT name FROM sqlite_master WHERE type ='table' AND sql LIKE '%Removed%' AND tbl_name != 'Relationships'";
                foreach (var table_name in connection.Query<String>(sql))
                {
                    var tables = connection.Query<Relationships>($"SELECT * FROM '{table_name}' {includeClause($"AND Guid {ToComparisonAndValue(guid)}", guid)} {includeClause($"AND Name {ToComparisonAndValue(name)}", name)}  {includeClause($"AND _Index {ToComparisonAndValue(index)}", index)}  {includeClause($"AND TypeId {ToComparisonAndValue(typeId)}", type)}");

                    if (tables.Count == 0)
                        continue;

                    var table = tables.Single();

                    setName(table.Guid, table_name.Name);
                    observer.OnNext(new Key(table.Guid, table.Parent, table.TypeId.HasValue ? ToType(table.TypeId.Value) : null, table.Name, index, table.Removed));
                    observer.OnCompleted();
                    return true;
                }
                return false;
            }
        }

        private string includeClause<T>(string clause, T? ss = default)
        {
            return ss == null ? string.Empty : clause;
        }

        public IObservable<Guid> InsertByParent(Guid parentGuid, string name, string? table_name = null, int? typeId = null, int? index = null)
        {
            return Observable.Create<Guid>(observer =>
            {
                return initialisationTask.ToObservable().Subscribe(a =>
                {
                    table_name ??= getName(parentGuid);
                    var guid = Guid.NewGuid();
                    setName(guid, table_name);
                    //var i = connection.Insert(new Relationships { Guid = guid, Name = name, _Index = index, Parent = parentGuid, Added = DateTime.Now, TypeId = typeId });
                    var query = $"INSERT INTO '{table_name}' (Guid, Name, _Index, Parent, Added, TypeId) VALUES('{guid}', '{name}', {ToValue(index)}, '{parentGuid}', '{date()}', {ToValue(typeId)});";
                    var i = connection.Execute(query);
                    observer.OnNext(guid);
                });
            });
        }

        public IObservable<Key?> InsertRoot(Guid guid, string name, System.Type type)
        {
            return Observable.Create<Key?>(observer =>
            {
                return initialisationTask.ToObservable().Subscribe(a =>
                {
                    var typeId = TypeId(type);

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
            var table_name = getName(guid);
            var _date = DateTime.Now;
            string cmd = $"UPDATE '{table_name}' SET Removed = '{date(_date)}' WHERE Guid = '{guid}'";
            connection.Execute(cmd);
            return _date;
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

        public virtual void Set(Guid guid, string name, object value, DateTime dateTime)
        {
            string text;
            if (values.ContainsKey(guid) && values[guid].ContainsKey(name) && values[guid][name].Value?.Equals(value) == true)
                return;

            text = JsonConvert.SerializeObject(value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            var query = $"SELECT * FROM '{nameof(Values)}' WHERE Guid = '{guid}' AND Name = '{name}' AND Value = '{text}'";
            var typeId = TypeId(value?.GetType());
            if (connection.Query<Values>(query).Any() == false)
            {
                connection.Insert(new Values { Guid = guid, Value = text, Name = name, Added = dateTime, TypeId = typeId });
            }
            else
            {
                connection.Execute($"UPDATE '{nameof(Values)}' SET Added = '{date()}' WHERE Guid = '{guid}' AND Name = '{name}' AND Value = '{text}'");
            }
            values.Get(guid)[name] = new(guid, name, dateTime, value);
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

        public virtual IObservable<DateValue> Get(Guid guid, string? name = null)
        {
            return Observable.Create<DateValue>(observer =>
            {
                if (values.ContainsKey(guid) && name != null && values[guid].ContainsKey(name))
                {
                    observer.OnNext((DateValue)values[guid][name]);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                List<Values>? tables = null;
                if (name == null)
                    tables = connection.Query<Values>($"SELECT * FROM 'Values' WHERE Guid = '{guid}' GROUP BY Name HAVING Max({nameof(Values.Added)})");
                else
                    tables = connection.Query<Values>($"SELECT * FROM 'Values' WHERE Guid = '{guid}' AND Name = '{name}' Order By '{nameof(Values.Added)}' Desc Limit 1");


                if (tables.Count != 0)
                {
                    foreach (var table in tables)
                    {
                        if (table is Values { Value: { } text, Added: { } added, Name: { } _name, TypeId: { } typeId })
                        {
                            System.Type? type = ToType(typeId);

                            if (type == null)
                            {
                                observer.OnNext(new DateValue(guid, _name, added, null));
                                continue;
                            }

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
            if (typeId == -1)
                return null;

            if (types.ContainsKey(typeId))
                return types[typeId];

            var type = connection.Table<Type>().Where(v => v.Id.Equals(typeId)).First();
            var systemType = convert(type);

            lock (types)
                types[typeId] = systemType;

            return systemType;
        }

        System.Type convert(Type type)
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

        public int TypeId(System.Type? type)
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
                connection.Insert(new Type { Assembly = type.Assembly.FullName, Namespace = type.Namespace, Name = type.Name, GenericTypeNames = str });
                var types = connection.Query<Type>($"SELECT * FROM '{nameof(Type)}' WHERE Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}' AND {nameof(Type.GenericTypeNames)} {ToComparisonAndValue(str)}");
                if (types.Count > 1)
                    throw new Exception("fds ");
            });

            typeId = connection.ExecuteScalar<int>($"SELECT Id FROM '{nameof(Type)}' WHERE Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}' AND {nameof(Type.GenericTypeNames)} {ToComparisonAndValue(str)}");
            this.types[typeId] = type;
            return typeId;
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

        private string date(DateTime? date = null)
        {
            //return (date ?? DateTime.Now).ToString("'yyyy-MM-dd HH:mm:ss'");
            return (date ?? DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ss.fff");
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

        public const string Utility = nameof(Utility);

        public static readonly string ProgramData = GetFolderPath(SpecialFolder.CommonApplicationData);

        public static readonly string DataPath = System.IO.Path.Combine(ProgramData, Utility);

        public static readonly string AssembliesPath = System.IO.Path.Combine(ProgramData, Utility, "Assemblies");

        public static TreeRepository Instance { get; } = new("../../../Data");

        static readonly Dictionary<string, TreeRepository> dictionary = new();
        //public static TreeRepository Create(string name) => dictionary.GetValueOrCreate(name, () => new TreeRepository(Path.Combine(ProgramData, name)));
    }
}
