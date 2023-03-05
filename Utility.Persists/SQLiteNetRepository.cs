using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using Utility.Persists.Infrastructure;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric.Data;

namespace Utility.Persists
{
    public class Repository_Legacy<T, R, S> : SQLiteNetRepository<T, IConvertible, R, S> where T : new()
    {
        public Repository_Legacy(Func<T, IConvertible> getkey, string? dbname = null) : base(getkey, dbname)
        {
        }
    }

    public class SQLiteRepository<T> : SQLiteNetRepository<T, int, IQuery, IQueryResult> where T : IId<int>, new()
    {
        public SQLiteRepository(string? dbname = null, string? dbDirectory = null) : base(getId: t => t.Id, dbname, dbDirectory)
        {
        }

        public override IQueryResult FindBy(IQuery query)
        {
            switch (query)
            {
                case MaxRowId:
                    return new MaxRowIdResult(connection.Table<T>().Length(), true);

                default:
                    throw new Exception("SD£vf fdsgff");
            }
        }
    }

    public class SQLiteNetRepository<T, TId, R, S> : IDisposable, IRepository<T, R, S> where T : new()// createtable needs T:new()
    {
        protected readonly SQLiteConnection connection;
        private static readonly string providerName = "SQLite";
        private readonly Func<T, TId> getId;

        public SQLiteNetRepository(Func<T, TId> getId, string? dbName = null, string? dbDirectory = null)
        {
            this.getId = getId;
            dbDirectory ??= Directory.CreateDirectory(Constants.Default.Directory).FullName;
            dbName ??= DatabaseEx.GetConnectionString(providerName, false);
            connection = new SQLiteConnection(string.IsNullOrEmpty(dbName) || string.IsNullOrWhiteSpace(dbName) ?
                Path.Combine(dbDirectory, typeof(T).Name + "." + Constants.FileExtension.Sqlite) :
                dbName);
            connection.CreateTable<T>();
        }

        public void Dispose()
        {
            connection.Dispose();
        }

        public S Find(T item)
        {
            throw new NotImplementedException();
        }

        public S Add(T item)
        {
            throw new NotImplementedException();
        }

        S IUpdate<T, S>.Update(T item)
        {
            throw new NotImplementedException();
        }

        public S Remove(T item)
        {
            throw new NotImplementedException();
        }

        public S FindMany(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public S AddMany(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public S UpdateMany(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public S RemoveMany(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public virtual S FindBy(R query)
        {
            throw new NotImplementedException();
        }

        public S AddBy(R query)
        {
            throw new NotImplementedException();
        }

        public S UpdateBy(R query)
        {
            throw new NotImplementedException();
        }

        public S RemoveBy(R query)
        {
            throw new NotImplementedException();
        }

        public S FindManyBy(R query)
        {
            throw new NotImplementedException();
        }

        public S AddManyBy(R query)
        {
            throw new NotImplementedException();
        }

        public S UpdateManyBy(R query)
        {
            throw new NotImplementedException();
        }

        public S RemoveManyBy(R query)
        {
            throw new NotImplementedException();
        }

        //public ICollection<T> FromDb<T>(string name) where T: IChild, new()
        //{
        //    return UtilityDAL.SqliteEx.FromDb<T>();

        //}

        //public bool ToDb<T>(ICollection<T> lst, string name) where T : IChild, new()
        //{
        //    return UtilityDAL.SqliteEx.ToDb<T>(lst);
        //}

        //public List<string> SelectIds()
        //{
        //    return System.IO.Directory.GetFiles(dbName).Select(_ => System.IO.Path.GetFileNameWithoutExtension(_)).ToList();
        //}
    }
}