using DryIoc.ImTools;
using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Utility.Helpers.Ex;
using Utility.Helpers.Reflection;
using Utility.PropertyNotifications;

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
            Directory.CreateDirectory("../../../Data/");
            return new SqliteManager<TCollection>(dbPath ?? "../../../Data/data.sqlite", observableCollection, funcId).Subscribe(observableCollection.GetType().InnerType());
        }
    }

    public class SqliteManager<TCollection>(string dbPath, TCollection collection, Func<object, Guid> funcId) where TCollection : IList, INotifyCollectionChanged
    {
        const string extantItems = "SELECT * FROM {0} AS t JOIN (SELECT * from {1} WHERE Type = '{2}' AND Removed = '0') AS m ON t.Id = m.Id";
        const string removedItems = "SELECT * FROM {0} AS t JOIN (SELECT * from {1} WHERE Type = '{2}' AND Removed != '0') AS m ON t.Id = m.Id";

        List<DataType> types = new List<DataType>();

        public IDisposable Subscribe(Type type)
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

            foreach (var r in toRemove)
                collection.Remove(r);

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
                    collection.Add(item);
            }

            foreach (var item in collection)
            {
                if (item is INotifyPropertyChanged changed)
                    changed.WhenChanged().Subscribe(a =>
                    {
                        Update(item, type);
                    });
            }

            return collection.SelectChanges().Subscribe(a =>
            {
                OnCollectionChanged(a, type);
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
            conn.Update(meta with { LastUpdated = DateTime.Now });

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
