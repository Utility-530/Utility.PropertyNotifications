using Newtonsoft.Json;
using SQLite;
using System.Reflection;
using Utility.PropertyNotifications;

namespace Utility.Nodes.Reflections
{
    public class ValueRepository
    {
        public record Table
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
        }

        //private readonly SQLiteAsyncConnection connection;
        private readonly SQLiteConnection connection;
        private readonly Task initialisationTask;

        private ValueRepository(string? dbDirectory = default)
        {
            if (dbDirectory != default)
                Directory.CreateDirectory(dbDirectory);
            //connection = new SQLiteAsyncConnection(Path.Combine(dbDirectory ?? string.Empty, "value_data" + "." + "sqlite"));
            //initialisationTask = Task.WhenAll(
            //    new[]{
            //        connection.CreateTableAsync<Table>(),
            //        connection.CreateTableAsync<Type>(),
            //    });

            connection = new SQLiteConnection(Path.Combine(dbDirectory ?? string.Empty, "value_data" + "." + "sqlite"));
            initialisationTask = Task.WhenAll(
                new[]{
                    Task.Run(()=>connection.CreateTable<Table>()),
                    Task.Run(()=>connection.CreateTable<Type>()),
                });
        }

        //public async void Register(Guid guid, INotifyPropertyCalled propertyCalled)
        //{
        //    await initialisationTask;
        //    propertyCalled
        //        .WhenCalled()
        //        .Subscribe(async a =>
        //        {
        //            var table = await connection.FindAsync<Table>(guid);
        //            if (table is Table { Value: { } text, TypeId: { } typeId })
        //            {
        //                System.Type? type = await Type(typeId);
        //                if (type == null)
        //                    throw new Exception("sd s389989898");

        //                var value = JsonConvert.DeserializeObject(text, type);

        //                if (value.Equals(a.Value) == false)
        //                    if (propertyCalled is IRaisePropertyChanged changed)
        //                    {
        //                        changed.RaisePropertyChanged(value);
        //                    }
        //            }
        //        });

        //    async Task<System.Type?> Type(int typeId)
        //    {
        //        var type = await connection.Table<Type>().Where(v => v.Id.Equals(typeId)).FirstAsync();
        //        string assemblyQualifiedName = Assembly.CreateQualifiedName(type.Assembly, $"{type.Namespace}.{type.Name}");
        //        return System.Type.GetType(assemblyQualifiedName);
        //    }
        //}

        //public async void Register(Guid guid, INotifyPropertyReceived propertyReceived)
        //{
        //    await initialisationTask;
        //    propertyReceived
        //        .WhenReceived()
        //        .Subscribe(async a =>
        //        {
        //            var typeId = await TypeId(a.Value.GetType());
        //            var text = JsonConvert.SerializeObject(a.Value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        //            await connection.InsertOrReplaceAsync(new Table { Guid = guid, Value = text, TypeId = typeId });
        //        });

        //    async Task<int> TypeId(System.Type type)
        //    {
        //        var types = await connection.QueryAsync<Type>($"Select * from 'Type' where Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}'");
        //        var singleType = types.SingleOrDefault();
        //        if (singleType == default)
        //        {
        //            await connection.RunInTransactionAsync(c =>
        //            {
        //                c.Insert(new Type { Assembly = type.Assembly.FullName, Namespace = type.Namespace, Name = type.Name });
        //                types = c.Query<Type>($"Select * from 'Type' where Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}'");
        //                if (types.Count > 1)
        //                    throw new Exception("fds ");

        //            });

        //            return await connection.ExecuteScalarAsync<int>($"Select Id from 'Type' where Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}' ");
        //        }
        //        else
        //        {
        //            return singleType.Id;
        //        }
        //    }
        //}


        public async void Register(Guid guid, INotifyPropertyCalled propertyCalled)
        {
            await initialisationTask;
            propertyCalled
                .WhenCalled()
                .Subscribe(a =>
                {
                    var table = connection.Find<Table>(guid);
                    if (table is Table { Value: { } text, TypeId: { } typeId })
                    {
                        System.Type? type = Type(typeId);
                        if (type == null)
                            throw new Exception("sd s389989898");

                        var value = JsonConvert.DeserializeObject(text, type);

                        if (value.Equals(a.Value) == false)
                            if (propertyCalled is IRaisePropertyChanged changed)
                            {
                                changed.RaisePropertyChanged(value);
                            }
                    }
                });

            System.Type? Type(int typeId)
            {
                var type = connection.Table<Type>().Where(v => v.Id.Equals(typeId)).First();
                string assemblyQualifiedName = Assembly.CreateQualifiedName(type.Assembly, $"{type.Namespace}.{type.Name}");
                return System.Type.GetType(assemblyQualifiedName);
            }
        }

        public async void Register(Guid guid, INotifyPropertyReceived propertyReceived)
        {
            await initialisationTask;
            propertyReceived
                .WhenReceived()
                .Subscribe(a =>
                {
                    var typeId = TypeId(a.Value.GetType());
                    var text = JsonConvert.SerializeObject(a.Value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                     connection.InsertOrReplace(new Table { Guid = guid, Value = text, TypeId = typeId });
                });

            int TypeId(System.Type type)
            {
                var types = connection.Query<Type>($"Select * from 'Type' where Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}'");
                var singleType = types.SingleOrDefault();
                if (singleType == default)
                {
                    connection.RunInTransaction(() =>
                    {
                        connection.Insert(new Type { Assembly = type.Assembly.FullName, Namespace = type.Namespace, Name = type.Name });
                        types = connection.Query<Type>($"Select * from 'Type' where Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}'");
                        if (types.Count > 1)
                            throw new Exception("fds ");

                    });

                    return connection.ExecuteScalar<int>($"Select Id from 'Type' where Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}' ");
                }
                else
                {
                    return singleType.Id;
                }
            }
        }


        public static ValueRepository Instance { get; } = new("../../../Data");
    }
}
