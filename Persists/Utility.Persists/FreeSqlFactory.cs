using System;
using System.IO;
using System.Threading;
using FreeSql;
using Utility.Interfaces.NonGeneric;

namespace Utility.Persists
{
    public static class FreeSqlFactory
    {
        private static bool initialised;

        public static void InitialiseSQLite()
        {
            if (initialised)
                return;
            AsyncLocal<IUnitOfWork> asyncUow = new AsyncLocal<IUnitOfWork>();
            Directory.CreateDirectory("../../../Data");
            var fsql = new FreeSql.FreeSqlBuilder()
                      .UseAutoSyncStructure(true)
                      .UseNoneCommandParameter(true)
                      .UseConnectionString(FreeSql.DataType.Sqlite, "data source=../../../Data/FreeSQL.sqlite;max pool size=5")
                      .UseMonitorCommand(null, (umcmd, log) => Console.WriteLine(umcmd.Connection.ConnectionString + ":" + umcmd.CommandText))
                      .UseLazyLoading(true)
                      .UseGenerateCommandParameterWithLambda(true)
                      .Build();
            BaseEntity.Initialization(fsql, () => asyncUow.Value);
            initialised = true;
        }
    }

    public static class QueryEx<T> where T : BaseEntity, IEquatable
    {
        public static DateTime Order(string key)
        {
            var where = BaseEntity.Orm.Select<T>().Where(a => a.Equals(key));
            var match = where.MaxAsync(a => a.UpdateTime);
            if (match.Result == default)
            {
                return where.MaxAsync(a => a.CreateTime).Result;
            }
            return match.Result;
        }
    }
}