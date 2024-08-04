using SQLite;
using System.Globalization;
using System.Reflection;
using Utility.Conversions;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using static Utility.Repos.SqliteRepository;
using _Key = Utility.Models.Key;

namespace Utility.Repos
{

    public partial class SqliteRepository : IRepository
    {
        public record Table
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            public Guid Guid { get; set; }

            public Guid? Parent { get; set; }

            public string Name { get; set; }

            public int Type { get; set; }
        }

        public record Type
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public string? Assembly { get; set; }
            public string? Namespace { get; set; }
            public string Name { get; set; }
        }

        public record Property
        {
            [PrimaryKey]
            public Guid Guid { get; set; }
            public DateTime Added { get; set; }
            public DateTime? Removed { get; set; }
            public string Value { get; set; }
        }

        protected readonly SQLiteAsyncConnection connection;
        private Task initialisationTask;

        public SqliteRepository(string? dbDirectory = default)
        {
            Directory.CreateDirectory(dbDirectory);
            connection = new SQLiteAsyncConnection(Path.Combine(dbDirectory ?? string.Empty, "data" + "." + "sqlite"));
            Initialise();
        }

        public SqliteRepository(DatabaseDirectory dbDirectory) : this(dbDirectory.Path)
        {
        }

        public IEquatable Key => new Key<SqliteRepository>(Guids.SQLite);


        private void Initialise()
        {
            initialisationTask = Task.WhenAll(
             new[]{
                 connection.CreateTableAsync<Table>(),
                 connection.CreateTableAsync<Type>()
             });
        }

        public async Task Update(IEquatable key, object value)
        {
            if (key is not IGuid { Guid: var guid } _key)
            {
                throw new Exception("reg 43cs ");
            }

            await initialisationTask;

            var tables = await connection.Table<Table>().Where(v => v.Guid.Equals(guid)).ToListAsync();

            if (tables.Count == 1)
            {
                var single = tables.Single();
                var id = single.Id;
                var tableName = "T" + id;

                await connection.RunInTransactionAsync(c =>
                {

                    if (value is Key { Guid: Guid valueGuid, Name: var name } && valueGuid == guid)
                    {
                        if (single.Name != name)
                            c.Update(single with { Name = name });
                        return;
                    }
                    if (value == null)
                    {
                        c.Delete<Table>(id);
                        c.Execute($"DROP TABLE IF EXISTS '{tableName}'");
                        return;
                    }
                    var count = c.ExecuteScalar<int>($"SELECT RowId FROM sqlite_master WHERE type = 'table' AND name = '{tableName}'");

                    if (count == 0)
                    {
                        c.Execute($"Create Table {tableName} (Guid GUID PRIMARY KEY, Added DateTime, Removed DateTime, Value Text)");
                    }
                    var lastWithSameValue = c.ExecuteScalar<int>($"Select RowId from '{tableName}' where RowId = (SELECT MAX(RowId) from '{tableName}') And Value = '{value}'");
                    if (lastWithSameValue == 0)
                    {
                        c.Execute($"Update '{tableName}' Set Removed = '{DateTime.Now}' where Removed == null");
                        c.Execute($"INSERT INTO '{tableName}' (Guid,Added,Removed, Value) VALUES('{Guid.NewGuid()}','{DateTime.Now}',null,'{value}')");
                    }
                });
            }
            else if (tables.Count == 0)
            {
                throw new Exception("22v ere 4323");
            }
            else
            {
                throw new Exception("676 ere 4323");
            }
        }


        public async Task<IEquatable[]> FindKeys(IEquatable key)
        {
            if (key is not IGuid { Guid: var parent } _key)
            {
                throw new Exception("reg 43cs ");
            }

            await initialisationTask;

            if (key is _Key { Name: null, Type: null })
            {
                var tables = await connection.QueryAsync<Table>($"Select * from 'Table' where Parent = '{parent}'");
                List<IEquatable> childKeys = new();
                foreach (var table in tables)
                {
                    var types = await connection.QueryAsync<Type>($"Select * from 'Type' where Id = '{table.Type}'");
                    var singleType = types.Single();
                    var clrType = TypeHelper.ToType(singleType.Assembly, singleType.Namespace, singleType.Name);
                    var childKey = new _Key(table.Guid, table.Name, clrType);
                    childKeys.Add(childKey);
                }
                return childKeys.ToArray();
            }

            if (key is _Key { Name: var name, Type: System.Type type })
            {
                if (name == null)
                {
                    var types = await connection.QueryAsync<Type>($"Select * from 'Type' where Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}'");
                    var singleType = types.SingleOrDefault();
                    if (singleType == default)
                        return Array.Empty<IEquatable>();
                    var tables = await connection.QueryAsync<Table>($"Select * from 'Table' where Parent = '{parent}' AND Type = '{singleType.Id}'");
                    List<IEquatable> childKeys = new();
                    foreach (var table in tables)
                    {
                        var clrType = TypeHelper.ToType(singleType.Assembly, singleType.Namespace, singleType.Name);
                        var childKey = new _Key(table.Guid, table.Name, clrType);
                        childKeys.Add(childKey);
                    }
                    return childKeys.ToArray();
                }
                else
                {
                    var tables = await connection.QueryAsync<Table>($"Select * from 'Table' where Parent = '{parent}' AND Name = '{name}'");
                    if (tables.Count == 0)
                    {
                        //throw new Exception("2241!43 ere 4323");
                        var guid = Guid.NewGuid();
                        //var max = await connection.ExecuteScalarAsync<int>("SELECT MAX(Id) FROM Table");
                        await connection.RunInTransactionAsync(c =>
                        {
                            var tables = c.Query<Table>($"Select * from 'Table' where Parent = '{parent}' AND Name = '{name}'");
                            if (tables.Count != 0)
                                return;
                            var types = c.Query<Type>($"Select * from 'Type' where Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}'");
                            int typeId;

                            if (types.Count == 0)
                            {
                                c.Insert(new Type { Assembly = type.Assembly.FullName, Namespace = type.Namespace, Name = type.Name });
                                typeId = c.ExecuteScalar<int>("Select Max(Id) from 'Type'");
                            }
                            else if (types.Count == 1)
                            {
                                typeId = types.Single().Id;
                            }
                            else
                                throw new Exception("f 434 4");

                            var i = c.Insert(new Table { Guid = guid, Name = name, Parent = parent, Type = typeId });
                        });
                        return new[] { new _Key(guid, name, type) };
                    }
                    if (tables.Count == 1)
                    {
                        var table = tables.Single();
                        return new[] { new _Key(table.Guid, name, type) };
                    }
                    else
                    {
                        throw new Exception("3e909re 4323");
                    }
                }
            }
            throw new Exception(";d;d 3e9d 3209re 4323");
        }

        public async Task<object?> FindValue(IEquatable key)
        {
            if (key is not IGuid { Guid: var guid } _key)
            {
                throw new Exception("reg 43cs ");
            }

            await initialisationTask;
            var tables = await connection.Table<Table>().Where(v => v.Guid.Equals(guid)).ToListAsync();

            if (tables.Count == 0)
            {
                throw new Exception($"!43 ere 4323 {key}");
            }
            else if (tables.Count == 1)
            {
                var table = tables.Single();
                var tableName = $"T{tables.Single().Id}";
                var count = await connection.ExecuteScalarAsync<int>($"SELECT RowId FROM sqlite_master WHERE type = 'table' AND name = '{tableName}'");

                if (count == 0)
                {
                    return null;
                }

                var type = await connection.Table<Type>().Where(v => v.Id.Equals(table.Type)).FirstAsync();
                string assemblyQualifiedName = Assembly.CreateQualifiedName(type.Assembly, $"{type.Namespace}.{type.Name}");
                var _type = System.Type.GetType(assemblyQualifiedName);
                //await connection.InsertOrReplaceAsync(new KeyValue { Id = x.Single().Id, Key = key, Value = JsonConvert.SerializeObject(value) });
                var properties = await connection.QueryAsync<Property>($"Select * from '{tableName}' where Removed is null order by Added asc");
                {
                    List<object> list = new();
                    foreach (var property in properties)
                    {
                        if (ConversionHelper.TryChangeType(property.Value, _type, CultureInfo.CurrentCulture, out var value))
                            list.Add(value);
                        else
                            throw new Exception("332 b64ere 4323");
                    }
                    return list.LastOrDefault() ?? null;
                }
            }
            else
            {
                throw new Exception("ere 4323");
            }
        }
    }
}