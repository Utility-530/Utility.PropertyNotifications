using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using SQLite;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Generic;
using Utility.PropertyNotifications;
using Utility.Reactives;

namespace Utility.Persists
{
    public record Meta
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        [Indexed]
        public DateTime Added { get; set; }
        [Indexed]
        public DateTime Removed { get; set; }
        [Indexed]
        public DateTime LastUpdated { get; set; }
        [Indexed]
        public int Type { get; set; }
    }

    public record DataType
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Type { get; set; }
    }

    public static class DatabaseHelper
    {
        public static IDisposable ToManager<TCollection>(this TCollection observableCollection, Func<object, Guid> funcId, string? dbPath = null) where TCollection : IList, INotifyCollectionChanged
        {
            string path = "../../../Data/models.sqlite";
            //string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            //string path = Path.Combine(userProfile, "Data", "models.sqlite");

            return new SqliteManager<TCollection>(dbPath ?? path, observableCollection, funcId).Subscribe(observableCollection.GetType().InnerType());
        }
    }

    public class SqliteManager<TCollection>(string dbPath, TCollection collection, Func<object, Guid> funcId) where TCollection : IList
    {
        private const string extantItems = "SELECT * FROM '{0}' AS t LEFT JOIN (SELECT * from {1} WHERE Type = '{2}' AND Removed = '0') AS m ON t.Id = m.Id";
        private const string removedItems = "SELECT * FROM '{0}' AS t JOIN (SELECT * from {1} WHERE Type = '{2}' AND Removed != '0') AS m ON t.Id = m.Id";

        private List<DataType> types = new();

        public IDisposable Subscribe(Type type)
        {
            Directory.CreateDirectory(Directory.GetParent(dbPath).FullName);
            var context = SynchronizationContext.Current;
            return Task.Run(() =>
            {
                using var conn = new SQLiteConnection(dbPath, true);
                conn.CreateTable(type, CreateFlags.ImplicitPK);
                conn.CreateTable<Meta>();
                conn.CreateTable<DataType>();

                types = conn.Table<DataType>().ToList();
                var typeString = TypeSerialization.TypeSerializer.Serialize(type);

                if (types.All(a => a.Type != typeString))
                {
                    var dataType = new DataType { Id = types.Count + 1, Type = typeString };
                    conn.Insert(dataType);
                    types.Add(dataType);
                }
                var typeId = types.Single(a => a.Type == typeString).Id;
                var mapping = conn.TableMappings.Single(a => a.TableName == type.Name);

                var items = conn.Query(mapping, string.Format(extantItems, type.Name, nameof(Meta), typeId));
                var removedItems = conn.Query(mapping, string.Format(SqliteManager<TCollection>.removedItems, type.Name, nameof(Meta), typeId));

                List<object> list = new List<object>();
                List<object> toRemove = new List<object>();
                List<object> toAdd = new List<object>();
                foreach (var c in collection)
                {
                    bool flag = false;
                    foreach (var item in removedItems)
                    {
                        if (funcId(c) == funcId(item))
                        {
                            toRemove.Add(c);
                        }
                    }
                    if (toRemove.Contains(c) == false)
                    {
                        foreach (var item in items)
                        {
                            if (funcId(c) == funcId(item))
                            {
                                flag = true;
                                break;
                            }
                        }

                        if (flag == false)
                            list.Add(c);
                    }
                }

                //foreach (var r in toRemove)
                //    collection.Remove(r);

                InsertBulk(list, type);

                foreach (var item in items)
                {
                    bool flag = false;
                    foreach (var c in collection)
                    {
                        if (funcId(c) == funcId(item))
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (flag == false)
                        toAdd.Add(item);
                }

                context.Post((a) =>
                {
                    if (collection.GetType().GetMethod(nameof(IAddRange<>.AddRange)) is MethodInfo methodInfo)
                    {
                        var itemType = methodInfo.GetParameters()[0].ParameterType.GetGenericArguments()[0];
                        var castMethod = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(itemType);
                        var toAddTyped = castMethod.Invoke(null, new object[] { toAdd });
                        var toRemoveTyped = castMethod.Invoke(null, new object[] { toRemove });
                        var exceptMethod = typeof(Enumerable)
                        .GetMethods()
                        .First(m => m.Name == "Except" && m.GetParameters().Length == 2)
                        .MakeGenericMethod(itemType);

                        var itemsToAdd = exceptMethod.Invoke(null, new object[] { toAddTyped, toRemoveTyped });

                        methodInfo.Invoke(collection, new object[] { itemsToAdd });
                    }
                    else
                        foreach (var item in toAdd.Except(toRemove))
                        {
                            collection.Add(item);
                        }
                    foreach (var item in toRemove)
                    {
                        collection.Remove(item);
                    }

                    foreach (var item in collection)
                    {
                        if (item is INotifyPropertyChanged changed)
                            changed.WhenChanged().Subscribe(a =>
                            {
                                Task.Run(() => Update(item, type));
                            });
                    }

                    if (collection is INotifyCollectionChanged notifyCollection)
                        notifyCollection.Changes()
                            .Subscribe(a =>
                            {
                                Task.Run(() => OnCollectionChanged(a, type));
                            });
                }, null);
            });
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e, Type type)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                InsertBulk(e.NewItems, type);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                DeleteBulk(e.OldItems, type);
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                DeleteBulk(e.OldItems, type);
                InsertBulk(e.NewItems, type);
            }
        }

        private void Insert(object item, Type type)
        {
            InsertBulk(new[] { item }, type);
        }

        private void Update(object item, Type type)
        {
            using var conn = new SQLiteConnection(dbPath, true);
            conn.Update(item);
            var meta = conn.Find<Meta>(funcId(item));
            if (meta == null)
            {
                var typeString = TypeSerialization.TypeSerializer.Serialize(type);
                var typeId = types.Single(a => a.Type == typeString).Id;
                conn.Insert(new Meta { Id = funcId(item), Added = DateTime.Now, Type = typeId });
            }
            else
            {
                conn.Update(meta with { LastUpdated = DateTime.Now });
            }
        }

        private void InsertBulk(IEnumerable items, Type type)
        {
            using var conn = new SQLiteConnection(dbPath, true);
            List<Meta> list = new();
            var typeString = TypeSerialization.TypeSerializer.Serialize(type);
            var typeId = types.Single(a => a.Type == typeString).Id;

            foreach (var item in items)
            {
                if (item is INotifyPropertyChanged changed)
                    changed.WhenChanged().Subscribe(a =>
                    {
                        Update(item, type);
                    });
                list.Add(new Meta { Id = funcId(item), Added = DateTime.Now, Type = typeId });
            }
            conn.BeginTransaction();

            conn.InsertAll(items);
            conn.InsertAll(list);
            conn.Commit();
        }

        private void Delete(object item, Type type)
        {
            DeleteBulk(new[] { item }, type);
            //conn.Delete(item);
        }

        private void DeleteBulk(IEnumerable items, Type type)
        {
            using var conn = new SQLiteConnection(dbPath, true);
            //var mapping = conn.TableMappings.Single(a => a.TableName == nameof(Meta));
            foreach (var item in items)
            {
                var id = funcId(item);
                var meta = conn.Find<Meta>(id);
                conn.Update(meta with { Removed = DateTime.Now });
            }
            //conn.R(mapping, items);
        }
    }
}